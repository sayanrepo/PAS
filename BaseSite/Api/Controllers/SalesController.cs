#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;

using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Sale))]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SalePage>> Get([FromQuery] SaleSearch filter, CancellationToken cancellationToken)
    {
        if (filter.HasFilters && !User.IsInRole(nameof(OPERATIONS.Sale_Search))) return Forbid();
        using var db = new PantaEntities();
        var query = SaleQueries.Apply(db.Sale_Sale.AsNoTracking(), filter);
        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await SaleQueries.Project(query.OrderByDescending(x => x.Id).Skip(offset).Take(filter.PageSize)).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;
        return Ok(new SalePage { Items = items, TotalCount = count });
    }

    [HttpGet("lookups")]
    [Authorize(Roles = nameof(OPERATIONS.Sale_Search))]
    public ActionResult<OrderLookups> Lookups()
    {
        using var db = new PantaEntities();
        return Ok(new OrderLookups
        {
            Statuses = db.Order_Status.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
            TradeTypes = db.Tb_TradeTypes.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList()
        });
    }

    [HttpGet("customers")]
    [Authorize(Roles = nameof(OPERATIONS.Sale_Search))]
    public async Task<ActionResult<List<OrderLookup>>> Customers([FromQuery] string? term, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => db.Sale_Sale.Any(o => o.CustomerId == x.Id && o.StoreId == 1));
        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(term));
        }
        return Ok(await query.OrderBy(x => x.Name).ThenBy(x => x.LastName).Take(30)
            .Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToListAsync(cancellationToken));
    }


    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Sale_Detail))]
    public async Task<ActionResult<SaleDetail>> Detail(int id, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var sale = await db.Sale_Sale.AsNoTracking().Include(x => x.Account_Users).Include(x => x.Account_Users1)
            .Include(x => x.Order_Status).Include(x => x.Tb_TradeTypes).Include(x => x.Sale_Goods)
            .SingleOrDefaultAsync(x => x.Id == id && x.StoreId == 1, cancellationToken);
        if (sale is null) return NotFound(new ProblemDetails { Title = "فروش کالا یافت نشد." });
        return Ok(new SaleDetail {
            Summary = SaleQueries.Project(new[] { sale }.AsQueryable()).Single(),
            ClienteleName = sale.ClienteleName ?? "", DeliveryAddress = sale.DeliveryAddress ?? "",
            Comment = sale.Comment ?? "", DeliveryCost = sale.DeliveryCost ?? 0, TaxPercent = sale.Tax, Discount = sale.Discount,
            Items = sale.Sale_Goods.OrderBy(x => x.Id).Select(x => new SaleItem {
                Name = x.Name ?? "", Count = x.Count, UnitPrice = x.Phi, Amount = x.Count * x.Phi,
                Comment = x.Comment ?? "", DeliveryComment = x.DeliveryComment ?? ""
            }).ToList()
        });
    }
}
