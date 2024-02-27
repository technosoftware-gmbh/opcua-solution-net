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
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Linq;

using Opc.Ua;

using Technosoftware.UaServer.Sessions;
#endregion

namespace Technosoftware.UaServer.Subscriptions 
{
    /// <summary>
    /// Manages a subscription created by a client.
    /// </summary>
    public class Subscription : IUaSubscription, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object.
        /// </summary>
        public Subscription(
            IUaServerData  server,
            Sessions.Session session,
            uint             subscriptionId,
            double           publishingInterval,
            uint             maxLifetimeCount,
            uint             maxKeepAliveCount,
            uint             maxNotificationsPerPublish,
            byte             priority,
            bool             publishingEnabled,
            uint             maxMessageCount)
        {
            if (server == null)  throw new ArgumentNullException(nameof(server));
            if (session == null) throw new ArgumentNullException(nameof(session));

            server_                      = server;
            session_                     = session;
            id_                          = subscriptionId;
            publishingInterval_          = publishingInterval;
            maxLifetimeCount_            = maxLifetimeCount;
            maxKeepAliveCount_           = maxKeepAliveCount;
            maxNotificationsPerPublish_  = maxNotificationsPerPublish;
            publishingEnabled_           = publishingEnabled;
            priority_                    = priority;
            publishTimerExpiry_          = HiResClock.TickCount64 + (long)publishingInterval;
            keepAliveCounter_            = maxKeepAliveCount;
            lifetimeCounter_             = 0;
            waitingForPublish_           = false;
            maxMessageCount_             = maxMessageCount;
            sentMessages_                = new List<NotificationMessage>();

            monitoredItems_ = new Dictionary<uint, LinkedListNode<IUaMonitoredItem>>();
            itemsToCheck_                = new LinkedList<IUaMonitoredItem>();
            itemsToPublish_              = new LinkedList<IUaMonitoredItem>();
            itemsToTrigger_ = new Dictionary<uint, List<IUaTriggeredMonitoredItem>>();

            sequenceNumber_              = 1;

            // initialize diagnostics.
            diagnostics_ = new SubscriptionDiagnosticsDataType();

            diagnostics_.SessionId = session_.Id;
            diagnostics_.SubscriptionId = id_;
            diagnostics_.Priority = priority;
            diagnostics_.PublishingInterval = publishingInterval;
            diagnostics_.MaxKeepAliveCount = maxKeepAliveCount;
            diagnostics_.MaxLifetimeCount = maxLifetimeCount;
            diagnostics_.MaxNotificationsPerPublish = maxNotificationsPerPublish;
            diagnostics_.PublishingEnabled = publishingEnabled;
            diagnostics_.ModifyCount = 0;
            diagnostics_.EnableCount = 0;
            diagnostics_.DisableCount = 0;
            diagnostics_.RepublishMessageRequestCount = 0;
            diagnostics_.RepublishMessageCount = 0;
            diagnostics_.TransferRequestCount = 0;
            diagnostics_.TransferredToSameClientCount = 0;
            diagnostics_.TransferredToAltClientCount = 0;
            diagnostics_.PublishRequestCount = 0;
            diagnostics_.DataChangeNotificationsCount = 0;
            diagnostics_.EventNotificationsCount = 0;
            diagnostics_.NotificationsCount = 0;
            diagnostics_.LatePublishRequestCount = 0;
            diagnostics_.CurrentKeepAliveCount = 0;
            diagnostics_.CurrentLifetimeCount = 0;
            diagnostics_.UnacknowledgedMessageCount = 0;
            diagnostics_.DiscardedMessageCount = 0;
            diagnostics_.MonitoredItemCount = 0;
            diagnostics_.DisabledMonitoredItemCount = 0;
            diagnostics_.MonitoringQueueOverflowCount = 0;
            diagnostics_.NextSequenceNumber = (uint)sequenceNumber_;

            var systemContext = server_.DefaultSystemContext.Copy(session);

            diagnosticsId_ = server.DiagnosticsNodeManager.CreateSubscriptionDiagnostics(
                systemContext,
                diagnostics_,
                OnUpdateDiagnostics);

            TraceState(LogLevel.Information, TraceStateId.Config, "CREATED");
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
                lock (lock_)
                {
                    monitoredItems_.Clear();
                    sentMessages_.Clear();
                    itemsToCheck_.Clear();
                    itemsToPublish_.Clear();
                }
            }
        }
        #endregion

        #region IUaSubscription Members
        /// <summary>
        /// The session that owns the monitored item.
        /// </summary>
        public Sessions.Session Session
        {
            get { return session_; }
        }

        /// <summary>
        /// The unique identifier assigned to the subscription.
        /// </summary>
        public uint Id
        {
            get { return id_; }
        }

        /// <summary>
        /// Queues an item that is ready to publish.
        /// </summary>
        public void ItemReadyToPublish(IUaMonitoredItem monitoredItem)
        {
            /*
            lock (itemsReadyToPublish_)
            {
                itemsReadyToPublish_.Enqueue(monitoredItem);
            }
            */
        }

        /// <summary>
        /// Tells the subscription that notifications are available but the item is not ready to publish.
        /// </summary>
        public void ItemNotificationsAvailable(IUaMonitoredItem monitoredItem)
        {
            /*
            lock (itemsReadyToPublish_)
            {
                itemsNotificationsAvailable_.AddLast(monitoredItem);
            }
            */
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// The identifier for the session that owns the subscription.
        /// </summary>
        public NodeId SessionId
        {
            get
            {
                lock (lock_)
                {
                    if (session_ == null)
                    {
                        return null;
                    }

                    return session_.Id;
                }
            }
        }

        /// <summary>
        /// The owner identity.
        /// </summary>
        public UserIdentityToken OwnerIdentity
        {
            get { return (session_ != null) ? session_.IdentityToken : savedOwnerIdentity_; }
        }

        /// <summary>
        /// Gets the lock that must be acquired before accessing the contents of the Diagnostics property.
        /// </summary>
        public object DiagnosticsLock
        {
            get
            {
                return diagnostics_;
            }
        }

        /// <summary>
        /// Gets the lock that must be acquired before updating the contents of the Diagnostics property.
        /// </summary>
        public object DiagnosticsWriteLock
        {
            get
            {
                // mark diagnostic nodes dirty
                if (server_ != null && server_.DiagnosticsNodeManager != null)
                {
                    server_.DiagnosticsNodeManager.ForceDiagnosticsScan();
                }
                return DiagnosticsLock;
            }
        }

        /// <summary>
        /// Gets the current diagnostics for the subscription.
        /// </summary>
        public SubscriptionDiagnosticsDataType Diagnostics
        {
            get
            {
                return diagnostics_;
            }
        }

        /// <summary>
        /// The publishing rate for the subscription.
        /// </summary>
        public double PublishingInterval
        {
            get
            {
                lock (lock_)
                {
                    return publishingInterval_;
                }
            }
        }

        /// <summary>
        /// The number of monitored items.
        /// </summary>
        public int MonitoredItemCount
        {
            get
            {
                lock (lock_)
                {
                    return monitoredItems_.Count;
                }
            }
        }

        /// <summary>
        /// The priority assigned to the subscription.
        /// </summary>
        public byte Priority
        {
            get
            {
                return priority_;
            }
        }

        /// <summary>
        /// Deletes the subscription.
        /// </summary>
        public void Delete(UaServerOperationContext context)
        {
            // delete the diagnostics.
            if (diagnosticsId_ != null && !diagnosticsId_.IsNullNodeId)
            {
                var systemContext = server_.DefaultSystemContext.Copy(session_);
                server_.DiagnosticsNodeManager.DeleteSubscriptionDiagnostics(systemContext, diagnosticsId_);
            }

            lock (lock_)
            {
                try
                {
                    TraceState(LogLevel.Information, TraceStateId.Deleted, "DELETED");

                    // the context may be null if the server is cleaning up expired subscriptions.
                    // in this case we create a context with a dummy request and use the current session.
                    if (context == null)
                    {
                        var requestHeader = new RequestHeader();
                        requestHeader.ReturnDiagnostics = (uint)(int)DiagnosticsMasks.OperationSymbolicIdAndText;
                        context = new UaServerOperationContext(requestHeader, RequestType.Unknown);
                    }

                    StatusCodeCollection results;
                    DiagnosticInfoCollection diagnosticInfos;

                    DeleteMonitoredItems(
                        context,
                        new UInt32Collection(monitoredItems_.Keys),
                        true,
                        out results,
                        out diagnosticInfos);
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Delete items for subscription failed.");
                }
            }
        }

        /// <summary>
        /// Checks if the subscription is ready to publish.
        /// </summary>
        public UaPublishingState PublishTimerExpired()
        {
            lock (lock_)
            {
                long currentTime = HiResClock.TickCount64;

                // check if publish interval has elapsed.
                if (publishTimerExpiry_ >= currentTime)
                {
                    // check if waiting for publish.
                    if (waitingForPublish_)
                    {
                        return UaPublishingState.WaitingForPublish;
                    }

                    return UaPublishingState.Idle;
                }

                // set next expiry time.
                while (publishTimerExpiry_ < currentTime)
                {
                    publishTimerExpiry_ += (long)publishingInterval_;
                }

                // check lifetime has elapsed.
                if (waitingForPublish_)
                {
                    lifetimeCounter_++;

                    lock (DiagnosticsWriteLock)
                    {
                        diagnostics_.LatePublishRequestCount++;
                        diagnostics_.CurrentLifetimeCount = lifetimeCounter_;
                    }

                    if (lifetimeCounter_ >= maxLifetimeCount_)
                    {
                        TraceState(LogLevel.Information, TraceStateId.Deleted, "EXPIRED");
                        return UaPublishingState.Expired;
                    }
                }

                // increment keep alive counter.
                keepAliveCounter_++;

                lock (DiagnosticsWriteLock)
                {
                    diagnostics_.CurrentKeepAliveCount = keepAliveCounter_;
                }

                // check for monitored items.
                if (publishingEnabled_ && session_ != null)
                {
                    // check for monitored items that are ready to publish.
                    var current = itemsToCheck_.First;
                    var itemsTriggered = false;

                    while (current != null)
                    {
                        var next = current.Next;
                        var monitoredItem = current.Value;

                        // check if the item is ready to publish.
                        if (monitoredItem.IsReadyToPublish || monitoredItem.IsResendData)
                        {
                            itemsToCheck_.Remove(current);
                            itemsToPublish_.AddLast(current);
                        }

                        // update any triggered items.
                        List<IUaTriggeredMonitoredItem> triggeredItems = null;

                        if (monitoredItem.IsReadyToTrigger)
                        {
                            if (itemsToTrigger_.TryGetValue(current.Value.Id, out triggeredItems))
                            {
                                for (var ii = 0; ii < triggeredItems.Count; ii++)
                                {
                                    if (triggeredItems[ii].SetTriggered())
                                    {
                                        itemsTriggered = true;
                                    }
                                }

                                // clear ReadyToTrigger flag after trigger
                                monitoredItem.IsReadyToTrigger = false;
                            }
                        }

                        current = next;
                    }

                    // need to go through the list again if items were triggered.
                    if (itemsTriggered)
                    {
                        current = itemsToCheck_.First;

                        while (current != null)
                        {
                            var next = current.Next;
                            var monitoredItem = current.Value;

                            if (monitoredItem.IsReadyToPublish)
                            {
                                itemsToCheck_.Remove(current);
                                itemsToPublish_.AddLast(current);
                            }

                            current = next;
                        }
                    }

                    if (itemsToPublish_.Count > 0)
                    {
                        if (!waitingForPublish_)
                        {
                            // TraceState(LogLevel.Trace, TraceStateId.Deleted, "READY TO PUBLISH");
                        }

                        waitingForPublish_ = true;
                        return UaPublishingState.NotificationsAvailable;
                    }
                }

                // check if keep alive expired.
                if (keepAliveCounter_ >= maxKeepAliveCount_)
                {
                    if (!waitingForPublish_)
                    {
                        // TraceState(LogLevel.Trace, TraceStateId.Items, "READY TO KEEPALIVE");
                    }

                    waitingForPublish_ = true;
                    return UaPublishingState.NotificationsAvailable;
                }

                // do nothing.
                return UaPublishingState.Idle;
            }
        }

        /// <summary>
        /// Transfers the subscription to a new session.
        /// </summary>
        /// <param name="context">The session to which the subscription is transferred.</param>
        /// <param name="sendInitialValues">Whether the first Publish response shall contain current values.</param> 
        public void TransferSession(UaServerOperationContext context, bool sendInitialValues)
        {
            // locked by caller
            session_ = context.Session;

            var monitoredItems = monitoredItems_.Select(v => v.Value.Value).ToList();
            var errors = new List<ServiceResult>(monitoredItems.Count);
            for (int ii = 0; ii < monitoredItems.Count; ii++)
            {
                errors.Add(null);
            }

            server_.NodeManager.TransferMonitoredItems(context, sendInitialValues, monitoredItems, errors);

            int badTransfers = 0;
            for (int ii = 0; ii < errors.Count; ii++)
            {
                if (ServiceResult.IsBad(errors[ii]))
                {
                    badTransfers++;
                }
            }

            if (badTransfers > 0)
            {
                Utils.LogTrace("Failed to transfer {0} Monitored Items", badTransfers);
            }

            lock (DiagnosticsWriteLock)
            {
                diagnostics_.SessionId = session_.Id;
            }
        }

        /// <summary>
        /// Initiates resending of all data monitored items in a Subscription
        /// </summary>
        /// <param name="context"></param>
        public void ResendData(UaServerOperationContext context)
        {
            // check session.
            VerifySession(context);
            lock (lock_)
            {
                var monitoredItems = monitoredItems_.Select(v => v.Value.Value).ToList();
                foreach (IUaMonitoredItem monitoredItem in monitoredItems)
                {
                    monitoredItem.SetupResendDataTrigger();
                }
            }
        }

        /// <summary>
        /// Tells the subscription that the owning session is being closed.
        /// </summary>
        public void SessionClosed()
        {
            lock (lock_)
            {
                if (session_ != null)
                {
                    savedOwnerIdentity_ = session_.IdentityToken;
                    session_ = null;
                }
            }

            lock (DiagnosticsWriteLock)
            {
                diagnostics_.SessionId = null;
            }
        }

        /// <summary>
        /// Resets the keepalive counter.
        /// </summary>
        private void ResetKeepaliveCount()
        {
            keepAliveCounter_ = 0;

            lock (DiagnosticsWriteLock)
            {
                diagnostics_.CurrentKeepAliveCount = 0;
            }
        }

        /// <summary>
        /// Resets the lifetime count.
        /// </summary>
        private void ResetLifetimeCount()
        {
            lifetimeCounter_ = 0;

            lock (DiagnosticsWriteLock)
            {
                diagnostics_.CurrentLifetimeCount = 0;
            }
        }

        /// <summary>
        /// Update the monitoring queue overflow count.
        /// </summary>
        public void QueueOverflowHandler()
        {
            lock (DiagnosticsWriteLock)
            {
                diagnostics_.MonitoringQueueOverflowCount++;
            }
        }

        /// <summary>
        /// Removes a message from the message queue.
        /// </summary>
        public ServiceResult Acknowledge(UaServerOperationContext context, uint sequenceNumber)
        {
            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                // find message in queue.
                for (var ii = 0; ii < sentMessages_.Count; ii++)
                {
                    if (sentMessages_[ii].SequenceNumber == sequenceNumber)
                    {
                        if (lastSentMessage_ > ii)
                        {
                            lastSentMessage_--;
                        }

                        sentMessages_.RemoveAt(ii);
                        return null;
                    }
                }

                if (sequenceNumber == 0)
                {
                    return StatusCodes.BadSequenceNumberInvalid;
                }

                // TraceState(LogLevel.Trace, TraceStateId.Items, "ACK " + sequenceNumber.ToString());

                // message not found.
                return StatusCodes.BadSequenceNumberUnknown;
            }
        }

        /// <summary>
        /// Returns all available notifications.
        /// </summary>
        public NotificationMessage Publish(
            UaServerOperationContext context,
            out UInt32Collection availableSequenceNumbers,
            out bool              moreNotifications)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            NotificationMessage message = null;

            lock (lock_)
            {
                moreNotifications = false;
                availableSequenceNumbers = null;

                // check if expired.
                if (expired_)
                {
                    return null;
                }

                try
                {
                    // update diagnostics.
                    lock (DiagnosticsWriteLock)
                    {
                        diagnostics_.PublishRequestCount++;
                    }

                    message = InnerPublish(context, out availableSequenceNumbers, out moreNotifications);

                    lock (DiagnosticsWriteLock)
                    {
                        diagnostics_.UnacknowledgedMessageCount = (uint)availableSequenceNumbers.Count;
                    }
                }
                finally
                {
                    // clear counters on success.
                    if (message != null)
                    {
                        // TraceState(LogLevel.Trace, TraceStateId.Items, Utils.Format("PUBLISH #{0}", message.SequenceNumber));
                        ResetKeepaliveCount();
                        waitingForPublish_ = moreNotifications;
                        ResetLifetimeCount();
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// Publishes a timeout status message.
        /// </summary>
        public NotificationMessage PublishTimeout()
        {
            NotificationMessage message = null;

            lock (lock_)
            {
                expired_ = true;

                message = new NotificationMessage
                {
                    SequenceNumber = (uint)sequenceNumber_,
                    PublishTime = DateTime.UtcNow
                };

                Utils.IncrementIdentifier(ref sequenceNumber_);

                lock (DiagnosticsWriteLock)
                {
                    diagnostics_.NextSequenceNumber = (uint)sequenceNumber_;
                }

                StatusChangeNotification notification = new StatusChangeNotification
                {
                    Status = StatusCodes.BadTimeout
                };
                message.NotificationData.Add(new ExtensionObject(notification));
            }

            return message;
        }

        /// <summary>
        /// Publishes a SubscriptionTransferred status message.
        /// </summary>
        public NotificationMessage SubscriptionTransferred()
        {
            NotificationMessage message = null;

            lock (lock_)
            {
                message = new NotificationMessage
                {
                    SequenceNumber = (uint)sequenceNumber_,
                    PublishTime = DateTime.UtcNow
                };

                Utils.IncrementIdentifier(ref sequenceNumber_);

                lock (DiagnosticsWriteLock)
                {
                    diagnostics_.NextSequenceNumber = (uint)sequenceNumber_;
                }

                var notification = new StatusChangeNotification
                {
                    Status = StatusCodes.GoodSubscriptionTransferred
                };
                message.NotificationData.Add(new ExtensionObject(notification));
            }

            return message;
        }

        /// <summary>
        /// Returns all available notifications.
        /// </summary>
        private NotificationMessage InnerPublish(
            UaServerOperationContext context,
            out UInt32Collection  availableSequenceNumbers,
            out bool              moreNotifications)
        {
            // check session.
            VerifySession(context);

            // TraceState(LogLevel.Trace, TraceStateId.Items, "PUBLISH");

            // check if a keep alive should be sent if there is no data.
            var keepAliveIfNoData = keepAliveCounter_ >= maxKeepAliveCount_;

            availableSequenceNumbers = new UInt32Collection();

            moreNotifications = false;

            if (lastSentMessage_ < sentMessages_.Count)
            {
                // return the available sequence numbers.
                for (var ii = 0; ii <= lastSentMessage_ && ii < sentMessages_.Count; ii++)
                {
                    availableSequenceNumbers.Add(sentMessages_[ii].SequenceNumber);
                }

                moreNotifications = waitingForPublish_ = lastSentMessage_ < sentMessages_.Count - 1;

                // TraceState(LogLevel.Trace, TraceStateId.Items, "PUBLISH QUEUED MESSAGE");
                return sentMessages_[lastSentMessage_++];
            }

            var messages = new List<NotificationMessage>();

            if (publishingEnabled_)
            {
                var start1 = DateTime.UtcNow;

                // collect notifications to publish.
                var events = new Queue<EventFieldList>();
                var datachanges = new Queue<MonitoredItemNotification>();
                var datachangeDiagnostics = new Queue<DiagnosticInfo>();

                // check for monitored items that are ready to publish.
                var current = itemsToPublish_.First;

                while (current != null)
                {
                    var next = current.Next;
                    var monitoredItem = current.Value;

                    if ((monitoredItem.MonitoredItemType & UaMonitoredItemTypeMask.DataChange) != 0)
                    {
                        ((IUaDataChangeMonitoredItem)monitoredItem).Publish(context, datachanges, datachangeDiagnostics);
                    }
                    else
                    {
                        ((IUaEventMonitoredItem)monitoredItem).Publish(context, events);
                    }

                    // add back to list to check.
                    itemsToPublish_.Remove(current);
                    itemsToCheck_.AddLast(current);

                    // check there are enough notifications for a message.
                    if (maxNotificationsPerPublish_ > 0 && events.Count + datachanges.Count > maxNotificationsPerPublish_)
                    {
                        // construct message.
                        int notificationCount;
                        var eventCount = events.Count;
                        var dataChangeCount = datachanges.Count;

                        var message = ConstructMessage(
                             events,
                             datachanges,
                             datachangeDiagnostics,
                             out notificationCount);

                        // add to list of messages to send.
                        messages.Add(message);

                        lock (DiagnosticsWriteLock)
                        {
                            diagnostics_.DataChangeNotificationsCount += (uint)dataChangeCount;
                            diagnostics_.EventNotificationsCount += (uint)(eventCount - events.Count);
                            diagnostics_.NotificationsCount += (uint)notificationCount;
                        }
                    }

                    current = next;
                }

                // pubish the remaining notifications.
                while (events.Count + datachanges.Count > 0)
                {
                    // construct message.
                    int notificationCount;
                    var eventCount = events.Count;
                    var dataChangeCount = datachanges.Count;

                     var message = ConstructMessage(
                         events,
                         datachanges,
                         datachangeDiagnostics,
                         out notificationCount);

                    // add to list of messages to send.
                    messages.Add(message);

                    lock (DiagnosticsWriteLock)
                    {
                        diagnostics_.DataChangeNotificationsCount += (uint)dataChangeCount;
                        diagnostics_.EventNotificationsCount += (uint)(eventCount - events.Count);
                        diagnostics_.NotificationsCount += (uint)notificationCount;
                    }
                }

                // check for missing notifications.
                if (!keepAliveIfNoData && messages.Count == 0)
                {
                    Utils.LogError("Oops! MonitoredItems queued but no notifications available.");

                    waitingForPublish_ = false;

                    return null;
                }

                var end1 = DateTime.UtcNow;

                var delta1 = ((double)(end1.Ticks - start1.Ticks)) / TimeSpan.TicksPerMillisecond;

                if (delta1 > 200)
                {
                    TraceState(LogLevel.Trace, TraceStateId.Publish, Utils.Format("PUBLISHING DELAY ({0}ms)", delta1));
                }
            }

            if (messages.Count == 0)
            {
                // create a keep alive message.
                var message = new NotificationMessage();

                // use the sequence number for the next message.
                message.SequenceNumber = (uint)sequenceNumber_;
                message.PublishTime    = DateTime.UtcNow;

                // return the available sequence numbers.
                for (var ii = 0; ii <= lastSentMessage_ && ii < sentMessages_.Count; ii++)
                {
                    availableSequenceNumbers.Add(sentMessages_[ii].SequenceNumber);
                }

                // TraceState(LogLevel.Trace, TraceStateId.Items, "PUBLISH KEEPALIVE");
                return message;
            }

            // have to drop unsent messages if out of queue space.
            var overflowCount = messages.Count - (int)maxMessageCount_;
            if (overflowCount > 0)
            {
                Utils.LogWarning(
                    "WARNING: QUEUE OVERFLOW. Dropping {0} Messages. Increase MaxMessageQueueSize. SubId={1}, MaxMessageQueueSize={2}",
                    overflowCount, id_, maxMessageCount_);

                messages.RemoveRange(0, overflowCount);
            }

            // remove old messages if queue is full.
            if (sentMessages_.Count > maxMessageCount_ - messages.Count)
            {
                lock (DiagnosticsWriteLock)
                {
                    diagnostics_.UnacknowledgedMessageCount += (uint)messages.Count;
                }

                if (maxMessageCount_ <= messages.Count)
                {
                    sentMessages_.Clear();
                }
                else
                {
                    sentMessages_.RemoveRange(0, messages.Count);
                }
            }

            // save new message
            lastSentMessage_ = sentMessages_.Count;
            sentMessages_.AddRange(messages);

            // check if there are more notifications to send.
            moreNotifications = waitingForPublish_ = messages.Count > 1;

            // return the available sequence numbers.
            for (var ii = 0; ii <= lastSentMessage_ && ii < sentMessages_.Count; ii++)
            {
                availableSequenceNumbers.Add(sentMessages_[ii].SequenceNumber);
            }

            // TraceState(LogLevel.Trace, TraceStateId.Items, "PUBLISH NEW MESSAGE");
            return sentMessages_[lastSentMessage_++];
        }

        /// <summary>
        /// Returns the available sequence numbers for retransmission
        /// For example used in Transfer Subscription
        /// </summary>
        public UInt32Collection AvailableSequenceNumbersForRetransmission()
        {
            var availableSequenceNumbers = new UInt32Collection();
            // Assumption we do not check lastSentMessage < sentMessages.Count because
            // in case of subscription transfer original client might have crashed by handling message,
            // therefor new client should have to chance to process all available messages
            for (int ii = 0; ii < sentMessages_.Count; ii++)
            {
                availableSequenceNumbers.Add(sentMessages_[ii].SequenceNumber);
            }
            return availableSequenceNumbers;
        }

        /// <summary>
        /// Construct a message from the queues.
        /// </summary>
        private NotificationMessage ConstructMessage(
            Queue<EventFieldList> events,
            Queue<MonitoredItemNotification> datachanges,
            Queue<DiagnosticInfo> datachangeDiagnostics,
            out int notificationCount)
        {
            notificationCount = 0;

            var message = new NotificationMessage();

            message.SequenceNumber = (uint)sequenceNumber_;
            message.PublishTime    = DateTime.UtcNow;

            Utils.IncrementIdentifier(ref sequenceNumber_);

            lock (DiagnosticsWriteLock)
            {
                diagnostics_.NextSequenceNumber = (uint)sequenceNumber_;
            }

            // add events.
            if (events.Count > 0 && notificationCount < maxNotificationsPerPublish_)
            {
                var notification = new EventNotificationList();

                while (events.Count > 0 && notificationCount < maxNotificationsPerPublish_)
                {
                    notification.Events.Add(events.Dequeue());
                    notificationCount++;
                }

                message.NotificationData.Add(new ExtensionObject(notification));
            }

            // add datachanges (space permitting).
            if (datachanges.Count > 0 && notificationCount < maxNotificationsPerPublish_)
            {
                var diagnosticsExist = false;
                var notification = new DataChangeNotification();

                notification.MonitoredItems  = new MonitoredItemNotificationCollection(datachanges.Count);
                notification.DiagnosticInfos = new DiagnosticInfoCollection(datachanges.Count);

                while (datachanges.Count > 0 && notificationCount < maxNotificationsPerPublish_)
                {
                    var datachange = datachanges.Dequeue();
                    notification.MonitoredItems.Add(datachange);

                    var diagnosticInfo = datachangeDiagnostics.Dequeue();

                    if (diagnosticInfo != null)
                    {
                        diagnosticsExist = true;
                    }

                    notification.DiagnosticInfos.Add(diagnosticInfo);

                    notificationCount++;
                }

                // clear diagnostics if not used.
                if (!diagnosticsExist)
                {
                    notification.DiagnosticInfos.Clear();
                }

                message.NotificationData.Add(new ExtensionObject(notification));
            }

            return message;
        }

        /// <summary>
        /// Returns a cached notification message.
        /// </summary>
        public NotificationMessage Republish(
            UaServerOperationContext context,
            uint             retransmitSequenceNumber)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            lock (DiagnosticsWriteLock)
            {
                diagnostics_.RepublishMessageRequestCount++;
            }

            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                lock (DiagnosticsWriteLock)
                {
                    diagnostics_.RepublishRequestCount++;
                    diagnostics_.RepublishMessageRequestCount++;
                }

                // find message.
                foreach (var sentMessage in sentMessages_)
                {
                    if (sentMessage.SequenceNumber == retransmitSequenceNumber)
                    {
                        lock (DiagnosticsWriteLock)
                        {
                            diagnostics_.RepublishMessageCount++;
                        }

                        return sentMessage;
                    }
                }

                // message not available.
                throw new ServiceResultException(StatusCodes.BadMessageNotAvailable);
            }
        }

        /// <summary>
        /// Updates the publishing parameters for the subscription.
        /// </summary>
        public void Modify(
            UaServerOperationContext context,
            double           publishingInterval,
            uint             maxLifetimeCount,
            uint             maxKeepAliveCount,
            uint             maxNotificationsPerPublish,
            byte             priority)
        {
            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                maxLifetimeCount_ = maxLifetimeCount;

                // update publishing interval.
                if (publishingInterval != publishingInterval_)
                {
                    publishingInterval_ = publishingInterval;
                    publishTimerExpiry_ = HiResClock.TickCount64 + (long)publishingInterval;
                    ResetKeepaliveCount();
                }

                // update keep alive count.
                if (maxKeepAliveCount != maxKeepAliveCount_)
                {
                    maxKeepAliveCount_ = maxKeepAliveCount;
                }

                maxNotificationsPerPublish_ = maxNotificationsPerPublish;

                // update priority.
                priority_ = priority;

                // update diagnostics
                lock (DiagnosticsWriteLock)
                {
                    diagnostics_.ModifyCount++;
                    diagnostics_.PublishingInterval = publishingInterval_;
                    diagnostics_.MaxKeepAliveCount = maxKeepAliveCount_;
                    diagnostics_.MaxLifetimeCount = maxLifetimeCount_;
                    diagnostics_.Priority = priority_;
                    diagnostics_.MaxNotificationsPerPublish = maxNotificationsPerPublish_;
                }

                TraceState(LogLevel.Information, TraceStateId.Config, "MODIFIED");
            }
        }

        /// <summary>
        /// Enables/disables publishing for the subscription.
        /// </summary>
        public void SetPublishingMode(
            UaServerOperationContext context,
            bool             publishingEnabled)
        {
            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                // update publishing interval.
                if (publishingEnabled != publishingEnabled_)
                {
                    publishingEnabled_ = publishingEnabled;

                    // update diagnostics
                    lock (DiagnosticsWriteLock)
                    {
                        diagnostics_.PublishingEnabled = publishingEnabled_;

                        if (publishingEnabled_)
                        {
                            diagnostics_.EnableCount++;
                        }
                        else
                        {
                            diagnostics_.DisableCount++;
                        }
                    }
                }

                TraceState(LogLevel.Information, TraceStateId.Config, (publishingEnabled) ? "ENABLED" : "DISABLED");
            }
        }

        /// <summary>
        /// Updates the triggers for the monitored item.
        /// </summary>
        public void SetTriggering(
            UaServerOperationContext context,
            uint triggeringItemId,
            UInt32Collection linksToAdd,
            UInt32Collection linksToRemove,
            out StatusCodeCollection addResults,
            out DiagnosticInfoCollection addDiagnosticInfos,
            out StatusCodeCollection removeResults,
            out DiagnosticInfoCollection removeDiagnosticInfos)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (linksToAdd == null) throw new ArgumentNullException(nameof(linksToAdd));
            if (linksToRemove == null) throw new ArgumentNullException(nameof(linksToRemove));

            // allocate results.
            var diagnosticsExist = false;
            addResults = new StatusCodeCollection();
            addDiagnosticInfos = null;
            removeResults = new StatusCodeCollection();
            removeDiagnosticInfos = null;

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                addDiagnosticInfos = new DiagnosticInfoCollection();
                removeDiagnosticInfos = new DiagnosticInfoCollection();
            }

            // build list of items to modify.
            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                // look up triggering item.
                LinkedListNode<IUaMonitoredItem> triggerNode = null;

                if (!monitoredItems_.TryGetValue(triggeringItemId, out triggerNode))
                {
                    throw new ServiceResultException(StatusCodes.BadMonitoredItemIdInvalid);
                }

                // lookup existing list.
                List<IUaTriggeredMonitoredItem> triggeredItems = null;

                if (!itemsToTrigger_.TryGetValue(triggeringItemId, out triggeredItems))
                {
                    itemsToTrigger_[triggeringItemId] = triggeredItems = new List<IUaTriggeredMonitoredItem>();
                }

                // remove old links.
                for (var ii = 0; ii < linksToRemove.Count; ii++)
                {
                    removeResults.Add(StatusCodes.Good);

                    var found = false;

                    for (var jj = 0; jj < triggeredItems.Count; jj++)
                    {
                        if (triggeredItems[jj].Id == linksToRemove[ii])
                        {
                            found = true;
                            triggeredItems.RemoveAt(jj);
                            break;
                        }
                    }

                    if (!found)
                    {
                        removeResults[ii] = StatusCodes.BadMonitoredItemIdInvalid;

                        // update diagnostics.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, removeResults[ii]);
                            diagnosticsExist = true;
                            removeDiagnosticInfos.Add(diagnosticInfo);
                        }

                        continue;
                    }

                    // update diagnostics.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        removeDiagnosticInfos.Add(null);
                    }
                }

                // add new links.
                for (var ii = 0; ii < linksToAdd.Count; ii++)
                {
                    addResults.Add(StatusCodes.Good);

                    LinkedListNode<IUaMonitoredItem> node = null;

                    if (!monitoredItems_.TryGetValue(linksToAdd[ii], out node))
                    {
                        addResults[ii] = StatusCodes.BadMonitoredItemIdInvalid;

                        // update diagnostics.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, addResults[ii]);
                            diagnosticsExist = true;
                            addDiagnosticInfos.Add(diagnosticInfo);
                        }

                        continue;
                    }

                    // check if triggering interface is supported.
                    var triggeredItem = node.Value as IUaTriggeredMonitoredItem;

                    if (triggeredItem == null)
                    {
                        addResults[ii] = StatusCodes.BadNotSupported;

                        // update diagnostics.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, addResults[ii]);
                            diagnosticsExist = true;
                            addDiagnosticInfos.Add(diagnosticInfo);
                        }

                        continue;
                    }

                    // add value if not already in list.
                    var found = false;

                    for (var jj = 0; jj < triggeredItems.Count; jj++)
                    {
                        if (triggeredItems[jj].Id == triggeredItem.Id)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        triggeredItems.Add(triggeredItem);
                    }

                    // update diagnostics.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        addDiagnosticInfos.Add(null);
                    }
                }

                // remove an empty list.
                if (triggeredItems.Count == 0)
                {
                    itemsToTrigger_.Remove(triggeringItemId);
                }

                // clear diagnostics if not required.
                if (!diagnosticsExist)
                {
                    if (addDiagnosticInfos != null) addDiagnosticInfos.Clear();
                    if (removeDiagnosticInfos != null) removeDiagnosticInfos.Clear();
                }
            }
        }

        /// <summary>
        /// Adds monitored items to a subscription.
        /// </summary>
        public void CreateMonitoredItems(
            UaServerOperationContext                        context,
            TimestampsToReturn                      timestampsToReturn,
            MonitoredItemCreateRequestCollection    itemsToCreate,
            out MonitoredItemCreateResultCollection results,
            out DiagnosticInfoCollection            diagnosticInfos)
        {
            if (context == null)       throw new ArgumentNullException(nameof(context));
            if (itemsToCreate == null) throw new ArgumentNullException(nameof(itemsToCreate));

            var count = itemsToCreate.Count;

            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();
            }

            // create the monitored items.
            var monitoredItems = new List<IUaMonitoredItem>(count);
            var errors = new List<ServiceResult>(count);
            var filterResults = new List<MonitoringFilterResult>(count);

            for (var ii = 0; ii < count; ii++)
            {
                monitoredItems.Add(null);
                errors.Add(null);
                filterResults.Add(null);
            }

            server_.NodeManager.CreateMonitoredItems(
                context,
                this.id_,
                publishingInterval_,
                timestampsToReturn,
                itemsToCreate,
                errors,
                filterResults,
                monitoredItems);

            // allocate results.
            var diagnosticsExist = false;
            results = new MonitoredItemCreateResultCollection(count);
            diagnosticInfos = null;

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos = new DiagnosticInfoCollection(count);
            }

            lock (lock_)
            {
                // check session again after CreateMonitoredItems.
                VerifySession(context);

                for (var ii = 0; ii < errors.Count; ii++)
                {
                    // update results.
                    MonitoredItemCreateResult result = null;

                    if (ServiceResult.IsBad(errors[ii]))
                    {
                        result = new MonitoredItemCreateResult();
                        result.StatusCode = errors[ii].Code;

                        if (filterResults[ii] != null)
                        {
                            result.FilterResult = new ExtensionObject(filterResults[ii]);
                        }
                    }
                    else
                    {
                        var monitoredItem = monitoredItems[ii];

                        if (monitoredItem != null)
                        {
                            monitoredItem.SubscriptionCallback = this;

                            var node = itemsToCheck_.AddLast(monitoredItem);
                            monitoredItems_.Add(monitoredItem.Id, node);

                            errors[ii] = monitoredItem.GetCreateResult(out result);

                            // update sampling interval diagnostics.
                            AddItemToSamplingInterval(result.RevisedSamplingInterval, itemsToCreate[ii].MonitoringMode);
                        }
                    }

                    results.Add(result);

                    // update diagnostics.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        DiagnosticInfo diagnosticInfo = null;

                        if (errors[ii] != null && errors[ii].Code != StatusCodes.Good)
                        {
                            diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                        }

                        diagnosticInfos.Add(diagnosticInfo);
                    }
                }

                // clear diagnostics if not required.
                if (!diagnosticsExist && diagnosticInfos != null)
                {
                    diagnosticInfos.Clear();
                }

                TraceState(LogLevel.Information, TraceStateId.Items, "ITEMS CREATED");
            }
        }

        /// <summary>
        /// Adds an item to the sampling interval.
        /// </summary>
        private void AddItemToSamplingInterval(
            double samplingInterval,
            MonitoringMode monitoringMode)
        {
            // update diagnostics
            lock (DiagnosticsWriteLock)
            {
                if (monitoringMode == MonitoringMode.Disabled)
                {
                    diagnostics_.DisabledMonitoredItemCount++;
                }
                diagnostics_.MonitoredItemCount++;
            }
        }

        /// <summary>
        /// Adds an item to the sampling interval.
        /// </summary>
        private void ModifyItemSamplingInterval(
            double oldInterval,
            double newInterval,
            MonitoringMode monitoringMode)
        {
            // TBD
        }

        /// <summary>
        /// Removes an item from the sampling interval.
        /// </summary>
        private void RemoveItemToSamplingInterval(
            double samplingInterval,
            MonitoringMode monitoringMode)
        {
            // update diagnostics
            lock (DiagnosticsWriteLock)
            {
                if (monitoringMode == MonitoringMode.Disabled)
                {
                    diagnostics_.DisabledMonitoredItemCount--;
                }
                diagnostics_.MonitoredItemCount--;
            }
        }

        /// <summary>
        /// Changes the monitoring mode for an item.
        /// </summary>
        private void ModifyItemMonitoringMode(
            double samplingInterval,
            MonitoringMode oldMode,
            MonitoringMode newMode)
        {
            if (newMode != oldMode)
            {
                // update diagnostics
                lock (DiagnosticsWriteLock)
                {
                    if (newMode == MonitoringMode.Disabled)
                    {
                        diagnostics_.DisabledMonitoredItemCount++;
                    }
                    else
                    {
                        diagnostics_.DisabledMonitoredItemCount--;
                    }
                }
            }
        }

        /// <summary>
        /// Modifies monitored items in a subscription.
        /// </summary>
        public void ModifyMonitoredItems(
            UaServerOperationContext                        context,
            TimestampsToReturn                      timestampsToReturn,
            MonitoredItemModifyRequestCollection    itemsToModify,
            out MonitoredItemModifyResultCollection results,
            out DiagnosticInfoCollection            diagnosticInfos)
        {
            if (context == null)       throw new ArgumentNullException(nameof(context));
            if (itemsToModify == null) throw new ArgumentNullException(nameof(itemsToModify));

            var count = itemsToModify.Count;

            // allocate results.
            var diagnosticsExist = false;
            results = new MonitoredItemModifyResultCollection(count);
            diagnosticInfos = null;

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos = new DiagnosticInfoCollection(count);
            }

            // build list of items to modify.
            var monitoredItems = new List<IUaMonitoredItem>(count);
            var errors = new List<ServiceResult>(count);
            var filterResults = new List<MonitoringFilterResult>(count);
            var originalSamplingIntervals = new double[count];

            var validItems = false;

            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                for (var ii = 0; ii < count; ii++)
                {
                    filterResults.Add(null);

                    LinkedListNode<IUaMonitoredItem> node = null;

                    if (!monitoredItems_.TryGetValue(itemsToModify[ii].MonitoredItemId, out node))
                    {
                        monitoredItems.Add(null);
                        errors.Add(StatusCodes.BadMonitoredItemIdInvalid);

                        // update diagnostics.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                            diagnosticInfos.Add(diagnosticInfo);
                        }

                        continue;
                    }

                    var monitoredItem = node.Value;
                    monitoredItems.Add(monitoredItem);
                    originalSamplingIntervals[ii] = monitoredItem.SamplingInterval;

                    errors.Add(null);
                    validItems = true;

                    // update diagnostics.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }
                }
            }

             // update items.
            if (validItems)
            {
                server_.NodeManager.ModifyMonitoredItems(
                    context,
                    timestampsToReturn,
                    monitoredItems,
                    itemsToModify,
                    errors,
                    filterResults);
            }

            lock (lock_)
            {
                // create results.
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    var error = errors[ii];

                    MonitoredItemModifyResult result = null;

                    if (ServiceResult.IsGood(error))
                    {
                        error = monitoredItems[ii].GetModifyResult(out result);
                    }

                    if (result == null)
                    {
                        result = new MonitoredItemModifyResult();
                    }

                    if (error == null)
                    {
                        result.StatusCode = StatusCodes.Good;
                    }
                    else
                    {
                        result.StatusCode = error.StatusCode;
                    }

                    // update diagnostics.
                    if (ServiceResult.IsGood(error))
                    {
                        ModifyItemSamplingInterval(originalSamplingIntervals[ii], result.RevisedSamplingInterval, monitoredItems[ii].MonitoringMode);
                    }

                    if (filterResults[ii] != null)
                    {
                        result.FilterResult = new ExtensionObject(filterResults[ii]);
                    }

                    results.Add(result);

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        if (error != null && error.Code != StatusCodes.Good)
                        {
                            diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                            diagnosticsExist = true;
                        }
                    }
                }

                // clear diagnostics if not required.
                if (!diagnosticsExist && diagnosticInfos != null)
                {
                    diagnosticInfos.Clear();
                }

                TraceState(LogLevel.Information, TraceStateId.Items, "ITEMS MODIFIED");
            }
        }

        /// <summary>
        /// Deletes the monitored items in a subscription.
        /// </summary>
        public void DeleteMonitoredItems(
            UaServerOperationContext context,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            DeleteMonitoredItems(context, monitoredItemIds, false, out results, out diagnosticInfos);
        }

        /// <summary>
        /// Deletes the monitored items in a subscription.
        /// </summary>
        private void DeleteMonitoredItems(
            UaServerOperationContext             context,
            UInt32Collection             monitoredItemIds,
            bool                         doNotCheckSession,
            out StatusCodeCollection     results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (context == null)          throw new ArgumentNullException(nameof(context));
            if (monitoredItemIds == null) throw new ArgumentNullException(nameof(monitoredItemIds));

            var count = monitoredItemIds.Count;

            var diagnosticsExist = false;
            results = new StatusCodeCollection(count);
            diagnosticInfos = null;

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos = new DiagnosticInfoCollection(count);
            }

            // build list of items to modify.
            var monitoredItems = new List<IUaMonitoredItem>(count);
            var errors = new List<ServiceResult>(count);
            var originalSamplingIntervals = new double[count];
            var originalMonitoringModes = new MonitoringMode[count];

            var validItems = false;

            lock (lock_)
            {
                // check session.
                if (!doNotCheckSession)
                {
                    VerifySession(context);
                }

                // clear lifetime counter.
                ResetLifetimeCount();

                for (var ii = 0; ii < count; ii++)
                {
                    LinkedListNode<IUaMonitoredItem> node = null;

                    if (!monitoredItems_.TryGetValue(monitoredItemIds[ii], out node))
                    {
                        monitoredItems.Add(null);
                        errors.Add(StatusCodes.BadMonitoredItemIdInvalid);

                        // update diagnostics.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                            diagnosticInfos.Add(diagnosticInfo);
                        }

                        continue;
                    }

                    var monitoredItem = node.Value;
                    monitoredItems.Add(monitoredItem);

                    // remove the item from the internal lists.
                    monitoredItems_.Remove(monitoredItemIds[ii]);
                    itemsToTrigger_.Remove(monitoredItemIds[ii]);

                    //remove the links towards the deleted monitored item
                    List<IUaTriggeredMonitoredItem> triggeredItems = null;
                    foreach (var item in itemsToTrigger_)
                    {
                        triggeredItems = item.Value;
                        for (var jj = 0; jj < triggeredItems.Count; jj++)
                        {
                            if (triggeredItems[jj].Id == monitoredItemIds[ii])
                            {
                                triggeredItems.RemoveAt(jj);
                                break;
                            }
                        }
                    }

                    if (node.List != null)
                    {
                        node.List.Remove(node);
                    }

                    originalSamplingIntervals[ii] = monitoredItem.SamplingInterval;
                    originalMonitoringModes[ii] = monitoredItem.MonitoringMode;

                    errors.Add(null);
                    validItems = true;

                    // update diagnostics.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }
                }
            }

            // update items.
            if (validItems)
            {
                server_.NodeManager.DeleteMonitoredItems(
                    context,
                    id_,
                    monitoredItems,
                    errors);
            }

            lock (lock_)
            {
                // update diagnostics.
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    var error = errors[ii];

                    if (error == null)
                    {
                        results.Add(StatusCodes.Good);
                    }
                    else
                    {
                        results.Add(error.StatusCode);
                    }

                    // update diagnostics.
                    if (ServiceResult.IsGood(error))
                    {
                        RemoveItemToSamplingInterval(originalSamplingIntervals[ii], originalMonitoringModes[ii]);
                    }

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        if (error != null && error.Code != StatusCodes.Good)
                        {
                            diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                            diagnosticsExist = true;
                        }
                    }
                }

                // clear diagnostics if not required.
                if (!diagnosticsExist && diagnosticInfos != null)
                {
                    diagnosticInfos.Clear();
                }

                TraceState(LogLevel.Information, TraceStateId.Items, "ITEMS DELETED");
            }
        }

        /// <summary>
        /// Changes the monitoring mode for a set of items.
        /// </summary>
        public void SetMonitoringMode(
            UaServerOperationContext             context,
            MonitoringMode               monitoringMode,
            UInt32Collection             monitoredItemIds,
            out StatusCodeCollection     results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            if (context == null)          throw new ArgumentNullException(nameof(context));
            if (monitoredItemIds == null) throw new ArgumentNullException(nameof(monitoredItemIds));

            var count = monitoredItemIds.Count;

            var diagnosticsExist = false;
            results = new StatusCodeCollection(count);
            diagnosticInfos = null;

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos = new DiagnosticInfoCollection(count);
            }

            // build list of items to modify.
            var monitoredItems = new List<IUaMonitoredItem>(count);
            var errors = new List<ServiceResult>(count);
            var originalMonitoringModes = new MonitoringMode[count];

            var validItems = false;

            lock (lock_)
            {
                // check session.
                VerifySession(context);

                // clear lifetime counter.
                ResetLifetimeCount();

                for (var ii = 0; ii < count; ii++)
                {
                    LinkedListNode<IUaMonitoredItem> node = null;

                    if (!monitoredItems_.TryGetValue(monitoredItemIds[ii], out node))
                    {
                        monitoredItems.Add(null);
                        errors.Add(StatusCodes.BadMonitoredItemIdInvalid);

                        // update diagnostics.
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, errors[ii]);
                            diagnosticsExist = true;
                            diagnosticInfos.Add(diagnosticInfo);
                        }

                        continue;
                    }

                    var monitoredItem = node.Value;
                    monitoredItems.Add(monitoredItem);
                    originalMonitoringModes[ii] = monitoredItem.MonitoringMode;

                    errors.Add(null);
                    validItems = true;

                    // update diagnostics.
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }
                }
            }

            // update items.
            if (validItems)
            {
                server_.NodeManager.SetMonitoringMode(
                    context,
                    monitoringMode,
                    monitoredItems,
                    errors);
            }

            lock (lock_)
            {
                // update diagnostics.
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    var error = errors[ii];

                    if (error == null)
                    {
                        results.Add(StatusCodes.Good);
                    }
                    else
                    {
                        results.Add(error.StatusCode);
                    }

                    // update diagnostics.
                    if (ServiceResult.IsGood(error))
                    {
                        ModifyItemMonitoringMode(monitoredItems[ii].SamplingInterval, originalMonitoringModes[ii], monitoringMode);
                    }

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        if (error != null && error.Code != StatusCodes.Good)
                        {
                            diagnosticInfos[ii] = UaServerUtils.CreateDiagnosticInfo(server_, context, error);
                            diagnosticsExist = true;
                        }
                    }
                }

                // clear diagnostics if not required.
                if (!diagnosticsExist && diagnosticInfos != null)
                {
                    diagnosticInfos.Clear();
                }

                if (monitoringMode == MonitoringMode.Disabled)
                {
                    TraceState(LogLevel.Information, TraceStateId.Monitor, "MONITORING DISABLED");
                }
                else if (monitoringMode == MonitoringMode.Reporting)
                {
                    TraceState(LogLevel.Information, TraceStateId.Monitor, "REPORTING");
                }
                else
                {
                    TraceState(LogLevel.Information, TraceStateId.Monitor, "SAMPLING");
                }
            }
        }

        /// <summary>
        /// Verifies that a condition refresh operation is permitted.
        /// </summary>
        public void ValidateConditionRefresh(UaServerOperationContext context)
        {
            lock (lock_)
            {
                VerifySession(context);

                if (refreshInProgress_)
                {
                    throw new ServiceResultException(StatusCodes.BadRefreshInProgress);
                }
            }
        }

        /// <summary>
        /// Verifies that a condition refresh operation is permitted.
        /// </summary>
        public void ValidateConditionRefresh2(UaServerOperationContext context, uint monitoredItemId)
        {
            ValidateConditionRefresh(context);

            lock (lock_)
            {
                if (!monitoredItems_.ContainsKey(monitoredItemId))
                {
                    throw new ServiceResultException(StatusCodes.BadMonitoredItemIdInvalid,
                        "Cannot refresh conditions for a monitored item that does not exist.");
                }
            }
        }

        /// <summary>
        /// Refreshes the conditions.
        /// </summary>
        public void ConditionRefresh()
        {
            List<IUaEventMonitoredItem> monitoredItems = new List<IUaEventMonitoredItem>();

            lock (lock_)
            {
                // build list of items to refresh.
                foreach (LinkedListNode<IUaMonitoredItem> monitoredItem in monitoredItems_.Values)
                {
                    UaMonitoredItem eventMonitoredItem = monitoredItem.Value as UaMonitoredItem;

                    if (eventMonitoredItem != null && eventMonitoredItem.EventFilter != null)
                    {
                        // add to list that gets reported to the NodeManagers.
                        monitoredItems.Add(eventMonitoredItem);
                    }
                }

                // nothing to do if no event subscriptions.
                if (monitoredItems.Count == 0)
                {
                    return;
                }
            }

            ConditionRefresh(monitoredItems, 0);
        }

        /// <summary>
        /// Refreshes the conditions.
        /// </summary>
        public void ConditionRefresh2(uint monitoredItemId)
        {
            List<IUaEventMonitoredItem> monitoredItems = new List<IUaEventMonitoredItem>();

            lock (lock_)
            {
                // build list of items to refresh.
                if (monitoredItems_.ContainsKey(monitoredItemId))
                {
                    LinkedListNode<IUaMonitoredItem> monitoredItem = monitoredItems_[monitoredItemId];

                    UaMonitoredItem eventMonitoredItem = monitoredItem.Value as UaMonitoredItem;

                    if (eventMonitoredItem != null && eventMonitoredItem.EventFilter != null)
                    {
                        // add to list that gets reported to the NodeManagers.
                        monitoredItems.Add(eventMonitoredItem);
                    }
                }
                else
                {
                    throw new ServiceResultException(StatusCodes.BadMonitoredItemIdInvalid,
                        "Cannot refresh conditions for a monitored item that does not exist.");
                }

                // nothing to do if no event subscriptions.
                if (monitoredItems.Count == 0)
                {
                    return;
                }
            }

            ConditionRefresh(monitoredItems, monitoredItemId);
        }

        /// <summary>
        /// Refreshes the conditions.  Works for both ConditionRefresh and ConditionRefresh2
        /// </summary>
        private void ConditionRefresh(List<IUaEventMonitoredItem> monitoredItems, uint monitoredItemId)
        {
            UaServerContext systemContext = server_.DefaultSystemContext.Copy(session_);

            string messageTemplate = Utils.Format("Condition refresh {{0}} for subscription {0}.", Id);
            if (monitoredItemId > 0)
            {
                messageTemplate = Utils.Format("Condition refresh {{0}} for subscription {0}, monitored item {1}.", Id, monitoredItemId);
            }

            lock (lock_)
            {
                // generate start event.
                RefreshStartEventState e = new RefreshStartEventState(null);

                TranslationInfo message = null;

                message = new TranslationInfo(
                    "RefreshStartEvent",
                    "en-US",
                    Utils.Format(messageTemplate, "started"));

                e.Initialize(
                    systemContext,
                    null,
                    EventSeverity.Low,
                    new LocalizedText(message));

                e.SetChildValue(systemContext, BrowseNames.SourceNode, diagnosticsId_, false);
                e.SetChildValue(systemContext, BrowseNames.SourceName, Utils.Format("Subscription/{0}", Id), false);
                e.SetChildValue(systemContext, BrowseNames.ReceiveTime, DateTime.UtcNow, false);

                // build list of items to refresh.
                foreach (IUaEventMonitoredItem monitoredItem in monitoredItems)
                {
                    UaMonitoredItem eventMonitoredItem = monitoredItem as UaMonitoredItem;

                    if (eventMonitoredItem != null && eventMonitoredItem.EventFilter != null)
                    {
                        // queue start refresh event.
                        eventMonitoredItem.QueueEvent(e, true);
                    }
                }

                // nothing to do if no event subscriptions.
                if (monitoredItems.Count == 0)
                {
                    return;
                }
            }

            // tell the NodeManagers to report the current state of the conditions.
            try
            {
                refreshInProgress_ = true;

                UaServerOperationContext operationContext = new UaServerOperationContext(session_, DiagnosticsMasks.None);
                server_.NodeManager.ConditionRefresh(operationContext, monitoredItems);
            }
            finally
            {
                refreshInProgress_ = false;
            }

            lock (lock_)
            {
                // generate start event.
                RefreshEndEventState e = new RefreshEndEventState(null);

                TranslationInfo message = null;

                message = new TranslationInfo(
                    "RefreshEndEvent",
                    "en-US",
                    Utils.Format(messageTemplate, "completed"));

                e.Initialize(
                    systemContext,
                    null,
                    EventSeverity.Low,
                    new LocalizedText(message));

                e.SetChildValue(systemContext, BrowseNames.SourceNode, diagnosticsId_, false);
                e.SetChildValue(systemContext, BrowseNames.SourceName, Utils.Format("Subscription/{0}", Id), false);
                e.SetChildValue(systemContext, BrowseNames.ReceiveTime, DateTime.UtcNow, false);

                // send refresh end event.
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    UaMonitoredItem monitoredItem = monitoredItems[ii] as UaMonitoredItem;

                    if (monitoredItem.EventFilter != null)
                    {
                        monitoredItem.QueueEvent(e, true);
                    }
                }

                // TraceState("CONDITION REFRESH");
            }
        }

        /// <summary>
        /// Sets the subscription to durable mode.
        /// </summary>
        public ServiceResult SetSubscriptionDurable(uint lifeTimeInHours, out uint revisedLifeTimeInHours)
        {
            lock (lock_)
            {
                // set default
                revisedLifeTimeInHours = 0;

                if (monitoredItems_.Count > 0)
                {
                    return StatusCodes.BadInvalidState;
                }

                // TODO: enable the durable subscription support here

                return StatusCodes.Good;
            }
        }

        /// <summary>
        /// Gets the monitored items for the subscription.
        /// </summary>
        public void GetMonitoredItems(out uint[] serverHandles, out uint[] clientHandles)
        {
            lock (lock_)
            {
                serverHandles = new uint[monitoredItems_.Count];
                clientHandles = new uint[monitoredItems_.Count];

                var ii = 0;

                foreach (var entry in monitoredItems_)
                {
                    serverHandles[ii] = entry.Key;
                    clientHandles[ii] = entry.Value.Value.ClientHandle;
                    ii++;
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns a copy of the current diagnostics.
        /// </summary>
        private ServiceResult OnUpdateDiagnostics(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            lock (DiagnosticsLock)
            {
                value = Utils.Clone(diagnostics_);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Throws an exception if the session is not the owner.
        /// </summary>
        private void VerifySession(UaServerOperationContext context)
        {
            if (expired_)
            {
                throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
            }

            if (!Object.ReferenceEquals(context.Session, session_))
            {
                throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid, "Subscription belongs to a different session.");
            }
        }

        /// <summary>
        /// The states to log.
        /// </summary>
        private enum TraceStateId
        {
            Config,
            Items,
            Monitor,
            Publish,
            Deleted
        };

        /// <summary>
        /// Dumps the current state of the session queue.
        /// </summary>
        private void TraceState(LogLevel logLevel, TraceStateId id, string context)
        {
            const string DeletedMessage = "Subscription {0}, SessionId={1}, Id={2}, SeqNo={3}, MessageCount={4}";
            const string ConfigMessage = "Subscription {0}, SessionId={1}, Id={2}, Priority={3}, Publishing={4}, KeepAlive={5}, LifeTime={6}, MaxNotifications={7}, Enabled={8}";
            const string MonitorMessage = "Subscription {0}, Id={1}, KeepAliveCount={2}, LifeTimeCount={3}, WaitingForPublish={4}, SeqNo={5}, ItemCount={6}, ItemsToCheck={7}, ItemsToPublish={8}, MessageCount={9}";
            const string ItemsMessage = "Subscription {0}, Id={1}, ItemCount={2}, ItemsToCheck={3}, ItemsToPublish={4}";

            if (!Utils.Logger.IsEnabled(logLevel))
            {
                return;
            }

            // save counters
            Monitor.Enter(lock_);

            long sequenceNumber = sequenceNumber_;
            int itemsToCheck = itemsToCheck_.Count;
            int monitoredItems = monitoredItems_.Count;
            int itemsToPublish = itemsToPublish_.Count;
            int sentMessages = sentMessages_.Count;
            bool publishingEnabled = publishingEnabled_;
            bool waitingForPublish = waitingForPublish_;

            Monitor.Exit(lock_);

            switch (id)
            {
                case TraceStateId.Deleted:
                    Utils.Log(logLevel, DeletedMessage, context, session_?.Id, id_,
                        sequenceNumber, sentMessages);
                    break;

                case TraceStateId.Config:
                    Utils.Log(logLevel, ConfigMessage, context, session_?.Id, id_,
                        priority_, publishingInterval_, maxKeepAliveCount_,
                        maxLifetimeCount_, maxNotificationsPerPublish_, publishingEnabled);
                    break;

                case TraceStateId.Items:
                    Utils.Log(logLevel, ItemsMessage, context, id_,
                        monitoredItems, itemsToCheck, itemsToPublish);
                    break;

                case TraceStateId.Publish:
                case TraceStateId.Monitor:
                    Utils.Log(logLevel, MonitorMessage, context, id_, keepAliveCounter_, lifetimeCounter_,
                        waitingForPublish, sequenceNumber, monitoredItems, itemsToCheck,
                        itemsToPublish, sentMessages);
                    break;
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private Sessions.Session session_;
        private uint id_;
        private UserIdentityToken savedOwnerIdentity_;
        private double publishingInterval_;
        private uint maxLifetimeCount_;
        private uint maxKeepAliveCount_;
        private uint maxNotificationsPerPublish_;
        private bool publishingEnabled_;
        private byte priority_;
        private long publishTimerExpiry_;
        private uint keepAliveCounter_;
        private uint lifetimeCounter_;
        private bool waitingForPublish_;
        private List<NotificationMessage> sentMessages_;
        private int lastSentMessage_;
        private long sequenceNumber_;
        private uint maxMessageCount_;
        private Dictionary<uint, LinkedListNode<IUaMonitoredItem>> monitoredItems_;
        private LinkedList<IUaMonitoredItem> itemsToCheck_;
        private LinkedList<IUaMonitoredItem> itemsToPublish_;
        private NodeId diagnosticsId_;
        private SubscriptionDiagnosticsDataType diagnostics_;
        private bool refreshInProgress_;
        private bool expired_;
        private Dictionary<uint, List<IUaTriggeredMonitoredItem>> itemsToTrigger_;
        #endregion
    }
}
