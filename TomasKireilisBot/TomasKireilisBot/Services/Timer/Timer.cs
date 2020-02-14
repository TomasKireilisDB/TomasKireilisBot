using System;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace TomasKireilisBot.Services.Timer
{
    public class Timer
    {
        private readonly IAdapterIntegration _adapter;

        public Timer(
            IAdapterIntegration adapter,
            ConversationReference conversationReference,
            int seconds,
            int number)
        {
            _adapter = adapter;

            ConversationReference = conversationReference;
            Seconds = seconds;
            Number = number;
        }

        public ConversationReference ConversationReference { get; }
        public double Elapsed => ((FinishedAt ?? DateTime.Now) - (StartedAt ?? DateTime.Now)).TotalMilliseconds;
        public DateTime? FinishedAt { get; private set; }
        public int Number { get; }
        public int Seconds { get; }
        public DateTime? StartedAt { get; private set; }
        public string Status { get; private set; } = "Started";

        public async Task Start()
        {
            StartedAt = DateTime.Now;
            Status = "Running";

            await Task.Delay(Seconds * 1000);

            FinishedAt = DateTime.Now;
            Status = "Finished";

            await _adapter.ContinueConversationAsync("not-important-for-emulator", ConversationReference, SendMessageAsync);
        }

        private async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync($"Timer #{Number} finished! ({Seconds})s");
        }
    }
}