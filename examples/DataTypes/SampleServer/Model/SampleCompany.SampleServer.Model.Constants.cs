/* ========================================================================
 * Copyright (c) 2005-2019 The OPC Foundation, Inc. All rights reserved.
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
    #region Object Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <summary>
        /// The identifier for the MachineType_MachineInfo Object.
        /// </summary>
        public const uint MachineType_MachineInfo = 60;

        /// <summary>
        /// The identifier for the MachineType_Temperature Object.
        /// </summary>
        public const uint MachineType_Temperature = 66;

        /// <summary>
        /// The identifier for the MachineType_Flow Object.
        /// </summary>
        public const uint MachineType_Flow = 79;

        /// <summary>
        /// The identifier for the MachineType_Level Object.
        /// </summary>
        public const uint MachineType_Level = 92;
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
        /// The identifier for the MachineInfoType ObjectType.
        /// </summary>
        public const uint MachineInfoType = 1;

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
        /// The identifier for the MachineInfoType_MachineName Variable.
        /// </summary>
        public const uint MachineInfoType_MachineName = 2;

        /// <summary>
        /// The identifier for the MachineInfoType_Manufacturer Variable.
        /// </summary>
        public const uint MachineInfoType_Manufacturer = 3;

        /// <summary>
        /// The identifier for the MachineInfoType_SerialNumber Variable.
        /// </summary>
        public const uint MachineInfoType_SerialNumber = 4;

        /// <summary>
        /// The identifier for the MachineInfoType_IsProducing Variable.
        /// </summary>
        public const uint MachineInfoType_IsProducing = 5;

        /// <summary>
        /// The identifier for the MachineInfoType_MachineState Variable.
        /// </summary>
        public const uint MachineInfoType_MachineState = 6;

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
        /// The identifier for the MachineType_MachineInfo_MachineName Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_MachineName = 61;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_Manufacturer Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_Manufacturer = 62;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_SerialNumber Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_SerialNumber = 63;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_IsProducing Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_IsProducing = 64;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_MachineState Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_MachineState = 65;

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Temperature_SetPoint = 67;

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Temperature_SetPoint_EURange = 71;

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement Variable.
        /// </summary>
        public const uint MachineType_Temperature_Measurement = 73;

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Temperature_Measurement_EURange = 77;

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Flow_SetPoint = 80;

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Flow_SetPoint_EURange = 84;

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement Variable.
        /// </summary>
        public const uint MachineType_Flow_Measurement = 86;

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Flow_Measurement_EURange = 90;

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Level_SetPoint = 93;

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Level_SetPoint_EURange = 97;

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement Variable.
        /// </summary>
        public const uint MachineType_Level_Measurement = 99;

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Level_Measurement_EURange = 103;
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
        /// The identifier for the MachineType_MachineInfo Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.MachineType_MachineInfo, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

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
        /// The identifier for the MachineInfoType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.MachineInfoType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

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
        /// The identifier for the MachineInfoType_MachineName Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_MachineName = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineInfoType_MachineName, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineInfoType_Manufacturer Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_Manufacturer = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineInfoType_Manufacturer, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineInfoType_SerialNumber Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_SerialNumber = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineInfoType_SerialNumber, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineInfoType_IsProducing Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_IsProducing = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineInfoType_IsProducing, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineInfoType_MachineState Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_MachineState = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineInfoType_MachineState, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

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
        /// The identifier for the MachineType_MachineInfo_MachineName Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_MachineName = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_MachineInfo_MachineName, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_Manufacturer Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_Manufacturer = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_MachineInfo_Manufacturer, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_SerialNumber Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_SerialNumber = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_MachineInfo_SerialNumber, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_IsProducing Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_IsProducing = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_MachineInfo_IsProducing, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_MachineState Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_MachineState = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.MachineType_MachineInfo_MachineState, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

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
        /// The BrowseName for the IsProducing component.
        /// </summary>
        public const string IsProducing = "IsProducing";

        /// <summary>
        /// The BrowseName for the Level component.
        /// </summary>
        public const string Level = "Level";

        /// <summary>
        /// The BrowseName for the LevelControllerType component.
        /// </summary>
        public const string LevelControllerType = "LevelControllerType";

        /// <summary>
        /// The BrowseName for the MachineInfo component.
        /// </summary>
        public const string MachineInfo = "MachineInfo";

        /// <summary>
        /// The BrowseName for the MachineInfoType component.
        /// </summary>
        public const string MachineInfoType = "MachineInfoType";

        /// <summary>
        /// The BrowseName for the MachineName component.
        /// </summary>
        public const string MachineName = "MachineName";

        /// <summary>
        /// The BrowseName for the MachineState component.
        /// </summary>
        public const string MachineState = "MachineState";

        /// <summary>
        /// The BrowseName for the MachineType component.
        /// </summary>
        public const string MachineType = "MachineType";

        /// <summary>
        /// The BrowseName for the Manufacturer component.
        /// </summary>
        public const string Manufacturer = "Manufacturer";

        /// <summary>
        /// The BrowseName for the Measurement component.
        /// </summary>
        public const string Measurement = "Measurement";

        /// <summary>
        /// The BrowseName for the SerialNumber component.
        /// </summary>
        public const string SerialNumber = "SerialNumber";

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