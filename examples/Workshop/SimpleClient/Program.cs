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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Mono.Options;

using Opc.Ua;

using Technosoftware.UaClient;
using Technosoftware.UaConfiguration;

using OptionSet = Mono.Options.OptionSet;
#endregion

namespace Technosoftware.SimpleClient
{

    #region Enumerations
    public enum ExitCode
    {
        Ok = 0,
        ErrorCreateApplication = 0x11,
        ErrorDiscoverEndpoints = 0x12,
        ErrorCreateSession = 0x13,
        ErrorCreateSubscription = 0x15,
        ErrorMonitoredItem = 0x16,
        ErrorAddSubscription = 0x17,
        ErrorRunning = 0x18,
        ErrorNoKeepAlive = 0x30,
        ErrorInvalidCommandLine = 0x100
    }
    #endregion

    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Technosoftware {0} OPC UA Simple Client", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

            // command line options
            var showHelp = false;
            var stopTimeout = Timeout.Infinite;
            var autoAccept = false;
            var noBrowse = false;
            var verbose = false;
            var securityNone = false;
            string username = null;
            string password = null;
            string reverseConnectUrlString = null;
            Uri reverseConnectUrl = null;

            var options = new OptionSet {
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "t|timeout=", "the number of seconds until the client stops.", (int t) => stopTimeout = t },
                { "b|nobrowse", "Do not browse the address space of the server.", n => noBrowse = n != null},
                { "s|securitynone", "Do not use security for connection.", s => securityNone = s != null},
                { "u|username=", "Username to access server.", n => username = n},
                { "p|password=", "Password to access server.", n => password = n},
                { "v|verbose", "Verbose output.", v => verbose = v != null},
                { "rc|reverseconnect=", "Connect using the reverse connection.", url => reverseConnectUrlString = url},
            };

            IList<string> extraArgs = null;
            try
            {
                extraArgs = options.Parse(args);
                if (extraArgs.Count > 1)
                {
                    foreach (var extraArg in extraArgs)
                    {
                        Console.WriteLine("Error: Unknown option: {0}", extraArg);
                        showHelp = true;
                    }
                }
                if (reverseConnectUrlString != null)
                {
                    reverseConnectUrl = new Uri(reverseConnectUrlString);
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                showHelp = true;
            }

            if (showHelp)
            {
                // show some app description message
                Console.WriteLine("Usage: dotnet Technosoftware.SimpleClient.dll [OPTIONS] [ENDPOINTURL]");
                Console.WriteLine();

                // output the options
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return (int)ExitCode.ErrorInvalidCommandLine;
            }

            string endpointUrl;
            if (extraArgs == null || extraArgs.Count == 0)
            {
                // use Technosoftware OPC UA Simple Server
                endpointUrl = "opc.tcp://localhost:55550/TechnosoftwareSimpleServer";
            }
            else
            {
                endpointUrl = extraArgs[0];
            }

            var client = new MySampleClient(endpointUrl, autoAccept, stopTimeout)
            {
                Verbose = verbose,
                BrowseAddressSpace = !noBrowse,
                Username = username,
                Password = password,
                SecurityNone = securityNone,
                ReverseConnectUri = reverseConnectUrl
            };
            return (int)client.Run();
        }
    }

    public class MySampleClient
    {
        #region Properties
        public int ReconnectPeriod { get; set; } = 10;
        public ExitCode ExitCode { get; set; }
        public bool Verbose { get; set; }
        public bool BrowseAddressSpace { get; set; }
        public bool SecurityNone { get; set; } = true;
        public String Username { get; set; }
        public String Password { get; set; }
        public Uri ReverseConnectUri { get; set; }
        #endregion

        #region Constructors, Destructor, Initialization
        public MySampleClient(
            string endpointUrl,
            bool autoAccept,
            int stopTimeout)
        {
            endpointUrl_ = endpointUrl;
            autoAccept_ = autoAccept;
            clientRunTime_ = stopTimeout <= 0 ? Timeout.Infinite : stopTimeout * 1000;
        }

        ~MySampleClient()
        {
            quitEvent_?.Dispose();
            subscription_?.Dispose();
            reverseConnectManager_?.Dispose();
        }
        #endregion

        #region Public Methods
        public ExitCode Run()
        {
            Session session;

            try
            {
                session = ConsoleSampleClient().Result;
            }
            catch (Exception exception)
            {
                Utils.Trace("Exception:" + exception.Message);
                Console.WriteLine("Exception: {0}", exception.Message);
                return ExitCode;
            }

            quitEvent_ = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    quitEvent_.Set();
                    eArgs.Cancel = true;
                };
            }
            catch (Exception exception)
            {
                Utils.Trace("Exception:" + exception.Message);
                Console.WriteLine("Exception: {0}", exception.Message);
            }

            // Test the session reconnect handler
            var eventResult = quitEvent_.WaitOne(5000);
            if (!eventResult)
            {
                Console.WriteLine(" --- Start simulated reconnect... --- ");
                reconnectHandler_ = new SessionReconnectHandler();
                if (reverseConnectManager_ != null)
                {
                    reconnectHandler_.BeginReconnect(session, reverseConnectManager_, 1000, OnServerReconnectComplete);
                }
                else
                {
                    reconnectHandler_.BeginReconnect(session, 1000, OnServerReconnectComplete);
                }
            }

            // wait for timeout or Ctrl-C
            quitEvent_.WaitOne(clientRunTime_);

            // return error conditions
            if (session.KeepAliveStopped)
            {
                ExitCode = ExitCode.ErrorNoKeepAlive;
                return ExitCode;
            }

            ExitCode = ExitCode.Ok;
            return ExitCode;
        }
        #endregion

        #region Task handling the different OPC UA sample cases
        private async Task<Session> ConsoleSampleClient()
        {
            var application = new ApplicationInstance { ApplicationType = ApplicationType.Client };

            #region Create an Application Configuration
            Console.WriteLine(" 1 - Create an Application Configuration.");
            ExitCode = ExitCode.ErrorCreateApplication;

            // Load the Application Configuration and use the specified config section "Technosoftware.SimpleClient"
            var config = await application.LoadConfigurationAsync("Technosoftware.SimpleClient");

            // check the application certificate.
            var haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime);

            reverseConnectManager_ = null;
            if (ReverseConnectUri != null)
            {
                // start the reverse connection manager
                reverseConnectManager_ = new ReverseConnectManager();
                reverseConnectManager_.AddEndpoint(ReverseConnectUri);
                reverseConnectManager_.StartService(config);
            }

            if (haveAppCertificate)
            {
                config.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    autoAccept_ = true;
                }
                config.CertificateValidator.CertificateValidation += OnCertificateValidation;
            }
            else
            {
                Console.WriteLine("    WARN: missing application certificate, using unsecured connection.");
            }
            #endregion

            #region Discover endpoints
            Console.WriteLine(" 2 - Discover endpoints of {0}.", endpointUrl_);
            ExitCode = ExitCode.ErrorDiscoverEndpoints;
            EndpointDescription selectedEndpoint;
            if (reverseConnectManager_ == null)
            {
                selectedEndpoint = Discover.SelectEndpoint(endpointUrl_, haveAppCertificate && !SecurityNone, 15000);
            }
            else
            {
                Console.WriteLine("   Waiting for reverse connection.");
                var connection = await reverseConnectManager_.WaitForConnection(
                    new Uri(endpointUrl_), null, new CancellationTokenSource(60000).Token);
                if (connection == null)
                {
                    throw new ServiceResultException(StatusCodes.BadTimeout, "Waiting for a reverse connection timed out.");
                }
                selectedEndpoint = Discover.SelectEndpoint(config, connection, haveAppCertificate && !SecurityNone, 15000);
            }

            Console.WriteLine("    Selected endpoint uses: {0}",
                selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1));
            #endregion

            #region Create a session with OPC UA server
            Console.WriteLine(" 3 - Create a session with OPC UA server.");
            ExitCode = ExitCode.ErrorCreateSession;

            // create the user identity
            UserIdentity userIdentity;
            if (String.IsNullOrEmpty(Username) && String.IsNullOrEmpty(Password))
            {
                userIdentity = new UserIdentity(new AnonymousIdentityToken());
            }
            else
            {
                userIdentity = new UserIdentity(Username, Password);
            }

            // create worker session
            if (reverseConnectManager_ == null)
            {
                session_ = await CreateSessionAsync(config, selectedEndpoint, userIdentity).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine("   Waiting for reverse connection.");
                // Define the cancellation token.
                var source = new CancellationTokenSource(60000);
                var token = source.Token;
                try
                {
                    var connection = await reverseConnectManager_.WaitForConnection(
                        new Uri(endpointUrl_), null, token);
                    if (connection == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadTimeout,
                            "Waiting for a reverse connection timed out.");
                    }

                    session_ = await CreateSessionAsync(config, connection, selectedEndpoint, userIdentity)
                        .ConfigureAwait(false);
                }
                finally
                {
                    source.Dispose();
                }
            }

            // register keep alive handler
            session_.SessionKeepAliveEvent += OnSessionKeepAliveEvent;
            #endregion

            #region Browse the OPC UA Server
            Console.WriteLine(" 4 - Browse address space.");
            // Create the browser
            var browser = new Browser(session_)
            {
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ContinueUntilDone = false
            };

            // Browse from the RootFolder
            var references = browser.Browse(Objects.ObjectsFolder);

            GetElements(session_, browser, 0, references, Verbose);

            #endregion

            #region Read a single value
            Console.WriteLine(" 5 - Read a single value.");
            var simulatedDataValue = session_.ReadValue(simulatedDataNodeId_);
            Console.WriteLine("Node Value:" + simulatedDataValue.Value);
            #endregion

            #region Read multiple values
            Console.WriteLine(" 6 - Read multiple values.");
            // The input parameters of the ReadValues() method
            var variableIds = new List<NodeId>();
            var expectedTypes = new List<Type>();

            // Add a node to the list
            variableIds.Add(simulatedDataNodeId_);
            // Add an expected type to the list (null means we get the original type from the server)
            expectedTypes.Add(null);

            // Add another node to the list
            variableIds.Add(staticDataNodeId1_);
            // Add an expected type to the list (null means we get the original type from the server)
            expectedTypes.Add(null);

            // Add another node to the list
            variableIds.Add(staticDataNodeId2_);

            // Add an expected type to the list (null means we get the original type from the server)
            expectedTypes.Add(null);

            session_.ReadValues(variableIds, expectedTypes, out var values, out var errors);
            // write the result to the console.
            for (var i = 0; i < values.Count; i++)
            {
                Console.WriteLine("Status of Read of Node {0} is: {1}", variableIds[i].ToString(), errors[i]);
            }
            for (var i = 0; i < values.Count; i++)
            {
                Console.WriteLine("Value of Read of Node {0} is: Value: {1}", variableIds[i].ToString(), values[i]);
            }
            #endregion

            #region Read multiple values asynchronous
            Console.WriteLine(" 7 - Read multiple values asynchronous.");
            // start reading the value (setting a 10 second timeout).
            session_.BeginReadValues(
                variableIds,
                0,
                TimestampsToReturn.Both,
                OnReadComplete,
                new UserData { Session = session_, NodeIds = variableIds });
            #endregion

            #region Write a value
            Console.WriteLine(" 8 - Write a value.");
            short writeInt = 1234;

            Console.WriteLine("Write Value: " + writeInt);
            session_.WriteValue(staticDataNodeId1_, new DataValue(writeInt));

            // read it again to check the new value
            Console.WriteLine("Node Value (should be {0}): {1}", session_.ReadValue(staticDataNodeId1_).Value, writeInt);
            #endregion

            #region Write multiple values at once
            Console.WriteLine(" 9 - Write multiple values at once.");

            writeInt = 5678;
            var writeDouble = 1234.1234;

            var nodeIds = new List<NodeId>();
            var dataValues = new List<DataValue>();

            nodeIds.Add(staticDataNodeId1_);
            nodeIds.Add(staticDataNodeId2_);

            dataValues.Add(new DataValue(writeInt));
            dataValues.Add(new DataValue(writeDouble));

            Console.WriteLine("Write Values: {0} and {1}", writeInt, writeDouble);
            var statusCodes = session_.WriteValues(nodeIds, dataValues);

            Console.WriteLine("Returned status codes:");
            foreach (var statusCode in statusCodes)
            {
                Console.WriteLine("Status: {0}", statusCode.ToString());
            }

            // read it again to check the new value
            Console.WriteLine("Node Value (should be {0}): {1}", session_.ReadValue(staticDataNodeId1_).Value, writeInt);
            Console.WriteLine("Node Value (should be {0}): {1}", session_.ReadValue(staticDataNodeId2_).Value, writeDouble);
            #endregion

            #region Write multiple values asynchronous
            Console.WriteLine("10 - Write multiple values asynchronous.");

            // start writing the values.
            session_.BeginWriteValues(
                nodeIds,
                dataValues,
                OnWriteComplete,
                new UserData { Session = session_, NodeIds = nodeIds });
            #endregion

            #region Call a Method
            Console.WriteLine("11 - Call a Method.");
            INode node = session_.ReadNode(callHelloMethodNodeId_);


            if (node is MethodNode)
            {
                var methodId = callHelloMethodNodeId_;

                var objectId = methodsNodeId_;

                var inputArguments = new VariantCollection { new Variant("from Technosoftware") };

                var request = new CallMethodRequest { ObjectId = objectId, MethodId = methodId, InputArguments = inputArguments };

                var requests = new CallMethodRequestCollection { request };

                var responseHeader = session_.Call(
                    null,
                    requests,
                    out var results,
                    out var diagnosticInfos);

                if (StatusCode.IsBad(results[0].StatusCode))
                {
                    throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos,
                        responseHeader.StringTable));
                }

                Console.WriteLine("{0}", results[0].OutputArguments[0]);

            }

            #endregion

            #region Create a subscription with publishing interval of 1 second
            Console.WriteLine("12 - Create a subscription with publishing interval of 1 second.");
            ExitCode = ExitCode.ErrorCreateSubscription;
            subscription_ = new Subscription(session_.DefaultSubscription) { PublishingInterval = 1000 };
            #endregion

            #region Add all dynamic values and the server time to the subscription
            Console.WriteLine("13 - Add all dynamic values and the server time to the subscription.");
            ExitCode = ExitCode.ErrorMonitoredItem;
            var list = new List<MonitoredItem> {
                new MonitoredItem(subscription_.DefaultItem)
                {
                    DisplayName = "ServerStatusCurrentTime", StartNodeId = "i="+Variables.Server_ServerStatus_CurrentTime
                }
            };
            list.ForEach(i => i.MonitoredItemNotificationEvent += OnNotification);

            var newItem = new MonitoredItem(subscription_.DefaultItem)
            {
                DisplayName = "Simulated Data Value",
                StartNodeId = new NodeId(simulatedDataNodeId_)
            };
            newItem.MonitoredItemNotificationEvent += OnMonitoredItemNotificationEvent;
            list.Add(newItem);

            subscription_.AddItems(list);
            #endregion

            #region Add the subscription to the session
            Console.WriteLine("14 - Add the subscription to the session.");
            ExitCode = ExitCode.ErrorAddSubscription;
            session_.AddSubscription(subscription_);
            subscription_.Create();
            #endregion

            #region Running...Press Ctrl-C to exit...
            Console.WriteLine("15 - Running...Press Ctrl-C to exit...");
            ExitCode = ExitCode.ErrorRunning;
            #endregion

            return session_;
        }
        #endregion

        #region KeepAlive and ReConnect handling
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

        private void OnServerReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!ReferenceEquals(sender, reconnectHandler_))
            {
                return;
            }

            if (reconnectHandler_ != null)
            {
                session_ = reconnectHandler_.Session;
                reconnectHandler_.Dispose();
                reconnectHandler_ = null;
                Console.WriteLine("--- RECONNECTED ---");
            }
        }
        #endregion

        #region Helper Methods

        private Task<Session> CreateSessionAsync(
            ApplicationConfiguration config,
            EndpointDescription selectedEndpoint,
            IUserIdentity userIdentity)
        {
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            return Session.CreateAsync(config, endpoint, false, "Technosoftware OPC UA Simple Client", 60000, userIdentity, null);
        }

        private Task<Session> CreateSessionAsync(
            ApplicationConfiguration config,
            ITransportWaitingConnection connection,
            EndpointDescription selectedEndpoint,
            IUserIdentity userIdentity)
        {
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            return Session.CreateAsync(config, connection, endpoint, false, false, "Technosoftware OPC UA Simple Client", 60000, userIdentity, null);
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

        #region Asynchronous related classes and Handlers
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

        private void OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = autoAccept_;
                if (autoAccept_)
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
        private readonly NodeId simulatedDataNodeId_ = new NodeId("ns=2;s=Scalar_Simulation_Number");
        private readonly NodeId staticDataNodeId1_ = new NodeId("ns=2;s=Scalar_Static_Integer");
        private readonly NodeId staticDataNodeId2_ = new NodeId("ns=2;s=Scalar_Static_Double");

        private readonly NodeId methodsNodeId_ = new NodeId("ns=2;s=Methods");
        private readonly NodeId callHelloMethodNodeId_ = new NodeId("ns=2;s=Methods_Hello");

        private Session session_;
        private Subscription subscription_;
        private SessionReconnectHandler reconnectHandler_;
        private readonly string endpointUrl_;
        private bool autoAccept_;
        private readonly int clientRunTime_;
        private ManualResetEvent quitEvent_;
        private ReverseConnectManager reverseConnectManager_;
        #endregion
    }
}
