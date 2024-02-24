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
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Types.Utils;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// A subscription.
    /// </summary>
    [DataContract(Namespace = Namespaces.OpcUaXsd)]
    public partial class Subscription : IDisposable, ICloneable
    {
        #region Constants
        const int MinKeepAliveTimerInterval = 1000;
        const int KeepAliveTimerMargin = 1000;
        #endregion

        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a empty object.
        /// </summary>
        public Subscription()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the subscription from a template.
        /// </summary>
        public Subscription(Subscription template) : this(template, false)
        {
        }

        /// <summary>
        /// Initializes the subscription from a template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="copyEventHandlers">if set to <c>true</c> the event handlers are copied.</param>
        public Subscription(Subscription template, bool copyEventHandlers)
        {
            Initialize();

            if (template != null)
            {
                var displayName = template.DisplayName;

                if (String.IsNullOrEmpty(displayName))
                {
                    displayName = DisplayName;
                }

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

                DisplayName = Utils.Format("{0} {1}", displayName, Utils.IncrementIdentifier(ref globalSubscriptionCounter_));
                PublishingInterval = template.PublishingInterval;
                KeepAliveCount = template.KeepAliveCount;
                LifetimeCount = template.LifetimeCount;
                MinLifetimeInterval = template.MinLifetimeInterval;
                MaxNotificationsPerPublish = template.MaxNotificationsPerPublish;
                PublishingEnabled = template.PublishingEnabled;
                Priority = template.Priority;
                TimestampsToReturn = template.TimestampsToReturn;
                maxMessageCount_ = template.maxMessageCount_;
                sequentialPublishing_ = template.sequentialPublishing_;
                republishAfterTransfer_ = template.republishAfterTransfer_;
                DefaultItem = (MonitoredItem)template.DefaultItem.Clone();
                Handle = template.Handle;
                DisableMonitoredItemCache = template.DisableMonitoredItemCache;
                TransferId = template.TransferId;

                if (copyEventHandlers)
                {
                    StateChanged = template.StateChanged;
                    PublishStatusChangedEventHandler = template.PublishStatusChangedEventHandler;
                    FastDataChangeCallback = template.FastDataChangeCallback;
                    FastEventCallback = template.FastEventCallback;
                    FastKeepAliveCallback = template.FastKeepAliveCallback;
                }

                // copy the list of monitored items.
                foreach (MonitoredItem monitoredItem in template.MonitoredItems)
                {
                    var clone = new MonitoredItem(monitoredItem, copyEventHandlers, true) {
                        DisplayName = monitoredItem.DisplayName
                    };
                    AddItem(clone);
                }
            }
        }

        /// <summary>
        /// Resets the state of the publish timer and associated message worker. 
        /// </summary>
        private void ResetPublishTimerAndWorkerState()
        {
            // stop the publish timer.
            Utils.SilentDispose(publishTimer_);
            publishTimer_ = null;
            Utils.SilentDispose(m_messageWorkerCts);
            messageWorkerEvent_.Set();
            m_messageWorkerCts = null;
            messageWorkerTask_ = null;
        }

        /// <summary>
        /// Called by the .NET framework during deserialization.
        /// </summary>
        [OnDeserializing]
        protected void Initialize(StreamingContext context)
        {
            cache_ = new object();
            Initialize();
        }

        /// <summary>
        /// Sets the private members to default values.
        /// </summary>
        private void Initialize()
        {
            TransferId = Id = 0;
            DisplayName = "Subscription";
            PublishingInterval = 0;
            KeepAliveCount = 0;
            m_keepAliveInterval = 0;
            LifetimeCount = 0;
            MaxNotificationsPerPublish = 0;
            PublishingEnabled = false;
            TimestampsToReturn = TimestampsToReturn.Both;
            maxMessageCount_ = 10;
            republishAfterTransfer_ = false;
            outstandingMessageWorkers_ = 0;
            sequentialPublishing_ = false;
            lastSequenceNumberProcessed_ = 0;
            messageCache_ = new LinkedList<NotificationMessage>();
            monitoredItems_ = new SortedDictionary<uint, MonitoredItem>();
            deletedItems_ = new List<MonitoredItem>();
            messageWorkerEvent_ = new AsyncAutoResetEvent();
            m_messageWorkerCts = null;
            resyncLastSequenceNumberProcessed_ = false;

            DefaultItem = new MonitoredItem {
                DisplayName = "MonitoredItem",
                SamplingInterval = -1,
                MonitoringMode = MonitoringMode.Reporting,
                QueueSize = 0,
                DiscardOldest = true
            };
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ResetPublishTimerAndWorkerState();
            }
        }
        #endregion

        #region ICloneable Members
        /// <summary cref="ICloneable.Clone" />
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            return new Subscription(this);
        }

        /// <summary>
        /// Clones a subscription or a subclass with an option to copy event handlers.
        /// </summary>
        /// <returns>A cloned instance of the subscription or its subclass.</returns>
        public virtual Subscription CloneSubscription(bool copyEventHandlers)
        {
            return new Subscription(this, copyEventHandlers);
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised to indicate that the state of the subscription has changed.
        /// </summary>
        public event EventHandler<SubscriptionStatusChangedEventArgs> SubscriptionStatusChangedEvent
        {
            add => StateChanged += value;
            remove => StateChanged -= value;
        }

        /// <summary>
        ///     Raised to indicate the publishing state for the subscription has stopped or resumed (see PublishingStopped property).
        /// </summary>
        public event EventHandler<PublishStateChangedEventArgs> PublishStatusChangedEvent
        {
            add => PublishStatusChangedEventHandler += value;

            remove => PublishStatusChangedEventHandler -= value;
        }
        #endregion

        #region Persistent Properties
        /// <summary>
        /// A display name for the subscription.
        /// </summary>
        [DataMember(Order = 1)]
        public string DisplayName { get; set; }

        /// <summary>
        /// The publishing interval in milliseconds.
        /// </summary>
        [DataMember(Order = 2)]
        public int PublishingInterval { get; set; }

        /// <summary>
        /// The keep alive count.
        /// </summary>
        [DataMember(Order = 3)]
        public uint KeepAliveCount { get; set; }

        /// <summary>
        /// The life time of of the subscription in counts of
        /// publish interval.
        /// LifetimeCount shall be at least 3*KeepAliveCount.
        /// </summary>
        [DataMember(Order = 4)]
        public uint LifetimeCount { get; set; }

        /// <summary>
        /// The maximum number of notifications per publish request.
        /// </summary>
        [DataMember(Order = 5)]
        public uint MaxNotificationsPerPublish { get; set; }

        /// <summary>
        /// Whether publishing is enabled.
        /// </summary>
        [DataMember(Order = 6)]
        public bool PublishingEnabled { get; set; }

        /// <summary>
        /// The priority assigned to subscription.
        /// </summary>
        [DataMember(Order = 7)]
        public byte Priority { get; set; }

        /// <summary>
        /// The timestamps to return with the notification messages.
        /// </summary>
        [DataMember(Order = 8)]
        public TimestampsToReturn TimestampsToReturn { get; set; }

        /// <summary>
        /// The maximum number of messages to keep in the internal cache.
        /// </summary>
        [DataMember(Order = 9)]
        public int MaxMessageCount
        {
            get => maxMessageCount_;

            set
            {
                // lock needed to synchronize with message list processing
                lock (cache_)
                {
                    maxMessageCount_ = value;
                }
            }
        }

        /// <summary>
        /// The default monitored item.
        /// </summary>
        [DataMember(Order = 10)]
        public MonitoredItem DefaultItem { get; set; }

        /// <summary>
        /// The minimum lifetime for subscriptions in milliseconds.
        /// </summary>
        [DataMember(Order = 12)]
        public uint MinLifetimeInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the notifications are cached within the monitored items.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if monitored item cache is disabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Applications must process the Session.SessionNotificationEvent event if this is set to true.
        /// This flag improves performance by eliminating the processing involved in updating the cache.
        /// </remarks>
        [DataMember(Order = 13)]
        public bool DisableMonitoredItemCache { get; set; }

        /// <summary>
        /// Gets or sets the behavior of waiting for sequential order in handling incoming messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if incoming messages are handled sequentially; <c>false</c> otherwise.
        /// </value>
        /// <remarks>
        /// Setting <see cref="SequentialPublishing"/> to <c>true</c> means incoming messages are processed in a "single-threaded" manner and callbacks will not be invoked in parallel. 
        /// </remarks>
        [DataMember(Order = 14)]
        public bool SequentialPublishing
        {
            get => sequentialPublishing_;
            set
            {
                // synchronize with message list processing
                lock (cache_)
                {
                    sequentialPublishing_ = value;
                }
            }
        }

        /// <summary>
        /// If the available sequence numbers of a subscription
        /// are republished or acknowledged after a transfer. 
        /// </summary>
        /// <remarks>
        /// Default <c>false</c>, set to <c>true</c> if no data loss is important
        /// and available publish requests (sequence numbers) that were never acknowledged should be
        /// recovered with a republish. The setting is used after a subscription transfer.
        /// </remarks>   
        [DataMember(Name = "RepublishAfterTransfer", Order = 15)]
        public bool RepublishAfterTransfer
        {
            get => republishAfterTransfer_;
            set { lock (cache_) { republishAfterTransfer_ = value; } }
        }

        /// <summary>
        /// The unique identifier assigned by the server which can be used to transfer a session.
        /// </summary>
        [DataMember(Name = "TransferId", Order = 16)]
        public uint TransferId { get; set; }

        /// <summary>
        /// Gets or sets the fast data change callback.
        /// </summary>
        /// <value>The fast data change callback.</value>
        /// <remarks>
        /// Only one callback is allowed at a time but it is more efficient to call than an event.
        /// </remarks>
        public FastDataChangeNotificationEventHandler FastDataChangeCallback { get; set; }

        /// <summary>
        /// Gets or sets the fast event callback.
        /// </summary>
        /// <value>The fast event callback.</value>
        /// <remarks>
        /// Only one callback is allowed at a time but it is more efficient to call than an event.
        /// </remarks>
        public FastEventNotificationEventHandler FastEventCallback { get; set; }

        /// <summary>
        /// Gets or sets the fast keep alive callback.
        /// </summary>
        /// <value>The keep alive change callback.</value>
        /// <remarks>
        /// Only one callback is allowed at a time but it is more efficient to call than an event.
        /// </remarks>
        public FastKeepAliveNotificationEventHandler FastKeepAliveCallback { get; set; }

        /// <summary>
        /// The items to monitor.
        /// </summary>
        public IEnumerable<MonitoredItem> MonitoredItems
        {
            get
            {
                lock (cache_)
                {
                    return new List<MonitoredItem>(monitoredItems_.Values);
                }
            }
        }

        /// <summary>
        /// Allows the list of monitored items to be saved/restored when the object is serialized.
        /// </summary>
        [DataMember(Name = "MonitoredItems", Order = 11)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private List<MonitoredItem> SavedMonitoredItems
        {
            get
            {
                lock (cache_)
                {
                    return new List<MonitoredItem>(monitoredItems_.Values);
                }
            }

            set
            {
                if (Created)
                {
                    throw new InvalidOperationException("Cannot update a subscription that has been created on the server.");
                }

                lock (cache_)
                {
                    monitoredItems_.Clear();

                    foreach (MonitoredItem monitoredItem in value)
                    {
                        AddItem(monitoredItem);
                    }
                }
            }
        }
        #endregion

        #region Dynamic Properties
        /// <summary>
        /// Returns true if the subscription has changes that need to be applied.
        /// </summary>
        public bool ChangesPending
        {
            get
            {
                lock (cache_)
                {
                    if (deletedItems_.Count > 0)
                    {
                        return true;
                    }

                    foreach (MonitoredItem monitoredItem in monitoredItems_.Values)
                    {
                        if (Created && !monitoredItem.Status.Created)
                        {
                            return true;
                        }

                        if (monitoredItem.AttributesModified)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the number of monitored items.
        /// </summary>
        public uint MonitoredItemCount
        {
            get
            {
                lock (cache_)
                {
                    return (uint)monitoredItems_.Count;
                }
            }
        }

        /// <summary>
        /// The session that owns the subscription item.
        /// </summary>
        public IUaSession Session { get; protected internal set; }

        /// <summary>
        /// A local handle assigned to the subscription
        /// </summary>
        public object Handle { get; set; }

        /// <summary>
        /// The unique identifier assigned by the server.
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// Whether the subscription has been created on the server.
        /// </summary>
        public bool Created => Id != 0;

        /// <summary>
        /// The current publishing interval.
        /// </summary>
        [DataMember(Name = "CurrentPublishInterval", Order = 20)]
        public double CurrentPublishingInterval { get; private set; }

        /// <summary>
        /// The current keep alive count.
        /// </summary>
        [DataMember(Name = "CurrentKeepAliveCount", Order = 21)]
        public uint CurrentKeepAliveCount { get; private set; }

        /// <summary>
        /// The current lifetime count.
        /// </summary>
        [DataMember(Name = "CurrentLifetimeCount", Order = 22)]
        public uint CurrentLifetimeCount { get; private set; }

        /// <summary>
        /// Whether publishing is currently enabled.
        /// </summary>
        public bool CurrentPublishingEnabled { get; private set; }

        /// <summary>
        /// The priority assigned to subscription when it was created.
        /// </summary>
        public byte CurrentPriority { get; private set; }

        /// <summary>
        /// The when that the last notification received was published.
        /// </summary>
        public DateTime PublishTime
        {
            get
            {
                lock (cache_)
                {
                    if (messageCache_.Count > 0)
                    {
                        return messageCache_.Last.Value.PublishTime;
                    }
                }

                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// The when that the last notification was received.
        /// </summary>
        public DateTime LastNotificationTime
        {
            get
            {
                var ticks = Interlocked.Read(ref lastNotificationTime_);
                return new DateTime(ticks, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// The sequence number assigned to the last notification message.
        /// </summary>
        public uint SequenceNumber
        {
            get
            {
                lock (cache_)
                {
                    if (messageCache_.Count > 0)
                    {
                        return messageCache_.Last.Value.SequenceNumber;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// The number of notifications contained in the last notification message.
        /// </summary>
        public uint NotificationCount
        {
            get
            {
                lock (cache_)
                {
                    if (messageCache_.Count > 0)
                    {
                        return (uint)messageCache_.Last.Value.NotificationData.Count;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// The last notification received from the server.
        /// </summary>
        public NotificationMessage LastNotification
        {
            get
            {
                lock (cache_)
                {
                    return messageCache_.Count > 0 ? messageCache_.Last.Value : null;
                }
            }
        }

        /// <summary>
        /// The cached notifications.
        /// </summary>
        public IEnumerable<NotificationMessage> Notifications
        {
            get
            {
                lock (cache_)
                {
                    // make a copy to ensure the state of the last cannot change during enumeration.
                    return new List<NotificationMessage>(messageCache_);
                }
            }
        }

        /// <summary>
        /// The sequence numbers that are available for republish requests.
        /// </summary>
        public IEnumerable<uint> AvailableSequenceNumbers
        {
            get
            {
                lock (cache_)
                {
                    return availableSequenceNumbers_ != null ?
                        (IEnumerable<uint>)new ReadOnlyList<uint>(availableSequenceNumbers_) :
                        Enumerable.Empty<uint>();
                }
            }
        }

        /// <summary>
        /// Sends a notification that the state of the subscription has changed.
        /// </summary>
        public void ChangesCompleted()
        {
            StateChanged?.Invoke(this, new SubscriptionStatusChangedEventArgs(changeMask_));

            changeMask_ = SubscriptionChangeMask.None;
        }

        /// <summary>
        /// Returns true if the subscription is not receiving publishes.
        /// </summary>
        public bool PublishingStopped
        {
            get
            {
                var timeSinceLastNotification = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - Interlocked.Read(ref lastNotificationTime_));
                return timeSinceLastNotification.TotalMilliseconds > m_keepAliveInterval + KeepAliveTimerMargin;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a subscription on the server and adds all monitored items.
        /// </summary>
        public void Create()
        {
            VerifySubscriptionState(false);

            // create the subscription.
            var revisedKeepAliveCount = KeepAliveCount;
            var revisedLifetimeCounter = LifetimeCount;

            AdjustCounts(ref revisedKeepAliveCount, ref revisedLifetimeCounter);

            _ = Session.CreateSubscription(
                null,
                PublishingInterval,
                revisedLifetimeCounter,
                revisedKeepAliveCount,
                MaxNotificationsPerPublish,
                PublishingEnabled,
                Priority,
                out var subscriptionId,
                out var revisedPublishingInterval,
                out revisedLifetimeCounter,
                out revisedKeepAliveCount);

            CreateSubscription(subscriptionId, revisedPublishingInterval, revisedKeepAliveCount, revisedLifetimeCounter);

            _ = CreateItems();

            ChangesCompleted();

            TraceState("CREATED");
        }

        /// <summary>
        /// Called after the subscription was transferred.
        /// </summary>
        /// <param name="session">The session to which the subscription is transferred.</param>
        /// <param name="id">Id of the transferred subscription.</param>
        /// <param name="availableSequenceNumbers">The available sequence numbers on the server.</param>
        public bool Transfer(IUaSession session, uint id, UInt32Collection availableSequenceNumbers)
        {
            if (Created)
            {
                // handle the case when the client has the subscription template and reconnects
                if (id != Id)
                {
                    return false;
                }

                // remove the subscription from disconnected session
                if (Session?.RemoveTransferredSubscription(this) != true)
                {
                    Utils.LogError("SubscriptionId {0}: Failed to remove transferred subscription from owner SessionId={1}.", Id, Session?.SessionId);
                    return false;
                }

                // remove default subscription template which was copied in Session.Create()
                var subscriptionsToRemove = session.Subscriptions.Where(s => !s.Created && s.TransferId == Id).ToList();
                _ = session.RemoveSubscriptions(subscriptionsToRemove);

                // add transferred subscription to session
                if (!session.AddSubscription(this))
                {
                    Utils.LogError("SubscriptionId {0}: Failed to add transferred subscription to SessionId={1}.", Id, session.SessionId);
                    return false;
                }
            }
            else
            {
                // handle the case when the client restarts and loads the saved subscriptions from storage
                if (!GetMonitoredItems(out UInt32Collection serverHandles, out UInt32Collection clientHandles))
                {
                    Utils.LogError("SubscriptionId {0}: The server failed to respond to GetMonitoredItems after transfer.", Id);
                    return false;
                }

                if (serverHandles.Count != monitoredItems_.Count ||
                    clientHandles.Count != monitoredItems_.Count)
                {
                    // invalid state
                    Utils.LogError("SubscriptionId {0}: Number of Monitored Items on client and server do not match after transfer {1}!={2}",
                        Id, serverHandles.Count, monitoredItems_.Count);
                    return false;
                }

                // sets state to 'Created'
                Id = id;
                TransferItems(serverHandles, clientHandles, out IList<MonitoredItem> itemsToModify);

                _ = ModifyItems();
            }

            // add available sequence numbers to incoming 
            ProcessTransferredSequenceNumbers(availableSequenceNumbers);

            changeMask_ |= SubscriptionChangeMask.Transferred;
            ChangesCompleted();

            StartKeepAliveTimer();

            TraceState("TRANSFERRED");

            return true;
        }

        /// <summary>
        /// Called after the subscription was transferred.
        /// </summary>
        /// <param name="session">The session to which the subscription is transferred.</param>
        /// <param name="id">Id of the transferred subscription.</param>
        /// <param name="availableSequenceNumbers">The available sequence numbers on the server.</param>
        /// <param name="ct">The cancellation token.</param>
        public async Task<bool> TransferAsync(IUaSession session, uint id, UInt32Collection availableSequenceNumbers, CancellationToken ct = default)
        {
            if (Created)
            {
                // handle the case when the client has the subscription template and reconnects
                if (id != Id)
                {
                    return false;
                }

                // remove the subscription from disconnected session
                if (Session?.RemoveTransferredSubscription(this) != true)
                {
                    Utils.LogError("SubscriptionId {0}: Failed to remove transferred subscription from owner SessionId={1}.", Id, Session?.SessionId);
                    return false;
                }

                // remove default subscription template which was copied in Session.Create()
                var subscriptionsToRemove = session.Subscriptions.Where(s => !s.Created && s.TransferId == Id).ToList();
                _ = await session.RemoveSubscriptionsAsync(subscriptionsToRemove, ct).ConfigureAwait(false);

                // add transferred subscription to session
                if (!session.AddSubscription(this))
                {
                    Utils.LogError("SubscriptionId {0}: Failed to add transferred subscription to SessionId={1}.", Id, session.SessionId);
                    return false;
                }
            }
            else
            {
                // handle the case when the client restarts and loads the saved subscriptions from storage
                bool success;
                UInt32Collection serverHandles;
                UInt32Collection clientHandles;
                (success, serverHandles, clientHandles) = await GetMonitoredItemsAsync(ct).ConfigureAwait(false);
                if (!success)
                {
                    Utils.LogError("SubscriptionId {0}: The server failed to respond to GetMonitoredItems after transfer.", Id);
                    return false;
                }

                if (serverHandles.Count != monitoredItems_.Count ||
                    clientHandles.Count != monitoredItems_.Count)
                {
                    // invalid state
                    Utils.LogError("SubscriptionId {0}: Number of Monitored Items on client and server do not match after transfer {1}!={2}",
                        Id, serverHandles.Count, monitoredItems_.Count);
                    return false;
                }

                // sets state to 'Created'
                Id = id;
                TransferItems(serverHandles, clientHandles, out IList<MonitoredItem> itemsToModify);

                _ = await ModifyItemsAsync(ct).ConfigureAwait(false);
            }

            // add available sequence numbers to incoming 
            ProcessTransferredSequenceNumbers(availableSequenceNumbers);

            changeMask_ |= SubscriptionChangeMask.Transferred;
            ChangesCompleted();

            StartKeepAliveTimer();

            TraceState("TRANSFERRED ASYNC");

            return true;
        }

        /// <summary>
        /// Deletes a subscription on the server.
        /// </summary>
        public void Delete(bool silent)
        {
            if (!silent)
            {
                VerifySubscriptionState(true);
            }

            // nothing to do if not created.
            if (!Created)
            {
                return;
            }

            try
            {
                TraceState("DELETE");

                lock (cache_)
                {
                    ResetPublishTimerAndWorkerState();
                }

                // delete the subscription.
                UInt32Collection subscriptionIds = new[] { Id };

                ResponseHeader responseHeader = Session.DeleteSubscriptions(
                    null,
                    subscriptionIds,
                    out StatusCodeCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                // validate response.
                ClientBase.ValidateResponse(results, subscriptionIds);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, subscriptionIds);

                if (StatusCode.IsBad(results[0]))
                {
                    throw new ServiceResultException(ClientBase.GetResult(results[0], 0, diagnosticInfos, responseHeader));
                }
            }

            // suppress exception if silent flag is set.
            catch (Exception e)
            {
                if (!silent)
                {
                    throw new ServiceResultException(e, StatusCodes.BadUnexpectedError);
                }
            }

            // always put object in disconnected state even if an error occurs.
            finally
            {
                DeleteSubscription();
            }

            ChangesCompleted();
        }

        /// <summary>
        /// Modifies a subscription on the server.
        /// </summary>
        public void Modify()
        {
            VerifySubscriptionState(true);

            // modify the subscription.
            var revisedKeepAliveCount = KeepAliveCount;
            var revisedLifetimeCounter = LifetimeCount;

            AdjustCounts(ref revisedKeepAliveCount, ref revisedLifetimeCounter);

            _ = Session.ModifySubscription(
                null,
                Id,
                PublishingInterval,
                revisedLifetimeCounter,
                revisedKeepAliveCount,
                MaxNotificationsPerPublish,
                Priority,
                out var revisedPublishingInterval,
                out revisedLifetimeCounter,
                out revisedKeepAliveCount);

            // update current state.
            ModifySubscription(
                revisedPublishingInterval,
                revisedKeepAliveCount,
                revisedLifetimeCounter);

            ChangesCompleted();

            TraceState("MODIFIED");
        }

        /// <summary>
        /// Changes the publishing enabled state for the subscription.
        /// </summary>
        public void SetPublishingMode(bool enabled)
        {
            VerifySubscriptionState(true);

            // modify the subscription.
            UInt32Collection subscriptionIds = new[] { Id };

            ResponseHeader responseHeader = Session.SetPublishingMode(
                null,
                enabled,
                new[] { Id },
                out StatusCodeCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            // validate response.
            ClientBase.ValidateResponse(results, subscriptionIds);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, subscriptionIds);

            if (StatusCode.IsBad(results[0]))
            {
                throw new ServiceResultException(ClientBase.GetResult(results[0], 0, diagnosticInfos, responseHeader));
            }

            // update current state.
            CurrentPublishingEnabled = PublishingEnabled = enabled;

            changeMask_ |= SubscriptionChangeMask.Modified;
            ChangesCompleted();

            TraceState(enabled ? "PUBLISHING ENABLED" : "PUBLISHING DISABLED");
        }

        /// <summary>
        /// Republishes the specified notification message.
        /// </summary>
        public NotificationMessage Republish(uint sequenceNumber)
        {
            VerifySubscriptionState(true);

            _ = Session.Republish(
                null,
                Id,
                sequenceNumber,
                out NotificationMessage message);

            return message;
        }

        /// <summary>
        /// Applies any changes to the subscription items.
        /// </summary>
        public void ApplyChanges()
        {
            _ = DeleteItems();
            _ = ModifyItems();
            _ = CreateItems();
        }

        /// <summary>
        /// Resolves all relative paths to nodes on the server.
        /// </summary>
        public void ResolveItemNodeIds()
        {
            VerifySubscriptionState(true);

            // collect list of browse paths.
            var browsePaths = new BrowsePathCollection();
            var itemsToBrowse = new List<MonitoredItem>();


            PrepareResolveItemNodeIds(browsePaths, itemsToBrowse);

            // nothing to do.
            if (browsePaths.Count == 0)
            {
                return;
            }

            // translate browse paths.
            ResponseHeader responseHeader = Session.TranslateBrowsePathsToNodeIds(
                null,
                browsePaths,
                out BrowsePathResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, browsePaths);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, browsePaths);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToBrowse[ii].SetResolvePathResult(results[ii], ii, diagnosticInfos, responseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsModified;
        }

        /// <summary>
        /// Creates all items that have not already been created.
        /// </summary>
        public IList<MonitoredItem> CreateItems()
        {
            List<MonitoredItem> itemsToCreate;
            MonitoredItemCreateRequestCollection requestItems = PrepareItemsToCreate(out itemsToCreate);

            if (requestItems.Count == 0)
            {
                return itemsToCreate;
            }

            // create monitored items.
            ResponseHeader responseHeader = Session.CreateMonitoredItems(
                null,
                Id,
                TimestampsToReturn,
                requestItems,
                out MonitoredItemCreateResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, itemsToCreate);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToCreate);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToCreate[ii].SetCreateResult(requestItems[ii], results[ii], ii, diagnosticInfos, responseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsCreated;
            ChangesCompleted();

            // return the list of items affected by the change.
            return itemsToCreate;
        }

        /// <summary>
        /// Modifies all items that have been changed.
        /// </summary>
        public IList<MonitoredItem> ModifyItems()
        {
            VerifySubscriptionState(true);

            var requestItems = new MonitoredItemModifyRequestCollection();
            var itemsToModify = new List<MonitoredItem>();

            PrepareItemsToModify(requestItems, itemsToModify);

            if (requestItems.Count == 0)
            {
                return itemsToModify;
            }

            // modify the subscription.
            ResponseHeader responseHeader = Session.ModifyMonitoredItems(
                null,
                Id,
                TimestampsToReturn,
                requestItems,
                out MonitoredItemModifyResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, itemsToModify);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToModify);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToModify[ii].SetModifyResult(requestItems[ii], results[ii], ii, diagnosticInfos, responseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsCreated;
            ChangesCompleted();

            // return the list of items affected by the change.
            return itemsToModify;
        }

        /// <summary>
        /// Deletes all items that have been marked for deletion.
        /// </summary>
        public IList<MonitoredItem> DeleteItems()
        {
            VerifySubscriptionState(true);

            if (deletedItems_.Count == 0)
            {
                return new List<MonitoredItem>();
            }

            List<MonitoredItem> itemsToDelete = deletedItems_;
            deletedItems_ = new List<MonitoredItem>();

            var monitoredItemIds = new UInt32Collection();

            foreach (MonitoredItem monitoredItem in itemsToDelete)
            {
                monitoredItemIds.Add(monitoredItem.Status.Id);
            }

            ResponseHeader responseHeader = Session.DeleteMonitoredItems(
                null,
                Id,
                monitoredItemIds,
                out StatusCodeCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, monitoredItemIds);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, monitoredItemIds);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToDelete[ii].SetDeleteResult(results[ii], ii, diagnosticInfos, responseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsDeleted;
            ChangesCompleted();

            // return the list of items affected by the change.
            return itemsToDelete;
        }

        /// <summary>
        /// Set monitoring mode of items.
        /// </summary>
        public List<ServiceResult> SetMonitoringMode(
            MonitoringMode monitoringMode,
            IList<MonitoredItem> monitoredItems)
        {
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));

            VerifySubscriptionState(true);

            if (monitoredItems.Count == 0)
            {
                return null;
            }

            // get list of items to update.
            var monitoredItemIds = new UInt32Collection();

            foreach (MonitoredItem monitoredItem in monitoredItems)
            {
                monitoredItemIds.Add(monitoredItem.Status.Id);
            }

            ResponseHeader responseHeader = Session.SetMonitoringMode(
                null,
                Id,
                monitoringMode,
                monitoredItemIds,
                out StatusCodeCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, monitoredItemIds);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, monitoredItemIds);

            // update results.
            var errors = new List<ServiceResult>();
            var noErrors = UpdateMonitoringMode(
                monitoredItems, errors, results,
                diagnosticInfos, responseHeader,
                monitoringMode);

            // raise state changed event.
            changeMask_ |= SubscriptionChangeMask.ItemsModified;
            ChangesCompleted();

            // return null list if no errors occurred.
            return noErrors ? null : errors;
        }

        /// <summary>
        /// Adds the notification message to internal cache.
        /// </summary>
        public void SaveMessageInCache(
            IList<uint> availableSequenceNumbers,
            NotificationMessage message,
            IList<string> stringTable)
        {
            EventHandler<PublishStateChangedEventArgs> callback = null;

            lock (cache_)
            {
                if (availableSequenceNumbers != null)
                {
                    availableSequenceNumbers_ = availableSequenceNumbers;
                }

                if (message == null)
                {
                    return;
                }

                // check if a publish error was previously reported.
                if (PublishingStopped)
                {
                    callback = PublishStatusChangedEventHandler;
                    TraceState("PUBLISHING RECOVERED");
                }

                DateTime now = DateTime.UtcNow;
                _ = Interlocked.Exchange(ref lastNotificationTime_, now.Ticks);

                // save the string table that came with notification.
                message.StringTable = new List<string>(stringTable);

                // create queue for the first time.
                if (incomingMessages_ == null)
                {
                    incomingMessages_ = new LinkedList<IncomingMessage>();
                }

                // find or create an entry for the incoming sequence number.
                IncomingMessage entry = FindOrCreateEntry(now, message.SequenceNumber);

                // check for keep alive.
                if (message.NotificationData.Count > 0)
                {
                    entry.Message = message;
                    entry.Processed = false;
                }

                // fill in any gaps in the queue
                LinkedListNode<IncomingMessage> node = incomingMessages_.First;

                while (node != null)
                {
                    entry = node.Value;
                    LinkedListNode<IncomingMessage> next = node.Next;

                    if (next != null && next.Value.SequenceNumber > entry.SequenceNumber + 1)
                    {
                        var placeholder = new IncomingMessage {
                            SequenceNumber = entry.SequenceNumber + 1,
                            Timestamp = now
                        };
                        node = incomingMessages_.AddAfter(node, placeholder);
                        continue;
                    }

                    node = next;
                }

                // clean out processed values.
                node = incomingMessages_.First;

                while (node != null)
                {
                    entry = node.Value;
                    LinkedListNode<IncomingMessage> next = node.Next;

                    // can only pull off processed or expired messages.
                    if (!entry.Processed && !(entry.Republished && entry.Timestamp.AddSeconds(10) < now))
                    {
                        break;
                    }

                    if (next != null)
                    {
                        //If the message being removed is supposed to be the next message, advance it to release anything waiting on it to be processed
                        if (entry.SequenceNumber == lastSequenceNumberProcessed_ + 1)
                        {
                            if (!entry.Processed)
                            {
                                Utils.LogWarning("SubscriptionId {0} skipping PublishResponse Sequence Number {1}", Id, entry.SequenceNumber);
                            }

                            lastSequenceNumberProcessed_ = entry.SequenceNumber;
                        }

                        incomingMessages_.Remove(node);
                    }

                    node = next;
                }
            }

            // send notification that publishing received a keep alive or has to republish.
            if (callback != null)
            {
                try
                {
                    callback(this, new PublishStateChangedEventArgs(PublishStateChangedMask.Recovered));
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error while raising PublishStateChanged event.");
                }
            }

            // process messages.
            messageWorkerEvent_.Set();
        }

        /// <summary>
        /// Get the number of outstanding message workers
        /// </summary>
        public int OutstandingMessageWorkers => outstandingMessageWorkers_;

        /// <summary>
        /// Adds an item to the subscription.
        /// </summary>
        public void AddItem(MonitoredItem monitoredItem)
        {
            if (monitoredItem == null) throw new ArgumentNullException(nameof(monitoredItem));

            lock (cache_)
            {
                if (monitoredItems_.ContainsKey(monitoredItem.ClientHandle))
                {
                    return;
                }

                monitoredItems_.Add(monitoredItem.ClientHandle, monitoredItem);
                monitoredItem.Subscription = this;
            }

            changeMask_ |= SubscriptionChangeMask.ItemsAdded;
            ChangesCompleted();
        }

        /// <summary>
        /// Adds an item to the subscription.
        /// </summary>
        public void AddItems(IEnumerable<MonitoredItem> monitoredItems)
        {
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));

            var added = false;

            lock (cache_)
            {
                foreach (MonitoredItem monitoredItem in monitoredItems)
                {
                    if (!monitoredItems_.ContainsKey(monitoredItem.ClientHandle))
                    {
                        monitoredItems_.Add(monitoredItem.ClientHandle, monitoredItem);
                        monitoredItem.Subscription = this;
                        added = true;
                    }
                }
            }

            if (added)
            {
                changeMask_ |= SubscriptionChangeMask.ItemsAdded;
                ChangesCompleted();
            }
        }

        /// <summary>
        /// Removes an item from the subscription.
        /// </summary>
        public void RemoveItem(MonitoredItem monitoredItem)
        {
            if (monitoredItem == null) throw new ArgumentNullException(nameof(monitoredItem));

            lock (cache_)
            {
                if (!monitoredItems_.Remove(monitoredItem.ClientHandle))
                {
                    return;
                }

                monitoredItem.Subscription = null;
            }

            if (monitoredItem.Status.Created)
            {
                deletedItems_.Add(monitoredItem);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsRemoved;
            ChangesCompleted();
        }

        /// <summary>
        /// Removes an item from the subscription.
        /// </summary>
        public void RemoveItems(IEnumerable<MonitoredItem> monitoredItems)
        {
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));

            var changed = false;

            lock (cache_)
            {
                foreach (MonitoredItem monitoredItem in monitoredItems)
                {
                    if (monitoredItems_.Remove(monitoredItem.ClientHandle))
                    {
                        monitoredItem.Subscription = null;

                        if (monitoredItem.Status.Created)
                        {
                            deletedItems_.Add(monitoredItem);
                        }

                        changed = true;
                    }
                }
            }

            if (changed)
            {
                changeMask_ |= SubscriptionChangeMask.ItemsRemoved;
                ChangesCompleted();
            }
        }

        /// <summary>
        /// Returns the monitored item identified by the client handle.
        /// </summary>
        public MonitoredItem FindItemByClientHandle(uint clientHandle)
        {
            lock (cache_)
            {
                MonitoredItem monitoredItem = null;

                return monitoredItems_.TryGetValue(clientHandle, out monitoredItem) ? monitoredItem : null;
            }
        }

        /// <summary>
        /// Tells the server to refresh all conditions being monitored by the subscription.
        /// </summary>
        public bool ConditionRefresh()
        {
            VerifySubscriptionState(true);

            try
            {
                _ = Session.Call(
                    ObjectTypeIds.ConditionType,
                    MethodIds.ConditionType_ConditionRefresh,
                    Id);

                return true;
            }
            catch (ServiceResultException sre)
            {
                Utils.LogError(sre, "SubscriptionId {0}: Failed to call ConditionRefresh on server", Id);
            }
            return false;
        }

        /// <summary>
        /// Call the ResendData method on the server for this subscription.
        /// </summary>
        public bool ResendData()
        {
            VerifySubscriptionState(true);

            try
            {
                _ = Session.Call(ObjectIds.Server, MethodIds.Server_ResendData, Id);
                return true;
            }
            catch (ServiceResultException sre)
            {
                Utils.LogError(sre, "SubscriptionId {0}: Failed to call ResendData on server", Id);
            }
            return false;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the available sequence numbers and queues after transfer.
        /// </summary>
        /// <remarks>
        /// If <see cref="RepublishAfterTransfer"/> is set to <c>true</c>, sequence numbers
        /// are queued for republish, otherwise ack may be sent.
        /// </remarks>
        /// <param name="availableSequenceNumbers">The list of available sequence numbers on the server.</param>
        private void ProcessTransferredSequenceNumbers(UInt32Collection availableSequenceNumbers)
        {
            lock (cache_)
            {
                // reset incoming state machine and clear cache
                lastSequenceNumberProcessed_ = 0;
                resyncLastSequenceNumberProcessed_ = true;
                incomingMessages_ = new LinkedList<IncomingMessage>();

                // save available sequence numbers
                availableSequenceNumbers_ = (UInt32Collection)availableSequenceNumbers.MemberwiseClone();

                if (availableSequenceNumbers.Count != 0 && republishAfterTransfer_)
                {
                    // create queue for the first time.
                    if (incomingMessages_ == null)
                    {
                        incomingMessages_ = new LinkedList<IncomingMessage>();
                    }

                    // update last sequence number processed
                    // available seq numbers may not be in order
                    foreach (var sequenceNumber in availableSequenceNumbers)
                    {
                        if (sequenceNumber >= lastSequenceNumberProcessed_)
                        {
                            lastSequenceNumberProcessed_ = sequenceNumber + 1;
                        }
                    }

                    // only republish consecutive sequence numbers
                    // triggers the republish mechanism immediately,
                    // if event is in the past
                    DateTime now = DateTime.UtcNow.AddSeconds(-5);
                    var lastSequenceNumberToRepublish = lastSequenceNumberProcessed_ - 1;
                    var availableNumbers = availableSequenceNumbers.Count;
                    var republishMessages = 0;
                    for (var i = 0; i < availableNumbers; i++)
                    {
                        var found = false;
                        foreach (var sequenceNumber in availableSequenceNumbers)
                        {
                            if (lastSequenceNumberToRepublish == sequenceNumber)
                            {
                                _ = FindOrCreateEntry(now, sequenceNumber);
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            // remove sequence number handled for republish
                            _ = availableSequenceNumbers.Remove(lastSequenceNumberToRepublish);
                            lastSequenceNumberToRepublish--;
                            republishMessages++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    Utils.LogInfo("SubscriptionId {0}: Republishing {1} messages, next sequencenumber {2} after transfer.",
                        Id, republishMessages, lastSequenceNumberProcessed_);

                    availableSequenceNumbers.Clear();
                }
            }
        }

        /// <summary>
        /// Call the GetMonitoredItems method on the server.
        /// </summary>
        private bool GetMonitoredItems(out UInt32Collection serverHandles, out UInt32Collection clientHandles)
        {
            serverHandles = new UInt32Collection();
            clientHandles = new UInt32Collection();
            try
            {
                IList<object> outputArguments = Session.Call(ObjectIds.Server, MethodIds.Server_GetMonitoredItems, TransferId);
                if (outputArguments != null && outputArguments.Count == 2)
                {
                    serverHandles.AddRange((uint[])outputArguments[0]);
                    clientHandles.AddRange((uint[])outputArguments[1]);
                    return true;
                }
            }
            catch (ServiceResultException sre)
            {
                Utils.LogError(sre, "SubscriptionId {0}: Failed to call GetMonitoredItems on server", Id);
            }
            return false;
        }

        /// <summary>
        /// Call the GetMonitoredItems method on the server.
        /// </summary>
        private async Task<(bool, UInt32Collection, UInt32Collection)> GetMonitoredItemsAsync(CancellationToken ct = default)
        {
            var serverHandles = new UInt32Collection();
            var clientHandles = new UInt32Collection();
            try
            {
                IList<object> outputArguments = await Session.CallAsync(ObjectIds.Server, MethodIds.Server_GetMonitoredItems, ct, TransferId).ConfigureAwait(false);
                if (outputArguments != null && outputArguments.Count == 2)
                {
                    serverHandles.AddRange((uint[])outputArguments[0]);
                    clientHandles.AddRange((uint[])outputArguments[1]);
                    return (true, serverHandles, clientHandles);
                }
            }
            catch (ServiceResultException sre)
            {
                Utils.LogError(sre, "SubscriptionId {0}: Failed to call GetMonitoredItems on server", Id);
            }
            return (false, serverHandles, clientHandles);
        }

        /// <summary>
        /// Starts a timer to ensure publish requests are sent frequently enough to detect network interruptions.
        /// </summary>
        private void StartKeepAliveTimer()
        {
            // stop the publish timer.
            lock (cache_)
            {
                Utils.SilentDispose(publishTimer_);
                publishTimer_ = null;

                Interlocked.Exchange(ref lastNotificationTime_, DateTime.UtcNow.Ticks);
                m_keepAliveInterval = (int)(Math.Min(CurrentPublishingInterval * (CurrentKeepAliveCount + 1), Int32.MaxValue));
                if (m_keepAliveInterval < MinKeepAliveTimerInterval)
                {
                    m_keepAliveInterval = (int)(Math.Min(PublishingInterval * (KeepAliveCount + 1), Int32.MaxValue));
                    m_keepAliveInterval = Math.Min(MinKeepAliveTimerInterval, m_keepAliveInterval);
                }
#if NET6_0_OR_GREATER
                var publishTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(m_keepAliveInterval));
                _ = Task.Run(() => OnKeepAliveAsync(publishTimer));
                publishTimer_ = publishTimer;
#else
                publishTimer_ = new Timer(OnKeepAlive, KeepAliveCount, KeepAliveCount, m_keepAliveInterval);
#endif

                if (messageWorkerTask_ == null || messageWorkerTask_.IsCompleted)
                {
                    Utils.SilentDispose(m_messageWorkerCts);
                    m_messageWorkerCts = new CancellationTokenSource();
                    CancellationToken ct = m_messageWorkerCts.Token;
                    messageWorkerTask_ = Task.Run(() => {
                        return PublishResponseMessageWorkerAsync(ct);
                    });
                }
            }

            // start publishing. Fill the queue.
            Session.StartPublishing(BeginPublishTimeout(), false);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Checks if a notification has arrived. Sends a publish if it has not.
        /// </summary>
        private async Task OnKeepAliveAsync(PeriodicTimer publishTimer)
        {
            while (await publishTimer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                if (!PublishingStopped)
                {
                    continue;
                }

                HandleOnKeepAliveStopped();
            }
        }
#else
        /// <summary>
        /// Checks if a notification has arrived. Sends a publish if it has not.
        /// </summary>
        private void OnKeepAlive(object state)
        {
            if (!PublishingStopped)
            {
                return;
            }

            HandleOnKeepAliveStopped();
        }
#endif

        /// <summary>
        /// Handles callback if publishing stopped. Sends a publish.
        /// </summary>
        private void HandleOnKeepAliveStopped()
        {
            // check if a publish has arrived.
            EventHandler<PublishStateChangedEventArgs> callback = PublishStatusChangedEventHandler;

            _ = Interlocked.Increment(ref publishLateCount_);

            TraceState("PUBLISHING STOPPED");

            if (callback != null)
            {
                try
                {
                    callback(this, new PublishStateChangedEventArgs(PublishStateChangedMask.Stopped));
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error while raising PublishStateChanged event.");
                }
            }

            // try to send a publish to recover stopped publishing.
            _ = (Session?.BeginPublish(BeginPublishTimeout()));
        }

        /// <summary>
        /// Publish response worker task for the subscriptions.
        /// </summary>
        private async Task PublishResponseMessageWorkerAsync(CancellationToken ct)
        {
            Utils.LogTrace("SubscriptionId {0} - Publish Thread {1:X8} Started.", Id, Environment.CurrentManagedThreadId);

            bool cancelled;
            try
            {
                do
                {
                    await messageWorkerEvent_.WaitAsync().ConfigureAwait(false);

                    cancelled = ct.IsCancellationRequested;
                    if (!cancelled)
                    {
                        await OnMessageReceivedAsync(ct).ConfigureAwait(false);
                        cancelled = ct.IsCancellationRequested;
                    }
                }
                while (!cancelled);
            }
            catch (OperationCanceledException)
            {
                // intentionally fall through
            }
            catch (Exception e)
            {
                Utils.LogError(e, "SubscriptionId {0} - Publish Worker Thread {1:X8} Exited Unexpectedly.", Id, Environment.CurrentManagedThreadId);
                return;
            }

            Utils.LogTrace("SubscriptionId {0} - Publish Thread {1:X8} Exited Normally.", Id, Environment.CurrentManagedThreadId);
        }

        /// <summary>
        /// Dumps the current state of the session queue.
        /// </summary>
        internal void TraceState(string context)
        {
            UaClientUtils.EventLog.SubscriptionState(context, Id, new DateTime(lastNotificationTime_), Session?.GoodPublishRequestCount ?? 0,
                CurrentPublishingInterval, CurrentKeepAliveCount, CurrentPublishingEnabled, MonitoredItemCount);
        }

        /// <summary>
        /// Calculate the timeout of a publish request.
        /// </summary>
        private int BeginPublishTimeout()
        {
            return Math.Max(Math.Min(m_keepAliveInterval * 3, Int32.MaxValue), MinKeepAliveTimerInterval); ;
        }

        /// <summary>
        /// Update the subscription with the given revised settings.
        /// </summary>
        private void ModifySubscription(
            double revisedPublishingInterval,
            uint revisedKeepAliveCount,
            uint revisedLifetimeCounter
            )
        {
            CreateOrModifySubscription(false, 0,
                revisedPublishingInterval, revisedKeepAliveCount, revisedLifetimeCounter);
        }

        /// <summary>
        /// Update the subscription with the given revised settings.
        /// </summary>
        private void CreateSubscription(
            uint subscriptionId,
            double revisedPublishingInterval,
            uint revisedKeepAliveCount,
            uint revisedLifetimeCounter
            )
        {
            CreateOrModifySubscription(true, subscriptionId,
                revisedPublishingInterval, revisedKeepAliveCount, revisedLifetimeCounter);
        }

        /// <summary>
        /// Update the subscription with the given revised settings.
        /// </summary>
        private void CreateOrModifySubscription(
            bool created,
            uint subscriptionId,
            double revisedPublishingInterval,
            uint revisedKeepAliveCount,
            uint revisedLifetimeCounter
            )
        {
            // update current state.
            CurrentPublishingInterval = revisedPublishingInterval;
            CurrentKeepAliveCount = revisedKeepAliveCount;
            CurrentLifetimeCount = revisedLifetimeCounter;
            CurrentPriority = Priority;

            if (!created)
            {
                changeMask_ |= SubscriptionChangeMask.Modified;
            }
            else
            {
                CurrentPublishingEnabled = PublishingEnabled;
                TransferId = Id = subscriptionId;
                StartKeepAliveTimer();
                changeMask_ |= SubscriptionChangeMask.Created;
            }

            if (KeepAliveCount != revisedKeepAliveCount)
            {
                Utils.LogInfo("For subscription {0}, Keep alive count was revised from {1} to {2}",
                    Id, KeepAliveCount, revisedKeepAliveCount);
            }

            if (LifetimeCount != revisedLifetimeCounter)
            {
                Utils.LogInfo("For subscription {0}, Lifetime count was revised from {1} to {2}",
                    Id, LifetimeCount, revisedLifetimeCounter);
            }

            if (PublishingInterval != revisedPublishingInterval)
            {
                Utils.LogInfo("For subscription {0}, Publishing interval was revised from {1} to {2}",
                    Id, PublishingInterval, revisedPublishingInterval);
            }

            if (revisedLifetimeCounter < revisedKeepAliveCount * 3)
            {
                Utils.LogInfo("For subscription {0}, Revised lifetime counter (value={1}) is less than three times the keep alive count (value={2})", Id, revisedLifetimeCounter, revisedKeepAliveCount);
            }

            if (CurrentPriority == 0)
            {
                Utils.LogInfo("For subscription {0}, the priority was set to 0.", Id);
            }
        }

        /// <summary>
        /// Delete the subscription.
        /// Ignore errors, always reset all parameter.
        /// </summary>
        private void DeleteSubscription()
        {
            TransferId = Id = 0;
            CurrentPublishingInterval = 0;
            CurrentKeepAliveCount = 0;
            CurrentPublishingEnabled = false;
            CurrentPriority = 0;

            // update items.
            lock (cache_)
            {
                foreach (MonitoredItem monitoredItem in monitoredItems_.Values)
                {
                    monitoredItem.SetDeleteResult(StatusCodes.Good, -1, null, null);
                }
            }

            deletedItems_.Clear();

            changeMask_ |= SubscriptionChangeMask.Deleted;
        }

        /// <summary>
        /// Ensures sensible values for the counts.
        /// </summary>
        private void AdjustCounts(ref uint keepAliveCount, ref uint lifetimeCount)
        {
            const uint kDefaultKeepAlive = 10;
            const uint kDefaultLifeTime = 1000;

            // keep alive count must be at least 1, 10 is a good default.
            if (keepAliveCount == 0)
            {
                Utils.LogInfo("Adjusted KeepAliveCount from value={0}, to value={1}, for subscription {2}.",
                    keepAliveCount, kDefaultKeepAlive, Id);
                keepAliveCount = kDefaultKeepAlive;
            }

            // ensure the lifetime is sensible given the sampling interval.
            if (PublishingInterval > 0)
            {
                if (MinLifetimeInterval > 0 && MinLifetimeInterval < Session.SessionTimeout)
                {
                    Utils.LogWarning("A smaller MinLifetimeInterval {0}ms than session timeout {1}ms configured for subscription {2}.",
                        MinLifetimeInterval, Session.SessionTimeout, Id);
                }

                var minLifetimeCount = (uint)(MinLifetimeInterval / PublishingInterval);

                if (lifetimeCount < minLifetimeCount)
                {
                    lifetimeCount = minLifetimeCount;

                    if (MinLifetimeInterval % PublishingInterval != 0)
                    {
                        lifetimeCount++;
                    }

                    Utils.LogInfo("Adjusted LifetimeCount to value={0}, for subscription {1}. ",
                        lifetimeCount, Id);
                }

                if (lifetimeCount * PublishingInterval < Session.SessionTimeout)
                {
                    Utils.LogWarning("Lifetime {0}ms configured for subscription {1} is less than session timeout {2}ms.",
                        lifetimeCount * PublishingInterval, Id, Session.SessionTimeout);
                }
            }
            else if (lifetimeCount == 0)
            {
                // don't know what the sampling interval will be - use something large enough
                // to ensure the user does not experience unexpected drop outs.
                Utils.LogInfo("Adjusted LifetimeCount from value={0}, to value={1}, for subscription {2}. ",
                    lifetimeCount, kDefaultLifeTime, Id);
                lifetimeCount = kDefaultLifeTime;
            }

            // validate spec: lifetimecount shall be at least 3*keepAliveCount
            var minLifeTimeCount = 3 * keepAliveCount;
            if (lifetimeCount < minLifeTimeCount)
            {
                Utils.LogInfo("Adjusted LifetimeCount from value={0}, to value={1}, for subscription {2}. ",
                    lifetimeCount, minLifeTimeCount, Id);
                lifetimeCount = minLifeTimeCount;
            }
        }

        /// <summary>
        /// Processes the incoming messages.
        /// </summary>
        private async Task OnMessageReceivedAsync(CancellationToken ct)
        {
            try
            {
                Interlocked.Increment(ref outstandingMessageWorkers_);

                IUaSession session = null;
                uint subscriptionId = 0;
                EventHandler<PublishStateChangedEventArgs> callback = null;

                // list of new messages to process.
                List<NotificationMessage> messagesToProcess = null;

                // list of keep alive messages to process.
                List<IncomingMessage> keepAliveToProcess = null;

                // list of new messages to republish.
                List<IncomingMessage> messagesToRepublish = null;

                PublishStateChangedMask publishStateChangedMask = PublishStateChangedMask.None;

                lock (cache_)
                {
                    if (incomingMessages_ == null)
                    {
                        return;
                    }

                    for (LinkedListNode<IncomingMessage> ii = incomingMessages_.First; ii != null; ii = ii.Next)
                    {
                        // update monitored items with unprocessed messages.
                        if (ii.Value.Message != null && !ii.Value.Processed &&
                            (!sequentialPublishing_ || ValidSequentialPublishMessage(ii.Value)))
                        {
                            if (messagesToProcess == null)
                            {
                                messagesToProcess = new List<NotificationMessage>();
                            }

                            messagesToProcess.Add(ii.Value.Message);

                            // remove the oldest items.
                            while (messageCache_.Count > maxMessageCount_)
                            {
                                messageCache_.RemoveFirst();
                            }

                            messageCache_.AddLast(ii.Value.Message);
                            ii.Value.Processed = true;

                            // Keep the last sequence number processed going up
                            if (ii.Value.SequenceNumber > lastSequenceNumberProcessed_ ||
                               (ii.Value.SequenceNumber == 1 && lastSequenceNumberProcessed_ == uint.MaxValue))
                            {
                                lastSequenceNumberProcessed_ = ii.Value.SequenceNumber;
                                if (resyncLastSequenceNumberProcessed_)
                                {
                                    Utils.LogInfo("SubscriptionId {0}: Resynced last sequence number processed to {1}.",
                                        Id, lastSequenceNumberProcessed_);
                                    resyncLastSequenceNumberProcessed_ = false;
                                }
                            }
                        }

                        // process keep alive messages
                        else if (ii.Next == null && ii.Value.Message == null && !ii.Value.Processed)
                        {
                            if (keepAliveToProcess == null)
                            {
                                keepAliveToProcess = new List<IncomingMessage>();
                            }
                            keepAliveToProcess.Add(ii.Value);
                            publishStateChangedMask |= PublishStateChangedMask.KeepAlive;
                        }

                        // check for missing messages.
                        else if (ii.Next != null && ii.Value.Message == null && !ii.Value.Processed && !ii.Value.Republished)
                        {
                            if (ii.Value.Timestamp.AddSeconds(2) < DateTime.UtcNow)
                            {
                                if (messagesToRepublish == null)
                                {
                                    messagesToRepublish = new List<IncomingMessage>();
                                }

                                messagesToRepublish.Add(ii.Value);
                                ii.Value.Republished = true;
                                publishStateChangedMask |= PublishStateChangedMask.Republish;
                            }
                        }
#if DEBUG
                        // a message that is deferred because of a missing sequence number
                        else if (ii.Value.Message != null && !ii.Value.Processed)
                        {
                            Utils.LogDebug("Subscription {0}: Delayed message with sequence number {1}, expected sequence number is {2}.",
                                Id, ii.Value.SequenceNumber, lastSequenceNumberProcessed_ + 1);
                        }
#endif
                    }

                    session = Session;
                    subscriptionId = Id;
                    callback = PublishStatusChangedEventHandler;
                }

                // process new keep alive messages.
                FastKeepAliveNotificationEventHandler keepAliveCallback = FastKeepAliveCallback;
                if (keepAliveToProcess != null && keepAliveCallback != null)
                {
                    foreach (IncomingMessage message in keepAliveToProcess)
                    {
                        var keepAlive = new NotificationData {
                            PublishTime = message.Timestamp,
                            SequenceNumber = message.SequenceNumber
                        };
                        keepAliveCallback(this, keepAlive);
                    }
                }

                // process new messages.
                if (messagesToProcess != null)
                {
                    int noNotificationsReceived;
                    FastDataChangeNotificationEventHandler datachangeCallback = FastDataChangeCallback;
                    FastEventNotificationEventHandler eventCallback = FastEventCallback;

                    foreach (NotificationMessage message in messagesToProcess)
                    {
                        noNotificationsReceived = 0;
                        try
                        {
                            foreach (ExtensionObject notificationData in message.NotificationData)
                            {
                                if (notificationData.Body is DataChangeNotification datachange)
                                {
                                    datachange.PublishTime = message.PublishTime;
                                    datachange.SequenceNumber = message.SequenceNumber;

                                    noNotificationsReceived += datachange.MonitoredItems.Count;

                                    if (!DisableMonitoredItemCache)
                                    {
                                        SaveDataChange(message, datachange, message.StringTable);
                                    }

                                    if (datachangeCallback != null)
                                    {
                                        datachangeCallback(this, datachange, message.StringTable);
                                    }
                                }


                                if (notificationData.Body is EventNotificationList events)
                                {
                                    events.PublishTime = message.PublishTime;
                                    events.SequenceNumber = message.SequenceNumber;

                                    noNotificationsReceived += events.Events.Count;

                                    if (!DisableMonitoredItemCache)
                                    {
                                        SaveEvents(message, events, message.StringTable);
                                    }

                                    if (eventCallback != null)
                                    {
                                        eventCallback(this, events, message.StringTable);
                                    }
                                }


                                if (notificationData.Body is StatusChangeNotification statusChanged)
                                {
                                    statusChanged.PublishTime = message.PublishTime;
                                    statusChanged.SequenceNumber = message.SequenceNumber;

                                    Utils.LogWarning("StatusChangeNotification received with Status = {0} for SubscriptionId={1}.",
                                        statusChanged.Status.ToString(), Id);

                                    if (statusChanged.Status == StatusCodes.GoodSubscriptionTransferred)
                                    {
                                        publishStateChangedMask |= PublishStateChangedMask.Transferred;
                                        ResetPublishTimerAndWorkerState();
                                    }
                                    else if (statusChanged.Status == StatusCodes.BadTimeout)
                                    {
                                        publishStateChangedMask |= PublishStateChangedMask.Timeout;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e, "Error while processing incoming message #{0}.", message.SequenceNumber);
                        }

                        if (MaxNotificationsPerPublish != 0 && noNotificationsReceived > MaxNotificationsPerPublish)
                        {
                            Utils.LogWarning("For subscription {0}, more notifications were received={1} than the max notifications per publish value={2}",
                                Id, noNotificationsReceived, MaxNotificationsPerPublish);
                        }
                    }
                    if ((callback != null) && (publishStateChangedMask != PublishStateChangedMask.None))
                    {
                        try
                        {
                            callback(this, new PublishStateChangedEventArgs(publishStateChangedMask));
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e, "Error while raising PublishStateChanged event.");
                        }
                    }
                }

                // do any re-publishes.
                if (messagesToRepublish != null && session != null && subscriptionId != 0)
                {
                    for (var ii = 0; ii < messagesToRepublish.Count; ii++)
                    {
                        (var success, _) = await session.RepublishAsync(subscriptionId, messagesToRepublish[ii].SequenceNumber, ct).ConfigureAwait(false);
                        if (!success)
                        {
                            messagesToRepublish[ii].Republished = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Error while processing incoming messages.");
            }
            finally
            {
                Interlocked.Decrement(ref outstandingMessageWorkers_);
            }
        }

        /// <summary>
        /// Throws an exception if the subscription is not in the correct state.
        /// </summary>
        private void VerifySubscriptionState(bool created)
        {
            if (created && Id == 0)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState, "Subscription has not been created.");
            }

            if (!created && Id != 0)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState, "Subscription has already been created.");
            }

            if (!created && Session is null) // Occurs only on Create() and CreateAsync()
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState, "Subscription has not been assigned to a Session");
            }
        }

        /// <summary>
        /// Validates the sequence number of the incoming publish request.
        /// </summary>
        private bool ValidSequentialPublishMessage(IncomingMessage message)
        {
            // If sequential publishing is enabled, only release messages in perfect sequence. 
            return message.SequenceNumber <= lastSequenceNumberProcessed_ + 1 ||
                // reconnect / transfer subscription case
                resyncLastSequenceNumberProcessed_ ||
                // release the first message after wrapping around.
                message.SequenceNumber == 1 && lastSequenceNumberProcessed_ == uint.MaxValue;
        }

        /// <summary>
        /// Update the results to monitored items
        /// after updating the monitoring mode.
        /// </summary>
        private bool UpdateMonitoringMode(
            IList<MonitoredItem> monitoredItems,
            List<ServiceResult> errors,
            StatusCodeCollection results,
            DiagnosticInfoCollection diagnosticInfos,
            ResponseHeader responseHeader,
            MonitoringMode monitoringMode)
        {
            // update results.
            var noErrors = true;

            for (var ii = 0; ii < results.Count; ii++)
            {
                ServiceResult error = null;

                if (StatusCode.IsBad(results[ii]))
                {
                    error = ClientBase.GetResult(results[ii], ii, diagnosticInfos, responseHeader);
                    noErrors = false;
                }
                else
                {
                    monitoredItems[ii].MonitoringMode = monitoringMode;
                    monitoredItems[ii].Status.SetMonitoringMode(monitoringMode);
                }

                errors.Add(error);
            }

            return noErrors;
        }

        /// <summary>
        /// Prepare the creation requests for all monitored items that have not yet been created.
        /// </summary>
        private MonitoredItemCreateRequestCollection PrepareItemsToCreate(out List<MonitoredItem> itemsToCreate)
        {
            VerifySubscriptionState(true);

            ResolveItemNodeIds();

            var requestItems = new MonitoredItemCreateRequestCollection();
            itemsToCreate = new List<MonitoredItem>();

            lock (cache_)
            {
                foreach (MonitoredItem monitoredItem in monitoredItems_.Values)
                {
                    // ignore items that have been created.
                    if (monitoredItem.Status.Created)
                    {
                        continue;
                    }

                    // build item request.
                    var request = new MonitoredItemCreateRequest();

                    request.ItemToMonitor.NodeId = monitoredItem.ResolvedNodeId;
                    request.ItemToMonitor.AttributeId = monitoredItem.AttributeId;
                    request.ItemToMonitor.IndexRange = monitoredItem.IndexRange;
                    request.ItemToMonitor.DataEncoding = monitoredItem.Encoding;

                    request.MonitoringMode = monitoredItem.MonitoringMode;

                    request.RequestedParameters.ClientHandle = monitoredItem.ClientHandle;
                    request.RequestedParameters.SamplingInterval = monitoredItem.SamplingInterval;
                    request.RequestedParameters.QueueSize = monitoredItem.QueueSize;
                    request.RequestedParameters.DiscardOldest = monitoredItem.DiscardOldest;

                    if (monitoredItem.Filter != null)
                    {
                        request.RequestedParameters.Filter = new ExtensionObject(monitoredItem.Filter);
                    }

                    requestItems.Add(request);
                    itemsToCreate.Add(monitoredItem);
                }
            }
            return requestItems;
        }

        /// <summary>
        /// Prepare the modify requests for all monitored items
        /// that need modification.
        /// </summary>
        private void PrepareItemsToModify(
            MonitoredItemModifyRequestCollection requestItems,
            List<MonitoredItem> itemsToModify)
        {
            lock (cache_)
            {
                foreach (MonitoredItem monitoredItem in monitoredItems_.Values)
                {
                    // ignore items that have been created or modified.
                    if (!monitoredItem.Status.Created || !monitoredItem.AttributesModified)
                    {
                        continue;
                    }

                    // build item request.
                    var request = new MonitoredItemModifyRequest();

                    request.MonitoredItemId = monitoredItem.Status.Id;
                    request.RequestedParameters.ClientHandle = monitoredItem.ClientHandle;
                    request.RequestedParameters.SamplingInterval = monitoredItem.SamplingInterval;
                    request.RequestedParameters.QueueSize = monitoredItem.QueueSize;
                    request.RequestedParameters.DiscardOldest = monitoredItem.DiscardOldest;

                    if (monitoredItem.Filter != null)
                    {
                        request.RequestedParameters.Filter = new ExtensionObject(monitoredItem.Filter);
                    }

                    requestItems.Add(request);
                    itemsToModify.Add(monitoredItem);
                }
            }
        }

        /// <summary>
        /// Transfer all monitored items and prepares the modify
        /// requests if transfer of client handles is not possible.
        /// </summary>
        private void TransferItems(
            UInt32Collection serverHandles,
            UInt32Collection clientHandles,
            out IList<MonitoredItem> itemsToModify)
        {
            lock (cache_)
            {
                itemsToModify = new List<MonitoredItem>();
                var updatedMonitoredItems = new SortedDictionary<uint, MonitoredItem>();
                foreach (MonitoredItem monitoredItem in monitoredItems_.Values)
                {
                    var index = serverHandles.FindIndex(handle => handle == monitoredItem.Status.Id);
                    if (index >= 0 && index < clientHandles.Count)
                    {
                        var clientHandle = clientHandles[index];
                        updatedMonitoredItems[clientHandle] = monitoredItem;
                        monitoredItem.SetTransferResult(clientHandle);
                    }
                    else
                    {
                        // modify client handle on server
                        updatedMonitoredItems[monitoredItem.ClientHandle] = monitoredItem;
                        itemsToModify.Add(monitoredItem);
                    }
                }
                monitoredItems_ = updatedMonitoredItems;
            }
        }

        /// <summary>
        /// Prepare the ResolveItem to NodeId service call.
        /// </summary>
        private void PrepareResolveItemNodeIds(
            BrowsePathCollection browsePaths,
            List<MonitoredItem> itemsToBrowse)
        {
            lock (cache_)
            {
                foreach (MonitoredItem monitoredItem in monitoredItems_.Values)
                {
                    if (!String.IsNullOrEmpty(monitoredItem.RelativePath) && NodeId.IsNull(monitoredItem.ResolvedNodeId))
                    {
                        // cannot change the relative path after an item is created.
                        if (monitoredItem.Created)
                        {
                            throw new ServiceResultException(StatusCodes.BadInvalidState, "Cannot modify item path after it is created.");
                        }

                        var browsePath = new BrowsePath();

                        browsePath.StartingNode = monitoredItem.StartNodeId;

                        // parse the relative path.
                        try
                        {
                            browsePath.RelativePath = RelativePath.Parse(monitoredItem.RelativePath, Session.TypeTree);
                        }
                        catch (Exception e)
                        {
                            monitoredItem.SetError(new ServiceResult(e));
                            continue;
                        }

                        browsePaths.Add(browsePath);
                        itemsToBrowse.Add(monitoredItem);
                    }
                }
            }
        }

        /// <summary>
        /// Saves a data change in the monitored item cache.
        /// </summary>
        private void SaveDataChange(NotificationMessage message, DataChangeNotification notifications, IList<string> stringTable)
        {
            // check for empty monitored items list.
            if (notifications.MonitoredItems == null || notifications.MonitoredItems.Count == 0)
            {
                Utils.LogInfo("Publish response contains empty MonitoredItems list for SubscriptionId = {0}.", Id);
                return;
            }

            for (var ii = 0; ii < notifications.MonitoredItems.Count; ii++)
            {
                MonitoredItemNotification notification = notifications.MonitoredItems[ii];

                // lookup monitored item,
                MonitoredItem monitoredItem = null;

                lock (cache_)
                {
                    if (!monitoredItems_.TryGetValue(notification.ClientHandle, out monitoredItem))
                    {
                        Utils.LogWarning("Publish response contains invalid MonitoredItem. SubscriptionId = {0}, ClientHandle = {1}", Id, notification.ClientHandle);
                        continue;
                    }
                }

                // save the message.
                notification.Message = message;

                // get diagnostic info.
                if (notifications.DiagnosticInfos.Count > ii)
                {
                    notification.DiagnosticInfo = notifications.DiagnosticInfos[ii];
                }

                // save in cache.
                monitoredItem.SaveValueInCache(notification);
            }
        }

        /// <summary>
        /// Saves events in the monitored item cache.
        /// </summary>
        private void SaveEvents(NotificationMessage message, EventNotificationList notifications, IList<string> stringTable)
        {
            foreach (EventFieldList eventFields in notifications.Events)
            {
                MonitoredItem monitoredItem;

                lock (cache_)
                {
                    if (!monitoredItems_.TryGetValue(eventFields.ClientHandle, out monitoredItem))
                    {
                        Utils.LogWarning("Publish response contains invalid MonitoredItem.SubscriptionId = {0}, ClientHandle = {1}", Id, eventFields.ClientHandle);
                        continue;
                    }
                }

                // save the message.
                eventFields.Message = message;

                // save in cache.
                monitoredItem.SaveValueInCache(eventFields);
            }
        }

        /// <summary>
        /// Find or create an entry for the incoming sequence number.
        /// </summary>
        /// <param name="utcNow">The current Utc time.</param>
        /// <param name="sequenceNumber">The sequence number for the new entry.</param>
        private IncomingMessage FindOrCreateEntry(DateTime utcNow, uint sequenceNumber)
        {
            IncomingMessage entry = null;
            LinkedListNode<IncomingMessage> node = incomingMessages_.Last;

            while (node != null)
            {
                entry = node.Value;
                LinkedListNode<IncomingMessage> previous = node.Previous;

                if (entry.SequenceNumber == sequenceNumber)
                {
                    entry.Timestamp = utcNow;
                    break;
                }

                if (entry.SequenceNumber < sequenceNumber)
                {
                    entry = new IncomingMessage();
                    entry.SequenceNumber = sequenceNumber;
                    entry.Timestamp = utcNow;
                    _ = incomingMessages_.AddAfter(node, entry);
                    break;
                }

                node = previous;
                entry = null;
            }

            if (entry == null)
            {
                entry = new IncomingMessage();
                entry.SequenceNumber = sequenceNumber;
                entry.Timestamp = utcNow;
                _ = incomingMessages_.AddLast(entry);
            }

            return entry;
        }
        #endregion

        #region Private Fields
        private List<MonitoredItem> deletedItems_;
        private event EventHandler<SubscriptionStatusChangedEventArgs> StateChanged;

        private SubscriptionChangeMask changeMask_;
#if NET6_0_OR_GREATER
        private PeriodicTimer publishTimer_;
#else
        private Timer publishTimer_;
#endif
        private long lastNotificationTime_;
        private int m_keepAliveInterval;
        private int publishLateCount_;
        private event EventHandler<PublishStateChangedEventArgs> PublishStatusChangedEventHandler;

        private object cache_ = new object();
        private LinkedList<NotificationMessage> messageCache_;
        private IList<uint> availableSequenceNumbers_;
        private int maxMessageCount_;
        private bool republishAfterTransfer_;
        private SortedDictionary<uint, MonitoredItem> monitoredItems_;
        private int outstandingMessageWorkers_;
        private AsyncAutoResetEvent messageWorkerEvent_;
        private CancellationTokenSource m_messageWorkerCts;
        private Task messageWorkerTask_;
        private bool sequentialPublishing_;
        private uint lastSequenceNumberProcessed_;
        private bool resyncLastSequenceNumberProcessed_;

        /// <summary>
        /// A message received from the server cached until is processed or discarded.
        /// </summary>
        private class IncomingMessage
        {
            public uint SequenceNumber;
            public DateTime Timestamp;
            public NotificationMessage Message;
            public bool Processed;
            public bool Republished;
        }

        private LinkedList<IncomingMessage> incomingMessages_;

        private static long globalSubscriptionCounter_;
        #endregion
    }

    #region SubscriptionChangeMask Enumeration
    /// <summary>
    /// Flags indicating what has changed in a subscription.
    /// </summary>
    [Flags]
    public enum SubscriptionChangeMask
    {
        /// <summary>
        /// The subscription has not changed.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The subscription was created on the server.
        /// </summary>
        Created = 0x01,

        /// <summary>
        /// The subscription was deleted on the server.
        /// </summary>
        Deleted = 0x02,

        /// <summary>
        /// The subscription was modified on the server.
        /// </summary>
        Modified = 0x04,

        /// <summary>
        /// Monitored items were added to the subscription (but not created on the server)
        /// </summary>
        ItemsAdded = 0x08,

        /// <summary>
        /// Monitored items were removed to the subscription (but not deleted on the server)
        /// </summary>
        ItemsRemoved = 0x10,

        /// <summary>
        /// Monitored items were created on the server.
        /// </summary>
        ItemsCreated = 0x20,

        /// <summary>
        /// Monitored items were deleted on the server.
        /// </summary>
        ItemsDeleted = 0x40,

        /// <summary>
        /// Monitored items were modified on the server.
        /// </summary>
        ItemsModified = 0x80,

        /// <summary>
        /// Subscription was transferred on the server.
        /// </summary>
        Transferred = 0x100

    }
    #endregion

    #region PublishStateChangeMask Enumeration
    /// <summary>
    /// Flags indicating what has changed in a publish state change.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), Flags]
    public enum PublishStateChangedMask
    {
        /// <summary>
        /// The publish state has not changed.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// The publishing stopped.
        /// </summary>
        Stopped = 0x01,

        /// <summary>
        /// The publishing recovered.
        /// </summary>
        Recovered = 0x02,

        /// <summary>
        /// A keep alive message was received.
        /// </summary>
        KeepAlive = 0x04,

        /// <summary>
        /// A republish for a missing message was issued.
        /// </summary>
        Republish = 0x08,

        /// <summary>
        /// The publishing was transferred to another node.
        /// </summary>
        Transferred = 0x10,

        /// <summary>
        /// The publishing was timed out
        /// </summary>
        Timeout = 0x20,
    }
    #endregion

    /// <summary>
    /// The delegate used to receive data change notifications via a direct function call instead of a .NET Event.
    /// </summary>
    public delegate void FastDataChangeNotificationEventHandler(Subscription subscription, DataChangeNotification notification, IList<string> stringTable);

    /// <summary>
    /// The delegate used to receive event notifications via a direct function call instead of a .NET Event.
    /// </summary>
    public delegate void FastEventNotificationEventHandler(Subscription subscription, EventNotificationList notification, IList<string> stringTable);

    /// <summary>
    /// The delegate used to receive keep alive notifications via a direct function call instead of a .NET Event.
    /// </summary>
    public delegate void FastKeepAliveNotificationEventHandler(Subscription subscription, NotificationData notification);

    #region SubscriptionStatusChangedEventArgs Class
    /// <summary>
    /// The event arguments provided when the state of a subscription changes.
    /// </summary>
    public class SubscriptionStatusChangedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal SubscriptionStatusChangedEventArgs(SubscriptionChangeMask changeMask)
        {
            Status = changeMask;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The changes that have affected the subscription.
        /// </summary>
        public SubscriptionChangeMask Status { get; }

        #endregion

        #region Private Fields

        #endregion
    }
    #endregion

    #region PublishStateChangedEventArgs Class
    /// <summary>
    /// The event arguments provided when the state of a subscription changes.
    /// </summary>
    public class PublishStateChangedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal PublishStateChangedEventArgs(PublishStateChangedMask changeMask)
        {
            Status = changeMask;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The publish state changes.
        /// </summary>
        public PublishStateChangedMask Status { get; }
        #endregion

        #region Private Fields
        #endregion
    }
    #endregion

    /// <summary>
    /// A collection of subscriptions.
    /// </summary>
    [CollectionDataContract(Name = "ListOfSubscription", Namespace = Namespaces.OpcUaXsd, ItemName = "Subscription")]
    public partial class SubscriptionCollection : List<Subscription>, ICloneable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes an empty collection.
        /// </summary>
        public SubscriptionCollection() { }

        /// <summary>
        /// Initializes the collection from another collection.
        /// </summary>
        /// <param name="collection">The existing collection to use as the basis of creating this collection</param>
        public SubscriptionCollection(IEnumerable<Subscription> collection) : base(collection) { }

        /// <summary>
        /// Initializes the collection with the specified capacity.
        /// </summary>
        /// <param name="capacity">The max. capacity of the collection</param>
        public SubscriptionCollection(int capacity) : base(capacity) { }
        #endregion

        #region ICloneable Members
        /// <summary cref="ICloneable.Clone" />
        public virtual object Clone()
        {
            return (SubscriptionCollection)MemberwiseClone();
        }

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            var clone = new SubscriptionCollection();
            clone.AddRange(this.Select(item => (Subscription)item.Clone()));
            return clone;
        }

        /// <summary>
        /// Helper to clone a SubscriptionCollection with event handlers using the
        /// <see cref="Subscription.CloneSubscription(bool)"/> method.
        /// </summary>
        public virtual SubscriptionCollection CloneSubscriptions(bool copyEventhandlers)
        {
            var clone = new SubscriptionCollection();
            clone.AddRange(this.Select(item => (Subscription)item.CloneSubscription(copyEventhandlers)));
            return clone;
        }
        #endregion
    }
}
