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
using System.Diagnostics;
using System.Threading;

using Opc.Ua;

using Technosoftware.UaServer.Configuration;
using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// The master node manager for the server.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class MasterNodeManager : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object with default values.
        /// </summary>
        public MasterNodeManager(
            IUaServerData server,
            ApplicationConfiguration configuration,
            string dynamicNamespaceUri,
            params IUaBaseNodeManager[] additionalManagers)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            server_ = server;
            nodeManagers_ = new List<IUaBaseNodeManager>();
            maxContinuationPointsPerBrowse_ = (uint)configuration.ServerConfiguration.MaxBrowseContinuationPoints;

            // ensure the dynamic namespace uris.
            var dynamicNamespaceIndex = 1;

            if (!String.IsNullOrEmpty(dynamicNamespaceUri))
            {
                dynamicNamespaceIndex = server.NamespaceUris.GetIndex(dynamicNamespaceUri);

                if (dynamicNamespaceIndex == -1)
                {
                    dynamicNamespaceIndex = server.NamespaceUris.Append(dynamicNamespaceUri);
                }
            }

            // need to build a table of NamespaceIndexes and their NodeManagers.
            List<IUaBaseNodeManager> registeredManagers = null;
            var namespaceManagers = new Dictionary<int, List<IUaBaseNodeManager>>();

            namespaceManagers[0] = registeredManagers = new List<IUaBaseNodeManager>();
            namespaceManagers[1] = registeredManagers = new List<IUaBaseNodeManager>();

            // always add the diagnostics and configuration node manager to the start of the list.
            var configurationAndDiagnosticsManager = new ConfigurationNodeManager(server, configuration);
            RegisterNodeManager(configurationAndDiagnosticsManager, registeredManagers, namespaceManagers);

            // add the core node manager second because the diagnostics node manager takes priority.
            // always add the core node manager to the second of the list.
            nodeManagers_.Add(new CoreNodeManager(server_, configuration, (ushort)dynamicNamespaceIndex));

            // register core node manager for default UA namespace.
            namespaceManagers[0].Add(nodeManagers_[1]);

            // register core node manager for built-in server namespace.
            namespaceManagers[1].Add(nodeManagers_[1]);

            // add the custom NodeManagers provided by the application.
            if (additionalManagers != null)
            {
                foreach (var nodeManager in additionalManagers)
                {
                    RegisterNodeManager(nodeManager, registeredManagers, namespaceManagers);
                }

                // build table from dictionary.
                namespaceManagers_ = new IUaBaseNodeManager[server_.NamespaceUris.Count][];

                for (var ii = 0; ii < namespaceManagers_.Length; ii++)
                {
                    if (namespaceManagers.TryGetValue(ii, out registeredManagers))
                    {
                        namespaceManagers_[ii] = registeredManagers.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Registers the node manager with the master node manager.
        /// </summary>
        private void RegisterNodeManager(
            IUaBaseNodeManager nodeManager,
            List<IUaBaseNodeManager> registeredManagers,
            Dictionary<int, List<IUaBaseNodeManager>> namespaceManagers)
        {
            nodeManagers_.Add(nodeManager);

            // ensure the NamespaceUris supported by the NodeManager are in the Server's NamespaceTable.
            if (nodeManager.NamespaceUris != null)
            {
                foreach (var namespaceUri in nodeManager.NamespaceUris)
                {
                    // look up the namespace uri.
                    var index = server_.NamespaceUris.GetIndex(namespaceUri);

                    if (index == -1)
                    {
                        index = server_.NamespaceUris.Append(namespaceUri);
                    }

                    // add manager to list for the namespace.
                    if (!namespaceManagers.TryGetValue(index, out registeredManagers))
                    {
                        namespaceManagers[index] = registeredManagers = new List<IUaBaseNodeManager>();
                    }

                    registeredManagers.Add(nodeManager);
                }
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                List<IUaBaseNodeManager> nodeManagers = null;

                lock (lock_)
                {
                    nodeManagers = new List<IUaBaseNodeManager>(nodeManagers_);
                    nodeManagers_.Clear();
                }

                foreach (var nodeManager in nodeManagers)
                {
                    Utils.SilentDispose(nodeManager);
                }
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Adds a reference to the table of external references.
        /// </summary>
        /// <remarks>
        /// This is a convenience function used by custom NodeManagers.
        /// </remarks>
        public static void CreateExternalReference(
            IDictionary<NodeId, IList<IReference>> externalReferences,
            NodeId sourceId,
            NodeId referenceTypeId,
            bool isInverse,
            NodeId targetId)
        {
            var reference = new ReferenceNode();

            reference.ReferenceTypeId = referenceTypeId;
            reference.IsInverse = isInverse;
            reference.TargetId = targetId;

            IList<IReference> references = null;

            if (!externalReferences.TryGetValue(sourceId, out references))
            {
                externalReferences[sourceId] = references = new List<IReference>();
            }

            references.Add(reference);
        }

        /// <summary>
        /// Determine the required history access permission depending on the HistoryUpdateDetails
        /// </summary>
        /// <param name="historyUpdateDetails">The HistoryUpdateDetails passed in</param>
        /// <returns>The corresponding history access permission</returns>
        protected static PermissionType DetermineHistoryAccessPermission(HistoryUpdateDetails historyUpdateDetails)
        {
            var detailsType = historyUpdateDetails.GetType();

            if (detailsType == typeof(UpdateDataDetails))
            {
                var updateDataDetails = (UpdateDataDetails)historyUpdateDetails;
                return GetHistoryPermissionType(updateDataDetails.PerformInsertReplace);
            }
            else if (detailsType == typeof(UpdateStructureDataDetails))
            {
                var updateStructureDataDetails = (UpdateStructureDataDetails)historyUpdateDetails;
                return GetHistoryPermissionType(updateStructureDataDetails.PerformInsertReplace);
            }
            else if (detailsType == typeof(UpdateEventDetails))
            {
                var updateEventDetails = (UpdateEventDetails)historyUpdateDetails;
                return GetHistoryPermissionType(updateEventDetails.PerformInsertReplace);
            }
            else if (detailsType == typeof(DeleteRawModifiedDetails) ||
                detailsType == typeof(DeleteAtTimeDetails) ||
                detailsType == typeof(DeleteEventDetails))
            {
                return PermissionType.DeleteHistory;
            }

            return PermissionType.ModifyHistory;
        }

        /// <summary>
        ///  Determine the History PermissionType depending on PerformUpdateType 
        /// </summary>
        /// <param name="updateType"></param>
        /// <returns>The corresponding PermissionType</returns>
        protected static PermissionType GetHistoryPermissionType(PerformUpdateType updateType)
        {
            switch (updateType)
            {
                case PerformUpdateType.Insert:
                    return PermissionType.InsertHistory;
                case PerformUpdateType.Update:
                    return PermissionType.InsertHistory | PermissionType.ModifyHistory;
                default: // PerformUpdateType.Replace or PerformUpdateType.Remove
                    return PermissionType.ModifyHistory;
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Returns the core node manager.
        /// </summary>
        public CoreNodeManager CoreNodeManager
        {
            get
            {
                return nodeManagers_[1] as CoreNodeManager;
            }
        }

        /// <summary>
        /// Returns the diagnostics node manager.
        /// </summary>
        public DiagnosticsNodeManager DiagnosticsNodeManager
        {
            get
            {
                return nodeManagers_[0] as DiagnosticsNodeManager;
            }
        }

        /// <summary>
        /// Returns the configuration node manager.
        /// </summary>
        public ConfigurationNodeManager ConfigurationNodeManager
        {
            get
            {
                return nodeManagers_[0] as ConfigurationNodeManager;
            }
        }

        /// <summary>
        /// Creates the node managers and start them
        /// </summary>
        public virtual void Startup()
        {
            lock (lock_)
            {
                Utils.LogInfo(
                    Utils.TraceMasks.StartStop,
                    "MasterNodeManager.Startup - NodeManagers={0}",
                    nodeManagers_.Count);

                // create the address spaces.
                var externalReferences = new Dictionary<NodeId, IList<IReference>>();

                for (var ii = 0; ii < nodeManagers_.Count; ii++)
                {
                    var nodeManager = nodeManagers_[ii];

                    try
                    {
                        nodeManager.CreateAddressSpace(externalReferences);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError(e, "Unexpected error creating address space for NodeManager #{0}.", ii);
                        throw;
                    }
                }

                // update external references.               
                for (var ii = 0; ii < nodeManagers_.Count; ii++)
                {
                    var nodeManager = nodeManagers_[ii];

                    try
                    {
                        nodeManager.AddReferences(externalReferences);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError(e, "Unexpected error adding references for NodeManager #{0}.", ii);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Signals that a session is closing.
        /// </summary>
        public virtual void SessionClosing(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions)
        {
            lock (lock_)
            {
                for (var ii = 0; ii < nodeManagers_.Count; ii++)
                {
                    var nodeManager = nodeManagers_[ii] as IUaNodeManager;

                    if (nodeManager != null)
                    {
                        try
                        {
                            nodeManager.SessionClosing(context, sessionId, deleteSubscriptions);
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e, "Unexpected error closing session for NodeManager #{0}.", ii);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shuts down the node managers.
        /// </summary>
        public virtual void Shutdown()
        {
            lock (lock_)
            {
                Utils.LogInfo(
                    Utils.TraceMasks.StartStop,
                    "MasterNodeManager.Shutdown - NodeManagers={0}",
                    nodeManagers_.Count);

                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.DeleteAddressSpace();
                }
            }
        }

        /// <summary>
        /// Registers the node manager as the node manager for Nodes in the specified namespace.
        /// </summary>
        /// <param name="namespaceUri">The URI of the namespace.</param>
        /// <param name="nodeManager">The NodeManager which owns node in the namespace.</param>
        /// <remarks>
        /// Multiple NodeManagers may register interest in a Namespace. 
        /// The order in which this method is called determines the precedence if multiple NodeManagers exist.
        ///     This method adds the namespaceUri to the Server's Namespace table if it does not already exist.
        /// 
        /// This method is thread safe and can be called at anytime.
        /// 
        /// This method does not have to be called for any namespaces that were in the NodeManager's 
        /// NamespaceUri property when the MasterNodeManager was created.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Throw if the namespaceUri or the nodeManager are null.</exception>
        public void RegisterNamespaceManager(string namespaceUri, IUaBaseNodeManager nodeManager)
        {
            if (String.IsNullOrEmpty(namespaceUri)) throw new ArgumentNullException(nameof(namespaceUri));
            if (nodeManager == null) throw new ArgumentNullException(nameof(nodeManager));

            // look up the namespace uri.
            var index = server_.NamespaceUris.GetIndex(namespaceUri);

            if (index < 0)
            {
                index = server_.NamespaceUris.Append(namespaceUri);
            }

            // allocate a new table (using arrays instead of collections because lookup efficiency is critical).
            var namespaceManagers = new IUaBaseNodeManager[server_.NamespaceUris.Count][];

            try
            {
                readWriterLockSlim_.EnterWriteLock();

                // copy existing values.
                for (int ii = 0; ii < namespaceManagers_.Length; ii++)
                {
                    if (namespaceManagers_.Length >= ii)
                    {
                        namespaceManagers[ii] = namespaceManagers_[ii];
                    }
                }

                // allocate a new array for the index being updated.
                IUaBaseNodeManager[] registeredManagers = namespaceManagers[index];

                if (registeredManagers == null)
                {
                    registeredManagers = new IUaBaseNodeManager[1];
                }
                else
                {
                    registeredManagers = new IUaBaseNodeManager[registeredManagers.Length + 1];
                    Array.Copy(namespaceManagers[index], registeredManagers, namespaceManagers[index].Length);
                }

                // add new node manager to the end of the list.
                registeredManagers[registeredManagers.Length - 1] = nodeManager;
                namespaceManagers[index] = registeredManagers;

                // replace the table.
                namespaceManagers_ = namespaceManagers;

            }
            finally
            {
                readWriterLockSlim_.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unregisters the node manager as the node manager for Nodes in the specified namespace.
        /// </summary>
        /// <param name="namespaceUri">The URI of the namespace.</param>
        /// <param name="nodeManager">The NodeManager which no longer owns nodes in the namespace.</param>
        /// <returns>A value indicating whether the node manager was successfully unregistered.</returns>
        /// <exception cref="ArgumentNullException">Throw if the namespaceUri or the nodeManager are null.</exception>
        public bool UnregisterNamespaceManager(string namespaceUri, IUaBaseNodeManager nodeManager)
        {
            if (String.IsNullOrEmpty(namespaceUri)) throw new ArgumentNullException(nameof(namespaceUri));
            if (nodeManager == null) throw new ArgumentNullException(nameof(nodeManager));

            // look up the namespace uri.
            int namespaceIndex = server_.NamespaceUris.GetIndex(namespaceUri);
            if (namespaceIndex < 0)
            {
                return false;
            }

            // look up the node manager in the registered node managers for the namespace.
            int nodeManagerIndex = Array.IndexOf(namespaceManagers_[namespaceIndex], nodeManager);
            if (nodeManagerIndex < 0)
            {
                return false;
            }

            // allocate a new table (using arrays instead of collections because lookup efficiency is critical).
            IUaBaseNodeManager[][] namespaceManagers = new IUaBaseNodeManager[server_.NamespaceUris.Count][];

            try
            {
                readWriterLockSlim_.EnterWriteLock();

                // copy existing values.
                for (int ii = 0; ii < namespaceManagers_.Length; ii++)
                {
                    if (namespaceManagers_.Length >= ii)
                    {
                        namespaceManagers[ii] = namespaceManagers_[ii];
                    }
                }

                // allocate a new smaller array to support element removal for the index being updated.
                IUaBaseNodeManager[] registeredManagers = new IUaBaseNodeManager[namespaceManagers[namespaceIndex].Length - 1];

                // begin by populating the new array with existing elements up to the target index. 
                if (nodeManagerIndex > 0)
                {
                    Array.Copy(
                        namespaceManagers[namespaceIndex],
                        0,
                        registeredManagers,
                        0,
                        nodeManagerIndex);
                }

                // finish by populating the new array with existing elements after the target index.
                if (nodeManagerIndex < namespaceManagers[namespaceIndex].Length - 1)
                {
                    Array.Copy(
                        namespaceManagers[namespaceIndex],
                        nodeManagerIndex + 1,
                        registeredManagers,
                        nodeManagerIndex,
                        namespaceManagers[namespaceIndex].Length - nodeManagerIndex - 1);
                }

                // update the array for the target index.
                namespaceManagers[namespaceIndex] = registeredManagers;

                // replace the table.
                namespaceManagers_ = namespaceManagers;

                return true;
            }
            finally
            {
                readWriterLockSlim_.ExitWriteLock();
            }
        }

        /// <summary>
        /// Returns node handle and its node manager.
        /// </summary>
        public virtual object GetManagerHandle(NodeId nodeId, out IUaBaseNodeManager nodeManager)
        {
            nodeManager = null;
            object handle = null;

            // null node ids have no manager.
            if (NodeId.IsNull(nodeId))
            {
                return null;
            }

            // use the namespace index to select the node manager.
            int index = nodeId.NamespaceIndex;

            try
            {
                readWriterLockSlim_.EnterReadLock();
           
                // check if node managers are registered - use the core node manager if unknown.
                if (index >= namespaceManagers_.Length || namespaceManagers_[index] == null)
                {
                    handle = nodeManagers_[1].GetManagerHandle(nodeId);

                    if (handle != null)
                    {
                        nodeManager = nodeManagers_[1];
                        return handle;
                    }

                    return null;
                }

                // check each of the registered node managers.
                IUaBaseNodeManager[] nodeManagers = namespaceManagers_[index];

                for (int ii = 0; ii < nodeManagers.Length; ii++)
                {
                    handle = nodeManagers[ii].GetManagerHandle(nodeId);

                    if (handle != null)
                    {
                        nodeManager = nodeManagers[ii];
                        return handle;
                    }
                }
            }
            finally
            {
                readWriterLockSlim_.ExitReadLock();
            }

            // node not recognized.
            return null;
        }

        /// <summary>
        /// Adds the references to the target.
        /// </summary>
        public virtual void AddReferences(NodeId sourceId, IList<IReference> references)
        {
            foreach (var reference in references)
            {
                // find source node.
                IUaBaseNodeManager nodeManager = null;
                var sourceHandle = GetManagerHandle(sourceId, out nodeManager);

                if (sourceHandle == null)
                {
                    continue;
                }

                // delete the reference.

                var map = new Dictionary<NodeId, IList<IReference>>();
                map.Add(sourceId, references);
                nodeManager.AddReferences(map);
            }
        }

        /// <summary>
        /// Deletes the references to the target.
        /// </summary>
        public virtual void DeleteReferences(NodeId targetId, IList<IReference> references)
        {
            foreach (ReferenceNode reference in references)
            {
                var sourceId = ExpandedNodeId.ToNodeId(reference.TargetId, server_.NamespaceUris);

                // find source node.
                IUaBaseNodeManager nodeManager = null;
                var sourceHandle = GetManagerHandle(sourceId, out nodeManager);

                if (sourceHandle == null)
                {
                    continue;
                }

                // delete the reference.
                nodeManager.DeleteReference(sourceHandle, reference.ReferenceTypeId, !reference.IsInverse, targetId, false);
            }
        }

        /// <summary>
        /// Deletes the specified references.
        /// </summary>
        public void RemoveReferences(List<LocalReference> referencesToRemove)
        {
            for (var ii = 0; ii < referencesToRemove.Count; ii++)
            {
                var reference = referencesToRemove[ii];

                // find source node.
                var sourceHandle = GetManagerHandle(reference.SourceId, out IUaBaseNodeManager nodeManager);

                if (sourceHandle == null)
                {
                    continue;
                }

                // delete the reference.
                nodeManager.DeleteReference(sourceHandle, reference.ReferenceTypeId, reference.IsInverse, reference.TargetId, false);
            }
        }

        #region Register/Unregister Nodes
        /// <summary>
        /// Registers a set of node ids.
        /// </summary>
        public virtual void RegisterNodes(
            UaServerOperationContext context,
            NodeIdCollection nodesToRegister,
            out NodeIdCollection registeredNodeIds)
        {
            if (nodesToRegister == null) throw new ArgumentNullException(nameof(nodesToRegister));

            // return the node id provided.
            registeredNodeIds = new NodeIdCollection(nodesToRegister.Count);

            for (var ii = 0; ii < nodesToRegister.Count; ii++)
            {
                registeredNodeIds.Add(nodesToRegister[ii]);
            }

            Utils.LogTrace(
                (int)Utils.TraceMasks.ServiceDetail,
                "MasterNodeManager.RegisterNodes - Count={0}",
                nodesToRegister.Count);

            // it is up to the node managers to assign the handles.
            /*
            List<bool> processedNodes = new List<bool>(new bool[itemsToDelete.Count]);

            for (int ii = 0; ii < nodeManagers_.Count; ii++)
            {
                nodeManagers_[ii].RegisterNodes(
                    context,
                    nodesToRegister,
                    registeredNodeIds,
                    processedNodes);
            }
            */
        }

        /// <summary>
        /// Unregisters a set of node ids.
        /// </summary>
        public virtual void UnregisterNodes(
            UaServerOperationContext context,
            NodeIdCollection nodesToUnregister)
        {
            if (nodesToUnregister == null) throw new ArgumentNullException(nameof(nodesToUnregister));

            Utils.LogTrace(
                (int)Utils.TraceMasks.ServiceDetail,
                "MasterNodeManager.UnregisterNodes - Count={0}",
                nodesToUnregister.Count);

            // it is up to the node managers to assign the handles.
            /*
            List<bool> processedNodes = new List<bool>(new bool[itemsToDelete.Count]);

            for (int ii = 0; ii < nodeManagers_.Count; ii++)
            {
                nodeManagers_[ii].RegisterNodes(
                    context,
                    nodesToUnregister,
                    processedNodes);
            }
            */
        }
        #endregion

        #region TranslateBrowsePathsToNodeIds
        /// <summary>
        /// Translates a start node id plus a relative paths into a node id.
        /// </summary>
        public virtual void TranslateBrowsePathsToNodeIds(
            UaServerOperationContext context,
            BrowsePathCollection browsePaths,
            out BrowsePathResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (browsePaths == null) throw new ArgumentNullException(nameof(browsePaths));

            var diagnosticsExist = false;
            results = new BrowsePathResultCollection(browsePaths.Count);
            diagnosticInfos = new DiagnosticInfoCollection(browsePaths.Count);

            for (var ii = 0; ii < browsePaths.Count; ii++)
            {
                // check if request has timed out or been cancelled.
                if (StatusCode.IsBad(context.OperationStatus))
                {
                    throw new ServiceResultException(context.OperationStatus);
                }

                var browsePath = browsePaths[ii];

                var result = new BrowsePathResult();
                result.StatusCode = StatusCodes.Good;
                results.Add(result);

                ServiceResult error = null;

                // need to trap unexpected exceptions to handle bugs in the node managers.
                try
                {
                    error = TranslateBrowsePath(context, browsePath, result);
                }
                catch (Exception e)
                {
                    error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, "Unexpected error translating browse path.");
                }

                if (ServiceResult.IsGood(error))
                {
                    // check for no match.
                    if (result.Targets.Count == 0)
                    {
                        error = StatusCodes.BadNoMatch;
                    }

                    // put a placeholder for diagnostics.
                    else if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }
                }

                // check for error.
                if (error != null && error.Code != StatusCodes.Good)
                {
                    result.StatusCode = error.StatusCode;

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                        diagnosticInfos.Add(diagnosticInfo);
                        diagnosticsExist = true;
                    }
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Updates the diagnostics return parameter.
        /// </summary>
        private void UpdateDiagnostics(
            UaServerOperationContext context,
            bool diagnosticsExist,
            ref DiagnosticInfoCollection diagnosticInfos)
        {
            if (diagnosticInfos == null)
            {
                return;
            }

            if (diagnosticsExist && context.StringTable.Count == 0)
            {
                diagnosticsExist = false;

                for (var ii = 0; !diagnosticsExist && ii < diagnosticInfos.Count; ii++)
                {
                    var diagnosticInfo = diagnosticInfos[ii];

                    while (diagnosticInfo != null)
                    {
                        if (!String.IsNullOrEmpty(diagnosticInfo.AdditionalInfo))
                        {
                            diagnosticsExist = true;
                            break;
                        }

                        diagnosticInfo = diagnosticInfo.InnerDiagnosticInfo;
                    }
                }
            }

            if (!diagnosticsExist)
            {
                diagnosticInfos = null;
            }
        }

        /// <summary>
        /// Translates a browse path.
        /// </summary>
        protected ServiceResult TranslateBrowsePath(
            UaServerOperationContext context,
            BrowsePath browsePath,
            BrowsePathResult result)
        {
            Debug.Assert(browsePath != null);
            Debug.Assert(result != null);

            // check for valid start node.
            IUaBaseNodeManager nodeManager = null;

            var sourceHandle = GetManagerHandle(browsePath.StartingNode, out nodeManager);

            if (sourceHandle == null)
            {
                return StatusCodes.BadNodeIdUnknown;
            }

            // check the relative path.
            var relativePath = browsePath.RelativePath;

            if (relativePath.Elements == null || relativePath.Elements.Count == 0)
            {
                return StatusCodes.BadNothingToDo;
            }

            for (var ii = 0; ii < relativePath.Elements.Count; ii++)
            {
                var element = relativePath.Elements[ii];

                if (element == null || QualifiedName.IsNull(relativePath.Elements[ii].TargetName))
                {
                    return StatusCodes.BadBrowseNameInvalid;
                }

                if (NodeId.IsNull(element.ReferenceTypeId))
                {
                    element.ReferenceTypeId = ReferenceTypeIds.References;
                    element.IncludeSubtypes = true;
                }
            }
            // validate access rights and role permissions
            var serviceResult = ValidatePermissions(context, nodeManager, sourceHandle, PermissionType.Browse, null, true);
            if (ServiceResult.IsGood(serviceResult))
            {
                // translate path only if validation is passing
                TranslateBrowsePath(
                    context,
                    nodeManager,
                    sourceHandle,
                    relativePath,
                    result.Targets,
                    0);
            }

            return serviceResult;
        }

        /// <summary>
        /// Recursively processes the elements in the RelativePath starting at the specified index.
        /// </summary>
        private void TranslateBrowsePath(
            UaServerOperationContext context,
            IUaBaseNodeManager nodeManager,
            object sourceHandle,
            RelativePath relativePath,
            BrowsePathTargetCollection targets,
            int index)
        {
            Debug.Assert(nodeManager != null);
            Debug.Assert(sourceHandle != null);
            Debug.Assert(relativePath != null);
            Debug.Assert(targets != null);

            // check for end of list.
            if (index < 0 || index >= relativePath.Elements.Count)
            {
                return;
            }

            // follow the next hop.
            var element = relativePath.Elements[index];

            // check for valid reference type.
            if (!element.IncludeSubtypes && NodeId.IsNull(element.ReferenceTypeId))
            {
                return;
            }

            // check for valid target name.
            if (QualifiedName.IsNull(element.TargetName))
            {
                throw new ServiceResultException(StatusCodes.BadBrowseNameInvalid);
            }

            var targetIds = new List<ExpandedNodeId>();
            var externalTargetIds = new List<NodeId>();

            try
            {
                nodeManager.TranslateBrowsePath(
                    context,
                    sourceHandle,
                    element,
                    targetIds,
                    externalTargetIds);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error translating browse path.");
                return;
            }

            // must check the browse name on all external targets.
            for (var ii = 0; ii < externalTargetIds.Count; ii++)
            {
                // get the browse name from another node manager.
                var description = new ReferenceDescription();

                UpdateReferenceDescription(
                    context,
                    externalTargetIds[ii],
                    NodeClass.Unspecified,
                    BrowseResultMask.BrowseName,
                    description);

                // add to list if target name matches.
                if (description.BrowseName == element.TargetName)
                {
                    var found = false;

                    for (var jj = 0; jj < targetIds.Count; jj++)
                    {
                        if (targetIds[jj] == externalTargetIds[ii])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        targetIds.Add(externalTargetIds[ii]);
                    }
                }
            }

            // check if done after a final hop.
            if (index == relativePath.Elements.Count - 1)
            {
                for (var ii = 0; ii < targetIds.Count; ii++)
                {
                    // Check the role permissions for target nodes
                    IUaBaseNodeManager targetNodeManager = null;
                    var targetHandle = GetManagerHandle(ExpandedNodeId.ToNodeId(targetIds[ii], Server.NamespaceUris), out targetNodeManager);

                    if (targetHandle != null && targetNodeManager != null)
                    {
                        var nodeMetadata = targetNodeManager.GetNodeMetadata(context, targetHandle, BrowseResultMask.All);
                        var serviceResult = ValidateRolePermissions(context, nodeMetadata, PermissionType.Browse);

                        if (ServiceResult.IsBad(serviceResult))
                        {
                            // Remove target node without role permissions.
                            continue;
                        }
                    }

                    var target = new BrowsePathTarget();
                    target.TargetId = targetIds[ii];
                    target.RemainingPathIndex = UInt32.MaxValue;

                    targets.Add(target);
                }

                return;
            }

            // process next hops.
            for (var ii = 0; ii < targetIds.Count; ii++)
            {
                var targetId = targetIds[ii];

                // check for external reference.
                if (targetId.IsAbsolute)
                {
                    var target = new BrowsePathTarget();

                    target.TargetId = targetId;
                    target.RemainingPathIndex = (uint)(index + 1);

                    targets.Add(target);
                    continue;
                }

                // check for valid start node.   
                sourceHandle = GetManagerHandle((NodeId)targetId, out nodeManager);

                if (sourceHandle == null)
                {
                    continue;
                }

                // recursively follow hops.
                TranslateBrowsePath(
                    context,
                    nodeManager,
                    sourceHandle,
                    relativePath,
                    targets,
                    index + 1);
            }
        }
        #endregion

        #region Browse
        /// <summary>
        /// Returns the set of references that meet the filter criteria.
        /// </summary>
        public virtual void Browse(
            UaServerOperationContext context,
            ViewDescription view,
            uint maxReferencesPerNode,
            BrowseDescriptionCollection nodesToBrowse,
            out BrowseResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (nodesToBrowse == null) throw new ArgumentNullException(nameof(nodesToBrowse));

            if (view != null && !NodeId.IsNull(view.ViewId))
            {
                IUaBaseNodeManager viewManager = null;
                var viewHandle = GetManagerHandle(view.ViewId, out viewManager);

                if (viewHandle == null)
                {
                    throw new ServiceResultException(StatusCodes.BadViewIdUnknown);
                }

                var metadata = viewManager.GetNodeMetadata(context, viewHandle, BrowseResultMask.NodeClass);

                if (metadata == null || metadata.NodeClass != NodeClass.View)
                {
                    throw new ServiceResultException(StatusCodes.BadViewIdUnknown);
                }

                // validate access rights and role permissions
                var validationResult = ValidatePermissions(context, viewManager, viewHandle, PermissionType.Browse, null, true);
                if (ServiceResult.IsBad(validationResult))
                {
                    throw new ServiceResultException(validationResult);
                }
                view.Handle = viewHandle;
            }

            var diagnosticsExist = false;
            results = new BrowseResultCollection(nodesToBrowse.Count);
            diagnosticInfos = new DiagnosticInfoCollection(nodesToBrowse.Count);

            uint continuationPointsAssigned = 0;

            for (var ii = 0; ii < nodesToBrowse.Count; ii++)
            {
                // check if request has timed out or been cancelled.
                if (StatusCode.IsBad(context.OperationStatus))
                {
                    // release all allocated continuation points.
                    foreach (var current in results)
                    {
                        if (current != null && current.ContinuationPoint != null && current.ContinuationPoint.Length > 0)
                        {
                            var cp = context.Session.RestoreContinuationPoint(current.ContinuationPoint);
                            cp.Dispose();
                        }
                    }

                    throw new ServiceResultException(context.OperationStatus);
                }

                var nodeToBrowse = nodesToBrowse[ii];

                // initialize result.
                var result = new BrowseResult();
                result.StatusCode = StatusCodes.Good;
                results.Add(result);

                ServiceResult error = null;

                // need to trap unexpected exceptions to handle bugs in the node managers.
                try
                {
                    error = Browse(
                        context,
                        view,
                        maxReferencesPerNode,
                        continuationPointsAssigned < maxContinuationPointsPerBrowse_,
                        nodeToBrowse,
                        result);
                }
                catch (Exception e)
                {
                    error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, "Unexpected error browsing node.");
                }

                // check for continuation point.
                if (result.ContinuationPoint != null && result.ContinuationPoint.Length > 0)
                {
                    continuationPointsAssigned++;
                }

                // check for error.   
                result.StatusCode = error.StatusCode;

                if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                {
                    DiagnosticInfo diagnosticInfo = null;

                    if (error != null && error.Code != StatusCodes.Good)
                    {
                        diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                        diagnosticsExist = true;
                    }

                    diagnosticInfos.Add(diagnosticInfo);
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Prepare a cache per NodeManager and unique NodeId that holds the attributes needed to validate the AccessRestrictions and RolePermissions.
        /// This cache is then used in subsequenct calls to avoid triggering unnecessary time consuming callbacks.
        /// The current services that benefit from this are the Read service
        /// </summary>
        /// <typeparam name="T">One of the following types used in the service calls:
        ///     ReadValueId used in the Read service</typeparam>
        /// <param name="nodesCollection">The collection of nodes on which the service operates uppon</param>
        /// <param name="uniqueNodesServiceAttributes">The resulting cache that holds the values of the AccessRestrictions and RolePermissions attributes needed for Read service</param>
        private void PrepareValidationCache<T>(List<T> nodesCollection,
            out Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes)
        {
            List<NodeId> uniqueNodes = new List<NodeId>();
            for (int i = 0; i < nodesCollection.Count; i++)
            {
                Type listType = typeof(T);
                NodeId nodeId = null;

                if (listType == typeof(ReadValueId))
                {
                    nodeId = (nodesCollection[i] as ReadValueId)?.NodeId;
                }

                if (nodeId == null)
                {
                    throw new ArgumentException("Provided List<T> nodesCollection is of wrong type, T should be type BrowseDescription, ReadValueId or CallMethodRequest", nameof(nodesCollection));
                }

                if (!uniqueNodes.Contains(nodeId))
                {
                    uniqueNodes.Add(nodeId);
                }
            }
            // uniqueNodesReadAttributes is the place where the attributes for each unique nodeId are kept on the services
            uniqueNodesServiceAttributes = new Dictionary<NodeId, List<object>>();
            foreach (var uniqueNode in uniqueNodes)
            {
                uniqueNodesServiceAttributes.Add(uniqueNode, new List<object>());
            }
        }

        /// <summary>
        /// Continues a browse operation that was previously halted.
        /// </summary>
        public virtual void BrowseNext(
            UaServerOperationContext context,
            bool releaseContinuationPoints,
            ByteStringCollection continuationPoints,
            out BrowseResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (continuationPoints == null) throw new ArgumentNullException(nameof(continuationPoints));

            var diagnosticsExist = false;
            results = new BrowseResultCollection(continuationPoints.Count);
            diagnosticInfos = new DiagnosticInfoCollection(continuationPoints.Count);

            uint continuationPointsAssigned = 0;

            for (var ii = 0; ii < continuationPoints.Count; ii++)
            {
                UaContinuationPoint cp = null;

                // check if request has timed out or been canceled.
                if (StatusCode.IsBad(context.OperationStatus))
                {
                    // release all allocated continuation points.
                    foreach (var current in results)
                    {
                        if (current != null && current.ContinuationPoint != null && current.ContinuationPoint.Length > 0)
                        {
                            cp = context.Session.RestoreContinuationPoint(current.ContinuationPoint);
                            cp.Dispose();
                        }
                    }

                    throw new ServiceResultException(context.OperationStatus);
                }

                // find the continuation point.
                cp = context.Session.RestoreContinuationPoint(continuationPoints[ii]);

                // validate access rights and role permissions
                if (cp != null)
                {
                    var validationResult = ValidatePermissions(context, cp.Manager, cp.NodeToBrowse, PermissionType.Browse, null, true);
                    if (ServiceResult.IsBad(validationResult))
                    {
                        var badResult = new BrowseResult();
                        badResult.StatusCode = validationResult.Code;
                        results.Add(badResult);

                        // put placeholder for diagnostics
                        diagnosticInfos.Add(null);
                        continue;
                    }
                }

                // initialize result.    
                var result = new BrowseResult();
                result.StatusCode = StatusCodes.Good;
                results.Add(result);

                // check if simply releasing the continuation point.
                if (releaseContinuationPoints)
                {
                    if (cp != null)
                    {
                        cp.Dispose();
                        cp = null;
                    }

                    continue;
                }

                ServiceResult error = null;

                // check if continuation point has expired.
                if (cp == null)
                {
                    error = StatusCodes.BadContinuationPointInvalid;
                }

                if (cp != null)
                {
                    // need to trap unexpected exceptions to handle bugs in the node managers.
                    try
                    {
                        var references = result.References;

                        error = FetchReferences(
                            context,
                            continuationPointsAssigned < maxContinuationPointsPerBrowse_,
                            ref cp,
                            ref references);

                        result.References = references;
                    }
                    catch (Exception e)
                    {
                        error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, "Unexpected error browsing node.");
                    }

                    // check for continuation point.
                    if (result.ContinuationPoint != null && result.ContinuationPoint.Length > 0)
                    {
                        continuationPointsAssigned++;
                    }
                }

                // check for error.
                result.StatusCode = error.StatusCode;

                if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                {
                    DiagnosticInfo diagnosticInfo = null;

                    if (error != null && error.Code != StatusCodes.Good)
                    {
                        diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                        diagnosticsExist = true;
                    }

                    diagnosticInfos.Add(diagnosticInfo);
                }

                // check for continuation point.
                if (cp != null)
                {
                    result.StatusCode = StatusCodes.Good;
                    result.ContinuationPoint = cp.Id.ToByteArray();
                    continue;
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Returns the set of references that meet the filter criteria.
        /// </summary>
        private ServiceResult Browse(
            UaServerOperationContext context,
            ViewDescription view,
            uint maxReferencesPerNode,
            bool assignContinuationPoint,
            BrowseDescription nodeToBrowse,
            BrowseResult result)
        {
            Debug.Assert(context != null);
            Debug.Assert(nodeToBrowse != null);
            Debug.Assert(result != null);

            // find node manager that owns the node.
            IUaBaseNodeManager nodeManager = null;

            var handle = GetManagerHandle(nodeToBrowse.NodeId, out nodeManager);

            if (handle == null)
            {
                return StatusCodes.BadNodeIdUnknown;
            }

            if (!NodeId.IsNull(nodeToBrowse.ReferenceTypeId) && !server_.TypeTree.IsKnown(nodeToBrowse.ReferenceTypeId))
            {
                return StatusCodes.BadReferenceTypeIdInvalid;
            }

            if (nodeToBrowse.BrowseDirection < BrowseDirection.Forward || nodeToBrowse.BrowseDirection > BrowseDirection.Both)
            {
                return StatusCodes.BadBrowseDirectionInvalid;
            }

            // validate access rights and role permissions
            var validationResult = ValidatePermissions(context, nodeManager, handle, PermissionType.Browse, null, true);
            if (ServiceResult.IsBad(validationResult))
            {
                return validationResult;
            }

            // create a continuation point.
            var cp = new UaContinuationPoint();

            cp.Manager = nodeManager;
            cp.View = view;
            cp.NodeToBrowse = handle;
            cp.MaxResultsToReturn = maxReferencesPerNode;
            cp.BrowseDirection = nodeToBrowse.BrowseDirection;
            cp.ReferenceTypeId = nodeToBrowse.ReferenceTypeId;
            cp.IncludeSubtypes = nodeToBrowse.IncludeSubtypes;
            cp.NodeClassMask = nodeToBrowse.NodeClassMask;
            cp.ResultMask = (BrowseResultMask)nodeToBrowse.ResultMask;
            cp.Index = 0;
            cp.Data = null;

            // check if reference type left unspecified.
            if (NodeId.IsNull(cp.ReferenceTypeId))
            {
                cp.ReferenceTypeId = ReferenceTypeIds.References;
                cp.IncludeSubtypes = true;
            }

            // loop until browse is complete or max results.
            var references = result.References;
            var error = FetchReferences(context, assignContinuationPoint, ref cp, ref references);
            result.References = references;

            // save continuation point.
            if (cp != null)
            {
                result.StatusCode = StatusCodes.Good;
                result.ContinuationPoint = cp.Id.ToByteArray();
            }

            // all is good.
            return error;
        }

        /// <summary>
        /// Loops until browse is complete for max results reached.
        /// </summary>
        protected ServiceResult FetchReferences(
            UaServerOperationContext context,
            bool assignContinuationPoint,
            ref UaContinuationPoint cp,
            ref ReferenceDescriptionCollection references)
        {
            Debug.Assert(context != null);
            Debug.Assert(cp != null);
            Debug.Assert(references != null);

            var nodeManager = cp.Manager;
            var nodeClassMask = (NodeClass)cp.NodeClassMask;
            var resultMask = cp.ResultMask;

            // loop until browse is complete or max results.
            while (cp != null)
            {
                // fetch next batch.
                nodeManager.Browse(context, ref cp, references);

                var referencesToKeep = new ReferenceDescriptionCollection(references.Count);

                // check for incomplete reference descriptions.
                for (var ii = 0; ii < references.Count; ii++)
                {
                    var reference = references[ii];

                    // check if filtering must be applied.
                    if (reference.Unfiltered)
                    {
                        // ignore unknown external references.
                        if (reference.NodeId.IsAbsolute)
                        {
                            continue;
                        }

                        // update the description.
                        var include = UpdateReferenceDescription(
                            context,
                            (NodeId)reference.NodeId,
                            nodeClassMask,
                            resultMask,
                            reference);

                        if (!include)
                        {
                            continue;
                        }
                    }

                    // add to list.
                    referencesToKeep.Add(reference);
                }

                // replace list.
                references = referencesToKeep;

                // check if browse limit reached.
                if (cp != null && references.Count >= cp.MaxResultsToReturn)
                {
                    if (!assignContinuationPoint)
                    {
                        return StatusCodes.BadNoContinuationPoints;
                    }

                    cp.Id = Guid.NewGuid();
                    context.Session.SaveContinuationPoint(cp);
                    break;
                }
            }

            // all is good.
            return ServiceResult.Good;
        }
        #endregion

        /// <summary>
        /// Updates the reference description with the node attributes.
        /// </summary>
        private bool UpdateReferenceDescription(
            UaServerOperationContext context,
            NodeId targetId,
            NodeClass nodeClassMask,
            BrowseResultMask resultMask,
            ReferenceDescription description)
        {
            if (targetId == null) throw new ArgumentNullException(nameof(targetId));
            if (description == null) throw new ArgumentNullException(nameof(description));

            // find node manager that owns the node.
            IUaBaseNodeManager nodeManager = null;
            var handle = GetManagerHandle(targetId, out nodeManager);

            // dangling reference - nothing more to do.
            if (handle == null)
            {
                return false;
            }

            // fetch the node attributes.
            var metadata = nodeManager.GetNodeMetadata(context, handle, resultMask);

            if (metadata == null)
            {
                return false;
            }

            // check nodeclass filter.
            if (nodeClassMask != NodeClass.Unspecified && (metadata.NodeClass & nodeClassMask) == 0)
            {
                return false;
            }

            // update attributes.
            description.NodeId = metadata.NodeId;

            description.SetTargetAttributes(
                resultMask,
                metadata.NodeClass,
                metadata.BrowseName,
                metadata.DisplayName,
                metadata.TypeDefinition);

            description.Unfiltered = false;

            return true;
        }

        /// <summary>
        /// Reads a set of nodes.
        /// </summary>
        public virtual void Read(
            UaServerOperationContext context,
            double maxAge,
            TimestampsToReturn timestampsToReturn,
            ReadValueIdCollection nodesToRead,
            out DataValueCollection values,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (nodesToRead == null) throw new ArgumentNullException(nameof(nodesToRead));

            if (maxAge < 0)
            {
                throw new ServiceResultException(StatusCodes.BadMaxAgeInvalid);
            }

            if (timestampsToReturn < TimestampsToReturn.Source || timestampsToReturn > TimestampsToReturn.Neither)
            {
                throw new ServiceResultException(StatusCodes.BadTimestampsToReturnInvalid);
            }

            var diagnosticsExist = false;
            values = new DataValueCollection(nodesToRead.Count);
            diagnosticInfos = new DiagnosticInfoCollection(nodesToRead.Count);

            // create empty list of errors.
            var errors = new List<ServiceResult>(values.Count);
            for (var ii = 0; ii < nodesToRead.Count; ii++)
            {
                errors.Add(null);
            }

            // add placeholder for each result.
            var validItems = false;

            Utils.LogTrace(
                (int)Utils.TraceMasks.ServiceDetail,
                "MasterNodeManager.Read - Count={0}",
                nodesToRead.Count);

            Dictionary<NodeId, List<object>> uniqueNodesReadAttributes;
            PrepareValidationCache(nodesToRead, out uniqueNodesReadAttributes);

            for (var ii = 0; ii < nodesToRead.Count; ii++)
            {
                // add default value to values collection
                values.Add(null);
                // add placeholder for diagnostics
                diagnosticInfos.Add(null);

                // pre-validate and pre-parse parameter.
                errors[ii] = ValidateReadRequest(context, nodesToRead[ii], uniqueNodesReadAttributes);

                // return error status.
                if (ServiceResult.IsBad(errors[ii]))
                {
                    nodesToRead[ii].Processed = true;
                }
                // found at least one valid item.
                else
                {
                    nodesToRead[ii].Processed = false;
                    validItems = true;
                }
            }

            // call each node manager.
            if (validItems)
            {
                for (var ii = 0; ii < nodeManagers_.Count; ii++)
                {
#if VERBOSE
                    Utils.LogTrace(
                        (int)Utils.TraceMasks.ServiceDetail,
                        "MasterNodeManager.Read - Calling NodeManager {0} of {1}",
                        ii,
                        nodeManagers_.Count);
#endif
                    nodeManagers_[ii].Read(
                        context,
                        maxAge,
                        nodesToRead,
                        values,
                        errors);
                }
            }

            // process results.
            for (var ii = 0; ii < nodesToRead.Count; ii++)
            {
                var value = values[ii];

                // set an error code for nodes that were not handled by any node manager.
                if (!nodesToRead[ii].Processed)
                {
                    value = values[ii] = new DataValue(StatusCodes.BadNodeIdUnknown, DateTime.UtcNow);
                    errors[ii] = new ServiceResult(values[ii].StatusCode);
                }

                // update the diagnostic info and ensure the status code in the data value is the same as the error code.
                if (errors[ii] != null && errors[ii].Code != StatusCodes.Good)
                {
                    if (value == null)
                    {
                        value = values[ii] = new DataValue(errors[ii].Code, DateTime.UtcNow);
                    }

                    value.StatusCode = errors[ii].Code;

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                        diagnosticsExist = true;
                    }
                }

                // apply the timestamp filters.
                if (timestampsToReturn != TimestampsToReturn.Server && timestampsToReturn != TimestampsToReturn.Both)
                {
                    value.ServerTimestamp = DateTime.MinValue;
                }

                if (timestampsToReturn != TimestampsToReturn.Source && timestampsToReturn != TimestampsToReturn.Both)
                {
                    value.SourceTimestamp = DateTime.MinValue;
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Reads the history of a set of items.
        /// </summary>
        public virtual void HistoryRead(
            UaServerOperationContext context,
            ExtensionObject historyReadDetails,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueIdCollection nodesToRead,
            out HistoryReadResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            // validate history details parameter.
            if (ExtensionObject.IsNull(historyReadDetails))
            {
                throw new ServiceResultException(StatusCodes.BadHistoryOperationInvalid);
            }

            var details = historyReadDetails.Body as HistoryReadDetails;

            if (details == null)
            {
                throw new ServiceResultException(StatusCodes.BadHistoryOperationUnsupported);
            }

            // create result lists.
            var diagnosticsExist = false;
            results = new HistoryReadResultCollection(nodesToRead.Count);
            diagnosticInfos = new DiagnosticInfoCollection(nodesToRead.Count);

            // pre-validate items.
            var validItems = false;
            // create empty list of errors.
            var errors = new List<ServiceResult>(results.Count);
            for (var ii = 0; ii < nodesToRead.Count; ii++)
            {
                errors.Add(null);
            }

            for (var ii = 0; ii < nodesToRead.Count; ii++)
            {
                // Limit permission restrictions to Client initiated service call                
                HistoryReadResult result = null;
                DiagnosticInfo diagnosticInfo = null;

                // pre-validate and pre-parse parameter.
                errors[ii] = ValidateHistoryReadRequest(context, nodesToRead[ii]);

                // return error status.
                if (ServiceResult.IsBad(errors[ii]))
                {
                    nodesToRead[ii].Processed = true;
                    result = new HistoryReadResult();
                    result.StatusCode = errors[ii].Code;

                    // add diagnostics if requested.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                        diagnosticsExist = true;
                    }
                }
                // found at least one valid item.
                else
                {
                    nodesToRead[ii].Processed = false;
                    validItems = true;
                }

                results.Add(result);
                diagnosticInfos.Add(diagnosticInfo);
            }

            // call each node manager.
            if (validItems)
            {
                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.HistoryRead(
                        context,
                        details,
                        timestampsToReturn,
                        releaseContinuationPoints,
                        nodesToRead,
                        results,
                        errors);
                }

                for (var ii = 0; ii < nodesToRead.Count; ii++)
                {
                    var result = results[ii];

                    // set an error code for nodes that were not handled by any node manager.
                    if (!nodesToRead[ii].Processed)
                    {
                        nodesToRead[ii].Processed = true;
                        result = results[ii] = new HistoryReadResult();
                        result.StatusCode = StatusCodes.BadNodeIdUnknown;
                        errors[ii] = results[ii].StatusCode;
                    }

                    // update the diagnostic info and ensure the status code in the result is the same as the error code.
                    if (errors[ii] != null && errors[ii].Code != StatusCodes.Good)
                    {
                        if (result == null)
                        {
                            result = results[ii] = new HistoryReadResult();
                        }

                        result.StatusCode = errors[ii].Code;

                        // add diagnostics if requested.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                        }
                    }
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Writes a set of values.
        /// </summary>
        public virtual void Write(
            UaServerOperationContext context,
            WriteValueCollection nodesToWrite,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (nodesToWrite == null) throw new ArgumentNullException(nameof(nodesToWrite));

            var count = nodesToWrite.Count;

            var diagnosticsExist = false;
            results = new StatusCodeCollection(count);
            diagnosticInfos = new DiagnosticInfoCollection(count);

            // add placeholder for each result.
            var validItems = false;

            for (var ii = 0; ii < count; ii++)
            {
                StatusCode result = StatusCodes.Good;
                DiagnosticInfo diagnosticInfo = null;

                // pre-validate and pre-parse parameter. Validate also access rights and role permissions
                var error = ValidateWriteRequest(context, nodesToWrite[ii]);

                // return error status.
                if (ServiceResult.IsBad(error))
                {
                    nodesToWrite[ii].Processed = true;
                    result = error.Code;

                    // add diagnostics if requested.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                        diagnosticsExist = true;
                    }
                }

                // found at least one valid item.
                else
                {
                    nodesToWrite[ii].Processed = false;
                    validItems = true;
                }

                results.Add(result);
                diagnosticInfos.Add(diagnosticInfo);
            }

            // call each node manager.
            if (validItems)
            {
                var errors = new List<ServiceResult>(count);
                errors.AddRange(new ServiceResult[count]);

                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.Write(
                        context,
                        nodesToWrite,
                        errors);
                }

                for (var ii = 0; ii < nodesToWrite.Count; ii++)
                {
                    if (!nodesToWrite[ii].Processed)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;
                    }

                    if (errors[ii] != null && errors[ii].Code != StatusCodes.Good)
                    {
                        results[ii] = errors[ii].Code;

                        // add diagnostics if requested.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                        }
                    }

                    UaServerUtils.ReportWriteValue(nodesToWrite[ii].NodeId, nodesToWrite[ii].Value, results[ii]);
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Updates the history for a set of nodes.
        /// </summary>
        public virtual void HistoryUpdate(
            UaServerOperationContext context,
            ExtensionObjectCollection historyUpdateDetails,
            out HistoryUpdateResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            Type detailsType = null;
            var nodesToUpdate = new List<HistoryUpdateDetails>();

            // verify that all extension objects in the list have the same type.
            foreach (var details in historyUpdateDetails)
            {
                if (detailsType == null)
                {
                    detailsType = details.Body.GetType();
                }

                if (!ExtensionObject.IsNull(details))
                {
                    nodesToUpdate.Add(details.Body as HistoryUpdateDetails);
                }
            }

            // create result lists.
            var diagnosticsExist = false;
            results = new HistoryUpdateResultCollection(nodesToUpdate.Count);
            diagnosticInfos = new DiagnosticInfoCollection(nodesToUpdate.Count);

            // pre-validate items.
            var validItems = false;

            // create empty list of errors.
            var errors = new List<ServiceResult>(results.Count);
            for (var ii = 0; ii < nodesToUpdate.Count; ii++)
            {
                errors.Add(null);
            }

            for (var ii = 0; ii < nodesToUpdate.Count; ii++)
            {
                HistoryUpdateResult result = null;
                DiagnosticInfo diagnosticInfo = null;

                // check the type of details parameter.
                ServiceResult error = null;

                if (nodesToUpdate[ii].GetType() != detailsType)
                {
                    error = StatusCodes.BadHistoryOperationInvalid;
                }
                // pre-validate and pre-parse parameter.
                else
                {
                    error = ValidateHistoryUpdateRequest(context, nodesToUpdate[ii]);
                }

                // return error status.
                if (ServiceResult.IsBad(error))
                {
                    nodesToUpdate[ii].Processed = true;
                    result = new HistoryUpdateResult();
                    result.StatusCode = error.Code;

                    // add diagnostics if requested.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                        diagnosticsExist = true;
                    }
                }
                // found at least one valid item.
                else
                {
                    nodesToUpdate[ii].Processed = false;
                    validItems = true;
                }

                results.Add(result);
                diagnosticInfos.Add(diagnosticInfo);
            }

            // call each node manager.
            if (validItems)
            {
                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.HistoryUpdate(
                        context,
                        detailsType,
                        nodesToUpdate,
                        results,
                        errors);
                }

                for (var ii = 0; ii < nodesToUpdate.Count; ii++)
                {
                    var result = results[ii];

                    // set an error code for nodes that were not handled by any node manager.
                    if (!nodesToUpdate[ii].Processed)
                    {
                        nodesToUpdate[ii].Processed = true;
                        result = results[ii] = new HistoryUpdateResult();
                        result.StatusCode = StatusCodes.BadNodeIdUnknown;
                        errors[ii] = result.StatusCode;
                    }

                    // update the diagnostic info and ensure the status code in the result is the same as the error code.
                    if (errors[ii] != null && errors[ii].Code != StatusCodes.Good)
                    {
                        if (result == null)
                        {
                            result = results[ii] = new HistoryUpdateResult();
                        }

                        result.StatusCode = errors[ii].Code;

                        // add diagnostics if requested.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                        }
                    }
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Calls a method defined on a object.
        /// </summary>
        public virtual void Call(
            UaServerOperationContext context,
            CallMethodRequestCollection methodsToCall,
            out CallMethodResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (methodsToCall == null) throw new ArgumentNullException(nameof(methodsToCall));

            var diagnosticsExist = false;
            results = new CallMethodResultCollection(methodsToCall.Count);
            diagnosticInfos = new DiagnosticInfoCollection(methodsToCall.Count);
            var errors = new List<ServiceResult>(methodsToCall.Count);

            // add placeholder for each result.
            var validItems = false;

            for (var ii = 0; ii < methodsToCall.Count; ii++)
            {
                results.Add(null);
                errors.Add(null);

                if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                {
                    diagnosticInfos.Add(null);
                }

                // validate request parameters.
                errors[ii] = ValidateCallRequestItem(context, methodsToCall[ii]);

                if (ServiceResult.IsBad(errors[ii]))
                {
                    methodsToCall[ii].Processed = true;

                    // add diagnostics if requested.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                        diagnosticsExist = true;
                    }

                    continue;
                }

                // found at least one valid item.
                validItems = true;
                methodsToCall[ii].Processed = false;
            }

            // call each node manager.
            if (validItems)
            {
                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.Call(
                        context,
                        methodsToCall,
                        results,
                        errors);
                }
            }

            for (var ii = 0; ii < methodsToCall.Count; ii++)
            {
                // set an error code for calls that were not handled by any node manager.
                if (!methodsToCall[ii].Processed)
                {
                    results[ii] = new CallMethodResult();
                    errors[ii] = StatusCodes.BadNodeIdUnknown;
                }

                // update the diagnostic info and ensure the status code in the result is the same as the error code.
                if (errors[ii] != null && errors[ii].Code != StatusCodes.Good)
                {
                    if (results[ii] == null)
                    {
                        results[ii] = new CallMethodResult();
                    }

                    results[ii].StatusCode = errors[ii].Code;

                    // add diagnostics if requested.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                        diagnosticsExist = true;
                    }
                }
            }

            // clear the diagnostics array if no diagnostics requested or no errors occurred.
            UpdateDiagnostics(context, diagnosticsExist, ref diagnosticInfos);
        }

        /// <summary>
        /// Handles condition refresh request.
        /// </summary>
        public virtual void ConditionRefresh(UaServerOperationContext context, IList<IUaEventMonitoredItem> monitoredItems)
        {
            foreach (var nodeManager in nodeManagers_)
            {
                try
                {
                    nodeManager.ConditionRefresh(context, monitoredItems);
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error calling ConditionRefresh on NodeManager.");
                }
            }
        }

        /// <summary>
        /// Creates a set of monitored items.
        /// </summary>
        public virtual void CreateMonitoredItems(
            UaServerOperationContext context,
            uint subscriptionId,
            double publishingInterval,
            TimestampsToReturn timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult> errors,
            IList<MonitoringFilterResult> filterResults,
            IList<IUaMonitoredItem> monitoredItems)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (itemsToCreate == null) throw new ArgumentNullException(nameof(itemsToCreate));
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            if (filterResults == null) throw new ArgumentNullException(nameof(filterResults));
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));
            if (publishingInterval < 0) throw new ArgumentOutOfRangeException(nameof(publishingInterval));

            if (timestampsToReturn < TimestampsToReturn.Source || timestampsToReturn > TimestampsToReturn.Neither)
            {
                throw new ServiceResultException(StatusCodes.BadTimestampsToReturnInvalid);
            }

            // add placeholder for each result.
            var validItems = false;

            for (var ii = 0; ii < itemsToCreate.Count; ii++)
            {
                // validate request parameters.
                errors[ii] = ValidateMonitoredItemCreateRequest(context, itemsToCreate[ii]);

                if (ServiceResult.IsBad(errors[ii]))
                {
                    itemsToCreate[ii].Processed = true;
                    continue;
                }

                // found at least one valid item.
                validItems = true;
                itemsToCreate[ii].Processed = false;
            }

            // call each node manager.
            if (validItems)
            {
                // create items for event filters.
                CreateMonitoredItemsForEvents(
                    context,
                    subscriptionId,
                    publishingInterval,
                    timestampsToReturn,
                    itemsToCreate,
                    errors,
                    filterResults,
                    monitoredItems,
                    ref lastMonitoredItemId_);

                // create items for data access.
                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.CreateMonitoredItems(
                        context,
                        subscriptionId,
                        publishingInterval,
                        timestampsToReturn,
                        itemsToCreate,
                        errors,
                        filterResults,
                        monitoredItems,
                        ref lastMonitoredItemId_);
                }

                // fill results for unknown nodes.
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    if (!itemsToCreate[ii].Processed)
                    {
                        errors[ii] = new ServiceResult(StatusCodes.BadNodeIdUnknown);
                    }
                }
            }
        }

        /// <summary>
        /// Create monitored items for event subscriptions.
        /// </summary>
        private void CreateMonitoredItemsForEvents(
            UaServerOperationContext context,
            uint subscriptionId,
            double publishingInterval,
            TimestampsToReturn timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult> errors,
            IList<MonitoringFilterResult> filterResults,
            IList<IUaMonitoredItem> monitoredItems,
            ref long globalIdCounter)
        {
            for (var ii = 0; ii < itemsToCreate.Count; ii++)
            {
                var itemToCreate = itemsToCreate[ii];

                if (!itemToCreate.Processed)
                {
                    // must make sure the filter is not null before checking its type.
                    if (ExtensionObject.IsNull(itemToCreate.RequestedParameters.Filter))
                    {
                        continue;
                    }

                    // all event subscriptions required an event filter.
                    var filter = itemToCreate.RequestedParameters.Filter.Body as EventFilter;

                    if (filter == null)
                    {
                        continue;
                    }

                    itemToCreate.Processed = true;

                    // only the value attribute may be used with an event subscription.
                    if (itemToCreate.ItemToMonitor.AttributeId != Attributes.EventNotifier)
                    {
                        errors[ii] = StatusCodes.BadFilterNotAllowed;
                        continue;
                    }

                    // the index range parameter has no meaning for event subscriptions.
                    if (!String.IsNullOrEmpty(itemToCreate.ItemToMonitor.IndexRange))
                    {
                        errors[ii] = StatusCodes.BadIndexRangeInvalid;
                        continue;
                    }

                    // the data encoding has no meaning for event subscriptions.
                    if (!QualifiedName.IsNull(itemToCreate.ItemToMonitor.DataEncoding))
                    {
                        errors[ii] = StatusCodes.BadDataEncodingInvalid;
                        continue;
                    }

                    // validate the event filter.
                    var result = filter.Validate(new FilterContext(server_.NamespaceUris, server_.TypeTree, context));

                    if (ServiceResult.IsBad(result.Status))
                    {
                        errors[ii] = result.Status;
                        filterResults[ii] = result.ToEventFilterResult(context.DiagnosticsMask, context.StringTable);
                        continue;
                    }

                    // check if a valid node.
                    IUaBaseNodeManager nodeManager = null;

                    var handle = GetManagerHandle(itemToCreate.ItemToMonitor.NodeId, out nodeManager);

                    if (handle == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;
                        continue;
                    }
                    var nodeMetadata = nodeManager.GetNodeMetadata(context, handle, BrowseResultMask.All);

                    errors[ii] = ValidateRolePermissions(context, nodeMetadata, PermissionType.ReceiveEvents);

                    if (ServiceResult.IsBad(errors[ii]))
                    {
                        continue;
                    }

                    // create a globally unique identifier.
                    var monitoredItemId = Utils.IncrementIdentifier(ref globalIdCounter);

                    var monitoredItem = server_.EventManager.CreateMonitoredItem(
                        context,
                        nodeManager,
                        handle,
                        subscriptionId,
                        monitoredItemId,
                        timestampsToReturn,
                        publishingInterval,
                        itemToCreate,
                        filter);

                    // subscribe to all node managers.
                    if (itemToCreate.ItemToMonitor.NodeId == Objects.Server)
                    {
                        foreach (var manager in nodeManagers_)
                        {
                            try
                            {
                                manager.SubscribeToAllEvents(context, subscriptionId, monitoredItem, false);
                            }
                            catch (Exception e)
                            {
                                Utils.LogError(e, "NodeManager threw an exception subscribing to all events. NodeManager={0}", manager);
                            }
                        }
                    }

                    // only subscribe to the node manager that owns the node.
                    else
                    {
                        var error = nodeManager.SubscribeToEvents(context, handle, subscriptionId, monitoredItem, false);

                        if (ServiceResult.IsBad(error))
                        {
                            server_.EventManager.DeleteMonitoredItem(monitoredItem.Id);
                            errors[ii] = error;
                            continue;
                        }
                    }

                    monitoredItems[ii] = monitoredItem;
                    errors[ii] = StatusCodes.Good;
                }
            }
        }

        /// <summary>
        /// Modifies a set of monitored items.
        /// </summary>
        public virtual void ModifyMonitoredItems(
            UaServerOperationContext context,
            TimestampsToReturn timestampsToReturn,
            IList<IUaMonitoredItem> monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult> errors,
            IList<MonitoringFilterResult> filterResults)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (itemsToModify == null) throw new ArgumentNullException(nameof(itemsToModify));
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            if (filterResults == null) throw new ArgumentNullException(nameof(filterResults));

            if (timestampsToReturn < TimestampsToReturn.Source || timestampsToReturn > TimestampsToReturn.Neither)
            {
                throw new ServiceResultException(StatusCodes.BadTimestampsToReturnInvalid);
            }

            var validItems = false;

            for (var ii = 0; ii < itemsToModify.Count; ii++)
            {
                // check for errors.
                if (ServiceResult.IsBad(errors[ii]) || monitoredItems[ii] == null)
                {
                    itemsToModify[ii].Processed = true;
                    continue;
                }

                // validate request parameters.
                errors[ii] = ValidateMonitoredItemModifyRequest(itemsToModify[ii]);

                if (ServiceResult.IsBad(errors[ii]))
                {
                    itemsToModify[ii].Processed = true;
                    continue;
                }

                // found at least one valid item.
                validItems = true;
                itemsToModify[ii].Processed = false;
            }

            // call each node manager.
            if (validItems)
            {
                // modify items for event filters.
                ModifyMonitoredItemsForEvents(
                    context,
                    timestampsToReturn,
                    monitoredItems,
                    itemsToModify,
                    errors,
                    filterResults);

                // let each node manager figure out which items it owns.
                foreach (var nodeManager in nodeManagers_)
                {
                    nodeManager.ModifyMonitoredItems(
                        context,
                        timestampsToReturn,
                        monitoredItems,
                        itemsToModify,
                        errors,
                        filterResults);
                }

                // update results.
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    if (!itemsToModify[ii].Processed)
                    {
                        errors[ii] = new ServiceResult(StatusCodes.BadMonitoredItemIdInvalid);
                    }
                }
            }
        }

        /// <summary>
        /// Modify monitored items for event subscriptions.
        /// </summary>
        private void ModifyMonitoredItemsForEvents(
            UaServerOperationContext context,
            TimestampsToReturn timestampsToReturn,
            IList<IUaMonitoredItem> monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult> errors,
            IList<MonitoringFilterResult> filterResults)
        {
            for (var ii = 0; ii < itemsToModify.Count; ii++)
            {
                var monitoredItem = monitoredItems[ii] as IUaEventMonitoredItem;

                // all event subscriptions are handled by the event manager.
                if (monitoredItem == null || (monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.Events) == 0)
                {
                    continue;
                }

                var itemToModify = itemsToModify[ii];
                itemToModify.Processed = true;

                // check for a valid filter.
                if (ExtensionObject.IsNull(itemToModify.RequestedParameters.Filter))
                {
                    errors[ii] = StatusCodes.BadEventFilterInvalid;
                    continue;
                }

                // all event subscriptions required an event filter.
                var filter = itemToModify.RequestedParameters.Filter.Body as EventFilter;

                if (filter == null)
                {
                    errors[ii] = StatusCodes.BadEventFilterInvalid;
                    continue;
                }

                // validate the event filter.
                var result = filter.Validate(new FilterContext(server_.NamespaceUris, server_.TypeTree, context));

                if (ServiceResult.IsBad(result.Status))
                {
                    errors[ii] = result.Status;
                    filterResults[ii] = result.ToEventFilterResult(context.DiagnosticsMask, context.StringTable);
                    continue;
                }

                // modify the item.
                server_.EventManager.ModifyMonitoredItem(
                    context,
                    monitoredItem,
                    timestampsToReturn,
                    itemToModify,
                    filter);

                // subscribe to all node managers.
                if ((monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.AllEvents) != 0)
                {
                    foreach (var manager in nodeManagers_)
                    {
                        manager.SubscribeToAllEvents(
                            context,
                            monitoredItem.SubscriptionId,
                            monitoredItem,
                            false);
                    }
                }

                // only subscribe to the node manager that owns the node.
                else
                {
                    monitoredItem.NodeManager.SubscribeToEvents(
                        context,
                        monitoredItem.ManagerHandle,
                        monitoredItem.SubscriptionId,
                        monitoredItem,
                        false);
                }

                errors[ii] = StatusCodes.Good;
            }
        }

        /// <summary>
        /// Transfers a set of monitored items.
        /// </summary>
        public virtual void TransferMonitoredItems(
            UaServerOperationContext context,
            bool sendInitialValues,
            IList<IUaMonitoredItem> monitoredItems,
            IList<ServiceResult> errors)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            var processedItems = new List<bool>(monitoredItems.Count);

            // preset results for unknown nodes
            for (int ii = 0; ii < monitoredItems.Count; ii++)
            {
                processedItems.Add(monitoredItems[ii] == null);
                errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
            }

            // call each node manager.
            foreach (IUaBaseNodeManager nodeManager in nodeManagers_)
            {
                nodeManager.TransferMonitoredItems(
                    context,
                    sendInitialValues,
                    monitoredItems,
                    processedItems,
                    errors);
            }
        }

        /// <summary>
        /// Deletes a set of monitored items.
        /// </summary>
        public virtual void DeleteMonitoredItems(
            UaServerOperationContext context,
            uint subscriptionId,
            IList<IUaMonitoredItem> itemsToDelete,
            IList<ServiceResult> errors)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (itemsToDelete == null) throw new ArgumentNullException(nameof(itemsToDelete));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            var processedItems = new List<bool>(itemsToDelete.Count);

            for (var ii = 0; ii < itemsToDelete.Count; ii++)
            {
                processedItems.Add(ServiceResult.IsBad(errors[ii]) || itemsToDelete[ii] == null);
            }

            // delete items for event filters.
            DeleteMonitoredItemsForEvents(
                context,
                subscriptionId,
                itemsToDelete,
                processedItems,
                errors);

            // call each node manager.
            foreach (var nodeManager in nodeManagers_)
            {
                nodeManager.DeleteMonitoredItems(
                    context,
                    itemsToDelete,
                    processedItems,
                    errors);
            }

            // fill results for unknown nodes.
            for (var ii = 0; ii < errors.Count; ii++)
            {
                if (!processedItems[ii])
                {
                    errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                }
            }
        }

        /// <summary>
        /// Delete monitored items for event subscriptions.
        /// </summary>
        private void DeleteMonitoredItemsForEvents(
            UaServerOperationContext context,
            uint subscriptionId,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors)
        {
            for (var ii = 0; ii < monitoredItems.Count; ii++)
            {
                var monitoredItem = monitoredItems[ii] as IUaEventMonitoredItem;

                // all event subscriptions are handled by the event manager.
                if (monitoredItem == null || (monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.Events) == 0)
                {
                    continue;
                }

                processedItems[ii] = true;

                // unsubscribe to all node managers.
                if ((monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.AllEvents) != 0)
                {
                    foreach (var manager in nodeManagers_)
                    {
                        manager.SubscribeToAllEvents(context, subscriptionId, monitoredItem, true);
                    }
                }

                // only unsubscribe to the node manager that owns the node.
                else
                {
                    monitoredItem.NodeManager.SubscribeToEvents(context, monitoredItem.ManagerHandle, subscriptionId, monitoredItem, true);
                }

                // delete the item.
                server_.EventManager.DeleteMonitoredItem(monitoredItem.Id);

                // success.
                errors[ii] = StatusCodes.Good;
            }
        }

        /// <summary>
        /// Changes the monitoring mode for a set of items.
        /// </summary>
        public virtual void SetMonitoringMode(
            UaServerOperationContext context,
            MonitoringMode monitoringMode,
            IList<IUaMonitoredItem> itemsToModify,
            IList<ServiceResult> errors)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (itemsToModify == null) throw new ArgumentNullException(nameof(itemsToModify));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            // call each node manager.
            var processedItems = new List<bool>(itemsToModify.Count);

            for (var ii = 0; ii < itemsToModify.Count; ii++)
            {
                processedItems.Add(ServiceResult.IsBad(errors[ii]) || itemsToModify[ii] == null);
            }

            // delete items for event filters.
            SetMonitoringModeForEvents(
                context,
                monitoringMode,
                itemsToModify,
                processedItems,
                errors);

            foreach (var nodeManager in nodeManagers_)
            {
                nodeManager.SetMonitoringMode(
                    context,
                    monitoringMode,
                    itemsToModify,
                    processedItems,
                    errors);
            }

            // fill results for unknown nodes.
            for (var ii = 0; ii < errors.Count; ii++)
            {
                if (!processedItems[ii])
                {
                    errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                }
            }
        }

        /// <summary>
        /// Delete monitored items for event subscriptions.
        /// </summary>
        private static void SetMonitoringModeForEvents(
            UaServerOperationContext context,
            MonitoringMode monitoringMode,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors)
        {
            for (var ii = 0; ii < monitoredItems.Count; ii++)
            {
                var monitoredItem = monitoredItems[ii] as IUaEventMonitoredItem;

                // all event subscriptions are handled by the event manager.
                if (monitoredItem == null || (monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.Events) == 0)
                {
                    continue;
                }

                processedItems[ii] = true;

                // set the monitoring mode.
                monitoredItem.SetMonitoringMode(monitoringMode);

                // success.
                errors[ii] = StatusCodes.Good;
            }
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// The server that the node manager belongs to.
        /// </summary>
        protected IUaServerData Server
        {
            get { return server_; }
        }

        /// <summary>
        /// The node managers being managed.
        /// </summary>
        public IList<IUaBaseNodeManager> NodeManagers
        {
            get { return nodeManagers_; }
        }

        /// <summary>
        /// The namespace managers being managed
        /// </summary>
        public IUaBaseNodeManager[][] NamespaceManagers
        {
            get
            {
                return namespaceManagers_;
            }
        }

        /// <summary>
        /// Validates a monitoring attributes parameter.
        /// </summary>
        protected static ServiceResult ValidateMonitoringAttributes(MonitoringParameters attributes)
        {
            // check for null structure.
            if (attributes == null)
            {
                return new ServiceResult(StatusCodes.BadStructureMissing);
            }

            // check for known filter.
            if (!ExtensionObject.IsNull(attributes.Filter))
            {
                var filter = attributes.Filter.Body as MonitoringFilter;

                if (filter == null)
                {
                    return new ServiceResult(StatusCodes.BadMonitoredItemFilterInvalid);
                }
            }

            // passed basic validation.
            return null;
        }

        /// <summary>
        /// Validates a monitoring filter.
        /// </summary>
        protected static ServiceResult ValidateMonitoringFilter(ExtensionObject filter)
        {
            ServiceResult error = null;

            // check that no filter is specified for non-value attributes.
            if (!ExtensionObject.IsNull(filter))
            {
                var datachangeFilter = filter.Body as DataChangeFilter;

                // validate data change filter.
                if (datachangeFilter != null)
                {
                    error = datachangeFilter.Validate();

                    if (ServiceResult.IsBad(error))
                    {
                        return error;
                    }
                }
            }

            // passed basic validation.
            return null;
        }

        /// <summary>
        /// Validates a monitored item create request parameter.
        /// </summary>
        protected ServiceResult ValidateMonitoredItemCreateRequest(UaServerOperationContext operationContext, MonitoredItemCreateRequest item)
        {
            // check for null structure.
            if (item == null)
            {
                return new ServiceResult(StatusCodes.BadStructureMissing);
            }

            // validate read value id component. Validate also access rights and permissions
            ServiceResult error = ValidateReadRequest(operationContext, item.ItemToMonitor, null, true);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // check for valid monitoring mode.
            if ((int)item.MonitoringMode < 0 || (int)item.MonitoringMode > (int)MonitoringMode.Reporting)
            {
                return new ServiceResult(StatusCodes.BadMonitoringModeInvalid);
            }

            // check for null structure.
            var attributes = item.RequestedParameters;

            error = ValidateMonitoringAttributes(attributes);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // check that no filter is specified for non-value attributes.
            if (item.ItemToMonitor.AttributeId != Attributes.Value && item.ItemToMonitor.AttributeId != Attributes.EventNotifier)
            {
                if (!ExtensionObject.IsNull(attributes.Filter))
                {
                    return new ServiceResult(StatusCodes.BadFilterNotAllowed);
                }
            }
            else
            {
                error = ValidateMonitoringFilter(attributes.Filter);

                if (ServiceResult.IsBad(error))
                {
                    return error;
                }
            }

            // passed basic validation.
            return null;
        }

        /// <summary>
        /// Validates a monitored item modify request parameter.
        /// </summary>
        protected static ServiceResult ValidateMonitoredItemModifyRequest(MonitoredItemModifyRequest item)
        {
            // check for null structure.
            if (item == null)
            {
                return new ServiceResult(StatusCodes.BadStructureMissing);
            }

            // check for null structure.
            var attributes = item.RequestedParameters;

            var error = ValidateMonitoringAttributes(attributes);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // validate monitoring filter.         
            error = ValidateMonitoringFilter(attributes.Filter);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // passed basic validation.
            return null;
        }

        /// <summary>
        /// Validates a call request item parameter. It validates also access rights and role permissions
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="callMethodRequest"></param>
        /// <returns></returns>
        protected ServiceResult ValidateCallRequestItem(UaServerOperationContext operationContext, CallMethodRequest callMethodRequest)
        {
            // check for null structure.
            if (callMethodRequest == null)
            {
                return StatusCodes.BadStructureMissing;
            }

            // check object id.
            if (NodeId.IsNull(callMethodRequest.ObjectId))
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            // check method id.
            if (NodeId.IsNull(callMethodRequest.MethodId))
            {
                return StatusCodes.BadMethodInvalid;
            }

            // check input arguments
            if (callMethodRequest.InputArguments == null)
            {
                return StatusCodes.BadStructureMissing;
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Validates a Read or MonitoredItemCreate request. It validates also access rights and role permissions
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="readValueId"></param>
        /// <param name="uniqueNodesReadAttributes"></param>
        /// <param name="permissionsOnly"></param>
        /// <returns></returns>
        protected ServiceResult ValidateReadRequest(UaServerOperationContext operationContext,
            ReadValueId readValueId,
            Dictionary<NodeId, List<object>> uniqueNodesReadAttributes = null,
            bool permissionsOnly = false
            )
        {
            var serviceResult = ReadValueId.Validate(readValueId);

            if (ServiceResult.IsGood(serviceResult))
            {
                //any attribute other than Value or RolePermissions
                var requestedPermission = PermissionType.Browse;
                if (readValueId.AttributeId == Attributes.RolePermissions)
                {
                    requestedPermission = PermissionType.ReadRolePermissions;
                }
                else if (readValueId.AttributeId == Attributes.Value)
                {
                    requestedPermission = PermissionType.Read;
                }

                // check access rights and role permissions
                serviceResult = ValidatePermissions(operationContext,
                    readValueId.NodeId,
                    requestedPermission,
                    uniqueNodesReadAttributes,
                    permissionsOnly);
            }
            return serviceResult;
        }

        /// <summary>
        /// Validates a Write request. It validates also access rights and role permissions
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="writeValue"></param>
        /// <returns></returns>
        protected ServiceResult ValidateWriteRequest(UaServerOperationContext operationContext, WriteValue writeValue)
        {
            var serviceResult = WriteValue.Validate(writeValue);

            if (ServiceResult.IsGood(serviceResult))
            {
                var requestedPermission = PermissionType.WriteAttribute;  //any attribute other than Value, RolePermissions or Historizing
                if (writeValue.AttributeId == Attributes.RolePermissions)
                {
                    requestedPermission = PermissionType.WriteRolePermissions;
                }
                else if (writeValue.AttributeId == Attributes.Historizing)
                {
                    requestedPermission = PermissionType.WriteHistorizing;
                }
                else if (writeValue.AttributeId == Attributes.Value)
                {
                    requestedPermission = PermissionType.Write;
                }

                // check access rights and permissions
                serviceResult = ValidatePermissions(operationContext, writeValue.NodeId, requestedPermission, null, true);
            }
            return serviceResult;
        }

        /// <summary>
        /// Validates a HistoryRead request. It validates also access rights and role permissions
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="historyReadValueId"></param>
        /// <returns></returns>
        protected ServiceResult ValidateHistoryReadRequest(UaServerOperationContext operationContext, HistoryReadValueId historyReadValueId)
        {
            var serviceResult = HistoryReadValueId.Validate(historyReadValueId);

            if (ServiceResult.IsGood(serviceResult))
            {
                // check access rights and permissions
                serviceResult = ValidatePermissions(operationContext, historyReadValueId.NodeId, PermissionType.ReadHistory, null, true);
            }
            return serviceResult;
        }

        /// <summary>
        ///  Validates a HistoryUpdate request. It validates also access rights and role permissions
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="historyUpdateDetails"></param>
        /// <returns></returns>
        protected ServiceResult ValidateHistoryUpdateRequest(UaServerOperationContext operationContext, HistoryUpdateDetails historyUpdateDetails)
        {
            var serviceResult = HistoryUpdateDetails.Validate(historyUpdateDetails);

            if (ServiceResult.IsGood(serviceResult))
            {
                // check access rights and permissions
                var requiredPermission = DetermineHistoryAccessPermission(historyUpdateDetails);
                serviceResult = ValidatePermissions(operationContext, historyUpdateDetails.NodeId, requiredPermission, null, true);
            }

            return serviceResult;
        }
        #region Validate Permissions Methods

        /// <summary>
        /// Check if the Base NodeClass attributes and NameSpace meta-data attributes 
        /// are valid for the given operation context of the specified node.
        /// </summary>
        /// <param name="context">The Operation Context</param>
        /// <param name="nodeId">The node whose attributes are validated</param>
        /// <param name="requestedPermision">The requested permission</param>
        /// <param name="uniqueNodesServiceAttributes">The cache holding the values of the attributes neeeded to be used in subsequent calls</param>
        /// <param name="permissionsOnly">Only the AccessRestrictions and RolePermission attributes are read. Should be false if uniqueNodesServiceAttributes is not null</param>
        /// <returns>StatusCode Good if permission is granted, BadUserAccessDenied if not granted 
        /// or a bad status code describing the validation process failure </returns>
        protected ServiceResult ValidatePermissions(
            UaServerOperationContext context,
            NodeId nodeId,
            PermissionType requestedPermision,
            Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes = null,
            bool permissionsOnly = false
           )
        {
            if (context.Session != null)
            {
                IUaBaseNodeManager nodeManager = null;
                var nodeHandle = GetManagerHandle(nodeId, out nodeManager);

                return ValidatePermissions(context, nodeManager, nodeHandle, requestedPermision, uniqueNodesServiceAttributes, permissionsOnly);
            }
            return StatusCodes.Good;
        }

        /// <summary>
        /// Check if the Base NodeClass attributes and NameSpace meta-data attributes 
        /// are valid for the given operation context of the specified node.
        /// </summary>
        /// <param name="context">The Operation Context</param>
        /// <param name="nodeManager">The node manager handling the nodeHandle</param>
        /// <param name="nodeHandle">The node handle of the node whose attributes are validated</param>
        /// <param name="requestedPermision">The requested permission</param>
        /// <param name="uniqueNodesServiceAttributes">The cache holding the values of the attributes neeeded to be used in subsequent calls</param>
        /// <param name="permissionsOnly">Only the AccessRestrictions and RolePermission attributes are read. Should be false if uniqueNodesServiceAttributes is not null</param>
        /// <returns>StatusCode Good if permission is granted, BadUserAccessDenied if not granted 
        /// or a bad status code describing the validation process failure </returns>
        protected ServiceResult ValidatePermissions(
            UaServerOperationContext context,
            IUaBaseNodeManager nodeManager,
            object nodeHandle,
            PermissionType requestedPermision,
            Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes = null,
            bool permissionsOnly = false
            )
        {
            ServiceResult serviceResult = StatusCodes.Good;

            // check if validation is necessary
            if (context.Session != null && nodeManager != null && nodeHandle != null)
            {
                IUaNodeManager nodeManager2 = nodeManager as IUaNodeManager;

                UaNodeMetadata nodeMetadata = null;
                // If it happens that nodemanager does not fully implement IUaBaseNodeManager.GetPermissionMetadata or not IUaNodeManager,
                // fallback to INodeManager.GetNodeMetadata
                if (nodeManager2 != null)
                {
                    nodeMetadata = nodeManager2.GetPermissionMetadata(context, nodeHandle, BrowseResultMask.NodeClass, uniqueNodesServiceAttributes, permissionsOnly);
                }
                // If not IUaNodeManager or GetPermissionMetadata() returns null.
                if (nodeMetadata == null)
                {
                    nodeMetadata = nodeManager.GetNodeMetadata(context, nodeHandle, BrowseResultMask.NodeClass);
                }

                if (nodeMetadata != null)
                {
                    // check RolePermissions 
                    serviceResult = ValidateRolePermissions(context, nodeMetadata, requestedPermision);

                    if (ServiceResult.IsGood(serviceResult))
                    {
                        // check AccessRestrictions
                        serviceResult = ValidateAccessRestrictions(context, nodeMetadata);
                    }
                }
            }

            return serviceResult;
        }

        /// <summary>
        /// Validate the AccessRestrictions attribute
        /// </summary>
        /// <param name="context">The Operation Context</param>
        /// <param name="nodeMetadata"></param>
        /// <returns>Good if the AccessRestrictions passes the validation</returns>
        protected static ServiceResult ValidateAccessRestrictions(UaServerOperationContext context, UaNodeMetadata nodeMetadata)
        {
            ServiceResult serviceResult = StatusCodes.Good;
            var restrictions = AccessRestrictionType.None;

            if (nodeMetadata.AccessRestrictions != AccessRestrictionType.None)
            {
                restrictions = nodeMetadata.AccessRestrictions;
            }
            else if (nodeMetadata.DefaultAccessRestrictions != AccessRestrictionType.None)
            {
                restrictions = nodeMetadata.DefaultAccessRestrictions;
            }
            if (restrictions != AccessRestrictionType.None)
            {
                var encryptionRequired = (restrictions & AccessRestrictionType.EncryptionRequired) == AccessRestrictionType.EncryptionRequired;
                var signingRequired = (restrictions & AccessRestrictionType.SigningRequired) == AccessRestrictionType.SigningRequired;
                var sessionRequired = (restrictions & AccessRestrictionType.SessionRequired) == AccessRestrictionType.SessionRequired;
                var applyRestrictionsToBrowse = (restrictions & AccessRestrictionType.ApplyRestrictionsToBrowse) == AccessRestrictionType.ApplyRestrictionsToBrowse;

                var browseOperation = context.RequestType == Sessions.RequestType.Browse ||
                                       context.RequestType == Sessions.RequestType.BrowseNext ||
                                       context.RequestType == Sessions.RequestType.TranslateBrowsePathsToNodeIds;

                if ((encryptionRequired &&
                     context.ChannelContext.EndpointDescription.SecurityMode != MessageSecurityMode.SignAndEncrypt &&
                     context.ChannelContext.EndpointDescription.TransportProfileUri != Profiles.HttpsBinaryTransport &&
                     ((applyRestrictionsToBrowse && browseOperation) || !browseOperation)) ||
                    (signingRequired &&
                     context.ChannelContext.EndpointDescription.SecurityMode != MessageSecurityMode.Sign &&
                     context.ChannelContext.EndpointDescription.SecurityMode != MessageSecurityMode.SignAndEncrypt &&
                     context.ChannelContext.EndpointDescription.TransportProfileUri != Profiles.HttpsBinaryTransport &&
                     ((applyRestrictionsToBrowse && browseOperation) || !browseOperation)) ||
                   (sessionRequired && context.Session == null))
                {
                    serviceResult = ServiceResult.Create(StatusCodes.BadSecurityModeInsufficient,
                        "Access restricted to nodeId {0} due to insufficient security mode.", nodeMetadata.NodeId);
                }
            }

            return serviceResult;
        }

        /// <summary>
        /// Validates the role permissions 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nodeMetadata"></param>
        /// <param name="requestedPermission"></param>
        /// <returns></returns>
        public static ServiceResult ValidateRolePermissions(UaServerOperationContext context, UaNodeMetadata nodeMetadata, PermissionType requestedPermission)
        {
            if (context.Session == null || nodeMetadata == null || requestedPermission == PermissionType.None)
            {
                // no permission is required hence the validation passes
                return StatusCodes.Good;
            }

            // get the intersection of user role permissions and role permissions
            RolePermissionTypeCollection userRolePermissions = null, rolePermissions = null;
            if (nodeMetadata.UserRolePermissions != null && nodeMetadata.UserRolePermissions.Count > 0)
            {
                userRolePermissions = nodeMetadata.UserRolePermissions;
            }
            else if (nodeMetadata.DefaultUserRolePermissions != null && nodeMetadata.DefaultUserRolePermissions.Count > 0)
            {
                userRolePermissions = nodeMetadata.DefaultUserRolePermissions;
            }

            if (nodeMetadata.RolePermissions != null && nodeMetadata.RolePermissions.Count > 0)
            {
                rolePermissions = nodeMetadata.RolePermissions;
            }
            else
            {
                rolePermissions = nodeMetadata.DefaultRolePermissions;
            }

            if ((userRolePermissions == null || userRolePermissions.Count == 0) && (rolePermissions == null || rolePermissions.Count == 0))
            {
                // there is no restriction from role permissions
                return StatusCodes.Good;
            }

            // group all permissions defined in rolePermissions by RoleId 
            var roleIdPermissions = new Dictionary<NodeId, PermissionType>();
            if (rolePermissions != null && rolePermissions.Count > 0)
            {
                foreach (var rolePermission in rolePermissions)
                {
                    if (roleIdPermissions.ContainsKey(rolePermission.RoleId))
                    {
                        roleIdPermissions[rolePermission.RoleId] |= ((PermissionType)rolePermission.Permissions);
                    }
                    else
                    {
                        roleIdPermissions[rolePermission.RoleId] = ((PermissionType)rolePermission.Permissions) & requestedPermission;
                    }
                }
            }

            // group all permissions defined in userRolePermissions by RoleId 
            var roleIdPermissionsDefinedForUser = new Dictionary<NodeId, PermissionType>();
            if (userRolePermissions != null && userRolePermissions.Count > 0)
            {
                foreach (var rolePermission in userRolePermissions)
                {
                    if (roleIdPermissionsDefinedForUser.ContainsKey(rolePermission.RoleId))
                    {
                        roleIdPermissionsDefinedForUser[rolePermission.RoleId] |= ((PermissionType)rolePermission.Permissions);
                    }
                    else
                    {
                        roleIdPermissionsDefinedForUser[rolePermission.RoleId] = ((PermissionType)rolePermission.Permissions) & requestedPermission;
                    }
                }
            }

            Dictionary<NodeId, PermissionType> commonRoleIdPermissions = null;
            if (rolePermissions == null || rolePermissions.Count == 0)
            {
                // there were no role permissions defined for this node only user role permissions
                commonRoleIdPermissions = roleIdPermissionsDefinedForUser;
            }
            else if (userRolePermissions == null || userRolePermissions.Count == 0)
            {
                // there were no role permissions defined for this node only user role permissions
                commonRoleIdPermissions = roleIdPermissions;
            }
            else
            {
                commonRoleIdPermissions = new Dictionary<NodeId, PermissionType>();
                // intersect role permissions from node and user
                foreach (var roleId in roleIdPermissions.Keys)
                {
                    if (roleIdPermissionsDefinedForUser.ContainsKey(roleId))
                    {
                        commonRoleIdPermissions[roleId] = roleIdPermissions[roleId] & roleIdPermissionsDefinedForUser[roleId];
                    }
                }
            }

            var currentRoleIds = context.Session.Identity.GrantedRoleIds;
            if (currentRoleIds == null || currentRoleIds.Count == 0)
            {
                return ServiceResult.Create(StatusCodes.BadUserAccessDenied, "Current user has no granted role.");
            }

            foreach (var currentRoleId in currentRoleIds)
            {
                if (commonRoleIdPermissions.ContainsKey(currentRoleId) && commonRoleIdPermissions[currentRoleId] != PermissionType.None)
                {
                    // there is one role that current session has na is listed in requested role
                    return StatusCodes.Good;
                }
            }
            return ServiceResult.Create(StatusCodes.BadUserAccessDenied,
                "The requested permission {0} is not granted for node id {1}.", requestedPermission, nodeMetadata.NodeId);
        }

        #endregion
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private List<IUaBaseNodeManager> nodeManagers_;
        private long lastMonitoredItemId_;
        private IUaBaseNodeManager[][] namespaceManagers_;
        private uint maxContinuationPointsPerBrowse_;
        private ReaderWriterLockSlim readWriterLockSlim_ = new ReaderWriterLockSlim();
        #endregion
    }
}
