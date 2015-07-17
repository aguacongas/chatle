using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;

namespace ChatLe.HttpUtility
{
    public static class RemoveResponseHeaderServicesExtensions
    {
        public static IServiceCollection AddRemoveResponseHeaders(this IServiceCollection services, IConfiguration config = null, Action<CommaSeparatedListOptions> configure = null)
        {
            if (config != null)
                services.Configure<CommaSeparatedListOptions>(config);
            if (configure != null)
                services.Configure(configure);
            return services;
        }
    }
}