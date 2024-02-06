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
using System.Threading;
using System.Linq;

using Moq;

using NUnit.Framework;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Tests.Configuration
{
    [TestFixture(Description = "Tests for UAPublisher class")]
    public class UaPublisherTests
    {
        static IList<DateTime> s_publishTimes = new List<DateTime>();

        [Test(Description = "Test that PublishMessage method is called after a UAPublisher is started.")]
        [Combinatorial]
#if !CUSTOM_TESTS
        [Ignore("This test should be executed locally")]
#endif
        public void ValidateUaPublisherPublishIntervalDeviation(
            [Values(100, 1000, 2000)] double publishingInterval,
            [Values(30, 40)] double maxDeviation,
            [Values(10)] int publishTimeInSeconds)
        {
            //Arrange
            s_publishTimes.Clear();
            var mockConnection = new Mock<IUaPubSubConnection>();
            mockConnection.Setup(x => x.CanPublish(It.IsAny<WriterGroupDataType>())).Returns(true);

            mockConnection.Setup(x => x.CreateNetworkMessages(It.IsAny<WriterGroupDataType>(), It.IsAny<WriterGroupPublishState>()))
                .Callback(() => s_publishTimes.Add(DateTime.Now));

            WriterGroupDataType writerGroupDataType = new WriterGroupDataType();
            writerGroupDataType.PublishingInterval = publishingInterval;

            //Act 
            UaPublisher publisher = new UaPublisher(mockConnection.Object, writerGroupDataType);
            publisher.Start();

            //wait so many seconds
            Thread.Sleep(publishTimeInSeconds * 1000);
            publisher.Stop();
            int faultIndex = -1;
            double faultDeviation = 0;

            s_publishTimes =  (from t in s_publishTimes
                             orderby t
                            select t).ToList();

            //Assert
            for (int i = 1; i < s_publishTimes.Count; i++)
            {
                double interval = s_publishTimes[i].Subtract(s_publishTimes[i - 1]).TotalMilliseconds;
                double deviation = Math.Abs(publishingInterval - interval);
                if (deviation >= maxDeviation && deviation > faultDeviation)
                {
                    faultIndex = i;
                    faultDeviation = deviation;
                }
            }
            Assert.IsTrue(faultIndex < 0, "publishingInterval={0}, maxDeviation={1}, publishTimeInSecods={2}, deviation[{3}] = {4} has maximum deviation", publishingInterval, maxDeviation, publishTimeInSeconds, faultIndex, faultDeviation);
        }
    }
}
