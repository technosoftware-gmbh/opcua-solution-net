#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaStandardServer.Tests;
#endregion

namespace Technosoftware.UaClient.Tests
{
    /// <summary>
    /// Client fixture for tests.
    /// </summary>
    public class ClientFixture
    {
        private const uint DefaultOperationLimits = 5000;
        private NUnitTraceLogger traceLogger_;
        public ApplicationConfiguration Config { get; private set; }
        public ConfiguredEndpoint Endpoint { get; private set; }
        public string EndpointUrl { get; private set; }
        public string ReverseConnectUri { get; private set; }
        public ReverseConnectManager ReverseConnectManager { get; private set; }
        public uint SessionTimeout { get; set; } = 10000;
        public int OperationTimeout { get; set; } = 10000;
        public int TraceMasks { get; set; } = Utils.TraceMasks.Error | Utils.TraceMasks.StackTrace | Utils.TraceMasks.Security | Utils.TraceMasks.Information;

        public IUaSessionFactory SessionFactory { get; } = new DefaultSessionFactory();

        #region Public Methods
        /// <summary>
        /// Load the default client configuration.
        /// </summary>
        public async Task LoadClientConfiguration(string pkiRoot = null, string clientName = "TestClient")
        {
            ApplicationInstance application = new ApplicationInstance {
                ApplicationName = clientName
            };

            pkiRoot = pkiRoot ?? Path.Combine("%LocalApplicationData%", "OPC", "pki");

            // build the application configuration.
            Config = await application
                .CreateApplicationConfigurationManager(
                    "urn:localhost:technosoftware.com:" + clientName,
                    "http://technosoftware.com/" + clientName)
                .AsClient()
                .SetClientOperationLimits(new OperationLimits {
                    MaxNodesPerBrowse = DefaultOperationLimits,
                    MaxNodesPerRead = DefaultOperationLimits,
                    MaxMonitoredItemsPerCall = DefaultOperationLimits,
                    MaxNodesPerWrite = DefaultOperationLimits
                })
                .AddSecurityConfiguration(
                    "CN=" + clientName + ", O=Technosoftware GmbH, DC=localhost",
                    pkiRoot)
                .SetAutoAcceptUntrustedCertificates(true)
                .SetRejectSHA1SignedCertificates(false)
                .SetMinimumCertificateKeySize(1024)
                .SetOutputFilePath(Path.Combine(pkiRoot, "Logs", "Technosoftware.UaClient.Tests.log.txt"))
                .SetTraceMasks(TraceMasks)
                .CreateAsync().ConfigureAwait(false);

            // check the application certificate.
            bool haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(true, 0).ConfigureAwait(false);
            if (!haveAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }

            ReverseConnectManager = new ReverseConnectManager();
        }

        /// <summary>
        /// Start a host for reverse connections on random port.
        /// </summary>
        public async Task StartReverseConnectHost()
        {
            Random random = new Random();
            int testPort = ServerFixtureUtils.GetNextFreeIPPort();
            bool retryStartServer = false;
            int serverStartRetries = 25;
            do
            {
                try
                {
                    var reverseConnectUri = new Uri("opc.tcp://localhost:" + testPort);
                    ReverseConnectManager.AddEndpoint(reverseConnectUri);
                    ReverseConnectManager.StartService(Config);
                    ReverseConnectUri = reverseConnectUri.ToString();
                }
                catch (ServiceResultException sre)
                {
                    serverStartRetries--;
                    if (serverStartRetries == 0 ||
                        sre.StatusCode != StatusCodes.BadNoCommunication)
                    {
                        throw;
                    }
                    testPort = random.Next(ServerFixtureUtils.MinTestPort, ServerFixtureUtils.MaxTestPort);
                    retryStartServer = true;
                }
                await Task.Delay(random.Next(100, 1000)).ConfigureAwait(false);
            } while (retryStartServer);
        }

        /// <summary>
        /// Connects the specified endpoint URL.
        /// </summary>
        /// <param name="endpointUrl">The endpoint URL.</param>
        public async Task<IUaSession> Connect(string endpointUrl)
        {
            if (String.IsNullOrEmpty(endpointUrl))
            {
                throw new ArgumentNullException(nameof(endpointUrl));
            }

            if (!Uri.IsWellFormedUriString(endpointUrl, UriKind.Absolute))
            {
                throw new ArgumentException(endpointUrl + " is not a valid URL.", nameof(endpointUrl));
            }

            bool serverHalted;
            do
            {
                serverHalted = false;
                try
                {
                    EndpointDescription endpointDescription = Discover.SelectEndpoint(Config, endpointUrl, true);
                    EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(Config);
                    ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                    return await ConnectAsync(endpoint).ConfigureAwait(false);
                }
                catch (ServiceResultException e)
                {
                    if (e.StatusCode == StatusCodes.BadServerHalted)
                    {
                        serverHalted = true;
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (serverHalted);

            throw new ServiceResultException(StatusCodes.BadNoCommunication);
        }

        /// <summary>
        /// Connects the url endpoint with specified security profile.
        /// </summary>
        public async Task<IUaSession> ConnectAsync(Uri url, string securityProfile, EndpointDescriptionCollection endpoints = null, IUserIdentity userIdentity = null)
        {
            return await ConnectAsync(await GetEndpointAsync(url, securityProfile, endpoints).ConfigureAwait(false), userIdentity).ConfigureAwait(false);
        }

        /// <summary>
        /// Connects the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The configured endpoint.</param>
        public async Task<IUaSession> ConnectAsync(ConfiguredEndpoint endpoint, IUserIdentity userIdentity = null)
        {
            if (endpoint == null)
            {
                endpoint = Endpoint;
                if (endpoint == null)
                {
                    throw new ArgumentNullException(nameof(endpoint));
                }
            }

            var session = await SessionFactory.CreateAsync(
                Config, endpoint, false, false,
                Config.ApplicationName, SessionTimeout, userIdentity, null).ConfigureAwait(false);

            Endpoint = session.ConfiguredEndpoint;

            session.SessionKeepAliveEvent += Session_KeepAlive;

            session.ReturnDiagnostics = DiagnosticsMasks.SymbolicIdAndText;
            EndpointUrl = session.ConfiguredEndpoint.EndpointUrl.ToString();

            return session;
        }

        /// <summary>
        /// Create a channel using the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The configured endpoint</param>
        /// <returns></returns>
        public async Task<ITransportChannel> CreateChannelAsync(ConfiguredEndpoint endpoint)
        {
            return await Session.CreateChannelAsync(Config, null, endpoint, true, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a session using the specified channel.
        /// </summary>
        /// <param name="channel">The channel to use</param>
        /// <param name="endpoint">The configured endpoint</param>
        /// <returns></returns>
        public Session CreateSession(ITransportChannel channel, ConfiguredEndpoint endpoint)
        {
            return Session.Create(Config, channel, endpoint, null);
        }

        /// <summary>
        /// Get configured endpoint from url with security profile.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="securityPolicy"></param>
        /// <param name="endpoints"></param>
        public async Task<ConfiguredEndpoint> GetEndpointAsync(
            Uri url,
            string securityPolicy,
            EndpointDescriptionCollection endpoints = null)
        {
            if (endpoints == null)
            {
                endpoints = await GetEndpoints(url).ConfigureAwait(false);
            }
            var endpointDescription = SelectEndpoint(Config, endpoints, url, securityPolicy);
            if (endpointDescription == null)
            {
                Assert.Ignore("The endpoint is not supported by the server.");
            }
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(Config);
            endpointConfiguration.OperationTimeout = OperationTimeout;
            return new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
        }

        /// <summary>
        /// Select a security endpoint from description.
        /// </summary>
        public static EndpointDescription SelectEndpoint(
            ApplicationConfiguration configuration,
            EndpointDescriptionCollection endpoints,
            Uri url,
            string securityPolicy)
        {
            EndpointDescription selectedEndpoint = null;

            // select the best endpoint to use based on the selected URL and the UseSecurity checkbox. 
            foreach (var endpoint in endpoints)
            {
                // check for a match on the URL scheme.
                if (endpoint.EndpointUrl.StartsWith(url.Scheme))
                {
                    // skip unsupported security policies
                    if (SecurityPolicies.GetDisplayName(endpoint.SecurityPolicyUri) == null)
                    {
                        continue;
                    }

                    // pick the first available endpoint by default.
                    if (selectedEndpoint == null &&
                        securityPolicy.Equals(endpoint.SecurityPolicyUri, StringComparison.Ordinal))
                    {
                        selectedEndpoint = endpoint;
                        continue;
                    }

                    if (selectedEndpoint?.SecurityMode < endpoint.SecurityMode &&
                        securityPolicy.Equals(endpoint.SecurityPolicyUri, StringComparison.Ordinal))
                    {
                        selectedEndpoint = endpoint;
                    }
                }
            }
            // return the selected endpoint.
            return selectedEndpoint;
        }

        /// <summary>
        /// Get endpoints from discovery endpoint.
        /// </summary>
        /// <param name="url">The url of the discovery endpoint.</param>
        public async Task<EndpointDescriptionCollection> GetEndpoints(Uri url)
        {
            var endpointConfiguration = EndpointConfiguration.Create();
            endpointConfiguration.OperationTimeout = OperationTimeout;

            using (var client = DiscoveryClient.Create(url, endpointConfiguration))
            {
                return await client.GetEndpointsAsync(null).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Connect the nunit writer with the logger.
        /// </summary>
        public void SetTraceOutput(TextWriter writer)
        {
            if (traceLogger_ == null)
            {
                traceLogger_ = NUnitTraceLogger.Create(writer, Config, TraceMasks);
            }
            else
            {
                traceLogger_.SetWriter(writer);
            }
        }
        #endregion

        #region Private Methods
        private void Session_KeepAlive(object sender, SessionKeepAliveEventArgs e)
        {
            var session = (Session)sender;
            if (ServiceResult.IsBad(e.Status))
            {
                session?.Dispose();
            }
        }
        #endregion
    }
}
