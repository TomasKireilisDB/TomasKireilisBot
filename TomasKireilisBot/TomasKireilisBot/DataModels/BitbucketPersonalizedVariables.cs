using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomasKireilisBot.DataModels
{
    public class BitBucketPersonalizedVariables
    {
        public string FullName { get; set; }
        public string UserName { get; set; }

        public string? Password { get; set; }
        public string? PersonalAccessToken { get; set; }
    }
}