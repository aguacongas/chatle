using ChatLe.Hosting.FastCGI.Payloads;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatLe.Hosting.FastCGI
{
    public class TcpListener : Listener<IPEndPoint>
    {
        public TcpListener(ILoggerFactory loggerFactory, IListernerConfiguration configuration, Func<object, Task> app) :base(loggerFactory, configuration, app)
        {
        }
        protected override Socket CreateSocket(IPEndPoint enpoint)
        {
            var socket = new Socket(enpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            return socket;
        }
    }

    public abstract class Listener<T> : IDisposable, IListener<T> where T : EndPoint
    {
        ILogger _logger;
        public IListernerConfiguration Configuration { get; private set; }
        public Func<object, Task> App { get; private set; }
        public Listener(ILoggerFactory loggerFactory, IListernerConfiguration configuration, Func<object, Task> app)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            if (app == null)
                throw new ArgumentNullException("app");

            _logger = loggerFactory.Create<Listener<T>>();
            Configuration = configuration;
            App = app;
        }

        protected abstract Socket CreateSocket(T endpoint);

        Socket _listener;
        public void Start(T endpoint)
        {
            try
            {
                _listener = CreateSocket(endpoint);
                _listener.Bind(endpoint);
                _listener.Listen(Configuration.MaxConnections);
                BeginAccept();
            }
            catch (Exception e)
            {
                _logger.WriteError("UnHandled exception on Start", e);
                throw;
            }
        }

        private void BeginAccept()
        {
            _listener.BeginAccept(result => {

                Socket client = null;
                try
                {
                    client = _listener.EndAccept(result);                    
                }
                catch (ObjectDisposedException) // server shutdown
                {
                    return;
                }
                catch(Exception e)
                {
                    _logger.WriteError("UnHandled exception on EndAccecpt", e);
                    throw;
                }

                BeginAccept();

                if (client != null)
                {
                    BeginRead(client);
                }

            }, null);
        }

        private void BeginRead(Socket client)
        {
            try
            {
                var state = new HeaderState(client, this, _logger);
                client.BeginReceive(state.Buffer, 0, state.Length, SocketFlags.None, state.EndReceive, state);
            }
            catch(Exception e)
            {
                _logger.WriteError("Error on Read", e);
                ReceiveState.OnDisconnect(client);
            }
        }
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _listener.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}