using System.ComponentModel.DataAnnotations;
namespace BaseSite.Web.Models;
public sealed class NewDocumentRequest {
 [Range(1, int.MaxValue)] public int CustomerId { get; set; }
 [StringLength(50)] public string? ClienteleName { get; set; }
 [StringLength(50)] public string? ProjectName { get; set; }
 [StringLength(255)] public string? DeliveryAddress { get; set; }
 [StringLength(255)] public string? Comment { get; set; }
 public DateTime? FactorDate { get; set; }
 [Range(0, 100)] public double Tax { get; set; }
 [Range(0, 1000000000000d)] public double Discount { get; set; }
 [Range(0, 1000000000000d)] public double DeliveryCost { get; set; }
 [Range(0, 1000000000000d)] public double ServiceCost { get; set; }
 public byte? TradeTypeId { get; set; }
 public byte? OrderTypeId { get; set; }
 public List<NewGoodsItem> Items { get; set; } = [];
}
public sealed class NewGoodsItem {
 [Required, StringLength(100)] public string Name { get; set; } = "";
 [Range(1,100000)] public int Count { get; set; } = 1;
 [Range(0,1000000000d)] public double UnitPrice { get; set; }
}
public sealed class CreatedDocument { public int Id { get; set; } public int DocumentNumber { get; set; } }
public sealed class NewDocumentOptions {
 public List<OrderLookup> TradeTypes { get; set; } = [];
 public List<OrderLookup> OrderTypes { get; set; } = [];
}
