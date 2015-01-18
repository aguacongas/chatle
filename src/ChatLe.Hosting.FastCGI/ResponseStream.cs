using Microsoft.AspNet.HttpFeature;
using System;
using System.IO;
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

        long _length;
        public override long Length
        {
            get
            {
                return _length;
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
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<int>();
            _writeState = _writeState.Process(tcs, buffer, offset, count);
            return tcs.Task;
        }

        abstract class WriteState
        {
            public Context Context { get; private set; }
            public WriteState(Context context)
            {
                Context = context;
            }

            abstract public WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count);
        }

        class StartState :WriteState
        {
            public StartState(Context _context) :base(_context)
            { }

            public override WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count)
            {
                var header = CreateResponseHeader();

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

                Context.KeepAlive = keepAlive;

                return builder.ToString();
            }
        }

        class BobyState :WriteState
        {
            public BobyState(Context _context) :base(_context)
            { }

            public override WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }

        class EndState :WriteState
        {
            public EndState(Context _context) :base(_context)
            { }
            public override WriteState Process(TaskCompletionSource<int> tcs, byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }
    }
}