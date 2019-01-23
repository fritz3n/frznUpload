using System;
using System.Collections.Generic;
using System.Text;

namespace frznUpload.Shared
{
    public interface IMessageLogger
    {
        void LogMessage(bool outBound, Message message);
    }

    class ConsoleLogger : IMessageLogger
    {
        public void LogMessage(bool outBound, Message message)
        {
            Console.WriteLine((outBound ? "<- " : "-> ") + message);
        }
    }
}
