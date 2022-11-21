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
    /// required to implement Historical Events, Historical Access, Alarms and Conditions or more advanced servers. 
    /// The IUaOptionalServerPlugin interface can be implemented by a UaServerPlugin class implementation.
    /// </summary>
    public interface IUaOptionalServerPlugin
    {
        /// <summary>
        /// Get the server manager to be used.
        /// </summary>
        /// <returns>
        /// The server manager class based on the UaBaseServer class. If null is returned the UaBaserServer class is used
        /// as the standard version.
        /// </returns>
        UaBaseServer OnGetServer();

        /// <summary>
        /// Get the node manager to be used for the server.
        /// </summary>
        /// <param name="opcServer">The generic server object. Used to call methods the generic server provides.</param>
        /// <param name="uaServer">Server object that provides access to the shared components of the UA Server.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="namespaceUris">Array of namespaces that are used by the application.</param>
        /// <returns>
        /// The node manager class based on the UaBaseNodeManager class. If null is returned the generic server use the
        /// standard version of the UaBaseNodeManager.
        /// </returns>
        UaBaseNodeManager OnGetNodeManager(IUaServer opcServer, IUaServerData uaServer,
            ApplicationConfiguration configuration, params string[] namespaceUris);
    }
}