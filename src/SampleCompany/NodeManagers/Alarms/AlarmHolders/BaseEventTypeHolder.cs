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
using System.Linq;

using Opc.Ua;
#endregion

#pragma warning disable CS1591

namespace SampleCompany.NodeManagers.Alarms
{
    public class BaseEventTypeHolder : AlarmHolder
    {
        protected BaseEventTypeHolder(
            AlarmNodeManager alarmNodeManager,
            FolderState parent,
            SourceController trigger,
            string name,
            SupportedAlarmConditionType alarmConditionType,
            Type controllerType,
            int interval,
            bool optional) :
            base(alarmNodeManager, parent, trigger, controllerType, interval)
        {
            optional_ = optional;
        }

        protected new void Initialize(
            uint alarmTypeIdentifier,
            string name)
        {
            alarmTypeIdentifier_ = alarmTypeIdentifier;

            if (alarm_ != null)
            {
                // Call the base class to set parameters
                base.Initialize(alarmTypeIdentifier, name);

                BaseEventState alarm = GetAlarm();

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = new NodeId(alarmTypeIdentifier, GetNameSpaceIndex(alarmTypeIdentifier));
                alarm.SourceNode.Value = trigger_.NodeId;
                alarm.SourceName.Value = trigger_.SymbolicName;
                alarm.Time.Value = DateTime.UtcNow;
                alarm.ReceiveTime.Value = alarm.Time.Value;
                alarm.Message.Value = name + " Initialized";
                alarm.Severity.Value = AlarmConstants.InactiveSeverity;

                // TODO Implement for Optionals - Needs to go to all places where Time is set.
                alarm.LocalTime = null;
            }
        }

        #region Overrides

        public override void SetValue(string message = "")
        {

        }

        #endregion

        #region Helpers

        private BaseEventState GetAlarm(BaseEventState alarm = null)
        {
            if (alarm == null)
            {
                alarm = alarm_;
            }
            return (BaseEventState)alarm;
        }


        #endregion

        #region Child Helpers

        protected bool IsEvent(byte[] eventId)
        {
            bool isEvent = false;
            if (GetAlarm().EventId.Value.SequenceEqual(eventId))
            {
                isEvent = true;
            }

            return isEvent;
        }


        #endregion

    }
}
