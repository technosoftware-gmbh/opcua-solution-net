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
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Technosoftware.UaClient.Tests
{
    /// <summary>
    /// Object that creates instances of an Opc.Ua.Client.Session object.
    /// </summary>
    public class HeaderUpdatingSessionFactory : TraceableSessionFactory
    {
        /// <summary>
        /// The default instance of the factory.
        /// </summary>
        public new static readonly HeaderUpdatingSessionFactory Instance = new HeaderUpdatingSessionFactory();

        /// <summary>
        /// Force use of the default instance.
        /// </summary>
        protected HeaderUpdatingSessionFactory()
        {
        }        
    }
}
