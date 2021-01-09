#region Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;

using Opc.Ua;
using Technosoftware.UaClient;
#endregion

namespace Technosoftware.ModelDesignClient
{
    /// <summary>
    /// Defines numerous re-useable utility functions.
    /// </summary>
    public static class UtilityFunctions
    {
        /// <summary>
        /// The known event types which can be constructed by ConstructEvent()
        /// </summary>
        public static NodeId[] KnownEventTypes =
        {
            ObjectTypeIds.BaseEventType,
            ObjectTypeIds.ConditionType,
            ObjectTypeIds.DialogConditionType,
            ObjectTypeIds.AlarmConditionType,
            ObjectTypeIds.ExclusiveLimitAlarmType,
            ObjectTypeIds.NonExclusiveLimitAlarmType,
            ObjectTypeIds.AuditEventType,
            ObjectTypeIds.AuditUpdateMethodEventType
        };

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <returns>The best available endpoint.</returns>
        public static EndpointDescription SelectEndpoint(string discoveryUrl, bool useSecurity)
        {
            // needs to add the '/discovery' back onto non-UA TCP URLs.
            if (!discoveryUrl.StartsWith(Utils.UriSchemeOpcTcp))
            {
                if (!discoveryUrl.EndsWith("/discovery"))
                {
                    discoveryUrl += "/discovery";
                }
            }

            // parse the selected URL.
            var uri = new Uri(discoveryUrl);

            // set a short timeout because this is happening in the drop down event.
            var configuration = EndpointConfiguration.Create();
            configuration.OperationTimeout = 5000;

            EndpointDescription selectedEndpoint = null;

            // Connect to the server's discovery endpoint and find the available configuration.
            using (var client = DiscoveryClient.Create(uri, configuration))
            {
                var endpoints = client.GetEndpoints(null);

                // select the best endpoint to use based on the selected URL and the UseSecurity checkbox. 
                foreach (var endpoint in endpoints)
                {
                    // check for a match on the URL scheme.
                    if (endpoint.EndpointUrl.StartsWith(uri.Scheme))
                    {
                        // pick the first available endpoint by default.
                        if (selectedEndpoint == null)
                        {
                            selectedEndpoint = endpoint;
                        }

                        // check if security was requested.
                        if (useSecurity)
                        {
                            if (endpoint.SecurityMode != MessageSecurityMode.None)
                            {
                                // The security level is a relative measure assigned by the server to the 
                                // endpoints that it returns. Clients should always pick the highest level
                                // unless they have a reason not too.
                                if (endpoint.SecurityLevel > selectedEndpoint.SecurityLevel)
                                {
                                    selectedEndpoint = endpoint;
                                }
                            }
                        }

                        // look for an unsecured endpoint if requested.
                        else
                        {
                            if (endpoint.SecurityMode == MessageSecurityMode.None)
                            {
                                selectedEndpoint = endpoint;
                            }
                        }
                    }

                    // pick the first available endpoint by default.
                    if (selectedEndpoint == null && endpoints.Count > 0)
                    {
                        selectedEndpoint = endpoints[0];
                    }
                }
            }

            // if a server is behind a firewall it may return URLs that are not accessible to the client.
            // This problem can be avoided by assuming that the domain in the URL used to call 
            // GetEndpoints can be used to access any of the endpoints. This code makes that conversion.
            // Note that the conversion only makes sense if discovery uses the same protocol as the endpoint.

            if (selectedEndpoint != null)
            {
                var endpointUrl = Utils.ParseUri(selectedEndpoint.EndpointUrl);

                if (endpointUrl != null && endpointUrl.Scheme == uri.Scheme)
                {
                    var builder = new UriBuilder(endpointUrl) { Host = uri.DnsSafeHost, Port = uri.Port };
                    selectedEndpoint.EndpointUrl = builder.ToString();
                }
            }

            // return the selected endpoint.
            return selectedEndpoint;
        }

        /// <summary>
        /// Finds the type of the event for the notification.
        /// </summary>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The NodeId of the EventType.</returns>
        public static NodeId FindEventType(MonitoredItem monitoredItem, EventFieldList notification)
        {
            var filter = monitoredItem.Status.Filter as EventFilter;

            if (filter != null)
            {
                for (var ii = 0; ii < filter.SelectClauses.Count; ii++)
                {
                    var clause = filter.SelectClauses[ii];

                    if (clause.BrowsePath.Count == 1 && clause.BrowsePath[0] == BrowseNames.EventType)
                    {
                        return notification.EventFields[ii].Value as NodeId;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Constructs an event object from a notification.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="monitoredItem">The monitored item that produced the notification.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="eventTypeMappings">Mapping between event types and known event types.</param>
        /// <returns>
        /// The event object. Null if the notification is not a valid event type.
        /// </returns>
        public static BaseEventState ConstructEvent(
            Session session,
            MonitoredItem monitoredItem,
            EventFieldList notification,
            Dictionary<NodeId, NodeId> eventTypeMappings)
        {
            // find the event type.
            var eventTypeId = FindEventType(monitoredItem, notification);

            if (eventTypeId == null)
            {
                return null;
            }

            // look up the known event type.
            NodeId knownTypeId;

            if (!eventTypeMappings.TryGetValue(eventTypeId, out knownTypeId))
            {
                // check for a known type
                if (KnownEventTypes.Any(t => t == eventTypeId))
                {
                    knownTypeId = eventTypeId;
                    eventTypeMappings.Add(eventTypeId, eventTypeId);
                }

                // browse for the supertypes of the event type.
                if (knownTypeId == null)
                {
                    var supertypes = BrowseSuperTypes(session, eventTypeId, false);

                    // can't do anything with unknown types.
                    if (supertypes == null)
                    {
                        return null;
                    }

                    // find the first supertype that matches a known event type.
                    foreach (var t in supertypes)
                    {
                        foreach (var nodeId in KnownEventTypes)
                        {
                            if (nodeId == t.NodeId)
                            {
                                knownTypeId = nodeId;
                                eventTypeMappings.Add(eventTypeId, knownTypeId);
                                break;
                            }
                        }

                        if (knownTypeId != null)
                        {
                            break;
                        }
                    }
                }
            }

            if (knownTypeId == null)
            {
                return null;
            }

            // all of the known event types have a UInt32 as identifier.
            var id = knownTypeId.Identifier as uint?;

            if (id == null)
            {
                return null;
            }

            // construct the event based on the known event type.
            BaseEventState e;

            switch (id.Value)
            {
                case ObjectTypes.ConditionType: { e = new ConditionState(null); break; }
                case ObjectTypes.DialogConditionType: { e = new DialogConditionState(null); break; }
                case ObjectTypes.AlarmConditionType: { e = new AlarmConditionState(null); break; }
                case ObjectTypes.ExclusiveLimitAlarmType: { e = new ExclusiveLimitAlarmState(null); break; }
                case ObjectTypes.NonExclusiveLimitAlarmType: { e = new NonExclusiveLimitAlarmState(null); break; }
                case ObjectTypes.AuditEventType: { e = new AuditEventState(null); break; }
                case ObjectTypes.AuditUpdateMethodEventType: { e = new AuditUpdateMethodEventState(null); break; }

                default:
                    {
                        e = new BaseEventState(null);
                        break;
                    }
            }

            // get the filter which defines the contents of the notification.
            var filter = monitoredItem.Status.Filter as EventFilter;

            // initialize the event with the values in the notification.
            if (filter != null)
            {
                e.Update(session.SystemContext, filter.SelectClauses, notification);
            }

            // save the orginal notification.
            e.Handle = notification;
            return e;
        }

        /// <summary>
        /// Browses the address space and returns the references found.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="nodeToBrowse">The NodeId for the starting node.</param>
        /// <param name="throwOnError">if set to <c>true</c> a exception will be thrown on an error.</param>
        /// <returns>
        /// The references found. Null if an error occurred.
        /// </returns>
        public static ReferenceDescriptionCollection Browse(Session session, BrowseDescription nodeToBrowse, bool throwOnError)
        {
            try
            {
                var references = new ReferenceDescriptionCollection();

                // construct browse request.
                var nodesToBrowse = new BrowseDescriptionCollection { nodeToBrowse };

                // start the browse operation.
                BrowseResultCollection results;
                DiagnosticInfoCollection diagnosticInfos;

                session.Browse(
                    null,
                    null,
                    0,
                    nodesToBrowse,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToBrowse);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

                do
                {
                    // check for error.
                    if (StatusCode.IsBad(results[0].StatusCode))
                    {
                        throw new ServiceResultException(results[0].StatusCode);
                    }

                    // process results.
                    references.AddRange(results[0].References);

                    // check if all references have been fetched.
                    if (results[0].References.Count == 0 || results[0].ContinuationPoint == null)
                    {
                        break;
                    }

                    // continue browse operation.
                    var continuationPoints = new ByteStringCollection { results[0].ContinuationPoint };

                    session.BrowseNext(
                        null,
                        false,
                        continuationPoints,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, continuationPoints);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);
                }
                while (true);

                //return complete list.
                return references;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }

        /// <summary>
        /// Browses the address space and returns all of the supertypes of the specified type node.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="typeId">The NodeId for a type node in the address space.</param>
        /// <param name="throwOnError">if set to <c>true</c> a exception will be thrown on an error.</param>
        /// <returns>
        /// The references found. Null if an error occurred.
        /// </returns>
        public static ReferenceDescriptionCollection BrowseSuperTypes(Session session, NodeId typeId, bool throwOnError)
        {
            var supertypes = new ReferenceDescriptionCollection();

            try
            {
                // find all of the children of the field.
                var nodeToBrowse = new BrowseDescription
                {
                    NodeId = typeId,
                    BrowseDirection = BrowseDirection.Inverse,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = false,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.All
                };

                var references = Browse(session, nodeToBrowse, throwOnError);

                while (references != null && references.Count > 0)
                {
                    // should never be more than one supertype.
                    supertypes.Add(references[0]);

                    // only follow references within this server.
                    if (references[0].NodeId.IsAbsolute)
                    {
                        break;
                    }

                    // get the references for the next level up.
                    nodeToBrowse.NodeId = (NodeId)references[0].NodeId;
                    references = Browse(session, nodeToBrowse, throwOnError);
                }

                // return complete list.
                return supertypes;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }
    }
}
