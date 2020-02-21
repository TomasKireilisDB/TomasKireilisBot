using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
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

            await stepContext.Context.SendActivityAsync(
                JsonConvert.SerializeObject(
                await GlobalVariablesService.GetBitBucketConversationVariables(
                    stepContext.Context.Activity.Recipient.Id)), cancellationToken: cancellationToken);

            var reply = MessageFactory.Text("Please provide configuration file");
            try
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
            }
            catch
            {
                await stepContext.Context.SendActivityAsync("Warning. Did not receive configuration file. Configuration file remain unchanged", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result == null)
            {
                await stepContext.Context.SendActivityAsync("Wrong configuration file", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            BitBucketConversationVariables pullRequestDetails;
            try
            {
                pullRequestDetails = JsonConvert.DeserializeObject<BitBucketConversationVariables>(stepContext.Result.ToString());
            }
            catch (Exception e)
            {
                await stepContext.Context.SendActivityAsync("Wrong configuration file", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            if (pullRequestDetails == null)
            {
                await stepContext.Context.SendActivityAsync("Wrong configuration file", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            var rez = await GlobalVariablesService.SetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id, pullRequestDetails);
            if (!rez)
            {
                await stepContext.Context.SendActivityAsync("Could not update configuration file", cancellationToken: cancellationToken);
            }
            await stepContext.Context.SendActivityAsync("Configuration file is updated", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(pullRequestDetails, cancellationToken);
        }
    }
}