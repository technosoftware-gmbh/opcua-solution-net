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

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Interface for UaPublisher implementation
    /// </summary>
    public interface IUaPublisher : IDisposable
    {
        /// <summary>
        /// Get reference to the associated configuration object, the <see cref="WriterGroupDataType"/> instance.
        /// </summary>
        WriterGroupDataType WriterGroupConfiguration { get; }

        /// <summary>
        /// Get reference to the associated parent <see cref="IUaPubSubConnection"/> instance.
        /// </summary>
        IUaPubSubConnection PubSubConnection { get; }

        /// <summary>
        /// Starts the publisher and makes it ready to send data.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the publishing thread.
        /// </summary>
        void Stop();
    }
}
