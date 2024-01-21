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

using Opc.Ua;
#endregion

#pragma warning disable CS1591

namespace SampleCompany.NodeManagers.Alarms
{
    public class ConditionTypeHolder : BaseEventTypeHolder
    {

        protected ConditionTypeHolder(
            AlarmNodeManager alarmNodeManager,
            FolderState parent,
            SourceController trigger,
            string name,
            SupportedAlarmConditionType alarmConditionType,
            Type controllerType,
            int interval,
            bool optional) :
            base(alarmNodeManager, parent, trigger, name, alarmConditionType, controllerType, interval, optional)
        {
            alarmConditionType_ = alarmConditionType;
        }

        protected new void Initialize(
            uint alarmTypeIdentifier,
            string name)
        {
            if (alarm_ == null)
            {
                // this is invalid
                alarm_ = new ConditionState(parent_);
            }

            ConditionState alarm = GetAlarm();

            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name);

            // Set all ConditionType Parameters
            alarm.ClientUserId.Value = "Anonymous";
            alarm.ConditionClassId.Value = alarmConditionType_.Node;
            alarm.ConditionClassName.Value = new LocalizedText("", alarmConditionType_.ConditionName);
            alarm.ConditionName.Value = alarmRootName_;
            Utils.LogTrace("Alarm ConditionName = {0}", alarm.ConditionName.Value);

            alarm.BranchId.Value = new NodeId();
            alarm.Retain.Value = false;

            alarm.SetEnableState(SystemContext, true);
            alarm.Quality.Value = Opc.Ua.StatusCodes.Good;
            alarm.LastSeverity.Value = AlarmConstants.InactiveSeverity;
            alarm.Severity.Value = AlarmConstants.InactiveSeverity;
            alarm.Comment.Value = new LocalizedText("en", "");

            // Set Method Handlers
            alarm.OnEnableDisable = OnEnableDisableAlarm;
            alarm.OnAddComment = OnAddComment;

            alarm.ConditionSubClassId = null;
            alarm.ConditionSubClassName = null;
        }


        public BaseEventState FindBranch()
        {
            BaseEventState state = null;

            return state;
        }

        protected override void CreateBranch()
        {
            if (SupportsBranching)
            {
                ConditionState alarm = GetAlarm();

                int currentSeverity = alarm.Severity.Value;
                int newSeverity = GetSeverity();
                // A branch is created at the end of an active cycle
                // This could be a transition between alarm states,
                // or a transition to inactive
                // So a branch can only be created when the severity changes
                if (currentSeverity > AlarmConstants.InactiveSeverity &&
                    newSeverity != currentSeverity)
                {
                    NodeId branchId = GetNewBranchId();
                    ConditionState branch = alarm.CreateBranch(SystemContext, branchId);

                    string postEventId = Utils.ToHexString(branch.EventId.Value as byte[]);

                    Log("CreateBranch", " Branch " + branchId.ToString() +
                        " EventId " + postEventId + " created, Message " + alarm.Message.Value.Text);

                    alarmController_.SetBranchCount(alarm.GetBranchCount());
                }
            }
        }

        #region Overrides

        public override void SetValue(string message = "")
        {
            ConditionState alarm = GetAlarm();

            if (ShouldEvent() || message.Length > 0)
            {
                CreateBranch();

                int newSeverity = GetSeverity();

                alarm.SetSeverity(SystemContext, (EventSeverity)newSeverity);
                if (message.Length == 0)
                {
                    message = "Alarm Event Value = " + trigger_.Value.ToString();
                }

                alarm.Message.Value = new LocalizedText("en", message);

                ReportEvent();
            }
        }

        #endregion

        #region Child Helpers

        public void ReportEvent(ConditionState alarm = null)
        {
            if (alarm == null)
            {
                alarm = GetAlarm();
            }

            if (alarm.EnabledState.Id.Value)
            {
                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.Time.Value = DateTime.UtcNow;
                alarm.ReceiveTime.Value = alarm.Time.Value;

                Log("ReportEvent", " Value " + alarmController_.GetValue().ToString() +
                    " Message " + alarm.Message.Value.Text);

                alarm.ClearChangeMasks(SystemContext, true);

                InstanceStateSnapshot eventSnapshot = new InstanceStateSnapshot();
                eventSnapshot.Initialize(SystemContext, alarm);
                alarm.ReportEvent(SystemContext, eventSnapshot);
            }
        }

        protected virtual ushort GetSeverity()
        {
            ushort severity = AlarmConstants.InactiveSeverity;

            int level = alarmController_.GetValue();

            if (Analog)
            {
                if (level <= AlarmConstants.LowLowAlarm && Analog)
                {
                    severity = AlarmConstants.LowLowSeverity;
                }
                // Level is Low
                else if (level <= AlarmConstants.LowAlarm)
                {
                    severity = AlarmConstants.LowSeverity;
                }
                // Level is HighHigh
                else if (level >= AlarmConstants.HighHighAlarm && Analog)
                {
                    severity = AlarmConstants.HighHighSeverity;
                }
                // Level is High
                else if (level >= AlarmConstants.HighAlarm)
                {
                    severity = AlarmConstants.HighSeverity;
                }
            }
            else
            {
                if (level <= AlarmConstants.BoolLowAlarm)
                {
                    severity = AlarmConstants.LowSeverity;
                }
                // Level is High
                else if (level >= AlarmConstants.BoolHighAlarm)
                {
                    severity = AlarmConstants.HighSeverity;
                }

            }

            return severity;
        }

        protected bool IsActive()
        {
            bool isActive = false;
            if (GetSeverity() > AlarmConstants.InactiveSeverity)
            {
                isActive = true;
            }
            return isActive;
        }

        protected bool WasActive()
        {
            bool wasActive = false;
            ConditionState alarm = GetAlarm();
            if (alarm.Severity.Value > AlarmConstants.InactiveSeverity)
            {
                wasActive = true;
            }
            return wasActive;
        }

        protected bool ShouldEvent()
        {
            bool shouldEvent = false;
            ConditionState alarm = GetAlarm();
            ushort newSeverity = GetSeverity();
            if (newSeverity != alarm.Severity.Value)
            {
                shouldEvent = true;
            }

            return shouldEvent;
        }

        #endregion

        #region Helpers

        private ConditionState GetAlarm(BaseEventState alarm = null)
        {
            if (alarm == null)
            {
                alarm = alarm_;
            }
            return (ConditionState)alarm;
        }



        protected bool IsEvent(string caller, byte[] eventId)
        {
            bool isEvent = IsEvent(eventId);

            if (!isEvent)
            {
                LogError(caller, EventErrorMessage(eventId));
            }

            return isEvent;
        }

        protected string EventErrorMessage(byte[] eventId)
        {
            return " Requested Event " + Utils.ToHexString(eventId);
        }


        #endregion

        #region Method Handlers 
        public ServiceResult OnEnableDisableAlarm(
            ISystemContext context,
            ConditionState condition,
            bool enabling)
        {
            StatusCode status = StatusCodes.Good;

            ConditionState alarm = GetAlarm();

            if (enabling != alarm.EnabledState.Id.Value)
            {
                alarm.SetEnableState(SystemContext, enabling);
                alarm.Message.Value = enabling ? "Enabling" : "Disabling" + " alarm " + MapName;

                // if disabled, it will not fire
                ReportEvent();
            }
            else
            {
                if (enabling)
                {
                    status = StatusCodes.BadConditionAlreadyEnabled;
                }
                else
                {
                    status = StatusCodes.BadConditionAlreadyDisabled;
                }
            }

            return status;
        }

        private ServiceResult OnAddComment(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            ConditionState alarm = GetAlarm();

            ConditionState alarmOrBranch = alarm.GetEventByEventId(eventId);
            if (alarmOrBranch == null)
            {
                string errorMessage = "Unknown event id " + Utils.ToHexString(eventId);
                alarm.Message.Value = "OnAddComment " + errorMessage;
                LogError("OnAddComment", errorMessage);
                return StatusCodes.BadEventIdUnknown;
            }

            alarmController_.OnAddComment();

            // Don't call ReportEvent,  Core will send the event.

            delayedMessages_.Add("OnAddComment");

            return ServiceResult.Good;
        }

        protected bool CanSetComment(LocalizedText comment)
        {
            bool canSetComment = false;

            if (comment != null)
            {
                canSetComment = true;

                bool emptyComment = comment.Text == null || comment.Text.Length == 0;
                bool emptyLocale = comment.Locale == null || comment.Locale.Length == 0;

                if (emptyComment && emptyLocale)
                {
                    canSetComment = false;
                }
            }

            return canSetComment;
        }

        protected virtual bool GetRetainState()
        {
            return true;
        }

        #endregion
    }



}
