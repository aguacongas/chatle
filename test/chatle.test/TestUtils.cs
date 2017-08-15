using ChatLe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
			services.AddDbContext<ChatLeIdentityDbContext>(options => options.UseInMemoryDatabase("test"));
			services.AddTransient<ILoggerFactory, LoggerFactory>();
			services.AddIdentity<ChatLeUser, IdentityRole>();
			services.AddChatLe();
			return services.BuildServiceProvider();
		}
	}
}