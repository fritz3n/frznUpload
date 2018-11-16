using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using frznUpload.Shared;

namespace frznUpload.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Any;
            var listener = new TcpListener(address, 22340);
            

            listener.Start();

            ConnectionAcceptor(listener).Wait();
        }

        static private async Task ConnectionAcceptor(TcpListener tcp)
        {
            while (true)
            {

                var cli = await tcp.AcceptTcpClientAsync();

                Console.WriteLine("Connection established");


                EncryptionProvider enc = new EncryptionProvider();
                MessageHandler mes = new MessageHandler(cli.GetStream(), enc);
                byte[] localKey = enc.GetLocalKey();
                mes.Start();

                Console.WriteLine("encryption established");


                await mes.SendMessage(new Message(Message.MessageType.KeyExchange, new List<object> { localKey }));
                Message message = mes.WaitForMessage();

                Console.WriteLine("got package");


            }
        }
    }
}
