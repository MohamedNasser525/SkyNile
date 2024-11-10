﻿namespace SkyNile.Models
{

    public class Airplane
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }
        public string Airline { get; set; }
        public List<Flight> Flights { get; set; }
    }

    
}