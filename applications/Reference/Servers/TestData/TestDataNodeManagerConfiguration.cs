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
using System.Runtime.Serialization;
#endregion

namespace TestData
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
            saveFilePath_ = null;
            maxQueueSize_ = 100;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The path to the file that stores state of the node manager.
        /// </summary>
        [DataMember(Order = 1)]
        public string SaveFilePath
        {
            get { return saveFilePath_; }
            set { saveFilePath_ = value; }
        }

        /// <summary>
        /// The maximum length for a monitored item sampling queue.
        /// </summary>
        [DataMember(Order = 2)]
        public uint MaxQueueSize
        {
            get { return maxQueueSize_; }
            set { maxQueueSize_ = value; }
        }

        /// <summary>
        /// The next unused value that can be assigned to new nodes.
        /// </summary>
        [DataMember(Order = 3)]
        public uint NextUnusedId
        {
            get { return nextUnusedId_; }
            set { nextUnusedId_ = value; }
        }
        #endregion

        #region Private Members
        private string saveFilePath_;
        private uint maxQueueSize_;
        private uint nextUnusedId_;
        #endregion
    }
}
