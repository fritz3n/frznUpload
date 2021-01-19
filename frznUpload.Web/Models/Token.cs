using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
	public class Token
	{
		public int Id { get; set; }
		public virtual User User { get; set; }
		public string Serial { get; set; }
		public string Name { get; set; }

		public DateTime Created { get; set; }
		public DateTime LastUsed { get; set; }
		public DateTime ValidUntil { get; set; }
	}
}
