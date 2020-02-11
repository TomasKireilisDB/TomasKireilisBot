using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Helpers;

namespace TomasKireilisBot.Dialogs
{
    public class ChangePullRequestsConfigurationDialog : CancelAndHelpDialog
    {
        public ChangePullRequestsConfigurationDialog() : base(nameof(ChangePullRequestsConfigurationDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskForConfigurationFile,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskForConfigurationFile(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Please provide configuration file", cancellationToken: cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var pullRequestDetails = (BitBucketConversationVariables)stepContext.Result;
            if (pullRequestDetails == null)
            {
                await stepContext.Context.SendActivityAsync("Wrong configuration file", cancellationToken: cancellationToken);
            }

            var rez = await GlobalVariablesResolver.SetBitBucketConversationVariables(pullRequestDetails);
            if (!rez)
            {
                await stepContext.Context.SendActivityAsync("Could not update configuration file", cancellationToken: cancellationToken);
            }
            return await stepContext.EndDialogAsync(pullRequestDetails, cancellationToken);
        }
    }
}