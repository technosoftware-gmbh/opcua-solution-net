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
using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// Represents the event arguments provided when publish response
    /// sequence numbers are about to be achknoledged with a publish request.
    /// </summary>
    /// <remarks>
    /// A callee can defer an acknowledge to the next publish request by
    /// moving the <see cref="SubscriptionAcknowledgement"/> to the deferred list.
    /// The callee can modify the list of acknowledgements to send, it is the
    /// responsibility of the caller to protect the lists for modifications.
    /// </remarks>
    public class PublishSequenceNumbersToAcknowledgeEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal PublishSequenceNumbersToAcknowledgeEventArgs(
            SubscriptionAcknowledgementCollection acknowledgementsToSend,
            SubscriptionAcknowledgementCollection deferredAcknowledgementsToSend)
        {
            AcknowledgementsToSend = acknowledgementsToSend;
            deferredAcknowledgementsToSend_ = deferredAcknowledgementsToSend;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The acknowledgements which are sent with the next publish request.
        /// </summary>
        /// <remarks>
        /// A client may also chose to remove an acknowledgement from this list to add it back
        /// to the list in a subsequent callback when the request is fully processed.
        /// </remarks>
        public SubscriptionAcknowledgementCollection AcknowledgementsToSend { get; }

        /// <summary>
        /// The deferred list of acknowledgements.
        /// </summary>
        /// <remarks>
        /// The callee can transfer an outstanding <see cref="SubscriptionAcknowledgement"/>
        /// to this list to defer the acknowledge of a sequence number to the next publish request.
        /// </remarks>
        public SubscriptionAcknowledgementCollection DeferredAcknowledgementsToSend => deferredAcknowledgementsToSend_;
        #endregion

        #region Private Fields
        private readonly SubscriptionAcknowledgementCollection deferredAcknowledgementsToSend_;
        #endregion
    }

}
