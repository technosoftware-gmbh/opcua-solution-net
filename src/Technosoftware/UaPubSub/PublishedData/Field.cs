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
using Opc.Ua;
#endregion

namespace Technosoftware.UaPubSub.PublishedData
{
    /// <summary>
    /// Base class for a DataSet field
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Get/Set Value 
        /// </summary>
        public DataValue Value { get; set; }

        /// <summary>
        /// Get/Set Target NodeId 
        /// </summary>
        public NodeId TargetNodeId { get; set; }

        /// <summary>
        /// Get/Set target attribute 
        /// </summary>
        public uint TargetAttribute { get; set; }

        /// <summary>
        /// Get configured <see cref="FieldMetaData"/> object for this <see cref="Field"/> instance.
        /// </summary>
        public FieldMetaData FieldMetaData { get; internal set; }

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
            Field copy = base.MemberwiseClone() as Field;
            if (Value != null)
            {
                if (copy != null)
                {
                    copy.Value = Value.Clone() as DataValue;
                }
            }

            if (FieldMetaData != null)
            {
                if (copy != null)
                {
                    copy.FieldMetaData = FieldMetaData.Clone() as FieldMetaData;
                }
            }
            return copy;
        }
        #endregion
    }
}
