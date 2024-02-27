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
using System.Collections.Generic;
using System.Threading;

using Opc.Ua;

using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer.Server
{
    /// <summary>
    /// An object that manages requests from within the server.
    /// </summary>
    public class RequestManager : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initilizes the manager.
        /// </summary>
        /// <param name="server"></param>
        public RequestManager(IUaServerData server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));

            server_       = server;
            requests_     = new Dictionary<uint,UaServerOperationContext>();
            requestTimer_ = null;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "requestTimer_")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                List<UaServerOperationContext> operations = null;

                lock (requestsLock_)
                {
                    operations = new List<UaServerOperationContext>(requests_.Values);
                    requests_.Clear();
                }

                foreach (var operation in operations)
                {
                    operation.SetStatusCode(StatusCodes.BadSessionClosed);
                }

                Utils.SilentDispose(requestTimer_);
                requestTimer_ = null;
            }
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Raised when the status of an outstanding request changes.
        /// </summary>
        public event EventHandler<RequestCancelledEventArgs> RequestCancelledEvent
        {
            add
            {
                lock (lock_)
                {
                    RequestCancelledEventHandler += value;
                }
            }

            remove
            {
                lock (lock_)
                {
                    RequestCancelledEventHandler -= value;
                }
            }
        }

        /// <summary>
        /// Called when a new request arrives.
        /// </summary>
        /// <param name="context"></param>
        public void RequestReceived(UaServerOperationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            lock (requestsLock_)
            {
                requests_.Add(context.RequestId, context);

                if (context.OperationDeadline < DateTime.MaxValue && requestTimer_ == null)
                {
                    requestTimer_ = new Timer(OnTimerExpired, null, 1000, 1000);
                }
            }
        }

        /// <summary>
        /// Called when a request completes (normally or abnormally).
        /// </summary>
        public void RequestCompleted(UaServerOperationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            lock (requestsLock_)
            {
                // remove the request.
                requests_.Remove(context.RequestId);
            }
        }

        /// <summary>
        /// Called when the client wishes to cancel one or more requests.
        /// </summary>
        public void CancelRequests(uint requestHandle, out uint cancelCount)
        {
            var cancelledRequests = new List<uint>();

            // flag requests as cancelled.
            lock (requests_)
            {
                foreach (var request in requests_.Values)
                {
                    if (request.ClientHandle == requestHandle)
                    {
                        request.SetStatusCode(StatusCodes.BadRequestCancelledByRequest);
                        cancelledRequests.Add(request.RequestId);

                        // report the AuditCancelEventType
                        server_.ReportAuditCancelEvent(request.Session.Id, requestHandle, StatusCodes.Good);
                    }
                }
            }

            // return the number of requests found.
            cancelCount = (uint)cancelledRequests.Count;

            // raise notifications.
            lock (lock_)
            {
                for (var ii = 0; ii < cancelledRequests.Count; ii++)
                {
                    if (RequestCancelledEventHandler != null)
                    {
                        try
                        {
                            RequestCancelledEventHandler(this,
                                new RequestCancelledEventArgs(cancelledRequests[ii],
                                    StatusCodes.BadRequestCancelledByRequest));
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e, "Unexpected error reporting RequestCancelledEventHandler event.");
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks for any expired requests and changes their status.
        /// </summary>
        private void OnTimerExpired(object state)
        {
            var expiredRequests = new List<uint>();

            // flag requests as expired.
            lock (requestsLock_)
            {
                // find the completed request.
                bool deadlineExists = false;

                foreach (var request in requests_.Values)
                {
                    if (request.OperationDeadline < DateTime.UtcNow)
                    {
                        request.SetStatusCode(StatusCodes.BadTimeout);
                        expiredRequests.Add(request.RequestId);
                    }
                    else if (request.OperationDeadline < DateTime.MaxValue)
                    {
                        deadlineExists = true;
                    }
                }

                // check if the timer can be cancelled.
                if (requestTimer_ != null && !deadlineExists)
                {
                    requestTimer_.Dispose();
                    requestTimer_ = null;
                }
            }

            // raise notifications.
            lock (lock_)
            {
                for (var ii = 0; ii < expiredRequests.Count; ii++)
                {
                    if (RequestCancelledEventHandler != null)
                    {
                        try
                        {
                            RequestCancelledEventHandler(this,
                                new RequestCancelledEventArgs(expiredRequests[ii], StatusCodes.BadTimeout));
                        }
                        catch (Exception e)
                        {
                            Utils.LogError(e, "Unexpected error reporting RequestCancelledEventHandler event.");
                        }                        
                    }
                }
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private IUaServerData server_;
        private Dictionary<uint,UaServerOperationContext> requests_;
        private readonly object requestsLock_ = new object();
        private Timer requestTimer_;
        private event EventHandler<RequestCancelledEventArgs> RequestCancelledEventHandler;
        #endregion
    }
}
