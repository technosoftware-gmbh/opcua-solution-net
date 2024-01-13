#region Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Subscriptions;
#endregion

namespace Technosoftware.UaBaseServer
{
    /// <summary>
    ///     <para>OPC UA Server Interface</para>
    ///     <para>This interface defines the generic server interface.</para>
    ///     <para>
    ///         The IUaServer interface provides a set of generic server callback methods. These methods can be used to read
    ///         information from the generic server or change
    ///         data in the generic server. They are always called by the customization plugin.
    ///     </para>
    ///     <para>It also defines classes and enumerators used in the data exchange with the generic server</para>
    /// </summary>
    public interface IUaServer
    {
        #region Properties
        /// <summary>The licensed product (Evaluation, Server, Client or Bundle).</summary>
        string Product { get; }

        /// <summary>
        ///     Indicates whether the server must be restarted. This is mainly the case if the server is used in evaluation mode
        ///     and the 90 minutes evaluation time expired.
        /// </summary>
        bool RestartRequired { get; }

        /// <summary>
        ///     The default context to use.
        /// </summary>
        UaServerContext SystemContext { get; }

        /// <summary>
        ///     Gets the namespace indexes owned by the node manager.
        /// </summary>
        /// <value>The namespace indexes.</value>
        ushort[] NamespaceIndexes { get; }

        /// <summary>
        ///     The table of namespace uris known to the server.
        /// </summary>
        /// <value>The namespace URIs.</value>
        NamespaceTable NamespaceUris { get; }

        /// <summary>
        ///     Get the node manager.
        /// </summary>
        IUaNodeManager NodeManager { get; }
        #endregion

        #region General Methods (not related to an OPC specification)
        /// <summary>
        ///     Returns the current state of the server.
        /// </summary>
        /// <returns>Returns the current state of the server.</returns>
        ServerState CurrentState { get; }

        /// <summary>Requests a shutdown of the server.</summary>
        /// <returns>
        ///     A <see cref="StatusCodes" /> code with the result of the operation. Returning an error code indicates that the
        ///     server can't be stopped.
        /// </returns>
        StatusCode Shutdown();

        /// <summary>Returns information about the endpoints supported by the server.</summary>
        /// <returns>Returns a list of Endpoint information's.</returns>
        IList<EndpointDescription> GetEndpoints();

        /// <summary>
        ///     Returns all of the sessions known to the session manager.
        /// </summary>
        /// <returns>A list of the sessions.</returns>
        IList<Session> GetSessions();

        /// <summary>
        ///     Returns all of the subscriptions known to the server.
        /// </summary>
        /// <returns>A list of the subscriptions.</returns>
        IList<Subscription> GetSubscriptions();
        #endregion

        #region Core Server Facet related methods
        /// <summary>
        ///     Loads a node set from a resource and add them to the set of predefined nodes.
        /// </summary>
        /// <param name="resourceName">The name of the resource to be loaded</param>
        /// <param name="externalReferences">
        ///     <para>The externalReferences is an out parameter that allows the generic server to link to nodes.</para>
        /// </param>
        void LoadNodes(string resourceName, object externalReferences);

        /// <summary>
        ///     Returns the state object for the specified node if it exists.
        /// </summary>
        NodeState FindNode(NodeId nodeId);

        /// <summary>
        ///     Finds the specified node and checks if it is of the expected type.
        /// </summary>
        /// <param name="nodeId">The node to search for.</param>
        /// <param name="expectedType">The expected type of the node.</param>
        /// <returns>Returns null if not found or not of the correct type.</returns>
        NodeState FindNode(NodeId nodeId, Type expectedType);

        /// <summary>
        ///     Finds a node in the dynamic cache.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="handle">The handle of the node to validate.</param>
        /// <param name="cache">The cached nodes</param>
        /// <returns>Returns null if not found or the node object.</returns>
        NodeState FindNode(UaServerContext context, UaNodeHandle handle, IDictionary<NodeId, NodeState> cache);

        /// <summary>
        ///     Recursively indexes the node and its children and adds it to the predefined nodes
        /// </summary>
        /// <param name="node">The node to be added.</param>
        void AddNode(NodeState node);

        /// <summary>
        ///     Deletes a node and all of its children.
        /// </summary>
        /// <param name="nodeId">The node Id of the node to be removed.</param>
        bool DeleteNode(NodeId nodeId);
        #endregion

        #region Create Node Classes
        /// <summary>
        ///   <para>Creates a new folder.</para>
        ///   <para>Folders are used to organize the AddressSpace into a hierarchy of Nodes. They represent the root Node of a subtree, and have no other semantics associated
        /// with them.</para>
        /// </summary>
        /// <param name="browseName">Nodes have a BrowseName Attribute that is used as a non-localized human-readable name when browsing the AddressSpace to create paths out of BrowseNames. The
        /// TranslateBrowsePathsToNodeIds Service defined in OPC 10000-4 can be used to follow a path constructed of BrowseNames, e.g. /Static/Simple Types</param>
        /// <param name="parent">The parent NodeState object the new folder will be created in.</param>
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
        /// <returns>The created folder object which can be used in further calls to <see cref="CreateFolderState(NodeState,string,LocalizedText,LocalizedText,AttributeWriteMask,AttributeWriteMask,RolePermissionTypeCollection,RolePermissionTypeCollection)" />.</returns>
        FolderState CreateFolderState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        BaseObjectState CreateBaseObjectState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        // ReSharper disable once UnusedMember.Global
        PropertyState CreatePropertyState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel,
            object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName,
            LocalizedText displayName, LocalizedText description, NodeId dataType, int valueRank, byte accessLevel,
            object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, ExpandedNodeId dataType, int valueRank, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

        /// <summary>Writes to a variable.</summary>
        /// <param name="deviceItem">The <see cref="BaseVariableState" /> including the identifier.</param>
        /// <param name="newValue">Object with new item value.</param>
        /// <param name="statusCode">New status code of the item value.</param>
        /// <param name="timestamp">New timestamp of the new item value.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        StatusCode WriteBaseVariable(BaseVariableState deviceItem, object newValue, StatusCode statusCode, DateTime timestamp);
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
        DataItemState CreateDataItemState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        AnalogItemState CreateAnalogItemState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            // ReSharper disable once RedundantNameQualifier
            // ReSharper disable once RedundantNameQualifier
            Opc.Ua.Range euRange, EUInformation engineeringUnit = null, Opc.Ua.Range instrumentRange = null,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="TwoStateDiscreteState" /></returns>
        TwoStateDiscreteState CreateTwoStateDiscreteState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, byte accessLevel, bool initialValue, string trueState, string falseState, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null);

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
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <param name="values">The possible values the multi-state variable can have.</param>
        /// <returns>The created <see cref="MultiStateDiscreteState" /></returns>
        // ReSharper disable once UnusedMember.Global
        MultiStateDiscreteState CreateMultiStateDiscreteState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null, params LocalizedText[] values);
        #endregion

        #region Method Server Facet related methods
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
        Argument CreateArgument(string name, string description, BuiltInType dataType, int valueRank);

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
        MethodState CreateMethodState(BaseObjectState parent, string path, string name,
            GenericMethodCalledEventHandler2 callingMethod = null);

        /// <summary>Adds the input arguments to a method.</summary>
        /// <param name="parent">The method object.</param>
        /// <param name="inputArguments">The input arguments.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        StatusCode AddInputArguments(MethodState parent,params Argument[] inputArguments);

        /// <summary>Adds the output arguments to a method.</summary>
        /// <param name="parent">The method object.</param>
        /// <param name="outputArguments">The output arguments.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        StatusCode AddOutputArguments(MethodState parent, params Argument[] outputArguments);
        #endregion

        #region Events related methods
        /// <summary>
        ///     Adds a root notifier.
        /// </summary>
        /// <param name="notifier">The notifier.</param>
        /// <remarks>
        ///     A root notifier is a notifier owned by the NodeManager that is not the target of a
        ///     HasNotifier reference. These nodes need to be linked directly to the Server object.
        /// </remarks>
        void AddRootNotifier(NodeState notifier);

        /// <summary>
        ///     Removes a root notifier previously added with AddRootNotifier.
        /// </summary>
        /// <param name="notifier">The notifier.</param>
        /// <remarks>
        ///     A root notifier is a notifier owned by the NodeManager that is not the target of a
        ///     HasNotifier reference. These nodes need to be linked directly to the Server object.
        /// </remarks>
        void RemoveRootNotifier(NodeState notifier);

        /// <summary>
        ///     Can be called to report a global event.
        /// </summary>
        /// <param name="e">The event.</param>
        void ReportEvent(IFilterTarget e);
        #endregion

        #region Testing / Simulation related Methods
        /// <summary>Generates a simulated value for each of the supported data types.</summary>
        /// <param name="deviceItem">The <see cref="BaseVariableState" /> a new value should generated for.</param>
        /// <returns>The new value for the specified <see cref="BaseVariableState" /></returns>
        /// <example>
        ///     <para>
        ///         <font color="blue" size="2" face="Consolas">
        ///             <font color="blue" size="2" face="Consolas">
        ///                 <font color="blue" size="2" face="Consolas">foreach</font>
        ///             </font>
        ///         </font>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">(</font>
        ///         </font>
        ///         <font color="#2B91AF" size="2" face="Consolas">
        ///             <font color="#2B91AF" size="2" face="Consolas">
        ///                 <font color="#2B91AF" size="2" face="Consolas">BaseVariableState</font>
        ///             </font>
        ///         </font>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">deviceItem</font>
        ///         </font>
        ///         <font color="blue" size="2" face="Consolas">
        ///             <font color="blue" size="2" face="Consolas">
        ///                 <font color="blue" size="2" face="Consolas">in</font>
        ///             </font>
        ///         </font>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">_dynamicNodes)</font>
        ///         </font>
        ///     </para>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">{</font>
        ///         </font>
        ///     </para>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">
        ///                 deviceItem.Value =
        ///                 _opcServer.GetNewSimulatedValue(deviceItem);
        ///             </font>
        ///         </font>
        ///     </para>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">    deviceItem.StatusCode =</font>
        ///         </font>
        ///         <font color="#2B91AF" size="2" face="Consolas">
        ///             <font color="#2B91AF" size="2" face="Consolas">
        ///                 <font color="#2B91AF" size="2" face="Consolas">StatusCodes</font>
        ///             </font>
        ///         </font>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">.Good;</font>
        ///         </font>
        ///     </para>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">    deviceItem.Timestamp =</font>
        ///         </font>
        ///         <font color="#2B91AF" size="2" face="Consolas">
        ///             <font color="#2B91AF" size="2" face="Consolas">
        ///                 <font color="#2B91AF" size="2" face="Consolas">DateTime</font>
        ///             </font>
        ///         </font>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">.UtcNow;</font>
        ///         </font>
        ///     </para>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">    _opcServer.WriteVariable(deviceItem);</font>
        ///         </font>
        ///     </para>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">}</font>
        ///         </font>
        ///     </para>
        ///     <code title="" description="" id="06504386-1408-4995-807a-6c6f6d174c14" lang="neutral"></code>
        /// </example>
        // ReSharper disable once UnusedMember.Global
        object GetNewSimulatedValue(BaseVariableState deviceItem);
        #endregion
    }
}