using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace frznUpload.Client.Config
{
	static class ConfigHandler
	{
		public static string Path { get; set; }

		public static Configuration Config { get; set; } = null;

		/// <summary>
		/// Goes through a list of config paths until one config file is succesfully loaded. Does not set <see cref="ConfigHandler.Path"/>.
		/// Returns the loaded path or null if none were loaded.
		/// </summary>
		/// <param name="path"></param>
		public static string LoadFromPaths(string[] paths)
		{
			foreach (string path in paths)
			{
				if (File.Exists(path))
				{
					try
					{
						string json = File.ReadAllText(path);
						Config = JsonSerializer.Deserialize<Configuration>(json);
						return path;
					}
					catch (Exception) { }
				}
			}

			Config = new Configuration();
			return null;
		}

		public static void Save()
		{
			string json = JsonSerializer.Serialize(Config);
			File.WriteAllText(Path, json);
		}
	}
}
