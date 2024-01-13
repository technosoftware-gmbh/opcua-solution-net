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
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace SampleCompany.NodeManagers.TestData
{
    #region GenerateValuesMethodState Class
    #if (!OPCUA_EXCLUDE_GenerateValuesMethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GenerateValuesMethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public GenerateValuesMethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new GenerateValuesMethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAGAAAAEdlbmVyYXRlVmFsdWVzTWV0aG9kVHlwZQEBAQAALwEBAQAB" +
           "AAAAAQH/////AQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAQIAAC4ARAIAAACWAQAAAAEA" +
           "KgEBRgAAAAoAAABJdGVyYXRpb25zAAf/////AAAAAAMAAAAAJQAAAFRoZSBudW1iZXIgb2YgbmV3IHZh" +
           "bHVlcyB0byBnZW5lcmF0ZS4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public GenerateValuesMethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            uint iterations = (uint)_inputArguments[0];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    iterations);
            }

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult GenerateValuesMethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        uint iterations);
    #endif
    #endregion

    #region GenerateValuesEventState Class
    #if (!OPCUA_EXCLUDE_GenerateValuesEventState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class GenerateValuesEventState : BaseEventState
    {
        #region Constructors
        /// <remarks />
        public GenerateValuesEventState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.GenerateValuesEventType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAHwAAAEdlbmVyYXRlVmFsdWVzRXZlbnRUeXBlSW5zdGFuY2UBAQMA" +
           "AQEDAAMAAAD/////CgAAABVgiQoCAAAAAAAHAAAARXZlbnRJZAEBBAAALgBEBAAAAAAP/////wEB////" +
           "/wAAAAAVYIkKAgAAAAAACQAAAEV2ZW50VHlwZQEBBQAALgBEBQAAAAAR/////wEB/////wAAAAAVYIkK" +
           "AgAAAAAACgAAAFNvdXJjZU5vZGUBAQYAAC4ARAYAAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoA" +
           "AABTb3VyY2VOYW1lAQEHAAAuAEQHAAAAAAz/////AQH/////AAAAABVgiQoCAAAAAAAEAAAAVGltZQEB" +
           "CAAALgBECAAAAAEAJgH/////AQH/////AAAAABVgiQoCAAAAAAALAAAAUmVjZWl2ZVRpbWUBAQkAAC4A" +
           "RAkAAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAABwAAAE1lc3NhZ2UBAQsAAC4ARAsAAAAAFf//" +
           "//8BAf////8AAAAAFWCJCgIAAAAAAAgAAABTZXZlcml0eQEBDAAALgBEDAAAAAAF/////wEB/////wAA" +
           "AAAVYIkKAgAAAAEACgAAAEl0ZXJhdGlvbnMBAQ0AAC4ARA0AAAAAB/////8BAf////8AAAAAFWCJCgIA" +
           "AAABAA0AAABOZXdWYWx1ZUNvdW50AQEOAAAuAEQOAAAAAAf/////AQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<uint> Iterations
        {
            get
            {
                return m_iterations;
            }

            set
            {
                if (!Object.ReferenceEquals(m_iterations, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_iterations = value;
            }
        }

        /// <remarks />
        public PropertyState<uint> NewValueCount
        {
            get
            {
                return m_newValueCount;
            }

            set
            {
                if (!Object.ReferenceEquals(m_newValueCount, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_newValueCount = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_iterations != null)
            {
                children.Add(m_iterations);
            }

            if (m_newValueCount != null)
            {
                children.Add(m_newValueCount);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.Iterations:
                {
                    if (createOrReplace)
                    {
                        if (Iterations == null)
                        {
                            if (replacement == null)
                            {
                                Iterations = new PropertyState<uint>(this);
                            }
                            else
                            {
                                Iterations = (PropertyState<uint>)replacement;
                            }
                        }
                    }

                    instance = Iterations;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NewValueCount:
                {
                    if (createOrReplace)
                    {
                        if (NewValueCount == null)
                        {
                            if (replacement == null)
                            {
                                NewValueCount = new PropertyState<uint>(this);
                            }
                            else
                            {
                                NewValueCount = (PropertyState<uint>)replacement;
                            }
                        }
                    }

                    instance = NewValueCount;
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
        private PropertyState<uint> m_iterations;
        private PropertyState<uint> m_newValueCount;
        #endregion
    }
    #endif
    #endregion

    #region TestDataObjectState Class
    #if (!OPCUA_EXCLUDE_TestDataObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class TestDataObjectState : BaseObjectState
    {
        #region Constructors
        /// <remarks />
        public TestDataObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.TestDataObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAGgAAAFRlc3REYXRhT2JqZWN0VHlwZUluc3RhbmNlAQEPAAEBDwAP" +
           "AAAAAQAAAAAkAAEBEwADAAAANWCJCgIAAAABABAAAABTaW11bGF0aW9uQWN0aXZlAQEQAAMAAAAARwAA" +
           "AElmIHRydWUgdGhlIHNlcnZlciB3aWxsIHByb2R1Y2UgbmV3IHZhbHVlcyBmb3IgZWFjaCBtb25pdG9y" +
           "ZWQgdmFyaWFibGUuAC4ARBAAAAAAAf////8BAf////8AAAAABGGCCgQAAAABAA4AAABHZW5lcmF0ZVZh" +
           "bHVlcwEBEQAALwEBEQARAAAAAQH/////AQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBARIA" +
           "AC4ARBIAAACWAQAAAAEAKgEBRgAAAAoAAABJdGVyYXRpb25zAAf/////AAAAAAMAAAAAJQAAAFRoZSBu" +
           "dW1iZXIgb2YgbmV3IHZhbHVlcyB0byBnZW5lcmF0ZS4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAAARg" +
           "gAoBAAAAAQANAAAAQ3ljbGVDb21wbGV0ZQEBEwAALwEAQQsTAAAAAQAAAAAkAQEBDwAXAAAAFWCJCgIA" +
           "AAAAAAcAAABFdmVudElkAQEUAAAuAEQUAAAAAA//////AQH/////AAAAABVgiQoCAAAAAAAJAAAARXZl" +
           "bnRUeXBlAQEVAAAuAEQVAAAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNlTm9kZQEB" +
           "FgAALgBEFgAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJjZU5hbWUBARcAAC4ARBcA" +
           "AAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAQAAABUaW1lAQEYAAAuAEQYAAAAAQAmAf////8BAf//" +
           "//8AAAAAFWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEBGQAALgBEGQAAAAEAJgH/////AQH/////AAAA" +
           "ABVgiQoCAAAAAAAHAAAATWVzc2FnZQEBGwAALgBEGwAAAAAV/////wEB/////wAAAAAVYIkKAgAAAAAA" +
           "CAAAAFNldmVyaXR5AQEcAAAuAEQcAAAAAAX/////AQH/////AAAAABVgiQoCAAAAAAAQAAAAQ29uZGl0" +
           "aW9uQ2xhc3NJZAEBHQAALgBEHQAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAAEgAAAENvbmRpdGlv" +
           "bkNsYXNzTmFtZQEBHgAALgBEHgAAAAAV/////wEB/////wAAAAAVYIkKAgAAAAAADQAAAENvbmRpdGlv" +
           "bk5hbWUBASEAAC4ARCEAAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAgAAABCcmFuY2hJZAEBIgAA" +
           "LgBEIgAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAABgAAAFJldGFpbgEBIwAALgBEIwAAAAAB////" +
           "/wEB/////wAAAAAVYIkKAgAAAAAADAAAAEVuYWJsZWRTdGF0ZQEBJAAALwEAIyMkAAAAABX/////AQEC" +
           "AAAAAQAsIwABATgAAQAsIwABAUEAAQAAABVgiQoCAAAAAAACAAAASWQBASUAAC4ARCUAAAAAAf////8B" +
           "Af////8AAAAAFWCJCgIAAAAAAAcAAABRdWFsaXR5AQEtAAAvAQAqIy0AAAAAE/////8BAf////8BAAAA" +
           "FWCJCgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAS4AAC4ARC4AAAABACYB/////wEB/////wAAAAAV" +
           "YIkKAgAAAAAADAAAAExhc3RTZXZlcml0eQEBLwAALwEAKiMvAAAAAAX/////AQH/////AQAAABVgiQoC" +
           "AAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQEwAAAuAEQwAAAAAQAmAf////8BAf////8AAAAAFWCJCgIA" +
           "AAAAAAcAAABDb21tZW50AQExAAAvAQAqIzEAAAAAFf////8BAf////8BAAAAFWCJCgIAAAAAAA8AAABT" +
           "b3VyY2VUaW1lc3RhbXABATIAAC4ARDIAAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAADAAAAENs" +
           "aWVudFVzZXJJZAEBMwAALgBEMwAAAAAM/////wEB/////wAAAAAEYYIKBAAAAAAABwAAAERpc2FibGUB" +
           "ATQAAC8BAEQjNAAAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAABgAAAEVuYWJsZQEBNQAALwEA" +
           "QyM1AAAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAKAAAAQWRkQ29tbWVudAEBNgAALwEARSM2" +
           "AAAAAQEBAAAAAQD5CwABAA0LAQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBATcAAC4ARDcA" +
           "AACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlmaWVy" +
           "IGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAAAAADAAAA" +
           "ACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAABVgiQoCAAAAAAAKAAAAQWNrZWRTdGF0ZQEBOAAALwEAIyM4AAAAABX/////AQEBAAAAAQAs" +
           "IwEBASQAAQAAABVgiQoCAAAAAAACAAAASWQBATkAAC4ARDkAAAAAAf////8BAf////8AAAAABGGCCgQA" +
           "AAAAAAsAAABBY2tub3dsZWRnZQEBSgAALwEAlyNKAAAAAQEBAAAAAQD5CwABAPAiAQAAABdgqQoCAAAA" +
           "AAAOAAAASW5wdXRBcmd1bWVudHMBAUsAAC4AREsAAACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElkAA//" +
           "////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4BACoB" +
           "AUIAAAAHAAAAQ29tbWVudAAV/////wAAAAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8gdGhl" +
           "IGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<bool> SimulationActive
        {
            get
            {
                return m_simulationActive;
            }

            set
            {
                if (!Object.ReferenceEquals(m_simulationActive, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_simulationActive = value;
            }
        }

        /// <remarks />
        public GenerateValuesMethodState GenerateValues
        {
            get
            {
                return m_generateValuesMethod;
            }

            set
            {
                if (!Object.ReferenceEquals(m_generateValuesMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_generateValuesMethod = value;
            }
        }

        /// <remarks />
        public AcknowledgeableConditionState CycleComplete
        {
            get
            {
                return m_cycleComplete;
            }

            set
            {
                if (!Object.ReferenceEquals(m_cycleComplete, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_cycleComplete = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_simulationActive != null)
            {
                children.Add(m_simulationActive);
            }

            if (m_generateValuesMethod != null)
            {
                children.Add(m_generateValuesMethod);
            }

            if (m_cycleComplete != null)
            {
                children.Add(m_cycleComplete);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.SimulationActive:
                {
                    if (createOrReplace)
                    {
                        if (SimulationActive == null)
                        {
                            if (replacement == null)
                            {
                                SimulationActive = new PropertyState<bool>(this);
                            }
                            else
                            {
                                SimulationActive = (PropertyState<bool>)replacement;
                            }
                        }
                    }

                    instance = SimulationActive;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.GenerateValues:
                {
                    if (createOrReplace)
                    {
                        if (GenerateValues == null)
                        {
                            if (replacement == null)
                            {
                                GenerateValues = new GenerateValuesMethodState(this);
                            }
                            else
                            {
                                GenerateValues = (GenerateValuesMethodState)replacement;
                            }
                        }
                    }

                    instance = GenerateValues;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.CycleComplete:
                {
                    if (createOrReplace)
                    {
                        if (CycleComplete == null)
                        {
                            if (replacement == null)
                            {
                                CycleComplete = new AcknowledgeableConditionState(this);
                            }
                            else
                            {
                                CycleComplete = (AcknowledgeableConditionState)replacement;
                            }
                        }
                    }

                    instance = CycleComplete;
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
        private PropertyState<bool> m_simulationActive;
        private GenerateValuesMethodState m_generateValuesMethod;
        private AcknowledgeableConditionState m_cycleComplete;
        #endregion
    }
    #endif
    #endregion

    #region ScalarStructureVariableState Class
    #if (!OPCUA_EXCLUDE_ScalarStructureVariableState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ScalarStructureVariableState : BaseDataVariableState<ScalarStructureDataType>
    {
        #region Constructors
        /// <remarks />
        public ScalarStructureVariableState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.VariableTypes.ScalarStructureVariableType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        /// <remarks />
        protected override NodeId GetDefaultDataTypeId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.DataTypes.ScalarStructureDataType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        /// <remarks />
        protected override int GetDefaultValueRank()
        {
            return ValueRanks.Scalar;
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8VYIECAgAAAAEAIwAAAFNjYWxhclN0cnVjdHVyZVZhcmlhYmxlVHlwZUluc3RhbmNl" +
           "AQFPAAEBTwBPAAAAAQFOAAEB/////xsAAAAVYIkKAgAAAAEADAAAAEJvb2xlYW5WYWx1ZQEBUAAALwA/" +
           "UAAAAAAB/////wEB/////wAAAAAVYIkKAgAAAAEACgAAAFNCeXRlVmFsdWUBAVEAAC8AP1EAAAAAAv//" +
           "//8BAf////8AAAAAFWCJCgIAAAABAAkAAABCeXRlVmFsdWUBAVIAAC8AP1IAAAAAA/////8BAf////8A" +
           "AAAAFWCJCgIAAAABAAoAAABJbnQxNlZhbHVlAQFTAAAvAD9TAAAAAAT/////AQH/////AAAAABVgiQoC" +
           "AAAAAQALAAAAVUludDE2VmFsdWUBAVQAAC8AP1QAAAAABf////8BAf////8AAAAAFWCJCgIAAAABAAoA" +
           "AABJbnQzMlZhbHVlAQFVAAAvAD9VAAAAAAb/////AQH/////AAAAABVgiQoCAAAAAQALAAAAVUludDMy" +
           "VmFsdWUBAVYAAC8AP1YAAAAAB/////8BAf////8AAAAAFWCJCgIAAAABAAoAAABJbnQ2NFZhbHVlAQFX" +
           "AAAvAD9XAAAAAAj/////AQH/////AAAAABVgiQoCAAAAAQALAAAAVUludDY0VmFsdWUBAVgAAC8AP1gA" +
           "AAAACf////8BAf////8AAAAAFWCJCgIAAAABAAoAAABGbG9hdFZhbHVlAQFZAAAvAD9ZAAAAAAr/////" +
           "AQH/////AAAAABVgiQoCAAAAAQALAAAARG91YmxlVmFsdWUBAVoAAC8AP1oAAAAAC/////8BAf////8A" +
           "AAAAFWCJCgIAAAABAAsAAABTdHJpbmdWYWx1ZQEBWwAALwA/WwAAAAAM/////wEB/////wAAAAAVYIkK" +
           "AgAAAAEADQAAAERhdGVUaW1lVmFsdWUBAVwAAC8AP1wAAAAADf////8BAf////8AAAAAFWCJCgIAAAAB" +
           "AAkAAABHdWlkVmFsdWUBAV0AAC8AP10AAAAADv////8BAf////8AAAAAFWCJCgIAAAABAA8AAABCeXRl" +
           "U3RyaW5nVmFsdWUBAV4AAC8AP14AAAAAD/////8BAf////8AAAAAFWCJCgIAAAABAA8AAABYbWxFbGVt" +
           "ZW50VmFsdWUBAV8AAC8AP18AAAAAEP////8BAf////8AAAAAFWCJCgIAAAABAAsAAABOb2RlSWRWYWx1" +
           "ZQEBYAAALwA/YAAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAEAEwAAAEV4cGFuZGVkTm9kZUlkVmFs" +
           "dWUBAWEAAC8AP2EAAAAAEv////8BAf////8AAAAAFWCJCgIAAAABABIAAABRdWFsaWZpZWROYW1lVmFs" +
           "dWUBAWIAAC8AP2IAAAAAFP////8BAf////8AAAAAFWCJCgIAAAABABIAAABMb2NhbGl6ZWRUZXh0VmFs" +
           "dWUBAWMAAC8AP2MAAAAAFf////8BAf////8AAAAAFWCJCgIAAAABAA8AAABTdGF0dXNDb2RlVmFsdWUB" +
           "AWQAAC8AP2QAAAAAE/////8BAf////8AAAAAFWCJCgIAAAABAAwAAABWYXJpYW50VmFsdWUBAWUAAC8A" +
           "P2UAAAAAGP////8BAf////8AAAAAFWCJCgIAAAABABAAAABFbnVtZXJhdGlvblZhbHVlAQFmAAAvAD9m" +
           "AAAAAB3/////AQH/////AAAAABVgiQoCAAAAAQAOAAAAU3RydWN0dXJlVmFsdWUBAWcAAC8AP2cAAAAA" +
           "Fv////8BAf////8AAAAAFWCJCgIAAAABAAsAAABOdW1iZXJWYWx1ZQEBaAAALwA/aAAAAAAa/////wEB" +
           "/////wAAAAAVYIkKAgAAAAEADAAAAEludGVnZXJWYWx1ZQEBaQAALwA/aQAAAAAb/////wEB/////wAA" +
           "AAAVYIkKAgAAAAEADQAAAFVJbnRlZ2VyVmFsdWUBAWoAAC8AP2oAAAAAHP////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public BaseDataVariableState<bool> BooleanValue
        {
            get
            {
                return m_booleanValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_booleanValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_booleanValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<sbyte> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<short> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ushort> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<int> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<uint> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<long> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ulong> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<float> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<double> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<string> StringValue
        {
            get
            {
                return m_stringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_stringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_stringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<DateTime> DateTimeValue
        {
            get
            {
                return m_dateTimeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_dateTimeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_dateTimeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<Guid> GuidValue
        {
            get
            {
                return m_guidValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_guidValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_guidValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[]> ByteStringValue
        {
            get
            {
                return m_byteStringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteStringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteStringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<XmlElement> XmlElementValue
        {
            get
            {
                return m_xmlElementValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_xmlElementValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_xmlElementValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<NodeId> NodeIdValue
        {
            get
            {
                return m_nodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_nodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_nodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExpandedNodeId> ExpandedNodeIdValue
        {
            get
            {
                return m_expandedNodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_expandedNodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_expandedNodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<QualifiedName> QualifiedNameValue
        {
            get
            {
                return m_qualifiedNameValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_qualifiedNameValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_qualifiedNameValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<LocalizedText> LocalizedTextValue
        {
            get
            {
                return m_localizedTextValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_localizedTextValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_localizedTextValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<StatusCode> StatusCodeValue
        {
            get
            {
                return m_statusCodeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_statusCodeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_statusCodeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState VariantValue
        {
            get
            {
                return m_variantValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_variantValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_variantValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState EnumerationValue
        {
            get
            {
                return m_enumerationValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_enumerationValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_enumerationValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExtensionObject> StructureValue
        {
            get
            {
                return m_structureValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_structureValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_structureValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState NumberValue
        {
            get
            {
                return m_numberValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_numberValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_numberValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState IntegerValue
        {
            get
            {
                return m_integerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_integerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_integerValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState UIntegerValue
        {
            get
            {
                return m_uIntegerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uIntegerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uIntegerValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_booleanValue != null)
            {
                children.Add(m_booleanValue);
            }

            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_stringValue != null)
            {
                children.Add(m_stringValue);
            }

            if (m_dateTimeValue != null)
            {
                children.Add(m_dateTimeValue);
            }

            if (m_guidValue != null)
            {
                children.Add(m_guidValue);
            }

            if (m_byteStringValue != null)
            {
                children.Add(m_byteStringValue);
            }

            if (m_xmlElementValue != null)
            {
                children.Add(m_xmlElementValue);
            }

            if (m_nodeIdValue != null)
            {
                children.Add(m_nodeIdValue);
            }

            if (m_expandedNodeIdValue != null)
            {
                children.Add(m_expandedNodeIdValue);
            }

            if (m_qualifiedNameValue != null)
            {
                children.Add(m_qualifiedNameValue);
            }

            if (m_localizedTextValue != null)
            {
                children.Add(m_localizedTextValue);
            }

            if (m_statusCodeValue != null)
            {
                children.Add(m_statusCodeValue);
            }

            if (m_variantValue != null)
            {
                children.Add(m_variantValue);
            }

            if (m_enumerationValue != null)
            {
                children.Add(m_enumerationValue);
            }

            if (m_structureValue != null)
            {
                children.Add(m_structureValue);
            }

            if (m_numberValue != null)
            {
                children.Add(m_numberValue);
            }

            if (m_integerValue != null)
            {
                children.Add(m_integerValue);
            }

            if (m_uIntegerValue != null)
            {
                children.Add(m_uIntegerValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.BooleanValue:
                {
                    if (createOrReplace)
                    {
                        if (BooleanValue == null)
                        {
                            if (replacement == null)
                            {
                                BooleanValue = new BaseDataVariableState<bool>(this);
                            }
                            else
                            {
                                BooleanValue = (BaseDataVariableState<bool>)replacement;
                            }
                        }
                    }

                    instance = BooleanValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new BaseDataVariableState<sbyte>(this);
                            }
                            else
                            {
                                SByteValue = (BaseDataVariableState<sbyte>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new BaseDataVariableState<byte>(this);
                            }
                            else
                            {
                                ByteValue = (BaseDataVariableState<byte>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new BaseDataVariableState<short>(this);
                            }
                            else
                            {
                                Int16Value = (BaseDataVariableState<short>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new BaseDataVariableState<ushort>(this);
                            }
                            else
                            {
                                UInt16Value = (BaseDataVariableState<ushort>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new BaseDataVariableState<int>(this);
                            }
                            else
                            {
                                Int32Value = (BaseDataVariableState<int>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new BaseDataVariableState<uint>(this);
                            }
                            else
                            {
                                UInt32Value = (BaseDataVariableState<uint>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new BaseDataVariableState<long>(this);
                            }
                            else
                            {
                                Int64Value = (BaseDataVariableState<long>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new BaseDataVariableState<ulong>(this);
                            }
                            else
                            {
                                UInt64Value = (BaseDataVariableState<ulong>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new BaseDataVariableState<float>(this);
                            }
                            else
                            {
                                FloatValue = (BaseDataVariableState<float>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new BaseDataVariableState<double>(this);
                            }
                            else
                            {
                                DoubleValue = (BaseDataVariableState<double>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StringValue:
                {
                    if (createOrReplace)
                    {
                        if (StringValue == null)
                        {
                            if (replacement == null)
                            {
                                StringValue = new BaseDataVariableState<string>(this);
                            }
                            else
                            {
                                StringValue = (BaseDataVariableState<string>)replacement;
                            }
                        }
                    }

                    instance = StringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DateTimeValue:
                {
                    if (createOrReplace)
                    {
                        if (DateTimeValue == null)
                        {
                            if (replacement == null)
                            {
                                DateTimeValue = new BaseDataVariableState<DateTime>(this);
                            }
                            else
                            {
                                DateTimeValue = (BaseDataVariableState<DateTime>)replacement;
                            }
                        }
                    }

                    instance = DateTimeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.GuidValue:
                {
                    if (createOrReplace)
                    {
                        if (GuidValue == null)
                        {
                            if (replacement == null)
                            {
                                GuidValue = new BaseDataVariableState<Guid>(this);
                            }
                            else
                            {
                                GuidValue = (BaseDataVariableState<Guid>)replacement;
                            }
                        }
                    }

                    instance = GuidValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteStringValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteStringValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteStringValue = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                ByteStringValue = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ByteStringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.XmlElementValue:
                {
                    if (createOrReplace)
                    {
                        if (XmlElementValue == null)
                        {
                            if (replacement == null)
                            {
                                XmlElementValue = new BaseDataVariableState<XmlElement>(this);
                            }
                            else
                            {
                                XmlElementValue = (BaseDataVariableState<XmlElement>)replacement;
                            }
                        }
                    }

                    instance = XmlElementValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (NodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                NodeIdValue = new BaseDataVariableState<NodeId>(this);
                            }
                            else
                            {
                                NodeIdValue = (BaseDataVariableState<NodeId>)replacement;
                            }
                        }
                    }

                    instance = NodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ExpandedNodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (ExpandedNodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                ExpandedNodeIdValue = new BaseDataVariableState<ExpandedNodeId>(this);
                            }
                            else
                            {
                                ExpandedNodeIdValue = (BaseDataVariableState<ExpandedNodeId>)replacement;
                            }
                        }
                    }

                    instance = ExpandedNodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.QualifiedNameValue:
                {
                    if (createOrReplace)
                    {
                        if (QualifiedNameValue == null)
                        {
                            if (replacement == null)
                            {
                                QualifiedNameValue = new BaseDataVariableState<QualifiedName>(this);
                            }
                            else
                            {
                                QualifiedNameValue = (BaseDataVariableState<QualifiedName>)replacement;
                            }
                        }
                    }

                    instance = QualifiedNameValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.LocalizedTextValue:
                {
                    if (createOrReplace)
                    {
                        if (LocalizedTextValue == null)
                        {
                            if (replacement == null)
                            {
                                LocalizedTextValue = new BaseDataVariableState<LocalizedText>(this);
                            }
                            else
                            {
                                LocalizedTextValue = (BaseDataVariableState<LocalizedText>)replacement;
                            }
                        }
                    }

                    instance = LocalizedTextValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StatusCodeValue:
                {
                    if (createOrReplace)
                    {
                        if (StatusCodeValue == null)
                        {
                            if (replacement == null)
                            {
                                StatusCodeValue = new BaseDataVariableState<StatusCode>(this);
                            }
                            else
                            {
                                StatusCodeValue = (BaseDataVariableState<StatusCode>)replacement;
                            }
                        }
                    }

                    instance = StatusCodeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VariantValue:
                {
                    if (createOrReplace)
                    {
                        if (VariantValue == null)
                        {
                            if (replacement == null)
                            {
                                VariantValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                VariantValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = VariantValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.EnumerationValue:
                {
                    if (createOrReplace)
                    {
                        if (EnumerationValue == null)
                        {
                            if (replacement == null)
                            {
                                EnumerationValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                EnumerationValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = EnumerationValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StructureValue:
                {
                    if (createOrReplace)
                    {
                        if (StructureValue == null)
                        {
                            if (replacement == null)
                            {
                                StructureValue = new BaseDataVariableState<ExtensionObject>(this);
                            }
                            else
                            {
                                StructureValue = (BaseDataVariableState<ExtensionObject>)replacement;
                            }
                        }
                    }

                    instance = StructureValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NumberValue:
                {
                    if (createOrReplace)
                    {
                        if (NumberValue == null)
                        {
                            if (replacement == null)
                            {
                                NumberValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                NumberValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = NumberValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.IntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (IntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                IntegerValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                IntegerValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = IntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UIntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (UIntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                UIntegerValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                UIntegerValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = UIntegerValue;
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
        private BaseDataVariableState<bool> m_booleanValue;
        private BaseDataVariableState<sbyte> m_sByteValue;
        private BaseDataVariableState<byte> m_byteValue;
        private BaseDataVariableState<short> m_int16Value;
        private BaseDataVariableState<ushort> m_uInt16Value;
        private BaseDataVariableState<int> m_int32Value;
        private BaseDataVariableState<uint> m_uInt32Value;
        private BaseDataVariableState<long> m_int64Value;
        private BaseDataVariableState<ulong> m_uInt64Value;
        private BaseDataVariableState<float> m_floatValue;
        private BaseDataVariableState<double> m_doubleValue;
        private BaseDataVariableState<string> m_stringValue;
        private BaseDataVariableState<DateTime> m_dateTimeValue;
        private BaseDataVariableState<Guid> m_guidValue;
        private BaseDataVariableState<byte[]> m_byteStringValue;
        private BaseDataVariableState<XmlElement> m_xmlElementValue;
        private BaseDataVariableState<NodeId> m_nodeIdValue;
        private BaseDataVariableState<ExpandedNodeId> m_expandedNodeIdValue;
        private BaseDataVariableState<QualifiedName> m_qualifiedNameValue;
        private BaseDataVariableState<LocalizedText> m_localizedTextValue;
        private BaseDataVariableState<StatusCode> m_statusCodeValue;
        private BaseDataVariableState m_variantValue;
        private BaseDataVariableState m_enumerationValue;
        private BaseDataVariableState<ExtensionObject> m_structureValue;
        private BaseDataVariableState m_numberValue;
        private BaseDataVariableState m_integerValue;
        private BaseDataVariableState m_uIntegerValue;
        #endregion
    }

    #region ScalarStructureVariableValue Class
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public class ScalarStructureVariableValue : BaseVariableValue
    {
        #region Constructors
        /// <remarks />
        public ScalarStructureVariableValue(ScalarStructureVariableState variable, ScalarStructureDataType value, object dataLock) : base(dataLock)
        {
            m_value = value;

            if (m_value == null)
            {
                m_value = new ScalarStructureDataType();
            }

            Initialize(variable);
        }
        #endregion

        #region Public Members
        /// <remarks />
        public ScalarStructureVariableState Variable
        {
            get { return m_variable; }
        }

        /// <remarks />
        public ScalarStructureDataType Value
        {
            get { return m_value;  }
            set { m_value = value; }
        }
        #endregion

        #region Private Methods
        private void Initialize(ScalarStructureVariableState variable)
        {
            lock (Lock)
            {
                m_variable = variable;

                variable.Value = m_value;

                variable.OnReadValue = OnReadValue;
                variable.OnSimpleWriteValue = OnWriteValue;

                BaseVariableState instance = null;
                List<BaseInstanceState> updateList = new List<BaseInstanceState>();
                updateList.Add(variable);

                instance = m_variable.BooleanValue;
                instance.OnReadValue = OnRead_BooleanValue;
                instance.OnSimpleWriteValue = OnWrite_BooleanValue;
                updateList.Add(instance);
                instance = m_variable.SByteValue;
                instance.OnReadValue = OnRead_SByteValue;
                instance.OnSimpleWriteValue = OnWrite_SByteValue;
                updateList.Add(instance);
                instance = m_variable.ByteValue;
                instance.OnReadValue = OnRead_ByteValue;
                instance.OnSimpleWriteValue = OnWrite_ByteValue;
                updateList.Add(instance);
                instance = m_variable.Int16Value;
                instance.OnReadValue = OnRead_Int16Value;
                instance.OnSimpleWriteValue = OnWrite_Int16Value;
                updateList.Add(instance);
                instance = m_variable.UInt16Value;
                instance.OnReadValue = OnRead_UInt16Value;
                instance.OnSimpleWriteValue = OnWrite_UInt16Value;
                updateList.Add(instance);
                instance = m_variable.Int32Value;
                instance.OnReadValue = OnRead_Int32Value;
                instance.OnSimpleWriteValue = OnWrite_Int32Value;
                updateList.Add(instance);
                instance = m_variable.UInt32Value;
                instance.OnReadValue = OnRead_UInt32Value;
                instance.OnSimpleWriteValue = OnWrite_UInt32Value;
                updateList.Add(instance);
                instance = m_variable.Int64Value;
                instance.OnReadValue = OnRead_Int64Value;
                instance.OnSimpleWriteValue = OnWrite_Int64Value;
                updateList.Add(instance);
                instance = m_variable.UInt64Value;
                instance.OnReadValue = OnRead_UInt64Value;
                instance.OnSimpleWriteValue = OnWrite_UInt64Value;
                updateList.Add(instance);
                instance = m_variable.FloatValue;
                instance.OnReadValue = OnRead_FloatValue;
                instance.OnSimpleWriteValue = OnWrite_FloatValue;
                updateList.Add(instance);
                instance = m_variable.DoubleValue;
                instance.OnReadValue = OnRead_DoubleValue;
                instance.OnSimpleWriteValue = OnWrite_DoubleValue;
                updateList.Add(instance);
                instance = m_variable.StringValue;
                instance.OnReadValue = OnRead_StringValue;
                instance.OnSimpleWriteValue = OnWrite_StringValue;
                updateList.Add(instance);
                instance = m_variable.DateTimeValue;
                instance.OnReadValue = OnRead_DateTimeValue;
                instance.OnSimpleWriteValue = OnWrite_DateTimeValue;
                updateList.Add(instance);
                instance = m_variable.GuidValue;
                instance.OnReadValue = OnRead_GuidValue;
                instance.OnSimpleWriteValue = OnWrite_GuidValue;
                updateList.Add(instance);
                instance = m_variable.ByteStringValue;
                instance.OnReadValue = OnRead_ByteStringValue;
                instance.OnSimpleWriteValue = OnWrite_ByteStringValue;
                updateList.Add(instance);
                instance = m_variable.XmlElementValue;
                instance.OnReadValue = OnRead_XmlElementValue;
                instance.OnSimpleWriteValue = OnWrite_XmlElementValue;
                updateList.Add(instance);
                instance = m_variable.NodeIdValue;
                instance.OnReadValue = OnRead_NodeIdValue;
                instance.OnSimpleWriteValue = OnWrite_NodeIdValue;
                updateList.Add(instance);
                instance = m_variable.ExpandedNodeIdValue;
                instance.OnReadValue = OnRead_ExpandedNodeIdValue;
                instance.OnSimpleWriteValue = OnWrite_ExpandedNodeIdValue;
                updateList.Add(instance);
                instance = m_variable.QualifiedNameValue;
                instance.OnReadValue = OnRead_QualifiedNameValue;
                instance.OnSimpleWriteValue = OnWrite_QualifiedNameValue;
                updateList.Add(instance);
                instance = m_variable.LocalizedTextValue;
                instance.OnReadValue = OnRead_LocalizedTextValue;
                instance.OnSimpleWriteValue = OnWrite_LocalizedTextValue;
                updateList.Add(instance);
                instance = m_variable.StatusCodeValue;
                instance.OnReadValue = OnRead_StatusCodeValue;
                instance.OnSimpleWriteValue = OnWrite_StatusCodeValue;
                updateList.Add(instance);
                instance = m_variable.VariantValue;
                instance.OnReadValue = OnRead_VariantValue;
                instance.OnSimpleWriteValue = OnWrite_VariantValue;
                updateList.Add(instance);
                instance = m_variable.EnumerationValue;
                instance.OnReadValue = OnRead_EnumerationValue;
                instance.OnSimpleWriteValue = OnWrite_EnumerationValue;
                updateList.Add(instance);
                instance = m_variable.StructureValue;
                instance.OnReadValue = OnRead_StructureValue;
                instance.OnSimpleWriteValue = OnWrite_StructureValue;
                updateList.Add(instance);
                instance = m_variable.NumberValue;
                instance.OnReadValue = OnRead_NumberValue;
                instance.OnSimpleWriteValue = OnWrite_NumberValue;
                updateList.Add(instance);
                instance = m_variable.IntegerValue;
                instance.OnReadValue = OnRead_IntegerValue;
                instance.OnSimpleWriteValue = OnWrite_IntegerValue;
                updateList.Add(instance);
                instance = m_variable.UIntegerValue;
                instance.OnReadValue = OnRead_UIntegerValue;
                instance.OnSimpleWriteValue = OnWrite_UIntegerValue;
                updateList.Add(instance);

                SetUpdateList(updateList);
            }
        }

        /// <remarks />
        protected ServiceResult OnReadValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        private ServiceResult OnWriteValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value = (ScalarStructureDataType)Write(value);
            }

            return ServiceResult.Good;
        }

        #region BooleanValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_BooleanValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.BooleanValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_BooleanValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.BooleanValue = (bool)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region SByteValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_SByteValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.SByteValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_SByteValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.SByteValue = (sbyte)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region ByteValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_ByteValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.ByteValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_ByteValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.ByteValue = (byte)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region Int16Value Access Methods
        /// <remarks />
        private ServiceResult OnRead_Int16Value(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.Int16Value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_Int16Value(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.Int16Value = (short)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region UInt16Value Access Methods
        /// <remarks />
        private ServiceResult OnRead_UInt16Value(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.UInt16Value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_UInt16Value(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.UInt16Value = (ushort)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region Int32Value Access Methods
        /// <remarks />
        private ServiceResult OnRead_Int32Value(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.Int32Value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_Int32Value(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.Int32Value = (int)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region UInt32Value Access Methods
        /// <remarks />
        private ServiceResult OnRead_UInt32Value(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.UInt32Value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_UInt32Value(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.UInt32Value = (uint)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region Int64Value Access Methods
        /// <remarks />
        private ServiceResult OnRead_Int64Value(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.Int64Value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_Int64Value(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.Int64Value = (long)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region UInt64Value Access Methods
        /// <remarks />
        private ServiceResult OnRead_UInt64Value(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.UInt64Value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_UInt64Value(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.UInt64Value = (ulong)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region FloatValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_FloatValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.FloatValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_FloatValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.FloatValue = (float)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region DoubleValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_DoubleValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.DoubleValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_DoubleValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.DoubleValue = (double)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region StringValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_StringValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.StringValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_StringValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.StringValue = (string)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region DateTimeValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_DateTimeValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.DateTimeValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_DateTimeValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.DateTimeValue = (DateTime)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region GuidValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_GuidValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.GuidValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_GuidValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.GuidValue = (Uuid)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region ByteStringValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_ByteStringValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.ByteStringValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_ByteStringValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.ByteStringValue = (byte[])Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region XmlElementValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_XmlElementValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.XmlElementValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_XmlElementValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.XmlElementValue = (XmlElement)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region NodeIdValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_NodeIdValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.NodeIdValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_NodeIdValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.NodeIdValue = (NodeId)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region ExpandedNodeIdValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_ExpandedNodeIdValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.ExpandedNodeIdValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_ExpandedNodeIdValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.ExpandedNodeIdValue = (ExpandedNodeId)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region QualifiedNameValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_QualifiedNameValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.QualifiedNameValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_QualifiedNameValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.QualifiedNameValue = (QualifiedName)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region LocalizedTextValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_LocalizedTextValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.LocalizedTextValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_LocalizedTextValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.LocalizedTextValue = (LocalizedText)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region StatusCodeValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_StatusCodeValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.StatusCodeValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_StatusCodeValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.StatusCodeValue = (StatusCode)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region VariantValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_VariantValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.VariantValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_VariantValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.VariantValue = (Variant)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region EnumerationValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_EnumerationValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.EnumerationValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_EnumerationValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.EnumerationValue = (int)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region StructureValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_StructureValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.StructureValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_StructureValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.StructureValue = (ExtensionObject)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region NumberValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_NumberValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.NumberValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_NumberValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.NumberValue = (Variant)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region IntegerValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_IntegerValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.IntegerValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_IntegerValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.IntegerValue = (Variant)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region UIntegerValue Access Methods
        /// <remarks />
        private ServiceResult OnRead_UIntegerValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.UIntegerValue;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_UIntegerValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.UIntegerValue = (Variant)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion
        #endregion

        #region Private Fields
        private ScalarStructureDataType m_value;
        private ScalarStructureVariableState m_variable;
        #endregion
    }
    #endregion
    #endif
    #endregion

    #region ScalarValue1MethodState Class
    #if (!OPCUA_EXCLUDE_ScalarValue1MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ScalarValue1MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public ScalarValue1MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new ScalarValue1MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAFgAAAFNjYWxhclZhbHVlMU1ldGhvZFR5cGUBAWsAAC8BAWsAawAA" +
           "AAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQFsAAAuAERsAAAAlgsAAAABACoB" +
           "ARgAAAAJAAAAQm9vbGVhbkluAAH/////AAAAAAABACoBARYAAAAHAAAAU0J5dGVJbgAC/////wAAAAAA" +
           "AQAqAQEVAAAABgAAAEJ5dGVJbgAD/////wAAAAAAAQAqAQEWAAAABwAAAEludDE2SW4ABP////8AAAAA" +
           "AAEAKgEBFwAAAAgAAABVSW50MTZJbgAF/////wAAAAAAAQAqAQEWAAAABwAAAEludDMySW4ABv////8A" +
           "AAAAAAEAKgEBFwAAAAgAAABVSW50MzJJbgAH/////wAAAAAAAQAqAQEWAAAABwAAAEludDY0SW4ACP//" +
           "//8AAAAAAAEAKgEBFwAAAAgAAABVSW50NjRJbgAJ/////wAAAAAAAQAqAQEWAAAABwAAAEZsb2F0SW4A" +
           "Cv////8AAAAAAAEAKgEBFwAAAAgAAABEb3VibGVJbgAL/////wAAAAAAAQAoAQEAAAABAAAAAAAAAAEB" +
           "/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBbQAALgBEbQAAAJYLAAAAAQAqAQEZ" +
           "AAAACgAAAEJvb2xlYW5PdXQAAf////8AAAAAAAEAKgEBFwAAAAgAAABTQnl0ZU91dAAC/////wAAAAAA" +
           "AQAqAQEWAAAABwAAAEJ5dGVPdXQAA/////8AAAAAAAEAKgEBFwAAAAgAAABJbnQxNk91dAAE/////wAA" +
           "AAAAAQAqAQEYAAAACQAAAFVJbnQxNk91dAAF/////wAAAAAAAQAqAQEXAAAACAAAAEludDMyT3V0AAb/" +
           "////AAAAAAABACoBARgAAAAJAAAAVUludDMyT3V0AAf/////AAAAAAABACoBARcAAAAIAAAASW50NjRP" +
           "dXQACP////8AAAAAAAEAKgEBGAAAAAkAAABVSW50NjRPdXQACf////8AAAAAAAEAKgEBFwAAAAgAAABG" +
           "bG9hdE91dAAK/////wAAAAAAAQAqAQEYAAAACQAAAERvdWJsZU91dAAL/////wAAAAAAAQAoAQEAAAAB" +
           "AAAAAAAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public ScalarValue1MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            bool booleanIn = (bool)_inputArguments[0];
            sbyte sByteIn = (sbyte)_inputArguments[1];
            byte byteIn = (byte)_inputArguments[2];
            short int16In = (short)_inputArguments[3];
            ushort uInt16In = (ushort)_inputArguments[4];
            int int32In = (int)_inputArguments[5];
            uint uInt32In = (uint)_inputArguments[6];
            long int64In = (long)_inputArguments[7];
            ulong uInt64In = (ulong)_inputArguments[8];
            float floatIn = (float)_inputArguments[9];
            double doubleIn = (double)_inputArguments[10];

            bool booleanOut = (bool)_outputArguments[0];
            sbyte sByteOut = (sbyte)_outputArguments[1];
            byte byteOut = (byte)_outputArguments[2];
            short int16Out = (short)_outputArguments[3];
            ushort uInt16Out = (ushort)_outputArguments[4];
            int int32Out = (int)_outputArguments[5];
            uint uInt32Out = (uint)_outputArguments[6];
            long int64Out = (long)_outputArguments[7];
            ulong uInt64Out = (ulong)_outputArguments[8];
            float floatOut = (float)_outputArguments[9];
            double doubleOut = (double)_outputArguments[10];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    booleanIn,
                    sByteIn,
                    byteIn,
                    int16In,
                    uInt16In,
                    int32In,
                    uInt32In,
                    int64In,
                    uInt64In,
                    floatIn,
                    doubleIn,
                    ref booleanOut,
                    ref sByteOut,
                    ref byteOut,
                    ref int16Out,
                    ref uInt16Out,
                    ref int32Out,
                    ref uInt32Out,
                    ref int64Out,
                    ref uInt64Out,
                    ref floatOut,
                    ref doubleOut);
            }

            _outputArguments[0] = booleanOut;
            _outputArguments[1] = sByteOut;
            _outputArguments[2] = byteOut;
            _outputArguments[3] = int16Out;
            _outputArguments[4] = uInt16Out;
            _outputArguments[5] = int32Out;
            _outputArguments[6] = uInt32Out;
            _outputArguments[7] = int64Out;
            _outputArguments[8] = uInt64Out;
            _outputArguments[9] = floatOut;
            _outputArguments[10] = doubleOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult ScalarValue1MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        bool booleanIn,
        sbyte sByteIn,
        byte byteIn,
        short int16In,
        ushort uInt16In,
        int int32In,
        uint uInt32In,
        long int64In,
        ulong uInt64In,
        float floatIn,
        double doubleIn,
        ref bool booleanOut,
        ref sbyte sByteOut,
        ref byte byteOut,
        ref short int16Out,
        ref ushort uInt16Out,
        ref int int32Out,
        ref uint uInt32Out,
        ref long int64Out,
        ref ulong uInt64Out,
        ref float floatOut,
        ref double doubleOut);
    #endif
    #endregion

    #region ScalarValue2MethodState Class
    #if (!OPCUA_EXCLUDE_ScalarValue2MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ScalarValue2MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public ScalarValue2MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new ScalarValue2MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAFgAAAFNjYWxhclZhbHVlMk1ldGhvZFR5cGUBAW4AAC8BAW4AbgAA" +
           "AAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQFvAAAuAERvAAAAlgoAAAABACoB" +
           "ARcAAAAIAAAAU3RyaW5nSW4ADP////8AAAAAAAEAKgEBGQAAAAoAAABEYXRlVGltZUluAA3/////AAAA" +
           "AAABACoBARUAAAAGAAAAR3VpZEluAA7/////AAAAAAABACoBARsAAAAMAAAAQnl0ZVN0cmluZ0luAA//" +
           "////AAAAAAABACoBARsAAAAMAAAAWG1sRWxlbWVudEluABD/////AAAAAAABACoBARcAAAAIAAAATm9k" +
           "ZUlkSW4AEf////8AAAAAAAEAKgEBHwAAABAAAABFeHBhbmRlZE5vZGVJZEluABL/////AAAAAAABACoB" +
           "AR4AAAAPAAAAUXVhbGlmaWVkTmFtZUluABT/////AAAAAAABACoBAR4AAAAPAAAATG9jYWxpemVkVGV4" +
           "dEluABX/////AAAAAAABACoBARsAAAAMAAAAU3RhdHVzQ29kZUluABP/////AAAAAAABACgBAQAAAAEA" +
           "AAAAAAAAAQH/////AAAAABdgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQFwAAAuAERwAAAAlgoA" +
           "AAABACoBARgAAAAJAAAAU3RyaW5nT3V0AAz/////AAAAAAABACoBARoAAAALAAAARGF0ZVRpbWVPdXQA" +
           "Df////8AAAAAAAEAKgEBFgAAAAcAAABHdWlkT3V0AA7/////AAAAAAABACoBARwAAAANAAAAQnl0ZVN0" +
           "cmluZ091dAAP/////wAAAAAAAQAqAQEcAAAADQAAAFhtbEVsZW1lbnRPdXQAEP////8AAAAAAAEAKgEB" +
           "GAAAAAkAAABOb2RlSWRPdXQAEf////8AAAAAAAEAKgEBIAAAABEAAABFeHBhbmRlZE5vZGVJZE91dAAS" +
           "/////wAAAAAAAQAqAQEfAAAAEAAAAFF1YWxpZmllZE5hbWVPdXQAFP////8AAAAAAAEAKgEBHwAAABAA" +
           "AABMb2NhbGl6ZWRUZXh0T3V0ABX/////AAAAAAABACoBARwAAAANAAAAU3RhdHVzQ29kZU91dAAT////" +
           "/wAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public ScalarValue2MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            string stringIn = (string)_inputArguments[0];
            DateTime dateTimeIn = (DateTime)_inputArguments[1];
            Uuid guidIn = (Uuid)_inputArguments[2];
            byte[] byteStringIn = (byte[])_inputArguments[3];
            XmlElement xmlElementIn = (XmlElement)_inputArguments[4];
            NodeId nodeIdIn = (NodeId)_inputArguments[5];
            ExpandedNodeId expandedNodeIdIn = (ExpandedNodeId)_inputArguments[6];
            QualifiedName qualifiedNameIn = (QualifiedName)_inputArguments[7];
            LocalizedText localizedTextIn = (LocalizedText)_inputArguments[8];
            StatusCode statusCodeIn = (StatusCode)_inputArguments[9];

            string stringOut = (string)_outputArguments[0];
            DateTime dateTimeOut = (DateTime)_outputArguments[1];
            Uuid guidOut = (Uuid)_outputArguments[2];
            byte[] byteStringOut = (byte[])_outputArguments[3];
            XmlElement xmlElementOut = (XmlElement)_outputArguments[4];
            NodeId nodeIdOut = (NodeId)_outputArguments[5];
            ExpandedNodeId expandedNodeIdOut = (ExpandedNodeId)_outputArguments[6];
            QualifiedName qualifiedNameOut = (QualifiedName)_outputArguments[7];
            LocalizedText localizedTextOut = (LocalizedText)_outputArguments[8];
            StatusCode statusCodeOut = (StatusCode)_outputArguments[9];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    stringIn,
                    dateTimeIn,
                    guidIn,
                    byteStringIn,
                    xmlElementIn,
                    nodeIdIn,
                    expandedNodeIdIn,
                    qualifiedNameIn,
                    localizedTextIn,
                    statusCodeIn,
                    ref stringOut,
                    ref dateTimeOut,
                    ref guidOut,
                    ref byteStringOut,
                    ref xmlElementOut,
                    ref nodeIdOut,
                    ref expandedNodeIdOut,
                    ref qualifiedNameOut,
                    ref localizedTextOut,
                    ref statusCodeOut);
            }

            _outputArguments[0] = stringOut;
            _outputArguments[1] = dateTimeOut;
            _outputArguments[2] = guidOut;
            _outputArguments[3] = byteStringOut;
            _outputArguments[4] = xmlElementOut;
            _outputArguments[5] = nodeIdOut;
            _outputArguments[6] = expandedNodeIdOut;
            _outputArguments[7] = qualifiedNameOut;
            _outputArguments[8] = localizedTextOut;
            _outputArguments[9] = statusCodeOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult ScalarValue2MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        string stringIn,
        DateTime dateTimeIn,
        Uuid guidIn,
        byte[] byteStringIn,
        XmlElement xmlElementIn,
        NodeId nodeIdIn,
        ExpandedNodeId expandedNodeIdIn,
        QualifiedName qualifiedNameIn,
        LocalizedText localizedTextIn,
        StatusCode statusCodeIn,
        ref string stringOut,
        ref DateTime dateTimeOut,
        ref Uuid guidOut,
        ref byte[] byteStringOut,
        ref XmlElement xmlElementOut,
        ref NodeId nodeIdOut,
        ref ExpandedNodeId expandedNodeIdOut,
        ref QualifiedName qualifiedNameOut,
        ref LocalizedText localizedTextOut,
        ref StatusCode statusCodeOut);
    #endif
    #endregion

    #region ScalarValue3MethodState Class
    #if (!OPCUA_EXCLUDE_ScalarValue3MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ScalarValue3MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public ScalarValue3MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new ScalarValue3MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAFgAAAFNjYWxhclZhbHVlM01ldGhvZFR5cGUBAXEAAC8BAXEAcQAA" +
           "AAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQFyAAAuAERyAAAAlgMAAAABACoB" +
           "ARgAAAAJAAAAVmFyaWFudEluABj/////AAAAAAABACoBARwAAAANAAAARW51bWVyYXRpb25JbgAd////" +
           "/wAAAAAAAQAqAQEaAAAACwAAAFN0cnVjdHVyZUluABb/////AAAAAAABACgBAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAABdgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQFzAAAuAERzAAAAlgMAAAABACoBARkA" +
           "AAAKAAAAVmFyaWFudE91dAAY/////wAAAAAAAQAqAQEdAAAADgAAAEVudW1lcmF0aW9uT3V0AB3/////" +
           "AAAAAAABACoBARsAAAAMAAAAU3RydWN0dXJlT3V0ABb/////AAAAAAABACgBAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public ScalarValue3MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            object variantIn = (object)_inputArguments[0];
            int enumerationIn = (int)_inputArguments[1];
            ExtensionObject structureIn = (ExtensionObject)_inputArguments[2];

            object variantOut = (object)_outputArguments[0];
            int enumerationOut = (int)_outputArguments[1];
            ExtensionObject structureOut = (ExtensionObject)_outputArguments[2];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    variantIn,
                    enumerationIn,
                    structureIn,
                    ref variantOut,
                    ref enumerationOut,
                    ref structureOut);
            }

            _outputArguments[0] = variantOut;
            _outputArguments[1] = enumerationOut;
            _outputArguments[2] = structureOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult ScalarValue3MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        object variantIn,
        int enumerationIn,
        ExtensionObject structureIn,
        ref object variantOut,
        ref int enumerationOut,
        ref ExtensionObject structureOut);
    #endif
    #endregion

    #region ScalarValueObjectState Class
    #if (!OPCUA_EXCLUDE_ScalarValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ScalarValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public ScalarValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.ScalarValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAHQAAAFNjYWxhclZhbHVlT2JqZWN0VHlwZUluc3RhbmNlAQF0AAEB" +
           "dAB0AAAAAQAAAAAkAAEBeAAfAAAANWCJCgIAAAABABAAAABTaW11bGF0aW9uQWN0aXZlAQF1AAMAAAAA" +
           "RwAAAElmIHRydWUgdGhlIHNlcnZlciB3aWxsIHByb2R1Y2UgbmV3IHZhbHVlcyBmb3IgZWFjaCBtb25p" +
           "dG9yZWQgdmFyaWFibGUuAC4ARHUAAAAAAf////8BAf////8AAAAABGGCCgQAAAABAA4AAABHZW5lcmF0" +
           "ZVZhbHVlcwEBdgAALwEBEQB2AAAAAQH/////AQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMB" +
           "AXcAAC4ARHcAAACWAQAAAAEAKgEBRgAAAAoAAABJdGVyYXRpb25zAAf/////AAAAAAMAAAAAJQAAAFRo" +
           "ZSBudW1iZXIgb2YgbmV3IHZhbHVlcyB0byBnZW5lcmF0ZS4BACgBAQAAAAEAAAAAAAAAAQH/////AAAA" +
           "AARggAoBAAAAAQANAAAAQ3ljbGVDb21wbGV0ZQEBeAAALwEAQQt4AAAAAQAAAAAkAQEBdAAXAAAAFWCJ" +
           "CgIAAAAAAAcAAABFdmVudElkAQF5AAAuAER5AAAAAA//////AQH/////AAAAABVgiQoCAAAAAAAJAAAA" +
           "RXZlbnRUeXBlAQF6AAAuAER6AAAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNlTm9k" +
           "ZQEBewAALgBEewAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJjZU5hbWUBAXwAAC4A" +
           "RHwAAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAQAAABUaW1lAQF9AAAuAER9AAAAAQAmAf////8B" +
           "Af////8AAAAAFWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEBfgAALgBEfgAAAAEAJgH/////AQH/////" +
           "AAAAABVgiQoCAAAAAAAHAAAATWVzc2FnZQEBgAAALgBEgAAAAAAV/////wEB/////wAAAAAVYIkKAgAA" +
           "AAAACAAAAFNldmVyaXR5AQGBAAAuAESBAAAAAAX/////AQH/////AAAAABVgiQoCAAAAAAAQAAAAQ29u" +
           "ZGl0aW9uQ2xhc3NJZAEBggAALgBEggAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAAEgAAAENvbmRp" +
           "dGlvbkNsYXNzTmFtZQEBgwAALgBEgwAAAAAV/////wEB/////wAAAAAVYIkKAgAAAAAADQAAAENvbmRp" +
           "dGlvbk5hbWUBAYYAAC4ARIYAAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAgAAABCcmFuY2hJZAEB" +
           "hwAALgBEhwAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAABgAAAFJldGFpbgEBiAAALgBEiAAAAAAB" +
           "/////wEB/////wAAAAAVYIkKAgAAAAAADAAAAEVuYWJsZWRTdGF0ZQEBiQAALwEAIyOJAAAAABX/////" +
           "AQECAAAAAQAsIwABAZ0AAQAsIwABAaYAAQAAABVgiQoCAAAAAAACAAAASWQBAYoAAC4ARIoAAAAAAf//" +
           "//8BAf////8AAAAAFWCJCgIAAAAAAAcAAABRdWFsaXR5AQGSAAAvAQAqI5IAAAAAE/////8BAf////8B" +
           "AAAAFWCJCgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAZMAAC4ARJMAAAABACYB/////wEB/////wAA" +
           "AAAVYIkKAgAAAAAADAAAAExhc3RTZXZlcml0eQEBlAAALwEAKiOUAAAAAAX/////AQH/////AQAAABVg" +
           "iQoCAAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQGVAAAuAESVAAAAAQAmAf////8BAf////8AAAAAFWCJ" +
           "CgIAAAAAAAcAAABDb21tZW50AQGWAAAvAQAqI5YAAAAAFf////8BAf////8BAAAAFWCJCgIAAAAAAA8A" +
           "AABTb3VyY2VUaW1lc3RhbXABAZcAAC4ARJcAAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAADAAA" +
           "AENsaWVudFVzZXJJZAEBmAAALgBEmAAAAAAM/////wEB/////wAAAAAEYYIKBAAAAAAABwAAAERpc2Fi" +
           "bGUBAZkAAC8BAEQjmQAAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAABgAAAEVuYWJsZQEBmgAA" +
           "LwEAQyOaAAAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAKAAAAQWRkQ29tbWVudAEBmwAALwEA" +
           "RSObAAAAAQEBAAAAAQD5CwABAA0LAQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAZwAAC4A" +
           "RJwAAACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlm" +
           "aWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAAAAAD" +
           "AAAAACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAA" +
           "AQH/////AAAAABVgiQoCAAAAAAAKAAAAQWNrZWRTdGF0ZQEBnQAALwEAIyOdAAAAABX/////AQEBAAAA" +
           "AQAsIwEBAYkAAQAAABVgiQoCAAAAAAACAAAASWQBAZ4AAC4ARJ4AAAAAAf////8BAf////8AAAAABGGC" +
           "CgQAAAAAAAsAAABBY2tub3dsZWRnZQEBrwAALwEAlyOvAAAAAQEBAAAAAQD5CwABAPAiAQAAABdgqQoC" +
           "AAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAbAAAC4ARLAAAACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElk" +
           "AA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4B" +
           "ACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAAAAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8g" +
           "dGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAABVgiQoCAAAAAQAMAAAAQm9vbGVh" +
           "blZhbHVlAQGzAAAvAD+zAAAAAAH/////AQH/////AAAAABVgiQoCAAAAAQAKAAAAU0J5dGVWYWx1ZQEB" +
           "tAAALwA/tAAAAAAC/////wEB/////wAAAAAVYIkKAgAAAAEACQAAAEJ5dGVWYWx1ZQEBtQAALwA/tQAA" +
           "AAAD/////wEB/////wAAAAAVYIkKAgAAAAEACgAAAEludDE2VmFsdWUBAbYAAC8AP7YAAAAABP////8B" +
           "Af////8AAAAAFWCJCgIAAAABAAsAAABVSW50MTZWYWx1ZQEBtwAALwA/twAAAAAF/////wEB/////wAA" +
           "AAAVYIkKAgAAAAEACgAAAEludDMyVmFsdWUBAbgAAC8AP7gAAAAABv////8BAf////8AAAAAFWCJCgIA" +
           "AAABAAsAAABVSW50MzJWYWx1ZQEBuQAALwA/uQAAAAAH/////wEB/////wAAAAAVYIkKAgAAAAEACgAA" +
           "AEludDY0VmFsdWUBAboAAC8AP7oAAAAACP////8BAf////8AAAAAFWCJCgIAAAABAAsAAABVSW50NjRW" +
           "YWx1ZQEBuwAALwA/uwAAAAAJ/////wEB/////wAAAAAVYIkKAgAAAAEACgAAAEZsb2F0VmFsdWUBAbwA" +
           "AC8AP7wAAAAACv////8BAf////8AAAAAFWCJCgIAAAABAAsAAABEb3VibGVWYWx1ZQEBvQAALwA/vQAA" +
           "AAAL/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAFN0cmluZ1ZhbHVlAQG+AAAvAD++AAAAAAz/////" +
           "AQH/////AAAAABVgiQoCAAAAAQANAAAARGF0ZVRpbWVWYWx1ZQEBvwAALwA/vwAAAAAN/////wEB////" +
           "/wAAAAAVYIkKAgAAAAEACQAAAEd1aWRWYWx1ZQEBwAAALwA/wAAAAAAO/////wEB/////wAAAAAVYIkK" +
           "AgAAAAEADwAAAEJ5dGVTdHJpbmdWYWx1ZQEBwQAALwA/wQAAAAAP/////wEB/////wAAAAAVYIkKAgAA" +
           "AAEADwAAAFhtbEVsZW1lbnRWYWx1ZQEBwgAALwA/wgAAAAAQ/////wEB/////wAAAAAVYIkKAgAAAAEA" +
           "CwAAAE5vZGVJZFZhbHVlAQHDAAAvAD/DAAAAABH/////AQH/////AAAAABVgiQoCAAAAAQATAAAARXhw" +
           "YW5kZWROb2RlSWRWYWx1ZQEBxAAALwA/xAAAAAAS/////wEB/////wAAAAAVYIkKAgAAAAEAEgAAAFF1" +
           "YWxpZmllZE5hbWVWYWx1ZQEBxQAALwA/xQAAAAAU/////wEB/////wAAAAAVYIkKAgAAAAEAEgAAAExv" +
           "Y2FsaXplZFRleHRWYWx1ZQEBxgAALwA/xgAAAAAV/////wEB/////wAAAAAVYIkKAgAAAAEADwAAAFN0" +
           "YXR1c0NvZGVWYWx1ZQEBxwAALwA/xwAAAAAT/////wEB/////wAAAAAVYIkKAgAAAAEADAAAAFZhcmlh" +
           "bnRWYWx1ZQEByAAALwA/yAAAAAAY/////wEB/////wAAAAAVYIkKAgAAAAEAEAAAAEVudW1lcmF0aW9u" +
           "VmFsdWUBAckAAC8AP8kAAAAAHf////8BAf////8AAAAAFWCJCgIAAAABAA4AAABTdHJ1Y3R1cmVWYWx1" +
           "ZQEBygAALwA/ygAAAAAW/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAE51bWJlclZhbHVlAQHLAAAv" +
           "AD/LAAAAABr/////AQH/////AAAAABVgiQoCAAAAAQAMAAAASW50ZWdlclZhbHVlAQHMAAAvAD/MAAAA" +
           "ABv/////AQH/////AAAAABVgiQoCAAAAAQANAAAAVUludGVnZXJWYWx1ZQEBzQAALwA/zQAAAAAc////" +
           "/wEB/////wAAAAAVYIkKAgAAAAEACwAAAFZlY3RvclZhbHVlAQHOAAAvAQF5A84AAAABAXgD/////wEB" +
           "/////wMAAAAVYIkKAgAAAAEAAQAAAFgBAc8AAC4ARM8AAAAAC/////8BAf////8AAAAAFWCJCgIAAAAB" +
           "AAEAAABZAQHQAAAuAETQAAAAAAv/////AQH/////AAAAABVgiQoCAAAAAQABAAAAWgEB0QAALgBE0QAA" +
           "AAAL/////wEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public BaseDataVariableState<bool> BooleanValue
        {
            get
            {
                return m_booleanValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_booleanValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_booleanValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<sbyte> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<short> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ushort> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<int> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<uint> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<long> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ulong> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<float> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<double> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<string> StringValue
        {
            get
            {
                return m_stringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_stringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_stringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<DateTime> DateTimeValue
        {
            get
            {
                return m_dateTimeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_dateTimeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_dateTimeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<Guid> GuidValue
        {
            get
            {
                return m_guidValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_guidValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_guidValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[]> ByteStringValue
        {
            get
            {
                return m_byteStringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteStringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteStringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<XmlElement> XmlElementValue
        {
            get
            {
                return m_xmlElementValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_xmlElementValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_xmlElementValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<NodeId> NodeIdValue
        {
            get
            {
                return m_nodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_nodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_nodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExpandedNodeId> ExpandedNodeIdValue
        {
            get
            {
                return m_expandedNodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_expandedNodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_expandedNodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<QualifiedName> QualifiedNameValue
        {
            get
            {
                return m_qualifiedNameValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_qualifiedNameValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_qualifiedNameValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<LocalizedText> LocalizedTextValue
        {
            get
            {
                return m_localizedTextValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_localizedTextValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_localizedTextValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<StatusCode> StatusCodeValue
        {
            get
            {
                return m_statusCodeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_statusCodeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_statusCodeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState VariantValue
        {
            get
            {
                return m_variantValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_variantValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_variantValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState EnumerationValue
        {
            get
            {
                return m_enumerationValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_enumerationValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_enumerationValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExtensionObject> StructureValue
        {
            get
            {
                return m_structureValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_structureValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_structureValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState NumberValue
        {
            get
            {
                return m_numberValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_numberValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_numberValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState IntegerValue
        {
            get
            {
                return m_integerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_integerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_integerValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState UIntegerValue
        {
            get
            {
                return m_uIntegerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uIntegerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uIntegerValue = value;
            }
        }

        /// <remarks />
        public VectorVariableState VectorValue
        {
            get
            {
                return m_vectorValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_vectorValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_vectorValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_booleanValue != null)
            {
                children.Add(m_booleanValue);
            }

            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_stringValue != null)
            {
                children.Add(m_stringValue);
            }

            if (m_dateTimeValue != null)
            {
                children.Add(m_dateTimeValue);
            }

            if (m_guidValue != null)
            {
                children.Add(m_guidValue);
            }

            if (m_byteStringValue != null)
            {
                children.Add(m_byteStringValue);
            }

            if (m_xmlElementValue != null)
            {
                children.Add(m_xmlElementValue);
            }

            if (m_nodeIdValue != null)
            {
                children.Add(m_nodeIdValue);
            }

            if (m_expandedNodeIdValue != null)
            {
                children.Add(m_expandedNodeIdValue);
            }

            if (m_qualifiedNameValue != null)
            {
                children.Add(m_qualifiedNameValue);
            }

            if (m_localizedTextValue != null)
            {
                children.Add(m_localizedTextValue);
            }

            if (m_statusCodeValue != null)
            {
                children.Add(m_statusCodeValue);
            }

            if (m_variantValue != null)
            {
                children.Add(m_variantValue);
            }

            if (m_enumerationValue != null)
            {
                children.Add(m_enumerationValue);
            }

            if (m_structureValue != null)
            {
                children.Add(m_structureValue);
            }

            if (m_numberValue != null)
            {
                children.Add(m_numberValue);
            }

            if (m_integerValue != null)
            {
                children.Add(m_integerValue);
            }

            if (m_uIntegerValue != null)
            {
                children.Add(m_uIntegerValue);
            }

            if (m_vectorValue != null)
            {
                children.Add(m_vectorValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.BooleanValue:
                {
                    if (createOrReplace)
                    {
                        if (BooleanValue == null)
                        {
                            if (replacement == null)
                            {
                                BooleanValue = new BaseDataVariableState<bool>(this);
                            }
                            else
                            {
                                BooleanValue = (BaseDataVariableState<bool>)replacement;
                            }
                        }
                    }

                    instance = BooleanValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new BaseDataVariableState<sbyte>(this);
                            }
                            else
                            {
                                SByteValue = (BaseDataVariableState<sbyte>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new BaseDataVariableState<byte>(this);
                            }
                            else
                            {
                                ByteValue = (BaseDataVariableState<byte>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new BaseDataVariableState<short>(this);
                            }
                            else
                            {
                                Int16Value = (BaseDataVariableState<short>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new BaseDataVariableState<ushort>(this);
                            }
                            else
                            {
                                UInt16Value = (BaseDataVariableState<ushort>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new BaseDataVariableState<int>(this);
                            }
                            else
                            {
                                Int32Value = (BaseDataVariableState<int>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new BaseDataVariableState<uint>(this);
                            }
                            else
                            {
                                UInt32Value = (BaseDataVariableState<uint>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new BaseDataVariableState<long>(this);
                            }
                            else
                            {
                                Int64Value = (BaseDataVariableState<long>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new BaseDataVariableState<ulong>(this);
                            }
                            else
                            {
                                UInt64Value = (BaseDataVariableState<ulong>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new BaseDataVariableState<float>(this);
                            }
                            else
                            {
                                FloatValue = (BaseDataVariableState<float>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new BaseDataVariableState<double>(this);
                            }
                            else
                            {
                                DoubleValue = (BaseDataVariableState<double>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StringValue:
                {
                    if (createOrReplace)
                    {
                        if (StringValue == null)
                        {
                            if (replacement == null)
                            {
                                StringValue = new BaseDataVariableState<string>(this);
                            }
                            else
                            {
                                StringValue = (BaseDataVariableState<string>)replacement;
                            }
                        }
                    }

                    instance = StringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DateTimeValue:
                {
                    if (createOrReplace)
                    {
                        if (DateTimeValue == null)
                        {
                            if (replacement == null)
                            {
                                DateTimeValue = new BaseDataVariableState<DateTime>(this);
                            }
                            else
                            {
                                DateTimeValue = (BaseDataVariableState<DateTime>)replacement;
                            }
                        }
                    }

                    instance = DateTimeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.GuidValue:
                {
                    if (createOrReplace)
                    {
                        if (GuidValue == null)
                        {
                            if (replacement == null)
                            {
                                GuidValue = new BaseDataVariableState<Guid>(this);
                            }
                            else
                            {
                                GuidValue = (BaseDataVariableState<Guid>)replacement;
                            }
                        }
                    }

                    instance = GuidValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteStringValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteStringValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteStringValue = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                ByteStringValue = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ByteStringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.XmlElementValue:
                {
                    if (createOrReplace)
                    {
                        if (XmlElementValue == null)
                        {
                            if (replacement == null)
                            {
                                XmlElementValue = new BaseDataVariableState<XmlElement>(this);
                            }
                            else
                            {
                                XmlElementValue = (BaseDataVariableState<XmlElement>)replacement;
                            }
                        }
                    }

                    instance = XmlElementValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (NodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                NodeIdValue = new BaseDataVariableState<NodeId>(this);
                            }
                            else
                            {
                                NodeIdValue = (BaseDataVariableState<NodeId>)replacement;
                            }
                        }
                    }

                    instance = NodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ExpandedNodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (ExpandedNodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                ExpandedNodeIdValue = new BaseDataVariableState<ExpandedNodeId>(this);
                            }
                            else
                            {
                                ExpandedNodeIdValue = (BaseDataVariableState<ExpandedNodeId>)replacement;
                            }
                        }
                    }

                    instance = ExpandedNodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.QualifiedNameValue:
                {
                    if (createOrReplace)
                    {
                        if (QualifiedNameValue == null)
                        {
                            if (replacement == null)
                            {
                                QualifiedNameValue = new BaseDataVariableState<QualifiedName>(this);
                            }
                            else
                            {
                                QualifiedNameValue = (BaseDataVariableState<QualifiedName>)replacement;
                            }
                        }
                    }

                    instance = QualifiedNameValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.LocalizedTextValue:
                {
                    if (createOrReplace)
                    {
                        if (LocalizedTextValue == null)
                        {
                            if (replacement == null)
                            {
                                LocalizedTextValue = new BaseDataVariableState<LocalizedText>(this);
                            }
                            else
                            {
                                LocalizedTextValue = (BaseDataVariableState<LocalizedText>)replacement;
                            }
                        }
                    }

                    instance = LocalizedTextValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StatusCodeValue:
                {
                    if (createOrReplace)
                    {
                        if (StatusCodeValue == null)
                        {
                            if (replacement == null)
                            {
                                StatusCodeValue = new BaseDataVariableState<StatusCode>(this);
                            }
                            else
                            {
                                StatusCodeValue = (BaseDataVariableState<StatusCode>)replacement;
                            }
                        }
                    }

                    instance = StatusCodeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VariantValue:
                {
                    if (createOrReplace)
                    {
                        if (VariantValue == null)
                        {
                            if (replacement == null)
                            {
                                VariantValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                VariantValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = VariantValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.EnumerationValue:
                {
                    if (createOrReplace)
                    {
                        if (EnumerationValue == null)
                        {
                            if (replacement == null)
                            {
                                EnumerationValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                EnumerationValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = EnumerationValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StructureValue:
                {
                    if (createOrReplace)
                    {
                        if (StructureValue == null)
                        {
                            if (replacement == null)
                            {
                                StructureValue = new BaseDataVariableState<ExtensionObject>(this);
                            }
                            else
                            {
                                StructureValue = (BaseDataVariableState<ExtensionObject>)replacement;
                            }
                        }
                    }

                    instance = StructureValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NumberValue:
                {
                    if (createOrReplace)
                    {
                        if (NumberValue == null)
                        {
                            if (replacement == null)
                            {
                                NumberValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                NumberValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = NumberValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.IntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (IntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                IntegerValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                IntegerValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = IntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UIntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (UIntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                UIntegerValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                UIntegerValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = UIntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VectorValue:
                {
                    if (createOrReplace)
                    {
                        if (VectorValue == null)
                        {
                            if (replacement == null)
                            {
                                VectorValue = new VectorVariableState(this);
                            }
                            else
                            {
                                VectorValue = (VectorVariableState)replacement;
                            }
                        }
                    }

                    instance = VectorValue;
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
        private BaseDataVariableState<bool> m_booleanValue;
        private BaseDataVariableState<sbyte> m_sByteValue;
        private BaseDataVariableState<byte> m_byteValue;
        private BaseDataVariableState<short> m_int16Value;
        private BaseDataVariableState<ushort> m_uInt16Value;
        private BaseDataVariableState<int> m_int32Value;
        private BaseDataVariableState<uint> m_uInt32Value;
        private BaseDataVariableState<long> m_int64Value;
        private BaseDataVariableState<ulong> m_uInt64Value;
        private BaseDataVariableState<float> m_floatValue;
        private BaseDataVariableState<double> m_doubleValue;
        private BaseDataVariableState<string> m_stringValue;
        private BaseDataVariableState<DateTime> m_dateTimeValue;
        private BaseDataVariableState<Guid> m_guidValue;
        private BaseDataVariableState<byte[]> m_byteStringValue;
        private BaseDataVariableState<XmlElement> m_xmlElementValue;
        private BaseDataVariableState<NodeId> m_nodeIdValue;
        private BaseDataVariableState<ExpandedNodeId> m_expandedNodeIdValue;
        private BaseDataVariableState<QualifiedName> m_qualifiedNameValue;
        private BaseDataVariableState<LocalizedText> m_localizedTextValue;
        private BaseDataVariableState<StatusCode> m_statusCodeValue;
        private BaseDataVariableState m_variantValue;
        private BaseDataVariableState m_enumerationValue;
        private BaseDataVariableState<ExtensionObject> m_structureValue;
        private BaseDataVariableState m_numberValue;
        private BaseDataVariableState m_integerValue;
        private BaseDataVariableState m_uIntegerValue;
        private VectorVariableState m_vectorValue;
        #endregion
    }
    #endif
    #endregion

    #region StructureValueObjectState Class
    #if (!OPCUA_EXCLUDE_StructureValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class StructureValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public StructureValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.StructureValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAIAAAAFN0cnVjdHVyZVZhbHVlT2JqZWN0VHlwZUluc3RhbmNlAQHS" +
           "AAEB0gDSAAAAAQAAAAAkAAEB1gAFAAAANWCJCgIAAAABABAAAABTaW11bGF0aW9uQWN0aXZlAQHTAAMA" +
           "AAAARwAAAElmIHRydWUgdGhlIHNlcnZlciB3aWxsIHByb2R1Y2UgbmV3IHZhbHVlcyBmb3IgZWFjaCBt" +
           "b25pdG9yZWQgdmFyaWFibGUuAC4ARNMAAAAAAf////8BAf////8AAAAABGGCCgQAAAABAA4AAABHZW5l" +
           "cmF0ZVZhbHVlcwEB1AAALwEBEQDUAAAAAQH/////AQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVu" +
           "dHMBAdUAAC4ARNUAAACWAQAAAAEAKgEBRgAAAAoAAABJdGVyYXRpb25zAAf/////AAAAAAMAAAAAJQAA" +
           "AFRoZSBudW1iZXIgb2YgbmV3IHZhbHVlcyB0byBnZW5lcmF0ZS4BACgBAQAAAAEAAAAAAAAAAQH/////" +
           "AAAAAARggAoBAAAAAQANAAAAQ3ljbGVDb21wbGV0ZQEB1gAALwEAQQvWAAAAAQAAAAAkAQEB0gAXAAAA" +
           "FWCJCgIAAAAAAAcAAABFdmVudElkAQHXAAAuAETXAAAAAA//////AQH/////AAAAABVgiQoCAAAAAAAJ" +
           "AAAARXZlbnRUeXBlAQHYAAAuAETYAAAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNl" +
           "Tm9kZQEB2QAALgBE2QAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJjZU5hbWUBAdoA" +
           "AC4ARNoAAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAQAAABUaW1lAQHbAAAuAETbAAAAAQAmAf//" +
           "//8BAf////8AAAAAFWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEB3AAALgBE3AAAAAEAJgH/////AQH/" +
           "////AAAAABVgiQoCAAAAAAAHAAAATWVzc2FnZQEB3gAALgBE3gAAAAAV/////wEB/////wAAAAAVYIkK" +
           "AgAAAAAACAAAAFNldmVyaXR5AQHfAAAuAETfAAAAAAX/////AQH/////AAAAABVgiQoCAAAAAAAQAAAA" +
           "Q29uZGl0aW9uQ2xhc3NJZAEB4AAALgBE4AAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAAEgAAAENv" +
           "bmRpdGlvbkNsYXNzTmFtZQEB4QAALgBE4QAAAAAV/////wEB/////wAAAAAVYIkKAgAAAAAADQAAAENv" +
           "bmRpdGlvbk5hbWUBAeQAAC4AROQAAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAgAAABCcmFuY2hJ" +
           "ZAEB5QAALgBE5QAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAABgAAAFJldGFpbgEB5gAALgBE5gAA" +
           "AAAB/////wEB/////wAAAAAVYIkKAgAAAAAADAAAAEVuYWJsZWRTdGF0ZQEB5wAALwEAIyPnAAAAABX/" +
           "////AQECAAAAAQAsIwABAfsAAQAsIwABAQQBAQAAABVgiQoCAAAAAAACAAAASWQBAegAAC4AROgAAAAA" +
           "Af////8BAf////8AAAAAFWCJCgIAAAAAAAcAAABRdWFsaXR5AQHwAAAvAQAqI/AAAAAAE/////8BAf//" +
           "//8BAAAAFWCJCgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAfEAAC4ARPEAAAABACYB/////wEB////" +
           "/wAAAAAVYIkKAgAAAAAADAAAAExhc3RTZXZlcml0eQEB8gAALwEAKiPyAAAAAAX/////AQH/////AQAA" +
           "ABVgiQoCAAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQHzAAAuAETzAAAAAQAmAf////8BAf////8AAAAA" +
           "FWCJCgIAAAAAAAcAAABDb21tZW50AQH0AAAvAQAqI/QAAAAAFf////8BAf////8BAAAAFWCJCgIAAAAA" +
           "AA8AAABTb3VyY2VUaW1lc3RhbXABAfUAAC4ARPUAAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAA" +
           "DAAAAENsaWVudFVzZXJJZAEB9gAALgBE9gAAAAAM/////wEB/////wAAAAAEYYIKBAAAAAAABwAAAERp" +
           "c2FibGUBAfcAAC8BAEQj9wAAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAABgAAAEVuYWJsZQEB" +
           "+AAALwEAQyP4AAAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAKAAAAQWRkQ29tbWVudAEB+QAA" +
           "LwEARSP5AAAAAQEBAAAAAQD5CwABAA0LAQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAfoA" +
           "AC4ARPoAAACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVu" +
           "dGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAA" +
           "AAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAA" +
           "AAAAAQH/////AAAAABVgiQoCAAAAAAAKAAAAQWNrZWRTdGF0ZQEB+wAALwEAIyP7AAAAABX/////AQEB" +
           "AAAAAQAsIwEBAecAAQAAABVgiQoCAAAAAAACAAAASWQBAfwAAC4ARPwAAAAAAf////8BAf////8AAAAA" +
           "BGGCCgQAAAAAAAsAAABBY2tub3dsZWRnZQEBDQEALwEAlyMNAQAAAQEBAAAAAQD5CwABAPAiAQAAABdg" +
           "qQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAQ4BAC4ARA4BAACWAgAAAAEAKgEBRgAAAAcAAABFdmVu" +
           "dElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVu" +
           "dC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAAAAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQg" +
           "dG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAABVgiQoCAAAAAQAPAAAAU2Nh" +
           "bGFyU3RydWN0dXJlAQERAQAvAQFPABEBAAABAU4A/////wEB/////xsAAAAVYIkKAgAAAAEADAAAAEJv" +
           "b2xlYW5WYWx1ZQEBEgEALwA/EgEAAAAB/////wEB/////wAAAAAVYIkKAgAAAAEACgAAAFNCeXRlVmFs" +
           "dWUBARMBAC8APxMBAAAAAv////8BAf////8AAAAAFWCJCgIAAAABAAkAAABCeXRlVmFsdWUBARQBAC8A" +
           "PxQBAAAAA/////8BAf////8AAAAAFWCJCgIAAAABAAoAAABJbnQxNlZhbHVlAQEVAQAvAD8VAQAAAAT/" +
           "////AQH/////AAAAABVgiQoCAAAAAQALAAAAVUludDE2VmFsdWUBARYBAC8APxYBAAAABf////8BAf//" +
           "//8AAAAAFWCJCgIAAAABAAoAAABJbnQzMlZhbHVlAQEXAQAvAD8XAQAAAAb/////AQH/////AAAAABVg" +
           "iQoCAAAAAQALAAAAVUludDMyVmFsdWUBARgBAC8APxgBAAAAB/////8BAf////8AAAAAFWCJCgIAAAAB" +
           "AAoAAABJbnQ2NFZhbHVlAQEZAQAvAD8ZAQAAAAj/////AQH/////AAAAABVgiQoCAAAAAQALAAAAVUlu" +
           "dDY0VmFsdWUBARoBAC8APxoBAAAACf////8BAf////8AAAAAFWCJCgIAAAABAAoAAABGbG9hdFZhbHVl" +
           "AQEbAQAvAD8bAQAAAAr/////AQH/////AAAAABVgiQoCAAAAAQALAAAARG91YmxlVmFsdWUBARwBAC8A" +
           "PxwBAAAAC/////8BAf////8AAAAAFWCJCgIAAAABAAsAAABTdHJpbmdWYWx1ZQEBHQEALwA/HQEAAAAM" +
           "/////wEB/////wAAAAAVYIkKAgAAAAEADQAAAERhdGVUaW1lVmFsdWUBAR4BAC8APx4BAAAADf////8B" +
           "Af////8AAAAAFWCJCgIAAAABAAkAAABHdWlkVmFsdWUBAR8BAC8APx8BAAAADv////8BAf////8AAAAA" +
           "FWCJCgIAAAABAA8AAABCeXRlU3RyaW5nVmFsdWUBASABAC8APyABAAAAD/////8BAf////8AAAAAFWCJ" +
           "CgIAAAABAA8AAABYbWxFbGVtZW50VmFsdWUBASEBAC8APyEBAAAAEP////8BAf////8AAAAAFWCJCgIA" +
           "AAABAAsAAABOb2RlSWRWYWx1ZQEBIgEALwA/IgEAAAAR/////wEB/////wAAAAAVYIkKAgAAAAEAEwAA" +
           "AEV4cGFuZGVkTm9kZUlkVmFsdWUBASMBAC8APyMBAAAAEv////8BAf////8AAAAAFWCJCgIAAAABABIA" +
           "AABRdWFsaWZpZWROYW1lVmFsdWUBASQBAC8APyQBAAAAFP////8BAf////8AAAAAFWCJCgIAAAABABIA" +
           "AABMb2NhbGl6ZWRUZXh0VmFsdWUBASUBAC8APyUBAAAAFf////8BAf////8AAAAAFWCJCgIAAAABAA8A" +
           "AABTdGF0dXNDb2RlVmFsdWUBASYBAC8APyYBAAAAE/////8BAf////8AAAAAFWCJCgIAAAABAAwAAABW" +
           "YXJpYW50VmFsdWUBAScBAC8APycBAAAAGP////8BAf////8AAAAAFWCJCgIAAAABABAAAABFbnVtZXJh" +
           "dGlvblZhbHVlAQEoAQAvAD8oAQAAAB3/////AQH/////AAAAABVgiQoCAAAAAQAOAAAAU3RydWN0dXJl" +
           "VmFsdWUBASkBAC8APykBAAAAFv////8BAf////8AAAAAFWCJCgIAAAABAAsAAABOdW1iZXJWYWx1ZQEB" +
           "KgEALwA/KgEAAAAa/////wEB/////wAAAAAVYIkKAgAAAAEADAAAAEludGVnZXJWYWx1ZQEBKwEALwA/" +
           "KwEAAAAb/////wEB/////wAAAAAVYIkKAgAAAAEADQAAAFVJbnRlZ2VyVmFsdWUBASwBAC8APywBAAAA" +
           "HP////8BAf////8AAAAAFWCJCgIAAAABAA8AAABWZWN0b3JTdHJ1Y3R1cmUBAS0BAC8BAXkDLQEAAAEB" +
           "eAP/////AQH/////AwAAABVgiQoCAAAAAQABAAAAWAEBLgEALgBELgEAAAAL/////wEB/////wAAAAAV" +
           "YIkKAgAAAAEAAQAAAFkBAS8BAC4ARC8BAAAAC/////8BAf////8AAAAAFWCJCgIAAAABAAEAAABaAQEw" +
           "AQAuAEQwAQAAAAv/////AQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public ScalarStructureVariableState ScalarStructure
        {
            get
            {
                return m_scalarStructure;
            }

            set
            {
                if (!Object.ReferenceEquals(m_scalarStructure, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_scalarStructure = value;
            }
        }

        /// <remarks />
        public VectorVariableState VectorStructure
        {
            get
            {
                return m_vectorStructure;
            }

            set
            {
                if (!Object.ReferenceEquals(m_vectorStructure, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_vectorStructure = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_scalarStructure != null)
            {
                children.Add(m_scalarStructure);
            }

            if (m_vectorStructure != null)
            {
                children.Add(m_vectorStructure);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.ScalarStructure:
                {
                    if (createOrReplace)
                    {
                        if (ScalarStructure == null)
                        {
                            if (replacement == null)
                            {
                                ScalarStructure = new ScalarStructureVariableState(this);
                            }
                            else
                            {
                                ScalarStructure = (ScalarStructureVariableState)replacement;
                            }
                        }
                    }

                    instance = ScalarStructure;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VectorStructure:
                {
                    if (createOrReplace)
                    {
                        if (VectorStructure == null)
                        {
                            if (replacement == null)
                            {
                                VectorStructure = new VectorVariableState(this);
                            }
                            else
                            {
                                VectorStructure = (VectorVariableState)replacement;
                            }
                        }
                    }

                    instance = VectorStructure;
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
        private ScalarStructureVariableState m_scalarStructure;
        private VectorVariableState m_vectorStructure;
        #endregion
    }
    #endif
    #endregion

    #region AnalogScalarValueObjectState Class
    #if (!OPCUA_EXCLUDE_AnalogScalarValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class AnalogScalarValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public AnalogScalarValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.AnalogScalarValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAIwAAAEFuYWxvZ1NjYWxhclZhbHVlT2JqZWN0VHlwZUluc3RhbmNl" +
           "AQExAQEBMQExAQAAAQAAAAAkAAEBNQEQAAAANWCJCgIAAAABABAAAABTaW11bGF0aW9uQWN0aXZlAQEy" +
           "AQMAAAAARwAAAElmIHRydWUgdGhlIHNlcnZlciB3aWxsIHByb2R1Y2UgbmV3IHZhbHVlcyBmb3IgZWFj" +
           "aCBtb25pdG9yZWQgdmFyaWFibGUuAC4ARDIBAAAAAf////8BAf////8AAAAABGGCCgQAAAABAA4AAABH" +
           "ZW5lcmF0ZVZhbHVlcwEBMwEALwEBEQAzAQAAAQH/////AQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1" +
           "bWVudHMBATQBAC4ARDQBAACWAQAAAAEAKgEBRgAAAAoAAABJdGVyYXRpb25zAAf/////AAAAAAMAAAAA" +
           "JQAAAFRoZSBudW1iZXIgb2YgbmV3IHZhbHVlcyB0byBnZW5lcmF0ZS4BACgBAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAAARggAoBAAAAAQANAAAAQ3ljbGVDb21wbGV0ZQEBNQEALwEAQQs1AQAAAQAAAAAkAQEBMQEX" +
           "AAAAFWCJCgIAAAAAAAcAAABFdmVudElkAQE2AQAuAEQ2AQAAAA//////AQH/////AAAAABVgiQoCAAAA" +
           "AAAJAAAARXZlbnRUeXBlAQE3AQAuAEQ3AQAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291" +
           "cmNlTm9kZQEBOAEALgBEOAEAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJjZU5hbWUB" +
           "ATkBAC4ARDkBAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAQAAABUaW1lAQE6AQAuAEQ6AQAAAQAm" +
           "Af////8BAf////8AAAAAFWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEBOwEALgBEOwEAAAEAJgH/////" +
           "AQH/////AAAAABVgiQoCAAAAAAAHAAAATWVzc2FnZQEBPQEALgBEPQEAAAAV/////wEB/////wAAAAAV" +
           "YIkKAgAAAAAACAAAAFNldmVyaXR5AQE+AQAuAEQ+AQAAAAX/////AQH/////AAAAABVgiQoCAAAAAAAQ" +
           "AAAAQ29uZGl0aW9uQ2xhc3NJZAEBPwEALgBEPwEAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAAEgAA" +
           "AENvbmRpdGlvbkNsYXNzTmFtZQEBQAEALgBEQAEAAAAV/////wEB/////wAAAAAVYIkKAgAAAAAADQAA" +
           "AENvbmRpdGlvbk5hbWUBAUMBAC4AREMBAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAgAAABCcmFu" +
           "Y2hJZAEBRAEALgBERAEAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAABgAAAFJldGFpbgEBRQEALgBE" +
           "RQEAAAAB/////wEB/////wAAAAAVYIkKAgAAAAAADAAAAEVuYWJsZWRTdGF0ZQEBRgEALwEAIyNGAQAA" +
           "ABX/////AQECAAAAAQAsIwABAVoBAQAsIwABAWMBAQAAABVgiQoCAAAAAAACAAAASWQBAUcBAC4AREcB" +
           "AAAAAf////8BAf////8AAAAAFWCJCgIAAAAAAAcAAABRdWFsaXR5AQFPAQAvAQAqI08BAAAAE/////8B" +
           "Af////8BAAAAFWCJCgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAVABAC4ARFABAAABACYB/////wEB" +
           "/////wAAAAAVYIkKAgAAAAAADAAAAExhc3RTZXZlcml0eQEBUQEALwEAKiNRAQAAAAX/////AQH/////" +
           "AQAAABVgiQoCAAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQFSAQAuAERSAQAAAQAmAf////8BAf////8A" +
           "AAAAFWCJCgIAAAAAAAcAAABDb21tZW50AQFTAQAvAQAqI1MBAAAAFf////8BAf////8BAAAAFWCJCgIA" +
           "AAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAVQBAC4ARFQBAAABACYB/////wEB/////wAAAAAVYIkKAgAA" +
           "AAAADAAAAENsaWVudFVzZXJJZAEBVQEALgBEVQEAAAAM/////wEB/////wAAAAAEYYIKBAAAAAAABwAA" +
           "AERpc2FibGUBAVYBAC8BAEQjVgEAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAABgAAAEVuYWJs" +
           "ZQEBVwEALwEAQyNXAQAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAKAAAAQWRkQ29tbWVudAEB" +
           "WAEALwEARSNYAQAAAQEBAAAAAQD5CwABAA0LAQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMB" +
           "AVkBAC4ARFkBAACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElkAA//////AAAAAAMAAAAAKAAAAFRoZSBp" +
           "ZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4BACoBAUIAAAAHAAAAQ29tbWVudAAV////" +
           "/wAAAAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEA" +
           "AAAAAAAAAQH/////AAAAABVgiQoCAAAAAAAKAAAAQWNrZWRTdGF0ZQEBWgEALwEAIyNaAQAAABX/////" +
           "AQEBAAAAAQAsIwEBAUYBAQAAABVgiQoCAAAAAAACAAAASWQBAVsBAC4ARFsBAAAAAf////8BAf////8A" +
           "AAAABGGCCgQAAAAAAAsAAABBY2tub3dsZWRnZQEBbAEALwEAlyNsAQAAAQEBAAAAAQD5CwABAPAiAQAA" +
           "ABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAW0BAC4ARG0BAACWAgAAAAEAKgEBRgAAAAcAAABF" +
           "dmVudElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29t" +
           "bWVudC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAAAAADAAAAACQAAABUaGUgY29tbWVudCB0byBh" +
           "ZGQgdG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAABVgiQoCAAAAAQAKAAAA" +
           "U0J5dGVWYWx1ZQEBcAEALwEAQAlwAQAAAAL/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5n" +
           "ZQEBdAEALgBEdAEAAAEAdAP/////AQH/////AAAAABVgiQoCAAAAAQAJAAAAQnl0ZVZhbHVlAQF2AQAv" +
           "AQBACXYBAAAAA/////8BAf////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQF6AQAuAER6AQAAAQB0" +
           "A/////8BAf////8AAAAAFWCJCgIAAAABAAoAAABJbnQxNlZhbHVlAQF8AQAvAQBACXwBAAAABP////8B" +
           "Af////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQGAAQAuAESAAQAAAQB0A/////8BAf////8AAAAA" +
           "FWCJCgIAAAABAAsAAABVSW50MTZWYWx1ZQEBggEALwEAQAmCAQAAAAX/////AQH/////AQAAABVgiQoC" +
           "AAAAAAAHAAAARVVSYW5nZQEBhgEALgBEhgEAAAEAdAP/////AQH/////AAAAABVgiQoCAAAAAQAKAAAA" +
           "SW50MzJWYWx1ZQEBiAEALwEAQAmIAQAAAAb/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5n" +
           "ZQEBjAEALgBEjAEAAAEAdAP/////AQH/////AAAAABVgiQoCAAAAAQALAAAAVUludDMyVmFsdWUBAY4B" +
           "AC8BAEAJjgEAAAAH/////wEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAZIBAC4ARJIBAAAB" +
           "AHQD/////wEB/////wAAAAAVYIkKAgAAAAEACgAAAEludDY0VmFsdWUBAZQBAC8BAEAJlAEAAAAI////" +
           "/wEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAZgBAC4ARJgBAAABAHQD/////wEB/////wAA" +
           "AAAVYIkKAgAAAAEACwAAAFVJbnQ2NFZhbHVlAQGaAQAvAQBACZoBAAAACf////8BAf////8BAAAAFWCJ" +
           "CgIAAAAAAAcAAABFVVJhbmdlAQGeAQAuAESeAQAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAABAAoA" +
           "AABGbG9hdFZhbHVlAQGgAQAvAQBACaABAAAACv////8BAf////8BAAAAFWCJCgIAAAAAAAcAAABFVVJh" +
           "bmdlAQGkAQAuAESkAQAAAQB0A/////8BAf////8AAAAAFWCJCgIAAAABAAsAAABEb3VibGVWYWx1ZQEB" +
           "pgEALwEAQAmmAQAAAAv/////AQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBqgEALgBEqgEA" +
           "AAEAdAP/////AQH/////AAAAABVgiQoCAAAAAQALAAAATnVtYmVyVmFsdWUBAawBAC8BAEAJrAEAAAAa" +
           "/////wEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAbABAC4ARLABAAABAHQD/////wEB////" +
           "/wAAAAAVYIkKAgAAAAEADAAAAEludGVnZXJWYWx1ZQEBsgEALwEAQAmyAQAAABv/////AQH/////AQAA" +
           "ABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBtgEALgBEtgEAAAEAdAP/////AQH/////AAAAABVgiQoCAAAA" +
           "AQANAAAAVUludGVnZXJWYWx1ZQEBuAEALwEAQAm4AQAAABz/////AQH/////AQAAABVgiQoCAAAAAAAH" +
           "AAAARVVSYW5nZQEBvAEALgBEvAEAAAEAdAP/////AQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public AnalogItemState<sbyte> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<byte> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<short> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<ushort> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<int> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<uint> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<long> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<ulong> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<float> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<double> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState NumberValue
        {
            get
            {
                return m_numberValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_numberValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_numberValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState IntegerValue
        {
            get
            {
                return m_integerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_integerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_integerValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState UIntegerValue
        {
            get
            {
                return m_uIntegerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uIntegerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uIntegerValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_numberValue != null)
            {
                children.Add(m_numberValue);
            }

            if (m_integerValue != null)
            {
                children.Add(m_integerValue);
            }

            if (m_uIntegerValue != null)
            {
                children.Add(m_uIntegerValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new AnalogItemState<sbyte>(this);
                            }
                            else
                            {
                                SByteValue = (AnalogItemState<sbyte>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new AnalogItemState<byte>(this);
                            }
                            else
                            {
                                ByteValue = (AnalogItemState<byte>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new AnalogItemState<short>(this);
                            }
                            else
                            {
                                Int16Value = (AnalogItemState<short>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new AnalogItemState<ushort>(this);
                            }
                            else
                            {
                                UInt16Value = (AnalogItemState<ushort>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new AnalogItemState<int>(this);
                            }
                            else
                            {
                                Int32Value = (AnalogItemState<int>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new AnalogItemState<uint>(this);
                            }
                            else
                            {
                                UInt32Value = (AnalogItemState<uint>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new AnalogItemState<long>(this);
                            }
                            else
                            {
                                Int64Value = (AnalogItemState<long>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new AnalogItemState<ulong>(this);
                            }
                            else
                            {
                                UInt64Value = (AnalogItemState<ulong>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new AnalogItemState<float>(this);
                            }
                            else
                            {
                                FloatValue = (AnalogItemState<float>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new AnalogItemState<double>(this);
                            }
                            else
                            {
                                DoubleValue = (AnalogItemState<double>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NumberValue:
                {
                    if (createOrReplace)
                    {
                        if (NumberValue == null)
                        {
                            if (replacement == null)
                            {
                                NumberValue = new AnalogItemState(this);
                            }
                            else
                            {
                                NumberValue = (AnalogItemState)replacement;
                            }
                        }
                    }

                    instance = NumberValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.IntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (IntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                IntegerValue = new AnalogItemState(this);
                            }
                            else
                            {
                                IntegerValue = (AnalogItemState)replacement;
                            }
                        }
                    }

                    instance = IntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UIntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (UIntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                UIntegerValue = new AnalogItemState(this);
                            }
                            else
                            {
                                UIntegerValue = (AnalogItemState)replacement;
                            }
                        }
                    }

                    instance = UIntegerValue;
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
        private AnalogItemState<sbyte> m_sByteValue;
        private AnalogItemState<byte> m_byteValue;
        private AnalogItemState<short> m_int16Value;
        private AnalogItemState<ushort> m_uInt16Value;
        private AnalogItemState<int> m_int32Value;
        private AnalogItemState<uint> m_uInt32Value;
        private AnalogItemState<long> m_int64Value;
        private AnalogItemState<ulong> m_uInt64Value;
        private AnalogItemState<float> m_floatValue;
        private AnalogItemState<double> m_doubleValue;
        private AnalogItemState m_numberValue;
        private AnalogItemState m_integerValue;
        private AnalogItemState m_uIntegerValue;
        #endregion
    }
    #endif
    #endregion

    #region ArrayValue1MethodState Class
    #if (!OPCUA_EXCLUDE_ArrayValue1MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ArrayValue1MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public ArrayValue1MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new ArrayValue1MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAFQAAAEFycmF5VmFsdWUxTWV0aG9kVHlwZQEBvwEALwEBvwG/AQAA" +
           "AQH/////AgAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAcABAC4ARMABAACWCwAAAAEAKgEB" +
           "HAAAAAkAAABCb29sZWFuSW4AAQEAAAABAAAAAAAAAAABACoBARoAAAAHAAAAU0J5dGVJbgACAQAAAAEA" +
           "AAAAAAAAAAEAKgEBGQAAAAYAAABCeXRlSW4AAwEAAAABAAAAAAAAAAABACoBARoAAAAHAAAASW50MTZJ" +
           "bgAEAQAAAAEAAAAAAAAAAAEAKgEBGwAAAAgAAABVSW50MTZJbgAFAQAAAAEAAAAAAAAAAAEAKgEBGgAA" +
           "AAcAAABJbnQzMkluAAYBAAAAAQAAAAAAAAAAAQAqAQEbAAAACAAAAFVJbnQzMkluAAcBAAAAAQAAAAAA" +
           "AAAAAQAqAQEaAAAABwAAAEludDY0SW4ACAEAAAABAAAAAAAAAAABACoBARsAAAAIAAAAVUludDY0SW4A" +
           "CQEAAAABAAAAAAAAAAABACoBARoAAAAHAAAARmxvYXRJbgAKAQAAAAEAAAAAAAAAAAEAKgEBGwAAAAgA" +
           "AABEb3VibGVJbgALAQAAAAEAAAAAAAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8AAAAAF2CpCgIAAAAA" +
           "AA8AAABPdXRwdXRBcmd1bWVudHMBAcEBAC4ARMEBAACWCwAAAAEAKgEBHQAAAAoAAABCb29sZWFuT3V0" +
           "AAEBAAAAAQAAAAAAAAAAAQAqAQEbAAAACAAAAFNCeXRlT3V0AAIBAAAAAQAAAAAAAAAAAQAqAQEaAAAA" +
           "BwAAAEJ5dGVPdXQAAwEAAAABAAAAAAAAAAABACoBARsAAAAIAAAASW50MTZPdXQABAEAAAABAAAAAAAA" +
           "AAABACoBARwAAAAJAAAAVUludDE2T3V0AAUBAAAAAQAAAAAAAAAAAQAqAQEbAAAACAAAAEludDMyT3V0" +
           "AAYBAAAAAQAAAAAAAAAAAQAqAQEcAAAACQAAAFVJbnQzMk91dAAHAQAAAAEAAAAAAAAAAAEAKgEBGwAA" +
           "AAgAAABJbnQ2NE91dAAIAQAAAAEAAAAAAAAAAAEAKgEBHAAAAAkAAABVSW50NjRPdXQACQEAAAABAAAA" +
           "AAAAAAABACoBARsAAAAIAAAARmxvYXRPdXQACgEAAAABAAAAAAAAAAABACoBARwAAAAJAAAARG91Ymxl" +
           "T3V0AAsBAAAAAQAAAAAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public ArrayValue1MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            bool[] booleanIn = (bool[])_inputArguments[0];
            sbyte[] sByteIn = (sbyte[])_inputArguments[1];
            byte[] byteIn = (byte[])_inputArguments[2];
            short[] int16In = (short[])_inputArguments[3];
            ushort[] uInt16In = (ushort[])_inputArguments[4];
            int[] int32In = (int[])_inputArguments[5];
            uint[] uInt32In = (uint[])_inputArguments[6];
            long[] int64In = (long[])_inputArguments[7];
            ulong[] uInt64In = (ulong[])_inputArguments[8];
            float[] floatIn = (float[])_inputArguments[9];
            double[] doubleIn = (double[])_inputArguments[10];

            bool[] booleanOut = (bool[])_outputArguments[0];
            sbyte[] sByteOut = (sbyte[])_outputArguments[1];
            byte[] byteOut = (byte[])_outputArguments[2];
            short[] int16Out = (short[])_outputArguments[3];
            ushort[] uInt16Out = (ushort[])_outputArguments[4];
            int[] int32Out = (int[])_outputArguments[5];
            uint[] uInt32Out = (uint[])_outputArguments[6];
            long[] int64Out = (long[])_outputArguments[7];
            ulong[] uInt64Out = (ulong[])_outputArguments[8];
            float[] floatOut = (float[])_outputArguments[9];
            double[] doubleOut = (double[])_outputArguments[10];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    booleanIn,
                    sByteIn,
                    byteIn,
                    int16In,
                    uInt16In,
                    int32In,
                    uInt32In,
                    int64In,
                    uInt64In,
                    floatIn,
                    doubleIn,
                    ref booleanOut,
                    ref sByteOut,
                    ref byteOut,
                    ref int16Out,
                    ref uInt16Out,
                    ref int32Out,
                    ref uInt32Out,
                    ref int64Out,
                    ref uInt64Out,
                    ref floatOut,
                    ref doubleOut);
            }

            _outputArguments[0] = booleanOut;
            _outputArguments[1] = sByteOut;
            _outputArguments[2] = byteOut;
            _outputArguments[3] = int16Out;
            _outputArguments[4] = uInt16Out;
            _outputArguments[5] = int32Out;
            _outputArguments[6] = uInt32Out;
            _outputArguments[7] = int64Out;
            _outputArguments[8] = uInt64Out;
            _outputArguments[9] = floatOut;
            _outputArguments[10] = doubleOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult ArrayValue1MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        bool[] booleanIn,
        sbyte[] sByteIn,
        byte[] byteIn,
        short[] int16In,
        ushort[] uInt16In,
        int[] int32In,
        uint[] uInt32In,
        long[] int64In,
        ulong[] uInt64In,
        float[] floatIn,
        double[] doubleIn,
        ref bool[] booleanOut,
        ref sbyte[] sByteOut,
        ref byte[] byteOut,
        ref short[] int16Out,
        ref ushort[] uInt16Out,
        ref int[] int32Out,
        ref uint[] uInt32Out,
        ref long[] int64Out,
        ref ulong[] uInt64Out,
        ref float[] floatOut,
        ref double[] doubleOut);
    #endif
    #endregion

    #region ArrayValue2MethodState Class
    #if (!OPCUA_EXCLUDE_ArrayValue2MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ArrayValue2MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public ArrayValue2MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new ArrayValue2MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAFQAAAEFycmF5VmFsdWUyTWV0aG9kVHlwZQEBwgEALwEBwgHCAQAA" +
           "AQH/////AgAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAcMBAC4ARMMBAACWCgAAAAEAKgEB" +
           "GwAAAAgAAABTdHJpbmdJbgAMAQAAAAEAAAAAAAAAAAEAKgEBHQAAAAoAAABEYXRlVGltZUluAA0BAAAA" +
           "AQAAAAAAAAAAAQAqAQEZAAAABgAAAEd1aWRJbgAOAQAAAAEAAAAAAAAAAAEAKgEBHwAAAAwAAABCeXRl" +
           "U3RyaW5nSW4ADwEAAAABAAAAAAAAAAABACoBAR8AAAAMAAAAWG1sRWxlbWVudEluABABAAAAAQAAAAAA" +
           "AAAAAQAqAQEbAAAACAAAAE5vZGVJZEluABEBAAAAAQAAAAAAAAAAAQAqAQEjAAAAEAAAAEV4cGFuZGVk" +
           "Tm9kZUlkSW4AEgEAAAABAAAAAAAAAAABACoBASIAAAAPAAAAUXVhbGlmaWVkTmFtZUluABQBAAAAAQAA" +
           "AAAAAAAAAQAqAQEiAAAADwAAAExvY2FsaXplZFRleHRJbgAVAQAAAAEAAAAAAAAAAAEAKgEBHwAAAAwA" +
           "AABTdGF0dXNDb2RlSW4AEwEAAAABAAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAABdgqQoC" +
           "AAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQHEAQAuAETEAQAAlgoAAAABACoBARwAAAAJAAAAU3RyaW5n" +
           "T3V0AAwBAAAAAQAAAAAAAAAAAQAqAQEeAAAACwAAAERhdGVUaW1lT3V0AA0BAAAAAQAAAAAAAAAAAQAq" +
           "AQEaAAAABwAAAEd1aWRPdXQADgEAAAABAAAAAAAAAAABACoBASAAAAANAAAAQnl0ZVN0cmluZ091dAAP" +
           "AQAAAAEAAAAAAAAAAAEAKgEBIAAAAA0AAABYbWxFbGVtZW50T3V0ABABAAAAAQAAAAAAAAAAAQAqAQEc" +
           "AAAACQAAAE5vZGVJZE91dAARAQAAAAEAAAAAAAAAAAEAKgEBJAAAABEAAABFeHBhbmRlZE5vZGVJZE91" +
           "dAASAQAAAAEAAAAAAAAAAAEAKgEBIwAAABAAAABRdWFsaWZpZWROYW1lT3V0ABQBAAAAAQAAAAAAAAAA" +
           "AQAqAQEjAAAAEAAAAExvY2FsaXplZFRleHRPdXQAFQEAAAABAAAAAAAAAAABACoBASAAAAANAAAAU3Rh" +
           "dHVzQ29kZU91dAATAQAAAAEAAAAAAAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public ArrayValue2MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            string[] stringIn = (string[])_inputArguments[0];
            DateTime[] dateTimeIn = (DateTime[])_inputArguments[1];
            Uuid[] guidIn = (Uuid[])_inputArguments[2];
            byte[][] byteStringIn = (byte[][])_inputArguments[3];
            XmlElement[] xmlElementIn = (XmlElement[])_inputArguments[4];
            NodeId[] nodeIdIn = (NodeId[])_inputArguments[5];
            ExpandedNodeId[] expandedNodeIdIn = (ExpandedNodeId[])_inputArguments[6];
            QualifiedName[] qualifiedNameIn = (QualifiedName[])_inputArguments[7];
            LocalizedText[] localizedTextIn = (LocalizedText[])_inputArguments[8];
            StatusCode[] statusCodeIn = (StatusCode[])_inputArguments[9];

            string[] stringOut = (string[])_outputArguments[0];
            DateTime[] dateTimeOut = (DateTime[])_outputArguments[1];
            Uuid[] guidOut = (Uuid[])_outputArguments[2];
            byte[][] byteStringOut = (byte[][])_outputArguments[3];
            XmlElement[] xmlElementOut = (XmlElement[])_outputArguments[4];
            NodeId[] nodeIdOut = (NodeId[])_outputArguments[5];
            ExpandedNodeId[] expandedNodeIdOut = (ExpandedNodeId[])_outputArguments[6];
            QualifiedName[] qualifiedNameOut = (QualifiedName[])_outputArguments[7];
            LocalizedText[] localizedTextOut = (LocalizedText[])_outputArguments[8];
            StatusCode[] statusCodeOut = (StatusCode[])_outputArguments[9];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    stringIn,
                    dateTimeIn,
                    guidIn,
                    byteStringIn,
                    xmlElementIn,
                    nodeIdIn,
                    expandedNodeIdIn,
                    qualifiedNameIn,
                    localizedTextIn,
                    statusCodeIn,
                    ref stringOut,
                    ref dateTimeOut,
                    ref guidOut,
                    ref byteStringOut,
                    ref xmlElementOut,
                    ref nodeIdOut,
                    ref expandedNodeIdOut,
                    ref qualifiedNameOut,
                    ref localizedTextOut,
                    ref statusCodeOut);
            }

            _outputArguments[0] = stringOut;
            _outputArguments[1] = dateTimeOut;
            _outputArguments[2] = guidOut;
            _outputArguments[3] = byteStringOut;
            _outputArguments[4] = xmlElementOut;
            _outputArguments[5] = nodeIdOut;
            _outputArguments[6] = expandedNodeIdOut;
            _outputArguments[7] = qualifiedNameOut;
            _outputArguments[8] = localizedTextOut;
            _outputArguments[9] = statusCodeOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult ArrayValue2MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        string[] stringIn,
        DateTime[] dateTimeIn,
        Uuid[] guidIn,
        byte[][] byteStringIn,
        XmlElement[] xmlElementIn,
        NodeId[] nodeIdIn,
        ExpandedNodeId[] expandedNodeIdIn,
        QualifiedName[] qualifiedNameIn,
        LocalizedText[] localizedTextIn,
        StatusCode[] statusCodeIn,
        ref string[] stringOut,
        ref DateTime[] dateTimeOut,
        ref Uuid[] guidOut,
        ref byte[][] byteStringOut,
        ref XmlElement[] xmlElementOut,
        ref NodeId[] nodeIdOut,
        ref ExpandedNodeId[] expandedNodeIdOut,
        ref QualifiedName[] qualifiedNameOut,
        ref LocalizedText[] localizedTextOut,
        ref StatusCode[] statusCodeOut);
    #endif
    #endregion

    #region ArrayValue3MethodState Class
    #if (!OPCUA_EXCLUDE_ArrayValue3MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ArrayValue3MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public ArrayValue3MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new ArrayValue3MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAFQAAAEFycmF5VmFsdWUzTWV0aG9kVHlwZQEBxQEALwEBxQHFAQAA" +
           "AQH/////AgAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAcYBAC4ARMYBAACWAwAAAAEAKgEB" +
           "HAAAAAkAAABWYXJpYW50SW4AGAEAAAABAAAAAAAAAAABACoBASAAAAANAAAARW51bWVyYXRpb25JbgAd" +
           "AQAAAAEAAAAAAAAAAAEAKgEBHgAAAAsAAABTdHJ1Y3R1cmVJbgAWAQAAAAEAAAAAAAAAAAEAKAEBAAAA" +
           "AQAAAAAAAAABAf////8AAAAAF2CpCgIAAAAAAA8AAABPdXRwdXRBcmd1bWVudHMBAccBAC4ARMcBAACW" +
           "AwAAAAEAKgEBHQAAAAoAAABWYXJpYW50T3V0ABgBAAAAAQAAAAAAAAAAAQAqAQEhAAAADgAAAEVudW1l" +
           "cmF0aW9uT3V0AB0BAAAAAQAAAAAAAAAAAQAqAQEfAAAADAAAAFN0cnVjdHVyZU91dAAWAQAAAAEAAAAA" +
           "AAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public ArrayValue3MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            Variant[] variantIn = (Variant[])_inputArguments[0];
            int[] enumerationIn = (int[])_inputArguments[1];
            ExtensionObject[] structureIn = (ExtensionObject[])_inputArguments[2];

            Variant[] variantOut = (Variant[])_outputArguments[0];
            int[] enumerationOut = (int[])_outputArguments[1];
            ExtensionObject[] structureOut = (ExtensionObject[])_outputArguments[2];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    variantIn,
                    enumerationIn,
                    structureIn,
                    ref variantOut,
                    ref enumerationOut,
                    ref structureOut);
            }

            _outputArguments[0] = variantOut;
            _outputArguments[1] = enumerationOut;
            _outputArguments[2] = structureOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult ArrayValue3MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        Variant[] variantIn,
        int[] enumerationIn,
        ExtensionObject[] structureIn,
        ref Variant[] variantOut,
        ref int[] enumerationOut,
        ref ExtensionObject[] structureOut);
    #endif
    #endregion

    #region ArrayValueObjectState Class
    #if (!OPCUA_EXCLUDE_ArrayValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ArrayValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public ArrayValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.ArrayValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAHAAAAEFycmF5VmFsdWVPYmplY3RUeXBlSW5zdGFuY2UBAcgBAQHI" +
           "AcgBAAABAAAAACQAAQHMAR8AAAA1YIkKAgAAAAEAEAAAAFNpbXVsYXRpb25BY3RpdmUBAckBAwAAAABH" +
           "AAAASWYgdHJ1ZSB0aGUgc2VydmVyIHdpbGwgcHJvZHVjZSBuZXcgdmFsdWVzIGZvciBlYWNoIG1vbml0" +
           "b3JlZCB2YXJpYWJsZS4ALgBEyQEAAAAB/////wEB/////wAAAAAEYYIKBAAAAAEADgAAAEdlbmVyYXRl" +
           "VmFsdWVzAQHKAQAvAQERAMoBAAABAf////8BAAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEB" +
           "ywEALgBEywEAAJYBAAAAAQAqAQFGAAAACgAAAEl0ZXJhdGlvbnMAB/////8AAAAAAwAAAAAlAAAAVGhl" +
           "IG51bWJlciBvZiBuZXcgdmFsdWVzIHRvIGdlbmVyYXRlLgEAKAEBAAAAAQAAAAAAAAABAf////8AAAAA" +
           "BGCACgEAAAABAA0AAABDeWNsZUNvbXBsZXRlAQHMAQAvAQBBC8wBAAABAAAAACQBAQHIARcAAAAVYIkK" +
           "AgAAAAAABwAAAEV2ZW50SWQBAc0BAC4ARM0BAAAAD/////8BAf////8AAAAAFWCJCgIAAAAAAAkAAABF" +
           "dmVudFR5cGUBAc4BAC4ARM4BAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoAAABTb3VyY2VOb2Rl" +
           "AQHPAQAuAETPAQAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNlTmFtZQEB0AEALgBE" +
           "0AEAAAAM/////wEB/////wAAAAAVYIkKAgAAAAAABAAAAFRpbWUBAdEBAC4ARNEBAAABACYB/////wEB" +
           "/////wAAAAAVYIkKAgAAAAAACwAAAFJlY2VpdmVUaW1lAQHSAQAuAETSAQAAAQAmAf////8BAf////8A" +
           "AAAAFWCJCgIAAAAAAAcAAABNZXNzYWdlAQHUAQAuAETUAQAAABX/////AQH/////AAAAABVgiQoCAAAA" +
           "AAAIAAAAU2V2ZXJpdHkBAdUBAC4ARNUBAAAABf////8BAf////8AAAAAFWCJCgIAAAAAABAAAABDb25k" +
           "aXRpb25DbGFzc0lkAQHWAQAuAETWAQAAABH/////AQH/////AAAAABVgiQoCAAAAAAASAAAAQ29uZGl0" +
           "aW9uQ2xhc3NOYW1lAQHXAQAuAETXAQAAABX/////AQH/////AAAAABVgiQoCAAAAAAANAAAAQ29uZGl0" +
           "aW9uTmFtZQEB2gEALgBE2gEAAAAM/////wEB/////wAAAAAVYIkKAgAAAAAACAAAAEJyYW5jaElkAQHb" +
           "AQAuAETbAQAAABH/////AQH/////AAAAABVgiQoCAAAAAAAGAAAAUmV0YWluAQHcAQAuAETcAQAAAAH/" +
           "////AQH/////AAAAABVgiQoCAAAAAAAMAAAARW5hYmxlZFN0YXRlAQHdAQAvAQAjI90BAAAAFf////8B" +
           "AQIAAAABACwjAAEB8QEBACwjAAEB+gEBAAAAFWCJCgIAAAAAAAIAAABJZAEB3gEALgBE3gEAAAAB////" +
           "/wEB/////wAAAAAVYIkKAgAAAAAABwAAAFF1YWxpdHkBAeYBAC8BACoj5gEAAAAT/////wEB/////wEA" +
           "AAAVYIkKAgAAAAAADwAAAFNvdXJjZVRpbWVzdGFtcAEB5wEALgBE5wEAAAEAJgH/////AQH/////AAAA" +
           "ABVgiQoCAAAAAAAMAAAATGFzdFNldmVyaXR5AQHoAQAvAQAqI+gBAAAABf////8BAf////8BAAAAFWCJ" +
           "CgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAekBAC4AROkBAAABACYB/////wEB/////wAAAAAVYIkK" +
           "AgAAAAAABwAAAENvbW1lbnQBAeoBAC8BACoj6gEAAAAV/////wEB/////wEAAAAVYIkKAgAAAAAADwAA" +
           "AFNvdXJjZVRpbWVzdGFtcAEB6wEALgBE6wEAAAEAJgH/////AQH/////AAAAABVgiQoCAAAAAAAMAAAA" +
           "Q2xpZW50VXNlcklkAQHsAQAuAETsAQAAAAz/////AQH/////AAAAAARhggoEAAAAAAAHAAAARGlzYWJs" +
           "ZQEB7QEALwEARCPtAQAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAGAAAARW5hYmxlAQHuAQAv" +
           "AQBDI+4BAAABAQEAAAABAPkLAAEA8woAAAAABGGCCgQAAAAAAAoAAABBZGRDb21tZW50AQHvAQAvAQBF" +
           "I+8BAAABAQEAAAABAPkLAAEADQsBAAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEB8AEALgBE" +
           "8AEAAJYCAAAAAQAqAQFGAAAABwAAAEV2ZW50SWQAD/////8AAAAAAwAAAAAoAAAAVGhlIGlkZW50aWZp" +
           "ZXIgZm9yIHRoZSBldmVudCB0byBjb21tZW50LgEAKgEBQgAAAAcAAABDb21tZW50ABX/////AAAAAAMA" +
           "AAAAJAAAAFRoZSBjb21tZW50IHRvIGFkZCB0byB0aGUgY29uZGl0aW9uLgEAKAEBAAAAAQAAAAAAAAAB" +
           "Af////8AAAAAFWCJCgIAAAAAAAoAAABBY2tlZFN0YXRlAQHxAQAvAQAjI/EBAAAAFf////8BAQEAAAAB" +
           "ACwjAQEB3QEBAAAAFWCJCgIAAAAAAAIAAABJZAEB8gEALgBE8gEAAAAB/////wEB/////wAAAAAEYYIK" +
           "BAAAAAAACwAAAEFja25vd2xlZGdlAQEDAgAvAQCXIwMCAAABAQEAAAABAPkLAAEA8CIBAAAAF2CpCgIA" +
           "AAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBBAIALgBEBAIAAJYCAAAAAQAqAQFGAAAABwAAAEV2ZW50SWQA" +
           "D/////8AAAAAAwAAAAAoAAAAVGhlIGlkZW50aWZpZXIgZm9yIHRoZSBldmVudCB0byBjb21tZW50LgEA" +
           "KgEBQgAAAAcAAABDb21tZW50ABX/////AAAAAAMAAAAAJAAAAFRoZSBjb21tZW50IHRvIGFkZCB0byB0" +
           "aGUgY29uZGl0aW9uLgEAKAEBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAwAAABCb29sZWFu" +
           "VmFsdWUBAQcCAC8APwcCAAAAAQEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEACgAAAFNCeXRl" +
           "VmFsdWUBAQgCAC8APwgCAAAAAgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEACQAAAEJ5dGVW" +
           "YWx1ZQEBCQIALwA/CQIAAAADAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQAKAAAASW50MTZW" +
           "YWx1ZQEBCgIALwA/CgIAAAAEAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQALAAAAVUludDE2" +
           "VmFsdWUBAQsCAC8APwsCAAAABQEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEACgAAAEludDMy" +
           "VmFsdWUBAQwCAC8APwwCAAAABgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEACwAAAFVJbnQz" +
           "MlZhbHVlAQENAgAvAD8NAgAAAAcBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAoAAABJbnQ2" +
           "NFZhbHVlAQEOAgAvAD8OAgAAAAgBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAsAAABVSW50" +
           "NjRWYWx1ZQEBDwIALwA/DwIAAAAJAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQAKAAAARmxv" +
           "YXRWYWx1ZQEBEAIALwA/EAIAAAAKAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQALAAAARG91" +
           "YmxlVmFsdWUBARECAC8APxECAAAACwEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEACwAAAFN0" +
           "cmluZ1ZhbHVlAQESAgAvAD8SAgAAAAwBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAA0AAABE" +
           "YXRlVGltZVZhbHVlAQETAgAvAD8TAgAAAA0BAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAkA" +
           "AABHdWlkVmFsdWUBARQCAC8APxQCAAAADgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEADwAA" +
           "AEJ5dGVTdHJpbmdWYWx1ZQEBFQIALwA/FQIAAAAPAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAA" +
           "AQAPAAAAWG1sRWxlbWVudFZhbHVlAQEWAgAvAD8WAgAAABABAAAAAQAAAAAAAAABAf////8AAAAAF2CJ" +
           "CgIAAAABAAsAAABOb2RlSWRWYWx1ZQEBFwIALwA/FwIAAAARAQAAAAEAAAAAAAAAAQH/////AAAAABdg" +
           "iQoCAAAAAQATAAAARXhwYW5kZWROb2RlSWRWYWx1ZQEBGAIALwA/GAIAAAASAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAABdgiQoCAAAAAQASAAAAUXVhbGlmaWVkTmFtZVZhbHVlAQEZAgAvAD8ZAgAAABQBAAAAAQAA" +
           "AAAAAAABAf////8AAAAAF2CJCgIAAAABABIAAABMb2NhbGl6ZWRUZXh0VmFsdWUBARoCAC8APxoCAAAA" +
           "FQEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEADwAAAFN0YXR1c0NvZGVWYWx1ZQEBGwIALwA/" +
           "GwIAAAATAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQAMAAAAVmFyaWFudFZhbHVlAQEcAgAv" +
           "AD8cAgAAABgBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABABAAAABFbnVtZXJhdGlvblZhbHVl" +
           "AQEdAgAvAD8dAgAAAB0BAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAA4AAABTdHJ1Y3R1cmVW" +
           "YWx1ZQEBHgIALwA/HgIAAAAWAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQALAAAATnVtYmVy" +
           "VmFsdWUBAR8CAC8APx8CAAAAGgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEADAAAAEludGVn" +
           "ZXJWYWx1ZQEBIAIALwA/IAIAAAAbAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQANAAAAVUlu" +
           "dGVnZXJWYWx1ZQEBIQIALwA/IQIAAAAcAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQALAAAA" +
           "VmVjdG9yVmFsdWUBASICAC8APyICAAABAXgDAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public BaseDataVariableState<bool[]> BooleanValue
        {
            get
            {
                return m_booleanValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_booleanValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_booleanValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<sbyte[]> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[]> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<short[]> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ushort[]> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<int[]> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<uint[]> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<long[]> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ulong[]> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<float[]> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<double[]> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<string[]> StringValue
        {
            get
            {
                return m_stringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_stringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_stringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<DateTime[]> DateTimeValue
        {
            get
            {
                return m_dateTimeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_dateTimeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_dateTimeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<Guid[]> GuidValue
        {
            get
            {
                return m_guidValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_guidValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_guidValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[][]> ByteStringValue
        {
            get
            {
                return m_byteStringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteStringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteStringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<XmlElement[]> XmlElementValue
        {
            get
            {
                return m_xmlElementValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_xmlElementValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_xmlElementValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<NodeId[]> NodeIdValue
        {
            get
            {
                return m_nodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_nodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_nodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExpandedNodeId[]> ExpandedNodeIdValue
        {
            get
            {
                return m_expandedNodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_expandedNodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_expandedNodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<QualifiedName[]> QualifiedNameValue
        {
            get
            {
                return m_qualifiedNameValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_qualifiedNameValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_qualifiedNameValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<LocalizedText[]> LocalizedTextValue
        {
            get
            {
                return m_localizedTextValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_localizedTextValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_localizedTextValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<StatusCode[]> StatusCodeValue
        {
            get
            {
                return m_statusCodeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_statusCodeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_statusCodeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<object[]> VariantValue
        {
            get
            {
                return m_variantValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_variantValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_variantValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<int[]> EnumerationValue
        {
            get
            {
                return m_enumerationValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_enumerationValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_enumerationValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExtensionObject[]> StructureValue
        {
            get
            {
                return m_structureValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_structureValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_structureValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<object[]> NumberValue
        {
            get
            {
                return m_numberValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_numberValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_numberValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<object[]> IntegerValue
        {
            get
            {
                return m_integerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_integerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_integerValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<object[]> UIntegerValue
        {
            get
            {
                return m_uIntegerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uIntegerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uIntegerValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<Vector[]> VectorValue
        {
            get
            {
                return m_vectorValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_vectorValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_vectorValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_booleanValue != null)
            {
                children.Add(m_booleanValue);
            }

            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_stringValue != null)
            {
                children.Add(m_stringValue);
            }

            if (m_dateTimeValue != null)
            {
                children.Add(m_dateTimeValue);
            }

            if (m_guidValue != null)
            {
                children.Add(m_guidValue);
            }

            if (m_byteStringValue != null)
            {
                children.Add(m_byteStringValue);
            }

            if (m_xmlElementValue != null)
            {
                children.Add(m_xmlElementValue);
            }

            if (m_nodeIdValue != null)
            {
                children.Add(m_nodeIdValue);
            }

            if (m_expandedNodeIdValue != null)
            {
                children.Add(m_expandedNodeIdValue);
            }

            if (m_qualifiedNameValue != null)
            {
                children.Add(m_qualifiedNameValue);
            }

            if (m_localizedTextValue != null)
            {
                children.Add(m_localizedTextValue);
            }

            if (m_statusCodeValue != null)
            {
                children.Add(m_statusCodeValue);
            }

            if (m_variantValue != null)
            {
                children.Add(m_variantValue);
            }

            if (m_enumerationValue != null)
            {
                children.Add(m_enumerationValue);
            }

            if (m_structureValue != null)
            {
                children.Add(m_structureValue);
            }

            if (m_numberValue != null)
            {
                children.Add(m_numberValue);
            }

            if (m_integerValue != null)
            {
                children.Add(m_integerValue);
            }

            if (m_uIntegerValue != null)
            {
                children.Add(m_uIntegerValue);
            }

            if (m_vectorValue != null)
            {
                children.Add(m_vectorValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.BooleanValue:
                {
                    if (createOrReplace)
                    {
                        if (BooleanValue == null)
                        {
                            if (replacement == null)
                            {
                                BooleanValue = new BaseDataVariableState<bool[]>(this);
                            }
                            else
                            {
                                BooleanValue = (BaseDataVariableState<bool[]>)replacement;
                            }
                        }
                    }

                    instance = BooleanValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new BaseDataVariableState<sbyte[]>(this);
                            }
                            else
                            {
                                SByteValue = (BaseDataVariableState<sbyte[]>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                ByteValue = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new BaseDataVariableState<short[]>(this);
                            }
                            else
                            {
                                Int16Value = (BaseDataVariableState<short[]>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new BaseDataVariableState<ushort[]>(this);
                            }
                            else
                            {
                                UInt16Value = (BaseDataVariableState<ushort[]>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new BaseDataVariableState<int[]>(this);
                            }
                            else
                            {
                                Int32Value = (BaseDataVariableState<int[]>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new BaseDataVariableState<uint[]>(this);
                            }
                            else
                            {
                                UInt32Value = (BaseDataVariableState<uint[]>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new BaseDataVariableState<long[]>(this);
                            }
                            else
                            {
                                Int64Value = (BaseDataVariableState<long[]>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new BaseDataVariableState<ulong[]>(this);
                            }
                            else
                            {
                                UInt64Value = (BaseDataVariableState<ulong[]>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new BaseDataVariableState<float[]>(this);
                            }
                            else
                            {
                                FloatValue = (BaseDataVariableState<float[]>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new BaseDataVariableState<double[]>(this);
                            }
                            else
                            {
                                DoubleValue = (BaseDataVariableState<double[]>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StringValue:
                {
                    if (createOrReplace)
                    {
                        if (StringValue == null)
                        {
                            if (replacement == null)
                            {
                                StringValue = new BaseDataVariableState<string[]>(this);
                            }
                            else
                            {
                                StringValue = (BaseDataVariableState<string[]>)replacement;
                            }
                        }
                    }

                    instance = StringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DateTimeValue:
                {
                    if (createOrReplace)
                    {
                        if (DateTimeValue == null)
                        {
                            if (replacement == null)
                            {
                                DateTimeValue = new BaseDataVariableState<DateTime[]>(this);
                            }
                            else
                            {
                                DateTimeValue = (BaseDataVariableState<DateTime[]>)replacement;
                            }
                        }
                    }

                    instance = DateTimeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.GuidValue:
                {
                    if (createOrReplace)
                    {
                        if (GuidValue == null)
                        {
                            if (replacement == null)
                            {
                                GuidValue = new BaseDataVariableState<Guid[]>(this);
                            }
                            else
                            {
                                GuidValue = (BaseDataVariableState<Guid[]>)replacement;
                            }
                        }
                    }

                    instance = GuidValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteStringValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteStringValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteStringValue = new BaseDataVariableState<byte[][]>(this);
                            }
                            else
                            {
                                ByteStringValue = (BaseDataVariableState<byte[][]>)replacement;
                            }
                        }
                    }

                    instance = ByteStringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.XmlElementValue:
                {
                    if (createOrReplace)
                    {
                        if (XmlElementValue == null)
                        {
                            if (replacement == null)
                            {
                                XmlElementValue = new BaseDataVariableState<XmlElement[]>(this);
                            }
                            else
                            {
                                XmlElementValue = (BaseDataVariableState<XmlElement[]>)replacement;
                            }
                        }
                    }

                    instance = XmlElementValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (NodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                NodeIdValue = new BaseDataVariableState<NodeId[]>(this);
                            }
                            else
                            {
                                NodeIdValue = (BaseDataVariableState<NodeId[]>)replacement;
                            }
                        }
                    }

                    instance = NodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ExpandedNodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (ExpandedNodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                ExpandedNodeIdValue = new BaseDataVariableState<ExpandedNodeId[]>(this);
                            }
                            else
                            {
                                ExpandedNodeIdValue = (BaseDataVariableState<ExpandedNodeId[]>)replacement;
                            }
                        }
                    }

                    instance = ExpandedNodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.QualifiedNameValue:
                {
                    if (createOrReplace)
                    {
                        if (QualifiedNameValue == null)
                        {
                            if (replacement == null)
                            {
                                QualifiedNameValue = new BaseDataVariableState<QualifiedName[]>(this);
                            }
                            else
                            {
                                QualifiedNameValue = (BaseDataVariableState<QualifiedName[]>)replacement;
                            }
                        }
                    }

                    instance = QualifiedNameValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.LocalizedTextValue:
                {
                    if (createOrReplace)
                    {
                        if (LocalizedTextValue == null)
                        {
                            if (replacement == null)
                            {
                                LocalizedTextValue = new BaseDataVariableState<LocalizedText[]>(this);
                            }
                            else
                            {
                                LocalizedTextValue = (BaseDataVariableState<LocalizedText[]>)replacement;
                            }
                        }
                    }

                    instance = LocalizedTextValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StatusCodeValue:
                {
                    if (createOrReplace)
                    {
                        if (StatusCodeValue == null)
                        {
                            if (replacement == null)
                            {
                                StatusCodeValue = new BaseDataVariableState<StatusCode[]>(this);
                            }
                            else
                            {
                                StatusCodeValue = (BaseDataVariableState<StatusCode[]>)replacement;
                            }
                        }
                    }

                    instance = StatusCodeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VariantValue:
                {
                    if (createOrReplace)
                    {
                        if (VariantValue == null)
                        {
                            if (replacement == null)
                            {
                                VariantValue = new BaseDataVariableState<object[]>(this);
                            }
                            else
                            {
                                VariantValue = (BaseDataVariableState<object[]>)replacement;
                            }
                        }
                    }

                    instance = VariantValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.EnumerationValue:
                {
                    if (createOrReplace)
                    {
                        if (EnumerationValue == null)
                        {
                            if (replacement == null)
                            {
                                EnumerationValue = new BaseDataVariableState<int[]>(this);
                            }
                            else
                            {
                                EnumerationValue = (BaseDataVariableState<int[]>)replacement;
                            }
                        }
                    }

                    instance = EnumerationValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StructureValue:
                {
                    if (createOrReplace)
                    {
                        if (StructureValue == null)
                        {
                            if (replacement == null)
                            {
                                StructureValue = new BaseDataVariableState<ExtensionObject[]>(this);
                            }
                            else
                            {
                                StructureValue = (BaseDataVariableState<ExtensionObject[]>)replacement;
                            }
                        }
                    }

                    instance = StructureValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NumberValue:
                {
                    if (createOrReplace)
                    {
                        if (NumberValue == null)
                        {
                            if (replacement == null)
                            {
                                NumberValue = new BaseDataVariableState<object[]>(this);
                            }
                            else
                            {
                                NumberValue = (BaseDataVariableState<object[]>)replacement;
                            }
                        }
                    }

                    instance = NumberValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.IntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (IntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                IntegerValue = new BaseDataVariableState<object[]>(this);
                            }
                            else
                            {
                                IntegerValue = (BaseDataVariableState<object[]>)replacement;
                            }
                        }
                    }

                    instance = IntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UIntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (UIntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                UIntegerValue = new BaseDataVariableState<object[]>(this);
                            }
                            else
                            {
                                UIntegerValue = (BaseDataVariableState<object[]>)replacement;
                            }
                        }
                    }

                    instance = UIntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VectorValue:
                {
                    if (createOrReplace)
                    {
                        if (VectorValue == null)
                        {
                            if (replacement == null)
                            {
                                VectorValue = new BaseDataVariableState<Vector[]>(this);
                            }
                            else
                            {
                                VectorValue = (BaseDataVariableState<Vector[]>)replacement;
                            }
                        }
                    }

                    instance = VectorValue;
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
        private BaseDataVariableState<bool[]> m_booleanValue;
        private BaseDataVariableState<sbyte[]> m_sByteValue;
        private BaseDataVariableState<byte[]> m_byteValue;
        private BaseDataVariableState<short[]> m_int16Value;
        private BaseDataVariableState<ushort[]> m_uInt16Value;
        private BaseDataVariableState<int[]> m_int32Value;
        private BaseDataVariableState<uint[]> m_uInt32Value;
        private BaseDataVariableState<long[]> m_int64Value;
        private BaseDataVariableState<ulong[]> m_uInt64Value;
        private BaseDataVariableState<float[]> m_floatValue;
        private BaseDataVariableState<double[]> m_doubleValue;
        private BaseDataVariableState<string[]> m_stringValue;
        private BaseDataVariableState<DateTime[]> m_dateTimeValue;
        private BaseDataVariableState<Guid[]> m_guidValue;
        private BaseDataVariableState<byte[][]> m_byteStringValue;
        private BaseDataVariableState<XmlElement[]> m_xmlElementValue;
        private BaseDataVariableState<NodeId[]> m_nodeIdValue;
        private BaseDataVariableState<ExpandedNodeId[]> m_expandedNodeIdValue;
        private BaseDataVariableState<QualifiedName[]> m_qualifiedNameValue;
        private BaseDataVariableState<LocalizedText[]> m_localizedTextValue;
        private BaseDataVariableState<StatusCode[]> m_statusCodeValue;
        private BaseDataVariableState<object[]> m_variantValue;
        private BaseDataVariableState<int[]> m_enumerationValue;
        private BaseDataVariableState<ExtensionObject[]> m_structureValue;
        private BaseDataVariableState<object[]> m_numberValue;
        private BaseDataVariableState<object[]> m_integerValue;
        private BaseDataVariableState<object[]> m_uIntegerValue;
        private BaseDataVariableState<Vector[]> m_vectorValue;
        #endregion
    }
    #endif
    #endregion

    #region AnalogArrayValueObjectState Class
    #if (!OPCUA_EXCLUDE_AnalogArrayValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class AnalogArrayValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public AnalogArrayValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.AnalogArrayValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAIgAAAEFuYWxvZ0FycmF5VmFsdWVPYmplY3RUeXBlSW5zdGFuY2UB" +
           "ASMCAQEjAiMCAAABAAAAACQAAQEnAhAAAAA1YIkKAgAAAAEAEAAAAFNpbXVsYXRpb25BY3RpdmUBASQC" +
           "AwAAAABHAAAASWYgdHJ1ZSB0aGUgc2VydmVyIHdpbGwgcHJvZHVjZSBuZXcgdmFsdWVzIGZvciBlYWNo" +
           "IG1vbml0b3JlZCB2YXJpYWJsZS4ALgBEJAIAAAAB/////wEB/////wAAAAAEYYIKBAAAAAEADgAAAEdl" +
           "bmVyYXRlVmFsdWVzAQElAgAvAQERACUCAAABAf////8BAAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3Vt" +
           "ZW50cwEBJgIALgBEJgIAAJYBAAAAAQAqAQFGAAAACgAAAEl0ZXJhdGlvbnMAB/////8AAAAAAwAAAAAl" +
           "AAAAVGhlIG51bWJlciBvZiBuZXcgdmFsdWVzIHRvIGdlbmVyYXRlLgEAKAEBAAAAAQAAAAAAAAABAf//" +
           "//8AAAAABGCACgEAAAABAA0AAABDeWNsZUNvbXBsZXRlAQEnAgAvAQBBCycCAAABAAAAACQBAQEjAhcA" +
           "AAAVYIkKAgAAAAAABwAAAEV2ZW50SWQBASgCAC4ARCgCAAAAD/////8BAf////8AAAAAFWCJCgIAAAAA" +
           "AAkAAABFdmVudFR5cGUBASkCAC4ARCkCAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoAAABTb3Vy" +
           "Y2VOb2RlAQEqAgAuAEQqAgAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNlTmFtZQEB" +
           "KwIALgBEKwIAAAAM/////wEB/////wAAAAAVYIkKAgAAAAAABAAAAFRpbWUBASwCAC4ARCwCAAABACYB" +
           "/////wEB/////wAAAAAVYIkKAgAAAAAACwAAAFJlY2VpdmVUaW1lAQEtAgAuAEQtAgAAAQAmAf////8B" +
           "Af////8AAAAAFWCJCgIAAAAAAAcAAABNZXNzYWdlAQEvAgAuAEQvAgAAABX/////AQH/////AAAAABVg" +
           "iQoCAAAAAAAIAAAAU2V2ZXJpdHkBATACAC4ARDACAAAABf////8BAf////8AAAAAFWCJCgIAAAAAABAA" +
           "AABDb25kaXRpb25DbGFzc0lkAQExAgAuAEQxAgAAABH/////AQH/////AAAAABVgiQoCAAAAAAASAAAA" +
           "Q29uZGl0aW9uQ2xhc3NOYW1lAQEyAgAuAEQyAgAAABX/////AQH/////AAAAABVgiQoCAAAAAAANAAAA" +
           "Q29uZGl0aW9uTmFtZQEBNQIALgBENQIAAAAM/////wEB/////wAAAAAVYIkKAgAAAAAACAAAAEJyYW5j" +
           "aElkAQE2AgAuAEQ2AgAAABH/////AQH/////AAAAABVgiQoCAAAAAAAGAAAAUmV0YWluAQE3AgAuAEQ3" +
           "AgAAAAH/////AQH/////AAAAABVgiQoCAAAAAAAMAAAARW5hYmxlZFN0YXRlAQE4AgAvAQAjIzgCAAAA" +
           "Ff////8BAQIAAAABACwjAAEBTAIBACwjAAEBVQIBAAAAFWCJCgIAAAAAAAIAAABJZAEBOQIALgBEOQIA" +
           "AAAB/////wEB/////wAAAAAVYIkKAgAAAAAABwAAAFF1YWxpdHkBAUECAC8BACojQQIAAAAT/////wEB" +
           "/////wEAAAAVYIkKAgAAAAAADwAAAFNvdXJjZVRpbWVzdGFtcAEBQgIALgBEQgIAAAEAJgH/////AQH/" +
           "////AAAAABVgiQoCAAAAAAAMAAAATGFzdFNldmVyaXR5AQFDAgAvAQAqI0MCAAAABf////8BAf////8B" +
           "AAAAFWCJCgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAUQCAC4AREQCAAABACYB/////wEB/////wAA" +
           "AAAVYIkKAgAAAAAABwAAAENvbW1lbnQBAUUCAC8BACojRQIAAAAV/////wEB/////wEAAAAVYIkKAgAA" +
           "AAAADwAAAFNvdXJjZVRpbWVzdGFtcAEBRgIALgBERgIAAAEAJgH/////AQH/////AAAAABVgiQoCAAAA" +
           "AAAMAAAAQ2xpZW50VXNlcklkAQFHAgAuAERHAgAAAAz/////AQH/////AAAAAARhggoEAAAAAAAHAAAA" +
           "RGlzYWJsZQEBSAIALwEARCNIAgAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAGAAAARW5hYmxl" +
           "AQFJAgAvAQBDI0kCAAABAQEAAAABAPkLAAEA8woAAAAABGGCCgQAAAAAAAoAAABBZGRDb21tZW50AQFK" +
           "AgAvAQBFI0oCAAABAQEAAAABAPkLAAEADQsBAAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEB" +
           "SwIALgBESwIAAJYCAAAAAQAqAQFGAAAABwAAAEV2ZW50SWQAD/////8AAAAAAwAAAAAoAAAAVGhlIGlk" +
           "ZW50aWZpZXIgZm9yIHRoZSBldmVudCB0byBjb21tZW50LgEAKgEBQgAAAAcAAABDb21tZW50ABX/////" +
           "AAAAAAMAAAAAJAAAAFRoZSBjb21tZW50IHRvIGFkZCB0byB0aGUgY29uZGl0aW9uLgEAKAEBAAAAAQAA" +
           "AAAAAAABAf////8AAAAAFWCJCgIAAAAAAAoAAABBY2tlZFN0YXRlAQFMAgAvAQAjI0wCAAAAFf////8B" +
           "AQEAAAABACwjAQEBOAIBAAAAFWCJCgIAAAAAAAIAAABJZAEBTQIALgBETQIAAAAB/////wEB/////wAA" +
           "AAAEYYIKBAAAAAAACwAAAEFja25vd2xlZGdlAQFeAgAvAQCXI14CAAABAQEAAAABAPkLAAEA8CIBAAAA" +
           "F2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBXwIALgBEXwIAAJYCAAAAAQAqAQFGAAAABwAAAEV2" +
           "ZW50SWQAD/////8AAAAAAwAAAAAoAAAAVGhlIGlkZW50aWZpZXIgZm9yIHRoZSBldmVudCB0byBjb21t" +
           "ZW50LgEAKgEBQgAAAAcAAABDb21tZW50ABX/////AAAAAAMAAAAAJAAAAFRoZSBjb21tZW50IHRvIGFk" +
           "ZCB0byB0aGUgY29uZGl0aW9uLgEAKAEBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAoAAABT" +
           "Qnl0ZVZhbHVlAQFiAgAvAQBACWICAAAAAgEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAAAAAABwAA" +
           "AEVVUmFuZ2UBAWYCAC4ARGYCAAABAHQD/////wEB/////wAAAAAXYIkKAgAAAAEACQAAAEJ5dGVWYWx1" +
           "ZQEBaAIALwEAQAloAgAAAAMBAAAAAQAAAAAAAAABAf////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdl" +
           "AQFsAgAuAERsAgAAAQB0A/////8BAf////8AAAAAF2CJCgIAAAABAAoAAABJbnQxNlZhbHVlAQFuAgAv" +
           "AQBACW4CAAAABAEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAXICAC4A" +
           "RHICAAABAHQD/////wEB/////wAAAAAXYIkKAgAAAAEACwAAAFVJbnQxNlZhbHVlAQF0AgAvAQBACXQC" +
           "AAAABQEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAXgCAC4ARHgCAAAB" +
           "AHQD/////wEB/////wAAAAAXYIkKAgAAAAEACgAAAEludDMyVmFsdWUBAXoCAC8BAEAJegIAAAAGAQAA" +
           "AAEAAAAAAAAAAQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBfgIALgBEfgIAAAEAdAP/////" +
           "AQH/////AAAAABdgiQoCAAAAAQALAAAAVUludDMyVmFsdWUBAYACAC8BAEAJgAIAAAAHAQAAAAEAAAAA" +
           "AAAAAQH/////AQAAABVgiQoCAAAAAAAHAAAARVVSYW5nZQEBhAIALgBEhAIAAAEAdAP/////AQH/////" +
           "AAAAABdgiQoCAAAAAQAKAAAASW50NjRWYWx1ZQEBhgIALwEAQAmGAgAAAAgBAAAAAQAAAAAAAAABAf//" +
           "//8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQGKAgAuAESKAgAAAQB0A/////8BAf////8AAAAAF2CJ" +
           "CgIAAAABAAsAAABVSW50NjRWYWx1ZQEBjAIALwEAQAmMAgAAAAkBAAAAAQAAAAAAAAABAf////8BAAAA" +
           "FWCJCgIAAAAAAAcAAABFVVJhbmdlAQGQAgAuAESQAgAAAQB0A/////8BAf////8AAAAAF2CJCgIAAAAB" +
           "AAoAAABGbG9hdFZhbHVlAQGSAgAvAQBACZICAAAACgEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAA" +
           "AAAABwAAAEVVUmFuZ2UBAZYCAC4ARJYCAAABAHQD/////wEB/////wAAAAAXYIkKAgAAAAEACwAAAERv" +
           "dWJsZVZhbHVlAQGYAgAvAQBACZgCAAAACwEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAAAAAABwAA" +
           "AEVVUmFuZ2UBAZwCAC4ARJwCAAABAHQD/////wEB/////wAAAAAXYIkKAgAAAAEACwAAAE51bWJlclZh" +
           "bHVlAQGeAgAvAQBACZ4CAAAAGgEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFu" +
           "Z2UBAaICAC4ARKICAAABAHQD/////wEB/////wAAAAAXYIkKAgAAAAEADAAAAEludGVnZXJWYWx1ZQEB" +
           "pAIALwEAQAmkAgAAABsBAAAAAQAAAAAAAAABAf////8BAAAAFWCJCgIAAAAAAAcAAABFVVJhbmdlAQGo" +
           "AgAuAESoAgAAAQB0A/////8BAf////8AAAAAF2CJCgIAAAABAA0AAABVSW50ZWdlclZhbHVlAQGqAgAv" +
           "AQBACaoCAAAAHAEAAAABAAAAAAAAAAEB/////wEAAAAVYIkKAgAAAAAABwAAAEVVUmFuZ2UBAa4CAC4A" +
           "RK4CAAABAHQD/////wEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public AnalogItemState<sbyte[]> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<byte[]> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<short[]> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<ushort[]> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<int[]> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<uint[]> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<long[]> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<ulong[]> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public AnalogItemState<float[]> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<double[]> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<object[]> NumberValue
        {
            get
            {
                return m_numberValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_numberValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_numberValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<object[]> IntegerValue
        {
            get
            {
                return m_integerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_integerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_integerValue = value;
            }
        }

        /// <remarks />
        public AnalogItemState<object[]> UIntegerValue
        {
            get
            {
                return m_uIntegerValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uIntegerValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uIntegerValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_numberValue != null)
            {
                children.Add(m_numberValue);
            }

            if (m_integerValue != null)
            {
                children.Add(m_integerValue);
            }

            if (m_uIntegerValue != null)
            {
                children.Add(m_uIntegerValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new AnalogItemState<sbyte[]>(this);
                            }
                            else
                            {
                                SByteValue = (AnalogItemState<sbyte[]>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new AnalogItemState<byte[]>(this);
                            }
                            else
                            {
                                ByteValue = (AnalogItemState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new AnalogItemState<short[]>(this);
                            }
                            else
                            {
                                Int16Value = (AnalogItemState<short[]>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new AnalogItemState<ushort[]>(this);
                            }
                            else
                            {
                                UInt16Value = (AnalogItemState<ushort[]>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new AnalogItemState<int[]>(this);
                            }
                            else
                            {
                                Int32Value = (AnalogItemState<int[]>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new AnalogItemState<uint[]>(this);
                            }
                            else
                            {
                                UInt32Value = (AnalogItemState<uint[]>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new AnalogItemState<long[]>(this);
                            }
                            else
                            {
                                Int64Value = (AnalogItemState<long[]>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new AnalogItemState<ulong[]>(this);
                            }
                            else
                            {
                                UInt64Value = (AnalogItemState<ulong[]>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new AnalogItemState<float[]>(this);
                            }
                            else
                            {
                                FloatValue = (AnalogItemState<float[]>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new AnalogItemState<double[]>(this);
                            }
                            else
                            {
                                DoubleValue = (AnalogItemState<double[]>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NumberValue:
                {
                    if (createOrReplace)
                    {
                        if (NumberValue == null)
                        {
                            if (replacement == null)
                            {
                                NumberValue = new AnalogItemState<object[]>(this);
                            }
                            else
                            {
                                NumberValue = (AnalogItemState<object[]>)replacement;
                            }
                        }
                    }

                    instance = NumberValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.IntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (IntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                IntegerValue = new AnalogItemState<object[]>(this);
                            }
                            else
                            {
                                IntegerValue = (AnalogItemState<object[]>)replacement;
                            }
                        }
                    }

                    instance = IntegerValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UIntegerValue:
                {
                    if (createOrReplace)
                    {
                        if (UIntegerValue == null)
                        {
                            if (replacement == null)
                            {
                                UIntegerValue = new AnalogItemState<object[]>(this);
                            }
                            else
                            {
                                UIntegerValue = (AnalogItemState<object[]>)replacement;
                            }
                        }
                    }

                    instance = UIntegerValue;
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
        private AnalogItemState<sbyte[]> m_sByteValue;
        private AnalogItemState<byte[]> m_byteValue;
        private AnalogItemState<short[]> m_int16Value;
        private AnalogItemState<ushort[]> m_uInt16Value;
        private AnalogItemState<int[]> m_int32Value;
        private AnalogItemState<uint[]> m_uInt32Value;
        private AnalogItemState<long[]> m_int64Value;
        private AnalogItemState<ulong[]> m_uInt64Value;
        private AnalogItemState<float[]> m_floatValue;
        private AnalogItemState<double[]> m_doubleValue;
        private AnalogItemState<object[]> m_numberValue;
        private AnalogItemState<object[]> m_integerValue;
        private AnalogItemState<object[]> m_uIntegerValue;
        #endregion
    }
    #endif
    #endregion

    #region UserScalarValueObjectState Class
    #if (!OPCUA_EXCLUDE_UserScalarValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class UserScalarValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public UserScalarValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.UserScalarValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAIQAAAFVzZXJTY2FsYXJWYWx1ZU9iamVjdFR5cGVJbnN0YW5jZQEB" +
           "xwIBAccCxwIAAAEAAAAAJAABAcsCGQAAADVgiQoCAAAAAQAQAAAAU2ltdWxhdGlvbkFjdGl2ZQEByAID" +
           "AAAAAEcAAABJZiB0cnVlIHRoZSBzZXJ2ZXIgd2lsbCBwcm9kdWNlIG5ldyB2YWx1ZXMgZm9yIGVhY2gg" +
           "bW9uaXRvcmVkIHZhcmlhYmxlLgAuAETIAgAAAAH/////AQH/////AAAAAARhggoEAAAAAQAOAAAAR2Vu" +
           "ZXJhdGVWYWx1ZXMBAckCAC8BAREAyQIAAAEB/////wEAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1l" +
           "bnRzAQHKAgAuAETKAgAAlgEAAAABACoBAUYAAAAKAAAASXRlcmF0aW9ucwAH/////wAAAAADAAAAACUA" +
           "AABUaGUgbnVtYmVyIG9mIG5ldyB2YWx1ZXMgdG8gZ2VuZXJhdGUuAQAoAQEAAAABAAAAAAAAAAEB////" +
           "/wAAAAAEYIAKAQAAAAEADQAAAEN5Y2xlQ29tcGxldGUBAcsCAC8BAEELywIAAAEAAAAAJAEBAccCFwAA" +
           "ABVgiQoCAAAAAAAHAAAARXZlbnRJZAEBzAIALgBEzAIAAAAP/////wEB/////wAAAAAVYIkKAgAAAAAA" +
           "CQAAAEV2ZW50VHlwZQEBzQIALgBEzQIAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJj" +
           "ZU5vZGUBAc4CAC4ARM4CAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoAAABTb3VyY2VOYW1lAQHP" +
           "AgAuAETPAgAAAAz/////AQH/////AAAAABVgiQoCAAAAAAAEAAAAVGltZQEB0AIALgBE0AIAAAEAJgH/" +
           "////AQH/////AAAAABVgiQoCAAAAAAALAAAAUmVjZWl2ZVRpbWUBAdECAC4ARNECAAABACYB/////wEB" +
           "/////wAAAAAVYIkKAgAAAAAABwAAAE1lc3NhZ2UBAdMCAC4ARNMCAAAAFf////8BAf////8AAAAAFWCJ" +
           "CgIAAAAAAAgAAABTZXZlcml0eQEB1AIALgBE1AIAAAAF/////wEB/////wAAAAAVYIkKAgAAAAAAEAAA" +
           "AENvbmRpdGlvbkNsYXNzSWQBAdUCAC4ARNUCAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAABIAAABD" +
           "b25kaXRpb25DbGFzc05hbWUBAdYCAC4ARNYCAAAAFf////8BAf////8AAAAAFWCJCgIAAAAAAA0AAABD" +
           "b25kaXRpb25OYW1lAQHZAgAuAETZAgAAAAz/////AQH/////AAAAABVgiQoCAAAAAAAIAAAAQnJhbmNo" +
           "SWQBAdoCAC4ARNoCAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAYAAABSZXRhaW4BAdsCAC4ARNsC" +
           "AAAAAf////8BAf////8AAAAAFWCJCgIAAAAAAAwAAABFbmFibGVkU3RhdGUBAdwCAC8BACMj3AIAAAAV" +
           "/////wEBAgAAAAEALCMAAQHwAgEALCMAAQH5AgEAAAAVYIkKAgAAAAAAAgAAAElkAQHdAgAuAETdAgAA" +
           "AAH/////AQH/////AAAAABVgiQoCAAAAAAAHAAAAUXVhbGl0eQEB5QIALwEAKiPlAgAAABP/////AQH/" +
           "////AQAAABVgiQoCAAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQHmAgAuAETmAgAAAQAmAf////8BAf//" +
           "//8AAAAAFWCJCgIAAAAAAAwAAABMYXN0U2V2ZXJpdHkBAecCAC8BACoj5wIAAAAF/////wEB/////wEA" +
           "AAAVYIkKAgAAAAAADwAAAFNvdXJjZVRpbWVzdGFtcAEB6AIALgBE6AIAAAEAJgH/////AQH/////AAAA" +
           "ABVgiQoCAAAAAAAHAAAAQ29tbWVudAEB6QIALwEAKiPpAgAAABX/////AQH/////AQAAABVgiQoCAAAA" +
           "AAAPAAAAU291cmNlVGltZXN0YW1wAQHqAgAuAETqAgAAAQAmAf////8BAf////8AAAAAFWCJCgIAAAAA" +
           "AAwAAABDbGllbnRVc2VySWQBAesCAC4AROsCAAAADP////8BAf////8AAAAABGGCCgQAAAAAAAcAAABE" +
           "aXNhYmxlAQHsAgAvAQBEI+wCAAABAQEAAAABAPkLAAEA8woAAAAABGGCCgQAAAAAAAYAAABFbmFibGUB" +
           "Ae0CAC8BAEMj7QIAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAACgAAAEFkZENvbW1lbnQBAe4C" +
           "AC8BAEUj7gIAAAEBAQAAAAEA+QsAAQANCwEAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQHv" +
           "AgAuAETvAgAAlgIAAAABACoBAUYAAAAHAAAARXZlbnRJZAAP/////wAAAAADAAAAACgAAABUaGUgaWRl" +
           "bnRpZmllciBmb3IgdGhlIGV2ZW50IHRvIGNvbW1lbnQuAQAqAQFCAAAABwAAAENvbW1lbnQAFf////8A" +
           "AAAAAwAAAAAkAAAAVGhlIGNvbW1lbnQgdG8gYWRkIHRvIHRoZSBjb25kaXRpb24uAQAoAQEAAAABAAAA" +
           "AAAAAAEB/////wAAAAAVYIkKAgAAAAAACgAAAEFja2VkU3RhdGUBAfACAC8BACMj8AIAAAAV/////wEB" +
           "AQAAAAEALCMBAQHcAgEAAAAVYIkKAgAAAAAAAgAAAElkAQHxAgAuAETxAgAAAAH/////AQH/////AAAA" +
           "AARhggoEAAAAAAALAAAAQWNrbm93bGVkZ2UBAQIDAC8BAJcjAgMAAAEBAQAAAAEA+QsAAQDwIgEAAAAX" +
           "YKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQEDAwAuAEQDAwAAlgIAAAABACoBAUYAAAAHAAAARXZl" +
           "bnRJZAAP/////wAAAAADAAAAACgAAABUaGUgaWRlbnRpZmllciBmb3IgdGhlIGV2ZW50IHRvIGNvbW1l" +
           "bnQuAQAqAQFCAAAABwAAAENvbW1lbnQAFf////8AAAAAAwAAAAAkAAAAVGhlIGNvbW1lbnQgdG8gYWRk" +
           "IHRvIHRoZSBjb25kaXRpb24uAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAVYIkKAgAAAAEADAAAAEJv" +
           "b2xlYW5WYWx1ZQEBBgMALwA/BgMAAAEBsAL/////AQH/////AAAAABVgiQoCAAAAAQAKAAAAU0J5dGVW" +
           "YWx1ZQEBBwMALwA/BwMAAAEBsQL/////AQH/////AAAAABVgiQoCAAAAAQAJAAAAQnl0ZVZhbHVlAQEI" +
           "AwAvAD8IAwAAAQGyAv////8BAf////8AAAAAFWCJCgIAAAABAAoAAABJbnQxNlZhbHVlAQEJAwAvAD8J" +
           "AwAAAQGzAv////8BAf////8AAAAAFWCJCgIAAAABAAsAAABVSW50MTZWYWx1ZQEBCgMALwA/CgMAAAEB" +
           "tAL/////AQH/////AAAAABVgiQoCAAAAAQAKAAAASW50MzJWYWx1ZQEBCwMALwA/CwMAAAEBtQL/////" +
           "AQH/////AAAAABVgiQoCAAAAAQALAAAAVUludDMyVmFsdWUBAQwDAC8APwwDAAABAbYC/////wEB////" +
           "/wAAAAAVYIkKAgAAAAEACgAAAEludDY0VmFsdWUBAQ0DAC8APw0DAAABAbcC/////wEB/////wAAAAAV" +
           "YIkKAgAAAAEACwAAAFVJbnQ2NFZhbHVlAQEOAwAvAD8OAwAAAQG4Av////8BAf////8AAAAAFWCJCgIA" +
           "AAABAAoAAABGbG9hdFZhbHVlAQEPAwAvAD8PAwAAAQG5Av////8BAf////8AAAAAFWCJCgIAAAABAAsA" +
           "AABEb3VibGVWYWx1ZQEBEAMALwA/EAMAAAEBugL/////AQH/////AAAAABVgiQoCAAAAAQALAAAAU3Ry" +
           "aW5nVmFsdWUBAREDAC8APxEDAAABAbsC/////wEB/////wAAAAAVYIkKAgAAAAEADQAAAERhdGVUaW1l" +
           "VmFsdWUBARIDAC8APxIDAAABAbwC/////wEB/////wAAAAAVYIkKAgAAAAEACQAAAEd1aWRWYWx1ZQEB" +
           "EwMALwA/EwMAAAEBvQL/////AQH/////AAAAABVgiQoCAAAAAQAPAAAAQnl0ZVN0cmluZ1ZhbHVlAQEU" +
           "AwAvAD8UAwAAAQG+Av////8BAf////8AAAAAFWCJCgIAAAABAA8AAABYbWxFbGVtZW50VmFsdWUBARUD" +
           "AC8APxUDAAABAb8C/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAE5vZGVJZFZhbHVlAQEWAwAvAD8W" +
           "AwAAAQHAAv////8BAf////8AAAAAFWCJCgIAAAABABMAAABFeHBhbmRlZE5vZGVJZFZhbHVlAQEXAwAv" +
           "AD8XAwAAAQHBAv////8BAf////8AAAAAFWCJCgIAAAABABIAAABRdWFsaWZpZWROYW1lVmFsdWUBARgD" +
           "AC8APxgDAAABAcIC/////wEB/////wAAAAAVYIkKAgAAAAEAEgAAAExvY2FsaXplZFRleHRWYWx1ZQEB" +
           "GQMALwA/GQMAAAEBwwL/////AQH/////AAAAABVgiQoCAAAAAQAPAAAAU3RhdHVzQ29kZVZhbHVlAQEa" +
           "AwAvAD8aAwAAAQHEAv////8BAf////8AAAAAFWCJCgIAAAABAAwAAABWYXJpYW50VmFsdWUBARsDAC8A" +
           "PxsDAAABAcUC/////wEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public BaseDataVariableState<bool> BooleanValue
        {
            get
            {
                return m_booleanValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_booleanValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_booleanValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<sbyte> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<short> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ushort> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<int> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<uint> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<long> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ulong> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<float> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<double> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<string> StringValue
        {
            get
            {
                return m_stringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_stringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_stringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<DateTime> DateTimeValue
        {
            get
            {
                return m_dateTimeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_dateTimeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_dateTimeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<Guid> GuidValue
        {
            get
            {
                return m_guidValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_guidValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_guidValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[]> ByteStringValue
        {
            get
            {
                return m_byteStringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteStringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteStringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<XmlElement> XmlElementValue
        {
            get
            {
                return m_xmlElementValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_xmlElementValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_xmlElementValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<NodeId> NodeIdValue
        {
            get
            {
                return m_nodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_nodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_nodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExpandedNodeId> ExpandedNodeIdValue
        {
            get
            {
                return m_expandedNodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_expandedNodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_expandedNodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<QualifiedName> QualifiedNameValue
        {
            get
            {
                return m_qualifiedNameValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_qualifiedNameValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_qualifiedNameValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<LocalizedText> LocalizedTextValue
        {
            get
            {
                return m_localizedTextValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_localizedTextValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_localizedTextValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<StatusCode> StatusCodeValue
        {
            get
            {
                return m_statusCodeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_statusCodeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_statusCodeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState VariantValue
        {
            get
            {
                return m_variantValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_variantValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_variantValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_booleanValue != null)
            {
                children.Add(m_booleanValue);
            }

            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_stringValue != null)
            {
                children.Add(m_stringValue);
            }

            if (m_dateTimeValue != null)
            {
                children.Add(m_dateTimeValue);
            }

            if (m_guidValue != null)
            {
                children.Add(m_guidValue);
            }

            if (m_byteStringValue != null)
            {
                children.Add(m_byteStringValue);
            }

            if (m_xmlElementValue != null)
            {
                children.Add(m_xmlElementValue);
            }

            if (m_nodeIdValue != null)
            {
                children.Add(m_nodeIdValue);
            }

            if (m_expandedNodeIdValue != null)
            {
                children.Add(m_expandedNodeIdValue);
            }

            if (m_qualifiedNameValue != null)
            {
                children.Add(m_qualifiedNameValue);
            }

            if (m_localizedTextValue != null)
            {
                children.Add(m_localizedTextValue);
            }

            if (m_statusCodeValue != null)
            {
                children.Add(m_statusCodeValue);
            }

            if (m_variantValue != null)
            {
                children.Add(m_variantValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.BooleanValue:
                {
                    if (createOrReplace)
                    {
                        if (BooleanValue == null)
                        {
                            if (replacement == null)
                            {
                                BooleanValue = new BaseDataVariableState<bool>(this);
                            }
                            else
                            {
                                BooleanValue = (BaseDataVariableState<bool>)replacement;
                            }
                        }
                    }

                    instance = BooleanValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new BaseDataVariableState<sbyte>(this);
                            }
                            else
                            {
                                SByteValue = (BaseDataVariableState<sbyte>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new BaseDataVariableState<byte>(this);
                            }
                            else
                            {
                                ByteValue = (BaseDataVariableState<byte>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new BaseDataVariableState<short>(this);
                            }
                            else
                            {
                                Int16Value = (BaseDataVariableState<short>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new BaseDataVariableState<ushort>(this);
                            }
                            else
                            {
                                UInt16Value = (BaseDataVariableState<ushort>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new BaseDataVariableState<int>(this);
                            }
                            else
                            {
                                Int32Value = (BaseDataVariableState<int>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new BaseDataVariableState<uint>(this);
                            }
                            else
                            {
                                UInt32Value = (BaseDataVariableState<uint>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new BaseDataVariableState<long>(this);
                            }
                            else
                            {
                                Int64Value = (BaseDataVariableState<long>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new BaseDataVariableState<ulong>(this);
                            }
                            else
                            {
                                UInt64Value = (BaseDataVariableState<ulong>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new BaseDataVariableState<float>(this);
                            }
                            else
                            {
                                FloatValue = (BaseDataVariableState<float>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new BaseDataVariableState<double>(this);
                            }
                            else
                            {
                                DoubleValue = (BaseDataVariableState<double>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StringValue:
                {
                    if (createOrReplace)
                    {
                        if (StringValue == null)
                        {
                            if (replacement == null)
                            {
                                StringValue = new BaseDataVariableState<string>(this);
                            }
                            else
                            {
                                StringValue = (BaseDataVariableState<string>)replacement;
                            }
                        }
                    }

                    instance = StringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DateTimeValue:
                {
                    if (createOrReplace)
                    {
                        if (DateTimeValue == null)
                        {
                            if (replacement == null)
                            {
                                DateTimeValue = new BaseDataVariableState<DateTime>(this);
                            }
                            else
                            {
                                DateTimeValue = (BaseDataVariableState<DateTime>)replacement;
                            }
                        }
                    }

                    instance = DateTimeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.GuidValue:
                {
                    if (createOrReplace)
                    {
                        if (GuidValue == null)
                        {
                            if (replacement == null)
                            {
                                GuidValue = new BaseDataVariableState<Guid>(this);
                            }
                            else
                            {
                                GuidValue = (BaseDataVariableState<Guid>)replacement;
                            }
                        }
                    }

                    instance = GuidValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteStringValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteStringValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteStringValue = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                ByteStringValue = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ByteStringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.XmlElementValue:
                {
                    if (createOrReplace)
                    {
                        if (XmlElementValue == null)
                        {
                            if (replacement == null)
                            {
                                XmlElementValue = new BaseDataVariableState<XmlElement>(this);
                            }
                            else
                            {
                                XmlElementValue = (BaseDataVariableState<XmlElement>)replacement;
                            }
                        }
                    }

                    instance = XmlElementValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (NodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                NodeIdValue = new BaseDataVariableState<NodeId>(this);
                            }
                            else
                            {
                                NodeIdValue = (BaseDataVariableState<NodeId>)replacement;
                            }
                        }
                    }

                    instance = NodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ExpandedNodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (ExpandedNodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                ExpandedNodeIdValue = new BaseDataVariableState<ExpandedNodeId>(this);
                            }
                            else
                            {
                                ExpandedNodeIdValue = (BaseDataVariableState<ExpandedNodeId>)replacement;
                            }
                        }
                    }

                    instance = ExpandedNodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.QualifiedNameValue:
                {
                    if (createOrReplace)
                    {
                        if (QualifiedNameValue == null)
                        {
                            if (replacement == null)
                            {
                                QualifiedNameValue = new BaseDataVariableState<QualifiedName>(this);
                            }
                            else
                            {
                                QualifiedNameValue = (BaseDataVariableState<QualifiedName>)replacement;
                            }
                        }
                    }

                    instance = QualifiedNameValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.LocalizedTextValue:
                {
                    if (createOrReplace)
                    {
                        if (LocalizedTextValue == null)
                        {
                            if (replacement == null)
                            {
                                LocalizedTextValue = new BaseDataVariableState<LocalizedText>(this);
                            }
                            else
                            {
                                LocalizedTextValue = (BaseDataVariableState<LocalizedText>)replacement;
                            }
                        }
                    }

                    instance = LocalizedTextValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StatusCodeValue:
                {
                    if (createOrReplace)
                    {
                        if (StatusCodeValue == null)
                        {
                            if (replacement == null)
                            {
                                StatusCodeValue = new BaseDataVariableState<StatusCode>(this);
                            }
                            else
                            {
                                StatusCodeValue = (BaseDataVariableState<StatusCode>)replacement;
                            }
                        }
                    }

                    instance = StatusCodeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VariantValue:
                {
                    if (createOrReplace)
                    {
                        if (VariantValue == null)
                        {
                            if (replacement == null)
                            {
                                VariantValue = new BaseDataVariableState(this);
                            }
                            else
                            {
                                VariantValue = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = VariantValue;
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
        private BaseDataVariableState<bool> m_booleanValue;
        private BaseDataVariableState<sbyte> m_sByteValue;
        private BaseDataVariableState<byte> m_byteValue;
        private BaseDataVariableState<short> m_int16Value;
        private BaseDataVariableState<ushort> m_uInt16Value;
        private BaseDataVariableState<int> m_int32Value;
        private BaseDataVariableState<uint> m_uInt32Value;
        private BaseDataVariableState<long> m_int64Value;
        private BaseDataVariableState<ulong> m_uInt64Value;
        private BaseDataVariableState<float> m_floatValue;
        private BaseDataVariableState<double> m_doubleValue;
        private BaseDataVariableState<string> m_stringValue;
        private BaseDataVariableState<DateTime> m_dateTimeValue;
        private BaseDataVariableState<Guid> m_guidValue;
        private BaseDataVariableState<byte[]> m_byteStringValue;
        private BaseDataVariableState<XmlElement> m_xmlElementValue;
        private BaseDataVariableState<NodeId> m_nodeIdValue;
        private BaseDataVariableState<ExpandedNodeId> m_expandedNodeIdValue;
        private BaseDataVariableState<QualifiedName> m_qualifiedNameValue;
        private BaseDataVariableState<LocalizedText> m_localizedTextValue;
        private BaseDataVariableState<StatusCode> m_statusCodeValue;
        private BaseDataVariableState m_variantValue;
        #endregion
    }
    #endif
    #endregion

    #region UserScalarValue1MethodState Class
    #if (!OPCUA_EXCLUDE_UserScalarValue1MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class UserScalarValue1MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public UserScalarValue1MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new UserScalarValue1MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAGgAAAFVzZXJTY2FsYXJWYWx1ZTFNZXRob2RUeXBlAQEcAwAvAQEc" +
           "AxwDAAABAf////8CAAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBHQMALgBEHQMAAJYMAAAA" +
           "AQAqAQEaAAAACQAAAEJvb2xlYW5JbgEBsAL/////AAAAAAABACoBARgAAAAHAAAAU0J5dGVJbgEBsQL/" +
           "////AAAAAAABACoBARcAAAAGAAAAQnl0ZUluAQGyAv////8AAAAAAAEAKgEBGAAAAAcAAABJbnQxNklu" +
           "AQGzAv////8AAAAAAAEAKgEBGQAAAAgAAABVSW50MTZJbgEBtAL/////AAAAAAABACoBARgAAAAHAAAA" +
           "SW50MzJJbgEBtQL/////AAAAAAABACoBARkAAAAIAAAAVUludDMySW4BAbYC/////wAAAAAAAQAqAQEY" +
           "AAAABwAAAEludDY0SW4BAbcC/////wAAAAAAAQAqAQEZAAAACAAAAFVJbnQ2NEluAQG4Av////8AAAAA" +
           "AAEAKgEBGAAAAAcAAABGbG9hdEluAQG5Av////8AAAAAAAEAKgEBGQAAAAgAAABEb3VibGVJbgEBugL/" +
           "////AAAAAAABACoBARkAAAAIAAAAU3RyaW5nSW4BAbsC/////wAAAAAAAQAoAQEAAAABAAAAAAAAAAEB" +
           "/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBHgMALgBEHgMAAJYMAAAAAQAqAQEb" +
           "AAAACgAAAEJvb2xlYW5PdXQBAbAC/////wAAAAAAAQAqAQEZAAAACAAAAFNCeXRlT3V0AQGxAv////8A" +
           "AAAAAAEAKgEBGAAAAAcAAABCeXRlT3V0AQGyAv////8AAAAAAAEAKgEBGQAAAAgAAABJbnQxNk91dAEB" +
           "swL/////AAAAAAABACoBARoAAAAJAAAAVUludDE2T3V0AQG0Av////8AAAAAAAEAKgEBGQAAAAgAAABJ" +
           "bnQzMk91dAEBtQL/////AAAAAAABACoBARoAAAAJAAAAVUludDMyT3V0AQG2Av////8AAAAAAAEAKgEB" +
           "GQAAAAgAAABJbnQ2NE91dAEBtwL/////AAAAAAABACoBARoAAAAJAAAAVUludDY0T3V0AQG4Av////8A" +
           "AAAAAAEAKgEBGQAAAAgAAABGbG9hdE91dAEBuQL/////AAAAAAABACoBARoAAAAJAAAARG91YmxlT3V0" +
           "AQG6Av////8AAAAAAAEAKgEBGgAAAAkAAABTdHJpbmdPdXQBAbsC/////wAAAAAAAQAoAQEAAAABAAAA" +
           "AAAAAAEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public UserScalarValue1MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            bool booleanIn = (bool)_inputArguments[0];
            sbyte sByteIn = (sbyte)_inputArguments[1];
            byte byteIn = (byte)_inputArguments[2];
            short int16In = (short)_inputArguments[3];
            ushort uInt16In = (ushort)_inputArguments[4];
            int int32In = (int)_inputArguments[5];
            uint uInt32In = (uint)_inputArguments[6];
            long int64In = (long)_inputArguments[7];
            ulong uInt64In = (ulong)_inputArguments[8];
            float floatIn = (float)_inputArguments[9];
            double doubleIn = (double)_inputArguments[10];
            string stringIn = (string)_inputArguments[11];

            bool booleanOut = (bool)_outputArguments[0];
            sbyte sByteOut = (sbyte)_outputArguments[1];
            byte byteOut = (byte)_outputArguments[2];
            short int16Out = (short)_outputArguments[3];
            ushort uInt16Out = (ushort)_outputArguments[4];
            int int32Out = (int)_outputArguments[5];
            uint uInt32Out = (uint)_outputArguments[6];
            long int64Out = (long)_outputArguments[7];
            ulong uInt64Out = (ulong)_outputArguments[8];
            float floatOut = (float)_outputArguments[9];
            double doubleOut = (double)_outputArguments[10];
            string stringOut = (string)_outputArguments[11];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    booleanIn,
                    sByteIn,
                    byteIn,
                    int16In,
                    uInt16In,
                    int32In,
                    uInt32In,
                    int64In,
                    uInt64In,
                    floatIn,
                    doubleIn,
                    stringIn,
                    ref booleanOut,
                    ref sByteOut,
                    ref byteOut,
                    ref int16Out,
                    ref uInt16Out,
                    ref int32Out,
                    ref uInt32Out,
                    ref int64Out,
                    ref uInt64Out,
                    ref floatOut,
                    ref doubleOut,
                    ref stringOut);
            }

            _outputArguments[0] = booleanOut;
            _outputArguments[1] = sByteOut;
            _outputArguments[2] = byteOut;
            _outputArguments[3] = int16Out;
            _outputArguments[4] = uInt16Out;
            _outputArguments[5] = int32Out;
            _outputArguments[6] = uInt32Out;
            _outputArguments[7] = int64Out;
            _outputArguments[8] = uInt64Out;
            _outputArguments[9] = floatOut;
            _outputArguments[10] = doubleOut;
            _outputArguments[11] = stringOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult UserScalarValue1MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        bool booleanIn,
        sbyte sByteIn,
        byte byteIn,
        short int16In,
        ushort uInt16In,
        int int32In,
        uint uInt32In,
        long int64In,
        ulong uInt64In,
        float floatIn,
        double doubleIn,
        string stringIn,
        ref bool booleanOut,
        ref sbyte sByteOut,
        ref byte byteOut,
        ref short int16Out,
        ref ushort uInt16Out,
        ref int int32Out,
        ref uint uInt32Out,
        ref long int64Out,
        ref ulong uInt64Out,
        ref float floatOut,
        ref double doubleOut,
        ref string stringOut);
    #endif
    #endregion

    #region UserScalarValue2MethodState Class
    #if (!OPCUA_EXCLUDE_UserScalarValue2MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class UserScalarValue2MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public UserScalarValue2MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new UserScalarValue2MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAGgAAAFVzZXJTY2FsYXJWYWx1ZTJNZXRob2RUeXBlAQEfAwAvAQEf" +
           "Ax8DAAABAf////8CAAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBIAMALgBEIAMAAJYKAAAA" +
           "AQAqAQEbAAAACgAAAERhdGVUaW1lSW4BAbwC/////wAAAAAAAQAqAQEXAAAABgAAAEd1aWRJbgEBvQL/" +
           "////AAAAAAABACoBAR0AAAAMAAAAQnl0ZVN0cmluZ0luAQG+Av////8AAAAAAAEAKgEBHQAAAAwAAABY" +
           "bWxFbGVtZW50SW4BAb8C/////wAAAAAAAQAqAQEZAAAACAAAAE5vZGVJZEluAQHAAv////8AAAAAAAEA" +
           "KgEBIQAAABAAAABFeHBhbmRlZE5vZGVJZEluAQHBAv////8AAAAAAAEAKgEBIAAAAA8AAABRdWFsaWZp" +
           "ZWROYW1lSW4BAcIC/////wAAAAAAAQAqAQEgAAAADwAAAExvY2FsaXplZFRleHRJbgEBwwL/////AAAA" +
           "AAABACoBAR0AAAAMAAAAU3RhdHVzQ29kZUluAQHEAv////8AAAAAAAEAKgEBGgAAAAkAAABWYXJpYW50" +
           "SW4BAcUC/////wAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1" +
           "dEFyZ3VtZW50cwEBIQMALgBEIQMAAJYKAAAAAQAqAQEcAAAACwAAAERhdGVUaW1lT3V0AQG8Av////8A" +
           "AAAAAAEAKgEBGAAAAAcAAABHdWlkT3V0AQG9Av////8AAAAAAAEAKgEBHgAAAA0AAABCeXRlU3RyaW5n" +
           "T3V0AQG+Av////8AAAAAAAEAKgEBHgAAAA0AAABYbWxFbGVtZW50T3V0AQG/Av////8AAAAAAAEAKgEB" +
           "GgAAAAkAAABOb2RlSWRPdXQBAcAC/////wAAAAAAAQAqAQEiAAAAEQAAAEV4cGFuZGVkTm9kZUlkT3V0" +
           "AQHBAv////8AAAAAAAEAKgEBIQAAABAAAABRdWFsaWZpZWROYW1lT3V0AQHCAv////8AAAAAAAEAKgEB" +
           "IQAAABAAAABMb2NhbGl6ZWRUZXh0T3V0AQHDAv////8AAAAAAAEAKgEBHgAAAA0AAABTdGF0dXNDb2Rl" +
           "T3V0AQHEAv////8AAAAAAAEAKgEBGwAAAAoAAABWYXJpYW50T3V0AQHFAv////8AAAAAAAEAKAEBAAAA" +
           "AQAAAAAAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public UserScalarValue2MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            DateTime dateTimeIn = (DateTime)_inputArguments[0];
            Uuid guidIn = (Uuid)_inputArguments[1];
            byte[] byteStringIn = (byte[])_inputArguments[2];
            XmlElement xmlElementIn = (XmlElement)_inputArguments[3];
            NodeId nodeIdIn = (NodeId)_inputArguments[4];
            ExpandedNodeId expandedNodeIdIn = (ExpandedNodeId)_inputArguments[5];
            QualifiedName qualifiedNameIn = (QualifiedName)_inputArguments[6];
            LocalizedText localizedTextIn = (LocalizedText)_inputArguments[7];
            StatusCode statusCodeIn = (StatusCode)_inputArguments[8];
            object variantIn = (object)_inputArguments[9];

            DateTime dateTimeOut = (DateTime)_outputArguments[0];
            Uuid guidOut = (Uuid)_outputArguments[1];
            byte[] byteStringOut = (byte[])_outputArguments[2];
            XmlElement xmlElementOut = (XmlElement)_outputArguments[3];
            NodeId nodeIdOut = (NodeId)_outputArguments[4];
            ExpandedNodeId expandedNodeIdOut = (ExpandedNodeId)_outputArguments[5];
            QualifiedName qualifiedNameOut = (QualifiedName)_outputArguments[6];
            LocalizedText localizedTextOut = (LocalizedText)_outputArguments[7];
            StatusCode statusCodeOut = (StatusCode)_outputArguments[8];
            object variantOut = (object)_outputArguments[9];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    dateTimeIn,
                    guidIn,
                    byteStringIn,
                    xmlElementIn,
                    nodeIdIn,
                    expandedNodeIdIn,
                    qualifiedNameIn,
                    localizedTextIn,
                    statusCodeIn,
                    variantIn,
                    ref dateTimeOut,
                    ref guidOut,
                    ref byteStringOut,
                    ref xmlElementOut,
                    ref nodeIdOut,
                    ref expandedNodeIdOut,
                    ref qualifiedNameOut,
                    ref localizedTextOut,
                    ref statusCodeOut,
                    ref variantOut);
            }

            _outputArguments[0] = dateTimeOut;
            _outputArguments[1] = guidOut;
            _outputArguments[2] = byteStringOut;
            _outputArguments[3] = xmlElementOut;
            _outputArguments[4] = nodeIdOut;
            _outputArguments[5] = expandedNodeIdOut;
            _outputArguments[6] = qualifiedNameOut;
            _outputArguments[7] = localizedTextOut;
            _outputArguments[8] = statusCodeOut;
            _outputArguments[9] = variantOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult UserScalarValue2MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        DateTime dateTimeIn,
        Uuid guidIn,
        byte[] byteStringIn,
        XmlElement xmlElementIn,
        NodeId nodeIdIn,
        ExpandedNodeId expandedNodeIdIn,
        QualifiedName qualifiedNameIn,
        LocalizedText localizedTextIn,
        StatusCode statusCodeIn,
        object variantIn,
        ref DateTime dateTimeOut,
        ref Uuid guidOut,
        ref byte[] byteStringOut,
        ref XmlElement xmlElementOut,
        ref NodeId nodeIdOut,
        ref ExpandedNodeId expandedNodeIdOut,
        ref QualifiedName qualifiedNameOut,
        ref LocalizedText localizedTextOut,
        ref StatusCode statusCodeOut,
        ref object variantOut);
    #endif
    #endregion

    #region UserArrayValueObjectState Class
    #if (!OPCUA_EXCLUDE_UserArrayValueObjectState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class UserArrayValueObjectState : TestDataObjectState
    {
        #region Constructors
        /// <remarks />
        public UserArrayValueObjectState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.UserArrayValueObjectType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAIAAAAFVzZXJBcnJheVZhbHVlT2JqZWN0VHlwZUluc3RhbmNlAQEj" +
           "AwEBIwMjAwAAAQAAAAAkAAEBJwMZAAAANWCJCgIAAAABABAAAABTaW11bGF0aW9uQWN0aXZlAQEkAwMA" +
           "AAAARwAAAElmIHRydWUgdGhlIHNlcnZlciB3aWxsIHByb2R1Y2UgbmV3IHZhbHVlcyBmb3IgZWFjaCBt" +
           "b25pdG9yZWQgdmFyaWFibGUuAC4ARCQDAAAAAf////8BAf////8AAAAABGGCCgQAAAABAA4AAABHZW5l" +
           "cmF0ZVZhbHVlcwEBJQMALwEBEQAlAwAAAQH/////AQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVu" +
           "dHMBASYDAC4ARCYDAACWAQAAAAEAKgEBRgAAAAoAAABJdGVyYXRpb25zAAf/////AAAAAAMAAAAAJQAA" +
           "AFRoZSBudW1iZXIgb2YgbmV3IHZhbHVlcyB0byBnZW5lcmF0ZS4BACgBAQAAAAEAAAAAAAAAAQH/////" +
           "AAAAAARggAoBAAAAAQANAAAAQ3ljbGVDb21wbGV0ZQEBJwMALwEAQQsnAwAAAQAAAAAkAQEBIwMXAAAA" +
           "FWCJCgIAAAAAAAcAAABFdmVudElkAQEoAwAuAEQoAwAAAA//////AQH/////AAAAABVgiQoCAAAAAAAJ" +
           "AAAARXZlbnRUeXBlAQEpAwAuAEQpAwAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNl" +
           "Tm9kZQEBKgMALgBEKgMAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJjZU5hbWUBASsD" +
           "AC4ARCsDAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAQAAABUaW1lAQEsAwAuAEQsAwAAAQAmAf//" +
           "//8BAf////8AAAAAFWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEBLQMALgBELQMAAAEAJgH/////AQH/" +
           "////AAAAABVgiQoCAAAAAAAHAAAATWVzc2FnZQEBLwMALgBELwMAAAAV/////wEB/////wAAAAAVYIkK" +
           "AgAAAAAACAAAAFNldmVyaXR5AQEwAwAuAEQwAwAAAAX/////AQH/////AAAAABVgiQoCAAAAAAAQAAAA" +
           "Q29uZGl0aW9uQ2xhc3NJZAEBMQMALgBEMQMAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAAEgAAAENv" +
           "bmRpdGlvbkNsYXNzTmFtZQEBMgMALgBEMgMAAAAV/////wEB/////wAAAAAVYIkKAgAAAAAADQAAAENv" +
           "bmRpdGlvbk5hbWUBATUDAC4ARDUDAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAgAAABCcmFuY2hJ" +
           "ZAEBNgMALgBENgMAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAABgAAAFJldGFpbgEBNwMALgBENwMA" +
           "AAAB/////wEB/////wAAAAAVYIkKAgAAAAAADAAAAEVuYWJsZWRTdGF0ZQEBOAMALwEAIyM4AwAAABX/" +
           "////AQECAAAAAQAsIwABAUwDAQAsIwABAVUDAQAAABVgiQoCAAAAAAACAAAASWQBATkDAC4ARDkDAAAA" +
           "Af////8BAf////8AAAAAFWCJCgIAAAAAAAcAAABRdWFsaXR5AQFBAwAvAQAqI0EDAAAAE/////8BAf//" +
           "//8BAAAAFWCJCgIAAAAAAA8AAABTb3VyY2VUaW1lc3RhbXABAUIDAC4AREIDAAABACYB/////wEB////" +
           "/wAAAAAVYIkKAgAAAAAADAAAAExhc3RTZXZlcml0eQEBQwMALwEAKiNDAwAAAAX/////AQH/////AQAA" +
           "ABVgiQoCAAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQFEAwAuAEREAwAAAQAmAf////8BAf////8AAAAA" +
           "FWCJCgIAAAAAAAcAAABDb21tZW50AQFFAwAvAQAqI0UDAAAAFf////8BAf////8BAAAAFWCJCgIAAAAA" +
           "AA8AAABTb3VyY2VUaW1lc3RhbXABAUYDAC4AREYDAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAA" +
           "DAAAAENsaWVudFVzZXJJZAEBRwMALgBERwMAAAAM/////wEB/////wAAAAAEYYIKBAAAAAAABwAAAERp" +
           "c2FibGUBAUgDAC8BAEQjSAMAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAABgAAAEVuYWJsZQEB" +
           "SQMALwEAQyNJAwAAAQEBAAAAAQD5CwABAPMKAAAAAARhggoEAAAAAAAKAAAAQWRkQ29tbWVudAEBSgMA" +
           "LwEARSNKAwAAAQEBAAAAAQD5CwABAA0LAQAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAUsD" +
           "AC4AREsDAACWAgAAAAEAKgEBRgAAAAcAAABFdmVudElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVu" +
           "dGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVudC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAA" +
           "AAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQgdG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAA" +
           "AAAAAQH/////AAAAABVgiQoCAAAAAAAKAAAAQWNrZWRTdGF0ZQEBTAMALwEAIyNMAwAAABX/////AQEB" +
           "AAAAAQAsIwEBATgDAQAAABVgiQoCAAAAAAACAAAASWQBAU0DAC4ARE0DAAAAAf////8BAf////8AAAAA" +
           "BGGCCgQAAAAAAAsAAABBY2tub3dsZWRnZQEBXgMALwEAlyNeAwAAAQEBAAAAAQD5CwABAPAiAQAAABdg" +
           "qQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAV8DAC4ARF8DAACWAgAAAAEAKgEBRgAAAAcAAABFdmVu" +
           "dElkAA//////AAAAAAMAAAAAKAAAAFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdG8gY29tbWVu" +
           "dC4BACoBAUIAAAAHAAAAQ29tbWVudAAV/////wAAAAADAAAAACQAAABUaGUgY29tbWVudCB0byBhZGQg" +
           "dG8gdGhlIGNvbmRpdGlvbi4BACgBAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQAMAAAAQm9v" +
           "bGVhblZhbHVlAQFiAwAvAD9iAwAAAQGwAgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEACgAA" +
           "AFNCeXRlVmFsdWUBAWMDAC8AP2MDAAABAbECAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQAJ" +
           "AAAAQnl0ZVZhbHVlAQFkAwAvAD9kAwAAAQGyAgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEA" +
           "CgAAAEludDE2VmFsdWUBAWUDAC8AP2UDAAABAbMCAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAA" +
           "AQALAAAAVUludDE2VmFsdWUBAWYDAC8AP2YDAAABAbQCAQAAAAEAAAAAAAAAAQH/////AAAAABdgiQoC" +
           "AAAAAQAKAAAASW50MzJWYWx1ZQEBZwMALwA/ZwMAAAEBtQIBAAAAAQAAAAAAAAABAf////8AAAAAF2CJ" +
           "CgIAAAABAAsAAABVSW50MzJWYWx1ZQEBaAMALwA/aAMAAAEBtgIBAAAAAQAAAAAAAAABAf////8AAAAA" +
           "F2CJCgIAAAABAAoAAABJbnQ2NFZhbHVlAQFpAwAvAD9pAwAAAQG3AgEAAAABAAAAAAAAAAEB/////wAA" +
           "AAAXYIkKAgAAAAEACwAAAFVJbnQ2NFZhbHVlAQFqAwAvAD9qAwAAAQG4AgEAAAABAAAAAAAAAAEB////" +
           "/wAAAAAXYIkKAgAAAAEACgAAAEZsb2F0VmFsdWUBAWsDAC8AP2sDAAABAbkCAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAABdgiQoCAAAAAQALAAAARG91YmxlVmFsdWUBAWwDAC8AP2wDAAABAboCAQAAAAEAAAAAAAAA" +
           "AQH/////AAAAABdgiQoCAAAAAQALAAAAU3RyaW5nVmFsdWUBAW0DAC8AP20DAAABAbsCAQAAAAEAAAAA" +
           "AAAAAQH/////AAAAABdgiQoCAAAAAQANAAAARGF0ZVRpbWVWYWx1ZQEBbgMALwA/bgMAAAEBvAIBAAAA" +
           "AQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAkAAABHdWlkVmFsdWUBAW8DAC8AP28DAAABAb0CAQAA" +
           "AAEAAAAAAAAAAQH/////AAAAABdgiQoCAAAAAQAPAAAAQnl0ZVN0cmluZ1ZhbHVlAQFwAwAvAD9wAwAA" +
           "AQG+AgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEADwAAAFhtbEVsZW1lbnRWYWx1ZQEBcQMA" +
           "LwA/cQMAAAEBvwIBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABAAsAAABOb2RlSWRWYWx1ZQEB" +
           "cgMALwA/cgMAAAEBwAIBAAAAAQAAAAAAAAABAf////8AAAAAF2CJCgIAAAABABMAAABFeHBhbmRlZE5v" +
           "ZGVJZFZhbHVlAQFzAwAvAD9zAwAAAQHBAgEAAAABAAAAAAAAAAEB/////wAAAAAXYIkKAgAAAAEAEgAA" +
           "AFF1YWxpZmllZE5hbWVWYWx1ZQEBdAMALwA/dAMAAAEBwgIBAAAAAQAAAAAAAAABAf////8AAAAAF2CJ" +
           "CgIAAAABABIAAABMb2NhbGl6ZWRUZXh0VmFsdWUBAXUDAC8AP3UDAAABAcMCAQAAAAEAAAAAAAAAAQH/" +
           "////AAAAABdgiQoCAAAAAQAPAAAAU3RhdHVzQ29kZVZhbHVlAQF2AwAvAD92AwAAAQHEAgEAAAABAAAA" +
           "AAAAAAEB/////wAAAAAXYIkKAgAAAAEADAAAAFZhcmlhbnRWYWx1ZQEBdwMALwA/dwMAAAEBxQIBAAAA" +
           "AQAAAAAAAAABAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public BaseDataVariableState<bool[]> BooleanValue
        {
            get
            {
                return m_booleanValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_booleanValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_booleanValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<sbyte[]> SByteValue
        {
            get
            {
                return m_sByteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_sByteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_sByteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[]> ByteValue
        {
            get
            {
                return m_byteValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<short[]> Int16Value
        {
            get
            {
                return m_int16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ushort[]> UInt16Value
        {
            get
            {
                return m_uInt16Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt16Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt16Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<int[]> Int32Value
        {
            get
            {
                return m_int32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<uint[]> UInt32Value
        {
            get
            {
                return m_uInt32Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt32Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt32Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<long[]> Int64Value
        {
            get
            {
                return m_int64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_int64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_int64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ulong[]> UInt64Value
        {
            get
            {
                return m_uInt64Value;
            }

            set
            {
                if (!Object.ReferenceEquals(m_uInt64Value, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_uInt64Value = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<float[]> FloatValue
        {
            get
            {
                return m_floatValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_floatValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_floatValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<double[]> DoubleValue
        {
            get
            {
                return m_doubleValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_doubleValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_doubleValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<string[]> StringValue
        {
            get
            {
                return m_stringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_stringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_stringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<DateTime[]> DateTimeValue
        {
            get
            {
                return m_dateTimeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_dateTimeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_dateTimeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<Guid[]> GuidValue
        {
            get
            {
                return m_guidValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_guidValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_guidValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<byte[][]> ByteStringValue
        {
            get
            {
                return m_byteStringValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_byteStringValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_byteStringValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<XmlElement[]> XmlElementValue
        {
            get
            {
                return m_xmlElementValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_xmlElementValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_xmlElementValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<NodeId[]> NodeIdValue
        {
            get
            {
                return m_nodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_nodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_nodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<ExpandedNodeId[]> ExpandedNodeIdValue
        {
            get
            {
                return m_expandedNodeIdValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_expandedNodeIdValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_expandedNodeIdValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<QualifiedName[]> QualifiedNameValue
        {
            get
            {
                return m_qualifiedNameValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_qualifiedNameValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_qualifiedNameValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<LocalizedText[]> LocalizedTextValue
        {
            get
            {
                return m_localizedTextValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_localizedTextValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_localizedTextValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<StatusCode[]> StatusCodeValue
        {
            get
            {
                return m_statusCodeValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_statusCodeValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_statusCodeValue = value;
            }
        }

        /// <remarks />
        public BaseDataVariableState<object[]> VariantValue
        {
            get
            {
                return m_variantValue;
            }

            set
            {
                if (!Object.ReferenceEquals(m_variantValue, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_variantValue = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_booleanValue != null)
            {
                children.Add(m_booleanValue);
            }

            if (m_sByteValue != null)
            {
                children.Add(m_sByteValue);
            }

            if (m_byteValue != null)
            {
                children.Add(m_byteValue);
            }

            if (m_int16Value != null)
            {
                children.Add(m_int16Value);
            }

            if (m_uInt16Value != null)
            {
                children.Add(m_uInt16Value);
            }

            if (m_int32Value != null)
            {
                children.Add(m_int32Value);
            }

            if (m_uInt32Value != null)
            {
                children.Add(m_uInt32Value);
            }

            if (m_int64Value != null)
            {
                children.Add(m_int64Value);
            }

            if (m_uInt64Value != null)
            {
                children.Add(m_uInt64Value);
            }

            if (m_floatValue != null)
            {
                children.Add(m_floatValue);
            }

            if (m_doubleValue != null)
            {
                children.Add(m_doubleValue);
            }

            if (m_stringValue != null)
            {
                children.Add(m_stringValue);
            }

            if (m_dateTimeValue != null)
            {
                children.Add(m_dateTimeValue);
            }

            if (m_guidValue != null)
            {
                children.Add(m_guidValue);
            }

            if (m_byteStringValue != null)
            {
                children.Add(m_byteStringValue);
            }

            if (m_xmlElementValue != null)
            {
                children.Add(m_xmlElementValue);
            }

            if (m_nodeIdValue != null)
            {
                children.Add(m_nodeIdValue);
            }

            if (m_expandedNodeIdValue != null)
            {
                children.Add(m_expandedNodeIdValue);
            }

            if (m_qualifiedNameValue != null)
            {
                children.Add(m_qualifiedNameValue);
            }

            if (m_localizedTextValue != null)
            {
                children.Add(m_localizedTextValue);
            }

            if (m_statusCodeValue != null)
            {
                children.Add(m_statusCodeValue);
            }

            if (m_variantValue != null)
            {
                children.Add(m_variantValue);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.BooleanValue:
                {
                    if (createOrReplace)
                    {
                        if (BooleanValue == null)
                        {
                            if (replacement == null)
                            {
                                BooleanValue = new BaseDataVariableState<bool[]>(this);
                            }
                            else
                            {
                                BooleanValue = (BaseDataVariableState<bool[]>)replacement;
                            }
                        }
                    }

                    instance = BooleanValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.SByteValue:
                {
                    if (createOrReplace)
                    {
                        if (SByteValue == null)
                        {
                            if (replacement == null)
                            {
                                SByteValue = new BaseDataVariableState<sbyte[]>(this);
                            }
                            else
                            {
                                SByteValue = (BaseDataVariableState<sbyte[]>)replacement;
                            }
                        }
                    }

                    instance = SByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteValue = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                ByteValue = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ByteValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int16Value:
                {
                    if (createOrReplace)
                    {
                        if (Int16Value == null)
                        {
                            if (replacement == null)
                            {
                                Int16Value = new BaseDataVariableState<short[]>(this);
                            }
                            else
                            {
                                Int16Value = (BaseDataVariableState<short[]>)replacement;
                            }
                        }
                    }

                    instance = Int16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt16Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt16Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt16Value = new BaseDataVariableState<ushort[]>(this);
                            }
                            else
                            {
                                UInt16Value = (BaseDataVariableState<ushort[]>)replacement;
                            }
                        }
                    }

                    instance = UInt16Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int32Value:
                {
                    if (createOrReplace)
                    {
                        if (Int32Value == null)
                        {
                            if (replacement == null)
                            {
                                Int32Value = new BaseDataVariableState<int[]>(this);
                            }
                            else
                            {
                                Int32Value = (BaseDataVariableState<int[]>)replacement;
                            }
                        }
                    }

                    instance = Int32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt32Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt32Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt32Value = new BaseDataVariableState<uint[]>(this);
                            }
                            else
                            {
                                UInt32Value = (BaseDataVariableState<uint[]>)replacement;
                            }
                        }
                    }

                    instance = UInt32Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Int64Value:
                {
                    if (createOrReplace)
                    {
                        if (Int64Value == null)
                        {
                            if (replacement == null)
                            {
                                Int64Value = new BaseDataVariableState<long[]>(this);
                            }
                            else
                            {
                                Int64Value = (BaseDataVariableState<long[]>)replacement;
                            }
                        }
                    }

                    instance = Int64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UInt64Value:
                {
                    if (createOrReplace)
                    {
                        if (UInt64Value == null)
                        {
                            if (replacement == null)
                            {
                                UInt64Value = new BaseDataVariableState<ulong[]>(this);
                            }
                            else
                            {
                                UInt64Value = (BaseDataVariableState<ulong[]>)replacement;
                            }
                        }
                    }

                    instance = UInt64Value;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.FloatValue:
                {
                    if (createOrReplace)
                    {
                        if (FloatValue == null)
                        {
                            if (replacement == null)
                            {
                                FloatValue = new BaseDataVariableState<float[]>(this);
                            }
                            else
                            {
                                FloatValue = (BaseDataVariableState<float[]>)replacement;
                            }
                        }
                    }

                    instance = FloatValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DoubleValue:
                {
                    if (createOrReplace)
                    {
                        if (DoubleValue == null)
                        {
                            if (replacement == null)
                            {
                                DoubleValue = new BaseDataVariableState<double[]>(this);
                            }
                            else
                            {
                                DoubleValue = (BaseDataVariableState<double[]>)replacement;
                            }
                        }
                    }

                    instance = DoubleValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StringValue:
                {
                    if (createOrReplace)
                    {
                        if (StringValue == null)
                        {
                            if (replacement == null)
                            {
                                StringValue = new BaseDataVariableState<string[]>(this);
                            }
                            else
                            {
                                StringValue = (BaseDataVariableState<string[]>)replacement;
                            }
                        }
                    }

                    instance = StringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.DateTimeValue:
                {
                    if (createOrReplace)
                    {
                        if (DateTimeValue == null)
                        {
                            if (replacement == null)
                            {
                                DateTimeValue = new BaseDataVariableState<DateTime[]>(this);
                            }
                            else
                            {
                                DateTimeValue = (BaseDataVariableState<DateTime[]>)replacement;
                            }
                        }
                    }

                    instance = DateTimeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.GuidValue:
                {
                    if (createOrReplace)
                    {
                        if (GuidValue == null)
                        {
                            if (replacement == null)
                            {
                                GuidValue = new BaseDataVariableState<Guid[]>(this);
                            }
                            else
                            {
                                GuidValue = (BaseDataVariableState<Guid[]>)replacement;
                            }
                        }
                    }

                    instance = GuidValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ByteStringValue:
                {
                    if (createOrReplace)
                    {
                        if (ByteStringValue == null)
                        {
                            if (replacement == null)
                            {
                                ByteStringValue = new BaseDataVariableState<byte[][]>(this);
                            }
                            else
                            {
                                ByteStringValue = (BaseDataVariableState<byte[][]>)replacement;
                            }
                        }
                    }

                    instance = ByteStringValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.XmlElementValue:
                {
                    if (createOrReplace)
                    {
                        if (XmlElementValue == null)
                        {
                            if (replacement == null)
                            {
                                XmlElementValue = new BaseDataVariableState<XmlElement[]>(this);
                            }
                            else
                            {
                                XmlElementValue = (BaseDataVariableState<XmlElement[]>)replacement;
                            }
                        }
                    }

                    instance = XmlElementValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.NodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (NodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                NodeIdValue = new BaseDataVariableState<NodeId[]>(this);
                            }
                            else
                            {
                                NodeIdValue = (BaseDataVariableState<NodeId[]>)replacement;
                            }
                        }
                    }

                    instance = NodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ExpandedNodeIdValue:
                {
                    if (createOrReplace)
                    {
                        if (ExpandedNodeIdValue == null)
                        {
                            if (replacement == null)
                            {
                                ExpandedNodeIdValue = new BaseDataVariableState<ExpandedNodeId[]>(this);
                            }
                            else
                            {
                                ExpandedNodeIdValue = (BaseDataVariableState<ExpandedNodeId[]>)replacement;
                            }
                        }
                    }

                    instance = ExpandedNodeIdValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.QualifiedNameValue:
                {
                    if (createOrReplace)
                    {
                        if (QualifiedNameValue == null)
                        {
                            if (replacement == null)
                            {
                                QualifiedNameValue = new BaseDataVariableState<QualifiedName[]>(this);
                            }
                            else
                            {
                                QualifiedNameValue = (BaseDataVariableState<QualifiedName[]>)replacement;
                            }
                        }
                    }

                    instance = QualifiedNameValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.LocalizedTextValue:
                {
                    if (createOrReplace)
                    {
                        if (LocalizedTextValue == null)
                        {
                            if (replacement == null)
                            {
                                LocalizedTextValue = new BaseDataVariableState<LocalizedText[]>(this);
                            }
                            else
                            {
                                LocalizedTextValue = (BaseDataVariableState<LocalizedText[]>)replacement;
                            }
                        }
                    }

                    instance = LocalizedTextValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.StatusCodeValue:
                {
                    if (createOrReplace)
                    {
                        if (StatusCodeValue == null)
                        {
                            if (replacement == null)
                            {
                                StatusCodeValue = new BaseDataVariableState<StatusCode[]>(this);
                            }
                            else
                            {
                                StatusCodeValue = (BaseDataVariableState<StatusCode[]>)replacement;
                            }
                        }
                    }

                    instance = StatusCodeValue;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.VariantValue:
                {
                    if (createOrReplace)
                    {
                        if (VariantValue == null)
                        {
                            if (replacement == null)
                            {
                                VariantValue = new BaseDataVariableState<object[]>(this);
                            }
                            else
                            {
                                VariantValue = (BaseDataVariableState<object[]>)replacement;
                            }
                        }
                    }

                    instance = VariantValue;
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
        private BaseDataVariableState<bool[]> m_booleanValue;
        private BaseDataVariableState<sbyte[]> m_sByteValue;
        private BaseDataVariableState<byte[]> m_byteValue;
        private BaseDataVariableState<short[]> m_int16Value;
        private BaseDataVariableState<ushort[]> m_uInt16Value;
        private BaseDataVariableState<int[]> m_int32Value;
        private BaseDataVariableState<uint[]> m_uInt32Value;
        private BaseDataVariableState<long[]> m_int64Value;
        private BaseDataVariableState<ulong[]> m_uInt64Value;
        private BaseDataVariableState<float[]> m_floatValue;
        private BaseDataVariableState<double[]> m_doubleValue;
        private BaseDataVariableState<string[]> m_stringValue;
        private BaseDataVariableState<DateTime[]> m_dateTimeValue;
        private BaseDataVariableState<Guid[]> m_guidValue;
        private BaseDataVariableState<byte[][]> m_byteStringValue;
        private BaseDataVariableState<XmlElement[]> m_xmlElementValue;
        private BaseDataVariableState<NodeId[]> m_nodeIdValue;
        private BaseDataVariableState<ExpandedNodeId[]> m_expandedNodeIdValue;
        private BaseDataVariableState<QualifiedName[]> m_qualifiedNameValue;
        private BaseDataVariableState<LocalizedText[]> m_localizedTextValue;
        private BaseDataVariableState<StatusCode[]> m_statusCodeValue;
        private BaseDataVariableState<object[]> m_variantValue;
        #endregion
    }
    #endif
    #endregion

    #region VectorVariableState Class
    #if (!OPCUA_EXCLUDE_VectorVariableState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class VectorVariableState : BaseDataVariableState<Vector>
    {
        #region Constructors
        /// <remarks />
        public VectorVariableState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.VariableTypes.VectorVariableType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        /// <remarks />
        protected override NodeId GetDefaultDataTypeId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.DataTypes.Vector, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        /// <remarks />
        protected override int GetDefaultValueRank()
        {
            return ValueRanks.Scalar;
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8VYIECAgAAAAEAGgAAAFZlY3RvclZhcmlhYmxlVHlwZUluc3RhbmNlAQF5AwEBeQN5" +
           "AwAAAQF4AwEB/////wMAAAAVYIkKAgAAAAEAAQAAAFgBAXoDAC4ARHoDAAAAC/////8BAf////8AAAAA" +
           "FWCJCgIAAAABAAEAAABZAQF7AwAuAER7AwAAAAv/////AQH/////AAAAABVgiQoCAAAAAQABAAAAWgEB" +
           "fAMALgBEfAMAAAAL/////wEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<double> X
        {
            get
            {
                return m_x;
            }

            set
            {
                if (!Object.ReferenceEquals(m_x, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_x = value;
            }
        }

        /// <remarks />
        public PropertyState<double> Y
        {
            get
            {
                return m_y;
            }

            set
            {
                if (!Object.ReferenceEquals(m_y, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_y = value;
            }
        }

        /// <remarks />
        public PropertyState<double> Z
        {
            get
            {
                return m_z;
            }

            set
            {
                if (!Object.ReferenceEquals(m_z, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_z = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_x != null)
            {
                children.Add(m_x);
            }

            if (m_y != null)
            {
                children.Add(m_y);
            }

            if (m_z != null)
            {
                children.Add(m_z);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.X:
                {
                    if (createOrReplace)
                    {
                        if (X == null)
                        {
                            if (replacement == null)
                            {
                                X = new PropertyState<double>(this);
                            }
                            else
                            {
                                X = (PropertyState<double>)replacement;
                            }
                        }
                    }

                    instance = X;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Y:
                {
                    if (createOrReplace)
                    {
                        if (Y == null)
                        {
                            if (replacement == null)
                            {
                                Y = new PropertyState<double>(this);
                            }
                            else
                            {
                                Y = (PropertyState<double>)replacement;
                            }
                        }
                    }

                    instance = Y;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.Z:
                {
                    if (createOrReplace)
                    {
                        if (Z == null)
                        {
                            if (replacement == null)
                            {
                                Z = new PropertyState<double>(this);
                            }
                            else
                            {
                                Z = (PropertyState<double>)replacement;
                            }
                        }
                    }

                    instance = Z;
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
        private PropertyState<double> m_x;
        private PropertyState<double> m_y;
        private PropertyState<double> m_z;
        #endregion
    }

    #region VectorVariableValue Class
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public class VectorVariableValue : BaseVariableValue
    {
        #region Constructors
        /// <remarks />
        public VectorVariableValue(VectorVariableState variable, Vector value, object dataLock) : base(dataLock)
        {
            m_value = value;

            if (m_value == null)
            {
                m_value = new Vector();
            }

            Initialize(variable);
        }
        #endregion

        #region Public Members
        /// <remarks />
        public VectorVariableState Variable
        {
            get { return m_variable; }
        }

        /// <remarks />
        public Vector Value
        {
            get { return m_value;  }
            set { m_value = value; }
        }
        #endregion

        #region Private Methods
        private void Initialize(VectorVariableState variable)
        {
            lock (Lock)
            {
                m_variable = variable;

                variable.Value = m_value;

                variable.OnReadValue = OnReadValue;
                variable.OnSimpleWriteValue = OnWriteValue;

                BaseVariableState instance = null;
                List<BaseInstanceState> updateList = new List<BaseInstanceState>();
                updateList.Add(variable);

                instance = m_variable.X;
                instance.OnReadValue = OnRead_X;
                instance.OnSimpleWriteValue = OnWrite_X;
                updateList.Add(instance);
                instance = m_variable.Y;
                instance.OnReadValue = OnRead_Y;
                instance.OnSimpleWriteValue = OnWrite_Y;
                updateList.Add(instance);
                instance = m_variable.Z;
                instance.OnReadValue = OnRead_Z;
                instance.OnSimpleWriteValue = OnWrite_Z;
                updateList.Add(instance);

                SetUpdateList(updateList);
            }
        }

        /// <remarks />
        protected ServiceResult OnReadValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        private ServiceResult OnWriteValue(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value = (Vector)Write(value);
            }

            return ServiceResult.Good;
        }

        #region X Access Methods
        /// <remarks />
        private ServiceResult OnRead_X(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.X;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_X(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.X = (double)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region Y Access Methods
        /// <remarks />
        private ServiceResult OnRead_Y(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.Y;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_Y(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.Y = (double)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion

        #region Z Access Methods
        /// <remarks />
        private ServiceResult OnRead_Z(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            lock (Lock)
            {
                DoBeforeReadProcessing(context, node);

                if (m_value != null)
                {
                    value = m_value.Z;
                }

                return Read(context, node, indexRange, dataEncoding, ref value, ref statusCode, ref timestamp);
            }
        }

        /// <remarks />
        private ServiceResult OnWrite_Z(ISystemContext context, NodeState node, ref object value)
        {
            lock (Lock)
            {
                m_value.Z = (double)Write(value);
            }

            return ServiceResult.Good;
        }
        #endregion
        #endregion

        #region Private Fields
        private Vector m_value;
        private VectorVariableState m_variable;
        #endregion
    }
    #endregion
    #endif
    #endregion

    #region UserArrayValue1MethodState Class
    #if (!OPCUA_EXCLUDE_UserArrayValue1MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class UserArrayValue1MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public UserArrayValue1MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new UserArrayValue1MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAGQAAAFVzZXJBcnJheVZhbHVlMU1ldGhvZFR5cGUBAX8DAC8BAX8D" +
           "fwMAAAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQGAAwAuAESAAwAAlgwAAAAB" +
           "ACoBAR4AAAAJAAAAQm9vbGVhbkluAQGwAgEAAAABAAAAAAAAAAABACoBARwAAAAHAAAAU0J5dGVJbgEB" +
           "sQIBAAAAAQAAAAAAAAAAAQAqAQEbAAAABgAAAEJ5dGVJbgEBsgIBAAAAAQAAAAAAAAAAAQAqAQEcAAAA" +
           "BwAAAEludDE2SW4BAbMCAQAAAAEAAAAAAAAAAAEAKgEBHQAAAAgAAABVSW50MTZJbgEBtAIBAAAAAQAA" +
           "AAAAAAAAAQAqAQEcAAAABwAAAEludDMySW4BAbUCAQAAAAEAAAAAAAAAAAEAKgEBHQAAAAgAAABVSW50" +
           "MzJJbgEBtgIBAAAAAQAAAAAAAAAAAQAqAQEcAAAABwAAAEludDY0SW4BAbcCAQAAAAEAAAAAAAAAAAEA" +
           "KgEBHQAAAAgAAABVSW50NjRJbgEBuAIBAAAAAQAAAAAAAAAAAQAqAQEcAAAABwAAAEZsb2F0SW4BAbkC" +
           "AQAAAAEAAAAAAAAAAAEAKgEBHQAAAAgAAABEb3VibGVJbgEBugIBAAAAAQAAAAAAAAAAAQAqAQEdAAAA" +
           "CAAAAFN0cmluZ0luAQG7AgEAAAABAAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAABdgqQoC" +
           "AAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQGBAwAuAESBAwAAlgwAAAABACoBAR8AAAAKAAAAQm9vbGVh" +
           "bk91dAEBsAIBAAAAAQAAAAAAAAAAAQAqAQEdAAAACAAAAFNCeXRlT3V0AQGxAgEAAAABAAAAAAAAAAAB" +
           "ACoBARwAAAAHAAAAQnl0ZU91dAEBsgIBAAAAAQAAAAAAAAAAAQAqAQEdAAAACAAAAEludDE2T3V0AQGz" +
           "AgEAAAABAAAAAAAAAAABACoBAR4AAAAJAAAAVUludDE2T3V0AQG0AgEAAAABAAAAAAAAAAABACoBAR0A" +
           "AAAIAAAASW50MzJPdXQBAbUCAQAAAAEAAAAAAAAAAAEAKgEBHgAAAAkAAABVSW50MzJPdXQBAbYCAQAA" +
           "AAEAAAAAAAAAAAEAKgEBHQAAAAgAAABJbnQ2NE91dAEBtwIBAAAAAQAAAAAAAAAAAQAqAQEeAAAACQAA" +
           "AFVJbnQ2NE91dAEBuAIBAAAAAQAAAAAAAAAAAQAqAQEdAAAACAAAAEZsb2F0T3V0AQG5AgEAAAABAAAA" +
           "AAAAAAABACoBAR4AAAAJAAAARG91YmxlT3V0AQG6AgEAAAABAAAAAAAAAAABACoBAR4AAAAJAAAAU3Ry" +
           "aW5nT3V0AQG7AgEAAAABAAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public UserArrayValue1MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            bool[] booleanIn = (bool[])_inputArguments[0];
            sbyte[] sByteIn = (sbyte[])_inputArguments[1];
            byte[] byteIn = (byte[])_inputArguments[2];
            short[] int16In = (short[])_inputArguments[3];
            ushort[] uInt16In = (ushort[])_inputArguments[4];
            int[] int32In = (int[])_inputArguments[5];
            uint[] uInt32In = (uint[])_inputArguments[6];
            long[] int64In = (long[])_inputArguments[7];
            ulong[] uInt64In = (ulong[])_inputArguments[8];
            float[] floatIn = (float[])_inputArguments[9];
            double[] doubleIn = (double[])_inputArguments[10];
            string[] stringIn = (string[])_inputArguments[11];

            bool[] booleanOut = (bool[])_outputArguments[0];
            sbyte[] sByteOut = (sbyte[])_outputArguments[1];
            byte[] byteOut = (byte[])_outputArguments[2];
            short[] int16Out = (short[])_outputArguments[3];
            ushort[] uInt16Out = (ushort[])_outputArguments[4];
            int[] int32Out = (int[])_outputArguments[5];
            uint[] uInt32Out = (uint[])_outputArguments[6];
            long[] int64Out = (long[])_outputArguments[7];
            ulong[] uInt64Out = (ulong[])_outputArguments[8];
            float[] floatOut = (float[])_outputArguments[9];
            double[] doubleOut = (double[])_outputArguments[10];
            string[] stringOut = (string[])_outputArguments[11];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    booleanIn,
                    sByteIn,
                    byteIn,
                    int16In,
                    uInt16In,
                    int32In,
                    uInt32In,
                    int64In,
                    uInt64In,
                    floatIn,
                    doubleIn,
                    stringIn,
                    ref booleanOut,
                    ref sByteOut,
                    ref byteOut,
                    ref int16Out,
                    ref uInt16Out,
                    ref int32Out,
                    ref uInt32Out,
                    ref int64Out,
                    ref uInt64Out,
                    ref floatOut,
                    ref doubleOut,
                    ref stringOut);
            }

            _outputArguments[0] = booleanOut;
            _outputArguments[1] = sByteOut;
            _outputArguments[2] = byteOut;
            _outputArguments[3] = int16Out;
            _outputArguments[4] = uInt16Out;
            _outputArguments[5] = int32Out;
            _outputArguments[6] = uInt32Out;
            _outputArguments[7] = int64Out;
            _outputArguments[8] = uInt64Out;
            _outputArguments[9] = floatOut;
            _outputArguments[10] = doubleOut;
            _outputArguments[11] = stringOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult UserArrayValue1MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        bool[] booleanIn,
        sbyte[] sByteIn,
        byte[] byteIn,
        short[] int16In,
        ushort[] uInt16In,
        int[] int32In,
        uint[] uInt32In,
        long[] int64In,
        ulong[] uInt64In,
        float[] floatIn,
        double[] doubleIn,
        string[] stringIn,
        ref bool[] booleanOut,
        ref sbyte[] sByteOut,
        ref byte[] byteOut,
        ref short[] int16Out,
        ref ushort[] uInt16Out,
        ref int[] int32Out,
        ref uint[] uInt32Out,
        ref long[] int64Out,
        ref ulong[] uInt64Out,
        ref float[] floatOut,
        ref double[] doubleOut,
        ref string[] stringOut);
    #endif
    #endregion

    #region UserArrayValue2MethodState Class
    #if (!OPCUA_EXCLUDE_UserArrayValue2MethodState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class UserArrayValue2MethodState : MethodState
    {
        #region Constructors
        /// <remarks />
        public UserArrayValue2MethodState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        public new static NodeState Construct(NodeState parent)
        {
            return new UserArrayValue2MethodState(parent);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYYIKBAAAAAEAGQAAAFVzZXJBcnJheVZhbHVlMk1ldGhvZFR5cGUBAYIDAC8BAYID" +
           "ggMAAAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQGDAwAuAESDAwAAlgoAAAAB" +
           "ACoBAR8AAAAKAAAARGF0ZVRpbWVJbgEBvAIBAAAAAQAAAAAAAAAAAQAqAQEbAAAABgAAAEd1aWRJbgEB" +
           "vQIBAAAAAQAAAAAAAAAAAQAqAQEhAAAADAAAAEJ5dGVTdHJpbmdJbgEBvgIBAAAAAQAAAAAAAAAAAQAq" +
           "AQEhAAAADAAAAFhtbEVsZW1lbnRJbgEBvwIBAAAAAQAAAAAAAAAAAQAqAQEdAAAACAAAAE5vZGVJZElu" +
           "AQHAAgEAAAABAAAAAAAAAAABACoBASUAAAAQAAAARXhwYW5kZWROb2RlSWRJbgEBwQIBAAAAAQAAAAAA" +
           "AAAAAQAqAQEkAAAADwAAAFF1YWxpZmllZE5hbWVJbgEBwgIBAAAAAQAAAAAAAAAAAQAqAQEkAAAADwAA" +
           "AExvY2FsaXplZFRleHRJbgEBwwIBAAAAAQAAAAAAAAAAAQAqAQEhAAAADAAAAFN0YXR1c0NvZGVJbgEB" +
           "xAIBAAAAAQAAAAAAAAAAAQAqAQEeAAAACQAAAFZhcmlhbnRJbgEBxQIBAAAAAQAAAAAAAAAAAQAoAQEA" +
           "AAABAAAAAAAAAAEB/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBhAMALgBEhAMA" +
           "AJYKAAAAAQAqAQEgAAAACwAAAERhdGVUaW1lT3V0AQG8AgEAAAABAAAAAAAAAAABACoBARwAAAAHAAAA" +
           "R3VpZE91dAEBvQIBAAAAAQAAAAAAAAAAAQAqAQEiAAAADQAAAEJ5dGVTdHJpbmdPdXQBAb4CAQAAAAEA" +
           "AAAAAAAAAAEAKgEBIgAAAA0AAABYbWxFbGVtZW50T3V0AQG/AgEAAAABAAAAAAAAAAABACoBAR4AAAAJ" +
           "AAAATm9kZUlkT3V0AQHAAgEAAAABAAAAAAAAAAABACoBASYAAAARAAAARXhwYW5kZWROb2RlSWRPdXQB" +
           "AcECAQAAAAEAAAAAAAAAAAEAKgEBJQAAABAAAABRdWFsaWZpZWROYW1lT3V0AQHCAgEAAAABAAAAAAAA" +
           "AAABACoBASUAAAAQAAAATG9jYWxpemVkVGV4dE91dAEBwwIBAAAAAQAAAAAAAAAAAQAqAQEiAAAADQAA" +
           "AFN0YXR1c0NvZGVPdXQBAcQCAQAAAAEAAAAAAAAAAAEAKgEBHwAAAAoAAABWYXJpYW50T3V0AQHFAgEA" +
           "AAABAAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Event Callbacks
        /// <remarks />
        public UserArrayValue2MethodStateMethodCallHandler OnCall;
        #endregion

        #region Public Properties
        #endregion

        #region Overridden Methods
        /// <remarks />
        protected override ServiceResult Call(
            ISystemContext _context,
            NodeId _objectId,
            IList<object> _inputArguments,
            IList<object> _outputArguments)
        {
            if (OnCall == null)
            {
                return base.Call(_context, _objectId, _inputArguments, _outputArguments);
            }

            ServiceResult _result = null;

            DateTime[] dateTimeIn = (DateTime[])_inputArguments[0];
            Uuid[] guidIn = (Uuid[])_inputArguments[1];
            byte[][] byteStringIn = (byte[][])_inputArguments[2];
            XmlElement[] xmlElementIn = (XmlElement[])_inputArguments[3];
            NodeId[] nodeIdIn = (NodeId[])_inputArguments[4];
            ExpandedNodeId[] expandedNodeIdIn = (ExpandedNodeId[])_inputArguments[5];
            QualifiedName[] qualifiedNameIn = (QualifiedName[])_inputArguments[6];
            LocalizedText[] localizedTextIn = (LocalizedText[])_inputArguments[7];
            StatusCode[] statusCodeIn = (StatusCode[])_inputArguments[8];
            Variant[] variantIn = (Variant[])_inputArguments[9];

            DateTime[] dateTimeOut = (DateTime[])_outputArguments[0];
            Uuid[] guidOut = (Uuid[])_outputArguments[1];
            byte[][] byteStringOut = (byte[][])_outputArguments[2];
            XmlElement[] xmlElementOut = (XmlElement[])_outputArguments[3];
            NodeId[] nodeIdOut = (NodeId[])_outputArguments[4];
            ExpandedNodeId[] expandedNodeIdOut = (ExpandedNodeId[])_outputArguments[5];
            QualifiedName[] qualifiedNameOut = (QualifiedName[])_outputArguments[6];
            LocalizedText[] localizedTextOut = (LocalizedText[])_outputArguments[7];
            StatusCode[] statusCodeOut = (StatusCode[])_outputArguments[8];
            Variant[] variantOut = (Variant[])_outputArguments[9];

            if (OnCall != null)
            {
                _result = OnCall(
                    _context,
                    this,
                    _objectId,
                    dateTimeIn,
                    guidIn,
                    byteStringIn,
                    xmlElementIn,
                    nodeIdIn,
                    expandedNodeIdIn,
                    qualifiedNameIn,
                    localizedTextIn,
                    statusCodeIn,
                    variantIn,
                    ref dateTimeOut,
                    ref guidOut,
                    ref byteStringOut,
                    ref xmlElementOut,
                    ref nodeIdOut,
                    ref expandedNodeIdOut,
                    ref qualifiedNameOut,
                    ref localizedTextOut,
                    ref statusCodeOut,
                    ref variantOut);
            }

            _outputArguments[0] = dateTimeOut;
            _outputArguments[1] = guidOut;
            _outputArguments[2] = byteStringOut;
            _outputArguments[3] = xmlElementOut;
            _outputArguments[4] = nodeIdOut;
            _outputArguments[5] = expandedNodeIdOut;
            _outputArguments[6] = qualifiedNameOut;
            _outputArguments[7] = localizedTextOut;
            _outputArguments[8] = statusCodeOut;
            _outputArguments[9] = variantOut;

            return _result;
        }
        #endregion

        #region Private Fields
        #endregion
    }

    /// <remarks />
    /// <exclude />
    public delegate ServiceResult UserArrayValue2MethodStateMethodCallHandler(
        ISystemContext _context,
        MethodState _method,
        NodeId _objectId,
        DateTime[] dateTimeIn,
        Uuid[] guidIn,
        byte[][] byteStringIn,
        XmlElement[] xmlElementIn,
        NodeId[] nodeIdIn,
        ExpandedNodeId[] expandedNodeIdIn,
        QualifiedName[] qualifiedNameIn,
        LocalizedText[] localizedTextIn,
        StatusCode[] statusCodeIn,
        Variant[] variantIn,
        ref DateTime[] dateTimeOut,
        ref Uuid[] guidOut,
        ref byte[][] byteStringOut,
        ref XmlElement[] xmlElementOut,
        ref NodeId[] nodeIdOut,
        ref ExpandedNodeId[] expandedNodeIdOut,
        ref QualifiedName[] qualifiedNameOut,
        ref LocalizedText[] localizedTextOut,
        ref StatusCode[] statusCodeOut,
        ref Variant[] variantOut);
    #endif
    #endregion

    #region MethodTestState Class
    #if (!OPCUA_EXCLUDE_MethodTestState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class MethodTestState : FolderState
    {
        #region Constructors
        /// <remarks />
        public MethodTestState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.MethodTestType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAFgAAAE1ldGhvZFRlc3RUeXBlSW5zdGFuY2UBAYUDAQGFA4UDAAD/" +
           "////CgAAAARhggoEAAAAAQANAAAAU2NhbGFyTWV0aG9kMQEBhgMALwEBhgOGAwAAAQH/////AgAAABdg" +
           "qQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAYcDAC4ARIcDAACWCwAAAAEAKgEBGAAAAAkAAABCb29s" +
           "ZWFuSW4AAf////8AAAAAAAEAKgEBFgAAAAcAAABTQnl0ZUluAAL/////AAAAAAABACoBARUAAAAGAAAA" +
           "Qnl0ZUluAAP/////AAAAAAABACoBARYAAAAHAAAASW50MTZJbgAE/////wAAAAAAAQAqAQEXAAAACAAA" +
           "AFVJbnQxNkluAAX/////AAAAAAABACoBARYAAAAHAAAASW50MzJJbgAG/////wAAAAAAAQAqAQEXAAAA" +
           "CAAAAFVJbnQzMkluAAf/////AAAAAAABACoBARYAAAAHAAAASW50NjRJbgAI/////wAAAAAAAQAqAQEX" +
           "AAAACAAAAFVJbnQ2NEluAAn/////AAAAAAABACoBARYAAAAHAAAARmxvYXRJbgAK/////wAAAAAAAQAq" +
           "AQEXAAAACAAAAERvdWJsZUluAAv/////AAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAABdgqQoC" +
           "AAAAAAAPAAAAT3V0cHV0QXJndW1lbnRzAQGIAwAuAESIAwAAlgsAAAABACoBARkAAAAKAAAAQm9vbGVh" +
           "bk91dAAB/////wAAAAAAAQAqAQEXAAAACAAAAFNCeXRlT3V0AAL/////AAAAAAABACoBARYAAAAHAAAA" +
           "Qnl0ZU91dAAD/////wAAAAAAAQAqAQEXAAAACAAAAEludDE2T3V0AAT/////AAAAAAABACoBARgAAAAJ" +
           "AAAAVUludDE2T3V0AAX/////AAAAAAABACoBARcAAAAIAAAASW50MzJPdXQABv////8AAAAAAAEAKgEB" +
           "GAAAAAkAAABVSW50MzJPdXQAB/////8AAAAAAAEAKgEBFwAAAAgAAABJbnQ2NE91dAAI/////wAAAAAA" +
           "AQAqAQEYAAAACQAAAFVJbnQ2NE91dAAJ/////wAAAAAAAQAqAQEXAAAACAAAAEZsb2F0T3V0AAr/////" +
           "AAAAAAABACoBARgAAAAJAAAARG91YmxlT3V0AAv/////AAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////" +
           "AAAAAARhggoEAAAAAQANAAAAU2NhbGFyTWV0aG9kMgEBiQMALwEBiQOJAwAAAQH/////AgAAABdgqQoC" +
           "AAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAYoDAC4ARIoDAACWCgAAAAEAKgEBFwAAAAgAAABTdHJpbmdJ" +
           "bgAM/////wAAAAAAAQAqAQEZAAAACgAAAERhdGVUaW1lSW4ADf////8AAAAAAAEAKgEBFQAAAAYAAABH" +
           "dWlkSW4ADv////8AAAAAAAEAKgEBGwAAAAwAAABCeXRlU3RyaW5nSW4AD/////8AAAAAAAEAKgEBGwAA" +
           "AAwAAABYbWxFbGVtZW50SW4AEP////8AAAAAAAEAKgEBFwAAAAgAAABOb2RlSWRJbgAR/////wAAAAAA" +
           "AQAqAQEfAAAAEAAAAEV4cGFuZGVkTm9kZUlkSW4AEv////8AAAAAAAEAKgEBHgAAAA8AAABRdWFsaWZp" +
           "ZWROYW1lSW4AFP////8AAAAAAAEAKgEBHgAAAA8AAABMb2NhbGl6ZWRUZXh0SW4AFf////8AAAAAAAEA" +
           "KgEBGwAAAAwAAABTdGF0dXNDb2RlSW4AE/////8AAAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8AAAAA" +
           "F2CpCgIAAAAAAA8AAABPdXRwdXRBcmd1bWVudHMBAYsDAC4ARIsDAACWCgAAAAEAKgEBGAAAAAkAAABT" +
           "dHJpbmdPdXQADP////8AAAAAAAEAKgEBGgAAAAsAAABEYXRlVGltZU91dAAN/////wAAAAAAAQAqAQEW" +
           "AAAABwAAAEd1aWRPdXQADv////8AAAAAAAEAKgEBHAAAAA0AAABCeXRlU3RyaW5nT3V0AA//////AAAA" +
           "AAABACoBARwAAAANAAAAWG1sRWxlbWVudE91dAAQ/////wAAAAAAAQAqAQEYAAAACQAAAE5vZGVJZE91" +
           "dAAR/////wAAAAAAAQAqAQEgAAAAEQAAAEV4cGFuZGVkTm9kZUlkT3V0ABL/////AAAAAAABACoBAR8A" +
           "AAAQAAAAUXVhbGlmaWVkTmFtZU91dAAU/////wAAAAAAAQAqAQEfAAAAEAAAAExvY2FsaXplZFRleHRP" +
           "dXQAFf////8AAAAAAAEAKgEBHAAAAA0AAABTdGF0dXNDb2RlT3V0ABP/////AAAAAAABACgBAQAAAAEA" +
           "AAAAAAAAAQH/////AAAAAARhggoEAAAAAQANAAAAU2NhbGFyTWV0aG9kMwEBjAMALwEBjAOMAwAAAQH/" +
           "////AgAAABdgqQoCAAAAAAAOAAAASW5wdXRBcmd1bWVudHMBAY0DAC4ARI0DAACWAwAAAAEAKgEBGAAA" +
           "AAkAAABWYXJpYW50SW4AGP////8AAAAAAAEAKgEBHAAAAA0AAABFbnVtZXJhdGlvbkluAB3/////AAAA" +
           "AAABACoBARoAAAALAAAAU3RydWN0dXJlSW4AFv////8AAAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8A" +
           "AAAAF2CpCgIAAAAAAA8AAABPdXRwdXRBcmd1bWVudHMBAY4DAC4ARI4DAACWAwAAAAEAKgEBGQAAAAoA" +
           "AABWYXJpYW50T3V0ABj/////AAAAAAABACoBAR0AAAAOAAAARW51bWVyYXRpb25PdXQAHf////8AAAAA" +
           "AAEAKgEBGwAAAAwAAABTdHJ1Y3R1cmVPdXQAFv////8AAAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8A" +
           "AAAABGGCCgQAAAABAAwAAABBcnJheU1ldGhvZDEBAY8DAC8BAY8DjwMAAAEB/////wIAAAAXYKkKAgAA" +
           "AAAADgAAAElucHV0QXJndW1lbnRzAQGQAwAuAESQAwAAlgsAAAABACoBARwAAAAJAAAAQm9vbGVhbklu" +
           "AAEBAAAAAQAAAAAAAAAAAQAqAQEaAAAABwAAAFNCeXRlSW4AAgEAAAABAAAAAAAAAAABACoBARkAAAAG" +
           "AAAAQnl0ZUluAAMBAAAAAQAAAAAAAAAAAQAqAQEaAAAABwAAAEludDE2SW4ABAEAAAABAAAAAAAAAAAB" +
           "ACoBARsAAAAIAAAAVUludDE2SW4ABQEAAAABAAAAAAAAAAABACoBARoAAAAHAAAASW50MzJJbgAGAQAA" +
           "AAEAAAAAAAAAAAEAKgEBGwAAAAgAAABVSW50MzJJbgAHAQAAAAEAAAAAAAAAAAEAKgEBGgAAAAcAAABJ" +
           "bnQ2NEluAAgBAAAAAQAAAAAAAAAAAQAqAQEbAAAACAAAAFVJbnQ2NEluAAkBAAAAAQAAAAAAAAAAAQAq" +
           "AQEaAAAABwAAAEZsb2F0SW4ACgEAAAABAAAAAAAAAAABACoBARsAAAAIAAAARG91YmxlSW4ACwEAAAAB" +
           "AAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAABdgqQoCAAAAAAAPAAAAT3V0cHV0QXJndW1l" +
           "bnRzAQGRAwAuAESRAwAAlgsAAAABACoBAR0AAAAKAAAAQm9vbGVhbk91dAABAQAAAAEAAAAAAAAAAAEA" +
           "KgEBGwAAAAgAAABTQnl0ZU91dAACAQAAAAEAAAAAAAAAAAEAKgEBGgAAAAcAAABCeXRlT3V0AAMBAAAA" +
           "AQAAAAAAAAAAAQAqAQEbAAAACAAAAEludDE2T3V0AAQBAAAAAQAAAAAAAAAAAQAqAQEcAAAACQAAAFVJ" +
           "bnQxNk91dAAFAQAAAAEAAAAAAAAAAAEAKgEBGwAAAAgAAABJbnQzMk91dAAGAQAAAAEAAAAAAAAAAAEA" +
           "KgEBHAAAAAkAAABVSW50MzJPdXQABwEAAAABAAAAAAAAAAABACoBARsAAAAIAAAASW50NjRPdXQACAEA" +
           "AAABAAAAAAAAAAABACoBARwAAAAJAAAAVUludDY0T3V0AAkBAAAAAQAAAAAAAAAAAQAqAQEbAAAACAAA" +
           "AEZsb2F0T3V0AAoBAAAAAQAAAAAAAAAAAQAqAQEcAAAACQAAAERvdWJsZU91dAALAQAAAAEAAAAAAAAA" +
           "AAEAKAEBAAAAAQAAAAAAAAABAf////8AAAAABGGCCgQAAAABAAwAAABBcnJheU1ldGhvZDIBAZIDAC8B" +
           "AZIDkgMAAAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQGTAwAuAESTAwAAlgoA" +
           "AAABACoBARsAAAAIAAAAU3RyaW5nSW4ADAEAAAABAAAAAAAAAAABACoBAR0AAAAKAAAARGF0ZVRpbWVJ" +
           "bgANAQAAAAEAAAAAAAAAAAEAKgEBGQAAAAYAAABHdWlkSW4ADgEAAAABAAAAAAAAAAABACoBAR8AAAAM" +
           "AAAAQnl0ZVN0cmluZ0luAA8BAAAAAQAAAAAAAAAAAQAqAQEfAAAADAAAAFhtbEVsZW1lbnRJbgAQAQAA" +
           "AAEAAAAAAAAAAAEAKgEBGwAAAAgAAABOb2RlSWRJbgARAQAAAAEAAAAAAAAAAAEAKgEBIwAAABAAAABF" +
           "eHBhbmRlZE5vZGVJZEluABIBAAAAAQAAAAAAAAAAAQAqAQEiAAAADwAAAFF1YWxpZmllZE5hbWVJbgAU" +
           "AQAAAAEAAAAAAAAAAAEAKgEBIgAAAA8AAABMb2NhbGl6ZWRUZXh0SW4AFQEAAAABAAAAAAAAAAABACoB" +
           "AR8AAAAMAAAAU3RhdHVzQ29kZUluABMBAAAAAQAAAAAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAA" +
           "AAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBlAMALgBElAMAAJYKAAAAAQAqAQEcAAAACQAA" +
           "AFN0cmluZ091dAAMAQAAAAEAAAAAAAAAAAEAKgEBHgAAAAsAAABEYXRlVGltZU91dAANAQAAAAEAAAAA" +
           "AAAAAAEAKgEBGgAAAAcAAABHdWlkT3V0AA4BAAAAAQAAAAAAAAAAAQAqAQEgAAAADQAAAEJ5dGVTdHJp" +
           "bmdPdXQADwEAAAABAAAAAAAAAAABACoBASAAAAANAAAAWG1sRWxlbWVudE91dAAQAQAAAAEAAAAAAAAA" +
           "AAEAKgEBHAAAAAkAAABOb2RlSWRPdXQAEQEAAAABAAAAAAAAAAABACoBASQAAAARAAAARXhwYW5kZWRO" +
           "b2RlSWRPdXQAEgEAAAABAAAAAAAAAAABACoBASMAAAAQAAAAUXVhbGlmaWVkTmFtZU91dAAUAQAAAAEA" +
           "AAAAAAAAAAEAKgEBIwAAABAAAABMb2NhbGl6ZWRUZXh0T3V0ABUBAAAAAQAAAAAAAAAAAQAqAQEgAAAA" +
           "DQAAAFN0YXR1c0NvZGVPdXQAEwEAAAABAAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAAARh" +
           "ggoEAAAAAQAMAAAAQXJyYXlNZXRob2QzAQGVAwAvAQGVA5UDAAABAf////8CAAAAF2CpCgIAAAAAAA4A" +
           "AABJbnB1dEFyZ3VtZW50cwEBlgMALgBElgMAAJYDAAAAAQAqAQEcAAAACQAAAFZhcmlhbnRJbgAYAQAA" +
           "AAEAAAAAAAAAAAEAKgEBIAAAAA0AAABFbnVtZXJhdGlvbkluAB0BAAAAAQAAAAAAAAAAAQAqAQEeAAAA" +
           "CwAAAFN0cnVjdHVyZUluABYBAAAAAQAAAAAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAXYKkK" +
           "AgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBlwMALgBElwMAAJYDAAAAAQAqAQEdAAAACgAAAFZhcmlh" +
           "bnRPdXQAGAEAAAABAAAAAAAAAAABACoBASEAAAAOAAAARW51bWVyYXRpb25PdXQAHQEAAAABAAAAAAAA" +
           "AAABACoBAR8AAAAMAAAAU3RydWN0dXJlT3V0ABYBAAAAAQAAAAAAAAAAAQAoAQEAAAABAAAAAAAAAAEB" +
           "/////wAAAAAEYYIKBAAAAAEAEQAAAFVzZXJTY2FsYXJNZXRob2QxAQGYAwAvAQGYA5gDAAABAf////8C" +
           "AAAAF2CpCgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBmQMALgBEmQMAAJYMAAAAAQAqAQEaAAAACQAA" +
           "AEJvb2xlYW5JbgEBsAL/////AAAAAAABACoBARgAAAAHAAAAU0J5dGVJbgEBsQL/////AAAAAAABACoB" +
           "ARcAAAAGAAAAQnl0ZUluAQGyAv////8AAAAAAAEAKgEBGAAAAAcAAABJbnQxNkluAQGzAv////8AAAAA" +
           "AAEAKgEBGQAAAAgAAABVSW50MTZJbgEBtAL/////AAAAAAABACoBARgAAAAHAAAASW50MzJJbgEBtQL/" +
           "////AAAAAAABACoBARkAAAAIAAAAVUludDMySW4BAbYC/////wAAAAAAAQAqAQEYAAAABwAAAEludDY0" +
           "SW4BAbcC/////wAAAAAAAQAqAQEZAAAACAAAAFVJbnQ2NEluAQG4Av////8AAAAAAAEAKgEBGAAAAAcA" +
           "AABGbG9hdEluAQG5Av////8AAAAAAAEAKgEBGQAAAAgAAABEb3VibGVJbgEBugL/////AAAAAAABACoB" +
           "ARkAAAAIAAAAU3RyaW5nSW4BAbsC/////wAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAXYKkK" +
           "AgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBmgMALgBEmgMAAJYMAAAAAQAqAQEbAAAACgAAAEJvb2xl" +
           "YW5PdXQBAbAC/////wAAAAAAAQAqAQEZAAAACAAAAFNCeXRlT3V0AQGxAv////8AAAAAAAEAKgEBGAAA" +
           "AAcAAABCeXRlT3V0AQGyAv////8AAAAAAAEAKgEBGQAAAAgAAABJbnQxNk91dAEBswL/////AAAAAAAB" +
           "ACoBARoAAAAJAAAAVUludDE2T3V0AQG0Av////8AAAAAAAEAKgEBGQAAAAgAAABJbnQzMk91dAEBtQL/" +
           "////AAAAAAABACoBARoAAAAJAAAAVUludDMyT3V0AQG2Av////8AAAAAAAEAKgEBGQAAAAgAAABJbnQ2" +
           "NE91dAEBtwL/////AAAAAAABACoBARoAAAAJAAAAVUludDY0T3V0AQG4Av////8AAAAAAAEAKgEBGQAA" +
           "AAgAAABGbG9hdE91dAEBuQL/////AAAAAAABACoBARoAAAAJAAAARG91YmxlT3V0AQG6Av////8AAAAA" +
           "AAEAKgEBGgAAAAkAAABTdHJpbmdPdXQBAbsC/////wAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAA" +
           "AAAEYYIKBAAAAAEAEQAAAFVzZXJTY2FsYXJNZXRob2QyAQGbAwAvAQGbA5sDAAABAf////8CAAAAF2Cp" +
           "CgIAAAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBnAMALgBEnAMAAJYKAAAAAQAqAQEbAAAACgAAAERhdGVU" +
           "aW1lSW4BAbwC/////wAAAAAAAQAqAQEXAAAABgAAAEd1aWRJbgEBvQL/////AAAAAAABACoBAR0AAAAM" +
           "AAAAQnl0ZVN0cmluZ0luAQG+Av////8AAAAAAAEAKgEBHQAAAAwAAABYbWxFbGVtZW50SW4BAb8C////" +
           "/wAAAAAAAQAqAQEZAAAACAAAAE5vZGVJZEluAQHAAv////8AAAAAAAEAKgEBIQAAABAAAABFeHBhbmRl" +
           "ZE5vZGVJZEluAQHBAv////8AAAAAAAEAKgEBIAAAAA8AAABRdWFsaWZpZWROYW1lSW4BAcIC/////wAA" +
           "AAAAAQAqAQEgAAAADwAAAExvY2FsaXplZFRleHRJbgEBwwL/////AAAAAAABACoBAR0AAAAMAAAAU3Rh" +
           "dHVzQ29kZUluAQHEAv////8AAAAAAAEAKgEBGgAAAAkAAABWYXJpYW50SW4BAcUC/////wAAAAAAAQAo" +
           "AQEAAAABAAAAAAAAAAEB/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEBnQMALgBE" +
           "nQMAAJYKAAAAAQAqAQEcAAAACwAAAERhdGVUaW1lT3V0AQG8Av////8AAAAAAAEAKgEBGAAAAAcAAABH" +
           "dWlkT3V0AQG9Av////8AAAAAAAEAKgEBHgAAAA0AAABCeXRlU3RyaW5nT3V0AQG+Av////8AAAAAAAEA" +
           "KgEBHgAAAA0AAABYbWxFbGVtZW50T3V0AQG/Av////8AAAAAAAEAKgEBGgAAAAkAAABOb2RlSWRPdXQB" +
           "AcAC/////wAAAAAAAQAqAQEiAAAAEQAAAEV4cGFuZGVkTm9kZUlkT3V0AQHBAv////8AAAAAAAEAKgEB" +
           "IQAAABAAAABRdWFsaWZpZWROYW1lT3V0AQHCAv////8AAAAAAAEAKgEBIQAAABAAAABMb2NhbGl6ZWRU" +
           "ZXh0T3V0AQHDAv////8AAAAAAAEAKgEBHgAAAA0AAABTdGF0dXNDb2RlT3V0AQHEAv////8AAAAAAAEA" +
           "KgEBGwAAAAoAAABWYXJpYW50T3V0AQHFAv////8AAAAAAAEAKAEBAAAAAQAAAAAAAAABAf////8AAAAA" +
           "BGGCCgQAAAABABAAAABVc2VyQXJyYXlNZXRob2QxAQGeAwAvAQGeA54DAAABAf////8CAAAAF2CpCgIA" +
           "AAAAAA4AAABJbnB1dEFyZ3VtZW50cwEBnwMALgBEnwMAAJYMAAAAAQAqAQEeAAAACQAAAEJvb2xlYW5J" +
           "bgEBsAIBAAAAAQAAAAAAAAAAAQAqAQEcAAAABwAAAFNCeXRlSW4BAbECAQAAAAEAAAAAAAAAAAEAKgEB" +
           "GwAAAAYAAABCeXRlSW4BAbICAQAAAAEAAAAAAAAAAAEAKgEBHAAAAAcAAABJbnQxNkluAQGzAgEAAAAB" +
           "AAAAAAAAAAABACoBAR0AAAAIAAAAVUludDE2SW4BAbQCAQAAAAEAAAAAAAAAAAEAKgEBHAAAAAcAAABJ" +
           "bnQzMkluAQG1AgEAAAABAAAAAAAAAAABACoBAR0AAAAIAAAAVUludDMySW4BAbYCAQAAAAEAAAAAAAAA" +
           "AAEAKgEBHAAAAAcAAABJbnQ2NEluAQG3AgEAAAABAAAAAAAAAAABACoBAR0AAAAIAAAAVUludDY0SW4B" +
           "AbgCAQAAAAEAAAAAAAAAAAEAKgEBHAAAAAcAAABGbG9hdEluAQG5AgEAAAABAAAAAAAAAAABACoBAR0A" +
           "AAAIAAAARG91YmxlSW4BAboCAQAAAAEAAAAAAAAAAAEAKgEBHQAAAAgAAABTdHJpbmdJbgEBuwIBAAAA" +
           "AQAAAAAAAAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3Vt" +
           "ZW50cwEBoAMALgBEoAMAAJYMAAAAAQAqAQEfAAAACgAAAEJvb2xlYW5PdXQBAbACAQAAAAEAAAAAAAAA" +
           "AAEAKgEBHQAAAAgAAABTQnl0ZU91dAEBsQIBAAAAAQAAAAAAAAAAAQAqAQEcAAAABwAAAEJ5dGVPdXQB" +
           "AbICAQAAAAEAAAAAAAAAAAEAKgEBHQAAAAgAAABJbnQxNk91dAEBswIBAAAAAQAAAAAAAAAAAQAqAQEe" +
           "AAAACQAAAFVJbnQxNk91dAEBtAIBAAAAAQAAAAAAAAAAAQAqAQEdAAAACAAAAEludDMyT3V0AQG1AgEA" +
           "AAABAAAAAAAAAAABACoBAR4AAAAJAAAAVUludDMyT3V0AQG2AgEAAAABAAAAAAAAAAABACoBAR0AAAAI" +
           "AAAASW50NjRPdXQBAbcCAQAAAAEAAAAAAAAAAAEAKgEBHgAAAAkAAABVSW50NjRPdXQBAbgCAQAAAAEA" +
           "AAAAAAAAAAEAKgEBHQAAAAgAAABGbG9hdE91dAEBuQIBAAAAAQAAAAAAAAAAAQAqAQEeAAAACQAAAERv" +
           "dWJsZU91dAEBugIBAAAAAQAAAAAAAAAAAQAqAQEeAAAACQAAAFN0cmluZ091dAEBuwIBAAAAAQAAAAAA" +
           "AAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAEYYIKBAAAAAEAEAAAAFVzZXJBcnJheU1ldGhvZDIB" +
           "AaEDAC8BAaEDoQMAAAEB/////wIAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQGiAwAuAESi" +
           "AwAAlgoAAAABACoBAR8AAAAKAAAARGF0ZVRpbWVJbgEBvAIBAAAAAQAAAAAAAAAAAQAqAQEbAAAABgAA" +
           "AEd1aWRJbgEBvQIBAAAAAQAAAAAAAAAAAQAqAQEhAAAADAAAAEJ5dGVTdHJpbmdJbgEBvgIBAAAAAQAA" +
           "AAAAAAAAAQAqAQEhAAAADAAAAFhtbEVsZW1lbnRJbgEBvwIBAAAAAQAAAAAAAAAAAQAqAQEdAAAACAAA" +
           "AE5vZGVJZEluAQHAAgEAAAABAAAAAAAAAAABACoBASUAAAAQAAAARXhwYW5kZWROb2RlSWRJbgEBwQIB" +
           "AAAAAQAAAAAAAAAAAQAqAQEkAAAADwAAAFF1YWxpZmllZE5hbWVJbgEBwgIBAAAAAQAAAAAAAAAAAQAq" +
           "AQEkAAAADwAAAExvY2FsaXplZFRleHRJbgEBwwIBAAAAAQAAAAAAAAAAAQAqAQEhAAAADAAAAFN0YXR1" +
           "c0NvZGVJbgEBxAIBAAAAAQAAAAAAAAAAAQAqAQEeAAAACQAAAFZhcmlhbnRJbgEBxQIBAAAAAQAAAAAA" +
           "AAAAAQAoAQEAAAABAAAAAAAAAAEB/////wAAAAAXYKkKAgAAAAAADwAAAE91dHB1dEFyZ3VtZW50cwEB" +
           "owMALgBEowMAAJYKAAAAAQAqAQEgAAAACwAAAERhdGVUaW1lT3V0AQG8AgEAAAABAAAAAAAAAAABACoB" +
           "ARwAAAAHAAAAR3VpZE91dAEBvQIBAAAAAQAAAAAAAAAAAQAqAQEiAAAADQAAAEJ5dGVTdHJpbmdPdXQB" +
           "Ab4CAQAAAAEAAAAAAAAAAAEAKgEBIgAAAA0AAABYbWxFbGVtZW50T3V0AQG/AgEAAAABAAAAAAAAAAAB" +
           "ACoBAR4AAAAJAAAATm9kZUlkT3V0AQHAAgEAAAABAAAAAAAAAAABACoBASYAAAARAAAARXhwYW5kZWRO" +
           "b2RlSWRPdXQBAcECAQAAAAEAAAAAAAAAAAEAKgEBJQAAABAAAABRdWFsaWZpZWROYW1lT3V0AQHCAgEA" +
           "AAABAAAAAAAAAAABACoBASUAAAAQAAAATG9jYWxpemVkVGV4dE91dAEBwwIBAAAAAQAAAAAAAAAAAQAq" +
           "AQEiAAAADQAAAFN0YXR1c0NvZGVPdXQBAcQCAQAAAAEAAAAAAAAAAAEAKgEBHwAAAAoAAABWYXJpYW50" +
           "T3V0AQHFAgEAAAABAAAAAAAAAAABACgBAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public ScalarValue1MethodState ScalarMethod1
        {
            get
            {
                return m_scalarMethod1Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_scalarMethod1Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_scalarMethod1Method = value;
            }
        }

        /// <remarks />
        public ScalarValue2MethodState ScalarMethod2
        {
            get
            {
                return m_scalarMethod2Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_scalarMethod2Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_scalarMethod2Method = value;
            }
        }

        /// <remarks />
        public ScalarValue3MethodState ScalarMethod3
        {
            get
            {
                return m_scalarMethod3Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_scalarMethod3Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_scalarMethod3Method = value;
            }
        }

        /// <remarks />
        public ArrayValue1MethodState ArrayMethod1
        {
            get
            {
                return m_arrayMethod1Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_arrayMethod1Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_arrayMethod1Method = value;
            }
        }

        /// <remarks />
        public ArrayValue2MethodState ArrayMethod2
        {
            get
            {
                return m_arrayMethod2Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_arrayMethod2Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_arrayMethod2Method = value;
            }
        }

        /// <remarks />
        public ArrayValue3MethodState ArrayMethod3
        {
            get
            {
                return m_arrayMethod3Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_arrayMethod3Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_arrayMethod3Method = value;
            }
        }

        /// <remarks />
        public UserScalarValue1MethodState UserScalarMethod1
        {
            get
            {
                return m_userScalarMethod1Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_userScalarMethod1Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_userScalarMethod1Method = value;
            }
        }

        /// <remarks />
        public UserScalarValue2MethodState UserScalarMethod2
        {
            get
            {
                return m_userScalarMethod2Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_userScalarMethod2Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_userScalarMethod2Method = value;
            }
        }

        /// <remarks />
        public UserArrayValue1MethodState UserArrayMethod1
        {
            get
            {
                return m_userArrayMethod1Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_userArrayMethod1Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_userArrayMethod1Method = value;
            }
        }

        /// <remarks />
        public UserArrayValue2MethodState UserArrayMethod2
        {
            get
            {
                return m_userArrayMethod2Method;
            }

            set
            {
                if (!Object.ReferenceEquals(m_userArrayMethod2Method, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_userArrayMethod2Method = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_scalarMethod1Method != null)
            {
                children.Add(m_scalarMethod1Method);
            }

            if (m_scalarMethod2Method != null)
            {
                children.Add(m_scalarMethod2Method);
            }

            if (m_scalarMethod3Method != null)
            {
                children.Add(m_scalarMethod3Method);
            }

            if (m_arrayMethod1Method != null)
            {
                children.Add(m_arrayMethod1Method);
            }

            if (m_arrayMethod2Method != null)
            {
                children.Add(m_arrayMethod2Method);
            }

            if (m_arrayMethod3Method != null)
            {
                children.Add(m_arrayMethod3Method);
            }

            if (m_userScalarMethod1Method != null)
            {
                children.Add(m_userScalarMethod1Method);
            }

            if (m_userScalarMethod2Method != null)
            {
                children.Add(m_userScalarMethod2Method);
            }

            if (m_userArrayMethod1Method != null)
            {
                children.Add(m_userArrayMethod1Method);
            }

            if (m_userArrayMethod2Method != null)
            {
                children.Add(m_userArrayMethod2Method);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.ScalarMethod1:
                {
                    if (createOrReplace)
                    {
                        if (ScalarMethod1 == null)
                        {
                            if (replacement == null)
                            {
                                ScalarMethod1 = new ScalarValue1MethodState(this);
                            }
                            else
                            {
                                ScalarMethod1 = (ScalarValue1MethodState)replacement;
                            }
                        }
                    }

                    instance = ScalarMethod1;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ScalarMethod2:
                {
                    if (createOrReplace)
                    {
                        if (ScalarMethod2 == null)
                        {
                            if (replacement == null)
                            {
                                ScalarMethod2 = new ScalarValue2MethodState(this);
                            }
                            else
                            {
                                ScalarMethod2 = (ScalarValue2MethodState)replacement;
                            }
                        }
                    }

                    instance = ScalarMethod2;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ScalarMethod3:
                {
                    if (createOrReplace)
                    {
                        if (ScalarMethod3 == null)
                        {
                            if (replacement == null)
                            {
                                ScalarMethod3 = new ScalarValue3MethodState(this);
                            }
                            else
                            {
                                ScalarMethod3 = (ScalarValue3MethodState)replacement;
                            }
                        }
                    }

                    instance = ScalarMethod3;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ArrayMethod1:
                {
                    if (createOrReplace)
                    {
                        if (ArrayMethod1 == null)
                        {
                            if (replacement == null)
                            {
                                ArrayMethod1 = new ArrayValue1MethodState(this);
                            }
                            else
                            {
                                ArrayMethod1 = (ArrayValue1MethodState)replacement;
                            }
                        }
                    }

                    instance = ArrayMethod1;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ArrayMethod2:
                {
                    if (createOrReplace)
                    {
                        if (ArrayMethod2 == null)
                        {
                            if (replacement == null)
                            {
                                ArrayMethod2 = new ArrayValue2MethodState(this);
                            }
                            else
                            {
                                ArrayMethod2 = (ArrayValue2MethodState)replacement;
                            }
                        }
                    }

                    instance = ArrayMethod2;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.ArrayMethod3:
                {
                    if (createOrReplace)
                    {
                        if (ArrayMethod3 == null)
                        {
                            if (replacement == null)
                            {
                                ArrayMethod3 = new ArrayValue3MethodState(this);
                            }
                            else
                            {
                                ArrayMethod3 = (ArrayValue3MethodState)replacement;
                            }
                        }
                    }

                    instance = ArrayMethod3;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UserScalarMethod1:
                {
                    if (createOrReplace)
                    {
                        if (UserScalarMethod1 == null)
                        {
                            if (replacement == null)
                            {
                                UserScalarMethod1 = new UserScalarValue1MethodState(this);
                            }
                            else
                            {
                                UserScalarMethod1 = (UserScalarValue1MethodState)replacement;
                            }
                        }
                    }

                    instance = UserScalarMethod1;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UserScalarMethod2:
                {
                    if (createOrReplace)
                    {
                        if (UserScalarMethod2 == null)
                        {
                            if (replacement == null)
                            {
                                UserScalarMethod2 = new UserScalarValue2MethodState(this);
                            }
                            else
                            {
                                UserScalarMethod2 = (UserScalarValue2MethodState)replacement;
                            }
                        }
                    }

                    instance = UserScalarMethod2;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UserArrayMethod1:
                {
                    if (createOrReplace)
                    {
                        if (UserArrayMethod1 == null)
                        {
                            if (replacement == null)
                            {
                                UserArrayMethod1 = new UserArrayValue1MethodState(this);
                            }
                            else
                            {
                                UserArrayMethod1 = (UserArrayValue1MethodState)replacement;
                            }
                        }
                    }

                    instance = UserArrayMethod1;
                    break;
                }

                case SampleCompany.NodeManagers.TestData.BrowseNames.UserArrayMethod2:
                {
                    if (createOrReplace)
                    {
                        if (UserArrayMethod2 == null)
                        {
                            if (replacement == null)
                            {
                                UserArrayMethod2 = new UserArrayValue2MethodState(this);
                            }
                            else
                            {
                                UserArrayMethod2 = (UserArrayValue2MethodState)replacement;
                            }
                        }
                    }

                    instance = UserArrayMethod2;
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
        private ScalarValue1MethodState m_scalarMethod1Method;
        private ScalarValue2MethodState m_scalarMethod2Method;
        private ScalarValue3MethodState m_scalarMethod3Method;
        private ArrayValue1MethodState m_arrayMethod1Method;
        private ArrayValue2MethodState m_arrayMethod2Method;
        private ArrayValue3MethodState m_arrayMethod3Method;
        private UserScalarValue1MethodState m_userScalarMethod1Method;
        private UserScalarValue2MethodState m_userScalarMethod2Method;
        private UserArrayValue1MethodState m_userArrayMethod1Method;
        private UserArrayValue2MethodState m_userArrayMethod2Method;
        #endregion
    }
    #endif
    #endregion

    #region TestSystemConditionState Class
    #if (!OPCUA_EXCLUDE_TestSystemConditionState)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class TestSystemConditionState : ConditionState
    {
        #region Constructors
        /// <remarks />
        public TestSystemConditionState(NodeState parent) : base(parent)
        {
        }

        /// <remarks />
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.NodeManagers.TestData.ObjectTypes.TestSystemConditionType, SampleCompany.NodeManagers.TestData.Namespaces.TestData, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <remarks />
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <remarks />
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <remarks />
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAADsAAABodHRwOi8vc2FtcGxlY29tcGFueS5jb20vU2FtcGxlU2VydmVyL05vZGVNYW5hZ2Vycy9U" +
           "ZXN0RGF0Yf////8EYIACAQAAAAEAHwAAAFRlc3RTeXN0ZW1Db25kaXRpb25UeXBlSW5zdGFuY2UBAaQD" +
           "AQGkA6QDAAD/////FgAAABVgiQoCAAAAAAAHAAAARXZlbnRJZAEBpQMALgBEpQMAAAAP/////wEB////" +
           "/wAAAAAVYIkKAgAAAAAACQAAAEV2ZW50VHlwZQEBpgMALgBEpgMAAAAR/////wEB/////wAAAAAVYIkK" +
           "AgAAAAAACgAAAFNvdXJjZU5vZGUBAacDAC4ARKcDAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoA" +
           "AABTb3VyY2VOYW1lAQGoAwAuAESoAwAAAAz/////AQH/////AAAAABVgiQoCAAAAAAAEAAAAVGltZQEB" +
           "qQMALgBEqQMAAAEAJgH/////AQH/////AAAAABVgiQoCAAAAAAALAAAAUmVjZWl2ZVRpbWUBAaoDAC4A" +
           "RKoDAAABACYB/////wEB/////wAAAAAVYIkKAgAAAAAABwAAAE1lc3NhZ2UBAawDAC4ARKwDAAAAFf//" +
           "//8BAf////8AAAAAFWCJCgIAAAAAAAgAAABTZXZlcml0eQEBrQMALgBErQMAAAAF/////wEB/////wAA" +
           "AAAVYIkKAgAAAAAAEAAAAENvbmRpdGlvbkNsYXNzSWQBAa4DAC4ARK4DAAAAEf////8BAf////8AAAAA" +
           "FWCJCgIAAAAAABIAAABDb25kaXRpb25DbGFzc05hbWUBAa8DAC4ARK8DAAAAFf////8BAf////8AAAAA" +
           "FWCJCgIAAAAAAA0AAABDb25kaXRpb25OYW1lAQGyAwAuAESyAwAAAAz/////AQH/////AAAAABVgiQoC" +
           "AAAAAAAIAAAAQnJhbmNoSWQBAbMDAC4ARLMDAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAYAAABS" +
           "ZXRhaW4BAbQDAC4ARLQDAAAAAf////8BAf////8AAAAAFWCJCgIAAAAAAAwAAABFbmFibGVkU3RhdGUB" +
           "AbUDAC8BACMjtQMAAAAV/////wEB/////wEAAAAVYIkKAgAAAAAAAgAAAElkAQG2AwAuAES2AwAAAAH/" +
           "////AQH/////AAAAABVgiQoCAAAAAAAHAAAAUXVhbGl0eQEBvgMALwEAKiO+AwAAABP/////AQH/////" +
           "AQAAABVgiQoCAAAAAAAPAAAAU291cmNlVGltZXN0YW1wAQG/AwAuAES/AwAAAQAmAf////8BAf////8A" +
           "AAAAFWCJCgIAAAAAAAwAAABMYXN0U2V2ZXJpdHkBAcADAC8BACojwAMAAAAF/////wEB/////wEAAAAV" +
           "YIkKAgAAAAAADwAAAFNvdXJjZVRpbWVzdGFtcAEBwQMALgBEwQMAAAEAJgH/////AQH/////AAAAABVg" +
           "iQoCAAAAAAAHAAAAQ29tbWVudAEBwgMALwEAKiPCAwAAABX/////AQH/////AQAAABVgiQoCAAAAAAAP" +
           "AAAAU291cmNlVGltZXN0YW1wAQHDAwAuAETDAwAAAQAmAf////8BAf////8AAAAAFWCJCgIAAAAAAAwA" +
           "AABDbGllbnRVc2VySWQBAcQDAC4ARMQDAAAADP////8BAf////8AAAAABGGCCgQAAAAAAAcAAABEaXNh" +
           "YmxlAQHFAwAvAQBEI8UDAAABAQEAAAABAPkLAAEA8woAAAAABGGCCgQAAAAAAAYAAABFbmFibGUBAcYD" +
           "AC8BAEMjxgMAAAEBAQAAAAEA+QsAAQDzCgAAAAAEYYIKBAAAAAAACgAAAEFkZENvbW1lbnQBAccDAC8B" +
           "AEUjxwMAAAEBAQAAAAEA+QsAAQANCwEAAAAXYKkKAgAAAAAADgAAAElucHV0QXJndW1lbnRzAQHIAwAu" +
           "AETIAwAAlgIAAAABACoBAUYAAAAHAAAARXZlbnRJZAAP/////wAAAAADAAAAACgAAABUaGUgaWRlbnRp" +
           "ZmllciBmb3IgdGhlIGV2ZW50IHRvIGNvbW1lbnQuAQAqAQFCAAAABwAAAENvbW1lbnQAFf////8AAAAA" +
           "AwAAAAAkAAAAVGhlIGNvbW1lbnQgdG8gYWRkIHRvIHRoZSBjb25kaXRpb24uAQAoAQEAAAABAAAAAAAA" +
           "AAEB/////wAAAAAVYIkKAgAAAAEAEgAAAE1vbml0b3JlZE5vZGVDb3VudAEBzQMALgBEzQMAAAAG////" +
           "/wEB/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<int> MonitoredNodeCount
        {
            get
            {
                return m_monitoredNodeCount;
            }

            set
            {
                if (!Object.ReferenceEquals(m_monitoredNodeCount, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_monitoredNodeCount = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <remarks />
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_monitoredNodeCount != null)
            {
                children.Add(m_monitoredNodeCount);
            }

            base.GetChildren(context, children);
        }
            
        /// <remarks />
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
                case SampleCompany.NodeManagers.TestData.BrowseNames.MonitoredNodeCount:
                {
                    if (createOrReplace)
                    {
                        if (MonitoredNodeCount == null)
                        {
                            if (replacement == null)
                            {
                                MonitoredNodeCount = new PropertyState<int>(this);
                            }
                            else
                            {
                                MonitoredNodeCount = (PropertyState<int>)replacement;
                            }
                        }
                    }

                    instance = MonitoredNodeCount;
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
        private PropertyState<int> m_monitoredNodeCount;
        #endregion
    }
    #endif
    #endregion
}