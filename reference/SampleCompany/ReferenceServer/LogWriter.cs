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
using System.IO;
using System.Text;
using static Opc.Ua.Utils;
#endregion

namespace SampleCompany.ReferenceServer
{
    /// <summary>
    /// The log output implementation of a TextWriter.
    /// </summary>
    public class LogWriter : TextWriter
    {
        private readonly StringBuilder builder_ = new StringBuilder();

        public override void Write(char value)
        {
            _ = builder_.Append(value);
        }

        public override void WriteLine(char value)
        {
            _ = builder_.Append(value);
            LogInfo("{0}", builder_.ToString());
            _ = builder_.Clear();
        }

        public override void WriteLine()
        {
            LogInfo("{0}", builder_.ToString());
            _ = builder_.Clear();
        }

        public override void WriteLine(string format, object arg0)
        {
            _ = builder_.Append(format);
            LogInfo(builder_.ToString(), arg0);
            _ = builder_.Clear();
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            _ = builder_.Append(format);
            LogInfo(builder_.ToString(), arg0, arg1);
            _ = builder_.Clear();
        }

        public override void WriteLine(string format, params object[] arg)
        {
            _ = builder_.Append(format);
            LogInfo(builder_.ToString(), arg);
            _ = builder_.Clear();
        }

        public override void Write(string value)
        {
            _ = builder_.Append(value);
        }

        public override void WriteLine(string value)
        {
            _ = builder_.Append(value);
            LogInfo("{0}", builder_.ToString());
            _ = builder_.Clear();
        }

        public override Encoding Encoding => Encoding.Default;
    }
}

