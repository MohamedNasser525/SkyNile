using BusinessLogic.Models;
using BusinessLogic.Utilities;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FlightController(ApplicationDbContext db)
        {
            _context = db;
        }

        [HttpGet(Name = "GetFlights")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAvailableFlightsAsync([FromQuery] FlightUserCriteriaDTO flightCriteriaDTO)
        {
            var expression = DynamicSearchHelper.BuildSearchExpression<Flight>(flightCriteriaDTO);
            var results = await _context.Flights.Where(expression).ToListAsync();
            return Ok(results);
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
            return Ok(targetFlight);
        }
    }
}
