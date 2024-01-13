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

namespace Technosoftware.UaPubSub.Configuration
{
    /// <summary>
    /// EventArgs class for <see cref="DataSetWriterDataType"/> Add/Remove events
    /// </summary>
    public class DataSetWriterEventArgs : EventArgs
    {
        /// <summary>
        /// ConfigurationId of parent <see cref="WriterGroupDataType"/> object
        /// </summary>
        public uint WriterGroupId { get; set; }

        /// <summary>
        /// ConfigurationId of <see cref="DataSetWriterDataType"/> object
        /// </summary>
        public uint DataSetWriterId { get; set; }

        /// <summary>
        /// Reference to <see cref="DataSetWriterDataType"/> object
        /// </summary>
        public DataSetWriterDataType DataSetWriterDataType { get; set; }
    }
}
