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
using System.Text;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Diagnostics;
#endregion

namespace SampleCompany.NodeManagers.SampleNodeManager
{
    /// <summary>
    /// Provides a basic monitored item implementation which does not support queuing.
    /// </summary>
    public class DataChangeMonitoredItem : IUaDataChangeMonitoredItem2
    {
        #region Constructors
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public DataChangeMonitoredItem(
            MonitoredNode source,
            uint id,
            uint attributeId,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            double samplingInterval,
            bool alwaysReportUpdates)
        {
            source_ = source;
            id_ = id;
            attributeId_ = attributeId;
            indexRange_ = indexRange;
            dataEncoding_ = dataEncoding;
            timestampsToReturn_ = timestampsToReturn;
            diagnosticsMasks_ = diagnosticsMasks;
            monitoringMode_ = monitoringMode;
            clientHandle_ = clientHandle;
            samplingInterval_ = samplingInterval;
            nextSampleTime_ = DateTime.UtcNow.Ticks;
            readyToPublish_ = false;
            readyToTrigger_ = false;
            resendData_ = false;
            alwaysReportUpdates_ = alwaysReportUpdates;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public DataChangeMonitoredItem(
            MonitoredNode source,
            uint id,
            uint attributeId,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            DataChangeFilter filter,
            Opc.Ua.Range range,
            bool alwaysReportUpdates)
        {
            source_ = source;
            id_ = id;
            attributeId_ = attributeId;
            indexRange_ = indexRange;
            dataEncoding_ = dataEncoding;
            timestampsToReturn_ = timestampsToReturn;
            diagnosticsMasks_ = diagnosticsMasks;
            monitoringMode_ = monitoringMode;
            clientHandle_ = clientHandle;
            samplingInterval_ = samplingInterval;
            nextSampleTime_ = DateTime.UtcNow.Ticks;
            readyToPublish_ = false;
            readyToTrigger_ = false;
            resendData_ = false;
            queue_ = null;
            filter_ = filter;
            range_ = 0;
            alwaysReportUpdates_ = alwaysReportUpdates;

            if (range != null)
            {
                range_ = range.High - range.Low;
            }

            if (queueSize > 1)
            {
                queue_ = new MonitoredItemQueue(id);
                queue_.SetQueueSize(queueSize, discardOldest, diagnosticsMasks);
                queue_.SetSamplingInterval(samplingInterval);
            }
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets the id for the attribute being monitored.
        /// </summary>
        public uint AttributeId
        {
            get { return attributeId_; }
        }

        /// <summary>
        /// Gets the index range used to selected a subset of the value.
        /// </summary>
        public NumericRange IndexRange
        {
            get { return indexRange_; }
        }

        /// <summary>
        /// Gets the data encoding to use when returning the value.
        /// </summary>
        public QualifiedName DataEncoding
        {
            get { return dataEncoding_; }
        }

        /// <summary>
        /// Whether the monitored item should report a value without checking if it was changed.
        /// </summary>
        public bool AlwaysReportUpdates
        {
            get { return alwaysReportUpdates_; }
            set { alwaysReportUpdates_ = value; }
        }

        /// <summary>
        /// The number of milliseconds until the next sample.
        /// </summary>
        public int TimeToNextSample
        {
            get
            {
                lock (lock_)
                {
                    if (monitoringMode_ == MonitoringMode.Disabled)
                    {
                        return Int32.MaxValue;
                    }

                    DateTime now = DateTime.UtcNow;

                    if (nextSampleTime_ <= now.Ticks)
                    {
                        return 0;
                    }

                    return (int)((nextSampleTime_ - now.Ticks) / TimeSpan.TicksPerMillisecond);
                }
            }
        }

        /// <summary>
        /// The monitoring mode.
        /// </summary>
        public MonitoringMode MonitoringMode
        {
            get
            {
                return monitoringMode_;
            }
        }

        /// <summary>
        /// The sampling interval.
        /// </summary>
        public double SamplingInterval
        {
            get
            {
                lock (lock_)
                {
                    return samplingInterval_;
                }
            }
        }

        /// <summary>
        /// Modifies the monitored item parameters,
        /// </summary>
        public ServiceResult Modify(
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            uint clientHandle,
            double samplingInterval)
        {
            return Modify(diagnosticsMasks, timestampsToReturn, clientHandle, samplingInterval, 0, false, null, null);
        }

        /// <summary>
        /// Modifies the monitored item parameters,
        /// </summary>
        public ServiceResult Modify(
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            uint clientHandle,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            DataChangeFilter filter,
            Opc.Ua.Range range)
        {
            lock (lock_)
            {
                diagnosticsMasks_ = diagnosticsMasks;
                timestampsToReturn_ = timestampsToReturn;
                clientHandle_ = clientHandle;

                // subtract the previous sampling interval.
                long oldSamplingInterval = (long)(samplingInterval_ * TimeSpan.TicksPerMillisecond);

                if (oldSamplingInterval < nextSampleTime_)
                {
                    nextSampleTime_ -= oldSamplingInterval;
                }

                samplingInterval_ = samplingInterval;

                // calculate the next sampling interval.                
                long newSamplingInterval = (long)(samplingInterval_ * TimeSpan.TicksPerMillisecond);

                if (samplingInterval_ > 0)
                {
                    nextSampleTime_ += newSamplingInterval;
                }
                else
                {
                    nextSampleTime_ = 0;
                }

                // update the filter and the range.
                filter_ = filter;
                range_ = 0;

                if (range != null)
                {
                    range_ = range.High - range.Low;
                }

                // update the queue size.
                if (queueSize > 1)
                {
                    if (queue_ == null)
                    {
                        queue_ = new MonitoredItemQueue(id_);
                    }

                    queue_.SetQueueSize(queueSize, discardOldest, diagnosticsMasks);
                    queue_.SetSamplingInterval(samplingInterval);
                }
                else
                {
                    queue_ = null;
                }

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Called when the attribute being monitored changed. Reads and queues the value.
        /// </summary>
        public void ValueChanged(ISystemContext context)
        {
            DataValue value = new DataValue();

            ServiceResult error = source_.Node.ReadAttribute(context, attributeId_, NumericRange.Empty, null, value);

            if (ServiceResult.IsBad(error))
            {
                value = new DataValue(error.StatusCode);
            }

            value.ServerTimestamp = DateTime.UtcNow;

            QueueValue(value, error, false);
        }
        #endregion

        #region IMonitoredItem Members
        /// <summary>
        /// The node manager for the monitored item.
        /// </summary>
        public IUaBaseNodeManager NodeManager
        {
            get { return source_.NodeManager; }
        }

        /// <summary>
        /// The session for the monitored item.
        /// </summary>
        public Session Session
        {
            get
            {
                IUaSubscription subscription = subscription_;

                if (subscription != null)
                {
                    return subscription.Session;
                }

                return null;
            }
        }

        /// <summary>
        /// The identifier for the subscription that the monitored item belongs to.
        /// </summary>
        public uint SubscriptionId
        {
            get
            {
                IUaSubscription subscription = subscription_;

                if (subscription != null)
                {
                    return subscription.Id;
                }

                return 0;
            }
        }

        /// <summary>
        /// The unique identifier for the monitored item.
        /// </summary>
        public uint Id
        {
            get { return id_; }
        }

        /// <summary>
        /// The client handle.
        /// </summary>
        public uint ClientHandle
        {
            get { return clientHandle_; }
        }

        /// <summary>
        /// The callback to use to notify the subscription when values are ready to publish.
        /// </summary>
        public IUaSubscription SubscriptionCallback
        {
            get
            {
                return subscription_;
            }

            set
            {
                subscription_ = value;
            }
        }

        /// <summary>
        /// The handle assigned to the monitored item by the node manager.
        /// </summary>
        public object ManagerHandle
        {
            get { return source_; }
        }

        /// <summary>
        /// The type of monitor item.
        /// </summary>
        public int MonitoredItemType
        {
            get { return UaMonitoredItemTypeMask.DataChange; }
        }

        /// <summary>
        /// Returns true if the item is ready to publish.
        /// </summary>
        public bool IsReadyToPublish
        {
            get
            {
                lock (lock_)
                {
                    // check if not ready to publish.
                    if (!readyToPublish_)
                    {
                        return false;
                    }

                    // check if monitoring was turned off.
                    if (monitoringMode_ != MonitoringMode.Reporting)
                    {
                        return false;
                    }

                    // re-queue if too little time has passed since the last publish.
                    long now = DateTime.UtcNow.Ticks;

                    if (nextSampleTime_ > now)
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether the item is ready to trigger in case it has some linked items.
        /// </summary>
        public bool IsReadyToTrigger
        {
            get
            {
                lock (lock_)
                {
                    // only allow to trigger if sampling or reporting.
                    if (monitoringMode_ == MonitoringMode.Disabled)
                    {
                        return false;
                    }

                    return readyToTrigger_;
                }
            }

            set
            {
                lock (lock_)
                {
                    readyToTrigger_ = value;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsResendData
        {
            get
            {
                lock (lock_)
                {
                    return resendData_;
                }
            }
        }

        /// <summary>
        /// Returns the results for the create request.
        /// </summary>
        public ServiceResult GetCreateResult(out MonitoredItemCreateResult result)
        {
            lock (lock_)
            {
                result = new MonitoredItemCreateResult();

                result.MonitoredItemId = id_;
                result.StatusCode = StatusCodes.Good;
                result.RevisedSamplingInterval = samplingInterval_;
                result.RevisedQueueSize = 0;
                result.FilterResult = null;

                if (queue_ != null)
                {
                    result.RevisedQueueSize = queue_.QueueSize;
                }

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Returns the results for the modify request.
        /// </summary>
        public ServiceResult GetModifyResult(out MonitoredItemModifyResult result)
        {
            lock (lock_)
            {
                result = new MonitoredItemModifyResult();

                result.StatusCode = StatusCodes.Good;
                result.RevisedSamplingInterval = samplingInterval_;
                result.RevisedQueueSize = 0;
                result.FilterResult = null;

                if (queue_ != null)
                {
                    result.RevisedQueueSize = queue_.QueueSize;
                }

                return ServiceResult.Good;
            }
        }

        /// <inheritdoc/>
        public void SetupResendDataTrigger()
        {
            lock (lock_)
            {
                if (monitoringMode_ == MonitoringMode.Reporting)
                {
                    resendData_ = true;
                }
            }
        }
        #endregion

        #region IUaDataChangeMonitoredItem Members
        /// <inheritdoc/>
        public void QueueValue(DataValue value, ServiceResult error)
        {
            QueueValue(value, error, false);
        }
        #endregion

        #region IUaDataChangeMonitoredItem2 Members
        /// <inheritdoc/>
        public void QueueValue(DataValue value, ServiceResult error, bool ignoreFilters)
        {
            lock (lock_)
            {
                // check if value has changed.
                if (!alwaysReportUpdates_ && !ignoreFilters)
                {
                    if (!UaMonitoredItem.ValueChanged(value, error, lastValue_, lastError_, filter_, range_))
                    {
                        return;
                    }
                }

                // make a shallow copy of the value.
                if (value != null)
                {
                    DataValue copy = new DataValue();

                    copy.WrappedValue = value.WrappedValue;
                    copy.StatusCode = value.StatusCode;
                    copy.SourceTimestamp = value.SourceTimestamp;
                    copy.SourcePicoseconds = value.SourcePicoseconds;
                    copy.ServerTimestamp = value.ServerTimestamp;
                    copy.ServerPicoseconds = value.ServerPicoseconds;

                    value = copy;

                    // ensure the data value matches the error status code.
                    if (error != null && error.StatusCode.Code != 0)
                    {
                        value.StatusCode = error.StatusCode;
                    }
                }

                lastValue_ = value;
                lastError_ = error;

                // queue value.
                if (queue_ != null)
                {
                    queue_.QueueValue(value, error);
                }

                // flag the item as ready to publish.
                readyToPublish_ = true;
                readyToTrigger_ = true;
            }
        }

        /// <summary>
        /// Sets a flag indicating that the semantics for the monitored node have changed.
        /// </summary>
        /// <remarks>
        /// The StatusCode for next value reported by the monitored item will have the SemanticsChanged bit set.
        /// </remarks>
        public void SetSemanticsChanged()
        {
            lock (lock_)
            {
                semanticsChanged_ = true;
            }
        }

        /// <summary>
        /// Sets a flag indicating that the structure of the monitored node has changed.
        /// </summary>
        /// <remarks>
        /// The StatusCode for next value reported by the monitored item will have the StructureChanged bit set.
        /// </remarks>
        public void SetStructureChanged()
        {
            lock (lock_)
            {
                structureChanged_ = true;
            }
        }

        /// <summary>
        /// Changes the monitoring mode.
        /// </summary>
        public MonitoringMode SetMonitoringMode(MonitoringMode monitoringMode)
        {
            lock (lock_)
            {
                MonitoringMode previousMode = monitoringMode_;

                if (previousMode == monitoringMode)
                {
                    return previousMode;
                }

                if (previousMode == MonitoringMode.Disabled)
                {
                    nextSampleTime_ = DateTime.UtcNow.Ticks;
                    lastError_ = null;
                    lastValue_ = null;
                }

                monitoringMode_ = monitoringMode;

                if (monitoringMode == MonitoringMode.Disabled)
                {
                    readyToPublish_ = false;
                    readyToTrigger_ = false;
                }

                return previousMode;
            }
        }

        /// <summary>
        /// No filters supported.
        /// </summary>
        public DataChangeFilter DataChangeFilter
        {
            get { return filter_; }
        }

        /// <summary>
        /// Increments the sample time to the next interval.
        /// </summary>
        private void IncrementSampleTime()
        {
            // update next sample time.
            long now = DateTime.UtcNow.Ticks;
            long samplingInterval = (long)(samplingInterval_ * TimeSpan.TicksPerMillisecond);

            if (nextSampleTime_ > 0)
            {
                long delta = now - nextSampleTime_;

                if (samplingInterval > 0 && delta >= 0)
                {
                    nextSampleTime_ += ((delta / samplingInterval) + 1) * samplingInterval;
                }
            }

            // set sampling time based on current time.
            else
            {
                nextSampleTime_ = now + samplingInterval;
            }
        }

        /// <summary>
        /// Called by the subscription to publish any notification.
        /// </summary>
        public bool Publish(UaServerOperationContext context, Queue<MonitoredItemNotification> notifications, Queue<DiagnosticInfo> diagnostics)
        {
            lock (lock_)
            {
                // check if not ready to publish.
                if (!IsReadyToPublish)
                {
                    if (!resendData_)
                    {
                        return false;
                    }
                }
                else
                {
                    // update sample time.
                    IncrementSampleTime();
                }

                // update publish flag.
                readyToPublish_ = false;
                readyToTrigger_ = false;

                // check if queuing is enabled.
                if (queue_ != null && (!resendData_ || queue_.ItemsInQueue != 0))
                {
                    DataValue value = null;
                    ServiceResult error = null;

                    while (queue_.Publish(out value, out error))
                    {
                        Publish(context, value, error, notifications, diagnostics);

                        if (resendData_)
                        {
                            readyToPublish_ = queue_.ItemsInQueue > 0;
                            break;
                        }
                    }
                }
                else
                {
                    Publish(context, lastValue_, lastError_, notifications, diagnostics);
                }

                // update flags
                resendData_ = false;

                return true;
            }
        }

        /// <summary>
        /// Publishes a value.
        /// </summary>
        private void Publish(
            UaServerOperationContext context,
            DataValue value,
            ServiceResult error,
            Queue<MonitoredItemNotification> notifications,
            Queue<DiagnosticInfo> diagnostics)
        {
            // set semantics changed bit.
            if (semanticsChanged_)
            {
                if (value != null)
                {
                    value.StatusCode = value.StatusCode.SetSemanticsChanged(true);
                }

                if (error != null)
                {
                    error = new ServiceResult(
                        error.StatusCode.SetSemanticsChanged(true),
                        error.SymbolicId,
                        error.NamespaceUri,
                        error.LocalizedText,
                        error.AdditionalInfo,
                        error.InnerResult);
                }

                semanticsChanged_ = false;
            }

            // set structure changed bit.
            if (structureChanged_)
            {
                if (value != null)
                {
                    value.StatusCode = value.StatusCode.SetStructureChanged(true);
                }

                if (error != null)
                {
                    error = new ServiceResult(
                        error.StatusCode.SetStructureChanged(true),
                        error.SymbolicId,
                        error.NamespaceUri,
                        error.LocalizedText,
                        error.AdditionalInfo,
                        error.InnerResult);
                }

                structureChanged_ = false;
            }

            // copy data value.
            MonitoredItemNotification item = new MonitoredItemNotification();

            item.ClientHandle = clientHandle_;
            item.Value = value;

            // apply timestamp filter.
            if (timestampsToReturn_ != TimestampsToReturn.Server && timestampsToReturn_ != TimestampsToReturn.Both)
            {
                item.Value.ServerTimestamp = DateTime.MinValue;
            }

            if (timestampsToReturn_ != TimestampsToReturn.Source && timestampsToReturn_ != TimestampsToReturn.Both)
            {
                item.Value.SourceTimestamp = DateTime.MinValue;
            }

            notifications.Enqueue(item);

            // update diagnostic info.
            DiagnosticInfo diagnosticInfo = null;

            if (lastError_ != null)
            {
                if ((diagnosticsMasks_ & DiagnosticsMasks.OperationAll) != 0)
                {
                    diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(source_.Server, context, lastError_);
                }
            }

            diagnostics.Enqueue(diagnosticInfo);
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private MonitoredNode source_;
        private IUaSubscription subscription_;
        private uint id_;
        private DataValue lastValue_;
        private ServiceResult lastError_;
        private uint attributeId_;
        private NumericRange indexRange_;
        private QualifiedName dataEncoding_;
        private TimestampsToReturn timestampsToReturn_;
        private DiagnosticsMasks diagnosticsMasks_;
        private uint clientHandle_;
        private double samplingInterval_;
        private MonitoredItemQueue queue_;
        private DataChangeFilter filter_;
        private double range_;
        private MonitoringMode monitoringMode_;
        private long nextSampleTime_;
        private bool readyToPublish_;
        private bool readyToTrigger_;
        private bool alwaysReportUpdates_;
        private bool semanticsChanged_;
        private bool structureChanged_;
        private bool resendData_;
        #endregion
    }
}
