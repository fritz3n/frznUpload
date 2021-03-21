using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Shared
{
	public static class ShareHelper
	{
		public static async Task<(AccessStatus status, Share share)> CanAccess(HttpContext context, Database database, string shareId)
		{
			Share share = await database.Shares.FirstOrDefaultAsync(s => s.Identifier == shareId);
			if (share is null)
				return (AccessStatus.Denied, share);

			bool isAuthenticated = context.User.Identity.IsAuthenticated;
			User user = null;
			if (isAuthenticated)
				user = await database.Users.FirstOrDefaultAsync(u => u.Name == context.User.Identity.Name);

			if (share.File.User == user)
				return (AccessStatus.Owner, share);

			if (share.Public && !isAuthenticated)
				return (AccessStatus.Allowed, share);
			if (share.PublicRegistered && isAuthenticated)
				return (AccessStatus.Allowed, share);
			if (isAuthenticated && share.Whitelisted)
				return (share.Whitelist.Contains(user.Name) ? AccessStatus.Allowed : AccessStatus.Denied, share);

			if (share.FirstView)
			{
				if (share.FirstViewCookie != null)
				{
					if (!context.Request.Cookies.ContainsKey(shareId))
						return (AccessStatus.Denied, share);
					string value = context.Request.Cookies[shareId];
					if (share.FirstViewCookie == value)
						return (AccessStatus.Allowed, share);
				}
				else
				{
					share.FirstViewCookie = GetFirstViewCookie();
					context.Response.Cookies.Append(shareId, share.FirstViewCookie);
					database.SaveChanges();
					return (AccessStatus.Allowed, share);
				}
			}
			return (AccessStatus.Denied, share);
		}

		private static string GetFirstViewCookie()
		{
			using (RNGCryptoServiceProvider random = new())
			{
				byte[] data = new byte[16];
				random.GetBytes(data);
				return Convert.ToBase64String(data);
			}
		}

		public enum AccessStatus
		{
			Owner,
			Allowed,
			Denied
		}
	}
}
