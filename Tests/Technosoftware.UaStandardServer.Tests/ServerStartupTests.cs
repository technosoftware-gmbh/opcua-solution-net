#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System.Threading.Tasks;
using NUnit.Framework;

using Opc.Ua;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    /// <summary>
    /// Test Standard Server stratup.
    /// </summary>
    [TestFixture, Category("Server")]
    [SetCulture("en-us"), SetUICulture("en-us")]
    [Parallelizable]
    public class ServerStartupTests
    {
        const double MaxAge = 10000;
        const uint TimeoutHint = 10000;

        [DatapointSource]
        public string[] UriSchemes = { Utils.UriSchemeOpcTcp, Utils.UriSchemeHttps };

        #region Test Methods
        /// <summary>
        /// Start a server fixture with different uri schemes.
        /// </summary>
        [Theory]
        public async Task StartServerAsync(
            string uriScheme
            )
        {
            var fixture = new ServerFixture<UaStandardServer>();
            Assert.NotNull(fixture);
            fixture.UriScheme = uriScheme;
            var server = await fixture.StartAsync(TestContext.Out).ConfigureAwait(false);
            fixture.SetTraceOutput(TestContext.Out);
            Assert.NotNull(server);
            await Task.Delay(1000).ConfigureAwait(false);
            await fixture.StopAsync().ConfigureAwait(false);
        }
        #endregion
    }
}
