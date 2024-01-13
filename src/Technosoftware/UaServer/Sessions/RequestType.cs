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

#endregion

using Opc.Ua;

namespace Technosoftware.UaServer.Sessions
{
    /// <summary>
    /// The set of all service request types (used for collecting diagnostics and checking permissions).
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// The request type is not known.
        /// </summary>
		Unknown,

        /// <see cref="IDiscoveryServer.FindServers" />
		FindServers,

        /// <see cref="IDiscoveryServer.GetEndpoints" />
		GetEndpoints,

        /// <see cref="ISessionServer.CreateSession" />
		CreateSession,

        /// <see cref="ISessionServer.ActivateSession" />
		ActivateSession,

        /// <see cref="ISessionServer.CloseSession" />
		CloseSession,

        /// <see cref="ISessionServer.Cancel" />
		Cancel,

        /// <see cref="ISessionServer.Read" />
		Read,

        /// <see cref="ISessionServer.HistoryRead" />
		HistoryRead,

        /// <see cref="ISessionServer.Write" />
		Write,

        /// <see cref="ISessionServer.HistoryUpdate" />
		HistoryUpdate,

        /// <see cref="ISessionServer.Call" />
		Call,

        /// <see cref="ISessionServer.CreateMonitoredItems" />
		CreateMonitoredItems,

        /// <see cref="ISessionServer.ModifyMonitoredItems" />
		ModifyMonitoredItems,

        /// <see cref="ISessionServer.SetMonitoringMode" />
		SetMonitoringMode,

        /// <see cref="ISessionServer.SetTriggering" />
		SetTriggering,

        /// <see cref="ISessionServer.DeleteMonitoredItems" />
		DeleteMonitoredItems,

        /// <see cref="ISessionServer.CreateSubscription" />
		CreateSubscription,

        /// <see cref="ISessionServer.ModifySubscription" />
		ModifySubscription,

        /// <see cref="ISessionServer.SetPublishingMode" />
		SetPublishingMode,

        /// <see cref="ISessionServer.Publish" />
		Publish,

        /// <see cref="ISessionServer.Republish" />
		Republish,

        /// <see cref="ISessionServer.TransferSubscriptions" />
		TransferSubscriptions,

        /// <see cref="ISessionServer.DeleteSubscriptions" />
		DeleteSubscriptions,

        /// <see cref="ISessionServer.AddNodes" />
		AddNodes,

        /// <see cref="ISessionServer.AddReferences" />
		AddReferences,

        /// <see cref="ISessionServer.DeleteNodes" />
		DeleteNodes,

        /// <see cref="ISessionServer.DeleteReferences" />
		DeleteReferences,

        /// <see cref="ISessionServer.Browse" />
		Browse,

        /// <see cref="ISessionServer.BrowseNext" />
		BrowseNext,

        /// <see cref="ISessionServer.TranslateBrowsePathsToNodeIds" />
		TranslateBrowsePathsToNodeIds,

        /// <see cref="ISessionServer.QueryFirst" />
		QueryFirst,

        /// <see cref="ISessionServer.QueryNext" />
		QueryNext,

        /// <see cref="ISessionServer.RegisterNodes" />
		RegisterNodes,

        /// <see cref="ISessionServer.UnregisterNodes" />
		UnregisterNodes
    }
}
