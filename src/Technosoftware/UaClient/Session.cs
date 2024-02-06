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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.Extensions.Logging;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Manages a session with a server.
    /// </summary>
    public partial class Session : SessionClientBatched, IUaSession
    {
        #region Constants
        private const int ReconnectTimeout = 15000;
        private const int MinPublishRequestCountMax = 100;
        private const int DefaultPublishRequestCount = 1;
        private const int KeepAliveGuardBand = 1000;
        private const int kPublishRequestSequenceNumberOutOfOrderThreshold = 10;
        private const int kPublishRequestSequenceNumberOutdatedThreshold = 100;
        #endregion

        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Constructs a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="channel">The channel used to communicate with the server.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="endpoint">The endpoint use to initialize the channel.</param>
        public Session(
            ISessionChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint)
        :
            this(channel as ITransportChannel, configuration, endpoint, null)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="IUaSession"/> class.
        /// </summary>
        /// <param name="channel">The channel used to communicate with the server.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="endpoint">The endpoint used to initialize the channel.</param>
        /// <param name="clientCertificate">The certificate to use for the client.</param>
        /// <param name="availableEndpoints">The list of available endpoints returned by server in GetEndpoints() response.</param>
        /// <param name="discoveryProfileUris">The value of profileUris used in GetEndpoints() request.</param>
        /// <remarks>
        /// The application configuration is used to look up the certificate if none is provided.
        /// The clientCertificate must have the private key. This will require that the certificate
        ///     be loaded from a certificate store. Converting a DER encoded blob to a X509Certificate2
        /// will not include a private key.
        /// The <i>availableEndpoints</i> and <i>discoveryProfileUris</i> parameters are used to validate
        /// that the list of EndpointDescriptions returned at GetEndpoints matches the list returned at CreateSession.
        /// </remarks>
        public Session(
            ITransportChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate,
            EndpointDescriptionCollection availableEndpoints = null,
            StringCollection discoveryProfileUris = null)
            :
                base(channel)
        {
            Initialize(channel, configuration, endpoint, clientCertificate);
            discoveryServerEndpoints_ = availableEndpoints;
            discoveryProfileUris_ = discoveryProfileUris;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IUaSession"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="template">The template session.</param>
        /// <param name="copyEventHandlers">if set to <c>true</c> the event handlers are copied.</param>
        public Session(ITransportChannel channel, Session template, bool copyEventHandlers)
        :
            base(channel)
        {
            Initialize(channel, template.configuration_, template.ConfiguredEndpoint, template.instanceCertificate_);

            SessionFactory = template.SessionFactory;
            DefaultSubscription = template.DefaultSubscription;
            DeleteSubscriptionsOnClose = template.DeleteSubscriptionsOnClose;
            transferSubscriptionsOnReconnect_ = template.transferSubscriptionsOnReconnect_;
            sessionTimeout_ = template.sessionTimeout_;
            maxRequestMessageSize_ = template.maxRequestMessageSize_;
            minPublishRequestCount_ = template.minPublishRequestCount_;
            preferredLocales_ = template.preferredLocales_;
            SessionName = template.SessionName;
            Handle = template.Handle;
            Identity = template.Identity;
            keepAliveInterval_ = template.keepAliveInterval_;
            checkDomain_ = template.checkDomain_;
            if (template.OperationTimeout > 0)
            {
                OperationTimeout = template.OperationTimeout;
            }

            if (copyEventHandlers)
            {
                KeepAliveEventHandler = template.KeepAliveEventHandler;
                PublishEventHandler = template.PublishEventHandler;
                PublishErrorEventHandler = template.PublishErrorEventHandler;
                PublishSequenceNumbersToAcknowledgeEventHandler = template.PublishSequenceNumbersToAcknowledgeEventHandler;
                SubscriptionsChangedEventHandler = template.SubscriptionsChangedEventHandler;
                SessionClosingEventHandler = template.SessionClosingEventHandler;
                SessionConfigurationChangedEventHandler = template.SessionConfigurationChangedEventHandler;
            }

            foreach (var subscription in template.Subscriptions)
            {
                AddSubscription(subscription.CloneSubscription(copyEventHandlers));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the channel.
        /// </summary>
        private void Initialize(
            ITransportChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate)
        {
            Initialize();

            ValidateClientConfiguration(configuration);

            // save configuration information.
            configuration_ = configuration;
            ConfiguredEndpoint = endpoint;

            // update the default subscription.
            DefaultSubscription.MinLifetimeInterval = (uint)configuration.ClientConfiguration.MinSubscriptionLifetime;

            if (ConfiguredEndpoint.Description.SecurityPolicyUri != SecurityPolicies.None)
            {
                // update client certificate.
                instanceCertificate_ = clientCertificate;

                if (clientCertificate == null)
                {
                    // load the application instance certificate.
                    if (configuration_.SecurityConfiguration.ApplicationCertificate == null)
                    {
                        throw new ServiceResultException(
                            StatusCodes.BadConfigurationError,
                            "The client configuration does not specify an application instance certificate.");
                    }

                    instanceCertificate_ = configuration_.SecurityConfiguration.ApplicationCertificate.Find(true).Result;

                }

                // check for valid certificate.
                if (instanceCertificate_ == null)
                {
                    var cert = configuration_.SecurityConfiguration.ApplicationCertificate;
                    throw ServiceResultException.Create(
                        StatusCodes.BadConfigurationError,
                        "Cannot find the application instance certificate. Store={0}, SubjectName={1}, Thumbprint={2}.",
                        cert.StorePath, cert.SubjectName, cert.Thumbprint);
                }

                // check for private key.
                if (!instanceCertificate_.HasPrivateKey)
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadConfigurationError,
                        "No private key for the application instance certificate. Subject={0}, Thumbprint={1}.",
                        instanceCertificate_.Subject,
                        instanceCertificate_.Thumbprint);
                }

                // load certificate chain.
                instanceCertificateChain_ = new X509Certificate2Collection(instanceCertificate_);
                var issuers = new List<CertificateIdentifier>();
                configuration.CertificateValidator.GetIssuers(instanceCertificate_, issuers).Wait();

                foreach (var issuer in issuers)
                {
                    instanceCertificateChain_.Add(issuer.Certificate);
                }
            }

            // initialize the message context.
            var messageContext = channel.MessageContext;

            if (messageContext != null)
            {
                NamespaceUris = messageContext.NamespaceUris;
                ServerUris = messageContext.ServerUris;
                Factory = messageContext.Factory;
            }
            else
            {
                NamespaceUris = new NamespaceTable();
                ServerUris = new StringTable();
                Factory = new EncodeableFactory(EncodeableFactory.GlobalFactory);
            }

            // initialize the NodeCache late, it needs references to the namespaceUris
            NodeCache = new NodeCache(this);

            // set the default preferred locales.
            preferredLocales_ = new[] { CultureInfo.CurrentCulture.Name };

            // create a context to use.
            systemContext_ = new SystemContext {
                SystemHandle = this,
                EncodeableFactory = Factory,
                NamespaceUris = NamespaceUris,
                ServerUris = ServerUris,
                TypeTable = TypeTree,
                PreferredLocales = null,
                SessionId = null,
                UserIdentity = null
            };
        }

        /// <summary>
        /// Sets the object members to default values.
        /// </summary>
        private void Initialize()
        {
            SessionFactory = DefaultSessionFactory.Instance;
            sessionTimeout_ = 0;
            NamespaceUris = new NamespaceTable();
            ServerUris = new StringTable();
            Factory = EncodeableFactory.GlobalFactory;
            configuration_ = null;
            instanceCertificate_ = null;
            ConfiguredEndpoint = null;
            subscriptions_ = new List<Subscription>();
            dictionaries_ = new Dictionary<NodeId, DataDictionary>();
            acknowledgementsToSend_ = new SubscriptionAcknowledgementCollection();
            acknowledgementsToSendLock_ = new object();
#if DEBUG_SEQUENTIALPUBLISHING
            latestAcknowledgementsSent_ = new Dictionary<uint, uint>();
#endif
            identityHistory_ = new List<IUserIdentity>();
            outstandingRequests_ = new LinkedList<AsyncRequestState>();
            keepAliveInterval_ = 5000;
            tooManyPublishRequests_ = 0;
            minPublishRequestCount_ = DefaultPublishRequestCount;
            SessionName = "";
            DeleteSubscriptionsOnClose = true;
            transferSubscriptionsOnReconnect_ = false;
            reconnecting_ = false;
            reconnectLock_ = new SemaphoreSlim(1, 1);

            DefaultSubscription = new Subscription {
                DisplayName = "Subscription",
                PublishingInterval = 1000,
                KeepAliveCount = 10,
                LifetimeCount = 1000,
                Priority = 255,
                PublishingEnabled = true
            };
        }

        /// <summary>
        /// Check if all required configuration fields are populated.
        /// </summary>
        private void ValidateClientConfiguration(ApplicationConfiguration configuration)
        {
            String configurationField;
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.ClientConfiguration == null)
            {
                configurationField = "ClientConfiguration";
            }
            else if (configuration.SecurityConfiguration == null)
            {
                configurationField = "SecurityConfiguration";
            }
            else if (configuration.CertificateValidator == null)
            {
                configurationField = "CertificateValidator";
            }
            else
            {
                return;
            }

            throw new ServiceResultException(
                StatusCodes.BadConfigurationError,
                $"The client configuration does not specify the {configurationField}.");
        }

        /// <summary>
        /// Validates the server nonce and security parameters of user identity.
        /// </summary>
        private void ValidateServerNonce(
            IUserIdentity identity,
            byte[] serverNonce,
            string securityPolicyUri,
            byte[] previousServerNonce,
            MessageSecurityMode channelSecurityMode = MessageSecurityMode.None)
        {
            // skip validation if server nonce is not used for encryption.
            if (String.IsNullOrEmpty(securityPolicyUri) || securityPolicyUri == SecurityPolicies.None)
            {
                return;
            }

            if (identity != null && identity.TokenType != UserTokenType.Anonymous)
            {
                // the server nonce should be validated if the token includes a secret.
                if (!Utils.Nonce.ValidateNonce(serverNonce, MessageSecurityMode.SignAndEncrypt, (uint)configuration_.SecurityConfiguration.NonceLength))
                {
                    if (channelSecurityMode == MessageSecurityMode.SignAndEncrypt ||
                        configuration_.SecurityConfiguration.SuppressNonceValidationErrors)
                    {
                        Utils.LogWarning(Utils.TraceMasks.Security, "Warning: The server nonce has not the correct length or is not random enough. The error is suppressed by user setting or because the channel is encrypted.");
                    }
                    else
                    {
                        throw ServiceResultException.Create(StatusCodes.BadNonceInvalid, "The server nonce has not the correct length or is not random enough.");
                    }
                }

                // check that new nonce is different from the previously returned server nonce.
                if (previousServerNonce != null && Utils.CompareNonce(serverNonce, previousServerNonce))
                {
                    if (channelSecurityMode == MessageSecurityMode.SignAndEncrypt ||
                        configuration_.SecurityConfiguration.SuppressNonceValidationErrors)
                    {
                        Utils.LogWarning(Utils.TraceMasks.Security, "Warning: The Server nonce is equal with previously returned nonce. The error is suppressed by user setting or because the channel is encrypted.");
                    }
                    else
                    {
                        throw ServiceResultException.Create(StatusCodes.BadNonceInvalid, "Server nonce is equal with previously returned nonce.");
                    }
                }
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Closes the session and the underlying channel.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopKeepAliveTimer();

                Utils.SilentDispose(DefaultSubscription);
                DefaultSubscription = null;

                Utils.SilentDispose(NodeCache);
                NodeCache = null;

                List<Subscription> subscriptions = null;
                lock (SyncRoot)
                {
                    subscriptions = new List<Subscription>(subscriptions_);
                    subscriptions_.Clear();
                }

                foreach (var subscription in subscriptions_)
                {
                    Utils.SilentDispose(subscription);
                }
                subscriptions.Clear();
            }

            base.Dispose(disposing);

            if (disposing)
            {
                // suppress spurious events
                KeepAliveEventHandler = null;
                PublishEventHandler = null;
                PublishErrorEventHandler = null;
                PublishSequenceNumbersToAcknowledgeEventHandler = null;
                SubscriptionsChangedEventHandler = null;
                SessionClosingEventHandler = null;
                SessionConfigurationChangedEventHandler = null;
            }
        }
        #endregion

        #region Events
        /// <summary>
        ///     Raised when a keep alive arrives from the server or an error is detected.
        /// </summary>
        /// <remarks>
        ///     Once a session is created a timer will periodically read the server state and current time.
        ///     If this read operation succeeds this event will be raised each time the keep alive period elapses.
        ///     If an error is detected (KeepAliveStopped == true) then this event will be raised as well.
        /// </remarks>
        public event EventHandler<SessionKeepAliveEventArgs> SessionKeepAliveEvent
        {
            add
            {
                KeepAliveEventHandler += value;
            }

            remove
            {
                KeepAliveEventHandler -= value;
            }
        }

        /// <summary>
        ///     Raised when a notification message arrives in a publish response.
        /// </summary>
        /// <remarks>
        ///     All publish requests are managed by the Session object. When a response arrives it is
        ///     validated and passed to the appropriate Subscription object and this event is raised.
        /// </remarks>
        public event EventHandler<SessionNotificationEventArgs> SessionNotificationEvent
        {
            add
            {
                PublishEventHandler += value;
            }

            remove
            {
                PublishEventHandler -= value;
            }
        }

        /// <summary>
        ///     Raised when an exception occurs while processing a publish response.
        /// </summary>
        /// <remarks>
        /// Exceptions in a publish response are not necessarily fatal and the Session will
        ///     attempt to recover by issuing Republish requests if missing messages are detected.
        ///     That said, timeout errors may be a symptom of a OperationTimeout that is too short
        ///     when compared to the shortest PublishingInterval/KeepAliveCount amount the current
        ///     Subscriptions. The OperationTimeout should be twice the minimum value for
        ///     PublishingInterval*KeepAliveCount.
        /// </remarks>
        public event EventHandler<SessionPublishErrorEventArgs> SessionPublishErrorEvent
        {
            add
            {
                PublishErrorEventHandler += value;
            }

            remove
            {
                PublishErrorEventHandler -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<PublishSequenceNumbersToAcknowledgeEventArgs> PublishSequenceNumbersToAcknowledgeEvent
        {
            add
            {
                PublishSequenceNumbersToAcknowledgeEventHandler += value;
            }

            remove
            {
                PublishSequenceNumbersToAcknowledgeEventHandler -= value;
            }
        }

        /// <summary>
        ///     Raised when a subscription is added or removed
        /// </summary>
        public event EventHandler SubscriptionsChangedEvent
        {
            add => SubscriptionsChangedEventHandler += value;

            remove => SubscriptionsChangedEventHandler -= value;
        }

        /// <summary>
        ///     Raised to indicate the session is closing.
        /// </summary>
        public event EventHandler SessionClosingEvent
        {
            add => SessionClosingEventHandler += value;

            remove => SessionClosingEventHandler -= value;
        }

        /// <inheritdoc/>
        public event EventHandler SessionConfigurationChangedEvent
        {
            add
            {
                SessionConfigurationChangedEventHandler += value;
            }

            remove
            {
                SessionConfigurationChangedEventHandler -= value;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// A session factory that was used to create the session.
        /// </summary>
        public IUaSessionFactory SessionFactory { get; set; }

        /// <summary>
        /// Gets the endpoint used to connect to the server.
        /// </summary>
        public ConfiguredEndpoint ConfiguredEndpoint { get; private set; }

        /// <summary>
        /// Gets the name assigned to the session.
        /// </summary>
        public string SessionName { get; private set; }

        /// <summary>
        /// Gets the period for which the server will maintain the session if there is no communication from the client.
        /// </summary>
        public double SessionTimeout { get; private set; }

        /// <summary>
        /// Gets the local handle assigned to the session.
        /// </summary>
        public object Handle { get; set; }

        /// <summary>
        /// Gets the user identity currently used for the session.
        /// </summary>
        public IUserIdentity Identity { get; private set; }

        /// <summary>
        /// Gets a list of user identities that can be used to connect to the server.
        /// </summary>
        public IEnumerable<IUserIdentity> IdentityHistory => identityHistory_;

        /// <summary>
        /// Gets the table of namespace uris known to the server.
        /// </summary>
        public NamespaceTable NamespaceUris { get; private set; }

        /// <summary>
        /// Gets the table of remote server uris known to the server.
        /// </summary>
        public StringTable ServerUris { get; private set; }

        /// <summary>
        /// Gets the system context for use with the session.
        /// </summary>
        public ISystemContext SystemContext => systemContext_;

        /// <summary>
        /// Gets the factory used to create encode-able objects that the server understands.
        /// </summary>
        public IEncodeableFactory Factory { get; private set; }

        /// <summary>
        /// Gets the cache of the server's type tree.
        /// </summary>
        public ITypeTable TypeTree => NodeCache.TypeTree;

        /// <summary>
        /// Gets the cache of nodes fetched from the server.
        /// </summary>
        public IUaNodeCache NodeCache { get; private set; }

        /// <summary>
        /// Gets the context to use for filter operations.
        /// </summary>
        public FilterContext FilterContext => new FilterContext(NamespaceUris, NodeCache.TypeTree, preferredLocales_);

        /// <summary>
        /// Gets the locales that the server should use when returning localized text.
        /// </summary>
        public StringCollection PreferredLocales => preferredLocales_;

        /// <summary>
        /// Gets the data type system dictionaries in use.
        /// </summary>
        public IReadOnlyDictionary<NodeId, DataDictionary> DataTypeSystem => dictionaries_;

        /// <summary>
        /// Gets the subscriptions owned by the session.
        /// </summary>
        public IEnumerable<Subscription> Subscriptions
        {
            get
            {
                lock (SyncRoot)
                {
                    return new ReadOnlyList<Subscription>(subscriptions_);
                }
            }
        }

        /// <summary>
        /// Gets the number of subscriptions owned by the session.
        /// </summary>
        public int SubscriptionCount
        {
            get
            {
                lock (SyncRoot)
                {
                    return subscriptions_.Count;
                }
            }
        }

        /// <summary>
        /// If the subscriptions are deleted when a session is closed. 
        /// </summary>
        /// <remarks>
        /// Default <c>true</c>, set to <c>false</c> if subscriptions need to
        /// be transferred or for durable subscriptions.
        /// </remarks>
        public bool DeleteSubscriptionsOnClose { get; set; }

        /// <summary>
        /// If the subscriptions are transferred when a session is reconnected.
        /// </summary>
        /// <remarks>
        /// Default <c>false</c>, set to <c>true</c> if subscriptions should
        /// be transferred after reconnect. Service must be supported by server.
        /// </remarks>
        public bool TransferSubscriptionsOnReconnect
        {
            get { return transferSubscriptionsOnReconnect_; }
            set { transferSubscriptionsOnReconnect_ = value; }
        }

        /// <summary>
        /// Whether the endpoint Url domain is checked in the certificate.
        /// </summary>
        public bool CheckDomain => checkDomain_;

        /// <summary>
        /// Gets or Sets the default subscription for the session.
        /// </summary>
        public Subscription DefaultSubscription { get; set; }

        /// <summary>
        /// Gets or Sets how frequently the server is pinged to see if communication is still working.
        /// </summary>
        /// <remarks>
        /// This interval controls how much time elapses before a communication error is detected.
        /// If everything is ok the KeepAlive event will be raised each time this period elapses.
        /// </remarks>
        public int KeepAliveInterval
        {
            get => keepAliveInterval_;

            set
            {
                keepAliveInterval_ = value;
                StartKeepAliveTimer();
            }
        }

        /// <summary>
        /// Returns true if the session is not receiving keep alives.
        /// </summary>
        /// <remarks>
        /// Set to true if the server does not respond for 2 times the KeepAliveInterval.
        /// Set to false is communication recovers.
        /// </remarks>
        public bool KeepAliveStopped
        {
            get
            {
                TimeSpan delta = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - Interlocked.Read(ref lastKeepAliveTime_));

                // add a guard band to allow for network lag.
                return (keepAliveInterval_ + KeepAliveGuardBand) <= delta.TotalMilliseconds;
            }
        }

        /// <summary>
        /// Gets the time of the last keep alive.
        /// </summary>
        public DateTime LastKeepAliveTime
        {
            get
            {
                var ticks = Interlocked.Read(ref lastKeepAliveTime_);
                return new DateTime(ticks, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets the number of outstanding publish or keep alive requests.
        /// </summary>
        public int OutstandingRequestCount
        {
            get
            {
                lock (outstandingRequests_)
                {
                    return outstandingRequests_.Count;
                }
            }
        }

        /// <summary>
        /// Gets the number of outstanding publish or keep alive requests which appear to be missing.
        /// </summary>
        public int DefunctRequestCount
        {
            get
            {
                lock (outstandingRequests_)
                {
                    var count = 0;

                    for (var ii = outstandingRequests_.First; ii != null; ii = ii.Next)
                    {
                        if (ii.Value.Defunct)
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }
        }

        /// <summary>
        /// Gets the number of good outstanding publish requests.
        /// </summary>
        public int GoodPublishRequestCount
        {
            get
            {
                lock (outstandingRequests_)
                {
                    var count = 0;

                    for (var ii = outstandingRequests_.First; ii != null; ii = ii.Next)
                    {
                        if (!ii.Value.Defunct && ii.Value.RequestTypeId == DataTypes.PublishRequest)
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }
        }

        /// <summary>
        /// Gets and sets the minimum number of publish requests to be used in the session.
        /// </summary>
        public int MinPublishRequestCount
        {
            get => minPublishRequestCount_;
            set
            {
                lock (SyncRoot)
                {
                    if (value >= DefaultPublishRequestCount && value <= MinPublishRequestCountMax)
                    {
                        minPublishRequestCount_ = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(MinPublishRequestCount),
                            $"Minimum publish request count must be between {DefaultPublishRequestCount} and {MinPublishRequestCountMax}.");
                    }
                }
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Creates a new communication session with a server by invoking the CreateSession service
        /// </summary>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="updateBeforeConnect">If set to <c>true</c> the discovery endpoint is used to update the endpoint description before connecting.</param>
        /// <param name="sessionName">The name to assign to the session.</param>
        /// <param name="sessionTimeout">The timeout period for the session.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="preferredLocales">The user identity to associate with the session.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The new session object</returns>
        public static Task<Session> Create(
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            return CreateAsync(configuration, endpoint, updateBeforeConnect, false, sessionName, sessionTimeout, identity, preferredLocales, ct);
        }

        /// <summary>
        /// Creates a new communication session with a server by invoking the CreateSession service
        /// </summary>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="updateBeforeConnect">If set to <c>true</c> the discovery endpoint is used to update the endpoint description before connecting.</param>
        /// <param name="checkDomain">If set to <c>true</c> then the domain in the certificate must match the endpoint used.</param>
        /// <param name="sessionName">The name to assign to the session.</param>
        /// <param name="sessionTimeout">The timeout period for the session.</param>
        /// <param name="identity">The user identity to associate with the session.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The new session object.</returns>
        public static Task<Session> CreateAsync(
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            return Create(configuration, (ITransportWaitingConnection)null, endpoint, updateBeforeConnect, checkDomain, sessionName, sessionTimeout, identity, preferredLocales, ct);
        }

        /// <summary>
        /// Creates a new session with a server using the specified channel by invoking the CreateSession service
        /// </summary>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="channel">The channel for the server.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="clientCertificate">The certificate to use for the client.</param>
        /// <param name="availableEndpoints">The list of available endpoints returned by server in GetEndpoints() response.</param>
        /// <param name="discoveryProfileUris">The value of profileUris used in GetEndpoints() request.</param>
        public static Session Create(
           ApplicationConfiguration configuration,
           ITransportChannel channel,
           ConfiguredEndpoint endpoint,
           X509Certificate2 clientCertificate,
           EndpointDescriptionCollection availableEndpoints = null,
           StringCollection discoveryProfileUris = null)
        {
            return Create(DefaultSessionFactory.Instance, configuration, channel, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris);
        }

        /// <summary>
        /// Creates a new session with a server using the specified channel by invoking the CreateSession service.
        /// With the sessionInstantiator subclasses of Sessions can be created.
        /// </summary>
        /// <param name="sessionInstantiator">The Session constructor to use to create the session.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="channel">The channel for the server.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="clientCertificate">The certificate to use for the client.</param>
        /// <param name="availableEndpoints">The list of available endpoints returned by server in GetEndpoints() response.</param>
        /// <param name="discoveryProfileUris">The value of profileUris used in GetEndpoints() request.</param>
        public static Session Create(
            IUaSessionInstantiator sessionInstantiator,
            ApplicationConfiguration configuration,
            ITransportChannel channel,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate,
            EndpointDescriptionCollection availableEndpoints = null,
            StringCollection discoveryProfileUris = null)
        {
            return sessionInstantiator.Create(channel, configuration, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris);
        }

        /// <summary>
        /// Creates a secure channel to the specified endpoint.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="connection">The client endpoint for the reverse connect.</param>
        /// <param name="endpoint">A configured endpoint to connect to.</param>
        /// <param name="updateBeforeConnect">Update configuration based on server prior connect.</param>
        /// <param name="checkDomain">Check that the certificate specifies a valid domain (computer) name.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<ITransportChannel> CreateChannelAsync(
            ApplicationConfiguration configuration,
            ITransportWaitingConnection connection,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            CancellationToken ct = default)
        {
            endpoint.UpdateBeforeConnect = updateBeforeConnect;

            EndpointDescription endpointDescription = endpoint.Description;

            // create the endpoint configuration (use the application configuration to provide default values).
            EndpointConfiguration endpointConfiguration = endpoint.Configuration;

            if (endpointConfiguration == null)
            {
                endpoint.Configuration = endpointConfiguration = EndpointConfiguration.Create(configuration);
            }

            // create message context.
            IServiceMessageContext messageContext = configuration.CreateMessageContext(true);

            // update endpoint description using the discovery endpoint.
            if (endpoint.UpdateBeforeConnect && connection == null)
            {
                await endpoint.UpdateFromServerAsync(ct).ConfigureAwait(false);
                endpointDescription = endpoint.Description;
                endpointConfiguration = endpoint.Configuration;
            }

            // checks the domains in the certificate.
            if (checkDomain &&
                endpoint.Description.ServerCertificate != null &&
                endpoint.Description.ServerCertificate.Length > 0)
            {
                configuration.CertificateValidator?.ValidateDomains(
                    new X509Certificate2(endpoint.Description.ServerCertificate),
                    endpoint);
                checkDomain = false;
            }

            X509Certificate2 clientCertificate = null;
            X509Certificate2Collection clientCertificateChain = null;
            if (endpointDescription.SecurityPolicyUri != SecurityPolicies.None)
            {
                clientCertificate = await LoadCertificateAsync(configuration).ConfigureAwait(false);
                clientCertificateChain = await LoadCertificateChainAsync(configuration, clientCertificate).ConfigureAwait(false);
            }

            // initialize the channel which will be created with the server.
            ITransportChannel channel;
            if (connection != null)
            {
                channel = SessionChannel.CreateUaBinaryChannel(
                    configuration,
                    connection,
                    endpointDescription,
                    endpointConfiguration,
                    clientCertificate,
                    clientCertificateChain,
                    messageContext);
            }
            else
            {
                channel = SessionChannel.Create(
                     configuration,
                     endpointDescription,
                     endpointConfiguration,
                     clientCertificate,
                     clientCertificateChain,
                     messageContext);
            }

            return channel;
        }

        /// <summary>
        /// Creates a new communication session with a server using a reverse connection.
        /// </summary>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="connection">The client endpoint for the reverse connect.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="updateBeforeConnect">If set to <c>true</c> the discovery endpoint is used to update the endpoint description before connecting.</param>
        /// <param name="checkDomain">If set to <c>true</c> then the domain in the certificate must match the endpoint used.</param>
        /// <param name="sessionName">The name to assign to the session.</param>
        /// <param name="sessionTimeout">The timeout period for the session.</param>
        /// <param name="identity">The user identity to associate with the session.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The new session object.</returns>
        public static Task<Session> Create(
            ApplicationConfiguration configuration,
            ITransportWaitingConnection connection,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            return CreateAsync(DefaultSessionFactory.Instance, configuration, connection, endpoint, updateBeforeConnect, checkDomain, sessionName, sessionTimeout, identity, preferredLocales, ct);
        }

        /// <summary>
        /// Creates a new communication session with a server using a reverse connection.
        /// </summary>
        /// <param name="sessionInstantiator">The Session constructor to use to create the session.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="connection">The client endpoint for the reverse connect.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="updateBeforeConnect">If set to <c>true</c> the discovery endpoint is used to update the endpoint description before connecting.</param>
        /// <param name="checkDomain">If set to <c>true</c> then the domain in the certificate must match the endpoint used.</param>
        /// <param name="sessionName">The name to assign to the session.</param>
        /// <param name="sessionTimeout">The timeout period for the session.</param>
        /// <param name="identity">The user identity to associate with the session.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The new session object.</returns>
        public static async Task<Session> CreateAsync(
            IUaSessionInstantiator sessionInstantiator,
            ApplicationConfiguration configuration,
            ITransportWaitingConnection connection,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            // initialize the channel which will be created with the server.
            ITransportChannel channel = await Session.CreateChannelAsync(configuration, connection, endpoint, updateBeforeConnect, checkDomain, ct).ConfigureAwait(false);

            // create the session object.
            var session = sessionInstantiator.Create(channel, configuration, endpoint, null);

            // create the session.
            try
            {
                await session.OpenAsync(sessionName, sessionTimeout, identity, preferredLocales, checkDomain, ct).ConfigureAwait(false);
            }
            catch (Exception)
            {
                session.Dispose();
                throw;
            }

            return session;
        }

        /// <summary>
        /// Creates a new communication session with a server using a reverse connect manager.
        /// </summary>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="reverseConnectManager">The reverse connect manager for the client connection.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="updateBeforeConnect">If set to <c>true</c> the discovery endpoint is used to update the endpoint description before connecting.</param>
        /// <param name="checkDomain">If set to <c>true</c> then the domain in the certificate must match the endpoint used.</param>
        /// <param name="sessionName">The name to assign to the session.</param>
        /// <param name="sessionTimeout">The timeout period for the session.</param>
        /// <param name="userIdentity">The user identity to associate with the session.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The new session object.</returns>
        public static Task<Session> Create(
            ApplicationConfiguration configuration,
            ReverseConnectManager reverseConnectManager,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity userIdentity,
            IList<string> preferredLocales,
            CancellationToken ct = default
            )
        {
            return CreateAsync(DefaultSessionFactory.Instance, configuration, reverseConnectManager, endpoint, updateBeforeConnect, checkDomain, sessionName, sessionTimeout, userIdentity, preferredLocales, ct);
        }

        /// <summary>
        /// Creates a new communication session with a server using a reverse connect manager.
        /// </summary>
        /// <param name="sessionInstantiator">The Session constructor to use to create the session.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="reverseConnectManager">The reverse connect manager for the client connection.</param>
        /// <param name="endpoint">The endpoint for the server.</param>
        /// <param name="updateBeforeConnect">If set to <c>true</c> the discovery endpoint is used to update the endpoint description before connecting.</param>
        /// <param name="checkDomain">If set to <c>true</c> then the domain in the certificate must match the endpoint used.</param>
        /// <param name="sessionName">The name to assign to the session.</param>
        /// <param name="sessionTimeout">The timeout period for the session.</param>
        /// <param name="userIdentity">The user identity to associate with the session.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The new session object.</returns>
        public static async Task<Session> CreateAsync(
            IUaSessionInstantiator sessionInstantiator,
            ApplicationConfiguration configuration,
            ReverseConnectManager reverseConnectManager,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity userIdentity,
            IList<string> preferredLocales,
            CancellationToken ct = default
            )
        {
            if (reverseConnectManager == null)
            {
                return await CreateAsync(sessionInstantiator, configuration, (ITransportWaitingConnection)null, endpoint, updateBeforeConnect, checkDomain, sessionName, sessionTimeout, userIdentity, preferredLocales, ct).ConfigureAwait(false);
            }

            ITransportWaitingConnection connection;
            do
            {
                connection = await reverseConnectManager.WaitForConnectionAsync(
                    endpoint.EndpointUrl,
                    endpoint.ReverseConnect?.ServerUri,
                    ct).ConfigureAwait(false);

                if (updateBeforeConnect)
                {
                    await endpoint.UpdateFromServerAsync(
                        endpoint.EndpointUrl, connection,
                        endpoint.Description.SecurityMode,
                        endpoint.Description.SecurityPolicyUri,
                        ct).ConfigureAwait(false);
                    updateBeforeConnect = false;
                    connection = null;
                }
            } while (connection == null);

            return await CreateAsync(
                sessionInstantiator,
                configuration,
                connection,
                endpoint,
                false,
                checkDomain,
                sessionName,
                sessionTimeout,
                userIdentity,
                preferredLocales,
                ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Recreates a session based on a specified template.
        /// </summary>
        /// <param name="template">The Session object to use as template</param>
        /// <returns>The new session object.</returns>
        public static Session Recreate(Session template)
        {
            var messageContext = template.configuration_.CreateMessageContext();
            messageContext.Factory = template.Factory;

            // create the channel object used to connect to the server.
            var channel = SessionChannel.Create(
                template.configuration_,
                template.ConfiguredEndpoint.Description,
                template.ConfiguredEndpoint.Configuration,
                template.instanceCertificate_,
                template.configuration_.SecurityConfiguration.SendCertificateChain ?
                    template.instanceCertificateChain_ : null,
                messageContext);

            // create the session object.
            var session = template.CloneSession(channel, true);

            try
            {
                // open the session.
                session.Open(
                    template.SessionName,
                    (uint)template.sessionTimeout_,
                    template.Identity,
                    template.preferredLocales_,
                    template.checkDomain_);

                session.RecreateSubscriptions(template.Subscriptions);
            }
            catch (Exception e)
            {
                session.Dispose();
                throw ServiceResultException.Create(StatusCodes.BadCommunicationError, e, "Could not recreate session. {0}", template.SessionName);
            }

            return session;
        }

        /// <summary>
        /// Recreates a session based on a specified template.
        /// </summary>
        /// <param name="template">The Session object to use as template</param>
        /// <param name="connection">The waiting reverse connection.</param>
        /// <returns>The new session object.</returns>
        public static Session Recreate(Session template, ITransportWaitingConnection connection)
        {
            var messageContext = template.configuration_.CreateMessageContext();
            messageContext.Factory = template.Factory;

            // create the channel object used to connect to the server.
            var channel = SessionChannel.Create(
                template.configuration_,
                connection,
                template.ConfiguredEndpoint.Description,
                template.ConfiguredEndpoint.Configuration,
                template.instanceCertificate_,
                template.configuration_.SecurityConfiguration.SendCertificateChain ?
                    template.instanceCertificateChain_ : null,
                messageContext);

            // create the session object.
            Session session = template.CloneSession(channel, true);

            try
            {
                // open the session.
                session.Open(
                    template.SessionName,
                    (uint)template.sessionTimeout_,
                    template.Identity,
                    template.preferredLocales_,
                    template.checkDomain_);

                session.RecreateSubscriptions(template.Subscriptions);
            }
            catch (Exception e)
            {
                session.Dispose();
                throw ServiceResultException.Create(StatusCodes.BadCommunicationError, e, "Could not recreate session. {0}", template.SessionName);
            }

            return session;
        }

        /// <summary>
        /// Recreates a session based on a specified template using the provided channel.
        /// </summary>
        /// <param name="template">The Session object to use as template</param>
        /// <param name="transportChannel">The waiting reverse connection.</param>
        /// <returns>The new session object.</returns>
        public static Session Recreate(Session template, ITransportChannel transportChannel)
        {
            var messageContext = template.configuration_.CreateMessageContext();
            messageContext.Factory = template.Factory;

            // create the session object.
            Session session = template.CloneSession(transportChannel, true);

            try
            {
                // open the session.
                session.Open(
                    template.SessionName,
                    (uint)template.SessionTimeout,
                    template.Identity,
                    template.PreferredLocales,
                    template.checkDomain_);

                // create the subscriptions.
                foreach (Subscription subscription in session.Subscriptions)
                {
                    subscription.Create();
                }
            }
            catch (Exception e)
            {
                session.Dispose();
                throw ServiceResultException.Create(StatusCodes.BadCommunicationError, e, "Could not recreate session. {0}", template.SessionName);
            }

            return session;
        }
        #endregion

        #region Delegates and Events
        /// <inheritdoc/>
        public event RenewUserIdentity RenewUserIdentityEvent
        {
            add => RenewUserIdentityEventHandler += value;
            remove => RenewUserIdentityEventHandler -= value;
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public bool ApplySessionConfiguration(SessionConfiguration sessionConfiguration)
        {
            if (sessionConfiguration == null) throw new ArgumentNullException(nameof(sessionConfiguration));

            byte[] serverCertificate = ConfiguredEndpoint.Description?.ServerCertificate;
            SessionName = sessionConfiguration.SessionName;
            serverCertificate_ = serverCertificate != null ? new X509Certificate2(serverCertificate) : null;
            Identity = sessionConfiguration.Identity;
            checkDomain_ = sessionConfiguration.CheckDomain;
            serverNonce_ = sessionConfiguration.ServerNonce;
            SessionCreated(sessionConfiguration.SessionId, sessionConfiguration.AuthenticationToken);

            return true;
        }

        /// <inheritdoc/>
        public SessionConfiguration SaveSessionConfiguration(Stream stream = null)
        {
            var sessionConfiguration = new SessionConfiguration(this, serverNonce_, AuthenticationToken);
            if (stream != null)
            {
                XmlWriterSettings settings = Utils.DefaultXmlWriterSettings();
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(SessionConfiguration));
                    serializer.WriteObject(writer, sessionConfiguration);
                }
            }
            return sessionConfiguration;
        }

        /// <inheritdoc/>
        public void Reconnect()
            => Reconnect(null, null);

        /// <inheritdoc/>
        public void Reconnect(ITransportWaitingConnection connection)
            => Reconnect(connection, null);

        /// <inheritdoc/>
        public void Reconnect(ITransportChannel channel)
            => Reconnect(null, channel);

        /// <summary>
        /// Reconnects to the server after a network failure using a waiting connection.
        /// </summary>
        private void Reconnect(ITransportWaitingConnection connection, ITransportChannel transportChannel = null)
        {
            bool resetReconnect = false;
            try
            {
                reconnectLock_.Wait();
                bool reconnecting = reconnecting_;
                reconnecting_ = true;
                resetReconnect = true;
                reconnectLock_.Release();

                // check if already connecting.
                if (reconnecting)
                {
                    Utils.LogWarning("Session is already attempting to reconnect.");

                    throw ServiceResultException.Create(
                        StatusCodes.BadInvalidState,
                        "Session is already attempting to reconnect.");
                }

                StopKeepAliveTimer();

                IAsyncResult result = PrepareReconnectBeginActivate(
                            connection,
                    transportChannel);

                if (!result.AsyncWaitHandle.WaitOne(ReconnectTimeout / 2))
                {
                    Utils.LogWarning("WARNING: ACTIVATE SESSION timed out. {0}/{1}", GoodPublishRequestCount, OutstandingRequestCount);
                }

                // reactivate session.
                byte[] serverNonce = null;
                StatusCodeCollection certificateResults = null;
                DiagnosticInfoCollection certificateDiagnosticInfos = null;

                EndActivateSession(
                    result,
                    out serverNonce,
                    out certificateResults,
                    out certificateDiagnosticInfos);

                Utils.LogInfo("Session RECONNECT {0} completed successfully.", SessionId);

                lock (SyncRoot)
                {
                    previousServerNonce_ = serverNonce_;
                    serverNonce_ = serverNonce;
                }

                reconnectLock_.Wait();
                reconnecting_ = false;
                resetReconnect = false;
                reconnectLock_.Release();

                StartPublishing(OperationTimeout, true);

                StartKeepAliveTimer();

                IndicateSessionConfigurationChanged();
            }
            finally
            {
                if (resetReconnect)
                {
                    reconnectLock_.Wait();
                    reconnecting_ = false;
                    reconnectLock_.Release();
                }
            }
        }

        /// <inheritdoc/>
        public void Save(string filePath, IEnumerable<Type> knownTypes = null)
        {
            Save(filePath, Subscriptions, knownTypes);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IEnumerable<Subscription> subscriptions, IEnumerable<Type> knownTypes = null)
        {
            SubscriptionCollection subscriptionList = new SubscriptionCollection(subscriptions);
            XmlWriterSettings settings = Utils.DefaultXmlWriterSettings();

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection), knownTypes);
                serializer.WriteObject(writer, subscriptionList);
            }
        }

        /// <inheritdoc/>
        public void Save(string filePath, IEnumerable<Subscription> subscriptions, IEnumerable<Type> knownTypes = null)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Save(stream, subscriptions, knownTypes);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Subscription> Load(Stream stream, bool transferSubscriptions = false, IEnumerable<Type> knownTypes = null)
        {
            // secure settings
            XmlReaderSettings settings = Utils.DefaultXmlReaderSettings();
            settings.CloseInput = true;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(SubscriptionCollection), knownTypes);
                SubscriptionCollection subscriptions = (SubscriptionCollection)serializer.ReadObject(reader);
                foreach (Subscription subscription in subscriptions)
                {
                    if (!transferSubscriptions)
                    {
                        // ServerId must be reset if the saved list of subscriptions
                        // is not used to transfer a subscription
                        foreach (var monitoredItem in subscription.MonitoredItems)
                        {
                            monitoredItem.ServerId = 0;
                        }
                    }
                    AddSubscription(subscription);
                }
                return subscriptions;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Subscription> Load(string filePath, bool transferSubscriptions = false, IEnumerable<Type> knownTypes = null)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                return Load(stream, transferSubscriptions, knownTypes);
            }
        }

        /// <inheritdoc/>
        public void FetchNamespaceTables()
        {
            ReadValueIdCollection nodesToRead = PrepareNamespaceTableNodesToRead();

            // read from server.
            var responseHeader = base.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out var values,
                out var diagnosticInfos);

            ValidateResponse(values, nodesToRead);
            ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            UpdateNamespaceTable(values, diagnosticInfos, responseHeader);
        }

        /// <summary>
        /// Fetch the operation limits of the server.
        /// </summary>
        public void FetchOperationLimits()
        {
            try
            {
                var operationLimitsProperties = typeof(OperationLimits)
                    .GetProperties().Select(p => p.Name).ToList();

                var nodeIds = new NodeIdCollection(
                    operationLimitsProperties.Select(name => (NodeId)typeof(VariableIds)
                    .GetField("Server_ServerCapabilities_OperationLimits_" + name, BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null))
                    );

                ReadValues(nodeIds, Enumerable.Repeat(typeof(uint), nodeIds.Count).ToList(), out var values, out var errors);

                var configOperationLimits = configuration_?.ClientConfiguration?.OperationLimits ?? new OperationLimits();
                var operationLimits = new OperationLimits();

                for (int ii = 0; ii < nodeIds.Count; ii++)
                {
                    var property = typeof(OperationLimits).GetProperty(operationLimitsProperties[ii]);
                    uint value = (uint)property.GetValue(configOperationLimits);
                    if (values[ii] != null &&
                        ServiceResult.IsNotBad(errors[ii]))
                    {
                        uint serverValue = (uint)values[ii];
                        if (serverValue > 0 &&
                           (value == 0 || serverValue < value))
                        {
                            value = serverValue;
                        }
                    }
                    property.SetValue(operationLimits, value);
                }

                OperationLimits = operationLimits;
            }
            catch (Exception ex)
            {
                Utils.LogError(ex, "Failed to read operation limits from server. Using configuration defaults.");
                var operationLimits = configuration_?.ClientConfiguration?.OperationLimits;
                if (operationLimits != null)
                {
                    OperationLimits = operationLimits;
                }
            }
        }

        /// <inheritdoc/>
        public void FetchTypeTree(ExpandedNodeId typeId)
        {
            Node node = NodeCache.Find(typeId) as Node;

            if (node != null)
            {
                var subTypes = new ExpandedNodeIdCollection();
                foreach (IReference reference in node.Find(ReferenceTypeIds.HasSubtype, false))
                {
                    subTypes.Add(reference.TargetId);
                }
                if (subTypes.Count > 0)
                {
                    FetchTypeTree(subTypes);
                }
            }
        }

        /// <inheritdoc/>
        public void FetchTypeTree(ExpandedNodeIdCollection typeIds)
        {
            var referenceTypeIds = new NodeIdCollection() { ReferenceTypeIds.HasSubtype };
            IList<INode> nodes = NodeCache.FindReferences(typeIds, referenceTypeIds, false, false);
            var subTypes = new ExpandedNodeIdCollection();
            foreach (INode inode in nodes)
            {
                if (inode is Node node)
                {
                    foreach (IReference reference in node.Find(ReferenceTypeIds.HasSubtype, false))
                    {
                        if (!typeIds.Contains(reference.TargetId))
                        {
                            subTypes.Add(reference.TargetId);
                        }
                    }
                }
            }
            if (subTypes.Count > 0)
            {
                FetchTypeTree(subTypes);
            }
        }

        /// <inheritdoc/>
        public ReferenceDescriptionCollection ReadAvailableEncodings(NodeId variableId)
        {
            var variable = NodeCache.Find(variableId) as VariableNode;

            if (variable == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadNodeIdInvalid, "NodeId does not refer to a valid variable node.");
            }

            // no encodings available if there was a problem reading the data type for the node.
            if (NodeId.IsNull(variable.DataType))
            {
                return new ReferenceDescriptionCollection();
            }

            // no encodings for non-structures.
            if (!TypeTree.IsTypeOf(variable.DataType, DataTypes.Structure))
            {
                return new ReferenceDescriptionCollection();
            }

            // look for cached values.
            var encodings = NodeCache.Find(variableId, ReferenceTypeIds.HasEncoding, false, true);

            if (encodings.Count > 0)
            {
                var references = new ReferenceDescriptionCollection();

                foreach (var encoding in encodings)
                {
                    var reference = new ReferenceDescription
                    {
                        ReferenceTypeId = ReferenceTypeIds.HasEncoding,
                        IsForward = true,
                        NodeId = encoding.NodeId,
                        NodeClass = encoding.NodeClass,
                        BrowseName = encoding.BrowseName,
                        DisplayName = encoding.DisplayName,
                        TypeDefinition = encoding.TypeDefinitionId
                    };

                    references.Add(reference);
                }

                return references;
            }

            var browser = new Browser(this)
            {
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HasEncoding,
                IncludeSubtypes = false,
                NodeClassMask = 0
            };

            return browser.Browse(variable.DataType);
        }

        /// <inheritdoc/>
        public ReferenceDescription FindDataDescription(NodeId encodingId)
        {
            var browser = new Browser(this)
            {
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HasDescription,
                IncludeSubtypes = false,
                NodeClassMask = 0
            };

            var references = browser.Browse(encodingId);

            if (references.Count == 0)
            {
                throw ServiceResultException.Create(StatusCodes.BadNodeIdInvalid, "Encoding does not refer to a valid data description.");
            }

            return references[0];
        }

        /// <inheritdoc/>
        public async Task<DataDictionary> FindDataDictionaryAsync(NodeId descriptionId, CancellationToken ct = default)
        {
            // check if the dictionary has already been loaded.
            foreach (var dictionary in dictionaries_.Values)
            {
                if (dictionary.Contains(descriptionId))
                {
                    return dictionary;
                }
            }

            IList<INode> references = await NodeCache.FindReferencesAsync(descriptionId, ReferenceTypeIds.HasComponent, true, false, ct).ConfigureAwait(false);
            if (references.Count == 0)
            {
                throw ServiceResultException.Create(StatusCodes.BadNodeIdInvalid, "Description does not refer to a valid data dictionary.");
            }

            // load the dictionary.
            var dictionaryId = ExpandedNodeId.ToNodeId(references[0].NodeId, NamespaceUris);

            var dictionaryToLoad = new DataDictionary(this);

            dictionaryToLoad.Load(references[0]);

            dictionaries_[dictionaryId] = dictionaryToLoad;

            return dictionaryToLoad;
        }

        /// <inheritdoc/>
        public DataDictionary LoadDataDictionary(ReferenceDescription dictionaryNode, bool forceReload = false)
        {
            // check if the dictionary has already been loaded.
            var dictionaryId = ExpandedNodeId.ToNodeId(dictionaryNode.NodeId, NamespaceUris);
            if (!forceReload &&
                dictionaries_.TryGetValue(dictionaryId, out var dictionary))
            {
                return dictionary;
            }

            // load the dictionary.
            var dictionaryToLoad = new DataDictionary(this);
            dictionaryToLoad.Load(dictionaryId, dictionaryNode.ToString());
            dictionaries_[dictionaryId] = dictionaryToLoad;
            return dictionaryToLoad;
        }

        /// <inheritdoc/>
        public async Task<Dictionary<NodeId, DataDictionary>> LoadDataTypeSystemAsync(NodeId dataTypeSystem = null, CancellationToken ct = default)
        {
            if (dataTypeSystem == null)
            {
                dataTypeSystem = ObjectIds.OPCBinarySchema_TypeSystem;
            }
            else
            if (!Utils.IsEqual(dataTypeSystem, ObjectIds.OPCBinarySchema_TypeSystem) &&
                !Utils.IsEqual(dataTypeSystem, ObjectIds.XmlSchema_TypeSystem))
            {
                throw ServiceResultException.Create(StatusCodes.BadNodeIdInvalid, $"{nameof(dataTypeSystem)} does not refer to a valid data dictionary.");
            }

            // find the dictionary for the description.
            IList<INode> references = this.NodeCache.FindReferences(dataTypeSystem, ReferenceTypeIds.HasComponent, false, false);

            if (references.Count == 0)
            {
                throw ServiceResultException.Create(StatusCodes.BadNodeIdInvalid, "Type system does not contain a valid data dictionary.");
            }

            // batch read all encodings and namespaces
            var referenceNodeIds = references.Select(r => r.NodeId).ToList();

            // find namespace properties
            var namespaceNodes = this.NodeCache.FindReferences(referenceNodeIds, new NodeIdCollection { ReferenceTypeIds.HasProperty }, false, false)
                .Where(n => n.BrowseName == BrowseNames.NamespaceUri).ToList();
            var namespaceNodeIds = namespaceNodes.Select(n => ExpandedNodeId.ToNodeId(n.NodeId, this.NamespaceUris)).ToList();

            // read all schema definitions
            var referenceExpandedNodeIds = references
                .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, this.NamespaceUris))
                .Where(n => n.NamespaceIndex != 0).ToList();
            IDictionary<NodeId, byte[]> schemas = await DataDictionary.ReadDictionariesAsync(this, referenceExpandedNodeIds, ct).ConfigureAwait(false);

            // read namespace property values
            var namespaces = new Dictionary<NodeId, string>();
            ReadValues(namespaceNodeIds, Enumerable.Repeat(typeof(string), namespaceNodeIds.Count).ToList(), out var nameSpaceValues, out var errors);

            // build the namespace dictionary
            for (int ii = 0; ii < nameSpaceValues.Count; ii++)
            {
                if (StatusCode.IsNotBad(errors[ii].StatusCode))
                {
                    // servers may optimize space by not returning a dictionary
                    if (nameSpaceValues[ii] != null)
                    {
                        namespaces[((NodeId)referenceNodeIds[ii])] = (string)nameSpaceValues[ii];
                    }
                }
                else
                {
                    Utils.LogWarning("Failed to load namespace {0}: {1}", namespaceNodeIds[ii], errors[ii]);
                }
            }

            // build the namespace/schema import dictionary
            var imports = new Dictionary<string, byte[]>();
            foreach (var r in references)
            {
                NodeId nodeId = ExpandedNodeId.ToNodeId(r.NodeId, NamespaceUris);
                if (schemas.TryGetValue(nodeId, out var schema) && namespaces.TryGetValue(nodeId, out var ns))
                {
                    imports[ns] = schema;
                }
            }

            // read all type dictionaries in the type system
            foreach (var r in references)
            {
                DataDictionary dictionaryToLoad = null;
                NodeId dictionaryId = ExpandedNodeId.ToNodeId(r.NodeId, NamespaceUris);
                if (dictionaryId.NamespaceIndex != 0 &&
                    !dictionaries_.TryGetValue(dictionaryId, out dictionaryToLoad))
                {
                    try
                    {
                        dictionaryToLoad = new DataDictionary(this);
                        if (schemas.TryGetValue(dictionaryId, out var schema))
                        {
                            dictionaryToLoad.Load(dictionaryId, dictionaryId.ToString(), schema, imports);
                        }
                        else
                        {
                            dictionaryToLoad.Load(dictionaryId, dictionaryId.ToString());
                        }
                        dictionaries_[dictionaryId] = dictionaryToLoad;
                    }
                    catch (Exception ex)
                    {
                        Utils.LogError("Dictionary load error for Dictionary {0} : {1}", r.NodeId, ex.Message);
                    }
                }
            }

            return dictionaries_;
        }

        /// <inheritdoc/>
        public void ReadNodes(
            IList<NodeId> nodeIds,
            NodeClass nodeClass,
            out IList<Node> nodeCollection,
            out IList<ServiceResult> errors,
            bool optionalAttributes = false)
        {
            if (nodeIds.Count == 0)
            {
                nodeCollection = new NodeCollection();
                errors = new List<ServiceResult>();
                return;
            }

            if (nodeClass == NodeClass.Unspecified)
            {
                ReadNodes(nodeIds, out nodeCollection, out errors, optionalAttributes);
                return;
            }

            // determine attributes to read for nodeclass
            var attributesPerNodeId = new List<IDictionary<uint, DataValue>>(nodeIds.Count);
            var attributesToRead = new ReadValueIdCollection();
            nodeCollection = new NodeCollection(nodeIds.Count);

            CreateNodeClassAttributesReadNodesRequest(
                nodeIds, nodeClass,
                attributesToRead, attributesPerNodeId,
                nodeCollection, optionalAttributes);

            ResponseHeader responseHeader = Read(
                null,
                0,
                TimestampsToReturn.Neither,
                attributesToRead,
                out DataValueCollection values,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(values, attributesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, attributesToRead);

            errors = new ServiceResult[nodeIds.Count].ToList();
            ProcessAttributesReadNodesResponse(
                responseHeader,
                attributesToRead, attributesPerNodeId,
                values, diagnosticInfos,
                nodeCollection, errors);
        }

        /// <inheritdoc/>
        public void ReadNodes(
            IList<NodeId> nodeIds,
            out IList<Node> nodeCollection,
            out IList<ServiceResult> errors,
            bool optionalAttributes = false)
        {
            int count = nodeIds.Count;
            nodeCollection = new NodeCollection(count);
            errors = new List<ServiceResult>(count);

            if (count == 0)
            {
                return;
            }

            // first read only nodeclasses for nodes from server.
            var itemsToRead = new ReadValueIdCollection(
                nodeIds.Select(nodeId =>
                    new ReadValueId {
                        NodeId = nodeId,
                        AttributeId = Attributes.NodeClass
                    }));

            DataValueCollection nodeClassValues = null;
            DiagnosticInfoCollection diagnosticInfos = null;
            ResponseHeader responseHeader = null;

            if (count > 1)
            {
                responseHeader = Read(
                    null,
                    0,
                    TimestampsToReturn.Neither,
                    itemsToRead,
                    out nodeClassValues,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(nodeClassValues, itemsToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);
            }
            else
            {
                // for a single node read all attributes to skip the first service call
                nodeClassValues = new DataValueCollection() {
                    new DataValue(new Variant((int)NodeClass.Unspecified),
                    statusCode: StatusCodes.Good)
                    };
            }

            // second determine attributes to read per nodeclass
            var attributesPerNodeId = new List<IDictionary<uint, DataValue>>(count);
            var attributesToRead = new ReadValueIdCollection();

            CreateAttributesReadNodesRequest(
                responseHeader,
                itemsToRead, nodeClassValues, diagnosticInfos,
                attributesToRead, attributesPerNodeId,
                nodeCollection, errors,
                optionalAttributes);

            if (attributesToRead.Count > 0)
            {
                responseHeader = Read(
                    null,
                    0,
                    TimestampsToReturn.Neither,
                    attributesToRead,
                    out DataValueCollection values,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(values, attributesToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, attributesToRead);

                ProcessAttributesReadNodesResponse(
                    responseHeader,
                    attributesToRead, attributesPerNodeId,
                    values, diagnosticInfos,
                    nodeCollection, errors);
            }
        }

        /// <inheritdoc/>
        public Node ReadNode(NodeId nodeId)
        {
            return ReadNode(nodeId, NodeClass.Unspecified, true);
        }

        /// <inheritdoc/>
        public Node ReadNode(
            NodeId nodeId,
            NodeClass nodeClass,
            bool optionalAttributes = true)
        {
            // build list of attributes.
            var attributes = CreateAttributes(nodeClass, optionalAttributes);

            // build list of values to read.
            ReadValueIdCollection itemsToRead = new ReadValueIdCollection();
            foreach (uint attributeId in attributes.Keys)
            {
                ReadValueId itemToRead = new ReadValueId {
                    NodeId = nodeId,
                    AttributeId = attributeId
                };
                itemsToRead.Add(itemToRead);
            }

            // read from server.
            DataValueCollection values = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            ResponseHeader responseHeader = Read(
                null,
                0,
                TimestampsToReturn.Neither,
                itemsToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, itemsToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

            return ProcessReadResponse(responseHeader, attributes, itemsToRead, values, diagnosticInfos);
        }

        /// <inheritdoc/>
        public DataValue ReadValue(NodeId nodeId)
        {
            ReadValueId itemToRead = new ReadValueId {
                NodeId = nodeId,
                AttributeId = Attributes.Value
            };

            ReadValueIdCollection itemsToRead = new ReadValueIdCollection {
                itemToRead
            };

            // read from server.
            DataValueCollection values = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            ResponseHeader responseHeader = Read(
                null,
                0,
                TimestampsToReturn.Both,
                itemsToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, itemsToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

            if (StatusCode.IsBad(values[0].StatusCode))
            {
                ServiceResult result = ClientBase.GetResult(values[0].StatusCode, 0, diagnosticInfos, responseHeader);
                throw new ServiceResultException(result);
            }

            return values[0];
        }

        /// <inheritdoc/>
        public void ReadValues(
            IList<NodeId> nodeIds,
            out DataValueCollection values,
            out IList<ServiceResult> errors)
        {
            if (nodeIds.Count == 0)
            {
                values = new DataValueCollection();
                errors = new List<ServiceResult>();
                return;
            }

            // read all values from server.
            var itemsToRead = new ReadValueIdCollection(
                nodeIds.Select(nodeId =>
                    new ReadValueId {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value
                    }));

            // read from server.
            errors = new List<ServiceResult>(itemsToRead.Count);

            ResponseHeader responseHeader = Read(
                null,
                0,
                TimestampsToReturn.Both,
                itemsToRead,
                out values,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(values, itemsToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

            int ii = 0;
            foreach (var value in values)
            {
                ServiceResult result = ServiceResult.Good;
                if (StatusCode.IsNotGood(value.StatusCode))
                {
                    result = ClientBase.GetResult(value.StatusCode, ii, diagnosticInfos, responseHeader);
                }
                errors.Add(result);
                ii++;
            }
        }

        /// <inheritdoc/>
        public object ReadValue(NodeId nodeId, Type expectedType)
        {
            var dataValue = ReadValue(nodeId);

            var value = dataValue.Value;

            if (expectedType != null)
            {
                if (value is ExtensionObject extension)
                {
                    value = extension.Body;
                }

                if (!expectedType.IsInstanceOfType(value))
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadTypeMismatch,
                        "Server returned value unexpected type: {0}",
                        (value != null) ? value.GetType().Name : "(null)");
                }
            }

            return value;
        }

        /// <summary>
        ///     Writes the value of a node.
        /// </summary>
        /// <param name="nodeId">The node Id.</param>
        /// <param name="value">The value to write</param>
        /// <returns>Status of the write operation</returns>
        public StatusCode WriteValue(NodeId nodeId, DataValue value)
        {
            var values = new WriteValueCollection();

            var writeValue = new WriteValue { NodeId = nodeId, AttributeId = Attributes.Value, Value = value };
            values.Add(writeValue);

            // Call the Write service
            Write(null, values, out var results, out var diagnosticInfos);

            // validate response from the UA server.
            ValidateResponse(results, values);
            ValidateDiagnosticInfos(diagnosticInfos, values);

            return results[0];
        }

        /// <summary>
        ///     Writes the value of the specified nodes.
        /// </summary>
        /// <param name="nodeIds">The node Ids.</param>
        /// <param name="dataValues">The values to write</param>
        /// <returns>Status of the write operation for each of the specified node Ids.</returns>
        public List<StatusCode> WriteValues(IList<NodeId> nodeIds, IList<DataValue> dataValues)
        {
            var values = new WriteValueCollection();

            for (var ii = 0; ii < nodeIds.Count; ii++)
            {
                var writeValue = new WriteValue { NodeId = nodeIds[ii], AttributeId = Attributes.Value, Value = dataValues[ii] };
                values.Add(writeValue);
            }

            // Call the Write service
            Write(null, values, out var results, out var diagnosticInfos);

            // validate response from the UA server.
            ValidateResponse(results, values);
            ValidateDiagnosticInfos(diagnosticInfos, values);

            return results;
        }

        /// <summary>
        ///     Writes the value of the specified nodes.
        /// </summary>
        /// <param name="nodeIds">The node Ids.</param>
        /// <param name="dataValues">The values to write</param>
        /// <param name="callback">The callback to to be called as soon as the write operation is finished.</param>
        /// <param name="userData">The user data that is passed to the callback.</param>
        /// <returns>An object which must be passed to the EndWriteValues method.</returns>
        public IAsyncResult BeginWriteValues(
                                IList<NodeId> nodeIds,
                                IList<DataValue> dataValues,
                                AsyncCallback callback,
                                object userData)
        {
            var values = new WriteValueCollection();

            for (var ii = 0; ii < nodeIds.Count; ii++)
            {
                var writeValue = new WriteValue { NodeId = nodeIds[ii], AttributeId = Attributes.Value, Value = dataValues[ii] };
                values.Add(writeValue);
            }

            // Call the Write service
            return BeginWrite(null, values, callback, userData);

        }

        /// <summary>
        /// Finishes an asynchronous invocation of the Write service.
        /// </summary>
        /// <param name="result">The result returned from BeginWriteValues method.</param>
        /// <returns>Status of the write operation for each of the specified node Ids.</returns>
        public List<StatusCode> EndWriteValues(
            IAsyncResult result)
        {
            EndWrite(result, out var results, out _);

            return results;
        }

        /// <inheritdoc/>
        public ReferenceDescriptionCollection FetchReferences(NodeId nodeId)
        {
            // browse for all references.
            byte[] continuationPoint;
            ReferenceDescriptionCollection descriptions;

            Browse(
                null,
                null,
                nodeId,
                0,
                BrowseDirection.Both,
                null,
                true,
                0,
                out continuationPoint,
                out descriptions);

            // process any continuation point.
            while (continuationPoint != null)
            {
                byte[] revisedContinuationPoint;
                ReferenceDescriptionCollection additionalDescriptions;

                BrowseNext(
                    null,
                    false,
                    continuationPoint,
                    out revisedContinuationPoint,
                    out additionalDescriptions);

                continuationPoint = revisedContinuationPoint;

                descriptions.AddRange(additionalDescriptions);
            }

            return descriptions;
        }

        /// <inheritdoc/>
        public void FetchReferences(
            IList<NodeId> nodeIds,
            out IList<ReferenceDescriptionCollection> referenceDescriptions,
            out IList<ServiceResult> errors)
        {
            var result = new List<ReferenceDescriptionCollection>();

            // browse for all references.
            Browse(
                null,
                null,
                nodeIds,
                0,
                BrowseDirection.Both,
                null,
                true,
                0,
                out ByteStringCollection continuationPoints,
                out IList<ReferenceDescriptionCollection> descriptions,
                out errors);

            result.AddRange(descriptions);

            // process any continuation point.
            var previousResult = result;
            var previousErrors = errors;
            while (HasAnyContinuationPoint(continuationPoints))
            {
                var nextContinuationPoints = new ByteStringCollection();
                var nextResult = new List<ReferenceDescriptionCollection>();
                var nextErrors = new List<ServiceResult>();

                for (int ii = 0; ii < continuationPoints.Count; ii++)
                {
                    var cp = continuationPoints[ii];
                    if (cp != null)
                    {
                        nextContinuationPoints.Add(cp);
                        nextResult.Add(previousResult[ii]);
                        nextErrors.Add(previousErrors[ii]);
                    }
                }

                BrowseNext(
                    null,
                    false,
                    nextContinuationPoints,
                    out ByteStringCollection revisedContinuationPoints,
                    out descriptions,
                    out IList<ServiceResult> browseNextErrors);

                continuationPoints = revisedContinuationPoints;
                previousResult = nextResult;
                previousErrors = nextErrors;

                for (int ii = 0; ii < descriptions.Count; ii++)
                {
                    nextResult[ii].AddRange(descriptions[ii]);
                    if (StatusCode.IsBad(browseNextErrors[ii].StatusCode))
                    {
                        nextErrors[ii] = browseNextErrors[ii];
                    }
                }
            }

            referenceDescriptions = result;
        }

        /// <inheritdoc/>
        public void Open(
            string sessionName,
            IUserIdentity identity)
        {
            Open(sessionName, 0, identity, null);
        }

        /// <inheritdoc/>
        public void Open(
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales)
        {
            Open(sessionName, sessionTimeout, identity, preferredLocales, true);
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public void Open(
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            bool checkDomain)
        {
            OpenValidateIdentity(ref identity, out var identityToken, out var identityPolicy, out string securityPolicyUri, out bool requireEncryption);

            // validate the server certificate /certificate chain.
            X509Certificate2 serverCertificate = null;
            var certificateData = ConfiguredEndpoint.Description.ServerCertificate;

            if (certificateData != null && certificateData.Length > 0)
            {
                var serverCertificateChain = Utils.ParseCertificateChainBlob(certificateData);

                if (serverCertificateChain.Count > 0)
                {
                    serverCertificate = serverCertificateChain[0];
                }

                if (requireEncryption)
                {
                    if (checkDomain)
                    {
                        configuration_.CertificateValidator.Validate(serverCertificateChain, ConfiguredEndpoint);
                    }
                    else
                    {
                        configuration_.CertificateValidator.Validate(serverCertificateChain);
                    }
                    // save for reconnect
                    checkDomain_ = checkDomain;
                }
            }

            // create a nonce.
            var length = (uint)configuration_.SecurityConfiguration.NonceLength;
            var clientNonce = Utils.Nonce.CreateNonce(length);
            NodeId sessionId = null;
            NodeId sessionCookie = null;
            var serverNonce = new byte[0];
            var serverCertificateData = new byte[0];
            SignatureData serverSignature = null;
            EndpointDescriptionCollection serverEndpoints = null;
            SignedSoftwareCertificateCollection serverSoftwareCertificates = null;

            // send the application instance certificate for the client.
            BuildCertificateData(out byte[] clientCertificateData, out byte[] clientCertificateChainData);

            ApplicationDescription clientDescription = new ApplicationDescription {
                ApplicationUri = configuration_.ApplicationUri,
                ApplicationName = configuration_.ApplicationName,
                ApplicationType = ApplicationType.Client,
                ProductUri = configuration_.ProductUri
            };

            if (sessionTimeout == 0)
            {
                sessionTimeout = (uint)configuration_.ClientConfiguration.DefaultSessionTimeout;
            }

            var successCreateSession = false;
            //if security none, first try to connect without certificate
            if (ConfiguredEndpoint.Description.SecurityPolicyUri == SecurityPolicies.None)
            {
                //first try to connect with client certificate NULL
                try
                {
                    base.CreateSession(
                        null,
                        clientDescription,
                        ConfiguredEndpoint.Description.Server.ApplicationUri,
                        ConfiguredEndpoint.EndpointUrl.ToString(),
                        sessionName,
                        clientNonce,
                        null,
                        sessionTimeout,
                        (uint)MessageContext.MaxMessageSize,
                        out sessionId,
                        out sessionCookie,
                        out sessionTimeout_,
                        out serverNonce,
                        out serverCertificateData,
                        out serverEndpoints,
                        out serverSoftwareCertificates,
                        out serverSignature,
                        out maxRequestMessageSize_);

                    successCreateSession = true;
                }
                catch (Exception ex)
                {
                    Utils.LogInfo("Create session failed with client certificate NULL. " + ex.Message);
                    successCreateSession = false;
                }
            }

            if (!successCreateSession)
            {
                base.CreateSession(
                        null,
                        clientDescription,
                        ConfiguredEndpoint.Description.Server.ApplicationUri,
                        ConfiguredEndpoint.EndpointUrl.ToString(),
                        sessionName,
                        clientNonce,
                        clientCertificateChainData ?? clientCertificateData,
                        sessionTimeout,
                        (uint)MessageContext.MaxMessageSize,
                        out sessionId,
                        out sessionCookie,
                        out sessionTimeout_,
                        out serverNonce,
                        out serverCertificateData,
                        out serverEndpoints,
                        out serverSoftwareCertificates,
                        out serverSignature,
                        out maxRequestMessageSize_);
            }

            // save session id.
            lock (SyncRoot)
            {
                // save session id and cookie in base
                base.SessionCreated(sessionId, sessionCookie);
            }

            Utils.LogInfo("Revised session timeout value: {0}. ", sessionTimeout_);
            Utils.LogInfo("Max response message size value: {0}. Max request message size: {1} ",
                MessageContext.MaxMessageSize, maxRequestMessageSize_);

            //we need to call CloseSession if CreateSession was successful but some other exception is thrown
            try
            {
                // verify that the server returned the same instance certificate.
                ValidateServerCertificateData(serverCertificateData);

                ValidateServerEndpoints(serverEndpoints);

                ValidateServerSignature(serverCertificate, serverSignature, clientCertificateData, clientCertificateChainData, clientNonce);

                HandleSignedSoftwareCertificates(serverSoftwareCertificates);

                // create the client signature.
                var dataToSign = Utils.Append(serverCertificate?.RawData, serverNonce);
                var clientSignature = SecurityPolicies.Sign(instanceCertificate_, securityPolicyUri, dataToSign);

                // select the security policy for the user token.
                securityPolicyUri = identityPolicy.SecurityPolicyUri;

                if (String.IsNullOrEmpty(securityPolicyUri))
                {
                    securityPolicyUri = ConfiguredEndpoint.Description.SecurityPolicyUri;
                }

                // save previous nonce
                byte[] previousServerNonce = GetCurrentTokenServerNonce();

                // validate server nonce and security parameters for user identity.
                ValidateServerNonce(
                    identity,
                    serverNonce,
                    securityPolicyUri,
                    previousServerNonce,
                    ConfiguredEndpoint.Description.SecurityMode);

                // sign data with user token.
                var userTokenSignature = identityToken.Sign(dataToSign, securityPolicyUri);

                // encrypt token.
                identityToken.Encrypt(serverCertificate, serverNonce, securityPolicyUri);

                // send the software certificates assigned to the client.
                var clientSoftwareCertificates = GetSoftwareCertificates();

                // copy the preferred locales if provided.
                if (preferredLocales != null && preferredLocales.Count > 0)
                {
                    preferredLocales_ = new StringCollection(preferredLocales);
                }

                // activate session.
                ActivateSession(
                    null,
                    clientSignature,
                    clientSoftwareCertificates,
                    preferredLocales_,
                    new ExtensionObject(identityToken),
                    userTokenSignature,
                    out serverNonce,
                    out var certificateResults,
                    out _);

                if (certificateResults != null)
                {
                    for (var i = 0; i < certificateResults.Count; i++)
                    {
                        Utils.LogInfo("ActivateSession result[{0}] = {1}", i, certificateResults[i]);
                    }
                }

                if (certificateResults == null || certificateResults.Count == 0)
                {
                    Utils.LogInfo("Empty results were received for the ActivateSession call.");
                }

                // fetch namespaces.
                FetchNamespaceTables();

                lock (SyncRoot)
                {
                    // save nonces.
                    SessionName = sessionName;
                    Identity = identity;
                    previousServerNonce_ = previousServerNonce;
                    serverNonce_ = serverNonce;
                    serverCertificate_ = serverCertificate;

                    // update system context.
                    systemContext_.PreferredLocales = preferredLocales_;
                    systemContext_.SessionId = SessionId;
                    systemContext_.UserIdentity = identity;
                }

                // fetch operation limits
                FetchOperationLimits();

                // start keep alive thread.
                StartKeepAliveTimer();

                // raise event that session configuration chnaged.
                IndicateSessionConfigurationChanged();

                // notify session created callback, which was already set in base class only.
                SessionCreated(sessionId, sessionCookie);
            }
            catch (Exception)
            {
                try
                {
                    CloseSession(null, false);
                    CloseChannel();
                }
                catch (Exception e)
                {
                    Utils.LogError("Cleanup: CloseSession() or CloseChannel() raised exception. " + e.Message);
                }
                finally
                {
                    SessionCreated(null, null);
                }

                throw;
            }
        }

        /// <inheritdoc/>
        public void ChangePreferredLocales(StringCollection preferredLocales)
        {
            UpdateSession(Identity, preferredLocales);
        }

        /// <inheritdoc/>
        public void UpdateSession(IUserIdentity identity, StringCollection preferredLocales)
        {
            byte[] serverNonce;

            lock (SyncRoot)
            {
                // check connection state.
                if (!Connected)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidState, "Not connected to server.");
                }

                // get current nonce.
                serverNonce = serverNonce_;

                if (preferredLocales == null)
                {
                    preferredLocales = preferredLocales_;
                }
            }

            // get the identity token.
            var securityPolicyUri = ConfiguredEndpoint.Description.SecurityPolicyUri;

            // create the client signature.
            var dataToSign = Utils.Append(serverCertificate_?.RawData, serverNonce);
            var clientSignature = SecurityPolicies.Sign(instanceCertificate_, securityPolicyUri, dataToSign);

            // choose a default token.
            if (identity == null)
            {
                identity = new UserIdentity();
            }

            // check that the user identity is supported by the endpoint.
            var identityPolicy = ConfiguredEndpoint.Description.FindUserTokenPolicy(identity.TokenType, identity.IssuedTokenType);

            if (identityPolicy == null)
            {
                throw ServiceResultException.Create(
                    StatusCodes.BadUserAccessDenied,
                    "Endpoint does not support the user identity type provided.");
            }

            // select the security policy for the user token.
            securityPolicyUri = identityPolicy.SecurityPolicyUri;

            if (String.IsNullOrEmpty(securityPolicyUri))
            {
                securityPolicyUri = ConfiguredEndpoint.Description.SecurityPolicyUri;
            }

            var requireEncryption = securityPolicyUri != SecurityPolicies.None;

            // validate the server certificate before encrypting tokens.
            if (serverCertificate_ != null && requireEncryption && identity.TokenType != UserTokenType.Anonymous)
            {
                configuration_.CertificateValidator.Validate(serverCertificate_);
            }

            // validate server nonce and security parameters for user identity.
            ValidateServerNonce(
                identity,
                serverNonce,
                securityPolicyUri,
                previousServerNonce_,
                ConfiguredEndpoint.Description.SecurityMode);

            // sign data with user token.
            var identityToken = identity.GetIdentityToken();
            identityToken.PolicyId = identityPolicy.PolicyId;
            var userTokenSignature = identityToken.Sign(dataToSign, securityPolicyUri);

            // encrypt token.
            identityToken.Encrypt(serverCertificate_, serverNonce, securityPolicyUri);

            // send the software certificates assigned to the client.
            var clientSoftwareCertificates = GetSoftwareCertificates();

            // activate session.
            ActivateSession(
                null,
                clientSignature,
                clientSoftwareCertificates,
                preferredLocales,
                new ExtensionObject(identityToken),
                userTokenSignature,
                out serverNonce,
                out _,
                out _);

            // save nonce and new values.
            lock (SyncRoot)
            {
                if (identity != null)
                {
                    Identity = identity;
                }

                previousServerNonce_ = serverNonce_;
                serverNonce_ = serverNonce;
                preferredLocales_ = preferredLocales;

                // update system context.
                systemContext_.PreferredLocales = preferredLocales_;
                systemContext_.SessionId = SessionId;
                systemContext_.UserIdentity = identity;
            }

            IndicateSessionConfigurationChanged();
        }

        /// <inheritdoc/>
        public void FindComponentIds(
            NodeId instanceId,
            IList<string> componentPaths,
            out NodeIdCollection componentIds,
            out List<ServiceResult> errors)
        {
            componentIds = new NodeIdCollection();
            errors = new List<ServiceResult>();

            // build list of paths to translate.
            var pathsToTranslate = new BrowsePathCollection();

            foreach (var componentPath in componentPaths)
            {
                var pathToTranslate = new BrowsePath
                {
                    StartingNode = instanceId,
                    RelativePath = RelativePath.Parse(componentPath, TypeTree)
                };

                pathsToTranslate.Add(pathToTranslate);
            }

            // translate the paths.

            var responseHeader = TranslateBrowsePathsToNodeIds(
                null,
                pathsToTranslate,
                out var results,
                out var diagnosticInfos);

            // verify that the server returned the correct number of results.
            ClientBase.ValidateResponse(results, pathsToTranslate);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, pathsToTranslate);

            for (var ii = 0; ii < componentPaths.Count; ii++)
            {
                componentIds.Add(NodeId.Null);
                errors.Add(ServiceResult.Good);

                // process any diagnostics associated with any error.
                if (StatusCode.IsBad(results[ii].StatusCode))
                {
                    errors[ii] = new ServiceResult(results[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                    continue;
                }

                // Expecting exact one NodeId for a local node.
                // Report an error if the server returns anything other than that.

                if (results[ii].Targets.Count == 0)
                {
                    errors[ii] = ServiceResult.Create(
                        StatusCodes.BadTargetNodeIdInvalid,
                        "Could not find target for path: {0}.",
                        componentPaths[ii]);

                    continue;
                }

                if (results[ii].Targets.Count != 1)
                {
                    errors[ii] = ServiceResult.Create(
                        StatusCodes.BadTooManyMatches,
                        "Too many matches found for path: {0}.",
                        componentPaths[ii]);

                    continue;
                }

                if (results[ii].Targets[0].RemainingPathIndex != UInt32.MaxValue)
                {
                    errors[ii] = ServiceResult.Create(
                        StatusCodes.BadTargetNodeIdInvalid,
                        "Cannot follow path to external server: {0}.",
                        componentPaths[ii]);

                    continue;
                }

                if (NodeId.IsNull(results[ii].Targets[0].TargetId))
                {
                    errors[ii] = ServiceResult.Create(
                        StatusCodes.BadUnexpectedError,
                        "Server returned a null NodeId for path: {0}.",
                        componentPaths[ii]);

                    continue;
                }

                if (results[ii].Targets[0].TargetId.IsAbsolute)
                {
                    errors[ii] = ServiceResult.Create(
                        StatusCodes.BadUnexpectedError,
                        "Server returned a remote node for path: {0}.",
                        componentPaths[ii]);

                    continue;
                }

                // suitable target found.
                componentIds[ii] = ExpandedNodeId.ToNodeId(results[ii].Targets[0].TargetId, NamespaceUris);
            }
        }

        /// <inheritdoc/>
        public void ReadValues(
            IList<NodeId> variableIds,
            IList<Type> expectedTypes,
            out List<object> values,
            out List<ServiceResult> errors)
        {
            values = new List<object>();
            errors = new List<ServiceResult>();

            // build list of values to read.
            var valuesToRead = new ReadValueIdCollection();

            foreach (var variableId in variableIds)
            {
                var valueToRead = new ReadValueId
                {
                    NodeId = variableId, AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null
                };

                valuesToRead.Add(valueToRead);
            }

            // read the values.
            var responseHeader = Read(
                null,
                0,
                TimestampsToReturn.Both,
                valuesToRead,
                out var results,
                out var diagnosticInfos);

            // verify that the server returned the correct number of results.
            ClientBase.ValidateResponse(results, valuesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToRead);

            for (var ii = 0; ii < variableIds.Count; ii++)
            {
                values.Add(null);
                errors.Add(ServiceResult.Good);

                // process any diagnostics associated with bad or uncertain data.
                if (StatusCode.IsNotGood(results[ii].StatusCode))
                {
                    errors[ii] = new ServiceResult(results[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                    if (StatusCode.IsBad(results[ii].StatusCode))
                    {
                        continue;
                    }
                }

                var value = results[ii].Value;

                // extract the body from extension objects.
                if (value is ExtensionObject extension && extension.Body is IEncodeable)
                {
                    value = extension.Body;
                }

                // check expected type.
                if (expectedTypes[ii] != null && !expectedTypes[ii].IsInstanceOfType(value))
                {
                    errors[ii] = ServiceResult.Create(
                        StatusCodes.BadTypeMismatch,
                        "Value {0} does not have expected type: {1}.",
                        value,
                        expectedTypes[ii].Name);

                    continue;
                }

                // suitable value found.
                values[ii] = value;
            }
        }

        /// <summary>
        /// Reads the values for a set of variables.
        /// </summary>
        /// <param name="variableIds">The variable ids.</param>
        /// <param name="maxAge">The maximum age for any value.</param>
        /// <param name="timestampsToReturn">OPC UA defines two timestamps, the source and the server timestamp. </param>
        /// <param name="callback">The callback to to be called as soon as the read operation is finished.</param>
        /// <param name="userData">The user data that is passed to the callback.</param>
        /// <returns>An object which must be passed to the EndWriteValues method.</returns>
        public IAsyncResult BeginReadValues(
            IList<NodeId> variableIds,
            double maxAge,
            TimestampsToReturn timestampsToReturn,
            AsyncCallback callback,
            object userData)
        {
            var values = new ReadValueIdCollection();

            foreach (var variableId in variableIds)
            {
                var valueToRead = new ReadValueId
                {
                    NodeId = variableId, AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null
                };

                values.Add(valueToRead);
            }

            // Call the Write service
            return BeginRead(null, maxAge, timestampsToReturn, values, callback, userData);

        }

        /// <summary>
        /// Finishes an asynchronous invocation of the Read service.
        /// </summary>
        /// <param name="result">The result returned from BeginReadValues method.</param>
        /// <returns>Status of the read operation for each of the specified node Ids.</returns>
        public List<DataValue> EndReadValues(
            IAsyncResult result)
        {
            EndRead(result, out var results, out _);

            return results;
        }

        /// <inheritdoc/>
        public void ReadDisplayName(
            IList<NodeId> nodeIds,
            out IList<string> displayNames,
            out IList<ServiceResult> errors)
        {
            displayNames = new List<string>();
            errors = new List<ServiceResult>();

            // build list of values to read.
            var valuesToRead = new ReadValueIdCollection();

            foreach (var nodeId in nodeIds)
            {
                var valueToRead = new ReadValueId
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.DisplayName,
                    IndexRange = null,
                    DataEncoding = null
                };

                valuesToRead.Add(valueToRead);
            }

            // read the values.
            var responseHeader = Read(
                null,
                Int32.MaxValue,
                TimestampsToReturn.Both,
                valuesToRead,
                out var results,
                out var diagnosticInfos);

            // verify that the server returned the correct number of results.
            ClientBase.ValidateResponse(results, valuesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToRead);

            for (var ii = 0; ii < nodeIds.Count; ii++)
            {
                displayNames.Add(String.Empty);
                errors.Add(ServiceResult.Good);

                // process any diagnostics associated with bad or uncertain data.
                if (StatusCode.IsNotGood(results[ii].StatusCode))
                {
                    errors[ii] = new ServiceResult(results[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                    continue;
                }

                // extract the name.
                var displayName = results[ii].GetValue<LocalizedText>(null);

                if (!LocalizedText.IsNullOrEmpty(displayName))
                {
                    displayNames[ii] = displayName.Text;
                }
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            if (obj is IUaSession session)
            {
                if (!ConfiguredEndpoint.Equals(session.Endpoint)) return false;
                if (!SessionName.Equals(session.SessionName, StringComparison.Ordinal)) return false;
                if (!SessionId.Equals(session.SessionId)) return false;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(ConfiguredEndpoint, SessionName, SessionId);
        }

        /// <summary>
        /// An overrideable version of a session clone which is used
        /// internally to create new subclassed clones from a Session class.
        /// </summary>
        public virtual Session CloneSession(ITransportChannel channel, bool copyEventHandlers)
        {
            return new Session(channel, this, copyEventHandlers);
        }
        #endregion

        #region Close Methods
        /// <inheritdoc/>
        public override StatusCode Close()
        {
            return Close(keepAliveInterval_, true);
        }

        /// <inheritdoc/>
        public StatusCode Close(bool closeChannel)
        {
            return Close(keepAliveInterval_, closeChannel);
        }

        /// <inheritdoc/>
        public StatusCode Close(int timeout)
            => Close(timeout, true);

        /// <inheritdoc/>
        public virtual StatusCode Close(int timeout, bool closeChannel)
        {
            // check if already called.
            if (Disposed)
            {
                return StatusCodes.Good;
            }

            StatusCode result = StatusCodes.Good;

            // stop the keep alive timer.
            StopKeepAliveTimer();

            // check if currently connected.
            var connected = Connected;

            // halt all background threads.
            if (connected)
            {
                if (SessionClosingEventHandler != null)
                {
                    try
                    {
                        SessionClosingEventHandler(this, null);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError(e, "Session: Unexpected eror raising SessionClosing event.");
                    }
                }

                // close the session with the server.
                if (!KeepAliveStopped)
                {
                    try
                    {
                        // close the session and delete all subscriptions if specified.
                        var requestHeader = new RequestHeader() {
                            TimeoutHint = timeout > 0 ? (uint)timeout : (uint)(this.OperationTimeout > 0 ? this.OperationTimeout : 0),
                        };
                        CloseSession(requestHeader, DeleteSubscriptionsOnClose);

                        if (closeChannel)
                        {
                            CloseChannel();
                        }

                        // raised notification indicating the session is closed.
                        SessionCreated(null, null);
                    }
                    // dont throw errors on disconnect, but return them
                    // so the caller can log the error.
                    catch (ServiceResultException sre)
                    {
                        result = sre.StatusCode;
                    }
                    catch (Exception)
                    {
                        result = StatusCodes.Bad;
                    }
                }
            }

            // clean up.
            if (closeChannel)
            {
                Dispose();
            }

            return result;
        }
        #endregion

        #region Subscription Methods
        /// <inheritdoc/>
        public bool AddSubscription(Subscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            lock (SyncRoot)
            {
                if (subscriptions_.Contains(subscription))
                {
                    return false;
                }

                subscription.Session = this;
                subscriptions_.Add(subscription);
            }

            SubscriptionsChangedEventHandler?.Invoke(this, null);

            return true;
        }

        /// <inheritdoc/>
        public bool RemoveSubscription(Subscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            if (subscription.Created)
            {
                subscription.Delete(false);
            }

            lock (SyncRoot)
            {
                if (!subscriptions_.Remove(subscription))
                {
                    return false;
                }

                subscription.Session = null;
            }

            SubscriptionsChangedEventHandler?.Invoke(this, null);

            return true;
        }

        /// <inheritdoc/>
        public bool RemoveSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            if (subscriptions == null) throw new ArgumentNullException(nameof(subscriptions));

            var subscriptionsToDelete = new List<Subscription>();
            var removed = PrepareSubscriptionsToDelete(subscriptions, subscriptionsToDelete);

            foreach (var subscription in subscriptionsToDelete)
            {
                subscription.Delete(true);
            }

            if (removed)
            {
                SubscriptionsChangedEventHandler?.Invoke(this, null);
            }

            return removed;
        }

        /// <inheritdoc/>
        public bool RemoveTransferredSubscription(Subscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            if (subscription.Session != this)
            {
                return false;
            }

            lock (SyncRoot)
            {
                if (!subscriptions_.Remove(subscription))
                {
                    return false;
                }

                subscription.Session = null;
            }

            SubscriptionsChangedEventHandler?.Invoke(this, null);

            return true;
        }

        /// <inheritdoc/>
        public bool ReactivateSubscriptions(
            SubscriptionCollection subscriptions,
            bool sendInitialValues)
        {
            int failedSubscriptions = 0;
            UInt32Collection subscriptionIds = CreateSubscriptionIdsForTransfer(subscriptions);

            if (subscriptionIds.Count > 0)
            {
                try
                {
                    reconnectLock_.Wait();
                    reconnecting_ = true;

                    for (int ii = 0; ii < subscriptions.Count; ii++)
                    {
                        if (!subscriptions[ii].Transfer(this, subscriptionIds[ii], new UInt32Collection()))
                        {
                            Utils.LogError("SubscriptionId {0} failed to reactivate.", subscriptionIds[ii]);
                            failedSubscriptions++;
                        }
                    }

                    if (sendInitialValues)
                    {
                        if (!ResendData(subscriptions, out IList<ServiceResult> resendResults))
                        {
                            Utils.LogError("Failed to call resend data for subscriptions.");
                        }
                        else if (resendResults != null)
                        {
                            for (int ii = 0; ii < resendResults.Count; ii++)
                            {
                                // no need to try for subscriptions which do not exist
                                if (StatusCode.IsNotGood(resendResults[ii].StatusCode))
                                {
                                    Utils.LogError("SubscriptionId {0} failed to resend data.", subscriptionIds[ii]);
                                }
                            }
                        }
                    }

                    Utils.LogInfo("Session REACTIVATE of {0} subscriptions completed. {1} failed.", subscriptions.Count, failedSubscriptions);
                }
                finally
                {
                    reconnecting_ = false;
                    reconnectLock_.Release();
                }

                StartPublishing(OperationTimeout, false);
            }
            else
            {
                Utils.LogInfo("No subscriptions. Transfersubscription skipped.");
            }

            return failedSubscriptions == 0;
        }

        /// <inheritdoc/>
        public bool TransferSubscriptions(
            SubscriptionCollection subscriptions,
            bool sendInitialValues)
        {
            int failedSubscriptions = 0;
            UInt32Collection subscriptionIds = CreateSubscriptionIdsForTransfer(subscriptions);

            if (subscriptionIds.Count > 0)
            {
                if (reconnecting_)
                {
                    Utils.LogWarning("Already Reconnecting. Can not transfer subscriptions.");
                    return false;
                }

                try
                {
                    reconnectLock_.Wait();
                    reconnecting_ = true;

                    ResponseHeader responseHeader = base.TransferSubscriptions(null, subscriptionIds, sendInitialValues,
                        out TransferResultCollection results, out DiagnosticInfoCollection diagnosticInfos);
                    if (!StatusCode.IsGood(responseHeader.ServiceResult))
                    {
                        Utils.LogError("TransferSubscription failed: {0}", responseHeader.ServiceResult);
                        return false;
                    }
                    ClientBase.ValidateResponse(results, subscriptionIds);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, subscriptionIds);

                    for (int ii = 0; ii < subscriptions.Count; ii++)
                    {
                        if (StatusCode.IsGood(results[ii].StatusCode))
                        {
                            if (subscriptions[ii].Transfer(this, subscriptionIds[ii], results[ii].AvailableSequenceNumbers))
                            {
                                lock (acknowledgementsToSendLock_)
                                {
                                    // create ack for available sequence numbers
                                    foreach (var sequenceNumber in results[ii].AvailableSequenceNumbers)
                                    {
                                        AddAcknowledgementToSend(acknowledgementsToSend_, subscriptionIds[ii], sequenceNumber);
                                    }
                                }
                            }
                        }
                        else if (results[ii].StatusCode == StatusCodes.BadNothingToDo)
                        {
                            Utils.LogInfo("SubscriptionId {0} is already member of the session.", subscriptionIds[ii]);
                            failedSubscriptions++;
                        }
                        else
                        {
                            Utils.LogError("SubscriptionId {0} failed to transfer, StatusCode={1}", subscriptionIds[ii], results[ii].StatusCode);
                            failedSubscriptions++;
                        }
                    }

                    Utils.LogInfo("Session TRANSFER of {0} subscriptions completed. {1} failed.", subscriptions.Count, failedSubscriptions);
                }
                finally
                {
                    reconnecting_ = false;
                    reconnectLock_.Release();
                }

                StartPublishing(OperationTimeout, false);
            }
            else
            {
                Utils.LogInfo("No subscriptions. Transfersubscription skipped.");
            }

            return failedSubscriptions == 0;
        }
        #endregion

        #region Browse Methods
        /// <inheritdoc/>
        public virtual ResponseHeader Browse(
            RequestHeader requestHeader,
            ViewDescription view,
            NodeId nodeToBrowse,
            uint maxResultsToReturn,
            BrowseDirection browseDirection,
            NodeId referenceTypeId,
            bool includeSubtypes,
            uint nodeClassMask,
            out byte[] continuationPoint,
            out ReferenceDescriptionCollection references)
        {
            BrowseDescription description = new BrowseDescription();

            description.NodeId = nodeToBrowse;
            description.BrowseDirection = browseDirection;
            description.ReferenceTypeId = referenceTypeId;
            description.IncludeSubtypes = includeSubtypes;
            description.NodeClassMask = nodeClassMask;
            description.ResultMask = (uint)BrowseResultMask.All;

            BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
            nodesToBrowse.Add(description);

            BrowseResultCollection results;
            DiagnosticInfoCollection diagnosticInfos;

            ResponseHeader responseHeader = Browse(
                requestHeader,
                view,
                maxResultsToReturn,
                nodesToBrowse,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToBrowse);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable));
            }

            continuationPoint = results[0].ContinuationPoint;
            references = results[0].References;

            return responseHeader;
        }

        /// <inheritdoc/>
        public virtual ResponseHeader Browse(
            RequestHeader requestHeader,
            ViewDescription view,
            IList<NodeId> nodesToBrowse,
            uint maxResultsToReturn,
            BrowseDirection browseDirection,
            NodeId referenceTypeId,
            bool includeSubtypes,
            uint nodeClassMask,
            out ByteStringCollection continuationPoints,
            out IList<ReferenceDescriptionCollection> referencesList,
            out IList<ServiceResult> errors)
        {

            BrowseDescriptionCollection browseDescription = new BrowseDescriptionCollection();
            foreach (var nodeToBrowse in nodesToBrowse)
            {
                BrowseDescription description = new BrowseDescription {
                    NodeId = nodeToBrowse,
                    BrowseDirection = browseDirection,
                    ReferenceTypeId = referenceTypeId,
                    IncludeSubtypes = includeSubtypes,
                    NodeClassMask = nodeClassMask,
                    ResultMask = (uint)BrowseResultMask.All
                };

                browseDescription.Add(description);
            }

            ResponseHeader responseHeader = Browse(
                requestHeader,
                view,
                maxResultsToReturn,
                browseDescription,
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, browseDescription);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, browseDescription);

            int ii = 0;
            errors = new List<ServiceResult>();
            continuationPoints = new ByteStringCollection();
            referencesList = new List<ReferenceDescriptionCollection>();
            foreach (var result in results)
            {
                if (StatusCode.IsBad(result.StatusCode))
                {
                    errors.Add(new ServiceResult(result.StatusCode, ii, diagnosticInfos, responseHeader.StringTable));
                }
                else
                {
                    errors.Add(ServiceResult.Good);
                }
                continuationPoints.Add(result.ContinuationPoint);
                referencesList.Add(result.References);
                ii++;
            }

            return responseHeader;
        }

        /// <inheritdoc/>
        public IAsyncResult BeginBrowse(
            RequestHeader requestHeader,
            ViewDescription view,
            NodeId nodeToBrowse,
            uint maxResultsToReturn,
            BrowseDirection browseDirection,
            NodeId referenceTypeId,
            bool includeSubtypes,
            uint nodeClassMask,
            AsyncCallback callback,
            object asyncState)
        {
            var description = new BrowseDescription
            {
                NodeId = nodeToBrowse,
                BrowseDirection = browseDirection,
                ReferenceTypeId = referenceTypeId,
                IncludeSubtypes = includeSubtypes,
                NodeClassMask = nodeClassMask,
                ResultMask = (uint)BrowseResultMask.All
            };

            var nodesToBrowse = new BrowseDescriptionCollection {description};

            return BeginBrowse(
                requestHeader,
                view,
                maxResultsToReturn,
                nodesToBrowse,
                callback,
                asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndBrowse(
            IAsyncResult result,
            out byte[] continuationPoint,
            out ReferenceDescriptionCollection references)
        {
            var responseHeader = EndBrowse(
                result,
                out BrowseResultCollection results,
                out var diagnosticInfos);

            if (results == null || results.Count != 1)
            {
                throw new ServiceResultException(StatusCodes.BadUnknownResponse);
            }

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable));
            }

            continuationPoint = results[0].ContinuationPoint;
            references = results[0].References;

            return responseHeader;
        }
        #endregion

        #region BrowseNext Methods
        /// <inheritdoc/>
        public virtual ResponseHeader BrowseNext(
            RequestHeader requestHeader,
            bool releaseContinuationPoint,
            byte[] continuationPoint,
            out byte[] revisedContinuationPoint,
            out ReferenceDescriptionCollection references)
        {
            var continuationPoints = new ByteStringCollection {continuationPoint};

            var responseHeader = BrowseNext(
                requestHeader,
                releaseContinuationPoint,
                continuationPoints,
                out var results,
                out var diagnosticInfos);

            ClientBase.ValidateResponse(results, continuationPoints);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable));
            }

            revisedContinuationPoint = results[0].ContinuationPoint;
            references = results[0].References;

            return responseHeader;
        }

        /// <inheritdoc/>
        public virtual ResponseHeader BrowseNext(
            RequestHeader requestHeader,
            bool releaseContinuationPoint,
            ByteStringCollection continuationPoints,
            out ByteStringCollection revisedContinuationPoints,
            out IList<ReferenceDescriptionCollection> referencesList,
            out IList<ServiceResult> errors)
        {
            BrowseResultCollection results;
            DiagnosticInfoCollection diagnosticInfos;

            ResponseHeader responseHeader = BrowseNext(
                requestHeader,
                releaseContinuationPoint,
                continuationPoints,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, continuationPoints);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);

            int ii = 0;
            errors = new List<ServiceResult>();
            revisedContinuationPoints = new ByteStringCollection();
            referencesList = new List<ReferenceDescriptionCollection>();
            foreach (var result in results)
            {
                if (StatusCode.IsBad(result.StatusCode))
                {
                    errors.Add(new ServiceResult(result.StatusCode, ii, diagnosticInfos, responseHeader.StringTable));
                }
                else
                {
                    errors.Add(ServiceResult.Good);
                }
                revisedContinuationPoints.Add(result.ContinuationPoint);
                referencesList.Add(result.References);
                ii++;
            }

            return responseHeader;
        }

        /// <inheritdoc/>
        public IAsyncResult BeginBrowseNext(
            RequestHeader requestHeader,
            bool releaseContinuationPoint,
            byte[] continuationPoint,
            AsyncCallback callback,
            object asyncState)
        {
            ByteStringCollection continuationPoints = new ByteStringCollection();
            continuationPoints.Add(continuationPoint);

            return BeginBrowseNext(
                requestHeader,
                releaseContinuationPoint,
                continuationPoints,
                callback,
                asyncState);
        }

        /// <inheritdoc/>
        public ResponseHeader EndBrowseNext(
            IAsyncResult result,
            out byte[] revisedContinuationPoint,
            out ReferenceDescriptionCollection references)
        {
            var responseHeader = EndBrowseNext(
                result,
                out BrowseResultCollection results,
                out var diagnosticInfos);

            if (results == null || results.Count != 1)
            {
                throw new ServiceResultException(StatusCodes.BadUnknownResponse);
            }

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable));
            }

            revisedContinuationPoint = results[0].ContinuationPoint;
            references = results[0].References;

            return responseHeader;
        }
        #endregion

        #region Call Methods
        /// <inheritdoc/>
        public IList<object> Call(NodeId objectId, NodeId methodId, params object[] args)
        {
            var inputArguments = new VariantCollection();

            if (args != null)
            {
                foreach (var arg in args)
                {
                    inputArguments.Add(new Variant(arg));
                }
            }

            var request = new CallMethodRequest
            {
                ObjectId = objectId, MethodId = methodId,
                InputArguments = inputArguments
            };

            var requests = new CallMethodRequestCollection {request};

            var responseHeader = Call(
                null,
                requests,
                out var results,
                out var diagnosticInfos);

            ClientBase.ValidateResponse(results, requests);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, requests);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw ServiceResultException.Create(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable);
            }

            var outputArguments = new List<object>();

            foreach (var arg in results[0].OutputArguments)
            {
                outputArguments.Add(arg.Value);
            }

            return outputArguments;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Returns the software certificates assigned to the application.
        /// </summary>
        protected virtual SignedSoftwareCertificateCollection GetSoftwareCertificates()
        {
            return new SignedSoftwareCertificateCollection();
        }

        /// <summary>
        /// Handles an error when validating the application instance certificate provided by the server.
        /// </summary>
        protected virtual void OnApplicationCertificateError(byte[] serverCertificate, ServiceResult result)
        {
            throw new ServiceResultException(result);
        }

        /// <summary>
        /// Handles an error when validating software certificates provided by the server.
        /// </summary>
        protected virtual void OnSoftwareCertificateError(SignedSoftwareCertificate signedCertificate, ServiceResult result)
        {
            throw new ServiceResultException(result);
        }

        /// <summary>
        /// Inspects the software certificates provided by the server.
        /// </summary>
        protected virtual void ValidateSoftwareCertificates(List<SoftwareCertificate> softwareCertificates)
        {
            // always accept valid certificates.
        }

        /// <summary>
        /// Starts a timer to check that the connection to the server is still available.
        /// </summary>
        private void StartKeepAliveTimer()
        {
            var keepAliveInterval = keepAliveInterval_;

            Interlocked.Exchange(ref lastKeepAliveTime_, DateTime.UtcNow.Ticks);

            serverState_ = ServerState.Unknown;

            var nodesToRead = new ReadValueIdCollection() {
                // read the server state.
                new ReadValueId {
                    NodeId = Variables.Server_ServerStatus_State,
                    AttributeId = Attributes.Value,
                    DataEncoding = null,
                    IndexRange = null
                }
            };

            // restart the publish timer.
            lock (SyncRoot)
            {
                StopKeepAliveTimer();

#if NET6_0_OR_GREATER
                // start periodic timer loop
                var keepAliveTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(keepAliveInterval));
                _ = Task.Run(() => OnKeepAliveAsync(keepAliveTimer, nodesToRead));
                keepAliveTimer_ = keepAliveTimer;
            }
#else
                // start timer
                keepAliveTimer_ = new Timer(OnKeepAlive, nodesToRead, keepAliveInterval, keepAliveInterval);
            }

            // send initial keep alive.
            OnKeepAlive(nodesToRead);
#endif
        }

        /// <summary>
        /// Stops the keep alive timer.
        /// </summary>
        private void StopKeepAliveTimer()
        {
            Utils.SilentDispose(keepAliveTimer_);
            keepAliveTimer_ = null;
        }

        /// <summary>
        /// Removes a completed async request.
        /// </summary>
        private AsyncRequestState RemoveRequest(IAsyncResult result, uint requestId, uint typeId)
        {
            lock (outstandingRequests_)
            {
                for (var ii = outstandingRequests_.First; ii != null; ii = ii.Next)
                {
                    if (Object.ReferenceEquals(result, ii.Value.Result) || (requestId == ii.Value.RequestId && typeId == ii.Value.RequestTypeId))
                    {
                        var state = ii.Value;
                        outstandingRequests_.Remove(ii);
                        return state;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Adds a new async request.
        /// </summary>
        private void AsyncRequestStarted(IAsyncResult result, uint requestId, uint typeId)
        {
            lock (outstandingRequests_)
            {
                // check if the request completed asynchronously.
                var state = RemoveRequest(result, requestId, typeId);

                // add a new request.
                if (state == null)
                {
                    state = new AsyncRequestState
                    {
                        Defunct = false,
                        RequestId = requestId,
                        RequestTypeId = typeId,
                        Result = result,
                        Timestamp = DateTime.UtcNow
                    };

                    outstandingRequests_.AddLast(state);
                }
            }
        }

        /// <summary>
        /// Removes a completed async request.
        /// </summary>
        private void AsyncRequestCompleted(IAsyncResult result, uint requestId, uint typeId)
        {
            lock (outstandingRequests_)
            {
                // remove the request.
                var state = RemoveRequest(result, requestId, typeId);

                if (state != null)
                {
                    // mark any old requests as default (i.e. the should have returned before this request).
                    var maxAge = state.Timestamp.AddSeconds(-1);

                    for (var ii = outstandingRequests_.First; ii != null; ii = ii.Next)
                    {
                        if (ii.Value.RequestTypeId == typeId && ii.Value.Timestamp < maxAge)
                        {
                            ii.Value.Defunct = true;
                        }
                    }
                }

                // add a dummy placeholder since the begin request has not completed yet.
                if (state == null)
                {
                    state = new AsyncRequestState
                    {
                        Defunct = true,
                        RequestId = requestId,
                        RequestTypeId = typeId,
                        Result = result,
                        Timestamp = DateTime.UtcNow
                    };

                    outstandingRequests_.AddLast(state);
                }
            }
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Sends a keep alive by reading from the server.
        /// </summary>
        private async Task OnKeepAliveAsync(PeriodicTimer keepAliveTimer, ReadValueIdCollection nodesToRead)
        {
            // trigger first keep alive
            OnSendKeepAlive(nodesToRead);

            while (await keepAliveTimer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                OnSendKeepAlive(nodesToRead);
        }

            Utils.LogTrace("Session {0}: KeepAlive PeriodicTimer exit.", SessionId);
        }
#else
        /// <summary>
        /// Sends a keep alive by reading from the server.
        /// </summary>
        private void OnKeepAlive(object state)
        {
            ReadValueIdCollection nodesToRead = (ReadValueIdCollection)state;
            OnSendKeepAlive(nodesToRead);
        }
#endif

        /// <summary>
        /// Sends a keep alive by reading from the server.
        /// </summary>
        private void OnSendKeepAlive(ReadValueIdCollection nodesToRead)
        {
            try
            {
                // check if session has been closed.
                if (!Connected || keepAliveTimer_ == null)
                {
                    return;
                }

                // check if session has been closed.
                if (reconnecting_)
                {
                    Utils.LogWarning("Session {0}: KeepAlive ignored while reconnecting.", SessionId);
                    return;
                }

                // raise error if keep alives are not coming back.
                if (KeepAliveStopped)
                {
                    if (!OnKeepAliveError(ServiceResult.Create(StatusCodes.BadNoCommunication, "Server not responding to keep alive requests.")))
                    {
                        return;
                    }
                }

                var requestHeader = new RequestHeader {
                    RequestHandle = Utils.IncrementIdentifier(ref keepAliveCounter_),
                    TimeoutHint = (uint)(KeepAliveInterval * 2),
                    ReturnDiagnostics = 0
                };

                var result = BeginRead(
                    requestHeader,
                    0,
                    TimestampsToReturn.Neither,
                    nodesToRead,
                    OnKeepAliveComplete,
                    nodesToRead);

                AsyncRequestStarted(result, requestHeader.RequestHandle, DataTypes.ReadRequest);
            }
            catch (Exception e)
            {
                Utils.LogError("Could not send keep alive request: {0} {1}", e.GetType().FullName, e.Message);
            }
        }

        /// <summary>
        /// Checks if a notification has arrived. Sends a publish if it has not.
        /// </summary>
        private void OnKeepAliveComplete(IAsyncResult result)
        {
            var nodesToRead = (ReadValueIdCollection)result.AsyncState;

            AsyncRequestCompleted(result, 0, DataTypes.ReadRequest);

            try
            {
                // read the server status.
                var responseHeader = EndRead(
                    result,
                    out var values,
                    out var diagnosticInfos);

                ValidateResponse(values, nodesToRead);
                ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                // validate value returned.
                var error = ValidateDataValue(values[0], typeof(int), 0, diagnosticInfos, responseHeader);

                if (ServiceResult.IsBad(error))
                {
                    throw new ServiceResultException(error);
                }

                // send notification that keep alive completed.
                OnKeepAlive((ServerState)(int)values[0].Value, responseHeader.Timestamp);

                return;
            }
            catch (ServiceResultException sre) when (sre.StatusCode == StatusCodes.BadSessionIdInvalid)
            {
                // recover from error condition when secure channel is still alive
                OnKeepAliveError(ServiceResult.Create(StatusCodes.BadSessionIdInvalid, "Session unavailable for keep alive requests."));
            }
            catch (Exception e)
            {
                Utils.LogError("Unexpected keep alive error occurred: {0}", e.Message);
            }
        }

        /// <summary>
        /// Called when the server returns a keep alive response.
        /// </summary>
        protected virtual void OnKeepAlive(ServerState currentState, DateTime currentTime)
        {
            // restart publishing if keep alives recovered.
            if (KeepAliveStopped)
            {
                // ignore if already reconnecting.
                if (reconnecting_)
                {
                    return;
                }

                Interlocked.Exchange(ref lastKeepAliveTime_, DateTime.UtcNow.Ticks);

                lock (outstandingRequests_)
                {
                    for (var ii = outstandingRequests_.First; ii != null; ii = ii.Next)
                    {
                        if (ii.Value.RequestTypeId == DataTypes.PublishRequest)
                        {
                            ii.Value.Defunct = true;
                        }
                    }
                }

                StartPublishing(OperationTimeout, false);
            }
            else
            {
                Interlocked.Exchange(ref lastKeepAliveTime_, DateTime.UtcNow.Ticks);
            }

            // save server state.
            serverState_ = currentState;

            EventHandler<SessionKeepAliveEventArgs> callback = KeepAliveEventHandler;

            if (callback != null)
            {
                try
                {
                    callback(this, new SessionKeepAliveEventArgs(null, currentState, currentTime));
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Session: Unexpected error invoking KeepAliveCallback.");
                }
            }
        }

        /// <summary>
        /// Called when a error occurs during a keep alive.
        /// </summary>
        protected virtual bool OnKeepAliveError(ServiceResult result)
        {
            long delta = DateTime.UtcNow.Ticks - Interlocked.Read(ref lastKeepAliveTime_);

            Utils.LogInfo(
                "KEEP ALIVE LATE: {0}s, EndpointUrl={1}, RequestCount={2}/{3}",
                ((double)delta) / TimeSpan.TicksPerSecond,
                Endpoint?.EndpointUrl,
                GoodPublishRequestCount,
                OutstandingRequestCount);

            EventHandler<SessionKeepAliveEventArgs> callback = KeepAliveEventHandler;

            if (callback != null)
            {
                try
                {
                    var args = new SessionKeepAliveEventArgs(result, ServerState.Unknown, DateTime.UtcNow);
                    callback(this, args);
                    return !args.CancelKeepAlive;
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Session: Unexpected error invoking KeepAliveCallback.");
                }
            }

            return true;
        }

        /// <summary>
        /// Prepare a list of subscriptions to delete.
        /// </summary>
        private bool PrepareSubscriptionsToDelete(IEnumerable<Subscription> subscriptions, IList<Subscription> subscriptionsToDelete)
        {
            bool removed = false;
            lock (SyncRoot)
            {
                foreach (Subscription subscription in subscriptions)
                {
                    if (subscriptions_.Remove(subscription))
                    {
                        if (subscription.Created)
                        {
                            subscriptionsToDelete.Add(subscription);
                        }

                        removed = true;
                    }
                }
            }
            return removed;
        }

        /// <summary>
        /// Creates a read request with attributes determined by the NodeClass.
        /// </summary>
        private void CreateNodeClassAttributesReadNodesRequest(
            IList<NodeId> nodeIdCollection,
            NodeClass nodeClass,
            ReadValueIdCollection attributesToRead,
            IList<IDictionary<uint, DataValue>> attributesPerNodeId,
            IList<Node> nodeCollection,
            bool optionalAttributes)
        {
            for (int ii = 0; ii < nodeIdCollection.Count; ii++)
            {
                var node = new Node();
                node.NodeId = nodeIdCollection[ii];
                node.NodeClass = nodeClass;

                var attributes = CreateAttributes(node.NodeClass, optionalAttributes);
                foreach (uint attributeId in attributes.Keys)
                {
                    ReadValueId itemToRead = new ReadValueId {
                        NodeId = node.NodeId,
                        AttributeId = attributeId
                    };
                    attributesToRead.Add(itemToRead);
                }

                nodeCollection.Add(node);
                attributesPerNodeId.Add(attributes);
            }
        }

        /// <summary>
        /// Prepares the list of node ids to read to fetch the namespace table.
        /// </summary>
        private ReadValueIdCollection PrepareNamespaceTableNodesToRead()
        {
            var nodesToRead = new ReadValueIdCollection();

            // request namespace array.
            ReadValueId valueId = new ReadValueId {
                NodeId = Variables.Server_NamespaceArray,
                AttributeId = Attributes.Value
            };

            nodesToRead.Add(valueId);

            // request server array.
            valueId = new ReadValueId {
                NodeId = Variables.Server_ServerArray,
                AttributeId = Attributes.Value
            };

            nodesToRead.Add(valueId);

            return nodesToRead;
        }

        /// <summary>
        /// Updates the NamespaceTable with the result of the <see cref="PrepareNamespaceTableNodesToRead"/> read operation.
        /// </summary>
        private void UpdateNamespaceTable(DataValueCollection values, DiagnosticInfoCollection diagnosticInfos, ResponseHeader responseHeader)
        {
            // validate namespace array.
            ServiceResult result = ValidateDataValue(values[0], typeof(string[]), 0, diagnosticInfos, responseHeader);

            if (ServiceResult.IsBad(result))
            {
                Utils.LogError("FetchNamespaceTables: Cannot read NamespaceArray node: {0}", result.StatusCode);
            }
            else
            {
                NamespaceUris.Update((string[])values[0].Value);
            }

            // validate server array.
            result = ValidateDataValue(values[1], typeof(string[]), 1, diagnosticInfos, responseHeader);

            if (ServiceResult.IsBad(result))
            {
                Utils.LogError("FetchNamespaceTables: Cannot read ServerArray node: {0} ", result.StatusCode);
            }
            else
            {
                ServerUris.Update((string[])values[1].Value);
            }
        }

        /// <summary>
        /// Creates a read request with attributes determined by the NodeClass.
        /// </summary>
        private void CreateAttributesReadNodesRequest(
            ResponseHeader responseHeader,
            ReadValueIdCollection itemsToRead,
            DataValueCollection nodeClassValues,
            DiagnosticInfoCollection diagnosticInfos,
            ReadValueIdCollection attributesToRead,
            IList<IDictionary<uint, DataValue>> attributesPerNodeId,
            IList<Node> nodeCollection,
            IList<ServiceResult> errors,
            bool optionalAttributes
            )
        {
            int? nodeClass;
            for (int ii = 0; ii < itemsToRead.Count; ii++)
            {
                var node = new Node();
                node.NodeId = itemsToRead[ii].NodeId;
                if (!DataValue.IsGood(nodeClassValues[ii]))
                {
                    nodeCollection.Add(node);
                    errors.Add(new ServiceResult(nodeClassValues[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable));
                    attributesPerNodeId.Add(null);
                    continue;
                }

                // check for valid node class.
                nodeClass = nodeClassValues[ii].Value as int?;

                if (nodeClass == null)
                {
                    nodeCollection.Add(node);
                    errors.Add(ServiceResult.Create(StatusCodes.BadUnexpectedError,
                        "Node does not have a valid value for NodeClass: {0}.", nodeClassValues[ii].Value));
                    attributesPerNodeId.Add(null);
                    continue;
                }

                node.NodeClass = (NodeClass)nodeClass;

                var attributes = CreateAttributes(node.NodeClass, optionalAttributes);
                foreach (uint attributeId in attributes.Keys)
                {
                    ReadValueId itemToRead = new ReadValueId {
                        NodeId = node.NodeId,
                        AttributeId = attributeId
                    };
                    attributesToRead.Add(itemToRead);
                }

                nodeCollection.Add(node);
                errors.Add(ServiceResult.Good);
                attributesPerNodeId.Add(attributes);
            }
        }

        /// <summary>
        /// Builds the node collection results based on the attribute values of the read response.
        /// </summary>
        /// <param name="attributesToRead">The collection of all attributes to read passed in the read request.</param>
        /// <param name="attributesPerNodeId">The attributes requested per NodeId</param>
        /// <param name="values">The attribute values returned by the read request.</param>
        /// <param name="diagnosticInfos">The diagnostic info returned by the read request.</param>
        /// <param name="responseHeader">The response header of the read request.</param>
        /// <param name="nodeCollection">The node collection which holds the results.</param>
        /// <param name="errors">The service results for each node.</param>
        private void ProcessAttributesReadNodesResponse(
            ResponseHeader responseHeader,
            ReadValueIdCollection attributesToRead,
            IList<IDictionary<uint, DataValue>> attributesPerNodeId,
            DataValueCollection values,
            DiagnosticInfoCollection diagnosticInfos,
            IList<Node> nodeCollection,
            IList<ServiceResult> errors)
        {
            int readIndex = 0;
            for (int ii = 0; ii < nodeCollection.Count; ii++)
            {
                var attributes = attributesPerNodeId[ii];
                if (attributes == null)
                {
                    continue;
                }

                int readCount = attributes.Count;
                ReadValueIdCollection subRangeAttributes = new ReadValueIdCollection(attributesToRead.GetRange(readIndex, readCount));
                DataValueCollection subRangeValues = new DataValueCollection(values.GetRange(readIndex, readCount));
                DiagnosticInfoCollection subRangeDiagnostics = diagnosticInfos.Count > 0 ? new DiagnosticInfoCollection(diagnosticInfos.GetRange(readIndex, readCount)) : diagnosticInfos;
                try
                {
                    nodeCollection[ii] = ProcessReadResponse(responseHeader, attributes,
                        subRangeAttributes, subRangeValues, subRangeDiagnostics);
                    errors[ii] = ServiceResult.Good;
                }
                catch (ServiceResultException sre)
                {
                    errors[ii] = sre.Result;
                }
                readIndex += readCount;
            }
        }

        /// <summary>
        /// Creates a Node based on the read response.
        /// </summary>
        private Node ProcessReadResponse(
            ResponseHeader responseHeader,
            IDictionary<uint, DataValue> attributes,
            ReadValueIdCollection itemsToRead,
            DataValueCollection values,
            DiagnosticInfoCollection diagnosticInfos)
        {
            // process results.
            int? nodeClass = null;

            for (int ii = 0; ii < itemsToRead.Count; ii++)
            {
                uint attributeId = itemsToRead[ii].AttributeId;

                // the node probably does not exist if the node class is not found.
                if (attributeId == Attributes.NodeClass)
                {
                    if (!DataValue.IsGood(values[ii]))
                    {
                        throw ServiceResultException.Create(values[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                    }

                    // check for valid node class.
                    nodeClass = values[ii].Value as int?;

                    if (nodeClass == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not have a valid value for NodeClass: {0}.", values[ii].Value);
                    }
                }
                else
                {
                    if (!DataValue.IsGood(values[ii]))
                    {
                        // check for unsupported attributes.
                        if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                        {
                            continue;
                        }

                        // ignore errors on optional attributes
                        if (StatusCode.IsBad(values[ii].StatusCode))
                        {
                            if (attributeId == Attributes.AccessRestrictions ||
                                attributeId == Attributes.Description ||
                                attributeId == Attributes.RolePermissions ||
                                attributeId == Attributes.UserRolePermissions ||
                                attributeId == Attributes.UserWriteMask ||
                                attributeId == Attributes.WriteMask)
                            {
                                continue;
                            }
                        }

                        // all supported attributes must be readable.
                        if (attributeId != Attributes.Value)
                        {
                            throw ServiceResultException.Create(values[ii].StatusCode, ii, diagnosticInfos, responseHeader.StringTable);
                        }
                    }
                }

                attributes[attributeId] = values[ii];
            }

            Node node;
            DataValue value;
            switch ((NodeClass)nodeClass.Value)
            {
                default:
                {
                    throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not have a valid value for NodeClass: {0}.", nodeClass.Value);
                }

                case NodeClass.Object:
                {
                    ObjectNode objectNode = new ObjectNode();

                    value = attributes[Attributes.EventNotifier];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Object does not support the EventNotifier attribute.");
                    }

                    objectNode.EventNotifier = (byte)value.GetValue(typeof(byte));
                    node = objectNode;
                    break;
                }

                case NodeClass.ObjectType:
                {
                    ObjectTypeNode objectTypeNode = new ObjectTypeNode();

                    value = attributes[Attributes.IsAbstract];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "ObjectType does not support the IsAbstract attribute.");
                    }

                    objectTypeNode.IsAbstract = (bool)value.GetValue(typeof(bool));
                    node = objectTypeNode;
                    break;
                }

                case NodeClass.Variable:
                {
                    VariableNode variableNode = new VariableNode();

                    // DataType Attribute
                    value = attributes[Attributes.DataType];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the DataType attribute.");
                    }

                    variableNode.DataType = (NodeId)value.GetValue(typeof(NodeId));

                    // ValueRank Attribute
                    value = attributes[Attributes.ValueRank];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the ValueRank attribute.");
                    }

                    variableNode.ValueRank = (int)value.GetValue(typeof(int));

                    // ArrayDimensions Attribute
                    value = attributes[Attributes.ArrayDimensions];

                    if (value != null)
                    {
                        if (value.Value == null)
                        {
                            variableNode.ArrayDimensions = Array.Empty<uint>();
                        }
                        else
                        {
                            variableNode.ArrayDimensions = (uint[])value.GetValue(typeof(uint[]));
                        }
                    }

                    // AccessLevel Attribute
                    value = attributes[Attributes.AccessLevel];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the AccessLevel attribute.");
                    }

                    variableNode.AccessLevel = (byte)value.GetValue(typeof(byte));

                    // UserAccessLevel Attribute
                    value = attributes[Attributes.UserAccessLevel];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the UserAccessLevel attribute.");
                    }

                    variableNode.UserAccessLevel = (byte)value.GetValue(typeof(byte));

                    // Historizing Attribute
                    value = attributes[Attributes.Historizing];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Variable does not support the Historizing attribute.");
                    }

                    variableNode.Historizing = (bool)value.GetValue(typeof(bool));

                    // MinimumSamplingInterval Attribute
                    value = attributes[Attributes.MinimumSamplingInterval];

                    if (value != null)
                    {
                        variableNode.MinimumSamplingInterval = Convert.ToDouble(attributes[Attributes.MinimumSamplingInterval].Value);
                    }

                    // AccessLevelEx Attribute
                    value = attributes[Attributes.AccessLevelEx];

                    if (value != null)
                    {
                        variableNode.AccessLevelEx = (uint)value.GetValue(typeof(uint));
                    }

                    node = variableNode;
                    break;
                }

                case NodeClass.VariableType:
                {
                    VariableTypeNode variableTypeNode = new VariableTypeNode();

                    // IsAbstract Attribute
                    value = attributes[Attributes.IsAbstract];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "VariableType does not support the IsAbstract attribute.");
                    }

                    variableTypeNode.IsAbstract = (bool)value.GetValue(typeof(bool));

                    // DataType Attribute
                    value = attributes[Attributes.DataType];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "VariableType does not support the DataType attribute.");
                    }

                    variableTypeNode.DataType = (NodeId)value.GetValue(typeof(NodeId));

                    // ValueRank Attribute
                    value = attributes[Attributes.ValueRank];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "VariableType does not support the ValueRank attribute.");
                    }

                    variableTypeNode.ValueRank = (int)value.GetValue(typeof(int));

                    // ArrayDimensions Attribute
                    value = attributes[Attributes.ArrayDimensions];

                    if (value != null && value.Value != null)
                    {
                        variableTypeNode.ArrayDimensions = (uint[])value.GetValue(typeof(uint[]));
                    }

                    node = variableTypeNode;
                    break;
                }

                case NodeClass.Method:
                {
                    MethodNode methodNode = new MethodNode();

                    // Executable Attribute
                    value = attributes[Attributes.Executable];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Method does not support the Executable attribute.");
                    }

                    methodNode.Executable = (bool)value.GetValue(typeof(bool));

                    // UserExecutable Attribute
                    value = attributes[Attributes.UserExecutable];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Method does not support the UserExecutable attribute.");
                    }

                    methodNode.UserExecutable = (bool)value.GetValue(typeof(bool));

                    node = methodNode;
                    break;
                }

                case NodeClass.DataType:
                {
                    DataTypeNode dataTypeNode = new DataTypeNode();

                    // IsAbstract Attribute
                    value = attributes[Attributes.IsAbstract];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "DataType does not support the IsAbstract attribute.");
                    }

                    dataTypeNode.IsAbstract = (bool)value.GetValue(typeof(bool));

                    // DataTypeDefinition Attribute
                    value = attributes[Attributes.DataTypeDefinition];

                    if (value != null)
                    {
                        dataTypeNode.DataTypeDefinition = value.Value as ExtensionObject;
                    }

                    node = dataTypeNode;
                    break;
                }

                case NodeClass.ReferenceType:
                {
                    ReferenceTypeNode referenceTypeNode = new ReferenceTypeNode();

                    // IsAbstract Attribute
                    value = attributes[Attributes.IsAbstract];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "ReferenceType does not support the IsAbstract attribute.");
                    }

                    referenceTypeNode.IsAbstract = (bool)value.GetValue(typeof(bool));

                    // Symmetric Attribute
                    value = attributes[Attributes.Symmetric];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "ReferenceType does not support the Symmetric attribute.");
                    }

                    referenceTypeNode.Symmetric = (bool)value.GetValue(typeof(bool));

                    // InverseName Attribute
                    value = attributes[Attributes.InverseName];

                    if (value != null && value.Value != null)
                    {
                        referenceTypeNode.InverseName = (LocalizedText)value.GetValue(typeof(LocalizedText));
                    }

                    node = referenceTypeNode;
                    break;
                }

                case NodeClass.View:
                {
                    ViewNode viewNode = new ViewNode();

                    // EventNotifier Attribute
                    value = attributes[Attributes.EventNotifier];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "View does not support the EventNotifier attribute.");
                    }

                    viewNode.EventNotifier = (byte)value.GetValue(typeof(byte));

                    // ContainsNoLoops Attribute
                    value = attributes[Attributes.ContainsNoLoops];

                    if (value == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "View does not support the ContainsNoLoops attribute.");
                    }

                    viewNode.ContainsNoLoops = (bool)value.GetValue(typeof(bool));

                    node = viewNode;
                    break;
                }
            }

            // NodeId Attribute
            value = attributes[Attributes.NodeId];

            if (value == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not support the NodeId attribute.");
            }

            node.NodeId = (NodeId)value.GetValue(typeof(NodeId));
            node.NodeClass = (NodeClass)nodeClass.Value;

            // BrowseName Attribute
            value = attributes[Attributes.BrowseName];

            if (value == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not support the BrowseName attribute.");
            }

            node.BrowseName = (QualifiedName)value.GetValue(typeof(QualifiedName));

            // DisplayName Attribute
            value = attributes[Attributes.DisplayName];

            if (value == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Node does not support the DisplayName attribute.");
            }

            node.DisplayName = (LocalizedText)value.GetValue(typeof(LocalizedText));

            // all optional attributes follow

            // Description Attribute
            if (attributes.TryGetValue(Attributes.Description, out value) &&
                value != null && value.Value != null)
            {
                node.Description = (LocalizedText)value.GetValue(typeof(LocalizedText));
            }

            // WriteMask Attribute
            if (attributes.TryGetValue(Attributes.WriteMask, out value) &&
                value != null)
            {
                node.WriteMask = (uint)value.GetValue(typeof(uint));
            }

            // UserWriteMask Attribute
            if (attributes.TryGetValue(Attributes.UserWriteMask, out value) &&
                value != null)
            {
                node.UserWriteMask = (uint)value.GetValue(typeof(uint));
            }

            // RolePermissions Attribute
            if (attributes.TryGetValue(Attributes.RolePermissions, out value) &&
                value != null)
            {
                ExtensionObject[] rolePermissions = value.Value as ExtensionObject[];

                if (rolePermissions != null)
                {
                    node.RolePermissions = new RolePermissionTypeCollection();

                    foreach (ExtensionObject rolePermission in rolePermissions)
                    {
                        node.RolePermissions.Add(rolePermission.Body as RolePermissionType);
                    }
                }
            }

            // UserRolePermissions Attribute
            if (attributes.TryGetValue(Attributes.UserRolePermissions, out value) &&
                value != null)
            {
                ExtensionObject[] userRolePermissions = value.Value as ExtensionObject[];

                if (userRolePermissions != null)
                {
                    node.UserRolePermissions = new RolePermissionTypeCollection();

                    foreach (ExtensionObject rolePermission in userRolePermissions)
                    {
                        node.UserRolePermissions.Add(rolePermission.Body as RolePermissionType);
                    }
                }
            }

            // AccessRestrictions Attribute
            if (attributes.TryGetValue(Attributes.AccessRestrictions, out value) &&
                value != null)
            {
                node.AccessRestrictions = (ushort)value.GetValue(typeof(ushort));
            }

            return node;
        }

        /// <summary>
        /// Create a dictionary of attributes to read for a nodeclass.
        /// </summary>
        private IDictionary<uint, DataValue> CreateAttributes(NodeClass nodeclass = NodeClass.Unspecified, bool optionalAttributes = true)
        {
            // Attributes to read for all types of nodes
            var attributes = new SortedDictionary<uint, DataValue>() {
                { Attributes.NodeId, null },
                { Attributes.NodeClass, null },
                { Attributes.BrowseName, null },
                { Attributes.DisplayName, null },
            };

            switch (nodeclass)
            {
                case NodeClass.Object:
                    attributes.Add(Attributes.EventNotifier, null);
                    break;

                case NodeClass.Variable:
                    attributes.Add(Attributes.DataType, null);
                    attributes.Add(Attributes.ValueRank, null);
                    attributes.Add(Attributes.ArrayDimensions, null);
                    attributes.Add(Attributes.AccessLevel, null);
                    attributes.Add(Attributes.UserAccessLevel, null);
                    attributes.Add(Attributes.Historizing, null);
                    attributes.Add(Attributes.MinimumSamplingInterval, null);
                    attributes.Add(Attributes.AccessLevelEx, null);
                    break;

                case NodeClass.Method:
                    attributes.Add(Attributes.Executable, null);
                    attributes.Add(Attributes.UserExecutable, null);
                    break;

                case NodeClass.ObjectType:
                    attributes.Add(Attributes.IsAbstract, null);
                    break;

                case NodeClass.VariableType:
                    attributes.Add(Attributes.IsAbstract, null);
                    attributes.Add(Attributes.DataType, null);
                    attributes.Add(Attributes.ValueRank, null);
                    attributes.Add(Attributes.ArrayDimensions, null);
                    break;

                case NodeClass.ReferenceType:
                    attributes.Add(Attributes.IsAbstract, null);
                    attributes.Add(Attributes.Symmetric, null);
                    attributes.Add(Attributes.InverseName, null);
                    break;

                case NodeClass.DataType:
                    attributes.Add(Attributes.IsAbstract, null);
                    attributes.Add(Attributes.DataTypeDefinition, null);
                    break;

                case NodeClass.View:
                    attributes.Add(Attributes.EventNotifier, null);
                    attributes.Add(Attributes.ContainsNoLoops, null);
                    break;

                default:
                    // build complete list of attributes.
                    attributes = new SortedDictionary<uint, DataValue> {
                        { Attributes.NodeId, null },
                        { Attributes.NodeClass, null },
                        { Attributes.BrowseName, null },
                        { Attributes.DisplayName, null },
                        //{ Attributes.Description, null },
                        //{ Attributes.WriteMask, null },
                        //{ Attributes.UserWriteMask, null },
                        { Attributes.DataType, null },
                        { Attributes.ValueRank, null },
                        { Attributes.ArrayDimensions, null },
                        { Attributes.AccessLevel, null },
                        { Attributes.UserAccessLevel, null },
                        { Attributes.MinimumSamplingInterval, null },
                        { Attributes.Historizing, null },
                        { Attributes.EventNotifier, null },
                        { Attributes.Executable, null },
                        { Attributes.UserExecutable, null },
                        { Attributes.IsAbstract, null },
                        { Attributes.InverseName, null },
                        { Attributes.Symmetric, null },
                        { Attributes.ContainsNoLoops, null },
                        { Attributes.DataTypeDefinition, null },
                        //{ Attributes.RolePermissions, null },
                        //{ Attributes.UserRolePermissions, null },
                        //{ Attributes.AccessRestrictions, null },
                        { Attributes.AccessLevelEx, null }
                    };
                    break;
            }

            if (optionalAttributes)
            {
                attributes.Add(Attributes.Description, null);
                attributes.Add(Attributes.WriteMask, null);
                attributes.Add(Attributes.UserWriteMask, null);
                attributes.Add(Attributes.RolePermissions, null);
                attributes.Add(Attributes.UserRolePermissions, null);
                attributes.Add(Attributes.AccessRestrictions, null);
            }

            return attributes;
        }
        #endregion

        #region Publish Methods
        /// <summary>
        /// Sends an additional publish request.
        /// </summary>
        public IAsyncResult BeginPublish(int timeout)
        {
            // do not publish if reconnecting.
            if (reconnecting_)
            {
                Utils.LogWarning("Publish skipped due to reconnect");
                return null;
            }

            // get event handler to modify ack list
            EventHandler<PublishSequenceNumbersToAcknowledgeEventArgs> callback = PublishSequenceNumbersToAcknowledgeEventHandler;

            // collect the current set if acknowledgements.
            SubscriptionAcknowledgementCollection acknowledgementsToSend = null;
            lock (acknowledgementsToSendLock_)
            {
                if (callback != null)
                {
                    try
                    {
                        var deferredAcknowledgementsToSend = new SubscriptionAcknowledgementCollection();
                        callback(this, new PublishSequenceNumbersToAcknowledgeEventArgs(acknowledgementsToSend_, deferredAcknowledgementsToSend));
                        acknowledgementsToSend = acknowledgementsToSend_;
                        acknowledgementsToSend_ = deferredAcknowledgementsToSend;
                    }
                    catch (Exception e2)
                    {
                        Utils.LogError(e2, "Session: Unexpected error invoking PublishSequenceNumbersToAcknowledgeEventArgs.");
                    }
                }

                if (acknowledgementsToSend == null)
                {
                    // send all ack values, clear list
                    acknowledgementsToSend = acknowledgementsToSend_;
                    acknowledgementsToSend_ = new SubscriptionAcknowledgementCollection();
                }
#if DEBUG_SEQUENTIALPUBLISHING
                foreach (var toSend in acknowledgementsToSend)
                {
                    latestAcknowledgementsSent_[toSend.SubscriptionId] = toSend.SequenceNumber;
                }
#endif
            }

            uint timeoutHint = (uint)((timeout > 0) ? (uint)timeout : uint.MaxValue);
            timeoutHint = Math.Min((uint)(OperationTimeout / 2), timeoutHint);

            // send publish request.
            var requestHeader = new RequestHeader {
            // ensure the publish request is discarded before the timeout occurs to ensure the channel is dropped.
                TimeoutHint = timeoutHint,
                ReturnDiagnostics = (uint)(int)ReturnDiagnostics,
                RequestHandle = Utils.IncrementIdentifier(ref publishCounter_)
            };

            var state = new AsyncRequestState {
                RequestTypeId = DataTypes.PublishRequest,
                RequestId = requestHeader.RequestHandle,
                Timestamp = DateTime.UtcNow
            };

            UaClientUtils.EventLog.PublishStart((int)requestHeader.RequestHandle);

            try
            {
                var result = BeginPublish(
                    requestHeader,
                    acknowledgementsToSend,
                    OnPublishComplete,
                    new object[] { SessionId, acknowledgementsToSend, requestHeader });

                AsyncRequestStarted(result, requestHeader.RequestHandle, DataTypes.PublishRequest);

                return result;
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error sending publish request.");
                return null;
            }
        }

        /// <summary>
        /// Create the publish requests for the active subscriptions.
        /// </summary>
        public void StartPublishing(int timeout, bool fullQueue)
        {
            int publishCount = GetMinPublishRequestCount(true);

            if (tooManyPublishRequests_ > 0 && publishCount > tooManyPublishRequests_)
            {
                publishCount = Math.Max(tooManyPublishRequests_, minPublishRequestCount_);
            }

            // refill pipeline. Send at least one publish request if subscriptions are active.
            if (publishCount > 0 && BeginPublish(timeout) != null)
            {
                int startCount = fullQueue ? 1 : GoodPublishRequestCount + 1;
                for (int ii = startCount; ii < publishCount; ii++)
                {
                    if (BeginPublish(timeout) == null)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Completes an asynchronous publish operation.
        /// </summary>
        private void OnPublishComplete(IAsyncResult result)
        {
            // extract state information.
            var state = (object[])result.AsyncState;
            var sessionId = (NodeId)state[0];
            var acknowledgementsToSend = (SubscriptionAcknowledgementCollection)state[1];
            var requestHeader = (RequestHeader)state[2];
            uint subscriptionId = 0;
            bool moreNotifications;

            AsyncRequestCompleted(result, requestHeader.RequestHandle, DataTypes.PublishRequest);

            UaClientUtils.EventLog.PublishStop((int)requestHeader.RequestHandle);

            try
            {
                // gate entry if transfer/reactivate is busy
                reconnectLock_.Wait();
                bool reconnecting = reconnecting_;
                reconnectLock_.Release();

                // complete publish.
                var responseHeader = EndPublish(
                    result,
                    out subscriptionId,
                    out var availableSequenceNumbers,
                    out moreNotifications,
                    out var notificationMessage,
                    out var acknowledgeResults,
                    out _);

                LogLevel logLevel = LogLevel.Warning;
                foreach (var code in acknowledgeResults)
                {
                    if (StatusCode.IsBad(code))
                    {
                        Utils.Log(logLevel, "Publish Ack Response. ResultCode={0}; SubscriptionId={1}", code.ToString(), subscriptionId);
                        // only show the first error as warning
                        logLevel = LogLevel.Trace;
                    }
                }

                // nothing more to do if session changed.
                if (sessionId != SessionId)
                {
                    Utils.LogWarning("Publish response discarded because session id changed: Old {0} != New {1}", sessionId, SessionId);
                    return;
                }

                UaClientUtils.EventLog.NotificationReceived((int)subscriptionId, (int)notificationMessage.SequenceNumber);

                // process response.
                ProcessPublishResponse(
                    responseHeader,
                    subscriptionId,
                    availableSequenceNumbers,
                    moreNotifications,
                    notificationMessage);

                // nothing more to do if reconnecting.
                if (reconnecting)
                {
                    Utils.LogWarning("No new publish sent because of reconnect in progress.");
                    return;
                }
            }
            catch (Exception e)
            {
                if (subscriptions_.Count == 0)
                {
                    // Publish responses with error should occur after deleting the last subscription.
                    Utils.LogError("Publish #{0}, Subscription count = 0, Error: {1}", requestHeader.RequestHandle, e.Message);
                }
                else
                {
                    Utils.LogError("Publish #{0}, Reconnecting={1}, Error: {2}", requestHeader.RequestHandle, reconnecting_, e.Message);
                }

                // raise an error event.
                ServiceResult error = new ServiceResult(e);

                if (error.Code != StatusCodes.BadNoSubscription)
                {
                    EventHandler<SessionPublishErrorEventArgs> callback = PublishErrorEventHandler;

                    if (callback != null)
                    {
                        try
                        {
                            callback(this, new SessionPublishErrorEventArgs(error, subscriptionId, 0));
                        }
                        catch (Exception e2)
                        {
                            Utils.LogError(e2, "Session: Unexpected error invoking PublishErrorCallback.");
                        }
                    }
                }

                // ignore errors if reconnecting.
                if (reconnecting_)
                {
                    Utils.LogWarning("Publish abandoned after error due to reconnect: {0}", e.Message);
                    return;
                }

                // nothing more to do if session changed.
                if (sessionId != SessionId)
                {
                    Utils.LogError("Publish abandoned after error because session id changed: Old {0} != New {1}", sessionId, SessionId);
                    return;
                }

                // try to acknowledge the notifications again in the next publish.
                if (acknowledgementsToSend != null)
                {
                    lock (acknowledgementsToSendLock_)
                    {
                        acknowledgementsToSend_.AddRange(acknowledgementsToSend);
                    }
                }

                // don't send another publish for these errors,
                // or throttle to avoid server overload.
                switch (error.Code)
                {
                    case StatusCodes.BadTooManyPublishRequests:
                        int tooManyPublishRequests = GoodPublishRequestCount;
                        if (BelowPublishRequestLimit(tooManyPublishRequests))
                        {
                            tooManyPublishRequests_ = tooManyPublishRequests;
                            Utils.LogInfo("PUBLISH - Too many requests, set limit to GoodPublishRequestCount={0}.", tooManyPublishRequests_);
                        }
                        return;

                    case StatusCodes.BadNoSubscription:
                    case StatusCodes.BadSessionClosed:
                    case StatusCodes.BadSessionIdInvalid:
                    case StatusCodes.BadSecureChannelIdInvalid:
                    case StatusCodes.BadSecureChannelClosed:
                    case StatusCodes.BadSecurityChecksFailed:
                    case StatusCodes.BadCertificateInvalid:
                    case StatusCodes.BadServerHalted:
                            return;

                    // Servers may return this error when overloaded
                    case StatusCodes.BadTooManyOperations:
                    case StatusCodes.BadTcpServerTooBusy:
                    case StatusCodes.BadServerTooBusy:
                        // throttle the next publish to reduce server load
                        _ = Task.Run(async () => {
                            await Task.Delay(100).ConfigureAwait(false);
                            BeginPublish(OperationTimeout);
                        });
                        return;

                    default:
                        Utils.LogError(e, "PUBLISH #{0} - Unhandled error {1} during Publish.", requestHeader.RequestHandle, error.StatusCode);
                        goto case StatusCodes.BadServerTooBusy;

                        }
            }

            var requestCount = GoodPublishRequestCount;
            var minPublishRequestCount = GetMinPublishRequestCount(false);

            if (requestCount < minPublishRequestCount)
            {
                BeginPublish(OperationTimeout);
            }
            else
            {
                Utils.LogInfo("PUBLISH - Did not send another publish request. GoodPublishRequestCount={0}, MinPublishRequestCount={1}", requestCount, minPublishRequestCount);
            }
        }

        /// <inheritdoc/>
        public bool Republish(uint subscriptionId, uint sequenceNumber, out ServiceResult error)
        {
            bool result = true;
            error = ServiceResult.Good;

            // send republish request.
            var requestHeader = new RequestHeader {
                TimeoutHint = (uint)OperationTimeout,
                ReturnDiagnostics = (uint)(int)ReturnDiagnostics,
                RequestHandle = Utils.IncrementIdentifier(ref publishCounter_)
            };

            try
            {
                Utils.LogInfo("Requesting Republish for {0}-{1}", subscriptionId, sequenceNumber);

                // request republish.

                var responseHeader = Republish(
                    requestHeader,
                    subscriptionId,
                    sequenceNumber,
                    out var notificationMessage);

                Utils.LogInfo("Received Republish for {0}-{1}-{2}", subscriptionId, sequenceNumber, responseHeader.ServiceResult);

                // process response.
                ProcessPublishResponse(
                    responseHeader,
                    subscriptionId,
                    null,
                    false,
                    notificationMessage);
            }
            catch (Exception e)
            {
                (result, error) = ProcessRepublishResponseError(e, subscriptionId, sequenceNumber);
            }

            return result;
        }

        /// <inheritdoc/>
        public bool ResendData(IEnumerable<Subscription> subscriptions, out IList<ServiceResult> errors)
        {
            CallMethodRequestCollection requests = CreateCallRequestsForResendData(subscriptions);

            errors = new List<ServiceResult>(requests.Count);

            CallMethodResultCollection results;
            DiagnosticInfoCollection diagnosticInfos;
            try
            {
                ResponseHeader responseHeader = Call(
                    null,
                    requests,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, requests);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, requests);

                int ii = 0;
                foreach (var value in results)
                {
                    ServiceResult result = ServiceResult.Good;
                    if (StatusCode.IsNotGood(value.StatusCode))
                    {
                        result = ClientBase.GetResult(value.StatusCode, ii, diagnosticInfos, responseHeader);
                    }
                    errors.Add(result);
                    ii++;
                }

                return true;
            }
            catch (ServiceResultException sre)
            {
                Utils.LogError(sre, "Failed to call ResendData on server.");
            }

            return false;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Validates  the identity for an open call.
        /// </summary>
        private void OpenValidateIdentity(
            ref IUserIdentity identity,
            out UserIdentityToken identityToken,
            out UserTokenPolicy identityPolicy,
            out string securityPolicyUri,
            out bool requireEncryption)
        {
            // check connection state.
            lock (SyncRoot)
            {
                if (Connected)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidState, "Already connected to server.");
                }
            }

            securityPolicyUri = ConfiguredEndpoint.Description.SecurityPolicyUri;

            // catch security policies which are not supported by core
            if (SecurityPolicies.GetDisplayName(securityPolicyUri) == null)
            {
                throw ServiceResultException.Create(
                    StatusCodes.BadSecurityChecksFailed,
                    "The chosen security policy is not supported by the client to connect to the server.");
            }

            // get the identity token.
            if (identity == null)
            {
                identity = new UserIdentity();
            }

            // get identity token.
            identityToken = identity.GetIdentityToken();

            // check that the user identity is supported by the endpoint.
            identityPolicy = ConfiguredEndpoint.Description.FindUserTokenPolicy(identityToken.PolicyId);

            if (identityPolicy == null)
            {
                // try looking up by TokenType if the policy id was not found.
                identityPolicy = ConfiguredEndpoint.Description.FindUserTokenPolicy(identity.TokenType, identity.IssuedTokenType);

                if (identityPolicy == null)
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadUserAccessDenied,
                        "Endpoint does not support the user identity type provided.");
                }

                identityToken.PolicyId = identityPolicy.PolicyId;
            }

            requireEncryption = securityPolicyUri != SecurityPolicies.None;

            if (!requireEncryption)
            {
                requireEncryption = identityPolicy.SecurityPolicyUri != SecurityPolicies.None &&
                    !String.IsNullOrEmpty(identityPolicy.SecurityPolicyUri);
            }
        }

        private void BuildCertificateData(out byte[] clientCertificateData, out byte[] clientCertificateChainData)
        {
            // send the application instance certificate for the client.
            clientCertificateData = instanceCertificate_ != null ? instanceCertificate_.RawData : null;
            clientCertificateChainData = null;

            if (instanceCertificateChain_ != null && instanceCertificateChain_.Count > 0 &&
                configuration_.SecurityConfiguration.SendCertificateChain)
            {
                List<byte> clientCertificateChain = new List<byte>();

                for (int i = 0; i < instanceCertificateChain_.Count; i++)
                {
                    clientCertificateChain.AddRange(instanceCertificateChain_[i].RawData);
                }

                clientCertificateChainData = clientCertificateChain.ToArray();
            }
        }

        /// <summary>
        /// Validates the server certificate returned.
        /// </summary>
        private void ValidateServerCertificateData(byte[] serverCertificateData)
        {
            if (serverCertificateData != null &&
                ConfiguredEndpoint.Description.ServerCertificate != null &&
                !Utils.IsEqual(serverCertificateData, ConfiguredEndpoint.Description.ServerCertificate))
            {
                try
                {
                    // verify for certificate chain in endpoint.
                    X509Certificate2Collection serverCertificateChain = Utils.ParseCertificateChainBlob(ConfiguredEndpoint.Description.ServerCertificate);

                    if (serverCertificateChain.Count > 0 && !Utils.IsEqual(serverCertificateData, serverCertificateChain[0].RawData))
                    {
                        throw ServiceResultException.Create(
                                    StatusCodes.BadCertificateInvalid,
                                    "Server did not return the certificate used to create the secure channel.");
                    }
                }
                catch (Exception)
                {
                    throw ServiceResultException.Create(
                            StatusCodes.BadCertificateInvalid,
                            "Server did not return the certificate used to create the secure channel.");
                }
            }
        }

        /// <summary>
        /// Validates the server signature created with the client nonce.
        /// </summary>
        private void ValidateServerSignature(X509Certificate2 serverCertificate, SignatureData serverSignature,
            byte[] clientCertificateData, byte[] clientCertificateChainData, byte[] clientNonce)
        {
            if (serverSignature == null || serverSignature.Signature == null)
            {
                Utils.LogInfo("Server signature is null or empty.");

                //throw ServiceResultException.Create(
                //    StatusCodes.BadSecurityChecksFailed,
                //    "Server signature is null or empty.");
            }

            // validate the server's signature.
            byte[] dataToSign = Utils.Append(clientCertificateData, clientNonce);

            if (!SecurityPolicies.Verify(serverCertificate, ConfiguredEndpoint.Description.SecurityPolicyUri, dataToSign, serverSignature))
            {
                // validate the signature with complete chain if the check with leaf certificate failed.
                if (clientCertificateChainData != null)
                {
                    dataToSign = Utils.Append(clientCertificateChainData, clientNonce);

                    if (!SecurityPolicies.Verify(serverCertificate, ConfiguredEndpoint.Description.SecurityPolicyUri, dataToSign, serverSignature))
                    {
                        throw ServiceResultException.Create(
                            StatusCodes.BadApplicationSignatureInvalid,
                            "Server did not provide a correct signature for the nonce data provided by the client.");
                    }
                }
                else
                {
                    throw ServiceResultException.Create(
                       StatusCodes.BadApplicationSignatureInvalid,
                       "Server did not provide a correct signature for the nonce data provided by the client.");
                }
            }
        }

        /// <summary>
        /// Validates the server endpoints returned.
        /// </summary>
        private void ValidateServerEndpoints(EndpointDescriptionCollection serverEndpoints)
        {
            if (discoveryServerEndpoints_ != null && discoveryServerEndpoints_.Count > 0)
            {
                // Compare EndpointDescriptions returned at GetEndpoints with values returned at CreateSession
                EndpointDescriptionCollection expectedServerEndpoints = null;

                if (serverEndpoints != null &&
                    discoveryProfileUris_ != null && discoveryProfileUris_.Count > 0)
                {
                    // Select EndpointDescriptions with a transportProfileUri that matches the
                    // profileUris specified in the original GetEndpoints() request.
                    expectedServerEndpoints = new EndpointDescriptionCollection();

                    foreach (EndpointDescription serverEndpoint in serverEndpoints)
                    {
                        if (discoveryProfileUris_.Contains(serverEndpoint.TransportProfileUri))
                        {
                            expectedServerEndpoints.Add(serverEndpoint);
                        }
                    }
                }
                else
                {
                    expectedServerEndpoints = serverEndpoints;
                }

                if (expectedServerEndpoints == null ||
                    discoveryServerEndpoints_.Count != expectedServerEndpoints.Count)
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadSecurityChecksFailed,
                        "Server did not return a number of ServerEndpoints that matches the one from GetEndpoints.");
                }

                for (int ii = 0; ii < expectedServerEndpoints.Count; ii++)
                {
                    EndpointDescription serverEndpoint = expectedServerEndpoints[ii];
                    EndpointDescription expectedServerEndpoint = discoveryServerEndpoints_[ii];

                    if (serverEndpoint.SecurityMode != expectedServerEndpoint.SecurityMode ||
                        serverEndpoint.SecurityPolicyUri != expectedServerEndpoint.SecurityPolicyUri ||
                        serverEndpoint.TransportProfileUri != expectedServerEndpoint.TransportProfileUri ||
                        serverEndpoint.SecurityLevel != expectedServerEndpoint.SecurityLevel)
                    {
                        throw ServiceResultException.Create(
                            StatusCodes.BadSecurityChecksFailed,
                            "The list of ServerEndpoints returned at CreateSession does not match the list from GetEndpoints.");
                    }

                    if (serverEndpoint.UserIdentityTokens.Count != expectedServerEndpoint.UserIdentityTokens.Count)
                    {
                        throw ServiceResultException.Create(
                            StatusCodes.BadSecurityChecksFailed,
                            "The list of ServerEndpoints returned at CreateSession does not match the one from GetEndpoints.");
                    }

                    for (int jj = 0; jj < serverEndpoint.UserIdentityTokens.Count; jj++)
                    {
                        if (!serverEndpoint.UserIdentityTokens[jj].IsEqual(expectedServerEndpoint.UserIdentityTokens[jj]))
                        {
                            throw ServiceResultException.Create(
                            StatusCodes.BadSecurityChecksFailed,
                            "The list of ServerEndpoints returned at CreateSession does not match the one from GetEndpoints.");
                        }
                    }
                }
            }

            // find the matching description (TBD - check domains against certificate).
            bool found = false;
            Uri expectedUrl = Utils.ParseUri(ConfiguredEndpoint.Description.EndpointUrl);

            if (expectedUrl != null)
            {
                for (int ii = 0; ii < serverEndpoints.Count; ii++)
                {
                    EndpointDescription serverEndpoint = serverEndpoints[ii];
                    Uri actualUrl = Utils.ParseUri(serverEndpoint.EndpointUrl);

                    if (actualUrl != null && actualUrl.Scheme == expectedUrl.Scheme)
                    {
                        if (serverEndpoint.SecurityPolicyUri == ConfiguredEndpoint.Description.SecurityPolicyUri)
                        {
                            if (serverEndpoint.SecurityMode == ConfiguredEndpoint.Description.SecurityMode)
                            {
                                // ensure endpoint has up to date information.
                                ConfiguredEndpoint.Description.Server.ApplicationName = serverEndpoint.Server.ApplicationName;
                                ConfiguredEndpoint.Description.Server.ApplicationUri = serverEndpoint.Server.ApplicationUri;
                                ConfiguredEndpoint.Description.Server.ApplicationType = serverEndpoint.Server.ApplicationType;
                                ConfiguredEndpoint.Description.Server.ProductUri = serverEndpoint.Server.ProductUri;
                                ConfiguredEndpoint.Description.TransportProfileUri = serverEndpoint.TransportProfileUri;
                                ConfiguredEndpoint.Description.UserIdentityTokens = serverEndpoint.UserIdentityTokens;

                                found = true;
                                break;
                            }
                        }
                    }
                }
            }

            // could be a security risk.
            if (!found)
            {
                throw ServiceResultException.Create(
                    StatusCodes.BadSecurityChecksFailed,
                    "Server did not return an EndpointDescription that matched the one used to create the secure channel.");
            }
        }

        /// <summary>
        /// Helper to prepare the reconnect channel
        /// and signature data before activate.
        /// </summary>
        private IAsyncResult PrepareReconnectBeginActivate(
            ITransportWaitingConnection connection,
            ITransportChannel transportChannel
            )
        {
            Utils.LogInfo("Session RECONNECT {0} starting.", SessionId);

            // create the client signature.
            byte[] dataToSign = Utils.Append(serverCertificate_ != null ? serverCertificate_.RawData : null, serverNonce_);
            EndpointDescription endpoint = ConfiguredEndpoint.Description;
            SignatureData clientSignature = SecurityPolicies.Sign(instanceCertificate_, endpoint.SecurityPolicyUri, dataToSign);

            // check that the user identity is supported by the endpoint.
            UserTokenPolicy identityPolicy = endpoint.FindUserTokenPolicy(Identity.TokenType, Identity.IssuedTokenType);

            if (identityPolicy == null)
            {
                Utils.LogError("Reconnect: Endpoint does not support the user identity type provided.");

                throw ServiceResultException.Create(
                    StatusCodes.BadUserAccessDenied,
                    "Endpoint does not support the user identity type provided.");
            }

            // select the security policy for the user token.
            string securityPolicyUri = identityPolicy.SecurityPolicyUri;

            if (String.IsNullOrEmpty(securityPolicyUri))
            {
                securityPolicyUri = endpoint.SecurityPolicyUri;
            }

            // need to refresh the identity (reprompt for password, refresh token).
            if (RenewUserIdentityEventHandler != null)
            {
                RenewUserIdentityEventArgs args = new RenewUserIdentityEventArgs(Identity);
                Identity = RenewUserIdentityEventHandler(this, args);
            }

            // validate server nonce and security parameters for user identity.
            ValidateServerNonce(
                Identity,
                serverNonce_,
                securityPolicyUri,
                previousServerNonce_,
                ConfiguredEndpoint.Description.SecurityMode);

            // sign data with user token.
            UserIdentityToken identityToken = Identity.GetIdentityToken();
            identityToken.PolicyId = identityPolicy.PolicyId;
            SignatureData userTokenSignature = identityToken.Sign(dataToSign, securityPolicyUri);

            // encrypt token.
            identityToken.Encrypt(serverCertificate_, serverNonce_, securityPolicyUri);

            // send the software certificates assigned to the client.
            SignedSoftwareCertificateCollection clientSoftwareCertificates = GetSoftwareCertificates();

            Utils.LogInfo("Session REPLACING channel for {0}.", SessionId);

            if (connection != null)
            {
                ITransportChannel channel = NullableTransportChannel;

                // check if the channel supports reconnect.
                if (channel != null && (channel.SupportedFeatures & TransportChannelFeatures.Reconnect) != 0)
                {
                    channel.Reconnect(connection);
                }
                else
                {
                    // initialize the channel which will be created with the server.
                    channel = SessionChannel.Create(
                        configuration_,
                        connection,
                        ConfiguredEndpoint.Description,
                        ConfiguredEndpoint.Configuration,
                        instanceCertificate_,
                        configuration_.SecurityConfiguration.SendCertificateChain ? instanceCertificateChain_ : null,
                        MessageContext);

                    // disposes the existing channel.
                    TransportChannel = channel;
                }
            }
            else if (transportChannel != null)
            {
                TransportChannel = transportChannel;
            }
            else
            {
                ITransportChannel channel = NullableTransportChannel;

                // check if the channel supports reconnect.
                if (channel != null && (channel.SupportedFeatures & TransportChannelFeatures.Reconnect) != 0)
                {
                    channel.Reconnect();
                }
                else
                {
                    // initialize the channel which will be created with the server.
                    channel = SessionChannel.Create(
                        configuration_,
                        ConfiguredEndpoint.Description,
                        ConfiguredEndpoint.Configuration,
                        instanceCertificate_,
                        configuration_.SecurityConfiguration.SendCertificateChain ? instanceCertificateChain_ : null,
                        MessageContext);

                    // disposes the existing channel.
                    TransportChannel = channel;
                }
            }

            Utils.LogInfo("Session RE-ACTIVATING {0}.", SessionId);

            RequestHeader header = new RequestHeader() { TimeoutHint = ReconnectTimeout };
            return BeginActivateSession(
                header,
                clientSignature,
                null,
                preferredLocales_,
                new ExtensionObject(identityToken),
                userTokenSignature,
                null,
                null);
        }

        /// <summary>
        /// Process Republish error response.
        /// </summary>
        /// <param name="e">The exception that occurred during the republish operation.</param>
        /// <param name="subscriptionId">The subscription Id for which the republish was requested. </param>
        /// <param name="sequenceNumber">The sequencenumber for which the republish was requested.</param>
        private (bool, ServiceResult) ProcessRepublishResponseError(Exception e, uint subscriptionId, uint sequenceNumber)
        {

            ServiceResult error = new ServiceResult(e);

            bool result = true;
            switch (error.StatusCode.Code)
            {
                case StatusCodes.BadSubscriptionIdInvalid:
                case StatusCodes.BadMessageNotAvailable:
                    Utils.LogWarning("Message {0}-{1} no longer available.", subscriptionId, sequenceNumber);
                    break;

                // if encoding limits are exceeded, the issue is logged and
                // the published data is acknowledged to prevent the endless republish loop.
                case StatusCodes.BadEncodingLimitsExceeded:
                    Utils.LogError(e, "Message {0}-{1} exceeded size limits, ignored.", subscriptionId, sequenceNumber);
                    lock (acknowledgementsToSendLock_)
                    {
                        AddAcknowledgementToSend(acknowledgementsToSend_, subscriptionId, sequenceNumber);
                    }
                    break;

                default:
                    result = false;
                    Utils.LogError(e, "Unexpected error sending republish request.");
                    break;
            }

            EventHandler<SessionPublishErrorEventArgs> callback = PublishErrorEventHandler;

            // raise an error event.
            if (callback != null)
            {
                try
                {
                    SessionPublishErrorEventArgs args = new SessionPublishErrorEventArgs(
                        error,
                        subscriptionId,
                        sequenceNumber);

                    callback(this, args);
                }
                catch (Exception e2)
                {
                    Utils.LogError(e2, "Session: Unexpected error invoking PublishErrorCallback.");
                }
            }

            return (result, error);
        }

        /// <summary>
        /// If available, returns the current nonce or null.
        /// </summary>
        private byte[] GetCurrentTokenServerNonce()
        {
            var currentToken = NullableTransportChannel?.CurrentToken;
            return currentToken?.ServerNonce;
        }

        /// <summary>
        /// Handles the validation of server software certificates and application callback.
        /// </summary>
        private void HandleSignedSoftwareCertificates(SignedSoftwareCertificateCollection serverSoftwareCertificates)
        {
            // get a validator to check certificates provided by server.
            CertificateValidator validator = configuration_.CertificateValidator;

            // validate software certificates.
            List<SoftwareCertificate> softwareCertificates = new List<SoftwareCertificate>();

            foreach (SignedSoftwareCertificate signedCertificate in serverSoftwareCertificates)
            {
                SoftwareCertificate softwareCertificate = null;

                ServiceResult result = SoftwareCertificate.Validate(
                    validator,
                    signedCertificate.CertificateData,
                    out softwareCertificate);

                if (ServiceResult.IsBad(result))
                {
                    OnSoftwareCertificateError(signedCertificate, result);
                }

                softwareCertificates.Add(softwareCertificate);
            }

            // check if software certificates meet application requirements.
            ValidateSoftwareCertificates(softwareCertificates);
        }

        /// <summary>
        /// Processes the response from a publish request.
        /// </summary>
        private void ProcessPublishResponse(
            ResponseHeader responseHeader,
            uint subscriptionId,
            UInt32Collection availableSequenceNumbers,
            bool moreNotifications,
            NotificationMessage notificationMessage)
        {
            Subscription subscription = null;

            // send notification that the server is alive.
            OnKeepAlive(serverState_, responseHeader.Timestamp);

            // collect the current set of acknowledgements.
            lock (acknowledgementsToSendLock_)
            {
                // clear out acknowledgements for messages that the server does not have any more.
                var acknowledgementsToSend = new SubscriptionAcknowledgementCollection();

                uint latestSequenceNumberToSend = 0;

                // create an acknowledgement to be sent back to the server.
                if (notificationMessage.NotificationData.Count > 0)
                {
                    AddAcknowledgementToSend(acknowledgementsToSend, subscriptionId, notificationMessage.SequenceNumber);
                    UpdateLatestSequenceNumberToSend(ref latestSequenceNumberToSend, notificationMessage.SequenceNumber);
                    _ = availableSequenceNumbers?.Remove(notificationMessage.SequenceNumber);
                }

                for (int ii = 0; ii < acknowledgementsToSend_.Count; ii++)
                {
                    SubscriptionAcknowledgement acknowledgement = acknowledgementsToSend_[ii];

                    if (acknowledgement.SubscriptionId != subscriptionId)
                    {
                        acknowledgementsToSend.Add(acknowledgement);
                    }
                    else if (availableSequenceNumbers == null || availableSequenceNumbers.Remove(acknowledgement.SequenceNumber))
                    {
                        acknowledgementsToSend.Add(acknowledgement);
                        UpdateLatestSequenceNumberToSend(ref latestSequenceNumberToSend, acknowledgement.SequenceNumber);
                    }
                    // a publish response may by processed out of order,
                    // allow for a tolerance until the sequence number is removed.
                    else if (Math.Abs((int)(acknowledgement.SequenceNumber - latestSequenceNumberToSend)) < kPublishRequestSequenceNumberOutOfOrderThreshold)
                    {
                        acknowledgementsToSend.Add(acknowledgement);
                    }
                    else
                    {
                        Utils.LogWarning("SessionId {0}, SubscriptionId {1}, Sequence number={2} was not received in the available sequence numbers.", SessionId, subscriptionId, acknowledgement.SequenceNumber);
                    }
                }

                // Check for outdated sequence numbers. May have been not acked due to a network glitch. 
                if (latestSequenceNumberToSend != 0 && availableSequenceNumbers?.Count > 0)
                {
                    foreach (var sequenceNumber in availableSequenceNumbers)
                    {
                        if ((int)(latestSequenceNumberToSend - sequenceNumber) > kPublishRequestSequenceNumberOutdatedThreshold)
                        {
                            AddAcknowledgementToSend(acknowledgementsToSend, subscriptionId, sequenceNumber);
                            Utils.LogWarning("SessionId {0}, SubscriptionId {1}, Sequence number={2} was outdated, acknowledged.", SessionId, subscriptionId, sequenceNumber);
                        }
                    }
                }

#if DEBUG_SEQUENTIALPUBLISHING
                // Checks for debug info only.
                // Once more than a single publish request is queued, the checks are invalid
                // because a publish response may not include the latest ack information yet.

                uint lastSentSequenceNumber;
                if (availableSequenceNumbers != null)
                {
                    foreach (var availableSequenceNumber in availableSequenceNumbers)
                    {
                        if (latestAcknowledgementsSent_.ContainsKey(subscriptionId))
                        {
                            lastSentSequenceNumber = latestAcknowledgementsSent_[subscriptionId];

                            // If the last sent sequence number is uint.Max do not display the warning; the counter rolled over
                            // If the last sent sequence number is greater or equal to the available sequence number (returned by the publish),
                            // a warning must be logged.
                            if (((lastSentSequenceNumber >= availableSequenceNumber) && (lastSentSequenceNumber != uint.MaxValue)) ||
                                (lastSentSequenceNumber == availableSequenceNumber) && (lastSentSequenceNumber == uint.MaxValue))
                            {
                                Utils.LogWarning("Received sequence number which was already acknowledged={0}", availableSequenceNumber);
                            }
                        }
                    }
                }

                if (latestAcknowledgementsSent_.ContainsKey(subscriptionId))
                {
                    lastSentSequenceNumber = latestAcknowledgementsSent_[subscriptionId];

                    // If the last sent sequence number is uint.Max do not display the warning; the counter rolled over
                    // If the last sent sequence number is greater or equal to the notificationMessage's sequence number (returned by the publish),
                    // a warning must be logged.
                    if (((lastSentSequenceNumber >= notificationMessage.SequenceNumber) && (lastSentSequenceNumber != uint.MaxValue)) || (lastSentSequenceNumber == notificationMessage.SequenceNumber) && (lastSentSequenceNumber == uint.MaxValue))
                    {
                        Utils.LogWarning("Received sequence number which was already acknowledged={0}", notificationMessage.SequenceNumber);
                    }
                }
#endif

                acknowledgementsToSend_ = acknowledgementsToSend;

                if (notificationMessage.IsEmpty)
                {
                    Utils.LogTrace("Empty notification message received for SessionId {0} with PublishTime {1}", SessionId, notificationMessage.PublishTime.ToLocalTime());
                }
            }

            lock (SyncRoot)
            {
                // find the subscription.
                foreach (var current in subscriptions_)
                {
                    if (current.Id == subscriptionId)
                    {
                        subscription = current;
                        break;
                    }
                }
            }

            // ignore messages with a subscription that has been deleted.
            if (subscription != null)
            {
                // Validate publish time and reject old values.
                if (notificationMessage.PublishTime.AddMilliseconds(subscription.CurrentPublishingInterval * subscription.CurrentLifetimeCount) < DateTime.UtcNow)
                {
                    Utils.LogTrace("PublishTime {0} in publish response is too old for SubscriptionId {1}.", notificationMessage.PublishTime.ToLocalTime(), subscription.Id);
                }

                // Validate publish time and reject old values.
                if (notificationMessage.PublishTime > DateTime.UtcNow.AddMilliseconds(subscription.CurrentPublishingInterval * subscription.CurrentLifetimeCount))
                {
                    Utils.LogTrace("PublishTime {0} in publish response is newer than actual time for SubscriptionId {1}.", notificationMessage.PublishTime.ToLocalTime(), subscription.Id);
                }

                // update subscription cache.
                subscription.SaveMessageInCache(
                    availableSequenceNumbers,
                    notificationMessage,
                    responseHeader.StringTable);

                // raise the notification.
                EventHandler<SessionNotificationEventArgs> publishEventHandler = PublishEventHandler;
                if (publishEventHandler != null)
                {
                    var args = new SessionNotificationEventArgs(subscription, notificationMessage, responseHeader.StringTable);

                    Task.Run(() => {
                        OnRaisePublishNotification(publishEventHandler, args);
                    });
                }
            }
            else
            {
                if (DeleteSubscriptionsOnClose && !reconnecting_)
                {
                    // Delete abandoned subscription from server.
                    Utils.LogWarning("Received Publish Response for Unknown SubscriptionId={0}. Deleting abandoned subscription from server.", subscriptionId);

                    Task.Run(() => {
                        DeleteSubscription(subscriptionId);
                    });
                }
                else
                {
                    // Do not delete publish requests of stale subscriptions
                    Utils.LogWarning("Received Publish Response for Unknown SubscriptionId={0}. Ignored.", subscriptionId);
                }
            }
        }

        /// <summary>
        /// Recreate the subscriptions in a reconnected session.
        /// Uses Transfer service if <see cref="TransferSubscriptionsOnReconnect"/> is set to <c>true</c>.
        /// </summary>
        /// <param name="subscriptionsTemplate">The template for the subscriptions.</param>
        private void RecreateSubscriptions(IEnumerable<Subscription> subscriptionsTemplate)
        {
            bool transferred = false;
            if (TransferSubscriptionsOnReconnect)
            {
                try
                {
                    transferred = TransferSubscriptions(new SubscriptionCollection(subscriptionsTemplate), false);
                }
                catch (ServiceResultException sre)
                {
                    if (sre.StatusCode == StatusCodes.BadServiceUnsupported)
                    {
                        TransferSubscriptionsOnReconnect = false;
                        Utils.LogWarning("Transfer subscription unsupported, TransferSubscriptionsOnReconnect set to false.");
                    }
                    else
                    {
                        Utils.LogError(sre, "Transfer subscriptions failed.");
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogError(ex, "Unexpected Transfer subscriptions error.");
                }
            }

            if (!transferred)
            {
                // Create the subscriptions which were not transferred.
                foreach (Subscription subscription in Subscriptions)
                {
                    if (!subscription.Created)
                    {
                        subscription.Create();
                    }
                }
            }
        }

        /// <summary>
        /// Raises an event indicating that publish has returned a notification.
        /// </summary>
        private void OnRaisePublishNotification(EventHandler<SessionNotificationEventArgs> callback, SessionNotificationEventArgs args)
        {
            try
            {
                if (callback != null && args.Subscription.Id != 0)
                {
                    callback(this, args);
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Session: Unexpected error while raising Notification event.");
            }
        }

        /// <summary>
        /// Invokes a DeleteSubscriptions call for the specified subscriptionId.
        /// </summary>
        private void DeleteSubscription(uint subscriptionId)
        {
            try
            {
                Utils.LogInfo("Deleting server subscription for SubscriptionId={0}", subscriptionId);

                // delete the subscription.
                UInt32Collection subscriptionIds = new[] { subscriptionId };

                var responseHeader = DeleteSubscriptions(
                    null,
                    subscriptionIds,
                    out var results,
                    out var diagnosticInfos);

                // validate response.
                ClientBase.ValidateResponse(results, subscriptionIds);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, subscriptionIds);

                if (StatusCode.IsBad(results[0]))
                {
                    throw new ServiceResultException(ClientBase.GetResult(results[0], 0, diagnosticInfos, responseHeader));
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Session: Unexpected error while deleting subscription for SubscriptionId={0}.", subscriptionId);
            }
        }

        /// <summary>
        /// Load certificate for connection.
        /// </summary>
        private static async Task<X509Certificate2> LoadCertificateAsync(ApplicationConfiguration configuration)
        {
            if (configuration.SecurityConfiguration.ApplicationCertificate == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "ApplicationCertificate must be specified.");
            }

            var clientCertificate = await configuration.SecurityConfiguration.ApplicationCertificate.Find(true).ConfigureAwait(false);

            if (clientCertificate == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "ApplicationCertificate cannot be found.");
            }
            return clientCertificate;
        }

        /// <summary>
        /// Load certificate chain for connection.
        /// </summary>
        private static async Task<X509Certificate2Collection> LoadCertificateChainAsync(ApplicationConfiguration configuration, X509Certificate2 clientCertificate)
        {
            X509Certificate2Collection clientCertificateChain = null;
            // load certificate chain.
            if (configuration.SecurityConfiguration.SendCertificateChain)
            {
                clientCertificateChain = new X509Certificate2Collection(clientCertificate);
                var issuers = new List<CertificateIdentifier>();
                await configuration.CertificateValidator.GetIssuers(clientCertificate, issuers).ConfigureAwait(false);

                foreach (var issuer in issuers)
                {
                    clientCertificateChain.Add(issuer.Certificate);
                }
            }
            return clientCertificateChain;
        }

        /// <summary>
        /// Helper to determine if a continuation point needs to be processed.
        /// </summary>
        private bool HasAnyContinuationPoint(ByteStringCollection continuationPoints)
        {
            foreach (byte[] cp in continuationPoints)
            {
                if (cp != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddAcknowledgementToSend(SubscriptionAcknowledgementCollection acknowledgementsToSend, uint subscriptionId, uint sequenceNumber)
        {
            if (acknowledgementsToSend == null) throw new ArgumentNullException(nameof(acknowledgementsToSend));

            Debug.Assert(Monitor.IsEntered(acknowledgementsToSendLock_));

            SubscriptionAcknowledgement acknowledgement = new SubscriptionAcknowledgement {
                SubscriptionId = subscriptionId,
                SequenceNumber = sequenceNumber
            };

            acknowledgementsToSend.Add(acknowledgement);
        }

        /// <summary>
        /// Returns true if the Bad_TooManyPublishRequests limit
        /// has not been reached.
        /// </summary>
        /// <param name="requestCount">The actual number of publish requests.</param>
        /// <returns>If the publish request limit was reached.</returns>
        private bool BelowPublishRequestLimit(int requestCount)
        {
            return (tooManyPublishRequests_ == 0) ||
                (requestCount < tooManyPublishRequests_);
        }

        /// <summary>
        /// Returns the minimum number of active publish request that should be used.
        /// </summary>
        /// <remarks>
        /// Returns 0 if there are no subscriptions.
        /// </remarks>
        private int GetMinPublishRequestCount(bool createdOnly)
        {
            lock (SyncRoot)
            {
                if (subscriptions_.Count == 0)
                {
                    return 0;
                }

                if (createdOnly)
                {
                    int count = 0;
                    foreach (Subscription subscription in subscriptions_)
                    {
                        if (subscription.Created)
                        {
                            count++;
                        }
                    }

                    if (count == 0)
                    {
                        return 0;
                    }

                    return Math.Max(count, minPublishRequestCount_);
                }

                return Math.Max(subscriptions_.Count, minPublishRequestCount_);
            }
        }

        /// <summary>
        /// Creates resend data call requests for the subscriptions.
        /// </summary>
        /// <param name="subscriptions">The subscriptions to call resend data.</param>
        private CallMethodRequestCollection CreateCallRequestsForResendData(IEnumerable<Subscription> subscriptions)
        {
            CallMethodRequestCollection requests = new CallMethodRequestCollection();

            foreach (Subscription subscription in subscriptions)
            {
                VariantCollection inputArguments = new VariantCollection {
                    new Variant(subscription.Id)
                };

                var request = new CallMethodRequest {
                    ObjectId = ObjectIds.Server,
                    MethodId = MethodIds.Server_ResendData,
                    InputArguments = inputArguments
                };

                requests.Add(request);
            }
            return requests;
        }

        /// <summary>
        /// Creates and validates the subscription ids for a transfer.
        /// </summary>
        /// <param name="subscriptions">The subscriptions to transfer.</param>
        /// <returns>The subscription ids for the transfer.</returns>
        /// <exception cref="ServiceResultException">Thrown if a subscription is in invalid state.</exception>
        private UInt32Collection CreateSubscriptionIdsForTransfer(SubscriptionCollection subscriptions)
        {
            var subscriptionIds = new UInt32Collection();
            lock (SyncRoot)
            {
                foreach (var subscription in subscriptions)
                {
                    if (subscription.Created && SessionId.Equals(subscription.Session.SessionId))
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidState, Utils.Format("The subscriptionId {0} is already created.", subscription.Id));
                    }
                    if (subscription.TransferId == 0)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidState, Utils.Format("A subscription can not be transferred due to missing transfer Id."));
                    }
                    subscriptionIds.Add(subscription.TransferId);
                }
            }
            return subscriptionIds;
        }

        /// <summary>
        /// Indicates that the session configuration has changed.
        /// </summary>
        private void IndicateSessionConfigurationChanged()
        {
            try
            {
                SessionConfigurationChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error calling SessionConfigurationChangedEventHandler.");
            }
        }

        /// <summary>
        /// Helper to update the latest sequence number to send.
        /// Handles wrap around of sequence numbers.
        /// </summary>
        private static void UpdateLatestSequenceNumberToSend(ref uint latestSequenceNumberToSend, uint sequenceNumber)
        {
            // Handle wrap around with subtraction and test result is int.
            // Assume sequence numbers to ack do not differ by more than uint.Max / 2
            if (latestSequenceNumberToSend == 0 || ((int)(sequenceNumber - latestSequenceNumberToSend)) > 0)
            {
                latestSequenceNumberToSend = sequenceNumber;
            }
        }
        #endregion

        #region Protected Fields
        /// <summary>
        /// The period for which the server will maintain the session if there is no communication from the client.
        /// </summary>
        protected double sessionTimeout_;

        /// <summary>
        /// If set to<c>true</c> then the domain in the certificate must match the endpoint used.
        /// </summary>
        private bool checkDomain_;

        /// <summary>
        /// The Instance Certificate Chain.
        /// </summary>
        private X509Certificate2Collection instanceCertificateChain_;

        /// <summary>
        /// The Instance Certificate.
        /// </summary>
        private X509Certificate2 instanceCertificate_;

        /// <summary>
        /// The locales that the server should use when returning localized text.
        /// </summary>
        private StringCollection preferredLocales_;

        /// <summary>
        /// The Application Configuration.
        /// </summary>
        private ApplicationConfiguration configuration_;

        private SubscriptionAcknowledgementCollection acknowledgementsToSend_;
        private object acknowledgementsToSendLock_;
#if DEBUG_SEQUENTIALPUBLISHING
        private Dictionary<uint, uint> latestAcknowledgementsSent_;
#endif
        private List<Subscription> subscriptions_;
        private Dictionary<NodeId, DataDictionary> dictionaries_;
        private bool transferSubscriptionsOnReconnect_;
        private uint maxRequestMessageSize_;
        private SystemContext systemContext_;
        private List<IUserIdentity> identityHistory_;

        private byte[] serverNonce_;
        private byte[] previousServerNonce_;
        private X509Certificate2 serverCertificate_;
        private long publishCounter_;
        private int tooManyPublishRequests_;
        private long lastKeepAliveTime_;
        private ServerState serverState_;
        private int keepAliveInterval_;
#if NET6_0_OR_GREATER
        private PeriodicTimer keepAliveTimer_;
#else
        private Timer keepAliveTimer_;
#endif
        private long keepAliveCounter_;
        private bool reconnecting_;
        private SemaphoreSlim reconnectLock_;
        private int minPublishRequestCount_;
        private LinkedList<AsyncRequestState> outstandingRequests_;
        private readonly EndpointDescriptionCollection discoveryServerEndpoints_;
        private readonly StringCollection discoveryProfileUris_;

        private class AsyncRequestState
        {
            public uint RequestTypeId;
            public uint RequestId;
            public DateTime Timestamp;
            public IAsyncResult Result;
            public bool Defunct;
        }

        private readonly object eventLock_ = new object();
        private event EventHandler<SessionKeepAliveEventArgs> KeepAliveEventHandler;
        private event EventHandler<SessionNotificationEventArgs> PublishEventHandler;
        private event EventHandler<SessionPublishErrorEventArgs> PublishErrorEventHandler;
        private event EventHandler<PublishSequenceNumbersToAcknowledgeEventArgs> PublishSequenceNumbersToAcknowledgeEventHandler;
        private event EventHandler SubscriptionsChangedEventHandler;
        private event EventHandler SessionClosingEventHandler;
        private event EventHandler SessionConfigurationChangedEventHandler;
        private event RenewUserIdentity RenewUserIdentityEventHandler;
        #endregion
    }
}
