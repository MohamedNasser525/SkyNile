using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyNile.DTO;
using SkyNile.Services;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailingController : ControllerBase
    {
        private readonly IMailingServices _mailingService;
        public MailingController(IMailingServices mailingServices)
        {
            _mailingService = mailingServices;
        }
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromForm] MailDTO mailDTO)
        {
            await _mailingService.SendMailAsync(mailDTO.toEmail, mailDTO.Subject, mailDTO.Body, mailDTO.Attachments);
            return Ok();
        }
        [HttpPost("BookingConfirmation")]
        public async Task<IActionResult> SendBookingConfirmation([FromForm] MailDTO mailDTO)
        {
            string templatePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\html\\BookingConfirmationTemplate.html";
            var str = new StreamReader(templatePath);
            var mailBody = await str.ReadToEndAsync();
            str.Close();
            var body = mailBody.Replace("{TicketId}", "Abc21239402195").Replace("{FlightId}", "Abc21239402195").
                        Replace("{DepartureLocation}", "Cairo International Airport").
                        Replace("{ArrivalLocation}", "USA International Airport").
                        Replace("{DepartureTime}", "2024-10-10").
                        Replace("{ArrivalTime}", "2024-12-12").
                        Replace("{PassengerCount}", "5 Passengers");
            await _mailingService.SendMailAsync(mailDTO.toEmail, "Booking Confirmation", body);
            return Ok();
        }
    }
}
