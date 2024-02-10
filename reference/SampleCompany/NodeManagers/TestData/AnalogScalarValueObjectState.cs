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
using System.Xml;
using System.IO;
using System.Reflection;
using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    public partial class AnalogScalarValueObjectState
    {
        #region Initialization
        /// <summary>
        /// Initializes the object as a collection of counters which change value on read.
        /// </summary>
        protected override void OnAfterCreate(ISystemContext context, NodeState node)
        {
            base.OnAfterCreate(context, node);

            InitializeVariable(context, SByteValue, Variables.AnalogScalarValueObjectType_SByteValue);
            InitializeVariable(context, ByteValue, Variables.AnalogScalarValueObjectType_ByteValue);
            InitializeVariable(context, Int16Value, Variables.AnalogScalarValueObjectType_Int16Value);
            InitializeVariable(context, UInt16Value, Variables.AnalogScalarValueObjectType_UInt16Value);
            InitializeVariable(context, Int32Value, Variables.AnalogScalarValueObjectType_Int32Value);
            InitializeVariable(context, UInt32Value, Variables.AnalogScalarValueObjectType_UInt32Value);
            InitializeVariable(context, Int64Value, Variables.AnalogScalarValueObjectType_Int64Value);
            InitializeVariable(context, UInt64Value, Variables.AnalogScalarValueObjectType_UInt64Value);
            InitializeVariable(context, FloatValue, Variables.AnalogScalarValueObjectType_FloatValue);
            InitializeVariable(context, DoubleValue, Variables.AnalogScalarValueObjectType_DoubleValue);
            InitializeVariable(context, NumberValue, Variables.AnalogScalarValueObjectType_NumberValue);
            InitializeVariable(context, IntegerValue, Variables.AnalogScalarValueObjectType_IntegerValue);
            InitializeVariable(context, UIntegerValue, Variables.AnalogScalarValueObjectType_UIntegerValue);
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
            var system = context.SystemHandle as TestDataSystem;

            if (system == null)
            {
                return StatusCodes.BadOutOfService;
            }

            GenerateValue(system, SByteValue);
            GenerateValue(system, ByteValue);
            GenerateValue(system, Int16Value);
            GenerateValue(system, UInt16Value);
            GenerateValue(system, Int32Value);
            GenerateValue(system, UInt32Value);
            GenerateValue(system, UInt32Value);
            GenerateValue(system, Int64Value);
            GenerateValue(system, UInt64Value);
            GenerateValue(system, FloatValue);
            GenerateValue(system, DoubleValue);
            GenerateValue(system, NumberValue);
            GenerateValue(system, IntegerValue);
            GenerateValue(system, UIntegerValue);

            return base.OnGenerateValues(context, method, objectId, count);
        }
        #endregion
    }
}
