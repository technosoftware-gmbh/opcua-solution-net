#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Linq;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace Technosoftware.UaSample
{
    /// <summary>
    /// A node manager for a variety of test data.
    /// </summary>
    public class SampleNodeManager : IUaBaseNodeManager, INodeIdFactory, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public SampleNodeManager(IUaServerData server)
        {
            // save a reference to the server that owns the node manager.
            ServerData = server;

            // create the default context.
            SystemContext = ServerData.DefaultSystemContext.Copy();

            SystemContext.SystemHandle = null;
            SystemContext.NodeIdFactory = this;

            // create the table of nodes. 
            PredefinedNodes = new NodeIdDictionary<NodeState>();
            RootNotifiers = new List<NodeState>();
            sampledItems_ = new List<DataChangeMonitoredItem>();
            minimumSamplingInterval_ = 100;
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
                    Utils.SilentDispose(samplingTimer_);
                    samplingTimer_ = null;

                    foreach (NodeState node in PredefinedNodes.Values)
                    {
                        Utils.SilentDispose(node);
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
        protected virtual NodeState IsHandleInNamespace(object managerHandle)
        {
            NodeState source = managerHandle as NodeState;

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
                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
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
            UaServerContext contextToUse = (UaServerContext)SystemContext.Copy(context);

            lock (Lock)
            {
                instance.ReferenceTypeId = referenceTypeId;

                NodeState parent = null;

                if (parentId != null)
                {
                    if (!PredefinedNodes.TryGetValue(parentId, out parent))
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
            UaServerContext contextToUse = SystemContext.Copy(context);

            bool found = false;
            List<Technosoftware.UaServer.NodeManager.LocalReference> referencesToRemove = new List<Technosoftware.UaServer.NodeManager.LocalReference>();

            lock (Lock)
            {
                NodeState node = null;

                if (PredefinedNodes.TryGetValue(nodeId, out node))
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
        /// Adds all encodeable types defined in a node manager to the server factory.
        /// </summary>
        /// <param name="assembly">The assembly which contains the encodeable types.</param>
        /// <param name="filter">A filter with which the FullName of the type must start.</param>
        protected void AddEncodeableNodeManagerTypes(Assembly assembly, string filter)
        {
            ServerData.Factory.AddEncodeableTypes(assembly.GetExportedTypes().Where(t => t.FullName.StartsWith(filter)));
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
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public virtual void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                // add the uris to the server's namespace table and cache the indexes.
                for (int ii = 0; ii < namespaceUris_.Length; ii++)
                {
                    NamespaceIndexes[ii] = ServerData.NamespaceUris.GetIndexOrAppend(namespaceUris_[ii]);
                }

                LoadPredefinedNodes(SystemContext, externalReferences);
            }
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
            // load the predefined nodes from an XML document.
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromResource(context, resourcePath, assembly, true);

            // add the predefined nodes to the node manager.
            for (int ii = 0; ii < predefinedNodes.Count; ii++)
            {
                AddPredefinedNode(context, predefinedNodes[ii]);
            }

            // ensure the reverse refernces exist.
            AddReverseReferences(externalReferences);
        }

        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected virtual NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            return new NodeStateCollection();
        }

        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected virtual void LoadPredefinedNodes(
            ISystemContext context,
            IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            // load the predefined nodes from an XML document.
            NodeStateCollection predefinedNodes = LoadPredefinedNodes(context);

            // add the predefined nodes to the node manager.
            for (int ii = 0; ii < predefinedNodes.Count; ii++)
            {
                AddPredefinedNode(context, predefinedNodes[ii]);
            }

            // ensure the reverse refernces exist.
            AddReverseReferences(externalReferences);
        }

        /// <summary>
        /// Replaces the generic node with a node specific to the model.
        /// </summary>
        protected virtual NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
        {
            BaseObjectState passiveNode = predefinedNode as BaseObjectState;

            if (passiveNode == null)
            {
                return predefinedNode;
            }

            return predefinedNode;
        }

        /// <summary>
        /// Recursively indexes the node and its children.
        /// </summary>
        protected virtual void AddPredefinedNode(ISystemContext context, NodeState node)
        {
            NodeState activeNode = AddBehaviourToPredefinedNode(context, node);
            PredefinedNodes[activeNode.NodeId] = activeNode;

            BaseTypeState type = activeNode as BaseTypeState;

            if (type != null)
            {
                AddTypesToTypeTree(type);
            }

            List<BaseInstanceState> children = new List<BaseInstanceState>();
            activeNode.GetChildren(context, children);

            for (int ii = 0; ii < children.Count; ii++)
            {
                AddPredefinedNode(context, children[ii]);
            }
        }

        /// <summary>
        /// Recursively indexes the node and its children.
        /// </summary>
        protected virtual void RemovePredefinedNode(
            ISystemContext context,
            NodeState node,
            List<Technosoftware.UaServer.NodeManager.LocalReference> referencesToRemove)
        {
            PredefinedNodes.Remove(node.NodeId);
            node.UpdateChangeMasks(NodeStateChangeMasks.Deleted);
            node.ClearChangeMasks(context, false);
            OnNodeRemoved(node);

            // remove from the parent.
            BaseInstanceState instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                instance.Parent.RemoveChild(instance);
            }

            // remove children.
            List<BaseInstanceState> children = new List<BaseInstanceState>();
            node.GetChildren(context, children);

            for (int ii = 0; ii < children.Count; ii++)
            {
                node.RemoveChild(children[ii]);
            }

            for (int ii = 0; ii < children.Count; ii++)
            {
                RemovePredefinedNode(context, children[ii], referencesToRemove);
            }

            // remove from type table.
            BaseTypeState type = node as BaseTypeState;

            if (type != null)
            {
                ServerData.TypeTree.Remove(type.NodeId);
            }

            // remove inverse references.
            List<IReference> references = new List<IReference>();
            node.GetReferences(context, references);

            for (int ii = 0; ii < references.Count; ii++)
            {
                IReference reference = references[ii];

                if (reference.TargetId.IsAbsolute)
                {
                    continue;
                }

                Technosoftware.UaServer.NodeManager.LocalReference referenceToRemove = new Technosoftware.UaServer.NodeManager.LocalReference(
                    (NodeId)reference.TargetId,
                    reference.ReferenceTypeId,
                    reference.IsInverse,
                    node.NodeId);

                referencesToRemove.Add(referenceToRemove);
            }
        }

        /// <summary>
        /// Called after a node has been deleted.
        /// </summary>
        protected virtual void OnNodeRemoved(NodeState node)
        {
            // overridden by the sub-class.            
        }

        /// <summary>
        /// Add the node to the set of root notifiers.
        /// </summary>
        protected virtual void AddRootNotifier(NodeState notifier)
        {
            for (int ii = 0; ii < RootNotifiers.Count; ii++)
            {
                if (Object.ReferenceEquals(notifier, RootNotifiers[ii]))
                {
                    return;
                }
            }

            RootNotifiers.Add(notifier);

            // subscribe to existing events.
            if (ServerData.EventManager != null)
            {
                IList<IUaEventMonitoredItem> monitoredItems = ServerData.EventManager.GetMonitoredItems();

                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    if (monitoredItems[ii].MonitoringAllEvents)
                    {
                        SubscribeToAllEvents(
                            SystemContext,
                            monitoredItems[ii],
                            true,
                            notifier);
                    }
                }
            }
        }

        /// <summary>
        /// Remove the node from the set of root notifiers.
        /// </summary>
        protected virtual void RemoveRootNotifier(NodeState notifier)
        {
            for (int ii = 0; ii < RootNotifiers.Count; ii++)
            {
                if (Object.ReferenceEquals(notifier, RootNotifiers[ii]))
                {
                    RootNotifiers.RemoveAt(ii);
                    break;
                }
            }
        }

        /// <summary>
        /// Ensures that all reverse references exist.
        /// </summary>
        /// <param name="externalReferences">A list of references to add to external targets.</param>
        protected virtual void AddReverseReferences(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            foreach (NodeState source in PredefinedNodes.Values)
            {
                // assign a default value to any variable value.
                BaseVariableState variable = source as BaseVariableState;

                if (variable != null && variable.Value == null)
                {
                    variable.Value = Opc.Ua.TypeInfo.GetDefaultValue(variable.DataType, variable.ValueRank, ServerData.TypeTree);
                }

                // add reference from supertype for type nodes.
                /*
                BaseTypeState type = source as BaseTypeState;

                if (type != null && !NodeId.IsNull(type.SuperTypeId))
                {
                    if (!IsNodeIdInNamespace(type.SuperTypeId))
                    {
                        AddExternalReference(
                            type.SuperTypeId,
                            ReferenceTypeIds.HasSubtype,
                            false,
                            type.NodeId,
                            externalReferences);
                    }
                }
                */

                IList<IReference> references = new List<IReference>();
                source.GetReferences(SystemContext, references);

                for (int ii = 0; ii < references.Count; ii++)
                {
                    IReference reference = references[ii];

                    // nothing to do with external nodes.
                    if (reference.TargetId == null || reference.TargetId.IsAbsolute)
                    {
                        continue;
                    }

                    NodeId targetId = (NodeId)reference.TargetId;

                    // add inverse reference to internal targets.
                    NodeState target = null;

                    if (PredefinedNodes.TryGetValue(targetId, out target))
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
            IList<IReference> referencesToAdd = null;

            if (!externalReferences.TryGetValue(sourceId, out referencesToAdd))
            {
                externalReferences[sourceId] = referencesToAdd = new List<IReference>();
            }

            // add reserve reference from external node.
            ReferenceNode referenceToAdd = new ReferenceNode();

            referenceToAdd.ReferenceTypeId = referenceTypeId;
            referenceToAdd.IsInverse = isInverse;
            referenceToAdd.TargetId = targetId;

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
            BaseTypeState type = Find(typeId) as BaseTypeState;

            if (type == null)
            {
                return;
            }

            AddTypesToTypeTree(type);
        }

        /// <summary>
        /// Finds the specified and checks if it is of the expected type. 
        /// </summary>
        /// <returns>Returns null if not found or not of the correct type.</returns>
        public NodeState FindPredefinedNode(NodeId nodeId, Type expectedType)
        {
            if (nodeId == null)
            {
                return null;
            }

            NodeState node = null;

            if (!PredefinedNodes.TryGetValue(nodeId, out node))
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
                PredefinedNodes.Clear();
            }
        }

        /// <summary>
        /// Returns an unique handle for the node.
        /// </summary>
        /// <param name="nodeId">The node to get the handle for.</param>
        /// <returns>A node handle, null if the node manager does not recognize the node id.</returns>
        /// <remarks>
        /// The method must not block by querying an underlying system. If the node manager wraps an 
        /// underlying system then it must check to see if it recognizes the syntax of the node id. 
        /// The handle in this case may simply be a partially parsed version of the node id. 
        /// </remarks>
        public virtual object GetManagerHandle(NodeId nodeId)
        {
            lock (Lock)
            {
                return GetManagerHandle(SystemContext, nodeId, null);
            }
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        /// <remarks>
        /// This must efficiently determine whether the node belongs to the node manager. If it does belong to 
        /// NodeManager it should return a handle that does not require the NodeId to be validated again when
        /// the handle is passed into other methods such as 'Read' or 'Write'.
        /// </remarks>
        protected virtual object GetManagerHandle(ISystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that not in the namespace.
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                // lookup the node.
                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                return node;
            }
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
                foreach (KeyValuePair<NodeId, IList<IReference>> current in references)
                {
                    // check for valid handle.
                    NodeState source = GetManagerHandle(SystemContext, current.Key, null) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // add reference to external target.
                    foreach (IReference reference in current.Value)
                    {
                        source.AddReference(reference.ReferenceTypeId, reference.IsInverse, reference.TargetId);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to delete bi-directional references to nodes from other node managers.
        /// </summary>
        public virtual ServiceResult DeleteReference(
            object sourceHandle,
            NodeId referenceTypeId,
            bool isInverse,
            ExpandedNodeId targetId,
            bool deleteBidirectional)
        {
            lock (Lock)
            {
                // check for valid handle.
                NodeState source = IsHandleInNamespace(sourceHandle);

                if (source == null)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                source.RemoveReference(referenceTypeId, isInverse, targetId);

                if (deleteBidirectional)
                {
                    // check if the target is also managed by the node manager.
                    if (!targetId.IsAbsolute)
                    {
                        NodeState target = GetManagerHandle(SystemContext, (NodeId)targetId, null) as NodeState;

                        if (target != null)
                        {
                            target.RemoveReference(referenceTypeId, !isInverse, source.NodeId);
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
        public virtual UaNodeMetadata GetNodeMetadata(
            UaServerOperationContext context,
            object targetHandle,
            BrowseResultMask resultMask)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // check for valid handle.
                NodeState target = IsHandleInNamespace(targetHandle);

                if (target == null)
                {
                    return null;
                }

                // validate node.
                if (!ValidateNode(systemContext, target))
                {
                    return null;
                }

                // read the attributes.
                List<object> values = target.ReadAttributes(
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
                    Attributes.UserExecutable);

                // construct the metadata object.

                UaNodeMetadata metadata = new UaNodeMetadata(target, target.NodeId);

                metadata.NodeClass = target.NodeClass;
                metadata.BrowseName = target.BrowseName;
                metadata.DisplayName = target.DisplayName;

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

                // get instance references.
                BaseInstanceState instance = target as BaseInstanceState;

                if (instance != null)
                {
                    metadata.TypeDefinition = instance.TypeDefinitionId;
                    metadata.ModellingRule = instance.ModellingRuleId;
                }

                // fill in the common attributes.
                return metadata;
            }
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
            ref UaContinuationPoint continuationPoint,
            IList<ReferenceDescription> references)
        {
            if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));
            if (references == null) throw new ArgumentNullException(nameof(references));

            // check for view.
            if (!ViewDescription.IsDefault(continuationPoint.View))
            {
                throw new ServiceResultException(StatusCodes.BadViewIdUnknown);
            }

            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // verify that the node exists.
                NodeState source = IsHandleInNamespace(continuationPoint.NodeToBrowse);

                if (source == null)
                {
                    throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
                }

                // validate node.
                if (!ValidateNode(systemContext, source))
                {
                    throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
                }

                // check for previous continuation point.
                INodeBrowser browser = continuationPoint.Data as INodeBrowser;

                // fetch list of references.
                if (browser == null)
                {
                    // create a new browser.
                    browser = source.CreateBrowser(
                        systemContext,
                        continuationPoint.View,
                        continuationPoint.ReferenceTypeId,
                        continuationPoint.IncludeSubtypes,
                        continuationPoint.BrowseDirection,
                        null,
                        null,
                        false);
                }

                // apply filters to references.
                for (IReference reference = browser.Next(); reference != null; reference = browser.Next())
                {
                    // create the type definition reference.        
                    ReferenceDescription description = GetReferenceDescription(context, reference, continuationPoint);

                    if (description == null)
                    {
                        continue;
                    }

                    // check if limit reached.
                    if (continuationPoint.MaxResultsToReturn != 0 && references.Count >= continuationPoint.MaxResultsToReturn)
                    {
                        browser.Push(reference);
                        continuationPoint.Data = browser;
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
        /// Returns the references for the node that meets the criteria specified.
        /// </summary>
        private ReferenceDescription GetReferenceDescription(
            UaServerOperationContext context,
            IReference reference,
            UaContinuationPoint continuationPoint)
        {
            // create the type definition reference.        
            ReferenceDescription description = new ReferenceDescription();

            description.NodeId = reference.TargetId;
            description.SetReferenceType(continuationPoint.ResultMask, reference.ReferenceTypeId, !reference.IsInverse);

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
            NodeStateReference referenceInfo = reference as NodeStateReference;

            if (referenceInfo != null)
            {
                target = referenceInfo.Target;
            }

            // check for internal reference.
            if (target == null)
            {
                NodeId targetId = (NodeId)reference.TargetId;

                if (IsNodeIdInNamespace(targetId))
                {
                    if (!PredefinedNodes.TryGetValue(targetId, out target))
                    {
                        target = null;
                    }
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

            NodeId typeDefinition = null;

            BaseInstanceState instance = target as BaseInstanceState;

            if (instance != null)
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
            UaServerOperationContext context,
            object sourceHandle,
            RelativePathElement relativePath,
            IList<ExpandedNodeId> targetIds,
            IList<NodeId> unresolvedTargetIds)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();

            lock (Lock)
            {
                // verify that the node exists.
                NodeState source = IsHandleInNamespace(sourceHandle);

                if (source == null)
                {
                    return;
                }

                // validate node.
                if (!ValidateNode(systemContext, source))
                {
                    return;
                }

                // get list of references that relative path.
                INodeBrowser browser = source.CreateBrowser(
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
                    for (IReference reference = browser.Next(); reference != null; reference = browser.Next())
                    {
                        // ignore unknown external references.
                        if (reference.TargetId.IsAbsolute)
                        {
                            continue;
                        }

                        NodeState target = null;

                        // check for local reference.
                        NodeStateReference referenceInfo = reference as NodeStateReference;

                        if (referenceInfo != null)
                        {
                            target = referenceInfo.Target;
                        }

                        if (target == null)
                        {
                            NodeId targetId = (NodeId)reference.TargetId;

                            // the target may be a reference to a node in another node manager.
                            if (!IsNodeIdInNamespace(targetId))
                            {
                                unresolvedTargetIds.Add((NodeId)reference.TargetId);
                                continue;
                            }

                            // look up the target manually.
                            target = GetManagerHandle(systemContext, targetId, operationCache) as NodeState;

                            if (target == null)
                            {
                                continue;
                            }
                        }

                        // check browse name.
                        if (target.BrowseName == relativePath.TargetName)
                        {
                            // ensure duplicate node ids are not added.
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
            double maxAge,
            IList<ReadValueId> nodesToRead,
            IList<DataValue> values,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            List<ReadWriteOperationState> nodesToValidate = new List<ReadWriteOperationState>();

            lock (Lock)
            {
                for (int ii = 0; ii < nodesToRead.Count; ii++)
                {
                    ReadValueId nodeToRead = nodesToRead[ii];

                    // skip items that have already been processed.
                    if (nodeToRead.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    NodeState source = GetManagerHandle(systemContext, nodeToRead.NodeId, operationCache) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToRead.Processed = true;

                    // create an initial value.
                    DataValue value = values[ii] = new DataValue();

                    value.Value = null;
                    value.ServerTimestamp = DateTime.UtcNow;
                    value.SourceTimestamp = DateTime.MinValue;
                    value.StatusCode = StatusCodes.Good;

                    // check if the node is ready for reading.
                    if (source.ValidationRequired)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a seperate operation.
                        ReadWriteOperationState operation = new ReadWriteOperationState();

                        operation.Source = source;
                        operation.Index = ii;

                        nodesToValidate.Add(operation);

                        continue;
                    }

                    // read the attribute value.
                    errors[ii] = source.ReadAttribute(
                        systemContext,
                        nodeToRead.AttributeId,
                        nodeToRead.ParsedIndexRange,
                        nodeToRead.DataEncoding,
                        value);
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }

                // validates the nodes (reads values from the underlying data source if required).
                for (int ii = 0; ii < nodesToValidate.Count; ii++)
                {
                    ReadWriteOperationState operation = nodesToValidate[ii];

                    if (!ValidateNode(systemContext, operation.Source))
                    {
                        continue;
                    }

                    ReadValueId nodeToRead = nodesToRead[operation.Index];
                    DataValue value = values[operation.Index];

                    // update the attribute value.
                    errors[operation.Index] = operation.Source.ReadAttribute(
                        systemContext,
                        nodeToRead.AttributeId,
                        nodeToRead.ParsedIndexRange,
                        nodeToRead.DataEncoding,
                        value);
                }
            }
        }

        /// <summary>
        /// Stores the state of a call method operation.
        /// </summary>
        private struct ReadWriteOperationState
        {
            public NodeState Source;
            public int Index;
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected virtual bool ValidateNode(UaServerContext context, NodeState node)
        {
            // validate node only if required.
            if (node.ValidationRequired)
            {
                return node.Validate(context);
            }

            return true;
        }

        /// <summary>
        /// Reads the history for the specified nodes.
        /// </summary>
        public virtual void HistoryRead(
            UaServerOperationContext context,
            HistoryReadDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            List<ReadWriteOperationState> nodesToValidate = new List<ReadWriteOperationState>();
            List<ReadWriteOperationState> readsToComplete = new List<ReadWriteOperationState>();

            lock (Lock)
            {
                for (int ii = 0; ii < nodesToRead.Count; ii++)
                {
                    HistoryReadValueId nodeToRead = nodesToRead[ii];

                    // skip items that have already been processed.
                    if (nodeToRead.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    NodeState source = GetManagerHandle(systemContext, nodeToRead.NodeId, operationCache) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToRead.Processed = true;

                    // only variables supported.
                    BaseVariableState variable = source as BaseVariableState;

                    if (variable == null)
                    {
                        errors[ii] = StatusCodes.BadHistoryOperationUnsupported;
                        continue;
                    }

                    results[ii] = new HistoryReadResult();

                    ReadWriteOperationState operation = new ReadWriteOperationState();

                    operation.Source = source;
                    operation.Index = ii;

                    // check if the node is ready for reading.
                    if (source.ValidationRequired)
                    {
                        // must validate node in a seperate operation.
                        errors[ii] = StatusCodes.BadNodeIdUnknown;
                        nodesToValidate.Add(operation);
                        continue;
                    }

                    // read the data.
                    readsToComplete.Add(operation);
                }

                // validates the nodes (reads values from the underlying data source if required).
                for (int ii = 0; ii < nodesToValidate.Count; ii++)
                {
                    ReadWriteOperationState operation = nodesToValidate[ii];

                    if (!ValidateNode(systemContext, operation.Source))
                    {
                        continue;
                    }

                    readsToComplete.Add(operation);
                }
            }

            // reads the data without holding onto the lock.
            for (int ii = 0; ii < readsToComplete.Count; ii++)
            {
                ReadWriteOperationState operation = readsToComplete[ii];

                errors[operation.Index] = HistoryRead(
                    systemContext,
                    operation.Source,
                    details,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    nodesToRead[operation.Index],
                    results[operation.Index]);
            }
        }

        /// <summary>
        /// Reads the history for a single node which has already been validated.
        /// </summary>
        protected virtual ServiceResult HistoryRead(
            ISystemContext context,
            NodeState source,
            HistoryReadDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueId nodesToRead,
            HistoryReadResult result)
        {
            // check for variable.
            BaseVariableState variable = source as BaseVariableState;

            if (variable == null)
            {
                return StatusCodes.BadHistoryOperationUnsupported;
            }

            // check for access.
            lock (Lock)
            {
                if ((variable.AccessLevel & AccessLevels.HistoryRead) == 0)
                {
                    return StatusCodes.BadNotReadable;
                }
            }

            // handle read raw.
            ReadRawModifiedDetails readRawDetails = details as ReadRawModifiedDetails;

            if (readRawDetails != null)
            {
                return HistoryReadRaw(
                    context,
                    variable,
                    readRawDetails,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    nodesToRead,
                    result);
            }

            // handle read processed.
            ReadProcessedDetails readProcessedDetails = details as ReadProcessedDetails;

            if (readProcessedDetails != null)
            {
                return HistoryReadProcessed(
                    context,
                    variable,
                    readProcessedDetails,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    nodesToRead,
                    result);
            }

            // handle read processed.
            ReadAtTimeDetails readAtTimeDetails = details as ReadAtTimeDetails;

            if (readAtTimeDetails != null)
            {
                return HistoryReadAtTime(
                    context,
                    variable,
                    readAtTimeDetails,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    nodesToRead,
                    result);
            }

            return StatusCodes.BadHistoryOperationUnsupported;
        }

        /// <summary>
        /// Reads the raw history for the variable value.
        /// </summary>
        protected virtual ServiceResult HistoryReadRaw(
            ISystemContext context,
            BaseVariableState source,
            ReadRawModifiedDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueId nodeToRead,
            HistoryReadResult result)
        {
            return StatusCodes.BadHistoryOperationUnsupported;
        }

        /// <summary>
        /// Reads the processed history for the variable value.
        /// </summary>
        protected virtual ServiceResult HistoryReadProcessed(
            ISystemContext context,
            BaseVariableState source,
            ReadProcessedDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueId nodeToRead,
            HistoryReadResult result)
        {
            return StatusCodes.BadHistoryOperationUnsupported;
        }

        /// <summary>
        /// Reads the history for the variable value.
        /// </summary>
        protected virtual ServiceResult HistoryReadAtTime(
            ISystemContext context,
            BaseVariableState source,
            ReadAtTimeDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueId nodeToRead,
            HistoryReadResult result)
        {
            return StatusCodes.BadHistoryOperationUnsupported;
        }


        /// <summary>
        /// Writes the value for the specified attributes.
        /// </summary>
        public virtual void Write(
            UaServerOperationContext context,
            IList<WriteValue> nodesToWrite,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            List<ReadWriteOperationState> nodesToValidate = new List<ReadWriteOperationState>();

            lock (Lock)
            {
                for (int ii = 0; ii < nodesToWrite.Count; ii++)
                {
                    WriteValue nodeToWrite = nodesToWrite[ii];

                    // skip items that have already been processed.
                    if (nodeToWrite.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    NodeState source = GetManagerHandle(systemContext, nodeToWrite.NodeId, operationCache) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToWrite.Processed = true;

                    // index range is not supported.
                    if (!String.IsNullOrEmpty(nodeToWrite.IndexRange))
                    {
                        errors[ii] = StatusCodes.BadWriteNotSupported;
                        continue;
                    }

                    // check if the node is ready for reading.
                    if (source.ValidationRequired)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a seperate operation.
                        ReadWriteOperationState operation = new ReadWriteOperationState();

                        operation.Source = source;
                        operation.Index = ii;

                        nodesToValidate.Add(operation);

                        continue;
                    }

                    // write the attribute value.
                    errors[ii] = source.WriteAttribute(
                        systemContext,
                        nodeToWrite.AttributeId,
                        nodeToWrite.ParsedIndexRange,
                        nodeToWrite.Value);

                    // updates to source finished - report changes to monitored items.
                    source.ClearChangeMasks(systemContext, false);
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }

                // validates the nodes (reads values from the underlying data source if required).
                for (int ii = 0; ii < nodesToValidate.Count; ii++)
                {
                    ReadWriteOperationState operation = nodesToValidate[ii];

                    if (!ValidateNode(systemContext, operation.Source))
                    {
                        continue;
                    }

                    WriteValue nodeToWrite = nodesToWrite[operation.Index];

                    // write the attribute value.
                    errors[operation.Index] = operation.Source.WriteAttribute(
                        systemContext,
                        nodeToWrite.AttributeId,
                        nodeToWrite.ParsedIndexRange,
                        nodeToWrite.Value);

                    // updates to source finished - report changes to monitored items.
                    operation.Source.ClearChangeMasks(systemContext, false);
                }
            }
        }

        /// <summary>
        /// Updates the history for the specified nodes.
        /// </summary>
        public virtual void HistoryUpdate(
            UaServerOperationContext context,
            Type detailsType,
            IList<HistoryUpdateDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            List<ReadWriteOperationState> nodesToValidate = new List<ReadWriteOperationState>();

            lock (Lock)
            {
                for (int ii = 0; ii < nodesToUpdate.Count; ii++)
                {
                    HistoryUpdateDetails nodeToUpdate = nodesToUpdate[ii];

                    // skip items that have already been processed.
                    if (nodeToUpdate.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    NodeState source = GetManagerHandle(systemContext, nodeToUpdate.NodeId, operationCache) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    nodeToUpdate.Processed = true;

                    // check if the node is ready for reading.
                    if (source.ValidationRequired)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a seperate operation.
                        ReadWriteOperationState operation = new ReadWriteOperationState();

                        operation.Source = source;
                        operation.Index = ii;

                        nodesToValidate.Add(operation);

                        continue;
                    }

                    // historical data not available.
                    errors[ii] = StatusCodes.BadHistoryOperationUnsupported;
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }

                // validates the nodes (reads values from the underlying data source if required).
                for (int ii = 0; ii < nodesToValidate.Count; ii++)
                {
                    ReadWriteOperationState operation = nodesToValidate[ii];

                    if (!ValidateNode(systemContext, operation.Source))
                    {
                        continue;
                    }

                    // historical data not available.
                    errors[ii] = StatusCodes.BadHistoryOperationUnsupported;
                }
            }
        }

        /// <summary>
        /// Calls a method on the specified nodes.
        /// </summary>
        public virtual void Call(
            UaServerOperationContext context,
            IList<CallMethodRequest> methodsToCall,
            IList<CallMethodResult> results,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            List<CallOperationState> nodesToValidate = new List<CallOperationState>();

            lock (Lock)
            {
                for (int ii = 0; ii < methodsToCall.Count; ii++)
                {
                    CallMethodRequest methodToCall = methodsToCall[ii];

                    // skip items that have already been processed.
                    if (methodToCall.Processed)
                    {
                        continue;
                    }

                    // check for valid handle.
                    NodeState source = GetManagerHandle(systemContext, methodToCall.ObjectId, operationCache) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    methodToCall.Processed = true;

                    // find the method.
                    MethodState method = source.FindMethod(systemContext, methodToCall.MethodId);

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

                    CallMethodResult result = results[ii] = new CallMethodResult();

                    // check if the node is ready for reading.
                    if (source.ValidationRequired)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a seperate operation.
                        CallOperationState operation = new CallOperationState();

                        operation.Source = source;
                        operation.Method = method;
                        operation.Index = ii;

                        nodesToValidate.Add(operation);

                        continue;
                    }

                    // call the method.
                    errors[ii] = Call(
                        systemContext,
                        methodToCall,
                        source,
                        method,
                        result);
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }

                // validates the nodes (reads values from the underlying data source if required).
                for (int ii = 0; ii < nodesToValidate.Count; ii++)
                {
                    CallOperationState operation = nodesToValidate[ii];

                    // validate the object.
                    if (!ValidateNode(systemContext, operation.Source))
                    {
                        continue;
                    }

                    // call the method.
                    CallMethodResult result = results[operation.Index];

                    errors[operation.Index] = Call(
                        systemContext,
                        methodsToCall[operation.Index],
                        operation.Source,
                        operation.Method,
                        result);
                }
            }
        }

        /// <summary>
        /// Stores the state of a call method operation.
        /// </summary>
        private struct CallOperationState
        {
            public NodeState Source;
            public MethodState Method;
            public int Index;
        }

        /// <summary>
        /// Calls a method on an object.
        /// </summary>
        protected virtual ServiceResult Call(
            ISystemContext context,
            CallMethodRequest methodToCall,
            NodeState source,
            MethodState method,
            CallMethodResult result)
        {
            UaServerContext systemContext = context as UaServerContext;
            List<ServiceResult> argumentErrors = new List<ServiceResult>();
            VariantCollection outputArguments = new VariantCollection();

            ServiceResult error = method.Call(
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
            bool argumentsValid = true;

            for (int jj = 0; jj < argumentErrors.Count; jj++)
            {
                ServiceResult argumentError = argumentErrors[jj];

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
            UaServerOperationContext context,
            object sourceId,
            uint subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool unsubscribe)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // check for valid handle.
                NodeState source = IsHandleInNamespace(sourceId);

                if (source == null)
                {
                    return StatusCodes.BadNodeIdInvalid;
                }

                // check if the object supports subscritions.
                BaseObjectState instance = sourceId as BaseObjectState;

                if (instance == null || instance.EventNotifier != EventNotifiers.SubscribeToEvents)
                {
                    return StatusCodes.BadNotSupported;
                }

                MonitoredNode monitoredNode = instance.Handle as MonitoredNode;

                // handle unsubscribe.
                if (unsubscribe)
                {
                    if (monitoredNode != null)
                    {
                        monitoredNode.UnsubscribeToEvents(systemContext, monitoredItem);

                        // do any post processing.
                        OnUnsubscribeToEvents(systemContext, monitoredNode, monitoredItem);
                    }

                    return ServiceResult.Good;
                }

                // subscribe to events.
                if (monitoredNode == null)
                {
                    instance.Handle = monitoredNode = new MonitoredNode(ServerData, this, source);
                }

                monitoredNode.SubscribeToEvents(systemContext, monitoredItem);

                // do any post processing.
                OnSubscribeToEvents(systemContext, monitoredNode, monitoredItem);

                return ServiceResult.Good;
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
            UaServerOperationContext context,
            uint subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool unsubscribe)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                // update root notifiers.
                for (int ii = 0; ii < RootNotifiers.Count; ii++)
                {
                    SubscribeToAllEvents(
                        systemContext,
                        monitoredItem,
                        unsubscribe,
                        RootNotifiers[ii]);
                }

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Subscribes/unsubscribes to all events produced by the specified node.
        /// </summary>
        protected void SubscribeToAllEvents(
            ISystemContext systemContext,
            IUaEventMonitoredItem monitoredItem,
            bool unsubscribe,
            NodeState source)
        {
            MonitoredNode monitoredNode = source.Handle as MonitoredNode;

            // handle unsubscribe.
            if (unsubscribe)
            {
                if (monitoredNode != null)
                {
                    monitoredNode.UnsubscribeToEvents(systemContext, monitoredItem);

                    // do any post processing.
                    OnUnsubscribeToEvents(systemContext, monitoredNode, monitoredItem);
                }

                return;
            }

            // subscribe to events.
            if (monitoredNode == null)
            {
                source.Handle = monitoredNode = new MonitoredNode(ServerData, this, source);
            }

            monitoredNode.SubscribeToEvents(systemContext, monitoredItem);

            // do any post processing.
            OnSubscribeToEvents(systemContext, monitoredNode, monitoredItem);
        }

        /// <summary>
        /// Does any processing after a monitored item is subscribed to.
        /// </summary>
        protected virtual void OnSubscribeToEvents(
            ISystemContext systemContext,
            MonitoredNode monitoredNode,
            IUaEventMonitoredItem monitoredItem)
        {
            // does nothing.
        }

        /// <summary>
        /// Does any processing after a monitored item is subscribed to.
        /// </summary>
        protected virtual void OnUnsubscribeToEvents(
            ISystemContext systemContext,
            MonitoredNode monitoredNode,
            IUaEventMonitoredItem monitoredItem)
        {
            // does nothing.
        }

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
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    IUaEventMonitoredItem monitoredItem = monitoredItems[ii];

                    if (monitoredItem == null)
                    {
                        continue;
                    }

                    // check for global subscription.
                    if (monitoredItem.MonitoringAllEvents)
                    {
                        for (int jj = 0; jj < RootNotifiers.Count; jj++)
                        {
                            MonitoredNode monitoredNode = RootNotifiers[jj].Handle as MonitoredNode;

                            if (monitoredNode == null)
                            {
                                continue;
                            }

                            monitoredNode.ConditionRefresh(systemContext, monitoredItem);
                        }
                    }

                    // check for subscription to local node.
                    else
                    {
                        NodeState source = IsHandleInNamespace(monitoredItem.ManagerHandle);

                        if (source == null)
                        {
                            continue;
                        }

                        MonitoredNode monitoredNode = source.Handle as MonitoredNode;

                        if (monitoredNode == null)
                        {
                            continue;
                        }

                        monitoredNode.ConditionRefresh(systemContext, monitoredItem);
                    }
                }
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Creates a new set of monitored items for a set of variables.
        /// </summary>
        /// <remarks>
        /// This method only handles data change subscriptions. Event subscriptions are created by the SDK.
        /// </remarks>
        public virtual void CreateMonitoredItems(
            UaServerOperationContext context,
            uint subscriptionId,
            double publishingInterval,
            TimestampsToReturn timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult> errors,
            IList<MonitoringFilterResult> filterErrors,
            IList<IUaMonitoredItem> monitoredItems,
            ref long globalIdCounter)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();
            List<ReadWriteOperationState> nodesToValidate = new List<ReadWriteOperationState>();

            lock (Lock)
            {
                for (int ii = 0; ii < itemsToCreate.Count; ii++)
                {
                    MonitoredItemCreateRequest itemToCreate = itemsToCreate[ii];

                    // skip items that have already been processed.
                    if (itemToCreate.Processed)
                    {
                        continue;
                    }

                    ReadValueId itemToMonitor = itemToCreate.ItemToMonitor;

                    // check for valid handle.
                    NodeState source = GetManagerHandle(systemContext, itemToMonitor.NodeId, operationCache) as NodeState;

                    if (source == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    itemToCreate.Processed = true;

                    // check if the node is ready for reading.
                    if (source.ValidationRequired)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;

                        // must validate node in a seperate operation.
                        ReadWriteOperationState operation = new ReadWriteOperationState();

                        operation.Source = source;
                        operation.Index = ii;

                        nodesToValidate.Add(operation);

                        continue;
                    }

                    MonitoringFilterResult filterError = null;
                    IUaMonitoredItem monitoredItem = null;

                    errors[ii] = CreateMonitoredItem(
                        systemContext,
                        source,
                        subscriptionId,
                        publishingInterval,
                        context.DiagnosticsMask,
                        timestampsToReturn,
                        itemToCreate,
                        ref globalIdCounter,
                        out filterError,
                        out monitoredItem);

                    // save any filter error details.
                    filterErrors[ii] = filterError;

                    if (ServiceResult.IsBad(errors[ii]))
                    {
                        continue;
                    }

                    // save the monitored item.
                    monitoredItems[ii] = monitoredItem;
                }

                // check for nothing to do.
                if (nodesToValidate.Count == 0)
                {
                    return;
                }

                // validates the nodes (reads values from the underlying data source if required).
                for (int ii = 0; ii < nodesToValidate.Count; ii++)
                {
                    ReadWriteOperationState operation = nodesToValidate[ii];

                    // validate the object.
                    if (!ValidateNode(systemContext, operation.Source))
                    {
                        continue;
                    }

                    MonitoredItemCreateRequest itemToCreate = itemsToCreate[operation.Index];

                    MonitoringFilterResult filterError = null;
                    IUaMonitoredItem monitoredItem = null;

                    errors[operation.Index] = CreateMonitoredItem(
                        systemContext,
                        operation.Source,
                        subscriptionId,
                        publishingInterval,
                        context.DiagnosticsMask,
                        timestampsToReturn,
                        itemToCreate,
                        ref globalIdCounter,
                        out filterError,
                        out monitoredItem);

                    // save any filter error details.
                    filterErrors[operation.Index] = filterError;

                    if (ServiceResult.IsBad(errors[operation.Index]))
                    {
                        continue;
                    }

                    // save the monitored item.
                    monitoredItems[operation.Index] = monitoredItem;
                }
            }
        }

        /// <summary>
        /// Reads the initial value for a monitored item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The monitored node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="ignoreFilters">If the filters should be ignored.</param>
        protected virtual ServiceResult ReadInitialValue(
            ISystemContext context,
            MonitoredNode node,
            IUaDataChangeMonitoredItem2 monitoredItem,
            bool ignoreFilters)
        {
            DataValue initialValue = new DataValue
            {
                Value = null,
                ServerTimestamp = DateTime.UtcNow,
                SourceTimestamp = DateTime.MinValue,
                StatusCode = StatusCodes.BadWaitingForInitialData
            };

            ServiceResult error = node.Node.ReadAttribute(
                context,
                monitoredItem.AttributeId,
                monitoredItem.IndexRange,
                monitoredItem.DataEncoding,
                initialValue);

            monitoredItem.QueueValue(initialValue, error, ignoreFilters);

            return error;
        }

        /// <summary>
        /// Validates a data change filter provided by the client.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="source">The node being monitored.</param>
        /// <param name="attributeId">The attribute being monitored.</param>
        /// <param name="requestedFilter">The requested monitoring filter.</param>
        /// <param name="filter">The validated data change filter.</param>
        /// <param name="range">The EU range associated with the value if required by the filter.</param>
        /// <returns>Any error condition. Good if no errors occurred.</returns>
        protected ServiceResult ValidateDataChangeFilter(
            ISystemContext context,
            NodeState source,
            uint attributeId,
            ExtensionObject requestedFilter,
            out DataChangeFilter filter,
            out Opc.Ua.Range range)
        {
            filter = null;
            range = null;

            // check for valid filter type.
            filter = requestedFilter.Body as DataChangeFilter;

            if (filter == null)
            {
                return StatusCodes.BadMonitoredItemFilterUnsupported;
            }

            // only supported for value attributes.
            if (attributeId != Attributes.Value)
            {
                return StatusCodes.BadMonitoredItemFilterUnsupported;
            }

            // only supported for variables.
            BaseVariableState variable = source as BaseVariableState;

            if (variable == null)
            {
                return StatusCodes.BadMonitoredItemFilterUnsupported;
            }

            // check the datatype.
            if (filter.DeadbandType != (uint)DeadbandType.None)
            {
                BuiltInType builtInType = Opc.Ua.TypeInfo.GetBuiltInType(variable.DataType, ServerData.TypeTree);

                if (!Opc.Ua.TypeInfo.IsNumericType(builtInType))
                {
                    return StatusCodes.BadMonitoredItemFilterUnsupported;
                }
            }

            // validate filter.
            ServiceResult error = filter.Validate();

            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            if (filter.DeadbandType == (uint)DeadbandType.Percent)
            {
                BaseVariableState euRange = variable.FindChild(context, BrowseNames.EURange) as BaseVariableState;

                if (euRange == null)
                {
                    return StatusCodes.BadMonitoredItemFilterUnsupported;
                }

                range = euRange.Value as Opc.Ua.Range;

                if (range == null)
                {
                    return StatusCodes.BadMonitoredItemFilterUnsupported;
                }
            }

            // all good.
            return ServiceResult.Good;
        }

        /// <summary>
        /// Creates a new set of monitored items for a set of variables.
        /// </summary>
        /// <remarks>
        /// This method only handles data change subscriptions. Event subscriptions are created by the SDK.
        /// </remarks>
        protected virtual ServiceResult CreateMonitoredItem(
            ISystemContext context,
            NodeState source,
            uint subscriptionId,
            double publishingInterval,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemCreateRequest itemToCreate,
            ref long globalIdCounter,
            out MonitoringFilterResult filterError,
            out IUaMonitoredItem monitoredItem)
        {
            filterError = null;
            monitoredItem = null;
            ServiceResult error = null;

            // read initial value.
            DataValue initialValue = new DataValue
            {
                Value = null,
                ServerTimestamp = DateTime.UtcNow,
                SourceTimestamp = DateTime.MinValue,
                StatusCode = StatusCodes.BadWaitingForInitialData
            };

            error = source.ReadAttribute(
                context,
                itemToCreate.ItemToMonitor.AttributeId,
                itemToCreate.ItemToMonitor.ParsedIndexRange,
                itemToCreate.ItemToMonitor.DataEncoding,
                initialValue);

            if (ServiceResult.IsBad(error))
            {
                if (error.StatusCode == StatusCodes.BadAttributeIdInvalid ||
                    error.StatusCode == StatusCodes.BadDataEncodingInvalid ||
                    error.StatusCode == StatusCodes.BadDataEncodingUnsupported)
                {
                    return error;
                }

                initialValue.StatusCode = error.StatusCode;
                error = ServiceResult.Good;
            }

            // validate parameters.
            MonitoringParameters parameters = itemToCreate.RequestedParameters;

            // validate the data change filter.
            DataChangeFilter filter = null;
            Opc.Ua.Range range = null;

            if (!ExtensionObject.IsNull(parameters.Filter))
            {
                error = ValidateDataChangeFilter(
                    context,
                    source,
                    itemToCreate.ItemToMonitor.AttributeId,
                    parameters.Filter,
                    out filter,
                    out range);

                if (ServiceResult.IsBad(error))
                {
                    return error;
                }
            }

            // create monitored node.
            MonitoredNode monitoredNode = source.Handle as MonitoredNode;

            if (monitoredNode == null)
            {
                source.Handle = monitoredNode = new MonitoredNode(ServerData, this, source);
            }

            // create a globally unique identifier.
            uint monitoredItemId = Utils.IncrementIdentifier(ref globalIdCounter);

            // determine the sampling interval.
            double samplingInterval = itemToCreate.RequestedParameters.SamplingInterval;

            if (samplingInterval < 0)
            {
                samplingInterval = publishingInterval;
            }

            // check if the variable needs to be sampled.
            bool samplingRequired = false;

            if (itemToCreate.ItemToMonitor.AttributeId == Attributes.Value)
            {
                BaseVariableState variable = source as BaseVariableState;

                if (variable.MinimumSamplingInterval > 0)
                {
                    samplingInterval = CalculateSamplingInterval(variable, samplingInterval);
                    samplingRequired = true;
                }
            }

            // create the item.
            DataChangeMonitoredItem datachangeItem = monitoredNode.CreateDataChangeItem(
                context,
                monitoredItemId,
                itemToCreate.ItemToMonitor.AttributeId,
                itemToCreate.ItemToMonitor.ParsedIndexRange,
                itemToCreate.ItemToMonitor.DataEncoding,
                diagnosticsMasks,
                timestampsToReturn,
                itemToCreate.MonitoringMode,
                itemToCreate.RequestedParameters.ClientHandle,
                samplingInterval,
                itemToCreate.RequestedParameters.QueueSize,
                itemToCreate.RequestedParameters.DiscardOldest,
                filter,
                range,
                false);

            if (samplingRequired)
            {
                CreateSampledItem(samplingInterval, datachangeItem);
            }

            // report the initial value.
            datachangeItem.QueueValue(initialValue, null, true);

            // do any post processing.
            OnCreateMonitoredItem(context, itemToCreate, monitoredNode, datachangeItem);

            // update monitored item list.
            monitoredItem = datachangeItem;

            return ServiceResult.Good;
        }

        /// <summary>
        /// Calculates the sampling interval.
        /// </summary>
        private double CalculateSamplingInterval(BaseVariableState variable, double samplingInterval)
        {
            if (samplingInterval < variable.MinimumSamplingInterval)
            {
                samplingInterval = variable.MinimumSamplingInterval;
            }

            if ((samplingInterval % minimumSamplingInterval_) != 0)
            {
                samplingInterval = Math.Truncate(samplingInterval / minimumSamplingInterval_);
                samplingInterval += 1;
                samplingInterval *= minimumSamplingInterval_;
            }

            return samplingInterval;
        }

        /// <summary>
        /// Creates a new sampled item.
        /// </summary>
        private void CreateSampledItem(double samplingInterval, DataChangeMonitoredItem monitoredItem)
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
        private void DeleteSampledItem(DataChangeMonitoredItem monitoredItem)
        {
            for (int ii = 0; ii < sampledItems_.Count; ii++)
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
                    for (int ii = 0; ii < sampledItems_.Count; ii++)
                    {
                        DataChangeMonitoredItem monitoredItem = sampledItems_[ii];

                        if (monitoredItem.TimeToNextSample < minimumSamplingInterval_)
                        {
                            monitoredItem.ValueChanged(SystemContext);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error during diagnostics scan.");
            }
        }

        /// <summary>
        /// Does any processing after a monitored item is created.
        /// </summary>
        protected virtual void OnCreateMonitoredItem(
            ISystemContext systemContext,
            MonitoredItemCreateRequest itemToCreate,
            MonitoredNode monitoredNode,
            DataChangeMonitoredItem monitoredItem)
        {
            // does nothing.
        }

        /// <summary>
        /// Modifies the parameters for a set of monitored items.
        /// </summary>
        public virtual void ModifyMonitoredItems(
            UaServerOperationContext context,
            TimestampsToReturn timestampsToReturn,
            IList<IUaMonitoredItem> monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult> errors,
            IList<MonitoringFilterResult> filterErrors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    MonitoredItemModifyRequest itemToModify = itemsToModify[ii];

                    // skip items that have already been processed.
                    if (itemToModify.Processed)
                    {
                        continue;
                    }

                    // modify the monitored item.
                    MonitoringFilterResult filterError = null;

                    errors[ii] = ModifyMonitoredItem(
                        systemContext,
                        context.DiagnosticsMask,
                        timestampsToReturn,
                        monitoredItems[ii],
                        itemToModify,
                        out filterError);

                    // save any filter error details.
                    filterErrors[ii] = filterError;
                }
            }
        }

        /// <summary>
        /// Modifies the parameters for a monitored item.
        /// </summary>
        protected virtual ServiceResult ModifyMonitoredItem(
            ISystemContext context,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            IUaMonitoredItem monitoredItem,
            MonitoredItemModifyRequest itemToModify,
            out MonitoringFilterResult filterError)
        {
            filterError = null;
            ServiceResult error = null;

            // check for valid handle.
            MonitoredNode monitoredNode = monitoredItem.ManagerHandle as MonitoredNode;

            if (monitoredNode == null)
            {
                return ServiceResult.Good;
            }

            if (IsHandleInNamespace(monitoredNode.Node) == null)
            {
                return ServiceResult.Good;
            }

            // owned by this node manager.
            itemToModify.Processed = true;

            // check for valid monitored item.
            DataChangeMonitoredItem datachangeItem = monitoredItem as DataChangeMonitoredItem;

            // validate parameters.
            MonitoringParameters parameters = itemToModify.RequestedParameters;

            // validate the data change filter.
            DataChangeFilter filter = null;
            Opc.Ua.Range range = null;

            if (!ExtensionObject.IsNull(parameters.Filter))
            {
                error = ValidateDataChangeFilter(
                    context,
                    monitoredNode.Node,
                    datachangeItem.AttributeId,
                    parameters.Filter,
                    out filter,
                    out range);

                if (ServiceResult.IsBad(error))
                {
                    return error;
                }
            }

            double previousSamplingInterval = datachangeItem.SamplingInterval;

            // check if the variable needs to be sampled.
            double samplingInterval = itemToModify.RequestedParameters.SamplingInterval;

            if (datachangeItem.AttributeId == Attributes.Value)
            {
                BaseVariableState variable = monitoredNode.Node as BaseVariableState;

                if (variable.MinimumSamplingInterval > 0)
                {
                    samplingInterval = CalculateSamplingInterval(variable, samplingInterval);
                }
            }

            // modify the monitored item parameters.
            error = datachangeItem.Modify(
                diagnosticsMasks,
                timestampsToReturn,
                itemToModify.RequestedParameters.ClientHandle,
                samplingInterval,
                itemToModify.RequestedParameters.QueueSize,
                itemToModify.RequestedParameters.DiscardOldest,
                filter,
                range);

            // do any post processing.
            OnModifyMonitoredItem(
                context,
                itemToModify,
                monitoredNode,
                datachangeItem,
                previousSamplingInterval);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Does any processing after a monitored item is created.
        /// </summary>
        protected virtual void OnModifyMonitoredItem(
            ISystemContext systemContext,
            MonitoredItemModifyRequest itemToModify,
            MonitoredNode monitoredNode,
            DataChangeMonitoredItem monitoredItem,
            double previousSamplingInterval)
        {
            // does nothing.
        }

        /// <summary>
        /// Deletes a set of monitored items.
        /// </summary>
        public virtual void DeleteMonitoredItems(
            UaServerOperationContext context,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii])
                    {
                        continue;
                    }

                    // delete the monitored item.
                    bool processed = false;

                    errors[ii] = DeleteMonitoredItem(
                        systemContext,
                        monitoredItems[ii],
                        out processed);

                    // indicate whether it was processed or not.
                    processedItems[ii] = processed;
                }
            }
        }

        /// <summary>
        /// Deletes a monitored item.
        /// </summary>
        protected virtual ServiceResult DeleteMonitoredItem(
            ISystemContext context,
            IUaMonitoredItem monitoredItem,
            out bool processed)
        {
            processed = false;

            // check for valid handle.
            MonitoredNode monitoredNode = monitoredItem.ManagerHandle as MonitoredNode;

            if (monitoredNode == null)
            {
                return ServiceResult.Good;
            }

            if (IsHandleInNamespace(monitoredNode.Node) == null)
            {
                return ServiceResult.Good;
            }

            // owned by this node manager.
            processed = true;

            // get the  source.
            NodeState source = monitoredNode.Node;

            // check for valid monitored item.
            DataChangeMonitoredItem datachangeItem = monitoredItem as DataChangeMonitoredItem;

            // check if the variable needs to be sampled.
            if (datachangeItem.AttributeId == Attributes.Value)
            {
                BaseVariableState variable = monitoredNode.Node as BaseVariableState;

                if (variable.MinimumSamplingInterval > 0)
                {
                    DeleteSampledItem(datachangeItem);
                }
            }

            // remove item.
            monitoredNode.DeleteItem(datachangeItem);

            // do any post processing.
            OnDeleteMonitoredItem(context, monitoredNode, datachangeItem);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Does any processing after a monitored item is deleted.
        /// </summary>
        protected virtual void OnDeleteMonitoredItem(
            ISystemContext systemContext,
            MonitoredNode monitoredNode,
            DataChangeMonitoredItem monitoredItem)
        {
            // does nothing.
        }

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
                    // check for valid handle.
                    MonitoredNode monitoredNode = monitoredItems[ii].ManagerHandle as MonitoredNode;

                    if (monitoredNode == null)
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
            // overridden by the sub-class.
        }

        /// <summary>
        /// Changes the monitoring mode for a set of monitored items.
        /// </summary>
        public virtual void SetMonitoringMode(
            UaServerOperationContext context,
            MonitoringMode monitoringMode,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            lock (Lock)
            {
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii])
                    {
                        continue;
                    }

                    // update monitoring mode.
                    bool processed = false;

                    errors[ii] = SetMonitoringMode(
                        systemContext,
                        monitoredItems[ii],
                        monitoringMode,
                        out processed);

                    // indicate whether it was processed or not.
                    processedItems[ii] = processed;
                }
            }
        }

        /// <summary>
        /// Changes the monitoring mode for an item.
        /// </summary>
        protected virtual ServiceResult SetMonitoringMode(
            ISystemContext context,
            IUaMonitoredItem monitoredItem,
            MonitoringMode monitoringMode,
            out bool processed)
        {
            processed = false;

            // check for valid handle.
            MonitoredNode monitoredNode = monitoredItem.ManagerHandle as MonitoredNode;

            if (monitoredNode == null)
            {
                return ServiceResult.Good;
            }

            if (IsHandleInNamespace(monitoredNode.Node) == null)
            {
                return ServiceResult.Good;
            }

            // owned by this node manager.
            processed = true;

            // check for valid monitored item.
            DataChangeMonitoredItem datachangeItem = monitoredItem as DataChangeMonitoredItem;

            // update monitoring mode.
            MonitoringMode previousMode = datachangeItem.SetMonitoringMode(monitoringMode);

            // need to provide an immediate update after enabling.
            if (previousMode == MonitoringMode.Disabled && monitoringMode != MonitoringMode.Disabled)
            {
                ReadInitialValue(context, monitoredNode, datachangeItem, false);
            }

            // do any post processing.
            OnSetMonitoringMode(context, monitoredNode, datachangeItem, previousMode, monitoringMode);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Does any processing after a monitored item is created.
        /// </summary>
        protected virtual void OnSetMonitoringMode(
            ISystemContext systemContext,
            MonitoredNode monitoredNode,
            DataChangeMonitoredItem monitoredItem,
            MonitoringMode previousMode,
            MonitoringMode currentMode)
        {
            // does nothing.
        }
        #endregion

        #region Private Fields
        private string[] namespaceUris_;
        private Timer samplingTimer_;
        private List<DataChangeMonitoredItem> sampledItems_;
        private double minimumSamplingInterval_;
        #endregion
    }
}
