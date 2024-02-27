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

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// Stores a reference between NodeManagers that is needs to be created or deleted.
    /// </summary>
    public class LocalReference
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the the reference.
        /// </summary>
        public LocalReference(
            NodeId sourceId,
            NodeId referenceTypeId,
            bool isInverse,
            NodeId targetId)
        {
            SourceId = sourceId;
            ReferenceTypeId = referenceTypeId;
            IsInverse = isInverse;
            TargetId = targetId;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The source of the reference.
        /// </summary>
        public NodeId SourceId { get; }

        /// <summary>
        /// The type of reference.
        /// </summary>
        public NodeId ReferenceTypeId { get; }

        /// <summary>
        /// True is the reference is an inverse reference.
        /// </summary>
        public bool IsInverse { get; }

        /// <summary>
        /// The target of the reference.
        /// </summary>
        public NodeId TargetId { get; }

        #endregion
    }
}