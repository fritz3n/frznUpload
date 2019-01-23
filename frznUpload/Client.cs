using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Server
{
    class Client : IDisposable
    {
        private TcpClient Tcp;
        private SslStream stream;
        private MessageHandler mes;
        private CancellationTokenSource tokenSource;
        private DataBase db;
        private Logger log;

        public bool IsAuthenticated { get; private set; }

        public event EventHandler OnDispose;

        public Client(TcpClient tcp, X509Certificate2 Cert, bool verbose = false)
        {
            log = new Logger();

            log.WriteLine("Client born");

            Tcp = tcp;
            db = new DataBase();

            stream = new SslStream(tcp.GetStream(), false);

            stream.AuthenticateAsServer(Cert, false, true);

            log.WriteLine("Encryption established");

            mes = new MessageHandler(tcp, stream, verbose, log.VerboseMessageLogger);
            mes.OnDisconnect += OnDisconnect;
            mes.Start();

            log.WriteLine("Client initialized");
        }

        private void OnDisconnect(object sender, MessageHandler.DisconnectReason disconnectReason)
        {
            log.WriteLine("Disconnected: " + disconnectReason);
        }

        public void Start()
        {
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            new Thread(() => ClientLoop(tokenSource.Token)).Start();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        
        public void Stop()
        {
            tokenSource?.Cancel();
            mes.Stop();
        }

        private async Task ClientLoop(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    Message message;
                    try
                    {
                        Task<Message> t = new Task<Message>(() => mes.WaitForMessage(), token);
                        t.Start();

                        message = await t;
                    }catch(GracefulShutdownException)
                    {
                        log.WriteLine("Client disconnected");

                        Dispose();
                        return;
                    }
                    catch (Exception e)
                    {
                        log.WriteLine(e);
                        try
                        {
                            await mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));
                        }
                        catch { }

                        log.WriteLine("Shutting down because of a non-recoverable error");
                        Dispose();
                        return;
                    }

                    

                    if (message.IsError)
                    {
                        log.WriteLine(message);
                        stream.Close();
                    }


                    if (!IsAuthenticated)
                    {
                        switch (message.Type)
                        {
                            case Message.MessageType.ChallengeRequest:

                                var chal = new Challenge();
                                chal.SetPublicComponents(message[0], message[1]);
                                if (!db.CheckTokenExists(chal.GetThumbprint()))
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.Challenge, true, "Token not registered"));
                                    break;
                                }
                                
                                await mes.SendMessage(new Message(Message.MessageType.Challenge, false, chal.GenerateChallenge(8)));
                                var m = mes.WaitForMessage(true, Message.MessageType.ChallengeResponse);
                                
                                bool auth = chal.ValidateChallenge(m[0]);

                                if(auth == false)
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.ChallengeApproved, true, "Challenge failed"));
                                    log.WriteLine("Failed to authenticate using Public Key");
                                    break;
                                }

                                db.SetUser(chal.GetThumbprint());
                                IsAuthenticated = true;

                                await mes.SendMessage(new Message(Message.MessageType.ChallengeApproved, false, db.Name));

                                log.WriteLine("Authenticated using Public Key");
                                log.WriteLine("Username: ", db.Name);
                                log.Id = db.Name;
                                break;

                            case Message.MessageType.Auth:

                                chal = new Challenge();

                                chal.SetPublicComponents(message[2], message[3]);

                                if(db.SetToken(message[0], message[1], chal.GetThumbprint()))
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.AuthSuccess));
                                    log.WriteLine("Authenticated a Public Key");
                                }
                                else
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.AuthSuccess, true, "Login data not correct"));
                                    log.WriteLine("Failed to authenticate a Public Key");
                                }

                                break;

                            default:
                                await mes.SendMessage(new Message(Message.MessageType.Sequence, true, "Not authenticated"));
                                break;
                        }
                    }
                    else
                    {
                        switch (message.Type)
                        {
                            case Message.MessageType.FileUploadRequest:

                                (bool, string) returned = await FileHandler.ReceiveFile(message, mes, db, log);

                                if (returned.Item1)
                                    log.WriteLine("Uploaded a file: ", returned.Item2.Substring(0, 10));
                                else
                                    log.WriteLine("Failed to upload a file");

                                break;

                            case Message.MessageType.FileListRequest:

                                var fileList = db.GetFiles();

                                dynamic[] Fields = new dynamic[1 + fileList.Count];
                                Fields[0] = fileList.Count;

                                for(int i = 0; i < fileList.Count; i++)
                                {
                                    Fields[i + 1] = new Message(Message.MessageType.FileInfo, false, fileList[i].Filename, fileList[i].File_extension, fileList[i].Identifier, fileList[i].Size, BitConverter.GetBytes(fileList[i].Tags));
                                }

                                await mes.SendMessage(new Message(Message.MessageType.FileList, false, Fields));

                                log.WriteLine("Listed files");

                                break;

                            case Message.MessageType.ShareListRequest:

                                List<Share> shareList = db.GetShares(message[0]);

                                Fields = new dynamic[1 + shareList.Count];
                                Fields[0] = shareList.Count;

                                for (int i = 0; i < shareList.Count; i++)
                                {
                                    Fields[i + 1] = new Message(Message.MessageType.FileInfo, false, shareList[i].Share_id, shareList[i].File_identifier, shareList[i].First_view, shareList[i].Public, shareList[i].Public_registered, shareList[i].Whitelisted, shareList[i].Whitelist);
                                }

                                await mes.SendMessage(new Message(Message.MessageType.FileList, false, Fields));

                                log.WriteLine("Listed shares for file: " + message[0]);

                                break;

                            case Message.MessageType.ShareRequest:

                                string id = db.SetFileShare(
                                    message[0],
                                    message[1] == 1,
                                    message[2] == 1,
                                    message[3] == 1,
                                    message[4] == 1,
                                    message[5]
                                    );

                                await mes.SendMessage(new Message(Message.MessageType.ShareResponse, false, id));

                                log.WriteLine("Created a share: " + id);

                                break;

                            default:
                                await mes.SendMessage(new Message(Message.MessageType.Sequence, true, "Not expected"));
                                break;
                        }
                    }
                }
            }catch(SequenceBreakException e)
            {
                log.WriteLine(e);
                try
                {
                    await mes.SendMessage(new Message(Message.MessageType.Sequence, true, e.ToString()));
                }
                catch { }

                log.WriteLine("Shutting down because of a non-recoverable error");
                Dispose();
            } catch(Exception e)
            {
                log.WriteLine(e);
                try
                {
                    await mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));
                }
                catch { }

                log.WriteLine("Shutting down because of a non-recoverable error");
                Dispose();
            }
        }
        
        public void Dispose()
        {
            stream?.Dispose();
            stream = null;

            db?.Dispose();
            db = null;

            mes?.Dispose();
            mes = null;

            OnDispose?.Invoke(this, null);
        }

        ~Client() => Dispose();
    }
}
