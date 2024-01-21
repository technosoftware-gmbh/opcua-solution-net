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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient.Tests
{

    /// <summary>
    /// A subclass of a session to override some implementations from CleintBase
    /// </summary> 
    public class HeaderUpdatingSession : Session
    {
        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="channel">The channel used to communicate with the server.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="endpoint">The endpoint use to initialize the channel.</param>
        public HeaderUpdatingSession(
            ISessionChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint)
        :
            this(channel as ITransportChannel, configuration, endpoint, null)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="ISession"/> class.
        /// </summary>
        /// <param name="channel">The channel used to communicate with the server.</param>
        /// <param name="configuration">The configuration for the client application.</param>
        /// <param name="endpoint">The endpoint used to initialize the channel.</param>
        /// <param name="clientCertificate">The certificate to use for the client.</param>
        /// <param name="availableEndpoints">The list of available endpoints returned by server in GetEndpoints() response.</param>
        /// <param name="discoveryProfileUris">The value of profileUris used in GetEndpoints() request.</param>
        /// <remarks>
        /// The application configuration is used to look up the certificate if none is provided.
        /// The clientCertificate must have the private key. This will require that the certificate
        /// be loaded from a certicate store. Converting a DER encoded blob to a X509Certificate2
        /// will not include a private key.
        /// The <i>availableEndpoints</i> and <i>discoveryProfileUris</i> parameters are used to validate
        /// that the list of EndpointDescriptions returned at GetEndpoints matches the list returned at CreateSession.
        /// </remarks>
        public HeaderUpdatingSession(
            ITransportChannel channel,
            ApplicationConfiguration configuration,
            ConfiguredEndpoint endpoint,
            X509Certificate2 clientCertificate,
            EndpointDescriptionCollection availableEndpoints = null,
            StringCollection discoveryProfileUris = null)
            : base(channel, configuration, endpoint, clientCertificate, availableEndpoints, discoveryProfileUris)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISession"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="template">The template session.</param>
        /// <param name="copyEventHandlers">if set to <c>true</c> the event handlers are copied.</param>
        public HeaderUpdatingSession(ITransportChannel channel, Session template, bool copyEventHandlers)
        :
            base(channel, template, copyEventHandlers)
        {
        }
        #endregion

        /// <summary>
        /// Holds the tracing context for propagation across system boundaries.
        /// </summary>
        public struct TraceContext
        {
            /// <summary>
            /// Gets the core trace identifiers like TraceId and SpanId.
            /// </summary>
            public ActivityContext ActivityContext { get; }

            /// <summary>
            /// Gets the user-defined data associated with the trace.
            /// </summary>
            public Dictionary<string, string> Baggage { get; }

            /// <summary>
            /// Constructs a new TraceContext.
            /// </summary>
            /// <param name="activityContext">Core trace identifiers.</param>
            /// <param name="baggage">User-defined data for the trace.</param>
            public TraceContext(ActivityContext activityContext, Dictionary<string, string> baggage)
            {
                ActivityContext = activityContext;
                Baggage = baggage;
            }
        }

        /// <summary>
        /// Populates AdditionalParameters with details from the TraceContext
        /// </summary>
        public static void InjectTraceIntoAdditionalParameters(TraceContext context, out AdditionalParametersType traceData)
        {
            traceData = new AdditionalParametersType();

            // Determine the trace flag based on the 'Recorded' status.
            string traceFlags = (context.ActivityContext.TraceFlags & ActivityTraceFlags.Recorded) != 0 ? "01" : "00";

            // Construct the traceparent header, adhering to the W3C Trace Context format.
            string traceparent = $"00-{context.ActivityContext.TraceId}-{context.ActivityContext.SpanId}-{traceFlags}";
            traceData.Parameters.Add(new Opc.Ua.KeyValuePair() { Key = "traceparent", Value = traceparent });

            // If baggage (tracestate) exists, include it as a single concatenated string.
            if (context.Baggage != null && context.Baggage.Count > 0)
            {
                var tracestate = string.Join(",", context.Baggage.Select(kv => $"{kv.Key}={kv.Value}"));
                traceData.Parameters.Add(new Opc.Ua.KeyValuePair() { Key = "tracestate", Value = tracestate });
            }
        }

        /// <summary>
        /// Extracts the trace and baggage details from the given dictionary
        /// </summary>
        public static TraceContext ExtractTraceContextFromParameters(AdditionalParametersType parameters)
        {
            if (parameters == null)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Parameters are null");
            }

            ActivityTraceId traceId = default;
            ActivitySpanId spanId = default;
            ActivityTraceFlags traceFlags = ActivityTraceFlags.None;
            Dictionary<string, string> baggage = new Dictionary<string, string>();

            foreach (var item in parameters.Parameters)
            {
                if (item.Key == "traceparent")
                {
                    var parts = item.Value.ToString().Split('-');
                    if (parts.Length != 4)
                    {
                        throw new ServiceResultException(StatusCodes.BadDecodingError, "Invalid traceparent format");
                    }
                    traceId = ActivityTraceId.CreateFromString(parts[1].AsSpan());
                    spanId = ActivitySpanId.CreateFromString(parts[2].AsSpan());
                    traceFlags = parts[3] == "01" ? ActivityTraceFlags.Recorded : ActivityTraceFlags.None;
                }
                else if (item.Key == "tracestate")
                {
                    var baggageItems = item.Value.ToString().Split(',');
                    foreach (var baggageItem in baggageItems)
                    {
                        var keyValue = baggageItem.Split('=');
                        if (keyValue.Length == 2)
                        {
                            baggage[keyValue[0].Trim()] = keyValue[1].Trim();
                        }
                    }
                }
            }

            if (traceId == default || spanId == default)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Failed to extract trace context");
            }

            var activityContext = new ActivityContext(traceId, spanId, traceFlags);
            return new TraceContext(activityContext, baggage);
        }

        ///<inheritdoc/>
        [Obsolete("Must override the version with useDefault parameter.")]
        protected override void UpdateRequestHeader(IServiceRequest request)
        {
            UpdateRequestHeader(request, request == null);
        }

        ///<inheritdoc/>
        protected override void UpdateRequestHeader(IServiceRequest request, bool useDefaults)
        {
            base.UpdateRequestHeader(request, useDefaults);

            if (Activity.Current != null)
            {
                InjectTraceIntoAdditionalParameters(new TraceContext(Activity.Current.Context, null), out AdditionalParametersType traceData);

                if (request.RequestHeader.AdditionalHeader == null)
                {
                    request.RequestHeader.AdditionalHeader = new ExtensionObject(traceData);
                }
                else if (request.RequestHeader.AdditionalHeader.Body is AdditionalParametersType existingParameters)
                {
                    // Merge the trace data into the existing parameters.
                    existingParameters.Parameters.AddRange(traceData.Parameters);
                }
            }
        }

        ///<inheritdoc/>
        protected override void UpdateRequestHeader(IServiceRequest request, bool useDefaults, string serviceName)
        {
            base.UpdateRequestHeader(request, useDefaults, serviceName);

            if (Activity.Current != null)
            {
                InjectTraceIntoAdditionalParameters(new TraceContext(Activity.Current.Context, null), out AdditionalParametersType traceData);

                if (request.RequestHeader.AdditionalHeader == null)
                {
                    request.RequestHeader.AdditionalHeader = new ExtensionObject(traceData);
                }
                else if (request.RequestHeader.AdditionalHeader.Body is AdditionalParametersType existingParameters)
                {
                    // Merge the trace data into the existing parameters.
                    existingParameters.Parameters.AddRange(traceData.Parameters);
                }
            }
        }
    }
}
