using frznUpload.Shared;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
	static class Program
	{
		private static ILog log;

		static Program()
		{
#if DEBUG
			AllocConsole();
#endif
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			var f = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

			Directory.SetCurrentDirectory(f.Directory.FullName);

			var configFileMap = new ExeConfigurationFileMap();

#if DEBUG
			configFileMap.ExeConfigFilename = "App.Release.config";
#else
			configFileMap.ExeConfigFilename = "App.Release.config";
#endif
			Config.Configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);


			var rollingFileAppender = new RollingFileAppender()
			{
				Layout = new PatternLayout("%date{HH:mm:ss} [%thread] %-3level %logger - %message%newline"),
				File = "rollingLog.txt",
				AppendToFile = true,
				MaximumFileSize = "100KB",
				MaxSizeRollBackups = 2
			};
			rollingFileAppender.ActivateOptions();

			BasicConfigurator.Configure(
				new ConsoleAppender(),
				rollingFileAppender
			);

			log = LogManager.GetLogger(nameof(Program));


			log.Info("Starting...");

			log.Info("Working Directory: " + Directory.GetCurrentDirectory());

			Application.ThreadException += Application_ThreadException;


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

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			log.Error("Uncaught Exception", e.Exception);
		}

#if DEBUG
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();
#endif
	}
}
