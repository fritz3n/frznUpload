using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frznUpload.Web.Pages.Shared
{
	public static class HtmlPartialExtensions
	{
		public static HtmlString PartialSection(this IHtmlHelper htmlHelper, string type, params Func<object, HelperResult>[] templates)
		{
			foreach (Func<object, HelperResult> template in templates)
				htmlHelper.ViewContext.HttpContext.Items[$"_{type}_" + Guid.NewGuid()] = template;
			return HtmlString.Empty;
		}

		public static HtmlString RenderPartialSection(this IHtmlHelper htmlHelper, string type)
		{
			foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
			{
				if (key.ToString().StartsWith($"_{type}_"))
				{
					var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
					if (template != null)
					{
						htmlHelper.ViewContext.Writer.Write(template(null));
					}
				}
			}
			return HtmlString.Empty;
		}
	}
}
