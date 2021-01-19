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
		public static Task<IHtmlContent> PartialFileView(this IHtmlHelper html, FileViewInfo info)
		{
			new FileExtensionContentTypeProvider().TryGetContentType(info.FileName, out string contentType);
			contentType ??= "application/octet-stream";
			string[] mimeType = contentType.Split('/');

			info.MimeType = contentType;

			foreach (KeyValuePair<string, string[][]> partial in FileViewMap.Map)
			{
				foreach (string[] match in partial.Value)
				{
					if (Matches(mimeType, match))
						return html.PartialAsync(partial.Key, info);
				}
			}

			return html.PartialAsync("/Pages/Shared/FileViews/_DownloadView.cshtml", info);
		}

		private static bool Matches(string[] target, string[] match)
		{
			if (target[0] != match[0])
				return false;
			if (match.Length == 1)
				return true;
			for (int i = 1; i < match.Length; i++)
			{
				if (target[1] == match[i])
					return true;
			}
			return false;
		}

		public static Task<IHtmlContent> PartialFileView(this IHtmlHelper html, string filename, string sourcePath)
		{
			return html.PartialFileView(new FileViewInfo()
			{
				FileName = filename,
				SourcePath = sourcePath
			});
		}
	}

	public class FileViewInfo
	{
		public string FileName { get; set; }
		public string SourcePath { get; set; }

		/// <summary>
		/// Is populated by FileViewHelper
		/// </summary>
		public string MimeType { get; set; }
	}
}
