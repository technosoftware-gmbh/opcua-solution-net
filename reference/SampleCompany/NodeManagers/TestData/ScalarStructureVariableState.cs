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
using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    public partial class ScalarStructureVariableState : ITestDataSystemValuesGenerator
    {
        #region Initialization
        /// <summary>
        /// Initializes the object as a collection of counters which change value on read.
        /// </summary>
        protected override void OnAfterCreate(ISystemContext context, NodeState node)
        {
            base.OnAfterCreate(context, node);

            InitializeVariable(context, BooleanValue);
            InitializeVariable(context, SByteValue);
            InitializeVariable(context, ByteValue);
            InitializeVariable(context, Int16Value);
            InitializeVariable(context, UInt16Value);
            InitializeVariable(context, Int32Value);
            InitializeVariable(context, UInt32Value);
            InitializeVariable(context, Int64Value);
            InitializeVariable(context, UInt64Value);
            InitializeVariable(context, FloatValue);
            InitializeVariable(context, DoubleValue);
            InitializeVariable(context, StringValue);
            InitializeVariable(context, DateTimeValue);
            InitializeVariable(context, GuidValue);
            InitializeVariable(context, ByteStringValue);
            InitializeVariable(context, XmlElementValue);
            InitializeVariable(context, NodeIdValue);
            InitializeVariable(context, ExpandedNodeIdValue);
            InitializeVariable(context, QualifiedNameValue);
            InitializeVariable(context, LocalizedTextValue);
            InitializeVariable(context, StatusCodeValue);
            InitializeVariable(context, VariantValue);
            InitializeVariable(context, EnumerationValue);
            InitializeVariable(context, StructureValue);
            InitializeVariable(context, NumberValue);
            InitializeVariable(context, IntegerValue);
            InitializeVariable(context, UIntegerValue);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Initializes the variable.
        /// </summary>
        protected void InitializeVariable(ISystemContext context, BaseVariableState variable)
        {
            // set a valid initial value.
            TestDataSystem system = context.SystemHandle as TestDataSystem;

            // copy access level to childs
            variable.AccessLevel = AccessLevel;
            variable.UserAccessLevel = UserAccessLevel;
        }
        #endregion

        #region Public Methods
        public virtual StatusCode OnGenerateValues(ISystemContext context)
        {
            TestDataSystem system = context.SystemHandle as TestDataSystem;

            if (system == null)
            {
                return StatusCodes.BadOutOfService;
            }

            var accessLevel = AccessLevel;
            var userAccessLevel = UserAccessLevel;
            AccessLevel = UserAccessLevel = AccessLevels.CurrentReadOrWrite;

            // generate structure values here
            ServiceResult result = WriteValueAttribute(context, NumericRange.Empty, system.ReadValue(this), StatusCodes.Good, DateTime.UtcNow);

            AccessLevel = accessLevel;
            UserAccessLevel = userAccessLevel;

            ClearChangeMasks(context, true);

            return result.StatusCode;
        }
        #endregion
    }
}
