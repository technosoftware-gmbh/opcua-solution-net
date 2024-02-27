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
    public class StartEndAggregateCalculator : AggregateCalculator
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
        public StartEndAggregateCalculator(
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
                    case Objects.AggregateFunction_Start:
                    {
                        return ComputeStartEnd(slice, false);
                    }

                    case Objects.AggregateFunction_End:
                    {
                        return ComputeStartEnd(slice, true);
                    }

                    case Objects.AggregateFunction_Delta:
                    {
                        return ComputeDelta(slice);
                    }

                    case Objects.AggregateFunction_StartBound:
                    {
                        return ComputeStartEnd2(slice, false);
                    }

                    case Objects.AggregateFunction_EndBound:
                    {
                        return ComputeStartEnd2(slice, true);
                    }

                    case Objects.AggregateFunction_DeltaBounds:
                    {
                        return ComputeDelta2(slice);
                    }
                }
            }

            return base.ComputeValue(slice);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Calculate the Start and End aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeStartEnd(TimeSlice slice, bool returnEnd)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // return start value.
            if (!returnEnd)
            {
                return values[0];
            }

            // return end value.
            else
            {
                return values[values.Count - 1];
            }
        }

        /// <summary>
        /// Calculates the Delta aggregate for the timeslice.
        /// </summary>
        protected DataValue ComputeDelta(TimeSlice slice)
        {
            // get the values in the slice.
            var values = GetValues(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // find start value.
            DataValue start = null;
            double startValue = 0;
            TypeInfo originalType = null;
            var badDataSkipped = false;

            for (var ii = 0; ii < values.Count; ii++)
            {
                start = values[ii];

                if (IsGood(start))
                {
                    try
                    {
                        startValue = CastToDouble(start);
                        originalType = start.WrappedValue.TypeInfo;
                        break;
                    }
                    catch (Exception)
                    {
                        startValue = Double.NaN;
                    }
                }

                start = null;
                badDataSkipped = true;
            }

            // find end value.
            DataValue end = null;
            double endValue = 0;

            for (var ii = values.Count - 1; ii >= 0; ii--)
            {
                end = values[ii];

                if (IsGood(end))    
                {
                    try
                    {
                        endValue = CastToDouble(end);
                        break;
                    }
                    catch (Exception)
                    {
                        endValue = Double.NaN;
                    }

                    break;
                }

                end = null;
                badDataSkipped = true;
            }

            // check if no good data.
            if (Double.IsNaN(startValue) || Double.IsNaN(endValue))
            {
                return GetNoDataValue(slice);
            }
            
            var value = new DataValue();
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);

            // set status code.
            if (badDataSkipped)
            {
                value.StatusCode = StatusCodes.UncertainDataSubNormal;
            }
            
            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);
            
            // calculate delta.
            var delta = endValue - startValue;

            if (originalType != null && originalType.BuiltInType != BuiltInType.Double)
            {
                var delta2 = TypeInfo.Cast(delta, TypeInfo.Scalars.Double, originalType.BuiltInType);
                value.WrappedValue = new Variant(delta2, originalType);
            }
            else
            {
                value.WrappedValue = new Variant(delta, TypeInfo.Scalars.Double);
            }

            // return result.
            return value;
        }

        /// <summary>
        /// Calculate the Start2 and End2 aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeStartEnd2(TimeSlice slice, bool returnEnd)
        {
            // get the values in the slice.
            var values = GetValuesWithSimpleBounds(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            DataValue value = null;

            // return start bound.
            if ((!returnEnd && !TimeFlowsBackward) || (returnEnd && TimeFlowsBackward))
            {
                value = values[0];
            }

            // return end bound.
            else
            {
                value = values[values.Count - 1];
            }

            if (!IsGood(value))
            {
                value.StatusCode = StatusCodes.BadNoData;
            }

            if (returnEnd)
            {
                value.SourceTimestamp = GetTimestamp(slice);
                value.ServerTimestamp = GetTimestamp(slice);

                if (StatusCode.IsNotBad(value.StatusCode))
                {
                    value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);
                }
            }

            return value;
        }

        /// <summary>
        /// Calculates the Delta2 aggregate for the timeslice.
        /// </summary>
        protected DataValue ComputeDelta2(TimeSlice slice)
        {
            // get the values in the slice.
            var values = GetValuesWithSimpleBounds(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            var start = values[0];
            var end = values[values.Count-1];

            // check for bad bounds.
            if (!IsGood(start) || !IsGood(end))
            {
                return GetNoDataValue(slice);
            }

            // convert to doubles.
            double startValue = 0;
            TypeInfo originalType = null;

            try
            {
                startValue = CastToDouble(start);
                originalType = start.WrappedValue.TypeInfo;
            }
            catch (Exception)
            {
                startValue = Double.NaN;
            }

            double endValue = 0;

            try
            {
                endValue = CastToDouble(end);
            }
            catch (Exception)
            {
                endValue = Double.NaN;
            }

            // check for bad bounds.
            if (Double.IsNaN(startValue) || Double.IsNaN(endValue))
            {
                return GetNoDataValue(slice);
            }

            var value = new DataValue();
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);

            if (!IsGood(start) || !IsGood(end))
            {
                value.StatusCode = StatusCodes.UncertainDataSubNormal;
            }

            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);

            // calculate delta.
            var delta = endValue - startValue;

            if (originalType != null && originalType.BuiltInType != BuiltInType.Double)
            {
                var delta2 = TypeInfo.Cast(delta, TypeInfo.Scalars.Double, originalType.BuiltInType);
                value.WrappedValue = new Variant(delta2, originalType);
            }
            else
            {
                value.WrappedValue = new Variant(delta, TypeInfo.Scalars.Double);
            }

            // return result.
            return value;
        }
        #endregion
    }
}
