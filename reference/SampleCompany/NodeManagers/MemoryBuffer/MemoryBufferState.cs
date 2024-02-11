#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace SampleCompany.NodeManagers.MemoryBuffer
{
    public partial class MemoryBufferState
    {
        #region Constructors
        /// <summary>
        /// Initializes the buffer from the configuration.
        /// </summary>
        public MemoryBufferState(ISystemContext context, MemoryBufferInstance configuration) : base(null)
        {
            Initialize(context);

            var dataType = "UInt32";
            var name = dataType;
            var count = 10;

            if (configuration != null)
            {
                count = configuration.TagCount;

                if (!String.IsNullOrEmpty(configuration.DataType))
                {
                    dataType = configuration.DataType;
                }

                if (!String.IsNullOrEmpty(configuration.Name))
                {
                    name = dataType;
                }
            }

            this.SymbolicName = name;

            BuiltInType elementType = BuiltInType.UInt32;

            switch (dataType)
            {
                case "Double":
                {
                    elementType = BuiltInType.Double;
                    break;
                }
            }

            CreateBuffer(elementType, count);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The server that the buffer belongs to.
        /// </summary>
        public IUaServerData Server
        {
            get { return server_; }
        }

        /// <summary>
        /// The node manager that the buffer belongs to.
        /// </summary>
        public IUaBaseNodeManager NodeManager
        {
            get { return nodeManager_; }
        }

        /// <summary>
        /// The built-in type for the values stored in the buffer.
        /// </summary>
        public BuiltInType ElementType
        {
            get { return elementType_; }
        }

        /// <summary>
        /// The size of each element in the buffer.
        /// </summary>
        public uint ElementSize
        {
            get { return (uint)elementSize_; }
        }

        /// <summary>
        /// The rate at which the buffer is scanned.
        /// </summary>
        public int MaximumScanRate
        {
            get { return maximumScanRate_; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the buffer with enough space to hold the specified number of elements.
        /// </summary>
        /// <param name="elementName">The type of element.</param>
        /// <param name="noOfElements">The number of elements.</param>
        public void CreateBuffer(string elementName, int noOfElements)
        {
            if (String.IsNullOrEmpty(elementName))
            {
                elementName = "UInt32";
            }

            BuiltInType elementType = BuiltInType.UInt32;

            switch (elementName)
            {
                case "Double":
                {
                    elementType = BuiltInType.Double;
                    break;
                }
            }

            CreateBuffer(elementType, noOfElements);
        }

        /// <summary>
        /// Initializes the buffer with enough space to hold the specified number of elements.
        /// </summary>
        /// <param name="elementType">The type of element.</param>
        /// <param name="noOfElements">The number of elements.</param>
        public void CreateBuffer(BuiltInType elementType, int noOfElements)
        {
            lock (dataLock_)
            {
                elementType_ = elementType;
                elementSize_ = 1;

                switch (elementType_)
                {
                    case BuiltInType.UInt32:
                    {
                        elementSize_ = 4;
                        break;
                    }

                    case BuiltInType.Double:
                    {
                        elementSize_ = 8;
                        break;
                    }
                }

                lastScanTime_ = DateTime.UtcNow;
                maximumScanRate_ = 1000;

                buffer_ = new byte[elementSize_ * noOfElements];
                SizeInBytes.Value = (uint)buffer_.Length;
            }
        }

        /// <summary>
        /// Creates an object which can browser the tags in the buffer.
        /// </summary>
        public override INodeBrowser CreateBrowser(
            ISystemContext context,
            ViewDescription view,
            NodeId referenceType,
            bool includeSubtypes,
            BrowseDirection browseDirection,
            QualifiedName browseName,
            IEnumerable<IReference> additionalReferences,
            bool internalOnly)
        {
            NodeBrowser browser = new MemoryBufferBrowser(
                context,
                view,
                referenceType,
                includeSubtypes,
                browseDirection,
                browseName,
                additionalReferences,
                internalOnly,
                this);

            PopulateBrowser(context, browser);

            return browser;
        }

        /// <summary>
        /// Handles the read operation for an invidual tag.
        /// </summary>
        public ServiceResult ReadTagValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var tag = node as MemoryTagState;

            if (tag == null)
            {
                return StatusCodes.BadNodeIdUnknown;
            }

            if (NumericRange.Empty != indexRange)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            if (!QualifiedName.IsNull(dataEncoding))
            {
                return StatusCodes.BadDataEncodingUnsupported;
            }

            var offset = (int)tag.Offset;

            lock (dataLock_)
            {
                if (offset < 0 || offset >= buffer_.Length)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                if (buffer_ == null)
                {
                    return StatusCodes.BadOutOfService;
                }

                value = GetValueAtOffset(offset).Value;
            }

            statusCode = StatusCodes.Good;
            timestamp = lastScanTime_;

            return ServiceResult.Good;
        }

        /// <summary>
        /// Handles a write operation for an individual tag.
        /// </summary>
        public ServiceResult WriteTagValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var tag = node as MemoryTagState;

            if (tag == null)
            {
                return StatusCodes.BadNodeIdUnknown;
            }

            if (NumericRange.Empty != indexRange)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            if (!QualifiedName.IsNull(dataEncoding))
            {
                return StatusCodes.BadDataEncodingUnsupported;
            }

            if (statusCode != StatusCodes.Good)
            {
                return StatusCodes.BadWriteNotSupported;
            }

            if (timestamp != DateTime.MinValue)
            {
                return StatusCodes.BadWriteNotSupported;
            }

            var changed = false;
            var offset = (int)tag.Offset;

            lock (dataLock_)
            {
                if (offset < 0 || offset >= buffer_.Length)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                if (buffer_ == null)
                {
                    return StatusCodes.BadOutOfService;
                }

                byte[] bytes = null;

                switch (elementType_)
                {
                    case BuiltInType.UInt32:
                    {
                        var valueToWrite = value as uint?;

                        if (valueToWrite == null)
                        {
                            return StatusCodes.BadTypeMismatch;
                        }

                        bytes = BitConverter.GetBytes(valueToWrite.Value);
                        break;
                    }

                    case BuiltInType.Double:
                    {
                        var valueToWrite = value as double?;

                        if (valueToWrite == null)
                        {
                            return StatusCodes.BadTypeMismatch;
                        }

                        bytes = BitConverter.GetBytes(valueToWrite.Value);
                        break;
                    }

                    default:
                    {
                        return StatusCodes.BadNodeIdUnknown;
                    }
                }

                for (var ii = 0; ii < bytes.Length; ii++)
                {
                    if (!changed)
                    {
                        if (buffer_[offset + ii] != bytes[ii])
                        {
                            changed = true;
                        }
                    }

                    buffer_[offset + ii] = bytes[ii];
                }
            }

            if (changed)
            {
                OnBufferChanged(offset);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Returns the value at the specified offset.
        /// </summary>
        public Variant GetValueAtOffset(int offset)
        {
            lock (dataLock_)
            {
                if (offset < 0 || offset >= buffer_.Length)
                {
                    return Variant.Null;
                }

                if (buffer_ == null)
                {
                    return Variant.Null;
                }

                switch (elementType_)
                {
                    case BuiltInType.UInt32:
                    {
                        return new Variant(BitConverter.ToUInt32(buffer_, offset));
                    }

                    case BuiltInType.Double:
                    {
                        return new Variant(BitConverter.ToDouble(buffer_, offset));
                    }
                }

                return Variant.Null;
            }
        }
        #endregion

        #region Monitoring Support Functions
        /// <summary>
        /// Initializes the instance with the context for the node being monitored.
        /// </summary>
        public void InitializeMonitoring(
            IUaServerData server,
            IUaBaseNodeManager nodeManager)
        {
            lock (dataLock_)
            {
                server_ = server;
                nodeManager_ = nodeManager;
                nonValueMonitoredItems_ = new Dictionary<uint, MemoryBufferMonitoredItem>();
            }
        }

        /// <summary>
        /// Creates a new data change monitored item.
        /// </summary>
        public MemoryBufferMonitoredItem CreateDataChangeItem(
            UaServerContext context,
            MemoryTagState tag,
            uint subscriptionId,
            uint monitoredItemId,
            ReadValueId itemToMonitor,
            DiagnosticsMasks diagnosticsMasks,
            TimestampsToReturn timestampsToReturn,
            MonitoringMode monitoringMode,
            uint clientHandle,
            double samplingInterval)

        /*
        ISystemContext context,
        MemoryTagState tag,
        uint monitoredItemId,
        uint attributeId,
        DiagnosticsMasks diagnosticsMasks,
        TimestampsToReturn timestampsToReturn,
        MonitoringMode monitoringMode,
        uint clientHandle,
        double samplingInterval)*/
        {
            lock (dataLock_)
            {
                var monitoredItem = new MemoryBufferMonitoredItem(
                    server_,
                    nodeManager_,
                    this,
                    tag.Offset,
                    0,
                    monitoredItemId,
                    itemToMonitor,
                    diagnosticsMasks,
                    timestampsToReturn,
                    monitoringMode,
                    clientHandle,
                    null,
                    null,
                    null,
                    samplingInterval,
                    0,
                    false,
                    0);

                /*
                MemoryBufferMonitoredItem monitoredItem = new MemoryBufferMonitoredItem(
                    this,
                    monitoredItemId,
                    tag.Offset,
                    attributeId,
                    diagnosticsMasks,
                    timestampsToReturn,
                    monitoringMode,
                    clientHandle,
                    samplingInterval);
                */

                if (itemToMonitor.AttributeId != Attributes.Value)
                {
                    nonValueMonitoredItems_.Add(monitoredItem.Id, monitoredItem);
                    return monitoredItem;
                }

                var elementCount = (int)(SizeInBytes.Value / ElementSize);

                if (monitoringTable_ == null)
                {
                    monitoringTable_ = new MemoryBufferMonitoredItem[elementCount][];
                    scanTimer_ = new Timer(DoScan, null, 100, 100);
                }

                var elementOffet = (int)(tag.Offset / ElementSize);

                MemoryBufferMonitoredItem[] monitoredItems = monitoringTable_[elementOffet];

                if (monitoredItems == null)
                {
                    monitoredItems = new MemoryBufferMonitoredItem[1];
                }
                else
                {
                    monitoredItems = new MemoryBufferMonitoredItem[monitoredItems.Length + 1];
                    monitoringTable_[elementOffet].CopyTo(monitoredItems, 0);
                }

                monitoredItems[monitoredItems.Length - 1] = monitoredItem;
                monitoringTable_[elementOffet] = monitoredItems;
                itemCount_++;

                return monitoredItem;
            }
        }

        /// <summary>
        /// Scans the buffer and updates every other element.
        /// </summary>
        void DoScan(object state)
        {
            DateTime start1 = DateTime.UtcNow;

            lock (dataLock_)
            {
                for (var ii = 0; ii < buffer_.Length; ii += elementSize_)
                {
                    buffer_[ii]++;

                    // notify any monitored items that the value has changed.
                    OnBufferChanged(ii);
                }

                lastScanTime_ = DateTime.UtcNow;
            }

            DateTime end1 = DateTime.UtcNow;

            var delta1 = ((double)(end1.Ticks - start1.Ticks)) / TimeSpan.TicksPerMillisecond;

            if (delta1 > 100)
            {
                Utils.LogWarning("{0} SAMPLING DELAY ({1}ms)", nameof(MemoryBufferState), delta1);
            }
        }

        /// <summary>
        /// Deletes the monitored item.
        /// </summary>
        public void DeleteItem(MemoryBufferMonitoredItem monitoredItem)
        {
            lock (dataLock_)
            {
                if (monitoredItem.AttributeId != Attributes.Value)
                {
                    nonValueMonitoredItems_.Remove(monitoredItem.Id);
                    return;
                }

                if (monitoringTable_ != null)
                {
                    var elementOffet = (int)(monitoredItem.Offset / ElementSize);

                    MemoryBufferMonitoredItem[] monitoredItems = monitoringTable_[elementOffet];

                    if (monitoredItems != null)
                    {
                        var index = -1;

                        for (var ii = 0; ii < monitoredItems.Length; ii++)
                        {
                            if (ReferenceEquals(monitoredItems[ii], monitoredItem))
                            {
                                index = ii;
                                break;
                            }
                        }

                        if (index >= 0)
                        {
                            itemCount_--;

                            if (monitoredItems.Length == 1)
                            {
                                monitoredItems = null;
                            }
                            else
                            {
                                monitoredItems = new MemoryBufferMonitoredItem[monitoredItems.Length - 1];

                                Array.Copy(monitoringTable_[elementOffet], 0, monitoredItems, 0, index);
                                Array.Copy(monitoringTable_[elementOffet], index + 1, monitoredItems, index, monitoredItems.Length - index);
                            }

                            monitoringTable_[elementOffet] = monitoredItems;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles change events raised by the node.
        /// </summary>
        public void OnBufferChanged(int offset)
        {
            lock (dataLock_)
            {
                if (monitoringTable_ != null)
                {
                    var elementOffet = (int)(offset / ElementSize);

                    MemoryBufferMonitoredItem[] monitoredItems = monitoringTable_[elementOffet];

                    if (monitoredItems != null)
                    {
                        var value = new DataValue {
                            WrappedValue = GetValueAtOffset(offset),
                            StatusCode = StatusCodes.Good,
                            ServerTimestamp = DateTime.UtcNow,
                            SourceTimestamp = lastScanTime_
                        };

                        for (var ii = 0; ii < monitoredItems.Length; ii++)
                        {
                            monitoredItems[ii].QueueValue(value, null);
                            updateCount_++;
                        }
                    }
                }
            }
        }

        void ScanTimer_Tick(object sender, EventArgs e)
        {
            DoScan(null);
        }

        void PublishTimer_Tick(object sender, EventArgs e)
        {
            DateTime start1 = DateTime.UtcNow;

            lock (dataLock_)
            {
                if (itemCount_ > 0 && updateCount_ < itemCount_)
                {
                    Utils.LogInfo("{0:HH:mm:ss.fff} MEMORYBUFFER Reported  {1}/{2} items ***.", DateTime.Now, updateCount_, itemCount_);
                }

                updateCount_ = 0;
            }

            DateTime end1 = DateTime.UtcNow;

            var delta1 = ((double)(end1.Ticks - start1.Ticks)) / TimeSpan.TicksPerMillisecond;

            if (delta1 > 100)
            {
                Utils.LogInfo("{0} ****** PUBLISH DELAY ({1}ms) ******", nameof(MemoryBufferState), delta1);
            }
        }
        #endregion

        #region Private Fields
        private readonly object dataLock_ = new object();
        private IUaServerData server_;
        private IUaBaseNodeManager nodeManager_;
        private MemoryBufferMonitoredItem[][] monitoringTable_;
        private Dictionary<uint, MemoryBufferMonitoredItem> nonValueMonitoredItems_;
        private BuiltInType elementType_;
        private int elementSize_;
        private DateTime lastScanTime_;
        private int maximumScanRate_;
        private byte[] buffer_;
        private Timer scanTimer_;
        private int updateCount_;
        private int itemCount_;
        #endregion
    }
}
