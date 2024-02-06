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
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// The client side interface with support for batching according to operation limits.
    /// </summary>
    public class SessionClientBatched : SessionClient
    {
        #region Constructors
        /// <summary>
        /// Intializes the object with a channel and default operation limits.
        /// </summary>
        public SessionClientBatched(ITransportChannel channel)
        :
            base(channel)
        {
            operationLimits_ = new OperationLimits();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The operation limits are used to batch the service requests.
        /// </summary>
        public OperationLimits OperationLimits
        {
            get => operationLimits_;
            protected internal set
            {
                if (value == null)
                {
                    operationLimits_ = new OperationLimits();
                }
                else
                {
                    operationLimits_ = value;
                };
            }
        }
        #endregion

        #region AddNodes Methods
        /// <inheritdoc/>
        public override ResponseHeader AddNodes(
            RequestHeader requestHeader,
            AddNodesItemCollection nodesToAdd,
            out AddNodesResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<AddNodesResult, AddNodesResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                nodesToAdd.Count, operationLimit
                );

            foreach (var batchNodesToAdd in
                nodesToAdd.Batch<AddNodesItem, AddNodesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.AddNodes(requestHeader,
                    batchNodesToAdd,
                    out AddNodesResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchNodesToAdd);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToAdd);

                AddResponses<AddNodesResult, AddNodesResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<AddNodesResponse> AddNodesAsync(
            RequestHeader requestHeader,
            AddNodesItemCollection nodesToAdd,
            CancellationToken ct)
        {
            AddNodesResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<AddNodesResult, AddNodesResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                nodesToAdd.Count, operationLimit
                );

            foreach (var batchNodesToAdd in
                nodesToAdd.Batch<AddNodesItem, AddNodesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.AddNodesAsync(requestHeader, batchNodesToAdd, ct).ConfigureAwait(false);

                AddNodesResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchNodesToAdd);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToAdd);

                AddResponses<AddNodesResult, AddNodesResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region AddReferences Methods
        /// <inheritdoc/>
        public override ResponseHeader AddReferences(
            RequestHeader requestHeader,
            AddReferencesItemCollection referencesToAdd,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out results, out diagnosticInfos, out var stringTable,
                referencesToAdd.Count, operationLimit
                );

            foreach (var batchReferencesToAdd in
                referencesToAdd.Batch<AddReferencesItem, AddReferencesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.AddReferences(requestHeader,
                    batchReferencesToAdd,
                    out StatusCodeCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchReferencesToAdd);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchReferencesToAdd);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<AddReferencesResponse> AddReferencesAsync(
            RequestHeader requestHeader,
            AddReferencesItemCollection referencesToAdd,
            CancellationToken ct)
        {
            AddReferencesResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                referencesToAdd.Count, operationLimit
                );

            foreach (var batchReferencesToAdd in
                referencesToAdd.Batch<AddReferencesItem, AddReferencesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.AddReferencesAsync(requestHeader, batchReferencesToAdd, ct).ConfigureAwait(false);

                StatusCodeCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchReferencesToAdd);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchReferencesToAdd);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region DeleteNodes Methods
        /// <inheritdoc/>
        public override ResponseHeader DeleteNodes(
            RequestHeader requestHeader,
            DeleteNodesItemCollection nodesToDelete,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out results, out diagnosticInfos, out var stringTable,
                nodesToDelete.Count, operationLimit
                );

            foreach (var batchNodesToDelete in
                nodesToDelete.Batch<DeleteNodesItem, DeleteNodesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.DeleteNodes(requestHeader,
                    batchNodesToDelete,
                    out StatusCodeCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchNodesToDelete);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToDelete);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<DeleteNodesResponse> DeleteNodesAsync(
            RequestHeader requestHeader,
            DeleteNodesItemCollection nodesToDelete,
            CancellationToken ct)
        {
            DeleteNodesResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                nodesToDelete.Count, operationLimit
                );

            foreach (var batchNodesToDelete in
                nodesToDelete.Batch<DeleteNodesItem, DeleteNodesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.DeleteNodesAsync(requestHeader,
                    batchNodesToDelete, ct).ConfigureAwait(false);

                StatusCodeCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchNodesToDelete);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToDelete);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region DeleteReferences Methods
        /// <inheritdoc/>
        public override ResponseHeader DeleteReferences(
            RequestHeader requestHeader,
            DeleteReferencesItemCollection referencesToDelete,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out results, out diagnosticInfos, out var stringTable,
                referencesToDelete.Count, operationLimit
                );

            foreach (var batchReferencesToDelete in
                referencesToDelete.Batch<DeleteReferencesItem, DeleteReferencesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.DeleteReferences(requestHeader,
                    batchReferencesToDelete,
                    out StatusCodeCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchReferencesToDelete);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchReferencesToDelete);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<DeleteReferencesResponse> DeleteReferencesAsync(
            RequestHeader requestHeader,
            DeleteReferencesItemCollection referencesToDelete,
            CancellationToken ct)
        {
            DeleteReferencesResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerNodeManagement;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                referencesToDelete.Count, operationLimit
                );

            foreach (var batchReferencesToDelete in
                referencesToDelete.Batch<DeleteReferencesItem, DeleteReferencesItemCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.DeleteReferencesAsync(requestHeader,
                    batchReferencesToDelete, ct).ConfigureAwait(false);

                StatusCodeCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchReferencesToDelete);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchReferencesToDelete);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region Browse Methods
        /// <inheritdoc/>
        public override ResponseHeader Browse(
            RequestHeader requestHeader,
            ViewDescription view,
            uint requestedMaxReferencesPerNode,
            BrowseDescriptionCollection nodesToBrowse,
            out BrowseResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerBrowse;
            InitResponseCollections<BrowseResult, BrowseResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                nodesToBrowse.Count, operationLimit
                );

            foreach (var nodesToBrowseBatch in
                nodesToBrowse.Batch<BrowseDescription, BrowseDescriptionCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.Browse(requestHeader,
                    view,
                    requestedMaxReferencesPerNode,
                    nodesToBrowseBatch,
                    out BrowseResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, nodesToBrowseBatch);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, nodesToBrowseBatch);

                AddResponses<BrowseResult, BrowseResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<BrowseResponse> BrowseAsync(
            RequestHeader requestHeader,
            ViewDescription view,
            uint requestedMaxReferencesPerNode,
            BrowseDescriptionCollection nodesToBrowse,
            CancellationToken ct)
        {
            BrowseResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerBrowse;
            InitResponseCollections<BrowseResult, BrowseResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                nodesToBrowse.Count, operationLimit
                );

            foreach (var nodesToBrowseBatch in
                nodesToBrowse.Batch<BrowseDescription, BrowseDescriptionCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.BrowseAsync(
                    requestHeader,
                    view,
                    requestedMaxReferencesPerNode,
                    nodesToBrowseBatch, ct).ConfigureAwait(false);

                BrowseResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, nodesToBrowseBatch);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, nodesToBrowseBatch);

                AddResponses<BrowseResult, BrowseResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region TranslateBrowsePathsToNodeIds Methods
        /// <inheritdoc/>
        public override ResponseHeader TranslateBrowsePathsToNodeIds(
            RequestHeader requestHeader,
            BrowsePathCollection browsePaths,
            out BrowsePathResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds;
            InitResponseCollections<BrowsePathResult, BrowsePathResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                browsePaths.Count, operationLimit
                );

            foreach (var batchBrowsePaths in
                browsePaths.Batch<BrowsePath, BrowsePathCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.TranslateBrowsePathsToNodeIds(requestHeader,
                    batchBrowsePaths,
                    out BrowsePathResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchBrowsePaths);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchBrowsePaths);

                AddResponses<BrowsePathResult, BrowsePathResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<TranslateBrowsePathsToNodeIdsResponse> TranslateBrowsePathsToNodeIdsAsync(
            RequestHeader requestHeader,
            BrowsePathCollection browsePaths,
            CancellationToken ct)
        {
            TranslateBrowsePathsToNodeIdsResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds;
            InitResponseCollections<BrowsePathResult, BrowsePathResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                browsePaths.Count, operationLimit
                );

            foreach (var batchBrowsePaths in
                browsePaths.Batch<BrowsePath, BrowsePathCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.TranslateBrowsePathsToNodeIdsAsync(
                    requestHeader,
                    batchBrowsePaths,
                    ct).ConfigureAwait(false);

                BrowsePathResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchBrowsePaths);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchBrowsePaths);

                AddResponses<BrowsePathResult, BrowsePathResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region RegisterNodes Methods
        /// <inheritdoc/>
        public override ResponseHeader RegisterNodes(
            RequestHeader requestHeader,
            NodeIdCollection nodesToRegister,
            out NodeIdCollection registeredNodeIds)
        {
            ResponseHeader responseHeader = null;
            registeredNodeIds = new NodeIdCollection();

            foreach (var batchNodesToRegister in
                nodesToRegister.Batch<NodeId, NodeIdCollection>(OperationLimits.MaxNodesPerRegisterNodes))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.RegisterNodes(
                    requestHeader,
                    batchNodesToRegister,
                    out NodeIdCollection batchRegisteredNodeIds);

                ClientBase.ValidateResponse(batchRegisteredNodeIds, batchNodesToRegister);

                registeredNodeIds.AddRange(batchRegisteredNodeIds);
            }

            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<RegisterNodesResponse> RegisterNodesAsync(
            RequestHeader requestHeader,
            NodeIdCollection nodesToRegister,
            CancellationToken ct)
        {
            RegisterNodesResponse response = null;
            var registeredNodeIds = new NodeIdCollection();

            foreach (var batchNodesToRegister in
                nodesToRegister.Batch<NodeId, NodeIdCollection>(OperationLimits.MaxNodesPerRegisterNodes))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.RegisterNodesAsync(
                    requestHeader,
                    batchNodesToRegister, ct).ConfigureAwait(false);

                NodeIdCollection batchRegisteredNodeIds = response.RegisteredNodeIds;

                ClientBase.ValidateResponse(batchRegisteredNodeIds, batchNodesToRegister);

                registeredNodeIds.AddRange(batchRegisteredNodeIds);
            }

            response.RegisteredNodeIds = registeredNodeIds;

            return response;
        }
        #endregion

        #region UnregisterNodes Methods
        /// <inheritdoc/>
        public override ResponseHeader UnregisterNodes(
            RequestHeader requestHeader,
            NodeIdCollection nodesToUnregister)
        {
            ResponseHeader responseHeader = null;

            foreach (var batchNodesToUnregister in
                nodesToUnregister.Batch<NodeId, NodeIdCollection>(OperationLimits.MaxNodesPerRegisterNodes))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.UnregisterNodes(requestHeader, batchNodesToUnregister);
            }

            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<UnregisterNodesResponse> UnregisterNodesAsync(
            RequestHeader requestHeader,
            NodeIdCollection nodesToUnregister,
            CancellationToken ct)
        {
            UnregisterNodesResponse response = null;

            foreach (var batchNodesToUnregister in
                nodesToUnregister.Batch<NodeId, NodeIdCollection>(OperationLimits.MaxNodesPerRegisterNodes))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.UnregisterNodesAsync(requestHeader, batchNodesToUnregister, ct).ConfigureAwait(false);
            }

            return response;
        }
        #endregion

        #region Read Methods
        /// <inheritdoc/>
        public override ResponseHeader Read(
            RequestHeader requestHeader,
            double maxAge,
            TimestampsToReturn timestampsToReturn,
            ReadValueIdCollection nodesToRead,
            out DataValueCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerRead;
            InitResponseCollections<DataValue, DataValueCollection>(
                out results, out diagnosticInfos, out var stringTable,
                nodesToRead.Count, operationLimit
                );

            foreach (var batchAttributesToRead in
                            nodesToRead.Batch<ReadValueId, ReadValueIdCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.Read(
                    requestHeader,
                    maxAge,
                    timestampsToReturn,
                    batchAttributesToRead,
                    out DataValueCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchAttributesToRead);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchAttributesToRead);

                AddResponses<DataValue, DataValueCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<ReadResponse> ReadAsync(
            RequestHeader requestHeader,
            double maxAge,
            TimestampsToReturn timestampsToReturn,
            ReadValueIdCollection nodesToRead,
            CancellationToken ct)
        {
            ReadResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerRead;
            InitResponseCollections<DataValue, DataValueCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                nodesToRead.Count, operationLimit
                );

            foreach (var batchAttributesToRead in
                nodesToRead.Batch<ReadValueId, ReadValueIdCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.ReadAsync(
                    requestHeader,
                    maxAge,
                    timestampsToReturn,
                    batchAttributesToRead, ct).ConfigureAwait(false);

                DataValueCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchAttributesToRead);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchAttributesToRead);

                AddResponses<DataValue, DataValueCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region HistoryRead Methods
        /// <inheritdoc/>
        public override ResponseHeader HistoryRead(
            RequestHeader requestHeader,
            ExtensionObject historyReadDetails,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueIdCollection nodesToRead,
            out HistoryReadResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerHistoryReadData;
            if (historyReadDetails?.Body is ReadEventDetails)
            {
                operationLimit = OperationLimits.MaxNodesPerHistoryReadEvents;
            }

            InitResponseCollections<HistoryReadResult, HistoryReadResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                nodesToRead.Count, operationLimit
                );

            foreach (var batchNodesToRead in
                nodesToRead.Batch<HistoryReadValueId, HistoryReadValueIdCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.HistoryRead(requestHeader,
                    historyReadDetails,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    batchNodesToRead,
                    out HistoryReadResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchNodesToRead);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToRead);

                AddResponses<HistoryReadResult, HistoryReadResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<HistoryReadResponse> HistoryReadAsync(
            RequestHeader requestHeader,
            ExtensionObject historyReadDetails,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueIdCollection nodesToRead,
            CancellationToken ct)
        {
            HistoryReadResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerHistoryReadData;
            if (historyReadDetails?.Body is ReadEventDetails)
            {
                operationLimit = OperationLimits.MaxNodesPerHistoryReadEvents;
            }

            InitResponseCollections<HistoryReadResult, HistoryReadResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                nodesToRead.Count, operationLimit
                );

            foreach (var batchNodesToRead in
                nodesToRead.Batch<HistoryReadValueId, HistoryReadValueIdCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.HistoryReadAsync(requestHeader,
                    historyReadDetails,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    batchNodesToRead, ct).ConfigureAwait(false);

                HistoryReadResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchNodesToRead);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToRead);

                AddResponses<HistoryReadResult, HistoryReadResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region Write Methods
        /// <inheritdoc/>
        public override ResponseHeader Write(
            RequestHeader requestHeader,
            WriteValueCollection nodesToWrite,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerWrite;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out results, out diagnosticInfos, out var stringTable,
                nodesToWrite.Count, operationLimit
                );

            foreach (var batchNodesToWrite in
                nodesToWrite.Batch<WriteValue, WriteValueCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.Write(requestHeader,
                    batchNodesToWrite,
                    out StatusCodeCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchNodesToWrite);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToWrite);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<WriteResponse> WriteAsync(
            RequestHeader requestHeader,
            WriteValueCollection nodesToWrite,
            CancellationToken ct)
        {
            WriteResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerWrite;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                nodesToWrite.Count, operationLimit
                );

            foreach (var batchNodesToWrite in
                nodesToWrite.Batch<WriteValue, WriteValueCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.WriteAsync(requestHeader,
                    batchNodesToWrite, ct).ConfigureAwait(false);

                StatusCodeCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchNodesToWrite);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchNodesToWrite);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region HistoryUpdate Methods
        /// <inheritdoc/>
        public override ResponseHeader HistoryUpdate(
            RequestHeader requestHeader,
            ExtensionObjectCollection historyUpdateDetails,
            out HistoryUpdateResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            // see https://reference.opcfoundation.org/v104/Core/docs/Part11/6.8.1/ as to why
            // history update of event, data or annotations should be called individually.
            // Mixed arrays have unpredicatble results, only the first entry is checked and taken
            // as operation limit source
            uint operationLimit = OperationLimits.MaxNodesPerHistoryUpdateData;
            if (historyUpdateDetails.Count > 0 &&
                historyUpdateDetails[0]?.Body is UpdateEventDetails)
            {
                operationLimit = OperationLimits.MaxNodesPerHistoryUpdateEvents;
            }

            InitResponseCollections<HistoryUpdateResult, HistoryUpdateResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                historyUpdateDetails.Count, operationLimit
                );

            foreach (var batchHistoryUpdateDetails in
                historyUpdateDetails.Batch<ExtensionObject, ExtensionObjectCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.HistoryUpdate(requestHeader,
                    batchHistoryUpdateDetails,
                    out HistoryUpdateResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchHistoryUpdateDetails);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchHistoryUpdateDetails);

                AddResponses<HistoryUpdateResult, HistoryUpdateResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<HistoryUpdateResponse> HistoryUpdateAsync(
            RequestHeader requestHeader,
            ExtensionObjectCollection historyUpdateDetails,
            CancellationToken ct)
        {
            HistoryUpdateResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerHistoryUpdateData;
            if (historyUpdateDetails.Count > 0 &&
                historyUpdateDetails[0].TypeId == DataTypeIds.UpdateEventDetails)
            {
                operationLimit = OperationLimits.MaxNodesPerHistoryUpdateEvents;
            }

            InitResponseCollections<HistoryUpdateResult, HistoryUpdateResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                historyUpdateDetails.Count, operationLimit
                );

            foreach (var batchHistoryUpdateDetails in
                historyUpdateDetails.Batch<ExtensionObject, ExtensionObjectCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.HistoryUpdateAsync(requestHeader,
                    batchHistoryUpdateDetails, ct).ConfigureAwait(false);
                HistoryUpdateResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchHistoryUpdateDetails);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchHistoryUpdateDetails);

                AddResponses<HistoryUpdateResult, HistoryUpdateResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region Call Methods
        /// <inheritdoc/>
        public override ResponseHeader Call(
            RequestHeader requestHeader,
            CallMethodRequestCollection methodsToCall,
            out CallMethodResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxNodesPerMethodCall;
            InitResponseCollections<CallMethodResult, CallMethodResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                methodsToCall.Count, operationLimit
                );

            foreach (var batchMethodsToCall in
                methodsToCall.Batch<CallMethodRequest, CallMethodRequestCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.Call(requestHeader,
                    batchMethodsToCall,
                    out CallMethodResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchMethodsToCall);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchMethodsToCall);

                AddResponses<CallMethodResult, CallMethodResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<CallResponse> CallAsync(
            RequestHeader requestHeader,
            CallMethodRequestCollection methodsToCall,
            CancellationToken ct)
        {
            CallResponse response = null;

            uint operationLimit = OperationLimits.MaxNodesPerMethodCall;
            InitResponseCollections<CallMethodResult, CallMethodResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                methodsToCall.Count, operationLimit
                );

            foreach (var batchMethodsToCall in
                methodsToCall.Batch<CallMethodRequest, CallMethodRequestCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.CallAsync(requestHeader,
                    batchMethodsToCall, ct).ConfigureAwait(false);

                CallMethodResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchMethodsToCall);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchMethodsToCall);

                AddResponses<CallMethodResult, CallMethodResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region CreateMonitoredItems Methods
        /// <inheritdoc/>
        public override ResponseHeader CreateMonitoredItems(
            RequestHeader requestHeader,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemCreateRequestCollection itemsToCreate,
            out MonitoredItemCreateResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<MonitoredItemCreateResult, MonitoredItemCreateResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                itemsToCreate.Count, operationLimit
                );

            foreach (var batchItemsToCreate in
                itemsToCreate.Batch<MonitoredItemCreateRequest, MonitoredItemCreateRequestCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.CreateMonitoredItems(requestHeader,
                    subscriptionId,
                    timestampsToReturn,
                    batchItemsToCreate,
                    out MonitoredItemCreateResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchItemsToCreate);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchItemsToCreate);

                AddResponses<MonitoredItemCreateResult, MonitoredItemCreateResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<CreateMonitoredItemsResponse> CreateMonitoredItemsAsync(
            RequestHeader requestHeader,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemCreateRequestCollection itemsToCreate,
            CancellationToken ct)
        {
            CreateMonitoredItemsResponse response = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<MonitoredItemCreateResult, MonitoredItemCreateResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                itemsToCreate.Count, operationLimit
                );

            foreach (var batchItemsToCreate in
                itemsToCreate.Batch<MonitoredItemCreateRequest, MonitoredItemCreateRequestCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.CreateMonitoredItemsAsync(requestHeader,
                    subscriptionId,
                    timestampsToReturn,
                    batchItemsToCreate, ct).ConfigureAwait(false);

                MonitoredItemCreateResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchItemsToCreate);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchItemsToCreate);

                AddResponses<MonitoredItemCreateResult, MonitoredItemCreateResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region ModifyMonitoredItems Methods
        /// <inheritdoc/>
        public override ResponseHeader ModifyMonitoredItems(
            RequestHeader requestHeader,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemModifyRequestCollection itemsToModify,
            out MonitoredItemModifyResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<MonitoredItemModifyResult, MonitoredItemModifyResultCollection>(
                out results, out diagnosticInfos, out var stringTable,
                itemsToModify.Count, operationLimit
                );

            foreach (var batchItemsToModify in
                itemsToModify.Batch<MonitoredItemModifyRequest, MonitoredItemModifyRequestCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.ModifyMonitoredItems(requestHeader,
                    subscriptionId,
                    timestampsToReturn,
                    batchItemsToModify,
                    out MonitoredItemModifyResultCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchItemsToModify);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchItemsToModify);

                AddResponses<MonitoredItemModifyResult, MonitoredItemModifyResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<ModifyMonitoredItemsResponse> ModifyMonitoredItemsAsync(
            RequestHeader requestHeader,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemModifyRequestCollection itemsToModify,
            CancellationToken ct)
        {
            ModifyMonitoredItemsResponse response = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<MonitoredItemModifyResult, MonitoredItemModifyResultCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                itemsToModify.Count, operationLimit
                );

            foreach (var batchItemsToModify in
                itemsToModify.Batch<MonitoredItemModifyRequest, MonitoredItemModifyRequestCollection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.ModifyMonitoredItemsAsync(requestHeader,
                    subscriptionId,
                    timestampsToReturn,
                    batchItemsToModify, ct).ConfigureAwait(false);

                MonitoredItemModifyResultCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchItemsToModify);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchItemsToModify);

                AddResponses<MonitoredItemModifyResult, MonitoredItemModifyResultCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region SetMonitoringMode Methods
        /// <inheritdoc/>
        public override ResponseHeader SetMonitoringMode(
            RequestHeader requestHeader,
            uint subscriptionId,
            MonitoringMode monitoringMode,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out results, out diagnosticInfos, out var stringTable,
                monitoredItemIds.Count, operationLimit
                );

            foreach (var batchMonitoredItemIds in
                monitoredItemIds.Batch<UInt32, UInt32Collection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.SetMonitoringMode(requestHeader,
                    subscriptionId,
                    monitoringMode,
                    batchMonitoredItemIds,
                    out StatusCodeCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchMonitoredItemIds);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchMonitoredItemIds);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<SetMonitoringModeResponse> SetMonitoringModeAsync(
            RequestHeader requestHeader,
            uint subscriptionId,
            MonitoringMode monitoringMode,
            UInt32Collection monitoredItemIds,
            CancellationToken ct)
        {
            SetMonitoringModeResponse response = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                monitoredItemIds.Count, operationLimit
                );

            foreach (var batchMonitoredItemIds in
                monitoredItemIds.Batch<UInt32, UInt32Collection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.SetMonitoringModeAsync(requestHeader,
                    subscriptionId,
                    monitoringMode,
                    batchMonitoredItemIds, ct).ConfigureAwait(false);

                StatusCodeCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchMonitoredItemIds);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchMonitoredItemIds);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region SetTriggering Methods
        /// <inheritdoc/>
        public override ResponseHeader SetTriggering(
            RequestHeader requestHeader,
            uint subscriptionId,
            uint triggeringItemId,
            UInt32Collection linksToAdd,
            UInt32Collection linksToRemove,
            out StatusCodeCollection addResults,
            out DiagnosticInfoCollection addDiagnosticInfos,
            out StatusCodeCollection removeResults,
            out DiagnosticInfoCollection removeDiagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out addResults, out addDiagnosticInfos, out var stringTable,
                linksToAdd.Count, operationLimit
                );

            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out removeResults, out removeDiagnosticInfos, out _,
                linksToRemove.Count, operationLimit
                );

            foreach (var batchLinksToAdd in
                linksToAdd.Batch<UInt32, UInt32Collection>(operationLimit))
            {
                UInt32Collection batchLinksToRemove;
                if (operationLimit == 0)
                {
                    batchLinksToRemove = linksToRemove;
                    linksToRemove = new UInt32Collection();
                }
                else if (batchLinksToAdd.Count < operationLimit)
                {
                    batchLinksToRemove = new UInt32Collection(linksToRemove.Take((int)operationLimit - batchLinksToAdd.Count));
                    linksToRemove = new UInt32Collection(linksToRemove.Skip(batchLinksToRemove.Count));
                }
                else
                {
                    batchLinksToRemove = new UInt32Collection();
                }

                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.SetTriggering(requestHeader,
                    subscriptionId,
                    triggeringItemId,
                    batchLinksToAdd,
                    batchLinksToRemove,
                    out StatusCodeCollection batchAddResults,
                    out DiagnosticInfoCollection batchAddDiagnosticInfos,
                    out StatusCodeCollection batchRemoveResults,
                    out DiagnosticInfoCollection batchRemoveDiagnosticInfos
                    );

                ClientBase.ValidateResponse(batchAddResults, batchLinksToAdd);
                ClientBase.ValidateDiagnosticInfos(batchAddDiagnosticInfos, batchLinksToAdd);
                ClientBase.ValidateResponse(batchRemoveResults, batchLinksToRemove);
                ClientBase.ValidateDiagnosticInfos(batchRemoveDiagnosticInfos, batchLinksToRemove);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref addResults, ref addDiagnosticInfos, ref stringTable, batchAddResults, batchAddDiagnosticInfos, responseHeader.StringTable);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref removeResults, ref removeDiagnosticInfos, ref stringTable, batchRemoveResults, batchRemoveDiagnosticInfos, responseHeader.StringTable);
            }

            if (linksToRemove.Count > 0)
            {
                foreach (var batchLinksToRemove in
                    linksToRemove.Batch<UInt32, UInt32Collection>(operationLimit))
                {
                    if (requestHeader != null)
                    {
                        requestHeader.RequestHandle = 0;
                    }

                    var batchLinksToAdd = new UInt32Collection();
                    responseHeader = base.SetTriggering(requestHeader,
                        subscriptionId,
                        triggeringItemId,
                        batchLinksToAdd,
                        batchLinksToRemove,
                        out StatusCodeCollection batchAddResults,
                        out DiagnosticInfoCollection batchAddDiagnosticInfos,
                        out StatusCodeCollection batchRemoveResults,
                        out DiagnosticInfoCollection batchRemoveDiagnosticInfos
                        );

                    ClientBase.ValidateResponse(batchAddResults, batchLinksToAdd);
                    ClientBase.ValidateDiagnosticInfos(batchAddDiagnosticInfos, batchLinksToAdd);
                    ClientBase.ValidateResponse(batchRemoveResults, batchLinksToRemove);
                    ClientBase.ValidateDiagnosticInfos(batchRemoveDiagnosticInfos, batchLinksToRemove);

                    AddResponses<StatusCode, StatusCodeCollection>(
                        ref addResults, ref addDiagnosticInfos, ref stringTable, batchAddResults, batchAddDiagnosticInfos, responseHeader.StringTable);

                    AddResponses<StatusCode, StatusCodeCollection>(
                        ref removeResults, ref removeDiagnosticInfos, ref stringTable, batchRemoveResults, batchRemoveDiagnosticInfos, responseHeader.StringTable);
                }
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<SetTriggeringResponse> SetTriggeringAsync(
            RequestHeader requestHeader,
            uint subscriptionId,
            uint triggeringItemId,
            UInt32Collection linksToAdd,
            UInt32Collection linksToRemove,
            CancellationToken ct)
        {
            SetTriggeringResponse response = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var addResults, out var addDiagnosticInfos, out var stringTable,
                linksToAdd.Count, operationLimit
                );

            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var removeResults, out var removeDiagnosticInfos, out _,
                linksToRemove.Count, operationLimit
                );

            foreach (var batchLinksToAdd in
                linksToAdd.Batch<UInt32, UInt32Collection>(operationLimit))
            {
                UInt32Collection batchLinksToRemove;
                if (operationLimit == 0)
                {
                    batchLinksToRemove = linksToRemove;
                    linksToRemove = new UInt32Collection();
                }
                else if (batchLinksToAdd.Count < operationLimit)
                {
                    batchLinksToRemove = new UInt32Collection(linksToRemove.Take((int)operationLimit - batchLinksToAdd.Count));
                    linksToRemove = new UInt32Collection(linksToRemove.Skip(batchLinksToRemove.Count));
                }
                else
                {
                    batchLinksToRemove = new UInt32Collection();
                }

                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.SetTriggeringAsync(requestHeader,
                    subscriptionId,
                    triggeringItemId,
                    batchLinksToAdd,
                    batchLinksToRemove,
                    ct).ConfigureAwait(false);

                StatusCodeCollection batchAddResults = response.AddResults;
                DiagnosticInfoCollection batchAddDiagnosticInfos = response.AddDiagnosticInfos;
                StatusCodeCollection batchRemoveResults = response.RemoveResults;
                DiagnosticInfoCollection batchRemoveDiagnosticInfos = response.RemoveDiagnosticInfos;

                ClientBase.ValidateResponse(batchAddResults, batchLinksToAdd);
                ClientBase.ValidateDiagnosticInfos(batchAddDiagnosticInfos, batchLinksToAdd);
                ClientBase.ValidateResponse(batchRemoveResults, batchLinksToRemove);
                ClientBase.ValidateDiagnosticInfos(batchRemoveDiagnosticInfos, batchLinksToRemove);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref addResults, ref addDiagnosticInfos, ref stringTable, batchAddResults, batchAddDiagnosticInfos, response.ResponseHeader.StringTable);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref removeResults, ref removeDiagnosticInfos, ref stringTable, batchRemoveResults, batchRemoveDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            if (linksToRemove.Count > 0)
            {
                foreach (var batchLinksToRemove in
                    linksToRemove.Batch<UInt32, UInt32Collection>(operationLimit))
                {
                    if (requestHeader != null)
                    {
                        requestHeader.RequestHandle = 0;
                    }

                    var batchLinksToAdd = new UInt32Collection();
                    response = await base.SetTriggeringAsync(requestHeader,
                        subscriptionId,
                        triggeringItemId,
                        batchLinksToAdd,
                        batchLinksToRemove,
                        ct).ConfigureAwait(false);

                    StatusCodeCollection batchAddResults = response.AddResults;
                    DiagnosticInfoCollection batchAddDiagnosticInfos = response.AddDiagnosticInfos;
                    StatusCodeCollection batchRemoveResults = response.RemoveResults;
                    DiagnosticInfoCollection batchRemoveDiagnosticInfos = response.RemoveDiagnosticInfos;

                    ClientBase.ValidateResponse(batchAddResults, batchLinksToAdd);
                    ClientBase.ValidateDiagnosticInfos(batchAddDiagnosticInfos, batchLinksToAdd);
                    ClientBase.ValidateResponse(batchRemoveResults, batchLinksToRemove);
                    ClientBase.ValidateDiagnosticInfos(batchRemoveDiagnosticInfos, batchLinksToRemove);

                    AddResponses<StatusCode, StatusCodeCollection>(
                        ref addResults, ref addDiagnosticInfos, ref stringTable, batchAddResults, batchAddDiagnosticInfos, response.ResponseHeader.StringTable);

                    AddResponses<StatusCode, StatusCodeCollection>(
                        ref removeResults, ref removeDiagnosticInfos, ref stringTable, batchRemoveResults, batchRemoveDiagnosticInfos, response.ResponseHeader.StringTable);
                }
            }

            response.AddResults = addResults;
            response.AddDiagnosticInfos = addDiagnosticInfos;
            response.RemoveResults = removeResults;
            response.RemoveDiagnosticInfos = removeDiagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region DeleteMonitoredItems Methods
        /// <inheritdoc/>
        public override ResponseHeader DeleteMonitoredItems(
            RequestHeader requestHeader,
            uint subscriptionId,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            ResponseHeader responseHeader = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out results, out diagnosticInfos, out var stringTable,
                monitoredItemIds.Count, operationLimit
                );

            foreach (var batchMonitoredItemIds in
                monitoredItemIds.Batch<UInt32, UInt32Collection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                responseHeader = base.DeleteMonitoredItems(requestHeader,
                    subscriptionId,
                    batchMonitoredItemIds,
                    out StatusCodeCollection batchResults,
                    out DiagnosticInfoCollection batchDiagnosticInfos);

                ClientBase.ValidateResponse(batchResults, batchMonitoredItemIds);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchMonitoredItemIds);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, responseHeader.StringTable);
            }

            responseHeader.StringTable = stringTable;
            return responseHeader;
        }

        /// <inheritdoc/>
        public override async Task<DeleteMonitoredItemsResponse> DeleteMonitoredItemsAsync(
            RequestHeader requestHeader,
            uint subscriptionId,
            UInt32Collection monitoredItemIds,
            CancellationToken ct)
        {
            DeleteMonitoredItemsResponse response = null;

            uint operationLimit = OperationLimits.MaxMonitoredItemsPerCall;
            InitResponseCollections<StatusCode, StatusCodeCollection>(
                out var results, out var diagnosticInfos, out var stringTable,
                monitoredItemIds.Count, operationLimit
                );

            foreach (var batchMonitoredItemIds in
                monitoredItemIds.Batch<UInt32, UInt32Collection>(operationLimit))
            {
                if (requestHeader != null)
                {
                    requestHeader.RequestHandle = 0;
                }

                response = await base.DeleteMonitoredItemsAsync(
                    requestHeader,
                    subscriptionId,
                    batchMonitoredItemIds,
                    ct).ConfigureAwait(false);
                StatusCodeCollection batchResults = response.Results;
                DiagnosticInfoCollection batchDiagnosticInfos = response.DiagnosticInfos;

                ClientBase.ValidateResponse(batchResults, batchMonitoredItemIds);
                ClientBase.ValidateDiagnosticInfos(batchDiagnosticInfos, batchMonitoredItemIds);

                AddResponses<StatusCode, StatusCodeCollection>(
                    ref results, ref diagnosticInfos, ref stringTable, batchResults, batchDiagnosticInfos, response.ResponseHeader.StringTable);
            }

            response.Results = results;
            response.DiagnosticInfos = diagnosticInfos;
            response.ResponseHeader.StringTable = stringTable;

            return response;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize the collections for a service call.
        /// </summary>
        /// <remarks>
        /// Preset the result collections with null if the operation limit
        /// is sufficient or with the final size if batching is necessary.
        /// </remarks>
        private static void InitResponseCollections<T, C>(
            out C results,
            out DiagnosticInfoCollection diagnosticInfos,
            out StringCollection stringTable,
            int count,
            uint operationLimit) where C : List<T>, new()
        {
            if (count <= operationLimit)
            {
                results = null;
                diagnosticInfos = null;
                stringTable = null;
            }
            else
            {
                results = new C() {
                    Capacity = count
                };
                diagnosticInfos = new DiagnosticInfoCollection(count);
                stringTable = new StringCollection();
            }
        }

        /// <summary>
        /// Add the result of a batched service call to the results.
        /// </summary>
        /// <remarks>
        /// Assigns the batched collection result to the result if the result
        /// collection is not initialized, adds the range to the result
        /// collections otherwise.
        /// The string table indexes are updated in the diagnostic infos if necessary.
        /// </remarks>
        private static void AddResponses<T, C>(
            ref C results,
            ref DiagnosticInfoCollection diagnosticInfos,
            ref StringCollection stringTable,
            C batchedResults,
            DiagnosticInfoCollection batchedDiagnosticInfos,
            StringCollection batchedStringTable) where C : List<T>
        {
            if (results == null)
            {
                results = batchedResults;
                diagnosticInfos = batchedDiagnosticInfos;
                stringTable = batchedStringTable;
            }
            else
            {
                bool hasDiagnosticInfos = diagnosticInfos.Count > 0;
                bool hasEmptyDiagnosticInfos = diagnosticInfos.Count == 0 && results.Count > 0;
                bool hasBatchDiagnosticInfos = batchedDiagnosticInfos.Count > 0;
                int correctionCount = 0;
                if (hasBatchDiagnosticInfos && hasEmptyDiagnosticInfos)
                {
                    correctionCount = results.Count;
                }
                else if (!hasBatchDiagnosticInfos && hasDiagnosticInfos)
                {
                    correctionCount = batchedResults.Count;
                }
                if (correctionCount > 0)
                {
                    // fill missing diagnostics infos with null entries
                    for (int i = 0; i < correctionCount; i++)
                    {
                        diagnosticInfos.Add(null);
                    }
                }
                else if (batchedStringTable.Count > 0)
                {
                    // correct indexes in the string table
                    int stringTableOffset = stringTable.Count;
                    foreach (DiagnosticInfo diagnosticInfo in batchedDiagnosticInfos)
                    {
                        UpdateDiagnosticInfoIndexes(diagnosticInfo, stringTableOffset);
                    }
            }
                results.AddRange(batchedResults);
                diagnosticInfos.AddRange(batchedDiagnosticInfos);
                stringTable.AddRange(batchedStringTable);
            }
        }

        private static void UpdateDiagnosticInfoIndexes(
            DiagnosticInfo diagnosticInfo,
            int stringTableOffset)
        {
            int depth = 0;
            while (diagnosticInfo != null && depth++ < DiagnosticInfo.MaxInnerDepth)
            {
                if (diagnosticInfo.LocalizedText >= 0)
                {
                    diagnosticInfo.LocalizedText += stringTableOffset;
                }
                if (diagnosticInfo.Locale >= 0)
                {
                    diagnosticInfo.Locale += stringTableOffset;
                }
                if (diagnosticInfo.NamespaceUri >= 0)
                {
                    diagnosticInfo.NamespaceUri += stringTableOffset;
                }
                if (diagnosticInfo.SymbolicId >= 0)
                {
                    diagnosticInfo.SymbolicId += stringTableOffset;
                }
                diagnosticInfo = diagnosticInfo.InnerDiagnosticInfo;
            }
        }
        #endregion

        #region Private 
        private OperationLimits operationLimits_;
        #endregion
    }

    /// <summary>
    /// Extension helpers for client service calls.
    /// </summary>
    public static class SessionClientExtensions
    {
        /// <summary>
        /// Returns batches of a collection for processing.
        /// </summary>
        /// <remarks>
        /// Returns the original collection if batchsize is 0 or the collection count is smaller than the batch size.
        /// </remarks>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <typeparam name="C">The type of the items in the collection.</typeparam>
        /// <param name="collection">The collection from which items are batched.</param>
        /// <param name="batchSize">The size of a batch.</param>
        /// <returns>The collection.</returns>
        internal static IEnumerable<C> Batch<T, C>(this C collection, uint batchSize) where C : List<T>, new()
        {
            if (collection.Count < batchSize || batchSize == 0)
            {
                yield return collection;
            }
            else
            {
                C nextbatch = new C {
                    Capacity = (int)batchSize
                };
                foreach (T item in collection)
                {
                    nextbatch.Add(item);
                    if (nextbatch.Count == batchSize)
                    {
                        yield return nextbatch;
                        nextbatch = new C {
                            Capacity = (int)batchSize
                        };
                    }
                }
                if (nextbatch.Count > 0)
                {
                    yield return nextbatch;
                }
            }
        }
    }
}
