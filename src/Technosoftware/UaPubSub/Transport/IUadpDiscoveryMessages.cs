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

namespace Technosoftware.UaPubSub.Transport
{
    /// <summary>
    /// UADP Discovery messages interface
    /// </summary>
    public interface IUadpDiscoveryMessages
    {
        /// <summary>
        /// Set GetPublisherEndpoints callback used by the subscriber to receive PublisherEndpoints data from publisher
        /// </summary>
        /// <param name="eventHandler"></param>
        void GetPublisherEndpointsCallback(GetPublisherEndpointsEventHandler eventHandler);

        /// <summary>
        /// Set GetDataSetWriterIds callback used by the subscriber to receive DataSetWriter ids from publisher
        /// </summary>
        /// <param name="eventHandler"></param>
        void GetDataSetWriterConfigurationCallback(GetDataSetWriterIdsEventHandler eventHandler);

        /// <summary>
        /// Create and return the list of EndpointDescription to be used only by UADP Discovery response messages
        /// </summary>
        /// <param name="endpoints"></param>
        /// <param name="publisherProvideEndpointsStatusCode"></param>
        /// <param name="publisherId"></param>
        /// <returns></returns>
        UaNetworkMessage CreatePublisherEndpointsNetworkMessage(EndpointDescription[] endpoints,
            StatusCode publisherProvideEndpointsStatusCode, object publisherId);

        /// <summary>
        /// Create and return the list of DataSetMetaData response messages 
        /// </summary>
        /// <param name="dataSetWriterIds"></param>
        /// <returns></returns>
        IList<UaNetworkMessage> CreateDataSetMetaDataNetworkMessages(UInt16[] dataSetWriterIds);

        /// <summary>
        /// Create and return the list of DataSetWriterConfiguration response message
        /// </summary>
        /// <param name="dataSetWriterIds">DatasetWriter ids</param>
        /// <returns></returns>
        IList<UaNetworkMessage> CreateDataSetWriterCofigurationMessage(UInt16[] dataSetWriterIds);

        /// <summary>
        /// Request UADP Discovery DataSetWriterConfiguration messages
        /// </summary>
        void RequestDataSetWriterConfiguration();

        /// <summary>
        /// Request UADP Discovery DataSetMetaData messages
        /// </summary>
        void RequestDataSetMetaData();

        /// <summary>
        /// Request UADP Discovery Publisher endpoints only
        /// </summary>
        void RequestPublisherEndpoints();
    }

    /// <summary>
    /// Get PublisherEndpoints event handler
    /// </summary>
    /// <returns></returns>
    public delegate IList<EndpointDescription> GetPublisherEndpointsEventHandler();

    /// <summary>
    /// Get DataSetWriterConfiguration ids event handler
    /// </summary>
    /// <returns></returns>
    public delegate IList<UInt16> GetDataSetWriterIdsEventHandler(UaPubSubApplication uaPubSubApplication);
}
