using BusinessLogic.Models;
using DataAccess.Data;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;
using Swashbuckle.AspNetCore.Annotations;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public AdminController(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost(Name = "InsertFlight")]
        [SwaggerOperation(Summary = "Insert flight information by an admin.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Request was successfully created.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> InsertFlight([FromBody] FlightAdminCreateDTO flightDTO)
        {
            var airplane = await _context.Airplanes.FirstOrDefaultAsync(a => a.Id == flightDTO.AirplaneId);
            if (airplane == null)
            {
                return BadRequest("There is no such airplane with that specified Id.");
            }
            if (airplane.Capacity < flightDTO.Seatsnum)
            {
                return BadRequest($"Choose available seats count <= {airplane.Capacity}");
            }
            var flight = flightDTO.Adapt<Flight>();
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
            nameof(FlightController.GetFlightById), // Reference method in FlightController
            controllerName: "Flight",
            routeValues: new { id = flight.Id },
            value: flight
        );

        }

        [HttpPut(Name = "UpdateFlightInfo")]
        [SwaggerOperation(Summary = "Update flight information by an admin.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Request was successfully Updated.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The flight is not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateFlight([FromBody] FlightAdminUpdateDTO flightDTO)
        {
            var currentFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightDTO.Id);
            if (currentFlight == null)
            {
                return NotFound();
            }
            flightDTO.Adapt(currentFlight);
            Console.WriteLine("Hello");
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // [HttpDelete("id:guid", Name = "DeleteFlight")]
        // [SwaggerOperation(Summary = "Cancel flight")]
        // [SwaggerResponse(StatusCodes.Status200OK, "Your flight is deleted successfully.")]
        // [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        // [SwaggerResponse(StatusCodes.Status404NotFound, "The desired flight is not found.")]
        // public async Task<IActionResult> DeleteFlight(Guid id){
        //     var flightToDelete = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
        //     if (flightToDelete == null){
        //         return NotFound();
        //     }
        //     _context.Flights.Remove(flightToDelete);
        //     await _context.SaveChangesAsync();
        //     return 
        // }

        [HttpPost("{id:guid}", Name = "AssignCrewFlight")]
        [SwaggerOperation(Summary = "Assign Crew member to a current flight")]
        [SwaggerResponse(StatusCodes.Status201Created, "Request was successfully Created.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The flight or crew member is not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignCrewFlight([FromRoute] Guid id, [FromBody] Guid flightId)
        {
            var crewMember = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            if (crewMember == null)
            {
                return NotFound();
            }
            var crewPossibleFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            if (crewPossibleFlight == null)
            {
                return NotFound();
            }
            // Find any current crew flight in these two ranges [48 before departure, departure] in the current arrival
            // && [departure, 48 after arrival] in the current crew departure
            var departureTime = crewPossibleFlight.DepartureTime;
            var twoDaysSpan = new TimeSpan(48, 0, 0);
            bool isCrewBusy = false;
            if (crewMember.Flight != null)
            {
                isCrewBusy = crewMember.Flight.Any(f =>
                {
                    bool v1 = f.ArrivalTime >= departureTime.Subtract(twoDaysSpan);
                    bool v2 = f.ArrivalTime <= departureTime;
                    bool v3 = f.DepartureTime >= departureTime;
                    bool v4 = f.DepartureTime <= departureTime.Add(twoDaysSpan);
                    return (v1 && v2) || (v3 && v4);
                });
                if (isCrewBusy == true)
                {
                    return BadRequest("The crew is busy, Check their schedule");
                }
            }
            crewMember.Flight.Add(crewPossibleFlight);
            await _context.SaveChangesAsync();
            return Created();
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
            if (!await _userManager.IsInRoleAsync(increw, "Crew"))
                return BadRequest("User not in crew");
            if (!increw.Flight.Any(x => x.Id == FlightID))
                return BadRequest("User in crew doesn't have a Flight yet");
            increw.Flight.Remove(flight);
            _context.SaveChanges();
            return Ok("cancel crow on flight");
        }
    }
}