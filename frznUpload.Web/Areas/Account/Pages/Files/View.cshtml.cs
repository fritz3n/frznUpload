using frznUpload.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Files
{
	public class ViewModel : PageModel
	{

		public ViewModel(UserManager userManager)
		{
			this.userManager = userManager;
		}

		public FileViewInfo FileViewInfo { get; set; }
		public string FileIdentifier { get; set; }
		public int FileId { get; set; }
		private readonly UserManager userManager;

		public IActionResult OnGet(string fileId)
		{

			Models.File file = userManager.GetUser(HttpContext).Files.FirstOrDefault(f => f.Identifier == fileId);
			if (file is null)
				return NotFound();
			FileIdentifier = fileId;
			FileId = file.Id;

			FileViewInfo = new FileViewInfo()
			{
				FileName = file.Filename + "." + file.Extension,
				SourcePath = "/Account/Files/d/" + fileId
			};

			return Page();
		}
	}
}
