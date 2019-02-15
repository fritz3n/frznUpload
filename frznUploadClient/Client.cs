using frznUpload.Shared;
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
    public class Client : IDisposable
    {
        private TcpClient Tcp;
        private SslStream stream;
        private MessageHandler mes;

        public string Name { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public bool Connected { get => Tcp.Connected; }

        private bool disposed = false;

        public Client(string url, int port)
        {
            Connect(url, port);
        }

        public Client()
        {

        }

        private void OnDisconnect(object sender, MessageHandler.DisconnectReason disconnectReason)
        {
            Console.WriteLine("Disconnected: " + disconnectReason);
            Disconnect();
        }

        public void Connect(string url, int port)
        {
            ThrowIfDisposed();
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
            mes.OnDisconnect += OnDisconnect;

            mes.SendMessage(new Message(Message.MessageType.Version, false, MessageHandler.Version));
            var m = mes.WaitForMessage(true, Message.MessageType.Version);

            if (m[0] != MessageHandler.Version)
            {
                Dispose();
                throw new InvalidOperationException("Server version does not match Client version");
            }
        }

        public async Task ConnectAsync(string url, int port)
        {
            ThrowIfDisposed();
            Stopwatch stp = new Stopwatch();
            stp.Start();
            Tcp = new TcpClient();

            await Tcp.ConnectAsync(url, port);

            stream = new SslStream(Tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));

            await stream.AuthenticateAsClientAsync("fritzen.tk");

            stp.Stop();

            Console.WriteLine("encryption established: " + stp.ElapsedMilliseconds);

            mes = new MessageHandler(Tcp, stream);
            mes.Start();
            mes.OnDisconnect += OnDisconnect;

            mes.SendMessage(new Message(Message.MessageType.Version, false, MessageHandler.Version));
            var m = await mes.WaitForMessageAsync(true, Message.MessageType.Version);

            if(m[0] != MessageHandler.Version)
            {
                Dispose();
                throw new InvalidOperationException("Server version does not match Client version");
            }
        }

        public void Disconnect()
        {
            ThrowIfDisposed();
            mes?.Stop();
            Tcp?.Close();
        }

        public async Task<bool> AuthWithKey(string file)
        {
            ThrowIfDisposed();
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

                mes.SendMessage(new Message(Message.MessageType.ChallengeRequest, false, pub[0], pub[1]));

                var m = await mes.WaitForMessageAsync(true, Message.MessageType.Challenge);

                mes.SendMessage(new Message(Message.MessageType.ChallengeResponse, false, chal.SignChallenge(m[0])));

                m = mes.WaitForMessage(true, Message.MessageType.ChallengeApproved);

                Name = m[0];
                IsAuthenticated = true;

                return true;

            }
            catch (SequenceBreakException e)
            {
                mes.SendMessage(new Message(Message.MessageType.Sequence, true));
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
            ThrowIfDisposed();
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

                mes.SendMessage(new Message(Message.MessageType.Auth, false, username, password, pub[0], pub[1]));

                Message rec = await mes.WaitForMessageAsync(false);
                if(rec.IsError == true)
                {
                    throw new Exception("Recived an error: " + rec.ToString());
                }
                else if(rec.Type == Message.MessageType.AuthSuccess)
                {
                    //auth recived all good.
                }else if (rec.Type == Message.MessageType.TowFactorNeeded)
                {
                    //tow fa needed
                    await DoTowFaExchangeAsync();
                }
                else
                {
                    //unexpected message
                    throw new Exception("Unexpected message: " + rec.ToString());
                }

                return await AuthWithKey(file);
            }
            catch (SequenceBreakException e)
            {
                mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                Console.WriteLine(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task DeauthKey()
        {
            ThrowIfDisposed();
            try
            {
                mes.SendMessage(new Message(Message.MessageType.DeauthRequest));
                await mes.WaitForMessageAsync(true, Message.MessageType.DeauthSuccess);
                IsAuthenticated = false;
            }
            catch (SequenceBreakException e)
            {
                mes.SendMessage(new Message(Message.MessageType.Sequence, true));
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public FileUploader UploadFile(string path, bool singleUse = false, string Filename = null)
        {
            ThrowIfDisposed();
            try
            {
                var up = new FileUploader(mes, this, path, singleUse, Filename);
                up.Start();
                return up;
            }
            catch (SequenceBreakException e)
            {
                mes.SendMessage(new Message(Message.MessageType.Sequence, true));
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
            ThrowIfDisposed();


            mes.SendMessage(new Message(Message.MessageType.ShareRequest, false, fileIdentifier, firstView ? 1 : 0, isPublic ? 1 : 0, publicRegistered ? 1 : 0, whitelisted ? 1 : 0, whitelist));

            var m = await mes.WaitForMessageAsync(true, Message.MessageType.ShareResponse);

            return m[0];
        }

        public async Task<List<RemoteFile>> GetFiles()
        {
            ThrowIfDisposed();
            mes.SendMessage(new Message(Message.MessageType.FileListRequest));
            var m = await mes.WaitForMessageAsync(true, Message.MessageType.FileList);

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
            ThrowIfDisposed();
            mes.SendMessage(new Message(Message.MessageType.FileListRequest));
            var m = await mes.WaitForMessageAsync(true, Message.MessageType.FileList);

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
        public async Task DoTowFaExchangeAsync()
        {
            mes.SendMessage(new Message(Message.MessageType.TowFactorNeeded, false, GetTowFaToken()));
            await mes.WaitForMessageAsync(true, Message.MessageType.TowFactorSuccess);
        }

        public string GetTowFaToken()
        {
            return Prompt.ShowDialog("Pleas enter your tow factor authentication token", "Further authentication required");
        }

        public async Task RemoveTowFaAsync()
        {
            try
            {
                mes.SendMessage(new Message(Message.MessageType.TowFactorRemove, false));
                await mes.WaitForMessageAsync(true, Message.MessageType.TowFactorNeeded);
            }
            catch (Exception e)
            {
                throw new Exception("Error while requesting tow factor authentication removal", e);
            }
            try
            {
                await DoTowFaExchangeAsync();
                await mes.WaitForMessageAsync(true, Message.MessageType.TowFactorRemove);
            }
            catch (Exception e)
            {
                throw new Exception("Error while deleting tow factor authentication", e);
            }
        }
        
        public async Task<bool> GetHasTowFaEnabled()
        {
            mes.SendMessage(new Message(Message.MessageType.HasTowFactor, false));
            Message m = await mes.WaitForMessageAsync(Message.MessageType.HasTowFactor);
            if (m.IsError)
            {
                throw new Exception("Error getting tow factor authentication status.");
            }
            else
            {
                return m.Fields[0];
            }
        }

        public async Task<string> GetTowFaSecret()
        {
            mes.SendMessage(new Message(Message.MessageType.TowFactorAdd));
            Message m = await mes.WaitForMessageAsync(true, Message.MessageType.TowFactorAdd);
            return m.Fields[0];
        }

    /// <summary>
    /// Asks the server to delete a file
    /// </summary>
        public async Task DeleteFileAsync(string file_identifier)
        {
            mes.SendMessage(new Message(Message.MessageType.DeleteFile, false, file_identifier));
            await mes.WaitForMessageAsync(true, Message.MessageType.DeleteFile);
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

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("Client");
        }

        public void Dispose()
        {
            if (disposed)
                return;
            Disconnect();
            disposed = true;
            Tcp?.Dispose();
            stream?.Dispose();
            mes?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Client()
        {
            Dispose();
        }
    }
}
