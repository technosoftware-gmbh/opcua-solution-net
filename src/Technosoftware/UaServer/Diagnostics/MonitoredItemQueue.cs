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

namespace Technosoftware.UaServer.Diagnostics
{
    /// <summary>
    /// Provides a queue for data changes.
    /// </summary>
    public class MonitoredItemQueue
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates an empty queue.
        /// </summary>
        public MonitoredItemQueue(uint monitoredItemId, DiscardedValueHandler discardedValueHandler = null)
        {
            monitoredItemId_ = monitoredItemId;
            values_ = null;
            errors_ = null;
            start_ = -1;
            end_ = -1;
            overflow_ = -1;
            discardOldest_ = false;
            nextSampleTime_ = 0;
            samplingInterval_ = 0;
            discardedValueHandler_ = discardedValueHandler;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The delegate for handling discarded values.
        /// </summary>
        public delegate void DiscardedValueHandler();

        /// <summary>
        /// Gets the current queue size.
        /// </summary>
        public uint QueueSize
        {
            get
            {
                if (values_ == null)
                {
                    return 0;
                }

                return (uint)values_.Length;
            }
        }

        /// <summary>
        /// Gets number of elements actually contained in value queue.
        /// </summary>
        public int ItemsInQueue
        {
            get
            {
                if (values_ == null)
                {
                    return 0;
                }

                if (start_ < end_)
                {
                    return end_ - start_ - 1;
                }

                return values_.Length - start_ + end_ - 1;
            }
        }

        /// <summary>
        /// Sets the sampling interval used when queuing values.
        /// </summary>
        /// <param name="samplingInterval">The new sampling interval.</param>
        public void SetSamplingInterval(double samplingInterval)
        {
            // substract the previous sampling interval.
            if (samplingInterval_ < nextSampleTime_)
            {
                nextSampleTime_ -= samplingInterval_;
            }

            // calculate the next sampling interval.
            samplingInterval_ = (long)(samplingInterval);

            if (samplingInterval_ > 0)
            {
                nextSampleTime_ += samplingInterval_;
            }
            else
            {
                nextSampleTime_ = 0;
            }
        }

        /// <summary>
        /// Sets the queue size.
        /// </summary>
        /// <param name="queueSize">The new queue size.</param>
        /// <param name="discardOldest">Whether to discard the oldest values if the queue overflows.</param>
        /// <param name="diagnosticsMasks">Specifies which diagnostics which should be kept in the queue.</param>
        public void SetQueueSize(uint queueSize, bool discardOldest, DiagnosticsMasks diagnosticsMasks)
        {
            var length = (int)queueSize;

            if (length < 1)
            {
                length = 1;
            }

            var start = start_;
            var end = end_;

            // create new queue.
            var values = new DataValue[length];
            ServiceResult[] errors = null;

            if ((diagnosticsMasks & DiagnosticsMasks.OperationAll) != 0)
            {
                errors = new ServiceResult[length];
            }

            // copy existing values.
            List<DataValue> existingValues = null;
            List<ServiceResult> existingErrors = null;

            if (start_ >= 0)
            {
                existingValues = new List<DataValue>();
                existingErrors = new List<ServiceResult>();

                DataValue value = null;
                ServiceResult error = null;

                while (Dequeue(out value, out error))
                {
                    existingValues.Add(value);
                    existingErrors.Add(error);
                }
            }

            // update internals.
            values_ = values;
            errors_ = errors;
            start_ = -1;
            end_ = 0;
            overflow_ = -1;
            discardOldest_ = discardOldest;

            // requeue the data.
            if (existingValues != null)
            {
                for (var ii = 0; ii < existingValues.Count; ii++)
                {
                    Enqueue(existingValues[ii], existingErrors[ii]);
                }
            }
        }

        /// <summary>
        /// Adds the value to the queue.
        /// </summary>
        /// <param name="value">The value to queue.</param>
        /// <param name="error">The error to queue.</param>
        public void QueueValue(DataValue value, ServiceResult error)
        {
            long now = HiResClock.TickCount64;

            if (start_ >= 0)
            {
                // check if too soon for another sample.
                if (now < nextSampleTime_)
                {
                    var last = end_ - 1;

                    if (last < 0)
                    {
                        last = values_.Length - 1;
                    }

                    // replace last value and error.
                    values_[last] = value;

                    if (errors_ != null)
                    {
                        errors_[last] = error;
                    }

                    discardedValueHandler_?.Invoke();

                    return;
                }
            }

            // update next sample time.
            if (nextSampleTime_ > 0)
            {
                var delta = now - nextSampleTime_;

                if (samplingInterval_ > 0 && delta >= 0)
                {
                    nextSampleTime_ += ((delta / samplingInterval_) + 1) * samplingInterval_;
                }
            }
            else
            {
                nextSampleTime_ = now + samplingInterval_;
            }

            // queue next value.
            Enqueue(value, error);
        }

        /// <summary>
        /// Publishes the oldest value in the queue.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="error">The error associated with the value.</param>
        /// <returns>True if a value was found. False if the queue is empty.</returns>
        public bool Publish(out DataValue value, out ServiceResult error)
        {
            return Dequeue(out value, out error);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds the value to the queue. Discards values if the queue is full.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="error">The error to add.</param>
        private void Enqueue(DataValue value, ServiceResult error)
        {
            // check for empty queue.
            if (start_ < 0)
            {
                start_ = 0;
                end_ = 1;
                overflow_ = -1;

                UaServerUtils.EventLog.EnqueueValue(value.WrappedValue);

                values_[start_] = value;

                if (errors_ != null)
                {
                    errors_[start_] = error;
                }

                return;
            }

            var next = end_;

            // check if the latest value has initial dummy data
            if (values_[end_ - 1].StatusCode != StatusCodes.BadWaitingForInitialData)
            {
                // check for wrap around.
                if (next >= values_.Length)
                {
                    next = 0;
                }

                // check if queue is full.
                if (start_ == next)
                {
                    discardedValueHandler_?.Invoke();

                    if (!discardOldest_)
                    {
                        overflow_ = end_ - 1;
                        UaServerUtils.ReportDiscardedValue(null, monitoredItemId_, value);

                        // overwrite last value
                        values_[overflow_] = value;

                        if (errors_ != null)
                        {
                            errors_[overflow_] = error;
                        }

                        return;
                    }

                    // remove oldest value.
                    start_++;

                    if (start_ >= values_.Length)
                    {
                        start_ = 0;
                    }

                    // set overflow bit.
                    overflow_ = start_;
                    UaServerUtils.ReportDiscardedValue(null, monitoredItemId_, values_[overflow_]);
                }
                else
                {
                    Utils.TraceDebug("ENQUEUE VALUE: Value={0}", value.WrappedValue);
                }
            }
            else
            {
                // overwrite the last value
                next = end_ - 1;
            }

            // add value.
            values_[next] = value;

            if (errors_ != null)
            {
                errors_[next] = error;
            }

            end_ = next + 1;
        }

        /// <summary>
        /// Removes a value and an error from the queue.
        /// </summary>
        /// <param name="value">The value removed from the queue.</param>
        /// <param name="error">The error removed from the queue.</param>
        /// <returns>True if a value was found. False if the queue is empty.</returns>
        private bool Dequeue(out DataValue value, out ServiceResult error)
        {
            value = null;
            error = null;

            // check for empty queue.
            if (start_ < 0)
            {
                return false;
            }

            value = values_[start_];
            values_[start_] = null;

            if (errors_ != null)
            {
                error = errors_[start_];
                errors_[start_] = null;
            }

            // set the overflow bit.
            if (overflow_ == start_)
            {
                SetOverflowBit(ref value, ref error);
                overflow_ = -1;
            }

            start_++;

            // check if queue has been emptied.
            if (start_ == end_)
            {
                start_ = -1;
                end_ = 0;
            }

            // check for wrap around.
            else if (start_ >= values_.Length)
            {
                start_ = 0;
            }

            UaServerUtils.EventLog.DequeueValue(value.WrappedValue, value.StatusCode);

            return true;
        }

        /// <summary>
        /// Sets the overflow bit in the value and error.
        /// </summary>
        /// <param name="value">The value to update.</param>
        /// <param name="error">The error to update.</param>
        private void SetOverflowBit(ref DataValue value, ref ServiceResult error)
        {
            if (value != null)
            {
                var status = value.StatusCode;
                status.Overflow = true;
                value.StatusCode = status;
            }

            if (error != null)
            {
                var status = error.StatusCode;
                status.Overflow = true;

                // have to copy before updating because the ServiceResult is invariant.
                var copy = new ServiceResult(
                    status,
                    error.SymbolicId,
                    error.NamespaceUri,
                    error.LocalizedText,
                    error.AdditionalInfo,
                    error.InnerResult);

                error = copy;
            }
        }
        #endregion

        #region Private Fields
        private uint monitoredItemId_;
        private DataValue[] values_;
        private ServiceResult[] errors_;
        private int start_;
        private int end_;
        private int overflow_;
        private bool discardOldest_;
        private long nextSampleTime_;
        private long samplingInterval_;
        DiscardedValueHandler discardedValueHandler_;
        #endregion
    }
}
