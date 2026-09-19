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
[Authorize(Roles = nameof(OPERATIONS.Sale))]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    private bool CanEdit(byte status) => status switch
    {
        1 => User.IsInRole(nameof(OPERATIONS.Sale_Add)),
        2 => User.IsInRole(nameof(OPERATIONS.Sale_Edit)),
        _ => false
    };

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

    [HttpGet("editor/new")]
    [Authorize(Roles = nameof(OPERATIONS.Sale_Add))]
    public ActionResult<SaleEditor> NewEditor()
    {
        using var db = new PantaEntities();
        var template = db.Sale_Sale.AsNoTracking().SingleOrDefault(x => x.Id == 0);
        if (template is null) return Problem("الگوی ثبت فروش کالا موجود نیست.");
        var form = SaleEditorData.Map(template);
        form.CustomerId = 0;
        form.StatusId = 1;
        form.FactorDate = null;
        form.Items = [new SaleItemForm { TypeId = 1, Count = 1 }];
        return Ok(Editor(form, new SaleDetail
        {
            Summary = new SaleSummary { Status = "پیش فاکتور", OrderDate = DateTime.Today }
        }, db, true));
    }

    [HttpGet("editor/{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Sale_Detail))]
    public ActionResult<SaleEditor> Editor(int id)
    {
        if (id <= 0) return NotFound();
        using var db = new PantaEntities();
        var sale = db.Sale_Sale.AsNoTracking().Include(x => x.Account_Users).Include(x => x.Account_Users1)
            .Include(x => x.Order_Status).Include(x => x.Tb_TradeTypes).Include(x => x.Sale_Goods)
            .SingleOrDefault(x => x.Id == id && x.StoreId == 1);
        if (sale is null) return NotFound(new ProblemDetails { Title = "فروش کالا یافت نشد." });
        return Ok(Editor(SaleEditorData.Map(sale), SaleEditorData.Detail(sale), db, CanEdit(sale.StatusId)));
    }

    [HttpGet("editor/customers")]
    public ActionResult<List<OrderLookup>> EditorCustomers([FromQuery] string? term)
    {
        if (!User.IsInRole(nameof(OPERATIONS.Sale_Add)) && !User.IsInRole(nameof(OPERATIONS.Sale_Edit))) return Forbid();
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
    [Authorize(Roles = nameof(OPERATIONS.Sale_Add))]
    public ActionResult<CreatedDocument> Create(SaleForm form) => Save(0, form);

    [HttpPut("editor/{id:int}")]
    public ActionResult<CreatedDocument> Update(int id, SaleForm form) => id <= 0 ? NotFound() : Save(id, form);

    private ActionResult<CreatedDocument> Save(int id, SaleForm form)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        using var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        var original = id == 0 ? null : db.Sale_Sale.Include(x => x.Sale_Goods)
            .SingleOrDefault(x => x.Id == id && x.StoreId == 1);
        if (id > 0 && original is null) return NotFound(new ProblemDetails { Title = "فروش کالا یافت نشد." });
        var persistedStatus = original?.StatusId ?? (byte)1;
        if (!SaleForm.IsEditable(persistedStatus))
            return Conflict(new ProblemDetails { Title = "این فروش کالا فقط قابل مشاهده است. وضعیت سند تغییر کرده است؛ صفحه را دوباره باز کنید." });
        if (!CanEdit(persistedStatus)) return Forbid();
        if (id == 0 && form.StatusId != (byte)OrderStatus.PishFactor)
            return BadRequest(new ProblemDetails { Title = "وضعیت فروش کالای جدید معتبر نیست." });
        if (id > 0 && form.StatusId != persistedStatus && !User.IsInRole(nameof(OPERATIONS.Sale_Edit)))
            return Forbid();
        var customer = db.Account_Users.SingleOrDefault(x => x.Id == form.CustomerId && x.Id > 0);
        if (customer is null) return BadRequest(new ProblemDetails { Title = "مشتری معتبر انتخاب کنید." });
        var template = id == 0 ? db.Sale_Sale.AsNoTracking().SingleOrDefault(x => x.Id == 0) : null;
        if (id == 0 && template is null) return Problem("الگوی ثبت فروش کالا موجود نیست.");
        var error = SaleEditorData.Validate(db, original, form);
        if (error is not null) return BadRequest(new ProblemDetails { Title = error });

        var sale = original ?? new Sale_Sale
        {
            TableId = (int)DB_Table.Sale_Sale,
            DocNumber = GenerateDocumentNumber(db),
            StatusId = 1,
            DateOrder = DateTime.Now,
            AccepterId = userId,
            StoreId = 1,
            OrderTypeId = template!.OrderTypeId,
            DeliveryCityId = customer.CityId1 ?? template.DeliveryCityId
        };
        if (id == 0) db.Sale_Sale.Add(sale);
        SaleEditorData.Apply(db, sale, form, customer);
        if (form.StatusId == (byte)OrderStatus.MojavezKhorooj && persistedStatus != form.StatusId)
            SaleEditorData.ApplyExitPermitDates(sale, DateTime.Now);
        sale.StatusId = form.StatusId;
        db.SaveChanges();
        transaction.Commit();
        LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, sale.DocNumber, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)(id == 0 ? LogActivity.Add : LogActivity.Edit),
            id == 0 ? "ثبت پیش فاکتور فروش کالا" : "ویرایش فروش کالا", sale.Cost);
        return Ok(new CreatedDocument { Id = sale.Id, DocumentNumber = sale.DocNumber });
    }

    private static int GenerateDocumentNumber(PantaEntities db)
    {
        int number;
        do number = Random.Shared.Next(800000, 900000);
        while (db.Sale_Sale.Any(x => x.DocNumber == number));
        return number;
    }

    private static SaleEditor Editor(SaleForm form, SaleDetail detail, PantaEntities db, bool canEdit) => new()
    {
        Form = form, Detail = detail, CanEdit = canEdit,
        Statuses = db.Order_Status.AsNoTracking().Where(x => x.Id == (byte)OrderStatus.PishFactor
                || x.Id == (byte)OrderStatus.DarDasteEghdam || x.Id == (byte)OrderStatus.MojavezKhorooj)
            .OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
        TradeTypes = db.Tb_TradeTypes.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
        GoodsTypes =
        [
            new OrderLookup { Id = 1, Name = "پوش باتون" },
            new OrderLookup { Id = 2, Name = "نمایشگر" },
            new OrderLookup { Id = 3, Name = "ملحقات" }
        ]
    };
}
