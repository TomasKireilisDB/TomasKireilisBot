using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomasKireilisBot.DataModels
{
    public class BitbucketPersonalizedVariablesSpecificInfo
    {
        public string UserName { get; set; }
        public string? Password { get; set; }
        public string? PersonalAccessToken { get; set; }
        public string BaseUrl { get; set; }
    }
}