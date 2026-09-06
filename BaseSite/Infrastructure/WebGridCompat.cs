using System.Collections;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace BaseSite.Infrastructure;

[Flags]
public enum WebGridPagerModes
{
    None = 0,
    NextPrevious = 1,
    Numeric = 2,
    FirstLast = 4,
    All = NextPrevious | Numeric | FirstLast
}

public sealed class WebGridColumn
{
    internal string ColumnName { get; init; }
    internal string Header { get; init; }
    internal Func<dynamic, object> Format { get; init; }
    internal string Style { get; init; }
    internal bool CanSort { get; init; }
}

/// <summary>
/// Compatibility implementation for the System.Web.Helpers WebGrid API removed
/// from ASP.NET Core. It intentionally supports only the API surface used by
/// the migrated views.
/// </summary>
public sealed class WebGrid
{
    private List<object> source = new();

    public WebGrid(IEnumerable source = null, int rowsPerPage = 10)
    {
        RowsPerPage = Math.Max(1, rowsPerPage);
        if (source is not null) Bind(source);
    }

    public int RowsPerPage { get; }
    public int PageIndex { get; private set; }
    public int TotalRowCount { get; private set; }
    public int PageCount => Math.Max(1, (int)Math.Ceiling((double)TotalRowCount / RowsPerPage));
    public IList<dynamic> Rows { get; private set; } = new List<dynamic>();

    public WebGrid Bind(IEnumerable source, IEnumerable<string> columnNames = null, bool autoSortAndPage = true, int rowCount = -1)
    {
        this.source = source?.Cast<object>().ToList() ?? new List<object>();
        TotalRowCount = rowCount >= 0 ? rowCount : this.source.Count;

        var request = new HttpContextAccessor().HttpContext?.Request;
        var pageValue = request?.Query["page"].FirstOrDefault();
        PageIndex = int.TryParse(pageValue, out var page) ? Math.Max(0, page - 1) : 0;
        PageIndex = Math.Min(PageIndex, PageCount - 1);

        IEnumerable<object> rows = this.source;
        var sort = request?.Query["sort"].FirstOrDefault();
        if (autoSortAndPage && !string.IsNullOrWhiteSpace(sort))
        {
            var descending = string.Equals(request.Query["sortdir"].FirstOrDefault(), "DESC", StringComparison.OrdinalIgnoreCase);
            rows = descending
                ? rows.OrderByDescending(item => GetPropertyValue(item, sort), ObjectComparer.Instance)
                : rows.OrderBy(item => GetPropertyValue(item, sort), ObjectComparer.Instance);
        }

        // A controller may already have supplied the requested page. Only page
        // locally when the supplied sequence represents the complete result set.
        if (autoSortAndPage && this.source.Count == TotalRowCount)
            rows = rows.Skip(PageIndex * RowsPerPage).Take(RowsPerPage);

        var wrappedRows = new List<dynamic>();
        foreach (var item in rows)
            wrappedRows.Add(new WebGridRow(item, this));
        Rows = wrappedRows;
        return this;
    }

    public WebGridColumn Column(string columnName = null, string header = null,
        Func<dynamic, object> format = null, string style = null, bool canSort = true) => new()
    {
        ColumnName = columnName,
        Header = header ?? columnName ?? string.Empty,
        Format = format,
        Style = style,
        CanSort = canSort
    };

    public WebGridColumn[] Columns(params WebGridColumn[] columns) => columns;

    public IHtmlContent GetHtml(string tableStyle = null, string headerStyle = null,
        string footerStyle = null, string rowStyle = null, string alternatingRowStyle = null,
        string selectedRowStyle = null, string caption = null, bool displayHeader = true,
        bool fillEmptyRows = false, string emptyRowCellValue = null,
        IEnumerable<WebGridColumn> columns = null, IEnumerable<string> exclusions = null,
        WebGridPagerModes mode = WebGridPagerModes.All, string firstText = null,
        string previousText = null, string nextText = null, string lastText = null,
        int numericLinksCount = 5, object htmlAttributes = null)
    {
        var selectedColumns = columns?.ToList() ?? new List<WebGridColumn>();
        var html = new StringBuilder();
        html.Append("<table");
        AppendAttribute(html, "class", tableStyle);
        AppendAnonymousAttributes(html, htmlAttributes);
        html.Append('>');
        if (!string.IsNullOrEmpty(caption)) html.Append("<caption>").Append(Encode(caption)).Append("</caption>");

        if (displayHeader)
        {
            html.Append("<thead><tr");
            AppendAttribute(html, "class", headerStyle);
            html.Append('>');
            foreach (var column in selectedColumns)
            {
                html.Append("<th");
                AppendAttribute(html, "class", column.Style);
                html.Append('>');
                if (column.CanSort && !string.IsNullOrWhiteSpace(column.ColumnName))
                    html.Append("<a href=\"").Append(Encode(BuildUrl(1, column.ColumnName))).Append("\">").Append(Encode(column.Header)).Append("</a>");
                else
                    html.Append(Encode(column.Header));
                html.Append("</th>");
            }
            html.Append("</tr></thead>");
        }

        html.Append("<tbody>");
        for (var index = 0; index < Rows.Count; index++)
        {
            var css = index % 2 == 1 ? alternatingRowStyle : rowStyle;
            html.Append("<tr");
            AppendAttribute(html, "class", css);
            html.Append('>');
            foreach (var column in selectedColumns)
            {
                html.Append("<td");
                AppendAttribute(html, "class", column.Style);
                html.Append('>');
                var value = column.Format is null
                    ? GetPropertyValue(((WebGridRow)Rows[index]).Value, column.ColumnName)
                    : column.Format(Rows[index]);
                AppendValue(html, value);
                html.Append("</td>");
            }
            html.Append("</tr>");
        }
        html.Append("</tbody>");

        if (PageCount > 1 && mode != WebGridPagerModes.None)
        {
            html.Append("<tfoot><tr><td colspan=\"").Append(Math.Max(1, selectedColumns.Count)).Append("\"");
            AppendAttribute(html, "class", footerStyle);
            html.Append("><div class=\"pagination\">");
            if (mode.HasFlag(WebGridPagerModes.FirstLast) && PageIndex > 0) AppendPageLink(html, 1, firstText ?? "First");
            if (mode.HasFlag(WebGridPagerModes.NextPrevious) && PageIndex > 0) AppendPageLink(html, PageIndex, previousText ?? "Previous");
            if (mode.HasFlag(WebGridPagerModes.Numeric))
            {
                var start = Math.Max(0, PageIndex - numericLinksCount / 2);
                var end = Math.Min(PageCount, start + numericLinksCount);
                for (var i = start; i < end; i++)
                    if (i == PageIndex) html.Append("<span class=\"active\">").Append(i + 1).Append("</span>");
                    else AppendPageLink(html, i + 1, (i + 1).ToString(CultureInfo.InvariantCulture));
            }
            if (mode.HasFlag(WebGridPagerModes.NextPrevious) && PageIndex + 1 < PageCount) AppendPageLink(html, PageIndex + 2, nextText ?? "Next");
            if (mode.HasFlag(WebGridPagerModes.FirstLast) && PageIndex + 1 < PageCount) AppendPageLink(html, PageCount, lastText ?? "Last");
            html.Append("</div></td></tr></tfoot>");
        }

        html.Append("</table>");
        return new HtmlString(html.ToString());
    }

    private void AppendPageLink(StringBuilder html, int page, string text) =>
        html.Append("<a href=\"").Append(Encode(BuildUrl(page))).Append("\">").Append(Encode(text)).Append("</a>");

    private static string BuildUrl(int page, string sort = null)
    {
        var request = new HttpContextAccessor().HttpContext?.Request;
        var values = request?.Query
            .Where(pair => !pair.Key.Equals("page", StringComparison.OrdinalIgnoreCase) &&
                           (sort is null || (!pair.Key.Equals("sort", StringComparison.OrdinalIgnoreCase) && !pair.Key.Equals("sortdir", StringComparison.OrdinalIgnoreCase))))
            .SelectMany(pair => pair.Value.Select(value => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(value)}"))
            .ToList() ?? new List<string>();
        values.Add($"page={page}");
        if (sort is not null)
        {
            var currentSort = request?.Query["sort"].FirstOrDefault();
            var currentDirection = request?.Query["sortdir"].FirstOrDefault();
            var direction = string.Equals(currentSort, sort, StringComparison.OrdinalIgnoreCase) && !string.Equals(currentDirection, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
            values.Add($"sort={Uri.EscapeDataString(sort)}");
            values.Add($"sortdir={direction}");
        }
        return "?" + string.Join("&", values);
    }

    private static object GetPropertyValue(object value, string path)
    {
        if (value is null || string.IsNullOrWhiteSpace(path)) return null;
        foreach (var part in path.Split('.'))
        {
            if (value is null) return null;
            value = value.GetType().GetProperty(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?.GetValue(value);
        }
        return value;
    }

    private static void AppendValue(StringBuilder builder, object value)
    {
        if (value is null) return;
        if (value is IHtmlContent content)
        {
            using var writer = new StringWriter(builder, CultureInfo.InvariantCulture);
            content.WriteTo(writer, HtmlEncoder.Default);
        }
        else builder.Append(Encode(Convert.ToString(value, CultureInfo.CurrentCulture)));
    }

    private static void AppendAttribute(StringBuilder builder, string name, string value)
    {
        if (!string.IsNullOrWhiteSpace(value)) builder.Append(' ').Append(name).Append("=\"").Append(Encode(value)).Append('"');
    }

    private static void AppendAnonymousAttributes(StringBuilder builder, object attributes)
    {
        if (attributes is null) return;
        foreach (var property in attributes.GetType().GetProperties())
            AppendAttribute(builder, property.Name.Replace('_', '-'), Convert.ToString(property.GetValue(attributes), CultureInfo.InvariantCulture));
    }

    private static string Encode(string value) => HtmlEncoder.Default.Encode(value ?? string.Empty);

    private sealed class WebGridRow : DynamicObject
    {
        public WebGridRow(object value, WebGrid webGrid) { Value = value; WebGrid = webGrid; }
        public object Value { get; }
        public WebGrid WebGrid { get; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == nameof(WebGrid)) { result = WebGrid; return true; }
            result = Value?.GetType().GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?.GetValue(Value);
            return true;
        }
    }

    private sealed class ObjectComparer : IComparer<object>
    {
        public static readonly ObjectComparer Instance = new();
        public int Compare(object x, object y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            if (x is IComparable comparable) return comparable.CompareTo(y);
            return StringComparer.CurrentCulture.Compare(x.ToString(), y.ToString());
        }
    }
}
