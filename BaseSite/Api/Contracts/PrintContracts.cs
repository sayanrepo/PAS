#nullable enable

namespace BaseSite.Api.Contracts;

// Bounded print snapshots: no EF entities, reverse navigation properties or account secrets.
// Legacy property names keep the existing print templates compatible.
public sealed class OrderPrintData
{
    public int DocNumber { get; set; }
    public string ShDateDelivery { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public PrintCustomer Account_Users { get; set; } = new();
    public string ShDateFactor { get; set; } = "";
    public PrintPackType Tb_PackTypes { get; set; } = new();
    public PrintElevatorBoard Tb_ElevatorBoards { get; set; } = new();
    public string DeliveryAddress { get; set; } = "";
    public List<PrintCabin> Order_Cabin { get; set; } = [];
    public List<PrintHall> Order_Hall { get; set; } = [];
    public List<PrintDoorTop> Order_DoorTop { get; set; } = [];
    public byte StatusId { get; set; }
    public int FactorNumber { get; set; }
    public double SumCostDeduction { get; set; }
    public double SumCostAddition { get; set; }
    public double DiscountRate { get; set; }
    public double Tax { get; set; }
    public double? DeliveryCost { get; set; }
    public double Cost { get; set; }
    public string ClienteleName { get; set; } = "";
    public int AccepterId { get; set; }
    public DateTime? DateDelivery { get; set; }
}

public sealed class PrintCustomer
{
    public string FullName { get; set; } = "";
    public string EconomicalNumber { get; set; } = "";
    public string NationalNumber { get; set; } = "";
    public PrintCity Location_Cities { get; set; } = new();
    public string PostalCode1 { get; set; } = "";
    public string Address1 { get; set; } = "";
    public string Phone1 { get; set; } = "";
}

public sealed class PrintPackType { public string Name { get; set; } = ""; }
public sealed class PrintElevatorBoard { public string Name { get; set; } = ""; }

public sealed class PrintCabin
{
    public int Count { get; set; }
    public PrintCabinPanel Tb_CabinPanels { get; set; } = new();
    public PrintInstallationType Tb_InstallationTypes { get; set; } = new();
    public PrintPushButton Tb_PushButtons { get; set; } = new();
    public PrintMonitor Tb_Monitors { get; set; } = new();
    public PrintCabinSurfaceMetal Tb_CabinSurfaceMetals { get; set; } = new();
    public int FloorCount { get; set; }
    public string FloorNames { get; set; } = "";
    public int UGFloorCount { get; set; }
    public PrintSpeaker Tb_Speakers { get; set; } = new();
    public PrintEmergencyLight EmergencyLigh { get; set; } = new();
    public bool PhoneCallButton { get; set; }
    public bool DO { get; set; }
    public bool DC { get; set; }
    public string LaserCuttingText { get; set; } = "";
    public string LaserEngravingText { get; set; } = "";
    public string Comment { get; set; } = "";
    public List<PrintAttachment> Order_Panel_Attachment { get; set; } = [];
    public int MonitorId { get; set; }
    public double Cost { get; set; }
    public double CostMonitor { get; set; }
    public List<PrintAddition> Order_Panel_Addition { get; set; } = [];
}

public sealed class PrintCabinPanel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public PrintProductStatus Order_ProductStatus { get; set; } = new();
    public string Description { get; set; } = "";
}

public sealed class PrintInstallationType { public string Name { get; set; } = ""; }
public sealed class PrintPushButton { public string Name { get; set; } = ""; }
public sealed class PrintProductStatus { public string Name { get; set; } = ""; }
public sealed class PrintMonitor { public string Name { get; set; } = ""; }
public sealed class PrintCabinSurfaceMetal { public string Name { get; set; } = ""; }
public sealed class PrintSpeaker { public string Name { get; set; } = ""; }
public sealed class PrintEmergencyLight { public string Name { get; set; } = ""; }

public sealed class PrintHall
{
    public int Count { get; set; }
    public PrintHallPanel Tb_HallPanels { get; set; } = new();
    public PrintPushButton Tb_PushButtons { get; set; } = new();
    public PrintMonitor Tb_Monitors { get; set; } = new();
    public PrintHallSurfaceMetal Tb_HallSurfaceMetals { get; set; } = new();
    public int FloorCount { get; set; }
    public string FloorNames { get; set; } = "";
    public int UGFloorCount { get; set; }
    public string Comment { get; set; } = "";
    public List<PrintAttachment> Order_Panel_Attachment { get; set; } = [];
    public double Cost { get; set; }
    public List<PrintAddition> Order_Panel_Addition { get; set; } = [];
}

public sealed class PrintHallPanel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public PrintProductStatus Order_ProductStatus { get; set; } = new();
}

public sealed class PrintHallSurfaceMetal { public string Name { get; set; } = ""; }

public sealed class PrintDoorTop
{
    public int Count { get; set; }
    public PrintDoorTopPanel Tb_DoorTopPanels { get; set; } = new();
    public PrintSurfaceMetal Tb_SurfaceMetals { get; set; } = new();
    public PrintMonitor Tb_Monitors { get; set; } = new();
    public string Comment { get; set; } = "";
    public List<PrintAttachment> Order_Panel_Attachment { get; set; } = [];
    public double Cost { get; set; }
    public List<PrintAddition> Order_Panel_Addition { get; set; } = [];
}

public sealed class PrintDoorTopPanel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public PrintProductStatus Order_ProductStatus { get; set; } = new();
}

public sealed class PrintAttachment
{
    public int Count { get; set; }
    public PrintAttachmentType Tb_Attachments { get; set; } = new();
    public double Cost { get; set; }
}

public sealed class PrintAttachmentType { public int Id { get; set; } public string Name { get; set; } = ""; }
public sealed class PrintAddition { public PrintAdditionType Tb_Additions { get; set; } = new(); public double Cost { get; set; } }
public sealed class PrintAdditionType { public int Id { get; set; } public string Name { get; set; } = ""; }
public sealed class PrintCity { public PrintProvince Location_Provinces { get; set; } = new(); public string Name { get; set; } = ""; }
public sealed class PrintProvince { public string Name { get; set; } = ""; }
public sealed class PrintSurfaceMetal { public string FullName { get; set; } = ""; }

public sealed class DeliveryPrintData
{
    public int DocNumber { get; set; }
    public string ShDate { get; set; } = "";
    public string SendResponsible { get; set; } = "";
    public string RecieveResponsible { get; set; } = "";
    public string CarierAgencyName { get; set; } = "";
    public string CarierAgencyBill { get; set; } = "";
    public string VehiclePlaque { get; set; } = "";
    public string DriverName { get; set; } = "";
    public string DriverPhone { get; set; } = "";
    public string RecieverName { get; set; } = "";
    public string RecieverPhone { get; set; } = "";
    public string RecieverMobile { get; set; } = "";
    public string DestinationAddress { get; set; } = "";
    public DeliveryPrintLookup Tb_PackTypes { get; set; } = new();
    public DeliveryPrintLookup Delivery_DeliveryLocations { get; set; } = new();
    public DeliveryPrintLookup Delivery_VehicleTypes { get; set; } = new();
    public DeliveryPrintOrder? Order_Order { get; set; }
    public DeliveryPrintSale? Sale_Sale { get; set; }
    public List<DeliveryPrintItem> Items { get; set; } = [];
}

public sealed class DeliveryPrintLookup { public string Name { get; set; } = ""; }
public sealed class DeliveryPrintOrder
{
    public int DocNumber { get; set; }
    public int FactorNumber { get; set; }
    public string ProjectName { get; set; } = "";
    public PrintCustomer Account_Users { get; set; } = new();
}
public sealed class DeliveryPrintSale
{
    public int DocNumber { get; set; }
    public int FactorNumber { get; set; }
    public bool GiveBack { get; set; }
    public PrintCustomer Account_Users { get; set; } = new();
}
public sealed class DeliveryPrintItem
{
    public byte Type { get; set; }
    public bool Checked { get; set; }
    public string Model { get; set; } = "";
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public string Comment { get; set; } = "";
    public string TypeName() => Type switch
    {
        1 => "پنل داخل کابین", 11 => "ملحقات داخل کابین",
        2 => "پنل طبقات", 12 => "ملحقات طبقات",
        3 => "پنل سردرب", 13 => "ملحقات سردرب",
        4 => "فروش قطعه", _ => ""
    };
}

public sealed class PaymentPrintData
{
    public int DocNumber { get; set; }
    public double Amount { get; set; }
    public string PrintedAt { get; set; } = "";
    public string PaymentTypeName { get; set; } = "";
    public string ReferenceNumber { get; set; } = "";
    public string DueDate { get; set; } = "";
    public string BankName { get; set; } = "";
    public string BankBranchCode { get; set; } = "";
    public string AccountNumber { get; set; } = "";
    public string Comment { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string PrinterName { get; set; } = "";
}

public sealed class SalePrintData
{
    public int DocNumber { get; set; }
    public int FactorNumber { get; set; }
    public bool GiveBack { get; set; }
    public byte StatusId { get; set; }
    public string ShDateFactor { get; set; } = "";
    public double Discount { get; set; }
    public double Tax { get; set; }
    public double? DeliveryCost { get; set; }
    public double Cost { get; set; }
    public PrintCustomer Account_Users { get; set; } = new();
    public List<PrintSaleGoods> Sale_Goods { get; set; } = [];
}

public sealed class PrintSaleGoods
{
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public double Phi { get; set; }
    public string Comment { get; set; } = "";
}

public sealed class ServicePrintData
{
    public int DocNumber { get; set; }
    public int FactorNumber { get; set; }
    public byte StatusId { get; set; }
    public string ShDateFactor { get; set; } = "";
    public string Comment { get; set; } = "";
    public double ServiceCost { get; set; }
    public double Discount { get; set; }
    public double Tax { get; set; }
    public double? DeliveryCost { get; set; }
    public double Cost { get; set; }
    public PrintCustomer Account_Users { get; set; } = new();
}
