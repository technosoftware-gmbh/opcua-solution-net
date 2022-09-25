#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Test;

using Technosoftware.UaServer;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Server;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Subscriptions;
#endregion

namespace Technosoftware.ServerBase
{
    /// <summary>
    /// The standard implementation of an OPC UA server with reverse connect.
    /// </summary>
    /// <remarks>
    ///     Each server instance must have one instance of a UaBaseServer object which is
    ///     responsible for reading the configuration file, creating the endpoints and dispatching
    ///     incoming requests to the appropriate handler.
    /// 
    ///     This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the NodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class UaBaseServer : GenericServer, IUaServer
    {
        #region Default Values
        /// <summary>
        /// The default reverse connect interval.
        /// </summary>
        public static int DefaultReverseConnectInterval => 15000;

        /// <summary>
        /// The default reverse connect timeout.
        /// </summary>
        public static int DefaultReverseConnectTimeout => 30000;

        /// <summary>
        /// The default timeout after a rejected connection attempt.
        /// </summary>
        public static int DefaultReverseConnectRejectTimeout => 60000;
        #endregion

        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a reverse connect server based on a GenericServer.
        /// </summary>
        public UaBaseServer()
        {
            UaServerPlugin = null;
            UseReverseConnect = false;

            if (dataGenerator_ == null)
            {
                dataGenerator_ = new DataGenerator(null) { BoundaryValueFrequency = 0 };
            }

            connectInterval_ = DefaultReverseConnectInterval;
            connectTimeout_ = DefaultReverseConnectTimeout;
            rejectTimeout_ = DefaultReverseConnectRejectTimeout;
            connections_ = new Dictionary<Uri, UaReverseConnectProperty>();
        }

        /// <summary>
        ///     Initializes the object with default values.
        /// </summary>
        public UaBaseServer(IUaServerPlugin opcServerPlugin, bool useReverseConnect = false)
        {
            UaServerPlugin = opcServerPlugin;
            UseReverseConnect = useReverseConnect;

            if (dataGenerator_ == null)
            {
                dataGenerator_ = new DataGenerator(null) { BoundaryValueFrequency = 0 };
            }

            connectInterval_ = DefaultReverseConnectInterval;
            connectTimeout_ = DefaultReverseConnectTimeout;
            rejectTimeout_ = DefaultReverseConnectRejectTimeout;
            connections_ = new Dictionary<Uri, UaReverseConnectProperty>();
        }
        #endregion

        #region GenericServer overrides
        /// <inheritdoc/>
        protected override void OnServerStarted(IUaServerData server)
        {
            base.OnServerStarted(server);

            UpdateConfiguration(base.Configuration);
            StartTimer(true);
        }

        /// <inheritdoc />
        protected override void OnUpdateConfiguration(ApplicationConfiguration configuration)
        {
            base.OnUpdateConfiguration(configuration);
            UpdateConfiguration(configuration);
        }

        /// <inheritdoc />
        protected override void OnServerStopping()
        {
            DisposeTimer();
            base.OnServerStopping();
        }
        #endregion

        #region Public Methods (reverse connect related)
        /// <summary>
        /// Add a reverse connection url.
        /// </summary>
        public virtual void AddReverseConnection(Uri url, int timeout = 0, int maxSessionCount = 0, bool enabled = true)
        {
            if (connections_.ContainsKey(url))
            {
                throw new ArgumentException("Connection for specified clientUrl is already configured", nameof(url));
            }
            else
            {
                var reverseConnection = new UaReverseConnectProperty(url, timeout, maxSessionCount, false, enabled);
                lock (connectionsLock_)
                {
                    connections_[url] = reverseConnection;
                    Utils.LogInfo("Reverse Connection added for EndpointUrl: {0}.", url);

                    StartTimer(false);
                }
            }
        }

        /// <summary>
        /// Remove a reverse connection url.
        /// </summary>
        /// <returns>true if the reverse connection is found and removed</returns>
        public virtual bool RemoveReverseConnection(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            lock (connectionsLock_)
            {
                var connectionRemoved = connections_.Remove(url);

                if (connectionRemoved)
                {
                    Utils.LogInfo("Reverse Connection removed for EndpointUrl: {0}.", url);
                }

                if (connections_.Count == 0)
                {
                    DisposeTimer();
                }

                return connectionRemoved;
            }
        }

        /// <summary>
        /// Return a dictionary of configured reverse connection Urls.
        /// </summary>
        public virtual ReadOnlyDictionary<Uri, UaReverseConnectProperty> GetReverseConnections()
        {
            lock (connections_)
            {
                return new ReadOnlyDictionary<Uri, UaReverseConnectProperty>(connections_);
            }
        }
        #endregion

        #region Private Properties (reverse connect related)
        /// <summary>
        /// Timer callback to establish new reverse connections.
        /// </summary>
        private void OnReverseConnect(object state)
        {
            try
            {
                lock (connectionsLock_)
                {
                    foreach (var reverseConnection in connections_.Values)
                    {
                        // recharge a rejected connection after timeout
                        if (reverseConnection.LastState == UaReverseConnectState.Rejected &&
                            reverseConnection.RejectTime + TimeSpan.FromMilliseconds(rejectTimeout_) < DateTime.UtcNow)
                        {
                            reverseConnection.LastState = UaReverseConnectState.Closed;
                        }

                        // try the reverse connect
                        if ((reverseConnection.Enabled) &&
                            (reverseConnection.MaxSessionCount == 0 ||
                            (reverseConnection.MaxSessionCount == 1 && reverseConnection.LastState == UaReverseConnectState.Closed) ||
                             reverseConnection.MaxSessionCount > ServerData.SessionManager.GetSessions().Count))
                        {
                            try
                            {
                                reverseConnection.LastState = UaReverseConnectState.Connecting;
                                base.CreateConnection(reverseConnection.ClientUrl,
                                    reverseConnection.Timeout > 0 ? reverseConnection.Timeout : connectTimeout_);
                                Utils.LogInfo("Create Connection! [{0}][{1}]", reverseConnection.LastState, reverseConnection.ClientUrl);
                            }
                            catch (Exception e)
                            {
                                reverseConnection.LastState = UaReverseConnectState.Errored;
                                reverseConnection.ServiceResult = new ServiceResult(e);
                                Utils.LogError("Create Connection failed! [{0}][{1}]",
                                    reverseConnection.LastState, reverseConnection.ClientUrl);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex, "OnReverseConnect unexpected error: {0}", ex.Message);
            }
            finally
            {
                StartTimer(true);
            }
        }

        /// <summary>
        /// Track reverse connection status.
        /// </summary>
        protected override void OnConnectionStatusChanged(object sender, ConnectionStatusEventArgs e)
        {
            lock (connectionsLock_)
            {
                UaReverseConnectProperty reverseConnection = null;
                if (connections_.TryGetValue(e.EndpointUrl, out reverseConnection))
                {
                    var priorStatus = reverseConnection.ServiceResult;
                    if (ServiceResult.IsBad(e.ChannelStatus))
                    {
                        reverseConnection.ServiceResult = e.ChannelStatus;
                        if (e.ChannelStatus.Code == StatusCodes.BadTcpMessageTypeInvalid)
                        {
                            reverseConnection.LastState = UaReverseConnectState.Rejected;
                            reverseConnection.RejectTime = DateTime.UtcNow;
                            Utils.LogWarning("Client Rejected Connection! [{0}][{1}]", reverseConnection.LastState, e.EndpointUrl);
                            return;
                        }
                        else
                        {
                            reverseConnection.LastState = UaReverseConnectState.Closed;
                            Utils.LogError("Connection Error! [{0}][{1}]", reverseConnection.LastState, e.EndpointUrl);
                            return;
                        }
                    }
                    reverseConnection.LastState = e.Closed ? UaReverseConnectState.Closed : UaReverseConnectState.Connected;
                    Utils.LogInfo("New Connection State! [{0}][{1}]", reverseConnection.LastState, e.EndpointUrl);
                }
                else
                {
                    Utils.LogWarning("Warning: Status changed for unknown reverse connection: [{0}][{1}]",
                        e.ChannelStatus, e.EndpointUrl);
                }
            }

            base.OnConnectionStatusChanged(sender, e);
        }

        /// <summary>
        /// Restart the timer. 
        /// </summary>
        private void StartTimer(bool forceRestart)
        {
            if (forceRestart)
            {
                DisposeTimer();
            }
            lock (connectionsLock_)
            {
                if (connectInterval_ > 0 &&
                    connections_.Count > 0 &&
                    reverseConnectTimer_ == null)
                {
                    reverseConnectTimer_ = new Timer(OnReverseConnect, this, forceRestart ? connectInterval_ : 1000, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Dispose the current timer.
        /// </summary>
        private void DisposeTimer()
        {
            // start registration timer.
            lock (connectionsLock_)
            {
                if (reverseConnectTimer_ != null)
                {
                    Utils.SilentDispose(reverseConnectTimer_);
                    reverseConnectTimer_ = null;
                }
            }
        }

        /// <summary>
        /// Remove a reverse connection url.
        /// </summary>
        private void ClearConnections(bool configEntry)
        {
            lock (connectionsLock_)
            {
                var toRemove = connections_.Where(r => r.Value.ConfigEntry == configEntry);
                foreach (var entry in toRemove)
                {
                    connections_.Remove(entry.Key);
                }
            }
        }

        /// <summary>
        /// Update the reverse connect configuration from the application configuration.
        /// </summary>
        private void UpdateConfiguration(ApplicationConfiguration configuration)
        {
            ClearConnections(true);

            // get the configuration for the reverse connections.
            var reverseConnect = configuration?.ServerConfiguration?.ReverseConnect;

            // add configuration reverse client connection properties.
            if (reverseConnect != null)
            {
                lock (connectionsLock_)
                {
                    connectInterval_ = reverseConnect.ConnectInterval > 0 ? reverseConnect.ConnectInterval : DefaultReverseConnectInterval;
                    connectTimeout_ = reverseConnect.ConnectTimeout > 0 ? reverseConnect.ConnectTimeout : DefaultReverseConnectTimeout;
                    rejectTimeout_ = reverseConnect.RejectTimeout > 0 ? reverseConnect.RejectTimeout : DefaultReverseConnectRejectTimeout;
                    if (reverseConnect.Clients != null)
                    {
                        foreach (var client in reverseConnect.Clients)
                        {
                            var uri = Utils.ParseUri(client.EndpointUrl);
                            if (uri != null)
                            {
                                if (connections_.ContainsKey(uri))
                                {
                                    Utils.LogWarning("Warning: ServerConfiguration.ReverseConnect contains duplicate EndpointUrl: {0}.", uri);
                                }
                                else
                                {
                                    connections_[uri] = new UaReverseConnectProperty(uri, client.Timeout, client.MaxSessionCount, true, client.Enabled);
                                    Utils.LogInfo("Reverse Connection added for EndpointUrl: {0}.", uri);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        ///     The Application property is only used during beta phase of the product. It will be removed with the release of the
        ///     product, so don't use it.
        /// </summary>
        public object Application { get; set; }

        /// <summary>
        /// </summary>
        public IUaServerPlugin UaServerPlugin { get; set; }

        /// <summary>
        ///     Get the node manager.
        /// </summary>
        public IUaNodeManager NodeManager => uaServerNodeManager_;

        /// <summary>The product the license was issued for.</summary>
        public string Product { get; set; }

        /// <summary>
        ///     Indicates whether the server must be restarted. This is mainly the case if the server is used in evaluation mode
        ///     and the 90 minutes evaluation time expired.
        /// </summary>
        public bool RestartRequired => Opc.Ua.LicenseHandler.IsExpired;

        /// <summary>
        ///     The default context to use.
        /// </summary>
        public UaServerContext SystemContext => uaServerNodeManager_.SystemContext;

        /// <summary>
        ///     Gets the namespace indexes owned by the node manager.
        /// </summary>
        /// <value>The namespace indexes.</value>
        public ushort[] NamespaceIndexes => uaServerNodeManager_.NamespaceIndexes;

        /// <summary>
        ///     The table of namespace uris known to the server.
        /// </summary>
        /// <value>The namespace URIs.</value>
        public NamespaceTable NamespaceUris => uaServerNodeManager_.ServerData.NamespaceUris;

        /// <summary>
        ///     Indicates whether the server should use reverse connect or not.
        /// </summary>
        public bool UseReverseConnect { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds all encodeable types to the server.
        /// </summary>
        /// <param name="uaServerData">The uaServerData data implementing the IUaServerData interface.</param>
        public virtual void AddEncodeableTypes(IUaServerData uaServerData)
        {
            // add the types defined in the information model library to the factory.
            uaServerData.Factory.AddEncodeableTypes(GetType().GetTypeInfo().Assembly);
        }
        #endregion

        #region Overridden Methods

        /// <summary>
        ///     Creates the node managers for the server.
        /// </summary>
        /// <remarks>
        ///     This method allows the sub-class create any additional node managers which it uses. The SDK
        ///     always creates a CoreNodeManager which handles the built-in nodes defined by the specification.
        ///     Any additional NodeManagers are expected to handle application specific nodes.
        /// </remarks>
        protected override MasterNodeManager CreateMasterNodeManager(IUaServerData server,
            ApplicationConfiguration configuration)
        {
            Utils.LogInfo(Utils.TraceMasks.StartStop, "Creating the Reference Server Node Manager.");

            var nodeManagers = new List<IUaBaseNodeManager>();
            uaServerNodeManager_ = null;

            var namespaceUris = UaServerPlugin.OnGetNamespaceUris();

            if (UaServerPlugin is IUaOptionalServerPlugin uaServerOptionalPlugin)
            {
                uaServerNodeManager_ = uaServerOptionalPlugin.OnGetNodeManager(this, server, configuration, namespaceUris);
            }
            if (uaServerNodeManager_ == null)
            {
                // create the custom node managers.
                uaServerNodeManager_ = new UaBaseNodeManager(this, UaServerPlugin, server, configuration, namespaceUris);
            }

            AddEncodeableTypes(server);
            
            nodeManagers.Add(uaServerNodeManager_);

            UaServerPlugin.OnInitialized(this, configuration);

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        /// <summary>
        ///     Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        ///     These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            var opcServerProperties = UaServerPlugin.OnGetServerProperties();

            var properties = new ServerProperties
            {
                ManufacturerName = opcServerProperties.ManufacturerName,
                ProductName = opcServerProperties.ProductName,
                ProductUri = opcServerProperties.ProductUri,
                SoftwareVersion = opcServerProperties.SoftwareVersion,
                BuildNumber = opcServerProperties.BuildNumber,
                BuildDate = opcServerProperties.BuildDate
            };

            // TBD - All applications have software certificates that need to added to the properties.

            return properties;
        }

        /// <summary>
        ///     Creates the resource manager for the server.
        /// </summary>
        protected override ResourceManager CreateResourceManager(IUaServerData server,
            ApplicationConfiguration configuration)
        {
            var resourceManager = new ResourceManager(server, configuration);

            var fields = typeof(StatusCodes).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.GetValue(typeof(StatusCodes)) is uint id)
                {
                    resourceManager.Add(id, "en-US", field.Name);
                }
            }

            return resourceManager;
        }

        /// <summary>
        ///     Verifies that the request header is valid.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <returns></returns>
        protected override UaServerOperationContext ValidateRequest(RequestHeader requestHeader, RequestType requestType)
        {
            var context = base.ValidateRequest(requestHeader, requestType);
            OnValidateOperationRequest(context, requestHeader, requestType);
            return context;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestHeader"></param>
        /// <param name="requestType"></param>
        public virtual void OnValidateOperationRequest(UaServerOperationContext context, RequestHeader requestHeader,
            RequestType requestType)
        {
        }

        /// <summary>
        ///     Verifies that the request header is valid.
        /// </summary>
        /// <param name="context">The operation context.</param>
        protected override void OnRequestComplete(UaServerOperationContext context)
        {
            OnOperationRequestComplete(context);

            base.OnRequestComplete(context);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnOperationRequestComplete(UaServerOperationContext context)
        {
        }

        #endregion

        #region General Methods (not related to an OPC specification)

        /// <summary>Requests a shutdown of the server.</summary>
        /// <returns>
        ///     A <see cref="StatusCodes" /> code with the result of the operation. Returning an error code indicates that the
        ///     server can't be stopped.
        /// </returns>
        public StatusCode Shutdown()
        {
            Stop();
            return StatusCodes.Good;
        }

        /// <summary>
        ///     Returns information about the endpoint's supported by the server.
        /// </summary>
        /// <returns>Returns a list of Endpoint information's.</returns>
        public new IList<EndpointDescription> GetEndpoints()
        {
            return base.GetEndpoints().ToList();
        }

        /// <summary>
        ///     Returns all of the sessions known to the session manager.
        /// </summary>
        /// <returns>A list of the sessions.</returns>
        public IList<Session> GetSessions()
        {
            var opcSessions = new List<Session>();

            var sessions = this.ServerData.SessionManager.GetSessions();

            foreach (var session in sessions)
            {
                lock (session.DiagnosticsLock)
                {
                    opcSessions.Add(session);
                }
            }

            return opcSessions;
        }

        /// <summary>
        ///     Returns all of the subscriptions known to the server.
        /// </summary>
        /// <returns>A list of the subscriptions.</returns>
        public IList<Subscription> GetSubscriptions()
        {
            return this.ServerData.SubscriptionManager.GetSubscriptions();
        }

        /// <summary>
        ///     Returns the current state of the server.
        /// </summary>
        /// <returns>Returns the current state of the server.</returns>
        public ServerState CurrentState
        {
            get
            {
                switch (this.ServerData.CurrentState)
                {
                    case ServerState.Running:
                        return ServerState.Running;
                    case ServerState.Failed:
                        return ServerState.Failed;
                    case ServerState.NoConfiguration:
                        return ServerState.NoConfiguration;
                    case ServerState.Suspended:
                        return ServerState.Suspended;
                    case ServerState.Shutdown:
                        return ServerState.Shutdown;
                    case ServerState.Test:
                        return ServerState.Test;
                    case ServerState.CommunicationFault:
                        return ServerState.CommunicationFault;
                    case ServerState.Unknown:
                        return ServerState.Unknown;
                    default:
                        return ServerState.Unknown;
                }
            }
        }

        #endregion

        #region Standard UA Server related methods
        /// <summary>
        ///     Loads a node set from a resource and add them to the set of predefined nodes.
        /// </summary>
        public void LoadNodes(string resourceName, object externalReferences)
        {
            var predefinedNodes = new NodeStateCollection();
#if NET461 || NET462 || NET472 || NET48
            predefinedNodes.LoadFromBinaryResource(uaServerNodeManager_.SystemContext, resourceName, Assembly.GetEntryAssembly(), true);
#else
            predefinedNodes.LoadFromBinaryResource(uaServerNodeManager_.SystemContext, resourceName, this.GetType().GetTypeInfo().Assembly, true);
#endif

            // add the predefined nodes to the node manager.
            foreach (var t in predefinedNodes)
            {
                uaServerNodeManager_.AddPredefinedNode(uaServerNodeManager_.SystemContext, t);
            }

            // ensure the reverse references exist.
            uaServerNodeManager_.AddReverseReferences((IDictionary<NodeId, IList<IReference>>)externalReferences);
        }

        /// <summary>
        ///     Returns the state object for the specified node if it exists.
        /// </summary>
        public NodeState FindNode(NodeId nodeId)
        {
            return uaServerNodeManager_.Find(nodeId);
        }

        /// <summary>
        ///     Finds the specified node and checks if it is of the expected type.
        /// </summary>
        /// <returns>Returns null if not found or not of the correct type.</returns>
        public NodeState FindNode(NodeId nodeId, Type expectedType)
        {
            return uaServerNodeManager_.FindPredefinedNode(nodeId, expectedType);
        }

        /// <summary>
        ///     Finds a node in the dynamic cache.
        /// </summary>
        /// <param name="context">The UA server implementation of the ISystemContext interface.</param>
        /// <param name="handle">The handle of the node to validate.</param>
        /// <param name="cache">The cached nodes</param>
        /// <returns>Returns null if not found or the node object.</returns>
        public NodeState FindNode(UaServerContext context, UaNodeHandle handle, IDictionary<NodeId, NodeState> cache)
        {
            return uaServerNodeManager_.FindNodeInCache(context, handle, cache);
        }

        /// <summary>
        ///     Recursively indexes the node and its children.
        /// </summary>
        public void AddNode(NodeState node)
        {
            uaServerNodeManager_.AddPredefinedNode(uaServerNodeManager_.SystemContext, node);
        }

        /// <summary>
        ///     Recursively indexes the node and its children.
        /// </summary>
        public bool DeleteNode(NodeId nodeId)
        {
            return uaServerNodeManager_.DeleteNode(uaServerNodeManager_.SystemContext, nodeId);
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
        public BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName, LocalizedText displayName, LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None, AttributeWriteMask userWriteMask = AttributeWriteMask.None, RolePermissionTypeCollection rolePermissions = null, RolePermissionTypeCollection userRolePermissions = null)
        {
            var baseDataVariableState = uaServerNodeManager_.CreateBaseDataVariableState(parent, browseName, displayName, description, dataType, valueRank, accessLevel, initialValue, writeMask, userWriteMask, rolePermissions, rolePermissions);
            baseDataVariableState.OnWriteValue = OnWriteVariable;
            return baseDataVariableState;
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
        public BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName, LocalizedText displayName, LocalizedText description, NodeId dataType, int valueRank, byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None, AttributeWriteMask userWriteMask = AttributeWriteMask.None, RolePermissionTypeCollection rolePermissions = null, RolePermissionTypeCollection userRolePermissions = null)
        {
            var baseDataVariableState = uaServerNodeManager_.CreateBaseDataVariableState(parent, browseName, displayName, description, dataType, valueRank, accessLevel, initialValue, writeMask, userWriteMask, rolePermissions, rolePermissions);
            baseDataVariableState.OnWriteValue = OnWriteVariable;
            return baseDataVariableState;
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
        public BaseDataVariableState CreateBaseDataVariableState(NodeState parent, string browseName, LocalizedText displayName, LocalizedText description, ExpandedNodeId dataType, int valueRank, byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None, AttributeWriteMask userWriteMask = AttributeWriteMask.None, RolePermissionTypeCollection rolePermissions = null, RolePermissionTypeCollection userRolePermissions = null)
        {
            var nodeId = ExpandedNodeId.ToNodeId(dataType, NamespaceUris);
            var baseDataVariableState = uaServerNodeManager_.CreateBaseDataVariableState(parent, browseName, displayName, description, nodeId, valueRank, accessLevel, initialValue, writeMask, userWriteMask, rolePermissions, rolePermissions);
            baseDataVariableState.OnWriteValue = OnWriteVariable;
            return baseDataVariableState;
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
        public Argument CreateArgument(string name, string description, BuiltInType dataType, int valueRank)
        {
            return uaServerNodeManager_.CreateArgument(name, description, dataType, valueRank);
        }

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
        public MethodState CreateMethodState(BaseObjectState parent, string path, string name, GenericMethodCalledEventHandler2 callingMethod = null)
        {
            return uaServerNodeManager_.CreateMethodState(parent, path, name, callingMethod);
        }

        /// <summary>Adds the input arguments to a method.</summary>
        /// <param name="parent">The method object.</param>
        /// <param name="nodeId">
        ///     The unique identifier for the variable in the server's address space. The NodeId can be either:
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="uint" />
        ///         </item>
        ///         <item>
        ///             <see cref="Guid" />
        ///         </item>
        ///         <item>
        ///             <see cref="string" />
        ///         </item>
        ///         <item><see cref="byte" />[]</item>
        ///     </list>
        ///     <b>Important:</b> Keep in mind that the actual ID's of nodes should be unique such that no two nodes within an
        ///     address-space share the same ID's.
        /// </param>
        /// <param name="inputArguments">The input arguments.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        public StatusCode AddInputArguments(MethodState parent, object nodeId, params Argument[] inputArguments)
        {
            return uaServerNodeManager_.AddInputArguments(parent, nodeId, inputArguments);
        }

        /// <summary>Adds the output arguments to a method.</summary>
        /// <param name="parent">The method object.</param>
        /// <param name="nodeId">
        ///     The unique identifier for the variable in the server's address space. The NodeId can be either:
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="uint" />
        ///         </item>
        ///         <item>
        ///             <see cref="Guid" />
        ///         </item>
        ///         <item>
        ///             <see cref="string" />
        ///         </item>
        ///         <item><see cref="byte" />[]</item>
        ///     </list>
        ///     <b>Important:</b> Keep in mind that the actual ID's of nodes should be unique such that no two nodes within an
        ///     address-space share the same ID's.
        /// </param>
        /// <param name="outputArguments">The output arguments.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        public StatusCode AddOutputArguments(MethodState parent, object nodeId, params Argument[] outputArguments)
        {
            return uaServerNodeManager_.AddOutputArguments(parent, nodeId, outputArguments);
        }

        /// <summary>Writes to a variable.</summary>
        /// <param name="deviceItem">The <see cref="BaseVariableState" /> including the identifier.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        /// <param name="newValue">Object with new item value.</param>
        /// <param name="statusCode">New status code of the item value.</param>
        /// <param name="timestamp">New timestamp of the new item value.</param>
        public StatusCode WriteBaseVariable(BaseVariableState deviceItem, object newValue, StatusCode statusCode,
            DateTime timestamp)
        {
            try
            {
                Opc.Ua.LicenseHandler.ValidateFeatures();
                var variable = deviceItem;
                variable.Value = newValue;
                variable.StatusCode = (uint)statusCode;
                variable.Timestamp = timestamp;
                variable.ClearChangeMasks(uaServerNodeManager_.SystemContext, false);
            }
            catch
            {
                return StatusCodes.BadUnexpectedError;
            }
            return StatusCodes.Good;
        }

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
        public FolderState CreateFolderState(NodeState parent, string browseName, LocalizedText displayName, LocalizedText description = null, AttributeWriteMask writeMask = AttributeWriteMask.None, AttributeWriteMask userWriteMask = AttributeWriteMask.None, RolePermissionTypeCollection rolePermissions = null, RolePermissionTypeCollection userRolePermissions = null)
        {
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
            return uaServerNodeManager_.CreateFolderState(parent, browseName, displayName, description, writeMask, userWriteMask, rolePermissions, userRolePermissions);
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
        public BaseObjectState CreateBaseObjectState(NodeState parent, string browseName, LocalizedText displayName, LocalizedText description = null, AttributeWriteMask writeMask = AttributeWriteMask.None, AttributeWriteMask userWriteMask = AttributeWriteMask.None, RolePermissionTypeCollection rolePermissions = null, RolePermissionTypeCollection userRolePermissions = null)
        {
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
            return uaServerNodeManager_.CreateBaseObjectState(parent, browseName, displayName, description, writeMask, userWriteMask, rolePermissions, userRolePermissions);
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
        public PropertyState CreatePropertyState(NodeState parent, string browseName, LocalizedText displayName, LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None, AttributeWriteMask userWriteMask = AttributeWriteMask.None, RolePermissionTypeCollection rolePermissions = null, RolePermissionTypeCollection userRolePermissions = null)
        {
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
            return uaServerNodeManager_.CreatePropertyState(parent, browseName, displayName, description, dataType, valueRank, accessLevel, initialValue, writeMask, userWriteMask, rolePermissions, userRolePermissions);
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
        ///         Specifies the maximum precision that the server can maintain for the item based on restrictions in the target
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
        public DataItemState CreateDataItemState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
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
            var variable = uaServerNodeManager_.CreateDataItemState(parent, browseName, displayName,
            description, dataType, valueRank, accessLevel, initialValue, writeMask, userWriteMask, null, valuePrecision, rolePermissions, userRolePermissions);
            variable.OnWriteValue = OnWriteDataItem;
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
        public AnalogItemState CreateAnalogItemState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, BuiltInType dataType, int valueRank, byte accessLevel, object initialValue,
            Opc.Ua.Range euRange, EUInformation engineeringUnit = null, Opc.Ua.Range instrumentRange = null,
            AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            double? valuePrecision = null,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
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
            var variable = uaServerNodeManager_.CreateAnalogItemState(parent, browseName, displayName, description, dataType, valueRank, accessLevel, initialValue, euRange, engineeringUnit, instrumentRange, writeMask, userWriteMask, null, valuePrecision, rolePermissions, userRolePermissions);
            variable.OnWriteValue = OnWriteAnalog;

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
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <returns>The created <see cref="TwoStateDiscreteState" /></returns>
        public TwoStateDiscreteState CreateTwoStateDiscreteState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, byte accessLevel, bool initialValue, string trueState, string falseState, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null)
        {
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
            var variable = uaServerNodeManager_.CreateTwoStateDiscreteState(parent, browseName,
                displayName, description, accessLevel, initialValue, trueState, falseState);
            variable.OnWriteValue = OnWriteDataItem;

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
        /// <param name="rolePermissions">The optional RolePermissions Attribute specifies the Permissions that apply to a Node for all Roles which have access to the Node.</param>
        /// <param name="userRolePermissions">The optional UserRolePermissions Attribute specifies the Permissions that apply to a Node for all Roles granted to current Session.</param>
        /// <param name="values">The possible values the multi-state variable can have.</param>
        /// <returns>The created <see cref="MultiStateDiscreteState" /></returns>
        public MultiStateDiscreteState CreateMultiStateDiscreteState(NodeState parent, string browseName, LocalizedText displayName,
            LocalizedText description, byte accessLevel, object initialValue, AttributeWriteMask writeMask = AttributeWriteMask.None,
            AttributeWriteMask userWriteMask = AttributeWriteMask.None,
            RolePermissionTypeCollection rolePermissions = null,
            RolePermissionTypeCollection userRolePermissions = null, params LocalizedText[] values)
        {
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
            var variable = uaServerNodeManager_.CreateMultiStateDiscreteState(parent,
                browseName, displayName, description, accessLevel, initialValue, writeMask, userWriteMask, null, rolePermissions, userRolePermissions, values);
            variable.OnWriteValue = OnWriteDataItem;

            return variable;
        }

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
        public void AddRootNotifier(NodeState notifier)
        {
            uaServerNodeManager_.AddRootNotifier(notifier);
        }

        /// <summary>
        ///     Removes a root notifier previously added with AddRootNotifier.
        /// </summary>
        /// <param name="notifier">The notifier.</param>
        /// <remarks>
        ///     A root notifier is a notifier owned by the NodeManager that is not the target of a
        ///     HasNotifier reference. These nodes need to be linked directly to the Server object.
        /// </remarks>
        public void RemoveRootNotifier(NodeState notifier)
        {
            uaServerNodeManager_.RemoveRootNotifier(notifier);
        }

        /// <summary>
        ///     Can be called to report a global event.
        /// </summary>
        /// <param name="e">The event.</param>
        public void ReportEvent(IFilterTarget e)
        {
            (uaServerNodeManager_).ServerData.ReportEvent(e);
        }

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
        /// </example>
        public object GetNewSimulatedValue(BaseVariableState deviceItem)
        {
            if (dataGenerator_ == null)
            {
                dataGenerator_ = new DataGenerator(null) { BoundaryValueFrequency = 0 };
            }

            object value;

            try
            {
                var builtInType = DataTypes.GetBuiltInType(deviceItem.DataType);

                value = dataGenerator_.GetRandom(builtInType);
            }
            catch
            {
                value = null;
            }
            return value;
        }

        #endregion

        #region Helper Methods

        private ServiceResult OnWriteVariable(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as BaseDataVariableState;

            // verify data type.
            if (variable != null)
            {
                var typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

                if (typeInfo == null || Equals(typeInfo, Opc.Ua.TypeInfo.Unknown))
                {
                    return StatusCodes.BadTypeMismatch;
                }
            }

            StatusCode opcStatusCode = statusCode.Code;
            var writtenItem = variable;
            var tempValue = value;
            var tempTimestamp = timestamp;

            // apply the index range.
            if (indexRange != NumericRange.Empty)
            {
                if (writtenItem != null)
                {
                    var target = writtenItem.Value;
                    ServiceResult result = indexRange.UpdateRange(ref target, value);

                    if (ServiceResult.IsBad(result))
                    {
                        return result;
                    }

                    tempValue = target;
                }
            }

            var resultCode = UaServerPlugin.OnWriteBaseVariable(writtenItem, ref tempValue, ref opcStatusCode,
                ref tempTimestamp);
            statusCode.Code = (uint)opcStatusCode;

            if (StatusCode.IsBad((uint)resultCode))
            {
                return StatusCodes.Bad;
            }

            if (resultCode != StatusCodes.GoodCompletesAsynchronously)
            {
                timestamp = tempTimestamp;
                value = tempValue;
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnWriteDataItem(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as BaseDataVariableState;

            // verify data type.
            if (variable != null)
            {
                var typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

                if (typeInfo == null || Equals(typeInfo, Opc.Ua.TypeInfo.Unknown))
                {
                    return StatusCodes.BadTypeMismatch;
                }
            }

            StatusCode opcStatusCode = statusCode.Code;
            var writtenItem = variable;
            var tempValue = value;
            var tempTimestamp = timestamp;
            var resultCode = UaServerPlugin.OnWriteBaseVariable(writtenItem, ref tempValue, ref opcStatusCode,
                ref tempTimestamp);
            statusCode.Code = (uint)opcStatusCode;

            if (StatusCode.IsBad((uint)resultCode))
            {
                return StatusCodes.Bad;
            }

            if (resultCode != StatusCodes.GoodCompletesAsynchronously)
            {
                timestamp = tempTimestamp;
                value = tempValue;
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnWriteAnalog(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as AnalogItemState;

            // verify data type.
            if (variable != null)
            {
                var typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

                if (typeInfo == null || Equals(typeInfo, Opc.Ua.TypeInfo.Unknown))
                {
                    return StatusCodes.BadTypeMismatch;
                }
            }

            // check index range.
            if (variable != null && variable.ValueRank >= 0)
            {
                if (indexRange != NumericRange.Empty)
                {
                    var target = variable.Value;
                    var result = indexRange.UpdateRange(ref target, value);

                    if (ServiceResult.IsBad(result))
                    {
                        return result;
                    }

                    value = target;
                }
            }

            // check EU range.
            else
            {
                if (indexRange != NumericRange.Empty)
                {
                    return StatusCodes.BadIndexRangeInvalid;
                }

                var number = Convert.ToDouble(value);

                if (variable != null && (number < variable.EURange.Value.Low || number > variable.EURange.Value.High))
                {
                    return StatusCodes.BadOutOfRange;
                }
            }

            StatusCode opcStatusCode = statusCode.Code;
            BaseDataVariableState writtenItem = variable;
            var tempValue = value;
            var tempTimestamp = timestamp;
            var resultCode = UaServerPlugin.OnWriteBaseVariable(writtenItem, ref tempValue, ref opcStatusCode,
                ref tempTimestamp);
            statusCode.Code = (uint)opcStatusCode;

            if (StatusCode.IsBad((uint)resultCode))
            {
                return StatusCodes.Bad;
            }

            if (resultCode != StatusCodes.GoodCompletesAsynchronously)
            {
                timestamp = tempTimestamp;
                value = tempValue;
            }

            return ServiceResult.Good;
        }

        #endregion

        #region Private Fields
        private DataGenerator dataGenerator_;
        private UaBaseNodeManager uaServerNodeManager_;

        private Timer reverseConnectTimer_;
        private int connectInterval_;
        private int connectTimeout_;
        private int rejectTimeout_;
        private Dictionary<Uri, UaReverseConnectProperty> connections_;
        private object connectionsLock_ = new object();
        #endregion
    }
}