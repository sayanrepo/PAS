using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models.DBModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Security.Cryptography;
using System.Text;

internal static class OrderEditorChecks
{
    public static void Run(Action<bool, string> check)
    {
        for (byte status = 0; status <= 11; status++)
        {
            check(OrderForm.IsEditable(status) == (status is 1 or 2), $"Server edit policy for status {status}");
            check(BaseSite.Web.Models.OrderForm.IsEditable(status) == (status is 1 or 2), $"Web edit policy for status {status}");
        }
        check(OrderForm.CanChangeStatus(1, 1) && OrderForm.CanChangeStatus(1, 2)
            && OrderForm.CanChangeStatus(2, 3) && !OrderForm.CanChangeStatus(1, 3),
            "Order status changes advance one editable stage at a time");
        var productionRequestedAt = new DateTime(2026, 9, 19, 10, 30, 0);
        var orderWithoutDeliveryDate = new Order_Order();
        OrderEditorData.ApplyProductionRequestDates(orderWithoutDeliveryDate, productionRequestedAt);
        check(orderWithoutDeliveryDate.DateDelivery == productionRequestedAt
            && orderWithoutDeliveryDate.DateFactor == productionRequestedAt.AddDays(10),
            "Production request defaults an empty delivery date to ten days after the request");
        var selectedDeliveryDate = new DateTime(2026, 10, 15);
        var orderWithDeliveryDate = new Order_Order { DateFactor = selectedDeliveryDate };
        OrderEditorData.ApplyProductionRequestDates(orderWithDeliveryDate, productionRequestedAt);
        check(orderWithDeliveryDate.DateDelivery == productionRequestedAt
            && orderWithDeliveryDate.DateFactor == selectedDeliveryDate,
            "Production request preserves the delivery date selected by the user");
        var routes = typeof(BaseSite.Web.Components.Pages.OrderDetailsPage).GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>().Select(x => x.Template).ToArray();
        check(routes.Contains("/documents/orders/new") && routes.Contains("/documents/orders/{Id:int}")
            && routes.Contains("/documents/orders/{Id:int}/edit"), "Create, view and edit resolve to one order component");

        for (byte status = 0; status <= 11; status++)
        {
            check(SaleForm.IsEditable(status) == (status is 1 or 2), $"Server goods-sale edit policy for status {status}");
            check(BaseSite.Web.Models.SaleForm.IsEditable(status) == (status is 1 or 2), $"Web goods-sale edit policy for status {status}");
        }
        check(SaleForm.CanChangeStatus(1, 1) && SaleForm.CanChangeStatus(1, 2)
            && SaleForm.CanChangeStatus(1, 7) && SaleForm.CanChangeStatus(2, 7)
            && !SaleForm.CanChangeStatus(2, 3),
            "Goods-sale status can advance from either editable stage to exit permit issued");
        var exitPermitIssuedAt = new DateTime(2026, 9, 19, 11, 45, 0);
        var saleWithoutDeliveryDate = new Sale_Sale();
        SaleEditorData.ApplyExitPermitDates(saleWithoutDeliveryDate, exitPermitIssuedAt);
        check(saleWithoutDeliveryDate.DateDelivery == exitPermitIssuedAt
            && saleWithoutDeliveryDate.DateFactor == exitPermitIssuedAt.AddDays(5),
            "Exit permit defaults an empty goods-sale delivery date to five days after issuance");
        var selectedSaleDeliveryDate = new DateTime(2026, 10, 10);
        var saleWithDeliveryDate = new Sale_Sale { DateFactor = selectedSaleDeliveryDate };
        SaleEditorData.ApplyExitPermitDates(saleWithDeliveryDate, exitPermitIssuedAt);
        check(saleWithDeliveryDate.DateDelivery == exitPermitIssuedAt
            && saleWithDeliveryDate.DateFactor == selectedSaleDeliveryDate,
            "Exit permit preserves the goods-sale delivery date selected by the user");
        var saleRoutes = typeof(BaseSite.Web.Components.Pages.SaleDetailsPage).GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>().Select(x => x.Template).ToArray();
        check(saleRoutes.Contains("/documents/sales/new") && saleRoutes.Contains("/documents/sales/{Id:int}")
            && saleRoutes.Contains("/documents/sales/{Id:int}/edit"), "Create, view and edit resolve to one goods-sale component");
        check(typeof(BaseSite.Api.Controllers.SaleActivityController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/sales/{id:int}/activity",
            "Goods-sale notes and history use the document activity endpoint");
        check(typeof(BaseSite.Api.Controllers.SalePrintController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/sales/{id:int}/print",
            "Goods-sale print data exposes the two legacy print kinds");
        var saleForm = new SaleForm
        {
            CustomerId = 1, TradeTypeId = 1, StatusId = 1, DeliveryCost = 10000, Tax = 10, Discount = 20000,
            Items = [new() { Name = "کالای نمونه", TypeId = 1, Count = 2, UnitPrice = 100000 }]
        };
        check(saleForm.Subtotal == 200000 && saleForm.TaxTotal == 18000 && saleForm.Total == 208000,
            "Goods-sale totals follow the legacy discount, tax and delivery formula");
        using (var db = new PantaEntities())
        {
            var lockedSale = new Sale_Sale { Id = 1, StatusId = 3 };
            check(SaleEditorData.Validate(db, lockedSale, saleForm) is not null,
                "Server rejects a goods-sale write after the document leaves the two editable statuses");
        }
        var salePrintController = new BaseSite.Api.Controllers.SalePrintController();
        check(salePrintController.Print(1, "unknown") is Microsoft.AspNetCore.Mvc.NotFoundResult
            && salePrintController.Print(0, "bill") is Microsoft.AspNetCore.Mvc.NotFoundResult,
            "Goods-sale print rejects unknown kinds and invalid document IDs before database access");

        for (byte status = 0; status <= 11; status++)
        {
            check(ServiceForm.IsEditable(status) == (status is 1 or 2), $"Server service edit policy for status {status}");
            check(BaseSite.Web.Models.ServiceForm.IsEditable(status) == (status is 1 or 2), $"Web service edit policy for status {status}");
        }
        var serviceRoutes = typeof(BaseSite.Web.Components.Pages.ServiceDetailsPage).GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>().Select(x => x.Template).ToArray();
        check(serviceRoutes.Contains("/documents/services/new") && serviceRoutes.Contains("/documents/services/{Id:int}")
            && serviceRoutes.Contains("/documents/services/{Id:int}/edit"), "Create, view and edit resolve to one service component");
        check(typeof(BaseSite.Api.Controllers.ServiceActivityController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/services/{id:int}/activity",
            "Service notes and history use the document activity endpoint");
        check(typeof(BaseSite.Api.Controllers.ServicePrintController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/services/{id:int}/print",
            "Service print data exposes the legacy invoice kind");
        var serviceForm = new ServiceForm { CustomerId = 1, OrderTypeId = 1, StatusId = 1, OrderDate = DateTime.Today,
            Comment = "شرح خدمات", ServiceCost = 200000, DeliveryCost = 10000, Tax = 10, Discount = 20000 };
        check(serviceForm.TaxTotal == 21000 && serviceForm.Total == 211000,
            "Service totals follow the legacy service, delivery, tax and discount formula");
        using (var db = new PantaEntities())
            check(ServiceEditorData.Validate(db, new Service_Service { Id = 1, StatusId = 3 }, serviceForm) is not null,
                "Server rejects a service write after the document leaves the two editable statuses");
        var servicePrintController = new BaseSite.Api.Controllers.ServicePrintController();
        check(servicePrintController.Print(1, "unknown") is Microsoft.AspNetCore.Mvc.NotFoundResult
            && servicePrintController.Print(0, "invoice") is Microsoft.AspNetCore.Mvc.NotFoundResult,
            "Service print rejects unknown kinds and invalid document IDs before database access");

        var paymentRoutes = typeof(BaseSite.Web.Components.Pages.PaymentDetailsPage).GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>().Select(x => x.Template).ToArray();
        check(paymentRoutes.Contains("/documents/payments/new") && paymentRoutes.Contains("/documents/payments/{Id:int}")
            && paymentRoutes.Contains("/documents/payments/{Id:int}/edit"), "Create, view and edit resolve to one received-document component");
        check(typeof(BaseSite.Api.Controllers.PaymentActivityController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/payments/{id:int}/activity",
            "Received-document notes and history use the document activity endpoint");
        check(typeof(BaseSite.Api.Controllers.PaymentPrintController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/payments/{id:int}/print",
            "Received-document print exposes both legacy receipt kinds");
        var paymentPrintController = new BaseSite.Api.Controllers.PaymentPrintController();
        check(paymentPrintController.Print(1, "unknown") is Microsoft.AspNetCore.Mvc.NotFoundResult
            && paymentPrintController.Print(0, "accounting") is Microsoft.AspNetCore.Mvc.NotFoundResult,
            "Received-document print rejects unknown kinds and invalid document IDs before database access");
        var paymentPage = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "BaseSite.Web", "Components", "Pages", "PaymentDetailsPage.razor"));
        check(paymentPage.Contains("sales-confirm", StringComparison.Ordinal)
            && paymentPage.Contains("finance-confirm", StringComparison.Ordinal)
            && paymentPage.Contains("finance-reject", StringComparison.Ordinal)
            && paymentPage.Contains("delete", StringComparison.Ordinal),
            "Received-document page exposes the legacy stage-specific confirm, reject and delete actions");

        var deliveryRoutes = typeof(BaseSite.Web.Components.Pages.DeliveryDetailsPage).GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>().Select(x => x.Template).ToArray();
        check(deliveryRoutes.Contains("/documents/deliveries/new") && deliveryRoutes.Contains("/documents/deliveries/{Id:int}")
            && deliveryRoutes.Contains("/documents/deliveries/{Id:int}/edit"), "Create, view and edit resolve to one delivery component");
        check(typeof(BaseSite.Api.Controllers.DeliveryActivityController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/deliveries/{id:int}/activity",
            "Delivery notes and history use the document activity endpoint");
        check(typeof(BaseSite.Api.Controllers.DeliveryPrintController)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/deliveries/{id:int}/print",
            "Delivery print exposes all three legacy print kinds");
        var deliveryPrintController = new BaseSite.Api.Controllers.DeliveryPrintController();
        check(deliveryPrintController.Print(1, "unknown") is Microsoft.AspNetCore.Mvc.NotFoundResult
            && deliveryPrintController.Print(0, "delivery") is Microsoft.AspNetCore.Mvc.NotFoundResult,
            "Delivery print rejects unknown kinds and invalid document IDs before database access");
        var deliveryPage = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "BaseSite.Web", "Components", "Pages", "DeliveryDetailsPage.razor"));
        check(deliveryPage.Contains("CanApprove", StringComparison.Ordinal)
            && deliveryPage.Contains("CanSend", StringComparison.Ordinal)
            && deliveryPage.Contains("ReadOnly", StringComparison.Ordinal),
            "Delivery page exposes legacy stage-specific approve and send actions and read-only states");

        var printController = typeof(BaseSite.Api.Controllers.OrderPrintController);
        check(printController.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false).Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/orders/{id:int}/print"
            && printController.GetMethod("Print")!.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), false).Cast<Microsoft.AspNetCore.Mvc.HttpGetAttribute>().Single().Template == "{kind}",
            "Order print endpoint exposes the three legacy print kinds");
        var activityController = typeof(BaseSite.Api.Controllers.OrderActivityController);
        check(activityController.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false).Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/orders/{id:int}/activity",
            "Order notes and activity history share the legacy order activity endpoint");
        var compiledViews = typeof(BaseSite.Web.Pages.Print.Orders.InvoiceModel).Assembly.GetCustomAttributes(typeof(RazorCompiledItemAttribute), false)
            .Cast<RazorCompiledItemAttribute>().Select(x => x.Identifier).ToHashSet();
        check(new[] { "/Pages/Print/Orders/Specification.cshtml", "/Pages/Print/Orders/Bill.cshtml", "/Pages/Print/Orders/Invoice.cshtml" }.All(compiledViews.Contains),
            "All three independent Razor print pages are compiled into Web");
        check(new[] { "/Pages/Print/Sales/Bill.cshtml", "/Pages/Print/Sales/Invoice.cshtml" }.All(compiledViews.Contains),
            "Both independent goods-sale Razor print pages are compiled into Web");
        check(compiledViews.Contains("/Pages/Print/Services/Invoice.cshtml"),
            "The independent service Razor print page is compiled into Web");
        check(compiledViews.Contains("/Pages/Print/Payments/Receipt.cshtml"),
            "The independent received-document Razor print page is compiled into Web");
        check(new[] { "/Pages/Print/Deliveries/Delivery.cshtml", "/Pages/Print/Deliveries/Pack.cshtml",
            "/Pages/Print/Deliveries/Panel.cshtml" }.All(compiledViews.Contains),
            "All three independent delivery Razor print pages are compiled into Web");
        check(LegacyPrintTemplatesAreUnchanged(), "Legacy print templates remain byte-for-byte equivalent after line-ending normalization");
        check(LegacySalePrintBodiesArePreserved(), "Goods-sale Razor pages preserve the complete legacy print bodies");
        check(LegacyServicePrintBodyIsPreserved(), "The service Razor page preserves the complete legacy print body");
        check(LegacyPaymentPrintBodyIsPreserved(), "Both received-document print kinds preserve the complete legacy receipt body");
        check(LegacyDeliveryPrintBodiesArePreserved(), "All three delivery Razor pages preserve the complete legacy print bodies");
        check(File.ReadAllText(Path.Combine(FindRepositoryRoot(), "BaseSite.Web", "Pages", "Print", "Sales", "Bill.cshtml"))
            .Contains("چاپ / ذخیره PDF", StringComparison.Ordinal),
            "Goods-sale bill provides the same print and PDF action as order bills");
        check(LegacyPrintAssetsAreUnchanged(), "Legacy print styles, scripts and fonts are copied byte-for-byte into the new web app");
        var printScript = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "BaseSite.Web", "wwwroot", "js", "order-print.js"));
        check(!printScript.Contains("document.write", StringComparison.Ordinal),
            "Printing never injects an HTML string into a popup");

        var sample = SampleOrderEditor.Create(2);
        var panel = sample.Form.Panels[0];
        panel.Calculate(sample.Options, 2);
        check(panel.Amount == 1900000, "In-progress order keeps the agreed model price");
        panel.Calculate(sample.Options, 1);
        check(panel.Amount == 1000000, "Pro forma order uses current catalog prices");
        panel.Attachments.Add(new() { LookupId = 1, Count = 2 });
        panel.Additions.Add(new() { LookupId = 1, Cost = 50000 });
        panel.Count = 2;
        panel.Calculate(sample.Options, 1);
        check(panel.Amount == 2250000, "Panel quantity and extras follow the legacy price formula");
        var form = new OrderForm { Panels = [panel], Deductions = [new() { LookupId = 1, Cost = 250000 }],
            DiscountRate = 10, Tax = 10, DeliveryCost = 100000 };
        check(form.DiscountTotal == 200000 && form.TaxTotal == 180000 && form.Total == 2080000,
            "Deductions, percentage discount, tax and delivery apply in legacy order");
        panel.ModelId = 0;
        panel.Calculate(sample.Options, 1);
        check(panel.Amount == 0, "An unused panel never contributes its extras to the invoice");

        var original = new Order_Order { Id = 25, StatusId = 2, CustomerId = 1,
            Order_Cabin = [new() { Id = 7, CabinPanelId = 1, Count = 1, CostCabinPanel = 500000,
                Order_Panel_Attachment = [new() { Id = 9, AttachmentId = 1, Count = 2, Cost = 50000 }] }] };
        var request = OrderEditorData.Map(original);
        check(request.Panels[0].Id == 7 && request.Panels[0].Attachments[0].Id == 9,
            "Editor round trip retains panel and attachment identities");
        request.Panels[0].Count = 2;
        request.Panels[0].Prices["Model"] = 1;
        request.Panels[0].Amount = 1;
        request.Panels[0].Attachments[0].UnitPrice = 1;
        using (var db = new PantaEntities())
            OrderEditorData.Apply(db, original, request, sample.Options);
        check(original.Cost == 1050000 && original.Order_Cabin.Single().CostCabinPanel == 500000,
            "Server ignores forged totals and snapshot prices and preserves agreed rates");
        check(original.Order_Cabin.Single().Order_Panel_Attachment.Single().Cost == 50000,
            "Server uses persisted attachment rates in the in-progress stage");

        request = OrderEditorData.Map(original);
        check(OrderEditorData.Validate(request, original, sample.Options) is null, "Valid order edit passes reference checks");
        request.Panels[0].Attachments[0].Id = 999;
        check(OrderEditorData.Validate(request, original, sample.Options) is not null, "Foreign attachment identifiers are rejected");
        request = OrderEditorData.Map(original);
        request.Panels[0].Id = 999;
        check(OrderEditorData.Validate(request, original, sample.Options) is not null, "Foreign panel identifiers are rejected");
        request = OrderEditorData.Map(original);
        original.StatusId = 3;
        check(OrderEditorData.Validate(request, original, sample.Options) is not null, "A stale editable form cannot write after production is requested");

        var web = System.Text.Json.JsonSerializer.Deserialize<BaseSite.Web.Models.OrderForm>(
            System.Text.Json.JsonSerializer.Serialize(form))!;
        check(web.Total == form.Total && web.Panels[0].Kind == form.Panels[0].Kind,
            "Web and API editor contracts round trip with identical totals");
    }

    private static bool LegacyPrintTemplatesAreUnchanged()
    {
        var expected = new Dictionary<string, string>
        {
            ["BaseSite/Views/Plan/PrintOrder.cshtml"] = "20B91FE939B6C2940AD0CBC5035E649A89E07106D1D3529FB9D69B8E97702E5C",
            ["BaseSite/Views/Order/PrintBill.cshtml"] = "B9C126DB82BB294EB10B94753B128D47C98B393E2EE59344B4672C7FCE80A1ED",
            ["BaseSite/Views/Order/PrintOrder.cshtml"] = "138FF38720DD1BD2A6C2BB5090362C9B0E5982DBB738AE1FEB1E059DE735EB4D",
            ["BaseSite/Views/Sale/PrintBill.cshtml"] = "EAF1D7B7065E80378C2AD1E57446F5A0FEDC83B3C8EDE9C6EEA0A72772352C33",
            ["BaseSite/Views/Sale/PrintSale.cshtml"] = "70104DBEB82BF03ACB527CAD01AA45D4D3BA6AFB5EAE67004B12203E21575F71",
            ["BaseSite/Views/Service/PrintService.cshtml"] = "A636B78BD8F3C2335716ED590B76950D19697FEA359FBB945935C4127E650B42",
            ["BaseSite/Views/Payment/PrintPayment.cshtml"] = "8146426B2EA66564F17B0475B2CD20A8832358BC701CCBC828C220B12DEE59DC",
            ["BaseSite/Views/Delivery/PrintDelivery.cshtml"] = "603DC1FB040870506D0698407CBFB262576453BC1E8DB9498F4B9E4DF44D8876",
            ["BaseSite/Views/Delivery/PrintDeliveryPack.cshtml"] = "E0E0C698383B2E1D47B2DABC817E599F67363324C9CC92DBE2DB3913A4FE28C0",
            ["BaseSite/Views/Delivery/PrintDeliveryPanel.cshtml"] = "7A89F3BE8B80FCDBDFECEF061AE997CB97608B6A407144B52A3567BD3B59BAE9"
        };
        var root = FindRepositoryRoot();
        return expected.All(item =>
        {
            var text = File.ReadAllText(Path.Combine(root, item.Key.Replace('/', Path.DirectorySeparatorChar))).Replace("\r\n", "\n");
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))) == item.Value;
        });
    }

    private static bool LegacyPrintAssetsAreUnchanged()
    {
        var root = FindRepositoryRoot();
        var assets = new[]
        {
            "css/bootstrap.css", "css/bootstrap-rtl.css", "css/MyStyle/PrintPA4.css",
            "css/MyStyle/PrintBill.css", "css/MyStyle/PrintLA5.css", "css/MyStyle/PrintLabel.css", "css/MyStyle/Fonts.css", "js/jquery-3.1.1.js",
            "js/printThis.js", "js/autoNumeric.js"
        };
        bool Same(string relative)
        {
            var source = Path.Combine(root, "BaseSite", "Contents", relative.Replace('/', Path.DirectorySeparatorChar));
            var target = Path.Combine(root, "BaseSite.Web", "wwwroot", "Contents", relative.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(target) && File.ReadAllBytes(source).AsSpan().SequenceEqual(File.ReadAllBytes(target));
        }
        if (!assets.All(Same)) return false;

        var fontsRoot = Path.Combine(root, "BaseSite", "Contents", "fonts");
        return Directory.EnumerateFiles(fontsRoot, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(Path.Combine(root, "BaseSite", "Contents"), path).Replace('\\', '/'))
            .All(Same);
    }

    private static bool LegacySalePrintBodiesArePreserved()
    {
        var root = FindRepositoryRoot();
        string Read(params string[] parts) => File.ReadAllText(Path.Combine([root, .. parts])).Replace("\r\n", "\n");
        static string Body(string text, string marker) => text[(text.IndexOf(marker, StringComparison.Ordinal))..];
        static string Adapt(string text) => text.Replace("Model.", "Model.Sale.")
            .Replace("ViewBag.title", "title").Replace("ViewBag.index", "index").Replace("ViewBag.sum", "sum")
            .Replace("(byte)BaseSite.Models.OrderStatus.PishFactor", "1")
            .Replace("(byte)BaseSite.Models.OrderStatus.MojavezKhorooj", "7")
            .Replace("new PersianDateTime(DateTime.Now).ToString(PersianDateTimeFormat.Date)", "PersianDates.Format(DateTime.Now)")
            .Replace("BaseSite.Models.StringExtensions.WithMaxLength", "PrintFormatting.WithMaxLength");
        var oldBill = Adapt(Body(Read("BaseSite", "Views", "Sale", "PrintBill.cshtml"), "<!DOCTYPE html>"));
        var newBill = Body(Read("BaseSite.Web", "Pages", "Print", "Sales", "Bill.cshtml"), "<!DOCTYPE html>")
            .Replace("    <link href=\"@Url.Content(\"~/css/order-print.css\")\" rel=\"stylesheet\">\n\n" +
                "    <nav class=\"print-toolbar\" aria-label=\"چاپ سند\">\n" +
                "        <button type=\"button\" onclick=\"window.print()\">چاپ / ذخیره PDF</button>\n" +
                "    </nav>\n", "");
        var oldInvoice = Adapt(Body(Read("BaseSite", "Views", "Sale", "PrintSale.cshtml"), "<!DOCTYPE html>"));
        var newInvoice = Body(Read("BaseSite.Web", "Pages", "Print", "Sales", "Invoice.cshtml"), "<!DOCTYPE html>");
        return oldBill.TrimEnd() == newBill.TrimEnd() && oldInvoice.TrimEnd() == newInvoice.TrimEnd();
    }

    private static bool LegacyServicePrintBodyIsPreserved()
    {
        var root = FindRepositoryRoot();
        string Read(params string[] parts) => File.ReadAllText(Path.Combine([root, .. parts])).Replace("\r\n", "\n");
        static string Body(string text) => text[(text.IndexOf("<!DOCTYPE html>", StringComparison.Ordinal))..];
        var oldBody = Body(Read("BaseSite", "Views", "Service", "PrintService.cshtml"))
            .Replace("Model.", "Model.Service.").Replace("ViewBag.title", "title")
            .Replace("ViewBag.index", "index").Replace("ViewBag.sum", "sum")
            .Replace("(byte)BaseSite.Models.OrderStatus.PishFactor", "1");
        var newBody = Body(Read("BaseSite.Web", "Pages", "Print", "Services", "Invoice.cshtml"))
            .Replace("    <link href=\"@Url.Content(\"~/css/order-print.css\")\" rel=\"stylesheet\">\n\n" +
                "    <nav class=\"print-toolbar\" aria-label=\"چاپ سند\">\n" +
                "        <button type=\"button\" onclick=\"window.print()\">چاپ / ذخیره PDF</button>\n" +
                "    </nav>\n", "");
        return oldBody.TrimEnd() == newBody.TrimEnd();
    }

    private static bool LegacyPaymentPrintBodyIsPreserved()
    {
        var root = FindRepositoryRoot();
        string Read(params string[] parts) => File.ReadAllText(Path.Combine([root, .. parts])).Replace("\r\n", "\n");
        static string Body(string text) => text[(text.IndexOf("<!DOCTYPE html>", StringComparison.Ordinal))..];
        var oldBody = Body(Read("BaseSite", "Views", "Payment", "PrintPayment.cshtml"))
            .Replace("ViewBag.title", "title").Replace("ViewBag.index", "index")
            .Replace("ViewBag.Accounting", "Model.Accounting")
            .Replace("@(new PersianDateTime(DateTime.Now).ToString(PersianDateTimeFormat.DateShortTime))", "@Model.Payment.PrintedAt")
            .Replace("Model.Payment_Types.Name", "Model.Payment.PaymentTypeName")
            .Replace("Model.ShomareSanad", "Model.Payment.ReferenceNumber")
            .Replace("Model.ShDateSarresid", "Model.Payment.DueDate")
            .Replace("Model.Payment_Banks.Name", "Model.Payment.BankName")
            .Replace("Model.BankBranchCode", "Model.Payment.BankBranchCode")
            .Replace("Model.ShomareHesab", "Model.Payment.AccountNumber")
            .Replace("Model.Account_Users.FullName", "Model.Payment.CustomerName")
            .Replace("User.GetFullName()", "Model.Payment.PrinterName")
            .Replace("Model.Comment", "Model.Payment.Comment")
            .Replace("Model.Amount", "Model.Payment.Amount")
            .Replace("Model.DocNumber", "Model.Payment.DocNumber")
            .Replace("@Model.ProjectName", "");
        var newBody = Body(Read("BaseSite.Web", "Pages", "Print", "Payments", "Receipt.cshtml"))
            .Replace("    <link href=\"@Url.Content(\"~/css/order-print.css\")\" rel=\"stylesheet\">\n\n" +
                "    <nav class=\"print-toolbar\" aria-label=\"چاپ سند\">\n" +
                "        <button type=\"button\" onclick=\"window.print()\">چاپ / ذخیره PDF</button>\n" +
                "    </nav>\n", "");
        return oldBody.TrimEnd() == newBody.TrimEnd();
    }

    private static bool LegacyDeliveryPrintBodiesArePreserved()
    {
        var root = FindRepositoryRoot();
        string Read(params string[] parts) => File.ReadAllText(Path.Combine([root, .. parts])).Replace("\r\n", "\n");
        static string Body(string text) => text[(text.IndexOf("<!DOCTYPE html>", StringComparison.Ordinal))..];
        static string StripToolbar(string text) => text.Replace(
            "    <link href=\"@Url.Content(\"~/css/order-print.css\")\" rel=\"stylesheet\">\n\n" +
            "    <nav class=\"print-toolbar\" aria-label=\"چاپ سند\">\n" +
            "        <button type=\"button\" onclick=\"window.print()\">چاپ / ذخیره PDF</button>\n" +
            "    </nav>\n\n", "");
        bool Same(string legacy, string current)
        {
            var oldBody = Body(Read("BaseSite", "Views", "Delivery", legacy))
                .Replace("Model.", "Model.Delivery.").Replace("ViewBag.title", "title");
            var newBody = StripToolbar(Body(Read("BaseSite.Web", "Pages", "Print", "Deliveries", current)));
            return oldBody.TrimEnd() == newBody.TrimEnd();
        }
        return Same("PrintDelivery.cshtml", "Delivery.cshtml")
            && Same("PrintDeliveryPack.cshtml", "Pack.cshtml")
            && Same("PrintDeliveryPanel.cshtml", "Panel.cshtml");
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "BaseSite", "BaseSite.Api.csproj"))) return directory.FullName;
        throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
