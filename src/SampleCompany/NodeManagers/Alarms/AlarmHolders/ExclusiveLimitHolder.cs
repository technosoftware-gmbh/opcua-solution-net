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

#pragma warning disable CS0219

namespace SampleCompany.NodeManagers.Alarms
{
    class ExclusiveLimitHolder : LimitAlarmTypeHolder
    {
        public ExclusiveLimitHolder(
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
                Initialize(Opc.Ua.ObjectTypes.ExclusiveLimitAlarmType, name, maxShelveTime);
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
                alarm_ = new ExclusiveLimitAlarmState(parent_);
            }

            ExclusiveLimitAlarmState alarm = GetAlarm();

            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name, maxTimeShelved);

            alarm.SetLimitState(SystemContext, LimitAlarmStates.Inactive);
        }


        public override void SetValue(string message = "")
        {
            ExclusiveLimitAlarmState alarm = GetAlarm();
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

        private ExclusiveLimitAlarmState GetAlarm()
        {
            return (ExclusiveLimitAlarmState)alarm_;
        }

    }
}
