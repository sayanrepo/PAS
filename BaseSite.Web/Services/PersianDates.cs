using System.Globalization;

namespace BaseSite.Web.Services;

public static class PersianDates
{
    public static string Format(DateTime? date)
    {
        if (!date.HasValue) return "—";
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(date.Value):0000}/{calendar.GetMonth(date.Value):00}/{calendar.GetDayOfMonth(date.Value):00}";
    }

    public static DateTime? Parse(string? text, string label)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var normalized = new string(text.Trim().Select(c => c is >= '۰' and <= '۹' ? (char)('0' + c - '۰') : c is >= '٠' and <= '٩' ? (char)('0' + c - '٠') : c).ToArray());
        var parts = normalized.Replace('-', '/').Split('/');
        if (parts.Length == 3 && int.TryParse(parts[0], out var year) && int.TryParse(parts[1], out var month) && int.TryParse(parts[2], out var day))
        {
            try { return new PersianCalendar().ToDateTime(year, month, day, 0, 0, 0, 0); }
            catch (ArgumentOutOfRangeException) { }
        }
        throw new InvalidOperationException($"{label} معتبر نیست. تاریخ شمسی را مانند ۱۴۰۵/۰۶/۱۷ وارد کنید.");
    }
}
