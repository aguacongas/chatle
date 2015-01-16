using Microsoft.AspNet.Hosting.Server;
using System;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.ConfigurationModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Framework.Logging;

namespace ChatLe.FastCGI
{
    public class ServerFactory : IServerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public ServerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public IServerInformation Initialize(IConfiguration configuration)
        {
            return new ServerInformation();
        }

        public IDisposable Start(IServerInformation serverInformation, Func<object, Task> application)
        {
            var listeners = new ListenerList();
            var information = serverInformation as ServerInformation;
            listeners.AddListeners(_loggerFactory, information);

            return listeners;
        }
    }

    public class ServerInformation : IServerInformation
    {
        public string Name
        {
            get
            {
                return "ChatLe.FastCGI";
            }
        }

        public List<int> Ports { get; } = new List<int>();
        public void Initialize(IConfiguration configuration)
        {
            string ports;
            if(!configuration.TryGet("ports", out ports))
            {
                Ports.Add(9000);
            }
            foreach(var port in ports.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Ports.Add(int.Parse(port));
            }
        }
    }
}