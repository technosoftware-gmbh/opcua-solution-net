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

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Configuration
{
    /// <summary>
    /// EventArgs class for <see cref="PubSubState"/> Change events
    /// </summary>
    public class PubSubStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Reference to the object whose <see cref="PubSubState"/> was changed
        /// </summary>
        public object ConfigurationObject { get; set; }

        /// <summary>
        /// Configuration Id of the object whose <see cref="PubSubState"/> was changed
        /// </summary>
        public uint ConfigurationObjectId { get; set; }

        /// <summary>
        /// New <see cref="PubSubState"/> 
        /// </summary>
        public PubSubState NewState { get; set; }

        /// <summary>
        /// Old <see cref="PubSubState"/> 
        /// </summary>
        public PubSubState OldState { get; set; }
    }
}
