#nullable enable

using System.ComponentModel.DataAnnotations;

namespace BaseSite.Api.Contracts;

public sealed class ServiceSearch : IValidatableObject
{
    public int? DocumentNumber { get; set; }
    public string? Customer { get; set; }
    public int? CustomerId { get; set; }
    public byte? StatusId { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public DateTime? FactorDateFrom { get; set; }
    public DateTime? FactorDateTo { get; set; }
    [Range(0, 1000000)] public int Page { get; set; }
    [Range(1, 100)] public int PageSize { get; set; } = 20;
    public bool HasFilters => DocumentNumber.HasValue || CustomerId.HasValue || !string.IsNullOrWhiteSpace(Customer)
        || StatusId.HasValue
        || OrderDateFrom.HasValue || OrderDateTo.HasValue || FactorDateFrom.HasValue || FactorDateTo.HasValue;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (OrderDateFrom?.Date > OrderDateTo?.Date)
            yield return new ValidationResult("بازه تاریخ سفارش معتبر نیست.");
        if (FactorDateFrom?.Date > FactorDateTo?.Date)
            yield return new ValidationResult("بازه تاریخ تحویل معتبر نیست.");
        if (OrderDateTo?.Date == DateTime.MaxValue.Date || FactorDateTo?.Date == DateTime.MaxValue.Date)
            yield return new ValidationResult("تاریخ پایان خارج از محدوده مجاز است.");
    }
}

public class ServiceSummary
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public int DocumentNumber { get; set; }
    public int FactorNumber { get; set; }
    public string Receiver { get; set; } = "";
    public string Customer { get; set; } = "";
    public DateTime? OrderDate { get; set; }
    public DateTime? FactorDate { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; } = "";
    public string OrderType { get; set; } = "";
    public bool HasTax { get; set; }
}

public sealed class ServicePage
{
    public List<ServiceSummary> Items { get; set; } = [];
    public int TotalCount { get; set; }
}


public sealed class ServiceDetail {
 public ServiceSummary Summary { get; set; } = new();
 public string ClienteleName { get; set; } = "";
 public string DeliveryAddress { get; set; } = "";
 public string Comment { get; set; } = "";
 public double DeliveryCost { get; set; }
 public double TaxPercent { get; set; }
 public double ServiceCost { get; set; }
 public double Discount { get; set; }
}
