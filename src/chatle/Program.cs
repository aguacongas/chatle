using ChatLe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace chatle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            string rootPath = Directory.GetCurrentDirectory();
            if (args.Length == 1)
                rootPath += '/' + args[0];

            var host = new WebHostBuilder()
                .UseContentRoot(rootPath)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseKestrel()
                .Build();

            host.Run();
        }
    }
}
