using Microsoft.AspNetCore.Mvc;

namespace BaseSite.Api.Controllers;

[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        service = "BaseSite.Api",
        framework = ".NET 10",
        status = "ready",
        utcTime = DateTimeOffset.UtcNow
    });
}
