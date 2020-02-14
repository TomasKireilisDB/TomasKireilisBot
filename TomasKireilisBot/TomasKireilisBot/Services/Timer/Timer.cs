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
        private bool _active = true;

        public async Task Start()
        {
            await _adapter.ContinueConversationAsync("Not-important for emulator", ConversationReference, SendMessageAsync);
            while (_active)
            {
                StartedAt = DateTime.Now;
                Status = "Running";

                await Task.Delay(Seconds * 1000);

                FinishedAt = DateTime.Now;
                Status = "Finished";
                if (_active)
                {
                    await _adapter.ContinueConversationAsync("5dd4aa78-9c8c-4486-9005-e2579e6ec5e1", ConversationReference, SendMessageAsync);
                }
            }
        }

        public void StopTimer()
        {
            _active = false;
        }

        private async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var activity = new Activity("Invoke");
            await turnContext.SendActivityAsync(activity, cancellationToken: cancellationToken);
        }
    }
}