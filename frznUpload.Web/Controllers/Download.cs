using frznUpload.Web.Data;
using frznUpload.Web.Models;
using frznUpload.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages
{
	public class Download : Controller
	{
		private readonly UserManager userManager;
		private readonly Database database;
		private readonly IConfiguration config;

		public Download(UserManager userManager, Database database, IConfiguration config)
		{
			this.userManager = userManager;
			this.database = database;
			this.config = config;
		}

		public IActionResult DownloadShare(string shareId)
		{
			if (string.IsNullOrWhiteSpace(shareId))
				return NotFound();

			ShareHelper.AccessStatus allowed = ShareHelper.CanAccess(HttpContext, database, shareId, out Share share);
			if (allowed == ShareHelper.AccessStatus.Denied)
				return NotFound();

			share.LastAccessed = DateTime.Now;

			string path = Path.Combine(config.GetValue<string>("FileDirectory"), share.File.Identifier + ".file");

			FileStream file = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			string filename = share.File.Filename + "." + share.File.Extension;

			new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contentType);

			return File(file, contentType, filename, true);
		}

		public IActionResult DownloadFile(string fileId)
		{
			Models.File file;
			if (HttpContext.User.IsInRole("Admin"))
				file = database.Files.FirstOrDefault(f => f.Identifier == fileId);
			else
				file = userManager.GetUser(HttpContext).Files.FirstOrDefault(f => f.Identifier == fileId);

			if (file is null)
				return NotFound();

			string path = Path.Combine(config.GetValue<string>("FileDirectory"), file.Identifier + ".file");

			FileStream fileStream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			string filename = file.Filename + "." + file.Extension;

			new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contentType);

			return File(fileStream, contentType, filename, true);
		}
	}
}
