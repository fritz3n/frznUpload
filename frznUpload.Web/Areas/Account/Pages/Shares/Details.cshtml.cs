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
	public class DetailsModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly UserManager userManager;

		public DetailsModel(frznUpload.Web.Data.Database context, UserManager userManager)
		{
			_context = context;
			this.userManager = userManager;
		}

		public Share Share { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Share = await _context.Shares.FirstOrDefaultAsync(m => m.Id == id);

			if (userManager.GetUser(HttpContext, _context) != Share.File.User)
				return Forbid();

			if (Share == null)
			{
				return NotFound();
			}
			return Page();
		}
	}
}
