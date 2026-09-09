#nullable enable
using System.ComponentModel.DataAnnotations;
namespace BaseSite.Web.Models;
public sealed class ReportInfo {
 public string Key {get;set;}=""; public string Title {get;set;}=""; public string Category {get;set;}="";
 public string Mode {get;set;}="table"; public bool CustomerFilter {get;set;} public bool PartFilter {get;set;}
 public bool Daily {get;set;}
}
public sealed class ReportFilter : IValidatableObject {
 [Required] public DateTime? DateFrom {get;set;}
 [Required] public DateTime? DateTo {get;set;}
 [Range(0,int.MaxValue)] public int CustomerId {get;set;}
 [Range(0,5)] public int Part {get;set;}
 public IEnumerable<ValidationResult> Validate(ValidationContext context) {
  if(DateFrom?.Date>DateTo?.Date || DateTo?.Date==DateTime.MaxValue.Date) yield return new("بازه تاریخ معتبر نیست.");
 }
}
public sealed class ReportTable {
 public string Title {get;set;}="";
 public List<string> Columns {get;set;}=[];
 public List<List<string>> Rows {get;set;}=[];
}
public sealed class ReportResult {
 public List<string> ChartLabels {get;set;}=[];
 public List<double> OrderAmounts {get;set;}=[];
 public List<double> PaymentAmounts {get;set;}=[];
 public List<ReportTable> Tables {get;set;}=[];
 public string? EmbedUrl {get;set;}
}
