#region Copyright (c) 2021 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2021 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Opc.Ua;
using Technosoftware.UaServer;
#endregion

namespace SampleCompany.SampleServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class SampleServerNodeManager : UaBaseNodeManager
    {
        #region Private Fields
        private readonly IUaServer opcServer_;
        private readonly IUaServerPlugin opcServerPlugin_;

        // Track whether Dispose has been called.
        private bool disposed_;
        private readonly object lockDisposable_ = new object();

        private Opc.Ua.Test.DataGenerator generator_;
        private Timer simulationTimer_;
        private UInt16 simulationInterval_ = 1000;
        private bool simulationEnabled_ = true;
        private List<BaseDataVariableState> dynamicNodes_;
        #endregion

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
                        simulationTimer_?.Dispose();
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
                dynamicNodes_ = new List<BaseDataVariableState>();

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = References = new List<IReference>();
                }
                else
                {
                    References = references;
                }

                LoadPredefinedNodes(SystemContext, externalReferences);

                // Create the root folder for all nodes of this server
                var root = CreateFolderState(null, "My Data", new LocalizedText("en", "My Data"),
                    new LocalizedText("en", "Root folder of the Sample Server. All nodes must be placed under this root."));

                try
                {
                    #region Static
                    var staticFolder = CreateFolderState(root, "Static", "Static", "A folder with a sample static variable.");
                    const string scalarStatic = "Static_";
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    #endregion

                    #region Simulation
                    var simulationFolder = CreateFolderState(root, "Simulation", "Simulation", "A folder with simulated variables.");
                    const string simulation = "Simulation_";

                    var simulatedVariable = CreateDynamicVariable(simulationFolder, simulation + "Double", "Double", "A simulated variable of type Double. If Enabled is true this value changes based on the defined Interval.", DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    simulatedVariable.Historizing = true;

                    var intervalVariable = CreateBaseDataVariableState(simulationFolder, simulation + "Interval", "Interval", "The Interval used for changing the simulated values.", DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, simulationInterval_);
                    intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                    var enabledVariable = CreateBaseDataVariableState(simulationFolder, simulation + "Enabled", "Enabled", "Specifies whether the simulation is enabled (true) or disabled (false).", DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, simulationEnabled_);
                    enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                    #endregion

                    #region Simulation with HistoricalAccess
                    var simulationHistoricalAccessFolder = CreateFolderState(root, "SimulationHistoricalAccess", "SimulationHistoricalAccess", "A folder with simulated variables supporting HistoricalAccess.");
                    const string simulationHistoricalAccess = "SimulationHistoricalAccess_";

                    var simulatedHistoricalAccessVariable = CreateHistoricalAccessVariable(simulationHistoricalAccessFolder, simulationHistoricalAccess + "Double", "Double", "A simulated variable of type Double. If Enabled is true this value changes based on the defined Interval.", DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    #endregion

                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Error creating the address space.");
                }
                // Add all nodes under root to the server
                AddPredefinedNode(SystemContext, root);
                simulationTimer_ = new Timer(DoSimulation, null, 1000, 1000);
            }
        }
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
            throw new NotImplementedException();
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
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateHistoricalAccessVariable(NodeState parent, string path, string name, string description, NodeId dataType, int valueRank, byte accessLevel, object initialValue)
        {
            var variable = CreateBaseDataVariableState(parent, path, name, description, dataType, valueRank, accessLevel, initialValue);
            variable.Historizing = true;
            variable.AccessLevel = AccessLevels.HistoryReadOrWrite | AccessLevels.CurrentRead;
            variable.UserAccessLevel = AccessLevels.HistoryReadOrWrite | AccessLevels.CurrentRead;
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;

            // TODO Support of Annotations
            //var annotations = new PropertyState<Annotation>(variable)
            //{
            //    ReferenceTypeId = ReferenceTypeIds.HasProperty,
            //    TypeDefinitionId = VariableTypeIds.PropertyType,
            //    SymbolicName = Opc.Ua.BrowseNames.Annotations,
            //    BrowseName = Opc.Ua.BrowseNames.Annotations
            //};
            //annotations.DisplayName = new LocalizedText(annotations.BrowseName.Name);
            //annotations.Description = null;
            //annotations.WriteMask = 0;
            //annotations.UserWriteMask = 0;
            //annotations.DataType = DataTypeIds.Annotation;
            //annotations.ValueRank = ValueRanks.Scalar;
            //annotations.AccessLevel = AccessLevels.HistoryReadOrWrite;
            //annotations.UserAccessLevel = AccessLevels.HistoryReadOrWrite;
            //annotations.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
            //annotations.Historizing = false;
            //variable.AddChild(annotations);

            //annotations.NodeId = new NodeId(annotations.BrowseName.Name, NamespaceIndex);

            HistoricalDataConfigurationState historicalDataConfigurationState = new HistoricalDataConfigurationState(variable);
            historicalDataConfigurationState.MaxTimeInterval = new PropertyState<double>(historicalDataConfigurationState);
            historicalDataConfigurationState.MinTimeInterval = new PropertyState<double>(historicalDataConfigurationState); ;
            historicalDataConfigurationState.StartOfArchive = new PropertyState<DateTime>(historicalDataConfigurationState);
            historicalDataConfigurationState.StartOfOnlineArchive = new PropertyState<DateTime>(historicalDataConfigurationState);

            historicalDataConfigurationState.Create(
                SystemContext,
                null,
                Opc.Ua.BrowseNames.HAConfiguration,
                null,
                true);

            historicalDataConfigurationState.SymbolicName = Opc.Ua.BrowseNames.HAConfiguration;
            historicalDataConfigurationState.ReferenceTypeId = ReferenceTypeIds.HasHistoricalConfiguration;

            variable.AddChild(historicalDataConfigurationState);

            dynamicNodes_.Add(variable);
            return variable;
        }
        #endregion

        #region Event Handlers
        private ServiceResult OnWriteInterval(ISystemContext context, NodeState node, ref object value)
        {
            try
            {
                simulationInterval_ = (ushort)value;

                if (simulationEnabled_)
                {
                    simulationTimer_.Change(100, simulationInterval_);
                }

                return ServiceResult.Good;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error writing Interval variable.");
                return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Interval variable.");
            }
        }

        private ServiceResult OnWriteEnabled(ISystemContext context, NodeState node, ref object value)
        {
            try
            {
                simulationEnabled_ = (bool)value;

                simulationTimer_.Change(100, simulationEnabled_ ? simulationInterval_ : 0);

                return ServiceResult.Good;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error writing Enabled variable.");
                return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Enabled variable.");
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, string description, NodeId dataType, int valueRank, byte accessLevel, object initialValue)
        {
            var variable = CreateBaseDataVariableState(parent, path, name, description, dataType, valueRank, accessLevel, initialValue);
            dynamicNodes_.Add(variable);
            return variable;
        }

        private object GetNewValue(BaseVariableState variable)
        {
            if (generator_ == null)
            {
                generator_ = new Opc.Ua.Test.DataGenerator(null) { BoundaryValueFrequency = 0 };
            }

            object value = null;
            var retryCount = 0;

            while (value == null && retryCount < 10)
            {
                value = generator_.GetRandom(variable.DataType, variable.ValueRank, new uint[] { 10 }, opcServer_.NodeManager.ServerData.TypeTree);
                retryCount++;
            }
            return value;
        }

        private void DoSimulation(object state)
        {
            try
            {
                lock (Lock)
                {
                    foreach (var variable in dynamicNodes_)
                    {
                        opcServer_.WriteBaseVariable(variable, GetNewValue(variable), StatusCodes.Good, DateTime.UtcNow);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error doing simulation.");
            }
        }
        #endregion
    }
}