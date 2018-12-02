using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Shared
{
    public class MessageHandler
    {
        Queue<Message> IncomingQueue = new Queue<Message>();
        SslStream stream;
        byte[] headerBuffer = new byte[4];
        CancellationTokenSource tokenSource;
        ManualResetEvent mre = new ManualResetEvent(false);

        public MessageHandler(SslStream stream)
        {
            this.stream = stream;
        }

        public void Start()
        {
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ReadFromStream(tokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void Stop()
        {
            tokenSource?.Cancel();
        }

        public async Task ReadFromStream(CancellationToken token)
        //public async void AsyncCallback(IAsyncResult ar)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                int l = await stream.ReadAsync(headerBuffer, 0, 4);

                if (l != 4)
                    throw new Exception("Omae Wa Mou Shindeiru");

                int length = BitConverter.ToInt32(headerBuffer, 0);
                byte[] bytes = new byte[length];
                await stream.ReadAsync(bytes, 0, length);

                IncomingQueue.Enqueue(new Message(bytes));
                mre.Set();
            }
        }

        public async Task SendMessage(Message message, bool encrypt = true)
        {
            byte[] data = message.ToByte();
            
            byte[] length = BitConverter.GetBytes(data.Length);

            await stream.WriteAsync(length, 0, 4);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public Message WaitForMessage()
        {
            if (IncomingQueue.Count != 0)
                return IncomingQueue.Dequeue();

            lock (mre)
            {
                mre.Reset();
                mre.WaitOne();
                return IncomingQueue.Dequeue();
            }
        }

        public Message WaitForMessage(bool throwIfError, Message.MessageType? expected = null)
        {
            Message m;

            if (expected != null)
                m = WaitForMessage((Message.MessageType)expected);
            else
                m = WaitForMessage();

            if (m.IsError & throwIfError)
                throw new Exception("Remote error was encountered");

            return m;
        }

        public Message WaitForMessage(Message.MessageType expected)
        {
            var m = WaitForMessage();

            if (m.Type != expected)
                throw new SequenceBreakException(expected, m.Type);

            return m;
        }
    }

    public class SequenceBreakException : Exception
    {
        public Message.MessageType ExpectedType {get; private set;}
        public Message.MessageType ReceivedType {get; private set;}

        public SequenceBreakException(Message.MessageType expectedType, Message.MessageType receivedType)
        {
            ExpectedType = expectedType;
            ReceivedType = receivedType;
        }

        public override string ToString()
        {
            return $"Expected {ExpectedType}, received {ReceivedType}";
        }
    }

}
