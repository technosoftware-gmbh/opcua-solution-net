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
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Opc.Ua;
using Opc.Ua.Export;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    /// <summary>
    /// Tests for the UANodeSet helper.
    /// </summary>
    [TestFixture, Category("UANodeSet")]
    [SetCulture("en-us"), SetUICulture("en-us")]
    [Parallelizable]
    public class UANodeSetHelpersTests
    {
        #region Test Setup
        [OneTimeSetUp]
        protected void OneTimeSetUp()
        {
        }

        [OneTimeTearDown]
        protected void OneTimeTearDown()
        {
        }

        [SetUp]
        protected void SetUp()
        {

        }

        [TearDown]
        protected void TearDown()
        {
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Test NodeSet2 import.
        /// </summary>
        [Test]
        [TestCase("reference/SampleCompany/NodeManagers/TestData/SampleCompany.NodeManagers.TestData.NodeSet2.xml")]
        [TestCase("reference/SampleCompany/NodeManagers/MemoryBuffer/SampleCompany.NodeManagers.MemoryBuffer.NodeSet2.xml")]
        public void NodeSet2ValidationTest(string nodeset2File)
        {
            var assetPath = Utils.GetAbsoluteFilePath("../../../../../" + nodeset2File, true, false, false);
            using (var importStream = new FileStream(assetPath, FileMode.Open))
            {
                var importedNodeSet = UANodeSet.Read(importStream);
                Assert.NotNull(importedNodeSet);

                var importedNodeStates = new NodeStateCollection();
                var localContext = new SystemContext();
                localContext.NamespaceUris = new NamespaceTable();
                if (importedNodeSet.NamespaceUris != null)
                {
                    foreach (var namespaceUri in importedNodeSet.NamespaceUris)
                    {
                        localContext.NamespaceUris.Append(namespaceUri);
                    }
                }
                importedNodeSet.Import(localContext, importedNodeStates);
            }
        }

        /// <summary>
        /// Test NodeSet2 import. Requires test assets to be in the 'Assets' folder.
        /// </summary>
        [Theory]
        public void NodeSet2ValidationTest(NodeSet2Asset nodeset2Asset)
        {
            using (var importStream = new MemoryStream(nodeset2Asset.Xml))
            {
                var importedNodeSet = Opc.Ua.Export.UANodeSet.Read(importStream);
                Assert.NotNull(importedNodeSet);

                var importedNodeStates = new NodeStateCollection();
                var localContext = new SystemContext();
                localContext.NamespaceUris = new NamespaceTable();
                if (importedNodeSet.NamespaceUris != null)
                {
                    foreach (var namespaceUri in importedNodeSet.NamespaceUris)
                    {
                        localContext.NamespaceUris.Append(namespaceUri);
                    }
                }
                importedNodeSet.Import(localContext, importedNodeStates);
            }
        }
    }
    #endregion

    #region Asset helpers
     /// <summary>
    /// A NodeSet2 as test asset.
    /// </summary>
    public class NodeSet2Asset : IAsset, IFormattable
    {
        public NodeSet2Asset() { }

        public string Path { get; private set; }
        public byte[] Xml { get; private set; }

        public void Initialize(byte[] blob, string path)
        {
            Path = path;
            Xml = blob;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var file = System.IO.Path.GetFileName(Path);
            return $"{file}";
        }
    }
    #endregion
}
