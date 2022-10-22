#region Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.IO;
using NUnit.Framework;
using Opc.Ua;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{


    /// <summary>
    /// A NUnit trace logger replacement.
    /// </summary>
    public class NUnitTraceLogger
    {
        private TextWriter writer_;
        private int traceMasks_;

        /// <summary>
        /// Create a serilog trace logger which replaces the default logging.
        /// </summary>
        public static NUnitTraceLogger Create(
            TextWriter writer,
            ApplicationConfiguration config,
            int traceMasks)
        {
            var traceLogger = new NUnitTraceLogger(writer, traceMasks);

            // disable the built in tracing, use nunit trace output
            Utils.SetTraceMask(Utils.TraceMask & Utils.TraceMasks.StackTrace);
            Utils.SetTraceOutput(Utils.TraceOutput.Off);
            Utils.Tracing.TraceEventHandler += traceLogger.TraceEventHandler;

            return traceLogger;
        }

        public void SetWriter(TextWriter writer)
        {
            writer_ = writer;
        }

        /// <summary>
        /// Ctor of trace logger.
        /// </summary>
        private NUnitTraceLogger(TextWriter writer, int traceMasks)
        {
            writer_ = writer;
            traceMasks_ = traceMasks;
        }

        /// <summary>
        /// Callback for logging OPC UA stack trace output
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">The trace event args.</param>
        public void TraceEventHandler(object sender, TraceEventArgs e)
        {
            if ((e.TraceMask & traceMasks_) != 0)
            {
                if (e.Exception != null)
                {
                    writer_.WriteLine(e.Exception);
                }
                writer_.WriteLine(string.Format(e.Format, e.Arguments ?? Array.Empty<object>()));
            }
        }
    }
}
