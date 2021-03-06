﻿using ChatLe.Repository.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChatLe.Models
{
    public static class BuilderExtensions
    {
        public static IServiceCollection ConfigureChatLe(this IServiceCollection services, Action<ChatOptions> configure)
        {
            services.Configure(configure);
            return services;
        }

        public static IServiceCollection AddChatLe<TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection>(this IServiceCollection services, Action<ChatOptions> configure = null)
            where TUser : IdentityUser<string>, IChatUser<string>
            where TContext : DbContext
            where TConversation : Conversation<string>, new()
            where TAttendee : Attendee<string>, new()
            where TMessage : Message<string>, new()
            where TNotificationConnection : NotificationConnection<string>, new()
        {
            if (configure != null)
                services.ConfigureChatLe(configure);

            services.AddTransient<IChatStore<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, ChatStore<string, TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection, IdentityUserLogin<string>>>()
                .AddTransient<IChatManager<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, ChatManager<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>>()
                .AddSingleton<ILookupNormalizer, LookupNormalizer>()
                .AddScoped<SignInManager>();            

            return services;
        }

        public static IServiceCollection AddChatLe(this IServiceCollection services, Action<ChatOptions> configure = null)
        {            
            return services.AddChatLe<ChatLeUser, ChatLeIdentityDbContext, Conversation, Attendee, Message, NotificationConnection>(configure);
        }

        public static IApplicationBuilder UseChatLe(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            store.Init();
            return app;
        }
    }
}