
using frznUpload.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Areas.Admin.Pages.Users
{
	public class UserViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string TwoFaSecret { get; set; }
		public UserRole Role { get; set; }
	}
}
