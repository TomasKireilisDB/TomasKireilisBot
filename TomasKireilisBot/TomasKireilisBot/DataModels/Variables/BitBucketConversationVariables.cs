using System.Collections.Generic;

namespace TomasKireilisBot.DataModels
{
    public class BitBucketConversationVariables
    {
        public string PushNotifications { get; set; }
        public List<BitBucketGlobalVariables> GlobalVariables = new List<BitBucketGlobalVariables>();
        public string Data { get; set; }
    }
}