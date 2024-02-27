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
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Threading.Tasks;

using Opc.Ua;
using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer.Sessions
{
    /// <summary>
    /// A generic session manager object for a server.
    /// </summary>
    public class SessionManager : IUaSessionManager, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the manager with its configuration.
        /// </summary>
        public SessionManager(
            IUaServerData server,
            ApplicationConfiguration configuration)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            server_ = server;

            minSessionTimeout_ = configuration.ServerConfiguration.MinSessionTimeout;
            maxSessionTimeout_ = configuration.ServerConfiguration.MaxSessionTimeout;
            maxSessionCount_ = configuration.ServerConfiguration.MaxSessionCount;
            maxRequestAge_ = configuration.ServerConfiguration.MaxRequestAge;
            maxBrowseContinuationPoints_ = configuration.ServerConfiguration.MaxBrowseContinuationPoints;
            maxHistoryContinuationPoints_ = configuration.ServerConfiguration.MaxHistoryContinuationPoints;
            minNonceLength_ = configuration.SecurityConfiguration.NonceLength;

            sessions_ = new Dictionary<NodeId, Session>();
            lastSessionId_ = BitConverter.ToInt64(Utils.Nonce.CreateNonce(sizeof(long)), 0);

            // create a event to signal shutdown.
            shutdownEvent_ = new ManualResetEvent(true);
        }
        #endregion

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
                List<Session> sessions = null;

                lock (lock_)
                {
                    sessions = new List<Session>(sessions_.Values);
                    sessions_.Clear();
                }

                foreach (var session in sessions)
                {
                    Utils.SilentDispose(session);
                }

                shutdownEvent_.Set();
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Starts the session manager.
        /// </summary>
        public virtual void Startup()
        {
            lock (lock_)
            {
                // start thread to monitor sessions.
                shutdownEvent_.Reset();

                Task.Factory.StartNew(() => {
                    MonitorSessions(minSessionTimeout_);
                }, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
            }
        }

        /// <summary>
        /// Stops the session manager and closes all sessions.
        /// </summary>
        public virtual void Shutdown()
        {
            lock (lock_)
            {
                // stop the monitoring thread.
                shutdownEvent_.Set();

                // dispose of session objects.
                foreach (var session in sessions_.Values)
                {
                    session.Dispose();
                }

                sessions_.Clear();
            }
        }

        /// <summary>
        /// Creates a new session.
        /// </summary>
        public virtual Session CreateSession(
            UaServerOperationContext context,
            X509Certificate2 serverCertificate,
            string sessionName,
            byte[] clientNonce,
            ApplicationDescription clientDescription,
            string endpointUrl,
            X509Certificate2 clientCertificate,
            double requestedSessionTimeout,
            uint maxResponseMessageSize,
            out NodeId sessionId,
            out NodeId authenticationToken,
            out byte[] serverNonce,
            out double revisedSessionTimeout)
        {
            sessionId = 0;
            revisedSessionTimeout = requestedSessionTimeout;

            Session session = null;

            lock (lock_)
            {
                // check session count.
                if (maxSessionCount_ > 0 && sessions_.Count >= maxSessionCount_)
                {
                    throw new ServiceResultException(StatusCodes.BadTooManySessions);
                }

                // check for same Nonce in another session
                if (clientNonce != null)
                {
                    foreach (var sessionIterator in sessions_.Values)
                    {
                        if (Utils.CompareNonce(sessionIterator.ClientNonce, clientNonce))
                        {
                            throw new ServiceResultException(StatusCodes.BadNonceInvalid);
                        }
                    }
                }

                // can assign a simple identifier if secured.
                authenticationToken = null;
                if (!String.IsNullOrEmpty(context.ChannelContext.SecureChannelId))
                {
                    if (context.ChannelContext.EndpointDescription.SecurityMode != MessageSecurityMode.None)
                    {
                        authenticationToken = Utils.IncrementIdentifier(ref lastSessionId_);
                    }
                }

                // must assign a hard-to-guess id if not secured.
                if (authenticationToken == null)
                {
                    var token = Utils.Nonce.CreateNonce(32);
                    authenticationToken = new NodeId(token);
                }

                // determine session timeout.
                if (requestedSessionTimeout > maxSessionTimeout_)
                {
                    revisedSessionTimeout = maxSessionTimeout_;
                }

                if (requestedSessionTimeout < minSessionTimeout_)
                {
                    revisedSessionTimeout = minSessionTimeout_;
                }

                // create server nonce.
                serverNonce = Utils.Nonce.CreateNonce((uint)minNonceLength_);

                // assign client name.
                if (String.IsNullOrEmpty(sessionName))
                {
                    sessionName = Utils.Format("Session {0}", sessionId);
                }

                // create instance of session.
                session = CreateSession(
                    context,
                    server_,
                    serverCertificate,
                    authenticationToken,
                    clientNonce,
                    serverNonce,
                    sessionName,
                    clientDescription,
                    endpointUrl,
                    clientCertificate,
                    revisedSessionTimeout,
                    maxResponseMessageSize,
                    maxRequestAge_,
                    maxBrowseContinuationPoints_);

                // get the session id.
                sessionId = session.Id;

                // save session.
                sessions_.Add(authenticationToken, session);
            }

            // raise session related event.
            RaiseSessionEvent(session, SessionEventReason.Created);

            // return session.
            return session;
        }

        /// <summary>
        /// Activates an existing session
        /// </summary>
        public virtual bool ActivateSession(
            UaServerOperationContext context,
            NodeId authenticationToken,
            SignatureData clientSignature,
            List<SoftwareCertificate> clientSoftwareCertificates,
            ExtensionObject userIdentityToken,
            SignatureData userTokenSignature,
            StringCollection localeIds,
            out byte[] serverNonce)
        {
            serverNonce = null;

            Session session = null;
            UserIdentityToken newIdentity = null;
            UserTokenPolicy userTokenPolicy = null;

            lock (lock_)
            {
                // find session.
                if (!sessions_.TryGetValue(authenticationToken, out session))
                {
                    throw new ServiceResultException(StatusCodes.BadSessionIdInvalid);
                }

                // check if session timeout has expired.
                if (session.HasExpired)
                {
                    // raise audit event for session closed because of timeout
                    server_.ReportAuditCloseSessionEvent(null, session, "Session/Timeout");

                    server_.CloseSession(null, session.Id, false);

                    throw new ServiceResultException(StatusCodes.BadSessionClosed);
                }

                // create new server nonce.
                serverNonce = Utils.Nonce.CreateNonce((uint)minNonceLength_);

                // validate before activation.
                session.ValidateBeforeActivate(
                    context,
                    clientSignature,
                    clientSoftwareCertificates,
                    userIdentityToken,
                    userTokenSignature,
                    localeIds,
                    serverNonce,
                    out newIdentity,
                    out userTokenPolicy);
            }

            IUserIdentity identity = null;
            IUserIdentity effectiveIdentity = null;
            ServiceResult error = null;

            try
            {
                // check if the application has a callback which validates the identity tokens.
                lock (eventLock_)
                {
                    if (ImpersonateUserEventHandler != null)
                    {
                        var args = new UaImpersonateUserEventArgs(newIdentity, userTokenPolicy, context.ChannelContext.EndpointDescription);
                        ImpersonateUserEventHandler(session, args);

                        if (ServiceResult.IsBad(args.IdentityValidationError))
                        {
                            error = args.IdentityValidationError;
                        }
                        else
                        {
                            identity = args.Identity;
                            effectiveIdentity = args.EffectiveIdentity;
                        }
                    }
                }

                // parse the token manually if the identity is not provided.
                if (identity == null)
                {
                    identity = newIdentity != null ? new UserIdentity(newIdentity) : new UserIdentity();
                }

                // use the identity as the effectiveIdentity if not provided.
                if (effectiveIdentity == null)
                {
                    effectiveIdentity = identity;
                }
            }
            catch (Exception e)
            {
                if (e is ServiceResultException)
                {
                    throw;
                }

                throw ServiceResultException.Create(
                    StatusCodes.BadIdentityTokenInvalid,
                    e,
                    "Could not validate user identity token: {0}",
                    newIdentity);
            }

            // check for validation error.
            if (ServiceResult.IsBad(error))
            {
                throw new ServiceResultException(error);
            }

            // activate session.
            var contextChanged = session.Activate(
                context,
                clientSoftwareCertificates,
                newIdentity,
                identity,
                effectiveIdentity,
                localeIds,
                serverNonce);

            // raise session related event.
            if (contextChanged)
            {
                RaiseSessionEvent(session, SessionEventReason.Activated);
            }

            // indicates that the identity context for the session has changed.
            return contextChanged;
        }

        /// <summary>
        /// Closes the specifed session.
        /// </summary>
        /// <remarks>
        /// This method should not throw an exception if the session no longer exists.
        /// </remarks>
        public virtual void CloseSession(NodeId sessionId)
        {
            // find the session.
            Session session = null;

            lock (lock_)
            {
                foreach (var current in sessions_)
                {
                    if (current.Value.Id == sessionId)
                    {
                        session = current.Value;
                        sessions_.Remove(current.Key);
                        break;
                    }
                }
            }

            if (session != null)
            {
                // raise session related event.
                RaiseSessionEvent(session, SessionEventReason.Closing);

                // close the session.
                session.Close();

                // update diagnostics.
                lock (server_.DiagnosticsWriteLock)
                {
                    server_.ServerDiagnostics.CurrentSessionCount--;
                }
            }

        }

        /// <summary>
        /// Validates request header and returns a request context.
        /// </summary>
        /// <remarks>
        /// This method verifies that the session id is valid and that it uses secure channel id
        /// associated with current thread. It also verifies that the timestamp is not too 
        /// and that the sequence number is not out of order (update requests only).
        /// </remarks>
        public virtual UaServerOperationContext ValidateRequest(RequestHeader requestHeader, RequestType requestType)
        {
            if (requestHeader == null) throw new ArgumentNullException(nameof(requestHeader));

            Session session = null;

            try
            {
                lock (lock_)
                {
                    // check for create session request.
                    if (requestType == RequestType.CreateSession || requestType == RequestType.ActivateSession)
                    {
                        return new UaServerOperationContext(requestHeader, requestType);
                    }

                    // find session.
                    if (!sessions_.TryGetValue(requestHeader.AuthenticationToken, out session))
                    {
                        var handler = ValidateSessionLessRequestEventHandler;

                        if (handler != null)
                        {
                            var args = new ValidateSessionLessRequestEventArgs(requestHeader.AuthenticationToken, requestType);
                            handler(this, args);

                            if (ServiceResult.IsBad(args.Error))
                            {
                                throw new ServiceResultException(args.Error);
                            }

                            return new UaServerOperationContext(requestHeader, requestType, args.Identity);
                        }

                        throw new ServiceResultException(StatusCodes.BadSessionIdInvalid);
                    }

                    // validate request header.
                    session.ValidateRequest(requestHeader, requestType);

                    // validate user has permissions for additional info
                    session.ValidateDiagnosticInfo(requestHeader);

                    // return context.
                    return new UaServerOperationContext(requestHeader, requestType, session);
                }
            }
            catch (Exception e)
            {
                var sre = e as ServiceResultException;

                if (sre != null && sre.StatusCode == StatusCodes.BadSessionNotActivated)
                {
                    if (session != null)
                    {
                        CloseSession(session.Id);
                    }
                }

                throw new ServiceResultException(e, StatusCodes.BadUnexpectedError);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Creates a new instance of a session.
        /// </summary>
        protected virtual Session CreateSession(
            UaServerOperationContext context,
            IUaServerData server,
            X509Certificate2 serverCertificate,
            NodeId sessionCookie,
            byte[] clientNonce,
            byte[] serverNonce,
            string sessionName,
            ApplicationDescription clientDescription,
            string endpointUrl,
            X509Certificate2 clientCertificate,
            double sessionTimeout,
            uint maxResponseMessageSize,
            int maxRequestAge, // TBD - Remove unused parameter.
            int maxContinuationPoints) // TBD - Remove unused parameter.
        {
            var session = new Session(
                context,
                server_,
                serverCertificate,
                sessionCookie,
                clientNonce,
                serverNonce,
                sessionName,
                clientDescription,
                endpointUrl,
                clientCertificate,
                sessionTimeout,
                maxResponseMessageSize,
                maxRequestAge_,
                maxBrowseContinuationPoints_,
                maxHistoryContinuationPoints_);

            return session;
        }

        /// <summary>
        /// Raises an event related to a session.
        /// </summary>
        protected virtual void RaiseSessionEvent(Session session, SessionEventReason reason)
        {
            lock (eventLock_)
            {
                EventHandler<SessionEventArgs> handler = null;

                switch (reason)
                {
                    case SessionEventReason.Created: { handler = SessionCreatedEventhandler; break; }
                    case SessionEventReason.Activated: { handler = SessionActivatedEventHandler; break; }
                    case SessionEventReason.Closing: { handler = SessionClosingEventHandler; break; }
                }

                if (handler != null)
                {
                    try
                    {
                        handler(session, new SessionEventArgs(reason));
                    }
                    catch (Exception e)
                    {
                        Utils.LogTrace(e, "Session event handler raised an exception.");
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Periodically checks if the sessions have timed out.
        /// </summary>
        private void MonitorSessions(object data)
        {
            try
            {
                Utils.LogInfo("Server - Session Monitor Thread Started.");

                var sleepCycle = Convert.ToInt32(data, CultureInfo.InvariantCulture);

                do
                {
                    Session[] sessions = null;

                    lock (lock_)
                    {
                        sessions = new Session[sessions_.Count];
                        sessions_.Values.CopyTo(sessions, 0);
                    }

                    for (var ii = 0; ii < sessions.Length; ii++)
                    {
                        if (sessions[ii].HasExpired)
                        {
                            // update diagnostics.
                            lock (server_.DiagnosticsWriteLock)
                            {
                                server_.ServerDiagnostics.SessionTimeoutCount++;
                            }

                            // raise audit event for session closed because of timeout
                            server_.ReportAuditCloseSessionEvent(null, sessions[ii], "Session/Timeout");

                            server_.CloseSession(null, sessions[ii].Id, false);
                        }
                    }

                    if (shutdownEvent_.WaitOne(sleepCycle))
                    {
                        Utils.LogTrace("Server - Session Monitor Thread Exited Normally.");
                        break;
                    }
                }
                while (true);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Server - Session Monitor Thread Exited Unexpectedly");
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private Dictionary<NodeId, Session> sessions_;
        private long lastSessionId_;
        private ManualResetEvent shutdownEvent_;

        private int minSessionTimeout_;
        private int maxSessionTimeout_;
        private int maxSessionCount_;
        private int maxRequestAge_;
        private int maxBrowseContinuationPoints_;
        private int maxHistoryContinuationPoints_;
        private int minNonceLength_;

        private readonly object eventLock_ = new object();
        private event EventHandler<SessionEventArgs> SessionCreatedEventhandler;
        private event EventHandler<SessionEventArgs> SessionActivatedEventHandler;
        private event EventHandler<SessionEventArgs> SessionClosingEventHandler;
        private event EventHandler<UaImpersonateUserEventArgs> ImpersonateUserEventHandler;
        private event EventHandler<ValidateSessionLessRequestEventArgs> ValidateSessionLessRequestEventHandler;
        #endregion

        #region IUaSessionManager Members
        /// <inheritdoc/>
        public event EventHandler<SessionEventArgs> SessionCreatedEvent
        {
            add
            {
                lock (eventLock_)
                {
                    SessionCreatedEventhandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    SessionCreatedEventhandler -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event EventHandler<SessionEventArgs> SessionActivatedEvent
        {
            add
            {
                lock (eventLock_)
                {
                    SessionActivatedEventHandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    SessionActivatedEventHandler -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event EventHandler<SessionEventArgs> SessionClosingEvent
        {
            add
            {
                lock (eventLock_)
                {
                    SessionClosingEventHandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    SessionClosingEventHandler -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event EventHandler<UaImpersonateUserEventArgs> ImpersonateUserEvent
        {
            add
            {
                lock (eventLock_)
                {
                    ImpersonateUserEventHandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    ImpersonateUserEventHandler -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event EventHandler<ValidateSessionLessRequestEventArgs> ValidateSessionLessRequestEvent
        {
            add
            {
                lock (eventLock_)
                {
                    ValidateSessionLessRequestEventHandler += value;
                }
            }

            remove
            {
                lock (eventLock_)
                {
                    ValidateSessionLessRequestEventHandler -= value;
                }
            }
        }

        /// <inheritdoc/>
        public IList<Session> GetSessions()
        {
            lock (lock_)
            {
                return new List<Session>(sessions_.Values);
            }
        }


        /// <inheritdoc/>
        public Session GetSession(NodeId authenticationToken)
        {

            Session session = null;
            lock (lock_)
            {
                // find session.
                sessions_.TryGetValue(authenticationToken, out session);
            }
            return session;
        }
        #endregion

    }
}
