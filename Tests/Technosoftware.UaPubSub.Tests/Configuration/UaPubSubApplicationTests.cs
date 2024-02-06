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
using System.IO;
using System.Linq;

using NUnit.Framework;

using Opc.Ua;

using Technosoftware.UaPubSub.Configuration;
#endregion

namespace Technosoftware.UaPubSub.Tests.Configuration
{
    [TestFixture(Description = "Tests for UaPubSubApplication class")]
    public class UaPubSubApplicationTests
    {
        private string ConfigurationFileName = Path.Combine("Configuration", "PublisherConfiguration.xml");

        private PubSubConfigurationDataType m_pubSubConfiguration;

        [OneTimeSetUp()]
        public void MyTestInitialize()
        {
            string configurationFile = Utils.GetAbsoluteFilePath(ConfigurationFileName, true, true, false);
            m_pubSubConfiguration = UaPubSubConfigurationHelper.LoadConfiguration(configurationFile);
        }

        [Test(Description = "Validate Create call with null path")]
        public void ValidateUaPubSubApplicationCreateNullFilePath()
        {
            Assert.Throws<ArgumentException>(() => UaPubSubApplication.Create((string)null), "Calling Create with null parameter shall throw error");
        }

        [Test(Description = "Validate Create call with null PubSubConfigurationDataType")]
        public void ValidateUaPubSubApplicationCreateNullPubSubConfigurationDataType()
        {
            Assert.DoesNotThrow(() => UaPubSubApplication.Create((PubSubConfigurationDataType)null), "Calling Create with null parameter shall not throw error");
        }

        [Test(Description = "Validate Create call")]
        public void ValidateUaPubSubApplicationCreate()
        {
            // Arrange
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(m_pubSubConfiguration);

            // Assert
            Assert.IsTrue(uaPubSubApplication.PubSubConnections != null, "uaPubSubApplication.PubSubConnections collection is null");
            Assert.AreEqual(3, uaPubSubApplication.PubSubConnections.Count, "uaPubSubApplication.PubSubConnections count");
            UaPubSubConnection connection = uaPubSubApplication.PubSubConnections[0] as UaPubSubConnection;
            Assert.IsTrue(connection.Publishers != null, "connection.Publishers is null");
            Assert.IsTrue(connection.Publishers.Count == 1, "connection.Publishers count is not 2");
            int index = 0;
            foreach (IUaPublisher publisher in connection.Publishers)
            {
                Assert.IsTrue(publisher != null, "connection.Publishers[{0}] is null", index);
                Assert.IsTrue(publisher.PubSubConnection == connection, "connection.Publishers[{0}].PubSubConnection is not set correctly", index);
                Assert.IsTrue(publisher.WriterGroupConfiguration.WriterGroupId == m_pubSubConfiguration.Connections.First().WriterGroups[index].WriterGroupId, "connection.Publishers[{0}].WriterGroupConfiguration is not set correctly", index);
                index++;
            }
        }

    }
}
