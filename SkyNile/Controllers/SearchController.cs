using BusinessLogic.Models;
using BusinessLogic.Utilities;
using DataAccess.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyNile.DTO;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public SearchController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet(Name = "GetFlights")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult GetAvailableFlights([FromQuery] FlightDTO flightCriteriaDTO)
        {
            var expression = DynamicSearchHelper.BuildSearchExpression<Flight>(flightCriteriaDTO);
            //var results = _db.Flights.Where(expression);
            var results = _db.Flights.Where(f => f.DepartureTime == flightCriteriaDTO.DepartureTime &&f.ArrivalTime == flightCriteriaDTO.ArrivalTime);
            return Ok(results);
        }
    }
}
