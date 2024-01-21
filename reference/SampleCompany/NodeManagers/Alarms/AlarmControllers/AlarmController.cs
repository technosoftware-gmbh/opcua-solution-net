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
    /// <summary>
    /// 
    /// </summary>
    public class AlarmController
    {
        #region Constants
        private const int DefaultCycleTime = 180;
        #endregion

        #region Constructors, Destructor, Initialization
        public AlarmController(BaseDataVariableState variable, int interval, bool isBoolean)
        {
            variable_ = variable;
            interval_ = interval;
            isBoolean_ = isBoolean;
            increment_ = true;

            value_ = midpoint_;
            stopTime_ = stopTime_.AddYears(5);

            allowChanges_ = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the Alarm cycles for n seconds.
        /// </summary>
        public virtual void Start(UInt32 seconds = 0)
        {
            Stop();

            Utils.LogInfo("Start the Alarms for {0} seconds!", seconds);

            validLastMaxValue_ = false;

            nextTime_ = 
            stopTime_ = DateTime.Now;
            stopTime_ = stopTime_.AddSeconds(seconds == 0 ? DefaultCycleTime : seconds);

            allowChanges_ = true;
        }

        public virtual void Stop()
        {
            Utils.LogInfo("Stop the Alarms!");

            value_ = midpoint_;
            increment_ = true;
            allowChanges_ = false;

            reset_ = true;
        }

        public virtual bool Update(ISystemContext systemContext)
        {
            var valueSet = false;
            if (CanSetValue())
            {
                var value = 0;
                var boolValue = false;
                GetValue(ref value, ref boolValue);

                Utils.LogInfo("AlarmController Update Value = {0}", value);

                if (isBoolean_)
                {
                    variable_.Value = boolValue;
                }
                else
                {
                    variable_.Value = value;
                }
                variable_.Timestamp = DateTime.UtcNow;
                variable_.ClearChangeMasks(systemContext, false);

                valueSet = true;
            }

            return valueSet;
        }

        public void ManualWrite(object value)
        {
            if (value.GetType().Name == "Int32")
            {
                // Don't let anyone write a value out of range
                var potentialWrite = (Int32)value;
                if (potentialWrite >= AlarmConstants.MinValue && potentialWrite <= AlarmConstants.MaxValue)
                {
                    value_ = potentialWrite;
                }
                else
                {
                    Utils.LogError("AlarmController Received out of range manual write of {0}", value);
                }
            }
            else
            {
                if ((bool)value)
                {
                    value_ = 70;
                    increment_ = true;
                }
                else
                {
                    value_ = midpoint_;
                }
            }
        }

        public virtual bool CanSetValue()
        {
            var setValue = false;

            if (DateTime.Now > stopTime_)
            {
                Stop();
                stopTime_ = DateTime.MaxValue;
            }
            else if (allowChanges_ || reset_)
            {
                reset_ = false;

                if (DateTime.Now > nextTime_)
                {
                    SetNextInterval();
                    setValue = true;
                }
            }

            return setValue;
        }

        public bool SupportsBranching
        {
            get { return supportsBranching_; }
            set { supportsBranching_ = value; }
        }

        public virtual void SetBranchCount(int count)
        {
            branchCount_ = count;
        }

        public bool IsBooleanActive()
        {
            var isActive = false;
            if (value_ >= AlarmConstants.BoolHighAlarm || value_ <= AlarmConstants.BoolLowAlarm)
            {
                isActive = true;
            }

            return isActive;
        }

        public int GetValue()
        {
            return value_;
        }

        public int GetSawtooth()
        {
            return value_;
        }

        public int GetSine(int minValue, int maxValue)
        {
            return CalcSine(minValue, maxValue, value_);
        }

        public int CalcSine(int minValue, int maxValue, int value)
        {
            // What I want is a sawtooth compared against a sine value.
            // This calculates a simular sine value that will have predictable differences between value and sine

            /*
             * https://www.mathsisfun.com/algebra/amplitude-period-frequency-phase-shift.html
             * Sine with Phase Shift and Vertical Shift!  This is what I want
             * y = A sin(B(x + C)) + D
             * A - Amplitude
             * B - relates to period - This should extend the time period
             * C - Phase Shift
             * D - Vertical Shift
             * 
             */

            var twoPi = Math.PI * 2;

            double normalSpan = maxValue - minValue;
            var amplitude = normalSpan / 2;
            var median = maxValue - amplitude;

            double offsetValue = value - minValue;
            var percentageOfRange = offsetValue / normalSpan;

            var reducedPeriod = percentageOfRange / 2;

            var period = twoPi; // this would relate to the interval.  Ignore for now.
            var phase = -0.25; // phaseShift;
            var verticalShift = median; // amplitude

            var calculated = (amplitude * Math.Sin(period * (reducedPeriod + phase))) + verticalShift;

            Utils.LogTrace(
                " Phase {0:0.00} Value {1} Sine {2:0.00}" +
                " Offset Value {3:0.00} Span {4:0.00}" +
                " Percentage of Range {5:0.00}",
                phase, value, calculated, offsetValue, normalSpan, percentageOfRange);

            return (int)calculated;
        }

        public virtual bool ShouldSuppress()
        {
            return false;
        }

        public virtual bool ShouldUnsuppress()
        {
            return false;
        }

        public virtual void OnAddComment()
        {

        }

        public virtual void OnAcknowledge()
        {

        }

        public virtual void OnConfirm()
        {

        }
        #endregion

        #region Protected Methods


        protected virtual void SetNextInterval()
        {
            nextTime_ = DateTime.Now;
            nextTime_ = nextTime_.AddMilliseconds(interval_);
        }

        protected virtual void GetValue(ref int intValue, ref bool boolValue)
        {
            var maxValue = 100;
            var minValue = 0;

            TypicalGetValue(minValue, maxValue, ref intValue, ref boolValue);
        }
        protected void TypicalGetValue(int minValue, int maxValue, ref int intValue, ref bool boolValue)
        {
            var incrementValue = 5;
            if (isBoolean_)
            {
                incrementValue = 10;
            }
            if (increment_)
            {
                value_ += incrementValue;
                if (value_ >= maxValue)
                {
                    if (validLastMaxValue_)
                    {
                        Utils.LogInfo(
                            "Cycle Time {0} Interval {1}", (DateTime.Now - lastMaxValue_), interval_);
                    }
                    lastMaxValue_ = DateTime.Now;
                    validLastMaxValue_ = true;

                    increment_ = false;
                }
            }
            else
            {
                value_ -= incrementValue;
                if (value_ <= minValue)
                {
                    increment_ = true;
                }
            }

            intValue = value_;
            boolValue = IsBooleanActive();
        }
        #endregion

        #region Protected Fields
        protected BaseDataVariableState variable_;
        protected int value_;
        protected bool increment_ = true;
        protected DateTime nextTime_ = DateTime.Now;
        protected DateTime stopTime_ = DateTime.Now;
        protected int interval_;
        protected bool isBoolean_;
        protected bool allowChanges_;
        protected bool reset_;
        protected DateTime lastMaxValue_;
        protected bool validLastMaxValue_;
        protected int midpoint_ = AlarmConstants.NormalStartValue;
        #endregion

        #region Private Fields
        private int branchCount_;
        private bool supportsBranching_;
        #endregion
    }
}
