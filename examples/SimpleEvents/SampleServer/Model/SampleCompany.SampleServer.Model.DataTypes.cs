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

namespace SampleCompany.SampleServer.Model
{
    #region CycleStepDataType Class
    #if (!OPCUA_EXCLUDE_CycleStepDataType)
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    [DataContract(Namespace = SampleCompany.SampleServer.Model.Namespaces.SampleServer)]
    public partial class CycleStepDataType : IEncodeable, IJsonEncodeable
    {
        #region Constructors
        /// <remarks />
        public CycleStepDataType()
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
            m_name = null;
            m_duration = (double)0;
        }
        #endregion

        #region Public Properties
        /// <remarks />
        [DataMember(Name = "Name", IsRequired = false, Order = 1)]
        public string Name
        {
            get { return m_name;  }
            set { m_name = value; }
        }

        /// <remarks />
        [DataMember(Name = "Duration", IsRequired = false, Order = 2)]
        public double Duration
        {
            get { return m_duration;  }
            set { m_duration = value; }
        }
        #endregion

        #region IEncodeable Members
        /// <summary cref="IEncodeable.TypeId" />
        public virtual ExpandedNodeId TypeId => DataTypeIds.CycleStepDataType; 

        /// <summary cref="IEncodeable.BinaryEncodingId" />
        public virtual ExpandedNodeId BinaryEncodingId => ObjectIds.CycleStepDataType_Encoding_DefaultBinary;

        /// <summary cref="IEncodeable.XmlEncodingId" />
        public virtual ExpandedNodeId XmlEncodingId => ObjectIds.CycleStepDataType_Encoding_DefaultXml;
                    
        /// <summary cref="IJsonEncodeable.JsonEncodingId" />
        public virtual ExpandedNodeId JsonEncodingId => ObjectIds.CycleStepDataType_Encoding_DefaultJson; 

        /// <summary cref="IEncodeable.Encode(IEncoder)" />
        public virtual void Encode(IEncoder encoder)
        {
            encoder.PushNamespace(SampleCompany.SampleServer.Model.Namespaces.SampleServer);

            encoder.WriteString("Name", Name);
            encoder.WriteDouble("Duration", Duration);

            encoder.PopNamespace();
        }

        /// <summary cref="IEncodeable.Decode(IDecoder)" />
        public virtual void Decode(IDecoder decoder)
        {
            decoder.PushNamespace(SampleCompany.SampleServer.Model.Namespaces.SampleServer);

            Name = decoder.ReadString("Name");
            Duration = decoder.ReadDouble("Duration");

            decoder.PopNamespace();
        }

        /// <summary cref="IEncodeable.IsEqual(IEncodeable)" />
        public virtual bool IsEqual(IEncodeable encodeable)
        {
            if (Object.ReferenceEquals(this, encodeable))
            {
                return true;
            }

            CycleStepDataType value = encodeable as CycleStepDataType;

            if (value == null)
            {
                return false;
            }

            if (!Utils.IsEqual(m_name, value.m_name)) return false;
            if (!Utils.IsEqual(m_duration, value.m_duration)) return false;

            return true;
        }

        #if !NET_STANDARD
        /// <summary cref="ICloneable.Clone" />
        public virtual object Clone()
        {
            return (CycleStepDataType)this.MemberwiseClone();
        }
        #endif

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            CycleStepDataType clone = (CycleStepDataType)base.MemberwiseClone();

            clone.m_name = (string)Utils.Clone(this.m_name);
            clone.m_duration = (double)Utils.Clone(this.m_duration);

            return clone;
        }
        #endregion

        #region Private Fields
        private string m_name;
        private double m_duration;
        #endregion
    }

    #region CycleStepDataTypeCollection Class
    /// <remarks />
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    [CollectionDataContract(Name = "ListOfCycleStepDataType", Namespace = SampleCompany.SampleServer.Model.Namespaces.SampleServer, ItemName = "CycleStepDataType")]
    #if !NET_STANDARD
    public partial class CycleStepDataTypeCollection : List<CycleStepDataType>, ICloneable
    #else
    public partial class CycleStepDataTypeCollection : List<CycleStepDataType>
    #endif
    {
        #region Constructors
        /// <remarks />
        public CycleStepDataTypeCollection() {}

        /// <remarks />
        public CycleStepDataTypeCollection(int capacity) : base(capacity) {}

        /// <remarks />
        public CycleStepDataTypeCollection(IEnumerable<CycleStepDataType> collection) : base(collection) {}
        #endregion

        #region Static Operators
        /// <remarks />
        public static implicit operator CycleStepDataTypeCollection(CycleStepDataType[] values)
        {
            if (values != null)
            {
                return new CycleStepDataTypeCollection(values);
            }

            return new CycleStepDataTypeCollection();
        }

        /// <remarks />
        public static explicit operator CycleStepDataType[](CycleStepDataTypeCollection values)
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
            return (CycleStepDataTypeCollection)this.MemberwiseClone();
        }
        #endregion
        #endif

        /// <summary cref="Object.MemberwiseClone" />
        public new object MemberwiseClone()
        {
            CycleStepDataTypeCollection clone = new CycleStepDataTypeCollection(this.Count);

            for (int ii = 0; ii < this.Count; ii++)
            {
                clone.Add((CycleStepDataType)Utils.Clone(this[ii]));
            }

            return clone;
        }
    }
    #endregion
    #endif
    #endregion
}