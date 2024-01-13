#region Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.Reference
{
    public static class VariableExtensions
    {
        /// <summary>
        /// Set the minimum sampling interval supported by the variable.
        /// </summary>
        /// <param name="variable">The variable to set the MinimumSamplingInterval.</param>
        /// <param name="minimumSamplingInterval">The minimum sampling interval.</param>
        /// <returns></returns>
        public static BaseDataVariableState MinimumSamplingInterval(this BaseDataVariableState variable, int minimumSamplingInterval)
        {
            variable.MinimumSamplingInterval = minimumSamplingInterval;
            return variable;
        }
    }
}