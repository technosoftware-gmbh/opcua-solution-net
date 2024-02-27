#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Object that creates instances of an Opc.Ua.Client.Session object.
    /// </summary>
    public class DefaultSessionFactory : IUaSessionFactory, IUaSessionInstantiator
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// The default instance of the factory.
        /// </summary>
        public static readonly DefaultSessionFactory Instance = new DefaultSessionFactory();

        /// <summary>
        /// Force use of the default instance.
        /// </summary>
        protected DefaultSessionFactory()
        {
        }
        #endregion

        #region IUaSessionFactory Methods
        /// <inheritdoc/>
        public virtual Task<IUaSession> CreateAsync(
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            return CreateAsync(configuration, endpoint, updateBeforeConnect, false, sessionName, sessionTimeout, identity, preferredLocales, ct);
        }

        /// <inheritdoc/>
        public async virtual Task<IUaSession> CreateAsync(
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            return await Session.CreateAsync(this, configuration, (ITransportWaitingConnection)null, endpoint,
                updateBeforeConnect, checkDomain, sessionName, sessionTimeout,
                identity, preferredLocales, ct).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async virtual Task<IUaSession> CreateAsync(
            ApplicationConfiguration configuration,
            ITransportWaitingConnection connection,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            return await Session.CreateAsync(this, configuration, connection, endpoint,
                updateBeforeConnect, checkDomain, sessionName, sessionTimeout,
                identity, preferredLocales, ct
                ).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async virtual Task<IUaSession> CreateAsync(
            ApplicationConfiguration configuration,
            ReverseConnectManager reverseConnectManager,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity userIdentity,
            IList<string> preferredLocales,
            CancellationToken ct = default
            )
        {
            if (reverseConnectManager == null)
            {
                return await CreateAsync(configuration, endpoint, updateBeforeConnect,
                    checkDomain, sessionName, sessionTimeout, userIdentity, preferredLocales, ct).ConfigureAwait(false);
            }

            ITransportWaitingConnection connection;
            do
            {
                connection = await reverseConnectManager.WaitForConnectionAsync(
                    endpoint.EndpointUrl,
                    endpoint.ReverseConnect?.ServerUri,
                    ct).ConfigureAwait(false);

                if (updateBeforeConnect)
                {
                    await endpoint.UpdateFromServerAsync(
                        endpoint.EndpointUrl, connection,
                        endpoint.Description.SecurityMode,
                        endpoint.Description.SecurityPolicyUri,
                        ct).ConfigureAwait(false);
                    updateBeforeConnect = false;
                    connection = null;
                }
            } while (connection == null);

            return await CreateAsync(
                configuration,
                connection,
                endpoint,
                false,
                checkDomain,
                sessionName,
                sessionTimeout,
                userIdentity,
                preferredLocales,
                ct).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual IUaSession Create(
           ApplicationConfiguration configuration,
           ITransportChannel channel,
           ConfiguredEndpoint endpoint,
           X509Certificate2 clientCertificate,
           EndpointDescriptionCollection availableEndpoints = null,
           StringCollection discoveryProfileUris = null)
        {
            return Session.Create(this, configuration, channel, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris);
        }

        /// <inheritdoc/>
        public virtual Task<ITransportChannel> CreateChannelAsync(
            ApplicationConfiguration configuration,
            ITransportWaitingConnection connection,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            CancellationToken ct = default)
        {
            return Session.CreateChannelAsync(configuration, connection, endpoint, updateBeforeConnect, checkDomain, ct);
        }

        /// <inheritdoc/>
        public virtual async Task<IUaSession> RecreateAsync(IUaSession sessionTemplate, CancellationToken ct = default)
        {
            return !(sessionTemplate is Session template)
                ? throw new ArgumentOutOfRangeException(nameof(sessionTemplate), "The IUaSession provided is not of a supported type.")
                : (IUaSession)await Session.RecreateAsync(template, ct).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IUaSession> RecreateAsync(IUaSession sessionTemplate, ITransportWaitingConnection connection, CancellationToken ct = default)
        {
            return !(sessionTemplate is Session template)
                ? throw new ArgumentOutOfRangeException(nameof(sessionTemplate), "The IUaSession provided is not of a supported type")
                : (IUaSession)await Session.RecreateAsync(template, connection, ct).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<IUaSession> RecreateAsync(IUaSession sessionTemplate, ITransportChannel transportChannel, CancellationToken ct = default)
        {
            return !(sessionTemplate is Session template)
                ? throw new ArgumentOutOfRangeException(nameof(sessionTemplate), "The IUaSession provided is not of a supported type")
                : (IUaSession)await Session.RecreateAsync(template, transportChannel, ct).ConfigureAwait(false);
        }
        #endregion

        #region IUaSessionInstantiator Members
        /// <inheritdoc/>
        public virtual Session Create(
            ISessionChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint)
        {
            return new Session(channel, configuration, endpoint);
        }

        /// <inheritdoc/>
        public virtual Session Create(
            ITransportChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate,
            EndpointDescriptionCollection availableEndpoints = null,
            StringCollection discoveryProfileUris = null)
        {
            return new Session(channel, configuration, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris);
        }
        #endregion
    }
}
