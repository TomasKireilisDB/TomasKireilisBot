using Bitbucket.Net.Models.Core.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TomasKireilisBot.Services.BitbucketService
{
    public interface IInnerBitbucketClient
    {
        Task<List<PullRequest>> FetchActivePullRequests(
            string baseUrl,
            string projectName,
            string repositoryName,
            string personalAccessToken,
            string password,
            string userName);

        Task<bool> ApprovePullRequest(
            string baseUrl,
            string projectName,
            string repositoryName,
            string personalAccessToken,
            string password,
            string userName,
            long pullRequestId);
    }
}