using ChatLe.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;

namespace chatle.test
{
    public class TestUtils
    {
        public static IServiceProvider GetServiceProvider()
        {
            var services = HostingServices.Create();
            services.AddEntityFramework()
                .AddInMemoryStore()
                .AddDbContext<ChatLeIdentityDbContext>();
            services.AddInstance<ILoggerFactory>(new LoggerFactory());
            services.AddIdentity<ChatLeUser, IdentityRole>();
            services.AddChatLe();
            return services.BuildServiceProvider();
        }
    }
}