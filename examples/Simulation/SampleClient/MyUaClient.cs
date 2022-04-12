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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
using Technosoftware.UaClient;
using Technosoftware.UaConfiguration;
#endregion

namespace SampleCompany.SampleClient
{
    /// <summary>The UA client sample functionality.</summary>
    public class MyUaClient
    {
        #region Enumerations
        /// <summary>Values that represent exit codes.</summary>
        public enum ExitCode
        {
            /// <summary>An enum constant representing the ok option.</summary>
            Ok = 0,
            /// <summary>An enum constant representing the error create application option.</summary>
            ErrorCreateApplication = 0x11,
            /// <summary>An enum constant representing the error discover endpoints option.</summary>
            ErrorDiscoverEndpoints = 0x12,
            /// <summary>An enum constant representing the error create session option.</summary>
            ErrorCreateSession = 0x13,
            /// <summary>An enum constant representing the error read server status option.</summary>
            ErrorReadServerStatus = 0x14,
            /// <summary>An enum constant representing the error create subscription option.</summary>
            ErrorCreateSubscription = 0x15,
            /// <summary>An enum constant representing the error monitored item option.</summary>
            ErrorMonitoredItem = 0x16,
            /// <summary>An enum constant representing the error add subscription option.</summary>
            ErrorAddSubscription = 0x17,
            /// <summary>An enum constant representing the error running option.</summary>
            ErrorRunning = 0x18,
            /// <summary>An enum constant representing the error no keep alive option.</summary>
            ErrorNoKeepAlive = 0x30,
            /// <summary>An enum constant representing the error invalid command line option.</summary>
            ErrorInvalidCommandLine = 0x100
        }
        #endregion

        #region Public Properties
        /// <summary>The error code can be used to evaluate the step an error happened.</summary>
        ///
        /// <value>The error code.</value>
        public ExitCode ErrorCode { get; private set; } = ExitCode.Ok;

        /// <summary>Gets or sets whether detailed output should be written to console or not.</summary>
        ///
        /// <value>if set to <c>true</c> detailed output is written to the console, false if not.</value>
        public bool Verbose { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the server application certificate should be auto accepted or not.
        /// </summary>
        ///
        /// <value>if set to <c>true</c> the server application certificate will be automatically accepted, false if not.</value>
        public bool AutoAccept { get; set; }

        /// <summary>
        /// Gets or sets the server URL. Default is "opc.tcp://localhost:55555/SampleServer".
        /// </summary>
        ///
        /// <value>The endpoint server URL.</value>
        public string EndpointServerUrl { get; set; } = "opc.tcp://localhost:55555/SampleServer";

        /// <summary>
        /// Gets or sets the flag whether a secure connection should be established or not. Default is true.
        /// </summary>
        ///
        /// <value>if set to <c>true</c> select an endpoint that uses security, false if not.</value>
        public bool UseSecurity { get; set; } = true;

        /// <summary>Gets or sets the user name.</summary>
        ///
        /// <value>The user name.</value>
        public String Username { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
        public String Password { get; set; }

        /// <summary>Gets or sets the reconnect period in seconds.</summary>
        ///
        /// <value>The reconnect period in seconds.</value>
        public int ReconnectPeriod { get; set; } = 10;

        /// <summary>Gets or sets URI of the reverse connect.</summary>
        ///
        /// <value>The reverse connect URI.</value>
        public Uri ReverseConnectUri { get; set; }

        /// <summary>Gets or sets the application configuration.</summary>
        ///
        /// <value>The configuration.</value>
        public ApplicationConfiguration Configuration { get; private set; }

        /// <summary>Gets or sets the session.</summary>
        ///
        /// <value>The session.</value>
        public Session Session { get; private set; }
        #endregion

        #region Constructors, Destructor, Initialization
        /// <summary>Constructor.</summary>
        ///
        /// <param name="applicationConfigurationFile">The application configuration file.</param>
        public MyUaClient(
            string applicationConfigurationFile)
        {
            applicationConfigurationFile_ = applicationConfigurationFile;
        }

        ~MyUaClient()
        {
            reverseConnectManager_?.Dispose();
        }
        #endregion

        #region Public Methods
        /// <summary>Loads the application configuration.</summary>
        ///
        /// <returns>The created application configuration.</returns>
        public async Task<ApplicationConfiguration> CreateApplicationAsync()
        {
            try
            {
                ApplicationInstance application = new ApplicationInstance { ApplicationType = ApplicationType.Client };

                // Load the Application Configuration
                Configuration = await application.LoadApplicationConfigurationAsync(applicationConfigurationFile_, silent: true).ConfigureAwait(false);

                // check the application certificate.
                var haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime).ConfigureAwait(false);

                reverseConnectManager_ = null;
                if (ReverseConnectUri != null)
                {
                    // start the reverse connection manager
                    reverseConnectManager_ = new ReverseConnectManager();
                    reverseConnectManager_.AddEndpoint(ReverseConnectUri);
                    reverseConnectManager_.StartService(Configuration);
                }

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

        /// <summary>Creates a session with the UA server.</summary>
        ///
        /// <returns>if set to <c>true</c> the session could be created, false if not.</returns>
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
                    EndpointDescription selectedEndpoint = Discover.SelectEndpoint(EndpointServerUrl, UseSecurity, 15000);
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

        /// <summary>Read some values from the server status node.</summary>
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

        /// <summary>Read value from given node.</summary>
        ///
        /// <param name="nodeId">Identifier for the node.</param>
        public void ReadSingleValue(string nodeId)
        {
            NodeId simulatedDataNodeId = new NodeId(nodeId);
            Console.WriteLine($"Read a single value from node {nodeId}.");
            var simulatedDataValue = Session.ReadValue(simulatedDataNodeId);
            Console.WriteLine($"   Node {nodeId} Value = {simulatedDataValue.Value} StatusCode = {simulatedDataValue.StatusCode}.");
        }

        public void ReadMultipleValues(List<string> nodeIds)
        {
            var variableIds = new List<NodeId>();
            var expectedTypes = new List<Type>();
            Console.WriteLine($"Read multiple values from different nodes.");
            foreach (var nodeNames in nodeIds)
            {
                NodeId nodeId = new NodeId(nodeNames);
                variableIds.Add(nodeId);
                // Add an expected type to the list (null means we get the original type from the server)
                expectedTypes.Add(null);
            }

            Session.ReadValues(variableIds, expectedTypes, out var values, out var errors);
            // write the result to the console.
            for (var i = 0; i < values.Count; i++)
            {
                Console.WriteLine($"   Node {variableIds[i]} Value = {values[i]} StatusCode = {errors[i]}.");
            }
        }

        public void ReadMultipleValuesAsynchronous(List<string> nodeIds)
        {
            var variableIds = new List<NodeId>();

            Console.WriteLine("Read multiple values asynchronous."); 
            foreach (var nodeNames in nodeIds)
            {
                NodeId nodeId = new NodeId(nodeNames);
                variableIds.Add(nodeId);
            }

            // start reading the value (setting a 10 second timeout).
            Session.BeginReadValues(
                variableIds,
                0,
                TimestampsToReturn.Both,
                OnReadComplete,
                new UserData { Session = Session, NodeIds = variableIds });
        }

        /// <summary>Read some values from the server status.</summary>
        public void SimulateReconnect()
        {
            Console.WriteLine("--- SIMULATE RECONNECT --- ");
            reconnectHandler_ = new SessionReconnectHandler();
            if (reverseConnectManager_ != null)
            {
                reconnectHandler_.BeginReconnect(Session, reverseConnectManager_, 1000, OnServerReconnectComplete);
            }
            else
            {
                reconnectHandler_.BeginReconnect(Session, 1000, OnServerReconnectComplete);
            }
        }

        /// <summary>Browses the address space of the UA server.</summary>
        public void Browse()
        {
            Console.WriteLine("Browse address space.");

            // Create the browser
            var browser = new Browser(Session)
            {
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ContinueUntilDone = false
            };

            // Browse from the RootFolder
            var references = browser.Browse(Objects.ObjectsFolder);

            GetElements(Session, browser, 0, references, Verbose);
        }
        #endregion

        #region Asynchronous related classes and Handlers
        /// <summary>Raises the certificate validation event.</summary>
        ///
        /// <param name="sender">Source of the event. </param>
        /// <param name="e">     Event information to send to registered event handlers. </param>
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

        /// <summary>
        /// A object used to pass state with an asynchronous write call.
        /// </summary>
        private class UserData
        {
            public Session Session { get; set; }
            public List<NodeId> NodeIds { get; set; }
        }

        /// <summary>
        /// Finishes an asynchronous read request.
        /// </summary>
        private void OnReadComplete(IAsyncResult result)
        {
            // get the session used to send the request which was passed as the userData in the BeginWriteValues call.
            var userData = (UserData)result.AsyncState;

            if (userData == null)
            {
                Console.WriteLine("No user data provided in OnReadComplete().");
                return;
            }

            try
            {
                // get the results.
                var results = userData.Session.EndReadValues(result);

                // write the result to the console.
                for (var i = 0; i < results.Count; i++)
                {
                    Console.WriteLine("Status of Read of Node {0} is: {1}", userData.NodeIds[i].ToString(), results[i].Value);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error in OnReadComplete(): {0}", exception.Message);
            }
        }

        /// <summary>
        /// Finishes an asynchronous read request.
        /// </summary>
        private void OnWriteComplete(IAsyncResult result)
        {
            // get the session used to send the request which was passed as the userData in the BeginWriteValues call.
            var userData = (UserData)result.AsyncState;

            if (userData == null)
            {
                Console.WriteLine("No user data provided in OnWriteComplete().");
                return;
            }

            try
            {
                // get the results.
                var results = userData.Session.EndWriteValues(result);

                // write the result to the console.
                for (var i = 0; i < results.Count; i++)
                {
                    Console.WriteLine("Status of Write to Node {0} is: {1}", userData.NodeIds[i].ToString(), results[i].ToString());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error in OnWriteComplete(): {0}", exception.Message);
            }
        }

        private void OnNotification(object sender, MonitoredItemNotificationEventArgs e)
        {
            var item = sender as MonitoredItem;
            if (item == null)
            {
                return;
            }
            foreach (var value in item.DequeueValues())
            {
                Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
            }
        }

        private void OnMonitoredItemNotificationEvent(object sender, MonitoredItemNotificationEventArgs e)
        {
            var item = sender as MonitoredItem;
            if (item == null)
            {
                return;
            }
            foreach (var value in item.DequeueValues())
            {
                Console.WriteLine("{0}: {1}, {2}", item.DisplayName, value.SourceTimestamp, value.StatusCode);
                if (Verbose)
                {
                    Console.WriteLine(value);
                }
            }
        }
        #endregion

        #region KeepAlive and ReConnect handling
        /// <summary>Raises the session keep alive event.</summary>
        ///
        /// <param name="sender">Source of the event. </param>
        /// <param name="e">     Event information to send to registered event handlers. </param>
        private void OnSessionKeepAliveEvent(object sender, SessionKeepAliveEventArgs e)
        {
            if (sender is Session session && e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                Console.WriteLine("{0} {1}/{2}", e.Status, session.OutstandingRequestCount, session.DefunctRequestCount);

                if (reconnectHandler_ == null)
                {
                    Console.WriteLine("--- RECONNECTING ---");
                    reconnectHandler_ = new SessionReconnectHandler();
                    reconnectHandler_.BeginReconnect(session, ReconnectPeriod * 1000, OnServerReconnectComplete);
                }
            }
        }

        /// <summary>Raises the server reconnect complete event.</summary>
        ///
        /// <param name="sender">Source of the event. </param>
        /// <param name="e">     Event information to send to registered event handlers. </param>
        private void OnServerReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!ReferenceEquals(sender, reconnectHandler_))
            {
                return;
            }

            if (reconnectHandler_ != null)
            {
                Session = reconnectHandler_.Session;
                reconnectHandler_.Dispose();
                reconnectHandler_ = null;
                Console.WriteLine("--- RECONNECTED ---");
            }
        }
        #endregion

        #region Private Methods (Browse related)
        /// <summary>
        /// Gets all elements for the specified references
        /// </summary>
        /// <param name="session">The session to use</param>
        /// <param name="browser">The browser to use</param>
        /// <param name="level">The level</param>
        /// <param name="references">The references to browse</param>
        /// <param name="verbose">If true the address space will be printed out to the console; otherwise not</param>
        private static void GetElements(Session session, Browser browser, uint level, ReferenceDescriptionCollection references, bool verbose)
        {
            var spaces = new StringBuilder();
            for (var i = 0; i <= level; i++)
            {
                spaces.Append(i);
                spaces.Append("   ");
            }

            // Iterate through the references and print the variables
            foreach (var reference in references)
            {
                // make sure the type definition is in the cache.
                session.NodeCache.Find(reference.ReferenceTypeId);

                switch (reference.NodeClass)
                {
                    case NodeClass.Object:
                        if (verbose)
                        {
                            Console.WriteLine(spaces + "+ " + reference.DisplayName);
                        }
                        break;

                    default:
                        if (verbose)
                        {
                            Console.WriteLine(spaces + "- " + reference.DisplayName);
                        }
                        break;
                }
                var subReferences = browser.Browse((NodeId)reference.NodeId);
                level += 1;
                GetElements(session, browser, level, subReferences, verbose);
                level -= 1;
            }
        }
        #endregion

        #region Fields
        /// <summary>(Immutable) the application configuration file.</summary>
        private readonly string applicationConfigurationFile_;
        /// <summary>Manager for reverse connect.</summary>
        private ReverseConnectManager reverseConnectManager_;
        /// <summary>The reconnect handler.</summary>
        private SessionReconnectHandler reconnectHandler_;
        #endregion      
    }
}