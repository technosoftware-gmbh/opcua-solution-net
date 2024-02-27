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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

using Opc.Ua;
using Technosoftware.UaServer.Configuration;
#endregion

namespace Technosoftware.UaServer.Diagnostics
{
    /// <summary>
    /// A node manager the diagnostic information exposed by the server.
    /// </summary>
    public class DiagnosticsNodeManager : UaGenericNodeManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public DiagnosticsNodeManager(
            IUaServerData server,
            ApplicationConfiguration configuration)
        :
            base(server, configuration)
        {
            this.AliasRoot = "Core";

            var namespaceUris = new string[2];
            namespaceUris[0] = Opc.Ua.Namespaces.OpcUa;
            namespaceUris[1] = Opc.Ua.Namespaces.OpcUa + "Diagnostics";
            SetNamespaces(namespaceUris);

            namespaceIndex_ = ServerData.NamespaceUris.GetIndexOrAppend(namespaceUris[1]);
            lastUsedId_ = (long)(DateTime.UtcNow.Ticks & 0x7FFFFFFF);
            sessions_ = new List<SessionDiagnosticsData>();
            subscriptions_ = new List<SubscriptionDiagnosticsData>();
            diagnosticsEnabled_ = true;
            doScanBusy_ = false;
            sampledItems_ = new List<UaMonitoredItem>();
            minimumSamplingInterval_ = 100;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (Lock)
                {
                    Utils.SilentDispose(diagnosticsScanTimer_);
                    diagnosticsScanTimer_ = null;

                    Utils.SilentDispose(samplingTimer_);
                    samplingTimer_ = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns>The new NodeId.</returns>
        public override NodeId Create(ISystemContext context, NodeState node)
        {
            var id = Utils.IncrementIdentifier(ref lastUsedId_);
            return new NodeId(id, namespaceIndex_);
        }
        #endregion

        #region IUaNodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);

                // sampling interval diagnostics not supported by the server.
                var serverDiagnosticsNode = (ServerDiagnosticsState)FindPredefinedNode(
                    ObjectIds.Server_ServerDiagnostics,
                    typeof(ServerDiagnosticsState));

                if (serverDiagnosticsNode != null)
                {
                    NodeState samplingDiagnosticsArrayNode = serverDiagnosticsNode.FindChild(
                        SystemContext,
                        BrowseNames.SamplingIntervalDiagnosticsArray);

                    if (samplingDiagnosticsArrayNode != null)
                    {
                        DeleteNode(SystemContext, VariableIds.Server_ServerDiagnostics_SamplingIntervalDiagnosticsArray);
                        serverDiagnosticsNode.SamplingIntervalDiagnosticsArray = null;
                    }
                }

                // The nodes are now loaded by the DiagnosticsNodeManager from the file
                // output by the ModelDesigner V2. These nodes are added to the CoreNodeManager
                // via the AttachNode() method when the DiagnosticsNodeManager starts.
                ServerData.CoreNodeManager.ImportNodes(SystemContext, PredefinedNodes.Values, true);

                // hook up the server GetMonitoredItems method.
                var getMonitoredItems = (GetMonitoredItemsMethodState)FindPredefinedNode(
                    MethodIds.Server_GetMonitoredItems,
                    typeof(GetMonitoredItemsMethodState));

                if (getMonitoredItems != null)
                {
                    getMonitoredItems.OnCallMethod = OnGetMonitoredItems;
                }

                // set ArrayDimensions for GetMonitoredItems.OutputArguments.Value.
                var getMonitoredItemsOutputArguments = (PropertyState)FindPredefinedNode(
                    VariableIds.Server_GetMonitoredItems_OutputArguments,
                    typeof(PropertyState));

                if (getMonitoredItemsOutputArguments != null)
                {
                    var outputArgumentsValue = (Argument[])getMonitoredItemsOutputArguments.Value;

                    if (outputArgumentsValue != null)
                    {
                        foreach (var argument in outputArgumentsValue)
                        {
                            argument.ArrayDimensions = new UInt32Collection { 0 };
                        }

                        getMonitoredItemsOutputArguments.ClearChangeMasks(SystemContext, false);
                    }
                }

#if SUPPORT_DURABLE_SUBSCRIPTION
                // hook up the server SetSubscriptionDurable method.
                SetSubscriptionDurableMethodState setSubscriptionDurable= (SetSubscriptionDurableMethodState)FindPredefinedNode(
                    MethodIds.Server_SetSubscriptionDurable,
                    typeof(SetSubscriptionDurableMethodState));

                if (setSubscriptionDurable != null)
                {
                    setSubscriptionDurable.OnCall = OnSetSubscriptionDurable;
                }
#else
                // Subscription Durable mode not supported by the server.
                ServerObjectState serverObject = (ServerObjectState)FindPredefinedNode(
                    ObjectIds.Server,
                    typeof(ServerObjectState));

                if (serverObject != null)
                {
                    NodeState setSubscriptionDurableNode = serverObject.FindChild(
                        SystemContext,
                        BrowseNames.SetSubscriptionDurable);

                    if (setSubscriptionDurableNode != null)
                    {
                        DeleteNode(SystemContext, MethodIds.Server_SetSubscriptionDurable);
                        serverObject.SetSubscriptionDurable = null;
                    }
                }
#endif

                // hookup server ResendData method.

                ResendDataMethodState resendData = (ResendDataMethodState)FindPredefinedNode(
                    MethodIds.Server_ResendData,
                    typeof(ResendDataMethodState));

                if (resendData != null)
                {
                    resendData.OnCallMethod = OnResendData;
                }
            }
        }

        /// <summary>
        /// Called when a client sets a subscription as durable.
        /// </summary>

        public ServiceResult OnSetSubscriptionDurable(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint subscriptionId,
            uint lifetimeInHours,
            ref uint revisedLifetimeInHours)
        {
            revisedLifetimeInHours = 0;

            foreach (Subscriptions.Subscription subscription in ServerData.SubscriptionManager.GetSubscriptions())
            {
                if (subscription.Id == subscriptionId)
                {
                    if (subscription.SessionId != context.SessionId)
                    {
                        // user tries to access subscription of different session
                        return StatusCodes.BadUserAccessDenied;
                    }

                    ServiceResult result = subscription.SetSubscriptionDurable(lifetimeInHours, out uint revisedLifeTimeHours);

                    revisedLifetimeInHours = revisedLifeTimeHours;
                    return result;
                }
            }

            return StatusCodes.BadSubscriptionIdInvalid;
        }

        /// <summary>
        /// Called when a client gets the monitored items of a subscription.
        /// </summary>
        public ServiceResult OnGetMonitoredItems(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (inputArguments == null || inputArguments.Count != 1)
            {
                return StatusCodes.BadInvalidArgument;
            }

            var subscriptionId = inputArguments[0] as uint?;

            if (subscriptionId == null)
            {
                return StatusCodes.BadInvalidArgument;
            }

            foreach (var subscription in ServerData.SubscriptionManager.GetSubscriptions())
            {
                if (subscription.Id == subscriptionId)
                {
                    if (subscription.SessionId != context.SessionId)
                    {
                        // user tries to access subscription of different session
                        return StatusCodes.BadUserAccessDenied;
                    }

                    subscription.GetMonitoredItems(out var serverHandles, out var clientHandles);

                    outputArguments[0] = serverHandles;
                    outputArguments[1] = clientHandles;

                    return ServiceResult.Good;
                }
            }

            return StatusCodes.BadSubscriptionIdInvalid;
        }

        /// <summary>
        /// Called when a client initiates resending of all data monitored items in a Subscription.
        /// </summary>
        public ServiceResult OnResendData(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (inputArguments == null || inputArguments.Count != 1)
            {
                return StatusCodes.BadInvalidArgument;
            }

            uint? subscriptionId = inputArguments[0] as uint?;

            if (subscriptionId == null)
            {
                return StatusCodes.BadInvalidArgument;
            }

            foreach (Subscriptions.Subscription subscription in ServerData.SubscriptionManager.GetSubscriptions())
            {
                if (subscription.Id == subscriptionId)
                {
                    if (subscription.SessionId != context.SessionId)
                    {
                        // user tries to access subscription of different session
                        return StatusCodes.BadUserAccessDenied;
                    }

                    subscription.ResendData((UaServerOperationContext)((SystemContext)context)?.OperationContext);

                    return ServiceResult.Good;
                }
            }

            return StatusCodes.BadSubscriptionIdInvalid;
        }

        /// <summary>
        /// Called when a client locks the server.
        /// </summary>
        public ServiceResult OnLockServer(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var systemContext = context as UaServerContext;

            if (serverLockHolder_ != null)
            {
                if (systemContext != null && serverLockHolder_ != systemContext.SessionId)
                {
                    return StatusCodes.BadSessionIdInvalid;
                }
            }

            if (systemContext != null)
            {
                serverLockHolder_ = systemContext.SessionId;
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Called when a client locks the server.
        /// </summary>
        public ServiceResult OnUnlockServer(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var systemContext = context as UaServerContext;

            if (serverLockHolder_ != null)
            {
                if (systemContext != null && serverLockHolder_ != systemContext.SessionId)
                {
                    return StatusCodes.BadSessionIdInvalid;
                }
            }

            serverLockHolder_ = null;

            return ServiceResult.Good;
        }

        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            var predefinedNodes = new NodeStateCollection();
            var assembly = typeof(ArgumentCollection).GetTypeInfo().Assembly;
            predefinedNodes.LoadFromBinaryResource(context, "Opc.Ua.Stack.Generated.Opc.Ua.PredefinedNodes.uanodes", assembly, true);
            return predefinedNodes;
        }

        /// <summary>
        /// Replaces the generic node with a node specific to the model.
        /// </summary>
        protected override NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
        {
            var passiveNode = predefinedNode as BaseObjectState;

            if (passiveNode == null)
            {
                BaseVariableState passiveVariable = predefinedNode as BaseVariableState;
                if (passiveVariable != null)
                {
                    if (passiveVariable.NodeId == VariableIds.ServerStatusType_BuildInfo)
                    {
                        if (passiveVariable is BuildInfoVariableState)
                        {
                            return predefinedNode;
                        }

                        BuildInfoVariableState activeNode = new BuildInfoVariableState(passiveVariable.Parent);
                        activeNode.Create(context, passiveVariable);

                        // replace the node in the parent.
                        if (passiveVariable.Parent != null)
                        {
                            passiveVariable.Parent.ReplaceChild(context, activeNode);
                        }

                        return activeNode;
                    }
                    return predefinedNode;
                }

                var passiveMethod = predefinedNode as MethodState;

                if (passiveMethod == null)
                {
                    return predefinedNode;
                }

                if (passiveMethod.NodeId == MethodIds.ConditionType_ConditionRefresh)
                {
                    var activeNode = new ConditionRefreshMethodState(passiveMethod.Parent);
                    activeNode.Create(context, passiveMethod);

                    // replace the node in the parent.
                    if (passiveMethod.Parent != null)
                    {
                        passiveMethod.Parent.ReplaceChild(context, activeNode);
                    }

                    activeNode.OnCall = OnConditionRefresh;

                    return activeNode;
                }
                else if (passiveMethod.NodeId == MethodIds.ConditionType_ConditionRefresh2)
                {
                    ConditionRefresh2MethodState activeNode = new ConditionRefresh2MethodState(passiveMethod.Parent);
                    activeNode.Create(context, passiveMethod);

                    // replace the node in the parent.
                    if (passiveMethod.Parent != null)
                    {
                        passiveMethod.Parent.ReplaceChild(context, activeNode);
                    }

                    activeNode.OnCall = OnConditionRefresh2;

                    return activeNode;
                }

                return predefinedNode;
            }

            var typeId = passiveNode.TypeDefinitionId;

            if (!IsNodeIdInNamespace(typeId) || typeId.IdType != IdType.Numeric)
            {
                return predefinedNode;
            }

            switch ((uint)typeId.Identifier)
            {
                case ObjectTypes.ServerType:
                    {
                        if (passiveNode is ServerObjectState)
                        {
                            break;
                        }

                        var activeNode = new ServerObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        // add the server object as the root notifier.
                        AddRootNotifier(activeNode);

                        // replace the node in the parent.
                        if (passiveNode.Parent != null)
                        {
                            passiveNode.Parent.ReplaceChild(context, activeNode);
                        }

                        return activeNode;
                    }

                case ObjectTypes.HistoryServerCapabilitiesType:
                    {
                        if (passiveNode is HistoryServerCapabilitiesState)
                        {
                            break;
                        }

                        HistoryServerCapabilitiesState activeNode = new HistoryServerCapabilitiesState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        // replace the node in the parent.
                        if (passiveNode.Parent != null)
                        {
                            passiveNode.Parent.ReplaceChild(context, activeNode);
                        }

                        return activeNode;
                    }
            }

            return predefinedNode;
        }

        /// <summary>
        /// Handles a request to refresh conditions for a subscription.
        /// </summary>
        private ServiceResult OnConditionRefresh(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint subscriptionId)
        {
            var systemContext = context as UaServerContext ?? SystemContext;

            if (systemContext == null)
            {
                systemContext = this.SystemContext;
            }

            ServerData.ConditionRefresh(systemContext.OperationContext, subscriptionId);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Handles a request to refresh conditions for a subscription and specific monitored item.
        /// </summary>
        private ServiceResult OnConditionRefresh2(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint subscriptionId,
            uint monitoredItemId)
        {
            var systemContext = context as UaServerContext ?? SystemContext;

            if (systemContext == null)
            {
                systemContext = this.SystemContext;
            }

            ServerData.ConditionRefresh2(systemContext.OperationContext, subscriptionId, monitoredItemId);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Returns true of the node is a diagnostics node.
        /// </summary>
        private bool IsDiagnosticsNode(NodeState node)
        {
            if (node == null)
            {
                return false;
            }

            if (!IsDiagnosticsStructureNode(node))
            {
                var instance = node as BaseInstanceState;

                if (instance == null)
                {
                    return false;
                }

                return IsDiagnosticsStructureNode(instance.Parent);
            }

            return true;
        }

        /// <summary>
        /// Returns true of the node is a diagnostics node.
        /// </summary>
        private bool IsDiagnosticsStructureNode(NodeState node)
        {
            var instance = node as BaseInstanceState;

            if (instance == null)
            {
                return false;
            }

            var typeId = instance.TypeDefinitionId;

            if (typeId == null || typeId.IdType != IdType.Numeric || typeId.NamespaceIndex != 0)
            {
                return false;
            }

            switch ((uint)typeId.Identifier)
            {
                case VariableTypes.ServerDiagnosticsSummaryType:
                case ObjectTypes.SessionDiagnosticsObjectType:
                case VariableTypes.SessionDiagnosticsVariableType:
                case VariableTypes.SessionDiagnosticsArrayType:
                case VariableTypes.SessionSecurityDiagnosticsType:
                case VariableTypes.SessionSecurityDiagnosticsArrayType:
                case VariableTypes.SubscriptionDiagnosticsType:
                case VariableTypes.SubscriptionDiagnosticsArrayType:
                case VariableTypes.SamplingIntervalDiagnosticsArrayType:
                    {
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// Force out of band diagnostics update after a change of diagnostics variables.
        /// </summary>
        public void ForceDiagnosticsScan()
        {
            lastDiagnosticsScanTime_ = DateTime.MinValue;
        }

        /// <summary>
        /// True is diagnostics are currently enabled.
        /// </summary>
        public bool DiagnosticsEnabled => diagnosticsEnabled_;

        /// <summary>
        /// Sets the flag controlling whether diagnostics is enabled for the server.
        /// </summary>
        public void SetDiagnosticsEnabled(UaServerContext context, bool enabled)
        {
            var nodesToDelete = new List<NodeState>();

            lock (Lock)
            {
                if (enabled == diagnosticsEnabled_)
                {
                    return;
                }

                diagnosticsEnabled_ = enabled;

                if (!enabled)
                {
                    // stop scans.
                    if (diagnosticsScanTimer_ != null)
                    {
                        diagnosticsScanTimer_.Dispose();
                        diagnosticsScanTimer_ = null;
                    }

                    if (sessions_ != null)
                    {
                        foreach (var sessionDiagnostics in sessions_)
                        {
                            nodesToDelete.Add(sessionDiagnostics.Summary);
                        }

                        sessions_.Clear();
                    }

                    if (subscriptions_ != null)
                    {
                        for (var ii = 0; ii < subscriptions_.Count; ii++)
                        {
                            if (sessions_ != null)
                            {
                                nodesToDelete.Add(sessions_[ii].Value.Variable);
                            }
                        }

                        subscriptions_.Clear();
                    }
                }
                else
                {
                    // reset all diagnostics nodes.
                    if (serverDiagnostics_ != null)
                    {
                        serverDiagnostics_.Value = null;
                        serverDiagnostics_.Error = StatusCodes.BadWaitingForInitialData;
                        serverDiagnostics_.Timestamp = DateTime.UtcNow;
                    }

                    // get the node.
                    var diagnosticsNode = (ServerDiagnosticsState)FindPredefinedNode(
                        ObjectIds.Server_ServerDiagnostics,
                        typeof(ServerDiagnosticsState));

                    // clear arrays.
                    if (diagnosticsNode != null)
                    {
                        if (diagnosticsNode.SamplingIntervalDiagnosticsArray != null)
                        {
                            diagnosticsNode.SamplingIntervalDiagnosticsArray.Value = null;
                            diagnosticsNode.SamplingIntervalDiagnosticsArray.StatusCode = StatusCodes.BadWaitingForInitialData;
                            diagnosticsNode.SamplingIntervalDiagnosticsArray.Timestamp = DateTime.UtcNow;
                        }

                        if (diagnosticsNode.SubscriptionDiagnosticsArray != null)
                        {
                            diagnosticsNode.SubscriptionDiagnosticsArray.Value = null;
                            diagnosticsNode.SubscriptionDiagnosticsArray.StatusCode = StatusCodes.BadWaitingForInitialData;
                            diagnosticsNode.SubscriptionDiagnosticsArray.Timestamp = DateTime.UtcNow;
                        }

                        if (diagnosticsNode.SessionsDiagnosticsSummary != null)
                        {
                            diagnosticsNode.SessionsDiagnosticsSummary.SessionDiagnosticsArray.Value = null;
                            diagnosticsNode.SessionsDiagnosticsSummary.SessionDiagnosticsArray.StatusCode = StatusCodes.BadWaitingForInitialData;
                            diagnosticsNode.SessionsDiagnosticsSummary.SessionDiagnosticsArray.Timestamp = DateTime.UtcNow;
                        }

                        if (diagnosticsNode.SessionsDiagnosticsSummary != null)
                        {
                            diagnosticsNode.SessionsDiagnosticsSummary.SessionSecurityDiagnosticsArray.Value = null;
                            diagnosticsNode.SessionsDiagnosticsSummary.SessionSecurityDiagnosticsArray.StatusCode = StatusCodes.BadWaitingForInitialData;
                            diagnosticsNode.SessionsDiagnosticsSummary.SessionSecurityDiagnosticsArray.Timestamp = DateTime.UtcNow;
                        }
                    }

                    DoScan(true);
                }
            }

            foreach (var nodeToDelete in nodesToDelete)
            {
                DeleteNode(context, nodeToDelete.NodeId);
            }
        }

        /// <summary>
        /// Creates the diagnostics node for the server.
        /// </summary>
        public void CreateServerDiagnostics(
            UaServerContext systemContext,
            ServerDiagnosticsSummaryDataType diagnostics,
            NodeValueSimpleEventHandler updateCallback)
        {
            lock (Lock)
            {
                // get the node.
                var diagnosticsNode = (ServerDiagnosticsSummaryState)FindPredefinedNode(
                    VariableIds.Server_ServerDiagnostics_ServerDiagnosticsSummary,
                    typeof(ServerDiagnosticsSummaryState));

                // wrap diagnostics in a thread safe object.
                var diagnosticsValue = new ServerDiagnosticsSummaryValue(
                    diagnosticsNode,
                    diagnostics,
                    Lock);

                // must ensure the first update gets sent.
                diagnosticsValue.Value = null;
                diagnosticsValue.Error = StatusCodes.BadWaitingForInitialData;
                diagnosticsValue.CopyPolicy = Opc.Ua.VariableCopyPolicy.Never;
                diagnosticsValue.OnBeforeRead = OnBeforeReadDiagnostics;
                // Hook the OnReadUserRolePermissions callback to control which user roles can access the services on this node
                diagnosticsNode.OnReadUserRolePermissions = OnReadUserRolePermissions;

                serverDiagnostics_ = diagnosticsValue;
                serverDiagnosticsCallback_ = updateCallback;

                // set up handler for session diagnostics array.
                var array1 = (SessionDiagnosticsArrayState)FindPredefinedNode(
                    VariableIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary_SessionDiagnosticsArray,
                    typeof(SessionDiagnosticsArrayState));

                if (array1 != null)
                {
                    array1.OnSimpleReadValue = OnReadDiagnosticsArray;
                }

                // set up handler for session security diagnostics array.
                var array2 = (SessionSecurityDiagnosticsArrayState)FindPredefinedNode(
                    VariableIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary_SessionSecurityDiagnosticsArray,
                    typeof(SessionSecurityDiagnosticsArrayState));

                if (array2 != null)
                {
                    array2.OnSimpleReadValue = OnReadDiagnosticsArray;
                }

                // set up handler for subscription security diagnostics array.
                var array3 = (SubscriptionDiagnosticsArrayState)FindPredefinedNode(
                    VariableIds.Server_ServerDiagnostics_SubscriptionDiagnosticsArray,
                    typeof(SubscriptionDiagnosticsArrayState));

                if (array3 != null)
                {
                    array3.OnSimpleReadValue = OnReadDiagnosticsArray;
                    // Hook the OnReadUserRolePermissions callback to control which user roles can access the services on this node
                    array3.OnReadUserRolePermissions = OnReadUserRolePermissions;
                }

                // send initial update.
                DoScan(true);
            }
        }

        /// <summary>
        /// Creates the diagnostics node for a subscription.
        /// </summary>
        public NodeId CreateSessionDiagnostics(
            UaServerContext systemContext,
            SessionDiagnosticsDataType diagnostics,
            NodeValueSimpleEventHandler updateCallback,
            SessionSecurityDiagnosticsDataType securityDiagnostics,
            NodeValueSimpleEventHandler updateSecurityCallback)
        {
            NodeId nodeId;

            lock (Lock)
            {
                var sessionNode = new SessionDiagnosticsObjectState(null);

                // create a new instance and assign ids.
                nodeId = CreateNode(
                    systemContext,
                    null,
                    ReferenceTypeIds.HasComponent,
                    new QualifiedName(diagnostics.SessionName),
                    sessionNode);

                diagnostics.SessionId = nodeId;
                securityDiagnostics.SessionId = nodeId;

                // check if diagnostics have been enabled.
                if (!diagnosticsEnabled_)
                {
                    return nodeId;
                }

                // add reference to session summary object.
                sessionNode.AddReference(
                    ReferenceTypeIds.HasComponent,
                    true,
                    ObjectIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary);

                // add reference from session summary object.
                var summary = (SessionsDiagnosticsSummaryState)FindPredefinedNode(
                    ObjectIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary,
                    typeof(SessionsDiagnosticsSummaryState));

                if (summary != null)
                {
                    summary.AddReference(ReferenceTypeIds.HasComponent, false, sessionNode.NodeId);
                }

                // Hook the OnReadUserRolePermissions callback to control which user roles can access the services on this node
                sessionNode.OnReadUserRolePermissions = OnReadUserRolePermissions;

                // initialize diagnostics node.
                var diagnosticsNode = sessionNode.CreateChild(
                   systemContext,
                   BrowseNames.SessionDiagnostics) as SessionDiagnosticsVariableState;

                // wrap diagnostics in a thread safe object.
                var diagnosticsValue = new SessionDiagnosticsVariableValue(
                    diagnosticsNode,
                    diagnostics,
                    Lock);

                // must ensure the first update gets sent.
                diagnosticsValue.Value = null;
                diagnosticsValue.Error = StatusCodes.BadWaitingForInitialData;
                diagnosticsValue.CopyPolicy = Opc.Ua.VariableCopyPolicy.Never;
                diagnosticsValue.OnBeforeRead = OnBeforeReadDiagnostics;

                // initialize security diagnostics node.
                var securityDiagnosticsNode = sessionNode.CreateChild(
                   systemContext,
                   BrowseNames.SessionSecurityDiagnostics) as SessionSecurityDiagnosticsState;

                // wrap diagnostics in a thread safe object.
                var securityDiagnosticsValue = new SessionSecurityDiagnosticsValue(
                    securityDiagnosticsNode,
                    securityDiagnostics,
                    Lock);

                // must ensure the first update gets sent.
                securityDiagnosticsValue.Value = null;
                securityDiagnosticsValue.Error = StatusCodes.BadWaitingForInitialData;
                securityDiagnosticsValue.CopyPolicy = VariableCopyPolicy.Never;
                securityDiagnosticsValue.OnBeforeRead = OnBeforeReadDiagnostics;

                // save the session.
                var sessionData = new SessionDiagnosticsData(
                    sessionNode,
                    diagnosticsValue,
                    updateCallback,
                    securityDiagnosticsValue,
                    updateSecurityCallback);

                sessions_.Add(sessionData);

                // send initial update.
                DoScan(true);
            }

            return nodeId;
        }

        /// <summary>
        /// Delete the diagnostics node for a session.
        /// </summary>
        public void DeleteSessionDiagnostics(
            UaServerContext systemContext,
            NodeId nodeId)
        {
            lock (Lock)
            {
                for (var ii = 0; ii < sessions_.Count; ii++)
                {
                    var summary = sessions_[ii].Summary;

                    if (summary.NodeId == nodeId)
                    {
                        sessions_.RemoveAt(ii);
                        break;
                    }
                }

                // release the server lock if it is being held.
                if (serverLockHolder_ == nodeId)
                {
                    serverLockHolder_ = null;
                }
            }

            DeleteNode(systemContext, nodeId);
        }

        /// <summary>
        /// Creates the diagnostics node for a subscription.
        /// </summary>
        public NodeId CreateSubscriptionDiagnostics(
            UaServerContext systemContext,
            SubscriptionDiagnosticsDataType diagnostics,
            NodeValueSimpleEventHandler updateCallback)
        {
            NodeId nodeId = null;

            lock (Lock)
            {
                // check if diagnostics have been enabled.
                if (!diagnosticsEnabled_)
                {
                    return null;
                }

                var diagnosticsNode = new SubscriptionDiagnosticsState(null);

                // create a new instance and assign ids.
                nodeId = CreateNode(
                    systemContext,
                    null,
                    ReferenceTypeIds.HasComponent,
                    new QualifiedName(diagnostics.SubscriptionId.ToString(CultureInfo.InvariantCulture)),
                    diagnosticsNode);

                // add reference to subscription array.
                diagnosticsNode.AddReference(
                    ReferenceTypeIds.HasComponent,
                    true,
                    VariableIds.Server_ServerDiagnostics_SubscriptionDiagnosticsArray);

                // wrap diagnostics in a thread safe object.
                var diagnosticsValue = new SubscriptionDiagnosticsValue(diagnosticsNode, diagnostics, Lock);
                diagnosticsValue.CopyPolicy = Opc.Ua.VariableCopyPolicy.Never;
                diagnosticsValue.OnBeforeRead = OnBeforeReadDiagnostics;

                // must ensure the first update gets sent.
                diagnosticsValue.Value = null;
                diagnosticsValue.Error = StatusCodes.BadWaitingForInitialData;

                subscriptions_.Add(new SubscriptionDiagnosticsData(diagnosticsValue, updateCallback));

                // add reference from subscription array.
                var array = (SubscriptionDiagnosticsArrayState)FindPredefinedNode(
                    VariableIds.Server_ServerDiagnostics_SubscriptionDiagnosticsArray,
                    typeof(SubscriptionDiagnosticsArrayState));

                if (array != null)
                {
                    array.AddReference(ReferenceTypeIds.HasComponent, false, diagnosticsNode.NodeId);
                }

                // add reference to session subscription array.
                diagnosticsNode.AddReference(
                    ReferenceTypeIds.HasComponent,
                    true,
                    diagnostics.SessionId);

                // add reference from session subscription array.
                var sessionNode = (SessionDiagnosticsObjectState)FindPredefinedNode(
                    diagnostics.SessionId,
                    typeof(SessionDiagnosticsObjectState));

                if (sessionNode != null)
                {
                    // add reference from subscription array.
                    array = (SubscriptionDiagnosticsArrayState)sessionNode.CreateChild(
                        systemContext,
                        BrowseNames.SubscriptionDiagnosticsArray);

                    if (array != null)
                    {
                        array.AddReference(ReferenceTypeIds.HasComponent, false, diagnosticsNode.NodeId);
                    }
                }

                // send initial update.
                DoScan(true);
            }

            return nodeId;
        }

        /// <summary>
        /// Delete the diagnostics node for a subscription.
        /// </summary>
        public void DeleteSubscriptionDiagnostics(
            UaServerContext systemContext,
            NodeId nodeId)
        {
            lock (Lock)
            {
                for (var ii = 0; ii < subscriptions_.Count; ii++)
                {
                    var diagnostics = subscriptions_[ii];

                    if (diagnostics.Value.Variable.NodeId == nodeId)
                    {
                        subscriptions_.RemoveAt(ii);
                        break;
                    }
                }
            }

            DeleteNode(systemContext, nodeId);
        }

        /// <summary>
        /// Gets the default history capabilities object.
        /// </summary>
        public HistoryServerCapabilitiesState GetDefaultHistoryCapabilities()
        {
            lock (Lock)
            {
                if (historyCapabilities_ != null)
                {
                    return historyCapabilities_;
                }

                // search the Node in PredefinedNodes.
                HistoryServerCapabilitiesState historyServerCapabilitiesNode = (HistoryServerCapabilitiesState)FindPredefinedNode(
                    ObjectIds.HistoryServerCapabilities,
                    typeof(HistoryServerCapabilitiesState));

                if (historyServerCapabilitiesNode == null)
                {
                    // create new node if not found.
                    historyServerCapabilitiesNode = new HistoryServerCapabilitiesState(null);

                    NodeId nodeId = CreateNode(
                        SystemContext,
                        null,
                        ReferenceTypeIds.HasComponent,
                        new QualifiedName(BrowseNames.HistoryServerCapabilities),
                        historyServerCapabilitiesNode);

                    historyServerCapabilitiesNode.AccessHistoryDataCapability.Value = false;
                    historyServerCapabilitiesNode.AccessHistoryEventsCapability.Value = false;
                    historyServerCapabilitiesNode.MaxReturnDataValues.Value = 0;
                    historyServerCapabilitiesNode.MaxReturnEventValues.Value = 0;
                    historyServerCapabilitiesNode.ReplaceDataCapability.Value = false;
                    historyServerCapabilitiesNode.UpdateDataCapability.Value = false;
                    historyServerCapabilitiesNode.InsertEventCapability.Value = false;
                    historyServerCapabilitiesNode.ReplaceEventCapability.Value = false;
                    historyServerCapabilitiesNode.UpdateEventCapability.Value = false;
                    historyServerCapabilitiesNode.InsertAnnotationCapability.Value = false;
                    historyServerCapabilitiesNode.InsertDataCapability.Value = false;
                    historyServerCapabilitiesNode.DeleteRawCapability.Value = false;
                    historyServerCapabilitiesNode.DeleteAtTimeCapability.Value = false;
                    historyServerCapabilitiesNode.ServerTimestampSupported.Value = false;

                    NodeState parent = FindPredefinedNode(ObjectIds.Server_ServerCapabilities, typeof(ServerCapabilitiesState));

                    if (parent != null)
                    {
                        parent.AddReference(ReferenceTypes.HasComponent, false, historyServerCapabilitiesNode.NodeId);
                        historyServerCapabilitiesNode.AddReference(ReferenceTypes.HasComponent, true, parent.NodeId);
                    }

                    AddPredefinedNode(SystemContext, historyServerCapabilitiesNode);
                }

                historyCapabilities_ = historyServerCapabilitiesNode;
                return historyCapabilities_;
            }
        }

        /// <summary>
        /// Adds an aggregate function to the server capabilities object.
        /// </summary>
        public void AddAggregateFunction(NodeId aggregateId, string aggregateName, bool isHistorical)
        {
            lock (Lock)
            {
                var state = new FolderState(null);

                state.SymbolicName = aggregateName;
                state.ReferenceTypeId = ReferenceTypes.HasComponent;
                state.TypeDefinitionId = ObjectTypeIds.AggregateFunctionType;
                state.NodeId = aggregateId;
                state.BrowseName = new QualifiedName(aggregateName, aggregateId.NamespaceIndex);
                state.DisplayName = state.BrowseName.Name;
                state.WriteMask = AttributeWriteMask.None;
                state.UserWriteMask = AttributeWriteMask.None;
                state.EventNotifier = EventNotifiers.None;

                var folder = FindPredefinedNode(ObjectIds.Server_ServerCapabilities_AggregateFunctions, typeof(BaseObjectState));

                if (folder != null)
                {
                    folder.AddReference(ReferenceTypes.Organizes, false, state.NodeId);
                    state.AddReference(ReferenceTypes.Organizes, true, folder.NodeId);
                }

                if (isHistorical)
                {
                    folder = FindPredefinedNode(ObjectIds.HistoryServerCapabilities_AggregateFunctions, typeof(BaseObjectState));

                    if (folder != null)
                    {
                        folder.AddReference(ReferenceTypes.Organizes, false, state.NodeId);
                        state.AddReference(ReferenceTypes.Organizes, true, folder.NodeId);
                    }
                }

                AddPredefinedNode(SystemContext, state);
            }
        }

        /// <summary>
        /// Updates the server diagnostics summary structure.
        /// </summary>
        private bool UpdateServerDiagnosticsSummary()
        {
            // get the latest snapshot.
            object value = null;

            var result = serverDiagnosticsCallback_(
                SystemContext,
                serverDiagnostics_.Variable,
                ref value);

            var newValue = value as ServerDiagnosticsSummaryDataType;

            // check for changes.
            if (Utils.IsEqual(newValue, serverDiagnostics_.Value))
            {
                return false;
            }

            serverDiagnostics_.Error = null;

            // check for bad value.
            if (ServiceResult.IsNotBad(result) && newValue == null)
            {
                result = StatusCodes.BadOutOfService;
            }

            // check for bad result.
            if (ServiceResult.IsBad(result))
            {
                serverDiagnostics_.Error = result;
                newValue = null;
            }

            // update the value.
            serverDiagnostics_.Value = newValue;
            serverDiagnostics_.Timestamp = DateTime.UtcNow;

            // notify any monitored items.
            serverDiagnostics_.ChangesComplete(SystemContext);


            return true;
        }

        /// <summary>
        /// Updates the session diagnostics summary structure.
        /// </summary>
        private bool UpdateSessionDiagnostics(
            ISystemContext context,
            SessionDiagnosticsData diagnostics,
            SessionDiagnosticsDataType[] sessionArray,
            int index)
        {
            // get the latest snapshot.
            object value = null;

            var result = diagnostics.UpdateCallback(
                SystemContext,
                diagnostics.Value.Variable,
                ref value);

            var newValue = value as SessionDiagnosticsDataType;
            sessionArray[index] = newValue;

            if ((context != null) && (sessionArray?[index] != null))
            {
                FilterOutUnAuthorized(sessionArray, newValue.SessionId, context, index);
            }

            // check for changes.
            if (Utils.IsEqual(newValue, diagnostics.Value.Value))
            {
                return false;
            }

            diagnostics.Value.Error = null;

            // check for bad value.
            if (ServiceResult.IsNotBad(result) && newValue == null)
            {
                result = StatusCodes.BadOutOfService;
            }

            // check for bad result.
            if (ServiceResult.IsBad(result))
            {
                diagnostics.Value.Error = result;
                newValue = null;
            }

            // update the value.
            diagnostics.Value.Value = newValue;
            diagnostics.Value.Timestamp = DateTime.UtcNow;

            // notify any monitored items.
            diagnostics.Value.ChangesComplete(SystemContext);

            return true;
        }

        /// <summary>
        /// Updates the session diagnostics summary structure.
        /// </summary>
        private bool UpdateSessionSecurityDiagnostics(
            ISystemContext context,
            SessionDiagnosticsData diagnostics,
            SessionSecurityDiagnosticsDataType[] sessionArray,
            int index)
        {
            // get the latest snapshot.
            object value = null;

            var result = diagnostics.SecurityUpdateCallback(
                SystemContext,
                diagnostics.SecurityValue.Variable,
                ref value);

            var newValue = value as SessionSecurityDiagnosticsDataType;
            sessionArray[index] = newValue;

            if ((context != null) && (sessionArray?[index] != null))
            {
                FilterOutUnAuthorized(sessionArray, newValue.SessionId, context, index);
            }

            // check for changes.
            if (Utils.IsEqual(newValue, diagnostics.SecurityValue.Value))
            {
                return false;
            }

            diagnostics.SecurityValue.Error = null;

            // check for bad value.
            if (ServiceResult.IsNotBad(result) && newValue == null)
            {
                result = StatusCodes.BadOutOfService;
            }

            // check for bad result.
            if (ServiceResult.IsBad(result))
            {
                diagnostics.SecurityValue.Error = result;
                newValue = null;
            }

            // update the value.
            diagnostics.SecurityValue.Value = newValue;
            diagnostics.SecurityValue.Timestamp = DateTime.UtcNow;

            // notify any monitored items.
            diagnostics.SecurityValue.ChangesComplete(SystemContext);

            return true;
        }

        /// <summary>
        /// Updates the subscription diagnostics summary structure.
        /// </summary>
        private bool UpdateSubscriptionDiagnostics(
            ISystemContext context,
            SubscriptionDiagnosticsData diagnostics,
            SubscriptionDiagnosticsDataType[] subscriptionArray,
            int index)
        {
            // get the latest snapshot.
            object value = null;

            var result = diagnostics.UpdateCallback(
                SystemContext,
                diagnostics.Value.Variable,
                ref value);

            var newValue = value as SubscriptionDiagnosticsDataType;
            subscriptionArray[index] = newValue;

            if ((context != null) && (subscriptionArray?[index] != null))
            {
                FilterOutUnAuthorized(subscriptionArray, newValue.SessionId, context, index);
            }

            // check for changes.
            if (Utils.IsEqual(newValue, diagnostics.Value.Value))
            {
                return false;
            }

            diagnostics.Value.Error = null;

            // check for bad value.
            if (ServiceResult.IsNotBad(result) && newValue == null)
            {
                result = StatusCodes.BadOutOfService;
            }

            // check for bad result.
            if (ServiceResult.IsBad(result))
            {
                diagnostics.Value.Error = result;
                newValue = null;
            }

            // update the value.
            diagnostics.Value.Value = newValue;
            diagnostics.Value.Timestamp = DateTime.UtcNow;

            // notify any monitored items.
            diagnostics.Value.ChangesComplete(SystemContext);

            return true;
        }


        /// <summary>
        /// Filter out the members which corespond to users that are not allowed to see their contents
        /// Current user is allowed to read its data, together with users which have permissions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sessionId"></param>
        /// <param name="context"></param>
        /// <param name="index"></param>
        private void FilterOutUnAuthorized<T>(IList<T> list, NodeId sessionId, ISystemContext context, int index)
        {
            if ((sessionId != context.SessionId) &&
                    !HasApplicationSecureAdminAccess(context))
            {
                list[index] = default;
            }
        }

        /// <summary>
        /// Set custom role permissions for desired node
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ServiceResult OnReadUserRolePermissions(
            ISystemContext context,
            NodeState node,
            ref RolePermissionTypeCollection value)
        {
            bool admitUser;

            if ((node.NodeId == VariableIds.Server_ServerDiagnostics_ServerDiagnosticsSummary) ||
                 (node.NodeId == VariableIds.Server_ServerDiagnostics_SubscriptionDiagnosticsArray))
            {
                admitUser = HasApplicationSecureAdminAccess(context);
            }
            else
            {
                admitUser = (node.NodeId == context.SessionId) ||
                            HasApplicationSecureAdminAccess(context);
            }

            if (admitUser)
            {
                var rolePermissionTypes = from roleId in wellKnownRoles_
                                          select new RolePermissionType()
                                          {
                                              RoleId = roleId,
                                              Permissions = (uint)(PermissionType.Browse | PermissionType.Read | PermissionType.ReadRolePermissions | PermissionType.Write)
                                          };

                value = new RolePermissionTypeCollection(rolePermissionTypes);
            }
            else
            {
                var rolePermissionTypes = from roleId in wellKnownRoles_
                                          select new RolePermissionType()
                                          {
                                              RoleId = roleId,
                                              Permissions = (uint)PermissionType.None
                                          };

                value = new RolePermissionTypeCollection(rolePermissionTypes);

            }
            return ServiceResult.Good;
        }

        /// <summary>
        /// Does a scan before the diagnostics are read.
        /// </summary>
        private void OnBeforeReadDiagnostics(
            ISystemContext context,
            BaseVariableValue variable,
            NodeState component)
        {
            lock (Lock)
            {
                if (!diagnosticsEnabled_)
                {
                    return;
                }

                if (DateTime.UtcNow < lastDiagnosticsScanTime_.AddSeconds(1))
                {
                    return;
                }

                DoScan(true);
            }
        }

        /// <summary>
        /// Does a scan before the diagnostics are read.
        /// </summary>
        private ServiceResult OnReadDiagnosticsArray(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            lock (Lock)
            {
                if (!diagnosticsEnabled_)
                {
                    return StatusCodes.BadOutOfService;
                }

                if (DateTime.UtcNow < lastDiagnosticsScanTime_.AddSeconds(1))
                {
                    // diagnostic nodes already scanned.
                    return ServiceResult.Good;
                }

                if (node.NodeId == VariableIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary_SessionDiagnosticsArray)
                {
                    // read session diagnostics.
                    SessionDiagnosticsDataType[] sessionArray = new SessionDiagnosticsDataType[sessions_.Count];

                    for (int ii = 0; ii < sessions_.Count; ii++)
                    {
                        SessionDiagnosticsData diagnostics = sessions_[ii];
                        UpdateSessionDiagnostics(context, diagnostics, sessionArray, ii);
                    }
                    sessionArray = sessionArray.Where(s => s != null).ToArray();

                    value = sessionArray;
                }
                else if (node.NodeId == VariableIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary_SessionSecurityDiagnosticsArray)
                {
                    // read session security diagnostics.
                    SessionSecurityDiagnosticsDataType[] sessionSecurityArray = new SessionSecurityDiagnosticsDataType[sessions_.Count];

                    for (int ii = 0; ii < sessions_.Count; ii++)
                    {
                        UpdateSessionSecurityDiagnostics(context, sessions_[ii], sessionSecurityArray, ii);
                    }
                    sessionSecurityArray = sessionSecurityArray.Where(s => s != null).ToArray();

                    value = sessionSecurityArray;
                }
                else if (node.NodeId == VariableIds.Server_ServerDiagnostics_SubscriptionDiagnosticsArray)
                {
                    // read subscription diagnostics.
                    SubscriptionDiagnosticsDataType[] subscriptionArray = new SubscriptionDiagnosticsDataType[subscriptions_.Count];

                    for (int ii = 0; ii < subscriptions_.Count; ii++)
                    {
                        UpdateSubscriptionDiagnostics(context, subscriptions_[ii], subscriptionArray, ii);
                    }
                    subscriptionArray = subscriptionArray.Where(s => s != null).ToArray();

                    value = subscriptionArray;
                }

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Determine if the impersonated user has admin access.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ServiceResultException"/>
        /// <seealso cref="StatusCodes.BadUserAccessDenied"/>
        private bool HasApplicationSecureAdminAccess(ISystemContext context)
        {
            UaServerOperationContext operationContext = (context as SystemContext)?.OperationContext as UaServerOperationContext;
            if (operationContext != null)
            {
                if (operationContext.ChannelContext?.EndpointDescription?.SecurityMode != MessageSecurityMode.SignAndEncrypt)
                {
                    return false;
                }

                SystemConfigurationIdentity user = context.UserIdentity as SystemConfigurationIdentity;
                if (user == null ||
                    user.TokenType == UserTokenType.Anonymous ||
                    !user.GrantedRoleIds.Contains(ObjectIds.WellKnownRole_SecurityAdmin))
                {
                    return false;
                }

                return true;
            }
            return false;
        }


        /// <summary>
        /// Reports notifications for any monitored diagnostic nodes.
        /// </summary>
        private void DoScan(object alwaysUpdateArrays)
        {
            try
            {
                lock (Lock)
                {
                    if (!diagnosticsEnabled_ || doScanBusy_)
                    {
                        return;
                    }

                    try
                    {
                        doScanBusy_ = true;

                        lastDiagnosticsScanTime_ = DateTime.UtcNow;

                        // update server diagnostics.
                        UpdateServerDiagnosticsSummary();

                        // update session diagnostics.
                        var sessionsChanged = alwaysUpdateArrays != null;
                        var sessionArray = new SessionDiagnosticsDataType[sessions_.Count];

                        for (var ii = 0; ii < sessions_.Count; ii++)
                        {
                            var diagnostics = sessions_[ii];

                            if (UpdateSessionDiagnostics(null, diagnostics, sessionArray, ii))
                            {
                                sessionsChanged = true;
                            }
                        }

                        // check of the session diagnostics array node needs to be updated.
                        var sessionsNode = (SessionDiagnosticsArrayState)FindPredefinedNode(
                            VariableIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary_SessionDiagnosticsArray,
                            typeof(SessionDiagnosticsArrayState));

                        if (sessionsNode != null && (sessionsNode.Value == null || StatusCode.IsBad(sessionsNode.StatusCode) || sessionsChanged))
                        {
                            sessionsNode.Value = sessionArray;
                            sessionsNode.ClearChangeMasks(SystemContext, false);
                        }

                        var sessionsSecurityChanged = alwaysUpdateArrays != null;
                        var sessionSecurityArray = new SessionSecurityDiagnosticsDataType[sessions_.Count];

                        for (var ii = 0; ii < sessions_.Count; ii++)
                        {
                            var diagnostics = sessions_[ii];

                            if (UpdateSessionSecurityDiagnostics(null, diagnostics, sessionSecurityArray, ii))
                            {
                                sessionsSecurityChanged = true;
                            }
                        }

                        // check of the array node needs to be updated.
                        var sessionsSecurityNode = (SessionSecurityDiagnosticsArrayState)FindPredefinedNode(
                            VariableIds.Server_ServerDiagnostics_SessionsDiagnosticsSummary_SessionSecurityDiagnosticsArray,
                            typeof(SessionSecurityDiagnosticsArrayState));

                        if (sessionsSecurityNode != null && (sessionsSecurityNode.Value == null || StatusCode.IsBad(sessionsSecurityNode.StatusCode) || sessionsSecurityChanged))
                        {
                            sessionsSecurityNode.Value = sessionSecurityArray;
                            sessionsSecurityNode.ClearChangeMasks(SystemContext, false);
                        }

                        var subscriptionsChanged = alwaysUpdateArrays != null;
                        var subscriptionArray = new SubscriptionDiagnosticsDataType[subscriptions_.Count];

                        for (var ii = 0; ii < subscriptions_.Count; ii++)
                        {
                            var diagnostics = subscriptions_[ii];

                            if (UpdateSubscriptionDiagnostics(null, diagnostics, subscriptionArray, ii))
                            {
                                subscriptionsChanged = true;
                            }
                        }

                        // check of the subscription node needs to be updated.
                        var subscriptionsNode = (SubscriptionDiagnosticsArrayState)FindPredefinedNode(
                            VariableIds.Server_ServerDiagnostics_SubscriptionDiagnosticsArray,
                            typeof(SubscriptionDiagnosticsArrayState));

                        if (subscriptionsNode != null && (subscriptionsNode.Value == null || StatusCode.IsBad(subscriptionsNode.StatusCode) || subscriptionsChanged))
                        {
                            subscriptionsNode.Value = subscriptionArray;
                            subscriptionsNode.ClearChangeMasks(SystemContext, false);
                        }

                        for (var ii = 0; ii < sessions_.Count; ii++)
                        {
                            var diagnostics = sessions_[ii];
                            var subscriptionDiagnosticsArray = new List<SubscriptionDiagnosticsDataType>();

                            var sessionId = diagnostics.Summary.NodeId;

                            for (var jj = 0; jj < subscriptions_.Count; jj++)
                            {
                                var subscriptionDiagnostics = subscriptions_[jj];

                                if (subscriptionDiagnostics.Value.Value == null)
                                {
                                    continue;
                                }

                                if (subscriptionDiagnostics.Value.Value.SessionId != sessionId)
                                {
                                    continue;
                                }

                                subscriptionDiagnosticsArray.Add(subscriptionDiagnostics.Value.Value);
                            }

                            // update session subscription array.
                            subscriptionsNode = (SubscriptionDiagnosticsArrayState)diagnostics.Summary.CreateChild(
                                SystemContext,
                                BrowseNames.SubscriptionDiagnosticsArray);

                            if (subscriptionsNode != null && (subscriptionsNode.Value == null || StatusCode.IsBad(subscriptionsNode.StatusCode) || subscriptionsChanged))
                            {
                                subscriptionsNode.Value = subscriptionDiagnosticsArray.ToArray();
                                subscriptionsNode.ClearChangeMasks(SystemContext, false);
                            }
                        }
                    }
                    finally
                    {
                        doScanBusy_ = false;
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error during diagnostics scan.");
            }
        }

        /// <summary>
        /// Validates the view description passed to a browse request (throws on error).
        /// </summary>
        protected override void ValidateViewDescription(UaServerContext context, ViewDescription view)
        {
            // always accept all views so the root nodes appear in the view.
        }

        /// <summary>
        /// Called after creating a UaMonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemCreated(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            // check if the variable needs to be sampled.
            if (monitoredItem.AttributeId == Attributes.Value)
            {
                var variable = handle.Node as BaseVariableState;

                if (variable != null && variable.MinimumSamplingInterval > 0)
                {
                    CreateSampledItem(monitoredItem.SamplingInterval, monitoredItem);
                }
            }

            // check if diagnostics collection needs to be turned one.
            if (IsDiagnosticsNode(handle.Node))
            {
                monitoredItem.AlwaysReportUpdates = IsDiagnosticsStructureNode(handle.Node);

                if (monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                {
                    diagnosticsMonitoringCount_++;

                    if (diagnosticsScanTimer_ == null)
                    {
                        diagnosticsScanTimer_ = new Timer(DoScan, null, 1000, 1000);
                    }

                    DoScan(true);
                }
            }
        }

        /// <summary>
        /// Called after deleting a UaMonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemDeleted(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            // check if diagnostics collection needs to be turned off.
            if (IsDiagnosticsNode(handle.Node))
            {
                if (monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                {
                    diagnosticsMonitoringCount_--;

                    if (diagnosticsMonitoringCount_ == 0 && diagnosticsScanTimer_ != null)
                    {
                        diagnosticsScanTimer_.Dispose();
                        diagnosticsScanTimer_ = null;
                    }

                    if (diagnosticsScanTimer_ != null)
                    {
                        DoScan(true);
                    }
                }
            }

            // check if sampling needs to be turned off.
            if (monitoredItem.AttributeId == Attributes.Value)
            {
                var variable = handle.Node as BaseVariableState;

                if (variable != null && variable.MinimumSamplingInterval > 0)
                {
                    DeleteSampledItem(monitoredItem);
                }
            }
        }

        /// <summary>
        /// Called after changing the MonitoringMode for a UaMonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="previousMode">The previous monitoring mode.</param>
        /// <param name="monitoringMode">The current monitoring mode.</param>
        protected override void OnMonitoringModeChanged(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem,
            MonitoringMode previousMode,
            MonitoringMode monitoringMode)
        {
            if (previousMode != MonitoringMode.Disabled)
            {
                diagnosticsMonitoringCount_--;
            }

            if (monitoringMode != MonitoringMode.Disabled)
            {
                diagnosticsMonitoringCount_++;
            }

            if (diagnosticsMonitoringCount_ == 0 && diagnosticsScanTimer_ != null)
            {
                if (diagnosticsScanTimer_ != null)
                {
                    diagnosticsScanTimer_.Dispose();
                    diagnosticsScanTimer_ = null;
                }
            }
            else
            {
                if (diagnosticsScanTimer_ != null)
                {
                    diagnosticsScanTimer_ = new Timer(DoScan, null, 1000, 1000);
                }
            }
        }
        #endregion

        #region Node Access Functions
        #endregion

        #region SessionDiagnosticsData Class
        /// <summary>
        /// Stores the callback information for a session diagnostics structures.
        /// </summary>
        private class SessionDiagnosticsData
        {
            public SessionDiagnosticsData(
                SessionDiagnosticsObjectState summary,
                SessionDiagnosticsVariableValue value,
                NodeValueSimpleEventHandler updateCallback,
                SessionSecurityDiagnosticsValue securityValue,
                NodeValueSimpleEventHandler securityUpdateCallback)
            {
                Summary = summary;
                Value = value;
                UpdateCallback = updateCallback;
                SecurityValue = securityValue;
                SecurityUpdateCallback = securityUpdateCallback;
            }

            public SessionDiagnosticsObjectState Summary;
            public SessionDiagnosticsVariableValue Value;
            public NodeValueSimpleEventHandler UpdateCallback;
            public SessionSecurityDiagnosticsValue SecurityValue;
            public NodeValueSimpleEventHandler SecurityUpdateCallback;
        }
        #endregion

        #region SubscriptionDiagnosticsData Class
        /// <summary>
        /// Stores the callback information for a subscription diagnostics structure.
        /// </summary>
        private class SubscriptionDiagnosticsData
        {
            public SubscriptionDiagnosticsData(
                SubscriptionDiagnosticsValue value,
                NodeValueSimpleEventHandler updateCallback)
            {
                Value = value;
                UpdateCallback = updateCallback;
            }

            public SubscriptionDiagnosticsValue Value;
            public NodeValueSimpleEventHandler UpdateCallback;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a new sampled item.
        /// </summary>
        private void CreateSampledItem(double samplingInterval, UaMonitoredItem monitoredItem)
        {
            sampledItems_.Add(monitoredItem);

            if (samplingTimer_ == null)
            {
                samplingTimer_ = new Timer(DoSample, null, (int)minimumSamplingInterval_, (int)minimumSamplingInterval_);
            }
        }

        /// <summary>
        /// Deletes a sampled item.
        /// </summary>
        private void DeleteSampledItem(UaMonitoredItem monitoredItem)
        {
            for (var ii = 0; ii < sampledItems_.Count; ii++)
            {
                if (Object.ReferenceEquals(monitoredItem, sampledItems_[ii]))
                {
                    sampledItems_.RemoveAt(ii);
                    break;
                }
            }

            if (sampledItems_.Count == 0)
            {
                if (samplingTimer_ != null)
                {
                    samplingTimer_.Dispose();
                    samplingTimer_ = null;
                }
            }
        }

        /// <summary>
        /// Polls each monitored item which requires sample. 
        /// </summary>
        private void DoSample(object state)
        {
            try
            {
                lock (Lock)
                {
                    for (var ii = 0; ii < sampledItems_.Count; ii++)
                    {
                        var monitoredItem = sampledItems_[ii];

                        // get the handle.
                        var handle = monitoredItem.ManagerHandle as UaNodeHandle;

                        if (handle == null)
                        {
                            continue;
                        }

                        // check if it is time to sample.
                        if (monitoredItem.TimeToNextSample > minimumSamplingInterval_)
                        {
                            continue;
                        }

                        // read the value.
                        var value = new DataValue();

                        var error = handle.Node.ReadAttribute(
                            SystemContext,
                            monitoredItem.AttributeId,
                            monitoredItem.IndexRange,
                            monitoredItem.DataEncoding,
                            value);

                        if (ServiceResult.IsBad(error))
                        {
                            value = new DataValue(error.StatusCode);
                        }

                        value.ServerTimestamp = DateTime.UtcNow;

                        // queue the value.
                        monitoredItem.QueueValue(value, error);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error during diagnostics scan.");
            }
        }
        #endregion

        #region Private Fields
        private ushort namespaceIndex_;
        private long lastUsedId_;
        private Timer diagnosticsScanTimer_;
        private int diagnosticsMonitoringCount_;
        private bool diagnosticsEnabled_;
        private bool doScanBusy_;
        private DateTime lastDiagnosticsScanTime_;
        private ServerDiagnosticsSummaryValue serverDiagnostics_;
        private NodeValueSimpleEventHandler serverDiagnosticsCallback_;
        private List<SessionDiagnosticsData> sessions_;
        private List<SubscriptionDiagnosticsData> subscriptions_;
        private NodeId serverLockHolder_;
        private Timer samplingTimer_;
        private List<UaMonitoredItem> sampledItems_;
        private double minimumSamplingInterval_;
        private HistoryServerCapabilitiesState historyCapabilities_;
        #endregion

        #region Private Readonly Fields
        private static readonly NodeId[] wellKnownRoles_ = {
            ObjectIds.WellKnownRole_Anonymous,
            ObjectIds.WellKnownRole_AuthenticatedUser,
            ObjectIds.WellKnownRole_ConfigureAdmin,
            ObjectIds.WellKnownRole_Engineer,
            ObjectIds.WellKnownRole_Observer,
            ObjectIds.WellKnownRole_Operator,
            ObjectIds.WellKnownRole_SecurityAdmin,
            ObjectIds.WellKnownRole_Supervisor };
        #endregion
    }
}
