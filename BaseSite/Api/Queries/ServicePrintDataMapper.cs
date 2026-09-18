#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class ServicePrintDataMapper
{
    public static ServicePrintData Map(Service_Service service) => new()
    {
        DocNumber = service.DocNumber, FactorNumber = service.FactorNumber,
        StatusId = service.StatusId, ShDateFactor = service.ShDateFactor ?? "",
        Comment = service.Comment ?? "", ServiceCost = service.ServiceCost,
        Discount = service.Discount, Tax = service.Tax, DeliveryCost = service.DeliveryCost,
        Cost = service.Cost, Account_Users = OrderPrintDataMapper.Map(service.Account_Users)
    };
}
