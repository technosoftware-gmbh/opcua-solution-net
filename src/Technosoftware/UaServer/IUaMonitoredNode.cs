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
using System.Collections.Generic;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Stores the current set of MonitoredItems for a Node.
    /// </summary>
    public interface IUaMonitoredNode
    {
        /// <summary>
        /// Gets or sets the Node being monitored.
        /// </summary>
        NodeState Node { get; set; }

        /// <summary>
        /// Gets the current list of data change MonitoredItems.
        /// </summary>
        List<UaMonitoredItem> DataChangeMonitoredItems { get; }

        /// <summary>
        /// Gets the current list of event MonitoredItems.
        /// </summary>
        List<IUaEventMonitoredItem> EventMonitoredItems { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has monitored items.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has monitored items; otherwise, <c>false</c>.
        /// </value>
        bool HasMonitoredItems { get; }

        /// <summary>
        /// Adds the specified data change monitored item.
        /// </summary>
        /// <param name="dataChangeItem">The monitored item.</param>
        void Add(UaMonitoredItem dataChangeItem);

        /// <summary>
        /// Removes the specified data change monitored item.
        /// </summary>
        /// <param name="dataChangeItem">The monitored item.</param>
        void Remove(UaMonitoredItem dataChangeItem);

        /// <summary>
        /// Adds the specified event monitored item.
        /// </summary>
        /// <param name="eventItem">The monitored item.</param>
        void Add(IUaEventMonitoredItem eventItem);

        /// <summary>
        /// Removes the specified event monitored item.
        /// </summary>
        /// <param name="eventItem">The monitored item.</param>
        void Remove(IUaEventMonitoredItem eventItem);

        /// <summary>
        /// Called when a Node produces an event.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="node">The affected node.</param>
        /// <param name="e">The event.</param>
        void OnReportEvent(ISystemContext context, NodeState node, IFilterTarget e);

        /// <summary>
        /// Called when the state of a Node changes.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="node">The affected node.</param>
        /// <param name="changes">The mask indicating what changes have occurred.</param>
        void OnMonitoredNodeChanged(ISystemContext context, NodeState node, NodeStateChangeMasks changes);

        /// <summary>
        /// Reads the value of an attribute and reports it to the MonitoredItem.
        /// </summary>
        void QueueValue(
            ISystemContext context,
            NodeState node,
            UaMonitoredItem monitoredItem);
    }
}