#nullable enable

using System.ComponentModel.DataAnnotations;

namespace BaseSite.Web.Models;

public sealed class OrderSearch : IValidatableObject
{
    public int? DocumentNumber { get; set; }
    public string? Customer { get; set; }
    public int? CustomerId { get; set; }
    public byte? StatusId { get; set; }
    public byte? TradeTypeId { get; set; }
    public string? ProjectName { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public DateTime? FactorDateFrom { get; set; }
    public DateTime? FactorDateTo { get; set; }
    [Range(0, 1000000)] public int Page { get; set; }
    [Range(1, 100)] public int PageSize { get; set; } = 20;
    public bool HasFilters => DocumentNumber.HasValue || CustomerId.HasValue || !string.IsNullOrWhiteSpace(Customer)
        || StatusId.HasValue || TradeTypeId.HasValue || !string.IsNullOrWhiteSpace(ProjectName)
        || OrderDateFrom.HasValue || OrderDateTo.HasValue || FactorDateFrom.HasValue || FactorDateTo.HasValue;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (OrderDateFrom?.Date > OrderDateTo?.Date)
            yield return new ValidationResult("بازه تاریخ سفارش معتبر نیست.");
        if (FactorDateFrom?.Date > FactorDateTo?.Date)
            yield return new ValidationResult("بازه تاریخ فاکتور معتبر نیست.");
        if (OrderDateTo?.Date == DateTime.MaxValue.Date || FactorDateTo?.Date == DateTime.MaxValue.Date)
            yield return new ValidationResult("تاریخ پایان خارج از محدوده مجاز است.");
    }
}

public class OrderSummary
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public int DocumentNumber { get; set; }
    public int FactorNumber { get; set; }
    public string Receiver { get; set; } = "";
    public string Customer { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public DateTime? OrderDate { get; set; }
    public DateTime? FactorDate { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; } = "";
    public string TradeType { get; set; } = "";
    public bool HasTax { get; set; }
}

public sealed class OrderPage
{
    public List<OrderSummary> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public sealed class OrderLookup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public override string ToString() => Name;
}

public sealed class OrderLookups
{
    public List<OrderLookup> Statuses { get; set; } = [];
    public List<OrderLookup> TradeTypes { get; set; } = [];
}

public sealed class OrderDetail
{
    public OrderSummary Summary { get; set; } = new();
    public string ClienteleName { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public string PackType { get; set; } = "";
    public string ElevatorBoard { get; set; } = "";
    public string Comment { get; set; } = "";
    public DateTime? ProductionRequestDate { get; set; }
    public double? DeliveryCost { get; set; }
    public double TaxPercent { get; set; }
    public double DiscountPercent { get; set; }
    public double CabinTotal { get; set; }
    public double HallTotal { get; set; }
    public double DoorTopTotal { get; set; }
    public double AttachmentTotal { get; set; }
    public double AdditionTotal { get; set; }
    public double DeductionTotal { get; set; }
    public double TaxTotal { get; set; }
    public double DiscountTotal { get; set; }
    public List<OrderPanel> Panels { get; set; } = [];
    public List<OrderExtra> Deductions { get; set; } = [];
}

public sealed class OrderPanel
{
    public string Kind { get; set; } = "";
    public string Name { get; set; } = "";
    public int DocumentNumber { get; set; }
    public int Count { get; set; }
    public double Amount { get; set; }
    public List<OrderSpec> Specifications { get; set; } = [];
    public List<OrderExtra> Extras { get; set; } = [];
}

public sealed class OrderSpec
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
}

public sealed class OrderExtra
{
    public string Kind { get; set; } = "";
    public string Name { get; set; } = "";
    public int? Count { get; set; }
    public double Amount { get; set; }
}
