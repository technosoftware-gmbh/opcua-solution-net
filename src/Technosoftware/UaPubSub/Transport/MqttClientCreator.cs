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
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Transport
{
    internal static class MqttClientCreator
    {
        /// <summary>
        /// The method which returns an MQTT client
        /// </summary>
        /// <param name="reconnectInterval">Number of seconds to reconnect to the MQTT broker</param>
        /// <param name="mqttClientOptions">The client options for MQTT broker connection</param>
        /// <param name="receiveMessageHandler">The receiver message handler</param>
        /// <param name="topicFilter">The topics to which to subscribe</param>
        /// <returns></returns>
        internal static async Task<IMqttClient> GetMqttClientAsync(int reconnectInterval,
                                                                   MqttClientOptions mqttClientOptions,
                                                                   Func<MqttApplicationMessageReceivedEventArgs, Task> receiveMessageHandler,
                                                                   StringCollection topicFilter = null)
        {
            IMqttClient mqttClient = mqttClientFactory_.Value.CreateMqttClient();

            // Hook the receiveMessageHandler in case we deal with a subscriber
            if ((receiveMessageHandler != null) && (topicFilter != null))
            {

                mqttClient.ApplicationMessageReceivedAsync += receiveMessageHandler;
                mqttClient.ConnectedAsync += async e => {
                    Utils.Trace("{0} Connected to MQTTBroker", mqttClient?.Options?.ClientId);

                    try
                    {
                        foreach (string topic in topicFilter)
                        {
                            // subscribe to provided topics, messages are also filtered on the receiveMessageHandler
                            await mqttClient.SubscribeAsync(topic).ConfigureAwait(false);
                        }

                        Utils.Trace("{0} Subscribed to topics: {1}", mqttClient?.Options?.ClientId, string.Join(",", topicFilter));
                    }
                    catch (Exception exception)
                    {
                        Utils.Trace(exception, "{0} could not subscribe to topics: {1}", mqttClient?.Options?.ClientId, string.Join(",", topicFilter));
                    }
                };
            }
            else
            {
                if (receiveMessageHandler == null)
                {
                    Utils.Trace("The provided MQTT message handler is null therefore messages will not be processed on client {0}!!!", mqttClient?.Options?.ClientId);
                }
                if (topicFilter == null)
                {
                    Utils.Trace("The provided MQTT message topic filter is null therefore messages will not be processed on client {0}!!!", mqttClient?.Options?.ClientId);
                }
            }

            // Setup reconnect handler
            mqttClient.DisconnectedAsync += async e => {
                await Task.Delay(TimeSpan.FromSeconds(reconnectInterval)).ConfigureAwait(false);
                try
                {
                    Utils.Trace("Disconnect Handler called on client {0}, reason: {1} was connected: {2}",
                        mqttClient?.Options?.ClientId,
                        e.Reason,
                        e.ClientWasConnected);
                    await Connect(reconnectInterval, mqttClientOptions, mqttClient).ConfigureAwait(false);
                }
                catch (Exception excOnDisconnect)
                {
                    Utils.Trace("{0} Failed to reconnect after disconnect occurred: {1}", mqttClient?.Options?.ClientId, excOnDisconnect.Message);
                }
            };

            await Connect(reconnectInterval, mqttClientOptions, mqttClient).ConfigureAwait(false);

            return mqttClient;
        }

        /// <summary>
        /// Perform the connection to the MQTTBroker
        /// </summary>
        /// <param name="reconnectInterval"></param>
        /// <param name="mqttClientOptions"></param>
        /// <param name="mqttClient"></param>
        private static async Task Connect(int reconnectInterval, MqttClientOptions mqttClientOptions, IMqttClient mqttClient)
        {
            try
            {
                var result = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).ConfigureAwait(false);
                if (MqttClientConnectResultCode.Success == result.ResultCode)
                {
                    Utils.Trace("MQTT client {0} successfully connected", mqttClient?.Options?.ClientId);
                }
                else
                {
                    Utils.Trace("MQTT client {0} connect atempt returned {0}", mqttClient?.Options?.ClientId, result?.ResultCode);
                }
            }
            catch (Exception e)
            {
                Utils.Trace("MQTT client {0} connect atempt returned {1} will try to reconnect in {2} seconds",
                    mqttClient?.Options?.ClientId,
                    e.Message,
                    reconnectInterval);
            }
        }

        #region Private Fields
        private static readonly Lazy<MqttFactory> mqttClientFactory_ = new Lazy<MqttFactory>(() => new MqttFactory());
        #endregion
    }
}
