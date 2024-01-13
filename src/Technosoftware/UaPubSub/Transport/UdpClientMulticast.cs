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
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// Represents a specialized <see cref="UdpClient"/> class, configured for Multicast
    /// </summary>
    internal class UdpClientMulticast : UdpClient
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClient"/> class and binds it to the specified local endpoint 
        /// and joins the specified multicast group
        /// </summary>
        /// <param name="localAddress">An <see cref="IPAddress"/> that represents the local address.</param>
        /// <param name="multicastAddress">The multicast <see cref="IPAddress"/> of the group you want to join.</param>
        /// <param name="port">The port.</param>       
        /// <exception cref="SocketException">An error occurred when accessing the socket.</exception>
        public UdpClientMulticast(IPAddress localAddress, IPAddress multicastAddress, int port) : base()
        {
            Address = localAddress;
            MulticastAddress = multicastAddress;
            Port = port;

            try
            {
                // this might throw exception on some platforms
                Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }
            catch (Exception ex)
            {
                Utils.Trace(Utils.TraceMasks.Error, "UdpClientMulticast set SetSocketOption resulted in ex {0}", ex.Message);
            }
            try
            {
                // this might throw exception on some platforms
                ExclusiveAddressUse = false;
            }
            catch (Exception ex)
            {
                Utils.Trace(Utils.TraceMasks.Error, "UdpClientMulticast set ExclusiveAddressUse = false resulted in ex {0}", ex.Message);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Client.Bind(new IPEndPoint(IPAddress.Any, port));
                JoinMulticastGroup(multicastAddress);
            }
            else
            {
                Client.Bind(new IPEndPoint(localAddress, port));
                JoinMulticastGroup(multicastAddress, localAddress);
            }

            Utils.Trace("UdpClientMulticast was created for local Address: {0}:{1} and multicast address: {2}.",
                localAddress, port, multicastAddress);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Local Address
        /// </summary>
        internal IPAddress Address { get; }

        /// <summary>
        /// The Multicast address
        /// </summary>
        internal IPAddress MulticastAddress { get; }

        /// <summary>
        /// The local port
        /// </summary>
        internal int Port { get; }
        #endregion
    }
}
