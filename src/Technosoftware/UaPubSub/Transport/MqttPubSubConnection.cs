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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;

using Opc.Ua;

using Technosoftware.UaPubSub.Encoding;
using Technosoftware.UaPubSub.PublishedData;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// MQTT implementation of <see cref="UaPubSubConnection"/> class.
    /// </summary>
    internal class MqttPubSubConnection : UaPubSubConnection, IMqttPubSubConnection
    {
        #region Private Fields
        private string brokerHostName_ = "localhost";
        private string urlScheme_;
        private int brokerPort_ = Utils.MqttDefaultPort;
        private readonly int reconnectIntervalSeconds_ = 5;

        private IMqttClient publisherMqttClient_;
        private IMqttClient subscriberMqttClient_;
        private readonly MessageMapping messageMapping_;
        private readonly MessageCreator messageCreator_;

        private CertificateValidator certificateValidator_;
        private MqttClientTlsOptions mqttClientTlsOptions_;

        private readonly List<MqttMetadataPublisher> m_metaDataPublishers = new List<MqttMetadataPublisher>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the host name or IP address of the broker.
        /// </summary>
        public string BrokerHostName { get => brokerHostName_; }

        /// <summary>
        /// Gets the port of the mqttConnection.
        /// </summary>
        public int BrokerPort { get { return brokerPort_; } }

        /// <summary>
        /// Gets the scheme of the Url.
        /// </summary>
        public string UrlScheme { get => urlScheme_; }
        #endregion Public Properties

        #region Constants
        /// <summary>
        /// Value in seconds with which to surpass the max keep alive value found.
        /// </summary>
        private readonly int MaxKeepAliveIncrement = 5;
        #endregion

        #region Constructor
        /// <summary>
        ///  Create new instance of <see cref="MqttPubSubConnection"/> from <see cref="PubSubConnectionDataType"/> configuration data
        /// </summary>
        /// <param name="uaPubSubApplication"></param>
        /// <param name="pubSubConnectionDataType"></param>
        /// <param name="messageMapping"></param>
        public MqttPubSubConnection(UaPubSubApplication uaPubSubApplication, PubSubConnectionDataType pubSubConnectionDataType, MessageMapping messageMapping)
            : base(uaPubSubApplication, pubSubConnectionDataType)
        {
            transportProtocol_ = TransportProtocol.MQTT;
            messageMapping_ = messageMapping;

            // initialize the message creators for current message 
            if (messageMapping_ == MessageMapping.Json)
            {
                messageCreator_ = new JsonMessageCreator(this);
            }
            else if (messageMapping_ == MessageMapping.Uadp)
            {
                messageCreator_ = new UadpMessageCreator(this);
            }
            else
            {
                Utils.Trace(Utils.TraceMasks.Error, "The current MessageMapping {0} does not have a valid message creator", messageMapping_);
            }
            Utils.Trace("MqttPubSubConnection with name '{0}' was created.", pubSubConnectionDataType.Name);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Determine if the connection can publish metadata for specified writer group and data set writer
        /// </summary>
        public bool CanPublishMetaData(WriterGroupDataType writerGroupConfiguration,
            DataSetWriterDataType dataSetWriter)
        {
            return !CanPublish(writerGroupConfiguration)
                ? false
                : Application.UaPubSubConfigurator.FindStateForObject(dataSetWriter) == PubSubState.Operational;
        }

        /// <summary>
        /// Create the list of network messages built from the provided writerGroupConfiguration
        /// </summary>
        public override IList<UaNetworkMessage> CreateNetworkMessages(WriterGroupDataType writerGroupConfiguration, WriterGroupPublishState state)
        {
            if (!(ExtensionObject.ToEncodeable(writerGroupConfiguration.TransportSettings) is BrokerWriterGroupTransportDataType))
            {
                //Wrong configuration of writer group MessageSettings
                return null;
            }

            if (messageCreator_ != null)
            {
                return messageCreator_.CreateNetworkMessages(writerGroupConfiguration, state);
            }

            // no other encoding is implemented
            return null;
        }

        /// <summary> 
        /// Create and return the DataSetMetaData message for a DataSetWriter
        /// </summary>
        /// <returns></returns>
        public UaNetworkMessage CreateDataSetMetaDataNetworkMessage(WriterGroupDataType writerGroup, DataSetWriterDataType dataSetWriter)
        {
            PublishedDataSetDataType publishedDataSet = Application.DataCollector.GetPublishedDataSet(dataSetWriter.DataSetName);
            if (publishedDataSet != null && publishedDataSet.DataSetMetaData != null)
            {
                if (messageCreator_ != null)
                {
                    return messageCreator_.CreateDataSetMetaDataNetworkMessage(writerGroup,
                        dataSetWriter.DataSetWriterId, publishedDataSet.DataSetMetaData);
                }
            }
            return null;
        }

        /// <summary>
        /// Publish the network message
        /// </summary>
        public override bool PublishNetworkMessage(UaNetworkMessage networkMessage)
        {
            if (networkMessage == null || publisherMqttClient_ == null)
            {
                return false;
            }

            try
            {
                lock (lock_)
                {
                    if (publisherMqttClient_ != null && publisherMqttClient_.IsConnected)
                    {
                        // get the encoded bytes
                        var bytes = networkMessage.Encode(MessageContext);

                        try
                        {
                            string queueName = null;
                            BrokerTransportQualityOfService qos = BrokerTransportQualityOfService.AtLeastOnce;

                            // the network messages that have DataSetWriterId are either metaData messages or SingleDataSet messages and 
                            if (networkMessage.DataSetWriterId != null)
                            {
                                DataSetWriterDataType dataSetWriter = networkMessage.WriterGroupConfiguration.DataSetWriters
                                    .Find(x => x.DataSetWriterId == networkMessage.DataSetWriterId);

                                if (dataSetWriter != null)
                                {
                                    if (ExtensionObject
                                        .ToEncodeable(dataSetWriter.TransportSettings) is BrokerDataSetWriterTransportDataType transportSettings)
                                    {
                                        qos = transportSettings.RequestedDeliveryGuarantee;

                                        queueName = networkMessage.IsMetaDataMessage ? transportSettings.MetaDataQueueName : transportSettings.QueueName;
                                    }
                                }
                            }

                            if (queueName == null || qos == BrokerTransportQualityOfService.NotSpecified)
                            {
                                if (ExtensionObject.ToEncodeable(
                                    networkMessage.WriterGroupConfiguration.TransportSettings) is BrokerWriterGroupTransportDataType transportSettings)
                                {
                                    if (queueName == null)
                                    {
                                        queueName = transportSettings.QueueName;
                                    }
                                    // if the value is not specified and the value of the parent object shall be used
                                    if (qos == BrokerTransportQualityOfService.NotSpecified)
                                    {
                                        qos = transportSettings.RequestedDeliveryGuarantee;
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(queueName))
                            {
                                var message = new MqttApplicationMessage {
                                    Topic = queueName,
                                    PayloadSegment = new ArraySegment<byte>(bytes),
                                    QualityOfServiceLevel = GetMqttQualityOfServiceLevel(qos),
                                    Retain = networkMessage.IsMetaDataMessage
                                };

                                _ = publisherMqttClient_.PublishAsync(message).GetAwaiter().GetResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.Trace(ex, "MqttPubSubConnection.PublishNetworkMessage");
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "MqttPubSubConnection.PublishNetworkMessage");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Get flag that indicates if all the network connections are active and connected
        /// </summary>
        public override bool AreClientsConnected()
        {
            // Check if existing clients are connected
            return (publisherMqttClient_ == null || publisherMqttClient_.IsConnected)
                && (subscriberMqttClient_ == null || subscriberMqttClient_.IsConnected);
        }
        #endregion Public Methods

        #region Protected Methods
        /// <summary>
        /// Perform specific Start tasks
        /// </summary>
        protected override async Task InternalStart()
        {
            //cleanup all existing MQTT connections previously open
            await InternalStop().ConfigureAwait(false);

            lock (lock_)
            {
                if (!(ExtensionObject.ToEncodeable(
                    PubSubConnectionConfiguration.Address) is NetworkAddressUrlDataType networkAddressUrlState))
                {
                    Utils.Trace(
                        Utils.TraceMasks.Error,
                        "The configuration for mqttConnection {0} has invalid Address configuration.",
                        PubSubConnectionConfiguration.Name);

                    return;
                }

                Uri connectionUri;
                urlScheme_ = null;

                if (networkAddressUrlState.Url != null && Uri.TryCreate(networkAddressUrlState.Url, UriKind.Absolute, out connectionUri))
                {
                    if ((connectionUri.Scheme == Utils.UriSchemeMqtt) || (connectionUri.Scheme == Utils.UriSchemeMqtts))
                    {
                        if (!String.IsNullOrEmpty(connectionUri.Host))
                        {
                            brokerHostName_ = connectionUri.Host;
                            brokerPort_ = (connectionUri.Port > 0) ? connectionUri.Port : ((connectionUri.Scheme == Utils.UriSchemeMqtt) ? 1883 : 8883);
                            urlScheme_ = connectionUri.Scheme;
                        }
                    }
                }

                if (urlScheme_ == null)
                {
                    Utils.Trace(
                        Utils.TraceMasks.Error,
                        "The configuration for mqttConnection {0} has invalid MQTT URL '{1}'.",
                        PubSubConnectionConfiguration.Name,
                        networkAddressUrlState.Url);

                    return;
                }

                // create the DataSetMetaData publishers
                foreach (WriterGroupDataType writerGroup in PubSubConnectionConfiguration.WriterGroups)
                {
                    foreach (DataSetWriterDataType dataSetWriter in writerGroup.DataSetWriters)
                    {
                        if (dataSetWriter.DataSetWriterId == 0)
                        {
                            continue;
                        }

                        if (!(ExtensionObject.ToEncodeable(dataSetWriter.TransportSettings) is BrokerDataSetWriterTransportDataType transport) || transport.MetaDataUpdateTime == 0)
                        {
                            continue;
                        }

                        m_metaDataPublishers.Add(new MqttMetadataPublisher(this, writerGroup, dataSetWriter, transport.MetaDataUpdateTime));
                    }
                }

                // start the mqtt metadata publishers
                foreach (MqttMetadataPublisher metaDataPublisher in m_metaDataPublishers)
                {
                    metaDataPublisher.Start();
                }
            }

            MqttClient publisherClient = null;
            MqttClient subscriberClient = null;
            MqttClientOptions mqttOptions = GetMqttClientOptions();

            var nrOfPublishers = Publishers.Count;
            var nrOfSubscribers = GetAllDataSetReaders().Count;

            //publisher initialization
            if (nrOfPublishers > 0)
            {
                publisherClient = (MqttClient)await MqttClientCreator.GetMqttClientAsync(
                    reconnectIntervalSeconds_,
                    mqttOptions,
                    null).ConfigureAwait(false);
            }

            //subscriber initialization
            if (nrOfSubscribers > 0)
            {
                // collect all topics from all ReaderGroups
                var topics = new StringCollection();
                foreach (ReaderGroupDataType readerGroup in PubSubConnectionConfiguration.ReaderGroups)
                {
                    if (!readerGroup.Enabled)
                    {
                        continue;
                    }

                    foreach (DataSetReaderDataType dataSetReader in readerGroup.DataSetReaders)
                    {
                        if (!dataSetReader.Enabled)
                        {
                            continue;
                        }


                        if (ExtensionObject.ToEncodeable(dataSetReader.TransportSettings) is BrokerDataSetReaderTransportDataType brokerTransportSettings && !topics.Contains(brokerTransportSettings.QueueName))
                        {
                            topics.Add(brokerTransportSettings.QueueName);

                            if (brokerTransportSettings.MetaDataQueueName != null)
                            {
                                topics.Add(brokerTransportSettings.MetaDataQueueName);
                            }
                        }
                    }
                }

                subscriberClient = (MqttClient)await MqttClientCreator.GetMqttClientAsync(
                    reconnectIntervalSeconds_,
                    mqttOptions,
                    ProcessMqttMessage,
                    topics).ConfigureAwait(false);
            }

            lock (lock_)
            {
                publisherMqttClient_ = publisherClient;
                subscriberMqttClient_ = subscriberClient;
            }

            Utils.Trace("Connection '{0}' started {1} publishers and {2} subscribers.",
                PubSubConnectionConfiguration.Name, nrOfPublishers, nrOfSubscribers);
        }

        /// <summary>
        /// Perform specific Stop tasks
        /// </summary>
        protected override async Task InternalStop()
        {
            IMqttClient publisherMqttClient = publisherMqttClient_;
            IMqttClient subscriberMqttClient = subscriberMqttClient_;

            void DisposeCerts(X509CertificateCollection certificates)
            {
                if (certificates != null)
                {
                    // dispose certificates
                    foreach (X509Certificate cert in certificates)
                    {
                        Utils.SilentDispose(cert);
                    }
                }
            }
            async Task InternalStop(IMqttClient client)
            {
                if (client != null)
                {
                    X509CertificateCollection certificates = client.Options?.ChannelOptions?.TlsOptions?.ClientCertificatesProvider?.GetCertificates();
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync().ContinueWith((e) => {
                            DisposeCerts(certificates);
                            Utils.SilentDispose(client);
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        DisposeCerts(certificates);
                        Utils.SilentDispose(client);
                    }
                }
            }
            await InternalStop(publisherMqttClient).ConfigureAwait(false);
            await InternalStop(subscriberMqttClient).ConfigureAwait(false);

            if (m_metaDataPublishers != null)
            {
                foreach (MqttMetadataPublisher metaDataPublisher in m_metaDataPublishers)
                {
                    metaDataPublisher.Stop();
                }
                m_metaDataPublishers.Clear();
            }

            lock (lock_)
            {
                publisherMqttClient_ = null;
                subscriberMqttClient_ = null;
                mqttClientTlsOptions_ = null;
            }
        }
        #endregion Protected Methods

        #region Private Methods

        private static bool MatchTopic(string pattern, string topic)
        {
            if (String.IsNullOrEmpty(pattern) || pattern == "#")
            {
                return true;
            }

            var fields1 = pattern.Split('/');
            var fields2 = topic.Split('/');

            for (var ii = 0; ii < fields1.Length && ii < fields2.Length; ii++)
            {
                if (fields1[ii] == "#")
                {
                    return true;
                }

                if (fields1[ii] != "+" && fields1[ii] != fields2[ii])
                {
                    return false;
                }
            }

            return fields1.Length == fields2.Length;
        }

        /// <summary>
        /// Processes a message from the MQTT broker.
        /// </summary>
        /// <param name="eventArgs"></param>
        private Task ProcessMqttMessage(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var topic = eventArgs.ApplicationMessage.Topic;

            Utils.Trace("MQTTConnection - ProcessMqttMessage() received from topic={0}", topic);

            // get the datasetreaders for received message topic
            var dataSetReaders = new List<DataSetReaderDataType>();
            foreach (DataSetReaderDataType dsReader in GetOperationalDataSetReaders())
            {
                if (dsReader == null)
                {
                    continue;
                }

                var brokerDataSetReaderTransportDataType =
                    ExtensionObject.ToEncodeable(dsReader.TransportSettings)
                       as BrokerDataSetReaderTransportDataType;

                var queueName = brokerDataSetReaderTransportDataType.QueueName;
                var metadataQueueName = brokerDataSetReaderTransportDataType.MetaDataQueueName;

                if (!MatchTopic(queueName, topic))
                {
                    if (String.IsNullOrEmpty(metadataQueueName))
                    {
                        continue;
                    }

                    if (!MatchTopic(metadataQueueName, topic))
                    {
                        continue;
                    }
                }

                // At this point the message is accepted 
                // if ((topic.Length == queueName.Length) && (topic == queueName)) || (queueName == #)
                dataSetReaders.Add(dsReader);
            }

            if (dataSetReaders.Count > 0)
            {
                // raise RawData received event
                var rawDataReceivedEventArgs = new RawDataReceivedEventArgs() {
                    Message = eventArgs.ApplicationMessage.PayloadSegment.Array,
                    Source = topic,
                    TransportProtocol = TransportProtocol,
                    MessageMapping = messageMapping_,
                    PubSubConnectionConfiguration = PubSubConnectionConfiguration
                };

                // trigger notification for received raw data
                Application.RaiseRawDataReceivedEvent(rawDataReceivedEventArgs);

                // check if the RawData message is marked as handled
                if (rawDataReceivedEventArgs.Handled)
                {
                    Utils.Trace("MqttConnection message from topic={0} is marked as handled and will not be decoded.", topic);
                    return Task.CompletedTask;
                }

                // initialize the expected NetworkMessage
                UaNetworkMessage networkMessage = messageCreator_.CreateNewNetworkMessage();

                // trigger message decoding
                if (networkMessage != null)
                {
                    networkMessage.Decode(MessageContext, eventArgs.ApplicationMessage.PayloadSegment.Array, dataSetReaders);

                    // Handle the decoded message and raise the necessary event on UaPubSubApplication 
                    ProcessDecodedNetworkMessage(networkMessage, topic);
                }
            }
            else
            {
                Utils.Trace("MqttConnection - ProcessMqttMessage() No DataSetReader is registered for topic={0}.", topic);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Transform pub sub setting into MqttNet enum
        /// </summary>
        /// <param name="brokerTransportQualityOfService"></param>
        /// <returns></returns>
        private static MqttQualityOfServiceLevel GetMqttQualityOfServiceLevel(BrokerTransportQualityOfService brokerTransportQualityOfService)
        {
            switch (brokerTransportQualityOfService)
            {
                case BrokerTransportQualityOfService.AtLeastOnce:
                    return MqttQualityOfServiceLevel.AtLeastOnce;
                case BrokerTransportQualityOfService.AtMostOnce:
                    return MqttQualityOfServiceLevel.AtMostOnce;
                case BrokerTransportQualityOfService.ExactlyOnce:
                    return MqttQualityOfServiceLevel.ExactlyOnce;
                default:
                    return MqttQualityOfServiceLevel.AtLeastOnce;
            }
        }

        /// <summary>
        /// Get appropriate IMqttClientOptions with which to connect to the MQTTBroker
        /// </summary>
        /// <returns></returns>
        private MqttClientOptions GetMqttClientOptions()
        {
            MqttClientOptions mqttOptions = null;
            var mqttKeepAlive = TimeSpan.FromSeconds(GetWriterGroupsMaxKeepAlive() + MaxKeepAliveIncrement);

            if (!(ExtensionObject.ToEncodeable(PubSubConnectionConfiguration.Address) is NetworkAddressUrlDataType networkAddressUrlState))
            {
                Utils.Trace(Utils.TraceMasks.Error,
                    "The configuration for mqttConnection {0} has invalid Address configuration.",
                    PubSubConnectionConfiguration.Name);
                return null;
            }

            Uri connectionUri = null;

            if (networkAddressUrlState.Url != null &&
                Uri.TryCreate(networkAddressUrlState.Url, UriKind.Absolute, out connectionUri))
            {
                if ((connectionUri.Scheme != Utils.UriSchemeMqtt) && (connectionUri.Scheme != Utils.UriSchemeMqtts))
                {
                    Utils.Trace(Utils.TraceMasks.Error,
                        "The configuration for mqttConnection '{0}' has an invalid Url value {1}. The Uri scheme should be either {2}:// or {3}://",
                        PubSubConnectionConfiguration.Name,
                        networkAddressUrlState.Url,
                        Utils.UriSchemeMqtt,
                        Utils.UriSchemeMqtts);
                    return null;
                }
            }

            if (connectionUri == null)
            {
                Utils.Trace(Utils.TraceMasks.Error,
                    "The configuration for mqttConnection '{0}' has an invalid Url value {1}.",
                    PubSubConnectionConfiguration.Name,
                    networkAddressUrlState.Url);
                return null;
            }

            ITransportProtocolConfiguration transportProtocolConfiguration =
                new MqttClientProtocolConfiguration(PubSubConnectionConfiguration.ConnectionProperties);


            if (transportProtocolConfiguration is MqttClientProtocolConfiguration mqttProtocolConfiguration)
            {
                var mqttProtocolVersion =
                    (MqttProtocolVersion)((MqttClientProtocolConfiguration)transportProtocolConfiguration)
                    .ProtocolVersion;
                // create uniques client id
                var clientId = $"ClientId_{new Random().Next():D10}";
                // MQTTS mqttConnection.
                if (connectionUri.Scheme == Utils.UriSchemeMqtts)
                {
                    MqttTlsOptions mqttTlsOptions =
                        ((MqttClientProtocolConfiguration)transportProtocolConfiguration).MqttTlsOptions;

                    var x509Certificate2s = new List<X509Certificate2>();
                    if (mqttTlsOptions?.Certificates != null)
                    {
                        foreach (X509Certificate x509cert in mqttTlsOptions?.Certificates.X509Certificates)
                        {
                            x509Certificate2s.Add(new X509Certificate2(x509cert.Handle));
                        }
                    }

                    MqttClientOptionsBuilder mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer(brokerHostName_, brokerPort_)
                        .WithKeepAlivePeriod(mqttKeepAlive)
                        .WithProtocolVersion(mqttProtocolVersion)
                        .WithClientId(clientId)
                        .WithTlsOptions(o => {
                            _ = o.UseTls(true);
                            _ = o.WithClientCertificates(x509Certificate2s);
                            _ = o.WithSslProtocols(mqttTlsOptions?.SslProtocolVersion ??
                                System.Security.Authentication.SslProtocols.None);// Allow OS to choose best option
                            _ = o.WithAllowUntrustedCertificates(mqttTlsOptions?.AllowUntrustedCertificates ?? false);
                            _ = o.WithIgnoreCertificateChainErrors(mqttTlsOptions?.IgnoreCertificateChainErrors ?? false);
                            _ = o.WithIgnoreCertificateRevocationErrors(mqttTlsOptions?.IgnoreRevocationListErrors ?? false);
                            _ = o.WithCertificateValidationHandler(ValidateBrokerCertificate);
                        });

                    // Set user credentials.
                    if (mqttProtocolConfiguration.UseCredentials)
                    {
                        _ = mqttClientOptionsBuilder.WithCredentials(
                            new System.Net.NetworkCredential(string.Empty, mqttProtocolConfiguration.UserName)
                                .Password,
                            new System.Net.NetworkCredential(string.Empty, mqttProtocolConfiguration.Password)
                                .Password);

                        // Set ClientId for Azure.
                        if (mqttProtocolConfiguration.UseAzureClientId)
                        {
                            _ = mqttClientOptionsBuilder.WithClientId(mqttProtocolConfiguration.AzureClientId);
                        }
                    }

                    mqttOptions = mqttClientOptionsBuilder.Build();

                    // Create the certificate validator for broker certificates.
                    certificateValidator_ = CreateCertificateValidator(mqttTlsOptions);
                    certificateValidator_.CertificateValidation += CertificateValidator_CertificateValidation;
                    mqttClientTlsOptions_ = mqttOptions?.ChannelOptions?.TlsOptions;
                }
                // MQTT mqttConnection
                else if (connectionUri.Scheme == Utils.UriSchemeMqtt)
                {
                    MqttClientOptionsBuilder mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer(brokerHostName_, brokerPort_)
                        .WithKeepAlivePeriod(mqttKeepAlive)
                        .WithClientId(clientId)
                        .WithProtocolVersion(mqttProtocolVersion);

                    // Set user credentials.
                    if (mqttProtocolConfiguration.UseCredentials)
                    {
                        _ = mqttClientOptionsBuilder.WithCredentials(
                            new System.Net.NetworkCredential(string.Empty, mqttProtocolConfiguration.UserName)
                                .Password,
                            new System.Net.NetworkCredential(string.Empty, mqttProtocolConfiguration.Password)
                                .Password);
                    }

                    mqttOptions = mqttClientOptionsBuilder.Build();
                }
            }

            return mqttOptions;
        }

        /// <summary>
        /// Set up a new instance of a certificate validator based on passed in tls options
        /// </summary>
        /// <param name="mqttTlsOptions"><see cref="MqttTlsOptions"/></param>
        /// <returns>A new instance of stack validator <see cref="CertificateValidator"/></returns>
        private CertificateValidator CreateCertificateValidator(MqttTlsOptions mqttTlsOptions)
        {
            var certificateValidator = new CertificateValidator();

            var securityConfiguration = new SecurityConfiguration();
            securityConfiguration.TrustedIssuerCertificates = (CertificateTrustList)mqttTlsOptions.TrustedIssuerCertificates;
            securityConfiguration.TrustedPeerCertificates = (CertificateTrustList)mqttTlsOptions.TrustedPeerCertificates;
            securityConfiguration.RejectedCertificateStore = mqttTlsOptions.RejectedCertificateStore;

            securityConfiguration.RejectSHA1SignedCertificates = true;
            securityConfiguration.AutoAcceptUntrustedCertificates = mqttTlsOptions.AllowUntrustedCertificates;
            securityConfiguration.RejectUnknownRevocationStatus = !mqttTlsOptions.IgnoreRevocationListErrors;

            certificateValidator.Update(securityConfiguration).Wait();

            return certificateValidator;
        }

        /// <summary>
        /// Validates the broker certificate.
        /// </summary>
        /// <param name="context">The context of the validation</param>
        private bool ValidateBrokerCertificate(MqttClientCertificateValidationEventArgs context)
        {
            var brokerCertificate = new X509Certificate2(context.Certificate.GetRawCertData());

            try
            {
                // check if the broker certificate validation has been overridden.
                if (Application?.OnValidateBrokerCertificate != null)
                {
                    return Application.OnValidateBrokerCertificate(brokerCertificate);
                }
                else
                {
                    certificateValidator_?.Validate(brokerCertificate);
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Connection '{0}' - Broker certificate '{1}' rejected.",
                    PubSubConnectionConfiguration.Name, brokerCertificate.Subject);
                return false;
            }

            Utils.Trace(Utils.TraceMasks.Security, "Connection '{0}' - Broker certificate '{1}'  accepted.",
                PubSubConnectionConfiguration.Name, brokerCertificate.Subject);
            return true;
        }

        /// <summary>
        /// Handler for validation errors of MQTT broker certificate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            try
            {
                if (((e.Error.StatusCode == StatusCodes.BadCertificateRevocationUnknown) ||
                     (e.Error.StatusCode == StatusCodes.BadCertificateIssuerRevocationUnknown) ||
                     (e.Error.StatusCode == StatusCodes.BadCertificateRevoked) ||
                     (e.Error.StatusCode == StatusCodes.BadCertificateIssuerRevoked)) &&
                    (mqttClientTlsOptions_?.IgnoreCertificateRevocationErrors ?? false))
                {
                    // Accept broker certificate with revocation errors.
                    e.Accept = true;
                }
                else if ((e.Error.StatusCode == StatusCodes.BadCertificateChainIncomplete) &&
                         (mqttClientTlsOptions_?.IgnoreCertificateChainErrors ?? false))
                {
                    // Accept broker certificate with chain errors.
                    e.Accept = true;
                }
                else if ((e.Error.StatusCode == StatusCodes.BadCertificateUntrusted) &&
                         (mqttClientTlsOptions_?.AllowUntrustedCertificates ?? false))
                {
                    // Accept untrusted broker certificate.
                    e.Accept = true;
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "MqttPubSubConnection.CertificateValidation error.");
            }
        }

        #endregion Private methods

        #region MessageCreator innner classes
        /// <summary>
        /// Base abstract class for MessageCreator
        /// </summary>
        private abstract class MessageCreator
        {
            protected readonly MqttPubSubConnection m_mqttConnection;

            /// <summary>
            /// Create new instance of <see cref="MessageCreator"/>
            /// </summary>
            protected MessageCreator(MqttPubSubConnection mqttConnection)
            {
                m_mqttConnection = mqttConnection;
            }

            /// <summary>
            /// Create and return a new instance of the right <see cref="UaNetworkMessage"/> implementation.
            /// </summary>
            /// <returns></returns>
            public abstract UaNetworkMessage CreateNewNetworkMessage();

            /// <summary>
            /// Create the list of network messages to be published by the publisher
            /// </summary>
            public abstract IList<UaNetworkMessage> CreateNetworkMessages(WriterGroupDataType writerGroupConfiguration,
                WriterGroupPublishState state);

            /// <summary> 
            /// Create and return the Json DataSetMetaData message for a DataSetWriter
            /// </summary>
            public abstract UaNetworkMessage CreateDataSetMetaDataNetworkMessage(WriterGroupDataType writerGroup, UInt16 dataSetWriterId,
                DataSetMetaDataType dataSetMetaData);
        }

        /// <summary>
        /// The Json implementation for the Message creator
        /// </summary>
        private class JsonMessageCreator : MessageCreator
        {
            /// <summary>
            /// Create new instance of <see cref="JsonMessageCreator"/>
            /// </summary>
            public JsonMessageCreator(MqttPubSubConnection mqttConnection) : base(mqttConnection)
            {
            }

            /// <summary>
            /// Create and return a new instance of the right <see cref="JsonNetworkMessage"/>.
            /// </summary>
            public override UaNetworkMessage CreateNewNetworkMessage()
            {
                return new JsonNetworkMessage();
            }

            /// <summary>
            /// The Json implementation of CreateNetworkMessages for MQTT mqttConnection
            /// </summary>
            public override IList<UaNetworkMessage> CreateNetworkMessages(WriterGroupDataType writerGroupConfiguration,
                WriterGroupPublishState state)
            {
                if (!(ExtensionObject.ToEncodeable(
                        writerGroupConfiguration.MessageSettings) is JsonWriterGroupMessageDataType jsonMessageSettings))
                {
                    //Wrong configuration of writer group MessageSettings
                    return null;
                }

                //Create list of dataSet messages to be sent
                var jsonDataSetMessages = new List<JsonDataSetMessage>();
                var networkMessages = new List<UaNetworkMessage>();

                foreach (DataSetWriterDataType dataSetWriter in writerGroupConfiguration.DataSetWriters)
                {
                    //check if dataSetWriter enabled
                    if (dataSetWriter.Enabled)
                    {
                        DataSet dataSet = m_mqttConnection.CreateDataSet(dataSetWriter, state);

                        if (dataSet != null)
                        {
                            // check if the MetaData version is changed and issue a MetaData message
                            var hasMetaDataChanged = state.HasMetaDataChanged(dataSetWriter, dataSet.DataSetMetaData);

                            if (hasMetaDataChanged)
                            {
                                networkMessages.Add(CreateDataSetMetaDataNetworkMessage(writerGroupConfiguration, dataSetWriter.DataSetWriterId, dataSet.DataSetMetaData));
                            }

                            if (ExtensionObject.ToEncodeable(dataSetWriter.MessageSettings) is JsonDataSetWriterMessageDataType jsonDataSetMessageSettings)
                            {
                                var jsonDataSetMessage = new JsonDataSetMessage(dataSet) {
                                    DataSetMessageContentMask = (JsonDataSetMessageContentMask)jsonDataSetMessageSettings.DataSetMessageContentMask
                                };

                                // set common properties of dataset message
                                jsonDataSetMessage.SetFieldContentMask((DataSetFieldContentMask)dataSetWriter.DataSetFieldContentMask);
                                jsonDataSetMessage.DataSetWriterId = dataSetWriter.DataSetWriterId;
                                jsonDataSetMessage.SequenceNumber = dataSet.SequenceNumber;

                                jsonDataSetMessage.MetaDataVersion = dataSet.DataSetMetaData.ConfigurationVersion;
                                jsonDataSetMessage.Timestamp = DateTime.UtcNow;
                                jsonDataSetMessage.Status = StatusCodes.Good;

                                jsonDataSetMessages.Add(jsonDataSetMessage);

                                state.OnMessagePublished(dataSetWriter, dataSet);
                            }
                        }
                    }
                }

                //send existing network messages if no dataset message was created
                if (jsonDataSetMessages.Count == 0)
                {
                    return networkMessages;
                }

                // each entry of this list will generate a network message
                var dataSetMessagesList = new List<List<JsonDataSetMessage>>();
                if ((((JsonNetworkMessageContentMask)jsonMessageSettings.NetworkMessageContentMask) & JsonNetworkMessageContentMask.SingleDataSetMessage) != 0)
                {
                    // create a new network message for each dataset
                    foreach (JsonDataSetMessage dataSetMessage in jsonDataSetMessages)
                    {
                        dataSetMessagesList.Add(new List<JsonDataSetMessage>() { dataSetMessage });
                    }
                }
                else
                {
                    dataSetMessagesList.Add(jsonDataSetMessages);
                }

                foreach (List<JsonDataSetMessage> dataSetMessagesToUse in dataSetMessagesList)
                {
                    var jsonNetworkMessage = new JsonNetworkMessage(writerGroupConfiguration, dataSetMessagesToUse);
                    jsonNetworkMessage.SetNetworkMessageContentMask((JsonNetworkMessageContentMask)jsonMessageSettings?.NetworkMessageContentMask);

                    // Network message header
                    jsonNetworkMessage.PublisherId = m_mqttConnection.PubSubConnectionConfiguration.PublisherId.Value.ToString();
                    jsonNetworkMessage.WriterGroupId = writerGroupConfiguration.WriterGroupId;

                    if ((jsonNetworkMessage.NetworkMessageContentMask & JsonNetworkMessageContentMask.SingleDataSetMessage) != 0)
                    {
                        jsonNetworkMessage.DataSetClassId = dataSetMessagesToUse[0].DataSet?.DataSetMetaData?.DataSetClassId.ToString();
                    }

                    networkMessages.Add(jsonNetworkMessage);
                }

                return networkMessages;
            }

            /// <summary> 
            /// Create and return the Json DataSetMetaData message for a DataSetWriter
            /// </summary>
            public override UaNetworkMessage CreateDataSetMetaDataNetworkMessage(WriterGroupDataType writerGroup, UInt16 dataSetWriterId, DataSetMetaDataType dataSetMetaData)
            {
                // return UADP metadata network message
                return new JsonNetworkMessage(writerGroup, dataSetMetaData) {
                    PublisherId = m_mqttConnection.PubSubConnectionConfiguration.PublisherId.Value.ToString(),
                    DataSetWriterId = dataSetWriterId
                };
            }
        }

        /// <summary>
        /// The Uadp implementation for the Message creator
        /// </summary>
        private class UadpMessageCreator : MessageCreator
        {
            /// <summary>
            /// Create new instance of <see cref="UadpMessageCreator"/>
            /// </summary>
            public UadpMessageCreator(MqttPubSubConnection mqttConnection) : base(mqttConnection)
            {

            }

            /// <summary>
            /// Create and return a new instance of the right <see cref="UadpNetworkMessage"/>.
            /// </summary>
            public override UaNetworkMessage CreateNewNetworkMessage()
            {
                return new UadpNetworkMessage();
            }

            /// <summary>
            /// The Uadp implementation of CreateNetworkMessages for MQTT mqttConnection
            /// </summary>
            public override IList<UaNetworkMessage> CreateNetworkMessages(WriterGroupDataType writerGroupConfiguration,
                WriterGroupPublishState state)
            {
                if (!(ExtensionObject.ToEncodeable(
                        writerGroupConfiguration.MessageSettings) is UadpWriterGroupMessageDataType uadpMessageSettings))
                {
                    //Wrong configuration of writer group MessageSettings
                    return null;
                }

                //Create list of dataSet messages to be sent
                var uadpDataSetMessages = new List<UadpDataSetMessage>();
                var networkMessages = new List<UaNetworkMessage>();

                foreach (DataSetWriterDataType dataSetWriter in writerGroupConfiguration.DataSetWriters)
                {
                    //check if dataSetWriter enabled
                    if (dataSetWriter.Enabled)
                    {
                        DataSet dataSet = m_mqttConnection.CreateDataSet(dataSetWriter, state);

                        if (dataSet != null)
                        {
                            // check if the MetaData version is changed and issue a MetaData message
                            var hasMetaDataChanged = state.HasMetaDataChanged(dataSetWriter, dataSet.DataSetMetaData);

                            if (hasMetaDataChanged)
                            {
                                networkMessages.Add(CreateDataSetMetaDataNetworkMessage(writerGroupConfiguration, dataSetWriter.DataSetWriterId, dataSet.DataSetMetaData));
                            }

                            // try to create Uadp message
                            // check MessageSettings to see how to encode DataSet
                            if (ExtensionObject.ToEncodeable(dataSetWriter.MessageSettings) is UadpDataSetWriterMessageDataType uadpDataSetMessageSettings)
                            {
                                var uadpDataSetMessage = new UadpDataSetMessage(dataSet);
                                uadpDataSetMessage.SetMessageContentMask((UadpDataSetMessageContentMask)uadpDataSetMessageSettings.DataSetMessageContentMask);
                                uadpDataSetMessage.ConfiguredSize = uadpDataSetMessageSettings.ConfiguredSize;
                                uadpDataSetMessage.DataSetOffset = uadpDataSetMessageSettings.DataSetOffset;

                                // set common properties of dataset message
                                uadpDataSetMessage.SetFieldContentMask((DataSetFieldContentMask)dataSetWriter.DataSetFieldContentMask);
                                uadpDataSetMessage.DataSetWriterId = dataSetWriter.DataSetWriterId;
                                uadpDataSetMessage.SequenceNumber = dataSet.SequenceNumber;

                                uadpDataSetMessage.MetaDataVersion = dataSet.DataSetMetaData.ConfigurationVersion;

                                uadpDataSetMessage.Timestamp = DateTime.UtcNow;
                                uadpDataSetMessage.Status = StatusCodes.Good;

                                uadpDataSetMessages.Add(uadpDataSetMessage);

                                state.OnMessagePublished(dataSetWriter, dataSet);
                            }
                        }
                    }
                }

                //send existing network messages if no dataset message was created
                if (uadpDataSetMessages.Count == 0)
                {
                    return networkMessages;
                }

                var uadpNetworkMessage =
                    new UadpNetworkMessage(writerGroupConfiguration, uadpDataSetMessages);
                uadpNetworkMessage.SetNetworkMessageContentMask(
                    (UadpNetworkMessageContentMask)uadpMessageSettings?.NetworkMessageContentMask);

                // Network message header
                uadpNetworkMessage.PublisherId = m_mqttConnection.PubSubConnectionConfiguration.PublisherId.Value;
                uadpNetworkMessage.WriterGroupId = writerGroupConfiguration.WriterGroupId;

                // Writer group header
                uadpNetworkMessage.GroupVersion = uadpMessageSettings.GroupVersion;
                uadpNetworkMessage.NetworkMessageNumber = 1; //only one network message per publish

                networkMessages.Add(uadpNetworkMessage);

                return networkMessages;
            }


            /// <summary> 
            /// Create and return the Uadp DataSetMetaData message for a DataSetWriter
            /// </summary>
            public override UaNetworkMessage CreateDataSetMetaDataNetworkMessage(WriterGroupDataType writerGroup, UInt16 dataSetWriterId, DataSetMetaDataType dataSetMetaData)
            {
                // return UADP metadata network message
                return new UadpNetworkMessage(writerGroup, dataSetMetaData) {
                    PublisherId = m_mqttConnection.PubSubConnectionConfiguration.PublisherId.Value,
                    DataSetWriterId = dataSetWriterId
                };
            }
        }
        #endregion
    }
}
