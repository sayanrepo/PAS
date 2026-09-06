#nullable enable

using BaseSite.Api.Contracts;
using BaseSite.Data;
using BaseSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(OPERATIONS.Setting_Persons))]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<CustomerSummaryDto>> Get([FromQuery] string? term = null, [FromQuery] int take = 100)
    {
        take = Math.Clamp(take, 1, 200);
        using var db = new PantaEntities();
        var query = db.Account_Users.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(x => (x.Name + " " + x.LastName).Contains(term) || x.Mobile1.Contains(term));

        return Ok(query.OrderByDescending(x => x.Id).Take(take).Select(x => new CustomerSummaryDto
        {
            Id = x.Id,
            FullName = x.Name + " " + x.LastName,
            Mobile = x.Mobile1,
            Phone = x.Phone1,
            City = x.Location_Cities.Name,
            Status = x.Account_UserStatus.Name
        }).ToList());
    }
}
