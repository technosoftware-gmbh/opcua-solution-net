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
using System.Threading.Tasks;
using System.Xml;

using Opc.Ua;
#endregion

namespace Technosoftware.UaConfiguration
{
    /// <summary>
    /// ApplicationConfiguration Manager.
    /// </summary>
    public interface IUaApplicationConfigurationManager :
        IUaApplicationConfiguration,
        IUaApplicationConfigurationTransportQuotas,
        IUaApplicationConfigurationTransportQuotasSet,
        IUaApplicationConfigurationServerSelected,
        IUaApplicationConfigurationServerOptions,
        IUaApplicationConfigurationClientSelected,
        IUaApplicationConfigurationSecurity,
        IUaApplicationConfigurationSecurityOptions,
        IUaApplicationConfigurationSecurityOptionStores,
        IUaApplicationConfigurationServerPolicies,
        IUaApplicationConfigurationCreate
    {
    };
}

