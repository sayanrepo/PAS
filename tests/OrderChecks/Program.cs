using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Models.DBModel;
using BaseSite.Web.Services;
using System.ComponentModel.DataAnnotations;

if (args.Contains("--preview"))
{
    await Preview.RunAsync(args);
    return;
}

var passed = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine("PASS: " + message);
    passed++;
}
var date = new DateTime(2026, 9, 8);
var rows = SampleOrders.Create();
Check(PersianDates.Parse("۱۴۰۵/۰۶/۱۷", "تاریخ") == date, "Persian digits parse to Gregorian API date");
Check(PersianDates.Parse("١٤٠٥-٠٦-١٧", "تاریخ") == date, "Arabic digits and alternate separator are accepted");
Check(PersianDates.Format(date) == "1405/06/17", "Dates display using Persian calendar");
Check(PersianDates.Parse(" ", "تاریخ") is null, "Empty dates clear the filter");
try { PersianDates.Parse("۱۴۰۴/۱۲/۳۰", "تاریخ"); throw new Exception("Invalid date accepted"); }
catch (InvalidOperationException) { Check(true, "Non-leap Esfand 30 is rejected"); }
var range = new OrderSearch { OrderDateFrom = date, OrderDateTo = date };
Check(OrderQueries.Apply(rows, range).Count() == 45, "Date range includes late times on its last day and excludes the next day");
range = new() { FactorDateFrom = date.AddDays(3), FactorDateTo = date.AddDays(3) };
Check(OrderQueries.Apply(rows, range).Count() == 23, "Invoice filter uses DateFactor instead of production DateDelivery");
var combined = new OrderSearch { CustomerId = 1, StatusId = 1, TradeTypeId = 1, ProjectName = "برج", DocumentNumber = 700001 };
Check(OrderQueries.Apply(rows, combined).Single().Id == 1, "All filters combine rather than overriding each other");
Check(OrderQueries.Apply(rows, new() { Customer = "مشتری نمونه", ProjectName = "برج" }).Count() == 45, "Customer and project text filters combine");
var projected = OrderQueries.Project(rows.Where(x => x.Id == 1)).Single();
Check(projected.Receiver == "کارشناس فروش" && projected.ProjectName == "برج نمونه" && projected.HasTax && projected.FactorDate == date.AddDays(3), "Legacy columns map to receiver, project, tax marker and invoice date");
var sorted = rows.OrderByDescending(x => x.Id);
var first = OrderQueries.Project(sorted.Take(20)).Select(x => x.Id).ToArray();
var second = OrderQueries.Project(sorted.Skip(20).Take(20)).Select(x => x.Id).ToArray();
Check(first.Length == 20 && second.Length == 20 && !first.Intersect(second).Any(), "Server pages contain different records in stable order");
Check(rows.Count() == 46, "Count represents all matching rows, not one page");
var errors = new List<ValidationResult>();
var invalidRange = new OrderSearch { OrderDateFrom = date.AddDays(1), OrderDateTo = date };
Check(!Validator.TryValidateObject(invalidRange, new ValidationContext(invalidRange), errors, true), "Reversed range is rejected");
var detail = OrderDetails.Map(rows.First());
Check(detail.Summary.Id == 1 && detail.Panels.Single().Kind == "پنل کابین" && detail.Panels.Single().Extras.Count == 1, "Details preserve the selected order and its panel attachments");
var sales = Enumerable.Range(1, 45).Select(i => new Sale_Sale {
    Id = i, StoreId = i == 45 ? (byte)2 : (byte)1, DocNumber = 800000 + i, CustomerId = 1,
    StatusId = 1, TradeTypeId = 1, GiveBack = i == 1, Tax = 10, DateOrder = date.AddHours(23),
    DateFactor = date.AddDays(2), DateDelivery = date.AddDays(5),
    Account_Users = new() { Name = "مشتری", LastName = "نمونه" },
    Account_Users1 = new() { Name = "کارشناس", LastName = "فروش" },
    Order_Status = new() { Name = "پیش فاکتور" }, Tb_TradeTypes = new() { Name = "فروش" }
}).AsQueryable();
Check(SaleQueries.Apply(sales, new()).Count() == 44, "Goods sales exclude store documents");
Check(SaleQueries.Apply(sales, new() { DocumentNumber = 800001, Customer = "نمونه", StatusId = 1, TradeTypeId = 1 }).Single().Id == 1, "Sales filters combine");
Check(SaleQueries.Apply(sales, new() { OrderDateFrom = date, OrderDateTo = date }).Count() == 44, "Sales date range includes the entire last day");
Check(SaleQueries.Apply(sales, new() { FactorDateFrom = date.AddDays(2), FactorDateTo = date.AddDays(2) }).Count() == 44, "Sales delivery filter follows legacy DateFactor");
var saleRow = SaleQueries.Project(sales.Where(x => x.Id == 1)).Single();
Check(saleRow.GiveBack && saleRow.HasTax && saleRow.Receiver == "کارشناس فروش", "Sales return, tax and receiver columns map correctly");
Check(SaleQueries.Apply(sales, new()).OrderByDescending(x => x.Id).Skip(20).Take(20).Count() == 20, "Sales retain records beyond the first page");
var services = new[] {
    new Service_Service { Id = 1, DocNumber = 900001, CustomerId = 1, StatusId = 1,
        DateOrder = date.AddHours(23), DateFactor = date.AddDays(3), DateDelivery = date.AddDays(9),
        Account_Users = new() { Name = "مشتری", LastName = "نمونه" },
        Account_Users1 = new() { Name = "کارشناس", LastName = "خدمات" },
        Order_Status = new() { Name = "پیش فاکتور" }, Tb_OrderTypes = new() { Name = "نرمال" } },
    new Service_Service { Id = 0, DateOrder = date }
}.AsQueryable();
Check(ServiceQueries.Apply(services, new()).Count() == 1, "Service template record is excluded");
Check(ServiceQueries.Apply(services, new() { DocumentNumber = 900001, CustomerId = 1, StatusId = 1, OrderDateFrom = date, OrderDateTo = date }).Count() == 1, "Services combine filters and include the final day");
Check(ServiceQueries.Apply(services, new() { FactorDateFrom = date.AddDays(3), FactorDateTo = date.AddDays(3) }).Count() == 1, "Services use the legacy delivery date");
Check(ServiceQueries.Project(services.Where(x => x.Id == 1)).Single().OrderType == "نرمال", "Services display order type rather than trade type");
var request = new NewDocumentRequest { CustomerId = 0, Tax = 101, DeliveryCost = -1 };
Check(!Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true), "Creation rejects invalid customer and financial inputs");
Check(PersianDates.Parse("1403/12/30", "تاریخ") == new DateTime(2025, 3, 20), "Persian leap-year final day is selectable");
var payments = Enumerable.Range(0, 43).Select(i => new Payment_Payment {
    Id = i, DocNumber = 930000 + i, CustomerId = 1, PaymentTypeId = 2, PaymentBabatId = 14, StatusId = 1,
    DateSanad = i == 42 ? date.AddDays(1) : date.AddHours(23), DateSarresid = date.AddDays(3).AddHours(23), Amount = 44000000,
    Account_Users = new() { Name = "مشتری", LastName = "نمونه" }, Accepter = new() { Name = "ثبت", LastName = "کننده" },
    Payment_Status = new() { Name = "تأیید نشده" }, Payment_Types = new() { Name = "حواله" }, Payment_Babats = new() { Name = "سفارش" }
}).AsQueryable();
Check(PaymentQueries.Apply(payments, new()).Count() == 42, "Received documents exclude template records");
Check(PaymentQueries.Apply(payments, new() { DocumentNumber = 930001, CustomerId = 1, Customer = "نمونه", PaymentTypeId = 2, BabatId = 14, StatusId = 1 }).Single().Id == 1, "Received document filters combine");
Check(PaymentQueries.Apply(payments, new() { DocumentDateFrom = date, DocumentDateTo = date }).Count() == 41, "Document date includes the last evening and excludes the next day");
Check(PaymentQueries.Apply(payments, new() { DueDateFrom = date.AddDays(3), DueDateTo = date.AddDays(3) }).Count() == 42, "Due date filters the separate maturity date");
var paymentRow = PaymentQueries.Project(payments.Where(x => x.Id == 1)).Single();
Check(paymentRow.Receiver == "ثبت کننده" && paymentRow.Amount == 44000000 && paymentRow.Bank == "" && paymentRow.Babat == "سفارش" && paymentRow.PaymentType == "حواله", "Received document columns map correctly with optional bank");
Check(PaymentQueries.Apply(payments, new()).OrderByDescending(x => x.DateSanad).ThenByDescending(x => x.Id).Skip(40).Count() == 2, "Received documents remain available beyond two pages");
var badPaymentRange = new PaymentSearch { DueDateFrom = date.AddDays(1), DueDateTo = date };
Check(!Validator.TryValidateObject(badPaymentRange, new ValidationContext(badPaymentRange), new List<ValidationResult>(), true), "Reversed maturity date range is rejected");
var newPayment = new NewPaymentRequest();
Check(!Validator.TryValidateObject(newPayment, new ValidationContext(newPayment), new List<ValidationResult>(), true), "Received document creation requires customer, method, purpose, dates and positive amount");
newPayment = new() { CustomerId = 1, PaymentTypeId = 2, BabatId = 14, DocumentDate = date, DueDate = date.AddDays(3), Amount = 44000000 };
Check(Validator.TryValidateObject(newPayment, new ValidationContext(newPayment), new List<ValidationResult>(), true), "Valid received document accepts an optional bank");
var deliveries = new[] {
 new Delivery_Delivery { Id=1,DocNumber=5700001,OrderId=1,Date=date.AddHours(23),StatusId=1,
  Order_Order=rows.First(),Delivery_Status=new(){Name="صادر شده"} },
 new Delivery_Delivery { Id=2,DocNumber=6800001,SaleId=1,Date=date.AddDays(1),StatusId=2,
  Sale_Sale=sales.First(),Delivery_Status=new(){Name="تأیید شده"} },
 new Delivery_Delivery {Id=0,DocNumber=0}
}.AsQueryable();
Check(DeliveryQueries.Apply(deliveries,new()).Count()==2,"Deliveries exclude template records");
Check(DeliveryQueries.Apply(deliveries,new(){DocumentNumber=700001,CustomerId=1,Customer="نمونه",StatusId=1,DateFrom=date,DateTo=date}).Single().Id==1,"Delivery filters combine and accept legacy source document suffix");
Check(DeliveryQueries.Apply(deliveries,new(){DocumentNumber=5700001}).Single().Id==1,"Delivery filter accepts full delivery document number");
Check(DeliveryQueries.Apply(deliveries,new(){DateFrom=date,DateTo=date}).Single().Id==1,"Delivery date range includes the final evening");
var deliveryRows=DeliveryQueries.Project(deliveries.Where(x=>x.Id>0)).ToList();
Check(deliveryRows[0].Kind=="سفارش" && deliveryRows[0].ProjectName=="برج نمونه" && deliveryRows[1].Kind=="فروش کالا" && deliveryRows[1].ProjectName=="-","Delivery columns distinguish orders and goods sales");
var sourceSale=new Sale_Sale {Sale_Goods=[
 new(){Id=1,Name="قلم آزاد",Count=2},
 new(){Id=2,Name="قلم تحویل",Count=3,DeliveryId=7},
 new(){Id=3,Name="تحویل دیگر",Count=1,DeliveryId=8}]};
Check(DeliveryItems.ForSale(sourceSale).Single().Id==1,"New deliveries only offer unassigned goods");
Check(DeliveryItems.ForSale(sourceSale,7).Single().Id==2,"Delivery details only contain their own assigned goods");
var badDeliveryRange=new DeliverySearch{DateFrom=date.AddDays(1),DateTo=date};
Check(!Validator.TryValidateObject(badDeliveryRange,new ValidationContext(badDeliveryRange),new List<ValidationResult>(),true),"Reversed delivery date range is rejected");
Check(ReportCatalog.All.Count==16 && ReportCatalog.All.Select(x=>x.Info.Key).Distinct().Count()==16,"Report catalog includes all sixteen legacy reports with unique routes");
var reportUser=new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity([
 new(System.Security.Claims.ClaimTypes.Role,"Report"),new(System.Security.Claims.ClaimTypes.Role,"Report_KPI")],"test"));
Check(ReportCatalog.All.Count(x=>ReportCatalog.CanRead(reportUser,x))==5,"KPI access does not grant access to sales or financial reports");
var noReportUser=new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity([new(System.Security.Claims.ClaimTypes.Role,"Report_KPI")],"test"));
Check(!ReportCatalog.All.Any(x=>ReportCatalog.CanRead(noReportUser,x)),"Report section permission is required in addition to report permission");
var invalidReportFilter=new ReportFilter{DateFrom=date.AddDays(1),DateTo=date};
Check(!Validator.TryValidateObject(invalidReportFilter,new ValidationContext(invalidReportFilter),new List<ValidationResult>(),true),"Reports reject reversed date ranges");
Check(BaseSite.Api.Controllers.ReportsController.ShDate(date)=="1405/06/17","Report procedure parameters use the legacy Persian date format");
Check(typeof(BaseSite.Api.Controllers.ReportsController).Assembly.GetManifestResourceNames().Contains("BaseSite.Api.Queries.ReportColumns.json"),"Persian report column labels are included in published assemblies");
Console.WriteLine($"{passed} document checks passed; no database was accessed.");

internal static class SampleOrders
{
    public static IQueryable<Order_Order> Create() => Enumerable.Range(1, 46).Select(i => new Order_Order
    {
        Id = i, DocNumber = 700000 + i, FactorNumber = i, CustomerId = 1, StatusId = (byte)(i % 2 == 1 ? 1 : 2), TradeTypeId = 1,
        Account_Users = new() { Name = "مشتری", LastName = "نمونه" }, Account_Users1 = new() { Name = "کارشناس", LastName = "فروش" },
        ProjectName = i <= 45 ? "برج نمونه" : "پروژه دیگر", DateOrder = new DateTime(2026, 9, i <= 45 ? 8 : 9, 23, 59, 0),
        DateFactor = i % 2 == 1 ? new DateTime(2026, 9, 11) : null, DateDelivery = new DateTime(2026, 9, 20), Cost = 1000000 + i * 10000, Tax = i % 2 == 1 ? 10 : 0,
        Order_Status = new() { Id = (byte)(i % 2 == 1 ? 1 : 2), Name = i % 2 == 1 ? "پیش فاکتور" : "در جریان تولید" },
        Tb_TradeTypes = new() { Id = 1, Name = "فروش" },
        Order_Cabin = [new() { Id = 1, DocNumber = 1700000 + i, Count = 1, Cost = 1000000, Tb_CabinPanels = new() { Name = "پنل نمونه" },
            Order_Panel_Attachment = [new() { Count = 2, Cost = 20000, Tb_Attachments = new() { Name = "متعلقات نمونه" } }] }]
    }).AsQueryable();
}
