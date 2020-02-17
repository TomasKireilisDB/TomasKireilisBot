using System.Collections.Generic;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Services.BitbucketService
{
    public interface IInnerBitbucketClient
    {
        Task<List<PullRequest>> FetchActivePullRequests(
            BitBucketGlobalVariables bitBucketGlobalVariables);

        Task<bool> ApprovePullRequest(
            BitBucketGlobalVariables bitBucketGlobalVariables,
            long pullRequestId);
    }
}