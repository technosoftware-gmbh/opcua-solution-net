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
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// <see cref="EventArgs"/> class for <see cref="UaPubSubApplication.DataReceivedEvent"/> event
    /// </summary>
    public class SubscribedDataEventArgs : EventArgs
    {
        /// <summary>
        /// Get the received NetworkMessage.
        /// </summary>
        public UaNetworkMessage NetworkMessage { get; internal set; }

        /// <summary>
        /// Get the source information
        /// </summary>
        public string Source { get; internal set; }
    }
}
