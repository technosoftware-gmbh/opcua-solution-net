#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using Opc.Ua;

using SampleCompany.NodeManagers.Reference;
#endregion

namespace Technosoftware.UaClient.Tests
{
    public class TokenValidatorMock : ITokenValidator
    {
        public IssuedIdentityToken LastIssuedToken { get; set; }
            
        public IUserIdentity ValidateToken(IssuedIdentityToken issuedToken)
        {
            this.LastIssuedToken = issuedToken;

            return new UserIdentity(issuedToken);
        }
    }
}
