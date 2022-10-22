#region Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace MemoryBuffer
{
    /// <summary>
    /// Provides a basic monitored item implementation which does not support queuing.
    /// </summary>
    public class MemoryBufferMonitoredItem : UaMonitoredItem
    {
        /// <summary>
        /// Initializes the object with its node type.
        /// </summary>
        public MemoryBufferMonitoredItem(
            IUaServerData server,
            IUaBaseNodeManager nodeManager,
            object mangerHandle,
            uint offset,
            uint subscriptionId,
            uint id,
            ReadValueId itemToMonitor,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            MonitoringFilter originalFilter,
            MonitoringFilter filterToUse,
            Opc.Ua.Range range,
            double samplingInterval,
            uint queueSize,
            bool discardOldest,
            double minimumSamplingInterval)
        :
            base(
                server,
                nodeManager,
                mangerHandle,
                subscriptionId,
                id,
                itemToMonitor,
                diagnosticsMasks,
                timestampsToReturn,
                monitoringMode,
                clientHandle,
                originalFilter,
                filterToUse,
                range,
                samplingInterval,
                queueSize,
                discardOldest,
                minimumSamplingInterval)
        {
            offset_ = offset;
        }

        /// <summary>
        /// Modifies the monitored item parameters,
        /// </summary>
        public ServiceResult Modify(
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            uint clientHandle,
            double samplingInterval)
        {
            return base.ModifyAttributes(diagnosticsMasks,
                timestampsToReturn,
                clientHandle,
                null,
                null,
                null,
                samplingInterval,
                0,
                false);
        }

        /// <summary>
        /// The offset in the memory buffer.
        /// </summary>
        public uint Offset
        {
            get
            {
                return offset_;
            }
        }

        private uint offset_;
    }
}
