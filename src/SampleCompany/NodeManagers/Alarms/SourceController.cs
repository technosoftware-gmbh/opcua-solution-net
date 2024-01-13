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

#pragma warning disable CS1591

namespace SampleCompany.NodeManagers.Alarms
{
    public class SourceController
    {
        #region Constructors, Destructor, Initialization
        public SourceController(BaseDataVariableState source, AlarmController controller)
        {
            Source = source;
            Controller = controller;
        }
        #endregion

        #region Properties
        public AlarmController Controller { get; set; }
        public BaseDataVariableState Source { get; set; }
        #endregion

    }
}
