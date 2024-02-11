#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;
#endregion

#pragma warning disable CS1591


namespace SampleCompany.NodeManagers.Alarms
{
    // Ignore all Optionals for Confirm, as it should be supported.  For this object, that means remove all Optional conditions

    public class AcknowledgeableConditionTypeHolder : ConditionTypeHolder
    {
        public AcknowledgeableConditionTypeHolder(
            AlarmNodeManager alarmNodeManager,
            FolderState parent,
            SourceController trigger,
            string name,
            SupportedAlarmConditionType alarmConditionType,
            Type controllerType,
            int interval,
            bool optional = true,
            bool create = true) :
            base(alarmNodeManager, parent, trigger, name, alarmConditionType, controllerType, interval, optional)
        {
            if (create)
            {
                Initialize(ObjectTypes.AcknowledgeableConditionType, name);
            }
        }

        protected new void Initialize(
            uint alarmTypeIdentifier,
            string name)
        {
            // Create an alarm and trigger name - Create a base method for creating the trigger, just provide the name

            if (alarm_ == null)
            {
                alarm_ = new AcknowledgeableConditionState(parent_);
            }

            AcknowledgeableConditionState alarm = GetAlarm();
            InitializeInternal(alarm);

            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name);

            alarm.SetAcknowledgedState(SystemContext, acknowledged: true);

            alarm.SetConfirmedState(SystemContext, confirmed: true);
            alarm.OnConfirm = OnConfirm;

            alarm.Retain.Value = GetRetainState();
            alarm.AutoReportStateChanges = true;
        }

        private void InitializeInternal(AcknowledgeableConditionState alarm)
        {
            alarm.OnAcknowledge = OnAcknowledge;

            // Create any optional 
            if (alarm.ConfirmedState == null)
            {
                alarm.ConfirmedState = new TwoStateVariableState(alarm);
            }

            alarm.Confirm = new AddCommentMethodState(alarm);
        }

        #region Overrides 

        public override void SetValue(string message = "")
        {
            var requiresUpdate = false;

            if (ShouldEvent() || message.Length > 0)
            {
                requiresUpdate = true;
                AcknowledgeableConditionState alarm = GetAlarm();
                if (IsActive())
                {
                    Log("AcknowledgeableConditionTypeHolder", "Setting Acked State to false");
                    alarm.SetAcknowledgedState(SystemContext, acknowledged: false);
                    alarm.Retain.Value = true;

                }
                else
                {
                    alarm.Retain.Value = GetRetainState();
                }
            }

            if (requiresUpdate)
            {
                base.SetValue(message);
            }
        }

        protected override bool GetRetainState()
        {
            AcknowledgeableConditionState alarm = GetAlarm();

            var retainState = true;
            if (alarm.AckedState.Id.Value)
            {
                if (alarm.ConfirmedState.Id.Value)
                {
                    retainState = false;
                }
            }

            return retainState;
        }


        #endregion

        #region Helpers

        private AcknowledgeableConditionState GetAlarm(BaseEventState alarm = null)
        {
            if (alarm == null)
            {
                alarm = alarm_;
            }
            return (AcknowledgeableConditionState)alarm;
        }

        private AcknowledgeableConditionState GetAlarmOrBranch(byte[] eventId)
        {
            AcknowledgeableConditionState alarmOrBranch = null;

            AcknowledgeableConditionState alarm = GetAlarm();
            ConditionState alarmOrBranchConditionState = alarm.GetEventByEventId(eventId);
            if (alarmOrBranchConditionState != null)
            {
                alarmOrBranch = (AcknowledgeableConditionState)alarmOrBranchConditionState;
            }
            return alarmOrBranch;
        }




        #endregion

        #region Methods

        private ServiceResult OnAcknowledge(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            var eventIdString = Utils.ToHexString(eventId);

            if (acked_.Contains(eventIdString))
            {
                LogError("OnAcknowledge", EventErrorMessage(eventId) + " already acknowledged");
                return StatusCodes.BadConditionBranchAlreadyAcked;
            }

            AcknowledgeableConditionState alarm = GetAlarmOrBranch(eventId);

            if (alarm == null)
            {
                LogError("OnAcknowledge", EventErrorMessage(eventId));
                return StatusCodes.BadEventIdUnknown;
            }

            acked_.Add(eventIdString);

            if (alarm.AckedState.Id.Value)
            {
                return StatusCodes.BadConditionBranchAlreadyAcked;
            }

            // No Confirming on Acknowledge tests
            if (alarmNodeManager_.GetUnitFromNodeState(alarm) == "Acknowledge")
            {
                alarm.SetConfirmedState(SystemContext, confirmed: true);
                Log("OnAcknowledge", "Ignore Confirmed State, setting confirmed to true");
            }
            else
            {
                alarm.Message.Value = "User Acknowledged Event " + DateTime.Now.ToShortTimeString();
                Log("OnAcknowledge", "Setting Confirmed State to False");
                alarm.SetConfirmedState(SystemContext, confirmed: false);
            }

            alarmController_.OnAcknowledge();

            // TODO This will need to go away
            alarm.Retain.Value = GetRetainState();

            return ServiceResult.Good;
        }


        private ServiceResult OnConfirm(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {

            var eventIdString = Utils.ToHexString(eventId);

            Log("OnConfirm", "Called with eventId " + eventIdString + " Comment " + comment?.Text ?? "(empty)");

            if (confirmed_.Contains(eventIdString))
            {
                LogError("OnConfirm", EventErrorMessage(eventId) + " already confirmed");
                return StatusCodes.BadConditionBranchAlreadyConfirmed;
            }

            AcknowledgeableConditionState alarm = GetAlarmOrBranch(eventId);

            if (alarm == null)
            {
                LogError("OnConfirm", EventErrorMessage(eventId));

                return StatusCodes.BadEventIdUnknown;
            }

            confirmed_.Add(eventIdString);

            alarm.Message.Value = "User Confirmed Event " + DateTime.Now.ToShortTimeString();

            alarmController_.OnAcknowledge();

            // TODO Go Away?
            alarm.Retain.Value = GetRetainState();

            return ServiceResult.Good;
        }

        #endregion

        #region Protected Fields
        protected HashSet<string> acked_ = new HashSet<string>();
        protected HashSet<string> confirmed_ = new HashSet<string>();
        #endregion
    }
}
