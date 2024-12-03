using BusinessLogic.Models;
using DataAccess.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;
using SkyNile.Services.Interfaces;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISearchService _flightSearchService;
        private readonly UserManager<User> _userManager;
        public FlightController(ApplicationDbContext db, ISearchService flightSearchService, UserManager<User> userManager)
        {
            _context = db;
            _flightSearchService = flightSearchService;
            _userManager = userManager;
        }

        [HttpGet("GetFlights/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAvailableFlightsAsync([FromRoute] Guid userId, [FromQuery] FlightUserCriteriaDTO flightCriteriaDTO)
        {
            var expression = _flightSearchService.BuildSearchExpression<Flight>(flightCriteriaDTO);
            var beforeSortList = await _context.Flights.Where(expression).ToListAsync();           
            foreach (var flight in beforeSortList)
            {
                //Dynamic Price Change
                if (flight.UpdatePrisce == false && flight.DepartureTime <= DateTime.Now.AddHours(24))
                {
                    flight.Price *= 1.3;
                    flight.UpdatePrisce = true;
                }
            }
            await _context.SaveChangesAsync();
            var flightDTO = beforeSortList.Adapt<List<FlightSortDTO>>();
            FlightPreference preference = (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString()))!.FlightPreference;
            var sortedDTO =  _flightSearchService.SortFlightsByUserPreference(flightDTO, preference);
            var flights = sortedDTO.Adapt<List<Flight>>();
            return Ok(flights);
        }

        [HttpGet("{id:guid}", Name = "GetFlightById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetFlightById(Guid id)
        {
            var targetFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            if (targetFlight == null)
                return NotFound("No flight with such information provided");
            //Dynamic Price Change
            if (targetFlight.UpdatePrisce == false && targetFlight.DepartureTime <= DateTime.Now.AddHours(24))
            {
                targetFlight.Price *= 1.3;
                targetFlight.UpdatePrisce = true;
            }
            await _context.SaveChangesAsync();
            return Ok(targetFlight);
        }
    }
}
