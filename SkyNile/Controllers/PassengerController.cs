using BusinessLogic.Models;
using DataAccess.Data;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using SkyNile.DTO;
using SkyNile.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMailingServices? _mailingServices;
        public PassengerController(UserManager<User> userManager, ApplicationDbContext context, IMailingServices mailingServices)
        {
            _userManager = userManager;
            _context = context;
            _mailingServices = mailingServices;
        }

        [HttpPost("booking")]
        [SwaggerOperation(Summary = "For passenger to book ticket")]
        [SwaggerResponse(StatusCodes.Status200OK, "Booking Done")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> booking(Guid UserID, Guid FlightID, int TicketCount)
        {
            var user = await _userManager.FindByIdAsync(UserID.ToString());
            if (user == null)
            {
                return BadRequest("user id invaild");
            }

            var flight = await _context.Flights.FindAsync(FlightID);
            if (flight == null)
            {
                return BadRequest("flight id invaild");
            }
            if (flight.Seatsnum <= 0)
            {
                return BadRequest("no ticket available");
            }
            Ticket ticket = new Ticket()
            {
                Id = Guid.NewGuid(),
                UserId = UserID,
                FlightId = flight.Id,
                TicketCount = TicketCount,
                TotalPrice = flight.Price * TicketCount
            };
            flight.Seatsnum -= 1;
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            string bodyForConfirm = await _mailingServices.PrepareBookingConfirmationBodyAsync(user, flight, ticket);
            await _mailingServices.SendMailAsync(user.Email, "Booking Confirmation", bodyForConfirm);
            string bodyForRemainder = await _mailingServices.PrepareBookingRemainderBodyAsync(user, flight, ticket);
            TimeSpan timeLeftForFlight = flight.DepartureTime - DateTime.Now;
            TimeSpan oneDayHours = new TimeSpan(24, 0, 0);
            if (timeLeftForFlight > oneDayHours)
            {
                timeLeftForFlight.Subtract(oneDayHours);
                BackgroundJob.Schedule(() => _mailingServices.SendMailAsync(user.Email, "Flight departure remainder",
                bodyForRemainder, null), timeLeftForFlight);
            }
            return Ok("Your flight Booked Successfully.");
        }
        [HttpPatch]
        [SwaggerOperation(Summary = "For passenger to update ticket's seat count")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> updatebooking(Guid UserID, Guid TicketID, int TicketCount)
        {
            var t = await _context.Tickets.Include(x => x.Flight).SingleOrDefaultAsync(x => x.Id == TicketID);
            if (t == null)
                return BadRequest("Ticket not found");
            if (t.UserId != UserID)
                return BadRequest("U can't cancel this ticket");
            t.TicketCount = TicketCount;
            t.TotalPrice = TicketCount * t.Flight.Price;

            await _context.SaveChangesAsync();
            return Ok("updating succeed");
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "cancel booking")]
        [SwaggerResponse(StatusCodes.Status200OK, "Booking Canceled")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> cancelbooking(Guid UserID, Guid TicketID)
        {
            var t = await _context.Tickets.FindAsync(TicketID);
            if (t == null)
                return BadRequest("Ticket not found");
            if (t.UserId != UserID)
                return BadRequest("U can't cancel this ticket");
            t.Flight.Seatsnum += 1;
            _context.Tickets.Remove(t);
            await _context.SaveChangesAsync();
            return Ok("Booking Canceled");
        }

    }
}
