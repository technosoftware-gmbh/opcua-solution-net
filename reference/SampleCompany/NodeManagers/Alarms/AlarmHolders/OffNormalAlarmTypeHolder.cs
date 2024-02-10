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
    public class OffNormalAlarmTypeHolder : DiscreteHolder
    {
        public OffNormalAlarmTypeHolder(
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
                Initialize(ObjectTypes.OffNormalAlarmType, name, maxShelveTime);
            }
        }

        public new void Initialize(
            uint alarmTypeIdentifier,
            string name,
            double maxTimeShelved = AlarmConstants.NormalMaxTimeShelved)
        {
            if (alarm_ == null)
            {
                alarm_ = new OffNormalAlarmState(parent_);
            }

            OffNormalAlarmState alarm = GetAlarm();

            base.Initialize(alarmTypeIdentifier, name, maxTimeShelved);

            alarm.NormalState.Value = new NodeId();
        }

        #region Helpers

        private OffNormalAlarmState GetAlarm()
        {
            return (OffNormalAlarmState)alarm_;
        }

        #endregion

    }
}
