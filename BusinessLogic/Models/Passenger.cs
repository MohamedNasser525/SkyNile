using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Models
{
    public class Passenger 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string PassportNumber { get; set; }
        public string? Image { get; set; }

        List<Ticket> Ticket { get; set; }

    }

}
