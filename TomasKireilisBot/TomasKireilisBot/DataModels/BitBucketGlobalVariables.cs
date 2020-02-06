using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace TomasKireilisBot.DataModels
{
    public class BitBucketGlobalVariables
    {
        public string BaseUri { get; set; }
        public string ProjectName { get; set; }
        public string RepositoryName { get; set; }
    }
}