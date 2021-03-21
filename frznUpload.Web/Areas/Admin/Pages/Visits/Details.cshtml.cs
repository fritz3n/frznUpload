using frznUpload.Web.Data;
using frznUpload.Web.Models;
using IpInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UAParser;
using static frznUpload.Web.Pages.Shared.ShareHelper;

namespace frznUpload.Web.Areas.Admin.Pages.Visits
{
	public class DetailsModel : PageModel
	{
		private readonly frznUpload.Web.Data.Database _context;
		private readonly IHttpClientFactory clientFactory;
		private readonly IConfiguration configuration;

		public DetailsModel(Database context, IHttpClientFactory clientFactory, IConfiguration configuration)
		{
			_context = context;
			this.clientFactory = clientFactory;
			this.configuration = configuration;
		}

		public new VisitView Visit { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{

			if (id == null)
			{
				return NotFound();
			}


			Visit visit = await _context.Visits.FirstOrDefaultAsync(m => m.Id == id);

			if (visit == null)
			{
				return NotFound();
			}


			var uaParser = Parser.GetDefault();
			ClientInfo client = uaParser.Parse(visit.UserAgent);

			var api = new IpInfoApi(configuration.GetValue<string>("IpinfoToken"), clientFactory.CreateClient());
			FullResponse response = await api.GetInformationByIpAsync(visit.IP);

			Visit = new()
			{
				Id = visit.Id,
				Share = visit.Share,
				User = visit.User,
				Date = visit.Date,
				IP = visit.IP,
				UserAgent = visit.UserAgent,
				ClientInfo = client,
				IpInfo = response,
				Access = visit.Access
			};

			return Page();
		}


		public class VisitView
		{
			public int Id { get; set; }
			public Share Share { get; set; }
			public User User { get; set; }
			public DateTime Date { get; set; }
			public string IP { get; set; }
			public string UserAgent { get; set; }

			public ClientInfo ClientInfo { get; set; }
			public FullResponse IpInfo { get; set; }

			public AccessStatus Access { get; set; }
		}
	}
}
