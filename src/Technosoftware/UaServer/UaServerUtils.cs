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
using System.Data;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// The interface that a server exposes to objects that it contains.
    /// </summary>
    public static partial class UaServerUtils
    {
        private enum EventType
        {
            WriteValue,
            CreateItem,
            ModifyItem,
            QueueValue,
            FilterValue,
            DiscardValue,
            PublishValue
        }

        private class Event
        {
            public DateTime Timestamp;
            public EventType EventType;
            public NodeId NodeId;
            public uint ServerHandle;
            public DataValue Value;
            public MonitoringParameters Parameters;
            public MonitoringMode MonitoringMode;
        }

        private static Queue<Event> events_ = new Queue<Event>();
        private static bool eventsEnabled_;

        /// <summary>
        /// Whether event queuing is enabled.
        /// </summary>
        public static bool EventsEnabled
        {
            get => eventsEnabled_;

            set
            {
                if (eventsEnabled_ != value)
                {
                    if (!value)
                    {
                        lock (events_)
                        {
                            events_.Clear();
                        }
                    }
                }

                eventsEnabled_ = value;
            }
        }

        /// <summary>
        /// Reports a value written.
        /// </summary>
        public static void ReportWriteValue(NodeId nodeId, DataValue value, StatusCode error)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.WriteValue,
                    NodeId = nodeId,
                    ServerHandle = 0,
                    Timestamp = HiResClock.UtcNow,
                    Value = value,
                    Parameters = null,
                    MonitoringMode = MonitoringMode.Disabled
                };

                if (StatusCode.IsBad(error))
                {
                    e.Value = new DataValue(error) {WrappedValue = value.WrappedValue};
                }

                events_.Enqueue(e);
            }
        }

        /// <summary>
        /// Reports a value queued.
        /// </summary>
        public static void ReportQueuedValue(NodeId nodeId, uint serverHandle, DataValue value)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.QueueValue,
                    NodeId = nodeId,
                    ServerHandle = serverHandle,
                    Timestamp = HiResClock.UtcNow,
                    Value = value,
                    Parameters = null,
                    MonitoringMode = MonitoringMode.Disabled
                };
                events_.Enqueue(e);
            }
        }

        /// <summary>
        /// Reports a value excluded by the filter.
        /// </summary>
        public static void ReportFilteredValue(NodeId nodeId, uint serverHandle, DataValue value)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.FilterValue,
                    NodeId = nodeId,
                    ServerHandle = serverHandle,
                    Timestamp = HiResClock.UtcNow,
                    Value = value,
                    Parameters = null,
                    MonitoringMode = MonitoringMode.Disabled
                };
                events_.Enqueue(e);
            }
        }

        /// <summary>
        /// Reports a value discarded because of queue overflow.
        /// </summary>
        public static void ReportDiscardedValue(NodeId nodeId, uint serverHandle, DataValue value)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.DiscardValue,
                    NodeId = nodeId,
                    ServerHandle = serverHandle,
                    Timestamp = HiResClock.UtcNow,
                    Value = value,
                    Parameters = null,
                    MonitoringMode = MonitoringMode.Disabled
                };
                events_.Enqueue(e);
            }
        }

        /// <summary>
        /// Reports a value published.
        /// </summary>
        public static void ReportPublishValue(NodeId nodeId, uint serverHandle, DataValue value)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.PublishValue,
                    NodeId = nodeId,
                    ServerHandle = serverHandle,
                    Timestamp = HiResClock.UtcNow,
                    Value = value,
                    Parameters = null,
                    MonitoringMode = MonitoringMode.Disabled
                };
                events_.Enqueue(e);
            }
        }

        /// <summary>
        /// Reports a new monitored item.
        /// </summary>
        public static void ReportCreateMonitoredItem(
            NodeId nodeId,
            uint serverHandle,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            MonitoringFilter filter,
            MonitoringMode monitoringMode)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.CreateItem,
                    NodeId = nodeId,
                    ServerHandle = serverHandle,
                    Timestamp = HiResClock.UtcNow,
                    Value = null,
                    Parameters = new MonitoringParameters
                    {
                        SamplingInterval = samplingInterval,
                        QueueSize = queueSize,
                        DiscardOldest = discardOldest,
                        Filter = new ExtensionObject(filter)
                    },
                    MonitoringMode = monitoringMode
                };
                events_.Enqueue(e);
            }
        }

        /// <summary>
        /// Reports a modified monitored item.
        /// </summary>
        public static void ReportModifyMonitoredItem(
            NodeId nodeId,
            uint serverHandle,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            MonitoringFilter filter,
            MonitoringMode monitoringMode)
        {
            if (!eventsEnabled_)
            {
                return;
            }

            lock (events_)
            {
                var e = new Event
                {
                    EventType = EventType.ModifyItem,
                    NodeId = nodeId,
                    ServerHandle = serverHandle,
                    Timestamp = HiResClock.UtcNow,
                    Value = null,
                    Parameters = new MonitoringParameters
                    {
                        SamplingInterval = samplingInterval,
                        QueueSize = queueSize,
                        DiscardOldest = discardOldest,
                        Filter = new ExtensionObject(filter)
                    },
                    MonitoringMode = monitoringMode
                };
                events_.Enqueue(e);
            }
        }

        #region Error and Diagnostics
        /// <summary>
        /// Fills in the diagnostic information after an error.
        /// </summary>
        public static uint CreateError(
            uint code,
            UaServerOperationContext context,
            DiagnosticInfoCollection diagnosticInfos,
            int index)
        {
            var error = new ServiceResult(code);

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos[index] = new DiagnosticInfo(error, context.DiagnosticsMask, false, context.StringTable);
            }

            return error.Code;
        }

        /// <summary>
        /// Fills in the diagnostic information after an error.
        /// </summary>
        public static bool CreateError(
            uint code,
            StatusCodeCollection results,
            DiagnosticInfoCollection diagnosticInfos,
            UaServerOperationContext context)
        {
            var error = new ServiceResult(code);
            results.Add(error.Code);

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos.Add(new DiagnosticInfo(error, context.DiagnosticsMask, false, context.StringTable));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fills in the diagnostic information after an error.
        /// </summary>
        public static bool CreateError(
            uint code,
            StatusCodeCollection results,
            DiagnosticInfoCollection diagnosticInfos,
            int index,
            UaServerOperationContext context)
        {
            var error = new ServiceResult(code);
            results[index] = error.Code;

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos[index] = new DiagnosticInfo(error, context.DiagnosticsMask, false, context.StringTable);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a place holder in the lists for the results.
        /// </summary>
        public static void CreateSuccess(
            StatusCodeCollection results,
            DiagnosticInfoCollection diagnosticInfos,
            UaServerOperationContext context)
        {
            results.Add(StatusCodes.Good);

            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
            {
                diagnosticInfos.Add(null);
            }
        }

        /// <summary>
        /// Creates a collection of diagnostics from a set of errors.
        /// </summary>
        public static DiagnosticInfoCollection CreateDiagnosticInfoCollection(
            UaServerOperationContext context,
            IList<ServiceResult> errors)
        {
            // all done if no diagnostics requested.
            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) == 0)
            {
                return null;
            }

            // create diagnostics.
            var results = new DiagnosticInfoCollection(errors.Count);

            foreach (var error in errors)
            {
                if (ServiceResult.IsBad(error))
                {
                    results.Add(new DiagnosticInfo(error, context.DiagnosticsMask, false, context.StringTable));
                }
                else
                {
                    results.Add(null);
                }
            }

            return results;
        }

        /// <summary>
        /// Creates a collection of status codes and diagnostics from a set of errors.
        /// </summary>
        public static StatusCodeCollection CreateStatusCodeCollection(
            UaServerOperationContext context,
            IList<ServiceResult> errors,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            diagnosticInfos = null;

            var noErrors = true;
            var results = new StatusCodeCollection(errors.Count);

            foreach (var error in errors)
            {
                if (ServiceResult.IsBad(error))
                {
                    results.Add(error.Code);
                    noErrors = false;
                }
                else
                {
                    results.Add(StatusCodes.Good);
                }
            }

            // only generate diagnostics if errors exist.
            if (noErrors)
            {
                diagnosticInfos = CreateDiagnosticInfoCollection(context, errors);
            }

            return results;
        }

        /// <summary>
        /// Creates the diagnostic info and translates any strings.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The context containing the string stable.</param>
        /// <param name="error">The error to translate.</param>
        /// <returns>The diagnostics with references to the strings in the context string table.</returns>
        public static DiagnosticInfo CreateDiagnosticInfo(
            IUaServerData server,
            UaServerOperationContext context,
            ServiceResult error)
        {
            if (error == null)
            {
                return null;
            }

            var translatedError = error;

            if ((context.DiagnosticsMask & DiagnosticsMasks.LocalizedText) != 0)
            {
                translatedError = server.ResourceManager.Translate(context.PreferredLocales, error);
            }

            var diagnosticInfo = new DiagnosticInfo(
                translatedError,
                context.DiagnosticsMask,
                false,
                context.StringTable);

            return diagnosticInfo;
        }
        #endregion
    }
}
