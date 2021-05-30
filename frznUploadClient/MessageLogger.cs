using frznUpload.Shared;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
	class MessageLogger : IMessageLogger
	{
		private readonly ILog log;

		public MessageLogger(ILog log)
		{
			this.log = log;
		}

		public void LogMessage(bool outBound, Message message)
		{
#if DEBUG
			log.Debug((outBound ? "<- " : "-> ") + message);
#endif
		}
	}
}
