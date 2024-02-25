#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// An object which periodically reads the items and updates the cache.
    /// </summary>
    public class SamplingGroup : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new instance of a sampling group.
        /// </summary>
        public SamplingGroup(
            IUaServerData         server,
            IUaBaseNodeManager            nodeManager,
            List<SamplingRateGroup> samplingRates,
            UaServerOperationContext        context,
            double                  samplingInterval)
        {
            if (server == null)        throw new ArgumentNullException(nameof(server));
            if (nodeManager == null)   throw new ArgumentNullException(nameof(nodeManager));
            if (samplingRates == null) throw new ArgumentNullException(nameof(samplingRates));

            server_           = server;
            nodeManager_      = nodeManager;
            samplingRates_    = samplingRates;
            session_          = context.Session;
            diagnosticsMask_  = (DiagnosticsMasks)context.DiagnosticsMask & DiagnosticsMasks.OperationAll;
            samplingInterval_ = AdjustSamplingInterval(samplingInterval);

            itemsToAdd_    = new List<IUaSampledDataChangeMonitoredItem>();
            itemsToRemove_ = new List<IUaSampledDataChangeMonitoredItem>();
            items_         = new Dictionary<uint, IUaSampledDataChangeMonitoredItem>();

            // create a event to signal shutdown.
            shutdownEvent_ = new ManualResetEvent(true);
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
                lock (lock_)
                {
                    shutdownEvent_.Set();
                    samplingRates_.Clear();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the sampling thread which periodically reads the items in the group.
        /// </summary>
        public void Startup()
        {
            lock (lock_)
            {
                shutdownEvent_.Reset();

                Task.Run(() =>
                {
                    SampleMonitoredItems(samplingInterval_);
                });
            }
        }

        /// <summary>
        /// Stops the sampling thread.
        /// </summary>
        public void Shutdown()
        {
            lock (lock_)
            {
                shutdownEvent_.Set();
                items_.Clear();
            }
        }

        /// <summary>
        /// Checks if the monitored item can be handled by the group.
        /// </summary>
        /// <returns>
        /// True if the item was added to the group.
        /// </returns>
        /// <remarks>
        /// The ApplyChanges() method must be called to actually start sampling the item. 
        /// </remarks>
        public bool StartMonitoring(UaServerOperationContext context, IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            lock (lock_)
            {
                if (MeetsGroupCriteria(context, monitoredItem))
                {
                    itemsToAdd_.Add(monitoredItem);
                    monitoredItem.SetSamplingInterval(samplingInterval_);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if the monitored item can still be handled by the group.
        /// </summary>
        /// <returns>
        /// False if the item has be marked for removal from the group.
        /// </returns>
        /// <remarks>
        /// The ApplyChanges() method must be called to actually stop sampling the item. 
        /// </remarks>
        public bool ModifyMonitoring(UaServerOperationContext context, IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            lock (lock_)
            {
                if (items_.ContainsKey(monitoredItem.Id))
                {
                    if (MeetsGroupCriteria(context, monitoredItem))
                    {
                        monitoredItem.SetSamplingInterval(samplingInterval_);
                        return true;
                    }

                    itemsToRemove_.Add(monitoredItem);
                }

                return false;
            }
        }

        /// <summary>
        /// Stops monitoring the item.
        /// </summary>
        /// <returns>
        /// Returns true if the items was marked for removal from the group.
        /// </returns>
        public bool StopMonitoring(IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            lock (lock_)
            {
                if (items_.ContainsKey(monitoredItem.Id))
                {
                    itemsToRemove_.Add(monitoredItem);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Updates the group by apply any pending changes.
        /// </summary>
        /// <returns>
        /// Returns true if the group has no more items and can be dropped.
        /// </returns>
        public bool ApplyChanges()
        {
            lock (lock_)
            {
                // add items.
                var itemsToSample = new List<IUaSampledDataChangeMonitoredItem>();

                for (var ii = 0; ii < itemsToAdd_.Count; ii++)
                {
                    var monitoredItem = itemsToAdd_[ii];

                    if (!items_.ContainsKey(monitoredItem.Id))
                    {
                        items_.Add(monitoredItem.Id, monitoredItem);
                        
                        if (monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                        {
                            itemsToSample.Add(monitoredItem);
                        }
                    }
                }

                itemsToAdd_.Clear();

                // collect first sample.
                if (itemsToSample.Count > 0)
                {
                    Task.Run(() => {
                        DoSample(itemsToSample);
                    });
                }

                // remove items.
                for (var ii = 0; ii < itemsToRemove_.Count; ii++)
                {
                    items_.Remove(itemsToRemove_[ii].Id);
                }

                itemsToRemove_.Clear();

                // start the group if it is not running.
                if (items_.Count > 0)
                {
                    Startup();
                }

                // stop the group if it is running.
                else if (items_.Count == 0)
                {
                    Shutdown();
                }

                // can be shutdown if no items left.
                return items_.Count == 0;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks if the item meets the group's criteria.
        /// </summary>
        private bool MeetsGroupCriteria(UaServerOperationContext context, IUaSampledDataChangeMonitoredItem monitoredItem)
        {
            // can only sample variables.
            if ((monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.DataChange) == 0)
            {
                return false;
            }

            // can't sample disabled items.
            if (monitoredItem.MonitoringMode == MonitoringMode.Disabled)
            {
                return false;
            }

            // check sampling interval.
            if (AdjustSamplingInterval(monitoredItem.SamplingInterval) != samplingInterval_)
            {
                return false;
            }

            // compare session.
            if (context.SessionId != session_.Id)
            {
                return false;
            }

            // check the diagnostics marks.
            if (diagnosticsMask_ != (context.DiagnosticsMask & DiagnosticsMasks.OperationAll))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ensures the requested sampling interval lines up with one of the supported sampling rates.
        /// </summary>
        private double AdjustSamplingInterval(double samplingInterval)
        {
            foreach (var samplingRate in samplingRates_)
            {
                // groups are ordered by start rate.
                if (samplingInterval <= samplingRate.Start)
                {
                    return samplingRate.Start;
                }

                // check if within range specified by the group.
                var maxSamplingRate = samplingRate.Start;

                if (samplingRate.Increment > 0)
                {
                    maxSamplingRate += samplingRate.Increment*samplingRate.Count;
                }

                if (samplingInterval > maxSamplingRate)
                {
                    continue;
                }

                // find sampling rate within rate group.
                if (samplingInterval == maxSamplingRate)
                {
                    return maxSamplingRate;
                }

                for (var ii = samplingRate.Start; ii <= maxSamplingRate; ii += samplingRate.Increment)
                {
                    if (ii >= samplingInterval)
                    {
                        return ii;
                    }
                }
            }

            return samplingInterval;
        }

        /// <summary>
        /// Periodically checks if the sessions have timed out.
        /// </summary>
        private void SampleMonitoredItems(object data)
        {
            try
            {
                Utils.LogTrace("Server: {0} Thread Started.", Thread.CurrentThread.Name);

                var sleepCycle = Convert.ToInt32(data, CultureInfo.InvariantCulture);
                var timeToWait = sleepCycle;

                while (server_.IsRunning)
                {
                    var start = DateTime.UtcNow;

                    // wait till next sample.
                    if (shutdownEvent_.WaitOne(timeToWait))
                    {
                        break;
                    }

                    // get current list of items to sample.
                    var items = new List<IUaSampledDataChangeMonitoredItem>();

                    lock (lock_)
                    {
                        uint disabledItemCount = 0;
                        var enumerator = items_.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            var monitoredItem = enumerator.Current.Value;

                            if (monitoredItem.MonitoringMode == MonitoringMode.Disabled)
                            {
                                disabledItemCount++;
                                continue;
                            }

                            // check whether the item should be sampled.
                            //if (!monitoredItem.SamplingIntervalExpired())
                            //{
                            //    continue;
                            //}

                            items.Add(monitoredItem);
                        }
                    }

                    // sample the values.
                    DoSample(items);

                    var delay = (int)(DateTime.UtcNow - start).TotalMilliseconds;
                    timeToWait = sleepCycle;

                    if (delay > sleepCycle)
                    {
                        timeToWait = 2*sleepCycle - delay;

                        if (timeToWait < 0)
                        {
                            Utils.LogWarning("WARNING: SamplingGroup cannot sample fast enough. TimeToSample={0}ms, SamplingInterval={1}ms", delay, sleepCycle);
                            timeToWait = sleepCycle;
                        }
                    }
                }

                Utils.LogTrace("Server: {0} Thread Exited Normally.", Thread.CurrentThread.Name);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Server: SampleMonitoredItems Thread Exited Unexpectedly.");
            }
        }

        /// <summary>
        /// Samples the values of the items.
        /// </summary>
        private void DoSample(object state)
        {
            try
            {
                var items = state as List<IUaSampledDataChangeMonitoredItem>;

                // read values for all enabled items.
                if (items != null && items.Count > 0)
                {
                    var itemsToRead = new ReadValueIdCollection(items.Count);
                    var values = new DataValueCollection(items.Count);
                    var errors = new List<ServiceResult>(items.Count);

                    // allocate space for results.
                    for (var ii = 0; ii < items.Count; ii++)
                    {
                        var readValueId = items[ii].GetReadValueId();
                        readValueId.Processed = false;
                        itemsToRead.Add(readValueId);

                        values.Add(null);
                        errors.Add(null);
                    }

                    var context = new UaServerOperationContext(session_, diagnosticsMask_);

                    // read values.
                    nodeManager_.Read(
                        context,
                        0,
                        itemsToRead,
                        values,
                        errors);

                    // update monitored items.
                    for (var ii = 0; ii < items.Count; ii++)
                    {
                        if (values[ii] == null)
                        {
                            values[ii] = new DataValue(StatusCodes.BadInternalError, DateTime.UtcNow);
                        }

                        items[ii].QueueValue(values[ii], errors[ii]);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Server: Unexpected error sampling values.");
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private IUaBaseNodeManager nodeManager_;
        private Sessions.Session session_;
        private DiagnosticsMasks diagnosticsMask_;
        private double samplingInterval_;
        private List<IUaSampledDataChangeMonitoredItem> itemsToAdd_;
        private List<IUaSampledDataChangeMonitoredItem> itemsToRemove_;
        private Dictionary<uint, IUaSampledDataChangeMonitoredItem> items_;
        private ManualResetEvent shutdownEvent_;
        private List<SamplingRateGroup> samplingRates_;
        #endregion
    }
}
