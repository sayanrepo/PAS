#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;

namespace BaseSite.Api.Queries;

public static class SalePrintDataMapper
{
    public static SalePrintData Map(Sale_Sale sale) => new()
    {
        DocNumber = sale.DocNumber, FactorNumber = sale.FactorNumber, GiveBack = sale.GiveBack,
        StatusId = sale.StatusId, ShDateFactor = sale.ShDateFactor ?? "", Discount = sale.Discount,
        Tax = sale.Tax, DeliveryCost = sale.DeliveryCost, Cost = sale.Cost,
        Account_Users = OrderPrintDataMapper.Map(sale.Account_Users),
        Sale_Goods = sale.Sale_Goods.OrderBy(x => x.Id).Select(x => new PrintSaleGoods
        {
            Name = x.Name ?? "", Count = x.Count, Phi = x.Phi, Comment = x.Comment ?? ""
        }).ToList()
    };
}
