using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

namespace frznUpload.Shared
{
    class PingPongHandler
    {
        public int AverageTurnaround { get => (int)TurnaoroundTimes.Average(); }

        private DateTime LastActivity = DateTime.Now;
        private List<Ping> WaitingPings = new List<Ping>();
        private Queue<int> TurnaoroundTimes = new Queue<int>();

        private System.Timers.Timer PingTimer = new System.Timers.Timer();

        private MessageHandler mes;

        public event EventHandler Timeout;

        public PingPongHandler(MessageHandler messageHandler)
        {
            mes = messageHandler;

            PingTimer.Elapsed += HandlePing;
            PingTimer.Interval = 1000;
            PingTimer.AutoReset = true;
        }

        
        private void HandlePing(object sender, System.Timers.ElapsedEventArgs e)
        {
            PingTimer.Interval = TurnaoroundTimes.Average() * 10;

            if (WaitingPings.Count > 10)
            {
                mes.Stop(MessageHandler.DisconnectReason.Timeout);
                Timeout?.Invoke(this, null);
            }

            TimeSpan time =  DateTime.Now - LastActivity;

            if (time.TotalMilliseconds >= AverageTurnaround * 10)
            {
                LastActivity = DateTime.Now;
                var p = new Ping();
                WaitingPings.Add(p);
                var t = mes.SendMessage(p.Send());
#if DEBUG
                Console.WriteLine("<Ping");
#endif
            }
        }

        public bool HandleMessage(Message m)
        {
            LastActivity = DateTime.Now;
            
            if (m.Type == Message.MessageType.Ping)
            {
#if DEBUG
                Console.WriteLine(">Ping");
#endif
                if (MessagePatterns.CheckMessage(m).Item1)
                {
#if DEBUG
                    Console.WriteLine("<Pong");
#endif
                    var t = mes.SendMessage(new Message(Message.MessageType.Pong, false, m[0], MessageHandler.Version));
                }
                return true;
            }

            if (m.Type == Message.MessageType.Pong)
            {
#if DEBUG
                Console.WriteLine(">Pong");
#endif
                if (MessagePatterns.CheckMessage(m).Item1)
                {
                    var t = HandlePong(m);
                }
                return true;
            }

            return false;
        }

        private async Task HandlePong(Message m)
        {
            if (m[1] != MessageHandler.Version)
            {
                await mes.SendMessage(new Message(Message.MessageType.Version, true, MessageHandler.Version));
                return;
            }

            var List = WaitingPings.Where((p) => p.Id == m[0]).ToList();

            if (List.Count != 1)
            { 
                await mes.SendMessage(new Message(Message.MessageType.None, true));
                return;
            }

            var ping = List[0];
            ping.Pong();

            WaitingPings.Remove(ping);
            TurnaoroundTimes.Enqueue(ping.TurnaroundMs);

            if (TurnaoroundTimes.Count > 10)
                TurnaoroundTimes.Dequeue();
        }

        public void Start()
        {
            WaitingPings = new List<Ping>();
            TurnaoroundTimes = new Queue<int>();

            TurnaoroundTimes.Enqueue(100);
            
            PingTimer.Enabled = true;
        }

        public void Stop()
        {
            PingTimer.Enabled = false;
        }
    }
}
