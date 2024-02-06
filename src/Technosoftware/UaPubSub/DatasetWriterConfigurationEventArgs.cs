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
    /// Class that contains data related to DatasetWriterConfigurationReceived event
    /// </summary>
    public class DataSetWriterConfigurationEventArgs : EventArgs
    {
        /// <summary>
        /// Get the ids of the DataSetWriters
        /// </summary>
        public ushort[] DataSetWriterIds { get; internal set; }

        /// <summary>
        /// Get the received configuration.
        /// </summary>
        public WriterGroupDataType DataSetWriterConfiguration { get; internal set; }

        /// <summary>
        /// Get the source information
        /// </summary>
        public string Source { get; internal set; }

        /// <summary>
        /// Get the publisher Id
        /// </summary>
        public object PublisherId { get; internal set; }

        /// <summary>
        /// Get the statuses code of the DataSetWriter
        /// </summary>
        public StatusCode[] StatusCodes { get; internal set; }
    }
}
