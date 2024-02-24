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
using System.Runtime.Serialization;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// A monitored item.
    /// </summary>
    [DataContract(Namespace = Namespaces.OpcUaXsd)]
    [KnownType(typeof(DataChangeFilter))]
    [KnownType(typeof(EventFilter))]
    [KnownType(typeof(AggregateFilter))]
    public class MonitoredItem : ICloneable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredItem"/> class.
        /// </summary>
        public MonitoredItem()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredItem"/> class.
        /// </summary>
        /// <param name="clientHandle">The client handle. The caller must ensure it uniquely identifies the monitored item.</param>
        public MonitoredItem(uint clientHandle)
        {
            Initialize();
            ClientHandle = clientHandle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredItem"/> class.
        /// </summary>
        /// <param name="template">The template used to specify the monitoring parameters.</param>
        public MonitoredItem(MonitoredItem template) : this(template, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredItem"/> class.
        /// </summary>
        /// <param name="template">The template used to specify the monitoring parameters.</param>
        /// <param name="copyEventHandlers">if set to <c>true</c> the event handlers are copied.</param>
        public MonitoredItem(MonitoredItem template, bool copyEventHandlers) : this(template, copyEventHandlers, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredItem"/> class.
        /// </summary>
        /// <param name="template">The template used to specify the monitoring parameters.</param>
        /// <param name="copyEventHandlers">if set to <c>true</c> the event handlers are copied.</param>
        /// <param name="copyClientHandle">if set to <c>true</c> the clientHandle is of the template copied.</param>
        public MonitoredItem(MonitoredItem template, bool copyEventHandlers, bool copyClientHandle)
        {
            Initialize();

            if (template != null)
            {
                var displayName = template.DisplayName;

                if (displayName != null)
                {
                    // remove any existing numeric suffix.
                    var index = displayName.LastIndexOf(' ');

                    if (index != -1)
                    {
                        try
                        {
                            displayName = displayName.Substring(0, index);
                        }
                        catch
                        {
                            // not a numeric suffix.
                        }
                    }
                }

                Handle = template.Handle;
                DisplayName = Utils.Format("{0} {1}", displayName, ClientHandle);
                StartNodeId = template.StartNodeId;
                relativePath_ = template.relativePath_;
                AttributeId = template.AttributeId;
                IndexRange = template.IndexRange;
                Encoding = template.Encoding;
                MonitoringMode = template.MonitoringMode;
                samplingInterval_ = template.samplingInterval_;
                filter_ = (MonitoringFilter)Utils.Clone(template.filter_);
                queueSize_ = template.queueSize_;
                discardOldest_ = template.discardOldest_;
                AttributesModified = true;

                if (copyEventHandlers)
                {
                    NotificationEventHandler = template.NotificationEventHandler;
                }

                if (copyClientHandle)
                {
                    ClientHandle = template.ClientHandle;
                }

                // this ensures the state is consistent with the node class.
                NodeClass = template.nodeClass_;
            }
        }

        /// <summary>
        /// Called by the .NET framework during deserialization.
        /// </summary>
        [OnDeserializing]
        protected void Initialize(StreamingContext context)
        {
            // object initializers are not called during deserialization.
            cache_ = new object();

            Initialize();
        }

        /// <summary>
        /// Sets the private members to default values.
        /// </summary>
        private void Initialize()
        {
            StartNodeId = null;
            relativePath_ = null;
            ClientHandle = 0;
            AttributeId = Attributes.Value;
            IndexRange = null;
            Encoding = null;
            MonitoringMode = MonitoringMode.Reporting;
            samplingInterval_ = -1;
            filter_ = null;
            queueSize_ = 0;
            discardOldest_ = true;
            AttributesModified = true;
            Status = new MonitoredItemStatus();

            // this ensures the state is consistent with the node class.
            NodeClass = NodeClass.Variable;

            // assign a unique handle.
            ClientHandle = Utils.IncrementIdentifier(ref globalClientHandle_);
        }
        #endregion

        #region Persistent Properties
        /// <summary>
        /// A display name for the monitored item.
        /// </summary>
        [DataMember(Order = 1)]
        public string DisplayName { get; set; }

        /// <summary>
        /// The start node for the browse path that identifies the node to monitor.
        /// </summary>
        [DataMember(Order = 2)]
        public NodeId StartNodeId { get; set; }

        /// <summary>
        /// The relative path from the browse path to the node to monitor.
        /// </summary>
        /// <remarks>
        /// A null or empty string specifies that the start node id should be monitored.
        /// </remarks>
        [DataMember(Order = 3)]
        public string RelativePath
        {
            get => relativePath_;

            set
            {
                // clear resolved path if relative path has changed.
                if (relativePath_ != value)
                {
                    resolvedNodeId_ = null;
                }

                relativePath_ = value;
            }
        }

        /// <summary>
        /// The node class of the node being monitored (affects the type of filter available).
        /// </summary>
        [DataMember(Order = 4)]
        public NodeClass NodeClass
        {
            get => nodeClass_;

            set
            {
                if (nodeClass_ != value)
                {
                    if ((value & (NodeClass.Object | NodeClass.View)) != 0)
                    {
                        // ensure a valid event filter.
                        if (!(filter_ is EventFilter))
                        {
                            UseDefaultEventFilter();
                        }

                        // set the queue size to the default for events.
                        if (QueueSize <= 1)
                        {
                            QueueSize = Int32.MaxValue;
                        }

                        eventCache_ = new MonitoredItemEventCache(100);
                        AttributeId = Attributes.EventNotifier;
                    }
                    else
                    {
                        // clear the filter if it is only valid for events.
                        if (filter_ is EventFilter)
                        {
                            filter_ = null;
                        }

                        // set the queue size to the default for data changes.
                        if (QueueSize == Int32.MaxValue)
                        {
                            QueueSize = 1;
                        }

                        dataCache_ = new MonitoredItemDataCache(1);
                    }
                }

                nodeClass_ = value;
            }
        }

        /// <summary>
        /// The attribute to monitor.
        /// </summary>
        [DataMember(Order = 5)]
        public uint AttributeId { get; set; }

        /// <summary>
        /// The range of array indexes to monitor.
        /// </summary>
        [DataMember(Order = 6)]
        public string IndexRange { get; set; }

        /// <summary>
        /// The encoding to use when returning notifications.
        /// </summary>
        [DataMember(Order = 7)]
        public QualifiedName Encoding { get; set; }

        /// <summary>
        /// The monitoring mode.
        /// </summary>
        [DataMember(Order = 8)]
        public MonitoringMode MonitoringMode { get; set; }

        /// <summary>
        /// The sampling interval.
        /// </summary>
        [DataMember(Order = 9)]
        public int SamplingInterval
        {
            get => samplingInterval_;

            set
            {
                if (samplingInterval_ != value)
                {
                    AttributesModified = true;
                }

                samplingInterval_ = value;
            }
        }

        /// <summary>
        /// The filter to use to select values to return.
        /// </summary>
        [DataMember(Order = 10)]
        public MonitoringFilter Filter
        {
            get => filter_;

            set
            {
                // validate filter against node class.
                ValidateFilter(nodeClass_, value);

                AttributesModified = true;
                filter_ = value;
            }
        }

        /// <summary>
        /// The length of the queue used to buffer values.
        /// </summary>
        [DataMember(Order = 11)]
        public uint QueueSize
        {
            get => queueSize_;

            set
            {
                if (queueSize_ != value)
                {
                    AttributesModified = true;
                }

                queueSize_ = value;
            }
        }

        /// <summary>
        /// Whether to discard the oldest entries in the queue when it is full.
        /// </summary>
        [DataMember(Order = 12)]
        public bool DiscardOldest
        {
            get => discardOldest_;

            set
            {
                if (discardOldest_ != value)
                {
                    AttributesModified = true;
                }

                discardOldest_ = value;
            }
        }

        /// <summary>
        /// Server-assigned id for the MonitoredItem.
        /// </summary>
        [DataMember(Order = 13)]
        public uint ServerId
        {
            get => Status.Id;
            set => Status.Id = value;
        }
        #endregion

        #region Dynamic Properties
        /// <summary>
        /// The subscription that owns the monitored item.
        /// </summary>
        public Subscription Subscription { get; internal set; }

        /// <summary>
        /// A local handle assigned to the monitored item.
        /// </summary>
        public object Handle { get; set; }

        /// <summary>
        /// Whether the item has been created on the server.
        /// </summary>
        public bool Created => Status.Created;

        /// <summary>
        /// The identifier assigned by the client.
        /// </summary>
        public uint ClientHandle { get; private set; }

        /// <summary>
        /// The node id to monitor after applying any relative path.
        /// </summary>
        public NodeId ResolvedNodeId
        {
            get =>
                // just return the start id if relative path is empty.
                string.IsNullOrEmpty(relativePath_) ? StartNodeId : resolvedNodeId_;

            internal set => resolvedNodeId_ = value;
        }

        /// <summary>
        /// Whether the monitoring attributes have been modified since the item was created.
        /// </summary>
        public bool AttributesModified { get; private set; }

        /// <summary>
        /// The status associated with the monitored item.
        /// </summary>
        public MonitoredItemStatus Status { get; private set; }

        #endregion

        #region Cache Related Functions
        /// <summary>
        /// Returns the queue size used by the cache.
        /// </summary>
        public int CacheQueueSize
        {
            get
            {
                lock (cache_)
                {
                    return dataCache_ != null ? dataCache_.QueueSize : eventCache_ != null ? eventCache_.QueueSize : 0;
                }
            }

            set
            {
                lock (cache_)
                {
                    dataCache_?.SetQueueSize(value);
                    eventCache_?.SetQueueSize(value);
                }
            }
        }

        /// <summary>
        /// The last value or event received from the server.
        /// </summary>
        public IEncodeable LastValue
        {
            get
            {
                lock (cache_)
                {
                    return lastNotification_;
                }
            }
        }

        /// <summary>
        /// Read all values in the cache queue.
        /// </summary>
        public IList<DataValue> DequeueValues()
        {
            lock (cache_)
            {
                return dataCache_ != null ? dataCache_.Publish() : new List<DataValue>();
            }
        }

        /// <summary>
        /// Read all events in the cache queue.
        /// </summary>
        public IList<EventFieldList> DequeueEvents()
        {
            lock (cache_)
            {
                return eventCache_ != null ? eventCache_.Publish() : new List<EventFieldList>();
            }
        }

        /// <summary>
        /// The last message containing a notification for the item.
        /// </summary>
        public NotificationMessage LastMessage
        {
            get
            {
                lock (cache_)
                {
                    return dataCache_ != null
                        ? ((MonitoredItemNotification)lastNotification_).Message
                        : eventCache_ != null ? ((EventFieldList)lastNotification_).Message : null;
                }
            }
        }

        /// <summary>
        /// Raised when a new notification arrives.
        /// </summary>
        public event EventHandler<MonitoredItemNotificationEventArgs> MonitoredItemNotificationEvent
        {
            add
            {
                lock (cache_)
                {
                    NotificationEventHandler += value;
                }
            }

            remove
            {
                lock (cache_)
                {
                    NotificationEventHandler -= value;
                }
            }
        }

        /// <summary>
        /// Reset the notification event handler.
        /// </summary>
        public void DetachNotificationEventHandlers()
        {
            lock (cache_)
            {
                NotificationEventHandler = null;
            }
        }

        /// <summary>
        /// Saves a data change or event in the cache.
        /// </summary>
        public void SaveValueInCache(IEncodeable newValue)
        {
            lock (cache_)
            {
                // only validate timestamp on first sample
                var validateTimestamp = lastNotification_ == null;

                lastNotification_ = newValue;

                if (dataCache_ != null)
                {
                    if (newValue is MonitoredItemNotification datachange)
                    {
                        if (datachange.Value != null)
                        {
                            if (validateTimestamp)
                            {
                                DateTime now = DateTime.UtcNow;

                                // validate the ServerTimestamp of the notification.
                                if (datachange.Value.ServerTimestamp > now)
                                {
                                    Utils.LogWarning("Received ServerTimestamp {0} is in the future for MonitoredItemId {1}",
                                        datachange.Value.ServerTimestamp.ToLocalTime(), ClientHandle);
                                }

                                // validate SourceTimestamp of the notification.
                                if (datachange.Value.SourceTimestamp > now)
                                {
                                    Utils.LogWarning("Received SourceTimestamp {0} is in the future for MonitoredItemId {1}",
                                        datachange.Value.SourceTimestamp.ToLocalTime(), ClientHandle);
                                }
                            }

                            if (datachange.Value.StatusCode.Overflow)
                            {
                                Utils.LogWarning("Overflow bit set for data change with ServerTimestamp {0} and value {1} for MonitoredItemId {2}",
                                    datachange.Value.ServerTimestamp.ToLocalTime(), datachange.Value.Value, ClientHandle);
                            }
                        }

                        dataCache_.OnNotification(datachange);
                    }
                }

                if (eventCache_ != null)
                {
                    if (newValue is EventFieldList eventchange)
                    {
                        eventCache_?.OnNotification(eventchange);
                    }
                }

                NotificationEventHandler?.Invoke(this, new MonitoredItemNotificationEventArgs(newValue));
            }
        }
        #endregion

        #region ICloneable Members
        /// <inheritdoc/>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Creates a deep copy of the object.
        /// </summary>
        public new object MemberwiseClone()
        {
            return new MonitoredItem(this);
        }

        /// <summary>
        /// Clones a monitored item or the subclass with an option to copy event handlers.
        /// </summary>
        /// <returns>A cloned instance of the monitored item or a subclass.</returns>
        public virtual MonitoredItem CloneMonitoredItem(bool copyEventHandlers, bool copyClientHandle)
        {
            return new MonitoredItem(this, copyEventHandlers, copyClientHandle);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the error status for the monitored item.
        /// </summary>
        public void SetError(ServiceResult error)
        {
            Status.SetError(error);
        }

        /// <summary>
        /// Updates the object with the results of a translate browse path request.
        /// </summary>
        public void SetResolvePathResult(
            BrowsePathResult result,
            int index,
            DiagnosticInfoCollection diagnosticInfos,
            ResponseHeader responseHeader)
        {
            ServiceResult error = null;

            if (StatusCode.IsBad(result.StatusCode))
            {
                error = ClientBase.GetResult(result.StatusCode, index, diagnosticInfos, responseHeader);
            }
            else
            {
                ResolvedNodeId = NodeId.Null;

                // update the node id.
                if (result.Targets.Count > 0)
                {
                    ResolvedNodeId = ExpandedNodeId.ToNodeId(result.Targets[0].TargetId, Subscription.Session.NamespaceUris);
                }
            }

            Status.SetResolvePathResult(result, error);
        }

        /// <summary>
        /// Updates the object with the results of a create monitored item request.
        /// </summary>
        public void SetCreateResult(
            MonitoredItemCreateRequest request,
            MonitoredItemCreateResult result,
            int index,
            DiagnosticInfoCollection diagnosticInfos,
            ResponseHeader responseHeader)
        {
            ServiceResult error = null;

            if (StatusCode.IsBad(result.StatusCode))
            {
                error = ClientBase.GetResult(result.StatusCode, index, diagnosticInfos, responseHeader);
            }

            Status.SetCreateResult(request, result, error);
            AttributesModified = false;
        }

        /// <summary>
        /// Updates the object with the results of a modify monitored item request.
        /// </summary>
        public void SetModifyResult(
            MonitoredItemModifyRequest request,
            MonitoredItemModifyResult result,
            int index,
            DiagnosticInfoCollection diagnosticInfos,
            ResponseHeader responseHeader)
        {
            ServiceResult error = null;

            if (StatusCode.IsBad(result.StatusCode))
            {
                error = ClientBase.GetResult(result.StatusCode, index, diagnosticInfos, responseHeader);
            }

            Status.SetModifyResult(request, result, error);
            AttributesModified = false;
        }

        /// <summary>
        /// Updates the object with the results of a transfer subscription request.
        /// </summary>
        public void SetTransferResult(uint clientHandle)
        {
            // ensure the global counter is not duplicating future handle ids
            _ = Utils.LowerLimitIdentifier(ref globalClientHandle_, clientHandle);
            ClientHandle = clientHandle;
            Status.SetTransferResult(this);
            AttributesModified = false;
        }

        /// <summary>
        /// Updates the object with the results of a delete monitored item request.
        /// </summary>
        public void SetDeleteResult(
            StatusCode result,
            int index,
            DiagnosticInfoCollection diagnosticInfos,
            ResponseHeader responseHeader)
        {
            ServiceResult error = null;

            if (StatusCode.IsBad(result))
            {
                error = ClientBase.GetResult(result, index, diagnosticInfos, responseHeader);
            }

            Status.SetDeleteResult(error);
        }

        /// <summary>
        /// Returns the field name the specified SelectClause in the EventFilter.
        /// </summary>
        public string GetFieldName(int index)
        {
            return !(filter_ is EventFilter filter)
                ? null
                : index < 0 || index >= filter.SelectClauses.Count
                ? null
                : Utils.Format("{0}", SimpleAttributeOperand.Format(filter.SelectClauses[index].BrowsePath));
        }

        /// <summary>
        /// Returns value of the field name containing the event type.
        /// </summary>
        public object GetFieldValue(
            EventFieldList eventFields,
            NodeId eventTypeId,
            string browsePath,
            uint attributeId)
        {
            QualifiedNameCollection browseNames = SimpleAttributeOperand.Parse(browsePath);
            return GetFieldValue(eventFields, eventTypeId, browseNames, attributeId);
        }

        /// <summary>
        /// Returns value of the field name containing the event type.
        /// </summary>
        public object GetFieldValue(
            EventFieldList eventFields,
            NodeId eventTypeId,
            QualifiedName browseName)
        {
            var browsePath = new QualifiedNameCollection { browseName };
            return GetFieldValue(eventFields, eventTypeId, browsePath, Attributes.Value);
        }

        /// <summary>
        /// Returns value of the field name containing the event type.
        /// </summary>
        public object GetFieldValue(
            EventFieldList eventFields,
            NodeId eventTypeId,
            IList<QualifiedName> browsePath,
            uint attributeId)
        {
            if (eventFields == null)
            {
                return null;
            }

            if (!(filter_ is EventFilter filter))
            {
                return null;
            }

            for (var ii = 0; ii < filter.SelectClauses.Count; ii++)
            {
                if (ii >= eventFields.EventFields.Count)
                {
                    return null;
                }

                // check for match.
                SimpleAttributeOperand clause = filter.SelectClauses[ii];

                // attribute id
                if (clause.AttributeId != attributeId)
                {
                    continue;
                }

                // match null browse path.
                if (browsePath == null || browsePath.Count == 0)
                {
                    if (clause.BrowsePath != null && clause.BrowsePath.Count > 0)
                    {
                        continue;
                    }

                    // ignore event type id when matching null browse paths.
                    return eventFields.EventFields[ii].Value;
                }

                // match browse path.

                // event type id.
                if (clause.TypeDefinitionId != eventTypeId)
                {
                    continue;
                }

                // match element count.
                if (clause.BrowsePath.Count != browsePath.Count)
                {
                    continue;
                }

                // check each element.
                var match = true;

                for (var jj = 0; jj < clause.BrowsePath.Count; jj++)
                {
                    if (clause.BrowsePath[jj] != browsePath[jj])
                    {
                        match = false;
                        break;
                    }
                }

                // check of no match.
                if (!match)
                {
                    continue;
                }

                // return value.
                return eventFields.EventFields[ii].Value;
            }

            // no event type in event field list.
            return null;
        }

        /// <summary>
        /// Returns value of the field name containing the event type.
        /// </summary>
        public INode GetEventType(EventFieldList eventFields)
        {
            // get event type.
            var eventTypeId = GetFieldValue(eventFields, ObjectTypes.BaseEventType, BrowseNames.EventType) as NodeId;

            if (eventTypeId != null && Subscription?.Session != null)
            {
                return Subscription.Session.NodeCache.Find(eventTypeId);
            }

            // no event type in event field list.
            return null;
        }

        /// <summary>
        /// Returns value of the field name containing the event type.
        /// </summary>
        public DateTime GetEventTime(EventFieldList eventFields)
        {
            // get event time.
            var eventTime = GetFieldValue(eventFields, ObjectTypes.BaseEventType, BrowseNames.Time) as DateTime?;

            if (eventTime != null)
            {
                return eventTime.Value;
            }

            // no event time in event field list.
            return DateTime.MinValue;
        }

        /// <summary>
        /// The service result for a data change notification.
        /// </summary>
        public static ServiceResult GetServiceResult(IEncodeable notification)
        {
            if (!(notification is MonitoredItemNotification dataChange))
            {
                return null;
            }

            NotificationMessage message = dataChange.Message;

            return message == null ? null : new ServiceResult(dataChange.Value.StatusCode, dataChange.DiagnosticInfo, message.StringTable);
        }

        /// <summary>
        /// The service result for a field in an notification (the field must contain a Status object).
        /// </summary>
        public static ServiceResult GetServiceResult(IEncodeable notification, int index)
        {
            if (!(notification is EventFieldList eventFields))
            {
                return null;
            }

            NotificationMessage message = eventFields.Message;

            return message == null
                ? (global::Opc.Ua.ServiceResult)null
                : index < 0 || index >= eventFields.EventFields.Count
                ? null
                : !(ExtensionObject.ToEncodeable(eventFields.EventFields[index].Value as ExtensionObject) is StatusResult status)
                ? null
                : new ServiceResult(status.StatusCode, status.DiagnosticInfo, message.StringTable);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Throws an exception if the filter cannot be used with the node class.
        /// </summary>
        private void ValidateFilter(NodeClass nodeClass, MonitoringFilter filter)
        {
            if (filter == null)
            {
                return;
            }

            switch (nodeClass)
            {
                case NodeClass.Variable:
                case NodeClass.VariableType:
                {
                    if (!typeof(DataChangeFilter).IsInstanceOfType(filter))
                    {
                        nodeClass_ = NodeClass.Variable;
                    }

                    break;
                }

                case NodeClass.Object:
                case NodeClass.View:
                {
                    if (!typeof(EventFilter).IsInstanceOfType(filter))
                    {
                        nodeClass_ = NodeClass.Object;
                    }

                    break;
                }

                case NodeClass.Unspecified:
                case NodeClass.Method:
                case NodeClass.ObjectType:
                case NodeClass.ReferenceType:
                case NodeClass.DataType:
                default:
                {
                    throw ServiceResultException.Create(StatusCodes.BadFilterNotAllowed, "Filters may not be specified for nodes of class '{0}'.", nodeClass);
                }
            }
        }

        /// <summary>
        /// Sets the default event filter.
        /// </summary>
        private void UseDefaultEventFilter()
        {
            var filter = new EventFilter();

            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.EventId);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.EventType);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.SourceNode);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.SourceName);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.Time);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.ReceiveTime);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.LocalTime);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.Message);
            filter.AddSelectClause(ObjectTypes.BaseEventType, BrowseNames.Severity);

            filter_ = filter;
        }
        #endregion

        #region Private Fields
        private string relativePath_;
        private NodeId resolvedNodeId_;
        private NodeClass nodeClass_;
        private int samplingInterval_;
        private MonitoringFilter filter_;
        private uint queueSize_;
        private bool discardOldest_;
        private static long globalClientHandle_;

        private object cache_ = new object();
        private MonitoredItemDataCache dataCache_;
        private MonitoredItemEventCache eventCache_;
        private IEncodeable lastNotification_;
        private event EventHandler<MonitoredItemNotificationEventArgs> NotificationEventHandler;
        #endregion
    }
}
