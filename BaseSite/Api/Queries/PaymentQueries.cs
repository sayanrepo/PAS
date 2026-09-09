#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class PaymentQueries
{
    public static IQueryable<Payment_Payment> Apply(IQueryable<Payment_Payment> query, PaymentSearch filter)
    {
        query = query.Where(x => x.Id > 0);
        if (filter.DocumentNumber.HasValue) query = query.Where(x => x.DocNumber == filter.DocumentNumber.Value);
        if (filter.CustomerId.HasValue) query = query.Where(x => x.CustomerId == filter.CustomerId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Customer))
        {
            var term = filter.Customer.Trim();
            query = query.Where(x => ((x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? "")).Contains(term));
        }
        if (filter.PaymentTypeId.HasValue) query = query.Where(x => x.PaymentTypeId == filter.PaymentTypeId.Value);
        if (filter.BabatId.HasValue) query = query.Where(x => x.PaymentBabatId == filter.BabatId.Value);
        if (filter.StatusId.HasValue) query = query.Where(x => x.StatusId == filter.StatusId.Value);
        if (filter.DocumentDateFrom.HasValue)
        {
            var start = filter.DocumentDateFrom.Value.Date;
            query = query.Where(x => x.DateSanad >= start);
        }
        if (filter.DocumentDateTo.HasValue)
        {
            var end = filter.DocumentDateTo.Value.Date.AddDays(1);
            query = query.Where(x => x.DateSanad < end);
        }
        if (filter.DueDateFrom.HasValue)
        {
            var start = filter.DueDateFrom.Value.Date;
            query = query.Where(x => x.DateSarresid >= start);
        }
        if (filter.DueDateTo.HasValue)
        {
            var end = filter.DueDateTo.Value.Date.AddDays(1);
            query = query.Where(x => x.DateSarresid < end);
        }
        return query;
    }

    public static IQueryable<PaymentSummary> Project(IQueryable<Payment_Payment> query) => query.Select(x => new PaymentSummary
    {
        Id = x.Id, DocumentNumber = x.DocNumber, 
        Receiver = (x.Accepter.Name ?? "") + " " + (x.Accepter.LastName ?? ""),
        Customer = (x.Account_Users.Name ?? "") + " " + (x.Account_Users.LastName ?? ""),
        DocumentDate = x.DateSanad,
        DueDate = x.DateSarresid, Amount = x.Amount,
        Status = x.Payment_Status.Name, PaymentType = x.Payment_Types.Name, Babat = x.Payment_Babats.Name, Bank = x.Payment_Banks == null ? "" : x.Payment_Banks.Name
    });
}
