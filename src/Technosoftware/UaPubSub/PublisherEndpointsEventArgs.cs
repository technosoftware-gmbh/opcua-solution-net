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
    /// Class that contains data related to PublisherEndpoints event
    /// </summary>
    public class PublisherEndpointsEventArgs : EventArgs
    {
        /// <summary>
        /// Get the received Publisher identifier.
        /// </summary>
        public object PublisherId { get; internal set; }

        /// <summary>
        /// Get the source information
        /// </summary>
        public string Source { get; internal set; }

        /// <summary>
        /// Get the received Publisher Endpoints.
        /// </summary>
        public EndpointDescription[] PublisherEndpoints { get; internal set; }

        /// <summary>
        /// Get the status code of the DataSetWriter
        /// </summary>
        public StatusCode StatusCode { get; internal set; }
    }
}
