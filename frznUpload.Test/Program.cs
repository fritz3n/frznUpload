using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

            var stream = tcpclnt.GetStream();

            Console.WriteLine("Connection established");

            var headerBuffer = new byte[4];

            int l = stream.Read(headerBuffer, 0, 4);

            Console.WriteLine("got package");

            if (l != 4)
                throw new Exception("Omae Wa Mou Shindeiru");

            int length = BitConverter.ToInt32(headerBuffer, 0);
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, 0);

            var enc = new EncryptionProvider();
            enc.SetRemoteKey(bytes);

            Console.WriteLine("encryption established");

            var hand = new MessageHandler(stream, enc);
            hand.Start();

            hand.SendMessage(new Message(Message.MessageType.KeyExchange, new object[] { enc.GetLocalKey() })).Wait();

            while (true)
            {
                hand.WaitForMessage();
            }

        }
    }
}
