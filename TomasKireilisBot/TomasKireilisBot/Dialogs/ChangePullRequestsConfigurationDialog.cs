using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Helpers;

namespace TomasKireilisBot.Dialogs
{
    public class ChangePullRequestsConfigurationDialog : CancelAndHelpDialog
    {
        public ChangePullRequestsConfigurationDialog() : base(nameof(ChangePullRequestsConfigurationDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
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
            await stepContext.Context.SendActivityAsync("This is current configuration:", cancellationToken: cancellationToken);
            //var fileActivity = new Activity();
            //fileActivity.Attachments = new List<Attachment>();
            //var configFile = new Attachment();
            //configFile.Content = GlobalVariablesService.GetJsonGlobalVariables();
            //fileActivity.Attachments.Add(configFile);
            await stepContext.Context.SendActivityAsync(await GlobalVariablesService.GetJsonGlobalVariables(), cancellationToken: cancellationToken);
            //    await stepContext.Context.SendActivityAsync("Please provide configuration file", cancellationToken: cancellationToken);
            var reply = MessageFactory.Text("Please provide configuration file");
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var pullRequestDetails = JsonConvert.DeserializeObject<BitBucketConversationVariables>(stepContext.Result.ToString());
            if (pullRequestDetails == null)
            {
                await stepContext.Context.SendActivityAsync("Wrong configuration file", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            var rez = await GlobalVariablesService.SetBitBucketConversationVariables(pullRequestDetails);
            if (!rez)
            {
                await stepContext.Context.SendActivityAsync("Could not update configuration file", cancellationToken: cancellationToken);
            }
            await stepContext.Context.SendActivityAsync("Configuration file is updated", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(pullRequestDetails, cancellationToken);
        }
    }
}