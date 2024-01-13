#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

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
    public class MinMaxAggregateCalculator : AggregateCalculator
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
        public MinMaxAggregateCalculator(
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
                    case Objects.AggregateFunction_Minimum:
                    {
                        return ComputeMinMax(slice, 1, false);
                    }

                    case Objects.AggregateFunction_MinimumActualTime:
                    {
                        return ComputeMinMax(slice, 1, true);
                    }

                    case Objects.AggregateFunction_Maximum:
                    {
                        return ComputeMinMax(slice, 2, false);
                    }

                    case Objects.AggregateFunction_MaximumActualTime:
                    {
                        return ComputeMinMax(slice, 2, true);
                    }

                    case Objects.AggregateFunction_Range:
                    {
                        return ComputeMinMax(slice, 3, false);
                    }

                    case Objects.AggregateFunction_Minimum2:
                    {
                        return ComputeMinMax2(slice, 1, false);
                    }

                    case Objects.AggregateFunction_MinimumActualTime2:
                    {
                        return ComputeMinMax2(slice, 1, true);
                    }

                    case Objects.AggregateFunction_Maximum2:
                    {
                        return ComputeMinMax2(slice, 2, false);
                    }

                    case Objects.AggregateFunction_MaximumActualTime2:
                    {
                        return ComputeMinMax2(slice, 2, true);
                    }

                    case Objects.AggregateFunction_Range2:
                    {
                        return ComputeMinMax2(slice, 3, false);
                    }
                }
            }

            return base.ComputeValue(slice);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Calculate the Minimum, Maximum, MinimumActualTime and MaximumActualTime aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeMinMax(TimeSlice slice, int valueType, bool returnActualTime)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            var minimumGoodValue = Double.MaxValue;
            var minimumUncertainValue = Double.MaxValue;
            var maximumGoodValue = Double.MinValue;
            var maximumUncertainValue = Double.MinValue;

            var minimumGoodTimestamp = DateTime.MinValue;
            var maximumGoodTimestamp = DateTime.MinValue;

            TypeInfo minimumOriginalType = null;
            TypeInfo maximumOriginalType = null;

            var badValuesExist = false;
            var duplicatesMinimumsExist = false;
            var duplicatesMaximumsExist = false;
            var goodValueExists = false;

            for (var ii = 0; ii < values.Count; ii++)
            {
                double currentValue = 0;
                var currentTime = values[ii].SourceTimestamp;
                var currentStatus = values[ii].StatusCode;

                // ignore bad values.
                if (!IsGood(values[ii]))
                {
                    badValuesExist = true;
                    continue;
                }

                // convert to double.
                try
                {
                    currentValue = CastToDouble(values[ii]);
                }
                catch (Exception)
                {
                    badValuesExist = true;
                    continue;
                }

                // check for uncertain.
                if (StatusCode.IsUncertain(currentStatus))
                {
                    if (minimumUncertainValue > currentValue)
                    {
                        minimumUncertainValue = currentValue;
                    }

                    if (maximumUncertainValue < currentValue)
                    {
                        maximumUncertainValue = currentValue;
                    }

                    continue;
                }

                // check for new minimum.
                if (minimumGoodValue > currentValue)
                {
                    minimumGoodValue = currentValue;
                    minimumGoodTimestamp = currentTime;
                    minimumOriginalType = values[ii].WrappedValue.TypeInfo;
                    duplicatesMinimumsExist = false;
                    goodValueExists = true;
                }

                // check for duplicate minimums.
                else if (minimumGoodValue == currentValue)
                {
                    duplicatesMinimumsExist = true;
                }

                // check for new maximum.
                if (maximumGoodValue < currentValue)
                {
                    maximumGoodValue = currentValue;
                    maximumGoodTimestamp = currentTime;
                    maximumOriginalType = values[ii].WrappedValue.TypeInfo;
                    duplicatesMaximumsExist = false;
                    goodValueExists = true;
                }

                // check for duplicate maximums.
                else if (maximumGoodValue == currentValue)
                {
                    duplicatesMaximumsExist = true;
                }
            }

            // check if at least one good value exists.
            if (!goodValueExists)
            {
                return GetNoDataValue(slice);
            }

            // set the status code.
            StatusCode statusCode = StatusCodes.Good;

            // uncertain if any bad values exist.
            if (badValuesExist)
            {
                statusCode = StatusCodes.UncertainDataSubNormal;
            }

            // determine the calculated value to return.
            object processedValue = null;
            TypeInfo processedType = null;
            var processedTimestamp = DateTime.MinValue;
            var uncertainValueExists = false;
            var duplicatesExist = false;

            if (valueType == 1)
            {
                processedValue = minimumGoodValue;
                processedTimestamp = minimumGoodTimestamp;
                processedType = minimumOriginalType;
                uncertainValueExists = minimumGoodValue > minimumUncertainValue;
                duplicatesExist = duplicatesMinimumsExist;
            }

            else if (valueType == 2)
            {
                processedValue = maximumGoodValue;
                processedTimestamp = maximumGoodTimestamp;
                processedType = maximumOriginalType;
                uncertainValueExists = maximumGoodValue < maximumUncertainValue;
                duplicatesExist = duplicatesMaximumsExist;
            }

            else if (valueType == 3)
            {
                processedValue = Math.Abs(maximumGoodValue - minimumGoodValue);
                processedType = TypeInfo.Scalars.Double;
                uncertainValueExists = maximumGoodValue < maximumUncertainValue || minimumGoodValue > minimumUncertainValue;
            }

            // set calculated if not returning actual time and value is not at the start time.
            if (!returnActualTime && processedTimestamp != slice.StartTime)
            {
                statusCode = statusCode.SetAggregateBits(AggregateBits.Calculated);
            }

            // set the multiple values flags.
            if (duplicatesExist)
            {
                statusCode = statusCode.SetAggregateBits(statusCode.AggregateBits | AggregateBits.MultipleValues);
            }

            // convert back to original datatype.
            if (processedType != null && processedType.BuiltInType != BuiltInType.Double)
            {
                processedValue = TypeInfo.Cast(processedValue, TypeInfo.Scalars.Double, processedType.BuiltInType);
            }
            else
            {
                processedType = TypeInfo.Scalars.Double;
            }

            // create processed value.
            var value = new DataValue();
            value.WrappedValue = new Variant(processedValue, processedType);
            value.StatusCode = statusCode;

            if (returnActualTime)
            {
                value.SourceTimestamp = processedTimestamp;
                value.ServerTimestamp = processedTimestamp;
            }
            else
            {
                value.SourceTimestamp = GetTimestamp(slice);
                value.ServerTimestamp = GetTimestamp(slice);
            }

            return value;
        }

        /// <summary>
        /// Calculate the Minimum2, Maximum2, MinimumActualTime2, MaximumActualTime2 and Range2 aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeMinMax2(TimeSlice slice, int valueType, bool returnActualTime)
        {
            // get the values in the slice.
            var values = GetValuesWithSimpleBounds(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            var minimumGoodValue = Double.MaxValue;
            var maximumGoodValue = Double.MinValue;

            var minimumGoodTimestamp = DateTime.MinValue;
            var maximumGoodTimestamp = DateTime.MinValue;

            StatusCode minimumGoodStatusCode = StatusCodes.Good;
            StatusCode maximumGoodStatusCode = StatusCodes.Good;

            TypeInfo minimumOriginalType = null;
            TypeInfo maximumOriginalType = null;

            var duplicatesMinimumsExist = false;
            var duplicatesMaximumsExist = false;
            var goodValueExists = false;

            for (var ii = 0; ii < values.Count; ii++)
            {
                double currentValue = 0;
                var currentTime = values[ii].SourceTimestamp;
                var currentStatus = values[ii].StatusCode;

                // ignore bad values (as determined by the TreatUncertainAsBad parameter).
                if (!IsGood(values[ii]))
                {
                    continue;
                }

                // convert to double.
                try
                {
                    currentValue = CastToDouble(values[ii]);
                }
                catch (Exception)
                {
                    continue;
                }

                // skip endpoint if stepped.
                if (currentTime == slice.EndTime)
                {
                    if (Stepped)
                    {
                        break;
                    }
                }

                // check for new minimum.
                if (minimumGoodValue > currentValue)
                {
                    minimumGoodValue = currentValue;
                    minimumGoodTimestamp = currentTime;
                    minimumGoodStatusCode = currentStatus;
                    minimumOriginalType = values[ii].WrappedValue.TypeInfo;
                    duplicatesMinimumsExist = false;
                    goodValueExists = true;
                }

                // check for duplicate minimums.
                else if (minimumGoodValue == currentValue)
                {
                    duplicatesMinimumsExist = true;
                }

                // check for new maximum.
                if (maximumGoodValue < currentValue)
                {
                    maximumGoodValue = currentValue;
                    maximumGoodTimestamp = currentTime;
                    maximumGoodStatusCode = currentStatus;
                    maximumOriginalType = values[ii].WrappedValue.TypeInfo;
                    duplicatesMaximumsExist = false;
                    goodValueExists = true;
                }

                // check for duplicate maximums.
                else if (maximumGoodValue == currentValue)
                {
                    duplicatesMaximumsExist = true;
                }
            }

            // check if at least one good value exists.
            if (!goodValueExists)
            {
                DataValue noDataValue = GetNoDataValue(slice);
                // check if interval is partial and set the flag accordingly
                if (slice.Partial)
                {
                    noDataValue.StatusCode = noDataValue.StatusCode.SetAggregateBits(AggregateBits.Partial);
                }
                return noDataValue;
            }

            // determine the calculated value to return.
            object processedValue = null;
            TypeInfo processedType = null;
            var processedTimestamp = DateTime.MinValue;
            StatusCode processedStatusCode = StatusCodes.Good;
            var duplicatesExist = false;

            if (valueType == 1)
            {
                processedValue = minimumGoodValue;
                processedTimestamp = minimumGoodTimestamp;
                processedStatusCode = minimumGoodStatusCode;
                processedType = minimumOriginalType;
                duplicatesExist = duplicatesMinimumsExist;
            }

            else if (valueType == 2)
            {
                processedValue = maximumGoodValue;
                processedTimestamp = maximumGoodTimestamp;
                processedStatusCode = maximumGoodStatusCode;
                processedType = maximumOriginalType;
                duplicatesExist = duplicatesMaximumsExist;
            }

            else if (valueType == 3)
            {
                processedValue = Math.Abs(maximumGoodValue - minimumGoodValue);
                processedType = TypeInfo.Scalars.Double;
            }

            // set the status code.
            var statusCode = processedStatusCode;

            // set calculated if not returning actual time and value is not at the start time.
            if (!returnActualTime && processedTimestamp != slice.StartTime && (statusCode.AggregateBits & AggregateBits.Interpolated) == 0)
            {
                statusCode = statusCode.SetAggregateBits(statusCode.AggregateBits | AggregateBits.Calculated);
            }

            // set the multiple values flags.
            if (duplicatesExist)
            {
                statusCode = statusCode.SetAggregateBits(statusCode.AggregateBits | AggregateBits.MultipleValues);
            }

            // convert back to original datatype.
            if (processedType != null && processedType.BuiltInType != BuiltInType.Double)
            {
                processedValue = TypeInfo.Cast(processedValue, TypeInfo.Scalars.Double, processedType.BuiltInType);
            }
            else
            {
                processedType = TypeInfo.Scalars.Double;
            }

            // create processed value.
            var value = new DataValue();
            value.WrappedValue = new Variant(processedValue, processedType);
            value.StatusCode = GetTimeBasedStatusCode(slice, values, statusCode);

            // zero value if status is bad.
            if (StatusCode.IsBad(value.StatusCode))
            {
                value.WrappedValue = Variant.Null;
            }

            if (returnActualTime)
            {
                // calculate effective time if end bound is used.
                if (TimeFlowsBackward)
                {
                    if (processedTimestamp == slice.StartTime)
                    {
                        processedTimestamp = processedTimestamp.AddMilliseconds(+1);
                        value.StatusCode = value.StatusCode.SetAggregateBits(value.StatusCode.AggregateBits | AggregateBits.Interpolated);
                    }
                }
                else
                {
                    if (processedTimestamp == slice.EndTime)
                    {
                        processedTimestamp = processedTimestamp.AddMilliseconds(-1);
                        value.StatusCode = value.StatusCode.SetAggregateBits(value.StatusCode.AggregateBits | AggregateBits.Interpolated);
                    }
                }

                value.SourceTimestamp = processedTimestamp;
                value.ServerTimestamp = processedTimestamp;
            }
            else
            {
                value.SourceTimestamp = GetTimestamp(slice);
                value.ServerTimestamp = GetTimestamp(slice);
            }

            return value;
        }
        #endregion
    }
}
