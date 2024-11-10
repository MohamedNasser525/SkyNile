using Microsoft.AspNetCore.Identity;

namespace SkyNile.Models
{
    public class Crew : IdentityUser
    {
       
        public List<Flight> Flight { get; set; } 
    }

}
