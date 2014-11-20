using ChatLe.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using Xunit;

namespace chatle.test
{
    public class IdentityModelTest
    {

        IServiceProvider _provider;

        public IdentityModelTest()
        {
            var services = new ServiceCollection();
            services.Add(HostingServices.GetDefaultServices());
            services.AddEntityFramework()
                .AddInMemoryStore()
                .AddDbContext<ApplicationDbContext>();
            services.Add(OptionsServices.GetDefaultServices())
                .AddInstance<ILoggerFactory>(new LoggerFactory());

            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void SetConnectionStatusTest()
        {
            var context = _provider.GetService<ApplicationDbContext>();
            context.SetConnectionStatus("test", "test", true);
        }
    }
}
