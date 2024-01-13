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

using Opc.Ua;
#endregion

namespace Technosoftware.UaServer.Aggregates
{
    /// <summary>
    /// An object that manages aggregate factories supported by the server.
    /// </summary>
    public class AggregateManager : IDisposable
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initilizes the manager.
        /// </summary>
        public AggregateManager(IUaServerData server)
        {
            server_ = server;
            factories_ = new Dictionary<NodeId,AggregatorFactory>();
            minimumProcessingInterval_ = 1000;
        }
        #endregion
        
        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {   
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {  
            if (disposing)
            {
                // TBD
            }            
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Checks if the aggregate is supported by the server.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate function.</param>
        /// <returns>True if the aggregate is supported.</returns>
        public bool IsSupported(NodeId aggregateId)
        {
            if (NodeId.IsNull(aggregateId))
            {
                return false;
            }

            lock (lock_)
            {
                return factories_.ContainsKey(aggregateId);
            }
        }

        /// <summary>
        /// The minimum processing interval for any aggregate calculation.
        /// </summary>
        public double MinimumProcessingInterval 
        {
            get
            {
                lock (lock_)
                {
                    return minimumProcessingInterval_;
                }
            }

            set
            {
                lock (lock_)
                {
                    minimumProcessingInterval_ = value;
                }
            }
        }

        /// <summary>
        /// Returns the default configuration for the specified variable id.
        /// </summary>
        /// <param name="variableId">The id of history data node.</param>
        /// <returns>The configuration.</returns>
        public AggregateConfiguration GetDefaultConfiguration(NodeId variableId)
        {
            lock (lock_)
            {
                if (defaultConfiguration_ == null)
                {
                    defaultConfiguration_ = new AggregateConfiguration();
                    defaultConfiguration_.PercentDataBad = 100;
                    defaultConfiguration_.PercentDataGood = 100;
                    defaultConfiguration_.TreatUncertainAsBad = false;
                    defaultConfiguration_.UseSlopedExtrapolation = false;
                    defaultConfiguration_.UseServerCapabilitiesDefaults = false;
                }

                return defaultConfiguration_;
            }
        }

        /// <summary>
        /// Sets the default aggregate configuration.
        /// </summary>
        /// <param name="configuration">The default aggregate configuration..</param>
        public void SetDefaultConfiguration(AggregateConfiguration configuration)
        {
            lock (lock_)
            {
                defaultConfiguration_ = configuration;
            }
        }

        /// <summary>
        /// Creates a new aggregate calculator.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate function.</param>
        /// <param name="startTime">When to start processing.</param>
        /// <param name="endTime">When to stop processing.</param>
        /// <param name="processingInterval">The processing interval.</param>
        /// <param name="stepped">Whether stepped interpolation should be used.</param>
        /// <param name="configuration">The configuration to use.</param>
        /// <returns></returns>
        public IUaAggregateCalculator CreateCalculator(
            NodeId aggregateId,
            DateTime startTime,
            DateTime endTime,
            double processingInterval,
            bool stepped,
            AggregateConfiguration configuration)
        {
            if (NodeId.IsNull(aggregateId))
            {
                return null;
            }

            AggregatorFactory factory = null;

            lock (lock_)
            {
                if (!factories_.TryGetValue(aggregateId, out factory))
                {
                    return null;
                }
            }

            if (configuration.UseServerCapabilitiesDefaults)
            {
                // ensure the configuration is initialized
                configuration = GetDefaultConfiguration(null); 
            }

            var calculator = factory(aggregateId, startTime, endTime, processingInterval, stepped, configuration);

            if (calculator == null)
            {
                return null;
            }

            return calculator;
        }

        /// <summary>
        /// Registers an aggregate factory.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate function.</param>
        /// <param name="aggregateName">The id of the aggregate name.</param>
        /// <param name="factory">The factory used to create calculators.</param>
        public void RegisterFactory(NodeId aggregateId, string aggregateName, AggregatorFactory factory)
        {
            lock (lock_)
            {
                factories_[aggregateId] = factory;
            }

            if (server_ != null)
            {
                server_.DiagnosticsNodeManager.AddAggregateFunction(aggregateId, aggregateName, true);
            }
        }

        /// <summary>
        /// Unregisters an aggregate factory.
        /// </summary>
        /// <param name="aggregateId">The id of the aggregate function.</param>
        public void RegisterFactory(NodeId aggregateId)
        {
            lock (lock_)
            {
                factories_.Remove(aggregateId);
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        private IUaServerData server_;
        private AggregateConfiguration defaultConfiguration_;
        private Dictionary<NodeId,AggregatorFactory> factories_;
        private double minimumProcessingInterval_;
        #endregion
    }
}
