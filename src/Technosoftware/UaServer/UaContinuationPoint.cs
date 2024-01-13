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

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// The table of all reference types known to the server.
    /// </summary>
    /// <remarks>This class is thread safe.</remarks>
    public class UaContinuationPoint : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object with default values.
        /// </summary>
        public UaContinuationPoint()
        {
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Utils.SilentDispose(Data);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// A unique identifier for the continuation point.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The node manager that created the continuation point.
        /// </summary>
        public IUaBaseNodeManager Manager { get; set; }

        /// <summary>
        /// The view being browsed.
        /// </summary>
        public ViewDescription View { get; set; }

        /// <summary>
        /// The node being browsed.
        /// </summary>
        public object NodeToBrowse { get; set; }

        /// <summary>
        /// The maximum number of results to return.
        /// </summary>
        public uint MaxResultsToReturn { get; set; }

        /// <summary>
        /// What direction to follow the references.
        /// </summary>                
        public BrowseDirection BrowseDirection { get; set; }

        /// <summary>
        /// The reference type of the references to return.
        /// </summary>                
        public NodeId ReferenceTypeId { get; set; }

        /// <summary>
        /// Whether subtypes of the reference type should be return as well.
        /// </summary>        
        public bool IncludeSubtypes { get; set; }

        /// <summary>
        /// The node class of the target nodes for the references to return.
        /// </summary>  
        public uint NodeClassMask { get; set; }

        /// <summary>
        /// The values to return.
        /// </summary>  
        public BrowseResultMask ResultMask { get; set; }

        /// <summary>
        /// The index where browsing halted.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Node manager specific data that is necessary to continue the browse.
        /// </summary>
        /// <remarks>
        /// A node manager needs to hold onto unmanaged resources to continue the browse.
        /// If this is the case then the object stored here must implement the Idispose 
        /// interface. This will ensure the unmanaged resources are freed if the continuation
        /// point expires.
        /// </remarks>
        public object Data { get; set; }

        /// <summary>
        /// Whether the ReferenceTypeId should be returned in the result.
        /// </summary>
        public bool ReferenceTypeIdRequired
        {
            get { return (ResultMask & BrowseResultMask.ReferenceTypeId) != 0; }
        }

        /// <summary>
        /// Whether the IsForward flag should be returned in the result.
        /// </summary>
        public bool IsForwardRequired
        {
            get { return (ResultMask & BrowseResultMask.IsForward) != 0; }
        }

        /// <summary>
        /// Whether the NodeClass should be returned in the result.
        /// </summary>
        public bool NodeClassRequired
        {
            get { return (ResultMask & BrowseResultMask.NodeClass) != 0; }
        }

        /// <summary>
        /// Whether the BrowseName should be returned in the result.
        /// </summary>
        public bool BrowseNameRequired
        {
            get { return (ResultMask & BrowseResultMask.BrowseName) != 0; }
        }

        /// <summary>
        /// Whether the DisplayName should be returned in the result.
        /// </summary>
        public bool DisplayNameRequired
        {
            get { return (ResultMask & BrowseResultMask.DisplayName) != 0; }
        }

        /// <summary>
        /// Whether the TypeDefinition should be returned in the result.
        /// </summary>
        public bool TypeDefinitionRequired
        {
            get { return (ResultMask & BrowseResultMask.TypeDefinition) != 0; }
        }
        
        /// <summary>
        /// False if it is not necessary to read the attributes a target node.
        /// </summary>
        /// <remarks>
        /// This flag is true if the NodeClass filter is set or the target node attributes are returned in the result.
        /// </remarks>
        public bool TargetAttributesRequired
        {
            get
            {
                if (NodeClassMask != 0)
                {
                    return true;
                }

                return (ResultMask & (BrowseResultMask.NodeClass | BrowseResultMask.BrowseName | BrowseResultMask.DisplayName | BrowseResultMask.TypeDefinition)) != 0;
            }
        }
        #endregion

        #region Private Fields

        #endregion
    }
}
