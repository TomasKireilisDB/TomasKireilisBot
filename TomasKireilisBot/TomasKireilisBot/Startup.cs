// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TomasKireilisBot.Bots;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Dialogs;
using TomasKireilisBot.Helpers;
using TomasKireilisBot.Services.BitbucketService;
using TomasKireilisBot.Services.Timer;

namespace TomasKireilisBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            services.AddSingleton<IAdapterIntegration, BotFrameworkHttpAdapter>();
            services.AddSingleton<Timers>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            services.AddSingleton<CheckActivePullRequestsDialog>();

            services.AddSingleton<ChangePullRequestsConfigurationDialog>();

            services.AddSingleton<ActivatePullRequestNotificationDialog>();

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();

            services.AddSingleton<BitBucketConversationVariables>();
            services.AddSingleton<IInnerBitbucketClient, InnerBitbucketClient>();

            services.AddSingleton(new ConcurrentDictionary<string, ConversationReference>());
            //       services.AddSingleton<IConfiguration>(new ConfigurationRoot(new List<IConfigurationProvider>()));

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseMvc();
        }
    }
}