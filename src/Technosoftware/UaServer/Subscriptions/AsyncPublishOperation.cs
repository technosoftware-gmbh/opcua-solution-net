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

using Opc.Ua;

using Technosoftware.UaServer.Server;

#endregion

namespace Technosoftware.UaServer.Subscriptions 
{
    /// <summary>
    /// Stores the state of an asynchrounous publish operation.
    /// </summary>  
    public class AsyncPublishOperation
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncPublishOperation"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <param name="server">The server.</param>
        public AsyncPublishOperation(
             UaServerOperationContext context,
             IEndpointIncomingRequest request,
             UaGenericServer server)
        {
            Context = context;
            request_ = request;
            server_ = server;
            Response = new PublishResponse();
            request_.Calldata = this;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {   
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                request_.OperationCompleted(null, StatusCodes.BadServerHalted);
            }
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public UaServerOperationContext Context { get; }

        /// <summary>
        /// Gets the request handle.
        /// </summary>
        /// <value>The request handle.</value>
        public uint RequestHandle
        {
            get { return request_.Request.RequestHeader.RequestHandle; }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public PublishResponse Response { get; }

        /// <summary>
        /// Gets the calldata.
        /// </summary>
        /// <value>The calldata.</value>
        public object Calldata { get; private set; }

        /// <summary>
        /// Schedules a thread to complete the request.
        /// </summary>
        /// <param name="calldata">The data that is used to complete the operation</param>
        public void CompletePublish(object calldata)
        {
            Calldata = calldata;
            server_.ScheduleIncomingRequest(request_);
        }
        #endregion

        #region Private Fields
        private IEndpointIncomingRequest request_;
        private UaGenericServer server_;

        #endregion
    }
}
