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
using System.Xml;
using System.Globalization;
using System.Threading;

using Opc.Ua;
using Range = Opc.Ua.Range;

using Technosoftware.UaServer.Aggregates;
using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// A handle that describes how to access a node/attribute via an i/o manager.
    /// </summary>
    public class UaMonitoredItem : IUaEventMonitoredItem, IUaSampledDataChangeMonitoredItem, IUaTriggeredMonitoredItem
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object with its node type.
        /// </summary>
        [Obsolete("Use UaMonitoredItem constructor without the session parameter.")]
        public UaMonitoredItem(
            IUaServerData server,
            UaServerContext context,
            IUaBaseNodeManager nodeManager,
            object mangerHandle,
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
            Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            double sourceSamplingInterval)
         : this(server, nodeManager, mangerHandle, subscriptionId,
            id, itemToMonitor, diagnosticsMasks, timestampsToReturn, monitoringMode,
            clientHandle, originalFilter, filterToUse, range, samplingInterval,
            queueSize, discardOldest, sourceSamplingInterval)
        {
        }

        /// <summary>
        /// Initializes the object with its node type.
        /// </summary>
        public UaMonitoredItem(
            IUaServerData server,
            IUaBaseNodeManager nodeManager,
            object mangerHandle,
            uint subscriptionId,
            uint id,
            ReadValueId itemToMonitor,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            MonitoringFilter originalFilter,
            MonitoringFilter filterToUse,
            Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            double sourceSamplingInterval)
        {
            if (itemToMonitor == null) throw new ArgumentNullException(nameof(itemToMonitor));

            Initialize();

            server_ = server;
            NodeManager = nodeManager;
            ManagerHandle = mangerHandle;
            SubscriptionId = subscriptionId;
            id_ = id;
            nodeId_ = itemToMonitor.NodeId;
            attributeId_ = itemToMonitor.AttributeId;
            indexRange_ = itemToMonitor.IndexRange;
            parsedIndexRange_ = itemToMonitor.ParsedIndexRange;
            encoding_ = itemToMonitor.DataEncoding;
            diagnosticsMasks_ = diagnosticsMasks;
            timestampsToReturn_ = timestampsToReturn;
            monitoringMode_ = monitoringMode;
            clientHandle_ = clientHandle;
            originalFilter_ = originalFilter;
            filterToUse_ = filterToUse;
            range_ = 0;
            samplingInterval_ = samplingInterval;
            queueSize_ = queueSize;
            discardOldest_ = discardOldest;
            sourceSamplingInterval_ = (int)sourceSamplingInterval;
            calculator_ = null;
            nextSamplingTime_ = HiResClock.TickCount64;
            alwaysReportUpdates_ = false;

            MonitoredItemType = UaMonitoredItemTypeMask.DataChange;

            if (originalFilter is EventFilter)
            {
                MonitoredItemType = UaMonitoredItemTypeMask.Events;

                if (itemToMonitor.NodeId == Objects.Server)
                {
                    MonitoredItemType |= UaMonitoredItemTypeMask.AllEvents;
                }
            }

            // create aggregate calculator.
            var aggregateFilter = filterToUse as ServerAggregateFilter;

            if (filterToUse is ServerAggregateFilter)
            {
                calculator_ = server_.AggregateManager.CreateCalculator(
                    aggregateFilter.AggregateType,
                    aggregateFilter.StartTime,
                    DateTime.MaxValue,
                    aggregateFilter.ProcessingInterval,
                    aggregateFilter.Stepped,
                    aggregateFilter.AggregateConfiguration);
            }

            if (range != null)
            {
                range_ = range.High - range.Low;
            }

            // report change to item state.
            UaServerUtils.ReportCreateMonitoredItem(
                nodeId_,
                id_,
                samplingInterval_,
                queueSize_,
                discardOldest_,
                filterToUse_,
                monitoringMode_);

            InitializeQueue();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
            server_ = null;
            NodeManager = null;
            ManagerHandle = null;
            SubscriptionId = 0;
            id_ = 0;
            nodeId_ = null;
            attributeId_ = 0;
            indexRange_ = null;
            parsedIndexRange_ = NumericRange.Empty;
            encoding_ = null;
            clientHandle_ = 0;
            monitoringMode_ = MonitoringMode.Disabled;
            samplingInterval_ = 0;
            queueSize_ = 0;
            discardOldest_ = true;
            originalFilter_ = null;
            lastValue_ = null;
            lastError_ = null;
            events_ = null;
            overflow_ = false;
            readyToPublish_ = false;
            readyToTrigger_ = false;
            sourceSamplingInterval_ = 0;
            samplingError_ = ServiceResult.Good;
            resendData_ = false;
        }
        #endregion

        #region IUaMonitoredItem Members
        /// <summary>
        /// The node manager that created the item.
        /// </summary>
        public IUaBaseNodeManager NodeManager { get; private set; }

        /// <summary>
        /// The handle assigned by the node manager when it created the item.
        /// </summary>
        public object ManagerHandle { get; private set; }

        /// <summary>
        /// The identifier for the subscription that owns the monitored item.
        /// </summary>
        public uint SubscriptionId { get; private set; }

        /// <summary>
        /// A bit mask that indicates what the monitored item is.
        /// </summary>
        /// <remarks>
        /// Predefined bits are defined by the MonitoredItemTypeMasks class.
        /// NodeManagers may use the remaining bits.
        /// </remarks>
        public int MonitoredItemType { get; }

        /// <summary>
        /// Returns true if the item is ready to publish.
        /// </summary>
        public bool IsReadyToPublish
        {
            get
            {
                // check if aggregate interval has passed.
                if (calculator_ != null)
                {
                    if (calculator_.HasEndTimePassed(DateTime.UtcNow))
                    {
                        return true;
                    }
                }

                // check if not ready to publish in case it doesn't ResendData
                if (!readyToPublish_)
                {
                    UaServerUtils.EventLog.MonitoredItemReady(id_, "FALSE");
                    return false;
                }

                // check if it has been triggered.
                if (monitoringMode_ != MonitoringMode.Disabled && triggered_)
                {
                    UaServerUtils.EventLog.MonitoredItemReady(id_, "TRIGGERED");
                    return true;
                }

                // check if monitoring was turned off.
                if (monitoringMode_ != MonitoringMode.Reporting)
                {
                    UaServerUtils.EventLog.MonitoredItemReady(id_, "FALSE");
                    return false;
                }

                if (sourceSamplingInterval_ == 0)
                {
                    // re-queue if too little time has passed since the last publish, in case it doesn't ResendData
                    var now = HiResClock.TickCount64;

                    if (nextSamplingTime_ > now)
                    {
                        UaServerUtils.EventLog.MonitoredItemReady(id_, Utils.Format("FALSE {0}ms", nextSamplingTime_ - now));
                        return false;
                    }
                }
                UaServerUtils.EventLog.MonitoredItemReady(id_, "NORMAL");
                return true;
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

        /// <inheritdoc/>
        public void SetupResendDataTrigger()
        {
            lock (lock_)
            {
                if (monitoringMode_ == MonitoringMode.Reporting &&
                    (MonitoredItemType & UaMonitoredItemTypeMask.DataChange) != 0)
                {
                    resendData_ = true;
                }
            }
        }

        /// <summary>
        /// Sets a flag indicating that the item has been triggered and should publish.
        /// </summary>
        public bool SetTriggered()
        {
            lock (lock_)
            {
                if (readyToPublish_)
                {
                    Utils.LogTrace(Utils.TraceMasks.OperationDetail, "SetTriggered[{0}]", id_);
                    triggered_ = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets a flag indicating that the semantics for the monitored node have changed.
        /// </summary>
        /// <remarks>
        /// The StatusCode for next value reported by the monitored item will have the SemanticsChanged bit set.
        /// </remarks>
        public void SetSemanticsChanged()
        {
            semanticsChanged_ = true;
        }

        /// <summary>
        /// Sets a flag indicating that the structure of the monitored node has changed.
        /// </summary>
        /// <remarks>
        /// The StatusCode for next value reported by the monitored item will have the StructureChanged bit set.
        /// </remarks>
        public void SetStructureChanged()
        {
            structureChanged_ = true;
        }

        /// <summary>
        /// The filter used by the monitored item.
        /// </summary>
        public MonitoringFilter Filter => originalFilter_;

        /// <summary>
        /// The event filter used by the monitored item.
        /// </summary>
        public EventFilter EventFilter => originalFilter_ as EventFilter;

        /// <summary>
        /// The data change filter used by the monitored item.
        /// </summary>
        public DataChangeFilter DataChangeFilter => originalFilter_ as DataChangeFilter;

        /// <summary>
        /// The session that owns the monitored item.
        /// </summary>
        public Sessions.Session Session
        {
            get
            {
                lock (lock_)
                {
                    return subscription_?.Session;
                }
            }
        }

        /// <summary>
        /// The identifier for the item that is unique within the server.
        /// </summary>
        public uint Id => id_;

        /// <summary>
        /// The identifier for the client handle assigned to the monitored item.
        /// </summary>
        public uint ClientHandle => clientHandle_;

        /// <summary>
        /// The node id being monitored.
        /// </summary>
        public NodeId NodeId => nodeId_;

        /// <summary>
        /// The attribute being monitored.
        /// </summary>
        public uint AttributeId => attributeId_;

        /// <summary>
        /// The current monitoring mode for the item
        /// </summary>
        public MonitoringMode MonitoringMode => monitoringMode_;

        /// <summary>
        /// The sampling interval for the item.
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
        /// The minimum sampling interval for the item.
        /// </summary>
        public double MinimumSamplingInterval => sourceSamplingInterval_;

        /// <summary>
        /// The queue size for the item.
        /// </summary>
        public uint QueueSize => queueSize_;

        /// <summary>
        /// Gets number of elements actually contained in value queue.
        /// </summary>
        public int ItemsInQueue
        {
            get
            {
                lock (lock_)
                {
                    if (events_ != null)
                    {
                        return events_.Count;
                    }

                    if (queue_ != null)
                    {
                        return queue_.ItemsInQueue;
                    }

                    return 0;
                }
            }
        }

        /// <summary>
        /// The diagnostics masks to use when collecting notifications for the item.
        /// </summary>
        public DiagnosticsMasks DiagnosticsMasks => diagnosticsMasks_;

        /// <summary>
        /// The index range requested by the monitored item.
        /// </summary>
        public NumericRange IndexRange => parsedIndexRange_;

        /// <summary>
        /// The data encoding requested by the monitored item.
        /// </summary>
        public QualifiedName DataEncoding => encoding_;

        /// <summary>
        /// Whether the monitored item should report a value without checking if it was changed.
        /// </summary>
        public bool AlwaysReportUpdates
        {
            get => alwaysReportUpdates_;
            set => alwaysReportUpdates_ = value;
        }

        /// <summary>
        /// Returns a description of the item being monitored. 
        /// </summary>
        public ReadValueId GetReadValueId()
        {
            lock (lock_)
            {
                var valueId = new ReadValueId
                {
                    NodeId = nodeId_,
                    AttributeId = attributeId_,
                    IndexRange = indexRange_,
                    ParsedIndexRange = parsedIndexRange_,
                    DataEncoding = encoding_,
                    Handle = ManagerHandle
                };

                return valueId;
            }
        }

        /// <summary>
        /// Sets an error that occured in the sampling group.
        /// </summary>
        /// <remarks>
        /// The sampling group or node manager that owns the item may call this to indicate that
        /// a fatal error occurred which means the item will no longer receive any data updates.
        /// This error state can be cleared by calling this method and passing in ServiceResult.Good.
        /// </remarks>
        public void SetSamplingError(ServiceResult error)
        {
            lock (lock_)
            {
                if (error == null)
                {
                    samplingError_ = ServiceResult.Good;
                }

                samplingError_ = error;
            }
        }

        /// <summary>
        /// Returns the result after creating the monitor item.
        /// </summary>
        public ServiceResult GetCreateResult(out MonitoredItemCreateResult result)
        {
            lock (lock_)
            {
                result = new MonitoredItemCreateResult
                {
                    MonitoredItemId = id_,
                    RevisedSamplingInterval = samplingInterval_,
                    RevisedQueueSize = queueSize_,
                    StatusCode = StatusCodes.Good
                };


                if (ServiceResult.IsBad(samplingError_))
                {
                    result.StatusCode = samplingError_.Code;
                }

                return samplingError_;
            }
        }

        /// <summary>
        /// Returns the result after modifying the monitor item.
        /// </summary>
        public ServiceResult GetModifyResult(out MonitoredItemModifyResult result)
        {
            lock (lock_)
            {
                result = new MonitoredItemModifyResult
                {
                    RevisedSamplingInterval = samplingInterval_,
                    RevisedQueueSize = queueSize_,
                    StatusCode = StatusCodes.Good
                };


                if (ServiceResult.IsBad(samplingError_))
                {
                    result.StatusCode = samplingError_.Code;
                }

                return samplingError_;
            }
        }

        /// <summary>
        /// Modifies the attributes for monitored item.
        /// </summary>
        public ServiceResult ModifyAttributes(
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            uint clientHandle,
            MonitoringFilter originalFilter,
            MonitoringFilter filterToUse,
            Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest)
        {
            lock (lock_)
            {
                diagnosticsMasks_ = diagnosticsMasks;
                timestampsToReturn_ = timestampsToReturn;
                clientHandle_ = clientHandle;
                discardOldest_ = discardOldest;

                originalFilter_ = originalFilter;
                filterToUse_ = filterToUse;

                if (range != null)
                {
                    range_ = range.High - range.Low;
                }

                SetSamplingInterval(samplingInterval);
                queueSize_ = queueSize;

                // check if aggregate filter has been updated.
                var aggregateFilter = filterToUse as ServerAggregateFilter;

                if (filterToUse is ServerAggregateFilter)
                {
                    var existingFilter = filterToUse as ServerAggregateFilter;

                    var match = existingFilter != null;

                    if (match) if (existingFilter.AggregateType != aggregateFilter.AggregateType) match = false;
                    if (match) if (existingFilter.ProcessingInterval != aggregateFilter.ProcessingInterval) match = false;
                    if (match) if (existingFilter.StartTime != aggregateFilter.StartTime) match = false;
                    if (match) if (!existingFilter.AggregateConfiguration.IsEqual(aggregateFilter.AggregateConfiguration)) match = false;

                    if (!match)
                    {
                        calculator_ = server_.AggregateManager.CreateCalculator(
                            aggregateFilter.AggregateType,
                            aggregateFilter.StartTime,
                            DateTime.MaxValue,
                            aggregateFilter.ProcessingInterval,
                            aggregateFilter.Stepped,
                            aggregateFilter.AggregateConfiguration);
                    }
                }

                // report change to item state.
                UaServerUtils.ReportModifyMonitoredItem(
                    nodeId_,
                    id_,
                    samplingInterval_,
                    queueSize_,
                    discardOldest_,
                    filterToUse_,
                    monitoringMode_);

                InitializeQueue();

                return null;
            }
        }

        /// <summary>
        /// Updates the sampling interval for an item.
        /// </summary>
        public void SetSamplingInterval(double samplingInterval)
        {
            lock (lock_)
            {
                if (samplingInterval == -1)
                {
                    return;
                }

                // subtract the previous sampling interval.
                var oldSamplingInterval = (long)(samplingInterval_);

                if (oldSamplingInterval < nextSamplingTime_)
                {
                    nextSamplingTime_ -= oldSamplingInterval;
                }

                samplingInterval_ = samplingInterval;

                // calculate the next sampling interval.                
                var newSamplingInterval = (long)(samplingInterval_);

                if (samplingInterval_ > 0)
                {
                    nextSamplingTime_ += newSamplingInterval;
                }
                else
                {
                    nextSamplingTime_ = 0;
                }
            }
        }

        /// <summary>
        /// Changes the monitoring mode for the item.
        /// </summary>
        void IUaSampledDataChangeMonitoredItem.SetMonitoringMode(MonitoringMode monitoringMode)
        {
            SetMonitoringMode(monitoringMode);
        }

        /// <summary>
        /// Changes the monitoring mode for the item.
        /// </summary>
        void IUaEventMonitoredItem.SetMonitoringMode(MonitoringMode monitoringMode)
        {
            SetMonitoringMode(monitoringMode);
        }

        /// <summary>
        /// Changes the monitoring mode for the item.
        /// </summary>
        public MonitoringMode SetMonitoringMode(MonitoringMode monitoringMode)
        {
            lock (lock_)
            {
                var previousMode = monitoringMode_;

                if (previousMode == monitoringMode)
                {
                    return previousMode;
                }

                Utils.LogTrace("MONITORING MODE[{0}] {1} -> {2}", id_, monitoringMode_, monitoringMode);

                if (previousMode == MonitoringMode.Disabled)
                {
                    nextSamplingTime_ = HiResClock.TickCount64;
                    lastError_ = null;
                    lastValue_ = null;
                }

                monitoringMode_ = monitoringMode;

                if (monitoringMode == MonitoringMode.Disabled)
                {
                    readyToPublish_ = false;
                    readyToTrigger_ = false;
                    triggered_ = false;
                }

                // report change to item state.
                UaServerUtils.ReportModifyMonitoredItem(
                    nodeId_,
                    id_,
                    samplingInterval_,
                    queueSize_,
                    discardOldest_,
                    filterToUse_,
                    monitoringMode_);

                InitializeQueue();

                return previousMode;
            }
        }

        /// <summary>
        /// Adds an event to the queue.
        /// </summary>
        public virtual void QueueValue(DataValue value, ServiceResult error)
        {
            QueueValue(value, error, false);
        }

        /// <summary>
        /// Updates the queue with a data value or an error.
        /// </summary>
        public virtual void QueueValue(DataValue value, ServiceResult error, bool ignoreFilters)
        {
            lock (lock_)
            {
                // this method should only be called for variables.
                if ((MonitoredItemType & UaMonitoredItemTypeMask.DataChange) == 0)
                {
                    throw new ServiceResultException(StatusCodes.BadInternalError);
                }

                // check monitoring mode.
                if (monitoringMode_ == MonitoringMode.Disabled)
                {
                    return;
                }

                // make a shallow copy of the value.
                if (value != null)
                {
                    Utils.LogTrace(Utils.TraceMasks.OperationDetail, "RECEIVED VALUE[{0}] Value={1}", this.id_, value.WrappedValue);

                    var copy = new DataValue
                    {
                        WrappedValue = value.WrappedValue,
                        StatusCode = value.StatusCode,
                        SourceTimestamp = value.SourceTimestamp,
                        SourcePicoseconds = value.SourcePicoseconds,
                        ServerTimestamp = value.ServerTimestamp,
                        ServerPicoseconds = value.ServerPicoseconds
                    };


                    value = copy;

                    // ensure the data value matches the error status code.
                    if (error != null && error.StatusCode.Code != 0)
                    {
                        value.StatusCode = error.StatusCode;
                    }
                }

                // create empty value if none provided.
                if (ServiceResult.IsBad(error))
                {
                    if (value == null)
                    {
                        value = new DataValue
                        {
                            StatusCode = error.StatusCode,
                            SourceTimestamp = DateTime.UtcNow,
                            ServerTimestamp = DateTime.UtcNow
                        };
                    }
                }

                // this should never happen.
                if (value == null)
                {
                    return;
                }

                // apply aggregate filter.
                if (calculator_ != null)
                {
                    if (!calculator_.QueueRawValue(value))
                    {
                        Utils.LogTrace("Value received out of order: {1}, ServerHandle={0}", id_, value.SourceTimestamp.ToLocalTime().ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
                    }

                    var processedValue = calculator_.GetProcessedValue(false);

                    while (processedValue != null)
                    {
                        AddValueToQueue(processedValue, null);
                        processedValue = calculator_.GetProcessedValue(false);
                    }

                    return;
                }

                // apply filter to incoming item.
                if (!alwaysReportUpdates_ && !ignoreFilters)
                {
                    if (!ApplyFilter(value, error))
                    {
                        UaServerUtils.ReportFilteredValue(nodeId_, id_, value);
                        return;
                    }
                }

                UaServerUtils.ReportQueuedValue(nodeId_, id_, value);

                // add the value to the queue.
                AddValueToQueue(value, error);
            }
        }

        /// <summary>
        /// Sets the overflow bit.
        /// </summary>
        private ServiceResult SetOverflowBit(
            object value,
            ServiceResult error)
        {
            var dataValue = value as DataValue;

            if (dataValue != null)
            {
                dataValue.StatusCode = dataValue.StatusCode.SetOverflow(true);
            }

            if (error != null)
            {
                error = new ServiceResult(
                    error.StatusCode.SetOverflow(true),
                    error.SymbolicId,
                    error.NamespaceUri,
                    error.LocalizedText,
                    error.AdditionalInfo,
                    error.InnerResult);
            }

            return error;
        }

        /// <summary>
        /// Adds a value to the queue.
        /// </summary>
        private void AddValueToQueue(DataValue value, ServiceResult error)
        {
            if (queueSize_ > 1)
            {
                queue_.QueueValue(value, error);
            }

            if (lastValue_ != null)
            {
                readyToTrigger_ = true;
            }

            // save last value received.
            lastValue_ = value;
            lastError_ = error;
            readyToPublish_ = true;
            UaServerUtils.EventLog.QueueValue(id_, lastValue_.WrappedValue, lastValue_.StatusCode);
        }

        /// <summary>
        /// Whether the item is monitoring all events produced by the server.
        /// </summary>
        public bool MonitoringAllEvents => this.nodeId_ == ObjectIds.Server;

        /// <summary>
        /// Fetches the event fields from the event.
        /// </summary>
        private EventFieldList GetEventFields(FilterContext context, EventFilter filter, IFilterTarget instance)
        {
            // fetch the event fields.
            var fields = new EventFieldList { ClientHandle = clientHandle_, Handle = instance };

            foreach (var clause in filter.SelectClauses)
            {
                // get the value of the attribute (apply localization).
                var value = instance.GetAttributeValue(
                    context,
                    clause.TypeDefinitionId,
                    clause.BrowsePath,
                    clause.AttributeId,
                    clause.ParsedIndexRange);

                // add the value to the list of event fields.
                if (value != null)
                {
                    // translate any localized text.
                    var text = value as LocalizedText;

                    if (text != null)
                    {
                        value = server_.ResourceManager.Translate(Session?.PreferredLocales, text);
                    }

                    // add value.
                    fields.EventFields.Add(new Variant(value));
                }

                // add a dummy entry for missing values.
                else
                {
                    fields.EventFields.Add(Variant.Null);
                }
            }

            return fields;
        }

        /// <summary>
        /// Adds an event to the queue.
        /// </summary>
        public virtual void QueueEvent(IFilterTarget instance)
        {
            QueueEvent(instance, false);
        }

        /// <summary>
        /// Adds an event to the queue.
        /// </summary>
        public virtual void QueueEvent(IFilterTarget instance, bool bypassFilter)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            lock (lock_)
            {
                // this method should only be called for objects or views.
                if ((MonitoredItemType & UaMonitoredItemTypeMask.Events) == 0)
                {
                    throw new ServiceResultException(StatusCodes.BadInternalError);
                }

                // can't do anything if queuing is disabled.
                if (events_ == null)
                {
                    return;
                }

                // check for duplicate instances being reported via multiple paths.
                foreach (var eventField in events_)
                {
                    if (eventField is EventFieldList processedEvent)
                    {
                        if (ReferenceEquals(instance, processedEvent.Handle))
                        {
                            return;
                        }
                    }
                }

                // check for space in the queue.
                if (events_.Count >= queueSize_)
                {
                    if (!discardOldest_)
                    {
                        overflow_ = true;
                        return;
                    }
                }

                // construct the context to use for the event filter.
                var context = new FilterContext(server_.NamespaceUris, server_.TypeTree, Session?.PreferredLocales);

                // event filter must be specified.
                var filter = filterToUse_ as EventFilter;

                if (filter == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInternalError);
                }

                // apply filter.
                if (!bypassFilter)
                {
                    if (!filter.WhereClause.Evaluate(context, instance))
                    {
                        return;
                    }
                }

                // fetch the event fields.
                var fields = GetEventFields(context, filter, instance);
                QueueEvent(fields);
            }
        }

        /// <summary>
        /// Adds an event to the queue.
        /// </summary>
        public virtual void QueueEvent(EventFieldList fields)
        {
            lock (lock_)
            {
                // make space in the queue.
                if (events_.Count >= queueSize_)
                {
                    overflow_ = true;

                    if (discardOldest_)
                    {
                        events_.RemoveAt(0);
                    }
                }

                // queue the event.
                events_.Add(fields);
                readyToPublish_ = true;
                readyToTrigger_ = true;
            }
        }

        /// <summary>
        /// Used to check whether the item is ready to sample.
        /// </summary>
        public bool SamplingIntervalExpired()
        {
            lock (lock_)
            {
                return TimeToNextSample <= 0;
            }
        }

        /// <summary>
        /// Increments the sample time to the next interval.
        /// </summary>
        private void IncrementSampleTime()
        {
            // update next sample time.
            var now = HiResClock.TickCount64;
            var samplingInterval = (long)(samplingInterval_);

            if (nextSamplingTime_ > 0)
            {
                var delta = now - nextSamplingTime_;

                if (samplingInterval > 0 && delta >= 0)
                {
                    nextSamplingTime_ += ((delta / samplingInterval) + 1) * samplingInterval;
                }
            }

            // set sampling time based on current time.
            else
            {
                nextSamplingTime_ = now + samplingInterval;
            }
        }

        /// <summary>
        /// Publishes all available event notifications.
        /// </summary>
        public virtual bool Publish(UaServerOperationContext context, Queue<EventFieldList> notifications)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (notifications == null) throw new ArgumentNullException(nameof(notifications));

            lock (lock_)
            {
                // check if the item reports events.
                if ((MonitoredItemType & UaMonitoredItemTypeMask.Events) == 0)
                {
                    return false;
                }

                // only publish if reporting.
                if (!IsReadyToPublish)
                {
                    return false;
                }

                // go to the next sampling interval.
                IncrementSampleTime();

                // publish events.
                if (events_ != null)
                {
                    Utils.LogTrace(Utils.TraceMasks.OperationDetail, "MONITORED ITEM: Publish(QueueSize={0})", notifications.Count);

                    EventFieldList overflowEvent = null;

                    if (overflow_)
                    {
                        // construct event.
                        var e = new EventQueueOverflowEventState(null);

                        var message = new TranslationInfo(
                            "EventQueueOverflowEventState",
                            "en-US",
                            "Events lost due to queue overflow.");

                        ISystemContext systemContext = new UaServerContext(server_, context);

                        e.Initialize(
                            systemContext,
                            null,
                            EventSeverity.Low,
                            new LocalizedText(message));

                        e.SetChildValue(systemContext, BrowseNames.SourceNode, ObjectIds.Server, false);
                        e.SetChildValue(systemContext, BrowseNames.SourceName, "Internal", false);

                        // fetch the event fields.
                        overflowEvent = GetEventFields(
                            new FilterContext(server_.NamespaceUris, server_.TypeTree, Session.PreferredLocales),
                            filterToUse_ as EventFilter,
                            e);
                    }

                    // place event at the beginning of the queue.
                    if (overflowEvent != null && discardOldest_)
                    {
                        notifications.Enqueue(overflowEvent);
                    }

                    for (int ii = 0; ii < events_.Count; ii++)
                    {
                        EventFieldList fields = (EventFieldList)events_[ii];

                        // apply any diagnostic masks.
                        for (int jj = 0; jj < fields.EventFields.Count; jj++)
                        {
                            object value = fields.EventFields[jj].Value;

                            StatusResult result = value as StatusResult;

                            if (result != null)
                            {
                                result.ApplyDiagnosticMasks(context.DiagnosticsMask, context.StringTable);
                            }
                        }

                        notifications.Enqueue(events_[ii]);
                    }

                    events_.Clear();

                    // place event at the end of the queue.
                    if (overflowEvent != null && !discardOldest_)
                    {
                        notifications.Enqueue(overflowEvent);
                    }

                    Utils.LogTrace(Utils.TraceMasks.OperationDetail, "MONITORED ITEM: Publish(QueueSize={0})", notifications.Count);
                }

                // reset state variables.
                overflow_ = false;
                readyToPublish_ = false;
                readyToTrigger_ = false;
                triggered_ = false;

                return false;
            }
        }

        /// <summary>
        /// Publishes all available data change notifications.
        /// </summary>
        public virtual bool Publish(
            UaServerOperationContext context,
            Queue<MonitoredItemNotification> notifications,
            Queue<DiagnosticInfo> diagnostics)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (notifications == null) throw new ArgumentNullException(nameof(notifications));
            if (diagnostics == null) throw new ArgumentNullException(nameof(diagnostics));

            lock (lock_)
            {
                // check if the item reports data changes.
                if ((MonitoredItemType & UaMonitoredItemTypeMask.DataChange) == 0)
                {
                    return false;
                }

                if (!IsReadyToPublish)
                {
                    if (!resendData_)
                    {
                        return false;
                    }
                }
                else
                {
                    // pull any unprocessed data.
                    if (calculator_ != null)
                    {
                        if (calculator_.HasEndTimePassed(DateTime.UtcNow))
                        {
                            var processedValue = calculator_.GetProcessedValue(false);

                            while (processedValue != null)
                            {
                                AddValueToQueue(processedValue, null);
                            }

                            processedValue = calculator_.GetProcessedValue(true);
                            AddValueToQueue(processedValue, null);
                        }
                    }

                    IncrementSampleTime();
                }

                readyToPublish_ = false;

                // check if queueing enabled.
                if (queue_ != null && (!resendData_ || queue_.ItemsInQueue != 0))
                {
                    DataValue value = null;
                    ServiceResult error = null;

                    while (queue_.Publish(out value, out error))
                    {
                        Publish(context, notifications, diagnostics, value, error);
                        if (resendData_)
                        {
                            readyToPublish_ = queue_.ItemsInQueue > 0;
                            break;
                        }
                    }
                }

                // publish last value if no queuing or no items are queued
                else
                {
                    UaServerUtils.EventLog.DequeueValue(lastValue_.WrappedValue, lastValue_.StatusCode);
                    Publish(context, notifications, diagnostics, lastValue_, lastError_);
                }

                // reset state variables.
                overflow_ = false;
                readyToTrigger_ = false;
                resendData_ = false;
                triggered_ = false;

                return false;
            }
        }

        /// <summary>
        /// Publishes a single data change notifications.
        /// </summary>
        protected virtual bool Publish(
            UaServerOperationContext context,
            Queue<MonitoredItemNotification> notifications,
            Queue<DiagnosticInfo> diagnostics,
            DataValue value,
            ServiceResult error)
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
            var item = new MonitoredItemNotification { ClientHandle = clientHandle_, Value = value };


            // apply timestamp filter.
            if (timestampsToReturn_ != TimestampsToReturn.Server && timestampsToReturn_ != TimestampsToReturn.Both)
            {
                item.Value.ServerTimestamp = DateTime.MinValue;
            }

            if (timestampsToReturn_ != TimestampsToReturn.Source && timestampsToReturn_ != TimestampsToReturn.Both)
            {
                item.Value.SourceTimestamp = DateTime.MinValue;
            }

            UaServerUtils.ReportPublishValue(nodeId_, id_, item.Value);
            notifications.Enqueue(item);

            // update diagnostic info.
            DiagnosticInfo diagnosticInfo = null;

            if ((diagnosticsMasks_ & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
            }

            diagnostics.Enqueue(diagnosticInfo);

            return false;
        }

        /// <summary>
        /// The object to call when item is ready to publish.
        /// </summary>
        public IUaSubscription SubscriptionCallback
        {
            get
            {
                lock (lock_)
                {
                    return subscription_;
                }
            }

            set
            {
                lock (lock_)
                {
                    subscription_ = value;
                }
            }
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

                    // node manager responsible for ensuring correct sampling.
                    if (sourceSamplingInterval_ > 0)
                    {
                        return 0;
                    }

                    var now = HiResClock.TickCount64;

                    if (nextSamplingTime_ <= now)
                    {
                        return 0;
                    }

                    return (int)(nextSamplingTime_ - now);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Applies the filter to value to determine if the new value should be kept.
        /// </summary>
        protected virtual bool ApplyFilter(DataValue value, ServiceResult error)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var changed = ValueChanged(
                value,
                error,
                lastValue_,
                lastError_,
                filterToUse_ as DataChangeFilter,
                range_);

            return changed;
        }

        /// <summary>
        /// Applies the filter to value to determine if the new value should be kept.
        /// </summary>
        public static bool ValueChanged(
            DataValue value,
            ServiceResult error,
            DataValue lastValue,
            ServiceResult lastError,
            DataChangeFilter filter,
            double range)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // select default data change filters.
            var deadband = 0.0;
            var deadbandType = DeadbandType.None;
            var trigger = DataChangeTrigger.StatusValue;

            // apply filter.
            if (filter != null)
            {
                trigger = filter.Trigger;
                deadbandType = (DeadbandType)(int)filter.DeadbandType;
                deadband = filter.DeadbandValue;

                // when deadband is used and the trigger is StatusValueTimestamp, then it should behave as if trigger is StatusValue.
                if ((deadbandType != DeadbandType.None) && (trigger == DataChangeTrigger.StatusValueTimestamp))
                {
                    trigger = DataChangeTrigger.StatusValue;
                }
            }

            // get the current status.
            var status = StatusCodes.Good;

            if (error != null)
            {
                status = error.StatusCode.Code;
            }
            else if (lastValue != null)
            {
                status = value.StatusCode.Code;
            }

            // get the last status.
            var lastStatus = StatusCodes.Good;

            if (lastError != null)
            {
                lastStatus = lastError.StatusCode.Code;
            }
            else if (lastValue != null)
            {
                lastStatus = lastValue.StatusCode.Code;
            }

            // value changed if any status change occur.
            if (status != lastStatus)
            {
                return true;
            }

            // value changed if only one is null.
            if (value == null || lastValue == null)
            {
                return lastValue != null || value != null;
            }

            // check if timestamp has changed.
            if (trigger == DataChangeTrigger.StatusValueTimestamp)
            {
                if (lastValue.SourceTimestamp != value.SourceTimestamp)
                {
                    return true;
                }
            }

            // check if value changes are ignored.
            if (trigger == DataChangeTrigger.Status)
            {
                return false;
            }

            // check if reference to same object.
            if (!Equals(lastValue.Value, value.Value, deadbandType, deadband, range))
            {
                return true;
            }

            // must be equal.
            return false;
        }

        /// <summary>
        /// Checks if the two values are equal.
        /// </summary>
        protected static bool Equals(object value1, object value2, DeadbandType deadbandType, double deadband, double range)
        {
            // check if reference to same object.
            if (ReferenceEquals(value1, value2))
            {
                return true;
            }

            // check for invalid values.
            if (value1 == null || value2 == null)
            {
                return value1 == value2;
            }

            // check for type change.
            if (value1.GetType() != value2.GetType())
            {
                return false;
            }

            // special case NaN is always not equal
            if (value1.Equals(float.NaN) ||
                value1.Equals(double.NaN) ||
                value2.Equals(float.NaN) ||
                value2.Equals(double.NaN))
            {
                return false;
            }

            // check if values are equal.
            if (value1.Equals(value2))
            {
                return true;
            }

            // check for arrays.
            var array1 = value1 as Array;
            var array2 = value2 as Array;

            if (array1 == null || array2 == null)
            {

                XmlElement xmlElement1 = value1 as XmlElement;
                XmlElement xmlElement2 = value2 as XmlElement;

                if (xmlElement1 != null && xmlElement2 != null)
                {
                    return xmlElement1.OuterXml.Equals(xmlElement2.OuterXml);
                }

                // nothing more to do if no deadband.
                if (deadbandType == DeadbandType.None)
                {
                    return false;
                }

                // check deadband.
                return !ExceedsDeadband(value1, value2, deadbandType, deadband, range);
            }

            // compare lengths.
            if (array1.Length != array2.Length)
            {
                return false;
            }

            // compare each element.
            var isVariant = array1.GetType().GetElementType() == typeof(Variant);

            for (var ii = 0; ii < array1.Length; ii++)
            {
                var element1 = array1.GetValue(ii);
                var element2 = array2.GetValue(ii);

                if (isVariant)
                {
                    element1 = ((Variant)element1).Value;
                    element2 = ((Variant)element2).Value;
                }

                if (!Equals(element1, element2, deadbandType, deadband, range))
                {
                    return false;
                }
            }

            // must be equal.
            return true;
        }

        /// <summary>
        /// Returns true if the deadband was exceeded.
        /// </summary>
        protected static bool ExceedsDeadband(object value1, object value2, DeadbandType deadbandType, double deadband, double range)
        {
            // cannot convert doubles safely to decimals.
            if (value1 is double)
            {
                return ExceedsDeadband((double)value1, (double)value2, deadbandType, deadband, range);
            }

            try
            {
                var decimal1 = Convert.ToDecimal(value1, CultureInfo.InvariantCulture);
                var decimal2 = Convert.ToDecimal(value2, CultureInfo.InvariantCulture);
                decimal baseline = 1;

                if (deadbandType == DeadbandType.Percent)
                {
                    baseline = ((decimal)range) / 100;
                }

                if (baseline > 0)
                {
                    if (Math.Abs((decimal1 - decimal2) / baseline) <= (decimal)deadband)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                // treat all conversion errors as evidence that the deadband was exceeded.
            }

            return true;
        }

        /// <summary>
        /// Returns true if the deadband was exceeded.
        /// </summary>
        private static bool ExceedsDeadband(double value1, double value2, DeadbandType deadbandType, double deadband, double range)
        {
            double baseline = 1;

            if (deadbandType == DeadbandType.Percent)
            {
                baseline = range / 100;
            }

            if (baseline > 0)
            {
                if (Math.Abs((value1 - value2) / baseline) <= deadband)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears and re-initializes the queue if the monitoring parameters changed.
        /// </summary>
        protected void InitializeQueue()
        {
            switch (monitoringMode_)
            {
                default:
                case MonitoringMode.Disabled:
                    {
                        queue_ = null;
                        events_ = null;
                        break;
                    }

                case MonitoringMode.Reporting:
                case MonitoringMode.Sampling:
                    {
                        // check if queuing is disabled.
                        if (queueSize_ == 0)
                        {
                            if (MonitoredItemType == UaMonitoredItemTypeMask.DataChange)
                            {
                                queueSize_ = 1;
                            }

                            if ((MonitoredItemType & UaMonitoredItemTypeMask.Events) != 0)
                            {
                                queueSize_ = 1000;
                            }
                        }

                        // create data queue.
                        if (MonitoredItemType == UaMonitoredItemTypeMask.DataChange)
                        {
                            if (queueSize_ <= 1)
                            {
                                queue_ = null;
                                break; // queueing is disabled
                            }

                            var queueLastValue = false;

                            if (queue_ == null)
                            {
                                queue_ = new MonitoredItemQueue(id_, QueueOverflowHandler);
                                queueLastValue = true;
                            }

                            queue_.SetQueueSize(queueSize_, discardOldest_, diagnosticsMasks_);
                            queue_.SetSamplingInterval(samplingInterval_);

                            if (queueLastValue && lastValue_ != null)
                            {
                                queue_.QueueValue(lastValue_, lastError_);
                            }
                        }
                        else // create event queue.
                        {
                            if (events_ == null)
                            {
                                events_ = new List<EventFieldList>();
                            }

                            // check if existing queue entries must be discarded;
                            if (events_.Count > queueSize_)
                            {
                                var queueSize = (int)queueSize_;

                                if (discardOldest_)
                                {
                                    events_.RemoveRange(0, events_.Count - queueSize);
                                }
                                else
                                {
                                    events_.RemoveRange(queueSize, events_.Count - queueSize);
                                }
                            }
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Update the overflow count.
        /// </summary>
        private void QueueOverflowHandler()
        {
            subscription_?.QueueOverflowHandler();
        }
        #endregion

        #region Private Members
        private object lock_ = new object();
        private IUaServerData server_;
        private uint id_;
        private NodeId nodeId_;
        private uint attributeId_;
        private string indexRange_;
        private NumericRange parsedIndexRange_;
        private QualifiedName encoding_;
        private DiagnosticsMasks diagnosticsMasks_;
        private TimestampsToReturn timestampsToReturn_;
        private uint clientHandle_;
        private MonitoringMode monitoringMode_;
        private MonitoringFilter originalFilter_;
        private MonitoringFilter filterToUse_;
        private double range_;
        private double samplingInterval_;
        private uint queueSize_;
        private bool discardOldest_;
        private int sourceSamplingInterval_;
        private bool alwaysReportUpdates_;

        private DataValue lastValue_;
        private ServiceResult lastError_;
        private long nextSamplingTime_;
        private List<EventFieldList> events_;
        private MonitoredItemQueue queue_;
        private bool overflow_;
        private bool readyToPublish_;
        private bool readyToTrigger_;
        private bool semanticsChanged_;
        private bool structureChanged_;
        private IUaSubscription subscription_;
        private ServiceResult samplingError_;
        private IUaAggregateCalculator calculator_;
        private bool triggered_;
        private bool resendData_;
        #endregion
    }
}
