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
        private readonly ApplicationDbContext _db;
        public FlightController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet(Name = "GetFlights")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAvailableFlightsAsync([FromQuery] FlightDTO flightCriteriaDTO)
        {
            var expression = DynamicSearchHelper.BuildSearchExpression<Flight>(flightCriteriaDTO);
            var results = await _db.Flights.Where(f => f.DepartureTime == flightCriteriaDTO.DepartureTime && 
            f.ArrivalTime == flightCriteriaDTO.ArrivalTime).ToListAsync();
            return Ok(results);
        }
    }
}
