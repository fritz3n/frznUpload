using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly UserManager userManager;
		private readonly ILogger<RegisterModel> _logger;

		public RegisterModel(
			UserManager userManager,
			ILogger<RegisterModel> logger)
		{
			this.userManager = userManager;
			_logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

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

		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("/Login");
			if (ModelState.IsValid)
			{
				await userManager.RegisterUser(Input.Name, Input.Password);

				RedirectToPage(returnUrl);
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
