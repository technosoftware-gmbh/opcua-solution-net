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
    public class AverageAggregateCalculator : AggregateCalculator
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
        public AverageAggregateCalculator(
            NodeId aggregateId,
            DateTime startTime,
            DateTime endTime,
            double processingInterval,
            bool stepped,
            AggregateConfiguration configuration)
        : 
            base(aggregateId, startTime, endTime, processingInterval, stepped, configuration)
        {
            SetPartialBit = aggregateId != Opc.Ua.ObjectIds.AggregateFunction_Average;
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
                    case Objects.AggregateFunction_Average:
                    {
                        return ComputeAverage(slice);
                    }

                    case Objects.AggregateFunction_TimeAverage:
                    {
                        return ComputeTimeAverage(slice, false, 1);
                    }

                    case Objects.AggregateFunction_Total:
                    {
                        return ComputeTimeAverage(slice, false, 2);
                    }

                    case Objects.AggregateFunction_TimeAverage2:
                    {
                        return ComputeTimeAverage(slice, true, 1);
                    }

                    case Objects.AggregateFunction_Total2:
                    {
                        return ComputeTimeAverage(slice, true, 2);
                    }
                }
            }

            return base.ComputeValue(slice);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Calculates the RegSlope, RegConst and RegStdDev aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeAverage(TimeSlice slice)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // calculate total and count.
            var count = 0;
            double total = 0;

            for (var ii = 0; ii < values.Count; ii++)
            {
                if (StatusCode.IsGood(values[ii].StatusCode))
                {
                    try
                    {
                        var sample = CastToDouble(values[ii]);
                        total += sample;
                        count++;
                    }
                    catch
                    {
                        // ignore conversion errors.
                    }
                }
            }

            // check for empty slice.
            if (count == 0)
            {
                return GetNoDataValue(slice);
            }

            // select the result.
            var result = total/count;

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(result, TypeInfo.Scalars.Double);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);
            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);
            value.StatusCode = GetValueBasedStatusCode(slice, values, value.StatusCode);

            // return result.
            return value;
        }

        /// <summary>
        /// Calculates the StdDev, Variance, StdDev2 and Variance2 aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeTimeAverage(TimeSlice slice, bool useSimpleBounds, int valueType)
        {
            // get the values in the slice.
            List<DataValue> values = null;

            if (useSimpleBounds)
            {
                values = GetValuesWithSimpleBounds(slice);
            }
            else
            {
                values = GetValuesWithInterpolatedBounds(slice);
            }

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // get the regions.
            var regions = GetRegionsInValueSet(values, !useSimpleBounds, Stepped);

            double total = 0;
            double totalDuration = 0;
            var nonGoodRegionsExists = false;

            for (var ii = 0; ii < regions.Count; ii++)
            {
                var duration = regions[ii].Duration/1000.0;

                if (StatusCode.IsNotBad(regions[ii].StatusCode))
                {
                    total += (regions[ii].StartValue + regions[ii].EndValue) * duration / 2;
                    totalDuration += duration;
                }

                if (StatusCode.IsNotGood(regions[ii].StatusCode))
                {
                    nonGoodRegionsExists = true;
                }
            }

            // check if no good data.
            if (totalDuration == 0)
            {
                return GetNoDataValue(slice);
            }

            // select the result.
            double result = 0;

            switch (valueType)
            {
                case 1: { result = total/totalDuration; break; }
                case 2: { result = total; break; }
            }

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(result, TypeInfo.Scalars.Double);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);

            if (useSimpleBounds)
            {
                value.StatusCode = GetTimeBasedStatusCode(regions, value.StatusCode);
            }
            else
            {
                value.StatusCode = StatusCodes.Good;

                if (nonGoodRegionsExists)
                {
                    value.StatusCode = StatusCodes.UncertainDataSubNormal;
                }
            }

            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);

            // return result.
            return value;
        }
        #endregion
    }
}
