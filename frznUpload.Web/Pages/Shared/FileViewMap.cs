using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Shared
{
	public static class FileViewMap
	{
		public static Dictionary<string, string[][]> Map { get; } = new()
		{
			{
				"/Pages/Shared/FileViews/_ImageView.cshtml",
				new[]
				{
					new[]{  "image" }
				}
			},
			{
				"/Pages/Shared/FileViews/_VideoView.cshtml",
				new[]
				{
					new[]{  "video" }
				}
			},
			{
				"/Pages/Shared/FileViews/_AudioView.cshtml",
				new[]
				{
					new[]{  "audio" }
				}
			},
			{
				"/Pages/Shared/FileViews/_TextView.cshtml",
				new[]
				{
					new[]{  "text" },
					new[]{  "application", "json", "xml", "xhtml+xml", "x-httpd-php" }
				}
			}
		};
	}
}
