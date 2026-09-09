#nullable enable

using System.ComponentModel.DataAnnotations;

namespace BaseSite.Api.Contracts;

public sealed class PaymentSearch : IValidatableObject
{
    public int? DocumentNumber { get; set; }
    public string? Customer { get; set; }
    public int? CustomerId { get; set; }
    public byte? PaymentTypeId { get; set; }
    public byte? BabatId { get; set; }
    public byte? StatusId { get; set; }
    public DateTime? DocumentDateFrom { get; set; }
    public DateTime? DocumentDateTo { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    [Range(0, 1000000)] public int Page { get; set; }
    [Range(1, 100)] public int PageSize { get; set; } = 20;
    public bool HasFilters => DocumentNumber.HasValue || CustomerId.HasValue || !string.IsNullOrWhiteSpace(Customer)
        || StatusId.HasValue || PaymentTypeId.HasValue || BabatId.HasValue
        || DocumentDateFrom.HasValue || DocumentDateTo.HasValue || DueDateFrom.HasValue || DueDateTo.HasValue;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DocumentDateFrom?.Date > DocumentDateTo?.Date)
            yield return new ValidationResult("بازه تاریخ سند معتبر نیست.");
        if (DueDateFrom?.Date > DueDateTo?.Date)
            yield return new ValidationResult("بازه تاریخ سررسید معتبر نیست.");
        if (DocumentDateTo?.Date == DateTime.MaxValue.Date || DueDateTo?.Date == DateTime.MaxValue.Date)
            yield return new ValidationResult("تاریخ پایان خارج از محدوده مجاز است.");
    }
}

public class PaymentSummary
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public int DocumentNumber { get; set; }
    public string Receiver { get; set; } = "";
    public string Customer { get; set; } = "";
    public DateTime? DocumentDate { get; set; }
    public DateTime? DueDate { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; } = "";
    public string PaymentType { get; set; } = "";
    public string Babat { get; set; } = "";
    public string Bank { get; set; } = "";

}

public sealed class PaymentPage
{
    public List<PaymentSummary> Items { get; set; } = [];
    public int TotalCount { get; set; }
}



public sealed class PaymentLookups {
 public List<OrderLookup> Statuses { get; set; } = [];
 public List<OrderLookup> Types { get; set; } = [];
 public List<OrderLookup> Babats { get; set; } = [];
 public List<OrderLookup> Banks { get; set; } = [];
}
public sealed class PaymentDetail {
 public PaymentSummary Summary { get; set; } = new();
 public string ProjectName { get; set; } = "";
 public string BankBranchCode { get; set; } = "";
 public string ReferenceNumber { get; set; } = "";
 public string AccountNumber { get; set; } = "";
 public string Comment { get; set; } = "";
 public bool Returned { get; set; }
}
public sealed class NewPaymentRequest {
 [Range(1,int.MaxValue, ErrorMessage="مشتری را انتخاب کنید.")] public int CustomerId { get; set; }
 [Required(ErrorMessage="نحوه وصول را انتخاب کنید.")] public byte? PaymentTypeId { get; set; }
 [Required(ErrorMessage="بابت را انتخاب کنید.")] public byte? BabatId { get; set; }
 public short? BankId { get; set; }
 [Required(ErrorMessage="تاریخ سند را وارد کنید.")] public DateTime? DocumentDate { get; set; }
 [Required(ErrorMessage="تاریخ سررسید را وارد کنید.")] public DateTime? DueDate { get; set; }
 [Range(1,100000000000000d, ErrorMessage="مبلغ باید بزرگ‌تر از صفر باشد.")] public double Amount { get; set; }
 [StringLength(50)] public string? ProjectName { get; set; }
 [StringLength(50)] public string? BankBranchCode { get; set; }
 [StringLength(50)] public string? ReferenceNumber { get; set; }
 [StringLength(50)] public string? AccountNumber { get; set; }
 [StringLength(255)] public string? Comment { get; set; }
 public bool Returned { get; set; }
}
