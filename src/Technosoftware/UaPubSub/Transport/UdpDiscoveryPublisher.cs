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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;

using Opc.Ua;

using Technosoftware.UaPubSub.Encoding;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// Class responsible to manage the UDP Discovery Request/Response messages for a <see cref="UdpPubSubConnection"/> entity as a publisher. 
    /// </summary>
    internal class UdpDiscoveryPublisher : UdpDiscovery
    {
        private const int kMinimumResponseInterval = 500;

        // The list that will store the WriterIds that shall be set as DataSetMetaData Response message
        private readonly List<UInt16> m_metadataWriterIdsToSend;
        private int m_responseInterval = kMinimumResponseInterval;

        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="UdpDiscoveryPublisher"/>
        /// </summary>
        /// <param name="udpConnection"></param>
        public UdpDiscoveryPublisher(UdpPubSubConnection udpConnection) : base(udpConnection)
        {
            m_metadataWriterIdsToSend = new List<ushort>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Implementation of StartAsync for the Publisher Discovery
        /// </summary>
        /// <param name="messageContext">The <see cref="IServiceMessageContext"/> object that should be used in encode/decode messages</param>
        /// <returns></returns>
        public override async Task StartAsync(IServiceMessageContext messageContext)
        {
            await base.StartAsync(messageContext).ConfigureAwait(false);

            if (m_discoveryUdpClients != null)
            {
                foreach (UdpClient discoveryUdpClient in m_discoveryUdpClients)
                {
                    try
                    {
                        // attach callback for receiving messages
                        discoveryUdpClient.BeginReceive(OnUadpDiscoveryReceive, discoveryUdpClient);
                    }
                    catch (Exception ex)
                    {
                        Utils.Trace(Utils.TraceMasks.Information, "UdpDiscoveryPublisher: UdpClient '{0}' Cannot receive data. Exception: {1}",
                          discoveryUdpClient.Client.LocalEndPoint, ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Handle Receive event for an UADP channel on Discovery channel
        /// </summary>
        /// <param name="result"></param>
        private void OnUadpDiscoveryReceive(IAsyncResult result)
        {
            // this is what had been passed into BeginReceive as the second parameter:
            UdpClient socket = result.AsyncState as UdpClient;

            if (socket == null)
            {
                return;
            }

            // points towards whoever had sent the message:
            IPEndPoint source = new IPEndPoint(0, 0);
            // get the actual message and fill out the source:
            try
            {
                byte[] message = socket.EndReceive(result, ref source);

                if (message != null)
                {
                    Utils.Trace(Utils.TraceMasks.Information, "OnUadpDiscoveryReceive received message with length {0} from {1}", message.Length, source.Address);

                    if (message.Length > 1)
                    {
                        // call on a new thread
                        Task.Run(() => {
                            ProcessReceivedMessageDiscovery(message, source);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "OnUadpDiscoveryReceive from {0}", source.Address);
            }

            try
            {
                // schedule the next receive operation once reading is done:
                socket.BeginReceive(OnUadpDiscoveryReceive, socket);
            }
            catch (Exception ex)
            {
                Utils.Trace(Utils.TraceMasks.Information, "OnUadpDiscoveryReceive BeginReceive threw Exception {0}", ex.Message);

                lock (m_lock)
                {
                    Renew(socket);
                }
            }
        }

        /// <summary>
        /// Process the bytes received from UADP discovery channel 
        /// </summary>
        private void ProcessReceivedMessageDiscovery(byte[] messageBytes, IPEndPoint source)
        {
            Utils.Trace(Utils.TraceMasks.Information, "UdpDiscoveryPublisher.ProcessReceivedMessageDiscovery from source={0}", source);

            UadpNetworkMessage networkMessage = new UadpNetworkMessage();
            // decode the received message
            networkMessage.Decode(MessageContext, messageBytes, null);

            if (networkMessage.UADPNetworkMessageType == UADPNetworkMessageType.DiscoveryRequest
                    && networkMessage.UADPDiscoveryType == UADPNetworkMessageDiscoveryType.DataSetMetaData
                    && networkMessage.DataSetWriterIds != null)
            {
                Utils.Trace(Utils.TraceMasks.Information, "UdpDiscoveryPublisher.ProcessReceivedMessageDiscovery Request MetaData Received on endpoint {1} for {0}",
                String.Join(", ", networkMessage.DataSetWriterIds), source.Address);

                foreach (UInt16 dataSetWriterId in networkMessage.DataSetWriterIds)
                {
                    lock (m_lock)
                    {
                        if (!m_metadataWriterIdsToSend.Contains(dataSetWriterId))
                        {
                            // collect requested ids
                            m_metadataWriterIdsToSend.Add(dataSetWriterId);
                        }
                    }
                }

                Task.Run(SendResponseDataSetMetaData).ConfigureAwait(false);
            }

            else if (networkMessage.UADPNetworkMessageType == UADPNetworkMessageType.DiscoveryRequest
                    && networkMessage.UADPDiscoveryType == UADPNetworkMessageDiscoveryType.PublisherEndpoint)
            {
                Task.Run(SendResponsePublisherEndpoints).ConfigureAwait(false);
            }

            else if (networkMessage.UADPNetworkMessageType == UADPNetworkMessageType.DiscoveryRequest
                && networkMessage.UADPDiscoveryType == UADPNetworkMessageDiscoveryType.DataSetWriterConfiguration
                && networkMessage.DataSetWriterIds != null)
            {
                Task.Run(SendResponseDataSetWriterConfiguration).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Sends a DataSetMetadata discovery response message
        /// </summary>
        private async Task SendResponseDataSetMetaData()
        {
            await Task.Delay(m_responseInterval).ConfigureAwait(false);

            lock (m_lock)
            {
                if (m_metadataWriterIdsToSend.Count > 0)
                {
                    IList<UaNetworkMessage> responseMessages = m_udpConnection.CreateDataSetMetaDataNetworkMessages(m_metadataWriterIdsToSend.ToArray());

                    foreach (UaNetworkMessage message in responseMessages)
                    {
                        Utils.Trace("UdpDiscoveryPublisher.SendResponseDataSetMetaData before sending message for DataSetWriterId:{0}", message.DataSetWriterId);

                        m_udpConnection.PublishNetworkMessage(message);
                    }
                    m_metadataWriterIdsToSend.Clear();
                }
            }
        }

        /// <summary>
        /// Sends a DataSetWriterConfiguration discovery response message
        /// </summary>
        private async Task SendResponseDataSetWriterConfiguration()
        {
            await Task.Delay(kMinimumResponseInterval).ConfigureAwait(false);
            lock (m_lock)
            {
                IList<UInt16> dataSetWriterIdsToSend = new List<UInt16>();
                if (GetDataSetWriterIds != null)
                {
                    dataSetWriterIdsToSend = GetDataSetWriterIds.Invoke(m_udpConnection.Application);
                }

                if (dataSetWriterIdsToSend.Count > 0)
                {
                    IList<UaNetworkMessage> responsesMessages = m_udpConnection.CreateDataSetWriterCofigurationMessage(
                        dataSetWriterIdsToSend.ToArray());

                    foreach (var responsesMessage in responsesMessages)
                    {
                        Utils.Trace("UdpDiscoveryPublisher.SendResponseDataSetWriterConfiguration Before sending message for DataSetWriterId:{0}", responsesMessage.DataSetWriterId);

                        m_udpConnection.PublishNetworkMessage(responsesMessage);
                    }
                }
            }
        }

        /// <summary>
        ///  Send response PublisherEndpoints
        /// </summary>
        private async Task SendResponsePublisherEndpoints()
        {
            await Task.Delay(kMinimumResponseInterval).ConfigureAwait(false);

            lock (m_lock)
            {
                IList<EndpointDescription> publisherEndpointsToSend = new List<EndpointDescription>();
                if (GetPublisherEndpoints != null)
                {
                    publisherEndpointsToSend = GetPublisherEndpoints.Invoke();
                }

                UaNetworkMessage message = m_udpConnection.CreatePublisherEndpointsNetworkMessage(
                    publisherEndpointsToSend.ToArray(),
                    publisherEndpointsToSend.Count > 0 ? StatusCodes.Good : StatusCodes.BadNotFound,
                    m_udpConnection.PubSubConnectionConfiguration.PublisherId.Value);

                Utils.Trace("UdpDiscoveryPublisher.SendResponsePublisherEndpoints before sending message for PublisherEndpoints.");

                m_udpConnection.PublishNetworkMessage(message);
            }
        }

        /// <summary>
        /// Re initializes the socket 
        /// </summary>
        /// <param name="socket">The socket which should be reinitialized</param>
        private void Renew(UdpClient socket)
        {
            UdpClient newsocket = null;

            if (socket is UdpClientMulticast mcastSocket)
            {
                newsocket = new UdpClientMulticast(mcastSocket.Address, mcastSocket.MulticastAddress, mcastSocket.Port);
            }
            else if (socket is UdpClientBroadcast bcastSocket)
            {
                newsocket = new UdpClientBroadcast(bcastSocket.Address, bcastSocket.Port, bcastSocket.PubSubContext);
            }
            else if (socket is UdpClientUnicast ucastSocket)
            {
                newsocket = new UdpClientUnicast(ucastSocket.Address, ucastSocket.Port);
            }
            m_discoveryUdpClients.Remove(socket);
            m_discoveryUdpClients.Add(newsocket);
            socket.Close();
            socket.Dispose();

            if (newsocket != null)
            {
                newsocket.BeginReceive(OnUadpDiscoveryReceive, newsocket);
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The GetPublisherEndpoints event callback reference to store the EndpointDescription[] to be set as PublisherEndpoints Response message 
        /// </summary>
        public GetPublisherEndpointsEventHandler GetPublisherEndpoints { get; set; }

        /// <summary>
        ///  The GetDataSetWriterIds event callback reference to store the DataSetWriter ids to be set as PublisherEndpoints Response message
        /// </summary>
        public GetDataSetWriterIdsEventHandler GetDataSetWriterIds { get; set; }

        #endregion
    }
}
