using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Timer
{
    public static class PublishMessageToBot
    {
        [FunctionName("PublishMessageToBot")]
        public static void Run([QueueTrigger("botqueue", Connection = "DefaultEndpointsProtocol=https;AccountName=tomasbotstorage;AccountKey=I4+e7qZyKNIiFsLR1R8XLn5AQVSq6DP3gRd3eHq2H/x3n44zUtLEqB5PIYee0PBGbQpo358xA/CUHtr/RAAPaw==;EndpointSuffix=core.windows.net")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}