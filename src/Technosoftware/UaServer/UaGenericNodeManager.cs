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

using Opc.Ua;

using Technosoftware.UaServer.Aggregates;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// A base implementation of the IUaNodeManager interface.
    /// </summary>
    /// <remarks>
    /// This node manager is a base class used in multiple samples. It implements the IUaNodeManager
    /// interface and allows sub-classes to override only the methods that they need.
    /// </remarks>
    public class UaGenericNodeManager : IUaNodeManager, INodeIdFactory, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        /// <param name="uaServerData">The uaServerData data implementing the IUaServerData interface.</param>
        /// <param name="namespaceUris">Array of namespaces that are used by the application.</param>
        protected UaGenericNodeManager(
            IUaServerData uaServerData,
            params string[] namespaceUris)
        :
            this(uaServerData, (ApplicationConfiguration)null, namespaceUris)
        {
        }

        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        /// <param name="uaServerData">The uaServerData data implementing the IUaServerData interface.</param>
        /// <param name="configuration">The used application configuration.</param>
        /// <param name="namespaceUris">Array of namespaces that are used by the application.</param>
        protected UaGenericNodeManager(
            IUaServerData uaServerData,
            ApplicationConfiguration configuration,
            params string[] namespaceUris)
        {
            // set defaults.
            MaxQueueSize = 1000;

            if (configuration?.ServerConfiguration != null)
            {
                MaxQueueSize = (uint)configuration.ServerConfiguration.MaxNotificationQueueSize;
            }

            // save a reference to the UA server instance that owns the node manager.
            ServerData = uaServerData;

            // all operations require information about the system
            SystemContext = ServerData.DefaultSystemContext.Copy();

            // the node id factory assigns new node ids to new nodes.
            // the strategy used by a NodeManager depends on what kind of information it provides.
            SystemContext.NodeIdFactory = this;

            // create the table of namespaces that are used by the NodeManager.
            namespaceUris_ = namespaceUris;

            // add the uris to the server's namespace table and cache the indexes.
            if (namespaceUris != null)
            {
                NamespaceIndexes = new ushort[namespaceUris_.Length];

                for (var ii = 0; ii < namespaceUris_.Length; ii++)
                {
                    NamespaceIndexes[ii] = ServerData.NamespaceUris.GetIndexOrAppend(namespaceUris_[ii]);
                }
            }

            // create the table of monitored items.
            // these are items created by clients when they subscribe to data or events.
            MonitoredItems = new Dictionary<uint, IUaDataChangeMonitoredItem>();

            // create the table of monitored nodes.
            // these are created by the node manager whenever a client subscribe to an attribute of the node.
            MonitoredNodes = new Dictionary<NodeId, UaMonitoredNode>();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (Lock)
                {
                    if (PredefinedNodes != null)
                    {
                        foreach (var node in PredefinedNodes.Values)
                        {
                            Utils.SilentDispose(node);
                        }

                        PredefinedNodes.Clear();
                    }
                }
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns>The new NodeId.</returns>
        public virtual NodeId Create(ISystemContext context, NodeState node)
        {
            return node.NodeId;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Acquires the lock on the node manager.
        /// </summary>
        public object Lock { get; } = new object();

        /// <summary>
        /// Gets the server that the node manager belongs to.
        /// </summary>
        public IUaServerData ServerData { get; private set; }

        /// <summary>
        /// The default context to use.
        /// </summary>
        public UaServerContext SystemContext { get; private set; }

        /// <summary>
        /// Gets the default index for the node manager's namespace.
        /// </summary>
        public ushort NamespaceIndex => NamespaceIndexes[0];

        /// <summary>
        /// Gets the namespace indexes owned by the node manager.
        /// </summary>
        /// <value>The namespace indexes.</value>
        public ushort[] NamespaceIndexes { get; private set; }

        /// <summary>
        /// Gets or sets the maximum size of a monitored item queue.
        /// </summary>
        /// <value>The maximum size of a monitored item queue.</value>
        public uint MaxQueueSize { get; set; }

        /// <summary>
        /// The root for the alias assigned to the node manager.
        /// </summary>
        public string AliasRoot { get; set; }
        #endregion

        #region Protected Members
        /// <summary>
        /// The predefined nodes managed by the node manager.
        /// </summary>
        protected NodeIdDictionary<NodeState> PredefinedNodes { get; private set; }

        /// <summary>
        /// The root notifiers for the node manager.
        /// </summary>
        protected List<NodeState> RootNotifiers { get; private set; }

        /// <summary>
        /// Gets the table of monitored items.
        /// </summary>
        protected Dictionary<uint, IUaDataChangeMonitoredItem> MonitoredItems { get; private set; }

        /// <summary>
        /// Gets the table of nodes being monitored.
        /// </summary>
        protected Dictionary<NodeId, UaMonitoredNode> MonitoredNodes { get; private set; }

        /// <summary>
        /// Sets the namespaces supported by the NodeManager.
        /// </summary>
        /// <param name="namespaceUris">The namespace uris.</param>
        protected void SetNamespaces(params string[] namespaceUris)
        {
            // create the table of namespaces that are used by the NodeManager.
            namespaceUris_ = namespaceUris;

            // add the uris to the server's namespace table and cache the indexes.
            NamespaceIndexes = new ushort[namespaceUris_.Length];

            for (var ii = 0; ii < namespaceUris_.Length; ii++)
            {
                NamespaceIndexes[ii] = ServerData.NamespaceUris.GetIndexOrAppend(namespaceUris_[ii]);
            }
        }

        /// <summary>
        /// Sets the namespace indexes supported by the NodeManager.
        /// </summary>
        protected void SetNamespaceIndexes(ushort[] namespaceIndexes)
        {
            NamespaceIndexes = namespaceIndexes;
            namespaceUris_ = new string[namespaceIndexes.Length];

            for (var ii = 0; ii < namespaceIndexes.Length; ii++)
            {
                namespaceUris_[ii] = ServerData.NamespaceUris.GetString(namespaceIndexes[ii]);
            }
        }

        /// <summary>
        /// Returns true if the namespace for the node id is one of the namespaces managed by the node manager.
        /// </summary>
        /// <param name="nodeId">The node id to check.</param>
        /// <returns>True if the namespace is one of the nodes.</returns>
        protected virtual bool IsNodeIdInNamespace(NodeId nodeId)
        {
            // nulls are never a valid node.
            if (NodeId.IsNull(nodeId))
            {
                return false;
            }

            // quickly exclude nodes that not in the namespace.
            foreach (var namespaceIndex in NamespaceIndexes)
            {
                if (nodeId.NamespaceIndex == namespaceIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the node if the handle refers to a node managed by this manager.
        /// </summary>
        /// <param name="managerHandle">The handle to check.</param>
        /// <returns>Non-null if the handle belongs to the node manager.</returns>
        protected virtual UaNodeHandle IsHandleInNamespace(object managerHandle)
        {
            var source = managerHandle as UaNodeHandle;

            if (source == null)
            {
                return null;
            }

            if (!IsNodeIdInNamespace(source.NodeId))
            {
                return null;
            }

            return source;
        }

        /// <summary>
        /// Returns the state object for the specified node if it exists.
        /// </summary>
        public NodeState Find(NodeId nodeId)
        {
            lock (Lock)
            {
                if (PredefinedNodes == null)
                {
                    return null;
                }

                if (!PredefinedNodes.TryGetValue(nodeId, out var node))
                {
                    return null;
                }

                return node;
            }
        }

        /// <summary>
        /// Creates a new instance and assigns unique identifiers to all children.
        /// </summary>
        /// <param name="context">The operation context.</param>
        /// <param name="parentId">An optional parent identifier.</param>
        /// <param name="referenceTypeId">The reference type from the parent.</param>
        /// <param name="browseName">The browse name.</param>
        /// <param name="instance">The instance to create.</param>
        /// <returns>The new node id.</returns>
        public NodeId CreateNode(
            UaServerContext context,
            NodeId parentId,
            NodeId referenceTypeId,
            QualifiedName browseName,
            BaseInstanceState instance)
        {
            var contextToUse = SystemContext.Copy(context);

            lock (Lock)
            {
                if (PredefinedNodes == null)
                {
                    PredefinedNodes = new NodeIdDictionary<NodeState>();
                }

                instance.ReferenceTypeId = referenceTypeId;

                if (parentId != null)
                {
                    if (!PredefinedNodes.TryGetValue(parentId, out var parent))
                    {
                        throw ServiceResultException.Create(
                            StatusCodes.BadNodeIdUnknown,
                            "Cannot find parent with id: {0}",
                            parentId);
                    }

                    parent.AddChild(instance);
                }

                instance.Create(contextToUse, null, browseName, null, true);
                AddPredefinedNode(contextToUse, instance);

                return instance.NodeId;
            }
        }

        /// <summary>
        /// Deletes a node and all of its children.
        /// </summary>
        public bool DeleteNode(
            UaServerContext context,
            NodeId nodeId)
        {
            var contextToUse = SystemContext.Copy(context);

            var found = false;
            var referencesToRemove = new List<LocalReference>();

            lock (Lock)
            {
                if (PredefinedNodes == null)
                {
                    return false;
                }

                if (PredefinedNodes.TryGetValue(nodeId, out var node))
                {
                    RemovePredefinedNode(contextToUse, node, referencesToRemove);
                    found = true;
                }

                RemoveRootNotifier(node);
            }

            // must release the lock before removing cross references to other node managers.
            if (referencesToRemove.Count > 0)
            {
                ServerData.NodeManager.RemoveReferences(referencesToRemove);
            }

            return found;
        }

        /// <summary>
        /// Searches the node id in all node managers
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public NodeState FindNodeInAddressSpace(NodeId nodeId)
        {
            if (nodeId == null)
            {
                return null;
            }
            // search node id in all node managers
            foreach (IUaBaseNodeManager nodeManager in ServerData.NodeManager.NodeManagers)
            {
                var handle = nodeManager.GetManagerHandle(nodeId) as UaNodeHandle;
                if (handle == null)
                {
                    continue;
                }
                return handle.Node;
            }
            return null;
        }
        #endregion

        #region IUaNodeManager Members
        /// <summary>
        /// Returns the namespaces used by the node manager.
        /// </summary>
        /// <remarks>
        /// All NodeIds exposed by the node manager must be qualified by a namespace URI. This property
        /// returns the URIs used by the node manager. In this example all NodeIds use a single URI.
        /// </remarks>
        public virtual IEnumerable<string> NamespaceUris
        {
            get => namespaceUris_;

            protected set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var namespaceUris = new List<string>(value);
                SetNamespaces(namespaceUris.ToArray());
            }
        }

        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <param name="externalReferences">
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.
        /// </param>
        public virtual void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            LoadPredefinedNodes(SystemContext, externalReferences);
        }

        #region CreateAddressSpace Support Functions
        /// <summary>
        /// Loads a node set from a file or resource and add them to the set of predefined nodes.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="externalReferences"></param>
        public virtual void LoadPredefinedNodes(
            ISystemContext context,
            Assembly assembly,
            string resourcePath,
            IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            if (PredefinedNodes == null)
            {
                PredefinedNodes = new NodeIdDictionary<NodeState>();
            }

            // load the predefined nodes from an XML document.
            var predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromResource(context, resourcePath, assembly, true);

            // add the predefined nodes to the node manager.
            foreach (var node in predefinedNodes)
            {
                AddPredefinedNode(context, node);
            }

            // ensure the reverse references exist.
            AddReverseReferences(externalReferences);
        }

        /// <summary>
        /// Loads a node set from a file or resource and add them to the set of predefined nodes.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        protected virtual NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            return new NodeStateCollection();
        }

        /// <summary>
        /// Loads a node set from a file or resource and add them to the set of predefined nodes.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="externalReferences">The externalReferences is an out parameter that allows the generic server to link to nodes.</param>
        protected virtual void LoadPredefinedNodes(
            ISystemContext context,
            IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            // load the predefined nodes from an XML document.
            var predefinedNodes = LoadPredefinedNodes(context);

            // add the predefined nodes to the node manager.
            foreach (var t in predefinedNodes)
            {
                AddPredefinedNode(context, t);
            }

            // ensure the reverse references exist.
            AddReverseReferences(externalReferences);
        }

        /// <summary>
        /// Replaces the generic node with a node specific to the model.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="predefinedNode">The predefined node.</param>
        protected virtual NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
        {
            var passiveNode = predefinedNode as BaseObjectState;

            if (passiveNode == null)
            {
                return predefinedNode;
            }

            return predefinedNode;
        }

        /// <summary>
        /// Recursively indexes the node and its children and add them to the predefined nodes.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="node">The node to add as predefined node.</param>
        public virtual void AddPredefinedNode(ISystemContext context, NodeState node)
        {
            if (PredefinedNodes == null)
            {
                PredefinedNodes = new NodeIdDictionary<NodeState>();
            }

            // assign a default value to any variable in namespace 0
            if (node is BaseVariableState nodeStateVar)
            {
                if (nodeStateVar.NodeId.NamespaceIndex == 0 && nodeStateVar.Value == null)
                {
                    nodeStateVar.Value = Opc.Ua.TypeInfo.GetDefaultValue(nodeStateVar.DataType,
                        nodeStateVar.ValueRank,
                        ServerData.TypeTree);
                }
            }

            var activeNode = AddBehaviourToPredefinedNode(context, node);
            PredefinedNodes[activeNode.NodeId] = activeNode;

            if (activeNode is BaseTypeState type)
            {
                AddTypesToTypeTree(type);
            }

            // update the root notifiers.
            if (RootNotifiers != null)
            {
                for (var ii = 0; ii < RootNotifiers.Count; ii++)
                {
                    if (RootNotifiers[ii].NodeId == activeNode.NodeId)
                    {
                        RootNotifiers[ii] = activeNode;

                        // need to prevent recursion with the server object.
                        if (activeNode.NodeId != ObjectIds.Server)
                        {
                            activeNode.OnReportEvent = OnReportEvent;

                            if (!activeNode.ReferenceExists(ReferenceTypeIds.HasNotifier, true, ObjectIds.Server))
                            {
                                activeNode.AddReference(ReferenceTypeIds.HasNotifier, true, ObjectIds.Server);
                            }
                        }

                        break;
                    }
                }
            }

            var children = new List<BaseInstanceState>();
            activeNode.GetChildren(context, children);

            foreach (var child in children)
            {
                AddPredefinedNode(context, child);
            }
        }

        /// <summary>
        /// Recursively indexes the node and its children and removes  them from the predefined nodes.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="node">The node to remove from the predefined nodes.</param>
        /// <param name="referencesToRemove">The references to remove.</param>
        protected virtual void RemovePredefinedNode(
            ISystemContext context,
            NodeState node,
            List<LocalReference> referencesToRemove)
        {
            if (PredefinedNodes == null)
            {
                return;
            }

            PredefinedNodes.Remove(node.NodeId);
            node.UpdateChangeMasks(NodeStateChangeMasks.Deleted);
            node.ClearChangeMasks(context, false);
            OnNodeRemoved(node);

            // remove from the parent.

            if (node is BaseInstanceState instance)
            {
                instance.Parent?.RemoveChild(instance);
            }

            // remove children.
            var children = new List<BaseInstanceState>();
            node.GetChildren(context, children);

            foreach (var child in children)
            {
                node.RemoveChild(child);
            }

            foreach (var child in children)
            {
                RemovePredefinedNode(context, child, referencesToRemove);
            }

            // remove from type table.

            if (node is BaseTypeState type)
            {
                ServerData.TypeTree.Remove(type.NodeId);
            }

            // remove inverse references.
            var references = new List<IReference>();
            node.GetReferences(context, references);

            foreach (var reference in references)
            {
                if (reference.TargetId.IsAbsolute)
                {
                    continue;
                }

                var referenceToRemove = new LocalReference(
                    (NodeId)reference.TargetId,
                    reference.ReferenceTypeId,
                    !reference.IsInverse,
                    node.NodeId);

                referencesToRemove.Add(referenceToRemove);
            }
        }

        /// <summary>
        /// Called after a node has been deleted.
        /// </summary>
        /// <param name="node">The removed node.</param>
        protected virtual void OnNodeRemoved(NodeState node)
        {
            // overridden by the sub-class.
        }

        /// <summary>
        /// Ensures that all reverse references exist.
        /// </summary>
        /// <param name="externalReferences">A list of references to add to external targets.</param>
        public virtual void AddReverseReferences(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            if (PredefinedNodes == null)
            {
                return;
            }

            foreach (var source in PredefinedNodes.Values)
            {

                IList<IReference> references = new List<IReference>();
                source.GetReferences(SystemContext, references);

                foreach (var reference in references)
                {
                    // nothing to do with external nodes.
                    if (reference.TargetId == null || reference.TargetId.IsAbsolute)
                    {
                        continue;
                    }

                    // no need to add HasSubtype references since these are handled via the type table.
                    if (reference.ReferenceTypeId == ReferenceTypeIds.HasSubtype)
                    {
                        continue;
                    }

                    var targetId = (NodeId)reference.TargetId;

                    // check for data type encoding references.
                    if (reference.IsInverse && reference.ReferenceTypeId == ReferenceTypeIds.HasEncoding)
                    {
                        ServerData.TypeTree.AddEncoding(targetId, source.NodeId);
                    }

                    // add inverse reference to internal targets.

                    if (PredefinedNodes.TryGetValue(targetId, out var target))
                    {
                        if (!target.ReferenceExists(reference.ReferenceTypeId, !reference.IsInverse, source.NodeId))
                        {
                            target.AddReference(reference.ReferenceTypeId, !reference.IsInverse, source.NodeId);
                        }

                        continue;
                    }

                    // check for inverse references to external notifiers.
                    if (reference.IsInverse && reference.ReferenceTypeId == ReferenceTypeIds.HasNotifier)
                    {
                        AddRootNotifier(source);
                    }

                    // nothing more to do for references to nodes managed by this manager.
                    if (IsNodeIdInNamespace(targetId))
                    {
                        continue;
                    }

                    // add external reference.
                    AddExternalReference(
                        targetId,
                        reference.ReferenceTypeId,
                        !reference.IsInverse,
                        source.NodeId,
                        externalReferences);
                }
            }
        }

        /// <summary>
        /// Adds an external reference to the dictionary.
        /// </summary>
        /// <param name="sourceId">The ID of the source node.</param>
        /// <param name="referenceTypeId">The ID of the reference type.</param>
        /// <param name="isInverse">Is the reference an inverse reference?</param>
        /// <param name="targetId">The ID of the target node.</param>
        /// <param name="externalReferences">The externalReferences is an out parameter that allows the generic server to link to nodes.</param>
        protected void AddExternalReference(
            NodeId sourceId,
            NodeId referenceTypeId,
            bool isInverse,
            NodeId targetId,
            IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            // get list of references to external nodes.

            if (!externalReferences.TryGetValue(sourceId, out var referencesToAdd))
            {
                externalReferences[sourceId] = referencesToAdd = new List<IReference>();
            }

            // add reserve reference from external node.
            var referenceToAdd = new ReferenceNode
            {
                ReferenceTypeId = referenceTypeId, IsInverse = isInverse, TargetId = targetId
            };


            referencesToAdd.Add(referenceToAdd);
        }

        /// <summary>
        /// Recursively adds the types to the type tree.
        /// </summary>
        /// <param name="type">The type to add.</param>
        protected void AddTypesToTypeTree(BaseTypeState type)
        {
            if (!NodeId.IsNull(type.SuperTypeId))
            {
                if (!ServerData.TypeTree.IsKnown(type.SuperTypeId))
                {
                    AddTypesToTypeTree(type.SuperTypeId);
                }
            }

            if (type.NodeClass != NodeClass.ReferenceType)
            {
                ServerData.TypeTree.AddSubtype(type.NodeId, type.SuperTypeId);
            }
            else
            {
                ServerData.TypeTree.AddReferenceSubtype(type.NodeId, type.SuperTypeId, type.BrowseName);
            }
        }

        /// <summary>
        /// Recursively adds the types to the type tree.
        /// </summary>
        /// <param name="typeId">The node ID of the type to add.</param>
        protected void AddTypesToTypeTree(NodeId typeId)
        {
            if (!PredefinedNodes.TryGetValue(typeId, out var node))
            {
                return;
            }

            if (!(node is BaseTypeState type))
            {
                return;
            }

            AddTypesToTypeTree(type);
        }

        /// <summary>
        /// Finds the specified and checks if it is of the expected type.
        /// </summary>
        /// <param name="nodeId">The node to find.</param>
        /// <param name="expectedType">The expected type of the node.</param>
        /// <returns>Returns null if not found or not of the correct type.</returns>
        public NodeState FindPredefinedNode(NodeId nodeId, Type expectedType)
        {
            if (nodeId == null)
            {
                return null;
            }

            if (!PredefinedNodes.TryGetValue(nodeId, out var node))
            {
                return null;
            }

            if (expectedType != null)
            {
                if (!expectedType.IsInstanceOfType(node))
                {
                    return null;
                }
            }

            return node;
        }
        #endregion

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public virtual void DeleteAddressSpace()
        {
            lock (Lock)
            {
                if (PredefinedNodes != null)
                {
                    foreach (var node in PredefinedNodes.Values)
                    {
                        Utils.SilentDispose(node);
                    }

                    PredefinedNodes.Clear();
                }
            }
        }

        /// <summary>
        /// Returns an unique handle for the node.
        /// </summary>
        /// <param name="nodeId">The node to get the handle for.</param>
        /// <returns>A node handle, null if the node manager does not recognize the node id.</returns>
        /// <remarks>
        /// This must efficiently determine whether the node belongs to the node manager. If it does belong to
        /// NodeManager it should return a handle that does not require the NodeId to be validated again when
        /// the handle is passed into other methods such as 'Read' or 'Write'.
        /// </remarks>
        public virtual object GetManagerHandle(NodeId nodeId)
        {
            lock (Lock)
            {
                return GetManagerHandle(SystemContext, nodeId, null);
            }
        }

        /// <summary>
        /// Returns an unique handle for the node.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="nodeId">The node to get the handle for.</param>
        /// <param name="cache">The list of nodes to check.</param>
        /// <returns>A node handle, null if the node manager does not recognize the node id.</returns>
        protected virtual UaNodeHandle GetManagerHandle(UaServerContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            if (!IsNodeIdInNamespace(nodeId))
            {
                return null;
            }

            if (PredefinedNodes != null)
            {
                if (PredefinedNodes.TryGetValue(nodeId, out var node))
                {
                    var handle = new UaNodeHandle { NodeId = nodeId, Node = node, Validated = true };


                    return handle;
                }
            }

            return null;
        }

        /// <summary>
        /// This method is used to add bi-directional references to nodes from other node managers.
        /// </summary>
        /// <remarks>
        /// The additional references are optional, however, the NodeManager should support them.
        /// </remarks>
        public virtual void AddReferences(IDictionary<NodeId, IList<IReference>> references)
        {
            lock (Lock)
            {
                foreach (var current in references)
                {
                    // get the handle.
                    var source = GetManagerHandle(SystemContext, current.Key, null);

                    // only support external references to nodes that are stored in memory.
                    if (source == null || !source.Validated || source.Node == null)
                    {
                        continue;
                    }

                    // add reference to external target.
                    foreach (var reference in current.Value)
                    {
                        if (!source.Node.ReferenceExists(reference.ReferenceTypeId, reference.IsInverse, reference.TargetId))
                        {
                            source.Node.AddReference(reference.ReferenceTypeId, reference.IsInverse, reference.TargetId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to delete bi-directional references to nodes from other node managers.
        /// </summary>
        public virtual ServiceResult DeleteReference(
            object         sourceHandle,
            NodeId         referenceTypeId,
            bool           isInverse,
            ExpandedNodeId targetId,
            bool           deleteBiDirectional)
        {
            lock (Lock)
            {
                // get the handle.
                var source = IsHandleInNamespace(sourceHandle);

                if (source == null)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                // only support external references to nodes that are stored in memory.
                if (!source.Validated || source.Node == null)
                {
                    return StatusCodes.BadNotSupported;
                }

                // only support references to Source Areas.
                source.Node.RemoveReference(referenceTypeId, isInverse, targetId);

                if (deleteBiDirectional)
                {
                    // check if the target is also managed by this node manager.
                    if (!targetId.IsAbsolute)
                    {
                        var target = GetManagerHandle(SystemContext, (NodeId)targetId, null);

                        if (target != null && target.Validated && target.Node != null)
                        {
                            target.Node.RemoveReference(referenceTypeId, !isInverse, source.NodeId);
                        }
                    }
                }

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Returns the basic metadata for the node. Returns null if the node does not exist.
        /// </summary>
        /// <remarks>
        /// This method validates any placeholder handle.
        /// </remarks>
        /// <param name="context">The UA server implementation of the IOperationContext interface.</param>
        /// <param name="targetHandle">The node to get the basic metadata from.</param>
        /// <param name="resultMask">The returned basic metadata.</param>
        public virtual UaNodeMetadata GetNodeMetadata(
            UaServerOperationContext context,
            object           targetHandle,
            BrowseResultMask resultMask)
        {
            var systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // check for valid handle.
                var handle = IsHandleInNamespace(targetHandle);

                if (handle == null)
                {
                    return null;
                }

                // validate node.
                var target = ValidateNode(systemContext, handle, null);

                if (target == null)
                {
                    return null;
                }

                // read the attributes.
                var values = target.ReadAttributes(
                    systemContext,
                    Attributes.WriteMask,
                    Attributes.UserWriteMask,
                    Attributes.DataType,
                    Attributes.ValueRank,
                    Attributes.ArrayDimensions,
                    Attributes.AccessLevel,
                    Attributes.UserAccessLevel,
                    Attributes.EventNotifier,
                    Attributes.Executable,
                    Attributes.UserExecutable,
                    Attributes.AccessRestrictions,
                    Attributes.RolePermissions,
                    Attributes.UserRolePermissions);

                // construct the metadata object.
                var metadata = new UaNodeMetadata(target, target.NodeId)
                {
                    NodeClass = target.NodeClass, BrowseName = target.BrowseName, DisplayName = target.DisplayName
                };


                if (values[0] != null && values[1] != null)
                {
                    metadata.WriteMask = (AttributeWriteMask)(((uint)values[0]) & ((uint)values[1]));
                }

                metadata.DataType = (NodeId)values[2];

                if (values[3] != null)
                {
                    metadata.ValueRank = (int)values[3];
                }

                metadata.ArrayDimensions = (IList<uint>)values[4];

                if (values[5] != null && values[6] != null)
                {
                    metadata.AccessLevel = (byte)(((byte)values[5]) & ((byte)values[6]));
                }

                if (values[7] != null)
                {
                    metadata.EventNotifier = (byte)values[7];
                }

                if (values[8] != null && values[9] != null)
                {
                    metadata.Executable = (((bool)values[8]) && ((bool)values[9]));
                }

                if (values[10] != null)
                {
                    metadata.AccessRestrictions = (AccessRestrictionType)Enum.ToObject(typeof(AccessRestrictionType), values[10]);
                }

                if (values[11] != null)
                {
                    metadata.RolePermissions = new RolePermissionTypeCollection(ExtensionObject.ToList<RolePermissionType>(values[11]));
                }

                if (values[12] != null)
                {
                    metadata.UserRolePermissions = new RolePermissionTypeCollection(ExtensionObject.ToList<RolePermissionType>(values[12]));
                }

                SetDefaultPermissions(systemContext, target, metadata);

                // get instance references.

                if (target is BaseInstanceState instance)
                {
                    metadata.TypeDefinition = instance.TypeDefinitionId;
                    metadata.ModellingRule = instance.ModellingRuleId;
                }

                // fill in the common attributes.
                return metadata;
            }
        }

        /// <summary>
        /// Sets the AccessRestrictions, RolePermissions and UserRolePermissions values in the metadata
        /// </summary>
        /// <param name="values"></param>
        /// <param name="metadata"></param>
        private static void SetAccessAndRolePermissions(List<object> values, UaNodeMetadata metadata)
        {
            if (values[0] != null)
            {
                metadata.AccessRestrictions = (AccessRestrictionType)Enum.ToObject(typeof(AccessRestrictionType), values[0]);
            }
            if (values[1] != null)
            {
                metadata.RolePermissions = new RolePermissionTypeCollection(ExtensionObject.ToList<RolePermissionType>(values[1]));
            }
            if (values[2] != null)
            {
                metadata.UserRolePermissions = new RolePermissionTypeCollection(ExtensionObject.ToList<RolePermissionType>(values[2]));
            }
        }

        /// <summary>
        /// Reads and caches the Attributes used by the AccessRestrictions and RolePermission validation process
        /// </summary>
        /// <param name="uniqueNodesServiceAttributes">The cache used to save the attributes</param>
        /// <param name="systemContext">The context</param>
        /// <param name="target">The target for which the attributes are read and cached</param>
        /// <param name="key">The key representing the NodeId for which the cache is kept</param>
        /// <returns>The values of the attributes</returns>
        private static List<object> ReadAndCacheValidationAttributes(Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes, UaServerContext systemContext, NodeState target, NodeId key)
        {
            List<object> values = ReadValidationAttributes(systemContext, target);
            uniqueNodesServiceAttributes[key] = values;

            return values;
        }

        /// <summary>
        /// Reads the Attributes used by the AccessRestrictions and RolePermission validation process
        /// </summary>
        /// <param name="systemContext">The context</param>
        /// <param name="target">The target for which the attributes are read and cached</param>
        /// <returns>The values of the attributes</returns>
        private static List<object> ReadValidationAttributes(UaServerContext systemContext, NodeState target)
        {
            // This is the list of attributes to be populated by GetNodeMetadata from CustomNodeManagers.
            // The are originating from services in the context of AccessRestrictions and RolePermission validation.
            // For such calls the other attributes are ignored since reading them might trigger unnecessary callbacks
            List<object> values = target.ReadAttributes(systemContext,
                                           Attributes.AccessRestrictions,
                                           Attributes.RolePermissions,
                                           Attributes.UserRolePermissions);

            return values;
        }

        /// <summary>
        /// Browses the references from a node managed by the node manager.
        /// </summary>
        /// <remarks>
        /// The continuation point is created for every browse operation and contains the browse parameters.
        /// The node manager can store its state information in the Data and Index properties.
        /// </remarks>
        public virtual void Browse(
            UaServerOperationContext context,
            ref UaContinuationPoint       continuationPoint,
            IList<ReferenceDescription> references)
        {
            if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));
            if (references == null) throw new ArgumentNullException(nameof(references));

            var systemContext = SystemContext.Copy(context);

            // check for valid view.
            ValidateViewDescription(systemContext, continuationPoint.View);

            INodeBrowser browser;

            lock (Lock)
            {
                // check for valid handle.
                var handle = IsHandleInNamespace(continuationPoint.NodeToBrowse);

                if (handle == null)
                {
                    throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
                }

                // validate node.
                var source = ValidateNode(systemContext, handle, null);

                if (source == null)
                {
                    throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
                }

                // check if node is in the view.
                if (!IsNodeInView(systemContext, continuationPoint, source))
                {
                    throw new ServiceResultException(StatusCodes.BadNodeNotInView);
                }

                // check if node is in the view.
                if (!IsNodeAccessibleForUser(systemContext, continuationPoint, source))
                {
                    throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
                }

                // check for previous continuation point.
                browser = continuationPoint.Data as INodeBrowser;

                // fetch list of references.
                if (browser == null)
                {
                    // create a new browser.
                    continuationPoint.Data = browser = source.CreateBrowser(
                        systemContext,
                        continuationPoint.View,
                        continuationPoint.ReferenceTypeId,
                        continuationPoint.IncludeSubtypes,
                        continuationPoint.BrowseDirection,
                        null,
                        null,
                        false);
                }
            }

            // prevent multiple access the browser object.
            lock (browser)
            {
                // apply filters to references.
                var cache = new Dictionary<NodeId, NodeState>();

                for (var reference = browser.Next(); reference != null; reference = browser.Next())
                {
                    // validate Browse permission
                    var serviceResult = ValidateRolePermissions(context,
                        ExpandedNodeId.ToNodeId(reference.TargetId, ServerData.NamespaceUris),
                        PermissionType.Browse);
                    if (ServiceResult.IsBad(serviceResult))
                    {
                        // ignore reference
                        continue;
                    }
                    // create the type definition reference.
                    var description = GetReferenceDescription(systemContext, cache, reference, continuationPoint);

                    if (description == null)
                    {
                        continue;
                    }

                    // check if limit reached.
                    if (continuationPoint.MaxResultsToReturn != 0 && references.Count >= continuationPoint.MaxResultsToReturn)
                    {
                        browser.Push(reference);
                        return;
                    }

                    references.Add(description);
                }

                // release the continuation point if all done.
                continuationPoint.Dispose();
                continuationPoint = null;
            }
        }

        #region Browse Support Functions
        /// <summary>
        /// Validates the view description passed to a browse request (throws on error).
        /// </summary>
        protected virtual void ValidateViewDescription(UaServerContext context, ViewDescription view)
        {
            if (ViewDescription.IsDefault(view))
            {
                return;
            }

            var node = (ViewState)FindPredefinedNode(view.ViewId, typeof(ViewState));

            if (node == null)
            {
                throw new ServiceResultException(StatusCodes.BadViewIdUnknown);
            }

            if (view.Timestamp != DateTime.MinValue)
            {
                throw new ServiceResultException(StatusCodes.BadViewTimestampInvalid);
            }

            if (view.ViewVersion != 0)
            {
                throw new ServiceResultException(StatusCodes.BadViewVersionInvalid);
            }
        }

        /// <summary>
        /// Checks if the node is in the view.
        /// </summary>
        protected virtual bool IsNodeInView(UaServerContext context, UaContinuationPoint continuationPoint, NodeState node)
        {
            if (continuationPoint == null || ViewDescription.IsDefault(continuationPoint.View))
            {
                return true;
            }

            return IsNodeInView(context, continuationPoint.View.ViewId, node);
        }

        /// <summary>
        /// Checks if the node is in the view.
        /// </summary>
        protected virtual bool IsNodeInView(UaServerContext context, NodeId viewId, NodeState node)
        {
            var view = (ViewState)FindPredefinedNode(viewId, typeof(ViewState));

            if (view != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the reference is in the view.
        /// </summary>
        protected virtual bool IsReferenceInView(UaServerContext context, UaContinuationPoint continuationPoint, IReference reference)
        {
            return true;
        }

        /// <summary>
        /// Checks if the user is allowed to access this node.
        /// </summary>
        protected virtual bool IsNodeAccessibleForUser(UaServerContext context, UaContinuationPoint continuationPoint, NodeState node)
        {
            return true;
        }

        /// <summary>
        /// Checks if the user is allowed to access this reference.
        /// </summary>
        protected virtual bool IsReferenceAccessibleForUser(UaServerContext context, UaContinuationPoint continuationPoint, IReference reference)
        {
            return true;
        }

        /// <summary>
        /// Returns the references for the node that meets the criteria specified.
        /// </summary>
        protected virtual ReferenceDescription GetReferenceDescription(
            UaServerContext context,
            Dictionary<NodeId, NodeState> cache,
            IReference reference,
            UaContinuationPoint continuationPoint)
        {
            SystemContext.Copy(context);

            // create the type definition reference.
            var description = new ReferenceDescription { NodeId = reference.TargetId };

            description.SetReferenceType(continuationPoint.ResultMask, reference.ReferenceTypeId, !reference.IsInverse);

            // check if reference is in the view.
            if (!IsReferenceInView(context, continuationPoint, reference))
            {
                return null;
            }

            // check if the user is allowed to access this reference.
            if (!IsReferenceAccessibleForUser(context, continuationPoint, reference))
            {
                return null;
            }

            // do not cache target parameters for remote nodes.
            if (reference.TargetId.IsAbsolute)
            {
                // only return remote references if no node class filter is specified.
                if (continuationPoint.NodeClassMask != 0)
                {
                    return null;
                }

                return description;
            }

            NodeState target = null;

            // check for local reference.

            if (reference is NodeStateReference referenceInfo)
            {
                target = referenceInfo.Target;
            }

            // check for internal reference.
            if (target == null)
            {
                if (GetManagerHandle(context, (NodeId)reference.TargetId, null) is UaNodeHandle handle)
                {
                    target = ValidateNode(context, handle, null);
                }
            }

            // the target may be a reference to a node in another node manager. In these cases
            // the target attributes must be fetched by the caller. The Unfiltered flag tells the
            // caller to do that.
            if (target == null)
            {
                description.Unfiltered = true;
                return description;
            }

            // apply node class filter.
            if (continuationPoint.NodeClassMask != 0 && ((continuationPoint.NodeClassMask & (uint)target.NodeClass) == 0))
            {
                return null;
            }

            // check if target is in the view.
            if (!IsNodeInView(context, continuationPoint, target))
            {
                return null;
            }

            // look up the type definition.
            NodeId typeDefinition = null;

            if (target is BaseInstanceState instance)
            {
                typeDefinition = instance.TypeDefinitionId;
            }

            // set target attributes.
            description.SetTargetAttributes(
                continuationPoint.ResultMask,
                target.NodeClass,
                target.BrowseName,
                target.DisplayName,
                typeDefinition);

            return description;
        }
        #endregion

        /// <summary>
        /// Returns the target of the specified browse path fragment(s).
        /// </summary>
        /// <remarks>
        /// If reference exists but the node manager does not know the browse name it must
        /// return the NodeId as an unresolvedTargetIds. The caller will try to check the
        /// browse name.
        /// </remarks>
        public virtual void TranslateBrowsePath(
            UaServerOperationContext      context,
            object                sourceHandle,
            RelativePathElement   relativePath,
            IList<ExpandedNodeId> targetIds,
            IList<NodeId>         unresolvedTargetIds)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();

            lock (Lock)
            {
                // check for valid handle.
                var handle = IsHandleInNamespace(sourceHandle);

                if (handle == null)
                {
                    return;
                }

                // validate node.
                var source = ValidateNode(systemContext, handle, operationCache);

                if (source == null)
                {
                    return;
                }

                // get list of references that relative path.
                var browser = source.CreateBrowser(
                    systemContext,
                    null,
                    relativePath.ReferenceTypeId,
                    relativePath.IncludeSubtypes,
                    (relativePath.IsInverse) ? BrowseDirection.Inverse : BrowseDirection.Forward,
                    relativePath.TargetName,
                    null,
                    false);

                // check the browse names.
                try
                {
                    for (var reference = browser.Next(); reference != null; reference = browser.Next())
                    {
                        // ignore unknown external references.
                        if (reference.TargetId.IsAbsolute)
                        {
                            continue;
                        }

                        NodeState target = null;

                        // check for local reference.

                        if (reference is NodeStateReference referenceInfo)
                        {
                            target = referenceInfo.Target;
                        }

                        if (target == null)
                        {
                            var targetId = (NodeId)reference.TargetId;

                            // the target may be a reference to a node in another node manager.
                            if (!IsNodeIdInNamespace(targetId))
                            {
                                unresolvedTargetIds.Add((NodeId)reference.TargetId);
                                continue;
                            }

                            // look up the target manually.
                            var targetHandle = GetManagerHandle(systemContext, targetId, operationCache);

                            if (targetHandle == null)
                            {
                                continue;
                            }

                            // validate target.
                            target = ValidateNode(systemContext, targetHandle, operationCache);

                            if (target == null)
                            {
                                continue;
                            }
                        }

                        // check browse name.
                        if (target.BrowseName == relativePath.TargetName)
                        {
                            if (!targetIds.Contains(reference.TargetId))
                            {
                                targetIds.Add(reference.TargetId);
                            }
                        }
                    }
                }
                finally
                {
                    browser.Dispose();
                }
            }
        }

        /// <summary>
        /// Reads the value for the specified attribute.
        /// </summary>
        public virtual void Read(
            UaServerOperationContext context,
            double               maxAge,
            IList<ReadValueId>   nodesToRead,
            IList<DataValue>     values,
            IList<ServiceResult> errors)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            var nodesToValidate = new List<UaNodeHandle>();

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToRead.Count; ii++)
                {
                    var nodeToRead = nodesToRead[ii];

                    // skip items that have already been processed.
                    if (nodeToRead.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    var handle = GetManagerHandle(systemContext, nodeToRead.NodeId, operationCache);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToRead.Processed = true;

                    // create an initial value.
                    var value = values[ii] = new DataValue();

                    value.Value           = null;
                    value.ServerTimestamp = DateTime.UtcNow;
                    value.SourceTimestamp = DateTime.MinValue;
                    value.StatusCode      = StatusCodes.Good;

                    // check if the node is a area in memory.
                    if (handle.Node == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a separate operation
                        handle.Index = ii;
                        nodesToValidate.Add(handle);

                        continue;
                    }

                    // read the attribute value.
                    errors[ii] = handle.Node.ReadAttribute(
                        systemContext,
                        nodeToRead.AttributeId,
                        nodeToRead.ParsedIndexRange,
                        nodeToRead.DataEncoding,
                        value);
#if DEBUG
                    if (nodeToRead.AttributeId == Attributes.Value)
                    {
                        UaServerUtils.EventLog.ReadValueRange(nodeToRead.NodeId, value.WrappedValue, nodeToRead.IndexRange);
                    }
#endif
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }
            }

            // validates the nodes (reads values from the underlying data source if required).
            Read(
                systemContext,
                nodesToRead,
                values,
                errors,
                nodesToValidate,
                operationCache);
        }

        #region Read Support Functions
        /// <summary>
        /// Finds a node in the dynamic cache.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="handle">The node handle.</param>
        /// <param name="cache">The cache to search.</param>
        /// <returns>The node if found. Null otherwise.</returns>
        public virtual NodeState FindNodeInCache(
            UaServerContext context,
            UaNodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            NodeState target;

            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            // construct id for root node.
            var rootId = handle.RootId;

            if (cache != null)
            {
                // lookup component in local cache for request.
                if (cache.TryGetValue(handle.NodeId, out target))
                {
                    return target;
                }

                // lookup root in local cache for request.
                if (!string.IsNullOrEmpty(handle.ComponentPath))
                {
                    if (cache.TryGetValue(rootId, out target))
                    {
                        target = target.FindChildBySymbolicName(context, handle.ComponentPath);

                        // component exists.
                        if (target != null)
                        {
                            return target;
                        }
                    }
                }
            }

            // lookup component in shared cache.
            target = LookupNodeInComponentCache(context, handle);

            if (target != null)
            {
                return target;
            }

            return null;
        }

        /// <summary>
        /// Marks the handle as validated and saves the node in the dynamic cache.
        /// </summary>
        protected virtual NodeState ValidationComplete(
            UaServerContext context,
            UaNodeHandle handle,
            NodeState node,
            IDictionary<NodeId, NodeState> cache)
        {
            handle.Node = node;
            handle.Validated = true;

            if (cache != null && handle != null)
            {
                cache[handle.NodeId] = node;
            }

            return node;
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        public virtual NodeState ValidateNode(
            UaServerContext context,
            UaNodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            // lookup in cache.
            var target = FindNodeInCache(context, handle, cache);

            if (target != null)
            {
                handle.Node = target;
                handle.Validated = true;
                return handle.Node;
            }

            // return default.
            return handle.Node;
        }

        /// <summary>
        /// Validates the nodes and reads the values from the underlying source.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nodesToRead">The nodes to read.</param>
        /// <param name="values">The values.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="nodesToValidate">The nodes to validate.</param>
        /// <param name="cache">The cache.</param>
        protected virtual void Read(
            UaServerContext context,
            IList<ReadValueId> nodesToRead,
            IList<DataValue> values,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToValidate,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToValidate)
            {
                lock (Lock)
                {
                    // validate node.
                    var source = ValidateNode(context, handle, cache);

                    if (source == null)
                    {
                        continue;
                    }

                    var nodeToRead = nodesToRead[handle.Index];
                    var value = values[handle.Index];

                    // update the attribute value.
                    errors[handle.Index] = source.ReadAttribute(
                        context,
                        nodeToRead.AttributeId,
                        nodeToRead.ParsedIndexRange,
                        nodeToRead.DataEncoding,
                        value);
                }
            }
        }
        #endregion

        /// <summary>
        /// Writes the value for the specified attributes.
        /// </summary>
        public virtual void Write(
            UaServerOperationContext     context,
            IList<WriteValue>    nodesToWrite,
            IList<ServiceResult> errors)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            var nodesToValidate = new List<UaNodeHandle>();

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToWrite.Count; ii++)
                {
                    var nodeToWrite = nodesToWrite[ii];

                    // skip items that have already been processed.
                    if (nodeToWrite.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    var handle = GetManagerHandle(systemContext, nodeToWrite.NodeId, operationCache);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToWrite.Processed = true;

                    // index range is not supported.
                    if (nodeToWrite.AttributeId != Attributes.Value)
                    {
                        if (!string.IsNullOrEmpty(nodeToWrite.IndexRange))
                        {
                            errors[ii] = StatusCodes.BadWriteNotSupported;
                            continue;
                        }
                    }

                    // check if the node is a area in memory.
                    if (handle.Node == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a separate operation.
                        handle.Index = ii;
                        nodesToValidate.Add(handle);

                        continue;
                    }

                    // check if the node is AnalogItem and the values are outside the InstrumentRange.
                    AnalogItemState analogItemState = handle.Node as AnalogItemState;
                    if (analogItemState != null && analogItemState.InstrumentRange != null)
                    {
                        try
                        {
                            if (nodeToWrite.Value.Value is Array array)
                            {
                                bool isOutOfRange = false;
                                foreach (var arrayValue in array)
                                {
                                    double newValue = Convert.ToDouble(arrayValue, CultureInfo.InvariantCulture);
                                    if (newValue > analogItemState.InstrumentRange.Value.High ||
                                        newValue < analogItemState.InstrumentRange.Value.Low)
                                    {
                                        isOutOfRange = true;
                                        break;
                                    }
                                }
                                if (isOutOfRange)
                                {
                                    errors[ii] = StatusCodes.BadOutOfRange;
                                    continue;
                                }
                            }
                            else
                            {
                                double newValue = Convert.ToDouble(nodeToWrite.Value.Value, CultureInfo.InvariantCulture);

                                if (newValue > analogItemState.InstrumentRange.Value.High ||
                                    newValue < analogItemState.InstrumentRange.Value.Low)
                                {
                                    errors[ii] = StatusCodes.BadOutOfRange;
                                    continue;
                                }
                            }
                        }
                        catch
                        {
                            //skip the InstrumentRange check if the transformation isn't possible.
                        }

                    }

#if DEBUG
                    UaServerUtils.EventLog.WriteValueRange(nodeToWrite.NodeId, nodeToWrite.Value.WrappedValue, nodeToWrite.IndexRange);
#endif

                    var propertyState = handle.Node as PropertyState;
                    object previousPropertyValue = null;

                    if (propertyState != null)
                    {
                        if (propertyState.Value is ExtensionObject extension)
                        {
                            previousPropertyValue = extension.Body;
                        }
                        else
                        {
                            previousPropertyValue = propertyState.Value;
                        }
                    }

                    DataValue oldValue = null;

                    if (ServerData?.Auditing == true)
                    {
                        //current server supports auditing 
                        oldValue = new DataValue();
                        // read the old value for the purpose of auditing
                        handle.Node.ReadAttribute(systemContext, nodeToWrite.AttributeId, nodeToWrite.ParsedIndexRange, null, oldValue);
                    }

                    // write the attribute value.
                    errors[ii] = handle.Node.WriteAttribute(
                        systemContext,
                        nodeToWrite.AttributeId,
                        nodeToWrite.ParsedIndexRange,
                        nodeToWrite.Value);

                    // report the write value audit event 
                    ServerData.ReportAuditWriteUpdateEvent(systemContext, nodeToWrite, oldValue?.Value, errors[ii]?.StatusCode ?? StatusCodes.Good);

                    if (!ServiceResult.IsGood(errors[ii]))
                    {
                        continue;
                    }

                    if (propertyState != null)
                    {
                        object propertyValue;

                        if (nodeToWrite.Value.Value is ExtensionObject extension)
                        {
                            propertyValue = extension.Body;
                        }
                        else
                        {
                            propertyValue = nodeToWrite.Value.Value;
                        }

                        CheckIfSemanticsHaveChanged(systemContext, propertyState, propertyValue, previousPropertyValue);
                    }

                    // updates to source finished - report changes to monitored items.
                    handle.Node.ClearChangeMasks(systemContext, true);
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }
            }

            // validates the nodes and writes the value to the underlying system.
            Write(
                systemContext,
                nodesToWrite,
                errors,
                nodesToValidate,
                operationCache);
        }

        private void CheckIfSemanticsHaveChanged(UaServerContext systemContext, PropertyState property, object newPropertyValue, object previousPropertyValue)
        {
            // check if the changed property is one that can trigger semantic changes
            var propertyName = property.BrowseName.Name;

            if (propertyName != BrowseNames.EURange &&
                propertyName != BrowseNames.InstrumentRange &&
                propertyName != BrowseNames.EngineeringUnits &&
                propertyName != BrowseNames.Title &&
                propertyName != BrowseNames.AxisDefinition &&
                propertyName != BrowseNames.FalseState &&
                propertyName != BrowseNames.TrueState &&
                propertyName != BrowseNames.EnumStrings &&
                propertyName != BrowseNames.XAxisDefinition &&
                propertyName != BrowseNames.YAxisDefinition &&
                propertyName != BrowseNames.ZAxisDefinition)
            {
                return;
            }

            //look for the Parent and its monitoring items
            foreach (var monitoredNode in MonitoredNodes.Values)
            {
                var propertyState = monitoredNode.Node.FindChild(systemContext, property.BrowseName);

                if (propertyState != null && property != null && propertyState.NodeId == property.NodeId && !Utils.IsEqual(newPropertyValue, previousPropertyValue))
                {
                    foreach (var monitoredItem in monitoredNode.DataChangeMonitoredItems)
                    {
                        if (monitoredItem.AttributeId == Attributes.Value)
                        {
                            var node = monitoredNode.Node;

                            if ((node is AnalogItemState && (propertyName == BrowseNames.EURange || propertyName == BrowseNames.EngineeringUnits)) ||
                                (node is TwoStateDiscreteState && (propertyName == BrowseNames.FalseState || propertyName == BrowseNames.TrueState)) ||
                                (node is MultiStateDiscreteState && (propertyName == BrowseNames.EnumStrings)) ||
                                (node is ArrayItemState && (propertyName == BrowseNames.InstrumentRange || propertyName == BrowseNames.EURange || propertyName == BrowseNames.EngineeringUnits || propertyName == BrowseNames.Title)) ||
                                ((node is YArrayItemState || node is XYArrayItemState) && (propertyName == BrowseNames.InstrumentRange || propertyName == BrowseNames.EURange || propertyName == BrowseNames.EngineeringUnits || propertyName == BrowseNames.Title || propertyName == BrowseNames.XAxisDefinition)) ||
                                (node is ImageItemState && (propertyName == BrowseNames.InstrumentRange || propertyName == BrowseNames.EURange || propertyName == BrowseNames.EngineeringUnits || propertyName == BrowseNames.Title || propertyName == BrowseNames.XAxisDefinition || propertyName == BrowseNames.YAxisDefinition)) ||
                                (node is CubeItemState && (propertyName == BrowseNames.InstrumentRange || propertyName == BrowseNames.EURange || propertyName == BrowseNames.EngineeringUnits || propertyName == BrowseNames.Title || propertyName == BrowseNames.XAxisDefinition || propertyName == BrowseNames.YAxisDefinition || propertyName == BrowseNames.ZAxisDefinition)) ||
                                (node is NDimensionArrayItemState && (propertyName == BrowseNames.InstrumentRange || propertyName == BrowseNames.EURange || propertyName == BrowseNames.EngineeringUnits || propertyName == BrowseNames.Title || propertyName == BrowseNames.AxisDefinition)))
                            {
                                monitoredItem.SetSemanticsChanged();

                                var value = new DataValue { ServerTimestamp = DateTime.UtcNow };

                                monitoredNode.Node.ReadAttribute(systemContext, Attributes.Value, monitoredItem.IndexRange, null, value);

                                monitoredItem.QueueValue(value, ServiceResult.Good, true);
                            }
                        }
                    }
                }
            }
        }

        #region Write Support Functions
        /// <summary>
        /// Validates the nodes and writes the value to the underlying system.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nodesToWrite">The nodes to write.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="nodesToValidate">The nodes to validate.</param>
        /// <param name="cache">The cache.</param>
        protected virtual void Write(
            UaServerContext context,
            IList<WriteValue> nodesToWrite,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToValidate,
            IDictionary<NodeId, NodeState> cache)
        {
            // validates the nodes (reads values from the underlying data source if required).
            foreach (var handle in nodesToValidate)
            {
                lock (Lock)
                {
                    // validate node.
                    var source = ValidateNode(context, handle, cache);

                    if (source == null)
                    {
                        continue;
                    }

                    var nodeToWrite = nodesToWrite[handle.Index];

                    // write the attribute value.
                    errors[handle.Index] = source.WriteAttribute(
                        context,
                        nodeToWrite.AttributeId,
                        nodeToWrite.ParsedIndexRange,
                        nodeToWrite.Value);

                    // updates to source finished - report changes to monitored items.
                    source.ClearChangeMasks(context, false);
                }
            }
        }
        #endregion

        /// <summary>
        /// Reads the history for the specified nodes.
        /// </summary>
        public virtual void HistoryRead(
            UaServerOperationContext          context,
            HistoryReadDetails        details,
            TimestampsToReturn        timestampsToReturn,
            bool                      releaseContinuationPoints,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult>  results,
            IList<ServiceResult>      errors)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            var nodesToProcess = new List<UaNodeHandle>();

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToRead.Count; ii++)
                {
                    var nodeToRead = nodesToRead[ii];

                    // skip items that have already been processed.
                    if (nodeToRead.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    var handle = GetManagerHandle(systemContext, nodeToRead.NodeId, operationCache);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToRead.Processed = true;

                    // create an initial result.
                    var result = results[ii] = new HistoryReadResult();

                    result.HistoryData       = null;
                    result.ContinuationPoint = null;
                    result.StatusCode        = StatusCodes.Good;

                    // check if the node is a area in memory.
                    if (handle.Node == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a separate operation
                        handle.Index = ii;
                        nodesToProcess.Add(handle);

                        continue;
                    }

                    errors[ii] = StatusCodes.BadHistoryOperationUnsupported;

                    // check for data history variable.

                    if (handle.Node is BaseVariableState variable)
                    {
                        if ((variable.AccessLevel & AccessLevels.HistoryRead) != 0)
                        {
                            handle.Index = ii;
                            nodesToProcess.Add(handle);
                            continue;
                        }
                    }

                    // check for event history object.

                    if (handle.Node is BaseObjectState notifier)
                    {
                        if ((notifier.EventNotifier & EventNotifiers.HistoryRead) != 0)
                        {
                            handle.Index = ii;
                            nodesToProcess.Add(handle);
                        }
                    }
                }

                // check for nothing to do.
                if (nodesToProcess.Count == 0)
                {
                    return;
                }
            }

            // validates the nodes (reads values from the underlying data source if required).
            HistoryRead(
                systemContext,
                details,
                timestampsToReturn,
                releaseContinuationPoints,
                nodesToRead,
                results,
                errors,
                nodesToProcess,
                operationCache);
        }

        #region HistoryRead Support Functions
        /// <summary>
        /// Releases the continuation points.
        /// </summary>
        protected virtual void HistoryReleaseContinuationPoints(
            UaServerContext context,
            IList<HistoryReadValueId> nodesToRead,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
            }
        }

        /// <summary>
        /// Reads raw history data.
        /// </summary>
        protected virtual void HistoryReadRawModified(
            UaServerContext context,
            ReadRawModifiedDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Reads processed history data.
        /// </summary>
        protected virtual void HistoryReadProcessed(
            UaServerContext context,
            ReadProcessedDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Reads history data at specified times.
        /// </summary>
        protected virtual void HistoryReadAtTime(
            UaServerContext context,
            ReadAtTimeDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Reads history events.
        /// </summary>
        protected virtual void HistoryReadEvents(
            UaServerContext context,
            ReadEventDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Validates the nodes and reads the values from the underlying source.
        /// </summary>
        protected virtual void HistoryRead(
            UaServerContext context,
            HistoryReadDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            // check if continuation points are being released.
            if (releaseContinuationPoints)
            {
                HistoryReleaseContinuationPoints(
                    context,
                    nodesToRead,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // check timestamps to return.
            if (timestampsToReturn < TimestampsToReturn.Source || timestampsToReturn > TimestampsToReturn.Neither)
            {
                throw new ServiceResultException(StatusCodes.BadTimestampsToReturnInvalid);
            }

            // handle raw data request.

            if (details is ReadRawModifiedDetails readRawModifiedDetails)
            {
                // at least one must be provided.
                if (readRawModifiedDetails.StartTime == DateTime.MinValue && readRawModifiedDetails.EndTime == DateTime.MinValue)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidTimestampArgument);
                }

                // if one is null the num values must be provided.
                if (readRawModifiedDetails.StartTime == DateTime.MinValue || readRawModifiedDetails.EndTime == DateTime.MinValue)
                {
                    if (readRawModifiedDetails.NumValuesPerNode == 0)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidTimestampArgument);
                    }
                }

                HistoryReadRawModified(
                    context,
                    readRawModifiedDetails,
                    timestampsToReturn,
                    nodesToRead,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle processed data request.

            if (details is ReadProcessedDetails readProcessedDetails)
            {
                // check the list of aggregates.
                if (readProcessedDetails.AggregateType == null || readProcessedDetails.AggregateType.Count != nodesToRead.Count)
                {
                    throw new ServiceResultException(StatusCodes.BadAggregateListMismatch);
                }

                // check start/end time.
                if (readProcessedDetails.StartTime == DateTime.MinValue || readProcessedDetails.EndTime == DateTime.MinValue)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidTimestampArgument);
                }

                HistoryReadProcessed(
                    context,
                    readProcessedDetails,
                    timestampsToReturn,
                    nodesToRead,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle raw data at time request.

            if (details is ReadAtTimeDetails readAtTimeDetails)
            {
                HistoryReadAtTime(
                    context,
                    readAtTimeDetails,
                    timestampsToReturn,
                    nodesToRead,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle read events request.

            if (details is ReadEventDetails readEventDetails)
            {
                // check start/end time and max values.
                if (readEventDetails.NumValuesPerNode == 0)
                {
                    if (readEventDetails.StartTime == DateTime.MinValue || readEventDetails.EndTime == DateTime.MinValue)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidTimestampArgument);
                    }
                }
                else
                {
                    if (readEventDetails.StartTime == DateTime.MinValue && readEventDetails.EndTime == DateTime.MinValue)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidTimestampArgument);
                    }
                }

                // validate the event filter.
                var result = readEventDetails.Filter.Validate(new FilterContext(ServerData.NamespaceUris, ServerData.TypeTree, context));

                if (ServiceResult.IsBad(result.Status))
                {
                    throw new ServiceResultException(result.Status);
                }

                // read the event history.
                HistoryReadEvents(
                    context,
                    readEventDetails,
                    timestampsToReturn,
                    nodesToRead,
                    results,
                    errors,
                    nodesToProcess,
                    cache);
            }
        }
        #endregion

        /// <summary>
        /// Updates the history for the specified nodes.
        /// </summary>
        public virtual void HistoryUpdate(
            UaServerOperationContext            context,
            Type                        detailsType,
            IList<HistoryUpdateDetails> nodesToUpdate,
            IList<HistoryUpdateResult>  results,
            IList<ServiceResult>        errors)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            var nodesToProcess = new List<UaNodeHandle>();

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToUpdate.Count; ii++)
                {
                    var nodeToUpdate = nodesToUpdate[ii];

                    // skip items that have already been processed.
                    if (nodeToUpdate.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    var handle = GetManagerHandle(systemContext, nodeToUpdate.NodeId, operationCache);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToUpdate.Processed = true;

                    // create an initial result.
                    var result = results[ii] = new HistoryUpdateResult();
                    result.StatusCode = StatusCodes.Good;

                    // check if the node is a area in memory.
                    if (handle.Node == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a separate operation
                        handle.Index = ii;
                        nodesToProcess.Add(handle);
                        continue;
                    }

                    errors[ii] = StatusCodes.BadHistoryOperationUnsupported;

                    // check for data history variable.

                    if (handle.Node is BaseVariableState variable)
                    {
                        if ((variable.AccessLevel & AccessLevels.HistoryWrite) != 0)
                        {
                            handle.Index = ii;
                            nodesToProcess.Add(handle);
                            continue;
                        }
                    }

                    // check for event history object.

                    if (handle.Node is BaseObjectState notifier)
                    {
                        if ((notifier.EventNotifier & EventNotifiers.HistoryWrite) != 0)
                        {
                            handle.Index = ii;
                            nodesToProcess.Add(handle);
                        }
                    }
                }

                // check for nothing to do.
                if (nodesToProcess.Count == 0)
                {
                    return;
                }
            }

            // validates the nodes and updates.
            HistoryUpdate(
                systemContext,
                detailsType,
                nodesToUpdate,
                results,
                errors,
                nodesToProcess,
                operationCache);
        }

        #region HistoryUpdate Support Functions
        /// <summary>
        /// Validates the nodes and updates the history.
        /// </summary>
        protected virtual void HistoryUpdate(
            UaServerContext context,
            Type                           detailsType,
            IList<HistoryUpdateDetails>    nodesToUpdate,
            IList<HistoryUpdateResult>     results,
            IList<ServiceResult>           errors,
            List<UaNodeHandle>               nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            // handle update data request.
            if (detailsType == typeof(UpdateDataDetails))
            {
                var details = new UpdateDataDetails[nodesToUpdate.Count];

                for (var ii = 0; ii < details.Length; ii++)
                {
                    details[ii] = (UpdateDataDetails)nodesToUpdate[ii];
                }

                HistoryUpdateData(
                    context,
                    details,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle update structure data request.
            if (detailsType == typeof(UpdateStructureDataDetails))
            {
                var details = new UpdateStructureDataDetails[nodesToUpdate.Count];

                for (var ii = 0; ii < details.Length; ii++)
                {
                    details[ii] = (UpdateStructureDataDetails)nodesToUpdate[ii];
                }

                HistoryUpdateStructureData(
                    context,
                    details,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle update events request.
            if (detailsType == typeof(UpdateEventDetails))
            {
                var details = new UpdateEventDetails[nodesToUpdate.Count];

                for (var ii = 0; ii < details.Length; ii++)
                {
                    details[ii] = (UpdateEventDetails)nodesToUpdate[ii];
                }

                HistoryUpdateEvents(
                    context,
                    details,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle delete raw data request.
            if (detailsType == typeof(DeleteRawModifiedDetails))
            {
                var details = new DeleteRawModifiedDetails[nodesToUpdate.Count];

                for (var ii = 0; ii < details.Length; ii++)
                {
                    details[ii] = (DeleteRawModifiedDetails)nodesToUpdate[ii];
                }

                HistoryDeleteRawModified(
                    context,
                    details,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle delete at time request.
            if (detailsType == typeof(DeleteAtTimeDetails))
            {
                var details = new DeleteAtTimeDetails[nodesToUpdate.Count];

                for (var ii = 0; ii < details.Length; ii++)
                {
                    details[ii] = (DeleteAtTimeDetails)nodesToUpdate[ii];
                }

                HistoryDeleteAtTime(
                    context,
                    details,
                    results,
                    errors,
                    nodesToProcess,
                    cache);

                return;
            }

            // handle delete at time request.
            if (detailsType == typeof(DeleteEventDetails))
            {
                var details = new DeleteEventDetails[nodesToUpdate.Count];

                for (var ii = 0; ii < details.Length; ii++)
                {
                    details[ii] = (DeleteEventDetails)nodesToUpdate[ii];
                }

                HistoryDeleteEvents(
                    context,
                    details,
                    results,
                    errors,
                    nodesToProcess,
                    cache);
            }
        }

        /// <summary>
        /// Updates the data history for one or more nodes.
        /// </summary>
        protected virtual void HistoryUpdateData(
            UaServerContext context,
            IList<UpdateDataDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Updates the structured data history for one or more nodes.
        /// </summary>
        protected virtual void HistoryUpdateStructureData(
            UaServerContext context,
            IList<UpdateStructureDataDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Updates the event history for one or more nodes.
        /// </summary>
        protected virtual void HistoryUpdateEvents(
            UaServerContext context,
            IList<UpdateEventDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Deletes the data history for one or more nodes.
        /// </summary>
        protected virtual void HistoryDeleteRawModified(
            UaServerContext context,
            IList<DeleteRawModifiedDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Deletes the data history for one or more nodes.
        /// </summary>
        protected virtual void HistoryDeleteAtTime(
            UaServerContext context,
            IList<DeleteAtTimeDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }

        /// <summary>
        /// Deletes the event history for one or more nodes.
        /// </summary>
        protected virtual void HistoryDeleteEvents(
            UaServerContext context,
            IList<DeleteEventDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            foreach (var handle in nodesToProcess)
            {
                // validate node.
                var source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                errors[handle.Index] = StatusCodes.BadHistoryOperationUnsupported;
            }
        }
        #endregion

        /// <summary>
        /// Calls a method on the specified nodes.
        /// </summary>
        public virtual void Call(
            UaServerOperationContext context,
            IList<CallMethodRequest> methodsToCall,
            IList<CallMethodResult> results,
            IList<ServiceResult> errors)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();

            for (var ii = 0; ii < methodsToCall.Count; ii++)
            {
                var methodToCall = methodsToCall[ii];

                // skip items that have already been processed.
                if (methodToCall.Processed)
                {
                    continue;
                }

                MethodState method;

                lock (Lock)
                {
                    // check for valid handle.
                    var handle = GetManagerHandle(systemContext, methodToCall.ObjectId, operationCache);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    methodToCall.Processed = true;

                    // validate the source node.
                    var source = ValidateNode(systemContext, handle, operationCache);

                    if (source == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;
                        continue;
                    }

                    // find the method.
                    method = source.FindMethod(systemContext, methodToCall.MethodId);

                    if (method == null)
                    {
                        // check for loose coupling.
                        if (source.ReferenceExists(ReferenceTypeIds.HasComponent, false, methodToCall.MethodId))
                        {
                            method = (MethodState)FindPredefinedNode(methodToCall.MethodId, typeof(MethodState));
                        }

                        if (method == null)
                        {
                            errors[ii] = StatusCodes.BadMethodInvalid;
                            continue;
                        }
                    }

                    // validate the role permissions for method to be executed,
                    // it may be a diferent MethodState that does not have the MethodId specified in the method call
                    errors[ii] = ValidateRolePermissions(context,
                        method.NodeId,
                        PermissionType.Call);

                    if (ServiceResult.IsBad(errors[ii]))
                    {
                        continue;
                    }
                }

                // call the method.
                var result = results[ii] = new CallMethodResult();

                errors[ii] = Call(
                    systemContext,
                    methodToCall,
                    method,
                    result);
            }
        }

        /// <summary>
        /// Calls a method on an object.
        /// </summary>
        protected virtual ServiceResult Call(
            ISystemContext context,
            CallMethodRequest methodToCall,
            MethodState method,
            CallMethodResult result)
        {
            var systemContext = context as UaServerContext;
            var argumentErrors = new List<ServiceResult>();
            var outputArguments = new VariantCollection();

            var error = method.Call(
                context,
                methodToCall.ObjectId,
                methodToCall.InputArguments,
                argumentErrors,
                outputArguments);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // check for argument errors.
            var argumentsValid = true;

            foreach (var argumentError in argumentErrors)
            {
                if (argumentError != null)
                {
                    result.InputArgumentResults.Add(argumentError.StatusCode);

                    if (ServiceResult.IsBad(argumentError))
                    {
                        argumentsValid = false;
                    }
                }
                else
                {
                    result.InputArgumentResults.Add(StatusCodes.Good);
                }

                // only fill in diagnostic info if it is requested.
                if (systemContext?.OperationContext != null)
                {
                    if ((systemContext.OperationContext.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        if (ServiceResult.IsBad(argumentError))
                        {
                            argumentsValid = false;
                            result.InputArgumentDiagnosticInfos.Add(new DiagnosticInfo(argumentError, systemContext.OperationContext.DiagnosticsMask, false, systemContext.OperationContext.StringTable));
                        }
                        else
                        {
                            result.InputArgumentDiagnosticInfos.Add(null);
                        }
                    }
                }
            }

            // check for validation errors.
            if (!argumentsValid)
            {
                result.StatusCode = StatusCodes.BadInvalidArgument;
                return result.StatusCode;
            }

            // do not return diagnostics if there are no errors.
            result.InputArgumentDiagnosticInfos.Clear();

            // return output arguments.
            result.OutputArguments = outputArguments;

            return ServiceResult.Good;
        }


        /// <summary>
        /// Subscribes or unsubscribes to events produced by the specified source.
        /// </summary>
        /// <remarks>
        /// This method is called when a event subscription is created or deletes. The node manager
        /// must  start/stop reporting events for the specified object and all objects below it in
        /// the notifier hierarchy.
        /// </remarks>
        public virtual ServiceResult SubscribeToEvents(
            UaServerOperationContext    context,
            object              sourceId,
            uint                subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe)
        {
            var systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // check for valid handle.
                var handle = IsHandleInNamespace(sourceId);

                if (handle == null)
                {
                    return StatusCodes.BadNodeIdInvalid;
                }

                // check for valid node.
                var source = ValidateNode(systemContext, handle, null);

                if (source == null)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                // subscribe to events.
                return SubscribeToEvents(systemContext, source, monitoredItem, unsubscribe);
            }
        }

        /// <summary>
        /// Subscribes or unsubscribes to events produced by all event sources.
        /// </summary>
        /// <remarks>
        /// This method is called when a event subscription is created or deleted. The node
        /// manager must start/stop reporting events for all objects that it manages.
        /// </remarks>
        public virtual ServiceResult SubscribeToAllEvents(
            UaServerOperationContext    context,
            uint                subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe)
        {
            var systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // A client has subscribed to the Server object which means all events produced
                // by this manager must be reported. This is done by incrementing the monitoring
                // reference count for all root notifiers.
                if (RootNotifiers != null)
                {
                    foreach (var t in RootNotifiers)
                    {
                        SubscribeToEvents(systemContext, t, monitoredItem, unsubscribe);
                    }
                }

                return ServiceResult.Good;
            }
        }

        #region SubscribeToEvents Support Functions
        /// <summary>
        /// Adds a root notifier.
        /// </summary>
        /// <param name="notifier">The notifier.</param>
        /// <remarks>
        /// A root notifier is a notifier owned by the NodeManager that is not the target of a
        /// HasNotifier reference. These nodes need to be linked directly to the Server object.
        /// </remarks>
        public virtual void AddRootNotifier(NodeState notifier)
        {
            if (RootNotifiers == null)
            {
                RootNotifiers = new List<NodeState>();
            }

            var mustAdd = true;

            for (var ii = 0; ii < RootNotifiers.Count; ii++)
            {
                if (Object.ReferenceEquals(notifier, RootNotifiers[ii]))
                {
                    return;
                }

                if (RootNotifiers[ii].NodeId == notifier.NodeId)
                {
                    RootNotifiers[ii] = notifier;
                    mustAdd = false;
                    break;
                }
            }

            if (mustAdd)
            {
                RootNotifiers.Add(notifier);
            }

            // need to prevent recursion with the server object.
            if (notifier.NodeId != ObjectIds.Server)
            {
                notifier.OnReportEvent = OnReportEvent;

                if (!notifier.ReferenceExists(ReferenceTypeIds.HasNotifier, true, ObjectIds.Server))
                {
                    notifier.AddReference(ReferenceTypeIds.HasNotifier, true, ObjectIds.Server);
                }
            }

            // subscribe to existing events.
            if (ServerData.EventManager != null)
            {
                var monitoredItems = ServerData.EventManager.GetMonitoredItems();

                foreach (var t in monitoredItems)
                {
                    if (t.MonitoringAllEvents)
                    {
                        SubscribeToEvents(
                            SystemContext,
                            notifier,
                            t,
                            true);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a root notifier previously added with AddRootNotifier.
        /// </summary>
        /// <param name="notifier">The notifier.</param>
        public virtual void RemoveRootNotifier(NodeState notifier)
        {
            if (RootNotifiers != null)
            {
                for (var ii = 0; ii < RootNotifiers.Count; ii++)
                {
                    if (Object.ReferenceEquals(notifier, RootNotifiers[ii]))
                    {
                        notifier.OnReportEvent = null;
                        notifier.RemoveReference(ReferenceTypeIds.HasNotifier, true, ObjectIds.Server);
                        RootNotifiers.RemoveAt(ii);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reports an event for a root notifier.
        /// </summary>
        protected virtual void OnReportEvent(
            ISystemContext context,
            NodeState node,
            IFilterTarget e)
        {
            ServerData.ReportEvent(context, e);
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="source">The source.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="unsubscribe">if set to <c>true</c> [unsubscribe].</param>
        /// <returns>Any error code.</returns>
        protected virtual ServiceResult SubscribeToEvents(
            UaServerContext context,
            NodeState           source,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe)
        {
            UaMonitoredNode monitoredNode;

            // handle unsubscribe.
            if (unsubscribe)
            {
                // check for existing monitored node.
                if (!MonitoredNodes.TryGetValue(source.NodeId, out monitoredNode))
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                monitoredNode.Remove(monitoredItem);

                // check if node is no longer being monitored.
                if (!monitoredNode.HasMonitoredItems)
                {
                    MonitoredNodes.Remove(source.NodeId);
                }

                // update flag.
                source.SetAreEventsMonitored(context, !unsubscribe, true);

                // call subclass.
                OnSubscribeToEvents(context, monitoredNode, unsubscribe);

                // all done.
                return ServiceResult.Good;
            }

            // only objects or views can be subscribed to.
            var instance = source as BaseObjectState;

            if (instance == null || (instance.EventNotifier & EventNotifiers.SubscribeToEvents) == 0)
            {
                var view = source as ViewState;

                if (view == null || (view.EventNotifier & EventNotifiers.SubscribeToEvents) == 0)
                {
                    return StatusCodes.BadNotSupported;
                }
            }

            // check for existing monitored node.
            if (!MonitoredNodes.TryGetValue(source.NodeId, out monitoredNode))
            {
                MonitoredNodes[source.NodeId] = monitoredNode = new UaMonitoredNode(this, source);
            }

            if (monitoredNode.EventMonitoredItems != null)
            {
                // remove existing monitored items with the same Id prior to insertion in order to avoid duplicates
                // this is necessary since the SubscribeToEvents method is called also from ModifyMonitoredItemsForEvents
                monitoredNode.EventMonitoredItems.RemoveAll(e => e.Id == monitoredItem.Id);
            }

            // this links the node to specified monitored item and ensures all events
            // reported by the node are added to the monitored item's queue.
            monitoredNode.Add(monitoredItem);

            // This call recursively updates a reference count all nodes in the notifier
            // hierarchy below the area. Sources with a reference count of 0 do not have
            // any active subscriptions so they do not need to report events.
            source.SetAreEventsMonitored(context, !unsubscribe, true);

            // signal update.
            OnSubscribeToEvents(context, monitoredNode, unsubscribe);

            // all done.
            return ServiceResult.Good;
        }

        /// <summary>
        /// Called after subscribing/unsubscribing to events.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="monitoredNode">The monitored node.</param>
        /// <param name="unsubscribe">if set to <c>true</c> unsubscribing.</param>
        protected virtual void OnSubscribeToEvents(
            UaServerContext context,
            UaMonitoredNode monitoredNode,
            bool                unsubscribe)
        {
            // defined by the sub-class
        }
        #endregion

        /// <summary>
        /// Tells the node manager to refresh any conditions associated with the specified monitored items.
        /// </summary>
        /// <remarks>
        /// This method is called when the condition refresh method is called for a subscription.
        /// The node manager must create a refresh event for each condition monitored by the subscription.
        /// </remarks>
        public virtual ServiceResult ConditionRefresh(
            UaServerOperationContext context,
            IList<IUaEventMonitoredItem> monitoredItems)
        {
            var systemContext = SystemContext.Copy(context);

            foreach (var uaEventMonitoredItem in monitoredItems)
            {
                // the IUaEventMonitoredItem should always be MonitoredItems since they are created by the MasterNodeManager.
                var monitoredItem = uaEventMonitoredItem as UaMonitoredItem;

                if (monitoredItem == null)
                {
                    continue;
                }

                var events = new List<IFilterTarget>();
                var nodesToRefresh = new List<NodeState>();

                lock (Lock)
                {
                    // check for server subscription.
                    if (monitoredItem.NodeId == ObjectIds.Server)
                    {
                        if (RootNotifiers != null)
                        {
                            nodesToRefresh.AddRange(RootNotifiers);
                        }
                    }
                    else
                    {
                        // check for existing monitored node.

                        if (!MonitoredNodes.TryGetValue(monitoredItem.NodeId, out var monitoredNode))
                        {
                            continue;
                        }

                        // get the refresh events.
                        nodesToRefresh.Add(monitoredNode.Node);
                    }
                }

                // block and wait for the refresh.
                foreach (var nodeToRefresh in nodesToRefresh)
                {
                    nodeToRefresh.ConditionRefresh(systemContext, events, true);
                }

                // queue the events.
                foreach (var queuedEvent in events)
                {
                    // verify if the event can be received by the current monitored item
                    var result = ValidateEventRolePermissions(monitoredItem, queuedEvent);
                    if (ServiceResult.IsBad(result))
                    {
                        continue;
                    }
                    monitoredItem.QueueEvent(queuedEvent);
                }
            }

            // all done.
            return ServiceResult.Good;
        }

        /// <summary>
        /// Creates a new set of monitored items for a set of variables.
        /// </summary>
        /// <remarks>
        /// This method only handles data change subscriptions. Event subscriptions are created by the SDK.
        /// </remarks>
        public virtual void CreateMonitoredItems(
            UaServerOperationContext                  context,
            uint                              subscriptionId,
            double                            publishingInterval,
            TimestampsToReturn                timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterResults,
            IList<IUaMonitoredItem>             monitoredItems,
            ref long                          globalIdCounter)
        {
            var systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            var nodesToValidate = new List<UaNodeHandle>();
            var createdItems = new List<IUaMonitoredItem>();

            lock (Lock)
            {
                for (var ii = 0; ii < itemsToCreate.Count; ii++)
                {
                    var itemToCreate = itemsToCreate[ii];

                    // skip items that have already been processed.
                    if (itemToCreate.Processed)
                    {
                        continue;
                    }

                    var itemToMonitor = itemToCreate.ItemToMonitor;

                    // check for valid handle.
                    var handle = GetManagerHandle(systemContext, itemToMonitor.NodeId, operationCache);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    itemToCreate.Processed = true;

                    // must validate node in a separate operation.
                    errors[ii] = StatusCodes.BadNodeIdUnknown;

                    handle.Index = ii;
                    nodesToValidate.Add(handle);
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }
            }

            // validates the nodes (reads values from the underlying data source if required).
            foreach (var handle in nodesToValidate)
            {
                MonitoringFilterResult filterResult;
                IUaMonitoredItem monitoredItem;

                lock (Lock)
                {
                    // validate node.
                    var source = ValidateNode(systemContext, handle, operationCache);

                    if (source == null)
                    {
                        continue;
                    }

                    var itemToCreate = itemsToCreate[handle.Index];

                    // create monitored item.
                    errors[handle.Index] = CreateMonitoredItem(
                        systemContext,
                        handle,
                        subscriptionId,
                        publishingInterval,
                        context.DiagnosticsMask,
                        timestampsToReturn,
                        itemToCreate,
                        ref globalIdCounter,
                        out filterResult,
                        out monitoredItem);
                }

                // save any filter error details.
                filterResults[handle.Index] = filterResult;

                if (ServiceResult.IsBad(errors[handle.Index]))
                {
                    continue;
                }

                // save the monitored item.
                monitoredItems[handle.Index] = monitoredItem;
                createdItems.Add(monitoredItem);
            }

            // do any post processing.
            OnCreateMonitoredItemsComplete(systemContext, createdItems);
        }

        #region CreateMonitoredItem Support Functions
        /// <summary>
        /// Called when a batch of monitored items has been created.
        /// </summary>
        protected virtual void OnCreateMonitoredItemsComplete(UaServerContext context, IList<IUaMonitoredItem> monitoredItems)
        {
            // defined by the sub-class
        }

        /// <summary>
        /// Creates a new set of monitored items for a set of variables.
        /// </summary>
        /// <remarks>
        /// This method only handles data change subscriptions. Event subscriptions are created by the SDK.
        /// </remarks>
        protected virtual ServiceResult CreateMonitoredItem(
            UaServerContext context,
            UaNodeHandle handle,
            uint subscriptionId,
            double publishingInterval,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemCreateRequest itemToCreate,
            ref long globalIdCounter,
            out MonitoringFilterResult filterResult,
            out IUaMonitoredItem monitoredItem)
        {
            filterResult = null;
            monitoredItem = null;

            // validate parameters.
            var parameters = itemToCreate.RequestedParameters;

            // validate attribute.
            if (!Attributes.IsValid(handle.Node.NodeClass, itemToCreate.ItemToMonitor.AttributeId))
            {
                return StatusCodes.BadAttributeIdInvalid;
            }

            // check if the node is already being monitored.

            if (!MonitoredNodes.TryGetValue(handle.Node.NodeId, out var monitoredNode))
            {
                var cachedNode = AddNodeToComponentCache(context, handle, handle.Node);
                MonitoredNodes[handle.Node.NodeId] = monitoredNode = new UaMonitoredNode(this, cachedNode);
            }

            handle.Node = monitoredNode.Node;
            handle.MonitoredNode = monitoredNode;

            // create a globally unique identifier.
            var monitoredItemId = Utils.IncrementIdentifier(ref globalIdCounter);

            // determine the sampling interval.
            var samplingInterval = itemToCreate.RequestedParameters.SamplingInterval;

            if (samplingInterval < 0)
            {
                samplingInterval = publishingInterval;
            }

            // ensure minimum sampling interval is not exceeded.
            if (itemToCreate.ItemToMonitor.AttributeId == Attributes.Value)
            {
                if (handle.Node is BaseVariableState variable && samplingInterval < variable.MinimumSamplingInterval)
                {
                    samplingInterval = variable.MinimumSamplingInterval;
                }
            }

            // put a large upper limit on sampling.
            if (samplingInterval == double.MaxValue)
            {
                samplingInterval = 365 * 24 * 3600 * 1000.0;
            }

            // put an upper limit on queue size.
            var queueSize = itemToCreate.RequestedParameters.QueueSize;

            if (queueSize > MaxQueueSize)
            {
                queueSize = MaxQueueSize;
            }

            // validate the monitoring filter.
            ServiceResult error = ValidateMonitoringFilter(
                context,
                handle,
                itemToCreate.ItemToMonitor.AttributeId,
                samplingInterval,
                queueSize,
                parameters.Filter,
                out var filterToUse,
                out var euRange,
                out filterResult);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // create the item.
            var dataChangeItem = new UaMonitoredItem(
                ServerData,
                this,
                handle,
                subscriptionId,
                monitoredItemId,
                itemToCreate.ItemToMonitor,
                diagnosticsMasks,
                timestampsToReturn,
                itemToCreate.MonitoringMode,
                itemToCreate.RequestedParameters.ClientHandle,
                filterToUse,
                filterToUse,
                euRange,
                samplingInterval,
                queueSize,
                itemToCreate.RequestedParameters.DiscardOldest,
                0);

            // report the initial value.
            error = ReadInitialValue(context, handle, dataChangeItem);
            if (ServiceResult.IsBad(error))
            {
                if (error.StatusCode == StatusCodes.BadAttributeIdInvalid ||
                    error.StatusCode == StatusCodes.BadDataEncodingInvalid ||
                    error.StatusCode == StatusCodes.BadDataEncodingUnsupported)
                {
                    return error;
                }
                error = StatusCodes.Good;
            }

            // update monitored item list.
            monitoredItem = dataChangeItem;

            // save the monitored item.
            MonitoredItems.Add(monitoredItemId, dataChangeItem);
            monitoredNode.Add(dataChangeItem);

            // report change.
            OnMonitoredItemCreated(context, handle, dataChangeItem);

            return error;
        }

        /// <summary>
        /// Reads the initial value for a monitored item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The item handle.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected virtual ServiceResult ReadInitialValue(
            ISystemContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            var initialValue = new DataValue();

            initialValue.Value = null;
            initialValue.ServerTimestamp = DateTime.UtcNow;
            initialValue.SourceTimestamp = DateTime.MinValue;
            initialValue.StatusCode = StatusCodes.BadWaitingForInitialData;

            var error = handle.Node.ReadAttribute(
                context,
                monitoredItem.AttributeId,
                monitoredItem.IndexRange,
                monitoredItem.DataEncoding,
                initialValue);

            monitoredItem.QueueValue(initialValue, error, true);

            return error;
        }

        /// <summary>
        /// Called after creating a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected virtual void OnMonitoredItemCreated(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            // overridden by the sub-class.
        }

        /// <summary>
        /// Validates Role permissions for the specified NodeId
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="nodeId"></param>
        /// <param name="requestedPermission"></param>
        /// <returns></returns>
        public ServiceResult ValidateRolePermissions(UaServerOperationContext operationContext, NodeId nodeId, PermissionType requestedPermission)
        {
            if (operationContext.Session == null || requestedPermission == PermissionType.None)
            {
                // no permission is required hence the validation passes.
                return StatusCodes.Good;
            }

            IUaBaseNodeManager nodeManager = null;
            var nodeHandle = ServerData.NodeManager.GetManagerHandle(nodeId, out nodeManager);
            if (nodeHandle == null || nodeManager == null)
            {
                // ignore unknown nodes.
                return StatusCodes.Good;
            }

            var nodeMetadata = nodeManager.GetNodeMetadata(operationContext, nodeHandle, BrowseResultMask.All);

            return MasterNodeManager.ValidateRolePermissions(operationContext, nodeMetadata, requestedPermission);
        }

        /// <summary>
        /// Validates if the specified event monitored item has enough permissions to receive the specified event
        /// </summary>
        /// <returns></returns>
        public ServiceResult ValidateEventRolePermissions(IUaEventMonitoredItem monitoredItem, IFilterTarget filterTarget)
        {
            NodeId eventTypeId = null;
            NodeId sourceNodeId = null;
            BaseEventState baseEventState = filterTarget as BaseEventState;

            if (baseEventState == null && filterTarget is InstanceStateSnapshot snapshot)
            {
                // try to get the event instance from snapshot object
                baseEventState = snapshot.Handle as BaseEventState;
            }

            if (baseEventState != null)
            {
                eventTypeId = baseEventState.EventType?.Value;
                sourceNodeId = baseEventState.SourceNode?.Value;
            }
            
            UaServerOperationContext operationContext = new UaServerOperationContext(monitoredItem);

            // validate the event type id permissions as specified
            ServiceResult result = ValidateRolePermissions(operationContext, eventTypeId, PermissionType.ReceiveEvents);

            if (ServiceResult.IsBad(result))
            {
                return result;
            }

            // validate the source node id permissions as specified
            return ValidateRolePermissions(operationContext, sourceNodeId, PermissionType.ReceiveEvents);
        }

        /// <summary>
        /// Validates the monitoring filter specified by the client.
        /// </summary>
        protected virtual StatusCode ValidateMonitoringFilter(
            UaServerContext context,
            UaNodeHandle handle,
            uint attributeId,
            double samplingInterval,
            uint queueSize,
            ExtensionObject filter,
            out MonitoringFilter filterToUse,
            out Opc.Ua.Range range,
            out MonitoringFilterResult result)
        {
            range = null;
            filterToUse = null;
            result = null;

            // nothing to do if the filter is not specified.
            if (ExtensionObject.IsNull(filter))
            {
                return StatusCodes.Good;
            }

            // extension objects wrap any data structure. must check that the client provided the correct structure.
            var deadbandFilter = ExtensionObject.ToEncodeable(filter) as DataChangeFilter;

            if (deadbandFilter == null)
            {
                var aggregateFilter = ExtensionObject.ToEncodeable(filter) as AggregateFilter;

                if (aggregateFilter == null || attributeId != Attributes.Value)
                {
                    return StatusCodes.BadFilterNotAllowed;
                }

                if (!ServerData.AggregateManager.IsSupported(aggregateFilter.AggregateType))
                {
                    return StatusCodes.BadAggregateNotSupported;
                }

                var revisedFilter = new ServerAggregateFilter
                {
                    AggregateType = aggregateFilter.AggregateType,
                    StartTime = aggregateFilter.StartTime,
                    ProcessingInterval = aggregateFilter.ProcessingInterval,
                    AggregateConfiguration = aggregateFilter.AggregateConfiguration,
                    Stepped = false
                };

                var error = ReviseAggregateFilter(context, handle, samplingInterval, queueSize, revisedFilter);

                if (StatusCode.IsBad(error))
                {
                    return error;
                }

                var aggregateFilterResult = new AggregateFilterResult
                {
                    RevisedProcessingInterval = aggregateFilter.ProcessingInterval,
                    RevisedStartTime = aggregateFilter.StartTime,
                    RevisedAggregateConfiguration = aggregateFilter.AggregateConfiguration
                };

                filterToUse = revisedFilter;
                result = aggregateFilterResult;
                return StatusCodes.Good;
            }

            // deadband filters only allowed for variable values.
            if (attributeId != Attributes.Value)
            {
                return StatusCodes.BadFilterNotAllowed;
            }

            var variable = handle.Node as BaseVariableState;

            if (variable == null)
            {
                return StatusCodes.BadFilterNotAllowed;
            }

            // check for status filter.
            if (deadbandFilter.DeadbandType == (uint)DeadbandType.None)
            {
                filterToUse = deadbandFilter;
                return StatusCodes.Good;
            }

            // deadband filters can only be used for numeric values.
            if (!ServerData.TypeTree.IsTypeOf(variable.DataType, DataTypeIds.Number))
            {
                return StatusCodes.BadFilterNotAllowed;
            }

            // nothing more to do for absolute filters.
            if (deadbandFilter.DeadbandType == (uint)DeadbandType.Absolute)
            {
                filterToUse = deadbandFilter;
                return StatusCodes.Good;
            }

            // need to look up the EU range if a percent filter is requested.
            if (deadbandFilter.DeadbandType == (uint)DeadbandType.Percent)
            {
                if (!(handle.Node.FindChild(context, BrowseNames.EURange) is PropertyState property))
                {
                    return StatusCodes.BadMonitoredItemFilterUnsupported;
                }

                range = property.Value as Opc.Ua.Range;

                if (range == null)
                {
                    return StatusCodes.BadMonitoredItemFilterUnsupported;
                }

                filterToUse = deadbandFilter;

                return StatusCodes.Good;
            }

            // no other type of filter supported.
            return StatusCodes.BadFilterNotAllowed;
        }

        /// <summary>
        /// Revises an aggregate filter (may require knowledge of the variable being used).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="samplingInterval">The sampling interval for the monitored item.</param>
        /// <param name="queueSize">The queue size for the monitored item.</param>
        /// <param name="filterToUse">The filter to revise.</param>
        /// <returns>Good if the </returns>
        protected virtual StatusCode ReviseAggregateFilter(
            UaServerContext context,
            UaNodeHandle handle,
            double samplingInterval,
            uint queueSize,
            ServerAggregateFilter filterToUse)
        {
            if (filterToUse.ProcessingInterval < samplingInterval)
            {
                filterToUse.ProcessingInterval = samplingInterval;
            }

            if (filterToUse.ProcessingInterval < ServerData.AggregateManager.MinimumProcessingInterval)
            {
                filterToUse.ProcessingInterval = ServerData.AggregateManager.MinimumProcessingInterval;
            }

            var earliestStartTime = DateTime.UtcNow.AddMilliseconds(-(queueSize - 1) * filterToUse.ProcessingInterval);

            if (earliestStartTime > filterToUse.StartTime)
            {
                filterToUse.StartTime = earliestStartTime;
            }

            if (filterToUse.AggregateConfiguration.UseServerCapabilitiesDefaults)
            {
                filterToUse.AggregateConfiguration = ServerData.AggregateManager.GetDefaultConfiguration(null);
            }

            return StatusCodes.Good;
        }
        #endregion

        /// <summary>
        /// Modifies the parameters for a set of monitored items.
        /// </summary>
        public virtual void ModifyMonitoredItems(
            UaServerOperationContext context,
            TimestampsToReturn                timestampsToReturn,
            IList<IUaMonitoredItem>             monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterResults)
        {
            var systemContext = SystemContext.Copy(context);
            var modifiedItems = new List<IUaMonitoredItem>();

            lock (Lock)
            {
                for (var ii = 0; ii < monitoredItems.Count; ii++)
                {
                    var itemToModify = itemsToModify[ii];

                    // skip items that have already been processed.
                    if (itemToModify.Processed || monitoredItems[ii] == null)
                    {
                        continue;
                    }

                    // check handle.
                    var handle = IsHandleInNamespace(monitoredItems[ii].ManagerHandle);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    itemToModify.Processed = true;

                    // modify the monitored item.

                    errors[ii] = ModifyMonitoredItem(
                        systemContext,
                        context.DiagnosticsMask,
                        timestampsToReturn,
                        monitoredItems[ii],
                        itemToModify,
                        handle,
                        out var filterResult);

                    // save any filter error details.
                    filterResults[ii] = filterResult;

                    // save the modified item.
                    if (ServiceResult.IsGood(errors[ii]))
                    {
                        modifiedItems.Add(monitoredItems[ii]);
                    }
                }
            }

            // do any post processing.
            OnModifyMonitoredItemsComplete(systemContext, modifiedItems);
        }

        #region ModifyMonitoredItem Support Functions
        /// <summary>
        /// Called when a batch of monitored items has been modified.
        /// </summary>
        protected virtual void OnModifyMonitoredItemsComplete(UaServerContext context, IList<IUaMonitoredItem> monitoredItems)
        {
            // defined by the sub-class
        }

        /// <summary>
        /// Modifies the parameters for a monitored item.
        /// </summary>
        protected virtual ServiceResult ModifyMonitoredItem(
            UaServerContext context,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            IUaMonitoredItem monitoredItem,
            MonitoredItemModifyRequest itemToModify,
            UaNodeHandle handle,
            out MonitoringFilterResult filterResult)
        {
            filterResult = null;

            // check for valid monitored item.
            var datachangeItem = monitoredItem as UaMonitoredItem;

            // validate parameters.
            var parameters = itemToModify.RequestedParameters;

            var previousSamplingInterval = datachangeItem.SamplingInterval;

            // check if the variable needs to be sampled.
            var samplingInterval = itemToModify.RequestedParameters.SamplingInterval;

            if (samplingInterval < 0)
            {
                samplingInterval = previousSamplingInterval;
            }

            // ensure minimum sampling interval is not exceeded.
            if (datachangeItem.AttributeId == Attributes.Value)
            {
                var variable = handle.Node as BaseVariableState;

                if (variable != null && samplingInterval < variable.MinimumSamplingInterval)
                {
                    samplingInterval = variable.MinimumSamplingInterval;
                }
            }

            // put a large upper limit on sampling.
            if (samplingInterval == double.MaxValue)
            {
                samplingInterval = 365 * 24 * 3600 * 1000.0;
            }

            // put an upper limit on queue size.
            var queueSize = itemToModify.RequestedParameters.QueueSize;

            if (queueSize > MaxQueueSize)
            {
                queueSize = MaxQueueSize;
            }

            // validate the monitoring filter.

            ServiceResult error = ValidateMonitoringFilter(
                context,
                handle,
                datachangeItem.AttributeId,
                samplingInterval,
                queueSize,
                parameters.Filter,
                out var filterToUse,
                out var euRange,
                out filterResult);

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // modify the monitored item parameters.
            error = datachangeItem.ModifyAttributes(
                diagnosticsMasks,
                timestampsToReturn,
                itemToModify.RequestedParameters.ClientHandle,
                filterToUse,
                filterToUse,
                euRange,
                samplingInterval,
                queueSize,
                itemToModify.RequestedParameters.DiscardOldest);

            // report change.
            if (ServiceResult.IsGood(error))
            {
                OnMonitoredItemModified(context, handle, datachangeItem);
            }

            return error;
        }

        /// <summary>
        /// Called after modifying a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected virtual void OnMonitoredItemModified(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            // overridden by the sub-class.
        }
        #endregion

        /// <summary>
        /// Deletes a set of monitored items.
        /// </summary>
        public virtual void DeleteMonitoredItems(
            UaServerOperationContext     context,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool>          processedItems,
            IList<ServiceResult> errors)
        {
            var systemContext = SystemContext.Copy(context);
            var deletedItems = new List<IUaMonitoredItem>();

            lock (Lock)
            {
                for (var ii = 0; ii < monitoredItems.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii] || monitoredItems[ii] == null)
                    {
                        continue;
                    }

                    // check handle.
                    var handle = IsHandleInNamespace(monitoredItems[ii].ManagerHandle);

                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    processedItems[ii] = true;

                    errors[ii] = DeleteMonitoredItem(
                        systemContext,
                        monitoredItems[ii],
                        handle);

                    // save the modified item.
                    if (ServiceResult.IsGood(errors[ii]))
                    {
                        deletedItems.Add(monitoredItems[ii]);
                        RemoveNodeFromComponentCache(systemContext, handle);
                    }
                }
            }

            // do any post processing.
            OnDeleteMonitoredItemsComplete(systemContext, deletedItems);
        }

        #region DeleteMonitoredItems Support Functions
        /// <summary>
        /// Called when a batch of monitored items has been modified.
        /// </summary>
        protected virtual void OnDeleteMonitoredItemsComplete(UaServerContext context, IList<IUaMonitoredItem> monitoredItems)
        {
            // defined by the sub-class
        }

        /// <summary>
        /// Deletes a monitored item.
        /// </summary>
        protected virtual ServiceResult DeleteMonitoredItem(
            UaServerContext context,
            IUaMonitoredItem monitoredItem,
            UaNodeHandle handle)
        {
            // check for valid monitored item.
            var datachangeItem = monitoredItem as UaMonitoredItem;

            // check if the node is already being monitored.

            if (MonitoredNodes.TryGetValue(handle.NodeId, out var monitoredNode))
            {
                monitoredNode.Remove(datachangeItem);

                // check if node is no longer being monitored.
                if (!monitoredNode.HasMonitoredItems)
                {
                    MonitoredNodes.Remove(handle.NodeId);
                }
            }

            // remove the monitored item.
            MonitoredItems.Remove(monitoredItem.Id);

            // report change.
            OnMonitoredItemDeleted(context, handle, datachangeItem);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Called after deleting a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected virtual void OnMonitoredItemDeleted(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            // overridden by the sub-class.
        }
        #endregion

        #region TransferMonitoredItems Support Functions
        /// <summary>
        /// Transfers a set of monitored items.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sendInitialValues">Whether the subscription should send initial values after transfer.</param>
        /// <param name="monitoredItems">The set of monitoring items to update.</param>
        /// <param name="processedItems">The list of bool with items that were already processed.</param>
        /// <param name="errors">Any errors.</param>
        public virtual void TransferMonitoredItems(
            UaServerOperationContext context,
            bool sendInitialValues,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IList<IUaMonitoredItem> transferredItems = new List<IUaMonitoredItem>();
            lock (Lock)
            {
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii] || monitoredItems[ii] == null)
                    {
                        continue;
                    }

                    // check handle.
                    UaNodeHandle handle = IsHandleInNamespace(monitoredItems[ii].ManagerHandle);
                    if (handle == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    processedItems[ii] = true;
                    transferredItems.Add(monitoredItems[ii]);
                    if (sendInitialValues)
                    {
                        monitoredItems[ii].SetupResendDataTrigger();
                    }
                    errors[ii] = StatusCodes.Good;
                }
            }

            // do any post processing.
            OnMonitoredItemsTransferred(systemContext, transferredItems);
        }

        /// <summary>
        /// Called after transfer of MonitoredItems.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="monitoredItems">The transferred monitored items.</param>
        protected virtual void OnMonitoredItemsTransferred(
            UaServerContext context,
            IList<IUaMonitoredItem> monitoredItems
            )
        {
            // defined by the sub-class
        }
        #endregion

        /// <summary>
        /// Changes the monitoring mode for a set of monitored items.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="monitoringMode">The monitoring mode.</param>
        /// <param name="monitoredItems">The set of monitoring items to update.</param>
        /// <param name="processedItems">Flags indicating which items have been processed.</param>
        /// <param name="errors">Any errors.</param>
        public virtual void SetMonitoringMode(
            UaServerOperationContext context,
            MonitoringMode        monitoringMode,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool>           processedItems,
            IList<ServiceResult>  errors)
        {
            var systemContext = SystemContext.Copy(context);
            var changedItems = new List<IUaMonitoredItem>();

            lock (Lock)
            {
                for (var ii = 0; ii < monitoredItems.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii] || monitoredItems[ii] == null)
                    {
                        continue;
                    }

                    // check handle.
                    var handle = IsHandleInNamespace(monitoredItems[ii].ManagerHandle);

                    if (handle == null)
                    {
                        continue;
                    }

                    // indicate whether it was processed or not.
                    processedItems[ii] = true;

                    // update monitoring mode.
                    errors[ii] = SetMonitoringMode(
                        systemContext,
                        monitoredItems[ii],
                        monitoringMode,
                        handle);

                    // save the modified item.
                    if (ServiceResult.IsGood(errors[ii]))
                    {
                        changedItems.Add(monitoredItems[ii]);
                    }
                }
            }

            // do any post processing.
            OnSetMonitoringModeComplete(systemContext, changedItems);
        }

        #region SetMonitoringMode Support Functions
        /// <summary>
        /// Called when a batch of monitored items has their monitoring mode changed.
        /// </summary>
        protected virtual void OnSetMonitoringModeComplete(UaServerContext context, IList<IUaMonitoredItem> monitoredItems)
        {
            // defined by the sub-class
        }

        /// <summary>
        /// Changes the monitoring mode for an item.
        /// </summary>
        protected virtual ServiceResult SetMonitoringMode(
            UaServerContext context,
            IUaMonitoredItem monitoredItem,
            MonitoringMode monitoringMode,
            UaNodeHandle handle)
        {
            // check for valid monitored item.
            var datachangeItem = monitoredItem as UaMonitoredItem;

            // update monitoring mode.
            var previousMode = datachangeItem.SetMonitoringMode(monitoringMode);

            // must send the latest value after enabling a disabled item.
            if (monitoringMode == MonitoringMode.Reporting && previousMode == MonitoringMode.Disabled)
            {
                handle.MonitoredNode.QueueValue(context, handle.Node, datachangeItem);
            }

            // report change.
            if (previousMode != monitoringMode)
            {
                OnMonitoringModeChanged(
                    context,
                    handle,
                    datachangeItem,
                    previousMode,
                    monitoringMode);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Called after changing the MonitoringMode for a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="previousMode">The previous monitoring mode.</param>
        /// <param name="monitoringMode">The current monitoring mode.</param>
        protected virtual void OnMonitoringModeChanged(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem,
            MonitoringMode previousMode,
            MonitoringMode monitoringMode)
        {
            // overridden by the sub-class.
        }
        #endregion
        #endregion

        #region IUaNodeManager2 Members
        /// <summary>
        /// Called when a session is closed.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="deleteSubscriptions">if set to <c>true</c> subscriptions are to be deleted.</param>
        public virtual void SessionClosing(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions)
        {
        }

        /// <summary>
        /// Returns true if a node is in a view.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="viewId">The view identifier.</param>
        /// <param name="nodeHandle">The node to check.</param>
        public virtual bool IsNodeInView(UaServerOperationContext context, NodeId viewId, object nodeHandle)
        {
            var handle = nodeHandle as UaNodeHandle;

            if (handle?.Node != null)
            {
                return IsNodeInView(context, viewId, handle.Node);
            }

            return false;
        }

        /// <summary>
        /// Returns the metadata containing the AccessRestrictions, RolePermissions and UserRolePermissions for the node.
        /// Returns null if the node does not exist.
        /// </summary>
        /// <remarks>
        /// This method validates any placeholder handle.
        /// </remarks>
        public virtual UaNodeMetadata GetPermissionMetadata(
            UaServerOperationContext context,
            object targetHandle,
            BrowseResultMask resultMask,
            Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes,
            bool permissionsOnly)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // check for valid handle.
                UaNodeHandle handle = IsHandleInNamespace(targetHandle);

                if (handle == null)
                {
                    return null;
                }

                // validate node.
                NodeState target = ValidateNode(systemContext, handle, null);

                if (target == null)
                {
                    return null;
                }

                List<object> values = null;

                // construct the meta-data object.
                UaNodeMetadata metadata = new UaNodeMetadata(target, target.NodeId);

                // Treat the case of calls originating from the optimized services that use the cache (Read, Browse and Call services)
                if (uniqueNodesServiceAttributes != null)
                {
                    NodeId key = handle.NodeId;
                    if (uniqueNodesServiceAttributes.ContainsKey(key))
                    {
                        if (uniqueNodesServiceAttributes[key].Count == 0)
                        {
                            values = ReadAndCacheValidationAttributes(uniqueNodesServiceAttributes, systemContext, target, key);
                        }
                        else
                        {
                            // Retrieve value from cache
                            values = uniqueNodesServiceAttributes[key];
                        }
                    }
                    else
                    {
                        values = ReadAndCacheValidationAttributes(uniqueNodesServiceAttributes, systemContext, target, key);
                    }

                    SetAccessAndRolePermissions(values, metadata);
                }// All other calls that do not use the cache
                else if (permissionsOnly == true)
                {
                    values = ReadValidationAttributes(systemContext, target);
                    SetAccessAndRolePermissions(values, metadata);
                }

                SetDefaultPermissions(systemContext, target, metadata);

                return metadata;
            }
        }

        /// <summary>
        /// Set the metadata default permission values for DefaultAccessRestrictions, DefaultRolePermissions and DefaultUserRolePermissions
        /// </summary>
        /// <param name="systemContext"></param>
        /// <param name="target"></param>
        /// <param name="metadata"></param>
        private void SetDefaultPermissions(UaServerContext systemContext, NodeState target, UaNodeMetadata metadata)
        {
            // check if NamespaceMetadata is defined for NamespaceUri
            string namespaceUri = ServerData.NamespaceUris.GetString(target.NodeId.NamespaceIndex);
            NamespaceMetadataState namespaceMetadataState = ServerData.NodeManager.ConfigurationNodeManager.GetNamespaceMetadataState(namespaceUri);

            if (namespaceMetadataState != null)
            {
                List<object> namespaceMetadataValues;

                if (namespaceMetadataState.DefaultAccessRestrictions != null)
                {
                    // get DefaultAccessRestrictions for Namespace
                    namespaceMetadataValues = namespaceMetadataState.DefaultAccessRestrictions.ReadAttributes(systemContext, Attributes.Value);

                    if (namespaceMetadataValues[0] != null)
                    {
                        metadata.DefaultAccessRestrictions = (AccessRestrictionType)Enum.ToObject(typeof(AccessRestrictionType), namespaceMetadataValues[0]);
                    }
                }

                if (namespaceMetadataState.DefaultRolePermissions != null)
                {
                    // get DefaultRolePermissions for Namespace
                    namespaceMetadataValues = namespaceMetadataState.DefaultRolePermissions.ReadAttributes(systemContext, Attributes.Value);

                    if (namespaceMetadataValues[0] != null)
                    {
                        metadata.DefaultRolePermissions = new RolePermissionTypeCollection(ExtensionObject.ToList<RolePermissionType>(namespaceMetadataValues[0]));
                    }
                }

                if (namespaceMetadataState.DefaultUserRolePermissions != null)
                {
                    // get DefaultUserRolePermissions for Namespace
                    namespaceMetadataValues = namespaceMetadataState.DefaultUserRolePermissions.ReadAttributes(systemContext, Attributes.Value);

                    if (namespaceMetadataValues[0] != null)
                    {
                        metadata.DefaultUserRolePermissions = new RolePermissionTypeCollection(ExtensionObject.ToList<RolePermissionType>(namespaceMetadataValues[0]));
                    }
                }
            }
        }
        #endregion

        #region ComponentCache Functions
        /// <summary>
        /// Stores a reference count for entries in the component cache.
        /// </summary>
        private class CacheEntry
        {
            public int RefCount;
            public NodeState Entry;
        }

        /// <summary>
        /// Looks up a component in cache.
        /// </summary>
        protected NodeState LookupNodeInComponentCache(ISystemContext context, UaNodeHandle handle)
        {
            lock (Lock)
            {
                if (componentCache_ == null)
                {
                    return null;
                }

                CacheEntry entry;

                if (!string.IsNullOrEmpty(handle.ComponentPath))
                {
                    if (componentCache_.TryGetValue(handle.RootId, out entry))
                    {
                        return entry.Entry.FindChildBySymbolicName(context, handle.ComponentPath);
                    }
                }
                else
                {
                    if (componentCache_.TryGetValue(handle.NodeId, out entry))
                    {
                        return entry.Entry;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Removes a reference to a component in the cache.
        /// </summary>
        protected void RemoveNodeFromComponentCache(ISystemContext context, UaNodeHandle handle)
        {
            lock (Lock)
            {
                if (handle == null)
                {
                    return;
                }

                if (componentCache_ != null)
                {
                    var nodeId = handle.NodeId;

                    if (!string.IsNullOrEmpty(handle.ComponentPath))
                    {
                        nodeId = handle.RootId;
                    }

                    if (componentCache_.TryGetValue(nodeId, out var entry))
                    {
                        entry.RefCount--;

                        if (entry.RefCount == 0)
                        {
                            componentCache_.Remove(nodeId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a node to the component cache.
        /// </summary>
        protected NodeState AddNodeToComponentCache(ISystemContext context, UaNodeHandle handle, NodeState node)
        {
            lock (Lock)
            {
                if (handle == null)
                {
                    return node;
                }

                if (componentCache_ == null)
                {
                    componentCache_ = new Dictionary<NodeId, CacheEntry>();
                }

                // check if a component is actually specified.
                if (!string.IsNullOrEmpty(handle.ComponentPath))
                {
                    if (componentCache_.TryGetValue(handle.RootId, out var entry))
                    {
                        entry.RefCount++;

                        if (!string.IsNullOrEmpty(handle.ComponentPath))
                        {
                            return entry.Entry.FindChildBySymbolicName(context, handle.ComponentPath);
                        }

                        return entry.Entry;
                    }

                    var root = node.GetHierarchyRoot();

                    if (root != null)
                    {
                        entry = new CacheEntry { RefCount = 1, Entry = root };
                        componentCache_.Add(handle.RootId, entry);
                    }
                }

                // simply add the node to the cache.
                else
                {
                    if (componentCache_.TryGetValue(handle.NodeId, out var entry))
                    {
                        entry.RefCount++;
                        return entry.Entry;
                    }

                    entry = new CacheEntry { RefCount = 1, Entry = node };
                    componentCache_.Add(handle.NodeId, entry);
                }

                return node;
            }
        }
        #endregion

        #region Private Fields
        private string[] namespaceUris_;
        private Dictionary<NodeId, CacheEntry> componentCache_;
        #endregion
    }
}
