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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// An implementation of a client side nodecache.
    /// </summary>
    public partial class NodeCache : IUaNodeCache
    {
        /// <inheritdoc/>
        public async Task<INode> FindAsync(ExpandedNodeId nodeId, CancellationToken ct = default)
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

                // check if node already exists.
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
                return await FetchNodeAsync(nodeId, ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Utils.LogError("Could not fetch node from server: NodeId={0}, Reason='{1}'.", nodeId, e.Message);
                // nodes_[nodeId] = null;
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<IList<INode>> FindAsync(IList<ExpandedNodeId> nodeIds, CancellationToken ct = default)
        {
            // check for null.
            if (nodeIds == null || nodeIds.Count == 0)
            {
                return new List<INode>();
            }

            var count = nodeIds.Count;
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
                fetchedNodes = await FetchNodesAsync(fetchNodeIds, ct).ConfigureAwait(false);
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

        #region ITypeTable Methods
        /// <inheritdoc/>
        public async Task<NodeId> FindSuperTypeAsync(ExpandedNodeId typeId, CancellationToken ct)
        {
            INode type = await FindAsync(typeId, ct).ConfigureAwait(false);

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
        public async Task<NodeId> FindSuperTypeAsync(NodeId typeId, CancellationToken ct = default)
        {
            INode type = await FindAsync(typeId, ct).ConfigureAwait(false);

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
        #endregion

        #region INodeCache Methods
        /// <inheritdoc/>
        public async Task<Node> FetchNodeAsync(ExpandedNodeId nodeId, CancellationToken ct)
        {
            var localId = ExpandedNodeId.ToNodeId(nodeId, session_.NamespaceUris);

            if (localId == null)
            {
                return null;
            }

            // fetch node from server.
            Node source = await session_.ReadNodeAsync(localId, ct).ConfigureAwait(false);

            try
            {
                // fetch references from server.
                ReferenceDescriptionCollection references = await session_.FetchReferencesAsync(localId, ct).ConfigureAwait(false);

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

                            var target = new Node(reference);

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
        public async Task<IList<Node>> FetchNodesAsync(IList<ExpandedNodeId> nodeIds, CancellationToken ct)
        {
            var count = nodeIds.Count;
            if (count == 0)
            {
                return new List<Node>();
            }

            var localIds = new NodeIdCollection(
                nodeIds.Select(nodeId => ExpandedNodeId.ToNodeId(nodeId, session_.NamespaceUris)));

            // fetch nodes and references from server.
            (IList<Node> sourceNodes, IList<ServiceResult> readErrors) = await session_.ReadNodesAsync(localIds, NodeClass.Unspecified, ct: ct).ConfigureAwait(false);
            (IList<ReferenceDescriptionCollection> referenceCollectionList, IList<ServiceResult> fetchErrors) = await session_.FetchReferencesAsync(localIds, ct).ConfigureAwait(false); ;


            var ii = 0;
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

                                var target = new Node(reference);

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
        public async Task<IList<INode>> FindReferencesAsync(
            ExpandedNodeId nodeId,
            NodeId referenceTypeId,
            bool isInverse,
            bool includeSubtypes,
            CancellationToken ct)
        {
            IList<INode> targets = new List<INode>();


            if (!(await FindAsync(nodeId, ct).ConfigureAwait(false) is Node source))
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

            IList<INode> result = await FindAsync(targetIds, ct).ConfigureAwait(false);

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
        public async Task<IList<INode>> FindReferencesAsync(
            IList<ExpandedNodeId> nodeIds,
            IList<NodeId> referenceTypeIds,
            bool isInverse,
            bool includeSubtypes,
            CancellationToken ct)
        {
            IList<INode> targets = new List<INode>();
            if (nodeIds.Count == 0 || referenceTypeIds.Count == 0)
            {
                return targets;
            }
            var targetIds = new ExpandedNodeIdCollection();
            IList<INode> sources = await FindAsync(nodeIds, ct).ConfigureAwait(false);
            foreach (INode source in sources)
            {
                if (!(source is Node node))
                {
                    continue;
                }

                foreach (NodeId referenceTypeId in referenceTypeIds)
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

            IList<INode> result = await FindAsync(targetIds, ct).ConfigureAwait(false);
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
        public async Task FetchSuperTypesAsync(ExpandedNodeId nodeId, CancellationToken ct)
        {
            // find the target node,

            if (!(await FindAsync(nodeId, ct).ConfigureAwait(false) is ILocalNode source))
            {
                return;
            }

            // follow the tree.
            ILocalNode subType = source;

            while (subType != null)
            {
                ILocalNode superType = null;

                IList<IReference> references = subType.References.Find(ReferenceTypeIds.HasSubtype, true, true, this);

                if (references != null && references.Count > 0)
                {
                    superType = await FindAsync(references[0].TargetId, ct).ConfigureAwait(false) as ILocalNode;
                }

                subType = superType;
            }
        }
        #endregion
    }
}