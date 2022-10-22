#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Diagnostics;
using System.Reflection;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaBaseServer;
#endregion

namespace SampleCompany.SampleServer
{
    /// <summary>
    /// Implements a basic OPC UA Server.
    /// </summary>
    /// <remarks>
    /// Each server instance must have one instance of a StandardServer object which is
    /// responsible for reading the configuration file, creating the endpoints and dispatching
    /// incoming requests to the appropriate handler.
    /// 
    /// This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the EmptyServerNodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class SampleServer : UaBaseServer
    {
        #region Overridden Methods
        protected override void OnServerStarted(IUaServerData server)
        {
            base.OnServerStarted(server);

            // request notifications when the user identity is changed.
            // All valid users are accepted by default.
            server.SessionManager.ImpersonateUserEvent += OnImpersonateUser;
        }
        #endregion

        #region User Validation Functions
        /// <summary>
        /// Called when a client tries to change its user identity.
        /// </summary>
        private void OnImpersonateUser(object sender, UaImpersonateUserEventArgs args)
        {
            // check for a user name token.
            if (args.NewIdentity is UserNameIdentityToken userNameToken)
            {
                args.Identity = VerifyPassword(userNameToken);
            }
        }

        /// <summary>
        /// Validates the password for a username token.
        /// </summary>
        private IUserIdentity VerifyPassword(UserNameIdentityToken userNameToken)
        {
            var userName = userNameToken.UserName;
            var password = userNameToken.DecryptedPassword;
            if (string.IsNullOrEmpty(userName))
            {
                // an empty username is not accepted.
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted.");
            }

            if (string.IsNullOrEmpty(password))
            {
                // an empty password is not accepted.
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenRejected,
                    "Security token is not a valid username token. An empty password is not accepted.");
            }

            // verify operator and administrator users
            if (!((userName == "operator" && password == "password1") ||
                 (userName == "administrator" && password == "password2")))
            {
                // construct translation object with default text.
                var info = new TranslationInfo(
                    "InvalidPassword",
                    "en-US",
                    "Invalid username or password.",
                    userName);

                // create an exception with a vendor defined sub-code.
                throw new ServiceResultException(new ServiceResult(
                    StatusCodes.BadUserAccessDenied,
                    "InvalidPassword",
                    LoadServerProperties().ProductUri,
                    new LocalizedText(info)));
            }
            return new UserIdentity(userNameToken);
        }
        #endregion
    }
}