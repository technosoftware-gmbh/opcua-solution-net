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
using System;
using System.Runtime.Serialization;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Stores the options to use for a browse operation.
    /// </summary>
    [DataContract(Namespace = Namespaces.OpcUaXsd)]
    public class Browser
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates an unattached instance of a browser.
        /// </summary>
        public Browser()
        {
            Initialize();
        }

        /// <summary>
        /// Creates new instance of a browser and attaches it to a session.
        /// </summary>
        public Browser(IUaSession session)
        {
            Initialize();
            session_ = session;
        }

        /// <summary>
        /// Creates a copy of a browser.
        /// </summary>
        public Browser(Browser template)
        {
            Initialize();

            if (template != null)
            {
                session_ = template.session_;
                view_ = template.view_;
                maxReferencesReturned_ = template.maxReferencesReturned_;
                browseDirection_ = template.browseDirection_;
                referenceTypeId_ = template.referenceTypeId_;
                includeSubtypes_ = template.includeSubtypes_;
                nodeClassMask_ = template.nodeClassMask_;
                resultMask_ = template.resultMask_;
                continueUntilDone_ = template.continueUntilDone_;
            }
        }

        /// <summary>
        /// Sets all private fields to default values.
        /// </summary>
        private void Initialize()
        {
            session_ = null;
            view_ = null;
            maxReferencesReturned_ = 0;
            browseDirection_ = BrowseDirection.Forward;
            referenceTypeId_ = null;
            includeSubtypes_ = true;
            nodeClassMask_ = 0;
            resultMask_ = (uint)BrowseResultMask.All;
            continueUntilDone_ = false;
            browseInProgress_ = false;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The session that the browse is attached to.
        /// </summary>
        public IUaSession Session
        {
            get => session_;

            set
            {
                CheckBrowserState();
                session_ = value;
            }
        }

        /// <summary>
        /// The view to use for the browse operation.
        /// </summary>
        [DataMember(Order = 1)]
        public ViewDescription View
        {
            get => view_;

            set
            {
                CheckBrowserState();
                view_ = value;
            }
        }

        /// <summary>
        /// The maximum number of references to return in a single browse operation.
        /// </summary>
        [DataMember(Order = 2)]
        public uint MaxReferencesReturned
        {
            get => maxReferencesReturned_;

            set
            {
                CheckBrowserState();
                maxReferencesReturned_ = value;
            }
        }

        /// <summary>
        /// The direction to browse.
        /// </summary>
        [DataMember(Order = 3)]
        public BrowseDirection BrowseDirection
        {
            get => browseDirection_;

            set
            {
                CheckBrowserState();
                browseDirection_ = value;
            }
        }

        /// <summary>
        /// The reference type to follow.
        /// </summary>
        [DataMember(Order = 4)]
        public NodeId ReferenceTypeId
        {
            get => referenceTypeId_;

            set
            {
                CheckBrowserState();
                referenceTypeId_ = value;
            }
        }

        /// <summary>
        /// Whether subtypes of the reference type should be included.
        /// </summary>
        [DataMember(Order = 5)]
        public bool IncludeSubtypes
        {
            get => includeSubtypes_;

            set
            {
                CheckBrowserState();
                includeSubtypes_ = value;
            }
        }

        /// <summary>
        /// The classes of the target nodes.
        /// </summary>
        [DataMember(Order = 6)]
        public int NodeClassMask
        {
            get => Utils.ToInt32(nodeClassMask_);

            set
            {
                CheckBrowserState();
                nodeClassMask_ = Utils.ToUInt32(value);
            }
        }

        /// <summary>
        /// The results to return.
        /// </summary>
        [DataMember(Order = 7)]
        public uint ResultMask
        {
            get => resultMask_;

            set
            {
                CheckBrowserState();
                resultMask_ = value;
            }
        }

        /// <summary>
        /// Raised when a browse operation halted because of a continuation point.
        /// </summary>
        public event EventHandler<BrowserEventArgs> BrowserEvent
        {
            add => MoreReferencesEventHandler += value;
            remove => MoreReferencesEventHandler -= value;
        }

        /// <summary>
        /// Whether subsequent continuation points should be processed automatically.
        /// </summary>
        public bool ContinueUntilDone
        {
            get => continueUntilDone_;

            set
            {
                CheckBrowserState();
                continueUntilDone_ = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Browses the specified node.
        /// </summary>
        public ReferenceDescriptionCollection Browse(NodeId nodeId)
        {
            if (session_ == null)
            {
                throw new ServiceResultException(StatusCodes.BadServerNotConnected, "Cannot browse if not connected to a server.");
            }

            try
            {
                browseInProgress_ = true;

                // construct request.
                var nodeToBrowse = new BrowseDescription {
                    NodeId = nodeId,
                    BrowseDirection = browseDirection_,
                    ReferenceTypeId = referenceTypeId_,
                    IncludeSubtypes = includeSubtypes_,
                    NodeClassMask = nodeClassMask_,
                    ResultMask = resultMask_
                };


                var nodesToBrowse = new BrowseDescriptionCollection { nodeToBrowse };

                // make the call to the server.
                ResponseHeader responseHeader = session_.Browse(
                    null,
                    view_,
                    maxReferencesReturned_,
                    nodesToBrowse,
                    out BrowseResultCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                // ensure that the server returned valid results.
                ClientBase.ValidateResponse(results, nodesToBrowse);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

                // check if valid.
                if (StatusCode.IsBad(results[0].StatusCode))
                {
                    throw ServiceResultException.Create(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable);
                }

                // fetch initial set of references.
                var continuationPoint = results[0].ContinuationPoint;
                ReferenceDescriptionCollection references = results[0].References;

                // process any continuation point.
                while (continuationPoint != null)
                {
                    if (!continueUntilDone_ && MoreReferencesEventHandler != null)
                    {
                        var args = new BrowserEventArgs(references);
                        MoreReferencesEventHandler(this, args);

                        // cancel browser and return the references fetched so far.
                        if (args.Cancel)
                        {
                            _ = BrowseNext(ref continuationPoint, true);
                            return references;
                        }

                        continueUntilDone_ = args.ContinueUntilDone;
                    }

                    ReferenceDescriptionCollection additionalReferences = BrowseNext(ref continuationPoint, false);
                    if (additionalReferences != null && additionalReferences.Count > 0)
                    {
                        references.AddRange(additionalReferences);
                    }
                    else
                    {
                        Utils.LogWarning("Browser: Continuation point exists, but the browse results are null/empty.");
                        break;
                    }
                }

                // return the results.
                return references;
            }
            finally
            {
                browseInProgress_ = false;
            }
        }
        #endregion        

        #region Private Methods
        /// <summary>
        /// Checks the state of the browser.
        /// </summary>
        private void CheckBrowserState()
        {
            if (browseInProgress_)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState, "Cannot change browse parameters while a browse operation is in progress.");
            }
        }

        /// <summary>
        /// Fetches the next batch of references.
        /// </summary>
        /// <param name="continuationPoint">The continuation point.</param>
        /// <param name="cancel">if set to <c>true</c> the browse operation is cancelled.</param>
        /// <returns>The next batch of references</returns>
        private ReferenceDescriptionCollection BrowseNext(ref byte[] continuationPoint, bool cancel)
        {
            var continuationPoints = new ByteStringCollection { continuationPoint };

            // make the call to the server.
            ResponseHeader responseHeader = session_.BrowseNext(
                null,
                cancel,
                continuationPoints,
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            // ensure that the server returned valid results.
            ClientBase.ValidateResponse(results, continuationPoints);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);

            // check if valid.
            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw ServiceResultException.Create(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable);
            }

            // update continuation point.
            continuationPoint = results[0].ContinuationPoint;

            // return references.
            return results[0].References;
        }
        #endregion

        #region Private Fields
        private IUaSession session_;
        private ViewDescription view_;
        private uint maxReferencesReturned_;
        private BrowseDirection browseDirection_;
        private NodeId referenceTypeId_;
        private bool includeSubtypes_;
        private uint nodeClassMask_;
        private uint resultMask_;
        private event EventHandler<BrowserEventArgs> MoreReferencesEventHandler;
        private bool continueUntilDone_;
        private bool browseInProgress_;
        #endregion        
    }

    #region BrowserEventArgs Class
    /// <summary>
    /// The event arguments provided a browse operation returns a continuation point.
    /// </summary>
    public class BrowserEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal BrowserEventArgs(ReferenceDescriptionCollection references)
        {
            References = references;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Whether the browse operation should be cancelled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Whether subsequent continuation points should be processed automatically.
        /// </summary>
        public bool ContinueUntilDone { get; set; }

        /// <summary>
        /// The references that have been fetched so far.
        /// </summary>
        public ReferenceDescriptionCollection References { get; }

        #endregion

        #region Private Fields

        #endregion
    }
    #endregion
}
