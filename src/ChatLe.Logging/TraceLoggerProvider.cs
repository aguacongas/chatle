using Microsoft.Framework.Logging;
using System;

namespace ChatLe.Logging
{
    /// <summary>
    /// Provide Trace logger
    /// </summary>
    public class TraceLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string name)
        {
            return new TraceLogger(name);
        }
    }
}
