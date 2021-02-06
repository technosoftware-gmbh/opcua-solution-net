#region Copyright (c) 2011-2021 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2021 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2021 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Diagnostics;
using System.Reflection;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
#endregion

namespace Technosoftware.ModelDesignServer
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
    /// the WorkshopUaServerSampleNodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class ModelDesignServer : UaBaseServer
    {
        #region Overridden Methods
        /// <summary>
        /// Initializes the server before it starts up.
        /// </summary>
        /// <remarks>
        /// This method is called before any startup processing occurs. The sub-class may update the 
        /// configuration object or do any other application specific startup tasks.
        /// </remarks>
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            Utils.Trace("The server is starting.");

            base.OnServerStarting(configuration);
        }

        /// <summary>
        /// Called after the server has been started.
        /// </summary>
        protected override void OnServerStarted(IUaServerData server)
        {
            base.OnServerStarted(server);

            // request notifications when the user identity is changed. all valid users are accepted by default.
            server.SessionManager.ImpersonateUserEvent += OnImpersonateUser;

            try
            {
                // allow a faster sampling interval for CurrentTime node.
                lock (server.Status.Lock)
                {
                    server.Status.Variable.CurrentTime.MinimumSamplingInterval = 250;
                }
            }
            catch (Exception exception)
            {
                Utils.Trace("Exception:" + exception.Message);
            }
        }

        /// <summary>
        /// Cleans up before the server shuts down.
        /// </summary>
        /// <remarks>
        /// This method is called before any shutdown processing occurs.
        /// </remarks>
        protected override void OnServerStopping()
        {
            Debug.WriteLine("The Server is stopping.");

            base.OnServerStopping();
        }

        /// <summary>
        /// Adds all encodeable types to the server.
        /// </summary>
        /// <param name="uaServerData">The uaServerData data implementing the IUaServerData interface.</param>
        public override void AddEncodeableTypes(IUaServerData uaServerData)
        {
            // add the types defined in the information model library to the factory.
            uaServerData.Factory.AddEncodeableTypes(GetType().GetTypeInfo().Assembly);
        }
        #endregion

        #region User Validation Functions
        /// <summary>
        /// Called when a client tries to change its user identity.
        /// </summary>
        private void OnImpersonateUser(object sender, UaImpersonateUserEventArgs args)
        {
            // check for a user name token.
            var userNameToken = args.NewIdentity as UserNameIdentityToken;

            if (userNameToken != null)
            {
                args.Identity = VerifyPassword(userNameToken);
                return;
            }

        }

        /// <summary>
        /// Validates the password for a username token.
        /// </summary>
        private IUserIdentity VerifyPassword(UserNameIdentityToken userNameToken)
        {
            var userName = userNameToken.UserName;
            var password = userNameToken.DecryptedPassword;
            if (String.IsNullOrEmpty(userName))
            {
                // an empty username is not accepted.
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted.");
            }

            if (String.IsNullOrEmpty(password))
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