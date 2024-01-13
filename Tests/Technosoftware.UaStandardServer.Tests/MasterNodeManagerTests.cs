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
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using Opc.Ua;
using Technosoftware.UaServer;
using Technosoftware.UaServer.NodeManager;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    /// <summary>
    /// Test <see cref="MasterNodeManager"/>
    /// </summary>
    [TestFixture, Category("MasterNodeManager")]
    [SetCulture("en-us"), SetUICulture("en-us")]
    [Parallelizable]
    public class MasterNodeManagerTests
    {
        #region Test Methods
        /// <summary>
        /// Test for registering a namespace manager for a namespace
        /// not contained in the server's namespace table
        /// </summary>
        [Test]
        public async Task RegisterNamespaceManagerNewNamespace()
        {
            var fixture = new ServerFixture<UaStandardServer>();

            try
            {
                //-- Arrange
                const string ns = "http://test.org/UA/Data/";

                var nodeManager = new Mock<IUaNodeManager>();
                nodeManager.Setup(x => x.NamespaceUris).Returns(new List<string>());

                //-- Act
                var server = await fixture.StartAsync(TestContext.Out).ConfigureAwait(false);
                var sut = new MasterNodeManager(
                    server.CurrentInstance,
                    fixture.Config,
                    null,
                    nodeManager.Object);
                sut.RegisterNamespaceManager(ns, nodeManager.Object);

                //-- Assert
                Assert.Contains(ns, server.CurrentInstance.NamespaceUris.ToArray());
                var registeredManagers = sut.NamespaceManagers[server.CurrentInstance.NamespaceUris.GetIndex(ns)];
                Assert.AreEqual(1, registeredManagers.Length);
                Assert.Contains(nodeManager.Object, registeredManagers);
            }
            finally
            {
                await fixture.StopAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Test for registering a namespace manager for a namespace
        /// contained in the server's namespace table
        /// </summary>
        [Test]
        public async Task RegisterNamespaceManagerExistingNamespace()
        {
            var fixture = new ServerFixture<UaStandardServer>();

            try
            {
                //-- Arrange
                const string ns = "http://test.org/UA/Data/";
                var namespaceUris = new List<string> { ns };

                var originalNodeManager = new Mock<IUaBaseNodeManager>();
                originalNodeManager.Setup(x => x.NamespaceUris).Returns(namespaceUris);

                var newNodeManager = new Mock<IUaBaseNodeManager>();
                newNodeManager.Setup(x => x.NamespaceUris).Returns(namespaceUris);

                //-- Act
                var server = await fixture.StartAsync(TestContext.Out).ConfigureAwait(false);
                var sut = new MasterNodeManager(
                    server.CurrentInstance,
                    fixture.Config,
                    null,
                    originalNodeManager.Object);
                sut.RegisterNamespaceManager(ns, newNodeManager.Object);

                //-- Assert
                Assert.Contains(ns, server.CurrentInstance.NamespaceUris.ToArray());
                var registeredManagers = sut.NamespaceManagers[server.CurrentInstance.NamespaceUris.GetIndex(ns)];
                Assert.AreEqual(2, registeredManagers.Length);
                Assert.Contains(originalNodeManager.Object, registeredManagers);
                Assert.Contains(newNodeManager.Object, registeredManagers);
            }
            finally
            {
                await fixture.StopAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Test for unregistering a namespace manager which had previously
        /// been registered
        /// </summary>
        [Test]
        [TestCase(3, 0)]
        [TestCase(3, 1)]
        [TestCase(3, 2)]
        public async Task UnregisterNamespaceManagerInCollection(int totalManagers, int indexToRemove)
        {
            var fixture = new ServerFixture<UaStandardServer>();

            try
            {
                //-- Arrange
                const string ns = "http://test.org/UA/Data/";
                var namespaceUris = new List<string> { ns };

                var additionalManagers = new IUaBaseNodeManager[totalManagers];
                for (int ii = 0; ii < totalManagers; ii++)
                {
                    var nodeManager = new Mock<IUaBaseNodeManager>();
                    nodeManager.Setup(x => x.NamespaceUris).Returns(namespaceUris);

                    additionalManagers[ii] = nodeManager.Object;
                }

                var nodeManagerToRemove = additionalManagers[indexToRemove];

                //-- Act
                var server = await fixture.StartAsync(TestContext.Out).ConfigureAwait(false);
                var sut = new MasterNodeManager(
                    server.CurrentInstance,
                    fixture.Config,
                    null,
                    additionalManagers);
                var result = sut.UnregisterNamespaceManager(ns, nodeManagerToRemove);

                //-- Assert
                Assert.IsTrue(result);
                Assert.Contains(ns, server.CurrentInstance.NamespaceUris.ToArray());
                var registeredManagers = sut.NamespaceManagers[server.CurrentInstance.NamespaceUris.GetIndex(ns)];
                Assert.AreEqual(totalManagers - 1, registeredManagers.Length);
                Assert.That(registeredManagers, Has.No.Member(nodeManagerToRemove));
            }
            finally
            {
                await fixture.StopAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Test for unregistering a namespace manager which had not
        /// previously been registered
        /// </summary>
        [Test]
        public async Task UnregisterNamespaceManagerNotInCollection()
        {
            var fixture = new ServerFixture<UaStandardServer>();

            try
            {
                //-- Arrange
                const string ns = "http://test.org/UA/Data/";
                var namespaceUris = new List<string> { ns };

                var firstNodeManager = new Mock<IUaBaseNodeManager>();
                firstNodeManager.Setup(x => x.NamespaceUris).Returns(namespaceUris);

                var secondNodeManager = new Mock<IUaBaseNodeManager>();
                secondNodeManager.Setup(x => x.NamespaceUris).Returns(namespaceUris);

                var thirdNodeManager = new Mock<IUaBaseNodeManager>();
                thirdNodeManager.Setup(x => x.NamespaceUris).Returns(namespaceUris);

                //-- Act
                var server = await fixture.StartAsync(TestContext.Out).ConfigureAwait(false);
                var sut = new MasterNodeManager(
                    server.CurrentInstance,
                    fixture.Config,
                    null,
                    firstNodeManager.Object,
                    // Do not add the secondNodeManager to additionalManagers
                    thirdNodeManager.Object);
                var result = sut.UnregisterNamespaceManager(ns, secondNodeManager.Object);

                //-- Assert
                Assert.IsFalse(result);
                Assert.Contains(ns, server.CurrentInstance.NamespaceUris.ToArray());
                var registeredManagers = sut.NamespaceManagers[server.CurrentInstance.NamespaceUris.GetIndex(ns)];
                Assert.AreEqual(2, registeredManagers.Length);
                Assert.Contains(firstNodeManager.Object, registeredManagers);
                Assert.Contains(thirdNodeManager.Object, registeredManagers);
            }
            finally
            {
                await fixture.StopAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Test for unregistering a namespace manager which had not
        /// previously been registered and is for a namespace
        /// which is unknown by the server
        /// </summary>
        [Test]
        public async Task UnregisterNamespaceManagerUnknownNamespace()
        {
            var fixture = new ServerFixture<UaStandardServer>();

            try
            {
                //-- Arrange
                const string originalNs = "http://test.org/UA/Data/";

                var originalNodeManager = new Mock<IUaBaseNodeManager>();
                originalNodeManager.Setup(x => x.NamespaceUris).Returns(new List<string> { originalNs });

                const string newNs = "http://test.org/UA/Data/Instance";
                var newNodeManager = new Mock<IUaBaseNodeManager>();
                newNodeManager.Setup(x => x.NamespaceUris).Returns(new List<string> { originalNs, newNs });

                //-- Act
                var server = await fixture.StartAsync(TestContext.Out).ConfigureAwait(false);
                var sut = new MasterNodeManager(
                    server.CurrentInstance,
                    fixture.Config,
                    null,
                    originalNodeManager.Object);
                var result = sut.UnregisterNamespaceManager(newNs, newNodeManager.Object);

                //-- Assert
                Assert.IsFalse(result);
                Assert.That(server.CurrentInstance.NamespaceUris.ToArray(), Has.No.Member(newNs));

                Assert.Contains(originalNs, server.CurrentInstance.NamespaceUris.ToArray());
                var registeredManagers = sut.NamespaceManagers[server.CurrentInstance.NamespaceUris.GetIndex(originalNs)];
                Assert.AreEqual(1, registeredManagers.Length);
                Assert.Contains(originalNodeManager.Object, registeredManagers);
            }
            finally
            {
                await fixture.StopAsync().ConfigureAwait(false);
            }
        }
        #endregion
    }
}
