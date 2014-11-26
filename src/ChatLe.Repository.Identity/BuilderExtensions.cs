using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;

namespace ChatLe.Models
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddChatLe<TUser, TContext>(this IServiceCollection services) 
            where TUser :class, IApplicationUser<string>
            where TContext : DbContext
        {
            services.AddScoped<IChatStore<string, TUser>, ChatStore<string, TUser, TContext>>();
            services.AddScoped<IChatManager<string, TUser>, ChatManager<string, TUser>>();
            return services;
        }
    }
}