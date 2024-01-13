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
using System.Linq;
using Technosoftware.UaPubSub.Configuration;
using System.IO;

using NUnit.Framework;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Tests.Configuration
{
    partial class PubSubStateMachineTests
    {
        private string PublisherConfigurationFileName = Path.Combine("Configuration", "PublisherConfiguration.xml");
        private string SubscriberConfigurationFileName = Path.Combine("Configuration", "SubscriberConfiguration.xml");

        private string publisherConfigurationFile = null;
        private string subscriberConfigurationFile = null;

        [OneTimeSetUp()]
        public void MyTestInitialize()
        {
            publisherConfigurationFile = Utils.GetAbsoluteFilePath(PublisherConfigurationFileName, true, true, false);
            subscriberConfigurationFile = Utils.GetAbsoluteFilePath(SubscriberConfigurationFileName, true, true, false);
        }

        [Test(Description = "Validate transition of state Disabled_0 to Paused_1 on Publisher")]
        public void ValidateDisabled_0ToPause_1_Publisher()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Disabled, Disabled, Disabled]

            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType publisherConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            WriterGroupDataType writerGroup = publisherConnection.WriterGroups[0];
            DataSetWriterDataType datasetWriter = writerGroup.DataSetWriters[0];

            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            PubSubState wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            PubSubState dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState  == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring connection to Enabled
            configurator.Enable(publisherConnection);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring writerGroup to Enabled
            configurator.Enable(writerGroup);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(wgState == PubSubState.Paused, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring datasetWriter to Enabled
            configurator.Enable(datasetWriter);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(wgState == PubSubState.Paused, Is.True);
            Assert.That(dswState == PubSubState.Paused, Is.True);
        }

        [Test(Description = "Validate transition of state Disabled_0 to Operational_2 on Publisher")]
        public void ValidateDisabled_0ToOperational_2_Publisher()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Disabled, Disabled, Disabled]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType publisherConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections.First();
            WriterGroupDataType writerGroup = publisherConnection.WriterGroups.First();
            DataSetWriterDataType datasetWriter = writerGroup.DataSetWriters.First();

            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            PubSubState wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            PubSubState dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring PubSub to Enabled
            configurator.Enable(pubSub);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring publisherConnection to Enabled
            configurator.Enable(publisherConnection);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring writerGroup to Enabled
            configurator.Enable(writerGroup);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(wgState == PubSubState.Operational, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // Bring datasetWriter to Enabled
            configurator.Enable(datasetWriter);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(wgState == PubSubState.Operational, Is.True);
            Assert.That(dswState == PubSubState.Operational, Is.True);
        }

        [Test(Description = "Validate transition of state Paused_1 to Disabled_0 on Publisher")]
        public void ValidatePaused_1ToDisabled_0_Publisher()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Paused, Paused, Paused]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType publisherConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections.First();
            WriterGroupDataType writerGroup = publisherConnection.WriterGroups[0];
            DataSetWriterDataType datasetWriter = writerGroup.DataSetWriters[0];

            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            configurator.Enable(publisherConnection);
            configurator.Enable(writerGroup);
            configurator.Enable(datasetWriter);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            PubSubState wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            PubSubState dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(wgState == PubSubState.Paused, Is.True);
            Assert.That(dswState == PubSubState.Paused, Is.True);

            // Bring Connection to Disabled
            configurator.Disable(publisherConnection);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Paused, Is.True);
            Assert.That(dswState == PubSubState.Paused, Is.True);

            // Bring writerGroup to Disabled
            configurator.Disable(writerGroup);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Paused, Is.True);

            // Bring datasetWriter to Disabled
            configurator.Disable(datasetWriter);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);
        }

        [Test(Description = "Validate transition of state Paused_1 to Operational_2 on Publisher")]
        public void ValidatePaused_1ToOperational_2_Publisher()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Paused, Paused, Paused]

            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType publisherConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections.First();
            WriterGroupDataType writerGroup = publisherConnection.WriterGroups[0];
            DataSetWriterDataType datasetWriter = writerGroup.DataSetWriters[0];

            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            configurator.Enable(publisherConnection);
            configurator.Enable(writerGroup);
            configurator.Enable(datasetWriter);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            PubSubState wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            PubSubState dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(wgState == PubSubState.Paused, Is.True);
            Assert.That(dswState == PubSubState.Paused, Is.True);

            // Bring pubSub to Enabled
            configurator.Enable(pubSub);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(wgState == PubSubState.Operational, Is.True);
            Assert.That(dswState == PubSubState.Operational, Is.True);

        }

        [Test(Description = "Validate transition of state Operational_2 to Disabled_0 on Publisher")]
        public void ValidateOperational_2ToDisabled_0_Publisher()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Disabled, Disabled, Disabled]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType publisherConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections.First();
            WriterGroupDataType writerGroup = publisherConnection.WriterGroups[0];
            DataSetWriterDataType datasetWriter = writerGroup.DataSetWriters[0];

            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            PubSubState wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            PubSubState dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Operational, Operational, Operational, Operational]
            configurator.Enable(pubSub);
            configurator.Enable(publisherConnection);
            configurator.Enable(writerGroup);
            configurator.Enable(datasetWriter);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(wgState == PubSubState.Operational, Is.True);
            Assert.That(dswState == PubSubState.Operational, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Disabled, Disabled, Disabled]
            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);
        }

        [Test(Description = "Validate transition of state Operational_2 to Paused_1 on Publisher")]
        public void ValidateOperational_2ToPaused_1_Publisher()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Disabled, Disabled, Disabled]

            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType publisherConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections.First();
            WriterGroupDataType writerGroup = publisherConnection.WriterGroups[0];
            DataSetWriterDataType datasetWriter = writerGroup.DataSetWriters[0];

            configurator.Disable(pubSub);
            configurator.Disable(publisherConnection);
            configurator.Disable(writerGroup);
            configurator.Disable(datasetWriter);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            PubSubState wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            PubSubState dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(wgState == PubSubState.Disabled, Is.True);
            Assert.That(dswState == PubSubState.Disabled, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Operational, Operational, Operational, Operational]
            configurator.Enable(pubSub);
            configurator.Enable(publisherConnection);
            configurator.Enable(writerGroup);
            configurator.Enable(datasetWriter);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(wgState == PubSubState.Operational, Is.True);
            Assert.That(dswState == PubSubState.Operational, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubWriterGroup -> DataSetWriter brought to [Disabled, Pause, Pause, Pause]
            configurator.Disable(pubSub);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(publisherConnection);
            wgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(writerGroup);
            dswState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetWriter);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(wgState == PubSubState.Paused, Is.True);
            Assert.That(dswState == PubSubState.Paused, Is.True);
        }
    }
}
