#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;

using Technosoftware.UaClient;
#endregion

namespace SampleCompany.ReferenceClient
{
    /// <summary>
    /// Defines numerous re-useable utility functions.
    /// </summary>
    public static class EventUtils
    {
        /// <summary>
        /// The known event types which can be constructed by ConstructEvent()
        /// </summary>
        public static NodeId[] KnownEventTypes = new NodeId[]
        {
            // BaseEventType
            ObjectTypeIds.BaseEventType,
            ObjectTypeIds.AuditEventType,
            ObjectTypeIds.BaseModelChangeEventType,
            ObjectTypeIds.ConditionType,
            ObjectTypeIds.EventQueueOverflowEventType,
            ObjectTypeIds.ProgressEventType,
            ObjectTypeIds.SemanticChangeEventType,
            ObjectTypeIds.SystemEventType,
            ObjectTypeIds.TransitionEventType,

            // AuditEventType
            ObjectTypeIds.AuditNodeManagementEventType,
            ObjectTypeIds.AuditSecurityEventType,
            ObjectTypeIds.AuditUpdateEventType,
            ObjectTypeIds.AuditUpdateMethodEventType,

            // BaseModelChangeEventType
            ObjectTypeIds.GeneralModelChangeEventType,

            // ConditionType
            ObjectTypeIds.AcknowledgeableConditionType,
            ObjectTypeIds.DialogConditionType,

            // SystemEventType
            ObjectTypeIds.DeviceFailureEventType,

            // TransitionEventType
            ObjectTypeIds.ProgramTransitionEventType,

            // AcknowledgeableConditionType
            ObjectTypeIds.AlarmConditionType,

            // AlarmConditionType
            ObjectTypeIds.DiscrepancyAlarmType,
            ObjectTypeIds.DiscreteAlarmType,
            ObjectTypeIds.LimitAlarmType,

            // LimitAlarmType
            ObjectTypeIds.ExclusiveLimitAlarmType,
            ObjectTypeIds.NonExclusiveLimitAlarmType,
        };

        /// <summary>
        /// Finds the type of the event for the notification.
        /// </summary>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>The NodeId of the EventType.</returns>
        public static NodeId FindEventType(MonitoredItem monitoredItem, EventFieldList notification)
        {
            EventFilter filter = monitoredItem.Status.Filter as EventFilter;

            if (filter != null)
            {
                for (int ii = 0; ii < filter.SelectClauses.Count; ii++)
                {
                    SimpleAttributeOperand clause = filter.SelectClauses[ii];

                    if (clause.BrowsePath.Count == 1 && clause.BrowsePath[0] == BrowseNames.EventType)
                    {
                        return notification.EventFields[ii].Value as NodeId;
                    }
                }
            }

            return null;
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
                ReferenceDescriptionCollection references = new ReferenceDescriptionCollection();

                // construct browse request.
                BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
                nodesToBrowse.Add(nodeToBrowse);

                // start the browse operation.
                BrowseResultCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

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
                    for (int ii = 0; ii < results[0].References.Count; ii++)
                    {
                        references.Add(results[0].References[ii]);
                    }

                    // check if all references have been fetched.
                    if (results[0].References.Count == 0 || results[0].ContinuationPoint == null)
                    {
                        break;
                    }

                    // continue browse operation.
                    ByteStringCollection continuationPoints = new ByteStringCollection();
                    continuationPoints.Add(results[0].ContinuationPoint);

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
            ReferenceDescriptionCollection supertypes = new ReferenceDescriptionCollection();

            try
            {
                // find all of the children of the field.
                BrowseDescription nodeToBrowse = new BrowseDescription();

                nodeToBrowse.NodeId = typeId;
                nodeToBrowse.BrowseDirection = BrowseDirection.Inverse;
                nodeToBrowse.ReferenceTypeId = ReferenceTypeIds.HasSubtype;
                nodeToBrowse.IncludeSubtypes = false; // more efficient to use IncludeSubtypes=False when possible.
                nodeToBrowse.NodeClassMask = 0; // the HasSubtype reference already restricts the targets to Types. 
                nodeToBrowse.ResultMask = (uint)BrowseResultMask.All;

                ReferenceDescriptionCollection references = Browse(session, nodeToBrowse, throwOnError);

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
            Dictionary<NodeId,NodeId> eventTypeMappings)
        {
            // find the event type.
            NodeId eventTypeId = FindEventType(monitoredItem, notification);

            if (eventTypeId == null)
            {
                return null;
            }

            // look up the known event type.
            NodeId knownTypeId = null;

            if (!eventTypeMappings.TryGetValue(eventTypeId, out knownTypeId))
            {
                // check for a known type
                for (int jj = 0; jj < KnownEventTypes.Length; jj++)
                {
                    if (KnownEventTypes[jj] == eventTypeId)
                    {
                        knownTypeId = eventTypeId;
                        eventTypeMappings.Add(eventTypeId, eventTypeId);
                        break;
                    }
                }
                
                // browse for the supertypes of the event type.
                if (knownTypeId == null)
                {
                    ReferenceDescriptionCollection supertypes = EventUtils.BrowseSuperTypes(session, eventTypeId, false);

                    // can't do anything with unknown types.
                    if (supertypes == null)
                    {
                        return null;
                    }

                    // find the first supertype that matches a known event type.
                    for (int ii = 0; ii < supertypes.Count; ii++)
                    {
                        for (int jj = 0; jj < KnownEventTypes.Length; jj++)
                        {
                            if (KnownEventTypes[jj] == supertypes[ii].NodeId)
                            {
                                knownTypeId = KnownEventTypes[jj];
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
            uint? id = knownTypeId.Identifier as uint?;

            if (id == null)
            {
                return null;
            }

            // construct the event based on the known event type.
            BaseEventState e = null;

            switch (id.Value)
            {
                // BaseEventType
                case ObjectTypes.BaseEventType: { e = new BaseEventState(null); break; }
                case ObjectTypes.AuditEventType: { e = new AuditEventState(null); break; }
                case ObjectTypes.BaseModelChangeEventType: { e = new BaseModelChangeEventState(null); break; }
                case ObjectTypes.ConditionType: { e = new ConditionState(null); break; }
                case ObjectTypes.EventQueueOverflowEventType: { e = new EventQueueOverflowEventState(null); break; }
                case ObjectTypes.ProgressEventType: { e = new ProgressEventState(null); break; }
                case ObjectTypes.SemanticChangeEventType: { e = new SemanticChangeEventState(null); break; }
                case ObjectTypes.SystemEventType: { e = new SystemEventState(null); break; }
                case ObjectTypes.TransitionEventType: { e = new TransitionEventState(null); break; }

                // AuditEventType
                case ObjectTypes.AuditNodeManagementEventType: { e = new AuditNodeManagementEventState(null); break; }
                case ObjectTypes.AuditSecurityEventType: { e = new AuditSecurityEventState(null); break; }
                case ObjectTypes.AuditUpdateEventType: { e = new AuditUpdateEventState(null); break; }
                case ObjectTypes.AuditUpdateMethodEventType: { e = new AuditUpdateMethodEventState(null); break; }

                // BaseModelChangeEventType
                case ObjectTypes.GeneralModelChangeEventType: { e = new GeneralModelChangeEventState(null); break; }

                // ConditionType
                case ObjectTypes.AcknowledgeableConditionType: { e = new AcknowledgeableConditionState(null); break; }
                case ObjectTypes.DialogConditionType: { e = new DialogConditionState(null); break; }

                // SystemEventType
                case ObjectTypes.DeviceFailureEventType: { e = new DeviceFailureEventState(null); break; }

                // TransitionEventType
                case ObjectTypes.ProgramTransitionEventType: { e = new ProgramTransitionEventState(null); break; }

                // AcknowledgeableConditionType
                case ObjectTypes.AlarmConditionType: { e = new AlarmConditionState(null); break; }

                // AlarmConditionType
                case ObjectTypes.DiscrepancyAlarmType: { e = new DiscrepancyAlarmState(null); break; }
                case ObjectTypes.DiscreteAlarmType: { e = new DiscreteAlarmState(null); break; }
                case ObjectTypes.LimitAlarmType: { e = new LimitAlarmState(null); break; }

                // LimitAlarmType
                case ObjectTypes.ExclusiveLimitAlarmType: { e = new ExclusiveLimitAlarmState(null); break; }
                case ObjectTypes.NonExclusiveLimitAlarmType: { e = new NonExclusiveLimitAlarmState(null); break; }

                default:
                {
                    e = new BaseEventState(null);
                    break;
                }
            }

            // get the filter which defines the contents of the notification.
            EventFilter filter = monitoredItem.Status.Filter as EventFilter;

            // initialize the event with the values in the notification.
            e.Update(session.SystemContext, filter.SelectClauses, notification);

            // save the orginal notification.
            e.Handle = notification;

            return e;
        }
    }
}
