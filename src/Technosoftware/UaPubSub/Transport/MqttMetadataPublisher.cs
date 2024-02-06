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

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// Entity responsible to trigger DataSetMetaData messages as configured for a <see cref="DataSetWriterDataType"/>.
    /// </summary>
    public class MqttMetadataPublisher
    {
        #region Private Fields
        private readonly IMqttPubSubConnection m_parentConnection;
        private readonly WriterGroupDataType m_writerGroup;
        private readonly DataSetWriterDataType m_dataSetWriter;

        // the component that triggers the publish messages
        private readonly IntervalRunner m_intervalRunner;
        #endregion

        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="MqttMetadataPublisher"/>.
        /// </summary>
        internal MqttMetadataPublisher(IMqttPubSubConnection parentConnection, WriterGroupDataType writerGroup, DataSetWriterDataType dataSetWriter,
            double metaDataUpdateTime)
        {
            m_parentConnection = parentConnection;
            m_writerGroup = writerGroup;
            m_dataSetWriter = dataSetWriter;
            m_intervalRunner = new IntervalRunner(dataSetWriter.DataSetWriterId, metaDataUpdateTime, CanPublish, PublishMessage);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the publisher and makes it ready to send data.
        /// </summary>
        public void Start()
        {
            m_intervalRunner.Start();
            Utils.Trace("The MqttMetadataPublisher for DataSetWriterId '{0}' was started.",
                m_dataSetWriter.DataSetWriterId);
        }

        /// <summary>
        /// Stop the publishing thread.
        /// </summary>
        public virtual void Stop()
        {
            m_intervalRunner.Stop();

            Utils.Trace("The MqttMetadataPublisher for DataSetWriterId '{0}' was stopped.",
                m_dataSetWriter.DataSetWriterId);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Decide if the DataSetWriter can publish metadata
        /// </summary>
        /// <returns></returns>
        private bool CanPublish()
        {
            return m_parentConnection.CanPublishMetaData(m_writerGroup, m_dataSetWriter);
        }

        /// <summary>
        /// Generate and publish the dataset MetaData message
        /// </summary>
        private void PublishMessage()
        {
            try
            {
                UaNetworkMessage metaDataNetworkMessage = m_parentConnection.CreateDataSetMetaDataNetworkMessage(m_writerGroup, m_dataSetWriter);
                if (metaDataNetworkMessage != null)
                {
                    bool success = m_parentConnection.PublishNetworkMessage(metaDataNetworkMessage);
                    Utils.Trace(
                        "MqttMetadataPublisher Publish DataSetMetaData, DataSetWriterId:{0}; success = {1}",
                        m_dataSetWriter.DataSetWriterId, success);
                }
            }
            catch (Exception e)
            {
                // Unexpected exception in PublishMessages
                Utils.Trace(e, "MqttMetadataPublisher.PublishMessages");
            }
        }
        #endregion

        /// <summary>
        /// Holds state of MetaData
        /// </summary>
        public class MetaDataState
        {
            #region Constructor

            /// <summary>
            /// Create new instance of <see cref="MetaDataState"/>
            /// </summary>
            /// <param name="dataSetWriter"></param>
            public MetaDataState(DataSetWriterDataType dataSetWriter)
            {
                DataSetWriter = dataSetWriter;
                LastSendTime = DateTime.MinValue;

                BrokerDataSetWriterTransportDataType transport =
                    ExtensionObject.ToEncodeable(DataSetWriter.TransportSettings)
                        as BrokerDataSetWriterTransportDataType;

                MetaDataUpdateTime = transport?.MetaDataUpdateTime ?? 0;
            }

            #endregion

            #region Properties

            /// <summary>
            /// The DataSetWriter associated with this MetadataState object
            /// </summary>
            public DataSetWriterDataType DataSetWriter { get; set; }

            /// <summary>
            /// Holds the last metadata that was sent
            /// </summary>
            public DataSetMetaDataType LastMetaData { get; set; }

            /// <summary>
            /// Holds the Utc DateTime for the last metadata sent
            /// </summary>
            public DateTime LastSendTime { get; set; }

            /// <summary>
            /// The configured interval when the metadata shall be sent
            /// </summary>
            public double MetaDataUpdateTime { get; set; }

            #endregion

            /// <summary>
            /// Get the next publish interval
            /// </summary>
            /// <returns></returns>
            public double GetNextPublishInterval()
            {
                return Math.Max(0, MetaDataUpdateTime - DateTime.UtcNow.Subtract(LastSendTime).TotalMilliseconds);
            }
        }
    }
}
