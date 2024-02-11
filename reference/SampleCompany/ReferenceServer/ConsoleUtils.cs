#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif
using Microsoft.Extensions.Logging;
using Mono.Options;

using Serilog;
using Serilog.Events;
using Serilog.Templates;

using Opc.Ua;
using static Opc.Ua.Utils;
#endregion

namespace SampleCompany.ReferenceServer
{
    /// <summary>
    /// Helper functions shared in various console applications.
    /// </summary>
    public static class ConsoleUtils
    {
        /// <summary>
        /// Process a command line of the console sample application.
        /// </summary>
        /// <param name="output">The TextWriter to use for the output.</param>
        /// <param name="args">The arguments to handle.</param>
        /// <param name="options">The available options to parse.</param>
        /// <param name="showHelp">true if help should be shown; false otherwise.</param>
        /// <param name="additionalArgs">true if additional arguments should be returned; false otherwise.</param>
        /// <returns>Returns an additional argument provided and not handled yet, e.g. used for client as Url.</returns>
        public static string ProcessCommandLine(
            TextWriter output,
            string[] args,
            Mono.Options.OptionSet options,
            ref bool showHelp,
            string environmentPrefix,
            bool additionalArgs = false)
        {
#if NET5_0_OR_GREATER
            // Convert environment settings to command line flags
            // because in some environments (e.g. docker cloud) it is
            // the only supported way to pass arguments.
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddEnvironmentVariables(environmentPrefix + "_")
                .Build();

            var argslist = args.ToList();
            foreach (Option option in options)
            {
                var names = option.GetNames();
                var longest = names.MaxBy(s => s.Length);
                if (longest != null && longest.Length >= 3)
                {
                    var envKey = config[longest.ToUpperInvariant()];
                    if (envKey != null)
                    {
                        if (string.IsNullOrWhiteSpace(envKey) || option.OptionValueType == OptionValueType.None)
                        {
                            argslist.Add("--" + longest);
                        }
                        else
                        {
                            argslist.Add("--" + longest + "=" + envKey);
                        }
                    }
                }
            }
            args = argslist.ToArray();
#endif

            IList<string> additionalArguments = null;
            try
            {
                additionalArguments = options.Parse(args);
                if (!additionalArgs)
                {
                    foreach (var additionalArg in additionalArguments)
                    {
                        output.WriteLine("Error: Unknown option: {0}", additionalArg);
                        showHelp = true;
                    }
                }
            }
            catch (OptionException e)
            {
                output.WriteLine(e.Message);
                showHelp = true;
            }

            if (showHelp)
            {
                options.WriteOptionDescriptions(output);
                throw new ErrorExitException("Invalid Commandline or help requested.", ExitCode.ErrorInvalidCommandLine);
            }

            return additionalArguments.FirstOrDefault();
        }

        /// <summary>
        /// Configure the logging providers.
        /// </summary>
        /// <remarks>
        /// Replaces the Opc.Ua.Core default ILogger with a Microsoft.Extension.Logger with a 
        /// Serilog file, debug and console logger.
        /// The debug logger is only enabled for debug builds.
        /// The console logger is enabled by the logConsole flag at the consoleLogLevel.
        /// The file logger uses the setting in the ApplicationConfiguration.
        /// The Trace logLevel is chosen if required by the Tracemasks.
        /// </remarks>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="context">The context name for the logger. </param>
        /// <param name="logConsole">Enable logging to the console.</param>
        /// <param name="consoleLogLevel">The LogLevel to use for the console/debug.<
        /// /param>
        public static void ConfigureLogging(
            ApplicationConfiguration configuration,
            string context,
            bool logConsole,
            LogLevel consoleLogLevel)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += Unobserved_TaskException;

            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext();

            if (logConsole)
            {
                loggerConfiguration.WriteTo.Console(
                    restrictedToMinimumLevel: (LogEventLevel)consoleLogLevel
                    );
            }
#if DEBUG
            else
            {
                loggerConfiguration
                    .WriteTo.Debug(restrictedToMinimumLevel: (LogEventLevel)consoleLogLevel);
            }
#endif
            LogLevel fileLevel = LogLevel.Information;

            // switch for Trace/Verbose output
            var traceMasks = configuration.TraceConfiguration.TraceMasks;
            if ((traceMasks & ~(TraceMasks.Information | TraceMasks.Error |
                TraceMasks.Security | TraceMasks.StartStop | TraceMasks.StackTrace)) != 0)
            {
                fileLevel = LogLevel.Trace;
            }

            // add file logging if configured
            var outputFilePath = configuration.TraceConfiguration.OutputFilePath;
            if (!string.IsNullOrWhiteSpace(outputFilePath))
            {
                loggerConfiguration.WriteTo.File(
                    new ExpressionTemplate("{UtcDateTime(@t):yyyy-MM-dd HH:mm:ss.fff} [{@l:u3}] {@m}\n{@x}"),
                    ReplaceSpecialFolderNames(outputFilePath),
                    restrictedToMinimumLevel: (LogEventLevel)fileLevel,
                    rollOnFileSizeLimit: true);
            }

            // adjust minimum level
            if (fileLevel < LogLevel.Information || consoleLogLevel < LogLevel.Information)
            {
                loggerConfiguration.MinimumLevel.Verbose();
            }

            // create the serilog logger
            Serilog.Core.Logger serilogger = loggerConfiguration
                .CreateLogger();

            // create the ILogger for Opc.Ua.Core
            Microsoft.Extensions.Logging.ILogger logger = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Trace))
                .AddSerilog(serilogger)
                .CreateLogger(context);

            // set logger interface, disables TraceEvent
            SetLogger(logger);
        }

        /// <summary>
        /// Output log messages.
        /// </summary>
        public static void LogTest()
        {
            // print legacy logging output, for testing
            Trace(TraceMasks.Error, "This is an Error message: {0}", TraceMasks.Error);
            Trace(TraceMasks.Information, "This is a Information message: {0}", TraceMasks.Information);
            Trace(TraceMasks.StackTrace, "This is a StackTrace message: {0}", TraceMasks.StackTrace);
            Trace(TraceMasks.Service, "This is a Service message: {0}", TraceMasks.Service);
            Trace(TraceMasks.ServiceDetail, "This is a ServiceDetail message: {0}", TraceMasks.ServiceDetail);
            Trace(TraceMasks.Operation, "This is a Operation message: {0}", TraceMasks.Operation);
            Trace(TraceMasks.OperationDetail, "This is a OperationDetail message: {0}", TraceMasks.OperationDetail);
            Trace(TraceMasks.StartStop, "This is a StartStop message: {0}", TraceMasks.StartStop);
            Trace(TraceMasks.ExternalSystem, "This is a ExternalSystem message: {0}", TraceMasks.ExternalSystem);
            Trace(TraceMasks.Security, "This is a Security message: {0}", TraceMasks.Security);

            // print ILogger logging output
            LogTrace("This is a Trace message: {0}", LogLevel.Trace);
            LogDebug("This is a Debug message: {0}", LogLevel.Debug);
            LogInfo("This is a Info message: {0}", LogLevel.Information);
            LogWarning("This is a Warning message: {0}", LogLevel.Warning);
            LogError("This is a Error message: {0}", LogLevel.Error);
            LogCritical("This is a Critical message: {0}", LogLevel.Critical);
        }

        /// <summary>
        /// Create an event which is set if a user
        /// enters the Ctrl-C key combination.
        /// </summary>
        public static ManualResetEvent CtrlCHandler(CancellationTokenSource cts)
        {
            var quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (_, eArgs) => {
                    cts.Cancel();
                    _ = quitEvent.Set();
                    eArgs.Cancel = true;
                };
            }
            catch
            {
                // intentionally left blank
            }
            return quitEvent;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogCritical("Unhandled Exception: {0} IsTerminating: {1}", args.ExceptionObject, args.IsTerminating);
        }

        private static void Unobserved_TaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            LogCritical("Unobserved Exception: {0} Observed: {1}", args.Exception, args.Observed);
        }

    }
}

