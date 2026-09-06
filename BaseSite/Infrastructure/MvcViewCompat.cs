using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BaseSite.Infrastructure;

/// <summary>Preserves the MVC 5 parameterless ViewDataDictionary construction used by partial views.</summary>
public class ViewDataDictionary : Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary
{
    public ViewDataDictionary() : base(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { }
}

/// <summary>ASP.NET Core equivalent of the MVC 5 raw HTML wrapper.</summary>
public sealed class MvcHtmlString : IHtmlContent
{
    private readonly string value;
    public MvcHtmlString(string value) => this.value = value ?? string.Empty;
    public void WriteTo(TextWriter writer, HtmlEncoder encoder) => writer.Write(value);
    public override string ToString() => value;
}

/// <summary>The legacy report resources are hosted by the .NET 10 report gateway.</summary>
public static class WebReportGlobals
{
    public static IHtmlContent Scripts() => HtmlString.Empty;
    public static IHtmlContent Styles() => HtmlString.Empty;
}
