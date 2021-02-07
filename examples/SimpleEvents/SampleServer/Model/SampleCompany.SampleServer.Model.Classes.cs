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
           "AAAANWCJCgIAAAAAAAcAAABFdmVudElkAQEDAAMAAAAAKwAAAEEgZ2xvYmFsbHkgdW5pcXVlIGlkZW50" +
           "aWZpZXIgZm9yIHRoZSBldmVudC4ALgBEAwAAAAAP/////wEB/////wAAAAA1YIkKAgAAAAAACQAAAEV2" +
           "ZW50VHlwZQEBBAADAAAAACIAAABUaGUgaWRlbnRpZmllciBmb3IgdGhlIGV2ZW50IHR5cGUuAC4ARAQA" +
           "AAAAEf////8BAf////8AAAAANWCJCgIAAAAAAAoAAABTb3VyY2VOb2RlAQEFAAMAAAAAGAAAAFRoZSBz" +
           "b3VyY2Ugb2YgdGhlIGV2ZW50LgAuAEQFAAAAABH/////AQH/////AAAAADVgiQoCAAAAAAAKAAAAU291" +
           "cmNlTmFtZQEBBgADAAAAACkAAABBIGRlc2NyaXB0aW9uIG9mIHRoZSBzb3VyY2Ugb2YgdGhlIGV2ZW50" +
           "LgAuAEQGAAAAAAz/////AQH/////AAAAADVgiQoCAAAAAAAEAAAAVGltZQEBBwADAAAAABgAAABXaGVu" +
           "IHRoZSBldmVudCBvY2N1cnJlZC4ALgBEBwAAAAEAJgH/////AQH/////AAAAADVgiQoCAAAAAAALAAAA" +
           "UmVjZWl2ZVRpbWUBAQgAAwAAAAA+AAAAV2hlbiB0aGUgc2VydmVyIHJlY2VpdmVkIHRoZSBldmVudCBm" +
           "cm9tIHRoZSB1bmRlcmx5aW5nIHN5c3RlbS4ALgBECAAAAAEAJgH/////AQH/////AAAAADVgiQoCAAAA" +
           "AAAHAAAATWVzc2FnZQEBCgADAAAAACUAAABBIGxvY2FsaXplZCBkZXNjcmlwdGlvbiBvZiB0aGUgZXZl" +
           "bnQuAC4ARAoAAAAAFf////8BAf////8AAAAANWCJCgIAAAAAAAgAAABTZXZlcml0eQEBCwADAAAAACEA" +
           "AABJbmRpY2F0ZXMgaG93IHVyZ2VudCBhbiBldmVudCBpcy4ALgBECwAAAAAF/////wEB/////wAAAAAV" +
           "YIkKAgAAAAEABwAAAEN5Y2xlSWQBAQwAAC4ARAwAAAAADP////8BAf////8AAAAAFWCJCgIAAAABAAsA" +
           "AABDdXJyZW50U3RlcAEBDQAALgBEDQAAAAEBAQD/////AQH/////AAAAAA==";
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
           "CwAAADVgiQoCAAAAAAAHAAAARXZlbnRJZAEBDwADAAAAACsAAABBIGdsb2JhbGx5IHVuaXF1ZSBpZGVu" +
           "dGlmaWVyIGZvciB0aGUgZXZlbnQuAC4ARA8AAAAAD/////8BAf////8AAAAANWCJCgIAAAAAAAkAAABF" +
           "dmVudFR5cGUBARAAAwAAAAAiAAAAVGhlIGlkZW50aWZpZXIgZm9yIHRoZSBldmVudCB0eXBlLgAuAEQQ" +
           "AAAAABH/////AQH/////AAAAADVgiQoCAAAAAAAKAAAAU291cmNlTm9kZQEBEQADAAAAABgAAABUaGUg" +
           "c291cmNlIG9mIHRoZSBldmVudC4ALgBEEQAAAAAR/////wEB/////wAAAAA1YIkKAgAAAAAACgAAAFNv" +
           "dXJjZU5hbWUBARIAAwAAAAApAAAAQSBkZXNjcmlwdGlvbiBvZiB0aGUgc291cmNlIG9mIHRoZSBldmVu" +
           "dC4ALgBEEgAAAAAM/////wEB/////wAAAAA1YIkKAgAAAAAABAAAAFRpbWUBARMAAwAAAAAYAAAAV2hl" +
           "biB0aGUgZXZlbnQgb2NjdXJyZWQuAC4ARBMAAAABACYB/////wEB/////wAAAAA1YIkKAgAAAAAACwAA" +
           "AFJlY2VpdmVUaW1lAQEUAAMAAAAAPgAAAFdoZW4gdGhlIHNlcnZlciByZWNlaXZlZCB0aGUgZXZlbnQg" +
           "ZnJvbSB0aGUgdW5kZXJseWluZyBzeXN0ZW0uAC4ARBQAAAABACYB/////wEB/////wAAAAA1YIkKAgAA" +
           "AAAABwAAAE1lc3NhZ2UBARYAAwAAAAAlAAAAQSBsb2NhbGl6ZWQgZGVzY3JpcHRpb24gb2YgdGhlIGV2" +
           "ZW50LgAuAEQWAAAAABX/////AQH/////AAAAADVgiQoCAAAAAAAIAAAAU2V2ZXJpdHkBARcAAwAAAAAh" +
           "AAAASW5kaWNhdGVzIGhvdyB1cmdlbnQgYW4gZXZlbnQgaXMuAC4ARBcAAAAABf////8BAf////8AAAAA" +
           "FWCJCgIAAAABAAcAAABDeWNsZUlkAQEYAAAuAEQYAAAAAAz/////AQH/////AAAAABVgiQoCAAAAAQAL" +
           "AAAAQ3VycmVudFN0ZXABARkAAC4ARBkAAAABAQEA/////wEB/////wAAAAAXYIkKAgAAAAEABQAAAFN0" +
           "ZXBzAQEaAAAuAEQaAAAAAQEBAAEAAAABAAAAAAAAAAEB/////wAAAAA=";
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
           "CwAAADVgiQoCAAAAAAAHAAAARXZlbnRJZAEBHAADAAAAACsAAABBIGdsb2JhbGx5IHVuaXF1ZSBpZGVu" +
           "dGlmaWVyIGZvciB0aGUgZXZlbnQuAC4ARBwAAAAAD/////8BAf////8AAAAANWCJCgIAAAAAAAkAAABF" +
           "dmVudFR5cGUBAR0AAwAAAAAiAAAAVGhlIGlkZW50aWZpZXIgZm9yIHRoZSBldmVudCB0eXBlLgAuAEQd" +
           "AAAAABH/////AQH/////AAAAADVgiQoCAAAAAAAKAAAAU291cmNlTm9kZQEBHgADAAAAABgAAABUaGUg" +
           "c291cmNlIG9mIHRoZSBldmVudC4ALgBEHgAAAAAR/////wEB/////wAAAAA1YIkKAgAAAAAACgAAAFNv" +
           "dXJjZU5hbWUBAR8AAwAAAAApAAAAQSBkZXNjcmlwdGlvbiBvZiB0aGUgc291cmNlIG9mIHRoZSBldmVu" +
           "dC4ALgBEHwAAAAAM/////wEB/////wAAAAA1YIkKAgAAAAAABAAAAFRpbWUBASAAAwAAAAAYAAAAV2hl" +
           "biB0aGUgZXZlbnQgb2NjdXJyZWQuAC4ARCAAAAABACYB/////wEB/////wAAAAA1YIkKAgAAAAAACwAA" +
           "AFJlY2VpdmVUaW1lAQEhAAMAAAAAPgAAAFdoZW4gdGhlIHNlcnZlciByZWNlaXZlZCB0aGUgZXZlbnQg" +
           "ZnJvbSB0aGUgdW5kZXJseWluZyBzeXN0ZW0uAC4ARCEAAAABACYB/////wEB/////wAAAAA1YIkKAgAA" +
           "AAAABwAAAE1lc3NhZ2UBASMAAwAAAAAlAAAAQSBsb2NhbGl6ZWQgZGVzY3JpcHRpb24gb2YgdGhlIGV2" +
           "ZW50LgAuAEQjAAAAABX/////AQH/////AAAAADVgiQoCAAAAAAAIAAAAU2V2ZXJpdHkBASQAAwAAAAAh" +
           "AAAASW5kaWNhdGVzIGhvdyB1cmdlbnQgYW4gZXZlbnQgaXMuAC4ARCQAAAAABf////8BAf////8AAAAA" +
           "FWCJCgIAAAABAAcAAABDeWNsZUlkAQElAAAuAEQlAAAAAAz/////AQH/////AAAAABVgiQoCAAAAAQAL" +
           "AAAAQ3VycmVudFN0ZXABASYAAC4ARCYAAAABAQEA/////wEB/////wAAAAAVYIkKAgAAAAEABQAAAEVy" +
           "cm9yAQEnAAAuAEQnAAAAABP/////AQH/////AAAAAA==";
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
           "/woAAAA1YIkKAgAAAAAABwAAAEV2ZW50SWQBASkAAwAAAAArAAAAQSBnbG9iYWxseSB1bmlxdWUgaWRl" +
           "bnRpZmllciBmb3IgdGhlIGV2ZW50LgAuAEQpAAAAAA//////AQH/////AAAAADVgiQoCAAAAAAAJAAAA" +
           "RXZlbnRUeXBlAQEqAAMAAAAAIgAAAFRoZSBpZGVudGlmaWVyIGZvciB0aGUgZXZlbnQgdHlwZS4ALgBE" +
           "KgAAAAAR/////wEB/////wAAAAA1YIkKAgAAAAAACgAAAFNvdXJjZU5vZGUBASsAAwAAAAAYAAAAVGhl" +
           "IHNvdXJjZSBvZiB0aGUgZXZlbnQuAC4ARCsAAAAAEf////8BAf////8AAAAANWCJCgIAAAAAAAoAAABT" +
           "b3VyY2VOYW1lAQEsAAMAAAAAKQAAAEEgZGVzY3JpcHRpb24gb2YgdGhlIHNvdXJjZSBvZiB0aGUgZXZl" +
           "bnQuAC4ARCwAAAAADP////8BAf////8AAAAANWCJCgIAAAAAAAQAAABUaW1lAQEtAAMAAAAAGAAAAFdo" +
           "ZW4gdGhlIGV2ZW50IG9jY3VycmVkLgAuAEQtAAAAAQAmAf////8BAf////8AAAAANWCJCgIAAAAAAAsA" +
           "AABSZWNlaXZlVGltZQEBLgADAAAAAD4AAABXaGVuIHRoZSBzZXJ2ZXIgcmVjZWl2ZWQgdGhlIGV2ZW50" +
           "IGZyb20gdGhlIHVuZGVybHlpbmcgc3lzdGVtLgAuAEQuAAAAAQAmAf////8BAf////8AAAAANWCJCgIA" +
           "AAAAAAcAAABNZXNzYWdlAQEwAAMAAAAAJQAAAEEgbG9jYWxpemVkIGRlc2NyaXB0aW9uIG9mIHRoZSBl" +
           "dmVudC4ALgBEMAAAAAAV/////wEB/////wAAAAA1YIkKAgAAAAAACAAAAFNldmVyaXR5AQExAAMAAAAA" +
           "IQAAAEluZGljYXRlcyBob3cgdXJnZW50IGFuIGV2ZW50IGlzLgAuAEQxAAAAAAX/////AQH/////AAAA" +
           "ABVgiQoCAAAAAQAHAAAAQ3ljbGVJZAEBMgAALgBEMgAAAAAM/////wEB/////wAAAAAVYIkKAgAAAAEA" +
           "CwAAAEN1cnJlbnRTdGVwAQEzAAAuAEQzAAAAAQEBAP////8BAf////8AAAAA";
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