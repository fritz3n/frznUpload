using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
	public class Share
	{
		public int Id { get; set; }

		public virtual File File { get; set; }
		public string Identifier { get; set; }

		public bool FirstView { get; set; }
		public string FirstViewCookie { get; set; }

		public bool Public { get; set; }
		public bool PublicRegistered { get; set; }

		public bool Whitelisted { get; set; }
		public string WhitelistText { get; set; }

		public DateTime Created { get; set; }
		public DateTime LastAccessed { get; set; }
		public bool DontTrack { get; set; }

		public virtual ICollection<Visit> Visits { get; set; }

		[NotMapped]
		public IEnumerable<string> Whitelist
		{
			get
			{
				if (string.IsNullOrWhiteSpace(WhitelistText))
					return Enumerable.Empty<string>();
				return WhitelistText.Split(',');
			}

			set => WhitelistText = string.Join(',', value);
		}
	}
}
