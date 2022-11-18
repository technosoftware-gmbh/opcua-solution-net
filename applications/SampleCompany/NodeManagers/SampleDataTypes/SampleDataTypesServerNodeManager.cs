#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using Opc.Ua;

using Opc.Ua.Test;

using Technosoftware.UaServer;
using Technosoftware.UaStandardServer;
using System.Reflection;
using System.Linq;
#endregion

namespace SampleCompany.NodeManagers.SampleDataTypes
{
    /// <summary>
    /// The node manager factory for test data.
    /// </summary>
    public class DataTypesServerNodeManagerFactory : IUaNodeManagerFactory
    {
        /// <inheritdoc/>
        public IUaBaseNodeManager Create(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new DataTypesServerNodeManager(server, configuration, NamespacesUris.ToArray());
        }

        /// <inheritdoc/>
        public StringCollection NamespacesUris
        {
            get
            {
                var nameSpaces = new StringCollection {
                    Namespaces.SampleDataTypes,
                    Namespaces.SampleDataTypes + "Instance"
                };
                return nameSpaces;
            }
        }
    }
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class DataTypesServerNodeManager : UaStandardNodeManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public DataTypesServerNodeManager(
            IUaServerData uaServer,
            ApplicationConfiguration configuration,
            string[] namespaceUris)
            : base(uaServer, configuration, namespaceUris)
        {
            SystemContext.NodeIdFactory = this;

            // update the namespaces.
            NamespaceUris = namespaceUris;

            ServerData.Factory.AddEncodeableTypes(typeof(DataTypesServerNodeManager).Assembly.GetExportedTypes().Where(t => t.FullName.StartsWith(typeof(DataTypesServerNodeManager).Namespace)));

            dynamicNodes_ = new List<BaseDataVariableState>();
        }
        #endregion

        #region IDisposable Members
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
            base.Dispose(disposing);
        }
        #endregion

        #region INodeIdFactory Members
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Loads a node set from a file or resource and address them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            // We know the model name to load but because this project is compiled for different environments we don't know
            // the assembly it is in. Therefor we search for it:
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();
            var resourcePath = String.Empty;

            foreach (var module in names)
            {
                if (module.Contains("SampleCompany.NodeManagers.SampleDataTypes.PredefinedNodes.uanodes"))
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
                if (!externalReferences.TryGetValue(Opc.Ua.ObjectIds.ObjectsFolder, out var references))
                {
                    externalReferences[Opc.Ua.ObjectIds.ObjectsFolder] = References = new List<IReference>();
                }
                else
                {
                    References = references;
                }

                LoadPredefinedNodes(SystemContext, externalReferences);

                // Create the root folder for all nodes of this server
                root_  = CreateFolderState(null, "My Data", new LocalizedText("en", "My Data"),
                    new LocalizedText("en", "Root folder of the Sample Server. All nodes must be placed under this root."));

                try
                {
                    #region Static
                    ResetRandomGenerator(1);
                    var staticFolder = CreateFolderState(root_, "Static", "Static", "A folder with a sample static variable.");
                    const string scalarStatic = "Static_";
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "String", "String", null, Opc.Ua.DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    #endregion

                    #region Simulation
                    var simulationFolder = CreateFolderState(root_, "Simulation", "Simulation", "A folder with simulated variables.");
                    const string simulation = "Simulation_";

                    var simulatedVariable = CreateDynamicVariable(simulationFolder, simulation + "Double", "Double", "A simulated variable of type Double. If Enabled is true this value changes based on the defined Interval.", Opc.Ua.DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

                    var intervalVariable = CreateBaseDataVariableState(simulationFolder, simulation + "Interval", "Interval", "The Interval used for changing the simulated values.", Opc.Ua.DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, simulationInterval_);
                    intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                    var enabledVariable = CreateBaseDataVariableState(simulationFolder, simulation + "Enabled", "Enabled", "Specifies whether the simulation is enabled (true) or disabled (false).", Opc.Ua.DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, simulationEnabled_);
                    enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                    #endregion

                    #region Devices
                    var devices = CreateFolderState(root_, "Devices", "Devices", null);
                    string symbolicName = $"Controler #1";
                    string displayName = symbolicName;
                    GenericControllerState controller = new GenericControllerState(devices);

                    var nodeId = new NodeId(symbolicName, devices.NodeId.NamespaceIndex);
                    controller.Create(SystemContext, nodeId, symbolicName, displayName, true);

                    controller.AddReference(ReferenceTypeIds.Organizes, true, devices.NodeId);
                    devices.AddReference(ReferenceTypeIds.Organizes, false, controller.NodeId);
                    AddPredefinedNode(SystemContext, controller);
                    #endregion

                    #region Plant
                    var plantFolder = CreateFolderState(root_, "Plant", "Plant", null);

                    // Create an instance for machine 1
                    symbolicName = $"Machine #1";
                    displayName = symbolicName;
                    machine1_ = new MachineState(plantFolder);

                    nodeId = new NodeId(symbolicName, plantFolder.NodeId.NamespaceIndex);
                    machine1_.Create(SystemContext, nodeId, symbolicName, displayName, true);
                    // Initialize the property value of MachineData
                    machine1_.MachineData.Value = new MachineDataType
                    {
                        MachineName = displayName,
                        Manufacturer = "SampleCompany",
                        SerialNumber = "SN 1079",
                        MachineState = MachineStateDataType.Inactive
                    };

                    machine1_.AddReference(ReferenceTypeIds.Organizes, true, plantFolder.NodeId);
                    plantFolder.AddReference(ReferenceTypeIds.Organizes, false, machine1_.NodeId);
                    AddPredefinedNode(SystemContext, machine1_);

                    // Create an instance for machine 2
                    symbolicName = $"Machine #2";
                    displayName = symbolicName;
                    machine2_ = new MachineState(plantFolder);

                    nodeId = new NodeId(symbolicName, plantFolder.NodeId.NamespaceIndex);
                    machine2_.Create(
                        SystemContext,
                        nodeId,
                        displayName,
                        null,
                        true);
                    // Initialize the property value of MachineData
                    machine2_.MachineData.Value = new MachineDataType
                    {
                        MachineName = displayName,
                        Manufacturer = "Unknown",
                        SerialNumber = "SN 1312",
                        MachineState = MachineStateDataType.PrepareRemove
                    };

                    machine2_.AddReference(ReferenceTypeIds.Organizes, true, plantFolder.NodeId);
                    plantFolder.AddReference(ReferenceTypeIds.Organizes, false, machine2_.NodeId);
                    AddPredefinedNode(SystemContext, machine2_);

                    // Create an instance of GetMachineDataMethodState
                    symbolicName = $"GetMachineData";
                    displayName = symbolicName;
                    GetMachineDataMethodState getMachineDataMethod = new GetMachineDataMethodState(plantFolder);

                    nodeId = new NodeId(symbolicName, plantFolder.NodeId.NamespaceIndex);
                    getMachineDataMethod.Create(SystemContext, nodeId, symbolicName, displayName, true);
                    getMachineDataMethod.AddReference(ReferenceTypeIds.Organizes, true, plantFolder.NodeId);
                    plantFolder.AddReference(ReferenceTypeIds.Organizes, false, getMachineDataMethod.NodeId);
                    plantFolder.AddChild(getMachineDataMethod);


                    // Add the event handler if the method is called
                    getMachineDataMethod.OnCall = OnGetMachineData;
                    AddPredefinedNode(SystemContext, getMachineDataMethod);
                    #endregion
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error creating the SampleDataTypesServerNodeManager address space.");
                }

                AddPredefinedNode(SystemContext, root_);

                // reset random generator and generate boundary values
                ResetRandomGenerator(100, 1);
                simulationTimer_ = new Timer(DoSimulation, null, 1000, 1000);
            }
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
                Utils.LogError(e, "Error writing Interval variable.");
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

        private ServiceResult OnGetMachineData(ISystemContext context, MethodState method, NodeId objectid, string machineName, ref MachineDataType machinedata)
        {
            machinedata = new MachineDataType { MachineName = machineName };

            if (machineName == "Machine #1")
            {
                machinedata.Manufacturer = machine1_.MachineData.Value.Manufacturer;
                machinedata.SerialNumber = machine1_.MachineData.Value.SerialNumber;
                machinedata.MachineState = machine1_.MachineData.Value.MachineState;
            }
            else if (machineName == "Machine #2")
            {
                machinedata.Manufacturer = machine1_.MachineData.Value.Manufacturer;
                machinedata.SerialNumber = machine1_.MachineData.Value.SerialNumber;
                machinedata.MachineState = machine1_.MachineData.Value.MachineState;
            }
            else
            {
                machinedata.Manufacturer = "Unknown";
                machinedata.SerialNumber = "Unknown";
                machinedata.MachineState = MachineStateDataType.Failed;
            }
            return ServiceResult.Good;
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

        private void DoSimulation(object state)
        {
            try
            {
                lock (Lock)
                {
                    var timeStamp = DateTime.UtcNow;
                    foreach (var variable in dynamicNodes_)
                    {
                        variable.Value = GetNewValue(variable);
                        variable.Timestamp = timeStamp;
                        variable.ClearChangeMasks(SystemContext, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error doing simulation.");
            }
        }
        #endregion

        #region Private Fields
        // Track whether Dispose has been called.
        private bool disposed_;
        private readonly object lockDisposable_ = new object();

        private Timer simulationTimer_;
        private UInt16 simulationInterval_ = 1000;
        private bool simulationEnabled_ = true;
        private List<BaseDataVariableState> dynamicNodes_;

        private MachineState machine1_;
        private MachineState machine2_;
        private FolderState root_;
        #endregion
    }
}