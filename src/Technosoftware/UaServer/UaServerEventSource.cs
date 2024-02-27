#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

using Opc.Ua;
using static Opc.Ua.Utils;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// The local EventSource for the server library.
    /// </summary>
    public static partial class UaServerUtils
    {
        /// <summary>
        /// The EventSource log interface.
        /// </summary>
        internal static UaServerEventSource EventLog { get; } = new UaServerEventSource();
    }

    /// <summary>
    /// Event source for high performance logging.
    /// </summary>
    [EventSource(Name = "Technosoftware.UAServer", Guid = "22B5B923-D612-4F47-B413-1FBEA289B475")]
    internal sealed class UaServerEventSource : EventSource
    {
        // client event ids
        private const int SendResponseId = 1;
        private const int ServerCallId = SendResponseId + 1;
        private const int SessionStateId = ServerCallId + 1;
        private const int MonitoredItemReadyId = SessionStateId + 1;

        /// <summary>
        /// The server messages used in event messages.
        /// </summary>
        private const string SendResponseMessage = "ChannelId {0}: SendResponse {1}";
        private const string ServerCallMessage = "Server Call={0}, Id={1}";
        private const string SessionStateMessage = "Session {0}, Id={1}, Name={2}, ChannelId={3}, User={4}";
        private const string MonitoredItemReadyMessage = "IsReadyToPublish[{0}] {1}";

        /// <summary>
        /// The Server ILogger event Ids used for event messages, when calling back to ILogger.
        /// </summary>
        private readonly EventId sendResponseEventId_ = new EventId(TraceMasks.ServiceDetail, nameof(SendResponse));
        private readonly EventId serverCallEventId_ = new EventId(TraceMasks.ServiceDetail, nameof(ServerCall));
        private readonly EventId sessionStateMessageEventId_ = new EventId(TraceMasks.Information, nameof(SessionState));
        private readonly EventId monitoredItemReadyEventId_ = new EventId(TraceMasks.OperationDetail, nameof(MonitoredItemReady));

        /// <summary>
        /// The send response.
        /// </summary>
        [Event(SendResponseId, Message = SendResponseMessage, Level = EventLevel.Verbose)]
        public void SendResponse(uint channelId, uint requestId)
        {
            if (IsEnabled())
            {
                WriteEvent(SendResponseId, channelId, requestId);
            }
            else if ((TraceMask & TraceMasks.ServiceDetail) != 0 &&
                Logger.IsEnabled(LogLevel.Trace))
            {
                LogTrace(sendResponseEventId_, SendResponseMessage, channelId, requestId);
            }
        }

        /// <summary>
        /// A server call message.
        /// </summary>
        [Event(ServerCallId, Message = ServerCallMessage, Level = EventLevel.Informational)]
        public void ServerCall(string requestType, uint requestId)
        {
            if (IsEnabled())
            {
                WriteEvent(ServerCallId, requestType, requestId);
            }
            else if ((TraceMask & TraceMasks.ServiceDetail) != 0 &&
                Logger.IsEnabled(LogLevel.Trace))
            {
                LogTrace(serverCallEventId_, ServerCallMessage, requestType, requestId);
            }
        }

        /// <summary>
        /// The state of the session.
        /// </summary>
        [Event(SessionStateId, Message = SessionStateMessage, Level = EventLevel.Informational)]
        public void SessionState(string context, string sessionId, string sessionName, string secureChannelId, string identity)
        {
            if (IsEnabled())
            {
                WriteEvent(SessionStateId, context, sessionId, sessionName, secureChannelId, identity);
            }
            else if (Logger.IsEnabled(LogLevel.Information))
            {
                LogInfo(sessionStateMessageEventId_, SessionStateMessage, context, sessionId, sessionName, secureChannelId, identity);
            }
        }

        /// <summary>
        /// The state of the server session.
        /// </summary>
        [Event(MonitoredItemReadyId, Message = MonitoredItemReadyMessage, Level = EventLevel.Verbose)]
        public void MonitoredItemReady(uint id, string state)
        {
            if ((TraceMask & TraceMasks.OperationDetail) != 0)
            {
                if (IsEnabled())
                {
                    WriteEvent(MonitoredItemReadyId, id, state);
                }
                else if (Logger.IsEnabled(LogLevel.Trace))
                {
                    LogTrace(monitoredItemReadyEventId_, MonitoredItemReadyMessage, id, state);
                }
            }
        }

        /// <summary>
        /// Log a WriteValue.
        /// </summary>
        [NonEvent]
        public void WriteValueRange(NodeId nodeId, Variant wrappedValue, string range)
        {
            if ((TraceMask & TraceMasks.ServiceDetail) != 0)
            {
                if (IsEnabled())
                {
                    //WriteEvent();
                }
                else if (Logger.IsEnabled(LogLevel.Trace))
                {
                    LogTrace(TraceMasks.ServiceDetail, "WRITE: NodeId={0} Value={1} Range={2}", nodeId, wrappedValue, range);
                }
            }
        }

        /// <summary>
        /// Log a ReadValue.
        /// </summary>
        [NonEvent]
        public void ReadValueRange(NodeId nodeId, Variant wrappedValue, string range)
        {
            if ((TraceMask & TraceMasks.ServiceDetail) != 0)
            {
                if (IsEnabled())
                {
                    //WriteEvent();
                }
                else if (Logger.IsEnabled(LogLevel.Trace))
                {
                    LogTrace(TraceMasks.ServiceDetail, "READ: NodeId={0} Value={1} Range={2}", nodeId, wrappedValue, range);
                }
            }
        }

        /// <summary>
        /// Log a Queued Value.
        /// </summary>
        [NonEvent]
        public void EnqueueValue(Variant wrappedValue)
        {
            if ((TraceMask & TraceMasks.OperationDetail) != 0)
            {
                if (IsEnabled())
                {
                    //WriteEvent();
                }
                else if (Logger.IsEnabled(LogLevel.Trace))
                {
                    LogTrace("ENQUEUE VALUE: Value={0}", wrappedValue);
                }
            }
        }

        /// <summary>
        /// Log a Dequeued Value.
        /// </summary>
        [NonEvent]
        public void DequeueValue(Variant wrappedValue, StatusCode statusCode)
        {
            if ((TraceMask & TraceMasks.OperationDetail) != 0)
            {
                if (IsEnabled())
                {
                    //WriteEvent();
                }
                else if (Logger.IsEnabled(LogLevel.Trace))
                {
                    LogTrace("DEQUEUE VALUE: Value={0} CODE={1}<{2:X8}> OVERFLOW={3}",
                        wrappedValue, statusCode.Code, statusCode.Code, statusCode.Overflow);
                }
            }
        }

        /// <summary>
        /// Log a Queued Value.
        /// </summary>
        [NonEvent]
        public void QueueValue(uint id, Variant wrappedValue, StatusCode statusCode)
        {
            if ((TraceMask & TraceMasks.OperationDetail) != 0)
            {
                if (IsEnabled())
                {
                    //WriteEvent();
                }
                else if (Logger.IsEnabled(LogLevel.Trace))
                {
                    LogTrace(TraceMasks.OperationDetail, "QUEUE VALUE[{0}]: Value={1} CODE={2}<{3:X8}> OVERFLOW={4}",
                        id, wrappedValue, statusCode.Code, statusCode.Code, statusCode.Overflow);
                }
            }
        }
    }
}
