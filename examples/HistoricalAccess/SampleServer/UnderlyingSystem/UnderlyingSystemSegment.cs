#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2021 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
#endregion

namespace SampleCompany.SampleServer.UnderlyingSystem
{
    /// <summary>
    /// Stores information about a segment in the system.
    /// </summary>
    public class UnderlyingSystemSegment
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlyingSystemSegment"/> class.
        /// </summary>
        public UnderlyingSystemSegment()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the unique id for the segment.
        /// </summary>
        /// <value>The unique id for the segment</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the segment.
        /// </summary>
        /// <value>The name of the segment.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the segment.
        /// </summary>
        /// <value>The type of the segment.</value>
        public string SegmentType { get; set; }
        #endregion

        #region Private Methods
        #endregion

        #region Private Fields
        #endregion
    }
}
