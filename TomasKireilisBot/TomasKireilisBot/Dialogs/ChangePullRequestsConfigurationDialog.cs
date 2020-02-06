using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace TomasKireilisBot.Dialogs
{
    public class ChangePullRequestsConfigurationDialog : CancelAndHelpDialog
    {
        public ChangePullRequestsConfigurationDialog() : base(nameof(ChangePullRequestsConfigurationDialog))
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
            var pullRequestDetails = (PullRequestDetails)stepContext.Options;

            return await stepContext.EndDialogAsync(pullRequestDetails, cancellationToken);
        }
    }
}