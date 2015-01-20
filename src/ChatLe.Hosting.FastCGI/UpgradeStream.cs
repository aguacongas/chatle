using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ChatLe.Hosting.FastCGI
{
    class UpgradeStream : Stream
    {
        readonly Stream _requestStream;
        readonly Stream _responseStream;

        public UpgradeStream(Stream requestStream, Stream responseStream)
        {
            _requestStream = requestStream;
            _responseStream = responseStream;
        }

        public override void Flush()
        {
            _responseStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _responseStream.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _requestStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _requestStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _responseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _requestStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _requestStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _requestStream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return _requestStream.ReadByte();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _responseStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            _responseStream.WriteByte(value);
        }

        public override bool CanRead
        {
            get
            {
                return _requestStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _requestStream.CanSeek;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return _responseStream.CanTimeout || _requestStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _responseStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _requestStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _requestStream.Position;
            }
            set
            {
                _requestStream.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return _requestStream.ReadTimeout;
            }
            set
            {
                _requestStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return _responseStream.WriteTimeout;
            }
            set
            {
                _responseStream.WriteTimeout = value;
            }
        }
    }
}