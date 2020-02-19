using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.Services.Timer;
using QueueMessage = TomasKireilisBot.DataModels.QueueMessage;

namespace TomasKireilisBot.Dialogs
{
    public class ActivatePullRequestNotificationDialog : CancelAndHelpDialog
    {
        private readonly Timers _timers;

        public ActivatePullRequestNotificationDialog(Timers timers) : base(nameof(ActivatePullRequestNotificationDialog))
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
            ConversationReference reference = new ConversationReference(stepContext.Context.Activity.Id, stepContext.Context.Activity.Recipient, stepContext.Context.Activity.From);
            var queueMessage = new QueueMessage
            {
                ConversationReference = reference,
                Text = "PullRequestNotification"
            };
            await stepContext.Context.SendActivityAsync("Adding notification... please wait. It might take some time", cancellationToken: cancellationToken);
            await _timers.AddTimer(stepContext.Context.Activity.GetConversationReference(), 15);
            //try
            //{
            //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomasbotstorage;AccountKey=I4+e7qZyKNIiFsLR1R8XLn5AQVSq6DP3gRd3eHq2H/x3n44zUtLEqB5PIYee0PBGbQpo358xA/CUHtr/RAAPaw==;EndpointSuffix=core.windows.net");
            //    CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();
            //    CloudQueue queue = queueClient.GetQueueReference("botmessagebacklog");
            //    await queue.CreateIfNotExistsAsync();
            //    CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(queueMessage));
            //    await queue.AddMessageAsync(message);
            //}
            //catch (Exception e)
            //{
            //    await stepContext.Context.SendActivityAsync(e.Message, cancellationToken: cancellationToken);
            //    throw;
            //}

            //if(queue.PeekMessageAsync(,message,cancellationToken) != null)

            await stepContext.Context.SendActivityAsync("Notification added", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}