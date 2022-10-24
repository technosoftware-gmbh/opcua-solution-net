#region Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
#endregion

static class Program
{
    // Main Method 
    public static void Main(String[] args)
    {
        IConfig config = ManualConfig.Create(DefaultConfig.Instance)
            // need this option because of reference to nunit.framework
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            ;
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}

