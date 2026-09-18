#nullable enable
using BaseSite.Api.Queries;
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.Delivery;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController, Authorize(Roles = nameof(OPERATIONS.Delivery_Print))]
[Route("api/deliveries/{id:int}/print")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class DeliveryPrintController : ControllerBase
{
    [HttpGet("{kind}")]
    [ProducesResponseType<DeliveryPrintData>(StatusCodes.Status200OK)]
    public IActionResult Print(int id, string kind)
    {
        if (id <= 0 || kind is not ("delivery" or "pack" or "panel")) return NotFound();
        using (var db = new PantaEntities())
            if (!db.Delivery_Delivery.AsNoTracking().Any(x => x.Id == id)) return NotFound();
        var delivery = DeliveryManager.Delivery_Delivery_Get(id);
        if (delivery is null) return NotFound();
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            LogManager.Log_Logs_Add((int)DB_Table.Delivery_Delivery, delivery.DocNumber, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, kind switch
                {
                    "delivery" => "چاپ فرم تحویل کالا", "pack" => "چاپ فرم محموله", _ => "چاپ لیبل پنل"
                });
        return Ok(DeliveryPrintDataMapper.Map(delivery));
    }
}
