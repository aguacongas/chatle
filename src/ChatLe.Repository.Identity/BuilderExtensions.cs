using ChatLe.Repository.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            where TUser : class, IChatUser<string>
            where TContext : DbContext
            where TConversation : Conversation<string>, new()
            where TAttendee : Attendee<string>, new()
            where TMessage : Message<string>, new()
            where TNotificationConnection : NotificationConnection<string>, new()
        {
            if (configure != null)
                services.ConfigureChatLe(configure);
            
            services.AddScoped<IChatStore<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, ChatStore<string, TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection>>();
            services.AddScoped<IChatManager<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, ChatManager<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>>();
            services.AddScoped<SignInManager>();

            return services;
        }

        public static IServiceCollection AddChatLe(this IServiceCollection services, Action<ChatOptions> configure = null)
        {            
            return services.AddChatLe<ChatLeUser, ChatLeIdentityDbContext, Conversation, Attendee, Message, NotificationConnection>(configure);
        }

        public static IApplicationBuilder UseChatLe(this IApplicationBuilder app)
        {
            var store = app.ApplicationServices.GetRequiredService<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();            
            store.Init();
            return app;
        }
    }
}