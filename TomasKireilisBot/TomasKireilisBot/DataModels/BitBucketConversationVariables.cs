using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomasKireilisBot.DataModels
{
    public class BitBucketConversationVariables
    {
        public List<BitBucketGlobalVariables> GlobalVariables = new List<BitBucketGlobalVariables>();
        public List<BitBucketPersonalizedVariables> PersonalizedVariables = new List<BitBucketPersonalizedVariables>();
    }
}