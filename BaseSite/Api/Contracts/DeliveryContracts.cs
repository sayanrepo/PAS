#nullable enable
using System.ComponentModel.DataAnnotations;
namespace BaseSite.Api.Contracts;
public sealed class DeliverySearch : IValidatableObject {
 public int? DocumentNumber {get;set;}
 public string? Customer {get;set;}
 public int? CustomerId {get;set;}
 public byte? StatusId {get;set;}
 public DateTime? DateFrom {get;set;}
 public DateTime? DateTo {get;set;}
 [Range(0,1000000)] public int Page {get;set;}
 [Range(1,100)] public int PageSize {get;set;} = 20;
 public bool HasFilters => DocumentNumber.HasValue || CustomerId.HasValue || !string.IsNullOrWhiteSpace(Customer) || StatusId.HasValue || DateFrom.HasValue || DateTo.HasValue;
 public IEnumerable<ValidationResult> Validate(ValidationContext context) {
  if(DocumentNumber <= 0) yield return new("شماره سند معتبر نیست.");
  if(DateFrom?.Date > DateTo?.Date || DateTo?.Date == DateTime.MaxValue.Date) yield return new("بازه تاریخ ثبت معتبر نیست.");
 }
}
public sealed class DeliverySummary {
 public int Id {get;set;} public int RowNumber {get;set;} public int DocumentNumber {get;set;}
 public DateTime? Date {get;set;} public string Kind {get;set;} = "";
 public string Customer {get;set;} = ""; public string ProjectName {get;set;} = "";
 public string Status {get;set;} = ""; public int FactorNumber {get;set;}
}
public sealed class DeliveryPage { public List<DeliverySummary> Items {get;set;} = []; public int TotalCount {get;set;} }
public sealed class DeliveryItem {
 public byte Type {get;set;} public int Id {get;set;} public string Name {get;set;} = "";
 public int Count {get;set;} public bool Selected {get;set;}
 [StringLength(255)] public string? Comment {get;set;}
}
public sealed class DeliveryForm {
 public string Kind {get;set;} = "orders";
 [Range(1,int.MaxValue)] public int SourceId {get;set;}
 public short PackTypeId {get;set;} public byte DeliveryLocationId {get;set;} = 1; public byte VehicleTypeId {get;set;} = 1;
 [StringLength(50)] public string? SendResponsible {get;set;}
 [StringLength(50)] public string? RecieveResponsible {get;set;}
 [StringLength(100)] public string? RecieverName {get;set;}
 [StringLength(100)] public string? RecieverPhone {get;set;}
 [StringLength(100)] public string? RecieverMobile {get;set;}
 [StringLength(100)] public string? CarierAgencyName {get;set;}
 [StringLength(100)] public string? CarierAgencyBill {get;set;}
 [StringLength(50)] public string? VehiclePlaque {get;set;}
 [StringLength(100)] public string? DriverName {get;set;}
 [StringLength(50)] public string? DriverPhone {get;set;}
 public string? DestinationAddress {get;set;}
 public List<DeliveryItem> Items {get;set;} = [];
}
public sealed class DeliveryDetail {
 public DeliverySummary Summary {get;set;} = new();
 public Dictionary<string,string> Fields {get;set;} = [];
 public List<DeliveryItem> Items {get;set;} = [];
}
public sealed class DeliveryLookups {
 public List<OrderLookup> Statuses {get;set;} = [];
 public List<OrderLookup> Packs {get;set;} = [];
 public List<OrderLookup> Locations {get;set;} = [];
 public List<OrderLookup> Vehicles {get;set;} = [];
}
public sealed class DeliveryDraft {
 public string Customer {get;set;} = "";
 public DeliveryForm Form {get;set;} = new();
}
