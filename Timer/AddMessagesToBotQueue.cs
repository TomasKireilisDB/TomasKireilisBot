using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;

namespace Timer
{
    public static class AddMessagesToBotQueue
    {
        [FunctionName("AddMessagesToBotQueue")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"Gathering queue messages");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomasbotstorage;AccountKey=I4+e7qZyKNIiFsLR1R8XLn5AQVSq6DP3gRd3eHq2H/x3n44zUtLEqB5PIYee0PBGbQpo358xA/CUHtr/RAAPaw==;EndpointSuffix=core.windows.net");
            log.LogInformation($"Connected to Cloud Storage");
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();
            log.LogInformation($"Added queue client");
            CloudQueue backLog = queueClient.GetQueueReference("botmessagebacklog");
            await backLog.CreateIfNotExistsAsync();
            CloudQueue botQueue = queueClient.GetQueueReference("botqueue");
            await backLog.CreateIfNotExistsAsync();
            log.LogInformation($"Got queues");
            var cloudBackLogMessages = await backLog.GetMessagesAsync(10, TimeSpan.FromDays(7), new QueueRequestOptions(), new OperationContext());
            log.LogInformation($"Gathered messages backlog");
            foreach (var cloudQueueMessage in cloudBackLogMessages)
            {
                await botQueue.AddMessageAsync(cloudQueueMessage, TimeSpan.FromMinutes(5), TimeSpan.Zero, new QueueRequestOptions(), new OperationContext());
                log.LogInformation($"Message was moved into bot queue");
                await backLog.AddMessageAsync(cloudQueueMessage, TimeSpan.FromDays(7), TimeSpan.Zero, new QueueRequestOptions(), new OperationContext());
                log.LogInformation($"Message was recreated in backlog");
            }
        }
    }
}