using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Models
{
    public class User : IdentityUser
    {

        public string PassportNumber { get; set; }
        public string? Image { get; set; }

        public virtual List<Ticket>? Ticket { get; set; }
        public virtual List<Flight>? Flight { get; set; } 
    }

}
