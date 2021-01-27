using frznUpload.Web.Data;
using frznUpload.Web.Files;
using frznUpload.Web.Models;
using frznUpload.Web.Server;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace frznUpload.Web.Pages
{
	public class UploadModel : PageModel
	{
		private readonly UserManager userManager;
		private readonly Database database;
		private readonly FileManager fileManager;

		public UploadModel(UserManager userManager, Database database, FileManager fileManager)
		{
			this.userManager = userManager;
			this.database = database;
			this.fileManager = fileManager;
		}

		public IActionResult OnGet()
		{
			User user = userManager.GetUser(HttpContext, database);
			if (user is null)
			{
				ModelState.AddModelError("", "Please login first");
				return Redirect("/Login/Login");
			}
			return Page();
		}


		[BindProperty]
		public ShareModel Share { get; set; }

		public class ShareModel
		{
			[Display(Name = "Public", Description = "Can people who are not registered view this share?")]
			public bool Public { get; set; }

			[Display(Name = "Public for users", Description = "Can people who are registered view this share?")]
			public bool PublicRegistered { get; set; }

			[Display(Name = "Enable FirstView", Description = "If this is enabled only you and the person who first accessed the share can see this share.")]
			public bool FirstView { get; set; }

			[Display(Name = "Enable Whitelist", Description = "If this is enabled you can specify the users which can access this share by their username.")]
			public bool Whitelisted { get; set; }


			[Display(Name = "Whitelist", Description = "The usernames of allowed people, seperated by semicolons (;)")]
			public string Whitelist { get; set; }
			public string FileId { get; set; }
		}


		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				string[] whitelist = (Share.Whitelist ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries);
				string identifier = await fileManager.CreateShare(Share.FileId, Share.FirstView, Share.Public, Share.PublicRegistered, Share.Whitelisted, whitelist);

				return Content("/v/" + identifier);
			}

			// If we got this far, something failed, redisplay form
			return StatusCode(400);
		}

		public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
		{
			if (file is null)
				return StatusCode(400);

			User user = userManager.GetUser(HttpContext, database);
			if (user is null)
				return Unauthorized();

			System.IO.FileStream stream = await fileManager.CreateFile(user, (int)file.Length, file.FileName, "/Web", out string identifier);

			await file.CopyToAsync(stream);
			await stream.FlushAsync();
			stream.Close();

			string response = identifier + ";" + Path.GetFileName(file.FileName).Replace(";", "");

			return Content(response);
		}

		public async Task<IActionResult> OnPostRenameAsync(string identifier = null, string newName = null)
		{
			if (identifier is null || newName is null)
				return StatusCode(400);

			User user = userManager.GetUser(HttpContext, database);
			if (user is null)
				return Unauthorized();

			Models.File file = user.Files.FirstOrDefault(f => f.Identifier == identifier);
			if (file is null)
				return NotFound();


			if (newName.Contains('.'))
			{
				int index = newName.LastIndexOf('.');
				file.Filename = newName.Substring(0, index);
				file.Extension = newName.Substring(index + 1, newName.Length - index - 1);
			}
			else
			{
				file.Filename = newName;
				newName += "." + file.Extension;
			}

			await database.SaveChangesAsync();

			return Content(newName);
		}
	}
}
