#nullable enable
using BaseSite.Models;
using BaseSite.Models.Log;
using BaseSite.Models.Order;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController, Authorize(Roles = nameof(OPERATIONS.Order))]
[Route("api/orders/{id:int}/print")]
public sealed class OrderPrintController : Controller
{
    [HttpGet("{kind}")]
    public IActionResult Print(int id, string kind)
    {
        var requiredRole = kind == "specification" ? nameof(OPERATIONS.Plan_Print) : nameof(OPERATIONS.Order_Print);
        if (!User.IsInRole(requiredRole)) return Forbid();
        if (kind is not ("specification" or "bill" or "invoice")) return NotFound();

        var order = OrderManager.Order_Order_Get(id);
        if (order is null || id <= 0) return NotFound();

        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            var description = kind switch
            {
                "specification" => "چاپ سند بزرگ",
                "bill" => "چاپ صورتحساب فروش",
                _ => "چاپ فاکتور"
            };
            LogManager.Log_Logs_Add((int)DB_Table.Order_Order, order.DocNumber, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, description);
        }

        return kind switch
        {
            "specification" => View("~/Views/Plan/PrintOrder.cshtml", order),
            "bill" => View("~/Views/Order/PrintBill.cshtml", order),
            _ => View("~/Views/Order/PrintOrder.cshtml", order)
        };
    }
}
