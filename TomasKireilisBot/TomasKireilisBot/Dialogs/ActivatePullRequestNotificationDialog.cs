using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.Helpers;

namespace TomasKireilisBot.Dialogs
{
    public class ActivatePullRequestNotificationDialog : CancelAndHelpDialog
    {
        public ActivatePullRequestNotificationDialog() : base(nameof(ActivatePullRequestNotificationDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Adding notification... please wait. It might take some time.", cancellationToken: cancellationToken);
            var variables = await GlobalVariablesService.GetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id);
            variables.PushNotifications = "true";
            if (!await GlobalVariablesService.SetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id, variables))
            {
                await stepContext.Context.SendActivityAsync($"Unexpected error happened while trying to add notification.", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync("Notification added", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}