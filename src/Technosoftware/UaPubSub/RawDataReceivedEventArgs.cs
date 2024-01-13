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

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// EventArgs class for RawData message received event
    /// </summary>
    public class RawDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Get/Set flag that indicates if the RawData message is handled and shall not be decoded by the PubSub library
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Get/Set the message bytes
        /// </summary>
        public byte[] Message { get; set; }

        /// <summary>
        /// Get/Set the message Source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Get/Set the TransportProtocol for the message that was received
        /// </summary>
        public TransportProtocol TransportProtocol { get; set; }

        /// <summary>
        /// Get/Set the current MessageMapping for the message that was received
        /// </summary>
        public MessageMapping MessageMapping { get; set; }

        /// <summary>
        /// Get/Set the PubSubConnection Configuration object for the connection that received this message
        /// </summary>
        public PubSubConnectionDataType PubSubConnectionConfiguration { get; set; }
    }
}
