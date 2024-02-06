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
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.Configuration
{
    /// <summary>
    /// Helper class for load/save configuration
    /// </summary>
    public static class UaPubSubConfigurationHelper
    {
        /// <summary>
        /// Save a <see cref="PubSubConfigurationDataType"/> instance as XML
        /// </summary>
        /// <param name="pubSubConfiguration">The configuration object that shall be saved in the file.</param>
        /// <param name="filePath">The file path from where the configuration shall be saved.</param>
        public static void SaveConfiguration(PubSubConfigurationDataType pubSubConfiguration, string filePath)
        {
            Stream ostrm = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);

            XmlWriterSettings settings = Utils.DefaultXmlWriterSettings();
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(PubSubConfigurationDataType));
                serializer.WriteObject(writer, pubSubConfiguration);
            }
        }

        /// <summary>
        /// Load a <see cref="PubSubConfigurationDataType"/> instance from and XML File
        /// </summary>
        /// <param name="filePath">The file path from where the configuration shall be loaded.</param>
        public static PubSubConfigurationDataType LoadConfiguration(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(PubSubConfigurationDataType));
                    return (PubSubConfigurationDataType)serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                StringBuilder buffer = new StringBuilder();
                buffer.AppendFormat("Configuration file could not be loaded: {0}\r\n", filePath);
                buffer.AppendFormat("Error: {0}", e.Message);

                throw ServiceResultException.Create(
                    StatusCodes.BadConfigurationError,
                    e,
                    buffer.ToString());
            }
        }
    }
}
