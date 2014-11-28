using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;

namespace ChatLe.Models
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddChatLe<TUser, TContext, TConversation, TAttendee, TMessage>(this IServiceCollection services)
            where TUser : class, IChatUser<string>
            where TContext : DbContext
            where TConversation : Conversation<string>
            where TAttendee : Attendee<string>
            where TMessage : Message<string>
        {
            services.AddScoped<IChatStore<string, TUser, TConversation, TAttendee, TMessage>, ChatStore<string, TUser, TContext, Conversation, Attendee, Message>>();
            services.AddScoped<IChatManager<string, TUser, TConversation, TAttendee, TMessage>, ChatManager<string, TUser, Conversation, Attendee, Message>>();
            return services;
        }

        public static IServiceCollection AddChatLe(this IServiceCollection services)
        {
            services.AddScoped<IChatStore<string, ChatLeUser, Conversation, Attendee, Message>, ChatStore>();
            services.AddScoped<IChatManager<string, ChatLeUser, Conversation, Attendee, Message>, ChatManager>();
            return services;

        }
    }
}