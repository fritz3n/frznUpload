using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    static class FileUploadHandler
    {
        static Queue<UploadContract> UploadQueue = new Queue<UploadContract>();
        static MainForm Form;
        static ClientManager Client;

        public static void Init(MainForm form, ClientManager client)
        {
            Form = form;
            Client = client;

            Form.UploadFinished += Form_UploadFinished;
        }

        private static void Form_UploadFinished(object sender, EventArgs e) => StartUpload();

        public static void UploadFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();

            UploadContract c = new UploadContract
            {
                Uploader = null,
                Path = filename,
                Filename = Path.GetFileName(filename),

                Share = true,
                FirstView = false,
                Public = true,
                PublicRegistered = true,
                Whitelisted = false,
                Whitelist = ""
            };

            Enqueue(c);
        }

        public static void UploadDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                throw new FileNotFoundException();

            foreach (string filename in Directory.EnumerateFiles(directory))
            {
                UploadFile(filename);
            }

            foreach (string directoryName in Directory.EnumerateDirectories(directory))
            {
                UploadDirectory(directoryName);
            }
        }

        public static void Enqueue(List<UploadContract> contracts)
        {
            foreach (UploadContract c in contracts)
            {
                UploadQueue.Enqueue(c);
            }
            StartUpload();
        }

        public static void Enqueue(UploadContract contract)
        {
            UploadQueue.Enqueue(contract);
            StartUpload();
        }

        private static async void StartUpload()
        {
            if (Form.Uploading)
                return;
            if (UploadQueue.Count == 0)
                return;

            UploadContract contract = UploadQueue.Dequeue();
            try
            {
                contract.Uploader = await Client.UploadFile(contract.Path, contract.Filename);
            }
            catch (RetryException e)
            {
                if (e.LastException is IOException)
                {
                    MessageBox.Show($"Couldn´t upload file \"{contract.Filename}\":\n{e.Message}");
                    StartUpload();
                    return;
                }
                throw;
            }

            Form.StartUpload(contract);
        }

    }
}
