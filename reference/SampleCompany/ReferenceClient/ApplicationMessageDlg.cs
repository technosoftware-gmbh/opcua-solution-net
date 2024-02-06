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
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Technosoftware.UaConfiguration;
#endregion

namespace SampleCompany.ReferenceClient
{
    /// <summary>
    /// A dialog which asks for user input.
    /// </summary>
    public class ApplicationMessageDlg : IUaApplicationMessageDlg
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Generic Message Dialog for issues during loading of an application
        /// </summary>
        /// <param name="output">The TextWriter to use for the output</param>
        public ApplicationMessageDlg(TextWriter output)
        {
            output_ = output;
        }
        #endregion

        #region Overridden Methods
        /// <inheritdoc/>
        public override void Message(string text, bool ask = false)
        {
            message_ = text;
            ask_ = ask;
        }

        /// <inheritdoc/>
        public override async Task<bool> ShowAsync()
        {
            if (ask_)
            {
                var message = new StringBuilder(message_);
                _ = message.Append(" (y/n, default y): ");
                await output_.WriteAsync(message.ToString()).ConfigureAwait(false);

                try
                {
                    ConsoleKeyInfo result = Console.ReadKey();
                    await output_.WriteLineAsync().ConfigureAwait(false);
                    return await Task.FromResult(result.KeyChar == 'y' ||
                        result.KeyChar == 'Y' || result.KeyChar == '\r').ConfigureAwait(false);
                }
                catch
                {
                    // intentionally fall through
                }
            }
            else
            {
                await output_.WriteLineAsync(message_).ConfigureAwait(false);
            }

            return await Task.FromResult(true).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override bool Show()
        {
            return ShowAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region Private Fields
        private readonly TextWriter output_;
        private string message_ = string.Empty;
        private bool ask_;
        #endregion
    }
}

