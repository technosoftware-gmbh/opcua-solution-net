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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaStandardServer;
#endregion

namespace SampleCompany.NodeManagers
{
    /// <summary>
    /// Helpers to find node managers implemented in this library.
    /// </summary>

    public static class NodeManagerUtils
    {
        /// <summary>
        /// Add all available node manager factories to the server.
        /// </summary>
        public static void AddDefaultNodeManagers(UaStandardServer server)
        {
            foreach (var nodeManagerFactory in NodeManagerFactories)
            {
                server.AddNodeManager(nodeManagerFactory);
            }
        }

        /// <summary>
        /// The property with available node manager factories.
        /// </summary>
        public static ReadOnlyList<IUaNodeManagerFactory> NodeManagerFactories
        {
            get
            {
                if (nodeManagerFactories_ == null)
                {
                    nodeManagerFactories_ = GetNodeManagerFactories();
                }
                return new ReadOnlyList<IUaNodeManagerFactory>(nodeManagerFactories_);
            }
        }

        /// <summary>
        /// Helper to determine the INodeManagerFactory by reflection.
        /// </summary>
        private static IUaNodeManagerFactory IsINodeManagerFactoryType(Type type)
        {
            var nodeManagerTypeInfo = type.GetTypeInfo();
            if (nodeManagerTypeInfo.IsAbstract ||
                !typeof(IUaNodeManagerFactory).IsAssignableFrom(type))
            {
                return null;
            }
            return Activator.CreateInstance(type) as IUaNodeManagerFactory;
        }

        /// <summary>
        /// Enumerates all node manager factories.
        /// </summary>
        /// <returns></returns>
        private static IList<IUaNodeManagerFactory> GetNodeManagerFactories()
        {
            var assembly = typeof(NodeManagerUtils).Assembly;
            var nodeManagerFactories = assembly.GetExportedTypes().Select(type => IsINodeManagerFactoryType(type)).Where(type => type != null);
            return nodeManagerFactories.ToList();
        }

        private static IList<IUaNodeManagerFactory> nodeManagerFactories_;
    }
}
