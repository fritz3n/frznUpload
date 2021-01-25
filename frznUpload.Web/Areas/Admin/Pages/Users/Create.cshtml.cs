using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Admin.Pages.Users
{
	public class CreateModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly UserManager userManager;

		public CreateModel(frznUpload.Web.Data.Database context, UserManager userManager)
		{
			_context = context;
			this.userManager = userManager;
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public UserViewModel Users { get; set; }

		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			User user;

			Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User> entry = _context.Add(user = new User());
			entry.CurrentValues.SetValues(Users);

			user.Salt = userManager.GetSalt();
			user.Hash = userManager.HashPassword(user, Users.Password);

			if (string.IsNullOrWhiteSpace(user.TwoFaSecret))
				user.TwoFaSecret = null;

			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
