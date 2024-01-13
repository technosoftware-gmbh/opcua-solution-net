#region Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved

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

namespace SampleCompany.ReferenceClient
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
}

