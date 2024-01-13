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

using NUnit.Framework;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Tests.Configuration
{
    [TestFixture(Description = "Tests for UaPubSubDataStore class")]
    public class UaPubSubDataStoreTests
    {
        #region WritePublishedDataItem
        [Test(Description = "Validate WritePublishedDataItem call with different values")]

        public void ValidateWritePublishedDataItem(
            [Values(true, (byte)1, (ushort)2, (short)3, (uint)4, (int)5, (ulong)6, (long)7,
            (double)8, (float)9, "10")] object value)
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();
            NodeId nodeId = new NodeId("ns=1;i=1");

            //Act     
            dataStore.WritePublishedDataItem(nodeId, Attributes.Value, new DataValue(new Variant(value)));
            DataValue readDataValue = dataStore.ReadPublishedDataItem(nodeId, Attributes.Value);

            //Assert
            Assert.IsNotNull(readDataValue, "Returned DataValue for written nodeId and attribute is null");
            Assert.AreEqual(readDataValue.Value, value, "Read after write returned different value");
        }

        [Test(Description = "Validate WritePublishedDataItem call with null NodeId")]
        public void ValidateWritePublishedDataItemNullNodeId()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();

            //Assert
            Assert.Throws(typeof(ArgumentException), () => dataStore.WritePublishedDataItem(null));
        }

        [Test(Description = "Validate WritePublishedDataItem call with invalid Attribute")]
        public void ValidateWritePublishedDataItemInvalidAttribute()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();

            //Assert
            Assert.Throws(typeof(ArgumentException),
                () => dataStore.WritePublishedDataItem(new NodeId("ns=0;i=2253"), (uint)Attributes.AccessLevelEx + 1));
        }
        #endregion

        #region ReadPublishedDataItem
        [Test(Description = "Validate ReadPublishedDataItem call for non existing node id")]
        public void ValidateReadPublishedDataItem()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();
            NodeId nodeId = new NodeId("ns=1;i=1");

            //Act     
            DataValue readDataValue = dataStore.ReadPublishedDataItem(nodeId, Attributes.Value);

            //Assert
            Assert.IsNull(readDataValue, "Returned DataValue for written nodeId and attribute is NOT null");
        }

        [Test(Description = "Validate ReadPublishedDataItem call with null NodeId")]
        public void ValidateReadPublishedDataItemNullNodeId()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();

            //Assert
            Assert.Throws(typeof(ArgumentException), () => dataStore.ReadPublishedDataItem(null));
        }

        [Test(Description = "Validate ReadPublishedDataItem call with invalid Attribute")]
        public void ValidateReadPublishedDataIteminvalidAttribute()
        {
            //Arrange
            UaPubSubDataStore dataStore = new UaPubSubDataStore();
            //Assert
            Assert.Throws(typeof(ArgumentException),
                () => dataStore.ReadPublishedDataItem(new NodeId("ns=0;i=2253"), (uint)Attributes.AccessLevelEx + 1));
        }
        #endregion
    }
}
