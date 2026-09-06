using BaseSite.Api.Contracts;
using BaseSite.Data;
using Microsoft.AspNetCore.Mvc;

namespace BaseSite.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    [HttpGet]
    public ActionResult<DashboardSummaryDto> Get()
    {
        using var db = new PantaEntities();
        return Ok(new DashboardSummaryDto
        {
            Orders = db.Order_Order.Count(),
            Sales = db.Sale_Sale.Count(),
            Deliveries = db.Delivery_Delivery.Count(),
            Payments = db.Payment_Payment.Count(),
            Services = db.Service_Service.Count(),
            Customers = db.Account_Users.Count(),
            Activities = db.CRM_Activity.Count()
        });
    }
}
