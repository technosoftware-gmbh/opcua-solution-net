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
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Class that contains data related to ConfigurationUpdating event 
    /// </summary>
    public class ConfigurationUpdatingEventArgs : EventArgs
    {
        /// <summary>
        /// The Property of <see cref="Parent"/> that should receive <see cref="NewValue"/>.
        /// </summary>
        public ConfigurationProperty ChangedProperty { get; set; }

        /// <summary>
        /// The the configuration object that should receive a <see cref="NewValue"/> in its <see cref="ChangedProperty"/>.
        /// </summary>
        public object Parent { get; set; }

        /// <summary>
        /// The new value that shall be set to the <see cref="Parent"/> in <see cref="ChangedProperty"/> property.
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// Flag that indicates if the Configuration update should be canceled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
