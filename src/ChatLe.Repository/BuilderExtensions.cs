using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;

namespace ChatLe.Models
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddChatLe<TUser, TContext>(this IServiceCollection services) 
            where TUser :class, IApplicationUser
            where TContext : DbContext
        {
            services.AddScoped<IChatStore<TUser>, ChatStore<string, TUser, TContext>>();
            services.AddScoped<ChatManager<TUser>>();
            return services;
        }
    }
}