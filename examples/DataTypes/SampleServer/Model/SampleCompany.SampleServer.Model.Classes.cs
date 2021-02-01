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
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace SampleCompany.SampleServer.Model
{
    #region MachineInfoState Class
    #if (!OPCUA_EXCLUDE_MachineInfoState)
    /// <summary>
    /// Stores an instance of the MachineInfoType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class MachineInfoState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public MachineInfoState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.MachineInfoType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAACsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL01vZGVs/////wRggAIB" +
           "AAAAAQAXAAAATWFjaGluZUluZm9UeXBlSW5zdGFuY2UBAQEAAQEBAAEAAAD/////BQAAABVgiQoCAAAA" +
           "AQALAAAATWFjaGluZU5hbWUBAQIAAC4ARAIAAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAwAAABN" +
           "YW51ZmFjdHVyZXIBAQMAAC4ARAMAAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAwAAABTZXJpYWxO" +
           "dW1iZXIBAQQAAC4ARAQAAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAsAAABJc1Byb2R1Y2luZwEB" +
           "BQAALgBEBQAAAAAB/////wMD/////wAAAAAVYIkKAgAAAAEADAAAAE1hY2hpbmVTdGF0ZQEBBgAALgBE" +
           "BgAAAAAH/////wMD/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<string> MachineName
        {
            get
            {
                return m_machineName;
            }

            set
            {
                if (!Object.ReferenceEquals(m_machineName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_machineName = value;
            }
        }

        /// <remarks />
        public PropertyState<string> Manufacturer
        {
            get
            {
                return m_manufacturer;
            }

            set
            {
                if (!Object.ReferenceEquals(m_manufacturer, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_manufacturer = value;
            }
        }

        /// <remarks />
        public PropertyState<string> SerialNumber
        {
            get
            {
                return m_serialNumber;
            }

            set
            {
                if (!Object.ReferenceEquals(m_serialNumber, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_serialNumber = value;
            }
        }

        /// <remarks />
        public PropertyState<bool> IsProducing
        {
            get
            {
                return m_isProducing;
            }

            set
            {
                if (!Object.ReferenceEquals(m_isProducing, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_isProducing = value;
            }
        }

        /// <remarks />
        public PropertyState<uint> MachineState
        {
            get
            {
                return m_machineState;
            }

            set
            {
                if (!Object.ReferenceEquals(m_machineState, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_machineState = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_machineName != null)
            {
                children.Add(m_machineName);
            }

            if (m_manufacturer != null)
            {
                children.Add(m_manufacturer);
            }

            if (m_serialNumber != null)
            {
                children.Add(m_serialNumber);
            }

            if (m_isProducing != null)
            {
                children.Add(m_isProducing);
            }

            if (m_machineState != null)
            {
                children.Add(m_machineState);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case SampleCompany.SampleServer.Model.BrowseNames.MachineName:
                {
                    if (createOrReplace)
                    {
                        if (MachineName == null)
                        {
                            if (replacement == null)
                            {
                                MachineName = new PropertyState<string>(this);
                            }
                            else
                            {
                                MachineName = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = MachineName;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.Manufacturer:
                {
                    if (createOrReplace)
                    {
                        if (Manufacturer == null)
                        {
                            if (replacement == null)
                            {
                                Manufacturer = new PropertyState<string>(this);
                            }
                            else
                            {
                                Manufacturer = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = Manufacturer;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.SerialNumber:
                {
                    if (createOrReplace)
                    {
                        if (SerialNumber == null)
                        {
                            if (replacement == null)
                            {
                                SerialNumber = new PropertyState<string>(this);
                            }
                            else
                            {
                                SerialNumber = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = SerialNumber;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.IsProducing:
                {
                    if (createOrReplace)
                    {
                        if (IsProducing == null)
                        {
                            if (replacement == null)
                            {
                                IsProducing = new PropertyState<bool>(this);
                            }
                            else
                            {
                                IsProducing = (PropertyState<bool>)replacement;
                            }
                        }
                    }

                    instance = IsProducing;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.MachineState:
                {
                    if (createOrReplace)
                    {
                        if (MachineState == null)
                        {
                            if (replacement == null)
                            {
                                MachineState = new PropertyState<uint>(this);
                            }
                            else
                            {
                                MachineState = (PropertyState<uint>)replacement;
                            }
                        }
                    }

                    instance = MachineState;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private PropertyState<string> m_machineName;
        private PropertyState<string> m_manufacturer;
        private PropertyState<string> m_serialNumber;
        private PropertyState<bool> m_isProducing;
        private PropertyState<uint> m_machineState;
        #endregion
    }
    #endif
    #endregion

    #region GenericControllerState Class
    #if (!OPCUA_EXCLUDE_GenericControllerState)
    /// <summary>
    /// Stores an instance of the GenericControllerType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GenericControllerState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public GenericControllerState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.GenericControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAACsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL01vZGVs/////wRggAIB" +
           "AAAAAQAdAAAAR2VuZXJpY0NvbnRyb2xsZXJUeXBlSW5zdGFuY2UBAQcAAQEHAAcAAAD/////AgAAABVg" +
           "iQoCAAAAAQAIAAAAU2V0UG9pbnQBAQgAAC8BAEAJCAAAAAAL/////wMD/////wEAAAAVYIkKAgAAAAAA" +
           "BwAAAEVVUmFuZ2UBAQwAAC4ARAwAAAABAHQD/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAE1lYXN1" +
           "cmVtZW50AQEOAAAvAQBACQ4AAAAAC/////8BAf////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQES" +
           "AAAuAEQSAAAAAQB0A/////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public AnalogItemState<double> SetPoint
        {
            get
            {
                return m_setPoint;
            }

            set
            {
                if (!Object.ReferenceEquals(m_setPoint, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_setPoint = value;
            }
        }

        /// <remarks />
        public AnalogItemState<double> Measurement
        {
            get
            {
                return m_measurement;
            }

            set
            {
                if (!Object.ReferenceEquals(m_measurement, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_measurement = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_setPoint != null)
            {
                children.Add(m_setPoint);
            }

            if (m_measurement != null)
            {
                children.Add(m_measurement);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case SampleCompany.SampleServer.Model.BrowseNames.SetPoint:
                {
                    if (createOrReplace)
                    {
                        if (SetPoint == null)
                        {
                            if (replacement == null)
                            {
                                SetPoint = new AnalogItemState<double>(this);
                            }
                            else
                            {
                                SetPoint = (AnalogItemState<double>)replacement;
                            }
                        }
                    }

                    instance = SetPoint;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.Measurement:
                {
                    if (createOrReplace)
                    {
                        if (Measurement == null)
                        {
                            if (replacement == null)
                            {
                                Measurement = new AnalogItemState<double>(this);
                            }
                            else
                            {
                                Measurement = (AnalogItemState<double>)replacement;
                            }
                        }
                    }

                    instance = Measurement;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private AnalogItemState<double> m_setPoint;
        private AnalogItemState<double> m_measurement;
        #endregion
    }
    #endif
    #endregion

    #region FlowControllerState Class
    #if (!OPCUA_EXCLUDE_FlowControllerState)
    /// <summary>
    /// Stores an instance of the FlowControllerType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class FlowControllerState : GenericControllerState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public FlowControllerState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.FlowControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAACsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL01vZGVs/////wRggAIB" +
           "AAAAAQAaAAAARmxvd0NvbnRyb2xsZXJUeXBlSW5zdGFuY2UBARQAAQEUABQAAAD/////AgAAABVgiQoC" +
           "AAAAAQAIAAAAU2V0UG9pbnQBARUAAC8BAEAJFQAAAAAL/////wMD/////wEAAAAVYIkKAgAAAAAABwAA" +
           "AEVVUmFuZ2UBARkAAC4ARBkAAAABAHQD/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAE1lYXN1cmVt" +
           "ZW50AQEbAAAvAQBACRsAAAAAC/////8BAf////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQEfAAAu" +
           "AEQfAAAAAQB0A/////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        #endregion
    }
    #endif
    #endregion

    #region LevelControllerState Class
    #if (!OPCUA_EXCLUDE_LevelControllerState)
    /// <summary>
    /// Stores an instance of the LevelControllerType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class LevelControllerState : GenericControllerState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public LevelControllerState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.LevelControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAACsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL01vZGVs/////wRggAIB" +
           "AAAAAQAbAAAATGV2ZWxDb250cm9sbGVyVHlwZUluc3RhbmNlAQEhAAEBIQAhAAAA/////wIAAAAVYIkK" +
           "AgAAAAEACAAAAFNldFBvaW50AQEiAAAvAQBACSIAAAAAC/////8DA/////8BAAAAFWCJCgIAAAAAAAcA" +
           "AABFVVJhbmdlAQEmAAAuAEQmAAAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAABAAsAAABNZWFzdXJl" +
           "bWVudAEBKAAALwEAQAkoAAAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBLAAA" +
           "LgBELAAAAAEAdAP/////AQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        #endregion
    }
    #endif
    #endregion

    #region TemperatureControllerState Class
    #if (!OPCUA_EXCLUDE_TemperatureControllerState)
    /// <summary>
    /// Stores an instance of the TemperatureControllerType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class TemperatureControllerState : GenericControllerState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public TemperatureControllerState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.TemperatureControllerType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAACsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL01vZGVs/////wRggAIB" +
           "AAAAAQAhAAAAVGVtcGVyYXR1cmVDb250cm9sbGVyVHlwZUluc3RhbmNlAQEuAAEBLgAuAAAA/////wIA" +
           "AAAVYIkKAgAAAAEACAAAAFNldFBvaW50AQEvAAAvAQBACS8AAAAAC/////8DA/////8BAAAAFWCJCgIA" +
           "AAAAAAcAAABFVVJhbmdlAQEzAAAuAEQzAAAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAABAAsAAABN" +
           "ZWFzdXJlbWVudAEBNQAALwEAQAk1AAAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5n" +
           "ZQEBOQAALgBEOQAAAAEAdAP/////AQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        #endregion
    }
    #endif
    #endregion

    #region MachineState Class
    #if (!OPCUA_EXCLUDE_MachineState)
    /// <summary>
    /// Stores an instance of the MachineType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class MachineState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public MachineState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.MachineType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAACsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL01vZGVs/////wRggAIB" +
           "AAAAAQATAAAATWFjaGluZVR5cGVJbnN0YW5jZQEBOwABATsAOwAAAP////8EAAAABGCACgEAAAABAAsA" +
           "AABNYWNoaW5lSW5mbwEBPAAALwEBAQA8AAAA/////wUAAAAVYIkKAgAAAAEACwAAAE1hY2hpbmVOYW1l" +
           "AQE9AAAuAEQ9AAAAAAz/////AQH/////AAAAABVgiQoCAAAAAQAMAAAATWFudWZhY3R1cmVyAQE+AAAu" +
           "AEQ+AAAAAAz/////AQH/////AAAAABVgiQoCAAAAAQAMAAAAU2VyaWFsTnVtYmVyAQE/AAAuAEQ/AAAA" +
           "AAz/////AQH/////AAAAABVgiQoCAAAAAQALAAAASXNQcm9kdWNpbmcBAUAAAC4AREAAAAAAAf////8D" +
           "A/////8AAAAAFWCJCgIAAAABAAwAAABNYWNoaW5lU3RhdGUBAUEAAC4AREEAAAAAB/////8DA/////8A" +
           "AAAABGCACgEAAAABAAsAAABUZW1wZXJhdHVyZQEBQgAALwEBLgBCAAAA/////wIAAAAVYIkKAgAAAAEA" +
           "CAAAAFNldFBvaW50AQFDAAAvAQBACUMAAAAAC/////8DA/////8BAAAAFWCJCgIAAAAAAAcAAABFVVJh" +
           "bmdlAQFHAAAuAERHAAAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAABAAsAAABNZWFzdXJlbWVudAEB" +
           "SQAALwEAQAlJAAAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBTQAALgBETQAA" +
           "AAEAdAP/////AQH/////AAAAAARggAoBAAAAAQAEAAAARmxvdwEBTwAALwEBFABPAAAA/////wIAAAAV" +
           "YIkKAgAAAAEACAAAAFNldFBvaW50AQFQAAAvAQBACVAAAAAAC/////8DA/////8BAAAAFWCJCgIAAAAA" +
           "AAcAAABFVVJhbmdlAQFUAAAuAERUAAAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAABAAsAAABNZWFz" +
           "dXJlbWVudAEBVgAALwEAQAlWAAAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEB" +
           "WgAALgBEWgAAAAEAdAP/////AQH/////AAAAAARggAoBAAAAAQAFAAAATGV2ZWwBAVwAAC8BASEAXAAA" +
           "AP////8CAAAAFWCJCgIAAAABAAgAAABTZXRQb2ludAEBXQAALwEAQAldAAAAAAv/////AwP/////AQAA" +
           "ABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBYQAALgBEYQAAAAEAdAP/////AQH/////AAAAABVgiQoCAAAA" +
           "AQALAAAATWVhc3VyZW1lbnQBAWMAAC8BAEAJYwAAAAAL/////wEB/////wEAAAAVYIkKAgAAAAAABwAA" +
           "AEVVUmFuZ2UBAWcAAC4ARGcAAAABAHQD/////wEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public MachineInfoState MachineInfo
        {
            get
            {
                return m_machineInfo;
            }

            set
            {
                if (!Object.ReferenceEquals(m_machineInfo, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_machineInfo = value;
            }
        }

        /// <remarks />
        public TemperatureControllerState Temperature
        {
            get
            {
                return m_temperature;
            }

            set
            {
                if (!Object.ReferenceEquals(m_temperature, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_temperature = value;
            }
        }

        /// <remarks />
        public FlowControllerState Flow
        {
            get
            {
                return m_flow;
            }

            set
            {
                if (!Object.ReferenceEquals(m_flow, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_flow = value;
            }
        }

        /// <remarks />
        public LevelControllerState Level
        {
            get
            {
                return m_level;
            }

            set
            {
                if (!Object.ReferenceEquals(m_level, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_level = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_machineInfo != null)
            {
                children.Add(m_machineInfo);
            }

            if (m_temperature != null)
            {
                children.Add(m_temperature);
            }

            if (m_flow != null)
            {
                children.Add(m_flow);
            }

            if (m_level != null)
            {
                children.Add(m_level);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case SampleCompany.SampleServer.Model.BrowseNames.MachineInfo:
                {
                    if (createOrReplace)
                    {
                        if (MachineInfo == null)
                        {
                            if (replacement == null)
                            {
                                MachineInfo = new MachineInfoState(this);
                            }
                            else
                            {
                                MachineInfo = (MachineInfoState)replacement;
                            }
                        }
                    }

                    instance = MachineInfo;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.Temperature:
                {
                    if (createOrReplace)
                    {
                        if (Temperature == null)
                        {
                            if (replacement == null)
                            {
                                Temperature = new TemperatureControllerState(this);
                            }
                            else
                            {
                                Temperature = (TemperatureControllerState)replacement;
                            }
                        }
                    }

                    instance = Temperature;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.Flow:
                {
                    if (createOrReplace)
                    {
                        if (Flow == null)
                        {
                            if (replacement == null)
                            {
                                Flow = new FlowControllerState(this);
                            }
                            else
                            {
                                Flow = (FlowControllerState)replacement;
                            }
                        }
                    }

                    instance = Flow;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.Level:
                {
                    if (createOrReplace)
                    {
                        if (Level == null)
                        {
                            if (replacement == null)
                            {
                                Level = new LevelControllerState(this);
                            }
                            else
                            {
                                Level = (LevelControllerState)replacement;
                            }
                        }
                    }

                    instance = Level;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private MachineInfoState m_machineInfo;
        private TemperatureControllerState m_temperature;
        private FlowControllerState m_flow;
        private LevelControllerState m_level;
        #endregion
    }
    #endif
    #endregion
}