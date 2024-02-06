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
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// This class handles the broadcast message sending.
    /// It enables fine tuning the routing option of the internal socket and binding to a specified endpoint so that the messages are routed on a corresponding 
    /// interface (the one to which the endpoint belongs to).
    /// </summary>
    internal class UdpClientBroadcast : UdpClient
    {
        #region Constructors
        /// <summary>
        /// Instantiates a UDP Broadcast client 
        /// </summary>
        /// <param name="address">The IPAddress which the socket should be bound to</param>
        /// <param name="port">The port used by the endpoint that should different than 0 on a Subscriber context</param>
        /// <param name="pubSubContext">The context in which the UDP client is to be used </param>
        public UdpClientBroadcast(IPAddress address, int port, UsedInContext pubSubContext)
        {
            Address = address;
            Port = port;
            PubSubContext = pubSubContext;

            CustomizeSocketToBroadcastThroughIf();

            IPEndPoint boundEndpoint = null;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || pubSubContext == UsedInContext.Publisher)
            {
                //Running on Windows or Publisher on Windows/Linux
                boundEndpoint = new IPEndPoint(address, port);
            }
            else
            {
                //Running on Linux and Subscriber
                // On Linux must bind to IPAddress.Any on receiving side to get Broadcast messages 
                boundEndpoint = new IPEndPoint(IPAddress.Any, port);
            }

            Client.Bind(boundEndpoint);
            EnableBroadcast = true;

            Utils.Trace("UdpClientBroadcast was created for address: {0}:{1} - {2}.", address, port, pubSubContext);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Ip Address
        /// </summary>
        internal IPAddress Address { get; }

        /// <summary>
        /// The port
        /// </summary>
        internal int Port { get; }

        /// <summary>
        /// Publisher or Subscriber context where the UdpClient is used
        /// </summary>
        internal UsedInContext PubSubContext { get; }
        #endregion

        #region Private methods
        /// <summary>
        /// Explicitly specifies that routing the packets to a specific interface is enabled
        /// and should broadcast only on the interface (to which the socket is bound)
        /// </summary>
        private void CustomizeSocketToBroadcastThroughIf()
        {
            Action<SocketOptionLevel, SocketOptionName, bool> setSocketOption = (SocketOptionLevel socketOptionLevel, SocketOptionName socketOptionName, bool value) => {
                try
                {
                    Client.SetSocketOption(socketOptionLevel, socketOptionName, value);
                }
                catch (Exception ex)
                {
                    Utils.Trace(Utils.TraceMasks.Information, "UdpClientBroadcast set SetSocketOption {1} to {2} resulted in ex {0}", ex.Message, SocketOptionName.Broadcast, value);
                };
            };
            setSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            setSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, false);
            setSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    ExclusiveAddressUse = false;
                }
                catch (Exception ex)
                {
                    Utils.Trace(Utils.TraceMasks.Information, "UdpClientBroadcast set ExclusiveAddressUse to false resulted in ex {0}", ex.Message);
                }

            }
        }
        #endregion
    }
}
