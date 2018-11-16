using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Shared
{
    public class MessageHandler
    {
        Queue<Message> IncomingQueue = new Queue<Message>();
        NetworkStream stream;
        byte[] headerBuffer = new byte[4];
        EncryptionProvider encryption;
        CancellationTokenSource tokenSource;
        ManualResetEvent mre = new ManualResetEvent(false);

        public MessageHandler(NetworkStream stream, EncryptionProvider encryption)
        {
            this.stream = stream;
            this.encryption = encryption;
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
                await stream.ReadAsync(bytes, 0, 0);
                byte[] dec = encryption.DecryptLocal(bytes);

                IncomingQueue.Enqueue(new Message(dec));
            }
        }

        public async Task SendMessage(Message message, bool encrypt = true)
        {
            byte[] data = message.ToByte();

            if (encrypt)
                data = encryption.EncryptRemote(data);

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
    }
}
