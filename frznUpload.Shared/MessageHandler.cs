using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace frznUpload.Shared
{
    public class MessageHandler : IDisposable
    {
        Stopwatch testWatch = new Stopwatch();

        public static int Version { get; } = HashEnum(typeof(Message.MessageType));

        public delegate void DisconnectHandler(object sender, DisconnectReason disconnectReason);
        public event DisconnectHandler OnDisconnect;

        public enum DisconnectReason
        {
            Stopped,
            Graceful,
            Timeout,
            Error,
        }

        public bool Synchronous { get; set; }
        private bool Verbose = false;
        IMessageLogger VerboseLogger;
        Queue<Message> IncomingQueue = new Queue<Message>();
        Queue<Message> OutgoingQueue = new Queue<Message>();
        SslStream stream;
        TcpClient tcp;
#pragma warning disable IDE0044 // Add readonly modifier
        byte[] headerBuffer = new byte[4];
#pragma warning restore IDE0044 // Add readonly modifier
        CancellationTokenSource tokenSource;
        ManualResetEvent readEvent = new ManualResetEvent(false);
        ManualResetEvent writeEvent = new ManualResetEvent(false);

        bool disposed = false;

        public bool Running { get; private set; } = false;
        ManualResetEvent ErrorEvent = new ManualResetEvent(true);
        public Exception ShutdownException { get; private set; }
        
        private bool graceful = false;

        PingPongHandler PingPong;

#if LOGMESSAGES
        List<(bool, Message)> Log = new List<(bool, Message)>();
#endif

        public MessageHandler(TcpClient cli, SslStream stream, bool synchronous = false,  bool verbose = false, IMessageLogger verboseLogger = null)
        {
            Synchronous = synchronous;
            this.stream = stream;
            tcp = cli;
            PingPong = new PingPongHandler(this);
            PingPong.Timeout += TimeoutHandler;

            Verbose = verbose;
            if (Verbose)
            {
                if (verboseLogger == null)
                    verboseLogger = new ConsoleLogger();
                VerboseLogger = verboseLogger;
            }
        }

        private void TimeoutHandler(object sender, EventArgs e)
        {
            Stop();
            ShutdownException = new TimeoutException();
        }

        public void Start()
        {
            if (Running)
                return;

            PingPong.Start();

            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();
            
            new Thread(() => ReadFromStream(tokenSource.Token)).Start();
            new Thread(() => WriteToStream(tokenSource.Token)).Start();
            Running = true;
        }

        public void Stop(DisconnectReason reason = DisconnectReason.Stopped)
        {
            if (!Running)
                return;
            
            PingPong.Stop();
            tokenSource?.Cancel();
            Running = false;
            OnDisconnect?.Invoke(this, reason);
        }

        public async Task WriteToStream(CancellationToken token)
        {
            try
            {
                ErrorEvent.Reset();
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    writeEvent.WaitOne();

                    while (OutgoingQueue.Count > 0) {
                        token.ThrowIfCancellationRequested();

                        Message m = OutgoingQueue.Dequeue();

                        testWatch.Stop();

                        if (Verbose)
                            VerboseLogger.LogMessage(true, m);

                        byte[] data = m.ToByte();

                        byte[] length = BitConverter.GetBytes(data.Length);

                        try { 
                            if (Synchronous)
                            {
                                stream.Write(length, 0, 4);
                                stream.Write(data, 0, data.Length);
                            }
                            else
                            {
                                await stream.WriteAsync(length, 0, 4, token);
                                await stream.WriteAsync(data, 0, data.Length, token);
                            }
                        }
                        catch (Exception e)
                        {
                            graceful = !tcp.Connected;
                            ShutdownException = e;
                            Stop(graceful ? DisconnectReason.Graceful : DisconnectReason.Error);
                            ErrorEvent.Set();
                            return;
                        }
                    }

                    writeEvent.Reset();
                }
            }
            catch (Exception e)
            {
                ShutdownException = e;
                Stop(DisconnectReason.Error);

                ErrorEvent.Set();


                Console.WriteLine(e);
            }
        }

        public async Task ReadFromStream(CancellationToken token)
        //public async void AsyncCallback(IAsyncResult ar)
        {
            try
            {
                ErrorEvent.Reset();
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    int l = 0;

                    try
                    {
                        while (l < 4)
                        {
                            if (Synchronous)
                            {
                                l += stream.Read(headerBuffer, l, 4 - l);
                            }
                            else
                            {
                                l += await stream.ReadAsync(headerBuffer, l, 4 - l, token);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        graceful = !tcp.Connected;
                        ShutdownException = e;
                        Stop(graceful ? DisconnectReason.Graceful : DisconnectReason.Error);
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
                        if (Synchronous)
                        {
                            read += stream.Read(bytes, read, (int)length - read);
                        }
                        else
                        {
                            read += await stream.ReadAsync(bytes, read, (int)length - read);
                        }
                    }
                    
                    var m = new Message(bytes);

                    if (Verbose)
                        VerboseLogger.LogMessage(false, m);

                    if (!PingPong.HandleMessage(m))
                    {
                        IncomingQueue.Enqueue(m);
                        readEvent.Set();
                    }
                    
#if LOGMESSAGES
                    Log.Add((false, m));
#endif
                }
            }catch(Exception e)
            {
                ShutdownException = e;
                Stop(DisconnectReason.Error);

                ErrorEvent.Set();


                Console.WriteLine(e);
            }
        }
        
        public void SendMessage(Message message)
        {
#if LOGMESSAGES
            Log.Add((true, message));
#endif
            testWatch.Restart();
            OutgoingQueue.Enqueue(message);
            writeEvent.Set();
        }

        public Message WaitForMessage()
        {
            lock (readEvent)
            {
                if (IncomingQueue.Count != 0)
                    return IncomingQueue.Dequeue();
                readEvent.Reset();
            
                WaitHandle.WaitAny(new WaitHandle[] { ErrorEvent, readEvent });

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

        static private int HashEnum(Type T)
        {
            //TODO: Fix enum hashing
            return 187;
            unchecked {
                int h = 37;
                h *= 392;

                h ^= Enum.GetNames(T).GetHashCode();
                h *= 392;

                h ^= Enum.GetValues(T).GetHashCode();

                return h;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Stop();
            tokenSource.Dispose();
            readEvent.Dispose();
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
