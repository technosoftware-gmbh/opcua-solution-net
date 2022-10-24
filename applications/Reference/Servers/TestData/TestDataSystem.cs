#region Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2022 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;

using Opc.Ua;

using Technosoftware.UaServer;
#endregion

namespace TestData
{
    public interface ITestDataSystemCallback
    {
        void OnDataChange(
            BaseVariableState variable,
            object value,
            StatusCode statusCode,
            DateTime timestamp);
    }

    public class TestDataSystem
    {
        public TestDataSystem(ITestDataSystemCallback callback, NamespaceTable namespaceUris, StringTable serverUris)
        {
            callback_ = callback;
            minimumSamplingInterval_ = Int32.MaxValue;
            monitoredNodes_ = new Dictionary<uint, BaseVariableState>();
            generator_ = new Opc.Ua.Test.DataGenerator(null);
            generator_.NamespaceUris = namespaceUris;
            generator_.ServerUris = serverUris;
            historyArchive_ = new HistoryArchive();
        }

        /// <summary>
        /// The number of nodes being monitored.
        /// </summary>
        public int MonitoredNodeCount
        {
            get
            {
                lock (lock_)
                {
                    if (monitoredNodes_ == null)
                    {
                        return 0;
                    }

                    return monitoredNodes_.Count;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current system status.
        /// </summary>
        public StatusCode SystemStatus
        {
            get
            {
                lock (lock_)
                {
                    return systemStatus_;
                }
            }

            set
            {
                lock (lock_)
                {
                    systemStatus_ = value;
                }
            }
        }

        /// <summary>
        /// Creates an archive for the variable.
        /// </summary>
        public void EnableHistoryArchiving(BaseVariableState variable)
        {
            if (variable == null)
            {
                return;
            }

            if (variable.ValueRank == ValueRanks.Scalar)
            {
                historyArchive_.CreateRecord(variable.NodeId, TypeInfo.GetBuiltInType(variable.DataType));
            }
        }

        /// <summary>
        /// Returns the history file for the variable.
        /// </summary>
        public IHistoryDataSource GetHistoryFile(BaseVariableState variable)
        {
            if (variable == null)
            {
                return null;
            }

            return historyArchive_.GetHistoryFile(variable.NodeId);
        }

        /// <summary>
        /// Returns a new value for the variable.
        /// </summary>
        public object ReadValue(BaseVariableState variable)
        {
            lock (lock_)
            {
                switch (variable.NumericId)
                {
                    case TestData.Variables.ScalarValueObjectType_BooleanValue:
                    case TestData.Variables.UserScalarValueObjectType_BooleanValue:
                    {
                        return generator_.GetRandom<bool>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_SByteValue:
                    case TestData.Variables.UserScalarValueObjectType_SByteValue:
                    {
                        return generator_.GetRandom<sbyte>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_SByteValue:
                    {
                        return (sbyte)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                    }

                    case TestData.Variables.ScalarValueObjectType_ByteValue:
                    case TestData.Variables.UserScalarValueObjectType_ByteValue:
                    {
                        return generator_.GetRandom<byte>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_ByteValue:
                    {
                        return (byte)((generator_.GetRandom<uint>(false) % 201) + 50);
                    }

                    case TestData.Variables.ScalarValueObjectType_Int16Value:
                    case TestData.Variables.UserScalarValueObjectType_Int16Value:
                    {
                        return generator_.GetRandom<short>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_Int16Value:
                    {
                        return (short)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                    }

                    case TestData.Variables.ScalarValueObjectType_UInt16Value:
                    case TestData.Variables.UserScalarValueObjectType_UInt16Value:
                    {
                        return generator_.GetRandom<ushort>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_UInt16Value:
                    {
                        return (ushort)((generator_.GetRandom<uint>(false) % 201) + 50);
                    }

                    case TestData.Variables.ScalarValueObjectType_Int32Value:
                    case TestData.Variables.UserScalarValueObjectType_Int32Value:
                    {
                        return generator_.GetRandom<int>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_Int32Value:
                    case TestData.Variables.AnalogScalarValueObjectType_IntegerValue:
                    {
                        return (int)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                    }

                    case TestData.Variables.ScalarValueObjectType_UInt32Value:
                    case TestData.Variables.UserScalarValueObjectType_UInt32Value:
                    {
                        return generator_.GetRandom<uint>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_UInt32Value:
                    case TestData.Variables.AnalogScalarValueObjectType_UIntegerValue:
                    {
                        return (uint)((generator_.GetRandom<uint>(false) % 201) + 50);
                    }

                    case TestData.Variables.ScalarValueObjectType_Int64Value:
                    case TestData.Variables.UserScalarValueObjectType_Int64Value:
                    {
                        return generator_.GetRandom<long>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_Int64Value:
                    {
                        return (long)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                    }

                    case TestData.Variables.ScalarValueObjectType_UInt64Value:
                    case TestData.Variables.UserScalarValueObjectType_UInt64Value:
                    {
                        return generator_.GetRandom<ulong>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_UInt64Value:
                    {
                        return (ulong)((generator_.GetRandom<uint>(false) % 201) + 50);
                    }

                    case TestData.Variables.ScalarValueObjectType_FloatValue:
                    case TestData.Variables.UserScalarValueObjectType_FloatValue:
                    {
                        return generator_.GetRandom<float>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_FloatValue:
                    {
                        return (float)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                    }

                    case TestData.Variables.ScalarValueObjectType_DoubleValue:
                    case TestData.Variables.UserScalarValueObjectType_DoubleValue:
                    {
                        return generator_.GetRandom<double>(false);
                    }

                    case TestData.Variables.AnalogScalarValueObjectType_DoubleValue:
                    case TestData.Variables.AnalogScalarValueObjectType_NumberValue:
                    {
                        return (double)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                    }

                    case TestData.Variables.ScalarValueObjectType_StringValue:
                    case TestData.Variables.UserScalarValueObjectType_StringValue:
                    {
                        return generator_.GetRandom<string>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_DateTimeValue:
                    case TestData.Variables.UserScalarValueObjectType_DateTimeValue:
                    {
                        return generator_.GetRandom<DateTime>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_GuidValue:
                    case TestData.Variables.UserScalarValueObjectType_GuidValue:
                    {
                        return generator_.GetRandom<Guid>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_ByteStringValue:
                    case TestData.Variables.UserScalarValueObjectType_ByteStringValue:
                    {
                        return generator_.GetRandom<byte[]>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_XmlElementValue:
                    case TestData.Variables.UserScalarValueObjectType_XmlElementValue:
                    {
                        return generator_.GetRandom<XmlElement>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_NodeIdValue:
                    case TestData.Variables.UserScalarValueObjectType_NodeIdValue:
                    {
                        return generator_.GetRandom<Opc.Ua.NodeId>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_ExpandedNodeIdValue:
                    case TestData.Variables.UserScalarValueObjectType_ExpandedNodeIdValue:
                    {
                        return generator_.GetRandom<ExpandedNodeId>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_QualifiedNameValue:
                    case TestData.Variables.UserScalarValueObjectType_QualifiedNameValue:
                    {
                        return generator_.GetRandom<QualifiedName>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_LocalizedTextValue:
                    case TestData.Variables.UserScalarValueObjectType_LocalizedTextValue:
                    {
                        return generator_.GetRandom<LocalizedText>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_StatusCodeValue:
                    case TestData.Variables.UserScalarValueObjectType_StatusCodeValue:
                    {
                        return generator_.GetRandom<StatusCode>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_VariantValue:
                    case TestData.Variables.UserScalarValueObjectType_VariantValue:
                    {
                        return generator_.GetRandomVariant(false).Value;
                    }

                    case TestData.Variables.ScalarValueObjectType_StructureValue:
                    {
                        return GetRandomStructure();
                    }

                    case TestData.Variables.ScalarValueObjectType_EnumerationValue:
                    {
                        return generator_.GetRandom<int>(false);
                    }

                    case TestData.Variables.ScalarValueObjectType_NumberValue:
                    {
                        return generator_.GetRandom(BuiltInType.Number);
                    }

                    case TestData.Variables.ScalarValueObjectType_IntegerValue:
                    {
                        return generator_.GetRandom(BuiltInType.Integer);
                    }

                    case TestData.Variables.ScalarValueObjectType_UIntegerValue:
                    {
                        return generator_.GetRandom(BuiltInType.UInteger);
                    }

                    case TestData.Variables.ArrayValueObjectType_BooleanValue:
                    case TestData.Variables.UserArrayValueObjectType_BooleanValue:
                    {
                        return generator_.GetRandomArray<bool>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_SByteValue:
                    case TestData.Variables.UserArrayValueObjectType_SByteValue:
                    {
                        return generator_.GetRandomArray<sbyte>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_SByteValue:
                    {
                        sbyte[] values = generator_.GetRandomArray<sbyte>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (sbyte)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_ByteValue:
                    case TestData.Variables.UserArrayValueObjectType_ByteValue:
                    {
                        return generator_.GetRandomArray<byte>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_ByteValue:
                    {
                        byte[] values = generator_.GetRandomArray<byte>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (byte)((generator_.GetRandom<uint>(false) % 201) + 50);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_Int16Value:
                    case TestData.Variables.UserArrayValueObjectType_Int16Value:
                    {
                        return generator_.GetRandomArray<short>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_Int16Value:
                    {
                        short[] values = generator_.GetRandomArray<short>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (short)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_UInt16Value:
                    case TestData.Variables.UserArrayValueObjectType_UInt16Value:
                    {
                        return generator_.GetRandomArray<ushort>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_UInt16Value:
                    {
                        ushort[] values = generator_.GetRandomArray<ushort>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (ushort)((generator_.GetRandom<uint>(false) % 201) + 50);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_Int32Value:
                    case TestData.Variables.UserArrayValueObjectType_Int32Value:
                    {
                        return generator_.GetRandomArray<int>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_Int32Value:
                    case TestData.Variables.AnalogArrayValueObjectType_IntegerValue:
                    {
                        int[] values = generator_.GetRandomArray<int>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (int)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_UInt32Value:
                    case TestData.Variables.UserArrayValueObjectType_UInt32Value:
                    {
                        return generator_.GetRandomArray<uint>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_UInt32Value:
                    case TestData.Variables.AnalogArrayValueObjectType_UIntegerValue:
                    {
                        uint[] values = generator_.GetRandomArray<uint>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (uint)((generator_.GetRandom<uint>(false) % 201) + 50);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_Int64Value:
                    case TestData.Variables.UserArrayValueObjectType_Int64Value:
                    {
                        return generator_.GetRandomArray<long>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_Int64Value:
                    {
                        long[] values = generator_.GetRandomArray<long>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (long)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_UInt64Value:
                    case TestData.Variables.UserArrayValueObjectType_UInt64Value:
                    {
                        return generator_.GetRandomArray<ulong>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_UInt64Value:
                    {
                        ulong[] values = generator_.GetRandomArray<ulong>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (ulong)((generator_.GetRandom<uint>(false) % 201) + 50);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_FloatValue:
                    case TestData.Variables.UserArrayValueObjectType_FloatValue:
                    {
                        return generator_.GetRandomArray<float>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_FloatValue:
                    {
                        float[] values = generator_.GetRandomArray<float>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (float)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_DoubleValue:
                    case TestData.Variables.UserArrayValueObjectType_DoubleValue:
                    {
                        return generator_.GetRandomArray<double>(false, 100, false);
                    }

                    case TestData.Variables.AnalogArrayValueObjectType_DoubleValue:
                    case TestData.Variables.AnalogArrayValueObjectType_NumberValue:
                    {
                        double[] values = generator_.GetRandomArray<double>(false, 100, false);

                        for (int ii = 0; ii < values.Length; ii++)
                        {
                            values[ii] = (double)(((int)(generator_.GetRandom<uint>(false) % 201)) - 100);
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_StringValue:
                    case TestData.Variables.UserArrayValueObjectType_StringValue:
                    {
                        return generator_.GetRandomArray<string>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_DateTimeValue:
                    case TestData.Variables.UserArrayValueObjectType_DateTimeValue:
                    {
                        return generator_.GetRandomArray<DateTime>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_GuidValue:
                    case TestData.Variables.UserArrayValueObjectType_GuidValue:
                    {
                        return generator_.GetRandomArray<Guid>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_ByteStringValue:
                    case TestData.Variables.UserArrayValueObjectType_ByteStringValue:
                    {
                        return generator_.GetRandomArray<byte[]>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_XmlElementValue:
                    case TestData.Variables.UserArrayValueObjectType_XmlElementValue:
                    {
                        return generator_.GetRandomArray<XmlElement>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_NodeIdValue:
                    case TestData.Variables.UserArrayValueObjectType_NodeIdValue:
                    {
                        return generator_.GetRandomArray<Opc.Ua.NodeId>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_ExpandedNodeIdValue:
                    case TestData.Variables.UserArrayValueObjectType_ExpandedNodeIdValue:
                    {
                        return generator_.GetRandomArray<ExpandedNodeId>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_QualifiedNameValue:
                    case TestData.Variables.UserArrayValueObjectType_QualifiedNameValue:
                    {
                        return generator_.GetRandomArray<QualifiedName>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_LocalizedTextValue:
                    case TestData.Variables.UserArrayValueObjectType_LocalizedTextValue:
                    {
                        return generator_.GetRandomArray<LocalizedText>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_StatusCodeValue:
                    case TestData.Variables.UserArrayValueObjectType_StatusCodeValue:
                    {
                        return generator_.GetRandomArray<StatusCode>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_VariantValue:
                    case TestData.Variables.UserArrayValueObjectType_VariantValue:
                    {
                        return generator_.GetRandomArray<object>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_StructureValue:
                    {
                        ExtensionObject[] values = generator_.GetRandomArray<ExtensionObject>(false, 10, false);

                        for (int ii = 0; values != null && ii < values.Length; ii++)
                        {
                            values[ii] = GetRandomStructure();
                        }

                        return values;
                    }

                    case TestData.Variables.ArrayValueObjectType_EnumerationValue:
                    {
                        return generator_.GetRandomArray<int>(false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_NumberValue:
                    {
                        return generator_.GetRandomArray(BuiltInType.Number, false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_IntegerValue:
                    {
                        return generator_.GetRandomArray(BuiltInType.Integer, false, 100, false);
                    }

                    case TestData.Variables.ArrayValueObjectType_UIntegerValue:
                    {
                        return generator_.GetRandomArray(BuiltInType.UInteger, false, 100, false);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns a random structure.
        /// </summary>
        private ExtensionObject GetRandomStructure()
        {
            if (generator_.GetRandomBoolean())
            {
                ScalarValueDataType value = new ScalarValueDataType();

                value.BooleanValue = generator_.GetRandom<bool>(false);
                value.SByteValue = generator_.GetRandom<sbyte>(false);
                value.ByteValue = generator_.GetRandom<byte>(false);
                value.Int16Value = generator_.GetRandom<short>(false);
                value.UInt16Value = generator_.GetRandom<ushort>(false);
                value.Int32Value = generator_.GetRandom<int>(false);
                value.UInt32Value = generator_.GetRandom<uint>(false);
                value.Int64Value = generator_.GetRandom<long>(false);
                value.UInt64Value = generator_.GetRandom<ulong>(false);
                value.FloatValue = generator_.GetRandom<float>(false);
                value.DoubleValue = generator_.GetRandom<double>(false);
                value.StringValue = generator_.GetRandom<string>(false);
                value.DateTimeValue = generator_.GetRandom<DateTime>(false);
                value.GuidValue = generator_.GetRandom<Uuid>(false);
                value.ByteStringValue = generator_.GetRandom<byte[]>(false);
                value.XmlElementValue = generator_.GetRandom<XmlElement>(false);
                value.NodeIdValue = generator_.GetRandom<Opc.Ua.NodeId>(false);
                value.ExpandedNodeIdValue = generator_.GetRandom<ExpandedNodeId>(false);
                value.QualifiedNameValue = generator_.GetRandom<QualifiedName>(false);
                value.LocalizedTextValue = generator_.GetRandom<LocalizedText>(false);
                value.StatusCodeValue = generator_.GetRandom<StatusCode>(false);
                value.VariantValue = generator_.GetRandomVariant(false);

                return new ExtensionObject(value.TypeId, value);
            }
            else
            {
                ArrayValueDataType value = new ArrayValueDataType();

                value.BooleanValue = generator_.GetRandomArray<bool>(false, 10, false);
                value.SByteValue = generator_.GetRandomArray<sbyte>(false, 10, false);
                value.ByteValue = generator_.GetRandomArray<byte>(false, 10, false);
                value.Int16Value = generator_.GetRandomArray<short>(false, 10, false);
                value.UInt16Value = generator_.GetRandomArray<ushort>(false, 10, false);
                value.Int32Value = generator_.GetRandomArray<int>(false, 10, false);
                value.UInt32Value = generator_.GetRandomArray<uint>(false, 10, false);
                value.Int64Value = generator_.GetRandomArray<long>(false, 10, false);
                value.UInt64Value = generator_.GetRandomArray<ulong>(false, 10, false);
                value.FloatValue = generator_.GetRandomArray<float>(false, 10, false);
                value.DoubleValue = generator_.GetRandomArray<double>(false, 10, false);
                value.StringValue = generator_.GetRandomArray<string>(false, 10, false);
                value.DateTimeValue = generator_.GetRandomArray<DateTime>(false, 10, false);
                value.GuidValue = generator_.GetRandomArray<Uuid>(false, 10, false);
                value.ByteStringValue = generator_.GetRandomArray<byte[]>(false, 10, false);
                value.XmlElementValue = generator_.GetRandomArray<XmlElement>(false, 10, false);
                value.NodeIdValue = generator_.GetRandomArray<Opc.Ua.NodeId>(false, 10, false);
                value.ExpandedNodeIdValue = generator_.GetRandomArray<ExpandedNodeId>(false, 10, false);
                value.QualifiedNameValue = generator_.GetRandomArray<QualifiedName>(false, 10, false);
                value.LocalizedTextValue = generator_.GetRandomArray<LocalizedText>(false, 10, false);
                value.StatusCodeValue = generator_.GetRandomArray<StatusCode>(false, 10, false);

                object[] values = generator_.GetRandomArray<object>(false, 10, false);

                for (int ii = 0; values != null && ii < values.Length; ii++)
                {
                    value.VariantValue.Add(new Variant(values[ii]));
                }

                return new ExtensionObject(value.TypeId, value);
            }
        }

        public void StartMonitoringValue(uint monitoredItemId, double samplingInterval, BaseVariableState variable)
        {
            lock (lock_)
            {
                if (monitoredNodes_ == null)
                {
                    monitoredNodes_ = new Dictionary<uint, BaseVariableState>();
                }

                monitoredNodes_[monitoredItemId] = variable;

                SetSamplingInterval(samplingInterval);
            }
        }

        public void SetSamplingInterval(double samplingInterval)
        {
            lock (lock_)
            {
                if (samplingInterval < 0)
                {
                    // m_samplingEvent.Set();
                    minimumSamplingInterval_ = Int32.MaxValue;

                    if (timer_ != null)
                    {
                        timer_.Dispose();
                        timer_ = null;
                    }

                    return;
                }

                if (minimumSamplingInterval_ > samplingInterval)
                {
                    minimumSamplingInterval_ = (int)samplingInterval;

                    if (minimumSamplingInterval_ < 100)
                    {
                        minimumSamplingInterval_ = 100;
                    }

                    if (timer_ != null)
                    {
                        timer_.Dispose();
                        timer_ = null;
                    }

                    timer_ = new Timer(DoSample, null, minimumSamplingInterval_, minimumSamplingInterval_);
                }
            }
        }

        void DoSample(object state)
        {
            Utils.LogTrace("DoSample HiRes={0:ss.ffff} Now={1:ss.ffff}", HiResClock.UtcNow, DateTime.UtcNow);

            Queue<Sample> samples = new Queue<Sample>();

            lock (lock_)
            {
                if (monitoredNodes_ == null)
                {
                    return;
                }

                foreach (BaseVariableState variable in monitoredNodes_.Values)
                {
                    Sample sample = new Sample();

                    sample.Variable = variable;
                    sample.Value = ReadValue(sample.Variable);
                    sample.StatusCode = StatusCodes.Good;
                    sample.Timestamp = DateTime.UtcNow;

                    samples.Enqueue(sample);
                }
            }

            while (samples.Count > 0)
            {
                Sample sample = samples.Dequeue();

                callback_.OnDataChange(
                    sample.Variable,
                    sample.Value,
                    sample.StatusCode,
                    sample.Timestamp);
            }
        }

        public void StopMonitoringValue(uint monitoredItemId)
        {
            lock (lock_)
            {
                if (monitoredNodes_ == null)
                {
                    return;
                }

                monitoredNodes_.Remove(monitoredItemId);

                if (monitoredNodes_.Count == 0)
                {
                    SetSamplingInterval(-1);
                }
            }
        }

        private class Sample
        {
            public BaseVariableState Variable;
            public object Value;
            public StatusCode StatusCode;
            public DateTime Timestamp;
        }

        #region Private Fields
        private object lock_ = new object();
        private ITestDataSystemCallback callback_;
        private Opc.Ua.Test.DataGenerator generator_;
        private int minimumSamplingInterval_;
        private Dictionary<uint, BaseVariableState> monitoredNodes_;
        private Timer timer_;
        private StatusCode systemStatus_;
        private HistoryArchive historyArchive_;
        #endregion
    }
}
