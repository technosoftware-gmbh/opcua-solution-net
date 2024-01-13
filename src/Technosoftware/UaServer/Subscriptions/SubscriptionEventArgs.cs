#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives

using System;

#endregion

namespace Technosoftware.UaServer.Subscriptions
{
    /// <summary>
    ///     A class which provides the event arguments for subscription related event.
    /// </summary>
    public class SubscriptionEventArgs : EventArgs
    {
        #region Constructors, Destructor, Initialization

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public SubscriptionEventArgs(bool deleted)
        {
            Deleted = deleted;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     True if the subscription was deleted.
        /// </summary>
        public bool Deleted { get; }

        #endregion

        #region Private Fields

        #endregion
    }
}