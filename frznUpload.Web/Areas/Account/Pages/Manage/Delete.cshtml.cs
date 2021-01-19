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
	public class DeleteModel : PageModel
	{
		private readonly UserManager userManager;
		private readonly ILogger<DeleteModel> _logger;

		public DeleteModel(
			UserManager userManager,
			ILogger<DeleteModel> logger)
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
			[Display(Name = "Im Sure")]
			public bool Sure { get; set; }
		}

		public void OnGet()
		{
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid && Input.Sure)
			{
				if (!await userManager.DeleteUser(HttpContext))
				{
					ModelState.AddModelError(string.Empty, "Could not delete User.");
					return Page();
				}

				return Redirect("/Account/Index");
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
