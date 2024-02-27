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
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;

using Technosoftware.UaServer.Aggregates;
using Technosoftware.UaServer.Configuration;
using Technosoftware.UaServer.Diagnostics;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Subscriptions;
#endregion

#pragma warning disable 0618

namespace Technosoftware.UaServer.Server
{
    /// <summary>
    /// A class that stores the globally accessible state of a server instance.
    /// </summary>
    /// <remarks>
    /// This is a readonly class that is initialized when the server starts up. It provides
    /// access to global objects and data that different parts of the server may require.
    /// It also defines some global methods.
    /// 
    /// This object is constructed is three steps:
    /// - the configuration is provided.
    /// - the node managers et. al. are provided.
    /// - the session/subscription managers are provided.
    /// 
    /// The server is not running until all three steps are complete.
    /// 
    /// The references returned from this object do not change after all three states are complete. 
    /// This ensures the object is thread safe even though it does not use a lock.
    /// Objects returned from this object can be assumed to be threadsafe unless otherwise stated.
    /// </remarks>
    public class GenericServerData : IUaServerData, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the datastore with the server configuration.
        /// </summary>
        /// <param name="serverDescription">The server description.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="messageContext">The message context.</param>
        /// <param name="certificateValidator">The certificate validator.</param>
        /// <param name="instanceCertificate">The instance certificate.</param>
        public GenericServerData(
            ServerProperties                     serverDescription, 
            ApplicationConfiguration             configuration,
            IServiceMessageContext               messageContext,
            CertificateValidator                 certificateValidator,
            X509Certificate2                     instanceCertificate)
        {
            serverDescription_ = serverDescription;
            configuration_ = configuration;
            MessageContext = messageContext;

            endpointAddresses_ = new List<Uri>();

            foreach (var baseAddresses in configuration_.ServerConfiguration.BaseAddresses)
            {
                var url = Utils.ParseUri(baseAddresses);

                if (url != null)
                {
                    endpointAddresses_.Add(url);
                }
            }

            NamespaceUris = MessageContext.NamespaceUris;
            Factory = MessageContext.Factory;

            ServerUris = new StringTable();
            TypeTree = new TypeTable(NamespaceUris);

            // add the server uri to the server table.
            ServerUris.Append(configuration_.ApplicationUri);

            // create the default system context.
            DefaultSystemContext = new UaServerContext(this);
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
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Utils.SilentDispose(ResourceManager);
                Utils.SilentDispose(RequestManager);
                Utils.SilentDispose(AggregateManager);
                Utils.SilentDispose(NodeManager);
                Utils.SilentDispose(SessionManager);
                Utils.SilentDispose(SubscriptionManager);
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// The session manager to use with the server.
        /// </summary>
        /// <value>The session manager.</value>
        public Sessions.SessionManager SessionManager { get; private set; }

        /// <summary>
        /// The subscription manager to use with the server.
        /// </summary>
        /// <value>The subscription manager.</value>
        public SubscriptionManager SubscriptionManager { get; private set; }

        /// <summary>
        /// Stores the MasterNodeManager and the CoreNodeManager
        /// </summary>
        /// <param name="nodeManager">The node manager.</param>
        public void SetNodeManager(MasterNodeManager nodeManager)
        {
            NodeManager = nodeManager;
            DiagnosticsNodeManager = nodeManager.DiagnosticsNodeManager;
            CoreNodeManager = nodeManager.CoreNodeManager;
        }

        /// <summary>
        /// Sets the EventManager, the ResourceManager, the RequestManager and the AggregateManager.
        /// </summary>
        /// <param name="eventManager">The event manager.</param>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="requestManager">The request manager.</param>
        public void CreateServerObject(
            EventManager      eventManager,
            ResourceManager   resourceManager, 
            RequestManager    requestManager)
        {
            EventManager = eventManager;
            ResourceManager = resourceManager;
            RequestManager = requestManager;

            // create the server object.
            CreateServerObject();
        }

        /// <summary>
        /// Stores the SessionManager, the SubscriptionManager in the datastore.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="subscriptionManager">The subscription manager.</param>
        public void SetSessionManager(
            Sessions.SessionManager      sessionManager,
            SubscriptionManager subscriptionManager)
        {
            SessionManager = sessionManager;
            SubscriptionManager = subscriptionManager;
        }
        #endregion

        #region IUaServerData Members

        /// <summary>
        /// The endpoint addresses used by the server.
        /// </summary>
        /// <value>The endpoint addresses.</value>
        public IEnumerable<Uri> EndpointAddresses
        {
            get { return endpointAddresses_; }
        }


        /// <summary>
        /// The context to use when serializing/deserializing extension objects.
        /// </summary>
        /// <value>The message context.</value>
        public IServiceMessageContext MessageContext { get; }

        /// <summary>
        /// The default system context for the server.
        /// </summary>
        /// <value>The default system context.</value>
        public UaServerContext DefaultSystemContext { get; }

        /// <summary>
        /// The table of namespace uris known to the server.
        /// </summary>
        /// <value>The namespace URIs.</value>
        public NamespaceTable NamespaceUris { get; }

        /// <summary>
        /// The table of remote server uris known to the server.
        /// </summary>
        /// <value>The server URIs.</value>
        public StringTable ServerUris { get; }

        /// <summary>
        /// The factory used to create encodeable objects that the server understands.
        /// </summary>
        /// <value>The factory.</value>
        public IEncodeableFactory Factory { get; }

        /// <summary>
        /// The datatypes, object types and variable types known to the server.
        /// </summary>
        /// <value>The type tree.</value>
        /// <remarks>
        /// The type tree table is a global object that all components of a server have access to.
        /// Node managers must populate this table with all types that they define.
        /// This object is thread safe.
        /// </remarks>
        public TypeTable TypeTree { get; }

        /// <summary>
        /// The master node manager for the server.
        /// </summary>
        /// <value>The node manager.</value>
        public MasterNodeManager NodeManager { get; private set; }

        /// <summary>
        /// The internal node manager for the servers.
        /// </summary>
        /// <value>The core node manager.</value>
        public CoreNodeManager CoreNodeManager { get; private set; }

        /// <summary>
        /// Returns the node manager that managers the server diagnostics.
        /// </summary>
        /// <value>The diagnostics node manager.</value>
        public DiagnosticsNodeManager DiagnosticsNodeManager { get; private set; }

        /// <summary>
        /// The manager for events that all components use to queue events that occur.
        /// </summary>
        /// <value>The event manager.</value>
        public EventManager EventManager { get; private set; }

        /// <summary>
        /// A manager for localized resources that components can use to localize text.
        /// </summary>
        /// <value>The resource manager.</value>
        public ResourceManager ResourceManager { get; private set; }

        /// <summary>
        /// A manager for outstanding requests that allows components to receive notifications if the timeout or are cancelled.
        /// </summary>
        /// <value>The request manager.</value>
        public RequestManager RequestManager { get; private set; }

        /// <summary>
        /// A manager for aggregate calculators supported by the server.
        /// </summary>
        /// <value>The aggregate manager.</value>
        public AggregateManager AggregateManager { get; set; }

        /// <summary>
        /// The manager for active sessions.
        /// </summary>
        /// <value>The session manager.</value>
        IUaSessionManager IUaServerData.SessionManager
        {
            get { return SessionManager; }
        }

        /// <summary>
        /// The manager for active subscriptions.
        /// </summary>
        IUaSubscriptionManager IUaServerData.SubscriptionManager
        {
            get { return SubscriptionManager; }
        }


        /// <summary>
        /// Returns the status object for the server.
        /// </summary>
        /// <value>The status.</value>
        public ServerStatusValue Status { get; private set; }

        /// <summary>
        /// Gets or sets the current state of the server.
        /// </summary>
        /// <value>The state of the current.</value>
        public ServerState CurrentState 
        {
            get
            {
                lock (Status.Lock)
                {
                    return Status.Value.State;
                }
            }

            set
            {
                lock (Status.Lock)
                {
                    Status.Value.State = value;
                }
            }
        }

        /// <summary>
        /// Returns the Server object node
        /// </summary>
        /// <value>The Server object node.</value>
        public ServerObjectState ServerObject
        {
            get { return serverObject_; }
        }

        /// <summary>
        /// Used to synchronize access to the server diagnostics.
        /// </summary>
        /// <value>The diagnostics lock.</value>
        public object DiagnosticsLock { get; } = new object();

        /// <summary>
        /// Used to synchronize write access to
        /// the server diagnostics.
        /// </summary>
        /// <value>The diagnostics lock.</value>
        public object DiagnosticsWriteLock
        {
            get
            {
                // implicitly force diagnostics update
                if (DiagnosticsNodeManager != null)
                {
                    DiagnosticsNodeManager.ForceDiagnosticsScan();
                }
                return DiagnosticsLock;
            }
        }

        /// <summary>
        /// Returns the diagnostics structure for the server.
        /// </summary>
        /// <value>The server diagnostics.</value>
        public ServerDiagnosticsSummaryDataType ServerDiagnostics { get; private set; }

        /// <summary>
        /// Whether the server is currently running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This flag is set to false when the server shuts down. Threads running should check this flag whenever
        /// they return from a blocking operation. If it is false the thread should clean up and terminate.
        /// </remarks>
        public bool IsRunning
        {
            get
            {
                if (Status == null)
                {
                    return false;
                }

                lock (Status.Lock)
                {
                    if (Status.Value.State == ServerState.Running)
                        return true;

                    if (Status.Value.State == ServerState.Shutdown && Status.Value.SecondsTillShutdown > 0)
                        return true;

                    return false;
                }
            }
        }

        /// <summary>
        /// Whether the server is collecting diagnostics.
        /// </summary>
        /// <value><c>true</c> if diagnostics are enabled; otherwise, <c>false</c>.</value>
        public bool DiagnosticsEnabled
        {
            get
            {
                if (DiagnosticsNodeManager == null)
                {
                    return false;
                }

                return DiagnosticsNodeManager.DiagnosticsEnabled;
            }
        }

        /// <summary>
        /// Closes the specified session.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="deleteSubscriptions">if set to <c>true</c> subscriptions are to be deleted.</param>
        public void CloseSession(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions)
        {
            NodeManager.SessionClosing(context, sessionId, deleteSubscriptions);
            SubscriptionManager.SessionClosing(context, sessionId, deleteSubscriptions);
            SessionManager.CloseSession(sessionId);
        }

        /// <summary>
        /// Deletes the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        public void DeleteSubscription(uint subscriptionId)
        {
            SubscriptionManager.DeleteSubscription(null, subscriptionId);
        }

        /// <summary>
        /// Called by any component to report a global event.
        /// </summary>
        /// <param name="e">The event.</param>
        public void ReportEvent(IFilterTarget e)
        {
            ReportEvent(DefaultSystemContext, e);
        }

        /// <summary>
        /// Called by any component to report a global event.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="e">The event.</param>
        public void ReportEvent(ISystemContext context, IFilterTarget e)
        {
            if ((Auditing == false) && (e is AuditEventState))
            {
                // do not report auditing events if server Auditing flag is false
                return;
            }

            serverObject_.ReportEvent(context, e);
        }

        /// <summary>
        /// Refreshes the conditions for the specified subscription.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        public void ConditionRefresh(UaServerOperationContext context, uint subscriptionId)
        {
            SubscriptionManager.ConditionRefresh(context, subscriptionId);
        }

        /// <summary>
        /// Refreshes the conditions for the specified subscription.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="monitoredItemId">The monitored item identifier.</param>
        public void ConditionRefresh2(UaServerOperationContext context, uint subscriptionId, uint monitoredItemId)
        {
            SubscriptionManager.ConditionRefresh2(context, subscriptionId, monitoredItemId);
        }
        #endregion

        #region IUaAuditReportEvents Members
        /// <inheritdoc/>
        public bool Auditing => auditing_;

        /// <inheritdoc/>
        public ISystemContext DefaultAuditContext => DefaultSystemContext.Copy();

        /// <inheritdoc/>
        public void ReportAuditEvent(ISystemContext context, AuditEventState e)
        {
            if (Auditing == false)
            {
                // do not report auditing events if server Auditing flag is false
                return;
            }

            ReportEvent(context, e);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the ServerObject and attaches it to the NodeManager.
        /// </summary>
        private void CreateServerObject()
        {
            lock (DiagnosticsNodeManager.Lock)
            {
                // get the server object.
                var serverObject = serverObject_ = (ServerObjectState)DiagnosticsNodeManager.FindPredefinedNode(
                    ObjectIds.Server,
                    typeof(ServerObjectState));

                // update server capabilities.
                serverObject.ServiceLevel.Value = 255;
                serverObject.ServerCapabilities.LocaleIdArray.Value = ResourceManager.GetAvailableLocales();
                serverObject.ServerCapabilities.ServerProfileArray.Value = configuration_.ServerConfiguration.ServerProfileArray.ToArray();
                serverObject.ServerCapabilities.MinSupportedSampleRate.Value = 0;
                serverObject.ServerCapabilities.MaxBrowseContinuationPoints.Value = (ushort)configuration_.ServerConfiguration.MaxBrowseContinuationPoints;
                serverObject.ServerCapabilities.MaxQueryContinuationPoints.Value = (ushort)configuration_.ServerConfiguration.MaxQueryContinuationPoints;
                serverObject.ServerCapabilities.MaxHistoryContinuationPoints.Value = (ushort)configuration_.ServerConfiguration.MaxHistoryContinuationPoints;
                serverObject.ServerCapabilities.MaxArrayLength.Value = (uint)configuration_.TransportQuotas.MaxArrayLength;
                serverObject.ServerCapabilities.MaxStringLength.Value = (uint)configuration_.TransportQuotas.MaxStringLength;
                serverObject.ServerCapabilities.MaxByteStringLength.Value = (uint)configuration_.TransportQuotas.MaxByteStringLength;

                // Any operational limits Property that is provided shall have a non zero value.
                var operationLimits = serverObject.ServerCapabilities.OperationLimits;
                var configOperationLimits = configuration_.ServerConfiguration.OperationLimits;
                if (configOperationLimits != null)
                {
                    operationLimits.MaxNodesPerRead = SetPropertyValue(operationLimits.MaxNodesPerRead, configOperationLimits.MaxNodesPerRead);
                    operationLimits.MaxNodesPerHistoryReadData = SetPropertyValue(operationLimits.MaxNodesPerHistoryReadData, configOperationLimits.MaxNodesPerHistoryReadData);
                    operationLimits.MaxNodesPerHistoryReadEvents = SetPropertyValue(operationLimits.MaxNodesPerHistoryReadEvents, configOperationLimits.MaxNodesPerHistoryReadEvents);
                    operationLimits.MaxNodesPerWrite = SetPropertyValue(operationLimits.MaxNodesPerWrite, configOperationLimits.MaxNodesPerWrite);
                    operationLimits.MaxNodesPerHistoryUpdateData = SetPropertyValue(operationLimits.MaxNodesPerHistoryUpdateData, configOperationLimits.MaxNodesPerHistoryUpdateData);
                    operationLimits.MaxNodesPerHistoryUpdateEvents = SetPropertyValue(operationLimits.MaxNodesPerHistoryUpdateEvents, configOperationLimits.MaxNodesPerHistoryUpdateEvents);
                    operationLimits.MaxNodesPerMethodCall = SetPropertyValue(operationLimits.MaxNodesPerMethodCall, configOperationLimits.MaxNodesPerMethodCall);
                    operationLimits.MaxNodesPerBrowse = SetPropertyValue(operationLimits.MaxNodesPerBrowse, configOperationLimits.MaxNodesPerBrowse);
                    operationLimits.MaxNodesPerRegisterNodes = SetPropertyValue(operationLimits.MaxNodesPerRegisterNodes, configOperationLimits.MaxNodesPerRegisterNodes);
                    operationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds = SetPropertyValue(operationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds, configOperationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds);
                    operationLimits.MaxNodesPerNodeManagement = SetPropertyValue(operationLimits.MaxNodesPerNodeManagement, configOperationLimits.MaxNodesPerNodeManagement);
                    operationLimits.MaxMonitoredItemsPerCall = SetPropertyValue(operationLimits.MaxMonitoredItemsPerCall, configOperationLimits.MaxMonitoredItemsPerCall);
                }
                else
                {
                    operationLimits.MaxNodesPerRead =
                    operationLimits.MaxNodesPerHistoryReadData =
                    operationLimits.MaxNodesPerHistoryReadEvents =
                    operationLimits.MaxNodesPerWrite =
                    operationLimits.MaxNodesPerHistoryUpdateData =
                    operationLimits.MaxNodesPerHistoryUpdateEvents =
                    operationLimits.MaxNodesPerMethodCall =
                    operationLimits.MaxNodesPerBrowse =
                    operationLimits.MaxNodesPerRegisterNodes =
                    operationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds =
                    operationLimits.MaxNodesPerNodeManagement =
                    operationLimits.MaxMonitoredItemsPerCall = null;
                }

                // setup PublishSubscribe Status State value
                PubSubState pubSubState = PubSubState.Disabled;

                var default_PubSubState = (BaseVariableState)DiagnosticsNodeManager.FindPredefinedNode(
                    VariableIds.PublishSubscribe_Status_State,
                    typeof(BaseVariableState));
                default_PubSubState.Value = pubSubState;

                // setup value for SupportedTransportProfiles
                var default_SupportedTransportProfiles = (BaseVariableState)DiagnosticsNodeManager.FindPredefinedNode(
                   VariableIds.PublishSubscribe_SupportedTransportProfiles,
                   typeof(BaseVariableState));
                default_SupportedTransportProfiles.Value = "uadp";

                // setup callbacks for dynamic values.
                serverObject.NamespaceArray.OnSimpleReadValue = OnReadNamespaceArray;
                serverObject.NamespaceArray.MinimumSamplingInterval = 1000;

                serverObject.ServerArray.OnSimpleReadValue = OnReadServerArray;
                serverObject.ServerArray.MinimumSamplingInterval = 1000;

                // dynamic change of enabledFlag is disabled to pass CTT
                serverObject.ServerDiagnostics.EnabledFlag.AccessLevel = AccessLevels.CurrentRead;
                serverObject.ServerDiagnostics.EnabledFlag.UserAccessLevel = AccessLevels.CurrentRead;
                serverObject.ServerDiagnostics.EnabledFlag.OnSimpleReadValue = OnReadDiagnosticsEnabledFlag;
                serverObject.ServerDiagnostics.EnabledFlag.OnSimpleWriteValue = OnWriteDiagnosticsEnabledFlag;
                serverObject.ServerDiagnostics.EnabledFlag.MinimumSamplingInterval = 1000;

                // initialize status.
                ServerStatusDataType serverStatus = new ServerStatusDataType {
                    StartTime = DateTime.UtcNow,
                    CurrentTime = DateTime.UtcNow,
                    State = ServerState.Shutdown
                };

                var buildInfo = new BuildInfo() {
                    ProductName = serverDescription_.ProductName,
                    ProductUri = serverDescription_.ProductUri,
                    ManufacturerName = serverDescription_.ManufacturerName,
                    SoftwareVersion = serverDescription_.SoftwareVersion,
                    BuildNumber = serverDescription_.BuildNumber,
                    BuildDate = serverDescription_.BuildDate,
                };
                var buildInfoVariableState = (BuildInfoVariableState)DiagnosticsNodeManager.FindPredefinedNode(VariableIds.Server_ServerStatus_BuildInfo, typeof(BuildInfoVariableState));
                var buildInfoVariable = new BuildInfoVariableValue(buildInfoVariableState, buildInfo, null);
                serverStatus.BuildInfo = buildInfoVariable.Value;

                serverObject.ServerStatus.MinimumSamplingInterval = 1000;
                serverObject.ServerStatus.CurrentTime.MinimumSamplingInterval = 1000;

                Status = new ServerStatusValue(
                    serverObject.ServerStatus,
                    serverStatus,
                    DiagnosticsLock);

                Status.Timestamp = DateTime.UtcNow;
                Status.OnBeforeRead = OnReadServerStatus;

                // initialize diagnostics.
                ServerDiagnostics = new ServerDiagnosticsSummaryDataType {
                    ServerViewCount = 0,
                    CurrentSessionCount = 0,
                    CumulatedSessionCount = 0,
                    SecurityRejectedSessionCount = 0,
                    RejectedSessionCount = 0,
                    SessionTimeoutCount = 0,
                    SessionAbortCount = 0,
                    PublishingIntervalCount = 0,
                    CurrentSubscriptionCount = 0,
                    CumulatedSubscriptionCount = 0,
                    SecurityRejectedRequestsCount = 0,
                    RejectedRequestsCount = 0
                };

                DiagnosticsNodeManager.CreateServerDiagnostics(
                    DefaultSystemContext,
                    ServerDiagnostics,
                    OnUpdateDiagnostics);

                // set the diagnostics enabled state.
                DiagnosticsNodeManager.SetDiagnosticsEnabled(
                    DefaultSystemContext,
                    configuration_.ServerConfiguration.DiagnosticsEnabled);

                var configurationNodeManager = DiagnosticsNodeManager as ConfigurationNodeManager;
                configurationNodeManager?.CreateServerConfiguration(
                    DefaultSystemContext,
                    configuration_);

                auditing_ = configuration_.ServerConfiguration.AuditingEnabled;
                PropertyState<bool> auditing = serverObject.Auditing;
                auditing.OnSimpleWriteValue += OnWriteAuditing;
                auditing.OnSimpleReadValue += OnReadAuditing;
                auditing.Value = auditing_;
                auditing.RolePermissions = new RolePermissionTypeCollection {
                        new RolePermissionType {
                            RoleId = ObjectIds.WellKnownRole_AuthenticatedUser,
                            Permissions = (uint)(PermissionType.Browse|PermissionType.Read)
                            },
                        new RolePermissionType {
                            RoleId = ObjectIds.WellKnownRole_SecurityAdmin,
                            Permissions = (uint)(PermissionType.Browse|PermissionType.Write|PermissionType.ReadRolePermissions|PermissionType.Read)
                            }};
                auditing.AccessLevel = AccessLevels.CurrentRead;
                auditing.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                auditing.MinimumSamplingInterval = 1000;
            }
        }

        /// <summary>
        /// Updates the server status before a read.
        /// </summary>
        private void OnReadServerStatus(
            ISystemContext context,
            BaseVariableValue variable,
            NodeState component)
        {
            lock (DiagnosticsLock)
            {
                var now = DateTime.UtcNow;
                Status.Timestamp = now;
                Status.Value.CurrentTime = now;
            }
        }

        /// <summary>
        /// Returns a copy of the namespace array.
        /// </summary>
        private ServiceResult OnReadNamespaceArray(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            value = NamespaceUris.ToArray();
            return ServiceResult.Good;
        }

        /// <summary>
        /// Returns a copy of the server array.
        /// </summary>
        private ServiceResult OnReadServerArray(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            value = ServerUris.ToArray();
            return ServiceResult.Good;
        }

        /// <summary>
        /// Returns Diagnostics.EnabledFlag
        /// </summary>
        private ServiceResult OnReadDiagnosticsEnabledFlag(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            value = DiagnosticsNodeManager.DiagnosticsEnabled;
            return ServiceResult.Good;
        }

        /// <summary>
        /// Sets the Diagnostics.EnabledFlag
        /// </summary>
        private ServiceResult OnWriteDiagnosticsEnabledFlag(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            var enabled = (bool)value;
            DiagnosticsNodeManager.SetDiagnosticsEnabled(
                DefaultSystemContext,
                enabled);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Updates the Server.Auditing flag.
        /// </summary>
        private ServiceResult OnWriteAuditing(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            auditing_ = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            return ServiceResult.Good;
        }

        /// <summary>
        /// Updates the Server.Auditing flag.
        /// </summary>
        private ServiceResult OnReadAuditing(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            value = auditing_;
            return ServiceResult.Good;
        }

        /// <summary>
        /// Returns a copy of the current diagnostics.
        /// </summary>
        private ServiceResult OnUpdateDiagnostics(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            lock (ServerDiagnostics)
            {
                value = Utils.Clone(ServerDiagnostics);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Set the property to null if the value is zero,
        /// to the value otherwise.
        /// </summary>
        private PropertyState<uint> SetPropertyValue(PropertyState<uint> property, uint value)
        {
            if (value != 0)
            {
                property.Value = value;
            }
            else
            {
                property = null;
            }
            return property;
        }
        #endregion

        #region Private Fields
        private ServerProperties serverDescription_;
        private ApplicationConfiguration configuration_;
        private List<Uri> endpointAddresses_;

        private ServerObjectState serverObject_;
        private bool auditing_;
        #endregion
    }
}
