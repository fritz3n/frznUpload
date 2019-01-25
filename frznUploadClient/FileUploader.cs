using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Client
{
    class FileUploader : IDisposable
    {
        int ChunkSize = 16384;

        public int TotalSize { get; private set; }
        public int WrittenSize { get; private set; }
        public double Progress { get => (double)WrittenSize / TotalSize; }
        public bool Finished { get; private set; }
        public bool Running { get; private set; }
        public string Identifier { get; private set; }

        public bool Error { get; private set; } = false;
        public Exception Exception { get; private set; }

        public string FilePath { get; private set; }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private FileStream file;

        MessageHandler mes;

        public FileUploader(MessageHandler messageHandler, string path)
        {
            mes = messageHandler;
            FilePath = path;
        }

        public void Start()
        {
            if (Running)
                throw new Exception();
            Running = true;

            tokenSource = new CancellationTokenSource();

            new Task(UploadLoop, tokenSource.Token).Start();
        }

        private async void UploadLoop()
        {
            try
            {
                Open();
                FileInfo info = new FileInfo(FilePath);
                int size = (int)(info.Length > int.MaxValue ? throw new FileLoadException() : info.Length);
                string filename = Path.GetFileNameWithoutExtension(info.FullName);
                string extension = info.Extension.Replace(".", ""); ;

                ChunkSize = size / 100;

                TotalSize = size;

                mes.SendMessage(new Message(Message.MessageType.FileUploadRequest, false, filename, extension, size));

                var m = mes.WaitForMessage(true, Message.MessageType.FileUploadApproved);

                //mes.Stop();

                string identifier = m[0];
                byte[] buffer = new byte[ChunkSize];
                int written = 0;

                while ((written = await file.ReadAsync(buffer, 0, ChunkSize)) > 0)
                {
                    mes.SendMessage(new Message(Message.MessageType.FileUpload, false, written, buffer));
                    WrittenSize += written;

                    //await Task.Delay(10);
                }

                mes.SendMessage(new Message(Message.MessageType.FileUploadFinished, false));

                //mes.Start();

                mes.WaitForMessage(true, Message.MessageType.FileUploadSuccess);

                Identifier = identifier;
            }
            catch (SequenceBreakException e)
            {
                Close();
                mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                Console.WriteLine(e);
                throw new AggregateException(e);
            }
            catch (Exception e)
            {

                Close();
                mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));

                Exception = e;
                Error = true;
            }

            Close();

            Finished = true;
            Running = false;
        }

        public void Abort()
        {
            tokenSource.Cancel();
            Running = false;
        }

        private void Open()
        {
            file = File.Open(FilePath, FileMode.Open, FileAccess.Read);
        }

        private void Close()
        {
            file?.Close();
            file?.Dispose();
        }

        public void Dispose()
        {
            Close();
            tokenSource.Dispose();
        }

        ~FileUploader()
        {
            Dispose();
        }
    }
}
