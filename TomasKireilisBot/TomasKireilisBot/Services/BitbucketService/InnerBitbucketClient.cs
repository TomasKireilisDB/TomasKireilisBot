using Bitbucket.Net;
using Bitbucket.Net.Models.Core.Projects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Services.BitbucketService
{
    public class InnerBitbucketClient : IInnerBitbucketClient
    {
        public InnerBitbucketClient()
        {
        }

        public async Task<List<PullRequest>> FetchActivePullRequests(
            BitBucketGlobalVariables bitBucketGlobalVariables)
        {
            var client = new BitbucketClient(
                bitBucketGlobalVariables.BaseUrl,
                bitBucketGlobalVariables.UserName,
                bitBucketGlobalVariables.Password ?? bitBucketGlobalVariables.PersonalAccessToken);
            return (await client.GetPullRequestsAsync(bitBucketGlobalVariables.ProjectName,
                bitBucketGlobalVariables.RepositoryName)).ToList();
        }

        public async Task<bool> ApprovePullRequest(
            BitBucketGlobalVariables bitBucketGlobalVariables,
            long pullRequestId)
        {
            var client = new BitbucketClient(
                bitBucketGlobalVariables.BaseUrl,
                bitBucketGlobalVariables.UserName,
                bitBucketGlobalVariables.Password ?? bitBucketGlobalVariables.PersonalAccessToken);
            var rez = await client.ApprovePullRequestAsync(bitBucketGlobalVariables.ProjectName,
                bitBucketGlobalVariables.RepositoryName,
                pullRequestId);
            return rez.Approved;
        }
    }
}