using frznUpload.Web.Data;
using frznUpload.Web.Files;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Shares
{
	public class CreateModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly UserManager userManager;
		private readonly FileManager fileManager;

		public CreateModel(frznUpload.Web.Data.Database context, UserManager userManager, FileManager fileManager)
		{
			_context = context;
			this.userManager = userManager;
			this.fileManager = fileManager;
		}

		public string FileId { get; set; }

		public IActionResult OnGet(string fileId)
		{
			if (string.IsNullOrWhiteSpace(fileId))
				return NotFound();

			FileId = fileId;

			return Page();
		}

		[BindProperty]
		public Share Share { get; set; }

		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync(string fileId)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}


			File file = userManager.GetUser(HttpContext, _context).Files.FirstOrDefault(m => m.Identifier == fileId);
			if (file is null)
				return NotFound();
			Share.File = file;
			Share.Identifier = fileManager.GetAvailableShareIdentifier();
			Share.Created = DateTime.Now;

			_context.Shares.Add(Share);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
