#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;

using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Service))]
[Route("api/services")]
public sealed class ServicesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ServicePage>> Get([FromQuery] ServiceSearch filter, CancellationToken cancellationToken)
    {
        if (filter.HasFilters && !User.IsInRole(nameof(OPERATIONS.Service_Search))) return Forbid();
        using var db = new PantaEntities();
        var query = ServiceQueries.Apply(db.Service_Service.AsNoTracking(), filter);
        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await ServiceQueries.Project(query.OrderByDescending(x => x.Id).Skip(offset).Take(filter.PageSize)).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;
        return Ok(new ServicePage { Items = items, TotalCount = count });
    }

    [HttpGet("lookups")]
    [Authorize(Roles = nameof(OPERATIONS.Service_Search))]
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
    [Authorize(Roles = nameof(OPERATIONS.Service_Search))]
    public async Task<ActionResult<List<OrderLookup>>> Customers([FromQuery] string? term, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => db.Service_Service.Any(o => o.CustomerId == x.Id));
        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(term));
        }
        return Ok(await query.OrderBy(x => x.Name).ThenBy(x => x.LastName).Take(30)
            .Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToListAsync(cancellationToken));
    }



    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Service_Detail))]
    public async Task<ActionResult<ServiceDetail>> Detail(int id, CancellationToken cancellationToken)
    {
        using var db = new PantaEntities();
        var item = await db.Service_Service.AsNoTracking().Include(x => x.Account_Users).Include(x => x.Account_Users1)
            .Include(x => x.Order_Status).Include(x => x.Tb_OrderTypes).SingleOrDefaultAsync(x => x.Id == id && x.Id > 0, cancellationToken);
        if (item is null) return NotFound(new ProblemDetails { Title = "سند خدمات یافت نشد." });
        return Ok(new ServiceDetail {
            Summary = ServiceQueries.Project(new[] { item }.AsQueryable()).Single(),
            ClienteleName = item.ClienteleName ?? "", DeliveryAddress = item.DeliveryAddress ?? "",
            Comment = item.Comment ?? "", DeliveryCost = item.DeliveryCost ?? 0, TaxPercent = item.Tax,
            Discount = item.Discount, ServiceCost = item.ServiceCost
        });
    }
}
