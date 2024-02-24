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
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Attempts to reconnect to the server.
    /// </summary>
    public class SessionReconnectHandler : IDisposable
    {
        /// <summary>
        /// The minimum reconnect period in ms.
        /// </summary>
        public const int MinReconnectPeriod = 500;

        /// <summary>
        /// The maximum reconnect period in ms.
        /// </summary>
        public const int MaxReconnectPeriod = 30000;

        /// <summary>
        /// The default reconnect period in ms.
        /// </summary>
        public const int DefaultReconnectPeriod = 1000;

        /// <summary>
        /// The default reconnect operation timeout in ms.
        /// </summary>
        public const int MinReconnectOperationTimeout = 5000;

        /// <summary>
        /// The internal state of the reconnect handler.
        /// </summary>
        public enum ReconnectState
        {
            /// <summary>
            /// The reconnect handler is ready to start the reconnect timer.
            /// </summary>
            Ready = 0,

            /// <summary>
            /// The reconnect timer is triggered and waiting to reconnect.
            /// </summary>
            Triggered = 1,

            /// <summary>
            /// The reconnection is in progress.
            /// </summary>
            Reconnecting = 2,

            /// <summary>
            /// The reconnect handler is disposed and can not be used for further reconnect attempts.
            /// </summary>
            Disposed = 4
        };

        /// <summary>
        /// Create a reconnect handler.
        /// </summary>
        /// <param name="reconnectAbort">Set to <c>true</c> to allow reconnect abort if keep alive recovered.</param>
        /// <param name="maxReconnectPeriod">
        ///     The upper limit for the reconnect period after exponential backoff.
        ///     -1 (default) indicates that no exponential backoff should be used.
        /// </param>
        public SessionReconnectHandler(bool reconnectAbort = false, int maxReconnectPeriod = -1)
        {
            reconnectAbort_ = reconnectAbort;
            reconnectTimer_ = new Timer(OnReconnectAsync, this, Timeout.Infinite, Timeout.Infinite);
            state_ = ReconnectState.Ready;
            cancelReconnect_ = false;
            updateFromServer_ = false;
            baseReconnectPeriod_ = DefaultReconnectPeriod;
            maxReconnectPeriod_ = maxReconnectPeriod < 0 ? -1 :
                Math.Max(MinReconnectPeriod, Math.Min(maxReconnectPeriod, MaxReconnectPeriod));
            random_ = new Random();
        }

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (lock_)
                {
                    if (reconnectTimer_ != null)
                    {
                        reconnectTimer_.Dispose();
                        reconnectTimer_ = null;
                    }
                    state_ = ReconnectState.Disposed;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the session managed by the handler.
        /// </summary>
        /// <value>The session.</value>
        public IUaSession Session { get; private set; }

        /// <summary>
        /// The internal state of the reconnect handler.
        /// </summary>
        public ReconnectState State
        {
            get
            {
                lock (lock_)
                {
                    return reconnectTimer_ == null ? ReconnectState.Disposed : state_;
                }
            }
        }

        /// <summary>
        /// Cancel a reconnect in progress.
        /// </summary>
        public void CancelReconnect()
        {
            lock (lock_)
            {
                if (reconnectTimer_ == null)
                {
                    return;
                }

                if (state_ == ReconnectState.Triggered)
                {
                    Session = null;
                    EnterReadyState();
                    return;
                }

                cancelReconnect_ = true;
            }
        }

        /// <summary>
        /// Begins the reconnect process.
        /// </summary>
        public ReconnectState BeginReconnect(IUaSession session, int reconnectPeriod, EventHandler callback)
        {
            return BeginReconnect(session, null, reconnectPeriod, callback);
        }

        /// <summary>
        /// Begins the reconnect process using a reverse connection.
        /// </summary>
        public ReconnectState BeginReconnect(IUaSession session, ReverseConnectManager reverseConnectManager, int reconnectPeriod, EventHandler callback)
        {
            lock (lock_)
            {
                if (reconnectTimer_ == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidState);
                }

                // cancel reconnect requested, if possible
                if (session == null)
                {
                    if (state_ == ReconnectState.Triggered)
                    {
                        Session = null;
                        EnterReadyState();
                        return state_;
                    }
                    // reconnect already in progress, schedule cancel
                    cancelReconnect_ = true;
                    return state_;
                }

                // set reconnect period within boundaries
                reconnectPeriod = CheckedReconnectPeriod(reconnectPeriod);

                // ignore subsequent trigger requests
                if (state_ == ReconnectState.Ready)
                {
                    Session = session;
                    baseReconnectPeriod_ = reconnectPeriod;
                    reconnectFailed_ = false;
                    cancelReconnect_ = false;
                    callback_ = callback;
                    reverseConnectManager_ = reverseConnectManager;
                    _ = reconnectTimer_.Change(JitteredReconnectPeriod(reconnectPeriod), Timeout.Infinite);
                    reconnectPeriod_ = CheckedReconnectPeriod(reconnectPeriod, true);
                    state_ = ReconnectState.Triggered;
                    return state_;
                }

                // if triggered, reset timer only if requested reconnect period is shorter
                if (state_ == ReconnectState.Triggered && reconnectPeriod < baseReconnectPeriod_)
                {
                    baseReconnectPeriod_ = reconnectPeriod;
                    _ = reconnectTimer_.Change(JitteredReconnectPeriod(reconnectPeriod), Timeout.Infinite);
                    reconnectPeriod_ = CheckedReconnectPeriod(reconnectPeriod, true);
                }

                return state_;
            }
        }

        /// <summary>
        /// Returns the reconnect period with a random jitter.
        /// </summary>
        public virtual int JitteredReconnectPeriod(int reconnectPeriod)
        {
            // The factors result in a jitter of 10%.
            const int JitterResolution = 1000;
            const int JitterFactor = 10;
            var jitter = (reconnectPeriod * random_.Next(-JitterResolution, JitterResolution)) /
                (JitterResolution * JitterFactor);
            return reconnectPeriod + jitter;
        }

        /// <summary>
        /// Returns the reconnect period within the min and max boundaries.
        /// </summary>
        public virtual int CheckedReconnectPeriod(int reconnectPeriod, bool exponentialBackoff = false)
        {
            // exponential backoff is controlled by m_maxReconnectPeriod
            if (maxReconnectPeriod_ > MinReconnectPeriod)
            {
                if (exponentialBackoff)
                {
                    reconnectPeriod *= 2;
                }
                return Math.Min(Math.Max(reconnectPeriod, MinReconnectPeriod), maxReconnectPeriod_);
            }
            else
            {
                return Math.Max(reconnectPeriod, MinReconnectPeriod);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called when the reconnect timer expires.
        /// </summary>
        private async void OnReconnectAsync(object state)
        {
            DateTime reconnectStart = DateTime.UtcNow;
            try
            {
                // check for exit.
                lock (lock_)
                {
                    if (reconnectTimer_ == null || Session == null)
                    {
                        return;
                    }
                    if (state_ != ReconnectState.Triggered)
                    {
                        return;
                    }
                    // enter reconnecting state
                    state_ = ReconnectState.Reconnecting;
                }

                var keepaliveRecovered = false;

                // preserve legacy behavior if reconnectAbort is not set
                if (Session != null && reconnectAbort_ &&
                    Session.Connected && !Session.KeepAliveStopped)
                {
                    keepaliveRecovered = true;
                    // breaking change, the callback must only assign the new
                    // session if the property is != null
                    Session = null;
                    Utils.LogInfo("Reconnect {0} aborted, KeepAlive recovered.", Session?.SessionId);
                }
                else
                {
                    Utils.LogInfo("Reconnect {0}.", Session?.SessionId);
                }

                // do the reconnect or recover state.
                if (keepaliveRecovered ||
                    await DoReconnectAsync().ConfigureAwait(false))
                {
                    lock (lock_)
                    {
                        EnterReadyState();
                    }

                    // notify the caller.
                    callback_(this, null);

                    return;
                }
            }
            catch (Exception exception)
            {
                Utils.LogError(exception, "Unexpected error during reconnect.");
            }

            // schedule the next reconnect.
            lock (lock_)
            {
                if (state_ != ReconnectState.Disposed)
                {
                    if (cancelReconnect_)
                    {
                        EnterReadyState();
                    }
                    else
                    {
                        var elapsed = (int)DateTime.UtcNow.Subtract(reconnectStart).TotalMilliseconds;
                        Utils.LogInfo("Reconnect period is {0} ms, {1} ms elapsed in reconnect.", reconnectPeriod_, elapsed);
                        var adjustedReconnectPeriod = CheckedReconnectPeriod(reconnectPeriod_ - elapsed);
                        adjustedReconnectPeriod = JitteredReconnectPeriod(adjustedReconnectPeriod);
                        _ = reconnectTimer_.Change(adjustedReconnectPeriod, Timeout.Infinite);
                        Utils.LogInfo("Next adjusted reconnect scheduled in {0} ms.", adjustedReconnectPeriod);
                        reconnectPeriod_ = CheckedReconnectPeriod(reconnectPeriod_, true);
                        state_ = ReconnectState.Triggered;
                    }
                }
            }
        }

        /// <summary>
        /// Reconnects to the server.
        /// </summary>
        private async Task<bool> DoReconnectAsync()
        {
            // helper to override operation timeout
            var operationTimeout = Session.OperationTimeout;
            var reconnectOperationTimeout = Math.Max(reconnectPeriod_, MinReconnectOperationTimeout);

            // try a reconnect.
            if (!reconnectFailed_)
            {
                try
                {
                    Session.OperationTimeout = reconnectOperationTimeout;
                    if (reverseConnectManager_ != null)
                    {
                        ITransportWaitingConnection connection = await reverseConnectManager_.WaitForConnectionAsync(
                                new Uri(Session.Endpoint.EndpointUrl),
                                Session.Endpoint.Server.ApplicationUri
                            ).ConfigureAwait(false);

                        await Session.ReconnectAsync(connection).ConfigureAwait(false);
                    }
                    else
                    {
                        await Session.ReconnectAsync().ConfigureAwait(false);
                    }

                    // monitored items should start updating on their own.
                    return true;
                }
                catch (Exception exception)
                {
                    // recreate the session if it has been closed.
                    if (exception is ServiceResultException sre)
                    {
                        Utils.LogWarning("Reconnect failed. Reason={0}.", sre.Result);

                        // check if the server endpoint could not be reached.
                        if (sre.StatusCode == StatusCodes.BadTcpInternalError ||
                            sre.StatusCode == StatusCodes.BadCommunicationError ||
                            sre.StatusCode == StatusCodes.BadNotConnected ||
                            sre.StatusCode == StatusCodes.BadRequestTimeout ||
                            sre.StatusCode == StatusCodes.BadTimeout)
                        {
                            // check if reactivating is still an option.
                            TimeSpan timeout = Session.LastKeepAliveTime.AddMilliseconds(Session.SessionTimeout) - DateTime.UtcNow;

                            if (timeout.TotalMilliseconds > 0)
                            {
                                Utils.LogInfo("Retry to reactivate, est. session timeout in {0} ms.", timeout.TotalMilliseconds);
                                return false;
                            }
                        }

                        // check if the security configuration may have changed
                        if (sre.StatusCode == StatusCodes.BadSecurityChecksFailed ||
                            sre.StatusCode == StatusCodes.BadCertificateInvalid)
                        {
                            updateFromServer_ = true;
                            Utils.LogInfo("Reconnect failed due to security check. Request endpoint update from server. {0}", sre.Message);
                        }
                        // wait for next scheduled reconnect if connection failed,
                        // otherwise recreate session immediately
                        else if (sre.StatusCode != StatusCodes.BadSessionIdInvalid)
                        {
                            // next attempt is to recreate session
                            reconnectFailed_ = true;
                            return false;
                        }
                    }
                    else
                    {
                        Utils.LogError(exception, "Reconnect failed.");
                    }

                    reconnectFailed_ = true;
                }
                finally
                {
                    Session.OperationTimeout = operationTimeout;
                }
            }

            // re-create the session.
            try
            {
                IUaSession session;
                Session.OperationTimeout = reconnectOperationTimeout;
                if (reverseConnectManager_ != null)
                {
                    ITransportWaitingConnection connection;
                    do
                    {
                        connection = await reverseConnectManager_.WaitForConnectionAsync(
                                new Uri(Session.Endpoint.EndpointUrl),
                                Session.Endpoint.Server.ApplicationUri
                            ).ConfigureAwait(false);

                        if (updateFromServer_)
                        {
                            ConfiguredEndpoint endpoint = Session.ConfiguredEndpoint;
                            await endpoint.UpdateFromServerAsync(
                                endpoint.EndpointUrl, connection,
                                endpoint.Description.SecurityMode,
                                endpoint.Description.SecurityPolicyUri).ConfigureAwait(false);
                            updateFromServer_ = false;
                            connection = null;
                        }
                    } while (connection == null);

                    session = await Session.SessionFactory.RecreateAsync(Session, connection).ConfigureAwait(false);
                }
                else
                {
                    if (updateFromServer_)
                    {
                        ConfiguredEndpoint endpoint = Session.ConfiguredEndpoint;
                        await endpoint.UpdateFromServerAsync(
                            endpoint.EndpointUrl,
                            endpoint.Description.SecurityMode,
                            endpoint.Description.SecurityPolicyUri).ConfigureAwait(false);
                        updateFromServer_ = false;
                    }

                    session = await Session.SessionFactory.RecreateAsync(Session).ConfigureAwait(false);
                }
                // note: the template session is not connected at this point
                //       and must be disposed by the owner
                Session = session;
                return true;
            }
            catch (ServiceResultException sre)
            {
                if (sre.InnerResult?.StatusCode == StatusCodes.BadSecurityChecksFailed ||
                    sre.InnerResult?.StatusCode == StatusCodes.BadCertificateInvalid)
                {
                    // schedule endpoint update and retry
                    updateFromServer_ = true;
                    if (maxReconnectPeriod_ > MinReconnectPeriod &&
                        reconnectPeriod_ >= maxReconnectPeriod_)
                    {
                        reconnectPeriod_ = baseReconnectPeriod_;
                    }
                    Utils.LogError("Could not reconnect due to failed security check. Request endpoint update from server. {0}", sre.Message);
                }
                else
                {
                    Utils.LogError("Could not reconnect the Session. {0}", sre.Message);
                }
                return false;
            }
            catch (Exception exception)
            {
                Utils.LogError("Could not reconnect the Session. {0}", exception.Message);
                return false;
            }
            finally
            {
                Session.OperationTimeout = operationTimeout;
            }
        }

        /// <summary>
        /// Reset the timer and enter ready state. 
        /// </summary>
        private void EnterReadyState()
        {
            _ = reconnectTimer_.Change(Timeout.Infinite, Timeout.Infinite);
            state_ = ReconnectState.Ready;
            cancelReconnect_ = false;
            updateFromServer_ = false;
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private ReconnectState state_;
        private Random random_;
        private bool reconnectFailed_;
        private bool reconnectAbort_;
        private bool cancelReconnect_;
        private bool updateFromServer_;
        private int reconnectPeriod_;
        private int baseReconnectPeriod_;
        private int maxReconnectPeriod_;
        private Timer reconnectTimer_;
        private EventHandler callback_;
        private ReverseConnectManager reverseConnectManager_;
        #endregion
    }
}
