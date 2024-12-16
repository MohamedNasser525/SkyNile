using BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTOS;


namespace SkyNile.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AuthorizationController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public AuthorizationController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }
        [HttpGet]
        public async Task<IActionResult> getallrole()
        {
            var roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            if (roles.Count == 0) { return NotFound("not roles found"); }
            return Ok(roles);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getrole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return BadRequest("Invalid user ID");

            var role = await _userManager.GetRolesAsync(user);

            return Ok(new { user.Id, user.UserName, role });
        }

        [HttpPost]
        public async Task<IActionResult> add([FromForm] newrole model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Invalid user ID or Role");

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return BadRequest("User already assigned to this role");

            var result = await _userManager.AddToRoleAsync(user, model.Role);


            if (!result.Succeeded) { return BadRequest("Sonething went wrong"); }
            return Ok("role is added");
        }

        [HttpDelete]
        public async Task<IActionResult> deleterole([FromForm] newrole model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Invalid user ID or Role");

            if (!await _userManager.IsInRoleAsync(user, model.Role))
                return BadRequest("User hasn't this role");

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);

            if (!result.Succeeded) { return BadRequest("Sonething went wrong"); }
            return Ok("role is deleted");

        }

    }
}
