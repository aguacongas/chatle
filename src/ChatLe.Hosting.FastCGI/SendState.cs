using Microsoft.Framework.Logging;
using System;
using System.Net.Sockets;

namespace ChatLe.Hosting.FastCGI
{
    class SendState :State
    {
        public override int Length
        {
            get
            {
                return Buffer.Length;
            }
        }
        public byte[] Buffer
        {
            get;
            private set;
        }
        public SendState(Socket socket, IListener listener, ILogger logger, byte[] buffer) : base(socket, listener, logger)
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
        private void EndSend(IAsyncResult result)
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
                var written = client.EndSend(result, out error);
                if (error != SocketError.Success || written <= 0)
                {
                    OnDisconnect(client);
                    return;
                }
                if (written < Length)
                {
                    Offset = written;
                    BeginSend();
                }
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                Logger.WriteError("Exception on EndSend", e);
                OnDisconnect(client);
            }
        }
    }
}