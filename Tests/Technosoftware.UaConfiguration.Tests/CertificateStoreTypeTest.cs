#region Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using NUnit.Framework;

using Opc.Ua;
using Opc.Ua.Security.Certificates;
#endregion

namespace Technosoftware.UaConfiguration
{
    /// <summary>
    /// Tests for the custom certificate store config extensions.
    /// </summary>
    [TestFixture, Category("CertificateStore")]
    [SetCulture("en-us")]
    public class CertificateStoreTypeTest
    {
        [OneTimeSetUp]
        protected void OneTimeSetUp()
        {
            CertificateStoreType.RegisterCertificateStoreType(TestCertStore.StoreTypePrefix, new TestStoreType());
        }

        [SetUp]
        public void SetUp()
        {
            tempPath_ = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath_);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(tempPath_, true);
        }

        #region Test Methods
        [Test]
        public async Task CertifcateStoreTypeNoConfigTest()
        {
            ApplicationInstance application = new ApplicationInstance() {
                ApplicationName = "Application",
            };

            string appStorePath = tempPath_ + Path.DirectorySeparatorChar + "own";
            string trustedStorePath = tempPath_ + Path.DirectorySeparatorChar + "trusted";
            string issuerStorePath = tempPath_ + Path.DirectorySeparatorChar + "issuer";
            string trustedUserStorePath = tempPath_ + Path.DirectorySeparatorChar + "trustedUser";
            string issuerUserStorePath = tempPath_ + Path.DirectorySeparatorChar + "userIssuer";

            var appConfigBuilder = application.CreateApplicationConfigurationManager(
                applicationUri: "urn:localhost:CertStoreTypeTest",
                productUri: "uri:opcfoundation.org:Tests:CertStoreTypeTest")
                .AsClient()
                .AddSecurityConfigurationStores(
                    subjectName: "CN=CertStoreTypeTest, O=OPC Foundation",
                    appRoot: TestCertStore.StoreTypePrefix + appStorePath,
                    trustedRoot: TestCertStore.StoreTypePrefix + trustedStorePath,
                    issuerRoot: TestCertStore.StoreTypePrefix + issuerStorePath
                    )
                .AddSecurityConfigurationUserStore(
                    trustedRoot: TestCertStore.StoreTypePrefix + trustedUserStorePath,
                    issuerRoot: TestCertStore.StoreTypePrefix + issuerUserStorePath
                );

            // patch custom stores before creating the config
            ApplicationConfiguration appConfig = await appConfigBuilder.CreateAsync().ConfigureAwait(false);

            bool certOK = await application.CheckApplicationInstanceCertificateAsync(true, 0).ConfigureAwait(false);
            Assert.True(certOK);

            int instancesCreatedWhileLoadingConfig = TestCertStore.InstancesCreated;
            Assert.IsTrue(instancesCreatedWhileLoadingConfig > 0);

            OpenCertStore(appConfig.SecurityConfiguration.TrustedIssuerCertificates);
            OpenCertStore(appConfig.SecurityConfiguration.TrustedPeerCertificates);
            OpenCertStore(appConfig.SecurityConfiguration.UserIssuerCertificates);
            OpenCertStore(appConfig.SecurityConfiguration.TrustedUserCertificates);

            int instancesCreatedWhileOpeningAuthRootStore = TestCertStore.InstancesCreated;
            Assert.IsTrue(instancesCreatedWhileLoadingConfig < instancesCreatedWhileOpeningAuthRootStore);
            CertificateStoreIdentifier.OpenStore(TestCertStore.StoreTypePrefix + trustedUserStorePath);
            Assert.IsTrue(instancesCreatedWhileOpeningAuthRootStore < TestCertStore.InstancesCreated);
        }
        #endregion Test Methods

        #region Private Methods
        private void OpenCertStore(CertificateTrustList trustList)
        {
            using (ICertificateStore trustListStore = trustList.OpenStore())
            {
                var certs = trustListStore.Enumerate();
                var crls = trustListStore.EnumerateCRLs();
                trustListStore.Close();
            }
        }
        #endregion

        #region Private Members
        private string tempPath_;
        #endregion
    }

    internal sealed class TestStoreType : ICertificateStoreType
    {
        public ICertificateStore CreateStore()
        {
            return new TestCertStore();
        }

        public bool SupportsStorePath(string storePath)
        {
            return storePath != null && storePath.StartsWith(TestCertStore.StoreTypePrefix, StringComparison.Ordinal);
        }
    }

    internal sealed class TestCertStore : ICertificateStore
    {
        public const string StoreTypePrefix = "testStoreType:";

        public TestCertStore()
        {
            instancesCreated_++;
            innerStore_ = new DirectoryCertificateStore(true);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            innerStore_.Dispose();
        }

        /// <inheritdoc/>
        public void Open(string location, bool noPrivateKeys)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            if (!location.StartsWith(StoreTypePrefix, StringComparison.Ordinal))
            {
                throw new ArgumentException($"Expected argument {nameof(location)} starting with {StoreTypePrefix}");
            }
            innerStore_.Open(location.Substring(StoreTypePrefix.Length), noPrivateKeys);
        }

        /// <inheritdoc/>
        public void Close()
        {
            innerStore_.Close();
        }

        /// <inheritdoc/>
        public string StoreType => StoreTypePrefix.Substring(0, StoreTypePrefix.Length - 1);

        /// <inheritdoc/>
        public string StorePath => innerStore_.StorePath;

        /// <inheritdoc/>
        public Task Add(X509Certificate2 certificate, string password = null)
        {
            return innerStore_.Add(certificate, password);
        }

        /// <inheritdoc/>
        public Task<bool> Delete(string thumbprint)
        {
            return innerStore_.Delete(thumbprint);
        }

        /// <inheritdoc/>
        public Task<X509Certificate2Collection> Enumerate()
        {
            return innerStore_.Enumerate();
        }

        /// <inheritdoc/>
        public Task<X509Certificate2Collection> FindByThumbprint(string thumbprint)
        {
            return innerStore_.FindByThumbprint(thumbprint);
        }

        /// <inheritdoc/>
        public bool SupportsCRLs
            => innerStore_.SupportsCRLs;

        /// <inheritdoc/>
        public Task AddCRL(X509CRL crl)
            => innerStore_.AddCRL(crl);

        /// <inheritdoc/>
        public Task<bool> DeleteCRL(X509CRL crl)
            => innerStore_.DeleteCRL(crl);

        /// <inheritdoc/>
        public Task<X509CRLCollection> EnumerateCRLs()
            => innerStore_.EnumerateCRLs();

        /// <inheritdoc/>
        public Task<X509CRLCollection> EnumerateCRLs(X509Certificate2 issuer, bool validateUpdateTime = true)
            => innerStore_.EnumerateCRLs(issuer, validateUpdateTime);

        /// <inheritdoc/>
        public Task<StatusCode> IsRevoked(X509Certificate2 issuer, X509Certificate2 certificate)
            => innerStore_.IsRevoked(issuer, certificate);

        /// <inheritdoc/>
        public bool SupportsLoadPrivateKey => innerStore_.SupportsLoadPrivateKey;

        /// <inheritdoc/>
        public Task<X509Certificate2> LoadPrivateKey(string thumbprint, string subjectName, string password)
            => innerStore_.LoadPrivateKey(thumbprint, subjectName, password);

        public static int InstancesCreated => instancesCreated_;

        #region Private Members
        private static int instancesCreated_ = 0;
        private readonly DirectoryCertificateStore innerStore_;
        #endregion 
    }
}
