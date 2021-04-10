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

namespace SampleCompany.SampleServer.Model
{
    #region SystemCycleStatusEventState Class
    #if (!OPCUA_EXCLUDE_SystemCycleStatusEventState)
    /// <summary>
    /// Stores an instance of the SystemCycleStatusEventType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class SystemCycleStatusEventState : SystemEventState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public SystemCycleStatusEventState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleStatusEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
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
           "AAAAAQAiAAAAU3lzdGVtQ3ljbGVTdGF0dXNFdmVudFR5cGVJbnN0YW5jZQEBAgABAQIAAgAAAP////8K" +
           "AAAAFWCJCgIAAAAAAAcAAABFdmVudElkAQEDAAAuAEQDAAAAAA//////AQH/////AAAAABVgiQoCAAAA" +
           "AAAJAAAARXZlbnRUeXBlAQEEAAAuAEQEAAAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291" +
           "cmNlTm9kZQEBBQAALgBEBQAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNvdXJjZU5hbWUB" +
           "AQYAAC4ARAYAAAAADP////8BAf////8AAAAAFWCJCgIAAAAAAAQAAABUaW1lAQEHAAAuAEQHAAAAAQAm" +
           "Af////8BAf////8AAAAAFWCJCgIAAAAAAAsAAABSZWNlaXZlVGltZQEBCAAALgBECAAAAAEAJgH/////" +
           "AQH/////AAAAABVgiQoCAAAAAAAHAAAATWVzc2FnZQEBCgAALgBECgAAAAAV/////wEB/////wAAAAAV" +
           "YIkKAgAAAAAACAAAAFNldmVyaXR5AQELAAAuAEQLAAAAAAX/////AQH/////AAAAABVgiQoCAAAAAQAH" +
           "AAAAQ3ljbGVJZAEBDAAALgBEDAAAAAAM/////wEB/////wAAAAAVYIkKAgAAAAEACwAAAEN1cnJlbnRT" +
           "dGVwAQENAAAuAEQNAAAAAQEBAP////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<string> CycleId
        {
            get
            {
                return m_cycleId;
            }

            set
            {
                if (!Object.ReferenceEquals(m_cycleId, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_cycleId = value;
            }
        }

        /// <remarks />
        public PropertyState<CycleStepDataType> CurrentStep
        {
            get
            {
                return m_currentStep;
            }

            set
            {
                if (!Object.ReferenceEquals(m_currentStep, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_currentStep = value;
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
            if (m_cycleId != null)
            {
                children.Add(m_cycleId);
            }

            if (m_currentStep != null)
            {
                children.Add(m_currentStep);
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
                case SampleCompany.SampleServer.Model.BrowseNames.CycleId:
                {
                    if (createOrReplace)
                    {
                        if (CycleId == null)
                        {
                            if (replacement == null)
                            {
                                CycleId = new PropertyState<string>(this);
                            }
                            else
                            {
                                CycleId = (PropertyState<string>)replacement;
                            }
                        }
                    }

                    instance = CycleId;
                    break;
                }

                case SampleCompany.SampleServer.Model.BrowseNames.CurrentStep:
                {
                    if (createOrReplace)
                    {
                        if (CurrentStep == null)
                        {
                            if (replacement == null)
                            {
                                CurrentStep = new PropertyState<CycleStepDataType>(this);
                            }
                            else
                            {
                                CurrentStep = (PropertyState<CycleStepDataType>)replacement;
                            }
                        }
                    }

                    instance = CurrentStep;
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
        private PropertyState<string> m_cycleId;
        private PropertyState<CycleStepDataType> m_currentStep;
        #endregion
    }
    #endif
    #endregion

    #region SystemCycleStartedEventState Class
    #if (!OPCUA_EXCLUDE_SystemCycleStartedEventState)
    /// <summary>
    /// Stores an instance of the SystemCycleStartedEventType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class SystemCycleStartedEventState : SystemCycleStatusEventState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public SystemCycleStartedEventState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleStartedEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
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
           "AAAAAQAjAAAAU3lzdGVtQ3ljbGVTdGFydGVkRXZlbnRUeXBlSW5zdGFuY2UBAQ4AAQEOAA4AAAD/////" +
           "CwAAABVgiQoCAAAAAAAHAAAARXZlbnRJZAEBDwAALgBEDwAAAAAP/////wEB/////wAAAAAVYIkKAgAA" +
           "AAAACQAAAEV2ZW50VHlwZQEBEAAALgBEEAAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNv" +
           "dXJjZU5vZGUBAREAAC4ARBEAAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoAAABTb3VyY2VOYW1l" +
           "AQESAAAuAEQSAAAAAAz/////AQH/////AAAAABVgiQoCAAAAAAAEAAAAVGltZQEBEwAALgBEEwAAAAEA" +
           "JgH/////AQH/////AAAAABVgiQoCAAAAAAALAAAAUmVjZWl2ZVRpbWUBARQAAC4ARBQAAAABACYB////" +
           "/wEB/////wAAAAAVYIkKAgAAAAAABwAAAE1lc3NhZ2UBARYAAC4ARBYAAAAAFf////8BAf////8AAAAA" +
           "FWCJCgIAAAAAAAgAAABTZXZlcml0eQEBFwAALgBEFwAAAAAF/////wEB/////wAAAAAVYIkKAgAAAAEA" +
           "BwAAAEN5Y2xlSWQBARgAAC4ARBgAAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAsAAABDdXJyZW50" +
           "U3RlcAEBGQAALgBEGQAAAAEBAQD/////AQH/////AAAAABdgiQoCAAAAAQAFAAAAU3RlcHMBARoAAC4A" +
           "RBoAAAABAQEAAQAAAAEAAAAAAAAAAQH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<CycleStepDataType[]> Steps
        {
            get
            {
                return m_steps;
            }

            set
            {
                if (!Object.ReferenceEquals(m_steps, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_steps = value;
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
            if (m_steps != null)
            {
                children.Add(m_steps);
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
                case SampleCompany.SampleServer.Model.BrowseNames.Steps:
                {
                    if (createOrReplace)
                    {
                        if (Steps == null)
                        {
                            if (replacement == null)
                            {
                                Steps = new PropertyState<CycleStepDataType[]>(this);
                            }
                            else
                            {
                                Steps = (PropertyState<CycleStepDataType[]>)replacement;
                            }
                        }
                    }

                    instance = Steps;
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
        private PropertyState<CycleStepDataType[]> m_steps;
        #endregion
    }
    #endif
    #endregion

    #region SystemCycleAbortedEventState Class
    #if (!OPCUA_EXCLUDE_SystemCycleAbortedEventState)
    /// <summary>
    /// Stores an instance of the SystemCycleAbortedEventType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class SystemCycleAbortedEventState : SystemCycleStatusEventState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public SystemCycleAbortedEventState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleAbortedEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
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
           "AAAAAQAjAAAAU3lzdGVtQ3ljbGVBYm9ydGVkRXZlbnRUeXBlSW5zdGFuY2UBARsAAQEbABsAAAD/////" +
           "CwAAABVgiQoCAAAAAAAHAAAARXZlbnRJZAEBHAAALgBEHAAAAAAP/////wEB/////wAAAAAVYIkKAgAA" +
           "AAAACQAAAEV2ZW50VHlwZQEBHQAALgBEHQAAAAAR/////wEB/////wAAAAAVYIkKAgAAAAAACgAAAFNv" +
           "dXJjZU5vZGUBAR4AAC4ARB4AAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoAAABTb3VyY2VOYW1l" +
           "AQEfAAAuAEQfAAAAAAz/////AQH/////AAAAABVgiQoCAAAAAAAEAAAAVGltZQEBIAAALgBEIAAAAAEA" +
           "JgH/////AQH/////AAAAABVgiQoCAAAAAAALAAAAUmVjZWl2ZVRpbWUBASEAAC4ARCEAAAABACYB////" +
           "/wEB/////wAAAAAVYIkKAgAAAAAABwAAAE1lc3NhZ2UBASMAAC4ARCMAAAAAFf////8BAf////8AAAAA" +
           "FWCJCgIAAAAAAAgAAABTZXZlcml0eQEBJAAALgBEJAAAAAAF/////wEB/////wAAAAAVYIkKAgAAAAEA" +
           "BwAAAEN5Y2xlSWQBASUAAC4ARCUAAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAsAAABDdXJyZW50" +
           "U3RlcAEBJgAALgBEJgAAAAEBAQD/////AQH/////AAAAABVgiQoCAAAAAQAFAAAARXJyb3IBAScAAC4A" +
           "RCcAAAAAE/////8BAf////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <remarks />
        public PropertyState<StatusCode> Error
        {
            get
            {
                return m_error;
            }

            set
            {
                if (!Object.ReferenceEquals(m_error, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_error = value;
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
            if (m_error != null)
            {
                children.Add(m_error);
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
                case SampleCompany.SampleServer.Model.BrowseNames.Error:
                {
                    if (createOrReplace)
                    {
                        if (Error == null)
                        {
                            if (replacement == null)
                            {
                                Error = new PropertyState<StatusCode>(this);
                            }
                            else
                            {
                                Error = (PropertyState<StatusCode>)replacement;
                            }
                        }
                    }

                    instance = Error;
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
        private PropertyState<StatusCode> m_error;
        #endregion
    }
    #endif
    #endregion

    #region SystemCycleFinishedEventState Class
    #if (!OPCUA_EXCLUDE_SystemCycleFinishedEventState)
    /// <summary>
    /// Stores an instance of the SystemCycleFinishedEventType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class SystemCycleFinishedEventState : SystemCycleStatusEventState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public SystemCycleFinishedEventState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(SampleCompany.SampleServer.Model.ObjectTypes.SystemCycleFinishedEventType, SampleCompany.SampleServer.Model.Namespaces.SampleServer, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            base.Initialize(context);
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
           "AAAAAQAkAAAAU3lzdGVtQ3ljbGVGaW5pc2hlZEV2ZW50VHlwZUluc3RhbmNlAQEoAAEBKAAoAAAA////" +
           "/woAAAAVYIkKAgAAAAAABwAAAEV2ZW50SWQBASkAAC4ARCkAAAAAD/////8BAf////8AAAAAFWCJCgIA" +
           "AAAAAAkAAABFdmVudFR5cGUBASoAAC4ARCoAAAAAEf////8BAf////8AAAAAFWCJCgIAAAAAAAoAAABT" +
           "b3VyY2VOb2RlAQErAAAuAEQrAAAAABH/////AQH/////AAAAABVgiQoCAAAAAAAKAAAAU291cmNlTmFt" +
           "ZQEBLAAALgBELAAAAAAM/////wEB/////wAAAAAVYIkKAgAAAAAABAAAAFRpbWUBAS0AAC4ARC0AAAAB" +
           "ACYB/////wEB/////wAAAAAVYIkKAgAAAAAACwAAAFJlY2VpdmVUaW1lAQEuAAAuAEQuAAAAAQAmAf//" +
           "//8BAf////8AAAAAFWCJCgIAAAAAAAcAAABNZXNzYWdlAQEwAAAuAEQwAAAAABX/////AQH/////AAAA" +
           "ABVgiQoCAAAAAAAIAAAAU2V2ZXJpdHkBATEAAC4ARDEAAAAABf////8BAf////8AAAAAFWCJCgIAAAAB" +
           "AAcAAABDeWNsZUlkAQEyAAAuAEQyAAAAAAz/////AQH/////AAAAABVgiQoCAAAAAQALAAAAQ3VycmVu" +
           "dFN0ZXABATMAAC4ARDMAAAABAQEA/////wEB/////wAAAAA=";
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
}