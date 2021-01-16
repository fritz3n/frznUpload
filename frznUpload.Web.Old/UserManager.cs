using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
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
		private frznUploadContext context;

		public UserManager(frznUploadContext context)
		{
			this.context = context;
		}


		public async Task SignIn(HttpContext httpContext, string name, string password, bool isPersistent = false)
		{

			User user = context.Users.Where(u => u.Name == name).First();

			if (await HashPassword(user, password) != user.Hash)
				throw new UnauthorizedAccessException();

			var identity = new ClaimsIdentity(GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			var authProperties = new AuthenticationProperties
			{
				AllowRefresh = true,

				IsPersistent = isPersistent,
			};

			await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}

		public async void SignOut(HttpContext httpContext)
		{
			await httpContext.SignOutAsync();
		}

		public async Task<string> HashPassword(User user, string password)
		{
			if (string.IsNullOrEmpty(user.Hash))
			{
				using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
				{
					byte[] tokenData = new byte[24];
					rng.GetBytes(tokenData);

					user.Salt = Convert.ToBase64String(tokenData);
				}

				context.Users.Update(user);
				await context.SaveChangesAsync();
			}

			string HashString = user.Name + password + user.Salt;

			using (var sha = new SHA512CryptoServiceProvider())
			{
				return Convert.ToBase64String(sha.ComputeHash(Encoding.Default.GetBytes(HashString)));
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
