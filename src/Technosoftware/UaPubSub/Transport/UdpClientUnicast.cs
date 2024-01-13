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

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// Represents a specialized <see cref="UdpClient"/> class, configured for Unicast
    /// </summary>
    internal class UdpClientUnicast : UdpClient
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClient"/> class and binds it to the specified local endpoint 
        /// </summary>
        /// <param name="localAddress">An <see cref="IPAddress"/> that represents the local address.</param>
        /// <param name="port">The port.</param>       
        /// <exception cref="SocketException">An error occurred when accessing the socket.</exception>
        public UdpClientUnicast(IPAddress localAddress, int port) : base()
        {
            Address = localAddress;
            Port = port;

            try
            {
                // this might throw exception on some platforms
                Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "SetSocketOption has thrown exception ");
            }
            try
            {
                // this might throw exception on some platforms
                ExclusiveAddressUse = false;
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Setting ExclusiveAddressUse to false has thrown exception ");
            }

            Client.Bind(new IPEndPoint(localAddress, port));

            Utils.Trace("UdpClientUnicast was created for local Address: {0}:{1}.", localAddress, port);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Unicast Ip Address
        /// </summary>
        internal IPAddress Address { get; }

        /// <summary>
        /// The Port
        /// </summary>
        internal int Port { get; }
        #endregion
    }
}
