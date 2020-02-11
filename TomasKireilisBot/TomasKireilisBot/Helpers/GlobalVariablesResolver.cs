using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Helpers
{
    public static class GlobalVariablesResolver
    {
        public static async Task<BitBucketConversationVariables> GetBitBucketConversationVariables()
        {
            using (StreamReader r = new StreamReader("GlobalVariables.json"))
            {
                string json = r.ReadToEnd();
                var bitBucketConversationVariables = JsonConvert.DeserializeObject<BitBucketConversationVariables>(json);
                return bitBucketConversationVariables;
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

        private static void LoadJson()
        {
        }
    }
}