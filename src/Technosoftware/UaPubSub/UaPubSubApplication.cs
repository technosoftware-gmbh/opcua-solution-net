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
using System.IO;
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;

using Technosoftware.UaPubSub.Configuration;
using Technosoftware.UaPubSub.PublishedData;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// A class that runs an OPC UA PubSub application.
    /// </summary>
    public class UaPubSubApplication : IDisposable
    {
        #region Events
        /// <summary>
        /// Event that is triggered when the <see cref="UaPubSubApplication"/> receives a message via its active connections
        /// </summary>
        public event EventHandler<RawDataReceivedEventArgs> RawDataReceivedEvent;

        /// <summary>
        /// Event that is triggered when the <see cref="UaPubSubApplication"/> receives and decodes subscribed DataSets
        /// </summary>
        public event EventHandler<SubscribedDataEventArgs> DataReceivedEvent;

        /// <summary>
        /// Event that is triggered when the <see cref="UaPubSubApplication"/> receives and decodes subscribed DataSet MetaData
        /// </summary>
        public event EventHandler<SubscribedDataEventArgs> MetaDataReceivedEvent;

        /// <summary>
        /// Event that is triggered when the <see cref="UaPubSubApplication"/> receives and decodes subscribed DataSet PublisherEndpoints
        /// </summary>
        public event EventHandler<PublisherEndpointsEventArgs> PublisherEndpointsReceivedEvent;

        /// <summary>
        /// Event that is triggered before the configuration is updated with a new MetaData 
        /// The configuration will not be updated if <see cref="ConfigurationUpdatingEventArgs.Cancel"/> flag is set on true.
        /// </summary>
        public event EventHandler<ConfigurationUpdatingEventArgs> ConfigurationUpdatingEvent;

        /// <summary>
        /// Event that is triggered when the <see cref="UaPubSubApplication"/> receives and decodes subscribed DataSet MetaData
        /// </summary>
        public event EventHandler<DataSetWriterConfigurationEventArgs> DataSetWriterConfigurationReceivedEvent;
        #endregion

        #region Event Callbacks
        /// <summary>
        /// Raised when the MQTT broker certificate is validated.
        /// </summary>
        /// <returns> 
        /// Returns whether the broker certificate is valid and trusted.
        /// </returns>
        public ValidateBrokerCertificateHandler OnValidateBrokerCertificate;
        #endregion

        #region Constructors
        /// <summary>
        ///  Initializes a new instance of the <see cref="UaPubSubApplication"/> class.
        /// </summary>
        /// <param name="dataStore"> The current implementation of <see cref="IUaPubSubDataStore"/> used by this instance of pub sub application</param>
        /// <param name="applicationId"> The application id for instance.</param>
        private UaPubSubApplication(IUaPubSubDataStore dataStore = null, string applicationId = null)
        {
            uaPubSubConnections_ = new List<IUaPubSubConnection>();

            if (dataStore != null)
            {
                dataStore_ = dataStore;
            }
            else
            {
                dataStore_ = new UaPubSubDataStore();
            }

            if (!String.IsNullOrEmpty(applicationId))
            {
                ApplicationId = applicationId;
            }
            else
            {
                ApplicationId = $"opcua:{System.Net.Dns.GetHostName()}:{new Random().Next().ToString("D10")}";
            }

            dataCollector_ = new DataCollector(dataStore_);
            uaPubSubConfigurator_ = new UaPubSubConfigurator();
            uaPubSubConfigurator_.ConnectionAddedEvent += UaPubSubConfigurator_ConnectionAdded;
            uaPubSubConfigurator_.ConnectionRemovedEvent += UaPubSubConfigurator_ConnectionRemoved;
            uaPubSubConfigurator_.PublishedDataSetAddedEvent += UaPubSubConfigurator_PublishedDataSetAdded;
            uaPubSubConfigurator_.PublishedDataSetRemovedEvent += UaPubSubConfigurator_PublishedDataSetRemoved;

            Utils.Trace("An instance of UaPubSubApplication was created.");
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="UaPublisher"/> class.
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
                uaPubSubConfigurator_.ConnectionAddedEvent -= UaPubSubConfigurator_ConnectionAdded;
                uaPubSubConfigurator_.ConnectionRemovedEvent -= UaPubSubConfigurator_ConnectionRemoved;
                uaPubSubConfigurator_.PublishedDataSetAddedEvent -= UaPubSubConfigurator_PublishedDataSetAdded;
                uaPubSubConfigurator_.PublishedDataSetRemovedEvent -= UaPubSubConfigurator_PublishedDataSetRemoved;

                Stop();
                // free managed resources
                foreach (var connection in uaPubSubConnections_)
                {
                    connection.Dispose();
                }
                uaPubSubConnections_.Clear();
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The application id associated with the UA
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Get the list of SupportedTransportProfiles
        /// </summary>
        public static string[] SupportedTransportProfiles
        {
            get
            {
                return new string[] { Profiles.PubSubUdpUadpTransport, Profiles.PubSubMqttJsonTransport, Profiles.PubSubMqttUadpTransport };
            }
        }

        /// <summary>
        /// Get reference to the associated <see cref="UaPubSubConfigurator"/> instance.
        /// </summary>
        public UaPubSubConfigurator UaPubSubConfigurator { get { return uaPubSubConfigurator_; } }

        /// <summary>
        /// Get reference to current DataStore. Write here all node values needed to be published by this PubSubApplication
        /// </summary>
        public IUaPubSubDataStore DataStore { get { return dataStore_; } }

        /// <summary>
        /// Get the read only list of <see cref="UaPubSubConnection"/> created for this Application instance 
        /// </summary>
        public ReadOnlyList<IUaPubSubConnection> PubSubConnections
        {
            get
            {
                return new ReadOnlyList<IUaPubSubConnection>(uaPubSubConnections_);
            }
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Get reference to current configured DataCollector for this UaPubSubApplication
        /// </summary>
        internal DataCollector DataCollector { get { return dataCollector_; } }
        #endregion

        #region Public Static Create Methods
        /// <summary>
        /// Creates a new <see cref="UaPubSubApplication"/> and associates it with a custom implementation of <see cref="IUaPubSubDataStore"/>.
        /// </summary>
        /// <param name="dataStore"> The current implementation of <see cref="IUaPubSubDataStore"/> used by this instance of pub sub application</param>
        /// <returns>New instance of <see cref="UaPubSubApplication"/></returns>
        public static UaPubSubApplication Create(IUaPubSubDataStore dataStore)
        {
            return Create(new PubSubConfigurationDataType(), dataStore);
        }

        /// <summary>
        /// Creates a new <see cref="UaPubSubApplication"/> by loading the configuration parameters from the specified path.
        /// </summary>
        /// <param name="configFilePath">The path of the configuration path.</param>
        /// <param name="dataStore"> The current implementation of <see cref="IUaPubSubDataStore"/> used by this instance of pub sub application</param>
        /// <returns>New instance of <see cref="UaPubSubApplication"/></returns>
        public static UaPubSubApplication Create(string configFilePath, IUaPubSubDataStore dataStore = null)
        {
            // validate input argument 
            if (configFilePath == null)
            {
                throw new ArgumentNullException(nameof(configFilePath));
            }
            if (!File.Exists(configFilePath))
            {
                throw new ArgumentException("The specified file {0} does not exist", configFilePath);
            }
            PubSubConfigurationDataType pubSubConfiguration = UaPubSubConfigurationHelper.LoadConfiguration(configFilePath);

            return Create(pubSubConfiguration, dataStore);
        }

        /// <summary>
        /// Creates a new <see cref="UaPubSubApplication"/> by loading the configuration parameters from the 
        /// specified <see cref="PubSubConfigurationDataType"/> parameter.
        /// </summary>
        /// <param name="pubSubConfiguration">The configuration object.</param>
        /// <param name="dataStore"> The current implementation of <see cref="IUaPubSubDataStore"/> used by this instance of pub sub application</param>
        /// <returns>New instance of <see cref="UaPubSubApplication"/></returns>
        public static UaPubSubApplication Create(PubSubConfigurationDataType pubSubConfiguration = null,
            IUaPubSubDataStore dataStore = null)
        {
            // if no argument received, start with empty configuration
            if (pubSubConfiguration == null)
            {
                pubSubConfiguration = new PubSubConfigurationDataType();
            }

            UaPubSubApplication uaPubSubApplication = new UaPubSubApplication(dataStore);
            uaPubSubApplication.uaPubSubConfigurator_.LoadConfiguration(pubSubConfiguration);
            return uaPubSubApplication;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start Publish/Subscribe jobs associated with this instance
        /// </summary>
        public void Start()
        {
            Utils.Trace("UaPubSubApplication is starting.");
            foreach (var connection in uaPubSubConnections_)
            {
                connection.Start();
            }
            Utils.Trace("UaPubSubApplication was started.");
        }

        /// <summary>
        /// Stop Publish/Subscribe jobs associated with this instance
        /// </summary>
        public void Stop()
        {
            Utils.Trace("UaPubSubApplication is stopping.");
            foreach (var connection in uaPubSubConnections_)
            {
                connection.Stop();
            }
            Utils.Trace("UaPubSubApplication is stopped.");
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Raise <see cref="RawDataReceivedEvent"/> event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseRawDataReceivedEvent(RawDataReceivedEventArgs e)
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            try
            {
                RawDataReceivedEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "UaPubSubApplication.RaiseRawDataReceivedEvent");
            }
        }

        /// <summary>
        /// Raise DataReceived event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseDataReceivedEvent(SubscribedDataEventArgs e)
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            try
            {
                DataReceivedEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "UaPubSubApplication.RaiseDataReceivedEvent");
            }
        }

        /// <summary>
        /// Raise MetaDataReceived event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseMetaDataReceivedEvent(SubscribedDataEventArgs e)
        {
            try
            {
                MetaDataReceivedEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "UaPubSubApplication.RaiseMetaDataReceivedEvent");
            }
        }
        /// <summary>
        /// Raise DatasetWriterConfigurationReceived event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseDatasetWriterConfigurationReceivedEvent(DataSetWriterConfigurationEventArgs e)
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            try
            {
                DataSetWriterConfigurationReceivedEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "UaPubSubApplication.DatasetWriterConfigurationReceivedEvent");
            }
        }

        /// <summary>
        /// Raise PublisherEndpointsReceived event
        /// </summary>
        /// <param name="e"></param>
        internal void RaisePublisherEndpointsReceivedEvent(PublisherEndpointsEventArgs e)
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            try
            {
                PublisherEndpointsReceivedEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "UaPubSubApplication.RaisePublisherEndpointsReceivedEvent");
            }
        }

        /// <summary>
        /// Raise <see cref="ConfigurationUpdatingEvent"/> event
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseConfigurationUpdatingEvent(ConfigurationUpdatingEventArgs e)
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            try
            {
                ConfigurationUpdatingEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "UaPubSubApplication.RaiseConfigurationUpdatingEvent");
            }
        }
        #endregion

        #region Private Methods - UaPubSubConfigurator event handlers
        /// <summary>
        /// Handler for PublishedDataSetAdded event
        /// </summary>
        private void UaPubSubConfigurator_PublishedDataSetAdded(object sender, PublishedDataSetEventArgs e)
        {
            DataCollector.AddPublishedDataSet(e.PublishedDataSetDataType);
        }

        /// <summary>
        /// Handler for PublishedDataSetRemoved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UaPubSubConfigurator_PublishedDataSetRemoved(object sender, PublishedDataSetEventArgs e)
        {
            DataCollector.RemovePublishedDataSet(e.PublishedDataSetDataType);
        }

        /// <summary>
        /// Handler for ConnectionRemoved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UaPubSubConfigurator_ConnectionRemoved(object sender, ConnectionEventArgs e)
        {
            IUaPubSubConnection removedUaPubSubConnection = null;
            foreach (var connection in uaPubSubConnections_)
            {
                if (connection.PubSubConnectionConfiguration.Equals(e.PubSubConnectionDataType))
                {
                    removedUaPubSubConnection = connection;
                    break;
                }
            }
            if (removedUaPubSubConnection != null)
            {
                uaPubSubConnections_.Remove(removedUaPubSubConnection);
                removedUaPubSubConnection.Dispose();
            }
        }

        /// <summary>
        /// Handler for ConnectionAdded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UaPubSubConfigurator_ConnectionAdded(object sender, ConnectionEventArgs e)
        {
            UaPubSubConnection newUaPubSubConnection = ObjectFactory.CreateConnection(this, e.PubSubConnectionDataType);
            if (newUaPubSubConnection != null)
            {
                uaPubSubConnections_.Add(newUaPubSubConnection);
            }
        }
        #endregion

        #region Private Fields
        private List<IUaPubSubConnection> uaPubSubConnections_;
        private DataCollector dataCollector_;
        private IUaPubSubDataStore dataStore_;
        private UaPubSubConfigurator uaPubSubConfigurator_;
        #endregion
    }

    /// <summary>
    /// A delegate which validates the MQTT broker certificate.
    /// </summary>
    /// <param name="brokerCertificate">The broker certificate.</param>
    /// <returns>Returns whether the broker certificate is valid and trusted.</returns>
    public delegate bool ValidateBrokerCertificateHandler(X509Certificate2 brokerCertificate);
}
