#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class DeliveryEditorData
{
    public static DeliveryForm Map(Delivery_Delivery delivery, List<DeliveryItem> items) => new()
    {
        Kind = delivery.OrderId.HasValue ? "orders" : "sales",
        SourceId = delivery.OrderId ?? delivery.SaleId ?? 0,
        StatusId = delivery.StatusId,
        PackTypeId = delivery.PackTypeId,
        DeliveryLocationId = delivery.DeliveryLocationId,
        VehicleTypeId = delivery.VehicleTypeId,
        SendResponsible = delivery.SendResponsible,
        RecieveResponsible = delivery.RecieveResponsible,
        RecieverName = delivery.RecieverName,
        RecieverPhone = delivery.RecieverPhone,
        RecieverMobile = delivery.RecieverMobile,
        CarierAgencyName = delivery.CarierAgencyName,
        CarierAgencyBill = delivery.CarierAgencyBill,
        VehiclePlaque = delivery.VehiclePlaque,
        DriverName = delivery.DriverName,
        DriverPhone = delivery.DriverPhone,
        DestinationType = delivery.DestinationType ?? 2,
        DestinationAddress = delivery.DestinationAddress,
        Items = items
    };

    public static DeliveryDetail Detail(Delivery_Delivery delivery, DeliverySummary summary, List<DeliveryItem> items) => new()
    {
        Summary = summary,
        Items = items.Where(x => x.Selected).ToList(),
        Fields = new()
        {
            ["شماره سند مبدأ"] = (delivery.Order_Order?.DocNumber ?? delivery.Sale_Sale?.DocNumber ?? 0).ToString(),
            ["نوع بسته‌بندی"] = delivery.Tb_PackTypes?.Name ?? "",
            ["محل تحویل"] = delivery.Delivery_DeliveryLocations?.Name ?? "",
            ["وسیله حمل"] = delivery.Delivery_VehicleTypes?.Name ?? "",
            ["مسئول ارسال"] = delivery.SendResponsible ?? "",
            ["مسئول دریافت"] = delivery.RecieveResponsible ?? "",
            ["تحویل گیرنده"] = delivery.RecieverName ?? "",
            ["تلفن"] = delivery.RecieverPhone ?? "",
            ["موبایل"] = delivery.RecieverMobile ?? "",
            ["آدرس تحویل"] = delivery.DestinationAddress ?? "",
            ["باربری"] = delivery.CarierAgencyName ?? "",
            ["شماره بارنامه"] = delivery.CarierAgencyBill ?? "",
            ["پلاک خودرو"] = delivery.VehiclePlaque ?? "",
            ["راننده"] = delivery.DriverName ?? "",
            ["تلفن راننده"] = delivery.DriverPhone ?? ""
        }
    };

    public static string? Validate(PantaEntities db, DeliveryForm form, byte status)
    {
        if (form.StatusId != status) return "وضعیت سند تحویل تغییر کرده است؛ صفحه را دوباره باز کنید.";
        if (!db.Tb_PackTypes.Any(x => x.Id == form.PackTypeId)) return "نوع بسته‌بندی معتبر انتخاب کنید.";
        if (!db.Delivery_DeliveryLocations.Any(x => x.Id == form.DeliveryLocationId)) return "محل ارسال معتبر انتخاب کنید.";
        if (!db.Delivery_VehicleTypes.Any(x => x.Id == form.VehicleTypeId)) return "وسیله حمل معتبر انتخاب کنید.";
        if (form.DestinationType is not (1 or 2)) return "محل تحویل کالا معتبر نیست.";
        return null;
    }

    public static void Apply(Delivery_Delivery delivery, DeliveryForm form, byte status)
    {
        delivery.DeliveryLocationId = form.DeliveryLocationId;
        delivery.VehicleTypeId = form.VehicleTypeId;
        delivery.CarierAgencyName = form.CarierAgencyName?.Trim();
        delivery.CarierAgencyBill = form.CarierAgencyBill?.Trim();
        delivery.VehiclePlaque = form.VehiclePlaque?.Trim();
        delivery.DriverName = form.DriverName?.Trim();
        delivery.DriverPhone = form.DriverPhone?.Trim();
        if (status != (byte)DeliveryStatus.SaderShode) return;
        delivery.PackTypeId = form.PackTypeId;
        delivery.SendResponsible = form.SendResponsible?.Trim();
        delivery.RecieveResponsible = form.RecieveResponsible?.Trim();
        delivery.RecieverName = form.RecieverName?.Trim();
        delivery.RecieverPhone = form.RecieverPhone?.Trim();
        delivery.RecieverMobile = form.RecieverMobile?.Trim();
        delivery.DestinationType = form.DestinationType;
        delivery.DestinationAddress = form.DestinationAddress?.Trim();
    }
}
