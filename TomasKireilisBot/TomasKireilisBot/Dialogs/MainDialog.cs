// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;

        private readonly List<ExpectedCommand> _expectedCommandsList = new List<ExpectedCommand>()
        {
            new ExpectedCommand(nameof(CheckActivePullRequestsDialog),"Get active pull requests","GPR"),
            new ExpectedCommand(nameof(ChangePullRequestsConfigurationDialog),"Change pull requests configuration","PRC"),
            new ExpectedCommand(nameof(ActivatePullRequestNotificationDialog),"Activate pull request notification","APR"),
            new ExpectedCommand("","Deactivate pull request notification","DPR"),
    };

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(CheckActivePullRequestsDialog checkActivePullRequestsDialog, ActivatePullRequestNotificationDialog activatePullRequestNotificationDialog, ChangePullRequestsConfigurationDialog changePullRequestsConfigurationDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(checkActivePullRequestsDialog);
            AddDialog(changePullRequestsConfigurationDialog);
            AddDialog(activatePullRequestNotificationDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("What we are going to do today?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Get active pull requests", Type = ActionTypes.ImBack, Value = "Get active pull requests" },
                    new CardAction() { Title = "Change pull request configuration", Type = ActionTypes.ImBack, Value = "Change pull request configuration" },
                    new CardAction() { Title = "Activate pull request notification", Type = ActionTypes.ImBack, Value = "Activate pull request notification" },
                    new CardAction() { Title = "Deactivate pull request notification", Type = ActionTypes.ImBack, Value = "Deactivate pull request notification" },
                },
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var foundCommand = _expectedCommandsList.Find(x => x.CheckIfCalledThisCommand((string)stepContext.Result));
            if (foundCommand != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You chosen {foundCommand.LongName}"), cancellationToken);

                return await stepContext.BeginDialogAsync(foundCommand.OpenDialogId, new BitBucketConversationVariables(), cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}