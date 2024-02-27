#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives

using System;
using System.Collections.Generic;

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.Aggregates
{
    /// <summary>
    /// Calculates the value of an aggregate. 
    /// </summary>
    public class CountAggregateCalculator : AggregateCalculator
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the aggregate calculator.
        /// </summary>
        /// <param name="aggregateId">The aggregate function to apply.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="processingInterval">The processing interval.</param>
        /// <param name="stepped">Whether to use stepped interpolation.</param>
        /// <param name="configuration">The aggregate configuration.</param>
        public CountAggregateCalculator(
            NodeId aggregateId,
            DateTime startTime,
            DateTime endTime,
            double processingInterval,
            bool stepped,
            AggregateConfiguration configuration)
        : 
            base(aggregateId, startTime, endTime, processingInterval, stepped, configuration)
        {
            SetPartialBit = true;
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Computes the value for the timeslice.
        /// </summary>
        protected override DataValue ComputeValue(TimeSlice slice)
        {
            var id = AggregateId.Identifier as uint?;

            if (id != null)
            {
                switch (id.Value)
                {
                    case Objects.AggregateFunction_Count:
                    {
                        return ComputeCount(slice);
                    }

                    case Objects.AggregateFunction_AnnotationCount:
                    {
                        return ComputeAnnotationCount(slice);
                    }

                    case Objects.AggregateFunction_DurationInStateZero:
                    {
                        return ComputeDurationInState(slice, false);
                    }

                    case Objects.AggregateFunction_DurationInStateNonZero:
                    {
                        return ComputeDurationInState(slice, true);
                    }

                    case Objects.AggregateFunction_NumberOfTransitions:
                    {
                        return ComputeNumberOfTransitions(slice);
                    }
                }
            }

            return base.ComputeValue(slice);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Calculates the Count aggregate for the timeslice.
        /// </summary>
        protected DataValue ComputeCount(TimeSlice slice)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null)
            {
                return GetNoDataValue(slice);
            }

            // count the values.
            var count = 0;

            for (var ii = 0; ii < values.Count; ii++)
            {
                if (StatusCode.IsGood(values[ii].StatusCode))
                {
                    count++;
                }
            }

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(count, TypeInfo.Scalars.Int32);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);           
            value.StatusCode = GetValueBasedStatusCode(slice, values, value.StatusCode);

            if (!StatusCode.IsBad(value.StatusCode))
            {
                // set aggregate bits fon non Bad values
                value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);
            }
            // return result.
            return value;
        }

        /// <summary>
        /// Calculates the AnnotationCount aggregate for the timeslice.
        /// </summary>
        protected DataValue ComputeAnnotationCount(TimeSlice slice)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null)
            {
                return GetNoDataValue(slice);
            }

            // count the values.
            var count = 0;

            for (var ii = 0; ii < values.Count; ii++)
            {
                count++;
            }

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(count, TypeInfo.Scalars.Int32);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);
            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);

            // return result.
            return value;
        }

        /// <summary>
        /// Calculates the DurationInStateZero and DurationInStateNonZero aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeDurationInState(TimeSlice slice, bool isNonZero)
        {
            // get the values in the slice.
            var values = GetValuesWithSimpleBounds(slice);

            // check for empty slice.
            if (values == null)
            {
                return GetNoDataValue(slice);
            }

            // get the regions.
            var regions = GetRegionsInValueSet(values, false, true);

            double duration = 0;

            for (var ii = 0; ii < regions.Count; ii++)
            {
                if (StatusCode.IsNotGood(regions[ii].StatusCode))
                {
                    continue;
                }

                if (isNonZero)
                {
                    if (regions[ii].StartValue != 0)
                    {
                        duration += regions[ii].Duration;
                    }
                }
                else
                {
                    if (regions[ii].StartValue == 0)
                    {
                        duration += regions[ii].Duration;
                    }
                }
            }

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(duration, TypeInfo.Scalars.Double);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);            
            value.StatusCode = GetTimeBasedStatusCode(regions, value.StatusCode);
            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);

            // return result.
            return value;
        }

        /// <summary>
        /// Calculates the Count aggregate for the timeslice.
        /// </summary>
        protected DataValue ComputeNumberOfTransitions(TimeSlice slice)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null)
            {
                return GetNoDataValue(slice);
            }

            // determine whether a transition occurs at the StartTime
            var lastValue = Double.NaN;

            if (slice.EarlyBound != null)
            {
                if (StatusCode.IsGood(slice.EarlyBound.Value.StatusCode))
                {
                    try
                    {
                        lastValue = CastToDouble(slice.EarlyBound.Value);
                    }
                    catch (Exception)
                    {
                        lastValue = Double.NaN;
                    }
                }
            }

            // count the transitions.
            var count = 0;

            for (var ii = 0; ii < values.Count; ii++)
            {
                if (!IsGood(values[ii]))
                {
                    continue;
                }

                double nextValue = 0;

                try
                {
                    nextValue = CastToDouble(values[ii]);
                }
                catch (Exception)
                {
                    continue;
                }

                if (!Double.IsNaN(lastValue))
                {
                    if (lastValue != nextValue)
                    {
                        count++;
                    }
                }

                lastValue = nextValue;
            }

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(count, TypeInfo.Scalars.Int32);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);
            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);
            value.StatusCode = GetValueBasedStatusCode(slice, values, value.StatusCode);

            // return result.
            return value;
        }
        #endregion
    }
}
