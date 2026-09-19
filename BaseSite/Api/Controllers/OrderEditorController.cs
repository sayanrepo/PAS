#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController, Authorize(Roles = nameof(OPERATIONS.Order))]
[Route("api/orders/editor")]
public sealed class OrderEditorController : ControllerBase
{
    private bool CanEdit(byte status) => OrderForm.IsEditable(status)
        && User.IsInRole(nameof(OPERATIONS.Order_Add))
        && (status == 1 || User.IsInRole(nameof(OPERATIONS.Order_Edit_Factor)));

    [HttpGet("new")]
    [Authorize(Roles = nameof(OPERATIONS.Order_Add))]
    public ActionResult<OrderEditor> New()
    {
        using var db = new PantaEntities();
        var template = OrderManager.Order_Order_Get(0);
        if (template is null) return Problem("الگوی ثبت سفارش موجود نیست.");
        var editor = new OrderEditor { Form = OrderEditorData.Map(template), Options = OrderEditorData.Options(db), CanEdit = true };
        editor.Form.CustomerId = 0;
        editor.Form.StatusId = 1;
        editor.Form.FactorDate = null;
        editor.Form.Deductions.Clear();
        foreach (var panel in editor.Form.Panels)
        {
            panel.Id = 0; panel.Count = 1; panel.DocumentNumber = 0;
            panel.Attachments.Clear(); panel.Additions.Clear();
        }
        editor.Detail.Summary.OrderDate = DateTime.Today;
        editor.Detail.Summary.Status = "پیش فاکتور";
        return Ok(editor);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Order_Detail))]
    public ActionResult<OrderEditor> Get(int id)
    {
        if (id <= 0) return NotFound();
        var order = OrderManager.Order_Order_Get(id);
        if (order is null) return NotFound(new ProblemDetails { Title = "سفارش یافت نشد." });
        using var db = new PantaEntities();
        return Ok(new OrderEditor { Form = OrderEditorData.Map(order), Detail = OrderDetails.Map(order),
            Options = OrderEditorData.Options(db), CanEdit = CanEdit(order.StatusId) });
    }

    [HttpGet("customers")]
    [Authorize(Roles = nameof(OPERATIONS.Order_Add))]
    public ActionResult<List<OrderLookup>> Customers([FromQuery] string? term)
    {
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

    [HttpPost]
    [Authorize(Roles = nameof(OPERATIONS.Order_Add))]
    public ActionResult<CreatedDocument> Create(OrderForm form) => Save(0, form);

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(OPERATIONS.Order_Add))]
    public ActionResult<CreatedDocument> Update(int id, OrderForm form) =>
        id <= 0 ? NotFound() : Save(id, form);

    private ActionResult<CreatedDocument> Save(int id, OrderForm form)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        // Keep the persisted status check and write in one transaction.
        using var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        var original = db.Order_Order
            .Include("Order_Cabin.Order_Panel_Attachment").Include("Order_Cabin.Order_Panel_Addition")
            .Include("Order_Hall.Order_Panel_Attachment").Include("Order_Hall.Order_Panel_Addition")
            .Include("Order_DoorTop.Order_Panel_Attachment").Include("Order_DoorTop.Order_Panel_Addition")
            .Include(x => x.Order_Deduction).SingleOrDefault(x => x.Id == id);
        if (original is null) return id == 0 ? Problem("الگوی ثبت سفارش موجود نیست.") : NotFound();
        if (id > 0 && !OrderForm.IsEditable(original.StatusId))
            return Conflict(new ProblemDetails { Title = "این سفارش فقط قابل مشاهده است. وضعیت سفارش تغییر کرده است؛ صفحه را دوباره باز کنید." });
        if (!CanEdit(id == 0 ? (byte)1 : original.StatusId)) return Forbid();
        if (id == 0 && form.StatusId != (byte)OrderStatus.PishFactor)
            return BadRequest(new ProblemDetails { Title = "وضعیت سفارش جدید معتبر نیست." });
        if (id > 0 && form.StatusId != original.StatusId && !User.IsInRole(nameof(OPERATIONS.Order_Edit_Factor)))
            return Forbid();
        var customer = db.Account_Users.SingleOrDefault(x => x.Id == form.CustomerId && x.Id > 0);
        if (customer is null) return BadRequest(new ProblemDetails { Title = "مشتری معتبر انتخاب کنید." });
        var order = original;
        if (id == 0)
        {
            order = new Order_Order { TableId = 14, DocNumber = OrderManager.Order_GenerateDocNumber(), StatusId = 1,
                DateOrder = DateTime.Now, AccepterId = userId, StoreId = 1, OrderTypeId = original.OrderTypeId,
                DeliveryCityId = customer.CityId1 ?? original.DeliveryCityId };
            db.Order_Order.Add(order);
        }
        var options = OrderEditorData.Options(db);
        var error = OrderEditorData.Validate(form, order, options);
        if (error is not null) return BadRequest(new ProblemDetails { Title = error });
        if (order.CustomerId != form.CustomerId) order.DeliveryCityId = customer.CityId1 ?? order.DeliveryCityId;
        try { OrderEditorData.Apply(db, order, form, options); }
        catch (ArgumentException ex) { return BadRequest(new ProblemDetails { Title = ex.Message }); }
        if (form.StatusId == (byte)OrderStatus.DarkhasteTolid && order.StatusId != form.StatusId)
            OrderEditorData.ApplyProductionRequestDates(order, DateTime.Now);
        order.StatusId = form.StatusId;
        db.SaveChanges();
        transaction.Commit();
        LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)(id == 0 ? LogActivity.Add : LogActivity.Edit),
            "ثبت سفارش", order.Cost);
        return Ok(new CreatedDocument { Id = order.Id, DocumentNumber = order.DocNumber });
    }
}
