#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Opc.Ua;

using Technosoftware.UaClient;
#endregion

namespace SampleCompany.ReferenceClient
{
    /// <summary>The UA client sample functionality.</summary>
    public class MyUaClient : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MyUaClient class.
        /// </summary>
        public MyUaClient(ApplicationConfiguration configuration, TextWriter writer, bool verbose)
        {
            output_ = writer;
            verbose_ = verbose;
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
            Utils.SilentDispose(session_);
            configuration_.CertificateValidator.CertificateValidation -= OnCertificateValidation;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the client session.
        /// </summary>
        public Session Session => session_;

        /// <summary>
        /// The session keepalive interval to be used in ms.
        /// </summary>
        public int KeepAliveInterval { get; set; } = 5000;

        /// <summary>
        /// The reconnect period to be used in ms.
        /// </summary>
        public int ReconnectPeriod { get; set; } = 10000;

        /// <summary>
        /// The session lifetime.
        /// </summary>
        public uint SessionLifeTime { get; set; } = 30 * 1000;

        /// <summary>
        /// The user identity to use to connect to the server.
        /// </summary>
        public IUserIdentity UserIdentity { get; set; } = new UserIdentity();

        /// <summary>
        /// Auto accept untrusted certificates.
        /// </summary>
        public bool AutoAccept { get; set; } = false;

        /// <summary>
        /// The file to use for log output.
        /// </summary>
        public string LogFile { get; set; }
        #endregion

        #region Public Methods
        public void DiscoverUaServers(ApplicationConfiguration configuration)
        {
            try
            {
                output_.WriteLine("Found OPC UA Servers:");

                // Discover all OPC UA Servers
                Opc.Ua.ApplicationDescriptionCollection servers = null;
                servers = Discover.GetServerDescriptions(configuration);

                if (servers != null)
                {
                    foreach (Opc.Ua.ApplicationDescription server in servers)
                    {
                        output_.WriteLine("      { 0}, { 1}", server.ApplicationName, server.ApplicationUri);
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
        public async Task<bool> ConnectAsync(string serverUrl, bool useSecurity = true)
        {
            if (serverUrl == null) throw new ArgumentNullException(nameof(serverUrl));

            try
            {
                if (session_ != null && session_.Connected == true)
                {
                    output_.WriteLine("Session already connected!");
                }
                else
                {
                    output_.WriteLine("Connecting to... {0}", serverUrl);

                    // Get the endpoint by connecting to server's discovery endpoint.
                    // Try to find the first endopint with security.
                    EndpointDescription endpointDescription = Discover.SelectEndpoint(configuration_, serverUrl, useSecurity);
                    EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration_);
                    ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                    // Create the session
                    var session = await Technosoftware.UaClient.Session.CreateAsync(
                        configuration_,
                        endpoint,
                        false,
                        false,
                        configuration_.ApplicationName,
                        SessionLifeTime,
                        UserIdentity,
                        null
                    ).ConfigureAwait(false);

                    // Assign the created session
                    if (session != null && session.Connected)
                    {
                        session_ = session;

                        // override keep alive interval
                        session_.KeepAliveInterval = KeepAliveInterval;

                        // set up keep alive callback.
                        session_.SessionKeepAliveEvent += OnSessionKeepAlive
                            ;
                    }

                    // Session created successfully.
                    if (verbose_)
                    {
                        output_.WriteLine("New Session Created with SessionName = {0}", session_.SessionName);
                    }
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
                if (session_ != null)
                {
                    output_.WriteLine("Disconnecting...");

                    lock (lock_)
                    {
                        session_.SessionKeepAliveEvent -= OnSessionKeepAlive;
                        reconnectHandler_?.Dispose();
                    }

                    session_.Close();
                    session_.Dispose();
                    session_ = null;

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

        /// <summary>
        /// Handles a keep alive event from a session and triggers a reconnect if necessary.
        /// </summary>
        private void OnSessionKeepAlive(object sender, SessionKeepAliveEventArgs e)
        {
            try
            {
                Session session = (Session)sender;

                // check for events from discarded sessions.
                if (!Object.ReferenceEquals(session, session_))
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

                    lock (lock_)
                    {
                        if (reconnectHandler_ == null)
                        {
                            Utils.LogInfo("KeepAlive status {0}, reconnecting in {1}ms.", e.Status, ReconnectPeriod);
                            output_.WriteLine("--- RECONNECTING {0} ---", e.Status);
                            reconnectHandler_ = new SessionReconnectHandler(true);
                            reconnectHandler_.BeginReconnect(session_, ReconnectPeriod, Client_ReconnectComplete);
                        }
                        else
                        {
                            Utils.LogInfo("KeepAlive status {0}, reconnect in progress.", e.Status);
                        }
                    }

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
        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!Object.ReferenceEquals(sender, reconnectHandler_))
            {
                return;
            }

            lock (lock_)
            {
                // if session recovered, Session property is null
                if (reconnectHandler_.Session != null)
                {
                    session_ = reconnectHandler_.Session as Session;
                }

                reconnectHandler_.Dispose();
                reconnectHandler_ = null;
            }

            output_.WriteLine("--- RECONNECTED ---");
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Handles the certificate validation event.
        /// This event is triggered every time an untrusted certificate is received from the server.
        /// </summary>
        protected virtual void OnCertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            bool certificateAccepted = false;

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
        private object lock_ = new object();
        private ApplicationConfiguration configuration_;        
        private SessionReconnectHandler reconnectHandler_;
        private Session session_;
        private readonly TextWriter output_;
        private readonly bool verbose_;
        #endregion
    }
}