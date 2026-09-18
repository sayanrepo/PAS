#nullable enable
using BaseSite.Api.Queries;
using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using BaseSite.Models.Log;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace BaseSite.Api.Controllers;

[ApiController, Authorize(Roles = nameof(OPERATIONS.Payment_Print))]
[Route("api/payments/{id:int}/print")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class PaymentPrintController : ControllerBase
{
    [HttpGet("{kind}")]
    [ProducesResponseType<PaymentPrintData>(StatusCodes.Status200OK)]
    public IActionResult Print(int id, string kind)
    {
        if (id <= 0 || kind is not ("accounting" or "customer")) return NotFound();
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0) return Unauthorized();
        using var db = new PantaEntities();
        var payment = db.Payment_Payment.AsNoTracking()
            .Include(x => x.Account_Users).Include(x => x.Payment_Types).Include(x => x.Payment_Banks)
            .SingleOrDefault(x => x.Id == id);
        if (payment is null) return NotFound();
        var printerName = db.Account_Users.AsNoTracking().Where(x => x.Id == userId)
            .Select(x => (x.Name ?? "") + " " + (x.LastName ?? "")).SingleOrDefault()?.Trim() ?? "";
        LogManager.Log_Logs_Add((int)DB_Table.Payment_Payment, payment.DocNumber, userId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print,
            kind == "accounting" ? "چاپ نسخه حسابداری" : "چاپ نسخه مشتری");
        return Ok(PaymentPrintDataMapper.Map(payment, printerName));
    }
}
