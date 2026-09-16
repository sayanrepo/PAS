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
        var routes = typeof(BaseSite.Web.Components.Pages.OrderDetailsPage).GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>().Select(x => x.Template).ToArray();
        check(routes.Contains("/documents/orders/new") && routes.Contains("/documents/orders/{Id:int}")
            && routes.Contains("/documents/orders/{Id:int}/edit"), "Create, view and edit resolve to one order component");

        var printController = typeof(BaseSite.Api.Controllers.OrderPrintController);
        check(printController.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false).Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/orders/{id:int}/print"
            && printController.GetMethod("Print")!.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), false).Cast<Microsoft.AspNetCore.Mvc.HttpGetAttribute>().Single().Template == "{kind}",
            "Order print endpoint exposes the three legacy print kinds");
        var activityController = typeof(BaseSite.Api.Controllers.OrderActivityController);
        check(activityController.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false).Cast<Microsoft.AspNetCore.Mvc.RouteAttribute>().Single().Template == "api/orders/{id:int}/activity",
            "Order notes and activity history share the legacy order activity endpoint");
        var compiledViews = printController.Assembly.GetCustomAttributes(typeof(RazorCompiledItemAttribute), false)
            .Cast<RazorCompiledItemAttribute>().Select(x => x.Identifier).ToHashSet();
        check(new[] { "/Views/Plan/PrintOrder.cshtml", "/Views/Order/PrintBill.cshtml", "/Views/Order/PrintOrder.cshtml" }.All(compiledViews.Contains),
            "All three legacy print templates are compiled into the API");
        check(LegacyPrintTemplatesAreUnchanged(), "Legacy print templates remain byte-for-byte equivalent after line-ending normalization");

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
            ["BaseSite/Views/Order/PrintOrder.cshtml"] = "138FF38720DD1BD2A6C2BB5090362C9B0E5982DBB738AE1FEB1E059DE735EB4D"
        };
        var root = FindRepositoryRoot();
        return expected.All(item =>
        {
            var text = File.ReadAllText(Path.Combine(root, item.Key.Replace('/', Path.DirectorySeparatorChar))).Replace("\r\n", "\n");
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))) == item.Value;
        });
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "BaseSite", "BaseSite.Api.csproj"))) return directory.FullName;
        throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
