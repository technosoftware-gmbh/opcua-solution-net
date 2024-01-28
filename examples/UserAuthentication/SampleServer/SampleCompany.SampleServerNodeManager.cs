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

using Opc.Ua;
using Technosoftware.UaServer;
using Technosoftware.UaBaseServer;
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

        private Timer simulationTimer_;
        private ushort simulationInterval_ = 1000;
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

                    var intervalVariable = CreateBaseDataVariableState(simulationFolder, simulation + "Interval", "Interval", "The Interval used for changing the simulated values.", DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, simulationInterval_);
                    intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                    var enabledVariable = CreateBaseDataVariableState(simulationFolder, simulation + "Enabled", "Enabled", "Specifies whether the simulation is enabled (true) or disabled (false).", DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, simulationEnabled_);
                    enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                    #endregion

                    #region Access Rights Handling
                    var folderAccessRights = CreateFolderState(root, "AccessRights", "AccessRights", null);
                    const string accessRights = "AccessRights_";
                    var accessRightsInstructions = CreateBaseDataVariableState(folderAccessRights, accessRights + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessRightsInstructions.Value = "This folder will be accessible to all authenticated users who enter, but contents therein will be secured.";


                    #region Access Rights Operator Handling
                    // sub-folder for "AccessOperator"
                    var folderAccessRightsAccessOperator = CreateFolderState(folderAccessRights, "AccessRights_AccessOperator", "AccessOperator", null);
                    const string accessRightsAccessOperator = "AccessRights_AccessOperator_";

                    var arOperatorRW = CreateBaseDataVariableState(folderAccessRightsAccessOperator, accessRightsAccessOperator + "OperatorUsable", "OperatorUsable", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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

                    var arAdministratorRW = CreateBaseDataVariableState(folderAccessRightsAccessAdministrator, accessRightsAccessAdministrator + "AdministratorOnly", "AdministratorOnly", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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
                // Add all nodes under root to the server
                AddPredefinedNode(SystemContext, root);
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
                    if (context.UserIdentity.GetIdentityToken() is UserNameIdentityToken user && 
                        ((user.UserName == "operator") || (user.UserName == "administrator")))
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
                if (context.UserIdentity.GetIdentityToken() is UserNameIdentityToken user && 
                    (user.UserName != "operator") && (user.UserName != "administrator"))
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
                    if (context.UserIdentity.GetIdentityToken() is UserNameIdentityToken user && user.UserName == "administrator")
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
                if (context.UserIdentity.GetIdentityToken() is UserNameIdentityToken user && user.UserName != "administrator")
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
                if (context.UserIdentity.GetIdentityToken() is UserNameIdentityToken user && user.UserName != "administrator")
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }
            return ServiceResult.Good;
        }
        #endregion

        #region User specific Browse handling
        /// <summary>
        /// Checks if the user is allowed to access this node.
        /// </summary>
        protected override bool IsNodeAccessibleForUser(UaServerContext context, UaContinuationPoint continuationPoint, NodeState node)
        {
            if (context.UserIdentity == null || context.UserIdentity.TokenType == UserTokenType.Anonymous)
            {
                if ((node.NodeId.Identifier.ToString() == "AccessAdministrator") ||
                    (node.NodeId.Identifier.ToString() == "AccessOperator"))
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
                if ((reference.TargetId.Identifier.ToString() == "AccessRights_AccessAdministrator") ||
                    (reference.TargetId.Identifier.ToString() == "AccessRights_AccessOperator"))
                {
                    return false;
                }
            }
            return true;
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