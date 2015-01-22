using Microsoft.Framework.Logging;
using System;
using System.Diagnostics;
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

        public void Write(LogLevel eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
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

            switch (eventType)
            {
                case LogLevel.Verbose:
                case LogLevel.Information:
                    Trace.TraceInformation(builder.ToString());
                    break;
                case LogLevel.Warning:
                    Trace.TraceWarning(builder.ToString());
                    break;
                case LogLevel.Critical:
                case LogLevel.Error:
                    Trace.TraceError(builder.ToString());
                    break;
            }
        }
    }
}