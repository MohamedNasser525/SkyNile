using BusinessLogic.Models;
using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
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
        private readonly IUnitOfWork _unitOfWork;
        public CrewController(UserManager<User> userManager, ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _context = context;
            _unitOfWork = unitOfWork;
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
            return Ok(flights.Select(x => new { x.DepartureTime, x.DepartureCountry, x.ArrivalTime, x.ArrivalCountry }).OrderBy(x => x.DepartureTime));
        }


        [HttpGet("GetAvailableCrew/{flightId:guid}")]
        [SwaggerOperation(Summary = "Get available Crew members list ready for flying")]
        [SwaggerResponse(StatusCodes.Status200OK, "List retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The flight is not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAvailableCrewList([FromRoute] Guid flightId)
        {
            // Check flight exists
            // Search for Crews entity based on conditions 
            // return the list
            var crewPossibleFlight = await _unitOfWork.Flights.GetByIdAsync(flightId);
            if (crewPossibleFlight == null) return NotFound();
            var allCrewMembers = await _userManager.GetUsersInRoleAsync("Crew");
            DateTime departureTime = crewPossibleFlight.DepartureTime, arrivalTime = crewPossibleFlight.ArrivalTime;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCrewFlight([FromRoute] Guid flightId, [FromBody] Guid id)
        {
            var crewMember = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            var crewPossibleFlight = await _unitOfWork.Flights.GetByIdAsync(flightId);
            if (crewPossibleFlight == null || crewMember == null) return NotFound();
            crewMember.Flight.Add(crewPossibleFlight);
            await _unitOfWork.CompleteAsync();
            return Created();
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "Remove crew member from his Flight")]
        [SwaggerResponse(StatusCodes.Status200OK, "Crew member is removed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelCrewFromFlight(Guid UserID, Guid FlightID)
        {
            var increw = await _userManager.FindByIdAsync(UserID.ToString());
            if (increw == null) return BadRequest("User id invaild");
            var flight = await _unitOfWork.Flights.GetByIdAsync(FlightID);
            if (flight == null) return BadRequest("Flight id invaild");
            if (!await _userManager.IsInRoleAsync(increw, "Crew")) return BadRequest("User not in crew");
            if (!increw.Flight.Any(x => x.Id == FlightID)) return BadRequest("User in crew doesn't have a Flight yet");
            increw.Flight.Remove(flight);
            await _unitOfWork.CompleteAsync();
            return Ok("cancel crow on flight");
        }
    }
}
