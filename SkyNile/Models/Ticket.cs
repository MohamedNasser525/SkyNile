namespace SkyNile.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string TicketCount { get; set; }
        public int PassengerId { get; set; }
        public int FlightId { get; set; }
        public double Price { get; set; }

        public Passenger Passenger { get; set; } 
        public Flight Flight { get; set; }      
    }

}
