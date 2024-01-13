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
    /// A class responsible with calculating and triggering publish messages.
    /// </summary>
    internal class UaPublisher : IUaPublisher
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UaPublisher"/> class.
        /// </summary>
        internal UaPublisher(IUaPubSubConnection pubSubConnection, WriterGroupDataType writerGroupConfiguration)
        {
            if (pubSubConnection == null)
            {
                throw new ArgumentNullException(nameof(pubSubConnection));
            }
            if (writerGroupConfiguration == null)
            {
                throw new ArgumentNullException(nameof(writerGroupConfiguration));
            }

            pubSubConnection_ = pubSubConnection;
            writerGroupConfiguration_ = writerGroupConfiguration;
            writerGroupPublishState_ = new WriterGroupPublishState();

            intervalRunner_ = new IntervalRunner(writerGroupConfiguration_.Name, writerGroupConfiguration_.PublishingInterval, CanPublish, PublishMessages);
            
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Get reference to the associated parent <see cref="IUaPubSubConnection"/> instance.
        /// </summary>
        public IUaPubSubConnection PubSubConnection
        {
            get { return pubSubConnection_; }
        }

        /// <summary>
        /// Get reference to the associated configuration object, the <see cref="WriterGroupDataType"/> instance.
        /// </summary>
        public WriterGroupDataType WriterGroupConfiguration
        {
            get { return writerGroupConfiguration_; }
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
                Stop();

                intervalRunner_.Dispose();
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the publisher and makes it ready to send data.
        /// </summary>
        public void Start()
        {
            intervalRunner_.Start();
            Utils.Trace("The UaPublisher for WriterGroup '{0}' was started.", writerGroupConfiguration_.Name);
        }

        /// <summary>
        /// Stop the publishing thread.
        /// </summary>
        public virtual void Stop()
        {
            intervalRunner_.Stop();

            Utils.Trace("The UaPublisher for WriterGroup '{0}' was stopped.", writerGroupConfiguration_.Name);
        }
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Decide if the connection can publish
        /// </summary>
        /// <returns></returns>
        private bool CanPublish()
        {
            lock (lock_)
            {
                return pubSubConnection_.CanPublish(writerGroupConfiguration_);
            }
        }

        /// <summary>
        /// Generate and publish a messages
        /// </summary>
        private void PublishMessages()
        {
            // check for valid license
            LicenseHandler.ValidateFeatures(LicenseHandler.ProductLicense.PubSub, Opc.Ua.LicenseHandler.ProductFeature.None);

            try
            {
                IList<UaNetworkMessage> networkMessages = pubSubConnection_.CreateNetworkMessages(writerGroupConfiguration_, writerGroupPublishState_);
                if (networkMessages != null)
                {
                    foreach (UaNetworkMessage uaNetworkMessage in networkMessages)
                    {
                        if (uaNetworkMessage != null)
                        {
                            bool success = pubSubConnection_.PublishNetworkMessage(uaNetworkMessage);
                            Utils.Trace(Utils.TraceMasks.Information,
                                "UaPublisher - PublishNetworkMessage, WriterGroupId:{0}; success = {1}", writerGroupConfiguration_.WriterGroupId, success.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Unexpected exception in PublishMessages
                Utils.Trace(e, "UaPublisher.PublishMessages");
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();

        private readonly IUaPubSubConnection pubSubConnection_;
        private readonly WriterGroupDataType writerGroupConfiguration_;
        private readonly WriterGroupPublishState writerGroupPublishState_;

        // the component that triggers the publish messages
        private readonly IntervalRunner intervalRunner_;
        #endregion
    }
}
