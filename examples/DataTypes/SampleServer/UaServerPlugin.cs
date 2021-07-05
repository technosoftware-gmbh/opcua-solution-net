#region Copyright (c) 2021 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2021 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.IO;

using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaServer;
#endregion

namespace SampleCompany.SampleServer
{

    /// <summary>
    /// OPC Server Configuration and IO Handling
    ///
    /// This C# based plugin for the OPC UA Server .NET shows a base 
    /// OPC UA server implementation. At startup items with several 
    /// data types and access rights are statically defined. 
    /// </summary>
    public class UaServerPlugin : IUaServerPlugin, IUaOptionalServerPlugin, IDisposable
    {
        #region Private Fields
        private IUaServer opcServer_;

        // Track whether Dispose has been called.
        private bool disposed_;
        private readonly object lockDisposable_ = new object();
        #endregion

        #region Constructors, Destructor, Initialization
        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructor in types derived from this class.
        ~UaServerPlugin()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed_)
            {
                lock (lockDisposable_)
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    // if (disposing)
                    // {
                    // }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    // If disposing is false,
                    // only the following code is executed.

                    // Disposing has been done.
                    disposed_ = true;
                }

            }
        }
        #endregion

        #region General Methods (not related to an OPC specification)
        //---------------------------------------------------------------------
        // OPC UA Server .NET Interface 
        // (Called by the generic server)
        //---------------------------------------------------------------------

        /// <summary>This method is the first method called from the generic server at the startup.</summary>
        /// <param name="args">String array with the command line parameters as they were specified when the server was being started.</param>
        /// <returns>A <see cref="StatusCode"/> code with the result of the operation. Returning an error code stops the further server execution.</returns>
        public StatusCode OnStartup(string[] args)
        {
            return StatusCodes.Good;
        }

        /// <summary>This method is the called after the application configuration was loaded.</summary>
        /// <param name="application">The application instance.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>
        ///     A <see cref="StatusCode" /> code with the result of the operation. Returning an error code stops the further
        ///     server execution.
        /// </returns>
        public StatusCode OnApplicationConfigurationLoaded(ApplicationInstance application, ApplicationConfiguration configuration)
        {
            return StatusCodes.Good;
        }

        /// <summary>
        /// This method is called from the generic server to get the license information. 
        /// </summary>
        /// <param name="serialNumber">Serial Number</param>
        /// <remarks>Returning empty strings activates the evaluation version of the OPC UA Server .NET. The evaluation allows the usage of the full product for 30 days.</remarks>
        public void OnGetLicenseInformation(out string serialNumber)
        {
            serialNumber = "";
        }

        /// <summary>
        /// Defines namespaces used by the application.
        /// </summary>
        /// <returns>Array of namespaces that are used by the application.</returns>
        public string[] OnGetNamespaceUris()
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnGetNamespaceUris(): Request the supported namespace Uris.");
            // set one namespace for the type model.
            var namespaceUrls = new string[1];
            namespaceUrls[0] = Model.Namespaces.SampleServer;
            return namespaceUrls;
        }

        /// <summary>This method is called after the node manager is initialized.</summary>
        /// <param name="opcServer">The generic server object. Used to call methods the generic server provides.</param>
        /// <param name="configuration">The application configuration</param>
        public void OnInitialized(IUaServer opcServer, ApplicationConfiguration configuration)
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnInitialized(): Server is initialized.");
            opcServer_ = opcServer;
        }

        /// <summary>
        /// This method is called from the generic server at the startup; when the first client connects or the service is started. All items supported by the server need to be defined by calling the methods provided by the <see cref="IUaServer">IUaServer</see> interface for each item.
        /// </summary>
        /// <param name="externalReferences">The externalReferences allows the generic server to link to the general nodes.</param>
        /// <returns>The root folder.</returns>
        public NodeState OnCreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            // Not called because the method CreateAddressSpace() is overwritten in the SampleServerNodeManager class
            return null;
        }

        /// <summary>
        /// This method is called from the generic server when the server was successfully started and is running. 
        /// </summary>
        /// <remarks>This method should only return if the server should be stopped.</remarks>
        /// <returns>A <see cref="StatusCode"/> code with the result of the operation. Returning from this method stops the further server execution.</returns>
        public StatusCode OnRunning()
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnRunning(): Server is running.");
            return StatusCodes.Good;
        }

        /// <summary>
        /// This method is called from the generic server when a Shutdown is executed.
        /// To ensure proper process shutdown, any communication channels should be closed and all threads terminated before this method returns.
        /// </summary>
        /// <param name="serverState">The current state of the server.</param>
        /// <param name="reason">The reason why the server shutdowns.</param>
        /// <param name="exception">The exception which caused an error. null if not an exception.</param>
        /// <returns>A <see cref="StatusCode"/> code with the result of the operation.</returns>
        public StatusCode OnShutdown(ServerState serverState, string reason, Exception exception)
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnShutdown(): Server is shutting down because of {0}.", reason);
            return StatusCodes.Good;
        }

        /// <summary>The OPC Server Properties of the current server instance.</summary>
        /// <returns>A <see cref="ServerProperties"/> object.</returns>
        public ServerProperties OnGetServerProperties()
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnGetServerProperties(): Request some standard information of the server}.");
            var properties = new ServerProperties
                                {
                                    ManufacturerName = "SampleCompany",
                                    ProductName = "SampleCompany OPC UA Sample Server",
                                    ProductUri = "http://samplecompany.com/SampleServer/v1.0",
                                    SoftwareVersion = GetAssemblySoftwareVersion(),
                                    BuildNumber = GetAssemblyBuildNumber(),
                                    BuildDate = GetAssemblyTimestamp()
                                };

            return properties;
        }
        #endregion

        #region Core Server Facet Methods
        //---------------------------------------------------------------------
        // OPC UA Server .NET Interface 
        // (Called by the generic server)
        //---------------------------------------------------------------------

        /// <summary>This method is called when a client executes a 'write' server call. The <see cref="BaseDataVariableState"/> includes all information required to identify the item
        /// as well as original (unmodified) value, timestamp and quality. Depending on the returned result code the cache is updated or not in the generic server after returning from this method.</summary>
        /// <param name="originalVariableState">The <see cref="BaseVariableState"/> of the variable including the original state of the variable. Can be used to check what changes happens</param>
        /// <param name="value">The value which should be written. The returned value is used for updating the cache depending on the returned result code.</param>
        /// <param name="statusCode">A <see cref="StatusCode"/> code which should be used as new status code for the value.</param>
        /// <param name="timestamp">The timestamp the value was written. The returned value is used for updating the cache depending on the returned result code.</param>
        /// <returns>A <see cref="StatusCode"/> code with the result of the operation.</returns>
        /// <remarks>
        /// 	<para>Rules for updating the cache:</para>
        /// 	<list type="number">
        /// 		<item>If the returned <see cref="StatusCode"/> is Bad (something like Bad...) the cache is not updated with timestamp and value.</item>
        /// 		<item>If the returned <see cref="StatusCode"/> is <see cref="StatusCodes.GoodCompletesAsynchronously">OpcStatusCodes.GoodCompletesAsynchronously</see> the cache is not updated with
        ///     timestamp and value. After the customization DLL has finished its operation it can use <see cref="IUaServer.WriteBaseVariable">WriteBaseVariable</see> to update the cache.</item>
        /// 		<item>In all other cases the cache is updated with timestamp and value.</item>
        /// 		<item>In all cases the status code is updated with the status code set in the 'statusCode' parameter.</item>
        /// 	</list>
        /// </remarks>
        public StatusCode OnWriteBaseVariable(BaseVariableState originalVariableState, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            // V1.3.0.0: To be compliant the provided timestamp must be used. So only if the provided timestamp is invalid set a correct one.
            if (timestamp == DateTime.MinValue)
            {
                timestamp = DateTime.UtcNow;
            }
            return StatusCodes.Good;
        }
        #endregion

        #region Optional Server Plugin methods
        public UaBaseServer OnGetServer()
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnGetServer(): Request the instance of the server.");
            return new SampleServer();
        }

        public UaBaseNodeManager OnGetNodeManager(IUaServer opcServer, IUaServerData uaServer, ApplicationConfiguration configuration, params string[] namespaceUris)
        {
            Utils.Trace(Utils.TraceMasks.Information, "OnGetNodeManager(): Request the instance of the node manager.");
            return new SampleServerNodeManager(opcServer, this, uaServer, configuration, namespaceUris);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns the linker timestamp for an assembly. 
        /// </summary>
        private static DateTime GetAssemblyTimestamp()
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            var bytes = new byte[2048];

            using (Stream stream = new FileStream(Assembly.GetCallingAssembly().Location, FileMode.Open, FileAccess.Read))
            {
                stream.Read(bytes, 0, bytes.Length);
            }

            // get the location of te PE header.
            var index = BitConverter.ToInt32(bytes, peHeaderOffset);

            // get the timestamp from the linker.
            var secondsSince1970 = BitConverter.ToInt32(bytes, index + linkerTimestampOffset);

            // convert to DateTime value.
            var timestamp = new DateTime(1970, 1, 1, 0, 0, 0);
            timestamp = timestamp.AddSeconds(secondsSince1970);
            return timestamp;
        }

        /// <summary>
        /// Returns the major/minor version number for an assembly formatted as a string.
        /// </summary>
        private static string GetAssemblySoftwareVersion()
        {
            var version = Assembly.GetCallingAssembly().GetName().Version;
            if (version != null)
            {
                return Format("{0}.{1}", version.Major, version.Minor);
            }
            return Format("1.0");
        }

        /// <summary>
        /// Returns the build/revision number for an assembly formatted as a string.
        /// </summary>
        private static string GetAssemblyBuildNumber()
        {
            var version = Assembly.GetCallingAssembly().GetName().Version;
            if (version != null)
            {
                return Format("{0}.{1}", version.Build, (version.MajorRevision << 16) + version.MinorRevision);
            }
            return Format("1.0");
        }

        /// <summary>
        /// Formats a message using the default locale.
        /// </summary>
        private static string Format(string text, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, text, args);
        }
        #endregion
    }
}
