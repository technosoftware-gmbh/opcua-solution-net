#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Opc.Ua;
using Opc.Ua.Test;
using Range = Opc.Ua.Range;

using Technosoftware.UaServer;
#endregion

namespace Technosoftware.UaStandardServer
{
    /// <summary>
    /// A base implementation of the IUaNodeManager interface.
    /// </summary>
    /// <remarks>
    /// This node manager is a base class used in multiple samples. It implements the IUaNodeManager
    /// interface and allows sub-classes to override only the methods that they need.
    /// </remarks>
    public class UaStandardNodeManager : UaGenericNodeManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        /// <param name="uaServerData">The uaServerData data implementing the IUaServerData interface.</param>
        /// <param name="configuration">The used application configuration.</param>
        /// <param name="namespaceUris">Array of namespaces that are used by the application.</param>
        protected UaStandardNodeManager(
            IUaServerData uaServerData,
            ApplicationConfiguration configuration,
            params string[] namespaceUris)
            :
            base(uaServerData, configuration, namespaceUris)
        {
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
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node is BaseInstanceState instance && instance.Parent != null)
            {
                if (instance.Parent.NodeId.Identifier is string id)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }

            return node.NodeId;
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// A list of references
        /// </summary>
        protected IList<IReference> References { get; set; }
        #endregion

        #region Create Node Classes
        /// <summary>
        /// <para>Creates a new folder.</para>
        /// <para>Folders are used to organize the AddressSpace into a hierarchy of Nodes. They represent the root Node of a subtree, and have no other semantics associated
        /// with them.</para>
        /// </summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created folder object which can be used in further calls to <see cref="CreateFolderState" />.</returns>
        protected FolderState CreateFolderState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var folderState = new FolderState(parent)
            {
                SymbolicName = displayName.ToString(),
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                EventNotifier = EventNotifiers.None
            };

            if (parent != null)
            {
                parent.AddChild(folderState);
            }
            else
            {
                folderState.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                References.Add(new NodeStateReference(ReferenceTypes.Organizes, false, folderState.NodeId));
                folderState.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(folderState);
            }

            return folderState;
        }

        /// <summary>
        ///   <para>Creates a new Object NodeClass.</para>
        ///   <para>Objects are used to represent systems, system components, real-world objects and software objects. Objects are defined using the BaseObjectState class.</para>
        /// </summary>
        /// <param name="parent">The parent NodeState object the new Object NodeClass will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.<br /></para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created Object NodeClass.</returns>
        protected BaseObjectState CreateBaseObjectState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var baseObjectState = new BaseObjectState(parent)
            {
                SymbolicName = displayName.ToString(),
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = ObjectTypeIds.BaseObjectType,
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                EventNotifier = EventNotifiers.None
            };

            parent?.AddChild(baseObjectState);

            return baseObjectState;
        }

        /// <summary>
        ///   <para>Creates a new Property</para>
        ///   <para>Properties are used to define the characteristics of Nodes. Properties are defined using the Variable NodeClass. However, they restrict their use.</para>
        ///   <para>Properties are the leaf of any hierarchy; therefore they shall not be the SourceNode of any hierarchical References. This includes the HasComponent or
        /// HasProperty Reference, that is, Properties do not contain Properties and cannot expose their complex structure. However, they may be the SourceNode of any
        /// non-hierarchical References.</para>
        ///   <para>The HasTypeDefinition Reference points to the VariableType of the Property. Since Properties are uniquely identified by their BrowseName, all Properties
        /// shall point to the PropertyType defined in OPC 10000-5.</para>
        ///   <para>Properties shall always be defined in the context of another Node and shall be the TargetNode of at least one HasProperty Reference. To distinguish them
        /// from DataVariables, they shall not be the TargetNode of any HasComponent Reference. Thus, a HasProperty Reference pointing to a Variable Node defines this Node
        /// as a Property.</para>
        ///   <para>The BrowseName of a Property is always unique in the context of a Node. It is not permitted for a Node to refer to two Variables using HasProperty
        /// References having the same BrowseName.</para>
        /// </summary>
        /// <param name="parent">The parent NodeState object the new Property will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="dataType">
        ///     The data type of the new variable, e.g. <see cref="BuiltInType.SByte" />. See <see cref="BuiltInType" /> for all possible types
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.</para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created property object.</returns>
        protected PropertyState CreatePropertyState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel,
            object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var propertyState = new PropertyState(parent)
            {
                SymbolicName = displayName.ToString(),
                TypeDefinitionId = VariableTypeIds.PropertyType,
                ReferenceTypeId = ReferenceTypeIds.HasProperty,
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                DataType = (uint)dataType,
                ValueRank = valueRank,
                AccessLevel = accessLevel,
                UserAccessLevel = accessLevel,
                Historizing = false
            };

            if (initialValue != null)
            {
                propertyState.Value = initialValue;
            }

            parent?.AddChild(propertyState);

            return propertyState;
        }

        /// <summary>
        ///   <para>Creates a new View NodeClass.</para>
        ///   <para>Underlying systems are often large, and Clients often have an interest in only a specific subset of the data. They do not need, or want, to be burdened with
        /// viewing Nodes in the AddressSpace for which they have no interest.</para>
        ///   <para>To address this problem, this standard defines the concept of a View. Each View defines a subset of the Nodes in the AddressSpace. The entire AddressSpace
        /// is the default View. Each Node in a View may contain only a subset of its References, as defined by the creator of the View. The View Node acts as the root for
        /// the Nodes in the View. Views are defined using the View NodeClass.</para>
        ///   <para>All Nodes contained in a View shall be accessible starting from the View Node when browsing in the context of the View. It is not expected that all
        /// containing Nodes can be browsed directly from the View Node but rather browsed from other Nodes contained in the View.</para>
        ///   <para>A View Node may not only be used as additional entry point into the AddressSpace but as a construct to organize the AddressSpace and thus as the only entry
        /// point into a subset of the AddressSpace. Therefore, Clients shall not ignore View Nodes when exposing the AddressSpace. Simple Clients that do not deal with
        /// Views for filtering purposes can, for example, handle a View Node like an Object of type FolderType.<br /></para>
        /// </summary>
        /// <param name="parent">The parent NodeState object the new View NodeClass will be created in.</param>
        /// <param name="externalReferences">
        ///   <para>The externalReferences is an out parameter that allows the generic server to link to nodes.</para>
        /// </param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.</para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="ViewState" /></returns>
        protected ViewState CreateViewState(NodeState parent,
            IDictionary<NodeId, IList<IReference>> externalReferences, string browseName, LocalizedText displayName,
            LocalizedText description, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var viewState = new ViewState
            {
                SymbolicName = displayName.ToString(),
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                ContainsNoLoops = true
            };

            if (externalReferences != null)
            {
                if (!externalReferences.TryGetValue(ObjectIds.ViewsFolder, out var references))
                {
                    externalReferences[ObjectIds.ViewsFolder] = references = new List<IReference>();
                }
                viewState.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ViewsFolder);
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, viewState.NodeId));
            }

            if (parent != null)
            {
                parent.AddReference(ReferenceTypes.Organizes, false, viewState.NodeId);
                viewState.AddReference(ReferenceTypes.Organizes, true, parent.NodeId);
            }

            AddPredefinedNode(SystemContext, viewState);
            return viewState;
        }

        /// <summary>Creates a new DataVariable NodeClass.</summary>
        /// <param name="parent">The parent NodeState object the new DataVariable NodeClass will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="dataType">
        ///     The data type of the new variable, e.g. <see cref="BuiltInType.SByte" />. See
        ///     <see cref="BuiltInType" /> for all possible types
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.</para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="BaseDataVariableState" /></returns>
        protected BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel,
            object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var baseDataVariableTypeState = new BaseDataVariableState(parent)
            {
                SymbolicName = displayName.ToString(),
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                DataType = (uint)dataType,
                ValueRank = valueRank,
                AccessLevel = accessLevel,
                UserAccessLevel = accessLevel,
                Historizing = false
            };

            baseDataVariableTypeState.Value = initialValue ?? GetNewValue(baseDataVariableTypeState);
            baseDataVariableTypeState.StatusCode = StatusCodes.Good;
            baseDataVariableTypeState.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                baseDataVariableTypeState.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                baseDataVariableTypeState.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            parent?.AddChild(baseDataVariableTypeState);

            return baseDataVariableTypeState;
        }

        /// <summary>Creates a new DataVariable NodeClass.</summary>
        /// <param name="parent">The parent NodeState object the new DataVariable NodeClass will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="dataType">
        ///     The Node Id of the node used as data type of the new variable.
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.</para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="BaseDataVariableState" /></returns>
        protected BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description, NodeId dataType, int valueRank, byte accessLevel,
            object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var baseDataVariableTypeState = new BaseDataVariableState(parent)
            {
                SymbolicName = displayName.ToString(),
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                DataType = dataType,
                ValueRank = valueRank,
                AccessLevel = accessLevel,
                UserAccessLevel = accessLevel,
                Historizing = false
            };

            baseDataVariableTypeState.Value = initialValue ?? GetNewValue(baseDataVariableTypeState);
            baseDataVariableTypeState.StatusCode = StatusCodes.Good;
            baseDataVariableTypeState.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                baseDataVariableTypeState.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                baseDataVariableTypeState.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            parent?.AddChild(baseDataVariableTypeState);

            return baseDataVariableTypeState;
        }

        /// <summary>Creates a new DataVariable NodeClass.</summary>
        /// <param name="parent">The parent NodeState object the new DataVariable NodeClass will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="dataType">
        ///     The Expanded Node Id of the node used as data type of the new variable.
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.</para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="BaseDataVariableState" /></returns>
        protected BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description, ExpandedNodeId dataType, int valueRank,
            byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var baseDataVariableTypeState = new BaseDataVariableState(parent)
            {
                SymbolicName = displayName.ToString(),
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(browseName, NamespaceIndex),
                BrowseName = new QualifiedName(browseName, NamespaceIndex),
                DisplayName = displayName,
                Description = description,
                WriteMask = writeMask,
                UserWriteMask = userWriteMask,
                RolePermissions = rolePermissions,
                UserRolePermissions = userRolePermissions,
                DataType = (NodeId)dataType,
                ValueRank = valueRank,
                AccessLevel = accessLevel,
                UserAccessLevel = accessLevel,
                Historizing = false
            };

            baseDataVariableTypeState.Value = initialValue ?? GetNewValue(baseDataVariableTypeState);
            baseDataVariableTypeState.StatusCode = StatusCodes.Good;
            baseDataVariableTypeState.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                baseDataVariableTypeState.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                baseDataVariableTypeState.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            parent?.AddChild(baseDataVariableTypeState);

            return baseDataVariableTypeState;
        }
        #endregion

        #region DataAccess Server Facet related Methods
        /// <summary>Creates a new DataItem variable.</summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="dataType">
        ///     The data type of the new variable, e.g. <see cref="BuiltInType.SByte" />. See
        ///     <see cref="BuiltInType" /> for all possible types
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="definition">Definition is a vendor-specific, human readable string that specifies how the value of this DataItem is calculated. Definition is non-localized and will often contain an equation that can be parsed by certain clients, e.g. Definition::= "(TempA � 25) + TempB� </param>
        /// <param name="valuePrecision">
        ///     <para>
        ///         The optional valuePrecision Specifies the maximum precision that the server can maintain for the item based on restrictions in the target
        ///         environment. If null is specified the property ValuePrecision will not be created.
        ///         The precision can be used for the following DataTypes:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>For Float and Double values it specifies the number of digits after the decimal place.</item>
        ///         <item>
        ///             For DateTime values it indicates the minimum time difference in nanoseconds. E.g., a precision of
        ///             20000000 defines a precision of 20 milliseconds.
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="DataItemState" /></returns>
        protected DataItemState CreateDataItemState(NodeState parent, string browseName,
            LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            string definition = null, double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var variable = new DataItemState(parent);

            if (definition != null)
            {
                variable.Definition = new PropertyState<string>(variable);
            }

            if (valuePrecision != null)
            {
                variable.ValuePrecision = new PropertyState<double>(variable);
            }

            variable.Create(
                SystemContext,
                null,
                variable.BrowseName,
                null,
                true);

            variable.SymbolicName = displayName.ToString();
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            variable.NodeId = new NodeId(browseName, NamespaceIndex);
            variable.BrowseName = new QualifiedName(browseName, NamespaceIndex);
            variable.DisplayName = displayName;
            variable.Description = description;
            variable.WriteMask = writeMask;
            variable.UserWriteMask = userWriteMask;
            variable.RolePermissions = rolePermissions;
            variable.UserRolePermissions = userRolePermissions;
            variable.DataType = (uint)dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = accessLevel;
            variable.Historizing = false;

            variable.Value = initialValue ??
                             Opc.Ua.TypeInfo.GetDefaultValue((uint)dataType, valueRank, ServerData.TypeTree);
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            switch (valueRank)
            {
                case ValueRanks.OneDimension:
                    variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
                    break;
                case ValueRanks.TwoDimensions:
                    variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
                    break;
            }

            if (definition != null)
            {
                variable.Definition.Value = definition;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            if (valuePrecision != null)
            {
                variable.ValuePrecision.Value = (double)valuePrecision;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            parent?.AddChild(variable);

            return variable;
        }

        /// <summary>Creates a new AnalogItem variable.</summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="dataType">
        ///     The data type of the new variable, e.g. <see cref="BuiltInType.SByte" />. See
        ///     <see cref="BuiltInType" /> for all possible types
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="euRange">
        ///     <para>
        ///         The engineering unit range defines the value <see cref="Opc.Ua.Range" /> likely to be obtained in normal operation. It
        ///         is intended for such use as automatically
        ///         scaling a bar graph display.
        ///     </para>
        ///     <para>
        ///         Sensor or instrument failure or deactivation can result in a returned item value which is actually outside
        ///         this range.
        ///     </para>
        /// </param>
        /// <param name="engineeringUnit">The optional engineering unit specifies the units for the item value</param>
        /// <param name="instrumentRange">
        ///     The optional instrument range defines the value <see cref="Opc.Ua.Range" /> that can be returned by the
        ///     instrument.
        /// </param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="definition">Definition is a vendor-specific, human readable string that specifies how the value of this DataItem is calculated. Definition is non-localized and will often contain an equation that can be parsed by certain clients, e.g. Definition::= "(TempA � 25) + TempB� </param>
        /// <param name="valuePrecision">
        ///     <para>
        ///         The optional valuePrecision Specifies the maximum precision that the server can maintain for the item based on restrictions in the target
        ///         environment. If null is specified the property ValuePrecision will not be created.
        ///         The precision can be used for the following DataTypes:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>For Float and Double values it specifies the number of digits after the decimal place.</item>
        ///         <item>
        ///             For DateTime values it indicates the minimum time difference in nanoseconds. E.g., a precision of
        ///             20000000 defines a precision of 20 milliseconds.
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="AnalogItemState" /></returns>
        protected AnalogItemState CreateAnalogItemState(NodeState parent, string browseName,
            LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            Range euRange, EUInformation engineeringUnit = null, Range instrumentRange = null,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            string definition = null, double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            return CreateAnalogItemState(parent, browseName, displayName, description, (uint)dataType, valueRank,
                accessLevel, initialValue, euRange, engineeringUnit, instrumentRange, writeMask, userWriteMask,
                definition, valuePrecision, rolePermissions, userRolePermissions);
        }

        /// <summary>Creates a new AnalogItem variable.</summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="dataType">
        ///     The Node Id of the node used as data type of the new variable.
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new variable, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="euRange">
        ///     <para>
        ///         The engineering unit range defines the value <see cref="Opc.Ua.Range" /> likely to be obtained in normal operation. It
        ///         is intended for such use as automatically
        ///         scaling a bar graph display.
        ///     </para>
        ///     <para>
        ///         Sensor or instrument failure or deactivation can result in a returned item value which is actually outside
        ///         this range.
        ///     </para>
        /// </param>
        /// <param name="engineeringUnit">The optional engineering unit specifies the units for the item value</param>
        /// <param name="instrumentRange">
        ///     The optional instrument range defines the value <see cref="Opc.Ua.Range" /> that can be returned by the
        ///     instrument.
        /// </param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="definition">Definition is a vendor-specific, human readable string that specifies how the value of this DataItem is calculated. Definition is non-localized and will often contain an equation that can be parsed by certain clients, e.g. Definition::= "(TempA � 25) + TempB� </param>
        /// <param name="valuePrecision">
        ///     <para>
        ///         The optional valuePrecision Specifies the maximum precision that the server can maintain for the item based on restrictions in the target
        ///         environment. If null is specified the property ValuePrecision will not be created.
        ///         The precision can be used for the following DataTypes:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>For Float and Double values it specifies the number of digits after the decimal place.</item>
        ///         <item>
        ///             For DateTime values it indicates the minimum time difference in nanoseconds. E.g., a precision of
        ///             20000000 defines a precision of 20 milliseconds.
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="AnalogItemState" /></returns>
        protected AnalogItemState CreateAnalogItemState(NodeState parent, string browseName,
            LocalizedText displayName,
            LocalizedText description, NodeId dataType, int valueRank, byte accessLevel, object initialValue,
            Range euRange, EUInformation engineeringUnit = null, Range instrumentRange = null,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            string definition = null, double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var variable = new AnalogItemState(parent)
            {
                BrowseName = new QualifiedName(browseName, NamespaceIndex)
            };

            if (engineeringUnit != null)
            {
                variable.EngineeringUnits = new PropertyState<EUInformation>(variable);
            }

            if (instrumentRange != null)
            {
                variable.InstrumentRange = new PropertyState<Range>(variable);
            }

            if (definition != null)
            {
                variable.Definition = new PropertyState<string>(variable);
            }

            if (valuePrecision != null)
            {
                variable.ValuePrecision = new PropertyState<double>(variable);
            }

            variable.Create(
                SystemContext,
                new NodeId(browseName, NamespaceIndex),
                variable.BrowseName,
                null,
                true);

            if (engineeringUnit != null)
            {
                variable.EngineeringUnits.Value = engineeringUnit;
                variable.EngineeringUnits.AccessLevel = accessLevel;
                variable.EngineeringUnits.UserAccessLevel = accessLevel;
            }

            if (instrumentRange != null)
            {
                variable.InstrumentRange.Value = instrumentRange;
                variable.InstrumentRange.AccessLevel = accessLevel;
                variable.InstrumentRange.UserAccessLevel = accessLevel;
            }

            if (definition != null)
            {
                variable.Definition.Value = definition;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            if (valuePrecision != null)
            {
                variable.ValuePrecision.Value = (double)valuePrecision;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            variable.NodeId = new NodeId(browseName, NamespaceIndex);
            variable.SymbolicName = displayName.ToString();
            variable.DisplayName = displayName;
            variable.Description = description;
            variable.WriteMask = writeMask;
            variable.UserWriteMask = userWriteMask;
            variable.RolePermissions = rolePermissions;
            variable.UserRolePermissions = userRolePermissions;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = accessLevel;
            variable.Historizing = false;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            variable.EURange.Value = euRange ?? new Range(100, 0);
            variable.EURange.AccessLevel = accessLevel;
            variable.EURange.UserAccessLevel = accessLevel;

            variable.Value = initialValue ?? Opc.Ua.TypeInfo.GetDefaultValue(dataType, valueRank, ServerData.TypeTree);

            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            parent?.AddChild(variable);

            return variable;
        }

        /// <summary>Creates a new two state variable.</summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="trueState">
        ///     Defines the string to be associated with this variable when it is TRUE. This is typically used for a contact when
        ///     it is in the closed (non-zero)
        ///     state.
        /// </param>
        /// <param name="falseState">
        ///     Defines the string to be associated with this variable when it is FALSE. This is typically
        ///     used for a contact when it is in the open(zero) state.
        /// </param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="definition">Definition is a vendor-specific, human readable string that specifies how the value of this DataItem is calculated. Definition is non-localized and will often contain an equation that can be parsed by certain clients, e.g. Definition::= "(TempA � 25) + TempB� </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="TwoStateDiscreteState" /></returns>
        protected TwoStateDiscreteState CreateTwoStateDiscreteState(NodeState parent, string browseName,
            LocalizedText displayName,
            LocalizedText description, byte accessLevel, bool initialValue, string trueState, string falseState,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            string definition = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var variable = new TwoStateDiscreteState(parent);

            if (definition != null)
            {
                variable.Definition = new PropertyState<string>(variable);
            }

            variable.Create(
                SystemContext,
                new NodeId(browseName, NamespaceIndex),
                new QualifiedName(browseName, NamespaceIndex),
                displayName,
                true);

            if (definition != null)
            {
                variable.Definition.Value = definition;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            variable.SymbolicName = displayName.ToString();
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.DataType = DataTypeIds.Boolean;
            variable.ValueRank = ValueRanks.Scalar;
            variable.Description = description;
            variable.WriteMask = writeMask;
            variable.UserWriteMask = userWriteMask;
            variable.RolePermissions = rolePermissions;
            variable.UserRolePermissions = userRolePermissions;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = accessLevel;
            variable.Historizing = false;
            variable.Value = initialValue;

            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            variable.TrueState.Value = trueState;
            variable.TrueState.AccessLevel = accessLevel;
            variable.TrueState.UserAccessLevel = accessLevel;

            variable.FalseState.Value = falseState;
            variable.FalseState.AccessLevel = accessLevel;
            variable.FalseState.UserAccessLevel = accessLevel;

            parent?.AddChild(variable);

            return variable;
        }

        /// <summary>Creates a new multi state variable.</summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="definition">Definition is a vendor-specific, human readable string that specifies how the value of this DataItem is calculated. Definition is non-localized and will often contain an equation that can be parsed by certain clients, e.g. Definition::= "(TempA � 25) + TempB� </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <param name="values">The possible values the multi-state variable can have.</param>
        /// <returns>The created <see cref="MultiStateDiscreteState" /></returns>
        protected MultiStateDiscreteState CreateMultiStateDiscreteState(NodeState parent, string browseName,
            LocalizedText displayName,
            LocalizedText description, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            string definition = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null, params LocalizedText[] values)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var variable = new MultiStateDiscreteState(parent);

            if (definition != null)
            {
                variable.Definition = new PropertyState<string>(variable);
            }

            variable.Create(
                SystemContext,
                new NodeId(browseName, NamespaceIndex),
                new QualifiedName(browseName, NamespaceIndex),
                displayName,
                true);

            if (definition != null)
            {
                variable.Definition.Value = definition;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            variable.SymbolicName = displayName.ToString();
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.DataType = DataTypeIds.UInt32;
            variable.ValueRank = ValueRanks.Scalar;
            variable.Description = description;
            variable.WriteMask = writeMask;
            variable.UserWriteMask = userWriteMask;
            variable.RolePermissions = rolePermissions;
            variable.UserRolePermissions = userRolePermissions;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = accessLevel;
            variable.Historizing = false;

            if (initialValue == null)
            {
                variable.Value = (uint)0;
            }
            else
            {
                variable.Value = initialValue;
            }

            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            LocalizedText[] strings = new LocalizedText[values.Length];
            for (int ii = 0; ii < strings.Length; ii++)
            {
                strings[ii] = values[ii];
            }

            variable.EnumStrings.Value = strings;
            variable.EnumStrings.AccessLevel = accessLevel;
            variable.EnumStrings.UserAccessLevel = accessLevel;

            parent?.AddChild(variable);

            return variable;
        }

        /// <summary>Creates a new multi state value variable.</summary>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="displayName">
        ///   <para>The DisplayName Attribute contains the localized name of the Node, e.g. Simple Types. Clients should use this Attribute if they want to display the name of
        /// the Node to the user. They should not use the BrowseName for this purpose.</para>
        ///   <para>The string part of the DisplayName is restricted to 512 characters.</para>
        /// </param>
        /// <param name="description">The optional Description Attribute shall explain the meaning of the Node in a localized text using the same mechanisms for localization as described for the
        /// DisplayName.</param>
        /// <param name="dataType">
        ///     The Node Id of the node used as data type of the new variable.
        /// </param>
        /// <param name="initialValue">The initial value. If null a default value is used as initial value.</param>
        /// <param name="accessLevel">
        ///     The access level of the new variable, e.g. <see cref="AccessLevels.CurrentRead" />. See
        ///     <see cref="AccessLevels" /> for all possible access levels.
        /// </param>
        /// <param name="writeMask">
        ///   <para>The optional WriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node. The WriteMask Attribute does not take any user
        /// access rights into account, that is, although an Attribute is writable this may be restricted to a certain user/user group.</para>
        ///   <para>If the OPC UA Server does not have the ability to get the WriteMask information for a specific Attribute from the underlying system, it should state that it
        /// is writable. If a write operation is called on the Attribute, the Server should transfer this request and return the corresponding StatusCode if such a request
        /// is rejected. StatusCodes are defined in OPC 10000-4.<br /></para>
        /// </param>
        /// <param name="userWriteMask">
        ///   <para>The optional UserWriteMask Attribute exposes the possibilities of a client to write the Attributes of the Node taking user access rights into account. It
        /// uses the AttributeWriteMask DataType which is defined in 0.</para>
        ///   <para>The UserWriteMask Attribute can only further restrict the WriteMask Attribute, when it is set to not writable in the general case that applies for every
        /// user.</para>
        ///   <para>Clients cannot assume an Attribute can be written based on the UserWriteMask Attribute.It is possible that the Server may return an access denied error due
        /// to some server specific change which was not reflected in the state of this Attribute at the time the Client accessed it.</para>
        /// </param>
        /// <param name="definition">Definition is a vendor-specific, human readable string that specifies how the value of this DataItem is calculated. Definition is non-localized and will often contain an equation that can be parsed by certain clients, e.g. Definition::= "(TempA � 25) + TempB� </param>
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <param name="enumNames">The possible values the multi-state variable can have.</param>
        /// <returns>The created <see cref="MultiStateDiscreteState" /></returns>
        protected MultiStateValueDiscreteState CreateMultiStateValueDiscreteState(NodeState parent,
            string browseName, LocalizedText displayName,
            LocalizedText description, NodeId dataType, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            string definition = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null, params LocalizedText[] enumNames)
        {
            if (displayName == null)
            {
                displayName = new LocalizedText("");
            }

            if (description == null)
            {
                description = new LocalizedText("");
            }

            if (rolePermissions == null)
            {
                rolePermissions = new RolePermissionTypeCollection();
            }

            if (userRolePermissions == null)
            {
                userRolePermissions = new RolePermissionTypeCollection();
            }

            var variable = new MultiStateValueDiscreteState(parent);

            if (definition != null)
            {
                variable.Definition = new PropertyState<string>(variable);
            }

            variable.Create(
                SystemContext,
                new NodeId(browseName, NamespaceIndex),
                new QualifiedName(browseName, NamespaceIndex),
                displayName,
                true);

            if (definition != null)
            {
                variable.Definition.Value = definition;
                variable.ValuePrecision.AccessLevel = accessLevel;
                variable.ValuePrecision.UserAccessLevel = accessLevel;
            }

            variable.SymbolicName = displayName.ToString();
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.DataType = dataType == null ? DataTypeIds.UInt32 : dataType;
            variable.ValueRank = ValueRanks.Scalar;
            variable.Description = description;
            variable.WriteMask = writeMask;
            variable.UserWriteMask = userWriteMask;
            variable.RolePermissions = rolePermissions;
            variable.UserRolePermissions = userRolePermissions;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = accessLevel;
            variable.Historizing = false;

            if (initialValue == null)
            {
                variable.Value = (uint)0;
            }
            else
            {
                variable.Value = initialValue;
            }

            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            // there are two enumerations for this type:
            // EnumStrings = the string representations for enumerated values
            // ValueAsText = the actual enumerated value

            // set the enumerated strings
            LocalizedText[] strings = new LocalizedText[enumNames.Length];
            for (int ii = 0; ii < strings.Length; ii++)
            {
                strings[ii] = enumNames[ii];
            }

            // set the enumerated values
            if (enumNames != null)
            {
                var values = new EnumValueType[enumNames.Length];
                for (var ii = 0; ii < values.Length; ii++)
                {
                    values[ii] = new EnumValueType
                    {
                        Value = ii,
                        Description = strings[ii],
                        DisplayName = strings[ii]
                    };
                }

                variable.EnumValues.Value = values;
            }

            variable.EnumValues.AccessLevel = accessLevel;
            variable.EnumValues.UserAccessLevel = accessLevel;
            variable.ValueAsText.Value = variable.EnumValues.Value[0].DisplayName;

            parent?.AddChild(variable);

            return variable;
        }
        #endregion

        #region Method related functions
        /// <summary>
        ///     <para>Creates a new method.</para>
        ///     <para>
        ///         Nodes of the type Method represent a method, that is, something that is called by a client and returns a
        ///         result.
        ///     </para>
        /// </summary>
        /// <param name="parent">The parent object the new method will be created in.</param>
        /// <param name="path">
        ///     The unique path name for the variable in the server's address space.
        /// </param>
        /// <param name="name">
        ///     The name of the new method, e.g. <font color="#A31515" size="2" face="Consolas">Method1</font>
        /// </param>
        /// <param name="callingMethod">The method which will be called if the method is executed.</param>
        /// <returns>The created method object.</returns>
        protected MethodState CreateMethodState(NodeState parent, string path, string name,
            GenericMethodCalledEventHandler2 callingMethod = null)
        {
            var method = new MethodState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                Executable = true,
                UserExecutable = true
            };

            parent?.AddChild(method);

            if (callingMethod != null)
            {
                method.OnCallMethod2 = callingMethod;
            }

            return method;
        }

        /// <summary>Creates a new argument.</summary>
        /// <param name="name">
        ///     The name of the new argument, e.g.
        ///     <font color="#A31515" size="2" face="Consolas">Initial State</font>
        /// </param>
        /// <param name="description">
        ///     The description of the new argument, e.g.
        ///     <font color="#A31515" size="2" face="Consolas">The initialize state for the process.</font>
        /// </param>
        /// <param name="dataType">
        ///     The data type of the new argument, e.g. <see cref="BuiltInType.SByte" />. See
        ///     <see cref="BuiltInType" /> for all possible types
        /// </param>
        /// <param name="valueRank">
        ///     The value rank of the new argument, e.g. <see cref="ValueRanks.Scalar" />. See
        ///     <see cref="ValueRanks" /> for all possible value ranks.
        /// </param>
        /// <returns>The created argument</returns>
        protected Argument CreateArgument(string name, string description, BuiltInType dataType, int valueRank)
        {
            var argument = new Argument
                { Name = name, Description = description, DataType = (uint)dataType, ValueRank = valueRank };

            return argument;
        }

        /// <summary>Adds the input arguments to a method.</summary>
        /// <param name="parent">The method object.</param>
        /// <param name="inputArguments">The input arguments.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        protected StatusCode AddInputArguments(MethodState parent, Argument[] inputArguments)
        {
            if (parent != null)
            {
                parent.InputArguments = new PropertyState<Argument[]>(parent);
                parent.InputArguments.NodeId = new NodeId(parent.BrowseName.Name + "InArgs", NamespaceIndex);
                parent.InputArguments.BrowseName = BrowseNames.InputArguments;
                parent.InputArguments.DisplayName = parent.InputArguments.BrowseName.Name;
                parent.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                parent.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                parent.InputArguments.DataType = DataTypeIds.Argument;
                parent.InputArguments.ValueRank = ValueRanks.OneDimension;
                parent.InputArguments.Value = inputArguments;

                return StatusCodes.Good;
            }

            return StatusCodes.Bad;
        }

        /// <summary>Adds the output arguments to a method.</summary>
        /// <param name="parent">The method object.</param>
        /// <param name="outputArguments">The output arguments.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        protected StatusCode AddOutputArguments(MethodState parent, params Argument[] outputArguments)
        {
            if (parent != null)
            {
                parent.OutputArguments = new PropertyState<Argument[]>(parent);
                parent.OutputArguments.NodeId = new NodeId(parent.BrowseName.Name + "OutArgs", NamespaceIndex);
                parent.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                parent.OutputArguments.DisplayName = parent.OutputArguments.BrowseName.Name;
                parent.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                parent.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                parent.OutputArguments.DataType = DataTypeIds.Argument;
                parent.OutputArguments.ValueRank = ValueRanks.OneDimension;
                parent.OutputArguments.Value = outputArguments;

                return StatusCodes.Good;
            }

            return StatusCodes.Bad;
        }
        #endregion

        #region Random value generator
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="boundaryValueFrequency"></param>
        protected void ResetRandomGenerator(int seed, int boundaryValueFrequency = 0)
        {
            randomSource_ = new RandomSource(seed);
            generator_ = new DataGenerator(randomSource_);
            generator_.BoundaryValueFrequency = boundaryValueFrequency;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        protected object GetNewValue(BaseVariableState variable)
        {
            Debug.Assert(generator_ != null, "Need a random generator!");

            object value = null;
            int retryCount = 0;

            while (value == null && retryCount < 10)
            {
                value = generator_.GetRandom(variable.DataType, variable.ValueRank, new uint[] { 10 }, ServerData.TypeTree);
                // skip Variant Null
                if (value is Variant variant)
                {
                    if (variant.Value == null)
                    {
                        value = null;
                    }
                }
                retryCount++;
            }

            return value;
        }
        #endregion

        #region Private Fields
        private RandomSource randomSource_;
        private DataGenerator generator_;
        #endregion
    }
}