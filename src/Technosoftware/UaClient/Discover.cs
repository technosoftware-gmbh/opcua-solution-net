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
using System.ComponentModel;
using System.Linq;
using System.Net;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Provides methods for discover (search) of OPC Servers.
    /// </summary>
    public static class Discover
    {
        /// <summary>
        /// A discovery suffix that may be appended to the discovery url of https endpoints.
        /// </summary>
        public static readonly string DiscoverySuffix = "/discovery";

        #region Public Properties
        /// <summary>
        /// The default timeout in milliseconds to use when discovering servers.
        /// </summary>
        public static readonly int DefaultDiscoverTimeout = 15000;
        #endregion

        #region Public Methods (Returns a list of Servers)
        /// <summary>
        /// Discovers the servers on the local machine.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A list of server urls.</returns>
        public static IList<string> DiscoverServers(ApplicationConfiguration configuration)
        {
            return DiscoverServers(configuration, DefaultDiscoverTimeout);
        }

        /// <summary>
        /// Discovers the servers on the local machine.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="discoverTimeout">Operation timeout in milliseconds.</param>
        /// <returns>A list of server urls.</returns>
        public static IList<string> DiscoverServers(
            ApplicationConfiguration configuration,
            int discoverTimeout
            )
        {
            var serverUrls = new List<string>();

            // set a short timeout because this is happening in the drop down event.
            var endpointConfiguration = EndpointConfiguration.Create(configuration);
            endpointConfiguration.OperationTimeout = discoverTimeout;

            // Connect to the local discovery server and find the available servers.
            using (var client = DiscoveryClient.Create(new Uri(Utils.Format(Utils.DiscoveryUrls[0], "localhost")), endpointConfiguration))
            {
                var servers = client.FindServers(null);

                // populate the drop down list with the discovery URLs for the available servers.
                foreach (var applicationDescription in servers)
                {
                    if (applicationDescription.ApplicationType == ApplicationType.DiscoveryServer)
                    {
                        continue;
                    }

                    foreach (var url in applicationDescription.DiscoveryUrls)
                    {
                        var discoveryUrl = url;

                        // Many servers will use the '/discovery' suffix for the discovery endpoint.
                        // The URL without this prefix should be the base URL for the server. 
                        if (discoveryUrl.EndsWith(DiscoverySuffix, StringComparison.OrdinalIgnoreCase))
                        {
                            discoveryUrl = discoveryUrl.Substring(0, discoveryUrl.Length - DiscoverySuffix.Length);
                        }

                        // ensure duplicates do not get added.
                        if (!serverUrls.Exists(serverUrl => serverUrl.Equals(discoveryUrl, StringComparison.OrdinalIgnoreCase)))
                        {
                            serverUrls.Add(discoveryUrl);
                        }
                    }
                }
            }

            return serverUrls;
        }

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <returns>The best available endpoint.</returns>
        public static EndpointDescription SelectEndpoint(string discoveryUrl, bool useSecurity)
        {
            return SelectEndpoint(discoveryUrl, useSecurity, DefaultDiscoverTimeout);
        }

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <param name="discoverTimeout">Operation timeout in milliseconds.</param>
        /// <returns>The best available endpoint.</returns>
        public static EndpointDescription SelectEndpoint(
            string discoveryUrl,
            bool useSecurity,
            int discoverTimeout
            )
        {
            var url = GetDiscoveryUrl(discoveryUrl);
            var endpointConfiguration = EndpointConfiguration.Create();
            endpointConfiguration.OperationTimeout = discoverTimeout;

            // Connect to the server's discovery endpoint and find the available configuration.
            using (var client = DiscoveryClient.Create(url, endpointConfiguration))
            {
                var endpoints = client.GetEndpoints(null);
                return SelectEndpoint(url, endpoints, useSecurity);
            }
        }

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        public static EndpointDescription SelectEndpoint(
            ApplicationConfiguration application,
            ITransportWaitingConnection connection,
            bool useSecurity
            )
        {
            return SelectEndpoint(application, connection, useSecurity, DefaultDiscoverTimeout);
        }

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        public static EndpointDescription SelectEndpoint(
            ApplicationConfiguration application,
            ITransportWaitingConnection connection,
            bool useSecurity,
            int discoverTimeout
            )
        {
            var endpointConfiguration = EndpointConfiguration.Create();
            endpointConfiguration.OperationTimeout = discoverTimeout > 0 ? discoverTimeout : DefaultDiscoverTimeout;

            using (var client = DiscoveryClient.Create(application, connection, endpointConfiguration))
            {
                var url = new Uri(client.Endpoint.EndpointUrl);
                var endpoints = client.GetEndpoints(null);
                return SelectEndpoint(url, endpoints, useSecurity);
            }
        }

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="application">The application configuration.</param>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <returns>The best available endpoint.</returns>
        public static EndpointDescription SelectEndpoint(
            ApplicationConfiguration application,
            string discoveryUrl,
            bool useSecurity)
        {
            return SelectEndpoint(application, discoveryUrl, useSecurity, DefaultDiscoverTimeout);
        }

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="application">The application configuration.</param>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <param name="discoverTimeout">The timeout for the discover operation.</param>
        /// <returns>The best available endpoint.</returns>
        public static EndpointDescription SelectEndpoint(
            ApplicationConfiguration application,
            string discoveryUrl,
            bool useSecurity,
            int discoverTimeout
            )
        {
            var uri = GetDiscoveryUrl(discoveryUrl);
            var endpointConfiguration = EndpointConfiguration.Create();
            endpointConfiguration.OperationTimeout = discoverTimeout;

            using (var client = DiscoveryClient.Create(application, uri, endpointConfiguration))
            {
                // Connect to the server's discovery endpoint and find the available configuration.
                var url = new Uri(client.Endpoint.EndpointUrl);
                var endpoints = client.GetEndpoints(null);
                var selectedEndpoint = SelectEndpoint(url, endpoints, useSecurity);

                var endpointUrl = Utils.ParseUri(selectedEndpoint.EndpointUrl);
                if (endpointUrl != null && endpointUrl.Scheme == uri.Scheme)
                {
                    var builder = new UriBuilder(endpointUrl) { Host = uri.DnsSafeHost, Port = uri.Port };
                    selectedEndpoint.EndpointUrl = builder.ToString();
                }

                return selectedEndpoint;
            }
        }

        /// <summary>
        /// Select the best supported endpoint from an
        /// EndpointDescriptionCollection, with or without security.
        /// </summary>
        /// <param name="url">The discovery URL.</param>
        /// <param name="endpoints"></param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <returns>The best available endpoint.</returns>
        public static EndpointDescription SelectEndpoint(
            Uri url,
            EndpointDescriptionCollection endpoints,
            bool useSecurity)
        {
            EndpointDescription selectedEndpoint = null;

            // select the best endpoint to use based on the selected URL and the UseSecurity checkbox. 
            foreach (var endpoint in endpoints)
            {
                // check for a match on the URL scheme.
                if (endpoint.EndpointUrl.StartsWith(url.Scheme))
                {
                    // check if security was requested.
                    if (useSecurity)
                    {
                        if (endpoint.SecurityMode == MessageSecurityMode.None)
                        {
                            continue;
                        }

                        // skip unsupported security policies
                        if (SecurityPolicies.GetDisplayName(endpoint.SecurityPolicyUri) == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (endpoint.SecurityMode != MessageSecurityMode.None)
                        {
                            continue;
                        }
                    }

                    // pick the first available endpoint by default.
                    if (selectedEndpoint == null)
                    {
                        selectedEndpoint = endpoint;
                    }

                    // The security level is a relative measure assigned by the server to the 
                    // endpoints that it returns. Clients should always pick the highest level
                    // unless they have a reason not too.
                    // Some servers however, mess this up a bit. So prefer a higher SecurityMode
                    // over the SecurityLevel.
                    if (endpoint.SecurityMode > selectedEndpoint.SecurityMode
                        || (endpoint.SecurityMode == selectedEndpoint.SecurityMode
                            && endpoint.SecurityLevel > selectedEndpoint.SecurityLevel))
                    {
                        selectedEndpoint = endpoint;
                    }
                }
            }

            // pick the first available endpoint by default.
            if (selectedEndpoint == null && endpoints.Count > 0)
            {
                selectedEndpoint = endpoints.FirstOrDefault(e => e.EndpointUrl?.StartsWith(url.Scheme) == true);
            }

            // return the selected endpoint.
            return selectedEndpoint;
        }

        /// <summary>
        /// Convert the discoveryUrl to a Uri and modify endpoint as per connection scheme if required.
        /// </summary>
        public static Uri GetDiscoveryUrl(string discoveryUrl)
        {
            // needs to add the '/discovery' back onto non-UA TCP URLs.
            if (discoveryUrl.StartsWith(Utils.UriSchemeHttp, StringComparison.Ordinal))
            {
                if (!discoveryUrl.EndsWith(DiscoverySuffix, StringComparison.OrdinalIgnoreCase))
                {
                    discoveryUrl += DiscoverySuffix;
                }
            }

            // parse the selected URL.
            return new Uri(discoveryUrl);
        }
        #endregion
    }
}
