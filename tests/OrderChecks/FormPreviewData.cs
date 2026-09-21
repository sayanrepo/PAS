using BaseSite.Web.Models;

// Synthetic, disconnected data for reviewing the shared forms in --preview mode.
internal static class FormPreviewData
{
    private static List<OrderLookup> Options(string name) => [new() { Id = 1, Name = name }];
    public static object? Get(string path)
    {
        if (path.EndsWith("/customers") && path != "/api/customers" && !path.StartsWith("/api/orders/"))
            return Options("مشتری نمونه");
        if (path.StartsWith("/api/sales/editor/")) return new SaleEditor
        {
            CanEdit = true, Statuses = Options("پیش فاکتور"), TradeTypes = Options("فروش"), GoodsTypes = Options("کالا"),
            Form = new() { CustomerId = 1, TradeTypeId = 1, FactorDate = DateTime.Today, ClienteleName = "سفارش‌دهنده نمونه", DeliveryAddress = "آدرس نمونه", Tax = 10,
                Items = [new() { Name = "کالای نمونه", Count = 2, UnitPrice = 1000000 }] },
            Detail = new() { Summary = new() { DocumentNumber = 1001, Customer = "مشتری نمونه", Status = "پیش فاکتور", OrderDate = DateTime.Today } }
        };
        if (path.StartsWith("/api/services/editor/")) return new ServiceEditor
        {
            CanEdit = true, OrderTypes = Options("خدمات"),
            Form = new() { CustomerId = 1, OrderTypeId = 1, OrderDate = DateTime.Today, FactorDate = DateTime.Today.AddDays(2), ServiceCost = 1000000, Tax = 10, Comment = "شرح خدمات نمونه" },
            Detail = new() { Summary = new() { DocumentNumber = 1002, Customer = "مشتری نمونه", Status = "پیش فاکتور" } }
        };
        if (path.StartsWith("/api/payments/editor/")) return new PaymentEditor
        {
            CanEdit = true, CanSalesConfirm = true,
            Lookups = new() { Types = Options("چک"), Babats = Options("فروش"), Banks = Options("بانک نمونه") },
            Form = new() { CustomerId = 1, PaymentTypeId = 1, BabatId = 1, BankId = 1, DocumentDate = DateTime.Today, DueDate = DateTime.Today.AddDays(7), Amount = 1000000 },
            Detail = new() { Summary = new() { DocumentNumber = 1003, Customer = "مشتری نمونه", Status = "ثبت اولیه" } }
        };
        if (path.StartsWith("/api/deliveries/editor/")) return new DeliveryEditor
        {
            CanEdit = true, CanSave = true, CustomerAddress = "آدرس مشتری نمونه", ProjectAddress = "آدرس پروژه نمونه",
            Lookups = new() { Packs = Options("کارتن"), Locations = Options("محل پروژه"), Vehicles = Options("وانت") },
            Form = new() { SourceId = 1, PackTypeId = 1, DestinationAddress = "آدرس پروژه نمونه", Items = [new() { Id = 1, Name = "پنل نمونه", Model = "مدل نمونه", TypeName = "پنل", Count = 2, Selected = true }] },
            Detail = new() { Summary = new() { DocumentNumber = 1004, Customer = "مشتری نمونه", ProjectName = "پروژه نمونه", Status = "صادر شده", Date = DateTime.Today } }
        };
        if (path == "/api/customers/lookups") return new PersonOrganizationLookups
        {
            PersonTypes = [new() { Id = 1, Name = "حقیقی" }, new() { Id = 2, Name = "حقوقی" }],
            PartnerTypes = [new() { Id = 2, Name = "مشتری" }], FindoutWays = [new() { Id = 1, Name = "معرفی" }],
            Countries = [new() { Id = 1, Name = "ایران" }, new() { Id = 2, Name = "کشور نمونه" }],
            Provinces = [new() { Id = 1, ParentId = 1, Name = "تهران" }, new() { Id = 2, ParentId = 2, Name = "استان نمونه" }],
            Cities = [new() { Id = 1, ParentId = 1, Name = "تهران" }, new() { Id = 2, ParentId = 2, Name = "شهر نمونه" }]
        };
        if (path == "/api/customers/1") return new PersonOrganizationDetails { Id = 1, Name = "شخص نمونه", FindoutWay = "معرفی", CityId1 = 1 };
        if (path.StartsWith("/api/basic/") && !path.Contains("compatibility")) return new BasicTable
        {
            Key = path.Split('/').Last(), Title = "اطلاعات پایه نمونه", CanAdd = true, CanEdit = true,
            Fields = [new() { Key = "Name", Label = "نام", CanAdd = true, CanEdit = true }, new() { Key = "Cost", Label = "قیمت", Kind = "number", CanAdd = true, CanEdit = true }],
            Rows = [new() { Id = 1, Values = new() { ["Name"] = "مدل نمونه", ["Cost"] = "1000000" } }]
        };
        return null;
    }
}
