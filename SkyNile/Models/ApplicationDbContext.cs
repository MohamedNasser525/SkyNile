using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SkyNile.Models
{
    public class ApplicationDbContext : IdentityDbContext<Crew>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        //public DbSet<Crew> Crews { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

    }
}
