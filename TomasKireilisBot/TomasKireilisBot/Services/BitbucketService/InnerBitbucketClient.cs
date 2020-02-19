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
            string baseUrl,
            string projectName,
            string repositoryName,
            string personalAccessToken,
            string password,
            string userName
            )
        {
            var client = new BitbucketClient(
                baseUrl,
                userName,
                password ?? personalAccessToken);
            return (await client.GetPullRequestsAsync(projectName,
                repositoryName)).ToList();
        }

        public async Task<bool> ApprovePullRequest(
            string baseUrl,
            string projectName,
            string repositoryName,
            string personalAccessToken,
            string password,
            string userName,
            long pullRequestId)
        {
            var client = new BitbucketClient(
                baseUrl,
                userName,
                password ?? personalAccessToken);
            var rez = await client.UnwatchPullRequestAsync(projectName,
                repositoryName,
                pullRequestId);
            return rez;
        }
    }
}