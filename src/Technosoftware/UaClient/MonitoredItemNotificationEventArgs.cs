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
using System;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// The event arguments provided when a new notification message arrives.
    /// </summary>
    public class MonitoredItemNotificationEventArgs : EventArgs
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal MonitoredItemNotificationEventArgs(IEncodeable notificationValue)
        {
            NotificationValue = notificationValue;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The new notification.
        /// </summary>
        public IEncodeable NotificationValue { get; }
        #endregion

        #region Private Fields
        #endregion
    }
}
