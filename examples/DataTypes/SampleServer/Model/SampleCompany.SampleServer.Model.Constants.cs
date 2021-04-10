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

namespace SampleCompany.SampleServer.Model
{
    #region DataType Identifiers
    /// <summary>
    /// A class that declares constants for all DataTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypes
    {
        /// <summary>
        /// The identifier for the MachineStateDataType DataType.
        /// </summary>
        public const uint MachineStateDataType = 1;

        /// <summary>
        /// The identifier for the MachineDataType DataType.
        /// </summary>
        public const uint MachineDataType = 3;
    }
    #endregion

    #region Object Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <summary>
        /// The identifier for the MachineType_Temperature Object.
        /// </summary>
        public const uint MachineType_Temperature = 60;

        /// <summary>
        /// The identifier for the MachineType_Flow Object.
        /// </summary>
        public const uint MachineType_Flow = 73;

        /// <summary>
        /// The identifier for the MachineType_Level Object.
        /// </summary>
        public const uint MachineType_Level = 86;

        /// <summary>
        /// The identifier for the MachineDataType_Encoding_DefaultBinary Object.
        /// </summary>
        public const uint MachineDataType_Encoding_DefaultBinary = 100;

        /// <summary>
        /// The identifier for the MachineDataType_Encoding_DefaultXml Object.
        /// </summary>
        public const uint MachineDataType_Encoding_DefaultXml = 108;

        /// <summary>
        /// The identifier for the MachineDataType_Encoding_DefaultJson Object.
        /// </summary>
        public const uint MachineDataType_Encoding_DefaultJson = 116;
    }
    #endregion

    #region ObjectType Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <summary>
        /// The identifier for the GenericControllerType ObjectType.
        /// </summary>
        public const uint GenericControllerType = 7;

        /// <summary>
        /// The identifier for the FlowControllerType ObjectType.
        /// </summary>
        public const uint FlowControllerType = 20;

        /// <summary>
        /// The identifier for the LevelControllerType ObjectType.
        /// </summary>
        public const uint LevelControllerType = 33;

        /// <summary>
        /// The identifier for the TemperatureControllerType ObjectType.
        /// </summary>
        public const uint TemperatureControllerType = 46;

        /// <summary>
        /// The identifier for the MachineType ObjectType.
        /// </summary>
        public const uint MachineType = 59;
    }
    #endregion

    #region Variable Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <summary>
        /// The identifier for the MachineStateDataType_EnumStrings Variable.
        /// </summary>
        public const uint MachineStateDataType_EnumStrings = 2;

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint Variable.
        /// </summary>
        public const uint GenericControllerType_SetPoint = 8;

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint GenericControllerType_SetPoint_EURange = 12;

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement Variable.
        /// </summary>
        public const uint GenericControllerType_Measurement = 14;

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint GenericControllerType_Measurement_EURange = 18;

        /// <summary>
        /// The identifier for the FlowControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint FlowControllerType_SetPoint_EURange = 25;

        /// <summary>
        /// The identifier for the FlowControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint FlowControllerType_Measurement_EURange = 31;

        /// <summary>
        /// The identifier for the LevelControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint LevelControllerType_SetPoint_EURange = 38;

        /// <summary>
        /// The identifier for the LevelControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint LevelControllerType_Measurement_EURange = 44;

        /// <summary>
        /// The identifier for the TemperatureControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint TemperatureControllerType_SetPoint_EURange = 51;

        /// <summary>
        /// The identifier for the TemperatureControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint TemperatureControllerType_Measurement_EURange = 57;

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Temperature_SetPoint = 61;

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Temperature_SetPoint_EURange = 65;

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement Variable.
        /// </summary>
        public const uint MachineType_Temperature_Measurement = 67;

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Temperature_Measurement_EURange = 71;

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Flow_SetPoint = 74;

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Flow_SetPoint_EURange = 78;

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement Variable.
        /// </summary>
        public const uint MachineType_Flow_Measurement = 80;

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Flow_Measurement_EURange = 84;

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Level_SetPoint = 87;

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Level_SetPoint_EURange = 91;

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement Variable.
        /// </summary>
        public const uint MachineType_Level_Measurement = 93;

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Level_Measurement_EURange = 97;

        /// <summary>
        /// The identifier for the MachineType_MachineData Variable.
        /// </summary>
        public const uint MachineType_MachineData = 99;

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema Variable.
        /// </summary>
        public const uint SampleServer_BinarySchema = 101;

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema_NamespaceUri Variable.
        /// </summary>
        public const uint SampleServer_BinarySchema_NamespaceUri = 103;

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema_Deprecated Variable.
        /// </summary>
        public const uint SampleServer_BinarySchema_Deprecated = 104;

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema_MachineDataType Variable.
        /// </summary>
        public const uint SampleServer_BinarySchema_MachineDataType = 105;

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema Variable.
        /// </summary>
        public const uint SampleServer_XmlSchema = 109;

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema_NamespaceUri Variable.
        /// </summary>
        public const uint SampleServer_XmlSchema_NamespaceUri = 111;

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema_Deprecated Variable.
        /// </summary>
        public const uint SampleServer_XmlSchema_Deprecated = 112;

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema_MachineDataType Variable.
        /// </summary>
        public const uint SampleServer_XmlSchema_MachineDataType = 113;
    }
    #endregion

    #region DataType Node Identifiers
    /// <summary>
    /// A class that declares constants for all DataTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypeIds
    {
        /// <summary>
        /// The identifier for the MachineStateDataType DataType.
        /// </summary>
        public static readonly ExpandedNodeId MachineStateDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.DataTypes.MachineStateDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineDataType DataType.
        /// </summary>
        public static readonly ExpandedNodeId MachineDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.DataTypes.MachineDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region Object Node Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <summary>
        /// The identifier for the MachineType_Temperature Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineType_Temperature, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Flow Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineType_Flow, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Level Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineType_Level, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineDataType_Encoding_DefaultBinary Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineDataType_Encoding_DefaultBinary, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineDataType_Encoding_DefaultXml Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineDataType_Encoding_DefaultXml, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineDataType_Encoding_DefaultJson Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineDataType_Encoding_DefaultJson, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <summary>
        /// The identifier for the GenericControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.GenericControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the FlowControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId FlowControllerType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.FlowControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the LevelControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId LevelControllerType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.LevelControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the TemperatureControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId TemperatureControllerType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.TemperatureControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId MachineType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.MachineType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region Variable Node Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <summary>
        /// The identifier for the MachineStateDataType_EnumStrings Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineStateDataType_EnumStrings = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineStateDataType_EnumStrings, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_SetPoint = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.GenericControllerType_SetPoint, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.GenericControllerType_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_Measurement = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.GenericControllerType_Measurement, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.GenericControllerType_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the FlowControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId FlowControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.FlowControllerType_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the FlowControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId FlowControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.FlowControllerType_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the LevelControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId LevelControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.LevelControllerType_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the LevelControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId LevelControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.LevelControllerType_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the TemperatureControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId TemperatureControllerType_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.TemperatureControllerType_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the TemperatureControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId TemperatureControllerType_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.TemperatureControllerType_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_SetPoint = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Temperature_SetPoint, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Temperature_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_Measurement = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Temperature_Measurement, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Temperature_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_SetPoint = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Flow_SetPoint, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Flow_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_Measurement = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Flow_Measurement, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Flow_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_SetPoint = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Level_SetPoint, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_SetPoint_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Level_SetPoint_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_Measurement = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Level_Measurement, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_Measurement_EURange = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_Level_Measurement_EURange, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_MachineData Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineData = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_MachineData, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_BinarySchema = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema_NamespaceUri Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_BinarySchema_NamespaceUri = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema_NamespaceUri, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema_Deprecated Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_BinarySchema_Deprecated = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema_Deprecated, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_BinarySchema_MachineDataType Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_BinarySchema_MachineDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema_MachineDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_XmlSchema = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema_NamespaceUri Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_XmlSchema_NamespaceUri = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema_NamespaceUri, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema_Deprecated Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_XmlSchema_Deprecated = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema_Deprecated, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the SampleServer_XmlSchema_MachineDataType Variable.
        /// </summary>
        public static readonly ExpandedNodeId SampleServer_XmlSchema_MachineDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema_MachineDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region BrowseName Declarations
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class BrowseNames
    {
        /// <summary>
        /// The BrowseName for the Flow component.
        /// </summary>
        public const string Flow = "Flow";

        /// <summary>
        /// The BrowseName for the FlowControllerType component.
        /// </summary>
        public const string FlowControllerType = "FlowControllerType";

        /// <summary>
        /// The BrowseName for the GenericControllerType component.
        /// </summary>
        public const string GenericControllerType = "GenericControllerType";

        /// <summary>
        /// The BrowseName for the Level component.
        /// </summary>
        public const string Level = "Level";

        /// <summary>
        /// The BrowseName for the LevelControllerType component.
        /// </summary>
        public const string LevelControllerType = "LevelControllerType";

        /// <summary>
        /// The BrowseName for the MachineData component.
        /// </summary>
        public const string MachineData = "MachineData";

        /// <summary>
        /// The BrowseName for the MachineDataType component.
        /// </summary>
        public const string MachineDataType = "MachineDataType";

        /// <summary>
        /// The BrowseName for the MachineStateDataType component.
        /// </summary>
        public const string MachineStateDataType = "MachineStateDataType";

        /// <summary>
        /// The BrowseName for the MachineType component.
        /// </summary>
        public const string MachineType = "MachineType";

        /// <summary>
        /// The BrowseName for the Measurement component.
        /// </summary>
        public const string Measurement = "Measurement";

        /// <summary>
        /// The BrowseName for the SampleServer_BinarySchema component.
        /// </summary>
        public const string SampleServer_BinarySchema = "SampleCompany.SampleServer.Model";

        /// <summary>
        /// The BrowseName for the SampleServer_XmlSchema component.
        /// </summary>
        public const string SampleServer_XmlSchema = "SampleCompany.SampleServer.Model";

        /// <summary>
        /// The BrowseName for the SetPoint component.
        /// </summary>
        public const string SetPoint = "SetPoint";

        /// <summary>
        /// The BrowseName for the Temperature component.
        /// </summary>
        public const string Temperature = "Temperature";

        /// <summary>
        /// The BrowseName for the TemperatureControllerType component.
        /// </summary>
        public const string TemperatureControllerType = "TemperatureControllerType";
    }
    #endregion

    #region Namespace Declarations
    /// <summary>
    /// Defines constants for all namespaces referenced by the model design.
    /// </summary>
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
        /// The URI for the SampleServer namespace (.NET code namespace is 'SampleCompany.SampleServer.Model').
        /// </summary>
        public const string SampleServer = "http://samplecompany.com/SampleServer/Model";
    }
    #endregion
}