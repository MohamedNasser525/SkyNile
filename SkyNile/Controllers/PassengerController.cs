using BusinessLogic.Models;
using DataAccess.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyNile.DTO;
using SkyNile.Services;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public PassengerController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("booking")]
        public async Task<IActionResult> booking(Guid UserID,Guid FlightID,int TicketCount)
        {
            var User = _userManager.FindByIdAsync(UserID.ToString());
            if (User == null)
            {
                return BadRequest("user id invaild");
            }

            var flight = await _context.Flights.FindAsync(FlightID);
            if (flight == null)
            {
                return BadRequest("flight id invaild");
            }

            Ticket t = new Ticket() {
                Id = Guid.NewGuid(),
                UserId = UserID,
                FlightId = flight.Id,
                TicketCount = TicketCount,
                TotalPrice = flight.Price * TicketCount
            };

            _context.Tickets.Add(t);
            await _context.SaveChangesAsync();
            return Ok(t);
        }

    }
}
