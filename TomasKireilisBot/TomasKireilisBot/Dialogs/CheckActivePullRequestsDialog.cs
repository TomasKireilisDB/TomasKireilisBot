using Bitbucket.Net.Models.Core.Projects;
using Microsoft.Bot.Builder;
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
        private BitBucketConversationVariables _bitBucketConversationVariables;

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
                //  var promptMlessage = MessageFactory.Text("dasdsadsda", "dasdsadsda");
                //  await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMlessage }, cancellationToken);
                try
                {
                    pullRequestList.AddRange(await _bitbucketClient.FetchActivePullRequests(globalVariables, _bitBucketConversationVariables.PersonalizedVariables.First()));
                }
                catch (Exception e)
                {
                    var promptMessage = MessageFactory.Text(e.Message);
                    return await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
                }
            }

            foreach (var pullRequest in pullRequestList)
            {
                var promptMessage = MessageFactory.Text(DestinationStepMsgText, DestinationStepMsgText);
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var pullRequestDetails = (PullRequestDetails)stepContext.Options;

            return await stepContext.EndDialogAsync(pullRequestDetails, cancellationToken);
        }
    }
}