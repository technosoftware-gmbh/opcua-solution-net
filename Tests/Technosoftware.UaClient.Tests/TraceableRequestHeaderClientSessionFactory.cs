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
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient.Tests
{
    /// <summary>
    /// Object that creates an instance of a Session object.
    /// It can be used to create instances of enhanced Session
    /// classes with added functionality or overridden methods.
    /// </summary>
    public class TraceableRequestHeaderClientSessionFactory : TraceableSessionFactory
    {
        #region ISessionInstantiator Members
        /// <summary>
        /// Object that creates instances of an Opc.Ua.Client.Session object with Activity Source.
        /// </summary>
        public new static readonly TraceableRequestHeaderClientSessionFactory Instance = new TraceableRequestHeaderClientSessionFactory();

        /// <inheritdoc/>
        public override Session Create(
            ISessionChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint)
        {
            return new TraceableRequestHeaderClientSession(channel, configuration, endpoint);
        }

        /// <inheritdoc/>
        public override Session Create(
            ITransportChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate,
            EndpointDescriptionCollection availableEndpoints = null,
            StringCollection discoveryProfileUris = null)
        {
            return new TraceableRequestHeaderClientSession(channel, configuration, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris);
        }
        #endregion
    }
}
