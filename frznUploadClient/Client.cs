﻿using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
    class Client : IDisposable
    {
        private TcpClient Tcp;
        private SslStream stream;
        private MessageHandler mes;

        public string Name { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public bool Connected { get => Tcp.Connected; }


        public Client(string url, int port)
        {
            Connect(url, port);
            mes.OnDisconnect += OnDisconnect;
        }

        private void OnDisconnect(object sender, MessageHandler.DisconnectReason disconnectReason)
        {
            Console.WriteLine("Disconnected: " + disconnectReason);
            Disconnect();
        }

        public void Connect(string url, int port)
        {
            Stopwatch stp = new Stopwatch();
            stp.Start();
            Tcp = new TcpClient();

            Tcp.Connect(url, port);

            stream = new SslStream(Tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));

            stream.AuthenticateAsClient("fritzen.tk");

            stp.Stop();

            Console.WriteLine("encryption established: " + stp.ElapsedMilliseconds);

            mes = new MessageHandler(Tcp, stream);
            mes.Start();
        }

        public void Disconnect()
        {
            Tcp.Close();
        }

        public async Task<bool> AuthWithKey(string file)
        {
            try
            {
                var chal = new Challenge();

                if (!File.Exists(file))
                {
                    var fileStrea = File.OpenWrite(file);
                    chal.GenerateKey(4096);
                    chal.ExportKey(fileStrea);
                    fileStrea.Close();
                }

                var fileStream = File.OpenRead(file);
                chal.LoadKey(fileStream);
                fileStream.Close();

                byte[][] pub = chal.GetPublicComponents();

                await mes.SendMessage(new Message(Message.MessageType.ChallengeRequest, false, pub[0], pub[1]));

                var m = mes.WaitForMessage(true, Message.MessageType.Challenge);

                await mes.SendMessage(new Message(Message.MessageType.ChallengeResponse, false, chal.SignChallenge(m[0])));

                m = mes.WaitForMessage(true, Message.MessageType.ChallengeApproved);

                Name = m[0];
                IsAuthenticated = true;

                return true;

            }
            catch (SequenceBreakException e)
            {
                await mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                Console.WriteLine(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AuthWithPass(string username, string password, string file)
        {
            try
            {
                var chal = new Challenge();

                if (!File.Exists(file))
                {
                    var fileStrea = File.OpenWrite(file);
                    chal.GenerateKey(4096);
                    chal.ExportKey(fileStrea);
                    fileStrea.Close();
                }

                var fileStream = File.OpenRead(file);
                chal.LoadKey(fileStream);
                fileStream.Close();

                byte[][] pub = chal.GetPublicComponents();

                await mes.SendMessage(new Message(Message.MessageType.Auth, false, username, password, pub[0], pub[1]));

                mes.WaitForMessage(true, Message.MessageType.AuthSuccess);

                return await AuthWithKey(file);
            }
            catch (SequenceBreakException e)
            {
                await mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                Console.WriteLine(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<FileUploader> UploadFile(string path)
        {
            try
            {
                var up = new FileUploader(mes, path);
                up.Start();
                return up;
            }
            catch (SequenceBreakException e)
            {
                await mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                Console.WriteLine(e);
                throw new AggregateException(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AggregateException(e);
            }
        }

        public async Task<string> ShareFile(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, string whitelist = "")
        {


            await mes.SendMessage(new Message(Message.MessageType.ShareRequest, false, fileIdentifier, firstView ? 1 : 0, isPublic ? 1 : 0, publicRegistered ? 1 : 0, whitelisted ? 1 : 0, whitelist));

            var m = mes.WaitForMessage(true, Message.MessageType.ShareResponse);

            return m[0];
        }

        public async Task<List<RemoteFile>> GetFiles()
        {
            await mes.SendMessage(new Message(Message.MessageType.FileListRequest));
            var m = mes.WaitForMessage(true, Message.MessageType.FileList);

            var list = new List<RemoteFile>();

            for (int i = 1; i < m[0] + 1; i++)
            {
                Message fileInfo = m[i];

                var file = new RemoteFile
                {
                    Filename = fileInfo[0],
                    File_extension = fileInfo[1],
                    Identifier = fileInfo[2],
                    Size = fileInfo[3],
                    Tags = BitConverter.ToInt64(fileInfo[4], 0)
                };

                list.Add(file);
            }

            return list;
        }

        public async Task<List<RemoteFile>> GetShares(string fileIdentifier)
        {
            await mes.SendMessage(new Message(Message.MessageType.FileListRequest));
            var m = mes.WaitForMessage(true, Message.MessageType.FileList);

            var list = new List<RemoteFile>();

            for (int i = 1; i < m[0] + 1; i++)
            {
                Message fileInfo = m[i];

                var file = new RemoteFile
                {
                    Filename = fileInfo[0],
                    File_extension = fileInfo[1],
                    Identifier = fileInfo[2],
                    Size = fileInfo[3],
                    Tags = BitConverter.ToInt64(fileInfo[4], 0)
                };

                list.Add(file);
            }

            return list;
        }


        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public void Dispose()
        {
            Disconnect();
            Tcp.Dispose();
            stream.Dispose();
            mes.Dispose();
        }

        ~Client()
        {
            Dispose();
        }
    }
}