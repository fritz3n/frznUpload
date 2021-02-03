using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
	public static class AutostartHandler
	{
		private static ILog log = LogManager.GetLogger(nameof(AutostartHandler));
		private static RegistryKey rkApp = null;
		private const string AppName = "frznUpload";

		public static void Init()
		{
			try
			{
				rkApp = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
			}
			catch (Exception e)
			{
				log.Error("Error while accsessing registry", e);
				return;
			}

			if (rkApp == null)
			{
				log.Error("Registrykey was not retrieved");
				return;
			}

			//if val is not null and paths dont match -> set to current path
			if (!IsNull() && !PathIsSame())
			{
				SetToCurrentPath();
			}
		}

		public static bool IsNull()
		{
			return rkApp.GetValue(AppName) == null;
		}


		public static bool IsEnabled()
		{
			return rkApp.GetValue(AppName) is not null;
		}

		public static bool PathIsSame()
		{

			if ((string)rkApp.GetValue(AppName) == GetExecutingPath())
			{
				return true;
			}
			return false;
		}

		public static string GetExecutingPath()
		{
			return System.Windows.Forms.Application.ExecutablePath;
		}

		public static void SetValue(string val)
		{
			rkApp.SetValue(AppName, val);
		}

		public static void SetToCurrentPath()
		{
			SetValue(GetExecutingPath());
		}

		public static void DeleteKey()
		{
			rkApp.DeleteValue(AppName);
		}

	}
}
