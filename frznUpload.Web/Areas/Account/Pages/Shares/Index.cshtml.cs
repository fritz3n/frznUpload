using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Shares
{
	public class IndexModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly UserManager userManager;

		public string FileId { get; set; }

		public IndexModel(frznUpload.Web.Data.Database context, UserManager userManager)
		{
			_context = context;
			this.userManager = userManager;
		}

		public IList<Share> Share { get; set; }

		public IActionResult OnGetAsync(string fileId)
		{
			if (fileId is null)
			{
				Share = userManager.GetUser(HttpContext).GetShares();
			}
			else
			{
				File file = userManager.GetUser(HttpContext).Files.FirstOrDefault(f => f.Identifier == fileId);
				if (file is null)
					return NotFound();
				FileId = file.Identifier;
				Share = file.Shares.ToList();
			}
			return Page();
		}
	}
}
