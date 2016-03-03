using ChatLe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace chatle.test
{
    public class StartupTest
    {
		[Fact]
		public void ConfigureTest()
		{
			var mockHostingEnvironment = new Mock<IHostingEnvironment>();
			mockHostingEnvironment.SetupGet(h => h.EnvironmentName).Returns("Development");
			var sartup = new Startup(mockHostingEnvironment.Object, new Mock<ILoggerFactory>().Object);
			var serviceCollection = new ServiceCollection();
			sartup.ConfigureServices(serviceCollection);
			var factory = new ApplicationBuilderFactory(serviceCollection.BuildServiceProvider());
			sartup.Configure(factory.CreateBuilder(new Mock<IFeatureCollection>().Object));
		}
	}
}
