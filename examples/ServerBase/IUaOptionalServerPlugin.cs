#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace Technosoftware.ServerBase
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