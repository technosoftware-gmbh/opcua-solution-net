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
using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Interface for accessing the configuration of the TransportProtocol
    /// </summary>
    public interface ITransportProtocolConfiguration
    {
        /// <summary>
        /// Retrieve the configuration in KeyValuePairCollection format
        /// </summary>
        /// <returns></returns>
        KeyValuePairCollection ConnectionProperties { get; set; }
    }
}
