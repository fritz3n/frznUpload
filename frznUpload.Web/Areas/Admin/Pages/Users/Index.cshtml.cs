using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Admin.Pages.Users
{
	public class IndexModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;

		public IndexModel(frznUpload.Web.Data.Database context)
		{
			_context = context;
		}

		public IList<UserListing> Users { get; set; }

		public record UserListing(int Id, string Name, UserRole Role, int FileCount, string TotalSize);


		public async Task OnGetAsync()
		{
			Users = await _context.Users
				.Select(u => new { u.Id, u.Name, u.Role, u.Files.Count, Size = u.Files.Sum(f => f.Size) })
				.Select(t => new UserListing(t.Id, t.Name, t.Role, t.Count, BytesToString(t.Size)))
				.ToListAsync();
		}


		private static string BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}
	}
}
