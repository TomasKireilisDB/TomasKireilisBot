namespace TomasKireilisBot.DataModels
{
    public class BitBucketGlobalVariables
    {
        public string BaseUrl { get; set; }
        public string ProjectName { get; set; }
        public string RepositoryName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
        public string PersonalAccessToken { get; set; }
    }
}