#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;

using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.UaServer;

#endregion

namespace Technosoftware.UaBaseServer
{
    /// <summary>
    ///     <para>OPC Server Plugin Interface</para>
    ///     <para>
    ///         This interface defines the server plugin interface. This interface must be implemented by the server plugin
    ///         assembly.
    ///     </para>
    /// </summary>
    public interface IUaServerPlugin
    {
        #region General Methods (not related to an OPC specification)

        //---------------------------------------------------------------------
        // OPC UA Server .NET Interface 
        // (Called by the generic server)
        //---------------------------------------------------------------------

        /// <summary>
        ///     This method is called from the generic server to get the license information.
        /// </summary>
        /// <param name="serialNumber">Serial Number</param>
        /// <remarks>
        ///     Returning an empty string for the serial number activates the evaluation version of the OPC UA Server .NET Standard. The evaluation allows
        ///     the usage of the full product for 30 days.
        /// </remarks>
        void OnGetLicenseInformation(out string serialNumber);

        /// <summary>This method is the first method called from the generic server at the startup.</summary>
        /// <param name="args">
        ///     String array with the command line parameters as they were specified when the server was being
        ///     started.
        /// </param>
        /// <remarks>
        ///     <para>The following command line parameters are handled by the generic server:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             /silent<br />
        ///             No output is done during startup of the server
        ///         </item>
        ///         <item>
        ///             /configfile<br />
        ///             Allows the definition of a specific application configuration file
        ///         </item>
        ///         <item>
        ///             /install<br />
        ///             Installs and configures the application according the configuration file, e.g. certificates are created and
        ///             firewall configured.
        ///         </item>
        ///         <item>
        ///             /uninstall<br />
        ///             Uninstalls the application by removing the changes made during installation.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <returns>
        ///     A <see cref="StatusCode" /> code with the result of the operation. Returning an error code stops the further
        ///     server execution.
        /// </returns>
        StatusCode OnStartup(string[] args);

        /// <summary>This method is the called after the application configuration was loaded.</summary>
        /// <param name="application">The application instance.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>
        ///     A <see cref="StatusCode" /> code with the result of the operation. Returning an error code stops the further
        ///     server execution.
        /// </returns>
        StatusCode OnApplicationConfigurationLoaded(ApplicationInstance application, ApplicationConfiguration configuration);

        /// <summary>Defines namespaces used by the application.</summary>
        /// <returns>Array of namespaces that are used by the application.</returns>
        string[] OnGetNamespaceUris();

        /// <summary>
        ///     This method is called after the node manager is initialized, after the call to OnGetNamespaceUris() and before
        ///     the call to OnCreateAddressSpace().
        /// </summary>
        /// <param name="opcServer">The generic server object. Used to call methods the generic server provides.</param>
        /// <param name="configuration">The application configuration.</param>
        void OnInitialized(IUaServer opcServer, ApplicationConfiguration configuration);

        /// <summary>
        ///     <para>
        ///         This method is called from the generic server at the startup; when the first client connects or the service is
        ///         started. All items supported by the server need to be defined by calling the methods provided by the
        ///         <see cref="IUaServer">IUaServer</see> interface for each item.
        ///     </para>
        /// </summary>
        /// <param name="externalReferences">The externalReferences allows the generic server to link to the general nodes.</param>
        /// <returns>The root folder.</returns>
        NodeState OnCreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences);

        /// <summary>
        ///     This method is called from the generic server when the server was successfully started and is running.
        /// </summary>
        /// <remarks>This method should only return if the server should be stopped.</remarks>
        /// <returns>
        ///     A <see cref="StatusCode" /> code with the result of the operation. Returning from this method stops the
        ///     further server execution.
        /// </returns>
        StatusCode OnRunning();

        /// <summary>
        ///     <para>This method is called from the generic server when a Shutdown is executed.</para>
        ///     <para>
        ///         To ensure proper process shutdown, any communication channels should be closed and all threads terminated
        ///         before this method returns.
        ///     </para>
        /// </summary>
        /// <param name="serverState">The current state of the server.</param>
        /// <param name="reason">The reason why the server shutdowns.</param>
        /// <param name="exception">The exception which caused an error. null if not an exception.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        StatusCode OnShutdown(ServerState serverState, string reason, Exception exception);

        /// <summary>
        ///     This method is called from the generic server when the OPC Server Properties of the current server instance
        ///     are requested by a client.
        /// </summary>
        /// <returns>An <see cref="ServerProperties">ServerProperties</see> object.</returns>
        ServerProperties OnGetServerProperties();

        #endregion

        #region Core Server Facet

        //---------------------------------------------------------------------
        // OPC UA Server .NET Standard Interface 
        // (Called by the generic server)
        //---------------------------------------------------------------------

        /// <summary>
        ///     <para>
        ///         This method is called when a client executes a 'write' server call. The <see cref="BaseDataVariableState" />
        ///         includes all information required to identify the item as
        ///         well as original (unmodified) value, timestamp and quality. Depending on the returned result code the cache is
        ///         updated or not in the generic server after
        ///         returning from this method.
        ///     </para>
        ///     <para>
        ///         <strong>Important:</strong> This method is only called for nodes created with
        ///     </para>
        ///     <list type="bullet">
        ///         <item>CreateBaseDataVariableState</item>
        ///         <item>CreateDataItemState</item>
        ///         <item>CreateAnalogItemState</item>
        ///         <item>CreateTwoStateDiscreteState</item>
        ///         <item>CreateMultiStateDiscreteState</item>
        ///     </list>
        ///     <para>
        ///         <font size="2" face="Consolas">
        ///             <font size="2" face="Consolas">
        ///                 <font size="2" face="Consolas">
        ///                     <font size="2" face="Consolas"></font>
        ///                 </font>
        ///             </font>
        ///         </font>
        ///     </para>
        /// </summary>
        /// <param name="originalVariableState">
        ///     The <see cref="BaseVariableState" /> of the variable including the original state
        ///     of the variable. Can be used to check what changes happens
        /// </param>
        /// <param name="value">
        ///     The value which should be written. The returned value is used for updating the cache depending on
        ///     the returned result code.
        /// </param>
        /// <param name="statusCode">A <see cref="StatusCode" /> code which should be used as new status code for the value.</param>
        /// <param name="timestamp">
        ///     The timestamp the value was written. The returned value is used for updating the cache
        ///     depending on the returned result code.
        /// </param>
        /// <remarks>
        ///     <para>The following rules apply for updating the cache base on the returned StatusCode:</para>
        ///     <list type="number">
        ///         <item>If the returned value is Bad (something like Bad...) the cache is not updated with timestamp and value.</item>
        ///         <item>
        ///             If the returned value is GoodCompletesAsynchronously the cache is not updated with timestamp and value.
        ///             After the customization DLL has finished its
        ///             operation it can use WriteBaseVariable to update the cache.
        ///         </item>
        ///         <item>In all other cases the cache is updated with timestamp and value.</item>
        ///         <item>In all cases the status code is updated with the status code set in the 'statusCode' parameter.</item>
        ///     </list>
        /// </remarks>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        StatusCode OnWriteBaseVariable(BaseVariableState originalVariableState, ref object value,
            ref StatusCode statusCode, ref DateTime timestamp);

        #endregion
    }
}