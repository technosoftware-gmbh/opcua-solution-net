#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// The certificates used by the tls/ssl layer 
    /// </summary>
    public class MqttTlsCertificates
    {
        #region Constructor
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="caCertificatePath"></param>
        /// <param name="clientCertificatePath"></param>
        /// <param name="clientCertificatePassword"></param>
        public MqttTlsCertificates(string caCertificatePath = null,
            string clientCertificatePath = null, string clientCertificatePassword = null)
        {
            CaCertificatePath = caCertificatePath ?? "";
            ClientCertificatePath = clientCertificatePath ?? "";
            ClientCertificatePassword = clientCertificatePassword ?? "";

            if (!string.IsNullOrEmpty(CaCertificatePath))
            {
                m_caCertificate = X509Certificate.CreateFromCertFile(CaCertificatePath);
            }
            if (!string.IsNullOrEmpty(clientCertificatePath))
            {
                m_clientCertificate = new X509Certificate2(clientCertificatePath, ClientCertificatePassword);
            }

            KeyValuePairs = new KeyValuePairCollection();

            QualifiedName qCaCertificatePath = EnumMqttClientConfigurationParameters.TlsCertificateCaCertificatePath.ToString();
            KeyValuePairs.Add(new Opc.Ua.KeyValuePair { Key = qCaCertificatePath, Value = CaCertificatePath });

            QualifiedName qClientCertificatePath = EnumMqttClientConfigurationParameters.TlsCertificateClientCertificatePath.ToString();
            KeyValuePairs.Add(new Opc.Ua.KeyValuePair { Key = qClientCertificatePath, Value = ClientCertificatePath });

            QualifiedName qClientCertificatePassword = EnumMqttClientConfigurationParameters.TlsCertificateClientCertificatePassword.ToString();
            KeyValuePairs.Add(new Opc.Ua.KeyValuePair { Key = qClientCertificatePassword, Value = ClientCertificatePassword });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public MqttTlsCertificates(KeyValuePairCollection keyValuePairs)
        {
            CaCertificatePath = "";
            QualifiedName qCaCertificatePath = EnumMqttClientConfigurationParameters.TlsCertificateCaCertificatePath.ToString();
            CaCertificatePath = keyValuePairs.Find(kvp => kvp.Key.Name.Equals(qCaCertificatePath.Name))?.Value.Value as string;

            ClientCertificatePath = "";
            QualifiedName qClientCertificatePath = EnumMqttClientConfigurationParameters.TlsCertificateClientCertificatePath.ToString();
            ClientCertificatePath = keyValuePairs.Find(kvp => kvp.Key.Name.Equals(qClientCertificatePath.Name))?.Value.Value as string;

            ClientCertificatePassword = "";
            QualifiedName qClientCertificatePassword = EnumMqttClientConfigurationParameters.TlsCertificateClientCertificatePassword.ToString();
            ClientCertificatePassword = keyValuePairs.Find(kvp => kvp.Key.Name.Equals(qClientCertificatePassword.Name))?.Value.Value as string;

            KeyValuePairs = keyValuePairs;

            if (!string.IsNullOrEmpty(CaCertificatePath))
            {
                m_caCertificate = X509Certificate.CreateFromCertFile(CaCertificatePath);
            }
            if (!string.IsNullOrEmpty(ClientCertificatePath))
            {
                m_clientCertificate = new X509Certificate2(ClientCertificatePath, ClientCertificatePassword);
            }

        }
        #endregion Constructor

        #region Internal Properties
        internal string CaCertificatePath { get; set; }
        internal string ClientCertificatePath { get; set; }
        internal string ClientCertificatePassword { get; set; }

        internal KeyValuePairCollection KeyValuePairs { get; set; }

        internal List<X509Certificate> X509Certificates
        {
            get
            {
                List<X509Certificate> values = new List<X509Certificate>();
                if (m_caCertificate != null)
                {
                    values.Add(m_caCertificate);
                }
                if (m_clientCertificate != null)
                {
                    values.Add(m_clientCertificate);
                }

                return values;
            }
        }
        #endregion  Internal Properties

        #region Private menbers
        private X509Certificate m_caCertificate;
        private X509Certificate m_clientCertificate;
        #endregion Private menbers

    }

    /// <summary>
    /// The implementation of the Tls client options
    /// </summary>
    public class MqttTlsOptions
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public MqttTlsOptions()
        {
            Certificates = null;
            SslProtocolVersion = SslProtocols.None;
            AllowUntrustedCertificates = false;
            IgnoreCertificateChainErrors = false;
            IgnoreRevocationListErrors = false;

            TrustedIssuerCertificates = null;
            TrustedPeerCertificates = null;
            RejectedCertificateStore = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="kvpMqttOptions">The key value pairs representing the values from which to construct MqttTlsOptions</param>
        public MqttTlsOptions(KeyValuePairCollection kvpMqttOptions)
        {
            Certificates = new MqttTlsCertificates(kvpMqttOptions);

            QualifiedName qSslProtocolVersion = EnumMqttClientConfigurationParameters.TlsProtocolVersion.ToString();
            SslProtocolVersion = (SslProtocols)Convert.ToInt32(kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qSslProtocolVersion.Name))?.Value.Value, CultureInfo.InvariantCulture);

            QualifiedName qAllowUntrustedCertificates = EnumMqttClientConfigurationParameters.TlsAllowUntrustedCertificates.ToString();
            AllowUntrustedCertificates = Convert.ToBoolean(kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qAllowUntrustedCertificates.Name, StringComparison.Ordinal))?.Value.Value, CultureInfo.InvariantCulture);

            QualifiedName qIgnoreCertificateChainErrors = EnumMqttClientConfigurationParameters.TlsIgnoreCertificateChainErrors.ToString();
            IgnoreCertificateChainErrors = Convert.ToBoolean(kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qIgnoreCertificateChainErrors.Name))?.Value.Value, CultureInfo.InvariantCulture);

            QualifiedName qIgnoreRevocationListErrors = EnumMqttClientConfigurationParameters.TlsIgnoreRevocationListErrors.ToString();
            IgnoreRevocationListErrors = Convert.ToBoolean(kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qIgnoreRevocationListErrors.Name))?.Value.Value, CultureInfo.InvariantCulture);

            QualifiedName qTrustedIssuerCertificatesStoreType = EnumMqttClientConfigurationParameters.TrustedIssuerCertificatesStoreType.ToString();
            string issuerCertificatesStoreType = kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qTrustedIssuerCertificatesStoreType.Name))?.Value.Value as string;
            QualifiedName qTrustedIssuerCertificatesStorePath = EnumMqttClientConfigurationParameters.TrustedIssuerCertificatesStorePath.ToString();
            string issuerCertificatesStorePath = kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qTrustedIssuerCertificatesStorePath.Name))?.Value.Value as string;

            TrustedIssuerCertificates = new CertificateTrustList {
                StoreType = issuerCertificatesStoreType,
                StorePath = issuerCertificatesStorePath
            };

            QualifiedName qTrustedPeerCertificatesStoreType = EnumMqttClientConfigurationParameters.TrustedPeerCertificatesStoreType.ToString();
            string peerCertificatesStoreType = kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qTrustedPeerCertificatesStoreType.Name))?.Value.Value as string;
            QualifiedName qTrustedPeerCertificatesStorePath = EnumMqttClientConfigurationParameters.TrustedPeerCertificatesStorePath.ToString();
            string peerCertificatesStorePath = kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qTrustedPeerCertificatesStorePath.Name))?.Value.Value as string;

            TrustedPeerCertificates = new CertificateTrustList {
                StoreType = peerCertificatesStoreType,
                StorePath = peerCertificatesStorePath
            };

            QualifiedName qRejectedCertificateStoreStoreType = EnumMqttClientConfigurationParameters.RejectedCertificateStoreStoreType.ToString();
            string rejectedCertificateStoreStoreType = kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qRejectedCertificateStoreStoreType.Name))?.Value.Value as string;
            QualifiedName qRejectedCertificateStoreStorePath = EnumMqttClientConfigurationParameters.RejectedCertificateStoreStorePath.ToString();
            string rejectedCertificateStoreStorePath = kvpMqttOptions.Find(kvp => kvp.Key.Name.Equals(qRejectedCertificateStoreStorePath.Name))?.Value.Value as string;

            RejectedCertificateStore = new CertificateTrustList {
                StoreType = rejectedCertificateStoreStoreType,
                StorePath = rejectedCertificateStoreStorePath
            };

            KeyValuePairs = kvpMqttOptions;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="certificates">The certificates used for encrypted communication including the CA certificate</param>
        /// <param name="sslProtocolVersion">The preferred version of SSL protocol</param>
        /// <param name="allowUntrustedCertificates">Specifies if untrusted certificates should be accepted in the process of certificate validation</param>
        /// <param name="ignoreCertificateChainErrors">Specifies if Certificate Chain errors should be validated in the process of certificate validation</param>
        /// <param name="ignoreRevocationListErrors">Specifies if Certificate Revocation List errors should be validated in the process of certificate validation</param>
        /// <param name="trustedIssuerCertificates">The trusted issuer certifficates store identifier</param>
        /// <param name="trustedPeerCertificates">The trusted peer certifficates store identifier</param>
        /// <param name="rejectedCertificateStore">The rejected certifficates store identifier</param>
        public MqttTlsOptions(MqttTlsCertificates certificates = null,
            SslProtocols sslProtocolVersion = SslProtocols.Tls12,
            bool allowUntrustedCertificates = false,
            bool ignoreCertificateChainErrors = false,
            bool ignoreRevocationListErrors = false,
            CertificateStoreIdentifier trustedIssuerCertificates = null,
            CertificateStoreIdentifier trustedPeerCertificates = null,
            CertificateStoreIdentifier rejectedCertificateStore = null
            )
        {
            Certificates = certificates;
            SslProtocolVersion = sslProtocolVersion;
            AllowUntrustedCertificates = allowUntrustedCertificates;
            IgnoreCertificateChainErrors = ignoreCertificateChainErrors;
            IgnoreRevocationListErrors = ignoreRevocationListErrors;

            TrustedIssuerCertificates = trustedIssuerCertificates;
            TrustedPeerCertificates = trustedPeerCertificates;
            RejectedCertificateStore = rejectedCertificateStore;

            KeyValuePairs = new KeyValuePairCollection();

            if (Certificates != null)
            {
                KeyValuePairs.AddRange(Certificates.KeyValuePairs);
            }

            Opc.Ua.KeyValuePair kvpTlsProtocolVersion = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TlsProtocolVersion.ToString(),
                Value = (int)SslProtocolVersion
            };
            KeyValuePairs.Add(kvpTlsProtocolVersion);
            Opc.Ua.KeyValuePair kvpAllowUntrustedCertificates = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TlsAllowUntrustedCertificates.ToString(),
                Value = AllowUntrustedCertificates
            };
            KeyValuePairs.Add(kvpAllowUntrustedCertificates);
            Opc.Ua.KeyValuePair kvpIgnoreCertificateChainErrors = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TlsIgnoreCertificateChainErrors.ToString(),
                Value = IgnoreCertificateChainErrors
            };
            KeyValuePairs.Add(kvpIgnoreCertificateChainErrors);
            Opc.Ua.KeyValuePair kvpIgnoreRevocationListErrors = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TlsIgnoreRevocationListErrors.ToString(),
                Value = IgnoreRevocationListErrors
            };
            KeyValuePairs.Add(kvpIgnoreRevocationListErrors);

            Opc.Ua.KeyValuePair kvpTrustedIssuerCertificatesStoreType = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TrustedIssuerCertificatesStoreType.ToString(),
                Value = TrustedIssuerCertificates?.StoreType
            };
            KeyValuePairs.Add(kvpTrustedIssuerCertificatesStoreType);
            Opc.Ua.KeyValuePair kvpTrustedIssuerCertificatesStorePath = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TrustedIssuerCertificatesStorePath.ToString(),
                Value = TrustedIssuerCertificates?.StorePath
            };
            KeyValuePairs.Add(kvpTrustedIssuerCertificatesStorePath);

            Opc.Ua.KeyValuePair kvpTrustedPeerCertificatesStoreType = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TrustedPeerCertificatesStoreType.ToString(),
                Value = TrustedPeerCertificates?.StoreType
            };
            KeyValuePairs.Add(kvpTrustedPeerCertificatesStoreType);
            Opc.Ua.KeyValuePair kvpTrustedPeerCertificatesStorePath = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.TrustedPeerCertificatesStorePath.ToString(),
                Value = TrustedPeerCertificates?.StorePath
            };
            KeyValuePairs.Add(kvpTrustedPeerCertificatesStorePath);

            Opc.Ua.KeyValuePair kvpRejectedCertificateStoreStoreType = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.RejectedCertificateStoreStoreType.ToString(),
                Value = RejectedCertificateStore?.StoreType
            };
            KeyValuePairs.Add(kvpRejectedCertificateStoreStoreType);
            Opc.Ua.KeyValuePair kvpRejectedCertificateStoreStorePath = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.RejectedCertificateStoreStorePath.ToString(),
                Value = RejectedCertificateStore?.StorePath
            };
            KeyValuePairs.Add(kvpRejectedCertificateStoreStorePath);
        }
        #endregion

        #region Internal Properties
        internal MqttTlsCertificates Certificates { get; set; }
        internal SslProtocols SslProtocolVersion { get; set; }
        internal bool AllowUntrustedCertificates { get; set; }
        internal bool IgnoreCertificateChainErrors { get; set; }
        internal bool IgnoreRevocationListErrors { get; set; }
        internal CertificateStoreIdentifier TrustedIssuerCertificates { get; set; }
        internal CertificateStoreIdentifier TrustedPeerCertificates { get; set; }
        internal CertificateStoreIdentifier RejectedCertificateStore { get; set; }
        internal KeyValuePairCollection KeyValuePairs { get; set; }
        #endregion
    }

    /// <summary>
    /// The implementation of the Mqtt specific client configuration
    /// </summary>
    public class MqttClientProtocolConfiguration : ITransportProtocolConfiguration
    {
        #region Private
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MqttClientProtocolConfiguration()
        {
            UserName = null;
            Password = null;
            AzureClientId = null;
            CleanSession = true;
            ProtocolVersion = EnumMqttProtocolVersion.V310;
            MqttTlsOptions = null;
            ConnectionProperties = null;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="userName">UserName part of user credentials</param>
        /// <param name="password">Password part of user credentials</param>
        /// <param name="azureClientId">The Client Id used in an Azure connection</param>
        /// <param name="cleanSession">Specifies if the MQTT session to the broker should be clean</param>
        /// <param name="version">The version of the MQTT protocol (default V310)</param>
        /// <param name="mqttTlsOptions">Instance of <see cref="MqttTlsOptions"/></param>
        public MqttClientProtocolConfiguration(SecureString userName = null,
                                               SecureString password = null,
                                               string azureClientId = null,
                                               bool cleanSession = true,
                                               EnumMqttProtocolVersion version = EnumMqttProtocolVersion.V310,
                                               MqttTlsOptions mqttTlsOptions = null)
        {
            UserName = userName;
            Password = password;
            AzureClientId = azureClientId;
            CleanSession = cleanSession;
            ProtocolVersion = version;
            MqttTlsOptions = mqttTlsOptions;

            ConnectionProperties = new KeyValuePairCollection();

            var kvpUserName = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.UserName.ToString(),
                Value = new System.Net.NetworkCredential(string.Empty, UserName).Password
            };
            ConnectionProperties.Add(kvpUserName);
            var kvpPassword = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.Password.ToString(),
                Value = new System.Net.NetworkCredential(string.Empty, Password).Password
            };
            ConnectionProperties.Add(kvpPassword);
            var kvpAzureClientId = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.AzureClientId.ToString(),
                Value = AzureClientId
            };
            ConnectionProperties.Add(kvpAzureClientId);
            var kvpCleanSession = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.CleanSession.ToString(),
                Value = CleanSession
            };
            ConnectionProperties.Add(kvpCleanSession);
            var kvpProtocolVersion = new Opc.Ua.KeyValuePair {
                Key = EnumMqttClientConfigurationParameters.ProtocolVersion.ToString(),
                Value = (int)ProtocolVersion
            };
            ConnectionProperties.Add(kvpProtocolVersion);

            if (MqttTlsOptions != null)
            {
                ConnectionProperties.AddRange(MqttTlsOptions.KeyValuePairs);
            }
        }

        /// <summary>
        /// Constructs a MqttClientProtocolConfiguration from given keyValuePairs
        /// </summary>
        /// <param name="connectionProperties"></param>
        public MqttClientProtocolConfiguration(KeyValuePairCollection connectionProperties)
        {
            UserName = new SecureString();
            QualifiedName qUserName = EnumMqttClientConfigurationParameters.UserName.ToString();
            if (connectionProperties.Find(kvp => kvp.Key.Name.Equals(qUserName.Name))?.Value.Value is string sUserName)
            {
                foreach (char c in sUserName?.ToCharArray())
                {
                    UserName.AppendChar(c);
                }
            }

            Password = new SecureString();
            QualifiedName qPassword = EnumMqttClientConfigurationParameters.Password.ToString();
            if (connectionProperties.Find(kvp => kvp.Key.Name.Equals(qPassword.Name))?.Value.Value is string sPassword)
            {
                foreach (char c in sPassword?.ToCharArray())
                {
                    Password.AppendChar(c);
                }
            }

            QualifiedName qAzureClientId = EnumMqttClientConfigurationParameters.AzureClientId.ToString();
            AzureClientId = Convert.ToString(connectionProperties.Find(kvp => kvp.Key.Name.Equals(qAzureClientId.Name))?.Value.Value, CultureInfo.InvariantCulture);

            QualifiedName qCleanSession = EnumMqttClientConfigurationParameters.CleanSession.ToString();
            CleanSession = Convert.ToBoolean(connectionProperties.Find(kvp => kvp.Key.Name.Equals(qCleanSession.Name))?.Value.Value, CultureInfo.InvariantCulture);

            QualifiedName qProtocolVersion = EnumMqttClientConfigurationParameters.ProtocolVersion.ToString();
            ProtocolVersion = (EnumMqttProtocolVersion)Convert.ToInt32(connectionProperties.Find(kvp => kvp.Key.Name.Equals(qProtocolVersion.Name))?.Value.Value, CultureInfo.InvariantCulture);
            if (ProtocolVersion == EnumMqttProtocolVersion.Unknown)
            {
                Utils.Trace(Utils.TraceMasks.Information, "Mqtt protocol version is Unknown and it will default to V310");
                ProtocolVersion = EnumMqttProtocolVersion.V310;
            }

            MqttTlsOptions = new MqttTlsOptions(connectionProperties);

            ConnectionProperties = connectionProperties;
        }
        #endregion

        #region Internal Properties
        internal SecureString UserName { get; set; }

        internal SecureString Password { get; set; }

        internal string AzureClientId { get; set; }

        internal bool CleanSession { get; set; }

        internal bool UseCredentials => (UserName != null) && (UserName.Length != 0);

        internal bool UseAzureClientId { get => (AzureClientId != null) && (AzureClientId.Length != 0); }

        internal EnumMqttProtocolVersion ProtocolVersion { get; set; }

        internal MqttTlsOptions MqttTlsOptions { get; set; }
        #endregion Internal Properties

        #region Public Properties
        /// <summary>
        /// The key value pairs representing the parameters of a MqttClientProtocolConfiguration
        /// </summary>
        public KeyValuePairCollection ConnectionProperties { get; set; }
        #endregion Public Propertis
    }
}
