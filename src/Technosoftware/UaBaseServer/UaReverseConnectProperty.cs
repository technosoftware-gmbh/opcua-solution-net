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

namespace Technosoftware.UaBaseServer
{
    /// <summary>
    /// Describes the properties of a server reverse connection.
    /// </summary>
    public class UaReverseConnectProperty
    {
        /// <summary>
        /// Initialize a reverse connect server property.
        /// </summary>
        /// <param name="clientUrl">The Url of the reverse connect client.</param>
        /// <param name="timeout">The timeout to use for a reverse connect attempt.</param>
        /// <param name="maxSessionCount">The maximum number of sessions allowed to the client.</param>
        /// <param name="configEntry">If this is an application configuration entry.</param>
        /// <param name="enabled">If the connection is enabled.</param>
        public UaReverseConnectProperty(
            Uri clientUrl,
            int timeout,
            int maxSessionCount,
            bool configEntry,
            bool enabled = true)
        {
            ClientUrl = clientUrl;
            Timeout = timeout > 0 ? timeout : UaBaseServer.DefaultReverseConnectTimeout;
            MaxSessionCount = maxSessionCount;
            ConfigEntry = configEntry;
            Enabled = enabled;
        }

        /// <summary>
        /// The Url of the reverse connect client.
        /// </summary>
        public readonly Uri ClientUrl;

        /// <summary>
        /// The timeout to use for a reverse connect attempt.
        /// </summary>
        public readonly int Timeout;

        /// <summary>
        /// If this is an application configuration entry.
        /// </summary>
        public readonly bool ConfigEntry;

        /// <summary>
        /// The service result of the last connection attempt.
        /// </summary>
        public ServiceResult ServiceResult;

        /// <summary>
        /// The last state of the reverse connection.
        /// </summary>
        public UaReverseConnectState LastState = UaReverseConnectState.Closed;

        /// <summary>
        /// The maximum number of sessions allowed to the client.
        /// </summary>
        public int MaxSessionCount;

        /// <summary>
        /// If the connection is enabled.
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// The time when the connection was rejected.
        /// </summary>
        public DateTime RejectTime;
    }
}