// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.Services.BitbucketService;

namespace TomasKireilisBot.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T>
        where T : Dialog
    {
        public DialogAndWelcomeBot(IInnerBitbucketClient innerBitbucketClient, ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(innerBitbucketClient, conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text("Hello");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }
    }
}