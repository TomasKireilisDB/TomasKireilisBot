using Bitbucket.Net.Models.Core.Projects;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Helpers;
using TomasKireilisBot.Services.BitbucketService;

namespace TomasKireilisBot.Dialogs
{
    public class CheckActivePullRequestsDialog : CancelAndHelpDialog
    {
        private const string DestinationStepMsgText = "(Get active pull requests) or (GPR)";
        private const string OriginStepMsgText = "Do you need extra filters? (y/n)";
        private IInnerBitbucketClient _bitbucketClient;
        public BitBucketConversationVariables _bitBucketConversationVariables;

        public CheckActivePullRequestsDialog(IInnerBitbucketClient bitbucketClient, BitBucketConversationVariables bitBucketConversationVariables)
            : base(nameof(CheckActivePullRequestsDialog))
        {
            _bitbucketClient = bitbucketClient;
            _bitBucketConversationVariables = bitBucketConversationVariables;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //  AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FetchPullRequests,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FetchPullRequests(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _bitBucketConversationVariables = await GlobalVariablesResolver.GetBitBucketConversationVariables();

            var pullRequestList = new List<PullRequest>();
            foreach (var globalVariables in _bitBucketConversationVariables.GlobalVariables)
            {
                await stepContext.Context.SendActivityAsync("Gathering info...", cancellationToken: cancellationToken);
                try
                {
                    pullRequestList.AddRange(await _bitbucketClient.FetchActivePullRequests(globalVariables, _bitBucketConversationVariables.PersonalizedVariables.First()));
                }
                catch (Exception e)
                {
                    await stepContext.Context.SendActivityAsync("Exception was thrown during fetching data, maybe there in a wrong info provided for fetching information?", cancellationToken: cancellationToken);
                    await stepContext.Context.SendActivityAsync(e.Message, cancellationToken: cancellationToken);
                }
            }

            if (pullRequestList.FindAll(x => x.Open).Count == 0)
            {
                await stepContext.Context.SendActivityAsync("No active pull requests found", cancellationToken: cancellationToken);
            }

            foreach (var pullRequest in pullRequestList.FindAll(x => x.Open))
            {
                await stepContext.Context.SendActivityAsync($"Author: {pullRequest.Author.User.Name}  \n  " +
                                                            $"Id: {pullRequest.Id} \n  " +
                                                            $"Description: {pullRequest.Description} \n", cancellationToken: cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}