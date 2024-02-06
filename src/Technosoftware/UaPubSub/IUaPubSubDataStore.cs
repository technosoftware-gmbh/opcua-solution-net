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

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Interface for a data store component responsible to store/get data to and from Ua publisher
    /// </summary>
    public interface IUaPubSubDataStore
    {
        /// <summary>
        /// Write a DataValue to the DataStore. 
        /// The DataValue is identified by node NodeId and Attribute.
        /// </summary>
        /// <param name="nodeId">NodeId identifier for DataValue that will be stored</param>
        /// <param name="attributeId">Default value is <see cref="Attributes.Value"/>.</param>
        /// <param name="dataValue">Default value is null. </param>
        void WritePublishedDataItem(NodeId nodeId, uint attributeId = Attributes.Value, DataValue dataValue = null);

        /// <summary>
        /// Read the DataValue stored for a specific NodeId and Attribute.
        /// </summary>
        /// <param name="nodeId">NodeId identifier of node</param>
        /// <param name="attributeId">Default value is <see cref="Attributes.Value"/></param>
        /// <returns></returns>
        DataValue ReadPublishedDataItem(NodeId nodeId, uint attributeId = Attributes.Value);

        /// <summary>
        /// Updates the metadata if it has changed from when the DataStore was created.
        /// </summary>
        void UpdateMetaData(PublishedDataSetDataType publishedDataSet);
    }
}
