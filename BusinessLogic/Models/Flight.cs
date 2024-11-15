﻿namespace BusinessLogic.Models
{
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
        public Airplane Airplane { get; set; } // Navigation property
        public List<User> User { get; set; }
        public List<Ticket> Tickets { get; set;}
    }

}