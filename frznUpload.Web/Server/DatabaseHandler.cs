using frznUpload.Web.Data;
using frznUpload.Web.Files;
using frznUpload.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Web.Server
{
	public class DatabaseHandler
	{
		private readonly Database database;
		public User User { get; private set; }
		private string serial;
		private Random rnd = new();
		public FileManager FileManager { get; }

		public string SerialNumber => serial;

		public bool IsAuthenticated { get; private set; }
		public string Name
		{
			get
			{
				if (!IsAuthenticated)
					throw new Exception("not authenticated");

				return User.Name;
			}
		}

		public DatabaseHandler(Database database, FileManager fileManager)
		{
			FileManager = fileManager;
			this.database = database;
		}

		public void SetUser(string serial)
		{
			Token token = database.Tokens.SingleOrDefault(t => t.Serial == serial);
			User = token?.User ?? throw new KeyNotFoundException();
			token.LastUsed = DateTime.Now;
			database.SaveChanges();
			this.serial = serial;
			IsAuthenticated = true;
		}

		public void Deauthenticate()
		{
			if (!IsAuthenticated)
				return;

			database.Tokens.Remove(database.Tokens.Where(t => t.Serial == serial).FirstOrDefault());
			database.SaveChanges();
			IsAuthenticated = false;
		}

		public string HashPassword(User user, string password)
		{
			user = user ?? throw new KeyNotFoundException();

			if (string.IsNullOrEmpty(user.Hash))
			{
				using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
				{
					byte[] tokenData = new byte[24];
					rng.GetBytes(tokenData);

					user.Salt = Convert.ToBase64String(tokenData);
				}

				database.SaveChanges();
			}

			string HashString = user.Name + password + user.Salt;

			using (var sha = new SHA512CryptoServiceProvider())
			{
				return Convert.ToBase64String(sha.ComputeHash(Encoding.Default.GetBytes(HashString)));
			}

		}

		public bool IsValid(string username, string password)
		{
			User user = database.Users.SingleOrDefault(u => u.Name == username);

			if (user is null)
				return false;

			if (user.Name != username)
				return false;

			return user.Hash == HashPassword(user, password);
		}

		public bool RemoveSerial(string serial)
		{
			Token token = database.Tokens.FirstOrDefault(t => t.Serial == serial);
			if (token is not null)
			{
				database.Tokens.Remove(token);
				database.SaveChanges();
				return true;
			}
			return false;
		}

		public bool SetSerial(string username, string serial, DateTime validUntil, string machineName)
		{
			User user = database.Users.SingleOrDefault(u => u.Name == username);

			database.Tokens.Add(new Token
			{
				User = user,
				Serial = serial,
				Created = DateTime.Now,
				Name = machineName,
				ValidUntil = validUntil
			});

			database.SaveChanges();
			return true;
		}

		public List<File> GetFiles()
		{
			ThrowIfNotAuthenticated();

			return User.Files.ToList();
		}


		public bool CheckTokenExists(string serial)
		{
			if (serial == null)
				throw new ArgumentNullException(nameof(serial));

			return database.Tokens.Any(t => t.Serial == serial);
		}

		public string CreateFile(string identifier, string filename, string extension, string path, int size)
		{
			ThrowIfNotAuthenticated();

			database.Files.Add(new File()
			{
				Identifier = identifier,
				Filename = filename,
				Extension = extension,
				Size = size,
				Path = path,
				User = User
			});

			database.SaveChanges();

			return identifier;
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

		private string GenerateShareIdentifier()
		{
			byte[] rndBytes = new byte[5];
			rnd.NextBytes(rndBytes);

			string s = Convert.ToBase64String(rndBytes).Replace('/', '-');

			return s.Substring(0, 6);
		}

		public string SetFileShare(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, IEnumerable<string> whitelist = null)
		{
			ThrowIfNotAuthenticated();
			File file = database.Files.SingleOrDefault(f => f.Identifier == fileIdentifier);

			if (file is null || file.User != User)
				return null;

			string shareId = GetAvailableShareIdentifier();

			file.Shares.Add(new Share()
			{
				Identifier = shareId,
				FirstView = firstView,
				Public = isPublic,
				PublicRegistered = publicRegistered,
				Whitelisted = whitelisted,
				Whitelist = whitelist,
				Created = DateTime.Now
			});

			database.SaveChanges();

			return shareId;
		}

		public List<Share> GetShares(string fileIdentifier)
		{
			if (string.IsNullOrWhiteSpace(fileIdentifier))
				throw new ArgumentException("Parameter is invalid", nameof(fileIdentifier));

			ThrowIfNotAuthenticated();

			return database.Files.SingleOrDefault(f => f.Identifier == fileIdentifier).Shares.ToList();
		}

		public void DeleteFile(string fileIdentifier)
		{
			ThrowIfNotAuthenticated();
			File file = database.Files.FirstOrDefault(f => f.Identifier == fileIdentifier);

			if (file.User == User)
			{
				database.Files.Remove(file);
				database.SaveChanges();
			}
			else
			{
				throw new UnauthorizedAccessException("The user dose not own this share");
			}
		}

		public string GetTwoFactorSecret(int? id = null)
		{
			id = id ?? User.Id;
			return database.Users.FirstOrDefault(u => u.Id == id).TwoFaSecret;
		}

		public bool HasTwoFa(int? id = null) => GetTwoFactorSecret(id) is not null;


		/// <summary>
		/// Sets a TwoFa secret in the db
		/// </summary>
		public void SetTwoFactorSecret(string val)
		{
			User.TwoFaSecret = val;
			database.SaveChanges();
		}

		public void RemoveTwoFactorSecret()
		{
			User.TwoFaSecret = null;
			database.SaveChanges();
		}

		/// <summary>
		/// Gets the corresponding id to a username
		/// </summary>
		/// <param name="username">the username to query for</param>
		/// <returns>the id or NULL if the user isnt found</returns>
		public int? GetUserId(string username)
		{

			return database.Users.SingleOrDefault(u => u.Name == username)?.Id;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		private void ThrowIfNotAuthenticated()
		{
			if (!IsAuthenticated)
				throw new ArgumentException("Not Authenticated");
		}
	}
}
