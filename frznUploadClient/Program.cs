using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var configFileMap = new ExeConfigurationFileMap();

#if DEBUG
			configFileMap.ExeConfigFilename = "App.Debug.config";
#else
			configFileMap.ExeConfigFilename = "App.Release.config";
#endif
			Config.Configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
			Console.WriteLine("Starting...");

			var f = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

			Directory.SetCurrentDirectory(f.Directory.FullName);

			Console.WriteLine("Working Directory: " + Directory.GetCurrentDirectory());

			ExplorerIntegrationHandler.Init();

			if (PipeHandler.IsServer)
			{

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new IconHandler(args));
			}
			else
			{
				PipeHandler.SendMessage(args);
			}
		}
	}
}
