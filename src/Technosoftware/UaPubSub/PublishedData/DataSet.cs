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

using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.PublishedData
{
    /// <summary>
    /// Entity that holds DataSet structure that is published/received bu the PubSub
    /// </summary>
    public class DataSet : ICloneable
    {
        #region Constructor
        /// <summary>
        /// Create new instance of <see cref="DataSet"/>
        /// </summary>
        /// <param name="name"></param>
        public DataSet(string name = null)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get/Set data set name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/Set flag that indicates if DataSet is delta frame
        /// </summary>
        public bool IsDeltaFrame { get; set; }

        /// <summary>
        /// Get/Set the DataSetWriterId that produced this DataSet
        /// </summary>
        public int DataSetWriterId { get; set; }

        /// <summary>
        /// Gets SequenceNumber - a strictly monotonically increasing sequence number assigned by the publisher to each DataSetMessage sent.
        /// </summary>
        public uint SequenceNumber { get; internal set; }

        /// <summary>
        /// Gets DataSetMetaData for this DataSet
        /// </summary>
        public DataSetMetaDataType DataSetMetaData { get; set; }

        /// <summary>
        /// Get/Set data set fields for this data set
        /// </summary>
        public Field[] Fields { get; set; }
        #endregion

        #region ICloneable method
        /// <inheritdoc/>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Create a deep copy of current DataSet
        /// </summary>
        public new object MemberwiseClone()
        {
            DataSet copy = base.MemberwiseClone() as DataSet;
            if (DataSetMetaData != null)
            {
                if (copy != null)
                {
                    copy.DataSetMetaData = DataSetMetaData.Clone() as DataSetMetaDataType;
                }
            }

            if (Fields != null)
            {
                if (copy != null)
                {
                    copy.Fields = new Field[Fields.Length];
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        copy.Fields[i] = Fields[i].Clone() as Field;
                    }
                }
            }
            return copy;
        }
        #endregion
    }
}
