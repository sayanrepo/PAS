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

[ApiController, Authorize(Roles = nameof(OPERATIONS.Service_Print))]
[Route("api/services/{id:int}/print")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class ServicePrintController : ControllerBase
{
    [HttpGet("{kind}")]
    [ProducesResponseType<ServicePrintData>(StatusCodes.Status200OK)]
    public IActionResult Print(int id, string kind)
    {
        if (id <= 0 || kind != "invoice") return NotFound();
        using var db = new PantaEntities();
        var service = db.Service_Service.AsNoTracking().Include(x => x.Account_Users).SingleOrDefault(x => x.Id == id);
        if (service is null) return NotFound();
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            LogManager.Log_Logs_Add((int)DB_Table.Service_Service, service.DocNumber, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print, "چاپ فاکتور خدمات");
        return Ok(ServicePrintDataMapper.Map(service));
    }
}
