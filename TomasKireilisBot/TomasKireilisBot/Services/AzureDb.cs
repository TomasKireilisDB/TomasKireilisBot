using Microsoft.Bot.Builder.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels.Variables;
using TomasKireilisBot.Helpers;

namespace TomasKireilisBot.Services
{
    public class AzureDb
    {
        private const string CosmosServiceEndpoint = "https://tomasbotdb.documents.azure.com:443/";
        private const string CosmosDbKey = "FcklPr254hZ2HIkUu4r4bzCKypfNjJPhJCbLnzxB456gung588zxvpaPrIh2q4kw70D3F2oB6jM8ynPmFOSSeA==";
        private const string CosmosDbDatabaseId = "tomasbotdbid";
        private const string CosmosDbContainerId = "tomasbotdbid-storage";

        private static readonly CosmosDbPartitionedStorage MyStorage = new CosmosDbPartitionedStorage(new CosmosDbPartitionedStorageOptions
        {
            CosmosDbEndpoint = CosmosServiceEndpoint,
            AuthKey = CosmosDbKey,
            DatabaseId = CosmosDbDatabaseId,
            ContainerId = CosmosDbContainerId,
        });

        public async Task<BitBucketConversationVariables> GetUserConfigurationsAsync(string userId)
        {
            var response = (Dictionary<string, object>)await MyStorage.ReadAsync(new[] { userId }) ?? new Dictionary<string, object>();
            if (response.TryGetValue(userId, out object value))
            {
                var jObject = JObject.Parse(value.ToString());
                return JsonConvert.DeserializeObject<BitBucketConversationVariables>(jObject.ToString());
            }
            await UpdateUserDefaultDbConfigurations(userId);
            return await GetUserConfigurationsAsync(userId);
        }

        public async Task<bool> SetUserConfigurationsAsync(
            string userId,
            BitBucketConversationVariables bitBucketConversationVariables)
        {
            if (GetUserConfigurationsAsync(userId) != null)
            {
                await MyStorage.DeleteAsync(new[] { userId });
            }
            IDictionary<string, object> defaultConfig = new Dictionary<string, object>();
            defaultConfig.Add(userId, JObject.Parse(JsonConvert.SerializeObject(bitBucketConversationVariables)));
            await MyStorage.WriteAsync(defaultConfig);
            return true;
        }

        public async Task<bool> UpdateUserDefaultDbConfigurations(string userId)
        {
            var response = (Dictionary<string, object>)await MyStorage.ReadAsync(new[] { userId }) ?? new Dictionary<string, object>();
            if (response.TryGetValue(userId, out object value))
            {
                return false;
            }

            IDictionary<string, object> defaultConfig = new Dictionary<string, object>();
            defaultConfig.Add(userId, JObject.Parse(await GlobalVariablesService.GetDefaultJsonGlobalVariables()));
            await MyStorage.WriteAsync(defaultConfig);
            return true;
        }
    }
}