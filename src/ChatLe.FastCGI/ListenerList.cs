using ChatLe.Hosting.FastCGI;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace ChatLe.FastCGI
{
    public class ListenerList : List<IListener>, IDisposable
    {

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var listener in this)
                        listener.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        internal void AddListeners(ILoggerFactory loggerFactory, ServerInformation information)
        {
            foreach(var port in information.Ports)
            {
                var listener = new TcpListener(loggerFactory, new Configuration());
                listener.Start(new IPEndPoint(IPAddress.Loopback, port));
                Add(listener);
            }
        }
        #endregion

    }
}