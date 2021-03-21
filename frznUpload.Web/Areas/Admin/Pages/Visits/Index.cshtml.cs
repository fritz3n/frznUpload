using frznUpload.Web.Data;
using frznUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAParser;
using static frznUpload.Web.Pages.Shared.ShareHelper;

namespace frznUpload.Web.Areas.Admin.Pages.Visits
{
	public class IndexModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database database;

		public IndexModel(frznUpload.Web.Data.Database context)
		{
			database = context;
		}

		public IList<VisitView> Visits { get; set; }

		public string UserName { get; set; }
		public int UserId { get; set; }
		public string FileName { get; set; }
		public int FileId { get; set; }
		public int ShareId { get; set; }
		public string ShareIdentifier { get; set; }

		public async Task<IActionResult> OnGetAsync(int? shareId)
		{
			if (shareId is not null)
			{
				Share share = await database.Shares.FindAsync(shareId);
				if (share is null)
					return NotFound();

				ShareId = share.Id;
				ShareIdentifier = share.Identifier;
				FileId = share.File.Id;
				FileName = share.File.Filename + "." + share.File.Extension;
				UserId = share.File.User.Id;
				UserName = share.File.User.Name;

				SetModels(await database.Visits.Where(f => f.Share.Id == shareId).ToListAsync());
				return Page();
			}

			SetModels(await database.Visits.ToListAsync());
			return Page();
		}

		public void SetModels(IList<Visit> visits)
		{
			var uaParser = Parser.GetDefault();
			Visits = new List<VisitView>(visits.Count);

			foreach (Visit visit in visits)
			{
				ClientInfo client = visit.UserAgent == null ? null : uaParser.Parse(visit.UserAgent);

				Visits.Add(new()
				{
					Id = visit.Id,
					Share = visit.Share,
					User = visit.User,
					Date = visit.Date,
					IP = visit.IP,
					UserAgent = visit.UserAgent,
					OS = client?.OS.ToString() ?? "unknown",
					Access = visit.Access
				});
			}

		}

		public class VisitView
		{
			public int Id { get; set; }
			public Share Share { get; set; }
			public User User { get; set; }
			public DateTime Date { get; set; }
			public string IP { get; set; }
			public string UserAgent { get; set; }

			public string OS { get; set; }

			public AccessStatus Access { get; set; }
		}


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
	}
}
