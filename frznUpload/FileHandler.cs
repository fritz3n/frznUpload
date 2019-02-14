using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Server
{
    static class FileHandler
    {
        const string directory = "../files/";
        const string backupDirectory = directory + "/deleted/";
        const int chunksSize = 16384;

        static FileHandler()
        {
            //Check if dirs exsist, if not create them
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);
        }

        public static async Task<(bool, string)> ReceiveFile(Message message, MessageHandler mes, DataBase db, Logger log)
        {
            int size = message[2];
            int written = 0;

            string filename = message[0];
            string extension = message[1];

            string identifier = db.GetAvailableFileIdentifier();

            mes.SendMessage(new Message(Message.MessageType.FileUploadApproved, false, identifier));

            

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
                    await file.WriteAsync(m[1], 0, m[0]);
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

        /// <summary>
        /// Deletes a File from the fs
        /// </summary>
        /// <param name="file_name">the file to be deleted</param>
        /// <param name="fullname">The name of the file, as given on upload</param>
        public static void DeleteFile(string file_name, string fullname)
        {
            string localFileName = directory + file_name + ".file";
            //copy the file, so we have a backup
            File.Copy(localFileName, backupDirectory + fullname + "==" + file_name + ".file");
            //Delete the file from the fs
            File.Delete(localFileName);
        }
    }
}
