using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Files
{
	public class DeleteModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly UserManager userManager;

		public DeleteModel(frznUpload.Web.Data.Database context, UserManager userManager)
		{
			_context = context;
			this.userManager = userManager;
		}

		[BindProperty]
		public File File { get; set; }

		public IActionResult OnGet(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			File = userManager.GetUser(HttpContext, _context).Files.FirstOrDefault(m => m.Id == id);

			if (File == null)
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

			File = await _context.Files.FindAsync(id);


			if (userManager.GetUser(HttpContext, _context) != File.User)
				return Forbid();

			if (File != null)
			{
				_context.Files.Remove(File);
				await _context.SaveChangesAsync();
				frznUpload.Web.Server.FileHandler.DeleteFile(File.Identifier);
			}

			return RedirectToPage("./Index");
		}
	}
}
