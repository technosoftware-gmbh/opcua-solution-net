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

using System;
using System.Collections.Generic;
using System.Data;

using Opc.Ua;

using Technosoftware.UaServer;

#endregion

namespace SampleCompany.SampleServer.UnderlyingSystem
{
    /// <summary>
    /// Stores the metadata for a node representing an item in the archive.
    /// </summary>
    public class ArchiveItemState : Opc.Ua.DataItemState
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Creates a new instance of a item.
        /// </summary>
        public ArchiveItemState(ISystemContext context, ArchiveItem item, ushort namespaceIndex)
        : 
            base(null)
        {
            ArchiveItem = item;

            this.TypeDefinitionId = VariableTypeIds.DataItemType;
            this.SymbolicName = ArchiveItem.Name;
            this.NodeId = ConstructId(ArchiveItem.UniquePath, namespaceIndex);
            this.BrowseName = new QualifiedName(ArchiveItem.Name, namespaceIndex);
            this.DisplayName = new LocalizedText(this.BrowseName.Name);
            this.Description = null;
            this.WriteMask = 0;
            this.UserWriteMask = 0;
            this.DataType = DataTypeIds.BaseDataType;
            this.ValueRank = ValueRanks.Scalar;
            this.AccessLevel = AccessLevels.HistoryReadOrWrite | AccessLevels.CurrentRead;
            this.UserAccessLevel = AccessLevels.HistoryReadOrWrite | AccessLevels.CurrentRead;
            this.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
            this.Historizing = true;

            annotations_ = new PropertyState<Annotation>(this);
            annotations_.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            annotations_.TypeDefinitionId = VariableTypeIds.PropertyType;
            annotations_.SymbolicName = Opc.Ua.BrowseNames.Annotations;
            annotations_.BrowseName = Opc.Ua.BrowseNames.Annotations;
            annotations_.DisplayName = new LocalizedText(annotations_.BrowseName.Name);
            annotations_.Description = null;
            annotations_.WriteMask = 0;
            annotations_.UserWriteMask = 0;
            annotations_.DataType = DataTypeIds.Annotation;
            annotations_.ValueRank = ValueRanks.Scalar;
            annotations_.AccessLevel = AccessLevels.HistoryReadOrWrite;
            annotations_.UserAccessLevel = AccessLevels.HistoryReadOrWrite;
            annotations_.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
            annotations_.Historizing = false;
            this.AddChild(annotations_);

            annotations_.NodeId = NodeTypes.ConstructIdForComponent(annotations_, namespaceIndex);

            configuration_ = new HistoricalDataConfigurationState(this);
            configuration_.MaxTimeInterval = new PropertyState<double>(configuration_);
            configuration_.MinTimeInterval = new PropertyState<double>(configuration_); ;
            configuration_.StartOfArchive = new PropertyState<DateTime>(configuration_);
            configuration_.StartOfOnlineArchive = new PropertyState<DateTime>(configuration_);

            configuration_.Create(
                context,
                null,
                Opc.Ua.BrowseNames.HAConfiguration,
                null,
                true);

            configuration_.SymbolicName = Opc.Ua.BrowseNames.HAConfiguration;
            configuration_.ReferenceTypeId = ReferenceTypeIds.HasHistoricalConfiguration;

            this.AddChild(configuration_);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        public void LoadConfiguration(ISystemContext context)
        {
            DataFileReader reader = new DataFileReader();

            if (reader.LoadConfiguration(context, ArchiveItem))
            {
                this.DataType = (uint)ArchiveItem.DataType;
                this.ValueRank = ArchiveItem.ValueRank;
                this.Historizing = ArchiveItem.Archiving;

                configuration_.MinTimeInterval.Value = ArchiveItem.SamplingInterval;
                configuration_.MaxTimeInterval.Value = ArchiveItem.SamplingInterval;
                configuration_.Stepped.Value = ArchiveItem.Stepped;

                AggregateConfiguration configuration = ArchiveItem.AggregateConfiguration;
                configuration_.AggregateConfiguration.PercentDataGood.Value = configuration.PercentDataGood;
                configuration_.AggregateConfiguration.PercentDataBad.Value = configuration.PercentDataBad;
                configuration_.AggregateConfiguration.UseSlopedExtrapolation.Value = configuration.UseSlopedExtrapolation;
                configuration_.AggregateConfiguration.TreatUncertainAsBad.Value = configuration.TreatUncertainAsBad;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public void ReloadFromSource(ISystemContext context)
        {
            LoadConfiguration(context);

            if (ArchiveItem.LastLoadTime == DateTime.MinValue || (ArchiveItem.Persistent && ArchiveItem.LastLoadTime.AddSeconds(10) < DateTime.UtcNow))
            {
                DataFileReader reader = new DataFileReader();
                reader.LoadHistoryData(context, ArchiveItem);

                // set the start of the archive.
                if (ArchiveItem.DataSet.Tables[0].DefaultView.Count > 0)
                {
                    configuration_.StartOfArchive.Value = (DateTime)ArchiveItem.DataSet.Tables[0].DefaultView[0].Row[0];
                    configuration_.StartOfOnlineArchive.Value = configuration_.StartOfArchive.Value;
                }

                if (ArchiveItem.Archiving)
                {
                    // save the pattern used to produce new data.
                    pattern_ = new List<DataValue>();

                    foreach (DataRowView row in ArchiveItem.DataSet.Tables[0].DefaultView)
                    {
                        DataValue value = (DataValue)row.Row[2];
                        pattern_.Add(value);
                        nextSampleTime_ = value.SourceTimestamp.AddMilliseconds(ArchiveItem.SamplingInterval);
                    }

                    // fill in data until the present time.
                    patternIndex_ = 0;
                    NewSamples(context);
                }
            }

            
        }

        /// <summary>
        /// Creates a new sample.
        /// </summary>
        public List<DataValue> NewSamples(ISystemContext context)
        {
            List<DataValue> newSamples = new List<DataValue>();

            while (pattern_ != null && nextSampleTime_ < DateTime.UtcNow)
            {
                DataValue value = new DataValue();
                value.WrappedValue = pattern_[patternIndex_].WrappedValue;
                value.ServerTimestamp = nextSampleTime_;
                value.SourceTimestamp = nextSampleTime_;
                value.StatusCode = pattern_[patternIndex_].StatusCode;
                nextSampleTime_ = value.SourceTimestamp.AddMilliseconds(ArchiveItem.SamplingInterval);
                newSamples.Add(value);

                DataRow row = ArchiveItem.DataSet.Tables[0].NewRow();

                row[0] = value.SourceTimestamp;
                row[1] = value.ServerTimestamp;
                row[2] = value;
                row[3] = value.WrappedValue.TypeInfo.BuiltInType;
                row[4] = value.WrappedValue.TypeInfo.ValueRank;

                ArchiveItem.DataSet.Tables[0].Rows.Add(row);
                patternIndex_ = (patternIndex_ + 1) % pattern_.Count;
            }

            ArchiveItem.DataSet.AcceptChanges();
            return newSamples;
        }

        /// <summary>
        /// Updates the history.
        /// </summary>
        public uint UpdateHistory(SystemContext context, DataValue value, PerformUpdateType performUpdateType)
        {
            bool replaced = false;

            if (performUpdateType == PerformUpdateType.Remove)
            {
                return StatusCodes.BadNotSupported;
            }

            if (StatusCode.IsNotBad(value.StatusCode))
            {
                TypeInfo typeInfo = value.WrappedValue.TypeInfo;

                if (typeInfo == null)
                {
                    typeInfo = TypeInfo.Construct(value.Value);
                }

                if (typeInfo == null || typeInfo.BuiltInType != ArchiveItem.DataType || typeInfo.ValueRank != ValueRanks.Scalar)
                {
                    return StatusCodes.BadTypeMismatch;
                }
            }

            string filter = String.Format(System.Globalization.CultureInfo.InvariantCulture, "SourceTimestamp = #{0}#", value.SourceTimestamp);

            DataView view = new DataView(
                ArchiveItem.DataSet.Tables[0],
                filter,
                null,
                DataViewRowState.CurrentRows);

            DataRow row = null;

            for (int ii = 0; ii < view.Count;)
            {
                if (performUpdateType == PerformUpdateType.Insert)
                {
                    return StatusCodes.BadEntryExists;
                }

                // add record indicating it was replaced.
                DataRow modifiedRow = ArchiveItem.DataSet.Tables[1].NewRow();

                modifiedRow[0] = view[ii].Row[0];
                modifiedRow[1] = view[ii].Row[1];
                modifiedRow[2] = view[ii].Row[2];
                modifiedRow[3] = view[ii].Row[3];
                modifiedRow[4] = view[ii].Row[4];
                modifiedRow[5] = HistoryUpdateType.Replace;
                modifiedRow[6] = GetModificationInfo(context, HistoryUpdateType.Replace);

                ArchiveItem.DataSet.Tables[1].Rows.Add(modifiedRow);

                replaced = true;
                row = view[ii].Row;
                break;
            }

            // add record indicating it was inserted.
            if (!replaced)
            {
                if (performUpdateType == PerformUpdateType.Replace)
                {
                    return StatusCodes.BadNoEntryExists;
                }

                DataRow modifiedRow = ArchiveItem.DataSet.Tables[1].NewRow();

                modifiedRow[0] = value.SourceTimestamp;
                modifiedRow[1] = value.ServerTimestamp;
                modifiedRow[2] = value;

                if (value.WrappedValue.TypeInfo != null)
                {
                    modifiedRow[3] = value.WrappedValue.TypeInfo.BuiltInType;
                    modifiedRow[4] = value.WrappedValue.TypeInfo.ValueRank;
                }
                else
                {
                    modifiedRow[3] = BuiltInType.Variant;
                    modifiedRow[4] = ValueRanks.Scalar;
                }

                modifiedRow[5] = HistoryUpdateType.Insert;
                modifiedRow[6] = GetModificationInfo(context, HistoryUpdateType.Insert);

                ArchiveItem.DataSet.Tables[1].Rows.Add(modifiedRow);

                row = ArchiveItem.DataSet.Tables[0].NewRow();
            }

            // add/update new record.
            row[0] = value.SourceTimestamp;
            row[1] = value.ServerTimestamp;
            row[2] = value;

            if (value.WrappedValue.TypeInfo != null)
            {
                row[3] = value.WrappedValue.TypeInfo.BuiltInType;
                row[4] = value.WrappedValue.TypeInfo.ValueRank;
            }
            else
            {
                row[3] = BuiltInType.Variant;
                row[4] = ValueRanks.Scalar;
            }

            if (!replaced)
            {
                ArchiveItem.DataSet.Tables[0].Rows.Add(row);
            }

            // accept all changes.
            ArchiveItem.DataSet.AcceptChanges();

            return StatusCodes.Good;
        }

        /// <summary>
        /// Updates the history.
        /// </summary>
        public uint UpdateAnnotations(SystemContext context, Annotation annotation, DataValue value, PerformUpdateType performUpdateType)
        {
            bool replaced = false;

            string filter = String.Format(System.Globalization.CultureInfo.InvariantCulture, "SourceTimestamp = #{0}#", value.SourceTimestamp);

            DataView view = new DataView(
                ArchiveItem.DataSet.Tables[2],
                filter,
                null,
                DataViewRowState.CurrentRows);

            DataRow row = null;

            for (int ii = 0; ii < view.Count; ii++)
            {
                Annotation current = (Annotation)view[ii].Row[5];

                replaced = (current.UserName == annotation.UserName);

                if (performUpdateType == PerformUpdateType.Insert)
                {
                    if (replaced)
                    {
                        return StatusCodes.BadEntryExists;
                    }
                }

                if (replaced)
                {
                    row = view[ii].Row;
                    break;
                }
            }

            // add record indicating it was inserted.
            if (!replaced)
            {
                if (performUpdateType == PerformUpdateType.Replace || performUpdateType == PerformUpdateType.Remove)
                {
                    return StatusCodes.BadNoEntryExists;
                }

                row = ArchiveItem.DataSet.Tables[2].NewRow();
            }

            // add/update new record.
            if (performUpdateType != PerformUpdateType.Remove)
            {
                row[0] = value.SourceTimestamp;
                row[1] = value.ServerTimestamp;
                row[2] = new DataValue(new ExtensionObject(annotation), StatusCodes.Good, value.SourceTimestamp, value.ServerTimestamp);
                row[3] = BuiltInType.ExtensionObject;
                row[4] = ValueRanks.Scalar;
                row[5] = annotation;

                if (!replaced)
                {
                    ArchiveItem.DataSet.Tables[2].Rows.Add(row);
                }
            }

            // delete record.
            else
            {
                row.Delete();
            }

            // accept all changes.
            ArchiveItem.DataSet.AcceptChanges();

            return StatusCodes.Good;
        }

        /// <summary>
        /// Deletes a value from the history.
        /// </summary>
        public uint DeleteHistory(SystemContext context, DateTime sourceTimestamp)
        {
            bool deleted = false;

            string filter = String.Format(System.Globalization.CultureInfo.InvariantCulture, "SourceTimestamp = #{0}#", sourceTimestamp);

            DataView view = new DataView(
                ArchiveItem.DataSet.Tables[0],
                filter,
                null,
                DataViewRowState.CurrentRows);

            for (int ii = 0; ii < view.Count; ii++)
            {
                int updateType = (int)view[ii].Row[5];

                if (updateType == 0)
                {
                    view[ii].Row[5] = HistoryUpdateType.Delete;
                    view[ii].Row[6] = GetModificationInfo(context, HistoryUpdateType.Delete);
                    deleted = true;
                }
            }

            if (!deleted)
            {
                return StatusCodes.BadNoEntryExists;
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Deletes a property value from the history.
        /// </summary>
        public uint DeleteAnnotationHistory(SystemContext context, QualifiedName propertyName, DateTime sourceTimestamp)
        {
            bool deleted = false;

            string filter = String.Format(System.Globalization.CultureInfo.InvariantCulture, "SourceTimestamp = #{0}#", sourceTimestamp);

            DataView view = new DataView(
                SelectTable(propertyName),
                filter,
                null,
                DataViewRowState.CurrentRows);

            for (int ii = 0; ii < view.Count; ii++)
            {
                int updateType = (int)view[ii].Row[5];
            }

            if (!deleted)
            {
                return StatusCodes.BadNoEntryExists;
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Deletes a value from the history.
        /// </summary>
        public uint DeleteHistory(SystemContext context, DateTime startTime, DateTime endTime, bool isModified)
        {
            // ensure time goes up.
            if (endTime < startTime)
            {
                DateTime temp = startTime;
                startTime = endTime;
                endTime = temp;
            }

            string filter = String.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "SourceTimestamp >= #{0}# AND SourceTimestamp < #{1}#", 
                startTime,
                endTime);

            // select the table.
            DataTable table = ArchiveItem.DataSet.Tables[0];

            if (isModified)
            {
                table = ArchiveItem.DataSet.Tables[1];
            }

            // delete the values.
            DataView view = new DataView(
                table,
                filter,
                null,
                DataViewRowState.CurrentRows);

            List<DataRow> rowsToDelete = new List<DataRow>();

            for (int ii = 0; ii < view.Count; ii++)
            {
                if (!isModified)
                {
                    DataRow modifiedRow = ArchiveItem.DataSet.Tables[1].NewRow();

                    modifiedRow[0] = view[ii].Row[0];
                    modifiedRow[1] = view[ii].Row[1];
                    modifiedRow[2] = view[ii].Row[2];
                    modifiedRow[3] = view[ii].Row[3];
                    modifiedRow[4] = view[ii].Row[4];
                    modifiedRow[5] = HistoryUpdateType.Delete;
                    modifiedRow[6] = GetModificationInfo(context, HistoryUpdateType.Delete);

                    ArchiveItem.DataSet.Tables[1].Rows.Add(modifiedRow);
                }

                rowsToDelete.Add(view[ii].Row);
            }

            // delete rows.
            foreach (DataRow row in rowsToDelete)
            {
                row.Delete();
            }

            // commit all changes.
            ArchiveItem.DataSet.AcceptChanges();

            return StatusCodes.Good;
        }

        /// <summary>
        /// Reads the history for the specified time range.
        /// </summary>
        public DataView ReadHistory(DateTime startTime, DateTime endTime, bool isModified)
        {
            return ReadHistory(startTime, endTime, isModified, null);
        }

        /// <summary>
        /// Reads the history for the specified time range.
        /// </summary>
        public DataView ReadHistory(DateTime startTime, DateTime endTime, bool isModified, QualifiedName browseName)
        {
            if (isModified)
            {
                return ArchiveItem.DataSet.Tables[1].DefaultView;
            }

            if (browseName == Opc.Ua.BrowseNames.Annotations)
            {
                return ArchiveItem.DataSet.Tables[2].DefaultView;
            }

            return ArchiveItem.DataSet.Tables[0].DefaultView;
        }

        /// <summary>
        /// Finds the value at or before the timestamp.
        /// </summary>
        public int FindValueAtOrBefore(DataView view, DateTime timestamp, bool ignoreBad, out bool dataIgnored)
        {
            dataIgnored = false;

            if (view.Count <= 0)
            {
                return -1;
            }

            int min = 0;
            int max = view.Count;
            int position = (max - min) / 2;

            while (position >= 0 && position < view.Count)
            {
                DateTime current = (DateTime)view[position].Row[0];

                // check for exact match.
                if (current == timestamp)
                {
                    // skip the first timestamp.
                    while (position > 0 && (DateTime)view[position - 1].Row[0] == timestamp)
                    {
                        position--;
                    }

                    return position;
                }

                // move up.
                if (current < timestamp)
                {
                    min = position + 1;
                }

                // move down.
                if (current > timestamp)
                {
                    max = position - 1;
                }

                // not found.
                if (max < min)
                {
                    // find the value before.
                    while (position >= 0)
                    {
                        timestamp = (DateTime)view[position].Row[0];

                        // skip the first timestamp in group.
                        while (position > 0 && (DateTime)view[position - 1].Row[0] == timestamp)
                        {
                            position--;
                        }

                        // ignore bad data.
                        if (ignoreBad)
                        {
                            DataValue value = (DataValue)view[position].Row[2];

                            if (StatusCode.IsBad(value.StatusCode))
                            {
                                position--;
                                dataIgnored = true;
                                continue;
                            }
                        }

                        break;
                    }

                    // return the position.
                    return position;
                }

                position = min + (max - min) / 2;
            }

            return -1;
        }

        /// <summary>
        /// Returns the next value after the current position.
        /// </summary>
        public int FindValueAfter(DataView view, int position, bool ignoreBad, out bool dataIgnored)
        {
            dataIgnored = false;

            if (position < 0 || position >= view.Count)
            {
                return -1;
            }

            DateTime timestamp = (DateTime)view[position].Row[0];

            // skip the current timestamp.
            while (position < view.Count && (DateTime)view[position].Row[0] == timestamp)
            {
                position++;
            }

            if (position >= view.Count)
            {
                return -1;
            }

            // find the value after.
            while (position < view.Count)
            {
                timestamp = (DateTime)view[position].Row[0];

                // ignore bad data.
                if (ignoreBad)
                {
                    DataValue value = (DataValue)view[position].Row[2];

                    if (StatusCode.IsBad(value.StatusCode))
                    {
                        position++;
                        dataIgnored = true;
                        continue;
                    }
                }

                break;
            }

            if (position >= view.Count)
            {
                return -1;
            }

            // return the position.
            return position;
        }

        /// <summary>
        /// Constructs a node identifier for a item object.
        /// </summary>
        public static NodeId ConstructId(string filePath, ushort namespaceIndex)
        {
            ParsedNodeId parsedNodeId = new ParsedNodeId();

            parsedNodeId.RootId = filePath;
            parsedNodeId.NamespaceIndex = namespaceIndex;
            parsedNodeId.RootType = NodeTypes.Item;

            return parsedNodeId.Construct();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Selects the table to use.
        /// </summary>
        private DataTable SelectTable(QualifiedName propertyName)
        {
            if (propertyName == null || propertyName.Name == null)
            {
                return ArchiveItem.DataSet.Tables[0];
            }

            switch (propertyName.Name)
            {
                case Opc.Ua.BrowseNames.Annotations:
                    {
                        return ArchiveItem.DataSet.Tables[2];
                    }
            }

            return ArchiveItem.DataSet.Tables[0];
        }

        /// <summary>
        /// Creates a modification info record.
        /// </summary>
        private ModificationInfo GetModificationInfo(SystemContext context, HistoryUpdateType updateType)
        {
            ModificationInfo info = new ModificationInfo();
            info.UpdateType = updateType;
            info.ModificationTime = DateTime.UtcNow;

            if (context.OperationContext != null && context.OperationContext.UserIdentity != null)
            {
                info.UserName = context.OperationContext.UserIdentity.DisplayName;
            }

            return info;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The item in the archive.
        /// </summary>
        public ArchiveItem ArchiveItem { get; }

        /// <summary>
        /// The item in the archive.
        /// </summary>
        public int SubscribeCount { get; set; }
        #endregion

        #region Private Fields
        private HistoricalDataConfigurationState configuration_;
        private PropertyState<Annotation> annotations_;
        private List<DataValue> pattern_;
        private int patternIndex_;
        private DateTime nextSampleTime_;
        #endregion
    }
}
