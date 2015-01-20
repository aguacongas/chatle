using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ChatLe.Hosting.FastCGI
{
    public class RequestStream : Stream
    {
        public override bool CanRead
        {
            get
            {
                return true;
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
                return false;
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

        long _position;
        public override long Position
        {
            get
            {
                return _position;
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

        ManualResetEvent _event = new ManualResetEvent(false);
        int _index;
        int _currentPossition;
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position == _length)
                return 0;

            if (_index == _buffers.Count)
            {
                _event.WaitOne(TimeSpan.FromMinutes(1.5));
                _event.Reset();
                if (_index == _buffers.Count)
                    return 0;
            }
                           
            var current = _buffers[_index];
            var maxLength = current.Length - _currentPossition;
            var length = maxLength >= count ? count : maxLength;
            Buffer.BlockCopy(current, _currentPossition, buffer, offset, length);
            if (length == maxLength)
            {
                _index++;
                _currentPossition = 0;
            }               
            _position += length;
            return length;
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
            throw new NotImplementedException();
        }
        List<byte[]> _buffers = new List<byte[]>();
        internal void Append(byte[] buffer)
        {
            _buffers.Add(buffer);
            _event.Set();
        }

        private bool disposedValue = false; // To detect redundant calls
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _event.Dispose();
                    base.Dispose(disposing);
                }
                disposedValue = true;
            }
        }

        internal void Completed()
        {
            _event.Set();
        }
    }
}