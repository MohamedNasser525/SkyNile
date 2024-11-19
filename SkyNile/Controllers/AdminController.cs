using BusinessLogic.Models;
using DataAccess.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost(Name = "InsertFlight")]
        [SwaggerOperation(Summary = "Insert flight information by an admin.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Request was successfully created.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> InsertFlight([FromBody] FlightAdminCreateDTO flightDTO)
        {
            var flight = flightDTO.Adapt<Flight>();
            await _context.Flights.AddAsync(flight);
            _context.SaveChanges();
            return CreatedAtAction(
                nameof(FlightController.GetFlightById), // Reference method in FlightController
                controllerName: "Flight", 
                routeValues: new { id = flight.Id },
                value: flight
            );

        }

        [HttpDelete]
        [SwaggerOperation(Summary = "Remove users crew on his Flight")]
        [SwaggerResponse(StatusCodes.Status200OK, "cancel crow on flight")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "This API is for Admin members only")]

        public async Task<IActionResult> cancelcrowonflight(Guid UserID, Guid FlightID)
        {
            var increw = await _userManager.FindByIdAsync(UserID.ToString());
            if (increw == null)
                return BadRequest("User id invaild");

            var flight = await _context.Flights.FindAsync(FlightID);
            if (flight == null)
                return BadRequest("Flight id invaild");

            if (! await _userManager.IsInRoleAsync(increw, "Crew"))
                return BadRequest("User not in crew");

            if (! increw.Flight.Any(x => x.Id == FlightID))
                return BadRequest("User in crew doesn't have a Flight yet");


            increw.Flight.Remove(flight);
            _context.SaveChanges();

            return Ok("cancel crow on flight");
        }



    }
}
