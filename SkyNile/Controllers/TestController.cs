using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("OK")]
        public async Task<IActionResult> ok()
        {
            return Ok("You're goddamn right");
        }


        // [Authorize(Roles = "Crew")]
        [HttpGet("AuthOK")]
        public async Task<IActionResult> AuthOk()
        {
            return Ok("You're goddamn right");
        }
    }
}
