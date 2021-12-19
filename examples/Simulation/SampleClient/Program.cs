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
using System.Threading;
using System.Threading.Tasks;
using Mono.Options;

using Opc.Ua;

using Technosoftware.UaClient;

using OptionSet = Mono.Options.OptionSet;
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
        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("SampleCompany {0} OPC UA Sample Client", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

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
                Console.WriteLine("Usage: dotnet SampleCompany.SampleClient.dll [OPTIONS] [ENDPOINTURL]");
                Console.WriteLine();

                // output the options
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return (int)MyUaClient.ExitCode.ErrorInvalidCommandLine;
            }

            string endpointUrl;
            if (extraArgs == null || extraArgs.Count == 0)
            {
                // use OPC UA Sample Server
                endpointUrl = "opc.tcp://localhost:55555/SampleServer";
            }
            else
            {
                endpointUrl = extraArgs[0];
            }

            stopTimeout = stopTimeout <= 0 ? Timeout.Infinite : stopTimeout * 1000;
            browseAddressSpace_ = !noBrowse;

            myUaClient_ = new MyUaClient("SampleCompany.SampleClient.Config.xml")
            {
                EndpointServerUrl = endpointUrl,
                AutoAccept = autoAccept,
                Verbose = verbose,
                Username = username,
                Password = password,
                UseSecurity = !securityNone,
                ReverseConnectUri = reverseConnectUrl
            };

            return (int)await StartUaClient(stopTimeout);
        }

        /// <summary>
        /// Main entry point for the different client tests.
        /// </summary>
        /// <param name="stopTime">The time in seconds the client should run</param>
        /// <returns></returns>
        private static async Task<MyUaClient.ExitCode> StartUaClient(int stopTime)
        {
            bool connected;
            try
            {
                connected = await ExecuteClientFunctions().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Utils.Trace("Exception:" + exception.Message);
                Console.WriteLine("Exception: {0}", exception.Message);
                return myUaClient_.ErrorCode;
            }

            if (connected)
            {
                #region Running...Press Ctrl-C to exit...
                Console.WriteLine("Running...Press Ctrl-C to exit...");
                #endregion
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
                    myUaClient_.SimulateReconnect();
                }

                // wait for timeout or Ctrl-C
                quitEvent_.WaitOne(stopTime);

                // return error conditions
                if (myUaClient_.Session != null && myUaClient_.Session.KeepAliveStopped)
                {
                    return MyUaClient.ExitCode.ErrorNoKeepAlive;
                }
            }
            return myUaClient_.ErrorCode;
        }

        /// <summary>
        /// Exdecutes the different client functions. Will be extended in the different sample applications
        /// </summary>
        /// <returns>if <c>true</c> the session was created successfully; otherwise false.</returns>
        private static async Task<bool> ExecuteClientFunctions()
        {
            var config = await myUaClient_.CreateApplicationAsync();

            if (config != null)
            {
                var connected = await myUaClient_.ConnectSessionAsync();
                if (connected)
                {
                    if (browseAddressSpace_)
                    {
                        myUaClient_.Browse();
                    }
                    myUaClient_.ReadServerStatus();
                    myUaClient_.ReadSingleValue("ns=2;s=Scalar_Simulation_Number");

                    List<string> nodeNames = new List<string>();
                    nodeNames.Add("ns=2;s=Scalar_Simulation_Number");
                    nodeNames.Add("ns=2;s=Scalar_Static_Integer");
                    nodeNames.Add("ns=2;s=Scalar_Static_Double");
                    myUaClient_.ReadMultipleValues(nodeNames);
                    myUaClient_.ReadMultipleValuesAsynchronous(nodeNames);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #region Fields
        private static MyUaClient myUaClient_;
        private static ManualResetEvent quitEvent_;
        private static bool browseAddressSpace_;
        #endregion
    }
}
