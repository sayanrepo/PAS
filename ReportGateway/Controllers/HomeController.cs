using Microsoft.AspNetCore.Mvc;

namespace ReportGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return $"Report Gateway is running {DateTime.Now}";
        }
    }
}
