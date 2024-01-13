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
using System.Linq;
using System.Threading.Tasks;

using Opc.Ua;

using Technosoftware.UaPubSub.Configuration;
using Technosoftware.UaPubSub.PublishedData;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Abstract class that represents a working connection for PubSub
    /// </summary>
    internal abstract class UaPubSubConnection : IUaPubSubConnection
    {
        #region Constructor
        /// <summary>
        /// Create new instance of UaPubSubConnection with PubSubConnectionDataType configuration data
        /// </summary>
        /// <param name="parentUaPubSubApplication">The OPC UA PubSub application.</param>
        /// <param name="pubSubConnectionDataType">Represent the configuration parameters for PubSubConnections</param>
        internal UaPubSubConnection(UaPubSubApplication parentUaPubSubApplication, PubSubConnectionDataType pubSubConnectionDataType)
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            // set the default message context that uses the GlobalContext
            MessageContext = new ServiceMessageContext {
                NamespaceUris = ServiceMessageContext.GlobalContext.NamespaceUris,
                ServerUris = ServiceMessageContext.GlobalContext.ServerUris
            };

            if (parentUaPubSubApplication == null)
            {
                throw new ArgumentException("The OPC UA PubSub application cannot be null.", nameof(parentUaPubSubApplication));
            }

            uaPubSubApplication_ = parentUaPubSubApplication;
            uaPubSubApplication_.UaPubSubConfigurator.WriterGroupAddedEvent += UaPubSubConfigurator_WriterGroupAdded;
            pubSubConnectionDataType_ = pubSubConnectionDataType;

            publishers_ = new List<IUaPublisher>();

            if (string.IsNullOrEmpty(pubSubConnectionDataType.Name))
            {
                pubSubConnectionDataType.Name = "<connection>";
                Utils.Trace("UaPubSubConnection() received a PubSubConnectionDataType object without name. '<connection>' will be used");
            }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="UaPubSubConnection"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  When overridden in a derived class, releases the unmanaged resources used by that class 
        ///  and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"> true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                uaPubSubApplication_.UaPubSubConfigurator.WriterGroupAddedEvent -= UaPubSubConfigurator_WriterGroupAdded;
                Stop();
                // free managed resources
                foreach (UaPublisher publisher in publishers_)
                {
                    publisher.Dispose();
                }

                Utils.Trace("Connection '{0}' was disposed.", pubSubConnectionDataType_.Name);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the assigned transport protocol for this connection instance
        /// </summary>
        public TransportProtocol TransportProtocol
        {
            get { return transportProtocol_; }
        }

        /// <summary>
        /// Get the configuration object for this PubSub connection
        /// </summary>
        public PubSubConnectionDataType PubSubConnectionConfiguration
        {
            get { return pubSubConnectionDataType_; }
        }

        /// <summary>
        /// Get reference to <see cref="UaPubSubApplication"/>
        /// </summary>
        public UaPubSubApplication Application
        {
            get { return uaPubSubApplication_; }
        }

        /// <summary>
        /// Get flag that indicates if the Connection is in running state
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning_; }
        }

        /// <summary>
        /// Get/Set the current <see cref="IServiceMessageContext"/>
        /// </summary>
        public IServiceMessageContext MessageContext { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start Publish/Subscribe jobs associated with this instance
        /// </summary>
        public void Start()
        {
            InternalStart().Wait();
            Utils.Trace("Connection '{0}' was started.", pubSubConnectionDataType_.Name);

            lock (lock_)
            {
                isRunning_ = true;
                foreach (var publisher in publishers_)
                {
                    publisher.Start();
                }
            }
        }

        /// <summary>
        /// Stop Publish/Subscribe jobs associated with this instance
        /// </summary>
        public void Stop()
        {
            InternalStop().Wait();
            lock (lock_)
            {
                isRunning_ = false;
                foreach (var publisher in publishers_)
                {
                    publisher.Stop();
                }
            }
            Utils.Trace("Connection '{0}' was stopped.", pubSubConnectionDataType_.Name);
        }

        /// <summary>
        /// Determine if the connection has anything to publish -> at least one WriterDataSet is configured as enabled for current writer group
        /// </summary>
        /// <param name="writerGroupConfiguration">Represent the configuration parameters for WriterGroups</param>
        /// <returns></returns>
        public bool CanPublish(WriterGroupDataType writerGroupConfiguration)
        {
            if (!isRunning_)
            {
                return false;
            }
            // check if connection status is operational
            if (Application.UaPubSubConfigurator.FindStateForObject(pubSubConnectionDataType_) != PubSubState.Operational)
            {
                return false;
            }

            if (Application.UaPubSubConfigurator.FindStateForObject(writerGroupConfiguration) != PubSubState.Operational)
            {
                return false;
            }

            foreach (var writer in writerGroupConfiguration.DataSetWriters)
            {
                if (writer.Enabled)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Create the network messages built from the provided writerGroupConfiguration
        /// </summary>
        /// <param name="writerGroupConfiguration">The writer group configuration </param>
        /// <param name="state">The publish state for the writer group.</param>
        /// <returns>A list of the <see cref="UaNetworkMessage"/> created from the provided writerGroupConfiguration.</returns>
        public abstract IList<UaNetworkMessage> CreateNetworkMessages(WriterGroupDataType writerGroupConfiguration, WriterGroupPublishState state);

        /// <summary>
        /// Publish the network message
        /// </summary>
        /// <param name="networkMessage">The network message that needs to be published.</param>
        /// <returns>True if send was successful.</returns>
        public abstract bool PublishNetworkMessage(UaNetworkMessage networkMessage);

        /// <summary>
        /// Get flag that indicates if all the network clients are connected
        /// </summary>
        public abstract bool AreClientsConnected();

        /// <summary>
        /// Get current list of Operational DataSetReaders available in this UaSubscriber component
        /// </summary>
        public List<DataSetReaderDataType> GetOperationalDataSetReaders()
        {
            var readersList = new List<DataSetReaderDataType>();
            if (Application.UaPubSubConfigurator.FindStateForObject(pubSubConnectionDataType_) != PubSubState.Operational)
            {
                return readersList;
            }
            foreach (var readerGroup in pubSubConnectionDataType_.ReaderGroups)
            {
                if (Application.UaPubSubConfigurator.FindStateForObject(readerGroup) == PubSubState.Operational)
                {
                    foreach (var reader in readerGroup.DataSetReaders)
                    {
                        // check if the reader is properly configured to receive data
                        if (Application.UaPubSubConfigurator.FindStateForObject(reader) == PubSubState.Operational)
                        {
                            readersList.Add(reader);
                        }
                    }
                }
            }
            return readersList;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Perform specific Start tasks
        /// </summary>
        protected abstract Task InternalStart();

        /// <summary>
        /// Perform specific Stop tasks
        /// </summary>
        protected abstract Task InternalStop();

        /// <summary>
        /// Processes the decoded <see cref="UaNetworkMessage"/> and
        /// raises the <see cref="UaPubSubApplication.DataReceivedEvent"/> or <see cref="UaPubSubApplication.MetaDataReceivedEvent"/> or <see cref="UaPubSubApplication.DataSetWriterConfigurationReceivedEvent"/> or <see cref="UaPubSubApplication.PublisherEndpointsReceivedEvent"/>event.
        /// </summary>
        /// <param name="networkMessage">The network message that was received.</param>
        /// <param name="source">The source of the received event.</param>
        protected void ProcessDecodedNetworkMessage(UaNetworkMessage networkMessage, string source)
        {
            if (networkMessage.IsMetaDataMessage)
            {
                // update configuration of the corresponding reader objects found in this connection configuration
                var allReaders = GetAllDataSetReaders();
                foreach (var reader in allReaders)
                {
                    var raiseChangedEvent = false;

                    lock (lock_)
                    {
                        // check if reader's MetaData shall be updated
                        if (reader.DataSetWriterId != 0
                            && reader.DataSetWriterId == networkMessage.DataSetWriterId
                            && (reader.DataSetMetaData == null
                            || !Utils.IsEqual(reader.DataSetMetaData.ConfigurationVersion, networkMessage.DataSetMetaData.ConfigurationVersion)))
                        {
                            raiseChangedEvent = true;
                        }
                    }

                    if (raiseChangedEvent)
                    {
                        // raise event
                        ConfigurationUpdatingEventArgs metaDataUpdatedEventArgs = new ConfigurationUpdatingEventArgs() {
                            ChangedProperty = ConfigurationProperty.DataSetMetaData,
                            Parent = reader,
                            NewValue = networkMessage.DataSetMetaData,
                            Cancel = false
                        };

                        // raise the ConfigurationUpdating event and see if configuration shall be changed
                        uaPubSubApplication_.RaiseConfigurationUpdatingEvent(metaDataUpdatedEventArgs);

                        // check to see if the event handler canceled the save of new MetaData
                        if (!metaDataUpdatedEventArgs.Cancel)
                        {
                            Utils.Trace("Connection '{0}' - The MetaData is updated for DataSetReader '{1}' with DataSetWriterId={2}",
                                    source, reader.Name, networkMessage.DataSetWriterId);

                            lock (lock_)
                            {
                                reader.DataSetMetaData = networkMessage.DataSetMetaData;
                            }
                        }
                    }
                }

                SubscribedDataEventArgs subscribedDataEventArgs = new SubscribedDataEventArgs() {
                    NetworkMessage = networkMessage,
                    Source = source
                };

                // trigger notification for received DataSet MetaData
                Application.RaiseMetaDataReceivedEvent(subscribedDataEventArgs);

                Utils.Trace(
                    "Connection '{0}' - RaiseMetaDataReceivedEvent() from source={0}",
                    source,
                    subscribedDataEventArgs.NetworkMessage.DataSetMessages.Count);
            }
            else if (networkMessage.DataSetMessages != null && networkMessage.DataSetMessages.Count > 0)
            {
                SubscribedDataEventArgs subscribedDataEventArgs = new SubscribedDataEventArgs() {
                    NetworkMessage = networkMessage,
                    Source = source
                };

                //trigger notification for received subscribed DataSet
                Application.RaiseDataReceivedEvent(subscribedDataEventArgs);

                Utils.Trace(
                    "Connection '{0}' - RaiseNetworkMessageDataReceivedEvent() from source={0}, with {1} DataSets",
                    source,
                    subscribedDataEventArgs.NetworkMessage.DataSetMessages.Count);
            }
            else if (networkMessage is Encoding.UadpNetworkMessage)
            {
                Encoding.UadpNetworkMessage uadpNetworkMessage = networkMessage as Encoding.UadpNetworkMessage;

                if (uadpNetworkMessage != null)
                {
                    if (uadpNetworkMessage.UADPDiscoveryType == UADPNetworkMessageDiscoveryType.DataSetWriterConfiguration &&
                        uadpNetworkMessage.UADPNetworkMessageType == UADPNetworkMessageType.DiscoveryResponse)
                    {
                        DataSetWriterConfigurationEventArgs eventArgs = new DataSetWriterConfigurationEventArgs() {
                            DataSetWriterIds = uadpNetworkMessage.DataSetWriterIds,
                            Source = source,
                            DataSetWriterConfiguration = uadpNetworkMessage.DataSetWriterConfiguration,
                            PublisherId = uadpNetworkMessage.PublisherId,
                            StatusCodes = uadpNetworkMessage.MessageStatusCodes
                        };

                        //trigger notification for received configuration
                        Application.RaiseDatasetWriterConfigurationReceivedEvent(eventArgs);

                        Utils.Trace(
                            "Connection '{0}' - RaiseDataSetWriterConfigurationReceivedEvent() from source={0}, with {1} DataSetWriterConfiguration",
                            source,
                            eventArgs.DataSetWriterIds.Count());
                    }
                    else if (uadpNetworkMessage.UADPDiscoveryType == UADPNetworkMessageDiscoveryType.PublisherEndpoint &&
                        uadpNetworkMessage.UADPNetworkMessageType == UADPNetworkMessageType.DiscoveryResponse)
            {
                        PublisherEndpointsEventArgs publisherEndpointsEventArgs = new PublisherEndpointsEventArgs() {
                            PublisherEndpoints = uadpNetworkMessage.PublisherEndpoints,
                            Source = source,
                            PublisherId = uadpNetworkMessage.PublisherId,
                            StatusCode = uadpNetworkMessage.PublisherProvideEndpoints
                        };

                        //trigger notification for received publisher endpoints
                        Application.RaisePublisherEndpointsReceivedEvent(publisherEndpointsEventArgs);

                        Utils.Trace(
                            "Connection '{0}' - RaisePublisherEndpointsReceivedEvent() from source={0}, with {1} PublisherEndpoints",
                            source,
                            publisherEndpointsEventArgs.PublisherEndpoints.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Get all dataset readers defined for this UaSubscriber component
        /// </summary>
        protected List<DataSetReaderDataType> GetAllDataSetReaders()
        {
            var readersList = new List<DataSetReaderDataType>();
            foreach (var readerGroup in pubSubConnectionDataType_.ReaderGroups)
            {
                foreach (var reader in readerGroup.DataSetReaders)
                {
                    readersList.Add(reader);
                }
            }
            return readersList;
        }

        /// <summary>
        /// Get all dataset writers defined for this UaPublisher component
        /// </summary>
        protected List<DataSetWriterDataType> GetWriterGroupsDataType()
        {
            var writerList = new List<DataSetWriterDataType>();

            foreach (var writerGroup in pubSubConnectionDataType_.WriterGroups)
            {
                foreach (var writer in writerGroup.DataSetWriters)
                {
                    writerList.Add(writer);
                }
            }
            return writerList;
        }

        /// <summary>
        /// Get data set writer discovery responses
        /// </summary>
        protected IList<DataSetWriterConfigurationResponse> GetDataSetWriterDiscoveryResponses(UInt16[] dataSetWriterIds)
        {
            List<DataSetWriterConfigurationResponse> responses = new List<DataSetWriterConfigurationResponse>();

            List<ushort> writerGroupsIds = pubSubConnectionDataType_.WriterGroups
                .SelectMany(group => group.DataSetWriters)
                .Select(writer => writer.DataSetWriterId)
                .ToList();

            foreach (var dataSetWriterId in dataSetWriterIds)
            {
                DataSetWriterConfigurationResponse response = new DataSetWriterConfigurationResponse();

                if (!writerGroupsIds.Contains(dataSetWriterId))
                {
                    response.DataSetWriterIds = new ushort[] { dataSetWriterId };

                    response.StatusCodes = new StatusCode[] { StatusCodes.BadNotFound };
                }
                else
                {
                    response.DataSetWriterConfig = pubSubConnectionDataType_.WriterGroups
                        .First(group => group.DataSetWriters.First(writer => writer.DataSetWriterId == dataSetWriterId) != null);

                    response.DataSetWriterIds = new ushort[] { dataSetWriterId };

                    response.StatusCodes = new StatusCode[] { StatusCodes.Good };
                }

                responses.Add(response);
            }

            return responses;
        }

        /// <summary>
        /// Get the maximum KeepAlive value from all present WriterGroups
        /// </summary>
        protected double GetWriterGroupsMaxKeepAlive()
        {
            double maxKeepAlive = 0;
            foreach (var writerGroup in pubSubConnectionDataType_.WriterGroups)
            {
                if (maxKeepAlive < writerGroup.KeepAliveTime)
                {
                    maxKeepAlive = writerGroup.KeepAliveTime;
                }
            }
            return maxKeepAlive;
        }

        /// <summary>
        /// Create and return the current DataSet for the provided dataSetWriter according to current WriterGroupPublishState
        /// </summary>
        /// <param name="dataSetWriter">Represent the DataSetWriter parameter.</param>
        /// <param name="state">The publishing state for a writer group.</param>
        /// <returns></returns>
        protected DataSet CreateDataSet(DataSetWriterDataType dataSetWriter, WriterGroupPublishState state)
        {
            DataSet dataSet = null;
            //check if dataSetWriter enabled
            if (dataSetWriter.Enabled)
            {
                var isDeltaFrame = state.IsDeltaFrame(dataSetWriter, out var sequenceNumber);

                dataSet = Application.DataCollector.CollectData(dataSetWriter.DataSetName);

                if (dataSet != null)
                {
                    dataSet.SequenceNumber = sequenceNumber;
                    dataSet.IsDeltaFrame = isDeltaFrame;

                    if (isDeltaFrame)
                    {
                        dataSet = state.ExcludeUnchangedFields(dataSetWriter, dataSet);
                    }
                }
            }

            return dataSet;
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Get the list of current publishers associated with this connection
        /// </summary>
        internal IReadOnlyCollection<IUaPublisher> Publishers
        {
            get { return publishers_.AsReadOnly(); }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handler for <see cref="UaPubSubConfigurator.WriterGroupAddedEvent"/> event. 
        /// </summary>
        private void UaPubSubConfigurator_WriterGroupAdded(object sender, WriterGroupEventArgs e)
        {
            var pubSubConnectionDataType = uaPubSubApplication_.UaPubSubConfigurator.FindObjectById(e.ConnectionId)
                as PubSubConnectionDataType;
            if (pubSubConnectionDataType_ == pubSubConnectionDataType)
            {
                var publisher = new UaPublisher(this, e.WriterGroupDataType);
                publishers_.Add(publisher);
            }
        }
        #endregion

        #region Fields
        protected object lock_ = new object();
        private bool isRunning_;
        private readonly List<IUaPublisher> publishers_;
        private readonly PubSubConnectionDataType pubSubConnectionDataType_;
        private readonly UaPubSubApplication uaPubSubApplication_;
        protected TransportProtocol transportProtocol_ = TransportProtocol.NotAvailable;
        #endregion
    }
}
