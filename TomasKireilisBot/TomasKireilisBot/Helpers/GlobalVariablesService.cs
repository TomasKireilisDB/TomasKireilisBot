using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Helpers
{
    public static class GlobalVariablesService
    {
        public static async Task<BitBucketConversationVariables> GetBitBucketConversationVariables()
        {
            return JsonConvert.DeserializeObject<BitBucketConversationVariables>(await GetJsonGlobalVariables());
        }

        public static async Task<string> GetJsonGlobalVariables()
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
    }
}