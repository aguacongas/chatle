using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication
{
    public class Program
    {
        public virtual void Configure(IApplicationBuilder app)
        {
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        public static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            var host = new WebHostBuilder()
				.UseKestrel()
                .UseContentRoot(currentDirectory)
                .UseConfiguration(config)
				.UseIISIntegration()
				.UseStartup<Program>()
				.Build();

			host.Run();
        }
    }
}
