using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Models
{
	[Flags]
	public enum UserRole
	{
		User,
		Admin,
		DontTrack
	}
}
