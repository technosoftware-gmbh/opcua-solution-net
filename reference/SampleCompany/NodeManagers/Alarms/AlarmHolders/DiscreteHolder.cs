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

#pragma warning disable CS0219

#pragma warning disable CS1591

namespace SampleCompany.NodeManagers.Alarms
{
    public class DiscreteHolder : AlarmConditionTypeHolder
    {
        public DiscreteHolder(
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
            Utils.LogTrace("{0} Discrete Constructor Optional = {1}", name, optional);
            if (create)
            {
                Initialize(ObjectTypes.DiscreteAlarmType, name, maxShelveTime);
            }
        }

        public new void Initialize(
            uint alarmTypeIdentifier,
            string name,
            double maxTimeShelved = AlarmConstants.NormalMaxTimeShelved)
        {
            analog_ = false;

            if (alarm_ == null)
            {
                alarm_ = new DiscreteAlarmState(parent_);
            }

            // Call the base class to set parameters
            base.Initialize(alarmTypeIdentifier, name, maxTimeShelved);
        }

        #region Overrides

        public override void SetValue(string message = "")
        {

            var active = alarmController_.IsBooleanActive();
            var value = alarmController_.GetValue();

            if (message.Length == 0)
            {
                message = "Discrete Alarm analog value = " + value.ToString() + ", active = " + active.ToString();
            }

            base.SetValue(message);
        }

        #endregion

        #region Helpers
        private DiscreteAlarmState GetAlarm()
        {
            return (DiscreteAlarmState)alarm_;
        }

        #endregion
    }


}
