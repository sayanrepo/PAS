#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class DeliveryPrintDataMapper
{
    public static DeliveryPrintData Map(Delivery_Delivery delivery) => new()
    {
        DocNumber = delivery.DocNumber, ShDate = delivery.ShDate ?? "",
        SendResponsible = delivery.SendResponsible ?? "", RecieveResponsible = delivery.RecieveResponsible ?? "",
        CarierAgencyName = delivery.CarierAgencyName ?? "", CarierAgencyBill = delivery.CarierAgencyBill ?? "",
        VehiclePlaque = delivery.VehiclePlaque ?? "", DriverName = delivery.DriverName ?? "", DriverPhone = delivery.DriverPhone ?? "",
        RecieverName = delivery.RecieverName ?? "", RecieverPhone = delivery.RecieverPhone ?? "", RecieverMobile = delivery.RecieverMobile ?? "",
        DestinationAddress = delivery.DestinationAddress ?? "",
        Tb_PackTypes = new() { Name = delivery.Tb_PackTypes?.Name ?? "" },
        Delivery_DeliveryLocations = new() { Name = delivery.Delivery_DeliveryLocations?.Name ?? "" },
        Delivery_VehicleTypes = new() { Name = delivery.Delivery_VehicleTypes?.Name ?? "" },
        Order_Order = delivery.Order_Order is null ? null : new DeliveryPrintOrder
        {
            DocNumber = delivery.Order_Order.DocNumber, FactorNumber = delivery.Order_Order.FactorNumber,
            ProjectName = delivery.Order_Order.ProjectName ?? "", Account_Users = OrderPrintDataMapper.Map(delivery.Order_Order.Account_Users)
        },
        Sale_Sale = delivery.Sale_Sale is null ? null : new DeliveryPrintSale
        {
            DocNumber = delivery.Sale_Sale.DocNumber, FactorNumber = delivery.Sale_Sale.FactorNumber, GiveBack = delivery.Sale_Sale.GiveBack,
            Account_Users = OrderPrintDataMapper.Map(delivery.Sale_Sale.Account_Users)
        },
        Items = (delivery.Items ?? []).Select(x => new DeliveryPrintItem
        {
            Type = x.Type, Checked = x.Checked, Model = x.Model ?? "", Name = x.Name ?? "",
            Count = x.Count, Comment = x.Comment ?? ""
        }).ToList()
    };
}
