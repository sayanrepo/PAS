#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Payment;
using BaseSite.Models.Log;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Payment))]
[Route("api/payments")]
public sealed class PaymentsController(ILogger<PaymentsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaymentPage>> Get([FromQuery] PaymentSearch filter, CancellationToken cancellationToken)
    {
        if (filter.HasFilters && !User.IsInRole(nameof(OPERATIONS.Payment_Search))) return Forbid();
        using var db = new PantaEntities();
        var query = PaymentQueries.Apply(db.Payment_Payment.AsNoTracking(), filter);
        var count = await query.CountAsync(cancellationToken);
        var offset = filter.Page * filter.PageSize;
        var items = await PaymentQueries.Project(query.OrderByDescending(x => x.DateSanad).ThenByDescending(x => x.Id).Skip(offset).Take(filter.PageSize)).ToListAsync(cancellationToken);
        for (var i = 0; i < items.Count; i++) items[i].RowNumber = offset + i + 1;
        return Ok(new PaymentPage { Items = items, TotalCount = count });
    }


    [HttpGet("lookups")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Search) + "," + nameof(OPERATIONS.Payment_Add))]
    public ActionResult<PaymentLookups> Lookups() {
        using var db = new PantaEntities();
        return Ok(new PaymentLookups {
            Statuses = db.Payment_Status.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
            Types = db.Payment_Types.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
            Babats = db.Payment_Babats.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
            Banks = db.Payment_Banks.AsNoTracking().OrderBy(x => x.Name).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList()
        });
    }
    [HttpGet("customers")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Search) + "," + nameof(OPERATIONS.Payment_Add))]
    public async Task<ActionResult<List<OrderLookup>>> Customers([FromQuery] string? term, CancellationToken token) {
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => x.Id > 0);
        if (!User.IsInRole(nameof(OPERATIONS.Payment_Add))) query = query.Where(x => db.Payment_Payment.Any(p => p.CustomerId == x.Id));
        if (!string.IsNullOrWhiteSpace(term)) { var text = term.Trim(); query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(text)); }
        return Ok(await query.OrderBy(x => x.Name).ThenBy(x => x.Id).Take(30).Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToListAsync(token));
    }
    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Detail))]
    public async Task<ActionResult<PaymentDetail>> Detail(int id, CancellationToken token) {
        using var db = new PantaEntities();
        var query = db.Payment_Payment.AsNoTracking().Where(x => x.Id == id && x.Id > 0);
        var summary = await PaymentQueries.Project(query).SingleOrDefaultAsync(token);
        if (summary is null) return NotFound(new ProblemDetails { Title = "سند دریافتی یافت نشد." });
        var item = await query.SingleAsync(token);
        return Ok(new PaymentDetail { Summary = summary, ProjectName = item.ProjectName ?? "", BankBranchCode = item.BankBranchCode ?? "",
            ReferenceNumber = item.ShomareSanad ?? "", AccountNumber = item.ShomareHesab ?? "", Comment = item.Comment ?? "", Returned = item.Bargashti });
    }
    [HttpPost]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Add))]
    public async Task<ActionResult<CreatedDocument>> Create(NewPaymentRequest request) {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        if (!await db.Account_Users.AnyAsync(x => x.Id == request.CustomerId && x.Id > 0)
            || !await db.Payment_Types.AnyAsync(x => x.Id == request.PaymentTypeId)
            || !await db.Payment_Babats.AnyAsync(x => x.Id == request.BabatId)
            || (request.BankId.HasValue && !await db.Payment_Banks.AnyAsync(x => x.Id == request.BankId)))
            return BadRequest(new ProblemDetails { Title = "مشتری، نحوه وصول، بابت و بانک را از فهرست معتبر انتخاب کنید." });
        var saved = PaymentManager.Payment_Payment_Edit(new Payment_Payment {
            CustomerId = request.CustomerId, AccepterId = userId, PaymentTypeId = request.PaymentTypeId!.Value,
            PaymentBabatId = request.BabatId!.Value, BankId = request.BankId,
            DateSanad = request.DocumentDate, DateSarresid = request.DueDate, Amount = request.Amount,
            ProjectName = request.ProjectName, BankBranchCode = request.BankBranchCode, ShomareSanad = request.ReferenceNumber,
            ShomareHesab = request.AccountNumber, Comment = request.Comment, Bargashti = request.Returned
        }, "submit");
        try {
            LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, saved.DocNumber, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Add, "ثبت سند دریافتی", saved.Amount);
        } catch (Exception ex) { logger.LogError(ex, "Unable to record audit for received document {Id}", saved.Id); }
        return Ok(new CreatedDocument { Id = saved.Id, DocumentNumber = saved.DocNumber });
    }
}
