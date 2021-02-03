using CliWrap;
using CliWrap.Buffered;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace frznUpload.Client.Handlers
{
	static class UpdateHandler
	{
		private static ILog log = LogManager.GetLogger(nameof(UpdateHandler));
		private static System.Timers.Timer timer;
		private const string relativePath = "../Update.exe";
		private const string user = "fritz3n";
		private const string repo = "frznUpload";
		private const string exeName = "frznUpload.Client.exe";

		static UpdateHandler()
		{
			timer = new System.Timers.Timer(1000 * 60 * 60); // every hour
			timer.Elapsed += Timer_Elapsed;
			timer.AutoReset = true;
			timer.Enabled = true;
		}

		private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			await UpdateAndStop();
		}

		public static async Task UpdateAndStop()
		{
			try
			{
				bool stop = await UpdateIfPossible();
				if (stop)
				{
					Application.Exit();
					Environment.Exit(0);
				}
			}
			catch (Exception ex)
			{
				log.Error("Error while updating", ex);
			}
		}

		public static async Task<bool> UpdateIfPossible()
		{
			if (!Exists()) // Ignore updating if Update.exe does not exist
			{
				log.Warn("No Updater found");
				return false;
			}

			string url = await GetUpdateUrl();

			log.Info("Trying to update from " + url);

			BufferedCommandResult result = await Cli.Wrap(relativePath)
				.WithArguments(b => b.Add($"--checkForUpdate={url}", true))
				.WithValidation(CommandResultValidation.None)
				.ExecuteBufferedAsync();

			if (result.ExitCode != 0)
			{
				log.Error($"Checked for updates with output and ERROR:\n{result.StandardOutput}\n{result.StandardError}");
				return false;
			}

			log.Info($"Checked for updates with output:\n{result.StandardOutput}");

			string json = result.StandardOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Last();

			SquirrelInfo info = JsonConvert.DeserializeObject<SquirrelInfo>(json);

			if (info.CurrentVersion == info.FutureVersion)
			{
				log.Info($"No update available");
				return false;
			}

			result = await Cli.Wrap(relativePath)
				.WithArguments(b => b.Add($"--update={url}", true))
				.WithValidation(CommandResultValidation.None)
				.ExecuteBufferedAsync();

			if (result.ExitCode != 0)
			{
				log.Error($"Ran updater with output and ERROR:\n{result.StandardOutput}\n{result.StandardError}");
				return false;
			}

			log.Info($"Ran updater with output:\n{result.StandardOutput}");

			log.Info("Restarting application for update.");

			Process.Start(relativePath, string.Format("--processStartAndWait \"{0}\"", exeName));

			return true;
		}

		static async Task<string> GetUpdateUrl()
		{
#if DEBUG
			return @"C:\Users\fritzen\Source\Repos\frznUpload\frznUploadClient\Releases";
#endif
			bool pre = Config.AppSettings.AllKeys.Contains("Prerelease") && Config.AppSettings["Prerelease"].Value == "true";

			List<GithubRelease> releases = await GetReleases();
			GithubRelease latestRelease = releases
					.Where(x => pre || !x.Prerelease)
					.OrderByDescending(x => x.PublishedAt)
					.First();

			return latestRelease.HtmlUrl.Replace("/tag/", "/download/");
		}

		static async Task<List<GithubRelease>> GetReleases()
		{
			string json = await RequestJson();
			return JsonConvert.DeserializeObject<List<GithubRelease>>(json);
		}

		static async Task<string> RequestJson()
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add("User-Agent", "frznUpload");
			return await client.GetStringAsync(GetApiUrl());
		}

		static string GetApiUrl()
		{
			return $"https://api.github.com/repos/{user}/{repo}/releases";
		}
		static bool Exists()
		{
			return File.Exists(relativePath);
		}


		class GithubRelease
		{
			[JsonProperty("prerelease")]
			public bool Prerelease { get; set; }

			[JsonProperty("published_at")]
			public DateTime PublishedAt { get; set; }

			[JsonProperty("html_url")]
			public string HtmlUrl { get; set; }
		}

		class SquirrelInfo
		{

			[JsonProperty("currentVersion")]
			public string CurrentVersion { get; set; }

			[JsonProperty("futureVersion")]
			public string FutureVersion { get; set; }

			[JsonProperty("releasesToApply")]
			public List<SquirrelRelease> ApplyableReleases { get; set; }
		}

		class SquirrelRelease
		{
			[JsonProperty("version")]
			public string Version { get; set; }

			[JsonProperty("releaseNotes")]
			public string ReleaseNotes { get; set; }
		}
	}
}
