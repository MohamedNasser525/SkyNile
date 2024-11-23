using System;

namespace SkyNile.Services;

public interface IMailingServices
{
    public Task SendMailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachment = null);
}
