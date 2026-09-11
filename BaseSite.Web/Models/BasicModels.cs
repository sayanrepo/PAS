#nullable enable
namespace BaseSite.Web.Models;
public sealed class BasicField {
 public string Key {get;set;}=""; public string Label {get;set;}=""; public string Kind {get;set;}="text";
 public int MaxLength {get;set;}=100; public bool CanEdit {get;set;} public bool CanAdd {get;set;}
 public List<OrderLookup> Choices {get;set;}=[];
}
public sealed class BasicRecord { public int Id {get;set;} public Dictionary<string,string> Values {get;set;}=[]; }
public sealed class BasicTable {
 public string Key {get;set;}="";public string Title {get;set;}="";
 public bool CanAdd {get;set;} public bool CanEdit {get;set;} public bool CanDelete {get;set;}
 public string? ParentKey {get;set;} public int? ParentId {get;set;}
 public List<OrderLookup> Parents {get;set;}=[];
 public List<BasicField> Fields {get;set;}=[];public List<BasicRecord> Rows {get;set;}=[];
}
public sealed class BasicSave {public int? ParentId {get;set;} public Dictionary<string,string> Values {get;set;}=[];}
public sealed class BasicPriceRow { public int Id {get;set;} public double ExpectedCost {get;set;} }
public sealed class BasicPriceChange { public double Percent {get;set;} public double Amount {get;set;} public List<BasicPriceRow> Rows {get;set;}=[]; }
public sealed class CompatibilityGrid {
 public List<OrderLookup> Panels {get;set;}=[]; public List<OrderLookup> Parts {get;set;}=[];
 public List<CompatibilityCell> Cells {get;set;}=[]; public bool CanEdit {get;set;}
}
public sealed class CompatibilityCell {public int PanelId {get;set;} public int PartId {get;set;} public double Value {get;set;}=1;}
