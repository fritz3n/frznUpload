using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace frznUpload.Shared
{
    class Ping
    {
        public int Id { get; } = (int)(new Random().NextDouble() * int.MaxValue);
        public bool Pinged { get; private set; } = false;
        public bool Ponged { get; private set; } = false;

        private Stopwatch watch = new Stopwatch();

        public int TurnaroundMs { get => Ponged ? (int)watch.ElapsedMilliseconds : int.MaxValue; }

        public Message Send()
        {
            var m = new Message(Message.MessageType.Ping, false, Id, MessageHandler.Version);
            Pinged = true;

            watch.Start();

            return m;
        }

        public int Pong()
        {
            watch.Stop();
            Ponged = true;

            return TurnaroundMs;
        }
    }
}
