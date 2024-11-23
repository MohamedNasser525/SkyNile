using System;
using System.ComponentModel.DataAnnotations;

namespace SkyNile.DTO;

public class MailDTO
{
    [Required, EmailAddress]
    public string toEmail { get; set; }
    [Required]
    public string Subject { get; set; }
    [Required]
    public string Body { get; set; }

    public IList<IFormFile>? Attachments { get; set; }

}
