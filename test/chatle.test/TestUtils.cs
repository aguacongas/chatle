using ChatLe.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Chatle.test
{
    public class TestUtils
    {
        public static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddMvc();
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<ChatLeIdentityDbContext>();
            services.AddInstance<ILoggerFactory>(new LoggerFactory());
            services.AddIdentity<ChatLeUser, IdentityRole>();
            services.AddChatLe();
            return services.BuildServiceProvider();
        }
    }
}