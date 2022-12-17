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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Opc.Ua;

using Technosoftware.UaClient;
#endregion

namespace SampleCompany.ReferenceClient
{
    /// <summary>
    /// Sample Session calls based on the reference server node model.
    /// </summary>
    public class ClientFunctions
    {
        #region Constructors, Destructor, Initialization
        public ClientFunctions(TextWriter output, ManualResetEvent quitEvent, bool verbose = false, string csvFile = null)
        {
            output_ = output;
            quitEvent_ = quitEvent;
            verbose_ = verbose;
            csvFile_ = csvFile;
            eventTypeMappings_ = new Dictionary<NodeId, NodeId>();
        }
        #endregion

        #region Public Sample Methods
        /// <summary>
        /// Read a list of nodes from Server
        /// </summary>
        public void ReadNodes(Session session)
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
                ReadValueIdCollection nodesToRead = new ReadValueIdCollection()
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
                session.Read(
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
        public void WriteNodes(Session session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Write the configured nodes
                WriteValueCollection nodesToWrite = new WriteValueCollection();

                // Int32 Node - Objects\CTT\Scalar\Scalar_Static\Int32
                WriteValue intWriteVal = new WriteValue();
                intWriteVal.NodeId = new NodeId("ns=2;s=Scalar_Static_Int32");
                intWriteVal.AttributeId = Attributes.Value;
                intWriteVal.Value = new DataValue();
                intWriteVal.Value.Value = (int)100;
                nodesToWrite.Add(intWriteVal);

                // Float Node - Objects\CTT\Scalar\Scalar_Static\Float
                WriteValue floatWriteVal = new WriteValue();
                floatWriteVal.NodeId = new NodeId("ns=2;s=Scalar_Static_Float");
                floatWriteVal.AttributeId = Attributes.Value;
                floatWriteVal.Value = new DataValue();
                floatWriteVal.Value.Value = (float)100.5;
                nodesToWrite.Add(floatWriteVal);

                // String Node - Objects\CTT\Scalar\Scalar_Static\String
                WriteValue stringWriteVal = new WriteValue();
                stringWriteVal.NodeId = new NodeId("ns=2;s=Scalar_Static_String");
                stringWriteVal.AttributeId = Attributes.Value;
                stringWriteVal.Value = new DataValue();
                stringWriteVal.Value.Value = "String Test";
                nodesToWrite.Add(stringWriteVal);

                // Write the node attributes
                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos;
                output_.WriteLine("Writing nodes...");

                // Call Write Service
                session.Write(null,
                                nodesToWrite,
                                out results,
                                out diagnosticInfos);

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
        public void Browse(Session session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Create a Browser object
                Browser browser = new Browser(session);

                // Set browse parameters
                browser.BrowseDirection = BrowseDirection.Forward;
                browser.NodeClassMask = (int)NodeClass.Object | (int)NodeClass.Variable;
                browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences;

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
        public void CallMethod(Session session)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Define the UA Method to call
                // Parent node - Objects\CTT\Methods
                // Method node - Objects\CTT\Methods\Add
                NodeId objectId = new NodeId("ns=2;s=Methods");
                NodeId methodId = new NodeId("ns=2;s=Methods_Add");

                // Define the method parameters
                // Input argument requires a Float and an UInt32 value
                object[] inputArguments = new object[] { (float)10.5, (uint)10 };
                IList<object> outputArguments = null;

                // Invoke Call service
                output_.WriteLine("Calling UAMethod for node {0} ...", methodId);
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

        /// <summary>
        /// Create Subscription and MonitoredItems for DataChanges
        /// </summary>
        public void SubscribeToDataChanges(Session session, uint minLifeTime)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return;
            }

            try
            {
                // Create a subscription for receiving data change notifications

                // Define Subscription parameters
                Subscription subscription = new Subscription(session.DefaultSubscription)
                {
                    DisplayName = "Console ReferenceClient Subscription",
                    PublishingEnabled = true,
                    PublishingInterval = 1000,
                    LifetimeCount = 0,
                    MinLifetimeInterval = minLifeTime,
                };

                session.AddSubscription(subscription);

                // Create the subscription on Server side
                subscription.Create();
                output_.WriteLine("New Subscription created with SubscriptionId = {0}.", subscription.Id);

                // Create MonitoredItems for data changes (Reference Server)

                MonitoredItem intMonitoredItem = new MonitoredItem(subscription.DefaultItem);
                // Int32 Node - Objects\CTT\Scalar\Simulation\Int32
                intMonitoredItem.StartNodeId = new NodeId("ns=2;s=Scalar_Simulation_Int32");
                intMonitoredItem.AttributeId = Attributes.Value;
                intMonitoredItem.DisplayName = "Int32 Variable";
                intMonitoredItem.SamplingInterval = 1000;
                intMonitoredItem.QueueSize = 10;
                intMonitoredItem.DiscardOldest = true;
                intMonitoredItem.MonitoredItemNotificationEvent += OnMonitoredItemNotification;

                subscription.AddItem(intMonitoredItem);

                MonitoredItem floatMonitoredItem = new MonitoredItem(subscription.DefaultItem);
                // Float Node - Objects\CTT\Scalar\Simulation\Float
                floatMonitoredItem.StartNodeId = new NodeId("ns=2;s=Scalar_Simulation_Float");
                floatMonitoredItem.AttributeId = Attributes.Value;
                floatMonitoredItem.DisplayName = "Float Variable";
                floatMonitoredItem.SamplingInterval = 1000;
                floatMonitoredItem.QueueSize = 10;
                floatMonitoredItem.MonitoredItemNotificationEvent += OnMonitoredItemNotification;

                subscription.AddItem(floatMonitoredItem);

                MonitoredItem stringMonitoredItem = new MonitoredItem(subscription.DefaultItem);
                // String Node - Objects\CTT\Scalar\Simulation\String
                stringMonitoredItem.StartNodeId = new NodeId("ns=2;s=Scalar_Simulation_String");
                stringMonitoredItem.AttributeId = Attributes.Value;
                stringMonitoredItem.DisplayName = "String Variable";
                stringMonitoredItem.SamplingInterval = 1000;
                stringMonitoredItem.QueueSize = 10;
                stringMonitoredItem.MonitoredItemNotificationEvent += OnMonitoredItemNotification;

                subscription.AddItem(stringMonitoredItem);

                // Create the monitored items on Server side
                subscription.ApplyChanges();
                output_.WriteLine("MonitoredItems created for SubscriptionId = {0}.", subscription.Id);
            }
            catch (Exception ex)
            {
                output_.WriteLine("Subscribe error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Create Subscription and MonitoredItems for DataChanges
        /// </summary>
        public bool SubscribeToEventChanges(Session session, uint minLifeTime)
        {
            if (session == null || session.Connected == false)
            {
                output_.WriteLine("Session not connected!");
                return false;
            }

            currentSession_ = session;

            try
            {
                // Create a subscription for receiving event change notifications

                // Define Subscription parameters
                Subscription subscription = new Subscription(session.DefaultSubscription)
                {
                    DisplayName = "Console ReferenceClient Event Subscription",
                    PublishingEnabled = true,
                    PublishingInterval = 1000,
                    LifetimeCount = 0,
                    MinLifetimeInterval = minLifeTime,
                };

                session.AddSubscription(subscription);

                // Create the subscription on Server side
                subscription.Create();
                if (verbose_)
                {
                    output_.WriteLine("New Event Subscription created with SubscriptionId = {0}.", subscription.Id);
                }

                // Create MonitoredItems for event changes (Reference Server)

                // the filter to use.
                var filterDefinition = new EventFilterDefinition();

                // must specify the fields that the client is interested in.
                filterDefinition.SelectClauses = filterDefinition.ConstructSelectClauses(session, ObjectIds.Server, ObjectTypeIds.BaseEventType);

                // create a monitored item based on the current filter settings.
                MonitoredItem monitoredEventItem = filterDefinition.CreateMonitoredItem(session);

                // set up callback for notifications.
                monitoredEventItem.MonitoredItemNotificationEvent += OnMonitoredItemEventNotification;

                subscription.AddItem(monitoredEventItem);

                // Create the monitored items on Server side
                subscription.ApplyChanges();
                if (verbose_)
                {
                    output_.WriteLine("MonitoredItems created for SubscriptionId = {0}.", subscription.Id);
                }
                return true;
            }
            catch (Exception ex)
            {
                output_.WriteLine("Subscribe error: {0}", ex.Message);
                return false;
            }
        }
        #endregion

        #region Fetch with NodeCache
        /// <summary>
        /// Fetch all references and nodes with attributes except values from the server.
        /// </summary>
        /// <param name="uaClient">The UAClient with a session to use.</param>
        /// <param name="startingNode">The node from which the hierarchical nodes are fetched.</param>
        /// <param name="fetchTree">Iterate to fetch all nodes in the tree.</param>
        /// <param name="addRootNode">Adds the root node to the result.</param>
        /// <param name="filterUATypes">Filters nodes from namespace 0 from the result.</param>
        /// <returns>The list of nodes on the server.</returns>
        public IList<INode> FetchAllNodesNodeCache(
            MyUaClient uaClient,
            NodeId startingNode,
            bool fetchTree = false,
            bool addRootNode = false,
            bool filterUATypes = true)
        {
            var stopwatch = new Stopwatch();
            var nodeDictionary = new Dictionary<ExpandedNodeId, INode>();
            var references = new NodeIdCollection { ReferenceTypeIds.HierarchicalReferences };
            var nodesToBrowse = new ExpandedNodeIdCollection {
                    startingNode
                };

            // clear NodeCache to fetch all nodes from server
            uaClient.Session.NodeCache.Clear();

            // start
            stopwatch.Start();

            // fetch the reference types first, otherwise browse for e.g. hierarchical references with subtypes won't work
            var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            var namespaceUris = uaClient.Session.NamespaceUris;
            var referenceTypes = typeof(ReferenceTypeIds)
                     .GetFields(bindingFlags)
                     .Select(field => NodeId.ToExpandedNodeId((NodeId)field.GetValue(null), namespaceUris));
            uaClient.Session.FetchTypeTree(new ExpandedNodeIdCollection(referenceTypes));

            // add root node
            if (addRootNode)
            {
                var rootNode = uaClient.Session.NodeCache.Find(startingNode);
                nodeDictionary[rootNode.NodeId] = rootNode;
            }

            int searchDepth = 0;
            while (nodesToBrowse.Count > 0 && searchDepth < MaxSearchDepth)
            {
                if (quitEvent_.WaitOne(0))
                {
                    output_.WriteLine("Browse aborted.");
                    break;
                }

                searchDepth++;
                Utils.LogInfo("{0}: Find {1} references after {2}ms", searchDepth, nodesToBrowse.Count, stopwatch.ElapsedMilliseconds);
                var response = uaClient.Session.NodeCache.FindReferences(
                    nodesToBrowse,
                    references,
                    false,
                    true);

                var nextNodesToBrowse = new ExpandedNodeIdCollection();
                int duplicates = 0;
                foreach (var node in response)
                {
                    if (!nodeDictionary.ContainsKey(node.NodeId))
                    {
                        if (fetchTree)
                        {
                            nextNodesToBrowse.Add(node.NodeId);
                        }

                        if (filterUATypes)
                        {
                            if (node.NodeId.NamespaceIndex != 0)
                            {
                                // filter out default namespace
                                nodeDictionary[node.NodeId] = node;
                            }
                        }
                        else
                        {
                            nodeDictionary[node.NodeId] = node; ;
                        }
                    }
                    else
                    {
                        duplicates++;
                    }
                }
                if (duplicates > 0)
                {
                    Utils.LogInfo("Find References {0} duplicate nodes were ignored", duplicates);
                }
                nodesToBrowse = nextNodesToBrowse;
            }

            stopwatch.Stop();

            output_.WriteLine("FetchAllNodesNodeCache found {0} nodes in {1}ms", nodeDictionary.Count, stopwatch.ElapsedMilliseconds);

            var result = nodeDictionary.Values.ToList();
            result.Sort((x, y) => (x.NodeId.CompareTo(y.NodeId)));

            if (verbose_)
            {
                foreach (var node in result)
                {
                    output_.WriteLine("NodeId {0} {1} {2}", node.NodeId, node.NodeClass, node.BrowseName);
                }
            }

            return result;
        }
        #endregion

        #region BrowseAddressSpace sample
        /// <summary>
        /// Browse full address space.
        /// </summary>
        /// <param name="uaClient">The UAClient with a session to use.</param>
        /// <param name="startingNode">The node where the browse operation starts.</param>
        /// <param name="browseDescription">An optional BrowseDescription to use.</param>
        public ReferenceDescriptionCollection BrowseFullAddressSpace(
            MyUaClient uaClient,
            NodeId startingNode = null,
            BrowseDescription browseDescription = null)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Browse template
            const int kMaxReferencesPerNode = 1000;
            var browseTemplate = browseDescription ?? new BrowseDescription
            {
                NodeId = startingNode ?? ObjectIds.RootFolder,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ResultMask = (uint)BrowseResultMask.All
            };
            var browseDescriptionCollection = CreateBrowseDescriptionCollectionFromNodeId(
                new NodeIdCollection(new NodeId[] { startingNode ?? ObjectIds.RootFolder }),
                browseTemplate);

            // Browse
            var referenceDescriptions = new Dictionary<ExpandedNodeId, ReferenceDescription>();

            int searchDepth = 0;
            uint maxNodesPerBrowse = 0;
            while (browseDescriptionCollection.Any() && searchDepth < MaxSearchDepth)
            {
                searchDepth++;
                Utils.LogInfo("{0}: Browse {1} nodes after {2}ms",
                    searchDepth, browseDescriptionCollection.Count, stopWatch.ElapsedMilliseconds);

                BrowseResultCollection allBrowseResults = new BrowseResultCollection();
                bool repeatBrowse;
                BrowseResultCollection browseResultCollection = new BrowseResultCollection();
                DiagnosticInfoCollection diagnosticsInfoCollection;
                do
                {
                    if (quitEvent_.WaitOne(0))
                    {
                        output_.WriteLine("Browse aborted.");
                        break;
                    }

                    var browseCollection = (maxNodesPerBrowse == 0) ?
                        browseDescriptionCollection :
                        browseDescriptionCollection.Take((int)maxNodesPerBrowse).ToArray();
                    repeatBrowse = false;
                    try
                    {
                        _ = uaClient.Session.Browse(null, null,
                            kMaxReferencesPerNode, browseCollection,
                            out browseResultCollection, out diagnosticsInfoCollection);
                        ClientBase.ValidateResponse(browseResultCollection, browseCollection);
                        ClientBase.ValidateDiagnosticInfos(diagnosticsInfoCollection, browseCollection);

                        allBrowseResults.AddRange(browseResultCollection);
                    }
                    catch (ServiceResultException sre)
                    {
                        if (sre.StatusCode == StatusCodes.BadEncodingLimitsExceeded ||
                            sre.StatusCode == StatusCodes.BadResponseTooLarge)
                        {
                            // try to address by overriding operation limit
                            maxNodesPerBrowse = maxNodesPerBrowse == 0 ?
                                (uint)browseCollection.Count / 2 : maxNodesPerBrowse / 2;
                            repeatBrowse = true;
                        }
                        else
                        {
                            output_.WriteLine("Browse error: {0}", sre.Message);
                            throw;
                        }
                    }
                } while (repeatBrowse);

                if (maxNodesPerBrowse == 0)
                {
                    browseDescriptionCollection.Clear();
                }
                else
                {
                    browseDescriptionCollection = browseDescriptionCollection.Skip(browseResultCollection.Count).ToArray();
                }

                // Browse next
                var continuationPoints = PrepareBrowseNext(browseResultCollection);
                while (continuationPoints.Any())
                {
                    if (quitEvent_.WaitOne(0))
                    {
                        output_.WriteLine("Browse aborted.");
                    }

                    Utils.LogInfo("BrowseNext {0} continuation points.", continuationPoints.Count);
                    _ = uaClient.Session.BrowseNext(null, false, continuationPoints,
                        out var browseNextResultCollection, out diagnosticsInfoCollection);
                    ClientBase.ValidateResponse(browseNextResultCollection, continuationPoints);
                    ClientBase.ValidateDiagnosticInfos(diagnosticsInfoCollection, continuationPoints);
                    allBrowseResults.AddRange(browseNextResultCollection);
                    continuationPoints = PrepareBrowseNext(browseNextResultCollection);
                }

                // Build browse request for next level
                var browseTable = new NodeIdCollection();
                int duplicates = 0;
                foreach (var browseResult in allBrowseResults)
                {
                    foreach (var reference in browseResult.References)
                    {
                        if (!referenceDescriptions.ContainsKey(reference.NodeId))
                        {
                            referenceDescriptions[reference.NodeId] = reference;
                            browseTable.Add(ExpandedNodeId.ToNodeId(reference.NodeId, uaClient.Session.NamespaceUris));
                        }
                        else
                        {
                            duplicates++;
                        }
                    }
                }
                if (duplicates > 0)
                {
                    Utils.LogInfo("Browse Result {0} duplicate nodes were ignored.", duplicates);
                }
                browseDescriptionCollection.AddRange(CreateBrowseDescriptionCollectionFromNodeId(browseTable, browseTemplate));
            }

            stopWatch.Stop();

            var result = new ReferenceDescriptionCollection(referenceDescriptions.Values);
            result.Sort((x, y) => (x.NodeId.CompareTo(y.NodeId)));

            output_.WriteLine("BrowseFullAddressSpace found {0} references on server in {1}ms.",
                referenceDescriptions.Count, stopWatch.ElapsedMilliseconds);

            if (verbose_)
            {
                foreach (var reference in result)
                {
                    output_.WriteLine("NodeId {0} {1} {2}", reference.NodeId, reference.NodeClass, reference.BrowseName);
                }
            }

            return result;
        }
        #endregion

        #region Read Values and output as JSON sample
        /// <summary>
        /// Output all values as JSON.
        /// </summary>
        /// <param name="uaClient">The UAClient with a session to use.</param>
        /// <param name="variableIds">The variables to output.</param>
        public async Task ReadAllValuesAsync(
            MyUaClient uaClient,
            NodeIdCollection variableIds)
        {
            bool retrySingleRead = false;
            do
            {
                DataValueCollection values;
                IList<ServiceResult> errors;
                try
                {
                    if (retrySingleRead)
                    {
                        values = new DataValueCollection();
                        errors = new List<ServiceResult>();

                        foreach (var variableId in variableIds)
                        {
                            try
                            {
                                output_.WriteLine("Read {0}", variableId);
                                var value = await uaClient.Session.ReadValueAsync(variableId).ConfigureAwait(false);
                                values.Add(value);
                                errors.Add(value.StatusCode);

                                if (ServiceResult.IsNotBad(value.StatusCode))
                                {
                                    var valueString = ClientFunctions.FormatValueAsJson(uaClient.Session.MessageContext, variableId.ToString(), value, true);
                                    output_.WriteLine(valueString);
                                }
                                else
                                {
                                    output_.WriteLine("Error: {0}", value.StatusCode);
                                }
                            }
                            catch (ServiceResultException sre)
                            {
                                output_.WriteLine("Error: {0}", sre.Message);
                                values.Add(new DataValue(sre.StatusCode));
                                errors.Add(sre.Result);
                            }
                        }
                    }
                    else
                    {
                        (values, errors) = await uaClient.Session.ReadValuesAsync(variableIds).ConfigureAwait(false);

                        int ii = 0;
                        foreach (var value in values)
                        {
                            if (ServiceResult.IsNotBad(errors[ii]))
                            {
                                var valueString = ClientFunctions.FormatValueAsJson(uaClient.Session.MessageContext, variableIds[ii].ToString(), value, true);
                                output_.WriteLine(valueString);
                            }
                            else
                            {
                                output_.WriteLine("Error: {0}", value.StatusCode);
                            }
                            ii++;
                        }
                    }

                    retrySingleRead = false;
                }
                catch (ServiceResultException sre) when (sre.StatusCode == StatusCodes.BadEncodingLimitsExceeded)
                {
                    output_.WriteLine("Retry to read the values due to error:", sre.Message);
                    retrySingleRead = !retrySingleRead;
                }
            } while (retrySingleRead);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Create a prettified JSON string of a DataValue.
        /// </summary>
        /// <param name="name">The key of the Json value.</param>
        /// <param name="value">The DataValue.</param>
        /// <param name="jsonReversible">Use reversible encoding.</param>
        public static string FormatValueAsJson(
            IServiceMessageContext messageContext,
            string name,
            DataValue value,
            bool jsonReversible)
        {
            var jsonEncoder = new JsonEncoder(messageContext, jsonReversible);
            jsonEncoder.WriteDataValue(name, value);
            var textbuffer = jsonEncoder.CloseAndReturnText();
            // prettify
            using (var stringWriter = new StringWriter())
            {
                try
                {
                    using (var stringReader = new StringReader(textbuffer))
                    {
                        var jsonReader = new JsonTextReader(stringReader);
                        var jsonWriter = new JsonTextWriter(stringWriter)
                        {
                            Formatting = Formatting.Indented,
                            Culture = CultureInfo.InvariantCulture
                        };
                        jsonWriter.WriteToken(jsonReader);
                    }
                }
                catch (Exception ex)
                {

                    stringWriter.WriteLine("Failed to format the JSON output: {0}", ex.Message);
                    stringWriter.WriteLine(textbuffer);
                    throw;
                }
                return stringWriter.ToString();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handle DataChange notifications from Server
        /// </summary>
        private void OnMonitoredItemNotification(object sender, MonitoredItemNotificationEventArgs e)
        {
            try
            {
                MonitoredItem monitoredItem = (MonitoredItem)sender;
                // Log MonitoredItem Notification event
                MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
                output_.WriteLine("Notification: {0} \"{1}\" and Value = {2}.", notification.Message.SequenceNumber, monitoredItem.DisplayName, notification.Value);
            }
            catch (Exception ex)
            {
                output_.WriteLine("OnMonitoredItemNotification error: {0}", ex.Message);
            }
        }


        private void OnMonitoredItemEventNotification(object sender, MonitoredItemNotificationEventArgs e)
        {
            try
            {
                MonitoredItem monitoredItem = (MonitoredItem)sender;
                EventFieldList notification = e.NotificationValue as EventFieldList;

                if (notification == null)
                {
                    return;
                }

                // check the type of event.
                NodeId eventTypeId = EventUtils.FindEventType(monitoredItem, notification);

                // ignore unknown events.
                if (NodeId.IsNull(eventTypeId))
                {
                    return;
                }

                // check for refresh start.
                if (eventTypeId == ObjectTypeIds.RefreshStartEventType)
                {
                    return;
                }

                // check for refresh end.
                if (eventTypeId == ObjectTypeIds.RefreshEndEventType)
                {
                    return;
                }

                // construct the condition object.
                ConditionState condition = EventUtils.ConstructEvent(
                    currentSession_,
                    monitoredItem,
                    notification,
                    eventTypeMappings_) as ConditionState;

                if (condition == null)
                {
                    return;
                }

                // look up the condition type metadata in the local cache.
                INode type = currentSession_.NodeCache.Find(condition.TypeDefinitionId);

                var sourceName = "";
                var conditionName = "";
                var branchId = "";
                var typeText = "";
                var severity = "";
                var time = "";
                var enabledState = "";
                var message = "";
                var comment = "";

                // Source
                if (condition.SourceName != null)
                {
                    sourceName = Utils.Format("{0}", condition.SourceName.Value);
                }

                // Condition
                if (condition.ConditionName != null)
                {
                    conditionName = Utils.Format("{0}", condition.ConditionName.Value);
                }

                // Branch
                if (condition.BranchId != null && !NodeId.IsNull(condition.BranchId.Value))
                {
                    branchId = Utils.Format("{0}", condition.BranchId.Value);
                }

                // Type
                if (type != null)
                {
                    typeText = Utils.Format("{0}", type);
                }

                // Severity
                if (condition.Severity != null)
                {
                    severity = Utils.Format("{0}", (EventSeverity)condition.Severity.Value);
                }

                // Time
                if (condition.Time != null)
                {
                    time = Utils.Format("{0:HH:mm:ss.fff}", condition.Time.Value.ToLocalTime());
                }

                // State
                if (condition.EnabledState != null && condition.EnabledState.EffectiveDisplayName != null)
                {
                    enabledState = Utils.Format("{0}", condition.EnabledState.EffectiveDisplayName.Value);
                }

                // Message
                if (condition.Message != null)
                {
                    message = Utils.Format("{0}", condition.Message.Value);
                }

                // Comment
                if (condition.Comment != null)
                {
                    comment = Utils.Format("{0}", condition.Comment.Value);
                }
                output_.WriteLine("Event Notification {0}: SourceName {1}, ConditionName = {2}, BranchId = {3}, Type = {4}, Severity = {5}, Time = {6}, EnabledState = {7},  Message = {8},  Comment = {9}.", notification.Message.SequenceNumber, sourceName, conditionName, branchId, typeText, severity, time, enabledState, message, comment);
            }
            catch (Exception ex)
            {
                output_.WriteLine("OnMonitoredItemNotification error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Create a browse description from a node id collection.
        /// </summary>
        /// <param name="nodeIdCollection">The node id collection.</param>
        /// <param name="template">The template for the browse description for each node id.</param>
        private static BrowseDescriptionCollection CreateBrowseDescriptionCollectionFromNodeId(
            NodeIdCollection nodeIdCollection,
            BrowseDescription template)
        {
            var browseDescriptionCollection = new BrowseDescriptionCollection();
            foreach (var nodeId in nodeIdCollection)
            {
                BrowseDescription browseDescription = (BrowseDescription)template.MemberwiseClone();
                browseDescription.NodeId = nodeId;
                browseDescriptionCollection.Add(browseDescription);
            }
            return browseDescriptionCollection;
        }

        /// <summary>
        /// Create the continuation point collection from the browse result
        /// collection for the BrowseNext service.
        /// </summary>
        /// <param name="browseResultCollection">The browse result collection to use.</param>
        /// <returns>The collection of continuation points for the BrowseNext service.</returns>
        private static ByteStringCollection PrepareBrowseNext(BrowseResultCollection browseResultCollection)
        {
            var continuationPoints = new ByteStringCollection();
            foreach (var browseResult in browseResultCollection)
            {
                if (browseResult.ContinuationPoint != null)
                {
                    continuationPoints.Add(browseResult.ContinuationPoint);
                }
            }
            return continuationPoints;
        }
        #endregion

        #region Constants
        const int MaxSearchDepth = 128;
        #endregion

        #region Private Fieds
        private readonly TextWriter output_;
        private readonly ManualResetEvent quitEvent_;
        private readonly bool verbose_;
        private readonly string csvFile_;

        private Session currentSession_;
        private Dictionary<NodeId, NodeId> eventTypeMappings_;
        #endregion
    }
}
