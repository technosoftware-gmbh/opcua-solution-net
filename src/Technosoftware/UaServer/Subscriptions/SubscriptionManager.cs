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
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;

using Opc.Ua;

using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer.Subscriptions
{
    /// <summary>
    /// A generic session manager object for a server.
    /// </summary>
    public class SubscriptionManager : IDisposable, IUaSubscriptionManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the manager with its configuration.
        /// </summary>
        public SubscriptionManager(
            IUaServerData server,
            ApplicationConfiguration configuration)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            server_ = server;

            minPublishingInterval_ = configuration.ServerConfiguration.MinPublishingInterval;
            maxPublishingInterval_ = configuration.ServerConfiguration.MaxPublishingInterval;
            publishingResolution_ = configuration.ServerConfiguration.PublishingResolution;
            maxSubscriptionLifetime_ = (uint)configuration.ServerConfiguration.MaxSubscriptionLifetime;
            minSubscriptionLifetime_ = (uint)configuration.ServerConfiguration.MinSubscriptionLifetime;
            maxMessageCount_ = (uint)configuration.ServerConfiguration.MaxMessageQueueSize;
            maxNotificationsPerPublish_ = (uint)configuration.ServerConfiguration.MaxNotificationsPerPublish;
            maxPublishRequestCount_ = configuration.ServerConfiguration.MaxPublishRequestCount;
            maxSubscriptionCount_ = configuration.ServerConfiguration.MaxSubscriptionCount;

            subscriptions_ = new Dictionary<uint, Subscription>();
            publishQueues_ = new Dictionary<NodeId, SessionPublishQueue>();
            statusMessages_ = new Dictionary<NodeId, Queue<StatusMessage>>();
            lastSubscriptionId_ = BitConverter.ToInt64(Utils.Nonce.CreateNonce(sizeof(long)), 0);

            // create a event to signal shutdown.
            shutdownEvent_ = new ManualResetEvent(true);

            // create queue and event for condition refresh worker
            conditionRefreshEvent_ = new ManualResetEvent(false);
            conditionRefreshQueue_ = new Queue<ConditionRefreshTask>();
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
                List<Subscription> subscriptions = null;
                List<SessionPublishQueue> publishQueues = null;

                lock (lock_)
                {
                    publishQueues = new List<SessionPublishQueue>(publishQueues_.Values);
                    publishQueues_.Clear();

                    subscriptions = new List<Subscription>(subscriptions_.Values);
                    subscriptions_.Clear();
                }

                foreach (var publishQueue in publishQueues)
                {
                    Utils.SilentDispose(publishQueue);
                }

                foreach (var subscription in subscriptions)
                {
                    Utils.SilentDispose(subscription);
                }

            }
        }
        #endregion

        #region IUaSubscriptionManager Members
        /// <summary>
        /// Raised after a new subscription is created.
        /// </summary>
        public event EventHandler<SubscriptionEventArgs> SubscriptionCreated
        {
            add
            {
                lock (eventLock_)
                {
                    SubscriptionCreatedEventHandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    SubscriptionCreatedEventHandler -= value;
                }
            }
        }

        /// <summary>
        /// Raised before a subscription is deleted.
        /// </summary>
        public event EventHandler<SubscriptionEventArgs> SubscriptionDeleted
        {
            add
            {
                lock (eventLock_)
                {
                    SubscriptionDeletedEventHandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    SubscriptionDeletedEventHandler -= value;
                }
            }
        }

        /// <summary>
        /// Returns all of the subscriptions known to the subscription manager.
        /// </summary>
        /// <returns>A list of the subscriptions.</returns>
        public IList<Subscription> GetSubscriptions()
        {
            var subscriptions = new List<Subscription>();

            lock (lock_)
            {
                subscriptions.AddRange(subscriptions_.Values);
            }

            return subscriptions;
        }

        /// <summary>
        /// Raises an event related to a subscription.
        /// </summary>
        protected virtual void RaiseSubscriptionEvent(Subscription subscription, bool deleted)
        {
            EventHandler<SubscriptionEventArgs> handler = null;

            lock (eventLock_)
            {
                handler = SubscriptionCreatedEventHandler;

                if (deleted)
                {
                    handler = SubscriptionDeletedEventHandler;
                }
            }

            if (handler != null)
            {
                try
                {
                    handler(subscription, new SubscriptionEventArgs(deleted));
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Subscription event handler raised an exception.");
                }
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Starts up the manager makes it ready to create subscriptions.
        /// </summary>
        public virtual void Startup()
        {
            lock (lock_)
            {
                shutdownEvent_.Reset();

                Task.Factory.StartNew(() => {
                    PublishSubscriptions(publishingResolution_);
                }, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);

                conditionRefreshEvent_.Reset();

                Task.Factory.StartNew(() => {
                    ConditionRefreshWorker();
                }, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
            }
        }

        /// <summary>
        /// Closes all subscriptions and rejects any new requests.
        /// </summary>
        public virtual void Shutdown()
        {
            lock (lock_)
            {
                // stop the publishing thread.
                shutdownEvent_.Set();

                // trigger the condition refresh thread.
                conditionRefreshEvent_.Set();

                // dispose of publish queues.
                foreach (var queue in publishQueues_.Values)
                {
                    queue.Dispose();
                }

                publishQueues_.Clear();

                // dispose of subscriptions objects.
                foreach (var subscription in subscriptions_.Values)
                {
                    subscription.Dispose();
                }

                subscriptions_.Clear();
            }
        }

        /// <summary>
        /// Signals that a session is closing.
        /// </summary>
        public virtual void SessionClosing(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions)
        {
            // close the publish queue for the session.
            SessionPublishQueue queue = null;
            IList<Subscription> subscriptionsToDelete = null;
            uint publishingIntervalCount = 0;

            lock (lock_)
            {
                if (publishQueues_.TryGetValue(sessionId, out queue))
                {
                    publishQueues_.Remove(sessionId);
                    subscriptionsToDelete = queue.Close();

                    // remove the subscriptions.
                    if (deleteSubscriptions && subscriptionsToDelete != null)
                    {
                        for (var ii = 0; ii < subscriptionsToDelete.Count; ii++)
                        {
                            subscriptions_.Remove(subscriptionsToDelete[ii].Id);
                        }
                    }
                }
            }

            //remove the expired subscription status change notifications for this session
            lock (statusMessages_)
            {
                Queue<StatusMessage> statusQueue = null;
                if (statusMessages_.TryGetValue(sessionId, out statusQueue))
                {
                    statusMessages_.Remove(sessionId);
                }
            }

            // process all subscriptions in the queue.
            if (subscriptionsToDelete != null)
            {
                for (var ii = 0; ii < subscriptionsToDelete.Count; ii++)
                {
                    var subscription = subscriptionsToDelete[ii];

                    // delete the subscription.
                    if (deleteSubscriptions)
                    {
                        // raise subscription event.
                        RaiseSubscriptionEvent(subscription, true);

                        // delete subscription.
                        subscription.Delete(context);

                        // get the count for the diagnostics.
                        publishingIntervalCount = GetPublishingIntervalCount();

                        lock (server_.DiagnosticsWriteLock)
                        {
                            ServerDiagnosticsSummaryDataType diagnostics = server_.ServerDiagnostics;
                            diagnostics.CurrentSubscriptionCount--;
                            diagnostics.PublishingIntervalCount = publishingIntervalCount;
                        }
                    }

                    // mark the subscriptions as abandoned.
                    else
                    {
                        lock (lock_)
                        {
                            if (abandonedSubscriptions_ == null)
                            {
                                abandonedSubscriptions_ = new List<Subscription>();
                            }

                            abandonedSubscriptions_.Add(subscription);
                            Utils.LogWarning("Subscription {0}, Id={1}.", "ABANDONED", subscription.Id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the conditions for the specified subscription.
        /// </summary>
        public void ConditionRefresh(UaServerOperationContext context, uint subscriptionId)
        {
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadSubscriptionIdInvalid,
                        "Cannot refresh conditions for a subscription that does not exist.");
                }
            }

            // ensure a condition refresh is allowed.
            subscription.ValidateConditionRefresh(context);

            var conditionRefreshTask = new ConditionRefreshTask(subscription, 0);

            ServiceResultException serviceResultException = null;
            lock (conditionRefreshLock_)
            {
                if (!conditionRefreshQueue_.Contains(conditionRefreshTask))
                {
                    conditionRefreshQueue_.Enqueue(conditionRefreshTask);
                }
                else
                {
                    serviceResultException = new ServiceResultException(StatusCodes.BadRefreshInProgress);
                }

                // trigger the refresh worker.
                conditionRefreshEvent_.Set();
            }

            if (serviceResultException != null)
            {
                throw serviceResultException;
            }
        }

        /// <summary>
        /// Refreshes the conditions for the specified subscription and monitored item.
        /// </summary>
        public void ConditionRefresh2(UaServerOperationContext context, uint subscriptionId, uint monitoredItemId)
        {
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadSubscriptionIdInvalid,
                        "Cannot refresh conditions for a subscription that does not exist.");
                }
            }

            // ensure a condition refresh is allowed.
            subscription.ValidateConditionRefresh2(context, monitoredItemId);

            var conditionRefreshTask = new ConditionRefreshTask(subscription, monitoredItemId);

            lock (conditionRefreshLock_)
            {
                if (!conditionRefreshQueue_.Contains(conditionRefreshTask))
                {
                    conditionRefreshQueue_.Enqueue(conditionRefreshTask);
                }
                else
                {
                    throw new ServiceResultException(StatusCodes.BadRefreshInProgress);
                }

                // trigger the refresh worker.
                conditionRefreshEvent_.Set();
            }
        }

        /// <summary>
        /// Completes a refresh conditions request.
        /// </summary>
        private void DoConditionRefresh(Subscription subscription)
        {
            try
            {
                Utils.LogTrace("Subscription ConditionRefresh started, Id={0}.", subscription.Id);
                subscription.ConditionRefresh();
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Subscription - DoConditionRefresh Exited Unexpectedly");
            }
        }

        /// <summary>
        /// Completes a refresh conditions request.
        /// </summary>
        private void DoConditionRefresh2(Subscription subscription, uint monitoredItemId)
        {
            try
            {
                Utils.LogTrace("Subscription ConditionRefresh2 started, Id={0}, MonitoredItemId={1}.",
                    subscription.Id, monitoredItemId);
                subscription.ConditionRefresh2(monitoredItemId);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Subscription - DoConditionRefresh2 Exited Unexpectedly");
            }
        }

        /// <summary>
        /// Deletes the specified subscription.
        /// </summary>
        public StatusCode DeleteSubscription(UaServerOperationContext context, uint subscriptionId)
        {
            uint publishingIntervalCount = 0;
            var monitoredItemCount = 0;
            Subscription subscription = null;

            lock (lock_)
            {
                // remove from publish queue.
                if (subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    var sessionId = subscription.SessionId;

                    if (!NodeId.IsNull(sessionId))
                    {
                        // check that the subscription is the owner.
                        if (context != null && !Object.ReferenceEquals(context.Session, subscription.Session))
                        {
                            throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                        }

                        SessionPublishQueue queue = null;

                        if (publishQueues_.TryGetValue(sessionId, out queue))
                        {
                            queue.Remove(subscription, true);
                        }
                    }
                }

                // check for abandoned subscription.
                if (abandonedSubscriptions_ != null)
                {
                    for (var ii = 0; ii < abandonedSubscriptions_.Count; ii++)
                    {
                        if (abandonedSubscriptions_[ii].Id == subscriptionId)
                        {
                            abandonedSubscriptions_.RemoveAt(ii);
                            Utils.LogWarning("Subscription {0}, Id={1}.", "DELETED(ABANDONED)", subscriptionId);
                            break;
                        }
                    }
                }

                // remove subscription.
                subscriptions_.Remove(subscriptionId);
            }

            if (subscription != null)
            {
                monitoredItemCount = subscription.MonitoredItemCount;

                // raise subscription event.
                RaiseSubscriptionEvent(subscription, true);

                // delete subscription.
                subscription.Delete(context);

                // get the count for the diagnostics.
                publishingIntervalCount = GetPublishingIntervalCount();

                lock (server_.DiagnosticsWriteLock)
                {
                    var diagnostics = server_.ServerDiagnostics;
                    diagnostics.CurrentSubscriptionCount--;
                    diagnostics.PublishingIntervalCount = publishingIntervalCount;
                }

                if (context != null && context.Session != null)
                {
                    lock (context.Session.DiagnosticsLock)
                    {
                        var diagnostics = context.Session.SessionDiagnostics;
                        diagnostics.CurrentSubscriptionsCount--;
                        UpdateCurrentMonitoredItemsCount(diagnostics, -monitoredItemCount);
                    }
                }

                return StatusCodes.Good;
            }

            return StatusCodes.BadSubscriptionIdInvalid;
        }

        /// <summary>
        /// Updates the current monitored item count for the session.
        /// </summary>
        private void UpdateCurrentMonitoredItemsCount(SessionDiagnosticsDataType diagnostics, int change)
        {
            var monitoredItemsCount = (long)diagnostics.CurrentMonitoredItemsCount;
            monitoredItemsCount += change;

            if (monitoredItemsCount > 0)
            {
                diagnostics.CurrentMonitoredItemsCount = (uint)monitoredItemsCount;
            }
            else
            {
                diagnostics.CurrentMonitoredItemsCount = 0;
            }
        }

        /// <summary>
        /// Gets the total number of publishing intervals in use.
        /// </summary>
        private uint GetPublishingIntervalCount()
        {
            var publishingDiagnostics = new Dictionary<double, uint>();

            lock (lock_)
            {
                foreach (var subscription in subscriptions_.Values)
                {
                    var publishingInterval = subscription.PublishingInterval;

                    uint total = 0;

                    if (!publishingDiagnostics.TryGetValue(publishingInterval, out total))
                    {
                        total = 0;
                    }

                    publishingDiagnostics[publishingInterval] = total + 1;
                }
            }

            return (uint)publishingDiagnostics.Count;
        }

        /// <summary>
        /// Creates a new subscription.
        /// </summary>
        public virtual void CreateSubscription(
            UaServerOperationContext context,
            double requestedPublishingInterval,
            uint requestedLifetimeCount,
            uint requestedMaxKeepAliveCount,
            uint maxNotificationsPerPublish,
            bool publishingEnabled,
            byte priority,
            out uint subscriptionId,
            out double revisedPublishingInterval,
            out uint revisedLifetimeCount,
            out uint revisedMaxKeepAliveCount)
        {
            lock (lock_)
            {
                if (subscriptions_.Count >= maxSubscriptionCount_)
                {
                    throw new ServiceResultException(StatusCodes.BadTooManySubscriptions);
                }
            }

            subscriptionId = 0;
            revisedPublishingInterval = 0;
            revisedLifetimeCount = 0;
            revisedMaxKeepAliveCount = 0;

            uint publishingIntervalCount = 0;
            Subscription subscription = null;

            // get sessin from context.
            var session = context.Session;

            // assign new identifier.
            subscriptionId = Utils.IncrementIdentifier(ref lastSubscriptionId_);

            // calculate publishing interval.
            revisedPublishingInterval = CalculatePublishingInterval(requestedPublishingInterval);

            // calculate the keep alive count.
            revisedMaxKeepAliveCount = CalculateKeepAliveCount(revisedPublishingInterval, requestedMaxKeepAliveCount);

            // calculate the lifetime count.
            revisedLifetimeCount = CalculateLifetimeCount(revisedPublishingInterval, revisedMaxKeepAliveCount, requestedLifetimeCount);

            // calculate the max notification count.
            maxNotificationsPerPublish = CalculateMaxNotificationsPerPublish(maxNotificationsPerPublish);

            // create the subscription.
            subscription = CreateSubscription(
                context,
                subscriptionId,
                revisedPublishingInterval,
                revisedLifetimeCount,
                revisedMaxKeepAliveCount,
                maxNotificationsPerPublish,
                priority,
                publishingEnabled);

            lock (lock_)
            {
                // save subscription.
                subscriptions_.Add(subscriptionId, subscription);

                // create/update publish queue.
                SessionPublishQueue queue = null;

                if (!publishQueues_.TryGetValue(session.Id, out queue))
                {
                    publishQueues_[session.Id] = queue = new SessionPublishQueue(server_, session, maxPublishRequestCount_);
                }

                queue.Add(subscription);

                // get the count for the diagnostics.
                publishingIntervalCount = GetPublishingIntervalCount();
            }

            lock (statusMessages_)
            {
                Queue<StatusMessage> messagesQueue = null;
                if (!statusMessages_.TryGetValue(session.Id, out messagesQueue))
                {
                    statusMessages_[session.Id] = new Queue<StatusMessage>();
                }
            }

            lock (server_.DiagnosticsWriteLock)
            {
                var diagnostics = server_.ServerDiagnostics;
                diagnostics.CurrentSubscriptionCount++;
                diagnostics.CumulatedSubscriptionCount++;
                diagnostics.PublishingIntervalCount = publishingIntervalCount;
            }

            if (context.Session != null)
            {
                lock (context.Session.DiagnosticsLock)
                {
                    var diagnostics = context.Session.SessionDiagnostics;
                    diagnostics.CurrentSubscriptionsCount++;
                }
            }

            // raise subscription event.
            RaiseSubscriptionEvent(subscription, false);
        }

        /// <summary>
        /// Deletes group of subscriptions.
        /// </summary>
        public void DeleteSubscriptions(
            UaServerOperationContext context,
            UInt32Collection subscriptionIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            var diagnosticsExist = false;
            results = new StatusCodeCollection(subscriptionIds.Count);
            diagnosticInfos = new DiagnosticInfoCollection(subscriptionIds.Count);

            foreach (var subscriptionId in subscriptionIds)
            {
                try
                {
                    var result = DeleteSubscription(context, subscriptionId);
                    results.Add(result);

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }
                }
                catch (Exception e)
                {
                    var result = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, String.Empty);
                    results.Add(result.Code);

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, result);
                        diagnosticInfos.Add(diagnosticInfo);
                        diagnosticsExist = true;
                    }
                }
            }

            if (!diagnosticsExist)
            {
                diagnosticInfos.Clear();
            }
        }

        /// <summary>
        /// Publishes a subscription.
        /// </summary>
        public NotificationMessage Publish(
            UaServerOperationContext context,
            SubscriptionAcknowledgementCollection subscriptionAcknowledgements,
            AsyncPublishOperation operation,
            out uint subscriptionId,
            out UInt32Collection availableSequenceNumbers,
            out bool moreNotifications,
            out StatusCodeCollection acknowledgeResults,
            out DiagnosticInfoCollection acknowledgeDiagnosticInfos)
        {
            availableSequenceNumbers = null;
            moreNotifications = false;

            // get publish queue for session.
            SessionPublishQueue queue = null;

            lock (lock_)
            {
                if (!publishQueues_.TryGetValue(context.Session.Id, out queue))
                {
                    if (subscriptions_.Count == 0)
                    {
                        throw new ServiceResultException(StatusCodes.BadNoSubscription);
                    }

                    throw new ServiceResultException(StatusCodes.BadSessionClosed);
                }
            }

            // acknowledge previous messages.
            queue.Acknowledge(
                context,
                subscriptionAcknowledgements,
                out acknowledgeResults,
                out acknowledgeDiagnosticInfos);

            // update diagnostics.
            if (context.Session != null)
            {
                lock (context.Session.DiagnosticsLock)
                {
                    var diagnostics = context.Session.SessionDiagnostics;
                    diagnostics.CurrentPublishRequestsInQueue++;
                }
            }

            // save results for asynchrounous operation.
            if (operation != null)
            {
                operation.Response.Results = acknowledgeResults;
                operation.Response.DiagnosticInfos = acknowledgeDiagnosticInfos;
            }

            // gets the next message that is ready to publish.
            var message = GetNextMessage(
                context,
                queue,
                operation,
                out subscriptionId,
                out availableSequenceNumbers,
                out moreNotifications);

            // if no message and no async operation then a timeout occurred.
            if (message == null && operation == null)
            {
                throw new ServiceResultException(StatusCodes.BadTimeout);
            }

            // return message.
            return message;
        }

        /// <summary>
        /// Called when a subscription expires.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        internal void SubscriptionExpired(Subscription subscription)
        {
            lock (statusMessages_)
            {
                var message = new StatusMessage();
                message.SubscriptionId = subscription.Id;
                message.Message = subscription.PublishTimeout();

                Queue<StatusMessage> queue = null;

                if (subscription.SessionId != null && statusMessages_.TryGetValue(subscription.SessionId, out queue))
                {
                    queue.Enqueue(message);
                }
            }
        }

        /// <summary>
        /// Completes the publish.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="operation">The asynchronous operation.</param>
        /// <returns>
        /// True if successful. False if the request has been requeued.
        /// </returns>
        public bool CompletePublish(
            UaServerOperationContext context,
            AsyncPublishOperation operation)
        {
            // get publish queue for session.
            SessionPublishQueue queue = null;

            lock (lock_)
            {
                if (!publishQueues_.TryGetValue(context.Session.Id, out queue))
                {
                    throw new ServiceResultException(StatusCodes.BadSessionClosed);
                }
            }

            uint subscriptionId = 0;
            UInt32Collection availableSequenceNumbers = null;
            var moreNotifications = false;

            NotificationMessage message = null;

            Utils.LogTrace("Publish #{0} ReceivedFromClient", context.ClientHandle);
            var requeue = false;

            do
            {
                // wait for a subscription to publish.
                var subscription = queue.CompletePublish(requeue, operation, operation.Calldata);

                if (subscription == null)
                {
                    return false;
                }

                subscriptionId = subscription.Id;
                moreNotifications = false;

                // publish notifications.
                try
                {
                    requeue = false;

                    message = subscription.Publish(
                        context,
                        out availableSequenceNumbers,
                        out moreNotifications);

                    // a null message indicates a false alarm and that there were no notifications
                    // to publish and that the request needs to be requeued.
                    if (message != null)
                    {
                        break;
                    }

                    Utils.LogTrace("Publish False Alarm - Request #{0} Requeued.", context.ClientHandle);
                    requeue = true;
                }
                finally
                {
                    queue.PublishCompleted(subscription, moreNotifications);
                }
            }
            while (requeue);

            // fill in response if operation completed.
            if (message != null)
            {
                operation.Response.SubscriptionId = subscriptionId;
                operation.Response.AvailableSequenceNumbers = availableSequenceNumbers;
                operation.Response.MoreNotifications = moreNotifications;
                operation.Response.NotificationMessage = message;

                // update diagnostics.
                if (context.Session != null)
                {
                    lock (context.Session.DiagnosticsLock)
                    {
                        var diagnostics = context.Session.SessionDiagnostics;
                        diagnostics.CurrentPublishRequestsInQueue--;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Publishes a subscription.
        /// </summary>
        public NotificationMessage GetNextMessage(
            UaServerOperationContext context,
            SessionPublishQueue queue,
            AsyncPublishOperation operation,
            out uint subscriptionId,
            out UInt32Collection availableSequenceNumbers,
            out bool moreNotifications)
        {
            subscriptionId = 0;
            availableSequenceNumbers = null;
            moreNotifications = false;

            NotificationMessage message = null;

            try
            {
                Utils.LogTrace("Publish #{0} ReceivedFromClient", context.ClientHandle);

                if (ReturnPendingStatusMessage(context, out message, out subscriptionId))
                {
                    return message;
                }

                var requeue = false;

                do
                {
                    // wait for a subscription to publish.
                    var subscription = queue.Publish(
                        context.ClientHandle,
                        context.OperationDeadline,
                        requeue,
                        operation);

                    if (subscription == null)
                    {
                        // check for pending status message.
                        if (ReturnPendingStatusMessage(context, out message, out subscriptionId))
                        {
                            return message;
                        }

                        Utils.LogTrace("Publish #{0} Timeout", context.ClientHandle);
                        return null;
                    }

                    subscriptionId = subscription.Id;
                    moreNotifications = false;

                    // publish notifications.
                    try
                    {
                        requeue = false;

                        message = subscription.Publish(
                            context,
                            out availableSequenceNumbers,
                            out moreNotifications);

                        // a null message indicates a false alarm and that there were no notifications
                        // to publish and that the request needs to be requeued.
                        if (message != null)
                        {
                            break;
                        }

                        Utils.LogTrace("Publish False Alarm - Request #{0} Requeued.", context.ClientHandle);
                        requeue = true;
                    }
                    finally
                    {
                        queue.PublishCompleted(subscription, moreNotifications);
                    }
                }
                while (requeue);
            }
            finally
            {
                // update diagnostics.
                if (context.Session != null)
                {
                    lock (context.Session.DiagnosticsLock)
                    {
                        var diagnostics = context.Session.SessionDiagnostics;
                        diagnostics.CurrentPublishRequestsInQueue--;
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// Modifies an existing subscription.
        /// </summary>
        public void ModifySubscription(
            UaServerOperationContext context,
            uint subscriptionId,
            double requestedPublishingInterval,
            uint requestedLifetimeCount,
            uint requestedMaxKeepAliveCount,
            uint maxNotificationsPerPublish,
            byte priority,
            out double revisedPublishingInterval,
            out uint revisedLifetimeCount,
            out uint revisedMaxKeepAliveCount)
        {
            revisedPublishingInterval = requestedPublishingInterval;
            revisedLifetimeCount = requestedLifetimeCount;
            revisedMaxKeepAliveCount = requestedMaxKeepAliveCount;

            uint publishingIntervalCount = 0;

            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            var publishingInterval = subscription.PublishingInterval;

            // calculate publishing interval.
            revisedPublishingInterval = CalculatePublishingInterval(requestedPublishingInterval);

            // calculate the keep alive count.
            revisedMaxKeepAliveCount = CalculateKeepAliveCount(revisedPublishingInterval, requestedMaxKeepAliveCount);

            // calculate the lifetime count.
            revisedLifetimeCount = CalculateLifetimeCount(revisedPublishingInterval, revisedMaxKeepAliveCount, requestedLifetimeCount);

            // calculate the max notification count.
            maxNotificationsPerPublish = CalculateMaxNotificationsPerPublish(maxNotificationsPerPublish);

            // update the subscription.
            subscription.Modify(
                context,
                revisedPublishingInterval,
                revisedLifetimeCount,
                revisedMaxKeepAliveCount,
                maxNotificationsPerPublish,
                priority);

            // get the count for the diagnostics.
            publishingIntervalCount = GetPublishingIntervalCount();

            lock (server_.DiagnosticsWriteLock)
            {
                var diagnostics = server_.ServerDiagnostics;
                diagnostics.PublishingIntervalCount = publishingIntervalCount;
            }
        }

        /// <summary>
        /// Sets the publishing mode for a set of subscriptions.
        /// </summary>
        public void SetPublishingMode(
            UaServerOperationContext context,
            bool publishingEnabled,
            UInt32Collection subscriptionIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            var diagnosticsExist = false;
            results = new StatusCodeCollection(subscriptionIds.Count);
            diagnosticInfos = new DiagnosticInfoCollection(subscriptionIds.Count);

            for (var ii = 0; ii < subscriptionIds.Count; ii++)
            {
                try
                {
                    // find subscription.
                    Subscription subscription = null;

                    lock (lock_)
                    {
                        if (!subscriptions_.TryGetValue(subscriptionIds[ii], out subscription))
                        {
                            throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                        }
                    }

                    // update the subscription.
                    subscription.SetPublishingMode(context, publishingEnabled);

                    // save results.
                    results.Add(StatusCodes.Good);

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }
                }
                catch (Exception e)
                {
                    var result = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, String.Empty);
                    results.Add(result.Code);

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, result);
                        diagnosticInfos.Add(diagnosticInfo);
                        diagnosticsExist = true;
                    }
                }

                if (!diagnosticsExist)
                {
                    diagnosticInfos.Clear();
                }
            }
        }

        /// <summary>
        /// Attaches a groups of subscriptions to a different session.
        /// </summary>
        public void TransferSubscriptions(
            UaServerOperationContext context,
            UInt32Collection subscriptionIds,
            bool sendInitialValues,
            out TransferResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            results = new TransferResultCollection();
            diagnosticInfos = new DiagnosticInfoCollection();

            Utils.LogInfo("TransferSubscriptions to SessionId={0}, Count={1}, sendInitialValues={2}",
                context.Session.Id, subscriptionIds.Count, sendInitialValues);

            for (int ii = 0; ii < subscriptionIds.Count; ii++)
            {
                TransferResult result = new TransferResult();
                try
                {
                    // find subscription.
                    Subscription subscription = null;
                    Sessions.Session ownerSession = null;

                    lock (lock_)
                    {
                        if (!subscriptions_.TryGetValue(subscriptionIds[ii], out subscription))
                        {
                            result.StatusCode = StatusCodes.BadSubscriptionIdInvalid;
                            results.Add(result);
                            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                            {
                                diagnosticInfos.Add(null);
                            }
                            continue;
                        }

                        lock (subscription.DiagnosticsLock)
                        {
                            SubscriptionDiagnosticsDataType diagnostics = subscription.Diagnostics;
                            diagnostics.TransferRequestCount++;
                        }

                        // check if new and old sessions are different
                        ownerSession = subscription.Session;
                        if (ownerSession != null)
                        {
                            if (!NodeId.IsNull(ownerSession.Id) && ownerSession.Id == context.Session.Id)
                            {
                                result.StatusCode = StatusCodes.BadNothingToDo;
                                results.Add(result);
                                if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                                {
                                    diagnosticInfos.Add(null);
                                }
                                continue;
                            }
                        }
                    }

                    // get the identity of the current or last owner
                    UserIdentityToken ownerIdentity = subscription.OwnerIdentity;

                    // Validate the identity of the user who owns/owned the subscription
                    // is the same as the new owner.
                    bool validIdentity = Utils.IsEqualUserIdentity(ownerIdentity, context.Session.IdentityToken);

                    // Test if anonymous user is using a
                    // secure session using Sign or SignAndEncrypt
                    if (validIdentity && (ownerIdentity is AnonymousIdentityToken))
                    {
                        var securityMode = context.ChannelContext.EndpointDescription.SecurityMode;
                        if (securityMode != MessageSecurityMode.Sign &&
                            securityMode != MessageSecurityMode.SignAndEncrypt)
                        {
                            validIdentity = false;
                        }
                    }

                    // continue if identity check failed
                    if (!validIdentity)
                    {
                        result.StatusCode = StatusCodes.BadUserAccessDenied;
                        results.Add(result);
                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            diagnosticInfos.Add(null);
                        }
                        continue;
                    }

                    // transfer session, add subscription to publish queue
                    lock (lock_)
                    {
                        subscription.TransferSession(context, sendInitialValues);

                        // remove from queue in old session
                        if (ownerSession != null)
                        {
                            if (publishQueues_.TryGetValue(ownerSession.Id, out var ownerPublishQueue) &&
                                ownerPublishQueue != null)
                            {
                                // keep the queued requests for the status message
                                ownerPublishQueue.Remove(subscription, false);
                            }
                        }

                        // add to queue in new session, create queue if necessary
                        if (!publishQueues_.TryGetValue(context.SessionId, out var publishQueue) ||
                            publishQueue == null)
                        {
                            publishQueues_[context.SessionId] = publishQueue =
                                new SessionPublishQueue(server_, context.Session, maxPublishRequestCount_);
                        }
                        publishQueue.Add(subscription);
                    }

                    lock (statusMessagesLock_)
                    {
                        var processedQueue = new Queue<StatusMessage>();
                        if (statusMessages_.TryGetValue(context.SessionId, out var messagesQueue) &&
                            messagesQueue != null)
                        {
                            // There must not be any messages left from
                            // the transferred subscription
                            foreach (var statusMessage in messagesQueue)
                            {
                                if (statusMessage.SubscriptionId == subscription.Id)
                                {
                                    continue;
                                }
                                processedQueue.Enqueue(statusMessage);
                            }
                        }
                        statusMessages_[context.SessionId] = processedQueue;
                    }

                    if (context.Session != null)
                    {
                        lock (context.Session.DiagnosticsLock)
                        {
                            SessionDiagnosticsDataType diagnostics = context.Session.SessionDiagnostics;
                            diagnostics.CurrentSubscriptionsCount++;
                        }
                    }

                    // raise subscription event.
                    RaiseSubscriptionEvent(subscription, false);
                    result.StatusCode = StatusCodes.Good;

                    // Notify old session with Good_SubscriptionTransferred.
                    if (ownerSession != null)
                    {
                        lock (ownerSession.DiagnosticsLock)
                        {
                            SessionDiagnosticsDataType diagnostics = ownerSession.SessionDiagnostics;
                            diagnostics.CurrentSubscriptionsCount--;
                        }

                        // queue the Good_SubscriptionTransferred message
                        bool statusQueued = false;
                        lock (statusMessagesLock_)
                        {
                            if (!NodeId.IsNull(ownerSession.Id) && statusMessages_.TryGetValue(ownerSession.Id, out var queue))
                            {
                                var message = new StatusMessage {
                                    SubscriptionId = subscription.Id,
                                    Message = subscription.SubscriptionTransferred()
                                };
                                queue.Enqueue(message);
                                statusQueued = true;
                            }
                        }

                        lock (lock_)
                        {
                            // trigger publish response to return status immediately
                            if (publishQueues_.TryGetValue(ownerSession.Id, out var ownerPublishQueue) &&
                                ownerPublishQueue != null)
                            {
                                if (statusQueued)
                                {
                                    // queue the status message
                                    bool success = ownerPublishQueue.TryPublishCustomStatus(StatusCodes.GoodSubscriptionTransferred);
                                    if (!success)
                                    {
                                        Utils.LogWarning("Failed to queue Good_SubscriptionTransferred for SessionId {0}, SubscriptionId {1} due to an empty request queue.",
                                            ownerSession.Id, subscription.Id);
                                    }
                                }

                                // check to remove queued requests if no subscriptions are active
                                ownerPublishQueue.RemoveQueuedRequests();
                            }
                        }
                    }

                    // Return the sequence numbers that are available for retransmission.
                    result.AvailableSequenceNumbers = subscription.AvailableSequenceNumbersForRetransmission();

                    lock (subscription.DiagnosticsLock)
                    {
                        SubscriptionDiagnosticsDataType diagnostics = subscription.Diagnostics;
                        diagnostics.TransferredToSameClientCount++;
                    }

                    // save results.
                    results.Add(result);
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(null);
                    }

                    Utils.LogInfo("Transferred subscription Id {0} to SessionId {1}", subscription.Id, context.Session.Id);
                }
                catch (Exception e)
                {
                    result.StatusCode = StatusCodes.Bad;
                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos.Add(new DiagnosticInfo(e, context.DiagnosticsMask, false, null));
                    }
                }

                for (int i = 0; i < results.Count; i++)
                {
                    server_.ReportAuditTransferSubscriptionEvent(context.AuditEntryId, context.Session, results[i].StatusCode);
                }
            }
        }

        /// <summary>
        /// Republishes a previously published notification message.
        /// </summary>
        public NotificationMessage Republish(
            UaServerOperationContext context,
            uint subscriptionId,
            uint retransmitSequenceNumber)
        {
            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            // fetch the message.
            return subscription.Republish(context, retransmitSequenceNumber);
        }

        /// <summary>
        /// Updates the triggers for the monitored item.
        /// </summary>
        public void SetTriggering(
            UaServerOperationContext context,
            uint subscriptionId,
            uint triggeringItemId,
            UInt32Collection linksToAdd,
            UInt32Collection linksToRemove,
            out StatusCodeCollection addResults,
            out DiagnosticInfoCollection addDiagnosticInfos,
            out StatusCodeCollection removeResults,
            out DiagnosticInfoCollection removeDiagnosticInfos)
        {
            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            // update the triggers.
            subscription.SetTriggering(
                context,
                triggeringItemId,
                linksToAdd,
                linksToRemove,
                out addResults,
                out addDiagnosticInfos,
                out removeResults,
                out removeDiagnosticInfos);
        }

        /// <summary>
        /// Adds monitored items to a subscription.
        /// </summary>
        public void CreateMonitoredItems(
            UaServerOperationContext context,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemCreateRequestCollection itemsToCreate,
            out MonitoredItemCreateResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            var monitoredItemCountIncrement = 0;

            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            var currentMonitoredItemCount = subscription.MonitoredItemCount;

            // create the items.
            subscription.CreateMonitoredItems(
                context,
                timestampsToReturn,
                itemsToCreate,
                out results,
                out diagnosticInfos);

            monitoredItemCountIncrement = subscription.MonitoredItemCount - currentMonitoredItemCount;

            // update diagnostics.
            if (context.Session != null)
            {
                lock (context.Session.DiagnosticsLock)
                {
                    var diagnostics = context.Session.SessionDiagnostics;
                    UpdateCurrentMonitoredItemsCount(diagnostics, monitoredItemCountIncrement);
                }
            }
        }

        /// <summary>
        /// Modifies monitored items in a subscription.
        /// </summary>
        public void ModifyMonitoredItems(
            UaServerOperationContext context,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemModifyRequestCollection itemsToModify,
            out MonitoredItemModifyResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            // modify the items.
            subscription.ModifyMonitoredItems(
                context,
                timestampsToReturn,
                itemsToModify,
                out results,
                out diagnosticInfos);
        }

        /// <summary>
        /// Deletes the monitored items in a subscription.
        /// </summary>
        public void DeleteMonitoredItems(
            UaServerOperationContext context,
            uint subscriptionId,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            var monitoredItemCountIncrement = 0;

            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            var currentMonitoredItemCount = subscription.MonitoredItemCount;

            // create the items.
            subscription.DeleteMonitoredItems(
                context,
                monitoredItemIds,
                out results,
                out diagnosticInfos);

            monitoredItemCountIncrement = subscription.MonitoredItemCount - currentMonitoredItemCount;

            // update diagnostics.
            if (context.Session != null)
            {
                lock (context.Session.DiagnosticsLock)
                {
                    var diagnostics = context.Session.SessionDiagnostics;
                    UpdateCurrentMonitoredItemsCount(diagnostics, monitoredItemCountIncrement);
                }
            }
        }

        /// <summary>
        /// Changes the monitoring mode for a set of items.
        /// </summary>
        public void SetMonitoringMode(
            UaServerOperationContext context,
            uint subscriptionId,
            MonitoringMode monitoringMode,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            // find subscription.
            Subscription subscription = null;

            lock (lock_)
            {
                if (!subscriptions_.TryGetValue(subscriptionId, out subscription))
                {
                    throw new ServiceResultException(StatusCodes.BadSubscriptionIdInvalid);
                }
            }

            // create the items.
            subscription.SetMonitoringMode(
                context,
                monitoringMode,
                monitoredItemIds,
                out results,
                out diagnosticInfos);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Calculates the publishing interval.
        /// </summary>
        protected virtual double CalculatePublishingInterval(double publishingInterval)
        {
            if (Double.IsNaN(publishingInterval) || publishingInterval < minPublishingInterval_)
            {
                publishingInterval = minPublishingInterval_;
            }

            if (publishingInterval > maxPublishingInterval_)
            {
                publishingInterval = maxPublishingInterval_;
            }

            if (publishingInterval < publishingResolution_)
            {
                publishingInterval = publishingResolution_;
            }

            if (publishingInterval % publishingResolution_ != 0)
            {
                publishingInterval = (((int)publishingInterval) / ((int)publishingResolution_) + 1) * publishingResolution_;
            }

            return publishingInterval;
        }

        /// <summary>
        /// Calculates the keep alive count.
        /// </summary>
        protected virtual uint CalculateKeepAliveCount(double publishingInterval, uint keepAliveCount)
        {
            // set default.
            if (keepAliveCount == 0)
            {
                keepAliveCount = 3;
            }

            var keepAliveInterval = keepAliveCount * publishingInterval;

            // keep alive interval cannot be longer than the max subscription lifetime.
            if (keepAliveInterval > maxSubscriptionLifetime_)
            {
                keepAliveCount = (uint)(maxSubscriptionLifetime_ / publishingInterval);

                if (keepAliveCount < UInt32.MaxValue)
                {
                    if (maxSubscriptionLifetime_ % publishingInterval != 0)
                    {
                        keepAliveCount++;
                    }
                }

                keepAliveInterval = keepAliveCount * publishingInterval;
            }

            // the time between publishes cannot exceed the max publishing interval.
            if (keepAliveInterval > maxPublishingInterval_)
            {
                keepAliveCount = (uint)(maxPublishingInterval_ / publishingInterval);

                if (keepAliveCount < UInt32.MaxValue)
                {
                    if (maxPublishingInterval_ % publishingInterval != 0)
                    {
                        keepAliveCount++;
                    }
                }
            }

            return keepAliveCount;
        }

        /// <summary>
        /// Calculates the lifetime count.
        /// </summary>
        protected virtual uint CalculateLifetimeCount(double publishingInterval, uint keepAliveCount, uint lifetimeCount)
        {
            var lifetimeInterval = lifetimeCount * publishingInterval;

            // lifetime cannot be longer than the max subscription lifetime.
            if (lifetimeInterval > maxSubscriptionLifetime_)
            {
                lifetimeCount = (uint)(maxSubscriptionLifetime_ / publishingInterval);

                if (lifetimeCount < UInt32.MaxValue)
                {
                    if (maxSubscriptionLifetime_ % publishingInterval != 0)
                    {
                        lifetimeCount++;
                    }
                }
            }

            // the lifetime must be greater than the keepalive.
            if (keepAliveCount < UInt32.MaxValue / 3)
            {
                if (keepAliveCount * 3 > lifetimeCount)
                {
                    lifetimeCount = keepAliveCount * 3;
                }

                lifetimeInterval = lifetimeCount * publishingInterval;
            }
            else
            {
                lifetimeCount = UInt32.MaxValue;
                lifetimeInterval = Double.MaxValue;
            }

            // apply the minimum.
            if (minSubscriptionLifetime_ > publishingInterval && minSubscriptionLifetime_ > lifetimeInterval)
            {
                lifetimeCount = (uint)(minSubscriptionLifetime_ / publishingInterval);

                if (lifetimeCount < UInt32.MaxValue)
                {
                    if (minSubscriptionLifetime_ % publishingInterval != 0)
                    {
                        lifetimeCount++;
                    }
                }
            }

            return lifetimeCount;
        }

        /// <summary>
        /// Calculates the maximum number of notifications per publish.
        /// </summary>
        protected virtual uint CalculateMaxNotificationsPerPublish(uint maxNotificationsPerPublish)
        {
            if (maxNotificationsPerPublish == 0 || maxNotificationsPerPublish > maxNotificationsPerPublish_)
            {
                return maxNotificationsPerPublish_;
            }

            return maxNotificationsPerPublish;
        }

        /// <summary>
        /// Creates a new instance of a subscription.
        /// </summary>
        protected virtual Subscription CreateSubscription(
            UaServerOperationContext context,
            uint subscriptionId,
            double publishingInterval,
            uint lifetimeCount,
            uint keepAliveCount,
            uint maxNotificationsPerPublish,
            byte priority,
            bool publishingEnabled)
        {
            var subscription = new Subscription(
                server_,
                context.Session,
                subscriptionId,
                publishingInterval,
                lifetimeCount,
                keepAliveCount,
                maxNotificationsPerPublish,
                priority,
                publishingEnabled,
                maxMessageCount_);

            return subscription;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks if there is a status message to return.
        /// </summary>
        private bool ReturnPendingStatusMessage(UaServerOperationContext context, out NotificationMessage message, out uint subscriptionId)
        {
            message = null;
            subscriptionId = 0;

            // check for status messages.
            lock (statusMessagesLock_)
            {
                if (statusMessages_.TryGetValue(context.SessionId, out Queue<StatusMessage> statusQueue))
                {
                    if (statusQueue.Count > 0)
                    {
                        StatusMessage status = statusQueue.Dequeue();
                        subscriptionId = status.SubscriptionId;
                        message = status.Message;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Periodically checks if the sessions have timed out.
        /// </summary>
        private void PublishSubscriptions(object data)
        {
            try
            {
                Utils.LogInfo("Subscription - Publish Thread {0:X8} Started.", Environment.CurrentManagedThreadId);

                var sleepCycle = Convert.ToInt32(data, CultureInfo.InvariantCulture);
                var timeToWait = sleepCycle;

                do
                {
                    var start = DateTime.UtcNow;

                    SessionPublishQueue[] queues = null;
                    Subscription[] abandonedSubscriptions = null;

                    lock (lock_)
                    {
                        // collect active session queues.
                        queues = new SessionPublishQueue[publishQueues_.Count];
                        publishQueues_.Values.CopyTo(queues, 0);

                        // collect abandoned subscriptions.
                        if (abandonedSubscriptions_ != null && abandonedSubscriptions_.Count > 0)
                        {
                            abandonedSubscriptions = new Subscription[abandonedSubscriptions_.Count];

                            for (var ii = 0; ii < abandonedSubscriptions.Length; ii++)
                            {
                                abandonedSubscriptions[ii] = abandonedSubscriptions_[ii];
                            }
                        }
                    }

                    // check the publish timer for each subscription.
                    for (var ii = 0; ii < queues.Length; ii++)
                    {
                        queues[ii].PublishTimerExpired();
                    }

                    // check the publish timer for each abandoned subscription.
                    if (abandonedSubscriptions != null)
                    {
                        var subscriptionsToDelete = new List<Subscription>();

                        for (var ii = 0; ii < abandonedSubscriptions.Length; ii++)
                        {
                            var subscription = abandonedSubscriptions[ii];

                            if (subscription.PublishTimerExpired() != UaPublishingState.Expired)
                            {
                                continue;
                            }

                            if (subscriptionsToDelete == null)
                            {
                                subscriptionsToDelete = new List<Subscription>();
                            }

                            subscriptionsToDelete.Add(subscription);
                            SubscriptionExpired(subscription);
                            Utils.LogInfo("Subscription - Abandoned Subscription Id={0} Delete Scheduled.", subscription.Id);
                        }

                        // schedule cleanup on a background thread.
                        if (subscriptionsToDelete.Count > 0)
                        {
                            lock (lock_)
                            {
                                for (var ii = 0; ii < subscriptionsToDelete.Count; ii++)
                                {
                                    abandonedSubscriptions_.Remove(subscriptionsToDelete[ii]);
                                }
                            }

                            CleanupSubscriptions(server_, subscriptionsToDelete);
                        }
                    }

                    if (shutdownEvent_.WaitOne(timeToWait))
                    {
                        Utils.LogInfo("Subscription - Publish Thread {0:X8} Exited Normally.", Environment.CurrentManagedThreadId);
                        break;
                    }

                    var delay = (int)(DateTime.UtcNow - start).TotalMilliseconds;
                    timeToWait = sleepCycle;
                }
                while (true);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Subscription - Publish Thread {0:X8} Exited Unexpectedly.", Environment.CurrentManagedThreadId);
            }
        }

        /// <summary>
        /// A single thread to execute the condition refresh.
        /// </summary>
        private void ConditionRefreshWorker()
        {
            try
            {
                Utils.LogInfo("Subscription - ConditionRefresh Thread {0:X8} Started.", Environment.CurrentManagedThreadId);

                do
                {
                    ConditionRefreshTask conditionRefreshTask = null; ;

                    lock (conditionRefreshLock_)
                    {
                        if (conditionRefreshQueue_.Count > 0)
                        {
                            conditionRefreshTask = conditionRefreshQueue_.Dequeue();
                        }
                        else
                        {
                            conditionRefreshEvent_.Reset();
                        }
                    }

                    if (conditionRefreshTask == null)
                    {
                        conditionRefreshEvent_.WaitOne();
                    }
                    else
                    {
                        if (conditionRefreshTask.MonitoredItemId == 0)
                        {
                            DoConditionRefresh(conditionRefreshTask.Subscription);
                        }
                        else
                        {
                            DoConditionRefresh2(conditionRefreshTask.Subscription, conditionRefreshTask.MonitoredItemId);
                        }
                    }

                    // use shutdown event to end loop
                    if (shutdownEvent_.WaitOne(0))
                    {
                        Utils.LogInfo("Subscription - ConditionRefresh Thread {0:X8} Exited Normally.", Environment.CurrentManagedThreadId);
                        break;
                    }
                }
                while (true);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Subscription - ConditionRefresh Thread {0:X8} Exited Unexpectedly.", Environment.CurrentManagedThreadId);
            }
        }

        /// <summary>
        /// Cleanups the subscriptions.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="subscriptionsToDelete">The subscriptions to delete.</param>
        internal static void CleanupSubscriptions(IUaServerData server, IList<Subscription> subscriptionsToDelete)
        {
            if (subscriptionsToDelete != null && subscriptionsToDelete.Count > 0)
            {
                Utils.LogInfo("Server - {0} Subscriptions scheduled for delete.", subscriptionsToDelete.Count);

                Task.Run(() => {
                    CleanupSubscriptions(new object[] { server, subscriptionsToDelete });
                });
            }
        }

        /// <summary>
        /// Deletes any expired subscriptions.
        /// </summary>
        internal static void CleanupSubscriptions(object data)
        {
            try
            {
                Utils.LogInfo("Server - CleanupSubscriptions Task Started");

                var args = (object[])data;

                var server = (IUaServerData)args[0];
                var subscriptions = (List<Subscription>)args[1];

                foreach (var subscription in subscriptions)
                {
                    server.DeleteSubscription(subscription.Id);
                }

                Utils.LogInfo("Server - CleanupSubscriptions Task Completed");
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Server - CleanupSubscriptions Task Halted Unexpectedly");
            }
        }
        #endregion

        #region StatusMessage Class
        private class StatusMessage
        {
            public uint SubscriptionId;
            public NotificationMessage Message;
        }
        #endregion

        #region ConditionRefreshTask Class
        private class ConditionRefreshTask
        {
            public ConditionRefreshTask(Subscription subscription, uint monitoredItemId)
            {
                Subscription = subscription;
                MonitoredItemId = monitoredItemId;
            }

            public Subscription Subscription { get; private set; }
            public uint MonitoredItemId { get; private set; }

            public override bool Equals(Object obj)
            {
                var crt = obj as ConditionRefreshTask;

                if (crt != null)
                {
                    if (Subscription?.Id == crt.Subscription?.Id &&
                        MonitoredItemId == crt.MonitoredItemId)
                    {
                        return true;
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Subscription.Id, MonitoredItemId);
            }
        }
        #endregion

        #region Private Fields
        private object lock_ = new object();
        private long lastSubscriptionId_;
        private IUaServerData server_;
        private double minPublishingInterval_;
        private double maxPublishingInterval_;
        private int publishingResolution_;
        private uint maxSubscriptionLifetime_;
        private uint minSubscriptionLifetime_;
        private uint maxMessageCount_;
        private uint maxNotificationsPerPublish_;
        private int maxPublishRequestCount_;
        private int maxSubscriptionCount_;
        private Dictionary<uint, Subscription> subscriptions_;
        private List<Subscription> abandonedSubscriptions_;
        private Dictionary<NodeId, Queue<StatusMessage>> statusMessages_;
        private object statusMessagesLock_ = new object();
        private Dictionary<NodeId, SessionPublishQueue> publishQueues_;
        private ManualResetEvent shutdownEvent_;
        private Queue<ConditionRefreshTask> conditionRefreshQueue_;
        private ManualResetEvent conditionRefreshEvent_;

        private object eventLock_ = new object();
        private object conditionRefreshLock_ = new object();
        private event EventHandler<SubscriptionEventArgs> SubscriptionCreatedEventHandler;
        private event EventHandler<SubscriptionEventArgs> SubscriptionDeletedEventHandler;
        #endregion
    }
}
