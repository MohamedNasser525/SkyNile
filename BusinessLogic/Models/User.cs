using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models
{
    public enum FlightPreference
    {
        CheapestFlight = 1,          // Level 1: Cheapest Flight possible (Wp = 1, Ws = 0)
        MostlyCheap = 2,             // Level 2: More Cheap, Less Speed (Wp = 0.7, Ws = 0.3)
        Balanced = 3,                // Level 3: Balanced (Wp = 0.5, Ws = 0.5)
        MostlyFast = 4,              // Level 4: More Speed, Less Cheap (Wp = 0.3, Ws = 0.7)
        FastestFlight = 5            // Level 5: Fastest Flight possible (Wp = 0, Ws = 1)
    }

    public class User : IdentityUser
    {
        [MaxLength(14)] 
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Passport number must contain only alphanumeric characters.")]
        public string PassportNumber { get; set; }
        public string? Image { get; set; }
        public FlightPreference FlightPreference { get; set; }
        public virtual List<Ticket>? Ticket { get; set; }
        public virtual List<Flight>? Flight { get; set; }
    }


}
