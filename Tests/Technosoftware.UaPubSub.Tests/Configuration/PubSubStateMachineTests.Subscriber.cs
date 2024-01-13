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
using NUnit.Framework;

using Opc.Ua;

using Technosoftware.UaPubSub.Configuration;
#endregion

namespace Technosoftware.UaPubSub.Tests.Configuration
{
    partial class PubSubStateMachineTests
    {
        [Test(Description = "Validate transition of state Disabled_0 to Paused_1 on Reader")]
        public void ValidateDisabled_0ToPause_1_Reader()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(subscriberConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Disabled, Disabled, Disabled]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType subscriberConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            ReaderGroupDataType readerGroup = subscriberConnection.ReaderGroups[0];
            DataSetReaderDataType datasetReader = readerGroup.DataSetReaders[0];

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            PubSubState rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            PubSubState dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring Connection to Enabled
            configurator.Enable(subscriberConnection);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring readerGroup to Enabled
            configurator.Enable(readerGroup);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(rgState == PubSubState.Paused, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring datasetReader to Enabled
            configurator.Enable(datasetReader);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(rgState == PubSubState.Paused, Is.True);
            Assert.That(dsrState == PubSubState.Paused, Is.True);
        }

        [Test(Description = "Validate transition of state Disabled_0 to Operational_2 on Reader")]
        public void ValidateDisabled_0ToOperational_2_Reader()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(subscriberConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Disabled, Disabled, Disabled]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType subscriberConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            ReaderGroupDataType readerGroup = subscriberConnection.ReaderGroups[0];
            DataSetReaderDataType datasetReader = readerGroup.DataSetReaders[0];

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            PubSubState rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            PubSubState dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring PubSub to Enabled
            configurator.Enable(pubSub);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring subscriberConnection to Enabled
            configurator.Enable(subscriberConnection);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring readerGroup to Enabled
            configurator.Enable(readerGroup);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(rgState == PubSubState.Operational, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // Bring datasetReader to Enabled
            configurator.Enable(datasetReader);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(rgState == PubSubState.Operational, Is.True);
            Assert.That(dsrState == PubSubState.Operational, Is.True);
        }

        [Test(Description = "Validate transition of state Paused_1 to Disabled_0 on Reader")]
        public void ValidatePaused_1ToDisabled_0_Reader()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(subscriberConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Paused, Paused, Paused]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType subscriberConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            ReaderGroupDataType readerGroup = subscriberConnection.ReaderGroups[0];
            DataSetReaderDataType datasetReader = readerGroup.DataSetReaders[0];

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            configurator.Enable(subscriberConnection);
            configurator.Enable(readerGroup);
            configurator.Enable(datasetReader);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            PubSubState rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            PubSubState dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(rgState == PubSubState.Paused, Is.True);
            Assert.That(dsrState == PubSubState.Paused, Is.True);

            // Bring Connection to Disabled
            configurator.Disable(subscriberConnection);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Paused, Is.True);
            Assert.That(dsrState == PubSubState.Paused, Is.True);

            // Bring readerGroup to Disabled
            configurator.Disable(readerGroup);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Paused, Is.True);

            // Bring datasetReader to Disabled
            configurator.Disable(datasetReader);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);
        }

        [Test(Description = "Validate transition of state Paused_1 to Operational_2 on Reader")]
        public void ValidatePaused_1ToOperational_2_Reader()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(subscriberConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Paused, Paused, Paused]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType subscriberConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            ReaderGroupDataType readerGroup = subscriberConnection.ReaderGroups[0];
            DataSetReaderDataType datasetReader = readerGroup.DataSetReaders[0];

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            configurator.Enable(subscriberConnection);
            configurator.Enable(readerGroup);
            configurator.Enable(datasetReader);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            PubSubState rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            PubSubState dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(rgState == PubSubState.Paused, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(dsrState == PubSubState.Paused, Is.True);

            // Bring pubSub to Enabled
            configurator.Enable(pubSub);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(rgState == PubSubState.Operational, Is.True);
            Assert.That(dsrState == PubSubState.Operational, Is.True);
        }

        [Test(Description = "Validate transition of state Operational_2 to Disabled_0 on Reader")]
        public void ValidateOperational_2ToDisabled_0_Reader()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(subscriberConfigurationFile);

            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Disabled, Disabled, Disabled]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType subscriberConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            ReaderGroupDataType readerGroup = subscriberConnection.ReaderGroups[0];
            DataSetReaderDataType datasetReader = readerGroup.DataSetReaders[0];

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            PubSubState rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            PubSubState dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Operational, Operational, Operational, Operational]
            configurator.Enable(pubSub);
            configurator.Enable(subscriberConnection);
            configurator.Enable(readerGroup);
            configurator.Enable(datasetReader);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(rgState == PubSubState.Operational, Is.True);
            Assert.That(dsrState == PubSubState.Operational, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Disabled, Disabled, Disabled]

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);
        }

        [Test(Description = "Validate transition of state Operational_2 to Paused_1 on Reader")]
        public void ValidateOperational_2ToPaused_1_Reader()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(subscriberConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Disabled, Disabled, Disabled]
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            PubSubConnectionDataType subscriberConnection = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration.Connections[0];
            ReaderGroupDataType readerGroup = subscriberConnection.ReaderGroups[0];
            DataSetReaderDataType datasetReader = readerGroup.DataSetReaders[0];

            configurator.Disable(pubSub);
            configurator.Disable(subscriberConnection);
            configurator.Disable(readerGroup);
            configurator.Disable(datasetReader);

            PubSubState psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            PubSubState conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            PubSubState rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            PubSubState dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Disabled, Is.True);
            Assert.That(rgState == PubSubState.Disabled, Is.True);
            Assert.That(dsrState == PubSubState.Disabled, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Operational, Operational, Operational, Operational]

            configurator.Enable(pubSub);
            configurator.Enable(subscriberConnection);
            configurator.Enable(readerGroup);
            configurator.Enable(datasetReader);

            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Operational, Is.True);
            Assert.That(conState == PubSubState.Operational, Is.True);
            Assert.That(rgState == PubSubState.Operational, Is.True);
            Assert.That(dsrState == PubSubState.Operational, Is.True);

            // The hierarchy PubSub -> PubSubConnection -> PubSubReaderGroup -> DataSetReader brought to [Disabled, Pause, Pause, Pause]
            configurator.Disable(pubSub);
            psState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(pubSub);
            conState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(subscriberConnection);
            rgState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(readerGroup);
            dsrState = uaPubSubApplication.UaPubSubConfigurator.FindStateForObject(datasetReader);
            Assert.That(psState == PubSubState.Disabled, Is.True);
            Assert.That(conState == PubSubState.Paused, Is.True);
            Assert.That(rgState == PubSubState.Paused, Is.True);
            Assert.That(dsrState == PubSubState.Paused, Is.True);
        }

    }
}
