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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Options;

using Opc.Ua;
using Technosoftware.UaConfiguration;
using Technosoftware.UaServer.Sessions;
#endregion

namespace Technosoftware.ReferenceServer
{
    /// <summary>
    /// A dialog which asks for user input.
    /// </summary>
    public class ApplicationMessageDlg : IApplicationMessageDlg
    {
        private string message_ = string.Empty;
        private bool ask_ = false;

        public override void Message(string text, bool ask)
        {
            message_ = text;
            ask_ = ask;
        }

        public override bool Show()
        {
            return ShowAsync().GetAwaiter().GetResult();
        }

        public override async Task<bool> ShowAsync()
        {
            if (ask_)
            {
                message_ += " (y/n, default y): ";
                Console.Write(message_);
            }
            else
            {
                Console.WriteLine(message_);
            }
            if (ask_)
            {
                try
                {
                    ConsoleKeyInfo result = Console.ReadKey();
                    Console.WriteLine();
                    return await Task.FromResult((result.KeyChar == 'y') || (result.KeyChar == 'Y') || (result.KeyChar == '\r')).ConfigureAwait(false);
                }
                catch
                {
                    // intentionally fall through
                }
            }
            return await Task.FromResult(true).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// The error code why the server exited.
    /// </summary>
    public enum ExitCode : int
    {
        Ok = 0,
        ErrorServerNotStarted = 0x80,
        ErrorServerRunning = 0x81,
        ErrorServerException = 0x82,
        ErrorInvalidCommandLine = 0x100
    };

    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Technosoftware {0} OPC UA Reference Server", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

            // command line options
            var showHelp = false;
            var autoAccept = false;
            string password = null;

            var options = new Mono.Options.OptionSet {
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "p|password=", "optional password for private key", (string p) => password = p }
            };

            try
            {
                IList<string> extraArgs = options.Parse(args);
                foreach (var extraArg in extraArgs)
                {
                    Console.WriteLine("Error: Unknown option: {0}", extraArg);
                    showHelp = true;
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                showHelp = true;
            }

            if (showHelp)
            {
                Console.WriteLine(Utils.IsRunningOnMono() ? "Usage: mono Technosoftware.ReferenceServer.exe [OPTIONS]" : "Usage: dotnet Technosoftware.ReferenceServer.dll [OPTIONS]");
                Console.WriteLine();

                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return (int)ExitCode.ErrorInvalidCommandLine;
            }

            var server = new MyReferenceServer() {
                AutoAccept = autoAccept,
                Password = password
            };
            await server.Run().ConfigureAwait(false);

            return (int)server.ExitCode;
        }
    }

    public class MyReferenceServer
    {
        #region Fields
        private static UaServer.UaServer uaServer_ = new UaServer.UaServer();
        private static readonly UaServerPlugin uaServerPlugin_ = new UaServerPlugin();

        private Task status_;
        private DateTime lastEventTime_;
        public bool LogConsole { get; set; } = false;
        public bool AutoAccept { get; set; } = false;
        public string Password { get; set; } = null;
        public ExitCode ExitCode { get; private set; }
        #endregion
        public async Task Run()
        {
            try
            {
                ExitCode = ExitCode.ErrorServerNotStarted;
                await ReferenceConsoleServerAsync().ConfigureAwait(false);
                Console.WriteLine("Server started. Press Ctrl-C to exit...");
                ExitCode = ExitCode.ErrorServerRunning;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
                ExitCode = ExitCode.ErrorServerException;
                return;
            }

            var quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    quitEvent.Set();
                    eArgs.Cancel = true;
                };
            }
            catch
            {
                // ignored
            }

            // wait for timeout or Ctrl-C
            quitEvent.WaitOne();

            if (uaServer_ != null)
            {
                Console.WriteLine("Server stopped. Waiting for exit...");

                using (var server = uaServer_)
                {
                    // Stop status thread
                    uaServer_ = null;
                    status_.Wait();
                    // Stop server and dispose
                    server.Stop();
                }
            }

            ExitCode = ExitCode.Ok;
        }

        private void OnCertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                if (AutoAccept)
                {
                    if (!LogConsole)
                    {
                        Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
                    }
                    Utils.Trace(Utils.TraceMasks.Security, "Accepted Certificate: {0}", e.Certificate.Subject);
                    e.Accept = true;
                    return;
                }
            }
            if (!LogConsole)
            {
                Console.WriteLine("Rejected Certificate: {0} {1}", e.Error, e.Certificate.Subject);
            }
            Utils.Trace(Utils.TraceMasks.Security, "Rejected Certificate: {0} {1}", e.Error, e.Certificate.Subject);
        }

        private async Task ReferenceConsoleServerAsync()
        {
            ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
            CertificatePasswordProvider passwordProvider = new CertificatePasswordProvider(Password);

            // start the server.
            await uaServer_.StartAsync(uaServerPlugin_, "Technosoftware.ReferenceServer", passwordProvider, OnCertificateValidation, null).ConfigureAwait(false);

            // print endpoint info
            var endpoints = uaServer_.BaseServer.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine(endpoint);
            }

            // start the status thread
            status_ = Task.Run(new Action(StatusThreadAsync));

            // print notification on session events
            uaServer_.BaseServer.CurrentInstance.SessionManager.SessionActivatedEvent += EventStatus;
            uaServer_.BaseServer.CurrentInstance.SessionManager.SessionClosingEvent += EventStatus;
            uaServer_.BaseServer.CurrentInstance.SessionManager.SessionCreatedEvent += EventStatus;
        }

        private void EventStatus(object sender, SessionEventArgs eventArgs)
        {
            lastEventTime_ = DateTime.UtcNow;
            var session = sender as Session;
            PrintSessionStatus(session, eventArgs.Reason.ToString());
        }

        private void PrintSessionStatus(Session session, string reason, bool lastContact = false)
        {
            lock (session.DiagnosticsLock)
            {
                var item = $"{reason,9}:{session.SessionDiagnostics.SessionName,20}:";
                if (lastContact)
                {
                    item += $"Last Event:{session.SessionDiagnostics.ClientLastContactTime.ToLocalTime():HH:mm:ss}";
                }
                else
                {
                    if (session.Identity != null)
                    {
                        item += $":{session.Identity.DisplayName,20}";
                    }
                    item += $":{session.Id}";
                }
                Console.WriteLine(item);
            }
        }

        private async void StatusThreadAsync()
        {
            while (uaServer_ != null)
            {
                if (DateTime.UtcNow - lastEventTime_ > TimeSpan.FromMilliseconds(6000))
                {
                    var sessions = uaServer_.BaseServer.CurrentInstance.SessionManager.GetSessions();
                    foreach (var session in sessions)
                    {
                        PrintSessionStatus(session, "-Status-", true);
                    }
                    lastEventTime_ = DateTime.UtcNow;
                }
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }
}
