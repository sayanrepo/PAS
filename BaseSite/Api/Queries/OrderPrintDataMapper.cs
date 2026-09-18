#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class OrderPrintDataMapper
{
    public static OrderPrintData ForPrint(Order_Order order, string kind)
    {
        var data = Map(order);
        if (kind != "specification") return data;

        // Plan_Print permits the specification only, never financial/customer billing data.
        data.Account_Users = new() { FullName = data.Account_Users.FullName };
        data.Cost = data.SumCostAddition = data.SumCostDeduction = data.DiscountRate = data.Tax = 0;
        data.DeliveryCost = null;
        foreach (var panel in data.Order_Cabin) panel.Cost = panel.CostMonitor = 0;
        foreach (var panel in data.Order_Hall) panel.Cost = 0;
        foreach (var panel in data.Order_DoorTop) panel.Cost = 0;
        foreach (var attachment in data.Order_Cabin.SelectMany(x => x.Order_Panel_Attachment)
            .Concat(data.Order_Hall.SelectMany(x => x.Order_Panel_Attachment))
            .Concat(data.Order_DoorTop.SelectMany(x => x.Order_Panel_Attachment))) attachment.Cost = 0;
        foreach (var addition in data.Order_Cabin.SelectMany(x => x.Order_Panel_Addition)
            .Concat(data.Order_Hall.SelectMany(x => x.Order_Panel_Addition))
            .Concat(data.Order_DoorTop.SelectMany(x => x.Order_Panel_Addition))) addition.Cost = 0;
        return data;
    }

    public static OrderPrintData Map(Order_Order? value) => value is null ? new() : new()
    {
        DocNumber = value.DocNumber,
        ShDateDelivery = value.ShDateDelivery ?? "",
        ProjectName = value.ProjectName ?? "",
        Account_Users = Map(value.Account_Users),
        ShDateFactor = value.ShDateFactor ?? "",
        Tb_PackTypes = Map(value.Tb_PackTypes),
        Tb_ElevatorBoards = Map(value.Tb_ElevatorBoards),
        DeliveryAddress = value.DeliveryAddress ?? "",
        Order_Cabin = value.Order_Cabin.Select(Map).ToList(),
        Order_Hall = value.Order_Hall.Select(Map).ToList(),
        Order_DoorTop = value.Order_DoorTop.Select(Map).ToList(),
        StatusId = value.StatusId,
        FactorNumber = value.FactorNumber,
        SumCostDeduction = value.SumCostDeduction,
        SumCostAddition = value.SumCostAddition,
        DiscountRate = value.DiscountRate,
        Tax = value.Tax,
        DeliveryCost = value.DeliveryCost,
        Cost = value.Cost,
        ClienteleName = value.ClienteleName ?? "",
        AccepterId = value.AccepterId,
        DateDelivery = value.DateDelivery,
    };

    public static PrintCustomer Map(Account_Users? value) => value is null ? new() : new()
    {
        FullName = value.FullName ?? "",
        EconomicalNumber = value.EconomicalNumber ?? "",
        NationalNumber = value.NationalNumber ?? "",
        Location_Cities = Map(value.Location_Cities),
        PostalCode1 = value.PostalCode1 ?? "",
        Address1 = value.Address1 ?? "",
        Phone1 = value.Phone1 ?? "",
    };

    public static PrintPackType Map(Tb_PackTypes? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintElevatorBoard Map(Tb_ElevatorBoards? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintCabin Map(Order_Cabin? value) => value is null ? new() : new()
    {
        Count = value.Count,
        Tb_CabinPanels = Map(value.Tb_CabinPanels),
        Tb_InstallationTypes = Map(value.Tb_InstallationTypes),
        Tb_PushButtons = Map(value.Tb_PushButtons),
        Tb_Monitors = Map(value.Tb_Monitors),
        Tb_CabinSurfaceMetals = Map(value.Tb_CabinSurfaceMetals),
        FloorCount = value.FloorCount,
        FloorNames = value.FloorNames ?? "",
        UGFloorCount = value.UGFloorCount,
        Tb_Speakers = Map(value.Tb_Speakers),
        EmergencyLigh = Map(value.EmergencyLigh),
        PhoneCallButton = value.PhoneCallButton,
        DO = value.DO,
        DC = value.DC,
        LaserCuttingText = value.LaserCuttingText ?? "",
        LaserEngravingText = value.LaserEngravingText ?? "",
        Comment = value.Comment ?? "",
        Order_Panel_Attachment = value.Order_Panel_Attachment.Select(Map).ToList(),
        MonitorId = value.MonitorId,
        Cost = value.Cost,
        CostMonitor = value.CostMonitor,
        Order_Panel_Addition = value.Order_Panel_Addition.Select(Map).ToList(),
    };

    public static PrintCabinPanel Map(Tb_CabinPanels? value) => value is null ? new() : new()
    {
        Id = value.Id,
        Name = value.Name ?? "",
        Order_ProductStatus = Map(value.Order_ProductStatus),
        Description = value.Description ?? "",
    };

    public static PrintInstallationType Map(Tb_InstallationTypes? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintPushButton Map(Tb_PushButtons? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintProductStatus Map(Order_ProductStatus? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintMonitor Map(Tb_Monitors? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintCabinSurfaceMetal Map(Tb_CabinSurfaceMetals? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintSpeaker Map(Tb_Speakers? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintEmergencyLight Map(Tb_EmergencyLights? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintHall Map(Order_Hall? value) => value is null ? new() : new()
    {
        Count = value.Count,
        Tb_HallPanels = Map(value.Tb_HallPanels),
        Tb_PushButtons = Map(value.Tb_PushButtons),
        Tb_Monitors = Map(value.Tb_Monitors),
        Tb_HallSurfaceMetals = Map(value.Tb_HallSurfaceMetals),
        FloorCount = value.FloorCount,
        FloorNames = value.FloorNames ?? "",
        UGFloorCount = value.UGFloorCount,
        Comment = value.Comment ?? "",
        Order_Panel_Attachment = value.Order_Panel_Attachment.Select(Map).ToList(),
        Cost = value.Cost,
        Order_Panel_Addition = value.Order_Panel_Addition.Select(Map).ToList(),
    };

    public static PrintHallPanel Map(Tb_HallPanels? value) => value is null ? new() : new()
    {
        Id = value.Id,
        Name = value.Name ?? "",
        Order_ProductStatus = Map(value.Order_ProductStatus),
    };

    public static PrintHallSurfaceMetal Map(Tb_HallSurfaceMetals? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

    public static PrintDoorTop Map(Order_DoorTop? value) => value is null ? new() : new()
    {
        Count = value.Count,
        Tb_DoorTopPanels = Map(value.Tb_DoorTopPanels),
        Tb_SurfaceMetals = new PrintSurfaceMetal { FullName = value.Tb_SurfaceMetals?.FullName ?? "" },
        Tb_Monitors = Map(value.Tb_Monitors),
        Comment = value.Comment ?? "",
        Order_Panel_Attachment = value.Order_Panel_Attachment.Select(Map).ToList(),
        Cost = value.Cost,
        Order_Panel_Addition = value.Order_Panel_Addition.Select(Map).ToList(),
    };

    public static PrintDoorTopPanel Map(Tb_DoorTopPanels? value) => value is null ? new() : new()
    {
        Id = value.Id,
        Name = value.Name ?? "",
        Order_ProductStatus = Map(value.Order_ProductStatus),
    };

    public static PrintAttachment Map(Order_Panel_Attachment? value) => value is null ? new() : new()
    {
        Count = value.Count,
        Tb_Attachments = Map(value.Tb_Attachments),
        Cost = value.Cost,
    };

    public static PrintAttachmentType Map(Tb_Attachments? value) => value is null ? new() : new()
    {
        Id = value.Id,
        Name = value.Name ?? "",
    };

    public static PrintAddition Map(Order_Panel_Addition? value) => value is null ? new() : new()
    {
        Tb_Additions = Map(value.Tb_Additions),
        Cost = value.Cost,
    };

    public static PrintAdditionType Map(Tb_Additions? value) => value is null ? new() : new()
    {
        Id = value.Id,
        Name = value.Name ?? "",
    };

    public static PrintCity Map(Location_Cities? value) => value is null ? new() : new()
    {
        Location_Provinces = Map(value.Location_Provinces),
        Name = value.Name ?? "",
    };

    public static PrintProvince Map(Location_Provinces? value) => value is null ? new() : new()
    {
        Name = value.Name ?? "",
    };

}
