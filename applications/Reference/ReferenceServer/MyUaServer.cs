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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaStandardServer;
#endregion

namespace Technosoftware.ReferenceServer
{
    public class MyUaServer<T> where T : UaStandardServer.UaStandardServer, new()
    {
        public ApplicationInstance Application => application_;
        public ApplicationConfiguration Configuration => application_.ApplicationConfiguration;

        public bool AutoAccept { get; set; }
        public string Password { get; set; }

        public ExitCode ExitCode { get; private set; }

        /// <summary>
        /// Ctor of the server.
        /// </summary>
        /// <param name="writer">The text output.</param>
        public MyUaServer(TextWriter writer)
        {
            output_ = writer;
        }

        /// <summary>
        /// Load the application configuration.
        /// </summary>
        public async Task LoadAsync(string applicationName, string configSectionName)
        {
            try
            {
                ExitCode = ExitCode.ErrorNotStarted;

                ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output_);
                CertificatePasswordProvider PasswordProvider = new CertificatePasswordProvider(Password);
                application_ = new ApplicationInstance {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Server,
                    ConfigSectionName = configSectionName,
                    CertificatePasswordProvider = PasswordProvider
                };

                // load the application configuration.
                await application_.LoadApplicationConfigurationAsync(false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw new ErrorExitException(ex.Message, ExitCode);
            }
        }

        /// <summary>
        /// Load the application configuration.
        /// </summary>
        public async Task CheckCertificateAsync(bool renewCertificate)
        {
            try
            {
                var config = application_.ApplicationConfiguration;
                if (renewCertificate)
                {
                    await application_.DeleteApplicationInstanceCertificateAsync().ConfigureAwait(false);
                }

                // check the application certificate.
                bool haveAppCertificate = await application_.CheckApplicationInstanceCertificate(false, minimumKeySize: 0).ConfigureAwait(false);
                if (!haveAppCertificate)
                {
                    throw new ErrorExitException("Application instance certificate invalid!");
                }

                if (!config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
                }
            }
            catch (Exception ex)
            {
                throw new ErrorExitException(ex.Message, ExitCode);
            }
        }

        /// <summary>
        /// Create server instance and add node managers.
        /// </summary>
        public void Create(IList<IUaNodeManagerFactory> nodeManagerFactories)
        {
            try
            {
                // create the server.
                server_ = new T();
                if (nodeManagerFactories != null)
                {
                    foreach (var factory in nodeManagerFactories)
                    {
                        server_.AddNodeManager(factory);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ErrorExitException(ex.Message, ExitCode);
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public async Task StartAsync()
        {
            try
            {
                // create the server.
                server_ = server_ ?? new T();

                // start the server
                await application_.StartAsync(server_).ConfigureAwait(false);

                // save state
                ExitCode = ExitCode.ErrorRunning;

                // print endpoint info
                var endpoints = application_.BaseServer.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
                foreach (var endpoint in endpoints)
                {
                    output_.WriteLine(endpoint);
                }

                // start the status thread
                status_ = Task.Run(StatusThreadAsync);

                // print notification on session events
                server_.CurrentInstance.SessionManager.SessionActivatedEvent += EventStatus;
                server_.CurrentInstance.SessionManager.SessionClosingEvent += EventStatus;
                server_.CurrentInstance.SessionManager.SessionCreatedEvent += EventStatus;
            }
            catch (Exception ex)
            {
                throw new ErrorExitException(ex.Message, ExitCode);
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public async Task StopAsync()
        {
            try
            {
                if (server_ != null)
                {
                    using (T server = server_)
                    {
                        // Stop status thread
                        server_ = null;
                        await status_.ConfigureAwait(false);

                        // Stop server and dispose
                        server.Stop();
                    }
                }

                ExitCode = ExitCode.Ok;
            }
            catch (Exception ex)
            {
                throw new ErrorExitException(ex.Message, ExitCode.ErrorStopping);
            }
        }

        /// <summary>
        /// The certificate validator is used
        /// if auto accept is not selected in the configuration.
        /// </summary>
        private void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                if (AutoAccept)
                {
                    output_.WriteLine("Accepted Certificate: [{0}] [{1}]", e.Certificate.Subject, e.Certificate.Thumbprint);
                    e.Accept = true;
                    return;
                }
            }
            output_.WriteLine("Rejected Certificate: {0} [{1}] [{2}]", e.Error, e.Certificate.Subject, e.Certificate.Thumbprint);
        }

        /// <summary>
        /// Update the session status.
        /// </summary>
        private void EventStatus(object sender, SessionEventArgs eventArgs)
        {
            lastEventTime_ = DateTime.UtcNow;
            var session = sender as Session;
            PrintSessionStatus(session, eventArgs.Reason.ToString());
        }

        /// <summary>
        /// Output the status of a connected session.
        /// </summary>
        private void PrintSessionStatus(Session session, string reason, bool lastContact = false)
        {
            StringBuilder item = new StringBuilder();
            lock (session.DiagnosticsLock)
            {
                item.AppendFormat("{0,9}:{1,20}:", reason, session.SessionDiagnostics.SessionName);
                if (lastContact)
                {
                    item.AppendFormat("Last Event:{0:HH:mm:ss}", session.SessionDiagnostics.ClientLastContactTime.ToLocalTime());
                }
                else
                {
                    if (session.Identity != null)
                    {
                        item.AppendFormat(":{0,20}", session.Identity.DisplayName);
                    }
                    item.AppendFormat(":{0}", session.Id);
                }
            }
            output_.WriteLine(item.ToString());
        }

        /// <summary>
        /// Status thread, prints connection status every 10 seconds.
        /// </summary>
        private async Task StatusThreadAsync()
        {
            while (server_ != null)
            {
                if (DateTime.UtcNow - lastEventTime_ > TimeSpan.FromMilliseconds(10000))
                {
                    IList<Session> sessions = server_.CurrentInstance.SessionManager.GetSessions();
                    for (int ii = 0; ii < sessions.Count; ii++)
                    {
                        Session session = sessions[ii];
                        PrintSessionStatus(session, "-Status-", true);
                    }
                    lastEventTime_ = DateTime.UtcNow;
                }
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        #region Private Members
        private readonly TextWriter output_;
        private ApplicationInstance application_;
        private T server_;
        private Task status_;
        private DateTime lastEventTime_;
        #endregion
    }
}

