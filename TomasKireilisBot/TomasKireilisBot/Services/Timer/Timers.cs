using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TomasKireilisBot.Services.Timer
{
    public class Timers
    {
        private readonly IAdapterIntegration _adapter;

        public Timers(IAdapterIntegration adapter)
        {
            _adapter = adapter;
        }

        public List<Timer> List { get; set; } = new List<Timer>();

        public async Task AddTimer(ConversationReference reference, int repeatRateInSeconds)
        {
            var timer = new Timer(_adapter, reference, repeatRateInSeconds, List.Count + 1);
            List.Add(timer);
            Task.Run(() => timer.Start());
        }

        public void RemoveTimers()
        {
            foreach (var timer in List)
            {
                timer.StopTimer();
            }
            List.Clear();
        }
    }
}