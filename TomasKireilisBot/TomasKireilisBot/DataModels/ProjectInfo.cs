using System.Collections.Generic;

namespace TomasKireilisBot.DataModels
{
    public class ProjectInfo
    {
        public ProjectInfo()
        {
            RepositorySlugs = new List<string>();
        }

        public string ProjectKey { get; set; }
        public List<string> RepositorySlugs { get; set; }
    }
}