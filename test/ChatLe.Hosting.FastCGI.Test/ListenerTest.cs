using Microsoft.Framework.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace ChatLe.Hosting.FastCGI.Test
{
    public class ListenerTest
    {
        [Fact]
        public void BeginRequestTest()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new Logging.TraceLoggerProvider());
            var listener = new TcpListener(loggerFactory, new Configuration());
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            listener.Start(endpoint);
            using (var client = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                client.ReceiveTimeout = 500;
                client.Connect(endpoint);
                var data = new byte[] { 1, 1, 0, 1, 0, 3, 2, 0, 0, 1, 1, 255, 255 };
                client.Send(data);
                Thread.Sleep(5000);
                data = new byte[] { 1, 1, 0, 2, 0, 3 };
                client.Send(data);
                Thread.Sleep(5000);
                data = new byte[] { 2, 0, 0, 1, 1, 255, 255 };
                client.Send(data);
                Thread.Sleep(5000);
                data = new byte[] { 1, 1, 0, 3, 0, 3, 2, 0, 0, 1, 1, 255, 255, 1, 1, 0, 4, 0, 3, 1, 0, 0, 1, 1, 255 };
                client.Send(data);
                Thread.Sleep(5000);
            }
        }

        [Fact]
        public void GetValuesTest()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new Logging.TraceLoggerProvider());
            var configuration = new Configuration();
            var listener = new TcpListener(loggerFactory, configuration);
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
            listener.Start(endpoint);
            using (var client = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(endpoint);
                var data = new byte[] { 1, 1, 0, 1, 0, 3, 2, 0, 0, 1, 1, 255, 255 };
                client.Send(data);

                var buffer = new byte[] { 1, 9, 0, 1, 0, 0, 0, 0 };

                using (var stream = new MemoryStream())
                {
                    NameValuePairsSerializer.Write(stream, "FCGI_MAX_CONNS", configuration.MaxConnections.ToString());
                    NameValuePairsSerializer.Write(stream, "FCGI_MAX_REQS", configuration.MaxRequests.ToString());
                    NameValuePairsSerializer.Write(stream, "FCGI_MPXS_CONNS", configuration.SupportMultiplexing ? "1" : "0");
                    buffer = buffer.Concat(stream.ToArray()).ToArray();
                    buffer[4] = (byte)(stream.Length >> 8);
                    buffer[5] = (byte)stream.Length;
                    client.Send(buffer);
                    var rec = new byte[100];
                    client.Receive(rec);
                }
            }
        }
    }
}