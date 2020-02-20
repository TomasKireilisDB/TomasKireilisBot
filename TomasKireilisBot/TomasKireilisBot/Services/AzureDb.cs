using Microsoft.Bot.Builder.Azure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Helpers;

namespace TomasKireilisBot.Services
{
    public class AzureDb
    {
        private const string CosmosServiceEndpoint = "https://tomasbotdb.documents.azure.com:443/";
        private const string CosmosDBKey = "FcklPr254hZ2HIkUu4r4bzCKypfNjJPhJCbLnzxB456gung588zxvpaPrIh2q4kw70D3F2oB6jM8ynPmFOSSeA==";
        private const string CosmosDBDatabaseId = "tomasbotdbid";
        private const string CosmosDBContainerId = "tomasbotdbid-storage";

        private static readonly CosmosDbPartitionedStorage _myStorage = new CosmosDbPartitionedStorage(new CosmosDbPartitionedStorageOptions
        {
            CosmosDbEndpoint = CosmosServiceEndpoint,
            AuthKey = CosmosDBKey,
            DatabaseId = CosmosDBDatabaseId,
            ContainerId = CosmosDBContainerId,
        });

        public async Task<BitBucketConversationVariables> GetUserConfigurationsAsync(string userId)
        {
            var response = (Dictionary<string, object>)await _myStorage.ReadAsync(new[] { userId });
            if (response.TryGetValue(userId, out object value))
            {
                var re = value.ToString();
            }
            return null;
        }

        public async Task<bool> UpdateUserDefaultDbConfigurations(string userId)
        {
            var response = new Dictionary<string, object>();
            response = (Dictionary<string, object>)await _myStorage.ReadAsync(new[] { userId }) ?? new Dictionary<string, object>();
            if (response.TryGetValue(userId, out object value))
            {
                return false;
            }
            else
            {
                IDictionary<string, object> defaultConfig = new Dictionary<string, object>();
                defaultConfig.Add(userId, await GlobalVariablesService.GetDefaultJsonGlobalVariables());
                await _myStorage.WriteAsync(defaultConfig);
                return true;
            }
        }
    }
}