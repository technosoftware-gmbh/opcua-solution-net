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

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    ///     A class which provides the event arguments for session related event.
    /// </summary>
    public class UaImpersonateUserEventArgs : EventArgs
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public UaImpersonateUserEventArgs(UserIdentityToken newIdentity, UserTokenPolicy userTokenPolicy, EndpointDescription endpointDescription = null)
        {
            NewIdentity = newIdentity;
            UserTokenPolicy = userTokenPolicy;
            EndpointDescription = endpointDescription;
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     The new user identity for the session.
        /// </summary>
        public UserIdentityToken NewIdentity { get; }

        /// <summary>
        ///     The user token policy selected by the client.
        /// </summary>
        public UserTokenPolicy UserTokenPolicy { get; }

        /// <summary>
        ///     An application defined handle that can be used for access control operations.
        /// </summary>
        public IUserIdentity Identity { get; set; }

        /// <summary>
        ///     An application defined handle that can be used for access control operations.
        /// </summary>
        public IUserIdentity EffectiveIdentity { get; set; }

        /// <summary>
        ///     Set to indicate that an error occurred validating the identity and that it should be rejected.
        /// </summary>
        public ServiceResult IdentityValidationError { get; set; }

        /// <summary>
        /// Get the EndpointDescription  
        /// </summary>
        public EndpointDescription EndpointDescription { get; }

        #endregion
    }
}