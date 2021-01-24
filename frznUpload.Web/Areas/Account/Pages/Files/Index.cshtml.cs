using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Account.Pages.Files
{
	public class IndexModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database database;
		private readonly UserManager userManager;

		public IndexModel(frznUpload.Web.Data.Database context, UserManager userManager)
		{
			database = context;
			this.userManager = userManager;
		}

		public IList<File> Files { get; set; }
		public IList<Directory> Directories { get; set; } = new List<Directory>();

		public string Path { get; set; }

		public record Directory(string Name, string Path);

		public string BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}

		public async Task OnGetAsync(string path = null)
		{
			Path = path ?? "/";

			User user = userManager.GetUser(HttpContext, database);

			if (Path == "*")
			{
				Files = user.Files.ToList();
				return;
			}

			if (!Path.StartsWith('/'))
				Path = '/' + Path;
			if (!Path.EndsWith('/'))
				Path += '/';

			if (Path == "/") // special clause to ensure that files without a path are displayed in the root
			{
				Files = await database.Files.Where(f => f.User == user && (f.Path == "" || f.Path == "/")).ToListAsync();
			}
			else
			{
				string unnormailzedPath = Path.Substring(0, Path.Length - 1);
				Files = await database.Files.Where(f => f.User == user && (f.Path == Path || f.Path == unnormailzedPath)).ToListAsync();
				int parentDirLength = unnormailzedPath.LastIndexOf('/') + 1;
				Directories.Add(new Directory("..", Path.Substring(0, parentDirLength)));
			}

			List<string> rawDirectories = await database.Files
				.Where(f => f.User == user && f.Path != Path && f.Path.StartsWith(Path))
				.Select(f => f.Path)
				.Distinct().ToListAsync();

			int maxSubDirs = Path.Count(c => c == '/') + 1;

			foreach (string directory in rawDirectories)
			{
				if (directory.Count(c => c == '/') > maxSubDirs)
					continue;

				string name = directory.Substring(Path.Length, directory.Length - Path.Length);
				Directories.Add(new Directory(name, directory));
			}
		}
	}
}
