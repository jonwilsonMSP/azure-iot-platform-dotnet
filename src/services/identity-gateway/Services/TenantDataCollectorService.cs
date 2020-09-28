// <copyright file="TenantDataCollectorService.cs" company="3M">
// Copyright (c) 3M. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Mmm.Iot.Common.Services.Config;
using Mmm.Iot.Common.Services.External.Azure;

namespace Mmm.Iot.IdentityGateway.Services
{
    public class TenantDataCollectorService
    {
        private readonly IAzure client;
        private readonly AppConfig config;
        private ResourceManagementClient rmClient;
        private StorageManagementClient storageClient;

        public TenantDataCollectorService(IAzureManagementClientFactory clientFactory, AppConfig config)
        {
            this.client = clientFactory.Create();
            this.config = config;
            this.rmClient = (ResourceManagementClient)this.client.ManagementClients.FirstOrDefault(t =>
                t.GetType() == typeof(ResourceManagementClient));
            this.storageClient = (StorageManagementClient)this.client.ManagementClients.FirstOrDefault(t => t.GetType() == typeof(StorageManagementClient));
        }

        public async Task<object> GetResourceGroups()
        {
            var x = (await this.rmClient.ResourceGroups.ListAsync()).ToList();

            return x;
        }

        public async Task<object> GetResourcesByResourceGroups()
        {
            var x = (await this.rmClient.Resources.ListByResourceGroupAsync("rg-iot-ggk-dev")).ToList();
            string storageAccountName = x.Where(y => y.Kind == "Storage").ToList()[0].Name;
            string storageAccConnectionString = this.GetStorageAccountConnectionString("rg-iot-ggk-dev", storageAccountName);
            return x;
        }

        public string GetStorageAccountConnectionString(string resourceGroupName, string storageAccountName)
        {
            IList<StorageAccountKey> acctKeys = this.storageClient.StorageAccounts.ListKeysAsync(resourceGroupName, storageAccountName).Result.Keys;
            return $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={acctKeys.FirstOrDefault()?.Value};EndpointSuffix=core.windows.net";
        }
    }
}