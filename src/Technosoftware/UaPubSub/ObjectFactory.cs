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

using Opc.Ua;

using Technosoftware.UaPubSub.Transport;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Implementation of Factory pattern - Used to create objects depending on used protocol
    /// </summary>
    internal static class ObjectFactory
    {
        /// <summary>
        /// Create connections from PubSubConnectionDataType configuration objects.
        /// </summary>
        /// <param name="uaPubSubApplication">The parent <see cref="UaPubSubApplication"/></param>
        /// <param name="pubSubConnectionDataType">The configuration object for the new <see cref="UaPubSubConnection"/></param>
        /// <returns>The new instance of <see cref="UaPubSubConnection"/>.</returns>
        public static UaPubSubConnection CreateConnection(UaPubSubApplication uaPubSubApplication, PubSubConnectionDataType pubSubConnectionDataType)
        {
            if (pubSubConnectionDataType.TransportProfileUri == Profiles.PubSubUdpUadpTransport)
            {
                return new UdpPubSubConnection(uaPubSubApplication, pubSubConnectionDataType);
            }
            else if (pubSubConnectionDataType.TransportProfileUri == Profiles.PubSubMqttUadpTransport)
            {
                return new MqttPubSubConnection(uaPubSubApplication, pubSubConnectionDataType, MessageMapping.Uadp);
            }
            else if (pubSubConnectionDataType.TransportProfileUri == Profiles.PubSubMqttJsonTransport)
            {
                return new MqttPubSubConnection(uaPubSubApplication, pubSubConnectionDataType, MessageMapping.Json);
            }
            throw new ArgumentException("Invalid TransportProfileUri.", nameof(pubSubConnectionDataType));
        }
    }
}
