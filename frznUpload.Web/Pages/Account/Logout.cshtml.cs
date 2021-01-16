using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Account
{
	public class LogoutModel : PageModel
	{
		private readonly UserManager userManager;

		public LogoutModel(UserManager userManager)
		{
			this.userManager = userManager;
		}

		public void OnGet()
		{
			userManager.SignOut(HttpContext);
		}
	}
}
