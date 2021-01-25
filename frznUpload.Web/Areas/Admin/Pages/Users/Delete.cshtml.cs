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
	public class DeleteModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;

		public DeleteModel(frznUpload.Web.Data.Database context)
		{
			_context = context;
		}

		[BindProperty]
		public User Users { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Users = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

			if (Users == null)
			{
				return NotFound();
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Users = await _context.Users.FindAsync(id);

			if (Users != null)
			{
				_context.Users.Remove(Users);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}
