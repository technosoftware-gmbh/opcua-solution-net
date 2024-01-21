/* ========================================================================
 * Copyright (c) 2005-2021 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace SampleCompany.NodeManagers.TestData
{
    #region DataType Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypes
    {
        /// <remarks />
        public const uint ScalarStructureDataType = 78;

        /// <remarks />
        public const uint ArrayValueDataType = 446;

        /// <remarks />
        public const uint BooleanDataType = 688;

        /// <remarks />
        public const uint SByteDataType = 689;

        /// <remarks />
        public const uint ByteDataType = 690;

        /// <remarks />
        public const uint Int16DataType = 691;

        /// <remarks />
        public const uint UInt16DataType = 692;

        /// <remarks />
        public const uint Int32DataType = 693;

        /// <remarks />
        public const uint UInt32DataType = 694;

        /// <remarks />
        public const uint Int64DataType = 695;

        /// <remarks />
        public const uint UInt64DataType = 696;

        /// <remarks />
        public const uint FloatDataType = 697;

        /// <remarks />
        public const uint DoubleDataType = 698;

        /// <remarks />
        public const uint StringDataType = 699;

        /// <remarks />
        public const uint DateTimeDataType = 700;

        /// <remarks />
        public const uint GuidDataType = 701;

        /// <remarks />
        public const uint ByteStringDataType = 702;

        /// <remarks />
        public const uint XmlElementDataType = 703;

        /// <remarks />
        public const uint NodeIdDataType = 704;

        /// <remarks />
        public const uint ExpandedNodeIdDataType = 705;

        /// <remarks />
        public const uint QualifiedNameDataType = 706;

        /// <remarks />
        public const uint LocalizedTextDataType = 707;

        /// <remarks />
        public const uint StatusCodeDataType = 708;

        /// <remarks />
        public const uint VariantDataType = 709;

        /// <remarks />
        public const uint UserScalarValueDataType = 710;

        /// <remarks />
        public const uint UserArrayValueDataType = 802;

        /// <remarks />
        public const uint Vector = 888;

        /// <remarks />
        public const uint VectorUnion = 2588;

        /// <remarks />
        public const uint VectorWithOptionalFields = 2589;

        /// <remarks />
        public const uint MultipleVectors = 2590;

        /// <remarks />
        public const uint WorkOrderStatusType = 893;

        /// <remarks />
        public const uint WorkOrderType = 894;
    }
    #endregion

    #region Method Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Methods
    {
        /// <remarks />
        public const uint TestDataObjectType_GenerateValues = 17;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Disable = 52;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Enable = 53;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_AddComment = 54;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Acknowledge = 74;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Disable = 153;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Enable = 154;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_AddComment = 155;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Acknowledge = 175;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Disable = 247;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Enable = 248;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_AddComment = 249;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Acknowledge = 269;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Disable = 342;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Enable = 343;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_AddComment = 344;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Acknowledge = 364;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Disable = 493;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Enable = 494;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_AddComment = 495;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Acknowledge = 515;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Disable = 584;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Enable = 585;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_AddComment = 586;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Acknowledge = 606;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Disable = 748;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Enable = 749;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_AddComment = 750;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Acknowledge = 770;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Disable = 840;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Enable = 841;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_AddComment = 842;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Acknowledge = 862;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod1 = 902;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod2 = 905;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod3 = 908;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod1 = 911;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod2 = 914;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod3 = 917;

        /// <remarks />
        public const uint MethodTestType_UserScalarMethod1 = 920;

        /// <remarks />
        public const uint MethodTestType_UserScalarMethod2 = 923;

        /// <remarks />
        public const uint MethodTestType_UserArrayMethod1 = 926;

        /// <remarks />
        public const uint MethodTestType_UserArrayMethod2 = 929;

        /// <remarks />
        public const uint Data_Static_Scalar_GenerateValues = 978;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Disable = 1013;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Enable = 1014;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_AddComment = 1015;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Acknowledge = 1035;

        /// <remarks />
        public const uint Data_Static_Structure_GenerateValues = 1072;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Disable = 1107;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Enable = 1108;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_AddComment = 1109;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Acknowledge = 1129;

        /// <remarks />
        public const uint Data_Static_Array_GenerateValues = 1167;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Disable = 1202;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Enable = 1203;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_AddComment = 1204;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Acknowledge = 1224;

        /// <remarks />
        public const uint Data_Static_UserScalar_GenerateValues = 1258;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Disable = 1293;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Enable = 1294;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_AddComment = 1295;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Acknowledge = 1315;

        /// <remarks />
        public const uint Data_Static_UserArray_GenerateValues = 1343;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Disable = 1378;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Enable = 1379;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_AddComment = 1380;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Acknowledge = 1400;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_GenerateValues = 1428;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Disable = 1463;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Enable = 1464;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_AddComment = 1465;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Acknowledge = 1485;

        /// <remarks />
        public const uint Data_Static_AnalogArray_GenerateValues = 1569;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Disable = 1604;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Enable = 1605;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_AddComment = 1606;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Acknowledge = 1626;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod1 = 1709;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod2 = 1712;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod3 = 1715;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod1 = 1718;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod2 = 1721;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod3 = 1724;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserScalarMethod1 = 1727;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserScalarMethod2 = 1730;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserArrayMethod1 = 1733;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserArrayMethod2 = 1736;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_GenerateValues = 1742;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Disable = 1777;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Enable = 1778;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_AddComment = 1779;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Acknowledge = 1799;

        /// <remarks />
        public const uint Data_Dynamic_Structure_GenerateValues = 1836;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Disable = 1871;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Enable = 1872;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_AddComment = 1873;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Acknowledge = 1893;

        /// <remarks />
        public const uint Data_Dynamic_Array_GenerateValues = 1931;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Disable = 1966;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Enable = 1967;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_AddComment = 1968;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Acknowledge = 1988;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_GenerateValues = 2022;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Disable = 2057;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Enable = 2058;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_AddComment = 2059;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Acknowledge = 2079;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_GenerateValues = 2107;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Disable = 2142;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Enable = 2143;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_AddComment = 2144;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Acknowledge = 2164;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_GenerateValues = 2192;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Disable = 2227;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Enable = 2228;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_AddComment = 2229;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Acknowledge = 2249;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_GenerateValues = 2333;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Disable = 2368;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Enable = 2369;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_AddComment = 2370;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Acknowledge = 2390;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Disable = 2506;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Enable = 2507;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_AddComment = 2508;
    }
    #endregion

    #region Object Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <remarks />
        public const uint TestDataObjectType_CycleComplete = 19;

        /// <remarks />
        public const uint Data = 974;

        /// <remarks />
        public const uint Data_Static = 975;

        /// <remarks />
        public const uint Data_Static_Scalar = 976;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete = 980;

        /// <remarks />
        public const uint Data_Static_Structure = 1070;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete = 1074;

        /// <remarks />
        public const uint Data_Static_Array = 1165;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete = 1169;

        /// <remarks />
        public const uint Data_Static_UserScalar = 1256;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete = 1260;

        /// <remarks />
        public const uint Data_Static_UserArray = 1341;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete = 1345;

        /// <remarks />
        public const uint Data_Static_AnalogScalar = 1426;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete = 1430;

        /// <remarks />
        public const uint Data_Static_AnalogArray = 1567;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete = 1571;

        /// <remarks />
        public const uint Data_Static_MethodTest = 1708;

        /// <remarks />
        public const uint Data_Dynamic = 1739;

        /// <remarks />
        public const uint Data_Dynamic_Scalar = 1740;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete = 1744;

        /// <remarks />
        public const uint Data_Dynamic_Structure = 1834;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete = 1838;

        /// <remarks />
        public const uint Data_Dynamic_Array = 1929;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete = 1933;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar = 2020;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete = 2024;

        /// <remarks />
        public const uint Data_Dynamic_UserArray = 2105;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete = 2109;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar = 2190;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete = 2194;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray = 2331;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete = 2335;

        /// <remarks />
        public const uint Data_Conditions = 2472;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus = 2473;

        /// <remarks />
        public const uint ScalarStructureDataType_Encoding_DefaultBinary = 2511;

        /// <remarks />
        public const uint ArrayValueDataType_Encoding_DefaultBinary = 2512;

        /// <remarks />
        public const uint UserScalarValueDataType_Encoding_DefaultBinary = 2513;

        /// <remarks />
        public const uint UserArrayValueDataType_Encoding_DefaultBinary = 2514;

        /// <remarks />
        public const uint Vector_Encoding_DefaultBinary = 2515;

        /// <remarks />
        public const uint VectorUnion_Encoding_DefaultBinary = 2603;

        /// <remarks />
        public const uint VectorWithOptionalFields_Encoding_DefaultBinary = 2604;

        /// <remarks />
        public const uint MultipleVectors_Encoding_DefaultBinary = 2605;

        /// <remarks />
        public const uint WorkOrderStatusType_Encoding_DefaultBinary = 2516;

        /// <remarks />
        public const uint WorkOrderType_Encoding_DefaultBinary = 2517;

        /// <remarks />
        public const uint ScalarStructureDataType_Encoding_DefaultXml = 2543;

        /// <remarks />
        public const uint ArrayValueDataType_Encoding_DefaultXml = 2544;

        /// <remarks />
        public const uint UserScalarValueDataType_Encoding_DefaultXml = 2545;

        /// <remarks />
        public const uint UserArrayValueDataType_Encoding_DefaultXml = 2546;

        /// <remarks />
        public const uint Vector_Encoding_DefaultXml = 2547;

        /// <remarks />
        public const uint VectorUnion_Encoding_DefaultXml = 2615;

        /// <remarks />
        public const uint VectorWithOptionalFields_Encoding_DefaultXml = 2616;

        /// <remarks />
        public const uint MultipleVectors_Encoding_DefaultXml = 2617;

        /// <remarks />
        public const uint WorkOrderStatusType_Encoding_DefaultXml = 2548;

        /// <remarks />
        public const uint WorkOrderType_Encoding_DefaultXml = 2549;

        /// <remarks />
        public const uint ScalarStructureDataType_Encoding_DefaultJson = 2575;

        /// <remarks />
        public const uint ArrayValueDataType_Encoding_DefaultJson = 2576;

        /// <remarks />
        public const uint UserScalarValueDataType_Encoding_DefaultJson = 2577;

        /// <remarks />
        public const uint UserArrayValueDataType_Encoding_DefaultJson = 2578;

        /// <remarks />
        public const uint Vector_Encoding_DefaultJson = 2579;

        /// <remarks />
        public const uint VectorUnion_Encoding_DefaultJson = 2627;

        /// <remarks />
        public const uint VectorWithOptionalFields_Encoding_DefaultJson = 2628;

        /// <remarks />
        public const uint MultipleVectors_Encoding_DefaultJson = 2629;

        /// <remarks />
        public const uint WorkOrderStatusType_Encoding_DefaultJson = 2580;

        /// <remarks />
        public const uint WorkOrderType_Encoding_DefaultJson = 2581;
    }
    #endregion

    #region ObjectType Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <remarks />
        public const uint GenerateValuesEventType = 3;

        /// <remarks />
        public const uint TestDataObjectType = 15;

        /// <remarks />
        public const uint ScalarValueObjectType = 116;

        /// <remarks />
        public const uint StructureValueObjectType = 210;

        /// <remarks />
        public const uint AnalogScalarValueObjectType = 305;

        /// <remarks />
        public const uint ArrayValueObjectType = 456;

        /// <remarks />
        public const uint AnalogArrayValueObjectType = 547;

        /// <remarks />
        public const uint UserScalarValueObjectType = 711;

        /// <remarks />
        public const uint UserArrayValueObjectType = 803;

        /// <remarks />
        public const uint MethodTestType = 901;

        /// <remarks />
        public const uint TestSystemConditionType = 932;
    }
    #endregion

    #region Variable Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <remarks />
        public const uint GenerateValuesEventType_Iterations = 13;

        /// <remarks />
        public const uint GenerateValuesEventType_NewValueCount = 14;

        /// <remarks />
        public const uint TestDataObjectType_SimulationActive = 16;

        /// <remarks />
        public const uint TestDataObjectType_GenerateValues_InputArguments = 18;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_EventId = 20;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_EventType = 21;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_SourceNode = 22;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_SourceName = 23;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Time = 24;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_ReceiveTime = 25;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Message = 27;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Severity = 28;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_ConditionClassId = 29;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_ConditionClassName = 30;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_ConditionName = 33;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_BranchId = 34;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Retain = 35;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_EnabledState = 36;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_EnabledState_Id = 37;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Quality = 45;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Quality_SourceTimestamp = 46;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_LastSeverity = 47;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_LastSeverity_SourceTimestamp = 48;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Comment = 49;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Comment_SourceTimestamp = 50;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_ClientUserId = 51;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_AddComment_InputArguments = 55;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_AckedState = 56;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_AckedState_Id = 57;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_ConfirmedState_Id = 66;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Acknowledge_InputArguments = 75;

        /// <remarks />
        public const uint TestDataObjectType_CycleComplete_Confirm_InputArguments = 77;

        /// <remarks />
        public const uint ScalarStructureVariableType_BooleanValue = 80;

        /// <remarks />
        public const uint ScalarStructureVariableType_SByteValue = 81;

        /// <remarks />
        public const uint ScalarStructureVariableType_ByteValue = 82;

        /// <remarks />
        public const uint ScalarStructureVariableType_Int16Value = 83;

        /// <remarks />
        public const uint ScalarStructureVariableType_UInt16Value = 84;

        /// <remarks />
        public const uint ScalarStructureVariableType_Int32Value = 85;

        /// <remarks />
        public const uint ScalarStructureVariableType_UInt32Value = 86;

        /// <remarks />
        public const uint ScalarStructureVariableType_Int64Value = 87;

        /// <remarks />
        public const uint ScalarStructureVariableType_UInt64Value = 88;

        /// <remarks />
        public const uint ScalarStructureVariableType_FloatValue = 89;

        /// <remarks />
        public const uint ScalarStructureVariableType_DoubleValue = 90;

        /// <remarks />
        public const uint ScalarStructureVariableType_StringValue = 91;

        /// <remarks />
        public const uint ScalarStructureVariableType_DateTimeValue = 92;

        /// <remarks />
        public const uint ScalarStructureVariableType_GuidValue = 93;

        /// <remarks />
        public const uint ScalarStructureVariableType_ByteStringValue = 94;

        /// <remarks />
        public const uint ScalarStructureVariableType_XmlElementValue = 95;

        /// <remarks />
        public const uint ScalarStructureVariableType_NodeIdValue = 96;

        /// <remarks />
        public const uint ScalarStructureVariableType_ExpandedNodeIdValue = 97;

        /// <remarks />
        public const uint ScalarStructureVariableType_QualifiedNameValue = 98;

        /// <remarks />
        public const uint ScalarStructureVariableType_LocalizedTextValue = 99;

        /// <remarks />
        public const uint ScalarStructureVariableType_StatusCodeValue = 100;

        /// <remarks />
        public const uint ScalarStructureVariableType_VariantValue = 101;

        /// <remarks />
        public const uint ScalarStructureVariableType_EnumerationValue = 102;

        /// <remarks />
        public const uint ScalarStructureVariableType_StructureValue = 103;

        /// <remarks />
        public const uint ScalarStructureVariableType_NumberValue = 104;

        /// <remarks />
        public const uint ScalarStructureVariableType_IntegerValue = 105;

        /// <remarks />
        public const uint ScalarStructureVariableType_UIntegerValue = 106;

        /// <remarks />
        public const uint ScalarValueObjectType_GenerateValues_InputArguments = 119;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_EventId = 121;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_EventType = 122;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_SourceNode = 123;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_SourceName = 124;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Time = 125;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_ReceiveTime = 126;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Message = 128;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Severity = 129;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_ConditionClassId = 130;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_ConditionClassName = 131;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_ConditionName = 134;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_BranchId = 135;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Retain = 136;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_EnabledState = 137;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_EnabledState_Id = 138;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Quality = 146;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Quality_SourceTimestamp = 147;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_LastSeverity = 148;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 149;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Comment = 150;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Comment_SourceTimestamp = 151;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_ClientUserId = 152;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_AddComment_InputArguments = 156;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_AckedState = 157;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_AckedState_Id = 158;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_ConfirmedState_Id = 167;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Acknowledge_InputArguments = 176;

        /// <remarks />
        public const uint ScalarValueObjectType_CycleComplete_Confirm_InputArguments = 178;

        /// <remarks />
        public const uint ScalarValueObjectType_BooleanValue = 179;

        /// <remarks />
        public const uint ScalarValueObjectType_SByteValue = 180;

        /// <remarks />
        public const uint ScalarValueObjectType_ByteValue = 181;

        /// <remarks />
        public const uint ScalarValueObjectType_Int16Value = 182;

        /// <remarks />
        public const uint ScalarValueObjectType_UInt16Value = 183;

        /// <remarks />
        public const uint ScalarValueObjectType_Int32Value = 184;

        /// <remarks />
        public const uint ScalarValueObjectType_UInt32Value = 185;

        /// <remarks />
        public const uint ScalarValueObjectType_Int64Value = 186;

        /// <remarks />
        public const uint ScalarValueObjectType_UInt64Value = 187;

        /// <remarks />
        public const uint ScalarValueObjectType_FloatValue = 188;

        /// <remarks />
        public const uint ScalarValueObjectType_DoubleValue = 189;

        /// <remarks />
        public const uint ScalarValueObjectType_StringValue = 190;

        /// <remarks />
        public const uint ScalarValueObjectType_DateTimeValue = 191;

        /// <remarks />
        public const uint ScalarValueObjectType_GuidValue = 192;

        /// <remarks />
        public const uint ScalarValueObjectType_ByteStringValue = 193;

        /// <remarks />
        public const uint ScalarValueObjectType_XmlElementValue = 194;

        /// <remarks />
        public const uint ScalarValueObjectType_NodeIdValue = 195;

        /// <remarks />
        public const uint ScalarValueObjectType_ExpandedNodeIdValue = 196;

        /// <remarks />
        public const uint ScalarValueObjectType_QualifiedNameValue = 197;

        /// <remarks />
        public const uint ScalarValueObjectType_LocalizedTextValue = 198;

        /// <remarks />
        public const uint ScalarValueObjectType_StatusCodeValue = 199;

        /// <remarks />
        public const uint ScalarValueObjectType_VariantValue = 200;

        /// <remarks />
        public const uint ScalarValueObjectType_EnumerationValue = 201;

        /// <remarks />
        public const uint ScalarValueObjectType_StructureValue = 202;

        /// <remarks />
        public const uint ScalarValueObjectType_NumberValue = 203;

        /// <remarks />
        public const uint ScalarValueObjectType_IntegerValue = 204;

        /// <remarks />
        public const uint ScalarValueObjectType_UIntegerValue = 205;

        /// <remarks />
        public const uint ScalarValueObjectType_VectorValue = 206;

        /// <remarks />
        public const uint ScalarValueObjectType_VectorValue_X = 207;

        /// <remarks />
        public const uint ScalarValueObjectType_VectorValue_Y = 208;

        /// <remarks />
        public const uint ScalarValueObjectType_VectorValue_Z = 209;

        /// <remarks />
        public const uint ScalarValueObjectType_VectorUnionValue = 2582;

        /// <remarks />
        public const uint ScalarValueObjectType_VectorWithOptionalFieldsValue = 2583;

        /// <remarks />
        public const uint ScalarValueObjectType_MultipleVectorsValue = 2584;

        /// <remarks />
        public const uint StructureValueObjectType_GenerateValues_InputArguments = 213;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_EventId = 215;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_EventType = 216;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_SourceNode = 217;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_SourceName = 218;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Time = 219;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_ReceiveTime = 220;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Message = 222;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Severity = 223;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_ConditionClassId = 224;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_ConditionClassName = 225;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_ConditionName = 228;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_BranchId = 229;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Retain = 230;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_EnabledState = 231;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_EnabledState_Id = 232;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Quality = 240;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Quality_SourceTimestamp = 241;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_LastSeverity = 242;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 243;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Comment = 244;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Comment_SourceTimestamp = 245;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_ClientUserId = 246;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_AddComment_InputArguments = 250;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_AckedState = 251;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_AckedState_Id = 252;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_ConfirmedState_Id = 261;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Acknowledge_InputArguments = 270;

        /// <remarks />
        public const uint StructureValueObjectType_CycleComplete_Confirm_InputArguments = 272;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure = 273;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_BooleanValue = 274;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_SByteValue = 275;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_ByteValue = 276;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_Int16Value = 277;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_UInt16Value = 278;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_Int32Value = 279;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_UInt32Value = 280;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_Int64Value = 281;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_UInt64Value = 282;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_FloatValue = 283;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_DoubleValue = 284;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_StringValue = 285;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_DateTimeValue = 286;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_GuidValue = 287;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_ByteStringValue = 288;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_XmlElementValue = 289;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_NodeIdValue = 290;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_ExpandedNodeIdValue = 291;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_QualifiedNameValue = 292;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_LocalizedTextValue = 293;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_StatusCodeValue = 294;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_VariantValue = 295;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_EnumerationValue = 296;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_StructureValue = 297;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_NumberValue = 298;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_IntegerValue = 299;

        /// <remarks />
        public const uint StructureValueObjectType_ScalarStructure_UIntegerValue = 300;

        /// <remarks />
        public const uint StructureValueObjectType_VectorStructure = 301;

        /// <remarks />
        public const uint StructureValueObjectType_VectorStructure_X = 302;

        /// <remarks />
        public const uint StructureValueObjectType_VectorStructure_Y = 303;

        /// <remarks />
        public const uint StructureValueObjectType_VectorStructure_Z = 304;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_GenerateValues_InputArguments = 308;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_EventId = 310;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_EventType = 311;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_SourceNode = 312;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_SourceName = 313;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Time = 314;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_ReceiveTime = 315;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Message = 317;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Severity = 318;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_ConditionClassId = 319;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_ConditionClassName = 320;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_ConditionName = 323;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_BranchId = 324;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Retain = 325;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_EnabledState = 326;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_EnabledState_Id = 327;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Quality = 335;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Quality_SourceTimestamp = 336;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_LastSeverity = 337;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 338;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Comment = 339;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Comment_SourceTimestamp = 340;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_ClientUserId = 341;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_AddComment_InputArguments = 345;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_AckedState = 346;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_AckedState_Id = 347;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_ConfirmedState_Id = 356;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Acknowledge_InputArguments = 365;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_CycleComplete_Confirm_InputArguments = 367;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_SByteValue = 368;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_SByteValue_EURange = 372;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_ByteValue = 374;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_ByteValue_EURange = 378;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_Int16Value = 380;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_Int16Value_EURange = 384;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UInt16Value = 386;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UInt16Value_EURange = 390;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_Int32Value = 392;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_Int32Value_EURange = 396;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UInt32Value = 398;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UInt32Value_EURange = 402;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_Int64Value = 404;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_Int64Value_EURange = 408;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UInt64Value = 410;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UInt64Value_EURange = 414;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_FloatValue = 416;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_FloatValue_EURange = 420;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_DoubleValue = 422;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_DoubleValue_EURange = 426;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_NumberValue = 428;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_NumberValue_EURange = 432;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_IntegerValue = 434;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_IntegerValue_EURange = 438;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UIntegerValue = 440;

        /// <remarks />
        public const uint AnalogScalarValueObjectType_UIntegerValue_EURange = 444;

        /// <remarks />
        public const uint ArrayValueObjectType_GenerateValues_InputArguments = 459;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_EventId = 461;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_EventType = 462;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_SourceNode = 463;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_SourceName = 464;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Time = 465;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_ReceiveTime = 466;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Message = 468;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Severity = 469;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_ConditionClassId = 470;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_ConditionClassName = 471;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_ConditionName = 474;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_BranchId = 475;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Retain = 476;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_EnabledState = 477;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_EnabledState_Id = 478;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Quality = 486;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Quality_SourceTimestamp = 487;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_LastSeverity = 488;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 489;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Comment = 490;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Comment_SourceTimestamp = 491;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_ClientUserId = 492;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_AddComment_InputArguments = 496;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_AckedState = 497;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_AckedState_Id = 498;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_ConfirmedState_Id = 507;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Acknowledge_InputArguments = 516;

        /// <remarks />
        public const uint ArrayValueObjectType_CycleComplete_Confirm_InputArguments = 518;

        /// <remarks />
        public const uint ArrayValueObjectType_BooleanValue = 519;

        /// <remarks />
        public const uint ArrayValueObjectType_SByteValue = 520;

        /// <remarks />
        public const uint ArrayValueObjectType_ByteValue = 521;

        /// <remarks />
        public const uint ArrayValueObjectType_Int16Value = 522;

        /// <remarks />
        public const uint ArrayValueObjectType_UInt16Value = 523;

        /// <remarks />
        public const uint ArrayValueObjectType_Int32Value = 524;

        /// <remarks />
        public const uint ArrayValueObjectType_UInt32Value = 525;

        /// <remarks />
        public const uint ArrayValueObjectType_Int64Value = 526;

        /// <remarks />
        public const uint ArrayValueObjectType_UInt64Value = 527;

        /// <remarks />
        public const uint ArrayValueObjectType_FloatValue = 528;

        /// <remarks />
        public const uint ArrayValueObjectType_DoubleValue = 529;

        /// <remarks />
        public const uint ArrayValueObjectType_StringValue = 530;

        /// <remarks />
        public const uint ArrayValueObjectType_DateTimeValue = 531;

        /// <remarks />
        public const uint ArrayValueObjectType_GuidValue = 532;

        /// <remarks />
        public const uint ArrayValueObjectType_ByteStringValue = 533;

        /// <remarks />
        public const uint ArrayValueObjectType_XmlElementValue = 534;

        /// <remarks />
        public const uint ArrayValueObjectType_NodeIdValue = 535;

        /// <remarks />
        public const uint ArrayValueObjectType_ExpandedNodeIdValue = 536;

        /// <remarks />
        public const uint ArrayValueObjectType_QualifiedNameValue = 537;

        /// <remarks />
        public const uint ArrayValueObjectType_LocalizedTextValue = 538;

        /// <remarks />
        public const uint ArrayValueObjectType_StatusCodeValue = 539;

        /// <remarks />
        public const uint ArrayValueObjectType_VariantValue = 540;

        /// <remarks />
        public const uint ArrayValueObjectType_EnumerationValue = 541;

        /// <remarks />
        public const uint ArrayValueObjectType_StructureValue = 542;

        /// <remarks />
        public const uint ArrayValueObjectType_NumberValue = 543;

        /// <remarks />
        public const uint ArrayValueObjectType_IntegerValue = 544;

        /// <remarks />
        public const uint ArrayValueObjectType_UIntegerValue = 545;

        /// <remarks />
        public const uint ArrayValueObjectType_VectorValue = 546;

        /// <remarks />
        public const uint ArrayValueObjectType_VectorUnionValue = 2585;

        /// <remarks />
        public const uint ArrayValueObjectType_VectorWithOptionalFieldsValue = 2586;

        /// <remarks />
        public const uint ArrayValueObjectType_MultipleVectorsValue = 2587;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_GenerateValues_InputArguments = 550;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_EventId = 552;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_EventType = 553;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_SourceNode = 554;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_SourceName = 555;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Time = 556;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_ReceiveTime = 557;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Message = 559;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Severity = 560;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_ConditionClassId = 561;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_ConditionClassName = 562;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_ConditionName = 565;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_BranchId = 566;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Retain = 567;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_EnabledState = 568;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_EnabledState_Id = 569;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Quality = 577;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Quality_SourceTimestamp = 578;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_LastSeverity = 579;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 580;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Comment = 581;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Comment_SourceTimestamp = 582;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_ClientUserId = 583;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_AddComment_InputArguments = 587;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_AckedState = 588;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_AckedState_Id = 589;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_ConfirmedState_Id = 598;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Acknowledge_InputArguments = 607;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_CycleComplete_Confirm_InputArguments = 609;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_SByteValue = 610;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_SByteValue_EURange = 614;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_ByteValue = 616;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_ByteValue_EURange = 620;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_Int16Value = 622;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_Int16Value_EURange = 626;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UInt16Value = 628;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UInt16Value_EURange = 632;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_Int32Value = 634;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_Int32Value_EURange = 638;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UInt32Value = 640;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UInt32Value_EURange = 644;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_Int64Value = 646;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_Int64Value_EURange = 650;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UInt64Value = 652;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UInt64Value_EURange = 656;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_FloatValue = 658;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_FloatValue_EURange = 662;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_DoubleValue = 664;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_DoubleValue_EURange = 668;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_NumberValue = 670;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_NumberValue_EURange = 674;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_IntegerValue = 676;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_IntegerValue_EURange = 680;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UIntegerValue = 682;

        /// <remarks />
        public const uint AnalogArrayValueObjectType_UIntegerValue_EURange = 686;

        /// <remarks />
        public const uint UserScalarValueObjectType_GenerateValues_InputArguments = 714;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_EventId = 716;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_EventType = 717;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_SourceNode = 718;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_SourceName = 719;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Time = 720;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_ReceiveTime = 721;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Message = 723;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Severity = 724;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_ConditionClassId = 725;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_ConditionClassName = 726;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_ConditionName = 729;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_BranchId = 730;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Retain = 731;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_EnabledState = 732;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_EnabledState_Id = 733;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Quality = 741;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Quality_SourceTimestamp = 742;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_LastSeverity = 743;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 744;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Comment = 745;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Comment_SourceTimestamp = 746;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_ClientUserId = 747;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_AddComment_InputArguments = 751;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_AckedState = 752;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_AckedState_Id = 753;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_ConfirmedState_Id = 762;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Acknowledge_InputArguments = 771;

        /// <remarks />
        public const uint UserScalarValueObjectType_CycleComplete_Confirm_InputArguments = 773;

        /// <remarks />
        public const uint UserScalarValueObjectType_BooleanValue = 774;

        /// <remarks />
        public const uint UserScalarValueObjectType_SByteValue = 775;

        /// <remarks />
        public const uint UserScalarValueObjectType_ByteValue = 776;

        /// <remarks />
        public const uint UserScalarValueObjectType_Int16Value = 777;

        /// <remarks />
        public const uint UserScalarValueObjectType_UInt16Value = 778;

        /// <remarks />
        public const uint UserScalarValueObjectType_Int32Value = 779;

        /// <remarks />
        public const uint UserScalarValueObjectType_UInt32Value = 780;

        /// <remarks />
        public const uint UserScalarValueObjectType_Int64Value = 781;

        /// <remarks />
        public const uint UserScalarValueObjectType_UInt64Value = 782;

        /// <remarks />
        public const uint UserScalarValueObjectType_FloatValue = 783;

        /// <remarks />
        public const uint UserScalarValueObjectType_DoubleValue = 784;

        /// <remarks />
        public const uint UserScalarValueObjectType_StringValue = 785;

        /// <remarks />
        public const uint UserScalarValueObjectType_DateTimeValue = 786;

        /// <remarks />
        public const uint UserScalarValueObjectType_GuidValue = 787;

        /// <remarks />
        public const uint UserScalarValueObjectType_ByteStringValue = 788;

        /// <remarks />
        public const uint UserScalarValueObjectType_XmlElementValue = 789;

        /// <remarks />
        public const uint UserScalarValueObjectType_NodeIdValue = 790;

        /// <remarks />
        public const uint UserScalarValueObjectType_ExpandedNodeIdValue = 791;

        /// <remarks />
        public const uint UserScalarValueObjectType_QualifiedNameValue = 792;

        /// <remarks />
        public const uint UserScalarValueObjectType_LocalizedTextValue = 793;

        /// <remarks />
        public const uint UserScalarValueObjectType_StatusCodeValue = 794;

        /// <remarks />
        public const uint UserScalarValueObjectType_VariantValue = 795;

        /// <remarks />
        public const uint UserArrayValueObjectType_GenerateValues_InputArguments = 806;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_EventId = 808;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_EventType = 809;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_SourceNode = 810;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_SourceName = 811;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Time = 812;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_ReceiveTime = 813;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Message = 815;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Severity = 816;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_ConditionClassId = 817;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_ConditionClassName = 818;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_ConditionName = 821;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_BranchId = 822;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Retain = 823;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_EnabledState = 824;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_EnabledState_Id = 825;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Quality = 833;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Quality_SourceTimestamp = 834;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_LastSeverity = 835;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = 836;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Comment = 837;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Comment_SourceTimestamp = 838;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_ClientUserId = 839;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_AddComment_InputArguments = 843;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_AckedState = 844;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_AckedState_Id = 845;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_ConfirmedState_Id = 854;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Acknowledge_InputArguments = 863;

        /// <remarks />
        public const uint UserArrayValueObjectType_CycleComplete_Confirm_InputArguments = 865;

        /// <remarks />
        public const uint UserArrayValueObjectType_BooleanValue = 866;

        /// <remarks />
        public const uint UserArrayValueObjectType_SByteValue = 867;

        /// <remarks />
        public const uint UserArrayValueObjectType_ByteValue = 868;

        /// <remarks />
        public const uint UserArrayValueObjectType_Int16Value = 869;

        /// <remarks />
        public const uint UserArrayValueObjectType_UInt16Value = 870;

        /// <remarks />
        public const uint UserArrayValueObjectType_Int32Value = 871;

        /// <remarks />
        public const uint UserArrayValueObjectType_UInt32Value = 872;

        /// <remarks />
        public const uint UserArrayValueObjectType_Int64Value = 873;

        /// <remarks />
        public const uint UserArrayValueObjectType_UInt64Value = 874;

        /// <remarks />
        public const uint UserArrayValueObjectType_FloatValue = 875;

        /// <remarks />
        public const uint UserArrayValueObjectType_DoubleValue = 876;

        /// <remarks />
        public const uint UserArrayValueObjectType_StringValue = 877;

        /// <remarks />
        public const uint UserArrayValueObjectType_DateTimeValue = 878;

        /// <remarks />
        public const uint UserArrayValueObjectType_GuidValue = 879;

        /// <remarks />
        public const uint UserArrayValueObjectType_ByteStringValue = 880;

        /// <remarks />
        public const uint UserArrayValueObjectType_XmlElementValue = 881;

        /// <remarks />
        public const uint UserArrayValueObjectType_NodeIdValue = 882;

        /// <remarks />
        public const uint UserArrayValueObjectType_ExpandedNodeIdValue = 883;

        /// <remarks />
        public const uint UserArrayValueObjectType_QualifiedNameValue = 884;

        /// <remarks />
        public const uint UserArrayValueObjectType_LocalizedTextValue = 885;

        /// <remarks />
        public const uint UserArrayValueObjectType_StatusCodeValue = 886;

        /// <remarks />
        public const uint UserArrayValueObjectType_VariantValue = 887;

        /// <remarks />
        public const uint VectorVariableType_X = 890;

        /// <remarks />
        public const uint VectorVariableType_Y = 891;

        /// <remarks />
        public const uint VectorVariableType_Z = 892;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod1_InputArguments = 903;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod1_OutputArguments = 904;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod2_InputArguments = 906;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod2_OutputArguments = 907;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod3_InputArguments = 909;

        /// <remarks />
        public const uint MethodTestType_ScalarMethod3_OutputArguments = 910;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod1_InputArguments = 912;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod1_OutputArguments = 913;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod2_InputArguments = 915;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod2_OutputArguments = 916;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod3_InputArguments = 918;

        /// <remarks />
        public const uint MethodTestType_ArrayMethod3_OutputArguments = 919;

        /// <remarks />
        public const uint MethodTestType_UserScalarMethod1_InputArguments = 921;

        /// <remarks />
        public const uint MethodTestType_UserScalarMethod1_OutputArguments = 922;

        /// <remarks />
        public const uint MethodTestType_UserScalarMethod2_InputArguments = 924;

        /// <remarks />
        public const uint MethodTestType_UserScalarMethod2_OutputArguments = 925;

        /// <remarks />
        public const uint MethodTestType_UserArrayMethod1_InputArguments = 927;

        /// <remarks />
        public const uint MethodTestType_UserArrayMethod1_OutputArguments = 928;

        /// <remarks />
        public const uint MethodTestType_UserArrayMethod2_InputArguments = 930;

        /// <remarks />
        public const uint MethodTestType_UserArrayMethod2_OutputArguments = 931;

        /// <remarks />
        public const uint TestSystemConditionType_EnabledState_Id = 950;

        /// <remarks />
        public const uint TestSystemConditionType_Quality_SourceTimestamp = 959;

        /// <remarks />
        public const uint TestSystemConditionType_LastSeverity_SourceTimestamp = 961;

        /// <remarks />
        public const uint TestSystemConditionType_Comment_SourceTimestamp = 963;

        /// <remarks />
        public const uint TestSystemConditionType_AddComment_InputArguments = 968;

        /// <remarks />
        public const uint TestSystemConditionType_ConditionRefresh_InputArguments = 970;

        /// <remarks />
        public const uint TestSystemConditionType_ConditionRefresh2_InputArguments = 972;

        /// <remarks />
        public const uint TestSystemConditionType_MonitoredNodeCount = 973;

        /// <remarks />
        public const uint Data_Static_Scalar_SimulationActive = 977;

        /// <remarks />
        public const uint Data_Static_Scalar_GenerateValues_InputArguments = 979;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_EventId = 981;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_EventType = 982;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_SourceNode = 983;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_SourceName = 984;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Time = 985;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_ReceiveTime = 986;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Message = 988;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Severity = 989;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_ConditionClassId = 990;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_ConditionClassName = 991;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_ConditionName = 994;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_BranchId = 995;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Retain = 996;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_EnabledState = 997;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_EnabledState_Id = 998;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Quality = 1006;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Quality_SourceTimestamp = 1007;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_LastSeverity = 1008;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_LastSeverity_SourceTimestamp = 1009;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Comment = 1010;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Comment_SourceTimestamp = 1011;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_ClientUserId = 1012;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_AddComment_InputArguments = 1016;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_AckedState = 1017;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_AckedState_Id = 1018;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_ConfirmedState_Id = 1027;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Acknowledge_InputArguments = 1036;

        /// <remarks />
        public const uint Data_Static_Scalar_CycleComplete_Confirm_InputArguments = 1038;

        /// <remarks />
        public const uint Data_Static_Scalar_BooleanValue = 1039;

        /// <remarks />
        public const uint Data_Static_Scalar_SByteValue = 1040;

        /// <remarks />
        public const uint Data_Static_Scalar_ByteValue = 1041;

        /// <remarks />
        public const uint Data_Static_Scalar_Int16Value = 1042;

        /// <remarks />
        public const uint Data_Static_Scalar_UInt16Value = 1043;

        /// <remarks />
        public const uint Data_Static_Scalar_Int32Value = 1044;

        /// <remarks />
        public const uint Data_Static_Scalar_UInt32Value = 1045;

        /// <remarks />
        public const uint Data_Static_Scalar_Int64Value = 1046;

        /// <remarks />
        public const uint Data_Static_Scalar_UInt64Value = 1047;

        /// <remarks />
        public const uint Data_Static_Scalar_FloatValue = 1048;

        /// <remarks />
        public const uint Data_Static_Scalar_DoubleValue = 1049;

        /// <remarks />
        public const uint Data_Static_Scalar_StringValue = 1050;

        /// <remarks />
        public const uint Data_Static_Scalar_DateTimeValue = 1051;

        /// <remarks />
        public const uint Data_Static_Scalar_GuidValue = 1052;

        /// <remarks />
        public const uint Data_Static_Scalar_ByteStringValue = 1053;

        /// <remarks />
        public const uint Data_Static_Scalar_XmlElementValue = 1054;

        /// <remarks />
        public const uint Data_Static_Scalar_NodeIdValue = 1055;

        /// <remarks />
        public const uint Data_Static_Scalar_ExpandedNodeIdValue = 1056;

        /// <remarks />
        public const uint Data_Static_Scalar_QualifiedNameValue = 1057;

        /// <remarks />
        public const uint Data_Static_Scalar_LocalizedTextValue = 1058;

        /// <remarks />
        public const uint Data_Static_Scalar_StatusCodeValue = 1059;

        /// <remarks />
        public const uint Data_Static_Scalar_VariantValue = 1060;

        /// <remarks />
        public const uint Data_Static_Scalar_EnumerationValue = 1061;

        /// <remarks />
        public const uint Data_Static_Scalar_StructureValue = 1062;

        /// <remarks />
        public const uint Data_Static_Scalar_NumberValue = 1063;

        /// <remarks />
        public const uint Data_Static_Scalar_IntegerValue = 1064;

        /// <remarks />
        public const uint Data_Static_Scalar_UIntegerValue = 1065;

        /// <remarks />
        public const uint Data_Static_Scalar_VectorValue = 1066;

        /// <remarks />
        public const uint Data_Static_Scalar_VectorValue_X = 1067;

        /// <remarks />
        public const uint Data_Static_Scalar_VectorValue_Y = 1068;

        /// <remarks />
        public const uint Data_Static_Scalar_VectorValue_Z = 1069;

        /// <remarks />
        public const uint Data_Static_Scalar_VectorUnionValue = 2591;

        /// <remarks />
        public const uint Data_Static_Scalar_VectorWithOptionalFieldsValue = 2592;

        /// <remarks />
        public const uint Data_Static_Scalar_MultipleVectorsValue = 2593;

        /// <remarks />
        public const uint Data_Static_Structure_SimulationActive = 1071;

        /// <remarks />
        public const uint Data_Static_Structure_GenerateValues_InputArguments = 1073;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_EventId = 1075;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_EventType = 1076;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_SourceNode = 1077;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_SourceName = 1078;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Time = 1079;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_ReceiveTime = 1080;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Message = 1082;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Severity = 1083;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_ConditionClassId = 1084;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_ConditionClassName = 1085;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_ConditionName = 1088;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_BranchId = 1089;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Retain = 1090;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_EnabledState = 1091;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_EnabledState_Id = 1092;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Quality = 1100;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Quality_SourceTimestamp = 1101;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_LastSeverity = 1102;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_LastSeverity_SourceTimestamp = 1103;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Comment = 1104;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Comment_SourceTimestamp = 1105;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_ClientUserId = 1106;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_AddComment_InputArguments = 1110;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_AckedState = 1111;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_AckedState_Id = 1112;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_ConfirmedState_Id = 1121;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Acknowledge_InputArguments = 1130;

        /// <remarks />
        public const uint Data_Static_Structure_CycleComplete_Confirm_InputArguments = 1132;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure = 1133;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_BooleanValue = 1134;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_SByteValue = 1135;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_ByteValue = 1136;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_Int16Value = 1137;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_UInt16Value = 1138;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_Int32Value = 1139;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_UInt32Value = 1140;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_Int64Value = 1141;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_UInt64Value = 1142;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_FloatValue = 1143;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_DoubleValue = 1144;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_StringValue = 1145;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_DateTimeValue = 1146;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_GuidValue = 1147;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_ByteStringValue = 1148;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_XmlElementValue = 1149;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_NodeIdValue = 1150;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_ExpandedNodeIdValue = 1151;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_QualifiedNameValue = 1152;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_LocalizedTextValue = 1153;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_StatusCodeValue = 1154;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_VariantValue = 1155;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_EnumerationValue = 1156;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_StructureValue = 1157;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_NumberValue = 1158;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_IntegerValue = 1159;

        /// <remarks />
        public const uint Data_Static_Structure_ScalarStructure_UIntegerValue = 1160;

        /// <remarks />
        public const uint Data_Static_Structure_VectorStructure = 1161;

        /// <remarks />
        public const uint Data_Static_Structure_VectorStructure_X = 1162;

        /// <remarks />
        public const uint Data_Static_Structure_VectorStructure_Y = 1163;

        /// <remarks />
        public const uint Data_Static_Structure_VectorStructure_Z = 1164;

        /// <remarks />
        public const uint Data_Static_Array_SimulationActive = 1166;

        /// <remarks />
        public const uint Data_Static_Array_GenerateValues_InputArguments = 1168;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_EventId = 1170;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_EventType = 1171;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_SourceNode = 1172;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_SourceName = 1173;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Time = 1174;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_ReceiveTime = 1175;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Message = 1177;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Severity = 1178;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_ConditionClassId = 1179;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_ConditionClassName = 1180;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_ConditionName = 1183;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_BranchId = 1184;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Retain = 1185;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_EnabledState = 1186;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_EnabledState_Id = 1187;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Quality = 1195;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Quality_SourceTimestamp = 1196;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_LastSeverity = 1197;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_LastSeverity_SourceTimestamp = 1198;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Comment = 1199;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Comment_SourceTimestamp = 1200;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_ClientUserId = 1201;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_AddComment_InputArguments = 1205;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_AckedState = 1206;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_AckedState_Id = 1207;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_ConfirmedState_Id = 1216;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Acknowledge_InputArguments = 1225;

        /// <remarks />
        public const uint Data_Static_Array_CycleComplete_Confirm_InputArguments = 1227;

        /// <remarks />
        public const uint Data_Static_Array_BooleanValue = 1228;

        /// <remarks />
        public const uint Data_Static_Array_SByteValue = 1229;

        /// <remarks />
        public const uint Data_Static_Array_ByteValue = 1230;

        /// <remarks />
        public const uint Data_Static_Array_Int16Value = 1231;

        /// <remarks />
        public const uint Data_Static_Array_UInt16Value = 1232;

        /// <remarks />
        public const uint Data_Static_Array_Int32Value = 1233;

        /// <remarks />
        public const uint Data_Static_Array_UInt32Value = 1234;

        /// <remarks />
        public const uint Data_Static_Array_Int64Value = 1235;

        /// <remarks />
        public const uint Data_Static_Array_UInt64Value = 1236;

        /// <remarks />
        public const uint Data_Static_Array_FloatValue = 1237;

        /// <remarks />
        public const uint Data_Static_Array_DoubleValue = 1238;

        /// <remarks />
        public const uint Data_Static_Array_StringValue = 1239;

        /// <remarks />
        public const uint Data_Static_Array_DateTimeValue = 1240;

        /// <remarks />
        public const uint Data_Static_Array_GuidValue = 1241;

        /// <remarks />
        public const uint Data_Static_Array_ByteStringValue = 1242;

        /// <remarks />
        public const uint Data_Static_Array_XmlElementValue = 1243;

        /// <remarks />
        public const uint Data_Static_Array_NodeIdValue = 1244;

        /// <remarks />
        public const uint Data_Static_Array_ExpandedNodeIdValue = 1245;

        /// <remarks />
        public const uint Data_Static_Array_QualifiedNameValue = 1246;

        /// <remarks />
        public const uint Data_Static_Array_LocalizedTextValue = 1247;

        /// <remarks />
        public const uint Data_Static_Array_StatusCodeValue = 1248;

        /// <remarks />
        public const uint Data_Static_Array_VariantValue = 1249;

        /// <remarks />
        public const uint Data_Static_Array_EnumerationValue = 1250;

        /// <remarks />
        public const uint Data_Static_Array_StructureValue = 1251;

        /// <remarks />
        public const uint Data_Static_Array_NumberValue = 1252;

        /// <remarks />
        public const uint Data_Static_Array_IntegerValue = 1253;

        /// <remarks />
        public const uint Data_Static_Array_UIntegerValue = 1254;

        /// <remarks />
        public const uint Data_Static_Array_VectorValue = 1255;

        /// <remarks />
        public const uint Data_Static_Array_VectorUnionValue = 2594;

        /// <remarks />
        public const uint Data_Static_Array_VectorWithOptionalFieldsValue = 2595;

        /// <remarks />
        public const uint Data_Static_Array_MultipleVectorsValue = 2596;

        /// <remarks />
        public const uint Data_Static_UserScalar_SimulationActive = 1257;

        /// <remarks />
        public const uint Data_Static_UserScalar_GenerateValues_InputArguments = 1259;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_EventId = 1261;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_EventType = 1262;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_SourceNode = 1263;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_SourceName = 1264;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Time = 1265;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_ReceiveTime = 1266;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Message = 1268;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Severity = 1269;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_ConditionClassId = 1270;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_ConditionClassName = 1271;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_ConditionName = 1274;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_BranchId = 1275;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Retain = 1276;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_EnabledState = 1277;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_EnabledState_Id = 1278;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Quality = 1286;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Quality_SourceTimestamp = 1287;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_LastSeverity = 1288;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_LastSeverity_SourceTimestamp = 1289;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Comment = 1290;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Comment_SourceTimestamp = 1291;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_ClientUserId = 1292;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_AddComment_InputArguments = 1296;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_AckedState = 1297;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_AckedState_Id = 1298;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_ConfirmedState_Id = 1307;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Acknowledge_InputArguments = 1316;

        /// <remarks />
        public const uint Data_Static_UserScalar_CycleComplete_Confirm_InputArguments = 1318;

        /// <remarks />
        public const uint Data_Static_UserScalar_BooleanValue = 1319;

        /// <remarks />
        public const uint Data_Static_UserScalar_SByteValue = 1320;

        /// <remarks />
        public const uint Data_Static_UserScalar_ByteValue = 1321;

        /// <remarks />
        public const uint Data_Static_UserScalar_Int16Value = 1322;

        /// <remarks />
        public const uint Data_Static_UserScalar_UInt16Value = 1323;

        /// <remarks />
        public const uint Data_Static_UserScalar_Int32Value = 1324;

        /// <remarks />
        public const uint Data_Static_UserScalar_UInt32Value = 1325;

        /// <remarks />
        public const uint Data_Static_UserScalar_Int64Value = 1326;

        /// <remarks />
        public const uint Data_Static_UserScalar_UInt64Value = 1327;

        /// <remarks />
        public const uint Data_Static_UserScalar_FloatValue = 1328;

        /// <remarks />
        public const uint Data_Static_UserScalar_DoubleValue = 1329;

        /// <remarks />
        public const uint Data_Static_UserScalar_StringValue = 1330;

        /// <remarks />
        public const uint Data_Static_UserScalar_DateTimeValue = 1331;

        /// <remarks />
        public const uint Data_Static_UserScalar_GuidValue = 1332;

        /// <remarks />
        public const uint Data_Static_UserScalar_ByteStringValue = 1333;

        /// <remarks />
        public const uint Data_Static_UserScalar_XmlElementValue = 1334;

        /// <remarks />
        public const uint Data_Static_UserScalar_NodeIdValue = 1335;

        /// <remarks />
        public const uint Data_Static_UserScalar_ExpandedNodeIdValue = 1336;

        /// <remarks />
        public const uint Data_Static_UserScalar_QualifiedNameValue = 1337;

        /// <remarks />
        public const uint Data_Static_UserScalar_LocalizedTextValue = 1338;

        /// <remarks />
        public const uint Data_Static_UserScalar_StatusCodeValue = 1339;

        /// <remarks />
        public const uint Data_Static_UserScalar_VariantValue = 1340;

        /// <remarks />
        public const uint Data_Static_UserArray_SimulationActive = 1342;

        /// <remarks />
        public const uint Data_Static_UserArray_GenerateValues_InputArguments = 1344;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_EventId = 1346;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_EventType = 1347;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_SourceNode = 1348;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_SourceName = 1349;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Time = 1350;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_ReceiveTime = 1351;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Message = 1353;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Severity = 1354;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_ConditionClassId = 1355;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_ConditionClassName = 1356;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_ConditionName = 1359;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_BranchId = 1360;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Retain = 1361;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_EnabledState = 1362;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_EnabledState_Id = 1363;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Quality = 1371;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Quality_SourceTimestamp = 1372;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_LastSeverity = 1373;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_LastSeverity_SourceTimestamp = 1374;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Comment = 1375;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Comment_SourceTimestamp = 1376;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_ClientUserId = 1377;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_AddComment_InputArguments = 1381;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_AckedState = 1382;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_AckedState_Id = 1383;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_ConfirmedState_Id = 1392;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Acknowledge_InputArguments = 1401;

        /// <remarks />
        public const uint Data_Static_UserArray_CycleComplete_Confirm_InputArguments = 1403;

        /// <remarks />
        public const uint Data_Static_UserArray_BooleanValue = 1404;

        /// <remarks />
        public const uint Data_Static_UserArray_SByteValue = 1405;

        /// <remarks />
        public const uint Data_Static_UserArray_ByteValue = 1406;

        /// <remarks />
        public const uint Data_Static_UserArray_Int16Value = 1407;

        /// <remarks />
        public const uint Data_Static_UserArray_UInt16Value = 1408;

        /// <remarks />
        public const uint Data_Static_UserArray_Int32Value = 1409;

        /// <remarks />
        public const uint Data_Static_UserArray_UInt32Value = 1410;

        /// <remarks />
        public const uint Data_Static_UserArray_Int64Value = 1411;

        /// <remarks />
        public const uint Data_Static_UserArray_UInt64Value = 1412;

        /// <remarks />
        public const uint Data_Static_UserArray_FloatValue = 1413;

        /// <remarks />
        public const uint Data_Static_UserArray_DoubleValue = 1414;

        /// <remarks />
        public const uint Data_Static_UserArray_StringValue = 1415;

        /// <remarks />
        public const uint Data_Static_UserArray_DateTimeValue = 1416;

        /// <remarks />
        public const uint Data_Static_UserArray_GuidValue = 1417;

        /// <remarks />
        public const uint Data_Static_UserArray_ByteStringValue = 1418;

        /// <remarks />
        public const uint Data_Static_UserArray_XmlElementValue = 1419;

        /// <remarks />
        public const uint Data_Static_UserArray_NodeIdValue = 1420;

        /// <remarks />
        public const uint Data_Static_UserArray_ExpandedNodeIdValue = 1421;

        /// <remarks />
        public const uint Data_Static_UserArray_QualifiedNameValue = 1422;

        /// <remarks />
        public const uint Data_Static_UserArray_LocalizedTextValue = 1423;

        /// <remarks />
        public const uint Data_Static_UserArray_StatusCodeValue = 1424;

        /// <remarks />
        public const uint Data_Static_UserArray_VariantValue = 1425;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_SimulationActive = 1427;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_GenerateValues_InputArguments = 1429;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_EventId = 1431;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_EventType = 1432;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_SourceNode = 1433;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_SourceName = 1434;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Time = 1435;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_ReceiveTime = 1436;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Message = 1438;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Severity = 1439;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_ConditionClassId = 1440;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_ConditionClassName = 1441;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_ConditionName = 1444;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_BranchId = 1445;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Retain = 1446;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_EnabledState = 1447;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_EnabledState_Id = 1448;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Quality = 1456;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Quality_SourceTimestamp = 1457;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_LastSeverity = 1458;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_LastSeverity_SourceTimestamp = 1459;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Comment = 1460;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Comment_SourceTimestamp = 1461;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_ClientUserId = 1462;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_AddComment_InputArguments = 1466;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_AckedState = 1467;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_AckedState_Id = 1468;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_ConfirmedState_Id = 1477;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Acknowledge_InputArguments = 1486;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_CycleComplete_Confirm_InputArguments = 1488;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_SByteValue = 1489;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_SByteValue_EURange = 1493;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_ByteValue = 1495;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_ByteValue_EURange = 1499;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_Int16Value = 1501;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_Int16Value_EURange = 1505;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UInt16Value = 1507;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UInt16Value_EURange = 1511;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_Int32Value = 1513;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_Int32Value_EURange = 1517;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UInt32Value = 1519;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UInt32Value_EURange = 1523;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_Int64Value = 1525;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_Int64Value_EURange = 1529;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UInt64Value = 1531;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UInt64Value_EURange = 1535;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_FloatValue = 1537;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_FloatValue_EURange = 1541;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_DoubleValue = 1543;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_DoubleValue_EURange = 1547;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_NumberValue = 1549;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_NumberValue_EURange = 1553;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_IntegerValue = 1555;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_IntegerValue_EURange = 1559;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UIntegerValue = 1561;

        /// <remarks />
        public const uint Data_Static_AnalogScalar_UIntegerValue_EURange = 1565;

        /// <remarks />
        public const uint Data_Static_AnalogArray_SimulationActive = 1568;

        /// <remarks />
        public const uint Data_Static_AnalogArray_GenerateValues_InputArguments = 1570;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_EventId = 1572;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_EventType = 1573;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_SourceNode = 1574;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_SourceName = 1575;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Time = 1576;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_ReceiveTime = 1577;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Message = 1579;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Severity = 1580;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_ConditionClassId = 1581;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_ConditionClassName = 1582;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_ConditionName = 1585;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_BranchId = 1586;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Retain = 1587;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_EnabledState = 1588;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_EnabledState_Id = 1589;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Quality = 1597;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Quality_SourceTimestamp = 1598;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_LastSeverity = 1599;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_LastSeverity_SourceTimestamp = 1600;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Comment = 1601;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Comment_SourceTimestamp = 1602;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_ClientUserId = 1603;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_AddComment_InputArguments = 1607;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_AckedState = 1608;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_AckedState_Id = 1609;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_ConfirmedState_Id = 1618;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Acknowledge_InputArguments = 1627;

        /// <remarks />
        public const uint Data_Static_AnalogArray_CycleComplete_Confirm_InputArguments = 1629;

        /// <remarks />
        public const uint Data_Static_AnalogArray_SByteValue = 1630;

        /// <remarks />
        public const uint Data_Static_AnalogArray_SByteValue_EURange = 1634;

        /// <remarks />
        public const uint Data_Static_AnalogArray_ByteValue = 1636;

        /// <remarks />
        public const uint Data_Static_AnalogArray_ByteValue_EURange = 1640;

        /// <remarks />
        public const uint Data_Static_AnalogArray_Int16Value = 1642;

        /// <remarks />
        public const uint Data_Static_AnalogArray_Int16Value_EURange = 1646;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UInt16Value = 1648;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UInt16Value_EURange = 1652;

        /// <remarks />
        public const uint Data_Static_AnalogArray_Int32Value = 1654;

        /// <remarks />
        public const uint Data_Static_AnalogArray_Int32Value_EURange = 1658;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UInt32Value = 1660;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UInt32Value_EURange = 1664;

        /// <remarks />
        public const uint Data_Static_AnalogArray_Int64Value = 1666;

        /// <remarks />
        public const uint Data_Static_AnalogArray_Int64Value_EURange = 1670;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UInt64Value = 1672;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UInt64Value_EURange = 1676;

        /// <remarks />
        public const uint Data_Static_AnalogArray_FloatValue = 1678;

        /// <remarks />
        public const uint Data_Static_AnalogArray_FloatValue_EURange = 1682;

        /// <remarks />
        public const uint Data_Static_AnalogArray_DoubleValue = 1684;

        /// <remarks />
        public const uint Data_Static_AnalogArray_DoubleValue_EURange = 1688;

        /// <remarks />
        public const uint Data_Static_AnalogArray_NumberValue = 1690;

        /// <remarks />
        public const uint Data_Static_AnalogArray_NumberValue_EURange = 1694;

        /// <remarks />
        public const uint Data_Static_AnalogArray_IntegerValue = 1696;

        /// <remarks />
        public const uint Data_Static_AnalogArray_IntegerValue_EURange = 1700;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UIntegerValue = 1702;

        /// <remarks />
        public const uint Data_Static_AnalogArray_UIntegerValue_EURange = 1706;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod1_InputArguments = 1710;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod1_OutputArguments = 1711;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod2_InputArguments = 1713;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod2_OutputArguments = 1714;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod3_InputArguments = 1716;

        /// <remarks />
        public const uint Data_Static_MethodTest_ScalarMethod3_OutputArguments = 1717;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod1_InputArguments = 1719;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod1_OutputArguments = 1720;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod2_InputArguments = 1722;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod2_OutputArguments = 1723;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod3_InputArguments = 1725;

        /// <remarks />
        public const uint Data_Static_MethodTest_ArrayMethod3_OutputArguments = 1726;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserScalarMethod1_InputArguments = 1728;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserScalarMethod1_OutputArguments = 1729;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserScalarMethod2_InputArguments = 1731;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserScalarMethod2_OutputArguments = 1732;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserArrayMethod1_InputArguments = 1734;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserArrayMethod1_OutputArguments = 1735;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserArrayMethod2_InputArguments = 1737;

        /// <remarks />
        public const uint Data_Static_MethodTest_UserArrayMethod2_OutputArguments = 1738;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_SimulationActive = 1741;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_GenerateValues_InputArguments = 1743;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_EventId = 1745;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_EventType = 1746;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_SourceNode = 1747;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_SourceName = 1748;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Time = 1749;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_ReceiveTime = 1750;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Message = 1752;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Severity = 1753;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_ConditionClassId = 1754;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_ConditionClassName = 1755;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_ConditionName = 1758;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_BranchId = 1759;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Retain = 1760;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_EnabledState = 1761;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_EnabledState_Id = 1762;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Quality = 1770;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Quality_SourceTimestamp = 1771;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_LastSeverity = 1772;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_LastSeverity_SourceTimestamp = 1773;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Comment = 1774;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Comment_SourceTimestamp = 1775;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_ClientUserId = 1776;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_AddComment_InputArguments = 1780;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_AckedState = 1781;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_AckedState_Id = 1782;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_ConfirmedState_Id = 1791;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Acknowledge_InputArguments = 1800;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_CycleComplete_Confirm_InputArguments = 1802;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_BooleanValue = 1803;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_SByteValue = 1804;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_ByteValue = 1805;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_Int16Value = 1806;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_UInt16Value = 1807;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_Int32Value = 1808;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_UInt32Value = 1809;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_Int64Value = 1810;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_UInt64Value = 1811;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_FloatValue = 1812;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_DoubleValue = 1813;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_StringValue = 1814;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_DateTimeValue = 1815;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_GuidValue = 1816;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_ByteStringValue = 1817;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_XmlElementValue = 1818;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_NodeIdValue = 1819;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_ExpandedNodeIdValue = 1820;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_QualifiedNameValue = 1821;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_LocalizedTextValue = 1822;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_StatusCodeValue = 1823;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VariantValue = 1824;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_EnumerationValue = 1825;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_StructureValue = 1826;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_NumberValue = 1827;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_IntegerValue = 1828;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_UIntegerValue = 1829;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VectorValue = 1830;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VectorValue_X = 1831;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VectorValue_Y = 1832;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VectorValue_Z = 1833;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VectorUnionValue = 2597;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_VectorWithOptionalFieldsValue = 2598;

        /// <remarks />
        public const uint Data_Dynamic_Scalar_MultipleVectorsValue = 2599;

        /// <remarks />
        public const uint Data_Dynamic_Structure_SimulationActive = 1835;

        /// <remarks />
        public const uint Data_Dynamic_Structure_GenerateValues_InputArguments = 1837;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_EventId = 1839;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_EventType = 1840;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_SourceNode = 1841;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_SourceName = 1842;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Time = 1843;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_ReceiveTime = 1844;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Message = 1846;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Severity = 1847;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_ConditionClassId = 1848;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_ConditionClassName = 1849;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_ConditionName = 1852;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_BranchId = 1853;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Retain = 1854;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_EnabledState = 1855;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_EnabledState_Id = 1856;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Quality = 1864;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Quality_SourceTimestamp = 1865;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_LastSeverity = 1866;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_LastSeverity_SourceTimestamp = 1867;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Comment = 1868;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Comment_SourceTimestamp = 1869;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_ClientUserId = 1870;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_AddComment_InputArguments = 1874;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_AckedState = 1875;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_AckedState_Id = 1876;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_ConfirmedState_Id = 1885;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Acknowledge_InputArguments = 1894;

        /// <remarks />
        public const uint Data_Dynamic_Structure_CycleComplete_Confirm_InputArguments = 1896;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure = 1897;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_BooleanValue = 1898;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_SByteValue = 1899;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_ByteValue = 1900;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_Int16Value = 1901;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_UInt16Value = 1902;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_Int32Value = 1903;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_UInt32Value = 1904;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_Int64Value = 1905;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_UInt64Value = 1906;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_FloatValue = 1907;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_DoubleValue = 1908;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_StringValue = 1909;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_DateTimeValue = 1910;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_GuidValue = 1911;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_ByteStringValue = 1912;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_XmlElementValue = 1913;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_NodeIdValue = 1914;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_ExpandedNodeIdValue = 1915;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_QualifiedNameValue = 1916;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_LocalizedTextValue = 1917;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_StatusCodeValue = 1918;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_VariantValue = 1919;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_EnumerationValue = 1920;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_StructureValue = 1921;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_NumberValue = 1922;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_IntegerValue = 1923;

        /// <remarks />
        public const uint Data_Dynamic_Structure_ScalarStructure_UIntegerValue = 1924;

        /// <remarks />
        public const uint Data_Dynamic_Structure_VectorStructure = 1925;

        /// <remarks />
        public const uint Data_Dynamic_Structure_VectorStructure_X = 1926;

        /// <remarks />
        public const uint Data_Dynamic_Structure_VectorStructure_Y = 1927;

        /// <remarks />
        public const uint Data_Dynamic_Structure_VectorStructure_Z = 1928;

        /// <remarks />
        public const uint Data_Dynamic_Array_SimulationActive = 1930;

        /// <remarks />
        public const uint Data_Dynamic_Array_GenerateValues_InputArguments = 1932;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_EventId = 1934;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_EventType = 1935;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_SourceNode = 1936;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_SourceName = 1937;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Time = 1938;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_ReceiveTime = 1939;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Message = 1941;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Severity = 1942;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_ConditionClassId = 1943;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_ConditionClassName = 1944;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_ConditionName = 1947;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_BranchId = 1948;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Retain = 1949;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_EnabledState = 1950;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_EnabledState_Id = 1951;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Quality = 1959;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Quality_SourceTimestamp = 1960;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_LastSeverity = 1961;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_LastSeverity_SourceTimestamp = 1962;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Comment = 1963;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Comment_SourceTimestamp = 1964;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_ClientUserId = 1965;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_AddComment_InputArguments = 1969;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_AckedState = 1970;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_AckedState_Id = 1971;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_ConfirmedState_Id = 1980;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Acknowledge_InputArguments = 1989;

        /// <remarks />
        public const uint Data_Dynamic_Array_CycleComplete_Confirm_InputArguments = 1991;

        /// <remarks />
        public const uint Data_Dynamic_Array_BooleanValue = 1992;

        /// <remarks />
        public const uint Data_Dynamic_Array_SByteValue = 1993;

        /// <remarks />
        public const uint Data_Dynamic_Array_ByteValue = 1994;

        /// <remarks />
        public const uint Data_Dynamic_Array_Int16Value = 1995;

        /// <remarks />
        public const uint Data_Dynamic_Array_UInt16Value = 1996;

        /// <remarks />
        public const uint Data_Dynamic_Array_Int32Value = 1997;

        /// <remarks />
        public const uint Data_Dynamic_Array_UInt32Value = 1998;

        /// <remarks />
        public const uint Data_Dynamic_Array_Int64Value = 1999;

        /// <remarks />
        public const uint Data_Dynamic_Array_UInt64Value = 2000;

        /// <remarks />
        public const uint Data_Dynamic_Array_FloatValue = 2001;

        /// <remarks />
        public const uint Data_Dynamic_Array_DoubleValue = 2002;

        /// <remarks />
        public const uint Data_Dynamic_Array_StringValue = 2003;

        /// <remarks />
        public const uint Data_Dynamic_Array_DateTimeValue = 2004;

        /// <remarks />
        public const uint Data_Dynamic_Array_GuidValue = 2005;

        /// <remarks />
        public const uint Data_Dynamic_Array_ByteStringValue = 2006;

        /// <remarks />
        public const uint Data_Dynamic_Array_XmlElementValue = 2007;

        /// <remarks />
        public const uint Data_Dynamic_Array_NodeIdValue = 2008;

        /// <remarks />
        public const uint Data_Dynamic_Array_ExpandedNodeIdValue = 2009;

        /// <remarks />
        public const uint Data_Dynamic_Array_QualifiedNameValue = 2010;

        /// <remarks />
        public const uint Data_Dynamic_Array_LocalizedTextValue = 2011;

        /// <remarks />
        public const uint Data_Dynamic_Array_StatusCodeValue = 2012;

        /// <remarks />
        public const uint Data_Dynamic_Array_VariantValue = 2013;

        /// <remarks />
        public const uint Data_Dynamic_Array_EnumerationValue = 2014;

        /// <remarks />
        public const uint Data_Dynamic_Array_StructureValue = 2015;

        /// <remarks />
        public const uint Data_Dynamic_Array_NumberValue = 2016;

        /// <remarks />
        public const uint Data_Dynamic_Array_IntegerValue = 2017;

        /// <remarks />
        public const uint Data_Dynamic_Array_UIntegerValue = 2018;

        /// <remarks />
        public const uint Data_Dynamic_Array_VectorValue = 2019;

        /// <remarks />
        public const uint Data_Dynamic_Array_VectorUnionValue = 2600;

        /// <remarks />
        public const uint Data_Dynamic_Array_VectorWithOptionalFieldsValue = 2601;

        /// <remarks />
        public const uint Data_Dynamic_Array_MultipleVectorsValue = 2602;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_SimulationActive = 2021;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_GenerateValues_InputArguments = 2023;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_EventId = 2025;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_EventType = 2026;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_SourceNode = 2027;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_SourceName = 2028;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Time = 2029;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_ReceiveTime = 2030;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Message = 2032;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Severity = 2033;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_ConditionClassId = 2034;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_ConditionClassName = 2035;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_ConditionName = 2038;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_BranchId = 2039;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Retain = 2040;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_EnabledState = 2041;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_EnabledState_Id = 2042;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Quality = 2050;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Quality_SourceTimestamp = 2051;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_LastSeverity = 2052;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_LastSeverity_SourceTimestamp = 2053;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Comment = 2054;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Comment_SourceTimestamp = 2055;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_ClientUserId = 2056;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_AddComment_InputArguments = 2060;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_AckedState = 2061;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_AckedState_Id = 2062;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_ConfirmedState_Id = 2071;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Acknowledge_InputArguments = 2080;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_CycleComplete_Confirm_InputArguments = 2082;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_BooleanValue = 2083;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_SByteValue = 2084;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_ByteValue = 2085;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_Int16Value = 2086;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_UInt16Value = 2087;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_Int32Value = 2088;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_UInt32Value = 2089;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_Int64Value = 2090;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_UInt64Value = 2091;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_FloatValue = 2092;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_DoubleValue = 2093;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_StringValue = 2094;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_DateTimeValue = 2095;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_GuidValue = 2096;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_ByteStringValue = 2097;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_XmlElementValue = 2098;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_NodeIdValue = 2099;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_ExpandedNodeIdValue = 2100;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_QualifiedNameValue = 2101;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_LocalizedTextValue = 2102;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_StatusCodeValue = 2103;

        /// <remarks />
        public const uint Data_Dynamic_UserScalar_VariantValue = 2104;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_SimulationActive = 2106;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_GenerateValues_InputArguments = 2108;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_EventId = 2110;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_EventType = 2111;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_SourceNode = 2112;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_SourceName = 2113;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Time = 2114;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_ReceiveTime = 2115;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Message = 2117;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Severity = 2118;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_ConditionClassId = 2119;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_ConditionClassName = 2120;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_ConditionName = 2123;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_BranchId = 2124;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Retain = 2125;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_EnabledState = 2126;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_EnabledState_Id = 2127;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Quality = 2135;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Quality_SourceTimestamp = 2136;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_LastSeverity = 2137;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_LastSeverity_SourceTimestamp = 2138;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Comment = 2139;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Comment_SourceTimestamp = 2140;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_ClientUserId = 2141;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_AddComment_InputArguments = 2145;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_AckedState = 2146;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_AckedState_Id = 2147;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_ConfirmedState_Id = 2156;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Acknowledge_InputArguments = 2165;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_CycleComplete_Confirm_InputArguments = 2167;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_BooleanValue = 2168;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_SByteValue = 2169;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_ByteValue = 2170;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_Int16Value = 2171;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_UInt16Value = 2172;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_Int32Value = 2173;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_UInt32Value = 2174;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_Int64Value = 2175;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_UInt64Value = 2176;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_FloatValue = 2177;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_DoubleValue = 2178;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_StringValue = 2179;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_DateTimeValue = 2180;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_GuidValue = 2181;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_ByteStringValue = 2182;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_XmlElementValue = 2183;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_NodeIdValue = 2184;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_ExpandedNodeIdValue = 2185;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_QualifiedNameValue = 2186;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_LocalizedTextValue = 2187;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_StatusCodeValue = 2188;

        /// <remarks />
        public const uint Data_Dynamic_UserArray_VariantValue = 2189;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_SimulationActive = 2191;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_GenerateValues_InputArguments = 2193;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_EventId = 2195;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_EventType = 2196;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_SourceNode = 2197;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_SourceName = 2198;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Time = 2199;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_ReceiveTime = 2200;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Message = 2202;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Severity = 2203;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_ConditionClassId = 2204;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_ConditionClassName = 2205;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_ConditionName = 2208;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_BranchId = 2209;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Retain = 2210;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_EnabledState = 2211;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_EnabledState_Id = 2212;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Quality = 2220;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Quality_SourceTimestamp = 2221;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_LastSeverity = 2222;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_LastSeverity_SourceTimestamp = 2223;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Comment = 2224;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Comment_SourceTimestamp = 2225;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_ClientUserId = 2226;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_AddComment_InputArguments = 2230;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_AckedState = 2231;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_AckedState_Id = 2232;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_ConfirmedState_Id = 2241;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Acknowledge_InputArguments = 2250;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_CycleComplete_Confirm_InputArguments = 2252;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_SByteValue = 2253;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_SByteValue_EURange = 2257;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_ByteValue = 2259;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_ByteValue_EURange = 2263;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_Int16Value = 2265;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_Int16Value_EURange = 2269;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UInt16Value = 2271;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UInt16Value_EURange = 2275;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_Int32Value = 2277;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_Int32Value_EURange = 2281;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UInt32Value = 2283;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UInt32Value_EURange = 2287;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_Int64Value = 2289;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_Int64Value_EURange = 2293;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UInt64Value = 2295;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UInt64Value_EURange = 2299;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_FloatValue = 2301;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_FloatValue_EURange = 2305;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_DoubleValue = 2307;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_DoubleValue_EURange = 2311;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_NumberValue = 2313;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_NumberValue_EURange = 2317;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_IntegerValue = 2319;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_IntegerValue_EURange = 2323;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UIntegerValue = 2325;

        /// <remarks />
        public const uint Data_Dynamic_AnalogScalar_UIntegerValue_EURange = 2329;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_SimulationActive = 2332;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_GenerateValues_InputArguments = 2334;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_EventId = 2336;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_EventType = 2337;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_SourceNode = 2338;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_SourceName = 2339;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Time = 2340;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_ReceiveTime = 2341;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Message = 2343;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Severity = 2344;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_ConditionClassId = 2345;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_ConditionClassName = 2346;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_ConditionName = 2349;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_BranchId = 2350;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Retain = 2351;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_EnabledState = 2352;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_EnabledState_Id = 2353;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Quality = 2361;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Quality_SourceTimestamp = 2362;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_LastSeverity = 2363;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_LastSeverity_SourceTimestamp = 2364;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Comment = 2365;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Comment_SourceTimestamp = 2366;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_ClientUserId = 2367;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_AddComment_InputArguments = 2371;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_AckedState = 2372;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_AckedState_Id = 2373;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_ConfirmedState_Id = 2382;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Acknowledge_InputArguments = 2391;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_CycleComplete_Confirm_InputArguments = 2393;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_SByteValue = 2394;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_SByteValue_EURange = 2398;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_ByteValue = 2400;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_ByteValue_EURange = 2404;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_Int16Value = 2406;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_Int16Value_EURange = 2410;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UInt16Value = 2412;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UInt16Value_EURange = 2416;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_Int32Value = 2418;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_Int32Value_EURange = 2422;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UInt32Value = 2424;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UInt32Value_EURange = 2428;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_Int64Value = 2430;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_Int64Value_EURange = 2434;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UInt64Value = 2436;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UInt64Value_EURange = 2440;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_FloatValue = 2442;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_FloatValue_EURange = 2446;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_DoubleValue = 2448;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_DoubleValue_EURange = 2452;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_NumberValue = 2454;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_NumberValue_EURange = 2458;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_IntegerValue = 2460;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_IntegerValue_EURange = 2464;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UIntegerValue = 2466;

        /// <remarks />
        public const uint Data_Dynamic_AnalogArray_UIntegerValue_EURange = 2470;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_EventId = 2474;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_EventType = 2475;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_SourceNode = 2476;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_SourceName = 2477;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Time = 2478;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_ReceiveTime = 2479;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Message = 2481;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Severity = 2482;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_ConditionClassId = 2483;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_ConditionClassName = 2484;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_ConditionName = 2487;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_BranchId = 2488;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Retain = 2489;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_EnabledState = 2490;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_EnabledState_Id = 2491;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Quality = 2499;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Quality_SourceTimestamp = 2500;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_LastSeverity = 2501;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_LastSeverity_SourceTimestamp = 2502;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Comment = 2503;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_Comment_SourceTimestamp = 2504;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_ClientUserId = 2505;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_AddComment_InputArguments = 2509;

        /// <remarks />
        public const uint Data_Conditions_SystemStatus_MonitoredNodeCount = 2510;

        /// <remarks />
        public const uint TestData_BinarySchema = 2518;

        /// <remarks />
        public const uint TestData_BinarySchema_NamespaceUri = 2520;

        /// <remarks />
        public const uint TestData_BinarySchema_Deprecated = 2521;

        /// <remarks />
        public const uint TestData_BinarySchema_ScalarStructureDataType = 2522;

        /// <remarks />
        public const uint TestData_BinarySchema_ArrayValueDataType = 2525;

        /// <remarks />
        public const uint TestData_BinarySchema_UserScalarValueDataType = 2528;

        /// <remarks />
        public const uint TestData_BinarySchema_UserArrayValueDataType = 2531;

        /// <remarks />
        public const uint TestData_BinarySchema_Vector = 2534;

        /// <remarks />
        public const uint TestData_BinarySchema_VectorUnion = 2606;

        /// <remarks />
        public const uint TestData_BinarySchema_VectorWithOptionalFields = 2609;

        /// <remarks />
        public const uint TestData_BinarySchema_MultipleVectors = 2612;

        /// <remarks />
        public const uint TestData_BinarySchema_WorkOrderStatusType = 2537;

        /// <remarks />
        public const uint TestData_BinarySchema_WorkOrderType = 2540;

        /// <remarks />
        public const uint TestData_XmlSchema = 2550;

        /// <remarks />
        public const uint TestData_XmlSchema_NamespaceUri = 2552;

        /// <remarks />
        public const uint TestData_XmlSchema_Deprecated = 2553;

        /// <remarks />
        public const uint TestData_XmlSchema_ScalarStructureDataType = 2554;

        /// <remarks />
        public const uint TestData_XmlSchema_ArrayValueDataType = 2557;

        /// <remarks />
        public const uint TestData_XmlSchema_UserScalarValueDataType = 2560;

        /// <remarks />
        public const uint TestData_XmlSchema_UserArrayValueDataType = 2563;

        /// <remarks />
        public const uint TestData_XmlSchema_Vector = 2566;

        /// <remarks />
        public const uint TestData_XmlSchema_VectorUnion = 2618;

        /// <remarks />
        public const uint TestData_XmlSchema_VectorWithOptionalFields = 2621;

        /// <remarks />
        public const uint TestData_XmlSchema_MultipleVectors = 2624;

        /// <remarks />
        public const uint TestData_XmlSchema_WorkOrderStatusType = 2569;

        /// <remarks />
        public const uint TestData_XmlSchema_WorkOrderType = 2572;
    }
    #endregion

    #region VariableType Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableTypes
    {
        /// <remarks />
        public const uint ScalarStructureVariableType = 79;

        /// <remarks />
        public const uint VectorVariableType = 889;
    }
    #endregion

    #region DataType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.ScalarStructureDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.ArrayValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId BooleanDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.BooleanDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId SByteDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.SByteDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ByteDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.ByteDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Int16DataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.Int16DataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UInt16DataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.UInt16DataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Int32DataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.Int32DataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UInt32DataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.UInt32DataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Int64DataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.Int64DataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UInt64DataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.UInt64DataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId FloatDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.FloatDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId DoubleDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.DoubleDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StringDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.StringDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId DateTimeDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.DateTimeDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId GuidDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.GuidDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ByteStringDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.ByteStringDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId XmlElementDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.XmlElementDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId NodeIdDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.NodeIdDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ExpandedNodeIdDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.ExpandedNodeIdDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId QualifiedNameDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.QualifiedNameDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId LocalizedTextDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.LocalizedTextDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StatusCodeDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.StatusCodeDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VariantDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.VariantDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.UserScalarValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.UserArrayValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Vector = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.Vector, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorUnion = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.VectorUnion, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorWithOptionalFields = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.VectorWithOptionalFields, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MultipleVectors = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.MultipleVectors, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderStatusType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.WorkOrderStatusType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.DataTypes.WorkOrderType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);
    }
    #endregion

    #region Method Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class MethodIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.TestDataObjectType_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.TestDataObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.TestDataObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.TestDataObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.TestDataObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ScalarValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ScalarValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ScalarValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ScalarValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.StructureValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.StructureValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.StructureValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.StructureValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogScalarValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogScalarValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogScalarValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogScalarValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ArrayValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ArrayValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ArrayValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.ArrayValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogArrayValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogArrayValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogArrayValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.AnalogArrayValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserScalarValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserScalarValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserScalarValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserScalarValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserArrayValueObjectType_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserArrayValueObjectType_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserArrayValueObjectType_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.UserArrayValueObjectType_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_ScalarMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_ScalarMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod3 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_ScalarMethod3, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_ArrayMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_ArrayMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod3 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_ArrayMethod3, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserScalarMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_UserScalarMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserScalarMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_UserScalarMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserArrayMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_UserArrayMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserArrayMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.MethodTestType_UserArrayMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Scalar_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Scalar_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Scalar_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Scalar_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Scalar_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Structure_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Structure_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Structure_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Structure_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Structure_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Array_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Array_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Array_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Array_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_Array_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserScalar_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserScalar_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserScalar_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserScalar_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserScalar_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserArray_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserArray_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserArray_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserArray_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_UserArray_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogScalar_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogScalar_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogScalar_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogScalar_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogScalar_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogArray_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogArray_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogArray_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogArray_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_AnalogArray_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_ScalarMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_ScalarMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod3 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_ScalarMethod3, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_ArrayMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_ArrayMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod3 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_ArrayMethod3, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserScalarMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_UserScalarMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserScalarMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_UserScalarMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserArrayMethod1 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_UserArrayMethod1, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserArrayMethod2 = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Static_MethodTest_UserArrayMethod2, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Scalar_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Scalar_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Scalar_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Scalar_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Scalar_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Structure_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Structure_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Structure_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Structure_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Structure_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Array_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Array_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Array_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Array_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_Array_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserScalar_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserScalar_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserScalar_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserScalar_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserScalar_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserArray_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserArray_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserArray_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserArray_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_UserArray_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogScalar_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogScalar_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogScalar_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogScalar_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogScalar_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_GenerateValues = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogArray_GenerateValues, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogArray_CycleComplete_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogArray_CycleComplete_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogArray_CycleComplete_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Acknowledge = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Dynamic_AnalogArray_CycleComplete_Acknowledge, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Disable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Conditions_SystemStatus_Disable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Enable = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Conditions_SystemStatus_Enable, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_AddComment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Methods.Data_Conditions_SystemStatus_AddComment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);
    }
    #endregion

    #region Object Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.TestDataObjectType_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_Scalar, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_Scalar_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_Structure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_Structure_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_Array, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_Array_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_UserScalar, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_UserScalar_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_UserArray, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_UserArray_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_AnalogScalar, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_AnalogScalar_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_AnalogArray, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_AnalogArray_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Static_MethodTest, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_Scalar, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_Scalar_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_Structure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_Structure_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_Array, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_Array_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_UserScalar, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_UserScalar_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_UserArray, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_UserArray_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_AnalogScalar, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_AnalogScalar_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_AnalogArray, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Dynamic_AnalogArray_CycleComplete, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Conditions, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Data_Conditions_SystemStatus, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.ScalarStructureDataType_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.ArrayValueDataType_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.UserScalarValueDataType_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.UserArrayValueDataType_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Vector_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Vector_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorUnion_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.VectorUnion_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorWithOptionalFields_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.VectorWithOptionalFields_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MultipleVectors_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.MultipleVectors_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderStatusType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.WorkOrderStatusType_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.WorkOrderType_Encoding_DefaultBinary, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.ScalarStructureDataType_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.ArrayValueDataType_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.UserScalarValueDataType_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.UserArrayValueDataType_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Vector_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Vector_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorUnion_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.VectorUnion_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorWithOptionalFields_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.VectorWithOptionalFields_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MultipleVectors_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.MultipleVectors_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderStatusType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.WorkOrderStatusType_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.WorkOrderType_Encoding_DefaultXml, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.ScalarStructureDataType_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.ArrayValueDataType_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.UserScalarValueDataType_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.UserArrayValueDataType_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Vector_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.Vector_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorUnion_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.VectorUnion_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorWithOptionalFields_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.VectorWithOptionalFields_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MultipleVectors_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.MultipleVectors_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderStatusType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.WorkOrderStatusType_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId WorkOrderType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Objects.WorkOrderType_Encoding_DefaultJson, SampleCompany.NodeManagers.TestData.Namespaces.TestData);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId GenerateValuesEventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.GenerateValuesEventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.TestDataObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.ScalarValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.StructureValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.AnalogScalarValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.ArrayValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.AnalogArrayValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.UserScalarValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.UserArrayValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.MethodTestType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.ObjectTypes.TestSystemConditionType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);
    }
    #endregion

    #region Variable Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId GenerateValuesEventType_Iterations = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.GenerateValuesEventType_Iterations, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId GenerateValuesEventType_NewValueCount = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.GenerateValuesEventType_NewValueCount, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestDataObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestDataObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarStructureVariableType_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VectorValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VectorValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VectorValue_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VectorValue_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VectorValue_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VectorValue_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VectorValue_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VectorValue_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VectorUnionValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VectorUnionValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_VectorWithOptionalFieldsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_VectorWithOptionalFieldsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ScalarValueObjectType_MultipleVectorsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ScalarValueObjectType_MultipleVectorsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_ScalarStructure_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_ScalarStructure_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_VectorStructure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_VectorStructure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_VectorStructure_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_VectorStructure_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_VectorStructure_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_VectorStructure_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId StructureValueObjectType_VectorStructure_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.StructureValueObjectType_VectorStructure_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_SByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_SByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_ByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_ByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_Int16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_Int16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UInt16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UInt16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_Int32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_Int32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UInt32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UInt32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_Int64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_Int64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UInt64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UInt64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_FloatValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_FloatValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_DoubleValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_DoubleValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_NumberValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_NumberValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_IntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_IntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogScalarValueObjectType_UIntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogScalarValueObjectType_UIntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_VectorValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_VectorValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_VectorUnionValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_VectorUnionValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_VectorWithOptionalFieldsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_VectorWithOptionalFieldsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId ArrayValueObjectType_MultipleVectorsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.ArrayValueObjectType_MultipleVectorsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_SByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_SByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_ByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_ByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_Int16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_Int16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UInt16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UInt16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_Int32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_Int32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UInt32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UInt32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_Int64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_Int64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UInt64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UInt64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_FloatValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_FloatValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_DoubleValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_DoubleValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_NumberValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_NumberValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_IntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_IntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId AnalogArrayValueObjectType_UIntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.AnalogArrayValueObjectType_UIntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserScalarValueObjectType_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserScalarValueObjectType_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId UserArrayValueObjectType_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.UserArrayValueObjectType_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorVariableType_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.VectorVariableType_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorVariableType_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.VectorVariableType_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorVariableType_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.VectorVariableType_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ScalarMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ScalarMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ScalarMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ScalarMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod3_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ScalarMethod3_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ScalarMethod3_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ScalarMethod3_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ArrayMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ArrayMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ArrayMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ArrayMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod3_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ArrayMethod3_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_ArrayMethod3_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_ArrayMethod3_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserScalarMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserScalarMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserScalarMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserScalarMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserScalarMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserScalarMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserScalarMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserScalarMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserArrayMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserArrayMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserArrayMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserArrayMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserArrayMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserArrayMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId MethodTestType_UserArrayMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.MethodTestType_UserArrayMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_ConditionRefresh_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_ConditionRefresh_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_ConditionRefresh2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_ConditionRefresh2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestSystemConditionType_MonitoredNodeCount = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestSystemConditionType_MonitoredNodeCount, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VectorValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VectorValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VectorValue_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VectorValue_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VectorValue_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VectorValue_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VectorValue_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VectorValue_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VectorUnionValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VectorUnionValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_VectorWithOptionalFieldsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_VectorWithOptionalFieldsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Scalar_MultipleVectorsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Scalar_MultipleVectorsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_ScalarStructure_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_ScalarStructure_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_VectorStructure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_VectorStructure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_VectorStructure_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_VectorStructure_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_VectorStructure_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_VectorStructure_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Structure_VectorStructure_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Structure_VectorStructure_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_VectorValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_VectorValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_VectorUnionValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_VectorUnionValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_VectorWithOptionalFieldsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_VectorWithOptionalFieldsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_Array_MultipleVectorsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_Array_MultipleVectorsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserScalar_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserScalar_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_UserArray_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_UserArray_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_SByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_SByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_ByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_ByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_Int16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_Int16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UInt16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UInt16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_Int32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_Int32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UInt32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UInt32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_Int64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_Int64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UInt64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UInt64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_FloatValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_FloatValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_DoubleValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_DoubleValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_NumberValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_NumberValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_IntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_IntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogScalar_UIntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogScalar_UIntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_SByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_SByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_ByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_ByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_Int16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_Int16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UInt16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UInt16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_Int32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_Int32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UInt32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UInt32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_Int64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_Int64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UInt64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UInt64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_FloatValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_FloatValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_DoubleValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_DoubleValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_NumberValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_NumberValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_IntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_IntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_AnalogArray_UIntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_AnalogArray_UIntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ScalarMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ScalarMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ScalarMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ScalarMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod3_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ScalarMethod3_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ScalarMethod3_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ScalarMethod3_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ArrayMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ArrayMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ArrayMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ArrayMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod3_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ArrayMethod3_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_ArrayMethod3_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_ArrayMethod3_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserScalarMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserScalarMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserScalarMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserScalarMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserScalarMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserScalarMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserScalarMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserScalarMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserArrayMethod1_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserArrayMethod1_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserArrayMethod1_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserArrayMethod1_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserArrayMethod2_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserArrayMethod2_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Static_MethodTest_UserArrayMethod2_OutputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Static_MethodTest_UserArrayMethod2_OutputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VectorValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VectorValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VectorValue_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VectorValue_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VectorValue_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VectorValue_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VectorValue_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VectorValue_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VectorUnionValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VectorUnionValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_VectorWithOptionalFieldsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_VectorWithOptionalFieldsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Scalar_MultipleVectorsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Scalar_MultipleVectorsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_ScalarStructure_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_ScalarStructure_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_VectorStructure = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_VectorStructure, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_VectorStructure_X = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_VectorStructure_X, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_VectorStructure_Y = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_VectorStructure_Y, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Structure_VectorStructure_Z = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Structure_VectorStructure_Z, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_EnumerationValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_EnumerationValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_StructureValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_StructureValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_VectorValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_VectorValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_VectorUnionValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_VectorUnionValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_VectorWithOptionalFieldsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_VectorWithOptionalFieldsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_Array_MultipleVectorsValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_Array_MultipleVectorsValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserScalar_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserScalar_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_BooleanValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_BooleanValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_StringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_StringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_DateTimeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_DateTimeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_GuidValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_GuidValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_ByteStringValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_ByteStringValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_XmlElementValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_XmlElementValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_NodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_NodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_ExpandedNodeIdValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_ExpandedNodeIdValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_QualifiedNameValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_QualifiedNameValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_LocalizedTextValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_LocalizedTextValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_StatusCodeValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_StatusCodeValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_UserArray_VariantValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_UserArray_VariantValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_SByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_SByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_ByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_ByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_Int16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_Int16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UInt16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UInt16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_Int32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_Int32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UInt32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UInt32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_Int64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_Int64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UInt64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UInt64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_FloatValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_FloatValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_DoubleValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_DoubleValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_NumberValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_NumberValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_IntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_IntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogScalar_UIntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogScalar_UIntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_SimulationActive = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_SimulationActive, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_GenerateValues_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_GenerateValues_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_AckedState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_AckedState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_AckedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_AckedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_ConfirmedState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_ConfirmedState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Acknowledge_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Acknowledge_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_CycleComplete_Confirm_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_CycleComplete_Confirm_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_SByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_SByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_SByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_SByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_ByteValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_ByteValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_ByteValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_ByteValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_Int16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_Int16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_Int16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_Int16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UInt16Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UInt16Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UInt16Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UInt16Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_Int32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_Int32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_Int32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_Int32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UInt32Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UInt32Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UInt32Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UInt32Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_Int64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_Int64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_Int64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_Int64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UInt64Value = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UInt64Value, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UInt64Value_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UInt64Value_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_FloatValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_FloatValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_FloatValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_FloatValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_DoubleValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_DoubleValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_DoubleValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_DoubleValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_NumberValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_NumberValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_NumberValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_NumberValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_IntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_IntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_IntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_IntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UIntegerValue = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UIntegerValue, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Dynamic_AnalogArray_UIntegerValue_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Dynamic_AnalogArray_UIntegerValue_EURange, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_EventId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_EventId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_EventType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_EventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_SourceNode = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_SourceNode, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_SourceName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_SourceName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Time = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Time, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_ReceiveTime = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_ReceiveTime, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Message = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Message, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Severity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Severity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_ConditionClassId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_ConditionClassId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_ConditionClassName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_ConditionClassName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_ConditionName = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_ConditionName, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_BranchId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_BranchId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Retain = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Retain, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_EnabledState = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_EnabledState, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_EnabledState_Id = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_EnabledState_Id, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Quality = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Quality, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Quality_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Quality_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_LastSeverity = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_LastSeverity, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_LastSeverity_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_LastSeverity_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Comment = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Comment, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_Comment_SourceTimestamp = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_Comment_SourceTimestamp, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_ClientUserId = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_ClientUserId, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_AddComment_InputArguments = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_AddComment_InputArguments, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId Data_Conditions_SystemStatus_MonitoredNodeCount = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.Data_Conditions_SystemStatus_MonitoredNodeCount, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_NamespaceUri = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_NamespaceUri, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_Deprecated = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_Deprecated, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_ScalarStructureDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_ScalarStructureDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_ArrayValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_ArrayValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_UserScalarValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_UserScalarValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_UserArrayValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_UserArrayValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_Vector = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_Vector, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_VectorUnion = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_VectorUnion, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_VectorWithOptionalFields = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_VectorWithOptionalFields, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_MultipleVectors = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_MultipleVectors, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_WorkOrderStatusType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_WorkOrderStatusType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_BinarySchema_WorkOrderType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_BinarySchema_WorkOrderType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_NamespaceUri = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_NamespaceUri, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_Deprecated = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_Deprecated, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_ScalarStructureDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_ScalarStructureDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_ArrayValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_ArrayValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_UserScalarValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_UserScalarValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_UserArrayValueDataType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_UserArrayValueDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_Vector = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_Vector, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_VectorUnion = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_VectorUnion, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_VectorWithOptionalFields = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_VectorWithOptionalFields, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_MultipleVectors = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_MultipleVectors, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_WorkOrderStatusType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_WorkOrderStatusType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId TestData_XmlSchema_WorkOrderType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.Variables.TestData_XmlSchema_WorkOrderType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);
    }
    #endregion

    #region VariableType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId ScalarStructureVariableType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.VariableTypes.ScalarStructureVariableType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);

        /// <remarks />
        public static readonly ExpandedNodeId VectorVariableType = new ExpandedNodeId(SampleCompany.NodeManagers.TestData.VariableTypes.VectorVariableType, SampleCompany.NodeManagers.TestData.Namespaces.TestData);
    }
    #endregion

    #region BrowseName Declarations
    /// <remarks />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class BrowseNames
    {
        /// <remarks />
        public const string AnalogArrayValueObjectType = "AnalogArrayValueObjectType";

        /// <remarks />
        public const string AnalogScalarValueObjectType = "AnalogScalarValueObjectType";

        /// <remarks />
        public const string ArrayMethod1 = "ArrayMethod1";

        /// <remarks />
        public const string ArrayMethod2 = "ArrayMethod2";

        /// <remarks />
        public const string ArrayMethod3 = "ArrayMethod3";

        /// <remarks />
        public const string ArrayValueDataType = "ArrayValueDataType";

        /// <remarks />
        public const string ArrayValueObjectType = "ArrayValueObjectType";

        /// <remarks />
        public const string BooleanDataType = "BooleanDataType";

        /// <remarks />
        public const string BooleanValue = "BooleanValue";

        /// <remarks />
        public const string ByteDataType = "ByteDataType";

        /// <remarks />
        public const string ByteStringDataType = "ByteStringDataType";

        /// <remarks />
        public const string ByteStringValue = "ByteStringValue";

        /// <remarks />
        public const string ByteValue = "ByteValue";

        /// <remarks />
        public const string Conditions = "Conditions";

        /// <remarks />
        public const string CycleComplete = "CycleComplete";

        /// <remarks />
        public const string Data = "Data";

        /// <remarks />
        public const string DateTimeDataType = "DateTimeDataType";

        /// <remarks />
        public const string DateTimeValue = "DateTimeValue";

        /// <remarks />
        public const string DoubleDataType = "DoubleDataType";

        /// <remarks />
        public const string DoubleValue = "DoubleValue";

        /// <remarks />
        public const string Dynamic = "Dynamic";

        /// <remarks />
        public const string EnumerationValue = "EnumerationValue";

        /// <remarks />
        public const string ExpandedNodeIdDataType = "ExpandedNodeIdDataType";

        /// <remarks />
        public const string ExpandedNodeIdValue = "ExpandedNodeIdValue";

        /// <remarks />
        public const string FloatDataType = "FloatDataType";

        /// <remarks />
        public const string FloatValue = "FloatValue";

        /// <remarks />
        public const string GenerateValues = "GenerateValues";

        /// <remarks />
        public const string GenerateValuesEventType = "GenerateValuesEventType";

        /// <remarks />
        public const string GuidDataType = "GuidDataType";

        /// <remarks />
        public const string GuidValue = "GuidValue";

        /// <remarks />
        public const string Int16DataType = "Int16DataType";

        /// <remarks />
        public const string Int16Value = "Int16Value";

        /// <remarks />
        public const string Int32DataType = "Int32DataType";

        /// <remarks />
        public const string Int32Value = "Int32Value";

        /// <remarks />
        public const string Int64DataType = "Int64DataType";

        /// <remarks />
        public const string Int64Value = "Int64Value";

        /// <remarks />
        public const string IntegerValue = "IntegerValue";

        /// <remarks />
        public const string Iterations = "Iterations";

        /// <remarks />
        public const string LocalizedTextDataType = "LocalizedTextDataType";

        /// <remarks />
        public const string LocalizedTextValue = "LocalizedTextValue";

        /// <remarks />
        public const string MethodTestType = "MethodTestType";

        /// <remarks />
        public const string MonitoredNodeCount = "MonitoredNodeCount";

        /// <remarks />
        public const string MultipleVectors = "MultipleVectors";

        /// <remarks />
        public const string MultipleVectorsValue = "MultipleVectorsValue";

        /// <remarks />
        public const string NewValueCount = "NewValueCount";

        /// <remarks />
        public const string NodeIdDataType = "NodeIdDataType";

        /// <remarks />
        public const string NodeIdValue = "NodeIdValue";

        /// <remarks />
        public const string NumberValue = "NumberValue";

        /// <remarks />
        public const string QualifiedNameDataType = "QualifiedNameDataType";

        /// <remarks />
        public const string QualifiedNameValue = "QualifiedNameValue";

        /// <remarks />
        public const string SByteDataType = "SByteDataType";

        /// <remarks />
        public const string SByteValue = "SByteValue";

        /// <remarks />
        public const string ScalarMethod1 = "ScalarMethod1";

        /// <remarks />
        public const string ScalarMethod2 = "ScalarMethod2";

        /// <remarks />
        public const string ScalarMethod3 = "ScalarMethod3";

        /// <remarks />
        public const string ScalarStructure = "ScalarStructure";

        /// <remarks />
        public const string ScalarStructureDataType = "ScalarStructureDataType";

        /// <remarks />
        public const string ScalarStructureVariableType = "ScalarStructureVariableType";

        /// <remarks />
        public const string ScalarValueObjectType = "ScalarValueObjectType";

        /// <remarks />
        public const string SimulationActive = "SimulationActive";

        /// <remarks />
        public const string Static = "Static";

        /// <remarks />
        public const string StatusCodeDataType = "StatusCodeDataType";

        /// <remarks />
        public const string StatusCodeValue = "StatusCodeValue";

        /// <remarks />
        public const string StringDataType = "StringDataType";

        /// <remarks />
        public const string StringValue = "StringValue";

        /// <remarks />
        public const string StructureValue = "StructureValue";

        /// <remarks />
        public const string StructureValueObjectType = "StructureValueObjectType";

        /// <remarks />
        public const string TestData_BinarySchema = "SampleCompany.NodeManagers.TestData";

        /// <remarks />
        public const string TestData_XmlSchema = "SampleCompany.NodeManagers.TestData";

        /// <remarks />
        public const string TestDataObjectType = "TestDataObjectType";

        /// <remarks />
        public const string TestSystemConditionType = "TestSystemConditionType";

        /// <remarks />
        public const string UInt16DataType = "UInt16DataType";

        /// <remarks />
        public const string UInt16Value = "UInt16Value";

        /// <remarks />
        public const string UInt32DataType = "UInt32DataType";

        /// <remarks />
        public const string UInt32Value = "UInt32Value";

        /// <remarks />
        public const string UInt64DataType = "UInt64DataType";

        /// <remarks />
        public const string UInt64Value = "UInt64Value";

        /// <remarks />
        public const string UIntegerValue = "UIntegerValue";

        /// <remarks />
        public const string UserArrayMethod1 = "UserArrayMethod1";

        /// <remarks />
        public const string UserArrayMethod2 = "UserArrayMethod2";

        /// <remarks />
        public const string UserArrayValueDataType = "UserArrayValueDataType";

        /// <remarks />
        public const string UserArrayValueObjectType = "UserArrayValueObjectType";

        /// <remarks />
        public const string UserScalarMethod1 = "UserScalarMethod1";

        /// <remarks />
        public const string UserScalarMethod2 = "UserScalarMethod2";

        /// <remarks />
        public const string UserScalarValueDataType = "UserScalarValueDataType";

        /// <remarks />
        public const string UserScalarValueObjectType = "UserScalarValueObjectType";

        /// <remarks />
        public const string VariantDataType = "VariantDataType";

        /// <remarks />
        public const string VariantValue = "VariantValue";

        /// <remarks />
        public const string Vector = "Vector";

        /// <remarks />
        public const string VectorStructure = "VectorStructure";

        /// <remarks />
        public const string VectorUnion = "VectorUnion";

        /// <remarks />
        public const string VectorUnionValue = "VectorUnionValue";

        /// <remarks />
        public const string VectorValue = "VectorValue";

        /// <remarks />
        public const string VectorVariableType = "VectorVariableType";

        /// <remarks />
        public const string VectorWithOptionalFields = "VectorWithOptionalFields";

        /// <remarks />
        public const string VectorWithOptionalFieldsValue = "VectorWithOptionalFieldsValue";

        /// <remarks />
        public const string WorkOrderStatusType = "WorkOrderStatusType";

        /// <remarks />
        public const string WorkOrderType = "WorkOrderType";

        /// <remarks />
        public const string X = "X";

        /// <remarks />
        public const string XmlElementDataType = "XmlElementDataType";

        /// <remarks />
        public const string XmlElementValue = "XmlElementValue";

        /// <remarks />
        public const string Y = "Y";

        /// <remarks />
        public const string Z = "Z";
    }
    #endregion

    #region Namespace Declarations
    /// <remarks />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Namespaces
    {
        /// <summary>
        /// The URI for the OpcUa namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUa = "http://opcfoundation.org/UA/";

        /// <summary>
        /// The URI for the OpcUaXsd namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUaXsd = "http://opcfoundation.org/UA/2008/02/Types.xsd";

        /// <summary>
        /// The URI for the TestData namespace (.NET code namespace is 'SampleCompany.NodeManagers.TestData').
        /// </summary>
        public const string TestData = "http://samplecompany.com/SampleServer/NodeManagers/TestData";
    }
    #endregion
}