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
    public partial class ArrayValueObjectState
    {
        #region Initialization
        /// <summary>
        /// Initializes the object as a collection of counters which change value on read.
        /// </summary>
        protected override void OnAfterCreate(ISystemContext context, NodeState node)
        {
            base.OnAfterCreate(context, node);

            InitializeVariable(context, BooleanValue, Variables.ArrayValueObjectType_BooleanValue);
            InitializeVariable(context, SByteValue, Variables.ArrayValueObjectType_SByteValue);
            InitializeVariable(context, ByteValue, Variables.ArrayValueObjectType_ByteValue);
            InitializeVariable(context, Int16Value, Variables.ArrayValueObjectType_Int16Value);
            InitializeVariable(context, UInt16Value, Variables.ArrayValueObjectType_UInt16Value);
            InitializeVariable(context, Int32Value, Variables.ArrayValueObjectType_Int32Value);
            InitializeVariable(context, UInt32Value, Variables.ArrayValueObjectType_UInt32Value);
            InitializeVariable(context, Int64Value, Variables.ArrayValueObjectType_Int64Value);
            InitializeVariable(context, UInt64Value, Variables.ArrayValueObjectType_UInt64Value);
            InitializeVariable(context, FloatValue, Variables.ArrayValueObjectType_FloatValue);
            InitializeVariable(context, DoubleValue, Variables.ArrayValueObjectType_DoubleValue);
            InitializeVariable(context, StringValue, Variables.ArrayValueObjectType_StringValue);
            InitializeVariable(context, DateTimeValue, Variables.ArrayValueObjectType_DateTimeValue);
            InitializeVariable(context, GuidValue, Variables.ArrayValueObjectType_GuidValue);
            InitializeVariable(context, ByteStringValue, Variables.ArrayValueObjectType_ByteStringValue);
            InitializeVariable(context, XmlElementValue, Variables.ArrayValueObjectType_XmlElementValue);
            InitializeVariable(context, NodeIdValue, Variables.ArrayValueObjectType_NodeIdValue);
            InitializeVariable(context, ExpandedNodeIdValue, Variables.ArrayValueObjectType_ExpandedNodeIdValue);
            InitializeVariable(context, QualifiedNameValue, Variables.ArrayValueObjectType_QualifiedNameValue);
            InitializeVariable(context, LocalizedTextValue, Variables.ArrayValueObjectType_LocalizedTextValue);
            InitializeVariable(context, StatusCodeValue, Variables.ArrayValueObjectType_StatusCodeValue);
            InitializeVariable(context, VariantValue, Variables.ArrayValueObjectType_VariantValue);
            InitializeVariable(context, EnumerationValue, Variables.ArrayValueObjectType_EnumerationValue);
            InitializeVariable(context, StructureValue, Variables.ArrayValueObjectType_StructureValue);
            InitializeVariable(context, NumberValue, Variables.ArrayValueObjectType_NumberValue);
            InitializeVariable(context, IntegerValue, Variables.ArrayValueObjectType_IntegerValue);
            InitializeVariable(context, UIntegerValue, Variables.ArrayValueObjectType_UIntegerValue);
            InitializeVariable(context, VectorValue, Variables.ArrayValueObjectType_VectorValue);
            InitializeVariable(context, VectorUnionValue, Variables.ArrayValueObjectType_VectorUnionValue);
            InitializeVariable(context, VectorWithOptionalFieldsValue, Variables.ArrayValueObjectType_VectorWithOptionalFieldsValue);
            InitializeVariable(context, MultipleVectorsValue, Variables.ArrayValueObjectType_MultipleVectorsValue);
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

            GenerateValue(system, BooleanValue);
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
            GenerateValue(system, StringValue);
            GenerateValue(system, DateTimeValue);
            GenerateValue(system, GuidValue);
            GenerateValue(system, ByteStringValue);
            GenerateValue(system, XmlElementValue);
            GenerateValue(system, NodeIdValue);
            GenerateValue(system, ExpandedNodeIdValue);
            GenerateValue(system, QualifiedNameValue);
            GenerateValue(system, LocalizedTextValue);
            GenerateValue(system, StatusCodeValue);
            GenerateValue(system, VariantValue);
            GenerateValue(system, EnumerationValue);
            GenerateValue(system, StructureValue);
            GenerateValue(system, NumberValue);
            GenerateValue(system, IntegerValue);
            GenerateValue(system, UIntegerValue);
            GenerateValue(system, VectorValue);
            GenerateValue(system, VectorUnionValue);
            GenerateValue(system, VectorWithOptionalFieldsValue);
            GenerateValue(system, MultipleVectorsValue);

            return base.OnGenerateValues(context, method, objectId, count);
        }
        #endregion
    }
}
