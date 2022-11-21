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
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Opc.Ua;

using SampleCompany.NodeManagers;
#endregion

namespace SampleCompany.SampleServer
{
    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static async Task<int> Main(string[] args)
        {
            TextWriter output = Console.Out;
            output.WriteLine("SampleCompany {0} OPC UA Reference Server", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

            // The application name and config file names
            var applicationName = "SampleCompany.ReferenceServer";
            var configSectionName = "SampleCompany.ReferenceServer";

            // command line options
            bool showHelp = false;
            bool autoAccept = false;
            bool logConsole = false;
            bool appLog = false;
            bool renewCertificate = false;
            string password = null;
            int timeout = -1;

            var usage = Utils.IsRunningOnMono() ? $"Usage: mono {applicationName}.exe [OPTIONS]" : $"Usage: dotnet {applicationName}.dll [OPTIONS]";
            Mono.Options.OptionSet options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "c|console", "log to console", c => logConsole = c != null },
                { "l|log", "log app output", c => appLog = c != null },
                { "p|password=", "optional password for private key", p => password = p },
                { "r|renew", "renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "timeout in seconds to exit application", (int t) => timeout = t * 1000 },
            };

            try
            {
                // parse command line and set options
                ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp);

                if (logConsole && appLog)
                {
                    output = new LogWriter();
                }

                // create the UA server
                var server = new MyUaServer<NodeManagers.Reference.ReferenceServer>(output) {
                    AutoAccept = autoAccept,
                    Password = password
                };

                // load the server configuration, validate certificates
                output.WriteLine("Loading configuration from {0}.", configSectionName);
                await server.LoadAsync(applicationName, configSectionName).ConfigureAwait(false);

                // setup the logging
                ConsoleUtils.ConfigureLogging(server.Configuration, applicationName, logConsole, LogLevel.Information);

                // check or renew the certificate
                await output.WriteLineAsync("Check the certificate.");
                await server.CheckCertificateAsync(renewCertificate).ConfigureAwait(false);

                // Create and add the node managers
                server.Create(NodeManagerUtils.NodeManagerFactories);

                // start the server
                await output.WriteLineAsync("Start the server.");
                await server.StartAsync().ConfigureAwait(false);

                await output.WriteLineAsync("Server started. Press Ctrl-C to exit...");

                // wait for timeout or Ctrl-C
                var quitEvent = ConsoleUtils.CtrlCHandler();
                quitEvent.WaitOne(timeout);

                // stop server. May have to wait for clients to disconnect.
                await output.WriteLineAsync("Server stopped. Waiting for exit...");
                await server.StopAsync().ConfigureAwait(false);

                return (int)ExitCode.Ok;
            }
            catch (ErrorExitException errorExitException)
            {
                output.WriteLine("The application exits with error: {0}", errorExitException.Message);
                return (int)errorExitException.ExitCode;
            }
        }
    }
}
