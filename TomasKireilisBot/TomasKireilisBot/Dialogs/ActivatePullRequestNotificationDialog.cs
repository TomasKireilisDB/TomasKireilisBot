using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using Newtonsoft.Json;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Helpers;
using QueueMessage = TomasKireilisBot.DataModels.QueueMessage;

namespace TomasKireilisBot.Dialogs
{
    public partial class ActivatePullRequestNotificationDialog : CancelAndHelpDialog
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
            ConversationReference reference = new ConversationReference(Id);
            var queueMessage = new QueueMessage
            {
                ConversationReference = reference,
                Text = "PullRequestNotification"
            };
            await stepContext.Context.SendActivityAsync("Adding notification... please wait. It might take some time", cancellationToken: cancellationToken);
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomasbotstorage;AccountKey=I4+e7qZyKNIiFsLR1R8XLn5AQVSq6DP3gRd3eHq2H/x3n44zUtLEqB5PIYee0PBGbQpo358xA/CUHtr/RAAPaw==;EndpointSuffix=core.windows.net");
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("Botqueue");
            await queue.CreateIfNotExistsAsync();
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(queueMessage));
            await queue.AddMessageAsync(message);
            //if(queue.PeekMessageAsync(,message,cancellationToken) != null)

            await stepContext.Context.SendActivityAsync("Notification added", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}