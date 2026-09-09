#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.Order;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Order))]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<OrderPage>> Get([FromQuery] OrderSearch filter, CancellationToken cancellationToken)
    {
        if (filter.HasFilters && !User.IsInRole(nameof(OPERATIONS.Order_Search))) return Forbid();
        using var db = new PantaEntities();
        var query = OrderQueries.Apply(db.Order_Order.AsNoTracking(), filter);
        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await OrderQueries.Project(query.OrderByDescending(x => x.Id).Skip(offset).Take(filter.PageSize)).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;
        return Ok(new OrderPage { Items = items, TotalCount = count });
    }

    [HttpGet("lookups")]
    [Authorize(Roles = nameof(OPERATIONS.Order_Search))]
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
    [Authorize(Roles = nameof(OPERATIONS.Order_Search))]
    public async Task<ActionResult<List<OrderLookup>>> Customers([FromQuery] string? term, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => db.Order_Order.Any(o => o.CustomerId == x.Id));
        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(term));
        }
        return Ok(await query.OrderBy(x => x.Name).ThenBy(x => x.LastName).Take(30)
            .Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Order_Detail))]
    public ActionResult<OrderDetail> Detail(int id)
    {
        if (id <= 0) return NotFound(new ProblemDetails { Title = "سفارش یافت نشد." });
        var order = OrderManager.Order_Order_Get(id);
        if (order is null) return NotFound(new ProblemDetails { Title = "سفارش یافت نشد." });
        return Ok(OrderDetails.Map(order));
    }
}
