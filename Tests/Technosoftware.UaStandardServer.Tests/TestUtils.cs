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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Opc.Ua.Security.Certificates;

using Opc.Ua;
#endregion

namespace Technosoftware.UaStandardServer.Tests
{
    #region Asset Helpers
    /// <summary>
    /// The interface to initialize an asset.
    /// </summary>
    public interface IAsset
    {
        void Initialize(byte[] blob, string path);
    }

    /// <summary>
    /// Create a collection of test assets.
    /// </summary>
    public class AssetCollection<T> : List<T> where T : IAsset, new()
    {
        public AssetCollection() { }
        public AssetCollection(IEnumerable<T> collection) : base(collection) { }
        public AssetCollection(int capacity) : base(capacity) { }
        public static AssetCollection<T> ToAssetCollection(T[] values)
        {
            return values != null ? new AssetCollection<T>(values) : new AssetCollection<T>();
        }

        public AssetCollection(IEnumerable<string> filelist) : base()
        {
            foreach (var file in filelist)
            {
                Add(file);
            }
        }

        public void Add(string path)
        {
            byte[] blob = File.ReadAllBytes(path);
            T asset = new T();
            asset.Initialize(blob, path);
            Add(asset);
        }
    }
    #endregion

    #region TestUtils
    /// <summary>
    /// Test helpers.
    /// </summary>
    public static class TestUtils
    {
        public static string[] EnumerateTestAssets(string searchPattern)
        {
            var assetsPath = Utils.GetAbsoluteDirectoryPath("Assets", true, false, false);
            if (assetsPath != null)
            {
                return Directory.EnumerateFiles(assetsPath, searchPattern).ToArray();
            }
            return Array.Empty<string>();
        }

        public static void ValidateSelSignedBasicConstraints(X509Certificate2 certificate)
        {
            var basicConstraintsExtension = X509Extensions.FindExtension<X509BasicConstraintsExtension>(certificate.Extensions);
            Assert.NotNull(basicConstraintsExtension);
            Assert.False(basicConstraintsExtension.CertificateAuthority);
            Assert.True(basicConstraintsExtension.Critical);
            Assert.False(basicConstraintsExtension.HasPathLengthConstraint);
        }
    }
    #endregion
}
