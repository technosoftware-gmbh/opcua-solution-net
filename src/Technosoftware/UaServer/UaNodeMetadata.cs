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

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Stores metadata required to process requests related to a node.
    /// </summary>
    public class UaNodeMetadata
    {
        #region Constructors
        /// <summary>
        /// Initializes the object with its handle and NodeId.
        /// </summary>
        public UaNodeMetadata(object handle, NodeId nodeId)
        {
            Handle = handle;
            NodeId = nodeId;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The handle assigned by the NodeManager that owns the Node.
        /// </summary>
        public object Handle { get; }

        /// <summary>
        /// The canonical NodeId for the Node.
        /// </summary>
        public NodeId NodeId { get; }

        /// <summary>
        /// The NodeClass for the Node.
        /// </summary>
        public NodeClass NodeClass { get; set; }

        /// <summary>
        /// The BrowseName for the Node.
        /// </summary>
        public QualifiedName BrowseName { get; set; }

        /// <summary>
        /// The DisplayName for the Node.
        /// </summary>
        public LocalizedText DisplayName { get; set; }

        /// <summary>
        /// The type definition for the Node (if one exists).
        /// </summary>
        public ExpandedNodeId TypeDefinition { get; set; }

        /// <summary>
        /// The modelling for the Node (if one exists).
        /// </summary>
        public NodeId ModellingRule { get; set; }

        /// <summary>
        /// Specifies which attributes are writable.
        /// </summary>
        public AttributeWriteMask WriteMask { get; set; }

        /// <summary>
        /// Whether the Node can be used with event subscriptions or for historical event queries.
        /// </summary>
        public byte EventNotifier { get; set; }

        /// <summary>
        /// Whether the Node can be use to read or write current or historical values.
        /// </summary>
        public byte AccessLevel { get; set; }

        /// <summary>
        /// Whether the Node is a Method that can be executed.
        /// </summary>
        public bool Executable { get; set; }

        /// <summary>
        /// The DataType of the Value attribute for Variable or VariableType nodes.
        /// </summary>
        public NodeId DataType { get; set; }

        /// <summary>
        /// The ValueRank for the Value attribute for Variable or VariableType nodes.
        /// </summary>
        public int ValueRank { get; set; }

        /// <summary>
        /// The ArrayDimensions for the Value attribute for Variable or VariableType nodes.
        /// </summary>
        public IList<uint> ArrayDimensions { get; set; }

        /// <summary>
        /// Specifies the AccessRestrictions that apply to a Node.
        /// </summary>
        public AccessRestrictionType AccessRestrictions { get; set; }

        /// <summary>
        /// The value reflects the DefaultAccessRestrictions Property of the NamespaceMetadata Object for the Namespace
        /// to which the Node belongs.
        /// </summary>
        public AccessRestrictionType DefaultAccessRestrictions { get; set; }

        /// <summary>
        /// The RolePermissions for the Node.
        /// Specifies the Permissions that apply to a Node for all Roles which have access to the Node.
        /// </summary>
        public RolePermissionTypeCollection RolePermissions { get; set; }

        /// <summary>
        /// The DefaultRolePermissions of the Node's name-space meta-data
        /// The value reflects the DefaultRolePermissions Property from the NamespaceMetadata Object associated with the Node.
        /// </summary>
        public RolePermissionTypeCollection DefaultRolePermissions { get; set; }

        /// <summary>
        /// The UserRolePermissions of the Node.
        /// Specifies the Permissions that apply to a Node for all Roles granted to current Session.
        /// </summary>
        public RolePermissionTypeCollection UserRolePermissions { get; set; }

        /// <summary>
        /// The DefaultUserRolePermissions of the Node.
        /// The value reflects the DefaultUserRolePermissions Property from the NamespaceMetadata Object associated with the Node.
        /// </summary>
        public RolePermissionTypeCollection DefaultUserRolePermissions { get; set; }

        #endregion
    }
}