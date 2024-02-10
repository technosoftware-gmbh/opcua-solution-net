
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
#endregion


namespace SampleCompany.NodeManagers.Alarms
{
    public class AlarmConstants
    {
        public const int MaxValue = 100;
        public const int MinValue = 0;
        public const int NormalStartValue = 50;

        public const int HighHighAlarm = 90;
        public const int HighAlarm = 70;
        public const int LowAlarm = 30;
        public const int LowLowAlarm = 10;

        public const int BoolHighAlarm = 80;
        public const int BoolLowAlarm = 20;

        public const int InactiveSeverity = 100;

        public const int HighHighSeverity = 850;
        public const int HighSeverity = 450;
        public const int LowSeverity = 400;
        public const int LowLowSeverity = 800;

        public const int BoolSeverity = 500;

        public const double NormalMaxTimeShelved = 2000000;
        public const double ShortMaxTimeShelved = 30000;

        public const int MillisecondsPerSecond = 1000;
        public const int MillisecondsPerMinute = 60 * MillisecondsPerSecond;
        public const int MillisecondsPerHour = 60 * MillisecondsPerMinute;
        public const int MillisecondsPerDay = 24 * MillisecondsPerHour;
        public const int MillisecondsPerWeek = 7 * MillisecondsPerDay;
        public const int MillisecondsPerTwoWeeks = 2 * MillisecondsPerWeek;

        public const string TriggerExtension = ".Trigger";
        public const string AlarmExtension = ".Alarm";
        public const string DiscrepancyTargetName = "TargetValueNodeId";
    }
}
