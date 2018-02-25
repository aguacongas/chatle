using Aguacongas.Firebase;
using ChatLe.Repository.Identity;
using ChatLe.Repository.Identity.Firebase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace ChatLe.Models
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddFirebaseChatStore<TUser, TConversation, TAttendee, TMessage, TNotificationConnection>(this IServiceCollection services)
            where TUser : IdentityUser<string>, IChatUser<string>
            where TConversation : Conversation<string>, new()
            where TAttendee : Attendee<string>, new()
            where TMessage : Message<string>, new()
            where TNotificationConnection : NotificationConnection<string>, new()
        {
            services.AddTransient<IChatStore<string, TUser, TConversation, TAttendee, TMessage, TNotificationConnection>, FirebaseChatStore<TUser, TConversation, TAttendee, TMessage, TNotificationConnection>>();

            return services;
        }
    }
}