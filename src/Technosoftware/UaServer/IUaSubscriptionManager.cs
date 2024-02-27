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
using System.Collections.Generic;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Provides access to the subscription manager within the server.
    /// </summary>
    /// <remarks>
    /// Sinks that receive these events must not block the thread.
    /// </remarks>
    public interface IUaSubscriptionManager
    {
        /// <summary>
        /// Raised after a new subscription is created.
        /// </summary>
        event EventHandler<Subscriptions.SubscriptionEventArgs> SubscriptionCreated;

        /// <summary>
        /// Raised before a subscription is deleted.
        /// </summary>
        event EventHandler<Subscriptions.SubscriptionEventArgs> SubscriptionDeleted;

        /// <summary>
        /// Returns all of the subscriptions known to the subscription manager.
        /// </summary>
        /// <returns>A list of the subscriptions.</returns>
        IList<Subscriptions.Subscription> GetSubscriptions();
    }
}