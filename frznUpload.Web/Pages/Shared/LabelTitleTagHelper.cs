using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting &lt;label&gt; elements with an <c>asp-for</c> attribute.
/// Adds a <c>title</c> attribute to the &lt;label&gt; with the Description property from the model data annotation DisplayAttribute.
/// </summary>
[HtmlTargetElement("i", Attributes = ForAttributeName)]
public class LabelTitleTagHelper : TagHelper
{
	private const string ForAttributeName = "asp-tooltip-for";

	/// <summary>
	/// Creates a new <see cref="LabelTitleTagHelper"/>.
	/// </summary>
	/// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
	public LabelTitleTagHelper(IHtmlGenerator generator)
	{
		Generator = generator;
	}

	/// <inheritdoc />
	public override int Order
	{
		get
		{
			return -1000;
		}
	}

	[HtmlAttributeNotBound]
	[ViewContext]
	public ViewContext ViewContext { get; set; }

	protected IHtmlGenerator Generator { get; }

	/// <summary>
	/// An expression to be evaluated against the current model.
	/// </summary>
	[HtmlAttributeName(ForAttributeName)]
	public ModelExpression TitleFor { get; set; }

	/// <inheritdoc />
	/// <remarks>Does nothing if <see cref="TitleFor"/> is <c>null</c>.</remarks>
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		if (output == null)
		{
			throw new ArgumentNullException(nameof(output));
		}

		Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = TitleFor.Metadata;

		if (metadata == null)
		{
			throw new InvalidOperationException(string.Format("No provided metadata ({0})", ForAttributeName));
		}

		if (!string.IsNullOrWhiteSpace(metadata.Description))
		{
			output.Attributes.SetAttribute("title", metadata.Description);
			output.Attributes.SetAttribute("data-toggle", "tooltip");
			output.Attributes.SetAttribute("data-placement", "bottom");
		}
	}
}