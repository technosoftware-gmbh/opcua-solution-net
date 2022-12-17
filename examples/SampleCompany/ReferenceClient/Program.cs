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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Opc.Ua;

using Technosoftware.UaConfiguration;
#endregion

namespace SampleCompany.ReferenceClient
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
            var licenseData =
                    @"";
            var licensed = Technosoftware.UaClient.LicenseHandler.Validate(licenseData);
            #endregion

            TextWriter output = Console.Out;
            output.WriteLine("OPC UA Console Reference Client");

            // The application name and config file names
            var applicationName = "SampleCompany.ReferenceClient";
            var configSectionName = "SampleCompany.ReferenceClient";
            var usage = $"Usage: dotnet {applicationName}.dll [OPTIONS] [ENDPOINTURL]";

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
            string password = null;
            int timeout = Timeout.Infinite;
            string logFile = null;
            var discover = false;

            Mono.Options.OptionSet options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "Show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "Auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "un|username=", "The name of the user identity for the connection", (string u) => username = u },
                { "up|userpassword=", "The password of the user identity for the connection", (string u) => userpassword = u },
                { "co|console", "Log to console", c => logConsole = c != null },
                { "l|log", "Log app output", c => appLog = c != null },
                { "p|password=", "Optional password for private key", (string p) => password = p },
                { "r|renew", "Renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "Timeout in seconds to exit application", (int t) => timeout = t * 1000 },
                { "logfile=", "Custom file name for log output", l => { if (l != null) { logFile = l; } } },
                { "b|browseall", "Browse all references", b => { if (b != null) browseall = true; } },
                { "f|fetchall", "Fetch all nodes", f => { if (f != null) fetchall = true; } },
                { "j|json", "Output all Values as JSON", j => { if (j != null) jsonvalues = true; } },
                { "v|verbose", "Verbose output", v => { if (v != null) verbose = true; } },
                { "d|discover", "Browses for OPC UA servers on the local machine.", d => discover = d != null},
            };

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
                var extraArg = ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp, true);

                // connect Url?
                Uri serverUrl = null;
                if (!string.IsNullOrEmpty(extraArg))
                {
                    serverUrl = new Uri(extraArg);
                }
                else
                {
                    serverUrl = new Uri("opc.tcp://localhost:62555/ReferenceServer");
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
                var config = await application.LoadApplicationConfigurationAsync(silent: false);

                // override logfile
                if (logFile != null)
                {
                    var logFilePath = config.TraceConfiguration.OutputFilePath;
                    var filename = Path.GetFileNameWithoutExtension(logFilePath);
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

                // wait for timeout or Ctrl-C
                var quitEvent = ConsoleUtils.CtrlCHandler();

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
                    using (MyUaClient uaClient = new MyUaClient(
                        application.ApplicationConfiguration, output, verbose) {
                        AutoAccept = autoAccept
                    })
                    {
                        // set user identity
                        if (!String.IsNullOrEmpty(username))
                        {
                            uaClient.UserIdentity = new UserIdentity(username, userpassword ?? string.Empty);
                        }

                        bool connected = await uaClient.ConnectAsync(serverUrl.ToString(), false);
                        if (connected)
                        {
                            output.WriteLine("Connected! Ctrl-C to quit.");

                            // enable subscription transfer
                            uaClient.Session.TransferSubscriptionsOnReconnect = true;

                            var samples = new ClientFunctions(output, quitEvent, verbose);

                            if (browseall || fetchall || jsonvalues)
                            {
                                NodeIdCollection variableIds = null;
                                ReferenceDescriptionCollection referenceDescriptions = null;
                                if (browseall)
                                {
                                    referenceDescriptions =
                                        samples.BrowseFullAddressSpace(uaClient, Objects.RootFolder);
                                    variableIds = new NodeIdCollection(referenceDescriptions
                                        .Where(r => r.NodeClass == NodeClass.Variable && r.TypeDefinition.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                IList<INode> allNodes = null;
                                if (fetchall)
                                {
                                    allNodes = samples.FetchAllNodesNodeCache(
                                        uaClient, Objects.RootFolder, true, true, false);
                                    variableIds = new NodeIdCollection(allNodes
                                        .Where(r => r.NodeClass == NodeClass.Variable && ((VariableNode)r).DataType.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                if (jsonvalues && variableIds != null)
                                {
                                    await samples.ReadAllValuesAsync(uaClient, variableIds);
                                }

                                quit = true;
                            }
                            else
                            {
                                // Run tests for available methods on reference server.
                                samples.ReadNodes(uaClient.Session);
                                samples.WriteNodes(uaClient.Session);
                                samples.Browse(uaClient.Session);
                                samples.CallMethod(uaClient.Session);
                                samples.SubscribeToDataChanges(uaClient.Session, 120_000);
                                samples.SubscribeToEventChanges(uaClient.Session, 120_000);

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
        }
    }
}
