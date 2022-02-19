using Microsoft.AspNetCore.Mvc;

namespace MoneyCheckWebApp.Controllers
{
    [Route("/api/token-ensurer")]
    [ApiController]
    public class TokenEnsurerController : ControllerBase
    {
        [HttpGet("ensure")]
        public IActionResult Ensure()
        {
            return Ok();
        }
    }
}