using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using frznUpload.

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
                Encryption

            }
        }
    }
}
