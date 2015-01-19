using ChatLe.Hosting.FastCGI.Payloads;
using Microsoft.AspNet.HttpFeature;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Hosting.FastCGI
{
    class ResponseStream : Stream
    {
        readonly Context _context;
        WriteState _writeState;
        public ResponseStream(Context context)
        {
            _context = context;
            _writeState = new StartState(_context);
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<int>();
            _writeState.ProcessStart();            
            _writeState = _writeState.Process(tcs, buffer, offset, count);
            return tcs.Task;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            var tcs = new TaskCompletionSource<int>();
            _writeState.ProcessStart();
            _writeState.Buffers.Add(new ArraySegment<byte>());
            _writeState.BeginSend();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        abstract class WriteState : State
        {
            public Context Context { get; private set; }
            public WriteState(Context context) :base(context.State)
            {
                Context = context;
            }

            public List<ArraySegment<byte>> Buffers { get;  } = new List<ArraySegment<byte>>();

            public TaskCompletionSource<int> TaskCompletionSource { get; protected set; }

            public override int Length
            {
                get
                {
                    return Buffers[_currentBufferId].Count;
                }
            }
            abstract public WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count);

            abstract public void ProcessStart();


            protected byte[] CreateStdOutHeader(ushort length)
            {
                var header = new byte[] { Context.Version, (byte)RecordType.StdOutput, (byte)(Context.Id >> 8), (byte)Context.Id, (byte)(length >> 8), (byte)length, 0, 0 };
                return header;
            }

            int _currentBufferId;
            bool _end;
            public void BeginSend()
            {
                try
                {
                    var buffer = Buffers[_currentBufferId];
                    if (buffer.Array == null)
                    {
                        // send end request
                        buffer = new ArraySegment<byte>(new byte[] { Context.Version, (byte)RecordType.EndRequest, (byte)(Context.Id >> 8), (byte)Context.Id, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
                        Buffers[_currentBufferId] = buffer;
                        _end = true;
                    }

                    Socket.BeginSend(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, EndSend, this);
                }
                catch (ObjectDisposedException ode)
                {
                    TaskCompletionSource.SetException(ode);
                }
                catch (Exception e)
                {
                    TaskCompletionSource.SetException(e);
                    Logger.WriteError("UnHandled exception on BeginSend", e);
                    State.OnDisconnect(Socket);
                }
            }

            static void EndSend(IAsyncResult result)
            {
                var state = result.AsyncState as WriteState;
                try
                {
                    SocketError error;
                    var written = state.Socket.EndSend(result, out error);
                    if (error != SocketError.Success || written <= 0)
                    {
                        OnDisconnect(state.Socket);
                        return;
                    }

                    var buffer = state.Buffers[state._currentBufferId];

                    if (state.Offset + written < state.Length)
                    {
                        state.Offset += written;
                        state.BeginSend();
                    }
                    else
                        state.Sent(state._currentBufferId);

                }
                catch (ObjectDisposedException ode)
                {
                    state.TaskCompletionSource?.SetException(ode);
                }
                catch (Exception e)
                {
                    state.TaskCompletionSource?.SetException(e);
                    state.Logger.WriteError("UnHandled exception on EndSend", e);
                    State.OnDisconnect(state.Socket);
                }
            }

            protected virtual void Sent(int currentBufferId)
            {
                if (_end && !Context.KeepAlive)
                {
                    OnDisconnect(Socket);
                }
                if (++_currentBufferId < Buffers.Count)
                    BeginSend();
                else
                    TaskCompletionSource?.SetResult(0);
            }

            protected IEnumerable<ArraySegment<byte>> GetSegments(byte[] buffer, int offset, int count)
            {
                var len = count;
                var index = offset;                
                do
                {
                    var max = ushort.MaxValue < len ? ushort.MaxValue : len;
                    yield return new ArraySegment<byte>(CreateStdOutHeader((ushort)max));
                    yield return new ArraySegment<byte>(buffer, index, max);
                    len -= max;
                    index += max;
                } while (ushort.MaxValue < len);
            }
        }

        class StartState :WriteState
        {
            int _endHeaderId;
            public StartState(Context _context) :base(_context)
            { }

            public override WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count)
            {
                Buffers.AddRange(GetSegments(buffer, offset, count));
                BeginSend();
                return new BobyState(Context);
            }

            public override void ProcessStart()
            {
                var header = Encoding.ASCII.GetBytes(CreateResponseHeader());
                Buffers.AddRange(GetSegments(header, 0, header.Length));
                _endHeaderId = Buffers.Count - 1;
            }

            protected override void Sent(int _currentBufferId)
            {
                if (_currentBufferId == _endHeaderId)
                {
                    Context.HeaderSent();
                }
                base.Sent(_currentBufferId);
            }

            private string CreateResponseHeader()
            {
                var httpVersion = ((IHttpRequestFeature)Context).Protocol;
                var feature = Context as IHttpResponseFeature;
                var builder = new StringBuilder(httpVersion);
                builder.Append(' ');
                builder.Append(feature.StatusCode);
                builder.Append(' ');
                builder.Append(feature.ReasonPhrase);
                builder.Append('\r');
                builder.Append('\n');

                var hasConnection = false;
                var hasTransferEncoding = false;
                var hasContentLength = false;
                var keepAlive = Context.KeepAlive;
                var headers = feature.Headers;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        var isConnection = false;
                        if (!hasConnection &&
                            string.Equals(header.Key, "Connection", StringComparison.OrdinalIgnoreCase))
                        {
                            hasConnection = isConnection = true;
                        }
                        else if (!hasTransferEncoding &&
                            string.Equals(header.Key, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                        {
                            hasTransferEncoding = true;
                        }
                        else if (!hasContentLength &&
                            string.Equals(header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase))
                        {
                            hasContentLength = true;
                        }

                        foreach (var value in header.Value)
                        {
                            builder.Append(header.Key);
                            builder.Append(':');
                            builder.Append(' ');
                            builder.Append(value);
                            builder.Append('\r');
                            builder.Append('\n');

                            if (isConnection && value.IndexOf("close", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                keepAlive = false;
                            }
                        }
                    }
                }

                if (hasTransferEncoding == false && hasContentLength == false)
                {
                    keepAlive = false;
                }
                if (keepAlive == false && hasConnection == false && httpVersion == "HTTP/1.1")
                {
                    builder.Append("Connection: close\r\n\r\n");
                }
                else if (keepAlive && hasConnection == false && httpVersion == "HTTP/1.0")
                {
                    builder.Append("Connection: keep-alive\r\n\r\n");
                }
                else
                {
                    builder.Append('\r');
                    builder.Append('\n');
                }

                return builder.ToString();
            }
        }

        class BobyState :WriteState
        {
            public BobyState(Context _context) :base(_context)
            { }

            public override WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count)
            {
                Buffers.AddRange(GetSegments(buffer, offset, count));
                BeginSend();
                return new BobyState(Context);
            }

            public override void ProcessStart()
            {
            }
        }
       
    }
}