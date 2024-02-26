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
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using Microsoft.Extensions.Logging;

using Opc.Ua;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    public class NUnitTestLogger<T> : ILogger<T>
    {
        /// <summary>
        /// Create a nunit trace logger which replaces the default logging.
        /// </summary>
        public static NUnitTestLogger<T> Create(
            TextWriter writer,
            ApplicationConfiguration config,
            int traceMasks)
        {
            var traceLogger = new NUnitTestLogger<T>(writer);

            // disable the built in tracing, use nunit trace output
            Utils.SetTraceMask(Utils.TraceMask & Utils.TraceMasks.StackTrace);
            Utils.SetTraceOutput(Utils.TraceOutput.Off);
            Utils.SetLogger(traceLogger);

            return traceLogger;
        }

        private NUnitTestLogger(TextWriter outputWriter)
        {
            outputWriter_ = outputWriter;
        }

        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= MinimumLogLevel;
        }

        public void SetWriter(TextWriter outputWriter)
        {
            Interlocked.Exchange(ref outputWriter_, outputWriter);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel < MinimumLogLevel)
            {
                return;
            }

            try
            {
                var sb = new StringBuilder();
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0:yy-MM-dd HH:mm:ss.fff}: ", DateTime.UtcNow);
                sb.Append(formatter(state, exception));

                var logEntry = sb.ToString();

                outputWriter_.WriteLine(logEntry);
            }
            catch
            {
                // intentionally ignored
            }
        }

        private TextWriter outputWriter_;
    }


}
