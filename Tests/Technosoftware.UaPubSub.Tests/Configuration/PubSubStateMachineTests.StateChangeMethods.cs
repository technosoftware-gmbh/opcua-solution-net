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

using NUnit.Framework;

using Opc.Ua;

using Technosoftware.UaPubSub.Configuration;
#endregion

namespace Technosoftware.UaPubSub.Tests.Configuration
{
    partial class PubSubStateMachineTests
    {
        [Test(Description = "Validate Call Enable on Disabled object")]
        public void ValidateEnableOnDisabled()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            configurator.Disable(pubSub);
            Assert.AreEqual((StatusCode)StatusCodes.Good, configurator.Enable(pubSub));
        }

        [Test(Description = "Validate Call Enable on Enabled object")]
        public void ValidateEnableOnOperational()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            configurator.Enable(pubSub);
            Assert.AreEqual((StatusCode)StatusCodes.BadInvalidState, configurator.Enable(pubSub));
        }

        [Test(Description = "Validate Call Disable on Enabled object")]
        public void ValidateDisableOnEnabled()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            configurator.Enable(pubSub);
            Assert.AreEqual((StatusCode)StatusCodes.Good, configurator.Disable(pubSub));
        }

        [Test(Description = "Validate Call Disable on Disabled object")]
        public void ValidateDisableOnDisabled()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            PubSubConfigurationDataType pubSub = uaPubSubApplication.UaPubSubConfigurator.PubSubConfiguration;
            configurator.Disable(pubSub);
            Assert.AreEqual((StatusCode)StatusCodes.BadInvalidState, configurator.Disable(pubSub));
        }

        [Test(Description = "Validate Call Enable on null object")]
        public void ValidateEnableOnNUll()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            Assert.Throws<ArgumentException>(() => configurator.Enable(null), "The Enable method does not throw exception when called with null parameter.");
        }

        [Test(Description = "Validate Call Disable on null object")]
        public void ValidateDisableOnNUll()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            Assert.Throws<ArgumentException>(() => configurator.Disable(null), "The Disable method does not throw exception when called with null parameter.");
        }

        [Test(Description = "Validate Call Enable on non existing object")]
        public void ValidateEnableOnNonExisting()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            PubSubConfigurationDataType nonExisting = new PubSubConfigurationDataType();
            Assert.Throws<ArgumentException>(() => configurator.Enable(nonExisting), "The Enable method does not throw exception when called with non existing parameter.");
        }

        [Test(Description = "Validate Call Disable on non existing object")]
        public void ValidateDisableOnNonExisting()
        {
            UaPubSubApplication uaPubSubApplication = UaPubSubApplication.Create(publisherConfigurationFile);
            UaPubSubConfigurator configurator = uaPubSubApplication.UaPubSubConfigurator;
            PubSubConfigurationDataType nonExisting = new PubSubConfigurationDataType();
            Assert.Throws<ArgumentException>(() => configurator.Disable(nonExisting), "The Disable method does not throw exception when called with non existing parameter.");
        }

    }
}
