#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.DBModel;
using BaseSite.Models.Order;
using BaseSite.Models.Sale;
using BaseSite.Models.Service;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;
[ApiController, Authorize]
[Route("api/new-documents/{kind}")]
public sealed class NewDocumentsController : ControllerBase
{
    private bool CanAdd(string kind) => kind switch {
        "orders" => User.IsInRole(nameof(OPERATIONS.Order_Add)),
        "sales" => User.IsInRole(nameof(OPERATIONS.Sale_Add)),
        "services" => User.IsInRole(nameof(OPERATIONS.Service_Add)), _ => false
    };
    [HttpGet("options")]
    public ActionResult<NewDocumentOptions> Options(string kind) {
        if (!CanAdd(kind)) return Forbid();
        using var db = new PantaEntities();
        return Ok(new NewDocumentOptions {
            TradeTypes = db.Tb_TradeTypes.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList(),
            OrderTypes = db.Tb_OrderTypes.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderLookup { Id = x.Id, Name = x.Name }).ToList()
        });
    }
    [HttpGet("customers")]
    public ActionResult<List<OrderLookup>> Customers(string kind, [FromQuery] string? term) {
        if (!CanAdd(kind)) return Forbid();
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().Where(x => x.Id > 0);
        if (!string.IsNullOrWhiteSpace(term)) query = query.Where(x => ((x.Name ?? "") + " " + (x.LastName ?? "")).Contains(term.Trim()));
        return Ok(query.OrderBy(x => x.Name).ThenBy(x => x.Id).Take(30).Select(x => new OrderLookup { Id = x.Id, Name = (x.Name ?? "") + " " + (x.LastName ?? "") }).ToList());
    }
    [HttpPost]
    public async Task<ActionResult<CreatedDocument>> Create(string kind, NewDocumentRequest request) {
        if (!CanAdd(kind)) return Forbid();
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        var customer = await db.Account_Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.CustomerId && x.Id > 0);
        if (customer is null) return BadRequest(new ProblemDetails { Title = "مشتری معتبر انتخاب کنید." });
        if (request.TradeTypeId.HasValue && !db.Tb_TradeTypes.Any(x => x.Id == request.TradeTypeId)) return BadRequest();
        if (request.OrderTypeId.HasValue && !db.Tb_OrderTypes.Any(x => x.Id == request.OrderTypeId)) return BadRequest();
        if (request.Items is null || request.Items.Count > 100) return BadRequest();
        CreatedDocument result;
        int tableId;
        double amount;
        if (kind == "services") {
            var template = db.Service_Service.AsNoTracking().SingleOrDefault(x => x.Id == 0);
            if (template is null) return Problem("الگوی ثبت خدمات موجود نیست.");
            if (string.IsNullOrWhiteSpace(request.Comment)) return BadRequest(new ProblemDetails { Title = "شرح خدمات را وارد کنید." });
            if (request.Discount > (request.ServiceCost + request.DeliveryCost) * (1 + request.Tax / 100))
                return BadRequest(new ProblemDetails { Title = "تخفیف از مبلغ خدمات بیشتر است." });
            var saved = ServiceManager.Service_Service_Edit(new Service_Service {
                CustomerId = customer.Id, AccepterId = userId, OrderTypeId = request.OrderTypeId ?? template.OrderTypeId,
                DeliveryCityId = customer.CityId1 ?? template.DeliveryCityId, DeliveryAddress = request.DeliveryAddress ?? customer.Address1,
                ClienteleName = request.ClienteleName, DateOrder = DateTime.Now, DateFactor = request.FactorDate,
                ServiceCost = request.ServiceCost, DeliveryCost = request.DeliveryCost, Tax = request.Tax, Discount = request.Discount,
                Comment = request.Comment
            }, "submit");
            result = new() { Id = saved.Id, DocumentNumber = saved.DocNumber }; tableId = 21; amount = saved.Cost;
        } else if (kind == "sales") {
            if (request.Items.Count == 0) return BadRequest(new ProblemDetails { Title = "حداقل یک کالا وارد کنید." });
            var template = db.Sale_Sale.AsNoTracking().SingleOrDefault(x => x.Id == 0);
            if (template is null) return Problem("الگوی ثبت فروش کالا موجود نیست.");
            var subtotal = request.Items.Sum(x => x.Count * x.UnitPrice);
            if (request.Discount > subtotal) return BadRequest(new ProblemDetails { Title = "تخفیف از مبلغ اقلام بیشتر است." });
            var saved = SaleManager.Sale_Sale_Edit(new Sale_Sale {
                StoreId = 1, CustomerId = customer.Id, AccepterId = userId,
                OrderTypeId = request.OrderTypeId ?? template.OrderTypeId, TradeTypeId = request.TradeTypeId ?? template.TradeTypeId,
                DeliveryCityId = customer.CityId1 ?? template.DeliveryCityId, DeliveryAddress = request.DeliveryAddress ?? customer.Address1,
                ClienteleName = request.ClienteleName, DateFactor = request.FactorDate,
                DeliveryCost = request.DeliveryCost, Tax = request.Tax, Discount = request.Discount, Comment = request.Comment,
                Sale_Goods = request.Items.Select(x => new Sale_Goods { Name = x.Name, Count = x.Count, Phi = x.UnitPrice, TypeId = 0, ProductId = 0 }).ToList()
            }, "submit");
            result = new() { Id = saved.Id, DocumentNumber = saved.DocNumber }; tableId = 18; amount = saved.Cost;
        } else {
            var template = db.Order_Order.AsNoTracking().SingleOrDefault(x => x.Id == 0);
            if (template is null) return Problem("الگوی ثبت سفارش موجود نیست.");
            var saved = await OrderManager.Order_Order_Edit(new Order_Order {
                StoreId = 1, CustomerId = customer.Id, AccepterId = userId,
                OrderTypeId = request.OrderTypeId ?? template.OrderTypeId, TradeTypeId = request.TradeTypeId ?? template.TradeTypeId,
                ElevatorBoardId = template.ElevatorBoardId, PackTypeId = template.PackTypeId,
                DeliveryCityId = customer.CityId1 ?? template.DeliveryCityId, DeliveryAddress = request.DeliveryAddress ?? customer.Address1,
                ClienteleName = request.ClienteleName, ProjectName = request.ProjectName, DateFactor = request.FactorDate,
                Comment = request.Comment
            }, "submit");
            result = new() { Id = saved.Id, DocumentNumber = saved.DocNumber }; tableId = 14; amount = saved.Cost;
        }
        LogManager.Log_Logs_Add(tableId, result.DocumentNumber, userId, HttpContext.Connection.RemoteIpAddress?.ToString(),
            (int)LogActivity.Add, "ثبت پیش فاکتور", amount);
        return Ok(result);
    }
}
