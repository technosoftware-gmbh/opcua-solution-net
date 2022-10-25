#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

using Opc.Ua;

using Technosoftware.UaStandardServer.Tests;
using Technosoftware.ReferenceServer;
#endregion

namespace Technosoftware.UaClient.Tests
{
    /// <summary>
    /// Test Client Reverse Connect Services.
    /// </summary>
    [TestFixture, Category("Client")]
    [SetCulture("en-us"), SetUICulture("en-us")]
    [NonParallelizable]
    public class ReverseConnectTest : ClientTestFramework
    {
        Uri endpointUrl_;

        #region DataPointSources
        #endregion

        #region Test Setup
        /// <summary>
        /// Setup a server and client fixture.
        /// </summary>
        [OneTimeSetUp]
        public async Task OneTimeSetUpAsync()
        {
            // this test fails on macOS, ignore (TODO)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Ignore("Reverse connect fails on mac OS.");
            }
        
            // pki directory root for test runs. 
            PkiRoot = Path.GetTempPath() + Path.GetRandomFileName();

            // start ref server with reverse connect
            ServerFixture = new ServerFixture<ReferenceServer.ReferenceServer> {
                AutoAccept = true,
                SecurityNone = true,
                ReverseConnectTimeout = MaxTimeout,
                TraceMasks = Utils.TraceMasks.Error | Utils.TraceMasks.Security
            };
            ReferenceServer = await ServerFixture.StartAsync(TestContext.Out, PkiRoot).ConfigureAwait(false);

            // create client
            ClientFixture = new ClientFixture();
            await ClientFixture.LoadClientConfiguration(PkiRoot).ConfigureAwait(false);
            await ClientFixture.StartReverseConnectHost().ConfigureAwait(false);
            endpointUrl_ = new Uri(Utils.ReplaceLocalhost("opc.tcp://localhost:" + ServerFixture.Port.ToString()));
            // start reverse connection
            ReferenceServer.AddReverseConnection(new Uri(ClientFixture.ReverseConnectUri), MaxTimeout);
        }

        /// <summary>
        /// Tear down the Server and the Client.
        /// </summary>
        [OneTimeTearDown]
        public new Task OneTimeTearDownAsync()
        {
            return base.OneTimeTearDownAsync();
        }

        /// <summary>
        /// Test setup.
        /// </summary>
        [SetUp]
        public new Task SetUp()
        {
            return base.SetUp();
        }

        /// <summary>
        /// Test teardown.
        /// </summary>
        [TearDown]
        public new Task TearDown()
        {
            return base.TearDown();
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Get endpoints using a reverse connection.
        /// </summary>
        [Test, Order(100)]
        public async Task GetEndpoints()
        {
            await RequireEndpoints().ConfigureAwait(false);
        }

        /// <summary>
        /// Internal get endpoints which is called with semaphore.
        /// </summary>
        public async Task GetEndpointsInternal()
        {
            var config = ClientFixture.Config;
            ITransportWaitingConnection connection;
            using (var cancellationTokenSource = new CancellationTokenSource(MaxTimeout))
            {
                connection = await ClientFixture.ReverseConnectManager.WaitForConnection(
                    endpointUrl_, null, cancellationTokenSource.Token).ConfigureAwait(false);
                Assert.NotNull(connection, "Failed to get connection.");
            }
            var endpointConfiguration = EndpointConfiguration.Create();
            endpointConfiguration.OperationTimeout = MaxTimeout;
            using (DiscoveryClient client = DiscoveryClient.Create(config, connection, endpointConfiguration))
            {
                Endpoints = client.GetEndpoints(null);
            }
        }

        [Test, Order(200)]
        public async Task SelectEndpoint()
        {
            var config = ClientFixture.Config;
            ITransportWaitingConnection connection;
            using (var cancellationTokenSource = new CancellationTokenSource(MaxTimeout))
            {
                connection = await ClientFixture.ReverseConnectManager.WaitForConnection(
                    endpointUrl_, null, cancellationTokenSource.Token).ConfigureAwait(false);
                Assert.NotNull(connection, "Failed to get connection.");
            }
            var selectedEndpoint = Discover.SelectEndpoint(config, connection, true, MaxTimeout);
            Assert.NotNull(selectedEndpoint);
        }

        [Theory, Order(300)]
        public async Task ReverseConnect(string securityPolicy)
        {
            // ensure endpoints are available
            await RequireEndpoints().ConfigureAwait(false);

            // get a connection
            var config = ClientFixture.Config;
            ITransportWaitingConnection connection;
            using (var cancellationTokenSource = new CancellationTokenSource(MaxTimeout))
            {
                connection = await ClientFixture.ReverseConnectManager.WaitForConnection(
                    endpointUrl_, null, cancellationTokenSource.Token).ConfigureAwait(false);
                Assert.NotNull(connection, "Failed to get connection.");
            }

            // select the secure endpoint
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var selectedEndpoint = ClientFixture.SelectEndpoint(config, Endpoints, endpointUrl_, securityPolicy);
            Assert.NotNull(selectedEndpoint);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            Assert.NotNull(endpoint);

            // connect
            var session = await Session.CreateAsync(config, connection, endpoint, false, false, "Reverse Connect Client",
                MaxTimeout, new UserIdentity(new AnonymousIdentityToken()), null).ConfigureAwait(false);
            Assert.NotNull(session);

            // default request header
            var requestHeader = new RequestHeader();
            requestHeader.Timestamp = DateTime.UtcNow;
            requestHeader.TimeoutHint = MaxTimeout;

            // Browse
            var clientTestServices = new ClientTestServices(session);
            var referenceDescriptions = CommonTestWorkers.BrowseFullAddressSpaceWorker(clientTestServices, requestHeader);
            Assert.NotNull(referenceDescriptions);

            // close session
            var result = session.Close();
            Assert.NotNull(result);
            session.Dispose();
        }

        [Theory, Order(301)]
        public async Task ReverseConnect2(bool updateBeforeConnect, bool checkDomain)
        {
            string securityPolicy = SecurityPolicies.Basic256Sha256;

            // ensure endpoints are available
            await RequireEndpoints().ConfigureAwait(false);

            // get a connection
            var config = ClientFixture.Config;

            // select the secure endpoint
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var selectedEndpoint = ClientFixture.SelectEndpoint(config, Endpoints, endpointUrl_, securityPolicy);
            Assert.NotNull(selectedEndpoint);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            Assert.NotNull(endpoint);

            // connect
            var session = await Session.CreateAsync(config, ClientFixture.ReverseConnectManager, endpoint, updateBeforeConnect, checkDomain, "Reverse Connect Client",
                MaxTimeout, new UserIdentity(new AnonymousIdentityToken()), null).ConfigureAwait(false);
            Assert.NotNull(session);

            // header
            var requestHeader = new RequestHeader();
            requestHeader.Timestamp = DateTime.UtcNow;
            requestHeader.TimeoutHint = MaxTimeout;

            // Browse
            var clientTestServices = new ClientTestServices(session);
            var referenceDescriptions = CommonTestWorkers.BrowseFullAddressSpaceWorker(clientTestServices, requestHeader);
            Assert.NotNull(referenceDescriptions);

            // close session
            var result = session.Close();
            Assert.NotNull(result);
            session.Dispose();
        }
        #endregion

        #region Private Methods
        private async Task RequireEndpoints()
        {
            await requiredLock_.WaitAsync().ConfigureAwait(false);
            try
            {
                if (Endpoints == null)
                {
                    await GetEndpointsInternal().ConfigureAwait(false);
                }
            }
            finally
            {
                requiredLock_.Release();
            }
        }
        #endregion

        private SemaphoreSlim requiredLock_ = new SemaphoreSlim(1);
    }
}
