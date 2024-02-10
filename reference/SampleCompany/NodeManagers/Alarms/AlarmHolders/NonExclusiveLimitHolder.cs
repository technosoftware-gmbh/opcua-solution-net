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

namespace SampleCompany.NodeManagers.Alarms
{
    class NonExclusiveLimitHolder : LimitAlarmTypeHolder
    {
        public NonExclusiveLimitHolder(
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
            base(alarmNodeManager, parent, trigger, name, alarmConditionType, controllerType, interval, optional, maxShelveTime, false)
        {
            if (create)
            {
                Initialize(ObjectTypes.NonExclusiveLimitAlarmType, name, maxShelveTime);
            }
        }

        public new void Initialize(
            uint alarmTypeIdentifier,
            string name,
            double maxTimeShelved = AlarmConstants.NormalMaxTimeShelved)
        {
            // Create an alarm and trigger name - Create a base method for creating the trigger, just provide the name

            if (alarm_ == null)
            {
                alarm_ = new NonExclusiveLimitAlarmState(parent_);
            }

            NonExclusiveLimitAlarmState alarm = GetAlarm();

            alarm.HighState = new TwoStateVariableState(alarm);

            alarm.HighHighState = new TwoStateVariableState(alarm);
            alarm.LowState = new TwoStateVariableState(alarm);
            alarm.LowLowState = new TwoStateVariableState(alarm);

            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name, maxTimeShelved);

            alarm.SetLimitState(SystemContext, LimitAlarmStates.Inactive);

        }



        public override void SetValue(string message = "")
        {
            NonExclusiveLimitAlarmState alarm = GetAlarm();
            int newSeverity = GetSeverity();
            int currentSeverity = alarm.Severity.Value;

            if (newSeverity != currentSeverity)
            {
                LimitAlarmStates state = LimitAlarmStates.Inactive;

                if (newSeverity == AlarmConstants.HighHighSeverity)
                {
                    state = LimitAlarmStates.HighHigh;
                }
                else if (newSeverity == AlarmConstants.HighSeverity)
                {
                    state = LimitAlarmStates.High;
                }
                else if (newSeverity == AlarmConstants.LowSeverity)
                {
                    state = LimitAlarmStates.Low;
                }
                else if (newSeverity == AlarmConstants.LowLowSeverity)
                {
                    state = LimitAlarmStates.LowLow;
                }

                alarm.SetLimitState(SystemContext, state);
            }

            base.SetValue(message);
        }

        private NonExclusiveLimitAlarmState GetAlarm()
        {
            return (NonExclusiveLimitAlarmState)alarm_;
        }

    }
}
