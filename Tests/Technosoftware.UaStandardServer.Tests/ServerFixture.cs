#region Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.IO;
using System.Threading.Tasks;

using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaServer;

using SampleCompany.NodeManagers;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    /// <summary>
    /// Server fixture for testing.
    /// </summary>
    /// <typeparam name="T">A server class T used for testing.</typeparam>
    public class ServerFixture<T> where T : ServerBase, new()
    {
        private NUnitTestLogger<T> traceLogger_;
        public ApplicationInstance Application { get; private set; }
        public ApplicationConfiguration Config { get; private set; }
        public T Server { get; private set; }
        public bool LogConsole { get; set; }
        public bool AutoAccept { get; set; }
        public bool OperationLimits { get; set; }
        public int ReverseConnectTimeout { get; set; }
        public bool AllNodeManagers { get; set; }
        public int TraceMasks { get; set; } = Utils.TraceMasks.Error | Utils.TraceMasks.StackTrace | Utils.TraceMasks.Security | Utils.TraceMasks.Information;
        public bool SecurityNone { get; set; } = false;
        public string UriScheme { get; set; } = Utils.UriSchemeOpcTcp;
        public int Port { get; private set; }

        public async Task LoadConfiguration(string pkiRoot = null)
        {
            Application = new ApplicationInstance {
                ApplicationName = typeof(T).Name,
                ApplicationType = ApplicationType.Server
            };

            // create the application configuration. Use temp path for cert stores.
            pkiRoot = pkiRoot ?? Path.GetTempPath() + Path.GetRandomFileName();
            var endpointUrl = $"{UriScheme}://localhost:0/" + typeof(T).Name;
            var serverConfig = Application.CreateApplicationConfigurationManager(
                "urn:localhost:" + typeof(T).Name,
                "uri:technosoftware.com:" + typeof(T).Name)
                .SetMaxByteStringLength(4 * 1024 * 1024)
                .SetMaxArrayLength(1024 * 1024)
                .AsServer(
                    new string[] {
                    endpointUrl
                });

            if (SecurityNone)
            {
                serverConfig.AddUnsecurePolicyNone();
            }
            if (Utils.IsUriHttpsScheme(endpointUrl))
            {
                serverConfig.AddPolicy(MessageSecurityMode.SignAndEncrypt, SecurityPolicies.Basic256Sha256);
            }
            else if (endpointUrl.StartsWith(Utils.UriSchemeOpcTcp, StringComparison.Ordinal))
            {
                // add deprecated policies for opc.tcp tests
                serverConfig.AddPolicy(MessageSecurityMode.Sign, SecurityPolicies.Basic128Rsa15)
                    .AddPolicy(MessageSecurityMode.Sign, SecurityPolicies.Basic256)
                    .AddPolicy(MessageSecurityMode.SignAndEncrypt, SecurityPolicies.Basic128Rsa15)
                    .AddPolicy(MessageSecurityMode.SignAndEncrypt, SecurityPolicies.Basic256)
                    .AddSignPolicies()
                    .AddSignAndEncryptPolicies();
            }

            if (OperationLimits)
            {
                serverConfig.SetOperationLimits(new OperationLimits() {
                    MaxNodesPerBrowse = 2500,
                    MaxNodesPerRead = 1000,
                    MaxNodesPerWrite = 1000,
                    MaxNodesPerMethodCall = 1000,
                    MaxMonitoredItemsPerCall = 1000,
                    MaxNodesPerHistoryReadData = 1000,
                    MaxNodesPerHistoryReadEvents = 1000,
                    MaxNodesPerHistoryUpdateData = 1000,
                    MaxNodesPerHistoryUpdateEvents = 1000,
                    MaxNodesPerNodeManagement = 1000,
                    MaxNodesPerRegisterNodes = 1000,
                    MaxNodesPerTranslateBrowsePathsToNodeIds = 1000
                });
            }

            serverConfig.SetMaxMessageQueueSize(20);
            serverConfig.SetDiagnosticsEnabled(true);
            serverConfig.SetAuditingEnabled(true);

            if (ReverseConnectTimeout != 0)
            {
                serverConfig.SetReverseConnect(new ReverseConnectServerConfiguration() {
                    ConnectInterval = ReverseConnectTimeout / 4,
                    ConnectTimeout = ReverseConnectTimeout,
                    RejectTimeout = ReverseConnectTimeout / 4
                });
            }

            Config = await serverConfig.AddSecurityConfiguration(
                    "CN=" + typeof(T).Name + ", C=CH, S=Aargau, O=Technosoftware GmbH, DC=localhost",
                    pkiRoot)
                .SetAutoAcceptUntrustedCertificates(AutoAccept)
                .CreateAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Start server fixture on random or fixed port.
        /// </summary>
        public Task<T> StartAsync(TextWriter writer, int port = 0)
        {
            return StartAsync(writer, null, port);
        }

        /// <summary>
        /// Start server fixture on random or fixed port with dedicated PKI.
        /// </summary>
        public async Task<T> StartAsync(TextWriter writer, string pkiRoot, int port = 0)
        {
            Random random = new Random();
            bool retryStartServer = false;
            int testPort = port;
            int serverStartRetries = 1;

            if (Application == null)
            {
                await LoadConfiguration(pkiRoot).ConfigureAwait(false);
            }

            if (port <= 0)
            {
                testPort = ServerFixtureUtils.GetNextFreeIPPort();
                serverStartRetries = 25;
            }

            do
            {
                try
                {
                    await InternalStartServerAsync(writer, testPort).ConfigureAwait(false);
                }
                catch (ServiceResultException sre)
                {
                    if (serverStartRetries <= 0 ||
                        sre.StatusCode != StatusCodes.BadNoCommunication)
                    {
                        throw;
                    }
                    serverStartRetries--;
                    testPort = random.Next(ServerFixtureUtils.MinTestPort, ServerFixtureUtils.MaxTestPort);
                    retryStartServer = true;
                }
                await Task.Delay(random.Next(100, 1000)).ConfigureAwait(false);
            } while (retryStartServer);

            return Server;
        }

        /// <summary>
        /// Create the configuration and start the server.
        /// </summary>
        private async Task InternalStartServerAsync(TextWriter writer, int port)
        {
            Config.ServerConfiguration.BaseAddresses = new StringCollection() {
                $"{UriScheme}://localhost:{port}/{typeof(T).Name}"
            };

            if (writer != null)
            {
                traceLogger_ = NUnitTestLogger<T>.Create(writer, Config, TraceMasks);
            }

            // check the application certificate.
            bool haveAppCertificate = await Application.CheckApplicationInstanceCertificateAsync(
                true, CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime).ConfigureAwait(false);
            if (!haveAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }

            // start the server.
            T server = new T();
            if (AllNodeManagers && server is UaStandardServer standardServer)
            {
                NodeManagerUtils.AddDefaultNodeManagers(standardServer);
            }
            await Application.StartAsync(server).ConfigureAwait(false);
            Server = server;
            Port = port;
        }

        /// <summary>
        /// Connect the nunit writer with the logger.
        /// </summary>
        public void SetTraceOutput(TextWriter writer)
        {
            traceLogger_.SetWriter(writer);
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public Task StopAsync()
        {
            Server?.Stop();
            Server?.Dispose();
            Server = null;
            return Task.Delay(100);
        }
    }
}
