#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using Opc.Ua;
using Opc.Ua.Test;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Server;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Subscriptions;
#endregion

namespace Technosoftware.UaStandardServer
{
    /// <summary>
    /// Reverse connection states.
    /// </summary>
    public enum UaReverseConnectState
    {
        /// <summary>
        /// The connection is closed.
        /// </summary>
        Closed = 0,

        /// <summary>
        /// The server is connecting.
        /// </summary>
        Connecting = 1,

        /// <summary>
        /// The server is connected with a client.
        /// </summary>
        Connected = 2,

        /// <summary>
        /// The client rejected the connection with the server.
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// An error occurred connecting with the client.
        /// </summary>
        Errored = 4
    }
}