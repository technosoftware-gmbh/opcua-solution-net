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

using System.Xml;

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.Configuration
{
    /// <summary>
    /// Priviledged identity which can access the system configuration.
    /// </summary>
    public class SystemConfigurationIdentity : IUserIdentity
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Create a user identity with the priviledge
        /// to modify the system configuration.
        /// </summary>
        /// <param name="identity">The user identity.</param>
        public SystemConfigurationIdentity(IUserIdentity identity)
        {
            userIdentity_ = identity;
        }
        #endregion

        #region IUserIdentity
        /// <summary>
        /// A display name that identifies the user.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName => userIdentity_.DisplayName;

        /// <summary>
        /// The user token policy.
        /// </summary>
        /// <value>The user token policy.</value>
        public string PolicyId => userIdentity_.PolicyId;

        /// <summary>
        /// The type of identity token used.
        /// </summary>
        /// <value>The type of the token.</value>
        public UserTokenType TokenType => userIdentity_.TokenType;

        /// <summary>
        /// The type of issued token.
        /// </summary>
        /// <value>The type of the issued token.</value>
        public XmlQualifiedName IssuedTokenType => userIdentity_.IssuedTokenType;

        /// <summary>
        /// Whether the object can create signatures to prove possession of the user's credentials.
        /// </summary>
        /// <value><c>true</c> if signatures are supported; otherwise, <c>false</c>.</value>
        public bool SupportsSignatures => userIdentity_.SupportsSignatures;

        /// <summary>
        /// Get or sets the list of granted role ids associated to the UserIdentity.
        /// </summary>
        public NodeIdCollection GrantedRoleIds
        {
            get => userIdentity_.GrantedRoleIds;
            set => userIdentity_.GrantedRoleIds = value;
        }

        /// <summary>
        /// Returns a UA user identity token containing the user information.
        /// </summary>
        /// <returns>UA user identity token containing the user information.</returns>
        public UserIdentityToken GetIdentityToken()
        {
            return userIdentity_.GetIdentityToken();
        }
        #endregion

        #region Private Fields
        private readonly IUserIdentity userIdentity_;
        #endregion
    }
}