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

using Opc.Ua;
#endregion

#pragma warning disable CS1591

namespace SampleCompany.NodeManagers.Alarms
{
    public class AlarmConditionTypeHolder : AcknowledgeableConditionTypeHolder
    {
        public AlarmConditionTypeHolder(
            AlarmNodeManager alarmNodeManager,
            FolderState parent,
            SourceController trigger,
            string name,
            SupportedAlarmConditionType alarmConditionType,
            Type controllerType,
            int interval,
            bool optional = true,
            double maxShelveTime = AlarmConstants.NormalMaxTimeShelved,
            bool create = true) :
            base(alarmNodeManager, parent, trigger, name, alarmConditionType, controllerType, interval, optional, false)
        {
            if (create)
            {
                Initialize(Opc.Ua.ObjectTypes.AlarmConditionType, name, maxShelveTime);
            }
        }

        public void Initialize(
            uint alarmTypeIdentifier,
            string name,
            double maxTimeShelved = AlarmConstants.NormalMaxTimeShelved)
        {
            // Create an alarm and trigger name - Create a base method for creating the trigger, just provide the name

            if (alarm_ == null)
            {
                alarm_ = new AlarmConditionState(parent_);
            }

            AlarmConditionState alarm = GetAlarm();

            if (Optional)
            {
                if (alarm.SuppressedState == null)
                {
                    alarm.SuppressedState = new TwoStateVariableState(alarm);
                }


                if (alarm.OutOfServiceState == null)
                {
                    alarm.OutOfServiceState = new TwoStateVariableState(alarm);
                }

                if (alarm.ShelvingState == null)
                {
                    alarm.ShelvingState = new ShelvedStateMachineState(alarm);
                    alarm.ShelvingState.Create(SystemContext,
                        null,
                        BrowseNames.ShelvingState,
                        BrowseNames.ShelvingState,
                        false);
                }
                if (alarm.MaxTimeShelved == null)
                {
                    // Off normal does not create MaxTimeShelved.
                    alarm.MaxTimeShelved = new PropertyState<double>(alarm);
                }

            }


            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name);

            alarm.SetActiveState(SystemContext, active: false);
            alarm.InputNode.Value = new NodeId(trigger_.NodeId);

            if (Optional)
            {
                alarm.SetSuppressedState(SystemContext, suppressed: false);
                alarm.SetShelvingState(SystemContext, shelved: false, oneShot: false, shelvingTime: Double.MaxValue);
                alarm.ShelvingState.LastTransition.Value = new LocalizedText("");
                alarm.ShelvingState.LastTransition.Id.Value = 0;

                alarm.OnShelve = OnShelve;
                alarm.OnTimedUnshelve = OnTimedUnshelve;
                alarm.UnshelveTimeUpdateRate = 2000;

                alarm.MaxTimeShelved.Value = maxTimeShelved;

                alarm.LatchedState.Value = new LocalizedText("");
                alarm.LatchedState.Id.Value = false;


            }
            else
            {
                alarm.ShelvingState = null;
                alarm.LatchedState = null;
            }

            alarm.AudibleSound = null;
            alarm.AudibleEnabled = null;
        }

        #region Overrides

        public override void SetValue(string message = "")
        {
            bool setValue = false;
            AlarmConditionState alarm = GetAlarm();


            if (ShouldEvent())
            {
                alarm.SetActiveState(SystemContext, IsActive());

                setValue = true;
            }

            if (UpdateSuppression())
            {
                if (message.Length <= 0)
                {
                    message = "Updating due to Shelving State Update: " + alarm.ShelvingState.CurrentState.Value.ToString();
                }
                setValue = true;
            }
            else if (UpdateSuppression())
            {
                if (message.Length <= 0)
                {
                    message = "Updating due to Suppression Update: " + alarm.SuppressedState.Value.ToString();
                }
                setValue = true;
            }

            if (setValue)
            {
                base.SetValue(message);
            }
        }

        protected override bool GetRetainState()
        {
            AlarmConditionState alarm = GetAlarm();

            bool retainState = true;

            if (!alarm.ActiveState.Id.Value)
            {
                if (alarm.AckedState.Id.Value)
                {
                    if ((Optional))
                    {
                        if (alarm.ConfirmedState.Id.Value)
                        {
                            retainState = false;
                        }
                    }
                    else
                    {
                        retainState = false;
                    }
                }
            }

            return retainState;
        }

        protected override void SetActive(BaseEventState baseEvent, bool activeState)
        {
            AlarmConditionState alarm = GetAlarm(baseEvent);
            alarm.SetActiveState(SystemContext, activeState);
        }

        protected override bool UpdateShelving()
        {
            // Don't have to worry about changing state to Unshelved, there is an SDK timer to deal with that.
            bool update = false;

            return update;
        }

        protected override bool UpdateSuppression()
        {
            bool update = false;
            if (Optional)
            {
                AlarmConditionState alarm = GetAlarm();

                if (alarmController_.ShouldSuppress())
                {
                    alarm.SetSuppressedState(SystemContext, true);
                    update = true;
                }

                if (alarmController_.ShouldUnsuppress())
                {
                    alarm.SetSuppressedState(SystemContext, false);
                    update = true;
                }
            }
            return update;
        }

        #endregion

        #region Helpers

        private AlarmConditionState GetAlarm(BaseEventState alarm = null)
        {
            if (alarm == null)
            {
                alarm = alarm_;
            }
            return (AlarmConditionState)alarm;
        }


        #endregion

        #region Methods

        private ServiceResult OnShelve(
            ISystemContext context,
            AlarmConditionState alarm,
            bool shelving,
            bool oneShot,
            double shelvingTime)
        {
            string shelved = "Shelved";
            string dueTo = "";

            if (shelving)
            {
                if (oneShot)
                {
                    dueTo = " due to OneShotShelve";
                }
                else
                {
                    dueTo = " due to TimedShelve of " + shelvingTime.ToString();
                }
            }
            else
            {
                shelved = "Unshelved";
            }

            alarm.Message.Value = "The alarm is " + shelved + dueTo;
            alarm.SetShelvingState(context, shelving, oneShot, shelvingTime);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Called when the alarm is shelved.
        /// </summary>
        private ServiceResult OnTimedUnshelve(
            ISystemContext context,
            AlarmConditionState alarm)
        {
            // update the alarm state and produce and event.
            alarm.Message.Value = "The timed shelving period expired.";
            alarm.SetShelvingState(context, false, false, 0);

            base.SetValue(alarm.Message.Value.Text);

            return ServiceResult.Good;
        }

        #endregion
    }
}
