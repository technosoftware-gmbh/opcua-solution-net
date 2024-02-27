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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.Principal;
using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// An object that manages all events raised within the server.
    /// </summary>
    public class EventManager : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new instance of a sampling group.
        /// </summary>
        public EventManager(IUaServerData server, uint maxQueueSize)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));

            server_ = server;
            monitoredItems_ = new Dictionary<uint,IUaEventMonitoredItem>();
            maxEventQueueSize_ = maxQueueSize;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                List<IUaEventMonitoredItem> monitoredItems = null;

                lock (lock_)
                {
                    monitoredItems = new List<IUaEventMonitoredItem>(monitoredItems_.Values);
                    monitoredItems_.Clear();
                }

                foreach (var monitoredItem in monitoredItems)
                {
                    Utils.SilentDispose(monitoredItem);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reports an event.
        /// </summary>
        public static void ReportEvent(IFilterTarget e, IList<IUaEventMonitoredItem> monitoredItems)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            foreach (var monitoredItem in monitoredItems)
            {
                monitoredItem.QueueEvent(e);
            }
        }

        /// <summary>
        /// Creates a set of monitored items.
        /// </summary>
        public UaMonitoredItem CreateMonitoredItem(
            UaServerOperationContext           context,
            IUaBaseNodeManager               nodeManager,
            object                     handle,
            uint                       subscriptionId,
            uint                       monitoredItemId,
            TimestampsToReturn         timestampsToReturn,
            double                     publishingInterval,
            MonitoredItemCreateRequest itemToCreate,
            EventFilter                filter)
        {
            lock (lock_)
            {
                // calculate sampling interval.
                var samplingInterval = itemToCreate.RequestedParameters.SamplingInterval;

                if (samplingInterval < 0)
                {
                    samplingInterval = publishingInterval;
                }

                // limit the queue size.
                var queueSize = itemToCreate.RequestedParameters.QueueSize;

                if (queueSize > maxEventQueueSize_)
                {
                    queueSize = maxEventQueueSize_;
                }

                // create the monitored item.
                var monitoredItem = new UaMonitoredItem(
                    server_,
                    nodeManager,
                    handle,
                    subscriptionId,
                    monitoredItemId,
                    itemToCreate.ItemToMonitor,
                    context.DiagnosticsMask,
                    timestampsToReturn,
                    itemToCreate.MonitoringMode,
                    itemToCreate.RequestedParameters.ClientHandle,
                    filter,
                    filter,
                    null,
                    samplingInterval,
                    queueSize,
                    itemToCreate.RequestedParameters.DiscardOldest,
                    MinimumSamplingIntervals.Continuous);

                // save the monitored item.
                monitoredItems_.Add(monitoredItemId, monitoredItem);

                return monitoredItem;
            }
        }

        /// <summary>
        /// Modifies a monitored item.
        /// </summary>
        public void ModifyMonitoredItem(
            UaServerOperationContext           context,
            IUaEventMonitoredItem        monitoredItem,
            TimestampsToReturn         timestampsToReturn,
            MonitoredItemModifyRequest itemToModify,
            EventFilter                filter)
        {
            lock (lock_)
            {
                // should never be called with items that it does not own.
                if (!monitoredItems_.ContainsKey(monitoredItem.Id))
                {
                    return;
                }

                // limit the queue size.
                var queueSize = itemToModify.RequestedParameters.QueueSize;

                if (queueSize > maxEventQueueSize_)
                {
                    queueSize = maxEventQueueSize_;
                }

                // modify the attributes.
                monitoredItem.ModifyAttributes(
                    context.DiagnosticsMask,
                    timestampsToReturn,
                    itemToModify.RequestedParameters.ClientHandle,
                    filter,
                    filter,
                    null,
                    itemToModify.RequestedParameters.SamplingInterval,
                    queueSize,
                    itemToModify.RequestedParameters.DiscardOldest);
            }
        }

        /// <summary>
        /// Deletes a monitored item.
        /// </summary>
        public void DeleteMonitoredItem(uint monitoredItemId)
        {
            lock (lock_)
            {
                monitoredItems_.Remove(monitoredItemId);
            }
        }

        /// <summary>
        /// Returns the currently active monitored items. 
        /// </summary>
        public IList<IUaEventMonitoredItem> GetMonitoredItems()
        {
            lock (lock_)
            {
                return new List<IUaEventMonitoredItem>(monitoredItems_.Values);
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private Dictionary<uint, IUaEventMonitoredItem> monitoredItems_;
        private uint maxEventQueueSize_;
        #endregion
    }
}
