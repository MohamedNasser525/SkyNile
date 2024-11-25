using System;
using BusinessLogic.Models;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SkyNile.Settings;
namespace SkyNile.Services;

public class MailingServices : IMailingServices
{
    private readonly MailSettings _settings;
    public MailingServices(IOptions<MailSettings> settings)
    {
        _settings = settings.Value;
    }
    public async Task<String> PrepareBookingRemainderBodyAsync(User user, Flight flight, Ticket ticket)
    {
        string templatePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\html\\BookingRemainderTemplate.html";
        var str = new StreamReader(templatePath);
        var mailBody = await str.ReadToEndAsync();
        str.Close();
        var body = mailBody.Replace("{TicketId}", ticket.Id.ToString()).Replace("{FlightId}", flight.Id.ToString()).
                    Replace("{DepartureLocation}", flight.DepartureLocation).
                    Replace("{ArrivalLocation}", flight.ArrivalLocation).
                    Replace("{DepartureTime}", flight.DepartureTime.ToString()).
                    Replace("{ArrivalTime}", flight.ArrivalTime.ToString()).
                    Replace("{CustomerName}", user.UserName);
        return body;

    }
    public async Task<String> PrepareBookingConfirmationBodyAsync(User user, Flight flight, Ticket ticket)
    {
        string templatePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\html\\BookingConfirmationTemplate.html";
        var str = new StreamReader(templatePath);
        var mailBody = await str.ReadToEndAsync();
        str.Close();
        var body = mailBody.Replace("{TicketId}", ticket.Id.ToString()).Replace("{FlightId}", flight.Id.ToString()).
                    Replace("{DepartureLocation}", flight.DepartureLocation).
                    Replace("{ArrivalLocation}", flight.ArrivalLocation).
                    Replace("{DepartureTime}", flight.DepartureTime.ToString()).
                    Replace("{ArrivalTime}", flight.ArrivalTime.ToString()).
                    Replace("{PassengerCount}", ticket.TicketCount.ToString()).
                    Replace("{CustomerName}", user.UserName);
        return body;
    }

    public async Task SendMailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachment = null)
    {
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_settings.Email),
            Subject = subject,
        };
        email.To.Add(MailboxAddress.Parse(mailTo));
        var builder = new BodyBuilder();
        if (attachment != null)
        {
            byte[] fileBytes;
            foreach (var file in attachment)
            {
                if (file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }

            }
        }
        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Email));
        using var smtp = new SmtpClient();
        smtp.Connect(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_settings.Email, _settings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}
