using BusinessLogic.Models;
using DataAccess.Data;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public AdminController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
