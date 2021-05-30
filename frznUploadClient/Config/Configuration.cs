using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Config
{
	class Configuration
	{
		public bool Prerelease { get; set; } = false;
		public string CertFilePath { get; set; } = "../client.pfx";
		public int Port { get; set; } = 22340;
		public string Url { get; set; } = "dyn.fritzen.xyz";
		public string SharePrefix { get; set; } = "https://fritzen.xyz/v/";
		public string[] Hotkeys { get; set; } = Array.Empty<string>();

	}
}
