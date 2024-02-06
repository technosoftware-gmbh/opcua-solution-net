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
using System.IO;
using System.Linq;

using NUnit.Framework;

using Opc.Ua;

using Technosoftware.UaPubSub.Configuration;
using Technosoftware.UaPubSub.PublishedData;
#endregion

namespace Technosoftware.UaPubSub.Tests.PublishedData
{
    [TestFixture(Description = "Tests for DataCollector class")]
    public class DataCollectorTests
    {
        private string m_configurationFileName = Path.Combine("Configuration", "PublisherConfiguration.xml");

        public const int NamespaceIndex = 2;

        [Test(Description = "Validate AddPublishedDataSet with null parameter.")]
        public void ValidateAddPublishedDataSetWithNullParameter()
        {
            //Arrange
            DataCollector dataCollector = new DataCollector(new UaPubSubDataStore());

            //Assert
            Assert.Throws<ArgumentException>(() => dataCollector.AddPublishedDataSet(null));
        }

        [Test(Description = "Validate AddPublishedDataSet.")]
        public void ValidateAddPublishedDataSet()
        {
            //Arrange
            string configurationFile = Utils.GetAbsoluteFilePath(m_configurationFileName, true, true, false);
            var pubSubConfiguration = UaPubSubConfigurationHelper.LoadConfiguration(configurationFile);

            DataCollector dataCollector = new DataCollector(new UaPubSubDataStore());
            //Act  
            dataCollector.AddPublishedDataSet(pubSubConfiguration.PublishedDataSets.First());
            DataSet collectedDataSet = dataCollector.CollectData(pubSubConfiguration.PublishedDataSets.First().Name);
            //Assert
            Assert.IsNotNull(collectedDataSet,
                "Cannot collect data therefore the '{0}' publishedDataSet was not registered correctly.", pubSubConfiguration.PublishedDataSets[0].Name);
        }

        [Test(Description = "Validate RemovePublishedDataSet.")]
        public void ValidateRemovePublishedDataSet()
        {
            //Arrange
            DataCollector dataCollector = new DataCollector(new UaPubSubDataStore());
            PublishedDataSetDataType publishedDataSet = new PublishedDataSetDataType();
            publishedDataSet.Name = "Name";
            //Act  
            dataCollector.AddPublishedDataSet(publishedDataSet);
            dataCollector.RemovePublishedDataSet(publishedDataSet);
            DataSet collectedDataSet = dataCollector.CollectData(publishedDataSet.Name);
            //Assert
            Assert.IsNull(collectedDataSet, "The '{0}' publishedDataSet was not removed correctly.", publishedDataSet.Name);
        }

        [Test(Description = "Validate RemovePublishedDataSet with null parameter.")]
        public void ValidateRemovePublishedDataSetWithNullParameter()
        {
            //Arrange
            DataCollector dataCollector = new DataCollector(new UaPubSubDataStore());
            //Assert
            Assert.Throws<ArgumentException>(() => dataCollector.RemovePublishedDataSet(null));
        }

        [Test(Description = "Validate CollectData from DataStore.")]
        public void ValidateCollectDataFromDataStore()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();
            dataStore.WritePublishedDataItem(new NodeId("BoolToggle", NamespaceIndex), 0, new DataValue(new Variant(false)));
            dataStore.WritePublishedDataItem(new NodeId("Int32", NamespaceIndex), 0, new DataValue(new Variant(1)));
            dataStore.WritePublishedDataItem(new NodeId("Int32Fast", NamespaceIndex), 0, new DataValue(new Variant(2)));
            dataStore.WritePublishedDataItem(new NodeId("DateTime", NamespaceIndex), 0, new DataValue(new Variant(DateTime.MaxValue)));

            DataCollector dataCollector = new DataCollector(dataStore);
            #region set up published data set that collects data from extension fields
            PublishedDataSetDataType publishedDataSetSimple = new PublishedDataSetDataType();
            publishedDataSetSimple.Name = "Simple";
            // Define  publishedDataSetSimple.DataSetMetaData
            publishedDataSetSimple.DataSetMetaData = new DataSetMetaDataType();
            publishedDataSetSimple.DataSetMetaData.DataSetClassId = new Uuid(Guid.Empty);
            publishedDataSetSimple.DataSetMetaData.Name = publishedDataSetSimple.Name;
            publishedDataSetSimple.DataSetMetaData.Fields = new FieldMetaDataCollection()
                {
                    new FieldMetaData()
                    {
                        Name = "BoolToggle",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.Boolean,
                        ValueRank = ValueRanks.Scalar
                    },
                    new FieldMetaData()
                    {
                        Name = "Int32",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.Int32,
                        ValueRank = ValueRanks.Scalar
                    },
                    new FieldMetaData()
                    {
                        Name = "Int32Fast",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.Int32,
                        ValueRank = ValueRanks.Scalar
                    },
                    new FieldMetaData()
                    {
                        Name = "DateTime",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.DateTime,
                        ValueRank = ValueRanks.Scalar
                    }
                };

            PublishedDataItemsDataType publishedDataItems = new PublishedDataItemsDataType();
            publishedDataItems.PublishedData = new PublishedVariableDataTypeCollection();
            //create PublishedData based on metadata names
            foreach (var field in publishedDataSetSimple.DataSetMetaData.Fields)
            {
                publishedDataItems.PublishedData.Add(
                    new PublishedVariableDataType() {
                        PublishedVariable = new NodeId(field.Name, NamespaceIndex),
                        AttributeId = Attributes.Value,
                    });
            }
            publishedDataSetSimple.DataSetSource = new ExtensionObject(publishedDataItems);
            #endregion 

            //Act  
            dataCollector.AddPublishedDataSet(publishedDataSetSimple);
            DataSet collectedDataSet = dataCollector.CollectData(publishedDataSetSimple.Name);

            //Assert
            Assert.IsNotNull(publishedDataItems, "The m_firstPublishedDataSet.DataSetSource is not PublishedDataItemsDataType.");
            Assert.IsNotNull(collectedDataSet, "collectedDataSet is null.");
            Assert.IsNotNull(collectedDataSet.Fields, "collectedDataSet.Fields is null.");

            Assert.AreEqual(collectedDataSet.Fields.Length, publishedDataItems.PublishedData.Count, "collectedDataSet and published data fields count do not match.");

            // validate collected values
            Assert.AreEqual(collectedDataSet.Fields[0].Value.Value, false, "collectedDataSet.Fields[0].Value.Value does not match.");
            Assert.AreEqual(collectedDataSet.Fields[1].Value.Value, (int)1, "collectedDataSet.Fields[1].Value.Value does not match.");
            Assert.AreEqual(collectedDataSet.Fields[2].Value.Value, (int)2, "collectedDataSet.Fields[2].Value.Value does not match.");
            Assert.AreEqual(collectedDataSet.Fields[3].Value.Value, DateTime.MaxValue, "collectedDataSet.Fields[3].Value.Value does not match.");
        }


        [Test(Description = "Validate CollectData from ExtensionFields.")]
        public void ValidateCollectDataFromExtensionFields()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();
            DataCollector dataCollector = new DataCollector(dataStore);
            #region set up published data set that collects data from extension fields
            PublishedDataSetDataType publishedDataSetSimple = new PublishedDataSetDataType();
            publishedDataSetSimple.Name = "Simple";
            // Define  publishedDataSetSimple.DataSetMetaData
            publishedDataSetSimple.DataSetMetaData = new DataSetMetaDataType();
            publishedDataSetSimple.DataSetMetaData.DataSetClassId = new Uuid(Guid.Empty);
            publishedDataSetSimple.DataSetMetaData.Name = publishedDataSetSimple.Name;
            publishedDataSetSimple.DataSetMetaData.Fields = new FieldMetaDataCollection()
                {
                    new FieldMetaData()
                    {
                        Name = "BoolToggle",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.Boolean,
                        ValueRank = ValueRanks.Scalar
                    },
                    new FieldMetaData()
                    {
                        Name = "Int32",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.Int32,
                        ValueRank = ValueRanks.Scalar
                    },
                    new FieldMetaData()
                    {
                        Name = "Int32Fast",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.Int32,
                        ValueRank = ValueRanks.Scalar
                    },
                    new FieldMetaData()
                    {
                        Name = "DateTime",
                        DataSetFieldId = new Uuid(Guid.NewGuid()),
                        DataType = DataTypeIds.DateTime,
                        ValueRank = ValueRanks.Scalar
                    }
                };

            //initialize Extension fields collection
            publishedDataSetSimple.ExtensionFields = new KeyValuePairCollection()
                {
                    new Opc.Ua.KeyValuePair()
                    {
                        Key =  new QualifiedName("BoolToggle"),
                        Value = true
                    },
                     new Opc.Ua.KeyValuePair()
                    {
                        Key =  new QualifiedName("Int32"),
                        Value = (int)100
                    },
                     new Opc.Ua.KeyValuePair()
                    {
                        Key =  new QualifiedName("Int32Fast"),
                        Value = (int)50
                    },
                    new Opc.Ua.KeyValuePair()
                    {
                        Key =  new QualifiedName( "DateTime"),
                        Value = DateTime.Today
                    }
                };

            PublishedDataItemsDataType publishedDataItems = new PublishedDataItemsDataType();
            publishedDataItems.PublishedData = new PublishedVariableDataTypeCollection();
            //create PublishedData based on metadata names
            foreach (var field in publishedDataSetSimple.DataSetMetaData.Fields)
            {
                publishedDataItems.PublishedData.Add(
                    new PublishedVariableDataType() {
                        SubstituteValue = new QualifiedName(field.Name)
                    });
            }
            publishedDataSetSimple.DataSetSource = new ExtensionObject(publishedDataItems);
            #endregion

            //Act  
            dataCollector.AddPublishedDataSet(publishedDataSetSimple);
            DataSet collectedDataSet = dataCollector.CollectData(publishedDataSetSimple.Name);
            //Assert
            Assert.IsNotNull(publishedDataItems, "The m_firstPublishedDataSet.DataSetSource is not PublishedDataItemsDataType.");
            Assert.IsNotNull(collectedDataSet, "collectedDataSet is null.");
            Assert.IsNotNull(collectedDataSet.Fields, "collectedDataSet.Fields is null.");

            Assert.AreEqual(collectedDataSet.Fields.Length, publishedDataItems.PublishedData.Count, "collectedDataSet and published data fields count do not match.");
            // validate collected values
            Assert.AreEqual(collectedDataSet.Fields[0].Value.Value, true, "collectedDataSet.Fields[0].Value.Value does not match.");
            Assert.AreEqual(collectedDataSet.Fields[1].Value.Value, (int)100, "collectedDataSet.Fields[1].Value.Value does not match.");
            Assert.AreEqual(collectedDataSet.Fields[2].Value.Value, (int)50, "collectedDataSet.Fields[2].Value.Value does not match.");
            Assert.AreEqual(collectedDataSet.Fields[3].Value.Value, DateTime.Today, "collectedDataSet.Fields[3].Value.Value does not match.");
        }

        [Test(Description = "Validate CollectData unknown dataset name.")]
        public void ValidateCollectDataUnknownDataSetName()
        {
            //Arrange
            DataCollector dataCollector = new DataCollector(new UaPubSubDataStore());
            //Act              
            DataSet collectedDataSet = dataCollector.CollectData("");
            //Assert
            Assert.IsNull(collectedDataSet, "The data collect returns data for unknown DataSetName.");
        }

        [Test(Description = "Validate CollectData null dataset name.")]
        public void ValidateCollectDataNullDataSetName()
        {
            //Arrange
            DataCollector dataCollector = new DataCollector(new UaPubSubDataStore());

            //Assert
            Assert.Throws<ArgumentException>(() => dataCollector.CollectData(null), "The data collect does not throw exception when null parameter.");
        }
    }
}
