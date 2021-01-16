using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Shared
{
	public static class FileViewHelper
	{
		public static Task<IHtmlContent> PartialFileView(this IHtmlHelper html, string filename, string sourcePath)
		{
			new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contentType);
			string[] mimeType = (contentType ?? "application/octet-stream").Split('/');
			var info = new FileViewInfo()
			{
				FileName = filename,
				SourcePath = sourcePath
			};

			switch (mimeType[0])
			{
				case "image":
					return html.PartialAsync("_ImageView", info);
			}

			return html.PartialAsync("_DownloadView", info);
		}
	}

	class FileViewInfo
	{
		public string FileName { get; set; }
		public string SourcePath
		{
			get; set;
		}
	}
}
