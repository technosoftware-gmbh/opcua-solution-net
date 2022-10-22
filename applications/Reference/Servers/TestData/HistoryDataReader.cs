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
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Globalization;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace TestData
{
    /// <summary>
    /// A class used to read values from a history data source.
    /// </summary>
    public class HistoryDataReader : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructs a reader for the source.
        /// </summary>
        /// <param name="source">The source of the history data.</param>
        public HistoryDataReader(NodeId variableId, IHistoryDataSource source)
        {
            m_id = Guid.NewGuid();
            m_variableId = variableId;
            m_source = source;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // nothing to do.
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// A globally unique identifier for the instance.
        /// </summary>
        public Guid Id
        {
            get { return m_id; }
        }

        /// <summary>
        /// The identifier for the variable being read.
        /// </summary>
        public NodeId VariableId
        {
            get { return m_variableId; }
        }

        /// <summary>
        /// Starts reading raw values.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        /// <param name="request">The request parameters.</param>
        /// <param name="timestampsToReturn">The timestamps to return with the value.</param>
        /// <param name="indexRange">The range to return for array values.</param>
        /// <param name="dataEncoding">The data encoding to use for structured values.</param>
        /// <param name="values">The values to return.</param>
        public void BeginReadRaw(
            UaServerContext context,
            ReadRawModifiedDetails request,
            TimestampsToReturn timestampsToReturn,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DataValueCollection values)
        {
            m_request = request;

            // initialize start and end.
            m_startTime = m_request.StartTime;
            m_endTime = m_request.EndTime;

            if (m_endTime == DateTime.MinValue)
            {
                m_endTime = DateTime.MaxValue;
            }

            // check the direction.
            m_isForward = m_startTime < m_endTime;
            m_position = -1;

            DataValue value = null;

            // get first bound.
            if (m_request.ReturnBounds)
            {
                value = m_source.FirstRaw(m_startTime, !m_isForward, m_request.IsReadModified, out m_position);

                if (value != null)
                {
                    AddValue(timestampsToReturn, indexRange, dataEncoding, values, value);
                }
            }
        }

        /// <summary>
        /// Continues a read raw operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        /// <param name="timestampsToReturn">The timestamps to return with the value.</param>
        /// <param name="indexRange">The range to return for array values.</param>
        /// <param name="dataEncoding">The data encoding to use for structured values.</param>
        /// <param name="values">The values to return.</param>
        /// <returns>False if the operation halted because the maximum number of values was discovered.</returns>
        public bool NextReadRaw(
            UaServerContext context,
            TimestampsToReturn timestampsToReturn,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DataValueCollection values)
        {
            DataValue value = null;

            do
            {
                // check for limit.
                if (m_request.NumValuesPerNode > 0 && values.Count >= m_request.NumValuesPerNode)
                {
                    return false;
                }

                value = m_source.NextRaw(m_lastTime, m_isForward, m_request.IsReadModified, ref m_position);

                // no more data.
                if (value == null)
                {
                    return true;
                }

                // check for bound.
                if ((m_isForward && value.ServerTimestamp >= m_endTime) || (!m_isForward && value.ServerTimestamp <= m_endTime))
                {
                    if (m_request.ReturnBounds)
                    {
                        AddValue(timestampsToReturn, indexRange, dataEncoding, values, value);
                        return true;
                    }
                }

                // add value.
                AddValue(timestampsToReturn, indexRange, dataEncoding, values, value);
            }
            while (value != null);

            return true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds a DataValue to a list of values to return.
        /// </summary>
        private void AddValue(
            TimestampsToReturn timestampsToReturn,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            DataValueCollection values,
            DataValue value)
        {
            // ignore invalid case.
            if (value == null)
            {
                return;
            }

            // save the last timestamp returned.
            m_lastTime = value.ServerTimestamp;

            // check if the index range or data encoding can be applied.
            if (StatusCode.IsGood(value.StatusCode))
            {
                object valueToReturn = value.Value;

                // apply the index range.
                if (indexRange != NumericRange.Empty)
                {
                    StatusCode error = indexRange.ApplyRange(ref valueToReturn);

                    if (StatusCode.IsBad(error))
                    {
                        value.Value = null;
                        value.StatusCode = error;
                    }
                    else
                    {
                        value.Value = valueToReturn;
                    }
                }

                // apply the data encoding.
                if (!QualifiedName.IsNull(dataEncoding))
                {
                    value.Value = null;
                    value.StatusCode = StatusCodes.BadDataEncodingUnsupported;
                }
            }

            // apply the timestamps filter.
            if (timestampsToReturn == TimestampsToReturn.Neither || timestampsToReturn == TimestampsToReturn.Server)
            {
                value.SourceTimestamp = DateTime.MinValue;
            }

            if (timestampsToReturn == TimestampsToReturn.Neither || timestampsToReturn == TimestampsToReturn.Source)
            {
                value.ServerTimestamp = DateTime.MinValue;
            }

            // add result.
            values.Add(value);
        }
        #endregion

        #region Private Fields
        private Guid m_id;
        private NodeId m_variableId;
        private IHistoryDataSource m_source;
        private ReadRawModifiedDetails m_request;
        private DateTime m_startTime;
        private DateTime m_endTime;
        private bool m_isForward;
        private int m_position;
        private DateTime m_lastTime;
        #endregion
    }
}
