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
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// An interface used by the monitored items to signal the subscription.
    /// </summary>
    public interface IUaSubscription
    {
        /// <summary>
        /// The session that owns the monitored item.
        /// </summary>
        Sessions.Session Session { get; }

        /// <summary>
        /// The identifier for the item that is unique within the server.
        /// </summary>
        uint Id { get; } 

        /// <summary>
        /// Called when a monitored item is ready to publish.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        void ItemReadyToPublish(IUaMonitoredItem monitoredItem);

        /// <summary>
        /// Called when a monitored item is ready to publish.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        void ItemNotificationsAvailable(IUaMonitoredItem monitoredItem);

        /// <summary>
        /// Called when a value of monitored item is discarded in the monitoring queue.
        /// </summary>
        void QueueOverflowHandler();
    }
}