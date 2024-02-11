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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;

using Technosoftware.UaConfiguration;

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
            await output.WriteLineAsync("OPC UA Simple Console Sample Client").ConfigureAwait(false);

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
            var showHelp = false;
            var autoAccept = false;
            string username = null;
            string userPassword = null;
            var renewCertificate = false;
            var verbose = false;
            var noSecurity = false;
            string password = null;
            var timeout = Timeout.Infinite;
            var discover = false;

            var options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "Show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "Auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "nsec|nosecurity", "Select endpoint with security NONE, least secure if unavailable", s => noSecurity = s != null },
                { "un|username=", "The name of the user identity for the connection", u => username = u },
                { "up|userpassword=", "The password of the user identity for the connection", u => userPassword = u },
                { "p|password=", "Optional password for private key", p => password = p },
                { "r|renew", "Renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "Timeout in seconds to exit application", (int t) => timeout = t * 1000 },
                { "v|verbose", "Verbose output", v => { if (v != null) { verbose = true; } } },
                { "d|discover", "Browses for OPC UA servers on the local machine.", d => discover = d != null},
            };

            // wait for timeout or Ctrl-C
            var quitCts = new CancellationTokenSource();
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
                var extraArg = ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp, "SAMPLECLIENT");

                // connect Url?
                Uri serverUrl = !string.IsNullOrEmpty(extraArg) ? new Uri(extraArg) : new Uri("opc.tcp://localhost:62555/SampleServer");

                // Define the UA Client application
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
                var passwordProvider = new CertificatePasswordProvider(password);
                var application = new ApplicationInstance {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Client,
                    ConfigSectionName = configSectionName,
                    CertificatePasswordProvider = passwordProvider
                };

                // load the application configuration.
                ApplicationConfiguration config = await application.LoadApplicationConfigurationAsync(silent: false).ConfigureAwait(false);

                // delete old certificate
                if (renewCertificate)
                {
                    await application.DeleteApplicationInstanceCertificateAsync(quitCts.Token).ConfigureAwait(false);
                }

                // check the application certificate.
                var haveAppCertificate = await application.CheckApplicationInstanceCertificateAsync(false, minimumKeySize: 0).ConfigureAwait(false);
                if (!haveAppCertificate)
                {
                    throw new ErrorExitException("Application instance certificate invalid!", ExitCode.ErrorCertificate);
                }

                // connect to a server until application stops
                bool quit;
                DateTime start = DateTime.UtcNow;
                var waitTime = int.MaxValue;
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

                    using (var uaClient = new MyUaClient(application.ApplicationConfiguration, output) {
                        AutoAccept = autoAccept,
                        SessionLifeTime = 60_000,
                    })
                    {
                        // Discover UA Servers
                        if (discover)
                        {
                            uaClient.DiscoverUaServers();
                        }

                        // set user identity
                        if (!String.IsNullOrEmpty(username))
                        {
                            uaClient.UserIdentity = new UserIdentity(username, userPassword ?? string.Empty);
                        }

                        var connected = await uaClient.ConnectAsync(serverUrl.ToString(), !noSecurity, quitCts.Token).ConfigureAwait(false);
                        if (connected)
                        {
                            await output.WriteLineAsync("Connected! Ctrl-C to quit.").ConfigureAwait(false);

                            // enable subscription transfer
                            uaClient.ReconnectPeriod = 1000;
                            uaClient.ReconnectPeriodExponentialBackoff = 10000;
                            uaClient.Session.MinPublishRequestCount = 3;
                            uaClient.Session.TransferSubscriptionsOnReconnect = true;

                            var clientFunctions = new ClientFunctions(output);

                            // Run tests for available methods on sample server.
                            _ = clientFunctions.ReadServerStatus(uaClient.Session);
                            clientFunctions.ReadNodes(uaClient.Session);
                            clientFunctions.WriteNodes(uaClient.Session);
                            clientFunctions.Browse(uaClient.Session);
                            clientFunctions.CallMethod(uaClient.Session);

                            await output.WriteLineAsync("Waiting...").ConfigureAwait(false);

                            // Wait for some DataChange notifications from MonitoredItems
                            quit = quitEvent.WaitOne(timeout > 0 ? waitTime : 30_000);

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
                output.Close();
            }
        }
    }
}
