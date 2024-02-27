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
using System.Text;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// An interface to an object that manages a set of nodes in the address space.
    /// </summary>
    public interface IUaBaseNodeManager
    {
        /// <summary>
        /// Returns the NamespaceUris for the Nodes belonging to the NodeManager.
        /// </summary>
        /// <remarks>
        /// By default the MasterNodeManager uses the namespaceIndex to determine who owns an Node.
        /// 
        /// Servers that do not wish to partition their address space this way must provide their own
        /// implementation of MasterNodeManager.GetManagerHandle().
        /// 
        /// NodeManagers which depend on a custom partitioning scheme must return a null value.
        /// </remarks>
        IEnumerable<string> NamespaceUris { get; }

        /// <summary>
        /// Creates the address space by loading any configuration information an connecting to an underlying system (if applicable).
        /// </summary>
        /// <returns>A table of references that need to be added to other node managers.</returns>
        /// <remarks>
        /// A node manager owns a set of nodes. These nodes may be known in advance or they may be stored in an
        /// external system are retrieved on demand. These nodes may have two way references to nodes that are owned 
        /// by other node managers. In these cases, the node managers only manage one half of those references. The
        /// other half of the reference should be returned to the MasterNodeManager.
        /// </remarks>
        void CreateAddressSpace(IDictionary<NodeId,IList<IReference>> externalReferences);

        /// <summary>
        /// Deletes the address by releasing all resources and disconnecting from any underlying system.
        /// </summary>
        void DeleteAddressSpace();

        /// <summary>
        /// Returns an opaque handle identifying to the node to the node manager.
        /// </summary>
        /// <param name="nodeId">The node to get the handle for.</param>
        /// <returns>A node handle, null if the node manager does not recognize the node id.</returns>
        /// <remarks>
        /// The method must not block by querying an underlying system. If the node manager wraps an 
        /// underlying system then it must check to see if it recognizes the syntax of the node id. 
        /// The handle in this case may simply be a partially parsed version of the node id. 
        /// </remarks>
        object GetManagerHandle(NodeId nodeId);

        /// <summary>
        /// Adds references to the node manager.
        /// </summary>
        /// <remarks>
        /// The node manager checks the dictionary for nodes that it owns and ensures the associated references exist.
        /// </remarks>
        void AddReferences(IDictionary<NodeId, IList<IReference>> references);

        /// <summary>
        /// Deletes a reference.
        /// </summary>
        ServiceResult DeleteReference(
            object sourceHandle,
            NodeId referenceTypeId,
            bool isInverse,
            ExpandedNodeId targetId,
            bool deleteBidirectional);

        /// <summary>
        /// Returns the metadata associated with the node.
        /// </summary>
        /// <remarks>
        /// Returns null if the node does not exist.
        /// </remarks>
        UaNodeMetadata GetNodeMetadata(
            UaServerOperationContext context,
            object           targetHandle,
            BrowseResultMask resultMask);

        /// <summary>
        /// Returns the set of references that meet the filter criteria.
        /// </summary>
        /// <param name="context">The context to used when processing the request.</param>
        /// <param name="continuationPoint">The continuation point that stores the state of the Browse operation.</param>
        /// <param name="references">The list of references that meet the filter criteria.</param>     
        /// <remarks>
        /// NodeManagers will likely have references to other NodeManagers which means they will not be able
        /// to apply the NodeClassMask or fill in the attributes for the target Node. In these cases the 
        /// NodeManager must return a ReferenceDescription with the NodeId and ReferenceTypeId set. The caller will
        /// be responsible for filling in the target attributes. 
        /// The references parameter may already contain references when the method is called. The implementer must 
        /// include these references when calculating whether a continuation point must be returned.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if the context, continuationPoint or references parameters are null.</exception>
        /// <exception cref="ServiceResultException">Thrown if an error occurs during processing.</exception>
        void Browse(
            UaServerOperationContext            context,
            ref UaContinuationPoint       continuationPoint,
            IList<ReferenceDescription> references);

        /// <summary>
        /// Finds the targets of the relative path from the source node.
        /// </summary>
        /// <param name="context">The context to used when processing the request.</param>
        /// <param name="sourceHandle">The handle for the source node.</param>
        /// <param name="relativePath">The relative path to follow.</param>
        /// <param name="targetIds">The NodeIds for any target at the end of the relative path.</param>
        /// <param name="unresolvedTargetIds">The NodeIds for any local target that is in another NodeManager.</param>
        /// <remarks>
        /// A null context indicates that the server's internal logic is making the call.
        /// The first target in the list must be the target that matches the instance declaration (if applicable).
        /// Any local targets that belong to other NodeManagers are returned as unresolvedTargetIds. 
        /// The caller must check the BrowseName to determine if it matches the relativePath.
        /// The implementor must not throw an exception if the source or target nodes do not exist.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if the sourceHandle, relativePath or targetIds parameters are null.</exception>
        void TranslateBrowsePath(
            UaServerOperationContext      context,
            object                sourceHandle,
            RelativePathElement   relativePath,
            IList<ExpandedNodeId> targetIds,
            IList<NodeId>         unresolvedTargetIds);

        /// <summary>
        /// Reads the attribute values for a set of nodes.
        /// </summary>
        /// <remarks>
        /// The MasterNodeManager pre-processes the nodesToRead and ensures that:
        ///    - the AttributeId is a known attribute.
        ///    - the IndexRange, if specified, is valid.
        ///    - the DataEncoding and the IndexRange are not specified if the AttributeId is not Value.
        ///
        /// The MasterNodeManager post-processes the values by:
        ///    - sets values[ii].StatusCode to the value of errors[ii].Code
        ///    - creates a instance of DataValue if one does not exist and an errors[ii] is bad.
        ///    - removes timestamps from the DataValue if the client does not want them.
        /// 
        /// The node manager must ignore ReadValueId with the Processed flag set to true.
        /// The node manager must set the Processed flag for any ReadValueId that it processes.
        /// </remarks>
        void Read(
            UaServerOperationContext     context,
            double               maxAge,
            IList<ReadValueId>   nodesToRead,
            IList<DataValue>     values,
            IList<ServiceResult> errors);

        /// <summary>
        /// Reads the history of a set of items.
        /// </summary>
        void HistoryRead(
            UaServerOperationContext          context,
            HistoryReadDetails        details,
            TimestampsToReturn        timestampsToReturn,
            bool                      releaseContinuationPoints,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult>  results,
            IList<ServiceResult>      errors);

        /// <summary>
        /// Writes a set of values.
        /// </summary>
        /// <remarks>
        /// Each node manager should only process node ids that it recognizes. If it processes a value it
        /// must set the Processed flag in the WriteValue structure.
        /// </remarks>
        void Write(
            UaServerOperationContext     context,
            IList<WriteValue>    nodesToWrite,
            IList<ServiceResult> errors);

        /// <summary>
        /// Updates the history for a set of nodes.
        /// </summary>
        void HistoryUpdate(
            UaServerOperationContext            context,
            Type                        detailsType,
            IList<HistoryUpdateDetails> nodesToUpdate,
            IList<HistoryUpdateResult>  results,
            IList<ServiceResult>        errors);

        /// <summary>
        /// Calls a method defined on a object.
        /// </summary>
        void Call(
            UaServerOperationContext         context,
            IList<CallMethodRequest> methodsToCall,
            IList<CallMethodResult>  results,
            IList<ServiceResult>     errors);

        /// <summary>
        /// Tells the NodeManager to report events from the specified notifier.
        /// </summary>
        /// <remarks>
        /// This method may be called multiple times for the name monitoredItemId if the
        /// context for that UaMonitoredItem changes (i.e. UserIdentity and/or Locales).
        /// </remarks>
        ServiceResult SubscribeToEvents(
            UaServerOperationContext    context,
            object              sourceId,
            uint                subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe);

        /// <summary>
        /// Tells the NodeManager to report events all events from all sources.
        /// </summary>
        /// <remarks>
        /// This method may be called multiple times for the name monitoredItemId if the
        /// context for that UaMonitoredItem changes (i.e. UserIdentity and/or Locales).
        /// </remarks>
        ServiceResult SubscribeToAllEvents(
            UaServerOperationContext   context,
            uint                subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe);

        /// <summary>
        /// Tells the NodeManager to refresh any conditions.
        /// </summary>
        ServiceResult ConditionRefresh(
            UaServerOperationContext           context,
            IList<IUaEventMonitoredItem> monitoredItems);

        /// <summary>
        /// Creates a set of monitored items.
        /// </summary>
        void CreateMonitoredItems(
            UaServerOperationContext                  context,
            uint                              subscriptionId,
            double                            publishingInterval,
            TimestampsToReturn                timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterErrors,
            IList<IUaMonitoredItem>             monitoredItems,
            ref long                          globalIdCounter);

        /// <summary>
        /// Modifies a set of monitored items.
        /// </summary>
        void ModifyMonitoredItems(
            UaServerOperationContext                  context,
            TimestampsToReturn                timestampsToReturn,
            IList<IUaMonitoredItem>             monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterErrors);

        /// <summary>
        /// Deletes a set of monitored items.
        /// </summary>
        void DeleteMonitoredItems(
            UaServerOperationContext      context,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool>           processedItems,
            IList<ServiceResult>  errors);

        /// <summary>
        /// Transfers a set of monitored items.
        /// </summary>
        /// <remarks>
        /// Queue initial values from monitored items in the node managers.
        /// </remarks>
        void TransferMonitoredItems(
            UaServerOperationContext context,
            bool sendInitialValues,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors);

        /// <summary>
        /// Changes the monitoring mode for a set of monitored items.
        /// </summary>
        void SetMonitoringMode(
            UaServerOperationContext      context,
            MonitoringMode        monitoringMode,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool>           processedItems,
            IList<ServiceResult>  errors);
    }
}
