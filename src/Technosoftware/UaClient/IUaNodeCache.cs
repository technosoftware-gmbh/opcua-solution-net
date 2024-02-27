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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// A client side cache of the server's type model.
    /// </summary>
    public interface IUaNodeCache : INodeTable, ITypeTable
    {
        /// <summary>
        /// Loads the UA defined types into the cache.
        /// </summary>
        /// <param name="context">The context.</param>
        void LoadUaDefinedTypes(ISystemContext context);

        /// <summary>
        /// Removes all nodes from the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Fetches a node from the server and updates the cache.
        /// </summary>
        Node FetchNode(ExpandedNodeId nodeId);

        /// <summary>
        /// Finds a set of nodes in the nodeset,
        /// fetches missing nodes from server.
        /// </summary>
        /// <param name="nodeIds">The node identifier collection.</param>
        IList<INode> Find(IList<ExpandedNodeId> nodeIds);

        /// <summary>
        /// Fetches a node collection from the server and updates the cache.
        /// </summary>
        IList<Node> FetchNodes(IList<ExpandedNodeId> nodeIds);

        /// <summary>
        /// Finds a set of nodes in the nodeset,
        /// fetches missing nodes from server.
        /// </summary>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="ct"></param>
        Task<INode> FindAsync(ExpandedNodeId nodeId, CancellationToken ct = default);

        /// <summary>
        /// Finds a set of nodes in the nodeset,
        /// fetches missing nodes from server.
        /// </summary>
        /// <param name="nodeIds">The node identifier collection.</param>
        /// <param name="ct"></param>
        Task<IList<INode>> FindAsync(IList<ExpandedNodeId> nodeIds, CancellationToken ct = default);

        /// <summary>
        /// Fetches a node from the server and updates the cache.
        /// </summary>
        /// <param name="nodeId">Node id to fetch.</param>
        /// <param name="ct"></param>
        Task<Node> FetchNodeAsync(ExpandedNodeId nodeId, CancellationToken ct = default);

        /// <summary>
        /// Fetches a node collection from the server and updates the cache.
        /// </summary>
        /// <param name="nodeIds">The node identifier collection.</param>
        /// <param name="ct"></param>
        Task<IList<Node>> FetchNodesAsync(IList<ExpandedNodeId> nodeIds, CancellationToken ct = default);

        /// <summary>
        /// Adds the supertypes of the node to the cache.
        /// </summary>
        /// <param name="nodeId">Node id to fetch.</param>
        /// <param name="ct"></param>
        Task FetchSuperTypesAsync(ExpandedNodeId nodeId, CancellationToken ct = default);

        /// <summary>
        /// Adds the supertypes of the node to the cache.
        /// </summary>
        void FetchSuperTypes(ExpandedNodeId nodeId);

        /// <summary>
        /// Returns the references of the specified node that meet the criteria specified.
        /// </summary>
        IList<INode> FindReferences(ExpandedNodeId nodeId, NodeId referenceTypeId, bool isInverse, bool includeSubtypes);

        /// <summary>
        /// Returns the references of the specified nodes that meet the criteria specified.
        /// </summary>
        IList<INode> FindReferences(IList<ExpandedNodeId> nodeIds, IList<NodeId> referenceTypeIds, bool isInverse, bool includeSubtypes);

        /// <summary>
        /// Returns the references of the specified node that meet the criteria specified.
        /// </summary>
        Task<IList<INode>> FindReferencesAsync(ExpandedNodeId nodeId, NodeId referenceTypeId, bool isInverse, bool includeSubtypes, CancellationToken ct = default);

        /// <summary>
        /// Returns the references of the specified nodes that meet the criteria specified.
        /// </summary>
        Task<IList<INode>> FindReferencesAsync(IList<ExpandedNodeId> nodeIds, IList<NodeId> referenceTypeIds, bool isInverse, bool includeSubtypes, CancellationToken ct = default);

        /// <summary>
        /// Returns a display name for a node.
        /// </summary>
        string GetDisplayText(INode node);

        /// <summary>
        /// Returns a display name for a node.
        /// </summary>
        string GetDisplayText(ExpandedNodeId nodeId);

        /// <summary>
        /// Returns a display name for the target of a reference.
        /// </summary>
        string GetDisplayText(ReferenceDescription reference);
    }
}
