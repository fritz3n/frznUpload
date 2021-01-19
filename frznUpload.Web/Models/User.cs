using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
	public class User : IEquatable<User>
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		public string Salt { get; set; }
		public string TwoFaSecret { get; set; }

		public virtual ICollection<Token> Tokens { get; set; }
		public virtual ICollection<File> Files { get; set; }

		public IList<Share> GetShares()
		{
			List<Share> shares = new();
			foreach (File file in Files)
				shares.AddRange(file.Shares);
			return shares;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as User);
		}

		public bool Equals(User other)
		{
			return other != null &&
				   Id == other.Id &&
				   Name == other.Name;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, Name);
		}
	}
}
