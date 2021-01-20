using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Login
{
	public class RegisterModel : PageModel
	{
		private readonly UserManager userManager;

		public RegisterModel(
			UserManager userManager)
		{
			this.userManager = userManager;
		}

		[BindProperty]
		public InputModel Input { get; set; }


		public class InputModel
		{
			[Required]
			[Display(Name = "Username")]
			public string Name { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				await userManager.RegisterUser(Input.Name, Input.Password);

				RedirectToPage(Url.Content("/Login/Login"));
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
