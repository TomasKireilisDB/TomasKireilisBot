using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        private bool _active = true;

        public async Task Start()
        {
            await _adapter.ContinueConversationAsync(ConversationReference.Bot.Id, ConversationReference, SendMessageAsync);
            while (_active)
            {
                StartedAt = DateTime.Now;
                Status = "Running";

                await Task.Delay(Seconds * 1000);

                FinishedAt = DateTime.Now;
                Status = "Finished";
                if (_active)
                {
                    await _adapter.ContinueConversationAsync(ConversationReference.Bot.Id, ConversationReference, SendMessageAsync);
                }
            }
        }

        public void StopTimer()
        {
            _active = false;
        }

        private async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // var activity = new Activity("Invoke");
            await turnContext.SendActivityAsync("Hey here is pull request", cancellationToken: cancellationToken);
        }
    }
}