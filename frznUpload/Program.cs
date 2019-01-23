﻿using System;
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
        static bool Verbose = false;

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
                Console.WriteLine(args[i]);

            Console.WriteLine("Starting Logger");

            Logger.Open();
            Console.SetError(Logger.TextWriter);

            if (args.Length > 1 && args[1] == "v")
            {
                Verbose = true;
                Logger.WriteLineStatic("!!VERBOSE MODE!!", "fuck");
            }

            Logger.WriteLineStatic("Logger Online: " + Logger.FileName);
            Logger.WriteLineStatic("Working directory: " + System.Environment.CurrentDirectory);
            Logger.WriteLineStatic("Version: " + MessageHandler.Version);

            IPAddress address = IPAddress.Any;
            var listener = new TcpListener(address, 22340);


            string certdir = "../certs/cert.pfx";

#if DEBUG
            certdir = "cert.pfx";
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

                var Client = new Client(cli, Cert, Verbose);

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
