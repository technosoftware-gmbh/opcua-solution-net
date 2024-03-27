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
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
using static Opc.Ua.Utils;
#endregion

namespace Technosoftware.UaConfiguration
{
    /// <summary>
    /// A class that install, configures and runs a UA application.
    /// </summary>
    public class ApplicationInstance
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInstance"/> class.
        /// </summary>
        public ApplicationInstance()
        {
            DisableCertificateAutoCreation = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInstance"/> class.
        /// </summary>
        /// <param name="applicationConfiguration">The application configuration.</param>
        public ApplicationInstance(ApplicationConfiguration applicationConfiguration)
            : this()
        {
            ApplicationConfiguration = applicationConfiguration;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the type of the application.
        /// </summary>
        /// <value>The type of the application.</value>
        public ApplicationType ApplicationType { get; set; }

        /// <summary>
        /// Gets or sets the name of the config section containing the path to the application configuration file.
        /// </summary>
        /// <value>The name of the config section.</value>
        public string ConfigSectionName { get; set; }

        /// <summary>
        /// Gets or sets the type of configuration file.
        /// </summary>
        /// <value>The type of configuration file.</value>
        public Type ConfigurationType { get; set; }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>The server.</value>
        public ServerBase BaseServer { get; set; }

        /// <summary>
        /// Gets the application configuration used when the Start() method was called.
        /// </summary>
        /// <value>The application configuration.</value>
        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        /// <summary>
        /// Gets or Sets a Generic Message Dialog for issues during loading of an application
        /// </summary>
        public static IUaApplicationMessageDlg MessageDlg { get; set; }

        /// <summary>
        /// Get or set the certificate password provider.
        /// </summary>
        public ICertificatePasswordProvider CertificatePasswordProvider { get; set; }

        /// <summary>
        /// Get or set bool which indicates if the auto creation
        /// of a new application certificate during startup is disabled.
        /// Default is enabled./>
        /// </summary>
        /// <remarks>
        /// Prevents auto self signed cert creation in use cases
        /// where an expired certificate should not be automatically
        /// renewed or where it is required to only use certificates
        /// provided by the user.
        /// </remarks>
        public bool DisableCertificateAutoCreation { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the UA server.
        /// </summary>
        /// <param name="server">The server.</param>
        public async Task StartAsync(ServerBase server)
        {
            BaseServer = server;

            if (ApplicationConfiguration == null)
            {
                _ = await LoadApplicationConfigurationAsync(false).ConfigureAwait(false);
            }

            server.Start(ApplicationConfiguration);
        }

        /// <summary>
        /// Stops the UA server.
        /// </summary>
        public void Stop()
        {
            BaseServer.Stop();
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        public async Task<ApplicationConfiguration> LoadApplicationConfigurationAsync(
            bool silent,
            string filePath,
            ApplicationType applicationType,
            Type configurationType,
            bool applyTraceSettings,
            ICertificatePasswordProvider certificatePasswordProvider = null)
        {
            LogInfo("Loading application configuration file. {0}", filePath);

            try
            {
                // load the configuration file.
                ApplicationConfiguration configuration = await ApplicationConfiguration.Load(
                    new FileInfo(filePath),
                    applicationType,
                    configurationType,
                    applyTraceSettings,
                    certificatePasswordProvider)
                    .ConfigureAwait(false);

                return configuration ?? null;
            }
            catch (Exception e)
            {
                LogError(e, "Could not load configuration file. {0}", filePath);

                // warn user.
                if (!silent)
                {
                    if (MessageDlg != null)
                    {
                        MessageDlg.Message("Load Application Configuration: " + e.Message);
                        _ = await MessageDlg.ShowAsync().ConfigureAwait(false);
                    }

                    throw;
                }

                return null;
            }
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        public async Task<ApplicationConfiguration> LoadApplicationConfigurationAsync(
            bool silent,
            Stream stream,
            ApplicationType applicationType,
            Type configurationType,
            bool applyTraceSettings,
            ICertificatePasswordProvider certificatePasswordProvider = null)
        {
            LogInfo("Loading application from stream.");

            try
            {
                // load the configuration file.
                ApplicationConfiguration configuration = await ApplicationConfiguration.Load(
                    stream,
                    applicationType,
                    configurationType,
                    applyTraceSettings,
                    certificatePasswordProvider)
                    .ConfigureAwait(false);

                return configuration ?? null;
            }
            catch (Exception e)
            {
                LogError(e, "Could not load configuration from stream.");

                // warn user.
                if (!silent)
                {
                    if (MessageDlg != null)
                    {
                        MessageDlg.Message("Load Application Configuration: " + e.Message);
                        _ = await MessageDlg.ShowAsync().ConfigureAwait(false);
                    }

                    throw;
                }

                return null;
            }
        }

        /// <summary>
        /// Loads the application configuration.
        /// </summary>
        public async Task<ApplicationConfiguration> LoadApplicationConfigurationAsync(Stream stream, bool silent)
        {
            ApplicationConfiguration configuration = null;

            try
            {
                configuration = await LoadApplicationConfigurationAsync(
                    silent, stream, ApplicationType, ConfigurationType, true, CertificatePasswordProvider)
                    .ConfigureAwait(false);
            }
            catch (Exception) when (silent)
            {
            }

            if (configuration == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration.");
            }

            ApplicationConfiguration = FixupAppConfig(configuration);

            return configuration;
        }

        /// <summary>
        /// Loads the application configuration.
        /// </summary>
        public async Task<ApplicationConfiguration> LoadApplicationConfigurationAsync(string filePath, bool silent)
        {
            ApplicationConfiguration configuration = null;

            try
            {
                configuration = await LoadApplicationConfigurationAsync(
                    silent, filePath, ApplicationType, ConfigurationType, true, CertificatePasswordProvider)
                    .ConfigureAwait(false);
            }
            catch (Exception) when (silent)
            {
            }

            if (configuration == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");
            }

            ApplicationConfiguration = FixupAppConfig(configuration);

            return configuration;
        }

        /// <summary>
        /// Loads the application configuration.
        /// </summary>
        public async Task<ApplicationConfiguration> LoadApplicationConfigurationAsync(bool silent)
        {
            var filePath = ApplicationConfiguration.GetFilePathFromAppConfig(ConfigSectionName);

            return await LoadApplicationConfigurationAsync(filePath, silent).ConfigureAwait(false);
        }

        /// <summary>   Loads the application configuration. </summary>
        ///
        /// <param name="configSectionName">    The name of the config section containing the path to the
        ///                                     application configuration file. </param>
        ///
        /// <returns>   The application configuration. </returns>
        /// <exception caption="" cref="ServiceResultException">   Thrown when a service result error condition
        ///                                             occurs. </exception>
        public async Task<ApplicationConfiguration> LoadConfigurationAsync(string configSectionName)
        {
            ConfigSectionName = configSectionName;
            return await LoadApplicationConfigurationAsync(true).ConfigureAwait(false);
        }

        /// <summary>
        /// Fix the application configuration regarding localhost usage.
        /// </summary>
        /// <param name="configuration">The application configuration to be fixed.</param>
        /// <returns></returns>
        public static ApplicationConfiguration FixupAppConfig(
            ApplicationConfiguration configuration)
        {
            configuration.ApplicationUri = ReplaceLocalhost(configuration.ApplicationUri);
            if (configuration.ServerConfiguration != null)
            {
                for (var i = 0; i < configuration.ServerConfiguration.BaseAddresses.Count; i++)
                {
                    configuration.ServerConfiguration.BaseAddresses[i] =
                        ReplaceLocalhost(configuration.ServerConfiguration.BaseAddresses[i]);
                }
            }
            return configuration;
        }

        /// <summary>
        /// Create an application configuration manager for an OPC UA application.
        /// </summary>
        public IUaApplicationConfiguration CreateApplicationConfigurationManager(
            string applicationUri,
            string productUri
            )
        {
            // App Uri and cert subject
            ApplicationConfiguration = new ApplicationConfiguration {
                ApplicationName = ApplicationName,
                ApplicationType = ApplicationType,
                ApplicationUri = applicationUri,
                ProductUri = productUri,
                TraceConfiguration = new TraceConfiguration {
                    TraceMasks = TraceMasks.None
                },
                TransportQuotas = new TransportQuotas()
            };

            // Trace off
            ApplicationConfiguration.TraceConfiguration.ApplySettings();

            return new ApplicationConfigurationManager(this);
        }

        /// <summary>
        /// Returns the expiry date of the application instance certificate
        /// </summary>
        public async Task<DateTime> GetApplicationInstanceCertificateExpiryDateAsync()
        {
            if (ApplicationConfiguration == null)
            {
                _ = await LoadApplicationConfigurationAsync(true).ConfigureAwait(false);
            }

            ApplicationConfiguration configuration = ApplicationConfiguration;

            // find the existing certificate.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate ?? throw ServiceResultException.Create(StatusCodes.BadConfigurationError,
                    "Configuration file does not specify a certificate.");

            // reload the certificate from disk in the cache.
            ICertificatePasswordProvider passwordProvider = configuration.SecurityConfiguration.CertificatePasswordProvider;
            _ = await configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKeyEx(passwordProvider).ConfigureAwait(false);

            // load the certificate
            X509Certificate2 certificate = await id.Find(true).ConfigureAwait(false);

            // check that it is ok.
            if (certificate != null)
            {
                return Opc.Ua.X509Utils.GetCertificateExpiryDate(certificate);
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Checks for a valid application instance certificate.
        /// </summary>
        /// <param name="silent">if set to <c>true</c> no dialogs will be displayed.</param>
        /// <param name="minimumKeySize">Minimum size of the key.</param>
        public Task<bool> CheckApplicationInstanceCertificateAsync(
            bool silent,
            ushort minimumKeySize)
        {
            return CheckApplicationInstanceCertificateAsync(silent, minimumKeySize, CertificateFactory.DefaultLifeTime);
        }

        /// <summary>
        /// Delete the application certificate.
        /// </summary>
        public async Task DeleteApplicationInstanceCertificateAsync(CancellationToken ct = default)
        {
            if (ApplicationConfiguration == null)
            {
                throw new ArgumentException("Missing configuration.");
            }

            await DeleteApplicationInstanceCertificateAsync(ApplicationConfiguration, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks for a valid application instance certificate.
        /// </summary>
        /// <param name="silent">if set to <c>true</c> no dialogs will be displayed.</param>
        /// <param name="minimumKeySize">Minimum size of the key.</param>
        /// <param name="lifeTimeInMonths">The lifetime in months.</param>
        /// <param name="ct">The cancellation token.</param>
        public async Task<bool> CheckApplicationInstanceCertificateAsync(
            bool silent,
            ushort minimumKeySize,
            ushort lifeTimeInMonths,
            CancellationToken ct = default)
        {
            LogInfo("Checking application instance certificate.");

            if (ApplicationConfiguration == null)
            {
                _ = await LoadApplicationConfigurationAsync(silent).ConfigureAwait(false);
            }

            ApplicationConfiguration configuration = ApplicationConfiguration;

            // find the existing certificate.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate ?? throw ServiceResultException.Create(StatusCodes.BadConfigurationError,
                    "Configuration file does not specify a certificate.");

            // reload the certificate from disk in the cache.
            ICertificatePasswordProvider passwordProvider = configuration.SecurityConfiguration.CertificatePasswordProvider;
            _ = await configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKeyEx(passwordProvider).ConfigureAwait(false);

            // load the certificate
            X509Certificate2 certificate = await id.Find(true).ConfigureAwait(false);

            // check that it is ok.
            if (certificate != null)
            {
                LogCertificate("Check certificate:", certificate);
                var certificateValid = await CheckApplicationInstanceCertificateAsync(configuration, certificate, silent, minimumKeySize, ct).ConfigureAwait(false);

                if (!certificateValid)
                {
                    var message = new StringBuilder();
                    _ = message.AppendLine("The certificate with subject {0} in the configuration is invalid.");
                    _ = message.AppendLine(" Please update or delete the certificate from this location:");
                    _ = message.AppendLine(" {1}");
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError,
                        message.ToString(), id.SubjectName, ReplaceSpecialFolderNames(id.StorePath)
                        );
                }
            }
            else
            {
                // check for missing private key.
                certificate = await id.Find(false).ConfigureAwait(false);

                if (certificate != null)
                {
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError,
                        "Cannot access certificate private key. Subject={0}", certificate.Subject);
                }

                // check for missing thumbprint.
                if (!string.IsNullOrEmpty(id.Thumbprint))
                {
                    if (!string.IsNullOrEmpty(id.SubjectName))
                    {
                        var id2 = new CertificateIdentifier {
                            StoreType = id.StoreType,
                            StorePath = id.StorePath,
                            SubjectName = id.SubjectName
                        };
                        certificate = await id2.Find(true).ConfigureAwait(false);
                    }

                    if (certificate != null)
                    {
                        var message = new StringBuilder();
                        _ = message.AppendLine("Thumbprint was explicitly specified in the configuration.");
                        _ = message.AppendLine("Another certificate with the same subject name was found.");
                        _ = message.AppendLine("Use it instead?");
                        _ = message.AppendLine("Requested: {0}");
                        _ = message.AppendLine("Found: {1}");
                        if (!await ApproveMessageAsync(Format(message.ToString(), id.SubjectName, certificate.Subject), silent).ConfigureAwait(false))
                        {
                            throw ServiceResultException.Create(StatusCodes.BadConfigurationError,
                                message.ToString(), id.SubjectName, certificate.Subject);
                        }
                    }
                    else
                    {
                        var message = new StringBuilder();
                        _ = message.AppendLine("Thumbprint was explicitly specified in the configuration.");
                        _ = message.AppendLine("Cannot generate a new certificate.");
                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message.ToString());
                    }
                }
            }

            if (certificate == null)
            {
                if (!DisableCertificateAutoCreation)
                {
                    certificate = await CreateApplicationInstanceCertificateAsync(configuration,
                        minimumKeySize, lifeTimeInMonths, ct).ConfigureAwait(false);
                }
                else
                {
                    LogWarning("Application Instance certificate auto creation is disabled.");
                }

                if (certificate == null)
                {
                    var message = new StringBuilder();
                    _ = message.AppendLine("There is no cert with subject {0} in the configuration.");
                    _ = message.AppendLine(" Please generate a cert for your application,");
                    _ = message.AppendLine(" then copy the new cert to this location:");
                    _ = message.AppendLine(" {1}");
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError,
                        message.ToString(), id.SubjectName, id.StorePath
                        );
                }
            }
            else
            {
                if (configuration.SecurityConfiguration.AddAppCertToTrustedStore)
                {
                    // ensure it is trusted.
                    await AddToTrustedStoreAsync(configuration, certificate, ct).ConfigureAwait(false);
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a Certificate to the Trusted Store of the Application, needed e.g. for the GDS to trust it´s own CA
        /// </summary>
        /// <param name="certificate">The certificate to add to the store</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns></returns>
        public async Task AddOwnCertificateToTrustedStoreAsync(X509Certificate2 certificate, CancellationToken ct)
        {
            await AddToTrustedStoreAsync(ApplicationConfiguration, certificate, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Helper to suppress errors which are allowed for the application certificate validation.
        /// </summary>
        private class CertValidationSuppressibleStatusCodes
        {
            public StatusCode[] ApprovedCodes { get; }

            public CertValidationSuppressibleStatusCodes(StatusCode[] approvedCodes)
            {
                ApprovedCodes = approvedCodes;
            }

            public void OnCertificateValidation(object sender, CertificateValidationEventArgs e)
            {
                if (ApprovedCodes.Contains(e.Error.StatusCode))
                {
                    LogWarning("Application Certificate Validation suppressed {0}", e.Error.StatusCode);
                    e.Accept = true;
                }
            }
        }

        /// <summary>
        /// Creates an application instance certificate if one does not already exist.
        /// </summary>
        private static async Task<bool> CheckApplicationInstanceCertificateAsync(
            ApplicationConfiguration configuration,
            X509Certificate2 certificate,
            bool silent,
            ushort minimumKeySize,
            CancellationToken ct)
        {
            if (certificate == null)
            {
                return false;
            }

            // set suppressible errors
            var certValidator = new CertValidationSuppressibleStatusCodes(
                new StatusCode[] {
                    StatusCodes.BadCertificateUntrusted,
                    StatusCodes.BadCertificateTimeInvalid,
                    StatusCodes.BadCertificateIssuerTimeInvalid,
                    StatusCodes.BadCertificateHostNameInvalid,
                    StatusCodes.BadCertificateRevocationUnknown,
                    StatusCodes.BadCertificateIssuerRevocationUnknown,
                });

            LogCertificate("Check application instance certificate.", certificate);

            try
            {
                // validate certificate.
                configuration.CertificateValidator.CertificateValidation += certValidator.OnCertificateValidation;
                await configuration.CertificateValidator.ValidateAsync(certificate.HasPrivateKey ? new X509Certificate2(certificate.RawData) : certificate, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var message = Format(
                    "Error validating certificate. Exception: {0}. Use certificate anyway?", ex.Message);
                if (!await ApproveMessageAsync(message, silent).ConfigureAwait(false))
                {
                    return false;
                }
            }
            finally
            {
                configuration.CertificateValidator.CertificateValidation -= certValidator.OnCertificateValidation;
            }

            // check key size.
            var keySize = X509Utils.GetRSAPublicKeySize(certificate);
            if (minimumKeySize > keySize)
            {
                var message = Format(
                    "The key size ({0}) in the certificate is less than the minimum allowed ({1}). Use certificate anyway?",
                    keySize,
                    minimumKeySize);

                if (!await ApproveMessageAsync(message, silent).ConfigureAwait(false))
                {
                    return false;
                }
            }

            // check domains.
            if (configuration.ApplicationType != ApplicationType.Client)
            {
                if (!await CheckDomainsInCertificateAsync(configuration, certificate, silent, ct).ConfigureAwait(false))
                {
                    return false;
                }
            }

            // check uri.
            var applicationUri = X509Utils.GetApplicationUriFromCertificate(certificate);

            if (string.IsNullOrEmpty(applicationUri))
            {
                var message = "The Application URI could not be read from the certificate. Use certificate anyway?";
                if (!await ApproveMessageAsync(message, silent).ConfigureAwait(false))
                {
                    return false;
                }
            }
            else if (!configuration.ApplicationUri.Equals(applicationUri, StringComparison.Ordinal))
            {
                LogInfo("Updated the ApplicationUri: {0} --> {1}", configuration.ApplicationUri, applicationUri);
                configuration.ApplicationUri = applicationUri;
            }

            LogInfo("Using the ApplicationUri: {0}", applicationUri);

            // update configuration.
            configuration.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;

            return true;
        }

        /// <summary>
        /// Checks that the domains in the server addresses match the domains in the certificates.
        /// </summary>
        private static async Task<bool> CheckDomainsInCertificateAsync(
            ApplicationConfiguration configuration,
            X509Certificate2 certificate,
            bool silent,
            CancellationToken ct)
        {
            LogInfo("Check domains in certificate.");

            var valid = true;
            IList<string> serverDomainNames = configuration.GetServerDomainNames();
            IList<string> certificateDomainNames = X509Utils.GetDomainsFromCertificate(certificate);

            LogInfo("Server Domain names:");
            foreach (var name in serverDomainNames)
            {
                LogInfo(" {0}", name);
            }

            LogInfo("Certificate Domain names:");
            foreach (var name in certificateDomainNames)
            {
                LogInfo(" {0}", name);
            }

            // get computer name.
            var computerName = GetHostName();

            // get IP addresses.
            IPAddress[] addresses = null;

            foreach (var serverDomainName in serverDomainNames)
            {
                if (FindStringIgnoreCase(certificateDomainNames, serverDomainName))
                {
                    continue;
                }

                if (string.Equals(serverDomainName, "localhost", StringComparison.OrdinalIgnoreCase))
                {
                    if (FindStringIgnoreCase(certificateDomainNames, computerName))
                    {
                        continue;
                    }

                    // check for aliases.
                    var found = false;

                    // get IP addresses only if necessary.
                    if (addresses == null)
                    {
                        addresses = await GetHostAddressesAsync(computerName).ConfigureAwait(false);
                    }

                    // check for ip addresses.
                    foreach (IPAddress address in addresses)
                    {
                        if (FindStringIgnoreCase(certificateDomainNames, address.ToString()))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }
                }

                var message = Format(
                    "The server is configured to use domain '{0}' which does not appear in the certificate. Use certificate anyway?",
                    serverDomainName);

                valid = false;

                if (await ApproveMessageAsync(message, silent).ConfigureAwait(false))
                {
                    valid = true;
                    continue;
                }

                break;
            }

            return valid;
        }

        /// <summary>
        /// Creates the application instance certificate.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="keySize">Size of the key.</param>
        /// <param name="lifeTimeInMonths">The lifetime in months.</param>
        /// <param name="ct"></param>
        /// <returns>The new certificate</returns>
        private static async Task<X509Certificate2> CreateApplicationInstanceCertificateAsync(
            ApplicationConfiguration configuration,
            ushort keySize,
            ushort lifeTimeInMonths,
            CancellationToken ct)
        {
            // delete any existing certificate.
            await DeleteApplicationInstanceCertificateAsync(configuration, ct).ConfigureAwait(false);

            LogInfo("Creating application instance certificate.");

            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            // get the domains from the configuration file.
            IList<string> serverDomainNames = configuration.GetServerDomainNames();

            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(GetHostName());
            }

            // ensure the certificate store directory exists.
            if (id.StoreType == CertificateStoreType.Directory)
            {
                _ = GetAbsoluteDirectoryPath(id.StorePath, true, true, true);
            }

            ICertificatePasswordProvider passwordProvider = configuration.SecurityConfiguration.CertificatePasswordProvider;
            X509Certificate2 certificate = CertificateFactory.CreateCertificate(
                configuration.ApplicationUri,
                configuration.ApplicationName,
                id.SubjectName,
                serverDomainNames)
                .SetLifeTime(lifeTimeInMonths)
                .SetRSAKeySize(keySize)
                .CreateForRSA();

            // need id for password provider
            id.Certificate = certificate;
            _ = await certificate.AddToStoreAsync(
                    id.StoreType,
                id.StorePath,
                passwordProvider?.GetPassword(id),
                ct).ConfigureAwait(false);

            // ensure the certificate is trusted.
            if (configuration.SecurityConfiguration.AddAppCertToTrustedStore)
            {
                await AddToTrustedStoreAsync(configuration, certificate, ct).ConfigureAwait(false);
            }

            // reload the certificate from disk.
            id.Certificate = await configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKeyEx(passwordProvider).ConfigureAwait(false);

            await configuration.CertificateValidator.Update(configuration.SecurityConfiguration).ConfigureAwait(false);

            LogCertificate("Certificate created for {0}.", certificate, configuration.ApplicationUri);

            // do not dispose temp cert, or X509Store certs become unusable

            return id.Certificate;
        }

        /// <summary>
        /// Deletes an existing application instance certificate.
        /// </summary>
        /// <param name="configuration">The configuration instance that stores the configurable information for a UA application.</param>
        /// <param name="ct"></param>
        private static async Task DeleteApplicationInstanceCertificateAsync(ApplicationConfiguration configuration, CancellationToken ct)
        {
            // create a default certificate id none specified.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                return;
            }

            // delete certificate and private key.
            X509Certificate2 certificate = await id.Find().ConfigureAwait(false);
            if (certificate != null)
            {
                LogCertificate(TraceMasks.Security, "Deleting application instance certificate and private key.", certificate);
            }

            // delete trusted peer certificate.
            if (configuration.SecurityConfiguration != null &&
                configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                var thumbprint = id.Thumbprint;

                if (certificate != null)
                {
                    thumbprint = certificate.Thumbprint;
                }

                if (!string.IsNullOrEmpty(thumbprint))
                {
                    using (ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
                    {
                        var deleted = await store.Delete(thumbprint).ConfigureAwait(false);
                        if (deleted)
                        {
                            LogInfo(TraceMasks.Security, "Application Instance Certificate [{0}] deleted from trusted store.", thumbprint);
                        }
                    }
                }
            }

            // delete certificate and private key from owner store.
            if (certificate != null)
            {
                using (ICertificateStore store = id.OpenStore())
                {
                    var deleted = await store.Delete(certificate.Thumbprint).ConfigureAwait(false);
                    if (deleted)
                    {
                        LogCertificate(TraceMasks.Security, "Application certificate and private key deleted.", certificate);
                    }
                }
            }

            // erase the memory copy of the deleted certificate
            id.Certificate = null;
        }

        /// <summary>
        /// Adds the certificate to the Trusted Certificate Store
        /// </summary>
        /// <param name="configuration">The application's configuration which specifies the location of the TrustedStore.</param>
        /// <param name="certificate">The certificate to register.</param>
        /// <param name="ct">The cancellation token.</param>
        private static async Task AddToTrustedStoreAsync(ApplicationConfiguration configuration, X509Certificate2 certificate, CancellationToken ct)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            string storePath = null;

            if (configuration != null && configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                storePath = configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath;
            }

            if (string.IsNullOrEmpty(storePath))
            {
                LogWarning("WARNING: Trusted peer store not specified.");
                return;
            }

            try
            {
                ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();

                if (store == null)
                {
                    LogWarning("Could not open trusted peer store.");
                    return;
                }

                try
                {
                    // check if it already exists.
                    X509Certificate2Collection existingCertificates = await store.FindByThumbprint(certificate.Thumbprint).ConfigureAwait(false);

                    if (existingCertificates.Count > 0)
                    {
                        return;
                    }

                    LogCertificate("Adding application certificate to trusted peer store.", certificate);

                    List<string> subjectName = X509Utils.ParseDistinguishedName(certificate.Subject);

                    // check for old certificate.
                    X509Certificate2Collection certificates = await store.Enumerate().ConfigureAwait(false);

                    foreach (X509Certificate2 cert in certificates)
                    {
                        if (X509Utils.CompareDistinguishedName(cert, subjectName))
                        {
                            if (cert.Thumbprint == certificate.Thumbprint)
                            {
                                return;
                            }

                            LogCertificate("Delete Certificate from trusted store.", certificate);

                            _ = await store.Delete(cert.Thumbprint).ConfigureAwait(false);
                            break;
                        }
                    }

                    // add new certificate.
                    var publicKey = new X509Certificate2(certificate.RawData);
                    await store.Add(publicKey).ConfigureAwait(false);

                    LogInfo("Added application certificate to trusted peer store.");
                }
                finally
                {
                    store.Close();
                }
            }
            catch (Exception e)
            {
                LogError(e, "Could not add certificate to trusted peer store.");
            }
        }

        /// <summary>
        /// Show a message for approval and return result.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="silent"></param>
        /// <returns>True if approved, false otherwise.</returns>
        private static async Task<bool> ApproveMessageAsync(string message, bool silent)
        {
            if (!silent && MessageDlg != null)
            {
                MessageDlg.Message(message, true);
                return await MessageDlg.ShowAsync().ConfigureAwait(false);
            }
            else
            {
                LogError(message);
                return false;
            }
        }
        #endregion
    }
}
