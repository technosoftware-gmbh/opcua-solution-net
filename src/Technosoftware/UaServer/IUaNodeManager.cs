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
using System.Text;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// An interface to an object that manages a set of nodes in the address space.
    /// </summary>
    public interface IUaNodeManager : IUaBaseNodeManager
    {
        /// <summary>
        /// Gets the server that the node manager belongs to.
        /// </summary>
        IUaServerData ServerData { get; }

        /// <summary>
        /// Acquires the lock on the node manager.
        /// </summary>
        object Lock { get; }

        /// <summary>
        /// Called when the session is closed.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="deleteSubscriptions">if set to <c>true</c> subscriptions are to be deleted.</param>
        void SessionClosing(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions);

        /// <summary>
        /// Returns true if a node is in a view.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="viewId">The view identifier.</param>
        /// <param name="nodeHandle">The node to check.</param>
        /// <returns>True if the node is in the view.</returns>
        bool IsNodeInView(UaServerOperationContext context, NodeId viewId, object nodeHandle);

        /// <summary>
        /// Returns the metadata needed for validating permissions, associated with the node with
        /// the option to optimize services by using a cache.
        /// </summary>
        /// <remarks>
        /// Returns null if the node does not exist.
        /// It should return null in case the implementation wishes to handover the task to the parent INodeManager.GetNodeMetadata
        /// </remarks>
        UaNodeMetadata GetPermissionMetadata(
            UaServerOperationContext context,
            object targetHandle,
            BrowseResultMask resultMask,
            Dictionary<NodeId, List<object>> uniqueNodesServiceAttributesCache,
            bool permissionsOnly);

        /// <summary>
        /// Validates Role permissions for the specified NodeId
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="nodeId"></param>
        /// <param name="requestedPermission"></param>
        /// <returns></returns>
        ServiceResult ValidateRolePermissions(UaServerOperationContext operationContext, NodeId nodeId, PermissionType requestedPermission);

        /// <summary>
        /// Validates if the specified event monitored item has enough permissions to receive the specified event
        /// </summary>
        /// <returns></returns>
        ServiceResult ValidateEventRolePermissions(IUaEventMonitoredItem monitoredItem, IFilterTarget filterTarget);

    }
}
