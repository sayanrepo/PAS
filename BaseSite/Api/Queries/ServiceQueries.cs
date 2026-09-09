#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class ServiceQueries
{
    public static IQueryable<Service_Service> Apply(IQueryable<Service_Service> query, ServiceSearch filter)
    {
        query = query.Where(x => x.Id > 0);
        if (filter.DocumentNumber.HasValue) query = query.Where(x => x.DocNumber == filter.DocumentNumber.Value);
        if (filter.CustomerId.HasValue) query = query.Where(x => x.CustomerId == filter.CustomerId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Customer))
        {
            var term = filter.Customer.Trim();
            query = query.Where(x => ((x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? "")).Contains(term));
        }
        if (filter.StatusId.HasValue) query = query.Where(x => x.StatusId == filter.StatusId.Value);
        if (filter.OrderDateFrom.HasValue)
        {
            var start = filter.OrderDateFrom.Value.Date;
            query = query.Where(x => x.DateOrder >= start);
        }
        if (filter.OrderDateTo.HasValue)
        {
            var end = filter.OrderDateTo.Value.Date.AddDays(1);
            query = query.Where(x => x.DateOrder < end);
        }
        if (filter.FactorDateFrom.HasValue)
        {
            var start = filter.FactorDateFrom.Value.Date;
            query = query.Where(x => x.DateFactor >= start);
        }
        if (filter.FactorDateTo.HasValue)
        {
            var end = filter.FactorDateTo.Value.Date.AddDays(1);
            query = query.Where(x => x.DateFactor < end);
        }
        return query;
    }

    public static IQueryable<ServiceSummary> Project(IQueryable<Service_Service> query) => query.Select(x => new ServiceSummary
    {
        Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
        Receiver = (x.Account_Users1.Name ?? "") + " " + (x.Account_Users1.LastName ?? ""),
        Customer = (x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? ""),
        OrderDate = x.DateOrder,
        // The legacy list labels DateFactor as the delivery date (ShDateFactor).
        FactorDate = x.DateFactor, Amount = x.Cost,
        Status = x.Order_Status.Name, OrderType = x.Tb_OrderTypes.Name, HasTax = x.Tax > 0
    });
}
