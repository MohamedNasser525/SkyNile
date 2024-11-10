using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("OK")]
        public async Task<IActionResult> ok()
        {
            return Ok("You're goddamn right");
        }
    }
}
