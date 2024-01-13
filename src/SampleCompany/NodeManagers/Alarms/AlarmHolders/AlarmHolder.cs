#region Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Opc.Ua;
#endregion

#pragma warning disable CS1591

namespace SampleCompany.NodeManagers.Alarms
{
    public class AlarmHolder
    {
        public AlarmHolder(AlarmNodeManager alarmNodeManager, FolderState parent, SourceController trigger, Type controllerType, int interval)
        {
            alarmNodeManager_ = alarmNodeManager;
            parent_ = parent;
            trigger_ = trigger.Source;
            alarmController_ = trigger.Controller;
            alarmControllerType_ = trigger.Controller.GetType();
            interval_ = interval;
        }

        protected void Initialize(uint alarmTypeIdentifier, string name)
        {
            alarmTypeIdentifier_ = alarmTypeIdentifier;
            alarmTypeName_ = GetAlarmTypeName(alarmTypeIdentifier_);

            string extraName = "";
            if (name.Length > 0)
            {
                extraName = "." + name;
            }

            alarmRootName_ = alarmTypeName_ + extraName;
            mapName_ = (string)parent_.NodeId.Identifier + "." + alarmRootName_;

            InitializeInternal(alarm_);
        }

        public bool HasBranches()
        {
            bool hasBranches = false;

            ConditionState alarm = alarm_ as ConditionState;
            if (alarm != null)
            {
                hasBranches = alarm.GetBranchCount() > 0;
            }

            return hasBranches;
        }

        public BaseEventState GetBranch(byte[] eventId)
        {
            BaseEventState state = null;

            ConditionState alarm = alarm_ as ConditionState;
            if (alarm != null)
            {
                state = alarm.GetBranch(eventId);
            }

            return state;
        }

        public void ClearBranches()
        {
            ConditionState alarm = alarm_ as ConditionState;
            if (alarm != null)
            {
                alarm.ClearBranches();
                alarmController_.SetBranchCount(0);
            }
        }

        public void GetBranchesForConditionRefresh(List<IFilterTarget> events)
        {
            ConditionState alarm = alarm_ as ConditionState;
            if (alarm != null)
            {
                Dictionary<string, ConditionState> branches = alarm.GetBranches();
                foreach (BaseEventState branch in branches.Values)
                {
                    events.Add(branch);
                }
            }
        }


        protected virtual void CreateBranch()
        {
        }

        private void InitializeInternal(BaseEventState alarm, NodeId branchId = null)
        {
            string alarmName = AlarmName;
            string alarmNodeId = (string)parent_.NodeId.Identifier + "." + AlarmName;

            alarm.SymbolicName = alarmName;

            NodeId createNodeId = null;
            QualifiedName createQualifiedName = new QualifiedName(alarmName, NamespaceIndex);
            LocalizedText createLocalizedText = null;


            bool isBranch = IsBranch(branchId);
            createNodeId = new NodeId(alarmNodeId, NamespaceIndex);
            createLocalizedText = new LocalizedText(alarmName);

            alarm.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            alarm.Create(
                SystemContext,
                createNodeId,
                createQualifiedName,
                createLocalizedText,
                true);


            if (!isBranch)
            {
                trigger_.AddReference(ReferenceTypes.HasCondition, false, alarm_.NodeId);
                parent_.AddChild(alarm);
            }

        }

        private bool IsBranch(NodeId branchId)
        {
            bool isBranch = false;
            if (branchId != null && !branchId.IsNullNodeId)
            {
                isBranch = true;
            }
            return isBranch;
        }

        public NodeId GetNewBranchId()
        {
            return new NodeId(++branchCounter_, NamespaceIndex);
        }

        public virtual void Update(bool updated)
        {
            DelayedEvents();
            if (updated)
            {
                SetValue();
            }
        }

        public virtual void DelayedEvents()
        {
            // Method calls are done by the core.
            // Delayed events are expected events to be logged to file.
            while (delayedMessages_.Count > 0)
            {
                Utils.LogWarning("Delayed:{0} Event Time: {1}", delayedMessages_[0], alarm_.Time.Value);
                delayedMessages_.RemoveAt(0);
            }
        }

        protected void Log(string caller, string message, BaseEventState alarm = null)
        {
            LogMessage(LogLevel.Information, caller, message);
        }

        protected void LogError(string caller, string message, BaseEventState alarm = null)
        {
            LogMessage(LogLevel.Error, caller, message);
        }

        protected void LogMessage(LogLevel logLevel, string caller, string message)
        {
            Utils.Log(logLevel, "{0}: {1} EventId {2} {3}", caller, mapName_, Utils.ToHexString(alarm_.EventId.Value), message);
        }


        public virtual void SetValue(string message = "")
        {
            Utils.LogError("AlarmHolder.SetValue() - Should not be called");
        }

        public void Start(UInt32 seconds)
        {
            ClearBranches();
            alarmController_.Start(seconds);
        }

        public void Stop()
        {
            ClearBranches();
            alarmController_.Stop();
        }

        protected virtual bool UpdateShelving()
        {
            Utils.LogError("AlarmHolder.UpdateShelving() - Should not be called");
            return false;
        }

        protected virtual bool UpdateSuppression()
        {
            Utils.LogError("AlarmHolder.UpdateSuppression() - Should not be called");
            return false;
        }

        protected virtual void SetActive(BaseEventState state, bool activeState)
        {

        }

        #region Methods
        public ServiceResult OnWriteAlarmTrigger(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            if (Trigger.Value != value)
            {
                Trigger.Value = value;
                SetValue("Manual Write to trigger " + value.ToString());
            }

            return Opc.Ua.StatusCodes.Good;
        }
        #endregion

        #region Properties

        public ISystemContext SystemContext
        {
            get { return GetNodeManager().SystemContext; }
        }

        public ushort NamespaceIndex
        {
            get { return GetNodeManager().NamespaceIndex; }
        }

        public BaseEventState Alarm
        {
            get { return alarm_; }
        }

        public AlarmController Controller
        {
            get { return alarmController_; }
        }

        public BaseDataVariableState Trigger
        {
            get { return trigger_; }
        }

        public string MapName
        {
            get { return mapName_; }
        }

        public string TriggerName
        {
            get { return alarmRootName_ + AlarmConstants.TriggerExtension; }
        }

        public string AlarmName
        {
            get { return alarmRootName_ + AlarmConstants.AlarmExtension; }
        }

        public string AlarmNodeName
        {
            get { return alarm_.NodeId.ToString(); }
        }

        public bool Analog
        {
            get { return analog_; }
        }
        public bool Optional
        {
            get { return optional_; }
        }

        public bool SupportsBranching
        {
            get { return supportsBranching_; }
        }

        public virtual void SetBranching(bool value)
        {

        }





        #endregion

        #region Helpers
        public PropertyState<NodeId> GetEventType()
        {
            return alarm_.EventType;
        }

        protected AlarmNodeManager GetNodeManager()
        {
            return alarmNodeManager_;
        }

        protected string GetAlarmTypeName(UInt32 alarmTypeIdentifier)
        {
            string alarmTypeName = "";

            switch (alarmTypeIdentifier)
            {
                case Opc.Ua.ObjectTypes.ConditionType:
                    alarmTypeName = "ConditionType";
                    break;

                case Opc.Ua.ObjectTypes.DialogConditionType:
                    alarmTypeName = "DialogConditionType";
                    break;

                case Opc.Ua.ObjectTypes.AcknowledgeableConditionType:
                    alarmTypeName = "AcknowledgeableConditionType";
                    break;

                case Opc.Ua.ObjectTypes.AlarmConditionType:
                    alarmTypeName = "AlarmConditionType";
                    break;

                case Opc.Ua.ObjectTypes.AlarmGroupType:
                    alarmTypeName = "AlarmGroupType";
                    break;

                case Opc.Ua.ObjectTypes.ShelvedStateMachineType:
                    alarmTypeName = "ShelvedStateMachineType";
                    break;

                case Opc.Ua.ObjectTypes.LimitAlarmType:
                    alarmTypeName = "LimitAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.ExclusiveLimitStateMachineType:
                    alarmTypeName = "ExclusiveLimitStateMachineType";
                    break;

                case Opc.Ua.ObjectTypes.ExclusiveLimitAlarmType:
                    alarmTypeName = "ExclusiveLimitAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.NonExclusiveLimitAlarmType:
                    alarmTypeName = "NonExclusiveLimitAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.NonExclusiveLevelAlarmType:
                    alarmTypeName = "NonExclusiveLevelAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.ExclusiveLevelAlarmType:
                    alarmTypeName = "ExclusiveLevelAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.NonExclusiveDeviationAlarmType:
                    alarmTypeName = "NonExclusiveDeviationAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.NonExclusiveRateOfChangeAlarmType:
                    alarmTypeName = "NonExclusiveRateOfChangeAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.ExclusiveDeviationAlarmType:
                    alarmTypeName = "ExclusiveDeviationAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.ExclusiveRateOfChangeAlarmType:
                    alarmTypeName = "ExclusiveRateOfChangeAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.DiscreteAlarmType:
                    alarmTypeName = "DiscreteAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.OffNormalAlarmType:
                    alarmTypeName = "OffNormalAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.SystemOffNormalAlarmType:
                    alarmTypeName = "SystemOffNormalAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.TripAlarmType:
                    alarmTypeName = "TripAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.InstrumentDiagnosticAlarmType:
                    alarmTypeName = "InstrumentDiagnosticAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.SystemDiagnosticAlarmType:
                    alarmTypeName = "SystemDiagnosticAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.CertificateExpirationAlarmType:
                    alarmTypeName = "CertificateExpirationAlarmType";
                    break;

                case Opc.Ua.ObjectTypes.DiscrepancyAlarmType:
                    alarmTypeName = "DiscrepancyAlarmType";
                    break;

                default:
                    break;
            }

            return alarmTypeName;
        }

        /// <summary>
        /// Function is to modify the namespace if this is a derived type.
        /// If no derived types, it's 0
        /// </summary>
        /// <param name="alarmTypeIdentifier"></param>
        /// <returns>ushort namespaceindex</returns>
        protected ushort GetNameSpaceIndex(UInt32 alarmTypeIdentifier)
        {
            ushort nameSpaceIndex = 0;

            return nameSpaceIndex;
        }
        #endregion

        #region Protected Fields
        protected AlarmNodeManager alarmNodeManager_ = null;
        protected BaseEventState alarm_ = null;
        protected Type alarmControllerType_ = null;
        protected AlarmController alarmController_ = null;
        protected BaseDataVariableState trigger_ = null;
        protected string alarmRootName_ = "";
        protected string mapName_ = "";
        protected bool analog_ = true;
        protected bool optional_ = false;
        protected int interval_ = 0;
        protected uint branchCounter_ = 0;
        protected bool supportsBranching_ = false;
        protected FolderState parent_ = null;
        protected uint alarmTypeIdentifier_ = 0;
        protected string alarmTypeName_ = "";
        protected SupportedAlarmConditionType alarmConditionType_ = null;
        protected List<string> delayedMessages_ = new List<string>();
        #endregion


    }
}
