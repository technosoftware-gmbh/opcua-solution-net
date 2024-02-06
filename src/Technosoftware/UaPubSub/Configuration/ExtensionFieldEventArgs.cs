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
    /// EventArgs class for ExtensionFields Add/Remove events
    /// </summary>
    public class ExtensionFieldEventArgs : EventArgs
    {
        /// <summary>
        /// Get/Set parent PublishedDataSet configuration id
        /// </summary>
        public uint PublishedDataSetId { get; set; }

        /// <summary>
        /// Get/Set the configuration id for the ExtensionField
        /// </summary>
        public uint ExtensionFieldId { get; set; }
        /// <summary>
        /// Get/Set the ExtensionField
        /// </summary>
        public KeyValuePair ExtensionField { get; set; }
    }
}
