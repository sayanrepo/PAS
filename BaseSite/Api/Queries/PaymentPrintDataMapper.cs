#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;
using System.Globalization;

namespace BaseSite.Api.Queries;

public static class PaymentPrintDataMapper
{
    public static PaymentPrintData Map(Payment_Payment payment, string printerName) => new()
    {
        DocNumber = payment.DocNumber,
        Amount = payment.Amount,
        PrintedAt = FormatPrintedAt(DateTime.Now),
        PaymentTypeName = payment.Payment_Types?.Name ?? "",
        ReferenceNumber = payment.ShomareSanad ?? "",
        DueDate = payment.ShDateSarresid ?? "",
        BankName = payment.Payment_Banks?.Name ?? "",
        BankBranchCode = payment.BankBranchCode ?? "",
        AccountNumber = payment.ShomareHesab ?? "",
        Comment = payment.Comment ?? "",
        CustomerName = payment.Account_Users?.FullName ?? "",
        PrinterName = printerName
    };

    private static string FormatPrintedAt(DateTime value)
    {
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(value):0000}/{calendar.GetMonth(value):00}/{calendar.GetDayOfMonth(value):00} {value:HH:mm:ss}";
    }
}
