using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace frznUpload.Server
{
    class Client
    {
        static X509Certificate2 Cert = new X509Certificate2("cert.pfx", "password");

        private TcpClient Tcp;
        private SslStream stream;
        private MessageHandler mes;
        private CancellationTokenSource tokenSource;
        private DataBase db;

        public bool IsAuthenticated { get; private set; }


        public Client(TcpClient tcp)
        {
            Tcp = tcp;
            db = new DataBase();

            stream = new SslStream(tcp.GetStream(), false);

            stream.AuthenticateAsServer(Cert, false, true);

            mes = new MessageHandler(stream);
            mes.Start();

            Console.WriteLine("encryption established");
        }

        public void Start()
        {
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ClientLoop(tokenSource.Token);
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
                    var message = await new Task<Message>(() => mes.WaitForMessage(), token);

                    if (message.IsError)
                    {
                        Console.WriteLine(message);
                        stream.Close();
                    }


                    if (!IsAuthenticated)
                    {
                        switch (message.Type)
                        {
                            case Message.MessageType.ChallengeRequest:

                                var chal = new Challenge();
                                chal.SetPublicComponents((byte[])message[0], (byte[])message[1]);
                                if (!db.CheckTokenExists(chal.GetThumbprint()))
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.Challenge, true));
                                    break;
                                }

                                await mes.SendMessage(new Message(Message.MessageType.Challenge, false, chal.GenerateChallenge(8)));

                                var m = mes.WaitForMessage(true, Message.MessageType.ChallengeResponse);

                                bool auth = chal.ValidateChallenge((byte[])m[0]);

                                if(auth == false)
                                {
                                    await mes.SendMessage(new Message(Message.MessageType.ChallengeApproved, true));
                                    break;
                                }

                                db.SetUser((byte[])m[0]);
                                IsAuthenticated = true;

                                await mes.SendMessage(new Message(Message.MessageType.ChallengeApproved, false, db.Name));
                                break;

                            case Message.MessageType.Auth:



                                break;

                            default:
                                await mes.SendMessage(new Message(Message.MessageType.Sequence, true, "Not authenticated"));
                                break;
                        }
                    }
                }
            }catch(SequenceBreakException e)
            {
                Console.WriteLine(e);
                await mes.SendMessage(new Message(Message.MessageType.Sequence, true, e.ToString()));
                stream.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e);
                stream.Close();
            }
        }
    }
}
