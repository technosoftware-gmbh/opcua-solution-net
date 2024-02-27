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
using System.Collections.Generic;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Manages a monitored item created by a client.
    /// </summary>
    public interface IUaMonitoredItem
    {
        /// <summary>
        /// The node manager that created the item.
        /// </summary>
        IUaBaseNodeManager NodeManager { get; }

        /// <summary>
        /// The session that owns the monitored item.
        /// </summary>
        Sessions.Session Session { get; }

        /// <summary>
        /// The identifier for the item that is unique within the server.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// The identifier for the subscription that is unique within the server.
        /// </summary>
        uint SubscriptionId { get; }

        /// <summary>
        /// The identifier for the client handle assigned to the monitored item.
        /// </summary>
        uint ClientHandle { get; }

        /// <summary>
        /// The object to call when item is ready to publish.
        /// </summary>
        IUaSubscription SubscriptionCallback { get; set; }

        /// <summary>
        /// The handle assigned by the NodeManager.
        /// </summary>
        object ManagerHandle { get; }

        /// <summary>
        /// A bit mask that indicates what the monitored item is.
        /// </summary>
        /// <remarks>
        /// Predefined bits are defined by the MonitoredItemTypeMasks class.
        /// NodeManagers may use the remaining bits.
        /// </remarks>
        int MonitoredItemType { get; }

        /// <summary>
        /// Checks if the monitored item is ready to publish.
        /// </summary>
        bool IsReadyToPublish { get; }

        /// <summary>
        /// Gets or Sets a value indicating whether the monitored item is ready to trigger the linked items.
        /// </summary>
        bool IsReadyToTrigger { get; set; }

        /// <summary>
        /// Gets a value indicating whether the monitored item is resending data.
        /// </summary>
        bool IsResendData { get; }

        /// <summary>
        /// Set the resend data trigger flag.
        /// </summary>
        void SetupResendDataTrigger();

        /// <summary>
        /// Returns the result after creating the monitor item.
        /// </summary>
        ServiceResult GetCreateResult(out MonitoredItemCreateResult result);

        /// <summary>
        /// Returns the result after modifying the monitor item.
        /// </summary>
        ServiceResult GetModifyResult(out MonitoredItemModifyResult result);

        /// <summary>
        /// The monitoring mode specified for the item.
        /// </summary>
        MonitoringMode MonitoringMode { get; }

        /// <summary>
        /// The sampling interval for the item.
        /// </summary>
        double SamplingInterval { get; }
    }

    /// <summary>
    /// A monitored item that can be triggered.
    /// </summary>
    public interface IUaTriggeredMonitoredItem
    {
        /// <summary>
        /// The identifier for the item that is unique within the server.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Flags the monitored item as triggered.
        /// </summary>
        /// <returns>True if there is something to publish.</returns>
        bool SetTriggered();
    }

    /// <summary>
    /// Manages a monitored item created by a client.
    /// </summary>
    public interface IUaEventMonitoredItem : IUaMonitoredItem
    {
        /// <summary>
        /// Whether the item is monitoring all events produced by the server.
        /// </summary>
        bool MonitoringAllEvents { get; }

        /// <summary>
        /// Adds an event to the queue.
        /// </summary>
        void QueueEvent(IFilterTarget instance);

        /// <summary>
        /// The filter used by the monitored item.
        /// </summary>
        EventFilter EventFilter { get; }

        /// <summary>
        /// Publishes all available event notifications.
        /// </summary>
        /// <returns>True if the caller should re-queue the item for publishing after the next interval elapses.</returns>
        bool Publish(UaServerOperationContext context, Queue<EventFieldList> notifications);

        /// <summary>
        /// Modifies the attributes for monitored item.
        /// </summary>
        ServiceResult ModifyAttributes(
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            uint clientHandle,
            MonitoringFilter originalFilter,
            MonitoringFilter filterToUse,
            Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest);

        /// <summary>
        /// Changes the monitoring mode for the item.
        /// </summary>
        void SetMonitoringMode(MonitoringMode monitoringMode);
    }

    /// <summary>
    /// Manages a monitored item created by a client.
    /// </summary>
    public interface IUaDataChangeMonitoredItem : IUaMonitoredItem
    {
        /// <summary>
        /// Updates the queue with a data value or an error.
        /// </summary>
        void QueueValue(DataValue value, ServiceResult error);

        /// <summary>
        /// The filter used by the monitored item.
        /// </summary>
        DataChangeFilter DataChangeFilter { get; }

        /// <summary>
        /// Publishes all available data change notifications.
        /// </summary>
        /// <returns>True if the caller should re-queue the item for publishing after the next interval elapses.</returns>
        bool Publish(
            UaServerOperationContext context,
            Queue<MonitoredItemNotification> notifications,
            Queue<DiagnosticInfo> diagnostics);
    }

    /// <summary>
    /// Manages a monitored item created by a client.
    /// </summary>
    public interface IUaDataChangeMonitoredItem2 : IUaDataChangeMonitoredItem
    {
        /// <summary>
        /// The attribute being monitored.
        /// </summary>
        uint AttributeId { get; }

        /// <summary>
        /// The index range requested by the monitored item.
        /// </summary>
        NumericRange IndexRange { get; }

        /// <summary>
        /// The data encoding requested by the monitored item.
        /// </summary>
        QualifiedName DataEncoding { get; }

        /// <summary>
        /// Updates the queue with a data value or an error.
        /// </summary>
        void QueueValue(DataValue value, ServiceResult error, bool ignoreFilters);
    }

    /// <summary>
    /// Manages a monitored item created by a client.
    /// </summary>
    public interface IUaSampledDataChangeMonitoredItem : IUaDataChangeMonitoredItem2
    {
        /// <summary>
        /// The diagnostics mask specified fro the monitored item.
        /// </summary>
        DiagnosticsMasks DiagnosticsMasks { get; }

        /// <summary>
        /// The queue size for the item.
        /// </summary>
        uint QueueSize { get; }

        /// <summary>
        /// The minimum sampling interval for the item.
        /// </summary>
        double MinimumSamplingInterval { get; }

        /// <summary>
        /// Used to check whether the item is ready to sample.
        /// </summary>
        bool SamplingIntervalExpired();

        /// <summary>
        /// Returns the parameters that can be used to read the monitored item.
        /// </summary>
        ReadValueId GetReadValueId();

        /// <summary>
        /// Modifies the attributes for monitored item.
        /// </summary>
        ServiceResult ModifyAttributes(
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            uint clientHandle,
            MonitoringFilter originalFilter,
            MonitoringFilter filterToUse,
            Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest);

        /// <summary>
		/// Changes the monitoring mode for the item.
		/// </summary>
        void SetMonitoringMode(MonitoringMode monitoringMode);

        /// <summary>
        /// Updates the sampling interval for an item.
        /// </summary>
        void SetSamplingInterval(double samplingInterval);
    }

    /// <summary>
    /// Defines constants for the monitored item type.
    /// </summary>
    /// <remarks>
    /// Bits 1-8 are reserved for internal use. NodeManagers may use other bits.
    /// </remarks>
    public static class UaMonitoredItemTypeMask
    {
        /// <summary>
        /// The monitored item subscribes to data changes.
        /// </summary>
        public const int DataChange = 0x1;

        /// <summary>
        /// The monitored item subscribes to events.
        /// </summary>
        public const int Events = 0x2;

        /// <summary>
        /// The monitored item subscribes to all events produced by the server.
        /// </summary>
        /// <remarks>
        /// If this bit is set the Events bit must be set too.
        /// </remarks>
        public const int AllEvents = 0x4;
    }
}
