#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;
using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Stores the current set of MonitoredItems for a Node.
    /// </summary>
    /// <remarks>
    ///     An instance of this object is created the first time a MonitoredItem is
    /// created for any attribute of a Node. The object is deleted when the last
    ///     MonitoredItem is deleted.
    /// </remarks>
    public class UaMonitoredNode : IUaMonitoredNode
    {
        #region Public Interface
        /// <summary>
        ///     Initializes a new instance of the <see cref="UaMonitoredNode" /> class.
        /// </summary>
        /// <param name="nodeManager">The node manager.</param>
        /// <param name="node">The node.</param>
        public UaMonitoredNode(IUaNodeManager nodeManager, NodeState node)
        {
            NodeManager = nodeManager;
            Node = node;
        }

        /// <summary>
        /// Gets or sets the NodeManager which the MonitoredNode belongs to.
        /// </summary>
        public IUaNodeManager NodeManager { get; set; }

        /// <summary>
        /// Gets or sets the Node being monitored.
        /// </summary>
        public NodeState Node { get; set; }

        /// <summary>
        /// Gets the current list of data change MonitoredItems.
        /// </summary>
        public List<UaMonitoredItem> DataChangeMonitoredItems { get; private set; }

        /// <summary>
        /// Gets the current list of event MonitoredItems.
        /// </summary>
        public List<IUaEventMonitoredItem> EventMonitoredItems { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has monitored items.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has monitored items; otherwise, <c>false</c>.
        /// </value>
        public bool HasMonitoredItems
        {
            get
            {
                if (DataChangeMonitoredItems != null && DataChangeMonitoredItems.Count > 0)
                {
                    return true;
                }

                if (EventMonitoredItems != null && EventMonitoredItems.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Adds the specified data change monitored item.
        /// </summary>
        /// <param name="dataChangeItem">The monitored item.</param>
        public void Add(UaMonitoredItem dataChangeItem)
        {
            if (DataChangeMonitoredItems == null)
            {
                DataChangeMonitoredItems = new List<UaMonitoredItem>();
                Node.OnStateChanged = OnMonitoredNodeChanged;
            }

            DataChangeMonitoredItems.Add(dataChangeItem);
        }

        /// <summary>
        /// Removes the specified data change monitored item.
        /// </summary>
        /// <param name="dataChangeItem">The monitored item.</param>
        public void Remove(UaMonitoredItem dataChangeItem)
        {
            for (var ii = 0; ii < DataChangeMonitoredItems.Count; ii++)
            {
                if (Object.ReferenceEquals(DataChangeMonitoredItems[ii], dataChangeItem))
                {
                    DataChangeMonitoredItems.RemoveAt(ii);
                    break;
                }
            }

            if (DataChangeMonitoredItems.Count == 0)
            {
                DataChangeMonitoredItems = null;
                Node.OnStateChanged = null;
            }
        }

        /// <summary>
        /// Adds the specified event monitored item.
        /// </summary>
        /// <param name="eventItem">The monitored item.</param>
        public void Add(IUaEventMonitoredItem eventItem)
        {
            if (EventMonitoredItems == null)
            {
                EventMonitoredItems = new List<IUaEventMonitoredItem>();
                Node.OnReportEvent = OnReportEvent;
            }

            EventMonitoredItems.Add(eventItem);
        }

        /// <summary>
        /// Removes the specified event monitored item.
        /// </summary>
        /// <param name="eventItem">The monitored item.</param>
        public void Remove(IUaEventMonitoredItem eventItem)
        {
            for (var ii = 0; ii < EventMonitoredItems.Count; ii++)
            {
                if (Object.ReferenceEquals(EventMonitoredItems[ii], eventItem))
                {
                    EventMonitoredItems.RemoveAt(ii);
                    break;
                }
            }

            if (EventMonitoredItems.Count == 0)
            {
                EventMonitoredItems = null;
                Node.OnReportEvent = null;
            }
        }

        /// <summary>
        /// Called when a Node produces an event.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="node">The affected node.</param>
        /// <param name="e">The event.</param>
        public void OnReportEvent(ISystemContext context, NodeState node, IFilterTarget e)
        {
            var eventMonitoredItems = new List<IUaEventMonitoredItem>();

            lock (NodeManager.Lock)
            {
                if (EventMonitoredItems == null)
                {
                    return;
                }

                for (var ii = 0; ii < EventMonitoredItems.Count; ii++)
                {
                    var monitoredItem = EventMonitoredItems[ii];
                    // enqueue event for role permission validation
                    eventMonitoredItems.Add(monitoredItem);
                }
            }

            for (var ii = 0; ii < eventMonitoredItems.Count; ii++)
            {
                var monitoredItem = eventMonitoredItems[ii];

                #region  Filter out audit events in case the Server_Auditing values is false or the channel is not encrypted

                if (e is AuditEventState)
                {
                    // check Server.Auditing flag and skip if false
                    if (!NodeManager.ServerData.Auditing)
                    {
                        continue;
                    }
                    else
                    {
                        // check if channel is not encrypted and skip if so
                        if (monitoredItem?.Session?.EndpointDescription?.SecurityMode != MessageSecurityMode.SignAndEncrypt &&
                            monitoredItem?.Session?.EndpointDescription?.TransportProfileUri != Profiles.HttpsBinaryTransport)
                        {
                            continue;
                        }
                    }
                }
                #endregion

                // validate if the monitored item has the required role permissions to receive the event
                ServiceResult validationResult = NodeManager.ValidateEventRolePermissions(monitoredItem, e);

                if (ServiceResult.IsBad(validationResult))
                {
                    // skip event reporting for EventType without permissions
                    continue;
                }

                lock (NodeManager.Lock)
                {
                    // enqueue event
                    monitoredItem?.QueueEvent(e);
                }
            }
        }

        /// <summary>
        /// Called when the state of a Node changes.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="node">The affected node.</param>
        /// <param name="changes">The mask indicating what changes have occurred.</param>
        public void OnMonitoredNodeChanged(ISystemContext context, NodeState node, NodeStateChangeMasks changes)
        {
            lock (NodeManager.Lock)
            {
                if (DataChangeMonitoredItems == null)
                {
                    return;
                }

                for (var ii = 0; ii < DataChangeMonitoredItems.Count; ii++)
                {
                    var monitoredItem = DataChangeMonitoredItems[ii];

                    if (monitoredItem.AttributeId == Attributes.Value && (changes & NodeStateChangeMasks.Value) != 0)
                    {
                        QueueValue(context, node, monitoredItem);
                        continue;
                    }

                    if (monitoredItem.AttributeId != Attributes.Value && (changes & NodeStateChangeMasks.NonValue) != 0)
                    {
                        QueueValue(context, node, monitoredItem);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        ///     Reads the value of an attribute and reports it to the MonitoredItem.
        /// </summary>
        public void QueueValue(
            ISystemContext context,
            NodeState node,
            UaMonitoredItem monitoredItem)
        {
            var value = new DataValue();

            value.Value = null;
            value.ServerTimestamp = DateTime.UtcNow;
            value.SourceTimestamp = DateTime.MinValue;
            value.StatusCode = StatusCodes.Good;

            var error = node.ReadAttribute(
                context,
                monitoredItem.AttributeId,
                monitoredItem.IndexRange,
                monitoredItem.DataEncoding,
                value);

            if (ServiceResult.IsBad(error))
            {
                value = null;
            }

            monitoredItem.QueueValue(value, error);
        }
        #endregion

        #region Private Fields

        #endregion
    }
}
