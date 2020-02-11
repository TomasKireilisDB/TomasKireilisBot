using Microsoft.Bot.Schema;

namespace TomasKireilisBot.DataModels
{
    public class QueueMessage
    {
        public ConversationReference ConversationReference;
        public string Text;
    }
}