#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections;
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
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UAClient class.
        /// </summary>
        public MyUaClient(ApplicationConfiguration configuration, TextWriter writer, Action<IList, IList> validateResponse, bool verbose)
        {
            validateResponse_ = validateResponse;
            output_ = writer;
            verbose_ = verbose;
            configuration_ = configuration;
            configuration_.CertificateValidator.CertificateValidation += OnCertificateValidation;
            reverseConnectManager_ = null;
        }

        /// <summary>
        /// Initializes a new instance of the MyUaClient class for reverse connections.
        /// </summary>
        public MyUaClient(ApplicationConfiguration configuration, ReverseConnectManager reverseConnectManager, TextWriter writer, Action<IList, IList> validateResponse, bool verbose)
        {
            validateResponse_ = validateResponse;
            output_ = writer;
            verbose_ = verbose;
            configuration_ = configuration;
            configuration_.CertificateValidator.CertificateValidation += OnCertificateValidation;
            reverseConnectManager_ = reverseConnectManager;
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
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Action used 
        /// </summary>
        Action<IList, IList> ValidateResponse => validateResponse_;

        /// <summary>
        /// Gets the client session.
        /// </summary>
        public IUaSession Session => session_;

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

        /// <summary>
        /// The file to use for log output.
        /// </summary>
        public string LogFile { get; set; }
        #endregion

        #region Public Methods
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
                if (session_ != null && session_.Connected)
                {
                    output_.WriteLine("Session already connected!");
                }
                else
                {
                    ITransportWaitingConnection connection = null;
                    EndpointDescription endpointDescription = null;
                    if (reverseConnectManager_ != null)
                    {
                        output_.WriteLine("Waiting for reverse connection to.... {0}", serverUrl);
                        do
                        {
                            using (var cts = new CancellationTokenSource(30_000))
                            using (var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(ct, cts.Token))
                            {
                                connection = await reverseConnectManager_.WaitForConnectionAsync(new Uri(serverUrl), null, linkedCTS.Token).ConfigureAwait(false);
                                if (connection == null)
                                {
                                    throw new ServiceResultException(StatusCodes.BadTimeout, "Waiting for a reverse connection timed out.");
                                }
                                if (endpointDescription == null)
                                {
                                    output_.WriteLine("Discover reverse connection endpoints....");
                                    endpointDescription = Discover.SelectEndpoint(configuration_, connection, useSecurity);
                                    connection = null;
                                }
                            }
                        } while (connection == null);
                    }
                    else
                    {
                        output_.WriteLine("Connecting to... {0}", serverUrl);
                        endpointDescription = Discover.SelectEndpoint(configuration_, serverUrl, useSecurity);
                    }

                    // Get the endpoint by connecting to server's discovery endpoint.
                    // Try to find the first endopint with security.
                    var endpointConfiguration = EndpointConfiguration.Create(configuration_);
                    var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                    TraceableSessionFactory sessionFactory = TraceableSessionFactory.Instance;

                    // Create the session
                    IUaSession session = await sessionFactory.CreateAsync(
                        configuration_,
                        connection,
                        endpoint,
                        connection == null,
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

                        // support transfer
                        session_.DeleteSubscriptionsOnClose = false;
                        session_.TransferSubscriptionsOnReconnect = true;

                        // set up keep alive callback.
                        session_.SessionKeepAliveEvent += OnSessionKeepAlive;

                        // prepare a reconnect handler
                        reconnectHandler_ = new SessionReconnectHandler(true, ReconnectPeriodExponentialBackoff);
                    }

                    // Session created successfully.
                    output_.WriteLine("New Session Created with SessionName = {0}", session_.SessionName);
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
                        reconnectHandler_ = null;
                    }

                    _ = session_.Close();
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
                var session = (Session)sender;

                // check for events from discarded sessions.
                if (!session_.Equals(session))
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

                    SessionReconnectHandler.ReconnectState state = reconnectHandler_.BeginReconnect(session_, reverseConnectManager_, ReconnectPeriod, OnReconnectComplete);
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
                    if (!ReferenceEquals(session_, reconnectHandler_.Session))
                    {
                        output_.WriteLine("--- RECONNECTED TO NEW SESSION --- {0}", reconnectHandler_.Session.SessionId);
                        IUaSession session = session_;
                        session_ = reconnectHandler_.Session;
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
        private readonly ReverseConnectManager reverseConnectManager_;
        private readonly ApplicationConfiguration configuration_;
        private SessionReconnectHandler reconnectHandler_;
        private IUaSession session_;
        private readonly TextWriter output_;
        private readonly bool verbose_;
        private readonly Action<IList, IList> validateResponse_;
        #endregion
    }
}
