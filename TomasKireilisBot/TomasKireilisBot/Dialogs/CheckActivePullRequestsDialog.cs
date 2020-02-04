// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using TomasKireilisBot.DataModels;

namespace TomasKireilisBot.Dialogs
{
    public class CheckActivePullRequestsDialog : CancelAndHelpDialog
    {
        private const string DestinationStepMsgText = "(Get active pull requests) or (GPR)";
        private const string OriginStepMsgText = "Do you need extra filters? (y/n)";

        public CheckActivePullRequestsDialog()
            : base(nameof(CheckActivePullRequestsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //  AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OnMessageActivityAsync,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        protected async Task<DialogTurnResult> OnMessageActivityAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var pullRequestDetails = (PullRequestDetails)stepContext.Options;
            if (pullRequestDetails.ProjectInfo == null)
            {
                var reply = MessageFactory.Text("What is your favorite color?", "What is your favorite color?", InputHints.ExpectingInput);

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
            }
            var newProject = new ProjectInfo();
            newProject.ProjectKey = (string)stepContext.Result;
            pullRequestDetails.ProjectInfo.Add(newProject);
            //reply.SuggestedActions = new SuggestedActions()
            //{
            //    Actions = new List<CardAction>()
            //    {
            //        new CardAction() { Title = "Get active pull requests => \"GPR\"", Type = ActionTypes.ImBack, Value = "Red" },
            //        new CardAction() { Title = "Change pull request configuration => \"PRC\"", Type = ActionTypes.ImBack, Value = "Yellow" },
            //        new CardAction() { Title = "Activate pull request notification => \"APR\"", Type = ActionTypes.ImBack, Value = "Blue" },
            //        new CardAction() { Title = "Deactivate pull request notification => \"DPR\"", Type = ActionTypes.ImBack, Value = "Orange" }
            //    },
            //};
            return await stepContext.NextAsync(pullRequestDetails.ProjectInfo, cancellationToken);
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = new BookingDetails();

            if (bookingDetails.Destination == null)
            {
                var promptMessage = MessageFactory.Text(DestinationStepMsgText, DestinationStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            return await stepContext.NextAsync(bookingDetails.Destination, cancellationToken);
        }

        //private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.Destination = (string)stepContext.Result;

        //    if (bookingDetails.Origin == null)
        //    {
        //        var promptMessage = MessageFactory.Text(OriginStepMsgText, OriginStepMsgText, InputHints.ExpectingInput);
        //        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.Origin, cancellationToken);
        //}

        //private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.Origin = (string)stepContext.Result;

        //    if (bookingDetails.TravelDate == null || IsAmbiguous(bookingDetails.TravelDate))
        //    {
        //        return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), bookingDetails.TravelDate, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.TravelDate, cancellationToken);
        //}

        //private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.TravelDate = (string)stepContext.Result;

        //    var messageText = $"Please confirm, I have you traveling to: {bookingDetails.Destination} from: {bookingDetails.Origin} on: {bookingDetails.TravelDate}. Is this correct?";
        //    var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

        //    return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var pullRequestDetails = (PullRequestDetails)stepContext.Options;

            return await stepContext.EndDialogAsync(pullRequestDetails, cancellationToken);
        }

        //private static bool IsAmbiguous(string timex)
        //{
        //    var timexProperty = new TimexProperty(timex);
        //    return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        //}
    }
}