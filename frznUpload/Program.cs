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
            IPAddress address = IPAddress.Parse("127.0.0.1");
            var listener = new TcpListener(address, 22340);
            

            listener.Start();

            
        }

        private async Task ConnectionAcceptor(TcpListener tcp)
        {
            while (true)
            {
                var cli = await tcp.AcceptTcpClientAsync();
                EncryptionProvider enc = new EncryptionProvider();
                MessageHandler mes = new MessageHandler(cli.GetStream(), enc);
                byte[] localKey = enc.GetLocalKey();

                await mes.SendMessage(new Message(Message.MessageType.KeyExchange, new List<object> { localKey }));
                Message message = mes.WaitForMessage();


            }
        }
    }
}
