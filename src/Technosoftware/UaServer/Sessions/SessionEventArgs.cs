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

#endregion

namespace Technosoftware.UaServer.Sessions
{
    /// <summary>
    ///     A class which provides the event arguments for session related event.
    /// </summary>
    public class SessionEventArgs : EventArgs
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public SessionEventArgs(SessionEventReason reason)
        {
            Reason = reason;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The possible reasons for a session related event.
        /// </summary>
        public SessionEventReason Reason { get; }

        #endregion

        #region Private Fields

        #endregion
    }
}