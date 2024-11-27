using BusinessLogic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Flight>().Property(f => f.FlightStatus).
            HasConversion(v => v.ToString(), v => (FlightStatus)Enum.Parse(typeof(FlightStatus), v)).
            HasDefaultValue(FlightStatus.Scheduled);

            builder.Entity<Ticket>().Property(t => t.TicketStatus).
            HasConversion(v => v.ToString(), v => (TicketStatus)Enum.Parse(typeof(TicketStatus), v)).
            HasDefaultValue(TicketStatus.Pending);
        }



    }
}
