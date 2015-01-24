using ChatLe.Hosting.FastCGI.Payloads;
using Microsoft.AspNet.HttpFeature;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine("\r\nResponseStream: WriteAsync " + Encoding.UTF8.GetString(buffer, offset, count) + "\r\n");
            _writeState.ProcessStart();            
            _writeState = _writeState.Process(tcs, buffer, offset, count);            
            return tcs.Task;
        }

        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("\r\nResponseStream: Dispose disposing: {0} disposedValue: {1}", disposing, disposedValue);
            if (!disposedValue)
            {
                if (disposing)
                {
                    _writeState.End = true;
                    _writeState.ProcessStart();
                    _writeState.Buffers.Add(new ArraySegment<byte>(new byte[] { _context.Version, (byte)RecordType.EndRequest, (byte)(_context.Id >> 8), (byte)_context.Id, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
                    _writeState.BeginSend();
                    base.Dispose(disposing);
                }
                disposedValue = true;
            }
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
            public bool End { get; set; }
            public void BeginSend()
            {
                try
                {
                    var buffer = Buffers[_currentBufferId];
                    Socket.BeginSend(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, EndSend, this);
                }
                catch (ObjectDisposedException ode)
                {
                    TaskCompletionSource?.SetException(ode);
                }
                catch (Exception e)
                {
                    TaskCompletionSource?.SetException(e);
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
                        return;

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
                    state.Logger.WriteError("EndSend ObjectDisposedException");
                    state.TaskCompletionSource?.SetException(ode);
                }
                catch (Exception e)
                {
                    state.TaskCompletionSource?.SetException(e);
                    state.Logger.WriteError("EndSend UnHandled exception on EndSend", e);
                    OnDisconnect(state.Socket);
                }
            }

            protected virtual void Sent(int bufferId)
            {
                if (++_currentBufferId < Buffers.Count)
                    BeginSend();
                else
                {
                    if (End && !Context.KeepAlive)
                    {
                        Logger.WriteInformation("Close socket");
                        OnDisconnect(Socket);
                    }

                    TaskCompletionSource?.SetResult(0);
                }                    
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
                this.TaskCompletionSource = tcs;
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

            protected override void Sent(int bufferId)
            {
                if (bufferId == _endHeaderId)
                {
                    Context.HeaderSent();
                }
                base.Sent(bufferId);
            }

            private string CreateResponseHeader()
            {
                var httpVersion = ((IHttpRequestFeature)Context).Protocol;
                var feature = Context as IHttpResponseFeature;
                var builder = new StringBuilder(httpVersion);
                builder.Append(' ');
                builder.Append(feature.StatusCode);
                builder.Append(' ');
                builder.Append(string.IsNullOrEmpty(feature.ReasonPhrase) ? GetReasonPhraseFromCode(feature.StatusCode) : feature.ReasonPhrase);
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

                var result = builder.ToString();

                Debug.WriteLine("\r\n" + result + "\r\n");

                return result;
            }

            private string GetReasonPhraseFromCode(int statusCode)
            {
                switch (statusCode)
                {
                    case 100:
                        return "Continue";
                    case 101:
                        return "Switching Protocols";
                    case 102:
                        return "Processing";
                    case 200:
                        return "OK";
                    case 201:
                        return "Created";
                    case 202:
                        return "Accepted";
                    case 203:
                        return "Non-Authoritative Information";
                    case 204:
                        return "No Content";
                    case 205:
                        return "Reset Content";
                    case 206:
                        return "Partial Content";
                    case 207:
                        return "Multi-Status";
                    case 226:
                        return "IM Used";
                    case 300:
                        return "Multiple Choices";
                    case 301:
                        return "Moved Permanently";
                    case 302:
                        return "Found";
                    case 303:
                        return "See Other";
                    case 304:
                        return "Not Modified";
                    case 305:
                        return "Use Proxy";
                    case 306:
                        return "Reserved";
                    case 307:
                        return "Temporary Redirect";
                    case 400:
                        return "Bad Request";
                    case 401:
                        return "Unauthorized";
                    case 402:
                        return "Payment Required";
                    case 403:
                        return "Forbidden";
                    case 404:
                        return "Not Found";
                    case 405:
                        return "Method Not Allowed";
                    case 406:
                        return "Not Acceptable";
                    case 407:
                        return "Proxy Authentication Required";
                    case 408:
                        return "Request Timeout";
                    case 409:
                        return "Conflict";
                    case 410:
                        return "Gone";
                    case 411:
                        return "Length Required";
                    case 412:
                        return "Precondition Failed";
                    case 413:
                        return "Request Entity Too Large";
                    case 414:
                        return "Request-URI Too Long";
                    case 415:
                        return "Unsupported Media Type";
                    case 416:
                        return "Requested Range Not Satisfiable";
                    case 417:
                        return "Expectation Failed";
                    case 418:
                        return "I'm a Teapot";
                    case 422:
                        return "Unprocessable Entity";
                    case 423:
                        return "Locked";
                    case 424:
                        return "Failed Dependency";
                    case 426:
                        return "Upgrade Required";
                    case 500:
                        return "Internal Server Error";
                    case 501:
                        return "Not Implemented";
                    case 502:
                        return "Bad Gateway";
                    case 503:
                        return "Service Unavailable";
                    case 504:
                        return "Gateway Timeout";
                    case 505:
                        return "HTTP Version Not Supported";
                    case 506:
                        return "Variant Also Negotiates";
                    case 507:
                        return "Insufficient Storage";
                    case 510:
                        return "Not Extended";
                    default:
                        return null;
                }
            }
        }

        class BobyState :WriteState
        {
            public BobyState(Context _context) :base(_context)
            { }

            public override WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count)
            {
                this.TaskCompletionSource = tcs;
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