using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		public string Salt { get; set; }
		public string TwoFaSecret { get; set; }

		public virtual ICollection<Token> Tokens { get; set; }
		public virtual ICollection<File> Files { get; set; }
	}
}
