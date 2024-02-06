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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Decorator class for traceable session with Activity Source.
    /// </summary>
    public class TraceableSession : IUaSession
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TraceableSession(IUaSession session)
        {
            session_ = session;
        }
        #endregion

        /// <summary>
        /// Activity Source Name.
        /// </summary>
        public static readonly string ActivitySourceName = "Technosoftware.UaClient-TraceableSession-ActivitySource";

        /// <summary>
        /// Activity Source static instance.
        /// </summary>
        public static ActivitySource ActivitySource => activitySource_.Value;
        private static readonly Lazy<ActivitySource> activitySource_ = new Lazy<ActivitySource>(() => new ActivitySource(ActivitySourceName, "1.0.0"));

        /// <summary>
        /// The IUaSession which is being traced.
        /// </summary>
        private readonly IUaSession session_;

        /// <inheritdoc/>
        public IUaSession Session => session_;

        #region IUaSession interface
        /// <inheritdoc/>
        public event EventHandler<SessionKeepAliveEventArgs> SessionKeepAliveEvent
        {
            add => session_.SessionKeepAliveEvent += value;
            remove => session_.SessionKeepAliveEvent -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<SessionNotificationEventArgs> SessionNotificationEvent
        {
            add => session_.SessionNotificationEvent += value;
            remove => session_.SessionNotificationEvent -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<SessionPublishErrorEventArgs> SessionPublishErrorEvent
        {
            add => session_.SessionPublishErrorEvent += value;
            remove => session_.SessionPublishErrorEvent -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<PublishSequenceNumbersToAcknowledgeEventArgs> PublishSequenceNumbersToAcknowledgeEvent
        {
            add => session_.PublishSequenceNumbersToAcknowledgeEvent += value;
            remove => session_.PublishSequenceNumbersToAcknowledgeEvent -= value;
        }

        /// <inheritdoc/>
        public event EventHandler SubscriptionsChangedEvent
        {
            add => session_.SubscriptionsChangedEvent += value;
            remove => session_.SubscriptionsChangedEvent -= value;
        }

        /// <inheritdoc/>
        public event EventHandler SessionClosingEvent
        {
            add => session_.SessionClosingEvent += value;
            remove => session_.SessionClosingEvent -= value;
        }

        /// <inheritdoc/>
        public event EventHandler SessionConfigurationChangedEvent
        {
            add => session_.SessionConfigurationChangedEvent += value;
            remove => session_.SessionConfigurationChangedEvent -= value;
        }

        /// <inheritdoc/>
        public event RenewUserIdentity RenewUserIdentityEvent
        {
            add => session_.RenewUserIdentityEvent += value;
            remove => session_.RenewUserIdentityEvent -= value;
        }

        /// <inheritdoc/>
        public IUaSessionFactory SessionFactory => TraceableSessionFactory.Instance;

        /// <inheritdoc/>
        public ConfiguredEndpoint ConfiguredEndpoint => session_.ConfiguredEndpoint;

        /// <inheritdoc/>
        public string SessionName => session_.SessionName;

        /// <inheritdoc/>
        public double SessionTimeout => session_.SessionTimeout;

        /// <inheritdoc/>
        public object Handle => session_.Handle;

        /// <inheritdoc/>
        public IUserIdentity Identity => session_.Identity;

        /// <inheritdoc/>
        public IEnumerable<IUserIdentity> IdentityHistory => session_.IdentityHistory;

        /// <inheritdoc/>
        public NamespaceTable NamespaceUris => session_.NamespaceUris;

        /// <inheritdoc/>
        public StringTable ServerUris => session_.ServerUris;

        /// <inheritdoc/>
        public ISystemContext SystemContext => session_.SystemContext;

        /// <inheritdoc/>
        public IEncodeableFactory Factory => session_.Factory;

        /// <inheritdoc/>
        public ITypeTable TypeTree => session_.TypeTree;

        /// <inheritdoc/>
        public IUaNodeCache NodeCache => session_.NodeCache;

        /// <inheritdoc/>
        public FilterContext FilterContext => session_.FilterContext;

        /// <inheritdoc/>
        public StringCollection PreferredLocales => session_.PreferredLocales;

        /// <inheritdoc/>
        public IReadOnlyDictionary<NodeId, DataDictionary> DataTypeSystem => session_.DataTypeSystem;

        /// <inheritdoc/>
        public IEnumerable<Subscription> Subscriptions => session_.Subscriptions;

        /// <inheritdoc/>
        public int SubscriptionCount => session_.SubscriptionCount;

        /// <inheritdoc/>
        public bool DeleteSubscriptionsOnClose
        {
            get => session_.DeleteSubscriptionsOnClose;
            set => session_.DeleteSubscriptionsOnClose = value;
        }

        /// <inheritdoc/>
        public Subscription DefaultSubscription
        {
            get => session_.DefaultSubscription;
            set => session_.DefaultSubscription = value;
        }

        /// <inheritdoc/>
        public int KeepAliveInterval
        {
            get => session_.KeepAliveInterval;
            set => session_.KeepAliveInterval = value;
        }

        /// <inheritdoc/>
        public bool KeepAliveStopped => session_.KeepAliveStopped;

        /// <inheritdoc/>
        public DateTime LastKeepAliveTime => session_.LastKeepAliveTime;

        /// <inheritdoc/>
        public int OutstandingRequestCount => session_.OutstandingRequestCount;

        /// <inheritdoc/>
        public int DefunctRequestCount => session_.DefunctRequestCount;

        /// <inheritdoc/>
        public int GoodPublishRequestCount => session_.GoodPublishRequestCount;

        /// <inheritdoc/>
        public int MinPublishRequestCount
        {
            get => session_.MinPublishRequestCount;
            set => session_.MinPublishRequestCount = value;
        }

        /// <inheritdoc/>
        public OperationLimits OperationLimits => session_.OperationLimits;

        /// <inheritdoc/>
        public bool TransferSubscriptionsOnReconnect
        {
            get => session_.TransferSubscriptionsOnReconnect;
            set => session_.TransferSubscriptionsOnReconnect = value;
        }

        /// <inheritdoc/>
        public NodeId SessionId => session_.SessionId;

        /// <inheritdoc/>
        public bool Connected => session_.Connected;

        /// <inheritdoc/>
        public EndpointDescription Endpoint => session_.Endpoint;

        /// <inheritdoc/>
        public EndpointConfiguration EndpointConfiguration => session_.EndpointConfiguration;

        /// <inheritdoc/>
        public IServiceMessageContext MessageContext => session_.MessageContext;

        /// <inheritdoc/>
        public ITransportChannel NullableTransportChannel => session_.NullableTransportChannel;

        /// <inheritdoc/>
        public ITransportChannel TransportChannel => session_.TransportChannel;

        /// <inheritdoc/>
        public DiagnosticsMasks ReturnDiagnostics
        {
            get => session_.ReturnDiagnostics;
            set => session_.ReturnDiagnostics = value;
        }

        /// <inheritdoc/>
        public int OperationTimeout
        {
            get => session_.OperationTimeout;
            set => session_.OperationTimeout = value;
        }

        /// <inheritdoc/>
        public bool Disposed => session_.Disposed;

        /// <inheritdoc/>
        public bool CheckDomain => session_.CheckDomain;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            // Presume that the wrapper is being compared to the
            // wrapped object, e.g. in a keep alive callback.
            if (ReferenceEquals(session_, obj)) return true;
            return session_?.Equals(obj) ?? false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return session_?.GetHashCode() ?? base.GetHashCode();
        }

        /// <inheritdoc/>
        public void Reconnect()
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Reconnect();
            }
        }

        /// <inheritdoc/>
        public void Reconnect(ITransportWaitingConnection connection)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Reconnect(connection);
            }
        }

        /// <inheritdoc/>
        public void Reconnect(ITransportChannel channel)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Reconnect(channel);
            }
        }

        /// <inheritdoc/>
        public async Task ReconnectAsync(CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.ReconnectAsync(ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task ReconnectAsync(ITransportWaitingConnection connection, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.ReconnectAsync(connection, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task ReconnectAsync(ITransportChannel channel, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.ReconnectAsync(channel, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public void Save(string filePath, IEnumerable<Type> knownTypes = null)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Save(filePath, knownTypes);
            }
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IEnumerable<Subscription> subscriptions, IEnumerable<Type> knownTypes = null)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Save(stream, subscriptions, knownTypes);
            }
        }

        /// <inheritdoc/>
        public void Save(string filePath, IEnumerable<Subscription> subscriptions, IEnumerable<Type> knownTypes = null)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Save(filePath, subscriptions, knownTypes);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Subscription> Load(Stream stream, bool transferSubscriptions = false, IEnumerable<Type> knownTypes = null)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Load(stream, transferSubscriptions, knownTypes);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Subscription> Load(string filePath, bool transferSubscriptions = false, IEnumerable<Type> knownTypes = null)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Load(filePath, transferSubscriptions, knownTypes);
            }
        }

        /// <inheritdoc/>
        public void FetchNamespaceTables()
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.FetchNamespaceTables();
            }
        }

        /// <inheritdoc/>
        public void FetchTypeTree(ExpandedNodeId typeId)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.FetchTypeTree(typeId);
            }
        }

        /// <inheritdoc/>
        public void FetchTypeTree(ExpandedNodeIdCollection typeIds)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.FetchTypeTree(typeIds);
            }
        }

        /// <inheritdoc/>
        public async Task FetchTypeTreeAsync(ExpandedNodeId typeId, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.FetchTypeTreeAsync(typeId, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task FetchTypeTreeAsync(ExpandedNodeIdCollection typeIds, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.FetchTypeTreeAsync(typeIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ReferenceDescriptionCollection ReadAvailableEncodings(NodeId variableId)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ReadAvailableEncodings(variableId);
            }
        }

        /// <inheritdoc/>
        public ReferenceDescription FindDataDescription(NodeId encodingId)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.FindDataDescription(encodingId);
            }
        }

        /// <inheritdoc/>
        public async Task<DataDictionary> FindDataDictionaryAsync(NodeId descriptionId, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.FindDataDictionaryAsync(descriptionId, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public DataDictionary LoadDataDictionary(ReferenceDescription dictionaryNode, bool forceReload = false)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.LoadDataDictionary(dictionaryNode, forceReload);
            }
        }

        /// <inheritdoc/>
        public async Task<Dictionary<NodeId, DataDictionary>> LoadDataTypeSystemAsync(NodeId dataTypeSystem = null, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.LoadDataTypeSystemAsync(dataTypeSystem, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Node ReadNode(NodeId nodeId)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ReadNode(nodeId);
            }
        }

        /// <inheritdoc/>
        public Node ReadNode(NodeId nodeId, NodeClass nodeClass, bool optionalAttributes = true)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ReadNode(nodeId, nodeClass, optionalAttributes);
            }
        }

        /// <inheritdoc/>
        public void ReadNodes(IList<NodeId> nodeIds, out IList<Node> nodeCollection, out IList<ServiceResult> errors, bool optionalAttributes = false)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.ReadNodes(nodeIds, out nodeCollection, out errors, optionalAttributes);
            }
        }

        /// <inheritdoc/>
        public void ReadNodes(IList<NodeId> nodeIds, NodeClass nodeClass, out IList<Node> nodeCollection, out IList<ServiceResult> errors, bool optionalAttributes = false)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.ReadNodes(nodeIds, nodeClass, out nodeCollection, out errors, optionalAttributes);
            }
        }

        /// <inheritdoc/>
        public DataValue ReadValue(NodeId nodeId)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ReadValue(nodeId);
            }
        }

        /// <inheritdoc/>
        public object ReadValue(NodeId nodeId, Type expectedType)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ReadValue(nodeId, expectedType);
            }
        }

        /// <inheritdoc/>
        public void ReadValues(IList<NodeId> nodeIds, out DataValueCollection values, out IList<ServiceResult> errors)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.ReadValues(nodeIds, out values, out errors);
            }
        }

        /// <inheritdoc/>
        public ReferenceDescriptionCollection FetchReferences(NodeId nodeId)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.FetchReferences(nodeId);
            }
        }

        /// <inheritdoc/>
        public void FetchReferences(IList<NodeId> nodeIds, out IList<ReferenceDescriptionCollection> referenceDescriptions, out IList<ServiceResult> errors)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.FetchReferences(nodeIds, out referenceDescriptions, out errors);
            }
        }

        /// <inheritdoc/>
        public async Task<ReferenceDescriptionCollection> FetchReferencesAsync(NodeId nodeId, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.FetchReferencesAsync(nodeId, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<(IList<ReferenceDescriptionCollection>, IList<ServiceResult>)> FetchReferencesAsync(IList<NodeId> nodeIds, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.FetchReferencesAsync(nodeIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public void Open(string sessionName, IUserIdentity identity)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Open(sessionName, identity);
            }
        }

        /// <inheritdoc/>
        public void Open(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Open(sessionName, sessionTimeout, identity, preferredLocales);
            }
        }

        /// <inheritdoc/>
        public void Open(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales, bool checkDomain)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.Open(sessionName, sessionTimeout, identity, preferredLocales, checkDomain);
            }
        }

        /// <inheritdoc/>
        public void ChangePreferredLocales(StringCollection preferredLocales)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.ChangePreferredLocales(preferredLocales);
            }
        }

        /// <inheritdoc/>
        public void UpdateSession(IUserIdentity identity, StringCollection preferredLocales)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.UpdateSession(identity, preferredLocales);
            }
        }

        /// <inheritdoc/>
        public void FindComponentIds(NodeId instanceId, IList<string> componentPaths, out NodeIdCollection componentIds, out List<ServiceResult> errors)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.FindComponentIds(instanceId, componentPaths, out componentIds, out errors);
            }
        }

        /// <inheritdoc/>
        public void ReadValues(IList<NodeId> variableIds, IList<Type> expectedTypes, out List<object> values, out List<ServiceResult> errors)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.ReadValues(variableIds, expectedTypes, out values, out errors);
            }
        }

        /// <inheritdoc/>
        public void ReadDisplayName(IList<NodeId> nodeIds, out IList<string> displayNames, out IList<ServiceResult> errors)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.ReadDisplayName(nodeIds, out displayNames, out errors);
            }
        }

        /// <inheritdoc/>
        public async Task OpenAsync(string sessionName, IUserIdentity identity, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.OpenAsync(sessionName, identity, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task OpenAsync(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.OpenAsync(sessionName, sessionTimeout, identity, preferredLocales, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task OpenAsync(string sessionName, uint sessionTimeout, IUserIdentity identity, IList<string> preferredLocales, bool checkDomain, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.OpenAsync(sessionName, sessionTimeout, identity, preferredLocales, checkDomain, ct).ConfigureAwait(false);
            }
        }


        /// <inheritdoc/>
        public async Task FetchNamespaceTablesAsync(CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                await session_.FetchNamespaceTablesAsync(ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<(IList<Node>, IList<ServiceResult>)> ReadNodesAsync(IList<NodeId> nodeIds, NodeClass nodeClass, bool optionalAttributes = false, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadNodesAsync(nodeIds, nodeClass, optionalAttributes, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<DataValue> ReadValueAsync(NodeId nodeId, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadValueAsync(nodeId, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<Node> ReadNodeAsync(NodeId nodeId, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadNodeAsync(nodeId, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<Node> ReadNodeAsync(NodeId nodeId, NodeClass nodeClass, bool optionalAttributes = true, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadNodeAsync(nodeId, nodeClass, optionalAttributes, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<(IList<Node>, IList<ServiceResult>)> ReadNodesAsync(IList<NodeId> nodeIds, bool optionalAttributes = false, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadNodesAsync(nodeIds, optionalAttributes, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<(DataValueCollection, IList<ServiceResult>)> ReadValuesAsync(IList<NodeId> nodeIds, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadValuesAsync(nodeIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public StatusCode Close(int timeout)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Close(timeout);
            }
        }

        /// <inheritdoc/>
        public StatusCode Close(bool closeChannel)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Close(closeChannel);
            }
        }

        /// <inheritdoc/>
        public StatusCode Close(int timeout, bool closeChannel)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Close(timeout, closeChannel);
            }
        }

        /// <inheritdoc/>
        public async Task<StatusCode> CloseAsync(CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CloseAsync(ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<StatusCode> CloseAsync(bool closeChannel, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CloseAsync(closeChannel, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<StatusCode> CloseAsync(int timeout, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CloseAsync(timeout, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<StatusCode> CloseAsync(int timeout, bool closeChannel, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CloseAsync(timeout, closeChannel, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public bool AddSubscription(Subscription subscription)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.AddSubscription(subscription);
            }
        }

        /// <inheritdoc/>
        public bool RemoveSubscription(Subscription subscription)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.RemoveSubscription(subscription);
            }
        }

        /// <inheritdoc/>
        public bool RemoveSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.RemoveSubscriptions(subscriptions);
            }
        }

        /// <inheritdoc/>
        public bool TransferSubscriptions(SubscriptionCollection subscriptions, bool sendInitialValues)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.TransferSubscriptions(subscriptions, sendInitialValues);
            }
        }

        /// <inheritdoc/>
        public bool RemoveTransferredSubscription(Subscription subscription)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.RemoveTransferredSubscription(subscription);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveSubscriptionAsync(Subscription subscription)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RemoveSubscriptionAsync(subscription).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveSubscriptionsAsync(IEnumerable<Subscription> subscriptions)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RemoveSubscriptionsAsync(subscriptions).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Browse(RequestHeader requestHeader, ViewDescription view, NodeId nodeToBrowse, uint maxResultsToReturn, BrowseDirection browseDirection, NodeId referenceTypeId, bool includeSubtypes, uint nodeClassMask, out byte[] continuationPoint, out ReferenceDescriptionCollection references)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Browse(requestHeader, view, nodeToBrowse, maxResultsToReturn, browseDirection, referenceTypeId, includeSubtypes, nodeClassMask, out continuationPoint, out references);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginBrowse(RequestHeader requestHeader, ViewDescription view, NodeId nodeToBrowse, uint maxResultsToReturn, BrowseDirection browseDirection, NodeId referenceTypeId, bool includeSubtypes, uint nodeClassMask, AsyncCallback callback, object asyncState)
        {
            return session_.BeginBrowse(requestHeader, view, nodeToBrowse, maxResultsToReturn, browseDirection, referenceTypeId, includeSubtypes, nodeClassMask, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndBrowse(IAsyncResult result, out byte[] continuationPoint, out ReferenceDescriptionCollection references)
        {
            return session_.EndBrowse(result, out continuationPoint, out references);
        }

        /// <inheritdoc/>
        public ResponseHeader BrowseNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection references)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.BrowseNext(requestHeader, releaseContinuationPoint, continuationPoint, out revisedContinuationPoint, out references);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginBrowseNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, AsyncCallback callback, object asyncState)
        {
            return session_.BeginBrowseNext(requestHeader, releaseContinuationPoint, continuationPoint, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndBrowseNext(IAsyncResult result, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection references)
        {
            return session_.EndBrowseNext(result, out revisedContinuationPoint, out references);
        }

        /// <inheritdoc/>
        public IList<object> Call(NodeId objectId, NodeId methodId, params object[] args)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Call(objectId, methodId, args);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPublish(int timeout)
        {
            return session_.BeginPublish(timeout);
        }

        /// <inheritdoc/>
        public void StartPublishing(int timeout, bool fullQueue)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.StartPublishing(timeout, fullQueue);
            }
        }

        /// <inheritdoc/>
        public bool Republish(uint subscriptionId, uint sequenceNumber, out ServiceResult error)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Republish(subscriptionId, sequenceNumber, out error);
            }
        }

        /// <inheritdoc/>
        public async Task<(bool, ServiceResult)> RepublishAsync(uint subscriptionId, uint sequenceNumber, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RepublishAsync(subscriptionId, sequenceNumber, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader CreateSession(RequestHeader requestHeader, ApplicationDescription clientDescription, string serverUri, string endpointUrl, string sessionName, byte[] clientNonce, byte[] clientCertificate, double requestedSessionTimeout, uint maxResponseMessageSize, out NodeId sessionId, out NodeId authenticationToken, out double revisedSessionTimeout, out byte[] serverNonce, out byte[] serverCertificate, out EndpointDescriptionCollection serverEndpoints, out SignedSoftwareCertificateCollection serverSoftwareCertificates, out SignatureData serverSignature, out uint maxRequestMessageSize)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.CreateSession(requestHeader, clientDescription, serverUri, endpointUrl, sessionName, clientNonce, clientCertificate, requestedSessionTimeout, maxResponseMessageSize, out sessionId, out authenticationToken, out revisedSessionTimeout, out serverNonce, out serverCertificate, out serverEndpoints, out serverSoftwareCertificates, out serverSignature, out maxRequestMessageSize);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCreateSession(RequestHeader requestHeader, ApplicationDescription clientDescription, string serverUri, string endpointUrl, string sessionName, byte[] clientNonce, byte[] clientCertificate, double requestedSessionTimeout, uint maxResponseMessageSize, AsyncCallback callback, object asyncState)
        {
            return session_.BeginCreateSession(requestHeader, clientDescription, serverUri, endpointUrl, sessionName, clientNonce, clientCertificate, requestedSessionTimeout, maxResponseMessageSize, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndCreateSession(IAsyncResult result, out NodeId sessionId, out NodeId authenticationToken, out double revisedSessionTimeout, out byte[] serverNonce, out byte[] serverCertificate, out EndpointDescriptionCollection serverEndpoints, out SignedSoftwareCertificateCollection serverSoftwareCertificates, out SignatureData serverSignature, out uint maxRequestMessageSize)
        {
            return session_.EndCreateSession(result, out sessionId, out authenticationToken, out revisedSessionTimeout, out serverNonce, out serverCertificate, out serverEndpoints, out serverSoftwareCertificates, out serverSignature, out maxRequestMessageSize);
        }

        /// <inheritdoc/>
        public async Task<CreateSessionResponse> CreateSessionAsync(RequestHeader requestHeader, ApplicationDescription clientDescription, string serverUri, string endpointUrl, string sessionName, byte[] clientNonce, byte[] clientCertificate, double requestedSessionTimeout, uint maxResponseMessageSize, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CreateSessionAsync(requestHeader, clientDescription, serverUri, endpointUrl, sessionName, clientNonce, clientCertificate, requestedSessionTimeout, maxResponseMessageSize, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader ActivateSession(RequestHeader requestHeader, SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature, out byte[] serverNonce, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, out serverNonce, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginActivateSession(RequestHeader requestHeader, SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature, AsyncCallback callback, object asyncState)
        {
            return session_.BeginActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndActivateSession(IAsyncResult result, out byte[] serverNonce, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndActivateSession(result, out serverNonce, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<ActivateSessionResponse> ActivateSessionAsync(RequestHeader requestHeader, SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ActivateSessionAsync(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader CloseSession(RequestHeader requestHeader, bool deleteSubscriptions)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.CloseSession(requestHeader, deleteSubscriptions);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCloseSession(RequestHeader requestHeader, bool deleteSubscriptions, AsyncCallback callback, object asyncState)
        {
            return session_.BeginCloseSession(requestHeader, deleteSubscriptions, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndCloseSession(IAsyncResult result)
        {
            return session_.EndCloseSession(result);
        }

        /// <inheritdoc/>
        public async Task<CloseSessionResponse> CloseSessionAsync(RequestHeader requestHeader, bool deleteSubscriptions, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CloseSessionAsync(requestHeader, deleteSubscriptions, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Cancel(RequestHeader requestHeader, uint requestHandle, out uint cancelCount)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Cancel(requestHeader, requestHandle, out cancelCount);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCancel(RequestHeader requestHeader, uint requestHandle, AsyncCallback callback, object asyncState)
        {
            return session_.BeginCancel(requestHeader, requestHandle, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndCancel(IAsyncResult result, out uint cancelCount)
        {
            return session_.EndCancel(result, out cancelCount);
        }

        /// <inheritdoc/>
        public async Task<CancelResponse> CancelAsync(RequestHeader requestHeader, uint requestHandle, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CancelAsync(requestHeader, requestHandle, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader AddNodes(RequestHeader requestHeader, AddNodesItemCollection nodesToAdd, out AddNodesResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.AddNodes(requestHeader, nodesToAdd, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginAddNodes(RequestHeader requestHeader, AddNodesItemCollection nodesToAdd, AsyncCallback callback, object asyncState)
        {
            return session_.BeginAddNodes(requestHeader, nodesToAdd, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndAddNodes(IAsyncResult result, out AddNodesResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndAddNodes(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<AddNodesResponse> AddNodesAsync(RequestHeader requestHeader, AddNodesItemCollection nodesToAdd, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.AddNodesAsync(requestHeader, nodesToAdd, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader AddReferences(RequestHeader requestHeader, AddReferencesItemCollection referencesToAdd, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.AddReferences(requestHeader, referencesToAdd, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginAddReferences(RequestHeader requestHeader, AddReferencesItemCollection referencesToAdd, AsyncCallback callback, object asyncState)
        {
            return session_.BeginAddReferences(requestHeader, referencesToAdd, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndAddReferences(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndAddReferences(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<AddReferencesResponse> AddReferencesAsync(RequestHeader requestHeader, AddReferencesItemCollection referencesToAdd, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.AddReferencesAsync(requestHeader, referencesToAdd, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader DeleteNodes(RequestHeader requestHeader, DeleteNodesItemCollection nodesToDelete, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.DeleteNodes(requestHeader, nodesToDelete, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginDeleteNodes(RequestHeader requestHeader, DeleteNodesItemCollection nodesToDelete, AsyncCallback callback, object asyncState)
        {
            return session_.BeginDeleteNodes(requestHeader, nodesToDelete, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndDeleteNodes(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndDeleteNodes(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<DeleteNodesResponse> DeleteNodesAsync(RequestHeader requestHeader, DeleteNodesItemCollection nodesToDelete, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.DeleteNodesAsync(requestHeader, nodesToDelete, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader DeleteReferences(RequestHeader requestHeader, DeleteReferencesItemCollection referencesToDelete, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.DeleteReferences(requestHeader, referencesToDelete, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginDeleteReferences(RequestHeader requestHeader, DeleteReferencesItemCollection referencesToDelete, AsyncCallback callback, object asyncState)
        {
            return session_.BeginDeleteReferences(requestHeader, referencesToDelete, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndDeleteReferences(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndDeleteReferences(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<DeleteReferencesResponse> DeleteReferencesAsync(RequestHeader requestHeader, DeleteReferencesItemCollection referencesToDelete, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.DeleteReferencesAsync(requestHeader, referencesToDelete, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Browse(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode, BrowseDescriptionCollection nodesToBrowse, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Browse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginBrowse(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode, BrowseDescriptionCollection nodesToBrowse, AsyncCallback callback, object asyncState)
        {
            return session_.BeginBrowse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndBrowse(IAsyncResult result, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndBrowse(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<BrowseResponse> BrowseAsync(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode, BrowseDescriptionCollection nodesToBrowse, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.BrowseAsync(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader BrowseNext(RequestHeader requestHeader, bool releaseContinuationPoints, ByteStringCollection continuationPoints, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.BrowseNext(requestHeader, releaseContinuationPoints, continuationPoints, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginBrowseNext(RequestHeader requestHeader, bool releaseContinuationPoints, ByteStringCollection continuationPoints, AsyncCallback callback, object asyncState)
        {
            return session_.BeginBrowseNext(requestHeader, releaseContinuationPoints, continuationPoints, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndBrowseNext(IAsyncResult result, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndBrowseNext(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<BrowseNextResponse> BrowseNextAsync(RequestHeader requestHeader, bool releaseContinuationPoints, ByteStringCollection continuationPoints, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.BrowseNextAsync(requestHeader, releaseContinuationPoints, continuationPoints, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader TranslateBrowsePathsToNodeIds(RequestHeader requestHeader, BrowsePathCollection browsePaths, out BrowsePathResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.TranslateBrowsePathsToNodeIds(requestHeader, browsePaths, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginTranslateBrowsePathsToNodeIds(RequestHeader requestHeader, BrowsePathCollection browsePaths, AsyncCallback callback, object asyncState)
        {
            return session_.BeginTranslateBrowsePathsToNodeIds(requestHeader, browsePaths, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndTranslateBrowsePathsToNodeIds(IAsyncResult result, out BrowsePathResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndTranslateBrowsePathsToNodeIds(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<TranslateBrowsePathsToNodeIdsResponse> TranslateBrowsePathsToNodeIdsAsync(RequestHeader requestHeader, BrowsePathCollection browsePaths, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.TranslateBrowsePathsToNodeIdsAsync(requestHeader, browsePaths, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader RegisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToRegister, out NodeIdCollection registeredNodeIds)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.RegisterNodes(requestHeader, nodesToRegister, out registeredNodeIds);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginRegisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToRegister, AsyncCallback callback, object asyncState)
        {
            return session_.BeginRegisterNodes(requestHeader, nodesToRegister, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndRegisterNodes(IAsyncResult result, out NodeIdCollection registeredNodeIds)
        {
            return session_.EndRegisterNodes(result, out registeredNodeIds);
        }

        /// <inheritdoc/>
        public async Task<RegisterNodesResponse> RegisterNodesAsync(RequestHeader requestHeader, NodeIdCollection nodesToRegister, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RegisterNodesAsync(requestHeader, nodesToRegister, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader UnregisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToUnregister)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.UnregisterNodes(requestHeader, nodesToUnregister);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginUnregisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToUnregister, AsyncCallback callback, object asyncState)
        {
            return session_.BeginUnregisterNodes(requestHeader, nodesToUnregister, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndUnregisterNodes(IAsyncResult result)
        {
            return session_.EndUnregisterNodes(result);
        }

        /// <inheritdoc/>
        public async Task<UnregisterNodesResponse> UnregisterNodesAsync(RequestHeader requestHeader, NodeIdCollection nodesToUnregister, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.UnregisterNodesAsync(requestHeader, nodesToUnregister, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader QueryFirst(RequestHeader requestHeader, ViewDescription view, NodeTypeDescriptionCollection nodeTypes, ContentFilter filter, uint maxDataSetsToReturn, uint maxReferencesToReturn, out QueryDataSetCollection queryDataSets, out byte[] continuationPoint, out ParsingResultCollection parsingResults, out DiagnosticInfoCollection diagnosticInfos, out ContentFilterResult filterResult)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.QueryFirst(requestHeader, view, nodeTypes, filter, maxDataSetsToReturn, maxReferencesToReturn, out queryDataSets, out continuationPoint, out parsingResults, out diagnosticInfos, out filterResult);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginQueryFirst(RequestHeader requestHeader, ViewDescription view, NodeTypeDescriptionCollection nodeTypes, ContentFilter filter, uint maxDataSetsToReturn, uint maxReferencesToReturn, AsyncCallback callback, object asyncState)
        {
            return session_.BeginQueryFirst(requestHeader, view, nodeTypes, filter, maxDataSetsToReturn, maxReferencesToReturn, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndQueryFirst(IAsyncResult result, out QueryDataSetCollection queryDataSets, out byte[] continuationPoint, out ParsingResultCollection parsingResults, out DiagnosticInfoCollection diagnosticInfos, out ContentFilterResult filterResult)
        {
            return session_.EndQueryFirst(result, out queryDataSets, out continuationPoint, out parsingResults, out diagnosticInfos, out filterResult);
        }

        /// <inheritdoc/>
        public async Task<QueryFirstResponse> QueryFirstAsync(RequestHeader requestHeader, ViewDescription view, NodeTypeDescriptionCollection nodeTypes, ContentFilter filter, uint maxDataSetsToReturn, uint maxReferencesToReturn, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.QueryFirstAsync(requestHeader, view, nodeTypes, filter, maxDataSetsToReturn, maxReferencesToReturn, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader QueryNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, out QueryDataSetCollection queryDataSets, out byte[] revisedContinuationPoint)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.QueryNext(requestHeader, releaseContinuationPoint, continuationPoint, out queryDataSets, out revisedContinuationPoint);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginQueryNext(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, AsyncCallback callback, object asyncState)
        {
            return session_.BeginQueryNext(requestHeader, releaseContinuationPoint, continuationPoint, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndQueryNext(IAsyncResult result, out QueryDataSetCollection queryDataSets, out byte[] revisedContinuationPoint)
        {
            return session_.EndQueryNext(result, out queryDataSets, out revisedContinuationPoint);
        }

        /// <inheritdoc/>
        public async Task<QueryNextResponse> QueryNextAsync(RequestHeader requestHeader, bool releaseContinuationPoint, byte[] continuationPoint, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.QueryNextAsync(requestHeader, releaseContinuationPoint, continuationPoint, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Read(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn, ReadValueIdCollection nodesToRead, out DataValueCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Read(requestHeader, maxAge, timestampsToReturn, nodesToRead, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginRead(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn, ReadValueIdCollection nodesToRead, AsyncCallback callback, object asyncState)
        {
            return session_.BeginRead(requestHeader, maxAge, timestampsToReturn, nodesToRead, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndRead(IAsyncResult result, out DataValueCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndRead(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<ReadResponse> ReadAsync(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn, ReadValueIdCollection nodesToRead, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReadAsync(requestHeader, maxAge, timestampsToReturn, nodesToRead, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader HistoryRead(RequestHeader requestHeader, ExtensionObject historyReadDetails, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead, out HistoryReadResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.HistoryRead(requestHeader, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginHistoryRead(RequestHeader requestHeader, ExtensionObject historyReadDetails, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead, AsyncCallback callback, object asyncState)
        {
            return session_.BeginHistoryRead(requestHeader, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndHistoryRead(IAsyncResult result, out HistoryReadResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndHistoryRead(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<HistoryReadResponse> HistoryReadAsync(RequestHeader requestHeader, ExtensionObject historyReadDetails, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints, HistoryReadValueIdCollection nodesToRead, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.HistoryReadAsync(requestHeader, historyReadDetails, timestampsToReturn, releaseContinuationPoints, nodesToRead, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Write(RequestHeader requestHeader, WriteValueCollection nodesToWrite, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Write(requestHeader, nodesToWrite, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginWrite(RequestHeader requestHeader, WriteValueCollection nodesToWrite, AsyncCallback callback, object asyncState)
        {
            return session_.BeginWrite(requestHeader, nodesToWrite, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndWrite(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndWrite(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<WriteResponse> WriteAsync(RequestHeader requestHeader, WriteValueCollection nodesToWrite, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.WriteAsync(requestHeader, nodesToWrite, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader HistoryUpdate(RequestHeader requestHeader, ExtensionObjectCollection historyUpdateDetails, out HistoryUpdateResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.HistoryUpdate(requestHeader, historyUpdateDetails, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginHistoryUpdate(RequestHeader requestHeader, ExtensionObjectCollection historyUpdateDetails, AsyncCallback callback, object asyncState)
        {
            return session_.BeginHistoryUpdate(requestHeader, historyUpdateDetails, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndHistoryUpdate(IAsyncResult result, out HistoryUpdateResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndHistoryUpdate(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<HistoryUpdateResponse> HistoryUpdateAsync(RequestHeader requestHeader, ExtensionObjectCollection historyUpdateDetails, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.HistoryUpdateAsync(requestHeader, historyUpdateDetails, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Call(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall, out CallMethodResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Call(requestHeader, methodsToCall, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCall(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall, AsyncCallback callback, object asyncState)
        {
            return session_.BeginCall(requestHeader, methodsToCall, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndCall(IAsyncResult result, out CallMethodResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndCall(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<CallResponse> CallAsync(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CallAsync(requestHeader, methodsToCall, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader CreateMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemCreateRequestCollection itemsToCreate, out MonitoredItemCreateResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.CreateMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToCreate, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCreateMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemCreateRequestCollection itemsToCreate, AsyncCallback callback, object asyncState)
        {
            return session_.BeginCreateMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToCreate, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndCreateMonitoredItems(IAsyncResult result, out MonitoredItemCreateResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndCreateMonitoredItems(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<CreateMonitoredItemsResponse> CreateMonitoredItemsAsync(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemCreateRequestCollection itemsToCreate, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CreateMonitoredItemsAsync(requestHeader, subscriptionId, timestampsToReturn, itemsToCreate, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader ModifyMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemModifyRequestCollection itemsToModify, out MonitoredItemModifyResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ModifyMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToModify, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginModifyMonitoredItems(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemModifyRequestCollection itemsToModify, AsyncCallback callback, object asyncState)
        {
            return session_.BeginModifyMonitoredItems(requestHeader, subscriptionId, timestampsToReturn, itemsToModify, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndModifyMonitoredItems(IAsyncResult result, out MonitoredItemModifyResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndModifyMonitoredItems(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<ModifyMonitoredItemsResponse> ModifyMonitoredItemsAsync(RequestHeader requestHeader, uint subscriptionId, TimestampsToReturn timestampsToReturn, MonitoredItemModifyRequestCollection itemsToModify, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ModifyMonitoredItemsAsync(requestHeader, subscriptionId, timestampsToReturn, itemsToModify, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader SetMonitoringMode(RequestHeader requestHeader, uint subscriptionId, MonitoringMode monitoringMode, UInt32Collection monitoredItemIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.SetMonitoringMode(requestHeader, subscriptionId, monitoringMode, monitoredItemIds, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginSetMonitoringMode(RequestHeader requestHeader, uint subscriptionId, MonitoringMode monitoringMode, UInt32Collection monitoredItemIds, AsyncCallback callback, object asyncState)
        {
            return session_.BeginSetMonitoringMode(requestHeader, subscriptionId, monitoringMode, monitoredItemIds, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndSetMonitoringMode(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndSetMonitoringMode(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<SetMonitoringModeResponse> SetMonitoringModeAsync(RequestHeader requestHeader, uint subscriptionId, MonitoringMode monitoringMode, UInt32Collection monitoredItemIds, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.SetMonitoringModeAsync(requestHeader, subscriptionId, monitoringMode, monitoredItemIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader SetTriggering(RequestHeader requestHeader, uint subscriptionId, uint triggeringItemId, UInt32Collection linksToAdd, UInt32Collection linksToRemove, out StatusCodeCollection addResults, out DiagnosticInfoCollection addDiagnosticInfos, out StatusCodeCollection removeResults, out DiagnosticInfoCollection removeDiagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.SetTriggering(requestHeader, subscriptionId, triggeringItemId, linksToAdd, linksToRemove, out addResults, out addDiagnosticInfos, out removeResults, out removeDiagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginSetTriggering(RequestHeader requestHeader, uint subscriptionId, uint triggeringItemId, UInt32Collection linksToAdd, UInt32Collection linksToRemove, AsyncCallback callback, object asyncState)
        {
            return session_.BeginSetTriggering(requestHeader, subscriptionId, triggeringItemId, linksToAdd, linksToRemove, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndSetTriggering(IAsyncResult result, out StatusCodeCollection addResults, out DiagnosticInfoCollection addDiagnosticInfos, out StatusCodeCollection removeResults, out DiagnosticInfoCollection removeDiagnosticInfos)
        {
            return session_.EndSetTriggering(result, out addResults, out addDiagnosticInfos, out removeResults, out removeDiagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<SetTriggeringResponse> SetTriggeringAsync(RequestHeader requestHeader, uint subscriptionId, uint triggeringItemId, UInt32Collection linksToAdd, UInt32Collection linksToRemove, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.SetTriggeringAsync(requestHeader, subscriptionId, triggeringItemId, linksToAdd, linksToRemove, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader DeleteMonitoredItems(RequestHeader requestHeader, uint subscriptionId, UInt32Collection monitoredItemIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.DeleteMonitoredItems(requestHeader, subscriptionId, monitoredItemIds, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginDeleteMonitoredItems(RequestHeader requestHeader, uint subscriptionId, UInt32Collection monitoredItemIds, AsyncCallback callback, object asyncState)
        {
            return session_.BeginDeleteMonitoredItems(requestHeader, subscriptionId, monitoredItemIds, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndDeleteMonitoredItems(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndDeleteMonitoredItems(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<DeleteMonitoredItemsResponse> DeleteMonitoredItemsAsync(RequestHeader requestHeader, uint subscriptionId, UInt32Collection monitoredItemIds, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.DeleteMonitoredItemsAsync(requestHeader, subscriptionId, monitoredItemIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader CreateSubscription(RequestHeader requestHeader, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, bool publishingEnabled, byte priority, out uint subscriptionId, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.CreateSubscription(requestHeader, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, publishingEnabled, priority, out subscriptionId, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginCreateSubscription(RequestHeader requestHeader, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, bool publishingEnabled, byte priority, AsyncCallback callback, object asyncState)
        {
            return session_.BeginCreateSubscription(requestHeader, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, publishingEnabled, priority, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndCreateSubscription(IAsyncResult result, out uint subscriptionId, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount)
        {
            return session_.EndCreateSubscription(result, out subscriptionId, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);
        }

        /// <inheritdoc/>
        public async Task<CreateSubscriptionResponse> CreateSubscriptionAsync(RequestHeader requestHeader, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, bool publishingEnabled, byte priority, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CreateSubscriptionAsync(requestHeader, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, publishingEnabled, priority, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader ModifySubscription(RequestHeader requestHeader, uint subscriptionId, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, byte priority, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ModifySubscription(requestHeader, subscriptionId, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, priority, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginModifySubscription(RequestHeader requestHeader, uint subscriptionId, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, byte priority, AsyncCallback callback, object asyncState)
        {
            return session_.BeginModifySubscription(requestHeader, subscriptionId, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, priority, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndModifySubscription(IAsyncResult result, out double revisedPublishingInterval, out uint revisedLifetimeCount, out uint revisedMaxKeepAliveCount)
        {
            return session_.EndModifySubscription(result, out revisedPublishingInterval, out revisedLifetimeCount, out revisedMaxKeepAliveCount);
        }

        /// <inheritdoc/>
        public async Task<ModifySubscriptionResponse> ModifySubscriptionAsync(RequestHeader requestHeader, uint subscriptionId, double requestedPublishingInterval, uint requestedLifetimeCount, uint requestedMaxKeepAliveCount, uint maxNotificationsPerPublish, byte priority, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ModifySubscriptionAsync(requestHeader, subscriptionId, requestedPublishingInterval, requestedLifetimeCount, requestedMaxKeepAliveCount, maxNotificationsPerPublish, priority, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader SetPublishingMode(RequestHeader requestHeader, bool publishingEnabled, UInt32Collection subscriptionIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.SetPublishingMode(requestHeader, publishingEnabled, subscriptionIds, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginSetPublishingMode(RequestHeader requestHeader, bool publishingEnabled, UInt32Collection subscriptionIds, AsyncCallback callback, object asyncState)
        {
            return session_.BeginSetPublishingMode(requestHeader, publishingEnabled, subscriptionIds, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndSetPublishingMode(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndSetPublishingMode(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<SetPublishingModeResponse> SetPublishingModeAsync(RequestHeader requestHeader, bool publishingEnabled, UInt32Collection subscriptionIds, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.SetPublishingModeAsync(requestHeader, publishingEnabled, subscriptionIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Publish(RequestHeader requestHeader, SubscriptionAcknowledgementCollection subscriptionAcknowledgements, out uint subscriptionId, out UInt32Collection availableSequenceNumbers, out bool moreNotifications, out NotificationMessage notificationMessage, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Publish(requestHeader, subscriptionAcknowledgements, out subscriptionId, out availableSequenceNumbers, out moreNotifications, out notificationMessage, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginPublish(RequestHeader requestHeader, SubscriptionAcknowledgementCollection subscriptionAcknowledgements, AsyncCallback callback, object asyncState)
        {
            return session_.BeginPublish(requestHeader, subscriptionAcknowledgements, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndPublish(IAsyncResult result, out uint subscriptionId, out UInt32Collection availableSequenceNumbers, out bool moreNotifications, out NotificationMessage notificationMessage, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndPublish(result, out subscriptionId, out availableSequenceNumbers, out moreNotifications, out notificationMessage, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<PublishResponse> PublishAsync(RequestHeader requestHeader, SubscriptionAcknowledgementCollection subscriptionAcknowledgements, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.PublishAsync(requestHeader, subscriptionAcknowledgements, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader Republish(RequestHeader requestHeader, uint subscriptionId, uint retransmitSequenceNumber, out NotificationMessage notificationMessage)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Republish(requestHeader, subscriptionId, retransmitSequenceNumber, out notificationMessage);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginRepublish(RequestHeader requestHeader, uint subscriptionId, uint retransmitSequenceNumber, AsyncCallback callback, object asyncState)
        {
            return session_.BeginRepublish(requestHeader, subscriptionId, retransmitSequenceNumber, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndRepublish(IAsyncResult result, out NotificationMessage notificationMessage)
        {
            return session_.EndRepublish(result, out notificationMessage);
        }

        /// <inheritdoc/>
        public async Task<RepublishResponse> RepublishAsync(RequestHeader requestHeader, uint subscriptionId, uint retransmitSequenceNumber, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RepublishAsync(requestHeader, subscriptionId, retransmitSequenceNumber, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader TransferSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, bool sendInitialValues, out TransferResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.TransferSubscriptions(requestHeader, subscriptionIds, sendInitialValues, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginTransferSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, bool sendInitialValues, AsyncCallback callback, object asyncState)
        {
            return session_.BeginTransferSubscriptions(requestHeader, subscriptionIds, sendInitialValues, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndTransferSubscriptions(IAsyncResult result, out TransferResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndTransferSubscriptions(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<TransferSubscriptionsResponse> TransferSubscriptionsAsync(RequestHeader requestHeader, UInt32Collection subscriptionIds, bool sendInitialValues, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.TransferSubscriptionsAsync(requestHeader, subscriptionIds, sendInitialValues, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public ResponseHeader DeleteSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.DeleteSubscriptions(requestHeader, subscriptionIds, out results, out diagnosticInfos);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginDeleteSubscriptions(RequestHeader requestHeader, UInt32Collection subscriptionIds, AsyncCallback callback, object asyncState)
        {
            return session_.BeginDeleteSubscriptions(requestHeader, subscriptionIds, callback, asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndDeleteSubscriptions(IAsyncResult result, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
        {
            return session_.EndDeleteSubscriptions(result, out results, out diagnosticInfos);
        }

        /// <inheritdoc/>
        public async Task<DeleteSubscriptionsResponse> DeleteSubscriptionsAsync(RequestHeader requestHeader, UInt32Collection subscriptionIds, CancellationToken ct)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.DeleteSubscriptionsAsync(requestHeader, subscriptionIds, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public void AttachChannel(ITransportChannel channel)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.AttachChannel(channel);
            }
        }

        /// <inheritdoc/>
        public void DetachChannel()
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                session_.DetachChannel();
            }
        }

        /// <inheritdoc/>
        public StatusCode Close()
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.Close();
            }
        }

        /// <inheritdoc/>
        public uint NewRequestHandle()
        {
            return session_.NewRequestHandle();
        }

        /// <summary>
        /// Disposes the session.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // note: do not null the session here,
                // properties may still be accessed after dispose.
                Utils.SilentDispose(session_);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public SessionConfiguration SaveSessionConfiguration(Stream stream = null)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.SaveSessionConfiguration(stream);
            }
        }

        /// <inheritdoc/>
        public bool ApplySessionConfiguration(SessionConfiguration sessionConfiguration)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ApplySessionConfiguration(sessionConfiguration);
            }
        }

        /// <inheritdoc/>
        public bool ReactivateSubscriptions(SubscriptionCollection subscriptions, bool sendInitialValues)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.ReactivateSubscriptions(subscriptions, sendInitialValues);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveSubscriptionAsync(Subscription subscription, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RemoveSubscriptionAsync(subscription, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveSubscriptionsAsync(IEnumerable<Subscription> subscriptions, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.RemoveSubscriptionsAsync(subscriptions, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ReactivateSubscriptionsAsync(SubscriptionCollection subscriptions, bool sendInitialValues, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ReactivateSubscriptionsAsync(subscriptions, sendInitialValues, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> TransferSubscriptionsAsync(SubscriptionCollection subscriptions, bool sendInitialValues, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.TransferSubscriptionsAsync(subscriptions, sendInitialValues, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<object>> CallAsync(NodeId objectId, NodeId methodId, CancellationToken ct = default, params object[] args)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.CallAsync(objectId, methodId, ct, args).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public bool ResendData(IEnumerable<Subscription> subscriptions, out IList<ServiceResult> errors)
        {
            using (Activity activity = ActivitySource.StartActivity(nameof(ResendData)))
            {
                return session_.ResendData(subscriptions, out errors);
            }
        }

        /// <inheritdoc/>
        public async Task<(bool, IList<ServiceResult>)> ResendDataAsync(IEnumerable<Subscription> subscriptions, CancellationToken ct = default)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return await session_.ResendDataAsync(subscriptions, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginReadValues(
                                IList<NodeId> variableIds,
                                double maxAge,
                                TimestampsToReturn timestampsToReturn,
                                AsyncCallback callback,
                                object userData)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.BeginReadValues(variableIds, maxAge, timestampsToReturn, callback, userData);
            }
        }

        /// <inheritdoc/>
        public List<DataValue> EndReadValues(
            IAsyncResult result)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.EndReadValues(result);
            }
        }

        /// <inheritdoc/>
        public StatusCode WriteValue(NodeId nodeId, DataValue value)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.WriteValue(nodeId, value);
            }
        }

        /// <inheritdoc/>
        public List<StatusCode> WriteValues(IList<NodeId> nodeIds, IList<DataValue> dataValues)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.WriteValues(nodeIds, dataValues);
            }
        }

        /// <inheritdoc/>
        public IAsyncResult BeginWriteValues(
                                IList<NodeId> nodeIds,
                                IList<DataValue> dataValues,
                                AsyncCallback callback,
                                object userData)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.BeginWriteValues(nodeIds, dataValues, callback, userData);
            }

        }

        /// <inheritdoc/>
        public List<StatusCode> EndWriteValues(
            IAsyncResult result)
        {
            using (Activity activity = ActivitySource.StartActivity())
            {
                return session_.EndWriteValues(result);
            }
        }
        #endregion
    }
}
