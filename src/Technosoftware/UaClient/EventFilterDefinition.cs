#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Defines a event filter for a subscription.
    /// </summary>
    public class EventFilterDefinition
    {
        #region Public Interface
        /// <summary>
        /// The NodeId for the Area that is subscribed to (the entire ServerData if not specified).
        /// </summary>
        public NodeId AreaId;

        /// <summary>
        /// The minimum severity for the events of interest.
        /// </summary>
        public EventSeverity Severity;
        
        /// <summary>
        /// The types for the events of interest.
        /// </summary>
        public IList<NodeId> EventTypes;
        
        /// <summary>
        /// The select clauses to use with the filter.
        /// </summary>
        public SimpleAttributeOperandCollection SelectClauses;
        
        /// <summary>
        /// Creates the monitored item based on the current definition.
        /// </summary>
        /// <returns>The monitored item.</returns>
        public MonitoredItem CreateMonitoredItem()
        {
            // choose the server object by default.
            if (AreaId == null)
            {
                AreaId = ObjectIds.Server;
            }

            // create the item with the filter.
            var monitoredItem = new MonitoredItem
            {
                DisplayName = null,
                StartNodeId = AreaId,
                RelativePath = null,
                NodeClass = NodeClass.Object,
                AttributeId = Attributes.EventNotifier,
                IndexRange = null,
                Encoding = null,
                MonitoringMode = MonitoringMode.Reporting,
                SamplingInterval = 0,
                QueueSize = uint.MaxValue,
                DiscardOldest = true,
                Filter = ConstructFilter(),

                // save the definition as the handle.
                Handle = this
            };

            return monitoredItem;
        }

        /// <summary>
        /// Constructs the select clauses for a set of event types.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventTypeIds">The event type ids.</param>
        /// <returns>The select clauses for all fields discovered.</returns>
        /// <remarks>
        /// Each event type is an ObjectType in the address space. The fields supported by the
        /// server are defined as children of the ObjectType. Many of the fields are manadatory
        /// and are defined by the UA information model, however, indiviudual servers many not 
        /// support all of the optional fields.
        /// 
        /// This method browses the type model and 
        /// </remarks>
        public SimpleAttributeOperandCollection ConstructSelectClauses(
            IUaSession session,
            params NodeId[] eventTypeIds)
        {
            // browse the type model in the server address space to find the fields available for the event type.
            var selectClauses = new SimpleAttributeOperandCollection();

            // must always request the NodeId for the condition instances.
            // this can be done by specifying an operand with an empty browse path.
            var operand = new SimpleAttributeOperand
            {
                TypeDefinitionId = ObjectTypeIds.BaseEventType,
                AttributeId = Attributes.NodeId,
                BrowsePath = new QualifiedNameCollection()
            };

            selectClauses.Add(operand);

            // add the fields for the selected EventTypes.
            if (eventTypeIds != null)
            {
                for (var ii = 0; ii < eventTypeIds.Length; ii++)
                {
                    CollectFields(session, eventTypeIds[ii], selectClauses);
                }
            }

            // use BaseEventType as the default if no EventTypes specified.
            else
            {
                CollectFields(session, ObjectTypeIds.BaseEventType, selectClauses);
            }

            return selectClauses;
        }

        /// <summary>
        /// Constructs the event filter for the subscription.
        /// </summary>
        /// <returns>The event filter.</returns>
        public EventFilter ConstructFilter()
        {
            var filter = new EventFilter
            {
                // the select clauses specify the values returned with each event notification.
                SelectClauses = SelectClauses
            };

            // the where clause restricts the events returned by the server.
            // it works a lot like the WHERE clause in a SQL statement and supports
            // arbitrary expession trees where the operands are literals or event fields.
            var whereClause = new ContentFilter();

            // the code below constructs a filter that looks like this:
            // (Severity >= X OR LastSeverity >= X) AND (SuppressedOrShelved == False) AND (OfType(A) OR OfType(B))

            // add the severity.
            ContentFilterElement element1 = null;
            if (Severity > EventSeverity.Min)
            {
                // select the Severity property of the event.
                var operand1 = new SimpleAttributeOperand
                {
                    TypeDefinitionId = ObjectTypeIds.BaseEventType
                };
                operand1.BrowsePath.Add(BrowseNames.Severity);
                operand1.AttributeId = Attributes.Value;

                // specify the value to compare the Severity property with.
                var operand2 = new LiteralOperand
                {
                    Value = new Variant((ushort)Severity)
                };

                // specify that the Severity property must be GreaterThanOrEqual the value specified.
                element1 = whereClause.Push(FilterOperator.GreaterThanOrEqual, operand1, operand2);
            }

            // add the event types.
            if (EventTypes != null && EventTypes.Count > 0)
            {
                ContentFilterElement element2 = null;

                // save the last element.
                for (var ii = 0; ii < EventTypes.Count; ii++)
                {
                    // for this example uses the 'OfType' operator to limit events to thoses with specified event type. 
                    var operand1 = new LiteralOperand
                    {
                        Value = new Variant(EventTypes[ii])
                    };
                    var element3 = whereClause.Push(FilterOperator.OfType, operand1);

                    // need to chain multiple types together with an OR clause.
                    if (element2 != null)
                    {
                        element2 = whereClause.Push(FilterOperator.Or, element2, element3);
                    }
                    else
                    {
                        element2 = element3;
                    }
                }

                // need to link the set of event types with the previous filters.
                if (element1 != null)
                {
                    whereClause.Push(FilterOperator.And, element1, element2);
                }
            }

            filter.WhereClause = whereClause;

            // return filter.
            return filter;
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Collects the fields for the event type.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventTypeId">The event type id.</param>
        /// <param name="eventFields">The event fields.</param>
        private void CollectFields(IUaSession session, NodeId eventTypeId, SimpleAttributeOperandCollection eventFields)
        {
            // get the supertypes.
            var supertypes = EventUtils.BrowseSuperTypes(session, eventTypeId, false);

            if (supertypes == null)
            {
                return;
            }

            // process the types starting from the top of the tree.
            var foundNodes = new Dictionary<NodeId, QualifiedNameCollection>();
            var parentPath = new QualifiedNameCollection();

            for (var ii = supertypes.Count-1; ii >= 0; ii--)
            {
                CollectFields(session, (NodeId)supertypes[ii].NodeId, parentPath, eventFields, foundNodes);
            }

            // collect the fields for the selected type.
            CollectFields(session, eventTypeId, parentPath, eventFields, foundNodes);
        }

        /// <summary>
        /// Collects the fields for the instance node.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="nodeId">The node id.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="eventFields">The event fields.</param>
        /// <param name="foundNodes">The table of found nodes.</param>
        private void CollectFields(
            IUaSession session,
            NodeId nodeId,
            QualifiedNameCollection parentPath,
            SimpleAttributeOperandCollection eventFields,
            Dictionary<NodeId, QualifiedNameCollection> foundNodes)
        {
            // find all of the children of the field.
            var nodeToBrowse = new BrowseDescription
            {
                NodeId = nodeId,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.Aggregates,
                IncludeSubtypes = true,
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable),
                ResultMask = (uint)BrowseResultMask.All
            };

            var children = EventUtils.Browse(session, nodeToBrowse, false);

            if (children == null)
            {
                return;
            }

            // process the children.
            for (var ii = 0; ii < children.Count; ii++)
            {
                var child = children[ii];

                if (child.NodeId.IsAbsolute)
                {
                    continue;
                }

                // construct browse path.
                var browsePath = new QualifiedNameCollection(parentPath)
                {
                    child.BrowseName
                };

                // check if the browse path is already in the list.
                if (!EventFilterDefinition.ContainsPath(eventFields, browsePath))
                {
                    var field = new SimpleAttributeOperand
                    {
                        TypeDefinitionId = ObjectTypeIds.BaseEventType,
                        BrowsePath = browsePath,
                        AttributeId = (child.NodeClass == NodeClass.Variable) ? Attributes.Value : Attributes.NodeId
                    };

                    eventFields.Add(field);
                }

                // recusively find all of the children.
                var targetId = (NodeId)child.NodeId;

                // need to guard against loops.
                if (!foundNodes.ContainsKey(targetId))
                {
                    foundNodes.Add(targetId, browsePath);
                    CollectFields(session, (NodeId)child.NodeId, browsePath, eventFields, foundNodes);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified select clause contains the browse path.
        /// </summary>
        /// <param name="selectClause">The select clause.</param>
        /// <param name="browsePath">The browse path.</param>
        /// <returns>
        /// 	<c>true</c> if the specified select clause contains path; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsPath(SimpleAttributeOperandCollection selectClause, QualifiedNameCollection browsePath)
        {
            for (var ii = 0; ii < selectClause.Count; ii++)
            {
                var field = selectClause[ii];

                if (field.BrowsePath.Count != browsePath.Count)
                {
                    continue;
                }

                var match = true;

                for (var jj = 0; jj < field.BrowsePath.Count; jj++)
                {
                    if (field.BrowsePath[jj] != browsePath[jj])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
