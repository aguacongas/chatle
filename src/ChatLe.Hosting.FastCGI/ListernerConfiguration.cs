using System;

namespace ChatLe.Hosting.FastCGI
{
    public class ListernerConfiguration : IListernerConfiguration
    {
        public int MaxConnections { get; set; } = ushort.MaxValue;
        public int MaxRequests { get; set; } = ushort.MaxValue;
        public bool SupportMultiplexing { get; set; } = true;
    }
}