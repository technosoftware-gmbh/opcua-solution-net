#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using Technosoftware.UaClient;
#endregion

namespace SampleCompany.ReferenceClient
{
    /// <summary>
    /// A client interface which holds an active session.
    /// The client handler may reconnect and the Session
    /// property may be updated during operation.
    /// </summary>
    public interface IMyUaClient
    {
        /// <summary>
        /// The session to use.
        /// </summary>
        IUaSession Session { get; }
    };
}
