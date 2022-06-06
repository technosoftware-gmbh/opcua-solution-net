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

using System.IO;
using System.Text;

using Opc.Ua;

using Technosoftware.UaServer;

#endregion

namespace SampleCompany.SampleServer.UnderlyingSystem
{
    /// <summary>
    /// Provides access to the system which stores the data.
    /// </summary>
    public class UnderlyingSystem
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Constructs a new system.
        /// </summary>
        public UnderlyingSystem(HistoricalAccessServerConfiguration configuration, ushort namespaceIndex)
        {
            configuration_ = configuration;
            namespaceIndex_ = namespaceIndex;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns a folder object for the specified node.
        /// </summary>
        public ArchiveFolderState GetFolderState(ISystemContext context, string rootId)
        {
            StringBuilder path = new StringBuilder();
            path.Append(configuration_.ArchiveRoot);
            path.Append('/');
            path.Append(rootId);

            ArchiveFolder folder = new ArchiveFolder(rootId, new DirectoryInfo(path.ToString()));
            return new ArchiveFolderState(context, folder, namespaceIndex_);
        }

        /// <summary>
        /// Returns a item object for the specified node.
        /// </summary>
        public ArchiveItemState GetItemState(ISystemContext context, ParsedNodeId parsedNodeId)
        {
            if (parsedNodeId.RootType != NodeTypes.Item)
            {
                return null;
            }

            StringBuilder path = new StringBuilder();
            path.Append(configuration_.ArchiveRoot);
            path.Append('/');
            path.Append(parsedNodeId.RootId);

            ArchiveItem item = new ArchiveItem(parsedNodeId.RootId, new FileInfo(path.ToString()));

            return new ArchiveItemState(context, item, namespaceIndex_);
        }
        #endregion

        #region Private Fields
        private ushort namespaceIndex_;
        private HistoricalAccessServerConfiguration configuration_;
        #endregion
    }
}
