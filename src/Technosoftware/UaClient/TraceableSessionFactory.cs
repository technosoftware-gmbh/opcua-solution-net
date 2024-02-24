#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Object that creates instances of an Opc.Ua.Client.Session object with Activity Source.
    /// </summary>
    public class TraceableSessionFactory : DefaultSessionFactory
    {
        /// <summary>
        /// The default instance of the factory.
        /// </summary>
        public new static readonly TraceableSessionFactory Instance = new TraceableSessionFactory();

        /// <summary>
        /// Force use of the default instance.
        /// </summary>
        protected TraceableSessionFactory()
        {
            // Set the default Id format to W3C (older .Net versions use ActivityIfFormat.HierarchicalId)
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;
        }

        #region IUaSessionFactory Members
        /// <inheritdoc/>
        public override async Task<IUaSession> CreateAsync(
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            string sessionName,
            uint sessionTimeout,
            IUserIdentity identity,
            IList<string> preferredLocales,
            CancellationToken ct = default)
        {
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(CreateAsync)))
            {
                IUaSession session = await base.CreateAsync(configuration, endpoint, updateBeforeConnect, false,
                    sessionName, sessionTimeout, identity, preferredLocales, ct).ConfigureAwait(false);
                return new TraceableSession(session);
            }
        }

        /// <inheritdoc/>
        public override async Task<IUaSession> CreateAsync(
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
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(CreateAsync)))
            {
                IUaSession session = await Session.CreateAsync(this, configuration, (ITransportWaitingConnection)null, endpoint,
                    updateBeforeConnect, checkDomain, sessionName, sessionTimeout,
                    identity, preferredLocales, ct).ConfigureAwait(false);

                return new TraceableSession(session);
            }
        }

        /// <inheritdoc/>
        public override async Task<IUaSession> CreateAsync(
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
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(CreateAsync)))
            {
                IUaSession session = await Session.CreateAsync(this, configuration, connection, endpoint,
                    updateBeforeConnect, checkDomain, sessionName, sessionTimeout,
                    identity, preferredLocales, ct
                    ).ConfigureAwait(false);

                return new TraceableSession(session);
            }
        }

        /// <inheritdoc/>
        public override IUaSession Create(
           ApplicationConfiguration configuration,
           ITransportChannel channel,
           ConfiguredEndpoint endpoint,
           X509Certificate2 clientCertificate,
           EndpointDescriptionCollection availableEndpoints = null,
           StringCollection discoveryProfileUris = null)
        {
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(CreateAsync)))
            {
                return new TraceableSession(base.Create(configuration, channel, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris));
            }
        }

        /// <inheritdoc/>
        public override Task<ITransportChannel> CreateChannelAsync(
            ApplicationConfiguration configuration,
            ITransportWaitingConnection connection,
            ConfiguredEndpoint endpoint,
            bool updateBeforeConnect,
            bool checkDomain,
            CancellationToken ct = default)
        {
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(CreateAsync)))
            {
                return base.CreateChannelAsync(configuration, connection, endpoint, updateBeforeConnect, checkDomain, ct);
            }
        }

        /// <inheritdoc/>
        public override async Task<IUaSession> CreateAsync(
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
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(CreateAsync)))
            {
                IUaSession session = await base.CreateAsync(configuration,
                    reverseConnectManager, endpoint,
                    updateBeforeConnect,
                    checkDomain, sessionName,
                    sessionTimeout, userIdentity,
                    preferredLocales, ct).ConfigureAwait(false);

                return new TraceableSession(session);
            }
        }

        /// <inheritdoc/>
        public override Task<IUaSession> RecreateAsync(IUaSession sessionTemplate, CancellationToken ct = default)
        {
            Session session = ValidateIUaSession(sessionTemplate);
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(RecreateAsync)))
            {
                return Task.FromResult((IUaSession)new TraceableSession(Session.Recreate(session)));
            }
        }

        /// <inheritdoc/>
        public override Task<IUaSession> RecreateAsync(IUaSession sessionTemplate, ITransportWaitingConnection connection, CancellationToken ct = default)
        {
            Session session = ValidateIUaSession(sessionTemplate);
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(RecreateAsync)))
            {
                return Task.FromResult((IUaSession)new TraceableSession(Session.Recreate(session, connection)));
            }
        }

        /// <inheritdoc/>
        public override Task<IUaSession> RecreateAsync(IUaSession sessionTemplate, ITransportChannel channel, CancellationToken ct = default)
        {
            Session session = ValidateIUaSession(sessionTemplate);
            using (Activity activity = TraceableSession.ActivitySource.StartActivity(nameof(RecreateAsync)))
            {
                return Task.FromResult((IUaSession)new TraceableSession(Session.Recreate(session, channel)));
            }
        }
        #endregion

        #region Private Methods
        private Session ValidateIUaSession(IUaSession sessionTemplate)
        {
            if (!(sessionTemplate is Session session))
            {
                session = sessionTemplate is TraceableSession template
                    ? (Session)template.Session
                    : throw new ArgumentOutOfRangeException(nameof(sessionTemplate), "The IUaSession provided is not of a supported type.");
            }
            return session;
        }
        #endregion
    }
}
