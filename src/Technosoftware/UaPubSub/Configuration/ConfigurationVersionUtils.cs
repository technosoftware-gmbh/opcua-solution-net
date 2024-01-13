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
#endregion

namespace Technosoftware.UaPubSub.Configuration
{
    /// <summary>
    /// Helper class that calculates the ConfigurationVersion for MetaData 
    /// </summary>
    public static class ConfigurationVersionUtils
    {
        /// <summary>
        /// Analyze and decide the right ConfigurationVersion for new MetaData 
        /// </summary>
        /// <param name="oldMetaData">The historical MetaData to be compared against the new MetaData</param>
        /// <param name="newMetaData">The new MetaData </param>
        /// <returns></returns>
        public static ConfigurationVersionDataType CalculateConfigurationVersion(DataSetMetaDataType oldMetaData, DataSetMetaDataType newMetaData)
        {
            if (newMetaData == null)
            {
                throw new ArgumentNullException(nameof(newMetaData));
            }

            bool hasMinorVersionChange = false;
            bool hasMajorVersionChange = false;

            if (oldMetaData == null)
            {
                // create first version of ConfigurationVersion
                hasMajorVersionChange = true;
            }
            else
            {
                /*Removing fields from the DataSet content, reordering fields, adding fields in between other fields or a
                 * DataType change in fields shall result in an update of the MajorVersion.  */
                // check if any field was deleted 
                if (oldMetaData.Fields.Count > newMetaData.Fields.Count)
                {
                    hasMajorVersionChange = true;
                }
                else
                {
                    // compare fileds
                    for (int i = 0; i < oldMetaData.Fields.Count; i++)
                    {
                        /*If at least one Property value of a DataSetMetaData field changes, the MajorVersion shall be updated.*/
                        if (!Utils.IsEqual(oldMetaData.Fields[i].Properties, newMetaData.Fields[1].Properties))
                        {
                            hasMajorVersionChange = true;
                            break;
                        }
                    }
                    if (!hasMajorVersionChange && oldMetaData.Fields.Count < newMetaData.Fields.Count)
                    {
                        /* Only the MinorVersion shall be updated if fields are added at the end of the DataSet content.*/
                        hasMinorVersionChange = true;
                    }
                }
            }

            if (hasMajorVersionChange || hasMinorVersionChange)
            {
                UInt32 versionTime = CalculateVersionTime(DateTime.UtcNow);
                if (hasMajorVersionChange)
                {
                    // Change both minor and major version
                    return new ConfigurationVersionDataType() {
                        MinorVersion = versionTime,
                        MajorVersion = versionTime
                    };
                }
                else
                {
                    // only minor version was changed
                    return new ConfigurationVersionDataType() {
                        MinorVersion = versionTime,
                        MajorVersion = newMetaData.ConfigurationVersion.MajorVersion
                    };
                }
            }

            // there is no change 
            return new ConfigurationVersionDataType() {
                MinorVersion = newMetaData.ConfigurationVersion.MinorVersion,
                MajorVersion = newMetaData.ConfigurationVersion.MajorVersion
            };

        }

        /// <summary>
        /// Calculate and return the VersionTime calculated for the input parameter
        /// </summary>
        /// <param name="timeOfConfiguration">The current time of configuration</param>
        /// <returns></returns>
        public static UInt32 CalculateVersionTime(DateTime timeOfConfiguration)
        {
            /*This primitive data type is a UInt32 that represents the time in seconds since the year 2000. The epoch date is midnight UTC (00:00) on January 1, 2000.

            It is used as version number based on the last change time. If the version is updated, the new value shall be greater than the previous value.
            If a Variable is initialized with a VersionTime value, the value must be either loaded from persisted configuration or time synchronization must be available to ensure a unique version is applied.
            The value 0 is used to indicate that no version information is available.*/
            return (uint)timeOfConfiguration.Subtract(epochDate_).TotalSeconds;
        }

        /// <summary>
        /// Check if the DataSetMetData is usable for decoding.
        /// It shall be not null and have the Fields collection defined and also the ConfigurationVersion shall be not null or Empty
        /// </summary>
        /// <param name="dataSetMetaData"></param>
        /// <returns></returns>
        public static bool IsUsable(DataSetMetaDataType dataSetMetaData)
        {
            if (dataSetMetaData == null) return false;
            if (dataSetMetaData.Fields == null) return false;
            if (dataSetMetaData.Fields.Count == 0) return false;

            if (dataSetMetaData.ConfigurationVersion == null) return false;
            if (dataSetMetaData.ConfigurationVersion.MajorVersion == 0) return false;
            if (dataSetMetaData.ConfigurationVersion.MinorVersion == 0) return false;

            return true;
        }

        #region Private Fields
        // The epoch date is midnight UTC (00:00) on January 1, 2000.
        private static readonly DateTime epochDate_ = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion
    }
}
