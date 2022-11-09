#region Copyright (c) 2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System.Collections.Generic;
using System.Linq;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.NodeManager;
using Technosoftware.UaStandardServer;
#endregion

namespace SampleCompany.SampleServer
{
    /// <summary>
    /// Implements a basic OPC UA Server.
    /// </summary>
    /// <remarks>
    /// Each server instance must have one instance of a StandardServer object which is
    /// responsible for reading the configuration file, creating the endpoints and dispatching
    /// incoming requests to the appropriate handler.
    /// 
    /// This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the SampleServerNodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public class SampleServer : UaStandardServer
    {
        #region Overridden Methods
        /// <summary>
        /// Creates the node managers for the server.
        /// </summary>
        /// <remarks>
        /// This method allows the sub-class create any additional node managers which it uses. The SDK
        /// always creates a CoreNodeManager which handles the built-in nodes defined by the specification.
        /// Any additional NodeManagers are expected to handle application specific nodes.
        /// </remarks>
        protected override MasterNodeManager CreateMasterNodeManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            Utils.LogInfo(Utils.TraceMasks.StartStop, "Creating the Sample Server Node Manager.");

            IList<IUaBaseNodeManager> nodeManagers = new List<IUaBaseNodeManager>();

            // create the custom node manager.
            nodeManagers.Add(new SampleServerNodeManager(server, configuration));

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            var properties = new ServerProperties
            {
                ManufacturerName = "Technosoftware GmbH",
                ProductName = "Technosoftware OPC UA Sample Server",
                ProductUri = "http://technosoftware.com/SampleServer/v1.04",
                SoftwareVersion = Utils.GetAssemblySoftwareVersion(),
                BuildNumber = Utils.GetAssemblyBuildNumber(),
                BuildDate = Utils.GetAssemblyTimestamp()
            };

            return properties;
        }

        /// <summary>
        /// Initializes the server before it starts up.
        /// </summary>
        /// <remarks>
        /// This method is called before any startup processing occurs. The sub-class may update the 
        /// configuration object or do any other application specific startup tasks.
        /// </remarks>
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            Utils.LogInfo(Utils.TraceMasks.StartStop, "The server is starting.");

            base.OnServerStarting(configuration);
        }

        /// <summary>
        /// Called after the server has been started.
        /// </summary>
        protected override void OnServerStarted(IUaServerData server)
        {
            base.OnServerStarted(server);

            try
            {
                lock (server.Status.Lock)
                {
                    // allow a faster sampling interval for CurrentTime node.
                    server.Status.Variable.CurrentTime.MinimumSamplingInterval = 250;
                }
            }
            catch
            {
                // ignored
            }
        }
        #endregion

        #region Private Fields
        #endregion
    }
}