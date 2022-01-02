#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
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
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Configuration;
#endregion

namespace Technosoftware.ReferenceServer
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
    /// the ReferenceServerNodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class ReferenceServer : UaBaseServer
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

            // it is up to the application to decide how to validate user identity tokens.
            // this function creates validator for X509 identity tokens.
            CreateUserIdentityValidators(configuration);
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
            catch
            {
                // ignored
            }
        }

        #endregion

        #region User Validation Functions
        /// <summary>
        /// Creates the objects used to validate the user identity tokens supported by the server.
        /// </summary>
        private void CreateUserIdentityValidators(ApplicationConfiguration configuration)
        {
            foreach (var policy in configuration.ServerConfiguration.UserTokenPolicies)
            {
                // create a validator for a certificate token policy.
                if (policy.TokenType == UserTokenType.Certificate)
                {
                    // check if user certificate trust lists are specified in configuration.
                    if (configuration.SecurityConfiguration.TrustedUserCertificates != null &&
                        configuration.SecurityConfiguration.UserIssuerCertificates != null)
                    {
                        var certificateValidator = new CertificateValidator();
                        certificateValidator.Update(configuration.SecurityConfiguration).Wait();
                        certificateValidator.Update(configuration.SecurityConfiguration.UserIssuerCertificates,
                            configuration.SecurityConfiguration.TrustedUserCertificates,
                            configuration.SecurityConfiguration.RejectedCertificateStore);

                        // set custom validator for user certificates.
                        userCertificateValidator_ = certificateValidator.GetChannelValidator();
                    }
                }
            }
        }

        /// <summary>
        /// Called when a client tries to change its user identity.
        /// </summary>
        private void OnImpersonateUser(object sender, UaImpersonateUserEventArgs args)
        {
            // check for a user name token.
            if (args.NewIdentity is UserNameIdentityToken userNameToken)
            {
                args.Identity = VerifyPassword(userNameToken);

                // set AuthenticatedUser role for accepted user/password authentication
                args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_AuthenticatedUser);

                if (args.Identity is SystemConfigurationIdentity)
                {
                    // set ConfigureAdmin role for user with permission to configure server
                    args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_ConfigureAdmin);
                    args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_SecurityAdmin);
                }

                return;
            }

            // check for x509 user token.
            if (args.NewIdentity is X509IdentityToken x509Token)
            {
                VerifyUserTokenCertificate(x509Token.Certificate);
                args.Identity = new UserIdentity(x509Token);
                Utils.Trace("X509 Token Accepted: {0}", args.Identity.DisplayName);

                // set AuthenticatedUser role for accepted certificate authentication
                args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_AuthenticatedUser);

                return;
            }

            // check for anonymous token.
            if (args.NewIdentity is AnonymousIdentityToken || args.NewIdentity == null)
            {
                // allow anonymous authentication and set Anonymous role for this authentication
                args.Identity = new UserIdentity();
                args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_Anonymous);

                return;
            }

            // unsuported identity token type.
            throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                   "Not supported user token type: {0}.", args.NewIdentity);
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

            // User with permission to configure server
            if (userName == "sysadmin" && password == "demo")
            {
                return new SystemConfigurationIdentity(new UserIdentity(userNameToken));
            }

            // standard users for CTT verification
            if (!((userName == "user1" && password == "password") ||
                (userName == "user2" && password == "password1")))
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

        /// <summary>
        /// Verifies that a certificate user token is trusted.
        /// </summary>
        private void VerifyUserTokenCertificate(X509Certificate2 certificate)
        {
            try
            {
                if (userCertificateValidator_ != null)
                {
                    userCertificateValidator_.Validate(certificate);
                }
                else
                {
                    CertificateValidator.Validate(certificate);
                }
            }
            catch (Exception e)
            {
                TranslationInfo info;
                StatusCode result = StatusCodes.BadIdentityTokenRejected;
                if (e is ServiceResultException se && se.StatusCode == StatusCodes.BadCertificateUseNotAllowed)
                {
                    info = new TranslationInfo(
                        "InvalidCertificate",
                        "en-US",
                        "'{0}' is an invalid user certificate.",
                        certificate.Subject);

                    result = StatusCodes.BadIdentityTokenInvalid;
                }
                else
                {
                    // construct translation object with default text.
                    info = new TranslationInfo(
                        "UntrustedCertificate",
                        "en-US",
                        "'{0}' is not a trusted user certificate.",
                        certificate.Subject);
                }

                // create an exception with a vendor defined sub-code.
                throw new ServiceResultException(new ServiceResult(
                    result,
                    info.Key,
                    LoadServerProperties().ProductUri,
                    new LocalizedText(info)));
            }
        }
        #endregion

        #region Private Fields
        private ICertificateValidator userCertificateValidator_;
        #endregion
    }
}
