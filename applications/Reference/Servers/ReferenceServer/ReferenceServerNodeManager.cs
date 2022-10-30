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
using System.Xml;
using System.Threading;
using System.Numerics;

using Opc.Ua;
using BrowseNames = Opc.Ua.BrowseNames;
using ObjectIds = Opc.Ua.ObjectIds;
using ReferenceTypeIds = Opc.Ua.ReferenceTypeIds;
using ReferenceTypes = Opc.Ua.ReferenceTypes;

using Opc.Ua.Test;

using Technosoftware.UaServer;
using Technosoftware.UaStandardServer;
#endregion

namespace Technosoftware.ReferenceServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class ReferenceServerNodeManager : UaStandardNodeManager
    {
        #region Constructors, Destructor, Initialization

        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public ReferenceServerNodeManager(
            IUaServerData uaServer,
            ApplicationConfiguration configuration)
            : base(uaServer, configuration, Namespaces.ReferenceServer)
        {
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            configuration_ = configuration.ParseExtension<ReferenceServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (configuration_ == null)
            {
                configuration_ = new ReferenceServerConfiguration();
            }

            dynamicNodes_ = new List<BaseDataVariableState>();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TBD
            }
            base.Dispose(disposing);
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId Create(ISystemContext context, NodeState node)
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

        #region Private Helper Functions
        private static bool IsUnsignedAnalogType(BuiltInType builtInType)
        {
            if (builtInType == BuiltInType.Byte ||
                builtInType == BuiltInType.UInt16 ||
                builtInType == BuiltInType.UInt32 ||
                builtInType == BuiltInType.UInt64)
            {
                return true;
            }
            return false;
        }

        private static bool IsAnalogType(BuiltInType builtInType)
        {
            switch (builtInType)
            {
                case BuiltInType.Byte:
                case BuiltInType.UInt16:
                case BuiltInType.UInt32:
                case BuiltInType.UInt64:
                case BuiltInType.SByte:
                case BuiltInType.Int16:
                case BuiltInType.Int32:
                case BuiltInType.Int64:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    return true;
            }
            return false;
        }

        private static Opc.Ua.Range GetAnalogRange(BuiltInType builtInType)
        {
            switch (builtInType)
            {
                case BuiltInType.UInt16:
                    return new Opc.Ua.Range(UInt16.MaxValue, UInt16.MinValue);
                case BuiltInType.UInt32:
                    return new Opc.Ua.Range(UInt32.MaxValue, UInt32.MinValue);
                case BuiltInType.UInt64:
                    return new Opc.Ua.Range(UInt64.MaxValue, UInt64.MinValue);
                case BuiltInType.SByte:
                    return new Opc.Ua.Range(SByte.MaxValue, SByte.MinValue);
                case BuiltInType.Int16:
                    return new Opc.Ua.Range(Int16.MaxValue, Int16.MinValue);
                case BuiltInType.Int32:
                    return new Opc.Ua.Range(Int32.MaxValue, Int32.MinValue);
                case BuiltInType.Int64:
                    return new Opc.Ua.Range(Int64.MaxValue, Int64.MinValue);
                case BuiltInType.Float:
                    return new Opc.Ua.Range(Single.MaxValue, Single.MinValue);
                case BuiltInType.Double:
                    return new Opc.Ua.Range(Double.MaxValue, Double.MinValue);
                case BuiltInType.Byte:
                    return new Opc.Ua.Range(Byte.MaxValue, Byte.MinValue);
                default:
                    return new Opc.Ua.Range(SByte.MaxValue, SByte.MinValue);
            }
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
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = References = new List<IReference>();
                }

                var root = CreateFolderState(null, "CTT", "CTT", null);

                var variables = new List<BaseDataVariableState>();

                try
                {
                    #region Scalar_Static
                    ResetRandomGenerator(1);
                    var scalarFolder = CreateFolderState(root, "Scalar", "Scalar", null);
                    var scalarInstructions = CreateBaseDataVariableState(scalarFolder, "Scalar_Instructions", "Scalar_Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    scalarInstructions.Value = "A library of Read/Write Variables of all supported data-types.";
                    variables.Add(scalarInstructions);

                    var staticFolder = CreateFolderState(scalarFolder, "Scalar_Static", "Scalar_Static", null);
                    const string scalarStatic = "Scalar_Static_";
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(100));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(staticFolder, scalarStatic + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(1000));

                    var decimalVariable = CreateBaseDataVariableState(staticFolder, scalarStatic + "Decimal", "Decimal", null, DataTypeIds.DecimalDataType, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    // Set an arbitrary precision decimal value.
                    var largeInteger = BigInteger.Parse("1234567890123546789012345678901234567890123456789012345");
                    var decimalValue = new DecimalDataType {
                        Scale = 100,
                        Value = largeInteger.ToByteArray()
                    };
                    decimalVariable.Value = decimalValue;
                    variables.Add(decimalVariable);
                    #endregion

                    #region Scalar_Static_Arrays
                    ResetRandomGenerator(2);
                    var arraysFolder = CreateFolderState(staticFolder, "Scalar_Static_Arrays", "Arrays", null);
                    const string staticArrays = "Scalar_Static_Arrays_";

                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));

                    var doubleArrayVar = CreateBaseDataVariableState(arraysFolder, staticArrays + "Double", "Double", null, DataTypeIds.Double, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null);
                    // Set the first elements of the array to a smaller value.
                    if (doubleArrayVar.Value is double[] doubleArrayVal)
                    {
                        doubleArrayVal[0] %= 10E+10;
                        doubleArrayVal[1] %= 10E+10;
                        doubleArrayVal[2] %= 10E+10;
                        doubleArrayVal[3] %= 10E+10;
                    }

                    variables.Add(doubleArrayVar);

                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));

                    var floatArrayVar = CreateBaseDataVariableState(arraysFolder, staticArrays + "Float", "Float", null, DataTypeIds.Float, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null);
                    // Set the first elements of the array to a smaller value.
                    if (floatArrayVar.Value is float[] floatArrayVal)
                    {
                        floatArrayVal[0] %= 0xf10E + 4;
                        floatArrayVal[1] %= 0xf10E + 4;
                        floatArrayVal[2] %= 0xf10E + 4;
                        floatArrayVal[3] %= 0xf10E + 4;
                    }

                    variables.Add(floatArrayVar);

                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Number", "Number", null, DataTypeIds.Number, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));

                    var stringArrayVar = CreateBaseDataVariableState(arraysFolder, staticArrays + "String", "String", null, DataTypeIds.String, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null);
                    stringArrayVar.Value = new[] {
                        "Лошадь_ Пурпурово( Змейка( Слон",
                        "猪 绿色 绵羊 大象~ 狗 菠萝 猪鼠",
                        "Лошадь Овцы Голубика Овцы Змейка",
                        "Чернота` Дракон Бело Дракон",
                        "Horse# Black Lemon Lemon Grape",
                        "猫< パイナップル; ドラゴン 犬 モモ",
                        "레몬} 빨간% 자주색 쥐 백색; 들" ,
                        "Yellow Sheep Peach Elephant Cow",
                        "Крыса Корова Свинья Собака Кот",
                        "龙_ 绵羊 大象 芒果; 猫'" };
                    variables.Add(stringArrayVar);

                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    #endregion

                    #region Scalar_Static_Arrays2D
                    ResetRandomGenerator(3);
                    var arrays2DFolder = CreateFolderState(staticFolder, "Scalar_Static_Arrays2D", "Arrays2D", null);
                    const string staticArrays2D = "Scalar_Static_Arrays2D_";
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Double", "Double", null, DataTypeIds.Double, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Float", "Float", null, DataTypeIds.Float, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(1000));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Number", "Number", null, DataTypeIds.Number, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "String", "String", null, DataTypeIds.String, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrays2DFolder, staticArrays2D + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(1000));
                    #endregion

                    #region Scalar_Static_ArrayDynamic
                    ResetRandomGenerator(4);
                    var arrayDynamicFolder = CreateFolderState(staticFolder, "Scalar_Static_ArrayDymamic", "ArrayDymamic", null);
                    const string staticArraysDynamic = "Scalar_Static_ArrayDynamic_";
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Double", "Double", null, DataTypeIds.Double, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Float", "Float", null, DataTypeIds.Float, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(1000));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Number", "Number", null, DataTypeIds.Number, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(1000));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "String", "String", null, DataTypeIds.String, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arrayDynamicFolder, staticArraysDynamic + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.OneOrMoreDimensions, AccessLevels.CurrentReadOrWrite, null).MinimumSamplingInterval(1000));
                    #endregion

                    #region Scalar_Static_Mass
                    ResetRandomGenerator(5);
                    // create 100 instances of each static scalar type
                    var massFolder = CreateFolderState(staticFolder, "Scalar_Static_Mass", "Mass", null);
                    const string staticMass = "Scalar_Static_Mass_";
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, 100));
                    variables.AddRange(CreateVariables(massFolder, staticMass + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, 100));
                    #endregion

                    #region Scalar_Simulation
                    ResetRandomGenerator(6);
                    var simulationFolder = CreateFolderState(scalarFolder, "Scalar_Simulation", "Simulation", null);
                    const string scalarSimulation = "Scalar_Simulation_";
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar);
                    CreateDynamicVariable(simulationFolder, scalarSimulation + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar);

                    var intervalVariable = CreateBaseDataVariableState(simulationFolder, scalarSimulation + "Interval", "Interval", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    intervalVariable.Value = simulationInterval_;
                    intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                    var enabledVariable = CreateBaseDataVariableState(simulationFolder, scalarSimulation + "Enabled", "Enabled", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    enabledVariable.Value = simulationEnabled_;
                    enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                    #endregion

                    #region Scalar_Simulation_Arrays
                    ResetRandomGenerator(7);
                    var arraysSimulationFolder = CreateFolderState(simulationFolder, "Scalar_Simulation_Arrays", "Arrays", null);
                    const string simulationArrays = "Scalar_Simulation_Arrays_";
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Double", "Double", null, DataTypeIds.Double, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Float", "Float", null, DataTypeIds.Float, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Number", "Number", null, DataTypeIds.Number, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "String", "String", null, DataTypeIds.String, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.OneDimension);
                    CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.OneDimension);
                    #endregion

                    #region Scalar_Simulation_Mass
                    ResetRandomGenerator(8);
                    var massSimulationFolder = CreateFolderState(simulationFolder, "Scalar_Simulation_Mass", "Mass", null);
                    const string massSimulation = "Scalar_Simulation_Mass_";
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, 100);
                    CreateDynamicVariables(massSimulationFolder, massSimulation + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, 100);
                    #endregion

                    #region DataAccess_DataItem
                    ResetRandomGenerator(9);
                    var daFolder = CreateFolderState(root, "DataAccess", "DataAccess", null);
                    var daInstructions = CreateBaseDataVariableState(daFolder, "DataAccess_Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    daInstructions.Value = "A library of Read/Write Variables of all supported data-types.";
                    variables.Add(daInstructions);

                    var dataItemFolder = CreateFolderState(daFolder, "DataAccess_DataItem", "DataItem", null);
                    const string daDataItem = "DataAccess_DataItem_";

                    foreach (var name in Enum.GetNames(typeof(BuiltInType)))
                    {
                        var item = CreateDataItemState(dataItemFolder, daDataItem + name, name, null, (BuiltInType)Enum.Parse(typeof(BuiltInType), name), ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, String.Empty, 2, null, null);

                        // set initial value to String.Empty for String node.
                        if (name == BuiltInType.String.ToString())
                        {
                            item.Value = String.Empty;
                        }
                    }
                    #endregion

                    #region DataAccess_AnalogType
                    ResetRandomGenerator(10);
                    var analogItemFolder = CreateFolderState(daFolder, "DataAccess_AnalogType", "AnalogType", null);
                    const string daAnalogItem = "DataAccess_AnalogType_";

                    foreach (var name in Enum.GetNames(typeof(BuiltInType)))
                    {
                        BuiltInType builtInType = (BuiltInType)Enum.Parse(typeof(BuiltInType), name);
                        if (IsAnalogType(builtInType))
                        {
                            var item = CreateAnalogItemState(analogItemFolder, daAnalogItem + name, name, null, builtInType, ValueRanks.Scalar);

                            if (builtInType == BuiltInType.Int64 ||
                                builtInType == BuiltInType.UInt64)
                            {
                                // make test case without optional ranges
                                item.EngineeringUnits = null;
                                item.InstrumentRange = null;
                            }
                            else if (builtInType == BuiltInType.Float)
                            {
                                item.EURange.Value.High = 0;
                                item.EURange.Value.Low = 0;
                            }

                            //set default value for Definition property
                            if (item.Definition != null)
                            {
                                item.Definition.Value = String.Empty;
                            }
                        }
                    }
                    #endregion

                    #region DataAccess_AnalogType_Array
                    ResetRandomGenerator(11);
                    var analogArrayFolder = CreateFolderState(analogItemFolder, "DataAccess_AnalogType_Array", "Array", null);
                    const string daAnalogArray = "DataAccess_AnalogType_Array_";

                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Boolean", "Boolean", null, BuiltInType.Boolean, ValueRanks.OneDimension, new[] { true, false, true, false, true, false, true, false, true });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Byte", "Byte", null, BuiltInType.Byte, ValueRanks.OneDimension, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "ByteString", "ByteString", null, BuiltInType.ByteString, ValueRanks.OneDimension, new[] { new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "DateTime", "DateTime", null, BuiltInType.DateTime, ValueRanks.OneDimension, new[] { DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Double", "Double", null, BuiltInType.Double, ValueRanks.OneDimension, new[] { 9.00001d, 9.0002d, 9.003d, 9.04d, 9.5d, 9.06d, 9.007d, 9.008d, 9.0009d });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.OneDimension, new[] { 9.00001d, 9.0002d, 9.003d, 9.04d, 9.5d, 9.06d, 9.007d, 9.008d, 9.0009d }, null);
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Float", "Float", null, BuiltInType.Float, ValueRanks.OneDimension, new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 1.1f, 2.2f, 3.3f, 4.4f, 5.5f });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Guid", "Guid", null, BuiltInType.Guid, ValueRanks.OneDimension, new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Int16", "Int16", null, BuiltInType.Int16, ValueRanks.OneDimension, new Int16[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Int32", "Int32", null, BuiltInType.Int32, ValueRanks.OneDimension, new Int32[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Int64", "Int64", null, BuiltInType.Int64, ValueRanks.OneDimension, new Int64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Integer", "Integer", null, BuiltInType.Integer, ValueRanks.OneDimension, new Int64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.OneDimension, new string[] { "en", "fr", "de", "en", "fr", "de", "en", "fr", "de", "en" }, null);
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "LocalizedText", "LocalizedText", null, BuiltInType.LocalizedText, ValueRanks.OneDimension, new LocalizedText[] { new LocalizedText("en", "Hello World1"), new LocalizedText("en", "Hello World2"), new LocalizedText("en", "Hello World3"), new LocalizedText("en", "Hello World4"), new LocalizedText("en", "Hello World5"), new LocalizedText("en", "Hello World6"), new LocalizedText("en", "Hello World7"), new LocalizedText("en", "Hello World8"), new LocalizedText("en", "Hello World9"), new LocalizedText("en", "Hello World10") });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "NodeId", "NodeId", null, BuiltInType.NodeId, ValueRanks.OneDimension, new NodeId[] { new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()) });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Number", "Number", null, BuiltInType.Number, ValueRanks.OneDimension, new Int16[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "QualifiedName", "QualifiedName", null, BuiltInType.QualifiedName, ValueRanks.OneDimension, new QualifiedName[] { "q0", "q1", "q2", "q3", "q4", "q5", "q6", "q7", "q8", "q9" });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "SByte", "SByte", null, BuiltInType.SByte, ValueRanks.OneDimension, new SByte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "String", "String", null, BuiltInType.String, ValueRanks.OneDimension, new[] { "a00", "b10", "c20", "d30", "e40", "f50", "g60", "h70", "i80", "j90" });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "UInt16", "UInt16", null, BuiltInType.UInt16, ValueRanks.OneDimension, new UInt16[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "UInt32", "UInt32", null, BuiltInType.UInt32, ValueRanks.OneDimension, new UInt32[] { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "UInt64", "UInt64", null, BuiltInType.UInt64, ValueRanks.OneDimension, new UInt64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "UInteger", "UInteger", null, BuiltInType.UInteger, ValueRanks.OneDimension, new UInt64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.OneDimension, new DateTime[] { DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime() }, null);
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.OneDimension, new Variant[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    var doc1 = new XmlDocument();
                    CreateAnalogItemState(analogArrayFolder, daAnalogArray + "XmlElement", "XmlElement", null, BuiltInType.XmlElement, ValueRanks.OneDimension, new XmlElement[] { doc1.CreateElement("tag1"), doc1.CreateElement("tag2"), doc1.CreateElement("tag3"), doc1.CreateElement("tag4"), doc1.CreateElement("tag5"), doc1.CreateElement("tag6"), doc1.CreateElement("tag7"), doc1.CreateElement("tag8"), doc1.CreateElement("tag9"), doc1.CreateElement("tag10") });
                    #endregion

                    #region DataAccess_DiscreteType
                    ResetRandomGenerator(12);
                    var discreteTypeFolder = CreateFolderState(daFolder, "DataAccess_DiscreteType", "DiscreteType", null);
                    var twoStateDiscreteFolder = CreateFolderState(discreteTypeFolder, "DataAccess_TwoStateDiscreteType", "TwoStateDiscreteType", null);
                    const string daTwoStateDiscrete = "DataAccess_TwoStateDiscreteType_";

                    // Add our Nodes to the folder, and specify their customized discrete enumerations
                    CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "001", "001", null, AccessLevels.CurrentReadOrWrite, false, "red", "blue");
                    CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "002", "002", null, AccessLevels.CurrentReadOrWrite, false, "open", "close");
                    CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "003", "003", null, AccessLevels.CurrentReadOrWrite, false, "up", "down");
                    CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "004", "004", null, AccessLevels.CurrentReadOrWrite, false, "left", "right");
                    CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "005", "005", null, AccessLevels.CurrentReadOrWrite, false, "circle", "cross");

                    var multiStateDiscreteFolder = CreateFolderState(discreteTypeFolder, "DataAccess_MultiStateDiscreteType", "MultiStateDiscreteType", null);
                    const string daMultiStateDiscrete = "DataAccess_MultiStateDiscreteType_";

                    // Add our Nodes to the folder, and specify their customized discrete enumerations
                    var variable = CreateMultiStateDiscreteState(multiStateDiscreteFolder, daMultiStateDiscrete + "001", "001", null, AccessLevels.CurrentReadOrWrite, null,
                        AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, "open", "closed", "jammed");
                    variable.OnWriteValue = OnWriteDiscrete;
                    variable = CreateMultiStateDiscreteState(multiStateDiscreteFolder, daMultiStateDiscrete + "002", "002", null, AccessLevels.CurrentReadOrWrite, null,
                        AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, "red", "green", "blue", "cyan");
                    variable.OnWriteValue = OnWriteDiscrete;
                    variable = CreateMultiStateDiscreteState(multiStateDiscreteFolder, daMultiStateDiscrete + "003", "003", null, AccessLevels.CurrentReadOrWrite, null,
                        AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, "lolo", "lo", "normal", "hi", "hihi");
                    variable.OnWriteValue = OnWriteDiscrete;
                    variable = CreateMultiStateDiscreteState(multiStateDiscreteFolder, daMultiStateDiscrete + "004", "004", null, AccessLevels.CurrentReadOrWrite, null,
                        AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, "left", "right", "center");
                    variable.OnWriteValue = OnWriteDiscrete;
                    variable = CreateMultiStateDiscreteState(multiStateDiscreteFolder, daMultiStateDiscrete + "005", "005", null, AccessLevels.CurrentReadOrWrite, null,
                        AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, "circle", "cross", "triangle");
                    variable.OnWriteValue = OnWriteDiscrete;
                    #endregion

                    #region DataAccess_MultiStateValueDiscreteType
                    ResetRandomGenerator(13);
                    var multiStateValueDiscreteFolder = CreateFolderState(discreteTypeFolder, "DataAccess_MultiStateValueDiscreteType", "MultiStateValueDiscreteType", null);
                    const string daMultiStateValueDiscrete = "DataAccess_MultiStateValueDiscreteType_";

                    // Add our Nodes to the folder, and specify their customized discrete enumerations
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "001", "001", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "002", "002", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "red", "green", "blue", "cyan" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "003", "003", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "lolo", "lo", "normal", "hi", "hihi" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "004", "004", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "left", "right", "center" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "005", "005", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "circle", "cross", "triangle" });

                    // Add our Nodes to the folder and specify varying data types
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Byte", "Byte", null, DataTypeIds.Byte, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Int16", "Int16", null, DataTypeIds.Int16, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "red", "green", "blue", "cyan" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Int32", "Int32", null, DataTypeIds.Int32, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "lolo", "lo", "normal", "hi", "hihi" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Int64", "Int64", null, DataTypeIds.Int64, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "left", "right", "center" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "SByte", "SByte", null, DataTypeIds.SByte, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "UInt16", "UInt16", null, DataTypeIds.UInt16, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "red", "green", "blue", "cyan" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "UInt32", "UInt32", null, DataTypeIds.UInt32, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "lolo", "lo", "normal", "hi", "hihi" });
                    CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "UInt64", "UInt64", null, DataTypeIds.UInt64, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "left", "right", "center" });

                    #endregion

                    #region References
                    ResetRandomGenerator(14);
                    var referencesFolder = CreateFolderState(root, "References", "References", null);
                    const string referencesPrefix = "References_";

                    var referencesInstructions = CreateBaseDataVariableState(referencesFolder, "References_Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    referencesInstructions.Value = "This folder will contain nodes that have specific Reference configurations.";
                    variables.Add(referencesInstructions);

                    // create variable nodes with specific references
                    var hasForwardReference = CreateMeshVariable(referencesFolder, referencesPrefix + "HasForwardReference", "HasForwardReference");
                    hasForwardReference.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
                    variables.Add(hasForwardReference);

                    var hasInverseReference = CreateMeshVariable(referencesFolder, referencesPrefix + "HasInverseReference", "HasInverseReference");
                    hasInverseReference.AddReference(ReferenceTypes.HasCause, true, variables[0].NodeId);
                    variables.Add(hasInverseReference);

                    BaseDataVariableState has3InverseReference = null;
                    for (var i = 1; i <= 5; i++)
                    {
                        var referenceString = "Has3ForwardReferences";
                        if (i > 1)
                        {
                            referenceString += i.ToString();
                        }
                        var has3ForwardReferences = CreateMeshVariable(referencesFolder, referencesPrefix + referenceString, referenceString);
                        has3ForwardReferences.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
                        has3ForwardReferences.AddReference(ReferenceTypes.HasCause, false, variables[1].NodeId);
                        has3ForwardReferences.AddReference(ReferenceTypes.HasCause, false, variables[2].NodeId);
                        if (i == 1)
                        {
                            has3InverseReference = has3ForwardReferences;
                        }
                        variables.Add(has3ForwardReferences);
                    }

                    var has3InverseReferences = CreateMeshVariable(referencesFolder, referencesPrefix + "Has3InverseReferences", "Has3InverseReferences");
                    has3InverseReferences.AddReference(ReferenceTypes.HasEffect, true, variables[0].NodeId);
                    has3InverseReferences.AddReference(ReferenceTypes.HasEffect, true, variables[1].NodeId);
                    has3InverseReferences.AddReference(ReferenceTypes.HasEffect, true, variables[2].NodeId);
                    variables.Add(has3InverseReferences);

                    var hasForwardAndInverseReferences = CreateMeshVariable(referencesFolder, referencesPrefix + "HasForwardAndInverseReference", "HasForwardAndInverseReference", hasForwardReference, hasInverseReference, has3InverseReference, has3InverseReferences, variables[0]);
                    variables.Add(hasForwardAndInverseReferences);
                    #endregion

                    #region AccessRights
                    ResetRandomGenerator(15);
                    var folderAccessRights = CreateFolderState(root, "AccessRights", "AccessRights", null);
                    const string accessRights = "AccessRights_";

                    var accessRightsInstructions = CreateBaseDataVariableState(folderAccessRights, accessRights + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessRightsInstructions.Value = "This folder will be accessible to all who enter, but contents therein will be secured.";
                    variables.Add(accessRightsInstructions);

                    // sub-folder for "AccessAll"
                    var folderAccessRightsAccessAll = CreateFolderState(folderAccessRights, "AccessRights_AccessAll", "AccessAll", null);
                    const string accessRightsAccessAll = "AccessRights_AccessAll_";

                    var arAllRo = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO", "RO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRo.AccessLevel = AccessLevels.CurrentRead;
                    arAllRo.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arAllRo);
                    var arAllWo = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "WO", "WO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllWo.AccessLevel = AccessLevels.CurrentWrite;
                    arAllWo.UserAccessLevel = AccessLevels.CurrentWrite;
                    variables.Add(arAllWo);
                    var arAllRw = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RW", "RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRw.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arAllRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arAllRw);
                    var arAllRoNotUser = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO_NotUser", "RO_NotUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRoNotUser.AccessLevel = AccessLevels.CurrentRead;
                    arAllRoNotUser.UserAccessLevel = AccessLevels.None;
                    variables.Add(arAllRoNotUser);
                    var arAllWoNotUser = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "WO_NotUser", "WO_NotUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllWoNotUser.AccessLevel = AccessLevels.CurrentWrite;
                    arAllWoNotUser.UserAccessLevel = AccessLevels.None;
                    variables.Add(arAllWoNotUser);
                    var arAllRwNotUser = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RW_NotUser", "RW_NotUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRwNotUser.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arAllRwNotUser.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arAllRwNotUser);
                    var arAllRoUserRw = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO_User1_RW", "RO_User1_RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRoUserRw.AccessLevel = AccessLevels.CurrentRead;
                    arAllRoUserRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arAllRoUserRw);
                    var arAllRoGroupRw = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO_Group1_RW", "RO_Group1_RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRoGroupRw.AccessLevel = AccessLevels.CurrentRead;
                    arAllRoGroupRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arAllRoGroupRw);

                    // sub-folder for "AccessUser1"
                    var folderAccessRightsAccessUser1 = CreateFolderState(folderAccessRights, "AccessRights_AccessUser1", "AccessUser1", null);
                    const string accessRightsAccessUser1 = "AccessRights_AccessUser1_";

                    var arUserRo = CreateBaseDataVariableState(folderAccessRightsAccessUser1, accessRightsAccessUser1 + "RO", "RO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arUserRo.AccessLevel = AccessLevels.CurrentRead;
                    arUserRo.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arUserRo);
                    var arUserWo = CreateBaseDataVariableState(folderAccessRightsAccessUser1, accessRightsAccessUser1 + "WO", "WO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arUserWo.AccessLevel = AccessLevels.CurrentWrite;
                    arUserWo.UserAccessLevel = AccessLevels.CurrentWrite;
                    variables.Add(arUserWo);
                    var arUserRw = CreateBaseDataVariableState(folderAccessRightsAccessUser1, accessRightsAccessUser1 + "RW", "RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arUserRw.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arUserRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arUserRw);

                    // sub-folder for "AccessGroup1"
                    var folderAccessRightsAccessGroup1 = CreateFolderState(folderAccessRights, "AccessRights_AccessGroup1", "AccessGroup1", null);
                    const string accessRightsAccessGroup1 = "AccessRights_AccessGroup1_";

                    var arGroupRo = CreateBaseDataVariableState(folderAccessRightsAccessGroup1, accessRightsAccessGroup1 + "RO", "RO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arGroupRo.AccessLevel = AccessLevels.CurrentRead;
                    arGroupRo.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arGroupRo);
                    var arGroupWo = CreateBaseDataVariableState(folderAccessRightsAccessGroup1, accessRightsAccessGroup1 + "WO", "WO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arGroupWo.AccessLevel = AccessLevels.CurrentWrite;
                    arGroupWo.UserAccessLevel = AccessLevels.CurrentWrite;
                    variables.Add(arGroupWo);
                    var arGroupRw = CreateBaseDataVariableState(folderAccessRightsAccessGroup1, accessRightsAccessGroup1 + "RW", "RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arGroupRw.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arGroupRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arGroupRw);

                    // sub folder for "RolePermissions"
                    var folderRolePermissions = CreateFolderState(folderAccessRights, "AccessRights_RolePermissions", "RolePermissions", null);
                    const string rolePermissions = "AccessRights_RolePermissions_";

                    var rpAnonymous = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AnonymousAccess", "AnonymousAccess", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    rpAnonymous.Description = "This node can be accessed by users that have Anonymous Role";
                    rpAnonymous.RolePermissions = new RolePermissionTypeCollection()
                    {
                        // allow access to users with Anonymous role
                        new RolePermissionType()
                        {
                            RoleId = ObjectIds.WellKnownRole_Anonymous,
                            Permissions = (uint)(PermissionType.Browse |PermissionType.Read|PermissionType.ReadRolePermissions | PermissionType.Write)
                        },
                    };
                    variables.Add(rpAnonymous);

                    var rpAuthenticatedUser = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AuthenticatedUser", "AuthenticatedUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    rpAuthenticatedUser.Description = "This node can be accessed by users that have AuthenticatedUser Role";
                    rpAuthenticatedUser.RolePermissions = new RolePermissionTypeCollection()
                    {
                        // allow access to users with AuthenticatedUser role
                        new RolePermissionType()
                        {
                            RoleId = ObjectIds.WellKnownRole_AuthenticatedUser,
                            Permissions = (uint)(PermissionType.Browse |PermissionType.Read|PermissionType.ReadRolePermissions | PermissionType.Write)
                        },
                    };
                    variables.Add(rpAuthenticatedUser);

                    var rpSecurityAdminUser = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AdminUser", "AdminUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    rpSecurityAdminUser.Description = "This node can be accessed by users that have SecurityAdmin Role over an encrypted connection";
                    rpSecurityAdminUser.AccessRestrictions = AccessRestrictionType.EncryptionRequired;
                    rpSecurityAdminUser.RolePermissions = new RolePermissionTypeCollection()
                    {
                        // allow access to users with SecurityAdmin role
                        new RolePermissionType()
                        {
                            RoleId = ObjectIds.WellKnownRole_SecurityAdmin,
                            Permissions = (uint)(PermissionType.Browse |PermissionType.Read|PermissionType.ReadRolePermissions | PermissionType.Write)
                        },
                    };
                    variables.Add(rpSecurityAdminUser);

                    var rpConfigAdminUser = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AdminUser", "AdminUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    rpConfigAdminUser.Description = "This node can be accessed by users that have ConfigureAdmin Role over an encrypted connection";
                    rpConfigAdminUser.AccessRestrictions = AccessRestrictionType.EncryptionRequired;
                    rpConfigAdminUser.RolePermissions = new RolePermissionTypeCollection()
                    {
                        // allow access to users with ConfigureAdmin role
                        new RolePermissionType()
                        {
                            RoleId = ObjectIds.WellKnownRole_ConfigureAdmin,
                            Permissions = (uint)(PermissionType.Browse |PermissionType.Read|PermissionType.ReadRolePermissions | PermissionType.Write)
                        },
                    };
                    variables.Add(rpConfigAdminUser);

                    // sub-folder for "AccessRestrictions"
                    var folderAccessRestrictions = CreateFolderState(folderAccessRights, "AccessRights_AccessRestrictions", "AccessRestrictions", null);
                    const string accessRestrictions = "AccessRights_AccessRestrictions_";

                    var arNone = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "None", "None", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arNone.AccessLevel = AccessLevels.CurrentRead;
                    arNone.UserAccessLevel = AccessLevels.CurrentRead;
                    arNone.AccessRestrictions = AccessRestrictionType.None;
                    variables.Add(arNone);

                    var arSigningRequired = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "SigningRequired", "SigningRequired", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arSigningRequired.AccessLevel = AccessLevels.CurrentRead;
                    arSigningRequired.UserAccessLevel = AccessLevels.CurrentRead;
                    arSigningRequired.AccessRestrictions = AccessRestrictionType.SigningRequired;
                    variables.Add(arSigningRequired);

                    var arEncryptionRequired = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "EncryptionRequired", "EncryptionRequired", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arEncryptionRequired.AccessLevel = AccessLevels.CurrentRead;
                    arEncryptionRequired.UserAccessLevel = AccessLevels.CurrentRead;
                    arEncryptionRequired.AccessRestrictions = AccessRestrictionType.EncryptionRequired;
                    variables.Add(arEncryptionRequired);

                    var arSessionRequired = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "SessionRequired", "SessionRequired", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arSessionRequired.AccessLevel = AccessLevels.CurrentRead;
                    arSessionRequired.UserAccessLevel = AccessLevels.CurrentRead;
                    arSessionRequired.AccessRestrictions = AccessRestrictionType.SessionRequired;
                    variables.Add(arSessionRequired);
                    #endregion

                    #region NodeIds
                    ResetRandomGenerator(16);
                    var nodeIdsFolder = CreateFolderState(root, "NodeIds", "NodeIds", null);
                    const string nodeIds = "NodeIds_";

                    var nodeIdsInstructions = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeIdsInstructions.Value = "All supported Node types are available except whichever is in use for the other nodes.";
                    variables.Add(nodeIdsInstructions);

                    var integerNodeId = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16Integer", "Int16Integer", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    integerNodeId.NodeId = new NodeId(9202, NamespaceIndex);
                    variables.Add(integerNodeId);

                    variables.Add(CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16String", "Int16String", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));

                    var guidNodeId = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16GUID", "Int16GUID", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    guidNodeId.NodeId = new NodeId(new Guid("00000000-0000-0000-0000-000000009204"), NamespaceIndex);
                    variables.Add(guidNodeId);

                    var opaqueNodeId = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16Opaque", "Int16Opaque", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    opaqueNodeId.NodeId = new NodeId(new byte[] { 9, 2, 0, 5 }, NamespaceIndex);
                    variables.Add(opaqueNodeId);
                    #endregion

                    #region Methods
                    var methodsFolder = CreateFolderState(root, "Methods", "Methods", null);
                    const string methods = "Methods_";

                    var methodsInstructions = CreateBaseDataVariableState(methodsFolder, methods + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    methodsInstructions.Value = "Contains methods with varying parameter definitions.";
                    variables.Add(methodsInstructions);

                    CreateMethodState(methodsFolder, methods + "Void", "Void", OnVoidCall);

                    #region Add Method
                    var addMethod = CreateMethodState(methodsFolder, methods + "Add", "Add", OnAddCall);
                    // set input arguments
                    addMethod.InputArguments = new PropertyState<Argument[]>(addMethod)
                    {
                        NodeId = new NodeId(addMethod.BrowseName.Name + "InArgs", NamespaceIndex),
                        BrowseName = BrowseNames.InputArguments
                    };
                    addMethod.InputArguments.DisplayName = addMethod.InputArguments.BrowseName.Name;
                    addMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    addMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    addMethod.InputArguments.DataType = DataTypeIds.Argument;
                    addMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    addMethod.InputArguments.Value = new[]
                    {
                        new Argument() { Name = "Float value", Description = "Float value",  DataType = DataTypeIds.Float, ValueRank = ValueRanks.Scalar },
                        new Argument() { Name = "UInt32 value", Description = "UInt32 value",  DataType = DataTypeIds.UInt32, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    addMethod.OutputArguments = new PropertyState<Argument[]>(addMethod)
                    {
                        NodeId = new NodeId(addMethod.BrowseName.Name + "OutArgs", NamespaceIndex),
                        BrowseName = BrowseNames.OutputArguments
                    };
                    addMethod.OutputArguments.DisplayName = addMethod.OutputArguments.BrowseName.Name;
                    addMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    addMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    addMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    addMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    addMethod.OutputArguments.Value = new[]
                    {
                        new Argument() { Name = "Add Result", Description = "Add Result",  DataType = DataTypeIds.Float, ValueRank = ValueRanks.Scalar }
                    };
                    #endregion

                    #region Multiply Method
                    var multiplyMethod = CreateMethodState(methodsFolder, methods + "Multiply", "Multiply", OnMultiplyCall);
                    // set input arguments
                    multiplyMethod.InputArguments = new PropertyState<Argument[]>(multiplyMethod);
                    multiplyMethod.InputArguments.NodeId = new NodeId(multiplyMethod.BrowseName.Name + "InArgs", NamespaceIndex);
                    multiplyMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
                    multiplyMethod.InputArguments.DisplayName = multiplyMethod.InputArguments.BrowseName.Name;
                    multiplyMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    multiplyMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    multiplyMethod.InputArguments.DataType = DataTypeIds.Argument;
                    multiplyMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    multiplyMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Int16 value", Description = "Int16 value",  DataType = DataTypeIds.Int16, ValueRank = ValueRanks.Scalar },
                        new Argument() { Name = "UInt16 value", Description = "UInt16 value",  DataType = DataTypeIds.UInt16, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    multiplyMethod.OutputArguments = new PropertyState<Argument[]>(multiplyMethod);
                    multiplyMethod.OutputArguments.NodeId = new NodeId(multiplyMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    multiplyMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    multiplyMethod.OutputArguments.DisplayName = multiplyMethod.OutputArguments.BrowseName.Name;
                    multiplyMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    multiplyMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    multiplyMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    multiplyMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    multiplyMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Multiply Result", Description = "Multiply Result",  DataType = DataTypeIds.Int32, ValueRank = ValueRanks.Scalar }
                    };

                    #endregion

                    #region Divide Method
                    var divideMethod = CreateMethodState(methodsFolder, methods + "Divide", "Divide", new GenericMethodCalledEventHandler2(OnDivideCall));
                    // set input arguments
                    divideMethod.InputArguments = new PropertyState<Argument[]>(divideMethod);
                    divideMethod.InputArguments.NodeId = new NodeId(divideMethod.BrowseName.Name + "InArgs", NamespaceIndex);
                    divideMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
                    divideMethod.InputArguments.DisplayName = divideMethod.InputArguments.BrowseName.Name;
                    divideMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    divideMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    divideMethod.InputArguments.DataType = DataTypeIds.Argument;
                    divideMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    divideMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Int32 value", Description = "Int32 value",  DataType = DataTypeIds.Int32, ValueRank = ValueRanks.Scalar },
                        new Argument() { Name = "UInt16 value", Description = "UInt16 value",  DataType = DataTypeIds.UInt16, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    divideMethod.OutputArguments = new PropertyState<Argument[]>(divideMethod);
                    divideMethod.OutputArguments.NodeId = new NodeId(divideMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    divideMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    divideMethod.OutputArguments.DisplayName = divideMethod.OutputArguments.BrowseName.Name;
                    divideMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    divideMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    divideMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    divideMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    divideMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Divide Result", Description = "Divide Result",  DataType = DataTypeIds.Float, ValueRank = ValueRanks.Scalar }
                    };
                    #endregion

                    #region Substract Method
                    var substractMethod = CreateMethodState(methodsFolder, methods + "Substract", "Substract", new GenericMethodCalledEventHandler2(OnSubstractCall));
                    // set input arguments
                    substractMethod.InputArguments = new PropertyState<Argument[]>(substractMethod);
                    substractMethod.InputArguments.NodeId = new NodeId(substractMethod.BrowseName.Name + "InArgs", NamespaceIndex);
                    substractMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
                    substractMethod.InputArguments.DisplayName = substractMethod.InputArguments.BrowseName.Name;
                    substractMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    substractMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    substractMethod.InputArguments.DataType = DataTypeIds.Argument;
                    substractMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    substractMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Int16 value", Description = "Int16 value",  DataType = DataTypeIds.Int16, ValueRank = ValueRanks.Scalar },
                        new Argument() { Name = "Byte value", Description = "Byte value",  DataType = DataTypeIds.Byte, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    substractMethod.OutputArguments = new PropertyState<Argument[]>(substractMethod);
                    substractMethod.OutputArguments.NodeId = new NodeId(substractMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    substractMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    substractMethod.OutputArguments.DisplayName = substractMethod.OutputArguments.BrowseName.Name;
                    substractMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    substractMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    substractMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    substractMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    substractMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Substract Result", Description = "Substract Result",  DataType = DataTypeIds.Int16, ValueRank = ValueRanks.Scalar }
                    };
                    #endregion

                    #region Hello Method
                    var helloMethod = CreateMethodState(methodsFolder, methods + "Hello", "Hello", new GenericMethodCalledEventHandler2(OnHelloCall));
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

                    #region Input Method
                    var inputMethod = CreateMethodState(methodsFolder, methods + "Input", "Input", new GenericMethodCalledEventHandler2(OnInputCall));
                    // set input arguments
                    inputMethod.InputArguments = new PropertyState<Argument[]>(inputMethod);
                    inputMethod.InputArguments.NodeId = new NodeId(inputMethod.BrowseName.Name + "InArgs", NamespaceIndex);
                    inputMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
                    inputMethod.InputArguments.DisplayName = inputMethod.InputArguments.BrowseName.Name;
                    inputMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    inputMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    inputMethod.InputArguments.DataType = DataTypeIds.Argument;
                    inputMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    inputMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "String value", Description = "String value",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
                    };
                    #endregion

                    #region Output Method
                    var outputMethod = CreateMethodState(methodsFolder, methods + "Output", "Output", new GenericMethodCalledEventHandler2(OnOutputCall));

                    // set output arguments
                    outputMethod.OutputArguments = new PropertyState<Argument[]>(helloMethod);
                    outputMethod.OutputArguments.NodeId = new NodeId(helloMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    outputMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    outputMethod.OutputArguments.DisplayName = helloMethod.OutputArguments.BrowseName.Name;
                    outputMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    outputMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    outputMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    outputMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    outputMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Output Result", Description = "Output Result",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
                    };
                    #endregion
                    #endregion

                    #region Views
                    ResetRandomGenerator(18);
                    var viewsFolder = CreateFolderState(root, "Views", "Views", null);
                    const string views = "Views_";

                    var viewStateOperations = CreateViewState(viewsFolder, externalReferences, views + "Operations", "Operations", null);
                    var viewStateEngineering = CreateViewState(viewsFolder, externalReferences, views + "Engineering", "Engineering", null);
                    #endregion

                    #region Locales
                    ResetRandomGenerator(19);
                    var localesFolder = CreateFolderState(root, "Locales", "Locales", null);
                    const string locales = "Locales_";

                    var qnEnglishVariable = CreateBaseDataVariableState(localesFolder, locales + "QNEnglish", "QNEnglish", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnEnglishVariable.Description = new LocalizedText("en", "English");
                    qnEnglishVariable.Value = new QualifiedName("Hello World", NamespaceIndex);
                    variables.Add(qnEnglishVariable);
                    var ltEnglishVariable = CreateBaseDataVariableState(localesFolder, locales + "LTEnglish", "LTEnglish", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltEnglishVariable.Description = new LocalizedText("en", "English");
                    ltEnglishVariable.Value = new LocalizedText("en", "Hello World");
                    variables.Add(ltEnglishVariable);

                    var qnFrancaisVariable = CreateBaseDataVariableState(localesFolder, locales + "QNFrancais", "QNFrancais", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnFrancaisVariable.Description = new LocalizedText("en", "Francais");
                    qnFrancaisVariable.Value = new QualifiedName("Salut tout le monde", NamespaceIndex);
                    variables.Add(qnFrancaisVariable);
                    var ltFrancaisVariable = CreateBaseDataVariableState(localesFolder, locales + "LTFrancais", "LTFrancais", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltFrancaisVariable.Description = new LocalizedText("en", "Francais");
                    ltFrancaisVariable.Value = new LocalizedText("fr", "Salut tout le monde");
                    variables.Add(ltFrancaisVariable);

                    var qnDeutschVariable = CreateBaseDataVariableState(localesFolder, locales + "QNDeutsch", "QNDeutsch", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnDeutschVariable.Description = new LocalizedText("en", "Deutsch");
                    qnDeutschVariable.Value = new QualifiedName("Hallo Welt", NamespaceIndex);
                    variables.Add(qnDeutschVariable);
                    var ltDeutschVariable = CreateBaseDataVariableState(localesFolder, locales + "LTDeutsch", "LTDeutsch", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltDeutschVariable.Description = new LocalizedText("en", "Deutsch");
                    ltDeutschVariable.Value = new LocalizedText("de", "Hallo Welt");
                    variables.Add(ltDeutschVariable);

                    var qnEspanolVariable = CreateBaseDataVariableState(localesFolder, locales + "QNEspanol", "QNEspanol", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnEspanolVariable.Description = new LocalizedText("en", "Espanol");
                    qnEspanolVariable.Value = new QualifiedName("Hola mundo", NamespaceIndex);
                    variables.Add(qnEspanolVariable);
                    var ltEspanolVariable = CreateBaseDataVariableState(localesFolder, locales + "LTEspanol", "LTEspanol", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltEspanolVariable.Description = new LocalizedText("en", "Espanol");
                    ltEspanolVariable.Value = new LocalizedText("es", "Hola mundo");
                    variables.Add(ltEspanolVariable);

                    var qnJapaneseVariable = CreateBaseDataVariableState(localesFolder, locales + "QN日本の", "QN日本の", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnJapaneseVariable.Description = new LocalizedText("en", "Japanese");
                    qnJapaneseVariable.Value = new QualifiedName("ハローワールド", NamespaceIndex);
                    variables.Add(qnJapaneseVariable);
                    var ltJapaneseVariable = CreateBaseDataVariableState(localesFolder, locales + "LT日本の", "LT日本の", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltJapaneseVariable.Description = new LocalizedText("en", "Japanese");
                    ltJapaneseVariable.Value = new LocalizedText("jp", "ハローワールド");
                    variables.Add(ltJapaneseVariable);

                    var qnChineseVariable = CreateBaseDataVariableState(localesFolder, locales + "QN中國的", "QN中國的", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnChineseVariable.Description = new LocalizedText("en", "Chinese");
                    qnChineseVariable.Value = new QualifiedName("世界您好", NamespaceIndex);
                    variables.Add(qnChineseVariable);
                    var ltChineseVariable = CreateBaseDataVariableState(localesFolder, locales + "LT中國的", "LT中國的", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltChineseVariable.Description = new LocalizedText("en", "Chinese");
                    ltChineseVariable.Value = new LocalizedText("ch", "世界您好");
                    variables.Add(ltChineseVariable);

                    var qnRussianVariable = CreateBaseDataVariableState(localesFolder, locales + "QNрусский", "QNрусский", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnRussianVariable.Description = new LocalizedText("en", "Russian");
                    qnRussianVariable.Value = new QualifiedName("LTрусский", NamespaceIndex);
                    variables.Add(qnRussianVariable);
                    var ltRussianVariable = CreateBaseDataVariableState(localesFolder, locales + "LTрусский", "LTрусский", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltRussianVariable.Description = new LocalizedText("en", "Russian");
                    ltRussianVariable.Value = new LocalizedText("ru", "LTрусский");
                    variables.Add(ltRussianVariable);

                    var qnArabicVariable = CreateBaseDataVariableState(localesFolder, locales + "QNالعربية", "QNالعربية", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnArabicVariable.Description = new LocalizedText("en", "Arabic");
                    qnArabicVariable.Value = new QualifiedName("مرحبا بالعال", NamespaceIndex);
                    variables.Add(qnArabicVariable);
                    var ltArabicVariable = CreateBaseDataVariableState(localesFolder, locales + "LTالعربية", "LTالعربية", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltArabicVariable.Description = new LocalizedText("en", "Arabic");
                    ltArabicVariable.Value = new LocalizedText("ae", "مرحبا بالعال");
                    variables.Add(ltArabicVariable);

                    var qnKlingonVariable = CreateBaseDataVariableState(localesFolder, locales + "QNtlhIngan", "QNtlhIngan", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnKlingonVariable.Description = new LocalizedText("en", "Klingon");
                    qnKlingonVariable.Value = new QualifiedName("qo' vIvan", NamespaceIndex);
                    variables.Add(qnKlingonVariable);
                    var ltKlingonVariable = CreateBaseDataVariableState(localesFolder, locales + "LTtlhIngan", "LTtlhIngan", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltKlingonVariable.Description = new LocalizedText("en", "Klingon");
                    ltKlingonVariable.Value = new LocalizedText("ko", "qo' vIvan");
                    variables.Add(ltKlingonVariable);
                    #endregion

                    #region Attributes
                    ResetRandomGenerator(20);
                    var folderAttributes = CreateFolderState(root, "Attributes", "Attributes", null);

                    #region AccessAll
                    var folderAttributesAccessAll = CreateFolderState(folderAttributes, "Attributes_AccessAll", "AccessAll", null);
                    const string attributesAccessAll = "Attributes_AccessAll_";

                    var accessLevelAccessAll = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "AccessLevel", "AccessLevel", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessLevelAccessAll.WriteMask = AttributeWriteMask.AccessLevel;
                    accessLevelAccessAll.UserWriteMask = AttributeWriteMask.AccessLevel;
                    variables.Add(accessLevelAccessAll);

                    var arrayDimensionsAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ArrayDimensions", "ArrayDimensions", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arrayDimensionsAccessLevel.WriteMask = AttributeWriteMask.ArrayDimensions;
                    arrayDimensionsAccessLevel.UserWriteMask = AttributeWriteMask.ArrayDimensions;
                    variables.Add(arrayDimensionsAccessLevel);

                    var browseNameAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "BrowseName", "BrowseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    browseNameAccessLevel.WriteMask = AttributeWriteMask.BrowseName;
                    browseNameAccessLevel.UserWriteMask = AttributeWriteMask.BrowseName;
                    variables.Add(browseNameAccessLevel);

                    var containsNoLoopsAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ContainsNoLoops", "ContainsNoLoops", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    containsNoLoopsAccessLevel.WriteMask = AttributeWriteMask.ContainsNoLoops;
                    containsNoLoopsAccessLevel.UserWriteMask = AttributeWriteMask.ContainsNoLoops;
                    variables.Add(containsNoLoopsAccessLevel);

                    var dataTypeAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "DataType", "DataType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    dataTypeAccessLevel.WriteMask = AttributeWriteMask.DataType;
                    dataTypeAccessLevel.UserWriteMask = AttributeWriteMask.DataType;
                    variables.Add(dataTypeAccessLevel);

                    var descriptionAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Description", "Description", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    descriptionAccessLevel.WriteMask = AttributeWriteMask.Description;
                    descriptionAccessLevel.UserWriteMask = AttributeWriteMask.Description;
                    variables.Add(descriptionAccessLevel);

                    var eventNotifierAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "EventNotifier", "EventNotifier", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    eventNotifierAccessLevel.WriteMask = AttributeWriteMask.EventNotifier;
                    eventNotifierAccessLevel.UserWriteMask = AttributeWriteMask.EventNotifier;
                    variables.Add(eventNotifierAccessLevel);

                    var executableAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Executable", "Executable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    executableAccessLevel.WriteMask = AttributeWriteMask.Executable;
                    executableAccessLevel.UserWriteMask = AttributeWriteMask.Executable;
                    variables.Add(executableAccessLevel);

                    var historizingAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Historizing", "Historizing", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    historizingAccessLevel.WriteMask = AttributeWriteMask.Historizing;
                    historizingAccessLevel.UserWriteMask = AttributeWriteMask.Historizing;
                    variables.Add(historizingAccessLevel);

                    var inverseNameAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "InverseName", "InverseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    inverseNameAccessLevel.WriteMask = AttributeWriteMask.InverseName;
                    inverseNameAccessLevel.UserWriteMask = AttributeWriteMask.InverseName;
                    variables.Add(inverseNameAccessLevel);

                    var isAbstractAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "IsAbstract", "IsAbstract", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    isAbstractAccessLevel.WriteMask = AttributeWriteMask.IsAbstract;
                    isAbstractAccessLevel.UserWriteMask = AttributeWriteMask.IsAbstract;
                    variables.Add(isAbstractAccessLevel);

                    var minimumSamplingIntervalAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "MinimumSamplingInterval", "MinimumSamplingInterval", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    minimumSamplingIntervalAccessLevel.WriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    minimumSamplingIntervalAccessLevel.UserWriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    variables.Add(minimumSamplingIntervalAccessLevel);

                    var nodeClassIntervalAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "NodeClass", "NodeClass", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeClassIntervalAccessLevel.WriteMask = AttributeWriteMask.NodeClass;
                    nodeClassIntervalAccessLevel.UserWriteMask = AttributeWriteMask.NodeClass;
                    variables.Add(nodeClassIntervalAccessLevel);

                    var nodeIdAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "NodeId", "NodeId", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeIdAccessLevel.WriteMask = AttributeWriteMask.NodeId;
                    nodeIdAccessLevel.UserWriteMask = AttributeWriteMask.NodeId;
                    variables.Add(nodeIdAccessLevel);

                    var symmetricAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Symmetric", "Symmetric", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    symmetricAccessLevel.WriteMask = AttributeWriteMask.Symmetric;
                    symmetricAccessLevel.UserWriteMask = AttributeWriteMask.Symmetric;
                    variables.Add(symmetricAccessLevel);

                    var userAccessLevelAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "UserAccessLevel", "UserAccessLevel", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userAccessLevelAccessLevel.WriteMask = AttributeWriteMask.UserAccessLevel;
                    userAccessLevelAccessLevel.UserWriteMask = AttributeWriteMask.UserAccessLevel;
                    variables.Add(userAccessLevelAccessLevel);

                    var userExecutableAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "UserExecutable", "UserExecutable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userExecutableAccessLevel.WriteMask = AttributeWriteMask.UserExecutable;
                    userExecutableAccessLevel.UserWriteMask = AttributeWriteMask.UserExecutable;
                    variables.Add(userExecutableAccessLevel);

                    var valueRankAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ValueRank", "ValueRank", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueRankAccessLevel.WriteMask = AttributeWriteMask.ValueRank;
                    valueRankAccessLevel.UserWriteMask = AttributeWriteMask.ValueRank;
                    variables.Add(valueRankAccessLevel);

                    var writeMaskAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "WriteMask", "WriteMask", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    writeMaskAccessLevel.WriteMask = AttributeWriteMask.WriteMask;
                    writeMaskAccessLevel.UserWriteMask = AttributeWriteMask.WriteMask;
                    variables.Add(writeMaskAccessLevel);

                    var valueForVariableTypeAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ValueForVariableType", "ValueForVariableType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueForVariableTypeAccessLevel.WriteMask = AttributeWriteMask.ValueForVariableType;
                    valueForVariableTypeAccessLevel.UserWriteMask = AttributeWriteMask.ValueForVariableType;
                    variables.Add(valueForVariableTypeAccessLevel);

                    var allAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "All", "All", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    allAccessLevel.WriteMask = AttributeWriteMask.AccessLevel | AttributeWriteMask.ArrayDimensions | AttributeWriteMask.BrowseName | AttributeWriteMask.ContainsNoLoops | AttributeWriteMask.DataType |
                            AttributeWriteMask.Description | AttributeWriteMask.DisplayName | AttributeWriteMask.EventNotifier | AttributeWriteMask.Executable | AttributeWriteMask.Historizing | AttributeWriteMask.InverseName | AttributeWriteMask.IsAbstract |
                            AttributeWriteMask.MinimumSamplingInterval | AttributeWriteMask.NodeClass | AttributeWriteMask.NodeId | AttributeWriteMask.Symmetric | AttributeWriteMask.UserAccessLevel | AttributeWriteMask.UserExecutable |
                            AttributeWriteMask.UserWriteMask | AttributeWriteMask.ValueForVariableType | AttributeWriteMask.ValueRank | AttributeWriteMask.WriteMask;
                    allAccessLevel.UserWriteMask = AttributeWriteMask.AccessLevel | AttributeWriteMask.ArrayDimensions | AttributeWriteMask.BrowseName | AttributeWriteMask.ContainsNoLoops | AttributeWriteMask.DataType |
                            AttributeWriteMask.Description | AttributeWriteMask.DisplayName | AttributeWriteMask.EventNotifier | AttributeWriteMask.Executable | AttributeWriteMask.Historizing | AttributeWriteMask.InverseName | AttributeWriteMask.IsAbstract |
                            AttributeWriteMask.MinimumSamplingInterval | AttributeWriteMask.NodeClass | AttributeWriteMask.NodeId | AttributeWriteMask.Symmetric | AttributeWriteMask.UserAccessLevel | AttributeWriteMask.UserExecutable |
                            AttributeWriteMask.UserWriteMask | AttributeWriteMask.ValueForVariableType | AttributeWriteMask.ValueRank | AttributeWriteMask.WriteMask;
                    variables.Add(allAccessLevel);
                    #endregion

                    #region AccessUser1
                    var folderAttributesAccessUser1 = CreateFolderState(folderAttributes, "Attributes_AccessUser1", "AccessUser1", null);
                    const string attributesAccessUser1 = "Attributes_AccessUser1_";

                    var accessLevelAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "AccessLevel", "AccessLevel", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessLevelAccessUser1.WriteMask = AttributeWriteMask.AccessLevel;
                    accessLevelAccessUser1.UserWriteMask = AttributeWriteMask.AccessLevel;
                    variables.Add(accessLevelAccessUser1);

                    var arrayDimensionsAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ArrayDimensions", "ArrayDimensions", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arrayDimensionsAccessUser1.WriteMask = AttributeWriteMask.ArrayDimensions;
                    arrayDimensionsAccessUser1.UserWriteMask = AttributeWriteMask.ArrayDimensions;
                    variables.Add(arrayDimensionsAccessUser1);

                    var browseNameAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "BrowseName", "BrowseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    browseNameAccessUser1.WriteMask = AttributeWriteMask.BrowseName;
                    browseNameAccessUser1.UserWriteMask = AttributeWriteMask.BrowseName;
                    variables.Add(browseNameAccessUser1);

                    var containsNoLoopsAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ContainsNoLoops", "ContainsNoLoops", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    containsNoLoopsAccessUser1.WriteMask = AttributeWriteMask.ContainsNoLoops;
                    containsNoLoopsAccessUser1.UserWriteMask = AttributeWriteMask.ContainsNoLoops;
                    variables.Add(containsNoLoopsAccessUser1);

                    var dataTypeAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "DataType", "DataType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    dataTypeAccessUser1.WriteMask = AttributeWriteMask.DataType;
                    dataTypeAccessUser1.UserWriteMask = AttributeWriteMask.DataType;
                    variables.Add(dataTypeAccessUser1);

                    var descriptionAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Description", "Description", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    descriptionAccessUser1.WriteMask = AttributeWriteMask.Description;
                    descriptionAccessUser1.UserWriteMask = AttributeWriteMask.Description;
                    variables.Add(descriptionAccessUser1);

                    var eventNotifierAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "EventNotifier", "EventNotifier", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    eventNotifierAccessUser1.WriteMask = AttributeWriteMask.EventNotifier;
                    eventNotifierAccessUser1.UserWriteMask = AttributeWriteMask.EventNotifier;
                    variables.Add(eventNotifierAccessUser1);

                    var executableAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Executable", "Executable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    executableAccessUser1.WriteMask = AttributeWriteMask.Executable;
                    executableAccessUser1.UserWriteMask = AttributeWriteMask.Executable;
                    variables.Add(executableAccessUser1);

                    var historizingAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Historizing", "Historizing", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    historizingAccessUser1.WriteMask = AttributeWriteMask.Historizing;
                    historizingAccessUser1.UserWriteMask = AttributeWriteMask.Historizing;
                    variables.Add(historizingAccessUser1);

                    var inverseNameAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "InverseName", "InverseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    inverseNameAccessUser1.WriteMask = AttributeWriteMask.InverseName;
                    inverseNameAccessUser1.UserWriteMask = AttributeWriteMask.InverseName;
                    variables.Add(inverseNameAccessUser1);

                    var isAbstractAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "IsAbstract", "IsAbstract", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    isAbstractAccessUser1.WriteMask = AttributeWriteMask.IsAbstract;
                    isAbstractAccessUser1.UserWriteMask = AttributeWriteMask.IsAbstract;
                    variables.Add(isAbstractAccessUser1);

                    var minimumSamplingIntervalAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "MinimumSamplingInterval", "MinimumSamplingInterval", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    minimumSamplingIntervalAccessUser1.WriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    minimumSamplingIntervalAccessUser1.UserWriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    variables.Add(minimumSamplingIntervalAccessUser1);

                    var nodeClassIntervalAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "NodeClass", "NodeClass", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeClassIntervalAccessUser1.WriteMask = AttributeWriteMask.NodeClass;
                    nodeClassIntervalAccessUser1.UserWriteMask = AttributeWriteMask.NodeClass;
                    variables.Add(nodeClassIntervalAccessUser1);

                    var nodeIdAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "NodeId", "NodeId", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeIdAccessUser1.WriteMask = AttributeWriteMask.NodeId;
                    nodeIdAccessUser1.UserWriteMask = AttributeWriteMask.NodeId;
                    variables.Add(nodeIdAccessUser1);

                    var symmetricAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Symmetric", "Symmetric", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    symmetricAccessUser1.WriteMask = AttributeWriteMask.Symmetric;
                    symmetricAccessUser1.UserWriteMask = AttributeWriteMask.Symmetric;
                    variables.Add(symmetricAccessUser1);

                    var userAccessUser1AccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "UserAccessUser1", "UserAccessUser1", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userAccessUser1AccessUser1.WriteMask = AttributeWriteMask.UserAccessLevel;
                    userAccessUser1AccessUser1.UserWriteMask = AttributeWriteMask.UserAccessLevel;
                    variables.Add(userAccessUser1AccessUser1);

                    var userExecutableAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "UserExecutable", "UserExecutable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userExecutableAccessUser1.WriteMask = AttributeWriteMask.UserExecutable;
                    userExecutableAccessUser1.UserWriteMask = AttributeWriteMask.UserExecutable;
                    variables.Add(userExecutableAccessUser1);

                    var valueRankAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ValueRank", "ValueRank", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueRankAccessUser1.WriteMask = AttributeWriteMask.ValueRank;
                    valueRankAccessUser1.UserWriteMask = AttributeWriteMask.ValueRank;
                    variables.Add(valueRankAccessUser1);

                    var writeMaskAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "WriteMask", "WriteMask", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    writeMaskAccessUser1.WriteMask = AttributeWriteMask.WriteMask;
                    writeMaskAccessUser1.UserWriteMask = AttributeWriteMask.WriteMask;
                    variables.Add(writeMaskAccessUser1);

                    var valueForVariableTypeAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ValueForVariableType", "ValueForVariableType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueForVariableTypeAccessUser1.WriteMask = AttributeWriteMask.ValueForVariableType;
                    valueForVariableTypeAccessUser1.UserWriteMask = AttributeWriteMask.ValueForVariableType;
                    variables.Add(valueForVariableTypeAccessUser1);

                    var allAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "All", "All", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    allAccessUser1.WriteMask = AttributeWriteMask.AccessLevel | AttributeWriteMask.ArrayDimensions | AttributeWriteMask.BrowseName | AttributeWriteMask.ContainsNoLoops | AttributeWriteMask.DataType |
                            AttributeWriteMask.Description | AttributeWriteMask.DisplayName | AttributeWriteMask.EventNotifier | AttributeWriteMask.Executable | AttributeWriteMask.Historizing | AttributeWriteMask.InverseName | AttributeWriteMask.IsAbstract |
                            AttributeWriteMask.MinimumSamplingInterval | AttributeWriteMask.NodeClass | AttributeWriteMask.NodeId | AttributeWriteMask.Symmetric | AttributeWriteMask.UserAccessLevel | AttributeWriteMask.UserExecutable |
                            AttributeWriteMask.UserWriteMask | AttributeWriteMask.ValueForVariableType | AttributeWriteMask.ValueRank | AttributeWriteMask.WriteMask;
                    allAccessUser1.UserWriteMask = AttributeWriteMask.AccessLevel | AttributeWriteMask.ArrayDimensions | AttributeWriteMask.BrowseName | AttributeWriteMask.ContainsNoLoops | AttributeWriteMask.DataType |
                            AttributeWriteMask.Description | AttributeWriteMask.DisplayName | AttributeWriteMask.EventNotifier | AttributeWriteMask.Executable | AttributeWriteMask.Historizing | AttributeWriteMask.InverseName | AttributeWriteMask.IsAbstract |
                            AttributeWriteMask.MinimumSamplingInterval | AttributeWriteMask.NodeClass | AttributeWriteMask.NodeId | AttributeWriteMask.Symmetric | AttributeWriteMask.UserAccessLevel | AttributeWriteMask.UserExecutable |
                            AttributeWriteMask.UserWriteMask | AttributeWriteMask.ValueForVariableType | AttributeWriteMask.ValueRank | AttributeWriteMask.WriteMask;
                    variables.Add(allAccessUser1);
                    #endregion
                    #endregion

                    #region MyCompany
                    ResetRandomGenerator(21);
                    var myCompanyFolder = CreateFolderState(root, "MyCompany", "MyCompany", null);
                    const string myCompany = "MyCompany_";

                    var myCompanyInstructions = CreateBaseDataVariableState(myCompanyFolder, myCompany + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    myCompanyInstructions.Value = "A place for the vendor to describe their address-space.";
                    variables.Add(myCompanyInstructions);
                    #endregion
                }
                catch (Exception e)
                {
                    Utils.LogError(e, "Error creating the ReferenceNodeManager address space.");
                }

                AddPredefinedNode(SystemContext, root);

                // reset random generator and generate boundary values
                ResetRandomGenerator(100, 1);
                simulationTimer_ = new Timer(DoSimulation, null, 1000, 1000);
            }
        }

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

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateMeshVariable(NodeState parent, string path, string name, params NodeState[] peers)
        {
            var variable = CreateBaseDataVariableState(parent, path, name, null, BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

            if (peers != null)
            {
                foreach (var peer in peers)
                {
                    peer.AddReference(ReferenceTypes.HasCause, false, variable.NodeId);
                    variable.AddReference(ReferenceTypes.HasCause, true, peer.NodeId);
                    peer.AddReference(ReferenceTypes.HasEffect, true, variable.NodeId);
                    variable.AddReference(ReferenceTypes.HasEffect, false, peer.NodeId);
                }
            }

            return variable;
        }

        #region DataAccess Server Facet related Methods
        private AnalogItemState CreateAnalogItemState(NodeState parent, string browseName, string name, string description, BuiltInType dataType, int valueRank, object initialValues = null, Opc.Ua.Range customRange = null)
        {
            return CreateAnalogItemState(parent, browseName, name, description, (uint)dataType, valueRank, initialValues, customRange);
        }

        private AnalogItemState CreateAnalogItemState(NodeState parent, string browseName, string name, string description, NodeId dataType, int valueRank, object initialValues = null, Opc.Ua.Range customRange = null)
        {
            var displayName = new LocalizedText("", name);

            var builtInType = Opc.Ua.TypeInfo.GetBuiltInType(dataType, ServerData.TypeTree);

            // Simulate a mV Voltmeter
            var newRange = GetAnalogRange(builtInType);
            // Using anything but 120,-10 fails a few tests
            newRange.High = Math.Min(newRange.High, 120);
            newRange.Low = Math.Max(newRange.Low, -10);

            var engineeringUnits = new EUInformation("mV", "millivolt", "http://www.opcfoundation.org/UA/units/un/cefact");
            // The mapping of the UNECE codes to OPC UA(EUInformation.unitId) is available here:
            // http://www.opcfoundation.org/UA/EngineeringUnits/UNECE/UNECE_to_OPCUA.csv
            engineeringUnits.UnitId = 12890; // "2Z"

            var variable = CreateAnalogItemState(parent, browseName, displayName, description, dataType, valueRank, AccessLevels.CurrentReadOrWrite, initialValues, customRange, engineeringUnits, newRange);

            variable.OnWriteValue = OnWriteAnalog;
            variable.EURange.OnWriteValue = OnWriteAnalogRange;
            variable.InstrumentRange.OnWriteValue = OnWriteAnalogRange;

            return variable;

        }
        #endregion

        private ServiceResult OnWriteDiscrete(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as MultiStateDiscreteState;

            // verify data type.
            var typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

            if (typeInfo == null || typeInfo == Opc.Ua.TypeInfo.Unknown)
            {
                return StatusCodes.BadTypeMismatch;
            }

            if (indexRange != NumericRange.Empty)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            var number = Convert.ToDouble(value);

            if (number >= variable.EnumStrings.Value.Length || number < 0)
            {
                return StatusCodes.BadOutOfRange;
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnWriteValueDiscrete(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as MultiStateValueDiscreteState;

            var typeInfo = Opc.Ua.TypeInfo.Construct(value);

            if (variable == null ||
                typeInfo == null ||
                typeInfo == Opc.Ua.TypeInfo.Unknown ||
                !Opc.Ua.TypeInfo.IsNumericType(typeInfo.BuiltInType))
            {
                return StatusCodes.BadTypeMismatch;
            }

            if (indexRange != NumericRange.Empty)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            var number = Convert.ToInt32(value);
            if (number >= variable.EnumValues.Value.Length || number < 0)
            {
                return StatusCodes.BadOutOfRange;
            }

            if (!node.SetChildValue(context, BrowseNames.ValueAsText, variable.EnumValues.Value[number].DisplayName, true))
            {
                return StatusCodes.BadOutOfRange;
            }

            node.ClearChangeMasks(context, true);

            return ServiceResult.Good;
        }

        private ServiceResult OnWriteAnalog(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as AnalogItemState;

            // verify data type.
            var typeInfo = Opc.Ua.TypeInfo.IsInstanceOfDataType(
                value,
                variable.DataType,
                variable.ValueRank,
                context.NamespaceUris,
                context.TypeTable);

            if (typeInfo == null || typeInfo == Opc.Ua.TypeInfo.Unknown)
            {
                return StatusCodes.BadTypeMismatch;
            }

            // check index range.
            if (variable.ValueRank >= 0)
            {
                if (indexRange != NumericRange.Empty)
                {
                    var target = variable.Value;
                    ServiceResult result = indexRange.UpdateRange(ref target, value);

                    if (ServiceResult.IsBad(result))
                    {
                        return result;
                    }

                    value = target;
                }
            }

            // check instrument range.
            else
            {
                if (indexRange != NumericRange.Empty)
                {
                    return StatusCodes.BadIndexRangeInvalid;
                }

                var number = Convert.ToDouble(value);

                if (variable.InstrumentRange != null && (number < variable.InstrumentRange.Value.Low || number > variable.InstrumentRange.Value.High))
                {
                    return StatusCodes.BadOutOfRange;
                }
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnWriteAnalogRange(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            var variable = node as PropertyState<Opc.Ua.Range>;
            var extensionObject = value as ExtensionObject;
            var typeInfo = Opc.Ua.TypeInfo.Construct(value);

            if (variable == null ||
                extensionObject == null ||
                typeInfo == null ||
                typeInfo == Opc.Ua.TypeInfo.Unknown)
            {
                return StatusCodes.BadTypeMismatch;
            }

            var newRange = extensionObject.Body as Opc.Ua.Range;
            var parent = variable.Parent as AnalogItemState;
            if (newRange == null ||
                parent == null)
            {
                return StatusCodes.BadTypeMismatch;
            }

            if (indexRange != NumericRange.Empty)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            var parentTypeInfo = Opc.Ua.TypeInfo.Construct(parent.Value);
            var parentRange = GetAnalogRange(parentTypeInfo.BuiltInType);
            if (parentRange.High < newRange.High ||
                parentRange.Low > newRange.Low)
            {
                return StatusCodes.BadOutOfRange;
            }

            value = newRange;

            return ServiceResult.Good;
        }

        private BaseDataVariableState[] CreateVariables(NodeState parent, string path, string name, string description, BuiltInType dataType, int valueRank, UInt16 numVariables)
        {
            return CreateVariables(parent, path, name, description, (uint)dataType, valueRank, numVariables);
        }

        private BaseDataVariableState[] CreateVariables(NodeState parent, string path, string name, string description, NodeId dataType, int valueRank, UInt16 numVariables)
        {
            // first, create a new Parent folder for this data-type
            var newParentFolder = CreateFolderState(parent, path, name, null);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                var newName = string.Format("{0}_{1}", name, i.ToString("00"));
                var newPath = string.Format("{0}_{1}", path, newName);
                itemsCreated.Add(CreateBaseDataVariableState(newParentFolder, newPath, newName, null, dataType, valueRank, AccessLevels.CurrentReadOrWrite, null));
            }
            return (itemsCreated.ToArray());
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, string description, BuiltInType dataType, int valueRank, byte accessLevel = AccessLevels.CurrentReadOrWrite)
        {
            return CreateDynamicVariable(parent, path, name, description, (uint)dataType, valueRank, accessLevel);
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, string description, NodeId dataType, int valueRank, byte accessLevel = AccessLevels.CurrentReadOrWrite)
        {
            var variable = CreateBaseDataVariableState(parent, path, name, description, dataType, valueRank, accessLevel, null);
            dynamicNodes_.Add(variable);
            return variable;
        }

        private BaseDataVariableState[] CreateDynamicVariables(NodeState parent, string path, string name, string description, BuiltInType dataType, int valueRank, uint numVariables)
        {
            return CreateDynamicVariables(parent, path, name, description, (uint)dataType, valueRank, numVariables);

        }

        private BaseDataVariableState[] CreateDynamicVariables(NodeState parent, string path, string name, string description, NodeId dataType, int valueRank, uint numVariables)
        {
            // first, create a new Parent folder for this data-type
            var newParentFolder = CreateFolderState(parent, path, name, null);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                var newName = string.Format("{0}_{1}", name, i.ToString("00"));
                var newPath = string.Format("{0}_{1}", path, newName);
                itemsCreated.Add(CreateDynamicVariable(newParentFolder, newPath, newName, description, dataType, valueRank));
            }//for i
            return (itemsCreated.ToArray());
        }

        private ServiceResult OnVoidCall(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            return ServiceResult.Good;
        }

        private ServiceResult OnAddCall(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            // all arguments must be provided.
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                var floatValue = (float)inputArguments[0];
                var uintValue = (UInt32)inputArguments[1];

                // set output parameter
                outputArguments[0] = (float)(floatValue + uintValue);
                return ServiceResult.Good;
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }

        private ServiceResult OnMultiplyCall(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            // all arguments must be provided.
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                var op1 = (Int16)inputArguments[0];
                var op2 = (UInt16)inputArguments[1];

                // set output parameter
                outputArguments[0] = (Int32)(op1 * op2);
                return ServiceResult.Good;
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }

        private ServiceResult OnDivideCall(ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            // all arguments must be provided.
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                var op1 = (Int32)inputArguments[0];
                var op2 = (UInt16)inputArguments[1];

                // set output parameter
                outputArguments[0] = (float)((float)op1 / (float)op2);
                return ServiceResult.Good;
            }
            catch
            {
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }

        private ServiceResult OnSubstractCall(ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            // all arguments must be provided.
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                var op1 = (Int16)inputArguments[0];
                var op2 = (Byte)inputArguments[1];

                // set output parameter
                outputArguments[0] = (Int16)(op1 - op2);
                return ServiceResult.Good;
            }
            catch
            {
                return StatusCodes.BadInvalidArgument;
            }
        }

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
                return StatusCodes.BadInvalidArgument;
            }
        }

        private ServiceResult OnInputCall(ISystemContext context,
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

            return ServiceResult.Good;
        }

        private ServiceResult OnOutputCall(ISystemContext context,
            MethodState method,
            NodeId objectId,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            // all arguments must be provided.
            try
            {
                // set output parameter
                outputArguments[0] = (string)("Output");
                return ServiceResult.Good;
            }
            catch
            {
                return StatusCodes.BadInvalidArgument;
            }
        }

        private void ResetRandomGenerator(int seed, int boundaryValueFrequency = 0)
        {
            randomSource_ = new RandomSource(seed);
            generator_ = new DataGenerator(randomSource_);
            generator_.BoundaryValueFrequency = boundaryValueFrequency;
        }

        private object GetNewValue(BaseVariableState variable)
        {
            Debug.Assert(generator_ != null, "Need a random generator!");

            object value = null;
            var retryCount = 0;

            while (value == null && retryCount < 10)
            {
                value = generator_.GetRandom(variable.DataType, variable.ValueRank, new uint[] { 10 }, ServerData.TypeTree);
                // skip Variant Null
                if (value is Variant variant)
                {
                    if (variant.Value == null)
                    {
                        value = null;
                    }
                }
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

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                // TBD
            }
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override UaNodeHandle GetManagerHandle(UaServerContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                if (!PredefinedNodes.TryGetValue(nodeId, out var node))
                {
                    return null;
                }

                var handle = new UaNodeHandle { NodeId = nodeId, Node = node, Validated = true };


                return handle;
            }
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        public override NodeState ValidateNode(
           UaServerContext context,
           UaNodeHandle handle,
           IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            return handle.Validated ? handle.Node : null;
        }
        #endregion

        #region Overrides
        #endregion

        #region Private Fields
        private ReferenceServerConfiguration configuration_;
        private RandomSource randomSource_;
        private DataGenerator generator_;
        private Timer simulationTimer_;
        private UInt16 simulationInterval_ = 1000;
        private bool simulationEnabled_ = true;
        private List<BaseDataVariableState> dynamicNodes_;
        #endregion
    }

    public static class VariableExtensions
    {
        public static BaseDataVariableState MinimumSamplingInterval(this BaseDataVariableState variable, int minimumSamplingInterval)
        {
            variable.MinimumSamplingInterval = minimumSamplingInterval;
            return variable;
        }
    }
}