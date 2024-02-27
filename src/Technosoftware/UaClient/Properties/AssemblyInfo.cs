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
using System.Runtime.CompilerServices;
#endregion

#if SIGNASSEMBLY
[assembly: InternalsVisibleTo("Technosoftware.UaClient.Tests, PublicKey = " +
    // Technosoftware GmbH Strong Name Public Key
    "0024000004800000940000000602000000240000525341310004000001000100c183fdb506d6e7" +
    "3b26cb69769655178d84d104e7dfd49167d338c4938db3e75d12e8e68929ab7ec2cccc74e33b11" +
    "b81d8fac6625c56cdf795220c7988460fb5bc721e8031fd148d3bde09d05a421fd44bd9c849c7f" +
    "c263395bba01dd28feb37a7c345e74b6ab5cfc8fd6c1eb8612ebc4bc22ae89c5d27bc69c96b2bf" +
    "c0b95ab1")]
#else
[assembly: InternalsVisibleTo("Technosoftware.UaClient.Tests")]
#endif
