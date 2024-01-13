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
using System.Linq;
using System.Reflection;
using System.Threading;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// An implementation of a client side nodecache.
    /// </summary>
    public partial class NodeCache : IUaNodeCache, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object with default values.
        /// </summary>
        public NodeCache(Session session)
        {
            session_ = session ?? throw new ArgumentNullException(nameof(session));
            typeTree_ = new TypeTable(session_.NamespaceUris);
            nodes_ = new NodeTable(session_.NamespaceUris, session_.ServerUris, typeTree_);
            uaTypesLoaded_ = false;
            cacheLock_ = new ReaderWriterLockSlim();
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                session_ = null;
                cacheLock_?.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region INodeTable Members
        /// <inheritdoc/>
        public NamespaceTable NamespaceUris => session_.NamespaceUris;

        /// <inheritdoc/>
        public StringTable ServerUris => session_.ServerUris;

        /// <inheritdoc/>
        public ITypeTable TypeTree => this;

        /// <inheritdoc/>
        public bool Exists(ExpandedNodeId nodeId)
        {
            return Find(nodeId) != null;
        }

        /// <inheritdoc/>
        public INode Find(ExpandedNodeId nodeId)
        {
            // check for null.
            if (NodeId.IsNull(nodeId))
            {
                return null;
            }

            INode node;
            try
            {
                cacheLock_.EnterReadLock();

                // check if node alredy exists.
                node = nodes_.Find(nodeId);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            if (node != null)
            {
                // do not return temporary nodes created after a Browse().
                if (node.GetType() != typeof(Node))
                {
                    return node;
                }
            }

            // fetch node from server.
            try
            {
                return FetchNode(nodeId);
            }
            catch (Exception e)
            {
                Utils.LogError("Could not fetch node from server: NodeId={0}, Reason='{1}'.", nodeId, e.Message);
                // nodes_[nodeId] = null;
                return null;
            }
        }

        /// <inheritdoc/>
        public IList<INode> Find(IList<ExpandedNodeId> nodeIds)
        {
            // check for null.
            if (nodeIds == null || nodeIds.Count == 0)
            {
                return new List<INode>();
            }

            int count = nodeIds.Count;
            IList<INode> nodes = new List<INode>(count);
            var fetchNodeIds = new ExpandedNodeIdCollection();

            int ii;
            for (ii = 0; ii < count; ii++)
            {
                INode node;
                try
                {
                    cacheLock_.EnterReadLock();

                    // check if node already exists.
                    node = nodes_.Find(nodeIds[ii]);
                }
                finally
                {
                    cacheLock_.ExitReadLock();
                }

                // do not return temporary nodes created after a Browse().
                if (node != null &&
                    node?.GetType() != typeof(Node))
                {
                    nodes.Add(node);
                }
                else
                {
                    nodes.Add(null);
                    fetchNodeIds.Add(nodeIds[ii]);
                }
            }

            if (fetchNodeIds.Count == 0)
            {
                return nodes;
            }

            // fetch missing nodes from server.
            IList<Node> fetchedNodes;
            try
            {
                fetchedNodes = FetchNodes(fetchNodeIds);
            }
            catch (Exception e)
            {
                Utils.LogError("Could not fetch nodes from server: Reason='{0}'.", e.Message);
                // nodes_[nodeId] = null;
                return nodes;
            }

            ii = 0;
            foreach (Node fetchedNode in fetchedNodes)
            {
                while (ii < count && nodes[ii] != null)
                {
                    ii++;
                }
                if (ii < count && nodes[ii] == null)
                {
                    nodes[ii++] = fetchedNode;
                }
                else
                {
                    Utils.LogError("Inconsistency fetching nodes from server. Not all nodes could be assigned.");
                    break;
                }
            }

            return nodes;
        }

        /// <inheritdoc/>
        public INode Find(
            ExpandedNodeId sourceId,
            NodeId referenceTypeId,
            bool isInverse,
            bool includeSubtypes,
            QualifiedName browseName)
        {
            // find the source.
            var source = Find(sourceId) as Node;
            if (source == null)
            {
                return null;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                // find all references.
                references = source.ReferenceTable.Find(referenceTypeId, isInverse, includeSubtypes, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }


            foreach (var reference in references)
            {
                var target = Find(reference.TargetId);

                if (target == null)
                {
                    continue;
                }

                if (target.BrowseName == browseName)
                {
                    return target;
                }
            }

            // target not found.
            return null;
        }

        /// <inheritdoc/>
        public IList<INode> Find(
            ExpandedNodeId sourceId,
            NodeId referenceTypeId,
            bool isInverse,
            bool includeSubtypes)
        {
            var hits = new List<INode>();

            // find the source.
            var source = Find(sourceId) as Node;

            if (source == null)
            {
                return hits;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                // find all references.
                references = source.ReferenceTable.Find(referenceTypeId, isInverse, includeSubtypes, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }


            foreach (var reference in references)
            {
                var target = Find(reference.TargetId);

                if (target == null)
                {
                    continue;
                }

                hits.Add(target);
            }

            return hits;
        }
        #endregion

        #region ITypeTable Methods
        /// <inheritdoc/>
        public bool IsKnown(ExpandedNodeId typeId)
        {
            var type = Find(typeId);

            if (type == null)
            {
                return false;
            }

            try
            {
                cacheLock_.EnterReadLock();

                return typeTree_.IsKnown(typeId);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }
        }

        /// <inheritdoc/>
        public bool IsKnown(NodeId typeId)
        {
            var type = Find(typeId);

            if (type == null)
            {
                return false;
            }

            try
            {
                cacheLock_.EnterReadLock();

                return typeTree_.IsKnown(typeId);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }
        }

        /// <inheritdoc/>
        public NodeId FindSuperType(ExpandedNodeId typeId)
        {
            var type = Find(typeId);

            if (type == null)
            {
                return null;
            }

            try
            {
                cacheLock_.EnterReadLock();

                return typeTree_.FindSuperType(typeId);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

        }

        /// <inheritdoc/>
        public NodeId FindSuperType(NodeId typeId)
        {
            var type = Find(typeId);

            if (type == null)
            {
                return null;
            }

            try
            {
                cacheLock_.EnterReadLock();

                return typeTree_.FindSuperType(typeId);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

        }

        /// <inheritdoc/>
        public IList<NodeId> FindSubTypes(ExpandedNodeId typeId)
        {
            var type = Find(typeId) as ILocalNode;

            if (type == null)
            {
                return new List<NodeId>();
            }

            var subtypes = new List<NodeId>();
            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = type.References.Find(ReferenceTypeIds.HasSubtype, false, true, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            foreach (var reference in references)
            {
                if (!reference.TargetId.IsAbsolute)
                {
                    subtypes.Add((NodeId)reference.TargetId);
                }
            }

            return subtypes;
        }

        /// <inheritdoc/>
        public bool IsTypeOf(ExpandedNodeId subTypeId, ExpandedNodeId superTypeId)
        {
            if (subTypeId == superTypeId)
            {
                return true;
            }

            var subtype = Find(subTypeId) as ILocalNode;

            if (subtype == null)
            {
                return false;
            }

            var superType = subtype;

            while (superType != null)
            {
                ExpandedNodeId currentId;
                try
                {
                    cacheLock_.EnterReadLock();

                    currentId = superType.References.FindTarget(ReferenceTypeIds.HasSubtype, true, true, typeTree_, 0);
                }
                finally
                {
                    cacheLock_.ExitReadLock();
                }

                if (currentId == superTypeId)
                {
                    return true;
                }

                superType = Find(currentId) as ILocalNode;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool IsTypeOf(NodeId subTypeId, NodeId superTypeId)
        {
            if (subTypeId == superTypeId)
            {
                return true;
            }

            var subtype = Find(subTypeId) as ILocalNode;

            if (subtype == null)
            {
                return false;
            }

            var superType = subtype;

            while (superType != null)
            {
                ExpandedNodeId currentId;
                try
                {
                    cacheLock_.EnterReadLock();

                    currentId = superType.References.FindTarget(ReferenceTypeIds.HasSubtype, true, true, typeTree_, 0);
                }
                finally
                {
                    cacheLock_.ExitReadLock();
                }

                if (currentId == superTypeId)
                {
                    return true;
                }

                superType = Find(currentId) as ILocalNode;
            }

            return false;
        }

        /// <inheritdoc/>
        public QualifiedName FindReferenceTypeName(NodeId referenceTypeId)
        {
            try
            {
                cacheLock_.EnterReadLock();

                return typeTree_.FindReferenceTypeName(referenceTypeId);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }
        }

        /// <inheritdoc/>
        public NodeId FindReferenceType(QualifiedName browseName)
        {
            try
            {
                cacheLock_.EnterReadLock();

                return typeTree_.FindReferenceType(browseName);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }
        }

        /// <inheritdoc/>
        public bool IsEncodingOf(ExpandedNodeId encodingId, ExpandedNodeId datatypeId)
        {
            var encoding = Find(encodingId) as ILocalNode;

            if (encoding == null)
            {
                return false;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = encoding.References.Find(ReferenceTypeIds.HasEncoding, true, true, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            foreach (var reference in references)
            {
                if (reference.TargetId == datatypeId)
                {
                    return true;
                }
            }

            // no match.
            return false;
        }

        /// <inheritdoc/>
        public bool IsEncodingFor(NodeId expectedTypeId, ExtensionObject value)
        {
            // no match on null values.
            if (value == null)
            {
                return false;
            }

            // check for exact match.
            if (expectedTypeId == value.TypeId)
            {
                return true;
            }

            // find the encoding.
            var encoding = Find(value.TypeId) as ILocalNode;

            if (encoding == null)
            {
                return false;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = encoding.References.Find(ReferenceTypeIds.HasEncoding, true, true, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            // find data type.
            foreach (var reference in references)
            {
                if (reference.TargetId == expectedTypeId)
                {
                    return true;
                }
            }

            // no match.
            return false;
        }

        /// <inheritdoc/>
        public bool IsEncodingFor(NodeId expectedTypeId, object value)
        {
            // null actual datatype matches nothing.
            if (value == null)
            {
                return false;
            }

            // null expected datatype matches everything.
            if (NodeId.IsNull(expectedTypeId))
            {
                return true;
            }

            // get the actual datatype.
            var actualTypeId = Opc.Ua.TypeInfo.GetDataTypeId(value);

            // value is valid if the expected datatype is same as or a supertype of the actual datatype
            // for example: expected datatype of 'Integer' matches an actual datatype of 'UInt32'.
            if (IsTypeOf(actualTypeId, expectedTypeId))
            {
                return true;
            }

            // allow matches non-structure values where the actual datatype is a supertype of the expected datatype.
            // for example: expected datatype of 'UtcTime' matches an actual datatype of 'DateTime'.
            if (actualTypeId != DataTypes.Structure)
            {
                return IsTypeOf(expectedTypeId, actualTypeId);
            }

            // for structure types must try to determine the subtype.
            if (value is ExtensionObject extension)
            {
                return IsEncodingFor(expectedTypeId, extension);
            }

            // every element in an array must match.
            if (value is ExtensionObject[] extensions)
            {
                foreach (var extensionObject in extensions)
                {
                    if (!IsEncodingFor(expectedTypeId, extensionObject))
                    {
                        return false;
                    }
                }

                return true;
            }

            // can only get here if the value is an unrecognized data type.
            return false;
        }

        /// <inheritdoc/>
        public NodeId FindDataTypeId(ExpandedNodeId encodingId)
        {
            var encoding = Find(encodingId) as ILocalNode;

            if (encoding == null)
            {
                return NodeId.Null;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = encoding.References.Find(ReferenceTypeIds.HasEncoding, true, true, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            if (references.Count > 0)
            {
                return ExpandedNodeId.ToNodeId(references[0].TargetId, session_.NamespaceUris);
            }

            return NodeId.Null;
        }

        /// <inheritdoc/>
        public NodeId FindDataTypeId(NodeId encodingId)
        {
            var encoding = Find(encodingId) as ILocalNode;

            if (encoding == null)
            {
                return NodeId.Null;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = encoding.References.Find(ReferenceTypeIds.HasEncoding, true, true, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            if (references.Count > 0)
            {
                return ExpandedNodeId.ToNodeId(references[0].TargetId, session_.NamespaceUris);
            }

            return NodeId.Null;
        }
        #endregion

        #region INodeCache Methods
        /// <inheritdoc/>
        public void LoadUaDefinedTypes(ISystemContext context)
        {
            if (uaTypesLoaded_)
            {
                return;
            }

            var predefinedNodes = new NodeStateCollection();

            var assembly = typeof(ArgumentCollection).GetTypeInfo().Assembly;
            predefinedNodes.LoadFromBinaryResource(context, "Opc.Ua.Stack.Generated.Opc.Ua.PredefinedNodes.uanodes", assembly, true);

            try
            {
                cacheLock_.EnterWriteLock();

                for (int ii = 0; ii < predefinedNodes.Count; ii++)
                {
                    BaseTypeState type = predefinedNodes[ii] as BaseTypeState;

                    if (type == null)
                    {
                        continue;
                    }

                    type.Export(context, nodes_);
                }
            }
            finally
            {
                cacheLock_.ExitWriteLock();
            }
            uaTypesLoaded_ = true;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            uaTypesLoaded_ = false;
            try
            {
                cacheLock_.EnterWriteLock();

                nodes_.Clear();
            }
            finally
            {
                cacheLock_.ExitWriteLock();
            }
        }

        /// <inheritdoc/>
        public Node FetchNode(ExpandedNodeId nodeId)
        {
            var localId = ExpandedNodeId.ToNodeId(nodeId, session_.NamespaceUris);

            if (localId == null)
            {
                return null;
            }

            // fetch node from server.
            var source = session_.ReadNode(localId);

            try
            {
                // fetch references from server.
                var references = session_.FetchReferences(localId);

                try
                {
                    cacheLock_.EnterUpgradeableReadLock();

                    foreach (ReferenceDescription reference in references)
                    {
                        // create a placeholder for the node if it does not already exist.
                        if (!nodes_.Exists(reference.NodeId))
                        {
                            // transform absolute identifiers.
                            if (reference.NodeId != null && reference.NodeId.IsAbsolute)
                            {
                                reference.NodeId = ExpandedNodeId.ToNodeId(reference.NodeId, NamespaceUris);
                            }

                            Node target = new Node(reference);

                            InternalWriteLockedAttach(target);
                        }

                        // add the reference.
                        source.ReferenceTable.Add(reference.ReferenceTypeId, !reference.IsForward, reference.NodeId);
                    }
                }
                finally
                {
                    cacheLock_.ExitUpgradeableReadLock();
                }
            }
            catch (Exception e)
            {
                Utils.LogError("Could not fetch references for valid node with NodeId = {0}. Error = {1}", nodeId, e.Message);
            }

            InternalWriteLockedAttach(source);

            return source;
        }

        /// <inheritdoc/>
        public IList<Node> FetchNodes(IList<ExpandedNodeId> nodeIds)
        {
            int count = nodeIds.Count;
            if (count == 0)
            {
                return new List<Node>();
            }

            NodeIdCollection localIds = new NodeIdCollection(
                nodeIds.Select(nodeId => ExpandedNodeId.ToNodeId(nodeId, session_.NamespaceUris)));

            // fetch nodes and references from server.
            session_.ReadNodes(localIds, out IList<Node> sourceNodes, out IList<ServiceResult> readErrors);
            session_.FetchReferences(localIds, out IList<ReferenceDescriptionCollection> referenceCollectionList, out IList<ServiceResult> fetchErrors);

            int ii = 0;
            for (ii = 0; ii < count; ii++)
            {
                if (ServiceResult.IsBad(readErrors[ii]))
                {
                    continue;
                }

                if (!ServiceResult.IsBad(fetchErrors[ii]))
                {
                    // fetch references from server.
                    ReferenceDescriptionCollection references = referenceCollectionList[ii];

                    foreach (ReferenceDescription reference in references)
                    {
                        try
                        {
                            cacheLock_.EnterUpgradeableReadLock();

                            // create a placeholder for the node if it does not already exist.
                            if (!nodes_.Exists(reference.NodeId))
                            {
                                // transform absolute identifiers.
                                if (reference.NodeId != null && reference.NodeId.IsAbsolute)
                                {
                                    reference.NodeId = ExpandedNodeId.ToNodeId(reference.NodeId, NamespaceUris);
                                }

                                Node target = new Node(reference);

                                InternalWriteLockedAttach(target);
                            }
                        }
                        finally
                        {
                            cacheLock_.ExitUpgradeableReadLock();
                        }

                        // add the reference.
                        sourceNodes[ii].ReferenceTable.Add(reference.ReferenceTypeId, !reference.IsForward, reference.NodeId);
                    }
                }

                InternalWriteLockedAttach(sourceNodes[ii]);
            }

            return sourceNodes;
        }

        /// <inheritdoc/>
        public void FetchSuperTypes(ExpandedNodeId nodeId)
        {
            // find the target node,
            var source = Find(nodeId) as ILocalNode;

            if (source == null)
            {
                return;
            }

            // follow the tree.
            var subType = source;

            while (subType != null)
            {
                ILocalNode superType = null;

                var references = subType.References.Find(ReferenceTypeIds.HasSubtype, true, true, this);

                if (references != null && references.Count > 0)
                {
                    superType = Find(references[0].TargetId) as ILocalNode;
                }

                subType = superType;
            }
        }

        /// <inheritdoc/>
        public IList<INode> FindReferences(
            ExpandedNodeId nodeId,
            NodeId referenceTypeId,
            bool isInverse,
            bool includeSubtypes)
        {
            IList<INode> targets = new List<INode>();

            var source = Find(nodeId) as Node;

            if (source == null)
            {
                return targets;
            }

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = source.ReferenceTable.Find(referenceTypeId, isInverse, includeSubtypes, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            var targetIds = new ExpandedNodeIdCollection(
                references.Select(reference => reference.TargetId));

            IList<INode> result = Find(targetIds);

            foreach (INode target in result)
            {
                if (target != null)
                {
                    targets.Add(target);
                }
            }
            return targets;
        }

        /// <inheritdoc/>
        public IList<INode> FindReferences(
            IList<ExpandedNodeId> nodeIds,
            IList<NodeId> referenceTypeIds,
            bool isInverse,
            bool includeSubtypes)
        {
            IList<INode> targets = new List<INode>();
            if (nodeIds.Count == 0 || referenceTypeIds.Count == 0)
            {
                return targets;
            }
            ExpandedNodeIdCollection targetIds = new ExpandedNodeIdCollection();
            IList<INode> sources = Find(nodeIds);
            foreach (INode source in sources)
            {
                if (!(source is Node node))
                {
                    continue;
                }

                foreach (var referenceTypeId in referenceTypeIds)
                {
                    IList<IReference> references;
                    try
                    {
                        cacheLock_.EnterReadLock();

                        references = node.ReferenceTable.Find(referenceTypeId, isInverse, includeSubtypes, typeTree_);
                    }
                    finally
                    {
                        cacheLock_.ExitReadLock();
                    }

                    targetIds.AddRange(
                        references.Select(reference => reference.TargetId));
                }
            }

            IList<INode> result = Find(targetIds);
            foreach (INode target in result)
            {
                if (target != null)
                {
                    targets.Add(target);
                }
            }

            return targets;
        }

        /// <inheritdoc/>
        public string GetDisplayText(INode node)
        {
            // check for null.
            if (node == null)
            {
                return String.Empty;
            }

            // check for remote node.
            var target = node as Node;

            if (target == null)
            {
                return node.ToString();
            }

            string displayText = null;

            // use the modelling rule to determine which parent to follow.
            var modellingRule = target.ModellingRule;

            IList<IReference> references;
            try
            {
                cacheLock_.EnterReadLock();

                references = target.ReferenceTable.Find(ReferenceTypeIds.Aggregates, true, true, typeTree_);
            }
            finally
            {
                cacheLock_.ExitReadLock();
            }

            foreach (var reference in references)
            {
                var parent = Find(reference.TargetId) as Node;

                // use the first parent if modelling rule is new.
                if (modellingRule == Objects.ModellingRule_Mandatory)
                {
                    displayText = GetDisplayText(parent);
                    break;
                }

                // use the type node as the parent for other modelling rules.
                if (parent is VariableTypeNode || parent is ObjectTypeNode)
                {
                    displayText = GetDisplayText(parent);
                    break;
                }
            }

            // prepend the parent display name.
            if (displayText != null)
            {
                return Utils.Format("{0}.{1}", displayText, node);
            }

            // simply use the node name.
            return node.ToString();
        }

        /// <inheritdoc/>
        public string GetDisplayText(ExpandedNodeId nodeId)
        {
            if (NodeId.IsNull(nodeId))
            {
                return String.Empty;
            }

            var node = Find(nodeId);

            if (node != null)
            {
                return GetDisplayText(node);
            }

            return Utils.Format("{0}", nodeId);
        }

        /// <inheritdoc/>
        public string GetDisplayText(ReferenceDescription reference)
        {
            if (reference == null || NodeId.IsNull(reference.NodeId))
            {
                return String.Empty;
            }

            var node = Find(reference.NodeId);

            if (node != null)
            {
                return GetDisplayText(node);
            }

            return reference.ToString();
        }
        #endregion

        #region Private Methods
        private void InternalWriteLockedAttach(ILocalNode node)
        {
            try
            {
                cacheLock_.EnterWriteLock();

                // add to cache.
                nodes_.Attach(node);
            }
            finally
            {
                cacheLock_.ExitWriteLock();
            }
        }
        #endregion

        #region Private Fields
        private ReaderWriterLockSlim cacheLock_ = new ReaderWriterLockSlim();
        private IUaSession session_;
        private TypeTable typeTree_;
        private NodeTable nodes_;
        private bool uaTypesLoaded_;
        #endregion
    }
}
