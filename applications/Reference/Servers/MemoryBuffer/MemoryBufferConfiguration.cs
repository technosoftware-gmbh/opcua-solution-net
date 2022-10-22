#region Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2021-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

using Technosoftware.UaServer;
#endregion

namespace MemoryBuffer
{
    /// <summary>
    /// Stores the configuration the test node manager
    /// </summary>
    [DataContract(Namespace = Namespaces.MemoryBuffer)]
    public class MemoryBufferConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public MemoryBufferConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
            buffers_ = null;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The buffers exposed by the memory 
        /// </summary>
        [DataMember(Order = 1)]
        public MemoryBufferInstanceCollection Buffers
        {
            get { return buffers_; }
            set { buffers_ = value; }
        }
        #endregion

        #region Private Members
        private MemoryBufferInstanceCollection buffers_;
        #endregion
    }

    /// <summary>
    /// Stores the configuration for a memory buffer instance.
    /// </summary>
    [DataContract(Namespace = Namespaces.MemoryBuffer)]
    public class MemoryBufferInstance
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public MemoryBufferInstance()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
            name_ = null;
            tagCount_ = 0;
            dataType_ = null;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The browse name for the instance.
        /// </summary>
        [DataMember(Order = 1)]
        public string Name
        {
            get { return name_; }
            set { name_ = value; }
        }

        /// <summary>
        /// The number of tags in the buffer.
        /// </summary>
        [DataMember(Order = 2)]
        public int TagCount
        {
            get { return tagCount_; }
            set { tagCount_ = value; }
        }

        /// <summary>
        /// The data type of the tags in the buffer.
        /// </summary>
        [DataMember(Order = 3)]
        public string DataType
        {
            get { return dataType_; }
            set { dataType_ = value; }
        }
        #endregion

        #region Private Members
        private string name_;
        private int tagCount_;
        private string dataType_;
        #endregion
    }

    #region MemoryBufferInstanceCollection Class
    /// <summary>
    /// A collection of MemoryBufferInstances.
    /// </summary>
    [CollectionDataContract(Name = "ListOfMemoryBufferInstance", Namespace = Namespaces.MemoryBuffer, ItemName = "MemoryBufferInstance")]
    public partial class MemoryBufferInstanceCollection : List<MemoryBufferInstance>
    {
    }
    #endregion
}
