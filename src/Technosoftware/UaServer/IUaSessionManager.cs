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

namespace Technosoftware.UaServer
{
    /// <summary>
    ///     Allows application components to receive notifications when changes to sessions occur.
    /// </summary>
    /// <remarks>
    ///     Sinks that receive these events must not block the thread.
    /// </remarks>
    public interface IUaSessionManager
    {
        /// <summary>
        ///     Raised after a new session is created.
        /// </summary>
        event EventHandler<Sessions.SessionEventArgs> SessionCreatedEvent;

        /// <summary>
        ///     Raised whenever a session is activated and the user identity or preferred locales changed.
        /// </summary>
        event EventHandler<Sessions.SessionEventArgs> SessionActivatedEvent;

        /// <summary>
        ///     Raised before a session is closed.
        /// </summary>
        event EventHandler<Sessions.SessionEventArgs> SessionClosingEvent;

        /// <summary>
        ///     Raised before the user identity for a session is changed.
        /// </summary>
        event EventHandler<UaImpersonateUserEventArgs> ImpersonateUserEvent;

        /// <summary>
        /// Raised to validate a session-less request.
        /// </summary>
        event EventHandler<ValidateSessionLessRequestEventArgs> ValidateSessionLessRequestEvent;

        /// <summary>
        ///     Returns all of the sessions known to the session manager.
        /// </summary>
        /// <returns>A list of the sessions.</returns>
        IList<Sessions.Session> GetSessions();
    }

    #region ValidateSessionLessRequestEventArgs Class
    /// <summary>
    /// A class which provides the event arguments for session related event.
    /// </summary>
    public class ValidateSessionLessRequestEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ValidateSessionLessRequestEventArgs(NodeId authenticationToken, Sessions.RequestType requestType)
        {
            AuthenticationToken = authenticationToken;
            RequestType = requestType;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The request type for the request.
        /// </summary>
        public Sessions.RequestType RequestType { get; private set; }

        /// <summary>
        /// The new user identity for the session.
        /// </summary>
        public NodeId AuthenticationToken { get; private set; }

        /// <summary>
        /// The identity to associate with the session-less request.
        /// </summary>
        public IUserIdentity Identity { get; set; }

        /// <summary>
        /// Set to indicate that an error occurred validating the session-less request and that it should be rejected.
        /// </summary>
        public ServiceResult Error { get; set; }
        #endregion
    }
    #endregion
}