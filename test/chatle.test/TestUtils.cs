using ChatLe.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;

namespace chatle.test
{
    public class TestUtils
    {
        public static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.Add(HostingServices.GetDefaultServices());
            services.AddEntityFramework()
                .AddInMemoryStore()
                .AddDbContext<ChatLeIdentityDbContext>();
            services.Add(OptionsServices.GetDefaultServices())
                .AddInstance<ILoggerFactory>(new LoggerFactory());

            return services.BuildServiceProvider();
        }
    }
}