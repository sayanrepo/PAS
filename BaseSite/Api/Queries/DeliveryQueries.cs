#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;
namespace BaseSite.Api.Queries;
public static class DeliveryQueries {
 public static IQueryable<Delivery_Delivery> Apply(IQueryable<Delivery_Delivery> query, DeliverySearch f) {
  query = query.Where(x => x.Id > 0);
  if(f.DocumentNumber.HasValue) {
   // Legacy search accepts the source document suffix as well as the full delivery number.
   long divisor = (long)Math.Pow(10, f.DocumentNumber.Value.ToString().Length);
   var number = f.DocumentNumber.Value;
   query = query.Where(x => ((long)x.DocNumber - number) % divisor == 0);
  }
  if(f.StatusId.HasValue) query = query.Where(x=>x.StatusId == f.StatusId.Value);
  if(f.CustomerId.HasValue) query = query.Where(x => x.OrderId.HasValue ? x.Order_Order.CustomerId == f.CustomerId : x.Sale_Sale.CustomerId == f.CustomerId);
  if(!string.IsNullOrWhiteSpace(f.Customer)) {
   var term=f.Customer.Trim();
   query=query.Where(x => x.OrderId.HasValue
    ? ((x.Order_Order.Account_Users.Name ?? "")+" "+(x.Order_Order.Account_Users.LastName ?? "")).Contains(term)
    : ((x.Sale_Sale.Account_Users.Name ?? "")+" "+(x.Sale_Sale.Account_Users.LastName ?? "")).Contains(term));
  }
  if(f.DateFrom.HasValue) { var start=f.DateFrom.Value.Date; query=query.Where(x=>x.Date>=start); }
  if(f.DateTo.HasValue) { var end=f.DateTo.Value.Date.AddDays(1); query=query.Where(x=>x.Date<end); }
  return query;
 }
 public static IQueryable<DeliverySummary> Project(IQueryable<Delivery_Delivery> query) => query.Select(x=>new DeliverySummary {
  Id=x.Id, DocumentNumber=x.DocNumber, Date=x.Date, Kind=x.OrderId.HasValue ? "سفارش" : x.SaleId.HasValue ? "فروش کالا" : "-",
  Customer=x.OrderId.HasValue ? (x.Order_Order.Account_Users.Name ?? "")+" "+(x.Order_Order.Account_Users.LastName ?? "") :
   x.SaleId.HasValue ? (x.Sale_Sale.Account_Users.Name ?? "")+" "+(x.Sale_Sale.Account_Users.LastName ?? "") : "-",
  ProjectName=x.OrderId.HasValue ? x.Order_Order.ProjectName : "-",
  Status=x.Delivery_Status.Name, FactorNumber=x.OrderId.HasValue ? x.Order_Order.FactorNumber : x.SaleId.HasValue ? x.Sale_Sale.FactorNumber : 0
 });
}
