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
using System.Runtime.Serialization;

using Technosoftware.UaServer;
using Technosoftware.UaStandardServer;
#endregion

namespace SampleCompany.NodeManagers.Reference
{
    /// <summary>
    /// Stores the configuration the data access node manager.
    /// </summary>
    [DataContract(Namespace = Namespaces.ReferenceServer)]
    public class ReferenceServerConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ReferenceServerConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private static void Initialize()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Whether the user dialog for accepting invalid certificates should be displayed.
        /// </summary>
        [DataMember(Order = 1)]
        public bool ShowCertificateValidationDialog
        {
            get => showCertificateValidationDialog_;
            set => showCertificateValidationDialog_ = value;
        }
        #endregion

        #region Private Members
        private bool showCertificateValidationDialog_;
        #endregion
    }
}
