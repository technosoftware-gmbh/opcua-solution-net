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
    public class StdDevAggregateCalculator : AggregateCalculator
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
        public StdDevAggregateCalculator(
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
                    case Objects.AggregateFunction_StandardDeviationPopulation:
                    {
                        return ComputeStdDev(slice, false, 1);
                    }

                    case Objects.AggregateFunction_StandardDeviationSample:
                    {
                        return ComputeStdDev(slice, false, 2);
                    }

                    case Objects.AggregateFunction_VariancePopulation:
                    {
                        return ComputeStdDev(slice, true, 1);
                    }

                    case Objects.AggregateFunction_VarianceSample:
                    {
                        return ComputeStdDev(slice, true, 2);
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
        protected DataValue ComputeRegression(TimeSlice slice, int valueType)
        {
            // get the values in the slice.
            var values = GetValuesWithSimpleBounds(slice);

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // get the regions.
            var regions = GetRegionsInValueSet(values, false, true);

            var xData = new List<double>();
            var yData = new List<double>();

            double duration = 0;
            var nonGoodDataExists = false;

            for (var ii = 0; ii < regions.Count; ii++)
            {
                if (StatusCode.IsGood(regions[ii].StatusCode))
                {
                    xData.Add(regions[ii].StartValue);
                    yData.Add(duration);
                }
                else
                {
                    nonGoodDataExists = true;
                }

                // normalize to seconds.
                duration += regions[ii].Duration/1000.0;
            }

            // check if no good data.
            if (xData.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // compute the regression parameters.
            double regSlope = 0;
            double regConst = 0;
            double regStdDev = 0;

            if (xData.Count > 1)
            {
                double xAvg = 0;
                double yAvg = 0;
                double xxAgv = 0;
                double xyAvg = 0;

                for (var ii = 0; ii < xData.Count; ii++)
                {
                    xAvg += xData[ii];
                    yAvg += yData[ii];
                    xxAgv += xData[ii] * xData[ii];
                    xyAvg += xData[ii] * yData[ii];
                }

                xAvg /= xData.Count;
                yAvg /= xData.Count;
                xxAgv /= xData.Count;
                xyAvg /= xData.Count;

                regSlope = (xyAvg - xAvg * yAvg) / (xxAgv - xAvg * xAvg);
                regConst = yAvg - regSlope * xAvg;
                
                var errors = new List<double>();

                double eAvg = 0;

                for (var ii = 0; ii < xData.Count; ii++)
                {
                    var error = yData[ii] - regConst - regSlope * xData[ii];
                    errors.Add(error);
                    eAvg += error;
                }

                eAvg /= errors.Count;

                double variance = 0;

                for (var ii = 0; ii < errors.Count; ii++)
                {
                    var error = errors[ii] - eAvg;
                    variance += error * error;
                }

                variance /= errors.Count;
                regStdDev = Math.Sqrt(variance);
            }

            // select the result.
            double result = 0;

            switch (valueType)
            {
                case 1: { result = regSlope;  break; }
                case 2: { result = regConst;  break; }
                case 3: { result = regStdDev; break; }
            }
            
            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(result, TypeInfo.Scalars.Double);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);

            if (nonGoodDataExists)
            {
                value.StatusCode = StatusCodes.UncertainDataSubNormal;
            }

            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);

            // return result.
            return value;
        }

        /// <summary>
        /// Calculates the StdDev, Variance, StdDev2 and Variance2 aggregates for the timeslice.
        /// </summary>
        protected DataValue ComputeStdDev(TimeSlice slice, bool includeBounds, int valueType)
        {
            // get the values in the slice.
            List<DataValue> values;

            if (includeBounds)
            {
                values = GetValuesWithSimpleBounds(slice);
            }
            else
            {
                values = GetValues(slice);
            }

            // check for empty slice.
            if (values == null || values.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            // get the regions.
            var regions = GetRegionsInValueSet(values, false, true);

            var xData = new List<double>();
            double average = 0;
            var nonGoodDataExists = false;

            for (var ii = 0; ii < regions.Count; ii++)
            {
                if (StatusCode.IsGood(regions[ii].StatusCode))
                {
                    xData.Add(regions[ii].StartValue);
                    average += regions[ii].StartValue;
                }
                else
                {
                    nonGoodDataExists = true;
                }
            }

            // check if no good data.
            if (xData.Count == 0)
            {
                return GetNoDataValue(slice);
            }

            average /= xData.Count;

            // calculate variance.
            double variance = 0;

            for (var ii = 0; ii < xData.Count; ii++)
            {
                var error = xData[ii] - average;
                variance += error*error;
            }

            // use the sample variance if bounds are included.
            if (includeBounds)
            {
                variance /= (xData.Count + 1);
            }
            
            // use the population variance if bounds are not included.
            else
            {
                variance /= xData.Count;
            }

            // select the result.
            double result = 0;

            switch (valueType)
            {
                case 1: { result = Math.Sqrt(variance); break; }
                case 2: { result = variance; break; }
            }

            // set the timestamp and status.
            var value = new DataValue();
            value.WrappedValue = new Variant(result, TypeInfo.Scalars.Double);
            value.SourceTimestamp = GetTimestamp(slice);
            value.ServerTimestamp = GetTimestamp(slice);

            if (nonGoodDataExists)
            {
                value.StatusCode = StatusCodes.UncertainDataSubNormal;
            }

            value.StatusCode = value.StatusCode.SetAggregateBits(AggregateBits.Calculated);

            // return result.
            return value;
        }
        #endregion
    }
}
