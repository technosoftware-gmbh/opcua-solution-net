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

namespace EmptyCompany.EmptyClient
{
    public static class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The arguments given on commandline.</param>
        /// <returns></returns>
        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("EmptyCompany {0} OPC UA Empty Client", Utils.IsRunningOnMono() ? "Mono" : ".NET Core");

            // command line options
            var showHelp = false;
            var stopTimeout = Timeout.Infinite;
            var autoAccept = false;
            var verbose = false;
            var securityNone = false;

            var options = new OptionSet {
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "t|timeout=", "the number of seconds until the client stops.", (int t) => stopTimeout = t },
                { "s|securitynone", "Do not use security for connection.", s => securityNone = s != null},
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
                Console.WriteLine("Usage: dotnet EmptyCompany.EmptyClient.dll [OPTIONS] [ENDPOINTURL]");
                Console.WriteLine();

                // output the options
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            string endpointUrl;
            if (extraArgs == null || extraArgs.Count == 0)
            {
                // use OPC UA Empty Server
                endpointUrl = "opc.tcp://localhost:55500/EmptyServer";
            }
            else
            {
                endpointUrl = extraArgs[0];
            }

            stopTimeout = stopTimeout <= 0 ? Timeout.Infinite : stopTimeout * 1000;

            return 0;
        }
    }
}
