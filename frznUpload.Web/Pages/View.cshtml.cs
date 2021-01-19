using frznUpload.Web.Data;
using frznUpload.Web.Models;
using frznUpload.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages
{
	public class ViewModel : PageModel
	{
		private readonly Database database;

		public ViewModel(Database database)
		{
			this.database = database;
		}

		public FileViewInfo FileViewInfo { get; private set; }
		public string ShareName { get; set; }
		public int FileId { get; set; }
		public int ShareId { get; set; }
		public bool IsOwner
		{
			get; private set;
		}

		public IActionResult OnGet(string shareId)
		{
			if (string.IsNullOrWhiteSpace(shareId))
				return NotFound();

			ShareName = shareId;

			ShareHelper.AccessStatus allowed = ShareHelper.CanAccess(HttpContext, database, shareId, out Share share);
			if (allowed == ShareHelper.AccessStatus.Denied)
				return NotFound();

			IsOwner = allowed == ShareHelper.AccessStatus.Owner;

			FileId = share.File.Id;
			ShareId = share.Id;

			FileViewInfo = new FileViewInfo()
			{
				FileName = share.File.Filename + "." + share.File.Extension,
				SourcePath = "/d/" + shareId
			};
			return Page();
		}
	}
}
