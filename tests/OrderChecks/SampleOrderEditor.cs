using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
internal static class SampleOrderEditor
{
    public static OrderEditor Create(int id)
    {
        var order = SampleOrders.Create().First();
        order.Id = id; order.DocNumber = id == 0 ? 0 : 700000 + id;
        order.StatusId = id == 3 ? (byte)3 : id == 2 ? (byte)2 : (byte)1;
        order.Order_Status.Name = order.StatusId switch { 1 => "پیش فاکتور", 2 => "در دست اقدام", _ => "درخواست تولید" };
        var form = OrderEditorData.Map(order);
        form.CustomerId = id == 0 ? 0 : 1;
        form.Panels.Clear();
        form.Panels.Add(new() { Id = id == 0 ? 0 : 1, Kind = "Cabin", ModelId = 1, Count = 1, FloorCount = 5, FloorNames = "G, 1, 2, 3, 4",
            DocumentNumber = id == 0 ? 0 : 2700000 + id, MonitorId = 1, PushButtonId = 1, SurfaceMetalId = 1,
            ProductionStatus = "نقشه کشی", OriginalSelections = new() { ["Model"] = 1 }, Prices = new() { ["Model"] = 1000000 } });
        form.Panels.Add(new() { Id = id == 0 ? 0 : 1, Kind = "Hall", ModelId = 1, Count = 5, ElevatorTypeId = 1, PushButtonCountId = 1, FloorCount = 5 });
        form.Panels.Add(new() { Id = id == 0 ? 0 : 1, Kind = "DoorTop", ModelId = 1, Count = 1 });
        var options = new Dictionary<string, List<OrderChoice>>
        {
            ["TradeTypes"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "فروش", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["ElevatorBoards"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["PackTypes"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["CabinPanels"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "پنل کابین نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["HallPanels"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "پنل طبقات نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["DoorTopPanels"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "پنل سردرب نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["CabinSurfaceMetals"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["HallSurfaceMetals"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["SurfaceMetals"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["PushButtons"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "شاسی نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["Monitors"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "نمایشگر نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["Speakers"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["EmergencyLights"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["InstallationTypes"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["ElevatorCounts"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["HallPushButtonCounts"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["Attachments"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "متعلقات نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["Additions"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
            ["Deductions"] = [new() { Id = 0, Name = "ندارد" }, new() { Id = 1, Name = "گزینه نمونه", Cost = 100000, StartFrom = 1, SurfaceArea = 0.5 }],
        };
        foreach (var panel in form.Panels) panel.Calculate(options, form.StatusId);
        var detail = OrderDetails.Map(order);
        detail.Summary.Amount = form.Total;
        return new() { Form = form, Options = options, Detail = detail, CanEdit = OrderForm.IsEditable(form.StatusId) };
    }
}

