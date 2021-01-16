using frznUpload.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Server
{
	public class MessageLogger : IMessageLogger
	{
		private readonly ILogger logger;

		public MessageLogger(ILogger logger)
		{
			this.logger = logger;
		}

		public void LogMessage(bool outBound, Message message)
		{
			logger.LogInformation((outBound ? "<- " : "-> ") + message);
		}
	}
}
