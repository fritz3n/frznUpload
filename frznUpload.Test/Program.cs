using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using frznUpload.Shared;

namespace frznUpload.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient tcpclnt = new TcpClient();

            tcpclnt.Connect("192.168.2.175", 22340);

            var stream = new SslStream(tcpclnt.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));

            stream.AuthenticateAsClient("fritzen.tk");

            Console.WriteLine("encryption established");

            var hand = new MessageHandler(stream);
            hand.Start();

            hand.SendMessage(new Message(Message.MessageType.Auth, "test")).Wait();

            while (true)
            {
                Console.WriteLine(hand.WaitForMessage());
            }

        }

        public static bool ValidateServerCertificate(
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
