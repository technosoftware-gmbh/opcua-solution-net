#region Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace Technosoftware.SimpleServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class SimpleServerNodeManager : UaBaseNodeManager
    {
        #region Private Fields
        private SimpleServerConfiguration configuration_;
        private Opc.Ua.Test.DataGenerator generator_;
        private Timer simulationTimer_;
        private UInt16 simulationInterval_ = 1000;
        private bool simulationEnabled_ = true;
        private List<BaseDataVariableState> dynamicNodes_;

        private readonly IUaServer opcServer_;
        private readonly IUaServerPlugin opcServerPlugin_;

        // Track whether Dispose has been called.
        private bool disposed_;
        private readonly object lockDisposable_ = new object();

        #endregion

        #region Constructors, Destructor, Initialization

        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public SimpleServerNodeManager(IUaServer opcServer,
            IUaServerPlugin opcServerPlugin,
            IUaServerData uaServer,
            ApplicationConfiguration configuration,
            params string[] namespaceUris)
            : base(uaServer, configuration, namespaceUris)
        {
            opcServer_ = opcServer;
            opcServerPlugin_ = opcServerPlugin;
            dynamicNodes_ = new List<BaseDataVariableState>();

            // get the configuration for the node manager. In case no configuration exists
            // use suitable defaults.
            configuration_ = configuration.ParseExtension<SimpleServerConfiguration>() ??
                             new SimpleServerConfiguration();

            var configurationFile = configuration_.ConfigurationFile;

            Console.WriteLine("Specified configuration file: {0}", configurationFile);

            SystemContext.NodeIdFactory = this;

        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructor in types derived from this class.
        /// </summary>
        ~SimpleServerNodeManager()
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
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            if (node is BaseInstanceState instance && instance.Parent != null)
            {
                if (instance.Parent.NodeId.Identifier is string id)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }

            return node.NodeId;
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
                dynamicNodes_ = new List<BaseDataVariableState>();

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = References = new List<IReference>();
                }
                else
                {
                    References = references;
                }

                var root = CreateFolderState(null, "My Data", new LocalizedText("en", "My Data"), new LocalizedText("en", "Root folder of Workshop Server"));
                References.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
                root.EventNotifier = EventNotifiers.SubscribeToEvents;
                opcServer_.AddRootNotifier(root);

                try
                {
                    #region Scalar_Static
                    var scalarFolder = CreateFolderState(root, "Scalar", "Scalar", null);
                    var scalarInstructions = CreateVariable(scalarFolder, "Scalar_Instructions", "Scalar_Instructions", DataTypeIds.String, ValueRanks.Scalar);
                    scalarInstructions.Value = "A library of Variables of different data-types.";
                    var staticFolder = CreateFolderState(scalarFolder, "Scalar_Static", "Scalar_Static", null);
                    const string scalarStatic = "Scalar_Static_";
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Time", "Time", null, DataTypeIds.Time, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    CreateBaseDataVariableState(staticFolder, scalarStatic + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

                    var decimalVariable = CreateBaseDataVariableState(staticFolder, scalarStatic + "Decimal", "Decimal", null, DataTypeIds.DecimalDataType, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    // Set an arbitrary precision decimal value.
                    var largeInteger = BigInteger.Parse("1234567890123546789012345678901234567890123456789012345");
                    var decimalValue = new DecimalDataType
                    {
                        Scale = 100, Value = largeInteger.ToByteArray()
                    };
                    decimalVariable.Value = decimalValue;
                    #endregion

                    #region Scalar_Simulation
                    var simulationFolder = CreateFolderState(scalarFolder, "Scalar_Simulation", "Simulation", null);
                    const string scalarSimulation = "Scalar_Simulation_";
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Boolean", "Boolean", DataTypeIds.Boolean, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Byte", "Byte", DataTypeIds.Byte, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "ByteString", "ByteString", DataTypeIds.ByteString, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "DateTime", "DateTime", DataTypeIds.DateTime, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Double", "Double", DataTypeIds.Double, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Duration", "Duration", DataTypeIds.Duration, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Float", "Float", DataTypeIds.Float, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Guid", "Guid", DataTypeIds.Guid, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Int16", "Int16", DataTypeIds.Int16, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Int32", "Int32", DataTypeIds.Int32, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Int64", "Int64", DataTypeIds.Int64, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Integer", "Integer", DataTypeIds.Integer, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "LocaleId", "LocaleId", DataTypeIds.LocaleId, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "LocalizedText", "LocalizedText", DataTypeIds.LocalizedText, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "NodeId", "NodeId", DataTypeIds.NodeId, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Number", "Number", DataTypeIds.Number, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "QualifiedName", "QualifiedName", DataTypeIds.QualifiedName, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "SByte", "SByte", DataTypeIds.SByte, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "String", "String", DataTypeIds.String, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Time", "Time", DataTypeIds.Time, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt16", "UInt16", DataTypeIds.UInt16, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt32", "UInt32", DataTypeIds.UInt32, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt64", "UInt64", DataTypeIds.UInt64, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInteger", "UInteger", DataTypeIds.UInteger, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UtcTime", "UtcTime", DataTypeIds.UtcTime, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Variant", "Variant", BuiltInType.Variant, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "XmlElement", "XmlElement", DataTypeIds.XmlElement, ValueRanks.Scalar);

                    var intervalVariable = CreateVariable(simulationFolder, scalarSimulation + "Interval", "Interval", DataTypeIds.UInt16, ValueRanks.Scalar);
                    intervalVariable.Value = simulationInterval_;
                    intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                    var enabledVariable = CreateVariable(simulationFolder, scalarSimulation + "Enabled", "Enabled", DataTypeIds.Boolean, ValueRanks.Scalar);
                    enabledVariable.Value = simulationEnabled_;
                    enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                    #endregion

                    #region Methods
                    var methodsFolder = CreateFolderState(root, "Methods", "Methods", null);
                    const string methods = "Methods_";

                    var methodsInstructions = CreateVariable(methodsFolder, methods + "Instructions", "Instructions", DataTypeIds.String, ValueRanks.Scalar);
                    methodsInstructions.Value = "Contains methods with varying parameter definitions.";

                    #region Hello Method
                    var helloMethod = CreateMethodState(methodsFolder, methods + "Hello", "Hello", OnHelloCall);
                    // set input arguments
                    helloMethod.InputArguments = new PropertyState<Argument[]>(helloMethod);
                    helloMethod.InputArguments.NodeId = new NodeId(helloMethod.BrowseName.Name + "InArgs", NamespaceIndex);
                    helloMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
                    helloMethod.InputArguments.DisplayName = helloMethod.InputArguments.BrowseName.Name;
                    helloMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    helloMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    helloMethod.InputArguments.DataType = DataTypeIds.Argument;
                    helloMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    helloMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "String value", Description = "String value",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    helloMethod.OutputArguments = new PropertyState<Argument[]>(helloMethod);
                    helloMethod.OutputArguments.NodeId = new NodeId(helloMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    helloMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    helloMethod.OutputArguments.DisplayName = helloMethod.OutputArguments.BrowseName.Name;
                    helloMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    helloMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    helloMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    helloMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    helloMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Hello Result", Description = "Hello Result",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
                    };
                    #endregion

                    #endregion

                    #region Access Rights Handling
                    var folderAccessRights = CreateFolderState(root, "AccessRights", "AccessRights", null);
                    const string accessRights = "AccessRights_";

                    var accessRightsInstructions = CreateVariable(folderAccessRights, accessRights + "Instructions", "Instructions", DataTypeIds.String, ValueRanks.Scalar);
                    accessRightsInstructions.Value = "This folder will be accessible to all authenticated users who enter, but contents therein will be secured.";


                    #region Access Rights Operator Handling
                    // sub-folder for "AccessOperator"
                    var folderAccessRightsAccessOperator = CreateFolderState(folderAccessRights, "AccessRights_AccessOperator", "AccessOperator", null);
                    const string accessRightsAccessOperator = "AccessRights_AccessOperator_";

                    var arOperatorRW = CreateVariable(folderAccessRightsAccessOperator, accessRightsAccessOperator + "OperatorUsable", "OperatorUsable", BuiltInType.Int16, ValueRanks.Scalar);
                    arOperatorRW.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arOperatorRW.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    arOperatorRW.OnReadUserAccessLevel = OnReadOperatorUserAccessLevel;
                    arOperatorRW.OnSimpleWriteValue = OnWriteOperatorValue;
                    arOperatorRW.OnReadValue = OnReadOperatorValue;
                    dynamicNodes_.Add(arOperatorRW);
                    #endregion

                    #region Access Rights Administrator Handling
                    // sub-folder for "AccessAdministrator"
                    var folderAccessRightsAccessAdministrator = CreateFolderState(folderAccessRights, "AccessRights_AccessAdministrator", "AccessAdministrator", null);
                    const string accessRightsAccessAdministrator = "AccessRights_AccessAdministrator_";

                    var arAdministratorRW = CreateVariable(folderAccessRightsAccessAdministrator, accessRightsAccessAdministrator + "AdministratorOnly", "AdministratorOnly", BuiltInType.Int16, ValueRanks.Scalar);
                    arAdministratorRW.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arAdministratorRW.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    arAdministratorRW.OnReadUserAccessLevel = OnReadAdministratorUserAccessLevel;
                    arAdministratorRW.OnSimpleWriteValue = OnWriteAdministratorValue;
                    arAdministratorRW.OnReadValue = OnReadAdministratorValue;
                    dynamicNodes_.Add(arAdministratorRW);
                    #endregion
                    #endregion
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Error creating the address space.");
                }

                AddPredefinedNode(SystemContext, root);
                simulationTimer_ = new Timer(DoSimulation, null, 1000, 1000);
            }
        }
        #endregion

        #region Browse Support Functions
        /// <summary>
        /// Checks if the user is allowed to access this node.
        /// </summary>
        protected override bool IsNodeAccessibleForUser(UaServerContext context, UaContinuationPoint continuationPoint, NodeState node)
        {
            if (context.UserIdentity == null || context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                if ((node.NodeId.Identifier.ToString() == "AccessAdministrator"))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the user is allowed to access this reference.
        /// </summary>
        protected override bool IsReferenceAccessibleForUser(UaServerContext context, UaContinuationPoint continuationPoint, IReference reference)
        {
            if (context.UserIdentity == null || context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                if ((reference.TargetId.Identifier.ToString() == "AccessRights_AccessAdministrator"))
                {
                    return false;
                }
            }

            return true;
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

                if (simulationEnabled_)
                {
                    simulationTimer_.Change(100, simulationInterval_);
                }
                else
                {
                    simulationTimer_.Change(100, 0);
                }

                return ServiceResult.Good;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error writing Enabled variable.");
                return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Enabled variable.");
            }
        }

        #region Operator specific handling
        public ServiceResult OnReadOperatorUserAccessLevel(ISystemContext context, NodeState node, ref byte value)
        {
            // If user identity is not set default user access level handling should apply

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                value = AccessLevels.None;
            }
            else
            {
                if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.UserName)
                {
                    var user = context.UserIdentity.GetIdentityToken() as UserNameIdentityToken;
                    if ((user.UserName == "operator") ||
                        (user.UserName == "administrator"))
                    {
                        value = AccessLevels.CurrentReadOrWrite;
                    }
                    else
                    {
                        value = AccessLevels.None;
                    }
                }
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnReadOperatorValue(
        ISystemContext context,
        NodeState node,
        NumericRange indexRange,
        QualifiedName dataEncoding,
        ref object value,
        ref StatusCode statusCode,
        ref DateTime timestamp)
        {
            // If user identity is not set default user access level handling should apply

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                return StatusCodes.BadUserAccessDenied;
            }

            return ServiceResult.Good;
        }

        public ServiceResult OnWriteOperatorValue(ISystemContext context, NodeState node, ref object value)
        {
            // If user identity is not set default user access level handling should apply

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                return StatusCodes.BadUserAccessDenied;
            }

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.UserName)
            {
                var user = context.UserIdentity.GetIdentityToken() as UserNameIdentityToken;
                if ((user.UserName != "operator") &&
                    (user.UserName != "administrator"))
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }

            return ServiceResult.Good;
        }
        #endregion

        #region Administrator specific handling
        public ServiceResult OnReadAdministratorUserAccessLevel(ISystemContext context, NodeState node, ref byte value)
        {
            // If user identity is not set default user access level handling should apply

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                value = AccessLevels.None;
            }
            else
            {
                if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.UserName)
                {
                    var user = context.UserIdentity.GetIdentityToken() as UserNameIdentityToken;
                    if (user.UserName == "administrator")
                    {
                        value = AccessLevels.CurrentReadOrWrite;
                    }
                    else
                    {
                        value = AccessLevels.None;
                    }
                }
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnReadAdministratorValue(
                                ISystemContext context,
                                NodeState node,
                                NumericRange indexRange,
                                QualifiedName dataEncoding,
                                ref object value,
                                ref StatusCode statusCode,
                                ref DateTime timestamp)
        {
            // If user identity is not set default user access level handling should apply

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                return StatusCodes.BadUserAccessDenied;
            }

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.UserName)
            {
                var user = context.UserIdentity.GetIdentityToken() as UserNameIdentityToken;
                if (user.UserName != "administrator")
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }

            return ServiceResult.Good;
        }

        public ServiceResult OnWriteAdministratorValue(ISystemContext context, NodeState node, ref object value)
        {
            // If user identity is not set default user access level handling should apply

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                return StatusCodes.BadUserAccessDenied;
            }

            if (context.UserIdentity != null && context.UserIdentity.TokenType == UserTokenType.UserName)
            {
                var user = context.UserIdentity.GetIdentityToken() as UserNameIdentityToken;
                if (user.UserName != "administrator")
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }

            return ServiceResult.Good;
        }
        #endregion

        private ServiceResult OnHelloCall(ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            // all arguments must be provided.
            if (inputArguments.Count < 1)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                var op1 = (string)inputArguments[0];

                // set output parameter
                outputArguments[0] = (string)("hello " + op1);
                return ServiceResult.Good;
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, BuiltInType dataType, int valueRank)
        {
            return CreateBaseDataVariableState(parent, path, name, null, dataType, valueRank, AccessLevels.CurrentReadOrWrite, null);
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            return CreateBaseDataVariableState(parent, path, name, null, dataType, valueRank, AccessLevels.CurrentReadOrWrite, null);
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, BuiltInType dataType, int valueRank)
        {
            return CreateDynamicVariable(parent, path, name, (uint)dataType, valueRank);
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            var variable = CreateVariable(parent, path, name, dataType, valueRank);
            dynamicNodes_.Add(variable);
            return variable;
        }

        private BaseDataVariableState[] CreateDynamicVariables(NodeState parent, string path, string name, BuiltInType dataType, int valueRank, uint numVariables)
        {
            return CreateDynamicVariables(parent, path, name, (uint)dataType, valueRank, numVariables);

        }

        private BaseDataVariableState[] CreateDynamicVariables(NodeState parent, string path, string name, NodeId dataType, int valueRank, uint numVariables)
        {
            // first, create a new Parent folder for this data-type
            var newParentFolder = CreateFolderState(parent, path, name, null);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                var newName = $"{name}_{i:00}";
                var newPath = $"{path}_{newName}";
                itemsCreated.Add(CreateDynamicVariable(newParentFolder, newPath, newName, dataType, valueRank));
            }
            return (itemsCreated.ToArray());
        }

        private object GetNewValue(BaseVariableState variable)
        {
            if (generator_ == null)
            {
                generator_ = new Opc.Ua.Test.DataGenerator(null);
                generator_.BoundaryValueFrequency = 0;
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
                        //variable.Value = GetNewValue(variable);
                        //variable.ClearChangeMasks(SystemContext, false);
                        opcServer_.WriteBaseVariable(variable, GetNewValue(variable), StatusCodes.Good, DateTime.UtcNow);
                    }
                    OnRaiseSystemEvents();
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error doing simulation.");
            }
        }

        private void OnRaiseSystemEvents()
        {
            SystemEventState eventState = null;
            try
            { 
                eventState = new SystemEventState(null);

                eventState.Initialize(
                    SystemContext,
                    null,
                    EventSeverity.Medium,
                    new LocalizedText("Raising Events"));

                eventState.SetChildValue(SystemContext, BrowseNames.SourceNode, ObjectIds.Server, false);
                eventState.SetChildValue(SystemContext, BrowseNames.SourceName, "Internal", false);

                ServerData.ReportEvent(eventState);

                var ae = new AuditEventState(null);

                ae.Initialize(
                    SystemContext,
                    null,
                    EventSeverity.Medium,
                    new LocalizedText("Events Raised"),
                    true,
                    DateTime.UtcNow);

                ae.SetChildValue(SystemContext, BrowseNames.SourceNode, ObjectIds.Server, false);
                ae.SetChildValue(SystemContext, BrowseNames.SourceName, "Internal", false);

                ServerData.ReportEvent(ae);
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error in OnRaiseSystemEvents");
            }
            finally
            {
                eventState?.Dispose();
            }
        }
        #endregion
    }
}