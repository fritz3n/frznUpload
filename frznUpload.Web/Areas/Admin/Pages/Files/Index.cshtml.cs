using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Admin.Pages.Files
{
	public class IndexModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database database;

		public IndexModel(frznUpload.Web.Data.Database context)
		{
			database = context;
		}

		public IList<File> Files { get; set; }

		public string UserName { get; set; }
		public int UserId { get; set; }

		public async Task<IActionResult> OnGetAsync(int? userId)
		{
			if (userId is not null)
			{
				UserName = (await database.Users.FindAsync(userId))?.Name;
				if (UserName is null)
					return NotFound();
				UserId = userId.Value;
				Files = await database.Files.Where(f => f.User.Id == userId).ToListAsync();
				return Page();
			}

			Files = await database.Files.ToListAsync();
			return Page();
		}



		public string BytesToString(long byteCount)
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
