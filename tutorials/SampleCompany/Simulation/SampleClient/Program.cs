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
            TextWriter output = Console.Out;
            await output.WriteLineAsync("OPC UA Console Sample Client").ConfigureAwait(false);

            #region License validation
            var licenseData =
                    @"";
            var licensed = Technosoftware.UaClient.LicenseHandler.Validate(licenseData);
            if (!licensed)
            {
                await output.WriteLineAsync("WARNING: No valid license applied.").ConfigureAwait(false);
            }
            #endregion

            // The application name and config file names
            var applicationName = "SampleCompany.SampleClient";
            var configSectionName = "SampleCompany.SampleClient";
            var usage = $"Usage: dotnet {applicationName}.dll [OPTIONS] [ENDPOINTURL]";

            // command line options
            bool showHelp = false;
            bool autoAccept = false;
            string username = null;
            string userPassword = null;
            bool logConsole = false;
            bool appLog = false;
            bool renewCertificate = false;
            bool browseAll = false;
            bool fetchAll = false;
            bool jsonValues = false;
            bool verbose = false;
            bool subscribe = false;
            bool noSecurity = false;
            string password = null;
            int timeout = Timeout.Infinite;
            string logFile = null;
            string reverseConnectUrlString = null;

            var options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "Show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "Auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "nsec|nosecurity", "Select endpoint with security NONE, least secure if unavailable", s => noSecurity = s != null },
                { "un|username=", "The name of the user identity for the connection", u => username = u },
                { "up|userpassword=", "The password of the user identity for the connection", u => userPassword = u },
                { "c|console", "Log to console", c => logConsole = c != null },
                { "l|log", "Log app output", c => appLog = c != null },
                { "p|password=", "Optional password for private key", p => password = p },
                { "r|renew", "Renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "Timeout in seconds to exit application", (int t) => timeout = t * 1000 },
                { "logfile=", "Custom file name for log output", l => { if (l != null) { logFile = l; } } },
                { "b|browseall", "Browse all references", b => { if (b != null) { browseAll = true; } } },
                { "f|fetchall", "Fetch all nodes", f => { if (f != null) { fetchAll = true; } } },
                { "j|json", "Output all Values as JSON", j => { if (j != null) { jsonValues = true; } } },
                { "v|verbose", "Verbose output", v => { if (v != null) { verbose = true; } } },
                { "s|subscribe", "Subscribe", s => { if (s != null) { subscribe = true; } } },
                { "rc|reverseconnect=", "Connect using the reverse connect endpoint. (e.g. rc=opc.tcp://localhost:65300)", url => reverseConnectUrlString = url},
            };

            ReverseConnectManager reverseConnectManager = null;

            // wait for timeout or Ctrl-C
            CancellationTokenSource quitCts = new CancellationTokenSource();
            ManualResetEvent quitEvent = ConsoleUtils.CtrlCHandler(quitCts);

            if (verbose)
            {
                var assemblyBuildNumber = Utils.GetAssemblyBuildNumber();
                var assemblyTimestamp = Utils.GetAssemblyTimestamp().ToString("G", CultureInfo.InvariantCulture);
                var assemblySoftwareVersion = Utils.GetAssemblySoftwareVersion();
                var outputString =
                    $"OPC UA library: {assemblyBuildNumber} @ {assemblyTimestamp} -- {assemblySoftwareVersion}";
                await output.WriteLineAsync(outputString).ConfigureAwait(false);
            }

            try
            {
                // parse command line and set options
                string extraArg = ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp, "SAMPLECLIENT");

                // connect Url?
                Uri serverUrl = !string.IsNullOrEmpty(extraArg) ? new Uri(extraArg) : new Uri("opc.tcp://localhost:62555/SampleServer");

                // log console output to logger
                if (logConsole && appLog)
                {
                    output = new LogWriter();
                }

                // Define the UA Client application
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
                CertificatePasswordProvider passwordProvider = new CertificatePasswordProvider(password);
                ApplicationInstance application = new ApplicationInstance {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Client,
                    ConfigSectionName = configSectionName,
                    CertificatePasswordProvider = passwordProvider
                };

                // load the application configuration.
                ApplicationConfiguration config = await application.LoadApplicationConfigurationAsync(silent: false).ConfigureAwait(false);

                // override logfile
                if (logFile != null)
                {
                    var logFilePath = config.TraceConfiguration.OutputFilePath;
                    if (logFilePath != null)
                    {
                        var filename = Path.GetFileNameWithoutExtension(logFilePath);
                        config.TraceConfiguration.OutputFilePath = logFilePath.Replace(filename, logFile);
                    }
                    config.TraceConfiguration.DeleteOnLoad = true;
                    config.TraceConfiguration.ApplySettings();
                }

                // setup the logging
                ConsoleUtils.ConfigureLogging(config, applicationName, logConsole, LogLevel.Information);

                // delete old certificate
                if (renewCertificate)
                {
                    await application.DeleteApplicationInstanceCertificateAsync(quitCts.Token).ConfigureAwait(false);
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
                    var outputString =
                        $"Create reverse connection endpoint at {reverseConnectUrlString}";

                    await output.WriteLineAsync(outputString).ConfigureAwait(false);
                    reverseConnectManager = new ReverseConnectManager();
                    reverseConnectManager.AddEndpoint(new Uri(reverseConnectUrlString));
                    reverseConnectManager.StartService(config);
                }

                // connect to a server until application stops
                bool quit;
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
                            uaClient.UserIdentity = new UserIdentity(username, userPassword ?? string.Empty);
                        }

                        bool connected = await uaClient.ConnectAsync(serverUrl.ToString(), !noSecurity, quitCts.Token).ConfigureAwait(false);
                        if (connected)
                        {
                            await output.WriteLineAsync("Connected! Ctrl-C to quit.").ConfigureAwait(false);

                            // enable subscription transfer
                            uaClient.ReconnectPeriod = 1000;
                            uaClient.ReconnectPeriodExponentialBackoff = 10000;
                            uaClient.Session.MinPublishRequestCount = 3;
                            uaClient.Session.TransferSubscriptionsOnReconnect = true;

                            var clientFunctions = new ClientFunctions(output, ClientBase.ValidateResponse, quitEvent, verbose);

                            if (browseAll || fetchAll || jsonValues)
                            {
                                NodeIdCollection variableIds = null;
                                ReferenceDescriptionCollection referenceDescriptions = null;
                                if (browseAll)
                                {
                                    referenceDescriptions =
                                        await clientFunctions.BrowseFullAddressSpaceAsync(uaClient, Objects.RootFolder).ConfigureAwait(false);
                                    variableIds = new NodeIdCollection(referenceDescriptions
                                        .Where(r => r.NodeClass == NodeClass.Variable && r.TypeDefinition.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                IList<INode> allNodes = null;
                                if (fetchAll)
                                {
                                    allNodes = await clientFunctions.FetchAllNodesNodeCacheAsync(uaClient, Objects.RootFolder, true, true, false).ConfigureAwait(false);
                                    variableIds = new NodeIdCollection(allNodes
                                        .Where(r => r.NodeClass == NodeClass.Variable && r is VariableNode node && node.DataType.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                if (jsonValues && variableIds != null)
                                {
                                    (DataValueCollection allValues, IList<ServiceResult> results) = await clientFunctions.ReadAllValuesAsync(uaClient, variableIds).ConfigureAwait(false);
                                }

                                if (subscribe && (browseAll || fetchAll))
                                {
                                    // subscribe to 100 random variables
                                    const int MaxVariables = 100;
                                    NodeCollection variables = new NodeCollection();
                                    Random random = new Random(62541);
                                    if (fetchAll)
                                    {
                                        variables.AddRange(allNodes
                                            .Where(r => r.NodeClass == NodeClass.Variable && r.NodeId.NamespaceIndex > 1)
                                            .Select(r => (VariableNode)r)
                                            .OrderBy(o => random.Next())
                                            .Take(MaxVariables));
                                    }
                                    else if (browseAll)
                                    {
                                        var variableReferences = referenceDescriptions
                                            .Where(r => r.NodeClass == NodeClass.Variable && r.NodeId.NamespaceIndex > 1)
                                            .Select(r => r.NodeId)
                                            .OrderBy(o => random.Next())
                                            .Take(MaxVariables)
                                            .ToList();
                                        variables.AddRange(uaClient.Session.NodeCache.Find(variableReferences).Cast<Node>());
                                    }

                                    await clientFunctions.SubscribeAllValuesAsync(uaClient,
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
                                clientFunctions.ReadNodes(uaClient.Session);
                                clientFunctions.WriteNodes(uaClient.Session);
                                clientFunctions.Browse(uaClient.Session);
                                clientFunctions.CallMethod(uaClient.Session);
                                _ = clientFunctions.SubscribeToDataChanges(uaClient.Session, 120_000);
                                _ = clientFunctions.SubscribeToEventChanges(uaClient.Session, 120_000);

                                await output.WriteLineAsync("Waiting...").ConfigureAwait(false);

                                // Wait for some DataChange notifications from MonitoredItems
                                quit = quitEvent.WaitOne(timeout > 0 ? waitTime : 30_000);
                            }

                            await output.WriteLineAsync("Client disconnected.").ConfigureAwait(false);

                            uaClient.Disconnect();
                        }
                        else
                        {
                            await output.WriteLineAsync("Could not connect to server! Retry in 10 seconds or Ctrl-C to quit.").ConfigureAwait(false);
                            quit = quitEvent.WaitOne(Math.Min(10_000, waitTime));
                        }
                    }

                } while (!quit);
                await output.WriteLineAsync("Client stopped.").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await output.WriteLineAsync(ex.Message).ConfigureAwait(false);
            }
            finally
            {
                quitEvent.Dispose();
                quitCts.Dispose();
                Utils.SilentDispose(reverseConnectManager);
                output.Close();
            }
        }
    }
}
