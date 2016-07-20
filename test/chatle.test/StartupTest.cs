using ChatLe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using Xunit;

namespace chatle.test
{
    public class StartupTest
    {
		
		[Fact(Skip = "Skipped for the moment")]
		public void ConfigureTest()
		{
			var mockHostingEnvironment = new Mock<IHostingEnvironment>();
			mockHostingEnvironment.SetupGet(h => h.EnvironmentName).Returns("Development");
			mockHostingEnvironment.SetupGet(h => h.ContentRootPath).Returns(Directory.GetCurrentDirectory());
			mockHostingEnvironment.SetupGet(h => h.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroo"));
			var startup = new Startup(mockHostingEnvironment.Object, new Mock<ILoggerFactory>().Object);
			var serviceCollection = new ServiceCollection();
			startup.ConfigureServices(serviceCollection);
			var factory = new ApplicationBuilderFactory(serviceCollection.BuildServiceProvider());
			startup.Configure(factory.CreateBuilder(new Mock<IFeatureCollection>().Object));
		}
	}
}
