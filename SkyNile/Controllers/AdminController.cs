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
using SkyNile.Services;
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
        private readonly IFlightSchedulingService _flightScheduler;
        private readonly IMailingServices _mailingServices;
        public AdminController(ApplicationDbContext context, IMapper mapper,
        UserManager<User> userManager, IFlightSchedulingService flightScheduler, IMailingServices mailingServices)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _flightScheduler = flightScheduler;
            _mailingServices = mailingServices;
        }

        [HttpGet("GetAvailableFlightSchedule/{targetDateTime:DateTime}")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableFlightSchedule([FromRoute] DateTime targetDateTime)
        {
            return Ok(await _flightScheduler.GetAvailableFlightTimeScheduleAsync(targetDateTime));
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

        [HttpPut("DeleteFlight/{flightId:guid}", Name = "DeleteFlight")]
        [SwaggerOperation(Summary = "Cancel flight")]
        [SwaggerResponse(StatusCodes.Status200OK, "Your flight is deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The desired flight is not found.")]
        public async Task<IActionResult> DeleteFlight(Guid flightId)
        {
            var flightToDelete = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            if (flightToDelete == null)
            {
                return NotFound();
            }
            // Get all users attached to this flight
            var ticketsToNotify = _context.Tickets.Where(t => t.FlightId == flightId);
            foreach(var ticket in ticketsToNotify){
                ticket.TicketStatus = TicketStatus.CancelledWithRefund;
                var userIdToNotify = ticket.UserId;
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userIdToNotify.ToString());
                var body = await _mailingServices.PrepareFlightCancellationBodyAsync(user, flightToDelete, ticket);
                await _mailingServices.SendMailAsync(user.Email, "Flight Cancellation Notice", body,null);
            }
            flightToDelete.FlightStatus = FlightStatus.Cancelled;
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("GetAvailableCrew/{flightId:guid}")]
        [SwaggerOperation(Summary = "Get available Crew members list ready for flying")]
        [SwaggerResponse(StatusCodes.Status200OK, "List retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The flight is not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableCrewList([FromRoute] Guid flightId)
        {
            // Check flight exists
            // Search for Crews entity based on conditions 
            // return the list
            var crewPossibleFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            if (crewPossibleFlight == null)
            {
                return NotFound();
            }
            var allCrewMembers = await _userManager.GetUsersInRoleAsync("Crew");
            var departureTime = crewPossibleFlight.DepartureTime;
            var arrivalTime = crewPossibleFlight.ArrivalTime;
            var twoDays = new TimeSpan(48, 0, 0);
            var AvailableCrewMembers = allCrewMembers.Where(c =>
            {
                bool isCrewBusy = false;
                if (c.Flight != null)
                {
                    isCrewBusy = c.Flight.Any(f =>
                        {
                            bool beforeFlight = f.ArrivalTime >= departureTime.Subtract(twoDays) && f.ArrivalTime <= departureTime;
                            bool inRangeFlight = f.DepartureTime >= departureTime && f.DepartureTime <= arrivalTime;
                            bool afterFlight = f.DepartureTime >= arrivalTime && f.DepartureTime - arrivalTime < twoDays;
                            return beforeFlight || inRangeFlight || afterFlight;
                        });
                }
                return !isCrewBusy;
            }).ToList();
            return Ok(AvailableCrewMembers);
        }


        [HttpPost("{flightId:guid}", Name = "AssignCrewFlight")]
        [SwaggerOperation(Summary = "Assign Crew member to a current flight")]
        [SwaggerResponse(StatusCodes.Status201Created, "Request was successfully Created.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The flight or crew member is not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignCrewFlight([FromRoute] Guid flightId, [FromBody] Guid id)
        {
            var crewMember = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            var crewPossibleFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            if (crewPossibleFlight == null || crewMember == null)
            {
                return NotFound();
            }
            crewMember.Flight.Add(crewPossibleFlight);
            await _context.SaveChangesAsync();
            return Created();
        }



        [HttpDelete]
        [SwaggerOperation(Summary = "Remove crew member from his Flight")]
        [SwaggerResponse(StatusCodes.Status200OK, "Crew member is removed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        public async Task<IActionResult> CancelCrewFromFlight(Guid UserID, Guid FlightID)
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