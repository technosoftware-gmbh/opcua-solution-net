#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives

using System.Collections.Generic;

using Opc.Ua;

using Technosoftware.UaServer;

#endregion

namespace SampleCompany.SampleServer.UnderlyingSystem
{
    /// <summary>
    /// Stores the metadata for a node representing a folder on a file system.
    /// </summary>
    public class ArchiveFolderState : Opc.Ua.FolderState
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new instance of a folder.
        /// </summary>
        public ArchiveFolderState(ISystemContext context, ArchiveFolder folder, ushort namespaceIndex)
        : 
            base(null)
        {
            ArchiveFolder = folder;

            this.TypeDefinitionId = ObjectTypeIds.FolderType;
            this.SymbolicName = folder.Name;
            this.NodeId = ConstructId(folder.UniquePath, namespaceIndex);
            this.BrowseName = new QualifiedName(folder.Name, namespaceIndex);
            this.DisplayName = new LocalizedText(this.BrowseName.Name);
            this.Description = null;
            this.WriteMask = 0;
            this.UserWriteMask = 0;
            this.EventNotifier = EventNotifiers.None;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Constructs a node identifier for a folder object.
        /// </summary>
        public static NodeId ConstructId(string filePath, ushort namespaceIndex)
        {
            ParsedNodeId parsedNodeId = new ParsedNodeId();

            parsedNodeId.RootId = filePath;
            parsedNodeId.NamespaceIndex = namespaceIndex;
            parsedNodeId.RootType = NodeTypes.Folder;

            return parsedNodeId.Construct();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The physical folder referenced by the node.
        /// </summary>
        public ArchiveFolder ArchiveFolder { get; }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Creates a browser that explores the structure of the block.
        /// </summary>
        public override INodeBrowser CreateBrowser(
            ISystemContext context,
            ViewDescription view,
            NodeId referenceType,
            bool includeSubtypes,
            BrowseDirection browseDirection,
            QualifiedName browseName,
            IEnumerable<IReference> additionalReferences,
            bool internalOnly)
        {
            NodeBrowser browser = new ArchiveFolderBrowser(
                context,
                view,
                referenceType,
                includeSubtypes,
                browseDirection,
                browseName,
                additionalReferences,
                internalOnly,
                this);

            PopulateBrowser(context, browser);

            return browser;
        }
        #endregion
    }
}
