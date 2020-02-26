using AdaptiveCards;
using Bitbucket.Net.Models.Core.Projects;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.DataModels.Variables;
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
            BitBucketConversationVariables = await GlobalVariablesService.GetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id);
            if (BitBucketConversationVariables?.GlobalVariables == null)
            {
                await stepContext.Context.SendActivityAsync("No configuration found", cancellationToken: cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }
            await stepContext.Context.SendActivityAsync("Gathering info...", cancellationToken: cancellationToken);
            bool foundAnyPullRequest = false;
            foreach (var globalVariable in BitBucketConversationVariables.GlobalVariables)
            {
                foreach (var project in globalVariable.Projects)
                {
                    foreach (var repositoryName in project.RepositoryNames)
                    {
                        var pullRequestList = new List<PullRequest>();

                        try
                        {
                            pullRequestList.AddRange((await _bitbucketClient.FetchActivePullRequests(
                                globalVariable.BaseUrl,
                                project.ProjectName,
                                repositoryName,
                                globalVariable.PersonalAccessToken,
                                globalVariable.Password,
                                globalVariable.UserName)).FindAll(x => x.Open));

                            if (pullRequestList.Count > 0) foundAnyPullRequest = true;
                        }
                        catch (Exception e)
                        {
                            await stepContext.Context.SendActivityAsync(
                                "Error was thrown during fetching data, maybe there in a wrong info provided for fetching information?",
                                cancellationToken: cancellationToken);
                            await stepContext.Context.SendActivityAsync(e.Message, cancellationToken: cancellationToken);
                        }

                        foreach (var pullRequest in pullRequestList)
                        {
                            var pullRequestCard = CreateAdaptiveCardAttachment(pullRequest, globalVariable.BaseUrl, project.ProjectName,
                                repositoryName);
                            var response = MessageFactory.Attachment(pullRequestCard);
                            await stepContext.Context.SendActivityAsync(response, cancellationToken);
                        }
                    }
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

        private Attachment CreateAdaptiveCardAttachment(PullRequest pullRequest, string baseUrl,
            string projectName, string repositoryName)
        {
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = $"Author: {pullRequest.Author?.User.Name}  \n  "
            });

            card.Body.Add(new TextBlock()
            {
                Text = $"Id: {pullRequest.Id} \n  "
            });

            card.Body.Add(new TextBlock()
            {
                Text = $"Description: {pullRequest.Description} \n"
            });

            card.Body.Add(new TextBlock()
            {
                Text = $"Create date: {pullRequest.CreatedDate} \n"
            });

            card.Actions.Add(new SubmitAction()
            {
                Title = "Approve pull request",
                DataJson = JsonConvert.SerializeObject(new CardAction() { Type = ActionTypes.ImBack, Value = $"ApprovePullRequest>{baseUrl}>{projectName}>{repositoryName}>{pullRequest.Id}" }),
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