#region Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved

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

namespace Technosoftware.ModelDesignClient
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
            Console.WriteLine("Technosoftware {0} OPC UA ModelDesign Client", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

            // command line options
            var showHelp = false;
            var stopTimeout = Timeout.Infinite;
            var autoAccept = false;
            var noBrowse = false;
            var verbose = false;
            string username = null;
            string password = null;

            var options = new OptionSet {
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "t|timeout=", "the number of seconds until the client stops.", (int t) => stopTimeout = t },
                { "b|nobrowse", "Do not browse the address space of the server.", n => noBrowse = n != null},
                { "u|username=", "Username to access server.", n => username = n},
                { "p|password=", "Password to access server.", n => password = n},
                { "v|verbose", "Verbose output.", v => verbose = v != null},
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
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                showHelp = true;
            }

            if (showHelp)
            {
                // show some app description message
                Console.WriteLine("Usage: dotnet Technosoftware.ModelDesignClient.dll [OPTIONS] [ENDPOINTURL]");
                Console.WriteLine();

                // output the options
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return (int)ExitCode.ErrorInvalidCommandLine;
            }

            string endpointUrl;
            if (extraArgs == null || extraArgs.Count == 0)
            {
                // use Technosoftware OPC UA ModelDesign Server
                endpointUrl = "opc.tcp://localhost:55552/TechnosoftwareModelDesignServer";
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
        public String Username { get; set; }
        public String Password { get; set; }
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
                reconnectHandler_.BeginReconnect(session, 1000, OnServerReconnectComplete);
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

            // Load the Application Configuration and use the specified config section "Technosoftware.ModelDesignClient"
            var config = await application.LoadConfigurationAsync("Technosoftware.ModelDesignClient");

            // check the application certificate.
            var haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime);

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
            var selectedEndpoint = Discover.SelectEndpoint(endpointUrl_, haveAppCertificate, 15000);

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
            session_ = await CreateSessionAsync(config, selectedEndpoint, userIdentity).ConfigureAwait(false);

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
            var simulatedDataValue = session_.ReadValue(machine1LevelNodeId_);
            Console.WriteLine("Measurement Value:" + simulatedDataValue.Value);
            #endregion

            #region Create a subscription with publishing interval of 1 second
            Console.WriteLine(" 6 - Create a subscription with publishing interval of 1 second.");
            ExitCode = ExitCode.ErrorCreateSubscription;
            subscription_ = new Subscription(session_.DefaultSubscription) { PublishingInterval = 1000 };
            #endregion

            #region Add all dynamic values and the server time to the subscription
            Console.WriteLine(" 7 - Add the server time to the subscription.");
            ExitCode = ExitCode.ErrorMonitoredItem;
            var list = new List<MonitoredItem> {
                new MonitoredItem(subscription_.DefaultItem)
                {
                    DisplayName = "ServerStatusCurrentTime", StartNodeId = "i="+Variables.Server_ServerStatus_CurrentTime
                }
            };
            list.ForEach(i => i.MonitoredItemNotificationEvent += OnNotification);

            subscription_.AddItems(list);
            #endregion

            #region Add the subscription to the session
            Console.WriteLine(" 8 - Add the subscription to the session.");
            ExitCode = ExitCode.ErrorAddSubscription;
            session_.AddSubscription(subscription_);
            subscription_.Create();
            #endregion

            #region Running...Press Ctrl-C to exit...
            Console.WriteLine(" 9 - Running...Press Ctrl-C to exit...");
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
            return Session.CreateAsync(config, endpoint, false, "OPC UA Console Client", 60000, userIdentity, null);
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
        private readonly NodeId machine1LevelNodeId_ = new NodeId("ns=2;s=0:Machine #1_Level_Measurement");

        private Session session_;
        private Subscription subscription_;
        private SessionReconnectHandler reconnectHandler_;
        private readonly string endpointUrl_;
        private bool autoAccept_;
        private readonly int clientRunTime_;
        private ManualResetEvent quitEvent_;
        #endregion
    }
}
