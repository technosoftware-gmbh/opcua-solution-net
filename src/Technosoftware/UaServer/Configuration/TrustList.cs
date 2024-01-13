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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Security.Certificates;

using Technosoftware.UaServer.Diagnostics;
#endregion

namespace Technosoftware.UaServer.Configuration
{
    /// <summary>
    /// Contains the configuration about a trust list.
    /// </summary>
    public class TrustList
    {
        const int DefaultTrustListCapacity = 0x10000;

        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Create a new trust list with default values.
        /// </summary>
        /// <param name="node">The node for the state of the trust list.</param>
        /// <param name="trustedListPath">The path for the trust list.</param>
        /// <param name="issuerListPath">The path for the issuer list.</param>
        /// <param name="readAccess">The read access requirements.</param>
        /// <param name="writeAccess">The write access requirements</param>
        public TrustList(
            TrustListState node,
            string trustedListPath,
            string issuerListPath,
            SecureAccess readAccess,
            SecureAccess writeAccess)
        {
            node_ = node;
            trustedStorePath_ = trustedListPath;
            issuerStorePath_ = issuerListPath;
            readAccess_ = readAccess;
            writeAccess_ = writeAccess;

            node.Open.OnCall = Open;
            node.OpenWithMasks.OnCall = OpenWithMasks;
            node.Read.OnCall = Read;
            node.Write.OnCall = Write;
            node.Close.OnCall = Close;
            node.CloseAndUpdate.OnCall = CloseAndUpdate;
            node.AddCertificate.OnCall = AddCertificate;
            node.RemoveCertificate.OnCall = RemoveCertificate;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Delegate to validate the access to the trust list.
        /// </summary>
        /// <param name="context">An interface to an object that describes how access the system containing the data.</param>
        public delegate void SecureAccess(ISystemContext context);
        #endregion

        #region Private Methods
        private ServiceResult Open(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            byte mode,
            ref uint fileHandle)
        {
            return Open(context, method, objectId, (OpenFileMode)mode, TrustListMasks.All, ref fileHandle);
        }

        private ServiceResult OpenWithMasks(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint masks,
            ref uint fileHandle)
        {
            return Open(context, method, objectId, OpenFileMode.Read, (TrustListMasks)masks, ref fileHandle);
        }

        private ServiceResult Open(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            OpenFileMode mode,
            TrustListMasks masks,
            ref uint fileHandle)
        {
            HasSecureReadAccess(context);

            if (mode == OpenFileMode.Read)
            {
                HasSecureReadAccess(context);
            }
            else if (mode == (OpenFileMode.Write | OpenFileMode.EraseExisting))
            {
                HasSecureWriteAccess(context);
            }
            else
            {
                return StatusCodes.BadNotWritable;
            }

            lock (lock_)
            {
                if (sessionId_ != null)
                {
                    // to avoid deadlocks, last open always wins
                    sessionId_ = null;
                    stream_ = null;
                    node_.OpenCount.Value = 0;
                }

                readMode_ = mode == OpenFileMode.Read;
                sessionId_ = context.SessionId;
                fileHandle = ++fileHandle_;

                var trustList = new TrustListDataType() {
                    SpecifiedLists = (uint)masks
                };

                using (var store = CertificateStoreIdentifier.OpenStore(trustedStorePath_))
                {
                    if ((masks & TrustListMasks.TrustedCertificates) != 0)
                    {
                        X509Certificate2Collection certificates = store.Enumerate().GetAwaiter().GetResult();
                        foreach (var certificate in certificates)
                        {
                            trustList.TrustedCertificates.Add(certificate.RawData);
                        }
                    }

                    if ((masks & TrustListMasks.TrustedCrls) != 0)
                    {
                        foreach (var crl in store.EnumerateCRLs().GetAwaiter().GetResult())
                        {
                            trustList.TrustedCrls.Add(crl.RawData);
                        }
                    }
                }

                using (var store = CertificateStoreIdentifier.OpenStore(issuerStorePath_))
                {
                    if ((masks & TrustListMasks.IssuerCertificates) != 0)
                    {
                        X509Certificate2Collection certificates = store.Enumerate().GetAwaiter().GetResult();
                        foreach (var certificate in certificates)
                        {
                            trustList.IssuerCertificates.Add(certificate.RawData);
                        }
                    }

                    if ((masks & TrustListMasks.IssuerCrls) != 0)
                    {
                        foreach (var crl in store.EnumerateCRLs().GetAwaiter().GetResult())
                        {
                            trustList.IssuerCrls.Add(crl.RawData);
                        }
                    }
                }

                if (readMode_)
                {
                    stream_ = EncodeTrustListData(context, trustList);
                }
                else
                {
                    stream_ = new MemoryStream(DefaultTrustListCapacity);
                }

                node_.OpenCount.Value = 1;
            }

            return ServiceResult.Good;
        }

        private ServiceResult Read(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint fileHandle,
            int length,
            ref byte[] data)
        {
            HasSecureReadAccess(context);

            lock (lock_)
            {
                if (sessionId_ != context.SessionId)
                {
                    return StatusCodes.BadUserAccessDenied;
                }

                if (fileHandle_ != fileHandle)
                {
                    return StatusCodes.BadInvalidArgument;
                }

                data = new byte[length];

                var bytesRead = stream_.Read(data, 0, length);

                if (bytesRead < 0)
                {
                    return StatusCodes.BadUnexpectedError;
                }

                if (bytesRead < length)
                {
                    var bytes = new byte[bytesRead];
                    Array.Copy(data, bytes, bytesRead);
                    data = bytes;
                }
            }

            return ServiceResult.Good;
        }

        private ServiceResult Write(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint fileHandle,
            byte[] data)
        {
            HasSecureWriteAccess(context);

            lock (lock_)
            {
                if (sessionId_ != context.SessionId)
                {
                    return StatusCodes.BadUserAccessDenied;
                }

                if (fileHandle_ != fileHandle)
                {
                    return StatusCodes.BadInvalidArgument;
                }

                stream_.Write(data, 0, data.Length);

            }

            return ServiceResult.Good;
        }


        private ServiceResult Close(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint fileHandle)
        {
            HasSecureReadAccess(context);

            lock (lock_)
            {
                if (sessionId_ != context.SessionId)
                {
                    return StatusCodes.BadUserAccessDenied;
                }

                if (fileHandle_ != fileHandle)
                {
                    return StatusCodes.BadInvalidArgument;
                }

                sessionId_ = null;
                stream_ = null;
                node_.OpenCount.Value = 0;
            }

            return ServiceResult.Good;
        }

        private ServiceResult CloseAndUpdate(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            uint fileHandle,
            ref bool restartRequired)
        {
            HasSecureWriteAccess(context);

            ServiceResult result = StatusCodes.Good;

            lock (lock_)
            {
                if (sessionId_ != context.SessionId)
                {
                    return StatusCodes.BadUserAccessDenied;
                }

                if (fileHandle_ != fileHandle)
                {
                    return StatusCodes.BadInvalidArgument;
                }

                try
                {

                    var trustList = DecodeTrustListData(context, stream_);
                    var masks = (TrustListMasks)trustList.SpecifiedLists;

                    X509Certificate2Collection issuerCertificates = null;
                    X509CRLCollection issuerCrls = null;
                    X509Certificate2Collection trustedCertificates = null;
                    X509CRLCollection trustedCrls = null;

                    // test integrity of all CRLs
                    if ((masks & TrustListMasks.IssuerCertificates) != 0)
                    {
                        issuerCertificates = new X509Certificate2Collection();
                        foreach (var cert in trustList.IssuerCertificates)
                        {
                            issuerCertificates.Add(new X509Certificate2(cert));
                        }
                    }
                    if ((masks & TrustListMasks.IssuerCrls) != 0)
                    {
                        issuerCrls = new X509CRLCollection();
                        foreach (var crl in trustList.IssuerCrls)
                        {
                            issuerCrls.Add(new X509CRL(crl));
                        }
                    }
                    if ((masks & TrustListMasks.TrustedCertificates) != 0)
                    {
                        trustedCertificates = new X509Certificate2Collection();
                        foreach (var cert in trustList.TrustedCertificates)
                        {
                            trustedCertificates.Add(new X509Certificate2(cert));
                        }
                    }
                    if ((masks & TrustListMasks.TrustedCrls) != 0)
                    {
                        trustedCrls = new X509CRLCollection();
                        foreach (var crl in trustList.TrustedCrls)
                        {
                            trustedCrls.Add(new X509CRL(crl));
                        }
                    }

                    // update store
                    // test integrity of all CRLs
                    var updateMasks = TrustListMasks.None;
                    if ((masks & TrustListMasks.IssuerCertificates) != 0)
                    {
                        if (UpdateStoreCertificatesAsync(issuerStorePath_, issuerCertificates).GetAwaiter().GetResult())
                        {
                            updateMasks |= TrustListMasks.IssuerCertificates;
                        }
                    }
                    if ((masks & TrustListMasks.IssuerCrls) != 0)
                    {
                        if (UpdateStoreCrlsAsync(issuerStorePath_, issuerCrls).GetAwaiter().GetResult())
                        {
                            updateMasks |= TrustListMasks.IssuerCrls;
                        }
                    }
                    if ((masks & TrustListMasks.TrustedCertificates) != 0)
                    {
                        if (UpdateStoreCertificatesAsync(trustedStorePath_, trustedCertificates).GetAwaiter().GetResult())
                        {
                            updateMasks |= TrustListMasks.TrustedCertificates;
                        }
                    }
                    if ((masks & TrustListMasks.TrustedCrls) != 0)
                    {
                        if (UpdateStoreCrlsAsync(trustedStorePath_, trustedCrls).GetAwaiter().GetResult())
                        {
                            updateMasks |= TrustListMasks.TrustedCrls;
                        }
                    }

                    if (masks != updateMasks)
                    {
                        result = StatusCodes.BadCertificateInvalid;
                    }
                }
                catch
                {
                    result = StatusCodes.BadCertificateInvalid;
                }
                finally
                {
                    sessionId_ = null;
                    stream_ = null;
                    node_.LastUpdateTime.Value = DateTime.UtcNow;
                    node_.OpenCount.Value = 0;
                }
            }

            restartRequired = false;

            // report the TrustListUpdatedAuditEvent
            object[] inputParameters = new object[] { fileHandle };
            node_.ReportTrustListUpdatedAuditEvent(context, objectId, "Method/CloseAndUpdate", method.NodeId, inputParameters, result.StatusCode);
            return result;
        }

        private ServiceResult AddCertificate(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            byte[] certificate,
            bool isTrustedCertificate)
        {
            HasSecureWriteAccess(context);

            ServiceResult result = StatusCodes.Good;
            lock (lock_)
            {

                if (sessionId_ != null)
                {
                    result = StatusCodes.BadInvalidState;
                }
                else if (certificate == null)
                {
                    result = StatusCodes.BadInvalidArgument;
                }
                else
                {
                    X509Certificate2 cert = null;
                    try
                    {
                        cert = new X509Certificate2(certificate);
                    }
                    catch
                    {
                        // note: a previous version of the sample code accepted also CRL,
                        // but the behaviour was not as specified and removed
                        // https://mantis.opcfoundation.org/view.php?id=6342
                        result = StatusCodes.BadCertificateInvalid;
                    }

                    using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(isTrustedCertificate ? trustedStorePath_ : issuerStorePath_))
                    {
                        if (cert != null)
                        {
                            store.Add(cert).GetAwaiter().GetResult();
                        }
                    }

                    node_.LastUpdateTime.Value = DateTime.UtcNow;
                }
            }

            // report the TrustListUpdatedAuditEvent
            object[] inputParameters = new object[] { certificate, isTrustedCertificate };
            node_.ReportTrustListUpdatedAuditEvent(context, objectId, "Method/AddCertificate", method.NodeId, inputParameters, result.StatusCode);

            return result;

        }

        private ServiceResult RemoveCertificate(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            string thumbprint,
            bool isTrustedCertificate)
        {
            HasSecureWriteAccess(context);
            ServiceResult result = StatusCodes.Good;
            lock (lock_)
            {

                if (sessionId_ != null)
                {
                    result =  StatusCodes.BadInvalidState;
                }
                else if (String.IsNullOrEmpty(thumbprint))
                {
                    result =  StatusCodes.BadInvalidArgument;
                }
                else
                {
                    using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(isTrustedCertificate ? trustedStorePath_ : issuerStorePath_))
                    {
                        var certCollection = store.FindByThumbprint(thumbprint).GetAwaiter().GetResult();

                        if (certCollection.Count == 0)
                        {
                            result = StatusCodes.BadInvalidArgument;
                        }
                        else
                        {
                            // delete all CRLs signed by cert
                            var crlsToDelete = new X509CRLCollection();
                            foreach (var crl in store.EnumerateCRLs().GetAwaiter().GetResult())
                            {
                                foreach (var cert in certCollection)
                                {
                                    if (X509Utils.CompareDistinguishedName(cert.SubjectName, crl.IssuerName) &&
                                        crl.VerifySignature(cert, false))
                                    {
                                        crlsToDelete.Add(crl);
                                        break;
                                    }
                                }
                            }

                            if (!store.Delete(thumbprint).GetAwaiter().GetResult())
                            {
                                result = StatusCodes.BadInvalidArgument;
                            }
                            else
                            {
                                foreach (var crl in crlsToDelete)
                                {
                                    if (!store.DeleteCRL(crl).GetAwaiter().GetResult())
                                    {
                                        // intentionally ignore errors, try best effort
                                        Utils.LogError("RemoveCertificate: Failed to delete CRL {0}.", crl.ToString());
                                    }
                                }
                            }
                        }
                    }

                    node_.LastUpdateTime.Value = DateTime.UtcNow;
                }
            }

            // report the TrustListUpdatedAuditEvent
            object[] inputParameters = new object[] { thumbprint };
            node_.ReportTrustListUpdatedAuditEvent(context, objectId, "Method/RemoveCertificate", method.NodeId, inputParameters, result.StatusCode);
            return result;
        }

        private Stream EncodeTrustListData(
            ISystemContext context,
            TrustListDataType trustList
            )
        {
            var messageContext = new ServiceMessageContext() {
                NamespaceUris = context.NamespaceUris,
                ServerUris = context.ServerUris,
                Factory = context.EncodeableFactory
            };
            var strm = new MemoryStream();
            using (BinaryEncoder encoder = new BinaryEncoder(strm, messageContext, true))
            {
                encoder.WriteEncodeable(null, trustList, null);
            }
            strm.Position = 0;
            return strm;
        }

        private TrustListDataType DecodeTrustListData(
            ISystemContext context,
            Stream strm)
        {
            var trustList = new TrustListDataType();
            var messageContext = new ServiceMessageContext() {
                NamespaceUris = context.NamespaceUris,
                ServerUris = context.ServerUris,
                Factory = context.EncodeableFactory
            };
            strm.Position = 0;
            var decoder = new BinaryDecoder(strm, messageContext);
            trustList.Decode(decoder);
            decoder.Close();
            return trustList;
        }

        private async Task<bool> UpdateStoreCrlsAsync(
            string storePath,
            X509CRLCollection updatedCrls)
        {
            var result = true;
            try
            {
                using (var store = CertificateStoreIdentifier.OpenStore(storePath))
                {
                    var storeCrls = await store.EnumerateCRLs().ConfigureAwait(false);
                    foreach (var crl in storeCrls)
                    {
                        if (!updatedCrls.Contains(crl))
                        {
                            if (!await store.DeleteCRL(crl).ConfigureAwait(false))
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            updatedCrls.Remove(crl);
                        }
                    }
                    foreach (var crl in updatedCrls)
                    {
                        await store.AddCRL(crl).ConfigureAwait(false);
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private async Task<bool> UpdateStoreCertificatesAsync(
            string storePath,
            X509Certificate2Collection updatedCerts)
        {
            var result = true;
            try
            {
                using (var store = CertificateStoreIdentifier.OpenStore(storePath))
                {
                    var storeCerts = await store.Enumerate().ConfigureAwait(false);
                    foreach (var cert in storeCerts)
                    {
                        if (!updatedCerts.Contains(cert))
                        {
                            if (!await store.Delete(cert.Thumbprint).ConfigureAwait(false))
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            updatedCerts.Remove(cert);
                        }
                    }
                    foreach (var cert in updatedCerts)
                    {
                        await store.Add(cert).ConfigureAwait(false);
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void HasSecureReadAccess(ISystemContext context)
        {
            if (readAccess_ != null)
            {
                readAccess_.Invoke(context);
            }
            else
            {
                throw new ServiceResultException(StatusCodes.BadUserAccessDenied);
            }
        }

        private void HasSecureWriteAccess(ISystemContext context)
        {
            if (writeAccess_ != null)
            {
                writeAccess_.Invoke(context);
            }
            else
            {
                throw new ServiceResultException(StatusCodes.BadUserAccessDenied);
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private SecureAccess readAccess_;
        private SecureAccess writeAccess_;
        private NodeId sessionId_;
        private uint fileHandle_;
        private readonly string trustedStorePath_;
        private readonly string issuerStorePath_;
        private TrustListState node_;
        private Stream stream_;
        private bool readMode_;
        #endregion

    }

}
