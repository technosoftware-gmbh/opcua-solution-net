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
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Linq;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    /// <summary>
    /// The node manager factory for test data.
    /// </summary>
    public class TestDataNodeManagerFactory : IUaNodeManagerFactory
    {
        /// <inheritdoc/>
        public IUaBaseNodeManager Create(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new TestDataNodeManager(server, configuration, NamespacesUris.ToArray());
        }

        /// <inheritdoc/>
        public StringCollection NamespacesUris
        {
            get
            {
                var nameSpaces = new StringCollection {
                    Namespaces.TestData,
                    Namespaces.TestData + "Instance"
                };
                return nameSpaces;
            }
        }
    }

    /// <summary>
    /// A node manager for a variety of test data.
    /// </summary>
    public class TestDataNodeManager : UaGenericNodeManager, ITestDataSystemCallback
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public TestDataNodeManager(
            IUaServerData server,
            ApplicationConfiguration configuration,
            string[] namespaceUris)
        :
            base(server, configuration)
        {
            // update the namespaces.
            NamespaceUris = namespaceUris;

            ServerData.Factory.AddEncodeableTypes(typeof(TestDataNodeManager).Assembly.GetExportedTypes().Where(t => t.FullName.StartsWith(typeof(TestDataNodeManager).Namespace)));

            // get the configuration for the node manager.
            configuration_ = configuration.ParseExtension<TestDataNodeManagerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (configuration_ == null)
            {
                configuration_ = new TestDataNodeManagerConfiguration();
            }

            lastUsedId_ = configuration_.NextUnusedId - 1;

            // create the object used to access the test system.
            system_ = new TestDataSystem(this, server.NamespaceUris, server.ServerUris);

            // update the default context.
            SystemContext.SystemHandle = system_;
        }
        #endregion

        #region ITestDataSystemCallback Members
        /// <summary>
        /// Updates the variable after receiving a notification that it has changed in the underlying system.
        /// </summary>
        public void OnDataChange(BaseVariableState variable, object value, StatusCode statusCode, DateTime timestamp)
        {
            lock (Lock)
            {
                variable.Value = value;
                variable.StatusCode = statusCode;
                variable.Timestamp = timestamp;

                // notifies any monitored items that the value has changed.
                variable.ClearChangeMasks(SystemContext, false);
            }
        }

        /// <summary>
        /// Generates values for variables with properties.
        /// </summary>
        public void OnGenerateValues(BaseVariableState variable)
        {
            lock (Lock)
            {
                if (variable is ITestDataSystemValuesGenerator generator)
                {
                    generator.OnGenerateValues(SystemContext);
                }
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns>The new NodeId.</returns>
        public override NodeId Create(ISystemContext context, NodeState node)
        {
            var id = Utils.IncrementIdentifier(ref lastUsedId_);
            return new NodeId(id, namespaceIndex_);
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Loads a node set from a file or resource and add them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            // We know the model name to load but because this project is compiled for different environments we don't know
            // the assembly it is in. Therefore we search for it:
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();
            var resourcePath = String.Empty;

            foreach (var module in names)
            {
                if (module.Contains("SampleCompany.NodeManagers.TestData.PredefinedNodes.uanodes"))
                {
                    resourcePath = module;
                    break;
                }
            }

            if (resourcePath == String.Empty)
            {
                // No assembly found containing the nodes of the model. Behaviour here can differ but in this case we just return null.
                return null;
            }

            var predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, resourcePath, assembly, true);
            return predefinedNodes;
        }
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
            lock (Lock)
            {
                // ensure the namespace used by the node manager is in the server's namespace table.
                typeNamespaceIndex_ = ServerData.NamespaceUris.GetIndexOrAppend(Namespaces.TestData);
                namespaceIndex_ = ServerData.NamespaceUris.GetIndexOrAppend(Namespaces.TestData + "Instance");

                base.CreateAddressSpace(externalReferences);

#if CONDITION_SAMPLES
                // start monitoring the system status.
                m_systemStatusCondition = (TestSystemConditionState)FindPredefinedNode(
                    new NodeId(Objects.Data_Conditions_SystemStatus, m_typeNamespaceIndex),
                    typeof(TestSystemConditionState));

                if (m_systemStatusCondition != null)
                {
                    m_systemStatusTimer = new Timer(OnCheckSystemStatus, null, 5000, 5000);
                    m_systemStatusCondition.Retain.Value = true;
                }
#endif
                // link all conditions to the conditions folder.
                var conditionsFolder = (NodeState)FindPredefinedNode(
                    new NodeId(Objects.Data_Conditions, typeNamespaceIndex_),
                    typeof(NodeState));

                foreach (NodeState node in PredefinedNodes.Values)
                {
                    var condition = node as ConditionState;
                    if (condition != null && !ReferenceEquals(condition.Parent, conditionsFolder))
                    {
                        condition.AddNotifier(SystemContext, null, true, conditionsFolder);
                        conditionsFolder.AddNotifier(SystemContext, null, false, condition);
                    }
                }

                // enable history for all numeric scalar values.
                var scalarValues = (ScalarValueObjectState)FindPredefinedNode(
                    new NodeId(Objects.Data_Dynamic_Scalar, typeNamespaceIndex_),
                    typeof(ScalarValueObjectState));

                if (scalarValues != null) 
                { 
                    scalarValues.Int32Value.Historizing = true;
                    scalarValues.Int32Value.AccessLevel = (byte)(scalarValues.Int32Value.AccessLevel | AccessLevels.HistoryRead);
                    system_.EnableHistoryArchiving(scalarValues.Int32Value);
                }

                // Initialize Root Variable for structures with variables
                {
                    var variable = FindTypeState<ScalarStructureVariableState>(Variables.Data_Static_Structure_ScalarStructure);
                    dataStaticStructureScalarStructure_ = new ScalarStructureVariableValue(variable, system_.GetRandomScalarStructureDataType(), null);
                }
                {
                    var variable = FindTypeState<ScalarStructureVariableState>(Variables.Data_Dynamic_Structure_ScalarStructure);
                    dataDynamicStructureScalarStructure_ = new ScalarStructureVariableValue(variable, system_.GetRandomScalarStructureDataType(), null);
                }
                {
                    var variable = FindTypeState<VectorVariableState>(Variables.Data_Static_Structure_VectorStructure);
                    dataStaticStructureVectorStructure_ = new VectorVariableValue(variable, system_.GetRandomVector(), null);
                }
                {
                    var variable = FindTypeState<VectorVariableState>(Variables.Data_Dynamic_Structure_VectorStructure);
                    dataDynamicStructureVectorStructure_ = new VectorVariableValue(variable, system_.GetRandomVector(), null);
                }
                {
                    var variable = FindTypeState<VectorVariableState>(Variables.Data_Static_Scalar_VectorValue);
                    dataStaticVectorScalarValue_ = new VectorVariableValue(variable, system_.GetRandomVector(), null);
                }
                {
                    var variable = FindTypeState<VectorVariableState>(Variables.Data_Dynamic_Scalar_VectorValue);
                    dataDynamicVectorScalarValue_ = new VectorVariableValue(variable, system_.GetRandomVector(), null);
                }
            }
        }

        /// <summary>
        /// Replaces the generic node with a node specific to the model.
        /// </summary>
        protected override NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
        {
            if (predefinedNode is BaseObjectState passiveNode)
            {
                NodeId typeId = passiveNode.TypeDefinitionId;

                if (!IsNodeIdInNamespace(typeId) || typeId.IdType != IdType.Numeric)
                {
                    return predefinedNode;
                }

                switch ((uint)typeId.Identifier)
                {
                    case ObjectTypes.TestSystemConditionType:
                    {
                        if (passiveNode is TestSystemConditionState)
                        {
                            break;
                        }

                        var activeNode = new TestSystemConditionState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.ScalarValueObjectType:
                    {
                        if (passiveNode is ScalarValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new ScalarValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.StructureValueObjectType:
                    {
                        if (passiveNode is StructureValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new StructureValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.AnalogScalarValueObjectType:
                    {
                        if (passiveNode is AnalogScalarValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new AnalogScalarValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.ArrayValueObjectType:
                    {
                        if (passiveNode is ArrayValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new ArrayValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.AnalogArrayValueObjectType:
                    {
                        if (passiveNode is AnalogArrayValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new AnalogArrayValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.UserScalarValueObjectType:
                    {
                        if (passiveNode is UserScalarValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new UserScalarValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.UserArrayValueObjectType:
                    {
                        if (passiveNode is UserArrayValueObjectState)
                        {
                            break;
                        }

                        var activeNode = new UserArrayValueObjectState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case ObjectTypes.MethodTestType:
                    {
                        if (passiveNode is MethodTestState)
                        {
                            break;
                        }

                        var activeNode = new MethodTestState(passiveNode.Parent);
                        activeNode.Create(context, passiveNode);

                        passiveNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }
                }
            }

            if (predefinedNode is BaseVariableState variableNode)
            {
                NodeId typeId = variableNode.TypeDefinitionId;

                if (!IsNodeIdInNamespace(typeId) || typeId.IdType != IdType.Numeric)
                {
                    return predefinedNode;
                }

                switch ((uint)typeId.Identifier)
                {
                    case VariableTypes.ScalarStructureVariableType:
                    {
                        if (variableNode is ScalarStructureVariableState)
                        {
                            break;
                        }

                        var activeNode = new ScalarStructureVariableState(variableNode.Parent);
                        activeNode.Create(context, variableNode);

                        variableNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                    case VariableTypes.VectorVariableType:
                    {
                        if (variableNode is VectorVariableState)
                        {
                            break;
                        }

                        var activeNode = new VectorVariableState(variableNode.Parent);
                        activeNode.Create(context, variableNode);

                        variableNode.Parent?.ReplaceChild(context, activeNode);

                        return activeNode;
                    }

                }
            }

            return predefinedNode;
        }

        /// <summary>
        /// Restores a previously cached history reader.
        /// </summary>
        protected virtual HistoryDataReader RestoreDataReader(UaServerOperationContext context, byte[] continuationPoint)
        {
            if (context == null || context.Session == null)
            {
                return null;
            }

            var reader = context.Session.RestoreHistoryContinuationPoint(continuationPoint) as HistoryDataReader;

            if (reader == null)
            {
                return null;
            }

            return reader;
        }

        /// <summary>
        /// Saves a history data reader.
        /// </summary>
        protected virtual void SaveDataReader(UaServerOperationContext context, HistoryDataReader reader)
        {
            if (context == null ||  context.Session == null)
            {
                return;
            }

            context.Session.SaveHistoryContinuationPoint(reader.Id, reader);
        }

        /// <summary>
        /// Returns the history data source for a node.
        /// </summary>
        protected virtual ServiceResult GetHistoryDataSource(
            UaServerOperationContext context,
            BaseVariableState variable,
            out IHistoryDataSource datasource)
        {
            datasource = system_.GetHistoryFile(variable);

            if (datasource == null)
            {
                return StatusCodes.BadNotReadable;
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Reads the raw data for a variable
        /// </summary>
        protected ServiceResult HistoryReadRaw(
            ISystemContext context,
            BaseVariableState source,
            ReadRawModifiedDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueId nodeToRead,
            HistoryReadResult result)
        {
            var serverContext = context as UaServerOperationContext;

            HistoryDataReader reader = null;
            var data = new HistoryData();

            if (nodeToRead.ContinuationPoint != null && nodeToRead.ContinuationPoint.Length > 0)
            {
                // restore the continuation point.
                reader = RestoreDataReader(serverContext, nodeToRead.ContinuationPoint);

                if (reader == null)
                {
                    return StatusCodes.BadContinuationPointInvalid;
                }

                // node id must match previous node id.
                if (reader.VariableId != nodeToRead.NodeId)
                {
                    Utils.SilentDispose(reader);
                    return StatusCodes.BadContinuationPointInvalid;
                }

                // check if releasing continuation points.
                if (releaseContinuationPoints)
                {
                    Utils.SilentDispose(reader);
                    return ServiceResult.Good;
                }
            }
            else
            {
                // get the source for the variable.
                IHistoryDataSource datasource = null;
                ServiceResult error = GetHistoryDataSource(serverContext, source, out datasource);

                if (ServiceResult.IsBad(error))
                {
                    return error;
                }

                // create a reader.
                reader = new HistoryDataReader(nodeToRead.NodeId, datasource);

                // start reading.
                reader.BeginReadRaw(
                    serverContext,
                    details,
                    timestampsToReturn,
                    nodeToRead.ParsedIndexRange,
                    nodeToRead.DataEncoding,
                    data.DataValues);
            }

            // continue reading data until done or max values reached.
            var complete = reader.NextReadRaw(
                serverContext,
                timestampsToReturn,
                nodeToRead.ParsedIndexRange,
                nodeToRead.DataEncoding,
                data.DataValues);

            // save continuation point.
            if (!complete)
            {
                SaveDataReader(serverContext, reader);
                result.StatusCode = StatusCodes.GoodMoreData;
            }

            // return the dat.
            result.HistoryData = new ExtensionObject(data);

            return result.StatusCode;
        }

        /// <summary>
        /// Returns true if the system must be scanning to provide updates for the monitored item.
        /// </summary>
        private static bool SystemScanRequired(IUaMonitoredNode monitoredNode, IUaDataChangeMonitoredItem2 monitoredItem)
        {
            // ingore other types of monitored items.
            if (monitoredItem == null)
            {
                return false;
            }

            // only care about variables and properties.
            var source = monitoredNode.Node as BaseVariableState;

            if (source == null)
            {
                return false;
            }

            // check for variables that need to be scanned.
            if (monitoredItem.AttributeId == Attributes.Value)
            {
                var test = source.Parent as TestDataObjectState;
                if (test != null && test.SimulationActive.Value)
                {
                    return true;
                }

                var sourcesource = source.Parent as BaseVariableState;
                var testtest = sourcesource?.Parent as TestDataObjectState;
                if (testtest != null && testtest.SimulationActive.Value)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called after creating a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemCreated(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            if (SystemScanRequired(handle.MonitoredNode, monitoredItem))
            {
                if (monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                {
                    system_.StartMonitoringValue(
                        monitoredItem.Id,
                        monitoredItem.SamplingInterval,
                        handle.Node as BaseVariableState);
                }
            }
        }

        /// <summary>
        /// Called after modifying a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemModified(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            if (SystemScanRequired(handle.MonitoredNode, monitoredItem))
            {
                if (monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                {
                    var source = handle.Node as BaseVariableState;
                    system_.StopMonitoringValue(monitoredItem.Id);
                    system_.StartMonitoringValue(monitoredItem.Id, monitoredItem.SamplingInterval, source);
                }
            }
        }

        /// <summary>
        /// Called after deleting a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemDeleted(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem)
        {
            // check for variables that need to be scanned.
            if (SystemScanRequired(handle.MonitoredNode, monitoredItem))
            {
                system_.StopMonitoringValue(monitoredItem.Id);
            }
        }

        /// <summary>
        /// Called after changing the MonitoringMode for a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <param name="previousMode">The previous monitoring mode.</param>
        /// <param name="monitoringMode">The current monitoring mode.</param>
        protected override void OnMonitoringModeChanged(
            UaServerContext context,
            UaNodeHandle handle,
            UaMonitoredItem monitoredItem,
            MonitoringMode previousMode,
            MonitoringMode monitoringMode)
        {
            if (SystemScanRequired(handle.MonitoredNode, monitoredItem))
            {
                var source = handle.Node as BaseVariableState;

                if (previousMode != MonitoringMode.Disabled && monitoredItem.MonitoringMode == MonitoringMode.Disabled)
                {
                    system_.StopMonitoringValue(monitoredItem.Id);
                }

                if (previousMode == MonitoringMode.Disabled && monitoredItem.MonitoringMode != MonitoringMode.Disabled)
                {
                    system_.StartMonitoringValue(monitoredItem.Id, monitoredItem.SamplingInterval, source);
                }
            }
        }

        private TS FindTypeState<TS>(uint nodeId)
            where TS : NodeState
        {
            var expandedNodeId = new ExpandedNodeId(nodeId, Namespaces.TestData);
            return FindPredefinedNode(
                ExpandedNodeId.ToNodeId(expandedNodeId, ServerData.NamespaceUris),
                typeof(TS)) as TS;
        }
        #endregion

#if CONDITION_SAMPLES
        /// <summary>
        /// Peridically checks the system state.
        /// </summary>
        private void OnCheckSystemStatus(object state)
        {
            lock (Lock)
            {
                try
                {  
                    // create the dialog.
                    if (m_dialog == null)
                    {
                        m_dialog = new DialogConditionState(null);

                        CreateNode(
                            SystemContext,
                            ExpandedNodeId.ToNodeId(ObjectIds.Data_Conditions, SystemContext.NamespaceUris),
                            ReferenceTypeIds.HasComponent,
                            new QualifiedName("ResetSystemDialog", m_namespaceIndex),
                            m_dialog);

                        m_dialog.OnAfterResponse = OnDialogComplete;
                    }
        
                    StatusCode systemStatus = m_system.SystemStatus;
                    m_systemStatusCondition.UpdateStatus(systemStatus);

                    // cycle through different status codes in order to simulate a real system.
                    if (StatusCode.IsGood(systemStatus))
                    {
                        m_systemStatusCondition.UpdateSeverity((ushort)EventSeverity.Low);
                        m_system.SystemStatus = StatusCodes.Uncertain;
                    }
                    else if (StatusCode.IsUncertain(systemStatus))
                    {
                        m_systemStatusCondition.UpdateSeverity((ushort)EventSeverity.Medium);
                        m_system.SystemStatus = StatusCodes.Bad;
                    }
                    else
                    {
                        m_systemStatusCondition.UpdateSeverity((ushort)EventSeverity.High);
                        m_system.SystemStatus = StatusCodes.Good;
                    }

                    // request a reset if status is bad.
                    if (StatusCode.IsBad(systemStatus))
                    {
                        m_dialog.RequestResponse(
                            SystemContext, 
                            "Reset the test system?", 
                            (uint)(int)(DialogConditionChoice.Ok | DialogConditionChoice.Cancel),
                            (ushort)EventSeverity.MediumHigh);
                    }
                                        
                    // report the event.
                    TranslationInfo info = new TranslationInfo(
                        "TestSystemStatusChange",
                        "en-US",
                        "The TestSystem status is now {0}.",
                        systemStatus);

                    m_systemStatusCondition.ReportConditionChange(
                        SystemContext,
                        null,
                        new LocalizedText(info),
                        false);
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Unexpected error monitoring system status.");
                }
            }
        }

        /// <summary>
        /// Handles a user response to a dialog.
        /// </summary>
        private ServiceResult OnDialogComplete(
            ISystemContext context, 
            DialogConditionState dialog, 
            DialogConditionChoice response)
        {
            if (m_dialog != null)
            {
                DeleteNode(SystemContext, m_dialog.NodeId);
                m_dialog = null;
            }

            return ServiceResult.Good;
        }
#endif

        #region Private Fields
        private TestDataNodeManagerConfiguration configuration_;
        private ushort namespaceIndex_;
        private ushort typeNamespaceIndex_;
        private TestDataSystem system_;
        private long lastUsedId_;
#if CONDITION_SAMPLES
        private Timer m_systemStatusTimer;
        private TestSystemConditionState m_systemStatusCondition;
        private DialogConditionState m_dialog;
#endif
        private ScalarStructureVariableValue dataStaticStructureScalarStructure_;
        private ScalarStructureVariableValue dataDynamicStructureScalarStructure_;
        private VectorVariableValue dataStaticStructureVectorStructure_;
        private VectorVariableValue dataDynamicStructureVectorStructure_;
        private VectorVariableValue dataStaticVectorScalarValue_;
        private VectorVariableValue dataDynamicVectorScalarValue_;
        #endregion
    }
}
