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
    private bool CanEdit(byte status) => status switch
    {
        (byte)PaymentStatus.TayidNashode => User.IsInRole(nameof(OPERATIONS.Payment_ForoshConfirm)),
        (byte)PaymentStatus.TayidForosh => User.IsInRole(nameof(OPERATIONS.Payment_MaliConfirm)),
        _ => false
    };

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

    [HttpGet("editor/new")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Add))]
    public ActionResult<PaymentEditor> NewEditor()
    {
        using var db = new PantaEntities();
        var today = DateTime.Today;
        var form = new PaymentForm
        {
            StatusId = (byte)PaymentStatus.TayidNashode,
            BabatId = db.Payment_Babats.Any(x => x.Id == 14) ? (byte)14 : db.Payment_Babats.OrderBy(x => x.Id).Select(x => (byte?)x.Id).FirstOrDefault(),
            DocumentDate = today,
            DueDate = today
        };
        return Ok(Editor(form, new PaymentDetail
        {
            Summary = new PaymentSummary
            {
                Status = db.Payment_Status.Where(x => x.Id == (byte)PaymentStatus.TayidNashode).Select(x => x.Name).FirstOrDefault() ?? "تأیید نشده",
                DocumentDate = today,
                DueDate = today
            }
        }, db, true));
    }

    [HttpGet("editor/{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Detail))]
    public ActionResult<PaymentEditor> Editor(int id)
    {
        if (id <= 0) return NotFound();
        using var db = new PantaEntities();
        var payment = db.Payment_Payment.AsNoTracking()
            .Include(x => x.Account_Users).Include(x => x.Accepter).Include(x => x.Payment_Status)
            .Include(x => x.Payment_Types).Include(x => x.Payment_Babats).Include(x => x.Payment_Banks)
            .SingleOrDefault(x => x.Id == id);
        if (payment is null) return NotFound(new ProblemDetails { Title = "سند دریافتی یافت نشد." });
        return Ok(Editor(PaymentEditorData.Map(payment), PaymentEditorData.Detail(payment), db, CanEdit(payment.StatusId)));
    }

    [HttpGet("editor/customers")]
    public ActionResult<List<OrderLookup>> EditorCustomers([FromQuery] string? term)
    {
        if (!User.IsInRole(nameof(OPERATIONS.Payment_Add))
            && !User.IsInRole(nameof(OPERATIONS.Payment_ForoshConfirm))
            && !User.IsInRole(nameof(OPERATIONS.Payment_MaliConfirm))) return Forbid();
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => x.Id > 0);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var text = term.Trim();
            query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(text));
        }
        return Ok(query.OrderBy(x => x.Name).ThenBy(x => x.Id).Take(30)
            .Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToList());
    }

    [HttpPost("editor")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Add))]
    public ActionResult<CreatedDocument> CreateEditor(PaymentForm form) => CreatePayment(form);

    [HttpPost("editor/{id:int}/transition")]
    [Authorize(Roles = nameof(OPERATIONS.Payment_Detail))]
    public ActionResult<CreatedDocument> Transition(int id, PaymentTransitionRequest request)
    {
        if (id <= 0) return NotFound();
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        using var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        var payment = db.Payment_Payment.SingleOrDefault(x => x.Id == id);
        if (payment is null) return NotFound(new ProblemDetails { Title = "سند دریافتی یافت نشد." });
        if (payment.StatusId != request.ExpectedStatusId)
            return Conflict(new ProblemDetails { Title = "وضعیت سند دریافتی تغییر کرده است؛ صفحه را دوباره باز کنید." });

        var action = request.Action.Trim().ToLowerInvariant();
        if (action == "delete")
        {
            if (payment.StatusId != (byte)PaymentStatus.TayidNashode || !User.IsInRole(nameof(OPERATIONS.Payment_Delete))) return Forbid();
            var documentNumber = payment.DocNumber;
            var amount = payment.Amount;
            db.Payment_Payment.Remove(payment);
            db.SaveChanges();
            transaction.Commit();
            TryLog(documentNumber, userId, LogActivity.Delete, "حذف سند دریافتی", amount);
            return Ok(new CreatedDocument { Id = id, DocumentNumber = documentNumber });
        }

        var permitted = action switch
        {
            "sales-confirm" => payment.StatusId == (byte)PaymentStatus.TayidNashode && User.IsInRole(nameof(OPERATIONS.Payment_ForoshConfirm)),
            "finance-confirm" or "finance-reject" => payment.StatusId == (byte)PaymentStatus.TayidForosh && User.IsInRole(nameof(OPERATIONS.Payment_MaliConfirm)),
            _ => false
        };
        if (!permitted) return Forbid();
        if (request.Form is null) return BadRequest(new ProblemDetails { Title = "اطلاعات سند دریافتی ارسال نشده است." });
        if (request.Form.StatusId != payment.StatusId)
            return Conflict(new ProblemDetails { Title = "وضعیت سند دریافتی تغییر کرده است؛ صفحه را دوباره باز کنید." });
        var validationError = PaymentEditorData.Validate(db, request.Form);
        if (validationError is not null) return BadRequest(new ProblemDetails { Title = validationError });

        PaymentEditorData.Apply(payment, request.Form);
        payment.StatusId = action switch
        {
            "sales-confirm" => (byte)PaymentStatus.TayidForosh,
            "finance-confirm" => (byte)PaymentStatus.TayidMali,
            _ => (byte)PaymentStatus.TayidNashode
        };
        db.SaveChanges();
        transaction.Commit();
        var description = action switch
        {
            "sales-confirm" => "تأیید سند دریافتی توسط واحد فروش",
            "finance-confirm" => "تأیید سند دریافتی توسط واحد مالی",
            _ => "برگشت سند دریافتی به واحد فروش"
        };
        TryLog(payment.DocNumber, userId, LogActivity.Edit, description, payment.Amount);
        return Ok(new CreatedDocument { Id = payment.Id, DocumentNumber = payment.DocNumber });
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

    private ActionResult<CreatedDocument> CreatePayment(PaymentForm form)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        using var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        var validationError = PaymentEditorData.Validate(db, form);
        if (validationError is not null) return BadRequest(new ProblemDetails { Title = validationError });
        var payment = new Payment_Payment
        {
            TableId = (int)DB_Table.Payment_Payment,
            DocNumber = GenerateDocumentNumber(db),
            StatusId = (byte)PaymentStatus.TayidNashode,
            AccepterId = userId
        };
        PaymentEditorData.Apply(payment, form);
        db.Payment_Payment.Add(payment);
        db.SaveChanges();
        transaction.Commit();
        TryLog(payment.DocNumber, userId, LogActivity.Add, "ثبت سند دریافتی", payment.Amount);
        return Ok(new CreatedDocument { Id = payment.Id, DocumentNumber = payment.DocNumber });
    }

    private PaymentEditor Editor(PaymentForm form, PaymentDetail detail, PantaEntities db, bool canEdit)
    {
        var status = form.StatusId;
        return new PaymentEditor
        {
            Form = form,
            Detail = detail,
            Lookups = new PaymentLookups
            {
                Statuses = db.Payment_Status.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
                Types = db.Payment_Types.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
                Babats = db.Payment_Babats.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
                Banks = db.Payment_Banks.AsNoTracking().OrderBy(x => x.Name).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList()
            },
            CanEdit = canEdit,
            CanSalesConfirm = status == (byte)PaymentStatus.TayidNashode && User.IsInRole(nameof(OPERATIONS.Payment_ForoshConfirm)),
            CanFinancialConfirm = status == (byte)PaymentStatus.TayidForosh && User.IsInRole(nameof(OPERATIONS.Payment_MaliConfirm)),
            CanFinancialReject = status == (byte)PaymentStatus.TayidForosh && User.IsInRole(nameof(OPERATIONS.Payment_MaliConfirm)),
            CanDelete = status == (byte)PaymentStatus.TayidNashode && User.IsInRole(nameof(OPERATIONS.Payment_Delete))
        };
    }

    private static int GenerateDocumentNumber(PantaEntities db)
    {
        int number;
        do number = 9000000 + Random.Shared.Next(100000, 1000000);
        while (db.Payment_Payment.Any(x => x.DocNumber == number));
        return number;
    }

    private void TryLog(int documentNumber, int userId, LogActivity activity, string description, double amount)
    {
        try
        {
            LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, documentNumber, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)activity, description, amount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to record audit for received document {DocumentNumber}", documentNumber);
        }
    }
}
