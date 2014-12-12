using Microsoft.AspNet.Builder;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;

namespace ChatLe.Models
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddChatLe<TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection>(this IServiceCollection services)
            where TUser : class, IChatUser<string>
            where TContext : DbContext
            where TConversation : Conversation<string>, new()
            where TAttendee : Attendee<string>, new()
            where TMessage : Message<string>, new()
            where TNotificationConnection : NotificationConnection<string>, new()
        {
            services.AddScoped<IChatStore<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, ChatStore<string, TUser, TContext, TConversation, TAttendee, TMessage, TNotificationConnection>>();
            services.AddScoped<IChatManager<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, ChatManager<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>>();
            return services;
        }

        public static IServiceCollection AddChatLe(this IServiceCollection services)
        {
            return services.AddChatLe<ChatLeUser, ChatLeIdentityDbContext, Conversation, Attendee, Message, NotificationConnection>();
        }

        public static void UseChatLe(this IApplicationBuilder app, int userPerPages = 50)
        {
            var store = app.ApplicationServices.GetRequiredService<IChatStore<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();
            store.Init();
            var manager = app.ApplicationServices.GetRequiredService<IChatManager<string, ChatLeUser, Conversation, Attendee, Message, NotificationConnection>>();

        }
    }
}