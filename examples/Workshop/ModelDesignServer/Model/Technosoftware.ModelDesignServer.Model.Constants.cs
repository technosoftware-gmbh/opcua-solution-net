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
using Technosoftware.ModelDesignServer.Engineering;
using Technosoftware.ModelDesignServer.Operations;

namespace Technosoftware.ModelDesignServer.Model
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
        public const uint MachineType_MachineInfo = 15098;

        /// <summary>
        /// The identifier for the MachineType_Temperature Object.
        /// </summary>
        public const uint MachineType_Temperature = 15065;

        /// <summary>
        /// The identifier for the MachineType_Flow Object.
        /// </summary>
        public const uint MachineType_Flow = 15078;

        /// <summary>
        /// The identifier for the MachineType_Level Object.
        /// </summary>
        public const uint MachineType_Level = 15091;
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
        public const uint MachineInfoType = 15162;

        /// <summary>
        /// The identifier for the GenericControllerType ObjectType.
        /// </summary>
        public const uint GenericControllerType = 15003;

        /// <summary>
        /// The identifier for the FlowControllerType ObjectType.
        /// </summary>
        public const uint FlowControllerType = 15018;

        /// <summary>
        /// The identifier for the LevelControllerType ObjectType.
        /// </summary>
        public const uint LevelControllerType = 15033;

        /// <summary>
        /// The identifier for the TemperatureControllerType ObjectType.
        /// </summary>
        public const uint TemperatureControllerType = 15050;

        /// <summary>
        /// The identifier for the MachineType ObjectType.
        /// </summary>
        public const uint MachineType = 15097;
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
        public const uint MachineInfoType_MachineName = 15164;

        /// <summary>
        /// The identifier for the MachineInfoType_Manufacturer Variable.
        /// </summary>
        public const uint MachineInfoType_Manufacturer = 15048;

        /// <summary>
        /// The identifier for the MachineInfoType_SerialNumber Variable.
        /// </summary>
        public const uint MachineInfoType_SerialNumber = 15049;

        /// <summary>
        /// The identifier for the MachineInfoType_IsProducing Variable.
        /// </summary>
        public const uint MachineInfoType_IsProducing = 15166;

        /// <summary>
        /// The identifier for the MachineInfoType_MachineState Variable.
        /// </summary>
        public const uint MachineInfoType_MachineState = 15167;

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint Variable.
        /// </summary>
        public const uint GenericControllerType_SetPoint = 15006;

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint GenericControllerType_SetPoint_EURange = 15010;

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement Variable.
        /// </summary>
        public const uint GenericControllerType_Measurement = 15012;

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint GenericControllerType_Measurement_EURange = 15016;

        /// <summary>
        /// The identifier for the FlowControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint FlowControllerType_SetPoint_EURange = 15025;

        /// <summary>
        /// The identifier for the FlowControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint FlowControllerType_Measurement_EURange = 15031;

        /// <summary>
        /// The identifier for the LevelControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint LevelControllerType_SetPoint_EURange = 15040;

        /// <summary>
        /// The identifier for the LevelControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint LevelControllerType_Measurement_EURange = 15046;

        /// <summary>
        /// The identifier for the TemperatureControllerType_SetPoint_EURange Variable.
        /// </summary>
        public const uint TemperatureControllerType_SetPoint_EURange = 15055;

        /// <summary>
        /// The identifier for the TemperatureControllerType_Measurement_EURange Variable.
        /// </summary>
        public const uint TemperatureControllerType_Measurement_EURange = 15061;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_MachineName Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_MachineName = 15099;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_Manufacturer Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_Manufacturer = 15063;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_SerialNumber Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_SerialNumber = 15064;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_IsProducing Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_IsProducing = 15100;

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_MachineState Variable.
        /// </summary>
        public const uint MachineType_MachineInfo_MachineState = 15101;

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Temperature_SetPoint = 15066;

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Temperature_SetPoint_EURange = 15070;

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement Variable.
        /// </summary>
        public const uint MachineType_Temperature_Measurement = 15072;

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Temperature_Measurement_EURange = 15076;

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Flow_SetPoint = 15079;

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Flow_SetPoint_EURange = 15083;

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement Variable.
        /// </summary>
        public const uint MachineType_Flow_Measurement = 15085;

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Flow_Measurement_EURange = 15089;

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint Variable.
        /// </summary>
        public const uint MachineType_Level_SetPoint = 15092;

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint_EURange Variable.
        /// </summary>
        public const uint MachineType_Level_SetPoint_EURange = 15096;

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement Variable.
        /// </summary>
        public const uint MachineType_Level_Measurement = 15151;

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement_EURange Variable.
        /// </summary>
        public const uint MachineType_Level_Measurement_EURange = 15155;
    }
    #endregion

    #region View Identifiers
    /// <summary>
    /// A class that declares constants for all Views in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Views
    {
        /// <summary>
        /// The identifier for the Engineering View.
        /// </summary>
        public const uint Engineering = 15001;

        /// <summary>
        /// The identifier for the Operations View.
        /// </summary>
        public const uint Operations = 15002;
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
        public static readonly ExpandedNodeId MachineType_MachineInfo = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Objects.MachineType_MachineInfo, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Objects.MachineType_Temperature, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Flow Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Objects.MachineType_Flow, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Level Object.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Objects.MachineType_Level, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);
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
        public static readonly ExpandedNodeId MachineInfoType = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.ObjectTypes.MachineInfoType, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the GenericControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.ObjectTypes.GenericControllerType, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the FlowControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId FlowControllerType = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.ObjectTypes.FlowControllerType, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the LevelControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId LevelControllerType = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.ObjectTypes.LevelControllerType, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the TemperatureControllerType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId TemperatureControllerType = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.ObjectTypes.TemperatureControllerType, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId MachineType = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.ObjectTypes.MachineType, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);
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
        public static readonly ExpandedNodeId MachineInfoType_MachineName = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineInfoType_MachineName, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineInfoType_Manufacturer Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_Manufacturer = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineInfoType_Manufacturer, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineInfoType_SerialNumber Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_SerialNumber = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineInfoType_SerialNumber, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineInfoType_IsProducing Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_IsProducing = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineInfoType_IsProducing, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineInfoType_MachineState Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineInfoType_MachineState = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineInfoType_MachineState, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_SetPoint = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.GenericControllerType_SetPoint, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the GenericControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.GenericControllerType_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_Measurement = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.GenericControllerType_Measurement, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the GenericControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId GenericControllerType_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.GenericControllerType_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the FlowControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId FlowControllerType_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.FlowControllerType_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the FlowControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId FlowControllerType_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.FlowControllerType_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the LevelControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId LevelControllerType_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.LevelControllerType_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the LevelControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId LevelControllerType_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.LevelControllerType_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the TemperatureControllerType_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId TemperatureControllerType_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.TemperatureControllerType_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the TemperatureControllerType_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId TemperatureControllerType_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.TemperatureControllerType_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_MachineName Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_MachineName = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_MachineInfo_MachineName, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_Manufacturer Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_Manufacturer = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_MachineInfo_Manufacturer, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_SerialNumber Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_SerialNumber = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_MachineInfo_SerialNumber, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_IsProducing Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_IsProducing = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_MachineInfo_IsProducing, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_MachineInfo_MachineState Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_MachineInfo_MachineState = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_MachineInfo_MachineState, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_SetPoint = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Temperature_SetPoint, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Temperature_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_Measurement = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Temperature_Measurement, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Temperature_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Temperature_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Temperature_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_SetPoint = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Flow_SetPoint, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Flow_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_Measurement = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Flow_Measurement, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Flow_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Flow_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Flow_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_SetPoint = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Level_SetPoint, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Level_SetPoint_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_SetPoint_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Level_SetPoint_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_Measurement = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Level_Measurement, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the MachineType_Level_Measurement_EURange Variable.
        /// </summary>
        public static readonly ExpandedNodeId MachineType_Level_Measurement_EURange = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Variables.MachineType_Level_Measurement_EURange, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);
    }
    #endregion

    #region View Node Identifiers
    /// <summary>
    /// A class that declares constants for all Views in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ViewIds
    {
        /// <summary>
        /// The identifier for the Engineering View.
        /// </summary>
        public static readonly ExpandedNodeId Engineering = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Views.Engineering, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);

        /// <summary>
        /// The identifier for the Operations View.
        /// </summary>
        public static readonly ExpandedNodeId Operations = new ExpandedNodeId(Technosoftware.ModelDesignServer.Model.Views.Operations, Technosoftware.ModelDesignServer.Model.Namespaces.ModelDesignServer);
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
        /// The BrowseName for the Engineering component.
        /// </summary>
        public const string Engineering = "Engineering";

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
        /// The BrowseName for the Operations component.
        /// </summary>
        public const string Operations = "Operations";

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
        /// The URI for the Engineering namespace (.NET code namespace is 'Technosoftware.ModelDesignServer.Engineering').
        /// </summary>
        public const string Engineering = "http://technosoftware.com/ModelDesignServer/Engineering";

        /// <summary>
        /// The URI for the Operations namespace (.NET code namespace is 'Technosoftware.ModelDesignServer.Operations').
        /// </summary>
        public const string Operations = "http://technosoftware.com/ModelDesignServer/Operations";

        /// <summary>
        /// The URI for the ModelDesignServer namespace (.NET code namespace is 'Technosoftware.ModelDesignServer.Model').
        /// </summary>
        public const string ModelDesignServer = "http://technosoftware.com/ModelDesignServer/Model";
    }
    #endregion
}