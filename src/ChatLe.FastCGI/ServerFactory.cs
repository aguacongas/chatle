using Microsoft.AspNet.Hosting.Server;
using System;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.ConfigurationModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Framework.Logging;
using ChatLe.Hosting.FastCGI;
using System.Net;

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
            var informatio =  new ServerInformation();
            informatio.Initialize(configuration);
            return informatio;
        }

        public IDisposable Start(IServerInformation serverInformation, Func<object, Task> application)
        {
            var information = serverInformation as ServerInformation;
            var listener = new TcpListener(_loggerFactory, information, application);
            listener.Start(new IPEndPoint(IPAddress.Loopback, information.Port));
            return listener;
        }
    }

    public class ServerInformation : ListernerConfiguration, IServerInformation
    {
        public string Name
        {
            get
            {
                return "ChatLe.FastCGI";
            }
        }

        public int Port { get; private set; } =9000;
        public void Initialize(IConfiguration configuration)
        {
            string port;
            if(configuration.TryGet("fastcgi.port", out port))
            {
                Port = int.Parse(port);
            }
            string maxConnections;
            if (configuration.TryGet("fastcgi.maxConnections", out maxConnections))
            {
                MaxConnections = int.Parse(maxConnections);
            }
            string maxRequests;
            if (configuration.TryGet("fastcgi.maxRequests", out maxRequests))
            {
                MaxRequests = int.Parse(maxRequests);
            }
            string supportMultiplexing;
            if (configuration.TryGet("fastcgi.supportMultiplexing", out supportMultiplexing))
            {
                SupportMultiplexing = bool.Parse(supportMultiplexing);
            }
        }
    }
}