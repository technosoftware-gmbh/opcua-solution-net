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
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;

using Technosoftware.UaStandardServer.Tests;
#endregion

namespace Technosoftware.UaClient.Tests
{
    /// <summary>
    /// Object that creates an instance of a Session object.
    /// It can be used to create instances of enhanced Session
    /// classes with added functionality or overridden methods.
    /// </summary>
    public class TestableSessionInstantiator : IUaSessionInstantiator
    {
        #region Constructors
        /// <inheritdoc/>
        public Session Create(
            ISessionChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint)
        {
            return new TestableSession(channel, configuration, endpoint);
        }

        /// <inheritdoc/>
        public Session Create(
            ITransportChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate,
            EndpointDescriptionCollection availableEndpoints = null,
            StringCollection discoveryProfileUris = null)
        {
            return new TestableSession(channel, configuration, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris);
        }
        #endregion
    }
}
