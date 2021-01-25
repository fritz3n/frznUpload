using frznUpload.Web.Data;
using frznUpload.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Admin.Pages.Files
{
	public class ViewModel : PageModel
	{

		public ViewModel(UserManager userManager, Database database)
		{
			this.userManager = userManager;
			this.database = database;
		}

		public FileViewInfo FileViewInfo { get; set; }
		public string FileIdentifier { get; set; }
		public int FileId { get; set; }
		private readonly UserManager userManager;
		private readonly Database database;

		public async Task<IActionResult> OnGet(string fileId)
		{
			Models.File file = await database.Files.FirstOrDefaultAsync(f => f.Identifier == fileId);
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
