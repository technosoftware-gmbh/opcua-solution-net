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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// Class responsible to manage the UDP Discovery Request/Response messages for a <see cref="UdpPubSubConnection"/> entity.
    /// </summary>
    internal abstract class UdpDiscovery
    {
        #region Fields
        private const string DefaultDiscoveryUrl = "opc.udp://224.0.2.14:4840";

        protected readonly object lock_ = new object();
        protected UdpPubSubConnection udpConnection_;
        protected List<UdpClient> discoveryUdpClients_;
        #endregion

        #region Constructors
        /// <summary>
        /// Create new instance of <see cref="UdpDiscovery"/>
        /// </summary>
        /// <param name="udpConnection"></param>
        protected UdpDiscovery(UdpPubSubConnection udpConnection)
        {
            udpConnection_ = udpConnection;

            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the Discovery <see cref="IPEndPoint"/> from <see cref="PubSubConnectionDataType"/>.TransportSettings.
        /// </summary>
        public IPEndPoint DiscoveryNetworkAddressEndPoint { get; private set; }

        /// <summary>
        /// Get the discovery NetworkInterface name from <see cref="PubSubConnectionDataType"/>.TransportSettings.
        /// </summary>
        public string DiscoveryNetworkInterfaceName { get; set; }

        /// <summary>
        /// Get the coresponding <see cref="IServiceMessageContext"/>
        /// </summary>
        public IServiceMessageContext MessageContext { get; private set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Start the UdpDiscovery process
        /// </summary>
        /// <param name="messageContext">The <see cref="IServiceMessageContext"/> object that should be used in encode/decode messages</param>
        /// <returns></returns>
        public virtual async Task StartAsync(IServiceMessageContext messageContext)
        {
            await Task.Run(() => {
                lock (lock_)
                {
                    MessageContext = messageContext;

                    // initialize Discovery channels
                    discoveryUdpClients_ = UdpClientCreator.GetUdpClients(UsedInContext.Discovery, DiscoveryNetworkInterfaceName, DiscoveryNetworkAddressEndPoint);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Start the UdpDiscovery process
        /// </summary>
        /// <returns></returns>
        public virtual async Task StopAsync()
        {
            lock (lock_)
            {
                if (discoveryUdpClients_ != null && discoveryUdpClients_.Count > 0)
                {
                    foreach (var udpClient in discoveryUdpClients_)
                    {
                        udpClient.Close();
                        udpClient.Dispose();
                    }
                    discoveryUdpClients_.Clear();
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize Connection properties from connection configuration object
        /// </summary>
        private void Initialize()
        {
            PubSubConnectionDataType pubSubConnectionConfiguration = udpConnection_.PubSubConnectionConfiguration;

            if (ExtensionObject.ToEncodeable(pubSubConnectionConfiguration.TransportSettings) is DatagramConnectionTransportDataType transportSettings && transportSettings.DiscoveryAddress != null)
            {
                if (ExtensionObject.ToEncodeable(transportSettings.DiscoveryAddress) is NetworkAddressUrlDataType discoveryNetworkAddressUrlState)
                {
                    Utils.Trace(Utils.TraceMasks.Information, "The configuration for connection {0} has custom DiscoveryAddress configuration.",
                              pubSubConnectionConfiguration.Name);

                    DiscoveryNetworkInterfaceName = discoveryNetworkAddressUrlState.NetworkInterface;
                    DiscoveryNetworkAddressEndPoint = UdpClientCreator.GetEndPoint(discoveryNetworkAddressUrlState.Url);
                }
            }

            if (DiscoveryNetworkAddressEndPoint == null)
            {
                Utils.Trace(Utils.TraceMasks.Information, "The configuration for connection {0} will use the default DiscoveryAddress: {1}.",
                              pubSubConnectionConfiguration.Name, DefaultDiscoveryUrl);

                DiscoveryNetworkAddressEndPoint = UdpClientCreator.GetEndPoint(DefaultDiscoveryUrl);
            }
        }
        #endregion

    }
}
