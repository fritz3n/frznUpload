using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Test
{
    class Client
    {
        private TcpClient Tcp;
        private SslStream stream;
        private MessageHandler mes;

        public Client(string url, int port)
        {
            Tcp = new TcpClient();

            Tcp.Connect(url, port);

            stream = new SslStream(Tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));

            stream.AuthenticateAsClient("fritzen.tk");

            Console.WriteLine("encryption established");

            mes = new MessageHandler(stream);
            mes.Start();
        }

        public async bool AuthWithKey(string file)
        {
            if (!File.Exists(file))
                return false;


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
    }
}
