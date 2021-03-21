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
	public class DetailsModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;

		public DetailsModel(frznUpload.Web.Data.Database context)
		{
			_context = context;
		}

		public new File File { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			File = await _context.Files.FirstOrDefaultAsync(m => m.Id == id);

			if (File == null)
			{
				return NotFound();
			}
			return Page();
		}
	}
}
