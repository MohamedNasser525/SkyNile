using BusinessLogic.Models;
using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using SkyNile.DTO;
using SkyNile.Services;
using SkyNile.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PassengerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailingServices? _mailingServices;
        private readonly ICreateOffers _ICreateOffers;
        private readonly IUnitOfWork _unitOfWork;
        public PassengerController(ICreateOffers createOffers,UserManager<User> userManager, ApplicationDbContext context, 
        IMailingServices mailingServices, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mailingServices = mailingServices;
            _ICreateOffers = createOffers;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetUserInfo/{userId:guid}")]
        public async Task<IActionResult> GetUserInfo([FromRoute] Guid userId) =>
            Ok(await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString()));
        

        [HttpGet("GetUserBooking/{userId:guid}")]
        public async Task<IActionResult> GetUserBookedTickets([FromRoute] Guid userId, [FromQuery] TicketStatus status) {
            var allUserTickets = await _unitOfWork.Tickets.FindAsync(t => t.UserId == userId && t.TicketStatus == status);
            return Ok(allUserTickets.Select(t => new { ticketId = t.Id, flightId = t.FlightId, ticketCount = t.TicketCount,
             totalPrice = t.TotalPrice, departureCountry = t.Flight.DepartureCountry, arrivalCountry = t.Flight.ArrivalCountry, 
             departureTime = t.Flight.DepartureTime, arrivalTime = t.Flight.ArrivalTime, ticketStatus = t.TicketStatus}));
        }
            
        [HttpPost("booking")]
        [SwaggerOperation(Summary = "For passenger to book ticket")]
        [SwaggerResponse(StatusCodes.Status200OK, "Booking Done")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BookFlight(Guid UserID, Guid FlightID, int TicketCount)
        {
            var user = await _userManager.FindByIdAsync(UserID.ToString());
            if (user == null)
                return BadRequest("user id invaild");
            
            var flight = await _unitOfWork.Flights.GetByIdAsync(FlightID);
            if (flight == null)
                return BadRequest("flight id invaild");
            
            if (flight.Seatsnum == 0)
                return BadRequest("no ticket available");
            
            else if (flight.Seatsnum == 1)
                flight.FlightStatus = FlightStatus.SoldOut;
            
            Ticket ticket = new Ticket()
            {
                Id = Guid.NewGuid(),
                UserId = UserID,
                FlightId = flight.Id,
                TicketCount = TicketCount,
                TotalPrice = flight.Price * TicketCount
            };
            // create offer
            Offer offer = null;
            if (ticket.TotalPrice > 1000 || ticket.TicketCount > 3)
            {
                offer = new Offer()
                {
                    Id = Guid.NewGuid(),
                    Code = _ICreateOffers.GenerateRandomString(10),
                    Discount = _ICreateOffers.GenerateRandomDouble(),
                    Ticket = ticket,
                };
                await _unitOfWork.Offers.AddAsync(offer);
            }
            flight.Seatsnum -= 1;
            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.CompleteAsync();
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
        public async Task<IActionResult> UpdateBookingDetails(Guid UserID, Guid TicketID, int TicketCount)
        {
            var t = await _unitOfWork.Tickets.GetByIdAsync(TicketID);
            if (t == null)
                return BadRequest("Ticket not found");
            if (t.UserId != UserID)
                return BadRequest("U can't cancel this ticket");
            t.TicketCount = TicketCount;
            t.TotalPrice = TicketCount * t.Flight.Price;

            await _unitOfWork.CompleteAsync();
            return Ok("updating succeed");
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "cancel booking")]
        [SwaggerResponse(StatusCodes.Status200OK, "Booking Canceled")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelBooking(Guid UserID, Guid TicketID)
        {
            var t = await _unitOfWork.Tickets.GetByIdAsync(TicketID);
            if (t == null)
                return BadRequest("Ticket not found");
            if (t.UserId != UserID)
                return BadRequest("U can't cancel this ticket");
            t.Flight.Seatsnum += 1;
            _unitOfWork.Tickets.Delete(t);
            await _unitOfWork.CompleteAsync();
            return Ok("Booking Canceled");
        }

    }
}