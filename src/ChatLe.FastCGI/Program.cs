using ChatLe.Hosting.FastCGI;
using Microsoft.Framework.Logging;
using System;
using System.Linq;
using System.Net;

namespace ChatLe.FastCGI
{
    public class Program
    {
        private readonly IServiceProvider _serviceProvider;

        public Program(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Main(string[] args)
        {
            var program = new Microsoft.AspNet.Hosting.Program(_serviceProvider);
            var mergedArgs = new[] { "--server", "ChatLe.FastCGI" }.Concat(args).ToArray();
            program.Main(mergedArgs);
        }
    }
}
