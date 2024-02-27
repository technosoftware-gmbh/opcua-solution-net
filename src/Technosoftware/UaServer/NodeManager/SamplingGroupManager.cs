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

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// An object that manages the sampling groups for a node manager.
    /// </summary>
    public class SamplingGroupManager : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new instance of a sampling group.
        /// </summary>
        public SamplingGroupManager(
            IUaServerData server,
            IUaBaseNodeManager nodeManager,
            uint maxQueueSize,
            IEnumerable<SamplingRateGroup> samplingRates)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (nodeManager == null) throw new ArgumentNullException(nameof(nodeManager));

            server_ = server;
            nodeManager_ = nodeManager;
            samplingGroups_ = new List<SamplingGroup>();
            sampledItems_ = new Dictionary<IUaSampledDataChangeMonitoredItem, SamplingGroup>();
            maxQueueSize_ = maxQueueSize;

            if (samplingRates != null)
            {
                samplingRates_ = new List<SamplingRateGroup>(samplingRates);

                if (samplingRates_.Count == 0)
                {
                    samplingRates_ = new List<SamplingRateGroup>(defaultSamplingRates_);
                }
            }

            if (samplingRates_ == null)
            {
                samplingRates_ = new List<SamplingRateGroup>(defaultSamplingRates_);
            }
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
                List<SamplingGroup> samplingGroups = null;
                List<IUaSampledDataChangeMonitoredItem> monitoredItems = null;

                lock (lock_)
                {
                    samplingGroups = new List<SamplingGroup>(samplingGroups_);
                    samplingGroups_.Clear();

                    monitoredItems = new List<IUaSampledDataChangeMonitoredItem>(sampledItems_.Keys);
                    sampledItems_.Clear();
                }

                foreach (var samplingGroup in samplingGroups)
                {
                    Utils.SilentDispose(samplingGroup);
                }

                foreach (UaMonitoredItem monitoredItem in monitoredItems)
                {
                    Utils.SilentDispose(monitoredItem);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Stops all sampling groups and clears all items.
        /// </summary>
        public virtual void Shutdown()
        {
            lock (lock_)
            {
                // stop sampling groups.
                foreach (var samplingGroup in samplingGroups_)
                {
                    samplingGroup.Shutdown();
                }

                samplingGroups_.Clear();
                sampledItems_.Clear();
            }
        }

        /// <summary>
        /// Creates a new monitored item and calls StartMonitoring().
        /// </summary>
        public virtual UaMonitoredItem CreateMonitoredItem(
            UaServerOperationContext context,
            uint subscriptionId,
            double publishingInterval,
            TimestampsToReturn timestampsToReturn,
            uint monitoredItemId,
            object managerHandle,
            MonitoredItemCreateRequest itemToCreate,
            Opc.Ua.Range range,
            double minimumSamplingInterval)
        {
            // use publishing interval as sampling interval.
            var samplingInterval = itemToCreate.RequestedParameters.SamplingInterval;

            if (samplingInterval < 0)
            {
                samplingInterval = publishingInterval;
            }

            // limit the sampling interval.
            if (minimumSamplingInterval > 0 && samplingInterval < minimumSamplingInterval)
            {
                samplingInterval = minimumSamplingInterval;
            }

            // calculate queue size.
            var queueSize = itemToCreate.RequestedParameters.QueueSize;

            if (queueSize > maxQueueSize_)
            {
                queueSize = maxQueueSize_;
            }

            // get filter.
            MonitoringFilter filter = null;

            if (!ExtensionObject.IsNull(itemToCreate.RequestedParameters.Filter))
            {
                filter = itemToCreate.RequestedParameters.Filter.Body as MonitoringFilter;
            }

            // update limits for event filters.
            if (filter is EventFilter)
            {
                if (queueSize == 0)
                {
                    queueSize = Int32.MaxValue;
                }

                samplingInterval = 0;
            }

            // check if the queue size was not specified.
            if (queueSize == 0)
            {
                queueSize = 1;
            }

            // create monitored item.
            var monitoredItem = CreateMonitoredItem(
                server_,
                nodeManager_,
                managerHandle,
                subscriptionId,
                monitoredItemId,
                context.Session,
                itemToCreate.ItemToMonitor,
                context.DiagnosticsMask,
                timestampsToReturn,
                itemToCreate.MonitoringMode,
                itemToCreate.RequestedParameters.ClientHandle,
                filter,
                filter,
                range,
                samplingInterval,
                queueSize,
                itemToCreate.RequestedParameters.DiscardOldest,
                samplingInterval);

            // start sampling.
            StartMonitoring(context, monitoredItem);

            // return item.
            return monitoredItem;
        }

        /// <summary>
        /// Creates a new monitored item.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="nodeManager">The node manager.</param>
        /// <param name="managerHandle">The manager handle.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="id">The id.</param>
        /// <param name="session">The session.</param>
        /// <param name="itemToMonitor">The item to monitor.</param>
        /// <param name="diagnosticsMasks">The diagnostics masks.</param>
        /// <param name="timestampsToReturn">The timestamps to return.</param>
        /// <param name="monitoringMode">The monitoring mode.</param>
        /// <param name="clientHandle">The client handle.</param>
        /// <param name="originalFilter">The original filter.</param>
        /// <param name="filterToUse">The filter to use.</param>
        /// <param name="range">The range.</param>
        /// <param name="samplingInterval">The sampling interval.</param>
        /// <param name="queueSize">Size of the queue.</param>
        /// <param name="discardOldest">if set to <c>true</c> [discard oldest].</param>
        /// <param name="minimumSamplingInterval">The minimum sampling interval.</param>
        /// <returns>The monitored item.</returns>
        protected virtual UaMonitoredItem CreateMonitoredItem(
            IUaServerData server,
            IUaBaseNodeManager nodeManager,
            object managerHandle,
            uint subscriptionId,
            uint id,
            Sessions.Session session,
            ReadValueId itemToMonitor,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            MonitoringFilter originalFilter,
            MonitoringFilter filterToUse,
            Opc.Ua.Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            double minimumSamplingInterval)
        {
            return new UaMonitoredItem(
                server,
                nodeManager,
                managerHandle,
                subscriptionId,
                id,
                itemToMonitor,
                diagnosticsMasks,
                timestampsToReturn,
                monitoringMode,
                clientHandle,
                originalFilter,
                filterToUse,
                range,
                samplingInterval,
                queueSize,
                discardOldest,
                minimumSamplingInterval);
        }

        /// <summary>
        /// Modifies a monitored item and calls ModifyMonitoring().
        /// </summary>
        public virtual ServiceResult ModifyMonitoredItem(
            UaServerOperationContext context,
            TimestampsToReturn timestampsToReturn,
            IUaSampledDataChangeMonitoredItem monitoredItem,
            MonitoredItemModifyRequest itemToModify,
            Opc.Ua.Range range)
        {
            // use existing interval as sampling interval.
            var samplingInterval = itemToModify.RequestedParameters.SamplingInterval;

            if (samplingInterval < 0)
            {
                samplingInterval = monitoredItem.SamplingInterval;
            }

            // limit the sampling interval.
            var minimumSamplingInterval = monitoredItem.MinimumSamplingInterval;

            if (minimumSamplingInterval > 0 && samplingInterval < minimumSamplingInterval)
            {
                samplingInterval = minimumSamplingInterval;
            }

            // calculate queue size.
            var queueSize = itemToModify.RequestedParameters.QueueSize;

            if (queueSize == 0)
            {
                queueSize = monitoredItem.QueueSize;
            }

            if (queueSize > maxQueueSize_)
            {
                queueSize = maxQueueSize_;
            }

            // get filter.
            MonitoringFilter filter = null;

            if (!ExtensionObject.IsNull(itemToModify.RequestedParameters.Filter))
            {
                filter = (MonitoringFilter)itemToModify.RequestedParameters.Filter.Body;
            }

            // update limits for event filters.
            if (filter is EventFilter)
            {
                samplingInterval = 0;
            }

            // modify the item attributes.
            var error = monitoredItem.ModifyAttributes(
                context.DiagnosticsMask,
                timestampsToReturn,
                itemToModify.RequestedParameters.ClientHandle,
                filter,
                filter,
                range,
                samplingInterval,
                queueSize,
                itemToModify.RequestedParameters.DiscardOldest);

            // state of item did not change if an error returned here.
            if (ServiceResult.IsBad(error))
            {
                return error;
            }

            // update sampling.
            ModifyMonitoring(context, monitoredItem);

            // everything is ok.
            return ServiceResult.Good;
        }

        /// <summary>
        /// Starts monitoring the item.
        /// </summary>
        /// <remarks>
        /// It will use the external source for monitoring if the source accepts the item.
        /// The changes will not take affect until the ApplyChanges() method is called.
        /// </remarks>
        public virtual void StartMonitoring(UaServerOperationContext context, IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            lock (lock_)
            {
                // do nothing for disabled or exception based items.
                if (monitoredItem.MonitoringMode == MonitoringMode.Disabled || monitoredItem.MinimumSamplingInterval == 0)
                {
                    sampledItems_.Add(monitoredItem, null);
                    return;
                }

                // find a suitable sampling group.
                foreach (var samplingGroup in samplingGroups_)
                {
                    if (samplingGroup.StartMonitoring(context, monitoredItem))
                    {
                        sampledItems_.Add(monitoredItem, samplingGroup);
                        return;
                    }
                }

                // create a new sampling group.
                var samplingGroup2 = new SamplingGroup(
                    server_,
                    nodeManager_,
                    samplingRates_,
                    context,
                    monitoredItem.SamplingInterval);

                samplingGroup2.StartMonitoring(context, monitoredItem);

                samplingGroups_.Add(samplingGroup2);
                sampledItems_.Add(monitoredItem, samplingGroup2);
            }
        }

        /// <summary>
        /// Changes monitoring attributes the item.
        /// </summary>
        /// <remarks>
        /// It will call the external source to change the monitoring if an external source was provided originally.
        /// The changes will not take affect until the ApplyChanges() method is called.
        /// </remarks>
        public virtual void ModifyMonitoring(UaServerOperationContext context, IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            lock (lock_)
            {
                // find existing sampling group.
                SamplingGroup samplingGroup = null;

                if (sampledItems_.TryGetValue(monitoredItem, out samplingGroup))
                {
                    if (samplingGroup != null)
                    {
                        if (samplingGroup.ModifyMonitoring(context, monitoredItem))
                        {
                            return;
                        }
                    }

                    sampledItems_.Remove(monitoredItem);
                }

                // assign to a new sampling group.
                StartMonitoring(context, monitoredItem);
                return;
            }
        }

        /// <summary>
        /// Stops monitoring the item.
        /// </summary>
        /// <remarks>
        /// It will call the external source to stop the monitoring if an external source was provided originally.
        /// The changes will not take affect until the ApplyChanges() method is called.
        /// </remarks>
        public virtual void StopMonitoring(IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            lock (lock_)
            {
                // check for sampling group.
                SamplingGroup samplingGroup = null;

                if (sampledItems_.TryGetValue(monitoredItem, out samplingGroup))
                {
                    if (samplingGroup != null)
                    {
                        samplingGroup.StopMonitoring(monitoredItem);
                    }

                    sampledItems_.Remove(monitoredItem);
                    return;
                }
            }
        }

        /// <summary>
        /// Applies any pending changes caused by adding,changing or removing monitored items.
        /// </summary>
        public virtual void ApplyChanges()
        {
            lock (lock_)
            {
                var unusedGroups = new List<SamplingGroup>();

                // apply changes to groups.
                foreach (var samplingGroup in samplingGroups_)
                {
                    if (samplingGroup.ApplyChanges())
                    {
                        unusedGroups.Add(samplingGroup);
                    }
                }

                // remove unused groups.
                foreach (var samplingGroup in unusedGroups)
                {
                    samplingGroup.Shutdown();
                    samplingGroups_.Remove(samplingGroup);
                }
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private IUaBaseNodeManager nodeManager_;
        private List<SamplingGroup> samplingGroups_;
        private Dictionary<IUaSampledDataChangeMonitoredItem, SamplingGroup> sampledItems_;
        private List<SamplingRateGroup> samplingRates_;
        private uint maxQueueSize_;

        /// <summary>
        /// The default sampling rates.
        /// </summary>
        private static readonly SamplingRateGroup[] defaultSamplingRates_ = new SamplingRateGroup[]
        {
            new SamplingRateGroup(100, 100, 4),
            new SamplingRateGroup(500, 250, 2),
            new SamplingRateGroup(1000, 1000, 4),
            new SamplingRateGroup(5000, 2500, 2),
            new SamplingRateGroup(10000, 10000, 4),
            new SamplingRateGroup(60000, 30000, 10),
            new SamplingRateGroup(300000, 60000, 15),
            new SamplingRateGroup(900000, 300000, 9),
            new SamplingRateGroup(3600000, 900000, 0)
        };

        #endregion
    }
}
