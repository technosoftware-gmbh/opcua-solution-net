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
using System.Threading;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Stores information used while a thread is completing an operation on behalf of a client.
    /// </summary>
    public class UaServerOperationContext : IOperationContext
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the context with a session.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="identity">The user identity used in the request.</param>
        public UaServerOperationContext(RequestHeader requestHeader, Sessions.RequestType requestType, IUserIdentity identity = null)
        {
            if (requestHeader == null) throw new ArgumentNullException(nameof(requestHeader));
            
            ChannelContext    = SecureChannelContext.Current;
            Session           = null;
            UserIdentity      = identity;
            PreferredLocales  = Array.Empty<string>();
            DiagnosticsMask   = (DiagnosticsMasks)requestHeader.ReturnDiagnostics;
            StringTable       = new StringTable();
            AuditEntryId      = requestHeader.AuditEntryId;
            RequestId         = Utils.IncrementIdentifier(ref lastRequestId_);
            RequestType       = requestType;
            ClientHandle      = requestHeader.RequestHandle;
            OperationDeadline = DateTime.MaxValue;

            if (requestHeader.TimeoutHint > 0)
            {
                OperationDeadline = DateTime.UtcNow.AddMilliseconds(requestHeader.TimeoutHint);
            }
        }

        /// <summary>
        /// Initializes the context with a session.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="session">The session.</param>
        /// <exception cref="ArgumentNullException">In case requestHeader or session is null.</exception>
        /// <exception cref="ArgumentNullException">In case session is null.</exception>
        public UaServerOperationContext(RequestHeader requestHeader, Sessions.RequestType requestType, Sessions.Session session)
        {
            if (requestHeader == null) throw new ArgumentNullException(nameof(requestHeader));
            if (session == null)       throw new ArgumentNullException(nameof(session));

            ChannelContext     = SecureChannelContext.Current;
            Session            = session;
            UserIdentity      = session.EffectiveIdentity;
            PreferredLocales  = session.PreferredLocales;
            DiagnosticsMask   = (DiagnosticsMasks)requestHeader.ReturnDiagnostics;
            StringTable       = new StringTable();
            AuditEntryId   = requestHeader.AuditEntryId;
            RequestId          = Utils.IncrementIdentifier(ref lastRequestId_);
            RequestType        = requestType;
            ClientHandle       = requestHeader.RequestHandle;
            OperationDeadline = DateTime.MaxValue;

            if (requestHeader.TimeoutHint > 0)
            {
                OperationDeadline = DateTime.UtcNow.AddMilliseconds(requestHeader.TimeoutHint);
            }
        }

        /// <summary>
        /// Initializes the context with a session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="diagnosticsMasks">The diagnostics masks.</param>
        /// <exception cref="ArgumentNullException">In case session is null.</exception>
        public UaServerOperationContext(Sessions.Session session, DiagnosticsMasks diagnosticsMasks)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            ChannelContext     = null;
            Session            = session;
            UserIdentity      = session.EffectiveIdentity;
            PreferredLocales  = session.PreferredLocales;
            DiagnosticsMask   = diagnosticsMasks;
            StringTable       = new StringTable();
            AuditEntryId   = null;
            RequestId          = 0;
            RequestType        = Sessions.RequestType.Unknown;
            ClientHandle       = 0;
            OperationDeadline = DateTime.MaxValue;
        }

        /// <summary>
        /// Initializes the context with a monitored item.
        /// </summary>
        /// <param name="monitoredItem">The monitored item.</param>
        /// <exception cref="ArgumentNullException">In case monitoredItem is null.</exception>
        public UaServerOperationContext(IUaMonitoredItem monitoredItem)
        {
            if (monitoredItem == null) throw new ArgumentNullException(nameof(monitoredItem));
            
            ChannelContext = null;
            Session = monitoredItem.Session;

            if (Session != null)
            {
                UserIdentity = Session.Identity;
                PreferredLocales  = Session.PreferredLocales;
            }                
                
            DiagnosticsMask   = DiagnosticsMasks.SymbolicId;
            StringTable       = new StringTable();
            AuditEntryId   = null;
            RequestId          = 0;
            RequestType        = Sessions.RequestType.Unknown;
            ClientHandle       = 0;
            OperationDeadline = DateTime.MaxValue;
        }
        #endregion   
                
        #region Public Properties
        /// <summary>
        /// The context for the secure channel used to send the request.
        /// </summary>
        /// <value>The channel context.</value>
        public SecureChannelContext ChannelContext { get; }

        /// <summary>
        /// The session associated with the context.
        /// </summary>
        /// <value>The session.</value>
        public Sessions.Session Session { get; }

        /// <summary>
        /// The security policy used for the secure channel.
        /// </summary>
        /// <value>The security policy URI.</value>
        public string SecurityPolicyUri => ChannelContext?.EndpointDescription?.SecurityPolicyUri;

        /// <summary>
        /// The type of request.
        /// </summary>
        /// <value>The type of the request.</value>
        public Sessions.RequestType RequestType { get; }

        /// <summary>
        /// A unique identifier assigned to the request by the server.
        /// </summary>
        /// <value>The request id.</value>
        public uint RequestId { get; }

        /// <summary>
        /// The handle assigned by the client to the request.
        /// </summary>
        /// <value>The client handle.</value>
        public uint ClientHandle { get; }

        /// <summary>
        /// Updates the status code (thread safe).
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public void SetStatusCode(StatusCode statusCode)
        {
            Interlocked.Exchange(ref operationStatus_, statusCode.Code);
        }
        #endregion

        #region IOperationContext Members
        /// <summary>
        /// The identifier for the session (null if multiple sessions are associated with the operation).
        /// </summary>
        /// <value>The session id.</value>
        public NodeId SessionId => Session?.Id;

        /// <summary>
        /// The identity context to use when processing the request.
        /// </summary>
        /// <value>The user identity.</value>
        public IUserIdentity UserIdentity { get; }

        /// <summary>
        /// The locales to use for the operation.
        /// </summary>
        /// <value>The preferred locales.</value>
        public IList<string> PreferredLocales { get; }

        /// <summary>
        /// The diagnostics mask specified with the request.
        /// </summary>
        /// <value>The diagnostics mask.</value>
        public DiagnosticsMasks DiagnosticsMask { get; }

        /// <summary>
        /// A table of diagnostics strings to return in the response.
        /// </summary>
        /// <value>The string table.</value>
        /// <remarks>
        /// This object is thread safe.
        /// </remarks>
        public StringTable StringTable { get; }

        /// <summary>
        /// When the request times out.
        /// </summary>
        /// <value>The operation deadline.</value>
        public DateTime OperationDeadline { get; }

        /// <summary>
        /// The current status of the request (used to check for timeouts/client cancel requests).
        /// </summary>
        /// <value>The operation status.</value>
        public StatusCode OperationStatus => (uint)operationStatus_;

        /// <summary>
        /// The audit log entry id provided by the client which must be included in an audit events generated by the server.
        /// </summary>
        /// <value>The audit entry id.</value>
        public string AuditEntryId { get; }

        #endregion

        #region Private Fields
        private long operationStatus_;
        private static long lastRequestId_;
        #endregion
    }
}
