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
using System;
using System.Text;

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// Stores the elements of a NodeId after it is parsed.
    /// </summary>
    /// <remarks>
    /// The NodeIds used by the samples are strings with an optional path appended.
    /// The RootType identifies the type of Root Node. The RootId is the unique identifier
    /// for the Root Node. The ComponentPath is constructed from the SymbolicNames
    /// of one or more children of the Root Node. 
    /// </remarks>
    public class ParsedNodeId
    {
        #region Public Interface
        /// <summary>
        /// The namespace index that qualified the NodeId.
        /// </summary>
        public ushort NamespaceIndex { get; set; }

        /// <summary>
        /// The identifier for the root of the NodeId.
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// The type of root node.
        /// </summary>
        public int RootType { get; set; }

        /// <summary>
        /// The relative path to the component identified by the NodeId.
        /// </summary>
        public string ComponentPath { get; set; }

        /// <summary>
        /// Parses the specified node identifier.
        /// </summary>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns>The parsed node identifier. Null if the identifier cannot be parsed.</returns>
        public static ParsedNodeId Parse(NodeId nodeId)
        {
            // can only parse non-null string node identifiers.
            if (NodeId.IsNull(nodeId))
            {
                return null;
            }

            var identifier = nodeId.Identifier as string;

            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            var parsedNodeId = new ParsedNodeId {NamespaceIndex = nodeId.NamespaceIndex, RootType = 0};

            // extract the type of identifier.
            var start = 0;

            for (var ii = 0; ii < identifier.Length; ii++)
            {
                if (!char.IsDigit(identifier[ii]))
                {
                    start = ii;
                    break;
                }

                parsedNodeId.RootType *= 10;
                parsedNodeId.RootType += (byte)(identifier[ii] - '0');
            }

            if (start >= identifier.Length || identifier[start] != ':')
            {
                return null;
            }

            // extract any component path.
            var buffer = new StringBuilder();

            var index = start+1;
            var end = identifier.Length;

            var escaped = false;

            while (index < end)
            {
                var ch = identifier[index++];

                // skip any escape character but keep the one after it.
                if (ch == '&')
                {
                    escaped = true;
                    continue;
                }

                if (!escaped && ch == '?')
                {
                    end = index;
                    break;
                }

                buffer.Append(ch);
                escaped = false;
            }

            // extract any component.
            parsedNodeId.RootId = buffer.ToString();
            parsedNodeId.ComponentPath = null;

            if (end < identifier.Length)
            {
                parsedNodeId.ComponentPath = identifier.Substring(end);
            }

            return parsedNodeId;
        }


        /// <summary>
        /// Constructs a node identifier from the component pieces.
        /// </summary>
        public static NodeId Construct(int rootType, string rootId, ushort namespaceIndex, params string[] componentNames)
        {
            var pnd = new ParsedNodeId {RootType = rootType, RootId = rootId, NamespaceIndex = namespaceIndex};

            if (componentNames != null)
            {
                var path = new StringBuilder();

                foreach (var componentName in componentNames)
                {
                    if (path.Length > 0)
                    {
                        path.Append('/');
                    }

                    path.Append(componentName);
                }

                pnd.ComponentPath = path.ToString();
            }

            return pnd.Construct(null);
        }

        /// <summary>
        /// Constructs a node identifier.
        /// </summary>
        /// <returns>The node identifier.</returns>
        public NodeId Construct()
        {
            return Construct(null);
        }

        /// <summary>
        /// Constructs a node identifier for a component with the specified name.
        /// </summary>
        /// <returns>The node identifier.</returns>
        public NodeId Construct(string componentName)
        {
            var buffer = new StringBuilder();

            // add the root type.
            buffer.Append(RootType);
            buffer.Append(':');

            // add the root identifier.
            if (RootId != null)
            {
                foreach (var ch in RootId)
                {
                    // escape any special characters.
                    if (ch == '&' || ch == '?')
                    {
                        buffer.Append('&');
                    }

                    buffer.Append(ch);
                }
            }

            // add the component path.
            if (!string.IsNullOrEmpty(ComponentPath))
            {
                buffer.Append('?');
                buffer.Append(ComponentPath);
            }

            // add the component name.
            if (!string.IsNullOrEmpty(componentName))
            {
                if (string.IsNullOrEmpty(ComponentPath))
                {
                    buffer.Append('?');
                }
                else
                {
                    buffer.Append('/');
                }

                buffer.Append(componentName);
            }

            // construct the node id with the namespace index provided.
            return new NodeId(buffer.ToString(), NamespaceIndex);
        }

        /// <summary>
        /// Constructs the node identifier for a component.
        /// </summary>
        public static NodeId CreateIdForComponent(NodeState component, ushort namespaceIndex)
        {
            if (component == null)
            {
                return null;
            }

            // components must be instances with a parent.
            var instance = component as BaseInstanceState;

            if (instance?.Parent == null)
            {
                return component.NodeId;
            }

            // parent must have a string identifier.
            if (!(instance.Parent.NodeId.Identifier is string parentId))
            {
                return null;
            }

            var buffer = new StringBuilder();
            buffer.Append(parentId);

            // check if the parent is another component.
            var index = parentId.IndexOf('?');

            if (index < 0)
            {
                buffer.Append('?');
            }
            else
            {
                buffer.Append('/');
            }

            buffer.Append(component.SymbolicName);

            // return the node identifier.
            return new NodeId(buffer.ToString(), namespaceIndex);
        }
        #endregion
    }
}
