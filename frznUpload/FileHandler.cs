using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Server
{
    class FileHandler
    {
        const string directory = "../files/";
        const int chunksSize = 16384;

        static public (bool, string) ReceiveFile(Message message, MessageHandler mes, DataBase db, Logger log)
        {
            int size = message[2];
            int written = 0;

            string filename = message[0];
            string extension = message[1];

            string identifier = db.GetAvailableFileIdentifier();

            mes.SendMessage(new Message(Message.MessageType.FileUploadApproved, false, identifier));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string localFilename = directory + identifier + ".file";

            FileStream file = File.Open(localFilename, FileMode.Create, FileAccess.Write);

            bool error = false;

            Message m;
            while (true)
            {
                try
                {
                    m = mes.WaitForMessage(true);
                }
                catch
                {
                    file.Close();
                    file.Dispose();

                    File.Delete(localFilename);

                    throw;
                }

                if (m.Type == Message.MessageType.FileUploadFinished)
                {
                    if (written != size)
                    {
                        mes.SendMessage(new Message(Message.MessageType.FileUpload, true, $"Expected {size} bytes, got {written}"));
                        error = true;
                        break;
                    }

                    mes.SendMessage(new Message(Message.MessageType.FileUploadSuccess, false));
                    break;
                }
                else if (m.Type != Message.MessageType.FileUpload)
                {
                    mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                    error = true;
                    break;
                }

                if (written >= size)
                {
                    mes.SendMessage(new Message(Message.MessageType.FileUpload, true, $"Expected {size} bytes, got {written}"));
                    error = true;
                    break;
                }

                try
                {
                    file.Write(m[1], 0, m[0]);
                    written += m[0];
                }
                catch (Exception e)
                {
                    mes.SendMessage(new Message(Message.MessageType.FileUpload, true, e.ToString()));
                    error = true;
                    break;
                }
            }

            file.Close();
            file.Dispose();

            if (error)
            {
                File.Delete(localFilename);
            }
            else
            {
                db.CreateFile(identifier, filename, extension, size);
            }

            return (!error, identifier);
        }
    }
}
