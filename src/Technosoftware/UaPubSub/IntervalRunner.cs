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
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// component that is specialized in calculating and executing a routine for a given interval
    /// </summary>
    public class IntervalRunner : IDisposable
    {
        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="IntervalRunner"/>.
        /// </summary>
        public IntervalRunner(object id, double interval, Func<bool> canExecuteFunc, Action intervalAction)
        {
            Id = id;
            Interval = interval;
            CanExecuteFunc = canExecuteFunc;
            IntervalAction = intervalAction;
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="UaPublisher"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  When overridden in a derived class, releases the unmanaged resources used by that class 
        ///  and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"> true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();

                if (cancellationToken_ != null)
                {
                    cancellationToken_.Dispose();
                    cancellationToken_ = null;
                }
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Identifier of current IntervalRunner
        /// </summary>
        public object Id { get; private set; }

        /// <summary>
        /// Get/set the Interval between Runs
        /// </summary>
        public double Interval
        {
            get { return interval_; }
            set
            {
                lock (lock_)
                {
                    if (value < MinInterval_)
                    {
                        value = MinInterval_;
                    }

                    interval_ = value;
                }
            }
        }

        /// <summary>
        /// Get the function that decides if the configured action can be executed when the Interval elapses
        /// </summary>
        public Func<bool> CanExecuteFunc { get; private set; }

        /// <summary>
        /// Get the action that will be executed at each interval
        /// </summary>
        public Action IntervalAction { get; private set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the IntervalRunner and makes it ready to execute the code.
        /// </summary>
        public void Start()
        {
            Task.Run(ProcessAsync).ConfigureAwait(false);
            Utils.Trace("IntervalRunner with id: {0} was started.", Id);
        }

        /// <summary>
        /// Stop the publishing thread.
        /// </summary>
        public virtual void Stop()
        {
            lock (lock_)
            {
                cancellationToken_?.Cancel();
            }

            Utils.Trace("IntervalRunner with id: {0} was stopped.", Id);
        }
        #endregion

        /// <summary>
        /// Periodically executes the .
        /// </summary>
        private async Task ProcessAsync()
        {
            do
            {
                int sleepCycle = 0;
                DateTime now = DateTime.UtcNow;
                DateTime nextPublishTime = DateTime.MinValue;

                lock (lock_)
                {
                    sleepCycle = Convert.ToInt32(interval_);

                    nextPublishTime = nextPublishTime_;
                }

                if (nextPublishTime > now)
                {
                    sleepCycle = (int)Math.Min((nextPublishTime - now).TotalMilliseconds, sleepCycle);
                    sleepCycle = (int)Math.Max(MinInterval_, sleepCycle);
                    await Task.Delay(TimeSpan.FromMilliseconds(sleepCycle), cancellationToken_.Token).ConfigureAwait(false);
                }

                lock (lock_)
                {
                    var nextCycle = Convert.ToInt32(interval_);
                    nextPublishTime_ = DateTime.UtcNow.AddMilliseconds(nextCycle);

                    if (IntervalAction != null && CanExecuteFunc != null && CanExecuteFunc())
                    {
                        // call on a new thread
                        Task.Run(() => {
                            IntervalAction();
                        });
                    }
                }
            }
            while (true);
        }

        #region Private Fields
        private const int MinInterval_ = 10;
        private readonly object lock_ = new object();

        private double interval_ = MinInterval_;
        private DateTime nextPublishTime_ = DateTime.MinValue;
        // event used to cancel run
        private CancellationTokenSource cancellationToken_ = new CancellationTokenSource();
        #endregion
    }
}
