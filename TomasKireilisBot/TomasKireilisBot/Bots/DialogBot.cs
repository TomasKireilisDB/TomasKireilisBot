// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Dialogs;
using TomasKireilisBot.Helpers;
using TomasKireilisBot.Services.BitbucketService;

namespace TomasKireilisBot.Bots
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class DialogBot<T> : ActivityHandler
        where T : Dialog
    {
        protected readonly Dialog Dialog;
        private readonly BitBucketConversationVariables _conversationVariables;
        private readonly IInnerBitbucketClient _innerBitbucketClient;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly ILogger Logger;
        private List<ExpectedCommand> _expectedCommandsList;

        public DialogBot(IInnerBitbucketClient innerBitbucketClient, ConversationState conversationState, UserState userState, T dialog,
            ILogger<DialogBot<T>> logger, List<ExpectedCommand> expectedCommandsList)
        {
            _conversationVariables = GlobalVariablesService.GetBitBucketConversationVariables().Result;
            _innerBitbucketClient = innerBitbucketClient;
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
            _expectedCommandsList = expectedCommandsList;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await CheckIfApprovePullRequestActivity(turnContext, cancellationToken);
            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        private async Task CheckIfApprovePullRequestActivity(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var dataList = new List<string>();
            var activityValue = "";
            try
            {
                activityValue = JsonConvert.DeserializeObject<PullRequestApprovalExecutionData>(turnContext.Activity.Value?.ToString())
                    .ApprovePullRequest;
            }
            catch
            {
                return;
            }

            if (activityValue == null)
            {
                return;
            }

            dataList = activityValue.Split('>', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (dataList.Count == 5 && dataList[0]?.ToLower() == "approvepullrequest")
            {
                await turnContext.SendActivityAsync("CheckingPullRequestStatus...", cancellationToken: cancellationToken);
                bool resultFound = false;

                foreach (var globalVariable in _conversationVariables.GlobalVariables)
                {
                    foreach (var project in globalVariable.Projects)
                    {
                        foreach (var repositoryName in project.RepositoryNames)
                        {
                            if (string.Equals(globalVariable.BaseUrl, dataList[1], StringComparison.CurrentCultureIgnoreCase) &&
                                string.Equals(project.ProjectName, dataList[2], StringComparison.CurrentCultureIgnoreCase) &&
                                string.Equals(repositoryName, dataList[3], StringComparison.CurrentCultureIgnoreCase))
                            {
                                resultFound = true;
                                try
                                {
                                    await _innerBitbucketClient.ApprovePullRequest(globalVariable.BaseUrl,
                                        project.ProjectName,
                                        repositoryName,
                                        globalVariable.PersonalAccessToken,
                                        globalVariable.Password,
                                        globalVariable.UserName, long.Parse(dataList[4]));
                                    await turnContext.SendActivityAsync("Approved successfully", cancellationToken: cancellationToken);
                                }
                                catch (Exception e)
                                {
                                    await turnContext.SendActivityAsync("Oooops. Something went wrong. Could not approve pull request",
                                        cancellationToken: cancellationToken);
                                    await turnContext.SendActivityAsync(e.Message,
                                        cancellationToken: cancellationToken);
                                }

                                break;
                            }
                        }
                    }
                }

                if (!resultFound)
                {
                    await turnContext.SendActivityAsync("Could not find such repository. Check project and repository names",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}