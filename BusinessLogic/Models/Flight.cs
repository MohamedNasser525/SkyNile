﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public enum FlightStatus
{
    Scheduled,
    Delayed,
    Completed,
    Cancelled
}

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

        [MaxLength(50)]
        public string DepartureLocation { get; set; }
        [MaxLength(50)]
        public string ArrivalLocation { get; set; }

        public int Seatsnum { get; set; }
        public double Price { get; set; }
        public bool UpdatePrisce { get; set; } = false;
        public FlightStatus FlightStatus { get; set; }
        
        public Guid AirplaneId { get; set; }
        public virtual Airplane Airplane { get; set; } 
        public virtual List<User> User { get; set; } = new List<User>();
        public virtual List<Ticket> Tickets { get; set;} = new List<Ticket>();
    }

}