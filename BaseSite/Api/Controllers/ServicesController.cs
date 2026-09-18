#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;

using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;
using BaseSite.Models.DBModel;
using BaseSite.Models.Log;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Service))]
[Route("api/services")]
public sealed class ServicesController : ControllerBase
{
    private bool CanEdit(byte status) => status switch
    {
        1 => User.IsInRole(nameof(OPERATIONS.Service_Add)),
        2 => User.IsInRole(nameof(OPERATIONS.Service_Edit)),
        _ => false
    };
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

    [HttpGet("editor/new")]
    [Authorize(Roles = nameof(OPERATIONS.Service_Add))]
    public ActionResult<ServiceEditor> NewEditor()
    {
        using var db = new PantaEntities();
        var template = db.Service_Service.AsNoTracking().SingleOrDefault(x => x.Id == 0);
        if (template is null) return Problem("الگوی ثبت خدمات موجود نیست.");
        var form = ServiceEditorData.Map(template);
        form.CustomerId = 0; form.StatusId = 1; form.OrderDate = DateTime.Today;
        form.FactorDate = DateTime.Today.AddDays(5);
        return Ok(Editor(form, new ServiceDetail
        {
            Summary = new ServiceSummary { Status = "پیش فاکتور", OrderDate = form.OrderDate, FactorDate = form.FactorDate }
        }, db, true));
    }

    [HttpGet("editor/{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Service_Detail))]
    public ActionResult<ServiceEditor> Editor(int id)
    {
        if (id <= 0) return NotFound();
        using var db = new PantaEntities();
        var service = db.Service_Service.AsNoTracking().Include(x => x.Account_Users).Include(x => x.Account_Users1)
            .Include(x => x.Order_Status).Include(x => x.Tb_OrderTypes).SingleOrDefault(x => x.Id == id);
        if (service is null) return NotFound(new ProblemDetails { Title = "سند خدمات یافت نشد." });
        return Ok(Editor(ServiceEditorData.Map(service), ServiceEditorData.Detail(service), db, CanEdit(service.StatusId)));
    }

    [HttpGet("editor/customers")]
    public ActionResult<List<OrderLookup>> EditorCustomers([FromQuery] string? term)
    {
        if (!User.IsInRole(nameof(OPERATIONS.Service_Add)) && !User.IsInRole(nameof(OPERATIONS.Service_Edit))) return Forbid();
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => x.Id > 0);
        if (!string.IsNullOrWhiteSpace(term))
        {
            term = term.Trim();
            query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(term));
        }
        return Ok(query.OrderBy(x => x.Name).ThenBy(x => x.Id).Take(30)
            .Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToList());
    }

    [HttpPost("editor")]
    [Authorize(Roles = nameof(OPERATIONS.Service_Add))]
    public ActionResult<CreatedDocument> Create(ServiceForm form) => Save(0, form);

    [HttpPut("editor/{id:int}")]
    public ActionResult<CreatedDocument> Update(int id, ServiceForm form) => id <= 0 ? NotFound() : Save(id, form);

    private ActionResult<CreatedDocument> Save(int id, ServiceForm form)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        using var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        var original = id == 0 ? null : db.Service_Service.SingleOrDefault(x => x.Id == id);
        if (id > 0 && original is null) return NotFound(new ProblemDetails { Title = "سند خدمات یافت نشد." });
        var persistedStatus = original?.StatusId ?? (byte)1;
        if (!ServiceForm.IsEditable(persistedStatus))
            return Conflict(new ProblemDetails { Title = "این سند خدمات فقط قابل مشاهده است. وضعیت سند تغییر کرده است؛ صفحه را دوباره باز کنید." });
        if (!CanEdit(persistedStatus)) return Forbid();
        var customer = db.Account_Users.SingleOrDefault(x => x.Id == form.CustomerId && x.Id > 0);
        if (customer is null) return BadRequest(new ProblemDetails { Title = "مشتری معتبر انتخاب کنید." });
        var template = id == 0 ? db.Service_Service.AsNoTracking().SingleOrDefault(x => x.Id == 0) : null;
        if (id == 0 && template is null) return Problem("الگوی ثبت خدمات موجود نیست.");
        var error = ServiceEditorData.Validate(db, original, form);
        if (error is not null) return BadRequest(new ProblemDetails { Title = error });
        var service = original ?? new Service_Service
        {
            TableId = (int)DB_Table.Service_Service, DocNumber = GenerateDocumentNumber(db), StatusId = 1,
            AccepterId = userId, OrderTypeId = template!.OrderTypeId,
            DeliveryCityId = customer.CityId1 ?? template.DeliveryCityId
        };
        if (id == 0) db.Service_Service.Add(service);
        form.StatusId = persistedStatus;
        ServiceEditorData.Apply(service, form, customer);
        db.SaveChanges(); transaction.Commit();
        LogManager.Log_Logs_Add((int)DB_Table.Service_Service, service.DocNumber, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)(id == 0 ? LogActivity.Add : LogActivity.Edit),
            id == 0 ? "ثبت پیش فاکتور خدمات" : "ویرایش خدمات", service.Cost);
        return Ok(new CreatedDocument { Id = service.Id, DocumentNumber = service.DocNumber });
    }

    private static int GenerateDocumentNumber(PantaEntities db)
    {
        int number;
        do number = Random.Shared.Next(900000, 1000000);
        while (db.Service_Service.Any(x => x.DocNumber == number));
        return number;
    }

    private static ServiceEditor Editor(ServiceForm form, ServiceDetail detail, PantaEntities db, bool canEdit) => new()
    {
        Form = form, Detail = detail, CanEdit = canEdit,
        OrderTypes = db.Tb_OrderTypes.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList()
    };
}
