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
		private Database context;

		public UserManager(Database context)
		{
			this.context = context;
		}


		public async Task<SignInResult> SignIn(HttpContext httpContext, string name, string password, bool isPersistent, string twoFa = null)
		{
			User user = context.Users.Where(u => u.Name == name).First();

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

		public async Task RegisterUser(string name, string password)
		{
			var user = new User()
			{
				Name = name,
				Salt = GetSalt(),
			};
			user.Hash = HashPassword(user, password);
			context.Users.Add(user);
			await context.SaveChangesAsync();
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
