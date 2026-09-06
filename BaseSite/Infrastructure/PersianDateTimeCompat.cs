using System.Globalization;

public static class PersianDateTimeFormat
{
    public const string Date = "yyyy/MM/dd";
    public const string DateTime = "yyyy/MM/dd HH:mm:ss";
    public const string DateShortTime = DateTime;
}

public sealed class PersianDateTime
{
    private static readonly PersianCalendar Calendar = new();
    private readonly DateTime value;

    public PersianDateTime(DateTime value) => this.value = value;
    public DateTime FirstDayOfYear => Calendar.ToDateTime(Calendar.GetYear(value), 1, 1, 0, 0, 0, 0);
    public DateTime LastDayOfYear => Calendar.ToDateTime(Calendar.GetYear(value), 12, Calendar.GetDaysInMonth(Calendar.GetYear(value), 12), 23, 59, 59, 999);
    public static PersianDateTime Parse(string value) => new(Calendar.ToDateTime(int.Parse(value[..4]), int.Parse(value.Substring(5, 2)), int.Parse(value.Substring(8, 2)), 0, 0, 0, 0));
    public DateTime ToDateTime() => value;
    public override string ToString() => ToString(PersianDateTimeFormat.Date);
    public string ToString(string format) => format == PersianDateTimeFormat.Date ? $"{Calendar.GetYear(value):0000}/{Calendar.GetMonth(value):00}/{Calendar.GetDayOfMonth(value):00}" : value.ToString(format, CultureInfo.InvariantCulture);
}
