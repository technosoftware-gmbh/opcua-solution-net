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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;

using Opc.Ua;
#endregion

namespace Technosoftware.UaConfiguration
{
    /// <summary>
    /// Manages a configuration for an OPC UA application.
    /// </summary>
    public class ApplicationConfigurationManager :
        IUaApplicationConfigurationManager
    {
        #region ctor
        /// <summary>
        /// Create the application configuration manager.
        /// </summary>
        public ApplicationConfigurationManager(ApplicationInstance applicationInstance)
        {
            ApplicationInstance = applicationInstance;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The application instance used to build the configuration.
        /// </summary>
        public ApplicationInstance ApplicationInstance { get; private set; }
        /// <summary>
        /// The application configuration.
        /// </summary>
        public ApplicationConfiguration ApplicationConfiguration => ApplicationInstance.ApplicationConfiguration;
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public IUaApplicationConfigurationClientSelected AsClient()
        {
            switch (ApplicationInstance.ApplicationType)
            {
                case ApplicationType.Client:
                case ApplicationType.ClientAndServer:
                    break;
                case ApplicationType.Server:
                    ApplicationInstance.ApplicationType =
                        typeSelected_ ? ApplicationType.ClientAndServer : ApplicationType.Client;
                    break;
                default:
                    throw new ArgumentException("Invalid application type for client.");
            }

            typeSelected_ = true;

            ApplicationConfiguration.ClientConfiguration = new ClientConfiguration();

            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions AddSecurityConfiguration(
            string subjectName,
            string pkiRoot = null,
            string appRoot = null,
            string rejectedRoot = null
            )
        {
            pkiRoot = DefaultPKIRoot(pkiRoot);
            appRoot = appRoot == null ? pkiRoot : DefaultPKIRoot(appRoot);
            rejectedRoot = rejectedRoot == null ? pkiRoot : DefaultPKIRoot(rejectedRoot);
            var appStoreType = CertificateStoreIdentifier.DetermineStoreType(appRoot);
            var pkiRootType = CertificateStoreIdentifier.DetermineStoreType(pkiRoot);
            var rejectedRootType = CertificateStoreIdentifier.DetermineStoreType(rejectedRoot);
            ApplicationConfiguration.SecurityConfiguration = new SecurityConfiguration {
                // app cert store
                ApplicationCertificate = new CertificateIdentifier() {
                    StoreType = appStoreType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Application, appRoot),
                    SubjectName = Utils.ReplaceDCLocalhost(subjectName)
                },
                // App trusted & issuer
                TrustedPeerCertificates = new CertificateTrustList() {
                    StoreType = pkiRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Trusted, pkiRoot)
                },
                TrustedIssuerCertificates = new CertificateTrustList() {
                    StoreType = pkiRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Issuer, pkiRoot)
                },
                // Https trusted & issuer
                TrustedHttpsCertificates = new CertificateTrustList() {
                    StoreType = pkiRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.TrustedHttps, pkiRoot)
                },
                HttpsIssuerCertificates = new CertificateTrustList() {
                    StoreType = pkiRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.IssuerHttps, pkiRoot)
                },
                // User trusted & issuer
                TrustedUserCertificates = new CertificateTrustList() {
                    StoreType = pkiRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.TrustedUser, pkiRoot)
                },
                UserIssuerCertificates = new CertificateTrustList() {
                    StoreType = pkiRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.IssuerUser, pkiRoot)
                },
                // rejected store
                RejectedCertificateStore = new CertificateTrustList() {
                    StoreType = rejectedRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Rejected, rejectedRoot)
                },
            };
            SetSecureDefaults(ApplicationConfiguration.SecurityConfiguration);

            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptionStores AddSecurityConfigurationStores(
            string subjectName,
            string appRoot,
            string trustedRoot,
            string issuerRoot,
            string rejectedRoot = null
            )
        {
            string appStoreType = CertificateStoreIdentifier.DetermineStoreType(appRoot);
            string issuerRootType = CertificateStoreIdentifier.DetermineStoreType(issuerRoot);
            string trustedRootType = CertificateStoreIdentifier.DetermineStoreType(trustedRoot);
            rejectedRoot = rejectedRoot ?? DefaultPKIRoot(null);
            string rejectedRootType = CertificateStoreIdentifier.DetermineStoreType(rejectedRoot);
            ApplicationConfiguration.SecurityConfiguration = new SecurityConfiguration {
                // app cert store
                ApplicationCertificate = new CertificateIdentifier() {
                    StoreType = appStoreType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Application, appRoot),
                    SubjectName = Utils.ReplaceDCLocalhost(subjectName)
                },
                // App trusted & issuer
                TrustedPeerCertificates = new CertificateTrustList() {
                    StoreType = trustedRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Trusted, trustedRoot)
                },
                TrustedIssuerCertificates = new CertificateTrustList() {
                    StoreType = issuerRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Issuer, issuerRoot)
                },
                // rejected store
                RejectedCertificateStore = new CertificateTrustList() {
                    StoreType = rejectedRootType,
                    StorePath = DefaultCertificateStorePath(TrustlistType.Rejected, rejectedRoot)
                },
            };
            SetSecureDefaults(ApplicationConfiguration.SecurityConfiguration);

            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptionStores AddSecurityConfigurationUserStore(
            string trustedRoot,
            string issuerRoot
            )
        {
            string trustedRootType = CertificateStoreIdentifier.DetermineStoreType(trustedRoot);
            string issuerRootType = CertificateStoreIdentifier.DetermineStoreType(issuerRoot);

            // User trusted & issuer
            ApplicationConfiguration.SecurityConfiguration.TrustedUserCertificates = new CertificateTrustList() {
                StoreType = trustedRootType,
                StorePath = DefaultCertificateStorePath(TrustlistType.TrustedUser, trustedRoot)
            };
            ApplicationConfiguration.SecurityConfiguration.UserIssuerCertificates = new CertificateTrustList() {
                StoreType = issuerRootType,
                StorePath = DefaultCertificateStorePath(TrustlistType.IssuerUser, issuerRoot)
            };
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptionStores AddSecurityConfigurationHttpsStore(
            string trustedRoot,
            string issuerRoot
            )
        {
            string trustedRootType = CertificateStoreIdentifier.DetermineStoreType(trustedRoot);
            string issuerRootType = CertificateStoreIdentifier.DetermineStoreType(issuerRoot);

            // Https trusted & issuer
            ApplicationConfiguration.SecurityConfiguration.TrustedHttpsCertificates = new CertificateTrustList() {
                StoreType = trustedRootType,
                StorePath = DefaultCertificateStorePath(TrustlistType.TrustedHttps, trustedRootType)
            };
            ApplicationConfiguration.SecurityConfiguration.HttpsIssuerCertificates = new CertificateTrustList() {
                StoreType = issuerRootType,
                StorePath = DefaultCertificateStorePath(TrustlistType.IssuerHttps, issuerRoot)
            };
            return this;
        }

        /// <inheritdoc/>
        public async Task<ApplicationConfiguration> CreateAsync()
        {
            // sanity checks
            if (ApplicationInstance.ApplicationType == ApplicationType.Server ||
                ApplicationInstance.ApplicationType == ApplicationType.ClientAndServer)
            {
                if (ApplicationConfiguration.ServerConfiguration == null) throw new ArgumentException("ApplicationType Server is not configured.");
            }
            if (ApplicationInstance.ApplicationType == ApplicationType.Client ||
                ApplicationInstance.ApplicationType == ApplicationType.ClientAndServer)
            {
                if (ApplicationConfiguration.ClientConfiguration == null) throw new ArgumentException("ApplicationType Client is not configured.");
            }

            // ensure for a user token policy
            if (ApplicationConfiguration.ServerConfiguration?.UserTokenPolicies.Count == 0)
            {
                ApplicationConfiguration.ServerConfiguration.UserTokenPolicies.Add(new UserTokenPolicy(UserTokenType.Anonymous));
            }

            // ensure for secure transport profiles
            if (ApplicationConfiguration.ServerConfiguration?.SecurityPolicies.Count == 0)
            {
                AddSecurityPolicies();
            }

            ApplicationConfiguration.TraceConfiguration?.ApplySettings();

            await ApplicationConfiguration.Validate(ApplicationInstance.ApplicationType).ConfigureAwait(false);

            await ApplicationConfiguration.CertificateValidator.
                Update(ApplicationConfiguration.SecurityConfiguration).ConfigureAwait(false);

            return ApplicationConfiguration;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AsServer(
            string[] baseAddresses,
            string[] alternateBaseAddresses = null)
        {
            switch (ApplicationInstance.ApplicationType)
            {
                case ApplicationType.Client:
                    ApplicationInstance.ApplicationType =
                        typeSelected_ ? ApplicationType.ClientAndServer : ApplicationType.Server;
                    break;
                case ApplicationType.Server:
                case ApplicationType.ClientAndServer: break;
                default:
                    throw new ArgumentException("Invalid application type for server.");
            }

            typeSelected_ = true;

            // configure a server
            var serverConfiguration = new ServerConfiguration();

            // by default disable LDS registration
            serverConfiguration.MaxRegistrationInterval = 0;

            // base addresses
            foreach (var baseAddress in baseAddresses)
            {
                serverConfiguration.BaseAddresses.Add(Utils.ReplaceLocalhost(baseAddress));
            }

            // alternate base addresses
            if (alternateBaseAddresses != null)
            {
                foreach (var alternateBaseAddress in alternateBaseAddresses)
                {
                    serverConfiguration.AlternateBaseAddresses.Add(Utils.ReplaceLocalhost(alternateBaseAddress));
                }
            }

            // add container for policies
            serverConfiguration.SecurityPolicies = new ServerSecurityPolicyCollection();

            // add user token policy container and Anonymous
            serverConfiguration.UserTokenPolicies = new UserTokenPolicyCollection();

            ApplicationConfiguration.ServerConfiguration = serverConfiguration;

            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AddUnsecurePolicyNone(bool addPolicy = true)
        {
            if (addPolicy)
            {
                var policies = ApplicationConfiguration.ServerConfiguration.SecurityPolicies;
                InternalAddPolicy(policies, MessageSecurityMode.None, SecurityPolicies.None);
            }
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AddSignPolicies(bool addPolicies = true)
        {
            if (addPolicies)
            {
                AddSecurityPolicies(true, false, false);
            }
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AddSignAndEncryptPolicies(bool addPolicies = true)
        {
            if (addPolicies)
            {
                AddSecurityPolicies(false, false, false);
            }
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AddPolicy(MessageSecurityMode securityMode, string securityPolicy)
        {
            if (SecurityPolicies.GetDisplayName(securityPolicy) == null) throw new ArgumentException("Unknown security policy", nameof(securityPolicy));
            if (securityMode == MessageSecurityMode.None || securityPolicy.Equals(SecurityPolicies.None)) throw new ArgumentException("Use AddUnsecurePolicyNone to add no security policy.");
            InternalAddPolicy(ApplicationConfiguration.ServerConfiguration.SecurityPolicies, securityMode, securityPolicy);
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AddUserTokenPolicy(UserTokenType userTokenType)
        {
            ApplicationConfiguration.ServerConfiguration.UserTokenPolicies.Add(new UserTokenPolicy(userTokenType));
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerSelected AddUserTokenPolicy(UserTokenPolicy userTokenPolicy)
        {
            if (userTokenPolicy == null) throw new ArgumentNullException(nameof(userTokenPolicy));
            ApplicationConfiguration.ServerConfiguration.UserTokenPolicies.Add(userTokenPolicy);
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetAutoAcceptUntrustedCertificates(bool autoAccept)
        {
            ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates = autoAccept;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetAddAppCertToTrustedStore(bool addToTrustedStore)
        {
            ApplicationConfiguration.SecurityConfiguration.AddAppCertToTrustedStore = addToTrustedStore;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetRejectSHA1SignedCertificates(bool rejectSHA1Signed)
        {
            ApplicationConfiguration.SecurityConfiguration.RejectSHA1SignedCertificates = rejectSHA1Signed;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetRejectUnknownRevocationStatus(bool rejectUnknownRevocationStatus)
        {
            ApplicationConfiguration.SecurityConfiguration.RejectUnknownRevocationStatus = rejectUnknownRevocationStatus;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetUseValidatedCertificates(bool useValidatedCertificates)
        {
            ApplicationConfiguration.SecurityConfiguration.UseValidatedCertificates = useValidatedCertificates;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetSuppressNonceValidationErrors(bool suppressNonceValidationErrors)
        {
            ApplicationConfiguration.SecurityConfiguration.SuppressNonceValidationErrors = suppressNonceValidationErrors;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetSendCertificateChain(bool sendCertificateChain)
        {
            ApplicationConfiguration.SecurityConfiguration.SendCertificateChain = sendCertificateChain;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions SetMinimumCertificateKeySize(ushort keySize)
        {
            ApplicationConfiguration.SecurityConfiguration.MinimumCertificateKeySize = keySize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationSecurityOptions AddCertificatePasswordProvider(ICertificatePasswordProvider certificatePasswordProvider)
        {
            ApplicationConfiguration.SecurityConfiguration.CertificatePasswordProvider = certificatePasswordProvider;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotasSet SetTransportQuotas(TransportQuotas transportQuotas)
        {
            ApplicationConfiguration.TransportQuotas = transportQuotas;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetOperationTimeout(int operationTimeout)
        {
            ApplicationConfiguration.TransportQuotas.OperationTimeout = operationTimeout;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetMaxStringLength(int maxStringLength)
        {
            ApplicationConfiguration.TransportQuotas.MaxStringLength = maxStringLength;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetMaxByteStringLength(int maxByteStringLength)
        {
            ApplicationConfiguration.TransportQuotas.MaxByteStringLength = maxByteStringLength;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetMaxArrayLength(int maxArrayLength)
        {
            ApplicationConfiguration.TransportQuotas.MaxArrayLength = maxArrayLength;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetMaxMessageSize(int maxMessageSize)
        {
            ApplicationConfiguration.TransportQuotas.MaxMessageSize = maxMessageSize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetMaxBufferSize(int maxBufferSize)
        {
            ApplicationConfiguration.TransportQuotas.MaxBufferSize = maxBufferSize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetChannelLifetime(int channelLifetime)
        {
            ApplicationConfiguration.TransportQuotas.ChannelLifetime = channelLifetime;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTransportQuotas SetSecurityTokenLifetime(int securityTokenLifetime)
        {
            ApplicationConfiguration.TransportQuotas.SecurityTokenLifetime = securityTokenLifetime;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMinRequestThreadCount(int minRequestThreadCount)
        {
            ApplicationConfiguration.ServerConfiguration.MinRequestThreadCount = minRequestThreadCount;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxRequestThreadCount(int maxRequestThreadCount)
        {
            ApplicationConfiguration.ServerConfiguration.MaxRequestThreadCount = maxRequestThreadCount;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxQueuedRequestCount(int maxQueuedRequestCount)
        {
            ApplicationConfiguration.ServerConfiguration.MaxQueuedRequestCount = maxQueuedRequestCount;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetDiagnosticsEnabled(bool diagnosticsEnabled)
        {
            ApplicationConfiguration.ServerConfiguration.DiagnosticsEnabled = diagnosticsEnabled;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxSessionCount(int maxSessionCount)
        {
            ApplicationConfiguration.ServerConfiguration.MaxSessionCount = maxSessionCount;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMinSessionTimeout(int minSessionTimeout)
        {
            ApplicationConfiguration.ServerConfiguration.MinSessionTimeout = minSessionTimeout;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxSessionTimeout(int maxSessionTimeout)
        {
            ApplicationConfiguration.ServerConfiguration.MaxSessionTimeout = maxSessionTimeout;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxBrowseContinuationPoints(int maxBrowseContinuationPoints)
        {
            ApplicationConfiguration.ServerConfiguration.MaxBrowseContinuationPoints = maxBrowseContinuationPoints;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxQueryContinuationPoints(int maxQueryContinuationPoints)
        {
            ApplicationConfiguration.ServerConfiguration.MaxQueryContinuationPoints = maxQueryContinuationPoints;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxHistoryContinuationPoints(int maxHistoryContinuationPoints)
        {
            ApplicationConfiguration.ServerConfiguration.MaxHistoryContinuationPoints = maxHistoryContinuationPoints;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxRequestAge(int maxRequestAge)
        {
            ApplicationConfiguration.ServerConfiguration.MaxRequestAge = maxRequestAge;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMinPublishingInterval(int minPublishingInterval)
        {
            ApplicationConfiguration.ServerConfiguration.MinPublishingInterval = minPublishingInterval;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxPublishingInterval(int maxPublishingInterval)
        {
            ApplicationConfiguration.ServerConfiguration.MaxPublishingInterval = maxPublishingInterval;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetPublishingResolution(int publishingResolution)
        {
            ApplicationConfiguration.ServerConfiguration.PublishingResolution = publishingResolution;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxSubscriptionLifetime(int maxSubscriptionLifetime)
        {
            ApplicationConfiguration.ServerConfiguration.MaxSubscriptionLifetime = maxSubscriptionLifetime;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxMessageQueueSize(int maxMessageQueueSize)
        {
            ApplicationConfiguration.ServerConfiguration.MaxMessageQueueSize = maxMessageQueueSize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxNotificationQueueSize(int maxNotificationQueueSize)
        {
            ApplicationConfiguration.ServerConfiguration.MaxNotificationQueueSize = maxNotificationQueueSize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxNotificationsPerPublish(int maxNotificationsPerPublish)
        {
            ApplicationConfiguration.ServerConfiguration.MaxNotificationsPerPublish = maxNotificationsPerPublish;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMinMetadataSamplingInterval(int minMetadataSamplingInterval)
        {
            ApplicationConfiguration.ServerConfiguration.MinMetadataSamplingInterval = minMetadataSamplingInterval;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetAvailableSamplingRates(SamplingRateGroupCollection availableSampleRates)
        {
            ApplicationConfiguration.ServerConfiguration.AvailableSamplingRates = availableSampleRates;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetRegistrationEndpoint(EndpointDescription registrationEndpoint)
        {
            ApplicationConfiguration.ServerConfiguration.RegistrationEndpoint = registrationEndpoint;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxRegistrationInterval(int maxRegistrationInterval)
        {
            ApplicationConfiguration.ServerConfiguration.MaxRegistrationInterval = maxRegistrationInterval;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetNodeManagerSaveFile(string nodeManagerSaveFile)
        {
            ApplicationConfiguration.ServerConfiguration.NodeManagerSaveFile = nodeManagerSaveFile;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMinSubscriptionLifetime(int minSubscriptionLifetime)
        {
            ApplicationConfiguration.ServerConfiguration.MinSubscriptionLifetime = minSubscriptionLifetime;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxPublishRequestCount(int maxPublishRequestCount)
        {
            ApplicationConfiguration.ServerConfiguration.MaxPublishRequestCount = maxPublishRequestCount;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxSubscriptionCount(int maxSubscriptionCount)
        {
            ApplicationConfiguration.ServerConfiguration.MaxSubscriptionCount = maxSubscriptionCount;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxEventQueueSize(int setMaxEventQueueSize)
        {
            ApplicationConfiguration.ServerConfiguration.MaxEventQueueSize = setMaxEventQueueSize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions AddServerProfile(string serverProfile)
        {
            ApplicationConfiguration.ServerConfiguration.ServerProfileArray.Add(serverProfile);
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetShutdownDelay(int shutdownDelay)
        {
            ApplicationConfiguration.ServerConfiguration.ShutdownDelay = shutdownDelay;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions AddServerCapabilities(string serverCapability)
        {
            ApplicationConfiguration.ServerConfiguration.ServerCapabilities.Add(serverCapability);
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetSupportedPrivateKeyFormats(StringCollection supportedPrivateKeyFormats)
        {
            ApplicationConfiguration.ServerConfiguration.SupportedPrivateKeyFormats = supportedPrivateKeyFormats;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMaxTrustListSize(int maxTrustListSize)
        {
            ApplicationConfiguration.ServerConfiguration.MaxTrustListSize = maxTrustListSize;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetMultiCastDnsEnabled(bool multiCastDnsEnabled)
        {
            ApplicationConfiguration.ServerConfiguration.MultiCastDnsEnabled = multiCastDnsEnabled;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetReverseConnect(ReverseConnectServerConfiguration reverseConnectConfiguration)
        {
            ApplicationConfiguration.ServerConfiguration.ReverseConnect = reverseConnectConfiguration;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetOperationLimits(OperationLimits operationLimits)
        {
            ApplicationConfiguration.ServerConfiguration.OperationLimits = operationLimits;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationServerOptions SetAuditingEnabled(bool auditingEnabled)
        {
            ApplicationConfiguration.ServerConfiguration.AuditingEnabled = auditingEnabled;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationClientOptions SetDefaultSessionTimeout(int defaultSessionTimeout)
        {
            ApplicationConfiguration.ClientConfiguration.DefaultSessionTimeout = defaultSessionTimeout;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationClientOptions AddWellKnownDiscoveryUrls(string wellKnownDiscoveryUrl)
        {
            ApplicationConfiguration.ClientConfiguration.WellKnownDiscoveryUrls.Add(wellKnownDiscoveryUrl);
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationClientOptions AddDiscoveryServer(EndpointDescription discoveryServer)
        {
            ApplicationConfiguration.ClientConfiguration.DiscoveryServers.Add(discoveryServer);
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationClientOptions SetEndpointCacheFilePath(string endpointCacheFilePath)
        {
            ApplicationConfiguration.ClientConfiguration.EndpointCacheFilePath = endpointCacheFilePath;
            return this;
        }

        /// <inheritdoc/>
        IUaApplicationConfigurationClientOptions IUaApplicationConfigurationClientOptions.SetMinSubscriptionLifetime(int minSubscriptionLifetime)
        {
            ApplicationConfiguration.ClientConfiguration.MinSubscriptionLifetime = minSubscriptionLifetime;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationClientOptions SetReverseConnect(ReverseConnectClientConfiguration reverseConnect)
        {
            ApplicationConfiguration.ClientConfiguration.ReverseConnect = reverseConnect;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationClientOptions SetClientOperationLimits(OperationLimits operationLimits)
        {
            ApplicationConfiguration.ClientConfiguration.OperationLimits = operationLimits;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTraceConfiguration SetOutputFilePath(string outputFilePath)
        {
            ApplicationConfiguration.TraceConfiguration.OutputFilePath = outputFilePath;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTraceConfiguration SetDeleteOnLoad(bool deleteOnLoad)
        {
            ApplicationConfiguration.TraceConfiguration.DeleteOnLoad = deleteOnLoad;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationTraceConfiguration SetTraceMasks(int traceMasks)
        {
            ApplicationConfiguration.TraceConfiguration.TraceMasks = traceMasks;
            return this;
        }

        /// <inheritdoc/>
        public IUaApplicationConfigurationExtension AddExtension<T>(XmlQualifiedName elementName, object value)
        {
            ApplicationConfiguration.UpdateExtension<T>(elementName, value);
            return this;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Internal enumeration of supported trust lists.
        /// </summary>
        private enum TrustlistType
        {
            Application,
            Trusted,
            Issuer,
            TrustedHttps,
            IssuerHttps,
            TrustedUser,
            IssuerUser,
            Rejected
        };

        /// <summary>
        /// Return the default PKI root path if root is unspecified, directory or X509Store.
        /// </summary>
        /// <param name="root">A real root path or the store type.</param>
        private string DefaultPKIRoot(string root)
        {
            if (root == null ||
                root.Equals(CertificateStoreType.Directory, StringComparison.OrdinalIgnoreCase))
            {
                return CertificateStoreIdentifier.DefaultPKIRoot;
            }
            else if (root.Equals(CertificateStoreType.X509Store, StringComparison.OrdinalIgnoreCase))
            {
                return CertificateStoreIdentifier.CurrentUser;
            }
            return root;
        }

        /// <summary>
        /// Determine the default store path for a given trust list type.
        /// </summary>
        /// <param name="trustListType">The trust list type.</param>
        /// <param name="pkiRoot">A PKI root for which the store path is needed.</param>
        private string DefaultCertificateStorePath(TrustlistType trustListType, string pkiRoot)
        {
            var pkiRootType = CertificateStoreIdentifier.DetermineStoreType(pkiRoot);
            if (pkiRootType.Equals(CertificateStoreType.Directory, StringComparison.OrdinalIgnoreCase))
            {
                string leafPath = "";
                // see https://reference.opcfoundation.org/v104/GDS/docs/F.1/
                switch (trustListType)
                {
                    case TrustlistType.Application: leafPath = "own"; break;
                    case TrustlistType.Trusted: leafPath = "trusted"; break;
                    case TrustlistType.Issuer: leafPath = "issuer"; break;
                    case TrustlistType.TrustedHttps: leafPath = "trustedHttps"; break;
                    case TrustlistType.IssuerHttps: leafPath = "issuerHttps"; break;
                    case TrustlistType.TrustedUser: leafPath = "trustedUser"; break;
                    case TrustlistType.IssuerUser: leafPath = "issuerUser"; break;
                    case TrustlistType.Rejected: leafPath = "rejected"; break;
                }
                // Caller may have already provided the leaf path, then no need to add.
                int startIndex = pkiRoot.Length - leafPath.Length;
                char lastChar = pkiRoot.Last();
                if (lastChar == Path.DirectorySeparatorChar ||
                    lastChar == Path.AltDirectorySeparatorChar)
                {
                    startIndex--;
                }
                if (startIndex > 0)
                {
                    if (pkiRoot.Substring(startIndex, leafPath.Length).Equals(leafPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return pkiRoot;
                    }
                }
                return Path.Combine(pkiRoot, leafPath);
            }
            else if (pkiRootType.Equals(CertificateStoreType.X509Store, StringComparison.OrdinalIgnoreCase))
            {
                switch (trustListType)
                {
                    case TrustlistType.Application:
#if !NETFRAMEWORK
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                            pkiRoot.StartsWith(CertificateStoreIdentifier.CurrentUser, StringComparison.OrdinalIgnoreCase))
                        {
                            return pkiRoot + "My";
                        }
#endif
                        return pkiRoot + "UA_MachineDefault";
                    case TrustlistType.Trusted:
                        return pkiRoot + "UA_Trusted";
                    case TrustlistType.Issuer:
                        return pkiRoot + "UA_Issuer";
                    case TrustlistType.TrustedHttps:
                        return pkiRoot + "UA_Trusted_Https";
                    case TrustlistType.IssuerHttps:
                        return pkiRoot + "UA_Issuer_Https";
                    case TrustlistType.TrustedUser:
                        return pkiRoot + "UA_Trusted_User";
                    case TrustlistType.IssuerUser:
                        return pkiRoot + "UA_Issuer_User";
                    case TrustlistType.Rejected:
                        return pkiRoot + "UA_Rejected";
                }
            }
            else
            {
                // return custom root store
                return pkiRoot;
            }
            throw new NotSupportedException("Unsupported store type.");
        }

        /// <summary>
        /// Add specified groups of security policies and security modes.
        /// </summary>
        /// <param name="includeSign">Include the Sign only policies.</param>
        /// <param name="deprecated">Include the deprecated policies.</param>
        /// <param name="policyNone">Include policy 'None'. (no security!)</param>
        private void AddSecurityPolicies(bool includeSign = false, bool deprecated = false, bool policyNone = false)
        {
            // create list of supported policies
            string[] defaultPolicyUris = SecurityPolicies.GetDefaultUris();
            if (deprecated)
            {
                var names = SecurityPolicies.GetDisplayNames();
                var deprecatedPolicyList = new List<string>();
                foreach (var name in names)
                {
                    var uri = SecurityPolicies.GetUri(name);
                    if (uri != null)
                    {
                        deprecatedPolicyList.Add(uri);
                    }
                }
                defaultPolicyUris = deprecatedPolicyList.ToArray();
            }

            foreach (MessageSecurityMode securityMode in typeof(MessageSecurityMode).GetEnumValues())
            {
                var policies = ApplicationConfiguration.ServerConfiguration.SecurityPolicies;
                if (policyNone && securityMode == MessageSecurityMode.None)
                {
                    InternalAddPolicy(policies, MessageSecurityMode.None, SecurityPolicies.None);
                }
                else if (securityMode >= MessageSecurityMode.SignAndEncrypt ||
                    (includeSign && securityMode == MessageSecurityMode.Sign))
                {
                    foreach (var policyUri in defaultPolicyUris)
                    {
                        InternalAddPolicy(policies, securityMode, policyUri);
                    }
                }
            }
        }

        /// <summary>
        /// Set secure defaults for flags.
        /// </summary>
        private void SetSecureDefaults(SecurityConfiguration securityConfiguration)
        {
            // ensure secure default settings
            securityConfiguration.AutoAcceptUntrustedCertificates = false;
            securityConfiguration.AddAppCertToTrustedStore = false;
            securityConfiguration.RejectSHA1SignedCertificates = true;
            securityConfiguration.RejectUnknownRevocationStatus = true;
            securityConfiguration.SuppressNonceValidationErrors = false;
            securityConfiguration.SendCertificateChain = true;
            securityConfiguration.MinimumCertificateKeySize = CertificateFactory.DefaultKeySize;
        }

        /// <summary>
        /// Add security policy if it doesn't exist yet.
        /// </summary>
        /// <param name="policies">The collection to which the policies are added.</param>
        /// <param name="securityMode">The message security mode.</param>
        /// <param name="policyUri">The security policy Uri.</param>
        private bool InternalAddPolicy(ServerSecurityPolicyCollection policies, MessageSecurityMode securityMode, string policyUri)
        {
            if (securityMode == MessageSecurityMode.Invalid) throw new ArgumentException("Invalid security mode selected", nameof(securityMode));
            var newPolicy = new ServerSecurityPolicy() {
                SecurityMode = securityMode,
                SecurityPolicyUri = policyUri
            };
            if (policies.Find(s =>
                s.SecurityMode == newPolicy.SecurityMode &&
                string.Equals(s.SecurityPolicyUri, newPolicy.SecurityPolicyUri, StringComparison.Ordinal)
                ) == null)
            {
                policies.Add(newPolicy);
                return true;
            }
            return false;
        }
        #endregion

        #region Private Fields
        private bool typeSelected_;
        #endregion
    }
}
