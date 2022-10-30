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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mono.Options;

using Serilog;
using Serilog.Events;
using Serilog.Templates;

using Opc.Ua;
using static Opc.Ua.Utils;

using Technosoftware.UaConfiguration;
#endregion

namespace Technosoftware.ReferenceServer
{
    /// <summary>
    /// The log output implementation of a TextWriter.
    /// </summary>
    public class LogWriter : TextWriter
    {
        private StringBuilder builder_ = new StringBuilder();

        public override void Write(char value)
        {
            builder_.Append(value);
        }

        public override void WriteLine(char value)
        {
            builder_.Append(value);
            LogInfo("{0}", builder_.ToString());
            builder_.Clear();
        }

        public override void WriteLine()
        {
            LogInfo("{0}", builder_.ToString());
            builder_.Clear();
        }

        public override void WriteLine(string format, object arg0)
        {
            builder_.Append(format);
            LogInfo(builder_.ToString(), arg0);
            builder_.Clear();
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            builder_.Append(format);
            LogInfo(builder_.ToString(), arg0, arg1);
            builder_.Clear();
        }

        public override void WriteLine(string format, params object[] arg)
        {
            builder_.Append(format);
            LogInfo(builder_.ToString(), arg);
            builder_.Clear();
        }

        public override void Write(string value)
        {
            builder_.Append(value);
        }

        public override void WriteLine(string value)
        {
            builder_.Append(value);
            LogInfo("{0}", builder_.ToString());
            builder_.Clear();
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }

    /// <summary>
    /// The error code why the application exit.
    /// </summary>
    public enum ExitCode : int
    {
        Ok = 0,
        ErrorNotStarted = 0x80,
        ErrorRunning = 0x81,
        ErrorException = 0x82,
        ErrorStopping = 0x83,
        ErrorCertificate = 0x84,
        ErrorInvalidCommandLine = 0x100
    };

    /// <summary>
    /// An exception that occured and caused an exit of the application.
    /// </summary>
    public class ErrorExitException : Exception
    {
        public ExitCode ExitCode { get; }

        public ErrorExitException(ExitCode exitCode)
        {
            ExitCode = exitCode;
        }

        public ErrorExitException()
        {
            ExitCode = ExitCode.Ok;
        }

        public ErrorExitException(string message) : base(message)
        {
            ExitCode = ExitCode.Ok;
        }

        public ErrorExitException(string message, ExitCode exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public ErrorExitException(string message, Exception innerException) : base(message, innerException)
        {
            ExitCode = ExitCode.Ok;
        }

        public ErrorExitException(string message, Exception innerException, ExitCode exitCode) : base(message, innerException)
        {
            ExitCode = exitCode;
        }
    }

    #region The certificate application message
    /// <summary>
    /// A dialog which asks for user input.
    /// </summary>
    public class ApplicationMessageDlg : IUaApplicationMessageDlg
    {
        private TextWriter output_;
        private string message_ = string.Empty;
        private bool ask_;

        public ApplicationMessageDlg(TextWriter output)
        {
            output_ = output;
        }

        public override void Message(string text, bool ask)
        {
            message_ = text;
            ask_ = ask;
        }

        public override async Task<bool> ShowAsync()
        {
            if (ask_)
            {
                var message = new StringBuilder(message_);
                message.Append(" (y/n, default y): ");
                output_.Write(message.ToString());

                try
                {
                    ConsoleKeyInfo result = Console.ReadKey();
                    output_.WriteLine();
                    return await Task.FromResult((result.KeyChar == 'y') ||
                        (result.KeyChar == 'Y') || (result.KeyChar == '\r')).ConfigureAwait(false);
                }
                catch
                {
                    // intentionally fall through
                }
            }
            else
            {
                output_.WriteLine(message_);
            }

            return await Task.FromResult(true).ConfigureAwait(false);
        }

        public override bool Show()
        {
            return ShowAsync().GetAwaiter().GetResult();
        }
    }
    #endregion

    /// <summary>
    /// Helper functions shared in various console applications.
    /// </summary>
    public static class ConsoleUtils
    {
        /// <summary>
        /// Process a command line of the console sample application.
        /// </summary>
        public static string ProcessCommandLine(
            TextWriter output,
            string[] args,
            Mono.Options.OptionSet options,
            ref bool showHelp,
            bool noExtraArgs = true)
        {
            IList<string> extraArgs = null;
            try
            {
                extraArgs = options.Parse(args);
                if (noExtraArgs)
                {
                    foreach (string extraArg in extraArgs)
                    {
                        output.WriteLine("Error: Unknown option: {0}", extraArg);
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

            return extraArgs.FirstOrDefault();
        }

        /// <summary>
        /// Configure the logging providers.
        /// </summary>
        /// <remarks>
        /// Replaces the Opc.Ua.Core default ILogger with a
        /// Microsoft.Extension.Logger with a Serilog file, debug and console logger.
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
            var loggerConfiguration = new LoggerConfiguration()
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
            var serilogger = loggerConfiguration
                .CreateLogger();

            // create the ILogger for Opc.Ua.Core
            var logger = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Trace))
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
        public static ManualResetEvent CtrlCHandler()
        {
            var quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (_, eArgs) => {
                    quitEvent.Set();
                    eArgs.Cancel = true;
                };
            }
            catch
            {
                // intentionally left blank
            }
            return quitEvent;
        }
    }
}
