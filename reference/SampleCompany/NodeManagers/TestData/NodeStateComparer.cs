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
using System.Collections.Generic;

using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    /// <summary>
    /// Helper which implements IEqualityComparer for Linq queries.
    /// </summary>
    public class NodeStateComparer : IEqualityComparer<NodeState>
    {
        /// <inheritdoc/>
        public bool Equals(NodeState x, NodeState y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.NodeId == y.NodeId)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public int GetHashCode(NodeState node)
        {
            if (ReferenceEquals(node, null))
            {
                return 0;
            }

            return node.NodeId.GetHashCode();
        }
    }
}
