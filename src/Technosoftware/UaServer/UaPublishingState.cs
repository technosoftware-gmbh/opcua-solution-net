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
#endregion

namespace Technosoftware.UaServer 
{
    /// <summary>
    /// The current publishing state for a subscription.
    /// </summary>  
    public enum UaPublishingState
    {
        /// <summary>
        /// The subscription is not ready to publish.
        /// </summary>
        Idle,

        /// <summary>
        /// The subscription has notifications that are ready to publish.
        /// </summary>
        NotificationsAvailable,

        /// <summary>
        /// The has already indicated that it is waiting for a publish request.
        /// </summary>
        WaitingForPublish,

        /// <summary>
        /// The subscription has expired.
        /// </summary>
        Expired
    }
}
