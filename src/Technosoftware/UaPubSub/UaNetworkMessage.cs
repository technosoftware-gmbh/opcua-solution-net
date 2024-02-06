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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Abstract class for an UA network message
    /// </summary>
    public abstract class UaNetworkMessage
    {
        #region Public Events
        /// <summary>
        /// The Default event for an error encountered during decoding the dataset messages
        /// </summary>
        public event EventHandler<DataSetDecodeErrorEventArgs> DataSetDecodeErrorOccurredEvent;
        #endregion

        #region Constructor
        /// <summary>
        /// Create instance of <see cref="UaNetworkMessage"/>.
        /// </summary>
        /// <param name="writerGroupConfiguration">The <see cref="WriterGroupDataType"/> confguration object that produced this message.</param>
        /// <param name="uaDataSetMessages">The containing data set messages.</param>
        protected UaNetworkMessage(WriterGroupDataType writerGroupConfiguration, List<UaDataSetMessage> uaDataSetMessages)
        {
            WriterGroupConfiguration = writerGroupConfiguration;
            uaDataSetMessages_ = uaDataSetMessages;
            metadata_ = null;
        }

        /// <summary>
        /// Create instance of <see cref="UaNetworkMessage"/>.
        /// </summary>
        protected UaNetworkMessage(WriterGroupDataType writerGroupConfiguration, DataSetMetaDataType metadata)
        {
            WriterGroupConfiguration = writerGroupConfiguration;
            uaDataSetMessages_ = new List<UaDataSetMessage>();
            metadata_ = metadata;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get and Set WriterGroupId
        /// </summary>
        public UInt16 WriterGroupId { get; set; }

        /// <summary>
        /// Get and Set DataSetWriterId if a single value exists for the message.
        /// </summary>
        public UInt16? DataSetWriterId
        {
            get
            {
                if (dataSetWriterId_ == 0)
                {
                    if (uaDataSetMessages_ != null && uaDataSetMessages_.Count == 1)
                    {
                        return uaDataSetMessages_[0].DataSetWriterId;
                    }

                    return null;
                }

                return ((dataSetWriterId_ != 0) ? dataSetWriterId_ : (UInt16?)null);
            }

            set
            {
                dataSetWriterId_ = (value != null) ? value.Value : (ushort)0;
            }
        }
    
        /// <summary>
        /// DataSet messages
        /// </summary>
        public List<UaDataSetMessage> DataSetMessages
        {
            get
            {
                return uaDataSetMessages_;
            }
        }

        /// <summary>
        /// DataSetMetaData messages
        /// </summary>
        public DataSetMetaDataType DataSetMetaData
        {
            get
            {
                return metadata_;
            }
        }

        /// <summary>
        /// TRUE if it is a metadata message.
        /// </summary>
        public bool IsMetaDataMessage
        {
            get { return metadata_ != null; }
        }

        /// <summary>
        /// Get the writer group configuration for this network message
        /// </summary>
        internal WriterGroupDataType WriterGroupConfiguration { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Encodes the object and returns the resulting byte array.
        /// </summary>
        /// <param name="messageContext">The context.</param>
        public abstract byte[] Encode(IServiceMessageContext messageContext);

        /// <summary>
        /// Encodes the object in the specified stream.
        /// </summary>
        /// <param name="messageContext">The context.</param>
        /// <param name="stream">The stream to use.</param>
        public abstract void Encode(IServiceMessageContext messageContext, Stream stream);

        /// <summary>
        /// Decodes the message
        /// </summary>
        /// <param name="messageContext"></param>
        /// <param name="message"></param>
        /// <param name="dataSetReaders"></param>
        public abstract void Decode(IServiceMessageContext messageContext, byte[] message, IList<DataSetReaderDataType> dataSetReaders);
        #endregion

        #region Protected Fields
        /// <summary>
        /// The DataSetMetaData
        /// </summary>
        protected DataSetMetaDataType metadata_;

        /// <summary>
        /// List of DataSet messages
        /// </summary>
        protected List<UaDataSetMessage> uaDataSetMessages_;
        #endregion

        #region Protected Methods
        /// <summary>
        /// The DataSetDecodeErrorOccurred event handler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataSetDecodeErrorOccurred(DataSetDecodeErrorEventArgs e)
        {
            DataSetDecodeErrorOccurredEvent?.Invoke(this, e);
        }
        #endregion

        #region Private Fields
        private ushort dataSetWriterId_;
        #endregion

    }
}
