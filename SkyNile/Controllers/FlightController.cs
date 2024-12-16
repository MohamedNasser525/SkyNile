using BusinessLogic.Models;
using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;
using SkyNile.Services;
using SkyNile.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("TokenBucketLimiter")]
    public class FlightController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISearchService _flightSearchService;
        private readonly UserManager<User> _userManager;
        private readonly ICacheService _cacheService;
        private readonly IFlightSchedulingService _flightScheduler;
        private readonly IFlightServices _flightService;
        public FlightController(IUnitOfWork unitOfWork, ISearchService flightSearchService,
        UserManager<User> userManager, ICacheService cacheService, IFlightSchedulingService flightScheduler,
        IFlightServices flightService)
        {
            _unitOfWork = unitOfWork;
            _flightSearchService = flightSearchService;
            _userManager = userManager;
            _cacheService = cacheService;
            _flightScheduler = flightScheduler;
            _flightService = flightService;
        }

        [HttpGet("GetFlights/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAvailableFlightsAsync([FromRoute] Guid userId, [FromQuery] FlightUserCriteriaDTO flightCriteriaDTO,
        int pageNumber, int pageSize)
        {
            flightCriteriaDTO.ArrivalCountry = flightCriteriaDTO.ArrivalCountry ?? "WORLD";
            flightCriteriaDTO.ArrivalAirport = flightCriteriaDTO.ArrivalAirport ?? "WORLD";
            string cacheKey = $"FlightSearch_{flightCriteriaDTO.DepartureCountry}_{flightCriteriaDTO.DepartureAirport}" +
            $"_{flightCriteriaDTO.ArrivalCountry}_{flightCriteriaDTO.DepartureAirport}_{flightCriteriaDTO.DepartureTime.Date}";
            var cachedFlights = _cacheService.GetData<IEnumerable<Flight>>(cacheKey);
            IEnumerable<Flight> beforeSortList;
            if (cachedFlights is not null)
                beforeSortList = cachedFlights;
            else
            {
                flightCriteriaDTO.ArrivalCountry = null; flightCriteriaDTO.ArrivalAirport = null;
                var expression = _flightSearchService.BuildSearchExpression<Flight>(flightCriteriaDTO);
                beforeSortList = await _unitOfWork.Flights.FindAsync(expression);
                beforeSortList = beforeSortList.Where(f => f.FlightStatus == FlightStatus.Scheduled); ;
                _cacheService.SetData<IEnumerable<Flight>>(cacheKey, beforeSortList);
            }
            foreach (var flight in beforeSortList)
            {
                //Dynamic Price Change
                if (flight.UpdatePrisce == false && flight.DepartureTime <= DateTime.Now.AddHours(24))
                {
                    flight.Price *= 1.3;
                    flight.UpdatePrisce = true;
                }
            }
            await _unitOfWork.CompleteAsync();
            var flightDTO = beforeSortList.Adapt<List<FlightSortDTO>>();
            FlightPreference preference = (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString()))!.FlightPreference;
            var sortedDTO = _flightSearchService.SortFlightsByUserPreference(flightDTO, preference);
            int totalItems = sortedDTO.Count();
            var sortedDTOPage = sortedDTO.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return Ok(sortedDTOPage);
        }

        [HttpGet("{id:guid}", Name = "GetFlightById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetFlightById(Guid id)
        {
            var targetFlight = await _unitOfWork.Flights.GetByIdAsync(id);
            if (targetFlight == null)
                return NotFound("No flight with such information provided");
            //Dynamic Price Change
            if (targetFlight.UpdatePrisce == false && targetFlight.DepartureTime <= DateTime.Now.AddHours(24))
            {
                targetFlight.Price *= 1.3;
                targetFlight.UpdatePrisce = true;
            }
            await _unitOfWork.CompleteAsync();
            return Ok(targetFlight);
        }

        [HttpGet("GetAvailableFlightSchedule/{targetDateTime:DateTime}")]
        [SwaggerOperation(Summary = "Ensure there is no overlapping flights")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAvailableFlightSchedule([FromRoute] DateTime targetDateTime) =>
         Ok(await _flightScheduler.GetAvailableFlightTimeScheduleAsync(targetDateTime));


        [HttpPost(Name = "InsertFlight")]
        [SwaggerOperation(Summary = "Admin Insert flight information.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Request was successfully created.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> InsertFlight([FromBody] FlightAdminCreateDTO flightDTO)
        {
            string cacheKey = $"FlightSearch_{flightDTO.DepartureCountry}_{flightDTO.DepartureAirport}" +
            $"_{flightDTO.ArrivalCountry}_{flightDTO.DepartureAirport}_{flightDTO.DepartureTime.Date}";
            var airplane = await _unitOfWork.Airplanes.GetByIdAsync(flightDTO.AirplaneId);
            if (airplane == null) return BadRequest("There is no such airplane with that specified Id.");
            if (airplane.Capacity < flightDTO.Seatsnum) return BadRequest($"Choose available seats count <= {airplane.Capacity}");
            var flight = flightDTO.Adapt<Flight>();
            await _unitOfWork.Flights.AddAsync(flight);
            await _unitOfWork.CompleteAsync();
            _cacheService.RemoveData(cacheKey);
            // Scheduling a delayed job with Hangfire
            var myData = flight.ArrivalTime - DateTime.Now.AddHours(1);
            var jobId = BackgroundJob.Schedule(
                () => _flightScheduler.DeleteFlightTimeScheduleAsync(flight), myData);
            return CreatedAtAction(
            nameof(FlightController.GetFlightById), // Reference method in FlightController
            controllerName: "Flight",
            routeValues: new { id = flight.Id },
            value: flight
            );
        }

        [HttpPut(Name = "UpdateFlightInfo")]
        [SwaggerOperation(Summary = "Admin Update flight information.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Request was successfully Updated.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The flight is not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateFlight([FromBody] FlightAdminUpdateDTO flightDTO)
        {
            var currentFlight = await _unitOfWork.Flights.GetByIdAsync(flightDTO.Id);
            if (currentFlight == null) return NotFound();
            DateTime oldFlightDepartureDateCached = currentFlight.DepartureTime.Date;
            flightDTO.Adapt(currentFlight);
            await _unitOfWork.CompleteAsync();
            string cacheKey = $"FlightSearch_{currentFlight.DepartureCountry}_{currentFlight.DepartureAirport}" +
            $"_{currentFlight.ArrivalCountry}_{currentFlight.DepartureAirport}_{oldFlightDepartureDateCached}";
            _cacheService.RemoveData(cacheKey);
            return NoContent();
        }

        [HttpPut("DeleteFlight/{flightId:guid}", Name = "DeleteFlight")]
        [SwaggerOperation(Summary = "Admin Cancel flight & Notify Subscribers to this flight by Mail")]
        [SwaggerResponse(StatusCodes.Status200OK, "Your flight is deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not allowed to perform this action.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The desired flight is not found.")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFlight(Guid flightId)
        {
            var flightToDelete = await _unitOfWork.Flights.GetByIdAsync(flightId);
            if (flightToDelete == null) return NotFound();
            // Get all users attached to this flight
            var ticketsToNotify = await _unitOfWork.Tickets.FindAsync(t => t.FlightId == flightId);
            await _flightService.NotifyDeletedFlightSubscribersAsync(flightToDelete, ticketsToNotify);
            await _unitOfWork.CompleteAsync();
            string cacheKey = $"FlightSearch_{flightToDelete.DepartureCountry}_{flightToDelete.DepartureAirport}" +
            $"_{flightToDelete.ArrivalCountry}_{flightToDelete.DepartureAirport}_{flightToDelete.DepartureTime.Date}";
            _cacheService.RemoveData(cacheKey);
            return Ok();
        }
    }
}