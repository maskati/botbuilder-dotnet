﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Adapters
{
    public class ConsoleBot : BotBase
    {
        public ConsoleBot() : base()
        {
        }

        public async Task ProcessActivity(Func<IBotContext, Task> callback = null)
        {
            while (true)
            {
                var msg = Console.ReadLine();
                if (msg == null)
                    break;

                var activity = new Activity()
                {
                    Text = msg,
                    ChannelId = "console",
                    From = new ChannelAccount(id: "user", name: "User1"),
                    Recipient = new ChannelAccount(id: "bot", name: "Bot"),
                    Conversation = new ConversationAccount(id: "Convo1"),
                    Timestamp = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    Type = ActivityTypes.Message
                };

                await this.ProcessActivityInternal(activity, callback);
            }
        }

        protected override async Task SendActivityImplementation(IBotContext context, IActivity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    {
                        IMessageActivity message = activity.AsMessageActivity();
                        if (message.Attachments != null && message.Attachments.Any())
                        {
                            var attachment = message.Attachments.Count == 1 ? "1 attachments" : $"{message.Attachments.Count()} attachments";
                            Console.WriteLine($"{message.Text} with {attachment} ");
                        }
                        else
                        {
                            Console.WriteLine($"{message.Text}");
                        }
                    }
                    break;
                case "delay":
                    {
                        // The Activity Schema doesn't have a delay type build in, so it's simulated
                        // here in the Bot. This matches the behavior in the Node connector. 
                        int delayMs = (int)((Activity)activity).Value;
                        await Task.Delay(delayMs).ConfigureAwait(false);
                    }
                    break;
                default:
                    Console.WriteLine("Bot: activity type: {0}", activity.Type);
                    break;
            }
        }

        protected override Task<ResourceResponse> UpdateActivityImplementation(IBotContext context, IActivity activity)
        {
            throw new NotImplementedException();
        }

        protected override Task DeleteActivityImplementation(IBotContext context, string conversationId, string activityId)
        {
            throw new NotImplementedException();
        }

        protected override Task CreateConversationImplementation()
        {
            throw new NotImplementedException();
        }
    }
}
