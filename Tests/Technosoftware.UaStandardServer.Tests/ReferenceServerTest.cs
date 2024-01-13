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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NUnit.Framework;

using Opc.Ua;
using Opc.Ua.Test;

using SampleCompany.NodeManagers.Reference;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    /// <summary>
    /// Test Reference Server.
    /// </summary>
    [TestFixture, Category("Server")]
    [SetCulture("en-us"), SetUICulture("en-us")]
    [Parallelizable]
    [MemoryDiagnoser]
    [DisassemblyDiagnoser]
    public class ReferenceServerTests
    {
        const double MaxAge_ = 10000;
        const uint TimeoutHint_ = 10000;
        const uint QueueSize_ = 5;

        ServerFixture<ReferenceServer> fixture_;
        ReferenceServer server_;
        RequestHeader requestHeader_;
        OperationLimits operationLimits_;
        ReferenceDescriptionCollection referenceDescriptions_;
        RandomSource random_;
        DataGenerator generator_;
        bool sessionClosed_;


        #region Test Setup
        /// <summary>
        /// Set up a Server fixture.
        /// </summary>
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // start Ref server
            fixture_ = new ServerFixture<ReferenceServer>() {
                AllNodeManagers = true,
                OperationLimits = true
            };
            server_ = await fixture_.StartAsync(TestContext.Out).ConfigureAwait(false);
        }

        /// <summary>
        /// Tear down the server fixture.
        /// </summary>
        [OneTimeTearDown]
        public async Task OneTimeTearDownAsync()
        {
            await fixture_.StopAsync().ConfigureAwait(false);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Create a session for a test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            fixture_.SetTraceOutput(TestContext.Out);
            requestHeader_ = server_.CreateAndActivateSession(TestContext.CurrentContext.Test.Name);
            requestHeader_.Timestamp = DateTime.UtcNow;
            requestHeader_.TimeoutHint = TimeoutHint_;
            random_ = new RandomSource(999);
            generator_ = new DataGenerator(random_);
        }

        /// <summary>
        /// Tear down the test session.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (!sessionClosed_)
            {
                requestHeader_.Timestamp = DateTime.UtcNow;
                server_.CloseSession(requestHeader_);
                requestHeader_ = null;
            }
        }
        #endregion

        #region Benchmark Setup
        /// <summary>
        /// Set up a Reference Server a session
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            // start Ref server
            fixture_ = new ServerFixture<ReferenceServer>() { AllNodeManagers = true };
            server_ = fixture_.StartAsync(null).GetAwaiter().GetResult();
            requestHeader_ = server_.CreateAndActivateSession("Bench");
        }

        /// <summary>
        /// Tear down Server and the close the session.
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            server_.CloseSession(requestHeader_);
            fixture_.StopAsync().GetAwaiter().GetResult();
            Thread.Sleep(1000);
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Test for expected exceptions.
        /// </summary>
        [Test]
        public void NoInvalidTimestampException()
        {
            // test that the server accepts an invalid timestamp
            requestHeader_.Timestamp = DateTime.UtcNow - TimeSpan.FromDays(30);
            server_.CloseSession(requestHeader_, false);
            sessionClosed_ = true;
        }

        /// <summary>
        /// Get Endpoints.
        /// </summary>
        [Test]
        public void GetEndpoints()
        {
            var endpoints = server_.GetEndpoints();
            Assert.NotNull(endpoints);
        }

        /// <summary>
        /// Get Operation limits.
        /// </summary>
        [Test, Order(100)]
        public void GetOperationLimits()
        {
            var readIdCollection = new ReadValueIdCollection() {
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerRead },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerHistoryReadData },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerHistoryReadEvents },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerWrite },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerHistoryUpdateData },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerHistoryUpdateEvents },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerBrowse },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxMonitoredItemsPerCall },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerNodeManagement },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerRegisterNodes },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerTranslateBrowsePathsToNodeIds },
                new ReadValueId(){ AttributeId = Attributes.Value, NodeId = VariableIds.Server_ServerCapabilities_OperationLimits_MaxNodesPerMethodCall }
            };

            var requestHeader = requestHeader_;
            requestHeader.Timestamp = DateTime.UtcNow;
            var response = server_.Read(requestHeader, MaxAge_, TimestampsToReturn.Neither, readIdCollection, out var results, out var diagnosticInfos);
            ServerFixtureUtils.ValidateResponse(response, results, readIdCollection);
            ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, results, response.StringTable);

            Assert.NotNull(results);
            Assert.AreEqual(readIdCollection.Count, results.Count);

            operationLimits_ = new OperationLimits() {
                MaxNodesPerRead = (uint)results[0].Value,
                MaxNodesPerHistoryReadData = (uint)results[1].Value,
                MaxNodesPerHistoryReadEvents = (uint)results[2].Value,
                MaxNodesPerWrite = (uint)results[3].Value,
                MaxNodesPerHistoryUpdateData = (uint)results[4].Value,
                MaxNodesPerHistoryUpdateEvents = (uint)results[5].Value,
                MaxNodesPerBrowse = (uint)results[6].Value,
                MaxMonitoredItemsPerCall = (uint)results[7].Value,
                MaxNodesPerNodeManagement = (uint)results[8].Value,
                MaxNodesPerRegisterNodes = (uint)results[9].Value,
                MaxNodesPerTranslateBrowsePathsToNodeIds = (uint)results[10].Value,
                MaxNodesPerMethodCall = (uint)results[11].Value
            };
        }

        /// <summary>
        /// Read node.
        /// </summary>
        [Test]
        [Benchmark]
        public void Read()
        {
            // Read
            var requestHeader = requestHeader_;
            requestHeader.Timestamp = DateTime.UtcNow;
            var nodesToRead = new ReadValueIdCollection();
            var nodeId = new NodeId("Scalar_Simulation_Int32", 2);
            foreach (var attributeId in ServerFixtureUtils.AttributesIds.Keys)
            {
                nodesToRead.Add(new ReadValueId() { NodeId = nodeId, AttributeId = attributeId });
            }
            var response = server_.Read(requestHeader, MaxAge_, TimestampsToReturn.Neither, nodesToRead,
                out var dataValues, out var diagnosticInfos);
            ServerFixtureUtils.ValidateResponse(response, dataValues, nodesToRead);
            ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, dataValues, response.StringTable);
        }

        /// <summary>
        /// Read all nodes.
        /// </summary>
        [Test]
        public void ReadAllNodes()
        {
            var serverTestServices = new ServerTestServices(server_);
            if (operationLimits_ == null)
            {
                GetOperationLimits();
            }
            if (referenceDescriptions_ == null)
            {
                referenceDescriptions_ = CommonTestWorkers.BrowseFullAddressSpaceWorker(serverTestServices, requestHeader_, operationLimits_);
            }

            // Read all variables
            var requestHeader = requestHeader_;
            foreach (var reference in referenceDescriptions_)
            {
                requestHeader.Timestamp = DateTime.UtcNow;
                var nodesToRead = new ReadValueIdCollection();
                var nodeId = ExpandedNodeId.ToNodeId(reference.NodeId, server_.CurrentInstance.NamespaceUris);
                foreach (var attributeId in ServerFixtureUtils.AttributesIds.Keys)
                {
                    nodesToRead.Add(new ReadValueId() { NodeId = nodeId, AttributeId = attributeId });
                }
                TestContext.Out.WriteLine("NodeId {0} {1}", reference.NodeId, reference.BrowseName);
                var response = server_.Read(requestHeader, MaxAge_, TimestampsToReturn.Both, nodesToRead,
                    out var dataValues, out var diagnosticInfos);
                ServerFixtureUtils.ValidateResponse(response, dataValues, nodesToRead);
                ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, dataValues, response.StringTable);

                foreach (var dataValue in dataValues)
                {
                    TestContext.Out.WriteLine(" {0}", dataValue.ToString());
                }
            }
        }

        /// <summary>
        /// Write Node.
        /// </summary>
        [Test]
        [Benchmark]
        public void Write()
        {
            // Write
            var requestHeader = requestHeader_;
            requestHeader.Timestamp = DateTime.UtcNow;
            var nodesToWrite = new WriteValueCollection();
            var nodeId = new NodeId("Scalar_Simulation_Int32", 2);
            nodesToWrite.Add(new WriteValue() { NodeId = nodeId, AttributeId = Attributes.Value, Value = new DataValue(1234) });
            var response = server_.Write(requestHeader, nodesToWrite,
                out var dataValues, out var diagnosticInfos);
            ServerFixtureUtils.ValidateResponse(response, dataValues, nodesToWrite);
            ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, dataValues, response.StringTable);
        }

        /// <summary>
        /// Update static Nodes, read modify write.
        /// </summary>
        [Test, Order(350)]
        public void ReadWriteUpdateNodes()
        {
            // Nodes
            var namespaceUris = server_.CurrentInstance.NamespaceUris;
            NodeId[] testSet = CommonTestWorkers.NodeIdTestSetStatic.Select(n => ExpandedNodeId.ToNodeId(n, namespaceUris)).ToArray();

            UpdateValues(testSet);
        }

        /// <summary>
        /// Browse full address space.
        /// </summary>
        [Test, Order(400)]
        [Benchmark]
        public void BrowseFullAddressSpace()
        {
            var serverTestServices = new ServerTestServices(server_);
            if (operationLimits_ == null)
            {
                GetOperationLimits();
            }
            referenceDescriptions_ = CommonTestWorkers.BrowseFullAddressSpaceWorker(serverTestServices, requestHeader_, operationLimits_);
        }

        /// <summary>
        /// Translate references.
        /// </summary>
        [Test, Order(500)]
        [Benchmark]
        public void TranslateBrowsePath()
        {
            var serverTestServices = new ServerTestServices(server_);
            if (operationLimits_ == null)
            {
                GetOperationLimits();
            }
            if (referenceDescriptions_ == null)
            {
                referenceDescriptions_ = CommonTestWorkers.BrowseFullAddressSpaceWorker(serverTestServices, requestHeader_, operationLimits_);
            }
            _ = CommonTestWorkers.TranslateBrowsePathWorker(serverTestServices, referenceDescriptions_, requestHeader_, operationLimits_);
        }

        /// <summary>
        /// Create a subscription with a monitored item.
        /// Read a few notifications with Publish.
        /// Delete the monitored item and subscription.
        /// </summary>
        [Test]
        public void Subscription()
        {
            var serverTestServices = new ServerTestServices(server_);
            CommonTestWorkers.SubscriptionTest(serverTestServices, requestHeader_);
        }

        /// <summary>
        /// Create a secondary Session.
        /// Create a subscription with a monitored item.
        /// Close session, but do not delete subscriptions.
        /// Transfer subscription from closed session to the other.
        /// </summary>
        [Theory]
        public void TransferSubscriptionSessionClosed(bool sendInitialData, bool useSecurity)
        {
            var serverTestServices = new ServerTestServices(server_);
            // save old security context, test fixture can only work with one session
            var securityContext = SecureChannelContext.Current;
            try
            {
                RequestHeader transferRequestHeader = server_.CreateAndActivateSession("ClosedSession", useSecurity);
                var transferSecurityContext = SecureChannelContext.Current;
                var namespaceUris = server_.CurrentInstance.NamespaceUris;
                NodeId[] testSet = CommonTestWorkers.NodeIdTestSetStatic.Select(n => ExpandedNodeId.ToNodeId(n, namespaceUris)).ToArray();
                transferRequestHeader.Timestamp = DateTime.UtcNow;
                var subscriptionIds = CommonTestWorkers.CreateSubscriptionForTransfer(serverTestServices, transferRequestHeader, testSet, QueueSize_, -1);

                transferRequestHeader.Timestamp = DateTime.UtcNow;
                server_.CloseSession(transferRequestHeader, false);

                //restore security context, transfer abandoned subscription
                SecureChannelContext.Current = securityContext;
                CommonTestWorkers.TransferSubscriptionTest(serverTestServices, requestHeader_, subscriptionIds, sendInitialData, !useSecurity);

                if (useSecurity)
                {
                    // subscription was deleted, expect 'BadNoSubscription'
                    var sre = Assert.Throws<ServiceResultException>(() => {
                        requestHeader_.Timestamp = DateTime.UtcNow;
                        CommonTestWorkers.VerifySubscriptionTransferred(serverTestServices, requestHeader_, subscriptionIds, true);
                    });
                    Assert.AreEqual(StatusCodes.BadNoSubscription, sre.StatusCode);
                }
            }
            finally
            {
                //restore security context, that close connection can work
                SecureChannelContext.Current = securityContext;
            }
        }

        /// <summary>
        /// Create a subscription with a monitored item.
        /// Create a secondary Session.
        /// Transfer subscription with a monitored item from one session to the other.
        /// </summary>
        [Theory]
        public void TransferSubscription(bool sendInitialData, bool useSecurity)
        {
            var serverTestServices = new ServerTestServices(server_);
            // save old security context, test fixture can only work with one session
            var securityContext = SecureChannelContext.Current;
            try
            {
                var namespaceUris = server_.CurrentInstance.NamespaceUris;
                NodeId[] testSet = CommonTestWorkers.NodeIdTestSetStatic.Select(n => ExpandedNodeId.ToNodeId(n, namespaceUris)).ToArray();
                var subscriptionIds = CommonTestWorkers.CreateSubscriptionForTransfer(serverTestServices, requestHeader_, testSet, QueueSize_, -1);

                RequestHeader transferRequestHeader = server_.CreateAndActivateSession("TransferSession", useSecurity);
                var transferSecurityContext = SecureChannelContext.Current;
                CommonTestWorkers.TransferSubscriptionTest(serverTestServices, transferRequestHeader, subscriptionIds, sendInitialData, !useSecurity);

                if (useSecurity)
                {
                    //restore security context
                    SecureChannelContext.Current = securityContext;
                    CommonTestWorkers.VerifySubscriptionTransferred(serverTestServices, requestHeader_, subscriptionIds, true);
                }

                transferRequestHeader.Timestamp = DateTime.UtcNow;
                SecureChannelContext.Current = transferSecurityContext;
                server_.CloseSession(transferRequestHeader);
            }
            finally
            {
                //restore security context, that close connection can work
                SecureChannelContext.Current = securityContext;
            }
        }


        /// <summary>
        /// Create a subscription with a monitored item.
        /// Call ResendData.
        /// Ensure only a single value per monitored item is returned after ResendData was called.
        /// </summary>
        [Test]
        [NonParallelizable]
        [TestCase(true, QueueSize_)]
        [TestCase(false, QueueSize_)]
        [TestCase(true, 0U)]
        [TestCase(false, 0U)]
        public void ResendData(bool updateValues, uint queueSize)
        {
            var serverTestServices = new ServerTestServices(server_);
            // save old security context, test fixture can only work with one session
            var securityContext = SecureChannelContext.Current;
            try
            {
                var namespaceUris = server_.CurrentInstance.NamespaceUris;
                NodeIdCollection testSetCollection = CommonTestWorkers.NodeIdTestSetStatic.Select(n => ExpandedNodeId.ToNodeId(n, namespaceUris)).ToArray();
                testSetCollection.AddRange(CommonTestWorkers.NodeIdTestDataSetStatic.Select(n => ExpandedNodeId.ToNodeId(n, namespaceUris)).ToArray());
                NodeId[] testSet = testSetCollection.ToArray();

                //Re-use method CreateSubscriptionForTransfer to create a subscription
                var subscriptionIds = CommonTestWorkers.CreateSubscriptionForTransfer(serverTestServices, requestHeader_, testSet, queueSize, 0);

                RequestHeader resendDataRequestHeader = server_.CreateAndActivateSession("ResendData");
                var resendDataSecurityContext = SecureChannelContext.Current;

                SecureChannelContext.Current = securityContext;
                // After the ResendData call there will be data to publish again
                var nodesToCall = ResendDataCall(StatusCodes.Good, subscriptionIds);

                Thread.Sleep(1000);

                // Make sure publish queue becomes empty by consuming it 
                Assert.AreEqual(1, subscriptionIds.Count);

                // Issue a Publish request
                requestHeader_.Timestamp = DateTime.UtcNow;
                var acknoledgements = new SubscriptionAcknowledgementCollection();
                var response = serverTestServices.Publish(requestHeader_, acknoledgements,
                    out uint publishedId, out UInt32Collection availableSequenceNumbers,
                    out bool moreNotifications, out NotificationMessage notificationMessage,
                    out StatusCodeCollection _, out DiagnosticInfoCollection diagnosticInfos);

                Assert.AreEqual(StatusCodes.Good, response.ServiceResult.Code);
                ServerFixtureUtils.ValidateResponse(response);
                ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, acknoledgements, response.StringTable);
                Assert.AreEqual(subscriptionIds[0], publishedId);
                Assert.AreEqual(1, notificationMessage.NotificationData.Count);

                // Validate nothing to publish a few times
                const int timesToCallPublish = 3;
                for (int i = 0; i < timesToCallPublish; i++)
                {
                    requestHeader_.Timestamp = DateTime.UtcNow;
                    response = serverTestServices.Publish(requestHeader_, acknoledgements,
                        out publishedId, out availableSequenceNumbers,
                        out moreNotifications, out notificationMessage,
                        out StatusCodeCollection _, out diagnosticInfos);

                    Assert.AreEqual(StatusCodes.Good, response.ServiceResult.Code);
                    ServerFixtureUtils.ValidateResponse(response);
                    ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, acknoledgements, response.StringTable);
                    Assert.AreEqual(subscriptionIds[0], publishedId);
                    Assert.AreEqual(0, notificationMessage.NotificationData.Count);
                }

                // Validate ResendData method call returns error from different session contexts

                // call ResendData method from different session context
                SecureChannelContext.Current = resendDataSecurityContext;
                resendDataRequestHeader.Timestamp = DateTime.UtcNow;
                response = server_.Call(resendDataRequestHeader,
                    nodesToCall,
                    out var results,
                    out diagnosticInfos);

                SecureChannelContext.Current = securityContext;

                Assert.AreEqual(StatusCodes.BadUserAccessDenied, results[0].StatusCode.Code);
                ServerFixtureUtils.ValidateResponse(response, results, nodesToCall);
                ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, nodesToCall, response.StringTable);

                // Still nothing to publish since previous ResendData call did not execute
                requestHeader_.Timestamp = DateTime.UtcNow;
                response = serverTestServices.Publish(requestHeader_, acknoledgements,
                    out publishedId, out availableSequenceNumbers,
                    out moreNotifications, out notificationMessage,
                    out StatusCodeCollection _, out diagnosticInfos);

                Assert.AreEqual(StatusCodes.Good, response.ServiceResult.Code);
                ServerFixtureUtils.ValidateResponse(response);
                ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, acknoledgements, response.StringTable);
                Assert.AreEqual(subscriptionIds[0], publishedId);
                Assert.AreEqual(0, notificationMessage.NotificationData.Count);

                if (updateValues)
                {
                    UpdateValues(testSet);

                    // fill queues, but only a single value per resend publish shall be returned
                    for (int i = 1; i < queueSize; i++)
                    {
                        UpdateValues(testSet);
                    }
                }

                // call ResendData method from the same session context
                ResendDataCall(StatusCodes.Good, subscriptionIds);

                // Data should be available for publishing now
                requestHeader_.Timestamp = DateTime.UtcNow;
                response = serverTestServices.Publish(requestHeader_, acknoledgements,
                    out publishedId, out availableSequenceNumbers,
                    out moreNotifications, out notificationMessage,
                    out StatusCodeCollection _, out diagnosticInfos);

                Assert.AreEqual(StatusCodes.Good, response.ServiceResult.Code);
                ServerFixtureUtils.ValidateResponse(response);
                ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, acknoledgements, response.StringTable);
                Assert.AreEqual(subscriptionIds[0], publishedId);
                Assert.AreEqual(1, notificationMessage.NotificationData.Count);
                var items = notificationMessage.NotificationData.FirstOrDefault();
                Assert.IsTrue(items.Body is Opc.Ua.DataChangeNotification);
                var monitoredItemsCollection = ((Opc.Ua.DataChangeNotification)items.Body).MonitoredItems;
                Assert.AreEqual(testSet.Length, monitoredItemsCollection.Count);

                Thread.Sleep(1000);

                if (updateValues && queueSize > 1)
                {
                    // remaining queue Data should be sent in this publish
                    requestHeader_.Timestamp = DateTime.UtcNow;
                    response = serverTestServices.Publish(requestHeader_, acknoledgements,
                        out publishedId, out availableSequenceNumbers,
                        out moreNotifications, out notificationMessage,
                        out StatusCodeCollection _, out diagnosticInfos);

                    Assert.AreEqual(StatusCodes.Good, response.ServiceResult.Code);
                    ServerFixtureUtils.ValidateResponse(response);
                    ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, acknoledgements, response.StringTable);
                    Assert.AreEqual(subscriptionIds[0], publishedId);
                    Assert.AreEqual(1, notificationMessage.NotificationData.Count);
                    items = notificationMessage.NotificationData.FirstOrDefault();
                    Assert.IsTrue(items.Body is Opc.Ua.DataChangeNotification);
                    monitoredItemsCollection = ((Opc.Ua.DataChangeNotification)items.Body).MonitoredItems;
                    Assert.AreEqual(testSet.Length * (queueSize - 1), monitoredItemsCollection.Count, testSet.Length);
                }

                // Call ResendData method with invalid subscription Id
                ResendDataCall(StatusCodes.BadSubscriptionIdInvalid, new UInt32Collection() { subscriptionIds.Last() + 20 });

                // Nothing to publish since previous ResendData call did not execute
                requestHeader_.Timestamp = DateTime.UtcNow;
                response = serverTestServices.Publish(requestHeader_, acknoledgements,
                    out publishedId, out availableSequenceNumbers,
                    out moreNotifications, out notificationMessage,
                    out StatusCodeCollection _, out diagnosticInfos);

                Assert.AreEqual(StatusCodes.Good, response.ServiceResult.Code);
                ServerFixtureUtils.ValidateResponse(response);
                ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, acknoledgements, response.StringTable);
                Assert.AreEqual(subscriptionIds[0], publishedId);
                Assert.AreEqual(0, notificationMessage.NotificationData.Count);

                resendDataRequestHeader.Timestamp = DateTime.UtcNow;
                SecureChannelContext.Current = resendDataSecurityContext;
                server_.CloseSession(resendDataRequestHeader);
            }
            finally
            {
                //restore security context, that close connection can work
                SecureChannelContext.Current = securityContext;
            }
        }
        #endregion

        #region Private Methods
        private CallMethodRequestCollection ResendDataCall(StatusCode expectedStatus, UInt32Collection subscriptionIds)
        {
            // Find the ResendData method
            var nodesToCall = new CallMethodRequestCollection();
            foreach (var subscriptionId in subscriptionIds)
            {
                nodesToCall.Add(new CallMethodRequest() {
                    ObjectId = ObjectIds.Server,
                    MethodId = MethodIds.Server_ResendData,
                    InputArguments = new VariantCollection() { new Variant(subscriptionId) }
                });
            }

            //call ResendData method with subscription ids
            requestHeader_.Timestamp = DateTime.UtcNow;
            var response = server_.Call(requestHeader_,
                nodesToCall,
                out var results,
                out var diagnosticInfos);

            Assert.AreEqual(expectedStatus, results[0].StatusCode.Code);
            ServerFixtureUtils.ValidateResponse(response, results, nodesToCall);
            ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, nodesToCall, response.StringTable);

            return nodesToCall;
        }

        /// <summary>
        /// Read Values of NodeIds, determine types, write back new random values.
        /// </summary>
        /// <param name="testSet">The nodeIds to modify.</param>
        private void UpdateValues(NodeId[] testSet)
        {
            // Read values
            var requestHeader = requestHeader_;
            var nodesToRead = new ReadValueIdCollection();
            foreach (NodeId nodeId in testSet)
            {
                nodesToRead.Add(new ReadValueId() { NodeId = nodeId, AttributeId = Attributes.Value });
            }
            var response = server_.Read(requestHeader, MaxAge_, TimestampsToReturn.Neither, nodesToRead,
                out var readDataValues, out var diagnosticInfos);

            ServerFixtureUtils.ValidateResponse(response, readDataValues, nodesToRead);
            ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, readDataValues, response.StringTable);
            Assert.AreEqual(testSet.Length, readDataValues.Count);

            var modifiedValues = new DataValueCollection();
            foreach (var dataValue in readDataValues)
            {
                var typeInfo = TypeInfo.Construct(dataValue.Value);
                Assert.IsNotNull(typeInfo);
                var value = generator_.GetRandom(typeInfo.BuiltInType);
                modifiedValues.Add(new DataValue() { WrappedValue = new Variant(value) });
            }

            int ii = 0;
            var nodesToWrite = new WriteValueCollection();
            foreach (NodeId nodeId in testSet)
            {
                nodesToWrite.Add(new WriteValue() { NodeId = nodeId, AttributeId = Attributes.Value, Value = modifiedValues[ii] });
                ii++;
            }

            // Write Nodes
            requestHeader.Timestamp = DateTime.UtcNow;
            response = server_.Write(requestHeader, nodesToWrite,
                out var writeDataValues, out diagnosticInfos);
            ServerFixtureUtils.ValidateResponse(response, writeDataValues, nodesToWrite);
            ServerFixtureUtils.ValidateDiagnosticInfos(diagnosticInfos, writeDataValues, response.StringTable);
        }
        #endregion
    }
}
