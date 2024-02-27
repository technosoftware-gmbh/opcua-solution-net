#region Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System.Threading.Tasks;
#endregion

namespace Technosoftware.UaConfiguration
{
    /// <summary>
    /// Generic Message Dialog for issues during loading of an application
    /// </summary>
    public abstract class IUaApplicationMessageDlg
    {
        /// <summary>
        /// Defines the message to show and if the user is asked for acceptance or not.
        /// </summary>
        /// <param name="text">The message to show.</param>
        /// <param name="ask">True if a yes/no option is given to the user.</param>
        public abstract void Message(string text, bool ask = false);

        /// <summary>
        /// Show the message
        /// </summary>
        /// <returns>True if user answered yes; otherwise false.</returns>
        public abstract bool Show();

        /// <summary>
        /// Asynchronous version of showing the message
        /// </summary>
        /// <returns>True if user answered yes; otherwise false.</returns>
        public abstract Task<bool> ShowAsync();
    }
}
