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

namespace SampleCompany.NodeManagers.SampleDataTypes
{
    #region DataType Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypes
    {
        /// <remarks />
        public const uint MachineStateDataType = 1;

        /// <remarks />
        public const uint MachineDataType = 3;
    }
    #endregion

    #region Object Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <remarks />
        public const uint MachineType_Temperature = 60;

        /// <remarks />
        public const uint MachineType_Flow = 73;

        /// <remarks />
        public const uint MachineType_Level = 86;

        /// <remarks />
        public const uint MachineDataType_Encoding_DefaultBinary = 100;

        /// <remarks />
        public const uint MachineDataType_Encoding_DefaultXml = 108;

        /// <remarks />
        public const uint MachineDataType_Encoding_DefaultJson = 116;
    }
    #endregion

    #region ObjectType Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <remarks />
        public const uint GenericControllerType = 7;

        /// <remarks />
        public const uint FlowControllerType = 20;

        /// <remarks />
        public const uint LevelControllerType = 33;

        /// <remarks />
        public const uint TemperatureControllerType = 46;

        /// <remarks />
        public const uint MachineType = 59;
    }
    #endregion

    #region Variable Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <remarks />
        public const uint MachineStateDataType_EnumStrings = 2;

        /// <remarks />
        public const uint GenericControllerType_SetPoint = 8;

        /// <remarks />
        public const uint GenericControllerType_SetPoint_EURange = 12;

        /// <remarks />
        public const uint GenericControllerType_Measurement = 14;

        /// <remarks />
        public const uint GenericControllerType_Measurement_EURange = 18;

        /// <remarks />
        public const uint FlowControllerType_SetPoint_EURange = 25;

        /// <remarks />
        public const uint FlowControllerType_Measurement_EURange = 31;

        /// <remarks />
        public const uint LevelControllerType_SetPoint_EURange = 38;

        /// <remarks />
        public const uint LevelControllerType_Measurement_EURange = 44;

        /// <remarks />
        public const uint TemperatureControllerType_SetPoint_EURange = 51;

        /// <remarks />
        public const uint TemperatureControllerType_Measurement_EURange = 57;

        /// <remarks />
        public const uint MachineType_Temperature_SetPoint = 61;

        /// <remarks />
        public const uint MachineType_Temperature_SetPoint_EURange = 65;

        /// <remarks />
        public const uint MachineType_Temperature_Measurement = 67;

        /// <remarks />
        public const uint MachineType_Temperature_Measurement_EURange = 71;

        /// <remarks />
        public const uint MachineType_Flow_SetPoint = 74;

        /// <remarks />
        public const uint MachineType_Flow_SetPoint_EURange = 78;

        /// <remarks />
        public const uint MachineType_Flow_Measurement = 80;

        /// <remarks />
        public const uint MachineType_Flow_Measurement_EURange = 84;

        /// <remarks />
        public const uint MachineType_Level_SetPoint = 87;

        /// <remarks />
        public const uint MachineType_Level_SetPoint_EURange = 91;

        /// <remarks />
        public const uint MachineType_Level_Measurement = 93;

        /// <remarks />
        public const uint MachineType_Level_Measurement_EURange = 97;

        /// <remarks />
        public const uint MachineType_MachineData = 99;

        /// <remarks />
        public const uint SampleDataTypes_BinarySchema = 117;

        /// <remarks />
        public const uint SampleDataTypes_BinarySchema_NamespaceUri = 119;

        /// <remarks />
        public const uint SampleDataTypes_BinarySchema_Deprecated = 120;

        /// <remarks />
        public const uint SampleDataTypes_BinarySchema_MachineDataType = 121;

        /// <remarks />
        public const uint SampleDataTypes_XmlSchema = 124;

        /// <remarks />
        public const uint SampleDataTypes_XmlSchema_NamespaceUri = 126;

        /// <remarks />
        public const uint SampleDataTypes_XmlSchema_Deprecated = 127;

        /// <remarks />
        public const uint SampleDataTypes_XmlSchema_MachineDataType = 128;
    }
    #endregion

    #region DataType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId MachineStateDataType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.DataTypes.MachineStateDataType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineDataType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.DataTypes.MachineDataType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);
    }
    #endregion

    #region Object Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Temperature = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Objects.MachineType_Temperature, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Flow = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Objects.MachineType_Flow, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Level = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Objects.MachineType_Level, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Objects.MachineDataType_Encoding_DefaultBinary, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Objects.MachineDataType_Encoding_DefaultXml, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Objects.MachineDataType_Encoding_DefaultJson, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId GenericControllerType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.ObjectTypes.GenericControllerType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId FlowControllerType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.ObjectTypes.FlowControllerType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId LevelControllerType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.ObjectTypes.LevelControllerType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId TemperatureControllerType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.ObjectTypes.TemperatureControllerType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.ObjectTypes.MachineType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);
    }
    #endregion

    #region Variable Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId MachineStateDataType_EnumStrings = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineStateDataType_EnumStrings, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId GenericControllerType_SetPoint = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.GenericControllerType_SetPoint, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId GenericControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.GenericControllerType_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId GenericControllerType_Measurement = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.GenericControllerType_Measurement, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId GenericControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.GenericControllerType_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId FlowControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.FlowControllerType_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId FlowControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.FlowControllerType_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId LevelControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.LevelControllerType_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId LevelControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.LevelControllerType_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId TemperatureControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.TemperatureControllerType_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId TemperatureControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.TemperatureControllerType_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Temperature_SetPoint = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Temperature_SetPoint, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Temperature_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Temperature_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Temperature_Measurement = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Temperature_Measurement, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Temperature_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Temperature_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Flow_SetPoint = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Flow_SetPoint, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Flow_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Flow_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Flow_Measurement = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Flow_Measurement, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Flow_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Flow_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Level_SetPoint = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Level_SetPoint, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Level_SetPoint_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Level_SetPoint_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Level_Measurement = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Level_Measurement, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_Level_Measurement_EURange = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_Level_Measurement_EURange, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId MachineType_MachineData = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.MachineType_MachineData, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_BinarySchema = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_BinarySchema, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_BinarySchema_NamespaceUri = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_BinarySchema_NamespaceUri, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_BinarySchema_Deprecated = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_BinarySchema_Deprecated, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_BinarySchema_MachineDataType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_BinarySchema_MachineDataType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_XmlSchema = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_XmlSchema, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_XmlSchema_NamespaceUri = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_XmlSchema_NamespaceUri, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_XmlSchema_Deprecated = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_XmlSchema_Deprecated, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

        /// <remarks />
        public static readonly ExpandedNodeId SampleDataTypes_XmlSchema_MachineDataType = new ExpandedNodeId(SampleCompany.NodeManagers.SampleDataTypes.Variables.SampleDataTypes_XmlSchema_MachineDataType, SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);
    }
    #endregion

    #region BrowseName Declarations
    /// <remarks />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class BrowseNames
    {
        /// <remarks />
        public const string Flow = "Flow";

        /// <remarks />
        public const string FlowControllerType = "FlowControllerType";

        /// <remarks />
        public const string GenericControllerType = "GenericControllerType";

        /// <remarks />
        public const string Level = "Level";

        /// <remarks />
        public const string LevelControllerType = "LevelControllerType";

        /// <remarks />
        public const string MachineData = "MachineData";

        /// <remarks />
        public const string MachineDataType = "MachineDataType";

        /// <remarks />
        public const string MachineStateDataType = "MachineStateDataType";

        /// <remarks />
        public const string MachineType = "MachineType";

        /// <remarks />
        public const string Measurement = "Measurement";

        /// <remarks />
        public const string SampleDataTypes_BinarySchema = "SampleCompany.NodeManagers.SampleDataTypes";

        /// <remarks />
        public const string SampleDataTypes_XmlSchema = "SampleCompany.NodeManagers.SampleDataTypes";

        /// <remarks />
        public const string SetPoint = "SetPoint";

        /// <remarks />
        public const string Temperature = "Temperature";

        /// <remarks />
        public const string TemperatureControllerType = "TemperatureControllerType";
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
        /// The URI for the SampleDataTypes namespace (.NET code namespace is 'SampleCompany.NodeManagers.SampleDataTypes').
        /// </summary>
        public const string SampleDataTypes = "http://samplecompany.com/SampleServer/NodeManagers/SampleDataTypes";
    }
    #endregion
}