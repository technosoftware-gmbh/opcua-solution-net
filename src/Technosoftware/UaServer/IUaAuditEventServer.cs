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
    /// An interface to report audit events in the server.
    /// </summary>
    public interface IAuditEventServer
    {
        /// <summary>
        /// If auditing is enabled.
        /// </summary>
        bool Auditing { get; }

        /// <summary>
        /// The default system context for the audit events.
        /// </summary>
        ISystemContext DefaultAuditContext { get; }

        /// <summary>
        /// Called by any component to report an audit event.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="e">The event.</param>
        void ReportAuditEvent(ISystemContext context, AuditEventState e);
    }
}
