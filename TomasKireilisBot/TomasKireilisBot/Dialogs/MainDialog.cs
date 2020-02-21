// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;
        private List<ExpectedCommand> _expectedCommandsList;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(CheckActivePullRequestsDialog checkActivePullRequestsDialog,
            ActivatePullRequestNotificationDialog activatePullRequestNotificationDialog,
            DeActivatePullRequestNotificationDialog deActivatePullRequestNotificationDialog,
            ChangePullRequestsConfigurationDialog changePullRequestsConfigurationDialog,

            ILogger<MainDialog> logger, List<ExpectedCommand> expectedCommandsList)
            : base(nameof(MainDialog))
        {
            Logger = logger;
            _expectedCommandsList = expectedCommandsList;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(checkActivePullRequestsDialog);
            AddDialog(changePullRequestsConfigurationDialog);
            AddDialog(activatePullRequestNotificationDialog);
            AddDialog(deActivatePullRequestNotificationDialog);
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
            var cardActionValue = await GetCardActionValueAsync(stepContext.Context);
            if (cardActionValue != null)
            {
                var foundCommand = _expectedCommandsList.Find(x => x.CheckIfCalledThisCommand(cardActionValue));
                if (foundCommand != null)
                {
                    return await stepContext.NextAsync(cardActionValue, cancellationToken);
                }
            }
            var reply = (Activity)MessageFactory.Attachment(CreateAdaptiveCardAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var foundCommand = _expectedCommandsList.Find(x => x.CheckIfCalledThisCommand((string)stepContext.Result));
            if (foundCommand != null)
            {
                return await stepContext.BeginDialogAsync(foundCommand.OpenDialogId, new BitBucketConversationVariables() { Data = (string)stepContext.Result }, cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private Attachment CreateAdaptiveCardAttachment()
        {
            AdaptiveCard card = new AdaptiveCard();

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = "Hi, what are you up to?"
            });

            card.Actions.Add(new SubmitAction()
            {
                Title = "Get active pull requests",
                DataJson = JsonConvert.SerializeObject(new CardAction() { Title = "Get active pull requests", Type = ActionTypes.ImBack, Value = "Get active pull requests" }),
            });

            card.Actions.Add(new SubmitAction()
            {
                Title = "Change pull requests configuration",
                DataJson = JsonConvert.SerializeObject(new CardAction() { Title = "Change pull requests configuration", Type = ActionTypes.ImBack, Value = "Change pull requests configuration" }),
            });

            card.Actions.Add(new SubmitAction()
            {
                Title = "Activate pull request notification",
                DataJson = JsonConvert.SerializeObject(new CardAction() { Title = "Activate pull request notification", Type = ActionTypes.ImBack, Value = "Activate pull request notification" }),
            });

            card.Actions.Add(new SubmitAction()
            {
                Title = "Deactivate pull request notifications",
                DataJson = JsonConvert.SerializeObject(new CardAction() { Title = "Deactivate pull request notifications", Type = ActionTypes.ImBack, Value = "Deactivate pull request notifications" }),
            });

            // Create the attachment.
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }

        private async Task<string> GetCardActionValueAsync(ITurnContext turnContext)
        {
            if (turnContext.Activity.Value == null)
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<CardAction>(turnContext.Activity.Value.ToString()).Value?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}