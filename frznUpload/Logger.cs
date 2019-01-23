using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using frznUpload.Shared;

namespace frznUpload.Server
{
    class Logger
    {
        const string seperator = "   ";
        const string staticId = "~";
        const string dateFormat = "H:mm d.M.yy";

        static FileStream file;
        static StreamWriter writer;
        public static TextWriter TextWriter { get => writer; }
        public MessageLogger VerboseMessageLogger { get; private set; }

        public class MessageLogger : IMessageLogger
        {
            private Logger Logger;

            public MessageLogger(Logger logger)
            {
                Logger = logger;
            }

            public void LogMessage(bool outBound, Message message)
            {
                Logger.WriteLine((outBound ? "<- " : "-> ") + message);
            }
        }

        static string _filename = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/var/log/frznUpload/frznUpload.log" : "log.txt";
        static public string FileName { get => _filename; set
            {
                bool wasOpen = Opened;
                if (Opened)
                    Close();
                _filename = value;
                if (wasOpen)
                    Open();
            }
        }
        static public bool Opened { get; set; }

        public string  Id {
            get => id;
            set
            {
                WriteLineStatic("Changed Id", id + "->" + value);
                id = value;
            }
        }

        string id;

        public Logger(string id = null)
        {
            VerboseMessageLogger = new MessageLogger(this);

            if (id == null)
            {
                long ms = DateTime.Now.Minute;
                long ms2 = DateTime.Now.Second;
                id = string.Format("{0:X}-{1:X}", ms, ms2).ToLower();
            }

            this.id = id;
        }

        public static void Open()
        {
            lock (_filename)
            {
                if (Opened)
                    return;

                file = File.Open(_filename, FileMode.Append, FileAccess.Write);
                writer = new StreamWriter(file);
                Opened = true;
            }
        }

        public static void Close()
        {
            lock (_filename)
            {
                if (!Opened)
                    return;

                file?.Close();
                file?.Dispose();
                file = null;
                writer.Dispose();
                Opened = false;
            }
        }

        public static void WriteLineStatic(string str, string id = staticId)
        {
            if (!Opened)
                Open();

            string date = DateTime.Now.ToString(dateFormat);

            string write = $"[{date}]{seperator}{id}{seperator}{str}";

            Console.WriteLine(write);

            lock (writer)
            {
                writer.WriteLine(write);
                writer.Flush();
            }
        }

        public void WriteLine(params object[] objects)
        {
            string str = "";
            foreach(object obj in objects)
            {
                str += obj;
            }

            WriteLineStatic(str, id);
        }
    }
}
