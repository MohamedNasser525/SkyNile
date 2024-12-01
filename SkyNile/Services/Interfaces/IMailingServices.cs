using System;
using BusinessLogic.Models;

namespace SkyNile.Services;

public interface IMailingServices
{
    public Task SendMailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachment = null);
    public Task<String> PrepareBookingConfirmationBodyAsync(User user, Flight flight, Ticket ticket);
    public Task<String> PrepareBookingRemainderBodyAsync(User user, Flight flight, Ticket ticket);
    public Task<String> PrepareFlightCancellationBodyAsync(User user, Flight flight, Ticket ticket);
}
