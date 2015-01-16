using ChatLe.Hosting.FastCGI.Payloads;
using Microsoft.AspNet.HttpFeature;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ChatLe.Hosting.FastCGI
{
    abstract class State
    {
        public const int HEADER_LENGTH = 8;
        public virtual int Length { get { return HEADER_LENGTH; } }
        public ILogger Logger { get; private set; }
        public int Offset { get; set; }
        public Socket Socket { get; private set; }
        public IListener Listener { get; private set; }

        public State(Socket socket, IListener listener, ILogger logger)
        {
            if (socket == null)
                throw new ArgumentNullException("soket");
            if (listener == null)
                throw new ArgumentNullException("listener");
            if (logger == null)
                throw new ArgumentNullException("logger");

            Socket = socket;
            Listener = listener;
            Logger = logger;
        }

        static public void OnDisconnect(Socket client)
        {
            if (client == null)
                return;

            try
            {
                client.Shutdown(SocketShutdown.Both);
            }
            catch
            { }
            try
            {
                client.Disconnect(false);
            }
            catch
            { }
            try
            {
                client.Dispose();
            }
            catch
            { }
        }
    }

    abstract class ReceiveState :State
    {
        public Record Record { get; set; } = new Record();
        byte[] _buffer;
        public virtual byte[] Buffer
        {
            get
            {
                if (_buffer == null)
                    _buffer = new byte[Length];

                return _buffer;
            }
        }


        public ReceiveState(Socket socket, IListener listener, ILogger logger) :base(socket, listener, logger)
        { }

        protected int GetMaxToReadSize()
        {
            var toRead = Length - Offset;
            return toRead > Socket.ReceiveBufferSize ? Socket.ReceiveBufferSize : toRead;
        }

        public abstract void Process();

        public void EndReceive(IAsyncResult result)
        {
            var state = result.AsyncState as ReceiveState;
            if (state == null)
                return;

            Socket client = state.Socket;
            if (client == null)
                return;

            try
            {
                SocketError error;
                var read = client.EndReceive(result, out error);
                if (error != SocketError.Success || read <= 0)
                {
                    OnDisconnect(client);
                    return;
                }
                
                if (state.Offset + read < state.Length)
                {
                    state.Offset += read;
                    client.BeginReceive(state.Buffer, state.Offset, state.GetMaxToReadSize(), SocketFlags.None, EndReceive, state);                    
                }                    
                else
                    state.Process();
            }
            catch(ObjectDisposedException)
            { }
            catch (Exception e)
            {
                Logger.WriteError("Unhanlded exception on EndReceive", e);
                OnDisconnect(client);
            }
        }

       
    }

    class HeaderState : ReceiveState
    {
        public HeaderState(Socket socket, IListener listener, ILogger logger) :base(socket, listener, logger)
        { }

        public override void Process()
        {
            Record.Version = Buffer[0];
            Record.Type = Buffer[1];
            Record.RequestId = (ushort)((Buffer[2] << 8) + Buffer[3]);
            Record.Length = (ushort)((Buffer[4] << 8) + Buffer[5]);
            Record.Padding = Buffer[6];

            Logger.WriteVerbose(string.Format("HeaderState Process Record type {0} id {1} length {2} padding {3}", (RecordType) Record.Type, Record.RequestId, Record.Length, Record.Padding));
            var bodyState = new BodyState(Socket, Listener, Logger) { Record = Record };
            if (Record.Length > 0)
                Socket.BeginReceive(bodyState.Buffer, 0, bodyState.Length, SocketFlags.None, EndReceive, bodyState);
            else
                bodyState.Process();
        }
    }

    class BodyState : ReceiveState
    {
        public BodyState(Socket socket, IListener listener, ILogger logger) : base(socket, listener, logger)
        { }

        public override int Length
        {
            get
            {
                return Record.Length;
            }
        }

        public override void Process()
        {
            Logger.WriteVerbose(string.Format("BodyState Process Record type {0} id {1} length {2} padding {3}", (RecordType)Record.Type, Record.RequestId, Record.Length, Record.Padding));

            if (Record.Type != 0
                && Record.Type != 1
                && Record.Type != 2
                && Record.Type != 4
                && Record.Type != 5
                && Record.Type != 9)
            {
                var buffer = new byte[] { Record.Version, (byte)RecordType.Unknown, (byte)(Record.RequestId >> 8), (byte)Record.RequestId, 0, 2, 0, 0, Record.Type, 0 };
                var state = new SendState(Socket, Listener, Logger, buffer);
                state.BeginSend();
            }

            switch ((RecordType)Record.Type)
            {
                case RecordType.BeginRequest:
                    Listener.SetRequest(new Context(Record.RequestId, (Buffer[2] & 1) == 1));
                    Receive();
                    break;
                case RecordType.AbortRequest:
                    ProcessAbort();
                    break;
                case RecordType.Params:
                    ProcessParams();                    
                    break;
                case RecordType.StdInput:
                    ProcessStdInput();
                    break;
                case RecordType.GetValues:
                    ProcessGetValues();
                    break;
            }
        }

        private void ProcessGetValues()
        {
            var request = Listener.GetRequest(Record.RequestId);
            if (request != null)
            {                
                var @params = NameValuePairsSerializer.Parse(Buffer);
                var buffer = new byte[] { Record.Version, (byte)RecordType.GetValuesResult, (byte)(Record.RequestId >> 8), (byte)Record.RequestId, 0, 0, 0 };
                
                using (var stream = new MemoryStream())
                {
                    foreach (var kv in @params)
                    {
                        switch (kv.Key)
                        {
                            case "FCGI_MAX_CONNS":
                                NameValuePairsSerializer.Write(stream, "FCGI_MAX_CONNS", Listener.Configuration.MaxConnections.ToString());
                                break;
                            case "FCGI_MAX_REQS":
                                NameValuePairsSerializer.Write(stream, "FCGI_MAX_REQS", Listener.Configuration.MaxRequests.ToString());
                                break;
                            case "FCGI_MPXS_CONNS":
                                NameValuePairsSerializer.Write(stream, "FCGI_MPXS_CONNS", Listener.Configuration.SupportMultiplexing ? "1" : "0");
                                break;
                        }
                    }
                    buffer = buffer.Concat(stream.ToArray()).ToArray();
                    buffer[4] = (byte)(stream.Length >> 8);
                    buffer[5] = (byte)stream.Length;
                    var state = new SendState(Socket, Listener, Logger, buffer);
                    state.BeginSend();
                }                
            }
            Receive();
        }
        
        private void ProcessParams()
        {
            var request = Listener.GetRequest(Record.RequestId);
            if (request != null)
            {
                var @params = NameValuePairsSerializer.Parse(Buffer);
                var feature = request as IHttpRequestFeature;
                var headers = new Dictionary<string, string[]>();
                foreach(var kv in @params)
                {
                    var key = kv.Key;
                    Logger.WriteVerbose(string.Format("Receive params from server {0}={1}", key, kv.Value));
                    if (key.StartsWith("HTTP_"))
                    {
                        key = FormatHeaderName(key.Substring(5));
                        if (!headers.ContainsKey(key))
                            headers.Add(key, new string[] { kv.Value });
                        else
                        {
                            var value = headers[key];
                            headers[key] = value.Concat(new string[] { kv.Value }).ToArray();
                        }
                    }
                    else
                    {
                        switch (key)
                        {                            
                            case "REQUEST_URI":
                                feature.Path = kv.Value;
                                break;
                            case "QUERY_STRING":
                                feature.QueryString = kv.Value;
                                break;
                            case "SERVER_PROTOCOL":
                                feature.Protocol = kv.Value;
                                break;
                            case "CONTENT_LENGHT":
                                var contentLength = !string.IsNullOrEmpty(kv.Value) ? long.Parse(kv.Value) : 0;
                                feature.Body.SetLength(contentLength);
                                break;
                        }
                    }
                }

                feature.Protocol = @params.Any( p => "HTTPS" == p.Key) ? "https" : "http";

                feature.Headers = headers;
            }
            Receive();
        }

        private string FormatHeaderName(string name)
        {
            char[] formated = new char[name.Length];

            bool upperCase = true;

            for(int i = 0; i < name.Length; i++)
            {
                if (name[i] == '_')
                {
                    formated[i] = '-';
                    upperCase = true;
                }
                else
                {
                    formated[i] = (upperCase) ? name[i] : char.ToLower(name[i]);
                    upperCase = false;
                }
            }

            return new string(formated);
        }

        private void Receive()
        {
            if (Record.Padding > 0)
            {
                var paddingState = new PaddingState(Socket, Listener, Logger) { Record = Record };
                Socket.BeginReceive(paddingState.Buffer, 0, paddingState.Length, SocketFlags.None, EndReceive, paddingState);
                return;
            }

            var headerState = new HeaderState(Socket, Listener, Logger);
            Socket.BeginReceive(headerState.Buffer, 0, headerState.Length, SocketFlags.None, EndReceive, headerState);
        }

        private void ProcessStdInput()
        {
            var request = Listener.GetRequest(Record.RequestId);
            if (request != null)
            {
                var feature = request as IHttpRequestFeature;
                if (Record.Length > 0)
                {
                    ((RequestStream)feature.Body).Append(Buffer);
                }

                Listener.App.Invoke(request);

                if (!request.KeepAlive && Record.Length == 0)
                    return;
            }
            Receive();
        }

        private void ProcessAbort()
        {
            var request = Listener.GetRequest(Record.RequestId);
            if (request != null)
            {
                // TODO Implement
                Listener.RemoveRequest(Record.RequestId);
                if (!request.KeepAlive)
                    return;
            }

            Receive();
        }
        
    }

    class PaddingState : ReceiveState
    {
        public PaddingState(Socket socket, IListener listener, ILogger logger) : base(socket, listener, logger)
        { }

        public override int Length
        {
            get
            {
                return Record.Padding;
            }
        }
        public override void Process()
        {
            Logger.WriteVerbose(string.Format("PaddingState Process Record type {0} id {1} length {2} padding {3}", (RecordType)Record.Type, Record.RequestId, Record.Length, Record.Padding));
            var headerState = new HeaderState(Socket, Listener, Logger) { Record = Record };
            Socket.BeginReceive(headerState.Buffer, 0, headerState.Length, SocketFlags.None, EndReceive, headerState);
        }
    }
}