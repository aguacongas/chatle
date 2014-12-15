using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;

namespace ChatLe.HttpUtility
{
    public static class RemoveResponseHeaderServicesExtensions
    {
        public static IServiceCollection AddRemoveResponseHeaders(this IServiceCollection services, IConfiguration config = null, Action<RemoveResponseHeardersOptions> configure = null)
        {
            if (config != null)
                services.Configure<RemoveResponseHeardersOptions>(config);
            if (configure != null)
                services.Configure(configure);
            return services;
        }
    }
}