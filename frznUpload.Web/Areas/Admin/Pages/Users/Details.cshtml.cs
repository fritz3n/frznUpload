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
	public class DetailsModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;

		public DetailsModel(frznUpload.Web.Data.Database context)
		{
			_context = context;
		}

		public User Users { get; set; }
		public int FileCount { get; set; }
		public string TotalSize { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Users = await _context.Users.FindAsync(id);
			FileCount = Users.Files.Count;
			TotalSize = BytesToString(Users.Files.Sum(f => f.Size));

			if (Users == null)
			{
				return NotFound();
			}
			return Page();
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
