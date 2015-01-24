using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        public override long Length
        {
            get
            {
                throw new NotImplementedException();
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

        EventWaitHandle _event = new AutoResetEvent(false);
        int _index;
        int _currentPossition;
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_index == _buffers.Count)
            {
                Debug.WriteLine("\r\nRequestStream: Waiting for new data\r\n");
                _event.Reset();
                _event.WaitOne();
                if (_index == _buffers.Count)
                {
                    Debug.WriteLine("\r\nRequestStream: End of request stream\r\n");
                    return 0;
                }
            }
                           
            var current = _buffers[_index];
            var maxLength = current.Length - _currentPossition;
            var length = maxLength >= count ? count : maxLength;
            Buffer.BlockCopy(current, _currentPossition, buffer, offset, length);

            Debug.WriteLine("\r\nRequestStream: Read: " + Encoding.UTF8.GetString(buffer, offset, count) +"\r\n");

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
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        List<byte[]> _buffers = new List<byte[]>();
        internal void Append(byte[] buffer)
        {
            _buffers.Add(buffer);
            Debug.WriteLine("\r\nRequestStream: Append buffer: " + Encoding.UTF8.GetString(buffer) + "\r\n");
            _event.Set();
        }

        private bool disposedValue = false; // To detect redundant calls
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("\r\nRequestStream: Dispose disposing: {0} disposedValue: {1}", disposing, disposedValue);
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
    }
}