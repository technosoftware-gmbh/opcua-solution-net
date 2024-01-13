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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Schema;
using Opc.Ua.Schema.Binary;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// A class that holds the configuration for a UA service.
    /// </summary>
    public class DataDictionary
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// The default constructor.
        /// </summary>
        public DataDictionary(IUaSession session)
        {
            Initialize();
            session_ = session;
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
            session_ = null;
            DataTypes = new Dictionary<NodeId, QualifiedName>();
            validator_ = null;
            TypeSystemId = null;
            TypeSystemName = null;
            DictionaryId = null;
            Name = null;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// The node id for the dictionary.
        /// </summary>
        public NodeId DictionaryId { get; private set; }

        /// <summary>
        /// The display name for the dictionary.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The node id for the type system.
        /// </summary>
        public NodeId TypeSystemId { get; private set; }

        /// <summary>
        /// The display name for the type system.
        /// </summary>
        public string TypeSystemName { get; private set; }

        /// <summary>
        /// The type dictionary.
        /// </summary>
        public TypeDictionary TypeDictionary { get; private set; }

        /// <summary>
        /// The data type dictionary DataTypes
        /// </summary>
        public Dictionary<NodeId, QualifiedName> DataTypes { get; private set; }

        /// <summary>
        /// Loads the dictionary identified by the node id.
        /// </summary>
        public void Load(INode dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            var dictionaryId = ExpandedNodeId.ToNodeId(dictionary.NodeId, session_.NamespaceUris);
            Load(dictionaryId, dictionary.ToString());
        }

        /// <summary>
        /// Loads the dictionary identified by the node id.
        /// </summary>
        public void Load(NodeId dictionaryId, string name, byte[] schema = null, IDictionary<string, byte[]> imports = null)
        {
            if (dictionaryId == null)
            {
                throw new ArgumentNullException(nameof(dictionaryId));
            }

            GetTypeSystem(dictionaryId);

            if (schema == null || schema.Length == 0)
            {
                schema = ReadDictionary(dictionaryId);
            }

            if (schema == null || schema.Length == 0)
            {
                throw ServiceResultException.Create(StatusCodes.BadUnexpectedError, "Cannot parse empty data dictionary.");
            }

            // Interoperability: some server may return a null terminated dictionary string, adjust length
            var zeroTerminator = Array.IndexOf<byte>(schema, 0);
            if (zeroTerminator >= 0)
            {
                Array.Resize(ref schema, zeroTerminator);
            }

            Validate(schema, imports);

            ReadDataTypes(dictionaryId);

            DictionaryId = dictionaryId;
            Name = name;
        }

        /// <summary>
        /// Returns true if the dictionary contains the data type description;
        /// </summary>
        public bool Contains(NodeId descriptionId)
        {
            return DataTypes.ContainsKey(descriptionId);
        }

        /// <summary>
        /// Returns the schema for the specified type (returns the entire dictionary if null).
        /// </summary>
        public string GetSchema(NodeId descriptionId)
        {
            if (descriptionId != null)
            {
                if (!DataTypes.TryGetValue(descriptionId, out QualifiedName browseName))
                {
                    return null;
                }

                return validator_.GetSchema(browseName.Name);
            }

            return validator_.GetSchema(null);
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Retrieves the type system for the dictionary.
        /// </summary>
        private void GetTypeSystem(NodeId dictionaryId)
        {
            var references = session_.NodeCache.FindReferences(dictionaryId, ReferenceTypeIds.HasComponent, true, false);
            if (references.Count > 0)
            {
                TypeSystemId = ExpandedNodeId.ToNodeId(references[0].NodeId, session_.NamespaceUris);
                TypeSystemName = references[0].ToString();
            }
        }

        /// <summary>
        /// Retrieves the data types in the dictionary.
        /// </summary>
        /// <remarks>
        /// In order to allow for fast Linq matching of dictionary
        /// QNames with the data type nodes, the BrowseName of
        /// the DataType node is replaced with Value string.
        /// </remarks>
        private void ReadDataTypes(NodeId dictionaryId)
        {
            IList<INode> references = session_.NodeCache.FindReferences(dictionaryId, ReferenceTypeIds.HasComponent, false, false);
            IList<NodeId> nodeIdCollection = references.Select(node => ExpandedNodeId.ToNodeId(node.NodeId, session_.NamespaceUris)).ToList();

            // read the value to get the names that are used in the dictionary
            session_.ReadValues(nodeIdCollection, out DataValueCollection values, out IList<ServiceResult> errors);

            int ii = 0;
            foreach (var reference in references)
            {
                NodeId datatypeId = ExpandedNodeId.ToNodeId(reference.NodeId, session_.NamespaceUris);
                if (datatypeId != null)
                {
                    if (ServiceResult.IsGood(errors[ii]))
                    {
                        var dictName = (String)values[ii].Value;
                        DataTypes[datatypeId] = new QualifiedName(dictName, datatypeId.NamespaceIndex);
                    }
                    ii++;
                }
            }
        }

        /// <summary>
        /// Reads the contents of multiple data dictionaries.
        /// </summary>
        public static async Task<IDictionary<NodeId, byte[]>> ReadDictionariesAsync(
            ISessionClientMethods session,
            IList<NodeId> dictionaryIds,
            CancellationToken ct = default)
        {
            ReadValueIdCollection itemsToRead = new ReadValueIdCollection();
            foreach (var nodeId in dictionaryIds)
            {
                // create item to read.
                ReadValueId itemToRead = new ReadValueId {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    IndexRange = null,
                    DataEncoding = null
                };
                itemsToRead.Add(itemToRead);
            }

            // read values.
            ReadResponse readResponse = await session.ReadAsync(
                null,
                0,
                TimestampsToReturn.Neither,
                itemsToRead,
                ct).ConfigureAwait(false);

            DataValueCollection values = readResponse.Results;
            DiagnosticInfoCollection diagnosticInfos = readResponse.DiagnosticInfos;
            ResponseHeader response = readResponse.ResponseHeader;

            ClientBase.ValidateResponse(values, itemsToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

            var result = new Dictionary<NodeId, byte[]>();

            int ii = 0;
            foreach (var nodeId in dictionaryIds)
            {
                // check for error.
                if (StatusCode.IsBad(values[ii].StatusCode))
                {
                    ServiceResult sr = ClientBase.GetResult(values[ii].StatusCode, 0, diagnosticInfos, response);
                    throw new ServiceResultException(sr);
                }

                // return as a byte array.
                result[nodeId] = values[ii].Value as byte[];
                ii++;
            }

            return result;
        }

        /// <summary>
        /// Reads the contents of a data dictionary.
        /// </summary>
        public byte[] ReadDictionary(NodeId dictionaryId)
        {
            // create item to read.
            ReadValueId itemToRead = new ReadValueId {
                NodeId = dictionaryId,
                AttributeId = Attributes.Value,
                IndexRange = null,
                DataEncoding = null
            };

            ReadValueIdCollection itemsToRead = new ReadValueIdCollection {
                itemToRead
            };

            // read value.
            DataValueCollection values;
            DiagnosticInfoCollection diagnosticInfos;

            ResponseHeader responseHeader = session_.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                itemsToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, itemsToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, itemsToRead);

            // check for error.
            if (StatusCode.IsBad(values[0].StatusCode))
            {
                ServiceResult result = ClientBase.GetResult(values[0].StatusCode, 0, diagnosticInfos, responseHeader);
                throw new ServiceResultException(result);
            }

            // return as a byte array.
            return values[0].Value as byte[];
        }

        /// <summary>
        /// Validates the type dictionary.
        /// </summary>
        /// <param name="dictionary">The encoded dictionary to validate.</param>
        /// <param name="throwOnError">Throw if an error occurred.</param>
        internal void Validate(byte[] dictionary, bool throwOnError)
        {
            Validate(dictionary, null, throwOnError);
        }

        /// <summary>
        /// Validates the type dictionary.
        /// </summary>
        /// <param name="dictionary">The encoded dictionary to validate.</param>
        /// <param name="imports">A table of imported namespace schemas.</param>
        /// <param name="throwOnError">Throw if an error occurred.</param>
        internal void Validate(byte[] dictionary, IDictionary<string, byte[]> imports = null, bool throwOnError = false)
        {
            var memoryStream = new MemoryStream(dictionary);

            if (TypeSystemId == Objects.XmlSchema_TypeSystem)
            {
                var validator = new Opc.Ua.Schema.Xml.XmlSchemaValidator(imports);

                try
                {
                    validator.Validate(memoryStream);
                }
                catch (Exception e)
                {
                    if (throwOnError)
                    {
                        throw;
                    }
                    Utils.LogWarning(e, "Could not validate XML schema, error is ignored.");
                }

                validator_ = validator;
            }

            if (TypeSystemId == Objects.OPCBinarySchema_TypeSystem)
            {
                var validator = new Opc.Ua.Schema.Binary.BinarySchemaValidator(imports);

                try
                {
                    validator.Validate(memoryStream);
                }
                catch (Exception e)
                {
                    if (throwOnError)
                    {
                        throw;
                    }
                    Utils.LogWarning(e, "Could not validate binary schema, error is ignored.");
                }

                validator_ = validator;
                TypeDictionary = validator.Dictionary;
            }
        }
        #endregion

        #region Private Fields
        private IUaSession session_;
        private SchemaValidator validator_;
        #endregion
    }
}
