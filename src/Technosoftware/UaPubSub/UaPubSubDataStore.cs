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
using System.Collections.Generic;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// DataStore is a repository where Publisher applications will push data values for nodes + attributes published in data sets
    /// </summary>
    public class UaPubSubDataStore : IUaPubSubDataStore
    {
        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="UaPubSubDataStore"/>
        /// </summary>
        public UaPubSubDataStore()
        {
            store_ = new Dictionary<NodeId, Dictionary<uint, DataValue>>();
        }
        #endregion

        #region Read/Write Public Methods
        /// <summary>
        /// Write a value to the DataStore. 
        /// The value is identified by node NodeId.
        /// </summary>
        /// <param name="nodeId">NodeId identifier for value that will be stored.</param>
        /// <param name="value">The value to be store. The value is NOT copied.</param>
        /// <param name="status">The status associated with the value.</param>
        /// <param name="timestamp">The timestamp associated with the value.</param>
        public void WritePublishedDataItem(
            NodeId nodeId,
            Variant value,
            StatusCode? status = null,
            DateTime? timestamp = null)
        {
            if (nodeId == null)
            {
                throw new ArgumentException("The Node ID cannot be null.", nameof(nodeId));
            }

            lock (lock_)
            {
                var dv = new DataValue() {
                    WrappedValue = value,
                    StatusCode = status ?? StatusCodes.Good,
                    SourceTimestamp = timestamp ?? DateTime.UtcNow
                };

                if (!store_.ContainsKey(nodeId))
                {
                    var dictionary = new Dictionary<uint, DataValue>();
                    dictionary.Add(Attributes.Value, dv);
                    store_.Add(nodeId, dictionary);
                }

                store_[nodeId][Attributes.Value] = dv;
            }
        }

        /// <summary>
        /// Write a DataValue to the DataStore. 
        /// The DataValue is identified by node NodeId and Attribute.
        /// </summary>
        /// <param name="nodeId">NodeId identifier for DataValue that will be stored</param>
        /// <param name="attributeId">Default value is <see cref="Attributes.Value"/>.</param>
        /// <param name="dataValue">Default value is null. </param>
        public void WritePublishedDataItem(NodeId nodeId, uint attributeId = Attributes.Value, DataValue dataValue = null)
        {
            if (nodeId == null)
            {
                throw new ArgumentException("The Node ID cannot be null.", nameof(nodeId));
            }
            if (attributeId == 0)
            {
                attributeId = Attributes.Value;
            }
            if (!Attributes.IsValid(attributeId))
            {
                throw new ArgumentException("The Attribute ID is not valid.", nameof(nodeId));
            }
            lock (lock_)
            {
                if (store_.ContainsKey(nodeId))
                {
                    store_[nodeId][attributeId] = dataValue;
                }
                else
                {
                    Dictionary<uint, DataValue> dictionary = new Dictionary<uint, DataValue>();
                    dictionary.Add(attributeId, dataValue);
                    store_.Add(nodeId, dictionary);
                }
            }
        }

        /// <summary>
        /// Read the DataValue stored for a specific NodeId and Attribute.
        /// </summary>
        /// <param name="nodeId">NodeId identifier of node</param>
        /// <param name="attributeId">Default value is <see cref="Attributes.Value"/></param>
        /// <returns></returns>
        public DataValue ReadPublishedDataItem(NodeId nodeId, uint attributeId = Attributes.Value)
        {
            // todo find out why the deltaFrame parameter is not used
            if (nodeId == null)
            {
                throw new ArgumentException("The Node ID cannot be null.", nameof(nodeId));
            }
            if (attributeId == 0)
            {
                attributeId = Attributes.Value;
            }
            if (!Attributes.IsValid(attributeId))
            {
                throw new ArgumentException("The Attribute ID is not valid.", nameof(nodeId));
            }
            lock (lock_)
            {
                if (store_.ContainsKey(nodeId))
                {
                    if (store_[nodeId].ContainsKey(attributeId))
                    {
                        return store_[nodeId][attributeId];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the metadata.
        /// </summary>
        public void UpdateMetaData(PublishedDataSetDataType publishedDataSet)
        {
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private readonly Dictionary<NodeId, Dictionary<uint, DataValue>> store_;
        #endregion
    }
}
