#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class OrderQueries
{
    public static IQueryable<Order_Order> Apply(IQueryable<Order_Order> query, OrderSearch filter)
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
        if (filter.TradeTypeId.HasValue) query = query.Where(x => x.TradeTypeId == filter.TradeTypeId.Value);
        if (!string.IsNullOrWhiteSpace(filter.ProjectName))
        {
            var project = filter.ProjectName.Trim();
            query = query.Where(x => x.ProjectName != null && x.ProjectName.Contains(project));
        }
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

    public static IQueryable<OrderSummary> Project(IQueryable<Order_Order> query) => query.Select(x => new OrderSummary
    {
        Id = x.Id, DocumentNumber = x.DocNumber, FactorNumber = x.FactorNumber,
        Receiver = (x.Account_Users1.Name ?? "") + " " + (x.Account_Users1.LastName ?? ""),
        Customer = (x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? ""),
        ProjectName = x.ProjectName ?? "", OrderDate = x.DateOrder,
        // The legacy list labels DateFactor as the delivery date (ShDateFactor).
        FactorDate = x.DateFactor, Amount = x.Cost,
        Status = x.Order_Status.Name, TradeType = x.Tb_TradeTypes.Name, HasTax = x.Tax > 0
    });
}
