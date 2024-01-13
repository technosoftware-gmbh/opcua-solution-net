#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Unified Architecture .NET Standard Libraries 
// and Samples. The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

using Opc.Ua;
using static Opc.Ua.Utils;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// EventSource for the server library.
    /// </summary>
    public static partial class UaClientUtils
    {
        /// <summary>
        /// The EventSource log interface.
        /// </summary>
        internal static UaClientEventSource EventLog { get; } = new UaClientEventSource();
    }

    /// <summary>
    /// Event source for high performance logging.
    /// </summary>
    [EventSource(Name = "Technosoftware.UAClient", Guid = "70A02853-C24D-4C36-8A3B-0AD181BE7D46")]
    internal sealed class UaClientEventSource : EventSource
    {
        private const int SubscriptionStateId = 1;
        private const int NotificationId = SubscriptionStateId + 1;
        private const int NotificationReceivedId = NotificationId + 1;
        private const int PublishStartId = NotificationId + 1;
        private const int PublishStopId = PublishStartId + 1;

        /// <summary>
        /// The client messages.
        /// </summary>
        private const string SubscriptionStateMessage = "Subscription {0}, Id={1}, LastNotificationTime={2:HH:mm:ss}, GoodPublishRequestCount={3}, PublishingInterval={4}, KeepAliveCount={5}, PublishingEnabled={6}, MonitoredItemCount={7}";
        private const string NotificationMessage = "Notification: ClientHandle={0}, Value={1}";
        private const string NotificationReceivedMessage = "NOTIFICATION RECEIVED: SubId={0}, SeqNo={1}";
        private const string PublishStartMessage = "PUBLISH #{0} SENT";
        private const string PublishStopMessage = "PUBLISH #{0} RECEIVED";

        /// <summary>
        /// The Client Event Ids used for event messages, when calling ILogger.
        /// </summary>
        private readonly EventId subscriptionStateMessageEventId_ = new EventId(TraceMasks.Operation, nameof(SubscriptionState));
        private readonly EventId notificationEventId_ = new EventId(TraceMasks.Operation, nameof(Notification));
        private readonly EventId notificationReceivedEventId_ = new EventId(TraceMasks.Operation, nameof(NotificationReceived));
        private readonly EventId publishStartEventId_ = new EventId(TraceMasks.ServiceDetail, nameof(PublishStart));
        private readonly EventId publishStopEventId_ = new EventId(TraceMasks.ServiceDetail, nameof(PublishStop));

        /// <summary>
        /// The state of the client subscription.
        /// </summary>
        [Event(SubscriptionStateId, Message = SubscriptionStateMessage, Level = EventLevel.Verbose)]
        public void SubscriptionState(string context, uint id, DateTime lastNotificationTime, int goodPublishRequestCount,
            double currentPublishingInterval, uint currentKeepAliveCount, bool currentPublishingEnabled, uint monitoredItemCount)
        {
            if (IsEnabled())
            {
                WriteEvent(SubscriptionStateId, context, id, lastNotificationTime, goodPublishRequestCount,
                    currentPublishingInterval, currentKeepAliveCount, currentPublishingEnabled, monitoredItemCount);
            }
            else if (Utils.Logger.IsEnabled(LogLevel.Information))
            {
                Utils.LogInfo(subscriptionStateMessageEventId_, SubscriptionStateMessage,
                    context, id, lastNotificationTime, goodPublishRequestCount,
                    currentPublishingInterval, currentKeepAliveCount, currentPublishingEnabled, monitoredItemCount);
            }
        }

        /// <summary>
        /// The notification message. Called internally to convert wrapped value.
        /// </summary>
        [Event(NotificationId, Message = NotificationMessage, Level = EventLevel.Verbose)]
        public void Notification(int clientHandle, string value)
        {
            WriteEvent(NotificationId, value, clientHandle);
        }

        /// <summary>
        /// A notification received in Publish complete.
        /// </summary>
        [Event(NotificationReceivedId, Message = NotificationReceivedMessage, Level = EventLevel.Verbose)]
        public void NotificationReceived(int subscriptionId, int sequenceNumber)
        {
            if (IsEnabled())
            {
                WriteEvent(NotificationReceivedId, subscriptionId, sequenceNumber);
            }
            else if (Utils.Logger.IsEnabled(LogLevel.Trace))
            {
                Utils.LogTrace(notificationReceivedEventId_, NotificationReceivedMessage,
                    subscriptionId, sequenceNumber);
            }
        }

        /// <summary>
        /// A Publish begin received.
        /// </summary>
        [Event(PublishStartId, Message = PublishStartMessage, Level = EventLevel.Verbose)]
        public void PublishStart(int requestHandle)
        {
            if (IsEnabled())
            {
                WriteEvent(PublishStartId, requestHandle);
            }
            else if (Utils.Logger.IsEnabled(LogLevel.Trace))
            {
                Utils.LogTrace(publishStartEventId_, PublishStartMessage, requestHandle);
            }
        }

        /// <summary>
        /// A Publish complete received.
        /// </summary>
        [Event(PublishStopId, Message = PublishStopMessage, Level = EventLevel.Verbose)]
        public void PublishStop(int requestHandle)
        {
            if (IsEnabled())
            {
                WriteEvent(PublishStopId, requestHandle);
            }
            else if (Utils.Logger.IsEnabled(LogLevel.Trace))
            {
                Utils.LogTrace(publishStopEventId_, PublishStopMessage, requestHandle);
            }
        }

        /// <summary>
        /// Log a Notification.
        /// </summary>
        [NonEvent]
        public void NotificationValue(uint clientHandle, Variant wrappedValue)
        {
            // expensive operation, only enable if tracemask set
            if ((Utils.TraceMask & Utils.TraceMasks.OperationDetail) != 0)
            {
                if (IsEnabled())
                {
                    Notification((int)clientHandle, wrappedValue.ToString());
                }
                else if (Utils.Logger.IsEnabled(LogLevel.Trace))
                {
                    Utils.LogTrace(notificationEventId_, NotificationMessage,
                        clientHandle, wrappedValue);
                }
            }
        }
    }
}
