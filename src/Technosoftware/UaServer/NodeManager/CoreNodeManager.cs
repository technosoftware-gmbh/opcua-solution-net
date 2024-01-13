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
using System.Threading.Tasks;

using Opc.Ua;

#endregion

#pragma warning disable 0618

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// The default node manager for the server.
    /// </summary>
    /// <remarks>
    /// Every Server has one instance of this NodeManager. 
    /// It stores objects that implement ILocalNode and indexes them by NodeId.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public partial class CoreNodeManager : IUaBaseNodeManager, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object with default values.
        /// </summary>
        public CoreNodeManager(
            IUaServerData          server,
            ApplicationConfiguration configuration,
            ushort                   dynamicNamespaceIndex)
        {
            if (server == null)        throw new ArgumentNullException(nameof(server));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
                  
            server_                         = server;
            nodes_                          = new NodeTable(server.NamespaceUris, server.ServerUris, server.TypeTree);
            monitoredItems_                 = new Dictionary<uint,UaMonitoredItem>();
            defaultMinimumSamplingInterval_ = 1000;
            namespaceUris_                  = new List<string>();
            dynamicNamespaceIndex_          = dynamicNamespaceIndex; 

            // use namespace 1 if out of range.
            if (dynamicNamespaceIndex_ == 0 || dynamicNamespaceIndex_ >= server.NamespaceUris.Count)
            {
                dynamicNamespaceIndex_ = 1;
            }

            samplingGroupManager_ = new SamplingGroupManager(
                server, 
                this,
                (uint)configuration.ServerConfiguration.MaxNotificationQueueSize,
                configuration.ServerConfiguration.AvailableSamplingRates);
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
                List<INode> nodes = null;                

                lock(Lock)
                {
                    nodes = new List<INode>(nodes_);
                    nodes_.Clear();

                    monitoredItems_.Clear();
                }

                foreach (var node in nodes)
                {
                    Utils.SilentDispose(node);
                }

                Utils.SilentDispose(samplingGroupManager_);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Acquires the lock on the node manager.
        /// </summary>
        public object Lock { get; } = new object();

        /// <summary>
        ///     The root for the alias assigned to the node manager.
        /// </summary>
        public string AliasRoot { get; set; }

        /// <summary>
        ///     The predefined nodes managed by the node manager.
        /// </summary>
        public Dictionary<NodeId, IUaMonitoredNode> MonitoredNodes
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the table of nodes being monitored.
        /// </summary>
        public NodeIdDictionary<NodeState> PredefinedNodes
        {
            get { return null; }
        }

        /// <summary>
        ///     Returns true if the namespace for the node id is one of the namespaces managed by the node manager.
        /// </summary>
        /// <param name="nodeId">The node id to check.</param>
        /// <returns>True if the namespace is one of the nodes.</returns>
        public virtual bool IsNodeIdInNamespace(NodeId nodeId)
        {
            return false;
        }

        /// <summary>
        ///     Returns true if the node is in the view.
        /// </summary>
        public virtual bool IsNodeInView(UaServerOperationContext context, NodeId viewId, object nodeHandle)
        {
            return false;
        }

        /// <summary>
        /// Returns the metadata containing the AccessRestrictions, RolePermissions and UserRolePermissions for the node.
        /// Returns null if the node does not exist.
        /// </summary>
        /// <remarks>
        /// This method validates any placeholder handle.
        /// </remarks>
        public virtual UaNodeMetadata GetPermissionMetadata(
            UaServerOperationContext context,
            object targetHandle,
            BrowseResultMask resultMask,
            Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes,
            bool permissionsOnly)
        {
            return null;
        }
        #endregion

        /// <summary>
        /// Imports the nodes from a dictionary of NodeState objects.
        /// </summary>
        public void ImportNodes(ISystemContext context, IEnumerable<NodeState> predefinedNodes)
        {
            ImportNodes(context, predefinedNodes, false);
        }

        /// <summary>
        /// Imports the nodes from a dictionary of NodeState objects.
        /// </summary>
        internal void ImportNodes(ISystemContext context, IEnumerable<NodeState> predefinedNodes, bool isInternal)
        {
            var nodesToExport = new NodeTable(ServerData.NamespaceUris, ServerData.ServerUris, ServerData.TypeTree);

            foreach (var node in predefinedNodes)
            {
                node.Export(context, nodesToExport);
            }

            lock (ServerData.CoreNodeManager.Lock)
            {
                foreach (ILocalNode nodeToExport in nodesToExport)
                {
                    ServerData.CoreNodeManager.AttachNode(nodeToExport, isInternal);
                }
            }
        }
 
        #region IUaBaseNodeManager Members
        /// <summary cref="IUaBaseNodeManager.NamespaceUris" />
        public IEnumerable<string> NamespaceUris
        {
            get
            {
                return namespaceUris_;
            }
        }

        /// <summary cref="IUaBaseNodeManager.CreateAddressSpace" />
        /// <remarks>
        /// Populates the NodeManager by loading the standard nodes from an XML file stored as an embedded resource.
        /// </remarks>
        public void CreateAddressSpace(IDictionary<NodeId,IList<IReference>> externalReferences)
        {
            // TBD
        }
                
        /// <summary cref="IUaBaseNodeManager.DeleteAddressSpace" />
        /// <remarks>
        /// Disposes all of the nodes.
        /// </remarks>
        public void DeleteAddressSpace()
        {
            var nodesToDispose = new List<IDisposable>();

            lock(Lock)
            {
                // collect nodes to dispose.
                foreach (var node in nodes_)
                {
                    var disposable = node as IDisposable;

                    if (disposable != null)
                    {
                        nodesToDispose.Add(disposable);
                    }
                }

                nodes_.Clear();    
            }

            // dispose of the nodes.
            foreach (var disposable in nodesToDispose)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Unexpected error disposing a Node object.");
                }
            }
        }
        
        /// <see cref="IUaBaseNodeManager.GetManagerHandle" />
        public object GetManagerHandle(NodeId nodeId)
        {
            lock(Lock)
            {
                if (NodeId.IsNull(nodeId))
                {
                    return null;
                }

                return GetLocalNode(nodeId);
            }
        }
                
        /// <see cref="IUaBaseNodeManager.TranslateBrowsePath(UaServerOperationContext,object,RelativePathElement,IList{ExpandedNodeId},IList{NodeId})" />
        public void TranslateBrowsePath(
            UaServerOperationContext      context,
            object                sourceHandle, 
            RelativePathElement   relativePath, 
            IList<ExpandedNodeId> targetIds,
            IList<NodeId>         unresolvedTargetIds)
        {
            if (sourceHandle == null) throw new ArgumentNullException(nameof(sourceHandle));
            if (relativePath == null) throw new ArgumentNullException(nameof(relativePath));
            if (targetIds == null) throw new ArgumentNullException(nameof(targetIds));
            if (unresolvedTargetIds == null) throw new ArgumentNullException(nameof(unresolvedTargetIds));

            // check for valid handle.
            var source = sourceHandle as ILocalNode;

            if (source == null)
            {
                return;
            }
            
            lock(Lock)
            {
                // find the references that meet the filter criteria.
                var references = source.References.Find(
                    relativePath.ReferenceTypeId, 
                    relativePath.IsInverse, 
                    relativePath.IncludeSubtypes, 
                    server_.TypeTree);

                // nothing more to do.
                if (references == null || references.Count == 0)
                {
                    return;
                }

                // find targets with matching browse names.
                foreach (var reference in references)
                {
                    INode target = GetLocalNode(reference.TargetId);

                    // target is not known to the node manager.
                    if (target == null)
                    {
                        // ignore unknown external references.
                        if (reference.TargetId.IsAbsolute)
                        {
                            continue;
                        }

                        // caller must check the browse name.
                        unresolvedTargetIds.Add((NodeId)reference.TargetId);
                        continue;
                    }

                    // check browse name.
                    if (target.BrowseName == relativePath.TargetName)
                    {
                        targetIds.Add(reference.TargetId);
                    }
                }
            }
        }

        #region Browse
        /// <see cref="IUaBaseNodeManager.Browse" />
        public void Browse(
            UaServerOperationContext            context,
            ref UaContinuationPoint       continuationPoint,
            IList<ReferenceDescription> references)
        {              
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));
            if (references == null) throw new ArgumentNullException(nameof(references));
            
            // check for valid handle.
            var source = continuationPoint.NodeToBrowse as ILocalNode;

            if (source == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }
            
            // check for view.
            if (!ViewDescription.IsDefault(continuationPoint.View))
            {
                throw new ServiceResultException(StatusCodes.BadViewIdUnknown);
            }

            lock (Lock)
            {
                // construct list of references.
                var maxResultsToReturn = continuationPoint.MaxResultsToReturn;

                // get previous enumerator.
                var enumerator = continuationPoint.Data as IEnumerator<IReference>;
            
                // fetch a snapshot all references for node.
                if (enumerator == null)
                {
                    var copy = new List<IReference>(source.References);
                    enumerator = copy.GetEnumerator();
                    enumerator.MoveNext();
                }

                do
                {
                    var reference = enumerator.Current;

                    // silently ignore bad values.
                    if (reference == null || NodeId.IsNull(reference.ReferenceTypeId) || NodeId.IsNull(reference.TargetId))
                    {
                        continue;
                    }

                    // apply browse filters.
                    var include = ApplyBrowseFilters(
                        reference,
                        continuationPoint.BrowseDirection,
                        continuationPoint.ReferenceTypeId,
                        continuationPoint.IncludeSubtypes);

                    if (include)
                    {                 
                        var description = new ReferenceDescription();
                        
                        description.NodeId = reference.TargetId;
                        description.SetReferenceType(continuationPoint.ResultMask, reference.ReferenceTypeId, !reference.IsInverse);

                        // only fetch the metadata if it is requested.
                        if (continuationPoint.TargetAttributesRequired)
                        {                        
                            // get the metadata for the node.
                            var metadata = GetNodeMetadata(context, GetManagerHandle(reference.TargetId), continuationPoint.ResultMask);

                            // update description with local node metadata.
                            if (metadata != null)
                            {
                                description.SetTargetAttributes(
                                    continuationPoint.ResultMask,
                                    metadata.NodeClass,
                                    metadata.BrowseName,
                                    metadata.DisplayName,
                                    metadata.TypeDefinition);

                                // check node class mask.
                                if (!CheckNodeClassMask(continuationPoint.NodeClassMask, description.NodeClass))
                                {
                                    continue;
                                }
                            }

                            // any target that is not remote must be owned by another node manager.
                            else if (!reference.TargetId.IsAbsolute)
                            {
                                description.Unfiltered = true;
                            }
                        }

                        // add reference to list.
                        references.Add(description);

                        // construct continuation point if max results reached.
                        if (maxResultsToReturn > 0 && references.Count >= maxResultsToReturn)
                        { 
                            continuationPoint.Index = 0;
                            continuationPoint.Data  = enumerator;
                            enumerator.MoveNext();
                            return;
                        }
                    }
                }
                while (enumerator.MoveNext());
                
                // nothing more to browse if it exits from the loop normally.
                continuationPoint.Dispose();
                continuationPoint = null;
            }
        }
        
        /// <summary>
        /// Returns true is the target meets the filter criteria.
        /// </summary>
        private bool ApplyBrowseFilters(
            IReference      reference,
            BrowseDirection browseDirection,
            NodeId          referenceTypeId,
            bool            includeSubtypes)
        {
            // check browse direction.
            if (reference.IsInverse)
            {
                if (browseDirection == BrowseDirection.Forward)
                {
                    return false;
                }
            }
            else
            {
                if (browseDirection == BrowseDirection.Inverse)
                {
                    return false;
                }
            }

            // check reference type filter.
            if (!NodeId.IsNull(referenceTypeId))
            {
                if (reference.ReferenceTypeId != referenceTypeId)
                {
                    if (includeSubtypes)
                    {
                        if (server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, referenceTypeId))
                        {
                            return true;
                        }
                    }
                        
                    return false;
                }
            }
                   
            // include reference for now.
            return true;
        }

        /// <summary>
        ///     Called when a session is closed.
        /// </summary>
        public virtual void SessionClosing(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions)
        {
        }

        #endregion
        
        /// <see cref="IUaBaseNodeManager.GetNodeMetadata" />
        public UaNodeMetadata GetNodeMetadata(
            UaServerOperationContext context,
            object           targetHandle,
            BrowseResultMask resultMask)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            
            // find target.
            var target = targetHandle as ILocalNode;

            if (target == null)
            {
                return null;
            }

            lock (Lock)
            {
                // copy the default metadata.
                var metadata = new UaNodeMetadata(target, target.NodeId);
                
                // copy target attributes.
                if ((resultMask & BrowseResultMask.NodeClass) != 0)
                {
                    metadata.NodeClass = (NodeClass)target.NodeClass;
                }

                if ((resultMask & BrowseResultMask.BrowseName) != 0)
                {
                    metadata.BrowseName = target.BrowseName;
                }
                
                if ((resultMask & BrowseResultMask.DisplayName) != 0)
                {
                    metadata.DisplayName = target.DisplayName;

                    // check if the display name can be localized.
                    if (!String.IsNullOrEmpty(metadata.DisplayName.Key))
                    {
                        metadata.DisplayName = ServerData.ResourceManager.Translate(context.PreferredLocales, metadata.DisplayName);
                    }
                }
                
                metadata.WriteMask = target.WriteMask;

                if (metadata.WriteMask != AttributeWriteMask.None)
                {
                    var value = new DataValue((uint)(int)target.UserWriteMask);
                    var result = target.Read(context, Attributes.UserWriteMask, value);

                    if (ServiceResult.IsBad(result))
                    {
                        metadata.WriteMask = AttributeWriteMask.None;
                    }
                    else
                    {
                        metadata.WriteMask = (AttributeWriteMask)(int)((uint)(int)metadata.WriteMask & (uint)value.Value);
                    }
                }

                metadata.EventNotifier = EventNotifiers.None;
                metadata.AccessLevel   = AccessLevels.None;
                metadata.Executable    = false;
                
                switch (target.NodeClass)
                {
                    case NodeClass.Object:
                    {
                        metadata.EventNotifier = ((IObject)target).EventNotifier;
                        break;
                    }

                    case NodeClass.View:
                    {
                        metadata.EventNotifier = ((IView)target).EventNotifier;
                        break;
                    }

                    case NodeClass.Variable:
                    {
                        var variable = (IVariable)target;
                        metadata.DataType = variable.DataType;
                        metadata.ValueRank = variable.ValueRank;
                        metadata.ArrayDimensions = variable.ArrayDimensions;                        
                        metadata.AccessLevel = variable.AccessLevel;

                        var value = new DataValue(variable.UserAccessLevel);
                        var result = variable.Read(context, Attributes.UserAccessLevel, value);

                        if (ServiceResult.IsBad(result))
                        {
                            metadata.AccessLevel = 0;
                            break;
                        }
                        
                        metadata.AccessLevel = (byte)(metadata.AccessLevel & (byte)value.Value);
                        break;
                    }

                    case NodeClass.Method:
                    {
                        var method = (IMethod)target;
                        metadata.Executable = method.Executable;

                        if (metadata.Executable)
                        {
                            var value = new DataValue(method.UserExecutable);
                            var result = method.Read(context, Attributes.UserExecutable, value);

                            if (ServiceResult.IsBad(result))
                            {
                                metadata.Executable = false;
                                break;
                            }
                            
                            metadata.Executable = (bool)value.Value;
                        }

                        break;
                    }
                }
                
                // look up type definition.
                if ((resultMask & BrowseResultMask.TypeDefinition) != 0)
                {
                    if (target.NodeClass == NodeClass.Variable || target.NodeClass == NodeClass.Object)
                    {
                        metadata.TypeDefinition = target.TypeDefinitionId;
                    }
                }

                // Set AccessRestrictions and RolePermissions
                var node = (Node)target;
                metadata.AccessRestrictions = (AccessRestrictionType)Enum.Parse(typeof(AccessRestrictionType), node.AccessRestrictions.ToString()); 
                metadata.RolePermissions = node.RolePermissions;
                metadata.UserRolePermissions = node.UserRolePermissions;

                // check if NamespaceMetadata is defined for NamespaceUri
                var namespaceUri = ServerData.NamespaceUris.GetString(target.NodeId.NamespaceIndex);
                var namespaceMetadataState = ServerData.NodeManager.ConfigurationNodeManager.GetNamespaceMetadataState(namespaceUri);
                if (namespaceMetadataState != null)
                {
                    metadata.DefaultAccessRestrictions = (AccessRestrictionType)Enum.ToObject(typeof(AccessRestrictionType),
                        namespaceMetadataState.DefaultAccessRestrictions.Value);
                
                    metadata.DefaultRolePermissions = namespaceMetadataState.DefaultRolePermissions.Value;
                    metadata.DefaultUserRolePermissions = namespaceMetadataState.DefaultUserRolePermissions.Value;
                }

                // return metadata.
                return metadata;
            }
        }

        /// <summary cref="IUaBaseNodeManager.AddReferences" />
        /// <remarks>
        /// This method must not be called without first acquiring 
        /// </remarks>
        public void AddReferences(IDictionary<NodeId,IList<IReference>> references)
        {
            if (references == null) throw new ArgumentNullException(nameof(references));

            lock (Lock)
            {
                var enumerator = references.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var actualNode = GetLocalNode(enumerator.Current.Key) as ILocalNode;

                    if (actualNode != null)
                    {
                        foreach (var reference in enumerator.Current.Value)
                        {
                            AddReference(actualNode, reference.ReferenceTypeId, reference.IsInverse, reference.TargetId);
                        }
                    }                        
                }
            }
        }

        /// <see cref="IUaBaseNodeManager.Read" />
        public void Read(
            UaServerOperationContext     context,
            double               maxAge,
            IList<ReadValueId>   nodesToRead,
            IList<DataValue>     values,
            IList<ServiceResult> errors)
        {
            if (context == null)     throw new ArgumentNullException(nameof(context));
            if (nodesToRead == null) throw new ArgumentNullException(nameof(nodesToRead));
            if (values == null)      throw new ArgumentNullException(nameof(values));
            if (errors == null)      throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToRead.Count; ii++)
                {
                    var nodeToRead = nodesToRead[ii];

                    // skip items that have already been processed.
                    if (nodeToRead.Processed)
                    {
                        continue;
                    }
                    
                    // look up the node.
                    var node = GetLocalNode(nodeToRead.NodeId) as ILocalNode;

                    if (node == null)
                    {
                        continue;
                    }

                    var value = values[ii] = new DataValue();
                    
                    value.Value           = null;
                    value.ServerTimestamp = DateTime.UtcNow;
                    value.SourceTimestamp = DateTime.MinValue;
                    value.StatusCode      = StatusCodes.BadAttributeIdInvalid;

                    // owned by this node manager.
                    nodeToRead.Processed = true;
                    
                    // read the default value (also verifies that the attribute id is valid for the node).                   
                    var error = node.Read(context, nodeToRead.AttributeId, value);

                    if (ServiceResult.IsBad(error))
                    {
                        errors[ii] = error;
                        continue;
                    }                    

                    // always use default value for base attributes.
                    var useDefault = false;

                    switch (nodeToRead.AttributeId)
                    {
                        case Attributes.NodeId:
                        case Attributes.NodeClass:
                        case Attributes.BrowseName:
                        {
                            useDefault = true;
                            break;
                        }
                    }

                    if (useDefault)
                    {
                        errors[ii] = error;
                        continue;
                    }
                      
                    // apply index range to value attributes.
                    if (nodeToRead.AttributeId == Attributes.Value)
                    {
                        var defaultValue = value.Value;

                        error = nodeToRead.ParsedIndexRange.ApplyRange(ref defaultValue);
                    
                        if (ServiceResult.IsBad(error))
                        {
                            value.Value = null;
                            errors[ii] = error;
                            continue;
                        }
                        
                        // apply data encoding.
                        if (!QualifiedName.IsNull(nodeToRead.DataEncoding))
                        {
                            error = EncodeableObject.ApplyDataEncoding(ServerData.MessageContext, nodeToRead.DataEncoding, ref defaultValue);
                                
                            if (ServiceResult.IsBad(error))
                            {
                                value.Value = null;
                                errors[ii] = error;
                                continue;
                            }
                        }
                            
                        value.Value = defaultValue;                     
                        
                        // don't replace timestamp if it was set in the NodeSource 
                        if (value.SourceTimestamp == DateTime.MinValue) 
                        { 
                            value.SourceTimestamp = DateTime.UtcNow; 
                        } 
                    }
                }
            }   
                
        }
 
        /// <see cref="IUaBaseNodeManager.HistoryRead" />
        public void HistoryRead(
            UaServerOperationContext          context,
            HistoryReadDetails        details, 
            TimestampsToReturn        timestampsToReturn, 
            bool                      releaseContinuationPoints, 
            IList<HistoryReadValueId> nodesToRead, 
            IList<HistoryReadResult>  results, 
            IList<ServiceResult>      errors) 
        {
            if (context == null)     throw new ArgumentNullException(nameof(context));
            if (details == null)     throw new ArgumentNullException(nameof(details));
            if (nodesToRead == null) throw new ArgumentNullException(nameof(nodesToRead));
            if (results == null)     throw new ArgumentNullException(nameof(results));
            if (errors == null)      throw new ArgumentNullException(nameof(errors));

            var readRawModifiedDetails = details as ReadRawModifiedDetails;
            var readAtTimeDetails = details as ReadAtTimeDetails;
            var readProcessedDetails = details as ReadProcessedDetails;
            var readEventDetails = details as ReadEventDetails;

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToRead.Count; ii++)
                {
                    var nodeToRead = nodesToRead[ii];

                    // skip items that have already been processed.
                    if (nodeToRead.Processed)
                    {
                        continue;
                    }
                    
                    // look up the node.
                    var node = GetLocalNode(nodeToRead.NodeId) as ILocalNode;

                    if (node == null)
                    {
                        continue;
                    }
                    
                    // owned by this node manager.
                    nodeToRead.Processed = true;

                    errors[ii] = StatusCodes.BadNotReadable;
                }
                
            }
            
        }

        /// <see cref="IUaBaseNodeManager.Write" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public void Write(
            UaServerOperationContext     context,
            IList<WriteValue>    nodesToWrite, 
            IList<ServiceResult> errors)
        {
            if (context == null)      throw new ArgumentNullException(nameof(context));
            if (nodesToWrite == null) throw new ArgumentNullException(nameof(nodesToWrite));
            if (errors == null)       throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToWrite.Count; ii++)
                {
                    var nodeToWrite = nodesToWrite[ii];

                    // skip items that have already been processed.
                    if (nodeToWrite.Processed)
                    {
                        continue;
                    }
                    
                    // look up the node.
                    var node = GetLocalNode(nodeToWrite.NodeId) as ILocalNode;

                    if (node == null)
                    {
                        continue;
                    }
                    
                    // owned by this node manager.
                    nodeToWrite.Processed = true;

                    if (!node.SupportsAttribute(nodeToWrite.AttributeId))
                    {
                        errors[ii] = StatusCodes.BadAttributeIdInvalid;
                        continue;
                    }
                    
                    // fetch the node metadata.
                    var metadata = GetNodeMetadata(context, node, BrowseResultMask.All);
                    
                    // check access.
                    var writeable = true;
                    ServiceResult error = null;

                    // determine access rights.
                    switch (nodeToWrite.AttributeId)
                    {
                        case Attributes.NodeId:
                        case Attributes.NodeClass:
                        case Attributes.AccessLevel:
                        case Attributes.UserAccessLevel:
                        case Attributes.Executable:
                        case Attributes.UserExecutable:
                        case Attributes.EventNotifier:
                        {
                            writeable = false;
                            break;
                        }

                        case Attributes.Value:
                        {
                            writeable = ((metadata.AccessLevel & AccessLevels.CurrentWrite)!= 0);
                            break;
                        }

                        default:
                        {
                            writeable = (metadata.WriteMask & Attributes.GetMask(nodeToWrite.AttributeId)) != 0;
                            break;
                        }
                    }

                    // error if not writeable.
                    if (!writeable)
                    {
                        errors[ii] = StatusCodes.BadNotWritable;
                        continue;
                    }

                    // determine expected datatype and value rank.
                    var expectedDatatypeId = metadata.DataType;
                    var expectedValueRank = metadata.ValueRank;
                    
                    if (nodeToWrite.AttributeId != Attributes.Value)
                    {
                        expectedDatatypeId = Attributes.GetDataTypeId(nodeToWrite.AttributeId);

                        var value = nodeToWrite.Value;

                        if (value.StatusCode != StatusCodes.Good || value.ServerTimestamp != DateTime.MinValue || value.SourceTimestamp != DateTime.MinValue)
                        {
                            errors[ii] = StatusCodes.BadWriteNotSupported;
                            continue;
                        }

                        expectedValueRank = ValueRanks.Scalar;

                        if (nodeToWrite.AttributeId == Attributes.ArrayDimensions)
                        {
                            expectedValueRank = ValueRanks.OneDimension;
                        }
                    }

                    // check whether value being written is an instance of the expected data type.
                    var valueToWrite = nodeToWrite.Value.Value;

                    var typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                        valueToWrite,
                        expectedDatatypeId,
                        expectedValueRank,
                        server_.NamespaceUris,
                        server_.TypeTree);

                    if (typeInfo == null)
                    {
                        errors[ii] = StatusCodes.BadTypeMismatch;
                        continue;
                    }

                    // check index range.
                    if (nodeToWrite.ParsedIndexRange.Count > 0)
                    {                            
                        // check index range for scalars.
                        if (typeInfo.ValueRank < 0)
                        {
                            errors[ii] = StatusCodes.BadIndexRangeInvalid;
                            continue;
                        }
                            
                        // check index range for arrays.
                        else
                        {
                            var array = (Array)valueToWrite;

                            if (nodeToWrite.ParsedIndexRange.Count != array.Length)
                            {
                                errors[ii] = StatusCodes.BadIndexRangeInvalid;
                                continue;
                            }
                        }
                    }
                            
                    // write the default value.
                    error = node.Write(nodeToWrite.AttributeId, nodeToWrite.Value);

                    if (ServiceResult.IsBad(error))
                    {
                        errors[ii] = error;
                        continue;
                    }
                }
            }
        }
               
        /// <see cref="IUaBaseNodeManager.HistoryUpdate" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public void HistoryUpdate(
            UaServerOperationContext            context,
            Type                        detailsType,
            IList<HistoryUpdateDetails> nodesToUpdate, 
            IList<HistoryUpdateResult>  results, 
            IList<ServiceResult>        errors) 
        {
            if (context == null)       throw new ArgumentNullException(nameof(context));
            if (nodesToUpdate == null) throw new ArgumentNullException(nameof(nodesToUpdate));
            if (results == null)       throw new ArgumentNullException(nameof(results));
            if (errors == null)        throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < nodesToUpdate.Count; ii++)
                {
                    var nodeToUpdate = nodesToUpdate[ii];

                    // skip items that have already been processed.
                    if (nodeToUpdate.Processed)
                    {
                        continue;
                    }
                    
                    // look up the node.
                    var node = GetLocalNode(nodeToUpdate.NodeId) as ILocalNode;

                    if (node == null)
                    {
                        continue;
                    }
                    
                    // owned by this node manager.
                    nodeToUpdate.Processed = true;
                    
                        errors[ii] = StatusCodes.BadNotWritable;
                }
            }
                
        }

        /// <see cref="IUaBaseNodeManager.Call" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public void Call(
            UaServerOperationContext         context,
            IList<CallMethodRequest> methodsToCall,
            IList<CallMethodResult>  results,
            IList<ServiceResult>     errors)
        {
            if (context == null)       throw new ArgumentNullException(nameof(context));
            if (methodsToCall == null) throw new ArgumentNullException(nameof(methodsToCall));
            if (results == null)       throw new ArgumentNullException(nameof(results));
            if (errors == null)        throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < methodsToCall.Count; ii++)
                {
                    var methodToCall = methodsToCall[ii];

                    // skip items that have already been processed.
                    if (methodToCall.Processed)
                    {
                        continue;
                    }
                                        
                    // look up the node.
                    var node = GetLocalNode(methodToCall.ObjectId) as ILocalNode;

                    if (node == null)
                    {
                        continue;
                    }
                    
                    methodToCall.Processed = true;                                      
                                        
                    // look up the method.
                    var method = GetLocalNode(methodToCall.MethodId) as ILocalNode;

                    if (method == null)
                    {
                        errors[ii] = ServiceResult.Create(StatusCodes.BadMethodInvalid, "Method is not in the address space.");
                        continue;
                    }

                    // check that the method is defined for the object.
                    if (!node.References.Exists(ReferenceTypeIds.HasComponent, false, methodToCall.MethodId, true, server_.TypeTree))
                    {
                        errors[ii] = ServiceResult.Create(StatusCodes.BadMethodInvalid, "Method is not a component of the Object.");
                        continue;
                    } 
                             
                    errors[ii] = StatusCodes.BadNotImplemented;
                }
            }
                  
        }
        
        /// <see cref="IUaBaseNodeManager.SubscribeToEvents" />
        public ServiceResult SubscribeToEvents(
            UaServerOperationContext    context,
            object              sourceId,
            uint                subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe)
        {
            if (context == null)  throw new ArgumentNullException(nameof(context));
            if (sourceId == null) throw new ArgumentNullException(nameof(sourceId));
            if (monitoredItem == null) throw new ArgumentNullException(nameof(monitoredItem));

            lock (Lock)
            {
                // validate the node.
                var metadata = GetNodeMetadata(context, sourceId, BrowseResultMask.NodeClass);

                if (metadata == null)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                // validate the node class.
                if (((metadata.NodeClass & NodeClass.Object | NodeClass.View)) == 0)
                {
                    return StatusCodes.BadNotSupported;
                }

                // check that it supports events.
                if ((metadata.EventNotifier & EventNotifiers.SubscribeToEvents) == 0)
                {
                    return StatusCodes.BadNotSupported;
                }
                
                return ServiceResult.Good;
            }
        }

        /// <see cref="IUaBaseNodeManager.SubscribeToAllEvents" />
        public ServiceResult SubscribeToAllEvents(
            UaServerOperationContext    context,
            uint                subscriptionId,
            IUaEventMonitoredItem monitoredItem,
            bool                unsubscribe)
        {  
            if (context == null)  throw new ArgumentNullException(nameof(context));
            if (monitoredItem == null)  throw new ArgumentNullException(nameof(monitoredItem));
            
            return ServiceResult.Good;
        }

        /// <see cref="IUaBaseNodeManager.ConditionRefresh" />
        public ServiceResult ConditionRefresh(        
            UaServerOperationContext           context,
            IList<IUaEventMonitoredItem> monitoredItems)
        {            
            if (context == null)  throw new ArgumentNullException(nameof(context));
            
                return ServiceResult.Good;
            }

        /// <summary>
        /// Creates a set of monitored items.
        /// </summary>
        public void CreateMonitoredItems(
            UaServerOperationContext                  context,
            uint                              subscriptionId,
            double                            publishingInterval,
            TimestampsToReturn                timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterErrors,
            IList<IUaMonitoredItem>             monitoredItems,
            ref long                          globalIdCounter)
        {
            if (context == null)         throw new ArgumentNullException(nameof(context));
            if (itemsToCreate == null)   throw new ArgumentNullException(nameof(itemsToCreate));
            if (errors == null)          throw new ArgumentNullException(nameof(errors));
            if (monitoredItems == null)  throw new ArgumentNullException(nameof(monitoredItems));

            lock (Lock)
            {
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    var itemToCreate = itemsToCreate[ii];

                    // skip items that have already been processed.
                    if (itemToCreate.Processed)
                    {
                        continue;
                    }
                    
                    // look up the node.
                    var node = this.GetLocalNode(itemToCreate.ItemToMonitor.NodeId) as ILocalNode;

                    if (node == null)
                    {
                        continue;
                    }

                    // owned by this node manager.
                    itemToCreate.Processed = true;

                    if (!node.SupportsAttribute(itemToCreate.ItemToMonitor.AttributeId))
                    {
                        errors[ii] = StatusCodes.BadAttributeIdInvalid;
                        continue;
                    }

                    // fetch the metadata for the node.
                    var metadata = GetNodeMetadata(context, node, BrowseResultMask.All);

                    if (itemToCreate.ItemToMonitor.AttributeId == Attributes.Value)
                    {
                        if ((metadata.AccessLevel & AccessLevels.CurrentRead) == 0)
                        {
                            errors[ii] = StatusCodes.BadNotReadable;
                            continue;
                        }
                    }

                    // check value rank against index range.
                    if (itemToCreate.ItemToMonitor.ParsedIndexRange != NumericRange.Empty)
                    {
                        var valueRank = metadata.ValueRank;
                        
                        if (itemToCreate.ItemToMonitor.AttributeId != Attributes.Value)
                        {
                            valueRank = Attributes.GetValueRank(itemToCreate.ItemToMonitor.AttributeId);
                        }

                        if (valueRank == ValueRanks.Scalar)
                        {
                            errors[ii] = StatusCodes.BadIndexRangeInvalid;
                            continue;
                        }
                    }

                    var rangeRequired = false;
                                        
                    // validate the filter against the node/attribute being monitored.
                    errors[ii] = ValidateFilter(
                        metadata,
                        itemToCreate.ItemToMonitor.AttributeId,
                        itemToCreate.RequestedParameters.Filter,
                        out rangeRequired);

                    if (ServiceResult.IsBad(errors[ii]))
                    {
                         continue;
                    }

                    // lookup EU range if required.
                    Opc.Ua.Range range = null;

                    if (rangeRequired)
                    {
                        errors[ii] = ReadEURange(context, node, out range);
                                                
                        if (ServiceResult.IsBad(errors[ii]))
                        {
                             continue;
                        }
                    }
                    
                    // create a globally unique identifier.
                    var monitoredItemId = Utils.IncrementIdentifier(ref globalIdCounter);

                    // limit the sampling rate for non-value attributes.
                    var minimumSamplingInterval = defaultMinimumSamplingInterval_;

                    if (itemToCreate.ItemToMonitor.AttributeId == Attributes.Value)
                    {
                        // use the MinimumSamplingInterval attribute to limit the sampling rate for value attributes.
                        var variableNode = node as IVariable;
 
                        if (variableNode != null)
                        {
                            minimumSamplingInterval = variableNode.MinimumSamplingInterval;

                            // use the default if the node does not specify one.
                            if (minimumSamplingInterval < 0)
                            {
                                minimumSamplingInterval = defaultMinimumSamplingInterval_;
                            }
                        }
                    }
                    
                    // create monitored item.
                    var monitoredItem = samplingGroupManager_.CreateMonitoredItem(
                        context,
                        subscriptionId,
                        publishingInterval,
                        timestampsToReturn,
                        monitoredItemId,
                        node,
                        itemToCreate,
                        range,
                        minimumSamplingInterval);

                    // final check for initial value
                    ServiceResult error = ReadInitialValue(context, node, monitoredItem);
                    if (ServiceResult.IsBad(error))
                    {
                        if (error.StatusCode == StatusCodes.BadAttributeIdInvalid ||
                            error.StatusCode == StatusCodes.BadDataEncodingInvalid ||
                            error.StatusCode == StatusCodes.BadDataEncodingUnsupported)
                        {
                            errors[ii] = error;
                            continue;
                        }
                    }

                    // save monitored item.
                    monitoredItems_.Add(monitoredItem.Id, monitoredItem);

                    // update monitored item list.
                    monitoredItems[ii] = monitoredItem;

                    // errors updating the monitoring groups will be reported in notifications.
                    errors[ii] = StatusCodes.Good;
                }
            }
 
            // update all groups with any new items.
            samplingGroupManager_.ApplyChanges();
        }

        /// <summary>
        /// Reads the initial value for a monitored item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node to read.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected virtual ServiceResult ReadInitialValue(
            UaServerOperationContext context,
            ILocalNode node,
            IUaDataChangeMonitoredItem2 monitoredItem)
        {
            DataValue initialValue = new DataValue {
                Value = null,
                ServerTimestamp = DateTime.UtcNow,
                SourceTimestamp = DateTime.MinValue,
                StatusCode = StatusCodes.BadWaitingForInitialData
            };

            ServiceResult error = node.Read(context, monitoredItem.AttributeId, initialValue);

            if (ServiceResult.IsBad(error))
            {
                initialValue.Value = null;
                initialValue.StatusCode = error.StatusCode;
            }

            monitoredItem.QueueValue(initialValue, error, true);

            return error;
        }

        /// <summary>
        /// Modifies a set of monitored items.
        /// </summary>
        public void ModifyMonitoredItems(
            UaServerOperationContext                  context,
            TimestampsToReturn                timestampsToReturn,
            IList<IUaMonitoredItem>             monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterErrors)
        { 
            if (context == null)         throw new ArgumentNullException(nameof(context));
            if (monitoredItems == null)  throw new ArgumentNullException(nameof(monitoredItems));
            if (itemsToModify == null)   throw new ArgumentNullException(nameof(itemsToModify));
            if (errors == null)          throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    var itemToModify = itemsToModify[ii];

                    // skip items that have already been processed.
                    if (itemToModify.Processed || monitoredItems[ii] == null)
                    {
                        continue;
                    }
                    
                    // check if the node manager created the item.                    
                    if (!Object.ReferenceEquals(this, monitoredItems[ii].NodeManager))
                    {
                        continue;
                    }
                                        
                    // owned by this node manager.
                    itemToModify.Processed = true;
                    
                    // validate monitored item.
                    UaMonitoredItem monitoredItem = null;

                    if (!monitoredItems_.TryGetValue(monitoredItems[ii].Id, out monitoredItem))
                    {
                        errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                        continue;
                    }

                    if (!Object.ReferenceEquals(monitoredItem, monitoredItems[ii]))
                    {
                        errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                        continue;
                    }
                    
                    // find the node being monitored.
                    var node = monitoredItem.ManagerHandle as ILocalNode;

                    if (node == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;
                        continue;
                    }

                    // fetch the metadata for the node.
                    var metadata = GetNodeMetadata(context, monitoredItem.ManagerHandle, BrowseResultMask.All);
                                        
                    var rangeRequired = false;

                    // validate the filter against the node/attribute being monitored.
                    errors[ii] = ValidateFilter(
                        metadata,
                        monitoredItem.AttributeId,
                        itemToModify.RequestedParameters.Filter,
                        out rangeRequired);

                    if (ServiceResult.IsBad(errors[ii]))
                    {
                         continue;
                    }

                    // lookup EU range if required.
                    Opc.Ua.Range range = null;

                    if (rangeRequired)
                    {
                        // look up EU range.
                        errors[ii] = ReadEURange(context, node, out range);
                                                
                        if (ServiceResult.IsBad(errors[ii]))
                        {
                             continue;
                        }
                    }

                    // update sampling.
                    errors[ii] = samplingGroupManager_.ModifyMonitoredItem(
                        context,
                        timestampsToReturn,
                        monitoredItem,
                        itemToModify,
                        range);
                    
                    // state of item did not change if an error returned here.
                    if (ServiceResult.IsBad(errors[ii]))
                    {
                        continue;
                    }

                    // item has been modified successfully. 
                    // errors updating the sampling groups will be reported in notifications.
                    errors[ii] = StatusCodes.Good;
                }
            }

            // update all sampling groups.
            samplingGroupManager_.ApplyChanges();
        }

        /// <summary>
        /// Deletes a set of monitored items.
        /// </summary>
        public void DeleteMonitoredItems(
            UaServerOperationContext      context,
            IList<IUaMonitoredItem> monitoredItems, 
            IList<bool>           processedItems,
            IList<ServiceResult>  errors)
        {
            if (context == null)        throw new ArgumentNullException(nameof(context));
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));
            if (errors == null)         throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < errors.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii] || monitoredItems[ii] == null)
                    {
                        continue;
                    }
                    
                    // check if the node manager created the item.                    
                    if (!Object.ReferenceEquals(this, monitoredItems[ii].NodeManager))
                    {
                        continue;
                    }

                    // owned by this node manager.
                    processedItems[ii]  = true;
                    
                    // validate monitored item.
                    UaMonitoredItem monitoredItem = null;

                    if (!monitoredItems_.TryGetValue(monitoredItems[ii].Id, out monitoredItem))
                    {
                        errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                        continue;
                    }

                    if (!Object.ReferenceEquals(monitoredItem, monitoredItems[ii]))
                    {
                        errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                        continue;
                    }

                    // remove item.
                    samplingGroupManager_.StopMonitoring(monitoredItem);

                    // remove association with the group.
                    monitoredItems_.Remove(monitoredItem.Id);

                    // delete successful.
                    errors[ii] = StatusCodes.Good;
                }
            }
 
            // remove all items from groups.
            samplingGroupManager_.ApplyChanges();
        }

        /// <summary>
        /// Transfers a set of monitored items.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sendInitialValues">Whether the subscription should send initial values after transfer.</param>
        /// <param name="monitoredItems">The set of monitoring items to update.</param>
        /// <param name="processedItems">The set of processed items.</param>
        /// <param name="errors">Any errors.</param>
        public virtual void TransferMonitoredItems(
            UaServerOperationContext context,
            bool sendInitialValues,
            IList<IUaMonitoredItem> monitoredItems,
            IList<bool> processedItems,
            IList<ServiceResult> errors)
        {
            if (context == null)        throw new ArgumentNullException(nameof(context));
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));
            if (processedItems == null) throw new ArgumentNullException(nameof(processedItems));

            lock (Lock)
            {
                for (int ii = 0; ii < monitoredItems.Count; ii++)
                {
                    // skip items that have already been processed.
                    if (processedItems[ii] || monitoredItems[ii] == null)
                    {
                        continue;
                    }

                    // check if the node manager created the item.
                    if (!Object.ReferenceEquals(this, monitoredItems[ii].NodeManager))
                    {
                        continue;
                    }

                    // owned by this node manager.
                    processedItems[ii]  = true;

                    // validate monitored item.
                    IUaMonitoredItem monitoredItem = monitoredItems[ii];

                    // find the node being monitored.
                    ILocalNode node = monitoredItem.ManagerHandle as ILocalNode;
                    if (node == null)
                    {
                        continue;
                    }

                    if (sendInitialValues)
                    {
                        monitoredItem.SetupResendDataTrigger();
                    }

                    errors[ii] = StatusCodes.Good;
                }
            }
        }

        /// <summary>
        /// Changes the monitoring mode for a set of monitored items.
        /// </summary>
        public void SetMonitoringMode(
            UaServerOperationContext      context,
            MonitoringMode        monitoringMode,
            IList<IUaMonitoredItem> monitoredItems, 
            IList<bool>           processedItems,
            IList<ServiceResult>  errors)
        {

            if (context == null)        throw new ArgumentNullException(nameof(context));
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));
            if (errors == null)         throw new ArgumentNullException(nameof(errors));

            lock (Lock)
            {
                for (var ii = 0; ii < errors.Count; ii++)
                {                  
                    // skip items that have already been processed.
                    if (processedItems[ii] || monitoredItems[ii] == null)
                    {
                        continue;
                    }
  
                    // check if the node manager created the item.                    
                    if (!Object.ReferenceEquals(this, monitoredItems[ii].NodeManager))
                    {
                        continue;
                    }

                    // owned by this node manager.
                    processedItems[ii]  = true;
                    
                    // validate monitored item.
                    UaMonitoredItem monitoredItem = null;

                    if (!monitoredItems_.TryGetValue(monitoredItems[ii].Id, out monitoredItem))
                    {
                        errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                        continue;
                    }

                    if (!Object.ReferenceEquals(monitoredItem, monitoredItems[ii]))
                    {
                        errors[ii] = StatusCodes.BadMonitoredItemIdInvalid;
                        continue;
                    }

                    // update monitoring mode.
                    var previousMode = monitoredItem.SetMonitoringMode(monitoringMode);

                    // need to provide an immediate update after enabling.
                    if (previousMode == MonitoringMode.Disabled && monitoringMode != MonitoringMode.Disabled)
                    {
                        var initialValue = new DataValue();

                        initialValue.ServerTimestamp = DateTime.UtcNow;
                        initialValue.StatusCode      = StatusCodes.BadWaitingForInitialData;
                        
                        // read the initial value.
                        var node = monitoredItem.ManagerHandle as Node;
                        
                        if (node != null)
                        {
                            var error = node.Read(context, monitoredItem.AttributeId, initialValue);

                            if (ServiceResult.IsBad(error))
                            {
                                initialValue.Value = null;
                                initialValue.StatusCode = error.StatusCode;
                            }
                        }
                                
                        monitoredItem.QueueValue(initialValue, null);
                    }
            
                    // modify the item attributes.   
                    samplingGroupManager_.ModifyMonitoring(context, monitoredItem);
                  
                    // item has been modified successfully. 
                    // errors updating the sampling groups will be reported in notifications.
                    errors[ii] = StatusCodes.Good;
                }
            }
   
            // update all sampling groups.
            samplingGroupManager_.ApplyChanges();
        }
        #endregion
        
        #region Static Members
        /// <summary>
        /// Returns true if the node class matches the node class mask.
        /// </summary>
        public static bool CheckNodeClassMask(uint nodeClassMask, NodeClass nodeClass)
        {
            if (nodeClassMask != 0)
            {
                return ((uint)nodeClass & nodeClassMask) != 0;
            }

            return true;
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// The server that the node manager belongs to.
        /// </summary>
        public IUaServerData ServerData
        {
            get { return server_; }
        }
        #endregion
     
        #region Browsing/Searching
        /// <summary>
        /// Returns an index for the NamespaceURI (Adds it to the server namespace table if it does not already exist).
        /// </summary>
        /// <remarks>
        /// Returns the server's default index (1) if the namespaceUri is empty or null. 
        /// </remarks>
        public ushort GetNamespaceIndex(string namespaceUri)
        {
            var namespaceIndex = 1;

            if (!String.IsNullOrEmpty(namespaceUri))
            {
                namespaceIndex = server_.NamespaceUris.GetIndex(namespaceUri);

                if (namespaceIndex == -1)
                {
                    namespaceIndex = server_.NamespaceUris.Append(namespaceUri);
                }
            }

            return (ushort)namespaceIndex;
        }
    
        /// <summary>
        /// Returns all targets of the specified reference.
        /// </summary>
        public NodeIdCollection FindLocalNodes(NodeId sourceId, NodeId referenceTypeId, bool isInverse)
        {
            if (sourceId == null)        throw new ArgumentNullException(nameof(sourceId));
            if (referenceTypeId == null) throw new ArgumentNullException(nameof(referenceTypeId));

            lock (Lock)
            {
                var source = GetManagerHandle(sourceId) as ILocalNode;

                if (source == null)
                {
                    return null;
                }

                var targets = new NodeIdCollection();
                
                foreach (var reference in source.References)
                {
                    if (reference.IsInverse != isInverse || !server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, referenceTypeId))
                    {
                        continue;
                    }

                    var targetId = reference.TargetId;

                    if (targetId.IsAbsolute)
                    {
                        continue;
                    }

                    targets.Add((NodeId)targetId);
                }
                    
                return targets;
            }
        }

        /// <summary>
        /// Returns the id the first node with the specified browse name if it exists. null otherwise
        /// </summary>
        public NodeId FindTargetId(NodeId sourceId, NodeId referenceTypeId, bool isInverse, QualifiedName browseName)
        {
            if (sourceId == null)        throw new ArgumentNullException(nameof(sourceId));
            if (referenceTypeId == null) throw new ArgumentNullException(nameof(referenceTypeId));

            lock (Lock)
            {
                var source = GetManagerHandle(sourceId) as ILocalNode;

                if (source == null)
                {
                    return null;
                }
                
                foreach (ReferenceNode reference in source.References)
                {
                    if (reference.IsInverse != isInverse || !server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, referenceTypeId))
                    {
                        continue;
                    }

                    var targetId = reference.TargetId;

                    if (targetId.IsAbsolute)
                    {
                        continue;
                    }

                    var target = GetManagerHandle((NodeId)targetId) as ILocalNode;

                    if (target == null)
                    {
                        continue;
                    }
                    
                    if (QualifiedName.IsNull(browseName) || target.BrowseName == browseName)
                    {
                        return (NodeId)targetId;
                    }
                }
                    
                return null;
            }
        }            

        /// <summary>
        /// Returns the first target that matches the browse path.
        /// </summary>
        public NodeId Find(NodeId sourceId, string browsePath)
        {
            var targets = TranslateBrowsePath(sourceId, browsePath);

            if (targets.Count > 0)
            {
                return targets[0];
            }

            return null;
        }

        /// <summary>
        /// Returns a list of targets the match the browse path.
        /// </summary>
        public IList<NodeId> TranslateBrowsePath(
            UaServerOperationContext context, 
            NodeId           sourceId, 
            string           browsePath)
        {
            return TranslateBrowsePath(context, sourceId, RelativePath.Parse(browsePath, server_.TypeTree));
        }
             
        /// <summary>
        /// Returns a list of targets the match the browse path.
        /// </summary>
        public IList<NodeId> TranslateBrowsePath(
            NodeId sourceId, 
            string browsePath)
        {
            return TranslateBrowsePath(null, sourceId, RelativePath.Parse(browsePath, server_.TypeTree));
        }
             
        /// <summary>
        /// Returns a list of targets the match the browse path.
        /// </summary>
        public IList<NodeId> TranslateBrowsePath(
            NodeId       sourceId, 
            RelativePath relativePath)
        {
            return TranslateBrowsePath(null, sourceId, relativePath);
        }
             
        /// <summary>
        /// Returns a list of targets the match the browse path.
        /// </summary>
        public IList<NodeId> TranslateBrowsePath(
            UaServerOperationContext context, 
            NodeId           sourceId, 
            RelativePath     relativePath)
        {
            var targets = new List<NodeId>();

            if (relativePath == null || relativePath.Elements.Count == 0)
            {
                targets.Add(sourceId);
                return targets;
            }

            // look up source in this node manager.
            ILocalNode source = null;

            lock (Lock)
            {
                source = GetLocalNode(sourceId) as ILocalNode;

                if (source == null)
                {
                    return targets;
                }
            }

            // return the set of matching targets.
            return targets;
        }
        #endregion

        #region Registering Data/Event Sources
        /// <summary>
        /// Registers a source for a node.
        /// </summary>
        /// <remarks>
        /// The source could be one or more of IDataSource, IEventSource, ICallable, IHistorian or IViewManager
        /// </remarks>
        public void RegisterSource(NodeId nodeId, object source, object handle, bool isEventSource)
        {
            if (nodeId == null) throw new ArgumentNullException(nameof(nodeId));
            if (source == null) throw new ArgumentNullException(nameof(source));
        }   

        /// <summary>
        /// Called when the source is no longer used.
        /// </summary>
        /// <remarks>
        /// When a source disappears it must either delete all of its nodes from the address space
        /// or unregister itself their source by calling RegisterSource with source == null.
        /// After doing that the source must call this method.
        /// </remarks>
        public void UnregisterSource(object source)
        {
        }
        #endregion

        #region Adding/Removing Nodes
        
        #region Apply Modelling Rules
        /// <summary>
        /// Applys the modelling rules to any existing instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public void ApplyModellingRules(
            ILocalNode instance, 
            ILocalNode typeDefinition, 
            ILocalNode templateDeclaration, 
            ushort     namespaceIndex)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (typeDefinition == null) throw new ArgumentNullException(nameof(typeDefinition));

            // check existing type definition.
            UpdateTypeDefinition(instance, typeDefinition.NodeId);

            // create list of declarations for the type definition (recursively collects definitions from supertypes).
            var declarations = new List<DeclarationNode>();
            BuildDeclarationList(typeDefinition, declarations);
            
            // add instance declaration if provided.
            if (templateDeclaration != null)
            {
                var declaration = new DeclarationNode();

                declaration.Node = templateDeclaration;
                declaration.BrowsePath = String.Empty;

                declarations.Add(declaration);

                BuildDeclarationList(templateDeclaration, declarations);
            }

            // build list of instances to create.
            var typeDefinitions = new List<ILocalNode>();
            var instanceDeclarations = new SortedDictionary<string,ILocalNode>();
            var possibleTargets = new SortedDictionary<NodeId,ILocalNode>();
            
            // create instances from declarations.
            // subtypes appear in list last so traversing the list backwards find the overridden nodes first.
            for (var ii = declarations.Count-1; ii >= 0; ii--)
            {
                var declaration = declarations[ii];
                
                // update type definition list.
                if (String.IsNullOrEmpty(declaration.BrowsePath))
                {
                    typeDefinitions.Add(declaration.Node);
                    continue;
                }

                // skip declaration if instance already exists.
                // (i.e. the declaration was overridden).
                if (instanceDeclarations.ContainsKey(declaration.BrowsePath))
                {
                    continue;
                }
                
                // update instance declaration list.
                instanceDeclarations[declaration.BrowsePath] = declaration.Node;
                                        
                // save the node as a possible target of references.
                possibleTargets[declaration.Node.NodeId] = declaration.Node; 
            }
            
            // build list of instances that already exist.
            var existingInstances = new SortedDictionary<string,ILocalNode>();
            BuildInstanceList(instance, String.Empty, existingInstances);

            // maps the instance declaration onto an instance node.
            var instancesToCreate = new Dictionary<NodeId,ILocalNode>(); 
            
            // apply modelling rules to instance declarations.
            foreach (var current in instanceDeclarations)
            {
                var browsePath = current.Key;
                var instanceDeclaration = current.Value;

                // check if the same instance has multiple browse paths to it.
                ILocalNode newInstance = null;
                
                if (instancesToCreate.TryGetValue(instanceDeclaration.NodeId, out newInstance))
                {   
                    continue;
                }
 
                // check for an existing instance.
                if (existingInstances.TryGetValue(browsePath, out newInstance))
                {
                    continue;
                }

                // apply modelling rule to determine whether to create a new instance.
                var modellingRule = instanceDeclaration.ModellingRule;
                
                // always create a new instance if one does not already exist.
                if (modellingRule == Objects.ModellingRule_Mandatory)
                {               
                    if (newInstance == null)
                    {
                        newInstance = instanceDeclaration.CreateCopy(CreateUniqueNodeId());
                        AddNode(newInstance);
                    }
                }

                // ignore optional instances unless one has been specified in the existing tree.
                else if (modellingRule == Objects.ModellingRule_Optional)
                {                            
                    if (newInstance == null)
                    {
                        continue;
                    }
                }

                // ignore any unknown modelling rules.
                else
                {
                    continue;
                }

                // save the mapping between the instance declaration and the new instance.
                instancesToCreate[instanceDeclaration.NodeId] = newInstance;
            }
            
            // add references from type definitions to top level.
            foreach (var type in typeDefinitions)
            {
                foreach (var reference in type.References)
                {
                    // ignore external references from type.
                    if (reference.TargetId.IsAbsolute)
                    {
                        continue;
                    }
                    
                    // ignore subtype references.
                    if (nodes_.TypeTree.IsTypeOf(reference.ReferenceTypeId, ReferenceTypeIds.HasSubtype))
                    {
                        continue;                            
                    }

                    // ignore targets that are not in the instance tree.
                    ILocalNode target = null;

                    if (!instancesToCreate.TryGetValue((NodeId)reference.TargetId, out target))
                    {
                        continue;
                    }

                    // add forward and backward reference.
                    AddReference(instance, reference.ReferenceTypeId, reference.IsInverse, target, true);
                }                  
            }
           
            // add references between instance declarations.
            foreach (var instanceDeclaration in instanceDeclarations.Values)
            {
                // find the source for the references.
                ILocalNode source = null;

                if (!instancesToCreate.TryGetValue(instanceDeclaration.NodeId, out source))
                {
                    continue;
                }
                
                // check if the source is a shared node.
                var sharedNode = Object.ReferenceEquals(instanceDeclaration, source);

                foreach (var reference in instanceDeclaration.References)
                {
                    // add external reference.
                    if (reference.TargetId.IsAbsolute)
                    {
                        if (!sharedNode)
                        {
                            AddReference(source, reference.ReferenceTypeId, reference.IsInverse, reference.TargetId);
                        }

                        continue;
                    }

                    // check for modelling rule.
                    if (reference.ReferenceTypeId == ReferenceTypeIds.HasModellingRule)
                    {
                        if (!source.References.Exists(ReferenceTypeIds.HasModellingRule, false, reference.TargetId, false, null))
                        {
                            AddReference(source, reference.ReferenceTypeId, false, reference.TargetId);
                        }

                        continue;                            
                    }

                    // check for type definition.
                    if (reference.ReferenceTypeId == ReferenceTypeIds.HasTypeDefinition)
                    {
                        if (!sharedNode)
                        {                        
                            UpdateTypeDefinition(source, instanceDeclaration.TypeDefinitionId);
                        }

                        continue;                            
                    }

                    // add targets that are not in the instance tree.
                    ILocalNode target = null;

                    if (!instancesToCreate.TryGetValue((NodeId)reference.TargetId, out target))
                    {
                        // don't update shared nodes because the reference should already exist.
                        if (sharedNode)
                        {
                            continue;
                        }

                        // top level references to the type definition node were already added.
                        if (reference.TargetId == typeDefinition.NodeId)
                        {
                            continue;
                        }

                        // see if a reference is allowed.
                        if (!IsExternalReferenceAllowed(reference.ReferenceTypeId))
                        {
                            continue;
                        }

                        // add one way reference.
                        source.References.Add(reference.ReferenceTypeId, reference.IsInverse, reference.TargetId);
                        continue;
                    }
                                                                    
                    // add forward and backward reference.
                    AddReference(source, reference.ReferenceTypeId, reference.IsInverse, target, true);
                }
            }
        }        

        /// <summary>
        /// Returns true if a one-way reference to external nodes is permitted.
        /// </summary>
        private bool IsExternalReferenceAllowed(NodeId referenceTypeId)
        {                        
            // always exclude hierarchial references.
            if (nodes_.TypeTree.IsTypeOf(referenceTypeId, ReferenceTypeIds.HierarchicalReferences))
            {
                return false;
            }

            // allow one way reference to event.
            if (nodes_.TypeTree.IsTypeOf(referenceTypeId, ReferenceTypes.GeneratesEvent))
            {
                return true;
            }

            // all other references not permitted.
            return false;
        }

        /// <summary>
        /// Updates the type definition for a node.
        /// </summary>
        private void UpdateTypeDefinition(ILocalNode instance, ExpandedNodeId typeDefinitionId)
        {
            // check existing type definition.
            var existingTypeId = instance.TypeDefinitionId;

            if (existingTypeId == typeDefinitionId)
            {
                return;
            }

            if (!NodeId.IsNull(existingTypeId))
            {
                if (nodes_.TypeTree.IsTypeOf(existingTypeId, typeDefinitionId))
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadTypeDefinitionInvalid,
                        "Type definition {0} is not a subtype of the existing type definition {1}.",
                        existingTypeId,
                        typeDefinitionId);
                }

                DeleteReference(instance, ReferenceTypeIds.HasTypeDefinition, false, existingTypeId, false);
            }

            AddReference(instance, ReferenceTypeIds.HasTypeDefinition, false, typeDefinitionId);
        }

        /// <summary>
        /// A node in the type system that is used to instantiate objects or variables.
        /// </summary>
        private class DeclarationNode
        {
            public ILocalNode Node;
            public string BrowsePath;
        }
        
        /// <summary>
        /// Builds the list of declaration nodes for a type definition. 
        /// </summary>
        private void BuildDeclarationList(ILocalNode typeDefinition, List<DeclarationNode> declarations)
        {
            if (typeDefinition == null) throw new ArgumentNullException(nameof(typeDefinition));
            if (declarations == null) throw new ArgumentNullException(nameof(declarations));

            // guard against loops (i.e. common grandparents).
            for (var ii = 0; ii < declarations.Count; ii++)
            {
                if (Object.ReferenceEquals(declarations[ii].Node, typeDefinition))
                {
                    return;
                }
            }

            // create the root declaration for the type.
            var declaration = new DeclarationNode();

            declaration.Node = typeDefinition;
            declaration.BrowsePath = String.Empty;

            declarations.Add(declaration);

            // follow references to supertypes first.
            foreach (var reference in typeDefinition.References.Find(ReferenceTypeIds.HasSubtype, true, false, null))
            {
                var supertype = GetLocalNode(reference.TargetId) as ILocalNode;

                if (supertype == null)
                {
                    continue;
                }

                BuildDeclarationList(supertype, declarations);
            }

            // add children of type.
            BuildDeclarationList(declaration, declarations);            
        }

        /// <summary>
        /// Builds a list of declarations from the nodes aggregated by a parent.
        /// </summary>
        private void BuildDeclarationList(DeclarationNode parent, List<DeclarationNode> declarations)
        {            
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (declarations == null) throw new ArgumentNullException(nameof(declarations));

            // get list of children.
            var references = parent.Node.References.Find(ReferenceTypeIds.HierarchicalReferences, false, true, nodes_.TypeTree);

            foreach (var reference in references)
            {
                // do not follow sub-type references.
                if (nodes_.TypeTree.IsTypeOf(reference.ReferenceTypeId, ReferenceTypeIds.HasSubtype))
                {
                    continue;
                }

                // find child (ignore children that are not in the node table).
                var child = GetLocalNode(reference.TargetId) as ILocalNode;

                if (child == null)
                {
                    continue;
                }

                // create the declartion node.
                var declaration = new DeclarationNode();

                declaration.Node = child;
                declaration.BrowsePath = Utils.Format("{0}.{1}", parent.BrowsePath, child.BrowseName);

                declarations.Add(declaration);

                // recursively include aggregated children.
                var modellingRule = child.ModellingRule;

                if (modellingRule == Objects.ModellingRule_Mandatory || modellingRule == Objects.ModellingRule_Optional)
                {
                    BuildDeclarationList(declaration, declarations);
                }
            }
        }
        
        /// <summary>
        /// Builds a table of instances indexed by browse path from the nodes aggregated by a parent
        /// </summary>
        private void BuildInstanceList(ILocalNode parent, string browsePath, IDictionary<string,ILocalNode> instances)
        { 
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (instances == null) throw new ArgumentNullException(nameof(instances));
            
            // guard against loops.
            if (instances.ContainsKey(browsePath))
            {
                return;
            }

            // index parent by browse path.    
            instances[browsePath] = parent;

            // get list of children.
            var references = parent.References.Find(ReferenceTypeIds.HierarchicalReferences, false, true, nodes_.TypeTree);

            foreach (var reference in references)
            {
                // find child (ignore children that are not in the node table).
                var child = GetLocalNode(reference.TargetId) as ILocalNode;

                if (child == null)
                {
                    continue;
                }
                
                // recursively include aggregated children.
                BuildInstanceList(child, Utils.Format("{0}.{1}", browsePath, child.BrowseName), instances);
            }
        }
        #endregion

        /// <summary>
        /// Exports a node to a nodeset.
        /// </summary>
        public void ExportNode(NodeId nodeId, NodeSet nodeSet)
        {
            lock (Lock)
            {
                var node = GetLocalNode(nodeId) as ILocalNode;

                if (node == null)
                {
                    throw ServiceResultException.Create(StatusCodes.BadNodeIdUnknown, "NodeId ({0}) does not exist.", nodeId);
                }

                ExportNode(node, nodeSet, (node.NodeClass & (NodeClass.Object | NodeClass.Variable)) != 0);
            }
        }

        /// <summary>
        /// Exports a node to a nodeset.
        /// </summary>
        public void ExportNode(ILocalNode node, NodeSet nodeSet, bool instance)
        {
            lock (Lock)
            {
                // check if the node has already been added.
                var exportedId = nodeSet.Export(node.NodeId, nodes_.NamespaceUris);

                if (nodeSet.Contains(exportedId))
                {
                    return;
                }

                // add to nodeset.
                var nodeToExport = nodeSet.Add(node, nodes_.NamespaceUris, nodes_.ServerUris);

                // follow children.
                foreach (ReferenceNode reference in node.References)
                {
                    // export all references.
                    var export = true;

                    // unless it is a subtype reference.
                    if (server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, ReferenceTypeIds.HasSubtype))
                    {
                        export = false;
                    }
       
                    if (export)
                    {
                        nodeSet.AddReference(nodeToExport, reference, nodes_.NamespaceUris, nodes_.ServerUris);
                    }
                    
                    if (reference.IsInverse || server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, ReferenceTypeIds.HasSubtype))
                    {
                        nodeSet.AddReference(nodeToExport, reference, nodes_.NamespaceUris, nodes_.ServerUris);
                    }

                    if (server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, ReferenceTypeIds.Aggregates))
                    {
                        if (reference.IsInverse)
                        {
                            continue;
                        }
                        
                        var child = GetLocalNode(reference.TargetId) as ILocalNode;

                        if (child != null)
                        {
                            if (instance)
                            {
                                var modellingRule = child.ModellingRule;

                                if (modellingRule != Objects.ModellingRule_Mandatory)
                                {
                                    continue;
                                }
                            }

                            ExportNode(child, nodeSet, instance);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the attributes for the node.
        /// </summary>
        private static void UpdateAttributes(ILocalNode node, NodeAttributes attributes)
        {
            // DisplayName
            if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.DisplayName) != 0)
            {
                node.DisplayName = attributes.DisplayName;
            
                if (node.DisplayName == null)
                {
                    node.DisplayName = new LocalizedText(node.BrowseName.Name);
                }
            }
            else
            {
                node.DisplayName = new LocalizedText(node.BrowseName.Name);
            }
            
            // Description
            if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.Description) != 0)
            {
                node.Description = attributes.Description;
            }
                     
            // WriteMask
            if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.WriteMask) != 0)
            {
                node.WriteMask = (AttributeWriteMask)attributes.WriteMask;
            }
            else
            {
                node.WriteMask = AttributeWriteMask.None;
            }
                    
            // WriteMask    
            if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.UserWriteMask) != 0)
            {
                node.UserWriteMask = (AttributeWriteMask)attributes.UserWriteMask;
            }
            else
            {
                node.UserWriteMask = AttributeWriteMask.None;
            }
        }
        
        /// <summary>
        /// Deletes a node from the address sapce.
        /// </summary>
        public void DeleteNode(NodeId nodeId, bool deleteChildren, bool silent)
        {
            if (nodeId == null) throw new ArgumentNullException(nameof(nodeId));
            
            // find the node to delete.
            var node = GetManagerHandle(nodeId) as ILocalNode;

            if (node == null)
            {
                if (!silent)
                {
                    throw ServiceResultException.Create(StatusCodes.BadSourceNodeIdInvalid, "Node '{0}' does not exist.", nodeId);
                }

                return;
            }

            var instance = (node.NodeClass & (NodeClass.Object | NodeClass.Variable)) != 0;

            var referencesToDelete = new Dictionary<NodeId,IList<IReference>>();
            
            if (silent)
            {
                try
                {
                    DeleteNode(node, deleteChildren, instance, referencesToDelete);
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error deleting node: {0}", nodeId);
                }
            }
            else
            {
                DeleteNode(node, deleteChildren, instance, referencesToDelete);
            }

            if (referencesToDelete.Count > 0)
            {
                Task.Run(() =>
                {
                    OnDeleteReferences(referencesToDelete);
                });
            }
        }       

        /// <summary>
        /// Deletes a node from the address sapce.
        /// </summary>
        private void DeleteNode(ILocalNode node, bool deleteChildren, bool instance, Dictionary<NodeId,IList<IReference>> referencesToDelete)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var nodesToDelete = new List<ILocalNode>();
            var referencesForNode = new List<IReference>();

            lock (Lock)
            {
                // remove the node.
                nodes_.Remove(node.NodeId);
                    
                // check need to connect subtypes to the supertype if they are being deleted.
                ExpandedNodeId supertypeId = server_.TypeTree.FindSuperType(node.NodeId);
                
                if (!NodeId.IsNull(supertypeId))
                {
                    server_.TypeTree.Remove(node.NodeId);
                }

                // remove any references to the node.
                foreach (var reference in node.References)
                {
                    // ignore remote references.
                    if (reference.TargetId.IsAbsolute)
                    {
                        continue;
                    }

                    // find the target.
                    var target = GetManagerHandle(reference.TargetId) as ILocalNode;

                    if (target == null)
                    {
                        referencesForNode.Add(reference);
                        continue;
                    }
                    
                    // delete the backward reference.
                    target.References.Remove(reference.ReferenceTypeId, !reference.IsInverse, node.NodeId);

                    // check for children that need to be deleted.
                    if (deleteChildren)
                    {
                        if (server_.TypeTree.IsTypeOf(reference.ReferenceTypeId, ReferenceTypeIds.Aggregates) && !reference.IsInverse)
                        {
                            nodesToDelete.Add(target);
                        }
                    }
                }

                if (referencesForNode.Count > 0)
                {
                    referencesToDelete[node.NodeId] = referencesForNode;
                }
            }

            // delete the child nodes.
            foreach (var nodeToDelete in nodesToDelete)
            {
                DeleteNode(nodeToDelete, deleteChildren, instance, referencesToDelete);
            }
        }         

        /// <summary>
        /// Deletes the external references to a node in a background thread.
        /// </summary>
        private void OnDeleteReferences(object state)
        {            
            var referencesToDelete = state as Dictionary<NodeId,IList<IReference>>;

            if (state == null)
            {
                return;
            }
            
            foreach (var current in referencesToDelete)
            {
                try
                {
                    server_.NodeManager.DeleteReferences(current.Key, current.Value);
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error deleting references for node: {0}", current.Key);
                }
            }            
        }

        #region Add/Remove Node Support Functions
        /// <summary>
        /// Verifies that the source and the target meet the restrictions imposed by the reference type.
        /// </summary>
        private void ValidateReference(
            ILocalNode source,
            NodeId     referenceTypeId,
            bool       isInverse,
            NodeClass  targetNodeClass)
        {
            // find reference type.
            var referenceType = GetLocalNode(referenceTypeId) as IReferenceType;

            if (referenceType == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadReferenceTypeIdInvalid, "Reference type '{0}' does not exist.", referenceTypeId);
            }

            // swap the source and target for inverse references.
            var sourceNodeClass = source.NodeClass;

            if (isInverse)
            {
                sourceNodeClass = targetNodeClass;
                targetNodeClass = source.NodeClass;
            }

            // check HasComponent references.
            if (server_.TypeTree.IsTypeOf(referenceTypeId, ReferenceTypeIds.HasComponent))
            {
                if ((sourceNodeClass & (NodeClass.Object | NodeClass.Variable | NodeClass.ObjectType | NodeClass.VariableType)) == 0)
                {
                    throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "Source node cannot be used with HasComponent references.");
                }

                if ((targetNodeClass & (NodeClass.Object | NodeClass.Variable | NodeClass.Method)) == 0)
                {
                    throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "Target node cannot be used with HasComponent references.");
                }
                
                if (targetNodeClass == NodeClass.Variable)
                {
                    if ((targetNodeClass & (NodeClass.Variable | NodeClass.VariableType)) == 0)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "A Variable must be a component of an Variable or VariableType.");
                    }
                }

                if (targetNodeClass == NodeClass.Method)
                {
                    if ((sourceNodeClass & (NodeClass.Object | NodeClass.ObjectType)) == 0)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "A Method must be a component of an Object or ObjectType.");
                    }
                }
            }
            
            // check HasProperty references.
            if (server_.TypeTree.IsTypeOf(referenceTypeId, ReferenceTypes.HasProperty))
            {
                if (targetNodeClass != NodeClass.Variable)
                {
                    throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "Targets of HasProperty references must be Variables.");
                }                
            }

            // check HasSubtype references.
            if (server_.TypeTree.IsTypeOf(referenceTypeId, ReferenceTypeIds.HasSubtype))
            {
                if ((sourceNodeClass & (NodeClass.DataType | NodeClass.ReferenceType | NodeClass.ObjectType | NodeClass.VariableType)) == 0)
                {
                    throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "Source node cannot be used with HasSubtype references.");
                }

                if (targetNodeClass != sourceNodeClass)
                {
                    throw ServiceResultException.Create(StatusCodes.BadReferenceNotAllowed, "The source and target cannot be connected by a HasSubtype reference.");
                }
            }                      

            // TBD - check rules for other reference types.
        }

        /// <summary>
        /// Validates Role permissions for the specified NodeId
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="nodeId"></param>
        /// <param name="requestedPermission"></param>
        /// <returns></returns>
        public ServiceResult ValidateRolePermissions(UaServerOperationContext operationContext, NodeId nodeId, PermissionType requestedPermission)
        {
            if (operationContext.Session == null || requestedPermission == PermissionType.None)
            {
                // no permission is required hence the validation passes.
                return StatusCodes.Good;
            }

            IUaBaseNodeManager nodeManager = null;
            var nodeHandle = ServerData.NodeManager.GetManagerHandle(nodeId, out nodeManager);
            if (nodeHandle == null || nodeManager == null)
            {
                // ignore unknown nodes.
                return StatusCodes.Good;
            }

            var nodeMetadata = nodeManager.GetNodeMetadata(operationContext, nodeHandle, BrowseResultMask.All);

            return MasterNodeManager.ValidateRolePermissions(operationContext, nodeMetadata, requestedPermission);
        }

        #endregion
        #endregion
        
        #region Adding/Removing References
        /// <summary>
        /// Adds a reference between two existing nodes.
        /// </summary>
        public ServiceResult AddReference(
            NodeId sourceId,
            NodeId referenceTypeId,
            bool   isInverse,
            NodeId targetId,
            bool   bidirectional)
        {
            lock (Lock)
            {
                // find source.
                var source = GetManagerHandle(sourceId) as ILocalNode;

                if (source == null)
                {
                    return StatusCodes.BadParentNodeIdInvalid;
                }
                                
                // add reference from target to source.      
                if (bidirectional)
                {              
                    // find target.
                    var target = GetManagerHandle(targetId) as ILocalNode;

                    if (target == null)
                    {
                        return StatusCodes.BadNodeIdUnknown;
                    }
                 
                    // ensure the reference is valid.
                    ValidateReference(source, referenceTypeId, isInverse, target.NodeClass);

                    // add reference from target to source.
                    AddReferenceToLocalNode(target, referenceTypeId, !isInverse, sourceId, false);
                }
                
                // add reference from source to target.                
                AddReferenceToLocalNode(source, referenceTypeId, isInverse, targetId, false);

                return null;
            }
        }

        /// <summary>
        /// Ensures any changes to built-in nodes are reflected in the diagnostics node manager.
        /// </summary>
        private void AddReferenceToLocalNode(
            ILocalNode     source,
            NodeId         referenceTypeId,
            bool           isInverse,
            ExpandedNodeId targetId,
            bool isInternal)
        {
            source.References.Add(referenceTypeId, isInverse, targetId);

            if (!isInternal && source.NodeId.NamespaceIndex == 0)
            {
                lock (ServerData.DiagnosticsNodeManager.Lock)
                {
                    var state = ServerData.DiagnosticsNodeManager.FindPredefinedNode(source.NodeId, null);

                    if (state != null)
                    {
                        var browser = state.CreateBrowser(
                            server_.DefaultSystemContext,
                            null,
                            referenceTypeId,
                            true,
                            (isInverse) ? BrowseDirection.Inverse : BrowseDirection.Forward,
                            null,
                            null,
                            true);

                        var found = false;

                        for (var reference = browser.Next(); reference != null; reference = browser.Next())
                        {
                            if (reference.TargetId == targetId)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            state.AddReference(referenceTypeId, isInverse, targetId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a reference between two existing nodes.
        /// </summary>
        public void CreateReference(
            NodeId       sourceId,
            NodeId       referenceTypeId,
            bool         isInverse,
            NodeId       targetId,
            bool         bidirectional)
        {
            lock (Lock)
            {
                var result = AddReference(sourceId, referenceTypeId, isInverse, targetId, bidirectional);

                if (ServiceResult.IsBad(result))
                {
                    throw new ServiceResultException(result);
                }
            }
        }

        /// <summary>
        /// Adds a reference to the address space.
        /// </summary>
        private void AddReference(
            ILocalNode source, 
            NodeId     referenceTypeId, 
            bool       isInverse, 
            ILocalNode target, 
            bool       bidirectional)
        {
            AddReferenceToLocalNode(source, referenceTypeId, isInverse, target.NodeId, false);
            
            if (bidirectional)
            {
                AddReferenceToLocalNode(target, referenceTypeId, !isInverse, source.NodeId, false);
            }            
        }        

        /// <summary>
        /// Adds a reference to the address space.
        /// </summary>
        private void AddReference(
            ILocalNode     source, 
            NodeId         referenceTypeId, 
            bool           isInverse, 
            ExpandedNodeId targetId)
        {
            AddReferenceToLocalNode(source, referenceTypeId, isInverse, targetId, false);
        }

        /// <summary>
        /// Deletes a reference.
        /// </summary>
        public ServiceResult DeleteReference(
            object         sourceHandle, 
            NodeId         referenceTypeId,
            bool           isInverse, 
            ExpandedNodeId targetId, 
            bool           deleteBidirectional)
        {
            if (sourceHandle == null)    throw new ArgumentNullException(nameof(sourceHandle));
            if (referenceTypeId == null) throw new ArgumentNullException(nameof(referenceTypeId));
            if (targetId == null)        throw new ArgumentNullException(nameof(targetId));

            lock (Lock)
            {
                var source = sourceHandle as ILocalNode;

                if (source == null)
                {
                    return StatusCodes.BadSourceNodeIdInvalid;
                }

                source.References.Remove(referenceTypeId, isInverse, targetId);

                if (deleteBidirectional)
                {
                    var target = GetManagerHandle(targetId) as ILocalNode;

                    if (target != null)
                    {
                        target.References.Remove(referenceTypeId, !isInverse, source.NodeId);
                    }
                }
                   
                return ServiceResult.Good;
            }
        }
        
        /// <summary>
        /// Deletes a reference.
        /// </summary>
        public void DeleteReference(
            NodeId sourceId, 
            NodeId referenceTypeId, 
            bool isInverse, 
            ExpandedNodeId targetId, 
            bool deleteBidirectional)
        { 
            var result = DeleteReference(
                GetManagerHandle(sourceId) as ILocalNode, 
                referenceTypeId, 
                isInverse, 
                targetId, 
                deleteBidirectional);

            if (ServiceResult.IsBad(result))
            {
                throw new ServiceResultException(result);
            }
        }

        /// <summary>
        /// Adds a node to the address space.
        /// </summary>
        private void AddNode(ILocalNode node)
        {            
            nodes_.Attach(node);
        }
        #endregion
        
        /// <summary>
        /// Returns a node managed by the manager with the specified node id.
        /// </summary>
        public ILocalNode GetLocalNode(ExpandedNodeId nodeId)
        {
            if (nodeId == null)
            {
                return null;
            }

            // check for absolute declarations of local nodes.
            if (nodeId.IsAbsolute)
            {
                if (nodeId.ServerIndex != 0)
                {
                    return null;
                }
                 
                var namespaceIndex = this.ServerData.NamespaceUris.GetIndex(nodeId.NamespaceUri);
                
                if (namespaceIndex < 0 || nodeId.NamespaceIndex >= this.ServerData.NamespaceUris.Count)
                {
                    return null;
                }

                return GetLocalNode(new NodeId(nodeId.Identifier, (ushort)namespaceIndex));
            }

            return GetLocalNode((NodeId)nodeId);
        }
        
        /// <summary>
        /// Returns a node managed by the manager with the specified node id.
        /// </summary>
        public ILocalNode GetLocalNode(
            NodeId nodeId,
            NodeId referenceTypeId,
            bool isInverse,
            bool includeSubtypes,
            QualifiedName browseName)
        {
            lock (Lock)
            {
                return nodes_.Find(nodeId, referenceTypeId, isInverse, includeSubtypes, browseName) as ILocalNode;
            }
        }

        /// <summary>
        /// Returns a node managed by the manager with the specified node id.
        /// </summary>
        public ILocalNode GetLocalNode(NodeId nodeId)
        {
            lock (Lock)
            {
                return nodes_.Find(nodeId) as ILocalNode;
            }
        }

        /// <summary>
        /// Returns a list of nodes which are targets of the specified references.
        /// </summary>
        public IList<ILocalNode> GetLocalNodes(
            NodeId        sourceId, 
            NodeId        referenceTypeId,
            bool          isInverse, 
            bool          includeSubtypes)
        {
            lock (Lock)
            {
                var targets = new List<ILocalNode>();

                var source = GetLocalNode(sourceId) as ILocalNode;

                if (source == null)
                {
                    return targets;
                }

                foreach (var reference in source.References.Find(referenceTypeId, isInverse, true, nodes_.TypeTree))
                {                    
                    var target = GetLocalNode(reference.TargetId) as ILocalNode;

                    if (target != null)
                    {
                        targets.Add(target);
                    }
                }

                return targets;
            }
        }
        
        /// <summary>
        /// Returns a node managed by the manager that has the specified browse name.
        /// </summary>
        public ILocalNode GetTargetNode(
            NodeId        sourceId, 
            NodeId        referenceTypeId,
            bool          isInverse, 
            bool          includeSubtypes, 
            QualifiedName browseName)
        {
            lock (Lock)
            {
                var source = GetLocalNode(sourceId) as ILocalNode;

                if (source == null)
                {
                    return null;
                }

                return GetTargetNode(source, referenceTypeId, isInverse, includeSubtypes, browseName);
            }
        }
        
        /// <summary>
        /// Returns a node managed by the manager that has the specified browse name.
        /// </summary>
        private ILocalNode GetTargetNode(
            ILocalNode    source, 
            NodeId        referenceTypeId,
            bool          isInverse, 
            bool          includeSubtypes, 
            QualifiedName browseName)
        {
            foreach (var reference in source.References.Find(referenceTypeId, isInverse, includeSubtypes, server_.TypeTree))
            {
                var target = GetLocalNode(reference.TargetId) as ILocalNode;

                if (target == null)
                {
                    continue;
                }

                if (QualifiedName.IsNull(browseName) || browseName == target.BrowseName)
                {
                    return target;
                }
            }

            return null;
        }
        /// <summary>
        /// Attaches a node to the address space.
        /// </summary>
        public void AttachNode(ILocalNode node)
        {
            AttachNode(node, false);
        }

        /// <summary>
        /// Attaches a node to the address space.
        /// </summary>
        private void AttachNode(ILocalNode node, bool isInternal)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            lock (Lock)
            {
                // check if node exists.
                if (nodes_.Exists(node.NodeId))
                {
                    throw ServiceResultException.Create(
                        StatusCodes.BadNodeIdExists, 
                        "A node with the same node id already exists: {0}",
                        node.NodeId);
                }

                // ensure reverse references exist.
                foreach (var reference in node.References)
                {
                    // ignore references that are always one way.
                    if (reference.ReferenceTypeId == ReferenceTypeIds.HasTypeDefinition || reference.ReferenceTypeId == ReferenceTypeIds.HasModellingRule)
                    {
                        continue;
                    }

                    // find target.
                    var target = GetLocalNode(reference.TargetId) as ILocalNode;

                    if (target != null)
                    {
                        AddReferenceToLocalNode(target, reference.ReferenceTypeId, !reference.IsInverse, node.NodeId, isInternal);
                    }                  
                }
                
                // must generate a model change event.
                AddNode(node);
            }
        }
        
        /// <summary>
        /// Creates a unique node identifier.
        /// </summary>
        public NodeId CreateUniqueNodeId()
        {
            return CreateUniqueNodeId(dynamicNamespaceIndex_);
        }
        
        #region Private Methods
        /// <see cref="IUaBaseNodeManager.GetManagerHandle" />
        private object GetManagerHandle(ExpandedNodeId nodeId)
        {
            lock (Lock)
            {
                if (nodeId == null || nodeId.IsAbsolute)
                {
                    return null;
                }

                return GetLocalNode(nodeId) as ILocalNode;
            }
        }        

        /// <summary>
        /// Reads the EU Range for a variable.
        /// </summary>
        private ServiceResult ReadEURange(UaServerOperationContext context, ILocalNode node, out Opc.Ua.Range range)
        {
            range = null;

            var target = GetTargetNode(node, ReferenceTypes.HasProperty, false, true, BrowseNames.EURange) as IVariable;

            if (target == null)
            {
                return StatusCodes.BadNodeIdUnknown;
            }

            range = target.Value as Opc.Ua.Range;
            
            if (range == null)
            {
                return StatusCodes.BadTypeMismatch;
            }

            return ServiceResult.Good;
        }
        
        /// <summary>
        /// Validates a filter for a monitored item.
        /// </summary>
        private ServiceResult ValidateFilter(
            UaNodeMetadata    metadata, 
            uint            attributeId, 
            ExtensionObject filter, 
            out bool        rangeRequired)
        {
            rangeRequired = false;

            // check filter.
            DataChangeFilter datachangeFilter = null;

            if (filter != null)
            {
                datachangeFilter = filter.Body as DataChangeFilter;
            }
            
            if (datachangeFilter != null)
            {
                // get the datatype of the node.                
                var datatypeId = metadata.DataType;
                
                // check that filter is valid.
                var error = datachangeFilter.Validate();

                if (ServiceResult.IsBad(error))
                {
                    return error;
                }
                
                // check datatype of the variable.
                if (!server_.TypeTree.IsTypeOf(datatypeId, DataTypes.Number))
                {
                    return StatusCodes.BadDeadbandFilterInvalid;
                }

                // percent deadbands only allowed for analog data items.
                if (datachangeFilter.DeadbandType == (uint)(int)DeadbandType.Percent)
                {
                    var typeDefinitionId = metadata.TypeDefinition;

                    if (typeDefinitionId == null)
                    {
                        return StatusCodes.BadDeadbandFilterInvalid;
                    }
                    
                    // percent deadbands only allowed for analog data items.
                    if (!server_.TypeTree.IsTypeOf(typeDefinitionId, VariableTypes.AnalogItemType))
                    {
                        return StatusCodes.BadDeadbandFilterInvalid;
                    }

                    // the EURange property is required to use the filter.
                    rangeRequired = true;
                }
            }
        
            // filter is valid
            return ServiceResult.Good;
        }

        /// <summary>
        /// Creates a new unique identifier for a node.
        /// </summary>
        private NodeId CreateUniqueNodeId(ushort namespaceIndex)
        {
            return new NodeId(Utils.IncrementIdentifier(ref lastId_), namespaceIndex);
        }
        #endregion
        
        #region Private Fields
        private IUaServerData server_;
        private NodeTable nodes_;
        private long lastId_;
        private SamplingGroupManager samplingGroupManager_;
        private Dictionary<uint, UaMonitoredItem> monitoredItems_;
        private double defaultMinimumSamplingInterval_;
        private List<string> namespaceUris_;
        private ushort dynamicNamespaceIndex_;
        #endregion            
    }    
    
}
