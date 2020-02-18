using AdaptiveCards;
using Bitbucket.Net.Models.Core.Projects;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
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
        private readonly IInnerBitbucketClient _bitbucketClient;
        public BitBucketConversationVariables BitBucketConversationVariables;

        public CheckActivePullRequestsDialog(IInnerBitbucketClient bitbucketClient, BitBucketConversationVariables bitBucketConversationVariables)
            : base(nameof(CheckActivePullRequestsDialog))
        {
            _bitbucketClient = bitbucketClient;
            BitBucketConversationVariables = bitBucketConversationVariables;
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
            BitBucketConversationVariables = await GlobalVariablesResolver.GetBitBucketConversationVariables();
            await stepContext.Context.SendActivityAsync("Gathering info...", cancellationToken: cancellationToken);

            bool foundAnyPullRequest = false;
            foreach (var globalVariables in BitBucketConversationVariables.GlobalVariables)
            {
                var pullRequestList = new List<PullRequest>();

                try
                {
                    pullRequestList.AddRange((await _bitbucketClient.FetchActivePullRequests(globalVariables)).FindAll(x => x.Open));

                    if (pullRequestList.Count > 0) foundAnyPullRequest = true;
                }
                catch (Exception e)
                {
                    await stepContext.Context.SendActivityAsync("Exception was thrown during fetching data, maybe there in a wrong info provided for fetching information?", cancellationToken: cancellationToken);
                    await stepContext.Context.SendActivityAsync(e.Message, cancellationToken: cancellationToken);
                }

                foreach (var pullRequest in pullRequestList)
                {
                    var welcomeCard = CreateAdaptiveCardAttachment(pullRequest, globalVariables.BaseUrl, globalVariables.ProjectName, globalVariables.RepositoryName);
                    var response = MessageFactory.Attachment(welcomeCard);
                    await stepContext.Context.SendActivityAsync(response, cancellationToken);
                }
            }

            if (!foundAnyPullRequest)
            {
                await stepContext.Context.SendActivityAsync("No active pull requests found", cancellationToken: cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private Attachment CreateAdaptiveCardAttachment(PullRequest pullRequest, string baseUrl, string projectName, string repositoryName)
        {
            AdaptiveCard card = new AdaptiveCard();

            // Specify speech for the card.
            card.Speak = "Active Pull Request";

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = $"Author: {pullRequest.Author.User.Name}  \n  "
            });

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = $"Id: {pullRequest.Id} \n  "
            });

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = $"Description: {pullRequest.Description} \n"
            });

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = $"Create date: {pullRequest.CreatedDate} \n"
            });

            card.Actions.Add(new SubmitAction()
            {
                Title = "Approve pull request",
                Data = $"ApprovePullRequest>{baseUrl}>{projectName}>{repositoryName}>{pullRequest.Id}"
            });

            // Create the attachment.
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }
    }
}