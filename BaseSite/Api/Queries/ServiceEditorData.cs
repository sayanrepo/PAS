#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class ServiceEditorData
{
    public static ServiceForm Map(Service_Service service) => new()
    {
        CustomerId = service.CustomerId, ClienteleName = service.ClienteleName ?? "",
        OrderTypeId = service.OrderTypeId, DeliveryAddress = service.DeliveryAddress ?? "",
        Comment = service.Comment ?? "", OrderDate = service.DateOrder, FactorDate = service.DateFactor,
        ServiceCost = service.ServiceCost, DeliveryCost = service.DeliveryCost ?? 0,
        Tax = service.Tax, Discount = service.Discount, StatusId = service.StatusId
    };

    public static ServiceDetail Detail(Service_Service service) => new()
    {
        Summary = ServiceQueries.Project(new[] { service }.AsQueryable()).Single(),
        ClienteleName = service.ClienteleName ?? "", DeliveryAddress = service.DeliveryAddress ?? "",
        Comment = service.Comment ?? "", DeliveryCost = service.DeliveryCost ?? 0,
        TaxPercent = service.Tax, Discount = service.Discount, ServiceCost = service.ServiceCost
    };

    public static string? Validate(PantaEntities db, Service_Service? original, ServiceForm form)
    {
        if (original is not null && !ServiceForm.IsEditable(original.StatusId)) return "این سند خدمات فقط قابل مشاهده است.";
        if (original is not null && form.StatusId != original.StatusId) return "وضعیت سند خدمات تغییر کرده است؛ صفحه را دوباره باز کنید.";
        if (!db.Tb_OrderTypes.Any(x => x.Id == form.OrderTypeId)) return "نوع سفارش معتبر نیست.";
        if (!form.OrderDate.HasValue) return "تاریخ سفارش را وارد کنید.";
        if (string.IsNullOrWhiteSpace(form.Comment)) return "شرح خدمات را وارد کنید.";
        if (form.Discount > form.ServiceCost + form.DeliveryCost + form.TaxTotal) return "تخفیف از مبلغ خدمات بیشتر است.";
        return null;
    }

    public static void Apply(Service_Service service, ServiceForm form, Account_Users customer)
    {
        if (service.CustomerId != form.CustomerId) service.DeliveryCityId = customer.CityId1 ?? service.DeliveryCityId;
        service.CustomerId = form.CustomerId; service.ClienteleName = form.ClienteleName.Trim();
        service.OrderTypeId = form.OrderTypeId; service.DeliveryAddress = form.DeliveryAddress.Trim();
        service.Comment = form.Comment.Trim(); service.DateOrder = form.OrderDate;
        service.DateDelivery = form.OrderDate; service.DateFactor = form.FactorDate;
        service.ServiceCost = form.ServiceCost; service.DeliveryCost = form.DeliveryCost;
        service.Tax = form.Tax; service.Discount = form.Discount; service.Cost = form.Total;
    }
}
