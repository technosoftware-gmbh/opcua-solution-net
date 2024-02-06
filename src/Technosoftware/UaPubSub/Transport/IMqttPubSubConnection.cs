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
using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// The interface for the MQTT PubSub connection
    /// </summary>
    public interface IMqttPubSubConnection : IUaPubSubConnection
    {
        /// <summary>
        /// Determine if the connection can publish metadata for specified writer group and data set writer
        /// </summary>
        bool CanPublishMetaData(WriterGroupDataType writerGroupConfiguration, DataSetWriterDataType dataSetWriter);

        /// <summary> 
        /// Create and return the DataSetMetaData message for a DataSetWriter
        /// </summary>
        /// <returns></returns>
        UaNetworkMessage CreateDataSetMetaDataNetworkMessage(WriterGroupDataType writerGroup,
            DataSetWriterDataType dataSetWriter);
    }
}
