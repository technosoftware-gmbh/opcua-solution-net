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
    public partial class VectorVariableState : ITestDataSystemValuesGenerator
    {
        #region Initialization
        /// <summary>
        /// Initializes the object as a collection of counters which change value on read.
        /// </summary>
        protected override void OnAfterCreate(ISystemContext context, NodeState node)
        {
            base.OnAfterCreate(context, node);

            InitializeVariable(context, X);
            InitializeVariable(context, Y);
            InitializeVariable(context, Z);
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
