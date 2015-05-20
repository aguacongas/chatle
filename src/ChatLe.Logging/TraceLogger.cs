using Microsoft.Framework.Logging;
using System;
#if !DNXCORE50
using System.Diagnostics;
#endif
using System.Text;

namespace ChatLe.Logging
{
    public class TraceLogger : ILogger
    {
        readonly string _name;
        public TraceLogger(string name)
        {
            _name = name;
        }
        public IDisposable BeginScope(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel eventType)
        {
            return true;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(_name);
            builder.Append("]: ");

            if (formatter != null)
                builder.Append(formatter(state, exception));
            else
            {
                if (state != null)
                    builder.Append(state);
                if (exception != null)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(exception);
                }
            }

            switch (logLevel)
            {
                case LogLevel.Verbose:
                case LogLevel.Information:
#if !DNXCORE50
                    Trace.TraceInformation(builder.ToString());
#else
                    Console.WriteLine(builder.ToString());
#endif
                    break;
                case LogLevel.Warning:
#if !DNXCORE50
                    Trace.TraceWarning(builder.ToString());
#else
                    Console.WriteLine(builder.ToString());
#endif
                    break;
                case LogLevel.Critical:
                case LogLevel.Error:
#if !DNXCORE50
                    Trace.TraceError(builder.ToString());
#else
                    Console.WriteLine(builder.ToString());
#endif
                    break;
            }
        }
    }
}