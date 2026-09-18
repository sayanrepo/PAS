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

[ApiController, Authorize(Roles = nameof(OPERATIONS.Sale_Print))]
[Route("api/sales/{id:int}/print")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class SalePrintController : ControllerBase
{
    [HttpGet("{kind}")]
    [ProducesResponseType<SalePrintData>(StatusCodes.Status200OK)]
    public IActionResult Print(int id, string kind)
    {
        if (id <= 0 || kind is not ("bill" or "invoice")) return NotFound();
        using var db = new PantaEntities();
        var sale = db.Sale_Sale.AsNoTracking().Include(x => x.Sale_Goods)
            .Include("Account_Users.Location_Cities.Location_Provinces")
            .SingleOrDefault(x => x.Id == id && x.StoreId == 1);
        if (sale is null) return NotFound();
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            LogManager.Log_Logs_Add((int)DB_Table.Sale_Sale, sale.DocNumber, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString(), (int)LogActivity.Print,
                kind == "bill" ? "چاپ صورتحساب فروش" : "چاپ فاکتور");
        return Ok(SalePrintDataMapper.Map(sale));
    }
}
