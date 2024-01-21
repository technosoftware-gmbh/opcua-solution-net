#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace SampleCompany.NodeManagers.SampleNodeManager
{
    /// <summary>
    /// Keeps track of the monitored items for a single node.
    /// </summary>
    public class MonitoredNode
    {
        #region Constructors
        /// <summary>
        /// Initializes the instance with the context for the node being monitored.
        /// </summary>
        public MonitoredNode(
            IUaServerData server,
            IUaBaseNodeManager nodeManager,
            NodeState node)
        {
            server_ = server;
            nodeManager_ = nodeManager;
            node_ = node;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The server that the node belongs to.
        /// </summary>
        public IUaServerData Server
        {
            get { return server_; }
        }

        /// <summary>
        /// The node manager that the node belongs to.
        /// </summary>
        public IUaBaseNodeManager NodeManager
        {
            get { return nodeManager_; }
        }

        /// <summary>
        /// The node being monitored.
        /// </summary>
        public NodeState Node
        {
            get { return node_; }
        }

        /// <summary>
        /// Whether the node has any active monitored items for the specified attribute.
        /// </summary>
        public bool IsMonitoringRequired(uint attributeId)
        {
            if (monitoredItems_ != null)
            {
                for (int ii = 0; ii < monitoredItems_.Count; ii++)
                {
                    DataChangeMonitoredItem monitoredItem = monitoredItems_[ii];

                    if (monitoredItem.AttributeId == attributeId && monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new data change monitored item.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="monitoredItemId">The unique identifier for the monitiored item.</param>
        /// <param name="attributeId">The attribute to monitor.</param>
        /// <param name="indexRange">The index range to use for array values.</param>
        /// <param name="dataEncoding">The data encoding to return for structured values.</param>
        /// <param name="diagnosticsMasks">The diagnostics masks to use.</param>
        /// <param name="timestampsToReturn">The timestamps to return.</param>
        /// <param name="monitoringMode">The initial monitoring mode.</param>
        /// <param name="clientHandle">The handle assigned by the client.</param>
        /// <param name="samplingInterval">The sampling interval.</param>
        /// <param name="queueSize">The queue size.</param>
        /// <param name="discardOldest">Whether to discard the oldest values when the queue overflows.</param>
        /// <param name="filter">The data change filter to use.</param>
        /// <param name="range">The range to use when evaluating a percentage deadband filter.</param>
        /// <param name="alwaysReportUpdates">Whether the monitored item should skip the check for a change in value.</param>
        /// <returns>The new monitored item.</returns>
        public DataChangeMonitoredItem CreateDataChangeItem(
            ISystemContext context,
            uint monitoredItemId,
            uint attributeId,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            DataChangeFilter filter,
            Opc.Ua.Range range,
            bool alwaysReportUpdates)
        {
            DataChangeMonitoredItem monitoredItem = new DataChangeMonitoredItem(
                this,
                monitoredItemId,
                attributeId,
                indexRange,
                dataEncoding,
                diagnosticsMasks,
                timestampsToReturn,
                monitoringMode,
                clientHandle,
                samplingInterval,
                queueSize,
                discardOldest,
                filter,
                range,
                alwaysReportUpdates);

            if (monitoredItems_ == null)
            {
                monitoredItems_ = new List<DataChangeMonitoredItem>();
                node_.OnStateChanged = OnNodeChange;
            }

            monitoredItems_.Add(monitoredItem);

            return monitoredItem;
        }

        /// <summary>
        /// Creates a new data change monitored item.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="monitoredItemId">The unique identifier for the monitiored item.</param>
        /// <param name="attributeId">The attribute to monitor.</param>
        /// <param name="indexRange">The index range to use for array values.</param>
        /// <param name="dataEncoding">The data encoding to return for structured values.</param>
        /// <param name="diagnosticsMasks">The diagnostics masks to use.</param>
        /// <param name="timestampsToReturn">The timestamps to return.</param>
        /// <param name="monitoringMode">The initial monitoring mode.</param>
        /// <param name="clientHandle">The handle assigned by the client.</param>
        /// <param name="samplingInterval">The sampling interval.</param>
        /// <param name="alwaysReportUpdates">Whether the monitored item should skip the check for a change in value.</param>
        /// <returns>The new monitored item.</returns>
        public DataChangeMonitoredItem CreateDataChangeItem(
            ISystemContext context,
            uint monitoredItemId,
            uint attributeId,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            double samplingInterval,
            bool alwaysReportUpdates)
        {
            return CreateDataChangeItem(
                context,
                monitoredItemId,
                attributeId,
                indexRange,
                dataEncoding,
                diagnosticsMasks,
                timestampsToReturn,
                monitoringMode,
                clientHandle,
                samplingInterval,
                0,
                false,
                null,
                null,
                alwaysReportUpdates);
        }

        /// <summary>
        /// Deletes the monitored item.
        /// </summary>
        public void DeleteItem(IUaMonitoredItem monitoredItem)
        {
            if (monitoredItems_ != null)
            {
                for (int ii = 0; ii < monitoredItems_.Count; ii++)
                {
                    if (Object.ReferenceEquals(monitoredItem, monitoredItems_[ii]))
                    {
                        monitoredItems_.RemoveAt(ii);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles change events raised by the node.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="state">The node that raised the event.</param>
        /// <param name="masks">What caused the event to be raised</param>
        public void OnNodeChange(ISystemContext context, NodeState state, NodeStateChangeMasks masks)
        {
            if (monitoredItems_ != null)
            {
                for (int ii = 0; ii < monitoredItems_.Count; ii++)
                {
                    DataChangeMonitoredItem monitoredItem = monitoredItems_[ii];

                    // check if the node has been deleted.
                    if ((masks & NodeStateChangeMasks.Deleted) != 0)
                    {
                        monitoredItem.QueueValue(null, StatusCodes.BadNodeIdUnknown, false);
                        continue;
                    }

                    if (monitoredItem.AttributeId == Attributes.Value)
                    {
                        if ((masks & NodeStateChangeMasks.Value) != 0)
                        {
                            monitoredItem.ValueChanged(context);
                        }
                    }
                    else
                    {
                        if ((masks & NodeStateChangeMasks.NonValue) != 0)
                        {
                            monitoredItem.ValueChanged(context);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to events produced by the node.
        /// </summary>
        public void SubscribeToEvents(ISystemContext context, IUaEventMonitoredItem eventSubscription)
        {
            if (eventSubscriptions_ == null)
            {
                eventSubscriptions_ = new List<IUaEventMonitoredItem>();
            }

            if (eventSubscriptions_.Count == 0)
            {
                node_.OnReportEvent = OnReportEvent;
                node_.SetAreEventsMonitored(context, true, true);
            }

            for (int ii = 0; ii < eventSubscriptions_.Count; ii++)
            {
                if (Object.ReferenceEquals(eventSubscription, eventSubscriptions_[ii]))
                {
                    return;
                }
            }

            eventSubscriptions_.Add(eventSubscription);
        }

        /// <summary>
        /// Unsubscribes to events produced by the node.
        /// </summary>
        public void UnsubscribeToEvents(ISystemContext context, IUaEventMonitoredItem eventSubscription)
        {
            if (eventSubscriptions_ != null)
            {
                for (int ii = 0; ii < eventSubscriptions_.Count; ii++)
                {
                    if (Object.ReferenceEquals(eventSubscription, eventSubscriptions_[ii]))
                    {
                        eventSubscriptions_.RemoveAt(ii);

                        if (eventSubscriptions_.Count == 0)
                        {
                            node_.SetAreEventsMonitored(context, false, true);
                            node_.OnReportEvent = null;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles events reported by the node.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="state">The node that raised the event.</param>
        /// <param name="e">The event to report.</param>
        public void OnReportEvent(ISystemContext context, NodeState state, IFilterTarget e)
        {
            if (eventSubscriptions_ != null)
            {
                for (int ii = 0; ii < eventSubscriptions_.Count; ii++)
                {
                    eventSubscriptions_[ii].QueueEvent(e);
                }
            }
        }

        /// <summary>
        /// Resends the events for any conditions belonging to the node or its children.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="monitoredItem">The item to refresh.</param>
        public void ConditionRefresh(
            ISystemContext context,
            IUaEventMonitoredItem monitoredItem)
        {
            if (eventSubscriptions_ != null)
            {
                for (int ii = 0; ii < eventSubscriptions_.Count; ii++)
                {
                    // only process items monitoring this node.
                    if (!Object.ReferenceEquals(monitoredItem, eventSubscriptions_[ii]))
                    {
                        continue;
                    }

                    // get the set of condition events for the node and its children.
                    List<IFilterTarget> events = new List<IFilterTarget>();
                    node_.ConditionRefresh(context, events, true);

                    // report the events to the monitored item.
                    for (int jj = 0; jj < events.Count; jj++)
                    {
                        monitoredItem.QueueEvent(events[jj]);
                    }
                }
            }
        }
        #endregion

        #region Private Fields
        private IUaServerData server_;
        private IUaBaseNodeManager nodeManager_;
        private NodeState node_;
        private List<IUaEventMonitoredItem> eventSubscriptions_;
        private List<DataChangeMonitoredItem> monitoredItems_;
        #endregion
    }
}
