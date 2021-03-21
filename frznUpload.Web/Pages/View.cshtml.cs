using frznUpload.Web.Data;
using frznUpload.Web.Models;
using frznUpload.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages
{
	public class ViewModel : PageModel
	{
		private readonly Database database;
		private readonly UserManager userManager;

		public ViewModel(Database database, UserManager userManager)
		{
			this.database = database;
			this.userManager = userManager;
		}

		public FileViewInfo FileViewInfo { get; private set; }
		public string ShareName { get; set; }
		public int FileId { get; set; }
		public int ShareId { get; set; }
		public bool IsOwner
		{
			get; private set;
		}

		public async Task<IActionResult> OnGetAsync(string shareId)
		{
			if (string.IsNullOrWhiteSpace(shareId))
				return NotFound();

			ShareName = shareId;

			(ShareHelper.AccessStatus allowed, Share share) = await ShareHelper.CanAccess(HttpContext, database, shareId);

			if (share != null && !share.DontTrack)
			{
				database.Visits.Add(new()
				{
					Access = allowed,
					Date = DateTime.Now,
					IP = Request.Headers["X-Forwarded-For"],
					UserAgent = Request.Headers["User-Agent"],
					Share = share,
					User = userManager.GetUser(HttpContext)
				});
				await database.SaveChangesAsync();
			}

			if (allowed == ShareHelper.AccessStatus.Denied)
			{
				await database.SaveChangesAsync();
				return NotFound();
			}

			share.LastAccessed = DateTime.Now;

			await database.SaveChangesAsync();

			IsOwner = allowed == ShareHelper.AccessStatus.Owner;

			FileId = share.File.Id;
			ShareId = share.Id;

			FileViewInfo = new FileViewInfo()
			{
				FileName = share.File.Filename + "." + share.File.Extension,
				SourcePath = "/d/" + shareId
			};
			return Page();
		}
	}
}
