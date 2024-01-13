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
    /// Interface for an UaPubSubConnection
    /// </summary>
    public interface IUaPubSubConnection : IDisposable
    {
        /// <summary>
        /// Get assigned transport protocol for this connection instance
        /// </summary>
        TransportProtocol TransportProtocol { get; }

        /// <summary>
        /// Get the configuration object for this PubSub connection
        /// </summary>
        PubSubConnectionDataType PubSubConnectionConfiguration { get; }

        /// <summary>
        /// Get reference to <see cref="UaPubSubApplication"/>
        /// </summary>
        UaPubSubApplication Application { get; }

        /// <summary>
        /// Get flag that indicates if the Connection is in running state
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start Publish/Subscribe jobs associated with this instance
        /// </summary>
        void Start();

        /// <summary>
        /// Stop Publish/Subscribe jobs associated with this instance
        /// </summary>
        void Stop();

        /// <summary>
        /// Determine if the connection has anything to publish -> at least one WriterDataSet is configured as enabled for current writer group
        /// </summary>
        bool CanPublish(WriterGroupDataType writerGroupConfiguration);

        /// <summary>
        /// Create the network messages built from the provided writerGroupConfiguration
        /// </summary>
        IList<UaNetworkMessage> CreateNetworkMessages(WriterGroupDataType writerGroupConfiguration, WriterGroupPublishState state);

        /// <summary>
        /// Publish the network message
        /// </summary>
        bool PublishNetworkMessage(UaNetworkMessage networkMessage);

        /// <summary>
        /// Get flag that indicates if all the network clients are connected
        /// </summary>
        bool AreClientsConnected();

        /// <summary>
        /// Get current list of dataset readers available in this UaSubscriber component
        /// </summary>
        List<DataSetReaderDataType> GetOperationalDataSetReaders();
    }
}
