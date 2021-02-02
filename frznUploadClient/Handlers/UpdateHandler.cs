using CliWrap;
using CliWrap.Buffered;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Handlers
{
	static class UpdateHandler
	{
		private static ILog log = LogManager.GetLogger(nameof(UpdateHandler));
		private const string relativePath = "../Update.exe";
		private const string user = "fritz3n";
		private const string repo = "frznUpload";

		public static async Task UpdateIfPossible()
		{
			if (!Exists()) // Ignore updating if Update.exe does not exist
				return;

			string url = await GetUpdateUrl();
			BufferedCommandResult result = await Cli.Wrap(relativePath)
				.WithArguments(b => b.Add($"--update={url}", true))
				.WithValidation(CommandResultValidation.None)
				.ExecuteBufferedAsync();

			if (result.ExitCode == 0)
			{
				log.Info($"Ran updater with output:\n{result.StandardOutput}");
			}
			else
			{
				log.Error($"Ran updater with output and ERROR:\n{result.StandardOutput}\n{result.StandardError}");
			}
		}

		static async Task<string> GetUpdateUrl()
		{
#if DEBUG
			return @"C:\Users\fritzen\Source\Repos\frznUpload\frznUploadClient\Releases";
#endif

			List<Release> releases = await GetReleases();
			Release latestRelease = releases
					.Where(x => !x.Prerelease)
					.OrderByDescending(x => x.PublishedAt)
					.First();

			return latestRelease.HtmlUrl.Replace("/tag/", "/download/");
		}

		static async Task<List<Release>> GetReleases()
		{
			string json = await RequestJson();
			return JsonConvert.DeserializeObject<List<Release>>(json);
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


		class Release
		{
			[JsonProperty("prerelease")]
			public bool Prerelease { get; set; }

			[JsonProperty("published_at")]
			public DateTime PublishedAt { get; set; }

			[JsonProperty("html_url")]
			public string HtmlUrl { get; set; }
		}

	}
}
