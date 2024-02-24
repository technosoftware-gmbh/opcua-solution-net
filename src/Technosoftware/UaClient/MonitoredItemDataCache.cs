#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
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
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives
using System.Collections.Generic;

using Opc.Ua;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// An item in the cache
    /// </summary>
    public class MonitoredItemDataCache
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Constructs a cache for a monitored item.
        /// </summary>
        public MonitoredItemDataCache(int queueSize)
        {
            QueueSize = queueSize;
            values_ = new Queue<DataValue>();
        }
        #endregion

        #region Public Members
        /// <summary>
        /// The size of the queue to maintain.
        /// </summary>
        public int QueueSize { get; private set; }

        /// <summary>
        /// The last value received from the server.
        /// </summary>
        public DataValue LastValue { get; private set; }

        /// <summary>
        /// Returns all values in the queue.
        /// </summary>
        public IList<DataValue> Publish()
        {
            var values = new DataValue[values_.Count];

            for (var ii = 0; ii < values.Length; ii++)
            {
                values[ii] = values_.Dequeue();
            }

            return values;
        }

        /// <summary>
        /// Saves a notification in the cache.
        /// </summary>
        public void OnNotification(MonitoredItemNotification notification)
        {
            values_.Enqueue(notification.Value);
            LastValue = notification.Value;

            UaClientUtils.EventLog.NotificationValue(notification.ClientHandle, LastValue.WrappedValue);


            while (values_.Count > QueueSize)
            {
                _ = values_.Dequeue();
            }
        }

        /// <summary>
        /// Changes the queue size.
        /// </summary>
        public void SetQueueSize(int queueSize)
        {
            if (queueSize == QueueSize)
            {
                return;
            }

            if (queueSize < 1)
            {
                queueSize = 1;
            }

            QueueSize = queueSize;

            while (values_.Count > QueueSize)
            {
                _ = values_.Dequeue();
            }
        }
        #endregion

        #region Private Fields
        private readonly Queue<DataValue> values_;
        #endregion
    }
}
