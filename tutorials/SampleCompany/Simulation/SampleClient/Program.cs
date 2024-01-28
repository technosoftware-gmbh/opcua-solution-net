#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaClient;

using SampleCompany.Common;
#endregion

namespace SampleCompany.SampleClient
{
    /// <summary>The main program.</summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The arguments given on commandline.</param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            #region License validation
            string licenseData =
                    @"";
            bool licensed = Technosoftware.UaClient.LicenseHandler.Validate(licenseData);
            #endregion

            TextWriter output = Console.Out;
            output.WriteLine("OPC UA Console Sample Client");

            // The application name and config file names
            string applicationName = "SampleCompany.SampleClient";
            string configSectionName = "SampleCompany.SampleClient";
            string usage = $"Usage: dotnet {applicationName}.dll [OPTIONS] [ENDPOINTURL]";

            // command line options
            bool showHelp = false;
            bool autoAccept = false;
            string username = null;
            string userpassword = null;
            bool logConsole = false;
            bool appLog = false;
            bool renewCertificate = false;
            bool browseall = false;
            bool fetchall = false;
            bool jsonvalues = false;
            bool verbose = false;
            bool subscribe = false;
            bool noSecurity = false;
            string password = null;
            int timeout = Timeout.Infinite;
            string logFile = null;
            string reverseConnectUrlString = null;

            Mono.Options.OptionSet options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "Show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "Auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "nsec|nosecurity", "Select endpoint with security NONE, least secure if unavailable", s => noSecurity = s != null },
                { "un|username=", "The name of the user identity for the connection", (string u) => username = u },
                { "up|userpassword=", "The password of the user identity for the connection", (string u) => userpassword = u },
                { "c|console", "Log to console", c => logConsole = c != null },
                { "l|log", "Log app output", c => appLog = c != null },
                { "p|password=", "Optional password for private key", (string p) => password = p },
                { "r|renew", "Renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "Timeout in seconds to exit application", (int t) => timeout = t * 1000 },
                { "logfile=", "Custom file name for log output", l => { if (l != null) { logFile = l; } } },
                { "b|browseall", "Browse all references", b => { if (b != null) browseall = true; } },
                { "f|fetchall", "Fetch all nodes", f => { if (f != null) fetchall = true; } },
                { "j|json", "Output all Values as JSON", j => { if (j != null) jsonvalues = true; } },
                { "v|verbose", "Verbose output", v => { if (v != null) { verbose = true; } } },
                { "s|subscribe", "Subscribe", s => { if (s != null) subscribe = true; } },
                { "rc|reverseconnect=", "Connect using the reverse connect endpoint. (e.g. rc=opc.tcp://localhost:65300)", (string url) => reverseConnectUrlString = url},
            };

            ReverseConnectManager reverseConnectManager = null;

            if (verbose)
            {
                output.WriteLine("OPC UA library: {0} @ {1} -- {2}",
                    Utils.GetAssemblyBuildNumber(),
                    Utils.GetAssemblyTimestamp().ToString("G", CultureInfo.InvariantCulture),
                    Utils.GetAssemblySoftwareVersion());
            }

            try
            {
                // parse command line and set options
                string extraArg = ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp, "SAMPLECLIENT", false);

                // connect Url?
                Uri serverUrl = null;
                if (!string.IsNullOrEmpty(extraArg))
                {
                    serverUrl = new Uri(extraArg);
                }
                else
                {
                    serverUrl = new Uri("opc.tcp://localhost:62555/SampleServer");
                }

                // log console output to logger
                if (logConsole && appLog)
                {
                    output = new LogWriter();
                }

                // Define the UA Client application
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
                CertificatePasswordProvider PasswordProvider = new CertificatePasswordProvider(password);
                ApplicationInstance application = new ApplicationInstance {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Client,
                    ConfigSectionName = configSectionName,
                    CertificatePasswordProvider = PasswordProvider
                };

                // load the application configuration.
                ApplicationConfiguration config = await application.LoadApplicationConfigurationAsync(silent: false).ConfigureAwait(false);

                // override logfile
                if (logFile != null)
                {
                    string logFilePath = config.TraceConfiguration.OutputFilePath;
                    string filename = Path.GetFileNameWithoutExtension(logFilePath);
                    config.TraceConfiguration.OutputFilePath = logFilePath.Replace(filename, logFile);
                    config.TraceConfiguration.DeleteOnLoad = true;
                    config.TraceConfiguration.ApplySettings();
                }

                // setup the logging
                ConsoleUtils.ConfigureLogging(config, applicationName, logConsole, LogLevel.Information);

                // delete old certificate
                if (renewCertificate)
                {
                    await application.DeleteApplicationInstanceCertificateAsync().ConfigureAwait(false);
                }

                // check the application certificate.
                bool haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, minimumKeySize: 0).ConfigureAwait(false);
                if (!haveAppCertificate)
                {
                    throw new ErrorExitException("Application instance certificate invalid!", ExitCode.ErrorCertificate);
                }

                if (reverseConnectUrlString != null)
                {
                    // start the reverse connection manager
                    output.WriteLine("Create reverse connection endpoint at {0}.", reverseConnectUrlString);
                    reverseConnectManager = new ReverseConnectManager();
                    reverseConnectManager.AddEndpoint(new Uri(reverseConnectUrlString));
                    reverseConnectManager.StartService(config);
                }

                // wait for timeout or Ctrl-C
                CancellationTokenSource quitCTS = new CancellationTokenSource();
                ManualResetEvent quitEvent = ConsoleUtils.CtrlCHandler(quitCTS);

                // connect to a server until application stops
                bool quit = false;
                DateTime start = DateTime.UtcNow;
                int waitTime = int.MaxValue;
                do
                {
                    if (timeout > 0)
                    {
                        waitTime = timeout - (int)DateTime.UtcNow.Subtract(start).TotalMilliseconds;
                        if (waitTime <= 0)
                        {
                            break;
                        }
                    }

                    // create the UA Client object and connect to configured server.

                    using (MyUaClient uaClient = new MyUaClient(application.ApplicationConfiguration, reverseConnectManager, output, ClientBase.ValidateResponse, verbose) {
                        AutoAccept = autoAccept,
                        SessionLifeTime = 60_000,
                    })
                    {
                        // set user identity
                        if (!String.IsNullOrEmpty(username))
                        {
                            uaClient.UserIdentity = new UserIdentity(username, userpassword ?? string.Empty);
                        }

                        bool connected = await uaClient.ConnectAsync(serverUrl.ToString(), !noSecurity, quitCTS.Token).ConfigureAwait(false);
                        if (connected)
                        {
                            output.WriteLine("Connected! Ctrl-C to quit.");

                            // enable subscription transfer
                            uaClient.ReconnectPeriod = 1000;
                            uaClient.ReconnectPeriodExponentialBackoff = 10000;
                            uaClient.Session.MinPublishRequestCount = 3;
                            uaClient.Session.TransferSubscriptionsOnReconnect = true;

                            ClientFunctions samples = new ClientFunctions(output, ClientBase.ValidateResponse, quitEvent, verbose);

                            if (browseall || fetchall || jsonvalues)
                            {
                                NodeIdCollection variableIds = null;
                                ReferenceDescriptionCollection referenceDescriptions = null;
                                if (browseall)
                                {
                                    referenceDescriptions =
                                        await samples.BrowseFullAddressSpaceAsync((IMyClient)uaClient, Objects.RootFolder).ConfigureAwait(false);
                                    variableIds = new NodeIdCollection(referenceDescriptions
                                        .Where(r => r.NodeClass == NodeClass.Variable && r.TypeDefinition.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                IList<INode> allNodes = null;
                                if (fetchall)
                                {
                                    allNodes = await samples.FetchAllNodesNodeCacheAsync((IMyClient)uaClient, Objects.RootFolder, true, true, false).ConfigureAwait(false);
                                    variableIds = new NodeIdCollection(allNodes
                                        .Where(r => r.NodeClass == NodeClass.Variable && r is VariableNode && ((VariableNode)r).DataType.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                if (jsonvalues && variableIds != null)
                                {
                                    (DataValueCollection allValues, IList<ServiceResult> results) = await samples.ReadAllValuesAsync((IMyClient)uaClient, variableIds).ConfigureAwait(false);
                                }

                                if (subscribe && (browseall || fetchall))
                                {
                                    // subscribe to 100 random variables
                                    const int MaxVariables = 100;
                                    NodeCollection variables = new NodeCollection();
                                    Random random = new Random(62541);
                                    if (fetchall)
                                    {
                                        variables.AddRange(allNodes
                                            .Where(r => r.NodeClass == NodeClass.Variable && r.NodeId.NamespaceIndex > 1)
                                            .Select(r => ((VariableNode)r))
                                            .OrderBy(o => random.Next())
                                            .Take(MaxVariables));
                                    }
                                    else if (browseall)
                                    {
                                        List<ExpandedNodeId> variableReferences = referenceDescriptions
                                            .Where(r => r.NodeClass == NodeClass.Variable && r.NodeId.NamespaceIndex > 1)
                                            .Select(r => r.NodeId)
                                            .OrderBy(o => random.Next())
                                            .Take(MaxVariables)
                                            .ToList();
                                        variables.AddRange(uaClient.Session.NodeCache.Find(variableReferences).Cast<Node>());
                                    }

                                    await samples.SubscribeAllValuesAsync((IMyClient)uaClient,
                                        variableIds: new NodeCollection(variables),
                                        samplingInterval: 1000,
                                        publishingInterval: 5000,
                                        queueSize: 10,
                                        lifetimeCount: 12,
                                        keepAliveCount: 2).ConfigureAwait(false);

                                    // Wait for DataChange notifications from MonitoredItems
                                    output.WriteLine("Subscribed to {0} variables. Press Ctrl-C to exit.", MaxVariables);
                                    quit = quitEvent.WaitOne(timeout > 0 ? waitTime : Timeout.Infinite);
                                }
                                else
                                {
                                    quit = true;
                                }
                            }
                            else
                            {
                                // Run tests for available methods on reference server.
                                samples.ReadNodes(uaClient.Session);
                                samples.WriteNodes(uaClient.Session);
                                samples.Browse(uaClient.Session);
                                samples.CallMethod(uaClient.Session);
                                samples.SubscribeToDataChanges(uaClient.Session, 120_000);
                                
                                output.WriteLine("Waiting...");

                                // Wait for some DataChange notifications from MonitoredItems
                                quit = quitEvent.WaitOne(timeout > 0 ? waitTime : 30_000);
                            }

                            output.WriteLine("Client disconnected.");

                            uaClient.Disconnect();
                        }
                        else
                        {
                            output.WriteLine("Could not connect to server! Retry in 10 seconds or Ctrl-C to quit.");
                            quit = quitEvent.WaitOne(Math.Min(10_000, waitTime));
                        }
                    }

                } while (!quit);

                output.WriteLine("Client stopped.");
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
            }
            finally
            {
                Utils.SilentDispose(reverseConnectManager);
                output.Close();
            }
        }
    }
}
