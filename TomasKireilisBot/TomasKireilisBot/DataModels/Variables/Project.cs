﻿using System.Collections.Generic;

namespace TomasKireilisBot.DataModels.Variables
{
    public class Project
    {
        public string ProjectName { get; set; }
        public List<string> RepositoryNames { get; set; } = new List<string>();
    }
}