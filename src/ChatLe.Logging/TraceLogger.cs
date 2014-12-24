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

        public bool IsEnabled(TraceType eventType)
        {
            return true;
        }

        public void Write(TraceType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
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
                case TraceType.Verbose:
                case TraceType.Information:
                    Trace.TraceInformation(builder.ToString());
                    break;
                case TraceType.Warning:
                    Trace.TraceWarning(builder.ToString());
                    break;
                case TraceType.Critical:
                case TraceType.Error:
                    Trace.TraceError(builder.ToString());
                    break;
            }
        }
    }
}