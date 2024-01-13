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

#endregion

namespace Technosoftware.UaServer.Sessions
{
    /// <summary>
    ///     The possible reasons for a session related event.
    /// </summary>
    public enum SessionEventReason
    {
        /// <summary>
        ///     A new session was created.
        /// </summary>
        Created,

        /// <summary>
        ///     A session is being activated with a new user identity.
        /// </summary>
        Impersonating,

        /// <summary>
        ///     A session was activated and the user identity or preferred locales changed.
        /// </summary>
        Activated,

        /// <summary>
        ///     A session is about to be closed.
        /// </summary>
        Closing
    }
}