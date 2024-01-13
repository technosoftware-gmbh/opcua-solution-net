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
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.Sessions
{
    /// <summary>
    /// A generic session manager object for a server.
    /// </summary>
    public class Session : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The Server object.</param>
        /// <param name="serverCertificate">The server certificate.</param>
        /// <param name="authenticationToken">The unique private identifier assigned to the Session.</param>
        /// <param name="clientNonce">The client nonce.</param>
        /// <param name="serverNonce">The server nonce.</param>
        /// <param name="sessionName">The name assigned to the Session.</param>
        /// <param name="clientDescription">Application description for the client application.</param>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="sessionTimeout">The session timeout.</param>
        /// <param name="maxResponseMessageSize">The maximum size of a response message</param>
        /// <param name="maxRequestAge">The max request age.</param>
        /// <param name="maxBrowseContinuationPoints">The maximum number of browse continuation points.</param>
        /// <param name="maxHistoryContinuationPoints">The maximum number of history continuation points.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public Session(
            UaServerOperationContext context,
            IUaServerData            server,
            X509Certificate2         serverCertificate,
            NodeId                   authenticationToken,
            byte[]                   clientNonce,
            byte[]                   serverNonce,
            string                   sessionName,
            ApplicationDescription   clientDescription,
            string                   endpointUrl,
            X509Certificate2         clientCertificate,
            double                   sessionTimeout,
            uint                     maxResponseMessageSize,
            double                   maxRequestAge,
            int                      maxBrowseContinuationPoints,
            int                      maxHistoryContinuationPoints)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (server == null) throw new ArgumentNullException(nameof(server));

            // verify that a secure channel was specified.
            if (context.ChannelContext == null)
            {
                throw new ServiceResultException(StatusCodes.BadSecureChannelIdInvalid);
            }

            server_                       = server;
            authenticationToken_          = authenticationToken;
            ClientNonce                  = clientNonce;
            serverNonce_                  = serverNonce;
            sessionName_                  = sessionName;
            serverCertificate_            = serverCertificate;
            ClientCertificate            = clientCertificate;
            secureChannelId_              = context.ChannelContext.SecureChannelId;
            maxResponseMessageSize_       = maxResponseMessageSize;
            maxRequestAge_                = maxRequestAge;
            maxBrowseContinuationPoints_  = maxBrowseContinuationPoints;
            maxHistoryContinuationPoints_ = maxHistoryContinuationPoints;
            endpoint_                     = context.ChannelContext.EndpointDescription;

            // use anonymous the default identity.
            Identity = new UserIdentity();

            // initialize diagnostics.
            SessionDiagnostics = new SessionDiagnosticsDataType
            {
                SessionId = null,
                SessionName = sessionName,
                ClientDescription = clientDescription,
                ServerUri = null,
                EndpointUrl = endpointUrl,
                LocaleIds = new StringCollection(),
                ActualSessionTimeout = sessionTimeout,
                ClientConnectionTime = DateTime.UtcNow,
                ClientLastContactTime = DateTime.UtcNow,
                CurrentSubscriptionsCount = 0,
                CurrentMonitoredItemsCount = 0,
                CurrentPublishRequestsInQueue = 0,
                TotalRequestCount = new ServiceCounterDataType(),
                UnauthorizedRequestCount = 0,
                ReadCount = new ServiceCounterDataType(),
                HistoryReadCount = new ServiceCounterDataType(),
                WriteCount = new ServiceCounterDataType(),
                HistoryUpdateCount = new ServiceCounterDataType(),
                CallCount = new ServiceCounterDataType(),
                CreateMonitoredItemsCount = new ServiceCounterDataType(),
                ModifyMonitoredItemsCount = new ServiceCounterDataType(),
                SetMonitoringModeCount = new ServiceCounterDataType(),
                SetTriggeringCount = new ServiceCounterDataType(),
                DeleteMonitoredItemsCount = new ServiceCounterDataType(),
                CreateSubscriptionCount = new ServiceCounterDataType(),
                ModifySubscriptionCount = new ServiceCounterDataType(),
                SetPublishingModeCount = new ServiceCounterDataType(),
                PublishCount = new ServiceCounterDataType(),
                RepublishCount = new ServiceCounterDataType(),
                TransferSubscriptionsCount = new ServiceCounterDataType(),
                DeleteSubscriptionsCount = new ServiceCounterDataType(),
                AddNodesCount = new ServiceCounterDataType(),
                AddReferencesCount = new ServiceCounterDataType(),
                DeleteNodesCount = new ServiceCounterDataType(),
                DeleteReferencesCount = new ServiceCounterDataType(),
                BrowseCount = new ServiceCounterDataType(),
                BrowseNextCount = new ServiceCounterDataType(),
                TranslateBrowsePathsToNodeIdsCount = new ServiceCounterDataType(),
                QueryFirstCount = new ServiceCounterDataType(),
                QueryNextCount = new ServiceCounterDataType(),
                RegisterNodesCount = new ServiceCounterDataType(),
                UnregisterNodesCount = new ServiceCounterDataType()
            };

            // initialize security diagnostics.
            securityDiagnostics_ = new SessionSecurityDiagnosticsDataType
            {
                SessionId = Id,
                ClientUserIdOfSession = Identity.DisplayName,
                AuthenticationMechanism = Identity.TokenType.ToString(),
                Encoding = context.ChannelContext.MessageEncoding.ToString(),
                ClientUserIdHistory = new StringCollection {Identity.DisplayName}
            };

            var description = context.ChannelContext.EndpointDescription;
            
            if (description != null)
            {
                securityDiagnostics_.TransportProtocol = new Uri(description.EndpointUrl).Scheme;
                securityDiagnostics_.SecurityMode      = endpoint_.SecurityMode;
                securityDiagnostics_.SecurityPolicyUri = endpoint_.SecurityPolicyUri;
            }

            if (clientCertificate != null)
            {
                securityDiagnostics_.ClientCertificate = clientCertificate.RawData;
            }

            var systemContext = server_.DefaultSystemContext.Copy(context);

            // create diagnostics object.
            Id = server.DiagnosticsNodeManager.CreateSessionDiagnostics(
                systemContext,
                SessionDiagnostics,
                OnUpdateDiagnostics,
                securityDiagnostics_,
                OnUpdateSecurityDiagnostics);

            TraceState("CREATED");
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                List<UaContinuationPoint> browseContinuationPoints;

                lock (lock_)
                {
                    browseContinuationPoints = continuationPoints_;
                    continuationPoints_ = null;
                }
                                
                if (browseContinuationPoints != null)
                {
                    foreach (var browseContinuationPoint in browseContinuationPoints)
                    {
                        Utils.SilentDispose(browseContinuationPoint);
                    }
                }

                List<HistoryContinuationPoint> historyContinuationPoints;

                lock (lock_)
                {
                    historyContinuationPoints = historyContinuationPoints_;
                    historyContinuationPoints_ = null;
                }
                
                if (historyContinuationPoints != null)
                {
                    foreach (var historyContinuationPoint in historyContinuationPoints)
                    {
                        Utils.SilentDispose(historyContinuationPoint.Value);
                    }
                }
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Gets the identifier assigned to the session when it was created.
        /// </summary>
        public NodeId Id { get; }

        /// <summary>
        /// The user identity provided by the client.
        /// </summary>
        public IUserIdentity Identity { get; private set; }

        /// <summary>
        /// The application defined mapping for user identity provided by the client.
        /// </summary>
        public IUserIdentity EffectiveIdentity { get; private set; }

        /// <summary>
        /// The user identity token provided by the client.
        /// </summary>
        public UserIdentityToken IdentityToken { get; private set; }

        /// <summary>
        /// A lock which must be acquired before accessing the diagnostics.
        /// </summary>
        public object DiagnosticsLock
        {
            get { return SessionDiagnostics; }
        }

        /// <summary>
        /// The diagnostics associated with the session.
        /// </summary>
        public SessionDiagnosticsDataType SessionDiagnostics { get; }

        /// <summary>
        /// Gets or sets the server certificate chain.
        /// </summary>
        /// <value>
        /// The server certificate chain.
        /// </value>
        public byte[] ServerCertificateChain { get; set; }

        /// <summary>
        /// The client Nonce associated with the session.
        /// </summary>
        public byte [] ClientNonce { get; }

        /// <summary>
        /// The application instance certificate associated with the client.
        /// </summary>
        public X509Certificate2 ClientCertificate { get; }

        /// <summary>
        /// The locales requested when the session was created.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] PreferredLocales { get; private set; }

        /// <summary>
        /// Whether the session timeout has elapsed since the last communication from the client.
        /// </summary>
        public bool HasExpired
        {
            get
            {
                lock (DiagnosticsLock)
                {
                    if (SessionDiagnostics.ClientLastContactTime.AddMilliseconds(SessionDiagnostics.ActualSessionTimeout) < DateTime.UtcNow)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Whether the session has been activated.
        /// </summary>
        public bool IsActivated { get; private set; }

        /// <summary>
        /// Returns the session's endpoint
        /// </summary>
        public EndpointDescription EndpointDescription
        {
            get
            {
                return endpoint_;
            }
        }

        /// <summary>
        /// Returns the session's SecureChannelId
        /// </summary>
        public string SecureChannelId
        {
            get
            {
                return secureChannelId_;
            }
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        public virtual void ValidateRequest(RequestHeader requestHeader, RequestType requestType)
        {
            if (requestHeader == null) throw new ArgumentNullException(nameof(requestHeader));

            lock (lock_)
            {
                // get the request context for the current thread.
                var context = SecureChannelContext.Current;

                if (context == null || !IsSecureChannelValid(context.SecureChannelId))
                {
                    UpdateDiagnosticCounters(requestType, true, true);
                    throw new ServiceResultException(StatusCodes.BadSecureChannelIdInvalid);
                }

                // verify that session has been activated.
                if (!IsActivated)
                {
                    if (requestType != RequestType.CloseSession)
                    {
                        UpdateDiagnosticCounters(requestType, true, true);
                        throw new ServiceResultException(StatusCodes.BadSessionNotActivated);
                    }
                }

                // request accepted.
                UpdateDiagnosticCounters(requestType, false, false);
            }
        }

        /// <summary>
        /// Validate the diagnostic info.
        /// </summary>
        public virtual void ValidateDiagnosticInfo(RequestHeader requestHeader)
        {
            const uint additionalInfoDiagnosticsMask = (uint)(DiagnosticsMasks.ServiceAdditionalInfo | DiagnosticsMasks.OperationAdditionalInfo);
            if ((requestHeader.ReturnDiagnostics & additionalInfoDiagnosticsMask) != 0)
            {
                var currentRoleIds = EffectiveIdentity?.GrantedRoleIds;
                if ((currentRoleIds?.Contains(ObjectIds.WellKnownRole_SecurityAdmin)) == true ||
                    (currentRoleIds?.Contains(ObjectIds.WellKnownRole_ConfigureAdmin)) == true)
                {
                    requestHeader.ReturnDiagnostics |= (uint)DiagnosticsMasks.UserPermissionAdditionalInfo;
                }
            }
        }

        /// <summary>
        /// Checks if the secure channel is currently valid.
        /// </summary>
        public virtual bool IsSecureChannelValid(string secureChannelId)
        {
            lock (lock_)
            {
                return (secureChannelId_ == secureChannelId);
            }
        }

        /// <summary>
        /// Updates the requested locale ids.
        /// </summary>
        /// <returns>true if the new locale ids are different from the old locale ids.</returns>
        public bool UpdateLocaleIds(StringCollection localeIds)
        {
            if (localeIds == null) throw new ArgumentNullException(nameof(localeIds));

            lock (lock_)
            {
                var ids = localeIds.ToArray();

                if (!Utils.IsEqual(ids, PreferredLocales))
                {
                    PreferredLocales = ids;

                    // update diagnostics.
                    lock (DiagnosticsLock)
                    {
                        SessionDiagnostics.LocaleIds = new StringCollection(localeIds);
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Activates the session and binds it to the current secure channel.
        /// </summary>
        public void ValidateBeforeActivate(
            UaServerOperationContext  context,
            SignatureData             clientSignature,
            List<SoftwareCertificate> clientSoftwareCertificates,
            ExtensionObject           userIdentityToken,
            SignatureData             userTokenSignature,
            StringCollection          localeIds,
            byte[]                    serverNonce,
            out UserIdentityToken     identityToken,
            out UserTokenPolicy       userTokenPolicy)
        {
            lock (lock_)
            {
                // verify that a secure channel was specified.
                if (context.ChannelContext == null)
                {
                    throw new ServiceResultException(StatusCodes.BadSecureChannelIdInvalid);
                }

                // verify that the same security policy has been used.
                var endpoint = context.ChannelContext.EndpointDescription;

                if (endpoint.SecurityPolicyUri != endpoint_.SecurityPolicyUri || endpoint.SecurityMode != endpoint_.SecurityMode)
                {
                    throw new ServiceResultException(StatusCodes.BadSecurityPolicyRejected);
                }

                // verify the client signature.
                if (ClientCertificate != null)
                {
                    if (endpoint_.SecurityPolicyUri != SecurityPolicies.None && clientSignature != null && clientSignature.Signature == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadApplicationSignatureInvalid);
                    }

                    var dataToSign = Utils.Append(serverCertificate_.RawData, serverNonce_);

                    if (!SecurityPolicies.Verify(ClientCertificate, endpoint_.SecurityPolicyUri, dataToSign, clientSignature))
                    {
                        // verify for certificate chain in endpoint.
                        // validate the signature with complete chain if the check with leaf certificate failed.
                        var serverCertificateChain = Utils.ParseCertificateChainBlob(endpoint_.ServerCertificate);

                        if (serverCertificateChain.Count > 1)
                        {
                            var serverCertificateChainList = new List<byte>();

                            for (var i = 0; i < serverCertificateChain.Count; i++)
                            {
                                serverCertificateChainList.AddRange(serverCertificateChain[i].RawData);
                            }

                            var serverCertificateChainData = serverCertificateChainList.ToArray();
                            dataToSign = Utils.Append(serverCertificateChainData, serverNonce_);

                            if (!SecurityPolicies.Verify(ClientCertificate, endpoint_.SecurityPolicyUri, dataToSign, clientSignature))
                            {
                                throw new ServiceResultException(StatusCodes.BadApplicationSignatureInvalid);
                            }
                        }
                        else
                        {
                            throw new ServiceResultException(StatusCodes.BadApplicationSignatureInvalid);
                        }
                    }
                }

                if (!IsActivated)
                {
                    // must active the session on the channel that was used to create it.
                    if (secureChannelId_ != context.ChannelContext.SecureChannelId)
                    {
                        throw new ServiceResultException(StatusCodes.BadSecureChannelIdInvalid);
                    }
                }
                else
                {
                    // cannot change the certificates after activation.
                    if (clientSoftwareCertificates != null && clientSoftwareCertificates.Count > 0)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidArgument);
                    }
                }

                // validate the user identity token.
                identityToken = ValidateUserIdentityToken(userIdentityToken, userTokenSignature, out userTokenPolicy);

                TraceState("VALIDATED");
            }
        }

        /// <summary>
        /// Activates the session and binds it to the current secure channel.
        /// </summary>
        public bool Activate(
            UaServerOperationContext          context,
            List<SoftwareCertificate> clientSoftwareCertificates,
            UserIdentityToken         identityToken,
            IUserIdentity             identity,
            IUserIdentity             effectiveIdentity,
            StringCollection          localeIds,
            byte[]                    serverNonce)
        {
            lock (lock_)
            {
                // update user identity.
                var changed = false;

                if (identityToken != null)
                {
                    if (UpdateUserIdentity(identityToken, identity, effectiveIdentity))
                    {
                        changed = true;
                    }
                }

                // update local ids.
                if (UpdateLocaleIds( localeIds ))
                {
                    changed = true;
                }

                if (!IsActivated)
                {
                    // toggle the activated flag.
                    IsActivated = true;

                    // save the software certificates.
                    softwareCertificates_ = clientSoftwareCertificates;

                    TraceState("FIRST ACTIVATION");
                }
                else
                {
                    // bind to the new secure channel.
                    secureChannelId_ = context.ChannelContext.SecureChannelId;      

                    TraceState("RE-ACTIVATION");  
                }

                // update server nonce.
                serverNonce_ = serverNonce;

                // build list of signed certificates for audit event.
                var signedSoftwareCertificates = new List<SignedSoftwareCertificate>();

                if (clientSoftwareCertificates != null)
                {
                    foreach (var softwareCertificate in clientSoftwareCertificates)
                    {
                        var item = new SignedSoftwareCertificate();
                        item.CertificateData = softwareCertificate.SignedCertificate.RawData;
                        signedSoftwareCertificates.Add(item);
                    }
                }

                // update the contact time.
                lock (DiagnosticsLock)
                {
                    SessionDiagnostics.ClientLastContactTime = DateTime.UtcNow;
                }

                // indicate whether the user context has changed.
                return changed;
            }
        }

        /// <summary>
        /// Closes a session and removes itself from the address space.
        /// </summary>
        public void Close()
        {
            TraceState("CLOSED");

            server_.DiagnosticsNodeManager.DeleteSessionDiagnostics(
                server_.DefaultSystemContext,
                Id);
        }

        /// <summary>
        /// Saves a continuation point for a session.
        /// </summary>
        /// <remarks>
        /// If the session has too many continuation points the oldest one is dropped.
        /// </remarks>
        public void SaveContinuationPoint(UaContinuationPoint continuationPoint)
        {
            if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));

            lock (lock_)
            {
                if (continuationPoints_ == null)
                {
                    continuationPoints_ = new List<UaContinuationPoint>();
                }

                // remove the first continuation point if too many points.
                while (continuationPoints_.Count > maxBrowseContinuationPoints_)
                {
                    var cp = continuationPoints_[0];
                    continuationPoints_.RemoveAt(0);
                    Utils.SilentDispose(cp);
                }

                // add to end of list.
                continuationPoints_.Add(continuationPoint);
            }
        }

        /// <summary>
        /// Restores a continuation point for a session.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for disposing the continuation point returned.
        /// </remarks>
        public UaContinuationPoint RestoreContinuationPoint(byte[] continuationPoint)
        {
            lock (lock_)
            {
                if (continuationPoints_ == null)
                {
                    return null;
                }

                if (continuationPoint == null || continuationPoint.Length != 16)
                {
                    return null;
                }

                var id = new Guid(continuationPoint);

                for (var ii = 0; ii < continuationPoints_.Count; ii++)
                {
                    if (continuationPoints_[ii].Id == id)
                    {
                        var cp = continuationPoints_[ii];
                        continuationPoints_.RemoveAt(ii);
                        return cp;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Saves a continuation point used for historical reads.
        /// </summary>
        /// <param name="id">The identifier for the continuation point.</param>
        /// <param name="continuationPoint">The continuation point.</param>
        /// <remarks>
        /// If the continuationPoint implements IDisposable it will be disposed when
        /// the Session is closed or discarded.
        /// </remarks>
        public void SaveHistoryContinuationPoint(Guid id, object continuationPoint)
        {
            if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));

            lock (lock_)
            {
                if (historyContinuationPoints_ == null)
                {
                    historyContinuationPoints_ = new List<HistoryContinuationPoint>();
                }

                // remove existing continuation point if space needed.
                while (historyContinuationPoints_.Count >= maxHistoryContinuationPoints_)
                {
                    var oldCP = historyContinuationPoints_[0];
                    historyContinuationPoints_.RemoveAt(0);
                    Utils.SilentDispose(oldCP.Value);
                }

                // create the cp.
                var cp = new HistoryContinuationPoint();

                cp.Id = id;
                cp.Value = continuationPoint;
                cp.Timestamp = DateTime.UtcNow;

                historyContinuationPoints_.Add(cp);
            }
        }

        /// <summary>
        /// Restores a previously saves history continuation point.
        /// </summary>
        /// <param name="continuationPoint">The identifier for the continuation point.</param>
        /// <returns>The save continuation point. null if not found.</returns>
        public object RestoreHistoryContinuationPoint(byte[] continuationPoint)
        {
            lock (lock_)
            {
                if (historyContinuationPoints_ == null)
                {
                    return null;
                }

                if (continuationPoint == null || continuationPoint.Length != 16)
                {
                    return null;
                }

                var id = new Guid(continuationPoint);

                for (var ii = 0; ii < historyContinuationPoints_.Count; ii++)
                {
                    var cp = historyContinuationPoints_[ii];

                    if (cp.Id == id)
                    {
                        historyContinuationPoints_.RemoveAt(ii);
                        return cp.Value;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Stores a continuation point used for historial reads.
        /// </summary>
        private class HistoryContinuationPoint
        {
            public Guid Id;
            public object Value;
            public DateTime Timestamp;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Dumps the current state of the session queue.
        /// </summary>
        internal void TraceState(string context)
        {
            UaServerUtils.EventLog.SessionState(context, Id.ToString(), sessionName_,
                secureChannelId_, Identity?.DisplayName ?? "(none)");
        }

        /// <summary>
        /// Returns a copy of the current diagnostics.
        /// </summary>
        private ServiceResult OnUpdateDiagnostics(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            lock (DiagnosticsLock)
            {
                value = Utils.Clone(SessionDiagnostics);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Returns a copy of the current security diagnostics.
        /// </summary>
        private ServiceResult OnUpdateSecurityDiagnostics(
            ISystemContext context,
            NodeState node,
            ref object value)
        {
            lock (DiagnosticsLock)
            {
                value = Utils.Clone(securityDiagnostics_);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Validates the identity token supplied by the client.
        /// </summary>
        private UserIdentityToken ValidateUserIdentityToken(
            ExtensionObject identityToken,
            SignatureData userTokenSignature,
            out UserTokenPolicy policy)
        {
            policy = null;

            // check for empty token.
            if (identityToken == null || identityToken.Body == null ||
                identityToken.Body.GetType() == typeof(Opc.Ua.AnonymousIdentityToken))
            {
                // check if an anonymous login is permitted.
                if (endpoint_.UserIdentityTokens != null && endpoint_.UserIdentityTokens.Count > 0)
                {
                    var found = false;

                    for (var ii = 0; ii < endpoint_.UserIdentityTokens.Count; ii++)
                    {
                        if (endpoint_.UserIdentityTokens[ii].TokenType == UserTokenType.Anonymous)
                        {
                            found = true;
                            policy = endpoint_.UserIdentityTokens[ii];
                            break;
                        }
                    }

                    if (!found)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied, "Anonymous user token policy not supported.");
                    }
                }

                // create an anonymous token to use for subsequent validation.
                var anonymousToken = new AnonymousIdentityToken();
                anonymousToken.PolicyId = policy.PolicyId;
                return anonymousToken;
            }

            UserIdentityToken token = null;
            // check for unrecognized token.
            if (!typeof(UserIdentityToken).IsInstanceOfType(identityToken.Body))
            {
                //handle the use case when the UserIdentityToken is binary encoded over xml message encoding
                if (identityToken.Encoding == ExtensionObjectEncoding.Binary && typeof(byte[]).IsInstanceOfType(identityToken.Body))
                {
                    var newToken = BaseVariableState.DecodeExtensionObject(null, typeof(UserIdentityToken), identityToken, false) as UserIdentityToken;
                    if (newToken == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied, "Invalid user identity token provided.");
                    }

                    policy = endpoint_.FindUserTokenPolicy(newToken.PolicyId);
                    if (policy == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied, "User token policy not supported.", "Technosoftware.UaServer.Session.ValidateUserIdentityToken");
                    }
                    switch (policy.TokenType)
                    {
                        case UserTokenType.Anonymous:
                            token = BaseVariableState.DecodeExtensionObject(null, typeof(AnonymousIdentityToken), identityToken, true) as AnonymousIdentityToken;
                            break;
                        case UserTokenType.UserName:
                            token = BaseVariableState.DecodeExtensionObject(null, typeof(UserNameIdentityToken), identityToken, true) as UserNameIdentityToken;
                            break;
                        case UserTokenType.Certificate:
                            token = BaseVariableState.DecodeExtensionObject(null, typeof(X509IdentityToken), identityToken, true) as X509IdentityToken;
                            break;
                        case UserTokenType.IssuedToken:
                            token = BaseVariableState.DecodeExtensionObject(null, typeof(IssuedIdentityToken), identityToken, true) as IssuedIdentityToken;
                            break;
                        default:
                            throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied, "Invalid user identity token provided.");
                    }
                }
                else
                {
                    throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied, "Invalid user identity token provided.");
                }
            }
            else
            {
                // get the token.
                token = (UserIdentityToken)identityToken.Body;
            }

            // find the user token policy.
            policy = endpoint_.FindUserTokenPolicy(token.PolicyId);

            if (policy == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid, "User token policy not supported.");
            }

            if (token is IssuedIdentityToken issuedToken)
            {
                if (policy.IssuedTokenType == Profiles.JwtUserToken)
                {
                    issuedToken.IssuedTokenType = IssuedTokenType.JWT; 
                }
            }

            // determine the security policy uri.
            var securityPolicyUri = policy.SecurityPolicyUri;

            if (string.IsNullOrEmpty(securityPolicyUri))
            {
                securityPolicyUri = endpoint_.SecurityPolicyUri;
            }

            if (ServerBase.RequireEncryption(endpoint_))
            {
                // decrypt the token.
                if (serverCertificate_ == null)
                {
                    serverCertificate_ = CertificateFactory.Create(endpoint_.ServerCertificate, true);

                    // check for valid certificate.
                    if (serverCertificate_ == null)
                    {
                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "ApplicationCertificate cannot be found.");
                    }
                }

                try
                {
                    token.Decrypt(serverCertificate_, serverNonce_, securityPolicyUri);
                }
                catch (Exception e)
                {
                    if (e is ServiceResultException)
                    {
                        throw;
                    }

                    throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid, e, "Could not decrypt identity token.");
                }

                // verify the signature.
                if (securityPolicyUri != SecurityPolicies.None)
                {
                    var dataToSign = Utils.Append(serverCertificate_.RawData, serverNonce_);

                    if (!token.Verify(dataToSign, userTokenSignature, securityPolicyUri))
                    {
                        // verify for certificate chain in endpoint.
                        // validate the signature with complete chain if the check with leaf certificate failed.
                        var serverCertificateChain = Utils.ParseCertificateChainBlob(endpoint_.ServerCertificate);

                        if (serverCertificateChain.Count > 1)
                        {
                            var serverCertificateChainList = new List<byte>();

                            foreach (var serverCertificate in serverCertificateChain)
                            {
                                serverCertificateChainList.AddRange(serverCertificate.RawData);
                            }

                            var serverCertificateChainData = serverCertificateChainList.ToArray();
                            dataToSign = Utils.Append(serverCertificateChainData, serverNonce_);

                            if (!token.Verify(dataToSign, userTokenSignature, securityPolicyUri))
                            {
                                throw new ServiceResultException(StatusCodes.BadIdentityTokenRejected, "Invalid user signature!");
                            }
                        }
                        else
                        {
                            throw new ServiceResultException(StatusCodes.BadIdentityTokenRejected, "Invalid user signature!");
                        }
                    }
                }
            }

            // validate user identity token.
            return token;
        }

        /// <summary>
        /// Updates the user identity.
        /// </summary>
        /// <returns>true if the new identity is different from the old identity.</returns>
        private bool UpdateUserIdentity(
            UserIdentityToken identityToken,
            IUserIdentity     identity,
            IUserIdentity     effectiveIdentity)
        {
            if (identityToken == null) throw new ArgumentNullException(nameof(identityToken));

            lock (lock_)
            {
                var changed = EffectiveIdentity == null && effectiveIdentity != null;

                if (EffectiveIdentity != null)
                {
                    changed = !EffectiveIdentity.Equals(effectiveIdentity);
                }

                // always save the new identity since it may have additional information that does not affect equality.
                IdentityToken = identityToken;
                Identity = identity;
                EffectiveIdentity = effectiveIdentity;

                // update diagnostics.
                lock (DiagnosticsLock)
                {
                    securityDiagnostics_.ClientUserIdOfSession   = identity.DisplayName;
                    securityDiagnostics_.AuthenticationMechanism = identity.TokenType.ToString();

                    securityDiagnostics_.ClientUserIdHistory.Add(identity.DisplayName);
                }

                return changed;
            }
        }

        /// <summary>
        /// Updates the diagnostic counters associated with the request.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void UpdateDiagnosticCounters(RequestType requestType, bool error, bool authorizationError)
        {
            lock (DiagnosticsLock)
            {
                if (!error)
                {
                    SessionDiagnostics.ClientLastContactTime = DateTime.UtcNow;
                }

                SessionDiagnostics.TotalRequestCount.TotalCount++;

                if (error)
                {
                    SessionDiagnostics.TotalRequestCount.ErrorCount++;

                    if (authorizationError)
                    {
                        SessionDiagnostics.UnauthorizedRequestCount++;
                    }
                }

                ServiceCounterDataType counter = null;

                switch (requestType)
                {
		            case RequestType.Read:                          { counter = SessionDiagnostics.ReadCount; break; }
		            case RequestType.HistoryRead:                   { counter = SessionDiagnostics.HistoryReadCount; break; }
		            case RequestType.Write:                         { counter = SessionDiagnostics.WriteCount; break; }
		            case RequestType.HistoryUpdate:                 { counter = SessionDiagnostics.HistoryUpdateCount; break; }
		            case RequestType.Call:                          { counter = SessionDiagnostics.CallCount; break; }
		            case RequestType.CreateMonitoredItems:          { counter = SessionDiagnostics.CreateMonitoredItemsCount; break; }
		            case RequestType.ModifyMonitoredItems:          { counter = SessionDiagnostics.ModifyMonitoredItemsCount; break; }
		            case RequestType.SetMonitoringMode:             { counter = SessionDiagnostics.SetMonitoringModeCount; break; }
		            case RequestType.SetTriggering:                 { counter = SessionDiagnostics.SetTriggeringCount; break; }
		            case RequestType.DeleteMonitoredItems:          { counter = SessionDiagnostics.DeleteMonitoredItemsCount; break; }
		            case RequestType.CreateSubscription:            { counter = SessionDiagnostics.CreateSubscriptionCount; break; }
		            case RequestType.ModifySubscription:            { counter = SessionDiagnostics.ModifySubscriptionCount; break; }
		            case RequestType.SetPublishingMode:             { counter = SessionDiagnostics.SetPublishingModeCount; break; }
		            case RequestType.Publish:                       { counter = SessionDiagnostics.PublishCount; break; }
		            case RequestType.Republish:                     { counter = SessionDiagnostics.RepublishCount; break; }
		            case RequestType.TransferSubscriptions:         { counter = SessionDiagnostics.TransferSubscriptionsCount; break; }
		            case RequestType.DeleteSubscriptions:           { counter = SessionDiagnostics.DeleteSubscriptionsCount; break; }
		            case RequestType.AddNodes:                      { counter = SessionDiagnostics.AddNodesCount; break; }
		            case RequestType.AddReferences:                 { counter = SessionDiagnostics.AddReferencesCount; break; }
		            case RequestType.DeleteNodes:                   { counter = SessionDiagnostics.DeleteNodesCount; break; }
		            case RequestType.DeleteReferences:              { counter = SessionDiagnostics.DeleteReferencesCount; break; }
		            case RequestType.Browse:                        { counter = SessionDiagnostics.BrowseCount; break; }
		            case RequestType.BrowseNext:                    { counter = SessionDiagnostics.BrowseNextCount; break; }
		            case RequestType.TranslateBrowsePathsToNodeIds: { counter = SessionDiagnostics.TranslateBrowsePathsToNodeIdsCount; break; }
		            case RequestType.QueryFirst:                    { counter = SessionDiagnostics.QueryFirstCount; break; }
		            case RequestType.QueryNext:                     { counter = SessionDiagnostics.QueryNextCount; break; }
		            case RequestType.RegisterNodes:                 { counter = SessionDiagnostics.RegisterNodesCount; break; }
		            case RequestType.UnregisterNodes:               { counter = SessionDiagnostics.UnregisterNodesCount; break; }
                }

                if (counter != null)
                {
                    counter.TotalCount += 1;

                    if (error)
                    {
                        counter.ErrorCount += 1;
                    }
                }
            }
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private NodeId authenticationToken_;
        private IUaServerData server_;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private List<SoftwareCertificate> softwareCertificates_;

        private byte[] serverNonce_;
        private string sessionName_;
        private string secureChannelId_;
        private EndpointDescription endpoint_;
        private X509Certificate2 serverCertificate_;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private uint maxResponseMessageSize_;
        private double maxRequestAge_;
        private int maxBrowseContinuationPoints_;
        private int maxHistoryContinuationPoints_;

        private SessionSecurityDiagnosticsDataType securityDiagnostics_;
        private List<UaContinuationPoint> continuationPoints_;
        private List<HistoryContinuationPoint> historyContinuationPoints_;
        #endregion
    }
}
