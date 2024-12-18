using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public enum FlightStatus
{
    Scheduled,
    Delayed,
    Completed,
    Cancelled,
    SoldOut
}

namespace BusinessLogic.Models
{
    [Index(nameof(DepartureTime))]
    [Index(nameof(ArrivalTime))]
    [Index(nameof(DepartureCountry))]
    [Index(nameof(DepartureAirport))]
    [Index(nameof(ArrivalCountry))]
    [Index(nameof(ArrivalAirport))]

    public class Flight
    {
        public Guid Id { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        [MaxLength(50)]
        public string DepartureCountry { get; set; } = "Hello";
        [MaxLength(50)]
        public string DepartureAirport { get; set; } = "Hello";
        [MaxLength(50)]
        public string ArrivalCountry { get; set; } = "Hello";
        [MaxLength(50)]
        public string ArrivalAirport { get; set; } = "Hello";

        public int Seatsnum { get; set; } = 200;
        public double Price { get; set; } = 500;
        public bool UpdatePrisce { get; set; } = false;
        public FlightStatus FlightStatus { get; set; }

        public Guid AirplaneId { get; set; }
        public virtual Airplane Airplane { get; set; }
        public virtual List<User> User { get; set; } = new List<User>();
        public virtual List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

}