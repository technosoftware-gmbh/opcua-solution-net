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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Opc.Ua;
using Opc.Ua.Bindings;

using Technosoftware.UaServer.Aggregates;
using Technosoftware.UaServer.Diagnostics;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaServer.Sessions;
using Technosoftware.UaServer.Subscriptions;
using Technosoftware.UaServer.Server;
#endregion

namespace Technosoftware.UaServer
{
    /// <summary>
    /// The standard implementation of a UA server.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public partial class UaGenericServer : SessionServerBase
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the object with default values.
        /// </summary>
        public UaGenericServer()
        {
            nodeManagerFactories_ = new List<IUaNodeManagerFactory>();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // halt any outstanding timer.
                if (registrationTimer_ != null)
                {
                    Utils.SilentDispose(registrationTimer_);
                    registrationTimer_ = null;
                }

                // close the watcher.
                if (configurationWatcher_ != null)
                {
                    Utils.SilentDispose(configurationWatcher_);
                    configurationWatcher_ = null;
                }

                // close the server.
                if (genericServerData_ != null)
                {
                    Utils.SilentDispose(genericServerData_);
                    genericServerData_ = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region IServer Methods
        /// <summary>
        /// Invokes the FindServers service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <param name="localeIds">The locale ids.</param>
        /// <param name="serverUris">The server uris.</param>
        /// <param name="servers">List of Servers that meet criteria specified in the request.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader FindServers(
            RequestHeader requestHeader,
            string endpointUrl,
            StringCollection localeIds,
            StringCollection serverUris,
            out ApplicationDescriptionCollection servers)
        {
            servers = new ApplicationDescriptionCollection();

            ValidateRequest(requestHeader);

            lock (lock_)
            {
                // parse the url provided by the client.
                IList<BaseAddress> baseAddresses = BaseAddresses;

                Uri parsedEndpointUrl = Utils.ParseUri(endpointUrl);

                if (parsedEndpointUrl != null)
                {
                    baseAddresses = FilterByEndpointUrl(parsedEndpointUrl, baseAddresses);
                }

                // check if nothing to do.
                if (baseAddresses.Count == 0)
                {
                    servers = new ApplicationDescriptionCollection();
                    return CreateResponse(requestHeader, StatusCodes.Good);
                }

                // build list of unique servers.
                var uniqueServers = new Dictionary<string, ApplicationDescription>();

                foreach (EndpointDescription description in GetEndpoints())
                {
                    ApplicationDescription server = description.Server;

                    // skip servers that have been processed.
                    if (uniqueServers.ContainsKey(server.ApplicationUri))
                    {
                        continue;
                    }

                    // check client is filtering by server uri.
                    if (serverUris != null && serverUris.Count > 0)
                    {
                        if (!serverUris.Contains(server.ApplicationUri))
                        {
                            continue;
                        }
                    }

                    // localize the application name if requested.
                    LocalizedText applicationName = server.ApplicationName;

                    if (localeIds != null && localeIds.Count > 0)
                    {
                        applicationName = genericServerData_.ResourceManager.Translate(localeIds, applicationName);
                    }

                    // get the application description.
                    ApplicationDescription application = TranslateApplicationDescription(
                        parsedEndpointUrl,
                        server,
                        baseAddresses,
                        applicationName);

                    uniqueServers.Add(server.ApplicationUri, application);

                    // add to list of servers to return.
                    servers.Add(application);
                }
            }

            return CreateResponse(requestHeader, StatusCodes.Good);
        }

        /// <summary>
        /// Invokes the GetEndpoints service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <param name="localeIds">The locale ids.</param>
        /// <param name="profileUris">The profile uris.</param>
        /// <param name="endpoints">The endpoints supported by the server.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader GetEndpoints(
            RequestHeader requestHeader,
            string endpointUrl,
            StringCollection localeIds,
            StringCollection profileUris,
            out EndpointDescriptionCollection endpoints)
        {
            endpoints = null;

            ValidateRequest(requestHeader);

            lock (lock_)
            {
                // filter by profile.
                IList<BaseAddress> baseAddresses = FilterByProfile(profileUris, BaseAddresses);

                // get the descriptions.
                endpoints = GetEndpointDescriptions(
                    endpointUrl,
                    baseAddresses,
                    localeIds);
            }

            return CreateResponse(requestHeader, StatusCodes.Good);
        }

        /// <summary>
        /// Returns the endpoints that match the base address and endpoint url.
        /// </summary>
        protected EndpointDescriptionCollection GetEndpointDescriptions(
            string endpointUrl,
            IList<BaseAddress> baseAddresses,
            StringCollection localeIds)
        {
            EndpointDescriptionCollection endpoints = null;

            // parse the url provided by the client.
            Uri parsedEndpointUrl = Utils.ParseUri(endpointUrl);

            if (parsedEndpointUrl != null)
            {
                baseAddresses = FilterByEndpointUrl(parsedEndpointUrl, baseAddresses);
            }

            // check if nothing to do.
            if (baseAddresses.Count != 0)
            {
                // localize the application name if requested.
                LocalizedText applicationName = ServerDescription.ApplicationName;

                if (localeIds != null && localeIds.Count > 0)
                {
                    applicationName = genericServerData_.ResourceManager.Translate(localeIds, applicationName);
                }

                // translate the application description.
                ApplicationDescription application = TranslateApplicationDescription(
                    parsedEndpointUrl,
                    ServerDescription,
                    baseAddresses,
                    applicationName);

                // translate the endpoint descriptions.
                endpoints = TranslateEndpointDescriptions(
                    parsedEndpointUrl,
                    baseAddresses,
                    Endpoints,
                    application);
            }

            return endpoints;
        }

        #region Report Audit Events
        /// <inheritdoc/>
        public override void ReportAuditOpenSecureChannelEvent(
            string globalChannelId,
            EndpointDescription endpointDescription,
            OpenSecureChannelRequest request,
            X509Certificate2 clientCertificate,
            Exception exception)
        {
            ServerData?.ReportAuditOpenSecureChannelEvent(globalChannelId, endpointDescription, request, clientCertificate, exception);
        }

        /// <inheritdoc/>
        public override void ReportAuditCloseSecureChannelEvent(
            string globalChannelId,
            Exception exception)
        {
            ServerData?.ReportAuditCloseSecureChannelEvent(globalChannelId, exception);
        }

        /// <inheritdoc/>
        public override void ReportAuditCertificateEvent(
            X509Certificate2 clientCertificate,
            Exception exception)
        {
            ServerData?.ReportAuditCertificateEvent(clientCertificate, exception);
        }
        #endregion Report Audit Events

        /// <summary>
        /// Invokes the CreateSession service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="clientDescription">Application description for the client application.</param>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <param name="sessionName">Name for the Session assigned by the client.</param>
        /// <param name="clientNonce">The client nonce.</param>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="requestedSessionTimeout">The requested session timeout.</param>
        /// <param name="maxResponseMessageSize">Size of the max response message.</param>
        /// <param name="sessionId">The unique public identifier assigned by the Server to the Session.</param>
        /// <param name="authenticationToken">The unique private identifier assigned by the Server to the Session.</param>
        /// <param name="revisedSessionTimeout">The revised session timeout.</param>
        /// <param name="serverNonce">The server nonce.</param>
        /// <param name="serverCertificate">The server certificate.</param>
        /// <param name="serverEndpoints">The server endpoints.</param>
        /// <param name="serverSoftwareCertificates">The server software certificates.</param>
        /// <param name="serverSignature">The server signature.</param>
        /// <param name="maxRequestMessageSize">Size of the max request message.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader CreateSession(
            RequestHeader requestHeader,
            ApplicationDescription clientDescription,
            string serverUri,
            string endpointUrl,
            string sessionName,
            byte[] clientNonce,
            byte[] clientCertificate,
            double requestedSessionTimeout,
            uint maxResponseMessageSize,
            out NodeId sessionId,
            out NodeId authenticationToken,
            out double revisedSessionTimeout,
            out byte[] serverNonce,
            out byte[] serverCertificate,
            out EndpointDescriptionCollection serverEndpoints,
            out SignedSoftwareCertificateCollection serverSoftwareCertificates,
            out SignatureData serverSignature,
            out uint maxRequestMessageSize)
        {
            sessionId = 0;
            revisedSessionTimeout = 0;
            serverNonce = null;
            serverCertificate = null;
            serverSoftwareCertificates = null;
            serverSignature = null;
            maxRequestMessageSize = (uint)MessageContext.MaxMessageSize;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.CreateSession);
            Session session = null;
            try
            {
                // check the server uri.
                if (!String.IsNullOrEmpty(serverUri))
                {
                    if (serverUri != Configuration.ApplicationUri)
                    {
                        throw new ServiceResultException(StatusCodes.BadServerUriInvalid);
                    }
                }

                var requireEncryption = RequireEncryption(context?.ChannelContext?.EndpointDescription);

                if (!requireEncryption && clientCertificate != null)
                {
                    requireEncryption = true;
                }

                // validate client application instance certificate.
                X509Certificate2 parsedClientCertificate = null;

                if (requireEncryption && clientCertificate != null && clientCertificate.Length > 0)
                {
                    try
                    {
                        X509Certificate2Collection clientCertificateChain = Utils.ParseCertificateChainBlob(clientCertificate);
                        parsedClientCertificate = clientCertificateChain[0];

                        if (context.SecurityPolicyUri != SecurityPolicies.None)
                        {
                            var certificateApplicationUri = X509Utils.GetApplicationUriFromCertificate(parsedClientCertificate);

                            // verify if applicationUri from ApplicationDescription matches the applicationUri in the client certificate.
                            if (!String.IsNullOrEmpty(certificateApplicationUri) &&
                                !String.IsNullOrEmpty(clientDescription.ApplicationUri) &&
                                certificateApplicationUri != clientDescription.ApplicationUri)
                            {
                                // report the AuditCertificateDataMismatch event for invalid uri
                                ServerData?.ReportAuditCertificateDataMismatchEvent(parsedClientCertificate, null, clientDescription.ApplicationUri, StatusCodes.BadCertificateUriInvalid);

                                throw ServiceResultException.Create(
                                    StatusCodes.BadCertificateUriInvalid,
                                    "The URI specified in the ApplicationDescription {0} does not match the URI in the Certificate: {1}.",
                                    clientDescription.ApplicationUri, certificateApplicationUri);
                            }

                            CertificateValidator.Validate(clientCertificateChain);
                        }
                    }
                    catch (Exception e)
                    {
                        // report audit event for client certificate
                        ReportAuditCertificateEvent(parsedClientCertificate, e);

                        OnApplicationCertificateError(clientCertificate, new ServiceResult(e));
                    }
                }

                // verify the nonce provided by the client.
                if (clientNonce != null)
                {
                    if (clientNonce.Length < minNonceLength_)
                    {
                        throw new ServiceResultException(StatusCodes.BadNonceInvalid);
                    }

                    // ignore nonce if security policy set to none
                    if (context.SecurityPolicyUri == SecurityPolicies.None)
                    {
                        clientNonce = null;
                    }
                }

                // create the session.
                session = ServerData.SessionManager.CreateSession(
                    context,
                    requireEncryption ? InstanceCertificate : null,
                    sessionName,
                    clientNonce,
                    clientDescription,
                    endpointUrl,
                    parsedClientCertificate,
                    requestedSessionTimeout,
                    maxResponseMessageSize,
                    out sessionId,
                    out authenticationToken,
                    out serverNonce,
                    out revisedSessionTimeout);

                if (endpointUrl != null)
                {
                    try
                    {
                        // check the endpointurl
                        var configuredEndpoint = new ConfiguredEndpoint() {
                            EndpointUrl = new Uri(endpointUrl)
                        };

                        CertificateValidator.ValidateDomains(InstanceCertificate, configuredEndpoint, true);
                    }
                    catch (ServiceResultException sre) when (sre.StatusCode == StatusCodes.BadCertificateHostNameInvalid)
                    {
                        Utils.LogWarning("Server - Client connects with an endpointUrl [{0}] which does not match Server hostnames.", endpointUrl);
                        ServerData.ReportAuditUrlMismatchEvent(context?.AuditEntryId, session, revisedSessionTimeout, endpointUrl);
                    }
                }

                lock (lock_)
                {
                    // return the application instance certificate for the server.
                    if (requireEncryption)
                    {
                        // check if complete chain should be sent.
                        if (Configuration.SecurityConfiguration.SendCertificateChain &&
                            InstanceCertificateChain != null &&
                            InstanceCertificateChain.Count > 1)
                        {
                            var serverCertificateChain = new List<byte>();

                            for (var i = 0; i < InstanceCertificateChain.Count; i++)
                            {
                                serverCertificateChain.AddRange(InstanceCertificateChain[i].RawData);
                            }

                            serverCertificate = serverCertificateChain.ToArray();
                        }
                        else
                        {
                            serverCertificate = InstanceCertificate.RawData;
                        }
                    }

                    // return the endpoints supported by the server.
                    serverEndpoints = GetEndpointDescriptions(endpointUrl, BaseAddresses, null);

                    // return the software certificates assigned to the server.
                    serverSoftwareCertificates = new SignedSoftwareCertificateCollection(ServerProperties.SoftwareCertificates);

                    // sign the nonce provided by the client.
                    serverSignature = null;

                    //  sign the client nonce (if provided).
                    if (parsedClientCertificate != null && clientNonce != null)
                    {
                        var dataToSign = Utils.Append(parsedClientCertificate.RawData, clientNonce);
                        serverSignature = SecurityPolicies.Sign(InstanceCertificate, context.SecurityPolicyUri, dataToSign);
                    }
                }

                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.CurrentSessionCount++;
                    ServerData.ServerDiagnostics.CumulatedSessionCount++;
                }

                Utils.LogInfo("Server - SESSION CREATED. SessionId={0}", sessionId);
                // report audit for successful create session
                ServerData.ReportAuditCreateSessionEvent(context?.AuditEntryId, session, revisedSessionTimeout);

                return CreateResponse(requestHeader, StatusCodes.Good);
            }
            catch (ServiceResultException e)
            {
                Utils.LogError("Server - SESSION CREATE failed. {0}", e.Message);

                // report the failed AuditCreateSessionEvent
                ServerData.ReportAuditCreateSessionEvent(context?.AuditEntryId, session, revisedSessionTimeout, e);

                if (session != null)
                {
                    ServerData.SessionManager.CloseSession(session.Id);
                }

                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedSessionCount++;
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedSessionCount++;
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException((DiagnosticsMasks)requestHeader.ReturnDiagnostics, new StringCollection(), e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the ActivateSession service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="clientSignature">The client signature.</param>
        /// <param name="clientSoftwareCertificates">The client software certificates.</param>
        /// <param name="localeIds">The locale ids.</param>
        /// <param name="userIdentityToken">The user identity token.</param>
        /// <param name="userTokenSignature">The user token signature.</param>
        /// <param name="serverNonce">The server nonce.</param>
        /// <param name="results">The results.</param>
        /// <param name="diagnosticInfos">The diagnostic infos.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader ActivateSession(
            RequestHeader requestHeader,
            SignatureData clientSignature,
            SignedSoftwareCertificateCollection clientSoftwareCertificates,
            StringCollection localeIds,
            ExtensionObject userIdentityToken,
            SignatureData userTokenSignature,
            out byte[] serverNonce,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            serverNonce = null;
            results = null;
            diagnosticInfos = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.ActivateSession);
            // validate client's software certificates.
            var softwareCertificates = new List<SoftwareCertificate>();

            try
            {
                if (context?.SecurityPolicyUri != SecurityPolicies.None)
                {
                    var diagnosticsExist = false;

                    if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                    {
                        diagnosticInfos = new DiagnosticInfoCollection();
                    }

                    results = new StatusCodeCollection();
                    diagnosticInfos = new DiagnosticInfoCollection();

                    foreach (SignedSoftwareCertificate signedCertificate in clientSoftwareCertificates)
                    {

                        ServiceResult result = SoftwareCertificate.Validate(
                            CertificateValidator,
                            signedCertificate.CertificateData,
                            out SoftwareCertificate softwareCertificate);

                        if (ServiceResult.IsBad(result))
                        {
                            results.Add(result.Code);

                            // add diagnostics if requested.
                            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                            {
                                DiagnosticInfo diagnosticInfo = UaServerUtils.CreateDiagnosticInfo(ServerData, context, result);
                                diagnosticInfos.Add(diagnosticInfo);
                                diagnosticsExist = true;
                            }
                        }
                        else
                        {
                            softwareCertificates.Add(softwareCertificate);
                            results.Add(StatusCodes.Good);

                            // add diagnostics if requested.
                            if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
                            {
                                diagnosticInfos.Add(null);
                            }
                        }
                    }

                    if (!diagnosticsExist && diagnosticInfos != null)
                    {
                        diagnosticInfos.Clear();
                    }
                }

                // check if certificates meet the server's requirements.
                ValidateSoftwareCertificates(softwareCertificates);

                // activate the session.
                var identityChanged = ServerData.SessionManager.ActivateSession(
                    context,
                    requestHeader.AuthenticationToken,
                    clientSignature,
                    softwareCertificates,
                    userIdentityToken,
                    userTokenSignature,
                    localeIds,
                    out serverNonce);

                if (identityChanged)
                {
                    // TBD - call Node Manager and Subscription Manager.
                }

                Utils.LogInfo("Server - SESSION ACTIVATED.");

                // report the audit event for session activate
                Session session = ServerData.SessionManager.GetSession(requestHeader.AuthenticationToken);
                ServerData.ReportAuditActivateSessionEvent(context?.AuditEntryId, session, softwareCertificates);

                return CreateResponse(requestHeader, StatusCodes.Good);
            }
            catch (ServiceResultException e)
            {
                Utils.LogInfo("Server - SESSION ACTIVATE failed. {0}", e.Message);

                // report the audit event for failed session activate
                Session session = ServerData.SessionManager.GetSession(requestHeader.AuthenticationToken);
                ServerData.ReportAuditActivateSessionEvent(context?.AuditEntryId, session, softwareCertificates, e);

                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedSessionCount++;
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedSessionCount++;
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException((DiagnosticsMasks)requestHeader.ReturnDiagnostics, localeIds, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Returns whether the error is a security error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        /// 	<c>true</c> if the error is one of the security errors, otherwise <c>false</c>.
        /// </returns>
        protected bool IsSecurityError(StatusCode error)
        {
            switch (error.CodeBits)
            {
                case StatusCodes.BadUserSignatureInvalid:
                case StatusCodes.BadUserAccessDenied:
                case StatusCodes.BadSecurityPolicyRejected:
                case StatusCodes.BadSecurityModeRejected:
                case StatusCodes.BadSecurityChecksFailed:
                case StatusCodes.BadSecureChannelTokenUnknown:
                case StatusCodes.BadSecureChannelIdInvalid:
                case StatusCodes.BadNoValidCertificates:
                case StatusCodes.BadIdentityTokenInvalid:
                case StatusCodes.BadIdentityTokenRejected:
                case StatusCodes.BadIdentityChangeNotSupported:
                case StatusCodes.BadCertificateUseNotAllowed:
                case StatusCodes.BadCertificateUriInvalid:
                case StatusCodes.BadCertificateUntrusted:
                case StatusCodes.BadCertificateTimeInvalid:
                case StatusCodes.BadCertificateRevoked:
                case StatusCodes.BadCertificateRevocationUnknown:
                case StatusCodes.BadCertificateIssuerUseNotAllowed:
                case StatusCodes.BadCertificateIssuerTimeInvalid:
                case StatusCodes.BadCertificateIssuerRevoked:
                case StatusCodes.BadCertificateIssuerRevocationUnknown:
                case StatusCodes.BadCertificateInvalid:
                case StatusCodes.BadCertificateHostNameInvalid:
                case StatusCodes.BadCertificatePolicyCheckFailed:
                case StatusCodes.BadApplicationSignatureInvalid:
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the response header.
        /// </summary>
        /// <param name="requestHeader">The object that contains description for the RequestHeader DataType.</param>
        /// <param name="exception">The exception used to create DiagnosticInfo assigned to the ServiceDiagnostics.</param>
        /// <returns>Returns a description for the ResponseHeader DataType. </returns>
        protected ResponseHeader CreateResponse(RequestHeader requestHeader, ServiceResultException exception)
        {
            var responseHeader = new ResponseHeader {
                ServiceResult = exception.StatusCode,

                Timestamp = DateTime.UtcNow,
                RequestHandle = requestHeader.RequestHandle
            };

            var stringTable = new StringTable();
            responseHeader.ServiceDiagnostics = new DiagnosticInfo(exception, (DiagnosticsMasks)requestHeader.ReturnDiagnostics, true, stringTable);
            responseHeader.StringTable = stringTable.ToArray();

            return responseHeader;
        }

        /// <summary>
        /// Invokes the CloseSession service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="deleteSubscriptions">if set to <c>true</c> subscriptions are deleted.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader CloseSession(RequestHeader requestHeader, bool deleteSubscriptions)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.CloseSession);

            try
            {
                Session session = ServerData.SessionManager.GetSession(requestHeader.AuthenticationToken);

                ServerData.CloseSession(context, context.Session.Id, deleteSubscriptions);

                // report the audit event for close session                
                ServerData.ReportAuditCloseSessionEvent(context.AuditEntryId, session, "Session/CloseSession");

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the Cancel service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="requestHandle">The request handle assigned to the request.</param>
        /// <param name="cancelCount">The number of cancelled requests.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Cancel(
            RequestHeader requestHeader,
            uint requestHandle,
            out uint cancelCount)
        {
            cancelCount = 0;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Cancel);

            try
            {
                genericServerData_.RequestManager.CancelRequests(requestHandle, out cancelCount);
                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the Browse service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="view">The view.</param>
        /// <param name="requestedMaxReferencesPerNode">The maximum number of references to return for each node.</param>
        /// <param name="nodesToBrowse">The list of nodes to browse.</param>
        /// <param name="results">The list of results for the passed starting nodes and filters.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Browse(
            RequestHeader requestHeader,
            ViewDescription view,
            uint requestedMaxReferencesPerNode,
            BrowseDescriptionCollection nodesToBrowse,
            out BrowseResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            results = null;
            diagnosticInfos = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Browse);

            try
            {
                ValidateOperationLimits(nodesToBrowse, OperationLimits.MaxNodesPerBrowse);

                genericServerData_.NodeManager.Browse(
                    context,
                    view,
                    requestedMaxReferencesPerNode,
                    nodesToBrowse,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the BrowseNext service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="releaseContinuationPoints">if set to <c>true</c> the continuation points are released.</param>
        /// <param name="continuationPoints">A list of continuation points returned in a previous Browse or BrewseNext call.</param>
        /// <param name="results">The list of resulted references for browse.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader BrowseNext(
            RequestHeader requestHeader,
            bool releaseContinuationPoints,
            ByteStringCollection continuationPoints,
            out BrowseResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            results = null;
            diagnosticInfos = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.BrowseNext);

            try
            {
                ValidateOperationLimits(continuationPoints, OperationLimits.MaxNodesPerBrowse);

                genericServerData_.NodeManager.BrowseNext(
                    context,
                    releaseContinuationPoints,
                    continuationPoints,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the RegisterNodes service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="nodesToRegister">The list of NodeIds to register.</param>
        /// <param name="registeredNodeIds">The list of NodeIds identifying the registered nodes. </param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader RegisterNodes(
            RequestHeader requestHeader,
            NodeIdCollection nodesToRegister,
            out NodeIdCollection registeredNodeIds)
        {
            registeredNodeIds = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.RegisterNodes);

            try
            {
                ValidateOperationLimits(nodesToRegister, OperationLimits.MaxNodesPerRegisterNodes);

                genericServerData_.NodeManager.RegisterNodes(
                    context,
                    nodesToRegister,
                    out registeredNodeIds);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the UnregisterNodes service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="nodesToUnregister">The list of NodeIds to unregister</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader UnregisterNodes(RequestHeader requestHeader, NodeIdCollection nodesToUnregister)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.UnregisterNodes);

            try
            {
                ValidateOperationLimits(nodesToUnregister, OperationLimits.MaxNodesPerRegisterNodes);

                genericServerData_.NodeManager.UnregisterNodes(
                    context,
                    nodesToUnregister);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the TranslateBrowsePathsToNodeIds service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="browsePaths">The list of browse paths for which NodeIds are being requested.</param>
        /// <param name="results">The list of results for the list of browse paths.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader TranslateBrowsePathsToNodeIds(
            RequestHeader requestHeader,
            BrowsePathCollection browsePaths,
            out BrowsePathResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            results = null;
            diagnosticInfos = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.TranslateBrowsePathsToNodeIds);

            try
            {
                ValidateOperationLimits(browsePaths, OperationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds);

                foreach (BrowsePath bp in browsePaths)
                {
                    ValidateOperationLimits(bp.RelativePath.Elements.Count, OperationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds);
                }

                genericServerData_.NodeManager.TranslateBrowsePathsToNodeIds(
                    context,
                    browsePaths,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the Read service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="maxAge">The Maximum age of the value to be read in milliseconds.</param>
        /// <param name="timestampsToReturn">The type of timestamps to be returned for the requested Variables.</param>
        /// <param name="nodesToRead">The list of Nodes and their Attributes to read.</param>
        /// <param name="results">The list of returned Attribute values</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Read(
            RequestHeader requestHeader,
            double maxAge,
            TimestampsToReturn timestampsToReturn,
            ReadValueIdCollection nodesToRead,
            out DataValueCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Read);

            try
            {
                ValidateOperationLimits(nodesToRead, OperationLimits.MaxNodesPerRead);

                genericServerData_.NodeManager.Read(
                    context,
                    maxAge,
                    timestampsToReturn,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                ServerData.ReportAuditEvent(context, "Read", e);

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the HistoryRead service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="historyReadDetails">The history read details.</param>
        /// <param name="timestampsToReturn">The timestamps to return.</param>
        /// <param name="releaseContinuationPoints">if set to <c>true</c> continuation points are released.</param>
        /// <param name="nodesToRead">The nodes to read.</param>
        /// <param name="results">The results.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader HistoryRead(
            RequestHeader requestHeader,
            ExtensionObject historyReadDetails,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            HistoryReadValueIdCollection nodesToRead,
            out HistoryReadResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.HistoryRead);

            try
            {
                if (historyReadDetails?.Body is ReadEventDetails)
                {
                    ValidateOperationLimits(nodesToRead, OperationLimits.MaxNodesPerHistoryReadEvents);
                }
                else
                {
                    ValidateOperationLimits(nodesToRead, OperationLimits.MaxNodesPerHistoryReadData);
                }

                genericServerData_.NodeManager.HistoryRead(
                    context,
                    historyReadDetails,
                    timestampsToReturn,
                    releaseContinuationPoints,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                ServerData.ReportAuditEvent(context, "HistoryRead", e);

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the Write service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="nodesToWrite">The list of Nodes, Attributes, and values to write.</param>
        /// <param name="results">The list of write result status codes for each write operation.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Write(
            RequestHeader requestHeader,
            WriteValueCollection nodesToWrite,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Write);

            try
            {
                ValidateOperationLimits(nodesToWrite, OperationLimits.MaxNodesPerWrite);

                genericServerData_.NodeManager.Write(
                    context,
                    nodesToWrite,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the HistoryUpdate service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="historyUpdateDetails">The details defined for the update.</param>
        /// <param name="results">The list of update results for the history update details.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader HistoryUpdate(
            RequestHeader requestHeader,
            ExtensionObjectCollection historyUpdateDetails,
            out HistoryUpdateResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.HistoryUpdate);

            try
            {
                // check only for BadNothingToDo here
                // MaxNodesPerHistoryUpdateEvents & MaxNodesPerHistoryUpdateData
                // must be checked in NodeManager (TODO)
                ValidateOperationLimits(historyUpdateDetails);

                genericServerData_.NodeManager.HistoryUpdate(
                    context,
                    historyUpdateDetails,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the CreateSubscription service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="requestedPublishingInterval">The cyclic rate that the Subscription is being requested to return Notifications to the Client.</param>
        /// <param name="requestedLifetimeCount">The client-requested lifetime count for the Subscription</param>
        /// <param name="requestedMaxKeepAliveCount">The requested max keep alive count.</param>
        /// <param name="maxNotificationsPerPublish">The maximum number of notifications that the Client wishes to receive in a single Publish response.</param>
        /// <param name="publishingEnabled">If set to <c>true</c> publishing is enabled for the Subscription.</param>
        /// <param name="priority">The relative priority of the Subscription.</param>
        /// <param name="subscriptionId">The Server-assigned identifier for the Subscription.</param>
        /// <param name="revisedPublishingInterval">The actual publishing interval that the Server will use.</param>
        /// <param name="revisedLifetimeCount">The revised lifetime count.</param>
        /// <param name="revisedMaxKeepAliveCount">The revised max keep alive count.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader CreateSubscription(
            RequestHeader requestHeader,
            double requestedPublishingInterval,
            uint requestedLifetimeCount,
            uint requestedMaxKeepAliveCount,
            uint maxNotificationsPerPublish,
            bool publishingEnabled,
            byte priority,
            out uint subscriptionId,
            out double revisedPublishingInterval,
            out uint revisedLifetimeCount,
            out uint revisedMaxKeepAliveCount)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.CreateSubscription);

            try
            {
                ServerData.SubscriptionManager.CreateSubscription(
                    context,
                    requestedPublishingInterval,
                    requestedLifetimeCount,
                    requestedMaxKeepAliveCount,
                    maxNotificationsPerPublish,
                    publishingEnabled,
                    priority,
                    out subscriptionId,
                    out revisedPublishingInterval,
                    out revisedLifetimeCount,
                    out revisedMaxKeepAliveCount);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the TransferSubscriptions service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionIds">The list of Subscriptions to transfer.</param>
        /// <param name="sendInitialValues">If the initial values should be sent.</param>
        /// <param name="results">The list of result StatusCodes for the Subscriptions to transfer.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        public override ResponseHeader TransferSubscriptions(
            RequestHeader requestHeader,
            UInt32Collection subscriptionIds,
            bool sendInitialValues,
            out TransferResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            results = null;
            diagnosticInfos = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.TransferSubscriptions);

            try
            {
                ValidateOperationLimits(subscriptionIds);

                ServerData.SubscriptionManager.TransferSubscriptions(
                    context,
                    subscriptionIds,
                    sendInitialValues,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the DeleteSubscriptions service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionIds">The list of Subscriptions to delete.</param>
        /// <param name="results">The list of result StatusCodes for the Subscriptions to delete.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader DeleteSubscriptions(
            RequestHeader requestHeader,
            UInt32Collection subscriptionIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.DeleteSubscriptions);

            try
            {
                ValidateOperationLimits(subscriptionIds);

                ServerData.SubscriptionManager.DeleteSubscriptions(
                    context,
                    subscriptionIds,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the Publish service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionAcknowledgements">The list of acknowledgements for one or more Subscriptions.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="availableSequenceNumbers">The available sequence numbers.</param>
        /// <param name="moreNotifications">If set to <c>true</c> the number of Notifications that were ready to be sent could not be sent in a single response.</param>
        /// <param name="notificationMessage">The NotificationMessage that contains the list of Notifications.</param>
        /// <param name="results">The list of results for the acknowledgements.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Publish(
            RequestHeader requestHeader,
            SubscriptionAcknowledgementCollection subscriptionAcknowledgements,
            out uint subscriptionId,
            out UInt32Collection availableSequenceNumbers,
            out bool moreNotifications,
            out NotificationMessage notificationMessage,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Publish);

            try
            {
                /*
                // check if there is an odd delay.
                if (DateTime.UtcNow > requestHeader.Timestamp.AddMilliseconds(100))
                {
                    Utils.LogTrace(m_eventId,
                        "WARNING. Unexpected delay receiving Publish request. Time={0:hh:mm:ss.fff}, ReceiveTime={1:hh:mm:ss.fff}",
                        DateTime.UtcNow,
                        requestHeader.Timestamp);
                }
                */

                Utils.LogTrace("PUBLISH #{0} RECEIVED. TIME={1:hh:mm:ss.fff}", requestHeader.RequestHandle, requestHeader.Timestamp);

                notificationMessage = ServerData.SubscriptionManager.Publish(
                    context,
                    subscriptionAcknowledgements,
                    null,
                    out subscriptionId,
                    out availableSequenceNumbers,
                    out moreNotifications,
                    out results,
                    out diagnosticInfos);

                /*
                if (notificationMessage != null)
                {
                    Utils.LogTrace(m_eventId, 
                        "PublishResponse: SubId={0} SeqNo={1}, PublishTime={2:mm:ss.fff}, Time={3:mm:ss.fff}",
                        subscriptionId,
                        notificationMessage.SequenceNumber,
                        notificationMessage.PublishTime,
                        DateTime.UtcNow);
                }
                */

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Begins an asynchronous publish operation.
        /// </summary>
        /// <param name="request">The request.</param>
        public virtual void BeginPublish(IEndpointIncomingRequest request)
        {
            var input = (PublishRequest)request.Request;
            UaServerOperationContext context = ValidateRequest(input.RequestHeader, RequestType.Publish);

            try
            {
                var operation = new AsyncPublishOperation(context, request, this);

                NotificationMessage notificationMessage = null;

                notificationMessage = ServerData.SubscriptionManager.Publish(
                    context,
                    input.SubscriptionAcknowledgements,
                    operation,
                    out var subscriptionId,
                    out UInt32Collection availableSequenceNumbers,
                    out var moreNotifications,
                    out StatusCodeCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                // request completed asynchronously.
                if (notificationMessage != null)
                {
                    OnRequestComplete(context);

                    operation.Response.ResponseHeader = CreateResponse(input.RequestHeader, context.StringTable);
                    operation.Response.SubscriptionId = subscriptionId;
                    operation.Response.AvailableSequenceNumbers = availableSequenceNumbers;
                    operation.Response.MoreNotifications = moreNotifications;
                    operation.Response.Results = results;
                    operation.Response.DiagnosticInfos = diagnosticInfos;
                    operation.Response.NotificationMessage = notificationMessage;

                    Utils.LogTrace("PUBLISH: #{0} Completed Synchronously", input.RequestHeader.RequestHandle);
                    request.OperationCompleted(operation.Response, null);
                }
            }
            catch (ServiceResultException e)
            {
                OnRequestComplete(context);

                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
        }

        /// <summary>
        /// Completes an asynchronous publish operation.
        /// </summary>
        /// <param name="request">The request.</param>
        public virtual void CompletePublish(IEndpointIncomingRequest request)
        {
            var operation = (AsyncPublishOperation)request.Calldata;
            UaServerOperationContext context = operation.Context;

            try
            {
                if (ServerData.SubscriptionManager.CompletePublish(context, operation))
                {
                    operation.Response.ResponseHeader = CreateResponse(request.Request.RequestHeader, context.StringTable);
                    request.OperationCompleted(operation.Response, null);
                    OnRequestComplete(context);
                }
            }
            catch (ServiceResultException e)
            {
                OnRequestComplete(context);

                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
        }

        /// <summary>
        /// Invokes the Republish service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="retransmitSequenceNumber">The sequence number of a specific NotificationMessage to be republished.</param>
        /// <param name="notificationMessage">The requested NotificationMessage.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Republish(
            RequestHeader requestHeader,
            uint subscriptionId,
            uint retransmitSequenceNumber,
            out NotificationMessage notificationMessage)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Republish);

            try
            {
                notificationMessage = ServerData.SubscriptionManager.Republish(
                    context,
                    subscriptionId,
                    retransmitSequenceNumber);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the ModifySubscription service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="requestedPublishingInterval">The cyclic rate that the Subscription is being requested to return Notifications to the Client.</param>
        /// <param name="requestedLifetimeCount">The client-requested lifetime count for the Subscription.</param>
        /// <param name="requestedMaxKeepAliveCount">The requested max keep alive count.</param>
        /// <param name="maxNotificationsPerPublish">The maximum number of notifications that the Client wishes to receive in a single Publish response.</param>
        /// <param name="priority">The relative priority of the Subscription.</param>
        /// <param name="revisedPublishingInterval">The revised publishing interval.</param>
        /// <param name="revisedLifetimeCount">The revised lifetime count.</param>
        /// <param name="revisedMaxKeepAliveCount">The revised max keep alive count.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader ModifySubscription(
            RequestHeader requestHeader,
            uint subscriptionId,
            double requestedPublishingInterval,
            uint requestedLifetimeCount,
            uint requestedMaxKeepAliveCount,
            uint maxNotificationsPerPublish,
            byte priority,
            out double revisedPublishingInterval,
            out uint revisedLifetimeCount,
            out uint revisedMaxKeepAliveCount)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.ModifySubscription);

            try
            {
                ServerData.SubscriptionManager.ModifySubscription(
                    context,
                    subscriptionId,
                    requestedPublishingInterval,
                    requestedLifetimeCount,
                    requestedMaxKeepAliveCount,
                    maxNotificationsPerPublish,
                    priority,
                    out revisedPublishingInterval,
                    out revisedLifetimeCount,
                    out revisedMaxKeepAliveCount);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the SetPublishingMode service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="publishingEnabled">If set to <c>true</c> publishing of NotificationMessages is enabled for the Subscription.</param>
        /// <param name="subscriptionIds">The list of subscription ids.</param>
        /// <param name="results">The list of StatusCodes for the Subscriptions to enable/disable.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object 
        /// </returns>
        public override ResponseHeader SetPublishingMode(
            RequestHeader requestHeader,
            bool publishingEnabled,
            UInt32Collection subscriptionIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.SetPublishingMode);

            try
            {
                ValidateOperationLimits(subscriptionIds);

                ServerData.SubscriptionManager.SetPublishingMode(
                    context,
                    publishingEnabled,
                    subscriptionIds,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the SetTriggering service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="triggeringItemId">The id for the UaMonitoredItem used as the triggering item.</param>
        /// <param name="linksToAdd">The list of ids of the items to report that are to be added as triggering links.</param>
        /// <param name="linksToRemove">The list of ids of the items to report for the triggering links to be deleted.</param>
        /// <param name="addResults">The list of StatusCodes for the items to add.</param>
        /// <param name="addDiagnosticInfos">The list of diagnostic information for the links to add.</param>
        /// <param name="removeResults">The list of StatusCodes for the items to delete.</param>
        /// <param name="removeDiagnosticInfos">The list of diagnostic information for the links to delete.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader SetTriggering(
            RequestHeader requestHeader,
            uint subscriptionId,
            uint triggeringItemId,
            UInt32Collection linksToAdd,
            UInt32Collection linksToRemove,
            out StatusCodeCollection addResults,
            out DiagnosticInfoCollection addDiagnosticInfos,
            out StatusCodeCollection removeResults,
            out DiagnosticInfoCollection removeDiagnosticInfos)
        {
            addResults = null;
            addDiagnosticInfos = null;
            removeResults = null;
            removeDiagnosticInfos = null;

            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.SetTriggering);

            try
            {
                if ((linksToAdd == null || linksToAdd.Count == 0) && (linksToRemove == null || linksToRemove.Count == 0))
                {
                    throw new ServiceResultException(StatusCodes.BadNothingToDo);
                }

                var monitoredItemsCount = 0;
                monitoredItemsCount += (linksToAdd?.Count) ?? 0;
                monitoredItemsCount += (linksToRemove?.Count) ?? 0;
                ValidateOperationLimits(monitoredItemsCount, OperationLimits.MaxMonitoredItemsPerCall);

                ServerData.SubscriptionManager.SetTriggering(
                    context,
                    subscriptionId,
                    triggeringItemId,
                    linksToAdd,
                    linksToRemove,
                    out addResults,
                    out addDiagnosticInfos,
                    out removeResults,
                    out removeDiagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the CreateMonitoredItems service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id that will report notifications.</param>
        /// <param name="timestampsToReturn">The type of timestamps to be returned for the MonitoredItems.</param>
        /// <param name="itemsToCreate">The list of MonitoredItems to be created and assigned to the specified subscription</param>
        /// <param name="results">The list of results for the MonitoredItems to create.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader CreateMonitoredItems(
            RequestHeader requestHeader,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemCreateRequestCollection itemsToCreate,
            out MonitoredItemCreateResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.CreateMonitoredItems);

            try
            {
                ValidateOperationLimits(itemsToCreate, OperationLimits.MaxMonitoredItemsPerCall);

                ServerData.SubscriptionManager.CreateMonitoredItems(
                    context,
                    subscriptionId,
                    timestampsToReturn,
                    itemsToCreate,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the ModifyMonitoredItems service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="timestampsToReturn">The type of timestamps to be returned for the MonitoredItems.</param>
        /// <param name="itemsToModify">The list of MonitoredItems to modify.</param>
        /// <param name="results">The list of results for the MonitoredItems to modify.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader ModifyMonitoredItems(
            RequestHeader requestHeader,
            uint subscriptionId,
            TimestampsToReturn timestampsToReturn,
            MonitoredItemModifyRequestCollection itemsToModify,
            out MonitoredItemModifyResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.ModifyMonitoredItems);

            try
            {
                ValidateOperationLimits(itemsToModify, OperationLimits.MaxMonitoredItemsPerCall);

                ServerData.SubscriptionManager.ModifyMonitoredItems(
                    context,
                    subscriptionId,
                    timestampsToReturn,
                    itemsToModify,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the DeleteMonitoredItems service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="monitoredItemIds">The list of MonitoredItems to delete.</param>
        /// <param name="results">The list of results for the MonitoredItems to delete.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader DeleteMonitoredItems(
            RequestHeader requestHeader,
            uint subscriptionId,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.DeleteMonitoredItems);

            try
            {
                ValidateOperationLimits(monitoredItemIds, OperationLimits.MaxMonitoredItemsPerCall);

                ServerData.SubscriptionManager.DeleteMonitoredItems(
                    context,
                    subscriptionId,
                    monitoredItemIds,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the SetMonitoringMode service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="monitoringMode">The monitoring mode to be set for the MonitoredItems.</param>
        /// <param name="monitoredItemIds">The list of MonitoredItems to modify.</param>
        /// <param name="results">The list of results for the MonitoredItems to modify.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader SetMonitoringMode(
            RequestHeader requestHeader,
            uint subscriptionId,
            MonitoringMode monitoringMode,
            UInt32Collection monitoredItemIds,
            out StatusCodeCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.SetMonitoringMode);

            try
            {
                ValidateOperationLimits(monitoredItemIds, OperationLimits.MaxMonitoredItemsPerCall);

                ServerData.SubscriptionManager.SetMonitoringMode(
                    context,
                    subscriptionId,
                    monitoringMode,
                    monitoredItemIds,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }

        /// <summary>
        /// Invokes the Call service.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="methodsToCall">The methods to call.</param>
        /// <param name="results">The results.</param>
        /// <param name="diagnosticInfos">The diagnostic information for the results.</param>
        /// <returns>
        /// Returns a <see cref="ResponseHeader"/> object
        /// </returns>
        public override ResponseHeader Call(
            RequestHeader requestHeader,
            CallMethodRequestCollection methodsToCall,
            out CallMethodResultCollection results,
            out DiagnosticInfoCollection diagnosticInfos)
        {
            UaServerOperationContext context = ValidateRequest(requestHeader, RequestType.Call);

            try
            {
                ValidateOperationLimits(methodsToCall, OperationLimits.MaxNodesPerMethodCall);

                genericServerData_.NodeManager.Call(
                    context,
                    methodsToCall,
                    out results,
                    out diagnosticInfos);

                return CreateResponse(requestHeader, context.StringTable);
            }
            catch (ServiceResultException e)
            {
                lock (ServerData.DiagnosticsWriteLock)
                {
                    ServerData.ServerDiagnostics.RejectedRequestsCount++;

                    if (IsSecurityError(e.StatusCode))
                    {
                        ServerData.ServerDiagnostics.SecurityRejectedRequestsCount++;
                    }
                }

                throw TranslateException(context, e);
            }
            finally
            {
                OnRequestComplete(context);
            }
        }
        #endregion

        #region Public Methods used by the Host Process
        /// <summary>
        /// The state object associated with the server.
        /// It provides the shared components for the Server.
        /// </summary>
        /// <value>The current instance.</value>
        public IUaServerData CurrentInstance
        {
            get
            {
                lock (lock_)
                {
                    return genericServerData_ == null ? throw new ServiceResultException(StatusCodes.BadServerHalted) : (IUaServerData)genericServerData_;
                }
            }
        }

        /// <summary>
        /// Returns the current status of the server.
        /// </summary>
        /// <returns>Returns a ServerStatusDataType object</returns>
        public ServerStatusDataType GetStatus()
        {
            lock (lock_)
            {
                return genericServerData_ == null ? throw new ServiceResultException(StatusCodes.BadServerHalted) : genericServerData_.Status.Value;
            }
        }

        /// <summary>
        /// Registers the server with the discovery server.
        /// </summary>
        /// <returns>Boolean value.</returns>
        public bool RegisterWithDiscoveryServer()
        {
            var configuration = new ApplicationConfiguration(Configuration);

            // use a dedicated certificate validator with the registration, but derive behavior from server config
            var registrationCertificateValidator = new CertificateValidationEventHandler(OnCertificateValidation);
            configuration.CertificateValidator = new CertificateValidator();
            configuration.CertificateValidator.CertificateValidation += registrationCertificateValidator;
            configuration.CertificateValidator.Update(configuration.SecurityConfiguration).GetAwaiter().GetResult();

            try
            {
                // try each endpoint.
                if (registrationEndpoints_ != null)
                {
                    foreach (ConfiguredEndpoint endpoint in registrationEndpoints_.Endpoints)
                    {
                        RegistrationClient client = null;
                        var i = 0;

                        while (i++ < 2)
                        {
                            try
                            {
                                // update from the server.
                                var updateRequired = true;

                                lock (registrationLock_)
                                {
                                    updateRequired = endpoint.UpdateBeforeConnect;
                                }

                                if (updateRequired)
                                {
                                    endpoint.UpdateFromServer();
                                }

                                lock (registrationLock_)
                                {
                                    endpoint.UpdateBeforeConnect = false;
                                }

                                var requestHeader = new RequestHeader {
                                    Timestamp = DateTime.UtcNow
                                };

                                // create the client.
                                client = RegistrationClient.Create(
                                    configuration,
                                    endpoint.Description,
                                    endpoint.Configuration,
                                    InstanceCertificate);

                                client.OperationTimeout = 10000;

                                // register the server.
                                if (useRegisterServer2_)
                                {
                                    var discoveryConfiguration = new ExtensionObjectCollection();
                                    var mdnsDiscoveryConfig = new MdnsDiscoveryConfiguration {
                                        ServerCapabilities = configuration.ServerConfiguration.ServerCapabilities,
                                        MdnsServerName = Utils.GetHostName()
                                    };
                                    var extensionObject = new ExtensionObject(mdnsDiscoveryConfig);
                                    discoveryConfiguration.Add(extensionObject);
                                    _ = client.RegisterServer2(
                                        requestHeader,
                                        registrationInfo_,
                                        discoveryConfiguration,
                                        out StatusCodeCollection configurationResults,
                                        out DiagnosticInfoCollection diagnosticInfos);
                                }
                                else
                                {
                                    _ = client.RegisterServer(requestHeader, registrationInfo_);
                                }

                                return true;
                            }
                            catch (Exception e)
                            {
                                Utils.LogWarning("RegisterServer{0} failed for at: {1}. Exception={2}",
                                    useRegisterServer2_ ? "2" : "", endpoint.EndpointUrl, e.Message);
                                useRegisterServer2_ = !useRegisterServer2_;
                            }
                            finally
                            {
                                if (client != null)
                                {
                                    try
                                    {
                                        _ = client.Close();
                                        client = null;
                                    }
                                    catch (Exception e)
                                    {
                                        Utils.LogWarning("Could not cleanly close connection with LDS. Exception={0}", e.Message);
                                    }
                                }
                            }
                        }
                    }
                    // retry to start with RegisterServer2 if both failed
                    useRegisterServer2_ = true;
                }
            }
            finally
            {
                if (configuration != null)
                {
                    configuration.CertificateValidator.CertificateValidation -= registrationCertificateValidator;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks that the domains in the certificate match the current host.
        /// </summary>
        private void OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            System.Net.IPAddress[] targetAddresses = Utils.GetHostAddresses(Utils.GetHostName());

            foreach (var domain in X509Utils.GetDomainsFromCertificate(e.Certificate))
            {
                System.Net.IPAddress[] actualAddresses = Utils.GetHostAddresses(domain);

                foreach (System.Net.IPAddress actualAddress in actualAddresses)
                {
                    foreach (System.Net.IPAddress targetAddress in targetAddresses)
                    {
                        if (targetAddress.Equals(actualAddress))
                        {
                            e.Accept = true;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers the server endpoints with the LDS.
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnRegisterServer(object state)
        {
            try
            {
                lock (registrationLock_)
                {
                    // halt any outstanding timer.
                    if (registrationTimer_ != null)
                    {
                        registrationTimer_.Dispose();
                        registrationTimer_ = null;
                    }
                }

                if (RegisterWithDiscoveryServer())
                {
                    // schedule next registration.                        
                    lock (registrationLock_)
                    {
                        if (maxRegistrationInterval_ > 0)
                        {
                            registrationTimer_ = new Timer(
                                OnRegisterServer,
                                this,
                                maxRegistrationInterval_,
                                Timeout.Infinite);

                            lastRegistrationInterval_ = minRegistrationInterval_;
                            Utils.LogInfo("Register server succeeded. Registering again in {0} ms", maxRegistrationInterval_);
                        }
                    }
                }
                else
                {
                    lock (registrationLock_)
                    {
                        if (registrationTimer_ == null)
                        {
                            // calculate next registration attempt.
                            lastRegistrationInterval_ *= 2;

                            if (lastRegistrationInterval_ > maxRegistrationInterval_)
                            {
                                lastRegistrationInterval_ = maxRegistrationInterval_;
                            }

                            Utils.LogInfo("Register server failed. Trying again in {0} ms", lastRegistrationInterval_);

                            // create timer.
                            registrationTimer_ = new Timer(OnRegisterServer, this, lastRegistrationInterval_, Timeout.Infinite);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected exception handling registration timer.");
            }
        }
        #endregion

        #region Protected Members used for Request Processing
        /// <summary>
        /// The synchronization object.
        /// </summary>
        protected object Lock => lock_;

        /// <summary>
        /// The state object associated with the server.
        /// </summary>
        /// <value>The server internal data.</value>
        protected GenericServerData ServerData
        {
            get
            {
                GenericServerData serverInternal = genericServerData_;

                return serverInternal ?? throw new ServiceResultException(StatusCodes.BadServerHalted);
            }
        }

        /// <summary>
        /// Verifies that the request header is valid.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        protected override void ValidateRequest(RequestHeader requestHeader)
        {
            // check for server error.
            ServiceResult error = ServerError;

            if (ServiceResult.IsBad(error))
            {
                throw new ServiceResultException(error);
            }

            // check server state.
            GenericServerData serverInternal = genericServerData_;

            if (serverInternal == null || !serverInternal.IsRunning)
            {
                throw new ServiceResultException(StatusCodes.BadServerHalted);
            }

            base.ValidateRequest(requestHeader);
        }

        /// <summary>
        /// Updates the server state.
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void SetServerState(ServerState state)
        {
            lock (lock_)
            {
                if (ServiceResult.IsBad(ServerError))
                {
                    throw new ServiceResultException(ServerError);
                }

                if (genericServerData_ == null)
                {
                    throw new ServiceResultException(StatusCodes.BadServerHalted);
                }

                Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - Enter {0} state.", state.ToString());

                genericServerData_.CurrentState = state;
            }
        }

        /// <summary>
        /// Reports an error during initialization after the base server object has been started.
        /// </summary>
        /// <param name="error">The error.</param>
        protected virtual void SetServerError(ServiceResult error)
        {
            lock (lock_)
            {
                ServerError = error;
            }
        }

        /// <summary>
        /// Handles an error when validating the application instance certificate provided by a client.
        /// </summary>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="result">The result.</param>
        protected virtual void OnApplicationCertificateError(byte[] clientCertificate, ServiceResult result)
        {
            throw new ServiceResultException(result);
        }

        /// <summary>
        /// Inspects the software certificates provided by the server.
        /// </summary>
        /// <param name="softwareCertificates">The software certificates.</param>
        protected virtual void ValidateSoftwareCertificates(List<SoftwareCertificate> softwareCertificates)
        {
            // always accept valid certificates.
        }

        /// <summary>
        /// Verifies that the request header is valid.
        /// </summary>
        /// <param name="requestHeader">The request header.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <returns></returns>
        protected virtual UaServerOperationContext ValidateRequest(RequestHeader requestHeader, RequestType requestType)
        {
            base.ValidateRequest(requestHeader);

            if (!ServerData.IsRunning)
            {
                throw new ServiceResultException(StatusCodes.BadServerHalted);
            }

            UaServerOperationContext context = ServerData.SessionManager.ValidateRequest(requestHeader, requestType);

            UaServerUtils.EventLog.ServerCall(context.RequestType.ToString(), context.RequestId);

            // notify the request manager.
            ServerData.RequestManager.RequestReceived(context);

            return context;
        }

        /// <summary>
        /// Validate operation limits.
        /// </summary>
        /// <param name="operation">A list of operations.</param>
        /// <param name="operationLimit">The operation limit property.</param>
        /// <exception cref="ServiceResultException">BadNothingToDo if list is null or empty.</exception>
        /// <exception cref="ServiceResultException">BadTooManyOperations if list is larger than operation limit property.</exception>
        protected void ValidateOperationLimits(IList operation, PropertyState<uint> operationLimit = null)
        {
            if (operation == null || operation.Count == 0)
            {
                throw new ServiceResultException(StatusCodes.BadNothingToDo);
            }
            ValidateOperationLimits(operation.Count, operationLimit);
        }

        /// <summary>
        /// Validate operation limits.
        /// </summary>
        /// <param name="count">A count of operations.</param>
        /// <param name="operationLimit">The operation limit property.</param>
        /// <exception cref="ServiceResultException">BadTooManyOperations if count is larger than operation limit property.</exception>
        protected void ValidateOperationLimits(int count, PropertyState<uint> operationLimit)
        {
            var operationLimitValue = (operationLimit != null) ? operationLimit.Value : 0;
            if (operationLimitValue > 0 && count > operationLimitValue)
            {
                throw new ServiceResultException(StatusCodes.BadTooManyOperations);
            }
        }

        /// <summary>
        /// Translates an exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="e">The ServiceResultException e.</param>
        /// <returns>Returns an exception thrown when a UA defined error occurs, the return type is <seealso cref="ServiceResultException"/>.</returns>
        protected virtual ServiceResultException TranslateException(UaServerOperationContext context, ServiceResultException e)
        {
            IList<string> preferredLocales = null;

            if (context != null && context.Session != null)
            {
                preferredLocales = context.Session.PreferredLocales;
            }

            return TranslateException(context.DiagnosticsMask, preferredLocales, e);
        }

        /// <summary>
        /// Translates an exception.
        /// </summary>
        /// <param name="diagnosticsMasks">The fields to return.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="e">The ServiceResultException e.</param>
        /// <returns>Returns an exception thrown when a UA defined error occurs, the return type is <seealso cref="ServiceResultException"/>.</returns>
        protected virtual ServiceResultException TranslateException(DiagnosticsMasks diagnosticsMasks, IList<string> preferredLocales, ServiceResultException e)
        {
            if (e == null)
            {
                return null;
            }

            // check if inner result required.
            ServiceResult innerResult = null;

            if ((diagnosticsMasks & (DiagnosticsMasks.ServiceInnerDiagnostics | DiagnosticsMasks.ServiceInnerStatusCode)) != 0)
            {
                innerResult = e.InnerResult;
            }

            // check if translated text required.
            LocalizedText translatedText = null;

            if ((diagnosticsMasks & DiagnosticsMasks.ServiceLocalizedText) != 0)
            {
                translatedText = e.LocalizedText;
            }

            // create new result object.
            var result = new ServiceResult(
                e.StatusCode,
                e.SymbolicId,
                e.NamespaceUri,
                translatedText,
                e.AdditionalInfo,
                innerResult);

            // translate result.
            result = genericServerData_.ResourceManager.Translate(preferredLocales, result);
            return new ServiceResultException(result);
        }

        /// <summary>
        /// Translates a service result.
        /// </summary>
        /// <param name="diagnosticsMasks">The fields to return.</param>
        /// <param name="preferredLocales">The preferred locales.</param>
        /// <param name="result">The result.</param>
        /// <returns>Returns a class that combines the status code and diagnostic info structures.</returns>
        protected virtual ServiceResult TranslateResult(DiagnosticsMasks diagnosticsMasks, IList<string> preferredLocales, ServiceResult result)
        {
            return result == null ? null : genericServerData_.ResourceManager.Translate(preferredLocales, result);
        }

        /// <summary>
        /// Verifies that the request header is valid.
        /// </summary>
        /// <param name="context">The operation context.</param>
        protected virtual void OnRequestComplete(UaServerOperationContext context)
        {
            lock (lock_)
            {
                if (genericServerData_ == null)
                {
                    throw new ServiceResultException(StatusCodes.BadServerHalted);
                }

                genericServerData_.RequestManager.RequestCompleted(context);
            }
        }
        #endregion

        #region Protected Members used for Initialization
        /// <summary>
        /// Raised when the configuration changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ConfigurationWatcherEventArgs"/> instance containing the event data.</param>
        protected virtual async void OnConfigurationChanged(object sender, ConfigurationWatcherEventArgs args)
        {
            try
            {
                ApplicationConfiguration configuration = await ApplicationConfiguration.Load(
                    new FileInfo(args.FilePath),
                    Configuration.ApplicationType,
                    Configuration.GetType());

                OnUpdateConfiguration(configuration);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Could not load updated configuration file from: {0}", args);
            }
        }

        /// <summary>
        /// Called when the server configuration is changed on disk.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <remarks>
        /// Servers are free to ignore changes if it is difficult/impossible to apply them without a restart.
        /// </remarks>
        protected override void OnUpdateConfiguration(ApplicationConfiguration configuration)
        {
            lock (lock_)
            {
                // update security configuration.
                configuration.SecurityConfiguration.Validate();

                Configuration.SecurityConfiguration.TrustedIssuerCertificates = configuration.SecurityConfiguration.TrustedIssuerCertificates;
                Configuration.SecurityConfiguration.TrustedPeerCertificates = configuration.SecurityConfiguration.TrustedPeerCertificates;
                Configuration.SecurityConfiguration.RejectedCertificateStore = configuration.SecurityConfiguration.RejectedCertificateStore;

                Configuration.CertificateValidator.Update(Configuration.SecurityConfiguration).Wait();

                // update trace configuration.
                Configuration.TraceConfiguration = configuration.TraceConfiguration;

                if (Configuration.TraceConfiguration == null)
                {
                    Configuration.TraceConfiguration = new TraceConfiguration();
                }

                Configuration.TraceConfiguration.ApplySettings();
            }
        }

        /// <summary>
        /// Called before the server starts.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            lock (lock_)
            {
                base.OnServerStarting(configuration);

                // save minimum nonce length.
                minNonceLength_ = configuration.SecurityConfiguration.NonceLength;

                // try first RegisterServer2
                useRegisterServer2_ = true;
            }
        }

        /// <summary>
        /// Creates the endpoints and creates the hosts.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="bindingFactory">The transport listener binding factory.</param>
        /// <param name="serverDescription">The server description.</param>
        /// <param name="endpoints">The endpoints.</param>
        /// <returns>
        /// Returns IList of a host for a UA service.
        /// </returns>
        protected override IList<ServiceHost> InitializeServiceHosts(
            ApplicationConfiguration configuration,
            TransportListenerBindings bindingFactory,
            out ApplicationDescription serverDescription,
            out EndpointDescriptionCollection endpoints)
        {
            serverDescription = null;
            endpoints = null;

            var hosts = new Dictionary<string, ServiceHost>();

            // ensure at least one security policy exists.
            if (configuration.ServerConfiguration.SecurityPolicies.Count == 0)
            {
                configuration.ServerConfiguration.SecurityPolicies.Add(new ServerSecurityPolicy());
            }

            // ensure at least one user token policy exists.
            if (configuration.ServerConfiguration.UserTokenPolicies.Count == 0)
            {
                var userTokenPolicy = new UserTokenPolicy {
                    TokenType = UserTokenType.Anonymous
                };
                userTokenPolicy.PolicyId = userTokenPolicy.TokenType.ToString();

                configuration.ServerConfiguration.UserTokenPolicies.Add(userTokenPolicy);
            }

            // set server description.
            serverDescription = new ApplicationDescription {
                ApplicationUri = configuration.ApplicationUri,
                ApplicationName = new LocalizedText("en-US", configuration.ApplicationName),
                ApplicationType = configuration.ApplicationType,
                ProductUri = configuration.ProductUri,
                DiscoveryUrls = GetDiscoveryUrls()
            };

            endpoints = new EndpointDescriptionCollection();
            IList<EndpointDescription> endpointsForHost = null;

            StringCollection baseAddresses = configuration.ServerConfiguration.BaseAddresses;
            IEnumerable<string> requiredSchemes = Utils.DefaultUriSchemes.Where(scheme => baseAddresses.Any(a => a.StartsWith(scheme, StringComparison.Ordinal)));

            foreach (var scheme in requiredSchemes)
            {
                ITransportListenerFactory binding = bindingFactory.GetBinding(scheme);
                if (binding != null)
                {
                    endpointsForHost = binding.CreateServiceHost(
                        this,
                        hosts,
                        configuration,
                        configuration.ServerConfiguration.BaseAddresses,
                        serverDescription,
                        configuration.ServerConfiguration.SecurityPolicies,
                        InstanceCertificate,
                        InstanceCertificateChain
                        );
                    endpoints.AddRange(endpointsForHost);
                }
            }

            return new List<ServiceHost>(hosts.Values);
        }

        /// <summary>
        /// Creates an instance of the service host.
        /// </summary>
        public override ServiceHost CreateServiceHost(ServerBase server, params Uri[] addresses)
        {
            return new ServiceHost(this, typeof(SessionEndpoint), addresses);
        }

        /// <summary>
        /// Returns the service contract to use.
        /// </summary>
        protected override Type GetServiceContract()
        {
            return typeof(ISessionEndpoint);
        }

        /// <summary>
        /// Returns an instance of the endpoint to use.
        /// </summary>
        protected override EndpointBase GetEndpointInstance(ServerBase server)
        {
            return new SessionEndpoint(server);
        }

        /// <summary>
        /// Starts the server application.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected override void StartApplication(ApplicationConfiguration configuration)
        {
            base.StartApplication(configuration);

            lock (lock_)
            {
                try
                {
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - Start application {0}.", configuration.ApplicationName);

                    // create the datastore for the instance.
                    genericServerData_ = new GenericServerData(
                        ServerProperties,
                        configuration,
                        MessageContext,
                        new CertificateValidator(),
                        InstanceCertificate);

                    // create the manager responsible for providing localized string resources.                    
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateResourceManager.");
                    ResourceManager resourceManager = CreateResourceManager(genericServerData_, configuration);

                    // create the manager responsible for incoming requests.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateRequestManager.");
                    RequestManager requestManager = CreateRequestManager(genericServerData_, configuration);

                    // create the master node manager.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateMasterNodeManager.");
                    MasterNodeManager masterNodeManager = CreateMasterNodeManager(genericServerData_, configuration);

                    // add the node manager to the datastore. 
                    genericServerData_.SetNodeManager(masterNodeManager);

                    // put the node manager into a state that allows it to be used by other objects.
                    masterNodeManager.Startup();

                    // create the manager responsible for handling events.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateEventManager.");
                    EventManager eventManager = CreateEventManager(genericServerData_, configuration);

                    // creates the server object. 
                    genericServerData_.CreateServerObject(
                        eventManager,
                        resourceManager,
                        requestManager);

                    // do any additional processing now that the node manager is up and running.
                    OnNodeManagerStarted(genericServerData_);

                    // create the manager responsible for aggregates.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateAggregateManager.");
                    genericServerData_.AggregateManager = CreateAggregateManager(genericServerData_, configuration);

                    // start the session manager.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateSessionManager.");
                    SessionManager sessionManager = CreateSessionManager(genericServerData_, configuration);
                    sessionManager.Startup();

                    // start the subscription manager.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - CreateSubscriptionManager.");
                    SubscriptionManager subscriptionManager = CreateSubscriptionManager(genericServerData_, configuration);
                    subscriptionManager.Startup();

                    // add the session manager to the datastore. 
                    genericServerData_.SetSessionManager(sessionManager, subscriptionManager);

                    ServerError = null;

                    // setup registration information.
                    lock (registrationLock_)
                    {
                        maxRegistrationInterval_ = configuration.ServerConfiguration.MaxRegistrationInterval;

                        ApplicationDescription serverDescription = ServerDescription;

                        registrationInfo_ = new RegisteredServer {
                            ServerUri = serverDescription.ApplicationUri
                        };
                        registrationInfo_.ServerNames.Add(serverDescription.ApplicationName);
                        registrationInfo_.ProductUri = serverDescription.ProductUri;
                        registrationInfo_.ServerType = serverDescription.ApplicationType;
                        registrationInfo_.GatewayServerUri = null;
                        registrationInfo_.IsOnline = true;
                        registrationInfo_.SemaphoreFilePath = null;

                        // add all discovery urls.
                        var computerName = Utils.GetHostName();

                        for (var ii = 0; ii < BaseAddresses.Count; ii++)
                        {
                            var uri = new UriBuilder(BaseAddresses[ii].DiscoveryUrl);

                            if (String.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                            {
                                uri.Host = computerName;
                            }

                            registrationInfo_.DiscoveryUrls.Add(uri.ToString());
                        }

                        // build list of registration endpoints.
                        registrationEndpoints_ = new ConfiguredEndpointCollection(configuration);

                        EndpointDescription endpoint = configuration.ServerConfiguration.RegistrationEndpoint;

                        if (endpoint == null)
                        {
                            endpoint = new EndpointDescription {
                                EndpointUrl = Utils.Format(Utils.DiscoveryUrls[0], "localhost"),
                                SecurityLevel = ServerSecurityPolicy.CalculateSecurityLevel(MessageSecurityMode.SignAndEncrypt, SecurityPolicies.Basic256Sha256),
                                SecurityMode = MessageSecurityMode.SignAndEncrypt,
                                SecurityPolicyUri = SecurityPolicies.Basic256Sha256
                            };
                            endpoint.Server.ApplicationType = ApplicationType.DiscoveryServer;
                        }

                        _ = registrationEndpoints_.Add(endpoint);

                        minRegistrationInterval_ = 1000;
                        lastRegistrationInterval_ = minRegistrationInterval_;

                        // start registration timer.
                        if (registrationTimer_ != null)
                        {
                            registrationTimer_.Dispose();
                            registrationTimer_ = null;
                        }

                        if (maxRegistrationInterval_ > 0)
                        {
                            Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - Registration Timer started.");
                            registrationTimer_ = new Timer(OnRegisterServer, this, minRegistrationInterval_, Timeout.Infinite);
                        }
                    }

                    // set the server status as running.
                    SetServerState(ServerState.Running);

                    // all initialization is complete.
                    Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - Started.");
                    OnServerStarted(genericServerData_);

                    // monitor the configuration file.
                    if (!String.IsNullOrEmpty(configuration.SourceFilePath))
                    {
                        Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - Configuration watcher started.");
                        configurationWatcher_ = new ConfigurationWatcher(configuration);
                        configurationWatcher_.Changed += new EventHandler<ConfigurationWatcherEventArgs>(OnConfigurationChanged);
                    }

                    CertificateValidator.CertificateUpdate += OnCertificateUpdate;
                }
                catch (Exception e)
                {
                    var message = "Unexpected error starting application";
                    Utils.LogCritical(Utils.TraceMasks.StartStop, e, message);
                    genericServerData_ = null;
                    var error = ServiceResult.Create(e, StatusCodes.BadInternalError, message);
                    ServerError = error;
                    throw new ServiceResultException(error);
                }
            }
        }

        /// <summary>
        /// Called before the server stops
        /// </summary>
        protected override void OnServerStopping()
        {
            Utils.LogInfo(Utils.TraceMasks.StartStop, "Server - Stopping.");

            ShutDownDelay();

            // halt any outstanding timer.
            lock (registrationLock_)
            {
                if (registrationTimer_ != null)
                {
                    registrationTimer_.Dispose();
                    registrationTimer_ = null;
                }
            }

            // attempt graceful shutdown the server.
            try
            {
                if (maxRegistrationInterval_ > 0)
                {
                    // unregister from Discovery ServerData
                    registrationInfo_.IsOnline = false;
                    _ = RegisterWithDiscoveryServer();
                }

                lock (lock_)
                {
                    if (genericServerData_ != null)
                    {
                        genericServerData_.SubscriptionManager.Shutdown();
                        genericServerData_.SessionManager.Shutdown();
                        genericServerData_.NodeManager.Shutdown();
                    }
                }
            }
            catch (Exception e)
            {
                ServerError = new ServiceResult(e);
            }
            finally
            {
                // ensure that everything is cleaned up.
                if (genericServerData_ != null)
                {
                    Utils.SilentDispose(genericServerData_);
                    genericServerData_ = null;
                }
            }
        }

        /// <summary>
        /// Implements the server shutdown delay if session are connected.
        /// </summary>
        protected void ShutDownDelay()
        {
            try
            {
                // check for connected clients.
                IList<Session> currentessions = ServerData.SessionManager.GetSessions();

                if (currentessions.Count > 0)
                {
                    // provide some time for the connected clients to detect the shutdown state.
                    ServerData.Status.Value.ShutdownReason = new LocalizedText("en-US", "Application closed.");
                    ServerData.Status.Variable.ShutdownReason.Value = new LocalizedText("en-US", "Application closed.");
                    ServerData.Status.Value.State = ServerState.Shutdown;
                    ServerData.Status.Variable.State.Value = ServerState.Shutdown;
                    ServerData.Status.Variable.ClearChangeMasks(ServerData.DefaultSystemContext, true);

                    foreach (Session session in currentessions)
                    {
                        // raise close session audit event
                        ServerData.ReportAuditCloseSessionEvent(null, session, "Session/Terminated");
                    }

                    for (var timeTillShutdown = Configuration.ServerConfiguration.ShutdownDelay; timeTillShutdown > 0; timeTillShutdown--)
                    {
                        ServerData.Status.Value.SecondsTillShutdown = (uint)timeTillShutdown;
                        ServerData.Status.Variable.SecondsTillShutdown.Value = (uint)timeTillShutdown;
                        ServerData.Status.Variable.ClearChangeMasks(ServerData.DefaultSystemContext, true);

                        // exit if all client connections are closed.
                        var sessions = ServerData.SessionManager.GetSessions().Count;
                        if (sessions == 0)
                        {
                            break;
                        }

                        Utils.LogInfo(Utils.TraceMasks.StartStop, "{0} active sessions. Seconds until shutdown: {1}s", sessions, timeTillShutdown);

                        Thread.Sleep(1000);
                    }
                }
            }
            catch
            {
                // ignore error during shutdown procedure.
            }
        }

        /// <summary>
        /// Creates the request manager for the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// Returns an object that manages requests from within the server, return type is <seealso cref="RequestManager"/>.
        /// </returns>
        protected virtual RequestManager CreateRequestManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new RequestManager(server);
        }

        /// <summary>
        /// Creates the aggregate manager used by the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The manager.</returns>
        protected virtual AggregateManager CreateAggregateManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            var manager = new AggregateManager(server);

            manager.RegisterFactory(ObjectIds.AggregateFunction_Interpolative, BrowseNames.AggregateFunction_Interpolative, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Average, BrowseNames.AggregateFunction_Average, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_TimeAverage, BrowseNames.AggregateFunction_TimeAverage, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_TimeAverage2, BrowseNames.AggregateFunction_TimeAverage2, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Total, BrowseNames.AggregateFunction_Total, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Total2, BrowseNames.AggregateFunction_Total2, Aggregators.CreateStandardCalculator);

            manager.RegisterFactory(ObjectIds.AggregateFunction_Minimum, BrowseNames.AggregateFunction_Minimum, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Maximum, BrowseNames.AggregateFunction_Maximum, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_MinimumActualTime, BrowseNames.AggregateFunction_MinimumActualTime, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_MaximumActualTime, BrowseNames.AggregateFunction_MaximumActualTime, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Range, BrowseNames.AggregateFunction_Range, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Minimum2, BrowseNames.AggregateFunction_Minimum2, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Maximum2, BrowseNames.AggregateFunction_Maximum2, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_MinimumActualTime2, BrowseNames.AggregateFunction_MinimumActualTime2, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_MaximumActualTime2, BrowseNames.AggregateFunction_MaximumActualTime2, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Range2, BrowseNames.AggregateFunction_Range2, Aggregators.CreateStandardCalculator);

            manager.RegisterFactory(ObjectIds.AggregateFunction_Count, BrowseNames.AggregateFunction_Count, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_AnnotationCount, BrowseNames.AggregateFunction_AnnotationCount, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_DurationInStateZero, BrowseNames.AggregateFunction_DurationInStateZero, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_DurationInStateNonZero, BrowseNames.AggregateFunction_DurationInStateNonZero, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_NumberOfTransitions, BrowseNames.AggregateFunction_NumberOfTransitions, Aggregators.CreateStandardCalculator);

            manager.RegisterFactory(ObjectIds.AggregateFunction_Start, BrowseNames.AggregateFunction_Start, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_End, BrowseNames.AggregateFunction_End, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_Delta, BrowseNames.AggregateFunction_Delta, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_StartBound, BrowseNames.AggregateFunction_StartBound, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_EndBound, BrowseNames.AggregateFunction_EndBound, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_DeltaBounds, BrowseNames.AggregateFunction_DeltaBounds, Aggregators.CreateStandardCalculator);

            manager.RegisterFactory(ObjectIds.AggregateFunction_DurationGood, BrowseNames.AggregateFunction_DurationGood, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_DurationBad, BrowseNames.AggregateFunction_DurationBad, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_PercentGood, BrowseNames.AggregateFunction_PercentGood, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_PercentBad, BrowseNames.AggregateFunction_PercentBad, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_WorstQuality, BrowseNames.AggregateFunction_WorstQuality, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_WorstQuality2, BrowseNames.AggregateFunction_WorstQuality2, Aggregators.CreateStandardCalculator);

            manager.RegisterFactory(ObjectIds.AggregateFunction_StandardDeviationPopulation, BrowseNames.AggregateFunction_StandardDeviationPopulation, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_VariancePopulation, BrowseNames.AggregateFunction_VariancePopulation, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_StandardDeviationSample, BrowseNames.AggregateFunction_StandardDeviationSample, Aggregators.CreateStandardCalculator);
            manager.RegisterFactory(ObjectIds.AggregateFunction_VarianceSample, BrowseNames.AggregateFunction_VarianceSample, Aggregators.CreateStandardCalculator);

            return manager;
        }

        /// <summary>
        /// Creates the resource manager for the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Returns an object that manages access to localized resources, the return type is <seealso cref="ResourceManager"/>.</returns>
        protected virtual ResourceManager CreateResourceManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            var resourceManager = new ResourceManager(server, configuration);

            // load default text for all status codes.
            resourceManager.LoadDefaultText();

            return resourceManager;
        }

        /// <summary>
        /// Creates the master node manager for the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Returns the master node manager for the server, the return type is <seealso cref="MasterNodeManager"/>.</returns>
        protected virtual MasterNodeManager CreateMasterNodeManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            var nodeManagers = new List<IUaBaseNodeManager>();

            foreach (IUaNodeManagerFactory nodeManagerFactory in nodeManagerFactories_)
            {
                nodeManagers.Add(nodeManagerFactory.Create(server, configuration));
            }

            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        /// <summary>
        /// Creates the event manager for the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Returns an object that manages all events raised within the server, the return type is <seealso cref="EventManager"/>.</returns>
        protected virtual EventManager CreateEventManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new EventManager(server, (uint)configuration.ServerConfiguration.MaxEventQueueSize);
        }

        /// <summary>
        /// Creates the session manager for the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Returns a generic session manager object for a server, the return type is <seealso cref="SessionManager"/>.</returns>
        protected virtual SessionManager CreateSessionManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new SessionManager(server, configuration);
        }

        /// <summary>
        /// Creates the session manager for the server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Returns a generic session manager object for a server, the return type is <seealso cref="SubscriptionManager"/>.</returns>
        protected virtual SubscriptionManager CreateSubscriptionManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            return new SubscriptionManager(server, configuration);
        }

        /// <summary>
        /// Called after the node managers have been started.
        /// </summary>
        /// <param name="server">The server.</param>
        protected virtual void OnNodeManagerStarted(IUaServerData server)
        {
            // may be overridden by the subclass.
        }

        /// <summary>
        /// Called after the server has been started.
        /// </summary>
        /// <param name="server">The server.</param>
        protected virtual void OnServerStarted(IUaServerData server)
        {
            // may be overridden by the subclass.
        }

        /// <summary>
        /// The node manager factories that are used on startup of the server.
        /// </summary>
        public IEnumerable<IUaNodeManagerFactory> NodeManagerFactories => nodeManagerFactories_;

        /// <summary>
        /// Add a node manager factory which is used on server start
        /// to instantiate the node manager in the server.
        /// </summary>
        /// <param name="nodeManagerFactory">The node manager factory used to create the NodeManager.</param>
        public virtual void AddNodeManager(IUaNodeManagerFactory nodeManagerFactory)
        {
            nodeManagerFactories_.Add(nodeManagerFactory);
        }

        /// <summary>
        /// Remove a node manager factory from the list of node managers.
        /// Does not remove a NodeManager from a running server,
        /// only removes the factory before the server starts.
        /// </summary>
        /// <param name="nodeManagerFactory">The node manager factory to remove.</param>
        public virtual void RemoveNodeManager(IUaNodeManagerFactory nodeManagerFactory)
        {
            _ = nodeManagerFactories_.Remove(nodeManagerFactory);
        }
        #endregion

        #region Private Properties
        private OperationLimitsState OperationLimits => ServerData.ServerObject.ServerCapabilities.OperationLimits;
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private GenericServerData genericServerData_;
        private ConfigurationWatcher configurationWatcher_;

        private readonly object registrationLock_ = new object();
        private ConfiguredEndpointCollection registrationEndpoints_;
        private RegisteredServer registrationInfo_;
        private Timer registrationTimer_;
        private int minRegistrationInterval_;
        private int maxRegistrationInterval_;
        private int lastRegistrationInterval_;
        private int minNonceLength_;
        private bool useRegisterServer2_;
        private readonly List<IUaNodeManagerFactory> nodeManagerFactories_;
        #endregion
    }
}
