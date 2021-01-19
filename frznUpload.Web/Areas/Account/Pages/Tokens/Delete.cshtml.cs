using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Tokens
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
		public Token Token { get; set; }

		public IActionResult OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Token = userManager.GetUser(HttpContext).Tokens.FirstOrDefault(m => m.Id == id);

			if (Token == null)
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

			Token = userManager.GetUser(HttpContext, _context).Tokens.FirstOrDefault(t => t.Id == id);

			if (Token != null)
			{
				_context.Tokens.Remove(Token);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}
