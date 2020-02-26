using System.Collections.Generic;

namespace TomasKireilisBot.DataModels.Variables
{
    public class BitBucketConversationVariables
    {
        public string PushNotifications { get; set; }
        public List<BitBucketGlobalVariables> GlobalVariables = new List<BitBucketGlobalVariables>();
    }
}