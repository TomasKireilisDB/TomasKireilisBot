using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.Helpers;
using TomasKireilisBot.Services.Timer;

namespace TomasKireilisBot.Dialogs
{
    public class DeActivatePullRequestNotificationDialog : CancelAndHelpDialog
    {
        private readonly Timers _timers;

        public DeActivatePullRequestNotificationDialog(Timers timers) : base(nameof(DeActivatePullRequestNotificationDialog))
        {
            _timers = timers;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Removing notifications... please wait. It might take some time.", cancellationToken: cancellationToken);
            var variables = await GlobalVariablesService.GetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id);
            variables.PushNotifications = "false";
            if (!await GlobalVariablesService.SetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id, variables))
            {
                await stepContext.Context.SendActivityAsync($"Unexpected error happened while trying to remove notification.", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            await stepContext.Context.SendActivityAsync("Notifications Removed", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}