using frznUpload.Web.Data;
using frznUpload.Web.Models;
using frznUpload.Web.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Web
{
	public class UserManager
	{
		private Database database;

		public UserManager(Database context)
		{
			database = context;
		}


		public async Task<SignInResult> SignIn(HttpContext httpContext, string name, string password, bool isPersistent, string twoFa = null)
		{
			User user = database.Users.Where(u => u.Name == name).FirstOrDefault();

			if (user is null)
				return SignInResult.Failed;

			if (HashPassword(user, password) != user.Hash)
				return SignInResult.Failed;

			if (user.TwoFaSecret != null)
			{
				if (twoFa is null)
					return SignInResult.TwoFactorRequired;

				if (!TwoFactorHandler.Verify(user.TwoFaSecret, twoFa))
					return SignInResult.Failed;
			}

			var identity = new ClaimsIdentity(GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			var authProperties = new AuthenticationProperties
			{
				AllowRefresh = true,
				IsPersistent = isPersistent,
			};

			await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
			return SignInResult.Success;
		}

		public async void SignOut(HttpContext httpContext)
		{
			await httpContext.SignOutAsync();
		}

		public async Task<bool> ChangePassword(HttpContext context, string oldPassword, string newPassword)
		{
			User user = GetUser(context);
			if (user is null)
				return false;
			if (HashPassword(user, oldPassword) != user.Hash)
				return false;
			user.Hash = HashPassword(user, newPassword);
			await database.SaveChangesAsync();
			return true;
		}


		public User GetUser(HttpContext context, Database db = null)
		{
			db ??= database;
			if (!context.User.Identity.IsAuthenticated)
				return null;
			int id = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			return db.Users.FirstOrDefault(u => u.Id == id);
		}

		public async Task<bool> DeleteUser(HttpContext context)
		{
			User user = GetUser(context);
			if (user is null)
				return false;
			SignOut(context);
			database.Users.Remove(user);
			await database.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RegisterUser(string name, string password)
		{
			if (database.Users.Any(u => u.Name == name))
				return false;

			var user = new User()
			{
				Name = name,
				Salt = GetSalt(),
			};


			user.Hash = HashPassword(user, password);
			database.Users.Add(user);
			await database.SaveChangesAsync();
			return true;
		}

		public string HashPassword(User user, string password)
		{
			string HashString = user.Name + password + user.Salt;

			using (var sha = new SHA512CryptoServiceProvider())
			{
				return Convert.ToBase64String(sha.ComputeHash(Encoding.Default.GetBytes(HashString)));
			}

		}

		private string GetSalt()
		{
			using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
			{
				byte[] tokenData = new byte[24];
				rng.GetBytes(tokenData);

				return Convert.ToBase64String(tokenData);
			}
		}

		private IEnumerable<Claim> GetUserClaims(User user)
		{
			var claims = new List<Claim>();

			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, user.Name));
			return claims;
		}
	}
}
