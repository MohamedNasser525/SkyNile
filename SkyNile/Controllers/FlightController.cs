using BusinessLogic.Models;
using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;
using SkyNile.Services.Interfaces;

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
        public FlightController(IUnitOfWork unitOfWork, ISearchService flightSearchService,
        UserManager<User> userManager, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _flightSearchService = flightSearchService;
            _userManager = userManager;
            _cacheService = cacheService;
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
                beforeSortList = beforeSortList.Where(f => f.FlightStatus == FlightStatus.Scheduled);;
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
    }
}