using System.Collections.Generic;

namespace TomasKireilisBot.DataModels
{
    public class BitBucketGlobalVariables
    {
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PersonalAccessToken { get; set; }
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}