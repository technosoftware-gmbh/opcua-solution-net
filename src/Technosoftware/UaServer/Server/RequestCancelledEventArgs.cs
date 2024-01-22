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

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.Server
{
    /// <summary>
    ///     The event arguments provided when a new notification message arrives.
    /// </summary>
    public class RequestCancelledEventArgs : EventArgs
    {
        #region Constructors, Destructor, Initialization

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        internal RequestCancelledEventArgs(uint requestId, StatusCode statusCode)
        {
            RequestId = requestId;
            StatusCode = statusCode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The request ID.
        /// </summary>
        public uint RequestId { get; private set; }

        /// <summary>
        ///     Thestatus of the operation.
        /// </summary>
        public StatusCode StatusCode { get; private set; }

        #endregion
    }
}