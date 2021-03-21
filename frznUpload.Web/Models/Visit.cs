using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using static frznUpload.Web.Pages.Shared.ShareHelper;

namespace frznUpload.Web.Models
{
	public class Visit
	{
		public int Id { get; set; }
		public virtual Share Share { get; set; }
		public virtual User User { get; set; }
		public DateTime Date { get; set; }
		public string IP { get; set; }
		public string UserAgent { get; set; }

		public AccessStatus Access { get; set; }

	}
}
