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
    class LimitAlarmTypeHolder : AlarmConditionTypeHolder
    {
        private bool m_isLimit = true;

        public LimitAlarmTypeHolder(
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
                Initialize(ObjectTypes.LimitAlarmType, name, maxShelveTime);
            }
        }

        public void Initialize(
            uint alarmTypeIdentifier,
            string name,
            double maxTimeShelved = AlarmConstants.NormalMaxTimeShelved,
            bool isLimit = true)
        {
            // Create an alarm and trigger name - Create a base method for creating the trigger, just provide the name

            if (alarm_ == null)
            {
                alarm_ = new LimitAlarmState(parent_);
            }

            m_isLimit = isLimit;

            LimitAlarmState alarm = GetAlarm();

            if (alarm.HighLimit == null)
            {
                alarm.HighLimit = new PropertyState<double>(alarm);
            }
            if (alarm.HighHighLimit == null)
            {
                alarm.HighHighLimit = new PropertyState<double>(alarm);
            }
            if (alarm.LowLimit == null)
            {
                alarm.LowLimit = new PropertyState<double>(alarm);
            }
            if (alarm.LowLowLimit == null)
            {
                alarm.LowLowLimit = new PropertyState<double>(alarm);
            }

            if (Optional)
            {
                alarm.BaseHighLimit = new PropertyState<double>(alarm);
                alarm.BaseHighHighLimit = new PropertyState<double>(alarm);
                alarm.BaseLowLimit = new PropertyState<double>(alarm);
                alarm.BaseLowLowLimit = new PropertyState<double>(alarm);
            }

            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name, maxTimeShelved);

            alarm.HighLimit.Value = AlarmConstants.HighAlarm;
            alarm.HighHighLimit.Value = AlarmConstants.HighHighAlarm;
            alarm.LowLimit.Value = AlarmConstants.LowAlarm;
            alarm.LowLowLimit.Value = AlarmConstants.LowLowAlarm;

            if (Optional)
            {
                alarm.BaseHighLimit.Value = AlarmConstants.HighAlarm;
                alarm.BaseHighHighLimit.Value = AlarmConstants.HighHighAlarm;
                alarm.BaseLowLimit.Value = AlarmConstants.LowAlarm;
                alarm.BaseLowLowLimit.Value = AlarmConstants.LowLowAlarm;
            }
            else
            {
                alarm.BaseHighHighLimit = null;
                alarm.BaseLowLimit = null;
                alarm.BaseLowLowLimit = null;
            }
        }

        #region Helpers

        private LimitAlarmState GetAlarm()
        {
            return (LimitAlarmState)alarm_;
        }

        #endregion


    }
}
