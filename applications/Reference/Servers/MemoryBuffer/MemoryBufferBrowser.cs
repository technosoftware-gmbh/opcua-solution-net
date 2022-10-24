#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Threading;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace MemoryBuffer
{
    /// <summary>
    /// A class to browse the references for a memory buffer. 
    /// </summary>
    public class MemoryBufferBrowser : NodeBrowser
    {
        #region Constructors
        /// <summary>
        /// Creates a new browser object with a set of filters.
        /// </summary>
        public MemoryBufferBrowser(
            ISystemContext context,
            ViewDescription view,
            NodeId referenceType,
            bool includeSubtypes,
            BrowseDirection browseDirection,
            QualifiedName browseName,
            IEnumerable<IReference> additionalReferences,
            bool internalOnly,
            MemoryBufferState buffer)
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
            buffer_ = buffer;
            stage_ = Stage.Begin;
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Returns the next reference.
        /// </summary>
        /// <returns></returns>
        public override IReference Next()
        {
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
                    stage_ = Stage.Components;
                    position_ = 0;
                }

                // don't start browsing huge number of references when only internal references are requested.
                if (InternalOnly)
                {
                    return null;
                }

                // enumerate components.
                if (stage_ == Stage.Components)
                {
                    if (IsRequired(ReferenceTypeIds.HasComponent, false))
                    {
                        reference = NextChild();

                        if (reference != null)
                        {
                            return reference;
                        }
                    }

                    stage_ = Stage.ModelParents;
                    position_ = 0;
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
            MemoryTagState tag = null;

            // check if a specific browse name is requested.
            if (!QualifiedName.IsNull(base.BrowseName))
            {
                // check if match found previously.
                if (position_ == UInt32.MaxValue)
                {
                    return null;
                }

                // browse name must be qualified by the correct namespace.
                if (buffer_.TypeDefinitionId.NamespaceIndex != base.BrowseName.NamespaceIndex)
                {
                    return null;
                }

                string name = base.BrowseName.Name;

                for (int ii = 0; ii < name.Length; ii++)
                {
                    if ("0123456789ABCDEF".IndexOf(name[ii]) == -1)
                    {
                        return null;
                    }
                }

                position_ = Convert.ToUInt32(name, 16);

                // check for memory overflow.
                if (position_ >= buffer_.SizeInBytes.Value)
                {
                    return null;
                }

                tag = new MemoryTagState(buffer_, position_);
                position_ = UInt32.MaxValue;
            }

            // return the child at the next position.
            else
            {
                if (position_ >= buffer_.SizeInBytes.Value)
                {
                    return null;
                }

                tag = new MemoryTagState(buffer_, position_);
                position_ += buffer_.ElementSize;

                // check for memory overflow.
                if (position_ >= buffer_.SizeInBytes.Value)
                {
                    return null;
                }
            }

            return new NodeStateReference(ReferenceTypeIds.HasComponent, false, tag);
        }
        #endregion

        #region Stage Enumeration
        /// <summary>
        /// The stages available in a browse operation.
        /// </summary>
        private enum Stage
        {
            Begin,
            Components,
            ModelParents,
            Done
        }
        #endregion

        #region Private Fields
        private Stage stage_;
        private uint position_;
        private MemoryBufferState buffer_;
        #endregion
    }
}
