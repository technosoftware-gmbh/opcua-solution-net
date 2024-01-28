#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Data;

using Opc.Ua;

using SampleCompany.SampleServer.UnderlyingSystem;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaBaseServer;
#endregion

namespace SampleCompany.SampleServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class SampleServerNodeManager : UaBaseNodeManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public SampleServerNodeManager(IUaServer opcServer,
            IUaServerPlugin opcServerPlugin,
            IUaServerData uaServer,
            ApplicationConfiguration configuration,
            params string[] namespaceUris)
            : base(uaServer, configuration, namespaceUris)
        {
            opcServer_ = opcServer;
            opcServerPlugin_ = opcServerPlugin;

            this.AliasRoot = "HDA";

            // get the configuration for the node manager.
            configuration_ = configuration.ParseExtension<HistoricalAccessServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (configuration_ == null)
            {
                configuration_ = new HistoricalAccessServerConfiguration();
                configuration_.ArchiveRoot = ".\\Archive";
            }

            SystemContext.SystemHandle = system_ = new UnderlyingSystem.UnderlyingSystem(configuration_, NamespaceIndex);
            SystemContext.NodeIdFactory = this;
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructor in types derived from this class.
        /// </summary>
        ~SampleServerNodeManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed_)
            {
                lock (lockDisposable_)
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    // If disposing is false,
                    // only the following code is executed.

                    // Disposing has been done.
                    disposed_ = true;
                }

            }
        }
        #endregion

        #region INodeIdFactory Members
        #endregion

        #region IUaNodeManager Methods
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (ServerData.DiagnosticsLock)
            {
                HistoryServerCapabilitiesState capabilities = ServerData.DiagnosticsNodeManager.GetDefaultHistoryCapabilities();
                capabilities.AccessHistoryDataCapability.Value = true;
                capabilities.InsertDataCapability.Value = true;
                capabilities.ReplaceDataCapability.Value = true;
                capabilities.UpdateDataCapability.Value = true;
                capabilities.DeleteRawCapability.Value = true;
                capabilities.DeleteAtTimeCapability.Value = true;
                capabilities.InsertAnnotationCapability.Value = true;
            }

            lock (Lock)
            {
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = References = new List<IReference>();
                }
                else
                {
                    References = references;
                }

                LoadPredefinedNodes(SystemContext, externalReferences);

                ArchiveFolderState root = system_.GetFolderState(SystemContext, String.Empty);
                References.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, root.NodeId));
                root.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

                try
                {

                    CreateFolderFromResources(root, "Sample");
                    CreateFolderFromResources(root, "Dynamic");

                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Error creating the address space.");
                }
                // Add all nodes under root to the server
                AddPredefinedNode(SystemContext, root);
            }
        }

        /// <summary>
        /// Creates items from embedded resources.
        /// </summary>
        private void CreateFolderFromResources(NodeState root, string folderName)
        {
            FolderState dataFolder = new FolderState(root);
            dataFolder.ReferenceTypeId = ReferenceTypeIds.Organizes;
            dataFolder.TypeDefinitionId = ObjectTypeIds.FolderType;
            dataFolder.NodeId = new NodeId(folderName, NamespaceIndex);
            dataFolder.BrowseName = new QualifiedName(folderName, NamespaceIndex);
            dataFolder.DisplayName = dataFolder.BrowseName.Name;
            dataFolder.WriteMask = AttributeWriteMask.None;
            dataFolder.UserWriteMask = AttributeWriteMask.None;
            dataFolder.EventNotifier = EventNotifiers.None;
            root.AddChild(dataFolder);
            AddPredefinedNode(SystemContext, root);

            foreach (string resourcePath in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (!resourcePath.StartsWith("SampleCompany.SampleServer.Data." + folderName))
                {
                    continue;
                }

                ArchiveItem item = new ArchiveItem(resourcePath, Assembly.GetExecutingAssembly(), resourcePath);
                ArchiveItemState node = new ArchiveItemState(SystemContext, item, NamespaceIndex);
                node.ReloadFromSource(SystemContext);

                dataFolder.AddReference(ReferenceTypeIds.Organizes, false, node.NodeId);
                node.AddReference(ReferenceTypeIds.Organizes, true, dataFolder.NodeId);

                AddPredefinedNode(SystemContext, node);
            }
        }
        #endregion

        #region Overridden Methods
        #endregion

        #region HistoricalAccess Server related methods
        /// <summary>
        /// Reads the raw data for an item.
        /// </summary>
        protected override void HistoryReadRawModified(
            UaServerContext context,
            ReadRawModifiedDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                UaNodeHandle handle = nodesToProcess[ii];
                HistoryReadValueId nodeToRead = nodesToRead[handle.Index];
                HistoryReadResult result = results[handle.Index];

                HistoryReadRequest request = null;

                try
                {
                    // validate node.
                    NodeState source = ValidateNode(context, handle, cache);

                    if (source == null)
                    {
                        continue;
                    }

                    // load an exising request.
                    if (nodeToRead.ContinuationPoint != null)
                    {
                        request = LoadContinuationPoint(context, nodeToRead.ContinuationPoint);

                        if (request == null)
                        {
                            errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
                            continue;
                        }
                    }

                    // create a new request.
                    else
                    {
                        request = CreateHistoryReadRequest(
                            context,
                            details,
                            handle,
                            nodeToRead);
                    }

                    // process values until the max is reached.
                    HistoryData data = (details.IsReadModified) ? new HistoryModifiedData() : new HistoryData();
                    HistoryModifiedData modifiedData = data as HistoryModifiedData;

                    while (request.NumValuesPerNode == 0 || data.DataValues.Count < request.NumValuesPerNode)
                    {
                        if (request.Values.Count == 0)
                        {
                            break;
                        }

                        DataValue value = request.Values.First.Value;
                        request.Values.RemoveFirst();
                        data.DataValues.Add(value);

                        if (modifiedData != null)
                        {
                            ModificationInfo modificationInfo = null;

                            if (request.ModificationInfos != null && request.ModificationInfos.Count > 0)
                            {
                                modificationInfo = request.ModificationInfos.First.Value;
                                request.ModificationInfos.RemoveFirst();
                            }

                            modifiedData.ModificationInfos.Add(modificationInfo);
                        }
                    }

                    errors[handle.Index] = ServiceResult.Good;

                    // check if a continuation point is requred.
                    if (request.Values.Count > 0)
                    {
                        // only set if both end time and start time are specified.
                        if (details.StartTime != DateTime.MinValue && details.EndTime != DateTime.MinValue)
                        {
                            result.ContinuationPoint = SaveContinuationPoint(context, request);
                        }
                    }

                    // check if no data returned.
                    else
                    {
                        errors[handle.Index] = StatusCodes.GoodNoData;
                    }

                    // return the data.
                    result.HistoryData = new ExtensionObject(data);
                }
                catch (Exception e)
                {
                    errors[handle.Index] = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, "Unexpected error processing request.");
                }
            }
        }

        /// <summary>
        /// Reads the processed data for an item.
        /// </summary>
        protected override void HistoryReadProcessed(
            UaServerContext context,
            ReadProcessedDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the data at the specified time for an item.
        /// </summary>
        protected override void HistoryReadAtTime(
            UaServerContext context,
            ReadAtTimeDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the data history for one or more nodes.
        /// </summary>
        protected override void HistoryUpdateData(
            UaServerContext context,
            IList<UpdateDataDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the data history for one or more nodes.
        /// </summary>
        protected override void HistoryUpdateStructureData(
            UaServerContext context,
            IList<UpdateStructureDataDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the data history for one or more nodes.
        /// </summary>
        protected override void HistoryDeleteRawModified(
            UaServerContext context,
            IList<DeleteRawModifiedDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the data history for one or more nodes.
        /// </summary>
        protected override void HistoryDeleteAtTime(
            UaServerContext context,
            IList<DeleteAtTimeDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<UaNodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region HistoricalAccess Helper Methods
        /// <summary>
        /// Loads the archive item state from the underlying source.
        /// </summary>
        private ArchiveItemState Reload(ISystemContext context, UaNodeHandle handle)
        {
            ArchiveItemState item = handle.Node as ArchiveItemState;

            if (item == null)
            {
                BaseInstanceState property = handle.Node as BaseInstanceState;

                if (property != null)
                {
                    item = property.Parent as ArchiveItemState;
                }
            }

            if (item != null)
            {
                item.ReloadFromSource(context);
            }

            return item;
        }

        /// <summary>
        /// Creates a new history request.
        /// </summary>
        private HistoryReadRequest CreateHistoryReadRequest(
            ISystemContext context,
            ReadRawModifiedDetails details,
            UaNodeHandle handle,
            HistoryReadValueId nodeToRead)
        {
            bool sizeLimited = (details.StartTime == DateTime.MinValue || details.EndTime == DateTime.MinValue);
            bool applyIndexRangeOrEncoding = (nodeToRead.ParsedIndexRange != NumericRange.Empty || !QualifiedName.IsNull(nodeToRead.DataEncoding));
            bool returnBounds = !details.IsReadModified && details.ReturnBounds;
            bool timeFlowsBackward = (details.StartTime == DateTime.MinValue) || (details.EndTime != DateTime.MinValue && details.EndTime < details.StartTime);

            // find the archive item.
            ArchiveItemState item = Reload(context, handle);

            if (item == null)
            {
                throw new ServiceResultException(StatusCodes.BadNotSupported);
            }

            LinkedList<DataValue> values = new LinkedList<DataValue>();
            LinkedList<ModificationInfo> modificationInfos = null;

            if (details.IsReadModified)
            {
                modificationInfos = new LinkedList<ModificationInfo>();
            }

            // read history. 
            DataView view = item.ReadHistory(details.StartTime, details.EndTime, details.IsReadModified, handle.Node.BrowseName);

            int startBound = -1;
            int endBound = -1;
            int ii = (timeFlowsBackward) ? view.Count - 1 : 0;

            while (ii >= 0 && ii < view.Count)
            {
                try
                {
                    DateTime timestamp = (DateTime)view[ii].Row[0];

                    // check if looking for start of data.
                    if (values.Count == 0)
                    {
                        if (timeFlowsBackward)
                        {
                            if ((details.StartTime != DateTime.MinValue && timestamp >= details.StartTime) || (details.StartTime == DateTime.MinValue && timestamp >= details.EndTime))
                            {
                                startBound = ii;

                                if (timestamp > details.StartTime)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (timestamp <= details.StartTime)
                            {
                                startBound = ii;

                                if (timestamp < details.StartTime)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    // check if absolute max values specified.
                    if (sizeLimited)
                    {
                        if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < values.Count)
                        {
                            break;
                        }
                    }

                    // check for end bound.
                    if (details.EndTime != DateTime.MinValue && timestamp >= details.EndTime)
                    {
                        if (timeFlowsBackward)
                        {
                            if (timestamp <= details.EndTime)
                            {
                                endBound = ii;
                                break;
                            }
                        }
                        else
                        {
                            if (timestamp >= details.EndTime)
                            {
                                endBound = ii;
                                break;
                            }
                        }
                    }

                    // check if the start bound needs to be returned.
                    if (returnBounds && values.Count == 0 && startBound != ii && details.StartTime != DateTime.MinValue)
                    {
                        // add start bound.
                        if (startBound == -1)
                        {
                            values.AddLast(new DataValue(Variant.Null, StatusCodes.BadBoundNotFound, details.StartTime, details.StartTime));
                        }
                        else
                        {
                            values.AddLast(RowToDataValue(context, nodeToRead, view[startBound], applyIndexRangeOrEncoding));
                        }

                        // check if absolute max values specified.
                        if (sizeLimited)
                        {
                            if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < values.Count)
                            {
                                break;
                            }
                        }
                    }

                    // add value.
                    values.AddLast(RowToDataValue(context, nodeToRead, view[ii], applyIndexRangeOrEncoding));

                    if (modificationInfos != null)
                    {
                        modificationInfos.AddLast((ModificationInfo)view[ii].Row[6]);
                    }
                }
                finally
                {
                    if (timeFlowsBackward)
                    {
                        ii--;
                    }
                    else
                    {
                        ii++;
                    }
                }
            }

            // add late bound.
            while (returnBounds && details.EndTime != DateTime.MinValue)
            {
                // add start bound.
                if (values.Count == 0)
                {
                    if (startBound == -1)
                    {
                        values.AddLast(new DataValue(Variant.Null, StatusCodes.BadBoundNotFound, details.StartTime, details.StartTime));
                    }
                    else
                    {
                        values.AddLast(RowToDataValue(context, nodeToRead, view[startBound], applyIndexRangeOrEncoding));
                    }
                }

                // check if absolute max values specified.
                if (sizeLimited)
                {
                    if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < values.Count)
                    {
                        break;
                    }
                }

                // add end bound.
                if (endBound == -1)
                {
                    values.AddLast(new DataValue(Variant.Null, StatusCodes.BadBoundNotFound, details.EndTime, details.EndTime));
                }
                else
                {
                    values.AddLast(RowToDataValue(context, nodeToRead, view[endBound], applyIndexRangeOrEncoding));
                }

                break;
            }

            HistoryReadRequest request = new HistoryReadRequest();
            request.Values = values;
            request.ModificationInfos = modificationInfos;
            request.NumValuesPerNode = details.NumValuesPerNode;
            request.Filter = null;
            return request;
        }

        /// <summary>
        /// Creates a new history request.
        /// </summary>
        private DataValue RowToDataValue(
            ISystemContext context,
            HistoryReadValueId nodeToRead,
            DataRowView row,
            bool applyIndexRangeOrEncoding)
        {
            DataValue value = (DataValue)row[2];

            // apply any index range or encoding.
            if (applyIndexRangeOrEncoding)
            {
                object rawValue = value.Value;
                ServiceResult result = BaseVariableState.ApplyIndexRangeAndDataEncoding(context, nodeToRead.ParsedIndexRange, nodeToRead.DataEncoding, ref rawValue);

                if (ServiceResult.IsBad(result))
                {
                    value.Value = rawValue;
                }
                else
                {
                    value.Value = null;
                    value.StatusCode = result.StatusCode;
                }
            }

            return value;
        }

        /// <summary>
        /// Stores a read history request.
        /// </summary>
        private class HistoryReadRequest
        {
            public byte[] ContinuationPoint;
            public LinkedList<DataValue> Values;
            public LinkedList<ModificationInfo> ModificationInfos;
            public uint NumValuesPerNode;
            public AggregateFilter Filter;
        }

        /// <summary>
        /// Loads a history continuation point.
        /// </summary>
        private HistoryReadRequest LoadContinuationPoint(
            UaServerContext context,
            byte[] continuationPoint)
        {
            Session session = context.OperationContext.Session;

            if (session == null)
            {
                return null;
            }

            HistoryReadRequest request = session.RestoreHistoryContinuationPoint(continuationPoint) as HistoryReadRequest;

            if (request == null)
            {
                return null;
            }

            return request;
        }

        /// <summary>
        /// Saves a history continuation point.
        /// </summary>
        private byte[] SaveContinuationPoint(
            UaServerContext context,
            HistoryReadRequest request)
        {
            Session session = context.OperationContext.Session;

            if (session == null)
            {
                return null;
            }

            Guid id = Guid.NewGuid();
            session.SaveHistoryContinuationPoint(id, request);
            request.ContinuationPoint = id.ToByteArray();
            return request.ContinuationPoint;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Runs the simulation.
        /// </summary>
        private void DoSimulation(object state)
        {
            try
            {
                lock (Lock)
                {
                }
            }
            catch (Exception e)
            {
                Utils.Trace("Unexpected error during simulation: {0}", e.Message);
            }
        }
        #endregion

        #region Private Fields
        private readonly IUaServer opcServer_;
        private readonly IUaServerPlugin opcServerPlugin_;

        // Track whether Dispose has been called.
        private bool disposed_;
        private readonly object lockDisposable_ = new object();
        
        private UnderlyingSystem.UnderlyingSystem system_;
        private HistoricalAccessServerConfiguration configuration_;
        #endregion

    }
}