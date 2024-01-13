#region Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    public partial class StructureValueObjectState
    {
        #region Initialization
        /// <summary>
        /// Initializes the object with structures.
        /// </summary>
        protected override void OnAfterCreate(ISystemContext context, NodeState node)
        {
            base.OnAfterCreate(context, node);

            InitializeVariable(context, ScalarStructure, TestData.Variables.StructureValueObjectType_ScalarStructure);
            InitializeVariable(context, VectorStructure, TestData.Variables.StructureValueObjectType_VectorStructure);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Handles the generate values method.
        /// </summary>
        protected override ServiceResult OnGenerateValues(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint count)
        {
            TestDataSystem system = context.SystemHandle as TestDataSystem;

            if (system == null)
            {
                return StatusCodes.BadOutOfService;
            }

            ScalarStructure.OnGenerateValues(context);
            VectorStructure.OnGenerateValues(context);

            return base.OnGenerateValues(context, method, objectId, count);
        }
        #endregion
    }
}
