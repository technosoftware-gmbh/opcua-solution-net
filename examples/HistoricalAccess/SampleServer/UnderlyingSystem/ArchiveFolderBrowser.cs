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

using System;
using System.Collections.Generic;

using Opc.Ua;

#endregion

namespace SampleCompany.SampleServer.UnderlyingSystem
{
    /// <summary>
    /// Browses the references for an archive folder.
    /// </summary>
    public class ArchiveFolderBrowser : NodeBrowser
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new browser object with a set of filters.
        /// </summary>
        /// <param name="context">The system context to use.</param>
        /// <param name="view">The view which may restrict the set of references/nodes found.</param>
        /// <param name="referenceType">The type of references being followed.</param>
        /// <param name="includeSubtypes">Whether subtypes of the reference type are followed.</param>
        /// <param name="browseDirection">Which way the references are being followed.</param>
        /// <param name="browseName">The browse name of a specific target (used when translating browse paths).</param>
        /// <param name="additionalReferences">Any additional references that should be included.</param>
        /// <param name="internalOnly">If true the browser should not making blocking calls to external systems.</param>
        /// <param name="source">The segment being accessed.</param>
        public ArchiveFolderBrowser(
            ISystemContext context,
            ViewDescription view,
            NodeId referenceType,
            bool includeSubtypes,
            BrowseDirection browseDirection,
            QualifiedName browseName,
            IEnumerable<IReference> additionalReferences,
            bool internalOnly,
            ArchiveFolderState source)
        :
            base(
                context,
                view,
                referenceType,
                includeSubtypes,
                browseDirection,
                browseName,
                additionalReferences,
                internalOnly)
        {
            source_ = source;
            stage_ = Stage.Begin;
        }
        #endregion
        
        #region Overridden Methods
        /// <summary>
        /// Returns the next reference.
        /// </summary>
        /// <returns>The next reference that meets the browse criteria.</returns>
        public override IReference Next()
        {
            var system = (UnderlyingSystem)this.SystemContext.SystemHandle;

            lock (DataLock)
            {
                IReference reference = null;

                // enumerate pre-defined references.
                // always call first to ensure any pushed-back references are returned first.
                reference = base.Next();

                if (reference != null)
                {
                    return reference;
                }

                if (stage_ == Stage.Begin)
                {
                    folders_ = source_.ArchiveFolder.GetChildFolders();
                    stage_ = Stage.Folders;
                    position_ = 0;
                }

                // don't start browsing huge number of references when only internal references are requested.
                if (InternalOnly)
                {
                    return null;
                }
                
                // enumerate folders.
                if (stage_ == Stage.Folders)
                {
                    if (IsRequired(ReferenceTypeIds.Organizes, false))
                    {
                        reference = NextChild();

                        if (reference != null)
                        {
                            return reference;
                        }
                    }

                    items_ = source_.ArchiveFolder.GetItems();
                    stage_ = Stage.Items;
                    position_ = 0;
                }
                
                // enumerate items.
                if (stage_ == Stage.Items)
                {
                    if (IsRequired(ReferenceTypeIds.Organizes, false))
                    {
                        reference = NextChild();

                        if (reference != null)
                        {
                            return reference;
                        }

                        stage_ = Stage.Parents;
                        position_ = 0;
                    }
                }

                // enumerate parents.
                if (stage_ == Stage.Parents)
                {
                    if (IsRequired(ReferenceTypeIds.Organizes, true))
                    {
                        reference = NextChild();

                        if (reference != null)
                        {
                            return reference;
                        }

                        stage_ = Stage.Done;
                        position_ = 0;
                    }
                }

                // all done.
                return null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns the next child.
        /// </summary>
        private IReference NextChild()
        {
            var system = (UnderlyingSystem)this.SystemContext.SystemHandle;

            NodeId targetId = null;

            // check if a specific browse name is requested.
            if (!QualifiedName.IsNull(base.BrowseName))
            {
                // check if match found previously.
                if (position_ == Int32.MaxValue)
                {
                    return null;
                }

                // browse name must be qualified by the correct namespace.
                if (source_.BrowseName.NamespaceIndex != base.BrowseName.NamespaceIndex)
                {
                    return null;
                }

                // look for matching folder.
                if (stage_ == Stage.Folders && folders_ != null)
                {
                    for (var ii = 0; ii < folders_.Length; ii++)
                    {
                        if (base.BrowseName.Name == folders_[ii].Name)
                        {
                            targetId = ArchiveFolderState.ConstructId(folders_[ii].UniquePath, source_.NodeId.NamespaceIndex);
                            break;
                        }
                    }
                }

                // look for matching item.
                else if (stage_ == Stage.Items && items_ != null)
                {
                    for (var ii = 0; ii < items_.Length; ii++)
                    {
                        if (base.BrowseName.Name == items_[ii].Name)
                        {
                            targetId = ArchiveItemState.ConstructId(items_[ii].UniquePath, source_.NodeId.NamespaceIndex);
                            break;
                        }
                    }
                }

                // look for matching parent.
                else if (stage_ == Stage.Parents)
                {
                    var parent = source_.ArchiveFolder.GetParentFolder();

                    if (base.BrowseName.Name == parent.Name)
                    {
                        targetId = ArchiveFolderState.ConstructId(parent.UniquePath, source_.NodeId.NamespaceIndex);
                    }
                }

                position_ = Int32.MaxValue;
            }

            // return the child at the next position.
            else
            {
                // look for next folder.
                if (stage_ == Stage.Folders && folders_ != null)
                {
                    if (position_ >= folders_.Length)
                    {
                        return null;
                    }

                    targetId = ArchiveFolderState.ConstructId(folders_[position_++].UniquePath, source_.NodeId.NamespaceIndex);
                }

                // look for next item.
                else if (stage_ == Stage.Items && items_ != null)
                {
                    if (position_ >= items_.Length)
                    {
                        return null;
                    }

                    targetId = ArchiveItemState.ConstructId(items_[position_++].UniquePath, source_.NodeId.NamespaceIndex);
                }

                // look for matching parent.
                else if (stage_ == Stage.Parents)
                {
                    var parent = source_.ArchiveFolder.GetParentFolder();

                    if (parent != null)
                    {
                        targetId = ArchiveFolderState.ConstructId(parent.UniquePath, source_.NodeId.NamespaceIndex);
                    }
                }
            }

            // create reference.
            if (targetId != null)
            {
                return new NodeStateReference(ReferenceTypeIds.Organizes, false, targetId);
            }

            return null;
        }
        #endregion

        #region Stage Enumeration
        /// <summary>
        /// The stages available in a browse operation.
        /// </summary>
        private enum Stage
        {
            Begin,
            Folders,
            Items,
            Parents,
            Done
        }
        #endregion

        #region Private Fields
        private Stage stage_;
        private int position_;
        private ArchiveFolderState source_;
        private ArchiveFolder[] folders_;
        private ArchiveItem[] items_;
        #endregion
    }
}
