#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class SaleEditorData
{
    public static SaleForm Map(Sale_Sale sale) => new()
    {
        CustomerId = sale.CustomerId,
        ClienteleName = sale.ClienteleName ?? "",
        TradeTypeId = sale.TradeTypeId,
        GiveBack = sale.GiveBack,
        DeliveryAddress = sale.DeliveryAddress ?? "",
        Comment = sale.Comment ?? "",
        FactorDate = sale.DateFactor,
        DeliveryCost = sale.DeliveryCost ?? 0,
        Tax = sale.Tax,
        Discount = sale.Discount,
        StatusId = sale.StatusId,
        Items = sale.Sale_Goods.OrderBy(x => x.Id).Select(x => new SaleItemForm
        {
            Id = x.Id, TypeId = x.TypeId, ProductId = x.ProductId, Name = x.Name ?? "",
            Count = x.Count, UnitPrice = x.Phi, Comment = x.Comment ?? ""
        }).ToList()
    };

    public static SaleDetail Detail(Sale_Sale sale) => new()
    {
        Summary = SaleQueries.Project(new[] { sale }.AsQueryable()).Single(),
        ClienteleName = sale.ClienteleName ?? "", DeliveryAddress = sale.DeliveryAddress ?? "",
        Comment = sale.Comment ?? "", DeliveryCost = sale.DeliveryCost ?? 0,
        TaxPercent = sale.Tax, Discount = sale.Discount,
        Items = sale.Sale_Goods.OrderBy(x => x.Id).Select(x => new SaleItem
        {
            Name = x.Name ?? "", Count = x.Count, UnitPrice = x.Phi, Amount = x.Count * x.Phi,
            Comment = x.Comment ?? "", DeliveryComment = x.DeliveryComment ?? ""
        }).ToList()
    };

    public static string? Validate(PantaEntities db, Sale_Sale? original, SaleForm form)
    {
        if (original is not null && !SaleForm.IsEditable(original.StatusId))
            return "این فروش کالا فقط قابل مشاهده است.";
        if (original is not null && !SaleForm.CanChangeStatus(original.StatusId, form.StatusId))
            return "وضعیت فروش کالا تغییر کرده است؛ صفحه را دوباره باز کنید.";
        if (!db.Tb_TradeTypes.Any(x => x.Id == form.TradeTypeId)) return "نوع معامله معتبر نیست.";
        if (form.Items.Count is < 1 or > 100) return "حداقل یک کالا و حداکثر ۱۰۰ کالا مجاز است.";
        if (form.Items.Any(x => x.TypeId is < 1 or > 3)) return "نوع کالا معتبر نیست.";
        if (form.Items.Any(x => string.IsNullOrWhiteSpace(x.Name))) return "شرح همه کالاها را وارد کنید.";
        if (form.Discount > form.Subtotal) return "تخفیف از مبلغ اقلام بیشتر است.";
        if (original is not null)
        {
            var validIds = original.Sale_Goods.Select(x => x.Id).ToHashSet();
            if (form.Items.Any(x => x.Id > 0 && !validIds.Contains(x.Id))) return "یکی از اقلام متعلق به این سند نیست.";
            if (form.Items.Where(x => x.Id > 0).GroupBy(x => x.Id).Any(x => x.Count() > 1)) return "یک قلم تکراری ارسال شده است.";
        }
        return null;
    }

    public static void ApplyExitPermitDates(Sale_Sale sale, DateTime issuedAt)
    {
        sale.DateDelivery = issuedAt;
        sale.DateFactor ??= issuedAt.AddDays(5);
    }

    public static void Apply(PantaEntities db, Sale_Sale sale, SaleForm form, Account_Users customer)
    {
        if (sale.CustomerId != form.CustomerId) sale.DeliveryCityId = customer.CityId1 ?? sale.DeliveryCityId;
        sale.CustomerId = form.CustomerId;
        sale.ClienteleName = form.ClienteleName.Trim();
        sale.TradeTypeId = form.TradeTypeId;
        sale.GiveBack = form.GiveBack;
        sale.DeliveryAddress = form.DeliveryAddress.Trim();
        sale.Comment = form.Comment.Trim();
        sale.DateFactor = form.FactorDate;
        sale.DeliveryCost = form.DeliveryCost;
        sale.Tax = form.Tax;
        sale.Discount = form.Discount;

        var requested = form.Items.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet();
        foreach (var removed in sale.Sale_Goods.Where(x => !requested.Contains(x.Id)).ToList())
            db.Sale_Goods.Remove(removed);
        foreach (var item in form.Items)
        {
            var entity = item.Id == 0 ? new Sale_Goods { Sale_Sale = sale } : sale.Sale_Goods.Single(x => x.Id == item.Id);
            if (item.Id == 0) sale.Sale_Goods.Add(entity);
            entity.TypeId = item.TypeId;
            entity.ProductId = Math.Max(0, item.ProductId);
            entity.Name = item.Name.Trim();
            entity.Count = item.Count;
            entity.Phi = item.UnitPrice;
            entity.Comment = item.Comment.Trim();
        }
        sale.Cost = form.Total;
    }
}
