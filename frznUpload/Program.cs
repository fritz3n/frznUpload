using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using frznUpload.Shared;

namespace frznUpload.Server
{
    class Program
    {
        static X509Certificate2 Cert;

        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Any;
            var listener = new TcpListener(address, 22340);

            Cert = new X509Certificate2("cert.pfx", "password");

            listener.Start();

            ConnectionAcceptor(listener).Wait();
        }

        static private async Task ConnectionAcceptor(TcpListener tcp)
        {
            while (true)
            {

                var cli = await tcp.AcceptTcpClientAsync();

                Console.WriteLine("Connection established");

                SslStream stream = new SslStream(cli.GetStream(), false);

                stream.AuthenticateAsServer(Cert, false, true);


                MessageHandler mes = new MessageHandler(stream);
                mes.Start();

                Console.WriteLine("encryption established");


                await mes.SendMessage(new Message(Message.MessageType.Auth, "test" ));
                while(true)
                    Console.WriteLine(mes.WaitForMessage());

                Console.WriteLine("got package");


            }
        }


    }
}
