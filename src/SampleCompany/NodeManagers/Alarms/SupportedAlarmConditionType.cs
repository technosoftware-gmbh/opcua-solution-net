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
using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.Alarms
{
    public class SupportedAlarmConditionType
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a supported alarm condition tpye.
        /// </summary>
        /// <param name="name">The name of the alarm.</param>
        /// <param name="conditionName">The name of the condition type.</param>
        /// <param name="nodeId">The node ID.</param>
        public SupportedAlarmConditionType(string name, string conditionName, NodeId nodeId)
        {
            Name = name;
            ConditionName = conditionName;
            Node = nodeId;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the alarm.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The name of the condition type.
        /// </summary>
        public string ConditionName { get; private set; }

        /// <summary>
        /// The node ID.
        /// </summary>
        public NodeId Node { get; private set; }
        #endregion

        #region Private Fields
        #endregion
    }
}
