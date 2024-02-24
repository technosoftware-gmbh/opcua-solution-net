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
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Bindings;
#endregion

namespace Technosoftware.UaClient
{
    /// <summary>
    /// The async interface for a subscription.
    /// </summary>
    public partial class Subscription
    {
        #region Public Async Methods (TPL)
        /// <summary>
        /// Creates a subscription on the server and adds all monitored items.
        /// </summary>
        public async Task CreateAsync(CancellationToken ct = default)
        {
            VerifySubscriptionState(false);

            // create the subscription.
            var revisedMaxKeepAliveCount = KeepAliveCount;
            var revisedLifetimeCount = LifetimeCount;

            AdjustCounts(ref revisedMaxKeepAliveCount, ref revisedLifetimeCount);

            CreateSubscriptionResponse response = await Session.CreateSubscriptionAsync(
                null,
                PublishingInterval,
                revisedLifetimeCount,
                revisedMaxKeepAliveCount,
                MaxNotificationsPerPublish,
                PublishingEnabled,
                Priority,
                ct).ConfigureAwait(false);

            CreateSubscription(
                response.SubscriptionId,
                response.RevisedPublishingInterval,
                response.RevisedMaxKeepAliveCount,
                response.RevisedLifetimeCount);

            _ = await CreateItemsAsync(ct).ConfigureAwait(false);

            ChangesCompleted();
        }

        /// <summary>
        /// Deletes a subscription on the server.
        /// </summary>
        public async Task DeleteAsync(bool silent, CancellationToken ct = default)
        {
            if (!silent)
            {
                VerifySubscriptionState(true);
            }

            // nothing to do if not created.
            if (!Created)
            {
                return;
            }

            try
            {
                lock (cache_)
                {
                    ResetPublishTimerAndWorkerState();
                }

                // delete the subscription.
                UInt32Collection subscriptionIds = new uint[] { Id };

                DeleteSubscriptionsResponse response = await Session.DeleteSubscriptionsAsync(
                    null,
                    subscriptionIds,
                    ct).ConfigureAwait(false);

                // validate response.
                ClientBase.ValidateResponse(response.Results, subscriptionIds);
                ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, subscriptionIds);

                if (StatusCode.IsBad(response.Results[0]))
                {
                    throw new ServiceResultException(
                        ClientBase.GetResult(response.Results[0], 0, response.DiagnosticInfos, response.ResponseHeader));
                }
            }

            // supress exception if silent flag is set. 
            catch (Exception e)
            {
                if (!silent)
                {
                    throw new ServiceResultException(e, StatusCodes.BadUnexpectedError);
                }
            }
            // always put object in disconnected state even if an error occurs.
            finally
            {
                DeleteSubscription();
            }

            ChangesCompleted();
        }

        /// <summary>
        /// Modifies a subscription on the server.
        /// </summary>
        public async Task ModifyAsync(CancellationToken ct = default)
        {
            VerifySubscriptionState(true);

            // modify the subscription.
            var revisedKeepAliveCount = KeepAliveCount;
            var revisedLifetimeCounter = LifetimeCount;

            AdjustCounts(ref revisedKeepAliveCount, ref revisedLifetimeCounter);

            ModifySubscriptionResponse response = await Session.ModifySubscriptionAsync(
                null,
                Id,
                PublishingInterval,
                revisedLifetimeCounter,
                revisedKeepAliveCount,
                MaxNotificationsPerPublish,
                Priority,
                ct).ConfigureAwait(false);

            // update current state.
            ModifySubscription(
                response.RevisedPublishingInterval,
                response.RevisedMaxKeepAliveCount,
                response.RevisedLifetimeCount);

            ChangesCompleted();
        }

        /// <summary>
        /// Changes the publishing enabled state for the subscription.
        /// </summary>
        public async Task SetPublishingModeAsync(bool enabled, CancellationToken ct = default)
        {
            VerifySubscriptionState(true);

            // modify the subscription.
            UInt32Collection subscriptionIds = new uint[] { Id };

            SetPublishingModeResponse response = await Session.SetPublishingModeAsync(
                null,
                enabled,
                new uint[] { Id },
                ct).ConfigureAwait(false);

            // validate response.
            ClientBase.ValidateResponse(response.Results, subscriptionIds);
            ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, subscriptionIds);

            if (StatusCode.IsBad(response.Results[0]))
            {
                throw new ServiceResultException(
                    ClientBase.GetResult(response.Results[0], 0, response.DiagnosticInfos, response.ResponseHeader));
            }

            // update current state.
            CurrentPublishingEnabled = PublishingEnabled = enabled;
            changeMask_ |= SubscriptionChangeMask.Modified;

            ChangesCompleted();
        }

        /// <summary>
        /// Republishes the specified notification message.
        /// </summary>
        public async Task<NotificationMessage> RepublishAsync(uint sequenceNumber, CancellationToken ct = default)
        {
            VerifySubscriptionState(true);

            RepublishResponse response = await Session.RepublishAsync(
                null,
                Id,
                sequenceNumber,
                ct).ConfigureAwait(false);

            return response.NotificationMessage;
        }

        /// <summary>
        /// Applies any changes to the subscription items.
        /// </summary>
        public async Task ApplyChangesAsync(CancellationToken ct = default)
        {
            _ = await DeleteItemsAsync(ct).ConfigureAwait(false);
            _ = await ModifyItemsAsync(ct).ConfigureAwait(false);
            _ = await CreateItemsAsync(ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Resolves all relative paths to nodes on the server.
        /// </summary>
        public async Task ResolveItemNodeIdsAsync(CancellationToken ct)
        {
            VerifySubscriptionState(true);

            // collect list of browse paths.
            var browsePaths = new BrowsePathCollection();
            var itemsToBrowse = new List<MonitoredItem>();

            PrepareResolveItemNodeIds(browsePaths, itemsToBrowse);

            // nothing to do.
            if (browsePaths.Count == 0)
            {
                return;
            }

            // translate browse paths.
            TranslateBrowsePathsToNodeIdsResponse response = await Session.TranslateBrowsePathsToNodeIdsAsync(
                null,
                browsePaths,
                ct).ConfigureAwait(false);

            BrowsePathResultCollection results = response.Results;
            ClientBase.ValidateResponse(results, browsePaths);
            ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, browsePaths);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToBrowse[ii].SetResolvePathResult(results[ii], ii, response.DiagnosticInfos, response.ResponseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsModified;
        }

        /// <summary>
        /// Creates all items on the server that have not already been created.
        /// </summary>
        public async Task<IList<MonitoredItem>> CreateItemsAsync(CancellationToken ct = default)
        {
            List<MonitoredItem> itemsToCreate;
            MonitoredItemCreateRequestCollection requestItems = PrepareItemsToCreate(out itemsToCreate);

            if (requestItems.Count == 0)
            {
                return itemsToCreate;
            }

            // create monitored items.
            CreateMonitoredItemsResponse response = await Session.CreateMonitoredItemsAsync(
                null,
                Id,
                TimestampsToReturn,
                requestItems,
                ct).ConfigureAwait(false);

            MonitoredItemCreateResultCollection results = response.Results;
            ClientBase.ValidateResponse(results, itemsToCreate);
            ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, itemsToCreate);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToCreate[ii].SetCreateResult(requestItems[ii], results[ii], ii, response.DiagnosticInfos, response.ResponseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsCreated;
            ChangesCompleted();

            // return the list of items affected by the change.
            return itemsToCreate;
        }

        /// <summary>
        /// Modies all items that have been changed.
        /// </summary>
        public async Task<IList<MonitoredItem>> ModifyItemsAsync(CancellationToken ct = default)
        {
            VerifySubscriptionState(true);

            var requestItems = new MonitoredItemModifyRequestCollection();
            var itemsToModify = new List<MonitoredItem>();

            PrepareItemsToModify(requestItems, itemsToModify);

            if (requestItems.Count == 0)
            {
                return itemsToModify;
            }

            // modify the subscription.
            ModifyMonitoredItemsResponse response = await Session.ModifyMonitoredItemsAsync(
                null,
                Id,
                TimestampsToReturn,
                requestItems,
                ct).ConfigureAwait(false);

            MonitoredItemModifyResultCollection results = response.Results;
            ClientBase.ValidateResponse(results, itemsToModify);
            ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, itemsToModify);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToModify[ii].SetModifyResult(requestItems[ii], results[ii], ii, response.DiagnosticInfos, response.ResponseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsModified;
            ChangesCompleted();

            // return the list of items affected by the change.
            return itemsToModify;
        }

        /// <summary>
        /// Deletes all items that have been marked for deletion.
        /// </summary>
        public async Task<IList<MonitoredItem>> DeleteItemsAsync(CancellationToken ct)
        {
            VerifySubscriptionState(true);

            if (deletedItems_.Count == 0)
            {
                return new List<MonitoredItem>();
            }

            List<MonitoredItem> itemsToDelete = deletedItems_;
            deletedItems_ = new List<MonitoredItem>();

            var monitoredItemIds = new UInt32Collection();

            foreach (MonitoredItem monitoredItem in itemsToDelete)
            {
                monitoredItemIds.Add(monitoredItem.Status.Id);
            }

            DeleteMonitoredItemsResponse response = await Session.DeleteMonitoredItemsAsync(
                null,
                Id,
                monitoredItemIds,
                ct).ConfigureAwait(false);

            StatusCodeCollection results = response.Results;
            ClientBase.ValidateResponse(results, monitoredItemIds);
            ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, monitoredItemIds);

            // update results.
            for (var ii = 0; ii < results.Count; ii++)
            {
                itemsToDelete[ii].SetDeleteResult(results[ii], ii, response.DiagnosticInfos, response.ResponseHeader);
            }

            changeMask_ |= SubscriptionChangeMask.ItemsDeleted;
            ChangesCompleted();

            // return the list of items affected by the change.
            return itemsToDelete;
        }

        /// <summary>
        /// Set monitoring mode of items.
        /// </summary>
        public async Task<List<ServiceResult>> SetMonitoringModeAsync(
            MonitoringMode monitoringMode,
            IList<MonitoredItem> monitoredItems,
            CancellationToken ct = default)
        {
            if (monitoredItems == null) throw new ArgumentNullException(nameof(monitoredItems));

            VerifySubscriptionState(true);

            if (monitoredItems.Count == 0)
            {
                return null;
            }

            // get list of items to update.
            var monitoredItemIds = new UInt32Collection();
            foreach (MonitoredItem monitoredItem in monitoredItems)
            {
                monitoredItemIds.Add(monitoredItem.Status.Id);
            }

            SetMonitoringModeResponse response = await Session.SetMonitoringModeAsync(
                null,
                Id,
                monitoringMode,
                monitoredItemIds,
                ct).ConfigureAwait(false);

            StatusCodeCollection results = response.Results;
            ClientBase.ValidateResponse(results, monitoredItemIds);
            ClientBase.ValidateDiagnosticInfos(response.DiagnosticInfos, monitoredItemIds);

            // update results.
            var errors = new List<ServiceResult>();
            var noErrors = UpdateMonitoringMode(
                monitoredItems, errors, results,
                response.DiagnosticInfos, response.ResponseHeader,
                monitoringMode);

            // raise state changed event.
            changeMask_ |= SubscriptionChangeMask.ItemsModified;
            ChangesCompleted();

            // return null list if no errors occurred.
            return noErrors ? null : errors;
        }

        /// <summary>
        /// Tells the server to refresh all conditions being monitored by the subscription.
        /// </summary>
        public async Task ConditionRefreshAsync(CancellationToken ct = default)
        {
            VerifySubscriptionState(true);

            var methodsToCall = new CallMethodRequestCollection();
            methodsToCall.Add(new CallMethodRequest() {
                MethodId = MethodIds.ConditionType_ConditionRefresh,
                InputArguments = new VariantCollection() { new Variant(Id) }
            });

            CallResponse response = await Session.CallAsync(
                null,
                methodsToCall,
                ct).ConfigureAwait(false);
        }
        #endregion
    }
}
