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
using System.Threading.Tasks;
using System.Xml;

using Opc.Ua;
#endregion

namespace Technosoftware.UaConfiguration
{
    /// <summary>
    /// The client or server application configuration.
    /// </summary>
    public interface IUaApplicationConfiguration :
        IUaApplicationConfigurationTransportQuotas,
        IUaApplicationConfigurationServer,
        IUaApplicationConfigurationClient
    {
    }

    /// <summary>
    /// The set transport quota state.
    /// </summary>
    public interface IUaApplicationConfigurationTransportQuotas :
        IUaApplicationConfigurationServer,
        IUaApplicationConfigurationClient
    {
        /// <summary>
        /// Set the transport quotas for this application (client and server).
        /// </summary>
        /// <param name="transportQuotas">The object with the new transport quotas.</param>
        IUaApplicationConfigurationTransportQuotasSet SetTransportQuotas(TransportQuotas transportQuotas);

        /// <inheritdoc cref="TransportQuotas.OperationTimeout"/>
        /// <remarks>applies to <see cref="TransportQuotas.OperationTimeout"/></remarks>
        /// <param name="operationTimeout">The operation timeout in ms.</param>
        IUaApplicationConfigurationTransportQuotas SetOperationTimeout(int operationTimeout);

        /// <inheritdoc cref="TransportQuotas.MaxStringLength"/>
        /// <remarks>applies to <see cref="TransportQuotas.MaxStringLength"/></remarks>
        /// <param name="maxStringLength">The max string length.</param>
        IUaApplicationConfigurationTransportQuotas SetMaxStringLength(int maxStringLength);

        /// <inheritdoc cref="TransportQuotas.MaxByteStringLength"/>
        /// <remarks>applies to <see cref="TransportQuotas.MaxByteStringLength"/></remarks>
        /// <param name="maxByteStringLength">The max byte string length.</param>
        IUaApplicationConfigurationTransportQuotas SetMaxByteStringLength(int maxByteStringLength);

        /// <inheritdoc cref="TransportQuotas.MaxArrayLength"/>
        /// <remarks>applies to <see cref="TransportQuotas.MaxArrayLength"/></remarks>
        /// <param name="maxArrayLength">The max array length.</param>
        IUaApplicationConfigurationTransportQuotas SetMaxArrayLength(int maxArrayLength);

        /// <inheritdoc cref="TransportQuotas.MaxMessageSize"/>
        /// <remarks>applies to <see cref="TransportQuotas.MaxMessageSize"/></remarks>
        /// <param name="maxMessageSize">The max message size.</param>
        IUaApplicationConfigurationTransportQuotas SetMaxMessageSize(int maxMessageSize);

        /// <inheritdoc cref="TransportQuotas.MaxBufferSize"/>
        /// <remarks>applies to <see cref="TransportQuotas.MaxBufferSize"/></remarks>
        /// <param name="maxBufferSize">The max buffer size.</param>
        IUaApplicationConfigurationTransportQuotas SetMaxBufferSize(int maxBufferSize);

        /// <inheritdoc cref="TransportQuotas.ChannelLifetime"/>
        /// <remarks>applies to <see cref="TransportQuotas.ChannelLifetime"/></remarks>
        /// <param name="channelLifetime">The lifetime.</param>
        IUaApplicationConfigurationTransportQuotas SetChannelLifetime(int channelLifetime);

        /// <inheritdoc cref="TransportQuotas.SecurityTokenLifetime"/>
        /// <remarks>applies to <see cref="TransportQuotas.SecurityTokenLifetime"/></remarks>
        /// <param name="securityTokenLifetime">The lifetime in milliseconds.</param>
        IUaApplicationConfigurationTransportQuotas SetSecurityTokenLifetime(int securityTokenLifetime);
    }

    /// <summary>
    /// The set transport quota state.
    /// </summary>
    public interface IUaApplicationConfigurationTransportQuotasSet :
        IUaApplicationConfigurationServer,
        IUaApplicationConfigurationClient
    {
    }

    /// <summary>
    /// The interfaces to implement if a server is selected.
    /// </summary>
    public interface IUaApplicationConfigurationServerSelected :
        IUaApplicationConfigurationServerPolicies,
        IUaApplicationConfigurationServerOptions,
        IUaApplicationConfigurationClient,
        IUaApplicationConfigurationSecurity
    {
    }

    /// <summary>
    /// The options which can be set if a server is selected.
    /// </summary>
    public interface IUaApplicationConfigurationServerOptions :
        IUaApplicationConfigurationClient,
        IUaApplicationConfigurationSecurity
    {
        /// <inheritdoc cref="ServerBaseConfiguration.MinRequestThreadCount"/>
        IUaApplicationConfigurationServerOptions SetMinRequestThreadCount(int minRequestThreadCount);

        /// <inheritdoc cref="ServerBaseConfiguration.MaxRequestThreadCount"/>
        IUaApplicationConfigurationServerOptions SetMaxRequestThreadCount(int maxRequestThreadCount);

        /// <inheritdoc cref="ServerBaseConfiguration.MaxQueuedRequestCount"/>
        IUaApplicationConfigurationServerOptions SetMaxQueuedRequestCount(int maxQueuedRequestCount);

        /// <inheritdoc cref="ServerConfiguration.DiagnosticsEnabled"/>
        IUaApplicationConfigurationServerOptions SetDiagnosticsEnabled(bool diagnosticsEnabled);

        /// <inheritdoc cref="ServerConfiguration.MaxSessionCount"/>
        IUaApplicationConfigurationServerOptions SetMaxSessionCount(int maxSessionCount);

        /// <inheritdoc cref="ServerConfiguration.MinSessionTimeout"/>
        IUaApplicationConfigurationServerOptions SetMinSessionTimeout(int minSessionTimeout);

        /// <inheritdoc cref="ServerConfiguration.MaxSessionTimeout"/>
        IUaApplicationConfigurationServerOptions SetMaxSessionTimeout(int maxSessionTimeout);

        /// <inheritdoc cref="ServerConfiguration.MaxBrowseContinuationPoints"/>
        IUaApplicationConfigurationServerOptions SetMaxBrowseContinuationPoints(int maxBrowseContinuationPoints);

        /// <inheritdoc cref="ServerConfiguration.MaxQueryContinuationPoints"/>
        IUaApplicationConfigurationServerOptions SetMaxQueryContinuationPoints(int maxQueryContinuationPoints);

        /// <inheritdoc cref="ServerConfiguration.MaxHistoryContinuationPoints"/>
        IUaApplicationConfigurationServerOptions SetMaxHistoryContinuationPoints(int maxHistoryContinuationPoints);

        /// <inheritdoc cref="ServerConfiguration.MaxRequestAge"/>
        IUaApplicationConfigurationServerOptions SetMaxRequestAge(int maxRequestAge);

        /// <inheritdoc cref="ServerConfiguration.MinPublishingInterval"/>
        IUaApplicationConfigurationServerOptions SetMinPublishingInterval(int minPublishingInterval);

        /// <inheritdoc cref="ServerConfiguration.MaxPublishingInterval"/>
        IUaApplicationConfigurationServerOptions SetMaxPublishingInterval(int maxPublishingInterval);

        /// <inheritdoc cref="ServerConfiguration.PublishingResolution"/>
        IUaApplicationConfigurationServerOptions SetPublishingResolution(int publishingResolution);

        /// <inheritdoc cref="ServerConfiguration.MaxSubscriptionLifetime"/>
        IUaApplicationConfigurationServerOptions SetMaxSubscriptionLifetime(int maxSubscriptionLifetime);

        /// <inheritdoc cref="ServerConfiguration.MaxMessageQueueSize"/>
        IUaApplicationConfigurationServerOptions SetMaxMessageQueueSize(int maxMessageQueueSize);

        /// <inheritdoc cref="ServerConfiguration.MaxNotificationQueueSize"/>
        IUaApplicationConfigurationServerOptions SetMaxNotificationQueueSize(int maxNotificationQueueSize);

        /// <inheritdoc cref="ServerConfiguration.MaxNotificationsPerPublish"/>
        IUaApplicationConfigurationServerOptions SetMaxNotificationsPerPublish(int maxNotificationsPerPublish);

        /// <inheritdoc cref="ServerConfiguration.MinMetadataSamplingInterval"/>
        IUaApplicationConfigurationServerOptions SetMinMetadataSamplingInterval(int minMetadataSamplingInterval);

        /// <inheritdoc cref="ServerConfiguration.AvailableSamplingRates"/>
        IUaApplicationConfigurationServerOptions SetAvailableSamplingRates(SamplingRateGroupCollection availableSampleRates);

        /// <inheritdoc cref="ServerConfiguration.RegistrationEndpoint"/>
        IUaApplicationConfigurationServerOptions SetRegistrationEndpoint(EndpointDescription registrationEndpoint);

        /// <inheritdoc cref="ServerConfiguration.MaxRegistrationInterval"/>
        IUaApplicationConfigurationServerOptions SetMaxRegistrationInterval(int maxRegistrationInterval);

        /// <inheritdoc cref="ServerConfiguration.NodeManagerSaveFile"/>
        IUaApplicationConfigurationServerOptions SetNodeManagerSaveFile(string nodeManagerSaveFile);

        /// <inheritdoc cref="ServerConfiguration.MinSubscriptionLifetime"/>
        IUaApplicationConfigurationServerOptions SetMinSubscriptionLifetime(int minSubscriptionLifetime);

        /// <inheritdoc cref="ServerConfiguration.MaxPublishRequestCount"/>
        IUaApplicationConfigurationServerOptions SetMaxPublishRequestCount(int maxPublishRequestCount);

        /// <inheritdoc cref="ServerConfiguration.MaxSubscriptionCount"/>
        IUaApplicationConfigurationServerOptions SetMaxSubscriptionCount(int maxSubscriptionCount);

        /// <inheritdoc cref="ServerConfiguration.MaxEventQueueSize"/>
        IUaApplicationConfigurationServerOptions SetMaxEventQueueSize(int setMaxEventQueueSize);

        /// <inheritdoc cref="ServerConfiguration.ServerProfileArray" path="/summary"/>
        /// <param name="serverProfile">Add a server profile to the array.</param>
        IUaApplicationConfigurationServerOptions AddServerProfile(string serverProfile);

        /// <inheritdoc cref="ServerConfiguration.ShutdownDelay"/>
        IUaApplicationConfigurationServerOptions SetShutdownDelay(int shutdownDelay);

        /// <inheritdoc cref="ServerConfiguration.ServerCapabilities" path="/summary"/>
        /// <param name="serverCapability">The server capability to add.</param>
        IUaApplicationConfigurationServerOptions AddServerCapabilities(string serverCapability);

        /// <inheritdoc cref="ServerConfiguration.SupportedPrivateKeyFormats"/>
        IUaApplicationConfigurationServerOptions SetSupportedPrivateKeyFormats(StringCollection supportedPrivateKeyFormats);

        /// <inheritdoc cref="ServerConfiguration.MaxTrustListSize"/>
        IUaApplicationConfigurationServerOptions SetMaxTrustListSize(int maxTrustListSize);

        /// <inheritdoc cref="ServerConfiguration.MultiCastDnsEnabled"/>
        IUaApplicationConfigurationServerOptions SetMultiCastDnsEnabled(bool multiCastDnsEnabled);

        /// <inheritdoc cref="ServerConfiguration.ReverseConnect"/>
        IUaApplicationConfigurationServerOptions SetReverseConnect(ReverseConnectServerConfiguration reverseConnectConfiguration);

        /// <inheritdoc cref="ServerConfiguration.OperationLimits"/>
        IUaApplicationConfigurationServerOptions SetOperationLimits(OperationLimits operationLimits);

        /// <inheritdoc cref="ServerConfiguration.AuditingEnabled"/>
        IUaApplicationConfigurationServerOptions SetAuditingEnabled(bool auditingEnabled);
    }

    /// <summary>
    /// The interfaces to implement if a client is selected.
    /// </summary>
    public interface IUaApplicationConfigurationClientSelected :
        IUaApplicationConfigurationClientOptions,
        IUaApplicationConfigurationSecurity
    {
    }

    /// <summary>
    /// The options to set if a client is selected.
    /// </summary>
    public interface IUaApplicationConfigurationClientOptions :
        IUaApplicationConfigurationSecurity
    {
        /// <inheritdoc cref="ClientConfiguration.DefaultSessionTimeout"/>
        IUaApplicationConfigurationClientOptions SetDefaultSessionTimeout(int defaultSessionTimeout);

        /// <inheritdoc cref="ClientConfiguration.WellKnownDiscoveryUrls"/>
        /// <param name="wellKnownDiscoveryUrl">The well known discovery server url to add.</param>
        IUaApplicationConfigurationClientOptions AddWellKnownDiscoveryUrls(string wellKnownDiscoveryUrl);

        /// <inheritdoc cref="ClientConfiguration.DiscoveryServers"/>
        /// <param name="discoveryServer">The discovery server endpoint description to add.</param>
        IUaApplicationConfigurationClientOptions AddDiscoveryServer(EndpointDescription discoveryServer);

        /// <inheritdoc cref="ClientConfiguration.EndpointCacheFilePath"/>
        IUaApplicationConfigurationClientOptions SetEndpointCacheFilePath(string endpointCacheFilePath);

        /// <inheritdoc cref="ClientConfiguration.MinSubscriptionLifetime"/>
        IUaApplicationConfigurationClientOptions SetMinSubscriptionLifetime(int minSubscriptionLifetime);

        /// <inheritdoc cref="ClientConfiguration.ReverseConnect"/>
        IUaApplicationConfigurationClientOptions SetReverseConnect(ReverseConnectClientConfiguration reverseConnect);

        /// <inheritdoc cref="ClientConfiguration.OperationLimits"/>
        IUaApplicationConfigurationClientOptions SetClientOperationLimits(OperationLimits operationLimits);
    }

    /// <summary>
    /// Add the server configuration (optional).
    /// </summary>
    public interface IUaApplicationConfigurationServer
    {
        /// <summary>
        /// Configure instance to be used for UA server.
        /// </summary>
        IUaApplicationConfigurationServerSelected AsServer(
            string[] baseAddresses,
            string[] alternateBaseAddresses = null);
    }

    /// <summary>
    /// Add the client configuration (optional).
    /// </summary>
    public interface IUaApplicationConfigurationClient
    {
        /// <summary>
        /// Configure instance to be used for UA client.
        /// </summary>
        IUaApplicationConfigurationClientSelected AsClient();
    }

    /// <summary>
    /// Add the supported server policies.
    /// </summary>
    public interface IUaApplicationConfigurationServerPolicies
    {
        /// <summary>
        /// Add the unsecure security policy type none to server configuration.
        /// </summary>
        /// <param name="addPolicy">Add policy if true.</param>
        IUaApplicationConfigurationServerSelected AddUnsecurePolicyNone(bool addPolicy = true);

        /// <summary>
        /// Add the sign security policies to the server configuration.
        /// </summary>
        /// <param name="addPolicies">Add policies if true.</param>
        IUaApplicationConfigurationServerSelected AddSignPolicies(bool addPolicies = true);

        /// <summary>
        /// Add the sign and encrypt security policies to the server configuration.
        /// </summary>
        /// <param name="addPolicies">Add policies if true.</param>
        IUaApplicationConfigurationServerSelected AddSignAndEncryptPolicies(bool addPolicies = true);

        /// <summary>
        /// Add the specified security policy with the specified security mode.
        /// </summary>
        /// <param name="securityMode">The message security mode to add the policy to.</param>
        /// <param name="securityPolicy">The security policy Uri string.</param>
        IUaApplicationConfigurationServerSelected AddPolicy(MessageSecurityMode securityMode, string securityPolicy);

        /// <summary>
        /// Add user token policy to the server configuration.
        /// </summary>
        /// <param name="userTokenType">The user token type to add.</param>
        IUaApplicationConfigurationServerSelected AddUserTokenPolicy(UserTokenType userTokenType);

        /// <summary>
        /// Add user token policy to the server configuration.
        /// </summary>
        /// <param name="userTokenPolicy">The user token policy to add.</param>
        IUaApplicationConfigurationServerSelected AddUserTokenPolicy(UserTokenPolicy userTokenPolicy);

    }

    /// <summary>
    /// Add the security configuration (mandatory).
    /// </summary>
    public interface IUaApplicationConfigurationSecurity
    {
        /// <summary>
        /// Add the security configuration.
        /// </summary>
        /// <remarks>
        /// The pki root path default to the certificate store
        /// location as defined in <see cref="CertificateStoreIdentifier.DefaultPKIRoot"/>
        /// A <see cref="CertificateStoreType"/> defaults to the corresponding default store location.
        /// </remarks>
        /// <param name="subjectName">Application certificate subject name as distinguished name. A DC=localhost entry is converted to the hostname. The common name CN= is mandatory.</param>
        /// <param name="pkiRoot">The path to the pki root. By default all cert stores use the pki root.</param>
        /// <param name="appRoot">The path to the app cert store, if different than the pki root.</param>
        /// <param name="rejectedRoot">The path to the rejected certificate store.</param>
        IUaApplicationConfigurationSecurityOptions AddSecurityConfiguration(
            string subjectName,
            string pkiRoot = null,
            string appRoot = null,
            string rejectedRoot = null
            );

        /// <summary>
        /// Add the security configuration for mandatory application, issuer and trusted stores.
        /// </summary>
        /// <param name="subjectName">Application certificate subject name as distinguished name.
        /// A DC=localhost entry is converted to the hostname. The common name CN= is mandatory.</param>
        /// <param name="appRoot">The path to the app cert store.</param>
        /// <param name="trustedRoot">The path to the trusted cert store.</param>
        /// <param name="issuerRoot">The path to the issuer cert store.</param>
        /// <param name="rejectedRoot">The path to the rejected certificate store.</param>
        IUaApplicationConfigurationSecurityOptionStores AddSecurityConfigurationStores(
            string subjectName,
            string appRoot,
            string trustedRoot,
            string issuerRoot,
            string rejectedRoot = null
            );
    }

    /// <summary>
    /// Add security options to the configuration.
    /// </summary>
    public interface IUaApplicationConfigurationSecurityOptionStores :
        IUaApplicationConfigurationTraceConfiguration,
        IUaApplicationConfigurationExtension,
        IUaApplicationConfigurationCreate
    {
        /// <summary>
        /// Add the security configuration for the user certificate issuer and trusted stores.
        /// </summary>
        /// <param name="trustedRoot">The path to the trusted cert store.</param>
        /// <param name="issuerRoot">The path to the issuer cert store.</param>
        IUaApplicationConfigurationSecurityOptionStores AddSecurityConfigurationUserStore(
            string trustedRoot,
            string issuerRoot
            );

        /// <summary>
        /// Add the security configuration for the https certificate issuer and trusted stores.
        /// </summary>
        /// <param name="trustedRoot">The path to the trusted cert store.</param>
        /// <param name="issuerRoot">The path to the issuer cert store.</param>
        IUaApplicationConfigurationSecurityOptionStores AddSecurityConfigurationHttpsStore(
            string trustedRoot,
            string issuerRoot
            );
    }

    /// <summary>
    /// Add security options to the configuration.
    /// </summary>
    public interface IUaApplicationConfigurationSecurityOptions :
        IUaApplicationConfigurationTraceConfiguration,
        IUaApplicationConfigurationExtension,
        IUaApplicationConfigurationCreate
    {
        /// <summary>
        /// Whether an unknown application certificate should be accepted
        /// once all other security checks passed.
        /// </summary>
        /// <param name="autoAccept"><see langword="true"/> to accept unknown application certificates.</param>
        IUaApplicationConfigurationSecurityOptions SetAutoAcceptUntrustedCertificates(bool autoAccept);

        /// <summary>
        /// Whether a newly created application certificate should be added to the trusted store.
        /// This function is only useful if multiple UA applications share the same trusted store.
        /// </summary>
        /// <param name="addToTrustedStore"><see langword="true"/> to add the cert to the trusted store.</param>
        IUaApplicationConfigurationSecurityOptions SetAddAppCertToTrustedStore(bool addToTrustedStore);

        /// <summary>
        /// Reject SHA1 signed certificates.
        /// </summary>
        /// <param name="rejectSHA1Signed"><see langword="false"/> to accept SHA1 signed certificates.</param>
        IUaApplicationConfigurationSecurityOptions SetRejectSHA1SignedCertificates(bool rejectSHA1Signed);

        /// <summary>
        /// Reject chain validation with CA certs with unknown revocation status,
        /// e.g. when the CRL is not available or the OCSP provider is offline.
        /// </summary>
        /// <param name="rejectUnknownRevocationStatus"><see langword="false"/> to accept CA certs with unknown revocation status.</param>
        IUaApplicationConfigurationSecurityOptions SetRejectUnknownRevocationStatus(bool rejectUnknownRevocationStatus);

        /// <summary>
        /// Use the validated certificates for fast Validation.
        /// </summary>
        /// <param name="useValidatedCertificates"><see langword="true"/> to use the validated certificates.</param>
        IUaApplicationConfigurationSecurityOptions SetUseValidatedCertificates(bool useValidatedCertificates);

        /// <summary>
        /// Whether to suppress errors which are caused by clients and servers which provide
        /// zero nonce values or nonce with insufficient entropy.
        /// Suppressing this error is a security risk and may allow an attacker to decrypt user tokens.
        /// Only use if interoperability issues with legacy servers or clients leave no other choice to operate.
        /// </summary>
        /// <param name="suppressNonceValidationErrors"><see langword="true"/> to suppress nonce validation errors.</param>
        IUaApplicationConfigurationSecurityOptions SetSuppressNonceValidationErrors(bool suppressNonceValidationErrors);

        /// <summary>
        /// Whether a certificate chain should be sent with the application certificate.
        /// Only used if the application certificate is CA signed.
        /// </summary>
        /// <param name="sendCertificateChain"><see langword="true"/> to send the certificate chain with the application certificate.</param>
        IUaApplicationConfigurationSecurityOptions SetSendCertificateChain(bool sendCertificateChain);

        /// <summary>
        /// The minimum RSA key size to accept.
        /// By default the key size is set to <see cref="CertificateFactory.DefaultKeySize"/>.
        /// </summary>
        /// <param name="keySize">The minimum RSA key size to accept.</param>
        IUaApplicationConfigurationSecurityOptions SetMinimumCertificateKeySize(ushort keySize);

        /// <summary>
        /// Add a certificate password provider.
        /// </summary>
        /// <param name="certificatePasswordProvider">The certificate password provider to use.</param>
        IUaApplicationConfigurationSecurityOptions AddCertificatePasswordProvider(ICertificatePasswordProvider certificatePasswordProvider);
    }

    /// <summary>
    /// Add extensions configuration.
    /// </summary>
    public interface IUaApplicationConfigurationExtension :
        IUaApplicationConfigurationTraceConfiguration
    {
        /// <summary>
        /// Add an extension to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of the object to add as an extension.</typeparam>
        /// <param name="elementName">The name of the extension, null to use the name.</param>
        /// <param name="value">The object to add and encode.</param>
        IUaApplicationConfigurationExtension AddExtension<T>(XmlQualifiedName elementName, object value);
    }

    /// <summary>
    /// Add the trace configuration.
    /// </summary>
    public interface IUaApplicationConfigurationTraceConfiguration :
        IUaApplicationConfigurationCreate
    {
        /// <inheritdoc cref="TraceConfiguration.OutputFilePath"/>
        IUaApplicationConfigurationTraceConfiguration SetOutputFilePath(string outputFilePath);

        /// <inheritdoc cref="TraceConfiguration.DeleteOnLoad"/>
        IUaApplicationConfigurationTraceConfiguration SetDeleteOnLoad(bool deleteOnLoad);

        /// <inheritdoc cref="TraceConfiguration.TraceMasks"/>
        IUaApplicationConfigurationTraceConfiguration SetTraceMasks(int traceMasks);
    }

    /// <summary>
    /// Create and validate the application configuration.
    /// </summary>
    public interface IUaApplicationConfigurationCreate
    {
        /// <summary>
        /// Creates and updates the application configuration.
        /// </summary>
        Task<ApplicationConfiguration> CreateAsync();
    }
}

