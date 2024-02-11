#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;

using Technosoftware.UaClient;
#endregion

namespace SampleCompany.SampleClient
{
    /// <summary>The UA client sample functionality.</summary>
    public class MyUaClient : IMyUaClient, IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the MyUaClient class.
        /// </summary>
        public MyUaClient(ApplicationConfiguration configuration, TextWriter writer)
        {
            output_ = writer;
            configuration_ = configuration;
            configuration_.CertificateValidator.CertificateValidation += OnCertificateValidation;

        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose objects.
        /// </summary>
        public void Dispose()
        {
            Utils.SilentDispose(Session);
            configuration_.CertificateValidator.CertificateValidation -= OnCertificateValidation;
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the client session.
        /// </summary>
        public Session Session { get; private set; }

        /// <summary>
        /// The session keepalive interval to be used in ms.
        /// </summary>
        public int KeepAliveInterval { get; set; } = 5000;

        /// <summary>
        /// The reconnect period to be used in ms.
        /// </summary>
        public int ReconnectPeriod { get; set; } = 1000;

        /// <summary>
        /// The reconnect period exponential backoff to be used in ms.
        /// </summary>
        public int ReconnectPeriodExponentialBackoff { get; set; } = 15000;

        /// <summary>
        /// The session lifetime.
        /// </summary>
        public uint SessionLifeTime { get; set; } = 60 * 1000;

        /// <summary>
        /// The user identity to use to connect to the server.
        /// </summary>
        public IUserIdentity UserIdentity { get; set; } = new UserIdentity();

        /// <summary>
        /// Auto accept untrusted certificates.
        /// </summary>
        public bool AutoAccept { get; set; } = false;
        #endregion

        #region Public Methods
        public void DiscoverUaServers()
        {
            try
            {
                // Discover all OPC UA Servers
                ApplicationDescriptionCollection servers = Discover.GetServerDescriptions(configuration_);

                if (servers != null)
                {
                    output_.WriteLine("Found OPC UA Servers:");
                    foreach (ApplicationDescription server in servers)
                    {
                        output_.WriteLine("      {0}, {1}", server.ApplicationName, server.ApplicationUri);
                    }
                }
                return;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Creates a session with the UA server
        /// </summary>
        public async Task<bool> ConnectAsync(string serverUrl, bool useSecurity = true, CancellationToken ct = default)
        {
            if (serverUrl == null)
            {
                throw new ArgumentNullException(nameof(serverUrl));
            }

            try
            {
                if (Session != null && Session.Connected)
                {
                    output_.WriteLine("Session already connected!");
                }
                else
                {
                    EndpointDescription endpointDescription = null;
                    output_.WriteLine("Connecting to... {0}", serverUrl);
                    endpointDescription = Discover.SelectEndpoint(configuration_, serverUrl, useSecurity);

                    // Get the endpoint by connecting to server's discovery endpoint.
                    // Try to find the first endopint with security.
                    var endpointConfiguration = EndpointConfiguration.Create(configuration_);
                    var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                    TraceableSessionFactory sessionFactory = TraceableSessionFactory.Instance;

                    // Create the session
                    Session session = await Session.CreateAsync(
                        configuration_,
                        endpoint,
                        false,
                        false,
                        configuration_.ApplicationName,
                        SessionLifeTime,
                        UserIdentity,
                        null,
                        ct
                    ).ConfigureAwait(false);

                    // Assign the created session
                    if (session != null && session.Connected)
                    {
                        Session = session;

                        // override keep alive interval
                        Session.KeepAliveInterval = KeepAliveInterval;

                        // support transfer
                        Session.DeleteSubscriptionsOnClose = false;
                        Session.TransferSubscriptionsOnReconnect = true;

                        // set up keep alive callback.
                        Session.SessionKeepAliveEvent += OnSessionKeepAlive;

                        // prepare a reconnect handler
                        reconnectHandler_ = new SessionReconnectHandler(true, ReconnectPeriodExponentialBackoff);
                    }

                    // Session created successfully.
                    output_.WriteLine("New Session Created with SessionName = {0}", Session.SessionName);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log Error
                output_.WriteLine("Create Session Error : {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Disconnects the session.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (Session != null)
                {
                    output_.WriteLine("Disconnecting...");

                    lock (lock_)
                    {
                        Session.SessionKeepAliveEvent -= OnSessionKeepAlive;
                        reconnectHandler_?.Dispose();
                        reconnectHandler_ = null;
                    }

                    _ = Session.Close();
                    Session.Dispose();
                    Session = null;

                    // Log Session Disconnected event
                    output_.WriteLine("Session Disconnected.");
                }
                else
                {
                    output_.WriteLine("Session not created!");
                }
            }
            catch (Exception ex)
            {
                // Log Error
                output_.WriteLine($"Disconnect Error : {ex.Message}");
            }
        }
        #endregion

        #region KeepAlive and ReConnect handling
        /// <summary>
        /// Handles a keep alive event from a session and triggers a reconnect if necessary.
        /// </summary>
        private void OnSessionKeepAlive(object sender, SessionKeepAliveEventArgs e)
        {
            try
            {
                var session = (Session)sender;

                // check for events from discarded sessions.
                if (!Session.Equals(session))
                {
                    return;
                }

                // start reconnect sequence on communication error.
                if (ServiceResult.IsBad(e.Status))
                {
                    if (ReconnectPeriod <= 0)
                    {
                        Utils.LogWarning("KeepAlive status {0}, but reconnect is disabled.", e.Status);
                        return;
                    }

                    SessionReconnectHandler.ReconnectState state = reconnectHandler_.BeginReconnect(Session, ReconnectPeriod, OnReconnectComplete);
                    if (state == SessionReconnectHandler.ReconnectState.Triggered)
                    {
                        Utils.LogInfo("KeepAlive status {0}, reconnect status {1}, reconnect period {2}ms.", e.Status, state, ReconnectPeriod);
                    }
                    else
                    {
                        Utils.LogInfo("KeepAlive status {0}, reconnect status {1}.", e.Status, state);
                    }

                    // cancel sending a new keep alive request, because reconnect is triggered.
                    e.CancelKeepAlive = true;

                    return;
                }
            }
            catch (Exception exception)
            {
                Utils.LogError(exception, "Error in OnKeepAlive.");
            }
        }

        /// <summary>
        /// Called when the reconnect attempt was successful.
        /// </summary>
        private void OnReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!ReferenceEquals(sender, reconnectHandler_))
            {
                return;
            }

            lock (lock_)
            {
                // if session recovered, Session property is null
                if (reconnectHandler_.Session != null)
                {
                    // ensure only a new instance is disposed
                    // after reactivate, the same session instance may be returned
                    if (!ReferenceEquals(Session, reconnectHandler_.Session))
                    {
                        output_.WriteLine("--- RECONNECTED TO NEW SESSION --- {0}", reconnectHandler_.Session.SessionId);
                        IUaSession session = Session;
                        Session = (Session)reconnectHandler_.Session;
                        Utils.SilentDispose(session);
                    }
                    else
                    {
                        output_.WriteLine("--- REACTIVATED SESSION --- {0}", reconnectHandler_.Session.SessionId);
                    }
                }
                else
                {
                    output_.WriteLine("--- RECONNECT KeepAlive recovered ---");
                }
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Handles the certificate validation event.
        /// This event is triggered every time an untrusted certificate is received from the server.
        /// </summary>
        protected virtual void OnCertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            var certificateAccepted = false;

            // ****
            // Implement a custom logic to decide if the certificate should be
            // accepted or not and set certificateAccepted flag accordingly.
            // The certificate can be retrieved from the e.Certificate field
            // ***

            ServiceResult error = e.Error;
            output_.WriteLine(error);
            if (error.StatusCode == StatusCodes.BadCertificateUntrusted && AutoAccept)
            {
                certificateAccepted = true;
            }

            if (certificateAccepted)
            {
                output_.WriteLine("Untrusted Certificate accepted. Subject = {0}", e.Certificate.Subject);
                e.Accept = true;
            }
            else
            {
                output_.WriteLine("Untrusted Certificate rejected. Subject = {0}", e.Certificate.Subject);
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private readonly ApplicationConfiguration configuration_;
        private SessionReconnectHandler reconnectHandler_;
        private readonly TextWriter output_;
        #endregion
    }
}
