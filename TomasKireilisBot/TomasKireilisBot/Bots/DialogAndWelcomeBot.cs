// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.Helpers;

namespace TomasKireilisBot.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T>
        where T : Dialog
    {
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    bool test = true;
                    try
                    {
                        test = await GlobalVariablesService.SetDefaultBitBucketConversationVariables(member.Id);
                    }
                    catch (Exception e)
                    {
                        await turnContext.SendActivityAsync("Could not establish connection between bot and database. Some features may not work correctly", cancellationToken: cancellationToken);
                        await turnContext.SendActivityAsync(e.Message, cancellationToken: cancellationToken);
                    }

                    var reply = MessageFactory.Text("Hello");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }
    }
}