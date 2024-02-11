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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;

using Opc.Ua;
#endregion

namespace SampleCompany.NodeManagers.TestData
{
    /// <summary>
    /// A class that provides access to archived data.
    /// </summary>
    internal class HistoryArchive : IDisposable
    {
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
            if (disposing)
            {
                if (m_updateTimer != null)
                {
                    m_updateTimer.Dispose();
                    m_updateTimer = null;
                }
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Returns an object that can be used to browse the archive.
        /// </summary>
        public HistoryFile GetHistoryFile(NodeId nodeId)
        {
            lock (m_lock)
            {
                if (m_records == null)
                {
                    return null;
                }

                HistoryRecord record = null;

                if (!m_records.TryGetValue(nodeId, out record))
                {
                    return null;
                }

                return new HistoryFile(m_lock, record.RawData);
            }
        }

        /// <summary>
        /// Creates a new record in the archive.
        /// </summary>
        public void CreateRecord(NodeId nodeId, BuiltInType dataType)
        {
            lock (m_lock)
            {
                var record = new HistoryRecord {
                    RawData = new List<HistoryEntry>(),
                    Historizing = true,
                    DataType = dataType
                };

                DateTime now = DateTime.UtcNow;

                for (var ii = 1000; ii >= 0; ii--)
                {
                    var entry = new HistoryEntry {
                        Value = new DataValue {
                            ServerTimestamp = now.AddSeconds(-(ii * 10))
                        }
                    };
                    entry.Value.SourceTimestamp = entry.Value.ServerTimestamp.AddMilliseconds(1234);
                    entry.IsModified = false;

                    switch (dataType)
                    {
                        case BuiltInType.Int32:
                        {
                            entry.Value.Value = ii;
                            break;
                        }
                    }

                    record.RawData.Add(entry);
                }

                if (m_records == null)
                {
                    m_records = new Dictionary<NodeId, HistoryRecord>();
                }

                m_records[nodeId] = record;

                if (m_updateTimer == null)
                {
                    m_updateTimer = new Timer(OnUpdate, null, 10000, 10000);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Periodically adds new values into the archive.
        /// </summary>
        private void OnUpdate(object state)
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                lock (m_lock)
                {
                    foreach (HistoryRecord record in m_records.Values)
                    {
                        if (!record.Historizing || record.RawData.Count >= 2000)
                        {
                            continue;
                        }

                        var entry = new HistoryEntry {
                            Value = new DataValue {
                                ServerTimestamp = now
                            }
                        };
                        entry.Value.SourceTimestamp = entry.Value.ServerTimestamp.AddMilliseconds(-4567);
                        entry.IsModified = false;

                        switch (record.DataType)
                        {
                            case BuiltInType.Int32:
                            {
                                var lastValue = (int)record.RawData[record.RawData.Count - 1].Value.Value;
                                entry.Value.Value = lastValue + 1;
                                break;
                            }
                        }

                        record.RawData.Add(entry);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Unexpected error updating history.");
            }
        }
        #endregion

        #region Private Fields
        private readonly object m_lock = new object();
        private Timer m_updateTimer;
        private Dictionary<NodeId, HistoryRecord> m_records;
        #endregion
    }

    #region HistoryEntry Class
    /// <summary>
    /// A single entry in the archive.
    /// </summary>
    internal class HistoryEntry
    {
        public DataValue Value;
        public bool IsModified;
    }
    #endregion

    #region HistoryRecord Class
    /// <summary>
    /// A record in the archive.
    /// </summary>
    internal class HistoryRecord
    {
        public List<HistoryEntry> RawData;
        public bool Historizing;
        public BuiltInType DataType;
    }
    #endregion
}
