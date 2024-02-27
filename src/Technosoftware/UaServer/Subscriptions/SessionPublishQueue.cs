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
using System.Globalization;
using System.Text;
using System.Threading;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer.Subscriptions
{
    /// <summary>
    /// Manages the publish queues for a session.
    /// </summary>
    public class SessionPublishQueue : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new queue.
        /// </summary>
        public SessionPublishQueue(IUaServerData server, Sessions.Session session, int maxPublishRequests)
        {
            if (server == null)  throw new ArgumentNullException(nameof(server));
            if (session == null) throw new ArgumentNullException(nameof(session));

            server_              = server;
            session_             = session;
            publishEvent_        = new ManualResetEvent(false);
            queuedRequests_      = new LinkedList<QueuedRequest>();
            queuedSubscriptions_ = new List<QueuedSubscription>();
            maxPublishRequests_  = maxPublishRequests;
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
                    publishEvent_.Set();

                    while (queuedRequests_.Count > 0)
                    {
                        var request = queuedRequests_.First.Value;
                        queuedRequests_.RemoveFirst();

                        try
                        {
                            request.Error = StatusCodes.BadServerHalted;
                            request.Dispose();
                        }
                        catch (Exception)
                        {
                            // ignore errors.
                        }
                    }

                    queuedSubscriptions_.Clear();
                    publishEvent_.Dispose();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Clears the queues because the session is closing.
        /// </summary>
        /// <returns>The list of subscriptions in the queue.</returns>
        public IList<Subscription> Close()
        {
            lock (lock_)
            {
                // TraceState("SESSION CLOSED");

                // wake up any waiting publish requests.
                publishEvent_.Set();

                while (queuedRequests_.Count > 0)
                {
                    var request = queuedRequests_.First.Value;
                    queuedRequests_.RemoveFirst();
                    request.Error = StatusCodes.BadSessionClosed;
                    request.Set();
                }

                // tell the subscriptions that the session is closed.
                var subscriptions = new Subscription[queuedSubscriptions_.Count];

                for (var ii = 0; ii < queuedSubscriptions_.Count; ii++)
                {
                    subscriptions[ii] = queuedSubscriptions_[ii].Subscription;
                    subscriptions[ii].SessionClosed();
                }

                // clear the queue.
                queuedSubscriptions_.Clear();

                return subscriptions;
            }
        }

        /// <summary>
        /// Adds a subscription from the publish queue.
        /// </summary>
        public void Add(Subscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            lock (lock_)
            {
                var queuedSubscription = new QueuedSubscription();

                queuedSubscription.ReadyToPublish = false;
                queuedSubscription.Timestamp = DateTime.UtcNow;
                queuedSubscription.Subscription = subscription;

                queuedSubscriptions_.Add(queuedSubscription);

                // TraceState("SUBSCRIPTION QUEUED");          
            }
        }

        /// <summary>
        /// Removes a subscription from the publish queue.
        /// </summary>
        public void Remove(Subscription subscription, bool removeQueuedRequests)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            lock (lock_)
            {
                // remove the subscription from the queue.
                for (var ii = 0; ii < queuedSubscriptions_.Count; ii++)
                {
                    if (Object.ReferenceEquals(queuedSubscriptions_[ii].Subscription, subscription))
                    {
                        queuedSubscriptions_.RemoveAt(ii);
                        break;
                    }
                }

                if (removeQueuedRequests)
                {
                    RemoveQueuedRequests();
                }

                // TraceState("SUBSCRIPTION REMOVED");
            }
        }

        /// <summary>
        /// Removes outstanding requests if no 
        /// </summary>
        public void RemoveQueuedRequests()
        {
            lock (lock_)
            {
                // remove any outstanding publishes.
                if (queuedSubscriptions_.Count == 0)
                {
                    while (queuedRequests_.Count > 0)
                    {
                        var request = queuedRequests_.First.Value;
                        request.Error = StatusCodes.BadNoSubscription;
                        request.Set();
                        queuedRequests_.RemoveFirst();
                    }
                }
            }
        }

        /// <summary>
        /// Try to publish a custom status message
        /// using a queued publish request.
        /// </summary>
        public bool TryPublishCustomStatus(StatusCode statusCode)
        {
            lock (lock_)
            {
                if (queuedRequests_.Count > 0)
                {
                    QueuedRequest request = queuedRequests_.Last.Value;
                    request.Error = statusCode;
                    request.Set();
                    queuedRequests_.RemoveLast();
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Processes acknowledgements for previously published messages.
        /// </summary>
        public void Acknowledge(
            UaServerOperationContext                      context,
            SubscriptionAcknowledgementCollection subscriptionAcknowledgements,
            out StatusCodeCollection              acknowledgeResults,
            out DiagnosticInfoCollection          acknowledgeDiagnosticInfos)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (subscriptionAcknowledgements == null) throw new ArgumentNullException(nameof(subscriptionAcknowledgements));

            lock (lock_)
            {
                var diagnosticsExist = false;
                acknowledgeResults = new StatusCodeCollection(subscriptionAcknowledgements.Count);
                acknowledgeDiagnosticInfos = new DiagnosticInfoCollection(subscriptionAcknowledgements.Count);

                for (var ii = 0; ii < subscriptionAcknowledgements.Count; ii++)
                {
                    var acknowledgement = subscriptionAcknowledgements[ii];

                    var found = false;

                    for (var jj = 0; jj < queuedSubscriptions_.Count; jj++)
                    {
                        var subscription = queuedSubscriptions_[jj];

                        if (subscription.Subscription.Id == acknowledgement.SubscriptionId)
                        {
                            var result = subscription.Subscription.Acknowledge(context, acknowledgement.SequenceNumber);

                            if (ServiceResult.IsGood(result))
                            {
                                acknowledgeResults.Add(StatusCodes.Good);

                                if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                                {
                                    acknowledgeDiagnosticInfos.Add(null);
                                }
                            }
                            else
                            {
                                acknowledgeResults.Add(result.Code);

                                if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                                {
                                    var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, result);
                                    acknowledgeDiagnosticInfos.Add(diagnosticInfo);
                                    diagnosticsExist = true;
                                }
                            }

                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var result = new ServiceResult(StatusCodes.BadSubscriptionIdInvalid);
                        acknowledgeResults.Add(result.Code);

                        if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                        {
                            var diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(server_, context, result);
                            acknowledgeDiagnosticInfos.Add(diagnosticInfo);
                            diagnosticsExist = true;
                        }
                    }
                }

                if (!diagnosticsExist)
                {
                    acknowledgeDiagnosticInfos.Clear();
                }
            }
        }

        /// <summary>
        /// Returns a subscription that is ready to publish.
        /// </summary>
        public Subscription Publish(uint clientHandle, DateTime deadline, bool requeue, AsyncPublishOperation operation)
        {
            QueuedRequest request = null;

            // DateTime queueTime = DateTime.UtcNow;
            // DateTime dequeueTime = DateTime.UtcNow;

            lock (lock_)
            {
                if (queuedSubscriptions_.Count == 0)
                {
                    // TraceState("PUBLISH ERROR (BadNoSubscription)");
                    throw new ServiceResultException(StatusCodes.BadNoSubscription);
                }

                // find the waiting subscription with the highest priority.
                var subscriptions = new List<QueuedSubscription>();

                for (var ii = 0; ii < queuedSubscriptions_.Count; ii++)
                {
                    var subscription = queuedSubscriptions_[ii];

                    if (subscription.ReadyToPublish && !subscription.Publishing)
                    {
                        subscriptions.Add(subscription);
                    }
                }

                // find waiting the subscription that has been waiting the longest.
                if (subscriptions.Count > 0)
                {
                    byte maxPriority = 0;
                    var earliestTimestamp = DateTime.MaxValue;
                    QueuedSubscription subscriptionToPublish = null;

                    for (var ii = 0; ii < subscriptions.Count; ii++)
                    {
                        var subscription = subscriptions[ii];
                        var priority = subscription.Subscription.Priority;
                        if (priority > maxPriority)
                        {
                            maxPriority = priority;
                            earliestTimestamp = DateTime.MaxValue;
                        }

                        if (priority >= maxPriority && earliestTimestamp > subscription.Timestamp)
                        {
                            earliestTimestamp = subscription.Timestamp;
                            subscriptionToPublish = subscription;
                        }
                    }

                    // reset subscriptions flag.
                    subscriptionsWaiting_ = false;

                    for (var jj = 0; jj < queuedSubscriptions_.Count; jj++)
                    {
                        if (queuedSubscriptions_[jj].ReadyToPublish)
                        {
                            subscriptionsWaiting_ = true;
                            break;
                        }
                    }

                    // TraceState("REQUEST #{0} ASSIGNED TO WAITING SUBSCRIPTION", clientHandle);
                    subscriptionToPublish.Publishing = true;
                    return subscriptionToPublish.Subscription;
                }

                // queue request because there is nothing waiting.
                if (subscriptions.Count == 0)
                {
                    var node = queuedRequests_.First;

                    while (node != null)
                    {
                        var next = node.Next;
                        var queuedRequest = node.Value;
                        StatusCode requestStatus = StatusCodes.Good;

                        // check if expired.
                        if (queuedRequest.Deadline < DateTime.MaxValue && queuedRequest.Deadline.AddMilliseconds(500) < DateTime.UtcNow)
                        {
                            requestStatus = StatusCodes.BadTimeout;
                        }

                        // check secure channel.
                        else if (!session_.IsSecureChannelValid(queuedRequest.SecureChannelId))
                        {
                            requestStatus = StatusCodes.BadSecureChannelIdInvalid;
                        }

                        // remove bad requests.
                        if (StatusCode.IsBad(requestStatus))
                        {
                            queuedRequest.Error = requestStatus;
                            queuedRequest.Set();
                            queuedRequests_.Remove(node);
                        }

                        node = next;
                    }

                    // clear excess requests - keep the newest ones.
                    while (maxPublishRequests_ > 0 && queuedRequests_.Count >= maxPublishRequests_)
                    {
                        request = queuedRequests_.First.Value;
                        request.Error = StatusCodes.BadTooManyPublishRequests;
                        request.Set();
                        queuedRequests_.RemoveFirst();
                    }

                    request = new QueuedRequest();

                    request.SecureChannelId = SecureChannelContext.Current.SecureChannelId;
                    request.Deadline = deadline;
                    request.Subscription = null;
                    request.Error = StatusCodes.Good;

                    if (operation == null)
                    {
                        request.Event = new ManualResetEvent(false);
                    }
                    else
                    {
                        request.Operation = operation;
                    }

                    if (requeue)
                    {
                        queuedRequests_.AddFirst(request);
                        // TraceState("REQUEST #{0} RE-QUEUED", clientHandle);
                    }
                    else
                    {
                        queuedRequests_.AddLast(request);
                        // TraceState("REQUEST #{0} QUEUED", clientHandle);
                    }
                }
            }

            // check for non-blocking operation.
            if (operation != null)
            {
                // TraceState("PUBLISH: #{0} Async Request Queued.", clientHandle);
                return null;
            }

            // wait for subscription.
            var error = request.Wait(Timeout.Infinite);

            // check for error.
            if (ServiceResult.IsGood(error))
            {
                if (StatusCode.IsBad(request.Error))
                {
                    error = request.Error;
                }
            }

            // must reassign subscription on error.
            if (ServiceResult.IsBad(request.Error))
            {
                if (request.Subscription != null)
                {
                    lock (lock_)
                    {
                        request.Subscription.Publishing = false;
                        AssignSubscriptionToRequest(request.Subscription);
                    }
                }

                // TraceState("REQUEST #{0} PUBLISH ERROR ({1})", clientHandle, error.StatusCode);
                throw new ServiceResultException(request.Error);
            }
            // special case to force a status message is handled correctly
            else if (request.Error == StatusCodes.GoodSubscriptionTransferred)
            {
                if (request.Subscription != null)
                {
                    lock (lock_)
                    {
                        request.Subscription.Publishing = false;
                        AssignSubscriptionToRequest(request.Subscription);
                    }
                    request.Subscription = null;
                }
                return null;
            }

            // must be shuting down if this is null but no error.
            if (request.Subscription == null)
            {
                throw new ServiceResultException(StatusCodes.BadNoSubscription);
            }

            // TraceState("REQUEST #{0} ASSIGNED", clientHandle);
            // return whatever was assigned.
            return request.Subscription.Subscription;
        }

        /// <summary>
        /// Completes the publish.
        /// </summary>
        /// <param name="requeue">if set to <c>true</c> the request must be requeued.</param>
        /// <param name="operation">The asynchronous operation.</param>
        /// <param name="calldata">The calldata.</param>
        /// <returns></returns>
        public Subscription CompletePublish(
            bool requeue,
            AsyncPublishOperation operation,
            object calldata)
        {
            Utils.LogTrace("PUBLISH: #{0} Completing", operation.RequestHandle, requeue);

            var request = (QueuedRequest)calldata;

            // check if need to requeue.
            lock (lock_)
            {
                if (requeue)
                {
                    request.Subscription = null;
                    request.Error = StatusCodes.Good;
                    queuedRequests_.AddFirst(request);
                    return null;
                }
            }

            // must reassign subscription on error.
            if (ServiceResult.IsBad(request.Error))
            {
                Utils.LogTrace("PUBLISH: #{0} Reassigned ERROR({1})", operation.RequestHandle, request.Error);

                if (request.Subscription != null)
                {
                    lock (lock_)
                    {
                        request.Subscription.Publishing = false;
                        AssignSubscriptionToRequest(request.Subscription);
                    }
                }

                // TraceState("REQUEST #{0} PUBLISH ERROR ({1})", clientHandle, error.StatusCode);
                throw new ServiceResultException(request.Error);
            }

            // must be shuting down if this is null but no error.
            if (request.Subscription == null)
            {
                throw new ServiceResultException(StatusCodes.BadNoSubscription);
            }

            // return whatever was assigned.
            return request.Subscription.Subscription;
        }

        /// <summary>
        /// Adds a subscription back into the queue because it has more notifications to publish.
        /// </summary>
        public void PublishCompleted(Subscription subscription, bool moreNotifications)
        {
            lock (lock_)
            {
                for (var ii = 0; ii < queuedSubscriptions_.Count; ii++)
                {
                    if (Object.ReferenceEquals(queuedSubscriptions_[ii].Subscription, subscription))
                    {
                        queuedSubscriptions_[ii].Publishing = false;

                        if (moreNotifications)
                        {
                            AssignSubscriptionToRequest(queuedSubscriptions_[ii]);
                        }
                        else
                        {
                            queuedSubscriptions_[ii].ReadyToPublish = false;
                            queuedSubscriptions_[ii].Timestamp = DateTime.UtcNow;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks the state of the subscriptions.
        /// </summary>
        public void PublishTimerExpired()
        {
            var subscriptionsToDelete = new List<Subscription>();

            lock (lock_)
            {
                var liveSubscriptions = new List<QueuedSubscription>(queuedSubscriptions_.Count);

                // check each available subscription.
                for (var ii = 0; ii < queuedSubscriptions_.Count; ii++)
                {
                    var subscription = queuedSubscriptions_[ii];

                    var state = subscription.Subscription.PublishTimerExpired();

                    // check for expired subscription.
                    if (state == UaPublishingState.Expired)
                    {
                        subscriptionsToDelete.Add(subscription.Subscription);
                        ((SubscriptionManager)server_.SubscriptionManager).SubscriptionExpired(subscription.Subscription);
                        continue;
                    }

                    liveSubscriptions.Add(subscription);

                    // check if idle.
                    if (state == UaPublishingState.Idle)
                    {
                        subscription.ReadyToPublish = false;
                        continue;
                    }

                    // do nothing if subscription has already been flagged as available.
                    if (subscription.ReadyToPublish)
                    {
                        if (subscription.ReadyToPublish && queuedRequests_.Count == 0)
                        {
                            if (!subscriptionsWaiting_)
                            {
                                subscriptionsWaiting_ = true;
                                // TraceState("SUBSCRIPTIONS WAITING");
                            }
                        }

                        continue;
                    }

                    // assign subscription to request if one is available.
                    if (!subscription.Publishing)
                    {
                        AssignSubscriptionToRequest(subscription);
                    }
                }

                // only keep the live subscriptions.
                queuedSubscriptions_ = liveSubscriptions;

                // schedule cleanup on a background thread.
                SubscriptionManager.CleanupSubscriptions(server_, subscriptionsToDelete);
            }
        }

        /// <summary>
        /// Checks the state of the subscriptions.
        /// </summary>
        private void AssignSubscriptionToRequest(QueuedSubscription subscription)
        {
            // find a request.
            for (var node = queuedRequests_.First; node != null; node = node.Next)
            {
                var request = node.Value;

                StatusCode error = StatusCodes.Good;

                // check if expired.
                if (request.Deadline < DateTime.MaxValue && request.Deadline.AddMilliseconds(500) < DateTime.UtcNow)
                {
                    error = StatusCodes.BadTimeout;
                }

                // check secure channel.
                else if (!session_.IsSecureChannelValid(request.SecureChannelId))
                {
                    error = StatusCodes.BadSecureChannelIdInvalid;
                    Utils.LogWarning("Publish abandoned because the secure channel changed.");
                }

                if (StatusCode.IsBad(error))
                {
                    // remove request.
                    var next = node.Next;
                    queuedRequests_.Remove(node);
                    node = next;

                    // wake up thread with error.
                    request.Error = error;
                    request.Set();

                    if (node == null)
                    {
                        break;
                    }

                    continue;
                }

                // remove request.
                queuedRequests_.Remove(node);

                Utils.LogTrace("PUBLISH: #000 Assigned To Subscription({0}).", subscription.Subscription.Id);

                request.Error = StatusCodes.Good;
                request.Subscription = subscription;
                request.Subscription.Publishing = true;
                request.Set();
                return;
            }

            // mark it as available.
            subscription.ReadyToPublish = true;
            subscription.Timestamp = DateTime.UtcNow;
        }
        #endregion

        #region QueuedRequest Class
        /// <summary>
        /// A request queued while waiting for a subscription.
        /// </summary>
        private class QueuedRequest : IDisposable
        {
            public ManualResetEvent Event;
            public AsyncPublishOperation Operation;
            public DateTime Deadline;
            public StatusCode Error;
            public QueuedSubscription Subscription;
            public string SecureChannelId;

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
                    this.Error = StatusCodes.BadServerHalted;

                    if (this.Operation != null)
                    {
                        this.Operation.Dispose();
                        this.Operation = null;
                    }

                    if (this.Event != null)
                    {
                        try
                        {
                            this.Event.Set();
                            this.Event.Dispose();
                        }
                        catch (Exception)
                        {
                            // ignore errors.
                        }
                    }
                }
            }
            #endregion

            /// <summary>
            /// Waits for the request to be processed.
            /// </summary>
            public ServiceResult Wait(int timeout)
            {
                try
                {
                    // do not block for an async operation.
                    if (Operation != null)
                    {
                        return StatusCodes.BadWouldBlock;
                    }

                    if (!Event.WaitOne(timeout))
                    {
                        return StatusCodes.BadTimeout;
                    }

                    return ServiceResult.Good;
                }
                catch (Exception e)
                {
                    return ServiceResult.Create(e, StatusCodes.BadTimeout, "Unexpected error waiting for subscription.");
                }
                finally
                {
                    try
                    {
                        Event.Dispose();
                    }
                    catch (Exception)
                    {
                        // ignore errors on close.                       
                    }
                }
            }

            /// <summary>
            /// Sets the event that wakes up the publish thread.
            /// </summary>
            public void Set()
            {
                try
                {
                    if (Operation != null)
                    {
                        Operation.CompletePublish(this);
                        return;
                    }

                    Event.Set();
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Publish request no longer available.");
                }
            }
        }
        #endregion

        #region QueuedSubscription Class
        /// <summary>
        /// Stores a subscription that has notifications ready to be sent back to the client.
        /// </summary>
        private class QueuedSubscription
        {
            public Subscription Subscription;
            public DateTime Timestamp;
            public bool ReadyToPublish;
            public bool Publishing;
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Dumps the current state of the session queue.
        /// </summary>
        internal void TraceState(string context, params object[] args)
        {
            // TODO: implement as EventSource
            if (!Utils.Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
            {
                return;
            }

            var buffer = new StringBuilder();

            lock (lock_)
            {
                buffer.Append("PublishQueue ");
                buffer.AppendFormat(CultureInfo.InvariantCulture, context, args);

                if (session_ != null)
                {
                    buffer.AppendFormat(CultureInfo.InvariantCulture, ", SessionId={0}", session_.Id);
                }

                buffer.AppendFormat(CultureInfo.InvariantCulture, ", SubscriptionCount={0}, RequestCount={1}",
                    queuedSubscriptions_.Count, queuedRequests_.Count);

                var readyToPublish = 0;

                for (var ii = 0; ii < queuedSubscriptions_.Count; ii++)
                {
                    if (queuedSubscriptions_[ii].ReadyToPublish)
                    {
                        readyToPublish++;
                    }
                }

                buffer.AppendFormat(CultureInfo.InvariantCulture, ", ReadyToPublishCount={0}", readyToPublish);

                var expiredRequests = 0;

                for (var ii = queuedRequests_.First; ii != null; ii = ii.Next)
                {
                    if (ii.Value.Deadline < DateTime.UtcNow)
                    {
                        expiredRequests++;
                    }
                }

                buffer.AppendFormat(CultureInfo.InvariantCulture, ", ExpiredCount={0}", expiredRequests);
            }

            Utils.LogTrace(buffer.ToString());
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private Sessions.Session session_;
        private ManualResetEvent publishEvent_;
        private LinkedList<QueuedRequest> queuedRequests_;
        private List<QueuedSubscription> queuedSubscriptions_;
        private int maxPublishRequests_;
        private bool subscriptionsWaiting_;
        #endregion
    }
}
