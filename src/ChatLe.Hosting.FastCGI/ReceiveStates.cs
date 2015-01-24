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
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ChatLe.Hosting.FastCGI
{
    public abstract class State
    {
        public const int HEADER_LENGTH = 8;
        public virtual int Length { get { return HEADER_LENGTH; } }
        public ILogger Logger { get; private set; }
        public int Offset { get; set; }
        public Socket Socket { get; private set; }
        public IListener Listener { get; private set; }

        public IDictionary<ushort, Context> Contexts { get; protected set; }

        protected State(Socket socket, IListener listener, ILogger logger)
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

        public State(State state)
        {
            Socket = state.Socket;
            Listener = state.Listener;
            Logger = state.Logger;
            Contexts = state.Contexts;
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

        public Context GetRequest(ushort id)
        {
            if (Contexts.ContainsKey(id))
                return Contexts[id];

            return null;
        }

        public void SetRequest(Context request)
        {
            Contexts[request.Id] = request;
        }

        public void RemoveRequest(ushort id)
        {
            Contexts.Remove(id);
        }

    }

    abstract class ReceiveState :State
    {
        public Record Record { get; protected set; }
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
        protected ReceiveState(Socket socket, IListener listener, ILogger logger) :base(socket, listener, logger)
        { }

        protected ReceiveState(State state):base(state)
        { }
        protected int GetMaxToReadSize()
        {
            var toRead = Length - Offset;
            return toRead > Socket.ReceiveBufferSize ? Socket.ReceiveBufferSize : toRead;
        }

        public abstract void Process();

        public void BeginRead()
        {
            try
            {
                Socket.BeginReceive(Buffer, 0, Length, SocketFlags.None, EndReceive, this);
            }
            catch (ObjectDisposedException)
            { }
            catch (SocketException)
            { }
            catch (Exception e)
            {
                Logger.WriteError("Error on begin read", e);
                OnDisconnect(Socket);
            }
        }

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
                    state.Process();
                    return;
                }

                if (state.Offset + read < state.Length)
                {
                    state.Offset += read;
                    state.BeginRead();                    
                }                    
                else
                    state.Process();
            }
            catch(ObjectDisposedException)
            { }
            catch (SocketException)
            { }
            catch (Exception e)
            {
                state.Logger.WriteError("Unhanlded exception on EndReceive", e);
                OnDisconnect(client);
            }
        }

       
    }

    class HeaderState : ReceiveState
    {
        public HeaderState(Socket socket, IListener listener, ILogger logger) :base(socket, listener, logger)
        {
            Contexts = new ConcurrentDictionary<ushort, Context>();
            Record = new Record();
        }

        public HeaderState(State state) :base(state)
        {
            Record = new Record();
        }

        public override void Process()
        {
            Record.Version = Buffer[0];
            Record.Type = Buffer[1];
            Record.RequestId = (ushort)((Buffer[2] << 8) + Buffer[3]);
            Record.Length = (ushort)((Buffer[4] << 8) + Buffer[5]);
            Record.Padding = Buffer[6];

            var bodyState = new BodyState(this);
            if (Record.Length > 0)
                bodyState.BeginRead();
            else
                bodyState.Process();
        }
    }

    class BodyState : ReceiveState
    {
        public BodyState(ReceiveState state) : base(state)
        {
            Record = state.Record;
        }

        public override int Length
        {
            get
            {
                return Record.Length;
            }
        }

        public override void Process()
        {
            if (Record.Type != 0
                && Record.Type != 1
                && Record.Type != 2
                && Record.Type != 4
                && Record.Type != 5
                && Record.Type != 9)
            {
                var buffer = new byte[] { Record.Version, (byte)RecordType.Unknown, (byte)(Record.RequestId >> 8), (byte)Record.RequestId, 0, 2, 0, 0, Record.Type, 0 };
                var state = new SendState(this, buffer);
                state.BeginSend();
            }

            switch ((RecordType)Record.Type)
            {
                case RecordType.BeginRequest:
                    SetRequest(new Context(Record.Version, Record.RequestId, (Buffer[2] & 1) == 1, this));
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

            Receive();
        }

        private void ProcessGetValues()
        {
            var request = GetRequest(Record.RequestId);
            if (request == null)
                return;

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
                var state = new SendState(this, buffer);
                state.BeginSend();
            }
        }
        
        private void ProcessParams()
        {
            var request = GetRequest(Record.RequestId);
            if (request == null || Record.Length == 0)
                return;

            var @params = NameValuePairsSerializer.Parse(Buffer);
            var feature = request as IHttpRequestFeature;
            var headers = feature.Headers;
            foreach(var kv in @params)
            {
                var key = kv.Key;

                if (key.StartsWith("HTTP_"))
                {
                    SetRequestHeader(headers, key.Substring(5), kv.Value);
                }
                else
                {
                    switch (key)
                    {
                        case "REQUEST_METHOD":
                            feature.Method = kv.Value;
                            break;
                        case "SCRIPT_NAME":
                            feature.Path = kv.Value;
                            break;
                        case "QUERY_STRING":
                            feature.QueryString = kv.Value;
                            break;
                        case "SERVER_PROTOCOL":
                            feature.Protocol = kv.Value;
                            break;
                        case "HTTPS":
                            feature.Protocol = "https";
                            break;
                        case "CONTENT_LENGTH":
                            if (!string.IsNullOrEmpty(kv.Value))
                                request._requestStream.SetLength(int.Parse(kv.Value));
                            break;
                    }
                }
            }

            Debug.WriteLine(string.Format("{0} {1} {2}\r\n{3}\r\n", feature.Method, feature.Path, feature.Protocol, string.Join("\r\n", headers.Select(h => h.Key + "=" + string.Join(",", h.Value)))));
        }

        static private string SetRequestHeader(IDictionary<string, string[]> headers, string key, string value)
        {
            key = FormatHeaderName(key);
            if (!headers.ContainsKey(key))
                headers.Add(key, value.Split(','));
            else
            {
                var v = headers[key];
                headers[key] = v.Concat(value.Split(',')).ToArray();
            }

            return key;
        }

        static private string FormatHeaderName(string name)
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
                var paddingState = new PaddingState(this);
                paddingState.BeginRead();
                return;
            }

            var headerState = new HeaderState(this);
            headerState.BeginRead();
        }

        private void ProcessStdInput()
        {
            var context = GetRequest(Record.RequestId);
            if (context == null)
                return;

            var feature = context as IHttpRequestFeature;

            Debug.WriteLine(Encoding.UTF8.GetString(Buffer));
            if (Record.Length > 0)
                context._requestStream.Append(Buffer);
            else
                context._requestStream.Completed();

            if (!context.Called)
            {
                context.Called = true;
                Listener.App.Invoke(context).ContinueWith(t =>
                    {
                        if (t.Exception != null)
                            Logger.WriteError("Error on execute", t.Exception);

                        context.Dispose();
                        RemoveRequest(context.Id);
                    }, context.CancellationTokenSource.Token);
            }
        }

        private void ProcessAbort()
        {
            var context = GetRequest(Record.RequestId);
            if (context != null)
            {
                context.CancellationTokenSource.Cancel();
                RemoveRequest(Record.RequestId);
                context.Dispose();
            }
        }
        
    }

    class PaddingState : ReceiveState
    {
        public PaddingState(ReceiveState state) : base(state)
        {
            Record = state.Record;
        }

        public override int Length
        {
            get
            {
                return Record.Padding;
            }
        }
        public override void Process()
        {
            var headerState = new HeaderState(this);
            headerState.BeginRead();
        }
    }

    class SendState : State
    {
        public override int Length
        {
            get
            {
                return Buffer.Length;
            }
        }
        public virtual byte[] Buffer
        {
            get;
            private set;
        }
        public SendState(State state, byte[] buffer) : base(state.Socket, state.Listener, state.Logger)
        {
            Buffer = buffer;
        }

        protected int GetMaxToWriteSize()
        {
            var toSend = Length - Offset;
            return toSend > Socket.SendBufferSize ? Socket.SendBufferSize : toSend;
        }
        public void BeginSend()
        {
            try
            {
                Socket.BeginSend(Buffer, Offset, GetMaxToWriteSize(), SocketFlags.None, EndSend, this);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                Logger.WriteError("UnHandled exception on BeginSend", e);
                OnDisconnect(Socket);
            }

        }
        private static void EndSend(IAsyncResult result)
        {
            var state = result.AsyncState as SendState;
            if (state == null)
                return;

            Socket client = state.Socket;
            if (client == null)
                return;

            try
            {
                SocketError error;
                var written = client.EndSend(result, out error);
                if (error != SocketError.Success || written <= 0)
                {
                    OnDisconnect(client);
                    return;
                }
                if (state.Offset + written < state.Length)
                {
                    state.Offset += written;
                    state.BeginSend();
                }
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                state.Logger.WriteError("Exception on EndSend", e);
                OnDisconnect(client);
            }
        }
    }
}