#nullable enable
using System.ComponentModel.DataAnnotations;
namespace BaseSite.Api.Contracts;

public sealed class OrderEditor
{
    public OrderForm Form { get; set; } = new();
    public OrderDetail Detail { get; set; } = new();
    public Dictionary<string, List<OrderChoice>> Options { get; set; } = [];
    public bool CanEdit { get; set; }
}
public sealed class OrderChoice
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Cost { get; set; }
    public double SurfaceArea { get; set; }
    public byte StartFrom { get; set; }
}
public sealed class OrderForm
{
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    [MaxLength(50)] public string ProjectName { get; set; } = "";
    [MaxLength(50)] public string ClienteleName { get; set; } = "";
    public byte TradeTypeId { get; set; }
    public int ElevatorBoardId { get; set; }
    public short PackTypeId { get; set; }
    [MaxLength(255)] public string DeliveryAddress { get; set; } = "";
    [MaxLength(255)] public string Comment { get; set; } = "";
    public DateTime? FactorDate { get; set; }
    [Range(0, 1e15)] public double DeliveryCost { get; set; }
    [Range(0, 100)] public double Tax { get; set; }
    [Range(0, 100)] public double DiscountRate { get; set; }
    public byte StatusId { get; set; } = 1;
    [Required, MaxLength(100)] public List<OrderPanelForm> Panels { get; set; } = [];
    [Required, MaxLength(100)] public List<OrderExtraForm> Deductions { get; set; } = [];
    public static bool IsEditable(byte status) => status is 1 or 2;
    public double Subtotal => Panels.Sum(x => x.Amount) - Deductions.Sum(x => x.Cost);
    public double DiscountTotal => Subtotal * DiscountRate / 100;
    public double TaxTotal => (Subtotal - DiscountTotal) * Tax / 100;
    public double Total => Subtotal - DiscountTotal + TaxTotal + DeliveryCost;
}
public sealed class OrderPanelForm
{
    public int Id { get; set; }
    [RegularExpression("Cabin|Hall|DoorTop")] public string Kind { get; set; } = "Cabin";
    public int ModelId { get; set; }
    [Range(1, 10000)] public int Count { get; set; } = 1;
    public int PushButtonId { get; set; }
    public int MonitorId { get; set; }
    public int SurfaceMetalId { get; set; }
    public int? SurfaceMetalId2 { get; set; }
    public short InstallationTypeId { get; set; }
    public short SpeakerId { get; set; }
    public short EmergencyLightId { get; set; }
    public short ElevatorTypeId { get; set; }
    public short PushButtonCountId { get; set; }
    [Range(0, 200)] public int FloorCount { get; set; }
    [Range(0, 200)] public int UGFloorCount { get; set; }
    [MaxLength(50)] public string FloorNames { get; set; } = "";
    [MaxLength(50)] public string UGFloorNames { get; set; } = "";
    public bool PhoneCallButton { get; set; }
    public bool DO { get; set; }
    public bool DC { get; set; }
    public int? SheetNumber { get; set; }
    [MaxLength(255)] public string LaserCuttingText { get; set; } = "";
    [MaxLength(255)] public string LaserEngravingText { get; set; } = "";
    [MaxLength(255)] public string Comment { get; set; } = "";
    [Required, MaxLength(100)] public List<OrderExtraForm> Attachments { get; set; } = [];
    [Required, MaxLength(100)] public List<OrderExtraForm> Additions { get; set; } = [];
    // Snapshot prices preserve agreed rates in the in-progress stage.
    public Dictionary<string, int> OriginalSelections { get; set; } = [];
    public Dictionary<string, double> Prices { get; set; } = [];
    public double Amount { get; set; }
    public double SurfaceArea { get; set; }
    public string ProductionStatus { get; set; } = "";
    public int DocumentNumber { get; set; }

    public void Calculate(Dictionary<string, List<OrderChoice>> options, byte status)
    {
        double Price(string key, int id, string source)
        {
            if (status == 2 && OriginalSelections.GetValueOrDefault(key, -1) == id && Prices.TryGetValue(key, out var agreed)) return agreed;
            return options.GetValueOrDefault(source)?.FirstOrDefault(x => x.Id == id)?.Cost ?? 0;
        }
        var model = Price("Model", ModelId, Kind + "Panels");
        var monitor = Price("Monitor", MonitorId, "Monitors");
        var metal = Price("Metal", SurfaceMetalId, Kind == "DoorTop" ? "SurfaceMetals" : Kind + "SurfaceMetals");
        var button = Price("Button", PushButtonId, "PushButtons");
        var area = status == 2 && OriginalSelections.GetValueOrDefault("Model", -1) == ModelId
            ? SurfaceArea : options.GetValueOrDefault("DoorTopPanels")?.FirstOrDefault(x => x.Id == ModelId)?.SurfaceArea ?? 0;
        foreach (var extra in Attachments)
            extra.Cost = (status == 2 && extra.OriginalLookupId == extra.LookupId ? extra.UnitPrice
                : options.GetValueOrDefault("Attachments")?.FirstOrDefault(x => x.Id == extra.LookupId)?.Cost ?? 0) * extra.Count;
        Amount = ModelId == 0 ? 0 : (Kind switch
        {
            "Cabin" => model + metal + monitor + button * (FloorCount + (DO ? 1 : 0) + (DC ? 1 : 0) + (PhoneCallButton ? 1 : 0) + 2),
            "Hall" => model + metal * ElevatorTypeId + monitor * ElevatorTypeId + button * PushButtonCountId,
            _ => model + monitor + (SurfaceMetalId > 10 ? Math.Round(metal * area) : metal)
        }) * Count + Attachments.Sum(x => x.Cost) + Additions.Sum(x => x.Cost);
    }
}
public sealed class OrderExtraForm
{
    public int Id { get; set; }
    public int LookupId { get; set; }
    [Range(1, 10000)] public int Count { get; set; } = 1;
    [Range(0, 1e15)] public double Cost { get; set; }
    public int OriginalLookupId { get; set; } = -1;
    public double UnitPrice { get; set; }
}

