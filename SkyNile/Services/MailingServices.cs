using System;
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
        // // Log or handle the exception
        // Console.WriteLine($"Email sending failed: {ex.Message}");
        // Console.WriteLine($"{_settings.Email} && {_settings.Password}");
    }
}
