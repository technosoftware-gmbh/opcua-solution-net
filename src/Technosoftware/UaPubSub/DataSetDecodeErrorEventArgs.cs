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
    /// Class that contains data related to DataSetDecodeErrorOccurred event 
    /// </summary>
    public class DataSetDecodeErrorEventArgs : EventArgs
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSetDecodeErrorReason"></param>
        /// <param name="networkMessage"></param>
        /// <param name="dataSetReader"></param>
        public DataSetDecodeErrorEventArgs(DataSetDecodeErrorReason dataSetDecodeErrorReason, UaNetworkMessage networkMessage, DataSetReaderDataType dataSetReader)
        {
            dataSetDecodeErrorReason_ = dataSetDecodeErrorReason;
            networkMessage_ = networkMessage;
            dataSetReader_ = dataSetReader;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The reason for triggering the DataSetDecodeErrorOccurred event
        /// </summary>
        public DataSetDecodeErrorReason DecodeErrorReason
        {
            get
            {
                return dataSetDecodeErrorReason_;
            }
            set
            {
                dataSetDecodeErrorReason_ = value;
            }
        }

        /// <summary>
        /// The DataSetMessage on which the decoding operated
        /// </summary>
        public UaNetworkMessage UaNetworkMessage
        {
            get
            {
                return networkMessage_;
            }
            set
            {
                networkMessage_ = value;
            }
        }
        /// <summary>
        /// The DataSetReader used by the decoding operation
        /// </summary>
        public DataSetReaderDataType DataSetReader
        {
            get
            {
                return dataSetReader_;
            }
            set
            {
                dataSetReader_ = value;
            }
        }
        #endregion

        #region Private Fields
        private DataSetDecodeErrorReason dataSetDecodeErrorReason_;
        private UaNetworkMessage networkMessage_;
        private DataSetReaderDataType dataSetReader_;
        #endregion

    }
}
