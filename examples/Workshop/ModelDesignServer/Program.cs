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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mono.Options;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Sessions;
#endregion

namespace Technosoftware.ModelDesignServer
{
    #region Enumerations
    public enum ExitCode
    {
        Ok = 0,
        ErrorServerNotStarted = 0x80,
        ErrorServerRunning = 0x81,
        ErrorServerException = 0x82,
        ErrorInvalidCommandLine = 0x100
    };
    #endregion

    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static int Main(string[] args)
        {
            Console.WriteLine("Technosoftware {0} OPC UA ModelDesign Server", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

            // command line options
            var showHelp = false;
            var stopTimeout = 0;
            var autoAccept = false;

            var options = new Mono.Options.OptionSet {
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "t|timeout=", "the number of seconds until the server stops.", (int t) => stopTimeout = t },
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
                Console.WriteLine(Utils.IsRunningOnMono() ? "Usage: mono Technosoftware.ModelDesignServer.exe [OPTIONS]" : "Usage: dotnet Technosoftware.ModelDesignServer.dll [OPTIONS]");
                Console.WriteLine();

                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return (int)ExitCode.ErrorInvalidCommandLine;
            }

            var server = new MySampleServer(autoAccept, stopTimeout);
            server.Run();

            return (int)MySampleServer.ExitCode;
        }
    }

    public class MySampleServer
    {
        #region Properties
        public Task Status { get; private set; }
        public DateTime LastEventTime { get; private set; }
        public int ServerRunTime { get; private set; }
        public static bool AutoAccept { get; private set; }
        public static ExitCode ExitCode { get; private set; }

        private static UaServer.UaServer uaServer_ = new UaServer.UaServer();
        private static UaServerPlugin uaServerPlugin_ = new UaServerPlugin();
        #endregion

        #region Constructors, Destructor, Initialization
        public MySampleServer(bool autoAccept, int stopTimeout)
        {
            AutoAccept = autoAccept;
            ServerRunTime = stopTimeout == 0 ? Timeout.Infinite : stopTimeout * 1000;
        }

        ~MySampleServer()
        {
            quitEvent_?.Dispose();
        }
        #endregion

        #region Public Methods
        public void Run()
        {

            try
            {
                ExitCode = ExitCode.ErrorServerNotStarted;
                ConsoleSampleServer().Wait();
                Console.WriteLine("Server started. Press Ctrl-C to exit...");
                ExitCode = ExitCode.ErrorServerRunning;
            }
            catch (Exception ex)
            {
                Utils.Trace("ServiceResultException:" + ex.Message);
                Console.WriteLine("Exception: {0}", ex.Message);
                ExitCode = ExitCode.ErrorServerException;
                return;
            }

            quitEvent_ = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (sender, eArgs) => {
                    quitEvent_.Set();
                    eArgs.Cancel = true;
                };
            }
            catch (Exception exception)
            {
                Utils.Trace("Exception:" + exception.Message);
            }

            // wait for timeout or Ctrl-C
            quitEvent_.WaitOne(ServerRunTime);

            if (uaServer_ != null)
            {
                Console.WriteLine("Server stopped. Waiting for exit...");

                using (var server = uaServer_)
                {
                    uaServer_.BaseServer.CurrentInstance.SessionManager.SessionActivatedEvent += EventStatus;
                    uaServer_.BaseServer.CurrentInstance.SessionManager.SessionClosingEvent += EventStatus;
                    uaServer_.BaseServer.CurrentInstance.SessionManager.SessionCreatedEvent += EventStatus;

                    // Stop status thread
                    uaServer_ = null;
                    Status.Wait();
                    // Stop server and dispose
                    server.Stop();
                }
            }

            ExitCode = ExitCode.Ok;
        }
        #endregion

        #region Task handling the startup of the OPC UA Server
        private async Task ConsoleSampleServer()
        {
            // start the server.
            await uaServer_.StartAsync(uaServerPlugin_, "Technosoftware.ModelDesignServer", null);

            // print endpoint info
            Console.WriteLine("Server Endpoints:");
            var endpoints = uaServer_.BaseServer.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
            foreach (var endpoint in endpoints)
            {
                Console.WriteLine(endpoint);
            }

            // start the status thread
            Status = Task.Run(StatusThread);

            // print notification on session events
            uaServer_.BaseServer.CurrentInstance.SessionManager.SessionActivatedEvent += EventStatus;
            uaServer_.BaseServer.CurrentInstance.SessionManager.SessionClosingEvent += EventStatus;
            uaServer_.BaseServer.CurrentInstance.SessionManager.SessionCreatedEvent += EventStatus;

        }
        #endregion

        #region Private Methods
        private void EventStatus(object sender, SessionEventArgs eventArgs)
        {
            LastEventTime = DateTime.UtcNow;
            var session = sender as Session;
            PrintSessionStatus(session, eventArgs.Reason.ToString());
        }

        void PrintSessionStatus(Session session, string reason, bool lastContact = false)
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

        private async void StatusThread()
        {
            while (uaServer_ != null)
            {
                if (DateTime.UtcNow - LastEventTime > TimeSpan.FromMilliseconds(6000))
                {
                    var sessions = uaServer_.BaseServer.CurrentInstance.SessionManager.GetSessions();
                    foreach (var session in sessions)
                    {
                        PrintSessionStatus(session, "-Status-", true);
                    }
                    LastEventTime = DateTime.UtcNow;
                }
                await Task.Delay(1000);
            }
        }
        #endregion

        #region Fields
        private ManualResetEvent quitEvent_;
        #endregion
    }
}
