#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class PaymentEditorData
{
    public static PaymentForm Map(Payment_Payment payment) => new()
    {
        CustomerId = payment.CustomerId,
        PaymentTypeId = payment.PaymentTypeId,
        BabatId = payment.PaymentBabatId,
        BankId = payment.BankId,
        DocumentDate = payment.DateSanad,
        DueDate = payment.DateSarresid,
        Amount = payment.Amount,
        ProjectName = payment.ProjectName,
        BankBranchCode = payment.BankBranchCode,
        ReferenceNumber = payment.ShomareSanad,
        AccountNumber = payment.ShomareHesab,
        Comment = payment.Comment,
        Returned = payment.Bargashti,
        StatusId = payment.StatusId
    };

    public static PaymentDetail Detail(Payment_Payment payment) => new()
    {
        Summary = PaymentQueries.Project(new[] { payment }.AsQueryable()).Single(),
        ProjectName = payment.ProjectName ?? "",
        BankBranchCode = payment.BankBranchCode ?? "",
        ReferenceNumber = payment.ShomareSanad ?? "",
        AccountNumber = payment.ShomareHesab ?? "",
        Comment = payment.Comment ?? "",
        Returned = payment.Bargashti
    };

    public static string? Validate(PantaEntities db, PaymentForm form)
    {
        if (!db.Account_Users.Any(x => x.Id == form.CustomerId && x.Id > 0)) return "مشتری معتبر انتخاب کنید.";
        if (!form.PaymentTypeId.HasValue || !db.Payment_Types.Any(x => x.Id == form.PaymentTypeId.Value)) return "نحوه وصول معتبر انتخاب کنید.";
        if (!form.BabatId.HasValue || !db.Payment_Babats.Any(x => x.Id == form.BabatId.Value)) return "بابت معتبر انتخاب کنید.";
        if (form.BankId.HasValue && !db.Payment_Banks.Any(x => x.Id == form.BankId.Value)) return "بانک معتبر انتخاب کنید.";
        if (!form.DocumentDate.HasValue) return "تاریخ سند را وارد کنید.";
        if (!form.DueDate.HasValue) return "تاریخ سررسید را وارد کنید.";
        if (form.Amount <= 0) return "مبلغ باید بزرگ‌تر از صفر باشد.";
        return null;
    }

    public static void Apply(Payment_Payment payment, PaymentForm form)
    {
        payment.CustomerId = form.CustomerId;
        payment.PaymentTypeId = form.PaymentTypeId!.Value;
        payment.PaymentBabatId = form.BabatId!.Value;
        payment.BankId = form.BankId;
        payment.DateSanad = form.DocumentDate;
        payment.DateSarresid = form.DueDate;
        payment.Amount = form.Amount;
        payment.ProjectName = form.ProjectName?.Trim();
        payment.BankBranchCode = form.BankBranchCode?.Trim();
        payment.ShomareSanad = form.ReferenceNumber?.Trim();
        payment.ShomareHesab = form.AccountNumber?.Trim();
        payment.Comment = form.Comment?.Trim();
        payment.Bargashti = form.Returned;
    }
}
