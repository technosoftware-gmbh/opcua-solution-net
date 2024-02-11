#region Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2022-2024 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;

using Opc.Ua;

using Technosoftware.UaClient;
#endregion

namespace SampleCompany.SampleClient
{
    /// <summary>
    /// Sample Session calls based on the samples server node model.
    /// </summary>
    public class ClientFunctions
    {
        #region Constructors, Destructor, Initialization
        public ClientFunctions(TextWriter output)
        {
            output_ = output;
        }
        #endregion

        #region Public Sample Client Methods
        /// <summary>
        /// Read a list of nodes from Server
        /// </summary>
        public void ReadNodes(IUaSession session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                #region Read a node by calling the Read Service

                // build a list of nodes to be read
                var nodesToRead = new ReadValueIdCollection()
                {
                    // Value of ServerStatus
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus, AttributeId = Attributes.Value },
                    // BrowseName of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.BrowseName },
                    // Value of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.Value }
                };

                // Read the node attributes
                output_.WriteLine("Reading nodes...");

                // Call Read Service
                _ = session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out DataValueCollection resultsValues,
                    out DiagnosticInfoCollection diagnosticInfos);

                // Validate the results
                ClientBase.ValidateResponse(resultsValues, nodesToRead);

                // Display the results.
                foreach (DataValue result in resultsValues)
                {
                    output_.WriteLine("Read Value = {0} , StatusCode = {1}", result.Value, result.StatusCode);
                }
                #endregion

                #region Read the Value attribute of a node by calling the Session.ReadValue method
                // Read Server NamespaceArray
                output_.WriteLine("Reading Value of NamespaceArray node...");
                DataValue namespaceArray = session.ReadValue(Variables.Server_NamespaceArray);
                // Display the result
                output_.WriteLine($"NamespaceArray Value = {namespaceArray}");
                #endregion
            }
            catch (Exception ex)
            {
                // Log Error
                output_.WriteLine($"Read Nodes Error : {ex.Message}.");
            }
        }

        /// <summary>
        /// Write a list of nodes to the Server.
        /// </summary>
        public void WriteNodes(IUaSession session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Write the configured nodes
                var nodesToWrite = new WriteValueCollection();

                // Int32 Node - Objects\CTT\Scalar\Scalar_Static\Int32
                var intWriteVal = new WriteValue {
                    NodeId = new NodeId("ns=2;s=Scalar_Static_Int32"),
                    AttributeId = Attributes.Value,
                    Value = new DataValue {
                        Value = 100
                    }
                };
                nodesToWrite.Add(intWriteVal);

                // Float Node - Objects\CTT\Scalar\Scalar_Static\Float
                var floatWriteVal = new WriteValue {
                    NodeId = new NodeId("ns=2;s=Scalar_Static_Float"),
                    AttributeId = Attributes.Value,
                    Value = new DataValue {
                        Value = (float)100.5
                    }
                };
                nodesToWrite.Add(floatWriteVal);

                // String Node - Objects\CTT\Scalar\Scalar_Static\String
                var stringWriteVal = new WriteValue {
                    NodeId = new NodeId("ns=2;s=Scalar_Static_String"),
                    AttributeId = Attributes.Value,
                    Value = new DataValue {
                        Value = "String Test"
                    }
                };
                nodesToWrite.Add(stringWriteVal);

                // Write the node attributes
                output_.WriteLine("Writing nodes...");

                // Call Write Service
                _ = session.Write(null,
                                nodesToWrite,
                                out StatusCodeCollection results,
                                out DiagnosticInfoCollection diagnosticInfos);

                // Validate the response
                ClientBase.ValidateResponse(results, nodesToWrite);

                // Display the results.
                output_.WriteLine("Write Results :");

                foreach (StatusCode writeResult in results)
                {
                    output_.WriteLine("     {0}", writeResult);
                }
            }
            catch (Exception ex)
            {
                // Log Error
                output_.WriteLine($"Write Nodes Error : {ex.Message}.");
            }
        }

        /// <summary>
        /// Browse Server nodes
        /// </summary>
        public void Browse(IUaSession session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Create a Browser object
                var browser = new Browser(session) {
                    // Set browse parameters
                    BrowseDirection = BrowseDirection.Forward,
                    NodeClassMask = (int)NodeClass.Object | (int)NodeClass.Variable,
                    ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                    IncludeSubtypes = true
                };

                NodeId nodeToBrowse = ObjectIds.Server;

                // Call Browse service
                output_.WriteLine("Browsing {0} node...", nodeToBrowse);
                ReferenceDescriptionCollection browseResults = browser.Browse(nodeToBrowse);

                // Display the results
                output_.WriteLine("Browse returned {0} results:", browseResults.Count);

                foreach (ReferenceDescription result in browseResults)
                {
                    output_.WriteLine("     DisplayName = {0}, NodeClass = {1}", result.DisplayName.Text, result.NodeClass);
                }
            }
            catch (Exception ex)
            {
                // Log Error
                output_.WriteLine($"Browse Error : {ex.Message}.");
            }
        }

        /// <summary>
        /// Call UA method
        /// </summary>
        public void CallMethod(IUaSession session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Define the UA Method to call
                // Parent node - Objects\My Data\Methods
                // Method node - Objects\My Data\Methods\Hello
                var objectId = new NodeId("ns=2;s=Methods");
                var methodId = new NodeId("ns=2;s=Methods_Hello");

                // Define the method parameters
                // Input argument requires a Float and an UInt32 value
                var inputArguments = "from Call Method";
                IList<object> outputArguments = null;

                // Invoke Call service
                output_.WriteLine("Calling UA method for node {0} ...", methodId);
                outputArguments = session.Call(objectId, methodId, inputArguments);

                // Display results
                output_.WriteLine("Method call returned {0} output argument(s):", outputArguments.Count);

                foreach (var outputArgument in outputArguments)
                {
                    output_.WriteLine("     OutputValue = {0}", outputArgument.ToString());
                }
            }
            catch (Exception ex)
            {
                output_.WriteLine("Method call error: {0}", ex.Message);
            }
        }
        #endregion

        #region Server Status
        /// <summary>Read some values from the server status node.</summary>
        public bool ReadServerStatus(IUaSession session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return false;
            }

            try
            {
                // Build a list of nodes to be read
                var nodesToRead = new ReadValueIdCollection()
                {
                    // Value of ServerStatus
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus, AttributeId = Attributes.Value },
                    // BrowseName of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.BrowseName },
                    // Value of ServerStatus_StartTime
                    new ReadValueId() { NodeId = Variables.Server_ServerStatus_StartTime, AttributeId = Attributes.Value }
                };

                // Read the node attributes
                output_.WriteLine("Reading server status...");

                // Call Read Service
                _ = session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out DataValueCollection resultsValues,
                    out _);

                // Validate the results
                ClientBase.ValidateResponse(resultsValues, nodesToRead);

                // Display the results.
                foreach (DataValue result in resultsValues)
                {
                    output_.WriteLine("   Read Value = {0} , StatusCode = {1}", result.Value, result.StatusCode);
                }
                return true;
            }
            catch (Exception ex)
            {
                output_.WriteLine($"Read Nodes Error : {ex.Message}.");
                return false;
            }
        }
        #endregion

        #region Private Fieds
        private readonly TextWriter output_;
        #endregion
    }
}
