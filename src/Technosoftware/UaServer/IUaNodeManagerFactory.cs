#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// An interface to an object that creates a IUaBaseNodeManager object.
    /// </summary>
    public interface IUaNodeManagerFactory
    {
        /// <summary>
        /// The INodeManager factory.
        /// </summary>
        /// <param name="server">The server instance.</param>
        /// <param name="configuration">The application configuration.</param>
        IUaBaseNodeManager Create(IUaServerData server, ApplicationConfiguration configuration);

        /// <summary>
        /// The namespace table of the NodeManager.
        /// </summary>
        StringCollection NamespacesUris { get; }
    }
}
