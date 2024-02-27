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
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// A generic implementation for ISystemContext interface.
    /// </summary>
    public class UaServerContext : SystemContext
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemContext"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public UaServerContext(IUaServerData server)
        {
            OperationContext = null;
            NamespaceUris = server.NamespaceUris;
            ServerUris = server.ServerUris;
            TypeTable = server.TypeTree;
            EncodeableFactory = server.Factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemContext"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="context">The context.</param>
        public UaServerContext(IUaServerData server, UaServerOperationContext context)
        {
            OperationContext = context;
            NamespaceUris = server.NamespaceUris;
            ServerUris = server.ServerUris;
            TypeTable = server.TypeTree;
            EncodeableFactory = server.Factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemContext"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="session">The session.</param>
        public UaServerContext(IUaServerData server, Sessions.Session session)
        {
            OperationContext = null;
            SessionId = session.Id;
            UserIdentity = session.Identity;
            PreferredLocales = session.PreferredLocales;
            NamespaceUris = server.NamespaceUris;
            ServerUris = server.ServerUris;
            TypeTable = server.TypeTree;
            EncodeableFactory = server.Factory;
        }
        #endregion

        #region Public Members
        /// <summary>
        /// The operation context associated with system context.
        /// </summary>
        /// <value>The operation context.</value>
        public new UaServerOperationContext OperationContext
        {
            get => base.OperationContext as UaServerOperationContext;
            set => base.OperationContext = value;
        }

        /// <summary>
        /// Creates a copy of the context that can be used with the specified operation context.
        /// </summary>
        /// <returns>A copy of the system context.</returns>
        public UaServerContext Copy()
        {
            return (UaServerContext)MemberwiseClone();
        }

        /// <summary>
        /// Creates a copy of the context that can be used with the specified operation context.
        /// </summary>
        /// <param name="context">The operation context to use.</param>
        /// <returns>
        /// A copy of the system context that references the new operation context.
        /// </returns>
        public UaServerContext Copy(UaServerOperationContext context)
        {
            var copy = (UaServerContext)MemberwiseClone();

            if (context != null)
            {
                copy.OperationContext = context;
            }

            return copy;
        }

        /// <summary>
        /// Creates a copy of the context that can be used with the specified session.
        /// </summary>
        /// <param name="session">The session to use.</param>
        /// <returns>
        /// A copy of the system context that references the new session.
        /// </returns>
        public UaServerContext Copy(Sessions.Session session)
        {
            var copy = (UaServerContext)MemberwiseClone();

            copy.OperationContext = null;

            if (session != null)
            {
                copy.SessionId = session.Id;
                copy.UserIdentity = session.Identity;
                copy.PreferredLocales = session.PreferredLocales;
            }
            else
            {
                copy.SessionId = null;
                copy.UserIdentity = null;
                copy.PreferredLocales = null;
            }

            return copy;
        }

        /// <summary>
        /// Creates a copy of the context that can be used with the specified server context.
        /// </summary>
        /// <param name="context">The server context to use.</param>
        /// <returns>
        /// A copy of the system context that references the new server context.
        /// </returns>
        public UaServerContext Copy(UaServerContext context)
        {
            var copy = (UaServerContext)MemberwiseClone();

            if (context != null)
            {
                copy.OperationContext = context.OperationContext;
                copy.SessionId = context.SessionId;
                copy.UserIdentity = context.UserIdentity;
                copy.PreferredLocales = context.PreferredLocales;
                copy.NamespaceUris = context.NamespaceUris;
                copy.ServerUris = context.ServerUris;
                copy.TypeTable = context.TypeTable;
                copy.EncodeableFactory = context.EncodeableFactory;
            }

            return copy;
        }
        #endregion
    }
}
