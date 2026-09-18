#nullable enable
using BaseSite.Models;
using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Models.Log;
using BaseSite.Models.Order;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController, Authorize(Roles = nameof(OPERATIONS.Order))]
[Route("api/orders/{id:int}/print")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class OrderPrintController : ControllerBase
{
    [HttpGet("{kind}")]
    [ProducesResponseType<OrderPrintData>(StatusCodes.Status200OK)]
    public IActionResult Print(int id, string kind)
    {
        if (id <= 0 || kind is not ("specification" or "bill" or "invoice")) return NotFound();
        var requiredRole = kind == "specification" ? nameof(OPERATIONS.Plan_Print) : nameof(OPERATIONS.Order_Print);
        if (!User.IsInRole(requiredRole)) return Forbid();

        var order = OrderManager.Order_Order_Get(id);
        if (order is null) return NotFound();

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

        return Ok(OrderPrintDataMapper.ForPrint(order, kind));
    }
}
