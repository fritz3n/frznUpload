using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Shared
{
    public class MessageHandler : IDisposable
    {
        Queue<Message> IncomingQueue = new Queue<Message>();
        SslStream stream;
        TcpClient tcp;
#pragma warning disable IDE0044 // Add readonly modifier
        byte[] headerBuffer = new byte[4];
#pragma warning restore IDE0044 // Add readonly modifier
        CancellationTokenSource tokenSource;
        ManualResetEvent mre = new ManualResetEvent(false);

        bool disposed = false;

        public bool Running { get; private set; } = false;
        ManualResetEvent ErrorEvent = new ManualResetEvent(true);
        public Exception ShutdownException { get; private set; }

        private bool graceful = false;
        

#if LOGMESSAGES
        List<(bool, Message)> Log = new List<(bool, Message)>();
#endif

        public MessageHandler(TcpClient cli, SslStream stream)
        {
            this.stream = stream;
            this.tcp = cli;
        }

        public void Start()
        {
            if (Running)
                return;

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
            try
            {
                Running = true;
                ErrorEvent.Reset();
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    int l;

                    try
                    {
                        l = await stream.ReadAsync(headerBuffer, 0, 4, token);
                    }
                    catch(Exception e)
                    {
                        graceful = !tcp.Connected;
                        ShutdownException = e;
                        Running = false;
                        ErrorEvent.Set();
                        return;
                    }

                    if (l != 4)
                        throw new Exception("Omae Wa Mou Shindeiru");

                    uint length = BitConverter.ToUInt32(headerBuffer, 0);
                    byte[] bytes = new byte[length];

                    int read = 0;
                    while (read != length)
                    {
                        read += await stream.ReadAsync(bytes, read, (int)length - read);
                    }
                    
                    var m = new Message(bytes);

                    IncomingQueue.Enqueue(m);
#if LOGMESSAGES
                    Log.Add((false, m));
#endif
                    mre.Set();
                }
            }catch(Exception e)
            {
                ShutdownException = e;
                Running = false;
                ErrorEvent.Set();

                Console.WriteLine(e);
            }
        }

        public async Task SendMessage(Message message, bool encrypt = true)
        {
#if LOGMESSAGES
            Log.Add((true, message));
#endif

            byte[] data = message.ToByte();

            byte[] length = BitConverter.GetBytes(data.Length);

            
            await stream.WriteAsync(length, 0, 4);
            await stream.WriteAsync(data, 0, data.Length);
            
        }

        public Message WaitForMessage()
        {
            lock (mre)
            {
                if (IncomingQueue.Count != 0)
                    return IncomingQueue.Dequeue();
                mre.Reset();
            
                WaitHandle.WaitAny(new WaitHandle[] { ErrorEvent, mre });

                if (!Running)
                {
                    if (graceful)
                        throw new GracefulShutdownException(ShutdownException);
                    else
                        throw ShutdownException;
                }

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

            var checkResult = MessagePatterns.CheckMessage(m);

            if (!checkResult.Item1)
                throw new MessageMatchException(m, checkResult.Item2);

            if (m.IsError & throwIfError)
                throw new ErrorMessageException(m);

            return m;
        }

        public Message WaitForMessage(Message.MessageType expected)
        {
            var m = WaitForMessage();

            if (m.Type != expected)
                throw new SequenceBreakException(expected, m.Type);

            return m;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Stop();
            tokenSource.Dispose();
            mre.Dispose();
            ErrorEvent.Dispose();
            IncomingQueue = null;

            disposed = true;
        }

        ~MessageHandler() => Dispose();
    }

    public class SequenceBreakException : Exception
    {
        public Message.MessageType ExpectedType {get; private set;}
        public Message.MessageType ReceivedType {get; private set;}
        public override string Message => ToString();

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

    public class ErrorMessageException : Exception
    {
        public Message BadMessage { get; private set; }

        public ErrorMessageException(Message m)
        {
            BadMessage = m;
        }

        public override string ToString()
        {
            return BadMessage + "\nis an error!";
        }
    }

    public class MessageMatchException : Exception
    {
        public Message BadMessage { get; private set; }
        public string Reason { get; private set; }

        public MessageMatchException(Message m, string reason)
        {
            BadMessage = m;
            Reason = reason;
        }

        public override string ToString()
        {
            return BadMessage + "\ndidnt match its saved pattern with reason: " + Reason;
        }
    }
    
    public class GracefulShutdownException : Exception
    {
        public GracefulShutdownException(Exception innerException)
       : base("A 'graceful' shutdown occured", innerException) { }
    }
}
