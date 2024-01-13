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
using Technosoftware.UaPubSub.PublishedData;

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub
{
    /// <summary>
    /// Base class for a DataSet message implementation
    /// </summary>
    public abstract class UaDataSetMessage
    {
        #region Fields
        // Configuration Major and Major current version (VersionTime)
        /// <summary>
        /// Default value for Configured MetaDataVersion.MajorVersion
        /// </summary>
        protected const UInt32 defaultConfigMajorVersion = 0;
        /// <summary>
        /// Default value for Configured MetaDataVersion.MinorVersion
        /// </summary>
        protected const UInt32 defaultConfigMinorVersion = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="UaDataSetMessage"/>
        /// </summary>
        public UaDataSetMessage()
        {
            DecodeErrorReason = DataSetDecodeErrorReason.NoError;
            Timestamp = DateTime.UtcNow;
            MetaDataVersion = new ConfigurationVersionDataType() {
                MajorVersion = defaultConfigMajorVersion,
                MinorVersion = defaultConfigMinorVersion
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get DataSet
        /// </summary>
        public DataSet DataSet { get; internal set; }

        /// <summary>
        /// Get and Set corresponding DataSetWriterId
        /// </summary>
        public ushort DataSetWriterId { get; set; }

        /// <summary>
        /// Get DataSetFieldContentMask
        /// This DataType defines flags to include DataSet field related information like status and 
        /// timestamp in addition to the value in the DataSetMessage.
        /// </summary>
        public DataSetFieldContentMask FieldContentMask { get; protected set; }

        /// <summary>
        /// The version of the DataSetMetaData which describes the contents of the Payload.
        /// </summary>
        public ConfigurationVersionDataType MetaDataVersion { get; set; }

        /// <summary>
        /// Get and Set SequenceNumber
        /// A strictly monotonically increasing sequence number assigned by the publisher to each DataSetMessage sent.
        /// </summary>
        public uint SequenceNumber { get; set; }

        /// <summary>
        /// Get and Set Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Get and Set Status
        /// </summary>
        public StatusCode Status { get; set; }

        /// <summary>
        /// Get and Set the reason that an error encountered while decoding occurred
        /// </summary>
        public DataSetDecodeErrorReason DecodeErrorReason { get; set; }

        /// <summary>
        /// Checks if the MetadataMajorVersion has changed depending on the value of DataSetDecodeErrorReason
        /// </summary>
        public bool IsMetadataMajorVersionChange
        {
            get
            {
                return DecodeErrorReason == DataSetDecodeErrorReason.MetadataMajorVersion;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set DataSetFieldContentMask 
        /// </summary>
        /// <param name="fieldContentMask">The new <see cref="DataSetFieldContentMask"/> for this dataset</param>
        public abstract void SetFieldContentMask(DataSetFieldContentMask fieldContentMask);
        #endregion

        #region Protected Methods
        /// <summary>
        /// Validates the MetadataVersion against a given ConfigurationVersionDataType
        /// </summary>
        /// <param name="configurationVersionDataType">The value to validate MetadataVersion against</param>
        /// <returns>NoError if validation passes or the cause of the failure</returns>
        protected DataSetDecodeErrorReason ValidateMetadataVersion(ConfigurationVersionDataType configurationVersionDataType)
        {
            if (MetaDataVersion.MajorVersion != defaultConfigMajorVersion)
            {
                if (MetaDataVersion.MajorVersion != configurationVersionDataType.MajorVersion)
                {
                    return DataSetDecodeErrorReason.MetadataMajorVersion;
                }
            }

            return DataSetDecodeErrorReason.NoError;
        }
        #endregion
    }
}
