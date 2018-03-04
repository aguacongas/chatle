using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chatle.Hubs
{
    public enum TransportType
    {
        WebSockets = 0,
        ServerSentEvents = 1,
        LongPolling = 2,
    }

    public enum LogLevel
    {
        Trace = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        None = 4,
    }

    public class HubSettings
    {
        public string Url { get; set; }
        public TransportType TransportType { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
