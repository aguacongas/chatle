using System;

namespace ChatLe.Hosting.FastCGI
{
    public class Configuration : IConfiguration
    {
        public int MaxConnections { get; set; } = ushort.MaxValue;
        public int MaxRequests { get; set; } = ushort.MaxValue;
        public bool SupportMultiplexing { get; set; } = true;
    }
}