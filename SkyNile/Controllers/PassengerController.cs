﻿using BusinessLogic.Models;
using DataAccess.Data;
using Hangfire;
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
    public class PassengerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMailingServices? _mailingServices;
        private readonly ICreateOffers _ICreateOffers;
        public PassengerController(ICreateOffers createOffers,UserManager<User> userManager, ApplicationDbContext context, IMailingServices mailingServices)
        {
            _userManager = userManager;
            _context = context;
            _mailingServices = mailingServices;
            _ICreateOffers = createOffers;
        }
        [HttpGet("GetUserInfo/{userId:guid}")]
        public async Task<IActionResult> GetUserInfo([FromRoute] Guid userId){
            return Ok(await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString()));
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
            //Dynamic Price Change
            if (flight.UpdatePrisce == false && flight.DepartureTime <= DateTime.Now.AddHours(24))
            {
                flight.Price *= 1.3;
                flight.UpdatePrisce = true;
            }
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
                await _context.Offers.AddAsync(offer);
            }
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