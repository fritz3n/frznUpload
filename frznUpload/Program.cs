using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using frznUpload.Shared;

namespace frznUpload.Server
{
    class Program
    {
        static X509Certificate2 Cert;
        static List<Client> clients = new List<Client>();

        static void Main(string[] args)
        {
            Logger.Open();
            Console.SetOut(Logger.TextWriter);
            Console.SetError(Logger.TextWriter);

            IPAddress address = IPAddress.Any;
            var listener = new TcpListener(address, 22340);

            Cert = new X509Certificate2("cert.pfx", "password");

            listener.Start();

            Logger.WriteLineStatic("Started the Listener");

            var v = ConnectionAcceptor(listener).Result;
        }

        static private async Task<bool> ConnectionAcceptor(TcpListener tcp)
        {
            while (true)
            {
                var cli = await tcp.AcceptTcpClientAsync();
                
                Logger.WriteLineStatic("Connection established with " + cli.Client.RemoteEndPoint);

                var Client = new Client(cli, Cert);

                clients.Add(Client);

                Client.OnDispose += Client_OnDispose;
                
                Client.Start();
            }
            
        }

        private static void Client_OnDispose(object sender, EventArgs e)
        {
            clients.Remove(sender as Client);
            Logger.WriteLineStatic("Removed client");
        }
    }
}
