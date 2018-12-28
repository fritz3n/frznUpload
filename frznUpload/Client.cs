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

        public Client(TcpClient tcp, X509Certificate2 Cert)
        {
            log = new Logger();

            log.WriteLine("Client born");

            Tcp = tcp;
            db = new DataBase();

            stream = new SslStream(tcp.GetStream(), false);

            stream.AuthenticateAsServer(Cert, false, true);

            log.WriteLine("Encryption established");

            mes = new MessageHandler(tcp, stream);
            mes.Start();

            log.WriteLine("Client initialized");
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
                                    break;
                                }

                                db.SetUser(chal.GetThumbprint());
                                IsAuthenticated = true;

                                await mes.SendMessage(new Message(Message.MessageType.ChallengeApproved, false, db.Name));
                                break;

                            case Message.MessageType.Auth:

                                chal = new Challenge();

                                chal.SetPublicComponents(message[2], message[3]);

                                if(db.SetToken(message[0], message[1], chal.GetThumbprint()))
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.AuthSuccess));
                                }
                                else
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.AuthSuccess, true, "Login data not correct"));
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

                                await FileHandler.ReceiveFile(message, mes, db);

                                break;

                            case Message.MessageType.FileListRequest:

                                var list = db.GetFiles();

                                dynamic[] Fields = new dynamic[1 + list.Count];
                                Fields[0] = list.Count;

                                for(int i = 0; i < list.Count; i++)
                                {
                                    Fields[i + 1] = new Message(Message.MessageType.FileInfo, false, list[i].Filename, list[i].File_extension, list[i].Identifier, list[i].Size, BitConverter.GetBytes(list[i].Tags));
                                }

                                await mes.SendMessage(new Message(Message.MessageType.FileList, false, Fields));

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

                Dispose();
            } catch(Exception e)
            {
                log.WriteLine(e);
                try
                {
                    await mes.SendMessage(new Message(Message.MessageType.None, true, e.ToString()));
                }
                catch { }

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
