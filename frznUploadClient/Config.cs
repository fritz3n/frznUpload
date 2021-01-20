using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
	static class Config
	{
		public static Configuration Configuration { get; set; }
		public static KeyValueConfigurationCollection AppSettings => Configuration.AppSettings.Settings;
	}
}
