using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Admin.Pages.Users
{
	public class EditModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly UserManager userManager;

		public EditModel(frznUpload.Web.Data.Database context, UserManager userManager)
		{
			_context = context;
			this.userManager = userManager;
		}

		[BindProperty]
		public UserViewModel Users { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			User user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			Users = new UserViewModel()
			{
				Id = user.Id,
				Name = user.Name,
				TwoFaSecret = user.TwoFaSecret,
				Role = user.Role
			};

			return Page();
		}

		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see https://aka.ms/RazorPagesCRUD.
		public async Task<IActionResult> OnPostAsync(int id)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			User user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			user.Name = Users.Name;
			if (string.IsNullOrWhiteSpace(Users.TwoFaSecret))
				user.TwoFaSecret = null;
			else
				user.TwoFaSecret = Users.TwoFaSecret;
			user.Role = Users.Role;

			if (!string.IsNullOrWhiteSpace(Users.Password))
			{
				user.Salt = userManager.GetSalt();
				user.Hash = userManager.HashPassword(user, Users.Password);
			}

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(Users.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return RedirectToPage("./Index");
		}

		private bool UserExists(int id)
		{
			return _context.Users.Any(e => e.Id == id);
		}
	}
}
