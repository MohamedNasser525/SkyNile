using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Models
{
    public class Crew : IdentityUser
    {
       
        public List<Flight> Flight { get; set; } 
    }

}
