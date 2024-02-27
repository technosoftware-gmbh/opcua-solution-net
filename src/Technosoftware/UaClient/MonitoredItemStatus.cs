#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// The current status of monitored item.
    /// </summary>
    public class MonitoredItemStatus
    {
        #region Constructors
        /// <summary>
        /// Creates a empty object.
        /// </summary>
        internal MonitoredItemStatus()
        {
            Initialize();
        }

        private void Initialize()
        {
            Id = 0;
            NodeId = null;
            AttributeId = Attributes.Value;
            IndexRange = null;
            DataEncoding = null;
            MonitoringMode = MonitoringMode.Disabled;
            ClientHandle = 0;
            SamplingInterval = 0;
            Filter = null;
            filterResult_ = null;
            QueueSize = 0;
            DiscardOldest = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The identifier assigned by the server.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Whether the item has been created on the server.
        /// </summary>
        public bool Created => Id != 0;

        /// <summary>
        /// Any error condition associated with the monitored item.
        /// </summary>
        public ServiceResult Error { get; private set; }

        /// <summary>
        /// The node id being monitored.
        /// </summary>
        public NodeId NodeId { get; private set; }

        /// <summary>
        /// The attribute being monitored.
        /// </summary>
        public uint AttributeId { get; private set; }

        /// <summary>
        /// The range of array indexes to being monitored.
        /// </summary>
        public string IndexRange { get; private set; }

        /// <summary>
        /// The encoding to use when returning notifications.
        /// </summary>
        public QualifiedName DataEncoding { get; private set; }

        /// <summary>
        /// The monitoring mode.
        /// </summary>
        public MonitoringMode MonitoringMode { get; private set; }

        /// <summary>
        /// The identifier assigned by the client.
        /// </summary>
        public uint ClientHandle { get; private set; }

        /// <summary>
        /// The sampling interval.
        /// </summary>
        public double SamplingInterval { get; private set; }

        /// <summary>
        /// The filter to use to select values to return.
        /// </summary>
        public MonitoringFilter Filter { get; private set; }

        /// <summary>
        /// The result of applying the filter
        /// </summary>
        public MonitoringFilterResult FilterResult => filterResult_;

        /// <summary>
        /// The length of the queue used to buffer values.
        /// </summary>
        public uint QueueSize { get; private set; }

        /// <summary>
        /// Whether to discard the oldest entries in the queue when it is full.
        /// </summary>
        public bool DiscardOldest { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the monitoring mode.
        /// </summary>
        public void SetMonitoringMode(MonitoringMode monitoringMode)
        {
            MonitoringMode = monitoringMode;
        }

        /// <summary>
        /// Updates the object with the results of a translate browse paths request.
        /// </summary>
        internal void SetResolvePathResult(
            BrowsePathResult result,
            ServiceResult error)
        {
            Error = error;
        }

        /// <summary>
        /// Updates the object with the results of a create monitored item request.
        /// </summary>
        internal void SetCreateResult(
            MonitoredItemCreateRequest request,
            MonitoredItemCreateResult result,
            ServiceResult error)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (result == null) throw new ArgumentNullException(nameof(result));

            NodeId = request.ItemToMonitor.NodeId;
            AttributeId = request.ItemToMonitor.AttributeId;
            IndexRange = request.ItemToMonitor.IndexRange;
            DataEncoding = request.ItemToMonitor.DataEncoding;
            MonitoringMode = request.MonitoringMode;
            ClientHandle = request.RequestedParameters.ClientHandle;
            SamplingInterval = request.RequestedParameters.SamplingInterval;
            QueueSize = request.RequestedParameters.QueueSize;
            DiscardOldest = request.RequestedParameters.DiscardOldest;
            Filter = null;
            filterResult_ = null;
            Error = error;

            if (request.RequestedParameters.Filter != null)
            {
                Filter = Utils.Clone(request.RequestedParameters.Filter.Body) as MonitoringFilter;
            }

            if (ServiceResult.IsGood(error))
            {
                Id = result.MonitoredItemId;
                SamplingInterval = result.RevisedSamplingInterval;
                QueueSize = result.RevisedQueueSize;

                if (result.FilterResult != null)
                {
                    filterResult_ = Utils.Clone(result.FilterResult.Body) as MonitoringFilterResult;
                }
            }
        }

        /// <summary>
        /// Updates the object with the results of a transfer monitored item request.
        /// </summary>
        internal void SetTransferResult(MonitoredItem monitoredItem)
        {
            if (monitoredItem == null) throw new ArgumentNullException(nameof(monitoredItem));

            NodeId = monitoredItem.ResolvedNodeId;
            AttributeId = monitoredItem.AttributeId;
            IndexRange = monitoredItem.IndexRange;
            DataEncoding = monitoredItem.Encoding;
            MonitoringMode = monitoredItem.MonitoringMode;
            ClientHandle = monitoredItem.ClientHandle;
            SamplingInterval = monitoredItem.SamplingInterval;
            QueueSize = monitoredItem.QueueSize;
            DiscardOldest = monitoredItem.DiscardOldest;
            Filter = null;
            filterResult_ = null;

            if (monitoredItem.Filter != null)
            {
                Filter = Utils.Clone(monitoredItem.Filter) as MonitoringFilter;
            }
        }

        /// <summary>
        /// Updates the object with the results of a modify monitored item request.
        /// </summary>
        internal void SetModifyResult(
            MonitoredItemModifyRequest request,
            MonitoredItemModifyResult result,
            ServiceResult error)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (result == null) throw new ArgumentNullException(nameof(result));

            Error = error;

            if (ServiceResult.IsGood(error))
            {
                ClientHandle = request.RequestedParameters.ClientHandle;
                SamplingInterval = request.RequestedParameters.SamplingInterval;
                QueueSize = request.RequestedParameters.QueueSize;
                DiscardOldest = request.RequestedParameters.DiscardOldest;
                Filter = null;
                filterResult_ = null;

                if (request.RequestedParameters.Filter != null)
                {
                    Filter = Utils.Clone(request.RequestedParameters.Filter.Body) as MonitoringFilter;
                }

                SamplingInterval = result.RevisedSamplingInterval;
                QueueSize = result.RevisedQueueSize;

                if (result.FilterResult != null)
                {
                    filterResult_ = Utils.Clone(result.FilterResult.Body) as MonitoringFilterResult;
                }
            }
        }

        /// <summary>
        /// Updates the object with the results of a delete item request.
        /// </summary>
        internal void SetDeleteResult(ServiceResult error)
        {
            Id = 0;
            Error = error;
        }

        /// <summary>
        /// Sets the error state for the monitored item status.
        /// </summary>
        internal void SetError(ServiceResult error)
        {
            Error = error;
        }
        #endregion

        #region Private Fields
        private MonitoringFilterResult filterResult_;
        #endregion
    }
}
