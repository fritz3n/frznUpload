using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Manage
{
	public class ChangePasswordModel : PageModel
	{

		private readonly UserManager userManager;
		private readonly ILogger<ChangePasswordModel> _logger;

		public ChangePasswordModel(
			UserManager userManager,
			ILogger<ChangePasswordModel> logger)
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
			[DataType(DataType.Password)]
			[Display(Name = "Old password")]
			public string Password { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "New password")]
			public string NewPassword { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm new password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmNewPassword { get; set; }

		}

		public void OnGet()
		{
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				if (!await userManager.ChangePassword(HttpContext, Input.Password, Input.NewPassword))
				{
					ModelState.AddModelError(string.Empty, "Could not change Password.");
					return Page();
				}

				return Redirect("/Account/Index");
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
