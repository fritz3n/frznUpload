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
            Console.WriteLine("Starting Logger");

            Logger.Open();
            Console.SetError(Logger.TextWriter);

            Logger.WriteLineStatic("Logger Online");

            IPAddress address = IPAddress.Any;
            var listener = new TcpListener(address, 22340);
            

            string certdir = "cert.pfx";
             
#if Release 
            certdir = "../certs/cert.pfx";
#endif
            Logger.WriteLineStatic("Loading keyfile: " + certdir);

            Cert = new X509Certificate2(certdir, "FECBA15DE2919B0FF055E2C0A513261399B894691F208FE8AD54878824390902B2FC2753354FF173747F8B8079353ABCA10DEAED03482E419087CD044A5868F6");

            listener.Start();

            Logger.WriteLineStatic("Server initialized");

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
