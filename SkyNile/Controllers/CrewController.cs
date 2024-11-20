using BusinessLogic.Models;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SkyNile.DTO;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrewController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public CrewController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet("NextTrip")]
        [Authorize(Roles = "Crew")]
        [SwaggerOperation(Summary = "Returns all next trip for user in crew")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request was successful", typeof(IEnumerable<FlightCrewResponseDTO>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "User ID is invalid")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User doesn't have any upcoming flight trips")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "This API is for crew members only")]
        public async Task<IActionResult> NextFlight(string UserID)
        {
            var crew = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == UserID);
            if (crew == null)
            {
                return BadRequest("User ID is invalid");
            }

            if (crew.Flight == null)
            {
                return NotFound("u haven't flight trip soon");
            }
            List<Flight> flights = new List<Flight>();
            foreach (var f in crew.Flight)
            {
                if (f.DepartureTime >= DateTime.Now)
                {
                    flights.Add(f);
                }
            }

            return Ok(flights.Select(x => new { x.DepartureTime, x.DepartureLocation, x.ArrivalTime, x.ArrivalLocation }).OrderBy(x => x.DepartureTime));
        }
    }
}
