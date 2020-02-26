using System.IO;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.DataModels.Variables;
using TomasKireilisBot.Services;

namespace TomasKireilisBot.Helpers
{
    public static class GlobalVariablesService
    {
        private static readonly AzureDb AzureDb = new AzureDb();

        public static async Task<BitBucketConversationVariables> GetBitBucketConversationVariables(string userId)
        {
            try
            {
                return await AzureDb.GetUserConfigurationsAsync(userId);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<string> GetDefaultJsonGlobalVariables()
        {
            using (StreamReader r = new StreamReader("GlobalVariables.json"))
            {
                return r.ReadToEnd();
            }
        }

        public static async Task<bool> SetBitBucketConversationVariables(string userId, BitBucketConversationVariables bitBucketConversationVariables)
        {
            try
            {
                return await AzureDb.SetUserConfigurationsAsync(userId, bitBucketConversationVariables);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> SetDefaultBitBucketConversationVariables(string userId)
        {
            try
            {
                return await AzureDb.UpdateUserDefaultDbConfigurations(userId);
            }
            catch
            {
                return false;
            }
        }
    }
}