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
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypes
    {
        /// <remarks />
        public const uint CycleStepDataType = 1;
    }
    #endregion

    #region Object Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <remarks />
        public const uint CycleStepDataType_Encoding_DefaultBinary = 52;

        /// <remarks />
        public const uint CycleStepDataType_Encoding_DefaultXml = 60;

        /// <remarks />
        public const uint CycleStepDataType_Encoding_DefaultJson = 68;
    }
    #endregion

    #region ObjectType Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <remarks />
        public const uint SystemCycleStatusEventType = 2;

        /// <remarks />
        public const uint SystemCycleStartedEventType = 14;

        /// <remarks />
        public const uint SystemCycleAbortedEventType = 27;

        /// <remarks />
        public const uint SystemCycleFinishedEventType = 40;
    }
    #endregion

    #region Variable Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <remarks />
        public const uint SystemCycleStatusEventType_CycleId = 12;

        /// <remarks />
        public const uint SystemCycleStatusEventType_CurrentStep = 13;

        /// <remarks />
        public const uint SystemCycleStartedEventType_Steps = 26;

        /// <remarks />
        public const uint SystemCycleAbortedEventType_Error = 39;

        /// <remarks />
        public const uint SampleServer_BinarySchema = 53;

        /// <remarks />
        public const uint SampleServer_BinarySchema_NamespaceUri = 55;

        /// <remarks />
        public const uint SampleServer_BinarySchema_Deprecated = 56;

        /// <remarks />
        public const uint SampleServer_BinarySchema_CycleStepDataType = 57;

        /// <remarks />
        public const uint SampleServer_XmlSchema = 61;

        /// <remarks />
        public const uint SampleServer_XmlSchema_NamespaceUri = 63;

        /// <remarks />
        public const uint SampleServer_XmlSchema_Deprecated = 64;

        /// <remarks />
        public const uint SampleServer_XmlSchema_CycleStepDataType = 65;
    }
    #endregion

    #region DataType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class DataTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId CycleStepDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.DataTypes.CycleStepDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region Object Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId CycleStepDataType_Encoding_DefaultBinary = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.CycleStepDataType_Encoding_DefaultBinary, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId CycleStepDataType_Encoding_DefaultXml = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.CycleStepDataType_Encoding_DefaultXml, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId CycleStepDataType_Encoding_DefaultJson = new ExpandedNodeId(SampleCompany.SampleServer.Model.Objects.CycleStepDataType_Encoding_DefaultJson, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleStatusEventType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleStatusEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleStartedEventType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleStartedEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleAbortedEventType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleAbortedEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleFinishedEventType = new ExpandedNodeId(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleFinishedEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region Variable Node Identifiers
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleStatusEventType_CycleId = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SystemCycleStatusEventType_CycleId, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleStatusEventType_CurrentStep = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SystemCycleStatusEventType_CurrentStep, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleStartedEventType_Steps = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SystemCycleStartedEventType_Steps, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SystemCycleAbortedEventType_Error = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SystemCycleAbortedEventType_Error, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_BinarySchema = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_BinarySchema_NamespaceUri = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema_NamespaceUri, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_BinarySchema_Deprecated = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema_Deprecated, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_BinarySchema_CycleStepDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_BinarySchema_CycleStepDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_XmlSchema = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_XmlSchema_NamespaceUri = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema_NamespaceUri, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_XmlSchema_Deprecated = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema_Deprecated, SampleCompany.SampleServer.Model.Namespaces.SampleServer);

        /// <remarks />
        public static readonly ExpandedNodeId SampleServer_XmlSchema_CycleStepDataType = new ExpandedNodeId(SampleCompany.SampleServer.Model.Variables.SampleServer_XmlSchema_CycleStepDataType, SampleCompany.SampleServer.Model.Namespaces.SampleServer);
    }
    #endregion

    #region BrowseName Declarations
    /// <remarks />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class BrowseNames
    {
        /// <remarks />
        public const string CurrentStep = "CurrentStep";

        /// <remarks />
        public const string CycleId = "CycleId";

        /// <remarks />
        public const string CycleStepDataType = "CycleStepDataType";

        /// <remarks />
        public const string Error = "Error";

        /// <remarks />
        public const string SampleServer_BinarySchema = "SampleCompany.SampleServer.Model";

        /// <remarks />
        public const string SampleServer_XmlSchema = "SampleCompany.SampleServer.Model";

        /// <remarks />
        public const string Steps = "Steps";

        /// <remarks />
        public const string SystemCycleAbortedEventType = "SystemCycleAbortedEventType";

        /// <remarks />
        public const string SystemCycleFinishedEventType = "SystemCycleFinishedEventType";

        /// <remarks />
        public const string SystemCycleStartedEventType = "SystemCycleStartedEventType";

        /// <remarks />
        public const string SystemCycleStatusEventType = "SystemCycleStatusEventType";
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
        /// The URI for the SampleServer namespace (.NET code namespace is 'SampleCompany.SampleServer.Model').
        /// </summary>
        public const string SampleServer = "http://samplecompany.com/SampleServer/Model";
    }
    #endregion
}