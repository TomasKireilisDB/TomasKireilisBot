using System.Collections.Generic;

namespace TomasKireilisBot.DataModels
{
    public class BitBucketPersonalizedVariables
    {
        public string FullName { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
        public string PersonalAccessToken { get; set; }

        public List<BitbucketPersonalizedVariablesSpecificInfo> SpecificLoginInfo { get; set; }
    }
}