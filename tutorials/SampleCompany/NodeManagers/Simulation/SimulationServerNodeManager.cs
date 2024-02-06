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
using System.Threading;
using System.Numerics;

using Opc.Ua;
using BrowseNames = Opc.Ua.BrowseNames;
using ObjectIds = Opc.Ua.ObjectIds;
using ReferenceTypes = Opc.Ua.ReferenceTypes;

using Technosoftware.UaServer;
using Technosoftware.UaStandardServer;
#endregion

namespace SampleCompany.NodeManagers.Simulation
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class SimulationServerNodeManager : UaStandardNodeManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public SimulationServerNodeManager(
            IUaServerData uaServer,
            ApplicationConfiguration configuration)
            : base(uaServer, configuration, Namespaces.SimulationServer)
        {
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            configuration_ = configuration.ParseExtension<SimulationServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (configuration_ == null)
            {
                configuration_ = new SimulationServerConfiguration();
            }

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
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference> references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = References = new List<IReference>();
                }
                else
                {
                    References = references;
                }

                FolderState root = CreateFolderState(null, "CTT", "CTT", null);

                var variables = new List<BaseDataVariableState>();

                try
                {
                    #region Scalar_Static
                    ResetRandomGenerator(1);
                    FolderState scalarFolder = CreateFolderState(root, "Scalar", "Scalar", null);
                    BaseDataVariableState scalarInstructions = CreateBaseDataVariableState(scalarFolder, "Scalar_Instructions", "Scalar_Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    scalarInstructions.Value = "A library of Read/Write Variables of all supported data-types.";
                    variables.Add(scalarInstructions);

                    FolderState staticFolder = CreateFolderState(scalarFolder, "Scalar_Static", "Scalar_Static", null);
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

                    BaseDataVariableState decimalVariable = CreateBaseDataVariableState(staticFolder, scalarStatic + "Decimal", "Decimal", null, DataTypeIds.DecimalDataType, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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
                    FolderState arraysFolder = CreateFolderState(staticFolder, "Scalar_Static_Arrays", "Arrays", null);
                    const string staticArrays = "Scalar_Static_Arrays_";

                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
                    variables.Add(CreateBaseDataVariableState(arraysFolder, staticArrays + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));

                    BaseDataVariableState doubleArrayVar = CreateBaseDataVariableState(arraysFolder, staticArrays + "Double", "Double", null, DataTypeIds.Double, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null);
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

                    BaseDataVariableState floatArrayVar = CreateBaseDataVariableState(arraysFolder, staticArrays + "Float", "Float", null, DataTypeIds.Float, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null);
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

                    BaseDataVariableState stringArrayVar = CreateBaseDataVariableState(arraysFolder, staticArrays + "String", "String", null, DataTypeIds.String, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null);
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
                    FolderState arrays2DFolder = CreateFolderState(staticFolder, "Scalar_Static_Arrays2D", "Arrays2D", null);
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
                    FolderState arrayDynamicFolder = CreateFolderState(staticFolder, "Scalar_Static_ArrayDymamic", "ArrayDymamic", null);
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
                    FolderState massFolder = CreateFolderState(staticFolder, "Scalar_Static_Mass", "Mass", null);
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
                    FolderState simulationFolder = CreateFolderState(scalarFolder, "Scalar_Simulation", "Simulation", null);
                    const string scalarSimulation = "Scalar_Simulation_";
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar);
                    _ = CreateDynamicVariable(simulationFolder, scalarSimulation + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar);

                    BaseDataVariableState intervalVariable = CreateBaseDataVariableState(simulationFolder, scalarSimulation + "Interval", "Interval", null, DataTypeIds.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    intervalVariable.Value = simulationInterval_;
                    intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                    BaseDataVariableState enabledVariable = CreateBaseDataVariableState(simulationFolder, scalarSimulation + "Enabled", "Enabled", null, DataTypeIds.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    enabledVariable.Value = simulationEnabled_;
                    enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                    #endregion

                    #region Scalar_Simulation_Arrays
                    ResetRandomGenerator(7);
                    FolderState arraysSimulationFolder = CreateFolderState(simulationFolder, "Scalar_Simulation_Arrays", "Arrays", null);
                    const string simulationArrays = "Scalar_Simulation_Arrays_";
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Double", "Double", null, DataTypeIds.Double, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Float", "Float", null, DataTypeIds.Float, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Number", "Number", null, DataTypeIds.Number, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "String", "String", null, DataTypeIds.String, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.OneDimension);
                    _ = CreateDynamicVariable(arraysSimulationFolder, simulationArrays + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.OneDimension);
                    #endregion

                    #region Scalar_Simulation_Mass
                    ResetRandomGenerator(8);
                    FolderState massSimulationFolder = CreateFolderState(simulationFolder, "Scalar_Simulation_Mass", "Mass", null);
                    const string massSimulation = "Scalar_Simulation_Mass_";
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Boolean", "Boolean", null, DataTypeIds.Boolean, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Byte", "Byte", null, DataTypeIds.Byte, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "ByteString", "ByteString", null, DataTypeIds.ByteString, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "DateTime", "DateTime", null, DataTypeIds.DateTime, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Double", "Double", null, DataTypeIds.Double, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Float", "Float", null, DataTypeIds.Float, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Guid", "Guid", null, DataTypeIds.Guid, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Int16", "Int16", null, DataTypeIds.Int16, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Int32", "Int32", null, DataTypeIds.Int32, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Int64", "Int64", null, DataTypeIds.Int64, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Integer", "Integer", null, DataTypeIds.Integer, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "LocalizedText", "LocalizedText", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "NodeId", "NodeId", null, DataTypeIds.NodeId, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Number", "Number", null, DataTypeIds.Number, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "QualifiedName", "QualifiedName", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "SByte", "SByte", null, DataTypeIds.SByte, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "String", "String", null, DataTypeIds.String, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "UInt16", "UInt16", null, DataTypeIds.UInt16, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "UInt32", "UInt32", null, DataTypeIds.UInt32, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "UInt64", "UInt64", null, DataTypeIds.UInt64, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "UInteger", "UInteger", null, DataTypeIds.UInteger, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.Scalar, 100);
                    _ = CreateDynamicVariables(massSimulationFolder, massSimulation + "XmlElement", "XmlElement", null, DataTypeIds.XmlElement, ValueRanks.Scalar, 100);
                    #endregion

                    #region DataAccess_DataItem
                    ResetRandomGenerator(9);
                    FolderState daFolder = CreateFolderState(root, "DataAccess", "DataAccess", null);
                    BaseDataVariableState daInstructions = CreateBaseDataVariableState(daFolder, "DataAccess_Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    daInstructions.Value = "A library of Read/Write Variables of all supported data-types.";
                    variables.Add(daInstructions);

                    FolderState dataItemFolder = CreateFolderState(daFolder, "DataAccess_DataItem", "DataItem", null);
                    const string daDataItem = "DataAccess_DataItem_";

                    foreach (var name in Enum.GetNames(typeof(BuiltInType)))
                    {
                        DataItemState item = CreateDataItemState(dataItemFolder, daDataItem + name, name, null, (BuiltInType)Enum.Parse(typeof(BuiltInType), name), ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, String.Empty, 2, null, null);

                        // set initial value to String.Empty for String node.
                        if (name == BuiltInType.String.ToString())
                        {
                            item.Value = String.Empty;
                        }
                    }
                    #endregion

                    #region DataAccess_AnalogType
                    ResetRandomGenerator(10);
                    FolderState analogItemFolder = CreateFolderState(daFolder, "DataAccess_AnalogType", "AnalogType", null);
                    const string daAnalogItem = "DataAccess_AnalogType_";

                    foreach (var name in Enum.GetNames(typeof(BuiltInType)))
                    {
                        var builtInType = (BuiltInType)Enum.Parse(typeof(BuiltInType), name);
                        if (IsAnalogType(builtInType))
                        {
                            AnalogItemState item = CreateAnalogItemVariable(analogItemFolder, daAnalogItem + name, name, null, builtInType, ValueRanks.Scalar);

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
                    FolderState analogArrayFolder = CreateFolderState(analogItemFolder, "DataAccess_AnalogType_Array", "Array", null);
                    const string daAnalogArray = "DataAccess_AnalogType_Array_";

                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Boolean", "Boolean", null, BuiltInType.Boolean, ValueRanks.OneDimension, new[] { true, false, true, false, true, false, true, false, true });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Byte", "Byte", null, BuiltInType.Byte, ValueRanks.OneDimension, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "ByteString", "ByteString", null, BuiltInType.ByteString, ValueRanks.OneDimension, new[] { new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "DateTime", "DateTime", null, BuiltInType.DateTime, ValueRanks.OneDimension, new[] { DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Double", "Double", null, BuiltInType.Double, ValueRanks.OneDimension, new[] { 9.00001d, 9.0002d, 9.003d, 9.04d, 9.5d, 9.06d, 9.007d, 9.008d, 9.0009d });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Duration", "Duration", null, DataTypeIds.Duration, ValueRanks.OneDimension, new[] { 9.00001d, 9.0002d, 9.003d, 9.04d, 9.5d, 9.06d, 9.007d, 9.008d, 9.0009d }, null);
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Float", "Float", null, BuiltInType.Float, ValueRanks.OneDimension, new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 1.1f, 2.2f, 3.3f, 4.4f, 5.5f });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Guid", "Guid", null, BuiltInType.Guid, ValueRanks.OneDimension, new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Int16", "Int16", null, BuiltInType.Int16, ValueRanks.OneDimension, new Int16[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Int32", "Int32", null, BuiltInType.Int32, ValueRanks.OneDimension, new Int32[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Int64", "Int64", null, BuiltInType.Int64, ValueRanks.OneDimension, new Int64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Integer", "Integer", null, BuiltInType.Integer, ValueRanks.OneDimension, new Int64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "LocaleId", "LocaleId", null, DataTypeIds.LocaleId, ValueRanks.OneDimension, new string[] { "en", "fr", "de", "en", "fr", "de", "en", "fr", "de", "en" }, null);
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "LocalizedText", "LocalizedText", null, BuiltInType.LocalizedText, ValueRanks.OneDimension, new LocalizedText[] { new LocalizedText("en", "Hello World1"), new LocalizedText("en", "Hello World2"), new LocalizedText("en", "Hello World3"), new LocalizedText("en", "Hello World4"), new LocalizedText("en", "Hello World5"), new LocalizedText("en", "Hello World6"), new LocalizedText("en", "Hello World7"), new LocalizedText("en", "Hello World8"), new LocalizedText("en", "Hello World9"), new LocalizedText("en", "Hello World10") });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "NodeId", "NodeId", null, BuiltInType.NodeId, ValueRanks.OneDimension, new NodeId[] { new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()), new NodeId(Guid.NewGuid()) });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Number", "Number", null, BuiltInType.Number, ValueRanks.OneDimension, new Int16[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "QualifiedName", "QualifiedName", null, BuiltInType.QualifiedName, ValueRanks.OneDimension, new QualifiedName[] { "q0", "q1", "q2", "q3", "q4", "q5", "q6", "q7", "q8", "q9" });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "SByte", "SByte", null, BuiltInType.SByte, ValueRanks.OneDimension, new SByte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "String", "String", null, BuiltInType.String, ValueRanks.OneDimension, new[] { "a00", "b10", "c20", "d30", "e40", "f50", "g60", "h70", "i80", "j90" });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "UInt16", "UInt16", null, BuiltInType.UInt16, ValueRanks.OneDimension, new UInt16[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "UInt32", "UInt32", null, BuiltInType.UInt32, ValueRanks.OneDimension, new UInt32[] { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "UInt64", "UInt64", null, BuiltInType.UInt64, ValueRanks.OneDimension, new UInt64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "UInteger", "UInteger", null, BuiltInType.UInteger, ValueRanks.OneDimension, new UInt64[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "UtcTime", "UtcTime", null, DataTypeIds.UtcTime, ValueRanks.OneDimension, new DateTime[] { DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), DateTime.MinValue.ToUniversalTime() }, null);
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "Variant", "Variant", null, BuiltInType.Variant, ValueRanks.OneDimension, new Variant[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
                    var doc1 = new XmlDocument();
                    _ = CreateAnalogItemVariable(analogArrayFolder, daAnalogArray + "XmlElement", "XmlElement", null, BuiltInType.XmlElement, ValueRanks.OneDimension, new XmlElement[] { doc1.CreateElement("tag1"), doc1.CreateElement("tag2"), doc1.CreateElement("tag3"), doc1.CreateElement("tag4"), doc1.CreateElement("tag5"), doc1.CreateElement("tag6"), doc1.CreateElement("tag7"), doc1.CreateElement("tag8"), doc1.CreateElement("tag9"), doc1.CreateElement("tag10") });
                    #endregion

                    #region DataAccess_DiscreteType
                    ResetRandomGenerator(12);
                    FolderState discreteTypeFolder = CreateFolderState(daFolder, "DataAccess_DiscreteType", "DiscreteType", null);
                    FolderState twoStateDiscreteFolder = CreateFolderState(discreteTypeFolder, "DataAccess_TwoStateDiscreteType", "TwoStateDiscreteType", null);
                    const string daTwoStateDiscrete = "DataAccess_TwoStateDiscreteType_";

                    // Add our Nodes to the folder, and specify their customized discrete enumerations
                    _ = CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "001", "001", null, AccessLevels.CurrentReadOrWrite, false, "red", "blue");
                    _ = CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "002", "002", null, AccessLevels.CurrentReadOrWrite, false, "open", "close");
                    _ = CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "003", "003", null, AccessLevels.CurrentReadOrWrite, false, "up", "down");
                    _ = CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "004", "004", null, AccessLevels.CurrentReadOrWrite, false, "left", "right");
                    _ = CreateTwoStateDiscreteState(twoStateDiscreteFolder, daTwoStateDiscrete + "005", "005", null, AccessLevels.CurrentReadOrWrite, false, "circle", "cross");

                    FolderState multiStateDiscreteFolder = CreateFolderState(discreteTypeFolder, "DataAccess_MultiStateDiscreteType", "MultiStateDiscreteType", null);
                    const string daMultiStateDiscrete = "DataAccess_MultiStateDiscreteType_";

                    // Add our Nodes to the folder, and specify their customized discrete enumerations
                    MultiStateDiscreteState variable = CreateMultiStateDiscreteState(multiStateDiscreteFolder, daMultiStateDiscrete + "001", "001", null, AccessLevels.CurrentReadOrWrite, null,
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
                    FolderState multiStateValueDiscreteFolder = CreateFolderState(discreteTypeFolder, "DataAccess_MultiStateValueDiscreteType", "MultiStateValueDiscreteType", null);
                    const string daMultiStateValueDiscrete = "DataAccess_MultiStateValueDiscreteType_";

                    // Add our Nodes to the folder, and specify their customized discrete enumerations
                    MultiStateValueDiscreteState valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "001", "001", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "002", "002", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "red", "green", "blue", "cyan" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "003", "003", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "lolo", "lo", "normal", "hi", "hihi" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "004", "004", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "left", "right", "center" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "005", "005", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "circle", "cross", "triangle" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;

                    // Add our Nodes to the folder and specify varying data types
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Byte", "Byte", null, DataTypeIds.Byte, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Int16", "Int16", null, DataTypeIds.Int16, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "red", "green", "blue", "cyan" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Int32", "Int32", null, DataTypeIds.Int32, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "lolo", "lo", "normal", "hi", "hihi" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "Int64", "Int64", null, DataTypeIds.Int64, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "left", "right", "center" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "SByte", "SByte", null, DataTypeIds.SByte, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "UInt16", "UInt16", null, DataTypeIds.UInt16, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "red", "green", "blue", "cyan" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "UInt32", "UInt32", null, DataTypeIds.UInt32, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "lolo", "lo", "normal", "hi", "hihi" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;
                    valueDiscreteVariable = CreateMultiStateValueDiscreteState(multiStateValueDiscreteFolder, daMultiStateValueDiscrete + "UInt64", "UInt64", null, DataTypeIds.UInt64, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "left", "right", "center" });
                    valueDiscreteVariable.OnWriteValue = OnWriteValueDiscrete;

                    #endregion

                    #region References
                    ResetRandomGenerator(14);
                    FolderState referencesFolder = CreateFolderState(root, "References", "References", null);
                    const string referencesPrefix = "References_";

                    BaseDataVariableState referencesInstructions = CreateBaseDataVariableState(referencesFolder, "References_Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    referencesInstructions.Value = "This folder will contain nodes that have specific Reference configurations.";
                    variables.Add(referencesInstructions);

                    // create variable nodes with specific references
                    BaseDataVariableState hasForwardReference = CreateMeshVariable(referencesFolder, referencesPrefix + "HasForwardReference", "HasForwardReference");
                    hasForwardReference.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
                    variables.Add(hasForwardReference);

                    BaseDataVariableState hasInverseReference = CreateMeshVariable(referencesFolder, referencesPrefix + "HasInverseReference", "HasInverseReference");
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
                        BaseDataVariableState has3ForwardReferences = CreateMeshVariable(referencesFolder, referencesPrefix + referenceString, referenceString);
                        has3ForwardReferences.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
                        has3ForwardReferences.AddReference(ReferenceTypes.HasCause, false, variables[1].NodeId);
                        has3ForwardReferences.AddReference(ReferenceTypes.HasCause, false, variables[2].NodeId);
                        if (i == 1)
                        {
                            has3InverseReference = has3ForwardReferences;
                        }
                        variables.Add(has3ForwardReferences);
                    }

                    BaseDataVariableState has3InverseReferences = CreateMeshVariable(referencesFolder, referencesPrefix + "Has3InverseReferences", "Has3InverseReferences");
                    has3InverseReferences.AddReference(ReferenceTypes.HasEffect, true, variables[0].NodeId);
                    has3InverseReferences.AddReference(ReferenceTypes.HasEffect, true, variables[1].NodeId);
                    has3InverseReferences.AddReference(ReferenceTypes.HasEffect, true, variables[2].NodeId);
                    variables.Add(has3InverseReferences);

                    BaseDataVariableState hasForwardAndInverseReferences = CreateMeshVariable(referencesFolder, referencesPrefix + "HasForwardAndInverseReference", "HasForwardAndInverseReference", hasForwardReference, hasInverseReference, has3InverseReference, has3InverseReferences, variables[0]);
                    variables.Add(hasForwardAndInverseReferences);
                    #endregion

                    #region AccessRights
                    ResetRandomGenerator(15);
                    FolderState folderAccessRights = CreateFolderState(root, "AccessRights", "AccessRights", null);
                    const string accessRights = "AccessRights_";

                    BaseDataVariableState accessRightsInstructions = CreateBaseDataVariableState(folderAccessRights, accessRights + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessRightsInstructions.Value = "This folder will be accessible to all who enter, but contents therein will be secured.";
                    variables.Add(accessRightsInstructions);

                    // sub-folder for "AccessAll"
                    FolderState folderAccessRightsAccessAll = CreateFolderState(folderAccessRights, "AccessRights_AccessAll", "AccessAll", null);
                    const string accessRightsAccessAll = "AccessRights_AccessAll_";

                    BaseDataVariableState arAllRo = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO", "RO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRo.AccessLevel = AccessLevels.CurrentRead;
                    arAllRo.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arAllRo);
                    BaseDataVariableState arAllWo = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "WO", "WO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllWo.AccessLevel = AccessLevels.CurrentWrite;
                    arAllWo.UserAccessLevel = AccessLevels.CurrentWrite;
                    variables.Add(arAllWo);
                    BaseDataVariableState arAllRw = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RW", "RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRw.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arAllRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arAllRw);
                    BaseDataVariableState arAllRoNotUser = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO_NotUser", "RO_NotUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRoNotUser.AccessLevel = AccessLevels.CurrentRead;
                    arAllRoNotUser.UserAccessLevel = AccessLevels.None;
                    variables.Add(arAllRoNotUser);
                    BaseDataVariableState arAllWoNotUser = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "WO_NotUser", "WO_NotUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllWoNotUser.AccessLevel = AccessLevels.CurrentWrite;
                    arAllWoNotUser.UserAccessLevel = AccessLevels.None;
                    variables.Add(arAllWoNotUser);
                    BaseDataVariableState arAllRwNotUser = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RW_NotUser", "RW_NotUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRwNotUser.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arAllRwNotUser.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arAllRwNotUser);
                    BaseDataVariableState arAllRoUserRw = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO_User1_RW", "RO_User1_RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRoUserRw.AccessLevel = AccessLevels.CurrentRead;
                    arAllRoUserRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arAllRoUserRw);
                    BaseDataVariableState arAllRoGroupRw = CreateBaseDataVariableState(folderAccessRightsAccessAll, accessRightsAccessAll + "RO_Group1_RW", "RO_Group1_RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arAllRoGroupRw.AccessLevel = AccessLevels.CurrentRead;
                    arAllRoGroupRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arAllRoGroupRw);

                    // sub-folder for "AccessUser1"
                    FolderState folderAccessRightsAccessUser1 = CreateFolderState(folderAccessRights, "AccessRights_AccessUser1", "AccessUser1", null);
                    const string accessRightsAccessUser1 = "AccessRights_AccessUser1_";

                    BaseDataVariableState arUserRo = CreateBaseDataVariableState(folderAccessRightsAccessUser1, accessRightsAccessUser1 + "RO", "RO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arUserRo.AccessLevel = AccessLevels.CurrentRead;
                    arUserRo.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arUserRo);
                    BaseDataVariableState arUserWo = CreateBaseDataVariableState(folderAccessRightsAccessUser1, accessRightsAccessUser1 + "WO", "WO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arUserWo.AccessLevel = AccessLevels.CurrentWrite;
                    arUserWo.UserAccessLevel = AccessLevels.CurrentWrite;
                    variables.Add(arUserWo);
                    BaseDataVariableState arUserRw = CreateBaseDataVariableState(folderAccessRightsAccessUser1, accessRightsAccessUser1 + "RW", "RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arUserRw.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arUserRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arUserRw);

                    // sub-folder for "AccessGroup1"
                    FolderState folderAccessRightsAccessGroup1 = CreateFolderState(folderAccessRights, "AccessRights_AccessGroup1", "AccessGroup1", null);
                    const string accessRightsAccessGroup1 = "AccessRights_AccessGroup1_";

                    BaseDataVariableState arGroupRo = CreateBaseDataVariableState(folderAccessRightsAccessGroup1, accessRightsAccessGroup1 + "RO", "RO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arGroupRo.AccessLevel = AccessLevels.CurrentRead;
                    arGroupRo.UserAccessLevel = AccessLevels.CurrentRead;
                    variables.Add(arGroupRo);
                    BaseDataVariableState arGroupWo = CreateBaseDataVariableState(folderAccessRightsAccessGroup1, accessRightsAccessGroup1 + "WO", "WO", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arGroupWo.AccessLevel = AccessLevels.CurrentWrite;
                    arGroupWo.UserAccessLevel = AccessLevels.CurrentWrite;
                    variables.Add(arGroupWo);
                    BaseDataVariableState arGroupRw = CreateBaseDataVariableState(folderAccessRightsAccessGroup1, accessRightsAccessGroup1 + "RW", "RW", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arGroupRw.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    arGroupRw.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    variables.Add(arGroupRw);

                    // sub folder for "RolePermissions"
                    FolderState folderRolePermissions = CreateFolderState(folderAccessRights, "AccessRights_RolePermissions", "RolePermissions", null);
                    const string rolePermissions = "AccessRights_RolePermissions_";

                    BaseDataVariableState rpAnonymous = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AnonymousAccess", "AnonymousAccess", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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

                    BaseDataVariableState rpAuthenticatedUser = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AuthenticatedUser", "AuthenticatedUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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

                    BaseDataVariableState rpSecurityAdminUser = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AdminUser", "AdminUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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

                    BaseDataVariableState rpConfigAdminUser = CreateBaseDataVariableState(folderRolePermissions, rolePermissions + "AdminUser", "AdminUser", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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
                    FolderState folderAccessRestrictions = CreateFolderState(folderAccessRights, "AccessRights_AccessRestrictions", "AccessRestrictions", null);
                    const string accessRestrictions = "AccessRights_AccessRestrictions_";

                    BaseDataVariableState arNone = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "None", "None", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arNone.AccessLevel = AccessLevels.CurrentRead;
                    arNone.UserAccessLevel = AccessLevels.CurrentRead;
                    arNone.AccessRestrictions = AccessRestrictionType.None;
                    variables.Add(arNone);

                    BaseDataVariableState arSigningRequired = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "SigningRequired", "SigningRequired", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arSigningRequired.AccessLevel = AccessLevels.CurrentRead;
                    arSigningRequired.UserAccessLevel = AccessLevels.CurrentRead;
                    arSigningRequired.AccessRestrictions = AccessRestrictionType.SigningRequired;
                    variables.Add(arSigningRequired);

                    BaseDataVariableState arEncryptionRequired = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "EncryptionRequired", "EncryptionRequired", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arEncryptionRequired.AccessLevel = AccessLevels.CurrentRead;
                    arEncryptionRequired.UserAccessLevel = AccessLevels.CurrentRead;
                    arEncryptionRequired.AccessRestrictions = AccessRestrictionType.EncryptionRequired;
                    variables.Add(arEncryptionRequired);

                    BaseDataVariableState arSessionRequired = CreateBaseDataVariableState(folderAccessRestrictions, accessRestrictions + "SessionRequired", "SessionRequired", null, BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arSessionRequired.AccessLevel = AccessLevels.CurrentRead;
                    arSessionRequired.UserAccessLevel = AccessLevels.CurrentRead;
                    arSessionRequired.AccessRestrictions = AccessRestrictionType.SessionRequired;
                    variables.Add(arSessionRequired);
                    #endregion

                    #region NodeIds
                    ResetRandomGenerator(16);
                    FolderState nodeIdsFolder = CreateFolderState(root, "NodeIds", "NodeIds", null);
                    const string nodeIds = "NodeIds_";

                    BaseDataVariableState nodeIdsInstructions = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeIdsInstructions.Value = "All supported Node types are available except whichever is in use for the other nodes.";
                    variables.Add(nodeIdsInstructions);

                    BaseDataVariableState integerNodeId = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16Integer", "Int16Integer", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    integerNodeId.NodeId = new NodeId(9202, NamespaceIndex);
                    variables.Add(integerNodeId);

                    variables.Add(CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16String", "Int16String", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));

                    BaseDataVariableState guidNodeId = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16GUID", "Int16GUID", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    guidNodeId.NodeId = new NodeId(new Guid("00000000-0000-0000-0000-000000009204"), NamespaceIndex);
                    variables.Add(guidNodeId);

                    BaseDataVariableState opaqueNodeId = CreateBaseDataVariableState(nodeIdsFolder, nodeIds + "Int16Opaque", "Int16Opaque", null, DataTypeIds.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    opaqueNodeId.NodeId = new NodeId(new byte[] { 9, 2, 0, 5 }, NamespaceIndex);
                    variables.Add(opaqueNodeId);
                    #endregion

                    #region Methods
                    FolderState methodsFolder = CreateFolderState(root, "Methods", "Methods", null);
                    const string methods = "Methods_";

                    BaseDataVariableState methodsInstructions = CreateBaseDataVariableState(methodsFolder, methods + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    methodsInstructions.Value = "Contains methods with varying parameter definitions.";
                    variables.Add(methodsInstructions);

                    #region Void Method
                    _ = CreateMethodState(methodsFolder, methods + "Void", "Void", OnVoidCall);
                    #endregion

                    #region Add Method
                    MethodState addMethod = CreateMethodState(methodsFolder, methods + "Add", "Add", OnAddCall);
                    // set input arguments
                    Argument inputArgument1 = CreateArgument("Float value", "Float value", BuiltInType.Float, ValueRanks.Scalar);
                    Argument inputArgument2 = CreateArgument("UInt32 value", "UInt32 value", BuiltInType.UInt32, ValueRanks.Scalar);
                    _ = AddInputArguments(addMethod, new[] { inputArgument1, inputArgument2 });

                    // set output arguments
                    Argument outputArgument1 = CreateArgument("Add Result", "Add Result", BuiltInType.Float, ValueRanks.Scalar);
                    _ = AddOutputArguments(addMethod, new[] { outputArgument1 });
                    #endregion

                    #region Multiply Method
                    MethodState multiplyMethod = CreateMethodState(methodsFolder, methods + "Multiply", "Multiply", OnMultiplyCall);
                    // set input arguments
                    inputArgument1 = CreateArgument("Int16 value", "Int16 value", BuiltInType.Int16, ValueRanks.Scalar);
                    inputArgument2 = CreateArgument("UInt16 value", "UInt16 value", BuiltInType.UInt16, ValueRanks.Scalar);
                    _ = AddInputArguments(multiplyMethod, new[] { inputArgument1, inputArgument2 });

                    // set output arguments
                    multiplyMethod.OutputArguments = new PropertyState<Argument[]>(multiplyMethod);
                    multiplyMethod.OutputArguments.NodeId = new NodeId(multiplyMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    multiplyMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    multiplyMethod.OutputArguments.DisplayName = multiplyMethod.OutputArguments.BrowseName.Name;
                    multiplyMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    multiplyMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    multiplyMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    multiplyMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    outputArgument1 = CreateArgument("Multiply Result", "Multiply Result", BuiltInType.Int32, ValueRanks.Scalar);
                    _ = AddOutputArguments(multiplyMethod, new[] { outputArgument1 });
                    #endregion

                    #region Divide Method
                    MethodState divideMethod = CreateMethodState(methodsFolder, methods + "Divide", "Divide", new GenericMethodCalledEventHandler2(OnDivideCall));
                    // set input arguments
                    inputArgument1 = CreateArgument("Int32 value", "Int32 value", BuiltInType.Int32, ValueRanks.Scalar);
                    inputArgument2 = CreateArgument("UInt16 value", "UInt16 value", BuiltInType.UInt16, ValueRanks.Scalar);
                    _ = AddInputArguments(divideMethod, new[] { inputArgument1, inputArgument2 });

                    // set output arguments
                    outputArgument1 = CreateArgument("Divide Result", "Divide Result", BuiltInType.Float, ValueRanks.Scalar);
                    _ = AddOutputArguments(divideMethod, new[] { outputArgument1 });
                    #endregion

                    #region Substract Method
                    MethodState substractMethod = CreateMethodState(methodsFolder, methods + "Substract", "Substract", new GenericMethodCalledEventHandler2(OnSubstractCall));
                    // set input arguments
                    inputArgument1 = CreateArgument("Int16 value", "Int16 value", BuiltInType.Int16, ValueRanks.Scalar);
                    inputArgument2 = CreateArgument("Byte value", "Byte value", BuiltInType.Byte, ValueRanks.Scalar);
                    _ = AddInputArguments(substractMethod, new[] { inputArgument1, inputArgument2 });

                    // set output arguments
                    outputArgument1 = CreateArgument("Substract Result", "Substract Result", BuiltInType.Int16, ValueRanks.Scalar);
                    _ = AddOutputArguments(substractMethod, new[] { outputArgument1 });
                    #endregion

                    #region Hello Method
                    MethodState helloMethod = CreateMethodState(methodsFolder, methods + "Hello", "Hello", new GenericMethodCalledEventHandler2(OnHelloCall));
                    // set input arguments
                    inputArgument1 = CreateArgument("String value", "String value", BuiltInType.String, ValueRanks.Scalar);
                    _ = AddInputArguments(helloMethod, new[] { inputArgument1 });

                    // set output arguments
                    outputArgument1 = CreateArgument("Hello Result", "Hello Result", BuiltInType.String, ValueRanks.Scalar);
                    _ = AddOutputArguments(helloMethod, new[] { outputArgument1 });
                    #endregion

                    #region Input Method
                    MethodState inputMethod = CreateMethodState(methodsFolder, methods + "Input", "Input", new GenericMethodCalledEventHandler2(OnInputCall));
                    // set input arguments
                    inputArgument1 = CreateArgument("String value", "String value", BuiltInType.String, ValueRanks.Scalar);
                    _ = AddInputArguments(inputMethod, new[] { inputArgument1 });
                    #endregion

                    #region Output Method
                    MethodState outputMethod = CreateMethodState(methodsFolder, methods + "Output", "Output", new GenericMethodCalledEventHandler2(OnOutputCall));

                    // set output arguments
                    outputArgument1 = CreateArgument("Output Result", "Output Result", BuiltInType.String, ValueRanks.Scalar);
                    _ = AddOutputArguments(outputMethod, new[] { outputArgument1 });
                    #endregion
                    #endregion

                    #region Views
                    ResetRandomGenerator(18);
                    FolderState viewsFolder = CreateFolderState(root, "Views", "Views", null);
                    const string views = "Views_";

                    ViewState viewStateOperations = CreateViewState(viewsFolder, externalReferences, views + "Operations", "Operations", null);
                    ViewState viewStateEngineering = CreateViewState(viewsFolder, externalReferences, views + "Engineering", "Engineering", null);
                    #endregion

                    #region Locales
                    ResetRandomGenerator(19);
                    FolderState localesFolder = CreateFolderState(root, "Locales", "Locales", null);
                    const string locales = "Locales_";

                    BaseDataVariableState qnEnglishVariable = CreateBaseDataVariableState(localesFolder, locales + "QNEnglish", "QNEnglish", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnEnglishVariable.Description = new LocalizedText("en", "English");
                    qnEnglishVariable.Value = new QualifiedName("Hello World", NamespaceIndex);
                    variables.Add(qnEnglishVariable);
                    BaseDataVariableState ltEnglishVariable = CreateBaseDataVariableState(localesFolder, locales + "LTEnglish", "LTEnglish", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltEnglishVariable.Description = new LocalizedText("en", "English");
                    ltEnglishVariable.Value = new LocalizedText("en", "Hello World");
                    variables.Add(ltEnglishVariable);

                    BaseDataVariableState qnFrancaisVariable = CreateBaseDataVariableState(localesFolder, locales + "QNFrancais", "QNFrancais", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnFrancaisVariable.Description = new LocalizedText("en", "Francais");
                    qnFrancaisVariable.Value = new QualifiedName("Salut tout le monde", NamespaceIndex);
                    variables.Add(qnFrancaisVariable);
                    BaseDataVariableState ltFrancaisVariable = CreateBaseDataVariableState(localesFolder, locales + "LTFrancais", "LTFrancais", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltFrancaisVariable.Description = new LocalizedText("en", "Francais");
                    ltFrancaisVariable.Value = new LocalizedText("fr", "Salut tout le monde");
                    variables.Add(ltFrancaisVariable);

                    BaseDataVariableState qnDeutschVariable = CreateBaseDataVariableState(localesFolder, locales + "QNDeutsch", "QNDeutsch", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnDeutschVariable.Description = new LocalizedText("en", "Deutsch");
                    qnDeutschVariable.Value = new QualifiedName("Hallo Welt", NamespaceIndex);
                    variables.Add(qnDeutschVariable);
                    BaseDataVariableState ltDeutschVariable = CreateBaseDataVariableState(localesFolder, locales + "LTDeutsch", "LTDeutsch", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltDeutschVariable.Description = new LocalizedText("en", "Deutsch");
                    ltDeutschVariable.Value = new LocalizedText("de", "Hallo Welt");
                    variables.Add(ltDeutschVariable);

                    BaseDataVariableState qnEspanolVariable = CreateBaseDataVariableState(localesFolder, locales + "QNEspanol", "QNEspanol", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnEspanolVariable.Description = new LocalizedText("en", "Espanol");
                    qnEspanolVariable.Value = new QualifiedName("Hola mundo", NamespaceIndex);
                    variables.Add(qnEspanolVariable);
                    BaseDataVariableState ltEspanolVariable = CreateBaseDataVariableState(localesFolder, locales + "LTEspanol", "LTEspanol", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltEspanolVariable.Description = new LocalizedText("en", "Espanol");
                    ltEspanolVariable.Value = new LocalizedText("es", "Hola mundo");
                    variables.Add(ltEspanolVariable);

                    BaseDataVariableState qnJapaneseVariable = CreateBaseDataVariableState(localesFolder, locales + "QN日本の", "QN日本の", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnJapaneseVariable.Description = new LocalizedText("en", "Japanese");
                    qnJapaneseVariable.Value = new QualifiedName("ハローワールド", NamespaceIndex);
                    variables.Add(qnJapaneseVariable);
                    BaseDataVariableState ltJapaneseVariable = CreateBaseDataVariableState(localesFolder, locales + "LT日本の", "LT日本の", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltJapaneseVariable.Description = new LocalizedText("en", "Japanese");
                    ltJapaneseVariable.Value = new LocalizedText("jp", "ハローワールド");
                    variables.Add(ltJapaneseVariable);

                    BaseDataVariableState qnChineseVariable = CreateBaseDataVariableState(localesFolder, locales + "QN中國的", "QN中國的", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnChineseVariable.Description = new LocalizedText("en", "Chinese");
                    qnChineseVariable.Value = new QualifiedName("世界您好", NamespaceIndex);
                    variables.Add(qnChineseVariable);
                    BaseDataVariableState ltChineseVariable = CreateBaseDataVariableState(localesFolder, locales + "LT中國的", "LT中國的", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltChineseVariable.Description = new LocalizedText("en", "Chinese");
                    ltChineseVariable.Value = new LocalizedText("ch", "世界您好");
                    variables.Add(ltChineseVariable);

                    BaseDataVariableState qnRussianVariable = CreateBaseDataVariableState(localesFolder, locales + "QNрусский", "QNрусский", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnRussianVariable.Description = new LocalizedText("en", "Russian");
                    qnRussianVariable.Value = new QualifiedName("LTрусский", NamespaceIndex);
                    variables.Add(qnRussianVariable);
                    BaseDataVariableState ltRussianVariable = CreateBaseDataVariableState(localesFolder, locales + "LTрусский", "LTрусский", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltRussianVariable.Description = new LocalizedText("en", "Russian");
                    ltRussianVariable.Value = new LocalizedText("ru", "LTрусский");
                    variables.Add(ltRussianVariable);

                    BaseDataVariableState qnArabicVariable = CreateBaseDataVariableState(localesFolder, locales + "QNالعربية", "QNالعربية", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnArabicVariable.Description = new LocalizedText("en", "Arabic");
                    qnArabicVariable.Value = new QualifiedName("مرحبا بالعال", NamespaceIndex);
                    variables.Add(qnArabicVariable);
                    BaseDataVariableState ltArabicVariable = CreateBaseDataVariableState(localesFolder, locales + "LTالعربية", "LTالعربية", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltArabicVariable.Description = new LocalizedText("en", "Arabic");
                    ltArabicVariable.Value = new LocalizedText("ae", "مرحبا بالعال");
                    variables.Add(ltArabicVariable);

                    BaseDataVariableState qnKlingonVariable = CreateBaseDataVariableState(localesFolder, locales + "QNtlhIngan", "QNtlhIngan", null, DataTypeIds.QualifiedName, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    qnKlingonVariable.Description = new LocalizedText("en", "Klingon");
                    qnKlingonVariable.Value = new QualifiedName("qo' vIvan", NamespaceIndex);
                    variables.Add(qnKlingonVariable);
                    BaseDataVariableState ltKlingonVariable = CreateBaseDataVariableState(localesFolder, locales + "LTtlhIngan", "LTtlhIngan", null, DataTypeIds.LocalizedText, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    ltKlingonVariable.Description = new LocalizedText("en", "Klingon");
                    ltKlingonVariable.Value = new LocalizedText("ko", "qo' vIvan");
                    variables.Add(ltKlingonVariable);
                    #endregion

                    #region Attributes
                    ResetRandomGenerator(20);
                    FolderState folderAttributes = CreateFolderState(root, "Attributes", "Attributes", null);

                    #region AccessAll
                    FolderState folderAttributesAccessAll = CreateFolderState(folderAttributes, "Attributes_AccessAll", "AccessAll", null);
                    const string attributesAccessAll = "Attributes_AccessAll_";

                    BaseDataVariableState accessLevelAccessAll = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "AccessLevel", "AccessLevel", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessLevelAccessAll.WriteMask = AttributeWriteMask.AccessLevel;
                    accessLevelAccessAll.UserWriteMask = AttributeWriteMask.AccessLevel;
                    variables.Add(accessLevelAccessAll);

                    BaseDataVariableState arrayDimensionsAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ArrayDimensions", "ArrayDimensions", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arrayDimensionsAccessLevel.WriteMask = AttributeWriteMask.ArrayDimensions;
                    arrayDimensionsAccessLevel.UserWriteMask = AttributeWriteMask.ArrayDimensions;
                    variables.Add(arrayDimensionsAccessLevel);

                    BaseDataVariableState browseNameAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "BrowseName", "BrowseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    browseNameAccessLevel.WriteMask = AttributeWriteMask.BrowseName;
                    browseNameAccessLevel.UserWriteMask = AttributeWriteMask.BrowseName;
                    variables.Add(browseNameAccessLevel);

                    BaseDataVariableState containsNoLoopsAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ContainsNoLoops", "ContainsNoLoops", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    containsNoLoopsAccessLevel.WriteMask = AttributeWriteMask.ContainsNoLoops;
                    containsNoLoopsAccessLevel.UserWriteMask = AttributeWriteMask.ContainsNoLoops;
                    variables.Add(containsNoLoopsAccessLevel);

                    BaseDataVariableState dataTypeAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "DataType", "DataType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    dataTypeAccessLevel.WriteMask = AttributeWriteMask.DataType;
                    dataTypeAccessLevel.UserWriteMask = AttributeWriteMask.DataType;
                    variables.Add(dataTypeAccessLevel);

                    BaseDataVariableState descriptionAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Description", "Description", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    descriptionAccessLevel.WriteMask = AttributeWriteMask.Description;
                    descriptionAccessLevel.UserWriteMask = AttributeWriteMask.Description;
                    variables.Add(descriptionAccessLevel);

                    BaseDataVariableState eventNotifierAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "EventNotifier", "EventNotifier", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    eventNotifierAccessLevel.WriteMask = AttributeWriteMask.EventNotifier;
                    eventNotifierAccessLevel.UserWriteMask = AttributeWriteMask.EventNotifier;
                    variables.Add(eventNotifierAccessLevel);

                    BaseDataVariableState executableAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Executable", "Executable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    executableAccessLevel.WriteMask = AttributeWriteMask.Executable;
                    executableAccessLevel.UserWriteMask = AttributeWriteMask.Executable;
                    variables.Add(executableAccessLevel);

                    BaseDataVariableState historizingAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Historizing", "Historizing", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    historizingAccessLevel.WriteMask = AttributeWriteMask.Historizing;
                    historizingAccessLevel.UserWriteMask = AttributeWriteMask.Historizing;
                    variables.Add(historizingAccessLevel);

                    BaseDataVariableState inverseNameAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "InverseName", "InverseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    inverseNameAccessLevel.WriteMask = AttributeWriteMask.InverseName;
                    inverseNameAccessLevel.UserWriteMask = AttributeWriteMask.InverseName;
                    variables.Add(inverseNameAccessLevel);

                    BaseDataVariableState isAbstractAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "IsAbstract", "IsAbstract", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    isAbstractAccessLevel.WriteMask = AttributeWriteMask.IsAbstract;
                    isAbstractAccessLevel.UserWriteMask = AttributeWriteMask.IsAbstract;
                    variables.Add(isAbstractAccessLevel);

                    BaseDataVariableState minimumSamplingIntervalAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "MinimumSamplingInterval", "MinimumSamplingInterval", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    minimumSamplingIntervalAccessLevel.WriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    minimumSamplingIntervalAccessLevel.UserWriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    variables.Add(minimumSamplingIntervalAccessLevel);

                    BaseDataVariableState nodeClassIntervalAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "NodeClass", "NodeClass", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeClassIntervalAccessLevel.WriteMask = AttributeWriteMask.NodeClass;
                    nodeClassIntervalAccessLevel.UserWriteMask = AttributeWriteMask.NodeClass;
                    variables.Add(nodeClassIntervalAccessLevel);

                    BaseDataVariableState nodeIdAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "NodeId", "NodeId", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeIdAccessLevel.WriteMask = AttributeWriteMask.NodeId;
                    nodeIdAccessLevel.UserWriteMask = AttributeWriteMask.NodeId;
                    variables.Add(nodeIdAccessLevel);

                    BaseDataVariableState symmetricAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "Symmetric", "Symmetric", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    symmetricAccessLevel.WriteMask = AttributeWriteMask.Symmetric;
                    symmetricAccessLevel.UserWriteMask = AttributeWriteMask.Symmetric;
                    variables.Add(symmetricAccessLevel);

                    BaseDataVariableState userAccessLevelAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "UserAccessLevel", "UserAccessLevel", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userAccessLevelAccessLevel.WriteMask = AttributeWriteMask.UserAccessLevel;
                    userAccessLevelAccessLevel.UserWriteMask = AttributeWriteMask.UserAccessLevel;
                    variables.Add(userAccessLevelAccessLevel);

                    BaseDataVariableState userExecutableAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "UserExecutable", "UserExecutable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userExecutableAccessLevel.WriteMask = AttributeWriteMask.UserExecutable;
                    userExecutableAccessLevel.UserWriteMask = AttributeWriteMask.UserExecutable;
                    variables.Add(userExecutableAccessLevel);

                    BaseDataVariableState valueRankAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ValueRank", "ValueRank", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueRankAccessLevel.WriteMask = AttributeWriteMask.ValueRank;
                    valueRankAccessLevel.UserWriteMask = AttributeWriteMask.ValueRank;
                    variables.Add(valueRankAccessLevel);

                    BaseDataVariableState writeMaskAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "WriteMask", "WriteMask", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    writeMaskAccessLevel.WriteMask = AttributeWriteMask.WriteMask;
                    writeMaskAccessLevel.UserWriteMask = AttributeWriteMask.WriteMask;
                    variables.Add(writeMaskAccessLevel);

                    BaseDataVariableState valueForVariableTypeAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "ValueForVariableType", "ValueForVariableType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueForVariableTypeAccessLevel.WriteMask = AttributeWriteMask.ValueForVariableType;
                    valueForVariableTypeAccessLevel.UserWriteMask = AttributeWriteMask.ValueForVariableType;
                    variables.Add(valueForVariableTypeAccessLevel);

                    BaseDataVariableState allAccessLevel = CreateBaseDataVariableState(folderAttributesAccessAll, attributesAccessAll + "All", "All", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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
                    FolderState folderAttributesAccessUser1 = CreateFolderState(folderAttributes, "Attributes_AccessUser1", "AccessUser1", null);
                    const string attributesAccessUser1 = "Attributes_AccessUser1_";

                    BaseDataVariableState accessLevelAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "AccessLevel", "AccessLevel", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    accessLevelAccessUser1.WriteMask = AttributeWriteMask.AccessLevel;
                    accessLevelAccessUser1.UserWriteMask = AttributeWriteMask.AccessLevel;
                    variables.Add(accessLevelAccessUser1);

                    BaseDataVariableState arrayDimensionsAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ArrayDimensions", "ArrayDimensions", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    arrayDimensionsAccessUser1.WriteMask = AttributeWriteMask.ArrayDimensions;
                    arrayDimensionsAccessUser1.UserWriteMask = AttributeWriteMask.ArrayDimensions;
                    variables.Add(arrayDimensionsAccessUser1);

                    BaseDataVariableState browseNameAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "BrowseName", "BrowseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    browseNameAccessUser1.WriteMask = AttributeWriteMask.BrowseName;
                    browseNameAccessUser1.UserWriteMask = AttributeWriteMask.BrowseName;
                    variables.Add(browseNameAccessUser1);

                    BaseDataVariableState containsNoLoopsAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ContainsNoLoops", "ContainsNoLoops", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    containsNoLoopsAccessUser1.WriteMask = AttributeWriteMask.ContainsNoLoops;
                    containsNoLoopsAccessUser1.UserWriteMask = AttributeWriteMask.ContainsNoLoops;
                    variables.Add(containsNoLoopsAccessUser1);

                    BaseDataVariableState dataTypeAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "DataType", "DataType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    dataTypeAccessUser1.WriteMask = AttributeWriteMask.DataType;
                    dataTypeAccessUser1.UserWriteMask = AttributeWriteMask.DataType;
                    variables.Add(dataTypeAccessUser1);

                    BaseDataVariableState descriptionAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Description", "Description", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    descriptionAccessUser1.WriteMask = AttributeWriteMask.Description;
                    descriptionAccessUser1.UserWriteMask = AttributeWriteMask.Description;
                    variables.Add(descriptionAccessUser1);

                    BaseDataVariableState eventNotifierAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "EventNotifier", "EventNotifier", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    eventNotifierAccessUser1.WriteMask = AttributeWriteMask.EventNotifier;
                    eventNotifierAccessUser1.UserWriteMask = AttributeWriteMask.EventNotifier;
                    variables.Add(eventNotifierAccessUser1);

                    BaseDataVariableState executableAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Executable", "Executable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    executableAccessUser1.WriteMask = AttributeWriteMask.Executable;
                    executableAccessUser1.UserWriteMask = AttributeWriteMask.Executable;
                    variables.Add(executableAccessUser1);

                    BaseDataVariableState historizingAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Historizing", "Historizing", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    historizingAccessUser1.WriteMask = AttributeWriteMask.Historizing;
                    historizingAccessUser1.UserWriteMask = AttributeWriteMask.Historizing;
                    variables.Add(historizingAccessUser1);

                    BaseDataVariableState inverseNameAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "InverseName", "InverseName", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    inverseNameAccessUser1.WriteMask = AttributeWriteMask.InverseName;
                    inverseNameAccessUser1.UserWriteMask = AttributeWriteMask.InverseName;
                    variables.Add(inverseNameAccessUser1);

                    BaseDataVariableState isAbstractAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "IsAbstract", "IsAbstract", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    isAbstractAccessUser1.WriteMask = AttributeWriteMask.IsAbstract;
                    isAbstractAccessUser1.UserWriteMask = AttributeWriteMask.IsAbstract;
                    variables.Add(isAbstractAccessUser1);

                    BaseDataVariableState minimumSamplingIntervalAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "MinimumSamplingInterval", "MinimumSamplingInterval", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    minimumSamplingIntervalAccessUser1.WriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    minimumSamplingIntervalAccessUser1.UserWriteMask = AttributeWriteMask.MinimumSamplingInterval;
                    variables.Add(minimumSamplingIntervalAccessUser1);

                    BaseDataVariableState nodeClassIntervalAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "NodeClass", "NodeClass", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeClassIntervalAccessUser1.WriteMask = AttributeWriteMask.NodeClass;
                    nodeClassIntervalAccessUser1.UserWriteMask = AttributeWriteMask.NodeClass;
                    variables.Add(nodeClassIntervalAccessUser1);

                    BaseDataVariableState nodeIdAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "NodeId", "NodeId", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    nodeIdAccessUser1.WriteMask = AttributeWriteMask.NodeId;
                    nodeIdAccessUser1.UserWriteMask = AttributeWriteMask.NodeId;
                    variables.Add(nodeIdAccessUser1);

                    BaseDataVariableState symmetricAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "Symmetric", "Symmetric", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    symmetricAccessUser1.WriteMask = AttributeWriteMask.Symmetric;
                    symmetricAccessUser1.UserWriteMask = AttributeWriteMask.Symmetric;
                    variables.Add(symmetricAccessUser1);

                    BaseDataVariableState userAccessUser1AccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "UserAccessUser1", "UserAccessUser1", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userAccessUser1AccessUser1.WriteMask = AttributeWriteMask.UserAccessLevel;
                    userAccessUser1AccessUser1.UserWriteMask = AttributeWriteMask.UserAccessLevel;
                    variables.Add(userAccessUser1AccessUser1);

                    BaseDataVariableState userExecutableAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "UserExecutable", "UserExecutable", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    userExecutableAccessUser1.WriteMask = AttributeWriteMask.UserExecutable;
                    userExecutableAccessUser1.UserWriteMask = AttributeWriteMask.UserExecutable;
                    variables.Add(userExecutableAccessUser1);

                    BaseDataVariableState valueRankAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ValueRank", "ValueRank", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueRankAccessUser1.WriteMask = AttributeWriteMask.ValueRank;
                    valueRankAccessUser1.UserWriteMask = AttributeWriteMask.ValueRank;
                    variables.Add(valueRankAccessUser1);

                    BaseDataVariableState writeMaskAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "WriteMask", "WriteMask", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    writeMaskAccessUser1.WriteMask = AttributeWriteMask.WriteMask;
                    writeMaskAccessUser1.UserWriteMask = AttributeWriteMask.WriteMask;
                    variables.Add(writeMaskAccessUser1);

                    BaseDataVariableState valueForVariableTypeAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "ValueForVariableType", "ValueForVariableType", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    valueForVariableTypeAccessUser1.WriteMask = AttributeWriteMask.ValueForVariableType;
                    valueForVariableTypeAccessUser1.UserWriteMask = AttributeWriteMask.ValueForVariableType;
                    variables.Add(valueForVariableTypeAccessUser1);

                    BaseDataVariableState allAccessUser1 = CreateBaseDataVariableState(folderAttributesAccessUser1, attributesAccessUser1 + "All", "All", null, DataTypeIds.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
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
                    FolderState myCompanyFolder = CreateFolderState(root, "MyCompany", "MyCompany", null);
                    const string myCompany = "MyCompany_";

                    BaseDataVariableState myCompanyInstructions = CreateBaseDataVariableState(myCompanyFolder, myCompany + "Instructions", "Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    myCompanyInstructions.Value = "A place for the vendor to describe their address-space.";
                    variables.Add(myCompanyInstructions);
                    #endregion

                    #region StandardServerTest
                    ResetRandomGenerator(1);
                    FolderState standardServerTestFolder = CreateFolderState(root, "StandardServerTest", "StandardServerTest", null);
                    const string standardServerTest = "StandardServerTest_";

                    BaseDataVariableState standardServerTestInstructions = CreateBaseDataVariableState(standardServerTestFolder, "StandardServerTest_Instructions", "StandardServerTest_Instructions", null, DataTypeIds.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
                    standardServerTestInstructions.Value = "Several variables to increase code coverage of all supported UaStandardServer methods.";

                    BaseObjectState baseObjectState = CreateBaseObjectState(standardServerTestFolder, standardServerTest + "BaseObjectState1", "BaseObjectState1", null);
                    PropertyState propertyState = CreatePropertyState(baseObjectState, standardServerTest + "PropertyState", "PropertyState", null, BuiltInType.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

                    baseObjectState = CreateBaseObjectState(standardServerTestFolder, standardServerTest + "BaseObjectState2", "BaseObjectState2", null);
                    propertyState = CreatePropertyState(baseObjectState, standardServerTest + "PropertyState2", "PropertyState2", null, BuiltInType.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, true);

                    viewStateOperations = CreateViewState(standardServerTestFolder, externalReferences, views + "Operations 2", "Operations 2", null);

                    _ = CreateBaseDataVariableState(standardServerTestFolder, standardServerTest + "Double 1", "Double 1", null, BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

                    _ = CreateDataItemState(standardServerTestFolder, standardServerTest + "Double 1", "Double 1", null, BuiltInType.Double, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, String.Empty, 2, null, null);
                    _ = CreateDataItemState(standardServerTestFolder, standardServerTest + "Double 2", "Double 2", null, BuiltInType.Double, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, String.Empty, 2, null, null);

                    _ = CreateAnalogItemState(standardServerTestFolder, standardServerTest + "Boolean 1", "Boolean 1", null, BuiltInType.Boolean, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null, null);
                    _ = CreateAnalogItemState(standardServerTestFolder, standardServerTest + "Boolean 2", "Boolean 2", null, BuiltInType.Boolean, ValueRanks.TwoDimensions, AccessLevels.CurrentReadOrWrite, null, null);

                    _ = CreateTwoStateDiscreteState(standardServerTestFolder, standardServerTest + "005", "005", null, AccessLevels.CurrentReadOrWrite, false, "circle", "cross");

                    _ = CreateMultiStateDiscreteState(standardServerTestFolder, standardServerTest + "001", "001", null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, "open", "closed", "jammed");

                    _ = CreateMultiStateValueDiscreteState(standardServerTestFolder, daMultiStateValueDiscrete + "001", "001", null, null, AccessLevels.CurrentReadOrWrite, null, AttributeWriteMask.None, AttributeWriteMask.None, null, null, null, new LocalizedText[] { "open", "closed", "jammed" });

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
                    _ = simulationTimer_.Change(100, simulationInterval_);
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
                    _ = simulationTimer_.Change(100, simulationInterval_);
                }
                else
                {
                    _ = simulationTimer_.Change(100, 0);
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
            BaseDataVariableState variable = CreateBaseDataVariableState(parent, path, name, null, BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);

            if (peers != null)
            {
                foreach (NodeState peer in peers)
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
        private AnalogItemState CreateAnalogItemVariable(NodeState parent, string browseName, string name, string description, BuiltInType dataType, int valueRank, object initialValues = null, Opc.Ua.Range customRange = null)
        {
            return CreateAnalogItemVariable(parent, browseName, name, description, (uint)dataType, valueRank, initialValues, customRange);
        }

        private AnalogItemState CreateAnalogItemVariable(NodeState parent, string browseName, string name, string description, NodeId dataType, int valueRank, object initialValues = null, Opc.Ua.Range customRange = null)
        {
            var displayName = new LocalizedText("", name);

            BuiltInType builtInType = TypeInfo.GetBuiltInType(dataType, ServerData.TypeTree);

            // Simulate a mV Voltmeter

            Opc.Ua.Range newRange = GetAnalogRange(builtInType);
            // Using anything but 120,-10 fails a few tests
            newRange.High = Math.Min(newRange.High, 120);
            newRange.Low = Math.Max(newRange.Low, -10);

            var engineeringUnits = new EUInformation("mV", "millivolt", "http://www.opcfoundation.org/UA/units/un/cefact");
            // The mapping of the UNECE codes to OPC UA(EUInformation.unitId) is available here:
            // http://www.opcfoundation.org/UA/EngineeringUnits/UNECE/UNECE_to_OPCUA.csv
            engineeringUnits.UnitId = 12890; // "2Z"

            AnalogItemState variable = CreateAnalogItemState(parent, browseName, displayName, description, dataType, valueRank, AccessLevels.CurrentReadOrWrite, initialValues, customRange, engineeringUnits, newRange);

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
            var typeInfo = TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

            if (typeInfo == null || typeInfo == TypeInfo.Unknown)
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
            var typeInfo = TypeInfo.Construct(value);

            if (!(node is MultiStateValueDiscreteState variable) ||
                typeInfo == null ||
                typeInfo == TypeInfo.Unknown ||
                !TypeInfo.IsNumericType(typeInfo.BuiltInType))
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
            var typeInfo = TypeInfo.IsInstanceOfDataType(
                value,
                variable.DataType,
                variable.ValueRank,
                context.NamespaceUris,
                context.TypeTable);

            if (typeInfo == null || typeInfo == TypeInfo.Unknown)
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
            var typeInfo = TypeInfo.Construct(value);

            if (!(node is PropertyState<Opc.Ua.Range> variable) ||
                !(value is ExtensionObject extensionObject) ||
                typeInfo == null ||
                typeInfo == TypeInfo.Unknown)
            {
                return StatusCodes.BadTypeMismatch;
            }
            if (!(extensionObject.Body is Opc.Ua.Range newRange) ||
                !(variable.Parent is AnalogItemState parent))
            {
                return StatusCodes.BadTypeMismatch;
            }

            if (indexRange != NumericRange.Empty)
            {
                return StatusCodes.BadIndexRangeInvalid;
            }

            var parentTypeInfo = TypeInfo.Construct(parent.Value);

            Opc.Ua.Range parentRange = GetAnalogRange(parentTypeInfo.BuiltInType);
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
            FolderState newParentFolder = CreateFolderState(parent, path, name, null);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                var newName = string.Format("{0}_{1}", name, i.ToString("00"));
                var newPath = string.Format("{0}_{1}", path, newName);
                itemsCreated.Add(CreateBaseDataVariableState(newParentFolder, newPath, newName, null, dataType, valueRank, AccessLevels.CurrentReadOrWrite, null));
            }
            return itemsCreated.ToArray();
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
            BaseDataVariableState variable = CreateBaseDataVariableState(parent, path, name, description, dataType, valueRank, accessLevel, null);
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
            FolderState newParentFolder = CreateFolderState(parent, path, name, null);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                var newName = string.Format("{0}_{1}", name, i.ToString("00"));
                var newPath = string.Format("{0}_{1}", path, newName);
                itemsCreated.Add(CreateDynamicVariable(newParentFolder, newPath, newName, description, dataType, valueRank));
            }//for i
            return itemsCreated.ToArray();
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
                outputArguments[0] = floatValue + uintValue;
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
                outputArguments[0] = op1 * op2;
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
                outputArguments[0] = op1 / (float)op2;
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
                outputArguments[0] = "hello " + op1;
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
                outputArguments[0] = "Output";
                return ServiceResult.Good;
            }
            catch
            {
                return StatusCodes.BadInvalidArgument;
            }
        }

        private void DoSimulation(object state)
        {
            try
            {
                lock (Lock)
                {
                    DateTime timeStamp = DateTime.UtcNow;
                    foreach (BaseDataVariableState variable in dynamicNodes_)
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

                if (!PredefinedNodes.TryGetValue(nodeId, out NodeState node))
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

        #region Private Helper Functions
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

        #region Private Fields
        // Track whether Dispose has been called.
        private bool disposed_;
        private readonly object lockDisposable_ = new object();

        private readonly SimulationServerConfiguration configuration_;

        private Timer simulationTimer_;
        private UInt16 simulationInterval_ = 1000;
        private bool simulationEnabled_ = true;
        private readonly List<BaseDataVariableState> dynamicNodes_;
        #endregion
    }
}
