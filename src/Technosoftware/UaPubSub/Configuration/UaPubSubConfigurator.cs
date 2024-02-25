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
#endregion

namespace Technosoftware.UaPubSub.Configuration
{
    /// <summary>
    /// Entity responsible to configure a PubSub Application
    /// 
    /// It has methods for adding/removing configuration objects to a root <see cref="PubSubConfigurationDataType"/> object.
    /// When the root <see cref="PubSubConfigurationDataType"/> object is modified there are various events raised to allow reaction to configuration changes.
    /// Each child object from parent <see cref="PubSubConfigurationDataType"/> object has a configurationId associated to it and it can be used to alter configuration. 
    /// The configurationId can be obtained using the <see cref="FindIdForObject(object)"/> method.
    /// </summary>
    public class UaPubSubConfigurator
    {
        #region Public Events
        /// <summary>
        /// Event that is triggered when a published data set is added to the configurator
        /// </summary>
        public event EventHandler<PublishedDataSetEventArgs> PublishedDataSetAddedEvent;

        /// <summary>
        /// Event that is triggered when a published data set is removed from the configurator
        /// </summary>
        public event EventHandler<PublishedDataSetEventArgs> PublishedDataSetRemovedEvent;

        /// <summary>
        /// Event that is triggered when an extension field is added to a published data set
        /// </summary>
        public event EventHandler<ExtensionFieldEventArgs> ExtensionFieldAddedEvent;

        /// <summary>
        /// Event that is triggered when an extension field is removed from a published data set
        /// </summary>
        public event EventHandler<ExtensionFieldEventArgs> ExtensionFieldRemovedEvent;

        /// <summary>
        /// Event that is triggered when a connection is added to the configurator
        /// </summary>
        public event EventHandler<ConnectionEventArgs> ConnectionAddedEvent;

        /// <summary>
        /// Event that is triggered when a connection is removed from the configurator
        /// </summary>
        public event EventHandler<ConnectionEventArgs> ConnectionRemovedEvent;

        /// <summary>
        /// Event that is triggered when a WriterGroup is added to a connection
        /// </summary>
        public event EventHandler<WriterGroupEventArgs> WriterGroupAddedEvent;

        /// <summary>
        /// Event that is triggered when a WriterGroup is removed from a connection
        /// </summary>
        public event EventHandler<WriterGroupEventArgs> WriterGroupRemovedEvent;

        /// <summary>
        /// Event that is triggered when a ReaderGroup is added to a connection
        /// </summary>
        public event EventHandler<ReaderGroupEventArgs> ReaderGroupAddedEvent;

        /// <summary>
        /// Event that is triggered when a ReaderGroup is removed from a connection
        /// </summary>
        public event EventHandler<ReaderGroupEventArgs> ReaderGroupRemovedEvent;

        /// <summary>
        /// Event that is triggered when a DataSetWriter is added to a WriterGroup
        /// </summary>
        public event EventHandler<DataSetWriterEventArgs> DataSetWriterAddedEvent;

        /// <summary>
        /// Event that is triggered when a DataSetWriter is removed from a WriterGroup
        /// </summary>
        public event EventHandler<DataSetWriterEventArgs> DataSetWriterRemovedEvent;

        /// <summary>
        /// Event that is triggered when a DataSetreader is added to a ReaderGroup
        /// </summary>
        public event EventHandler<DataSetReaderEventArgs> DataSetReaderAddedEvent;

        /// <summary>
        /// Event that is triggered when a DataSetreader is removed from a ReaderGroup
        /// </summary>
        public event EventHandler<DataSetReaderEventArgs> DataSetReaderRemovedEvent;

        /// <summary>
        /// Event raised when the state of a configuration object is changed
        /// </summary>
        public event EventHandler<PubSubStateChangedEventArgs> PubSubStateChangedEvent;
        #endregion

        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="UaPubSubConfigurator"/>.
        /// </summary>
        public UaPubSubConfigurator()
        {
            idsToObjects_ = new Dictionary<uint, object>();
            objectsToIds_ = new Dictionary<object, uint>();
            idsToPubSubState_ = new Dictionary<uint, PubSubState>();
            idsToParentId_ = new Dictionary<uint, uint>();

            pubSubConfiguration_ = new PubSubConfigurationDataType {
                Connections = new PubSubConnectionDataTypeCollection(),
                PublishedDataSets = new PublishedDataSetDataTypeCollection()
            };

            //remember configuration id 
            var id = nextId_++;
            objectsToIds_.Add(pubSubConfiguration_, id);
            idsToObjects_.Add(id, pubSubConfiguration_);
            idsToPubSubState_.Add(id, GetInitialPubSubState(pubSubConfiguration_));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get reference to <see cref="PubSubConfigurationDataType"/> instance that maintains the configuration for this <see cref="UaPubSubConfigurator"/>.
        /// </summary>
        public PubSubConfigurationDataType PubSubConfiguration => pubSubConfiguration_;
        #endregion

        #region Public Methods - Find

        /// <summary>
        /// Search a configured <see cref="PublishedDataSetDataType"/> with the specified name and return it
        /// </summary>
        /// <param name="name">Name of the object to be found.
        /// Returns null if name was not found.</param>
        /// <returns></returns>
        public PublishedDataSetDataType FindPublishedDataSetByName(string name)
        {
            foreach (PublishedDataSetDataType publishedDataSet in pubSubConfiguration_.PublishedDataSets)
            {
                if (name == publishedDataSet.Name)
                {
                    return publishedDataSet;
                }
            }
            return null;
        }


        /// <summary>
        /// Search objects in current configuration and return them
        /// </summary>
        /// <param name="id">Id of the object to be found.
        /// Returns null if id was not found.</param>
        /// <returns></returns>
        public object FindObjectById(uint id)
        {
            return idsToObjects_.TryGetValue(id, out var objectById) ? objectById : null;
        }

        /// <summary>
        /// Search id for specified configuration object.
        /// </summary>
        /// <param name="configurationObject">The object whose id is searched.</param>
        /// <returns>Returns <see cref="InvalidId"/> if object was not found.</returns>
        public uint FindIdForObject(object configurationObject)
        {
            return objectsToIds_.TryGetValue(configurationObject, out var id) ? id : InvalidId;
        }

        /// <summary>
        /// Search <see cref="PubSubState"/> for specified configuration object.
        /// </summary>
        /// <param name="configurationObject">The object whose <see cref="PubSubState"/> is searched.</param>
        /// <returns>Returns <see cref="PubSubState"/> if the object.</returns>
        public PubSubState FindStateForObject(object configurationObject)
        {
            var id = FindIdForObject(configurationObject);
            return idsToPubSubState_.TryGetValue(id, out PubSubState pubSubState) ? pubSubState : PubSubState.Error;
        }

        /// <summary>
        /// Search <see cref="PubSubState"/> for specified configuration object.
        /// </summary>
        /// <param name="id">The id  of the object which <see cref="PubSubState"/> is searched.</param>
        /// <returns>Returns <see cref="PubSubState"/> if the object.</returns>
        public PubSubState FindStateForId(uint id)
        {
            return idsToPubSubState_.TryGetValue(id, out PubSubState pubsubState) ? pubsubState : PubSubState.Error;
        }
        /// <summary>
        /// Find the parent configuration object for a configuration object
        /// </summary>
        /// <param name="configurationObject"></param>
        /// <returns></returns>
        public object FindParentForObject(object configurationObject)
        {
            var id = FindIdForObject(configurationObject);
            return id != InvalidId && idsToParentId_.TryGetValue(id, out var parentId) ? FindObjectById(parentId) : null;
        }

        /// <summary>
        /// Find children ids for specified object
        /// </summary>
        /// <param name="configurationObject"></param>
        /// <returns></returns>
        public List<uint> FindChildrenIdsForObject(object configurationObject)
        {
            var parentId = FindIdForObject(configurationObject);

            var childrenIds = new List<uint>();
            if (parentId != InvalidId && idsToParentId_.ContainsValue(parentId))
            {
                foreach (var key in idsToParentId_.Keys)
                {
                    if (idsToParentId_[key] == parentId)
                    {
                        childrenIds.Add(key);
                    }
                }
            }
            return childrenIds;
        }
        #endregion

        #region Public Methods - LoadConfiguration
        /// <summary>
        /// Load the specified configuration 
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="replaceExisting"> flag that indicates if current configuration is overwritten</param>
        public void LoadConfiguration(string configFilePath, bool replaceExisting = true)
        {
            // validate input argument 
            if (configFilePath == null)
            {
                throw new ArgumentException(nameof(configFilePath));
            }
            if (!File.Exists(configFilePath))
            {
                throw new ArgumentException("The specified file {0} does not exist", configFilePath);
            }
            PubSubConfigurationDataType pubSubConfiguration = UaPubSubConfigurationHelper.LoadConfiguration(configFilePath);

            LoadConfiguration(pubSubConfiguration, replaceExisting);
        }

        /// <summary>
        /// Load the specified configuration 
        /// </summary>
        /// <param name="pubSubConfiguration"></param>
        /// <param name="replaceExisting"> flag that indicates if current configuration is overwritten</param>
        public void LoadConfiguration(PubSubConfigurationDataType pubSubConfiguration, bool replaceExisting = true)
        {
            lock (lock_)
            {
                if (replaceExisting)
                {
                    //remove previous configured published data sets
                    if (pubSubConfiguration_ != null && pubSubConfiguration_.PublishedDataSets.Count > 0)
                    {
                        foreach (PublishedDataSetDataType publishedDataSet in pubSubConfiguration.PublishedDataSets)
                        {
                            _ = RemovePublishedDataSet(publishedDataSet);
                        }
                    }

                    //remove previous configured connections
                    if (pubSubConfiguration_ != null && pubSubConfiguration_.Connections.Count > 0)
                    {
                        foreach (PubSubConnectionDataType connection in pubSubConfiguration_.Connections.ToArray())
                        {
                            _ = RemoveConnection(connection);
                        }
                    }

                    pubSubConfiguration_?.Connections.Clear();
                    pubSubConfiguration_?.PublishedDataSets.Clear();
                }

                //first load Published DataSet information
                foreach (PublishedDataSetDataType publishedDataSet in pubSubConfiguration.PublishedDataSets)
                {
                    _ = AddPublishedDataSet(publishedDataSet);
                }

                foreach (PubSubConnectionDataType pubSubConnectionDataType in pubSubConfiguration.Connections)
                {
                    // handle empty names 
                    if (string.IsNullOrEmpty(pubSubConnectionDataType.Name))
                    {
                        //set default name 
                        pubSubConnectionDataType.Name = "Connection_" + (nextId_ + 1);
                    }
                    _ = AddConnection(pubSubConnectionDataType);
                }
            }
        }
        #endregion

        #region Public Methods - PublishedDataSet
        /// <summary>
        /// Add a published data set to current configuration.
        /// </summary>
        /// <param name="publishedDataSetDataType">The <see cref="PublishedDataSetDataType"/> object to be added to configuration.</param>
        /// <returns></returns>
        public StatusCode AddPublishedDataSet(PublishedDataSetDataType publishedDataSetDataType)
        {
            if (objectsToIds_.ContainsKey(publishedDataSetDataType))
            {
                throw new ArgumentException("This PublishedDataSetDataType instance is already added to the configuration.");
            }
            try
            {
                lock (lock_)
                {
                    //validate duplicate name 
                    var duplicateName = false;
                    foreach (PublishedDataSetDataType publishedDataSet in pubSubConfiguration_.PublishedDataSets)
                    {
                        if (publishedDataSetDataType.Name == publishedDataSet.Name)
                        {
                            duplicateName = true;
                            break;
                        }
                    }
                    if (duplicateName)
                    {
                        Utils.Trace(Utils.TraceMasks.Error, "Attempted to add PublishedDataSetDataType with duplicate name = {0}", publishedDataSetDataType.Name);
                        return StatusCodes.BadBrowseNameDuplicated;
                    }

                    var newPublishedDataSetId = nextId_++;
                    //remember connection 
                    idsToObjects_.Add(newPublishedDataSetId, publishedDataSetDataType);
                    objectsToIds_.Add(publishedDataSetDataType, newPublishedDataSetId);
                    pubSubConfiguration_.PublishedDataSets.Add(publishedDataSetDataType);

                    // raise PublishedDataSetAdded event
                    PublishedDataSetAddedEvent?.Invoke(this, new PublishedDataSetEventArgs() { PublishedDataSetId = newPublishedDataSetId, PublishedDataSetDataType = publishedDataSetDataType });

                    if (publishedDataSetDataType.ExtensionFields == null)
                    {
                        publishedDataSetDataType.ExtensionFields = new KeyValuePairCollection();
                    }
                    var extensionFields = new KeyValuePairCollection(publishedDataSetDataType.ExtensionFields);
                    publishedDataSetDataType.ExtensionFields.Clear();
                    foreach (Opc.Ua.KeyValuePair extensionField in extensionFields)
                    {
                        _ = AddExtensionField(newPublishedDataSetId, extensionField);
                    }
                    return StatusCodes.Good;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.AddPublishedDataSet: Exception");
            }

            //todo implement state validation
            return StatusCodes.Bad;
        }

        /// <summary>
        /// Removes a published data set from current configuration.
        /// </summary>
        /// <param name="publishedDataSetId">Id of the published data set to be removed.</param>
        /// <returns> 
        /// - <see cref="StatusCodes.Good"/> if operation is successful, 
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> otherwise.
        /// </returns>
        public StatusCode RemovePublishedDataSet(uint publishedDataSetId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(publishedDataSetId) is PublishedDataSetDataType publishedDataSetDataType))
                {
                    // Unexpected exception 
                    Utils.Trace(Utils.TraceMasks.Information, "Current configuration does not contain PublishedDataSetDataType with ConfigId = {0}", publishedDataSetId);
                    return StatusCodes.Good;
                }
                return RemovePublishedDataSet(publishedDataSetDataType);
            }
        }

        /// <summary>
        /// Removes a published data set from current configuration.
        /// </summary>
        /// <param name="publishedDataSetDataType">The published data set to be removed.</param>
        /// <returns> 
        /// - <see cref="StatusCodes.Good"/> if operation is successful, 
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> otherwise.
        /// </returns>
        public StatusCode RemovePublishedDataSet(PublishedDataSetDataType publishedDataSetDataType)
        {
            try
            {
                lock (lock_)
                {
                    var publishedDataSetId = FindIdForObject(publishedDataSetDataType);
                    if (publishedDataSetDataType != null && publishedDataSetId != InvalidId)
                    {
                        /*A successful removal of the PublishedDataSetType Object removes all associated DataSetWriter Objects. 
                         * Before the Objects are removed, their state is changed to Disabled_0*/

                        // Find all associated DataSetWriter objects
                        foreach (PubSubConnectionDataType connection in pubSubConfiguration_.Connections)
                        {
                            foreach (WriterGroupDataType writerGroup in connection.WriterGroups)
                            {
                                foreach (DataSetWriterDataType dataSetWriter in writerGroup.DataSetWriters.ToArray())
                                {
                                    if (dataSetWriter.DataSetName == publishedDataSetDataType.Name)
                                    {
                                        _ = RemoveDataSetWriter(dataSetWriter);
                                    }
                                }
                            }
                        }

                        _ = pubSubConfiguration_.PublishedDataSets.Remove(publishedDataSetDataType);

                        //remove all references from dictionaries
                        _ = idsToObjects_.Remove(publishedDataSetId);
                        _ = objectsToIds_.Remove(publishedDataSetDataType);
                        _ = idsToParentId_.Remove(publishedDataSetId);
                        _ = idsToPubSubState_.Remove(publishedDataSetId);

                        PublishedDataSetRemovedEvent?.Invoke(this, new PublishedDataSetEventArgs() {
                            PublishedDataSetId = publishedDataSetId,
                            PublishedDataSetDataType = publishedDataSetDataType
                        });
                        return StatusCodes.Good;
                    }
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.RemovePublishedDataSet: Exception");
            }

            return StatusCodes.BadNodeIdUnknown;
        }

        /// <summary>
        /// Add Extension field to the specified publishedDataset
        /// </summary>
        /// <param name="publishedDataSetConfigId"></param>
        /// <param name="extensionField"></param>
        /// <returns></returns>
        public StatusCode AddExtensionField(uint publishedDataSetConfigId, Opc.Ua.KeyValuePair extensionField)
        {
            lock (lock_)
            {
                if (!(FindObjectById(publishedDataSetConfigId) is PublishedDataSetDataType publishedDataSetDataType))
                {
                    return StatusCodes.BadNodeIdInvalid;
                }
                if (publishedDataSetDataType.ExtensionFields == null)
                {
                    publishedDataSetDataType.ExtensionFields = new KeyValuePairCollection();
                }
                else
                {
                    //validate duplicate name 
                    var duplicateName = false;
                    foreach (Opc.Ua.KeyValuePair element in publishedDataSetDataType.ExtensionFields)
                    {
                        if (element.Key == extensionField.Key)
                        {
                            duplicateName = true;
                            break;
                        }
                    }
                    if (duplicateName)
                    {
                        Utils.Trace(Utils.TraceMasks.Error, "AddExtensionField -  A field with the name already exists. Duplicate name = {0}", extensionField.Key);
                        return StatusCodes.BadNodeIdExists;
                    }
                }
                var newextensionFieldId = nextId_++;
                //remember connection 
                idsToObjects_.Add(newextensionFieldId, extensionField);
                objectsToIds_.Add(extensionField, newextensionFieldId);
                publishedDataSetDataType.ExtensionFields.Add(extensionField);

                // raise ExtensionFieldAdded event
                ExtensionFieldAddedEvent?.Invoke(this, new ExtensionFieldEventArgs() { PublishedDataSetId = publishedDataSetConfigId, ExtensionFieldId = newextensionFieldId, ExtensionField = extensionField });

                return StatusCodes.Good;
            }
        }

        /// <summary>
        /// Removes an extension field from a published data set
        /// </summary>
        /// <param name="publishedDataSetConfigId"></param>
        /// <param name="extensionFieldConfigId"></param>
        /// <returns></returns>
        public StatusCode RemoveExtensionField(uint publishedDataSetConfigId, uint extensionFieldConfigId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(publishedDataSetConfigId) is PublishedDataSetDataType publishedDataSetDataType) || !(FindObjectById(extensionFieldConfigId) is Opc.Ua.KeyValuePair extensionFieldToRemove))
                {
                    return StatusCodes.BadNodeIdInvalid;
                }
                if (publishedDataSetDataType.ExtensionFields == null)
                {
                    publishedDataSetDataType.ExtensionFields = new KeyValuePairCollection();
                    return StatusCodes.BadNodeIdInvalid;
                }
                // locate the extension field 
                foreach (Opc.Ua.KeyValuePair extensionField in publishedDataSetDataType.ExtensionFields.ToArray())
                {
                    if (extensionField.Equals(extensionFieldToRemove))
                    {
                        _ = publishedDataSetDataType.ExtensionFields.Remove(extensionFieldToRemove);

                        // raise ExtensionFieldRemoved event
                        ExtensionFieldRemovedEvent?.Invoke(this, new ExtensionFieldEventArgs() { PublishedDataSetId = publishedDataSetConfigId, ExtensionFieldId = extensionFieldConfigId, ExtensionField = extensionField });
                        return StatusCodes.Good;
                    }
                }
            }
            return StatusCodes.BadNodeIdInvalid;
        }
        #endregion

        #region Public Methods - Connection
        /// <summary>
        /// Add a connection to current configuration.
        /// </summary>
        /// <param name="pubSubConnectionDataType">The <see cref="PubSubConnectionDataType"/> object that configures the new connection.</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The connection was added with success.
        /// - <see cref="StatusCodes.BadBrowseNameDuplicated"/> An Object with the name already exists.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error adding the connection.
        /// </returns>
        public StatusCode AddConnection(PubSubConnectionDataType pubSubConnectionDataType)
        {
            if (objectsToIds_.ContainsKey(pubSubConnectionDataType))
            {
                throw new ArgumentException("This PubSubConnectionDataType instance is already added to the configuration.");
            }
            try
            {
                lock (lock_)
                {
                    //validate connection name 
                    var duplicateName = false;
                    foreach (PubSubConnectionDataType connection in pubSubConfiguration_.Connections)
                    {
                        if (connection.Name == pubSubConnectionDataType.Name)
                        {
                            duplicateName = true;
                            break;
                        }
                    }
                    if (duplicateName)
                    {
                        Utils.Trace(Utils.TraceMasks.Error, "Attempted to add PubSubConnectionDataType with duplicate name = {0}", pubSubConnectionDataType.Name);
                        return StatusCodes.BadBrowseNameDuplicated;
                    }

                    // remember collections 
                    var writerGroups = new WriterGroupDataTypeCollection(pubSubConnectionDataType.WriterGroups);
                    pubSubConnectionDataType.WriterGroups.Clear();
                    var readerGroups = new ReaderGroupDataTypeCollection(pubSubConnectionDataType.ReaderGroups);
                    pubSubConnectionDataType.ReaderGroups.Clear();

                    var newConnectionId = nextId_++;
                    //remember connection 
                    idsToObjects_.Add(newConnectionId, pubSubConnectionDataType);
                    objectsToIds_.Add(pubSubConnectionDataType, newConnectionId);
                    // remember parent id
                    idsToParentId_.Add(newConnectionId, FindIdForObject(pubSubConfiguration_));
                    //remember initial state
                    idsToPubSubState_.Add(newConnectionId, GetInitialPubSubState(pubSubConnectionDataType));

                    pubSubConfiguration_.Connections.Add(pubSubConnectionDataType);

                    // raise ConnectionAdded event
                    ConnectionAddedEvent?.Invoke(this, new ConnectionEventArgs() { ConnectionId = newConnectionId, PubSubConnectionDataType = pubSubConnectionDataType });
                    //handler reader & writer groups 
                    foreach (WriterGroupDataType writerGroup in writerGroups)
                    {
                        // handle empty names 
                        if (string.IsNullOrEmpty(writerGroup.Name))
                        {
                            //set default name 
                            writerGroup.Name = "WriterGroup_" + (nextId_ + 1);
                        }
                        _ = AddWriterGroup(newConnectionId, writerGroup);
                    }
                    foreach (ReaderGroupDataType readerGroup in readerGroups)
                    {
                        // handle empty names 
                        if (string.IsNullOrEmpty(readerGroup.Name))
                        {
                            //set default name 
                            readerGroup.Name = "ReaderGroup_" + (nextId_ + 1);
                        }
                        _ = AddReaderGroup(newConnectionId, readerGroup);
                    }

                    return StatusCodes.Good;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.AddConnection: Exception");
            }
            return StatusCodes.BadInvalidArgument;
        }

        /// <summary>
        /// Removes a connection from current configuration.
        /// </summary>
        /// <param name="connectionId">Id of the connection to be removed.</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The Connection was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the Connection.
        /// </returns>
        public StatusCode RemoveConnection(uint connectionId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(connectionId) is PubSubConnectionDataType pubSubConnectionDataType))
                {
                    // Unexpected exception 
                    Utils.Trace(Utils.TraceMasks.Information, "Current configuration does not contain PubSubConnectionDataType with ConfigId = {0}", connectionId);
                    return StatusCodes.BadNodeIdUnknown;
                }
                return RemoveConnection(pubSubConnectionDataType);
            }
        }

        /// <summary>
        /// Removes a connection from current configuration.
        /// </summary>
        /// <param name="pubSubConnectionDataType">The connection to be removed.</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The Connection was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the Connection.
        /// </returns>
        public StatusCode RemoveConnection(PubSubConnectionDataType pubSubConnectionDataType)
        {
            try
            {
                lock (lock_)
                {
                    var connectionId = FindIdForObject(pubSubConnectionDataType);
                    if (pubSubConnectionDataType != null && connectionId != InvalidId)
                    {
                        // remove children
                        var writerGroups = new WriterGroupDataTypeCollection(pubSubConnectionDataType.WriterGroups);
                        foreach (WriterGroupDataType writerGroup in writerGroups)
                        {
                            _ = RemoveWriterGroup(writerGroup);
                        }
                        var readerGroups = new ReaderGroupDataTypeCollection(pubSubConnectionDataType.ReaderGroups);
                        foreach (ReaderGroupDataType readerGroup in readerGroups)
                        {
                            _ = RemoveReaderGroup(readerGroup);
                        }
                        _ = pubSubConfiguration_.Connections.Remove(pubSubConnectionDataType);

                        //remove all references from dictionaries
                        _ = idsToObjects_.Remove(connectionId);
                        _ = objectsToIds_.Remove(pubSubConnectionDataType);
                        _ = idsToParentId_.Remove(connectionId);
                        _ = idsToPubSubState_.Remove(connectionId);

                        ConnectionRemovedEvent?.Invoke(this, new ConnectionEventArgs() {
                            ConnectionId = connectionId,
                            PubSubConnectionDataType = pubSubConnectionDataType
                        });
                        return StatusCodes.Good;
                    }
                    return StatusCodes.BadNodeIdUnknown;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.RemoveConnection: Exception");
            }

            return StatusCodes.BadInvalidArgument;
        }
        #endregion

        #region Public Methods - WriterGroup
        /// <summary>
        /// Adds a writerGroup to the specified connection
        /// </summary>
        /// <param name="parentConnectionId"></param>
        /// <param name="writerGroupDataType"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The WriterGroup was added with success.
        /// - <see cref="StatusCodes.BadBrowseNameDuplicated"/> An Object with the name already exists.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error adding the WriterGroup.
        /// </returns>
        public StatusCode AddWriterGroup(uint parentConnectionId, WriterGroupDataType writerGroupDataType)
        {
            if (objectsToIds_.ContainsKey(writerGroupDataType))
            {
                throw new ArgumentException("This WriterGroupDataType instance is already added to the configuration.");
            }
            if (!idsToObjects_.TryGetValue(parentConnectionId, out var value))
            {
                throw new ArgumentException(Utils.Format("There is no connection with configurationId = {0} in current configuration.", parentConnectionId));
            }
            try
            {
                lock (lock_)
                {
                    // remember collections 
                    var dataSetWriters = new DataSetWriterDataTypeCollection(writerGroupDataType.DataSetWriters);
                    writerGroupDataType.DataSetWriters.Clear();
                    if (idsToObjects_[parentConnectionId] is PubSubConnectionDataType parentConnection)
                    {
                        //validate duplicate name 
                        var duplicateName = false;
                        foreach (WriterGroupDataType writerGroup in parentConnection.WriterGroups)
                        {
                            if (writerGroup.Name == writerGroupDataType.Name)
                            {
                                duplicateName = true;
                                break;
                            }
                        }
                        if (duplicateName)
                        {
                            Utils.Trace(Utils.TraceMasks.Error, "Attempted to add WriterGroupDataType with duplicate name = {0}", writerGroupDataType.Name);
                            return StatusCodes.BadBrowseNameDuplicated;
                        }

                        var newWriterGroupId = nextId_++;
                        //remember writer group 
                        idsToObjects_.Add(newWriterGroupId, writerGroupDataType);
                        objectsToIds_.Add(writerGroupDataType, newWriterGroupId);
                        parentConnection.WriterGroups.Add(writerGroupDataType);

                        // remember parent id
                        idsToParentId_.Add(newWriterGroupId, parentConnectionId);
                        //remember initial state
                        idsToPubSubState_.Add(newWriterGroupId, GetInitialPubSubState(writerGroupDataType));

                        // raise WriterGroupAdded event
                        WriterGroupAddedEvent?.Invoke(this,
    new WriterGroupEventArgs() { ConnectionId = parentConnectionId, WriterGroupId = newWriterGroupId, WriterGroupDataType = writerGroupDataType });

                        //handler datasetWriters
                        foreach (DataSetWriterDataType datasetWriter in dataSetWriters)
                        {
                            // handle empty names 
                            if (string.IsNullOrEmpty(datasetWriter.Name))
                            {
                                //set default name 
                                datasetWriter.Name = "DataSetWriter_" + (nextId_ + 1);
                            }
                            _ = AddDataSetWriter(newWriterGroupId, datasetWriter);
                        }

                        return StatusCodes.Good;
                    }
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.AddWriterGroup: Exception");
            }
            return StatusCodes.BadInvalidArgument;
        }

        /// <summary>
        /// Removes a WriterGroupDataType instance from current configuration specified by configId
        /// </summary>
        /// <param name="writerGroupId"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The WriterGroup was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the WriterGroup.
        /// </returns>
        public StatusCode RemoveWriterGroup(uint writerGroupId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(writerGroupId) is WriterGroupDataType writerGroupDataType))
                {
                    // Unexpected exception 
                    Utils.Trace(Utils.TraceMasks.Information, "Current configuration does not contain WriterGroupDataType with ConfigId = {0}", writerGroupId);
                    return StatusCodes.BadNodeIdUnknown;
                }
                return RemoveWriterGroup(writerGroupDataType);
            }
        }

        /// <summary>
        /// Removes a WriterGroupDataType instance from current configuration
        /// </summary>
        /// <param name="writerGroupDataType">Instance to remove</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The WriterGroup was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the WriterGroup.
        /// </returns>
        public StatusCode RemoveWriterGroup(WriterGroupDataType writerGroupDataType)
        {
            try
            {
                lock (lock_)
                {
                    var writerGroupId = FindIdForObject(writerGroupDataType);
                    if (writerGroupDataType != null && writerGroupId != InvalidId)
                    {
                        // remove children
                        var dataSetWriters = new DataSetWriterDataTypeCollection(writerGroupDataType.DataSetWriters);
                        foreach (DataSetWriterDataType dataSetWriter in dataSetWriters)
                        {
                            _ = RemoveDataSetWriter(dataSetWriter);
                        }
                        // find parent connection
                        var parentConnection = FindParentForObject(writerGroupDataType) as PubSubConnectionDataType;
                        var parentConnectionId = FindIdForObject(parentConnection);
                        if (parentConnection != null && parentConnectionId != InvalidId)
                        {
                            _ = parentConnection.WriterGroups.Remove(writerGroupDataType);

                            //remove all references from dictionaries
                            _ = idsToObjects_.Remove(writerGroupId);
                            _ = objectsToIds_.Remove(writerGroupDataType);
                            _ = idsToParentId_.Remove(writerGroupId);
                            _ = idsToPubSubState_.Remove(writerGroupId);

                            WriterGroupRemovedEvent?.Invoke(this, new WriterGroupEventArgs() {
                                WriterGroupId = writerGroupId,
                                WriterGroupDataType = writerGroupDataType,
                                ConnectionId = parentConnectionId
                            });
                            return StatusCodes.Good;
                        }
                    }

                    return StatusCodes.BadNodeIdUnknown;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.RemoveWriterGroup: Exception");
            }

            return StatusCodes.BadInvalidArgument;
        }
        #endregion

        #region Public Methods - DataSetWriter
        /// <summary>
        /// Adds a DataSetWriter to the specified writer group
        /// </summary>
        /// <param name="parentWriterGroupId"></param>
        /// <param name="dataSetWriterDataType"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The DataSetWriter was added with success.
        /// - <see cref="StatusCodes.BadBrowseNameDuplicated"/> An Object with the name already exists.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error adding the DataSetWriter.
        /// </returns>
        public StatusCode AddDataSetWriter(uint parentWriterGroupId, DataSetWriterDataType dataSetWriterDataType)
        {
            if (objectsToIds_.ContainsKey(dataSetWriterDataType))
            {
                throw new ArgumentException("This DataSetWriterDataType instance is already added to the configuration.");
            }
            if (!idsToObjects_.TryGetValue(parentWriterGroupId, out object value))
            {
                throw new ArgumentException(Utils.Format("There is no WriterGroup with configurationId = {0} in current configuration.", parentWriterGroupId));
            }
            try
            {
                lock (lock_)
                {
                    if (idsToObjects_[parentWriterGroupId] is WriterGroupDataType parentWriterGroup)
                    {
                        //validate duplicate name 
                        var duplicateName = false;
                        foreach (DataSetWriterDataType writer in parentWriterGroup.DataSetWriters)
                        {
                            if (writer.Name == dataSetWriterDataType.Name)
                            {
                                duplicateName = true;
                                break;
                            }
                        }
                        if (duplicateName)
                        {
                            Utils.Trace(Utils.TraceMasks.Error, "Attempted to add DataSetWriterDataType with duplicate name = {0}", dataSetWriterDataType.Name);
                            return StatusCodes.BadBrowseNameDuplicated;
                        }

                        var newDataSetWriterId = nextId_++;
                        //remember connection 
                        idsToObjects_.Add(newDataSetWriterId, dataSetWriterDataType);
                        objectsToIds_.Add(dataSetWriterDataType, newDataSetWriterId);
                        parentWriterGroup.DataSetWriters.Add(dataSetWriterDataType);

                        // remember parent id
                        idsToParentId_.Add(newDataSetWriterId, parentWriterGroupId);

                        //remember initial state
                        idsToPubSubState_.Add(newDataSetWriterId, GetInitialPubSubState(dataSetWriterDataType));

                        // raise DataSetWriterAdded event
                        DataSetWriterAddedEvent?.Invoke(this,
    new DataSetWriterEventArgs() { WriterGroupId = parentWriterGroupId, DataSetWriterId = newDataSetWriterId, DataSetWriterDataType = dataSetWriterDataType });

                        return StatusCodes.Good;
                    }
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.AddDataSetWriter: Exception");
            }
            return StatusCodes.BadInvalidArgument;
        }

        /// <summary>
        /// Removes a DataSetWriterDataType instance from current configuration specified by configId
        /// </summary>
        /// <param name="dataSetWriterId"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The DataSetWriter was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the DataSetWriter.
        /// </returns>
        public StatusCode RemoveDataSetWriter(uint dataSetWriterId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(dataSetWriterId) is DataSetWriterDataType dataSetWriterDataType))
                {
                    // Unexpected exception 
                    Utils.Trace(Utils.TraceMasks.Information, "Current configuration does not contain DataSetWriterDataType with ConfigId = {0}", dataSetWriterId);
                    return StatusCodes.BadNodeIdUnknown;
                }
                return RemoveDataSetWriter(dataSetWriterDataType);
            }
        }

        /// <summary>
        /// Removes a DataSetWriterDataType instance from current configuration
        /// </summary>
        /// <param name="dataSetWriterDataType">Instance to remove</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The DataSetWriter was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the DataSetWriter.
        /// </returns>
        public StatusCode RemoveDataSetWriter(DataSetWriterDataType dataSetWriterDataType)
        {
            try
            {
                lock (lock_)
                {
                    var dataSetWriterId = FindIdForObject(dataSetWriterDataType);
                    if (dataSetWriterDataType != null && dataSetWriterId != InvalidId)
                    {
                        // find parent writerGroup
                        var parentWriterGroup = FindParentForObject(dataSetWriterDataType) as WriterGroupDataType;
                        var parentWriterGroupId = FindIdForObject(parentWriterGroup);
                        if (parentWriterGroup != null && parentWriterGroupId != InvalidId)
                        {
                            _ = parentWriterGroup.DataSetWriters.Remove(dataSetWriterDataType);

                            //remove all references from dictionaries
                            _ = idsToObjects_.Remove(dataSetWriterId);
                            _ = objectsToIds_.Remove(dataSetWriterDataType);
                            _ = idsToParentId_.Remove(dataSetWriterId);
                            _ = idsToPubSubState_.Remove(dataSetWriterId);

                            DataSetWriterRemovedEvent?.Invoke(this, new DataSetWriterEventArgs() {
                                WriterGroupId = parentWriterGroupId,
                                DataSetWriterDataType = dataSetWriterDataType,
                                DataSetWriterId = dataSetWriterId
                            });
                            return StatusCodes.Good;
                        }
                    }
                    return StatusCodes.BadNodeIdUnknown;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.RemoveDataSetWriter: Exception");
            }

            return StatusCodes.BadInvalidArgument;
        }
        #endregion

        #region Public Methods - ReaderGroup
        /// <summary>
        /// Adds a readerGroup to the specified connection
        /// </summary>
        /// <param name="parentConnectionId"></param>
        /// <param name="readerGroupDataType"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The ReaderGroup was added with success.
        /// - <see cref="StatusCodes.BadBrowseNameDuplicated"/> An Object with the name already exists.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error adding the ReaderGroup.
        /// </returns>
        public StatusCode AddReaderGroup(uint parentConnectionId, ReaderGroupDataType readerGroupDataType)
        {
            if (objectsToIds_.ContainsKey(readerGroupDataType))
            {
                throw new ArgumentException("This ReaderGroupDataType instance is already added to the configuration.");
            }
            if (!idsToObjects_.TryGetValue(parentConnectionId, out object value))
            {
                throw new ArgumentException(Utils.Format("There is no connection with configurationId = {0} in current configuration.", parentConnectionId));
            }
            try
            {
                lock (lock_)
                {
                    // remember collections 
                    var dataSetReaders = new DataSetReaderDataTypeCollection(readerGroupDataType.DataSetReaders);
                    readerGroupDataType.DataSetReaders.Clear();
                    if (idsToObjects_[parentConnectionId] is PubSubConnectionDataType parentConnection)
                    {
                        //validate duplicate name 
                        var duplicateName = false;
                        foreach (ReaderGroupDataType readerGroup in parentConnection.ReaderGroups)
                        {
                            if (readerGroup.Name == readerGroupDataType.Name)
                            {
                                duplicateName = true;
                                break;
                            }
                        }
                        if (duplicateName)
                        {
                            Utils.Trace(Utils.TraceMasks.Error, "Attempted to add ReaderGroupDataType with duplicate name = {0}", readerGroupDataType.Name);
                            return StatusCodes.BadBrowseNameDuplicated;
                        }

                        var newReaderGroupId = nextId_++;
                        //remember reader group 
                        idsToObjects_.Add(newReaderGroupId, readerGroupDataType);
                        objectsToIds_.Add(readerGroupDataType, newReaderGroupId);
                        parentConnection.ReaderGroups.Add(readerGroupDataType);

                        // remember parent id
                        idsToParentId_.Add(newReaderGroupId, parentConnectionId);

                        //remember initial state
                        idsToPubSubState_.Add(newReaderGroupId, GetInitialPubSubState(readerGroupDataType));

                        // raise ReaderGroupAdded event
                        ReaderGroupAddedEvent?.Invoke(this,
    new ReaderGroupEventArgs() { ConnectionId = parentConnectionId, ReaderGroupId = newReaderGroupId, ReaderGroupDataType = readerGroupDataType });

                        //handler datasetWriters
                        foreach (DataSetReaderDataType datasetReader in dataSetReaders)
                        {
                            // handle empty names 
                            if (string.IsNullOrEmpty(datasetReader.Name))
                            {
                                //set default name 
                                datasetReader.Name = "DataSetReader_" + (nextId_ + 1);
                            }
                            _ = AddDataSetReader(newReaderGroupId, datasetReader);
                        }

                        return StatusCodes.Good;
                    }

                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.AddReaderGroup: Exception");
            }
            return StatusCodes.BadInvalidArgument;
        }

        /// <summary>
        /// Removes a ReaderGroupDataType instance from current configuration specified by configId
        /// </summary>
        /// <param name="readerGroupId"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The ReaderGroup was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the ReaderGroup.
        /// </returns>
        public StatusCode RemoveReaderGroup(uint readerGroupId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(readerGroupId) is ReaderGroupDataType readerGroupDataType))
                {
                    Utils.Trace(Utils.TraceMasks.Information, "Current configuration does not contain ReaderGroupDataType with ConfigId = {0}", readerGroupId);
                    return StatusCodes.BadInvalidArgument;
                }
                return RemoveReaderGroup(readerGroupDataType);
            }
        }

        /// <summary>
        /// Removes a ReaderGroupDataType instance from current configuration
        /// </summary>
        /// <param name="readerGroupDataType">Instance to remove</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The ReaderGroup was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the ReaderGroup.
        /// </returns>
        public StatusCode RemoveReaderGroup(ReaderGroupDataType readerGroupDataType)
        {
            try
            {
                lock (lock_)
                {
                    var readerGroupId = FindIdForObject(readerGroupDataType);
                    if (readerGroupDataType != null && readerGroupId != InvalidId)
                    {
                        // remove children
                        var dataSetReaders = new DataSetReaderDataTypeCollection(readerGroupDataType.DataSetReaders);
                        foreach (DataSetReaderDataType dataSetReader in dataSetReaders)
                        {
                            _ = RemoveDataSetReader(dataSetReader);
                        }
                        // find parent connection
                        var parentConnection = FindParentForObject(readerGroupDataType) as PubSubConnectionDataType;
                        var parentConnectionId = FindIdForObject(parentConnection);
                        if (parentConnection != null && parentConnectionId != InvalidId)
                        {
                            _ = parentConnection.ReaderGroups.Remove(readerGroupDataType);

                            //remove all references from dictionaries
                            _ = idsToObjects_.Remove(readerGroupId);
                            _ = objectsToIds_.Remove(readerGroupDataType);
                            _ = idsToParentId_.Remove(readerGroupId);
                            _ = idsToPubSubState_.Remove(readerGroupId);

                            ReaderGroupRemovedEvent?.Invoke(this, new ReaderGroupEventArgs() {
                                ReaderGroupId = readerGroupId,
                                ReaderGroupDataType = readerGroupDataType,
                                ConnectionId = parentConnectionId
                            });
                            return StatusCodes.Good;
                        }
                    }

                    return StatusCodes.BadNodeIdUnknown;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.RemoveReaderGroup: Exception");
            }

            return StatusCodes.BadInvalidArgument;
        }
        #endregion

        #region Public Methods - DataSetReader
        /// <summary>
        /// Adds a DataSetReader to the specified reader group
        /// </summary>
        /// <param name="parentReaderGroupId"></param>
        /// <param name="dataSetReaderDataType"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The DataSetReader was added with success.
        /// - <see cref="StatusCodes.BadBrowseNameDuplicated"/> An Object with the name already exists.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error adding the DataSetReader.
        /// </returns>
        public StatusCode AddDataSetReader(uint parentReaderGroupId, DataSetReaderDataType dataSetReaderDataType)
        {
            if (objectsToIds_.ContainsKey(dataSetReaderDataType))
            {
                throw new ArgumentException("This DataSetReaderDataType instance is already added to the configuration.");
            }
            if (!idsToObjects_.TryGetValue(parentReaderGroupId, out object value))
            {
                throw new ArgumentException(Utils.Format("There is no ReaderGroup with configurationId = {0} in current configuration.", parentReaderGroupId));
            }
            try
            {
                lock (lock_)
                {
                    if (idsToObjects_[parentReaderGroupId] is ReaderGroupDataType parentReaderGroup)
                    {
                        //validate duplicate name 
                        var duplicateName = false;
                        foreach (DataSetReaderDataType reader in parentReaderGroup.DataSetReaders)
                        {
                            if (reader.Name == dataSetReaderDataType.Name)
                            {
                                duplicateName = true;
                                break;
                            }
                        }
                        if (duplicateName)
                        {
                            Utils.Trace(Utils.TraceMasks.Error, "Attempted to add DataSetReaderDataType with duplicate name = {0}", dataSetReaderDataType.Name);
                            return StatusCodes.BadBrowseNameDuplicated;
                        }

                        var newDataSetReaderId = nextId_++;
                        //remember connection 
                        idsToObjects_.Add(newDataSetReaderId, dataSetReaderDataType);
                        objectsToIds_.Add(dataSetReaderDataType, newDataSetReaderId);
                        parentReaderGroup.DataSetReaders.Add(dataSetReaderDataType);

                        // remember parent id
                        idsToParentId_.Add(newDataSetReaderId, parentReaderGroupId);

                        //remember initial state
                        idsToPubSubState_.Add(newDataSetReaderId, GetInitialPubSubState(dataSetReaderDataType));

                        // raise WriterGroupAdded event
                        DataSetReaderAddedEvent?.Invoke(this,
    new DataSetReaderEventArgs() { ReaderGroupId = parentReaderGroupId, DataSetReaderId = newDataSetReaderId, DataSetReaderDataType = dataSetReaderDataType });

                        return StatusCodes.Good;
                    }
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.AddDataSetReader: Exception");
            }
            return StatusCodes.BadInvalidArgument;
        }

        /// <summary>
        /// Removes a DataSetReaderDataType instance from current configuration specified by configId
        /// </summary>
        /// <param name="dataSetReaderId"></param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The DataSetWriter was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the DataSetWriter.
        /// </returns>
        public StatusCode RemoveDataSetReader(uint dataSetReaderId)
        {
            lock (lock_)
            {
                if (!(FindObjectById(dataSetReaderId) is DataSetReaderDataType dataSetReaderDataType))
                {
                    // Unexpected exception 
                    Utils.Trace(Utils.TraceMasks.Information, "Current configuration does not contain DataSetReaderDataType with ConfigId = {0}", dataSetReaderId);
                    return StatusCodes.BadNodeIdUnknown;
                }
                return RemoveDataSetReader(dataSetReaderDataType);
            }
        }

        /// <summary>
        /// Removes a DataSetReaderDataType instance from current configuration
        /// </summary>
        /// <param name="dataSetReaderDataType">Instance to remove</param>
        /// <returns>
        /// - <see cref="StatusCodes.Good"/> The DataSetWriter was removed with success.
        /// - <see cref="StatusCodes.BadNodeIdUnknown"/> The GroupId is unknown.
        /// - <see cref="StatusCodes.BadInvalidArgument"/> There was an error removing the DataSetWriter.
        /// </returns>
        public StatusCode RemoveDataSetReader(DataSetReaderDataType dataSetReaderDataType)
        {
            try
            {
                lock (lock_)
                {
                    var dataSetReaderId = FindIdForObject(dataSetReaderDataType);
                    if (dataSetReaderDataType != null && dataSetReaderId != InvalidId)
                    {
                        // find parent readerGroup
                        var parentWriterGroup = FindParentForObject(dataSetReaderDataType) as ReaderGroupDataType;
                        var parenReaderGroupId = FindIdForObject(parentWriterGroup);
                        if (parentWriterGroup != null && parenReaderGroupId != InvalidId)
                        {
                            _ = parentWriterGroup.DataSetReaders.Remove(dataSetReaderDataType);

                            //remove all references from dictionaries
                            _ = idsToObjects_.Remove(dataSetReaderId);
                            _ = objectsToIds_.Remove(dataSetReaderDataType);
                            _ = idsToParentId_.Remove(dataSetReaderId);
                            _ = idsToPubSubState_.Remove(dataSetReaderId);

                            DataSetReaderRemovedEvent?.Invoke(this, new DataSetReaderEventArgs() {
                                ReaderGroupId = parenReaderGroupId,
                                DataSetReaderDataType = dataSetReaderDataType,
                                DataSetReaderId = dataSetReaderId
                            });
                            return StatusCodes.Good;
                        }
                    }
                    return StatusCodes.BadNodeIdUnknown;
                }
            }
            catch (Exception ex)
            {
                // Unexpected exception 
                Utils.Trace(ex, "UaPubSubConfigurator.RemoveDataSetReader: Exception");
            }

            return StatusCodes.BadInvalidArgument;
        }
        #endregion

        #region Public Methods - Enable/Disable
        /// <summary> 
        /// Enable the specified configuration object specified by Id
        /// </summary>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        public StatusCode Enable(uint configurationId)
        {
            return Enable(FindObjectById(configurationId));
        }

        /// <summary>
        /// Enable the specified configuration object
        /// </summary>
        /// <param name="configurationObject"></param>
        /// <returns></returns>
        public StatusCode Enable(object configurationObject)
        {
            if (configurationObject == null)
            {
                throw new ArgumentException("The parameter cannot be null.", nameof(configurationObject));
            }
            if (!objectsToIds_.ContainsKey(configurationObject))
            {
                throw new ArgumentException("This {0} instance is not part of current configuration.", configurationObject.GetType().Name);
            }
            PubSubState currentState = FindStateForObject(configurationObject);
            if (currentState != PubSubState.Disabled)
            {
                Utils.Trace(Utils.TraceMasks.Information, "Attempted to call Enable() on an object that is not in Disabled state");
                return StatusCodes.BadInvalidState;
            }
            PubSubState parentState = PubSubState.Operational;
            if (configurationObject != pubSubConfiguration_)
            {
                parentState = FindStateForObject(FindParentForObject(configurationObject));
            }

            if (parentState == PubSubState.Operational)
            {
                // Enabled and parent Operational
                SetStateForObject(configurationObject, PubSubState.Operational);
            }
            else
            {
                // Enabled but parent not Operational
                SetStateForObject(configurationObject, PubSubState.Paused);
            }
            UpdateChildrenState(configurationObject);
            return StatusCodes.Good;
        }


        /// <summary> 
        /// Disable the specified configuration object specified by Id
        /// </summary>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        public StatusCode Disable(uint configurationId)
        {
            return Disable(FindObjectById(configurationId));
        }

        /// <summary>
        /// Disable the specified configuration object
        /// </summary>
        /// <param name="configurationObject"></param>
        /// <returns></returns>
        public StatusCode Disable(object configurationObject)
        {
            if (configurationObject == null)
            {
                throw new ArgumentException("The parameter cannot be null.", nameof(configurationObject));
            }
            if (!objectsToIds_.ContainsKey(configurationObject))
            {
                throw new ArgumentException("This {0} instance is not part of current configuration.", configurationObject.GetType().Name);
            }
            PubSubState currentState = FindStateForObject(configurationObject);
            if (currentState == PubSubState.Disabled)
            {
                Utils.Trace(Utils.TraceMasks.Information, "Attempted to call Disable() on an object that is already in Disabled state");
                return StatusCodes.BadInvalidState;
            }

            SetStateForObject(configurationObject, PubSubState.Disabled);

            UpdateChildrenState(configurationObject);
            return StatusCodes.Good;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Change state for the specified configuration object
        /// </summary>
        /// <param name="configurationObject"></param>
        /// <param name="newState"></param>
        private void SetStateForObject(object configurationObject, PubSubState newState)
        {
            var id = FindIdForObject(configurationObject);
            if (id != InvalidId && idsToPubSubState_.TryGetValue(id, out PubSubState oldState))
            {
                idsToPubSubState_[id] = newState;
                PubSubStateChangedEvent?.Invoke(this, new PubSubStateChangedEventArgs() {
                    ConfigurationObject = configurationObject,
                    ConfigurationObjectId = id,
                    NewState = newState,
                    OldState = oldState
                });
                var configurationObjectEnabled = newState == PubSubState.Operational || newState == PubSubState.Paused;
                //update the Enabled flag in config object
                if (configurationObject is PubSubConfigurationDataType)
                {
                    ((PubSubConfigurationDataType)configurationObject).Enabled = configurationObjectEnabled;
                }
                else if (configurationObject is PubSubConnectionDataType)
                {
                    ((PubSubConnectionDataType)configurationObject).Enabled = configurationObjectEnabled;
                }
                else if (configurationObject is WriterGroupDataType)
                {
                    ((WriterGroupDataType)configurationObject).Enabled = configurationObjectEnabled;
                }
                else if (configurationObject is DataSetWriterDataType)
                {
                    ((DataSetWriterDataType)configurationObject).Enabled = configurationObjectEnabled;
                }
                else if (configurationObject is ReaderGroupDataType)
                {
                    ((ReaderGroupDataType)configurationObject).Enabled = configurationObjectEnabled;
                }
                else if (configurationObject is DataSetReaderDataType)
                {
                    ((DataSetReaderDataType)configurationObject).Enabled = configurationObjectEnabled;
                }
            }
        }

        /// <summary>
        /// Calculate and update the state for child objects of a configuration object (StATE MACHINE)
        /// </summary>
        /// <param name="configurationObject"></param>
        private void UpdateChildrenState(object configurationObject)
        {
            PubSubState parentState = FindStateForObject(configurationObject);
            //find child ids
            List<uint> childrenIds = FindChildrenIdsForObject(configurationObject);
            if (parentState == PubSubState.Operational)
            {
                // Enabled and parent Operational
                foreach (var childId in childrenIds)
                {
                    PubSubState childState = FindStateForId(childId);
                    if (childState == PubSubState.Paused)
                    {
                        // become Operational if Parent changed to Operational
                        var childObject = FindObjectById(childId);
                        SetStateForObject(childObject, PubSubState.Operational);

                        UpdateChildrenState(childObject);
                    }
                }
            }
            else if (parentState == PubSubState.Disabled || parentState == PubSubState.Paused)
            {
                // Parent changed to Disabled or Paused
                foreach (var childId in childrenIds)
                {
                    PubSubState childState = FindStateForId(childId);
                    if (childState == PubSubState.Operational || childState == PubSubState.Error)
                    {
                        // become Operational if Parent changed to Operational
                        var childObject = FindObjectById(childId);
                        SetStateForObject(childObject, PubSubState.Paused);

                        UpdateChildrenState(childObject);
                    }
                }
            }
        }

        /// <summary>
        /// Get <see cref="PubSubState"/> for an item depending on enabled flag and parent's <see cref="PubSubState"/>.
        /// </summary>
        /// <param name="enabled">Configured Enabled flag. </param>
        /// <param name="parentPubSubState"><see cref="PubSubState"/> of the parent configured object.</param>
        /// <returns></returns>
        private static PubSubState GetInitialPubSubState(bool enabled, PubSubState parentPubSubState)
        {
            if (enabled)
            {
                if (parentPubSubState == PubSubState.Operational)
                {
                    // The PubSub component is operational.
                    return PubSubState.Operational;
                }
                else
                {
                    // The PubSub component is enabled but currently paused by a parent component. The
                    // parent component is either Disabled_0 or Paused_1.
                    return PubSubState.Paused;
                }
            }
            else
            {
                // PubSub component is configured but currently disabled.
                return PubSubState.Disabled;
            }
        }

        /// <summary>
        /// Calculate and return the initial state of a pub sub data type configuration object
        /// </summary>
        /// <param name="configurationObject"></param>
        /// <returns></returns>
        private PubSubState GetInitialPubSubState(object configurationObject)
        {
            PubSubState parentPubSubState = PubSubState.Operational;

            bool configurationObjectEnabled;
            if (configurationObject is PubSubConfigurationDataType type)
            {
                configurationObjectEnabled = type.Enabled;
            }
            else if (configurationObject is PubSubConnectionDataType)
            {
                configurationObjectEnabled = ((PubSubConnectionDataType)configurationObject).Enabled;
                //find parent state 
                parentPubSubState = FindStateForObject(pubSubConfiguration_);
            }
            else if (configurationObject is WriterGroupDataType)
            {
                configurationObjectEnabled = ((WriterGroupDataType)configurationObject).Enabled;
                //find parent connection
                var parentConnection = FindParentForObject(configurationObject);
                //find parent state 
                parentPubSubState = FindStateForObject(parentConnection);
            }
            else if (configurationObject is DataSetWriterDataType)
            {
                configurationObjectEnabled = ((DataSetWriterDataType)configurationObject).Enabled;
                //find parent 
                var parentWriterGroup = FindParentForObject(configurationObject);
                //find parent state 
                parentPubSubState = FindStateForObject(parentWriterGroup);
            }
            else if (configurationObject is ReaderGroupDataType)
            {
                configurationObjectEnabled = ((ReaderGroupDataType)configurationObject).Enabled;
                //find parent connection
                var parentConnection = FindParentForObject(configurationObject);
                //find parent state 
                parentPubSubState = FindStateForObject(parentConnection);
            }
            else if (configurationObject is DataSetReaderDataType)
            {
                configurationObjectEnabled = ((DataSetReaderDataType)configurationObject).Enabled;
                //find parent 
                var parentReaderGroup = FindParentForObject(configurationObject);
                //find parent state 
                parentPubSubState = FindStateForObject(parentReaderGroup);
            }
            else
            {
                return PubSubState.Error;
            }
            return GetInitialPubSubState(configurationObjectEnabled, parentPubSubState);
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Value of an uninitialized identifier.
        /// </summary>
        internal static uint InvalidId;

        private readonly object lock_ = new object();
        private readonly PubSubConfigurationDataType pubSubConfiguration_;
        private readonly Dictionary<uint, object> idsToObjects_;
        private readonly Dictionary<object, uint> objectsToIds_;
        private readonly Dictionary<uint, PubSubState> idsToPubSubState_;
        private readonly Dictionary<uint, uint> idsToParentId_;
        private uint nextId_ = 1;
        #endregion

    }
}
