#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
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

using Technosoftware.UaConfiguration;
using Technosoftware.UaServer;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Server;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Subscriptions;
#endregion

namespace Technosoftware.UaStandardServer
{
    /// <summary>
    /// The standard implementation of an OPC UA server with reverse connect.
    /// </summary>
    /// <remarks>
    ///     Each server instance must have one instance of a UaStandardServer object which is
    ///     responsible for reading the configuration file, creating the endpoints and dispatching
    ///     incoming requests to the appropriate handler.
    /// 
    ///     This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the NodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class UaStandardServer : UaGenericServer
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
        public UaStandardServer()
        {
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
        /// <summary>The product the license was issued for.</summary>
        public string Product { get; set; }

        /// <summary>
        ///     Indicates whether the server must be restarted. This is mainly the case if the server is used in evaluation mode
        ///     and the 90 minutes evaluation time expired.
        /// </summary>
        public bool RestartRequired => Opc.Ua.LicenseHandler.IsExpired;

        /// <summary>
        ///     Indicates whether the server should use reverse connect or not.
        /// </summary>
        public bool UseReverseConnect { get; set; }
        #endregion

        #region Private Fields
        private Timer reverseConnectTimer_;
        private int connectInterval_;
        private int connectTimeout_;
        private int rejectTimeout_;
        private Dictionary<Uri, UaReverseConnectProperty> connections_;
        private object connectionsLock_ = new object();
        #endregion
    }
}