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
        private const string kDefaultDiscoveryUrl = "opc.udp://224.0.2.14:4840";

        protected object m_lock = new object();
        protected UdpPubSubConnection m_udpConnection;
        protected List<UdpClient> m_discoveryUdpClients;
        #endregion

        #region Constructors
        /// <summary>
        /// Create new instance of <see cref="UdpDiscovery"/>
        /// </summary>
        /// <param name="udpConnection"></param>
        protected UdpDiscovery(UdpPubSubConnection udpConnection)
        {
            m_udpConnection = udpConnection;

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
                lock (m_lock)
                {
                    MessageContext = messageContext;

                    // initialize Discovery channels
                    m_discoveryUdpClients = UdpClientCreator.GetUdpClients(UsedInContext.Discovery, DiscoveryNetworkInterfaceName, DiscoveryNetworkAddressEndPoint);                    
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Start the UdpDiscovery process
        /// </summary>
        /// <returns></returns>
        public virtual async Task StopAsync()
        {
            lock (m_lock)
            {
                if (m_discoveryUdpClients != null && m_discoveryUdpClients.Count > 0)
                {
                    foreach (var udpClient in m_discoveryUdpClients)
                    {
                        udpClient.Close();
                        udpClient.Dispose();
                    }
                    m_discoveryUdpClients.Clear();
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
            PubSubConnectionDataType pubSubConnectionConfiguration = m_udpConnection.PubSubConnectionConfiguration;
            DatagramConnectionTransportDataType transportSettings = ExtensionObject.ToEncodeable(pubSubConnectionConfiguration.TransportSettings)
                       as DatagramConnectionTransportDataType;

            if (transportSettings != null && transportSettings.DiscoveryAddress != null)
            {
                NetworkAddressUrlDataType discoveryNetworkAddressUrlState = ExtensionObject.ToEncodeable(transportSettings.DiscoveryAddress)
                       as NetworkAddressUrlDataType;
                if (discoveryNetworkAddressUrlState != null)
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
                              pubSubConnectionConfiguration.Name, kDefaultDiscoveryUrl);

                DiscoveryNetworkAddressEndPoint = UdpClientCreator.GetEndPoint(kDefaultDiscoveryUrl);
            }
        }

        
        
        #endregion

    }
}
