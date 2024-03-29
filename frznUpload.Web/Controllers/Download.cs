﻿using frznUpload.Web.Data;
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

		public async Task<IActionResult> DownloadShare(string shareId, bool show = false)
		{
			if (string.IsNullOrWhiteSpace(shareId))
				return NotFound();

			(ShareHelper.AccessStatus allowed, Share share) = await ShareHelper.CanAccess(HttpContext, database, shareId);
			if (allowed == ShareHelper.AccessStatus.Denied)
				return NotFound();

			share.LastAccessed = DateTime.Now;

			string path = Path.Combine(config.GetValue<string>("FileDirectory"), share.File.Identifier + ".file");

			FileStream file = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			string filename = share.File.Filename + "." + share.File.Extension;

			new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contentType);

			// Returns null on unkown file ending
			if (contentType == null)
				contentType = "application/octet-stream";

			HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store";
			HttpContext.Response.Headers["Expires"] = "-1";

			if(show)
				return File(file, contentType);
			else
				return File(file, contentType, filename, true);
		}

		public IActionResult DownloadFile(string fileId, bool show = false)
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

			// Returns null on unkown file ending
			if (contentType == null)
				contentType = "application/octet-stream";

			HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store";
			HttpContext.Response.Headers["Expires"] = "-1";

			if (show)
				return File(fileStream, contentType);
			else
				return File(fileStream, contentType, filename, true);
		}
	}
}
