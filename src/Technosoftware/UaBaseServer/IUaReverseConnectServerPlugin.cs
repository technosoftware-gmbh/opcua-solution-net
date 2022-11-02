#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace Technosoftware.UaBaseServer
{
    /// <summary>
    /// This interface defines functionality which can be implemented by Server Customization DLLs. In general it is
    /// required to implement Reverse Connect Feature. The IUaReverseConnectServerPlugin
    /// interface can be implemented by a UaServerPlugin class implementation.
    /// </summary>
    public interface IUaReverseConnectServerPlugin
    {
        /// <summary>
        /// Specifies whether the server should use reverse connect functionality or not.
        /// </summary>
        /// <returns>
        /// True if server should use reverse functionality; otherwise false.
        /// </returns>
        bool OnUseReverseConnect();
    }
}