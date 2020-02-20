using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Services;

namespace TomasKireilisBot.Helpers
{
    public static class GlobalVariablesService
    {
        private static readonly AzureDb _azureDb = new AzureDb();

        public static async Task<BitBucketConversationVariables> GetBitBucketConversationVariables(string userId)
        {
            return await _azureDb.GetUserConfigurationsAsync(userId);
            // return JsonConvert.DeserializeObject<BitBucketConversationVariables>(await GetJsonGlobalVariables());
        }

        public static async Task<string> GetDefaultJsonGlobalVariables()
        {
            using (StreamReader r = new StreamReader("GlobalVariables.json"))
            {
                return r.ReadToEnd();
            }
        }

        public static async Task<bool> SetBitBucketConversationVariables(BitBucketConversationVariables bitBucketConversationVariables)
        {
            using (StreamWriter r = new StreamWriter("GlobalVariables.json"))
            {
                try
                {
                    await r.WriteAsync(JsonConvert.SerializeObject(bitBucketConversationVariables));
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public static async Task<bool> SetDefaultBitBucketConversationVariables(string userId)
        {
            await _azureDb.UpdateUserDefaultDbConfigurations(userId);
            return true;
        }
    }
}