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
    /// EventArgs class for <see cref="PublishedDataSetDataType"/> Add/Remove events
    /// </summary>
    public class PublishedDataSetEventArgs : EventArgs
    {
        /// <summary>
        /// ConfigurationId of <see cref="PublishedDataSetDataType"/> object
        /// </summary>
        public uint PublishedDataSetId { get; set; }

        /// <summary>
        /// Reference to <see cref="PublishedDataSetDataType"/> object
        /// </summary>
        public PublishedDataSetDataType PublishedDataSetDataType { get; set; }
    }
}
