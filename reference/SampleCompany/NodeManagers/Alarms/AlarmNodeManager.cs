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
using System.Threading;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace SampleCompany.NodeManagers.Alarms
{
    /// <summary>
    /// The factory for the Alarm Node Manager.
    /// </summary>
    public class AlarmNodeManagerFactory : IUaNodeManagerFactory
    {
        /// <inheritdoc/>
        public IUaBaseNodeManager Create(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new AlarmNodeManager(server, configuration, NamespacesUris.ToArray());
        }

        /// <inheritdoc/>
        public StringCollection NamespacesUris
        {
            get
            {
                var uri = Namespaces.Alarms;
                var instanceUri = uri + "Instance";
                return new StringCollection { uri, instanceUri };
            }
        }

    }

    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class AlarmNodeManager : UaGenericNodeManager
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public AlarmNodeManager(IUaServerData server, ApplicationConfiguration configuration, string[] namespaceUris) :
            base(server, configuration, namespaceUris)
        {
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeTimer();
            }
        }

        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId Create(ISystemContext context, NodeState node)
        {
            var instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                var id = instance.Parent.NodeId.Identifier as string;

                if (id != null)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }

            return node.NodeId;
        }
        #endregion

        #region IUaNodeManager Members
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
            lock (Lock)
            {
                #region Setup
                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }
                #endregion

                try
                {
                    #region Initialize
                    var alarmsName = "Alarms";
                    var alarmsNodeName = alarmsName;

                    var alarmControllerType = Type.GetType("SampleCompany.NodeManagers.Alarms.AlarmController");
                    var interval = 1000;
                    var intervalString = interval.ToString();

                    var conditionTypeIndex = 0;
                    #endregion

                    #region Create Alarm Folder
                    FolderState alarmsFolder = CreateFolder(null, alarmsNodeName, alarmsName);
                    alarmsFolder.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                    references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, alarmsFolder.NodeId));
                    alarmsFolder.EventNotifier = EventNotifiers.SubscribeToEvents;
                    AddRootNotifier(alarmsFolder);

                    #endregion

                    #region Create Methods
                    var startMethodName = "Start";
                    var startMethodNodeName = alarmsNodeName + "." + startMethodName;
                    MethodState startMethod = AlarmHelpers.CreateMethod(alarmsFolder, NamespaceIndex, startMethodNodeName, startMethodName);
                    AlarmHelpers.AddStartInputParameters(startMethod, NamespaceIndex);
                    startMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnStart);

                    var startBranchMethodName = "StartBranch";
                    var startBranchMethodNodeName = alarmsNodeName + "." + startBranchMethodName;
                    MethodState startBranchMethod = AlarmHelpers.CreateMethod(alarmsFolder, NamespaceIndex, startBranchMethodNodeName, startBranchMethodName);
                    AlarmHelpers.AddStartInputParameters(startBranchMethod, NamespaceIndex);
                    startBranchMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnStartBranch);

                    var endMethodName = "End";
                    var endMethodNodeName = alarmsNodeName + "." + endMethodName;
                    MethodState endMethod = AlarmHelpers.CreateMethod(alarmsFolder, NamespaceIndex, endMethodNodeName, endMethodName);
                    endMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnEnd);
                    #endregion

                    #region Create Variables
                    var analogTriggerName = "AnalogSource";
                    var analogTriggerNodeName = alarmsNodeName + "." + analogTriggerName;
                    BaseDataVariableState analogTrigger = AlarmHelpers.CreateVariable(alarmsFolder,
                        NamespaceIndex, analogTriggerNodeName, analogTriggerName);
                    analogTrigger.OnWriteValue = OnWriteAlarmTrigger;
                    var analogAlarmController = (AlarmController)Activator.CreateInstance(alarmControllerType, analogTrigger, interval, false);
                    var analogSourceController = new SourceController(analogTrigger, analogAlarmController);
                    triggerMap_.Add("Analog", analogSourceController);

                    var booleanTriggerName = "BooleanSource";
                    var booleanTriggerNodeName = alarmsNodeName + "." + booleanTriggerName;
                    BaseDataVariableState booleanTrigger = AlarmHelpers.CreateVariable(alarmsFolder,
                        NamespaceIndex, booleanTriggerNodeName, booleanTriggerName, boolValue: true);
                    booleanTrigger.OnWriteValue = OnWriteAlarmTrigger;
                    var booleanAlarmController = (AlarmController)Activator.CreateInstance(alarmControllerType, booleanTrigger, interval, true);
                    var booleanSourceController = new SourceController(booleanTrigger, booleanAlarmController);
                    triggerMap_.Add("Boolean", booleanSourceController);
                    #endregion

                    #region Create Alarms
                    AlarmHolder mandatoryExclusiveLevel = new ExclusiveLevelHolder(
                        this,
                        alarmsFolder,
                        analogSourceController,
                        intervalString,
                        GetSupportedAlarmConditionType(ref conditionTypeIndex),
                        alarmControllerType,
                        interval,
                        optional: false);

                    alarms_.Add(mandatoryExclusiveLevel.AlarmNodeName, mandatoryExclusiveLevel);

                    AlarmHolder mandatoryNonExclusiveLevel = new NonExclusiveLevelHolder(
                        this,
                        alarmsFolder,
                        analogSourceController,
                        intervalString,
                        GetSupportedAlarmConditionType(ref conditionTypeIndex),
                        alarmControllerType,
                        interval,
                        optional: false);
                    alarms_.Add(mandatoryNonExclusiveLevel.AlarmNodeName, mandatoryNonExclusiveLevel);

                    AlarmHolder offNormal = new OffNormalAlarmTypeHolder(
                        this,
                        alarmsFolder,
                        booleanSourceController,
                        intervalString,
                        GetSupportedAlarmConditionType(ref conditionTypeIndex),
                        alarmControllerType,
                        interval,
                        optional: false);
                    alarms_.Add(offNormal.AlarmNodeName, offNormal);
                    #endregion

                    AddPredefinedNode(SystemContext, alarmsFolder);
                    StartTimer();
                    allowEntry_ = true;

                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error creating the AlarmNodeManager address space.");
                }

            }
        }



        /// <summary>
        /// Creates a new folder.
        /// </summary>
        private FolderState CreateFolder(NodeState parent, string path, string name)
        {
            var folder = new FolderState(parent) {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                EventNotifier = EventNotifiers.None
            };

            parent?.AddChild(folder);

            return folder;
        }

        /// <summary>
        /// Creates a new method.
        /// </summary>
        private MethodState CreateMethod(NodeState parent, string path, string name)
        {
            var method = new MethodState(parent) {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                Executable = true,
                UserExecutable = true
            };

            parent?.AddChild(method);

            return method;
        }

        private void DoSimulation(object state)
        {
            if (allowEntry_)
            {
                allowEntry_ = false;

                lock (alarms_)
                {
                    success_++;
                    try
                    {
                        foreach (SourceController controller in triggerMap_.Values)
                        {
                            var updated = controller.Controller.Update(SystemContext);

                            IList<IReference> references = new List<IReference>();
                            controller.Source.GetReferences(SystemContext, references, ReferenceTypes.HasCondition, false);
                            foreach (IReference reference in references)
                            {
                                var identifier = (string)reference.TargetId.ToString();
                                if (alarms_.ContainsKey(identifier))
                                {
                                    AlarmHolder holder = alarms_[identifier];
                                    holder.Update(updated);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.LogInfo(ex, "Alarm Loop Exception");
                    }
                }
                allowEntry_ = true;
            }
            else
            {
                if (success_ > 0)
                {
                    missed_++;
                    Utils.LogInfo("Alarms: Missed Loop {1} Success {2}", missed_, success_);
                }
            }
        }

        #region Methods
        public ServiceResult OnStart(
            ISystemContext context,
            NodeState node,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            // all arguments must be provided.
            UInt32 seconds;
            if (inputArguments.Count < 1)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                seconds = (UInt32)inputArguments[0];
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }

            ServiceResult result = ServiceResult.Good;

            Dictionary<string, SourceController> sourceControllers = GetUnitAlarms(node);
            if (sourceControllers == null)
            {
                result = StatusCodes.BadNodeIdUnknown;
            }

            if (sourceControllers != null)
            {
                Utils.LogInfo("Starting up alarm group {0}", GetUnitFromNodeId(node.NodeId));

                lock (alarms_)
                {
                    foreach (SourceController sourceController in sourceControllers.Values)
                    {
                        IList<IReference> references = new List<IReference>();
                        sourceController.Source.GetReferences(SystemContext, references, ReferenceTypes.HasCondition, false);
                        foreach (IReference reference in references)
                        {
                            var identifier = (string)reference.TargetId.ToString();
                            if (alarms_.ContainsKey(identifier))
                            {
                                AlarmHolder holder = alarms_[identifier];
                                holder.SetBranching(false);
                                holder.Start(seconds);
                                var updated = holder.Controller.Update(SystemContext);
                                holder.Update(updated);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public ServiceResult OnStartBranch(
            ISystemContext context,
            NodeState node,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            // all arguments must be provided.
            UInt32 seconds;
            if (inputArguments.Count < 1)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                seconds = (UInt32)inputArguments[0];
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }

            ServiceResult result = ServiceResult.Good;

            Dictionary<string, SourceController> sourceControllers = GetUnitAlarms(node);
            if (sourceControllers == null)
            {
                result = StatusCodes.BadNodeIdUnknown;
            }

            if (sourceControllers != null)
            {
                Utils.LogInfo("Starting up Branch for alarm group {0}", GetUnitFromNodeId(node.NodeId));

                lock (alarms_)
                {
                    foreach (SourceController sourceController in sourceControllers.Values)
                    {
                        IList<IReference> references = new List<IReference>();
                        sourceController.Source.GetReferences(SystemContext, references, ReferenceTypes.HasCondition, false);
                        foreach (IReference reference in references)
                        {
                            var identifier = (string)reference.TargetId.ToString();
                            if (alarms_.ContainsKey(identifier))
                            {
                                AlarmHolder holder = alarms_[identifier];
                                holder.SetBranching(true);
                                holder.Start(seconds);
                                var updated = holder.Controller.Update(SystemContext);
                                holder.Update(updated);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public ServiceResult OnEnd(
            ISystemContext context,
            NodeState node,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            ServiceResult result = ServiceResult.Good;

            Dictionary<string, SourceController> sourceControllers = GetUnitAlarms(node);
            if (sourceControllers == null)
            {
                result = StatusCodes.BadNodeIdUnknown;
            }

            if (sourceControllers != null)
            {
                Utils.LogInfo("Stopping alarm group {0}", GetUnitFromNodeId(node.NodeId));

                lock (alarms_)
                {
                    foreach (SourceController sourceController in sourceControllers.Values)
                    {
                        IList<IReference> references = new List<IReference>();
                        sourceController.Source.GetReferences(SystemContext, references, ReferenceTypes.HasCondition, false);
                        foreach (IReference reference in references)
                        {
                            var identifier = (string)reference.TargetId.ToString();
                            if (alarms_.ContainsKey(identifier))
                            {
                                AlarmHolder holder = alarms_[identifier];
                                holder.ClearBranches();
                            }
                        }

                        sourceController.Controller.Stop();
                    }
                }
            }

            return result;
        }

        public ServiceResult OnWriteAlarmTrigger(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            Dictionary<string, SourceController> sourceControllers = GetUnitAlarms(node);
            if (sourceControllers == null)
            {
                return StatusCodes.BadNodeIdUnknown;
            }

            if (sourceControllers != null)
            {
                SourceController sourceController = GetSourceControllerFromNodeState(node, sourceControllers);

                if (sourceController == null)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                Utils.LogInfo("Manual Write {0} to {1}", value, node.NodeId);

                lock (alarms_)
                {
                    sourceController.Source.Value = value;
                    Type valueType = value.GetType();
                    sourceController.Controller.ManualWrite(value);
                    IList<IReference> references = new List<IReference>();
                    sourceController.Source.GetReferences(SystemContext, references, ReferenceTypes.HasCondition, false);
                    foreach (IReference reference in references)
                    {
                        var identifier = (string)reference.TargetId.ToString();
                        if (alarms_.ContainsKey(identifier))
                        {
                            AlarmHolder holder = alarms_[identifier];
                            holder.Update(true);
                        }
                    }
                }
            }

            return StatusCodes.Good;
        }

        #endregion

        #region Helpers

        private AlarmHolder GetAlarmHolder(NodeState node)
        {
            return GetAlarmHolder(node.NodeId);
        }

        private AlarmHolder GetAlarmHolder(NodeId node)
        {
            AlarmHolder alarmHolder = null;

            Type nodeIdType = node.Identifier.GetType();
            if (nodeIdType.Name == "String")
            {
                var unmodifiedName = node.Identifier.ToString();

                // This is bad, but I'm not sure why the NodeName is being attached with an underscore, it messes with this lookup.
                var name = unmodifiedName.Replace("Alarms_", "Alarms.");

                var mapName = name;
                if (name.EndsWith(AlarmConstants.TriggerExtension) || name.EndsWith(AlarmConstants.AlarmExtension))
                {
                    var lastDot = name.LastIndexOf(".");
                    mapName = name.Substring(0, lastDot);
                }

                if (alarms_.ContainsKey(mapName))
                {
                    alarmHolder = alarms_[mapName];
                }
            }

            return alarmHolder;
        }


        public ServiceResult OnEnableDisableAlarm(
            ISystemContext context,
            ConditionState condition,
            bool enabling)
        {
            return ServiceResult.Good;
        }


        public Dictionary<string, SourceController> GetUnitAlarms(NodeState nodeState)
        {
            return triggerMap_;
        }


        public string GetUnitFromNodeState(NodeState nodeState)
        {
            return GetUnitFromNodeId(nodeState.NodeId);
        }

        public string GetUnitFromNodeId(NodeId nodeId)
        {
            var unit = "";

            if (nodeId.IdType == IdType.String)
            {
                var nodeIdString = (string)nodeId.Identifier;
                var splitString = nodeIdString.Split('.');
                // Alarms.UnitName.MethodName
                if (splitString.Length >= 1)
                {
                    unit = splitString[1];
                }
            }

            return unit;
        }

        public SourceController GetSourceControllerFromNodeState(NodeState nodeState, Dictionary<string, SourceController> map)
        {
            SourceController sourceController = null;

            var name = GetSourceNameFromNodeState(nodeState);
            if (map.ContainsKey(name))
            {
                sourceController = map[name];
            }

            return sourceController;
        }

        public string GetSourceNameFromNodeState(NodeState nodeState)
        {
            return GetSourceNameFromNodeId(nodeState.NodeId);
        }

        public string GetSourceNameFromNodeId(NodeId nodeId)
        {
            var sourceName = "";

            if (nodeId.IdType == IdType.String)
            {
                var nodeIdString = (string)nodeId.Identifier;
                var splitString = nodeIdString.Split('.');
                // Alarms.UnitName.AnalogSource
                if (splitString.Length >= 2)
                {
                    sourceName = splitString[splitString.Length - 1].Replace("Source", "");
                }
            }

            return sourceName;
        }

        public SupportedAlarmConditionType GetSupportedAlarmConditionType(ref int index)
        {
            SupportedAlarmConditionType conditionType = conditionTypes_[index];
            index++;
            if (index >= conditionTypes_.Length)
            {
                index = 0;
            }
            return conditionType;
        }
        #endregion

        #endregion

        #region Overrides
        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                // TBD
            }
        }

        /// <summary>
        /// Calls a method on the specified nodes.
        /// </summary>
        public override void Call(
            UaServerOperationContext context,
            IList<CallMethodRequest> methodsToCall,
            IList<CallMethodResult> results,
            IList<ServiceResult> errors)
        {
            UaServerContext systemContext = SystemContext.Copy(context);
            IDictionary<NodeId, NodeState> operationCache = new NodeIdDictionary<NodeState>();

            var didRefresh = false;

            for (var ii = 0; ii < methodsToCall.Count; ii++)
            {
                CallMethodRequest methodToCall = methodsToCall[ii];

                var refreshMethod = methodToCall.MethodId.Equals(MethodIds.ConditionType_ConditionRefresh) ||
                    methodToCall.MethodId.Equals(MethodIds.ConditionType_ConditionRefresh2);

                if (refreshMethod)
                {
                    if (didRefresh)
                    {
                        errors[ii] = StatusCodes.BadRefreshInProgress;
                        methodToCall.Processed = true;
                        continue;
                    }
                    else
                    {
                        didRefresh = true;
                    }
                }

                var ackMethod = methodToCall.MethodId.Equals(MethodIds.AcknowledgeableConditionType_Acknowledge);
                var confirmMethod = methodToCall.MethodId.Equals(MethodIds.AcknowledgeableConditionType_Confirm);
                var commentMethod = methodToCall.MethodId.Equals(MethodIds.ConditionType_AddComment);
                var ackConfirmMethod = ackMethod || confirmMethod || commentMethod;

                // Need to try to capture any calls to ConditionType::Acknowledge
                if (methodToCall.ObjectId.Equals(ObjectTypeIds.ConditionType) && ackConfirmMethod)
                {
                    // Mantis Issue 6944 which is a duplicate of 5544 - result is Confirm should be Bad_NodeIdInvalid
                    // Override any other errors that may be there, even if this is 'Processed'
                    errors[ii] = StatusCodes.BadNodeIdInvalid;
                    methodToCall.Processed = true;
                    continue;
                }

                // skip items that have already been processed.
                if (methodToCall.Processed)
                {
                    continue;
                }

                MethodState method = null;

                lock (Lock)
                {
                    // check for valid handle.
                    UaNodeHandle initialHandle = GetManagerHandle(systemContext, methodToCall.ObjectId, operationCache);

                    if (initialHandle == null)
                    {
                        if (ackConfirmMethod)
                        {
                            // Mantis 6944
                            errors[ii] = StatusCodes.BadNodeIdUnknown;
                            methodToCall.Processed = true;
                        }

                        continue;
                    }

                    // owned by this node manager.
                    methodToCall.Processed = true;

                    // Look for an alarm branchId to operate on.
                    UaNodeHandle handle = FindBranchNodeHandle(systemContext, initialHandle, methodToCall);

                    // validate the source node.
                    NodeState source = ValidateNode(systemContext, handle, operationCache);

                    if (source == null)
                    {
                        errors[ii] = StatusCodes.BadNodeIdUnknown;
                        continue;
                    }

                    // find the method.
                    method = source.FindMethod(systemContext, methodToCall.MethodId);

                    if (method == null)
                    {
                        // check for loose coupling.
                        if (source.ReferenceExists(ReferenceTypeIds.HasComponent, false, methodToCall.MethodId))
                        {
                            method = (MethodState)FindPredefinedNode(methodToCall.MethodId, typeof(MethodState));
                        }

                        if (method == null)
                        {
                            errors[ii] = StatusCodes.BadMethodInvalid;
                            continue;
                        }
                    }
                }

                // call the method.
                CallMethodResult result = results[ii] = new CallMethodResult();

                errors[ii] = Call(
                    systemContext,
                    methodToCall,
                    method,
                    result);
            }
        }

        /// <summary>
        /// Override ConditionRefresh.
        /// </summary>
        public override ServiceResult ConditionRefresh(
            UaServerOperationContext context,
            IList<IUaEventMonitoredItem> monitoredItems)
        {
            UaServerContext systemContext = SystemContext.Copy(context);

            for (var ii = 0; ii < monitoredItems.Count; ii++)
            {
                // the IEventMonitoredItem should always be MonitoredItems since they are created by the MasterNodeManager.
                var monitoredItem = monitoredItems[ii] as UaMonitoredItem;

                if (monitoredItem == null)
                {
                    continue;
                }

                var events = new List<IFilterTarget>();
                var nodesToRefresh = new List<NodeState>();

                lock (Lock)
                {
                    // check for server subscription.
                    if (monitoredItem.NodeId == ObjectIds.Server)
                    {
                        if (RootNotifiers != null)
                        {
                            nodesToRefresh.AddRange(RootNotifiers);
                        }
                    }
                    else
                    {
                        // check for existing monitored node.
                        UaMonitoredNode monitoredNode = null;

                        if (!MonitoredNodes.TryGetValue(monitoredItem.NodeId, out monitoredNode))
                        {
                            continue;
                        }

                        // get the refresh events.
                        nodesToRefresh.Add(monitoredNode.Node);
                    }
                }

                // block and wait for the refresh.
                for (var jj = 0; jj < nodesToRefresh.Count; jj++)
                {
                    nodesToRefresh[jj].ConditionRefresh(systemContext, events, true);
                }

                lock (Lock)
                {
                    // This is where I can add branch events
                    GetBranchesForConditionRefresh(events);
                }

                // queue the events.
                for (var jj = 0; jj < events.Count; jj++)
                {
                    monitoredItem.QueueEvent(events[jj]);
                }
            }

            // all done.
            return ServiceResult.Good;
        }
        #endregion

        #region Public Methods
        public UaNodeHandle FindBranchNodeHandle(ISystemContext systemContext, UaNodeHandle initialHandle, CallMethodRequest methodToCall)
        {
            UaNodeHandle nodeHandle = initialHandle;

            if (IsAckConfirm(methodToCall.MethodId))
            {
                AlarmHolder holder = GetAlarmHolder(methodToCall.ObjectId);

                if (holder != null)
                {

                    if (holder.HasBranches())
                    {
                        var eventId = GetEventIdFromAckConfirmMethod(methodToCall);

                        if (eventId != null)
                        {
                            BaseEventState state = holder.GetBranch(eventId);

                            if (state != null)
                            {
                                nodeHandle = new UaNodeHandle {
                                    NodeId = methodToCall.ObjectId,
                                    Node = state,
                                    Validated = true
                                };
                            }
                        }
                    }
                }
            }

            return nodeHandle;
        }

        public void GetBranchesForConditionRefresh(List<IFilterTarget> events)
        {
            // Don't look at Certificates, they won't have branches
            foreach (AlarmHolder alarmHolder in alarms_.Values)
            {
                alarmHolder.GetBranchesForConditionRefresh(events);
            }
        }
        #endregion

        #region Private Methods
        private bool IsAckConfirm(NodeId methodId)
        {
            var isAckConfirm = false;
            if (methodId.Equals(MethodIds.AcknowledgeableConditionType_Acknowledge) ||
                 methodId.Equals(MethodIds.AcknowledgeableConditionType_Confirm))
            {
                isAckConfirm = true;

            }
            return isAckConfirm;
        }

        private byte[] GetEventIdFromAckConfirmMethod(CallMethodRequest request)
        {
            byte[] eventId = null;

            // Bad magic Numbers here
            if (request.InputArguments != null && request.InputArguments.Count == 2)
            {
                if (request.InputArguments[0].TypeInfo.BuiltInType.Equals(BuiltInType.ByteString))
                {
                    eventId = (byte[])request.InputArguments[0].Value;
                }
            }
            return eventId;
        }

        /// <summary>
        /// Starts the timer to detect Alarms.
        /// </summary>
        private void StartTimer()
        {
            Utils.SilentDispose(simulationTimer_);
            simulationTimer_ = new Timer(DoSimulation, null, SimulationInterval, SimulationInterval);
        }

        /// <summary>
        /// Disposes the timer.
        /// </summary>
        private void DisposeTimer()
        {
            Utils.SilentDispose(simulationTimer_);
            simulationTimer_ = null;
        }
        #endregion

        #region Private Fields
        private Dictionary<string, AlarmHolder> alarms_ = new Dictionary<string, AlarmHolder>();
        private Dictionary<string, SourceController> triggerMap_ =
            new Dictionary<string, SourceController>();
        private bool allowEntry_ = false;
        private uint success_ = 0;
        private uint missed_ = 0;

        private SupportedAlarmConditionType[] conditionTypes_ = {
                    new SupportedAlarmConditionType( "Process", "ProcessConditionClassType",  ObjectTypeIds.ProcessConditionClassType ),
                    new SupportedAlarmConditionType( "Maintenance", "MaintenanceConditionClassType",  ObjectTypeIds.MaintenanceConditionClassType ),
                    new SupportedAlarmConditionType( "System", "SystemConditionClassType",  ObjectTypeIds.SystemConditionClassType ) };


        private const UInt16 SimulationInterval = 100;
        private Timer simulationTimer_;
        #endregion

    }
}
