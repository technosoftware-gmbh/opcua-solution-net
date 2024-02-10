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
using System.Runtime.Serialization;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    /// <summary>
    /// Stores the configuration the test node manager
    /// </summary>
    [DataContract(Namespace = Namespaces.TestData)]
    public class TestDataNodeManagerConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public TestDataNodeManagerConfiguration()
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
            m_saveFilePath = null;
            m_maxQueueSize = 100;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The path to the file that stores state of the node manager.
        /// </summary>
        [DataMember(Order = 1)]
        public string SaveFilePath
        {
            get => m_saveFilePath;
            set => m_saveFilePath = value;
        }

        /// <summary>
        /// The maximum length for a monitored item sampling queue.
        /// </summary>
        [DataMember(Order = 2)]
        public uint MaxQueueSize
        {
            get => m_maxQueueSize;
            set => m_maxQueueSize = value;
        }

        /// <summary>
        /// The next unused value that can be assigned to new nodes.
        /// </summary>
        [DataMember(Order = 3)]
        public uint NextUnusedId
        {
            get => m_nextUnusedId;
            set => m_nextUnusedId = value;
        }
        #endregion

        #region Private Members
        private string m_saveFilePath;
        private uint m_maxQueueSize;
        private uint m_nextUnusedId;
        #endregion
    }
}
