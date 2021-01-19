using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
	public class File
	{
		public int Id { get; set; }
		public virtual User User { get; set; }
		public string Identifier { get; set; }
		public string Filename { get; set; }
		public string Extension { get; set; }
		public int Size { get; set; }
		public string Path { get; set; } // For future use

		public virtual ICollection<Share> Shares { get; set; }
	}
}
