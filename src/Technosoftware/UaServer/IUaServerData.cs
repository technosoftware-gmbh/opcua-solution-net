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

using Opc.Ua;

using Technosoftware.UaServer.Aggregates;
using Technosoftware.UaServer.Diagnostics;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Server;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    ///     <para>IUaServerData is an interface to a server object that provides the shared components for the UA Server.</para>
    ///     <para>
    ///         The different manager objects use this interface to access shared tables such as the set of NamespaceUris or
    ///         the ServerStatus.
    ///     </para>
    ///     <para>
    ///         Any implementation of IUaServerData must be thread safe. The flow of calls must always be one way � from one
    ///         component of the server to the IUaServerData
    ///         object. Properties of the IUaServerData may return other objects which will have their own rules regarding to
    ///         call flow.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The BaseServerData object is the standard implementation of this interface. Developers can extend this class to add
    ///     additional shared data.
    /// </remarks>
    public interface IUaServerData : IAuditEventServer
    {
        /// <summary>
        /// The endpoint addresses used by the server.
        /// </summary>
        /// <value>The endpoint addresses.</value>
        IEnumerable<Uri> EndpointAddresses { get; }

        /// <summary>
        /// The context to use when serializing/deserializing extension objects.
        /// </summary>
        /// <value>The message context.</value>
        IServiceMessageContext MessageContext { get; }

        /// <summary>
        /// The default system context for the server.
        /// </summary>
        /// <value>The default system context.</value>
        UaServerContext DefaultSystemContext { get; }

        /// <summary>
        /// The table of namespace uris known to the server.
        /// </summary>
        /// <value>The namespace URIs.</value>
        NamespaceTable NamespaceUris { get; }

        /// <summary>
        /// The table of remote server uris known to the server.
        /// </summary>
        /// <value>The server URIs.</value>
        StringTable ServerUris { get; }

        /// <summary>
        /// The factory used to create encodeable objects that the server understands.
        /// </summary>
        /// <value>The factory.</value>
        IEncodeableFactory Factory { get; }

        /// <summary>
        /// The datatypes, object types and variable types known to the server.
        /// </summary>
        /// <value>The type tree.</value>
        /// <remarks>
        /// The type tree table is a global object that all components of a server have access to.
        /// Node managers must populate this table with all types that they define. 
        /// This object is thread safe.
        /// </remarks>
        TypeTable TypeTree { get; }

        /// <summary>
        /// The master node manager for the server.
        /// </summary>
        /// <value>The node manager.</value>
        MasterNodeManager NodeManager { get; }

        /// <summary>
        /// The internal node manager for the servers.
        /// </summary>
        /// <value>The core node manager.</value>
        CoreNodeManager CoreNodeManager { get; }

        /// <summary>
        ///     Returns the node manager that manages the server diagnostics.
        /// </summary>
        /// <value>The diagnostics node manager.</value>
        DiagnosticsNodeManager DiagnosticsNodeManager { get; }

        /// <summary>
        /// The manager for events that all components use to queue events that occur.
        /// </summary>
        /// <value>The event manager.</value>
        EventManager EventManager { get; }

        /// <summary>
        /// A manager for localized resources that components can use to localize text.
        /// </summary>
        /// <value>The resource manager.</value>
        ResourceManager ResourceManager { get; }

        /// <summary>
        /// A manager for outstanding requests that allows components to receive notifications if the timeout or are cancelled.
        /// </summary>
        /// <value>The request manager.</value>
        RequestManager RequestManager { get; }

        /// <summary>
        /// A manager for aggregate calculators supported by the server.
        /// </summary>
        /// <value>The aggregate manager.</value>
        AggregateManager AggregateManager { get; }

        /// <summary>
        /// The manager for active sessions.
        /// </summary>
        /// <value>The session manager.</value>
        IUaSessionManager SessionManager { get; }

        /// <summary>
        /// The manager for active subscriptions.
        /// </summary>
        IUaSubscriptionManager SubscriptionManager { get; }

        /// <summary>
        /// Whether the server is currently running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This flag is set to false when the server shuts down. Threads running should check this flag whenever
        /// they return from a blocking operation. If it is false the thread should clean up and terminate.
        /// </remarks>
        bool IsRunning { get; }

        /// <summary>
        /// Returns the status object for the server. Use Status.Lock to make it thread safe
        /// </summary>
        /// <example>
        /// lock (server.Status.Lock)
        /// {
        ///    server.Status.Variable.CurrentTime.MinimumSamplingInterval = 250;
        /// }
        /// </example>
        /// <value>The status.</value>
        ServerStatusValue Status { get; }

        /// <summary>
        /// Gets or sets the current state of the server.
        /// </summary>
        /// <value>The state of the current.</value>
        ServerState CurrentState { get; set; }

        /// <summary>
        /// Returns the Server object node
        /// </summary>
        /// <value>The Server object node.</value>
        ServerObjectState ServerObject { get; }

        /// <summary>
        /// Used to synchronize access to the server diagnostics.
        /// </summary>
        /// <value>The diagnostics lock.</value>
        object DiagnosticsLock { get; }

        /// <summary>
        /// Used to synchronize write access to the server diagnostics.
        /// </summary>
        /// <value>The diagnostics lock.</value>
        object DiagnosticsWriteLock { get; }

        /// <summary>
        /// Returns the diagnostics structure for the server.
        /// </summary>
        /// <value>The server diagnostics.</value>
        ServerDiagnosticsSummaryDataType ServerDiagnostics { get; }

        /// <summary>
        /// Whether the server is collecting diagnostics.
        /// </summary>
        /// <value><c>true</c> if diagnostics is enabled; otherwise, <c>false</c>.</value>
        bool DiagnosticsEnabled { get; }

        /// <summary>
        /// Closes the specified session.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="deleteSubscriptions">if set to <c>true</c> subscriptions are to be deleted.</param>
        void CloseSession(UaServerOperationContext context, NodeId sessionId, bool deleteSubscriptions);

        /// <summary>
        /// Deletes the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        void DeleteSubscription(uint subscriptionId);

        /// <summary>
        /// Called by any component to report a global event.
        /// </summary>
        /// <param name="e">The event.</param>
        void ReportEvent(IFilterTarget e);

        /// <summary>
        /// Called by any component to report a global event.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="e">The event.</param>
        void ReportEvent(ISystemContext context, IFilterTarget e);

        /// <summary>
        /// Refreshes the conditions for the specified subscription.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        void ConditionRefresh(UaServerOperationContext context, uint subscriptionId);

        /// <summary>
        /// Refreshes the conditions for the specified subscription and monitored item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="monitoredItemId">The monitored item identifier.</param>
        void ConditionRefresh2(UaServerOperationContext context, uint subscriptionId, uint monitoredItemId);

    }
}
