using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Models
{
    [Index(nameof(DepartureTime))]
    [Index(nameof(ArrivalTime))]
    [Index(nameof(DepartureLocation))]
    [Index(nameof(ArrivalLocation))]
    public class Flight
    {
        public Guid Id { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public string DepartureLocation { get; set; }
        public string ArrivalLocation { get; set; }

        public int Seatsnum { get; set; }
        public double Price { get; set; }

        public Guid AirplaneId { get; set; }
        public virtual Airplane Airplane { get; set; } 
        public virtual List<User> User { get; set; }
        public virtual List<Ticket> Tickets { get; set;}
    }

}