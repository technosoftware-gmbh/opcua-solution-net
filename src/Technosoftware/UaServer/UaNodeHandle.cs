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
using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Stores information about a NodeId specified by the client.
    /// </summary>
    /// <remarks>
    /// A UaNodeHandle is created when GetManagerHandle is called and will only contain
    /// information found by parsing the NodeId. The ValidateNode method is used to 
    /// verify that the NodeId refers to a real Node and find a NodeState object that 
    /// can be used to access the Node.
    /// </remarks>
    public class UaNodeHandle
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        ///     Initializes a new instance of the <see cref="UaNodeHandle" /> class.
        /// </summary>
        public UaNodeHandle()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UaNodeHandle" /> class.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="node">The node.</param>
        public UaNodeHandle(NodeId nodeId, NodeState node)
        {
            this.NodeId = nodeId;
            this.Validated = true;
            this.Node = node;
        }
        #endregion
        
        #region Public Interface
        /// <summary>
        /// The NodeId provided by the client.
        /// </summary>
        public NodeId NodeId { get; set; }

        /// <summary>
        /// The parsed identifier (must not be null if Validated == False).
        /// </summary>
        public object ParsedNodeId { get; set; }

        /// <summary>
        /// A unique string identifier for the root of a complex object tree.
        /// </summary>
        public NodeId RootId { get; set; }

        /// <summary>
        /// A path to a component within the tree identified by the root id.
        /// </summary>
        public string ComponentPath { get; set; }

        /// <summary>
        /// An index associated with the handle.
        /// </summary>
        /// <remarks>
        /// This is used to keep track of the position in the complete list of Nodes provided by the Client.
        /// </remarks>
        public int Index { get; set; }

        /// <summary>
        /// Whether the handle has been validated.
        /// </summary>
        /// <remarks>
        /// When validation is complete the Node property must have a valid object.
        /// </remarks>
        public bool Validated { get; set; }

        /// <summary>
        /// An object that can be used to access the Node identified by the NodeId.
        /// </summary>
        /// <remarks>
        /// Not set until after the handle is validated.
        /// </remarks>
        public NodeState Node { get; set; }

        /// <summary>
        /// An object that can be used to manage the items which are monitoring the node.
        /// </summary>
        public IUaMonitoredNode MonitoredNode { get; set; }
        #endregion
    }
}
