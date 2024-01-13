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
using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Data Set Writer Configuration message
    /// </summary>
    public class DataSetWriterConfigurationResponse
    {
        /// <summary>
        /// DataSetWriterIds contained in the configuration information.
        /// </summary>
        public ushort[] DataSetWriterIds { get; set; }

        /// <summary>
        /// The field shall contain only the entry for the requested or changed DataSetWriters in the WriterGroup.
        /// </summary>
        public WriterGroupDataType DataSetWriterConfig { get; set; }

        /// <summary>
        /// Status codes indicating the capability of the Publisher to provide 
        /// configuration information for the DataSetWriterIds.The size of the array
        /// shall match the size of the DataSetWriterIds array.
        /// </summary>
        public StatusCode[] StatusCodes { get; set; }
    }
}
