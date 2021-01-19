using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Files
{
	public class CreateModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;

		public CreateModel(frznUpload.Web.Data.Database context)
		{
			_context = context;
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public File File { get; set; }

		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync()
		{
			return NotFound();

			if (!ModelState.IsValid)
			{
				return Page();
			}

			_context.Files.Add(File);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
