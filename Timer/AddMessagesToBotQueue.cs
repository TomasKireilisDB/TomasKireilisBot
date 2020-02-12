using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

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
            CloudQueue queue = queueClient.GetQueueReference("botmessagebacklog");
            await queue.CreateIfNotExistsAsync();
            log.LogInformation($"Got queue");
            var cloudQueueMessages = await queue.GetMessagesAsync(10, TimeSpan.FromDays(7), new QueueRequestOptions(), new OperationContext());
            log.LogInformation($"Gathered messages queue");
            foreach (var cloudQueueMessage in cloudQueueMessages)
            {
                log.LogInformation($"A message");
                await queue.AddMessageAsync(cloudQueueMessage);
            }
        }
    }
}