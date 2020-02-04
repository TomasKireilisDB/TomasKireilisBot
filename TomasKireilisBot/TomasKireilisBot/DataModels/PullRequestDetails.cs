// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using System;
using System.Collections.Generic;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot
{
    public class PullRequestDetails
    {
        public PullRequestDetails()
        {
            ProjectInfo = new List<ProjectInfo>();
        }

        public Guid ConfigurationId { get; set; }
        public string BaseUrl { get; set; }
        public List<ProjectInfo> ProjectInfo { get; set; }
    }
}