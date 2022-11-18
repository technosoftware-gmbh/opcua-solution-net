/* ========================================================================
 * Copyright (c) 2005-2021 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace SampleCompany.NodeManagers.SampleDataTypes
{
    #region MachineStateDataType Enumeration
    #if (!OPCUA_EXCLUDE_MachineStateDataType)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    [DataContract(Namespace = SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes)]
    public enum MachineStateDataType
    {
        /// <remarks />
        [EnumMember(Value = "Inactive_0")]
        Inactive = 0,

        /// <remarks />
        [EnumMember(Value = "Cutting_1")]
        Cutting = 1,

        /// <remarks />
        [EnumMember(Value = "PrepareLoad_2")]
        PrepareLoad = 2,

        /// <remarks />
        [EnumMember(Value = "ExecuteLoad_3")]
        ExecuteLoad = 3,

        /// <remarks />
        [EnumMember(Value = "PrepareUnload_4")]
        PrepareUnload = 4,

        /// <remarks />
        [EnumMember(Value = "ExecuteUnload_5")]
        ExecuteUnload = 5,

        /// <remarks />
        [EnumMember(Value = "PrepareRemove_6")]
        PrepareRemove = 6,

        /// <remarks />
        [EnumMember(Value = "ExecuteRemove_7")]
        ExecuteRemove = 7,

        /// <remarks />
        [EnumMember(Value = "PrepareSort_8")]
        PrepareSort = 8,

        /// <remarks />
        [EnumMember(Value = "ExecuteSort_9")]
        ExecuteSort = 9,

        /// <remarks />
        [EnumMember(Value = "Finished_10")]
        Finished = 10,

        /// <remarks />
        [EnumMember(Value = "Failed_11")]
        Failed = 11,
    }

    #region MachineStateDataTypeCollection Class
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    [CollectionDataContract(Name = "ListOfMachineStateDataType", Namespace = SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes, ItemName = "MachineStateDataType")]
    #if !NET_STANDARD
    public partial class MachineStateDataTypeCollection : List<MachineStateDataType>, ICloneable
    #else
    public partial class MachineStateDataTypeCollection : List<MachineStateDataType>
    #endif
    {
        #region Constructors
        /// <remarks />
        public MachineStateDataTypeCollection() {}

        /// <remarks />
        public MachineStateDataTypeCollection(int capacity) : base(capacity) {}

        /// <remarks />
        public MachineStateDataTypeCollection(IEnumerable<MachineStateDataType> collection) : base(collection) {}
        #endregion

        #region Static Operators
        /// <remarks />
        public static implicit operator MachineStateDataTypeCollection(MachineStateDataType[] values)
        {
            if (values != null)
            {
                return new MachineStateDataTypeCollection(values);
            }

            return new MachineStateDataTypeCollection();
        }

        /// <remarks />
        public static explicit operator MachineStateDataType[](MachineStateDataTypeCollection values)
        {
            if (values != null)
            {
                return values.ToArray();
            }

            return null;
        }
        #endregion

        #if !NET_STANDARD
        #region ICloneable Methods
        /// <remarks />
        public object Clone()
        {
            return (MachineStateDataTypeCollection)this.MemberwiseClone();
        }
        #endregion
        #endif

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            MachineStateDataTypeCollection clone = new MachineStateDataTypeCollection(this.Count);

            for (int ii = 0; ii < this.Count; ii++)
            {
                clone.Add((MachineStateDataType)Utils.Clone(this[ii]));
            }

            return clone;
        }
    }
    #endregion
    #endif
    #endregion

    #region MachineDataType Class
    #if (!OPCUA_EXCLUDE_MachineDataType)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    [DataContract(Namespace = SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes)]
    public partial class MachineDataType : IEncodeable, IJsonEncodeable
    {
        #region Constructors
        /// <remarks />
        public MachineDataType()
        {
            Initialize();
        }
            
        [OnDeserializing]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }
            
        private void Initialize()
        {
            m_machineName = null;
            m_manufacturer = null;
            m_serialNumber = null;
            m_machineState = MachineStateDataType.Inactive;
        }
        #endregion

        #region Public Properties
        /// <remarks />
        [DataMember(Name = "MachineName", IsRequired = false, Order = 1)]
        public string MachineName
        {
            get { return m_machineName;  }
            set { m_machineName = value; }
        }

        /// <remarks />
        [DataMember(Name = "Manufacturer", IsRequired = false, Order = 2)]
        public string Manufacturer
        {
            get { return m_manufacturer;  }
            set { m_manufacturer = value; }
        }

        /// <remarks />
        [DataMember(Name = "SerialNumber", IsRequired = false, Order = 3)]
        public string SerialNumber
        {
            get { return m_serialNumber;  }
            set { m_serialNumber = value; }
        }

        /// <remarks />
        [DataMember(Name = "MachineState", IsRequired = false, Order = 4)]
        public MachineStateDataType MachineState
        {
            get { return m_machineState;  }
            set { m_machineState = value; }
        }
        #endregion

        #region IEncodeable Members
        /// <summary cref="IEncodeable.TypeId" />
        public virtual ExpandedNodeId TypeId => DataTypeIds.MachineDataType; 

        /// <summary cref="IEncodeable.BinaryEncodingId" />
        public virtual ExpandedNodeId BinaryEncodingId => ObjectIds.MachineDataType_Encoding_DefaultBinary;

        /// <summary cref="IEncodeable.XmlEncodingId" />
        public virtual ExpandedNodeId XmlEncodingId => ObjectIds.MachineDataType_Encoding_DefaultXml;
                    
        /// <summary cref="IJsonEncodeable.JsonEncodingId" />
        public virtual ExpandedNodeId JsonEncodingId => ObjectIds.MachineDataType_Encoding_DefaultJson; 

        /// <summary cref="IEncodeable.Encode(IEncoder)" />
        public virtual void Encode(IEncoder encoder)
        {
            encoder.PushNamespace(SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

            encoder.WriteString("MachineName", MachineName);
            encoder.WriteString("Manufacturer", Manufacturer);
            encoder.WriteString("SerialNumber", SerialNumber);
            encoder.WriteEnumerated("MachineState", MachineState);

            encoder.PopNamespace();
        }

        /// <summary cref="IEncodeable.Decode(IDecoder)" />
        public virtual void Decode(IDecoder decoder)
        {
            decoder.PushNamespace(SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes);

            MachineName = decoder.ReadString("MachineName");
            Manufacturer = decoder.ReadString("Manufacturer");
            SerialNumber = decoder.ReadString("SerialNumber");
            MachineState = (MachineStateDataType)decoder.ReadEnumerated("MachineState", typeof(MachineStateDataType));

            decoder.PopNamespace();
        }

        /// <summary cref="IEncodeable.IsEqual(IEncodeable)" />
        public virtual bool IsEqual(IEncodeable encodeable)
        {
            if (Object.ReferenceEquals(this, encodeable))
            {
                return true;
            }

            MachineDataType value = encodeable as MachineDataType;

            if (value == null)
            {
                return false;
            }

            if (!Utils.IsEqual(m_machineName, value.m_machineName)) return false;
            if (!Utils.IsEqual(m_manufacturer, value.m_manufacturer)) return false;
            if (!Utils.IsEqual(m_serialNumber, value.m_serialNumber)) return false;
            if (!Utils.IsEqual(m_machineState, value.m_machineState)) return false;

            return true;
        }

        #if !NET_STANDARD
        /// <summary cref="ICloneable.Clone" />
        public virtual object Clone()
        {
            return (MachineDataType)this.MemberwiseClone();
        }
        #endif

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            MachineDataType clone = (MachineDataType)base.MemberwiseClone();

            clone.m_machineName = (string)Utils.Clone(this.m_machineName);
            clone.m_manufacturer = (string)Utils.Clone(this.m_manufacturer);
            clone.m_serialNumber = (string)Utils.Clone(this.m_serialNumber);
            clone.m_machineState = (MachineStateDataType)Utils.Clone(this.m_machineState);

            return clone;
        }
        #endregion

        #region Private Fields
        private string m_machineName;
        private string m_manufacturer;
        private string m_serialNumber;
        private MachineStateDataType m_machineState;
        #endregion
    }

    #region MachineDataTypeCollection Class
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    [CollectionDataContract(Name = "ListOfMachineDataType", Namespace = SampleCompany.NodeManagers.SampleDataTypes.Namespaces.SampleDataTypes, ItemName = "MachineDataType")]
    #if !NET_STANDARD
    public partial class MachineDataTypeCollection : List<MachineDataType>, ICloneable
    #else
    public partial class MachineDataTypeCollection : List<MachineDataType>
    #endif
    {
        #region Constructors
        /// <remarks />
        public MachineDataTypeCollection() {}

        /// <remarks />
        public MachineDataTypeCollection(int capacity) : base(capacity) {}

        /// <remarks />
        public MachineDataTypeCollection(IEnumerable<MachineDataType> collection) : base(collection) {}
        #endregion

        #region Static Operators
        /// <remarks />
        public static implicit operator MachineDataTypeCollection(MachineDataType[] values)
        {
            if (values != null)
            {
                return new MachineDataTypeCollection(values);
            }

            return new MachineDataTypeCollection();
        }

        /// <remarks />
        public static explicit operator MachineDataType[](MachineDataTypeCollection values)
        {
            if (values != null)
            {
                return values.ToArray();
            }

            return null;
        }
        #endregion

        #if !NET_STANDARD
        #region ICloneable Methods
        /// <remarks />
        public object Clone()
        {
            return (MachineDataTypeCollection)this.MemberwiseClone();
        }
        #endregion
        #endif

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            MachineDataTypeCollection clone = new MachineDataTypeCollection(this.Count);

            for (int ii = 0; ii < this.Count; ii++)
            {
                clone.Add((MachineDataType)Utils.Clone(this[ii]));
            }

            return clone;
        }
    }
    #endregion
    #endif
    #endregion
}