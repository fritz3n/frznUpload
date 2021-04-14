using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Web.Files
{
	public class FileManager
	{
		private readonly Database database;
		private readonly IConfiguration config;
		static Random rnd = new Random();

		public FileManager(Database database, IConfiguration config)
		{
			this.database = database;
			this.config = config;
		}


		public Task<FileStream> CreateFile(User owner, int size, string name, string path, out string identifier)
		{
			identifier = GetAvailableFileIdentifier();

			string localFilename = GetFilePath(identifier);

			FileStream file = System.IO.File.Open(localFilename, FileMode.Create, FileAccess.Write, FileShare.Read);

			database.Files.Add(new Models.File()
			{
				Identifier = identifier,
				Filename = Path.GetFileNameWithoutExtension(name),
				Extension = Path.GetExtension(name).Replace(".", ""),
				Path = path,
				Size = size,
				User = owner,
				Uploaded = DateTime.Now
			});
			return database.SaveChangesAsync().ContinueWith((a) => file);
		}

		public Task<string> CreateShare(string fileId, bool firstView, bool isPublic, bool publicRegistered, bool whitelisted, IEnumerable<string> whitelist)
		{
			Models.File file = database.Files.First(file => file.Identifier == fileId);
			return CreateShare(file, firstView, isPublic, publicRegistered, whitelisted, whitelist);
		}

		public async Task<string> CreateShare(Models.File file, bool firstView, bool isPublic, bool publicRegistered, bool whitelisted, IEnumerable<string> whitelist)
		{
			string identifier = GetAvailableShareIdentifier();

			database.Shares.Add(new Models.Share()
			{
				Identifier = identifier,
				Created = DateTime.Now,
				File = file,
				FirstView = firstView,
				Public = isPublic,
				PublicRegistered = publicRegistered,
				Whitelisted = whitelisted,
				Whitelist = whitelist,
				DontTrack = file.User.Role.HasFlag(UserRole.DontTrack)
			});
			await database.SaveChangesAsync();

			return identifier;
		}

		public async Task DestroyFile(string fileId)
		{
			Models.File file = database.Files.FirstOrDefault(file => file.Identifier == fileId);
			if (file is null)
				return;

			System.IO.File.Delete(GetFilePath(fileId));

			database.Files.Remove(file);
			await database.SaveChangesAsync();
		}

		private string GetFilePath(string fileId)
		{

			return Path.Combine(config.GetValue<string>("FileDirectory"), fileId + ".file");
		}

		public string GetAvailableFileIdentifier()
		{
			string identifier;

			do
			{
				identifier = GenerateFileIdentifier();
			} while (database.Files.Any(f => f.Identifier == identifier));

			return identifier;
		}

		private string GenerateFileIdentifier()
		{
			byte[] rndBytes = new byte[96];
			rnd.NextBytes(rndBytes);

			return Convert.ToBase64String(rndBytes).Replace('/', '-');
		}

		public string GetAvailableShareIdentifier()
		{
			string identifier = "";

			do
			{
				identifier = GenerateShareIdentifier();
			} while (database.Shares.Any(s => s.Identifier == identifier));

			return identifier;
		}

		const string shareIdCharacters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz-+.*";
		const int shareIdLength = 6;

		public static string GenerateShareIdentifier()
		{
			var sb = new StringBuilder(shareIdLength);

			for (int i = 0; i < shareIdLength; i++)
				sb.Append(shareIdCharacters[rnd.Next(shareIdCharacters.Length)]);

			return sb.ToString();
		}

	}
}
