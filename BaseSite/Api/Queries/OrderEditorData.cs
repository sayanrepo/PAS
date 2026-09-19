#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;
using BaseSite.Data;
using System.Data.Entity;
namespace BaseSite.Api.Queries;

public static class OrderEditorData
{
    public static Dictionary<string, List<OrderChoice>> Options(PantaEntities db) => new()
    {
        ["OrderStatuses"] = db.Order_Status.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["TradeTypes"] = db.Tb_TradeTypes.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["ElevatorBoards"] = db.Tb_ElevatorBoards.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["PackTypes"] = db.Tb_PackTypes.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["CabinPanels"] = db.Tb_CabinPanels.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost, StartFrom = x.StartFrom }).ToList(),
        ["HallPanels"] = db.Tb_HallPanels.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost, StartFrom = x.StartFrom }).ToList(),
        ["DoorTopPanels"] = db.Tb_DoorTopPanels.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost, StartFrom = x.StartFrom, SurfaceArea = x.SurfaceArea }).ToList(),
        ["CabinSurfaceMetals"] = db.Tb_CabinSurfaceMetals.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost }).ToList(),
        ["HallSurfaceMetals"] = db.Tb_HallSurfaceMetals.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost }).ToList(),
        ["SurfaceMetals"] = db.Tb_SurfaceMetals.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost }).ToList(),
        ["PushButtons"] = db.Tb_PushButtons.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost }).ToList(),
        ["Monitors"] = db.Tb_Monitors.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost }).ToList(),
        ["Speakers"] = db.Tb_Speakers.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["EmergencyLights"] = db.Tb_EmergencyLights.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["InstallationTypes"] = db.Tb_InstallationTypes.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["ElevatorCounts"] = db.Tb_ElevatorCounts.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["HallPushButtonCounts"] = db.Tb_HallPushButtonCounts.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["Attachments"] = db.Tb_Attachments.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name, Cost = x.Cost }).ToList(),
        ["Additions"] = db.Tb_Additions.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
        ["Deductions"] = db.Tb_Deductions.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderChoice { Id = x.Id, Name = x.Name }).ToList(),
    };
    public static OrderForm Map(Order_Order x) => new()
    {
        CustomerId = x.CustomerId, ProjectName = x.ProjectName ?? "", ClienteleName = x.ClienteleName ?? "",
        TradeTypeId = x.TradeTypeId, ElevatorBoardId = x.ElevatorBoardId, PackTypeId = x.PackTypeId,
        DeliveryAddress = x.DeliveryAddress ?? "", Comment = x.Comment ?? "", FactorDate = x.DateFactor,
        DeliveryCost = x.DeliveryCost ?? 0, Tax = x.Tax, DiscountRate = x.DiscountRate, StatusId = x.StatusId,
        Panels = x.Order_Cabin.OrderBy(p => p.Id).Select(Map).Concat(x.Order_Hall.OrderBy(p => p.Id).Select(Map)).Concat(x.Order_DoorTop.OrderBy(p => p.Id).Select(Map)).ToList(),
        Deductions = x.Order_Deduction.Select(d => new OrderExtraForm { Id = d.Id, LookupId = d.DeductionId, Cost = d.Cost }).ToList()
    };
    private static List<OrderExtraForm> Attachments(IEnumerable<Order_Panel_Attachment> rows) =>
        rows.Select(x => new OrderExtraForm { Id = x.Id, LookupId = x.AttachmentId, Count = x.Count, Cost = x.Cost,
            OriginalLookupId = x.AttachmentId, UnitPrice = x.Count > 0 ? x.Cost / x.Count : 0 }).ToList();
    private static List<OrderExtraForm> Additions(IEnumerable<Order_Panel_Addition> rows) =>
        rows.Select(x => new OrderExtraForm { Id = x.Id, LookupId = x.AdditionId, Cost = x.Cost }).ToList();

    public static OrderPanelForm Map(Order_Cabin x) => new()
    {
        Id = x.Id, Kind = "Cabin", ModelId = x.CabinPanelId, DocumentNumber = x.DocNumber,
        ProductionStatus = x.Order_ProductStatus?.Name ?? "", Amount = x.Cost,
        Count = x.Count, MonitorId = x.MonitorId, SurfaceMetalId = x.SurfaceMetalId, Comment = x.Comment ?? "", PushButtonId = x.PushButtonId, SurfaceMetalId2 = x.SurfaceMetalId2, InstallationTypeId = x.InstallationTypeId, SpeakerId = x.SpeakerId, EmergencyLightId = x.EmergencyLightId, FloorCount = x.FloorCount, UGFloorCount = x.UGFloorCount, FloorNames = x.FloorNames ?? "", UGFloorNames = x.UGFloorNames ?? "", PhoneCallButton = x.PhoneCallButton, DO = x.DO, DC = x.DC, SheetNumber = x.SheetNumber, LaserCuttingText = x.LaserCuttingText ?? "", LaserEngravingText = x.LaserEngravingText ?? "",
        OriginalSelections = new() { ["Model"] = x.CabinPanelId, ["Monitor"] = x.MonitorId, ["Metal"] = x.SurfaceMetalId, ["Button"] = x.PushButtonId },
        Prices = new() { ["Model"] = x.CostCabinPanel, ["Monitor"] = x.CostMonitor, ["Metal"] = x.CostSurfaceMetal, ["Button"] = x.CostPushButton },
        
        Attachments = Attachments(x.Order_Panel_Attachment), Additions = Additions(x.Order_Panel_Addition)
    };

    public static OrderPanelForm Map(Order_Hall x) => new()
    {
        Id = x.Id, Kind = "Hall", ModelId = x.HallPanelId, DocumentNumber = x.DocNumber,
        ProductionStatus = x.Order_ProductStatus?.Name ?? "", Amount = x.Cost,
        Count = x.Count, MonitorId = x.MonitorId, SurfaceMetalId = x.SurfaceMetalId, Comment = x.Comment ?? "", PushButtonId = x.PushButtonId, ElevatorTypeId = x.ElevatorTypeId, PushButtonCountId = x.PushButtonCountId, FloorCount = x.FloorCount, UGFloorCount = x.UGFloorCount, FloorNames = x.FloorNames ?? "", UGFloorNames = x.UGFloorNames ?? "",
        OriginalSelections = new() { ["Model"] = x.HallPanelId, ["Monitor"] = x.MonitorId, ["Metal"] = x.SurfaceMetalId, ["Button"] = x.PushButtonId },
        Prices = new() { ["Model"] = x.CostHallPanel, ["Monitor"] = x.CostMonitor, ["Metal"] = x.CostSurfaceMetal, ["Button"] = x.CostPushButton },
        
        Attachments = Attachments(x.Order_Panel_Attachment), Additions = Additions(x.Order_Panel_Addition)
    };

    public static OrderPanelForm Map(Order_DoorTop x) => new()
    {
        Id = x.Id, Kind = "DoorTop", ModelId = x.DoorTopPanelId, DocumentNumber = x.DocNumber,
        ProductionStatus = x.Order_ProductStatus?.Name ?? "", Amount = x.Cost,
        Count = x.Count, MonitorId = x.MonitorId, SurfaceMetalId = x.SurfaceMetalId, Comment = x.Comment ?? "",
        OriginalSelections = new() { ["Model"] = x.DoorTopPanelId, ["Monitor"] = x.MonitorId, ["Metal"] = x.SurfaceMetalId },
        Prices = new() { ["Model"] = x.CostDoorTopPanel, ["Monitor"] = x.CostMonitor, ["Metal"] = x.CostSurfaceMetal },
        SurfaceArea = x.SurfaceMetalDosage,
        Attachments = Attachments(x.Order_Panel_Attachment), Additions = Additions(x.Order_Panel_Addition)
    };

    public static string? Validate(OrderForm form, Order_Order original, Dictionary<string, List<OrderChoice>> options)
    {
        bool Has(string key, int id) => options[key].Any(x => x.Id == id);
        if (!OrderForm.IsEditable(original.StatusId) || !OrderForm.CanChangeStatus(original.StatusId, form.StatusId)) return "وضعیت سفارش تغییر کرده است؛ صفحه را دوباره باز کنید.";
        if (!Has("TradeTypes", form.TradeTypeId) || !Has("ElevatorBoards", form.ElevatorBoardId) || !Has("PackTypes", form.PackTypeId)) return "مشخصات سفارش معتبر نیست.";
        var old = Map(original);
        bool ValidRows(List<OrderExtraForm> rows, List<OrderExtraForm> previous) =>
            rows.All(x => x is not null && (x.Id == 0 || previous.Any(p => p.Id == x.Id)))
            && !rows.Where(x => x.Id != 0).GroupBy(x => x.Id).Any(x => x.Count() > 1);
        if (form.Panels.Count > 100 || form.Panels.Any(x => x is null) || form.Deductions.Any(x => x is null)) return "اقلام سفارش معتبر نیست.";
        if (form.Panels.Where(x => x.Id != 0).GroupBy(x => (x.Kind, x.Id)).Any(g => g.Count() > 1)) return "پنل تکراری است.";
        foreach (var panel in form.Panels)
        {
            if (panel.Kind is not ("Cabin" or "Hall" or "DoorTop")) return "نوع پنل معتبر نیست.";
            if (panel.Id != 0 && !old.Panels.Any(x => x.Id == panel.Id && x.Kind == panel.Kind)) return "پنل متعلق به این سفارش نیست.";
            var previous = old.Panels.FirstOrDefault(x => x.Id == panel.Id && x.Kind == panel.Kind && x.Id != 0);
            if (!ValidRows(panel.Attachments, previous?.Attachments ?? []) || !ValidRows(panel.Additions, previous?.Additions ?? [])) return "ردیف‌های ملحقات یا اضافات معتبر نیست.";
            if (!Has(panel.Kind + "Panels", panel.ModelId) || !Has("Monitors", panel.MonitorId)
                || !Has(panel.Kind == "DoorTop" ? "SurfaceMetals" : panel.Kind + "SurfaceMetals", panel.SurfaceMetalId)) return "مدل یا مشخصات پنل معتبر نیست.";
            if (panel.Kind != "DoorTop" && !Has("PushButtons", panel.PushButtonId)) return "پوش باتون معتبر نیست.";
            if (panel.Kind == "Cabin" && (!Has("InstallationTypes", panel.InstallationTypeId) || !Has("Speakers", panel.SpeakerId)
                || !Has("EmergencyLights", panel.EmergencyLightId) || (panel.SurfaceMetalId2.HasValue && !Has("CabinSurfaceMetals", panel.SurfaceMetalId2.Value)))) return "مشخصات پنل کابین معتبر نیست.";
            if (panel.Kind == "Hall" && (!Has("ElevatorCounts", panel.ElevatorTypeId) || !Has("HallPushButtonCounts", panel.PushButtonCountId))) return "مشخصات پنل طبقات معتبر نیست.";
            if (panel.Attachments.Any(x => x is null || !Has("Attachments", x.LookupId)) || panel.Additions.Any(x => x is null || !Has("Additions", x.LookupId))) return "ملحقات یا اضافات معتبر نیست.";
        }
        if (form.Deductions.Any(x => !Has("Deductions", x.LookupId))) return "کسورات معتبر نیست.";
        if (!ValidRows(form.Deductions, old.Deductions)) return "ردیف‌های کسورات معتبر نیست.";
        return null;
    }

    public static void ApplyProductionRequestDates(Order_Order order, DateTime requestedAt)
    {
        order.DateDelivery = requestedAt;
        order.DateFactor ??= requestedAt.AddDays(10);
    }

    public static void Apply(PantaEntities db, Order_Order order, OrderForm form, Dictionary<string, List<OrderChoice>> options)
    {
        var old = Map(order);
        foreach (var panel in form.Panels)
        {
            var previous = old.Panels.FirstOrDefault(x => x.Kind == panel.Kind && x.Id == panel.Id && x.Id != 0);
            // Never accept client-supplied agreed prices or calculated totals.
            panel.OriginalSelections = previous?.OriginalSelections ?? [];
            panel.Prices = previous?.Prices ?? [];
            panel.SurfaceArea = previous?.SurfaceArea ?? 0;
            foreach (var extra in panel.Attachments)
            {
                var saved = previous?.Attachments.FirstOrDefault(x => x.Id == extra.Id && x.Id != 0);
                extra.OriginalLookupId = saved?.LookupId ?? -1;
                extra.UnitPrice = saved?.UnitPrice ?? 0;
            }
            panel.Calculate(options, order.StatusId);
        }
        if (form.Subtotal < 0) throw new ArgumentException("کسورات از مبلغ پنل‌ها بیشتر است.");
        order.CustomerId = form.CustomerId; order.ClienteleName = form.ClienteleName; order.ProjectName = form.ProjectName;
        order.TradeTypeId = form.TradeTypeId; order.ElevatorBoardId = form.ElevatorBoardId; order.PackTypeId = form.PackTypeId;
        order.DeliveryAddress = form.DeliveryAddress; order.Comment = form.Comment; order.DateFactor = form.FactorDate;
        order.DeliveryCost = form.DeliveryCost; order.Tax = form.Tax; order.DiscountRate = form.DiscountRate;
        var nextPanelNumber = old.Panels.Select(x => x.DocumentNumber / 1000000).DefaultIfEmpty(1).Max() + 1;

        foreach (var removed in order.Order_Cabin.Where(x => !form.Panels.Any(p => p.Kind == "Cabin" && p.Id == x.Id && p.Id != 0)).ToList())
        {
            db.Order_Panel_Attachment.RemoveRange(removed.Order_Panel_Attachment.ToList());
            db.Order_Panel_Addition.RemoveRange(removed.Order_Panel_Addition.ToList());
            db.Order_Cabin.Remove(removed);
        }
        foreach (var panel in form.Panels.Where(x => x.Kind == "Cabin"))
        {
            var entity = order.Order_Cabin.FirstOrDefault(x => x.Id == panel.Id && x.Id != 0);
            if (entity is null)
            {
                entity = new Order_Cabin { TableId = 15, DocNumber = order.DocNumber + nextPanelNumber++ * 1000000 };
                order.Order_Cabin.Add(entity);
            }
            double Price(string key, int id, string source) => order.StatusId == 2 && panel.OriginalSelections.GetValueOrDefault(key, -1) == id
                ? panel.Prices.GetValueOrDefault(key) : options[source].First(x => x.Id == id).Cost;
            entity.CostCabinPanel = Price("Model", panel.ModelId, "CabinPanels");
            entity.CostMonitor = Price("Monitor", panel.MonitorId, "Monitors");
            entity.CostSurfaceMetal = Price("Metal", panel.SurfaceMetalId, "CabinSurfaceMetals");
            entity.CostPushButton = Price("Button", panel.PushButtonId, "PushButtons");
            if (entity.Id == 0 || entity.CabinPanelId != panel.ModelId) entity.ProductStatusId = options["CabinPanels"].First(x => x.Id == panel.ModelId).StartFrom;
            entity.CabinPanelId = panel.ModelId;
            entity.Count = panel.Count;
            entity.MonitorId = panel.MonitorId;
            entity.SurfaceMetalId = panel.SurfaceMetalId;
            entity.Comment = panel.Comment;
            entity.PushButtonId = panel.PushButtonId;
            entity.SurfaceMetalId2 = panel.SurfaceMetalId2;
            entity.InstallationTypeId = panel.InstallationTypeId;
            entity.SpeakerId = panel.SpeakerId;
            entity.EmergencyLightId = panel.EmergencyLightId;
            entity.FloorCount = panel.FloorCount;
            entity.UGFloorCount = panel.UGFloorCount;
            entity.FloorNames = panel.FloorNames;
            entity.UGFloorNames = panel.UGFloorNames;
            entity.PhoneCallButton = panel.PhoneCallButton;
            entity.DO = panel.DO;
            entity.DC = panel.DC;
            entity.SheetNumber = panel.SheetNumber;
            entity.LaserCuttingText = panel.LaserCuttingText;
            entity.LaserEngravingText = panel.LaserEngravingText;
            entity.Cost = panel.Amount;
            SyncExtras(db, entity.Order_Panel_Attachment, entity.Order_Panel_Addition, panel);
        }

        foreach (var removed in order.Order_Hall.Where(x => !form.Panels.Any(p => p.Kind == "Hall" && p.Id == x.Id && p.Id != 0)).ToList())
        {
            db.Order_Panel_Attachment.RemoveRange(removed.Order_Panel_Attachment.ToList());
            db.Order_Panel_Addition.RemoveRange(removed.Order_Panel_Addition.ToList());
            db.Order_Hall.Remove(removed);
        }
        foreach (var panel in form.Panels.Where(x => x.Kind == "Hall"))
        {
            var entity = order.Order_Hall.FirstOrDefault(x => x.Id == panel.Id && x.Id != 0);
            if (entity is null)
            {
                entity = new Order_Hall { TableId = 16, DocNumber = order.DocNumber + nextPanelNumber++ * 1000000 };
                order.Order_Hall.Add(entity);
            }
            double Price(string key, int id, string source) => order.StatusId == 2 && panel.OriginalSelections.GetValueOrDefault(key, -1) == id
                ? panel.Prices.GetValueOrDefault(key) : options[source].First(x => x.Id == id).Cost;
            entity.CostHallPanel = Price("Model", panel.ModelId, "HallPanels");
            entity.CostMonitor = Price("Monitor", panel.MonitorId, "Monitors");
            entity.CostSurfaceMetal = Price("Metal", panel.SurfaceMetalId, "HallSurfaceMetals");
            entity.CostPushButton = Price("Button", panel.PushButtonId, "PushButtons");
            if (entity.Id == 0 || entity.HallPanelId != panel.ModelId) entity.ProductStatusId = options["HallPanels"].First(x => x.Id == panel.ModelId).StartFrom;
            entity.HallPanelId = panel.ModelId;
            entity.Count = panel.Count;
            entity.MonitorId = panel.MonitorId;
            entity.SurfaceMetalId = panel.SurfaceMetalId;
            entity.Comment = panel.Comment;
            entity.PushButtonId = panel.PushButtonId;
            entity.ElevatorTypeId = panel.ElevatorTypeId;
            entity.PushButtonCountId = panel.PushButtonCountId;
            entity.FloorCount = panel.FloorCount;
            entity.UGFloorCount = panel.UGFloorCount;
            entity.FloorNames = panel.FloorNames;
            entity.UGFloorNames = panel.UGFloorNames;
            entity.Cost = panel.Amount;
            SyncExtras(db, entity.Order_Panel_Attachment, entity.Order_Panel_Addition, panel);
        }

        foreach (var removed in order.Order_DoorTop.Where(x => !form.Panels.Any(p => p.Kind == "DoorTop" && p.Id == x.Id && p.Id != 0)).ToList())
        {
            db.Order_Panel_Attachment.RemoveRange(removed.Order_Panel_Attachment.ToList());
            db.Order_Panel_Addition.RemoveRange(removed.Order_Panel_Addition.ToList());
            db.Order_DoorTop.Remove(removed);
        }
        foreach (var panel in form.Panels.Where(x => x.Kind == "DoorTop"))
        {
            var entity = order.Order_DoorTop.FirstOrDefault(x => x.Id == panel.Id && x.Id != 0);
            if (entity is null)
            {
                entity = new Order_DoorTop { TableId = 17, DocNumber = order.DocNumber + nextPanelNumber++ * 1000000 };
                order.Order_DoorTop.Add(entity);
            }
            double Price(string key, int id, string source) => order.StatusId == 2 && panel.OriginalSelections.GetValueOrDefault(key, -1) == id
                ? panel.Prices.GetValueOrDefault(key) : options[source].First(x => x.Id == id).Cost;
            entity.CostDoorTopPanel = Price("Model", panel.ModelId, "DoorTopPanels");
            entity.CostMonitor = Price("Monitor", panel.MonitorId, "Monitors");
            entity.CostSurfaceMetal = Price("Metal", panel.SurfaceMetalId, "SurfaceMetals");
            entity.SurfaceMetalDosage = order.StatusId == 2 && panel.OriginalSelections.GetValueOrDefault("Model", -1) == panel.ModelId ? panel.SurfaceArea : options["DoorTopPanels"].First(x => x.Id == panel.ModelId).SurfaceArea;
            if (entity.Id == 0 || entity.DoorTopPanelId != panel.ModelId) entity.ProductStatusId = options["DoorTopPanels"].First(x => x.Id == panel.ModelId).StartFrom;
            entity.DoorTopPanelId = panel.ModelId;
            entity.Count = panel.Count;
            entity.MonitorId = panel.MonitorId;
            entity.SurfaceMetalId = panel.SurfaceMetalId;
            entity.Comment = panel.Comment;
            entity.Cost = panel.Amount;
            SyncExtras(db, entity.Order_Panel_Attachment, entity.Order_Panel_Addition, panel);
        }

        foreach (var removed in order.Order_Deduction.Where(x => !form.Deductions.Any(d => d.Id == x.Id && d.Id != 0)).ToList()) db.Order_Deduction.Remove(removed);
        foreach (var row in form.Deductions)
        {
            var entity = order.Order_Deduction.FirstOrDefault(x => x.Id == row.Id && x.Id != 0);
            if (entity is null) { entity = new(); order.Order_Deduction.Add(entity); }
            entity.DeductionId = row.LookupId; entity.Cost = row.Cost;
        }
        order.Cost = form.Total;
    }
    private static void SyncExtras(PantaEntities db, ICollection<Order_Panel_Attachment> attachments, ICollection<Order_Panel_Addition> additions, OrderPanelForm panel)
    {
        foreach (var removed in attachments.Where(x => panel.ModelId == 0 || !panel.Attachments.Any(p => p.Id == x.Id && p.Id != 0)).ToList()) db.Order_Panel_Attachment.Remove(removed);
        foreach (var removed in additions.Where(x => panel.ModelId == 0 || !panel.Additions.Any(p => p.Id == x.Id && p.Id != 0)).ToList()) db.Order_Panel_Addition.Remove(removed);
        if (panel.ModelId == 0) return;
        foreach (var row in panel.Attachments)
        {
            var entity = attachments.FirstOrDefault(x => x.Id == row.Id && x.Id != 0);
            if (entity is null) { entity = new(); attachments.Add(entity); }
            entity.AttachmentId = row.LookupId; entity.Count = row.Count; entity.Cost = row.Cost;
        }
        foreach (var row in panel.Additions)
        {
            var entity = additions.FirstOrDefault(x => x.Id == row.Id && x.Id != 0);
            if (entity is null) { entity = new(); additions.Add(entity); }
            entity.AdditionId = row.LookupId; entity.Cost = row.Cost;
        }
    }
}
