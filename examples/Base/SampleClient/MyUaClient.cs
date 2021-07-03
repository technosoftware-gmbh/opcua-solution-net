#region Copyright (c) 2021 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2021 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Threading.Tasks;

using Opc.Ua;
using Technosoftware.UaClient;
using Technosoftware.UaConfiguration;
#endregion

namespace SampleCompany.SampleClient
{
    public class MyUaClient
    {
        #region Enumerations
        public enum ExitCode
        {
            Ok = 0,
            ErrorCreateApplication = 0x11,
            ErrorDiscoverEndpoints = 0x12,
            ErrorCreateSession = 0x13,
            ErrorReadServerStatus = 0x14,
            ErrorCreateSubscription = 0x15,
            ErrorMonitoredItem = 0x16,
            ErrorAddSubscription = 0x17,
            ErrorRunning = 0x18,
            ErrorNoKeepAlive = 0x30,
            ErrorInvalidCommandLine = 0x100
        }
        #endregion

        #region Properties

        /// <summary>
        /// The error code can be used to evaluate the step an error happened.
        /// </summary>
        public ExitCode ErrorCode { get; private set; } = ExitCode.Ok;

        /// <summary>
        /// Gets or sets wether detailed output should be written to console or not.
        /// </summary>
        /// <remarks>if set to <c>true</c> detailed output is written to the console.</remarks>
        public bool Verbose { get; set; } = true;

        /// <summary>
        /// Gets or sets wether the server application certificate should be auto accepted or not.
        /// </summary>
        /// <remarks>if set to <c>true</c> the server application certificate will be automatically accepted.</remarks>
        public bool AutoAccept { get; set; }

        /// <summary>
        /// Gets or sets the server URL. Default is "opc.tcp://localhost:55555/SampleServer"
        /// </summary>
        public string EndpointServerUrl { get; set; } = "opc.tcp://localhost:55555/SampleServer";

        /// <summary>
        /// Gets or sets the flag wether a secure connection should be establised or not. Default is true.
        /// </summary>
        /// <remarks>if set to <c>true</c> select an endpoint that uses security.</remarks>
        public bool UseSecurity { get; set; } = true;

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public ApplicationConfiguration Configuration { get; private set; }

        public Session Session { get; private set; }
        #endregion

        #region Constructors, Destructor, Initialization
        public MyUaClient(
            string applicationConfigurationFile)
        {
            applicationConfigurationFile_ = applicationConfigurationFile;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the application configuration.
        /// </summary>
        public async Task<ApplicationConfiguration> CreateApplicationAsync()
        {
            try
            {
                ApplicationInstance application = new ApplicationInstance { ApplicationType = ApplicationType.Client };

                // Load the Application Configuration
                Configuration = await application.LoadApplicationConfigurationAsync(applicationConfigurationFile_, silent: true).ConfigureAwait(false);

                // check the application certificate.
                var haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime).ConfigureAwait(false);

                if (haveAppCertificate)
                {
                    Configuration.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(Configuration.SecurityConfiguration.ApplicationCertificate.Certificate);
                    Configuration.CertificateValidator.CertificateValidation += OnCertificateValidation;
                }
                else
                {
                    UseSecurity = false;
                    Console.WriteLine("WARN: missing application certificate, using unsecured connection.");
                }

                ErrorCode = ExitCode.Ok;
                return Configuration;
            }
            catch (Exception e)
            {
                // Log Error
                Console.WriteLine($"Create Application Error : {e.Message}");
                ErrorCode = ExitCode.ErrorCreateApplication;
                return Configuration;
            }
        }

        /// <summary>
        /// Creates a session with the UA server
        /// </summary>
        public async Task<bool> ConnectSessionAsync()
        {
            try
            {
                if (Session != null && Session.Connected)
                {
                    Console.WriteLine("Session already connected!");
                }
                else
                {
                    Console.WriteLine("Connecting...");

                    // Get the endpoint by connecting to server's discovery endpoint.
                    // Try to find the first endopint without security.
                    EndpointDescription selectedEndpoint = Discover.SelectEndpoint(EndpointServerUrl, UseSecurity);
                    if (selectedEndpoint == null)
                    {
                        Console.WriteLine($"Selecting an Endpoint failed.");
                        ErrorCode = ExitCode.ErrorDiscoverEndpoints;
                        return false;
                    }
                    EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(Configuration);
                    ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

                    if (Verbose)
                    {
                        Console.WriteLine("    Selected endpoint uses: {0}", selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1));
                    }
                    // Create the session
                    Session session = await Session.CreateAsync(
                        Configuration,
                        endpoint,
                        false,
                        false,
                        Configuration.ApplicationName,
                        30 * 60 * 1000,
                        new UserIdentity(),
                        null
                    );

                    // Assign the created session
                    if (session != null && session.Connected)
                    {
                        Session = session;
                    }

                    // Session created successfully.
                    if (Session != null && Verbose)
                    {
                        Console.WriteLine($"    New Session Created with SessionName = {Session.SessionName}");
                    }
                }

                ErrorCode = ExitCode.Ok;
                return true;
            }
            catch (Exception e)
            {
                // Log Error
                Console.WriteLine($"Create Session Error : {e.Message}");
                ErrorCode = ExitCode.ErrorCreateSession;
                return false;
            }
        }

        /// <summary>
        /// Read some values from the server status
        /// </summary>
        public void ReadServerStatus()
        {
            if (Session == null || Session.Connected == false)
            {
                Console.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Build a list of nodes to be read
                ReadValueIdCollection nodesToRead = new ReadValueIdCollection()
                {
                    // Value of ServerStatus
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus, AttributeId = Attributes.Value },
                    // BrowseName of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.BrowseName },
                    // Value of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.Value }
                };

                // Read the node attributes
                Console.WriteLine("Reading nodes...");

                // Call Read Service
                Session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out DataValueCollection resultsValues,
                    out _);

                // Validate the results
                ClientBase.ValidateResponse(resultsValues, nodesToRead);

                // Display the results.
                foreach (DataValue result in resultsValues)
                {
                    Console.WriteLine("   Read Value = {0} , StatusCode = {1}", result.Value, result.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read Nodes Error : {ex.Message}.");
                ErrorCode = ExitCode.ErrorReadServerStatus;
            }
        }
        #endregion
        
        #region Asynchronous related handlers
        private void OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = AutoAccept;
                if (AutoAccept)
                {
                    Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
                }
                else
                {
                    Console.WriteLine("Rejected Certificate: {0}", e.Certificate.Subject);
                }
            }
        }
        #endregion

        #region Fields
        private readonly string applicationConfigurationFile_;
        #endregion      
    }
}